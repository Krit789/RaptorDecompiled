using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using generate_interface;
using interpreter;
using parse_tree;

namespace raptor;

[Serializable]
public class Parallelogram : Component
{
	public string prompt = "";

	public bool is_input;

	public bool new_line = true;

	public bool input_is_expression;

	public parseable prompt_tree;

	private syntax_result prompt_result;

	public Parallelogram(int height, int width, string str_name, bool input)
		: base(height, width, str_name)
	{
		init();
		is_input = input;
	}

	public Parallelogram(Component Successor, int height, int width, string str_name, bool input)
		: base(Successor, height, width, str_name)
	{
		is_input = input;
		init();
	}

	public Parallelogram(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		prompt = (string)info.GetValue("_prompt", typeof(string));
		is_input = (bool)info.GetValue("_is_input", typeof(bool));
		if (incoming_serialization_version >= 9 && is_input)
		{
			input_is_expression = info.GetBoolean("_input_expression");
		}
		else
		{
			input_is_expression = false;
		}
		if (incoming_serialization_version >= 5)
		{
			new_line = info.GetBoolean("_new_line");
		}
		else
		{
			new_line = true;
		}
		if (is_input)
		{
			result = interpreter_pkg.input_syntax(base.Text, this);
			if (input_is_expression)
			{
				prompt_result = interpreter_pkg.output_syntax(prompt, new_line: false, this);
			}
		}
		else
		{
			result = interpreter_pkg.output_syntax(base.Text, new_line, this);
		}
		if (input_is_expression)
		{
			if (prompt_result.valid)
			{
				prompt_tree = prompt_result.tree;
			}
			else
			{
				prompt = "";
			}
		}
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
		info.AddValue("_prompt", prompt);
		info.AddValue("_is_input", is_input);
		info.AddValue("_new_line", new_line);
		info.AddValue("_input_expression", input_is_expression);
		base.GetObjectData(info, ctxt);
	}

