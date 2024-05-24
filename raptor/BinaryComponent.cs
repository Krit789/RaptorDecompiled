using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using generate_interface;

namespace raptor;

public abstract class BinaryComponent : Component
{
	protected Component first_child;

	protected Component second_child;

	protected bool is_compressed;

	public BinaryComponent(int h, int w, string str_name)
		: base(h, w, str_name)
	{
	}

	public BinaryComponent(Component S, int h, int w, string str_name)
		: base(S, h, w, str_name)
	{
	}

	public BinaryComponent(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		if (incoming_serialization_version >= 10)
		{
			is_compressed = (bool)info.GetValue("_is_compressed", typeof(bool));
		}
		else
		{
			is_compressed = false;
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("_is_compressed", is_compressed);
		base.GetObjectData(info, ctxt);
	}

	protected bool Diamond_Bounded_X(int x)
	{
		bool flag = Math.Abs(x - base.X) <= base.W / 2;
		if (Is_Wide_Diamond())
		{
			flag = Math.Abs(x - base.X) <= drawing_text_width / 2 + 3 * base.W / 8;
		}
		return flag;
	}

	protected int Diamond_Width()
	{
		if (Is_Wide_Diamond())
		{
			return drawing_text_width + 6 * base.W / 8;
		}
		return base.W;
	}

	public override int Count_Symbols()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (Successor != null)
		{
			num3 = Successor.Count_Symbols();
		}
		if (first_child != null)
		{
			num = first_child.Count_Symbols();
		}
		if (second_child != null)
		{
			num2 = second_child.Count_Symbols();
		}
		return 1 + num + num2 + num3;
	}

