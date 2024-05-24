using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using generate_interface;

namespace raptor;

[Serializable]
public class IF_Control : BinaryComponent
{
	public int bottom;

	public int min_bottom;

	public int x_left;

	public int y_left;

	public int x_right;

	public int y_right;

	public int left_connector_y;

	public int right_connector_y;

	public int line_height;

	public Component yes_child
	{
		get
		{
			if (Component.USMA_mode)
			{
				return second_child;
			}
			return first_child;
		}
		set
		{
			if (Component.USMA_mode)
			{
				second_child = value;
			}
			else
			{
				first_child = value;
			}
		}
	}

	public Component no_child
	{
		get
		{
			if (Component.USMA_mode)
			{
				return first_child;
			}
			return second_child;
		}
		set
		{
			if (Component.USMA_mode)
			{
				first_child = value;
			}
			else
			{
				second_child = value;
			}
		}
	}

	public Component left_Child
	{
		get
		{
			if (!Component.USMA_mode)
			{
				return first_child;
			}
			return first_child;
		}
		set
		{
			if (Component.USMA_mode)
			{
				first_child = value;
			}
			else
			{
				first_child = value;
			}
		}
	}

	public Component right_Child
	{
		get
		{
			if (!Component.USMA_mode)
			{
				return second_child;
			}
			return second_child;
		}
		set
		{
			if (Component.USMA_mode)
			{
				second_child = value;
			}
			else
			{
				second_child = value;
			}
		}
	}

	public IF_Control(int height, int width, string str_name)
		: base(height, width, str_name)
	{
		init();
	}

	public IF_Control(Component Successor, int height, int width, string str_name)
		: base(Successor, height, width, str_name)
	{
		init();
	}

