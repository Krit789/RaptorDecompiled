using System;
using System.Threading;
using NClass.Core;
using numbers;
using parse_tree;

namespace raptor;

public class Step_Helpers
{
	private enum State_Enum
	{
		Entering,
		Leaving
	}

	private static State_Enum state;

	public static Component call_rect;

	public static Component end_oval;

	public static Subchart next_chart;

	public static void Set_State_Entering()
	{
		state = State_Enum.Entering;
	}

	public static void Set_State_Leaving()
	{
		state = State_Enum.Leaving;
	}

	public static bool Am_Leaving()
	{
		return state == State_Enum.Leaving;
	}

	public static void Invoke_Super_Constructor(parameter_list parameters)
	{
		ClassTabPage classTabPage = Runtime.parent.currentClass();
		if (classTabPage == null)
		{
			throw new Exception("no constructor in " + Runtime.parent.running_tab.Text);
		}
		if (state == State_Enum.Entering)
		{
			int count = parse_tree_pkg.count_parameters(parameters);
			ClassType classType = Runtime.parent.projectCore.findClass(classTabPage.Text);
			classType = classType.BaseClass;
			findAndCallConstructor(classType.Name, parameters, classTabPage, count, classType);
		}
		else
		{
			call_rect.running = false;
		}
	}

	public static void Invoke_This_Constructor(parameter_list parameters)
	{
		ClassTabPage classTabPage = Runtime.parent.currentClass();
		if (classTabPage == null)
		{
			throw new Exception("no constructor in " + Runtime.parent.running_tab.Text);
		}
		if (state == State_Enum.Entering)
		{
			int count = parse_tree_pkg.count_parameters(parameters);
			ClassType classType = Runtime.parent.projectCore.findClass(classTabPage.Text);
			findAndCallConstructor(classType.Name, parameters, classTabPage, count, classType);
		}
		else
		{
			call_rect.running = false;
		}
	}

	private static void findAndCallConstructor(string name, parameter_list parameters, ClassTabPage ctp, int count, ClassType theClass)
	{
		foreach (Operation operation in theClass.Operations)
		{
			if (operation is Constructor && (operation as Constructor).numberArguments == count && (operation as Constructor).raptorTab is Procedure_Chart procedure_Chart)
			{
				Runtime.Set_Running(procedure_Chart);
				Invoke_Tab_Procedure_Enter(parameters, procedure_Chart, Runtime.getContext());
				CallStack.setContext(Runtime.getContext());
				return;
			}
		}
		throw new Exception("could not find constructor for " + name + " with " + count + " parameter(s).");
	}

	public static bool Invoke_Constructor(string name, parameter_list parameters)
	{
		int num = parse_tree_pkg.count_parameters(parameters);
		for (ClassType classType = Runtime.parent.projectCore.findClass(name); classType != null; classType = classType.BaseClass)
		{
			foreach (Operation operation in classType.Operations)
			{
				if (operation is Constructor && (operation as Constructor).numberArguments == num && (operation as Constructor).raptorTab is Procedure_Chart procedure_Chart)
				{
					Runtime.Set_Running(procedure_Chart);
					Runtime.Variable context = Runtime.getContext();
					Runtime.setContext(null);
					Invoke_Tab_Procedure_Enter(parameters, procedure_Chart, context);
					CallStack.setContext(Runtime.getContext());
					return true;
				}
			}
		}
		if (num == 0)
		{
			return false;
		}
		throw new Exception("could not find constructor for " + name + " with " + num + " parameter(s).");
	}

	public static void Invoke_Method(string name, parameter_list parameters)
	{
		if (Component.Current_Mode != Mode.Expert)
		{
			throw new Exception("there is no function named " + name);
		}
		Procedure_Chart chart = Runtime.Find_Method_Set_Running(name, parse_tree_pkg.count_parameters(parameters));
		if (state == State_Enum.Entering)
		{
			Runtime.Variable context = Runtime.getContext();
			Runtime.setContext(null);
			Invoke_Tab_Procedure_Enter(parameters, chart, context);
			CallStack.setContext(context);
		}
		else
		{
			Invoke_Tab_Procedure_Exit(parameters, chart);
		}
	}

