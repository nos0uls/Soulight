# Ghidra Guide: Анализ beelightLib.dll для взлома Beelight протокола

## Что ищем

Из криптоанализа известно:
- Шифрование = XOR с 3-байтовым ключом `key3 = [K0, K1, K2]`
- Keystream: `ks[i] = key3[i % 3]` (весь пакет одним ключом)
- Ключ меняется каждый пакет, детерминированно зависит от nonce или internal state
- Функции DLL экспортируют: `get_scen_package`, `get_connect_package`, `unpack_package`

Цель: найти как `key3` вычисляется из входных параметров или внутреннего состояния.

---

## Шаг 1: Установка Ghidra

1. Скачать Ghidra с https://ghidra-sre.org/
2. Распаковать архив (не требует установки)
3. Запустить `ghidraRun.bat` (Windows) или `ghidraRun` (Linux/Mac)
4. Создать новый Project: **File → New Project → Non-Shared Project**
5. Выбрать папку для проекта, назвать `beelight_analysis`

---

## Шаг 2: Импорт DLL

1. **File → Import File** (или drag-and-drop в окно проекта)
2. Выбрать: `c:\Users\n0souls\Documents\GitHub\Soulight\Новая папка\Dumps\beelightLib.dll`
3. В диалоге Import:
   - **Format**: PE (Windows)
   - **Language**: `x86:LE:32:default:gcc` (x86 32-bit Little Endian)
   - Нажать **OK**

---

## Шаг 3: Auto-Analysis

1. Откроется окно с вопросом "Analyze beelightLib.dll now?" — нажать **Yes**
2. В окне **Auto Analyze** оставить все галочки по умолчанию, нажать **Analyze**
3. Ждать завершения (1-2 минуты на 32-bit DLL)
4. После анализа откроется **CodeBrowser** — основное окно работы

---

## Шаг 4: Навигация к функциям

### Вариант A: Через Symbol Tree (самый простой)

1. В левой панели открыть **Window → Symbol Tree**
2. Развернуть раздел **Exports**
3. Найти и двойной клик на:
   - `get_scen_package` — приоритет #1 (известны все аргументы)
   - `get_connect_package` — приоритет #2 (проще всего)
   - `unpack_package` — приоритет #3 (обратное преобразование)

### Вариант B: Через адрес (если знаем RVA)

1. Нажать **G** (Go to...)
2. Ввести адрес: `0x100015b0` для `get_scen_package`
3. Нажать **OK**

---

## Шаг 5: Декомпиляция (получение pseudo-C)

1. Открыть функцию (двойной клик в Symbol Tree)
2. В панели Listing (дизассемблер) правый клик на функции
3. Выбрать **Decompile** (или просто открыть панель **Decompile** снизу)
4. В панели **Decompile** появится pseudo-C код

### Экспорт кода:

1. В панели Decompile: правый клик → **Copy → Copy All** (или Ctrl+A, Ctrl+C)
2. Вставить в текстовый файл `get_scen_package.txt`
3. Повторить для `get_connect_package` и `unpack_package`

---

## Шаг 6: Что искать в коде (чеклист)

### Признаки генерации key3:

1. **MD5 хеширование** — ищи паттерн констант:
   ```c
   // MD5 init constants
   0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476
   ```
   Или вызовы типа `MD5Init`, `MD5Update`, `MD5Final`

2. **XOR операции** — ищи `^` или `xor`:
   ```c
   *buffer = *buffer ^ key_byte;
   // или
   result = a ^ b;
   ```

3. **Циклы по 3 байтам** — ищи `for (i = 0; i < 3; i++)` или `i % 3`

4. **Использование входных параметров**:
   - `get_scen_package(scenid, dimmer, R, G, B, power, pout)`
   - Ищи где используются `R`, `G`, `B` — это цвет!

5. **Счётчик/Nonce**:
   - Глобальные переменные (псевдо-C покажет как `DAT_1000xxxx`)
   - Инкременты `counter++` или `++DAT_xxxxxx`
   - Статические переменные внутри функции

6. **Строка ключа** — ищи `"diy-d6lfwphynoh3"` или массив байт:
   ```c
   char key[] = {0x64, 0x69, 0x79, ...}; // "diy"
   ```

### Ключевые паттерны для поиска (Ctrl+Shift+E — Search → For String):

- `"diy"` — начало device key
- `"md5"` — функция хеширования
- `"xor"`, `"XOR"` — непосредственно XOR
- `0x67452301` — MD5 init constant (Search → For Scalars)

---

## Шаг 7: Анализ конкретных функций

### get_scen_package (0x100015b0) — приоритет #1

Сигнатура из .NET: `(byte scenid, byte dimmer, byte red, byte green, byte blue, byte power, byte* pout)`

