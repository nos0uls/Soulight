using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using vJiGl01UUJfXfNWas3;

namespace MacForm;

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