	public IF_Control(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		yes_child = (Component)info.GetValue("_left_Child", typeof(Component));
		no_child = (Component)info.GetValue("_right_Child", typeof(Component));
		bottom = (int)info.GetValue("_bottom", typeof(int));
		min_bottom = (int)info.GetValue("_min_bottom", typeof(int));
		x_left = (int)info.GetValue("_x_left", typeof(int));
		y_left = (int)info.GetValue("_y_left", typeof(int));
		x_right = (int)info.GetValue("_x_right", typeof(int));
		y_right = (int)info.GetValue("_y_right", typeof(int));
		left_connector_y = (int)info.GetValue("_left_connector_y", typeof(int));
		right_connector_y = (int)info.GetValue("_right_connector_y", typeof(int));
		line_height = (int)info.GetValue("_line_height", typeof(int));
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

	public override void Clear_Breakpoints()
	{
		has_breakpoint = false;
		if (left_Child != null)
		{
			left_Child.Clear_Breakpoints();
		}
		if (right_Child != null)
		{
			right_Child.Clear_Breakpoints();
		}
		if (Successor != null)
		{
			Successor.Clear_Breakpoints();
		}
	}

	public override Component Find_Predecessor(Component c)
	{
		if (left_Child == c)
		{
			return this;
		}
		if (right_Child == c)
		{
			return this;
		}
		if (left_Child != null)
		{
			Component component = left_Child.Find_Predecessor(c);
			if (component != null)
			{
				return component;
			}
		}
		if (right_Child != null)
		{
			Component component2 = right_Child.Find_Predecessor(c);
			if (component2 != null)
			{
				return component2;
			}
		}
		return base.Find_Predecessor(c);
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("_left_Child", yes_child);
		info.AddValue("_right_Child", no_child);
		info.AddValue("_bottom", bottom);
		info.AddValue("_min_bottom", min_bottom);
		info.AddValue("_x_left", x_left);
		info.AddValue("_y_left", y_left);
		info.AddValue("_x_right", x_right);
		info.AddValue("_y_right", y_right);
		info.AddValue("_left_connector_y", left_connector_y);
		info.AddValue("_right_connector_y", right_connector_y);
		info.AddValue("_line_height", line_height);
		base.GetObjectData(info, ctxt);
	}

	public override void draw(Graphics gr, int x, int y)
	{
		bool flag = !((double)scale <= 0.4) && head_heightOrig >= 10 && Component.text_visible;
		base.X = x;
		base.Y = y;
		height_of_text = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_times).Height);
		width_of_text = Convert.ToInt32(gr.MeasureString(base.Text + "XX", PensBrushes.default_times).Width);
		int num = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_arial).Width);
		int num2 = Convert.ToInt32(gr.MeasureString("No", PensBrushes.default_arial).Width);
		line_height = base.H;
		left_connector_y = base.Y + base.H / 2 + line_height;
		right_connector_y = base.Y + base.H / 2 + line_height;
		min_bottom = base.Y + base.H / 2 + line_height;
		bottom = min_bottom;
		x_left = x - base.W;
		y_left = y + base.H / 2;
		y_right = y + base.H / 2;
		x_right = x + base.W;
		if (flag && Is_Wide_Diamond())
		{
			x_left -= drawing_text_width / 2 - base.W / 2;
			x_right += drawing_text_width / 2 - base.W / 2;
		}
		if (left_Child != null && !is_compressed)
		{
			int num3 = x - left_Child.FP.right - base.W / 2;
			if (num3 < x_left)
			{
				x_left = num3;
			}
		}
		if (right_Child != null && !is_compressed)
		{
			int num4 = x + right_Child.FP.left + base.W / 2;
			if (num4 > x_right)
			{
				x_right = num4;
			}
		}
		if (Component.USMA_mode)
		{
			x_right = x;
			y_right = y + base.H;
			left_connector_y -= base.CL;
			right_connector_y -= base.CL;
			if (left_Child != null && right_Child != null && !is_compressed)
			{
				int num5 = x - (right_Child.FP.left + left_Child.FP.right + base.W / 2);
				if (num5 < x_left)
				{
					x_left = num5;
				}
			}
			else if (right_Child != null && !is_compressed)
			{
				int num6 = x - (right_Child.FP.left + base.W / 2);
				if (num6 < x_left)
				{
					x_left = num6;
				}
			}
		}
		int num7;
		if (left_Child != null && !is_compressed)
		{
			num7 = min_bottom + left_Child.FP.height + base.CL;
			bottom = num7;
		}
		int num8;
		if (right_Child != null && !is_compressed)
		{
			num8 = min_bottom + right_Child.FP.height + base.CL;
			if (Component.USMA_mode)
			{
				num8 += base.CL;
			}
			if (num8 > bottom)
			{
				bottom = num8;
			}
		}
		gr.SmoothingMode = SmoothingMode.HighQuality;
		Pen diamond_pen = PensBrushes.blue_pen;
		Pen pen = PensBrushes.blue_pen;
		if (base.selected)
		{
			diamond_pen = PensBrushes.red_pen;
			pen = PensBrushes.red_pen;
		}
		else if (running)
		{
			diamond_pen = PensBrushes.chartreuse_pen;
			pen = PensBrushes.blue_pen;
		}
		else if (is_compressed && have_child_running())
		{
			diamond_pen = PensBrushes.chartreuse_pen;
			pen = PensBrushes.chartreuse_pen;
		}
		if (has_breakpoint)
		{
			StopSign.Draw(gr, x - base.W / 2 - base.W / 6 - 2, y, base.W / 6);
		}
		Draw_Diamond_and_Text(gr, x, y, base.Text, diamond_pen, flag);
		if (flag && Is_Wide_Diamond())
		{
			gr.DrawLine(pen, x - drawing_text_width / 2 - base.W / 4, y + base.H / 2, x_left, y_left);
			if (!Component.USMA_mode)
			{
				gr.DrawLine(pen, x + drawing_text_width / 2 + base.W / 4, y + base.H / 2, x_right, y_right);
			}
		}
		else
		{
			gr.DrawLine(pen, x - base.W / 2, y + base.H / 2, x_left, y_left);
			if (!Component.USMA_mode)
			{
				gr.DrawLine(pen, x + base.W / 2, y + base.H / 2, x_right, y_right);
			}
		}
		if (!Component.USMA_mode)
		{
			num7 = bottom;
			num8 = bottom;
		}
		else
		{
			num7 = bottom - base.W / 8;
			num8 = bottom - base.W / 4;
		}
		if (left_Child != null && !is_compressed)
		{
			gr.DrawLine(pen, x_left, y_left, x_left, left_connector_y);
			gr.DrawLine(pen, x_left, left_connector_y, x_left - base.CL / 4, left_connector_y - base.CL / 4);
			gr.DrawLine(pen, x_left, left_connector_y, x_left + base.CL / 4, left_connector_y - base.CL / 4);
			left_Child.draw(gr, x_left, left_connector_y);
			gr.DrawLine(pen, x_left, left_connector_y + left_Child.FP.height, x_left, num7);
		}
		else
		{
			gr.DrawLine(pen, x_left, y_left, x_left, num7);
		}
		gr.DrawLine(pen, x_left, num7, x_left - base.CL / 4, num7 - base.CL / 4);
		gr.DrawLine(pen, x_left, num7, x_left + base.CL / 4, num7 - base.CL / 4);
		if (right_Child != null && !is_compressed)
		{
			gr.DrawLine(pen, x_right, y_right, x_right, right_connector_y);
			gr.DrawLine(pen, x_right, right_connector_y, x_right - base.CL / 4, right_connector_y - base.CL / 4);
			gr.DrawLine(pen, x_right, right_connector_y, x_right + base.CL / 4, right_connector_y - base.CL / 4);
			right_Child.draw(gr, x_right, right_connector_y);
			gr.DrawLine(pen, x_right, right_connector_y + right_Child.FP.height, x_right, num8);
		}
		else
		{
			gr.DrawLine(pen, x_right, y_right, x_right, num8);
		}
		gr.DrawLine(pen, x_right, num8, x_right - base.CL / 4, num8 - base.CL / 4);
		gr.DrawLine(pen, x_right, num8, x_right + base.CL / 4, num8 - base.CL / 4);
		if (Component.USMA_mode)
		{
			gr.DrawEllipse(pen, x_right - base.W / 8, num8, base.W / 4, base.W / 4);
			gr.DrawLine(pen, x_left, num7, x_right - base.W / 8, num7);
		}
		else
		{
			gr.DrawLine(pen, x_left, bottom, x_right, bottom);
		}
		if (flag)
		{
			if (!Component.USMA_mode)
			{
				if (Is_Wide_Diamond())
				{
					gr.DrawString("No", PensBrushes.default_arial, PensBrushes.blackbrush, x + drawing_text_width / 2 + base.W / 4 + num2 / 2, y + base.H / 2 - 5, PensBrushes.centered_stringFormat);
					gr.DrawString("Yes", PensBrushes.default_arial, PensBrushes.blackbrush, x - drawing_text_width / 2 - base.W / 4 - num / 2, y + base.H / 2 - 5, PensBrushes.centered_stringFormat);
				}
				else
				{
					gr.DrawString("No", PensBrushes.default_arial, PensBrushes.blackbrush, x + base.W / 2 + num2, y + base.H / 2 - 5, PensBrushes.centered_stringFormat);
					gr.DrawString("Yes", PensBrushes.default_arial, PensBrushes.blackbrush, x - base.W / 2 - num, y + base.H / 2 - 5, PensBrushes.centered_stringFormat);
				}
			}
			else if (Is_Wide_Diamond())
			{
				gr.DrawString("No", PensBrushes.default_arial, PensBrushes.blackbrush, x - (drawing_text_width / 2 + base.W / 4 + num2 / 2), y + base.H / 2 - 5, PensBrushes.centered_stringFormat);
				gr.DrawString("Yes", PensBrushes.default_arial, PensBrushes.blackbrush, x - base.W / 8 - num / 2, y + base.H + 5, PensBrushes.centered_stringFormat);
			}
			else
			{
				gr.DrawString("No", PensBrushes.default_arial, PensBrushes.blackbrush, x - (base.W / 2 + num2), y + base.H / 2 - 5, PensBrushes.centered_stringFormat);
				gr.DrawString("Yes", PensBrushes.default_arial, PensBrushes.blackbrush, x - base.W / 8 - num, y + base.H + 5, PensBrushes.centered_stringFormat);
			}
		}
		if (Successor != null)
		{
			gr.DrawLine(pen, x, bottom, x, bottom + base.CL);
			gr.DrawLine(pen, x, bottom + base.CL, x - base.CL / 4, bottom + base.CL - base.CL / 4);
			gr.DrawLine(pen, x, bottom + base.CL, x + base.CL / 4, bottom + base.CL - base.CL / 4);
			Successor.draw(gr, x, bottom + base.CL);
		}
		if (flag)
		{
			base.draw(gr, x, y);
		}
	}

	public override bool overplus(int x, int y)
	{
		bool num = Math.Abs(y - (base.Y + base.H / 8)) <= base.H / 8;
		bool flag = ((!Is_Wide_Diamond()) ? (Math.Abs(x - (base.X - base.W / 2 + base.H / 8)) <= base.H / 8) : (Math.Abs(x - (base.X - base.W / 4 - drawing_text_width / 2 + base.H / 8)) <= base.H / 8));
		return num && flag;
	}

	public override bool contains(int x, int y)
	{
		bool flag = Math.Abs(y - (base.Y + base.H / 2)) <= base.H / 2;
		return Diamond_Bounded_X(x) && flag;
	}

	public override bool contains(System.Drawing.Rectangle rec)
	{
		System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(base.X - Diamond_Width() / 2 + 2, base.Y, Diamond_Width() - 4, base.H);
		return rec.IntersectsWith(rectangle);
	}

	public override bool overline(int x, int y, int connector_y)
	{
		if (Successor != null)
		{
			bool num = Math.Abs(x - base.X) < proximity;
			bool flag = y < bottom + base.CL && y > bottom;
			return num && flag;
		}
		bool num2 = Math.Abs(x - base.X) < proximity;
		bool flag2 = y < connector_y && y > bottom;
		return num2 && flag2;
	}

	public bool overleft(int x, int y, int connector_y)
	{
		bool flag = Math.Abs(x - x_left) < proximity;
		if (left_Child != null)
		{
			bool flag2 = y < min_bottom && y > base.Y + base.H / 2;
			return flag && flag2;
		}
		bool flag3 = y < bottom && y > base.Y + base.H / 2;
		return flag && flag3;
	}

	public bool overright(int x, int y, int connector_y)
	{
		bool flag = Math.Abs(x - x_right) < proximity;
		if (right_Child != null)
		{
			bool flag2 = y < min_bottom && y > base.Y + base.H / 2;
			return flag && flag2;
		}
		bool flag3 = y < bottom && y > base.Y + base.H / 2;
		return flag && flag3;
	}

	public override void Scale(float new_scale)
	{
		base.H = (int)Math.Round(scale * (float)head_heightOrig);
		base.W = (int)Math.Round(scale * (float)head_widthOrig);
		if (left_Child != null)
		{
			left_Child.scale = scale;
			left_Child.Scale(new_scale);
		}
		if (right_Child != null)
		{
			right_Child.scale = scale;
			right_Child.Scale(new_scale);
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
		bool flag = Component.full_text && (double)scale > 0.4;
		init();
		Diamond_Footprint(gr, (double)scale > 0.4, base.W / 2 + base.W / 4);
		if (!Component.USMA_mode)
		{
			if (left_Child != null && !is_compressed)
			{
				left_Child.footprint(gr);
				int num = left_Child.FP.left + left_Child.FP.right + base.W / 2;
				if (num > FP.left)
				{
					FP.left = num;
				}
				if (flag && Is_Wide_Diamond())
				{
					num = left_Child.FP.left + drawing_text_width / 2 + base.W;
					if (num > FP.left)
					{
						FP.left = num;
					}
				}
			}
			if (right_Child != null && !is_compressed)
			{
				right_Child.footprint(gr);
				int num2 = right_Child.FP.left + right_Child.FP.right + base.W / 2;
				if (num2 > FP.right)
				{
					FP.right = num2;
				}
				if (flag && Is_Wide_Diamond())
				{
					num2 = right_Child.FP.right + drawing_text_width / 2 + base.W;
					if (num2 > FP.right)
					{
						FP.right = num2;
					}
				}
			}
		}
		else
		{
			if (right_Child != null && !is_compressed)
			{
				right_Child.footprint(gr);
				if (right_Child.FP.right > FP.right)
				{
					FP.right = right_Child.FP.right;
				}
			}
			if (left_Child != null && right_Child != null && !is_compressed)
			{
				left_Child.footprint(gr);
				int num3 = right_Child.FP.left + left_Child.FP.left + left_Child.FP.right + base.W / 2 + base.W / 8;
				if (num3 > FP.left)
				{
					FP.left = num3;
				}
				if (flag && Is_Wide_Diamond())
				{
					num3 = left_Child.FP.left + drawing_text_width / 2 + base.W;
					if (num3 > FP.left)
					{
						FP.left = num3;
					}
				}
			}
			else if ((right_Child != null) & !is_compressed)
			{
				int num4 = right_Child.FP.left + base.W / 2 + base.W / 8;
				if (num4 > FP.left)
				{
					FP.left = num4;
				}
			}
			else if (left_Child != null && !is_compressed)
			{
				left_Child.footprint(gr);
				int num5 = left_Child.FP.left + left_Child.FP.right + base.W / 2;
				if (num5 > FP.left)
				{
					FP.left = num5;
				}
				if (flag && Is_Wide_Diamond())
				{
					num5 = left_Child.FP.left + drawing_text_width / 2 + base.W;
					if (num5 > FP.left)
					{
						FP.left = num5;
					}
				}
			}
		}
		int num6 = 0;
		if (right_Child != null)
		{
			num6 = (Component.USMA_mode ? (right_Child.FP.height + base.CL) : right_Child.FP.height);
		}
		if (right_Child != null && left_Child != null && !is_compressed)
		{
			FP.height = base.H + base.H / 2 + ((num6 > left_Child.FP.height) ? num6 : left_Child.FP.height) + base.CL;
		}
		else if (right_Child != null && !is_compressed)
		{
			FP.height = base.H + base.H / 2 + num6 + base.CL;
		}
		else if (left_Child != null && !is_compressed)
		{
			FP.height = base.H + base.H / 2 + left_Child.FP.height + base.CL;
		}
		else
		{
			FP.height = base.H + base.H / 2;
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
		FP.height = base.H + base.H / 2;
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
		if (overleft(x, y, connector_y))
		{
			if (!is_compressed)
			{
				if (newObj != null)
				{
					newObj.set_parent_info(is_child: true, is_before_child: false, is_after_child: false, this);
					if (left_Child != null)
					{
						newObj.find_end().Successor = left_Child;
					}
					left_Child = newObj;
				}
				return true;
			}
			if (newObj != null)
			{
				MessageBox.Show("Can't insert in collapsed symbol");
			}
			return false;
		}
		if (overright(x, y, connector_y))
		{
			if (!is_compressed)
			{
				if (newObj != null)
				{
					newObj.set_parent_info(is_child: true, is_before_child: false, is_after_child: false, this);
					if (right_Child != null)
					{
						newObj.find_end().Successor = right_Child;
					}
					right_Child = newObj;
				}
				return true;
			}
			if (newObj != null)
			{
				MessageBox.Show("Can't insert in collapsed symbol");
			}
			return false;
		}
		if (left_Child != null)
		{
			flag = left_Child.insert(newObj, x, y, bottom);
		}
		if (!flag && right_Child != null)
		{
			flag = right_Child.insert(newObj, x, y, bottom);
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
		if (left_Child != null)
		{
			if (left_Child.selected)
			{
				Component component = left_Child.find_selection_end();
				if (component.Successor != null)
				{
					left_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					left_Child = null;
				}
				return true;
			}
			flag2 = left_Child.delete();
		}
		if (right_Child != null)
		{
			if (right_Child.selected)
			{
				Component component = right_Child.find_selection_end();
				if (component.Successor != null)
				{
					right_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					right_Child = null;
				}
				return true;
			}
			flag3 = right_Child.delete();
		}
		return flag || flag2 || flag3;
	}

	public override string getText(int x, int y)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		if (contains(x, y))
		{
			return base.Text;
		}
		if (Successor != null)
		{
			text = Successor.getText(x, y);
		}
		if (left_Child != null && !is_compressed)
		{
			text2 = left_Child.getText(x, y);
		}
		if (right_Child != null && !is_compressed)
		{
			text3 = right_Child.getText(x, y);
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
			new Control_Dlg(this, form, is_loop: false).ShowDialog();
			return true;
		}
		if (Successor != null)
		{
			flag = Successor.setText(x, y, form);
		}
		if (left_Child != null && !is_compressed)
		{
			flag2 = left_Child.setText(x, y, form);
		}
		if (right_Child != null && !is_compressed)
		{
			flag3 = right_Child.setText(x, y, form);
		}
		return flag || flag2 || flag3;
	}

	public override Component Clone()
	{
		IF_Control iF_Control = (IF_Control)base.Clone();
		if (left_Child != null)
		{
			iF_Control.left_Child = left_Child.Clone();
			iF_Control.left_Child.set_parent_info(is_child: true, is_before_child: false, is_after_child: false, iF_Control);
		}
		if (right_Child != null)
		{
			iF_Control.right_Child = right_Child.Clone();
			iF_Control.right_Child.set_parent_info(is_child: true, is_before_child: false, is_after_child: false, iF_Control);
		}
		return iF_Control;
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
		if (left_Child != null)
		{
			if (left_Child.selected)
			{
				Component successor = left_Child;
				Component component = left_Child.find_selection_end();
				if (component.Successor != null)
				{
					left_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					left_Child = null;
				}
				VF.clipboard = successor;
				VF.clipboard.reset();
				return true;
			}
			flag = left_Child.cut(VF);
		}
		if (right_Child != null)
		{
			if (right_Child.selected)
			{
				Component successor = right_Child;
				Component component = right_Child.find_selection_end();
				if (component.Successor != null)
				{
					right_Child = component.Successor;
					component.Successor = null;
				}
				else
				{
					right_Child = null;
				}
				VF.clipboard = successor;
				VF.clipboard.reset();
				return true;
			}
			flag2 = right_Child.cut(VF);
		}
		return flag || flag2 || flag3;
	}

	public override bool copy(Visual_Flow_Form VF)
	{
		if (base.selected)
		{
			return base.copy(VF);
		}
		if (left_Child != null && left_Child.copy(VF))
		{
			return true;
		}
		if (right_Child != null && right_Child.copy(VF))
		{
			return true;
		}
		if (Successor != null)
		{
			return Successor.copy(VF);
		}
		return false;
	}

	public override void Emit_Code(typ gen)
	{
		object o = gen.If_Start();
		if (parse_tree != null)
		{
			interpreter_pkg.emit_code(parse_tree, base.Text, gen);
		}
		gen.If_Then_Part(o);
		if (yes_child != null)
		{
			yes_child.Emit_Code(gen);
		}
		gen.If_Else_Part(o);
		if (no_child != null)
		{
			no_child.Emit_Code(gen);
		}
		gen.If_Done(o);
		if (Successor != null)
		{
			Successor.Emit_Code(gen);
		}
	}
}
