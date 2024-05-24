using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace raptor;

[Serializable]
public class CommentBox : ISerializable
{
	public static int current_serialization_version = 1;

	public int incoming_serialization_version;

	public string[] Text_Array;

	private int num_lines;

	public Component parent;

	private int x_location;

	private int y_location;

	private int draw_x;

	private int draw_y;

	private int height_of_text;

	private int width_of_text;

	private int height;

	private int width;

	public bool selected;

	public bool text_change = true;

	public int X
	{
		get
		{
			return x_location;
		}
		set
		{
			x_location = value;
			draw_x = (int)Math.Round(parent.scale * (float)X);
		}
	}

	public int Y
	{
		get
		{
			return y_location;
		}
		set
		{
			y_location = value;
			draw_y = (int)Math.Round(parent.scale * (float)Y);
		}
	}

	public int H
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public int W
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public CommentBox(Component my_parent)
	{
		parent = my_parent;
		Text_Array = new string[1];
		Text_Array[0] = "";
	}

	public CommentBox(SerializationInfo info, StreamingContext ctxt)
	{
		incoming_serialization_version = (int)info.GetValue("_version", typeof(int));
		num_lines = (int)info.GetValue("_num_lines", typeof(int));
		x_location = (int)info.GetValue("_x_location", typeof(int));
		y_location = (int)info.GetValue("_y_location", typeof(int));
		Text_Array = new string[num_lines];
		for (int i = 0; i < num_lines; i++)
		{
			Text_Array[i] = (string)info.GetValue("_line" + i, typeof(string));
		}
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		num_lines = Text_Array.Length;
		info.AddValue("_num_lines", num_lines);
		info.AddValue("_x_location", x_location);
		info.AddValue("_y_location", y_location);
		info.AddValue("_version", current_serialization_version);
		for (int i = 0; i < num_lines; i++)
		{
			info.AddValue("_line" + i, Text_Array[i]);
		}
	}

	private void resize(Graphics gr)
	{
		int num = 0;
		height_of_text = Convert.ToInt32(gr.MeasureString(Text_Array[0], PensBrushes.default_times).Height);
		num_lines = Text_Array.Length;
		if (Text_Array[num_lines - 1].Length <= 0)
		{
			num_lines--;
		}
		for (int i = 0; i < num_lines; i++)
		{
			width_of_text = Convert.ToInt32(gr.MeasureString(Text_Array[i], PensBrushes.default_times).Width);
			if (width_of_text > num)
			{
				num = width_of_text;
			}
		}
		width_of_text = num;
		W = width_of_text + 4;
		H = height_of_text * num_lines + 4;
	}

	public void draw(Graphics gr, int parent_x, int parent_y)
	{
		if (Text_Array != null && Text_Array.Length != 0)
		{
			if (text_change || Component.Inside_Print || Component.Just_After_Print)
			{
				resize(gr);
				text_change = false;
				Component.Just_After_Print = false;
			}
			gr.SmoothingMode = SmoothingMode.HighQuality;
			Balloon.Corner corner = ((draw_x + W / 2 < 0 && draw_y + H / 2 < parent.H / 2) ? Balloon.Corner.Lower_Right : ((draw_x + W / 2 < 0) ? Balloon.Corner.Upper_Right : ((draw_y + H / 2 <= parent.H / 2) ? Balloon.Corner.Lower_Left : Balloon.Corner.Upper_Left)));
			if (selected)
			{
				gr.DrawPath(PensBrushes.red_pen, Balloon.Make_Path(new System.Drawing.Rectangle(parent_x + draw_x, parent_y + draw_y, W + 10, H), corner));
			}
			else
			{
				gr.DrawPath(PensBrushes.green_pen, Balloon.Make_Path(new System.Drawing.Rectangle(parent_x + draw_x, parent_y + draw_y, W + 10, H), corner));
			}
			for (int i = 0; i < num_lines; i++)
			{
				gr.DrawString(layoutRectangle: new System.Drawing.Rectangle(parent_x + draw_x + 6, parent_y + draw_y + height_of_text * i, W, height_of_text), s: Text_Array[i], font: PensBrushes.default_times, brush: PensBrushes.greenbrush, format: PensBrushes.left_stringFormat);
			}
		}
	}

	public bool contains(int x, int y)
	{
		bool num = x >= parent.X + draw_x && x <= parent.X + draw_x + W;
		bool flag = y >= parent.Y + draw_y && y <= parent.Y + draw_y + H;
		return num && flag;
	}

	public System.Drawing.Rectangle Get_Bounds()
	{
		return new System.Drawing.Rectangle(parent.X + draw_x, parent.Y + draw_y, W, H);
	}

	public static System.Drawing.Rectangle Union(System.Drawing.Rectangle l, System.Drawing.Rectangle r)
	{
		int num = ((l.X >= r.X) ? r.X : l.X);
		int num2 = ((l.Y >= r.Y) ? r.Y : l.Y);
		int num3 = ((l.Right <= r.Right) ? r.Right : l.Right);
		int num4 = ((l.Bottom <= r.Bottom) ? r.Bottom : l.Bottom);
		return new System.Drawing.Rectangle(num, num2, num3 - num, num4 - num2);
	}

	public void Scale(float new_scale)
	{
		draw_x = (int)Math.Round(new_scale * (float)X);
		draw_y = (int)Math.Round(new_scale * (float)Y);
		text_change = true;
	}

	public bool select(int x, int y)
	{
		selected = false;
		if (contains(x, y))
		{
			selected = true;
		}
		return selected;
	}

	public string[] getText()
	{
		return Text_Array;
	}

	public void setText(Visual_Flow_Form form)
	{
		new Comment_Dlg(this, form).ShowDialog();
		if (text_change)
		{
			resize(form.CreateGraphics());
			text_change = false;
		}
	}

	public CommentBox Clone()
	{
		return (CommentBox)MemberwiseClone();
	}
}
