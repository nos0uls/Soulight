# __main__.py — позволяет запускать: python -m soulight
#
# Headless-ветка идёт первой и НЕ импортирует PyQt6 —
# в headless-окружении (systemd, SSH без X) GUI-зависимости не нужны.
# Любой headless-флаг (--mode, --restore, ...) тоже означает headless:
# иначе `python -m soulight --mode scene` молча открывал бы GUI.
import sys

_HEADLESS_FLAGS = frozenset({
    "--headless", "--mode", "--color", "--brightness", "--pattern",
    "--speed", "--fps", "--audio-mode", "--device", "--list-devices",
    "--monitor", "--edge", "--full-led", "--port", "--no-retry",
    "--restore", "--auto-brightness", "--camera",
})

if any(a in _HEADLESS_FLAGS for a in sys.argv[1:]):
    from soulight.headless import main
    sys.exit(main())
else:
    from soulight.app import main
    main()
