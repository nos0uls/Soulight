using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace SoulightBridge
{
    public class BeelightHelper
    {
        private static bool _initialized = false;
        private static MethodInfo _genRgbTransfer;
        private static Type _rgbType;
        private static FieldInfo _fieldR, _fieldG, _fieldB;
        private static BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;

        public static bool Init(string beelightPath)
        {
            if (_initialized) return true;
            try
            {
                if (!File.Exists(beelightPath)) return false;

                string bDir = Path.GetDirectoryName(beelightPath);
                AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
                {
                    string p = Path.Combine(bDir, a.Name.Split(',')[0] + ".dll");
                    return File.Exists(p) ? Assembly.LoadFrom(p) : null;
                };

                Assembly asm = Assembly.LoadFrom(beelightPath);
                Type lpCtrl = null;
                
                foreach (Type t in asm.GetTypes())
                {
                    if (t.Name == "LProtocolCtrl") lpCtrl = t;
                    if (t.FullName != null && t.FullName.Contains("LProtocolBase+RGB")) _rgbType = t;
                }

                if (lpCtrl == null || _rgbType == null) return false;

                _fieldR = _rgbType.GetField("R", _flags);
                _fieldG = _rgbType.GetField("G", _flags);
                _fieldB = _rgbType.GetField("B", _flags);

                BindingFlags flagsAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                foreach (MethodInfo m in lpCtrl.GetMethods(flagsAll))
                {
                    if (m.Name == "GenRGBTransferPackage" && m.GetParameters().Length == 2)
                    {
                        _genRgbTransfer = m;
                        break;
                    }
                }

                if (_genRgbTransfer == null) return false;

                _initialized = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SoulightBridge] Init error: " + ex.Message);
                return false;
            }
        }

        public static byte[] MakeRGBTransferPacket(byte[] flatRgb)
        {
            if (!_initialized || flatRgb == null) return null;
            try
            {
                int ledCount = flatRgb.Length / 3;
                if (ledCount > 75) ledCount = 75; // hardware max

                Array arr = Array.CreateInstance(_rgbType, 75);

                for (int i = 0; i < 75; i++)
                {
                    object rgb = FormatterServices.GetUninitializedObject(_rgbType);
                    byte r = 0, g = 0, b = 0;
                    
                    if (i < ledCount)
                    {
                        r = flatRgb[i * 3];
                        g = flatRgb[i * 3 + 1];
                        b = flatRgb[i * 3 + 2];
                    }

                    _fieldR.SetValue(rgb, r);
                    _fieldG.SetValue(rgb, g);
                    _fieldB.SetValue(rgb, b);
                    arr.SetValue(rgb, i);
                }

                // channelMark=0xFF
                object result = _genRgbTransfer.Invoke(null, new object[] { arr, (byte)0xFF });
                return result as byte[];
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SoulightBridge] Gen error: " + ex.Message);
                return null;
            }
        }
    }
}
