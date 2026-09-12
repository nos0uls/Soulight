# __main__.py — позволяет запускать: python -m soulight
#
# --headless ветка идёт первой и НЕ импортирует PyQt6 —
# в headless-окружении (systemd, SSH без X) GUI-зависимости не нужны.
import sys

if "--headless" in sys.argv:
    from soulight.headless import main
    sys.exit(main())
else:
    from soulight.app import main
    main()