	public override void draw(Graphics gr, int x, int y)
	{
		bool flag = !((double)scale <= 0.4) && head_heightOrig >= 10 && Component.text_visible;
		if (flag)
		{
			base.draw(gr, x, y);
		}
		int num = base.W / 8;
		base.X = x;
		base.Y = y;
		height_of_text = Convert.ToInt32(gr.MeasureString("Yes", PensBrushes.default_times).Height);
		width_of_text = Convert.ToInt32(gr.MeasureString(getDrawText() + " X", PensBrushes.default_times).Width);
		gr.SmoothingMode = SmoothingMode.HighQuality;
		Pen pen = (base.selected ? PensBrushes.red_pen : ((!running) ? PensBrushes.blue_pen : PensBrushes.chartreuse_pen));
		int num2 = ((drawing_text_width <= base.W) ? base.W : (drawing_text_width + 3 * num / 2));
		gr.DrawLine(pen, x - num2 / 2 + num, y, x + num2 / 2, y);
		gr.DrawLine(pen, x + num2 / 2, y, x + num2 / 2 - num, y + base.H);
		gr.DrawLine(pen, x - num2 / 2, y + base.H, x + num2 / 2 - num, y + base.H);
		gr.DrawLine(pen, x - num2 / 2, y + base.H, x - num2 / 2 + num, y);
		if (has_breakpoint)
		{
			StopSign.Draw(gr, x - num2 / 2 - base.W / 6 - 2, y, base.W / 6);
		}
		if (is_input)
		{
			gr.DrawLine(pen, x - num2 / 2 + 3 * base.W / 32 - base.W / 4, y + base.H / 4, x - num2 / 2 + 3 * base.W / 32, y + base.H / 4);
			gr.DrawLine(pen, x - num2 / 2 + 3 * base.W / 32, y + base.H / 4, x - num2 / 2 + 3 * base.W / 32 - base.W / 8, y + base.H / 4 - base.H / 8);
			gr.DrawLine(pen, x - num2 / 2 + 3 * base.W / 32, y + base.H / 4, x - num2 / 2 + 3 * base.W / 32 - base.W / 8, y + base.H / 4 + base.H / 8);
		}
		else
		{
			gr.DrawLine(pen, x + num2 / 2 - 3 * base.W / 32, y + base.H - base.H / 4, x + num2 / 2 - 3 * base.W / 32 + base.W / 4, y + base.H - base.H / 4);
			gr.DrawLine(pen, x + num2 / 2 - 3 * base.W / 32 + base.W / 4, y + base.H - base.H / 4, x + num2 / 2 - 3 * base.W / 32 + base.W / 4 - base.W / 8, y + base.H - base.H / 4 - base.H / 8);
			gr.DrawLine(pen, x + num2 / 2 - 3 * base.W / 32 + base.W / 4, y + base.H - base.H / 4, x + num2 / 2 - 3 * base.W / 32 + base.W / 4 - base.W / 8, y + base.H - base.H / 4 + base.H / 8);
		}
		if (flag)
		{
			if (Component.full_text)
			{
				if (drawing_text_width > 0)
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
				rect = new System.Drawing.Rectangle(x - base.W * 7 / 16, base.Y + base.H * 6 / 16, base.W - base.W / 8, height_of_text);
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
			pen = ((!base.selected) ? PensBrushes.blue_pen : PensBrushes.red_pen);
			gr.DrawLine(pen, x, y + base.H, x, y + base.H + base.CL);
			gr.DrawLine(pen, x, y + base.H + base.CL, x - base.CL / 4, y + base.H + base.CL - base.CL / 4);
			gr.DrawLine(pen, x, y + base.H + base.CL, x + base.CL / 4, y + base.H + base.CL - base.CL / 4);
			Successor.scale = scale;
			Successor.draw(gr, x, y + base.H + base.CL);
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
			FP.left = (num - base.W) / 2 + base.W / 2;
			FP.right = (num - base.W) / 2 + base.W / 2;
			drawing_text_width = num;
		}
		else if ((int)sizeF.Width > base.W)
		{
			for (num = base.W; num < (int)sizeF.Width; num += base.W / 2)
			{
			}
			FP.left = (num - base.W) / 2 + base.W / 2;
			FP.right = (num - base.W) / 2 + base.W / 2;
			drawing_text_width = num;
		}
		else
		{
			drawing_text_width = 0;
		}
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
			if (is_input)
			{
				new Input_Dlg(this, form).ShowDialog();
			}
			else
			{
				new Output_Dlg(this, form).ShowDialog();
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
		string text;
		string text2;
		if (Component.USMA_mode)
		{
			text = "INPUT " + base.Text;
			text2 = "OUTPUT " + base.Text;
		}
		else
		{
			text = "GET " + base.Text;
			text2 = "PUT " + base.Text;
		}
		string s = ((is_input && base.Text != "") ? ((!Component.full_text) ? text : (input_is_expression ? (prompt + "\n" + text) : ("\"" + prompt + "\"\n" + text))) : ((is_input || !(base.Text != "")) ? "" : (new_line ? (text2 + "¶") : text2)));
		if (is_input || (base.Text != "" && base.Text[0] == '"'))
		{
			return s;
		}
		return Component.unbreakString(s);
	}

	public override void compile_pass1(typ gen)
	{
		if (((parse_tree != null) & is_input) && input_is_expression && !Compile_Helpers.Start_New_Declaration("raptor_prompt_variable_zzyz"))
		{
			gen.Declare_String_Variable("raptor_prompt_variable_zzyz");
		}
		base.compile_pass1(gen);
	}

	public override void Emit_Code(typ gen)
	{
		if (parse_tree != null)
		{
			if (is_input)
			{
				if (!input_is_expression)
				{
					parse_tree_pkg.set_prompt(prompt);
				}
				else
				{
					parse_tree_pkg.set_prompt(null);
					gen.Variable_Assignment_Start("raptor_prompt_variable_zzyz");
					interpreter_pkg.emit_code(((expr_output)prompt_tree).expr, prompt, gen);
					gen.Variable_Assignment_PastRHS();
				}
			}
			interpreter_pkg.emit_code(parse_tree, base.Text, gen);
		}
		if (Successor != null)
		{
			Successor.Emit_Code(gen);
		}
	}

	public override void collect_variable_names(IList<string> l, IDictionary<string, string> types)
	{
		if (is_input && parse_tree != null)
		{
			string item = interpreter_pkg.get_name_input((input)parse_tree, base.Text);
			l.Add(item);
		}
		if (Successor != null)
		{
			Successor.collect_variable_names(l, types);
		}
	}
}
