using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace raptor;

[Serializable]
public class PensBrushes
{
	public enum family
	{
		times,
		arial,
		courier
	}

	public static Pen red_pen;

	public static Pen black_pen;

	public static Pen black_dash_pen;

	public static Pen blue_pen;

	public static Pen blue_dash_pen;

	public static Pen green_pen;

	public static Pen chartreuse_pen;

	public static FontStyle Boldstyle;

	public static FontStyle Regstyle;

	public static FontFamily times;

	public static FontFamily arial;

	public static FontFamily courier;

	public static Font times36;

	public static Font times30;

	public static Font times28;

	public static Font times24;

	public static Font times20;

	public static Font times18;

	public static Font times16;

	public static Font times14;

	public static Font times12;

	public static Font times10;

	public static Font times8;

	public static Font times6;

	public static Font times4;

	public static Font courier36;

	public static Font courier30;

	public static Font courier28;

	public static Font courier24;

	public static Font courier20;

	public static Font courier18;

	public static Font courier16;

	public static Font courier14;

	public static Font courier12;

	public static Font courier10;

	public static Font courier8;

	public static Font courier6;

	public static Font courier4;

	public static Font default_times;

	public static Font default_courier;

	public static Font default_arial;

	public static Font arial36;

	public static Font arial30;

	public static Font arial28;

	public static Font arial24;

	public static Font arial20;

	public static Font arial18;

	public static Font arial16;

	public static Font arial14;

	public static Font arial12;

	public static Font arial10;

	public static Font arial8;

	public static Font arial6;

	public static Font arial4;

	public static SolidBrush blackbrush;

	public static SolidBrush redbrush;

	public static SolidBrush greenbrush;

	public static StringFormat centered_stringFormat;

	public static StringFormat left_stringFormat;

	public static Font Get_Font(family f, int size)
	{
		return f switch
		{
			family.arial => size switch
			{
				4 => arial4, 
				6 => arial6, 
				8 => arial8, 
				10 => arial10, 
				12 => arial12, 
				14 => arial14, 
				16 => arial16, 
				18 => arial18, 
				20 => arial20, 
				24 => arial24, 
				28 => arial28, 
				30 => arial30, 
				36 => arial36, 
				_ => throw new Exception("no size:" + size + " in family: " + f), 
			}, 
			family.times => size switch
			{
				4 => times4, 
				6 => times6, 
				8 => times8, 
				10 => times10, 
				12 => times12, 
				14 => times14, 
				16 => times16, 
				18 => times18, 
				20 => times20, 
				24 => times24, 
				28 => times28, 
				30 => times30, 
				36 => times36, 
				_ => throw new Exception("no size:" + size + " in family: " + f), 
			}, 
			family.courier => size switch
			{
				4 => courier4, 
				6 => courier6, 
				8 => courier8, 
				10 => courier10, 
				12 => courier12, 
				14 => courier14, 
				16 => courier16, 
				18 => courier18, 
				20 => courier20, 
				24 => courier24, 
				28 => courier28, 
				30 => courier30, 
				36 => courier36, 
				_ => throw new Exception("no size:" + size + " in family: " + f), 
			}, 
			_ => throw new Exception("no such family"), 
		};
	}

	public static void initialize()
	{
		red_pen = new Pen(Color.Red);
		black_pen = new Pen(Color.Black);
		black_dash_pen = new Pen(Color.Black);
		blue_pen = new Pen(Color.Blue);
		blue_dash_pen = new Pen(Color.Blue);
		green_pen = new Pen(Color.Green);
		chartreuse_pen = new Pen(Color.Chartreuse, 4f);
		Boldstyle = FontStyle.Bold;
		Regstyle = FontStyle.Regular;
		try
		{
			times = new FontFamily("Times New Roman");
			if (times == null)
			{
				throw new Exception("times not found");
			}
		}
		catch
		{
			times = FontFamily.GenericSerif;
		}
		try
		{
			arial = new FontFamily("Arial");
			if (arial == null)
			{
				throw new Exception("arial not found");
			}
		}
		catch
		{
			arial = FontFamily.GenericSansSerif;
		}
		try
		{
			courier = new FontFamily("Courier New");
			if (courier == null)
			{
				throw new Exception("courier not found");
			}
		}
		catch
		{
			courier = FontFamily.GenericMonospace;
		}
		times36 = new Font(times, 36f, Regstyle);
		times30 = new Font(times, 30f, Regstyle);
		times28 = new Font(times, 28f, Regstyle);
		times24 = new Font(times, 24f, Regstyle);
		times20 = new Font(times, 20f, Regstyle);
		times18 = new Font(times, 18f, Regstyle);
		times16 = new Font(times, 16f, Regstyle);
		times14 = new Font(times, 14f, Regstyle);
		times12 = new Font(times, 12f, Regstyle);
		times10 = new Font(times, 10f, Regstyle);
		times8 = new Font(times, 8f, Regstyle);
		times6 = new Font(times, 6f, Regstyle);
		times4 = new Font(times, 4f, Regstyle);
		courier36 = new Font(courier, 36f, Regstyle);
		courier30 = new Font(courier, 30f, Regstyle);
		courier28 = new Font(courier, 28f, Regstyle);
		courier24 = new Font(courier, 24f, Regstyle);
		courier20 = new Font(courier, 20f, Regstyle);
		courier18 = new Font(courier, 18f, Regstyle);
		courier16 = new Font(courier, 16f, Regstyle);
		courier14 = new Font(courier, 14f, Regstyle);
		courier12 = new Font(courier, 12f, Regstyle);
		courier10 = new Font(courier, 10f, Regstyle);
		courier8 = new Font(courier, 8f, Regstyle);
		courier6 = new Font(courier, 6f, Regstyle);
		courier4 = new Font(courier, 4f, Regstyle);
		default_times = times10;
		default_courier = courier10;
		default_arial = arial8;
		arial36 = new Font(arial, 36f, Regstyle);
		arial30 = new Font(arial, 30f, Regstyle);
		arial28 = new Font(arial, 28f, Regstyle);
		arial24 = new Font(arial, 24f, Regstyle);
		arial20 = new Font(arial, 20f, Regstyle);
		arial18 = new Font(arial, 18f, Regstyle);
		arial16 = new Font(arial, 16f, Regstyle);
		arial14 = new Font(arial, 14f, Regstyle);
		arial12 = new Font(arial, 12f, Regstyle);
		arial10 = new Font(arial, 10f, Regstyle);
		arial8 = new Font(arial, 8f, Regstyle);
		arial6 = new Font(arial, 6f, Regstyle);
		arial4 = new Font(arial, 4f, Regstyle);
		blackbrush = new SolidBrush(Color.Black);
		redbrush = new SolidBrush(Color.Red);
		greenbrush = new SolidBrush(Color.Green);
		centered_stringFormat = new StringFormat();
		left_stringFormat = new StringFormat();
		centered_stringFormat.Alignment = StringAlignment.Center;
		centered_stringFormat.LineAlignment = StringAlignment.Center;
		left_stringFormat.Alignment = StringAlignment.Near;
		left_stringFormat.LineAlignment = StringAlignment.Center;
		black_dash_pen.DashStyle = DashStyle.DashDot;
		blue_dash_pen.DashStyle = DashStyle.DashDotDot;
		float[] dashPattern = new float[6] { 1f, 3f, 1f, 3f, 1f, 3f };
		blue_dash_pen.DashPattern = dashPattern;
	}
}