	public static void Invoke_Tab_Procedure(string name, parameter_list parameters)
	{
		Subchart subchart = Runtime.parent.Find_Tab(name);
		if (!(subchart is Procedure_Chart))
		{
			end_oval.running = false;
			call_rect.running = false;
			Runtime.parent.Possible_Tab_Update(next_chart);
		}
		else if (state == State_Enum.Entering)
		{
			Invoke_Tab_Procedure_Enter(parameters, subchart as Procedure_Chart, null);
		}
		else
		{
			Invoke_Tab_Procedure_Exit(parameters, subchart as Procedure_Chart);
		}
	}

	private static void Invoke_Tab_Procedure_Exit(parameter_list parameters, Procedure_Chart chart)
	{
		end_oval.running = true;
		call_rect.running = false;
		Runtime.parent.currentObj = end_oval;
		int num_params = chart.num_params;
		parameter_list parameter_list = parameters;
		_ = new value[num_params];
		string[] array = new string[num_params];
		bool[] array2 = new bool[num_params];
		bool[] array3 = new bool[num_params];
		object[] array4 = new object[num_params];
		for (int i = 0; i < num_params; i++)
		{
			if (chart.is_output_parameter(i))
			{
				try
				{
					array[i] = ((expr_output)parameter_list.parameter).get_string();
				}
				catch
				{
					array[i] = "";
				}
				if (Runtime.isArray(chart.parameter_name(i)))
				{
					array2[i] = true;
					array3[i] = false;
					array4[i] = Runtime.getValueArray(chart.parameter_name(i));
				}
				else if (Runtime.is_2D_Array(chart.parameter_name(i)))
				{
					array2[i] = false;
					array3[i] = true;
					array4[i] = Runtime.get2DValueArray(chart.parameter_name(i));
				}
				else
				{
					array2[i] = false;
					array3[i] = false;
					array4[i] = Runtime.getVariable(chart.parameter_name(i));
				}
			}
			parameter_list = parameter_list.next;
		}
		Runtime.Decrease_Scope();
		end_oval.running = false;
		call_rect.running = true;
		Runtime.parent.currentObj = call_rect;
		Runtime.parent.Possible_Tab_Update(next_chart);
		parameter_list = parameters;
		for (int j = 0; j < num_params; j++)
		{
			if (chart.is_output_parameter(j))
			{
				if (array2[j])
				{
					value[] array5 = (value[])array4[j];
					for (int num = array5.Length - 1; num >= 0; num--)
					{
						Runtime.setArrayElement(array[j], num + 1, array5[num]);
					}
				}
				else if (array3[j])
				{
					value[][] array6 = array4[j] as value[][];
					for (int num2 = array6.Length - 1; num2 >= 0; num2--)
					{
						for (int num3 = array6[0].Length - 1; num3 >= 0; num3--)
						{
							Runtime.set2DArrayElement(array[j], num2 + 1, num3 + 1, array6[num2][num3]);
						}
					}
				}
				else
				{
					value value = (value)array4[j];
					parse_tree_pkg.ms_assign_to(parameter_list.parameter, value, "Parameter " + (j + 1) + ":");
				}
			}
			parameter_list = parameter_list.next;
		}
		call_rect.running = false;
	}