Ищи:
1. Как `red`, `green`, `blue` попадают в выходной буфер
2. XOR с каким-то ключом перед записью
3. Генерацию 3-байтового ключа (R, G, B могут быть частью ключа!)

### get_connect_package (0x10001720) — приоритет #2

Самая простая функция — нет цветовых параметров.
Ищи:
1. Фиксированный пакет, который шифруется
2. Откуда берётся ключ для шифрования

### unpack_package (0x10001740) — приоритет #3

Обратное преобразование — расшифровка входящих пакетов.
Ищи:
1. XOR с тем же ключом (проверка: `plain = cipher ^ key`)
2. Алгоритм генерации ключа (должен совпадать с шифрованием)

---

## Шаг 8: Работа с псевдо-C (типичные артефакты)

Ghidra выдаёт примерно такой код:

```c
undefined4 get_scen_package(byte param_1, byte param_2, byte param_3, 
                            byte param_4, byte param_5, byte param_6, 
                            undefined4 *param_7)
{
  byte bVar1;
  uint uVar2;
  byte *pbVar3;
  undefined4 *puVar4;
  
  // ... инициализация ...
  
  // Цикл XOR
  uVar2 = 0;
  do {
    bVar1 = *(byte *)(DAT_10018000 + uVar2);  // ключ из глобальной таблицы?
    *(byte *)((int)param_7 + uVar2) = param_4 ^ bVar1;  // green ^ key?
    uVar2 = uVar2 + 1;
  } while (uVar2 < 3);  // цикл по 3 байтам!
  
  // ... остальной код ...
}
```

### Что важно:

- `DAT_xxxxxx` — глобальные переменные (статические данные)
- `param_X` — параметры функции
- `pbVar`, `puVar` — указатели на byte/undefined4
- `^` — XOR операция
- `& 0xff` — маска младшего байта

---

## Шаг 9: Экспорт данных для меня

Скинь мне:

1. **pseudo-C для `get_scen_package`** — полный текст из Decompile панели
2. **pseudo-C для `get_connect_package`** — если `get_scen_package` сложный
3. **Скриншот Data Types** для глобальных переменных:
   - В Symbol Tree → разверни **Data**
   - Найди `DAT_` переменные, которые используются в функциях
   - Скриншот значений (или копировать Define bytes)

4. **Если нашёл MD5 или хеш**:
   - Адрес функции хеширования
   - Какие данные подаются на вход (параметры)

Формат файлов:
```
Soulight/
  ghidra_exports/
    get_scen_package.txt      # pseudo-C
    get_connect_package.txt   # pseudo-C
    global_vars.txt           # DAT_xxxx значения
    notes.txt                 # твои наблюдения
```

---

## Быстрый старт (TL;DR)

```bash
# 1. Запуск
ghidraRun.bat

# 2. Новый проект
File → New Project → beelight_analysis

# 3. Импорт
File → Import File → beelightLib.dll
Language: x86:LE:32:default:gcc

# 4. Анализ
Analyze → Yes → Analyze (подождать)

# 5. Функции
Symbol Tree → Exports → get_scen_package (двойной клик)

# 6. Декомпиляция
Правая панель Decompile (или Window → Decompile)

# 7. Экспорт
Ctrl+A, Ctrl+C в панели Decompile → вставить в get_scen_package.txt
```

---

## Если застрял

### Проблема: "Cannot find function"
- Проверь что открыл CodeBrowser (двойной клик по имени файла в проекте)
- Используй Search → For Functions → введи `get_scen`

### Проблема: "Decompile показывает `<UNKNOWN>`"
- Функция не проанализирована полностью
- В Listing панели: правый клик → Function → Create Function
- Или просто подожди — auto-analysis работает в фоне

### Проблема: "Не понимаю псевдо-C"
- Ищи паттерны: `for`, `while`, `^` (XOR), `& 0xff` (байтовая маска)
- Скинь мне сырой текст — я разберу

### Проблема: "Нет экспортов в Symbol Tree"
- Проверь что файл PE правильно распознан
- Window → Memory Map — должен показать секции `.text`, `.data`, etc.

---

## Чеклист для проверки

- [ ] Ghidra установлена и запущена
- [ ] Проект `beelight_analysis` создан
- [ ] `beelightLib.dll` импортирован (x86 32-bit)
- [ ] Auto-analysis завершена
- [ ] `get_scen_package` открыта в CodeBrowser
- [ ] Decompile панель показывает pseudo-C
- [ ] Код экспортирован в `get_scen_package.txt`
- [ ] Файлы скинуты мне

После получения pseudo-C я найду формулу генерации key3 и реализую рабочий cipher.
