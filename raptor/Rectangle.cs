using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using generate_interface;
using parse_tree;

namespace raptor;

[Serializable]
public class Rectangle : Component
{
	public enum Kind_Of
	{
		Assignment,
		Call
	}

	public Kind_Of kind;

	public Rectangle(int height, int width, string str_name, Kind_Of the_kind)
		: base(height, width, str_name)
	{
		init();
		kind = the_kind;
	}

	public Rectangle(Component Successor, int height, int width, string str_name, Kind_Of the_kind)
		: base(Successor, height, width, str_name)
	{
		init();
		kind = the_kind;
	}

	public Rectangle(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		if (incoming_serialization_version >= 7)
		{
			kind = (Kind_Of)info.GetValue("_kind", typeof(Kind_Of));
		}
		else
		{
			int num = base.Text.IndexOf("=");
			if (num > 0)
			{
				kind = Kind_Of.Assignment;
				if (base.Text.IndexOf(":=") <= 0)
				{
					base.Text = base.Text.Substring(0, num) + ":=" + base.Text.Substring(num + 1, base.Text.Length - (num + 1));
				}
			}
			else
			{
				kind = Kind_Of.Call;
			}
		}
		result = interpreter_pkg.statement_syntax(base.Text, kind == Kind_Of.Call, this);
		if (result.valid)
		{
			parse_tree = result.tree;
			return;
		}
		if (!Component.warned_about_error && base.Text != "")
		{
			MessageBox.Show("Error: \n" + base.Text + "\nis not recognized.  Perhaps a DLL is missing?\nClose RAPTOR, save the DLL and then reopen");
			Component.warned_about_error = true;
		}
		base.Text = "";
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("_kind", kind);
	}

	public override bool contains(int x, int y)
	{
		if (kind == Kind_Of.Assignment)
		{
			return base.contains(x, y);
		}
		if (base.contains(x, y))
		{
			return true;
		}
		return ((drawing_text_width <= base.W) ? new System.Drawing.Rectangle(base.X + base.W / 2, base.Y + base.H / 4, base.W / 4, base.H / 2) : new System.Drawing.Rectangle(base.X + drawing_text_width / 2, base.Y + base.H / 4, base.W / 4, base.H / 2)).Contains(x, y);
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
		base.Scale(new_scale);
	}

	private string getText(bool unicode)
	{
		int num = base.Text.IndexOf(":=");
		if (num > 0)
		{
			if (!Component.USMA_mode)
			{
				if (unicode)
				{
					return base.Text.Substring(0, num) + " " + Component.assignmentSymbol + " " + base.Text.Substring(num + 2, base.Text.Length - (num + 2));
				}
				return "Set " + base.Text.Substring(0, num) + " to " + base.Text.Substring(num + 2, base.Text.Length - (num + 2));
			}
			return base.Text.Substring(0, num) + " = " + base.Text.Substring(num + 2, base.Text.Length - (num + 2));
		}
		return base.Text;
	}