	public override bool SelectRegion(System.Drawing.Rectangle rec)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (Successor != null)
		{
			flag = Successor.SelectRegion(rec);
		}
		if (first_child != null && !is_compressed)
		{
			flag2 = first_child.SelectRegion(rec);
		}
		if (second_child != null && !is_compressed)
		{
			flag3 = second_child.SelectRegion(rec);
		}
		if (contains(rec) || (flag2 && flag3) || (flag && (flag2 || flag3)))
		{
			base.selected = true;
			if (first_child != null)
			{
				first_child.selectAll();
			}
			if (second_child != null)
			{
				second_child.selectAll();
			}
			return true;
		}
		base.selected = false;
		return flag || flag2 || flag3;
	}

	public override void selectAll()
	{
		base.selected = true;
		if (Successor != null)
		{
			Successor.selectAll();
		}
		if (first_child != null)
		{
			first_child.selectAll();
		}
		if (second_child != null)
		{
			second_child.selectAll();
		}
	}

	public override Component select(int x, int y)
	{
		Component component = null;
		Component component2 = null;
		Component component3 = null;
		base.selected = false;
		if (first_child != null && !is_compressed)
		{
			component2 = first_child.select(x, y);
		}
		if (second_child != null && !is_compressed)
		{
			component3 = second_child.select(x, y);
		}
		if (Successor != null)
		{
			component = Successor.select(x, y);
		}
		if (contains(x, y))
		{
			base.selected = true;
			if (second_child != null)
			{
				second_child.selectAll();
			}
			if (first_child != null)
			{
				first_child.selectAll();
			}
			return this;
		}
		if (component2 != null)
		{
			return component2;
		}
		if (component3 != null)
		{
			return component3;
		}
		if (component != null)
		{
			return component;
		}
		return null;
	}

	public override Component Find_Component(int x, int y)
	{
		Component component = null;
		Component component2 = null;
		Component component3 = null;
		if (first_child != null && !is_compressed)
		{
			component2 = first_child.Find_Component(x, y);
		}
		if (second_child != null && !is_compressed)
		{
			component3 = second_child.Find_Component(x, y);
		}
		if (Successor != null)
		{
			component = Successor.Find_Component(x, y);
		}
		if (contains(x, y))
		{
			return this;
		}
		if (component2 != null)
		{
			return component2;
		}
		if (component3 != null)
		{
			return component3;
		}
		if (component != null)
		{
			return component;
		}
		return null;
	}

	public override CommentBox selectComment(int x, int y)
	{
		CommentBox commentBox = null;
		CommentBox commentBox2 = null;
		CommentBox commentBox3 = null;
		if (first_child != null && !is_compressed)
		{
			commentBox2 = first_child.selectComment(x, y);
		}
		if (second_child != null && !is_compressed)
		{
			commentBox3 = second_child.selectComment(x, y);
		}
		if (Successor != null)
		{
			commentBox = Successor.selectComment(x, y);
		}
		if (My_Comment != null)
		{
			My_Comment.selected = false;
			if (My_Comment.contains(x, y))
			{
				My_Comment.selected = true;
				return My_Comment;
			}
		}
		if (commentBox2 != null)
		{
			return commentBox2;
		}
		if (commentBox3 != null)
		{
			return commentBox3;
		}
		if (commentBox != null)
		{
			return commentBox;
		}
		return null;
	}

	public override void reset_number_method_expressions_run()
	{
		reset_this_method_expressions_run();
		if (Successor != null)
		{
			Successor.reset_number_method_expressions_run();
		}
		if (first_child != null)
		{
			Successor.reset_number_method_expressions_run();
		}
		if (second_child != null)
		{
			Successor.reset_number_method_expressions_run();
		}
	}

	public override bool has_code()
	{
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		if (Successor != null)
		{
			flag2 = Successor.has_code();
		}
		if (first_child != null)
		{
			flag3 = first_child.has_code();
		}
		if (second_child != null)
		{
			flag4 = second_child.has_code();
		}
		if (parse_tree == null)
		{
			flag = false;
		}
		return flag && flag2 && flag3 && flag4;
	}

	public override void Show_Guids()
	{
		Show_Guid();
		if (first_child != null)
		{
			first_child.Show_Guids();
		}
		if (second_child != null)
		{
			second_child.Show_Guids();
		}
		if (Successor != null)
		{
			Successor.Show_Guids();
		}
	}

	public override void mark_error()
	{
		if (Successor != null)
		{
			Successor.mark_error();
		}
		if (first_child != null)
		{
			first_child.mark_error();
		}
		if (second_child != null)
		{
			second_child.mark_error();
		}
		if (parse_tree == null)
		{
			base.Text = "Error";
			Runtime.parent.Show_Text_On_Error();
		}
	}

	protected void Draw_Diamond_and_Text(Graphics gr, int x, int diamond_top, string text, Pen diamond_pen, bool draw_text)
	{
		Pen pen = ((!base.selected) ? PensBrushes.blue_pen : PensBrushes.red_pen);
		if (Is_Wide_Diamond() && draw_text)
		{
			gr.DrawRectangle(pen, x - base.W / 4 - drawing_text_width / 2, diamond_top, base.H / 4, base.H / 4);
			if (is_compressed)
			{
				gr.DrawLine(pen, x - base.W / 4 - drawing_text_width / 2 + base.H / 8, diamond_top, x - base.W / 4 - drawing_text_width / 2 + base.H / 8, diamond_top + base.H / 4);
			}
			gr.DrawLine(pen, x - base.W / 4 - drawing_text_width / 2, diamond_top + base.H / 8, x - base.W / 4 - drawing_text_width / 2 + base.H / 4, diamond_top + base.H / 8);
			gr.DrawLine(diamond_pen, x, diamond_top, x + base.W / 8, diamond_top + base.H / 8);
			gr.DrawLine(diamond_pen, x, diamond_top, x - base.W / 8, diamond_top + base.H / 8);
			gr.DrawLine(diamond_pen, x, diamond_top + base.H, x + base.W / 8, diamond_top + 7 * base.H / 8);
			gr.DrawLine(diamond_pen, x, diamond_top + base.H, x - base.W / 8, diamond_top + 7 * base.H / 8);
			gr.DrawLine(PensBrushes.blue_dash_pen, x - drawing_text_width / 2, diamond_top + 7 * base.H / 8, x - base.W / 8, diamond_top + 7 * base.H / 8);
			gr.DrawLine(PensBrushes.blue_dash_pen, x + drawing_text_width / 2, diamond_top + 7 * base.H / 8, x + base.W / 8, diamond_top + 7 * base.H / 8);
			gr.DrawLine(PensBrushes.blue_dash_pen, x - drawing_text_width / 2, diamond_top + base.H / 8, x - base.W / 8, diamond_top + base.H / 8);
			gr.DrawLine(PensBrushes.blue_dash_pen, x + drawing_text_width / 2, diamond_top + base.H / 8, x + base.W / 8, diamond_top + base.H / 8);
			gr.DrawLine(diamond_pen, x - base.W / 4 - drawing_text_width / 2, diamond_top + base.H / 2, x - drawing_text_width / 2, diamond_top + base.H / 8);
			gr.DrawLine(diamond_pen, x - base.W / 4 - drawing_text_width / 2, diamond_top + base.H / 2, x - drawing_text_width / 2, diamond_top + 7 * base.H / 8);
			gr.DrawLine(diamond_pen, x + drawing_text_width / 2 + base.W / 4, diamond_top + base.H / 2, x + drawing_text_width / 2, diamond_top + base.H / 8);
			gr.DrawLine(diamond_pen, x + drawing_text_width / 2 + base.W / 4, diamond_top + base.H / 2, x + drawing_text_width / 2, diamond_top + 7 * base.H / 8);
		}
		else
		{
			gr.DrawRectangle(pen, x - base.W / 2, diamond_top, base.H / 4, base.H / 4);
			if (is_compressed)
			{
				gr.DrawLine(pen, x - base.W / 2 + base.H / 8, diamond_top, x - base.W / 2 + base.H / 8, diamond_top + base.H / 4);
			}
			gr.DrawLine(pen, x - base.W / 2, diamond_top + base.H / 8, x - base.W / 2 + base.H / 4, diamond_top + base.H / 8);
			gr.DrawLine(diamond_pen, x, diamond_top, x + base.W / 2, diamond_top + base.H / 2);
			gr.DrawLine(diamond_pen, x + base.W / 2, diamond_top + base.H / 2, x, diamond_top + base.H);
			gr.DrawLine(diamond_pen, x, diamond_top + base.H, x - base.W / 2, diamond_top + base.H / 2);
			gr.DrawLine(diamond_pen, x - base.W / 2, diamond_top + base.H / 2, x, diamond_top);
		}
		if (!draw_text || base.Text.Length <= 0)
		{
			return;
		}
		if (Component.full_text)
		{
			if (drawing_text_width > base.W)
			{
				rect = new System.Drawing.Rectangle(x - drawing_text_width / 2, diamond_top + base.H / 2 - height_of_text, drawing_text_width, height_of_text * 2);
			}
			else
			{
				rect = new System.Drawing.Rectangle(x - width_of_text / 2, diamond_top + base.H / 2 - height_of_text / 2, width_of_text, height_of_text);
			}
		}
		else
		{
			rect = new System.Drawing.Rectangle(x - base.W / 2, diamond_top + base.H * 7 / 16, base.W - base.W / 8, height_of_text);
		}
		if (base.Text == "Error")
		{
			gr.DrawString(base.Text, PensBrushes.default_times, PensBrushes.redbrush, rect, PensBrushes.centered_stringFormat);
		}
		else
		{
			gr.DrawString(getDrawText(), PensBrushes.default_times, PensBrushes.blackbrush, rect, PensBrushes.centered_stringFormat);
		}
	}

	protected bool Is_Wide_Diamond()
	{
		if (Component.full_text)
		{
			return drawing_text_width > 5 * base.W / 7;
		}
		return false;
	}

	protected void Diamond_Footprint(Graphics gr, bool draw_text, int buffer)
	{
		if (Component.full_text && draw_text)
		{
			int num = 5 * base.W / 7;
			int num2 = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_times).Height);
			SizeF sizeF;
			while (true)
			{
				sizeF = gr.MeasureString(getDrawText() + "XXXXX", PensBrushes.default_times, num);
				if (sizeF.Height < (float)(num2 * 5 / 2))
				{
					break;
				}
				num += base.W / 2;
			}
			if (sizeF.Height > (float)(num2 * 3 / 2) || num > 5 * base.W / 7)
			{
				FP.left = num / 2 + buffer;
				FP.right = num / 2 + buffer;
				drawing_text_width = num;
			}
			else
			{
				drawing_text_width = 5 * base.W / 7;
			}
		}
		else
		{
			drawing_text_width = 0;
		}
	}

	public override void change_compressed(bool compressed)
	{
		is_compressed = compressed;
		if (first_child != null)
		{
			first_child.change_compressed(compressed);
		}
		if (second_child != null)
		{
			second_child.change_compressed(compressed);
		}
		if (Successor != null)
		{
			Successor.change_compressed(compressed);
		}
	}

	public override bool child_running()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (running)
		{
			return true;
		}
		if (first_child != null)
		{
			flag = first_child.child_running();
		}
		if (second_child != null)
		{
			flag2 = second_child.child_running();
		}
		if (Successor != null)
		{
			flag3 = Successor.child_running();
		}
		return flag || flag2 || flag3;
	}

	public bool have_child_running()
	{
		if (first_child != null && first_child.child_running())
		{
			return true;
		}
		if (second_child != null && second_child.child_running())
		{
			return true;
		}
		return false;
	}

	public abstract bool overplus(int x, int y);

	public override bool check_expansion_click(int x, int y)
	{
		if (overplus(x, y))
		{
			is_compressed = !is_compressed;
			return true;
		}
		if (first_child != null && !is_compressed && first_child.check_expansion_click(x, y))
		{
			return true;
		}
		if (second_child != null && !is_compressed && second_child.check_expansion_click(x, y))
		{
			return true;
		}
		if (Successor != null)
		{
			return Successor.check_expansion_click(x, y);
		}
		return false;
	}

	public override bool Called_Tab(string s)
	{
		if (Successor != null && Successor.Called_Tab(s))
		{
			return true;
		}
		if (first_child != null && first_child.Called_Tab(s))
		{
			return true;
		}
		if (second_child != null && second_child.Called_Tab(s))
		{
			return true;
		}
		return false;
	}

	public override void Rename_Tab(string from, string to)
	{
		if (first_child != null)
		{
			first_child.Rename_Tab(from, to);
		}
		if (second_child != null)
		{
			second_child.Rename_Tab(from, to);
		}
		base.Rename_Tab(from, to);
	}

	public override void compile_pass1(typ gen)
	{
		if (parse_tree != null)
		{
			interpreter_pkg.compile_pass1(parse_tree, base.Text, gen);
		}
		if (first_child != null)
		{
			first_child.compile_pass1(gen);
		}
		if (second_child != null)
		{
			second_child.compile_pass1(gen);
		}
		if (Successor != null)
		{
			Successor.compile_pass1(gen);
		}
	}

	public override void collect_variable_names(IList<string> l, IDictionary<string, string> types)
	{
		if (first_child != null)
		{
			first_child.collect_variable_names(l, types);
		}
		if (second_child != null)
		{
			second_child.collect_variable_names(l, types);
		}
		if (Successor != null)
		{
			Successor.collect_variable_names(l, types);
		}
	}
}
