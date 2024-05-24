using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace raptor;

[Serializable]
public class Oval : Component
{
	public Oval(int height, int width, string str_name)
		: base(height, width, str_name)
	{
		init();
	}

	public Oval(Component Successor, int height, int width, string str_name)
		: base(Successor, height, width, str_name)
	{
		init();
	}

	public Oval(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
	}

	public override void draw(Graphics gr, int x, int y)
	{
		base.X = x;
		base.Y = y;
		bool flag = !((double)scale <= 0.4) && head_heightOrig >= 10 && Component.text_visible;
		height_of_text = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_times).Height);
		width_of_text = Convert.ToInt32(gr.MeasureString(base.Text + "XX", PensBrushes.default_times).Width);
		int num = ((drawing_text_width <= base.W || Component.compiled_flowchart) ? base.W : drawing_text_width);
		gr.SmoothingMode = SmoothingMode.HighQuality;
		Pen pen = (base.selected ? PensBrushes.red_pen : ((!running) ? PensBrushes.blue_pen : PensBrushes.chartreuse_pen));
		gr.DrawEllipse(pen, base.X - num / 2, base.Y, num, base.H);
		if (flag)
		{
			if (Component.full_text && !Component.compiled_flowchart)
			{
				if (drawing_text_width > base.W)
				{
					rect = new System.Drawing.Rectangle(x - drawing_text_width / 2, base.Y + base.H / 32, drawing_text_width, height_of_text * 3);
				}
				else
				{
					rect = new System.Drawing.Rectangle(x - width_of_text / 2, base.Y + base.H * 6 / 16, width_of_text, height_of_text);
				}
			}
			else
			{
				rect = new System.Drawing.Rectangle(x - base.W / 2, base.Y + base.H * 6 / 16, base.W, height_of_text);
			}
			if (Component.compiled_flowchart)
			{
				gr.DrawString("compiled", PensBrushes.default_times, PensBrushes.blackbrush, rect, PensBrushes.centered_stringFormat);
			}
			else
			{
				gr.DrawString(getDrawText(), PensBrushes.default_times, PensBrushes.blackbrush, rect, PensBrushes.centered_stringFormat);
			}
		}
		if (!Component.compiled_flowchart && Successor != null)
		{
			Pen pen2 = ((!base.selected) ? PensBrushes.blue_pen : PensBrushes.red_pen);
			drawConnector(gr, pen2);
			Successor.scale = scale;
			Successor.Scale(scale);
			Successor.draw(gr, base.X, base.Y + base.H + base.CL);
		}
		if (flag)
		{
			base.draw(gr, x, y);
		}
	}

	protected virtual void drawConnector(Graphics gr, Pen pen)
	{
		gr.DrawLine(pen, base.X, base.Y + base.H, base.X, base.Y + base.H + base.CL);
		gr.DrawLine(pen, base.X, base.Y + base.H + base.CL, base.X - base.CL / 4, base.Y + base.H + base.CL - base.CL / 4);
		gr.DrawLine(pen, base.X, base.Y + base.H + base.CL, base.X + base.CL / 4, base.Y + base.H + base.CL - base.CL / 4);
	}

	public override bool SelectRegion(System.Drawing.Rectangle rec)
	{
		base.selected = false;
		if (Successor != null)
		{
			return Successor.SelectRegion(rec);
		}
		return false;
	}

	public override void Scale(float new_scale)
	{
		base.H = (int)Math.Round(scale * (float)head_heightOrig);
		base.W = (int)Math.Round(scale * (float)head_widthOrig);
		if (Successor != null)
		{
			Successor.scale = scale;
			Successor.Scale(new_scale);
		}
		if (scale >= 2.75f)
		{
			PensBrushes.default_arial = PensBrushes.arial24;
			PensBrushes.default_times = PensBrushes.times30;
		}
		else if (scale >= 1.75f)
		{
			PensBrushes.default_arial = PensBrushes.arial16;
			PensBrushes.default_times = PensBrushes.times18;
		}
		else if (scale >= 1.5f)
		{
			PensBrushes.default_arial = PensBrushes.arial14;
			PensBrushes.default_times = PensBrushes.times16;
		}
		else if (scale >= 1.25f)
		{
			PensBrushes.default_arial = PensBrushes.arial12;
			PensBrushes.default_times = PensBrushes.times14;
		}
		else if (scale >= 1f)
		{
			PensBrushes.default_arial = PensBrushes.arial10;
			PensBrushes.default_times = PensBrushes.times12;
		}
		else if (scale >= 0.8f)
		{
			PensBrushes.default_arial = PensBrushes.arial8;
			PensBrushes.default_times = PensBrushes.times10;
		}
		else if (scale >= 0.6f)
		{
			PensBrushes.default_arial = PensBrushes.arial6;
			PensBrushes.default_times = PensBrushes.times8;
		}
		else if (scale >= 0.4f)
		{
			PensBrushes.default_arial = PensBrushes.arial4;
			PensBrushes.default_times = PensBrushes.times6;
		}
		base.Scale(new_scale);
	}

	public override bool editable_selected()
	{
		return false;
	}

	public override string getText(int x, int y)
	{
		if (contains(x, y))
		{
			return base.Text;
		}
		if (Successor != null)
		{
			return Successor.getText(x, y);
		}
		return "";
	}

	public override bool setText(int x, int y, Visual_Flow_Form form)
	{
		bool flag = false;
		if (contains(x, y))
		{
			return true;
		}
		if (Successor != null)
		{
			return Successor.setText(x, y, form);
		}
		return flag;
	}

	public override bool has_code()
	{
		bool flag = true;
		if (Successor != null)
		{
			flag = Successor.has_code();
		}
		return true && flag;
	}

	public override void mark_error()
	{
		if (Successor != null)
		{
			Successor.mark_error();
		}
	}

	public override void selectAll()
	{
		base.selected = false;
		if (Successor != null)
		{
			Successor.selectAll();
		}
	}
}
