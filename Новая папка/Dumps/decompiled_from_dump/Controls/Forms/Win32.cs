using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using vJiGl01UUJfXfNWas3;

namespace Controls.Forms;

internal class Win32
{
	public struct Size
	{
		public int cx;

		public int cy;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Size(int x, int y)
		{
		}

		static Size()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BLENDFUNCTION
	{
		public byte BlendOp;

		public byte BlendFlags;

		public byte SourceConstantAlpha;

		public byte AlphaFormat;
	}

	public struct Point
	{
		public int x;

		public int y;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Point(int x, int y)
		{
		}

		static Point()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

	[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("gdi32.dll", ExactSpelling = true)]
	public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

	[DllImport("user32.dll", ExactSpelling = true)]
	public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern int DeleteDC(IntPtr hDC);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern int DeleteObject(IntPtr hObj);

	[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr ExtCreateRegion(IntPtr lpXform, uint nCount, IntPtr rgnData);

	[MethodImpl(MethodImplOptions.NoInlining)]
	public Win32()
	{
	}

	static Win32()
	{
		DyyVDbaRvM1YfIq9il.vEB6drODu();
	}
}
