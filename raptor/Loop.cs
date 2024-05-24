using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using generate_interface;
using parse_tree;

namespace raptor;

[Serializable]
public class Loop : BinaryComponent
{
	public int bottom;

	public int end_first_connector;

	public int diamond_top;

	public int after_bottom;

	public int x_left;

	public int y_left;

	public int x_right;

	public int y_right;

	public int left_connector_y;

	public int right_connector_y;

	public int line_height;

	public bool light_head;

	private bool has_diamond_breakpoint;

	private string LP;

	public Component before_Child
	{
		get
		{
			return first_child;
		}
		set
		{
			first_child = value;
		}
	}

	public Component after_Child
	{
		get
		{
			return second_child;
		}
		set
		{
			second_child = value;
		}
	}

	public Loop(int height, int width, string str_name)
		: base(height, width, str_name)
	{
		init();
	}

	public Loop(Component Successor, int height, int width, string str_name)
		: base(Successor, height, width, str_name)
	{
		init();
	}

	private static bool paren_more_right(string txt)
	{
		int num = 0;
		for (int i = 0; i < txt.Length; i++)
		{
			switch (txt[i])
			{
			case '(':
				num++;
				break;
			case ')':
				num--;
				if (num == -1)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	public Loop(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		before_Child = (Component)info.GetValue("_before_Child", typeof(Component));
		after_Child = (Component)info.GetValue("_after_Child", typeof(Component));
		bottom = (int)info.GetValue("_bottom", typeof(int));
		end_first_connector = (int)info.GetValue("_min_before_bottom", typeof(int));
		diamond_top = (int)info.GetValue("_before_bottom", typeof(int));
		after_bottom = (int)info.GetValue("_after_bottom", typeof(int));
		x_left = (int)info.GetValue("_x_left", typeof(int));
		y_left = (int)info.GetValue("_y_left", typeof(int));
		x_right = (int)info.GetValue("_x_right", typeof(int));
		y_right = (int)info.GetValue("_y_right", typeof(int));
		left_connector_y = (int)info.GetValue("_left_connector_y", typeof(int));
		right_connector_y = (int)info.GetValue("_right_connector_y", typeof(int));
		line_height = (int)info.GetValue("_line_height", typeof(int));
		LP = (string)info.GetValue("_LP", typeof(string));
		if (Component.negate_loops && base.Text != null && base.Text != "")
		{
			if (base.Text.Length < 6 || base.Text.Substring(0, 5) != "not (" || base.Text[base.Text.Length - 1] != ')' || paren_more_right(base.Text.Substring(5, base.Text.Length - 6)))
			{
				base.Text = "not (" + base.Text + ")";
			}
			else
			{
				base.Text = base.Text.Substring(5, base.Text.Length - 6);
			}
		}
		result = interpreter_pkg.conditional_syntax(base.Text, this);
		if (result.valid)
		{
			parse_tree = result.tree;
			return;
		}
		if (!Component.warned_about_error && base.Text != "")
		{
			MessageBox.Show("Unknown error: \n" + base.Text + "\nis not recognized.");
			Component.warned_about_error = true;
		}
		base.Text = "";
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("_before_Child", before_Child);
		info.AddValue("_after_Child", after_Child);
		info.AddValue("_bottom", bottom);
		info.AddValue("_min_before_bottom", end_first_connector);
		info.AddValue("_before_bottom", diamond_top);
		info.AddValue("_after_bottom", after_bottom);
		info.AddValue("_x_left", x_left);
		info.AddValue("_y_left", y_left);
		info.AddValue("_x_right", x_right);
		info.AddValue("_y_right", y_right);
		info.AddValue("_left_connector_y", left_connector_y);
		info.AddValue("_right_connector_y", right_connector_y);
		info.AddValue("_line_height", line_height);
		info.AddValue("_LP", LP);
		base.GetObjectData(info, ctxt);
	}

	public override bool break_now()
	{
		if (!has_breakpoint || !light_head)
		{
			if (has_diamond_breakpoint)
			{
				return !light_head;
			}
			return false;
		}
		return true;
	}

	public override void Toggle_Breakpoint(int x, int y)
	{
		if (over_Diamond(x, y))
		{
			has_diamond_breakpoint = !has_diamond_breakpoint;
		}
		else
		{
			has_breakpoint = !has_breakpoint;
		}
	}

	public override void Clear_Breakpoints()
	{
		has_breakpoint = false;
		has_diamond_breakpoint = false;
		if (before_Child != null)
		{
			before_Child.Clear_Breakpoints();
		}
		if (after_Child != null)
		{
			after_Child.Clear_Breakpoints();
		}
		if (Successor != null)
		{
			Successor.Clear_Breakpoints();
		}
	}

	public override Component Find_Predecessor(Component c)
	{
		if (before_Child == c)
		{
			return this;
		}
		if (after_Child == c)
		{
			if (before_Child != null)
			{
				return before_Child.find_end();
			}
			return this;
		}
		if (before_Child != null)
		{
			Component component = before_Child.Find_Predecessor(c);
			if (component != null)
			{
				return component;
			}
		}
		if (after_Child != null)
		{
			Component component2 = after_Child.Find_Predecessor(c);
			if (component2 != null)
			{
				return component2;
			}
		}
		return base.Find_Predecessor(c);
	}

	public override void draw(Graphics gr, int x, int y)
	{
		base.X = x;
		base.Y = y;
		height_of_text = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_times).Height);
		width_of_text = Convert.ToInt32(gr.MeasureString(base.Text + "XX", PensBrushes.default_times).Width);
		int num = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_arial).Width);
		int num2 = Convert.ToInt32(gr.MeasureString("No", PensBrushes.default_arial).Width);
		gr.SmoothingMode = SmoothingMode.HighQuality;
		bool flag = !((double)scale <= 0.4) && head_heightOrig >= 10 && Component.text_visible;
		Pen pen = PensBrushes.blue_pen;
		Pen diamond_pen = PensBrushes.blue_pen;
		Pen pen2 = PensBrushes.blue_pen;
		if (base.selected)
		{
			pen = PensBrushes.red_pen;
			diamond_pen = PensBrushes.red_pen;
			pen2 = PensBrushes.red_pen;
		}
		else if (running && !light_head)
		{
			pen = PensBrushes.blue_pen;
			diamond_pen = PensBrushes.chartreuse_pen;
			pen2 = PensBrushes.blue_pen;
		}
		else if (running && light_head)
		{
			pen = PensBrushes.chartreuse_pen;
			diamond_pen = PensBrushes.blue_pen;
			pen2 = PensBrushes.blue_pen;
		}
		else if (is_compressed && have_child_running())
		{
			pen = PensBrushes.chartreuse_pen;
			diamond_pen = PensBrushes.chartreuse_pen;
			pen2 = PensBrushes.blue_pen;
		}
		if (!Component.USMA_mode)
		{
			gr.DrawEllipse(pen, x - base.W / 2, y, base.W, base.H);
			end_first_connector = base.Y + base.H + base.H / 2;
		}
		else
		{
			gr.DrawEllipse(pen, x - base.W / 8, y, base.W / 4, base.W / 4);
			end_first_connector = base.Y + base.W / 4 + base.H / 2;
		}
		if (has_breakpoint)
		{
			StopSign.Draw(gr, x - base.W / 2 - base.W / 6 - 2, y, base.W / 6);
		}
		if (!Component.USMA_mode)
		{
			gr.DrawLine(pen, x, y, x + base.CL / 4, y + base.CL / 4);
			gr.DrawLine(pen, x, y, x + base.CL / 4, y - base.CL / 4);
		}
		if (before_Child != null && after_Child != null && !is_compressed)
		{
			int right = before_Child.FP.right;
			int right2 = after_Child.FP.right;
			int num3 = ((right > right2) ? right : right2);
			x_right = x + base.W / 2 + num3;
			int left = after_Child.FP.left;
			x_left = x - base.W / 2 - left;
		}
		else if (before_Child != null && !is_compressed)
		{
			int right = before_Child.FP.right;
			x_right = x + base.W / 2 + right;
			x_left = x - base.W / 2 - base.W / 2;
		}
		else if (after_Child != null && !is_compressed)
		{
			int right2 = after_Child.FP.right;
			x_right = x + base.W / 2 + right2;
			int left = after_Child.FP.left;
			x_left = x - base.W / 2 - left;
		}
		else
		{
			x_right = x + base.W / 2 + base.W / 2;
			x_left = x - base.W / 2 - base.W / 2;
		}
		if (flag && Is_Wide_Diamond())
		{
			int num4 = x + drawing_text_width / 2 + base.W / 2 + base.W / 4;
			int num5 = x - drawing_text_width / 2 - base.W / 2 - base.W / 4;
			if (num4 > x_right)
			{
				x_right = num4;
			}
			if (num5 < x_left)
			{
				x_left = num5;
			}
		}
		int y2 = (Component.USMA_mode ? (y + base.W / 4) : (y + base.H));
		gr.DrawLine(pen2, x, y2, x, end_first_connector);
		gr.DrawLine(pen2, x, end_first_connector, x - base.CL / 4, end_first_connector - base.CL / 4);
		gr.DrawLine(pen2, x, end_first_connector, x + base.CL / 4, end_first_connector - base.CL / 4);
		if (before_Child != null && !is_compressed)
		{
			diamond_top = end_first_connector + before_Child.FP.height + base.CL;
			before_Child.draw(gr, x, end_first_connector);
			gr.DrawLine(pen2, x, end_first_connector + before_Child.FP.height, x, diamond_top);
			gr.DrawLine(pen2, x, diamond_top, x - base.CL / 4, diamond_top - base.CL / 4);
			gr.DrawLine(pen2, x, diamond_top, x + base.CL / 4, diamond_top - base.CL / 4);
		}
		else
		{
			diamond_top = end_first_connector;
		}
		Draw_Diamond_and_Text(gr, x, diamond_top, base.Text, diamond_pen, flag);
		if (has_diamond_breakpoint)
		{
			StopSign.Draw(gr, x - base.W / 2 - base.W / 6 - 2, diamond_top, base.W / 6);
		}
		gr.DrawLine(pen2, x, diamond_top + base.H, x, diamond_top + base.H + base.H / 2);
		gr.DrawLine(pen2, x, diamond_top + base.H + base.H / 2, x - base.CL / 4, diamond_top + base.H + base.H / 2 - base.CL / 4);
		gr.DrawLine(pen2, x, diamond_top + base.H + base.H / 2, x + base.CL / 4, diamond_top + base.H + base.H / 2 - base.CL / 4);
		if (after_Child != null && !is_compressed)
		{
			after_bottom = diamond_top + base.H + base.H / 2 + after_Child.FP.height + base.CL;
			after_Child.draw(gr, x, diamond_top + base.H + base.H / 2);
			gr.DrawLine(pen2, x, diamond_top + base.H + base.H / 2 + after_Child.FP.height, x, after_bottom);
			gr.DrawLine(pen2, x, after_bottom, x - base.CL / 4, after_bottom - base.CL / 4);
			gr.DrawLine(pen2, x, after_bottom, x + base.CL / 4, after_bottom - base.CL / 4);
		}
		else
		{
			after_bottom = diamond_top + base.H + base.H / 2;
		}
		gr.DrawLine(pen2, x, after_bottom, x_right, after_bottom);
		if (!Component.USMA_mode)
		{
			gr.DrawLine(pen2, x_right, after_bottom, x_right, y + base.H / 2);
			gr.DrawLine(pen2, x_right, y + base.H / 2, x + base.W / 2, y + base.H / 2);
			gr.DrawLine(pen2, x + base.W / 2, y + base.H / 2, x + base.W / 2 + base.CL / 4, y + base.H / 2 - base.CL / 4);
			gr.DrawLine(pen2, x + base.W / 2, y + base.H / 2, x + base.W / 2 + base.CL / 4, y + base.H / 2 + base.CL / 4);
		}
		else
		{
			gr.DrawLine(pen2, x_right, after_bottom, x_right, y + base.W / 8);
			gr.DrawLine(pen2, x_right, y + base.W / 8, x + base.W / 8, y + base.W / 8);
			gr.DrawLine(pen2, x + base.W / 8, y + base.W / 8, x + base.W / 8 + base.CL / 4, y + base.W / 8 - base.CL / 4);
			gr.DrawLine(pen2, x + base.W / 8, y + base.W / 8, x + base.W / 8 + base.CL / 4, y + base.W / 8 + base.CL / 4);
		}
		if (flag && Is_Wide_Diamond())
		{
			gr.DrawLine(pen2, x_left, diamond_top + base.H / 2, x - drawing_text_width / 2 - base.W / 4, diamond_top + base.H / 2);
		}
		else
		{
			gr.DrawLine(pen2, x_left, diamond_top + base.H / 2, x - base.W / 2, diamond_top + base.H / 2);
		}
		gr.DrawLine(pen2, x_left, diamond_top + base.H / 2, x_left, after_bottom + base.H / 2);
		gr.DrawLine(pen2, x_left, after_bottom + base.H / 2, x, after_bottom + base.H / 2);
		if (Successor != null)
		{
			gr.DrawLine(pen2, x, after_bottom + base.H / 2, x, after_bottom + base.H / 2 + base.CL);
			gr.DrawLine(pen2, x, after_bottom + base.H / 2 + base.CL, x - base.CL / 4, after_bottom + base.H / 2 + base.CL - base.CL / 4);
			gr.DrawLine(pen2, x, after_bottom + base.H / 2 + base.CL, x + base.CL / 4, after_bottom + base.H / 2 + base.CL - base.CL / 4);
			Successor.draw(gr, x, after_bottom + base.H / 2 + base.CL);
		}
		if (base.W > 30)
		{
			LP = "Loop";
		}
		else
		{
			LP = "";
		}
		if (flag)
		{
			if (!Component.reverse_loop_logic)
			{
				if (Is_Wide_Diamond())
				{
					gr.DrawString("Yes", PensBrushes.default_arial, PensBrushes.blackbrush, x - drawing_text_width / 2 - base.W / 4 - num / 2, diamond_top + base.H / 2 - 4, PensBrushes.centered_stringFormat);
				}
				else
				{
					gr.DrawString("Yes", PensBrushes.default_arial, PensBrushes.blackbrush, x - base.W / 2 - num, diamond_top + base.H / 2 - 4, PensBrushes.centered_stringFormat);
				}
				gr.DrawString("No", PensBrushes.default_arial, PensBrushes.blackbrush, x + num2, diamond_top + base.H + 5, PensBrushes.centered_stringFormat);
			}
			else
			{
				if (Is_Wide_Diamond())
				{
					gr.DrawString("No", PensBrushes.default_arial, PensBrushes.blackbrush, x - drawing_text_width / 2 - base.W / 4 - num2 / 2, diamond_top + base.H / 2 - 4, PensBrushes.centered_stringFormat);
				}
				else
				{
					gr.DrawString("No", PensBrushes.default_arial, PensBrushes.blackbrush, x - base.W / 2 - num2, diamond_top + base.H / 2 - 4, PensBrushes.centered_stringFormat);
				}
				gr.DrawString("Yes", PensBrushes.default_arial, PensBrushes.blackbrush, x + num, diamond_top + base.H + 5, PensBrushes.centered_stringFormat);
			}
			rect = new System.Drawing.Rectangle(x - base.W / 2, base.Y + base.H * 6 / 16, base.W, height_of_text);
			if (!Component.USMA_mode)
			{
				gr.DrawString(LP, PensBrushes.default_times, PensBrushes.blackbrush, rect, PensBrushes.centered_stringFormat);
			}
		}
		if (flag)
		{
			base.draw(gr, x, y);
		}
	}

	public override bool contains(int x, int y)
	{
		int num = diamond_top + base.H;
		num -= base.H / 2;
		bool flag;
		bool flag2;
		if (!Component.USMA_mode)
		{
			flag = Math.Abs(x - base.X) <= base.W / 2;
			flag2 = Math.Abs(y - (base.Y + base.H / 2)) <= base.H / 2;
		}
		else
		{
			flag = Math.Abs(x - base.X) <= base.W / 8;
			flag2 = Math.Abs(y - (base.Y + base.W / 8)) <= base.W / 8;
		}
		bool flag3 = Math.Abs(y - num) <= base.H / 2;
		if (!(flag && flag2))
		{
			return Diamond_Bounded_X(x) && flag3;
		}
		return true;
	}

	public override bool overplus(int x, int y)
	{
		bool num = Math.Abs(y - (diamond_top + base.H / 8)) <= base.H / 8;
		bool flag = ((!Is_Wide_Diamond()) ? (Math.Abs(x - (base.X - base.W / 2 + base.H / 8)) <= base.H / 8) : (Math.Abs(x - (base.X - base.W / 4 - drawing_text_width / 2 + base.H / 8)) <= base.H / 8));
		return num && flag;
	}

	public override bool contains(System.Drawing.Rectangle rec)
	{
		int y = diamond_top;
		System.Drawing.Rectangle rectangle = (Component.USMA_mode ? new System.Drawing.Rectangle(base.X - base.W / 8 + 1, base.Y, base.W / 4 - 2, base.H) : new System.Drawing.Rectangle(base.X - base.W / 2 + 2, base.Y, base.W - 4, base.H));
		System.Drawing.Rectangle rectangle2 = new System.Drawing.Rectangle(base.X - Diamond_Width() / 2 + 2, y, Diamond_Width() - 4, base.H);
		if (!rec.IntersectsWith(rectangle))
		{
			return rec.IntersectsWith(rectangle2);
		}
		return true;
	}

	public bool over_Diamond(int x, int y)
	{
		int num = diamond_top + base.H;
		num -= base.H / 2;
		bool num2 = Math.Abs(x - base.X) <= base.W / 2;
		bool flag = Math.Abs(y - num) <= base.H / 2;
		return num2 && flag;
	}

	public override bool overline(int x, int y, int connector_y)
	{
		bool flag = Math.Abs(x - x_left) < proximity;
		bool flag2 = y < after_bottom + base.H / 2 && y > diamond_top + base.H / 2;
		if (Successor != null)
		{
			bool num = Math.Abs(x - base.X) < proximity;
			bool flag3 = y < after_bottom + base.H / 2 + base.CL && y > after_bottom;
			if (!(num && flag3))
			{
				return flag && flag2;
			}
			return true;
		}
		bool num2 = Math.Abs(x - base.X) < proximity;
		bool flag4 = y < connector_y && y > after_bottom + base.H / 2;
		if (!(num2 && flag4))
		{
			return flag && flag2;
		}
		return true;
	}

	public bool overbefore(int x, int y, int connector_y)
	{
		bool num = Math.Abs(x - base.X) < proximity;
		bool flag = !Component.USMA_mode && Component.Current_Mode != Mode.Expert && y < end_first_connector && y > base.Y + base.H;
		return num && flag;
	}

	public bool overafter(int x, int y, int connector_y)
	{
		bool num = Math.Abs(x - base.X) < proximity;
		bool flag = y < diamond_top + base.H + base.H / 2 && y > diamond_top + base.H;
		return num && flag;
	}

	public override void Scale(float new_scale)
	{
		base.H = (int)Math.Round(scale * (float)head_heightOrig);
		base.W = (int)Math.Round(scale * (float)head_widthOrig);
		if (before_Child != null)
		{
			before_Child.scale = scale;
			before_Child.Scale(new_scale);
		}
		if (after_Child != null)
		{
			after_Child.scale = scale;
			after_Child.Scale(new_scale);
		}
		if (Successor != null)
		{
			Successor.scale = scale;
			Successor.Scale(new_scale);
		}
		base.Scale(new_scale);
	}

	public override void footprint(Graphics gr)
	{
		init();
		Diamond_Footprint(gr, (double)scale > 0.4, base.W / 2 + base.W / 4);
		int num = 0;
		int num2 = 0;
		if (!Component.USMA_mode)
		{
			FP.height = base.H + base.H / 2 + base.H + base.H / 2 + base.H / 2;
		}
		else
		{
			FP.height = base.W / 4 + base.H / 2 + base.H + base.H / 2 + base.H / 2;
		}
		if (after_Child != null && before_Child != null && !is_compressed)
		{
			before_Child.footprint(gr);
			after_Child.footprint(gr);
			int left = before_Child.FP.left;
			int left2 = after_Child.FP.left;
			num = ((left > left2) ? left : left2) + base.W / 2;
			int right = before_Child.FP.right;
			int right2 = after_Child.FP.right;
			num2 = ((right > right2) ? right : right2) + base.W / 2;
			FP.height += before_Child.FP.height + after_Child.FP.height + base.CL + base.CL;
		}
		else if (before_Child != null && !is_compressed)
		{
			before_Child.footprint(gr);
			num = before_Child.FP.left + base.W / 2;
			num2 = before_Child.FP.right + base.W / 2;
			FP.height += before_Child.FP.height + base.CL;
		}
		else if (after_Child != null && !is_compressed)
		{
			after_Child.footprint(gr);
			num = after_Child.FP.left + base.W / 2;
			num2 = after_Child.FP.right + base.W / 2;
			FP.height += after_Child.FP.height + base.CL;
		}
		if (num > FP.left)
		{
			FP.left = num;
		}
		if (num2 > FP.right)
		{
			FP.right = num2;
		}
		if (Successor != null)
		{
			Successor.footprint(gr);
			if (FP.left < Successor.FP.left)
			{
				FP.left = Successor.FP.left;
			}
			if (FP.right < Successor.FP.right)
			{
				FP.right = Successor.FP.right;
			}
			FP.height = FP.height + base.CL + Successor.FP.height;
		}
	}

	public override void init()
	{
		FP.left = base.W;
		FP.right = base.W;
		FP.height = 2 * base.H + base.H / 2 + base.H / 2 + base.H / 2;
	}

	public override bool insert(Component newObj, int x, int y, int connector_y)
	{
		bool flag = false;
		if (overline(x, y, connector_y))
		{
			if (newObj != null)
			{
				Component component = newObj.find_end();
				newObj.set_parent_info(is_child, is_beforeChild, is_afterChild, parent);
				component.Successor = Successor;
				Successor = newObj;
			}
			return true;
		}
		if (overbefore(x, y, connector_y))
		{
			if (!is_compressed)
			{
				if (newObj != null)
				{
					newObj.set_parent_info(is_child: true, is_before_child: true, is_after_child: false, this);
					if (before_Child != null)
					{
						newObj.find_end().Successor = before_Child;
					}
					before_Child = newObj;
				}
				return true;
			}
			if (newObj != null)
			{
				MessageBox.Show("Can't insert in collapsed symbol");
			}
			return false;
		}
		if (overafter(x, y, connector_y))
		{
			if (!is_compressed)
			{
				if (newObj != null)
				{
					newObj.set_parent_info(is_child: true, is_before_child: false, is_after_child: true, this);
					if (after_Child != null)
					{
						newObj.find_end().Successor = after_Child;
					}
					after_Child = newObj;
				}
				return true;
			}
			if (newObj != null)
			{
				MessageBox.Show("Can't insert in collapsed symbol");
			}
			return false;
		}
		if (before_Child != null)
		{
			flag = before_Child.insert(newObj, x, y, diamond_top);
		}
		if (!flag && after_Child != null)
		{
			flag = after_Child.insert(newObj, x, y, after_bottom);
		}
		if (!flag && Successor != null)
		{
			flag = Successor.insert(newObj, x, y, connector_y);
		}
		return flag;
	}

	public override bool delete()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (Successor != null)
		{
			if (Successor.editable_selected())
			{
				Component component = Successor.find_selection_end();
				if (component.Successor != null)
				{
					Successor = component.Successor;
					component.Successor = null;
				}
				else
				{
					Successor = null;
				}
				return true;
			}
			flag = Successor.delete();
		}
		if (before_Child != null)
		{
			if (before_Child.selected)
			{
				Component component = before_Child.find_selection_end();
				if (component.Successor != null)
				{
					before_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					before_Child = null;
				}
				return true;
			}
			flag2 = before_Child.delete();
		}
		if (after_Child != null)
		{
			if (after_Child.selected)
			{
				Component component = after_Child.find_selection_end();
				if (component.Successor != null)
				{
					after_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					after_Child = null;
				}
				return true;
			}
			flag3 = after_Child.delete();
		}
		return flag || flag2 || flag3;
	}

	public override string getText(int x, int y)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		if (contains(x, y) && !over_Diamond(x, y))
		{
			return "Loop";
		}
		if (over_Diamond(x, y))
		{
			return base.Text;
		}
		if (Successor != null)
		{
			text = Successor.getText(x, y);
		}
		if (before_Child != null && !is_compressed)
		{
			text2 = before_Child.getText(x, y);
		}
		if (after_Child != null && !is_compressed)
		{
			text3 = after_Child.getText(x, y);
		}
		return text + text2 + text3;
	}

	public override bool setText(int x, int y, Visual_Flow_Form form)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (contains(x, y))
		{
			new Control_Dlg(this, form, is_loop: true).ShowDialog();
			return true;
		}
		if (Successor != null)
		{
			flag = Successor.setText(x, y, form);
		}
		if (before_Child != null && !is_compressed)
		{
			flag2 = before_Child.setText(x, y, form);
		}
		if (after_Child != null && !is_compressed)
		{
			flag3 = after_Child.setText(x, y, form);
		}
		return flag || flag2 || flag3;
	}

	public override Component Clone()
	{
		Loop loop = (Loop)base.Clone();
		if (before_Child != null)
		{
			loop.before_Child = before_Child.Clone();
			loop.before_Child.set_parent_info(is_child: true, is_before_child: true, is_after_child: false, loop);
		}
		if (after_Child != null)
		{
			loop.after_Child = after_Child.Clone();
			loop.after_Child.set_parent_info(is_child: true, is_before_child: false, is_after_child: true, loop);
		}
		return loop;
	}

	public override bool cut(Visual_Flow_Form VF)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (Successor != null)
		{
			if (Successor.editable_selected())
			{
				Component successor = Successor;
				Component component = Successor.find_selection_end();
				if (component.Successor != null)
				{
					Successor = component.Successor;
					component.Successor = null;
				}
				else
				{
					Successor = null;
				}
				VF.clipboard = successor;
				VF.clipboard.reset();
				return true;
			}
			flag3 = Successor.cut(VF);
		}
		if (before_Child != null)
		{
			if (before_Child.selected)
			{
				Component successor = before_Child;
				Component component = before_Child.find_selection_end();
				if (component.Successor != null)
				{
					before_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					before_Child = null;
				}
				VF.clipboard = successor;
				VF.clipboard.reset();
				return true;
			}
			flag = before_Child.cut(VF);
		}
		if (after_Child != null)
		{
			if (after_Child.selected)
			{
				Component successor = after_Child;
				Component component = after_Child.find_selection_end();
				if (component.Successor != null)
				{
					after_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					after_Child = null;
				}
				VF.clipboard = successor;
				VF.clipboard.reset();
				return true;
			}
			flag2 = after_Child.cut(VF);
		}
		return flag || flag2 || flag3;
	}

	public override bool copy(Visual_Flow_Form VF)
	{
		if (base.selected)
		{
			return base.copy(VF);
		}
		if (before_Child != null && before_Child.copy(VF))
		{
			return true;
		}
		if (after_Child != null && after_Child.copy(VF))
		{
			return true;
		}
		if (Successor != null)
		{
			return Successor.copy(VF);
		}
		return false;
	}

	public override Component First_Of()
	{
		light_head = true;
		return this;
	}

	public override void Emit_Code(typ gen)
	{
		boolean_expression boolean_expression = parse_tree as boolean_expression;
		object o;
		if (Component.reverse_loop_logic)
		{
			o = gen.Loop_Start(before_Child == null, is_negated: true);
		}
		else
		{
			bool flag = boolean_expression.top_level_negated();
			o = gen.Loop_Start(before_Child == null, flag);
			if (flag)
			{
				boolean_expression = boolean_expression.remove_negation();
			}
		}
		if (before_Child != null)
		{
			before_Child.Emit_Code(gen);
		}
		gen.Loop_Start_Condition(o);
		if (parse_tree != null)
		{
			interpreter_pkg.emit_code(boolean_expression, base.Text, gen);
		}
		gen.Loop_End_Condition(o);
		if (after_Child != null)
		{
			after_Child.Emit_Code(gen);
		}
		gen.Loop_End(o);
		if (Successor != null)
		{
			Successor.Emit_Code(gen);
		}
	}
}