	private static void Invoke_Tab_Procedure_Enter(parameter_list parameters, Procedure_Chart chart, Runtime.Variable this_value)
	{
		int num_params = chart.num_params;
		value[] array = new value[num_params];
		bool[] array2 = new bool[num_params];
		bool[] array3 = new bool[num_params];
		object[] array4 = new object[num_params];
		int num = 0;
		parameter_list parameter_list;
		for (parameter_list = parameters; parameter_list != null; parameter_list = parameter_list.next)
		{
			num++;
		}
		parameter_list = parameters;
		if (num != num_params)
		{
			string text = ((num_params == 1) ? "parameter" : "parameters");
			string text2 = ((num == 1) ? "was" : "were");
			throw new Exception(chart.Text + " requires " + num_params + " " + text + " but " + num + " " + text2 + " provided.");
		}
		for (int i = 0; i < num_params; i++)
		{
			if (chart.is_input_parameter(i))
			{
				try
				{
					Runtime.processing_parameter_list = true;
					array[i] = ((expr_output)parameter_list.parameter).expr.execute();
				}
				catch
				{
					Runtime.processing_parameter_list = false;
					throw;
				}
				Runtime.processing_parameter_list = false;
				if (numbers_pkg.is_ref_1d(array[i]))
				{
					array2[i] = true;
					array3[i] = false;
					array4[i] = Runtime.getValueArray(numbers_pkg.object_of(array[i]) as Runtime.Variable);
				}
				else if (numbers_pkg.is_ref_2d(array[i]))
				{
					array2[i] = false;
					array3[i] = true;
					array4[i] = Runtime.get2DValueArray(numbers_pkg.object_of(array[i]) as Runtime.Variable);
				}
				else
				{
					array2[i] = false;
					array3[i] = false;
				}
			}
			parameter_list = parameter_list.next;
		}
		Runtime.Increase_Scope(chart.Text);
		if (this_value != null)
		{
			if (chart.method.IsStatic)
			{
				if (this_value.Kind != Runtime.Variable_Kind.Class_Value)
				{
					throw new Exception("can't call static method " + chart.Text + " with object");
				}
			}
			else
			{
				if (this_value.Kind != Runtime.Variable_Kind.Heap_Object)
				{
					throw new Exception("can't call dispatching method " + chart.Text + " without object");
				}
				Runtime.setVariable("this", this_value.Variable_Value);
			}
		}
		for (int j = 0; j < num_params; j++)
		{
			if (!chart.is_input_parameter(j))
			{
				continue;
			}
			if (array2[j])
			{
				value[] array5 = array4[j] as value[];
				for (int num2 = array5.Length - 1; num2 >= 0; num2--)
				{
					Runtime.setArrayElement(chart.parameter_name(j), num2 + 1, array5[num2]);
				}
			}
			else if (array3[j])
			{
				value[][] array6 = array4[j] as value[][];
				for (int num3 = array6.Length - 1; num3 >= 0; num3--)
				{
					for (int num4 = array6[0].Length - 1; num4 >= 0; num4--)
					{
						Runtime.set2DArrayElement(chart.parameter_name(j), num3 + 1, num4 + 1, array6[num3][num4]);
					}
				}
			}
			else
			{
				Runtime.setVariable(chart.parameter_name(j), array[j]);
			}
		}
	}

