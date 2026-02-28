using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controls.Forms;
using Cyotek.Demo.SimpleScreenshotCapture;
using LightProtocol.Application;
using LightProtocol.Hardware;
using LightProtocol.Hardware.HDriver;
using LightProtocol.Hardware.HDriver.DeviceNet;
using LightProtocol.LightDevice;
using LightProtocol.LightDevice.Device;
using LightProtocol.Vendor.LBitmap;
using LightProtocol.Vendor.LFramework.LMode;
using LightProtocol.Vendor.LFramework.LRender;
using LightProtocol.Vendor.LPVendor;
using Load;
using MacForm;
using Microsoft.Win32;
using MyOpaqueLayer;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Razer.Chroma.Broadcast;
using beelight.Control.UpdateHandle;
using beelight.DataView;
using beelight.Driver.Audio;
using beelight.Driver.Screen;
using beelight.Language;
using beelight.Library;
using beelight.Library.Protocol;
using beelight.Vendor.LFramework.LMode.ExternalMode;
using beelight.Vendor.Razer;
using beelight.Views.UIControlEvent;
using beelight.Views.UserControls;
using beelight.Views.UserForms;
using c5uHW3cSW8ou55rAF3;
using cH8IXcwQY4Peh2qpAn;
using cOiloqIe4m0u98YvJc;
using vJiGl01UUJfXfNWas3;

[assembly: Guid("85fb2e45-4948-499b-b4f3-22024d5aa465")]
[assembly: ComVisible(false)]
[assembly: AssemblyTrademark("")]
[assembly: SuppressIldasm]
[assembly: TargetFramework(".NETFramework,Version=v4.5", FrameworkDisplayName = ".NET Framework 4.5")]
[assembly: AssemblyFileVersion("3.0.1.0")]
[assembly: AssemblyCopyright("Copyright ©  2022")]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: CompilationRelaxations(8)]
[assembly: AssemblyTitle("Beelight V3.0")]
[assembly: AssemblyCompany("Glowwrom")]
[assembly: AssemblyProduct("Beelight V3.0")]
[assembly: AssemblyDescription("Copyright  2019-2022. Glowwrom.All Rights Reserved.")]
[assembly: AssemblyConfiguration("")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("3.0.1.0")]
[module: UnverifiableCode]
internal class <Module>
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	static <Module>()
	{
		//Discarded unreachable code: IL_0002
		while (false)
		{
			_ = ((object[])null)[0];
		}
		WHFZxkbSLrgnS9Moa2.lLHifFIsCLsZtjvFfN0i();
	}
}
internal static class 5bm\u00969\u008f\u0087\u008c6\u008bc\u00892\u008bnlet
{
	static 5bm\u00969\u008f\u0087\u008c6\u008bc\u00892\u008bnlet()
	{
		DyyVDbaRvM1YfIq9il.vEB6drODu();
	}
}
namespace MacForm
{
	public class RoundButton : Control
	{
		private float exBorderSize;

		private float exBorderRadius;

		private Color exBorderColor;

		private Color exButtonColor;

		private string exText;

		private Color exTextColor;

		private Font exTextFont;

		[DefaultValue(typeof(float), "0")]
		[Category("EX属性")]
		public float EXBorderSize
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0f;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[DefaultValue(typeof(float), "10")]
		[Category("EX属性")]
		public float EXBorderRadius
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0f;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[DefaultValue(typeof(Color), "Transparent")]
		[Category("EX属性")]
		public Color EXBorderColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[DefaultValue(typeof(Color), "Lime")]
		[Category("EX属性")]
		public Color EXButtonColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[DefaultValue(typeof(string), "RoundButton")]
		[Category("EX属性")]
		public string EXText
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[Category("EX属性")]
		public Color EXTextColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Category("EX属性")]
		public Font EXTextFont
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public RoundButton()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private RectangleF GetRectangleF(RectangleF rectangleF, float borderSize)
		{
			return (RectangleF)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private GraphicsPath GetGraphicsPath(RectangleF rectangleF, float borderRadius)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnPaint(PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnMouseDown(MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnMouseUp(MouseEventArgs e)
		{
		}

		static RoundButton()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace Controls.Forms
{
	public class FrmAnchorTips : Form
	{
		private string m_strMsg;

		private bool haveHandle;

		private Rectangle m_rectControl;

		private AnchorTipsLocation m_location;

		private Color? m_background;

		private Color? m_foreColor;

		private int m_fontSize;

		private IContainer components;

		public string StrMsg
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public Rectangle RectControl
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Rectangle)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private FrmAnchorTips(Rectangle rectControl, string strMsg, AnchorTipsLocation location = AnchorTipsLocation.RIGHT, Color? background = null, Color? foreColor = null, int fontSize = 10)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ResetForm(string strMsg)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static FrmAnchorTips ShowTips(Control anchorControl, string strMsg, AnchorTipsLocation location = AnchorTipsLocation.RIGHT, Color? background = null, Color? foreColor = null, Size? deviation = null, int fontSize = 10, bool blnTopMost = true)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static FrmAnchorTips ShowTips(Rectangle rectControl, string strMsg, AnchorTipsLocation location = AnchorTipsLocation.RIGHT, Color? background = null, Color? foreColor = null, int fontSize = 10, Control parentControl = null, bool blnTopMost = true)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool CheckControlClose(Control c)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnClosing(CancelEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnHandleCreated(EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InitializeStyles()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SetBits(Bitmap bitmap)
		{
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SetActiveWindow(IntPtr handle);

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void WndProc(ref Message m)
		{
		}

		[DllImport("kernel32.dll")]
		public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ClearMemory()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void FrmAnchorTips_FormClosing(object sender, FormClosingEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static FrmAnchorTips()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public enum AnchorTipsLocation
	{
		LEFT,
		TOP,
		RIGHT,
		BOTTOM
	}
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
}
namespace Cyotek.Demo.SimpleScreenshotCapture
{
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
	internal sealed class ScreenshotCapture
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureActiveWindow()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureDesktop()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureDesktop(bool workingAreaOnly)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureDesktop(Color invalidColor)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureDesktop(bool workingAreaOnly, Color invalidColor)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureDesktop(Predicate<int> includeScreen)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureDesktop(bool workingAreaOnly, Color invalidColor, Predicate<int> includeScreen)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureMonitor(Screen monitor)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureMonitor(Screen monitor, bool workingAreaOnly)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureMonitor(int index)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureMonitor(int index, bool workingAreaOnly)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureRegion(Rectangle region)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureWindow(IntPtr hWnd)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap CaptureWindow(Form form)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ScreenshotCapture()
		{
		}

		static ScreenshotCapture()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace Cyotek.Windows.Forms
{
	internal sealed class DesktopLayout
	{
		private Rectangle _bounds;

		private int _count;

		private Rectangle[] _displays;

		private bool _workingAreaOnly;

		public Rectangle Bounds
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Rectangle)(object)null;
			}
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
		}

		public int Height
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
		}

		public int Width
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
		}

		public bool WorkingAreaOnly
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DesktopLayout()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Rectangle GetDisplayBounds(int index)
		{
			return (Rectangle)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Rectangle GetNormalizedDisplayBounds(int index)
		{
			return (Rectangle)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Rectangle GetNormalizedDisplayBounds(Rectangle bounds)
		{
			return (Rectangle)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Initialize()
		{
		}

		static DesktopLayout()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace Load
{
	internal class OpaqueCommand
	{
		private global::MyOpaqueLayer.MyOpaqueLayer m_OpaqueLayer;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ShowOpaqueLayer(Control control, int alpha, bool isShowLoadingImage)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void HideOpaqueLayer()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public OpaqueCommand()
		{
		}

		static OpaqueCommand()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace MyOpaqueLayer
{
	[ToolboxBitmap(typeof(MyOpaqueLayer))]
	public class MyOpaqueLayer : Control
	{
		private bool _transparentBG;

		private int _alpha;

		private Container components;

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[Description("是否使用透明,默认为True")]
		[Category("MyOpaqueLayer")]
		public bool TransparentBG
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Category("MyOpaqueLayer")]
		[Description("设置透明度")]
		public int Alpha
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public MyOpaqueLayer()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public MyOpaqueLayer(int Alpha, bool IsShowLoadingImage)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnPaint(PaintEventArgs e)
		{
		}

		static MyOpaqueLayer()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight
{
	internal static class Program
	{
		private static bool glExitApp;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void Main()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void SetExceptionHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
		}

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ShowOrHiddenWin(IntPtr handle, uint flag)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void starting()
		{
		}

		static Program()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Views
{
	internal class DeviceForm : Form
	{
		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;

			public int cbData;

			public IntPtr lpData;
		}

		public enum BroadcastType
		{
			Screen,
			Music,
			Rainbow,
			Fire,
			Vitality,
			Seasons,
			Warm,
			Romance
		}

		public class BroadcastMusicData
		{
			public float power_average;

			public float power_max;

			public float percentage;

			public float change;

			public bool power_lock;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public BroadcastMusicData()
			{
			}

			static BroadcastMusicData()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private static DeviceForm deviceform;

		public ScreenCapture screenCapture;

		public bool updateControllerType;

		public int SelectMenuIndex;

		public Dictionary<string, Protocol> NewDevice;

		public Dictionary<string, MainForm> mainFroms;

		public Dictionary<string, GroupingMainForm> groupingMainFroms;

		public Dictionary<string, bool> fromsSwitch;

		public FeastMainForm feastMainForm;

		public OpaqueCommand cmd;

		public MainForm mainForm;

		private int beforeSelectMenuIndex;

		private int oldX;

		private int oldY;

		private AppData appData;

		private AppDriver appDriver;

		private IniPublicClass iniPublicClass;

		private bool errorformlock;

		private ContextMenuStrip contextMenuStripClose;

		private Dictionary<string, Protocol> dicts;

		private int tempname;

		private bool loadErrorForm;

		private Dictionary<string, string> boradcastmusic;

		private int DesktopDuplicationType;

		public double value;

		public static BroadcastType broadcastType;

		public BroadcastMusicData ele;

		public LFrame lFrame;

		private string lFrameInfo;

		private Dictionary<string, LFrame> keyBroadcast;

		private IContainer components;

		private PictureBox pictureBoxHiden;

		private PictureBox pictureboxClose;

		private NewPanel panel_device;

		private Label label_device;

		private NotifyIcon notifyIconDeviceForm;

		private NewPanel panel_content;

		private PictureBox pictureBoxSet;

		private Label labelDeviceError;

		private Label labelRefresh;

		private NewPanel panelDevice;

		private NewPanel panel_about;

		private Label label_about;

		private PictureBox pictureBox2;

		private FlowLayoutPanel flowLayoutPanel1;

		private NewPanel panel_advance;

		private Label label_advance;

		private NewPanel panel_setting;

		private Label label_setting;

		private PictureBox pictureBoxControl;

		private Label labelControl;

		private PictureBox pictureBox_deviceimg;

		private PictureBox pictureBox_advanceimg;

		private PictureBox pictureBox_settingimg;

		private PictureBox pictureBox_aboutimg;

		private PictureBox pictureBoxRefresh;

		private PictureBox pictureBoxAddGrouping;

		private Label labelAddGrouping;

		private NewPanel panel_feast;

		private PictureBox pictureBox_feastimg;

		private Label label_feast;

		private NewPanel newPanel1;

		private FlowLayoutPanel flowLayoutPanel3;

		public static DeviceForm deviceForm
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceForm()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void iniContextMenuStrip()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceForm_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void IniUI()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void DeviceToolStripMenuItem_MouseDown(string tag)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void buttonGroupingMouseDown(string text, string tag)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void mainformThreadPool(object obj)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UpdateControl(bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void NewDeviceDiscpvery()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AddNewDeviceEvent(IDevice device)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void RemoveDeviceEvent(IDevice device)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void addButton(bool update = false)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updatePanel()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void topMenuLocation()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void labelRefresh_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxHiden_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceForm_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceForm_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_menu_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void label_menu_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_menu_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxHiden_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void notifyIconDeviceForm_MouseClick(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void pictureBoxSet_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void pictureBoxSet_Click(Rectangle rect, Image image)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceForm_Shown(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceForm_EntryWritten(object sender, EntryWrittenEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ErrorForm()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_advance_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updatePanelUI()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UplabelControlText(LProtocolBase.LP_WK_MODE WorkMode)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void DefWndProc(ref Message m)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ProcessIncomingData(int iWParam, int sLParam, bool key)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelAddGrouping_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SendLProtocolSyncRGB()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void LFrameLoaderFinishEventHandle(bool success, string info, LFrame frame)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void rightPanel_MouseWheel(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UpdateDeviceControlsSwitch(string deviceid, bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static DeviceForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FeastMainForm : UserControl
	{
		public int logUSelect;

		public bool loadone;

		public IniPublicClass IniPublicClass;

		public Image pictureBox3;

		public new Image BackgroundImage;

		public int beforeSelectMenuIndex;

		public LProtocolBase.LP_WK_MODE OldWorkMode;

		private LanguageEngine language;

		private List<string> deviceids;

		private static AppData appData;

		private int uselect;

		private int luminance;

		private int oldX;

		private int oldY;

		private int select;

		public bool tasklFrameSwitch;

		private IContainer components;

		private NewPanel panel_content;

		private DimmerTrackBar dimmerTrackBar;

		private NewPanel panelScreen;

		private Label labelScreen;

		private PictureBox pictureBoxScreen;

		private NewPanel panelMusic;

		private Label labelMusic;

		private PictureBox pictureBoxMusic;

		private NewPanel newPanel3;

		private Label labelRomance;

		private PictureBox pictureBoxRomance;

		private NewPanel newPanel4;

		private Label labelSeasons;

		private PictureBox pictureBoxSeasons;

		private NewPanel newPanel5;

		private Label labelWarm;

		private PictureBox pictureBoxWarm;

		private NewPanel newPanel6;

		private Label labelVitality;

		private PictureBox pictureBoxVitality;

		private NewPanel newPanel1;

		private Label labelFire;

		private PictureBox pictureBoxFire;

		private NewPanel newPanel2;

		private Label labelRainbow;

		private PictureBox pictureBoxRainbow;

		private NewPanel panel_img;

		private PictureBox labelSwitch;

		private NewPanel panel1;

		private Label label_titletext;

		public int USelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FeastMainForm()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MouseEvent(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MainFrom_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initPictureBoxImgage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void label_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void updateImage(int select)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateBackgroundImage(Image backgroundimage)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelSwitch_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateUserFunnyControlpanel_img(Image backgroundimage, string text)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void loadShow()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void tasklFrame()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static FeastMainForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class GroupingMainForm : UserControl
	{
		public int logUSelect;

		public bool loadone;

		public IniPublicClass IniPublicClass;

		public string DeviceName;

		public Image pictureBox3;

		public new Image BackgroundImage;

		public int beforeSelectMenuIndex;

		public LProtocolBase.LP_WK_MODE OldWorkMode;

		private LanguageEngine language;

		private UpdateController updateController;

		private Thread fpsThread;

		private List<string> deviceids;

		private static AppData appData;

		private int uselect;

		private int luminance;

		private int oldX;

		private int oldY;

		private int select;

		private IContainer components;

		private NewPanel panel_content;

		private DimmerTrackBar dimmerTrackBar;

		private Label labelDeviceName;

		private Label labelFPS;

		private NewPanel panel_header;

		private TextBox textBox1;

		private Button button1;

		private Button button2;

		private Button button3;

		private NewPanel panel_img;

		private PictureBox labelSwitch;

		private NewPanel panel1;

		private Label label_titletext;

		private NewPanel panelScreen;

		private Label labelScreen;

		private PictureBox pictureBoxScreen;

		private NewPanel panelMusic;

		private Label labelMusic;

		private PictureBox pictureBoxMusic;

		private NewPanel newPanel3;

		private Label labelRomance;

		private PictureBox pictureBoxRomance;

		private NewPanel newPanel4;

		private Label labelSeasons;

		private PictureBox pictureBoxSeasons;

		private NewPanel newPanel5;

		private Label labelWarm;

		private PictureBox pictureBoxWarm;

		private NewPanel newPanel6;

		private Label labelVitality;

		private PictureBox pictureBoxVitality;

		private NewPanel newPanel1;

		private Label labelFire;

		private PictureBox pictureBoxFire;

		private NewPanel newPanel2;

		private Label labelRainbow;

		private PictureBox pictureBoxRainbow;

		private Label labelDevice;

		public int USelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public GroupingMainForm(string groupingname, string groupingifon)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MainFrom_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelFPS_DoubleClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void button1_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void button2_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void button3_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetModeSelect(int modeselect)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void picturebox_close_Click()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelSwitch_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void labelSwitch_Click(bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void upLabelDecice(string devicename)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initPictureBoxImgage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void label_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void updateImage(int select)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateUserFunnyControlpanel_img(Image backgroundimage, string text)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateBackgroundImage(Image backgroundimage)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MouseEvent(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void loadShow()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static GroupingMainForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class MainForm : UserControl
	{
		public IniPublicClass IniPublicClass;

		public string DeviceName;

		public Image pictureBox3;

		public new Image BackgroundImage;

		public int beforeSelectMenuIndex;

		public LProtocolBase.LP_WK_MODE OldWorkMode;

		public bool OldSleep;

		private LanguageEngine languageEngine;

		private UserControl[] PanelControls;

		private Size contentOriginalSize;

		private Size contentColorSize;

		private Size thisOriginalSize;

		private UpdateController updateController;

		private Thread fpsThread;

		private ConnectError connectError;

		private ManualResetEvent resetEvent;

		private int oldX;

		private int oldY;

		private IContainer components;

		private NewPanel panel_content;

		private Label label_connect;

		private PictureBox pictureBox_status;

		private DimmerTrackBar dimmerTrackBar;

		private Label labelScreen;

		private Label labelDevice;

		private Label labelScreenName;

		private Label labelDeviceName;

		private Label labelFPS;

		private ColorPanelControls colorPanelControls;

		private NewPanel panel_header;

		private TextBox textBox1;

		private PictureBox pictureBoxRedact;

		private PictureBox pictureBoxSet;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void IniPublic(string DeviceId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public MainForm(string deviceId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void init_Controls()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void init_param()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void init_language()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void init_service()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void panel_menu_Click(bool type, int index)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void FPSThreadHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectThreadError()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScreenName(string name)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ExitApp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initConnectLabel()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void topMenuLocation()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initConnectStatus()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateBackgroundImage(Image backgroundimage)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeConnect()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void picturebox_close_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void picturebox_close_Click()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Memory()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void WndProc(ref Message m)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MainFrom_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updateScreenSelect()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelFPS_DoubleClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void fromSynchronization()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceUpdateEvent(string deviceid)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AddNewDeviceEvent(IDevice device)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void RemoveDeviceEvent(IDevice device)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void taskThread()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxSet_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void headerSkewing()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxRedact_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void upLabelDecice(string devicename)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static MainForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Views.UserPanels
{
	internal class AboutPanel : UserControl
	{
		private UIEventController uIEventController;

		private LanguageEngine language;

		private Storage storage;

		private IContainer components;

		private Label labelTitle;

		private RichTextBox richTextBoxContent;

		private Label labelContent;

		private Label labelCopyrightIfon;

		private RoundButton roundButton1;

		private LinkLabel linkLabel1;

		private LinkLabel linkLabel2;

		private Label labelVersions;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AboutPanel(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AboutPanel_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updateLabelVersions()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AboutPanel_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static AboutPanel()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class AdvancePanel : UserControl
	{
		private IContainer components;

		private UserAdvanceControls userAdvanceControls;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AdvancePanel(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static AdvancePanel()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FunnyPanel : UserControl
	{
		private IContainer components;

		private UserFunnyControls_Lamp userFunnyControls_Lamp;

		private UserFunnyControls_Bulb userFunnyControls_Bulb;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FunnyPanel(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static FunnyPanel()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class SetForm : Form
	{
		private LanguageEngine language;

		private AppData appData;

		private Rectangle rectangle;

		private FrmAnchorTips frmAnchorTips;

		private IContainer components;

		private PictureBox pictureboxClose;

		private Label labelBECLeftRight;

		private UserBtnControls userBtnBECLeftRight;

		private Label labelBECTopBottom;

		private UserBtnControls userBtnBECTopBottom;

		private Label labelEcoMode;

		private UserBtnControls userBtnEcoMode;

		private UserBtnControls userBtnSmart;

		private Label labelNet;

		private UserBtnControls userBtnUpdate;

		private Label labelCheckUpdate;

		private Label labelBootStart;

		private UserBtnControls userBtnAutoStart;

		private ComboBox comboBoxLanguage;

		private Label labelLanguageChoose;

		private Label labelTitle;

		private Label labelHardwareUpdate;

		private UserBtnControls userBtnHardwareUpdate;

		private PictureBox labelBECTopBottomImg;

		private PictureBox labelCheckUpdateImg;

		private PictureBox labelBECLeftRightImg;

		private PictureBox labelBootStartImg;

		private PictureBox labelNetImg;

		private PictureBox labelEcoModeImg;

		private PictureBox labelHardwareUpdateImg;

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public SetForm(Rectangle rect, Image image)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void setLanguageText()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SetForm_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void comboBoxLanguage_SelectionChangeCommitted(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnSmart_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnUpdate_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnEcoMode_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelImgPublic_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelImgPublic_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string CutStr(string str, int len)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnBECLeftRight_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnBECTopBottom_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnAutoStart_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnHardwareUpdate_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static SetForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class SettingPanel : UserControl
	{
		private IContainer components;

		private UserSettingControls userSettingControls;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public SettingPanel(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static SettingPanel()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Views.UserForms
{
	internal class ConnectErrorAll : Form
	{
		private LanguageEngine language;

		private Storage storage;

		private UIEventController uIEventController;

		private int oldX;

		private int oldY;

		private IContainer components;

		private PictureBox picturebox_close;

		private PictureBox pictureBox1;

		private RoundButton roundButtonResetClose;

		private NewPanel panel1;

		private Label labelLamp;

		private Label labelLight;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ConnectErrorAll(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void picturebox_close_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectError_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonResetNetwork_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectError_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectError_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static ConnectErrorAll()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class Grouping : Form
	{
		private Rectangle rectangle;

		private AppData appData;

		private string oldname;

		private IContainer components;

		private NewPanel panelAll;

		private Label labelTitle;

		private Label labelGroupingName;

		private TextBox textBox2;

		private RoundButton roundButtonYes;

		private PictureBox pictureboxClose;

		private CheckBox checkBoxAll;

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Grouping(Image image, Rectangle rect, string name = "")
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grouping_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonYes_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void checkBox_MouseEventHandler(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void textBox2text(string name = "")
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void checkBoxAll_MouseClick(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string GettextBox2Text()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static Grouping()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class HardwareAppUpdate : Form
	{
		private IDevice Device;

		private string updateversion;

		private Rectangle rectangle;

		private LanguageEngine language;

		private AppData appData;

		private int plan;

		private int Percent;

		private bool otatimeoutlock;

		private int otatimeout;

		private IContainer components;

		private Label labelTitle;

		private RoundButton roundButtonOK;

		private RoundButton roundButtonNo;

		private Label labellabelCheckBox;

		private Label labelCheckBoxImg;

		private ProgressBar progressBar1;

		private Label labelError;

		private PictureBox pictureBox2;

		private PictureBox pictureBox3;

		private Label label1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public HardwareAppUpdate(string version, IDevice device, Rectangle rect)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonOK_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonNo_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelCheckBoxImg_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void FnDeviceOTAPercentEvent(int percent)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void HardwareAppUpdate_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void OTATiemoutTask()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static HardwareAppUpdate()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ConnectError : Form
	{
		private LanguageEngine language;

		private Storage storage;

		private UIEventController uIEventController;

		private int oldX;

		private int oldY;

		private IContainer components;

		private PictureBox picturebox_close;

		private Label labelText;

		private PictureBox pictureBox1;

		private RoundButton roundButtonResetClose;

		private PictureBox pictureBox2;

		private NewPanel panel1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ConnectError(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void picturebox_close_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectError_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonResetNetwork_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectError_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectError_Shown(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ConnectError_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static ConnectError()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class ConnectErrorBackground : Form
	{
		private IContainer components;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ConnectErrorBackground()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Show(Form frmTop, Form frmBackOwner, Color frmBackColor, double frmBackOpacity)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static ConnectErrorBackground()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ColorInstallationForm : Form
	{
		private Rectangle rectangle;

		private Storage storage;

		private int sideCount;

		private int modeselect;

		private LanguageEngine language;

		private List<Color> colors;

		private IContainer components;

		private PictureBox pictureBox1;

		private RoundButton roundButtonYes;

		private PictureBox pictureBox0;

		private NewPanel panel1;

		private PictureBox pictureBox24;

		private PictureBox pictureBox23;

		private PictureBox pictureBox18;

		private PictureBox pictureBox17;

		private PictureBox pictureBox12;

		private PictureBox pictureBox22;

		private PictureBox pictureBox11;

		private PictureBox pictureBox16;

		private PictureBox pictureBox6;

		private PictureBox pictureBox21;

		private PictureBox pictureBox10;

		private PictureBox pictureBox15;

		private PictureBox pictureBox5;

		private PictureBox pictureBox20;

		private PictureBox pictureBox9;

		private PictureBox pictureBox14;

		private PictureBox pictureBox4;

		private PictureBox pictureBox19;

		private PictureBox pictureBox8;

		private PictureBox pictureBox13;

		private PictureBox pictureBox3;

		private PictureBox pictureBox7;

		private PictureBox pictureBox2;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ColorInstallationForm(Storage sto, Rectangle rect)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InstallationForm_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonYes_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxIni(int index = -1)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static ColorInstallationForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class InstallationForm : Form
	{
		private Rectangle rectangle;

		private Storage storage;

		private int sideCount;

		private int modeselect;

		private LanguageEngine language;

		private List<Color> colors;

		private IContainer components;

		private PictureBox pictureBoxRightTop;

		private PictureBox pictureBoxRightBottom;

		private PictureBox pictureBoxLeftBottom;

		private PictureBox pictureBoxLeftTop;

		private RoundButton roundButtonYes;

		private RoundButton roundButtonNO;

		private PictureBox pictureBox1;

		private RichTextBox richTextBoxTooltip;

		private PictureBox pictureBoxLeft;

		private PictureBox pictureBoxRight;

		private PictureBox pictureBoxTop;

		private PictureBox pictureBoxBottom;

		private NewPanel panel1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public InstallationForm(Storage sto, Rectangle rect)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InstallationForm_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSidePatternPaint(int sidePattern)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void label_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void setModesValue()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool typeSwitch(bool tag, PictureBox label)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool SelectedValid(string name, string typeindex)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool NoSelectedValid(string name, string typeindex)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonNO_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonYes_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void upDateRichTextBox1Text()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void upDatePictureBoxBackgroundImage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static InstallationForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class DialogueBoxForm : Form
	{
		private Rectangle rectangle;

		private IContainer components;

		private Label labelTitle;

		private Label labelMessageBoxText;

		private RoundButton roundButtonNo;

		private RoundButton roundButtonOK;

		private NewPanel panel1;

		private PictureBox pictureboxClose;

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DialogueBoxForm(string language, string text, Image image, Rectangle rect)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MessageBoxForm_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonOK_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonNo_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static DialogueBoxForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class MessageBoxForm : Form
	{
		private Rectangle rectangle;

		private IContainer components;

		private RoundButton roundButtonOK;

		private Label labelMessageBoxText;

		private Label labelTitle;

		private NewPanel panel1;

		private PictureBox pictureboxClose;

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public MessageBoxForm(string language, string text, Image image, Rectangle rect)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonOK_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MessageBoxForm_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureboxClose_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static MessageBoxForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class UpDeviceName : Form
	{
		private Rectangle rectangle;

		private string deviceId;

		private IContainer components;

		private Label labelTitle;

		private Label label1;

		private Label label2;

		private TextBox textBox1;

		private TextBox textBox2;

		private RoundButton roundButtonYes;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UpDeviceName(string oldName, string deviceid, Rectangle rect, Image image)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void roundButtonYes_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpDeviceName_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpDeviceName_Shown(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string GetText()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static UpDeviceName()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Views.UIControlEvent
{
	internal class UIEvent
	{
		public delegate void Fn_ChangeSerial();

		public delegate void Fn_ChangeLanguage();

		public delegate void Fn_ShowColorPanel();

		public delegate void Fn_HidenColorPanel();

		public delegate void Fn_ChangeScen();

		public delegate void Fn_ChangeLightType();

		public delegate void Fn_ChangeDimmer();

		public delegate void Fn_ChangePowerStatus();

		public delegate void Fn_ChangeColor();

		public delegate void Fn_ChangeConnect();

		public delegate void Fn_ChangeDisplay();

		public delegate void Fn_UpdateSetting();

		public delegate void Fn_UpdatelabelOTAVersions();

		public delegate void Fn_UpdateScenSetting();

		public delegate void Fn_UpdateScreenName(string name);

		public delegate void Fn_UpdateExternPort();

		public delegate void Fn_ChangeExternPort();

		public delegate void Fn_UpdateWireless();

		public delegate void Fn_ExitApp();

		public delegate void Fn_UpdateFormClose();

		public delegate void Fn_UpdateBackgroundImage(Image backgroundimage);

		public delegate void Fn_UpdateUserFunnyControlpanel_img(Image backgroundimage, string text);

		public delegate bool Fn_UpdatePictureBoxImage(int index);

		public delegate void Fn_UpdateLabelSwitch(bool on_off);

		public delegate void Fn_SimulationPictureBox();

		public delegate void Fn_UpdateScreenSelect();

		public delegate void Fn_PanelLeftSetupPaint(bool type);

		public delegate void Fn_UpadteTargetColor();

		public delegate void Fn_UpdateScreen();

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UIEvent()
		{
		}

		static UIEvent()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class UIEventController
	{
		private static object m_lock;

		public UIEvent.Fn_HidenColorPanel HidenColorPanel;

		public UIEvent.Fn_ShowColorPanel ShowColorPanel;

		public UIEvent.Fn_ChangeLanguage ChangeLanguage;

		public UIEvent.Fn_ChangeScen ChangeScen;

		public UIEvent.Fn_ChangeSerial ChangeSerial;

		public UIEvent.Fn_ChangeLightType ChangeLightType;

		public UIEvent.Fn_ChangeDimmer ChangeDimmer;

		public UIEvent.Fn_ChangePowerStatus ChangePowerStatus;

		public UIEvent.Fn_ChangeColor ChangeColor;

		public UIEvent.Fn_ChangeConnect ChangeConnect;

		public UIEvent.Fn_ChangeDisplay ChangeDisplay;

		public UIEvent.Fn_UpdateSetting UpdateSetting;

		public UIEvent.Fn_UpdatelabelOTAVersions UpdatelabelOTAVersions;

		public UIEvent.Fn_UpdateScenSetting UpdateScenSetting;

		public UIEvent.Fn_UpdateScreenName UpdateScreenName;

		public UIEvent.Fn_UpdateExternPort UpdateExternPort;

		public UIEvent.Fn_ChangeExternPort ChangeExternPort;

		public UIEvent.Fn_UpdateWireless UpdateWireless;

		public UIEvent.Fn_ExitApp ExitApp;

		public UIEvent.Fn_UpdateFormClose UpdateFormClose;

		public UIEvent.Fn_UpdateBackgroundImage UpdateBackgroundImage;

		public UIEvent.Fn_UpdateUserFunnyControlpanel_img UpdateUserFunnyControlpanel_img;

		public UIEvent.Fn_UpdatePictureBoxImage UpdatePictureBoxImage;

		public UIEvent.Fn_UpdateLabelSwitch UpdateLabelSwitch;

		public UIEvent.Fn_SimulationPictureBox SimulationPictureBox;

		public UIEvent.Fn_UpdateScreenSelect UpdateScreenSelect;

		public UIEvent.Fn_PanelLeftSetupPaint PanelLeftSetupPaint;

		public UIEvent.Fn_UpadteTargetColor UpadteTargetColor;

		public UIEvent.Fn_UpdateScreen UpdateScreen;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private UIEventController()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static UIEventController GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static UIEventController()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			m_lock = new object();
		}
	}
}
namespace beelight.Views.UserControls
{
	public class DeviceControls : UserControl
	{
		private Image Image_On;

		private Image Image_Off;

		private ContextMenuStrip contextMenuStrip;

		private ContextMenuStrip contextMenuStripGrouping;

		private AppData appData;

		private DeviceType m_DeviceType;

		private IContainer components;

		private PictureBox pictureBoxIco;

		private PictureBox pictureBox2;

		private Label label1;

		[Description("显示类型")]
		[Category("自定义")]
		public DeviceType DeviceType
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (DeviceType)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceControls()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void iniContextMenuStrip()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetIniData(string labeltext, string tag, Image image_on, Image image_off)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonMouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonSwitchMouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void upDateSwitch(bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonMouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonMouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void RechristenToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ModifyGroupingToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeleteGroupingToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetSwitch(bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DeviceControls_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static DeviceControls()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public enum DeviceType
	{
		Grouping,
		Lamp,
		Light,
		Bulb
	}
	public class ColorControls : UserControl
	{
		public delegate void ChangeColorFunc(Color color);

		private int pointx;

		private int pointy;

		private int radius;

		private int centerx;

		private int centery;

		private int maxValue;

		private bool bool_down;

		private bool send;

		public Color color;

		public Color targetColor;

		private Bitmap bitmap;

		public ChangeColorFunc ChangeColor;

		private IContainer components;

		private PictureBox color_controls;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ColorControls()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void color_controls_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void color_controls_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void color_controls_MouseUp(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool isInSide(int x, int y)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void CalcuateColor()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void color_controls_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static ColorControls()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ColorPanelControls : UserControl
	{
		private IniPublicClass IniPublicClass;

		private Color[] colorsTable;

		private Color m_currentColor;

		private Color m_targetColor;

		private float color_offsetx;

		private float color_offsety;

		private float color_dltx;

		private float color_dlty;

		private float color_R;

		private IContainer components;

		private ColorControls colorControls;

		private NewPanel ColorSelectPanel;

		private NewPanel RGBAdjust;

		private ColorTrackBar blueColorTrackBar;

		private ColorTrackBar greenColorTrackBar;

		private ColorTrackBar redColorTrackBar;

		public Color TargetColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ColorPanelControls(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScenSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ColorSelectPanel_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ColorSelectPanel_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SendTargetColor()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeRed(int red)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeGreen(int green)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeBlue(int blue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeColor(Color color)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ColorPanelControls_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpadteTargetColor()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static ColorPanelControls()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class ColorTrackBar : UserControl
	{
		public delegate void ColorChangeFunc(int value);

		public ColorChangeFunc ColorChange;

		private float R;

		private float color_offsetx;

		private float color_offsety;

		private Color m_valueColor;

		private int m_value;

		private bool mouse_down;

		private IContainer components;

		private NewPanel colorPanel;

		private NewPanel trackBarPanel;

		[Category("自定义")]
		[Description("值颜色")]
		public Color ValueColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Description("值颜色")]
		[Category("自定义")]
		public int Value
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ColorTrackBar()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnPaint(PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void trackBarPanel_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void trackBarPanel_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void trackBarPanel_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void trackBarPanel_MouseUp(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void colorPanel_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static ColorTrackBar()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class DimmerTrackBar : UserControl
	{
		public delegate void DimmerChangeFunc(int dimmer);

		private IniPublicClass IniPublicClass;

		public DimmerChangeFunc DimmerChange;

		private RectangleF m_lineRectangle;

		private Color m_lineColor;

		private int minValue;

		private int currentValue;

		private int targetValue;

		private int maxValue;

		private Color m_valueColor;

		private bool blnDown;

		private IContainer components;

		private NewPanel dimmerpanel;

		public int Value
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Category("自定义")]
		[Description("最小值")]
		public int MinValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Description("最大值")]
		[Category("自定义")]
		public int MaxValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Description("值颜色")]
		[Category("自定义")]
		public Color ValueColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UCTrackBar_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UCTrackBar_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UCTrackBar_MouseUp(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DimmerTrackBar(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScenSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void DimmerThreadHandle()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void OnPaint(PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static DimmerTrackBar()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class UserFunnyControls_Bulb : UserControl
	{
		private string labelTextUserAdvanceControls;

		private LanguageEngine language;

		private int m_select;

		public bool IsShowColorPanel;

		private Size normalSize;

		private Size selectSize;

		private Point normalPoint;

		private Point selectPoint;

		private Size panelSize;

		private float labelRate;

		private Storage storage;

		private UIEventController uIEventController;

		private Statistics statistics;

		private IContainer components;

		private NewPanel panel1;

		private NewPanel panel2;

		private NewPanel panel3;

		private NewPanel panel4;

		private NewPanel panel5;

		private NewPanel panel6;

		private NewPanel panel7;

		private NewPanel panel8;

		private NewPanel panel9;

		private NewPanel panel10;

		private NewPanel panel11;

		private NewPanel panel12;

		private PictureBox pictureBox1;

		private Label labelRainbow;

		private Label labelFire;

		private Label labelReading;

		private Label labelFireworks;

		private Label labelSeasons;

		private Label labelMainColor;

		private Label labelWarm;

		private Label labelAuroraBorealis;

		private Label labelRomance;

		private Label labelFlow;

		private Label labelChase;

		private Label labelVitality;

		private NewPanel panel13;

		private NewPanel panel14;

		private PictureBox pictureBox2;

		private NewPanel panel15;

		private PictureBox pictureBox3;

		private NewPanel panel16;

		private PictureBox pictureBox4;

		private NewPanel panel17;

		private PictureBox pictureBox5;

		private NewPanel panel18;

		private PictureBox pictureBox6;

		private NewPanel panel19;

		private PictureBox pictureBox7;

		private NewPanel panel20;

		private PictureBox pictureBox8;

		private NewPanel panel21;

		private PictureBox pictureBox9;

		private NewPanel panel22;

		private PictureBox pictureBox10;

		private NewPanel panel23;

		private PictureBox pictureBox11;

		private NewPanel panel24;

		private PictureBox pictureBox12;

		private NewPanel panel25;

		public int USelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UserFunnyControls_Bulb(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScenSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initUI()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initAnimate()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void animateHandle()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool updatePictureBoxImage(int index = -1)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void initPictureBoxImage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void label_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UserFunnyControls_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static UserFunnyControls_Bulb()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class UserFunnyControls_Lamp : UserControl
	{
		private string labelTextUserAdvanceControls;

		private LanguageEngine language;

		private int m_select;

		public bool IsShowColorPanel;

		private Size normalSize;

		private Size selectSize;

		private Point normalPoint;

		private Point selectPoint;

		private Size panelSize;

		private float labelRate;

		private Storage storage;

		private UIEventController uIEventController;

		private Statistics statistics;

		private IContainer components;

		private NewPanel panel1;

		private NewPanel panel2;

		private NewPanel panel3;

		private NewPanel panel4;

		private NewPanel panel5;

		private NewPanel panel6;

		private NewPanel panel7;

		private NewPanel panel8;

		private NewPanel panel9;

		private NewPanel panel10;

		private NewPanel panel11;

		private NewPanel panel12;

		private PictureBox pictureBox1;

		private Label labelRainbow;

		private Label labelFire;

		private Label labelReading;

		private Label labelFireworks;

		private Label labelSeasons;

		private Label labelMainColor;

		private Label labelWarm;

		private Label labelAuroraBorealis;

		private Label labelRomance;

		private Label labelFlow;

		private Label labelChase;

		private Label labelVitality;

		private NewPanel panel13;

		private NewPanel panel14;

		private PictureBox pictureBox2;

		private NewPanel panel15;

		private PictureBox pictureBox3;

		private NewPanel panel16;

		private PictureBox pictureBox4;

		private NewPanel panel17;

		private PictureBox pictureBox5;

		private NewPanel panel18;

		private PictureBox pictureBox6;

		private NewPanel panel19;

		private PictureBox pictureBox7;

		private NewPanel panel20;

		private PictureBox pictureBox8;

		private NewPanel panel21;

		private PictureBox pictureBox9;

		private NewPanel panel22;

		private PictureBox pictureBox10;

		private NewPanel panel23;

		private PictureBox pictureBox11;

		private NewPanel panel24;

		private PictureBox pictureBox12;

		private NewPanel panel25;

		public int USelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UserFunnyControls_Lamp(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScenSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initUI()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initAnimate()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void animateHandle()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool updatePictureBoxImage(int index = -1)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void initPictureBoxImage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void label_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UserFunnyControls_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static UserFunnyControls_Lamp()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class UpdateForm : Form
	{
		private int oldX;

		private int oldY;

		private string content;

		private UIEventController controller;

		private LanguageEngine lan;

		private IContainer components;

		private Button buttonCancel;

		private Button buttonUpdate;

		private NewPanel panelContent;

		private NewPanel panel1;

		private RichTextBox richTextBox;

		private Label labelTitle;

		private Label labelClose;

		private Label labelCheckBoxImg;

		private Label labellabelCheckBox;

		private RoundButton roundButtonCancel;

		private RoundButton roundButtonUpdate;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UpdateForm(string content, IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InitContent()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InitParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonCancel_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonUpdate_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panel_header_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelClose_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelClose_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonUpdate_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonCancel_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelCheckBoxImg_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labellabelCheckBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static UpdateForm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class UserSettingControls : UserControl
	{
		private Size ChooseSize;

		private bool isLightStrip;

		private bool isLeftSetup;

		private Storage storage;

		private UIEventController uIEventController;

		private LanguageEngine language;

		private AppData appData;

		private List<string> externPorts;

		private FrmAnchorTips frmAnchorTips;

		private IContainer components;

		private NewPanel panelFirst;

		private NewPanel panelOther;

		private NewPanel panelStandingLight;

		private NewPanel panelLightStrip;

		private PictureBox pictureBoxLightStrip;

		private Label labelLightType;

		private PictureBox pictureBoxStandLamp;

		private Label labelOtherSetting;

		private Label labelStandLamp;

		private Label labelLightStrip;

		private NewPanel panelSelect2;

		private NewPanel panelSelect1;

		private UserBtnControls userBtnUpdate;

		private Label labelCheckUpdate;

		private Label labelBootStart;

		private UserBtnControls userBtnAutoStart;

		private ComboBox comboBoxLanguage;

		private Label labelLanguageChoose;

		private ComboBox comboBoxScreen;

		private Label labelScreen;

		private ComboBox comboBoxSerial;

		private Label labelSerial;

		private UserBtnControls userBtnSmart;

		private Label labelNet;

		private Label labelExternPort2;

		private Label labelExternPort1;

		private ComboBox comboBoxExternPort2;

		private ComboBox comboBoxExternPort1;

		private Label labelResetNetwork;

		private Button buttonResetNetwork;

		private RoundButton roundButtonResetNetwork;

		private Label labelEcoMode;

		private UserBtnControls userBtnEcoMode;

		private Label labelSerialImg;

		private Label labelEcoModeImg;

		private Label labelBootStartImg;

		private Label labelCheckUpdateImg;

		private Label labelNetImg;

		private Label labelScreenImg;

		private Label labelBECLeftRightImg;

		private Label labelBECLeftRight;

		private UserBtnControls userBtnBECLeftRight;

		private Label labelBECTopBottomImg;

		private Label labelBECTopBottom;

		private UserBtnControls userBtnBECTopBottom;

		private Label labelVersions;

		private Label labelOTAVersions;

		private Label labelOTATime;

		private Label labelPid;

		private UserBtnControls userBtnControlsColorEnhance;

		private Label labelColorEnhance;

		private Panel panelScreen;

		private FlowLayoutPanel flowLayoutPanel1;

		private Panel panel1;

		private Panel panel2;

		private PictureBox pictureBoxDie;

		private NewPanel newPanel1;

		private Label labelLightLocation;

		private Label labelDie;

		private PictureBox labelColorEnhanceImg;

		private PictureBox pictureBox1;

		private PictureBox pictureBoxInstallationImg;

		private Label labelInstallation;

		private NewPanel newPanel2;

		private PictureBox pictureBox2;

		private Label labelLampLocation;

		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UserSettingControls(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InitControls()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InitParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initExternPort()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updateSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initUI()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void upComboBoxLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeSerial()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateExternPort()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updateExterport()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeDisplay()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxLightStrip_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxStandLamp_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSelect1_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSelect2_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxLeft_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBoxRight_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void comboBoxLanguage_SelectionChangeCommitted(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void comboBoxSerial_SelectionChangeCommitted(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void comboBoxScreen_SelectionChangeCommitted(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnControls1_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UserSettingControls_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScreenSelect()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateInterConnect()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnSmart_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void comboBoxExternPort1_SelectionChangeCommitted(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void comboBoxExternPort2_SelectionChangeCommitted(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void buttonResetNetwork_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnUpdate_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnEcoMode_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelImgPublic_MouseEnter(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelImgPublic_MouseLeave(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string CutStr(string str, int len)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnBECLeftRight_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnBECTopBottom_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnControlsColorEnhance_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updatelabelOTAVersions()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScreen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox1_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox2_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static UserSettingControls()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class UserAdvanceControls : UserControl
	{
		private Point pictureBoxPoint;

		private Size pictureBoxNormalSize;

		private Size pictureBoxUSelectSize;

		private Point pictureBoxUSelectPoint;

		private Size panelSize;

		private int m_select;

		private Point textOffsetPoint;

		private float labelRate;

		private LanguageEngine language;

		private Storage storage;

		private UIEventController uIEventController;

		private Statistics statistics;

		private UserControl[] PanelControls;

		private int SubScen;

		private IniPublicClass IniPublicClass;

		private bool pictureBoxswitch;

		private bool load;

		private IContainer components;

		private NewPanel panelScreen;

		private PictureBox pictureBoxScreen;

		private NewPanel panelMusic;

		private PictureBox pictureBoxMusic;

		private NewPanel panelKeys;

		private PictureBox pictureBoxKeys;

		private Label labelScreen;

		private Label labelMusic;

		private Label labelKey;

		private NewPanel panelContent;

		private DimmerTrackBar dimmerTrackBar;

		private UserBtnControls userBtnControls;

		private NewPanel panel_img;

		private Label label_titletext;

		private NewPanel panel1;

		private NewPanel panelWhiteLight;

		private Label labelWhiteLight;

		private PictureBox pictureBoxWhiteLight;

		private PictureBox labelSwitch;

		public int USelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UserAdvanceControls(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initUI()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScenSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initThread()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void threadHandle()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updatePanel()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void pictureBox_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SetPanelContent(int uselect)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void label_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateUserFunnyControlpanel_img(Image backgroundimage, string text)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initPictureBoxImgage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void labelSwitch_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updateLabelSwitch(bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void simulationPictureBox()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UserAdvanceControls_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MusicTypeCut()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static UserAdvanceControls()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class UserBtnControls : UserControl
	{
		[Description("事件委托")]
		[Category("自定义")]
		public delegate void ButtonClickFunc(bool nowStatus);

		public ButtonClickFunc ButtonClick;

		private int currentValue;

		private int targetValue;

		private int buttonSize;

		private int buttonOffset;

		private bool m_value;

		private bool m_FontVisible;

		private Color m_BackgroundColor;

		private Color m_SelectedColor;

		private string m_SelectedText;

		private Color m_UnSelectedColor;

		private string m_UnSelectedText;

		public bool switch_Value;

		private IContainer components;

		[Category("自定义")]
		[Description("开关")]
		public bool Value
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Description("字是否显示")]
		[Category("自定义")]
		public bool FontVisible
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Category("自定义")]
		[Description("背景色")]
		public Color BackgroundColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Description("选中色")]
		[Category("自定义")]
		public Color SelectedColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Category("自定义")]
		[Description("选中文本")]
		public string SelectedText
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Description("未选中色")]
		[Category("自定义")]
		public Color UnSelectedColor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return (Color)(object)null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[Description("未选中文本")]
		[Category("自定义")]
		public string UnSelectedText
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UserBtnControls()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initThreadChange()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void BtnHandle()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UserBtnControls_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UserBtnControls_MouseClick(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static UserBtnControls()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Views.UserControls.AdvanceControls
{
	internal class AdvanceControlKeyboard : UserControl
	{
		private Color parentColor;

		private LanguageEngine language;

		private UIEventController uiEvent;

		private Storage storage;

		private int select;

		private IContainer components;

		private NewPanel panel3;

		private Label labelMouseAndKeyboard;

		private NewPanel panel2;

		private Label labelOnlyKeyboard;

		private NewPanel panel1;

		private Label labelOnlyMouse;

		public new int Select
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AdvanceControlKeyboard(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		~AdvanceControlKeyboard()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelPaint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelUnSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void LabelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static AdvanceControlKeyboard()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class AdvanceControlLED : UserControl
	{
		private string labelWhiteLightText;

		private int pointx;

		private int pointy;

		private bool bool_down;

		private int radius;

		private LanguageEngine language;

		private IniPublicClass publicClass;

		private int select;

		private IContainer components;

		private Label labelLevel2;

		private Label labelLevel1;

		private Label labelLevel0;

		private NewPanel panelSetCCT;

		private NewPanel panel3;

		private NewPanel panel2;

		private NewPanel panel1;

		private Label label1;

		private PictureBox pictureBoxLevel2img;

		private PictureBox pictureBoxLevel1img;

		private PictureBox pictureBoxLevel0img;

		public new int Select
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AdvanceControlLED(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSetCCT_MouseDown(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSetCCT_MouseUp(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSetCCT_MouseMove(object sender, MouseEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSetCCT_Paint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelPaint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelUnSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void LabelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void PictureBoxClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updatePictureBoxImage(int select)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AdvanceControlLED_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static AdvanceControlLED()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class AdvanceControlMovice : UserControl
	{
		private string labelTextUserAdvanceControls;

		private int select;

		private Color parentColor;

		private LanguageEngine language;

		private UIEventController uIEventController;

		private Storage storage;

		private Statistics statistics;

		private IContainer components;

		private NewPanel panel1;

		private NewPanel panel2;

		private NewPanel panel3;

		private Label labelLevel0;

		private Label labelLevel1;

		private Label labelLevel2;

		private PictureBox pictureBoxLevel0img;

		private PictureBox pictureBoxLevel1img;

		private PictureBox pictureBoxLevel2img;

		public new int Select
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AdvanceControlMovice(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScenSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelPaint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelUnSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void LabelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void PictureBoxClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AdvanceControlMovice_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updatePictureBoxImage(int select)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static AdvanceControlMovice()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class AdvanceControlMusic : UserControl
	{
		private string labelTextUserAdvanceControls;

		private int select;

		private Color parentColor;

		private LanguageEngine language;

		private UIEventController uIEventController;

		private Storage storage;

		private Statistics statistics;

		private IContainer components;

		private NewPanel panel3;

		private Label labelSmooth;

		private NewPanel panel2;

		private Label labelElectronic;

		private NewPanel panel1;

		private Label labelFreq;

		private PictureBox pictureBoxFreqimg;

		private PictureBox pictureBoxSmoothimg;

		private PictureBox pictureBoxElectronicimg;

		private UserBtnControls userBtnControls1;

		public new int Select
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AdvanceControlMusic(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initParam()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateScenSetting()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ChangeScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void initLanguage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelPaint(object sender, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelUnSelect(Panel panel, PaintEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void panelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void LabelClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void PictureBoxClick(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AdvanceControlMusic_Load(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void updatePictureBoxImage(int select)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void userBtnControls1_Click(object sender, EventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void Dispose(bool disposing)
		{
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private void InitializeComponent()
		{
		}

		static AdvanceControlMusic()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Views.LibControls
{
	public static class ControlHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void SetGDIHigh(this Graphics g)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static GraphicsPath CreateRoundedRectanglePath(this Rectangle rect, int cornerRadius)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void DrawPanelSelect(this Graphics g, Panel panel, Color color, int weight = 2)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void FillPanelSelect(this Graphics g, Panel panel, Color color, int weight = 2)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static GraphicsPath CreateRoundedRectanglePath(this RectangleF rect, int cornerRadius)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static int GetLabelWidth(Label label)
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void LayoutLabelCenter(Label label)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void LayoutLabelRight(Label label, int offset = 0)
		{
		}

		static ControlHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.Tuya
{
	internal class TuyaWriter
	{
		public enum ScenMode
		{
			ScenMode_Screen_Level0,
			ScenMode_Screen_Level1,
			ScenMode_Screen_Level2,
			ScenMode_Music_Freq,
			ScenMode_Music_Elect,
			ScenMode_Music_Smooth,
			ScenMode_Rainbow,
			ScenMode_Fire,
			ScenMode_Lighting,
			ScenMode_Firework,
			ScenMode_Star,
			ScenMode_Water,
			ScenMode_Particle,
			ScenMode_Fluid,
			ScenMode_Gravity,
			ScenMode_Newton,
			ScenMode_Breath,
			ScenMode_Static_Color
		}

		private enum DataType
		{
			DataType_Bool = 1,
			DataType_Int,
			DataType_String
		}

		public class HSV
		{
			public int h;

			public int s;

			public int v;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public HSV(int h, int s, int v)
			{
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public HSV()
			{
			}

			static HSV()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		public class RGB
		{
			public byte red;

			public byte green;

			public byte blue;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public RGB(byte red, byte green, byte blue)
			{
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public RGB()
			{
			}

			static RGB()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GetScenModeData(ScenMode mode)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GetDimmer(int dimmer)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GetColor(byte red, byte green, byte blue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public HSV RGB2HSV(byte red, byte green, byte blue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public RGB HSV2RGB(int h, int s, int v)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private byte[] initScenMode(int mode)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private List<byte> addHead(List<byte> list)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private List<byte> addCommond(List<byte> list, byte commond)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private List<byte> addDataLen(List<byte> list, int len)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private List<byte> addDp(List<byte> list, byte dp, DataType type, List<byte> data)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private List<byte> addInt2Hex(List<byte> list, int value)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private List<byte> addSumCheck(List<byte> list)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public TuyaWriter()
		{
		}

		static TuyaWriter()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.Razer
{
	internal class RazerControl
	{
		private RzChromaBroadcastAPI rzChromaBroadcastAPI;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void MonitorType(bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UnInit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Api_ConnectionChanged(object sender, RzChromaBroadcastConnectionChangedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Api_ColorChanged(object sender, RzChromaBroadcastColorChangedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public RazerControl()
		{
		}

		static RazerControl()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.LFramework.LMode
{
	internal class BulbLModeHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static List<ILMode> ILModeFactory(LRenderSet renderSet)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbLModeHelper()
		{
		}

		static BulbLModeHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampLModeHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static List<ILMode> ILModeFactory(LRenderSet renderSet)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampLModeHelper()
		{
		}

		static FloorLampLModeHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeAuroraBorealis : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private readonly int dlt;

		private int colorIndex;

		private int index;

		private int length;

		private Color greenRecord;

		private List<Color> listColor;

		private bool type;

		private int sleep;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeAuroraBorealis()
		{
		}

		static ILModeAuroraBorealis()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.LFramework.LMode.LightLMode
{
	internal class LightILModeChase : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private int index;

		private bool direction;

		private int colorIndex;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeChase()
		{
		}

		static LightILModeChase()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightILModeMovie : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		public Color[] LeftColor;

		public Color[] TopColor;

		public Color[] BottomColor;

		public Color[] RightColor;

		private ScreenCapture screenCapture;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private int GCD(int Width, int Height)
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void GetColors(ref Color[] colors, Color[] topColor, Color[] bottomColor, Color[] leftColor, Color[] rightColor, int allRenderLen)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeMovie()
		{
		}

		static LightILModeMovie()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightLModeHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static List<ILMode> ILModeFactory(LRenderSet renderSet)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightLModeHelper()
		{
		}

		static LightLModeHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightILModeMusic : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int colorIndex;

		private AudioCapture audioCapture;

		private int wavresultrecord;

		private int conversionCount;

		private EN_DIR direction;

		private int dimmer;

		private int oldWavresult;

		private Random random;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeMusic()
		{
		}

		static LightILModeMusic()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightILModeRainbow : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int step;

		private int colorIndex;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeRainbow()
		{
		}

		static LightILModeRainbow()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightILModeSeasons : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int indexRecord;

		private Color[] color;

		private bool ifon;

		private int runTime;

		private Thread threadtime;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void threadTime()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeSeasons()
		{
		}

		static LightILModeSeasons()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightlLModeFirework : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private int step;

		private int dlt;

		private int colorIndex;

		private int indexlog;

		private int len;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightlLModeFirework()
		{
		}

		static LightlLModeFirework()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightILModeAuroraBorealis : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private readonly int dlt;

		private int colorIndex;

		private int index;

		private int length;

		private Color greenRecord;

		private List<Color> listColor;

		private bool type;

		private int sleep;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeAuroraBorealis()
		{
		}

		static LightILModeAuroraBorealis()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightILModeSet : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		public Color[] LeftColor;

		public Color[] TopColor;

		public Color[] BottomColor;

		public Color[] RightColor;

		public Color[] R;

		public Color[] G;

		public Color[] B;

		public Color[] A;

		private ScreenCapture screenCapture;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private int GCD(int Width, int Height)
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void GetColors(ref Color[] colors, Color[] topColor, Color[] bottomColor, Color[] leftColor, Color[] rightColor, int allRenderLen)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeSet()
		{
		}

		static LightILModeSet()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.LFramework.LMode.LampLMode
{
	internal class ILModeChase : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private int index;

		private bool direction;

		private bool lefttype;

		private int colorIndex;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeChase()
		{
		}

		static ILModeChase()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeMovie : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		public Color[] leftColor;

		public Color[] rightColor;

		private ScreenCapture screenCapture;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeMovie()
		{
		}

		static ILModeMovie()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeMusic : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int colorIndex;

		private AudioCapture audioCapture;

		private int wavresultrecord;

		private int dimmer;

		private int oldWavresult;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeMusic()
		{
		}

		static ILModeMusic()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeSeasons : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int indexRecord;

		private Color[] color;

		private bool ifon;

		private int runTime;

		private Thread threadtime;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void threadTime()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeSeasons()
		{
		}

		static ILModeSeasons()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class lLModeFirework : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private int step;

		private int dlt;

		private int colorIndex;

		private int indexlog;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public lLModeFirework()
		{
		}

		static lLModeFirework()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeFire : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private int dlt;

		private Random random;

		private List<Color> colors;

		private EN_DIR direction;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeFire()
		{
		}

		static ILModeFire()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LightILModeFire : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private int dlt;

		private Random random;

		private List<Color> colors;

		private EN_DIR direction;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightILModeFire()
		{
		}

		static LightILModeFire()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeRainbow : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int step;

		private int colorIndex;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeRainbow()
		{
		}

		static ILModeRainbow()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.LFramework.LMode.FloorLampLMode
{
	internal class FloorLampILModeAuroraBorealis : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private readonly int dlt;

		private int colorIndex;

		private int index;

		private int length;

		private Color greenRecord;

		private bool type;

		private int sleep;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeAuroraBorealis()
		{
		}

		static FloorLampILModeAuroraBorealis()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeChase : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private int index;

		private bool direction;

		private bool lefttype;

		private int colorIndex;

		private int length;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeChase()
		{
		}

		static FloorLampILModeChase()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeFire : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private int dlt;

		private Random random;

		private List<Color> colors;

		private EN_DIR direction;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeFire()
		{
		}

		static FloorLampILModeFire()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeMovie : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		public Color[] LeftColor;

		public Color[] TopColor;

		public Color[] BottomColor;

		public Color[] RightColor;

		private ScreenCapture screenCapture;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeMovie()
		{
		}

		static FloorLampILModeMovie()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeMusic : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int colorIndex;

		private AudioCapture audioCapture;

		private EN_DIR direction;

		private int dimmer;

		private int oldWavresult;

		private int wavresultrecord;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeMusic()
		{
		}

		static FloorLampILModeMusic()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeRainbow : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int step;

		private int colorIndex;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeRainbow()
		{
		}

		static FloorLampILModeRainbow()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeSeasons : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int indexRecord;

		private Color[] color;

		private bool ifon;

		private int runTime;

		private Thread threadtime;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void threadTime()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeSeasons()
		{
		}

		static FloorLampILModeSeasons()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeSet : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		public Color[] LeftColor;

		public Color[] TopColor;

		public Color[] BottomColor;

		public Color[] RightColor;

		public Color[] R;

		public Color[] G;

		public Color[] B;

		public Color[] A;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeSet()
		{
		}

		static FloorLampILModeSet()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class FloorLampILModeFirework : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private int step;

		private int dlt;

		private int colorIndex;

		private int indexlog;

		private int len;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLamp()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLampILModeFirework()
		{
		}

		static FloorLampILModeFirework()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.LFramework.LMode.ExternalMode
{
	[Serializable]
	internal class ExternalTemplate
	{
		public int frames_number;

		public int frames_rate;

		private List<Color[]> listColors;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ExternalTemplate()
		{
		}

		static ExternalTemplate()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class LFrame : ICloneable
	{
		public int Rows;

		public int Columns;

		private int fps;

		private List<Color[]> listColors;

		private int length;

		public int Frames
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
		}

		public int FPS
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LFrame(int rows, int columns)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Add(Color[] colors)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Update(Color[] colors, int index)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Color[] Get(int frame)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public object Clone()
		{
			return null;
		}

		static LFrame()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LFrameLoader
	{
		public delegate void LFrameLoaderFinishEventHandle(bool success, string info, LFrame frame);

		public LFrameLoaderFinishEventHandle LFrameLoaderFinish;

		private readonly string fileName;

		private LFrame lFrame;

		private bool finishParser;

		private object lockObject;

		private Semaphore semaphore;

		private int stateStatus;

		private int stateSubStatus;

		private string storeKey;

		private int storeValue;

		private Dictionary<string, int> keyValue;

		private List<Color> listColor;

		private int[] rgb;

		private int rgbIndex;

		private int pixels;

		private bool errorStatus;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LFrameLoader(string fileName)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LFrame GetLFrame()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runParser(string fileName)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void stateMachine(string text)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void parserArray(char s)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void tryInitLFrame()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void tryAddColor()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void finishParserInfo(bool success, string info, LFrame frame = null)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void store(string key, int value)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private int load(string key)
		{
			return 0;
		}

		static LFrameLoader()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.LFramework.LMode.CommonLMode
{
	internal class ILModeLED : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		public int loadSon;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeLED()
		{
		}

		static ILModeLED()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeMainColor : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private readonly int dlt;

		private int colorIndex;

		private List<Color> listColor;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeMainColor()
		{
		}

		static ILModeMainColor()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeRomance : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private List<Color> colors;

		private int index;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeRomance()
		{
		}

		static ILModeRomance()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeVitality : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private int colorIndex;

		private List<Color> listColor;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeVitality()
		{
		}

		static ILModeVitality()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeWarm : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private readonly int dlt;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeWarm()
		{
		}

		static ILModeWarm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeBreathing : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private readonly int dlt;

		private Color color;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeBreathing()
		{
		}

		static ILModeBreathing()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeFlow : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private int step;

		private int dlt;

		private int colorIndex;

		private Color foreColor;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeFlow()
		{
		}

		static ILModeFlow()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ILModeRead : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILModeRead()
		{
		}

		static ILModeRead()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Vendor.LFramework.LMode.BulbLMode
{
	internal class BulbILModeAuroraBorealis : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private EN_DIR direction;

		private int dimmer;

		private readonly int dlt;

		private int colorIndex;

		private int index;

		private int length;

		private Color greenRecord;

		private List<Color> listColor;

		private bool type;

		private int sleep;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runBulb()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeAuroraBorealis()
		{
		}

		static BulbILModeAuroraBorealis()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeRainbow : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int step;

		private int colorIndex;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runLight()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeRainbow()
		{
		}

		static BulbILModeRainbow()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeSky : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private List<Color> colors;

		private int index;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeSky()
		{
		}

		static BulbILModeSky()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeFire : ILMode
	{
		private enum EN_DIR
		{
			EN_DIR_UP,
			EN_DIR_DOWN
		}

		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private int dlt;

		private Random random;

		private List<Color> colors;

		private EN_DIR direction;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runBulb()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeFire()
		{
		}

		static BulbILModeFire()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeMovie : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private ScreenCapture screenCapture;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runBulb()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeMovie()
		{
		}

		static BulbILModeMovie()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeMusic : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dlt;

		private int colorIndex;

		private AudioCapture audioCapture;

		private int wavresultrecord;

		private int conversionCount;

		private MusicData ele;

		private int dimmer;

		private int oldWavresult;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runBulb()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeMusic()
		{
		}

		static BulbILModeMusic()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class MusicData
	{
		public float power_average;

		public float power_max;

		public float percentage;

		public float change;

		public bool power_lock;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public MusicData()
		{
		}

		static MusicData()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeOcean : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private List<Color> colors;

		private int index;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeOcean()
		{
		}

		static BulbILModeOcean()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeSeasons : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int indexRecord;

		private Color[] color;

		private bool ifon;

		private int runTime;

		private Thread threadtime;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void threadTime()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void runBulb()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeSeasons()
		{
		}

		static BulbILModeSeasons()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BulbILModeForest : ILMode
	{
		public new int ModeId;

		public new string ModeName;

		private int dimmer;

		private readonly int dlt;

		private List<Color> colors;

		private int index;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Quit()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void run()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbILModeForest()
		{
		}

		static BulbILModeForest()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Properties
{
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		internal static Bitmap _return
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap add_grouping
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap background_MouseEnter
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap background_MouseLeave
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap background_MouseLeave_2
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap ball_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap ball_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Icon Beelight
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Breathe_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Breathe_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap bulb
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap bulblight
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap button
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap close
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap color_picker
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap color_temperature
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap colour_disk
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap colour_disk_box
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap colour_disk_coil
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap connected
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap connectedall
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap connectedlamp
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap connectedlight
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap dimmer
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap DisplayBackground
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Displayer
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap DisplayInstallationBackground
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Drip_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Drip_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap entrance
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_move
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_move_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_move_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_move_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_music
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_music_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_music_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap feast_music_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Fireworks_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Fireworks_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Flame_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Flame_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap flow_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap flow_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap grouping
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap grouping_off
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap grouping_on
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap HAU_BackgroundImage
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap hiden
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_about_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_about_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_aurora_borealis
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_aurora_borealis_inactive
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_chase
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_chase_inactive
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_connect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_device_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_device_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_exit_black_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_exit_black_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_exit_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_exit_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_main_color
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_main_color_inactive
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_main_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_main_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_minimality_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_minimality_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_mood_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_mood_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_doppel
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_high
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_low
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_mid
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_multipoint
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_movie_point
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_music_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_music_high
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_music_low
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_music_mid
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_music_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_not_connect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_romance
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_romance_inactive
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_scenes_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_scenes_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_seasons
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_seasons_inactive
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_set_up_hover
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_set_up_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_switch_off
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_switch_on
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_update
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_vitality
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_vitality_inactive
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_warm
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap icon_warm_inactive
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_aurora_borealis
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_aurora_borealis_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_ball_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_ball_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_breathe_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_breathe_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_chase
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_chase_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_drip_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_drip_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_fire_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_fire_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_firework_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_firework_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_flow_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_flow_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_lamp
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led_color
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led_move
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led_read
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led_rest
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_led_work
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_left_side
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_light
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_main_color
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_main_color_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_mood_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_mood_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_movie_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_movie_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_music_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_music_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_particle_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_particle_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_pure_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_pure_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_rainbow_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_rainbow_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_read_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_read_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_right_side
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_romance
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_romance_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_seasons
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_seasons_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_star_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_star_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_swing_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_swing_mode
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_vitality
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_vitality_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_warm
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap img_warm_back
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap InstallationBackground
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap lamp
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap lampnetwork
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap light
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Icon lightmi
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap lightnetwork
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap loading
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap lytmilogo
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap multi_select_checked
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap multi_select_checked_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap newBulb_off
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap newBulb_on
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap newBulbLight_off
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap newBulbLight_on
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap newLamp_off
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap newLamp_on
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap normal
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Particle_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Particle_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap pattern_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap pictureBox1_BackgroundIamge
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap pictureBox2_BackgroundIamge
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap pictureBox3_BackgroundIamge
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap pure_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap pure_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap question_mark
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap radio_check
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap radio_check_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap radio_check_nor0
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap radio_check0
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Rainbow_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Rainbow_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Read_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Read_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap redact
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap shuzi1
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap shuzi2
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap shuzi3
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap shuzi4
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Star_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Star_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Swing_active
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap Swing_nor
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap switch_off
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap switch_on
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap test
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap TransparentLayer
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap warning
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap xiangshang
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap xiangxia
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap xiangyou
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		internal static Bitmap xiangzuo
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal Resources()
		{
		}

		static Resources()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance;

		public static Settings Default
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Settings()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Settings()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
		}
	}
}
namespace beelight.LightDevice.Device
{
	public class DeviceDefaultLanguage
	{
		public string DefaultChinese;

		public string DefaultEnglish;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceDefaultLanguage(string chinese, string english)
		{
		}

		static DeviceDefaultLanguage()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class DeviceData
	{
		public string ClassName;

		public string Name;

		public Image Ico;

		public Image Ico_Off;

		public Image ErrorImg;

		public bool Illumination;

		public bool Installation;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceData(string className, string name, Image ico, Image ico_off, Image errorimg, bool illumination, bool installation)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public virtual object GetDevice(object driver)
		{
			return null;
		}

		static DeviceData()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class Lamp : DeviceData
	{
		public static string name;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Lamp(string className)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object GetDevice(object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Lamp()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		}
	}
	public class LampNetwork : DeviceData
	{
		public static string name;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LampNetwork(string className)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object GetDevice(object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static LampNetwork()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		}
	}
	public class Light : DeviceData
	{
		public static string name;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Light(string className)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object GetDevice(object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Light()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		}
	}
	public class LightNetwork : DeviceData
	{
		public static string name;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LightNetwork(string className)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object GetDevice(object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static LightNetwork()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		}
	}
	public class BulbLight : DeviceData
	{
		public static string name;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BulbLight(string className)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object GetDevice(object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static BulbLight()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		}
	}
	public class Bulb : DeviceData
	{
		public static string name;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bulb(string className)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object GetDevice(object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Bulb()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		}
	}
	public class FloorLamp : DeviceData
	{
		public static string name;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public FloorLamp(string className)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object GetDevice(object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static FloorLamp()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		}
	}
	public class Device
	{
		public enum ILModeIndex
		{
			ModeMovie = 12,
			ModeMusic,
			Breathing
		}

		public static Device device1;

		public static object m_lock;

		public const int whiteBright = 1000;

		public const int temperValue = 350;

		public static string[] lamp;

		public static string[] light;

		public static string[] bulb;

		public static string[] lightnetwork;

		public static string[] bulblight;

		public static string[] floorlamp;

		public static string[] lampnetwork;

		public Dictionary<string, DeviceData> keyValuePairs;

		public Dictionary<string, DeviceDefaultLanguage> keyValuePairsDeviceDefaultLanguage;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Device()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Device GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceData GetDeviceData(string pid)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public object GetIDevice(string pid, object driver)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceData GetClass(string className, string name)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string GetDefaultLanguage(string pid, string language)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Device()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			if (DyyVDbaRvM1YfIq9il.mCC9ZT9yx(976))
			{
				xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
				device1 = null;
				m_lock = new object();
				lamp = new string[1] { DyyVDbaRvM1YfIq9il.KX0HrYNeb(20736) };
				light = new string[2]
				{
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(20772),
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(20808)
				};
				bulb = new string[2]
				{
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(20844),
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(20880)
				};
				lightnetwork = new string[2]
				{
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(20916),
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(20952)
				};
				bulblight = new string[2]
				{
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(20988),
					DyyVDbaRvM1YfIq9il.KX0HrYNeb(21024)
				};
				floorlamp = new string[1] { DyyVDbaRvM1YfIq9il.KX0HrYNeb(21060) };
				lampnetwork = new string[1] { "" };
			}
		}
	}
	internal class DeviceBulb : IDevice
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceBulb(IDriver driver, List<ILMode> modes = null)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void syncConfigAckListener(object o, int sumPixel, int channels, int[] channelPixel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void SyncRender()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void DefaultLMode()
		{
		}

		static DeviceBulb()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class DeviceFloorLamp : IDevice
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceFloorLamp(IDriver driver, List<ILMode> modes = null)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void syncConfigAckListener(object o, int sumPixel, int channels, int[] channelPixel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void SyncRender()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void DefaultLMode()
		{
		}

		static DeviceFloorLamp()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class DeviceKeyData
	{
		public static string[] config;

		public static string key;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceKeyData()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static DeviceKeyData()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			config = new string[6]
			{
				DyyVDbaRvM1YfIq9il.KX0HrYNeb(18840),
				DyyVDbaRvM1YfIq9il.KX0HrYNeb(18878),
				DyyVDbaRvM1YfIq9il.KX0HrYNeb(14140),
				DyyVDbaRvM1YfIq9il.KX0HrYNeb(18910),
				DyyVDbaRvM1YfIq9il.KX0HrYNeb(14152),
				DyyVDbaRvM1YfIq9il.KX0HrYNeb(20336)
			};
			key = DyyVDbaRvM1YfIq9il.KX0HrYNeb(21164);
		}
	}
	internal class DeviceLight : IDevice
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceLight(IDriver driver, List<ILMode> modes = null)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void syncConfigAckListener(object o, int sumPixel, int channels, int[] channelPixel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void SyncRender()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void DefaultLMode()
		{
		}

		static DeviceLight()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class DeviceLamp : IDevice
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public DeviceLamp(IDriver driver, List<ILMode> modes = null)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void syncConfigAckListener(object o, int sumPixel, int channels, int[] channelPixel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected override void SyncRender()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void DefaultLMode()
		{
		}

		static DeviceLamp()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Language
{
	internal class LanguageEngine
	{
		private class language
		{
			private Dictionary<string, string> dicts;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public string GetString(string id)
			{
				return null;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public List<string> GetKeys()
			{
				return null;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public List<string> GetValues()
			{
				return null;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public void AddBind(string id, string name)
			{
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public language()
			{
			}

			static language()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private static object _lock_;

		private static LanguageEngine languageEngine;

		private Dictionary<string, language> dictionaries;

		public static string filename;

		private language currentLanguage;

		private string languageName;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private LanguageEngine()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static LanguageEngine GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void TryLoadFilePath()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ChangeLanguage(string name)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string GetLanguageName()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<string> GetKeys()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<string> GetValues()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<string> GetLanguageKeys()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<string> GetLanguageValues()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string GetString(string id)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void loadText()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private string GetLabel(char[] str, int start, out int end)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static LanguageEngine()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			_lock_ = new object();
			languageEngine = null;
			filename = DyyVDbaRvM1YfIq9il.KX0HrYNeb(21328);
		}
	}
}
namespace beelight.Library
{
	internal class AutoStartHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SetMeStart(bool onOff)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool SetAutoStart(bool onOff, string appName, string appPath)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetAppllictionPath()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool IsExistKey(string keyName)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool SelfRunning(bool isStart, string exeName, string path)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool SelfRunning(string exeName, string path)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AutoStartHelper()
		{
		}

		static AutoStartHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BeelightProtocol
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetLargeScreenPackage(IntPtr screenData, int row, int column, int channel)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetLargeScreenPackage(IntPtr screenData, int row, int column, int channel, byte top_bottom, byte left_right)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetNetScreenPackage()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetAudioPackage(int min, int max, int hz500, int hz2000, int hz4000)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetScenPackage(byte scenid, byte dimmer, byte red, byte green, byte blue, byte power)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetSettingsPackage(byte[] settings)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetNetScenPackage(byte scenid, byte dimmer, byte red, byte green, byte blue, byte power, byte white)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static byte[] GetResetFactoryPackage()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GetEcoPackage(bool enable)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GetConnectPackage()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GetScreenSizeSettingPackage(IniPublicClass iniPublicClass)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Unpack(ref byte[] package)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void LogDebug()
		{
		}

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_large_screenbuf_package_length();

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_middle_screenbuf_package_length();

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_large_screendata_package(IntPtr pscreen, int row, int column, int channel, IntPtr pout);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_large_screendata_package_ex(IntPtr pscreen, int row, int column, int channel, IntPtr pout, byte top_bottom, byte left_right);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_middle_screendata_package(IntPtr pout);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_audiodata_package(int min, int max, int hz500, int hz2000, int hz4000, IntPtr pout);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_scen_package(byte scenid, byte dimmer, byte red, byte green, byte blue, byte power, IntPtr pout);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_setting_package(IntPtr setting, int settinglen, IntPtr pout);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern void unpack_package(IntPtr package);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_top_start_line();

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_bottom_start_line();

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int get_net_udp_scen_package(byte scenid, byte dimmer, byte red, byte green, byte blue, byte power, IntPtr pout, byte white);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern void get_reset_factory_package(IntPtr package);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern void get_screen_size_setting_package(int w, int h, IntPtr pout);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern void get_eco_setting_package(byte enable, IntPtr pout);

		[DllImport("beelightLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern void get_connect_package(IntPtr pout);

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static byte[] GenColorFrame(byte channel, int pixels, RGB[] Buf)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] getFrame(byte ack, byte cmd, byte[] data)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static byte[] GenFrame(byte ack, byte cmd, byte[] data)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] protocol(byte[] datas)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static byte[] Protocol(byte[] datas)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetStringInfo(char[] datas)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetHexInfo(byte[] datas, int offset, int length)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetDateTime(long timeStamp)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BeelightProtocol()
		{
		}

		static BeelightProtocol()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class IniPublicClass
	{
		public UIEventController uIEventController;

		public Storage storage;

		public Statistics statistics;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IniPublicClass()
		{
		}

		static IniPublicClass()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class RGB
	{
		private byte r;

		private byte g;

		private byte b;

		public byte R
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public byte G
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public byte B
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public RGB()
		{
		}

		static RGB()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ScreenData
	{
		public byte[] TopBuffer;

		public byte[] LeftBuffer;

		public byte[] RightBuffer;

		public byte[] BottomBuffer;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ScreenData()
		{
		}

		static ScreenData()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Library.Protocol
{
	internal class Protocol
	{
		public enum ProtocolType
		{
			ProtocolTypeNone,
			ProtocolTypeData,
			ProtocolTypeScen,
			ProtocolTypeConfig,
			ProtocolTypeShakeHand
		}

		public class ShakeHandPackage
		{
			public int Len;

			public StringBuilder DeviceName;

			public bool LightStrip;

			public bool LeftSetup;

			public byte Scen;

			public byte Dimmer;

			public Color MColor;

			public bool PowerStatus;

			public bool Error;

			public byte ExternPort1;

			public byte ExternPort2;

			public byte VersionMaster;

			public byte VersionMinor;

			public byte VersionRevision;

			public string Version;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public ShakeHandPackage(byte[] datas)
			{
			}

			static ShakeHandPackage()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		public class ScenPackage
		{
			public byte Scen;

			public byte Dimmer;

			public Color MColor;

			public bool PowerStatus;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public ScenPackage(byte[] datas)
			{
			}

			static ScenPackage()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private int Status;

		public int Len;

		private int count;

		public bool Finish;

		private int index;

		private byte[] buf;

		private byte[] Head;

		public byte[] Data;

		public ShakeHandPackage ShakeHande;

		public IDevice idevice;

		public ScenPackage Scen;

		public bool type;

		public ProtocolType protocolType;

		public byte DataType;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Protocol()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ClearStatus()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Add(byte[] data)
		{
		}

		static Protocol()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Driver.Screen
{
	internal class ColorTools
	{
		private class ColorBuf
		{
			public int red;

			public int green;

			public int blue;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public ColorBuf()
			{
			}

			static ColorBuf()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private static int ROWS;

		private static int COLUMNS;

		private static int ZIP_ROWS;

		private static int ZIP_COLUMNS;

		private static int DLT_ROW;

		private static int DLT_COLUMN;

		private static int ERR_ROW;

		private static int ERR_COLUMN;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Color[] spaceConversion(Color[] colors, double value = 1.0)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Color[] getZipLineColor(Color[] colors, int zipNums)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Color[] getColumnColor(Color[] colors)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Color[] getRowColor(Color[] colors)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Color ColorStrengthen(Color color)
		{
			return (Color)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Color ColorHsv2RGB(int h, int s, int v)
		{
			return (Color)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ColorTools()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static ColorTools()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			ROWS = 18;
			COLUMNS = 32;
			ZIP_ROWS = 4;
			ZIP_COLUMNS = 6;
			DLT_ROW = ROWS / ZIP_ROWS;
			DLT_COLUMN = COLUMNS / ZIP_COLUMNS;
			ERR_ROW = (ROWS << 8) / ZIP_ROWS - (DLT_ROW << 8);
			ERR_COLUMN = (COLUMNS << 8) / ZIP_COLUMNS - (DLT_COLUMN << 8);
		}
	}
	internal class myData
	{
		public byte[] all;

		public Color[] top;

		public Color[] bottom;

		public Color[] left;

		public Color[] right;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public myData()
		{
		}

		static myData()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class ScreenCapture
	{
		public delegate void CallbackDelegate(IntPtr Image, int width, int height, int RowPitch, int ScreenNumber);

		public class BufferData
		{
			public IntPtr Image;

			public int Width;

			public int Height;

			public int RowPitch;

			public int ScreenNumber;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public BufferData()
			{
			}

			static BufferData()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		protected class Buffer
		{
			public BufferData bufferData;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public Buffer(IntPtr image, int width, int height, int rowPitch, int screenNumber)
			{
			}

			static Buffer()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private static object lockObject;

		private static ScreenCapture screenCapture;

		private static Dictionary<int, myData> keyValuePairs;

		private static Dictionary<int, Bitmap> Thumbnail;

		private System.Windows.Forms.Screen[] itemScreen;

		private ScreenshotCapture screenshotCapture;

		private int fps;

		private static BitmapData bitmapData;

		private static bool BECTopBottom;

		private static bool BECLeftRight;

		private static byte[] largebuf;

		private static LBitmap lBitmap;

		private static myData myData;

		private static Stopwatch sw;

		private Color[] colors;

		private LProtocolSyncRGB lProtocolSyncRGB;

		public static int dxgifps;

		public CallbackDelegate callbackDelegate;

		private static int GXGIMonitorTime;

		public ManualResetEvent GXGIMonitorAre;

		private object Lock;

		private int queueMaxLength;

		private Queue<Buffer> queue;

		private object queueLock;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private ScreenCapture()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static ScreenCapture GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void screenShot()
		{
		}

		[DllImport("DesktopDuplication.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		public static extern void SetRegisterFunctionCallback(CallbackDelegate callback);

		[DllImport("DesktopDuplication.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		public static extern int Initialize();

		[DllImport("DesktopDuplication.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
		private static extern int SetSwitch(int type);

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public static void CallBackFunction(IntPtr Image, int width, int height, int RowPitch, int ScreenNumber)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void GXDIMonitorThread()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void screenShotGXDI()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public myData GetKeyValuePairs(int nameindex)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Rectangle GetBounds(int index)
		{
			return (Rectangle)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Bitmap GetBitmap(int nameindex)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Color[] GetColors(int index)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GetLProtocolSyncRGB(Color[] color, double value)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GetLProtocolSyncRGB(Color[] colors, double dimmer, double value = 1.0)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetBECTopBottom(bool TopBottom)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetBECLeftRight(bool LeftRight)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetIfon(int type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Bitmap ThumbnailImage(Bitmap img, int maxHeight, int maxWidth)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void addQueue(BufferData bufferData)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public int GetQueueCount()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BufferData GetSendBuffer()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void screenImageThreadHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static ScreenCapture()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			lockObject = new object();
			screenCapture = null;
			keyValuePairs = new Dictionary<int, myData>();
			Thumbnail = new Dictionary<int, Bitmap>();
			sw = new Stopwatch();
			dxgifps = 0;
			GXGIMonitorTime = 5;
		}
	}
}
namespace beelight.Driver.Net
{
	internal class NetInfo
	{
		private static NetInfo netinfo;

		private static object _lock;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public NetInfo()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static NetInfo GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<string> GetIpInfo()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static NetInfo()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			netinfo = null;
			_lock = new object();
		}
	}
	internal class UdpLocation
	{
		public static List<IPAddress> IPAddresses;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UdpLocation()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<IPAddress> GetBroadcastList()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private IPAddress GetBroadcast(IPAddress ipAddress, IPAddress subnetMask)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private IPAddress GetSubnetMask(IPAddress ipAdd)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static UdpLocation()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			IPAddresses = new List<IPAddress>();
		}
	}
	internal class UDPServer
	{
		public static UDPServer udpServer;

		public static object _lock;

		public static readonly int PORT;

		public static readonly int RECIEVE_PORT;

		public static List<UdpClient> udpClients;

		public static IPEndPoint iPEndPoint;

		private Storage storage;

		private List<IPEndPoint> IPEndPoints;

		private static Thread wirlessThread;

		private static Dictionary<IPEndPoint, Protocol> dicts;

		private static Dictionary<Protocol, int> deviceOnline;

		private static List<IPEndPoint> listIPEndpoint;

		private static List<Protocol> devices;

		private static List<int> deviceHeartbeat;

		private static object mlock;

		private static UIEventController uIEvent;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private UDPServer(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InitUDPClient()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static UDPServer GetInstance(IniPublicClass iniPublicClass)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendByte(byte[] buffer)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendNetScen()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendPackage(byte[] buffer)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RecieveThread(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void FlashConnect()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static UDPServer()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			udpServer = null;
			_lock = new object();
			PORT = 9876;
			RECIEVE_PORT = 9875;
			udpClients = new List<UdpClient>();
			iPEndPoint = null;
			dicts = new Dictionary<IPEndPoint, Protocol>();
			deviceOnline = new Dictionary<Protocol, int>();
			listIPEndpoint = new List<IPEndPoint>();
			devices = new List<Protocol>();
			deviceHeartbeat = new List<int>();
			mlock = new object();
			uIEvent = null;
		}
	}
	public class Response
	{
		public string language;

		public string update;

		public string setup;

		public string ctime;

		public int major;

		public int minor;

		public int revision;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Response()
		{
		}

		static Response()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class ResponsePackage
	{
		public class Data
		{
			public int nums;

			public Response data;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public Data()
			{
			}

			static Data()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		public int code;

		public Data data;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ResponsePackage()
		{
		}

		static ResponsePackage()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class UpdateHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string GetOS()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Response TryGetPackageByHttps(string URL_BASE, string uuid, string major, string minor, string revision, string language, int debug)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Response TryGetPackage(string URL_BASE, string uuid, string major, string minor, string revision, string language, int debug)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string genSecret(string key)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string getUnixTime()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public UpdateHelper()
		{
		}

		static UpdateHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Driver.Keyboard
{
	public delegate void KeyHookListener(bool down, KeyEventArgs e);
	internal class Hook
	{
		public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

		public struct KeyMSG
		{
			public int vkCode;

			public int scanCode;

			public int flags;

			public int time;

			public int dwExtraInfo;
		}

		private IntPtr m_pKeyboardHook;

		private HookProc m_KeyboardHookProcedure;

		public bool Porwer;

		public static int pp;

		public static bool isSet;

		public static bool isHotkey;

		public static bool isInstall;

		public KeyHookListener KeyListener
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			set
			{
			}
		}

		public event KeyEventHandler KeyDown
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			add
			{
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			remove
			{
			}
		}

		public event KeyEventHandler KeyUp
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			add
			{
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			remove
			{
			}
		}

		public event KeyPressEventHandler KeyPress
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			add
			{
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			[CompilerGenerated]
			remove
			{
			}
		}

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr pInstance, int threadId);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool UnhookWindowsHookEx(IntPtr pHookHandle);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(IntPtr pHookHandle, int nCode, int wParam, IntPtr lParam);

		[MethodImpl(MethodImplOptions.NoInlining)]
		private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Start()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Stop()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Hook()
		{
		}

		static Hook()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace beelight.Driver.Audio
{
	internal class AudioCapture
	{
		private static object lockObject;

		private static AudioCapture audioCapture;

		private MMDevice mMDevice;

		public WasapiLoopbackCapture mWavIn;

		private int wavresult;

		private double soundSystem;

		private int dltsSampling;

		private double[] wavResults;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private AudioCapture()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static AudioCapture GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void StartRecord()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MWavIn_DataAvailable(object sender, WaveInEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MonitorMMDevice()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public int GetWavResult()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public int GetwavResult()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public double GetsoundSystem()
		{
			return 0.0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static AudioCapture()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			lockObject = new object();
			audioCapture = null;
		}
	}
}
namespace beelight.DataView
{
	internal class AppData
	{
		public class AppConfig
		{
			public Dictionary<string, string> Config;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public AppConfig()
			{
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public void AddBind(string key, string value)
			{
			}

			static AppConfig()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private ScreenCapture screenCapture;

		private string filename;

		public Dictionary<string, AppConfig> Dicts;

		private static object _lock;

		private static AppData appData;

		public DateTime dateTime;

		private string language;

		private bool autoStart;

		private bool checkUpdate;

		private bool hardwareUpdate;

		private bool boolInterConnect;

		private bool ecoValue;

		private bool becTopBottomValue;

		private bool becLeftRightValue;

		private object Lock;

		private static object o;

		public string Language
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool AutoStart
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool CheckUpdate
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool HardwareUpdate
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool BoolInterConnect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool EcoValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool BECTopBottomValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool BECLeftRightValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void setSetFormParameter()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SetSetFormParameter()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private string GetWindowsLanguage()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private AppData()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static AppData GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void TryLoadFilePath()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void loadconfig()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void LoadConfig()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private string GetLabel(char[] str, int start, out int end)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UpdateConfig(string DeviceName, string ConfigString, string ConfigParam)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdataFile()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string GetConfig(string DeviceName, string ConfigString)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string Get(string key)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Set(string key, string value)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static AppData()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			_lock = new object();
			appData = null;
			o = new object();
		}
	}
	internal class CurrentProcess
	{
		public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

		private static Hashtable processWnd;

		[MethodImpl(MethodImplOptions.NoInlining)]
		static CurrentProcess()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			if (processWnd == null)
			{
				processWnd = new Hashtable();
			}
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("kernel32.dll")]
		public static extern void SetLastError(uint dwErrCode);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IntPtr GetCurrentWindowHandle(Process process)
		{
			return (IntPtr)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
		{
			return true;
		}

		[DllImport("kernel32.dll")]
		public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ClearMemory()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public CurrentProcess()
		{
		}
	}
	internal class DataFile
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Dictionary<string, string[]> GetStringsParameter(string fileurl, string[] config, string key)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DataFile()
		{
		}

		static DataFile()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class BroadcastMusic
	{
		public class DataConfig
		{
			public Dictionary<string, string> Config;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public DataConfig()
			{
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public void AddBind(string key, string value)
			{
			}

			static DataConfig()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private string file;

		public Dictionary<string, DataConfig> Dicts;

		private static AppData appData;

		public DateTime dateTime;

		private string use;

		private object Lock;

		private static object o;

		public string Use
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public BroadcastMusic(string fileurl)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void TryLoadFilePath()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void LoadConfig()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private string GetLabel(char[] str, int start, out int end)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UpdateConfig(string DeviceName, string ConfigString, string ConfigParam)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdataFile()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string GetConfig(string DeviceName, string ConfigString)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static BroadcastMusic()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			appData = null;
			o = new object();
		}
	}
	internal class IniFile
	{
		private string FilePath;

		[DllImport("kernel32", CharSet = CharSet.Auto)]
		private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

		[DllImport("kernel32", CharSet = CharSet.Auto)]
		private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IniFile()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IniFile(string filename)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public string Read(string section, string key, string def, string filePath = "")
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[HandleProcessCorruptedStateExceptions]
		public int Write(string section, string key, string value, string filePath = "")
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public int DeleteSection(string section, string filePath = "")
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public int DeleteKey(string section, string key, string filePath = "")
		{
			return 0;
		}

		static IniFile()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class MyStyle
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void SetWindowRegion(Form form, int radian)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void SetWindowRegion(Panel panel, int radian)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void SetWindowRegion(PictureBox pictureBox, int radian)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Draw(Rectangle rectangle, Graphics g, int _radius, bool cusp, Color begin_color, Color end_color)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static GraphicsPath DrawRoundRect(int x, int y, int width, int height, int radius)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void lableComposing(Label label, string str, string language, int ruownum)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public MyStyle()
		{
		}

		static MyStyle()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class NewPanel : Panel
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public NewPanel()
		{
		}

		static NewPanel()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class NewTextBox : TextBox
	{
		protected override CreateParams CreateParams
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public NewTextBox()
		{
		}

		static NewTextBox()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class PublicHandle
	{
		private static readonly ReaderWriterLockSlim LogWriteLock;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ProcessStarct(string filename)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ProcessStop(string filename)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void log(string name, string error)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public PublicHandle()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static PublicHandle()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			LogWriteLock = new ReaderWriterLockSlim();
		}
	}
	public enum movieEnum
	{
		movie_low,
		movie_mid,
		movie_high
	}
	public enum musicEnum
	{
		music_low,
		music_mid,
		music_high
	}
	public enum moodEnum
	{
		rainbow,
		flame,
		read,
		fireworks,
		star,
		drip,
		particle,
		flow,
		ball,
		swing,
		breathe,
		pure
	}
	internal class Statistics
	{
		private long servicetime;

		private string typeoffunction;

		private string function;

		private Thread statistisctherad;

		private AutoResetEvent myResetEvent;

		private bool statistisctheradswitch;

		private string file;

		private Storage storage;

		private IniFile inifile;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Init()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Statistics(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void StatistiscTherad()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void suspend(string name, int select)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void statistics()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void close()
		{
		}

		static Statistics()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	public class TaskBarUtil
	{
		private struct Rect
		{
			public int left;

			public int top;

			public int right;

			public int bottom;
		}

		private static readonly CultureInfo _CultureInfo;

		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string windowTitle);

		[DllImport("user32.dll")]
		private static extern bool GetClientRect(IntPtr handle, out Rect rect);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr handle, uint message, int wParam, int lParam);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Task RefreshAsync()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Refresh()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Refresh(IntPtr windowHandle)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public TaskBarUtil()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static TaskBarUtil()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			_CultureInfo = CultureInfo.InstalledUICulture;
		}
	}
	internal class Storage
	{
		public bool deviceUpdateEvent;

		public IDevice device;

		public bool removetype;

		public string pid;

		public string appVersion;

		public string appVersionTime;

		public IDriver.DRIVER_TYPE driverType;

		public bool first;

		public int panelContentType;

		public int uselect_record;

		public string name_record;

		private string CONFIG;

		private static object _lock_;

		private int scen;

		private AppData appData;

		private object scenLock;

		private int modeselect;

		private string language;

		private bool ecoValue;

		private bool becTopBottomValue;

		private bool becLeftRightValue;

		private int fps;

		private bool checkUpdate;

		private bool hardwareUpdate;

		private int wirelessNums;

		private object mlock_wireless;

		private object mlock_wireless_list;

		private object mlock_wireless_select;

		private int wirlessSelect;

		private List<Protocol> protocols;

		public int LockSettings;

		public int SubScenMovice;

		public int SubScenMusic;

		public int SubScenKeyboard;

		public int SubScenLED;

		public int ColorTemperature;

		private bool autostart;

		private bool connectStatus;

		private string connectVersions;

		private int dimmer_percent;

		public byte[] ScreenDatas;

		public bool EnableScreenScen;

		public bool PowerStatus;

		public byte Red;

		public byte Green;

		public byte Blue;

		public SerialPort Serial;

		public string SerialName;

		public string DeviceID;

		public bool MusicType;

		public string UUID;

		private Dictionary<string, Protocol> dicts;

		private Protocol currentProtocol;

		private int externPort1;

		private int externPort2;

		private bool boolInterConnect;

		private bool controlsColorEnhance;

		public object lock_Dicts;

		public List<Size> ScreenSizeLists;

		public List<string> ScreenDevices;

		private int screenSelect;

		public int nameScreenSelect;

		public bool LightStrip;

		private bool setupLeft;

		private int sidePattern;

		public int value;

		private UIEventController uiEvent;

		private RazerControl razerControl;

		public int modeSelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public string Language
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool EcoValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool BECTopBottomValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool BECLeftRightValue
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int FPS
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool CheckUpdate
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool HardwareUpdate
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int WirelessNums
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int WirelessSelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public List<Protocol> Protocols
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool AutoStart
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool ConnectStatus
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public string ConnectVersions
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int Dimmer_Percent
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public Protocol CurrentProtocol
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int ExternPort1
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int ExternPort2
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public Dictionary<string, Protocol> Dicts
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return null;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool BoolInterConnect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool ControlsColorEnhance
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int ScreenSelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public bool SetupLeft
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return true;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		public int SidePattern
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<string> GetSerialsDevices()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetSerialDevices(string deviceName)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private Storage(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void getAppData()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Storage GetInstance(IniPublicClass iniPublicClass)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void StoreConfig(string key, string value)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void StoreConfig(string config, string key, string value)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string LoadConfig(string key)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string LoadConfig(string config, string key)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private string GetWindowsLanguage()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void InitRazerControl()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void MonitorRazerControl(bool type)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UnInitRazerControl()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Storage()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			_lock_ = new object();
		}
	}
}
namespace beelight.Control.UpdateHandle
{
	internal class UpdateController
	{
		private static object lock_obj;

		private Thread thread;

		private Storage storage;

		private bool close_form;

		private int ParentHandle;

		private Form parentForm;

		private IniPublicClass IniPublicClass;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private UpdateController(IniPublicClass iniPublicClass)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetParentHandle(int handle)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetParentForm(Form form)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Start()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static UpdateController GetInstance(IniPublicClass iniPublicClass)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ThreadHandle()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void FormClosedEventHandler(object sender, FormClosedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void UpdateFormClose()
		{
		}

		[DllImport("user32")]
		public static extern int SetParent(int children, int parent);

		[MethodImpl(MethodImplOptions.NoInlining)]
		static UpdateController()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			lock_obj = new object();
		}
	}
}
namespace beelight.Control.EventHandle
{
	internal class EventController
	{
		public delegate void LanguageChange();

		[MethodImpl(MethodImplOptions.NoInlining)]
		public EventController()
		{
		}

		static EventController()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class EventDispatch
	{
		private static object __lock;

		private static EventDispatch languageDispatch;

		public EventController handle;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private EventDispatch()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static EventDispatch GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static EventDispatch()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			__lock = new object();
			languageDispatch = null;
		}
	}
}
namespace LightProtocol.Vendor.LPVendor
{
	internal class ILProtocol
	{
		public delegate void LP_HeartBeatReq(object o);

		public delegate void LP_FrimAck(object o, Version bspVersion, Version appVersion, string pid, string devideId, DateTime dateTime, string manufactureInfo, string marketInfo);

		public delegate void LP_SyncConfigAck(object o, int sumPixel, int channels, int[] channelPixel);

		public delegate void LP_OtaStartAck(object o, LProtocolBase.LP_OTA_ACK status, Version bspVersion, Version appVersion, string pidInfo, string deviceId);

		public delegate void LP_OtaTransferAck(object o, long offset, int dataLen);

		public delegate void LP_SwitcherAck(object o, bool enable, byte channelMark);

		public delegate void LP_BrightAck(object o, int dimmer, byte channelMark);

		public delegate void LP_TemperAck(object o, int whiteDimmer, int temper, byte channelMark);

		public delegate void LP_ColorAck(object o, Color color, byte channelMark);

		public delegate void LP_WorkModeAck(object o, LProtocolBase.LP_WK_MODE mode, byte channelMark);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILProtocol()
		{
		}

		static ILProtocol()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocol
	{
		protected class Buffer
		{
			public byte[] Data;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public Buffer(byte[] data)
			{
			}

			static Buffer()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		private int queueMaxLength;

		private Queue<Buffer> queue;

		private object queueLock;

		private int protocol_status;

		private int protocol_len;

		private int protocol_recieve;

		private byte[] protocol_data;

		public int protocol_recieve_count;

		private LProtocolHeartbeat lProtocolHeartbeat;

		private LProtocolFirm lProtocolFirm;

		private LProtocolSyncStatus lProtocolSyncStatus;

		private LProtocolSyncConfig lProtocolSyncConfig;

		private LProtocolOTA lProtocolOTA;

		private LProtocolCtrl lProtocolCtrl;

		private LProtocolLog lProtocolLog;

		public bool otaLock;

		public int ProtocolId;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocol(int queueMaxLength = 20)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ProtocolReciever(byte[] data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ProtocolDecode(byte[] data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void ProtocolParser(byte[] data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void otaStartAckListener(object o, LProtocolBase.LP_OTA_ACK status, Version bspVersion, Version appVersion, string pidInfo, string deviceId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void otaTransferAckListener(object o, long offset, int dataLen)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void addQueue(byte[] data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public int GetQueueCount()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GetSendBuffer()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendFirmReq()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendSyncStatusReq()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendSyncConfigReq()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendOTAStartReq(Version appVersion, long fileSize)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendOTATransferReq(long offset, byte[] data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendCtrlSwitchReq(bool enable, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendCtrlBrightReq(int bright, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendCtrlTemperReq(int whiteBright, int temperValue, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendCtrlColorReq(Color color, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendCtrlRGBTransferReq(Color[] colors, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SendCtrlRGBTransferReq(LProtocolBase.RGB[] rgbs, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SenCtrlWorkModeReq(LProtocolBase.LP_WK_MODE mode, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SenCtrlWorkModeReq(LProtocolBase.LP_WK_MODE mode, byte innerEnable, byte innerMode, byte channel = byte.MaxValue)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddHeartBeatReqListener(ILProtocol.LP_HeartBeatReq fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveHeartBeatListener(ILProtocol.LP_HeartBeatReq fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddFirmAckListener(ILProtocol.LP_FrimAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveFirmAckListener(ILProtocol.LP_FrimAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddSyncConfigAckListener(ILProtocol.LP_SyncConfigAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveSyncConfigAckListener(ILProtocol.LP_SyncConfigAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddOtaStartAckListener(ILProtocol.LP_OtaStartAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveOtaStartAckListener(ILProtocol.LP_OtaStartAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddOtaTransferAckListener(ILProtocol.LP_OtaTransferAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveOtaTransferAckListener(ILProtocol.LP_OtaTransferAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddSyncStatusSwitcherAckListener(ILProtocol.LP_SwitcherAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveSyncStatusSwitcherAckListener(ILProtocol.LP_SwitcherAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddSyncStatusBrightAckListener(ILProtocol.LP_BrightAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveSyncStatusBrightAckListener(ILProtocol.LP_BrightAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddSyncStatusTemperAckListener(ILProtocol.LP_TemperAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveSyncStatusTemperAckListener(ILProtocol.LP_TemperAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddSyncStatusColorAckListener(ILProtocol.LP_ColorAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveSyncStatusColorAckListener(ILProtocol.LP_ColorAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddSyncStatusWorkModeAckListener(ILProtocol.LP_WorkModeAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveSyncStatusWorkModeAckListener(ILProtocol.LP_WorkModeAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddCtrlSwitcherAckListener(ILProtocol.LP_SwitcherAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveCtrlSwitcherAckListener(ILProtocol.LP_SwitcherAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddCtrlBrightAckListener(ILProtocol.LP_BrightAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveCtrlBrightAckListener(ILProtocol.LP_BrightAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddCtrlTemperAckListener(ILProtocol.LP_TemperAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveCtrlTemperAckListener(ILProtocol.LP_TemperAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddCtrlColorAckListener(ILProtocol.LP_ColorAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveCtrlColorAckListener(ILProtocol.LP_ColorAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddCtrlWorkModeAckListener(ILProtocol.LP_WorkModeAck fn)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveCtrlWorkModeAckListener(ILProtocol.LP_WorkModeAck fn)
		{
		}

		static LProtocol()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolBase
	{
		public class RGB
		{
			public byte R;

			public byte G;

			public byte B;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public RGB(byte R, byte G, byte B)
			{
			}

			static RGB()
			{
				DyyVDbaRvM1YfIq9il.vEB6drODu();
			}
		}

		public enum LP_CMD
		{
			LP_CMD_HEARTBEAT = 0,
			LP_CMD_FIRM = 1,
			LP_CMD_SYNCSTATUS = 2,
			LP_CMD_SYNCCONFIG = 3,
			LP_CMD_OTA = 4,
			LP_CMD_CTRL_DEVICE = 5,
			LP_CMD_CTRL_SYNC_RGB = 6,
			LP_CMD_CTRL_LOG = 26
		}

		public enum LP_ATTR
		{
			LP_ATTR_REQ,
			LP_ATTR_ACK
		}

		public enum LP_WK_MODE
		{
			LP_WK_MODE_PC,
			LP_WK_MODE_TUYA,
			LP_WK_MODE_NRF,
			LP_WK_MODE_KEY
		}

		public enum LP_OTA_CTRL
		{
			LP_OTA_CTRL_START = 1,
			LP_OTA_CTRL_TRANSFER
		}

		public enum LP_OTA_ACK
		{
			LP_OTA_ACK_SUCCESS,
			LP_OTA_ACK_OVERSIZE,
			LP_OTA_ACK_VERSION_LAST,
			LP_OTA_ACK_VERSION_ROLLBACK
		}

		public enum LP_CTRL
		{
			LP_CTRL_SWITCHER = 1,
			LP_CTRL_BRIGHT,
			LP_CTRL_TEMPER,
			LP_CTRL_COLOR,
			LP_CTRL_RGB_TRANSFER,
			LP_CTRL_WORKMODE
		}

		public static readonly byte Head0;

		public static readonly byte Head1;

		public static readonly byte Head2;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenFramePackage(LP_ATTR attr, LP_CMD cmd, byte[] data)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetStringInfo(char[] data)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static int GetHash(byte[] data)
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetHexCode(int value)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetHexCode(byte[] data)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static int GetInt(byte[] data, int offset)
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static long GetLong(byte[] data, int offset)
		{
			return 0L;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolBase()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static LProtocolBase()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			Head0 = 85;
			Head1 = 170;
			Head2 = 90;
		}
	}
	internal class LProtocolCtrl
	{
		public ILProtocol.LP_SwitcherAck FnSwitcherAck;

		public ILProtocol.LP_BrightAck FnBrightAck;

		public ILProtocol.LP_TemperAck FnTemperAck;

		public ILProtocol.LP_ColorAck FnColorAck;

		public ILProtocol.LP_WorkModeAck FnWorkModeAck;

		private object protocolObject;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolCtrl(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenSwitchPackage(bool enable, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenBrightPackage(int dimmer, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenTemperPackage(int whiteValue, int temperValue, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenColorPackage(Color color, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenRGBTransferPackage(Color[] color, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenRGBTransferPackage(LProtocolBase.RGB[] color, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenWorkModePackage(LProtocolBase.LP_WK_MODE workMode, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenWorkModePackage(LProtocolBase.LP_WK_MODE workMode, byte innerEnable, byte innerMode, byte channelMark = byte.MaxValue)
		{
			return null;
		}

		static LProtocolCtrl()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolFirm
	{
		public ILProtocol.LP_FrimAck FnFirmAck;

		private object protocolObject;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolFirm(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenFirmPackage()
		{
			return null;
		}

		static LProtocolFirm()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolHeartbeat
	{
		public ILProtocol.LP_HeartBeatReq FnHeartbeatReq;

		private object protocolObject;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolHeartbeat(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GenHeartBeatPackage()
		{
			return null;
		}

		static LProtocolHeartbeat()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolLog
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolLog()
		{
		}

		static LProtocolLog()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolOTA
	{
		public ILProtocol.LP_OtaStartAck FnOtaStartAck;

		public ILProtocol.LP_OtaTransferAck FnOtaTransferAck;

		private object protocolObject;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolOTA(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenOtaStartPackage(Version appVersion, long fileSize)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenOtaTransferPackage(long offset, byte[] data)
		{
			return null;
		}

		static LProtocolOTA()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolSyncConfig
	{
		private object protocolObject;

		public ILProtocol.LP_SyncConfigAck FnSyncConfigAck;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolSyncConfig(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenSyncConfigPackage()
		{
			return null;
		}

		static LProtocolSyncConfig()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolSyncRGB
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public byte[] GenProtocolSyncRGB(byte rows, byte columns, Color[] colors)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolSyncRGB()
		{
		}

		static LProtocolSyncRGB()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LProtocolSyncStatus
	{
		public ILProtocol.LP_SwitcherAck FnSwitcherAck;

		public ILProtocol.LP_BrightAck FnBrightAck;

		public ILProtocol.LP_TemperAck FnTemperAck;

		public ILProtocol.LP_ColorAck FnColorAck;

		public ILProtocol.LP_WorkModeAck FnWorkModeAck;

		private object protocolObject;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LProtocolSyncStatus(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool Parser(LProtocolBase.LP_ATTR ack, byte[] data)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static byte[] GenSyncStatusPackage()
		{
			return null;
		}

		static LProtocolSyncStatus()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.Vendor.LFramework.LRender
{
	internal class ILRender
	{
		private Color[] filterColor;

		private int filter;

		private Color[] colorBuf;

		private int startIndex;

		private int endIndex;

		private int dlt;

		private int len;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public ILRender(ref Color[] colorBuf, int startIndex, int endIndex)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public int GetLen()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Color GetColor(int index)
		{
			return (Color)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetColor(int index, Color color)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetColor(int index, Color color, bool controlscolorenhance)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RunRender()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Color[] CloneColorBuf()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetFilter(int filter)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Clear(Color color)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Color ColorStrengthen(Color color)
		{
			return (Color)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Color ColorHsv2RGB(int h, int s, int v)
		{
			return (Color)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public Color GetColorTest()
		{
			return (Color)(object)null;
		}

		static ILRender()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LRenderHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Clear(ILRender render, Color color)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Forward(ILRender render, Color color, int step = 1)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Color[] Resize(Color[] colors, int size, bool resever)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Backward(ILRender render, Color color, int step = 1)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Backward(ILRender render, Color color, int fold, bool type, int step = 1)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Color GetColor(int index)
		{
			return (Color)(object)null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static List<Color> GetSingleColorList(Color srcColor, Color desColor, int count)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LRenderHelper()
		{
		}

		static LRenderHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LRenderSet
	{
		public ILRender LeftRender;

		public ILRender RightRender;

		public ILRender TopRender;

		public ILRender BottomRender;

		public ILRender AllRender;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LRenderSet()
		{
		}

		static LRenderSet()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.Vendor.LFramework.LMode
{
	internal abstract class ILMode
	{
		public delegate void RenderHook(Color[] colors, byte channel);

		public delegate void RenderHookSendCtrlTemperReq(int whiteBright, int temperValue, byte channel);

		public delegate void RenderHookSendCtrlColorReq(Color color, byte channel);

		protected RenderHook fnRenderHook;

		protected RenderHookSendCtrlTemperReq fnRenderHookSendCtrlTemperReq;

		protected RenderHookSendCtrlColorReq fnRenderHookSendCtrlColorReq;

		public int filter;

		public bool ControlsColorEnhance;

		public int ModeId;

		public int Son;

		public bool MusicType;

		public int ScreenId;

		public int NameScreenId;

		public bool LeftSetup;

		public int SidePattern;

		public string Pid;

		public string ModeName;

		public Color ForeColor;

		public Color BackgroundColor;

		public int whiteBright;

		public int temperValue;

		protected LRenderSet render;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetRenderHook(RenderHook hook)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetRenderHookSendCtrlTemperReq(RenderHookSendCtrlTemperReq hook)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetRenderHookSendCtrlColorReq(RenderHookSendCtrlColorReq hook)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetRender(LRenderSet render)
		{
		}

		public abstract void Init();

		public abstract void Run();

		public abstract void Quit();

		[MethodImpl(MethodImplOptions.NoInlining)]
		public virtual void ModeSleep()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected ILMode()
		{
		}

		static ILMode()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
	internal class LampLModeHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static List<ILMode> ILModeFactory(LRenderSet renderSet)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LampLModeHelper()
		{
		}

		static LampLModeHelper()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.Vendor.LBitmap
{
	internal class LBitmap
	{
		public readonly int SampleSize;

		public Color[] Top;

		public Color[] Left;

		public Color[] Bottom;

		public Color[] Right;

		private int[] tops;

		private int[] lefts;

		private int[] bottoms;

		private int[] rights;

		private int topRow;

		private int leftColumn;

		private int bottomRow;

		private int rightColumn;

		private int width;

		private int height;

		private int stride;

		private bool topBottomCheck;

		private bool leftRightCheck;

		private int widthStepBytes;

		private int widthStep;

		private int heightStep;

		private int heightStepBytes;

		private int checkNums;

		private int checkPixelCount;

		private readonly int darkValue;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public LBitmap()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Run(ref BitmapData bitmapdata, bool topBottomCheck, bool leftRightCheck)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void clearBuffer()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void topBottomLine(byte* ptr)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void leftRightLine(byte* ptr)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe bool isDarkRow(byte* ptr)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe bool isDarkColumn(byte* ptr)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void calcuateRow(byte* ptr, ref int[] buffer)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void calcuateColumn(byte* ptr, ref int[] buffer)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void outColor()
		{
		}

		static LBitmap()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.LightDevice
{
	internal class DeviceManager
	{
		public delegate void AddNewDeviceEvent(IDevice device);

		public delegate void RemoveDeviceEvent(IDevice device);

		public AddNewDeviceEvent FnAddNewDeviceEvent;

		public RemoveDeviceEvent FnRemoveDeviceEvent;

		private List<IDevice> devices;

		private Dictionary<string, IDevice> dictDeviceId;

		private static DeviceManager deviceManager;

		private static object lockObject;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private DeviceManager()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static DeviceManager GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool AddDevice(IDevice device)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveDevice(int driverId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RemoveDevice(string deviceId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string[] GetDevicesID()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public string[] GetDevicesPID()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IDevice GetDevice(string deviceId)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IDevice GetDevice(int driverId)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<IDevice> GetAllDevice()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static DeviceManager()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			deviceManager = null;
			lockObject = new object();
		}
	}
}
namespace LightProtocol.LightDevice.Device
{
	internal abstract class IDevice
	{
		public delegate void DeviceUpdateEvent(string deviceid);

		public delegate void DeviceOTAFinishEvent(bool status);

		public delegate void DeviceOTAPercentEvent(int percent);

		public DeviceUpdateEvent FnDeviceUpdateEvent;

		public DeviceOTAFinishEvent FnDeviceOTAFinishEvent;

		public DeviceOTAPercentEvent FnDeviceOTAPercentEvent;

		public Version AppVersion;

		public Version BspVersion;

		public string PID;

		public string DeviceId;

		public DateTime PublishDateTime;

		public string ManufactureInfo;

		public string MarketInfo;

		public bool PowerEnable;

		public int Dimmer;

		public Color CurrentColor;

		public int TemperValue;

		public int WhiteValue;

		public LProtocolBase.LP_WK_MODE WorkMode;

		public bool sleep;

		public readonly IDriver Driver;

		public readonly LProtocol Protocol;

		public bool isRunsyncConfigAckListener;

		private object modeLock;

		private int modeSelect;

		protected LRenderSet renderSet;

		public List<ILMode> modes;

		public Color[] allColor;

		private string otaFileName;

		protected object otaLock;

		protected bool otaFlag;

		private LProtocolBase.LP_OTA_ACK otaStartAck;

		private Version otaAppVersion;

		private object taskLock;

		private bool isRunMode;

		private Semaphore otaSemphore;

		private long otaFileSize;

		public int ModeSelect
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return 0;
			}
			[MethodImpl(MethodImplOptions.NoInlining)]
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IDevice(IDriver driver)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void OTAProcess(object o, Version appVersion, string fileName)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void otaProcessHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void otaStartAckListener(object o, LProtocolBase.LP_OTA_ACK status, Version bspVersion, Version appVersion, string pidInfo, string deviceId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void otaTransferAckListener(object o, long offset, int dataLen)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void firmAckListener(object o, Version bspVersion, Version appVersion, string pid, string devideId, DateTime dateTime, string manufactureInfo, string marketInfo)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void baserSyncConfigAckListener(object o, int sumPixel, int channels, int[] channelPixel)
		{
		}

		public abstract void DefaultLMode();

		protected abstract void SyncRender();

		protected abstract void syncConfigAckListener(object o, int sumPixel, int channels, int[] channelPixel);

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected void switchAckListener(object o, bool enable, byte channel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected void brightAckListener(object o, int dimmer, byte channel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected void temperAckListener(object o, int whiteValue, int temperValue, byte channel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected void colorAckListener(object o, Color color, byte channel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected void workmodeAckListener(object o, LProtocolBase.LP_WK_MODE mode, byte channel)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void RecieveEventHandler(IDriver driver)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void writeThreadHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void sendSyncStatusReq()
		{
		}

		static IDevice()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.Hardware
{
	internal class HDriverManage
	{
		private static HDriverManage hDriverManage;

		private static object objectLock;

		private object lockDictionary;

		private Dictionary<string, IDriver> driverDictionary;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private HDriverManage()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static HDriverManage GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool RegisterDriver(string driverName, IDriver driver)
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UninstallDriver(int driverId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void UninstallDriver(string driverName)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<IDriver> GetSerialDriver()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public List<IDriver> GetNetworkDriver()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IDriver GetDriver(int driverId)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IDriver GetDriver(string name)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static HDriverManage()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			hDriverManage = null;
			objectLock = new object();
		}
	}
	internal class HDriverServer
	{
		public delegate void DriverDiscovery(IDriver device, string driverName);

		public DriverDiscovery FnDriverDiscovery;

		private Task netDiscoveryTask;

		private static object objectLock;

		private static HDriverServer hDriverServer;

		private object serialLock;

		private List<string> listActiveSerialPort;

		private int driverIndex;

		private DriverNet broadcastDriver;

		private Random random;

		private object driverLock;

		private Socket socket;

		private int count;

		private int broadcastCount;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private HDriverServer()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static HDriverServer GetInstance()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void serialDiscoveryHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private SerialPort GetSerialPort()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void serialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void netDiscoveryHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void netMonitor()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public bool getRestartSocket()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void restartSocket()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static HDriverServer()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			objectLock = new object();
			hDriverServer = null;
		}
	}
}
namespace LightProtocol.Hardware.HDriver
{
	internal abstract class IDriver
	{
		public delegate void DeviceDisconnectEvent(int driverId);

		public enum DRIVER_TYPE
		{
			DRIVER_TYPE_SERIALPORT = 1,
			DRIVER_TYPE_NETWORD
		}

		public delegate void RecieveListener(IDriver driver);

		public DeviceDisconnectEvent FnDriverDisconnectEventHandler;

		public RecieveListener FnRecieveListener;

		public readonly int DriverID;

		public readonly DRIVER_TYPE DriverType;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public IDriver(DRIVER_TYPE type, int driverId = 0)
		{
		}

		public abstract void Open();

		public abstract bool IsOpen();

		public abstract void Close();

		public abstract void Write(byte data);

		public abstract void Write(byte[] data, int offset, int count);

		public abstract bool WriteFinish();

		public abstract byte[] ReadBytes();

		public abstract byte[] ReadBytes(int nums);

		public abstract int ReadByteCount();

		static IDriver()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.Hardware.HDriver.DeviceSerial
{
	internal class DriverSerial : IDriver
	{
		private SerialPort serialPort;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DriverSerial(SerialPort serialPort, int deviceId = 0)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void serialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Close()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override bool IsOpen()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override int ReadByteCount()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override byte[] ReadBytes()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override byte[] ReadBytes(int nums)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Write(byte data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Write(byte[] data, int offset, int count)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override bool WriteFinish()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Open()
		{
		}

		static DriverSerial()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.Hardware.HDriver.DeviceNet
{
	internal class DriverNet : IDriver
	{
		private object queueLock;

		private Queue<byte[]> sendData;

		private bool isAlive;

		private Socket socket;

		private int bytesToRead;

		private byte[] bytesRecieve;

		private EndPoint remote;

		private Thread threadRecieve;

		private Semaphore semaphoreRead;

		private int countTime;

		private bool type;

		private bool sendType;

		private object sendDataLock;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DriverNet(Socket socket, EndPoint remote, int deviceId = 0)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void AddSendData(byte[] data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Send()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void receiveData(byte[] data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void driverTimeOut()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void recieveHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override bool IsOpen()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override int ReadByteCount()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override byte[] ReadBytes()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override byte[] ReadBytes(int nums)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Write(byte data)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Write(byte[] data, int offset, int count)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override bool WriteFinish()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Close()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Open()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void ClearAway()
		{
		}

		static DriverNet()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace LightProtocol.Application
{
	internal class AppDriver
	{
		private HDriverServer hDriverServer;

		private HDriverManage hDriverManage;

		private DeviceManager deviceManager;

		private object objectLock;

		private Dictionary<int, LProtocol> dictProtocol;

		private Dictionary<int, IDriver> dictIDriver;

		private Dictionary<int, int> dictTick;

		[MethodImpl(MethodImplOptions.NoInlining)]
		public AppDriver()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void driverDiscovery(IDriver driver, string deviceName)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void driverDisconnectEventHandler(int driverId)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void recieveListener(IDriver driver)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void heartbeatReqListener(object o)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void firmAckListener(object o, Version bspVersion, Version appVersion, string pid, string devideId, DateTime dateTime, string manufactureInfo, string marketInfo)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void writeThreadHandler()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void tickThreadHandler()
		{
		}

		static AppDriver()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
internal class <Module>{DBF1C536-9497-4F66-BAAA-06D7A4453A09}
{
	static <Module>{DBF1C536-9497-4F66-BAAA-06D7A4453A09}()
	{
		DyyVDbaRvM1YfIq9il.vEB6drODu();
	}
}
namespace SOj3wtG2Ob7xEudvw7
{
	internal class CDCWSn7SaPjUwoq2Cc
	{
		internal delegate void SFU4mbT3GMret7THonf(object o);

		internal static Module TWp4PNnQc;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void C9k\u000d\u000a\u00966\u009dxx\u0098q\u008f\u009a7(int typemdt)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public CDCWSn7SaPjUwoq2Cc()
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static CDCWSn7SaPjUwoq2Cc()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
			xrUtBVoaXtCT6B0w6a.S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca();
			TWp4PNnQc = Type.GetTypeFromHandle(KKr6hZkjvwWjdm9A4Z.Q\u0090r\u000d\u000a\u00966\u009dy\u0086\u009aof\u0092\u0098(33554743)).Assembly.ManifestModule;
		}
	}
}
namespace vJiGl01UUJfXfNWas3
{
	internal class DyyVDbaRvM1YfIq9il
	{
		internal class AXBrnIFfMAfABnJrF9 : Attribute
		{
			internal class z0oyxsqySXMDuI4ZyY<T>
			{
				[MethodImpl(MethodImplOptions.NoInlining)]
				public z0oyxsqySXMDuI4ZyY()
				{
				}

				static z0oyxsqySXMDuI4ZyY()
				{
					vEB6drODu();
				}
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			[AXBrnIFfMAfABnJrF9(typeof(z0oyxsqySXMDuI4ZyY<object>[]))]
			public AXBrnIFfMAfABnJrF9(object  )
			{
			}

			static AXBrnIFfMAfABnJrF9()
			{
				vEB6drODu();
			}
		}

		internal class ay67rn8SHAWRagidNL
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			[AXBrnIFfMAfABnJrF9(typeof(AXBrnIFfMAfABnJrF9.z0oyxsqySXMDuI4ZyY<object>[]))]
			internal static string D4r4O0AxSI(string  , string  )
			{
				return null;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public ay67rn8SHAWRagidNL()
			{
			}

			static ay67rn8SHAWRagidNL()
			{
				vEB6drODu();
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint rL2N9N6wh7IWY3IC3G(IntPtr classthis, IntPtr comp, IntPtr info, [MarshalAs(UnmanagedType.U4)] uint flags, IntPtr nativeEntry, ref uint nativeSizeOfCode);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr LhmiV9AUoOr1v5yhIs();

		internal struct Lk7BwHKFmNJY32ZC3n
		{
			internal bool bV44XU8KQo;

			internal byte[] Uu349Vtr47;
		}

		[Flags]
		private enum WDRJe2H6E4HVV6PGZs
		{

		}

		internal static Hashtable IBe4hEip2A;

		private static Assembly j8hgmZJ7n;

		private static byte[] lodECQQVs;

		private static int[] sMgC0o5PW;

		private static int Qwp4ejR7FG;

		private static SortedList d1uknJpcW;

		private static byte[] hIsn23p8h;

		private static bool TWn4MujlZv;

		internal static rL2N9N6wh7IWY3IC3G x3c4o2PyTx;

		private static IntPtr ghLACNa05;

		private static bool NFL4IGyoc7;

		private static int WS94a0Vnlv;

		private static int uS9zmJ6WC;

		private static IntPtr XtL4lyIIgx;

		private static bool PVVpfAGtG;

		private static uint[] M6EKmwjSJ;

		private static bool NrL10qsNW;

		private static bool hSjGubHK9;

		private static IntPtr c9FNce5cf;

		private static object diL3t0peo;

		private static byte[] VvPxdPh3O;

		internal static rL2N9N6wh7IWY3IC3G bFB44BUGlg;

		[AXBrnIFfMAfABnJrF9(typeof(AXBrnIFfMAfABnJrF9.z0oyxsqySXMDuI4ZyY<object>[]))]
		private static bool firstrundone;

		private static int S0FvrGWpN;

		private static bool cQCd71PIW;

		private static long phV4Uu6SUx;

		private static byte[] dKMLoMpMs;

		private static long i244bikuos;

		[MethodImpl(MethodImplOptions.NoInlining)]
		static DyyVDbaRvM1YfIq9il()
		{
			NrL10qsNW = false;
			j8hgmZJ7n = Type.GetTypeFromHandle(KKr6hZkjvwWjdm9A4Z.Q\u0090r\u000d\u000a\u00966\u009dy\u0086\u009aof\u0092\u0098(33554745)).Assembly;
			M6EKmwjSJ = new uint[64]
			{
				3614090360u, 3905402710u, 606105819u, 3250441966u, 4118548399u, 1200080426u, 2821735955u, 4249261313u, 1770035416u, 2336552879u,
				4294925233u, 2304563134u, 1804603682u, 4254626195u, 2792965006u, 1236535329u, 4129170786u, 3225465664u, 643717713u, 3921069994u,
				3593408605u, 38016083u, 3634488961u, 3889429448u, 568446438u, 3275163606u, 4107603335u, 1163531501u, 2850285829u, 4243563512u,
				1735328473u, 2368359562u, 4294588738u, 2272392833u, 1839030562u, 4259657740u, 2763975236u, 1272893353u, 4139469664u, 3200236656u,
				681279174u, 3936430074u, 3572445317u, 76029189u, 3654602809u, 3873151461u, 530742520u, 3299628645u, 4096336452u, 1126891415u,
				2878612391u, 4237533241u, 1700485571u, 2399980690u, 4293915773u, 2240044497u, 1873313359u, 4264355552u, 2734768916u, 1309151649u,
				4149444226u, 3174756917u, 718787259u, 3951481745u
			};
			PVVpfAGtG = false;
			cQCd71PIW = false;
			lodECQQVs = new byte[0];
			VvPxdPh3O = new byte[0];
			hIsn23p8h = new byte[0];
			dKMLoMpMs = new byte[0];
			ghLACNa05 = IntPtr.Zero;
			c9FNce5cf = IntPtr.Zero;
			diL3t0peo = new string[0];
			sMgC0o5PW = new int[0];
			S0FvrGWpN = 1;
			hSjGubHK9 = false;
			d1uknJpcW = new SortedList();
			uS9zmJ6WC = 0;
			i244bikuos = 0L;
			bFB44BUGlg = null;
			x3c4o2PyTx = null;
			phV4Uu6SUx = 0L;
			Qwp4ejR7FG = 0;
			TWn4MujlZv = false;
			NFL4IGyoc7 = false;
			WS94a0Vnlv = 0;
			XtL4lyIIgx = IntPtr.Zero;
			firstrundone = false;
			IBe4hEip2A = new Hashtable();
			try
			{
				RSACryptoServiceProvider.UseMachineKeyStore = true;
			}
			catch
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void qk\u0095\u000d\u000a\u00966\u009dxy1j5g9()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static byte[] creoiNvd7(byte[]  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			uint[] array = new uint[16];
			int num = 448 -  .Length * 8 % 512;
			uint num2 = (uint)((num + 512) % 512);
			if (num2 == 0)
			{
				num2 = 512u;
			}
			uint num3 = (uint)( .Length + num2 / 8u + 8);
			ulong num4 = (ulong) .Length * 8uL;
			byte[] array2 = new byte[num3];
			for (int i = 0; i <  .Length; i++)
			{
				array2[i] =  [i];
			}
			array2[ .Length] |= 128;
			for (int num5 = 8; num5 > 0; num5--)
			{
				array2[num3 - num5] = (byte)((num4 >> (8 - num5) * 8) & 0xFF);
			}
			uint num6 = (uint)(array2.Length * 8) / 32u;
			uint num7 = 1732584193u;
			uint num8 = 4023233417u;
			uint num9 = 2562383102u;
			uint num10 = 271733878u;
			for (uint num11 = 0u; num11 < num6 / 16u; num11++)
			{
				uint num12 = num11 << 6;
				for (uint num13 = 0u; num13 < 61; num13 += 4)
				{
					array[num13 >> 2] = (uint)((array2[num12 + (num13 + 3)] << 24) | (array2[num12 + (num13 + 2)] << 16) | (array2[num12 + (num13 + 1)] << 8) | array2[num12 + num13]);
				}
				uint num14 = num7;
				uint num15 = num8;
				uint num16 = num9;
				uint num17 = num10;
				jZiU8kt7k(ref num7, num8, num9, num10, 0u, 7, 1u, array);
				jZiU8kt7k(ref num10, num7, num8, num9, 1u, 12, 2u, array);
				jZiU8kt7k(ref num9, num10, num7, num8, 2u, 17, 3u, array);
				jZiU8kt7k(ref num8, num9, num10, num7, 3u, 22, 4u, array);
				jZiU8kt7k(ref num7, num8, num9, num10, 4u, 7, 5u, array);
				jZiU8kt7k(ref num10, num7, num8, num9, 5u, 12, 6u, array);
				jZiU8kt7k(ref num9, num10, num7, num8, 6u, 17, 7u, array);
				jZiU8kt7k(ref num8, num9, num10, num7, 7u, 22, 8u, array);
				jZiU8kt7k(ref num7, num8, num9, num10, 8u, 7, 9u, array);
				jZiU8kt7k(ref num10, num7, num8, num9, 9u, 12, 10u, array);
				jZiU8kt7k(ref num9, num10, num7, num8, 10u, 17, 11u, array);
				jZiU8kt7k(ref num8, num9, num10, num7, 11u, 22, 12u, array);
				jZiU8kt7k(ref num7, num8, num9, num10, 12u, 7, 13u, array);
				jZiU8kt7k(ref num10, num7, num8, num9, 13u, 12, 14u, array);
				jZiU8kt7k(ref num9, num10, num7, num8, 14u, 17, 15u, array);
				jZiU8kt7k(ref num8, num9, num10, num7, 15u, 22, 16u, array);
				yIEeUuogE(ref num7, num8, num9, num10, 1u, 5, 17u, array);
				yIEeUuogE(ref num10, num7, num8, num9, 6u, 9, 18u, array);
				yIEeUuogE(ref num9, num10, num7, num8, 11u, 14, 19u, array);
				yIEeUuogE(ref num8, num9, num10, num7, 0u, 20, 20u, array);
				yIEeUuogE(ref num7, num8, num9, num10, 5u, 5, 21u, array);
				yIEeUuogE(ref num10, num7, num8, num9, 10u, 9, 22u, array);
				yIEeUuogE(ref num9, num10, num7, num8, 15u, 14, 23u, array);
				yIEeUuogE(ref num8, num9, num10, num7, 4u, 20, 24u, array);
				yIEeUuogE(ref num7, num8, num9, num10, 9u, 5, 25u, array);
				yIEeUuogE(ref num10, num7, num8, num9, 14u, 9, 26u, array);
				yIEeUuogE(ref num9, num10, num7, num8, 3u, 14, 27u, array);
				yIEeUuogE(ref num8, num9, num10, num7, 8u, 20, 28u, array);
				yIEeUuogE(ref num7, num8, num9, num10, 13u, 5, 29u, array);
				yIEeUuogE(ref num10, num7, num8, num9, 2u, 9, 30u, array);
				yIEeUuogE(ref num9, num10, num7, num8, 7u, 14, 31u, array);
				yIEeUuogE(ref num8, num9, num10, num7, 12u, 20, 32u, array);
				HNMMnrD0K(ref num7, num8, num9, num10, 5u, 4, 33u, array);
				HNMMnrD0K(ref num10, num7, num8, num9, 8u, 11, 34u, array);
				HNMMnrD0K(ref num9, num10, num7, num8, 11u, 16, 35u, array);
				HNMMnrD0K(ref num8, num9, num10, num7, 14u, 23, 36u, array);
				HNMMnrD0K(ref num7, num8, num9, num10, 1u, 4, 37u, array);
				HNMMnrD0K(ref num10, num7, num8, num9, 4u, 11, 38u, array);
				HNMMnrD0K(ref num9, num10, num7, num8, 7u, 16, 39u, array);
				HNMMnrD0K(ref num8, num9, num10, num7, 10u, 23, 40u, array);
				HNMMnrD0K(ref num7, num8, num9, num10, 13u, 4, 41u, array);
				HNMMnrD0K(ref num10, num7, num8, num9, 0u, 11, 42u, array);
				HNMMnrD0K(ref num9, num10, num7, num8, 3u, 16, 43u, array);
				HNMMnrD0K(ref num8, num9, num10, num7, 6u, 23, 44u, array);
				HNMMnrD0K(ref num7, num8, num9, num10, 9u, 4, 45u, array);
				HNMMnrD0K(ref num10, num7, num8, num9, 12u, 11, 46u, array);
				HNMMnrD0K(ref num9, num10, num7, num8, 15u, 16, 47u, array);
				HNMMnrD0K(ref num8, num9, num10, num7, 2u, 23, 48u, array);
				U6ZIpjiMV(ref num7, num8, num9, num10, 0u, 6, 49u, array);
				U6ZIpjiMV(ref num10, num7, num8, num9, 7u, 10, 50u, array);
				U6ZIpjiMV(ref num9, num10, num7, num8, 14u, 15, 51u, array);
				U6ZIpjiMV(ref num8, num9, num10, num7, 5u, 21, 52u, array);
				U6ZIpjiMV(ref num7, num8, num9, num10, 12u, 6, 53u, array);
				U6ZIpjiMV(ref num10, num7, num8, num9, 3u, 10, 54u, array);
				U6ZIpjiMV(ref num9, num10, num7, num8, 10u, 15, 55u, array);
				U6ZIpjiMV(ref num8, num9, num10, num7, 1u, 21, 56u, array);
				U6ZIpjiMV(ref num7, num8, num9, num10, 8u, 6, 57u, array);
				U6ZIpjiMV(ref num10, num7, num8, num9, 15u, 10, 58u, array);
				U6ZIpjiMV(ref num9, num10, num7, num8, 6u, 15, 59u, array);
				U6ZIpjiMV(ref num8, num9, num10, num7, 13u, 21, 60u, array);
				U6ZIpjiMV(ref num7, num8, num9, num10, 4u, 6, 61u, array);
				U6ZIpjiMV(ref num10, num7, num8, num9, 11u, 10, 62u, array);
				U6ZIpjiMV(ref num9, num10, num7, num8, 2u, 15, 63u, array);
				U6ZIpjiMV(ref num8, num9, num10, num7, 9u, 21, 64u, array);
				num7 += num14;
				num8 += num15;
				num9 += num16;
				num10 += num17;
			}
			byte[] array3 = new byte[16];
			Array.Copy(BitConverter.GetBytes(num7), 0, array3, 0, 4);
			Array.Copy(BitConverter.GetBytes(num8), 0, array3, 4, 4);
			Array.Copy(BitConverter.GetBytes(num9), 0, array3, 8, 4);
			Array.Copy(BitConverter.GetBytes(num10), 0, array3, 12, 4);
			return array3;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void jZiU8kt7k(ref uint  , uint  , uint  , uint  , uint  , ushort  , uint  , uint[]  )
		{
			  += TYIaeXNeW(  + ((  &  ) | (~  &  )) +  [ ] + M6EKmwjSJ[  - 1],  );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void yIEeUuogE(ref uint  , uint  , uint  , uint  , uint  , ushort  , uint  , uint[]  )
		{
			  += TYIaeXNeW(  + ((  &  ) | (  & ~ )) +  [ ] + M6EKmwjSJ[  - 1],  );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void HNMMnrD0K(ref uint  , uint  , uint  , uint  , uint  , ushort  , uint  , uint[]  )
		{
			  += TYIaeXNeW(  + (  ^   ^  ) +  [ ] + M6EKmwjSJ[  - 1],  );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void U6ZIpjiMV(ref uint  , uint  , uint  , uint  , uint  , ushort  , uint  , uint[]  )
		{
			  += TYIaeXNeW(  + (  ^ (  | ~ )) +  [ ] + M6EKmwjSJ[  - 1],  );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static uint TYIaeXNeW(uint  , ushort  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return (  >> 32 -  ) | (  << (int) );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool rI3lmZ9FL()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			if (!PVVpfAGtG)
			{
				QWOOk18h0();
				PVVpfAGtG = true;
			}
			return cQCd71PIW;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static SymmetricAlgorithm SuhhReBcy()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			SymmetricAlgorithm symmetricAlgorithm = null;
			if (rI3lmZ9FL())
			{
				return new AesCryptoServiceProvider();
			}
			try
			{
				return new RijndaelManaged();
			}
			catch
			{
				return (SymmetricAlgorithm)Activator.CreateInstance("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Security.Cryptography.AesCryptoServiceProvider").Unwrap();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void QWOOk18h0()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			try
			{
				cQCd71PIW = CryptoConfig.AllowOnlyFipsAlgorithms;
			}
			catch
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static byte[] BjkXsyRir(byte[]  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			if (!rI3lmZ9FL())
			{
				return new MD5CryptoServiceProvider().ComputeHash( );
			}
			return creoiNvd7( );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[AXBrnIFfMAfABnJrF9(typeof(AXBrnIFfMAfABnJrF9.z0oyxsqySXMDuI4ZyY<object>[]))]
		internal static bool mCC9ZT9yx(int  )
		{
			//Discarded unreachable code: IL_0002, IL_0adf, IL_2c33, IL_2d17, IL_2d4d, IL_309b
			//IL_2cb8: Incompatible stack heights: 0 vs 1
			//IL_2cc2: Incompatible stack heights: 0 vs 1
			//IL_2ccc: Incompatible stack heights: 0 vs 1
			//IL_2d61: Incompatible stack heights: 0 vs 1
			//IL_2d6b: Incompatible stack heights: 0 vs 1
			//IL_2d8a: Incompatible stack heights: 0 vs 1
			//IL_2d94: Incompatible stack heights: 0 vs 1
			//IL_2d9e: Incompatible stack heights: 0 vs 1
			//IL_2da8: Incompatible stack heights: 0 vs 1
			//IL_2db2: Incompatible stack heights: 0 vs 1
			//IL_2dbc: Incompatible stack heights: 0 vs 1
			//IL_2dc6: Incompatible stack heights: 0 vs 1
			//IL_2dd0: Incompatible stack heights: 0 vs 1
			//IL_2dda: Incompatible stack heights: 0 vs 2
			//IL_2de4: Incompatible stack heights: 0 vs 1
			//IL_2dee: Incompatible stack heights: 0 vs 1
			//IL_2df8: Incompatible stack heights: 0 vs 1
			//IL_2e16: Incompatible stack heights: 0 vs 1
			//IL_2e20: Incompatible stack heights: 0 vs 1
			//IL_2e2a: Incompatible stack heights: 0 vs 1
			//IL_2e34: Incompatible stack heights: 0 vs 1
			//IL_2e3e: Incompatible stack heights: 0 vs 1
			//IL_2e48: Incompatible stack heights: 0 vs 1
			//IL_2e52: Incompatible stack heights: 0 vs 1
			//IL_2e66: Incompatible stack heights: 0 vs 2
			//IL_2e70: Incompatible stack heights: 0 vs 1
			//IL_2e7a: Incompatible stack heights: 0 vs 1
			//IL_2e84: Incompatible stack heights: 0 vs 1
			//IL_2e8e: Incompatible stack heights: 0 vs 1
			//IL_2e98: Incompatible stack heights: 0 vs 1
			//IL_2ea2: Incompatible stack heights: 0 vs 1
			//IL_2eac: Incompatible stack heights: 0 vs 1
			//IL_2eb6: Incompatible stack heights: 0 vs 1
			//IL_2ec0: Incompatible stack heights: 0 vs 1
			//IL_2eca: Incompatible stack heights: 0 vs 1
			//IL_2ed4: Incompatible stack heights: 0 vs 1
			//IL_2ede: Incompatible stack heights: 0 vs 1
			//IL_2ee8: Incompatible stack heights: 0 vs 1
			//IL_2ef2: Incompatible stack heights: 0 vs 1
			//IL_2f06: Incompatible stack heights: 0 vs 1
			//IL_2f10: Incompatible stack heights: 0 vs 1
			//IL_2f24: Incompatible stack heights: 0 vs 1
			//IL_2f2e: Incompatible stack heights: 0 vs 1
			//IL_2f38: Incompatible stack heights: 0 vs 1
			//IL_2f42: Incompatible stack heights: 0 vs 1
			//IL_2f4c: Incompatible stack heights: 0 vs 1
			//IL_2f56: Incompatible stack heights: 0 vs 1
			//IL_2f60: Incompatible stack heights: 0 vs 1
			//IL_2f6a: Incompatible stack heights: 0 vs 1
			//IL_2f74: Incompatible stack heights: 0 vs 1
			//IL_2f7e: Incompatible stack heights: 0 vs 1
			//IL_2f88: Incompatible stack heights: 0 vs 1
			//IL_2f92: Incompatible stack heights: 0 vs 1
			//IL_2f9c: Incompatible stack heights: 0 vs 1
			//IL_2fa6: Incompatible stack heights: 0 vs 1
			//IL_2fc4: Incompatible stack heights: 0 vs 1
			//IL_2fce: Incompatible stack heights: 0 vs 1
			//IL_2fd8: Incompatible stack heights: 0 vs 1
			//IL_2fe2: Incompatible stack heights: 0 vs 1
			//IL_2ff6: Incompatible stack heights: 0 vs 1
			//IL_3000: Incompatible stack heights: 0 vs 1
			//IL_300a: Incompatible stack heights: 0 vs 1
			//IL_3014: Incompatible stack heights: 0 vs 1
			//IL_301e: Incompatible stack heights: 0 vs 1
			//IL_3028: Incompatible stack heights: 0 vs 1
			//IL_3032: Incompatible stack heights: 0 vs 1
			//IL_303c: Incompatible stack heights: 0 vs 1
			//IL_3046: Incompatible stack heights: 0 vs 1
			//IL_3050: Incompatible stack heights: 0 vs 1
			//IL_305a: Incompatible stack heights: 0 vs 1
			//IL_3064: Incompatible stack heights: 0 vs 1
			//IL_306e: Incompatible stack heights: 0 vs 1
			//IL_3078: Incompatible stack heights: 0 vs 1
			//IL_3082: Incompatible stack heights: 0 vs 1
			//IL_308c: Incompatible stack heights: 0 vs 1
			//IL_3096: Incompatible stack heights: 0 vs 1
			while (false)
			{
				_ = ((object[])null)[0];
			}
			int num = 81;
			byte[] array3 = default(byte[]);
			int num8 = default(int);
			int num11 = default(int);
			byte[] array2 = default(byte[]);
			int num10 = default(int);
			int num9 = default(int);
			byte[] array5 = default(byte[]);
			byte[] array6 = default(byte[]);
			BinaryReader binaryReader = default(BinaryReader);
			byte[] array7 = default(byte[]);
			MemoryStream memoryStream = default(MemoryStream);
			byte[] array4 = default(byte[]);
			CryptoStream cryptoStream = default(CryptoStream);
			ICryptoTransform transform = default(ICryptoTransform);
			int num6 = default(int);
			SymmetricAlgorithm symmetricAlgorithm = default(SymmetricAlgorithm);
			bool result = default(bool);
			while (true)
			{
				int num2 = num;
				while (true)
				{
					IL_0ae8:
					int num3 = num2;
					while (true)
					{
						switch (num3)
						{
						case 336:
							break;
						case 43:
							array3[9] = (byte)num8;
							num2 = 136;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_0051: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 174;
						case 200:
							num11 = 238 - 79;
							num = 323;
							goto end_IL_0ae8;
						case 330:
							array2[8] = (byte)num10;
							num2 = 16;
							goto IL_0ae8;
						case 133:
							array3[12] = (byte)num8;
							num = 192;
							goto end_IL_0ae8;
						case 143:
							array2[24] = 26;
							num3 = 217;
							continue;
						case 28:
							num10 = 186 - 62;
							num2 = 10;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_00e4: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 146;
						case 7:
							num9 = 3 + 20;
							num = 175;
							goto end_IL_0ae8;
						case 5:
							num9 = 145 - 102;
							num = 3;
							goto end_IL_0ae8;
						case 271:
							array2[28] = 82;
							num2 = 34;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_0143: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 94;
						case 337:
							num11 = 236 - 78;
							num2 = 95;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_016a: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 288;
						case 95:
							array3[0] = (byte)num11;
							num3 = 241;
							continue;
						case 149:
							array2[28] = (byte)num10;
							num3 = 221;
							continue;
						case 185:
							array3[6] = 240;
							num = 305;
							goto end_IL_0ae8;
						case 179:
							num11 = 224 + 17;
							num3 = 20;
							continue;
						case 239:
							num10 = 184 + 7;
							num2 = 58;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 48;
						case 48:
							num8 = 81 + 13;
							num2 = 33;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_0217: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 303;
						case 131:
							num9 = 25 + 33;
							num = 106;
							goto end_IL_0ae8;
						case 211:
							num9 = 168 - 56;
							num2 = 13;
							goto IL_0ae8;
						case 160:
							array2[21] = (byte)num9;
							num = 218;
							goto end_IL_0ae8;
						case 151:
							array3[10] = 90;
							num2 = 176;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_0292: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 32;
						case 14:
							array2 = new byte[32];
							num = 226;
							goto end_IL_0ae8;
						case 175:
							array2[19] = (byte)num9;
							num = 6;
							goto end_IL_0ae8;
						case 0:
							array2[6] = (byte)num10;
							num2 = 90;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_02e8: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 151;
						case 173:
							array3[2] = (byte)num11;
							num2 = 304;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_030e: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 319;
						case 216:
							array2[26] = (byte)num9;
							num = 327;
							goto end_IL_0ae8;
						case 80:
							num10 = 181 - 60;
							num2 = 134;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 6;
						case 6:
							num10 = 140 + 7;
							num2 = 231;
							goto IL_0ae8;
						case 193:
							array5[1] = array6[0];
							num2 = 332;
							goto IL_0ae8;
						case 118:
							num9 = 36 + 34;
							num3 = 111;
							continue;
						case 54:
							num9 = 148 + 73;
							num2 = 52;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 299;
						case 299:
						{
							byLFjg1JT0D40pp2cs(binaryReader);
							s6NlJypZJSjEs2gqgj((object)/*Error near IL_2de4: Stack underflow*/);
							int num12 = (int)/*Error near IL_03c2: Stack underflow*/;
							ijGrTqvc1yM5Gp420u((object)/*Error near IL_2dee: Stack underflow*/, num12);
							array7 = (byte[])/*Error near IL_03c9: Stack underflow*/;
							num2 = 14;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_03dc: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 178;
						}
						case 166:
							lsWHlti4EhChsfALaA(memoryStream);
							num = 245;
							goto end_IL_0ae8;
						case 18:
							array4 = array2;
							num2 = 159;
							goto IL_0ae8;
						case 268:
							num10 = 62 + 47;
							num3 = 283;
							continue;
						case 127:
							num10 = 55 + 99;
							num2 = 103;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 281;
						case 281:
							array2[7] = (byte)num9;
							num2 = 123;
							goto IL_0ae8;
						case 140:
							if (array6 != null)
							{
								num2 = 15;
								goto IL_0ae8;
							}
							goto case 66;
						case 235:
							num10 = 65 + 31;
							num2 = 198;
							goto IL_0ae8;
						case 167:
							array3[14] = 142;
							num3 = 237;
							continue;
						case 265:
							array3[9] = (byte)num11;
							num = 197;
							goto end_IL_0ae8;
						case 189:
							pEXIllk0X5v7xWvpGD(cryptoStream);
							num2 = 210;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_04dc: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 174;
						case 174:
							num9 = 196 - 110;
							num3 = 109;
							continue;
						case 302:
							array3[2] = 156;
							num3 = 69;
							continue;
						case 300:
							array2[6] = 102;
							num3 = 23;
							continue;
						case 163:
							num10 = 67 + 25;
							num2 = 183;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 231;
						case 231:
							array2[19] = (byte)num10;
							num2 = 59;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 270;
						case 270:
							array2[25] = (byte)num10;
							num2 = 32;
							goto IL_0ae8;
						case 4:
							array3[0] = 83;
							num2 = 178;
							goto IL_0ae8;
						case 26:
							array2[30] = 41;
							num2 = 204;
							goto IL_0ae8;
						case 20:
							array3[10] = (byte)num11;
							num3 = 38;
							continue;
						case 141:
							array2[2] = 108;
							num3 = 116;
							continue;
						case 111:
							array2[17] = (byte)num9;
							num = 235;
							goto end_IL_0ae8;
						case 279:
							array3[8] = (byte)num8;
							num3 = 253;
							continue;
						case 320:
							num9 = 201 - 67;
							num2 = 258;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_0659: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 200;
						case 157:
							array2[31] = (byte)num10;
							num2 = 18;
							goto IL_0ae8;
						case 208:
							array2[0] = 185;
							num3 = 77;
							continue;
						case 202:
							array2[16] = 145;
							num3 = 232;
							continue;
						case 123:
							array2[7] = 156;
							num3 = 78;
							continue;
						case 256:
							array2[31] = (byte)num9;
							num2 = 37;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_06f8: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 155;
						case 155:
							array5[11] = array6[5];
							num2 = 11;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 237;
						case 237:
							num11 = 211 - 70;
							num = 105;
							goto end_IL_0ae8;
						case 61:
							array2[19] = 136;
							num = 7;
							goto end_IL_0ae8;
						case 286:
							array3[7] = (byte)num8;
							num = 309;
							goto end_IL_0ae8;
						case 292:
							array2[22] = (byte)num10;
							num = 275;
							goto end_IL_0ae8;
						case 33:
							array3[5] = (byte)num8;
							num = 96;
							goto end_IL_0ae8;
						case 117:
							array2[12] = 114;
							num3 = 53;
							continue;
						case 15:
							if (array6.Length > 0)
							{
								num2 = 193;
								goto IL_0ae8;
							}
							goto case 66;
						case 40:
							array3[3] = 113;
							num2 = 196;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_07ef: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 59;
						case 171:
							num8 = 235 - 78;
							num = 286;
							goto end_IL_0ae8;
						case 164:
							array2[11] = (byte)num10;
							num3 = 56;
							continue;
						case 97:
							num9 = 170 + 48;
							num = 55;
							goto end_IL_0ae8;
						case 135:
							array2[5] = (byte)num10;
							num3 = 209;
							continue;
						case 221:
							num10 = 12 + 27;
							num2 = 71;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_0878: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 2;
						case 56:
							array2[11] = 86;
							num = 249;
							goto end_IL_0ae8;
						case 119:
							num10 = 205 - 68;
							num2 = 82;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_08be: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 306;
						case 306:
							byLFjg1JT0D40pp2cs(binaryReader);
							EjrLuR2ZY7ms4AQOOb((object)/*Error near IL_2e5c: Stack underflow*/, 0L);
							num = 29;
							goto end_IL_0ae8;
						case 197:
							array3[9] = 57;
							num = 145;
							goto end_IL_0ae8;
						case 31:
							goto end_IL_0aec;
						case 129:
							num11 = 14 + 15;
							num2 = 228;
							goto IL_0ae8;
						case 60:
							num9 = 13 + 78;
							num3 = 92;
							continue;
						case 204:
							num9 = 101 + 52;
							num = 256;
							goto end_IL_0ae8;
						case 319:
							num11 = 74 + 2;
							num2 = 255;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 288;
						case 288:
							num9 = 95 + 122;
							num3 = 83;
							continue;
						case 59:
							array2[20] = 102;
							num2 = 312;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 47;
						case 47:
							array3[13] = 2;
							num = 63;
							goto end_IL_0ae8;
						case 91:
							array3[11] = (byte)num8;
							num = 184;
							goto end_IL_0ae8;
						case 184:
							array3[11] = 116;
							num3 = 137;
							continue;
						case 177:
							array2[8] = (byte)num9;
							num3 = 288;
							continue;
						case 112:
							array2[21] = 143;
							num2 = 98;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 206;
						case 206:
							num8 = 204 - 68;
							num3 = 133;
							continue;
						case 120:
							array2[29] = 90;
							num2 = 311;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 53;
						case 53:
							array2[12] = 23;
							num2 = 57;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_0ac5: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 138;
						case 138:
							array2[26] = 163;
							goto case 239;
						default:
							num3 = 239;
							continue;
						case 134:
							array2[3] = (byte)num10;
							num3 = 89;
							continue;
						case 324:
							num10 = 235 - 78;
							num3 = 207;
							continue;
						case 73:
							array2[4] = (byte)num10;
							num = 24;
							goto end_IL_0ae8;
						case 82:
							array2[10] = (byte)num10;
							num2 = 338;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 196;
						case 196:
							num8 = 75 - 15;
							num2 = 19;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 325;
						case 325:
							EZYBJ8F0ylktsnM1ti(j8hgmZJ7n);
							t8MFGHqlIE5SFcoIkT((object)/*Error near IL_2e84: Stack underflow*/);
							array6 = (byte[])/*Error near IL_10e0: Stack underflow*/;
							num3 = 140;
							continue;
						case 289:
							num10 = 147 - 49;
							num2 = 330;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_110c: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 251;
						case 251:
							array2[10] = 128;
							num = 119;
							goto end_IL_0ae8;
						case 2:
							array2[24] = (byte)num10;
							num3 = 250;
							continue;
						case 181:
							num8 = 181 - 103;
							num = 336;
							goto end_IL_0ae8;
						case 10:
							array2[0] = (byte)num10;
							num = 70;
							goto end_IL_0ae8;
						case 148:
							array3[15] = (byte)num11;
							num3 = 331;
							continue;
						case 296:
							num9 = 133 - 44;
							num2 = 272;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_11ae: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 28;
						case 124:
							num10 = 230 - 76;
							num2 = 266;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 77;
						case 77:
							array2[1] = 92;
							num = 328;
							goto end_IL_0ae8;
						case 92:
							array2[5] = (byte)num9;
							num3 = 285;
							continue;
						case 158:
							array2[4] = 88;
							num = 85;
							goto end_IL_0ae8;
						case 180:
							array2[28] = (byte)num9;
							num = 120;
							goto end_IL_0ae8;
						case 186:
							array2[2] = (byte)num9;
							num3 = 213;
							continue;
						case 234:
							new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
							cryptoStream = (CryptoStream)/*Error near IL_1263: Stack underflow*/;
							num3 = 195;
							continue;
						case 205:
							num11 = 34 + 61;
							num2 = 110;
							goto IL_0ae8;
						case 176:
							num11 = 10 + 93;
							num2 = 297;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 258;
						case 258:
							array2[19] = (byte)num9;
							num3 = 61;
							continue;
						case 71:
							array2[28] = (byte)num10;
							num2 = 269;
							goto IL_0ae8;
						case 272:
							array2[20] = (byte)num9;
							num = 124;
							goto end_IL_0ae8;
						case 273:
							array2[23] = (byte)num10;
							num3 = 42;
							continue;
						case 224:
							array2[4] = (byte)num10;
							num3 = 60;
							continue;
						case 318:
							array2[5] = (byte)num10;
							num2 = 300;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1345: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 258;
						case 37:
							array2[31] = 164;
							num3 = 261;
							continue;
						case 139:
							array3[12] = 36;
							num = 62;
							goto end_IL_0ae8;
						case 83:
							array2[8] = (byte)num9;
							num2 = 298;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_13a9: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 192;
						case 192:
							num11 = 225 - 102;
							num = 88;
							goto end_IL_0ae8;
						case 232:
							array2[16] = 157;
							num2 = 310;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 260;
						case 260:
							array3[14] = 147;
							num2 = 167;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_140e: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 164;
						case 187:
							array3[4] = (byte)num8;
							num2 = 230;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 99;
						case 99:
							array2[9] = (byte)num9;
							num = 126;
							goto end_IL_0ae8;
						case 287:
							array2[9] = (byte)num10;
							num = 223;
							goto end_IL_0ae8;
						case 110:
							array3[15] = (byte)num11;
							num = 200;
							goto end_IL_0ae8;
						case 253:
							num8 = 133 - 44;
							num = 114;
							goto end_IL_0ae8;
						case 69:
							num8 = 217 - 120;
							num2 = 301;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_14b3: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 225;
						case 225:
							array2[13] = 146;
							num = 203;
							goto end_IL_0ae8;
						case 323:
							array3[15] = (byte)num11;
							num2 = 295;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 303;
						case 303:
							array2[25] = 117;
							num = 291;
							goto end_IL_0ae8;
						case 322:
							array2[23] = 59;
							num = 41;
							goto end_IL_0ae8;
						case 137:
							array3[11] = 92;
							num2 = 139;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1555: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 253;
						case 3:
							array2[11] = (byte)num9;
							num3 = 9;
							continue;
						case 182:
							num11 = 194 - 64;
							num = 254;
							goto end_IL_0ae8;
						case 93:
							array3[1] = 55;
							num3 = 101;
							continue;
						case 282:
							array3[7] = (byte)num11;
							num = 181;
							goto end_IL_0ae8;
						case 170:
							array2[9] = 207;
							num3 = 8;
							continue;
						case 178:
							num8 = 186 - 62;
							num3 = 21;
							continue;
						case 116:
							array2[2] = 94;
							num2 = 80;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 50;
						case 50:
							array2[14] = 86;
							num = 54;
							goto end_IL_0ae8;
						case 240:
							array2[18] = 95;
							num2 = 320;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 199;
						case 199:
							array2[27] = 236;
							num = 308;
							goto end_IL_0ae8;
						case 277:
							array3[9] = (byte)num11;
							num3 = 146;
							continue;
						case 215:
							num10 = 213 - 71;
							num2 = 233;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_16cb: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 104;
						case 104:
							array2[15] = 206;
							num = 127;
							goto end_IL_0ae8;
						case 90:
							num10 = 223 - 74;
							num = 333;
							goto end_IL_0ae8;
						case 327:
							array2[26] = 162;
							num2 = 314;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 19;
						case 19:
							array3[3] = (byte)num8;
							num3 = 252;
							continue;
						case 333:
							array2[7] = (byte)num10;
							num = 211;
							goto end_IL_0ae8;
						case 27:
							array2[31] = (byte)num9;
							num2 = 131;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1778: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 228;
						case 115:
							num9 = 158 - 84;
							num2 = 316;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 23;
						case 23:
							array2[6] = 240;
							num2 = 163;
							goto IL_0ae8;
						case 128:
							array2[11] = (byte)num9;
							num3 = 290;
							continue;
						case 203:
							array2[13] = 178;
							num2 = 44;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 276;
						case 276:
							array3[2] = (byte)num11;
							num2 = 302;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 219;
						case 219:
							num10 = 120 + 102;
							num2 = 142;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 70;
						case 70:
							array2[0] = 167;
							num3 = 208;
							continue;
						case 152:
							num10 = 222 - 74;
							num3 = 273;
							continue;
						case 335:
							num9 = 228 - 76;
							num = 130;
							goto end_IL_0ae8;
						case 338:
							array2[10] = 200;
							num = 162;
							goto end_IL_0ae8;
						case 63:
							num8 = 87 + 65;
							num = 321;
							goto end_IL_0ae8;
						case 98:
							num9 = 169 - 49;
							num = 160;
							goto end_IL_0ae8;
						case 172:
							array2[28] = 43;
							num3 = 144;
							continue;
						case 42:
							num10 = 241 - 80;
							num3 = 244;
							continue;
						case 242:
							array2[18] = (byte)num10;
							num2 = 107;
							goto IL_0ae8;
						case 130:
							array2[29] = (byte)num9;
							num2 = 243;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 207;
						case 207:
							array2[6] = (byte)num10;
							num2 = 212;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 223;
						case 223:
							num9 = 146 - 48;
							num2 = 99;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 29;
						case 29:
							oeLdZuxiQiliYklUox(true);
							num2 = 299;
							goto IL_0ae8;
						case 263:
							num8 = 163 - 50;
							num = 43;
							goto end_IL_0ae8;
						case 257:
							array2[3] = (byte)num10;
							num2 = 174;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 122;
						case 122:
							array3[4] = (byte)num8;
							num = 182;
							goto end_IL_0ae8;
						case 214:
							array3[3] = (byte)num11;
							num2 = 40;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1a06: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 332;
						case 332:
							array5[3] = array6[1];
							num = 74;
							goto end_IL_0ae8;
						case 255:
							array3[13] = (byte)num11;
							num2 = 154;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1a39: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 195;
						case 195:
							BMQx4ZHGDgW4w6gyZ4(cryptoStream, array7, 0, array7.Length);
							num2 = 189;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_1a5a: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 305;
						case 305:
							array3[6] = 219;
							num = 247;
							goto end_IL_0ae8;
						case 106:
							array2[31] = (byte)num9;
							num2 = 36;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1a9a: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 76;
						case 76:
							array5[9] = array6[4];
							num3 = 155;
							continue;
						case 331:
							array5 = array3;
							num2 = 325;
							goto IL_0ae8;
						case 241:
							array3[0] = 162;
							num = 161;
							goto end_IL_0ae8;
						case 32:
							array2[25] = 99;
							num3 = 303;
							continue;
						case 285:
							num10 = 83 + 61;
							num2 = 135;
							goto IL_0ae8;
						case 145:
							num11 = 201 - 67;
							num2 = 277;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 275;
						case 275:
							array2[22] = 63;
							num = 125;
							goto end_IL_0ae8;
						case 64:
							num10 = 27 + 76;
							num2 = 262;
							goto IL_0ae8;
						case 308:
							array2[27] = 193;
							num = 227;
							goto end_IL_0ae8;
						case 230:
							num8 = 154 - 49;
							num2 = 246;
							goto IL_0ae8;
						case 67:
							array3[6] = (byte)num11;
							num = 30;
							goto end_IL_0ae8;
						case 44:
							array2[14] = 144;
							num3 = 50;
							continue;
						case 183:
							array2[6] = (byte)num10;
							num2 = 324;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 244;
						case 244:
							array2[23] = (byte)num10;
							num2 = 322;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1c27: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 222;
						case 222:
							num11 = 208 - 69;
							num = 282;
							goto end_IL_0ae8;
						case 150:
							array3[8] = 204;
							goto case 87;
						case 312:
							array2[20] = 54;
							num3 = 84;
							continue;
						case 21:
							array3[0] = (byte)num8;
							num3 = 337;
							continue;
						case 30:
							array3[6] = 102;
							num = 185;
							goto end_IL_0ae8;
						case 293:
							num10 = 45 + 102;
							num3 = 113;
							continue;
						case 161:
							array3[1] = 128;
							num2 = 93;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 88;
						case 88:
							array3[12] = (byte)num11;
							num2 = 129;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1d1d: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 329;
						case 329:
							array3[2] = 154;
							num3 = 264;
							continue;
						case 75:
							array2[7] = (byte)num9;
							num3 = 115;
							continue;
						case 274:
							array3[15] = (byte)num8;
							num2 = 205;
							goto IL_0ae8;
						case 8:
							array2[10] = 199;
							num = 251;
							goto end_IL_0ae8;
						case 109:
							array2[3] = (byte)num9;
							num = 158;
							goto end_IL_0ae8;
						case 313:
							array2[25] = 226;
							num = 307;
							goto end_IL_0ae8;
						case 79:
							array2[1] = 55;
							num2 = 35;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 248;
						case 248:
							num6 = 0;
							num = 339;
							goto end_IL_0ae8;
						case 38:
							array3[11] = 124;
							num2 = 190;
							goto IL_0ae8;
						case 291:
							array2[25] = 105;
							num2 = 313;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 108;
						case 108:
							array3[8] = 75;
							num = 238;
							goto end_IL_0ae8;
						case 11:
							array5[13] = array6[6];
							num2 = 188;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1e79: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 310;
						case 261:
							array2[31] = 144;
							num2 = 72;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1ea6: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 39;
						case 39:
							if (lodECQQVs.Length == 0)
							{
								num3 = 147;
								continue;
							}
							goto case 248;
						case 96:
							array3[5] = 133;
							num2 = 12;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 22;
						case 22:
							num10 = 144 - 62;
							num2 = 318;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 220;
						case 220:
							array3[12] = 139;
							num3 = 206;
							continue;
						case 311:
							num10 = 99 + 21;
							num3 = 49;
							continue;
						case 45:
							array2[26] = 124;
							num = 138;
							goto end_IL_0ae8;
						case 36:
							num10 = 112 - 75;
							num2 = 157;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 326;
						case 326:
							num10 = 249 - 83;
							num = 242;
							goto end_IL_0ae8;
						case 238:
							num11 = 38 + 58;
							num2 = 265;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 114;
						case 114:
							array3[8] = (byte)num8;
							num2 = 108;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_1fcb: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 145;
						case 264:
							num11 = 204 - 68;
							num = 173;
							goto end_IL_0ae8;
						case 81:
							if (VvPxdPh3O.Length == 0)
							{
								num2 = 315;
								dgDhRq7iq3m3W3BBsY();
								if ((int)/*Error near IL_2008: Stack underflow*/ == 0)
								{
									goto IL_0ae8;
								}
								goto case 177;
							}
							goto case 39;
						case 154:
							array3[13] = 146;
							num = 47;
							goto end_IL_0ae8;
						case 269:
							array2[28] = 230;
							num = 172;
							goto end_IL_0ae8;
						case 94:
						case 194:
							array2[30] = 144;
							num2 = 215;
							goto IL_0ae8;
						case 1:
							array2[29] = (byte)num9;
							num2 = 31;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_208f: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 210;
						case 210:
							EUOB16lP9J1TjE6ALn(memoryStream);
							VvPxdPh3O = (byte[])/*Error near IL_209b: Stack underflow*/;
							num2 = 166;
							goto IL_0ae8;
						case 132:
							array2[27] = 160;
							num2 = 199;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_20d1: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 283;
						case 52:
							array2[14] = (byte)num9;
							num2 = 104;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 266;
						case 266:
							array2[20] = (byte)num10;
							num3 = 284;
							continue;
						case 46:
							num11 = 15 + 62;
							num2 = 67;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 201;
						case 201:
							num10 = 199 - 66;
							num2 = 287;
							goto IL_0ae8;
						case 165:
							array2[15] = 218;
							num2 = 202;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_216e: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 310;
						case 310:
							array2[16] = 31;
							num3 = 118;
							continue;
						case 247:
							array3[7] = 101;
							num3 = 171;
							continue;
						case 84:
							num9 = 221 - 73;
							num3 = 267;
							continue;
						case 72:
							num9 = 36 + 67;
							num3 = 27;
							continue;
						case 254:
							array3[4] = (byte)num11;
							num = 153;
							goto end_IL_0ae8;
						case 101:
							array3[1] = 245;
							num3 = 329;
							continue;
						case 74:
							array5[5] = array6[2];
							num2 = 280;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 250;
						case 250:
							num10 = 193 - 64;
							num2 = 270;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 102;
						case 144:
							num9 = 119 - 65;
							num = 180;
							goto end_IL_0ae8;
						case 283:
							array2[17] = (byte)num10;
							num2 = 293;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_2295: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 188;
						case 188:
							array5[15] = array6[7];
							num2 = 66;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 316;
						case 316:
							array2[7] = (byte)num9;
							num = 65;
							goto end_IL_0ae8;
						case 107:
							num9 = 22 + 81;
							num2 = 25;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 218;
						case 218:
							num10 = 39 + 76;
							num2 = 292;
							goto IL_0ae8;
						case 245:
							lsWHlti4EhChsfALaA(cryptoStream);
							num = 229;
							goto end_IL_0ae8;
						case 86:
							NAXUMbIwDNgos1sBmT(symmetricAlgorithm, CipherMode.CBC);
							num = 294;
							goto end_IL_0ae8;
						case 57:
							array2[13] = 85;
							num2 = 225;
							goto IL_0ae8;
						case 125:
							num9 = 92 + 73;
							num = 191;
							goto end_IL_0ae8;
						case 298:
							array2[9] = 118;
							num = 201;
							goto end_IL_0ae8;
						case 168:
							array3[5] = (byte)num8;
							num = 46;
							goto end_IL_0ae8;
						case 213:
							array2[2] = 123;
							num2 = 219;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 78;
						case 78:
							num9 = 37 + 8;
							num3 = 75;
							continue;
						case 249:
							num9 = 89 + 19;
							num = 128;
							goto end_IL_0ae8;
						case 236:
							num9 = 111 + 33;
							num2 = 100;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 280;
						case 280:
							array5[7] = array6[3];
							num2 = 76;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_2425: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 210;
						case 62:
							array3[12] = 108;
							num = 220;
							goto end_IL_0ae8;
						case 169:
							num9 = 74 + 91;
							num3 = 281;
							continue;
						case 315:
							iPqb2MG5vuRl0loKTl(j8hgmZJ7n, "\u0095cp5\u0087c\u008b\u008f\u0091\u00948\u0099\u008d\u0089v2s\u0089.\u00965o7\u009f\u009d\u008c\u008aa\u0088\u008d\u0095\u0095k\u0098e\u0097\u0091");
							new BinaryReader((Stream)/*Error near IL_2fd8: Stack underflow*/);
							binaryReader = (BinaryReader)/*Error near IL_2478: Stack underflow*/;
							num = 306;
							goto end_IL_0ae8;
						case 156:
							array2[21] = 132;
							num3 = 112;
							continue;
						case 9:
							array2[12] = 29;
							num2 = 117;
							goto IL_0ae8;
						case 252:
							num8 = 117 + 111;
							num = 122;
							goto end_IL_0ae8;
						case 334:
							num8 = 182 - 60;
							num2 = 274;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_24ff: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 17;
						case 17:
							array2[9] = (byte)num9;
							num3 = 170;
							continue;
						case 229:
							mRTZbLhNC1cdCl6D4w(binaryReader);
							num3 = 39;
							continue;
						case 191:
							array2[22] = (byte)num9;
							num2 = 152;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_2549: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 278;
						case 278:
							num9 = 76 + 106;
							num2 = 186;
							goto IL_0ae8;
						case 304:
							num11 = 218 - 72;
							num = 276;
							goto end_IL_0ae8;
						case 58:
							array2[26] = (byte)num10;
							num3 = 132;
							continue;
						case 34:
							num10 = 203 - 67;
							num2 = 149;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 16;
						case 16:
							num9 = 201 - 67;
							num2 = 177;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 162;
						case 162:
							num10 = 46 + 28;
							num = 164;
							goto end_IL_0ae8;
						case 24:
							num10 = 224 + 10;
							num3 = 224;
							continue;
						case 226:
							array2[0] = 132;
							num2 = 28;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 100;
						case 100:
							array2[17] = (byte)num9;
							num2 = 326;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 227;
						case 227:
							array2[27] = 190;
							num = 97;
							goto end_IL_0ae8;
						case 55:
							array2[27] = (byte)num9;
							num2 = 271;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_2686: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 85;
						case 85:
							num10 = 34 + 6;
							num2 = 73;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_26a8: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 159;
						case 159:
							array3 = new byte[16];
							num2 = 4;
							goto IL_0ae8;
						case 198:
							array2[17] = (byte)num10;
							num3 = 268;
							continue;
						case 25:
							array2[18] = (byte)num9;
							num = 240;
							goto end_IL_0ae8;
						case 121:
							array3[13] = 114;
							num = 319;
							goto end_IL_0ae8;
						case 284:
							array2[20] = 59;
							num2 = 156;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 126;
						case 126:
							num9 = 148 - 49;
							num2 = 17;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_2759: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 217;
						case 217:
							num10 = 113 + 34;
							num = 2;
							goto end_IL_0ae8;
						case 297:
							array3[10] = (byte)num11;
							num3 = 179;
							continue;
						case 65:
							num10 = 58 + 22;
							num2 = 68;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_27ac: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 136;
						case 136:
							array3[10] = 128;
							num2 = 151;
							goto IL_0ae8;
						case 294:
							fTkWdZTvILE01nbqsb(symmetricAlgorithm, array4, array5);
							transform = (ICryptoTransform)/*Error near IL_27dc: Stack underflow*/;
							num3 = 102;
							continue;
						case 290:
							array2[11] = 164;
							num2 = 64;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_280e: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 321;
						case 321:
							array3[14] = (byte)num8;
							num2 = 260;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_282f: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 233;
						case 233:
							array2[30] = (byte)num10;
							num = 26;
							goto end_IL_0ae8;
						case 103:
							array2[15] = (byte)num10;
							num3 = 165;
							continue;
						case 209:
							array2[5] = 131;
							num = 22;
							goto end_IL_0ae8;
						case 87:
						case 259:
							num8 = 107 + 64;
							num = 279;
							goto end_IL_0ae8;
						case 301:
							array3[2] = (byte)num8;
							num3 = 51;
							continue;
						case 228:
							array3[13] = (byte)num11;
							num2 = 121;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_28d0: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 51;
						case 51:
							num11 = 28 + 34;
							num3 = 214;
							continue;
						case 317:
							array2[1] = 83;
							num = 278;
							goto end_IL_0ae8;
						case 307:
							num9 = 194 - 64;
							num = 216;
							goto end_IL_0ae8;
						case 314:
							array2[26] = 108;
							num3 = 45;
							continue;
						case 41:
							array2[24] = 110;
							num2 = 143;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_2968: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 278;
						case 328:
							array2[1] = 96;
							num2 = 79;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_2995: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 6;
						case 295:
							num11 = 189 - 70;
							num3 = 148;
							continue;
						case 190:
							num8 = 205 - 68;
							num2 = 91;
							goto IL_0ae8;
						case 89:
							num10 = 29 + 117;
							num = 257;
							goto end_IL_0ae8;
						case 105:
							array3[15] = (byte)num11;
							num2 = 334;
							dgDhRq7iq3m3W3BBsY();
							if ((int)/*Error near IL_2a0a: Stack underflow*/ == 0)
							{
								goto IL_0ae8;
							}
							goto case 146;
						case 146:
							array3[9] = 220;
							num2 = 263;
							if (true)
							{
								goto IL_0ae8;
							}
							goto case 246;
						case 246:
							array3[4] = (byte)num8;
							num3 = 48;
							continue;
						case 35:
							array2[1] = 153;
							num2 = 317;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 267;
						case 267:
							array2[20] = (byte)num9;
							num2 = 296;
							goto IL_0ae8;
						case 113:
							array2[17] = (byte)num10;
							num = 236;
							goto end_IL_0ae8;
						case 309:
							array3[7] = 95;
							num3 = 222;
							continue;
						case 68:
							array2[8] = (byte)num10;
							num3 = 289;
							continue;
						case 49:
							array2[29] = (byte)num10;
							num = 335;
							goto end_IL_0ae8;
						case 142:
							array2[2] = (byte)num10;
							num2 = 141;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_2b0e: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 157;
						case 147:
							th0esw8RdOD424WJbc(j8hgmZJ7n);
							((object)/*Error near IL_3082: Stack underflow*/).ToString();
							xx9ikTnlOw8AXB6iGv((object)/*Error near IL_308c: Stack underflow*/);
							lodECQQVs = (byte[])/*Error near IL_2b2c: Stack underflow*/;
							num = 248;
							goto end_IL_0ae8;
						case 212:
							num10 = 154 - 61;
							num3 = 0;
							continue;
						case 262:
							array2[11] = (byte)num10;
							num2 = 5;
							kO5nUij9Sn6maXBNYh();
							if ((int)/*Error near IL_2b70: Stack underflow*/ != 0)
							{
								goto IL_0ae8;
							}
							goto case 13;
						case 13:
							array2[7] = (byte)num9;
							num3 = 169;
							continue;
						case 243:
							num9 = 92 + 83;
							num3 = 1;
							continue;
						case 153:
							num8 = 38 + 34;
							num2 = 187;
							if (0 == 0)
							{
								goto IL_0ae8;
							}
							goto case 12;
						case 12:
							num8 = 98 - 70;
							num3 = 168;
							continue;
						case 339:
							try
							{
								byte[] array = new byte[4];
								dgDhRq7iq3m3W3BBsY();
								kO5nUij9Sn6maXBNYh();
								int num4;
								if ((int)/*Error near IL_2bef: Stack underflow*/ != 0)
								{
									num4 = 2;
									if (false)
									{
										goto IL_2bff;
									}
								}
								else
								{
									num4 = 4;
									if (false)
									{
										goto IL_2c21;
									}
								}
								goto IL_2c3c;
								IL_2c6b:
								array[3] = VvPxdPh3O[  + 3];
								num4 = 6;
								if (true)
								{
									goto IL_2c3c;
								}
								goto IL_2c87;
								IL_2c87:
								array[0] = VvPxdPh3O[ ];
								int num5 = 5;
								goto IL_2c38;
								IL_2c3c:
								switch (num4)
								{
								case 1:
									break;
								case 5:
									goto IL_2c21;
								default:
									goto IL_2c61;
								case 3:
								case 4:
									goto IL_2c6b;
								case 0:
								case 2:
									goto IL_2c87;
								case 6:
									LLfZLGcRPZ8mBUurk1(array, 0);
									num6 = (int)/*Error near IL_2cd3: Stack underflow*/;
									goto end_IL_2bd8;
								}
								goto IL_2bff;
								IL_2c61:
								num5 = 1;
								goto IL_2c38;
								IL_2c38:
								num4 = num5;
								goto IL_2c3c;
								IL_2c21:
								array[1] = VvPxdPh3O[  + 1];
								goto IL_2bff;
								IL_2bff:
								array[2] = VvPxdPh3O[  + 2];
								goto IL_2c6b;
								end_IL_2bd8:;
							}
							catch
							{
							}
							try
							{
								if (lodECQQVs[num6] == 128)
								{
									kO5nUij9Sn6maXBNYh();
									dgDhRq7iq3m3W3BBsY();
									int num7;
									if ((int)/*Error near IL_2cff: Stack underflow*/ == 0)
									{
										num7 = 2;
										if (false)
										{
											goto IL_2d0f;
										}
									}
									else
									{
										num7 = 3;
									}
									switch (num7)
									{
									case 0:
									case 2:
										break;
									default:
										return result;
									case 3:
										return result;
									}
									goto IL_2d0f;
								}
								goto end_IL_2cde;
								IL_2d0f:
								return true;
								end_IL_2cde:;
							}
							catch
							{
							}
							return false;
						case 66:
							QEue6GJ234R4VGGwY9();
							symmetricAlgorithm = (SymmetricAlgorithm)/*Error near IL_1592: Stack underflow*/;
							num3 = 86;
							continue;
						case 102:
							new MemoryStream();
							memoryStream = (MemoryStream)/*Error near IL_2251: Stack underflow*/;
							num = 234;
							goto end_IL_0ae8;
						}
						array3[7] = (byte)num8;
						num = 150;
						goto end_IL_0ae8;
						continue;
						end_IL_0aec:
						break;
					}
					array2[29] = 90;
					_ = 1;
					dgDhRq7iq3m3W3BBsY();
					num2 = (((int)/*Error near IL_0915: Stack underflow*/ != 0) ? 87 : 94);
					continue;
					end_IL_0ae8:
					break;
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[AXBrnIFfMAfABnJrF9(typeof(AXBrnIFfMAfABnJrF9.z0oyxsqySXMDuI4ZyY<object>[]))]
		internal static string KX0HrYNeb(int  )
		{
			//Discarded unreachable code: IL_0002, IL_02e6, IL_3de6
			//IL_39d9: Incompatible stack heights: 0 vs 1
			//IL_39e3: Incompatible stack heights: 0 vs 1
			//IL_3a03: Incompatible stack heights: 0 vs 1
			//IL_3a0d: Incompatible stack heights: 0 vs 1
			//IL_3a17: Incompatible stack heights: 0 vs 1
			//IL_3a21: Incompatible stack heights: 0 vs 1
			//IL_3a2b: Incompatible stack heights: 0 vs 1
			//IL_3a35: Incompatible stack heights: 0 vs 1
			//IL_3a3f: Incompatible stack heights: 0 vs 1
			//IL_3a49: Incompatible stack heights: 0 vs 1
			//IL_3a53: Incompatible stack heights: 0 vs 1
			//IL_3a5d: Incompatible stack heights: 0 vs 1
			//IL_3a67: Incompatible stack heights: 0 vs 1
			//IL_3a71: Incompatible stack heights: 0 vs 1
			//IL_3a7b: Incompatible stack heights: 0 vs 1
			//IL_3a85: Incompatible stack heights: 0 vs 1
			//IL_3a8f: Incompatible stack heights: 0 vs 1
			//IL_3a99: Incompatible stack heights: 0 vs 1
			//IL_3aad: Incompatible stack heights: 0 vs 1
			//IL_3ab7: Incompatible stack heights: 0 vs 1
			//IL_3ac1: Incompatible stack heights: 0 vs 1
			//IL_3acb: Incompatible stack heights: 0 vs 1
			//IL_3ad5: Incompatible stack heights: 0 vs 1
			//IL_3adf: Incompatible stack heights: 0 vs 1
			//IL_3ae9: Incompatible stack heights: 0 vs 1
			//IL_3b07: Incompatible stack heights: 0 vs 1
			//IL_3b11: Incompatible stack heights: 0 vs 1
			//IL_3b25: Incompatible stack heights: 0 vs 1
			//IL_3b2f: Incompatible stack heights: 0 vs 1
			//IL_3b39: Incompatible stack heights: 0 vs 1
			//IL_3b43: Incompatible stack heights: 0 vs 1
			//IL_3b4d: Incompatible stack heights: 0 vs 1
			//IL_3b57: Incompatible stack heights: 0 vs 1
			//IL_3b61: Incompatible stack heights: 0 vs 2
			//IL_3b6b: Incompatible stack heights: 0 vs 1
			//IL_3b75: Incompatible stack heights: 0 vs 1
			//IL_3b7f: Incompatible stack heights: 0 vs 1
			//IL_3b89: Incompatible stack heights: 0 vs 1
			//IL_3b93: Incompatible stack heights: 0 vs 1
			//IL_3b9d: Incompatible stack heights: 0 vs 1
			//IL_3ba7: Incompatible stack heights: 0 vs 1
			//IL_3bb1: Incompatible stack heights: 0 vs 1
			//IL_3bbb: Incompatible stack heights: 0 vs 1
			//IL_3bc5: Incompatible stack heights: 0 vs 1
			//IL_3bcf: Incompatible stack heights: 0 vs 1
			//IL_3bd9: Incompatible stack heights: 0 vs 1
			//IL_3be3: Incompatible stack heights: 0 vs 1
			//IL_3bed: Incompatible stack heights: 0 vs 1
			//IL_3bf7: Incompatible stack heights: 0 vs 1
			//IL_3c01: Incompatible stack heights: 0 vs 1
			//IL_3c0b: Incompatible stack heights: 0 vs 1
			//IL_3c15: Incompatible stack heights: 0 vs 1
			//IL_3c1f: Incompatible stack heights: 0 vs 1
			//IL_3c29: Incompatible stack heights: 0 vs 1
			//IL_3c33: Incompatible stack heights: 0 vs 1
			//IL_3c3d: Incompatible stack heights: 0 vs 1
			//IL_3c47: Incompatible stack heights: 0 vs 1
			//IL_3c51: Incompatible stack heights: 0 vs 1
			//IL_3c5b: Incompatible stack heights: 0 vs 1
			//IL_3c65: Incompatible stack heights: 0 vs 1
			//IL_3c6f: Incompatible stack heights: 0 vs 1
			//IL_3c79: Incompatible stack heights: 0 vs 1
			//IL_3c83: Incompatible stack heights: 0 vs 1
			//IL_3c8d: Incompatible stack heights: 0 vs 1
			//IL_3c97: Incompatible stack heights: 0 vs 1
			//IL_3ca1: Incompatible stack heights: 0 vs 1
			//IL_3cab: Incompatible stack heights: 0 vs 1
			//IL_3cb5: Incompatible stack heights: 0 vs 1
			//IL_3cc9: Incompatible stack heights: 0 vs 1
			//IL_3cdd: Incompatible stack heights: 0 vs 1
			//IL_3ce7: Incompatible stack heights: 0 vs 1
			//IL_3cf1: Incompatible stack heights: 0 vs 1
			//IL_3cfb: Incompatible stack heights: 0 vs 1
			//IL_3d05: Incompatible stack heights: 0 vs 1
			//IL_3d0f: Incompatible stack heights: 0 vs 1
			//IL_3d19: Incompatible stack heights: 0 vs 1
			//IL_3d23: Incompatible stack heights: 0 vs 1
			//IL_3d2d: Incompatible stack heights: 0 vs 1
			//IL_3d37: Incompatible stack heights: 0 vs 1
			//IL_3d41: Incompatible stack heights: 0 vs 1
			//IL_3d4b: Incompatible stack heights: 0 vs 1
			//IL_3d55: Incompatible stack heights: 0 vs 1
			//IL_3d5f: Incompatible stack heights: 0 vs 1
			//IL_3d73: Incompatible stack heights: 0 vs 1
			//IL_3d7d: Incompatible stack heights: 0 vs 1
			//IL_3d87: Incompatible stack heights: 0 vs 1
			//IL_3d91: Incompatible stack heights: 0 vs 1
			//IL_3d9b: Incompatible stack heights: 0 vs 1
			//IL_3da5: Incompatible stack heights: 0 vs 1
			//IL_3daf: Incompatible stack heights: 0 vs 1
			//IL_3db9: Incompatible stack heights: 0 vs 1
			//IL_3dc3: Incompatible stack heights: 0 vs 1
			//IL_3dcd: Incompatible stack heights: 0 vs 1
			//IL_3dd7: Incompatible stack heights: 0 vs 1
			while (false)
			{
				_ = ((object[])null)[0];
			}
			int num = 356;
			byte[] array2 = default(byte[]);
			int num5 = default(int);
			byte[] array5 = default(byte[]);
			int num6 = default(int);
			int num10 = default(int);
			uint num9 = default(uint);
			int num17 = default(int);
			byte[] array7 = default(byte[]);
			int num3 = default(int);
			uint num14 = default(uint);
			int num23 = default(int);
			byte[] array8 = default(byte[]);
			int num21 = default(int);
			uint num22 = default(uint);
			int num8 = default(int);
			byte[] array3 = default(byte[]);
			byte[] array4 = default(byte[]);
			int num11 = default(int);
			int num24 = default(int);
			CryptoStream cryptoStream = default(CryptoStream);
			int num16 = default(int);
			uint num12 = default(uint);
			int num18 = default(int);
			byte[] array6 = default(byte[]);
			BinaryReader binaryReader = default(BinaryReader);
			MemoryStream memoryStream = default(MemoryStream);
			int num20 = default(int);
			int num43 = default(int);
			SymmetricAlgorithm symmetricAlgorithm = default(SymmetricAlgorithm);
			ICryptoTransform transform = default(ICryptoTransform);
			uint num19 = default(uint);
			uint num15 = default(uint);
			uint num25 = default(uint);
			uint num13 = default(uint);
			int num2 = default(int);
			while (true)
			{
				int num4;
				int num7;
				switch (num)
				{
				case 272:
					array2[14] = (byte)num5;
					num4 = 287;
					goto IL_02ef;
				case 236:
					num5 = 193 - 64;
					num = 113;
					break;
				case 283:
					array5[1] = (byte)num6;
					num = 355;
					break;
				case 192:
					array2[29] = (byte)num5;
					num4 = 209;
					goto IL_02ef;
				case 185:
				case 291:
					num10++;
					num = 169;
					break;
				case 83:
					array5[6] = 158;
					num7 = 227;
					goto IL_02eb;
				case 392:
					num9 = 0u;
					num7 = 292;
					goto IL_02eb;
				case 259:
					num17 = array7.Length % 4;
					num4 = 50;
					goto IL_02ef;
				case 90:
					array2[27] = (byte)num5;
					num4 = 196;
					goto IL_02ef;
				case 362:
					num5 = 32 + 57;
					num = 425;
					break;
				case 251:
					num6 = 32 + 57;
					num7 = 350;
					goto IL_02eb;
				case 211:
					num3 = 39 + 108;
					num7 = 24;
					goto IL_02eb;
				case 323:
					num14 = (uint)(num23 * 4);
					num7 = 200;
					goto IL_02eb;
				case 239:
					num3 = 130 - 59;
					num4 = 345;
					if (1 == 0)
					{
						goto case 400;
					}
					goto IL_02ef;
				case 400:
					array8[num21 + 1] = (byte)((num22 & 0xFF00) >> 8);
					num = 120;
					break;
				case 15:
					num8 = 18 + 52;
					num4 = 31;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_01a6: Stack underflow*/ != 0)
					{
						goto case 438;
					}
					goto IL_02ef;
				case 438:
					num8 = 227 - 75;
					num = 145;
					break;
				case 380:
					num3 = 175 - 58;
					num4 = 104;
					if (1 == 0)
					{
						goto case 155;
					}
					goto IL_02ef;
				case 155:
					array2[1] = (byte)num5;
					num4 = 362;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_01fe: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 430;
				case 432:
					array2[15] = (byte)num3;
					num4 = 10;
					if (1 == 0)
					{
						goto case 264;
					}
					goto IL_02ef;
				case 264:
					array2[0] = 99;
					num4 = 99;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0248: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 204;
				case 371:
					num9 = (uint)((array7[num14 + 3] << 24) | (array7[num14 + 2] << 16) | (array7[num14 + 1] << 8) | array7[num14]);
					num7 = 393;
					goto IL_02eb;
				case 333:
					num6 = 214 - 71;
					num = 289;
					break;
				case 95:
					num5 = 197 - 65;
					num7 = 139;
					goto IL_02eb;
				case 384:
					array5[4] = (byte)num6;
					num7 = 389;
					goto IL_02eb;
				case 404:
					array2[28] = 176;
					goto case 316;
				default:
					num = 316;
					break;
				case 288:
					array5[3] = 54;
					num4 = 411;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0a1e: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 106;
				case 106:
					array5[4] = (byte)num8;
					num4 = 271;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 22;
				case 22:
					array2[30] = (byte)num3;
					num4 = 154;
					goto IL_02ef;
				case 353:
					array2[13] = 119;
					num4 = 39;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 389;
				case 389:
					array5[5] = 179;
					num = 250;
					break;
				case 286:
					array3[11] = array4[5];
					num4 = 328;
					goto IL_02ef;
				case 123:
					num3 = 118 - 0;
					num = 373;
					break;
				case 295:
					array5[11] = (byte)num8;
					num4 = 417;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 271;
				case 271:
					num6 = 156 + 68;
					num4 = 384;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 281;
				case 281:
					num3 = 192 + 22;
					num7 = 183;
					goto IL_02eb;
				case 377:
					num3 = 177 - 59;
					num = 4;
					break;
				case 332:
					num5 = 97 - 46;
					num4 = 272;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 136;
				case 136:
					array8 = new byte[array7.Length];
					num4 = 105;
					goto IL_02ef;
				case 327:
					num6 = 45 + 120;
					num4 = 176;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_0b90: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 308;
				case 151:
					num3 = 6 + 43;
					num7 = 22;
					goto IL_02eb;
				case 137:
					array5[5] = (byte)num8;
					num4 = 406;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 330;
				case 330:
					num8 = 63 + 12;
					num4 = 262;
					goto IL_02ef;
				case 273:
					array2[9] = 124;
					num7 = 280;
					goto IL_02eb;
				case 411:
					num8 = 118 + 111;
					num4 = 302;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0c29: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 431;
				case 431:
					array2[6] = (byte)num5;
					num = 233;
					break;
				case 38:
					num14 = 0u;
					num4 = 351;
					goto IL_02ef;
				case 78:
					array2[8] = (byte)num3;
					num4 = 68;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_0c73: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 45;
				case 102:
					num5 = 205 - 94;
					num4 = 117;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0c9a: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 110;
				case 110:
					array2[12] = (byte)num3;
					num = 1;
					break;
				case 157:
					array2[13] = (byte)num5;
					num = 34;
					break;
				case 166:
					array2[7] = (byte)num5;
					num = 119;
					break;
				case 130:
					num3 = 8 + 8;
					num4 = 205;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 50;
				case 50:
					num11 = array7.Length / 4;
					num4 = 136;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_0d1b: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 176;
				case 53:
					num5 = 133 - 44;
					num4 = 249;
					goto IL_02ef;
				case 266:
					array5 = new byte[16];
					num7 = 141;
					goto IL_02eb;
				case 351:
					num10 = 0;
					num4 = 160;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0d6b: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 213;
				case 213:
					array5[4] = (byte)num8;
					num7 = 73;
					goto IL_02eb;
				case 142:
					num5 = 50 + 105;
					num = 173;
					break;
				case 189:
					num8 = 104 + 21;
					num7 = 6;
					goto IL_02eb;
				case 268:
					num5 = 66 + 114;
					num4 = 234;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0dd7: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 377;
				case 435:
					array5[9] = (byte)num6;
					num7 = 42;
					goto IL_02eb;
				case 303:
					array5[12] = 118;
					num7 = 150;
					goto IL_02eb;
				case 168:
					num3 = 123 + 102;
					z5DnODHQiwNxpdaCsc();
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0e31: Stack underflow*/ != 0)
					{
						num4 = 372;
						z5DnODHQiwNxpdaCsc();
						if ((int)/*Error near IL_0e45: Stack underflow*/ == 0)
						{
							goto IL_02ef;
						}
						goto case 77;
					}
					num = 252;
					break;
				case 182:
					num24 = 0;
					num4 = 67;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 216;
				case 216:
					num5 = 231 - 77;
					num7 = 155;
					goto IL_02eb;
				case 370:
					num3 = 227 - 75;
					num = 229;
					break;
				case 366:
					num8 = 146 - 48;
					num4 = 36;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_0ecb: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 350;
				case 350:
					array5[1] = (byte)num6;
					num = 25;
					break;
				case 345:
					array2[12] = (byte)num3;
					num7 = 253;
					goto IL_02eb;
				case 181:
					num11++;
					num4 = 38;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_0f14: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 61;
				case 61:
					owxJFjWa7h5kWcIDDl(cryptoStream);
					num4 = 23;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_0f2e: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 306;
				case 82:
					num24 += 8;
					num4 = 70;
					goto IL_02ef;
				case 206:
					array2[16] = 148;
					num7 = 348;
					goto IL_02eb;
				case 290:
					array5[8] = (byte)num6;
					num4 = 191;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_0f87: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 390;
				case 390:
					array2[22] = (byte)num5;
					num4 = 370;
					goto IL_02ef;
				case 191:
					num6 = 35 + 32;
					num4 = 52;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 297;
				case 297:
					array5[8] = (byte)num6;
					num = 244;
					break;
				case 408:
					array2[3] = (byte)num3;
					num = 44;
					break;
				case 60:
					num3 = 108 + 116;
					num = 63;
					break;
				case 401:
					array5[7] = 146;
					num = 221;
					break;
				case 195:
					num9 |= array7[^(1 + num16)];
					num7 = 282;
					goto IL_02eb;
				case 89:
					num3 = 58 + 63;
					num4 = 255;
					goto IL_02ef;
				case 220:
					num3 = 91 - 0;
					num7 = 416;
					goto IL_02eb;
				case 284:
					array2[18] = 110;
					num = 243;
					break;
				case 367:
					num12 = 0u;
					num4 = 383;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 202;
				case 202:
					num5 = 58 + 81;
					num = 107;
					break;
				case 343:
					array2[16] = 140;
					num = 418;
					break;
				case 386:
					if (num17 <= 0)
					{
						goto IL_0e93;
					}
					num4 = 20;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_10fe: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 325;
				case 325:
					num6 = 151 - 66;
					num4 = 223;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1120: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 389;
				case 103:
					array2[5] = (byte)num5;
					num4 = 324;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_1146: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 148;
				case 120:
					array8[num21 + 2] = (byte)((num22 & 0xFF0000) >> 16);
					num = 276;
					break;
				case 204:
				case 403:
					if (num18 >= array3.Length)
					{
						num4 = 122;
						R3TVja2vZlnVEbMYYF();
						if ((int)/*Error near IL_1186: Stack underflow*/ != 0)
						{
							goto IL_02ef;
						}
						goto case 260;
					}
					array6[num18] = (byte)(array6[num18] ^ array3[num18]);
					num = 405;
					break;
				case 218:
					num3 = 8 + 73;
					num = 110;
					break;
				case 394:
					n8Grd6Vv25DLpRCsTV(binaryReader);
					DIdgMcmS7LIxdhnV9y((object)/*Error near IL_3af3: Stack underflow*/, 0L);
					num7 = 360;
					goto IL_02eb;
				case 104:
					array2[22] = (byte)num3;
					num = 29;
					break;
				case 277:
					array2[27] = (byte)num5;
					num7 = 306;
					goto IL_02eb;
				case 172:
					owxJFjWa7h5kWcIDDl(memoryStream);
					num4 = 61;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_1206: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 218;
				case 69:
					array2[5] = 110;
					num = 9;
					break;
				case 416:
					array2[23] = (byte)num3;
					num4 = 338;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_124b: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 373;
				case 373:
					array2[2] = (byte)num3;
					num = 0;
					break;
				case 420:
					array2[9] = (byte)num3;
					num7 = 124;
					goto IL_02eb;
				case 115:
					kxPACNxW9BO7L6G31k(array3);
					num = 248;
					break;
				case 376:
					num3 = 158 + 4;
					num4 = 158;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_12ae: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 144;
				case 144:
					array2[29] = 86;
					num4 = 57;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 46;
				case 46:
					array2[25] = (byte)num5;
					num7 = 188;
					goto IL_02eb;
				case 335:
					array6 = array2;
					num7 = 266;
					goto IL_02eb;
				case 148:
					num6 = 148 - 49;
					num4 = 127;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 436;
				case 436:
					num5 = 135 - 45;
					num4 = 339;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_133a: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 439;
				case 439:
					array2[10] = (byte)num3;
					num = 45;
					break;
				case 215:
				case 440:
					if (num16 >= num17)
					{
						num4 = 344;
						R3TVja2vZlnVEbMYYF();
						if ((int)/*Error near IL_136e: Stack underflow*/ != 0)
						{
							goto IL_02ef;
						}
						goto case 37;
					}
					if (num16 > 0)
					{
						num7 = 114;
						goto IL_02eb;
					}
					goto case 195;
				case 37:
					num5 = 171 - 57;
					num7 = 374;
					goto IL_02eb;
				case 412:
					array2[5] = (byte)num3;
					num = 79;
					break;
				case 338:
					num3 = 70 + 80;
					num4 = 307;
					goto IL_02ef;
				case 328:
					array3[13] = array4[6];
					num4 = 121;
					goto IL_02ef;
				case 276:
					array8[num21 + 3] = (byte)((num22 & 0xFF000000u) >> 24);
					num7 = 185;
					goto IL_02eb;
				case 396:
					array5[10] = (byte)num6;
					num = 354;
					break;
				case 9:
					num5 = 196 - 65;
					num4 = 103;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_142a: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 282;
				case 282:
					num16++;
					num4 = 215;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 369;
				case 369:
					array5[5] = (byte)num6;
					num7 = 98;
					goto IL_02eb;
				case 170:
					hIsn23p8h = array8;
					num4 = 395;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 311;
				case 311:
					num3 = 99 + 56;
					num7 = 298;
					goto IL_02eb;
				case 161:
					array5[15] = 191;
					num = 314;
					break;
				case 134:
					array2[20] = 84;
					num7 = 2;
					goto IL_02eb;
				case 354:
					array5[10] = 93;
					num7 = 278;
					goto IL_02eb;
				case 223:
					array5[11] = (byte)num6;
					num = 312;
					break;
				case 147:
					num6 = 14 + 122;
					num = 162;
					break;
				case 19:
					num20++;
					num4 = 96;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 154;
				case 154:
					array2[30] = 209;
					num4 = 207;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 17;
				case 17:
					num6 = 81 - 6;
					num7 = 369;
					goto IL_02eb;
				case 117:
					array2[25] = (byte)num5;
					num7 = 35;
					goto IL_02eb;
				case 285:
					array2[9] = (byte)num3;
					num4 = 273;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_159f: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 4;
				case 21:
					array2[6] = (byte)num3;
					num = 413;
					break;
				case 198:
					num3 = 130 - 43;
					num7 = 256;
					goto IL_02eb;
				case 358:
					if (array4 != null)
					{
						num4 = 208;
						goto IL_02ef;
					}
					goto case 8;
				case 175:
					array2[15] = (byte)num3;
					num = 143;
					break;
				case 443:
					array2[25] = 129;
					num4 = 322;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 309;
				case 309:
					num6 = 17 + 102;
					num4 = 435;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 112;
				case 112:
					num8 = 80 + 8;
					num = 213;
					break;
				case 109:
					num6 = 231 - 77;
					num7 = 402;
					goto IL_02eb;
				case 114:
					num9 <<= 8;
					num7 = 195;
					goto IL_02eb;
				case 212:
					array2[8] = 142;
					num4 = 12;
					goto IL_02ef;
				case 287:
					array2[15] = 100;
					num = 261;
					break;
				case 257:
					array5[7] = 160;
					num = 397;
					break;
				case 296:
					array2[21] = (byte)num5;
					num4 = 263;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1708: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 163;
				case 163:
					array2[4] = (byte)num5;
					num = 399;
					break;
				case 81:
					array2[30] = (byte)num3;
					num4 = 142;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 68;
				case 68:
					num3 = 103 + 18;
					num = 237;
					break;
				case 337:
					num5 = 177 + 54;
					num7 = 442;
					goto IL_02eb;
				case 201:
					array2[8] = (byte)num3;
					num7 = 193;
					goto IL_02eb;
				case 360:
				{
					n8Grd6Vv25DLpRCsTV(binaryReader);
					gvnVj6pORMhCv3F2J1((object)/*Error near IL_3b6b: Stack underflow*/);
					int num44 = (int)/*Error near IL_1796: Stack underflow*/;
					pP6gQvYy51t5JHwUV3((object)/*Error near IL_3b75: Stack underflow*/, num44);
					array7 = (byte[])/*Error near IL_179d: Stack underflow*/;
					num4 = 427;
					goto IL_02ef;
				}
				case 105:
					num43 = array6.Length / 4;
					num7 = 77;
					goto IL_02eb;
				case 378:
					num5 = 71 - 31;
					num = 33;
					break;
				case 18:
					t2I1HRISgUxYhBr3Dx(symmetricAlgorithm, array6, array3);
					transform = (ICryptoTransform)/*Error near IL_17e3: Stack underflow*/;
					num7 = 41;
					goto IL_02eb;
				case 131:
					array5[3] = (byte)num8;
					num7 = 438;
					goto IL_02eb;
				case 248:
					atF1Rdkck7mnjl1K13(j8hgmZJ7n);
					rksxtH9b1aE45V2xuA((object)/*Error near IL_3b93: Stack underflow*/);
					array4 = (byte[])/*Error near IL_1816: Stack underflow*/;
					num7 = 358;
					goto IL_02eb;
				case 97:
					num8 = 125 - 41;
					num7 = 131;
					goto IL_02eb;
				case 363:
					array2[9] = (byte)num5;
					num = 378;
					break;
				case 409:
					num21 = num10 * 4;
					num4 = 323;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 229;
				case 229:
					array2[23] = (byte)num3;
					num7 = 133;
					goto IL_02eb;
				case 382:
					array2[13] = (byte)num3;
					num7 = 89;
					goto IL_02eb;
				case 159:
					array5[9] = 78;
					num7 = 186;
					goto IL_02eb;
				case 245:
					array2[22] = 77;
					num4 = 380;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_18dd: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 364;
				case 364:
					array5[0] = (byte)num6;
					num4 = 66;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 222;
				case 222:
					num8 = 250 - 83;
					num4 = 62;
					goto IL_02ef;
				case 54:
					array2[20] = 88;
					num4 = 429;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 87;
				case 87:
					array2[30] = 166;
					num7 = 151;
					goto IL_02eb;
				case 121:
					array3[15] = array4[7];
					num = 8;
					break;
				case 430:
					array5[2] = (byte)num6;
					num = 189;
					break;
				case 361:
					array2[17] = 160;
					num = 211;
					break;
				case 253:
					num5 = 207 - 69;
					num4 = 30;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_19c6: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 329;
				case 329:
					num6 = 75 + 45;
					num4 = 430;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 160;
				case 160:
				case 169:
					if (num10 >= num11)
					{
						num4 = 170;
						R3TVja2vZlnVEbMYYF();
						if ((int)/*Error near IL_1a00: Stack underflow*/ == 0)
						{
							goto case 28;
						}
					}
					else
					{
						num23 = num10 % num43;
						num4 = 409;
					}
					goto IL_02ef;
				case 28:
					array2[19] = (byte)num5;
					num4 = 60;
					goto IL_02ef;
				case 318:
					array2[26] = 120;
					num = 391;
					break;
				case 336:
					num5 = 91 + 51;
					num = 90;
					break;
				case 381:
					array2[1] = (byte)num5;
					num7 = 216;
					goto IL_02eb;
				case 356:
					if (hIsn23p8h.Length == 0)
					{
						num4 = 194;
						if (0 == 0)
						{
							goto IL_02ef;
						}
						goto case 153;
					}
					goto case 395;
				case 153:
					num6 = 170 - 56;
					num = 424;
					break;
				case 94:
					array5[14] = 134;
					num = 164;
					break;
				case 8:
					num18 = 0;
					num = 204;
					break;
				case 180:
					array2[7] = (byte)num3;
					num4 = 214;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1aed: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 344;
				case 289:
					array5[1] = (byte)num6;
					num4 = 230;
					goto IL_02ef;
				case 6:
					array5[2] = (byte)num8;
					num4 = 241;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1b2f: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 39;
				case 39:
					num5 = 218 - 72;
					num4 = 157;
					goto IL_02ef;
				case 127:
					array5[6] = (byte)num6;
					num4 = 15;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 174;
				case 174:
					array2[19] = 198;
					num = 134;
					break;
				case 101:
					array2[19] = (byte)num5;
					num4 = 236;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 319;
				case 319:
					if (num17 > 0)
					{
						num4 = 392;
						goto IL_02ef;
					}
					goto IL_2284;
				case 178:
					array2[8] = (byte)num5;
					num = 342;
					break;
				case 177:
					num3 = 89 + 75;
					num4 = 439;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 126;
				case 126:
					num3 = 18 + 52;
					num = 180;
					break;
				case 359:
					array2[16] = 130;
					num = 206;
					break;
				case 88:
					num19 = 255u;
					num7 = 182;
					goto IL_02eb;
				case 152:
					num14 = (uint)num21;
					num4 = 371;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 194;
				case 194:
					C0CxYofPdu5pf1Nyrx(j8hgmZJ7n, "\u0096\u009b\u008847s\u0087\u0086\u00951\u008e\u008aj\u008fa\u008aco.\u009d\u009d\u0093\u008c\u0089j\u0088r\u008c\u0091g91\u0090\u009e\u008d\u0090\u0091");
					new BinaryReader((Stream)/*Error near IL_3be3: Stack underflow*/);
					binaryReader = (BinaryReader)/*Error near IL_1c8b: Stack underflow*/;
					num4 = 394;
					goto IL_02ef;
				case 113:
					array2[19] = (byte)num5;
					num4 = 174;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 77;
				case 77:
					num12 = 0u;
					num = 331;
					break;
				case 93:
					array2[15] = (byte)num3;
					num4 = 313;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1ce4: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 160;
				case 305:
					array2[21] = (byte)num5;
					num7 = 116;
					goto IL_02eb;
				case 118:
					array2[11] = 77;
					num7 = 218;
					goto IL_02eb;
				case 187:
					num5 = 145 - 48;
					num7 = 296;
					goto IL_02eb;
				case 237:
					array2[8] = (byte)num3;
					num7 = 171;
					goto IL_02eb;
				case 193:
					num3 = 158 - 52;
					num7 = 285;
					goto IL_02eb;
				case 406:
					array5[5] = 61;
					num4 = 375;
					goto IL_02ef;
				case 317:
					array2[2] = (byte)num3;
					num4 = 123;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 65;
				case 65:
					array5[10] = 143;
					num4 = 352;
					goto IL_02ef;
				case 437:
					new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					cryptoStream = (CryptoStream)/*Error near IL_1dee: Stack underflow*/;
					num7 = 275;
					goto IL_02eb;
				case 79:
					num5 = 136 - 45;
					num = 431;
					break;
				case 63:
					array2[19] = (byte)num3;
					num4 = 14;
					goto IL_02ef;
				case 227:
					array5[6] = 146;
					num4 = 257;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1e55: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 314;
				case 139:
					array2[28] = (byte)num5;
					num = 179;
					break;
				case 349:
					array2[24] = 244;
					num4 = 398;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_1e9a: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 426;
				case 414:
					num20 = 0;
					num4 = 111;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 205;
				case 205:
					array2[10] = (byte)num3;
					num4 = 232;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 348;
				case 348:
					array2[16] = 46;
					num = 385;
					break;
				case 265:
					array2[13] = (byte)num3;
					num4 = 310;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 162;
				case 162:
					array5[14] = (byte)num6;
					num = 441;
					break;
				case 292:
					num12 += num15;
					num = 74;
					break;
				case 64:
					array5[14] = (byte)num6;
					num4 = 147;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 247;
				case 247:
					array3[1] = array4[0];
					num4 = 421;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1f6b: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 42;
				case 42:
					array5[9] = 112;
					num4 = 159;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_1f93: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 43;
				case 320:
					array5[2] = 219;
					num4 = 97;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_1fc0: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 207;
				case 12:
					num3 = 204 - 105;
					num = 201;
					break;
				case 75:
					array2[31] = 204;
					num = 335;
					break;
				case 66:
					array5[0] = 159;
					num7 = 422;
					goto IL_02eb;
				case 256:
					array2[22] = (byte)num3;
					num4 = 245;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_203d: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 281;
				case 3:
					num8 = 59 + 10;
					num7 = 26;
					goto IL_02eb;
				case 209:
					num5 = 249 - 83;
					num4 = 197;
					goto IL_02ef;
				case 383:
				{
					uint num26 = num25;
					uint num27 = num25;
					uint num28 = 1788236480u;
					uint num29 = 1641844255u;
					uint num30 = 200510177u;
					uint num31 = num27;
					uint num32 = 93644960u;
					uint num33 = 1017401194u;
					uint num34 = ((num30 << 5) | (num30 >> 27)) ^ num28;
					uint num35 = num34 & 0xFF00FFu;
					num34 &= 0xFF00FF00u;
					num30 = (num34 >> 8) | (num35 << 8);
					uint num36 = ((num32 << 13) | (num32 >> 19)) + num28;
					uint num37 = num36 & 0xF0F0F0Fu;
					num36 &= 0xF0F0F0F0u;
					num32 = (num36 >> 4) | (num37 << 4);
					uint num38 = num28 & 0xF0F0F0Fu;
					uint num39 = num28 & 0xF0F0F0F0u;
					num38 = ((num38 >> 4) | (num39 << 4)) ^ num30;
					num28 = (num28 >> 13) | (num28 << 19);
					uint num40 = num29 & 0xFF00FFu;
					uint num41 = num29 & 0xFF00FF00u;
					num40 = ((num40 >> 8) | (num41 << 8)) + num30;
					num29 = (num29 << 10) | (num29 >> 22);
					ulong num42 = 1478622928 * num30;
					num42 |= 1;
					num33 = (uint)(num33 * num33 % num42);
					num31 ^= num31 << 2;
					num31 += num28;
					num31 ^= num31 << 15;
					num31 += num29;
					num31 ^= num31 >> 9;
					num31 += num33;
					num31 = (((num32 << 6) + num28) ^ num29) - num31;
					num25 = num26 + (uint)(double)num31;
					num4 = 48;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_227a: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 107;
				}
				case 45:
					num5 = 193 - 64;
					num7 = 40;
					goto IL_02eb;
				case 55:
					array2[6] = (byte)num5;
					num = 334;
					break;
				case 279:
					array2[18] = 169;
					num4 = 377;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 108;
				case 108:
					array2[3] = (byte)num3;
					num7 = 51;
					goto IL_02eb;
				case 156:
					array5[0] = 15;
					num4 = 3;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 41;
				case 314:
					array3 = array5;
					num4 = 115;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_2359: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 176;
				case 176:
					array5[6] = (byte)num6;
					num4 = 83;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 179;
				case 179:
					array2[28] = 213;
					num4 = 404;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_239e: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 332;
				case 391:
					array2[26] = 30;
					num7 = 336;
					goto IL_02eb;
				case 306:
					array2[27] = 185;
					num4 = 168;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 304;
				case 304:
					array2[17] = (byte)num5;
					num4 = 361;
					goto IL_02ef;
				case 48:
					num12 = num25;
					num7 = 199;
					goto IL_02eb;
				case 429:
					num5 = 123 - 116;
					num7 = 43;
					goto IL_02eb;
				case 442:
					array2[10] = (byte)num5;
					num = 27;
					break;
				case 263:
					num5 = 1 + 61;
					num7 = 305;
					goto IL_02eb;
				case 310:
					num3 = 26 - 8;
					num4 = 382;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_247c: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 293;
				case 293:
					array8[num21] = (byte)(num22 & 0xFFu);
					num4 = 400;
					goto IL_02ef;
				case 141:
					num6 = 208 - 69;
					num = 364;
					break;
				case 24:
					array2[17] = (byte)num3;
					num = 80;
					break;
				case 433:
					array2[7] = (byte)num5;
					num = 126;
					break;
				case 36:
					array5[14] = (byte)num8;
					num4 = 330;
					goto IL_02ef;
				case 133:
					array2[23] = 60;
					num4 = 268;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2525: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 186;
				case 186:
					array5[9] = 157;
					num7 = 379;
					goto IL_02eb;
				case 119:
					num3 = 65 + 49;
					num4 = 78;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 138;
				case 138:
					array5[9] = (byte)num8;
					num = 309;
					break;
				case 197:
					array2[29] = (byte)num5;
					num4 = 357;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 301;
				case 301:
					num5 = 37 + 16;
					num = 428;
					break;
				case 312:
					num6 = 248 - 82;
					num7 = 190;
					goto IL_02eb;
				case 352:
					num6 = 8 + 75;
					num7 = 238;
					goto IL_02eb;
				case 326:
					array2[0] = (byte)num5;
					num4 = 267;
					goto IL_02ef;
				case 422:
					num8 = 54 + 114;
					num4 = 242;
					goto IL_02ef;
				case 100:
					array5[13] = (byte)num6;
					num = 300;
					break;
				case 2:
					array2[20] = 140;
					num7 = 54;
					goto IL_02eb;
				case 73:
					num8 = 63 + 37;
					num4 = 106;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 294;
				case 294:
					array2[15] = (byte)num3;
					num7 = 281;
					goto IL_02eb;
				case 379:
					num6 = 244 - 81;
					num4 = 396;
					goto IL_02ef;
				case 92:
					num5 = 58 + 72;
					num = 423;
					break;
				case 388:
					array5[1] = (byte)num8;
					num4 = 109;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_26df: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 321;
				case 321:
					num5 = 171 - 57;
					num4 = 304;
					goto IL_02ef;
				case 331:
					num15 = 0u;
					num7 = 415;
					goto IL_02eb;
				case 158:
					array2[3] = (byte)num3;
					num4 = 203;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 240;
				case 240:
					num6 = 85 + 8;
					num = 290;
					break;
				case 208:
					if (array4.Length <= 0)
					{
						goto case 8;
					}
					num4 = 247;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_275c: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 374;
				case 374:
					array2[24] = (byte)num5;
					num4 = 11;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 235;
				case 235:
					num5 = 33 + 31;
					num7 = 381;
					goto IL_02eb;
				case 96:
				case 111:
					if (num20 < num17)
					{
						if (num20 > 0)
						{
							num = 13;
							break;
						}
						goto case 70;
					}
					num7 = 291;
					goto IL_02eb;
				case 188:
					num5 = 129 - 43;
					num7 = 71;
					goto IL_02eb;
				case 173:
					array2[30] = (byte)num5;
					num7 = 87;
					goto IL_02eb;
				case 344:
				case 393:
					num25 = num12;
					num4 = 367;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 224;
				case 224:
					num3 = 233 - 77;
					num = 317;
					break;
				case 298:
					array2[11] = (byte)num3;
					num7 = 118;
					goto IL_02eb;
				case 250:
					num8 = 88 + 0;
					num7 = 137;
					goto IL_02eb;
				case 342:
					num5 = 127 - 42;
					num4 = 91;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 399;
				case 399:
					num3 = 103 + 80;
					num4 = 365;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2873: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 238;
				case 238:
					array5[10] = (byte)num6;
					num4 = 132;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2894: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 410;
				case 51:
					num5 = 157 - 52;
					num4 = 129;
					goto IL_02ef;
				case 299:
					array2[5] = (byte)num3;
					num = 69;
					break;
				case 274:
					num3 = 48 + 81;
					num = 21;
					break;
				case 62:
					array5[11] = (byte)num8;
					num4 = 419;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2908: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 385;
				case 397:
					array5[7] = 170;
					num4 = 401;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_2935: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 242;
				case 242:
					array5[0] = (byte)num8;
					num4 = 156;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2956: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 288;
				case 280:
					num3 = 160 - 53;
					num4 = 420;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 70;
				case 70:
					array8[num21 + num20] = (byte)((num13 & num19) >> num24);
					num7 = 19;
					goto IL_02eb;
				case 128:
					lp5fg4ylTrq5EUjO8G(cryptoStream);
					num7 = 340;
					goto IL_02eb;
				case 1:
					array2[12] = 164;
					num = 426;
					break;
				case 171:
					num5 = 58 + 44;
					num4 = 178;
					goto IL_02ef;
				case 56:
					array2[6] = (byte)num5;
					num = 274;
					break;
				case 269:
					array3[7] = array4[3];
					num4 = 135;
					goto IL_02ef;
				case 221:
					array5[7] = 155;
					num = 240;
					break;
				case 387:
					num3 = 110 + 121;
					num4 = 81;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2a53: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 119;
				case 244:
					num8 = 34 + 9;
					num = 138;
					break;
				case 10:
					num3 = 119 + 29;
					num4 = 175;
					goto IL_02ef;
				case 14:
					num5 = 60 + 59;
					num7 = 101;
					goto IL_02eb;
				case 98:
					array5[6] = 66;
					num = 148;
					break;
				case 143:
					num3 = 112 + 121;
					num = 93;
					break;
				case 425:
					array2[1] = (byte)num5;
					num = 301;
					break;
				case 275:
					ocwDxSUU6Judcd8OTE(cryptoStream, array7, 0, array7.Length);
					num7 = 128;
					goto IL_02eb;
				case 11:
					array2[24] = 158;
					num7 = 349;
					goto IL_02eb;
				case 116:
					num3 = 104 - 16;
					num = 434;
					break;
				case 135:
					array3[9] = array4[4];
					num4 = 286;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 410;
				case 410:
					num3 = 219 - 73;
					num7 = 265;
					goto IL_02eb;
				case 13:
					num19 <<= 8;
					num7 = 82;
					goto IL_02eb;
				case 261:
					num3 = 26 + 56;
					num = 432;
					break;
				case 260:
					array2[7] = 66;
					num4 = 228;
					goto IL_02ef;
				case 86:
					num3 = 107 + 28;
					num7 = 165;
					goto IL_02eb;
				case 30:
					array2[13] = (byte)num5;
					num4 = 353;
					goto IL_02ef;
				case 234:
					array2[23] = (byte)num5;
					num4 = 220;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 243;
				case 243:
					array2[18] = 123;
					num7 = 258;
					goto IL_02eb;
				case 150:
					num6 = 62 - 29;
					num4 = 341;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2c57: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 405;
				case 405:
					num18++;
					num4 = 403;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 59;
				case 59:
					num6 = 190 - 63;
					num4 = 100;
					goto IL_02ef;
				case 226:
					num5 = 74 + 103;
					num7 = 166;
					goto IL_02eb;
				case 40:
					array2[10] = (byte)num5;
					num7 = 130;
					goto IL_02eb;
				case 43:
					array2[20] = (byte)num5;
					num = 202;
					break;
				case 231:
					num5 = 252 - 84;
					num4 = 163;
					goto IL_02ef;
				case 341:
					array5[12] = (byte)num6;
					num4 = 84;
					goto IL_02ef;
				case 395:
					wwniUilX0PGl2lH9FY(hIsn23p8h,  );
					num2 = (int)/*Error near IL_2d18: Stack underflow*/;
					num4 = 444;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_2d2b: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 313;
				case 313:
					num3 = 30 + 66;
					num4 = 294;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 107;
				case 107:
					array2[21] = (byte)num5;
					num4 = 436;
					goto IL_02ef;
				case 419:
					num8 = 99 + 56;
					num = 295;
					break;
				case 71:
					array2[25] = (byte)num5;
					num4 = 443;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_2d9f: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 52;
				case 228:
					num5 = 148 - 49;
					num4 = 433;
					goto IL_02ef;
				case 149:
					array2[14] = (byte)num3;
					num4 = 92;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 58;
				case 58:
					array2[29] = 79;
					num = 144;
					break;
				case 246:
					array2[2] = 120;
					num7 = 224;
					goto IL_02eb;
				case 385:
					num5 = 153 - 51;
					num7 = 49;
					goto IL_02eb;
				case 183:
					array2[15] = (byte)num3;
					num4 = 343;
					goto IL_02ef;
				case 16:
					num3 = 120 + 10;
					num7 = 254;
					goto IL_02eb;
				case 203:
					array2[4] = 165;
					num = 231;
					break;
				case 145:
					array5[3] = (byte)num8;
					num = 288;
					break;
				case 122:
					if (  != -1)
					{
						goto case 259;
					}
					num7 = 368;
					goto IL_02eb;
				case 428:
					array2[1] = (byte)num5;
					num7 = 86;
					goto IL_02eb;
				case 241:
					num8 = 150 - 50;
					goto case 252;
				case 72:
					if (num17 > 0)
					{
						num7 = 181;
						goto IL_02eb;
					}
					goto case 38;
				case 302:
					array5[3] = (byte)num8;
					num4 = 146;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 421;
				case 421:
					array3[3] = array4[1];
					num = 407;
					break;
				case 214:
					array2[7] = 165;
					num4 = 226;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 418;
				case 418:
					array2[16] = 160;
					num = 359;
					break;
				case 7:
					array2[12] = 157;
					num4 = 239;
					goto IL_02ef;
				case 207:
					num3 = 193 - 64;
					num = 219;
					break;
				case 423:
					array2[14] = (byte)num5;
					num7 = 332;
					goto IL_02eb;
				case 375:
					array5[5] = 150;
					num4 = 47;
					goto IL_02ef;
				case 254:
					array2[31] = (byte)num3;
					num4 = 75;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_3005: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 233;
				case 233:
					num5 = 0 + 16;
					num = 56;
					break;
				case 23:
					array7 = hIsn23p8h;
					num7 = 259;
					goto IL_02eb;
				case 199:
					if (num10 != num11 - 1)
					{
						goto IL_0e93;
					}
					num4 = 386;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_3064: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 60;
				case 258:
					num5 = 72 + 108;
					num = 28;
					break;
				case 25:
					num6 = 111 + 37;
					num4 = 283;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_30a4: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 53;
				case 132:
					array5[10] = 225;
					num4 = 222;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 278;
				case 278:
					array5[10] = 89;
					num = 65;
					break;
				case 324:
					num3 = 168 + 59;
					num = 412;
					break;
				case 225:
					num5 = 238 - 79;
					num4 = 326;
					goto IL_02ef;
				case 35:
					array2[26] = 161;
					num7 = 318;
					goto IL_02eb;
				case 339:
					array2[21] = (byte)num5;
					num = 187;
					break;
				case 125:
					num3 = 227 - 75;
					num = 149;
					break;
				case 252:
				case 270:
					array5[2] = (byte)num8;
					num4 = 320;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_3193: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 128;
				case 124:
					array2[9] = 109;
					num7 = 184;
					goto IL_02eb;
				case 74:
					num16 = 0;
					num4 = 440;
					goto IL_02ef;
				case 232:
					array2[10] = 143;
					num4 = 53;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_31f0: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 165;
				case 146:
					array5[3] = 132;
					num4 = 112;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_321d: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 29;
				case 29:
					num5 = 218 + 15;
					num7 = 390;
					goto IL_02eb;
				case 402:
					array5[1] = (byte)num6;
					num4 = 251;
					goto IL_02ef;
				case 267:
					array2[0] = 67;
					num = 140;
					break;
				case 210:
					num6 = 88 + 59;
					num = 297;
					break;
				case 49:
					array2[17] = (byte)num5;
					num = 321;
					break;
				case 76:
					array2 = new byte[32];
					num4 = 264;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 200;
				case 200:
					num15 = (uint)((array6[num14 + 3] << 24) | (array6[num14 + 2] << 16) | (array6[num14 + 1] << 8) | array6[num14]);
					num4 = 88;
					goto IL_02ef;
				case 184:
					num5 = 43 + 41;
					num = 363;
					break;
				case 0:
					num3 = 124 + 57;
					num7 = 408;
					goto IL_02eb;
				case 20:
					num13 = num12 ^ num9;
					num = 414;
					break;
				case 80:
					num3 = 75 - 30;
					num7 = 347;
					goto IL_02eb;
				case 164:
					array5[14] = 143;
					num4 = 366;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_337c: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 84;
				case 84:
					array5[13] = 156;
					num = 59;
					break;
				case 230:
					num8 = 157 - 52;
					num = 388;
					break;
				case 340:
					APVLN68lLNY4HvSeGn(memoryStream);
					hIsn23p8h = (byte[])/*Error near IL_33c0: Stack underflow*/;
					num4 = 172;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_33d3: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 217;
				case 57:
					num5 = 61 + 65;
					num4 = 192;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_341a: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 91;
				case 91:
					array2[8] = (byte)num5;
					num = 212;
					break;
				case 427:
					TSQmGfAxQFBBxCJTWu(binaryReader);
					num7 = 76;
					goto IL_02eb;
				case 262:
					array5[14] = (byte)num8;
					num4 = 167;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 165;
				case 165:
					array2[1] = (byte)num3;
					num7 = 246;
					goto IL_02eb;
				case 417:
					array5[11] = 34;
					num7 = 325;
					goto IL_02eb;
				case 5:
					array5[15] = 110;
					num4 = 161;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_34bf: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 194;
				case 33:
					array2[9] = (byte)num5;
					num4 = 177;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 316;
				case 316:
					num3 = 211 - 87;
					num4 = 315;
					goto IL_02ef;
				case 26:
					array5[1] = (byte)num8;
					num = 333;
					break;
				case 67:
					if (num10 == num11 - 1)
					{
						num = 319;
						break;
					}
					goto IL_2284;
				case 300:
					array5[13] = 159;
					num = 308;
					break;
				case 357:
					array2[29] = 79;
					num4 = 387;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 334;
				case 334:
					array2[7] = 123;
					num4 = 260;
					goto IL_02ef;
				case 315:
					array2[28] = (byte)num3;
					num4 = 58;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_35b2: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 197;
				case 415:
					num9 = 0u;
					num7 = 72;
					goto IL_02eb;
				case 85:
					num3 = 151 - 50;
					num4 = 299;
					goto IL_02ef;
				case 398:
					num5 = 196 - 65;
					num = 46;
					break;
				case 99:
					array2[0] = 90;
					num4 = 225;
					goto IL_02ef;
				case 413:
					num5 = 139 - 66;
					num4 = 55;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_363f: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 426;
				case 426:
					array2[12] = 131;
					num = 7;
					break;
				case 434:
					array2[21] = (byte)num3;
					num = 198;
					break;
				case 52:
					array5[8] = (byte)num6;
					num4 = 210;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_3697: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 256;
				case 346:
					array2[18] = 139;
					num = 279;
					break;
				case 365:
					array2[4] = (byte)num3;
					num = 85;
					break;
				case 47:
					array5[5] = 88;
					num4 = 17;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 322;
				case 322:
					array2[25] = 164;
					num = 102;
					break;
				case 31:
					array5[6] = (byte)num8;
					num4 = 327;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 44;
				case 44:
					num3 = 130 - 43;
					num4 = 108;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 308;
				case 308:
					array5[13] = 199;
					num4 = 94;
					goto IL_02ef;
				case 255:
					array2[14] = (byte)num3;
					num4 = 125;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_3795: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 327;
				case 27:
					array2[11] = 167;
					num4 = 311;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_37c2: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 441;
				case 441:
					array5[15] = 113;
					num7 = 5;
					goto IL_02eb;
				case 355:
					array5[2] = 53;
					num4 = 329;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_3809: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 307;
				case 307:
					array2[24] = (byte)num3;
					num = 37;
					break;
				case 219:
					array2[31] = (byte)num3;
					num4 = 16;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 190;
				case 190:
					array5[12] = (byte)num6;
					num4 = 153;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_385f: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 140;
				case 140:
					array2[1] = 91;
					num7 = 235;
					goto IL_02eb;
				case 424:
					array5[12] = (byte)num6;
					num = 303;
					break;
				case 347:
					array2[17] = (byte)num3;
					num = 346;
					break;
				case 407:
					array3[5] = array4[2];
					num7 = 269;
					goto IL_02eb;
				case 34:
					array2[13] = 110;
					num7 = 410;
					goto IL_02eb;
				case 4:
					array2[18] = (byte)num3;
					num7 = 284;
					goto IL_02eb;
				case 249:
					array2[10] = (byte)num5;
					num4 = 337;
					z5DnODHQiwNxpdaCsc();
					if ((int)/*Error near IL_3918: Stack underflow*/ == 0)
					{
						goto IL_02ef;
					}
					goto case 338;
				case 196:
					num5 = 187 - 62;
					num4 = 277;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_393f: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 1;
				case 167:
					num6 = 96 + 58;
					num7 = 64;
					goto IL_02eb;
				case 129:
					array2[3] = (byte)num5;
					num4 = 376;
					R3TVja2vZlnVEbMYYF();
					if ((int)/*Error near IL_397e: Stack underflow*/ != 0)
					{
						goto IL_02ef;
					}
					goto case 351;
				case 217:
				case 372:
					array2[28] = (byte)num3;
					num4 = 95;
					goto IL_02ef;
				case 32:
					dSQ9sXb6b1U7vMnAyG(symmetricAlgorithm, CipherMode.CBC);
					num = 18;
					break;
				case 444:
					try
					{
						McKvRNormqspo9uTHU();
						byte[] array = hIsn23p8h;
						VXWtrFDX7LCc0BEsLp((object)/*Error near IL_39d9: Stack underflow*/, array,   + 4, num2);
						return (string)/*Error near IL_39e8: Stack underflow*/;
					}
					catch
					{
					}
					return "";
				case 368:
					wwm9YqjHMAOjE6JME7();
					symmetricAlgorithm = (SymmetricAlgorithm)/*Error near IL_1c30: Stack underflow*/;
					num = 32;
					break;
				case 41:
					{
						new MemoryStream();
						memoryStream = (MemoryStream)/*Error near IL_2332: Stack underflow*/;
						num4 = 437;
						goto IL_02ef;
					}
					IL_2284:
					num12 += num15;
					num4 = 152;
					if (0 == 0)
					{
						goto IL_02ef;
					}
					goto case 45;
					IL_02eb:
					num4 = num7;
					goto IL_02ef;
					IL_0e93:
					num22 = num12 ^ num9;
					num4 = 293;
					if (true)
					{
						goto IL_02ef;
					}
					goto case 366;
					IL_02ef:
					num = num4;
					break;
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[AXBrnIFfMAfABnJrF9(typeof(AXBrnIFfMAfABnJrF9.z0oyxsqySXMDuI4ZyY<object>[]))]
		internal static string pvQ2Nvbv9(string  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			"{11111-22222-50001-00000}".Trim();
			byte[] array = Convert.FromBase64String( );
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		[DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory")]
		private static extern void KqVWF2r0M(IntPtr  , uint  );

		[DllImport("kernel32.dll", EntryPoint = "VirtualProtect", SetLastError = true)]
		private static extern int SR2f8Si0X(ref IntPtr  , int  , int  , ref int  );

		[DllImport("kernel32.dll", EntryPoint = "FindResource")]
		public static extern IntPtr LXFsnj021(IntPtr  , string  , uint  );

		[DllImport("kernel32.dll", EntryPoint = "VirtualAlloc", SetLastError = true)]
		private static extern IntPtr jMyYFyWuy(IntPtr  , uint  , uint  , uint  );

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static uint NvQ34uZt895nxEhi2FIr(IntPtr  , IntPtr  , IntPtr  , [MarshalAs(UnmanagedType.U4)] uint  , IntPtr  , ref uint  )
		{
			IntPtr ptr =  ;
			if (NrL10qsNW)
			{
				ptr =  ;
			}
			long num = 0L;
			num = ((IntPtr.Size != 4) ? Marshal.ReadInt64(ptr, IntPtr.Size * 2) : Marshal.ReadInt32(ptr, IntPtr.Size * 2));
			object obj = IBe4hEip2A[num];
			if (obj != null)
			{
				Lk7BwHKFmNJY32ZC3n lk7BwHKFmNJY32ZC3n = (Lk7BwHKFmNJY32ZC3n)obj;
				IntPtr intPtr = Marshal.AllocCoTaskMem(lk7BwHKFmNJY32ZC3n.Uu349Vtr47.Length);
				Marshal.Copy(lk7BwHKFmNJY32ZC3n.Uu349Vtr47, 0, intPtr, lk7BwHKFmNJY32ZC3n.Uu349Vtr47.Length);
				if (lk7BwHKFmNJY32ZC3n.bV44XU8KQo)
				{
					  = intPtr;
					  = (uint)lk7BwHKFmNJY32ZC3n.Uu349Vtr47.Length;
					yMayDYsjD( , lk7BwHKFmNJY32ZC3n.Uu349Vtr47.Length, 64, ref WS94a0Vnlv);
					return 0u;
				}
				Marshal.WriteIntPtr(ptr, IntPtr.Size * 2, intPtr);
				Marshal.WriteInt32(ptr, IntPtr.Size * 3, lk7BwHKFmNJY32ZC3n.Uu349Vtr47.Length);
				uint result = 0u;
				if (  != 216669565 || firstrundone)
				{
					result = bFB44BUGlg( ,  ,  ,  ,  , ref  );
				}
				else
				{
					firstrundone = true;
				}
				return result;
			}
			return bFB44BUGlg( ,  ,  ,  ,  , ref  );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static int gVU0QeojF()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return 5;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void HK2JaffxR()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			try
			{
				RSACryptoServiceProvider.UseMachineKeyStore = true;
			}
			catch
			{
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Delegate ubITRqgdO(IntPtr  , Type  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return (Delegate)Type.GetTypeFromHandle(KKr6hZkjvwWjdm9A4Z.Q\u0090r\u000d\u000a\u00966\u009dy\u0086\u009aof\u0092\u0098(16777310)).GetMethod("GetDelegateForFunctionPointer", new Type[2]
			{
				Type.GetTypeFromHandle(KKr6hZkjvwWjdm9A4Z.Q\u0090r\u000d\u000a\u00966\u009dy\u0086\u009aof\u0092\u0098(16777284)),
				Type.GetTypeFromHandle(KKr6hZkjvwWjdm9A4Z.Q\u0090r\u000d\u000a\u00966\u009dy\u0086\u009aof\u0092\u0098(16777296))
			}).Invoke(null, new object[2] {  ,   });
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal unsafe static void vEB6drODu()
		{
			//Discarded unreachable code: IL_29b6, IL_2ad6, IL_3570, IL_3aeb, IL_4a5c, IL_5614, IL_5640, IL_5651, IL_56de, IL_5706, IL_5753, IL_5782, IL_57b1, IL_57c5, IL_57cf, IL_5841, IL_584b, IL_590e, IL_5944, IL_594e, IL_5958, IL_5987, IL_59e9, IL_5a11, IL_5a7c, IL_5ad7, IL_5ae1, IL_5b1a, IL_5b56, IL_5b67, IL_5ba0, IL_5bcf, IL_5bed, IL_5c41, IL_5c9f, IL_5d2c, IL_5d36, IL_5d4a, IL_5d98, IL_5da2, IL_5dac, IL_5e0e, IL_5e18, IL_5e36, IL_5e54, IL_5e68, IL_5e9a, IL_5ed3, IL_5f67, IL_5fbc, IL_5fd0, IL_6034, IL_6048, IL_6052, IL_6057
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected I4, but got Unknown
			//IL_006a: Incompatible stack heights: 1 vs 0
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Expected I4, but got Unknown
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Expected I4, but got Unknown
			//IL_02af: Incompatible stack heights: 1 vs 0
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Expected I4, but got Unknown
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Expected I4, but got Unknown
			//IL_0572: Incompatible stack heights: 1 vs 0
			//IL_0764: Expected O, but got Unknown
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Expected I4, but got Unknown
			//IL_0926: Unknown result type (might be due to invalid IL or missing references)
			//IL_0928: Expected I4, but got Unknown
			//IL_0992: Incompatible stack heights: 1 vs 0
			//IL_0a43: Incompatible stack heights: 1 vs 0
			//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa9: Expected I4, but got Unknown
			//IL_0ac3: Incompatible stack heights: 1 vs 0
			//IL_0b83: Incompatible stack heights: 1 vs 0
			//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9d: Expected I4, but got Unknown
			//IL_0c53: Incompatible stack heights: 1 vs 0
			//IL_0ca5: Incompatible stack heights: 1 vs 0
			//IL_0f66: Incompatible stack heights: 1 vs 0
			//IL_0ff2: Incompatible stack heights: 1 vs 0
			//IL_1085: Unknown result type (might be due to invalid IL or missing references)
			//IL_108b: Expected I8, but got Unknown
			//IL_122b: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b6: Expected I4, but got Unknown
			//IL_1323: Unknown result type (might be due to invalid IL or missing references)
			//IL_1325: Expected I4, but got Unknown
			//IL_1380: Incompatible stack heights: 1 vs 0
			//IL_142f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1434: Expected I4, but got Unknown
			//IL_149b: Unknown result type (might be due to invalid IL or missing references)
			//IL_149d: Expected I4, but got Unknown
			//IL_14f3: Incompatible stack heights: 1 vs 0
			//IL_180e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1813: Expected I4, but got Unknown
			//IL_190b: Incompatible stack heights: 1 vs 0
			//IL_1923: Incompatible stack heights: 1 vs 0
			//IL_197b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ae9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1af1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1af6: Expected I4, but got Unknown
			//IL_1b9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c42: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d62: Incompatible stack heights: 1 vs 0
			//IL_1ec6: Incompatible stack heights: 1 vs 0
			//IL_1f6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f72: Expected I4, but got Unknown
			//IL_202c: Unknown result type (might be due to invalid IL or missing references)
			//IL_202f: Expected I4, but got Unknown
			//IL_2086: Unknown result type (might be due to invalid IL or missing references)
			//IL_2088: Expected I4, but got Unknown
			//IL_215d: Unknown result type (might be due to invalid IL or missing references)
			//IL_2160: Expected I4, but got Unknown
			//IL_21be: Unknown result type (might be due to invalid IL or missing references)
			//IL_21c3: Expected I4, but got Unknown
			//IL_22ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_2301: Expected I8, but got Unknown
			//IL_2324: Incompatible stack heights: 1 vs 0
			//IL_2536: Unknown result type (might be due to invalid IL or missing references)
			//IL_253b: Expected I4, but got Unknown
			//IL_260b: Unknown result type (might be due to invalid IL or missing references)
			//IL_260e: Expected I4, but got Unknown
			//IL_263b: Unknown result type (might be due to invalid IL or missing references)
			//IL_263d: Expected I4, but got Unknown
			//IL_276e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2773: Expected I4, but got Unknown
			//IL_285f: Incompatible stack heights: 1 vs 0
			//IL_2b23: Incompatible stack heights: 0 vs 1
			//IL_2ba0: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ba2: Expected I4, but got Unknown
			//IL_2bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_2de2: Unknown result type (might be due to invalid IL or missing references)
			//IL_2de7: Expected I4, but got Unknown
			//IL_2f30: Incompatible stack heights: 1 vs 0
			//IL_30d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_30dd: Expected I4, but got Unknown
			//IL_312a: Unknown result type (might be due to invalid IL or missing references)
			//IL_312c: Expected I4, but got Unknown
			//IL_315c: Unknown result type (might be due to invalid IL or missing references)
			//IL_315e: Expected I4, but got Unknown
			//IL_3217: Invalid comparison between Unknown and I8
			//IL_3304: Unknown result type (might be due to invalid IL or missing references)
			//IL_330a: Expected I8, but got Unknown
			//IL_33df: Incompatible stack heights: 0 vs 1
			//IL_3698: Unknown result type (might be due to invalid IL or missing references)
			//IL_369d: Expected I4, but got Unknown
			//IL_36cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_36d2: Expected I4, but got Unknown
			//IL_38bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_38c2: Expected I4, but got Unknown
			//IL_390a: Incompatible stack heights: 1 vs 0
			//IL_3961: Unknown result type (might be due to invalid IL or missing references)
			//IL_3963: Expected I4, but got Unknown
			//IL_398f: Incompatible stack heights: 1 vs 0
			//IL_39eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_39ee: Expected I4, but got Unknown
			//IL_3a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_3d00: Invalid comparison between Unknown and I4
			//IL_3d72: Unknown result type (might be due to invalid IL or missing references)
			//IL_3d77: Expected I4, but got Unknown
			//IL_3dc8: Incompatible stack heights: 1 vs 0
			//IL_3ee8: Unknown result type (might be due to invalid IL or missing references)
			//IL_3eea: Expected I4, but got Unknown
			//IL_4133: Unknown result type (might be due to invalid IL or missing references)
			//IL_4138: Expected I4, but got Unknown
			//IL_423f: Unknown result type (might be due to invalid IL or missing references)
			//IL_4241: Expected I4, but got Unknown
			//IL_44e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_44e2: Expected I4, but got Unknown
			//IL_45df: Incompatible stack heights: 1 vs 0
			//IL_4612: Incompatible stack heights: 1 vs 0
			//IL_482e: Unknown result type (might be due to invalid IL or missing references)
			//IL_482f: Unknown result type (might be due to invalid IL or missing references)
			//IL_4831: Unknown result type (might be due to invalid IL or missing references)
			//IL_4834: Expected I4, but got Unknown
			//IL_4995: Incompatible stack heights: 1 vs 0
			//IL_5558: Unknown result type (might be due to invalid IL or missing references)
			//IL_555d: Expected I4, but got Unknown
			//IL_55c3: Incompatible stack heights: 0 vs 1
			//IL_55cd: Incompatible stack heights: 0 vs 1
			//IL_55d7: Incompatible stack heights: 0 vs 1
			//IL_55de: Incompatible stack heights: 0 vs 1
			//IL_55e5: Incompatible stack heights: 0 vs 3
			//IL_55ef: Incompatible stack heights: 0 vs 3
			//IL_55f9: Incompatible stack heights: 0 vs 4
			//IL_560a: Incompatible stack heights: 0 vs 1
			//IL_561b: Incompatible stack heights: 0 vs 1
			//IL_5622: Incompatible stack heights: 0 vs 1
			//IL_562c: Incompatible stack heights: 0 vs 1
			//IL_5636: Incompatible stack heights: 0 vs 1
			//IL_565b: Incompatible stack heights: 0 vs 2
			//IL_5665: Incompatible stack heights: 0 vs 1
			//IL_566f: Incompatible stack heights: 0 vs 4
			//IL_5680: Incompatible stack heights: 0 vs 1
			//IL_5687: Incompatible stack heights: 0 vs 1
			//IL_5691: Incompatible stack heights: 0 vs 1
			//IL_569b: Incompatible stack heights: 0 vs 1
			//IL_56a5: Incompatible stack heights: 0 vs 1
			//IL_56af: Incompatible stack heights: 0 vs 1
			//IL_56c3: Incompatible stack heights: 0 vs 1
			//IL_56cd: Incompatible stack heights: 0 vs 1
			//IL_56f2: Incompatible stack heights: 0 vs 1
			//IL_56fc: Incompatible stack heights: 0 vs 4
			//IL_5710: Incompatible stack heights: 0 vs 4
			//IL_571a: Incompatible stack heights: 0 vs 1
			//IL_5724: Incompatible stack heights: 0 vs 1
			//IL_5735: Incompatible stack heights: 0 vs 2
			//IL_573f: Incompatible stack heights: 0 vs 1
			//IL_5749: Incompatible stack heights: 0 vs 1
			//IL_575d: Incompatible stack heights: 0 vs 1
			//IL_5767: Incompatible stack heights: 0 vs 3
			//IL_576e: Incompatible stack heights: 0 vs 3
			//IL_5793: Incompatible stack heights: 0 vs 1
			//IL_579d: Incompatible stack heights: 0 vs 1
			//IL_57a7: Incompatible stack heights: 0 vs 1
			//IL_57d9: Incompatible stack heights: 0 vs 1
			//IL_57e0: Incompatible stack heights: 0 vs 3
			//IL_57f4: Incompatible stack heights: 0 vs 1
			//IL_57fe: Incompatible stack heights: 0 vs 1
			//IL_5808: Incompatible stack heights: 0 vs 1
			//IL_5812: Incompatible stack heights: 0 vs 1
			//IL_5826: Incompatible stack heights: 0 vs 1
			//IL_5830: Incompatible stack heights: 0 vs 1
			//IL_5852: Incompatible stack heights: 0 vs 1
			//IL_5859: Incompatible stack heights: 0 vs 1
			//IL_5874: Incompatible stack heights: 0 vs 1
			//IL_587b: Incompatible stack heights: 0 vs 2
			//IL_5885: Incompatible stack heights: 0 vs 2
			//IL_588f: Incompatible stack heights: 0 vs 2
			//IL_5899: Incompatible stack heights: 0 vs 2
			//IL_58a3: Incompatible stack heights: 0 vs 2
			//IL_58b7: Incompatible stack heights: 0 vs 3
			//IL_58be: Incompatible stack heights: 0 vs 1
			//IL_58c8: Incompatible stack heights: 0 vs 1
			//IL_58d2: Incompatible stack heights: 0 vs 1
			//IL_58dc: Incompatible stack heights: 0 vs 1
			//IL_58e6: Incompatible stack heights: 0 vs 1
			//IL_58f0: Incompatible stack heights: 0 vs 2
			//IL_58fa: Incompatible stack heights: 0 vs 1
			//IL_5904: Incompatible stack heights: 0 vs 1
			//IL_591f: Incompatible stack heights: 0 vs 4
			//IL_5926: Incompatible stack heights: 0 vs 3
			//IL_593a: Incompatible stack heights: 0 vs 1
			//IL_5962: Incompatible stack heights: 0 vs 1
			//IL_596c: Incompatible stack heights: 0 vs 2
			//IL_5976: Incompatible stack heights: 0 vs 2
			//IL_597d: Incompatible stack heights: 0 vs 1
			//IL_5991: Incompatible stack heights: 0 vs 1
			//IL_599b: Incompatible stack heights: 0 vs 2
			//IL_59a5: Incompatible stack heights: 0 vs 1
			//IL_59af: Incompatible stack heights: 0 vs 1
			//IL_59b9: Incompatible stack heights: 0 vs 1
			//IL_59c3: Incompatible stack heights: 0 vs 2
			//IL_59ca: Incompatible stack heights: 0 vs 2
			//IL_59d1: Incompatible stack heights: 0 vs 1
			//IL_59d8: Incompatible stack heights: 0 vs 2
			//IL_59df: Expected I4, but got Unknown
			//IL_59f3: Incompatible stack heights: 0 vs 1
			//IL_59fd: Incompatible stack heights: 0 vs 2
			//IL_5a07: Expected I8, but got Unknown
			//IL_5a2c: Incompatible stack heights: 0 vs 1
			//IL_5a36: Incompatible stack heights: 0 vs 1
			//IL_5a40: Incompatible stack heights: 0 vs 1
			//IL_5a4a: Incompatible stack heights: 0 vs 2
			//IL_5a54: Incompatible stack heights: 0 vs 1
			//IL_5a5e: Incompatible stack heights: 0 vs 1
			//IL_5a68: Incompatible stack heights: 0 vs 1
			//IL_5a90: Incompatible stack heights: 0 vs 1
			//IL_5a97: Incompatible stack heights: 0 vs 1
			//IL_5aa1: Incompatible stack heights: 0 vs 4
			//IL_5aab: Incompatible stack heights: 0 vs 1
			//IL_5ab2: Incompatible stack heights: 0 vs 1
			//IL_5ab9: Incompatible stack heights: 0 vs 1
			//IL_5ac3: Incompatible stack heights: 0 vs 2
			//IL_5acd: Incompatible stack heights: 0 vs 1
			//IL_5aeb: Incompatible stack heights: 0 vs 3
			//IL_5af5: Incompatible stack heights: 0 vs 1
			//IL_5aff: Incompatible stack heights: 0 vs 2
			//IL_5b06: Incompatible stack heights: 0 vs 1
			//IL_5b10: Incompatible stack heights: 0 vs 1
			//IL_5b15: Incompatible stack heights: 1 vs 0
			//IL_5b24: Incompatible stack heights: 0 vs 1
			//IL_5b2e: Incompatible stack heights: 0 vs 2
			//IL_5b38: Incompatible stack heights: 0 vs 1
			//IL_5b42: Incompatible stack heights: 0 vs 1
			//IL_5b4c: Incompatible stack heights: 0 vs 1
			//IL_5b5d: Incompatible stack heights: 0 vs 1
			//IL_5b71: Incompatible stack heights: 0 vs 3
			//IL_5b78: Incompatible stack heights: 0 vs 1
			//IL_5b7d: Invalid comparison between Unknown and I4
			//IL_5baa: Incompatible stack heights: 0 vs 1
			//IL_5bb1: Incompatible stack heights: 0 vs 1
			//IL_5bc5: Incompatible stack heights: 0 vs 1
			//IL_5bd9: Incompatible stack heights: 0 vs 1
			//IL_5be3: Incompatible stack heights: 0 vs 4
			//IL_5bf7: Incompatible stack heights: 0 vs 2
			//IL_5bfe: Incompatible stack heights: 0 vs 2
			//IL_5c08: Incompatible stack heights: 0 vs 1
			//IL_5c0f: Incompatible stack heights: 0 vs 1
			//IL_5c19: Incompatible stack heights: 0 vs 2
			//IL_5c23: Incompatible stack heights: 0 vs 1
			//IL_5c2d: Incompatible stack heights: 0 vs 1
			//IL_5c37: Incompatible stack heights: 0 vs 1
			//IL_5c48: Incompatible stack heights: 0 vs 1
			//IL_5c52: Incompatible stack heights: 0 vs 1
			//IL_5c59: Incompatible stack heights: 0 vs 2
			//IL_5c63: Incompatible stack heights: 0 vs 1
			//IL_5c6d: Incompatible stack heights: 0 vs 1
			//IL_5c77: Incompatible stack heights: 0 vs 4
			//IL_5c81: Incompatible stack heights: 0 vs 4
			//IL_5c95: Incompatible stack heights: 0 vs 2
			//IL_5ca9: Incompatible stack heights: 0 vs 2
			//IL_5cb0: Incompatible stack heights: 0 vs 3
			//IL_5cba: Incompatible stack heights: 0 vs 1
			//IL_5cc4: Incompatible stack heights: 0 vs 1
			//IL_5ccb: Incompatible stack heights: 0 vs 1
			//IL_5cd5: Incompatible stack heights: 0 vs 2
			//IL_5cdc: Incompatible stack heights: 0 vs 4
			//IL_5ce6: Incompatible stack heights: 0 vs 1
			//IL_5cf0: Incompatible stack heights: 0 vs 1
			//IL_5d04: Incompatible stack heights: 0 vs 1
			//IL_5d0e: Incompatible stack heights: 0 vs 2
			//IL_5d18: Incompatible stack heights: 0 vs 1
			//IL_5d40: Incompatible stack heights: 0 vs 1
			//IL_5d54: Incompatible stack heights: 0 vs 2
			//IL_5d5e: Incompatible stack heights: 0 vs 1
			//IL_5d68: Incompatible stack heights: 0 vs 4
			//IL_5d72: Incompatible stack heights: 0 vs 1
			//IL_5d80: Incompatible stack heights: 0 vs 1
			//IL_5d87: Incompatible stack heights: 0 vs 1
			//IL_5d8e: Incompatible stack heights: 0 vs 2
			//IL_5dc0: Incompatible stack heights: 0 vs 1
			//IL_5dd1: Incompatible stack heights: 0 vs 2
			//IL_5ddb: Incompatible stack heights: 0 vs 1
			//IL_5de5: Incompatible stack heights: 0 vs 1
			//IL_5df3: Incompatible stack heights: 0 vs 3
			//IL_5dfd: Incompatible stack heights: 0 vs 4
			//IL_5e2c: Incompatible stack heights: 0 vs 1
			//IL_5e40: Incompatible stack heights: 0 vs 1
			//IL_5e5e: Incompatible stack heights: 0 vs 2
			//IL_5e72: Incompatible stack heights: 0 vs 1
			//IL_5e7c: Incompatible stack heights: 0 vs 2
			//IL_5e86: Incompatible stack heights: 0 vs 1
			//IL_5e90: Incompatible stack heights: 0 vs 1
			//IL_5e95: Incompatible stack heights: 1 vs 0
			//IL_5eab: Incompatible stack heights: 0 vs 1
			//IL_5eb5: Incompatible stack heights: 0 vs 1
			//IL_5ebf: Expected O, but got Unknown
			//IL_5ec9: Incompatible stack heights: 0 vs 1
			//IL_5edd: Incompatible stack heights: 0 vs 1
			//IL_5ee7: Incompatible stack heights: 0 vs 1
			//IL_5ef1: Incompatible stack heights: 0 vs 4
			//IL_5f02: Incompatible stack heights: 0 vs 1
			//IL_5f0c: Incompatible stack heights: 0 vs 1
			//IL_5f16: Incompatible stack heights: 0 vs 1
			//IL_5f20: Incompatible stack heights: 0 vs 1
			//IL_5f3e: Incompatible stack heights: 0 vs 1
			//IL_5f4f: Incompatible stack heights: 0 vs 1
			//IL_5f56: Incompatible stack heights: 0 vs 3
			//IL_5f5d: Incompatible stack heights: 0 vs 1
			//IL_5f7b: Incompatible stack heights: 0 vs 1
			//IL_5f82: Incompatible stack heights: 0 vs 3
			//IL_5f89: Incompatible stack heights: 0 vs 1
			//IL_5f90: Incompatible stack heights: 0 vs 5
			//IL_5f9a: Incompatible stack heights: 0 vs 1
			//IL_5fa1: Incompatible stack heights: 0 vs 1
			//IL_5fb2: Incompatible stack heights: 0 vs 1
			//IL_5fc6: Incompatible stack heights: 0 vs 2
			//IL_5fda: Incompatible stack heights: 0 vs 2
			//IL_5ff8: Incompatible stack heights: 0 vs 1
			//IL_6002: Incompatible stack heights: 0 vs 2
			//IL_600c: Incompatible stack heights: 0 vs 1
			//IL_6016: Incompatible stack heights: 0 vs 1
			//IL_602a: Incompatible stack heights: 0 vs 1
			//IL_603e: Incompatible stack heights: 0 vs 1
			//The blocks IL_3575, IL_357d, IL_35c3, IL_35cc, IL_35d6 are reachable both inside and outside the pinned region starting at IL_35b7. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
			//The blocks IL_000e, IL_001c, IL_0057, IL_0074, IL_0093, IL_009c, IL_00a6, IL_00be, IL_00cc, IL_00eb, IL_010e, IL_011e, IL_0141, IL_0161, IL_017d, IL_0199, IL_01c6, IL_01d4, IL_01f8, IL_0221, IL_023a, IL_0259, IL_0276, IL_02b4, IL_02c4, IL_02d8, IL_030d, IL_0325, IL_0332, IL_0351, IL_036e, IL_038d, IL_03ab, IL_03c9, IL_03f0, IL_0406, IL_041c, IL_0434, IL_044d, IL_046b, IL_0481, IL_0499, IL_04c7, IL_04d7, IL_04f4, IL_0508, IL_053c, IL_0577, IL_0587, IL_05a6, IL_05b1, IL_05c0, IL_05d8, IL_0600, IL_061a, IL_0630, IL_064d, IL_0666, IL_0682, IL_06a3, IL_06c2, IL_06df, IL_0703, IL_071f, IL_073c, IL_0752, IL_0773, IL_0794, IL_07b0, IL_07d8, IL_07fe, IL_080b, IL_0823, IL_083e, IL_0854, IL_0870, IL_0883, IL_089c, IL_08ac, IL_08ca, IL_08ed, IL_0913, IL_0937, IL_0957, IL_097e, IL_0997, IL_09b8, IL_09d4, IL_09fc, IL_0a15, IL_0a38, IL_0a48, IL_0a53, IL_0a6b, IL_0a8a, IL_0a9a, IL_0ac8, IL_0ae1, IL_0af8, IL_0b0d, IL_0b1f, IL_0b2d, IL_0b38, IL_0b47, IL_0b6f, IL_0b88, IL_0ba7, IL_0bba, IL_0bf4, IL_0c12, IL_0c2b, IL_0c44, IL_0c58, IL_0c7b, IL_0c86, IL_0c91, IL_0caa, IL_0cd1, IL_0cf2, IL_0d16, IL_0d2f, IL_0d3a, IL_0d44, IL_0d4f, IL_0d59, IL_0d72, IL_0d82, IL_0d8f, IL_0da4, IL_0dbc, IL_0dcf, IL_0de2, IL_0de9, IL_0e01, IL_0e1a, IL_0e32, IL_0e4b, IL_0e69, IL_0e82, IL_0e97, IL_0eb0, IL_0ec5, IL_0edc, IL_0ef4, IL_0f0c, IL_0f25, IL_0f3e, IL_0f52, IL_0f6b, IL_0f98, IL_0fb0, IL_0fd8, IL_0ff7, IL_101d, IL_102b, IL_1038, IL_105b, IL_1079, IL_10a6, IL_10bd, IL_10db, IL_10f3, IL_1111, IL_1127, IL_1134, IL_1158, IL_1177, IL_1184, IL_11bc, IL_11c9, IL_11de, IL_11fb, IL_120b, IL_122a, IL_1249, IL_1251, IL_1277, IL_12a7, IL_12c9, IL_1310, IL_132f, IL_134c, IL_1378, IL_1385, IL_1396, IL_13ee, IL_1406, IL_143e, IL_145f, IL_1475, IL_1488, IL_14a7, IL_14bf, IL_14e0, IL_14f8, IL_151a, IL_152b, IL_1535, IL_1554, IL_1573, IL_157a, IL_1588, IL_15a1, IL_15b1, IL_15d4, IL_15f1, IL_1606, IL_162a, IL_1646, IL_1652, IL_165c, IL_1673, IL_1686, IL_169e, IL_16c2, IL_16e4, IL_16fd, IL_171b, IL_1756, IL_1775, IL_1799, IL_17b2, IL_17ca, IL_17e6, IL_1804, IL_1822, IL_1835, IL_1867, IL_187a, IL_1883, IL_188d, IL_18ac, IL_18c2, IL_18f8, IL_1910, IL_1941, IL_194c, IL_195a, IL_197a, IL_1990, IL_19ad, IL_1baa, IL_1bc9, IL_1bf2, IL_1c15, IL_1c38, IL_1c56, IL_1c6f, IL_1c7f, IL_1c9a, IL_1cb9, IL_1cd1, IL_1cfe, IL_1d1d, IL_1d28, IL_1d36, IL_1d4e, IL_1d67, IL_1d7f, IL_1da1, IL_1db4, IL_1dc6, IL_1ddf, IL_1df7, IL_1e02, IL_1e0c, IL_1e25, IL_1e49, IL_1e59, IL_1e7c, IL_1eb9, IL_1ecb, IL_1ee8, IL_1f0c, IL_1f1c, IL_1f32, IL_1f4a, IL_1f7c, IL_1f99, IL_1fa4, IL_1fae, IL_1fc6, IL_1fdb, IL_1feb, IL_2003, IL_202b, IL_203d, IL_2052, IL_2073, IL_2092, IL_20ae, IL_20bb, IL_20db, IL_20fa, IL_2113, IL_2128, IL_2131, IL_213b, IL_215c, IL_2174, IL_2180, IL_219c, IL_21b4, IL_21d6, IL_21ee, IL_2207, IL_2219, IL_2236, IL_2244, IL_2267, IL_227f, IL_229c, IL_22ae, IL_22c8, IL_22ed, IL_230b, IL_2329, IL_2336, IL_2366, IL_2393, IL_23ac, IL_23bf, IL_23e2, IL_23fb, IL_2414, IL_2438, IL_2451, IL_2475, IL_2483, IL_24a2, IL_24b7, IL_24c6, IL_24d0, IL_24f1, IL_2509, IL_254a, IL_255f, IL_2577, IL_258f, IL_25a8, IL_25c0, IL_25d3, IL_25ec, IL_2609, IL_2618, IL_2628, IL_264c, IL_2656, IL_266e, IL_2694, IL_26b6, IL_26cf, IL_26ec, IL_26fd, IL_2720, IL_273e, IL_2781, IL_2796, IL_27a4, IL_27ba, IL_27d2, IL_27ef, IL_2808, IL_2818, IL_2830, IL_283b, IL_2845, IL_2864, IL_288c, IL_28a4, IL_28c1, IL_28d9, IL_28f6, IL_2913, IL_2925, IL_2a91, IL_2b7d, IL_2b8d, IL_2bac, IL_2bbe, IL_2be2, IL_2c02, IL_2c1a, IL_2c27, IL_2c30, IL_2c4f, IL_2c63, IL_2c82, IL_2c9a, IL_2caa, IL_2cc7, IL_2ce3, IL_2d02, IL_2d1b, IL_2d25, IL_2d3d, IL_2d55, IL_2d77, IL_2d8d, IL_2da5, IL_2dbd, IL_2df5, IL_2e16, IL_2e26, IL_2e43, IL_2e50, IL_2e69, IL_2e86, IL_2e94, IL_2eb1, IL_2ecb, IL_2ed3, IL_2ee1, IL_2f05, IL_2f22, IL_2f35, IL_2f4a, IL_2f63, IL_2f7b, IL_2f9a, IL_2fb2, IL_2fd4, IL_2fea, IL_301a, IL_3033, IL_304b, IL_3064, IL_307c, IL_309e, IL_30bd, IL_30e7, IL_30f4, IL_3107, IL_3117, IL_313b, IL_3149, IL_3168, IL_317c, IL_31ab, IL_31cb, IL_31e9, IL_31fc, IL_321c, IL_3226, IL_323e, IL_3252, IL_325c, IL_3276, IL_3281, IL_328b, IL_32b8, IL_32ee, IL_3314, IL_332e, IL_334a, IL_3375, IL_3388, IL_33aa, IL_33e4, IL_33f4, IL_361d, IL_3650, IL_3672, IL_36a7, IL_36c3, IL_36e0, IL_36f1, IL_370f, IL_3730, IL_373e, IL_3759, IL_3772, IL_378f, IL_37a5, IL_37c1, IL_37da, IL_37ef, IL_3802, IL_3817, IL_3834, IL_384a, IL_385c, IL_387b, IL_3894, IL_38b3, IL_38d1, IL_38e6, IL_38f7, IL_390f, IL_3922, IL_3935, IL_394e, IL_397b, IL_3994, IL_39b1, IL_39cd, IL_39eb, IL_39ff, IL_3a1e, IL_3b9e, IL_3bbe, IL_3bdd, IL_3bf5, IL_3bfe, IL_3c08, IL_3c20, IL_3c41, IL_3c54, IL_3c71, IL_3c95, IL_3cb0, IL_3cb8, IL_3cc6, IL_3cd7, IL_3cea, IL_3d05, IL_3d13, IL_3d27, IL_3d40, IL_3d68, IL_3d81, IL_3d9c, IL_3db4, IL_3dcd, IL_3dec, IL_3e05, IL_3e1e, IL_3e41, IL_3e55, IL_3e6e, IL_3e87, IL_3e9e, IL_3eb6, IL_3ed5, IL_3ef9, IL_3f18, IL_3f2e, IL_3f3e, IL_3f57, IL_3f7a, IL_3f9b, IL_3fb5, IL_3fc7, IL_3fdc, IL_3ff8, IL_400f, IL_4028, IL_403f, IL_405e, IL_4074, IL_407c, IL_408a, IL_40a9, IL_40e6, IL_40f0, IL_40f9, IL_4103, IL_411c, IL_4129, IL_4142, IL_415b, IL_41c9, IL_41de, IL_41fe, IL_424b, IL_4258, IL_426d, IL_4288, IL_42ac, IL_42cb, IL_42e7, IL_42ff, IL_4314, IL_4333, IL_434d, IL_4386, IL_43a5, IL_43c3, IL_43e6, IL_43ef, IL_43f9, IL_4421, IL_444d, IL_4471, IL_4499, IL_44b1, IL_44cd, IL_44ec, IL_450a, IL_4532, IL_4570, IL_4583, IL_45a0, IL_45b2, IL_45cc, IL_45e4, IL_45fe, IL_4617, IL_462c, IL_464f, IL_4667, IL_4671, IL_468e, IL_4696, IL_46ae, IL_46d4, IL_46f1, IL_4708, IL_473d, IL_475a, IL_4787, IL_47a5, IL_47b8, IL_47d9, IL_480b, IL_4820, IL_483e, IL_4861, IL_4876, IL_4887, IL_489e, IL_48b1, IL_48e6, IL_4924, IL_4936, IL_495a, IL_496b, IL_498d, IL_499a, IL_49bc, IL_49d5, IL_4a09, IL_4a25, IL_4a48, IL_4a61, IL_4a65, IL_4a69, IL_5492, IL_54ae, IL_54b6, IL_54c0, IL_54ff, IL_5517, IL_5536, IL_5567, IL_5580, IL_5599, IL_55a3, IL_55ad, IL_55be, IL_5620, IL_5674, IL_568c, IL_5696, IL_573a, IL_5850, IL_586f, IL_5880, IL_588a, IL_589e, IL_58c3, IL_58cd, IL_58eb, IL_58ff, IL_5967, IL_5996, IL_59be, IL_5a45, IL_5a59, IL_5afa, IL_5b33, IL_5ba5, IL_5c28, IL_5c5e, IL_5c68, IL_5ca4, IL_5cc9, IL_5cd0, IL_5cff, IL_5dbb, IL_5e27, IL_5e3b, IL_5e6d, IL_5e77, IL_5ea9, IL_5f87, IL_5fc1, IL_5fd5, IL_5ffd, IL_6007, IL_6025, IL_6039 are reachable both inside and outside the pinned region starting at IL_02fd. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
			int num = 608;
			byte[] array9 = default(byte[]);
			byte[] array17 = default(byte[]);
			int num5 = default(int);
			byte[] array2 = default(byte[]);
			int num9 = default(int);
			int num12 = default(int);
			int num67 = default(int);
			uint num39 = default(uint);
			uint num50 = default(uint);
			int num32 = default(int);
			byte[] array7 = default(byte[]);
			int num3 = default(int);
			long value = default(long);
			int num6 = default(int);
			int num21 = default(int);
			byte[] array10 = default(byte[]);
			long num8 = default(long);
			byte[] array11 = default(byte[]);
			int num7 = default(int);
			byte[] array16 = default(byte[]);
			byte[] array3 = default(byte[]);
			uint num23 = default(uint);
			int num43 = default(int);
			Lk7BwHKFmNJY32ZC3n lk7BwHKFmNJY32ZC3n = default(Lk7BwHKFmNJY32ZC3n);
			byte[] uu349Vtr = default(byte[]);
			int num54 = default(int);
			IntPtr intPtr8 = default(IntPtr);
			long num16 = default(long);
			IntPtr intPtr9 = default(IntPtr);
			IntPtr intPtr7 = default(IntPtr);
			int num36 = default(int);
			byte[] array12 = default(byte[]);
			IntPtr intPtr3 = default(IntPtr);
			int num17 = default(int);
			int num29 = default(int);
			uint num10 = default(uint);
			byte[] array13 = default(byte[]);
			byte[] array20 = default(byte[]);
			byte[] array = default(byte[]);
			CryptoStream cryptoStream = default(CryptoStream);
			BinaryReader binaryReader = default(BinaryReader);
			int num35 = default(int);
			MemoryStream memoryStream = default(MemoryStream);
			ICryptoTransform transform = default(ICryptoTransform);
			byte[] array14 = default(byte[]);
			byte[] array4 = default(byte[]);
			int num34 = default(int);
			IntPtr intPtr2 = default(IntPtr);
			int num37 = default(int);
			SymmetricAlgorithm symmetricAlgorithm = default(SymmetricAlgorithm);
			bool flag = default(bool);
			Process process = default(Process);
			IntPtr intPtr5 = default(IntPtr);
			int num14 = default(int);
			int num15 = default(int);
			IntPtr intPtr4 = default(IntPtr);
			uint num38 = default(uint);
			int num90 = default(int);
			WDRJe2H6E4HVV6PGZs wDRJe2H6E4HVV6PGZs = default(WDRJe2H6E4HVV6PGZs);
			long num11 = default(long);
			byte[] array15 = default(byte[]);
			byte[] array8 = default(byte[]);
			int num86 = default(int);
			uint num26 = default(uint);
			int num49 = default(int);
			int num41 = default(int);
			IntPtr intPtr10 = default(IntPtr);
			string text = default(string);
			ref byte reference2 = default(ref byte);
			int num48 = default(int);
			int num28 = default(int);
			byte[] array5 = default(byte[]);
			IntPtr intPtr6 = default(IntPtr);
			int num31 = default(int);
			int num42 = default(int);
			Lk7BwHKFmNJY32ZC3n lk7BwHKFmNJY32ZC3n2 = default(Lk7BwHKFmNJY32ZC3n);
			bool bV44XU8KQo = default(bool);
			IEnumerator enumerator = default(IEnumerator);
			uint num13 = default(uint);
			LhmiV9AUoOr1v5yhIs lhmiV9AUoOr1v5yhIs = default(LhmiV9AUoOr1v5yhIs);
			IntPtr intPtr11 = default(IntPtr);
			int num20 = default(int);
			ProcessModuleCollection processModuleCollection = default(ProcessModuleCollection);
			Version version2 = default(Version);
			Version version = default(Version);
			Version version3 = default(Version);
			int num30 = default(int);
			int num27 = default(int);
			ref byte reference = default(ref byte);
			while (true)
			{
				int num2 = num;
				while (true)
				{
					int num4;
					? val25;
					? val26;
					? val17;
					? val13;
					int num65;
					byte num66;
					? val3;
					? val28;
					? val27;
					? val24;
					IntPtr intPtr;
					IntPtr intPtr12;
					int num95;
					int num96;
					? val20;
					byte num93;
					? val19;
					int num91;
					byte num92;
					? val18;
					int num88;
					byte num89;
					long num87;
					? val16;
					? val15;
					byte num85;
					uint num69;
					? val14;
					? val12;
					long num68;
					? val11;
					? val10;
					long num60;
					long num61;
					int num62;
					int num64;
					? val9;
					? val21;
					byte num58;
					int num52;
					byte num53;
					? val8;
					? val7;
					long num51;
					? val6;
					? val5;
					byte num47;
					int num44;
					int num46;
					? val4;
					? val2;
					int num24;
					byte num25;
					byte num22;
					? val22;
					? val23;
					byte num94;
					int num18;
					byte num19;
					byte[] array6;
					object obj;
					object obj2;
					? val;
					byte[] array19;
					int num56;
					int num57;
					switch (num2)
					{
					case 383:
						array9 = array17;
						num4 = 172;
						goto IL_4a61;
					case 492:
						num5 = 114 - 88;
						num = 42;
						break;
					case 510:
						array2[8] = (byte)num9;
						_ = 144;
						goto IL_4a61;
					case 585:
						_ = num12 + num67;
						_ = num39 & num50;
						val25 = /*Error near IL_0086: Stack underflow*/& 0x1F;
						val26 = /*Error near IL_0087: Stack underflow*/>> (int)val25;
						((sbyte[])/*Error near IL_0089: Stack underflow*/)[/*Error near IL_0089: Stack underflow*/] = (sbyte)(byte)val26;
						num2 = 453;
						continue;
					case 518:
					case 597:
						if (num67 >= num32)
						{
							num2 = 181;
							continue;
						}
						if (num67 <= 0)
						{
							goto case 585;
						}
						num = 631;
						YiHWqdAEL2s4JBasBe();
						if ((int)/*Error near IL_46a9: Stack underflow*/ != 0)
						{
							break;
						}
						goto case 606;
					case 431:
						array7[10] = (byte)num3;
						num4 = 160;
						goto IL_4a61;
					case 519:
						value = 0L;
						num4 = 165;
						goto IL_4a61;
					case 155:
						array7[13] = 108;
						num4 = 301;
						goto IL_4a61;
					case 365:
						array7[20] = 104;
						num = 528;
						break;
					case 248:
						TWn4MujlZv = false;
						num2 = 113;
						continue;
					case 456:
						num6 = 23 + 8;
						num = 128;
						if (0 == 0)
						{
							break;
						}
						goto case 67;
					case 53:
						array7[26] = (byte)num3;
						num = 121;
						break;
					case 229:
						array2[9] = (byte)num5;
						num = 166;
						break;
					case 309:
						_ = 15;
						_ = 145;
						val17 = /*Error near IL_01ad: Stack underflow*/- 48;
						((sbyte[])/*Error near IL_01ae: Stack underflow*/)[/*Error near IL_01ae: Stack underflow*/] = (sbyte)(int)val17;
						num = 437;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 29;
					case 616:
						num21 = 9;
						num2 = 89;
						continue;
					case 145:
						_ = 15;
						_ = 232;
						_ = 77;
						val13 = /*Error near IL_01e8: Stack underflow*/- /*Error near IL_01e8: Stack underflow*/;
						((sbyte[])/*Error near IL_01e9: Stack underflow*/)[/*Error near IL_01e9: Stack underflow*/] = (sbyte)(int)val13;
						num = 11;
						if (0 == 0)
						{
							break;
						}
						goto case 39;
					case 39:
						array10 = (byte[])NelyXWhgfYRGJe9y0i(NGHo4rAHSuSQ4ufCkBP(num8));
						num = 67;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 76;
					case 1:
						array11[num7 + 6] = array10[6];
						_ = 600;
						num = (int)/*Error near IL_0235: Stack underflow*/;
						break;
					case 354:
						array7[25] = 109;
						num2 = 237;
						continue;
					case 588:
						array7[4] = (byte)num3;
						num4 = 310;
						goto IL_4a61;
					case 401:
						num65 = num7 + 6;
						num66 = array16[6];
						((sbyte[])/*Error near IL_027f: Stack underflow*/)[num65] = (sbyte)num66;
						num = 543;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 160;
					case 151:
						array3[0] = 99;
						num4 = 601;
						goto IL_4a61;
					case 48:
						num23 = (uint)(num43 * 4);
						num = 282;
						break;
					case 27:
						lk7BwHKFmNJY32ZC3n.Uu349Vtr47 = uu349Vtr;
						num = 101;
						if (0 == 0)
						{
							break;
						}
						goto case 216;
					case 423:
						array7[14] = (byte)num6;
						num2 = 8;
						continue;
					case 476:
						num54 = 0;
						num4 = 260;
						goto IL_4a61;
					case 396:
						array7[17] = 135;
						num4 = 386;
						goto IL_4a61;
					case 166:
						num5 = 23 + 1;
						num = 185;
						break;
					case 140:
						array7[12] = 58;
						num4 = 552;
						goto IL_4a61;
					case 8:
						num3 = 248 - 82;
						num = 236;
						if (true)
						{
							break;
						}
						goto case 363;
					case 363:
						num6 = 72 + 103;
						num = 313;
						if (true)
						{
							break;
						}
						goto case 152;
					case 152:
						num3 = 5 + 74;
						num = 640;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 270;
					case 514:
						intPtr8 = IntPtr.Zero;
						num = 69;
						if (true)
						{
							break;
						}
						goto case 339;
					case 339:
						num16 = intPtr9.ToInt64();
						num2 = 421;
						continue;
					case 254:
						dCWYxTAfxJ0l1weKVN1(new IntPtr(value), intPtr7);
						num2 = 589;
						continue;
					case 552:
						num3 = 174 - 109;
						num4 = 192;
						goto IL_4a61;
					case 103:
						_ = 9;
						_ = 57;
						num6 = /*Error near IL_0458: Stack underflow*/+ /*Error near IL_0458: Stack underflow*/;
						_ = 447;
						num = (int)/*Error near IL_0465: Stack underflow*/;
						if (true)
						{
							break;
						}
						goto case 217;
					case 217:
						num36 = 0;
						num = 231;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 531;
					case 531:
						array7[24] = (byte)num6;
						num4 = 57;
						goto IL_4a61;
					case 71:
						_ = 14;
						_ = 9;
						_ = 19;
						val3 = /*Error near IL_04ad: Stack underflow*/+ /*Error near IL_04ad: Stack underflow*/;
						((sbyte[])/*Error near IL_04ae: Stack underflow*/)[/*Error near IL_04ae: Stack underflow*/] = (sbyte)(int)val3;
						num2 = 195;
						continue;
					case 208:
						num36++;
						num4 = 183;
						goto IL_4a61;
					case 84:
						array12 = (byte[])NelyXWhgfYRGJe9y0i(intPtr3.ToInt32());
						num4 = 39;
						goto IL_4a61;
					case 395:
						num17 = NHZGRo0SaytvR3NXnJ((object)/*Error near IL_04f9: Stack underflow*/) - num29;
						num4 = 553;
						goto IL_4a61;
					case 211:
						num9 = 151 - 76;
						num = 436;
						if (0 == 0)
						{
							break;
						}
						goto case 433;
					case 573:
						num5 = 49 + 4;
						num2 = 450;
						continue;
					case 502:
						num10 <<= 8;
						num2 = 297;
						continue;
					case 333:
						array2[6] = 150;
						num4 = 219;
						goto IL_4a61;
					case 449:
						if (JLGytImuxrMxAZqJKu() == 4)
						{
							num = 356;
							if (true)
							{
								break;
							}
							goto case 416;
						}
						goto case 90;
					case 416:
						array2[0] = (byte)num5;
						num4 = 625;
						goto IL_4a61;
					case 57:
						array7[24] = 91;
						num = 158;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 563;
					case 563:
						array11[num7 + 3] = array12[3];
						num = 583;
						if (true)
						{
							break;
						}
						goto case 104;
					case 104:
						iRLmBjMF2IHavq0sHJ(array13, 0, array13.Length);
						num4 = 620;
						goto IL_4a61;
					case 286:
						array2[4] = (byte)num9;
						_ = 55;
						num = (int)/*Error near IL_0647: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 551;
					case 551:
						num6 = 136 - 45;
						num2 = 83;
						continue;
					case 99:
						array7[31] = (byte)num3;
						num = 193;
						break;
					case 275:
						array2[8] = (byte)num5;
						num = 295;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 109;
					case 109:
						array2[3] = 144;
						num2 = 134;
						continue;
					case 465:
						array7[25] = (byte)num6;
						num = 12;
						if (0 == 0)
						{
							break;
						}
						goto case 261;
					case 261:
						array2[2] = 174;
						num = 573;
						if (true)
						{
							break;
						}
						goto case 124;
					case 124:
					{
						string text2 = (string)eKmFLmAUTIA4RBnjHCA(FBQwxZAAAGt3s3HSZ3l(), array20);
						_ = 2;
						num = (int)/*Error near IL_071a: Stack underflow*/;
						break;
					}
					case 135:
						num3 = 61 + 27;
						num = 24;
						break;
					case 440:
						array3[9] = 100;
						num = 169;
						if (true)
						{
							break;
						}
						goto case 164;
					case 164:
						_ = new byte[40];
						g1cqHkAgBBeTN3XMfvl((object)/*Error near IL_0759: Stack underflow*/, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
						array = (byte[])/*Error near IL_56d4: Stack underflow*/;
						num4 = 137;
						goto IL_4a61;
					case 376:
						cImcIssX0mqXP11GuB(cryptoStream, array17, 0, array17.Length);
						num = 636;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 546;
					case 546:
						uv67RaQSbjWNCiTI1l(QX5YKsJaRCBODNJawM(binaryReader), 0L);
						num = 293;
						break;
					case 386:
						array7[18] = 141;
						num = 479;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 537;
					case 537:
						num35 = NHZGRo0SaytvR3NXnJ(binaryReader);
						num = 574;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 522;
					case 575:
						array2[9] = (byte)num5;
						num4 = 485;
						goto IL_4a61;
					case 390:
						cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
						_ = 376;
						num = (int)/*Error near IL_0838: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 512;
					case 512:
						array13[5] = array14[2];
						num = 206;
						break;
					case 272:
						array4 = (byte[])AJs46ZSqBoMlDC1IIM(memoryStream);
						num = 104;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 70;
					case 70:
						array13[11] = array14[5];
						num4 = 41;
						goto IL_4a61;
					case 253:
						num3 = 56 + 120;
						num4 = 588;
						goto IL_4a61;
					case 43:
						array20[0] = 103;
						num4 = 233;
						goto IL_4a61;
					case 45:
						num5 = 231 - 77;
						num = 280;
						if (true)
						{
							break;
						}
						goto case 191;
					case 191:
						_ = 6;
						_ = 192;
						_ = 64;
						val28 = /*Error near IL_08de: Stack underflow*/- /*Error near IL_08de: Stack underflow*/;
						((sbyte[])/*Error near IL_08df: Stack underflow*/)[/*Error near IL_08df: Stack underflow*/] = (sbyte)(int)val28;
						num = 258;
						break;
					case 441:
						array2[8] = (byte)num9;
						num = 480;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 218;
					case 58:
						_ = 28;
						_ = 132;
						_ = 14;
						val27 = /*Error near IL_0927: Stack underflow*/- /*Error near IL_0927: Stack underflow*/;
						((sbyte[])/*Error near IL_0928: Stack underflow*/)[/*Error near IL_0928: Stack underflow*/] = (sbyte)(int)val27;
						num = 407;
						if (true)
						{
							break;
						}
						goto case 569;
					case 569:
						array16 = (byte[])NelyXWhgfYRGJe9y0i(XtL4lyIIgx.ToInt32());
						num2 = 84;
						continue;
					case 189:
						num6 = 24 + 2;
						_ = 423;
						continue;
					case 450:
						array2[3] = (byte)num5;
						num = 467;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 607;
					case 607:
						array7[24] = (byte)num3;
						num = 126;
						break;
					case 483:
						array7[28] = 174;
						num = 23;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 80;
					case 80:
						num3 = 50 - 9;
						num2 = 442;
						continue;
					case 571:
						O2mGeMA7472VhJ2Eakk(blmQxKAZMFpiBMOawsF(j5I56qAWJhTHkpG09e6(bFB44BUGlg)));
						num = 130;
						if (true)
						{
							break;
						}
						goto case 453;
					case 453:
						num67++;
						_ = 597;
						continue;
					case 526:
						if (JLGytImuxrMxAZqJKu() == 4)
						{
							num = 251;
							if (!SFlWP1U3TGooXTabny())
							{
								break;
							}
							goto case 201;
						}
						goto case 14;
					case 337:
						array7[24] = 127;
						num4 = 484;
						goto IL_4a61;
					case 348:
						num34 = 0;
						num2 = 186;
						continue;
					case 378:
						_ = 124;
						_ = 96;
						num3 = /*Error near IL_0aa5: Stack underflow*/+ /*Error near IL_0aa5: Stack underflow*/;
						num = 431;
						break;
					case 33:
						num5 = 172 - 57;
						num4 = 21;
						goto IL_4a61;
					case 342:
						yMayDYsjD(intPtr2, 4, num37, ref num37);
						num2 = 318;
						continue;
					case 393:
						array3[9] = 108;
						num = 541;
						break;
					case 226:
						Tv7YbOvhTkPsTjfkQK(symmetricAlgorithm, CipherMode.CBC);
						num4 = 132;
						goto IL_4a61;
					case 187:
						num21 = 23;
						num4 = 245;
						goto IL_4a61;
					case 460:
						if (JLGytImuxrMxAZqJKu() != 4)
						{
							array16 = (byte[])HfDHTTAN2uenSb31bPn(XtL4lyIIgx.ToInt64());
							num = 550;
							if (true)
							{
								break;
							}
							goto case 53;
						}
						num = 569;
						if (true)
						{
							break;
						}
						goto case 381;
					case 381:
						array7[5] = 180;
						num = 359;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 32;
					case 32:
						num5 = 23 + 31;
						_ = 593;
						goto IL_4a61;
					case 407:
						_ = 29;
						_ = 142;
						val24 = /*Error near IL_0b9c: Stack underflow*/- 47;
						((sbyte[])/*Error near IL_0b9d: Stack underflow*/)[/*Error near IL_0b9d: Stack underflow*/] = (sbyte)(int)val24;
						num2 = 486;
						continue;
					case 173:
						_ = 4;
						_ = 105;
						((sbyte[])/*Error near IL_0bb0: Stack underflow*/)[/*Error near IL_0bb0: Stack underflow*/] = (sbyte)/*Error near IL_0bb0: Stack underflow*/;
						num4 = 281;
						goto IL_4a61;
					case 604:
						flag = yCGPKt8G6d0By1Tw2I(LXFsnj021(GRSglWdn3wkT4vDfh5(elLAI9D2FYClRc1Rw0(XKH3U5WCf2UyQCIIya())), "__", 10u), IntPtr.Zero);
						num = 443;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 234;
					case 234:
						num9 = 145 - 48;
						num = 494;
						if (true)
						{
							break;
						}
						goto case 17;
					case 17:
						num5 = 214 - 71;
						num4 = 93;
						goto IL_4a61;
					case 525:
						num3 = 231 - 77;
						num4 = 558;
						goto IL_4a61;
					case 30:
					case 641:
						process = (Process)XKH3U5WCf2UyQCIIya();
						_ = 420;
						continue;
					case 127:
						array7[20] = 180;
						num = 635;
						break;
					case 586:
						array3[5] = 116;
						goto case 535;
					case 409:
						num9 = 205 - 68;
						_ = 111;
						goto IL_4a61;
					case 533:
						T7LBbJ4ta(intPtr8, intPtr5, (byte[])NelyXWhgfYRGJe9y0i(NHZGRo0SaytvR3NXnJ(binaryReader)), 4u, out intPtr);
						num = 330;
						break;
					case 83:
						array7[21] = (byte)num6;
						num = 628;
						YiHWqdAEL2s4JBasBe();
						if ((int)/*Error near IL_0cf2: Stack underflow*/ != 0)
						{
							break;
						}
						goto case 480;
					case 480:
						array2[8] = 111;
						num = 86;
						if (true)
						{
							break;
						}
						goto case 35;
					case 35:
						num6 = 57 + 82;
						num4 = 72;
						goto IL_4a61;
					case 539:
						if (num14 != num15 - 1)
						{
							goto IL_3834;
						}
						num4 = 463;
						goto IL_4a61;
					case 606:
						if (JLGytImuxrMxAZqJKu() == 4)
						{
							num4 = 9;
						}
						else
						{
							num8 = iBfhYnAGRj4nG6UCpdH(new IntPtr(value));
							_ = 30;
						}
						goto IL_4a61;
					case 574:
						yMayDYsjD(intPtr4, num35 * 4, 4, ref num37);
						num2 = 348;
						continue;
					case 37:
						array3[4] = 105;
						num4 = 586;
						goto IL_4a61;
					case 277:
						num38 = 0u;
						num4 = 205;
						goto IL_4a61;
					case 501:
						array11[num7 + 1] = array12[1];
						num2 = 91;
						continue;
					case 447:
						array7[5] = (byte)num6;
						num2 = 16;
						continue;
					case 374:
						array11[num7] = array10[0];
						num2 = 634;
						continue;
					case 553:
						NHZGRo0SaytvR3NXnJ(binaryReader);
						num90 = (int)/*Error near IL_0dd8: Stack underflow*/;
						num2 = 489;
						continue;
					case 322:
						if (array14 == null)
						{
							goto case 476;
						}
						num = 639;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 528;
					case 36:
						_ = 0;
						intPtr12 = (nint)((Array)/*Error near IL_0e0a: Stack underflow*/).LongLength;
						iRLmBjMF2IHavq0sHJ((object)/*Error near IL_57ea: Stack underflow*/, (int)/*Error near IL_57ea: Stack underflow*/, (int)(nint)intPtr12);
						num4 = 476;
						goto IL_4a61;
					case 638:
						array7[4] = (byte)num3;
						num4 = 103;
						goto IL_4a61;
					case 210:
						array3[7] = 116;
						_ = 271;
						num = (int)/*Error near IL_0e41: Stack underflow*/;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 580;
					case 580:
						intPtr8 = Kxm8CyXvJ((uint)wDRJe2H6E4HVV6PGZs, 1, (uint)TMSAZx51W3OgMOtnPJ(XKH3U5WCf2UyQCIIya()));
						num4 = 526;
						goto IL_4a61;
					case 323:
						OKoHdvTcfykgJd9bmk(new IntPtr(&num11), 0);
						num2 = 344;
						continue;
					case 454:
						array11[num7 + 4] = array16[4];
						num4 = 107;
						goto IL_4a61;
					case 74:
						num3 = 181 - 60;
						num4 = 361;
						goto IL_4a61;
					case 256:
						array11[num21 + 1] = array16[1];
						num2 = 64;
						continue;
					case 132:
						transform = (ICryptoTransform)pagHAVKrD0QwEqDWBc(symmetricAlgorithm, array15, array13);
						num2 = 578;
						continue;
					case 192:
						array7[12] = (byte)num3;
						num2 = 102;
						continue;
					case 603:
						uv67RaQSbjWNCiTI1l(QX5YKsJaRCBODNJawM(binaryReader), 0L);
						num2 = 224;
						continue;
					case 18:
						num6 = 201 - 67;
						num4 = 250;
						goto IL_4a61;
					case 314:
						num5 = 112 + 24;
						num4 = 125;
						goto IL_4a61;
					case 458:
						array3[7] = 100;
						num = 7;
						break;
					case 3:
						num6 = 125 - 41;
						_ = 136;
						goto IL_4a61;
					case 96:
						array7[30] = 143;
						num = 451;
						SFlWP1U3TGooXTabny();
						if ((int)/*Error near IL_0f93: Stack underflow*/ == 0)
						{
							break;
						}
						goto case 106;
					case 72:
						array7[15] = (byte)num6;
						num4 = 432;
						goto IL_4a61;
					case 106:
						array7[14] = 113;
						num = 189;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 243;
					case 243:
						array7[7] = 94;
						_ = 326;
						continue;
					case 20:
						array7[4] = (byte)num6;
						num = 444;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 363;
					case 133:
						num8 = 0L;
						num2 = 606;
						continue;
					case 598:
						array12 = null;
						num4 = 119;
						goto IL_4a61;
					case 88:
						array7[31] = 114;
						num = 522;
						break;
					case 345:
						array8 = new byte[array9.Length];
						num = 47;
						break;
					case 392:
						num86 = array4.Length / 8;
						num = 629;
						if (true)
						{
							break;
						}
						goto case 522;
					case 522:
						num3 = 128 - 42;
						num = 99;
						if (true)
						{
							break;
						}
						goto case 255;
					case 255:
						array7[15] = (byte)num3;
						num2 = 35;
						continue;
					case 75:
						num6 = 118 + 61;
						num = 472;
						if (true)
						{
							break;
						}
						goto case 587;
					case 587:
						array10 = (byte[])HfDHTTAN2uenSb31bPn(num8);
						num2 = 154;
						continue;
					case 302:
						num29 = 0;
						num4 = 379;
						goto IL_4a61;
					case 602:
						array7[0] = 85;
						num = 341;
						if (true)
						{
							break;
						}
						goto case 269;
					case 269:
						array7[8] = 11;
						num2 = 317;
						continue;
					case 119:
						array10 = null;
						num2 = 257;
						continue;
					case 156:
						array2[2] = 166;
						num = 292;
						break;
					case 205:
						num26 = 0u;
						num2 = 567;
						continue;
					case 181:
					case 220:
						num14++;
						num2 = 44;
						continue;
					case 426:
						num95 = num35 * 4;
						num96 = num37;
						yMayDYsjD((IntPtr)/*Error near IL_11eb: Stack underflow*/, num95, num96, ref num37);
						num = 182;
						if (0 == 0)
						{
							break;
						}
						goto case 216;
					case 216:
					case 312:
						num49 = 0;
						num4 = 188;
						goto IL_4a61;
					case 430:
					case 474:
						if (num41 >= num32)
						{
							_ = 328;
							num = (int)/*Error near IL_121d: Stack underflow*/;
							if (true)
							{
								break;
							}
							goto case 504;
						}
						if (num41 > 0)
						{
							num = 502;
							break;
						}
						goto case 297;
					case 504:
						val20 = /*Error near IL_122c: Stack underflow*/+ 3;
						num93 = array16[3];
						((sbyte[])/*Error near IL_1231: Stack underflow*/)[val20] = (sbyte)num93;
						num = 454;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 182;
					case 26:
						if (num32 <= 0)
						{
							goto case 131;
						}
						num4 = 73;
						goto IL_4a61;
					case 169:
						array3[10] = 108;
						num2 = 194;
						continue;
					case 144:
						_ = 40;
						_ = 38;
						num5 = /*Error near IL_12b2: Stack underflow*/+ /*Error near IL_12b2: Stack underflow*/;
						num = 5;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 455;
					case 455:
						array7[3] = 143;
						num4 = 371;
						goto IL_4a61;
					case 294:
						_ = 8;
						_ = 99;
						val19 = /*Error near IL_1324: Stack underflow*/+ 87;
						((sbyte[])/*Error near IL_1325: Stack underflow*/)[/*Error near IL_1325: Stack underflow*/] = (sbyte)(int)val19;
						num2 = 435;
						continue;
					case 125:
						array2[14] = (byte)num5;
						num4 = 33;
						goto IL_4a61;
					case 555:
						num91 = num7 + 1;
						num92 = array16[1];
						((sbyte[])/*Error near IL_1355: Stack underflow*/)[num91] = (sbyte)num92;
						num2 = 351;
						continue;
					case 421:
						num37 = 0;
						_ = 302;
						continue;
					case 120:
						num10 = 0u;
						_ = 368;
						num = (int)/*Error near IL_1391: Stack underflow*/;
						break;
					case 251:
						intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(j8hgmZJ7n))[0]);
						num = 412;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 498;
					case 289:
						array7[12] = (byte)num6;
						num2 = 140;
						continue;
					case 635:
						array7[21] = 136;
						num4 = 402;
						goto IL_4a61;
					case 24:
						array7[27] = (byte)num3;
						num = 321;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 570;
					case 570:
						i244bikuos = intPtr9.ToInt64();
						num4 = 273;
						goto IL_4a61;
					case 631:
						num50 <<= 8;
						num2 = 637;
						continue;
					case 544:
						_ = 10;
						_ = 39;
						_ = 22;
						val18 = /*Error near IL_149c: Stack underflow*/- /*Error near IL_149c: Stack underflow*/;
						((sbyte[])/*Error near IL_149d: Stack underflow*/)[/*Error near IL_149d: Stack underflow*/] = (sbyte)(int)val18;
						num2 = 3;
						continue;
					case 233:
						_ = 1;
						_ = 101;
						((sbyte[])/*Error near IL_14b0: Stack underflow*/)[/*Error near IL_14b0: Stack underflow*/] = (sbyte)/*Error near IL_14b0: Stack underflow*/;
						num = 56;
						if (true)
						{
							break;
						}
						goto case 112;
					case 112:
						array7[15] = (byte)num6;
						num = 249;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 618;
					case 618:
						array2[5] = (byte)num9;
						_ = 561;
						continue;
					case 310:
						num6 = 114 + 7;
						num = 20;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 464;
					case 464:
						if (thBMZkAqCqa73KF6ess(intPtr10, IntPtr.Zero))
						{
							num2 = 343;
							continue;
						}
						goto case 623;
					case 352:
						array7[30] = 131;
						num4 = 96;
						goto IL_4a61;
					case 184:
						array7[30] = 163;
						num4 = 88;
						goto IL_4a61;
					case 443:
						if (!flag)
						{
							num = 143;
							break;
						}
						goto IL_1941;
					case 110:
						num9 = 15 + 76;
						num2 = 632;
						continue;
					case 276:
						array3[3] = 106;
						num4 = 37;
						goto IL_4a61;
					case 61:
						array7[23] = 188;
						num = 336;
						break;
					case 201:
						array7[7] = (byte)num6;
						num = 25;
						if (true)
						{
							break;
						}
						goto case 288;
					case 288:
						array3[1] = 115;
						num = 214;
						if (true)
						{
							break;
						}
						goto case 298;
					case 298:
						array2[1] = 98;
						num = 609;
						if (true)
						{
							break;
						}
						goto case 65;
					case 65:
						array7[23] = (byte)num6;
						num = 241;
						break;
					case 577:
						if (num90 < 1879048192)
						{
							goto case 433;
						}
						num2 = 228;
						continue;
					case 87:
						array13[9] = array14[4];
						num = 70;
						break;
					case 150:
						array11[num21] = array10[0];
						num4 = 66;
						goto IL_4a61;
					case 498:
						array2[10] = (byte)num5;
						num4 = 521;
						goto IL_4a61;
					case 377:
						array7[3] = 93;
						num = 622;
						if (0 == 0)
						{
							break;
						}
						goto case 123;
					case 123:
						num6 = 99 + 77;
						num = 538;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 285;
					case 285:
						num3 = 145 - 48;
						num2 = 239;
						continue;
					case 468:
						hSjGubHK9 = true;
						num = 338;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 490;
					case 439:
						array2[11] = (byte)num9;
						_ = 406;
						num = (int)/*Error near IL_1732: Stack underflow*/;
						break;
					case 362:
						array7[19] = 95;
						num4 = 75;
						goto IL_4a61;
					case 428:
						array2[12] = 163;
						num = 45;
						if (true)
						{
							break;
						}
						goto case 398;
					case 398:
						num9 = 178 - 56;
						num2 = 247;
						continue;
					case 222:
						array7[25] = (byte)num3;
						num2 = 157;
						continue;
					case 408:
						array7[11] = (byte)num6;
						num = 419;
						break;
					case 149:
						array11[num21 + 1] = array12[1];
						num = 197;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 500;
					case 500:
						_ = 151;
						_ = 50;
						num3 = /*Error near IL_180f: Stack underflow*/- /*Error near IL_180f: Stack underflow*/;
						num = 267;
						if (true)
						{
							break;
						}
						goto case 353;
					case 353:
						intPtr10 = puGi6bKKk(text);
						num4 = 464;
						goto IL_4a61;
					case 400:
						array7[17] = 122;
						num = 380;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 318;
					case 66:
						num88 = num21 + 1;
						num89 = array10[1];
						((sbyte[])/*Error near IL_1870: Stack underflow*/)[num88] = (sbyte)num89;
						num2 = 305;
						continue;
					case 188:
					case 466:
						if (num49 >= num86)
						{
							num2 = 162;
							continue;
						}
						_ = *(long*)((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference2) + (nint)num49 * (nint)8);
						_ = 291793153;
						num87 = (long)/*Error near IL_22ff: Stack underflow*/;
						val16 = /*Error near IL_2300: Stack underflow*/^ num87;
						*(long*)(nint)/*Error near IL_2301: Stack underflow*/ = (long)val16;
						num2 = 497;
						continue;
					case 625:
						array2[0] = 81;
						num2 = 370;
						continue;
					case 578:
						iRLmBjMF2IHavq0sHJ(array15, 0, array15.Length);
						num2 = 373;
						continue;
					case 52:
						array11[num7 + 5] = array12[5];
						_ = 422;
						num = (int)/*Error near IL_18d6: Stack underflow*/;
						break;
					case 532:
						array7[24] = (byte)num6;
						_ = 337;
						continue;
					case 232:
						new MemoryStream(array4);
						binaryReader = new BinaryReader((Stream)/*Error near IL_191c: Stack underflow*/);
						_ = 603;
						continue;
					case 143:
						return;
					case 521:
						num9 = 17 + 24;
						num4 = 626;
						goto IL_4a61;
					case 305:
						val15 = /*Error near IL_197c: Stack underflow*/+ 2;
						num85 = array10[2];
						((sbyte[])/*Error near IL_1981: Stack underflow*/)[val15] = (sbyte)num85;
						num = 319;
						if (0 == 0)
						{
							break;
						}
						goto case 42;
					case 42:
						array2[13] = (byte)num5;
						num4 = 314;
						goto IL_4a61;
					case 19:
					{
						num69 = num38;
						uint num70 = 1788236480u;
						uint num71 = 1641844255u;
						uint num72 = 200510177u;
						uint num73 = num69;
						uint num74 = 93644960u;
						uint num75 = 1017401194u;
						uint num76 = ((num72 << 5) | (num72 >> 27)) ^ num70;
						uint num77 = num76 & 0xFF00FFu;
						num76 &= 0xFF00FF00u;
						num72 = (num76 >> 8) | (num77 << 8);
						uint num78 = ((num74 << 13) | (num74 >> 19)) + num70;
						uint num79 = num78 & 0xF0F0F0Fu;
						num78 &= 0xF0F0F0F0u;
						num74 = (num78 >> 4) | (num79 << 4);
						uint num80 = num70 & 0xF0F0F0Fu;
						uint num81 = num70 & 0xF0F0F0F0u;
						num80 = ((num80 >> 4) | (num81 << 4)) ^ num72;
						num70 = (num70 >> 13) | (num70 << 19);
						uint num82 = num71 & 0xFF00FFu;
						uint num83 = num71 & 0xFF00FF00u;
						num82 = ((num82 >> 8) | (num83 << 8)) + num72;
						_ = 10;
						num71 = (uint)(int)((/*Error near IL_1aea: Stack underflow*/ << (int)/*Error near IL_1aea: Stack underflow*/) | (num71 >> 22));
						ulong num84 = 1478622928 * num72;
						num84 |= 1;
						num75 = (uint)(num75 * num75 % num84);
						num73 ^= num73 << 2;
						num73 += num70;
						num73 ^= num73 << 15;
						num73 += num71;
						num73 ^= num73 >> 9;
						num73 += num75;
						num73 = (((num74 << 6) + num70) ^ num71) - num73;
						num38 = (uint)(int)(/*Error near IL_1b9b: Stack underflow*/ + (uint)(double)num73);
						num2 = 539;
						continue;
					}
					case 559:
						array7[23] = 134;
						num4 = 327;
						goto IL_4a61;
					case 160:
						array7[10] = 137;
						num = 544;
						if (true)
						{
							break;
						}
						goto case 330;
					case 370:
						array2[1] = 156;
						num = 358;
						break;
					case 290:
						_ = ref intPtr4;
						_ = i244bikuos;
						val14 = /*Error near IL_1c43: Stack underflow*/+ num48;
						*(IntPtr*)/*Error near IL_5a07: Stack underflow*/ = new IntPtr((long)val14);
						num = 537;
						break;
					case 344:
						iTtk4eiI7iTGDjhjDS(new IntPtr(&num11), 0);
						num2 = 375;
						continue;
					case 513:
						num41++;
						num4 = 474;
						goto IL_4a61;
					case 0:
						x3c4o2PyTx = NvQ34uZt895nxEhi2FIr;
						num4 = 299;
						goto IL_4a61;
					case 241:
						array7[23] = 90;
						num2 = 617;
						continue;
					case 280:
						array2[12] = (byte)num5;
						num4 = 110;
						goto IL_4a61;
					case 176:
						array7[26] = 99;
						num = 363;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 138;
					case 264:
						num28 = NHZGRo0SaytvR3NXnJ(binaryReader);
						num = 581;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 260;
					case 260:
					case 584:
						if (num54 >= array13.Length)
						{
							num = 383;
							break;
						}
						array15[num54] = (byte)(array15[num54] ^ array13[num54]);
						num2 = 60;
						continue;
					case 111:
						array2[0] = (byte)num9;
						num2 = 141;
						continue;
					case 359:
						num6 = 12 + 86;
						_ = 462;
						goto IL_4a61;
					case 267:
						array7[8] = (byte)num3;
						num4 = 294;
						goto IL_4a61;
					case 375:
						FsHWNgkGiVuOl8OmJc(new IntPtr(&num11), 0, IntPtr.Zero);
						num = 76;
						if (true)
						{
							break;
						}
						goto case 366;
					case 366:
						array13[15] = array14[7];
						num4 = 36;
						goto IL_4a61;
					case 389:
						_ = new byte[1];
						array5 = (byte[])/*Error near IL_1dbc: Stack underflow*/;
						num4 = 413;
						goto IL_4a61;
					case 388:
						num9 = 22 + 76;
						num4 = 286;
						goto IL_4a61;
					case 117:
						array2[3] = (byte)num5;
						num4 = 6;
						goto IL_4a61;
					case 529:
						if (num14 != num15 - 1)
						{
							goto IL_2e86;
						}
						num4 = 15;
						goto IL_4a61;
					case 403:
						num3 = 60 + 93;
						num2 = 114;
						continue;
					case 51:
						array2[4] = 162;
						num = 32;
						if (true)
						{
							break;
						}
						goto case 56;
					case 56:
						array20[2] = 116;
						num2 = 470;
						continue;
					case 346:
						array2[7] = 73;
						num = 566;
						break;
					case 63:
						IBe4hEip2A = new Hashtable(NHZGRo0SaytvR3NXnJ(binaryReader) + 1);
						_ = 547;
						num = (int)/*Error near IL_1e98: Stack underflow*/;
						break;
					case 623:
						array20 = new byte[6];
						_ = 43;
						goto IL_4a61;
					case 237:
						num6 = 50 + 106;
						num = 465;
						break;
					case 372:
						array7[29] = 150;
						num = 18;
						if (0 == 0)
						{
							break;
						}
						goto case 271;
					case 271:
						array3[8] = 46;
						num4 = 440;
						goto IL_4a61;
					case 412:
						uS9zmJ6WC = intPtr9.ToInt32();
						num2 = 14;
						continue;
					case 538:
						array7[14] = (byte)num6;
						num2 = 106;
						continue;
					case 304:
						num5 = 92 + 2;
						num4 = 275;
						goto IL_4a61;
					case 614:
						array7[28] = (byte)num6;
						_ = 213;
						num = (int)/*Error near IL_1f93: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 165;
					case 165:
						if (JLGytImuxrMxAZqJKu() == 4)
						{
							num4 = 202;
							goto IL_4a61;
						}
						value = iBfhYnAGRj4nG6UCpdH(intPtr6);
						num2 = 568;
						continue;
					case 329:
						array7[9] = (byte)num3;
						num2 = 548;
						continue;
					case 470:
						array20[3] = 74;
						num = 173;
						if (true)
						{
							break;
						}
						goto case 7;
					case 7:
						array3[8] = 108;
						num4 = 393;
						goto IL_4a61;
					case 284:
						array2[13] = (byte)num5;
						num2 = 457;
						continue;
					case 153:
						array7[8] = 84;
						_ = 269;
						num = (int)/*Error near IL_2021: Stack underflow*/;
						break;
					case 73:
						num15 = /*Error near IL_202d: Stack underflow*/+ 1;
						num = 131;
						break;
					case 223:
						num12 = num14 * 4;
						num = 48;
						if (0 == 0)
						{
							break;
						}
						goto case 252;
					case 252:
						array2[13] = (byte)num9;
						num = 492;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 209;
					case 209:
						_ = 16;
						_ = 137;
						_ = 45;
						val12 = /*Error near IL_2087: Stack underflow*/- /*Error near IL_2087: Stack underflow*/;
						((sbyte[])/*Error near IL_2088: Stack underflow*/)[/*Error near IL_2088: Stack underflow*/] = (sbyte)(int)val12;
						num4 = 198;
						goto IL_4a61;
					case 82:
						array2[15] = (byte)num9;
						num = 391;
						break;
					case 624:
						array = null;
						num4 = 448;
						goto IL_4a61;
					case 113:
						yMayDYsjD(new IntPtr(value), JLGytImuxrMxAZqJKu(), 64, ref num31);
						num4 = 254;
						goto IL_4a61;
					case 270:
						array2[5] = 87;
						num2 = 360;
						continue;
					case 617:
						num6 = 52 - 23;
						num2 = 29;
						continue;
					case 197:
						array11[num21 + 2] = array12[2];
						num4 = 161;
						goto IL_4a61;
					case 183:
					case 231:
						if (num36 >= num28)
						{
							num4 = 438;
						}
						else
						{
							_ = ref intPtr5;
							_ = i244bikuos + NHZGRo0SaytvR3NXnJ(binaryReader);
							num68 = (long)/*Error near IL_3304: Stack underflow*/;
							val11 = /*Error near IL_3305: Stack underflow*/- num68;
							*(IntPtr*)/*Error near IL_330a: Stack underflow*/ = new IntPtr((long)val11);
							num4 = 445;
						}
						goto IL_4a61;
					case 469:
						array7[0] = (byte)num6;
						_ = 602;
						num = (int)/*Error near IL_2152: Stack underflow*/;
						break;
					case 637:
					{
						int num33 = /*Error near IL_215e: Stack underflow*/+ 8;
						num = 585;
						if (true)
						{
							break;
						}
						goto case 22;
					}
					case 22:
					case 328:
						num38 = (uint)/*Error near IL_2176: Stack underflow*/;
						num4 = 19;
						goto IL_4a61;
					case 471:
						array7[11] = (byte)num3;
						num = 525;
						break;
					case 461:
						array2[6] = (byte)num9;
						num4 = 530;
						goto IL_4a61;
					case 295:
						_ = 67;
						_ = 47;
						num9 = /*Error near IL_21bf: Stack underflow*/+ /*Error near IL_21bf: Stack underflow*/;
						_ = 34;
						num = (int)/*Error near IL_21cc: Stack underflow*/;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 459;
					case 459:
						array7[2] = (byte)num6;
						num2 = 100;
						continue;
					case 249:
						num3 = 112 + 118;
						num2 = 591;
						continue;
					case 47:
						num42 = array15.Length / 4;
						num4 = 277;
						goto IL_4a61;
					case 536:
						num6 = 97 + 108;
						num = 627;
						break;
					case 203:
						num7 = 18;
						num2 = 374;
						continue;
					case 399:
						array7[27] = 97;
						num = 325;
						break;
					case 511:
						array7[17] = (byte)num3;
						num4 = 396;
						goto IL_4a61;
					case 361:
						array7[3] = (byte)num3;
						num = 285;
						if (0 == 0)
						{
							break;
						}
						goto case 475;
					case 475:
						num67 = 0;
						num = 518;
						if (0 == 0)
						{
							break;
						}
						goto case 46;
					case 46:
						array11[num21 + 3] = array16[3];
						num = 108;
						if (0 == 0)
						{
							break;
						}
						goto case 589;
					case 589:
						yMayDYsjD(new IntPtr(value), JLGytImuxrMxAZqJKu(), num31, ref num31);
						num2 = 642;
						continue;
					case 118:
						O2mGeMA7472VhJ2Eakk(blmQxKAZMFpiBMOawsF(j5I56qAWJhTHkpG09e6(x3c4o2PyTx)));
						_ = 624;
						goto IL_4a61;
					case 567:
						num10 = 0u;
						num4 = 26;
						goto IL_4a61;
					case 13:
						array2[13] = (byte)num9;
						num2 = 554;
						continue;
					case 221:
						array7[16] = 89;
						num4 = 209;
						goto IL_4a61;
					case 516:
						reference2 = ref *(byte*)null;
						num4 = 216;
						goto IL_4a61;
					case 258:
						num9 = 138 - 46;
						num2 = 367;
						continue;
					case 89:
						array11[num21] = array16[0];
						num4 = 256;
						goto IL_4a61;
					case 451:
						array7[30] = 86;
						num = 350;
						break;
					case 126:
						num6 = 143 - 47;
						num4 = 532;
						goto IL_4a61;
					case 583:
						array11[num7 + 4] = array12[4];
						num = 52;
						break;
					case 349:
						array7[6] = 108;
						num = 560;
						if (true)
						{
							break;
						}
						goto case 485;
					case 485:
						num5 = 89 + 50;
						num2 = 229;
						continue;
					case 560:
						array7[6] = 41;
						num2 = 49;
						continue;
					case 163:
						((sbyte[])/*Error near IL_2479: Stack underflow*/)[2] = 114;
						_ = 276;
						continue;
					case 350:
						array7[30] = 108;
						num2 = 184;
						continue;
					case 199:
						mGMWpQe4ChoBUxrIed(cryptoStream);
						num = 411;
						break;
					case 379:
						lGqEHaRDPgHFpusu4D(j8hgmZJ7n);
						if ((int)/*Error near IL_24c6: Stack underflow*/ != 0)
						{
							num2 = 385;
							continue;
						}
						goto case 335;
					case 244:
						_ = ref lk7BwHKFmNJY32ZC3n2;
						_ = new byte[1];
						array5 = (byte[])/*Error near IL_24da: Stack underflow*/;
						array5[0] = 42;
						((Lk7BwHKFmNJY32ZC3n*)(nint)/*Error near IL_24e7: Stack underflow*/)->Uu349Vtr47 = array5;
						num2 = 473;
						continue;
					case 508:
						array2[12] = (byte)num9;
						num4 = 590;
						goto IL_4a61;
					case 147:
						array7[27] = 151;
						num = 135;
						break;
					case 78:
						array11[num7 + 2] = array10[2];
						num2 = 611;
						continue;
					case 315:
						array7[18] = (byte)num3;
						num4 = 576;
						goto IL_4a61;
					case 387:
						array2[12] = (byte)num5;
						num4 = 94;
						goto IL_4a61;
					case 316:
						num9 = 135 - 45;
						num4 = 242;
						goto IL_4a61;
					case 242:
						array2[4] = (byte)num9;
						num2 = 196;
						continue;
					case 90:
						_ = new byte[12];
						array3 = (byte[])/*Error near IL_25c9: Stack underflow*/;
						num4 = 146;
						goto IL_4a61;
					case 495:
						num5 = 209 - 69;
						num2 = 404;
						continue;
					case 332:
						array7[22] = (byte)num6;
						num4 = 262;
						goto IL_4a61;
					case 225:
						num38 = (uint)(int)(/*Error near IL_260c: Stack underflow*/ + num26);
						num4 = 50;
						goto IL_4a61;
					case 594:
						array15 = array7;
						num2 = 230;
						continue;
					case 565:
						_ = 5;
						_ = 81;
						val10 = /*Error near IL_263c: Stack underflow*/+ 49;
						((sbyte[])/*Error near IL_263d: Stack underflow*/)[/*Error near IL_263d: Stack underflow*/] = (sbyte)(int)val10;
						num4 = 381;
						goto IL_4a61;
					case 415:
						if ((int)/*Error near IL_5b82: Stack underflow*/ == -185339151)
						{
							num = 604;
							if (YiHWqdAEL2s4JBasBe())
							{
								break;
							}
							goto case 443;
						}
						goto IL_1941;
					case 627:
						array7[6] = (byte)num6;
						num = 349;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 190;
					case 452:
						num5 = 122 + 124;
						num = 498;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 86;
					case 86:
						num9 = 170 - 87;
						num4 = 510;
						goto IL_4a61;
					case 527:
						array7[6] = (byte)num6;
						num2 = 243;
						continue;
					case 300:
						array2[2] = 143;
						num = 261;
						break;
					case 62:
						num6 = 184 + 33;
						num = 175;
						if (true)
						{
							break;
						}
						goto case 404;
					case 404:
						array2[7] = (byte)num5;
						num = 346;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 316;
					case 281:
						array20[5] = 116;
						num2 = 124;
						continue;
					case 38:
						((sbyte[])/*Error near IL_279a: Stack underflow*/)[4] = 114;
						num4 = 424;
						goto IL_4a61;
					case 228:
						bV44XU8KQo = true;
						num = 433;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 185;
					case 185:
						array2[10] = (byte)num5;
						num4 = 200;
						goto IL_4a61;
					case 224:
						intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(j8hgmZJ7n))[0]);
						num4 = 339;
						goto IL_4a61;
					case 12:
						num3 = 109 - 11;
						num2 = 222;
						continue;
					case 214:
						array3[2] = 99;
						num4 = 499;
						goto IL_4a61;
					case 593:
						array2[4] = (byte)num5;
						num2 = 316;
						continue;
					case 67:
					case 154:
						if (JLGytImuxrMxAZqJKu() != 4)
						{
							num7 = 2;
							num4 = 446;
						}
						else
						{
							num4 = 616;
						}
						goto IL_4a61;
					case 121:
						array7[26] = 170;
						_ = 176;
						goto IL_4a61;
					case 102:
						array7[13] = 73;
						num = 155;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 4;
					case 4:
						array7[21] = (byte)num3;
						num2 = 62;
						continue;
					case 579:
						num9 = 142 - 47;
						num = 142;
						break;
					case 541:
						text = (string)eKmFLmAUTIA4RBnjHCA(FBQwxZAAAGt3s3HSZ3l(), array3);
						num2 = 274;
						continue;
					case 442:
						array7[22] = (byte)num3;
						num = 559;
						if (true)
						{
							break;
						}
						goto case 472;
					case 472:
						array7[19] = (byte)num6;
						num = 478;
						if (0 == 0)
						{
							break;
						}
						goto case 108;
					case 108:
						num21 = 16;
						_ = 150;
						num = (int)/*Error near IL_2920: Stack underflow*/;
						break;
					case 420:
						try
						{
							ProcessModuleCollection processModuleCollection2 = (ProcessModuleCollection)kxxORYNQpt7JeTNfIx(process);
							enumerator = (IEnumerator)JvuscRYrEHnEfwpXlR(processModuleCollection2);
							try
							{
								while (Prhas5pe6XdsQAESvm(enumerator))
								{
									ProcessModule processModule2 = (ProcessModule)w040Z1ZHbG0XVqZMoU(enumerator);
									YiHWqdAEL2s4JBasBe();
									int num59;
									if (!SFlWP1U3TGooXTabny())
									{
										num59 = 2;
										if (!YiHWqdAEL2s4JBasBe())
										{
											goto IL_2a29;
										}
									}
									else
									{
										num59 = 3;
										if (1 == 0)
										{
											goto IL_299a;
										}
									}
									goto IL_29bf;
									IL_29fc:
									if (!qGsoXYAtg5xUyI3IWm5(gQ3bxJAoGy2rvkCxjGU(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly), null))
									{
										continue;
									}
									num59 = 6;
									goto IL_29bf;
									IL_299a:
									num60 = num8;
									intPtr9 = GRSglWdn3wkT4vDfh5(processModule2);
									if (num60 < intPtr9.ToInt64())
									{
										goto IL_29fc;
									}
									goto IL_2a29;
									IL_2a29:
									num61 = num8;
									intPtr9 = GRSglWdn3wkT4vDfh5(processModule2);
									if (num61 <= intPtr9.ToInt64() + Btnc15ArMdr21Qis9mc(processModule2))
									{
										continue;
									}
									num59 = 4;
									if (1 == 0)
									{
										return;
									}
									goto IL_29bf;
									IL_29bf:
									while (true)
									{
										switch (num59)
										{
										case 0:
										case 2:
											break;
										case 1:
										case 3:
											goto IL_299a;
										default:
											num59 = 5;
											if (!SFlWP1U3TGooXTabny())
											{
												continue;
											}
											goto IL_29fc;
										case 4:
											goto IL_29fc;
										case 5:
											goto IL_2a29;
										case 6:
											return;
										}
										break;
									}
									if (!qNGJ4OfmSpfpcWEKvj(z1aSaf7nHt2GSLVr8b(processModule2), text))
									{
										continue;
									}
									goto IL_299a;
								}
							}
							finally
							{
								if (enumerator is IDisposable disposable5)
								{
									p8SJ0uC03Z63haWUBt(disposable5);
								}
							}
						}
						catch (object obj7)
						{
						}
						try
						{
							ProcessModuleCollection processModuleCollection3 = (ProcessModuleCollection)kxxORYNQpt7JeTNfIx(process);
							enumerator = (IEnumerator)JvuscRYrEHnEfwpXlR(processModuleCollection3);
							try
							{
								while (Prhas5pe6XdsQAESvm(enumerator))
								{
									ProcessModule processModule3 = (ProcessModule)w040Z1ZHbG0XVqZMoU(enumerator);
									num62 = 3;
									while (true)
									{
										int num63;
										switch (num62)
										{
										case 1:
										case 4:
											goto IL_2ac0;
										case 3:
											intPtr9 = GRSglWdn3wkT4vDfh5(processModule3);
											SFlWP1U3TGooXTabny();
											num64 = (num63 = (YiHWqdAEL2s4JBasBe() ? 4 : 0));
											goto IL_2adf;
										case 5:
											goto end_IL_2ae3;
										}
										goto IL_2b33;
										IL_2ac0:
										if (intPtr9.ToInt64() != phV4Uu6SUx)
										{
											goto IL_2b49;
										}
										goto IL_2b33;
										IL_2b33:
										num29 = 0;
										num63 = 5;
										goto IL_2adf;
										IL_2adf:
										num62 = num63;
										continue;
										end_IL_2ae3:
										break;
									}
									break;
									IL_2b49:;
								}
							}
							finally
							{
								if (enumerator is IDisposable disposable6)
								{
									p8SJ0uC03Z63haWUBt(disposable6);
								}
							}
						}
						catch (object obj8)
						{
						}
						bFB44BUGlg = null;
						num4 = 605;
						goto IL_4a61;
					case 358:
						_ = 1;
						_ = 222;
						_ = 74;
						val9 = /*Error near IL_2ba1: Stack underflow*/- /*Error near IL_2ba1: Stack underflow*/;
						((sbyte[])/*Error near IL_2ba2: Stack underflow*/)[/*Error near IL_2ba2: Stack underflow*/] = (sbyte)(int)val9;
						num4 = 298;
						goto IL_4a61;
					case 174:
						lk7BwHKFmNJY32ZC3n = default(Lk7BwHKFmNJY32ZC3n);
						num4 = 27;
						goto IL_4a61;
					case 566:
						array2[7] = 220;
						num = 304;
						if (true)
						{
							break;
						}
						goto case 182;
					case 182:
					case 438:
						IUSsYAngpyKANEnZ1U(QX5YKsJaRCBODNJawM(binaryReader));
						OhZpyDwZxpiyDh5BOJ(QX5YKsJaRCBODNJawM(binaryReader));
						val21 = /*Error near IL_2bfd: Stack underflow*/- 1;
						if (/*Error near IL_2c02: Stack underflow*/ < val21)
						{
							num = 95;
							if (!SFlWP1U3TGooXTabny())
							{
								break;
							}
							goto case 341;
						}
						JkHjxJCFT(intPtr8);
						num2 = 418;
						continue;
					case 257:
						array16 = null;
						num4 = 460;
						goto IL_4a61;
					case 265:
						if (array5.Length != 0)
						{
							while (true)
							{
								IL_02f5:
								fixed (byte* ptr = &array5[0])
								{
									num = 312;
									while (true)
									{
										IL_4a65_2:
										num2 = num;
										while (true)
										{
											switch (num2)
											{
											case 516:
												goto end_IL_4a69;
											case 383:
												array9 = array17;
												num4 = 172;
												goto IL_4a61_2;
											case 492:
												num5 = 114 - 88;
												num = 42;
												break;
											case 510:
												array2[8] = (byte)num9;
												_ = 144;
												goto IL_4a61_2;
											case 585:
												_ = num12 + num67;
												_ = num39 & num50;
												val25 = /*Error near IL_0086: Stack underflow*/& 0x1F;
												val26 = /*Error near IL_0087: Stack underflow*/>> (int)val25;
												((sbyte[])/*Error near IL_0089: Stack underflow*/)[/*Error near IL_0089: Stack underflow*/] = (sbyte)(byte)val26;
												num2 = 453;
												continue;
											case 518:
											case 597:
												if (num67 >= num32)
												{
													num2 = 181;
													continue;
												}
												if (num67 <= 0)
												{
													goto case 585;
												}
												num = 631;
												YiHWqdAEL2s4JBasBe();
												if ((int)/*Error near IL_46a9: Stack underflow*/ != 0)
												{
													break;
												}
												goto case 606;
											case 431:
												array7[10] = (byte)num3;
												num4 = 160;
												goto IL_4a61_2;
											case 519:
												value = 0L;
												num4 = 165;
												goto IL_4a61_2;
											case 155:
												array7[13] = 108;
												num4 = 301;
												goto IL_4a61_2;
											case 365:
												array7[20] = 104;
												num = 528;
												break;
											case 248:
												TWn4MujlZv = false;
												num2 = 113;
												continue;
											case 456:
												num6 = 23 + 8;
												num = 128;
												if (0 == 0)
												{
													break;
												}
												goto case 67;
											case 53:
												array7[26] = (byte)num3;
												num = 121;
												break;
											case 229:
												array2[9] = (byte)num5;
												num = 166;
												break;
											case 309:
												_ = 15;
												_ = 145;
												val17 = /*Error near IL_01ad: Stack underflow*/- 48;
												((sbyte[])/*Error near IL_01ae: Stack underflow*/)[/*Error near IL_01ae: Stack underflow*/] = (sbyte)(int)val17;
												num = 437;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 29;
											case 616:
												num21 = 9;
												num2 = 89;
												continue;
											case 145:
												_ = 15;
												_ = 232;
												_ = 77;
												val13 = /*Error near IL_01e8: Stack underflow*/- /*Error near IL_01e8: Stack underflow*/;
												((sbyte[])/*Error near IL_01e9: Stack underflow*/)[/*Error near IL_01e9: Stack underflow*/] = (sbyte)(int)val13;
												num = 11;
												if (0 == 0)
												{
													break;
												}
												goto case 39;
											case 39:
												array10 = (byte[])NelyXWhgfYRGJe9y0i(NGHo4rAHSuSQ4ufCkBP(num8));
												num = 67;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 76;
											case 1:
												array11[num7 + 6] = array10[6];
												_ = 600;
												num = (int)/*Error near IL_0235: Stack underflow*/;
												break;
											case 354:
												array7[25] = 109;
												num2 = 237;
												continue;
											case 588:
												array7[4] = (byte)num3;
												num4 = 310;
												goto IL_4a61_2;
											case 401:
												num65 = num7 + 6;
												num66 = array16[6];
												((sbyte[])/*Error near IL_027f: Stack underflow*/)[num65] = (sbyte)num66;
												num = 543;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 160;
											case 151:
												array3[0] = 99;
												num4 = 601;
												goto IL_4a61_2;
											case 48:
												num23 = (uint)(num43 * 4);
												num = 282;
												break;
											case 27:
												lk7BwHKFmNJY32ZC3n.Uu349Vtr47 = uu349Vtr;
												num = 101;
												if (0 == 0)
												{
													break;
												}
												goto case 216;
											case 423:
												array7[14] = (byte)num6;
												num2 = 8;
												continue;
											case 476:
												num54 = 0;
												num4 = 260;
												goto IL_4a61_2;
											case 396:
												array7[17] = 135;
												num4 = 386;
												goto IL_4a61_2;
											case 166:
												num5 = 23 + 1;
												num = 185;
												break;
											case 140:
												array7[12] = 58;
												num4 = 552;
												goto IL_4a61_2;
											case 8:
												num3 = 248 - 82;
												num = 236;
												if (true)
												{
													break;
												}
												goto case 363;
											case 363:
												num6 = 72 + 103;
												num = 313;
												if (true)
												{
													break;
												}
												goto case 152;
											case 152:
												num3 = 5 + 74;
												num = 640;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 270;
											case 514:
												intPtr8 = IntPtr.Zero;
												num = 69;
												if (true)
												{
													break;
												}
												goto case 339;
											case 339:
												num16 = intPtr9.ToInt64();
												num2 = 421;
												continue;
											case 254:
												dCWYxTAfxJ0l1weKVN1(new IntPtr(value), intPtr7);
												num2 = 589;
												continue;
											case 552:
												num3 = 174 - 109;
												num4 = 192;
												goto IL_4a61_2;
											case 103:
												_ = 9;
												_ = 57;
												num6 = /*Error near IL_0458: Stack underflow*/+ /*Error near IL_0458: Stack underflow*/;
												_ = 447;
												num = (int)/*Error near IL_0465: Stack underflow*/;
												if (true)
												{
													break;
												}
												goto case 217;
											case 217:
												num36 = 0;
												num = 231;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 531;
											case 531:
												array7[24] = (byte)num6;
												num4 = 57;
												goto IL_4a61_2;
											case 71:
												_ = 14;
												_ = 9;
												_ = 19;
												val3 = /*Error near IL_04ad: Stack underflow*/+ /*Error near IL_04ad: Stack underflow*/;
												((sbyte[])/*Error near IL_04ae: Stack underflow*/)[/*Error near IL_04ae: Stack underflow*/] = (sbyte)(int)val3;
												num2 = 195;
												continue;
											case 208:
												num36++;
												num4 = 183;
												goto IL_4a61_2;
											case 84:
												array12 = (byte[])NelyXWhgfYRGJe9y0i(intPtr3.ToInt32());
												num4 = 39;
												goto IL_4a61_2;
											case 395:
												num17 = NHZGRo0SaytvR3NXnJ((object)/*Error near IL_04f9: Stack underflow*/) - num29;
												num4 = 553;
												goto IL_4a61_2;
											case 211:
												num9 = 151 - 76;
												num = 436;
												if (0 == 0)
												{
													break;
												}
												goto case 433;
											case 573:
												num5 = 49 + 4;
												num2 = 450;
												continue;
											case 502:
												num10 <<= 8;
												num2 = 297;
												continue;
											case 333:
												array2[6] = 150;
												num4 = 219;
												goto IL_4a61_2;
											case 449:
												if (JLGytImuxrMxAZqJKu() == 4)
												{
													num = 356;
													if (true)
													{
														break;
													}
													goto case 416;
												}
												goto case 90;
											case 416:
												array2[0] = (byte)num5;
												num4 = 625;
												goto IL_4a61_2;
											case 57:
												array7[24] = 91;
												num = 158;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 563;
											case 563:
												array11[num7 + 3] = array12[3];
												num = 583;
												if (true)
												{
													break;
												}
												goto case 104;
											case 104:
												iRLmBjMF2IHavq0sHJ(array13, 0, array13.Length);
												num4 = 620;
												goto IL_4a61_2;
											case 286:
												array2[4] = (byte)num9;
												_ = 55;
												num = (int)/*Error near IL_0647: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 551;
											case 551:
												num6 = 136 - 45;
												num2 = 83;
												continue;
											case 99:
												array7[31] = (byte)num3;
												num = 193;
												break;
											case 275:
												array2[8] = (byte)num5;
												num = 295;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 109;
											case 109:
												array2[3] = 144;
												num2 = 134;
												continue;
											case 465:
												array7[25] = (byte)num6;
												num = 12;
												if (0 == 0)
												{
													break;
												}
												goto case 261;
											case 261:
												array2[2] = 174;
												num = 573;
												if (true)
												{
													break;
												}
												goto case 124;
											case 124:
											{
												string text2 = (string)eKmFLmAUTIA4RBnjHCA(FBQwxZAAAGt3s3HSZ3l(), array20);
												_ = 2;
												num = (int)/*Error near IL_071a: Stack underflow*/;
												break;
											}
											case 135:
												num3 = 61 + 27;
												num = 24;
												break;
											case 440:
												array3[9] = 100;
												num = 169;
												if (true)
												{
													break;
												}
												goto case 164;
											case 164:
												_ = new byte[40];
												g1cqHkAgBBeTN3XMfvl((object)/*Error near IL_0759: Stack underflow*/, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
												array = (byte[])/*Error near IL_56d4: Stack underflow*/;
												num4 = 137;
												goto IL_4a61_2;
											case 376:
												cImcIssX0mqXP11GuB(cryptoStream, array17, 0, array17.Length);
												num = 636;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 546;
											case 546:
												uv67RaQSbjWNCiTI1l(QX5YKsJaRCBODNJawM(binaryReader), 0L);
												num = 293;
												break;
											case 386:
												array7[18] = 141;
												num = 479;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 537;
											case 537:
												num35 = NHZGRo0SaytvR3NXnJ(binaryReader);
												num = 574;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 522;
											case 575:
												array2[9] = (byte)num5;
												num4 = 485;
												goto IL_4a61_2;
											case 390:
												cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
												_ = 376;
												num = (int)/*Error near IL_0838: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 512;
											case 512:
												array13[5] = array14[2];
												num = 206;
												break;
											case 272:
												array4 = (byte[])AJs46ZSqBoMlDC1IIM(memoryStream);
												num = 104;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 70;
											case 70:
												array13[11] = array14[5];
												num4 = 41;
												goto IL_4a61_2;
											case 253:
												num3 = 56 + 120;
												num4 = 588;
												goto IL_4a61_2;
											case 43:
												array20[0] = 103;
												num4 = 233;
												goto IL_4a61_2;
											case 45:
												num5 = 231 - 77;
												num = 280;
												if (true)
												{
													break;
												}
												goto case 191;
											case 191:
												_ = 6;
												_ = 192;
												_ = 64;
												val28 = /*Error near IL_08de: Stack underflow*/- /*Error near IL_08de: Stack underflow*/;
												((sbyte[])/*Error near IL_08df: Stack underflow*/)[/*Error near IL_08df: Stack underflow*/] = (sbyte)(int)val28;
												num = 258;
												break;
											case 441:
												array2[8] = (byte)num9;
												num = 480;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 218;
											case 58:
												_ = 28;
												_ = 132;
												_ = 14;
												val27 = /*Error near IL_0927: Stack underflow*/- /*Error near IL_0927: Stack underflow*/;
												((sbyte[])/*Error near IL_0928: Stack underflow*/)[/*Error near IL_0928: Stack underflow*/] = (sbyte)(int)val27;
												num = 407;
												if (true)
												{
													break;
												}
												goto case 569;
											case 569:
												array16 = (byte[])NelyXWhgfYRGJe9y0i(XtL4lyIIgx.ToInt32());
												num2 = 84;
												continue;
											case 189:
												num6 = 24 + 2;
												_ = 423;
												continue;
											case 450:
												array2[3] = (byte)num5;
												num = 467;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 607;
											case 607:
												array7[24] = (byte)num3;
												num = 126;
												break;
											case 483:
												array7[28] = 174;
												num = 23;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 80;
											case 80:
												num3 = 50 - 9;
												num2 = 442;
												continue;
											case 571:
												O2mGeMA7472VhJ2Eakk(blmQxKAZMFpiBMOawsF(j5I56qAWJhTHkpG09e6(bFB44BUGlg)));
												num = 130;
												if (true)
												{
													break;
												}
												goto case 453;
											case 453:
												num67++;
												_ = 597;
												continue;
											case 526:
												if (JLGytImuxrMxAZqJKu() == 4)
												{
													num = 251;
													if (!SFlWP1U3TGooXTabny())
													{
														break;
													}
													goto case 201;
												}
												goto case 14;
											case 337:
												array7[24] = 127;
												num4 = 484;
												goto IL_4a61_2;
											case 348:
												num34 = 0;
												num2 = 186;
												continue;
											case 378:
												_ = 124;
												_ = 96;
												num3 = /*Error near IL_0aa5: Stack underflow*/+ /*Error near IL_0aa5: Stack underflow*/;
												num = 431;
												break;
											case 33:
												num5 = 172 - 57;
												num4 = 21;
												goto IL_4a61_2;
											case 342:
												yMayDYsjD(intPtr2, 4, num37, ref num37);
												num2 = 318;
												continue;
											case 393:
												array3[9] = 108;
												num = 541;
												break;
											case 226:
												Tv7YbOvhTkPsTjfkQK(symmetricAlgorithm, CipherMode.CBC);
												num4 = 132;
												goto IL_4a61_2;
											case 187:
												num21 = 23;
												num4 = 245;
												goto IL_4a61_2;
											case 460:
												if (JLGytImuxrMxAZqJKu() != 4)
												{
													array16 = (byte[])HfDHTTAN2uenSb31bPn(XtL4lyIIgx.ToInt64());
													num = 550;
													if (true)
													{
														break;
													}
													goto case 53;
												}
												num = 569;
												if (true)
												{
													break;
												}
												goto case 381;
											case 381:
												array7[5] = 180;
												num = 359;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 32;
											case 32:
												num5 = 23 + 31;
												_ = 593;
												goto IL_4a61_2;
											case 407:
												_ = 29;
												_ = 142;
												val24 = /*Error near IL_0b9c: Stack underflow*/- 47;
												((sbyte[])/*Error near IL_0b9d: Stack underflow*/)[/*Error near IL_0b9d: Stack underflow*/] = (sbyte)(int)val24;
												num2 = 486;
												continue;
											case 173:
												_ = 4;
												_ = 105;
												((sbyte[])/*Error near IL_0bb0: Stack underflow*/)[/*Error near IL_0bb0: Stack underflow*/] = (sbyte)/*Error near IL_0bb0: Stack underflow*/;
												num4 = 281;
												goto IL_4a61_2;
											case 604:
												flag = yCGPKt8G6d0By1Tw2I(LXFsnj021(GRSglWdn3wkT4vDfh5(elLAI9D2FYClRc1Rw0(XKH3U5WCf2UyQCIIya())), "__", 10u), IntPtr.Zero);
												num = 443;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 234;
											case 234:
												num9 = 145 - 48;
												num = 494;
												if (true)
												{
													break;
												}
												goto case 17;
											case 17:
												num5 = 214 - 71;
												num4 = 93;
												goto IL_4a61_2;
											case 525:
												num3 = 231 - 77;
												num4 = 558;
												goto IL_4a61_2;
											case 30:
											case 641:
												process = (Process)XKH3U5WCf2UyQCIIya();
												_ = 420;
												continue;
											case 127:
												array7[20] = 180;
												num = 635;
												break;
											case 586:
												array3[5] = 116;
												goto case 535;
											case 409:
												num9 = 205 - 68;
												_ = 111;
												goto IL_4a61_2;
											case 533:
												T7LBbJ4ta(intPtr8, intPtr5, (byte[])NelyXWhgfYRGJe9y0i(NHZGRo0SaytvR3NXnJ(binaryReader)), 4u, out intPtr);
												num = 330;
												break;
											case 83:
												array7[21] = (byte)num6;
												num = 628;
												YiHWqdAEL2s4JBasBe();
												if ((int)/*Error near IL_0cf2: Stack underflow*/ != 0)
												{
													break;
												}
												goto case 480;
											case 480:
												array2[8] = 111;
												num = 86;
												if (true)
												{
													break;
												}
												goto case 35;
											case 35:
												num6 = 57 + 82;
												num4 = 72;
												goto IL_4a61_2;
											case 539:
												if (num14 != num15 - 1)
												{
													goto IL_3834_2;
												}
												num4 = 463;
												goto IL_4a61_2;
											case 606:
												if (JLGytImuxrMxAZqJKu() == 4)
												{
													num4 = 9;
												}
												else
												{
													num8 = iBfhYnAGRj4nG6UCpdH(new IntPtr(value));
													_ = 30;
												}
												goto IL_4a61_2;
											case 574:
												yMayDYsjD(intPtr4, num35 * 4, 4, ref num37);
												num2 = 348;
												continue;
											case 37:
												array3[4] = 105;
												num4 = 586;
												goto IL_4a61_2;
											case 277:
												num38 = 0u;
												num4 = 205;
												goto IL_4a61_2;
											case 501:
												array11[num7 + 1] = array12[1];
												num2 = 91;
												continue;
											case 447:
												array7[5] = (byte)num6;
												num2 = 16;
												continue;
											case 374:
												array11[num7] = array10[0];
												num2 = 634;
												continue;
											case 553:
												NHZGRo0SaytvR3NXnJ(binaryReader);
												num90 = (int)/*Error near IL_0dd8: Stack underflow*/;
												num2 = 489;
												continue;
											case 322:
												if (array14 == null)
												{
													goto case 476;
												}
												num = 639;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 528;
											case 36:
												_ = 0;
												intPtr12 = (nint)((Array)/*Error near IL_0e0a: Stack underflow*/).LongLength;
												iRLmBjMF2IHavq0sHJ((object)/*Error near IL_57ea: Stack underflow*/, (int)/*Error near IL_57ea: Stack underflow*/, (int)(nint)intPtr12);
												num4 = 476;
												goto IL_4a61_2;
											case 638:
												array7[4] = (byte)num3;
												num4 = 103;
												goto IL_4a61_2;
											case 210:
												array3[7] = 116;
												_ = 271;
												num = (int)/*Error near IL_0e41: Stack underflow*/;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 580;
											case 580:
												intPtr8 = Kxm8CyXvJ((uint)wDRJe2H6E4HVV6PGZs, 1, (uint)TMSAZx51W3OgMOtnPJ(XKH3U5WCf2UyQCIIya()));
												num4 = 526;
												goto IL_4a61_2;
											case 323:
												OKoHdvTcfykgJd9bmk(new IntPtr(&num11), 0);
												num2 = 344;
												continue;
											case 454:
												array11[num7 + 4] = array16[4];
												num4 = 107;
												goto IL_4a61_2;
											case 74:
												num3 = 181 - 60;
												num4 = 361;
												goto IL_4a61_2;
											case 256:
												array11[num21 + 1] = array16[1];
												num2 = 64;
												continue;
											case 132:
												transform = (ICryptoTransform)pagHAVKrD0QwEqDWBc(symmetricAlgorithm, array15, array13);
												num2 = 578;
												continue;
											case 192:
												array7[12] = (byte)num3;
												num2 = 102;
												continue;
											case 603:
												uv67RaQSbjWNCiTI1l(QX5YKsJaRCBODNJawM(binaryReader), 0L);
												num2 = 224;
												continue;
											case 18:
												num6 = 201 - 67;
												num4 = 250;
												goto IL_4a61_2;
											case 314:
												num5 = 112 + 24;
												num4 = 125;
												goto IL_4a61_2;
											case 458:
												array3[7] = 100;
												num = 7;
												break;
											case 3:
												num6 = 125 - 41;
												_ = 136;
												goto IL_4a61_2;
											case 96:
												array7[30] = 143;
												num = 451;
												SFlWP1U3TGooXTabny();
												if ((int)/*Error near IL_0f93: Stack underflow*/ == 0)
												{
													break;
												}
												goto case 106;
											case 72:
												array7[15] = (byte)num6;
												num4 = 432;
												goto IL_4a61_2;
											case 106:
												array7[14] = 113;
												num = 189;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 243;
											case 243:
												array7[7] = 94;
												_ = 326;
												continue;
											case 20:
												array7[4] = (byte)num6;
												num = 444;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 363;
											case 133:
												num8 = 0L;
												num2 = 606;
												continue;
											case 598:
												array12 = null;
												num4 = 119;
												goto IL_4a61_2;
											case 88:
												array7[31] = 114;
												num = 522;
												break;
											case 345:
												array8 = new byte[array9.Length];
												num = 47;
												break;
											case 392:
												num86 = array4.Length / 8;
												num = 629;
												if (true)
												{
													break;
												}
												goto case 522;
											case 522:
												num3 = 128 - 42;
												num = 99;
												if (true)
												{
													break;
												}
												goto case 255;
											case 255:
												array7[15] = (byte)num3;
												num2 = 35;
												continue;
											case 75:
												num6 = 118 + 61;
												num = 472;
												if (true)
												{
													break;
												}
												goto case 587;
											case 587:
												array10 = (byte[])HfDHTTAN2uenSb31bPn(num8);
												num2 = 154;
												continue;
											case 302:
												num29 = 0;
												num4 = 379;
												goto IL_4a61_2;
											case 602:
												array7[0] = 85;
												num = 341;
												if (true)
												{
													break;
												}
												goto case 269;
											case 269:
												array7[8] = 11;
												num2 = 317;
												continue;
											case 119:
												array10 = null;
												num2 = 257;
												continue;
											case 156:
												array2[2] = 166;
												num = 292;
												break;
											case 205:
												num26 = 0u;
												num2 = 567;
												continue;
											case 181:
											case 220:
												num14++;
												num2 = 44;
												continue;
											case 426:
												num95 = num35 * 4;
												num96 = num37;
												yMayDYsjD((IntPtr)/*Error near IL_11eb: Stack underflow*/, num95, num96, ref num37);
												num = 182;
												if (0 == 0)
												{
													break;
												}
												goto case 216;
											case 216:
											case 312:
												num49 = 0;
												num4 = 188;
												goto IL_4a61_2;
											case 430:
											case 474:
												if (num41 >= num32)
												{
													_ = 328;
													num = (int)/*Error near IL_121d: Stack underflow*/;
													if (true)
													{
														break;
													}
													goto case 504;
												}
												if (num41 > 0)
												{
													num = 502;
													break;
												}
												goto case 297;
											case 504:
												val20 = /*Error near IL_122c: Stack underflow*/+ 3;
												num93 = array16[3];
												((sbyte[])/*Error near IL_1231: Stack underflow*/)[val20] = (sbyte)num93;
												num = 454;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 182;
											case 26:
												if (num32 <= 0)
												{
													goto case 131;
												}
												num4 = 73;
												goto IL_4a61_2;
											case 169:
												array3[10] = 108;
												num2 = 194;
												continue;
											case 144:
												_ = 40;
												_ = 38;
												num5 = /*Error near IL_12b2: Stack underflow*/+ /*Error near IL_12b2: Stack underflow*/;
												num = 5;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 455;
											case 455:
												array7[3] = 143;
												num4 = 371;
												goto IL_4a61_2;
											case 294:
												_ = 8;
												_ = 99;
												val19 = /*Error near IL_1324: Stack underflow*/+ 87;
												((sbyte[])/*Error near IL_1325: Stack underflow*/)[/*Error near IL_1325: Stack underflow*/] = (sbyte)(int)val19;
												num2 = 435;
												continue;
											case 125:
												array2[14] = (byte)num5;
												num4 = 33;
												goto IL_4a61_2;
											case 555:
												num91 = num7 + 1;
												num92 = array16[1];
												((sbyte[])/*Error near IL_1355: Stack underflow*/)[num91] = (sbyte)num92;
												num2 = 351;
												continue;
											case 421:
												num37 = 0;
												_ = 302;
												continue;
											case 120:
												num10 = 0u;
												_ = 368;
												num = (int)/*Error near IL_1391: Stack underflow*/;
												break;
											case 251:
												intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(j8hgmZJ7n))[0]);
												num = 412;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 498;
											case 289:
												array7[12] = (byte)num6;
												num2 = 140;
												continue;
											case 635:
												array7[21] = 136;
												num4 = 402;
												goto IL_4a61_2;
											case 24:
												array7[27] = (byte)num3;
												num = 321;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 570;
											case 570:
												i244bikuos = intPtr9.ToInt64();
												num4 = 273;
												goto IL_4a61_2;
											case 631:
												num50 <<= 8;
												num2 = 637;
												continue;
											case 544:
												_ = 10;
												_ = 39;
												_ = 22;
												val18 = /*Error near IL_149c: Stack underflow*/- /*Error near IL_149c: Stack underflow*/;
												((sbyte[])/*Error near IL_149d: Stack underflow*/)[/*Error near IL_149d: Stack underflow*/] = (sbyte)(int)val18;
												num2 = 3;
												continue;
											case 233:
												_ = 1;
												_ = 101;
												((sbyte[])/*Error near IL_14b0: Stack underflow*/)[/*Error near IL_14b0: Stack underflow*/] = (sbyte)/*Error near IL_14b0: Stack underflow*/;
												num = 56;
												if (true)
												{
													break;
												}
												goto case 112;
											case 112:
												array7[15] = (byte)num6;
												num = 249;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 618;
											case 618:
												array2[5] = (byte)num9;
												_ = 561;
												continue;
											case 310:
												num6 = 114 + 7;
												num = 20;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 464;
											case 464:
												if (thBMZkAqCqa73KF6ess(intPtr10, IntPtr.Zero))
												{
													num2 = 343;
													continue;
												}
												goto case 623;
											case 352:
												array7[30] = 131;
												num4 = 96;
												goto IL_4a61_2;
											case 184:
												array7[30] = 163;
												num4 = 88;
												goto IL_4a61_2;
											case 443:
												if (!flag)
												{
													num = 143;
													break;
												}
												goto IL_1941_2;
											case 110:
												num9 = 15 + 76;
												num2 = 632;
												continue;
											case 276:
												array3[3] = 106;
												num4 = 37;
												goto IL_4a61_2;
											case 61:
												array7[23] = 188;
												num = 336;
												break;
											case 201:
												array7[7] = (byte)num6;
												num = 25;
												if (true)
												{
													break;
												}
												goto case 288;
											case 288:
												array3[1] = 115;
												num = 214;
												if (true)
												{
													break;
												}
												goto case 298;
											case 298:
												array2[1] = 98;
												num = 609;
												if (true)
												{
													break;
												}
												goto case 65;
											case 65:
												array7[23] = (byte)num6;
												num = 241;
												break;
											case 577:
												if (num90 < 1879048192)
												{
													goto case 433;
												}
												num2 = 228;
												continue;
											case 87:
												array13[9] = array14[4];
												num = 70;
												break;
											case 150:
												array11[num21] = array10[0];
												num4 = 66;
												goto IL_4a61_2;
											case 498:
												array2[10] = (byte)num5;
												num4 = 521;
												goto IL_4a61_2;
											case 377:
												array7[3] = 93;
												num = 622;
												if (0 == 0)
												{
													break;
												}
												goto case 123;
											case 123:
												num6 = 99 + 77;
												num = 538;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 285;
											case 285:
												num3 = 145 - 48;
												num2 = 239;
												continue;
											case 468:
												hSjGubHK9 = true;
												num = 338;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 490;
											case 439:
												array2[11] = (byte)num9;
												_ = 406;
												num = (int)/*Error near IL_1732: Stack underflow*/;
												break;
											case 362:
												array7[19] = 95;
												num4 = 75;
												goto IL_4a61_2;
											case 428:
												array2[12] = 163;
												num = 45;
												if (true)
												{
													break;
												}
												goto case 398;
											case 398:
												num9 = 178 - 56;
												num2 = 247;
												continue;
											case 222:
												array7[25] = (byte)num3;
												num2 = 157;
												continue;
											case 408:
												array7[11] = (byte)num6;
												num = 419;
												break;
											case 149:
												array11[num21 + 1] = array12[1];
												num = 197;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 500;
											case 500:
												_ = 151;
												_ = 50;
												num3 = /*Error near IL_180f: Stack underflow*/- /*Error near IL_180f: Stack underflow*/;
												num = 267;
												if (true)
												{
													break;
												}
												goto case 353;
											case 353:
												intPtr10 = puGi6bKKk(text);
												num4 = 464;
												goto IL_4a61_2;
											case 400:
												array7[17] = 122;
												num = 380;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 318;
											case 66:
												num88 = num21 + 1;
												num89 = array10[1];
												((sbyte[])/*Error near IL_1870: Stack underflow*/)[num88] = (sbyte)num89;
												num2 = 305;
												continue;
											case 188:
											case 466:
												if (num49 >= num86)
												{
													num2 = 162;
													continue;
												}
												_ = *(long*)(ptr + (nint)num49 * (nint)8);
												_ = 291793153;
												num87 = (long)/*Error near IL_22ff: Stack underflow*/;
												val16 = /*Error near IL_2300: Stack underflow*/^ num87;
												*(long*)(nint)/*Error near IL_2301: Stack underflow*/ = (long)val16;
												num2 = 497;
												continue;
											case 625:
												array2[0] = 81;
												num2 = 370;
												continue;
											case 578:
												iRLmBjMF2IHavq0sHJ(array15, 0, array15.Length);
												num2 = 373;
												continue;
											case 52:
												array11[num7 + 5] = array12[5];
												_ = 422;
												num = (int)/*Error near IL_18d6: Stack underflow*/;
												break;
											case 532:
												array7[24] = (byte)num6;
												_ = 337;
												continue;
											case 232:
												new MemoryStream(array4);
												binaryReader = new BinaryReader((Stream)/*Error near IL_191c: Stack underflow*/);
												_ = 603;
												continue;
											case 143:
												return;
											case 521:
												num9 = 17 + 24;
												num4 = 626;
												goto IL_4a61_2;
											case 305:
												val15 = /*Error near IL_197c: Stack underflow*/+ 2;
												num85 = array10[2];
												((sbyte[])/*Error near IL_1981: Stack underflow*/)[val15] = (sbyte)num85;
												num = 319;
												if (0 == 0)
												{
													break;
												}
												goto case 42;
											case 42:
												array2[13] = (byte)num5;
												num4 = 314;
												goto IL_4a61_2;
											case 19:
											{
												num69 = num38;
												uint num70 = 1788236480u;
												uint num71 = 1641844255u;
												uint num72 = 200510177u;
												uint num73 = num69;
												uint num74 = 93644960u;
												uint num75 = 1017401194u;
												uint num76 = ((num72 << 5) | (num72 >> 27)) ^ num70;
												uint num77 = num76 & 0xFF00FFu;
												num76 &= 0xFF00FF00u;
												num72 = (num76 >> 8) | (num77 << 8);
												uint num78 = ((num74 << 13) | (num74 >> 19)) + num70;
												uint num79 = num78 & 0xF0F0F0Fu;
												num78 &= 0xF0F0F0F0u;
												num74 = (num78 >> 4) | (num79 << 4);
												uint num80 = num70 & 0xF0F0F0Fu;
												uint num81 = num70 & 0xF0F0F0F0u;
												num80 = ((num80 >> 4) | (num81 << 4)) ^ num72;
												num70 = (num70 >> 13) | (num70 << 19);
												uint num82 = num71 & 0xFF00FFu;
												uint num83 = num71 & 0xFF00FF00u;
												num82 = ((num82 >> 8) | (num83 << 8)) + num72;
												_ = 10;
												num71 = (uint)(int)((/*Error near IL_1aea: Stack underflow*/ << (int)/*Error near IL_1aea: Stack underflow*/) | (num71 >> 22));
												ulong num84 = 1478622928 * num72;
												num84 |= 1;
												num75 = (uint)(num75 * num75 % num84);
												num73 ^= num73 << 2;
												num73 += num70;
												num73 ^= num73 << 15;
												num73 += num71;
												num73 ^= num73 >> 9;
												num73 += num75;
												num73 = (((num74 << 6) + num70) ^ num71) - num73;
												num38 = (uint)(int)(/*Error near IL_1b9b: Stack underflow*/ + (uint)(double)num73);
												num2 = 539;
												continue;
											}
											case 559:
												array7[23] = 134;
												num4 = 327;
												goto IL_4a61_2;
											case 160:
												array7[10] = 137;
												num = 544;
												if (true)
												{
													break;
												}
												goto case 330;
											case 370:
												array2[1] = 156;
												num = 358;
												break;
											case 290:
												_ = ref intPtr4;
												_ = i244bikuos;
												val14 = /*Error near IL_1c43: Stack underflow*/+ num48;
												*(IntPtr*)/*Error near IL_5a07: Stack underflow*/ = new IntPtr((long)val14);
												num = 537;
												break;
											case 344:
												iTtk4eiI7iTGDjhjDS(new IntPtr(&num11), 0);
												num2 = 375;
												continue;
											case 513:
												num41++;
												num4 = 474;
												goto IL_4a61_2;
											case 0:
												x3c4o2PyTx = NvQ34uZt895nxEhi2FIr;
												num4 = 299;
												goto IL_4a61_2;
											case 241:
												array7[23] = 90;
												num2 = 617;
												continue;
											case 280:
												array2[12] = (byte)num5;
												num4 = 110;
												goto IL_4a61_2;
											case 176:
												array7[26] = 99;
												num = 363;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 138;
											case 264:
												num28 = NHZGRo0SaytvR3NXnJ(binaryReader);
												num = 581;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 260;
											case 260:
											case 584:
												if (num54 >= array13.Length)
												{
													num = 383;
													break;
												}
												array15[num54] = (byte)(array15[num54] ^ array13[num54]);
												num2 = 60;
												continue;
											case 111:
												array2[0] = (byte)num9;
												num2 = 141;
												continue;
											case 359:
												num6 = 12 + 86;
												_ = 462;
												goto IL_4a61_2;
											case 267:
												array7[8] = (byte)num3;
												num4 = 294;
												goto IL_4a61_2;
											case 375:
												FsHWNgkGiVuOl8OmJc(new IntPtr(&num11), 0, IntPtr.Zero);
												num = 76;
												if (true)
												{
													break;
												}
												goto case 366;
											case 366:
												array13[15] = array14[7];
												num4 = 36;
												goto IL_4a61_2;
											case 389:
												_ = new byte[1];
												array5 = (byte[])/*Error near IL_1dbc: Stack underflow*/;
												num4 = 413;
												goto IL_4a61_2;
											case 388:
												num9 = 22 + 76;
												num4 = 286;
												goto IL_4a61_2;
											case 117:
												array2[3] = (byte)num5;
												num4 = 6;
												goto IL_4a61_2;
											case 529:
												if (num14 != num15 - 1)
												{
													goto IL_2e86_2;
												}
												num4 = 15;
												goto IL_4a61_2;
											case 403:
												num3 = 60 + 93;
												num2 = 114;
												continue;
											case 51:
												array2[4] = 162;
												num = 32;
												if (true)
												{
													break;
												}
												goto case 56;
											case 56:
												array20[2] = 116;
												num2 = 470;
												continue;
											case 346:
												array2[7] = 73;
												num = 566;
												break;
											case 63:
												IBe4hEip2A = new Hashtable(NHZGRo0SaytvR3NXnJ(binaryReader) + 1);
												_ = 547;
												num = (int)/*Error near IL_1e98: Stack underflow*/;
												break;
											case 623:
												array20 = new byte[6];
												_ = 43;
												goto IL_4a61_2;
											case 237:
												num6 = 50 + 106;
												num = 465;
												break;
											case 372:
												array7[29] = 150;
												num = 18;
												if (0 == 0)
												{
													break;
												}
												goto case 271;
											case 271:
												array3[8] = 46;
												num4 = 440;
												goto IL_4a61_2;
											case 412:
												uS9zmJ6WC = intPtr9.ToInt32();
												num2 = 14;
												continue;
											case 538:
												array7[14] = (byte)num6;
												num2 = 106;
												continue;
											case 304:
												num5 = 92 + 2;
												num4 = 275;
												goto IL_4a61_2;
											case 614:
												array7[28] = (byte)num6;
												_ = 213;
												num = (int)/*Error near IL_1f93: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 165;
											case 165:
												if (JLGytImuxrMxAZqJKu() == 4)
												{
													num4 = 202;
													goto IL_4a61_2;
												}
												value = iBfhYnAGRj4nG6UCpdH(intPtr6);
												num2 = 568;
												continue;
											case 329:
												array7[9] = (byte)num3;
												num2 = 548;
												continue;
											case 470:
												array20[3] = 74;
												num = 173;
												if (true)
												{
													break;
												}
												goto case 7;
											case 7:
												array3[8] = 108;
												num4 = 393;
												goto IL_4a61_2;
											case 284:
												array2[13] = (byte)num5;
												num2 = 457;
												continue;
											case 153:
												array7[8] = 84;
												_ = 269;
												num = (int)/*Error near IL_2021: Stack underflow*/;
												break;
											case 73:
												num15 = /*Error near IL_202d: Stack underflow*/+ 1;
												num = 131;
												break;
											case 223:
												num12 = num14 * 4;
												num = 48;
												if (0 == 0)
												{
													break;
												}
												goto case 252;
											case 252:
												array2[13] = (byte)num9;
												num = 492;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 209;
											case 209:
												_ = 16;
												_ = 137;
												_ = 45;
												val12 = /*Error near IL_2087: Stack underflow*/- /*Error near IL_2087: Stack underflow*/;
												((sbyte[])/*Error near IL_2088: Stack underflow*/)[/*Error near IL_2088: Stack underflow*/] = (sbyte)(int)val12;
												num4 = 198;
												goto IL_4a61_2;
											case 82:
												array2[15] = (byte)num9;
												num = 391;
												break;
											case 624:
												array = null;
												num4 = 448;
												goto IL_4a61_2;
											case 113:
												yMayDYsjD(new IntPtr(value), JLGytImuxrMxAZqJKu(), 64, ref num31);
												num4 = 254;
												goto IL_4a61_2;
											case 270:
												array2[5] = 87;
												num2 = 360;
												continue;
											case 617:
												num6 = 52 - 23;
												num2 = 29;
												continue;
											case 197:
												array11[num21 + 2] = array12[2];
												num4 = 161;
												goto IL_4a61_2;
											case 183:
											case 231:
												if (num36 >= num28)
												{
													num4 = 438;
												}
												else
												{
													_ = ref intPtr5;
													_ = i244bikuos + NHZGRo0SaytvR3NXnJ(binaryReader);
													num68 = (long)/*Error near IL_3304: Stack underflow*/;
													val11 = /*Error near IL_3305: Stack underflow*/- num68;
													*(IntPtr*)/*Error near IL_330a: Stack underflow*/ = new IntPtr((long)val11);
													num4 = 445;
												}
												goto IL_4a61_2;
											case 469:
												array7[0] = (byte)num6;
												_ = 602;
												num = (int)/*Error near IL_2152: Stack underflow*/;
												break;
											case 637:
											{
												int num33 = /*Error near IL_215e: Stack underflow*/+ 8;
												num = 585;
												if (true)
												{
													break;
												}
												goto case 22;
											}
											case 22:
											case 328:
												num38 = (uint)/*Error near IL_2176: Stack underflow*/;
												num4 = 19;
												goto IL_4a61_2;
											case 471:
												array7[11] = (byte)num3;
												num = 525;
												break;
											case 461:
												array2[6] = (byte)num9;
												num4 = 530;
												goto IL_4a61_2;
											case 295:
												_ = 67;
												_ = 47;
												num9 = /*Error near IL_21bf: Stack underflow*/+ /*Error near IL_21bf: Stack underflow*/;
												_ = 34;
												num = (int)/*Error near IL_21cc: Stack underflow*/;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 459;
											case 459:
												array7[2] = (byte)num6;
												num2 = 100;
												continue;
											case 249:
												num3 = 112 + 118;
												num2 = 591;
												continue;
											case 47:
												num42 = array15.Length / 4;
												num4 = 277;
												goto IL_4a61_2;
											case 536:
												num6 = 97 + 108;
												num = 627;
												break;
											case 203:
												num7 = 18;
												num2 = 374;
												continue;
											case 399:
												array7[27] = 97;
												num = 325;
												break;
											case 511:
												array7[17] = (byte)num3;
												num4 = 396;
												goto IL_4a61_2;
											case 361:
												array7[3] = (byte)num3;
												num = 285;
												if (0 == 0)
												{
													break;
												}
												goto case 475;
											case 475:
												num67 = 0;
												num = 518;
												if (0 == 0)
												{
													break;
												}
												goto case 46;
											case 46:
												array11[num21 + 3] = array16[3];
												num = 108;
												if (0 == 0)
												{
													break;
												}
												goto case 589;
											case 589:
												yMayDYsjD(new IntPtr(value), JLGytImuxrMxAZqJKu(), num31, ref num31);
												num2 = 642;
												continue;
											case 118:
												O2mGeMA7472VhJ2Eakk(blmQxKAZMFpiBMOawsF(j5I56qAWJhTHkpG09e6(x3c4o2PyTx)));
												_ = 624;
												goto IL_4a61_2;
											case 567:
												num10 = 0u;
												num4 = 26;
												goto IL_4a61_2;
											case 13:
												array2[13] = (byte)num9;
												num2 = 554;
												continue;
											case 221:
												array7[16] = 89;
												num4 = 209;
												goto IL_4a61_2;
											case 258:
												num9 = 138 - 46;
												num2 = 367;
												continue;
											case 89:
												array11[num21] = array16[0];
												num4 = 256;
												goto IL_4a61_2;
											case 451:
												array7[30] = 86;
												num = 350;
												break;
											case 126:
												num6 = 143 - 47;
												num4 = 532;
												goto IL_4a61_2;
											case 583:
												array11[num7 + 4] = array12[4];
												num = 52;
												break;
											case 349:
												array7[6] = 108;
												num = 560;
												if (true)
												{
													break;
												}
												goto case 485;
											case 485:
												num5 = 89 + 50;
												num2 = 229;
												continue;
											case 560:
												array7[6] = 41;
												num2 = 49;
												continue;
											case 163:
												((sbyte[])/*Error near IL_2479: Stack underflow*/)[2] = 114;
												_ = 276;
												continue;
											case 350:
												array7[30] = 108;
												num2 = 184;
												continue;
											case 199:
												mGMWpQe4ChoBUxrIed(cryptoStream);
												num = 411;
												break;
											case 379:
												lGqEHaRDPgHFpusu4D(j8hgmZJ7n);
												if ((int)/*Error near IL_24c6: Stack underflow*/ != 0)
												{
													num2 = 385;
													continue;
												}
												goto case 335;
											case 244:
												_ = ref lk7BwHKFmNJY32ZC3n2;
												_ = new byte[1];
												array5 = (byte[])/*Error near IL_24da: Stack underflow*/;
												array5[0] = 42;
												((Lk7BwHKFmNJY32ZC3n*)(nint)/*Error near IL_24e7: Stack underflow*/)->Uu349Vtr47 = array5;
												num2 = 473;
												continue;
											case 508:
												array2[12] = (byte)num9;
												num4 = 590;
												goto IL_4a61_2;
											case 147:
												array7[27] = 151;
												num = 135;
												break;
											case 78:
												array11[num7 + 2] = array10[2];
												num2 = 611;
												continue;
											case 315:
												array7[18] = (byte)num3;
												num4 = 576;
												goto IL_4a61_2;
											case 387:
												array2[12] = (byte)num5;
												num4 = 94;
												goto IL_4a61_2;
											case 316:
												num9 = 135 - 45;
												num4 = 242;
												goto IL_4a61_2;
											case 242:
												array2[4] = (byte)num9;
												num2 = 196;
												continue;
											case 90:
												_ = new byte[12];
												array3 = (byte[])/*Error near IL_25c9: Stack underflow*/;
												num4 = 146;
												goto IL_4a61_2;
											case 495:
												num5 = 209 - 69;
												num2 = 404;
												continue;
											case 332:
												array7[22] = (byte)num6;
												num4 = 262;
												goto IL_4a61_2;
											case 225:
												num38 = (uint)(int)(/*Error near IL_260c: Stack underflow*/ + num26);
												num4 = 50;
												goto IL_4a61_2;
											case 594:
												array15 = array7;
												num2 = 230;
												continue;
											case 565:
												_ = 5;
												_ = 81;
												val10 = /*Error near IL_263c: Stack underflow*/+ 49;
												((sbyte[])/*Error near IL_263d: Stack underflow*/)[/*Error near IL_263d: Stack underflow*/] = (sbyte)(int)val10;
												num4 = 381;
												goto IL_4a61_2;
											case 415:
												if ((int)/*Error near IL_5b82: Stack underflow*/ == -185339151)
												{
													num = 604;
													if (YiHWqdAEL2s4JBasBe())
													{
														break;
													}
													goto case 443;
												}
												goto IL_1941_2;
											case 627:
												array7[6] = (byte)num6;
												num = 349;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 190;
											case 452:
												num5 = 122 + 124;
												num = 498;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 86;
											case 86:
												num9 = 170 - 87;
												num4 = 510;
												goto IL_4a61_2;
											case 527:
												array7[6] = (byte)num6;
												num2 = 243;
												continue;
											case 300:
												array2[2] = 143;
												num = 261;
												break;
											case 62:
												num6 = 184 + 33;
												num = 175;
												if (true)
												{
													break;
												}
												goto case 404;
											case 404:
												array2[7] = (byte)num5;
												num = 346;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 316;
											case 281:
												array20[5] = 116;
												num2 = 124;
												continue;
											case 38:
												((sbyte[])/*Error near IL_279a: Stack underflow*/)[4] = 114;
												num4 = 424;
												goto IL_4a61_2;
											case 228:
												bV44XU8KQo = true;
												num = 433;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 185;
											case 185:
												array2[10] = (byte)num5;
												num4 = 200;
												goto IL_4a61_2;
											case 224:
												intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(j8hgmZJ7n))[0]);
												num4 = 339;
												goto IL_4a61_2;
											case 12:
												num3 = 109 - 11;
												num2 = 222;
												continue;
											case 214:
												array3[2] = 99;
												num4 = 499;
												goto IL_4a61_2;
											case 593:
												array2[4] = (byte)num5;
												num2 = 316;
												continue;
											case 67:
											case 154:
												if (JLGytImuxrMxAZqJKu() != 4)
												{
													num7 = 2;
													num4 = 446;
												}
												else
												{
													num4 = 616;
												}
												goto IL_4a61_2;
											case 121:
												array7[26] = 170;
												_ = 176;
												goto IL_4a61_2;
											case 102:
												array7[13] = 73;
												num = 155;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 4;
											case 4:
												array7[21] = (byte)num3;
												num2 = 62;
												continue;
											case 579:
												num9 = 142 - 47;
												num = 142;
												break;
											case 541:
												text = (string)eKmFLmAUTIA4RBnjHCA(FBQwxZAAAGt3s3HSZ3l(), array3);
												num2 = 274;
												continue;
											case 442:
												array7[22] = (byte)num3;
												num = 559;
												if (true)
												{
													break;
												}
												goto case 472;
											case 472:
												array7[19] = (byte)num6;
												num = 478;
												if (0 == 0)
												{
													break;
												}
												goto case 108;
											case 108:
												num21 = 16;
												_ = 150;
												num = (int)/*Error near IL_2920: Stack underflow*/;
												break;
											case 420:
												try
												{
													ProcessModuleCollection processModuleCollection2 = (ProcessModuleCollection)kxxORYNQpt7JeTNfIx(process);
													enumerator = (IEnumerator)JvuscRYrEHnEfwpXlR(processModuleCollection2);
													try
													{
														while (Prhas5pe6XdsQAESvm(enumerator))
														{
															ProcessModule processModule2 = (ProcessModule)w040Z1ZHbG0XVqZMoU(enumerator);
															YiHWqdAEL2s4JBasBe();
															int num59;
															if (!SFlWP1U3TGooXTabny())
															{
																num59 = 2;
																if (!YiHWqdAEL2s4JBasBe())
																{
																	goto IL_2a29_2;
																}
															}
															else
															{
																num59 = 3;
																if (1 == 0)
																{
																	goto IL_299a_2;
																}
															}
															goto IL_29bf_3;
															IL_29fc_2:
															if (!qGsoXYAtg5xUyI3IWm5(gQ3bxJAoGy2rvkCxjGU(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly), null))
															{
																continue;
															}
															num59 = 6;
															goto IL_29bf_3;
															IL_299a_2:
															num60 = num8;
															intPtr9 = GRSglWdn3wkT4vDfh5(processModule2);
															if (num60 < intPtr9.ToInt64())
															{
																goto IL_29fc_2;
															}
															goto IL_2a29_2;
															IL_2a29_2:
															num61 = num8;
															intPtr9 = GRSglWdn3wkT4vDfh5(processModule2);
															if (num61 <= intPtr9.ToInt64() + Btnc15ArMdr21Qis9mc(processModule2))
															{
																continue;
															}
															num59 = 4;
															if (1 == 0)
															{
																return;
															}
															goto IL_29bf_3;
															IL_29bf_3:
															while (true)
															{
																switch (num59)
																{
																case 0:
																case 2:
																	break;
																case 1:
																case 3:
																	goto IL_299a_2;
																default:
																	num59 = 5;
																	if (!SFlWP1U3TGooXTabny())
																	{
																		continue;
																	}
																	goto IL_29fc_2;
																case 4:
																	goto IL_29fc_2;
																case 5:
																	goto IL_2a29_2;
																case 6:
																	return;
																}
																break;
															}
															if (!qNGJ4OfmSpfpcWEKvj(z1aSaf7nHt2GSLVr8b(processModule2), text))
															{
																continue;
															}
															goto IL_299a_2;
														}
													}
													finally
													{
														if (enumerator is IDisposable disposable3)
														{
															p8SJ0uC03Z63haWUBt(disposable3);
														}
													}
												}
												catch (object obj7)
												{
												}
												try
												{
													ProcessModuleCollection processModuleCollection3 = (ProcessModuleCollection)kxxORYNQpt7JeTNfIx(process);
													enumerator = (IEnumerator)JvuscRYrEHnEfwpXlR(processModuleCollection3);
													try
													{
														while (Prhas5pe6XdsQAESvm(enumerator))
														{
															ProcessModule processModule3 = (ProcessModule)w040Z1ZHbG0XVqZMoU(enumerator);
															num62 = 3;
															while (true)
															{
																int num63;
																switch (num62)
																{
																case 1:
																case 4:
																	goto IL_2ac0_2;
																case 3:
																	intPtr9 = GRSglWdn3wkT4vDfh5(processModule3);
																	SFlWP1U3TGooXTabny();
																	num64 = (num63 = (YiHWqdAEL2s4JBasBe() ? 4 : 0));
																	goto IL_2adf_2;
																case 5:
																	goto end_IL_2ae3_2;
																}
																goto IL_2b33_2;
																IL_2ac0_2:
																if (intPtr9.ToInt64() != phV4Uu6SUx)
																{
																	goto IL_2b49_2;
																}
																goto IL_2b33_2;
																IL_2b33_2:
																num29 = 0;
																num63 = 5;
																goto IL_2adf_2;
																IL_2adf_2:
																num62 = num63;
																continue;
																end_IL_2ae3_2:
																break;
															}
															break;
															IL_2b49_2:;
														}
													}
													finally
													{
														if (enumerator is IDisposable disposable4)
														{
															p8SJ0uC03Z63haWUBt(disposable4);
														}
													}
												}
												catch (object obj8)
												{
												}
												bFB44BUGlg = null;
												num4 = 605;
												goto IL_4a61_2;
											case 358:
												_ = 1;
												_ = 222;
												_ = 74;
												val9 = /*Error near IL_2ba1: Stack underflow*/- /*Error near IL_2ba1: Stack underflow*/;
												((sbyte[])/*Error near IL_2ba2: Stack underflow*/)[/*Error near IL_2ba2: Stack underflow*/] = (sbyte)(int)val9;
												num4 = 298;
												goto IL_4a61_2;
											case 174:
												lk7BwHKFmNJY32ZC3n = default(Lk7BwHKFmNJY32ZC3n);
												num4 = 27;
												goto IL_4a61_2;
											case 566:
												array2[7] = 220;
												num = 304;
												if (true)
												{
													break;
												}
												goto case 182;
											case 182:
											case 438:
												IUSsYAngpyKANEnZ1U(QX5YKsJaRCBODNJawM(binaryReader));
												OhZpyDwZxpiyDh5BOJ(QX5YKsJaRCBODNJawM(binaryReader));
												val21 = /*Error near IL_2bfd: Stack underflow*/- 1;
												if (/*Error near IL_2c02: Stack underflow*/ < val21)
												{
													num = 95;
													if (!SFlWP1U3TGooXTabny())
													{
														break;
													}
													goto case 341;
												}
												JkHjxJCFT(intPtr8);
												num2 = 418;
												continue;
											case 257:
												array16 = null;
												num4 = 460;
												goto IL_4a61_2;
											case 265:
												if (array5.Length != 0)
												{
													goto IL_02f5;
												}
												num = 516;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 639;
											case 331:
												num58 = array12[0];
												((sbyte[])/*Error near IL_2c54: Stack underflow*/)[/*Error near IL_2c54: Stack underflow*/] = (sbyte)num58;
												num = 501;
												if (true)
												{
													break;
												}
												goto case 196;
											case 196:
												array2[4] = 80;
												num4 = 579;
												goto IL_4a61_2;
											case 462:
												array7[6] = (byte)num6;
												num2 = 417;
												continue;
											case 116:
												array13 = array2;
												num4 = 218;
												goto IL_4a61_2;
											case 628:
												num3 = 138 - 46;
												num = 4;
												break;
											case 250:
												array7[30] = (byte)num6;
												num = 352;
												break;
											case 486:
												array7[29] = 160;
												num2 = 372;
												continue;
											case 105:
												lGqEHaRDPgHFpusu4D(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly);
												if ((int)/*Error near IL_2d1b: Stack underflow*/ != 0)
												{
													num2 = 291;
													continue;
												}
												goto IL_33f4;
											case 136:
												array7[11] = (byte)num6;
												num4 = 263;
												goto IL_4a61_2;
											case 10:
											case 506:
												tlfI1xoeKCs7nuslSe(array11, 0, intPtr7, array11.Length);
												num2 = 248;
												continue;
											case 582:
												array7[7] = (byte)num6;
												num = 500;
												if (true)
												{
													break;
												}
												goto case 259;
											case 259:
												((sbyte[])/*Error near IL_2d83: Stack underflow*/)[num12] = (sbyte)(byte)(num13 & 0xFF);
												num4 = 355;
												goto IL_4a61_2;
											case 564:
												NHZGRo0SaytvR3NXnJ(binaryReader);
												M0UK57Lct29oQf1y7M((IntPtr)/*Error near IL_2d9b: Stack underflow*/, (int)/*Error near IL_2d9b: Stack underflow*/);
												num2 = 342;
												continue;
											case 436:
												array2[11] = (byte)num9;
												num2 = 428;
												continue;
											case 505:
												intPtr9.ToInt64();
												phV4Uu6SUx = (long)/*Error near IL_2dc9: Stack underflow*/;
												num = 449;
												if (0 == 0)
												{
													break;
												}
												goto case 592;
											case 487:
												intPtr6 = HfSOpPAiXrYiEn2URpZ(lhmiV9AUoOr1v5yhIs);
												num = 519;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 456;
											case 60:
												num54++;
												num2 = 584;
												continue;
											case 190:
												num6 = 77 + 10;
												num = 469;
												break;
											case 131:
												num23 = 0u;
												num4 = 68;
												goto IL_4a61_2;
											case 548:
												num3 = 178 + 58;
												num4 = 405;
												goto IL_4a61_2;
											case 142:
												array2[5] = (byte)num9;
												num4 = 270;
												goto IL_4a61_2;
											case 14:
												intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(j8hgmZJ7n))[0]);
												num4 = 570;
												goto IL_4a61_2;
											case 146:
												array3[0] = 109;
												num = 288;
												if (0 == 0)
												{
													break;
												}
												goto case 430;
											case 283:
												array7[16] = 90;
												num = 221;
												if (true)
												{
													break;
												}
												goto case 591;
											case 591:
												array7[16] = (byte)num3;
												num2 = 306;
												continue;
											case 422:
												num52 = num7 + 6;
												num53 = array12[6];
												((sbyte[])/*Error near IL_2f2b: Stack underflow*/)[num52] = (sbyte)num53;
												_ = 364;
												goto IL_4a61_2;
											case 634:
												array11[num7 + 1] = array10[1];
												num2 = 78;
												continue;
											case 477:
												num6 = 244 - 81;
												num4 = 459;
												goto IL_4a61_2;
											case 509:
												array7[16] = (byte)num6;
												num4 = 283;
												goto IL_4a61_2;
											case 198:
												array7[16] = 219;
												num4 = 400;
												goto IL_4a61_2;
											case 148:
												array7[8] = (byte)num3;
												num4 = 153;
												goto IL_4a61_2;
											case 528:
												num6 = 97 + 16;
												num = 534;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 171;
											case 171:
												if (!sPFYCLItnSewrnGJfh(gKjkGS27vZd6ReTL7N("System.Reflection.ReflectionContext", false), null))
												{
													goto IL_3b9e;
												}
												num = 613;
												if (true)
												{
													break;
												}
												goto case 2;
											case 326:
												num6 = 67 + 47;
												num2 = 201;
												continue;
											case 491:
												array7[1] = (byte)num3;
												num4 = 403;
												goto IL_4a61_2;
											case 382:
												num3 = 146 - 48;
												num2 = 491;
												continue;
											case 296:
												array7 = new byte[32];
												num2 = 190;
												continue;
											case 380:
												num3 = 97 + 40;
												num = 511;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 227;
											case 227:
												array2[3] = 149;
												num4 = 109;
												goto IL_4a61_2;
											case 620:
												mGMWpQe4ChoBUxrIed(memoryStream);
												num4 = 199;
												goto IL_4a61_2;
											case 612:
												bV44XU8KQo = false;
												num4 = 414;
												goto IL_4a61_2;
											case 278:
												num28 = NHZGRo0SaytvR3NXnJ(binaryReader);
												num4 = 599;
												goto IL_4a61_2;
											case 499:
												array3[3] = 111;
												num4 = 38;
												goto IL_4a61_2;
											case 292:
												_ = 2;
												_ = 79;
												_ = 71;
												val8 = /*Error near IL_312b: Stack underflow*/+ /*Error near IL_312b: Stack underflow*/;
												((sbyte[])/*Error near IL_312c: Stack underflow*/)[/*Error near IL_312c: Stack underflow*/] = (sbyte)(int)val8;
												num = 592;
												if (true)
												{
													break;
												}
												goto case 28;
											case 28:
												array4 = array8;
												num2 = 392;
												continue;
											case 321:
												_ = 27;
												_ = 105;
												_ = 40;
												val7 = /*Error near IL_315d: Stack underflow*/- /*Error near IL_315d: Stack underflow*/;
												((sbyte[])/*Error near IL_315e: Stack underflow*/)[/*Error near IL_315e: Stack underflow*/] = (sbyte)(int)val7;
												num4 = 427;
												goto IL_4a61_2;
											case 130:
												FqA3fjAY4CaOvkoaMo3(x3c4o2PyTx);
												num2 = 118;
												continue;
											case 137:
											case 212:
												intPtr7 = jMyYFyWuy(IntPtr.Zero, (uint)array.Length, 4096u, 64u);
												num = 434;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 299;
											case 293:
												array17 = (byte[])u2tFaHurfr1s9xSbVJ(binaryReader, (int)OhZpyDwZxpiyDh5BOJ(QX5YKsJaRCBODNJawM(binaryReader)));
												num2 = 296;
												continue;
											case 384:
												tg8t5OqN45OAVd2Uq7(new IntPtr(&num11), 0);
												num = 323;
												if (0 == 0)
												{
													break;
												}
												goto case 446;
											case 446:
												array11[num7] = array16[0];
												num2 = 555;
												continue;
											case 279:
											case 414:
												IUSsYAngpyKANEnZ1U(QX5YKsJaRCBODNJawM(binaryReader));
												QX5YKsJaRCBODNJawM(binaryReader);
												num51 = OhZpyDwZxpiyDh5BOJ((object)/*Error near IL_3214: Stack underflow*/);
												if ((long)/*Error near IL_321c: Stack underflow*/ >= num51 - 1)
												{
													intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly))[0]);
													num2 = 505;
													continue;
												}
												num4 = 395;
												goto IL_4a61_2;
											case 93:
												array2[13] = (byte)num5;
												num2 = 54;
												continue;
											case 385:
												if (KpNDnm3q1r6YoZRF9H(lGqEHaRDPgHFpusu4D(j8hgmZJ7n)) != 0)
												{
													goto case 264;
												}
												num4 = 335;
												goto IL_4a61_2;
											case 218:
												KKPSQdOpi1WKW3jfUr(array13);
												num = 488;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 397;
											case 397:
												if (JLGytImuxrMxAZqJKu() != 4)
												{
													T7LBbJ4ta(intPtr8, intPtr5, (byte[])NelyXWhgfYRGJe9y0i(NHZGRo0SaytvR3NXnJ(binaryReader)), 4u, out intPtr);
													num2 = 621;
												}
												else
												{
													num2 = 533;
												}
												continue;
											case 622:
												array7[3] = 51;
												num = 253;
												if (SFlWP1U3TGooXTabny())
												{
													goto case 10;
												}
												break;
											case 206:
												array13[7] = array14[3];
												num2 = 87;
												continue;
											case 364:
												array11[num7 + 7] = array12[7];
												num = 506;
												if (true)
												{
													break;
												}
												goto case 41;
											case 41:
												array13[13] = array14[6];
												num = 366;
												YiHWqdAEL2s4JBasBe();
												if ((int)/*Error near IL_334a: Stack underflow*/ != 0)
												{
													break;
												}
												goto case 178;
											case 178:
												_ = (LhmiV9AUoOr1v5yhIs)krLZHPATvDcIfeOggP9(intPtr11, rEacmnz8ipU2tXY6jc(typeof(LhmiV9AUoOr1v5yhIs).TypeHandle));
												lhmiV9AUoOr1v5yhIs = (LhmiV9AUoOr1v5yhIs)/*Error near IL_3362: Stack underflow*/;
												num = 487;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 581;
											case 581:
												num20 = NHZGRo0SaytvR3NXnJ(binaryReader);
												num4 = 515;
												goto IL_4a61_2;
											case 9:
												num8 = sYNV1XAk8wX0pLjqgmD(new IntPtr(value));
												num = 641;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 556;
											case 556:
												array7[25] = 98;
												num = 354;
												break;
											case 334:
												return;
											case 595:
												_ = 4;
												_ = 4;
												_ = ref num37;
												yMayDYsjD((IntPtr)/*Error near IL_5ce6: Stack underflow*/, (int)/*Error near IL_5ce6: Stack underflow*/, (int)/*Error near IL_5ce6: Stack underflow*/, ref *(int*)/*Error near IL_5ce6: Stack underflow*/);
												num = 564;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 245;
											case 245:
												array11[num21] = array12[0];
												_ = 149;
												num = (int)/*Error near IL_3684: Stack underflow*/;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 115;
											case 114:
												array7[1] = (byte)num3;
												num = 482;
												break;
											case 307:
												_ = 1;
												_ = 110;
												num3 = /*Error near IL_36ce: Stack underflow*/+ /*Error near IL_36ce: Stack underflow*/;
												num = 329;
												break;
											case 615:
												num50 = 255u;
												num2 = 129;
												continue;
											case 497:
												num49++;
												num = 466;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 166;
											case 235:
												_ = rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly;
												if (((Array)XYFEn2AdQMDhr0ac0wL(gQ3bxJAoGy2rvkCxjGU((object)/*Error near IL_3723: Stack underflow*/))).Length == 2)
												{
													num = 105;
													break;
												}
												goto IL_33f4;
											case 489:
												bV44XU8KQo = false;
												num = 577;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 600;
											case 157:
												num3 = 70 + 7;
												num4 = 53;
												goto IL_4a61_2;
											case 49:
												num6 = 166 - 113;
												num = 527;
												break;
											case 613:
												processModuleCollection = (ProcessModuleCollection)kxxORYNQpt7JeTNfIx(XKH3U5WCf2UyQCIIya());
												num4 = 324;
												goto IL_4a61_2;
											case 34:
												array2[8] = (byte)num9;
												num = 234;
												break;
											case 195:
												num6 = 116 + 76;
												num4 = 311;
												goto IL_4a61_2;
											case 107:
												array11[num7 + 5] = array16[5];
												num4 = 401;
												goto IL_4a61_2;
											case 95:
												num48 = NHZGRo0SaytvR3NXnJ(binaryReader);
												num4 = 290;
												goto IL_4a61_2;
											case 425:
												num34++;
												num = 410;
												if (true)
												{
													break;
												}
												goto case 324;
											case 324:
												JvuscRYrEHnEfwpXlR(processModuleCollection);
												enumerator = (IEnumerator)/*Error near IL_3820: Stack underflow*/;
												num = 81;
												if (0 == 0)
												{
													break;
												}
												goto case 181;
											case 16:
												array7[5] = 95;
												num2 = 152;
												continue;
											case 134:
												num5 = 45 - 26;
												num2 = 204;
												continue;
											case 325:
												array7[27] = 142;
												num4 = 147;
												goto IL_4a61_2;
											case 138:
												_ = 231;
												_ = 77;
												num6 = /*Error near IL_38be: Stack underflow*/- /*Error near IL_38be: Stack underflow*/;
												num = 177;
												if (true)
												{
													break;
												}
												goto case 543;
											case 543:
												array11[num7 + 7] = array16[7];
												num4 = 203;
												goto IL_4a61_2;
											case 194:
												array3[11] = 108;
												num2 = 215;
												continue;
											case 596:
												array7[3] = (byte)num3;
												_ = 377;
												goto IL_4a61_2;
											case 274:
												intPtr10 = puGi6bKKk(text);
												num2 = 623;
												continue;
											case 357:
											case 568:
												tg8t5OqN45OAVd2Uq7(intPtr6, 0);
												num4 = 0;
												goto IL_4a61_2;
											case 79:
												num9 = 148 - 49;
												num2 = 439;
												continue;
											case 482:
												_ = 2;
												_ = 120;
												_ = 114;
												val6 = /*Error near IL_3962: Stack underflow*/+ /*Error near IL_3962: Stack underflow*/;
												((sbyte[])/*Error near IL_3963: Stack underflow*/)[/*Error near IL_3963: Stack underflow*/] = (sbyte)(int)val6;
												num = 85;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 295;
											case 76:
												o00Z5hGmAa4HorFX8O(new IntPtr(&num11), 0, 0);
												_ = 139;
												goto IL_4a61_2;
											case 394:
												array8[num12 + 2] = (byte)((num13 & 0xFF0000) >> 16);
												num2 = 540;
												continue;
											case 204:
												array2[3] = (byte)num5;
												num = 388;
												break;
											case 373:
												memoryStream = new MemoryStream();
												num4 = 390;
												goto IL_4a61_2;
											case 170:
												num38 = (uint)(/*Error near IL_39ec: Stack underflow*/ + /*Error near IL_39ec: Stack underflow*/);
												num2 = 120;
												continue;
											case 611:
												val5 = /*Error near IL_3a01: Stack underflow*/+ 3;
												num47 = array10[3];
												((sbyte[])/*Error near IL_3a06: Stack underflow*/)[val5] = (sbyte)num47;
												num = 40;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 464;
											case 81:
												try
												{
													while (Prhas5pe6XdsQAESvm(enumerator))
													{
														ProcessModule processModule = (ProcessModule)w040Z1ZHbG0XVqZMoU(enumerator);
														num44 = 2;
														while (true)
														{
															int num45 = num44;
															while (true)
															{
																num46 = num45;
																while (true)
																{
																	switch (num46)
																	{
																	case 2:
																		break;
																	case 5:
																	case 7:
																		if (vAbWvNPlhhsilht7I2(version2, version))
																		{
																			goto IL_3a6e;
																		}
																		goto IL_3b75;
																	case 1:
																		goto IL_3a7c;
																	case 8:
																		goto end_IL_3af8;
																	case 4:
																		if (!B28kJjXJMefNoj8Ht7(version2, version3))
																		{
																			goto IL_3b75;
																		}
																		goto case 5;
																	case 0:
																	case 3:
																		version = new Version(4, 0, 30319, 17921);
																		goto case 4;
																	default:
																		num46 = 4;
																		continue;
																	case 6:
																		goto IL_3b2f;
																	case 9:
																		goto end_IL_3af0;
																	}
																	if (qNGJ4OfmSpfpcWEKvj(ustSntgIXxQiPYwipw(z1aSaf7nHt2GSLVr8b(processModule)), "clrjit.dll"))
																	{
																		num46 = 6;
																		continue;
																	}
																	goto IL_3b75;
																	IL_3a7c:
																	version3 = new Version(4, 0, 30319, 17020);
																	_ = 1;
																	num46 = (SFlWP1U3TGooXTabny() ? 7 : 0);
																	continue;
																	end_IL_3af8:
																	break;
																}
																break;
																IL_3a6e:
																num45 = 8;
															}
															NrL10qsNW = true;
															num44 = 9;
															continue;
															IL_3b2f:
															version2 = new Version(pYLE2mB3h2h5Q9EWWJ(XEpsOX15JQGw1yNP1V(processModule)), ddHTLXcmUp85qbEZ5v(XEpsOX15JQGw1yNP1V(processModule)), YSPPRZbemZq4tiMetn(XEpsOX15JQGw1yNP1V(processModule)), VfX7Wy6qBCfj5QN0tS(XEpsOX15JQGw1yNP1V(processModule)));
															num44 = 1;
															continue;
															end_IL_3af0:
															break;
														}
														break;
														IL_3b75:;
													}
												}
												finally
												{
													if (enumerator is IDisposable disposable2)
													{
														p8SJ0uC03Z63haWUBt(disposable2);
													}
												}
												goto IL_3b9e;
											case 320:
												if (qGsoXYAtg5xUyI3IWm5(gQ3bxJAoGy2rvkCxjGU(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly), null))
												{
													num = 235;
													if (YiHWqdAEL2s4JBasBe())
													{
														break;
													}
													goto case 63;
												}
												goto IL_33f4;
											case 44:
											case 179:
												if (num14 < num15)
												{
													num43 = num14 % num42;
													num2 = 223;
												}
												else
												{
													num2 = 28;
												}
												continue;
											case 239:
												array7[3] = (byte)num3;
												num2 = 455;
												continue;
											case 524:
												array2[6] = (byte)num5;
												num = 191;
												break;
											case 419:
												num3 = 157 - 114;
												num = 471;
												break;
											case 100:
												array7[2] = 141;
												num4 = 523;
												goto IL_4a61_2;
											case 463:
												if (num32 > 0)
												{
													num = 542;
													break;
												}
												goto IL_3834_2;
											case 368:
												num41 = 0;
												num = 430;
												break;
											case 434:
												array11 = array;
												num = 598;
												if (true)
												{
													break;
												}
												goto case 424;
											case 424:
												array3[5] = 106;
												num4 = 520;
												goto IL_4a61_2;
											case 338:
											{
												uint num40 = 4059231220u;
												num2 = 266;
												continue;
											}
											case 263:
												num6 = 65 + 76;
												num4 = 408;
												goto IL_4a61_2;
											case 327:
												array7[23] = 72;
												num = 61;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 530;
											case 530:
												_ = 108;
												_ = 78;
												num9 = /*Error near IL_3d73: Stack underflow*/+ /*Error near IL_3d73: Stack underflow*/;
												num4 = 545;
												goto IL_4a61_2;
											case 488:
												HYpd1oF0heYCHUYr2Q(xUQfefaWFJbhDoECkW(j8hgmZJ7n));
												array14 = (byte[])/*Error near IL_3d92: Stack underflow*/;
												num2 = 322;
												continue;
											case 517:
												array2[13] = (byte)num5;
												num4 = 17;
												goto IL_4a61_2;
											case 427:
												num6 = 174 - 58;
												_ = 614;
												goto IL_4a61_2;
											case 215:
												text = (string)eKmFLmAUTIA4RBnjHCA(FBQwxZAAAGt3s3HSZ3l(), array3);
												num = 353;
												break;
											case 457:
												num9 = 205 - 68;
												num2 = 252;
												continue;
											case 141:
												num5 = 217 - 72;
												num2 = 416;
												continue;
											case 630:
												array7[18] = 120;
												num = 362;
												break;
											case 202:
												value = sYNV1XAk8wX0pLjqgmD(intPtr6);
												num4 = 357;
												goto IL_4a61_2;
											case 287:
												num6 = 15 + 76;
												num2 = 289;
												continue;
											case 609:
												num5 = 72 + 93;
												num4 = 207;
												goto IL_4a61_2;
											case 601:
												_ = 1;
												_ = 108;
												((sbyte[])/*Error near IL_3e90: Stack underflow*/)[/*Error near IL_3e90: Stack underflow*/] = (sbyte)/*Error near IL_3e90: Stack underflow*/;
												num = 163;
												break;
											case 236:
												array7[14] = (byte)num3;
												num4 = 71;
												goto IL_4a61_2;
											case 549:
												array7[27] = 181;
												num4 = 399;
												goto IL_4a61_2;
											case 478:
												_ = 19;
												_ = 126;
												_ = 81;
												val4 = /*Error near IL_3ee9: Stack underflow*/- /*Error near IL_3ee9: Stack underflow*/;
												((sbyte[])/*Error near IL_3eea: Stack underflow*/)[/*Error near IL_3eea: Stack underflow*/] = (sbyte)(int)val4;
												num = 365;
												if (0 == 0)
												{
													break;
												}
												goto case 610;
											case 610:
												array2[15] = 146;
												num2 = 238;
												continue;
											case 340:
												uu349Vtr = (byte[])u2tFaHurfr1s9xSbVJ(binaryReader, num30);
												num4 = 174;
												goto IL_4a61_2;
											case 418:
												return;
											case 54:
												num5 = 221 - 73;
												num2 = 284;
												continue;
											case 391:
												array2[15] = 133;
												num = 610;
												break;
											case 175:
												array7[21] = (byte)num6;
												num = 98;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 600;
											case 600:
												array11[num7 + 7] = array10[7];
												num = 59;
												if (true)
												{
													break;
												}
												goto case 308;
											case 308:
												array13[1] = array14[0];
												num2 = 493;
												continue;
											case 542:
												num39 = num38 ^ num10;
												num = 475;
												break;
											case 230:
												array2 = new byte[16];
												num = 409;
												break;
											case 515:
												if (num20 == 4)
												{
													_ = 503;
													num = (int)/*Error near IL_4009: Stack underflow*/;
													if (0 == 0)
													{
														break;
													}
													goto case 98;
												}
												goto case 347;
											case 98:
												num6 = 192 - 64;
												num4 = 332;
												goto IL_4a61_2;
											case 343:
												array3 = new byte[10];
												num = 151;
												break;
											case 317:
												array7[9] = 111;
												num4 = 307;
												goto IL_4a61_2;
											case 445:
												yMayDYsjD(intPtr5, 4, 4, ref num37);
												num2 = 397;
												continue;
											case 15:
												if (num32 > 0)
												{
													num = 170;
													break;
												}
												goto IL_2e86_2;
											case 158:
												array7[24] = 227;
												num4 = 556;
												goto IL_4a61_2;
											case 554:
												num5 = 178 - 59;
												num = 517;
												if (0 == 0)
												{
													break;
												}
												goto case 291;
											case 186:
											case 410:
												if (num34 >= num35)
												{
													num2 = 426;
													continue;
												}
												intPtr4.ToInt64();
												M0UK57Lct29oQf1y7M(new IntPtr((long)(/*Error near IL_1086: Stack underflow*/ + num34 * 4)), NHZGRo0SaytvR3NXnJ(binaryReader));
												num = 425;
												if (true)
												{
													break;
												}
												goto case 392;
											case 481:
												num5 = 60 + 89;
												num2 = 575;
												continue;
											case 129:
											{
												int num33 = 0;
												num2 = 529;
												continue;
											}
											case 496:
												_ = 130;
												_ = 91;
												num6 = /*Error near IL_4134: Stack underflow*/- /*Error near IL_4134: Stack underflow*/;
												num2 = 582;
												continue;
											case 467:
												num5 = 54 + 32;
												num4 = 117;
												goto IL_4a61_2;
											case 605:
												try
												{
													bFB44BUGlg = (rL2N9N6wh7IWY3IC3G)krLZHPATvDcIfeOggP9(new IntPtr(num8), rEacmnz8ipU2tXY6jc(typeof(rL2N9N6wh7IWY3IC3G).TypeHandle));
												}
												catch (object obj4)
												{
													try
													{
														Delegate @delegate = (Delegate)krLZHPATvDcIfeOggP9(new IntPtr(num8), rEacmnz8ipU2tXY6jc(typeof(rL2N9N6wh7IWY3IC3G).TypeHandle));
														bFB44BUGlg = (rL2N9N6wh7IWY3IC3G)T5KjoaAD0DEhr9ndv3K(rEacmnz8ipU2tXY6jc(typeof(rL2N9N6wh7IWY3IC3G).TypeHandle), j5I56qAWJhTHkpG09e6(@delegate));
													}
													catch (object obj3)
													{
													}
												}
												_ = IntPtr.Zero;
												num = 97;
												if (true)
												{
													break;
												}
												goto case 172;
											case 172:
												num32 = array9.Length % 4;
												num = 303;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 356;
											case 550:
												array12 = (byte[])HfDHTTAN2uenSb31bPn(intPtr3.ToInt64());
												num = 587;
												if (0 == 0)
												{
													break;
												}
												goto case 503;
											case 97:
												num31 = 0;
												num2 = 320;
												continue;
											case 535:
											case 562:
												array3[6] = 46;
												num = 458;
												if (0 == 0)
												{
													break;
												}
												goto case 92;
											case 92:
												intPtr3 = hKbI6SAlKF8QuASWEYm(x3c4o2PyTx);
												_ = 133;
												goto IL_4a61_2;
											case 168:
												array7[18] = 43;
												num2 = 630;
												continue;
											case 632:
												array2[12] = (byte)num9;
												num = 167;
												break;
											case 313:
												array7[26] = (byte)num6;
												num2 = 549;
												continue;
											case 64:
												array11[num21 + 2] = array16[2];
												num2 = 46;
												continue;
											case 557:
												array7[31] = 118;
												num2 = 594;
												continue;
											case 351:
												array11[num7 + 2] = array16[2];
												num = 504;
												if (0 == 0)
												{
													break;
												}
												goto case 406;
											case 406:
												array2[11] = 119;
												num2 = 211;
												continue;
											case 417:
												array7[6] = 158;
												num2 = 536;
												continue;
											case 360:
												num9 = 187 - 63;
												num4 = 618;
												goto IL_4a61_2;
											case 180:
											case 369:
												if (num27 < num28)
												{
													intPtr2 = new IntPtr(num16 + NHZGRo0SaytvR3NXnJ(binaryReader) - num29);
													num = 595;
													break;
												}
												num2 = 63;
												continue;
											case 55:
												array2[4] = 176;
												num = 51;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 94;
											case 94:
												num9 = 97 + 71;
												num2 = 13;
												continue;
											case 162:
												goto end_IL_02f5;
											case 193:
												array7[31] = 93;
												_ = 557;
												num = (int)/*Error near IL_446b: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 437;
											case 437:
												array2[15] = 137;
												_ = 31;
												num = (int)/*Error near IL_448f: Stack underflow*/;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 246;
											case 246:
												array7[28] = (byte)num3;
												num2 = 58;
												continue;
											case 268:
												array2[2] = (byte)num9;
												num = 300;
												break;
											case 238:
												_ = 15;
												_ = 84;
												_ = 84;
												val2 = /*Error near IL_44e1: Stack underflow*/+ /*Error near IL_44e1: Stack underflow*/;
												((sbyte[])/*Error near IL_44e2: Stack underflow*/)[/*Error near IL_44e2: Stack underflow*/] = (sbyte)(int)val2;
												num2 = 116;
												continue;
											case 336:
												num6 = 16 + 5;
												num = 65;
												if (true)
												{
													break;
												}
												goto case 576;
											case 576:
												array7[18] = 93;
												num = 168;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 282;
											case 282:
												num26 = (uint)((array15[num23 + 3] << 24) | (array15[num23 + 2] << 16) | (array15[num23 + 1] << 8) | array15[num23]);
												num = 615;
												break;
											case 636:
												OlnCOhV4y6WcWPCFau((object)/*Error near IL_4575: Stack underflow*/);
												num = 272;
												break;
											case 11:
												num3 = 175 - 58;
												_ = 255;
												num = (int)/*Error near IL_459b: Stack underflow*/;
												break;
											case 493:
												array13[3] = array14[1];
												num2 = 512;
												continue;
											case 520:
												array3[6] = 105;
												num = 210;
												if (true)
												{
													break;
												}
												goto case 30;
											case 429:
												array11[num7 + 5] = array10[5];
												num = 1;
												if (true)
												{
													break;
												}
												goto case 306;
											case 306:
												num6 = 106 + 99;
												_ = 509;
												goto IL_4a61_2;
											case 335:
												num29 = 7168;
												num = 264;
												break;
											case 213:
												array7[28] = 94;
												num = 483;
												break;
											case 77:
												array7[2] = (byte)num3;
												num2 = 240;
												continue;
											case 639:
												if (array14.Length <= 0)
												{
													goto case 476;
												}
												num = 308;
												if (!YiHWqdAEL2s4JBasBe())
												{
													goto case 115;
												}
												break;
											case 21:
												array2[14] = (byte)num5;
												num = 398;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 193;
											case 405:
												array7[9] = (byte)num3;
												num4 = 378;
												goto IL_4a61_2;
											case 161:
												num24 = num21 + 3;
												num25 = array12[3];
												((sbyte[])/*Error near IL_46fa: Stack underflow*/)[num24] = (sbyte)num25;
												num = 10;
												break;
											case 50:
												num10 = (uint)((array9[num23 + 3] << 24) | (array9[num23 + 2] << 16) | (array9[num23 + 1] << 8) | array9[num23]);
												num4 = 22;
												goto IL_4a61_2;
											case 330:
											case 621:
												_ = 4;
												_ = ref num37;
												yMayDYsjD((IntPtr)/*Error near IL_474f: Stack underflow*/, (int)/*Error near IL_474f: Stack underflow*/, (int)/*Error near IL_474f: Stack underflow*/, ref *(int*)/*Error near IL_474f: Stack underflow*/);
												num2 = 208;
												continue;
											case 240:
												array7[2] = 160;
												num = 74;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 469;
											case 432:
												num6 = 187 - 84;
												num = 112;
												if (0 == 0)
												{
													break;
												}
												goto case 266;
											case 266:
												num11 = 0L;
												_ = 384;
												num = (int)/*Error near IL_47b2: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 247;
											case 247:
												array2[14] = (byte)num9;
												num = 309;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 319;
											case 319:
												_ = num21 + 3;
												num22 = ((byte[])/*Error near IL_47e6: Stack underflow*/)[3];
												((sbyte[])/*Error near IL_47e7: Stack underflow*/)[/*Error near IL_47e7: Stack underflow*/] = (sbyte)num22;
												num = 187;
												if (true)
												{
													break;
												}
												goto case 547;
											case 91:
												array11[num7 + 2] = array12[2];
												num2 = 563;
												continue;
											case 297:
												_ = array9.Length;
												_ = 1;
												val22 = /*Error near IL_482f: Stack underflow*/+ /*Error near IL_482f: Stack underflow*/;
												val23 = /*Error near IL_4830: Stack underflow*/- val22;
												num94 = ((byte[])/*Error near IL_4831: Stack underflow*/)[val23];
												num10 = (uint)(/*Error near IL_4832: Stack underflow*/ | num94);
												num2 = 513;
												continue;
											case 413:
												tlfI1xoeKCs7nuslSe(array5, 0, sKOIhjrvKfSLPpeKTp(8), 1);
												_ = 572;
												num = (int)/*Error near IL_4856: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 599;
											case 599:
												num20 = NHZGRo0SaytvR3NXnJ((object)/*Error near IL_4866: Stack underflow*/);
												num = 347;
												break;
											case 299:
												intPtr3 = IntPtr.Zero;
												num4 = 92;
												goto IL_4a61_2;
											case 473:
												lk7BwHKFmNJY32ZC3n2.bV44XU8KQo = false;
												num4 = 122;
												goto IL_4a61_2;
											case 40:
												num18 = num7 + 4;
												num19 = array10[4];
												((sbyte[])/*Error near IL_48a7: Stack underflow*/)[num18] = (sbyte)num19;
												num4 = 429;
												goto IL_4a61_2;
											case 479:
												num3 = 175 - 58;
												num4 = 315;
												goto IL_4a61_2;
											case 159:
											case 507:
												HifqVVy4sMTU0tX2Zq(IBe4hEip2A, num16 + num17, lk7BwHKFmNJY32ZC3n);
												num2 = 279;
												continue;
											case 303:
												num15 = array9.Length / 4;
												num2 = 345;
												continue;
											case 262:
												array7[22] = 98;
												num = 80;
												if (0 == 0)
												{
													break;
												}
												goto case 411;
											case 411:
												jR652Ijtdrg0nRlgXv(binaryReader);
												num2 = 278;
												continue;
											case 540:
												array8[num12 + 3] = (byte)((num13 & 0xFF000000u) >> 24);
												num = 220;
												if (0 == 0)
												{
													break;
												}
												goto case 68;
											case 68:
												num14 = 0;
												_ = 179;
												continue;
											case 31:
												num9 = 103 + 40;
												num = 82;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 633;
											case 633:
												num9 = 91 + 34;
												num4 = 441;
												goto IL_4a61_2;
											case 534:
												array7[20] = (byte)num6;
												num = 138;
												break;
											case 558:
												array7[12] = (byte)num3;
												num = 287;
												break;
											case 301:
												array7[13] = 91;
												num = 123;
												break;
											case 590:
												num5 = 44 - 25;
												goto case 387;
											case 355:
												array8[num12 + 1] = (byte)((num13 & 0xFF00) >> 8);
												num4 = 394;
												goto IL_4a61_2;
											case 347:
												if (num20 != 1)
												{
													num27 = 0;
													num4 = 180;
												}
												else
												{
													num4 = 514;
												}
												goto IL_4a61_2;
											case 367:
												array2[6] = (byte)num9;
												num = 333;
												SFlWP1U3TGooXTabny();
												if ((int)/*Error near IL_6020: Stack underflow*/ == 0)
												{
													break;
												}
												goto case 529;
											case 640:
												array7[5] = (byte)num3;
												num4 = 456;
												goto IL_4a61_2;
											case 341:
												array7[0] = 70;
												num2 = 619;
												continue;
											case 494:
												array2[8] = (byte)num9;
												num4 = 633;
												goto IL_4a61_2;
											case 371:
												num3 = 86 + 2;
												num4 = 596;
												goto IL_4a61_2;
											case 435:
												num3 = 167 - 55;
												num4 = 148;
												goto IL_4a61_2;
											case 629:
												array6 = (array5 = array4);
												if (array6 != null)
												{
													num2 = 265;
													continue;
												}
												goto end_IL_4a69;
											case 318:
												num27++;
												num4 = 369;
												goto IL_4a61_2;
											case 642:
												return;
											case 167:
												_ = 177;
												num9 = /*Error near IL_0044: Stack underflow*/- 59;
												_ = 508;
												num = (int)/*Error near IL_0051: Stack underflow*/;
												if (true)
												{
													break;
												}
												goto case 510;
											case 139:
												_ = ref num11;
												new IntPtr((void*)(nuint)/*Error near IL_029d: Stack underflow*/);
												sKsK3blfysN6UFS97t((IntPtr)/*Error near IL_02aa: Stack underflow*/, 0, 0L);
												_ = 389;
												goto IL_4a61_2;
											case 572:
												J8P1jmtmeLeJHAXE9m();
												num4 = 415;
												goto IL_4a61_2;
											case 433:
												NHZGRo0SaytvR3NXnJ((object)/*Error near IL_5691: Stack underflow*/);
												num30 = (int)/*Error near IL_0532: Stack underflow*/;
												num2 = 340;
												continue;
											case 122:
												_ = IBe4hEip2A;
												obj = 0L;
												obj2 = lk7BwHKFmNJY32ZC3n2;
												HifqVVy4sMTU0tX2Zq((object)/*Error near IL_056d: Stack underflow*/, obj, obj2);
												_ = 612;
												continue;
											case 273:
												_ = IntPtr.Zero;
												intPtr = (IntPtr)/*Error near IL_0abe: Stack underflow*/;
												_ = 217;
												goto IL_4a61_2;
											case 69:
												_ = 56;
												wDRJe2H6E4HVV6PGZs = (WDRJe2H6E4HVV6PGZs)/*Error near IL_11ae: Stack underflow*/;
												num = 580;
												break;
											case 626:
												_ = 11;
												((sbyte[])/*Error near IL_1269: Stack underflow*/)[/*Error near IL_1269: Stack underflow*/] = (sbyte)num9;
												num = 79;
												break;
											case 25:
												_ = 7;
												((sbyte[])/*Error near IL_129d: Stack underflow*/)[/*Error near IL_129d: Stack underflow*/] = 97;
												num2 = 496;
												continue;
											case 402:
												_ = 21;
												((sbyte[])/*Error near IL_12fd: Stack underflow*/)[/*Error near IL_12fd: Stack underflow*/] = 47;
												num = 551;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 294;
											case 608:
												_ = hSjGubHK9;
												if ((int)/*Error near IL_1369: Stack underflow*/ != 0)
												{
													return;
												}
												_ = 468;
												num = (int)/*Error near IL_1372: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 421;
											case 85:
												_ = 2;
												((sbyte[])/*Error near IL_13d6: Stack underflow*/)[/*Error near IL_13d6: Stack underflow*/] = -119;
												_ = 477;
												num = (int)/*Error near IL_13df: Stack underflow*/;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 109;
											case 484:
												_ = 106;
												num6 = /*Error near IL_1430: Stack underflow*/+ 2;
												num4 = 531;
												goto IL_4a61_2;
											case 200:
												_ = 10;
												((sbyte[])/*Error near IL_174c: Stack underflow*/)[/*Error near IL_174c: Stack underflow*/] = 91;
												num2 = 452;
												continue;
											case 29:
												_ = 23;
												((sbyte[])/*Error near IL_18e9: Stack underflow*/)[/*Error near IL_18e9: Stack underflow*/] = (sbyte)num6;
												num = 490;
												if (true)
												{
													break;
												}
												goto case 532;
											case 5:
												_ = 9;
												((sbyte[])/*Error near IL_1936: Stack underflow*/)[/*Error near IL_1936: Stack underflow*/] = (sbyte)num5;
												num4 = 481;
												goto IL_4a61_2;
											case 311:
												_ = 14;
												((sbyte[])/*Error near IL_1eab: Stack underflow*/)[/*Error near IL_1eab: Stack underflow*/] = (sbyte)num6;
												num = 145;
												break;
											case 490:
												_ = 56;
												num3 = /*Error near IL_1f6e: Stack underflow*/+ 2;
												num2 = 607;
												continue;
											case 545:
												_ = 7;
												((sbyte[])/*Error near IL_235c: Stack underflow*/)[/*Error near IL_235c: Stack underflow*/] = (sbyte)num9;
												num2 = 495;
												continue;
											case 23:
												_ = 180;
												num3 = /*Error near IL_2537: Stack underflow*/- 60;
												_ = 246;
												num = (int)/*Error near IL_2544: Stack underflow*/;
												if (true)
												{
													break;
												}
												goto case 78;
											case 219:
												_ = 104;
												num9 = /*Error near IL_276f: Stack underflow*/+ 106;
												num = 461;
												break;
											case 592:
												_ = 227;
												num9 = /*Error near IL_2de3: Stack underflow*/- 75;
												_ = 268;
												num = (int)/*Error near IL_2df0: Stack underflow*/;
												break;
											case 2:
												ROhFJh1RB((IntPtr)/*Error near IL_5c63: Stack underflow*/, (string)/*Error near IL_5c63: Stack underflow*/);
												intPtr11 = (IntPtr)/*Error near IL_3007: Stack underflow*/;
												num = 178;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 326;
											case 444:
												_ = 137;
												num3 = /*Error near IL_30d9: Stack underflow*/- 31;
												num4 = 638;
												goto IL_4a61_2;
											case 6:
												_ = 3;
												((sbyte[])/*Error near IL_32df: Stack underflow*/)[/*Error near IL_32df: Stack underflow*/] = 90;
												num2 = 227;
												continue;
											case 101:
												_ = ref lk7BwHKFmNJY32ZC3n;
												((Lk7BwHKFmNJY32ZC3n*)(nint)/*Error near IL_33d9: Stack underflow*/)->bV44XU8KQo = bV44XU8KQo;
												_ = 1;
												if (SFlWP1U3TGooXTabny())
												{
													num2 = 562;
													continue;
												}
												num = 159;
												break;
											case 619:
												_ = 1;
												((sbyte[])/*Error near IL_3646: Stack underflow*/)[/*Error near IL_3646: Stack underflow*/] = -108;
												num4 = 382;
												goto IL_4a61_2;
											case 115:
												_ = 210;
												num5 = /*Error near IL_3699: Stack underflow*/- 70;
												num4 = 524;
												goto IL_4a61_2;
											case 448:
												JLGytImuxrMxAZqJKu();
												if ((int)/*Error near IL_3d05: Stack underflow*/ != 4)
												{
													num = 164;
													break;
												}
												_ = new byte[30];
												g1cqHkAgBBeTN3XMfvl((object)/*Error near IL_43ca: Stack underflow*/, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
												array = (byte[])/*Error near IL_43d7: Stack underflow*/;
												_ = 212;
												num = (int)/*Error near IL_43e0: Stack underflow*/;
												if (0 == 0)
												{
													break;
												}
												goto case 180;
											case 291:
												_ = typeof(DyyVDbaRvM1YfIq9il).TypeHandle;
												if (KpNDnm3q1r6YoZRF9H(lGqEHaRDPgHFpusu4D(rEacmnz8ipU2tXY6jc((RuntimeTypeHandle)/*Error near IL_40d1: Stack underflow*/).Assembly)) <= 0)
												{
													goto IL_33f4;
												}
												num4 = 334;
												goto IL_4a61_2;
											case 503:
												iW4joX4pRXvcmAix5k();
												symmetricAlgorithm = (SymmetricAlgorithm)/*Error near IL_4222: Stack underflow*/;
												num2 = 226;
												continue;
											case 561:
												_ = 6;
												_ = 133;
												val = /*Error near IL_4240: Stack underflow*/- 44;
												((sbyte[])/*Error near IL_4241: Stack underflow*/)[/*Error near IL_4241: Stack underflow*/] = (sbyte)(int)val;
												num4 = 115;
												goto IL_4a61_2;
											case 59:
												_ = 30;
												num7 = (int)/*Error near IL_4373: Stack underflow*/;
												num = 331;
												if (!SFlWP1U3TGooXTabny())
												{
													break;
												}
												goto case 417;
											case 547:
												_ = ref lk7BwHKFmNJY32ZC3n2;
												*(Lk7BwHKFmNJY32ZC3n*)(nint)/*Error near IL_47fd: Stack underflow*/ = default(Lk7BwHKFmNJY32ZC3n);
												num4 = 244;
												goto IL_4a61_2;
											case 177:
												_ = 20;
												((sbyte[])/*Error near IL_48d8: Stack underflow*/)[/*Error near IL_48d8: Stack underflow*/] = (sbyte)num6;
												num = 127;
												break;
											case 128:
												_ = 5;
												((sbyte[])/*Error near IL_491a: Stack underflow*/)[/*Error near IL_491a: Stack underflow*/] = (sbyte)num6;
												num2 = 565;
												continue;
											case 207:
												_ = 1;
												((sbyte[])/*Error near IL_49ff: Stack underflow*/)[/*Error near IL_49ff: Stack underflow*/] = (sbyte)num5;
												num2 = 156;
												continue;
											default:
												_ = 387;
												num = (int)/*Error near IL_5483: Stack underflow*/;
												if (YiHWqdAEL2s4JBasBe())
												{
													break;
												}
												goto case 298;
											case 356:
												_ = phV4Uu6SUx;
												Qwp4ejR7FG = NGHo4rAHSuSQ4ufCkBP((long)/*Error near IL_54f0: Stack underflow*/);
												num4 = 90;
												goto IL_4a61_2;
											case 523:
												{
													_ = 50;
													num3 = /*Error near IL_5559: Stack underflow*/+ 49;
													num4 = 77;
													goto IL_4a61_2;
												}
												IL_33f4:
												try
												{
													object obj5 = HTMLgVAIg6ADBaOAp5x(KHBXOyA2J0TRDNUEb4n(XSYACKAmx9CEGKYuKSC(v0TnNMA8esHLQWVTRyb(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly))).GetField("m_ptr", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), XSYACKAmx9CEGKYuKSC(v0TnNMA8esHLQWVTRyb(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly)));
													if (obj5 is IntPtr)
													{
														XtL4lyIIgx = (IntPtr)obj5;
													}
													if (qNGJ4OfmSpfpcWEKvj(obj5.GetType().ToString(), "System.Reflection.RuntimeModule"))
													{
														XtL4lyIIgx = (IntPtr)HTMLgVAIg6ADBaOAp5x(obj5.GetType().GetField("m_pData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), obj5);
													}
													MemoryStream memoryStream2 = new MemoryStream();
													cImcIssX0mqXP11GuB(memoryStream2, new byte[JLGytImuxrMxAZqJKu()], 0, JLGytImuxrMxAZqJKu());
													if (JLGytImuxrMxAZqJKu() == 4)
													{
														cImcIssX0mqXP11GuB(memoryStream2, NelyXWhgfYRGJe9y0i(XtL4lyIIgx.ToInt32()), 0, 4);
													}
													else
													{
														cImcIssX0mqXP11GuB(memoryStream2, HfDHTTAN2uenSb31bPn(XtL4lyIIgx.ToInt64()), 0, 8);
													}
													cImcIssX0mqXP11GuB(memoryStream2, new byte[JLGytImuxrMxAZqJKu()], 0, JLGytImuxrMxAZqJKu());
													cImcIssX0mqXP11GuB(memoryStream2, new byte[JLGytImuxrMxAZqJKu()], 0, JLGytImuxrMxAZqJKu());
													uv67RaQSbjWNCiTI1l(memoryStream2, 0L);
													byte[] array18 = (byte[])AJs46ZSqBoMlDC1IIM(memoryStream2);
													mGMWpQe4ChoBUxrIed(memoryStream2);
													uint nativeSizeOfCode = 0u;
													try
													{
														array19 = (array5 = array18);
														if (array19 != null)
														{
															_ = 1;
															if (!SFlWP1U3TGooXTabny())
															{
																int num55;
																num56 = (num55 = 4);
																num57 = num55;
															}
															else
															{
																num57 = 3;
															}
															while (true)
															{
																switch (num57)
																{
																case 1:
																	break;
																case 0:
																case 4:
																	goto IL_35c3;
																default:
																	goto IL_35d6;
																}
																break;
																IL_35c3:
																if (array5.Length == 0)
																{
																	num57 = 1;
																	continue;
																}
																while (true)
																{
																	IL_35af:
																	fixed (byte* value2 = &array5[0])
																	{
																		int num55;
																		num56 = (num55 = 5);
																		num57 = num55;
																		while (true)
																		{
																			switch (num57)
																			{
																			case 1:
																				break;
																			case 0:
																			case 4:
																				if (array5.Length != 0)
																				{
																					goto IL_35af;
																				}
																				goto IL_35cc;
																			default:
																				x3c4o2PyTx(new IntPtr(value2), new IntPtr(value2), new IntPtr(value2), 216669565u, new IntPtr(value2), ref nativeSizeOfCode);
																				goto end_IL_35af;
																			}
																			break;
																			IL_35cc:
																			num57 = 1;
																		}
																	}
																	goto end_IL_357d;
																	continue;
																	end_IL_35af:
																	break;
																}
																goto end_IL_3547;
																continue;
																end_IL_357d:
																break;
															}
														}
														reference = ref *(byte*)null;
														goto IL_35d6;
														IL_35d6:
														x3c4o2PyTx(new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), 216669565u, new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), ref nativeSizeOfCode);
														end_IL_3547:;
													}
													finally
													{
														reference = ref *(byte*)null;
													}
												}
												catch (object obj6)
												{
												}
												FqA3fjAY4CaOvkoaMo3(bFB44BUGlg);
												num2 = 571;
												continue;
												IL_2e86_2:
												num23 = (uint)num12;
												num2 = 225;
												continue;
												IL_1941_2:
												if (JLGytImuxrMxAZqJKu() == 4)
												{
													num = 171;
													break;
												}
												goto IL_3b9e;
												IL_4a61_2:
												num = num4;
												break;
												IL_3b9e:
												binaryReader = new BinaryReader((Stream)rsW1hDE96lSnfJEg3D(j8hgmZJ7n, "\u008fcu\u008c\u0092m\u008d\u0097c\u009b\u008bmu\u00914\u0089ur.fa\u009574s\u008e\u009e\u0088d5\u0097s\u0090tm6j"));
												num4 = 546;
												goto IL_4a61_2;
												IL_3834_2:
												num13 = num38 ^ num10;
												num4 = 259;
												goto IL_4a61_2;
											}
											goto IL_4a65_2;
											continue;
											end_IL_4a69:
											break;
										}
										break;
									}
								}
								goto case 516;
								continue;
								end_IL_02f5:
								break;
							}
							goto case 162;
						}
						num = 516;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 639;
					case 331:
						num58 = array12[0];
						((sbyte[])/*Error near IL_2c54: Stack underflow*/)[/*Error near IL_2c54: Stack underflow*/] = (sbyte)num58;
						num = 501;
						if (true)
						{
							break;
						}
						goto case 196;
					case 196:
						array2[4] = 80;
						num4 = 579;
						goto IL_4a61;
					case 462:
						array7[6] = (byte)num6;
						num2 = 417;
						continue;
					case 116:
						array13 = array2;
						num4 = 218;
						goto IL_4a61;
					case 628:
						num3 = 138 - 46;
						num = 4;
						break;
					case 250:
						array7[30] = (byte)num6;
						num = 352;
						break;
					case 486:
						array7[29] = 160;
						num2 = 372;
						continue;
					case 105:
						lGqEHaRDPgHFpusu4D(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly);
						if ((int)/*Error near IL_2d1b: Stack underflow*/ != 0)
						{
							num2 = 291;
							continue;
						}
						goto IL_33f4_2;
					case 136:
						array7[11] = (byte)num6;
						num4 = 263;
						goto IL_4a61;
					case 10:
					case 506:
						tlfI1xoeKCs7nuslSe(array11, 0, intPtr7, array11.Length);
						num2 = 248;
						continue;
					case 582:
						array7[7] = (byte)num6;
						num = 500;
						if (true)
						{
							break;
						}
						goto case 259;
					case 259:
						((sbyte[])/*Error near IL_2d83: Stack underflow*/)[num12] = (sbyte)(byte)(num13 & 0xFF);
						num4 = 355;
						goto IL_4a61;
					case 564:
						NHZGRo0SaytvR3NXnJ(binaryReader);
						M0UK57Lct29oQf1y7M((IntPtr)/*Error near IL_2d9b: Stack underflow*/, (int)/*Error near IL_2d9b: Stack underflow*/);
						num2 = 342;
						continue;
					case 436:
						array2[11] = (byte)num9;
						num2 = 428;
						continue;
					case 505:
						intPtr9.ToInt64();
						phV4Uu6SUx = (long)/*Error near IL_2dc9: Stack underflow*/;
						num = 449;
						if (0 == 0)
						{
							break;
						}
						goto case 592;
					case 487:
						intPtr6 = HfSOpPAiXrYiEn2URpZ(lhmiV9AUoOr1v5yhIs);
						num = 519;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 456;
					case 60:
						num54++;
						num2 = 584;
						continue;
					case 190:
						num6 = 77 + 10;
						num = 469;
						break;
					case 131:
						num23 = 0u;
						num4 = 68;
						goto IL_4a61;
					case 548:
						num3 = 178 + 58;
						num4 = 405;
						goto IL_4a61;
					case 142:
						array2[5] = (byte)num9;
						num4 = 270;
						goto IL_4a61;
					case 14:
						intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(j8hgmZJ7n))[0]);
						num4 = 570;
						goto IL_4a61;
					case 146:
						array3[0] = 109;
						num = 288;
						if (0 == 0)
						{
							break;
						}
						goto case 430;
					case 283:
						array7[16] = 90;
						num = 221;
						if (true)
						{
							break;
						}
						goto case 591;
					case 591:
						array7[16] = (byte)num3;
						num2 = 306;
						continue;
					case 422:
						num52 = num7 + 6;
						num53 = array12[6];
						((sbyte[])/*Error near IL_2f2b: Stack underflow*/)[num52] = (sbyte)num53;
						_ = 364;
						goto IL_4a61;
					case 634:
						array11[num7 + 1] = array10[1];
						num2 = 78;
						continue;
					case 477:
						num6 = 244 - 81;
						num4 = 459;
						goto IL_4a61;
					case 509:
						array7[16] = (byte)num6;
						num4 = 283;
						goto IL_4a61;
					case 198:
						array7[16] = 219;
						num4 = 400;
						goto IL_4a61;
					case 148:
						array7[8] = (byte)num3;
						num4 = 153;
						goto IL_4a61;
					case 528:
						num6 = 97 + 16;
						num = 534;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 171;
					case 171:
						if (!sPFYCLItnSewrnGJfh(gKjkGS27vZd6ReTL7N("System.Reflection.ReflectionContext", false), null))
						{
							goto IL_3b9e_2;
						}
						num = 613;
						if (true)
						{
							break;
						}
						goto case 2;
					case 326:
						num6 = 67 + 47;
						num2 = 201;
						continue;
					case 491:
						array7[1] = (byte)num3;
						num4 = 403;
						goto IL_4a61;
					case 382:
						num3 = 146 - 48;
						num2 = 491;
						continue;
					case 296:
						array7 = new byte[32];
						num2 = 190;
						continue;
					case 380:
						num3 = 97 + 40;
						num = 511;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 227;
					case 227:
						array2[3] = 149;
						num4 = 109;
						goto IL_4a61;
					case 620:
						mGMWpQe4ChoBUxrIed(memoryStream);
						num4 = 199;
						goto IL_4a61;
					case 612:
						bV44XU8KQo = false;
						num4 = 414;
						goto IL_4a61;
					case 278:
						num28 = NHZGRo0SaytvR3NXnJ(binaryReader);
						num4 = 599;
						goto IL_4a61;
					case 499:
						array3[3] = 111;
						num4 = 38;
						goto IL_4a61;
					case 292:
						_ = 2;
						_ = 79;
						_ = 71;
						val8 = /*Error near IL_312b: Stack underflow*/+ /*Error near IL_312b: Stack underflow*/;
						((sbyte[])/*Error near IL_312c: Stack underflow*/)[/*Error near IL_312c: Stack underflow*/] = (sbyte)(int)val8;
						num = 592;
						if (true)
						{
							break;
						}
						goto case 28;
					case 28:
						array4 = array8;
						num2 = 392;
						continue;
					case 321:
						_ = 27;
						_ = 105;
						_ = 40;
						val7 = /*Error near IL_315d: Stack underflow*/- /*Error near IL_315d: Stack underflow*/;
						((sbyte[])/*Error near IL_315e: Stack underflow*/)[/*Error near IL_315e: Stack underflow*/] = (sbyte)(int)val7;
						num4 = 427;
						goto IL_4a61;
					case 130:
						FqA3fjAY4CaOvkoaMo3(x3c4o2PyTx);
						num2 = 118;
						continue;
					case 137:
					case 212:
						intPtr7 = jMyYFyWuy(IntPtr.Zero, (uint)array.Length, 4096u, 64u);
						num = 434;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 299;
					case 293:
						array17 = (byte[])u2tFaHurfr1s9xSbVJ(binaryReader, (int)OhZpyDwZxpiyDh5BOJ(QX5YKsJaRCBODNJawM(binaryReader)));
						num2 = 296;
						continue;
					case 384:
						tg8t5OqN45OAVd2Uq7(new IntPtr(&num11), 0);
						num = 323;
						if (0 == 0)
						{
							break;
						}
						goto case 446;
					case 446:
						array11[num7] = array16[0];
						num2 = 555;
						continue;
					case 279:
					case 414:
						IUSsYAngpyKANEnZ1U(QX5YKsJaRCBODNJawM(binaryReader));
						QX5YKsJaRCBODNJawM(binaryReader);
						num51 = OhZpyDwZxpiyDh5BOJ((object)/*Error near IL_3214: Stack underflow*/);
						if ((long)/*Error near IL_321c: Stack underflow*/ >= num51 - 1)
						{
							intPtr9 = qy8E1Jx0giZMnHMioa(((object[])w93lZh9ZsNOKSgl1T8(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly))[0]);
							num2 = 505;
							continue;
						}
						num4 = 395;
						goto IL_4a61;
					case 93:
						array2[13] = (byte)num5;
						num2 = 54;
						continue;
					case 385:
						if (KpNDnm3q1r6YoZRF9H(lGqEHaRDPgHFpusu4D(j8hgmZJ7n)) != 0)
						{
							goto case 264;
						}
						num4 = 335;
						goto IL_4a61;
					case 218:
						KKPSQdOpi1WKW3jfUr(array13);
						num = 488;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 397;
					case 397:
						if (JLGytImuxrMxAZqJKu() != 4)
						{
							T7LBbJ4ta(intPtr8, intPtr5, (byte[])NelyXWhgfYRGJe9y0i(NHZGRo0SaytvR3NXnJ(binaryReader)), 4u, out intPtr);
							num2 = 621;
						}
						else
						{
							num2 = 533;
						}
						continue;
					case 622:
						array7[3] = 51;
						num = 253;
						if (SFlWP1U3TGooXTabny())
						{
							goto case 10;
						}
						break;
					case 206:
						array13[7] = array14[3];
						num2 = 87;
						continue;
					case 364:
						array11[num7 + 7] = array12[7];
						num = 506;
						if (true)
						{
							break;
						}
						goto case 41;
					case 41:
						array13[13] = array14[6];
						num = 366;
						YiHWqdAEL2s4JBasBe();
						if ((int)/*Error near IL_334a: Stack underflow*/ != 0)
						{
							break;
						}
						goto case 178;
					case 178:
						_ = (LhmiV9AUoOr1v5yhIs)krLZHPATvDcIfeOggP9(intPtr11, rEacmnz8ipU2tXY6jc(typeof(LhmiV9AUoOr1v5yhIs).TypeHandle));
						lhmiV9AUoOr1v5yhIs = (LhmiV9AUoOr1v5yhIs)/*Error near IL_3362: Stack underflow*/;
						num = 487;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 581;
					case 581:
						num20 = NHZGRo0SaytvR3NXnJ(binaryReader);
						num4 = 515;
						goto IL_4a61;
					case 9:
						num8 = sYNV1XAk8wX0pLjqgmD(new IntPtr(value));
						num = 641;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 556;
					case 556:
						array7[25] = 98;
						num = 354;
						break;
					case 334:
						return;
					case 595:
						_ = 4;
						_ = 4;
						_ = ref num37;
						yMayDYsjD((IntPtr)/*Error near IL_5ce6: Stack underflow*/, (int)/*Error near IL_5ce6: Stack underflow*/, (int)/*Error near IL_5ce6: Stack underflow*/, ref *(int*)/*Error near IL_5ce6: Stack underflow*/);
						num = 564;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 245;
					case 245:
						array11[num21] = array12[0];
						_ = 149;
						num = (int)/*Error near IL_3684: Stack underflow*/;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 115;
					case 114:
						array7[1] = (byte)num3;
						num = 482;
						break;
					case 307:
						_ = 1;
						_ = 110;
						num3 = /*Error near IL_36ce: Stack underflow*/+ /*Error near IL_36ce: Stack underflow*/;
						num = 329;
						break;
					case 615:
						num50 = 255u;
						num2 = 129;
						continue;
					case 497:
						num49++;
						num = 466;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 166;
					case 235:
						_ = rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly;
						if (((Array)XYFEn2AdQMDhr0ac0wL(gQ3bxJAoGy2rvkCxjGU((object)/*Error near IL_3723: Stack underflow*/))).Length == 2)
						{
							num = 105;
							break;
						}
						goto IL_33f4_2;
					case 489:
						bV44XU8KQo = false;
						num = 577;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 600;
					case 157:
						num3 = 70 + 7;
						num4 = 53;
						goto IL_4a61;
					case 49:
						num6 = 166 - 113;
						num = 527;
						break;
					case 613:
						processModuleCollection = (ProcessModuleCollection)kxxORYNQpt7JeTNfIx(XKH3U5WCf2UyQCIIya());
						num4 = 324;
						goto IL_4a61;
					case 34:
						array2[8] = (byte)num9;
						num = 234;
						break;
					case 195:
						num6 = 116 + 76;
						num4 = 311;
						goto IL_4a61;
					case 107:
						array11[num7 + 5] = array16[5];
						num4 = 401;
						goto IL_4a61;
					case 95:
						num48 = NHZGRo0SaytvR3NXnJ(binaryReader);
						num4 = 290;
						goto IL_4a61;
					case 425:
						num34++;
						num = 410;
						if (true)
						{
							break;
						}
						goto case 324;
					case 324:
						JvuscRYrEHnEfwpXlR(processModuleCollection);
						enumerator = (IEnumerator)/*Error near IL_3820: Stack underflow*/;
						num = 81;
						if (0 == 0)
						{
							break;
						}
						goto case 181;
					case 16:
						array7[5] = 95;
						num2 = 152;
						continue;
					case 134:
						num5 = 45 - 26;
						num2 = 204;
						continue;
					case 325:
						array7[27] = 142;
						num4 = 147;
						goto IL_4a61;
					case 138:
						_ = 231;
						_ = 77;
						num6 = /*Error near IL_38be: Stack underflow*/- /*Error near IL_38be: Stack underflow*/;
						num = 177;
						if (true)
						{
							break;
						}
						goto case 543;
					case 543:
						array11[num7 + 7] = array16[7];
						num4 = 203;
						goto IL_4a61;
					case 194:
						array3[11] = 108;
						num2 = 215;
						continue;
					case 596:
						array7[3] = (byte)num3;
						_ = 377;
						goto IL_4a61;
					case 274:
						intPtr10 = puGi6bKKk(text);
						num2 = 623;
						continue;
					case 357:
					case 568:
						tg8t5OqN45OAVd2Uq7(intPtr6, 0);
						num4 = 0;
						goto IL_4a61;
					case 79:
						num9 = 148 - 49;
						num2 = 439;
						continue;
					case 482:
						_ = 2;
						_ = 120;
						_ = 114;
						val6 = /*Error near IL_3962: Stack underflow*/+ /*Error near IL_3962: Stack underflow*/;
						((sbyte[])/*Error near IL_3963: Stack underflow*/)[/*Error near IL_3963: Stack underflow*/] = (sbyte)(int)val6;
						num = 85;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 295;
					case 76:
						o00Z5hGmAa4HorFX8O(new IntPtr(&num11), 0, 0);
						_ = 139;
						goto IL_4a61;
					case 394:
						array8[num12 + 2] = (byte)((num13 & 0xFF0000) >> 16);
						num2 = 540;
						continue;
					case 204:
						array2[3] = (byte)num5;
						num = 388;
						break;
					case 373:
						memoryStream = new MemoryStream();
						num4 = 390;
						goto IL_4a61;
					case 170:
						num38 = (uint)(/*Error near IL_39ec: Stack underflow*/ + /*Error near IL_39ec: Stack underflow*/);
						num2 = 120;
						continue;
					case 611:
						val5 = /*Error near IL_3a01: Stack underflow*/+ 3;
						num47 = array10[3];
						((sbyte[])/*Error near IL_3a06: Stack underflow*/)[val5] = (sbyte)num47;
						num = 40;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 464;
					case 81:
						try
						{
							while (Prhas5pe6XdsQAESvm(enumerator))
							{
								ProcessModule processModule = (ProcessModule)w040Z1ZHbG0XVqZMoU(enumerator);
								num44 = 2;
								while (true)
								{
									int num45 = num44;
									while (true)
									{
										num46 = num45;
										while (true)
										{
											switch (num46)
											{
											case 2:
												break;
											case 5:
											case 7:
												if (vAbWvNPlhhsilht7I2(version2, version))
												{
													goto IL_3a6e_2;
												}
												goto IL_3b75_2;
											case 1:
												goto IL_3a7c_2;
											case 8:
												goto end_IL_3af8_2;
											case 4:
												if (!B28kJjXJMefNoj8Ht7(version2, version3))
												{
													goto IL_3b75_2;
												}
												goto case 5;
											case 0:
											case 3:
												version = new Version(4, 0, 30319, 17921);
												goto case 4;
											default:
												num46 = 4;
												continue;
											case 6:
												goto IL_3b2f_2;
											case 9:
												goto end_IL_3af0_2;
											}
											if (qNGJ4OfmSpfpcWEKvj(ustSntgIXxQiPYwipw(z1aSaf7nHt2GSLVr8b(processModule)), "clrjit.dll"))
											{
												num46 = 6;
												continue;
											}
											goto IL_3b75_2;
											IL_3a7c_2:
											version3 = new Version(4, 0, 30319, 17020);
											_ = 1;
											num46 = (SFlWP1U3TGooXTabny() ? 7 : 0);
											continue;
											end_IL_3af8_2:
											break;
										}
										break;
										IL_3a6e_2:
										num45 = 8;
									}
									NrL10qsNW = true;
									num44 = 9;
									continue;
									IL_3b2f_2:
									version2 = new Version(pYLE2mB3h2h5Q9EWWJ(XEpsOX15JQGw1yNP1V(processModule)), ddHTLXcmUp85qbEZ5v(XEpsOX15JQGw1yNP1V(processModule)), YSPPRZbemZq4tiMetn(XEpsOX15JQGw1yNP1V(processModule)), VfX7Wy6qBCfj5QN0tS(XEpsOX15JQGw1yNP1V(processModule)));
									num44 = 1;
									continue;
									end_IL_3af0_2:
									break;
								}
								break;
								IL_3b75_2:;
							}
						}
						finally
						{
							if (enumerator is IDisposable disposable)
							{
								p8SJ0uC03Z63haWUBt(disposable);
							}
						}
						goto IL_3b9e_2;
					case 320:
						if (qGsoXYAtg5xUyI3IWm5(gQ3bxJAoGy2rvkCxjGU(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly), null))
						{
							num = 235;
							if (YiHWqdAEL2s4JBasBe())
							{
								break;
							}
							goto case 63;
						}
						goto IL_33f4_2;
					case 44:
					case 179:
						if (num14 < num15)
						{
							num43 = num14 % num42;
							num2 = 223;
						}
						else
						{
							num2 = 28;
						}
						continue;
					case 239:
						array7[3] = (byte)num3;
						num2 = 455;
						continue;
					case 524:
						array2[6] = (byte)num5;
						num = 191;
						break;
					case 419:
						num3 = 157 - 114;
						num = 471;
						break;
					case 100:
						array7[2] = 141;
						num4 = 523;
						goto IL_4a61;
					case 463:
						if (num32 > 0)
						{
							num = 542;
							break;
						}
						goto IL_3834;
					case 368:
						num41 = 0;
						num = 430;
						break;
					case 434:
						array11 = array;
						num = 598;
						if (true)
						{
							break;
						}
						goto case 424;
					case 424:
						array3[5] = 106;
						num4 = 520;
						goto IL_4a61;
					case 338:
					{
						uint num40 = 4059231220u;
						num2 = 266;
						continue;
					}
					case 263:
						num6 = 65 + 76;
						num4 = 408;
						goto IL_4a61;
					case 327:
						array7[23] = 72;
						num = 61;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 530;
					case 530:
						_ = 108;
						_ = 78;
						num9 = /*Error near IL_3d73: Stack underflow*/+ /*Error near IL_3d73: Stack underflow*/;
						num4 = 545;
						goto IL_4a61;
					case 488:
						HYpd1oF0heYCHUYr2Q(xUQfefaWFJbhDoECkW(j8hgmZJ7n));
						array14 = (byte[])/*Error near IL_3d92: Stack underflow*/;
						num2 = 322;
						continue;
					case 517:
						array2[13] = (byte)num5;
						num4 = 17;
						goto IL_4a61;
					case 427:
						num6 = 174 - 58;
						_ = 614;
						goto IL_4a61;
					case 215:
						text = (string)eKmFLmAUTIA4RBnjHCA(FBQwxZAAAGt3s3HSZ3l(), array3);
						num = 353;
						break;
					case 457:
						num9 = 205 - 68;
						num2 = 252;
						continue;
					case 141:
						num5 = 217 - 72;
						num2 = 416;
						continue;
					case 630:
						array7[18] = 120;
						num = 362;
						break;
					case 202:
						value = sYNV1XAk8wX0pLjqgmD(intPtr6);
						num4 = 357;
						goto IL_4a61;
					case 287:
						num6 = 15 + 76;
						num2 = 289;
						continue;
					case 609:
						num5 = 72 + 93;
						num4 = 207;
						goto IL_4a61;
					case 601:
						_ = 1;
						_ = 108;
						((sbyte[])/*Error near IL_3e90: Stack underflow*/)[/*Error near IL_3e90: Stack underflow*/] = (sbyte)/*Error near IL_3e90: Stack underflow*/;
						num = 163;
						break;
					case 236:
						array7[14] = (byte)num3;
						num4 = 71;
						goto IL_4a61;
					case 549:
						array7[27] = 181;
						num4 = 399;
						goto IL_4a61;
					case 478:
						_ = 19;
						_ = 126;
						_ = 81;
						val4 = /*Error near IL_3ee9: Stack underflow*/- /*Error near IL_3ee9: Stack underflow*/;
						((sbyte[])/*Error near IL_3eea: Stack underflow*/)[/*Error near IL_3eea: Stack underflow*/] = (sbyte)(int)val4;
						num = 365;
						if (0 == 0)
						{
							break;
						}
						goto case 610;
					case 610:
						array2[15] = 146;
						num2 = 238;
						continue;
					case 340:
						uu349Vtr = (byte[])u2tFaHurfr1s9xSbVJ(binaryReader, num30);
						num4 = 174;
						goto IL_4a61;
					case 418:
						return;
					case 54:
						num5 = 221 - 73;
						num2 = 284;
						continue;
					case 391:
						array2[15] = 133;
						num = 610;
						break;
					case 175:
						array7[21] = (byte)num6;
						num = 98;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 600;
					case 600:
						array11[num7 + 7] = array10[7];
						num = 59;
						if (true)
						{
							break;
						}
						goto case 308;
					case 308:
						array13[1] = array14[0];
						num2 = 493;
						continue;
					case 542:
						num39 = num38 ^ num10;
						num = 475;
						break;
					case 230:
						array2 = new byte[16];
						num = 409;
						break;
					case 515:
						if (num20 == 4)
						{
							_ = 503;
							num = (int)/*Error near IL_4009: Stack underflow*/;
							if (0 == 0)
							{
								break;
							}
							goto case 98;
						}
						goto case 347;
					case 98:
						num6 = 192 - 64;
						num4 = 332;
						goto IL_4a61;
					case 343:
						array3 = new byte[10];
						num = 151;
						break;
					case 317:
						array7[9] = 111;
						num4 = 307;
						goto IL_4a61;
					case 445:
						yMayDYsjD(intPtr5, 4, 4, ref num37);
						num2 = 397;
						continue;
					case 15:
						if (num32 > 0)
						{
							num = 170;
							break;
						}
						goto IL_2e86;
					case 158:
						array7[24] = 227;
						num4 = 556;
						goto IL_4a61;
					case 554:
						num5 = 178 - 59;
						num = 517;
						if (0 == 0)
						{
							break;
						}
						goto case 291;
					case 186:
					case 410:
						if (num34 >= num35)
						{
							num2 = 426;
							continue;
						}
						intPtr4.ToInt64();
						M0UK57Lct29oQf1y7M(new IntPtr((long)(/*Error near IL_1086: Stack underflow*/ + num34 * 4)), NHZGRo0SaytvR3NXnJ(binaryReader));
						num = 425;
						if (true)
						{
							break;
						}
						goto case 392;
					case 481:
						num5 = 60 + 89;
						num2 = 575;
						continue;
					case 129:
					{
						int num33 = 0;
						num2 = 529;
						continue;
					}
					case 496:
						_ = 130;
						_ = 91;
						num6 = /*Error near IL_4134: Stack underflow*/- /*Error near IL_4134: Stack underflow*/;
						num2 = 582;
						continue;
					case 467:
						num5 = 54 + 32;
						num4 = 117;
						goto IL_4a61;
					case 605:
						try
						{
							bFB44BUGlg = (rL2N9N6wh7IWY3IC3G)krLZHPATvDcIfeOggP9(new IntPtr(num8), rEacmnz8ipU2tXY6jc(typeof(rL2N9N6wh7IWY3IC3G).TypeHandle));
						}
						catch (object obj4)
						{
							try
							{
								Delegate @delegate = (Delegate)krLZHPATvDcIfeOggP9(new IntPtr(num8), rEacmnz8ipU2tXY6jc(typeof(rL2N9N6wh7IWY3IC3G).TypeHandle));
								bFB44BUGlg = (rL2N9N6wh7IWY3IC3G)T5KjoaAD0DEhr9ndv3K(rEacmnz8ipU2tXY6jc(typeof(rL2N9N6wh7IWY3IC3G).TypeHandle), j5I56qAWJhTHkpG09e6(@delegate));
							}
							catch (object obj3)
							{
							}
						}
						_ = IntPtr.Zero;
						num = 97;
						if (true)
						{
							break;
						}
						goto case 172;
					case 172:
						num32 = array9.Length % 4;
						num = 303;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 356;
					case 550:
						array12 = (byte[])HfDHTTAN2uenSb31bPn(intPtr3.ToInt64());
						num = 587;
						if (0 == 0)
						{
							break;
						}
						goto case 503;
					case 97:
						num31 = 0;
						num2 = 320;
						continue;
					case 535:
					case 562:
						array3[6] = 46;
						num = 458;
						if (0 == 0)
						{
							break;
						}
						goto case 92;
					case 92:
						intPtr3 = hKbI6SAlKF8QuASWEYm(x3c4o2PyTx);
						_ = 133;
						goto IL_4a61;
					case 168:
						array7[18] = 43;
						num2 = 630;
						continue;
					case 632:
						array2[12] = (byte)num9;
						num = 167;
						break;
					case 313:
						array7[26] = (byte)num6;
						num2 = 549;
						continue;
					case 64:
						array11[num21 + 2] = array16[2];
						num2 = 46;
						continue;
					case 557:
						array7[31] = 118;
						num2 = 594;
						continue;
					case 351:
						array11[num7 + 2] = array16[2];
						num = 504;
						if (0 == 0)
						{
							break;
						}
						goto case 406;
					case 406:
						array2[11] = 119;
						num2 = 211;
						continue;
					case 417:
						array7[6] = 158;
						num2 = 536;
						continue;
					case 360:
						num9 = 187 - 63;
						num4 = 618;
						goto IL_4a61;
					case 180:
					case 369:
						if (num27 < num28)
						{
							intPtr2 = new IntPtr(num16 + NHZGRo0SaytvR3NXnJ(binaryReader) - num29);
							num = 595;
							break;
						}
						num2 = 63;
						continue;
					case 55:
						array2[4] = 176;
						num = 51;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 94;
					case 94:
						num9 = 97 + 71;
						num2 = 13;
						continue;
					case 162:
						reference2 = ref *(byte*)null;
						num = 232;
						if (0 == 0)
						{
							break;
						}
						goto case 193;
					case 193:
						array7[31] = 93;
						_ = 557;
						num = (int)/*Error near IL_446b: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 437;
					case 437:
						array2[15] = 137;
						_ = 31;
						num = (int)/*Error near IL_448f: Stack underflow*/;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 246;
					case 246:
						array7[28] = (byte)num3;
						num2 = 58;
						continue;
					case 268:
						array2[2] = (byte)num9;
						num = 300;
						break;
					case 238:
						_ = 15;
						_ = 84;
						_ = 84;
						val2 = /*Error near IL_44e1: Stack underflow*/+ /*Error near IL_44e1: Stack underflow*/;
						((sbyte[])/*Error near IL_44e2: Stack underflow*/)[/*Error near IL_44e2: Stack underflow*/] = (sbyte)(int)val2;
						num2 = 116;
						continue;
					case 336:
						num6 = 16 + 5;
						num = 65;
						if (true)
						{
							break;
						}
						goto case 576;
					case 576:
						array7[18] = 93;
						num = 168;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 282;
					case 282:
						num26 = (uint)((array15[num23 + 3] << 24) | (array15[num23 + 2] << 16) | (array15[num23 + 1] << 8) | array15[num23]);
						num = 615;
						break;
					case 636:
						OlnCOhV4y6WcWPCFau((object)/*Error near IL_4575: Stack underflow*/);
						num = 272;
						break;
					case 11:
						num3 = 175 - 58;
						_ = 255;
						num = (int)/*Error near IL_459b: Stack underflow*/;
						break;
					case 493:
						array13[3] = array14[1];
						num2 = 512;
						continue;
					case 520:
						array3[6] = 105;
						num = 210;
						if (true)
						{
							break;
						}
						goto case 30;
					case 429:
						array11[num7 + 5] = array10[5];
						num = 1;
						if (true)
						{
							break;
						}
						goto case 306;
					case 306:
						num6 = 106 + 99;
						_ = 509;
						goto IL_4a61;
					case 335:
						num29 = 7168;
						num = 264;
						break;
					case 213:
						array7[28] = 94;
						num = 483;
						break;
					case 77:
						array7[2] = (byte)num3;
						num2 = 240;
						continue;
					case 639:
						if (array14.Length <= 0)
						{
							goto case 476;
						}
						num = 308;
						if (!YiHWqdAEL2s4JBasBe())
						{
							goto case 115;
						}
						break;
					case 21:
						array2[14] = (byte)num5;
						num = 398;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 193;
					case 405:
						array7[9] = (byte)num3;
						num4 = 378;
						goto IL_4a61;
					case 161:
						num24 = num21 + 3;
						num25 = array12[3];
						((sbyte[])/*Error near IL_46fa: Stack underflow*/)[num24] = (sbyte)num25;
						num = 10;
						break;
					case 50:
						num10 = (uint)((array9[num23 + 3] << 24) | (array9[num23 + 2] << 16) | (array9[num23 + 1] << 8) | array9[num23]);
						num4 = 22;
						goto IL_4a61;
					case 330:
					case 621:
						_ = 4;
						_ = ref num37;
						yMayDYsjD((IntPtr)/*Error near IL_474f: Stack underflow*/, (int)/*Error near IL_474f: Stack underflow*/, (int)/*Error near IL_474f: Stack underflow*/, ref *(int*)/*Error near IL_474f: Stack underflow*/);
						num2 = 208;
						continue;
					case 240:
						array7[2] = 160;
						num = 74;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 469;
					case 432:
						num6 = 187 - 84;
						num = 112;
						if (0 == 0)
						{
							break;
						}
						goto case 266;
					case 266:
						num11 = 0L;
						_ = 384;
						num = (int)/*Error near IL_47b2: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 247;
					case 247:
						array2[14] = (byte)num9;
						num = 309;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 319;
					case 319:
						_ = num21 + 3;
						num22 = ((byte[])/*Error near IL_47e6: Stack underflow*/)[3];
						((sbyte[])/*Error near IL_47e7: Stack underflow*/)[/*Error near IL_47e7: Stack underflow*/] = (sbyte)num22;
						num = 187;
						if (true)
						{
							break;
						}
						goto case 547;
					case 91:
						array11[num7 + 2] = array12[2];
						num2 = 563;
						continue;
					case 297:
						_ = array9.Length;
						_ = 1;
						val22 = /*Error near IL_482f: Stack underflow*/+ /*Error near IL_482f: Stack underflow*/;
						val23 = /*Error near IL_4830: Stack underflow*/- val22;
						num94 = ((byte[])/*Error near IL_4831: Stack underflow*/)[val23];
						num10 = (uint)(/*Error near IL_4832: Stack underflow*/ | num94);
						num2 = 513;
						continue;
					case 413:
						tlfI1xoeKCs7nuslSe(array5, 0, sKOIhjrvKfSLPpeKTp(8), 1);
						_ = 572;
						num = (int)/*Error near IL_4856: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 599;
					case 599:
						num20 = NHZGRo0SaytvR3NXnJ((object)/*Error near IL_4866: Stack underflow*/);
						num = 347;
						break;
					case 299:
						intPtr3 = IntPtr.Zero;
						num4 = 92;
						goto IL_4a61;
					case 473:
						lk7BwHKFmNJY32ZC3n2.bV44XU8KQo = false;
						num4 = 122;
						goto IL_4a61;
					case 40:
						num18 = num7 + 4;
						num19 = array10[4];
						((sbyte[])/*Error near IL_48a7: Stack underflow*/)[num18] = (sbyte)num19;
						num4 = 429;
						goto IL_4a61;
					case 479:
						num3 = 175 - 58;
						num4 = 315;
						goto IL_4a61;
					case 159:
					case 507:
						HifqVVy4sMTU0tX2Zq(IBe4hEip2A, num16 + num17, lk7BwHKFmNJY32ZC3n);
						num2 = 279;
						continue;
					case 303:
						num15 = array9.Length / 4;
						num2 = 345;
						continue;
					case 262:
						array7[22] = 98;
						num = 80;
						if (0 == 0)
						{
							break;
						}
						goto case 411;
					case 411:
						jR652Ijtdrg0nRlgXv(binaryReader);
						num2 = 278;
						continue;
					case 540:
						array8[num12 + 3] = (byte)((num13 & 0xFF000000u) >> 24);
						num = 220;
						if (0 == 0)
						{
							break;
						}
						goto case 68;
					case 68:
						num14 = 0;
						_ = 179;
						continue;
					case 31:
						num9 = 103 + 40;
						num = 82;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 633;
					case 633:
						num9 = 91 + 34;
						num4 = 441;
						goto IL_4a61;
					case 534:
						array7[20] = (byte)num6;
						num = 138;
						break;
					case 558:
						array7[12] = (byte)num3;
						num = 287;
						break;
					case 301:
						array7[13] = 91;
						num = 123;
						break;
					case 590:
						num5 = 44 - 25;
						goto case 387;
					case 355:
						array8[num12 + 1] = (byte)((num13 & 0xFF00) >> 8);
						num4 = 394;
						goto IL_4a61;
					case 347:
						if (num20 != 1)
						{
							num27 = 0;
							num4 = 180;
						}
						else
						{
							num4 = 514;
						}
						goto IL_4a61;
					case 367:
						array2[6] = (byte)num9;
						num = 333;
						SFlWP1U3TGooXTabny();
						if ((int)/*Error near IL_6020: Stack underflow*/ == 0)
						{
							break;
						}
						goto case 529;
					case 640:
						array7[5] = (byte)num3;
						num4 = 456;
						goto IL_4a61;
					case 341:
						array7[0] = 70;
						num2 = 619;
						continue;
					case 494:
						array2[8] = (byte)num9;
						num4 = 633;
						goto IL_4a61;
					case 371:
						num3 = 86 + 2;
						num4 = 596;
						goto IL_4a61;
					case 435:
						num3 = 167 - 55;
						num4 = 148;
						goto IL_4a61;
					case 629:
						array6 = (array5 = array4);
						if (array6 != null)
						{
							num2 = 265;
							continue;
						}
						goto case 516;
					case 318:
						num27++;
						num4 = 369;
						goto IL_4a61;
					case 642:
						return;
					case 167:
						_ = 177;
						num9 = /*Error near IL_0044: Stack underflow*/- 59;
						_ = 508;
						num = (int)/*Error near IL_0051: Stack underflow*/;
						if (true)
						{
							break;
						}
						goto case 510;
					case 139:
						_ = ref num11;
						new IntPtr((void*)(nuint)/*Error near IL_029d: Stack underflow*/);
						sKsK3blfysN6UFS97t((IntPtr)/*Error near IL_02aa: Stack underflow*/, 0, 0L);
						_ = 389;
						goto IL_4a61;
					case 572:
						J8P1jmtmeLeJHAXE9m();
						num4 = 415;
						goto IL_4a61;
					case 433:
						NHZGRo0SaytvR3NXnJ((object)/*Error near IL_5691: Stack underflow*/);
						num30 = (int)/*Error near IL_0532: Stack underflow*/;
						num2 = 340;
						continue;
					case 122:
						_ = IBe4hEip2A;
						obj = 0L;
						obj2 = lk7BwHKFmNJY32ZC3n2;
						HifqVVy4sMTU0tX2Zq((object)/*Error near IL_056d: Stack underflow*/, obj, obj2);
						_ = 612;
						continue;
					case 273:
						_ = IntPtr.Zero;
						intPtr = (IntPtr)/*Error near IL_0abe: Stack underflow*/;
						_ = 217;
						goto IL_4a61;
					case 69:
						_ = 56;
						wDRJe2H6E4HVV6PGZs = (WDRJe2H6E4HVV6PGZs)/*Error near IL_11ae: Stack underflow*/;
						num = 580;
						break;
					case 626:
						_ = 11;
						((sbyte[])/*Error near IL_1269: Stack underflow*/)[/*Error near IL_1269: Stack underflow*/] = (sbyte)num9;
						num = 79;
						break;
					case 25:
						_ = 7;
						((sbyte[])/*Error near IL_129d: Stack underflow*/)[/*Error near IL_129d: Stack underflow*/] = 97;
						num2 = 496;
						continue;
					case 402:
						_ = 21;
						((sbyte[])/*Error near IL_12fd: Stack underflow*/)[/*Error near IL_12fd: Stack underflow*/] = 47;
						num = 551;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 294;
					case 608:
						_ = hSjGubHK9;
						if ((int)/*Error near IL_1369: Stack underflow*/ != 0)
						{
							return;
						}
						_ = 468;
						num = (int)/*Error near IL_1372: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 421;
					case 85:
						_ = 2;
						((sbyte[])/*Error near IL_13d6: Stack underflow*/)[/*Error near IL_13d6: Stack underflow*/] = -119;
						_ = 477;
						num = (int)/*Error near IL_13df: Stack underflow*/;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 109;
					case 484:
						_ = 106;
						num6 = /*Error near IL_1430: Stack underflow*/+ 2;
						num4 = 531;
						goto IL_4a61;
					case 200:
						_ = 10;
						((sbyte[])/*Error near IL_174c: Stack underflow*/)[/*Error near IL_174c: Stack underflow*/] = 91;
						num2 = 452;
						continue;
					case 29:
						_ = 23;
						((sbyte[])/*Error near IL_18e9: Stack underflow*/)[/*Error near IL_18e9: Stack underflow*/] = (sbyte)num6;
						num = 490;
						if (true)
						{
							break;
						}
						goto case 532;
					case 5:
						_ = 9;
						((sbyte[])/*Error near IL_1936: Stack underflow*/)[/*Error near IL_1936: Stack underflow*/] = (sbyte)num5;
						num4 = 481;
						goto IL_4a61;
					case 311:
						_ = 14;
						((sbyte[])/*Error near IL_1eab: Stack underflow*/)[/*Error near IL_1eab: Stack underflow*/] = (sbyte)num6;
						num = 145;
						break;
					case 490:
						_ = 56;
						num3 = /*Error near IL_1f6e: Stack underflow*/+ 2;
						num2 = 607;
						continue;
					case 545:
						_ = 7;
						((sbyte[])/*Error near IL_235c: Stack underflow*/)[/*Error near IL_235c: Stack underflow*/] = (sbyte)num9;
						num2 = 495;
						continue;
					case 23:
						_ = 180;
						num3 = /*Error near IL_2537: Stack underflow*/- 60;
						_ = 246;
						num = (int)/*Error near IL_2544: Stack underflow*/;
						if (true)
						{
							break;
						}
						goto case 78;
					case 219:
						_ = 104;
						num9 = /*Error near IL_276f: Stack underflow*/+ 106;
						num = 461;
						break;
					case 592:
						_ = 227;
						num9 = /*Error near IL_2de3: Stack underflow*/- 75;
						_ = 268;
						num = (int)/*Error near IL_2df0: Stack underflow*/;
						break;
					case 2:
						ROhFJh1RB((IntPtr)/*Error near IL_5c63: Stack underflow*/, (string)/*Error near IL_5c63: Stack underflow*/);
						intPtr11 = (IntPtr)/*Error near IL_3007: Stack underflow*/;
						num = 178;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 326;
					case 444:
						_ = 137;
						num3 = /*Error near IL_30d9: Stack underflow*/- 31;
						num4 = 638;
						goto IL_4a61;
					case 6:
						_ = 3;
						((sbyte[])/*Error near IL_32df: Stack underflow*/)[/*Error near IL_32df: Stack underflow*/] = 90;
						num2 = 227;
						continue;
					case 101:
						_ = ref lk7BwHKFmNJY32ZC3n;
						((Lk7BwHKFmNJY32ZC3n*)(nint)/*Error near IL_33d9: Stack underflow*/)->bV44XU8KQo = bV44XU8KQo;
						_ = 1;
						if (SFlWP1U3TGooXTabny())
						{
							num2 = 562;
							continue;
						}
						num = 159;
						break;
					case 619:
						_ = 1;
						((sbyte[])/*Error near IL_3646: Stack underflow*/)[/*Error near IL_3646: Stack underflow*/] = -108;
						num4 = 382;
						goto IL_4a61;
					case 115:
						_ = 210;
						num5 = /*Error near IL_3699: Stack underflow*/- 70;
						num4 = 524;
						goto IL_4a61;
					case 448:
						JLGytImuxrMxAZqJKu();
						if ((int)/*Error near IL_3d05: Stack underflow*/ != 4)
						{
							num = 164;
							break;
						}
						_ = new byte[30];
						g1cqHkAgBBeTN3XMfvl((object)/*Error near IL_43ca: Stack underflow*/, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
						array = (byte[])/*Error near IL_43d7: Stack underflow*/;
						_ = 212;
						num = (int)/*Error near IL_43e0: Stack underflow*/;
						if (0 == 0)
						{
							break;
						}
						goto case 180;
					case 291:
						_ = typeof(DyyVDbaRvM1YfIq9il).TypeHandle;
						if (KpNDnm3q1r6YoZRF9H(lGqEHaRDPgHFpusu4D(rEacmnz8ipU2tXY6jc((RuntimeTypeHandle)/*Error near IL_40d1: Stack underflow*/).Assembly)) <= 0)
						{
							goto IL_33f4_2;
						}
						num4 = 334;
						goto IL_4a61;
					case 503:
						iW4joX4pRXvcmAix5k();
						symmetricAlgorithm = (SymmetricAlgorithm)/*Error near IL_4222: Stack underflow*/;
						num2 = 226;
						continue;
					case 561:
						_ = 6;
						_ = 133;
						val = /*Error near IL_4240: Stack underflow*/- 44;
						((sbyte[])/*Error near IL_4241: Stack underflow*/)[/*Error near IL_4241: Stack underflow*/] = (sbyte)(int)val;
						num4 = 115;
						goto IL_4a61;
					case 59:
						_ = 30;
						num7 = (int)/*Error near IL_4373: Stack underflow*/;
						num = 331;
						if (!SFlWP1U3TGooXTabny())
						{
							break;
						}
						goto case 417;
					case 547:
						_ = ref lk7BwHKFmNJY32ZC3n2;
						*(Lk7BwHKFmNJY32ZC3n*)(nint)/*Error near IL_47fd: Stack underflow*/ = default(Lk7BwHKFmNJY32ZC3n);
						num4 = 244;
						goto IL_4a61;
					case 177:
						_ = 20;
						((sbyte[])/*Error near IL_48d8: Stack underflow*/)[/*Error near IL_48d8: Stack underflow*/] = (sbyte)num6;
						num = 127;
						break;
					case 128:
						_ = 5;
						((sbyte[])/*Error near IL_491a: Stack underflow*/)[/*Error near IL_491a: Stack underflow*/] = (sbyte)num6;
						num2 = 565;
						continue;
					case 207:
						_ = 1;
						((sbyte[])/*Error near IL_49ff: Stack underflow*/)[/*Error near IL_49ff: Stack underflow*/] = (sbyte)num5;
						num2 = 156;
						continue;
					default:
						_ = 387;
						num = (int)/*Error near IL_5483: Stack underflow*/;
						if (YiHWqdAEL2s4JBasBe())
						{
							break;
						}
						goto case 298;
					case 356:
						_ = phV4Uu6SUx;
						Qwp4ejR7FG = NGHo4rAHSuSQ4ufCkBP((long)/*Error near IL_54f0: Stack underflow*/);
						num4 = 90;
						goto IL_4a61;
					case 523:
						{
							_ = 50;
							num3 = /*Error near IL_5559: Stack underflow*/+ 49;
							num4 = 77;
							goto IL_4a61;
						}
						IL_33f4_2:
						try
						{
							object obj5 = HTMLgVAIg6ADBaOAp5x(KHBXOyA2J0TRDNUEb4n(XSYACKAmx9CEGKYuKSC(v0TnNMA8esHLQWVTRyb(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly))).GetField("m_ptr", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), XSYACKAmx9CEGKYuKSC(v0TnNMA8esHLQWVTRyb(rEacmnz8ipU2tXY6jc(typeof(DyyVDbaRvM1YfIq9il).TypeHandle).Assembly)));
							if (obj5 is IntPtr)
							{
								XtL4lyIIgx = (IntPtr)obj5;
							}
							if (qNGJ4OfmSpfpcWEKvj(obj5.GetType().ToString(), "System.Reflection.RuntimeModule"))
							{
								XtL4lyIIgx = (IntPtr)HTMLgVAIg6ADBaOAp5x(obj5.GetType().GetField("m_pData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), obj5);
							}
							MemoryStream memoryStream2 = new MemoryStream();
							cImcIssX0mqXP11GuB(memoryStream2, new byte[JLGytImuxrMxAZqJKu()], 0, JLGytImuxrMxAZqJKu());
							if (JLGytImuxrMxAZqJKu() == 4)
							{
								cImcIssX0mqXP11GuB(memoryStream2, NelyXWhgfYRGJe9y0i(XtL4lyIIgx.ToInt32()), 0, 4);
							}
							else
							{
								cImcIssX0mqXP11GuB(memoryStream2, HfDHTTAN2uenSb31bPn(XtL4lyIIgx.ToInt64()), 0, 8);
							}
							cImcIssX0mqXP11GuB(memoryStream2, new byte[JLGytImuxrMxAZqJKu()], 0, JLGytImuxrMxAZqJKu());
							cImcIssX0mqXP11GuB(memoryStream2, new byte[JLGytImuxrMxAZqJKu()], 0, JLGytImuxrMxAZqJKu());
							uv67RaQSbjWNCiTI1l(memoryStream2, 0L);
							byte[] array18 = (byte[])AJs46ZSqBoMlDC1IIM(memoryStream2);
							mGMWpQe4ChoBUxrIed(memoryStream2);
							uint nativeSizeOfCode = 0u;
							try
							{
								array19 = (array5 = array18);
								if (array19 != null)
								{
									_ = 1;
									if (!SFlWP1U3TGooXTabny())
									{
										int num55;
										num56 = (num55 = 4);
										num57 = num55;
									}
									else
									{
										num57 = 3;
									}
									while (true)
									{
										switch (num57)
										{
										case 1:
											break;
										case 0:
										case 4:
											goto IL_35c3_2;
										default:
											goto IL_35d6_2;
										}
										break;
										IL_35c3_2:
										if (array5.Length == 0)
										{
											num57 = 1;
											continue;
										}
										while (true)
										{
											IL_35af_2:
											fixed (byte* value2 = &array5[0])
											{
												int num55;
												num56 = (num55 = 5);
												num57 = num55;
												while (true)
												{
													switch (num57)
													{
													case 1:
														break;
													case 0:
													case 4:
														if (array5.Length != 0)
														{
															goto IL_35af_2;
														}
														goto IL_35cc_2;
													default:
														x3c4o2PyTx(new IntPtr(value2), new IntPtr(value2), new IntPtr(value2), 216669565u, new IntPtr(value2), ref nativeSizeOfCode);
														goto end_IL_35af_2;
													}
													break;
													IL_35cc_2:
													num57 = 1;
												}
											}
											goto end_IL_357d_2;
											continue;
											end_IL_35af_2:
											break;
										}
										goto end_IL_3547_2;
										continue;
										end_IL_357d_2:
										break;
									}
								}
								reference = ref *(byte*)null;
								goto IL_35d6_2;
								IL_35d6_2:
								x3c4o2PyTx(new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), 216669565u, new IntPtr(System.Runtime.CompilerServices.Unsafe.AsPointer(ref reference)), ref nativeSizeOfCode);
								end_IL_3547_2:;
							}
							finally
							{
								reference = ref *(byte*)null;
							}
						}
						catch (object obj6)
						{
						}
						FqA3fjAY4CaOvkoaMo3(bFB44BUGlg);
						num2 = 571;
						continue;
						IL_2e86:
						num23 = (uint)num12;
						num2 = 225;
						continue;
						IL_1941:
						if (JLGytImuxrMxAZqJKu() == 4)
						{
							num = 171;
							break;
						}
						goto IL_3b9e_2;
						IL_4a61:
						num = num4;
						break;
						IL_3b9e_2:
						binaryReader = new BinaryReader((Stream)rsW1hDE96lSnfJEg3D(j8hgmZJ7n, "\u008fcu\u008c\u0092m\u008d\u0097c\u009b\u008bmu\u00914\u0089ur.fa\u009574s\u008e\u009e\u0088d5\u0097s\u0090tm6j"));
						num4 = 546;
						goto IL_4a61;
						IL_3834:
						num13 = num38 ^ num10;
						num4 = 259;
						goto IL_4a61;
					}
					break;
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object vZF7RiFiF(object  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			try
			{
				if (File.Exists(((Assembly) ).Location))
				{
					return ((Assembly) ).Location;
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(((Assembly) ).GetName().CodeBase.ToString().Replace("file:///", "")))
				{
					return ((Assembly) ).GetName().CodeBase.ToString().Replace("file:///", "");
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists( .GetType().GetProperty("Location").GetValue( , new object[0])
					.ToString()))
				{
					return  .GetType().GetProperty("Location").GetValue( , new object[0])
						.ToString();
				}
			}
			catch
			{
			}
			return "";
		}

		[DllImport("kernel32", EntryPoint = "LoadLibrary")]
		public static extern IntPtr puGi6bKKk(string  );

		[DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress")]
		public static extern IntPtr ROhFJh1RB(IntPtr  , string  );

		[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
		private static extern int T7LBbJ4ta(IntPtr  , IntPtr  , [In][Out] byte[]  , uint  , out IntPtr  );

		[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
		private static extern int fMdPu7i25(IntPtr  , IntPtr  , [In][Out] byte[]  , uint  , out IntPtr  );

		[DllImport("kernel32.dll", EntryPoint = "VirtualProtect")]
		private static extern int yMayDYsjD(IntPtr  , int  , int  , ref int  );

		[DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
		private static extern IntPtr Kxm8CyXvJ(uint  , int  , uint  );

		[DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
		private static extern int JkHjxJCFT(IntPtr  );

		[MethodImpl(MethodImplOptions.NoInlining)]
		[AXBrnIFfMAfABnJrF9(typeof(AXBrnIFfMAfABnJrF9.z0oyxsqySXMDuI4ZyY<object>[]))]
		private static byte[] eM2t2dfoT(string  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			using FileStream fileStream = new FileStream( , FileMode.Open, FileAccess.Read, FileShare.Read);
			int num = 0;
			long length = fileStream.Length;
			int num2 = (int)length;
			byte[] array = new byte[num2];
			while (num2 > 0)
			{
				int num3 = fileStream.Read(array, num, num2);
				num += num3;
				num2 -= num3;
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[AXBrnIFfMAfABnJrF9(typeof(AXBrnIFfMAfABnJrF9.z0oyxsqySXMDuI4ZyY<object>[]))]
		private static byte[] vDfq2bW1V(byte[]  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = SuhhReBcy();
			symmetricAlgorithm.Key = new byte[32]
			{
				34, 170, 40, 238, 19, 55, 160, 149, 135, 244,
				132, 234, 80, 73, 101, 67, 148, 40, 138, 54,
				224, 145, 27, 234, 173, 246, 57, 208, 145, 40,
				233, 61
			};
			symmetricAlgorithm.IV = new byte[16]
			{
				111, 114, 32, 170, 7, 200, 137, 106, 102, 206,
				156, 148, 164, 18, 252, 231
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write( , 0,  .Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private byte[] B3XRfqih9()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private byte[] sVk5WFvVV()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private byte[] E3GryunuI()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private byte[] yxOcIGI9u()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private byte[] Oihu8LNHm()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private byte[] ifqQyNVWS()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal byte[] hcDmskCdX()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			string text = "{11111-22222-40001-00001}";
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal byte[] mKgSOTjDj()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			string text = "{11111-22222-40001-00002}";
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal byte[] aYTwtN0c5()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			string text = "{11111-22222-50001-00001}";
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal byte[] udfDaXdkp()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			string text = "{11111-22222-50001-00002}";
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public DyyVDbaRvM1YfIq9il()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			base..ctor();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object C0CxYofPdu5pf1Nyrx(object P_0, object P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((Assembly)P_0).GetManifestResourceStream((string)P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object n8Grd6Vv25DLpRCsTV(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((BinaryReader)P_0).BaseStream;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void DIdgMcmS7LIxdhnV9y(object P_0, long P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((Stream)P_0).Position = P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static long gvnVj6pORMhCv3F2J1(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((Stream)P_0).Length;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object pP6gQvYy51t5JHwUV3(object P_0, int P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((BinaryReader)P_0).ReadBytes(P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void TSQmGfAxQFBBxCJTWu(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((BinaryReader)P_0).Close();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void kxPACNxW9BO7L6G31k(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			Array.Reverse((Array)P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object atF1Rdkck7mnjl1K13(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((Assembly)P_0).GetName();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object rksxtH9b1aE45V2xuA(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((AssemblyName)P_0).GetPublicKeyToken();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object wwm9YqjHMAOjE6JME7()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return SuhhReBcy();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void dSQ9sXb6b1U7vMnAyG(object P_0, CipherMode P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((SymmetricAlgorithm)P_0).Mode = P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object t2I1HRISgUxYhBr3Dx(object P_0, object P_1, object P_2)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((SymmetricAlgorithm)P_0).CreateDecryptor((byte[])P_1, (byte[])P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void ocwDxSUU6Judcd8OTE(object P_0, object P_1, int P_2, int P_3)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((Stream)P_0).Write((byte[])P_1, P_2, P_3);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void lp5fg4ylTrq5EUjO8G(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((CryptoStream)P_0).FlushFinalBlock();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object APVLN68lLNY4HvSeGn(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((MemoryStream)P_0).ToArray();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void owxJFjWa7h5kWcIDDl(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((Stream)P_0).Close();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int wwniUilX0PGl2lH9FY(object P_0, int P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return BitConverter.ToInt32((byte[])P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object McKvRNormqspo9uTHU()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return Encoding.Unicode;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object VXWtrFDX7LCc0BEsLp(object P_0, object P_1, int P_2, int P_3)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((Encoding)P_0).GetString((byte[])P_1, P_2, P_3);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool R3TVja2vZlnVEbMYYF()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool z5DnODHQiwNxpdaCsc()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object iPqb2MG5vuRl0loKTl(object P_0, object P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((Assembly)P_0).GetManifestResourceStream((string)P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object byLFjg1JT0D40pp2cs(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((BinaryReader)P_0).BaseStream;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void EjrLuR2ZY7ms4AQOOb(object P_0, long P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((Stream)P_0).Position = P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void oeLdZuxiQiliYklUox(bool P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			RSACryptoServiceProvider.UseMachineKeyStore = P_0;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static long s6NlJypZJSjEs2gqgj(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((Stream)P_0).Length;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object ijGrTqvc1yM5Gp420u(object P_0, int P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((BinaryReader)P_0).ReadBytes(P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object EZYBJ8F0ylktsnM1ti(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((Assembly)P_0).GetName();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object t8MFGHqlIE5SFcoIkT(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((AssemblyName)P_0).GetPublicKeyToken();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object QEue6GJ234R4VGGwY9()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return SuhhReBcy();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void NAXUMbIwDNgos1sBmT(object P_0, CipherMode P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((SymmetricAlgorithm)P_0).Mode = P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object fTkWdZTvILE01nbqsb(object P_0, object P_1, object P_2)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((SymmetricAlgorithm)P_0).CreateDecryptor((byte[])P_1, (byte[])P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void BMQx4ZHGDgW4w6gyZ4(object P_0, object P_1, int P_2, int P_3)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((Stream)P_0).Write((byte[])P_1, P_2, P_3);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void pEXIllk0X5v7xWvpGD(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((CryptoStream)P_0).FlushFinalBlock();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object EUOB16lP9J1TjE6ALn(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return ((MemoryStream)P_0).ToArray();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void lsWHlti4EhChsfALaA(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((Stream)P_0).Close();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void mRTZbLhNC1cdCl6D4w(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			((BinaryReader)P_0).Close();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object th0esw8RdOD424WJbc(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return vZF7RiFiF(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object xx9ikTnlOw8AXB6iGv(object P_0)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return eM2t2dfoT((string)P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int LLfZLGcRPZ8mBUurk1(object P_0, int P_1)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return BitConverter.ToInt32((byte[])P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool kO5nUij9Sn6maXBNYh()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool dgDhRq7iq3m3W3BBsY()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static IntPtr tg8t5OqN45OAVd2Uq7(IntPtr P_0, int P_1)
		{
			return Marshal.ReadIntPtr(P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int OKoHdvTcfykgJd9bmk(IntPtr P_0, int P_1)
		{
			return Marshal.ReadInt32(P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static long iTtk4eiI7iTGDjhjDS(IntPtr P_0, int P_1)
		{
			return Marshal.ReadInt64(P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void FsHWNgkGiVuOl8OmJc(IntPtr P_0, int P_1, IntPtr P_2)
		{
			Marshal.WriteIntPtr(P_0, P_1, P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void o00Z5hGmAa4HorFX8O(IntPtr P_0, int P_1, int P_2)
		{
			Marshal.WriteInt32(P_0, P_1, P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void sKsK3blfysN6UFS97t(IntPtr P_0, int P_1, long P_2)
		{
			Marshal.WriteInt64(P_0, P_1, P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static IntPtr sKOIhjrvKfSLPpeKTp(int P_0)
		{
			return Marshal.AllocCoTaskMem(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void tlfI1xoeKCs7nuslSe(object P_0, int P_1, IntPtr P_2, int P_3)
		{
			Marshal.Copy((byte[])P_0, P_1, P_2, P_3);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void J8P1jmtmeLeJHAXE9m()
		{
			HK2JaffxR();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object XKH3U5WCf2UyQCIIya()
		{
			return Process.GetCurrentProcess();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object elLAI9D2FYClRc1Rw0(object P_0)
		{
			return ((Process)P_0).MainModule;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static IntPtr GRSglWdn3wkT4vDfh5(object P_0)
		{
			return ((ProcessModule)P_0).BaseAddress;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool yCGPKt8G6d0By1Tw2I(IntPtr P_0, IntPtr P_1)
		{
			return P_0 != P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int JLGytImuxrMxAZqJKu()
		{
			return IntPtr.Size;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static Type gKjkGS27vZd6ReTL7N(object P_0, bool P_1)
		{
			return Type.GetType((string)P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool sPFYCLItnSewrnGJfh(Type P_0, Type P_1)
		{
			return P_0 != P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object kxxORYNQpt7JeTNfIx(object P_0)
		{
			return ((Process)P_0).Modules;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object JvuscRYrEHnEfwpXlR(object P_0)
		{
			return ((ReadOnlyCollectionBase)P_0).GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object w040Z1ZHbG0XVqZMoU(object P_0)
		{
			return ((IEnumerator)P_0).Current;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object z1aSaf7nHt2GSLVr8b(object P_0)
		{
			return ((ProcessModule)P_0).ModuleName;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object ustSntgIXxQiPYwipw(object P_0)
		{
			return ((string)P_0).ToLower();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool qNGJ4OfmSpfpcWEKvj(object P_0, object P_1)
		{
			return (string)P_0 == (string)P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object XEpsOX15JQGw1yNP1V(object P_0)
		{
			return ((ProcessModule)P_0).FileVersionInfo;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int pYLE2mB3h2h5Q9EWWJ(object P_0)
		{
			return ((FileVersionInfo)P_0).ProductMajorPart;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int ddHTLXcmUp85qbEZ5v(object P_0)
		{
			return ((FileVersionInfo)P_0).ProductMinorPart;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int YSPPRZbemZq4tiMetn(object P_0)
		{
			return ((FileVersionInfo)P_0).ProductBuildPart;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int VfX7Wy6qBCfj5QN0tS(object P_0)
		{
			return ((FileVersionInfo)P_0).ProductPrivatePart;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool B28kJjXJMefNoj8Ht7(object P_0, object P_1)
		{
			return (Version)P_0 >= (Version)P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool vAbWvNPlhhsilht7I2(object P_0, object P_1)
		{
			return (Version)P_0 < (Version)P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool Prhas5pe6XdsQAESvm(object P_0)
		{
			return ((IEnumerator)P_0).MoveNext();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void p8SJ0uC03Z63haWUBt(object P_0)
		{
			((IDisposable)P_0).Dispose();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object rsW1hDE96lSnfJEg3D(object P_0, object P_1)
		{
			return ((Assembly)P_0).GetManifestResourceStream((string)P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object QX5YKsJaRCBODNJawM(object P_0)
		{
			return ((BinaryReader)P_0).BaseStream;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void uv67RaQSbjWNCiTI1l(object P_0, long P_1)
		{
			((Stream)P_0).Position = P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static long OhZpyDwZxpiyDh5BOJ(object P_0)
		{
			return ((Stream)P_0).Length;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object u2tFaHurfr1s9xSbVJ(object P_0, int P_1)
		{
			return ((BinaryReader)P_0).ReadBytes(P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void KKPSQdOpi1WKW3jfUr(object P_0)
		{
			Array.Reverse((Array)P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object xUQfefaWFJbhDoECkW(object P_0)
		{
			return ((Assembly)P_0).GetName();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object HYpd1oF0heYCHUYr2Q(object P_0)
		{
			return ((AssemblyName)P_0).GetPublicKeyToken();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void iRLmBjMF2IHavq0sHJ(object P_0, int P_1, int P_2)
		{
			Array.Clear((Array)P_0, P_1, P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object w93lZh9ZsNOKSgl1T8(object P_0)
		{
			return ((Assembly)P_0).GetModules();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static IntPtr qy8E1Jx0giZMnHMioa(object P_0)
		{
			return Marshal.GetHINSTANCE((Module)P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object lGqEHaRDPgHFpusu4D(object P_0)
		{
			return ((Assembly)P_0).Location;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int KpNDnm3q1r6YoZRF9H(object P_0)
		{
			return ((string)P_0).Length;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int NHZGRo0SaytvR3NXnJ(object P_0)
		{
			return ((BinaryReader)P_0).ReadInt32();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object iW4joX4pRXvcmAix5k()
		{
			return SuhhReBcy();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void Tv7YbOvhTkPsTjfkQK(object P_0, CipherMode P_1)
		{
			((SymmetricAlgorithm)P_0).Mode = P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object pagHAVKrD0QwEqDWBc(object P_0, object P_1, object P_2)
		{
			return ((SymmetricAlgorithm)P_0).CreateDecryptor((byte[])P_1, (byte[])P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void cImcIssX0mqXP11GuB(object P_0, object P_1, int P_2, int P_3)
		{
			((Stream)P_0).Write((byte[])P_1, P_2, P_3);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void OlnCOhV4y6WcWPCFau(object P_0)
		{
			((CryptoStream)P_0).FlushFinalBlock();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object AJs46ZSqBoMlDC1IIM(object P_0)
		{
			return ((MemoryStream)P_0).ToArray();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void mGMWpQe4ChoBUxrIed(object P_0)
		{
			((Stream)P_0).Close();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void jR652Ijtdrg0nRlgXv(object P_0)
		{
			((BinaryReader)P_0).Close();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int TMSAZx51W3OgMOtnPJ(object P_0)
		{
			return ((Process)P_0).Id;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object NelyXWhgfYRGJe9y0i(int P_0)
		{
			return BitConverter.GetBytes(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static long IUSsYAngpyKANEnZ1U(object P_0)
		{
			return ((Stream)P_0).Position;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void M0UK57Lct29oQf1y7M(IntPtr P_0, int P_1)
		{
			Marshal.WriteInt32(P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void HifqVVy4sMTU0tX2Zq(object P_0, object P_1, object P_2)
		{
			((Hashtable)P_0).Add(P_1, P_2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static Type rEacmnz8ipU2tXY6jc(RuntimeTypeHandle P_0)
		{
			return Type.GetTypeFromHandle(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int NGHo4rAHSuSQ4ufCkBP(long P_0)
		{
			return Convert.ToInt32(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object FBQwxZAAAGt3s3HSZ3l()
		{
			return Encoding.UTF8;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object eKmFLmAUTIA4RBnjHCA(object P_0, object P_1)
		{
			return ((Encoding)P_0).GetString((byte[])P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool thBMZkAqCqa73KF6ess(IntPtr P_0, IntPtr P_1)
		{
			return P_0 == P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object krLZHPATvDcIfeOggP9(IntPtr  , Type  )
		{
			return ubITRqgdO( ,  );
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static IntPtr HfSOpPAiXrYiEn2URpZ(object P_0)
		{
			return P_0();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int sYNV1XAk8wX0pLjqgmD(IntPtr P_0)
		{
			return Marshal.ReadInt32(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static long iBfhYnAGRj4nG6UCpdH(IntPtr P_0)
		{
			return Marshal.ReadInt64(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static IntPtr hKbI6SAlKF8QuASWEYm(object P_0)
		{
			return Marshal.GetFunctionPointerForDelegate((Delegate)P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int Btnc15ArMdr21Qis9mc(object P_0)
		{
			return ((ProcessModule)P_0).ModuleMemorySize;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object gQ3bxJAoGy2rvkCxjGU(object P_0)
		{
			return ((Assembly)P_0).EntryPoint;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool qGsoXYAtg5xUyI3IWm5(object P_0, object P_1)
		{
			return (MethodInfo)P_0 != (MethodInfo)P_1;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object j5I56qAWJhTHkpG09e6(object P_0)
		{
			return ((Delegate)P_0).Method;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object T5KjoaAD0DEhr9ndv3K(Type P_0, object P_1)
		{
			return Delegate.CreateDelegate(P_0, (MethodInfo)P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object XYFEn2AdQMDhr0ac0wL(object P_0)
		{
			return ((MethodBase)P_0).GetParameters();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object v0TnNMA8esHLQWVTRyb(object P_0)
		{
			return ((Assembly)P_0).ManifestModule;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static ModuleHandle XSYACKAmx9CEGKYuKSC(object P_0)
		{
			return ((Module)P_0).ModuleHandle;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static Type KHBXOyA2J0TRDNUEb4n(object P_0)
		{
			return P_0.GetType();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object HTMLgVAIg6ADBaOAp5x(object P_0, object P_1)
		{
			return ((FieldInfo)P_0).GetValue(P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object HfDHTTAN2uenSb31bPn(long P_0)
		{
			return BitConverter.GetBytes(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void FqA3fjAY4CaOvkoaMo3(object P_0)
		{
			RuntimeHelpers.PrepareDelegate((Delegate)P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static RuntimeMethodHandle blmQxKAZMFpiBMOawsF(object P_0)
		{
			return ((MethodBase)P_0).MethodHandle;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void O2mGeMA7472VhJ2Eakk(RuntimeMethodHandle P_0)
		{
			RuntimeHelpers.PrepareMethod(P_0);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void g1cqHkAgBBeTN3XMfvl(object P_0, RuntimeFieldHandle P_1)
		{
			RuntimeHelpers.InitializeArray((Array)P_0, P_1);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void dCWYxTAfxJ0l1weKVN1(IntPtr P_0, IntPtr P_1)
		{
			Marshal.WriteIntPtr(P_0, P_1);
		}

		internal static bool YiHWqdAEL2s4JBasBe()
		{
			return true;
		}

		internal static bool SFlWP1U3TGooXTabny()
		{
			return false;
		}
	}
}
namespace cH8IXcwQY4Peh2qpAn
{
	internal class xrUtBVoaXtCT6B0w6a
	{
		private static bool ywq4VEynyU;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void S\u008e9\u000d\u000a\u00966\u009dxzd\u0096\u0095\u008ca()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public xrUtBVoaXtCT6B0w6a()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			base..ctor();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static xrUtBVoaXtCT6B0w6a()
		{
			DyyVDbaRvM1YfIq9il.vEB6drODu();
		}
	}
}
namespace c5uHW3cSW8ou55rAF3
{
	internal class KKr6hZkjvwWjdm9A4Z
	{
		internal static ModuleHandle Uur4ZuAaiM;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static RuntimeTypeHandle Q\u0090r\u000d\u000a\u00966\u009dy\u0086\u009aof\u0092\u0098(int token)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return Uur4ZuAaiM.GetRuntimeTypeHandleFromMetadataToken(token);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static RuntimeFieldHandle b\u008a\u0087\u000d\u000a\u00966\u009dy\u0087\u009f\u00887bx(int token)
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			return Uur4ZuAaiM.GetRuntimeFieldHandleFromMetadataToken(token);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public KKr6hZkjvwWjdm9A4Z()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			base..ctor();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static KKr6hZkjvwWjdm9A4Z()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			Uur4ZuAaiM = typeof(KKr6hZkjvwWjdm9A4Z).Assembly.GetModules()[0].ModuleHandle;
		}
	}
}
namespace cOiloqIe4m0u98YvJc
{
	internal class WHFZxkbSLrgnS9Moa2
	{
		private static Hashtable OvB4WGug33;

		private static Hashtable pBI4f7NMse;

		private static bool k8f4sjMCVQ;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void lLHifFIsCLsZtjvFfN0i()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			if (!k8f4sjMCVQ)
			{
				k8f4sjMCVQ = true;
				AppDomain currentDomain = AppDomain.CurrentDomain;
				currentDomain.AssemblyResolve += H1C4H2H0pn;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Assembly H1C4H2H0pn(object  , ResolveEventArgs  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			lock (OvB4WGug33)
			{
				string text =  .Name.Trim();
				object obj = OvB4WGug33[text];
				if (obj == null)
				{
					try
					{
						string text2 = I5q42HS3xf(text);
						Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
						Assembly[] array = assemblies;
						foreach (Assembly assembly in array)
						{
							if (assembly.GetName().Name.ToUpper() == text2.ToUpper())
							{
								return assembly;
							}
						}
					}
					catch
					{
					}
				}
				if (obj == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string text3 = I5q42HS3xf(text);
						byte[] bytes = Encoding.Unicode.GetBytes(text3);
						string text4 = DyyVDbaRvM1YfIq9il.KX0HrYNeb(25454) + Convert.ToBase64String(DyyVDbaRvM1YfIq9il.BjkXsyRir(bytes));
						Stream manifestResourceStream = Type.GetTypeFromHandle(KKr6hZkjvwWjdm9A4Z.Q\u0090r\u000d\u000a\u00966\u009dy\u0086\u009aof\u0092\u0098(33554755)).Assembly.GetManifestResourceStream(text4);
						if (manifestResourceStream != null)
						{
							try
							{
								BinaryReader binaryReader = new BinaryReader(manifestResourceStream);
								binaryReader.BaseStream.Position = 0L;
								byte[] array2 = new byte[manifestResourceStream.Length];
								binaryReader.Read(array2, 0, array2.Length);
								binaryReader.Close();
								bool flag = false;
								Assembly assembly2 = null;
								try
								{
									assembly2 = Assembly.Load(array2);
								}
								catch (FileLoadException)
								{
									flag = true;
								}
								catch (BadImageFormatException)
								{
									flag = true;
								}
								if (flag)
								{
									string path = Path.Combine(Path.GetTempPath(), text4);
									string text5 = Path.Combine(path, text3 + DyyVDbaRvM1YfIq9il.KX0HrYNeb(25486));
									if (!File.Exists(text5) || !pBI4f7NMse.ContainsKey(text5))
									{
										try
										{
											pBI4f7NMse[text5] = null;
											if (!Directory.Exists(Path.GetDirectoryName(text5)))
											{
												Directory.CreateDirectory(Path.GetDirectoryName(text5));
											}
											FileStream fileStream = new FileStream(text5, FileMode.Create, FileAccess.Write);
											fileStream.Write(array2, 0, array2.Length);
											fileStream.Close();
										}
										catch
										{
										}
									}
									assembly2 = Assembly.LoadFile(text5);
									OvB4WGug33.Add(text, assembly2);
								}
								else
								{
									OvB4WGug33.Add(text, assembly2);
								}
								return assembly2;
							}
							catch (Exception)
							{
							}
						}
					}
					catch (Exception)
					{
					}
					return null;
				}
				return (Assembly)obj;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string I5q42HS3xf(string  )
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			string text =  .Trim();
			int num = text.IndexOf(',');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public WHFZxkbSLrgnS9Moa2()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			base..ctor();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static WHFZxkbSLrgnS9Moa2()
		{
			//Discarded unreachable code: IL_0002
			while (false)
			{
				_ = ((object[])null)[0];
			}
			OvB4WGug33 = new Hashtable();
			pBI4f7NMse = new Hashtable();
			k8f4sjMCVQ = false;
		}
	}
}
