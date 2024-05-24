using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace raptor;

[Serializable]
internal class Oval_Procedure : Oval
{
	protected int num_params;

	protected string[] param_names;

	protected bool[] param_is_input;

	protected bool[] param_is_output;

	public int Parameter_Count => num_params;

	public string Parameter_String => base.Text.Substring(6);

	public void changeParameters(int num_params, string[] param_names, bool[] param_is_input, bool[] param_is_output)
	{
		this.num_params = num_params;
		this.param_names = param_names;
		this.param_is_input = param_is_input;
		this.param_is_output = param_is_output;
		SetText();
	}

	public string RunDialog(string name, Visual_Flow_Form form)
	{
		string text = Procedure_name.RunDialog(name, ref param_names, ref param_is_input, ref param_is_output, form);
		if (text != null && text != "")
		{
			num_params = param_is_output.Length;
			SetText();
		}
		return text;
	}

	public string[] getArgs()
	{
		return param_names;
	}

	public bool[] getArgIsInput()
	{
		return param_is_input;
	}

	public bool[] getArgIsOutput()
	{
		return param_is_output;
	}

	public string Param_Name(int i)
	{
		return param_names[i];
	}

	public string Param_String(int i)
	{
		string text = "";
		if (param_is_input[i])
		{
			text += "in ";
		}
		if (param_is_output[i])
		{
			text += "out ";
		}
		return text + param_names[i];
	}

	public bool is_input_parameter(int i)
	{
		return param_is_input[i];
	}

	public bool is_output_parameter(int i)
	{
		return param_is_output[i];
	}

	public Oval_Procedure(Component Successor, int height, int width, string str_name, int param_count)
		: base(Successor, height, width, str_name)
	{
		num_params = param_count;
	}

	public Oval_Procedure(Component Successor, int height, int width, string str_name, string[] incoming_param_names, bool[] is_input, bool[] is_output)
		: base(Successor, height, width, str_name)
	{
		param_names = incoming_param_names;
		num_params = incoming_param_names.Length;
		param_is_input = is_input;
		param_is_output = is_output;
		SetText();
	}

	private void SetText()
	{
		base.Text = "Start (";
		for (int i = 0; i < num_params; i++)
		{
			if (i > 0)
			{
				base.Text += ",";
			}
			if (param_is_input[i])
			{
				base.Text += "in ";
			}
			if (param_is_output[i])
			{
				base.Text += "out ";
			}
			base.Text += param_names[i];
		}
		base.Text += ")";
	}

	public Oval_Procedure(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		num_params = info.GetInt32("_numparams");
		param_names = new string[num_params];
		param_is_input = new bool[num_params];
		param_is_output = new bool[num_params];
		for (int i = 0; i < num_params; i++)
		{
			param_names[i] = info.GetString("_paramname" + i);
			param_is_input[i] = info.GetBoolean("_paraminput" + i);
			param_is_output[i] = info.GetBoolean("_paramoutput" + i);
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("_numparams", num_params);
		for (int i = 0; i < num_params; i++)
		{
			info.AddValue("_paramname" + i, param_names[i]);
			info.AddValue("_paraminput" + i, param_is_input[i]);
			info.AddValue("_paramoutput" + i, param_is_output[i]);
		}
	}

	public override void collect_variable_names(IList<string> l, IDictionary<string, string> types)
	{
		if (param_names != null)
		{
			for (int i = 0; i < param_names.Length; i++)
			{
				l.Add(param_names[i]);
			}
		}
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

	public override bool setText(int x, int y, Visual_Flow_Form form)
	{
		bool flag = false;
		if (contains(x, y))
		{
			flag = true;
			if (Component.Current_Mode != Mode.Expert)
			{
				form.menuRenameSubchart_Click(null, null);
			}
			return flag;
		}
		if (Successor != null)
		{
			return Successor.setText(x, y, form);
		}
		return flag;
	}
}
