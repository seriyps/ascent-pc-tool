using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class RangeSliderControl : UserControl
{
	private int minValue;

	private int maxValue;

	private int lowerValue;

	private int upperValue;

	private int tickFrequency;

	private int step;

	private int minimumRange;

	private int thumbRadius;

	private int trackHeight;

	private int edgeMargin;

	private bool draggingLower;

	private bool draggingUpper;

	private bool draggingRange;

	private int dragStartMouseX;

	private int dragStartLowerValue;

	private int dragStartUpperValue;

	private IContainer components;

	[Category("Behavior")]
	public int Minimum
	{
		get
		{
			return minValue;
		}
		set
		{
			minValue = value;
			((Control)this).Invalidate();
		}
	}

	[Category("Behavior")]
	public int Maximum
	{
		get
		{
			return maxValue;
		}
		set
		{
			maxValue = value;
			((Control)this).Invalidate();
		}
	}

	[Category("Behavior")]
	public int LowerValue
	{
		get
		{
			return lowerValue;
		}
		set
		{
			lowerValue = Snap(Math.Max(Minimum, Math.Min(value, upperValue - minimumRange)));
			((Control)this).Invalidate();
			ValueChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	[Category("Behavior")]
	public int UpperValue
	{
		get
		{
			return upperValue;
		}
		set
		{
			upperValue = Snap(Math.Min(Maximum, Math.Max(value, lowerValue + minimumRange)));
			((Control)this).Invalidate();
			ValueChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	[Category("Behavior")]
	public int TickFrequency
	{
		get
		{
			return tickFrequency;
		}
		set
		{
			tickFrequency = value;
			((Control)this).Invalidate();
		}
	}

	[Category("Behavior")]
	public int Step
	{
		get
		{
			return step;
		}
		set
		{
			step = value;
		}
	}

	[Category("Behavior")]
	public int MinimumRange
	{
		get
		{
			return minimumRange;
		}
		set
		{
			minimumRange = value;
		}
	}

	[Category("Appearance")]
	[Description("每隔几个刻度显示文本，1 表示每个刻度都显示文本")]
	public int TextInterval { get; set; }

	[Category("Appearance")]
	[Description("滑块半径，控制滑块大小")]
	public int ThumbSize
	{
		get
		{
			return thumbRadius;
		}
		set
		{
			thumbRadius = Math.Max(2, value);
			edgeMargin = Math.Max(2, thumbRadius / 4);
			((Control)this).Invalidate();
		}
	}

	[Category("Appearance")]
	[Description("滑块颜色")]
	public Color ThumbColor { get; set; }

	[Category("Appearance")]
	[Description("轨道高度")]
	public int TrackHeightCustom
	{
		get
		{
			return trackHeight;
		}
		set
		{
			trackHeight = Math.Max(2, value);
			((Control)this).Invalidate();
		}
	}

	[Category("Appearance")]
	[Description("轨道未选中颜色")]
	public Color TrackColor { get; set; }

	[Category("Appearance")]
	[Description("轨道选中区颜色")]
	public Color SelectedTrackColor { get; set; }

	[Category("Appearance")]
	[Description("刻度线颜色")]
	public Color TickColor { get; set; }

	[Category("Appearance")]
	[Description("刻度文本颜色")]
	public Color TickTextColor { get; set; }

	[Category("Appearance")]
	[Description("刻度文本字体")]
	public Font TickFont { get; set; }

	[Category("Appearance")]
	[Description("数值气泡字体")]
	public Font ValueFont { get; set; }

	[Category("Appearance")]
	[Description("数值气泡颜色")]
	public Color ValueBubbleColor { get; set; }

	[Category("Appearance")]
	[Description("数值气泡是否显示")]
	public bool ValueBubbleVisible { get; set; }

	private int TrackLeft => thumbRadius + edgeMargin;

	private int TrackRight => ((Control)this).Width - thumbRadius - edgeMargin;

	public event EventHandler ValueChanged;

	public event EventHandler<RangeValChangedEventArgs> OnRangeValChangedEvent;

	public RangeSliderControl()
	{
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		minValue = 0;
		maxValue = 1000;
		lowerValue = 200;
		upperValue = 800;
		tickFrequency = 100;
		step = 10;
		minimumRange = 1;
		thumbRadius = 8;
		trackHeight = 6;
		edgeMargin = 2;
		draggingLower = false;
		draggingUpper = false;
		draggingRange = false;
		dragStartMouseX = 0;
		dragStartLowerValue = 0;
		dragStartUpperValue = 0;
		TextInterval = 1;
		ThumbColor = Color.White;
		TrackColor = Color.FromArgb(70, 70, 72);
		SelectedTrackColor = Color.FromArgb(0, 122, 204);
		TickColor = Color.Gray;
		TickTextColor = Color.LightGray;
		TickFont = new Font("Segoe UI", 8f);
		ValueFont = new Font("Segoe UI", 9f);
		ValueBubbleColor = Color.FromArgb(0, 122, 204);
		ValueBubbleVisible = true;
		components = null;
		((UserControl)this)._002Ector();
		InitializeComponent();
		((Control)this).DoubleBuffered = true;
		((Control)this).Height = 100;
		((Control)this).MinimumSize = new Size(80, 60);
		((Control)this).BackColor = Color.FromArgb(45, 45, 48);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		((Control)this).OnPaint(e);
		Graphics graphics = e.Graphics;
		graphics.SmoothingMode = (SmoothingMode)4;
		int trackLeft = TrackLeft;
		int trackRight = TrackRight;
		int y = ((Control)this).Height / 2;
		DrawTrack(graphics, trackLeft, trackRight, y);
		DrawTicks(graphics, trackLeft, trackRight, y);
		DrawThumb(graphics, ValueToX(lowerValue), y);
		DrawThumb(graphics, ValueToX(upperValue), y);
		DrawValueBubble(graphics, lowerValue, ValueToX(lowerValue));
		DrawValueBubble(graphics, upperValue, ValueToX(upperValue));
	}

	private void DrawTrack(Graphics g, int left, int right, int y)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		Rectangle rectangle = new Rectangle(left, y - trackHeight / 2, right - left, trackHeight);
		Brush val = (Brush)new SolidBrush(TrackColor);
		try
		{
			g.FillRectangle(val, rectangle);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		int num = ValueToX(lowerValue);
		int num2 = ValueToX(upperValue);
		Brush val2 = (Brush)new SolidBrush(SelectedTrackColor);
		try
		{
			g.FillRectangle(val2, num, y - trackHeight / 2, num2 - num, trackHeight);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DrawThumb(Graphics g, int x, int y)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		Rectangle rectangle = new Rectangle(x - thumbRadius, y - thumbRadius, thumbRadius * 2, thumbRadius * 2);
		Brush val = (Brush)new SolidBrush(ThumbColor);
		try
		{
			g.FillEllipse(val, rectangle);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		Pen val2 = new Pen(SelectedTrackColor, 2f);
		try
		{
			g.DrawEllipse(val2, rectangle);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void DrawTicks(Graphics g, int left, int right, int y)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		if (TickFrequency <= 0)
		{
			return;
		}
		Pen val = new Pen(TickColor);
		try
		{
			Brush val2 = (Brush)new SolidBrush(TickTextColor);
			try
			{
				int num = 0;
				for (int i = Minimum; i <= Maximum; i += TickFrequency)
				{
					int num2 = ValueToX(i);
					g.DrawLine(val, num2, y + 12, num2, y + 20);
					if (TextInterval > 0 && num % TextInterval == 0)
					{
						string text = i.ToString();
						SizeF sizeF = g.MeasureString(text, TickFont);
						float num3 = ((i == Minimum) ? ((float)left) : ((i != Maximum) ? ((float)num2 - sizeF.Width / 2f) : ((float)right - sizeF.Width)));
						g.DrawString(text, TickFont, val2, num3, (float)(y + 22));
					}
					num++;
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawValueBubble(Graphics g, int value, int x)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		if (!ValueBubbleVisible)
		{
			return;
		}
		string text = value.ToString();
		SizeF sizeF = g.MeasureString(text, ValueFont);
		int num = (int)sizeF.Width + 10;
		int num2 = (int)sizeF.Height + 6;
		int num3 = x - num / 2;
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num3 + num > ((Control)this).Width)
		{
			num3 = ((Control)this).Width - num;
		}
		int y = ((Control)this).Height / 2 - thumbRadius - num2 - 6;
		Rectangle r = new Rectangle(num3, y, num, num2);
		GraphicsPath val = RoundedRect(r, 4);
		try
		{
			Brush val2 = (Brush)new SolidBrush(ValueBubbleColor);
			try
			{
				Brush val3 = (Brush)new SolidBrush(Color.White);
				try
				{
					g.FillPath(val2, val);
					g.DrawString(text, ValueFont, val3, (float)r.X + ((float)num - sizeF.Width) / 2f, (float)r.Y + ((float)num2 - sizeF.Height) / 2f);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		int num = ValueToX(lowerValue);
		int num2 = ValueToX(upperValue);
		if (Math.Abs(e.X - num) <= thumbRadius)
		{
			draggingLower = true;
		}
		else if (Math.Abs(e.X - num2) <= thumbRadius)
		{
			draggingUpper = true;
		}
		else if (e.X > num + thumbRadius && e.X < num2 - thumbRadius)
		{
			draggingRange = true;
			dragStartMouseX = e.X;
			dragStartLowerValue = lowerValue;
			dragStartUpperValue = upperValue;
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (draggingLower)
		{
			LowerValue = XToValue(e.X);
		}
		else if (draggingUpper)
		{
			UpperValue = XToValue(e.X);
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		draggingLower = false;
		draggingUpper = false;
		draggingRange = false;
		OnRangeValChangedEvent?.Invoke(this, new RangeValChangedEventArgs(lowerValue, upperValue));
	}

	private int ValueToX(int value)
	{
		double num = (double)(value - Minimum) / (double)(Maximum - Minimum);
		return TrackLeft + (int)((double)(TrackRight - TrackLeft) * num);
	}

	private int XToValue(int x)
	{
		double val = (double)(x - TrackLeft) / (double)(TrackRight - TrackLeft);
		val = Math.Max(0.0, Math.Min(1.0, val));
		return Minimum + (int)((double)(Maximum - Minimum) * val);
	}

	private int Snap(int value)
	{
		if (Step <= 1)
		{
			return value;
		}
		return (int)(Math.Round((double)value / (double)Step) * (double)Step);
	}

	private GraphicsPath RoundedRect(Rectangle r, int radius)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		GraphicsPath val = new GraphicsPath();
		val.AddArc(r.X, r.Y, radius, radius, 180f, 90f);
		val.AddArc(r.Right - radius, r.Y, radius, radius, 270f, 90f);
		val.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0f, 90f);
		val.AddArc(r.X, r.Bottom - radius, radius, radius, 90f, 90f);
		val.CloseFigure();
		return val;
	}

	private void RangeSliderControl_Load(object sender, EventArgs e)
	{
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((ContainerControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		((Control)this).SuspendLayout();
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Name = "RangeSliderControl";
		((UserControl)this).Load += RangeSliderControl_Load;
		((Control)this).ResumeLayout(false);
	}
}
