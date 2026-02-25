using System;
using System.Runtime.InteropServices;
using vJiGl01UUJfXfNWas3;

namespace Cyotek.Demo.SimpleScreenshotCapture;

internal static class NativeMethods
{
	public enum StretchBltMode
	{
		BLACKONWHITE = 1,
		COLORONCOLOR = 3,
		HALFTONE = 4
	}

	[Flags]
	public enum RasterOperations
	{
		SRCCOPY = 0xCC0020,
		SRCPAINT = 0xEE0086,
		SRCAND = 0x8800C6,
		SRCINVERT = 0x660046,
		SRCERASE = 0x440328,
		NOTSRCCOPY = 0x330008,
		NOTSRCERASE = 0x1100A6,
		MERGECOPY = 0xC000CA,
		MERGEPAINT = 0xBB0226,
		PATCOPY = 0xF00021,
		PATPAINT = 0xFB0A09,
		PATINVERT = 0x5A0049,
		DSTINVERT = 0x550009,
		BLACKNESS = 0x42,
		WHITENESS = 0xFF0062,
		CAPTUREBLT = 0x40000000
	}

	public struct RECT
	{
		public int left;

		public int top;

		public int right;

		public int bottom;
	}

	[DllImport("gdi32.dll")]
	public static extern bool BitBlt(IntPtr hdcDest, int nxDest, int nyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, RasterOperations dwRop);

	[DllImport("gdi32.dll")]
	public static extern bool StretchBlt(IntPtr hdcDest, int x, int y, int nWidth, int nHeight, IntPtr pSrcDC, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, RasterOperations dwRop);

	[DllImport("gdi32.dll")]
	public static extern int SetStretchBltMode(IntPtr hdc, StretchBltMode iStretchMode);

	[DllImport("gdi32.dll")]
	public static extern bool SetBrushOrgEx(IntPtr hdc, int nXOrg, int nYOrg, IntPtr lppt);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

	[DllImport("gdi32.dll")]
	public static extern IntPtr DeleteDC(IntPtr hdc);

	[DllImport("gdi32.dll")]
	public static extern IntPtr DeleteObject(IntPtr hObject);

	[DllImport("dwmapi.dll")]
	public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

	[DllImport("user32.dll")]
	public static extern IntPtr GetDesktopWindow();

	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindowDC(IntPtr hWnd);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

	[DllImport("user32.dll")]
	public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

	[DllImport("gdi32.dll")]
	public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

	static NativeMethods()
	{
		DyyVDbaRvM1YfIq9il.vEB6drODu();
	}
}