	internal static Component Step_Once(Component currentObj, Visual_Flow_Form form)
	{
		if (currentObj.need_to_decrease_scope)
		{
			Runtime.Decrease_Scope();
			currentObj.need_to_decrease_scope = false;
			if (parse_tree_pkg.did_function_call)
			{
				if (Runtime.method_return_value != null)
				{
					currentObj.values.Add(Runtime.method_return_value);
				}
				Runtime.method_return_value = null;
			}
		}
		while (currentObj.number_method_expressions_run < currentObj.method_expressions.Count)
		{
			CallStack.Push(currentObj, (Subchart)form.running_tab);
			Set_State_Entering();
			bool num = interpreter_pkg.run_expon(currentObj.method_expressions[currentObj.number_method_expressions_run] as expon, currentObj.Text, currentObj);
			currentObj.number_method_expressions_run++;
			if (num)
			{
				currentObj.need_to_decrease_scope = true;
				form.Possible_Tab_Update(form.running_tab);
				form.currentObj = ((Subchart)form.running_tab).Start;
				form.currentObj.running = true;
				return form.currentObj;
			}
			CallStack.Pop();
		}
		if (currentObj.Name == "Oval")
		{
			if (currentObj.Successor != null)
			{
				Component component = currentObj.Successor.First_Of();
				component.reset_this_method_expressions_run();
				return component;
			}
			return null;
		}
		if (currentObj.Name == "Return")
		{
			Runtime.method_return_value = interpreter_pkg.run_return_value(currentObj.parse_tree, currentObj.Text);
			currentObj.values.Clear();
			return null;
		}
		if (currentObj.Name == "Rectangle")
		{
			if (currentObj.Text.ToLower() == "close_graph_window")
			{
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
				interpreter_pkg.run_assignment(currentObj.parse_tree, currentObj.Text);
				Thread.CurrentThread.Priority = priority;
			}
			else
			{
				interpreter_pkg.run_assignment(currentObj.parse_tree, currentObj.Text);
			}
			currentObj.values.Clear();
			Component component2 = Find_Successor(currentObj);
			component2.reset_this_method_expressions_run();
			return component2;
		}
		if (currentObj.Name == "IF_Control")
		{
			if (interpreter_pkg.run_boolean(currentObj.parse_tree, currentObj.Text))
			{
				if (((IF_Control)currentObj).yes_child != null)
				{
					Component component3 = ((IF_Control)currentObj).yes_child.First_Of();
					component3.reset_this_method_expressions_run();
					return component3;
				}
				Component component4 = Find_Successor(currentObj);
				component4.reset_this_method_expressions_run();
				return component4;
			}
			if (((IF_Control)currentObj).no_child != null)
			{
				Component component5 = ((IF_Control)currentObj).no_child.First_Of();
				component5.reset_this_method_expressions_run();
				return component5;
			}
			Component component6 = Find_Successor(currentObj);
			component6.reset_this_method_expressions_run();
			return component6;
		}
		if (currentObj.Name == "Parallelogram")
		{
			if (((Parallelogram)currentObj).is_input)
			{
				interpreter_pkg.run_input(currentObj.parse_tree, currentObj.Text, ((Parallelogram)currentObj).prompt, ((Parallelogram)currentObj).input_is_expression, ((Parallelogram)currentObj).prompt_tree);
			}
			else
			{
				interpreter_pkg.run_output(currentObj.parse_tree, currentObj.Text);
			}
			Component component7 = Find_Successor(currentObj);
			component7.reset_this_method_expressions_run();
			return component7;
		}
		if (currentObj.Name == "Loop" && ((Loop)currentObj).light_head)
		{
			((Loop)currentObj).light_head = false;
			if (((Loop)currentObj).before_Child != null)
			{
				Component component8 = ((Loop)currentObj).before_Child.First_Of();
				component8.reset_this_method_expressions_run();
				return component8;
			}
			return currentObj;
		}
		if (currentObj.Name == "Loop")
		{
			bool flag = interpreter_pkg.run_boolean(currentObj.parse_tree, currentObj.Text);
			if ((!flag && !Component.reverse_loop_logic) || (flag && Component.reverse_loop_logic))
			{
				if (((Loop)currentObj).after_Child != null)
				{
					Component component9 = ((Loop)currentObj).after_Child.First_Of();
					component9.reset_this_method_expressions_run();
					return component9;
				}
				Component component10 = currentObj.First_Of();
				component10.reset_this_method_expressions_run();
				return component10;
			}
			Component component11 = Find_Successor(currentObj);
			component11.reset_this_method_expressions_run();
			return component11;
		}
		throw new Exception("unrecognized object: " + currentObj.Name);
	}

	public static Component Find_Successor(Component c)
	{
		if (c.Successor != null)
		{
			return c.Successor.First_Of();
		}
		if (c.parent == null)
		{
			throw new Exception("I have no successor or parent!");
		}
		if (c.parent.Name == "IF_Control")
		{
			return Find_Successor(c.parent);
		}
		if (c.parent.Name == "Loop")
		{
			if (c.is_beforeChild)
			{
				return c.parent;
			}
			return c.parent.First_Of();
		}
		throw new Exception("My parent isn't a loop or if_control!");
	}
}
