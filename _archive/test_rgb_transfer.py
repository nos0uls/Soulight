# test_rgb_transfer.py — Тест GenRGBTransferPackage для per-LED контроля.
# Проверяет, можно ли отправить массив цветов (по одному на каждый LED).

import sys
sys.path.insert(0, ".")

from soulight.protocol.bridge import BeelightBridge


def hex_str(data):
    if data is None:
        return "NULL"
    return f"{len(data)}b: {data[:60].hex(' ')}"


def main():
    bridge = BeelightBridge()
    if not bridge.init():
        print("Bridge init failed")
        return

    print("=== Тест GenRGBTransferPackage ===")

    # Проверяем наличие метода
    if bridge._gen_rgb_transfer is None:
        print("GenRGBTransferPackage НЕ НАЙДЕН в assembly")
        return

    # Выводим сигнатуру метода
    m = bridge._gen_rgb_transfer
    params = m.GetParameters()
    print(f"Метод: {m.Name}")
    print(f"Параметры ({len(params)}):")
    for p in params:
        print(f"  {p.Name}: {p.ParameterType}")
    print(f"Return: {m.ReturnType}")

    # Ищем тип RGB в assembly (LProtocolBase+RGB — nested struct)
    from System.Reflection import Assembly, BindingFlags
    from System import Array, Byte, Activator

    asm = Assembly.LoadFrom(r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe")
    rgb_type = None
    for t in asm.GetTypes():
        full = str(t.FullName) if t.FullName else ""
        if "RGB" in t.Name and "LProtocol" in full:
            print(f"  Найден тип: {full}")
            for f in t.GetFields(BindingFlags.Public | BindingFlags.Instance):
                print(f"    Поле: {f.Name} ({f.FieldType})")
            # Берём именно LProtocolBase+RGB (nested struct с полями R, G, B)
            if "LProtocolBase+RGB" in full:
                rgb_type = t

    if rgb_type is None:
        print("Тип RGB не найден!")
        return

    # Создаём массив RGB и заполняем.
    # RGB — value type (struct), поэтому нужно использовать
    # System.Runtime.Serialization.FormatterServices.GetUninitializedObject
    # для создания экземпляров, или задавать поля через boxed объект.
    try:
        from System.Runtime.Serialization import FormatterServices

        colors = Array.CreateInstance(rgb_type, 75)

        # Получаем поля RGB struct
        flags = BindingFlags.Public | BindingFlags.Instance
        field_r = rgb_type.GetField("R", flags)
        field_g = rgb_type.GetField("G", flags)
        field_b = rgb_type.GetField("B", flags)
        print(f"\nПоля: R={field_r}, G={field_g}, B={field_b}")

        for i in range(75):
            # Создаём boxed struct
            rgb = FormatterServices.GetUninitializedObject(rgb_type)

            if i < 23:
                rv, gv, bv = 255, 0, 0      # Red
            elif i < 38:
                rv, gv, bv = 0, 255, 0      # Green
            elif i < 61:
                rv, gv, bv = 0, 0, 255      # Blue
            else:
                rv, gv, bv = 255, 255, 0    # Yellow

            field_r.SetValue(rgb, Byte(rv))
            field_g.SetValue(rgb, Byte(gv))
            field_b.SetValue(rgb, Byte(bv))
            colors.SetValue(rgb, i)

        print(f"Отправляем {len(colors)} RGB...")

        result = m.Invoke(None, [colors, Byte(0)])
        if result is None:
            print("Результат: NULL")
        else:
            data = bytes(result)
            print(f"Результат: {hex_str(data)}")
            if len(data) > 60:
                print(f"  Полный размер: {len(data)} байт")
            print(f"  Начинается с 55 AA 5A: {data[:3].hex() == '55aa5a'}")
    except Exception as e:
        print(f"ОШИБКА: {e}")
        import traceback
        traceback.print_exc()

    print("\nТест завершён.")


if __name__ == "__main__":
    main()
