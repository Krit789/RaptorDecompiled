using System.Reflection.Emit;
using NClass.Core;

namespace raptor;

public class Procedure_Chart : Subchart
{
	public MethodBuilder subMethodBuilder;

	internal Method method;

	public override int num_params => ((Oval_Procedure)Start).Parameter_Count;

	public string param_string => ((Oval_Procedure)Start).Parameter_String;

	public override string getFullName()
	{
		if (method != null && method.Parent != null)
		{
			return method.Parent.Name + "." + Text;
		}
		return Text;
	}

	public string[] getArgs()
	{
		return ((Oval_Procedure)Start).getArgs();
	}

	public bool[] getArgIsInput()
	{
		return ((Oval_Procedure)Start).getArgIsInput();
	}

	public bool[] getArgIsOutput()
	{
		return ((Oval_Procedure)Start).getArgIsOutput();
	}

	public string parameter_name(int i)
	{
		return ((Oval_Procedure)Start).Param_Name(i);
	}

	public string parameter_string(int i)
	{
		return ((Oval_Procedure)Start).Param_String(i);
	}

	public bool is_input_parameter(int i)
	{
		return ((Oval_Procedure)Start).is_input_parameter(i);
	}

	public bool is_output_parameter(int i)
	{
		return ((Oval_Procedure)Start).is_output_parameter(i);
	}

	public string RunDialog(string name, Visual_Flow_Form form)
	{
		string result = ((Oval_Procedure)Start).RunDialog(name, form);
		flow_panel.Invalidate();
		return result;
	}

	public Procedure_Chart(Visual_Flow_Form the_form, string name, int param_count)
	{
		initialize(the_form, name);
		Start = new Oval_Procedure(End, 60, 90, "Oval", param_count);
		Start.Text = "Start";
		Start.scale = Subchart.form.scale;
		Start.Scale(Subchart.form.scale);
		flow_panel.Invalidate();
		kind = Subchart_Kinds.Procedure;
		if (!Component.MONO)
		{
			Initialize_Ink();
		}
	}

	public Procedure_Chart(Visual_Flow_Form the_form, string name, string[] incoming_param_names, bool[] is_input, bool[] is_output)
	{
		initialize(the_form, name);
		Start = new Oval_Procedure(End, 60, 90, "Oval", incoming_param_names, is_input, is_output);
		Start.scale = Subchart.form.scale;
		Start.Scale(Subchart.form.scale);
		flow_panel.Invalidate();
		kind = Subchart_Kinds.Procedure;
		if (!Component.MONO)
		{
			Initialize_Ink();
		}
	}
}
