using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using generate_interface;
using generate_interface_oo;
using parse_tree;

namespace raptor;

[Serializable]
public class Oval_Return : Oval
{
	public Oval_Return(int height, int width, string str_name)
		: base(height, width, str_name)
	{
		init();
	}

	public Oval_Return(Component Successor, int height, int width, string str_name)
		: base(Successor, height, width, str_name)
	{
		init();
	}

	public Oval_Return(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		result = interpreter_pkg.output_syntax(base.Text, new_line: false, this);
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
		base.GetObjectData(info, ctxt);
	}

	public override void collect_variable_names(IList<string> l, IDictionary<string, string> types)
	{
		if (Successor != null)
		{
			Successor.collect_variable_names(l, types);
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
			if (sizeF.Height < (float)(num2 * 5 / 2))
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

	public override void draw(Graphics gr, int x, int y)
	{
		base.draw(gr, x, y);
	}

	public override bool SelectRegion(System.Drawing.Rectangle rec)
	{
		bool flag = false;
		if (Successor != null)
		{
			flag = Successor.SelectRegion(rec);
		}
		if (contains(rec))
		{
			base.selected = true;
			return true;
		}
		base.selected = false;
		return flag;
	}

	public override bool editable_selected()
	{
		return base.selected;
	}

	public override void selectAll()
	{
		base.selected = true;
		if (Successor != null)
		{
			Successor.selectAll();
		}
	}

	public override bool setText(int x, int y, Visual_Flow_Form form)
	{
		if (contains(x, y))
		{
			new Return_Dlg(this, form).ShowDialog();
			return true;
		}
		if (Successor != null)
		{
			return Successor.setText(x, y, form);
		}
		return false;
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
		if (base.Text != null && base.Text != "")
		{
			return "RETURN " + base.Text;
		}
		return "";
	}

	protected override void drawConnector(Graphics gr, Pen pen)
	{
		gr.DrawLine(pen, base.X, base.Y + base.H, base.X, base.Y + base.H + base.CL / 2);
		gr.DrawLine(pen, base.X, base.Y + base.H + base.CL / 2, base.X + base.W / 2, base.Y + base.H + base.CL / 2);
		gr.DrawLine(pen, base.X + base.W / 2, base.Y + base.H + base.CL / 2, base.X + base.W / 2 - base.CL / 4, base.Y + base.H + base.CL / 2 - base.CL / 4);
		gr.DrawLine(pen, base.X + base.W / 2, base.Y + base.H + base.CL / 2, base.X + base.W / 2 - base.CL / 4, base.Y + base.H + base.CL / 2 + base.CL / 4);
	}

	public override void Emit_Code(generate_interface.typ gen)
	{
		if (parse_tree != null)
		{
			expr_output obj = parse_tree as expr_output;
			((generate_interface_oo.typ)gen).start_return();
			interpreter_pkg.emit_code(obj.expr, base.Text, gen);
			((generate_interface_oo.typ)gen).end_return();
		}
		if (Successor != null)
		{
			Successor.Emit_Code(gen);
		}
	}
}
