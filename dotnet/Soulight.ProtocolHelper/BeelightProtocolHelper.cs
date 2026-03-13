using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Soulight.ProtocolHelper
{
    // Этот класс хранит уже распарсенный результат SyncConfigAck.
    // Python-код сможет читать его как обычный простой объект,
    // без работы с .NET delegate и внутренними типами Beelight.
    public sealed class SyncConfigInfo
    {
        // Общее число пикселей, которое сообщил контроллер.
        public int SumPixel { get; set; }

        // Количество каналов/линий, которые видит устройство.
        public int Channels { get; set; }

        // Сколько пикселей привязано к каждому каналу.
        public int[] ChannelPixel { get; set; }

        // Метод создаёт безопасную копию данных.
        // Это нужно, чтобы внешний код не менял внутреннее состояние helper.
        public SyncConfigInfo Clone()
        {
            return new SyncConfigInfo
            {
                SumPixel = SumPixel,
                Channels = Channels,
                ChannelPixel = ChannelPixel != null ? (int[])ChannelPixel.Clone() : Array.Empty<int>(),
            };
        }
    }

    // Этот helper изолирует всю сложную .NET interop логику.
    // Python потом будет работать уже с простыми методами,
    // а не напрямую с reflection и delegate типами Beelight.
    public sealed class BeelightProtocolHelper : IDisposable
    {
        // Это путь по умолчанию к установленному Beelight.
        // Его можно переопределить из Python, если понадобится другая папка.
        public const string DefaultBeelightDir = @"C:\Program Files (x86)\Beelight\Beelight V3.0";

        private readonly object _syncLock = new object();
        private readonly ManualResetEventSlim _syncConfigEvent = new ManualResetEventSlim(false);

        private readonly string _beelightDir;
        private readonly string _beelightExe;

        private bool _disposed;
        private bool _ready;
        private bool _resolveHooked;

        private Assembly _assembly;

        // Ниже лежат runtime-типы и методы из Beelight.exe.
        // Мы находим их через reflection, потому что проект не ссылается
        // напрямую на обфусцированную сборку.
        private Type _lProtocolType;
        private Type _lProtocolBaseType;
        private Type _lProtocolCtrlType;
        private Type _lProtocolSyncConfigType;
        private Type _lProtocolSyncRgbType;
        private Type _lpSyncConfigAckType;
        private Type _lpWkModeType;
        private Type _lpCmdType;
        private Type _lpAttrType;

        // Это runtime-экземпляры протокольных объектов.
        private object _protocol;
        private object _syncConfigProtocol;
        private object _syncRgbProtocol;

        // Это reflection-методы, которые helper будет вызывать дальше.
        private MethodInfo _protocolRecieverMethod;
        private MethodInfo _addSyncConfigAckListenerMethod;
        private MethodInfo _removeSyncConfigAckListenerMethod;
        private MethodInfo _genSyncConfigPackageMethod;
        private MethodInfo _genProtocolSyncRgbMethod;
        private MethodInfo _genSwitchPackageMethod;
        private MethodInfo _genWorkModePackageMethod;
        private MethodInfo _genFramePackageMethod;

        // Этот delegate создаётся уже внутри C#.
        // Именно это и решает проблему, с которой Python упирался ранее.
        private Delegate _syncConfigAckDelegate;

        // Здесь хранится последний успешный SyncConfigAck.
        private SyncConfigInfo _lastSyncConfig;

        public BeelightProtocolHelper() : this(DefaultBeelightDir)
        {
        }

        public BeelightProtocolHelper(string beelightDir)
        {
            _beelightDir = string.IsNullOrWhiteSpace(beelightDir) ? DefaultBeelightDir : beelightDir;
            _beelightExe = Path.Combine(_beelightDir, "Beelight.exe");
        }

        // Этот флаг показывает, что helper готов к работе.
        public bool IsReady => _ready;

        // Этот флаг нужен Python-коду, чтобы быстро понять,
        // был ли уже успешно получен SyncConfigAck.
        public bool HasSyncConfig
        {
            get
            {
                lock (_syncLock)
                {
                    return _lastSyncConfig != null;
                }
            }
        }

        // Это безопасная копия последнего ack.
        // Так Python получает готовые данные, но не может случайно
        // испортить внутреннее состояние helper.
        public SyncConfigInfo LastSyncConfig
        {
            get
            {
                lock (_syncLock)
                {
                    return _lastSyncConfig != null ? _lastSyncConfig.Clone() : null;
                }
            }
        }

        // Инициализация выполняет всю тяжёлую работу один раз:
        // загрузку сборки, поиск типов, создание runtime-объектов
        // и подписку на SyncConfigAck listener.
        public bool Initialize()
        {
            ThrowIfDisposed();

            if (_ready)
            {
                return true;
            }

            if (!File.Exists(_beelightExe))
            {
                throw new FileNotFoundException("Beelight.exe not found", _beelightExe);
            }

            HookAssemblyResolve();
            _assembly = Assembly.LoadFrom(_beelightExe);

            FindTypes();
            FindMethods();
            CreateRuntimeObjects();
            HookSyncConfigListener();

            _ready = true;
            return true;
        }

        // Этот метод очищает состояние ожидания перед новой попыткой handshake.
        // Его удобно вызывать прямо перед отправкой нового SyncConfigReq.
        public void ResetSyncConfigState()
        {
            ThrowIfDisposed();

            lock (_syncLock)
            {
                _lastSyncConfig = null;
                _syncConfigEvent.Reset();
            }
        }

        // Этот метод создаёт wire-format пакет heartbeat.
        // Он нужен для базового handshake контроллера до SyncConfig.
        public byte[] CreateHeartbeatPacket()
        {
            EnsureReady();

            object attrReq = Enum.ToObject(_lpAttrType, 0);
            object cmdHeartbeat = Enum.ToObject(_lpCmdType, 0);
            object result = _genFramePackageMethod.Invoke(null, new object[] { attrReq, cmdHeartbeat, Array.Empty<byte>() });
            return result as byte[];
        }

        // Пакет включения контроллера.
        public byte[] CreateSwitchPacket(bool enabled, byte channel)
        {
            EnsureReady();
            object result = _genSwitchPackageMethod.Invoke(null, new object[] { enabled, channel });
            return result as byte[];
        }

        // Пакет перевода устройства в PC mode.
        // Это обязательная часть базового handshake перед screen mirroring.
        public byte[] CreatePcWorkModePacket(byte channel)
        {
            EnsureReady();
            object pcMode = Enum.ToObject(_lpWkModeType, 0);
            object result = _genWorkModePackageMethod.Invoke(null, new object[] { pcMode, channel });
            return result as byte[];
        }

        // Этот метод генерирует SyncConfig request.
        // Дальше Python отправляет пакет в serial и начинает кормить helper
        // входящими байтами через FeedReceivedBytes().
        public byte[] CreateSyncConfigRequest()
        {
            EnsureReady();
            return _genSyncConfigPackageMethod.Invoke(_syncConfigProtocol, null) as byte[];
        }

        // Этот метод принимает очередную порцию байтов из COM-порта.
        // Внутри вызывается родной parser Beelight, а listener сам поднимет ack,
        // если в потоке встретится нужный пакет.
        public bool FeedReceivedBytes(byte[] data)
        {
            EnsureReady();

            if (data == null || data.Length == 0)
            {
                return HasSyncConfig;
            }

            _protocolRecieverMethod.Invoke(_protocol, new object[] { data });
            return HasSyncConfig;
        }

        // Этот метод позволяет подождать ack в blocking-сценарии.
        // Он не обязателен для работы, но удобен для тестов и диагностики.
        public bool WaitForSyncConfig(int timeoutMs)
        {
            ThrowIfDisposed();
            return _syncConfigEvent.Wait(timeoutMs);
        }

        // Этот метод создаёт SyncRGB пакет из плоского RGB массива.
        // Формат массива простой: [r0, g0, b0, r1, g1, b1, ...].
        public byte[] CreateSyncRgbPacket(byte rows, byte columns, byte[] rgbBytes)
        {
            EnsureReady();

            if (rgbBytes == null)
            {
                throw new ArgumentNullException(nameof(rgbBytes));
            }

            if (rgbBytes.Length % 3 != 0)
            {
                throw new ArgumentException("rgbBytes length must be divisible by 3", nameof(rgbBytes));
            }

            Color[] colors = new Color[rgbBytes.Length / 3];
            for (int i = 0; i < colors.Length; i++)
            {
                int baseIndex = i * 3;
                colors[i] = Color.FromArgb(rgbBytes[baseIndex], rgbBytes[baseIndex + 1], rgbBytes[baseIndex + 2]);
            }

            object result = _genProtocolSyncRgbMethod.Invoke(_syncRgbProtocol, new object[] { rows, columns, colors });
            return result as byte[];
        }

        // Этот handler вызывается уже внутри нативного .NET delegate.
        // Здесь мы просто сохраняем полезные поля ack в обычный DTO.
        private void OnSyncConfigAck(object protocolObject, int sumPixel, int channels, int[] channelPixel)
        {
            SyncConfigInfo info = new SyncConfigInfo
            {
                SumPixel = sumPixel,
                Channels = channels,
                ChannelPixel = channelPixel != null ? (int[])channelPixel.Clone() : Array.Empty<int>(),
            };

            lock (_syncLock)
            {
                _lastSyncConfig = info;
                _syncConfigEvent.Set();
            }
        }

        // Подключаем обработчик зависимостей только один раз.
        // Это нужно, чтобы Beelight.exe находил свои DLL рядом с собой.
        private void HookAssemblyResolve()
        {
            if (_resolveHooked)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            _resolveHooked = true;
        }

        private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            string fileName = args.Name.Split(',')[0] + ".dll";
            string dependencyPath = Path.Combine(_beelightDir, fileName);
            return File.Exists(dependencyPath) ? Assembly.LoadFrom(dependencyPath) : null;
        }

        // Находим все типы, которые нужны helper для работы.
        // Если хоть один критичный тип не найден, лучше упасть сразу,
        // чем потом ловить тихие ошибки в рантайме.
        private void FindTypes()
        {
            foreach (Type type in _assembly.GetTypes())
            {
                if (type.Name == "LProtocol") _lProtocolType = type;
                else if (type.Name == "LProtocolBase") _lProtocolBaseType = type;
                else if (type.Name == "LProtocolCtrl") _lProtocolCtrlType = type;
                else if (type.Name == "LProtocolSyncConfig") _lProtocolSyncConfigType = type;
                else if (type.Name == "LProtocolSyncRGB") _lProtocolSyncRgbType = type;
                else if (type.Name == "LP_WK_MODE") _lpWkModeType = type;
                else if (type.Name == "LP_CMD") _lpCmdType = type;
                else if (type.Name == "LP_ATTR") _lpAttrType = type;
                else if (type.Name == "LP_SyncConfigAck") _lpSyncConfigAckType = type;
            }

            RequireType(_lProtocolType, nameof(_lProtocolType));
            RequireType(_lProtocolBaseType, nameof(_lProtocolBaseType));
            RequireType(_lProtocolCtrlType, nameof(_lProtocolCtrlType));
            RequireType(_lProtocolSyncConfigType, nameof(_lProtocolSyncConfigType));
            RequireType(_lProtocolSyncRgbType, nameof(_lProtocolSyncRgbType));
            RequireType(_lpSyncConfigAckType, nameof(_lpSyncConfigAckType));
            RequireType(_lpWkModeType, nameof(_lpWkModeType));
            RequireType(_lpCmdType, nameof(_lpCmdType));
            RequireType(_lpAttrType, nameof(_lpAttrType));
        }

        // Ищем ровно те reflection-методы, которые нужны helper.
        // Мы не открываем весь API Beelight, только узкий и понятный слой.
        private void FindMethods()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            _protocolRecieverMethod = RequireMethod(_lProtocolType, "ProtocolReciever", flags, 1);
            _addSyncConfigAckListenerMethod = RequireMethod(_lProtocolType, "AddSyncConfigAckListener", flags, 1);
            _removeSyncConfigAckListenerMethod = RequireMethod(_lProtocolType, "RemoveSyncConfigAckListener", flags, 1);
            _genSyncConfigPackageMethod = RequireMethod(_lProtocolSyncConfigType, "GenSyncConfigPackage", flags, 0);
            _genProtocolSyncRgbMethod = RequireMethod(_lProtocolSyncRgbType, "GenProtocolSyncRGB", flags, 3);
            _genSwitchPackageMethod = RequireMethod(_lProtocolCtrlType, "GenSwitchPackage", flags, 2);
            _genFramePackageMethod = RequireMethod(_lProtocolBaseType, "GenFramePackage", flags, 3);

            foreach (MethodInfo method in _lProtocolCtrlType.GetMethods(flags))
            {
                if (method.Name == "GenWorkModePackage" && method.GetParameters().Length == 2)
                {
                    _genWorkModePackageMethod = method;
                    break;
                }
            }

            if (_genWorkModePackageMethod == null)
            {
                throw new MissingMethodException(_lProtocolCtrlType.FullName, "GenWorkModePackage(mode, channel)");
            }
        }

        // Создаём runtime-объекты один раз и затем переиспользуем.
        // Это уменьшает накладные расходы во время обычной работы приложения.
        private void CreateRuntimeObjects()
        {
            _protocol = Activator.CreateInstance(_lProtocolType, new object[] { 0 });
            _syncConfigProtocol = Activator.CreateInstance(_lProtocolSyncConfigType, new object[] { _protocol });
            _syncRgbProtocol = Activator.CreateInstance(_lProtocolSyncRgbType);
        }

        // Здесь мы создаём реальный .NET delegate нужного типа.
        // Именно этот шаг обходит ограничение Python при создании LP_SyncConfigAck.
        private void HookSyncConfigListener()
        {
            MethodInfo handlerMethod = GetType().GetMethod(nameof(OnSyncConfigAck), BindingFlags.NonPublic | BindingFlags.Instance);
            _syncConfigAckDelegate = Delegate.CreateDelegate(_lpSyncConfigAckType, this, handlerMethod, true);

            if (_syncConfigAckDelegate == null)
            {
                throw new InvalidOperationException("Failed to create LP_SyncConfigAck delegate");
            }

            _addSyncConfigAckListenerMethod.Invoke(_protocol, new object[] { _syncConfigAckDelegate });
        }

        private static MethodInfo RequireMethod(Type type, string name, BindingFlags flags, int parameterCount)
        {
            foreach (MethodInfo method in type.GetMethods(flags))
            {
                if (method.Name == name && method.GetParameters().Length == parameterCount)
                {
                    return method;
                }
            }

            throw new MissingMethodException(type.FullName, name);
        }

        private static void RequireType(Type type, string fieldName)
        {
            if (type == null)
            {
                throw new TypeLoadException($"Required Beelight type was not found: {fieldName}");
            }
        }

        private void EnsureReady()
        {
            ThrowIfDisposed();

            if (!_ready)
            {
                throw new InvalidOperationException("Helper is not initialized");
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(BeelightProtocolHelper));
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (_protocol != null && _removeSyncConfigAckListenerMethod != null && _syncConfigAckDelegate != null)
                {
                    _removeSyncConfigAckListenerMethod.Invoke(_protocol, new object[] { _syncConfigAckDelegate });
                }
            }
            catch
            {
                // Здесь мы специально не роняем приложение.
                // Dispose должен быть максимально безопасным даже при частичной инициализации.
            }

            _syncConfigEvent.Dispose();
            _disposed = true;
        }
    }
}