	public override string getText(int x, int y)
	{
		if (contains(x, y))
		{
			return getText(unicode: false);
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
			if (kind == Kind_Of.Assignment)
			{
				new Assignment_Dlg(this, form).ShowDialog();
			}
			else
			{
				new Call_Dialog(this, form).ShowDialog();
			}
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
		bool flag2 = true;
		if (Successor != null)
		{
			flag2 = Successor.has_code();
		}
		if (parse_tree == null)
		{
			flag = false;
		}
		return flag && flag2;
	}

	public override void mark_error()
	{
		if (Successor != null)
		{
			Successor.mark_error();
		}
		if (parse_tree == null)
		{
			base.Text = "Error";
			Runtime.parent.Show_Text_On_Error();
		}
	}

	public override string getDrawText()
	{
		return Component.unbreakString(getText(unicode: true));
	}

	public override bool Called_Tab(string s)
	{
		try
		{
			if (parse_tree is procedure_call && ((procedure_call)parse_tree).is_tab_call() && s.ToLower() == interpreter_pkg.get_name_call(parse_tree as procedure_call, base.Text).ToLower())
			{
				return true;
			}
			return base.Called_Tab(s);
		}
		catch
		{
			return base.Called_Tab(s);
		}
	}

	public override void Rename_Tab(string from, string to)
	{
		try
		{
			if (parse_tree is procedure_call)
			{
				string text = interpreter_pkg.get_name_call(parse_tree as procedure_call, base.Text);
				if (((procedure_call)parse_tree).is_tab_call() && from.ToLower() == text.ToLower())
				{
					base.Text = base.Text.Replace(text, to);
					parse_tree = interpreter_pkg.call_syntax(base.Text, this).tree;
				}
			}
		}
		catch
		{
		}
		base.Rename_Tab(from, to);
	}

	private string Recursion_Involves()
	{
		string text = "";
		for (int i = 0; i < Runtime.parent.carlisle.TabPages.Count; i++)
		{
			if (((Subchart)Runtime.parent.carlisle.TabPages[i]).am_compiling)
			{
				text = text + Runtime.parent.carlisle.TabPages[i].Text + " ";
			}
		}
		return text;
	}

	private Subchart Find_Start(string s)
	{
		TabControl.TabPageCollection tpc = Compile_Helpers.get_tpc();
		for (int i = 0; i < tpc.Count; i++)
		{
			if (tpc[i].Text.ToLower() == s.ToLower())
			{
				return (Subchart)tpc[i];
			}
		}
		return null;
	}

	public override void Emit_Code(typ gen)
	{
		if (kind == Kind_Of.Call && ((procedure_call)parse_tree).is_tab_call())
		{
			string s = interpreter_pkg.get_name_call(parse_tree as procedure_call, base.Text);
			Subchart subchart = Find_Start(s);
			if (!(subchart is Procedure_Chart))
			{
				subchart.Start.Emit_Code(gen);
			}
			else
			{
				Procedure_Chart procedure_Chart = subchart as Procedure_Chart;
				parameter_list parameter_list = ((procedure_call)parse_tree).param_list;
				object o = gen.Emit_Call_Subchart(procedure_Chart.Text);
				for (int i = 0; i < procedure_Chart.num_params; i++)
				{
					parse_tree_pkg.emit_parameter_number(parameter_list.parameter, gen, 0);
					parameter_list = parameter_list.next;
					if (parameter_list != null)
					{
						gen.Emit_Next_Parameter(o);
					}
				}
				gen.Emit_Last_Parameter(o);
			}
		}
		else if (parse_tree != null)
		{
			interpreter_pkg.emit_code(parse_tree, base.Text, gen);
		}
		if (Successor != null)
		{
			Successor.Emit_Code(gen);
		}
	}

	public override void compile_pass1(typ gen)
	{
		if (kind == Kind_Of.Call && ((procedure_call)parse_tree).is_tab_call())
		{
			string s = interpreter_pkg.get_name_call(parse_tree as procedure_call, base.Text);
			Subchart subchart = Find_Start(s);
			if (!(subchart is Procedure_Chart))
			{
				if (subchart.am_compiling)
				{
					throw new Exception("The RAPTOR compiler does not support recursive programs.\nRecursion found at call to: " + base.Text + "\nCycle of calls includes: " + Recursion_Involves());
				}
				subchart.am_compiling = true;
				subchart.Start.compile_pass1(gen);
				subchart.am_compiling = false;
			}
			else
			{
				Procedure_Chart procedure_Chart = subchart as Procedure_Chart;
				parameter_list parameter_list = ((procedure_call)parse_tree).param_list;
				for (int i = 0; i < procedure_Chart.num_params; i++)
				{
					parameter_list = parameter_list.next;
				}
			}
		}
		else if (parse_tree != null)
		{
			interpreter_pkg.compile_pass1(parse_tree, base.Text, gen);
		}
		if (Successor != null)
		{
			Successor.compile_pass1(gen);
		}
	}

	public override void wide_footprint(Graphics gr)
	{
		int num = 2 * base.W;
		int num2 = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_times).Height);
		SizeF sizeF;
		while (true)
		{
			sizeF = gr.MeasureString(getDrawText() + "XX", PensBrushes.default_times, num);
			if (sizeF.Height < (float)(num2 * 7 / 2))
			{
				break;
			}
			num += base.W / 2;
		}
		if (sizeF.Height > (float)(num2 * 3 / 2))
		{
			FP.left = (num - base.W) / 2 + 3 * base.W / 8;
			FP.right = (num - base.W) / 2 + 3 * base.W / 8;
			drawing_text_width = num;
		}
		else if ((int)sizeF.Width > base.W)
		{
			for (num = base.W; num < (int)sizeF.Width; num += base.W / 2)
			{
			}
			FP.left = (num - base.W) / 2 + 3 * base.W / 8;
			FP.right = (num - base.W) / 2 + 3 * base.W / 8;
			drawing_text_width = num;
		}
		else
		{
			drawing_text_width = 0;
		}
	}

	public override bool In_Footprint(int x, int y)
	{
		if (kind == Kind_Of.Assignment)
		{
			return base.In_Footprint(x, y);
		}
		if (base.In_Footprint(x, y))
		{
			return true;
		}
		return ((drawing_text_width <= base.W) ? new System.Drawing.Rectangle(base.X + base.W / 2, base.Y + base.H / 4, base.W / 4, base.H / 2) : new System.Drawing.Rectangle(base.X + drawing_text_width / 2, base.Y + base.H / 4, base.W / 4, base.H / 2)).Contains(x, y);
	}

	public override void draw(Graphics gr, int x, int y)
	{
		bool flag = !((double)scale <= 0.4) && head_heightOrig >= 10 && Component.text_visible;
		base.X = x;
		base.Y = y;
		height_of_text = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_times).Height);
		width_of_text = Convert.ToInt32(gr.MeasureString(base.Text + "XX", PensBrushes.default_times).Width);
		gr.SmoothingMode = SmoothingMode.HighQuality;
		Pen pen = (base.selected ? PensBrushes.red_pen : ((!running) ? PensBrushes.blue_pen : PensBrushes.chartreuse_pen));
		int num = ((drawing_text_width <= base.W) ? base.W : drawing_text_width);
		if (has_breakpoint)
		{
			StopSign.Draw(gr, x - num / 2 - base.W / 6 - 2, y, base.W / 6);
		}
		if (kind != Kind_Of.Call || !Component.USMA_mode)
		{
			gr.DrawRectangle(pen, x - num / 2, y, num, base.H);
		}
		else
		{
			gr.DrawLine(pen, x - num / 2, y, x + num / 2, y);
			gr.DrawLine(pen, x - num / 2, y, x - num / 2, y + 3 * base.H / 4);
			gr.DrawLine(pen, x + num / 2, y, x + num / 2, y + 3 * base.H / 4);
			gr.DrawLine(pen, x - num / 2, y + 3 * base.H / 4, x, y + base.H);
			gr.DrawLine(pen, x, y + base.H, x + num / 2, y + 3 * base.H / 4);
		}
		if (kind == Kind_Of.Call && !Component.USMA_mode)
		{
			gr.DrawLine(pen, x + num / 2, y + 5 * base.H / 12, x + num / 2 + base.W / 8, y + 5 * base.H / 12);
			gr.DrawLine(pen, x + num / 2 + base.W / 8, y + 5 * base.H / 12, x + num / 2 + base.W / 8, y + base.H / 4);
			gr.DrawLine(pen, x + num / 2 + base.W / 8, y + base.H / 4, x + num / 2 + 2 * base.W / 8, y + base.H / 2);
			gr.DrawLine(pen, x + num / 2, y + 7 * base.H / 12, x + num / 2 + base.W / 8, y + 7 * base.H / 12);
			gr.DrawLine(pen, x + num / 2 + base.W / 8, y + 7 * base.H / 12, x + num / 2 + base.W / 8, y + 3 * base.H / 4);
			gr.DrawLine(pen, x + num / 2 + base.W / 8, y + 3 * base.H / 4, x + num / 2 + 2 * base.W / 8, y + base.H / 2);
		}
		if (flag && base.Text.Length > 0)
		{
			if (Component.full_text)
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
			if (base.Text == "Error")
			{
				gr.DrawString(base.Text, PensBrushes.default_times, PensBrushes.redbrush, rect, PensBrushes.centered_stringFormat);
			}
			else
			{
				gr.DrawString(getDrawText(), PensBrushes.default_times, PensBrushes.blackbrush, rect, PensBrushes.centered_stringFormat);
			}
		}
		if (Successor != null)
		{
			Pen pen2 = ((!base.selected) ? PensBrushes.blue_pen : PensBrushes.red_pen);
			gr.DrawLine(pen2, x, y + base.H, x, y + base.H + base.CL);
			gr.DrawLine(pen2, x, y + base.H + base.CL, x - base.CL / 4, y + base.H + base.CL - base.CL / 4);
			gr.DrawLine(pen2, x, y + base.H + base.CL, x + base.CL / 4, y + base.H + base.CL - base.CL / 4);
			Successor.scale = scale;
			Successor.draw(gr, x, y + base.H + base.CL);
		}
		if (flag)
		{
			base.draw(gr, x, y);
		}
	}

	public override void collect_variable_names(IList<string> l, IDictionary<string, string> types)
	{
		if (kind == Kind_Of.Assignment && parse_tree != null)
		{
			string text = interpreter_pkg.get_name((assignment)parse_tree, base.Text);
			l.Add(text);
			if (parse_tree is expr_assignment)
			{
				string class_decl = (parse_tree as expr_assignment).expr_part.get_class_decl();
				if (class_decl != null && !types.ContainsKey(text.ToLower()))
				{
					types.Add(text.ToLower(), class_decl);
				}
			}
		}
		if (Successor != null)
		{
			Successor.collect_variable_names(l, types);
		}
	}
}
