using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using numbers;
using parse_tree;

namespace raptor;

public class Plugins
{
	private static MethodInfo[] methods;

	private static string[] Suggestions;

	private static List<string> plugins = new List<string>();

	private static List<string> assemblies = new List<string>();

	private static Dictionary<string, bool> is_used = new Dictionary<string, bool>();

	private static char[] separators = new char[2] { ',', ')' };

	private static MethodInfo GetMethod(string name)
	{
		if (methods == null)
		{
			return null;
		}
		for (int i = 0; i < methods.Length; i++)
		{
			if (methods[i].IsStatic && methods[i].Name.ToLower() != "Main" && methods[i].Name.ToLower() == name.ToLower())
			{
				return methods[i];
			}
		}
		return null;
	}

	public static value Invoke_Function(string name, parameter_list parameters)
	{
		MethodInfo method = GetMethod(name);
		ParameterInfo[] parameters2 = method.GetParameters();
		int num = parameters2.Length;
		object[] array = new object[num];
		object[] array2 = new object[num];
		parameter_list parameter_list = parameters;
		for (int i = 0; i < num; i++)
		{
			if (parameters2[i].ParameterType.Name == "Int32" || parameters2[i].ParameterType.Name == "Int32&")
			{
				value x = ((expr_output)parameter_list.parameter).expr.execute();
				array[i] = numbers_pkg.integer_of(x);
				array2[i] = numbers_pkg.integer_of(x);
			}
			else if (parameters2[i].ParameterType.Name == "Double" || parameters2[i].ParameterType.Name == "Double&")
			{
				value x = ((expr_output)parameter_list.parameter).expr.execute();
				array[i] = numbers_pkg.long_float_of(x);
				array2[i] = numbers_pkg.long_float_of(x);
			}
			else if (parameters2[i].ParameterType.Name == "Single" || parameters2[i].ParameterType.Name == "Single&")
			{
				value x = ((expr_output)parameter_list.parameter).expr.execute();
				array[i] = (float)numbers_pkg.long_float_of(x);
				array2[i] = (float)numbers_pkg.long_float_of(x);
			}
			else if (parameters2[i].ParameterType.Name == "String")
			{
				value x = ((expr_output)parameter_list.parameter).expr.execute();
				array[i] = numbers_pkg.string_of(x);
			}
			else if (parameters2[i].ParameterType.Name == "Int32[]")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				array2[i] = ((Array)(array[i] = Runtime.getIntArray(@string))).Clone();
			}
			else if (parameters2[i].ParameterType.Name == "Int32[]&")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				if (Runtime.is_Variable(@string))
				{
					array2[i] = ((Array)(array[i] = Runtime.getIntArray(@string))).Clone();
				}
				else
				{
					array[i] = null;
					array2[i] = null;
				}
			}
			else if (parameters2[i].ParameterType.Name == "Single[]")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				array2[i] = ((Array)(array[i] = Runtime.getArray(@string))).Clone();
			}
			else if (parameters2[i].ParameterType.Name == "Single[]&")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				if (Runtime.is_Variable(@string))
				{
					array2[i] = ((Array)(array[i] = Runtime.getArray(@string))).Clone();
				}
				else
				{
					array[i] = null;
					array2[i] = null;
				}
			}
			else if (parameters2[i].ParameterType.Name == "Double[]")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				array2[i] = ((Array)(array[i] = Runtime.getArray(@string))).Clone();
			}
			else if (parameters2[i].ParameterType.Name == "Double[]&")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				if (Runtime.is_Variable(@string))
				{
					array2[i] = ((Array)(array[i] = Runtime.getArray(@string))).Clone();
				}
				else
				{
					array[i] = null;
					array2[i] = null;
				}
			}
			else if (parameters2[i].ParameterType.Name == "Int32[][]")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				int[][] array3 = Runtime.get2DIntArray(@string);
				array[i] = array3;
				int[][] array4 = Runtime.get2DIntArray(@string);
				array2[i] = array4;
			}
			else if (parameters2[i].ParameterType.Name == "Int32[][]&")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				if (Runtime.is_Variable(@string))
				{
					int[][] array5 = Runtime.get2DIntArray(@string);
					array[i] = array5;
					int[][] array6 = Runtime.get2DIntArray(@string);
					array2[i] = array6;
				}
				else
				{
					array[i] = null;
					array2[i] = null;
				}
			}
			else if (parameters2[i].ParameterType.Name == "Single[][]")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				double[][] array7 = Runtime.get2DArray(@string);
				array[i] = array7;
				double[][] array8 = Runtime.get2DArray(@string);
				array2[i] = array8;
			}
			else if (parameters2[i].ParameterType.Name == "Single[][]&")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				if (Runtime.is_Variable(@string))
				{
					double[][] array9 = Runtime.get2DArray(@string);
					array[i] = array9;
					double[][] array10 = Runtime.get2DArray(@string);
					array2[i] = array10;
				}
				else
				{
					array[i] = null;
					array2[i] = null;
				}
			}
			else if (parameters2[i].ParameterType.Name == "Double[][]")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				double[][] array11 = Runtime.get2DArray(@string);
				array[i] = array11;
				double[][] array12 = Runtime.get2DArray(@string);
				array2[i] = array12;
			}
			else if (parameters2[i].ParameterType.Name == "Double[][]&")
			{
				string @string = ((expr_output)parameter_list.parameter).get_string();
				if (Runtime.is_Variable(@string))
				{
					double[][] array13 = Runtime.get2DArray(@string);
					array[i] = array13;
					double[][] array14 = Runtime.get2DArray(@string);
					array2[i] = array14;
				}
				else
				{
					array[i] = null;
					array2[i] = null;
				}
			}
			parameter_list = parameter_list.next;
		}
		object obj = method.Invoke(null, array);
		parameter_list = parameters;
		for (int j = 0; j < num; j++)
		{
			if (parameters2[j].ParameterType.Name == "Int32&")
			{
				if ((int)array[j] != (int)array2[j])
				{
					parse_tree_pkg.ms_assign_to(parameter_list.parameter, numbers_pkg.make_value__3((int)array[j]), "parameter for " + parameters2[j].Name);
				}
			}
			else if (parameters2[j].ParameterType.Name == "Double&")
			{
				if ((double)array[j] != (double)array2[j])
				{
					parse_tree_pkg.ms_assign_to(parameter_list.parameter, numbers_pkg.make_value__2((double)array[j]), "parameter for " + parameters2[j].Name);
				}
			}
			else if (parameters2[j].ParameterType.Name == "Single&")
			{
				if ((float)array[j] != (float)array2[j])
				{
					parse_tree_pkg.ms_assign_to(parameter_list.parameter, numbers_pkg.make_value__2((float)array[j]), "parameter for " + parameters2[j].Name);
				}
			}
			else if (parameters2[j].ParameterType.Name == "Int32[]" || (parameters2[j].ParameterType.Name == "Int32[]&" && array[j] == array2[j]))
			{
				for (int k = 0; k < ((int[])array[j]).Length; k++)
				{
					if (((int[])array[j])[k] != ((int[])array2[j])[k])
					{
						Runtime.setArrayElement(((expr_output)parameter_list.parameter).get_string(), k + 1, numbers_pkg.make_value__3(((int[])array[j])[k]));
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Int32[]&")
			{
				for (int l = 0; l < ((int[])array[j]).Length; l++)
				{
					Runtime.setArrayElement(((expr_output)parameter_list.parameter).get_string(), l + 1, numbers_pkg.make_value__3(((int[])array[j])[l]));
				}
			}
			else if (parameters2[j].ParameterType.Name == "Single[]" || (parameters2[j].ParameterType.Name == "Single[]&" && array[j] == array2[j]))
			{
				for (int m = 0; m < ((float[])array[j]).Length; m++)
				{
					if (((float[])array[j])[m] != ((float[])array2[j])[m])
					{
						Runtime.setArrayElement(((expr_output)parameter_list.parameter).get_string(), m + 1, numbers_pkg.make_value__2(((float[])array[j])[m]));
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Single[]&")
			{
				for (int n = 0; n < ((float[])array[j]).Length; n++)
				{
					Runtime.setArrayElement(((expr_output)parameter_list.parameter).get_string(), n + 1, numbers_pkg.make_value__2(((float[])array[j])[n]));
				}
			}
			else if (parameters2[j].ParameterType.Name == "Double[]" || (parameters2[j].ParameterType.Name == "Double[]&" && array[j] == array2[j]))
			{
				for (int num2 = 0; num2 < ((double[])array[j]).Length; num2++)
				{
					if (((double[])array[j])[num2] != ((double[])array2[j])[num2])
					{
						Runtime.setArrayElement(((expr_output)parameter_list.parameter).get_string(), num2 + 1, numbers_pkg.make_value__2(((double[])array[j])[num2]));
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Double[]&")
			{
				for (int num3 = 0; num3 < ((double[])array[j]).Length; num3++)
				{
					Runtime.setArrayElement(((expr_output)parameter_list.parameter).get_string(), num3 + 1, numbers_pkg.make_value__2(((double[])array[j])[num3]));
				}
			}
			else if (parameters2[j].ParameterType.Name == "Int32[][]" || (parameters2[j].ParameterType.Name == "Int32[][]&" && array[j] == array2[j]))
			{
				for (int num4 = 0; num4 < ((int[][])array[j]).Length; num4++)
				{
					for (int num5 = 0; num5 < ((int[][])array[j])[0].Length; num5++)
					{
						if (((int[][])array[j])[num4][num5] != ((int[][])array2[j])[num4][num5])
						{
							Runtime.set2DArrayElement(((expr_output)parameter_list.parameter).get_string(), num4 + 1, num5 + 1, numbers_pkg.make_value__2((float)((int[][])array[j])[num4][num5]));
						}
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Int32[][]&")
			{
				for (int num6 = 0; num6 < ((int[][])array[j]).Length; num6++)
				{
					for (int num7 = 0; num7 < ((int[][])array[j])[0].Length; num7++)
					{
						Runtime.set2DArrayElement(((expr_output)parameter_list.parameter).get_string(), num6 + 1, num7 + 1, numbers_pkg.make_value__2((float)((int[][])array[j])[num6][num7]));
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Single[][]" || (parameters2[j].ParameterType.Name == "Single[][]&" && array[j] == array2[j]))
			{
				for (int num8 = 0; num8 < ((float[][])array[j]).Length; num8++)
				{
					for (int num9 = 0; num9 < ((float[][])array[j])[0].Length; num9++)
					{
						if (((float[][])array[j])[num8][num9] != ((float[][])array2[j])[num8][num9])
						{
							Runtime.set2DArrayElement(((expr_output)parameter_list.parameter).get_string(), num8 + 1, num9 + 1, numbers_pkg.make_value__2(((float[][])array[j])[num8][num9]));
						}
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Single[][]&")
			{
				for (int num10 = 0; num10 < ((float[][])array[j]).Length; num10++)
				{
					for (int num11 = 0; num11 < ((float[][])array[j])[0].Length; num11++)
					{
						Runtime.set2DArrayElement(((expr_output)parameter_list.parameter).get_string(), num10 + 1, num11 + 1, numbers_pkg.make_value__2(((float[][])array[j])[num10][num11]));
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Double[][]" || (parameters2[j].ParameterType.Name == "Double[][]&" && array[j] == array2[j]))
			{
				for (int num12 = 0; num12 < ((double[][])array[j]).Length; num12++)
				{
					for (int num13 = 0; num13 < ((double[][])array[j])[0].Length; num13++)
					{
						if (((double[][])array[j])[num12][num13] != ((double[][])array2[j])[num12][num13])
						{
							Runtime.set2DArrayElement(((expr_output)parameter_list.parameter).get_string(), num12 + 1, num13 + 1, numbers_pkg.make_value__2(((double[][])array[j])[num12][num13]));
						}
					}
				}
			}
			else if (parameters2[j].ParameterType.Name == "Double[][]&")
			{
				for (int num14 = 0; num14 < ((double[][])array[j]).Length; num14++)
				{
					for (int num15 = 0; num15 < ((double[][])array[j])[0].Length; num15++)
					{
						Runtime.set2DArrayElement(((expr_output)parameter_list.parameter).get_string(), num14 + 1, num15 + 1, numbers_pkg.make_value__2(((double[][])array[j])[num14][num15]));
					}
				}
			}
			parameter_list = parameter_list.next;
		}
		if (obj == null)
		{
			return numbers_pkg.make_value__2(0.0);
		}
		if (obj.GetType().Name == "Single")
		{
			return numbers_pkg.make_value__2((double)obj);
		}
		if (obj.GetType().Name == "Double")
		{
			return numbers_pkg.make_value__2((double)obj);
		}
		if (obj.GetType().Name == "Int32")
		{
			return numbers_pkg.make_value__3((int)obj);
		}
		if (obj.GetType().Name == "String")
		{
			return numbers_pkg.make_string_value((string)obj);
		}
		if (obj.GetType().Name == "Boolean")
		{
			if ((bool)obj)
			{
				return numbers_pkg.make_value__2(1.0);
			}
			return numbers_pkg.make_value__2(0.0);
		}
		return numbers_pkg.make_value__2(0.0);
	}

	internal static void Emit_Invoke_Function(string name, parameter_list parameters, Generate_IL gil)
	{
		Type type = new value().GetType();
		ILGenerator gen = gil.gen;
		MethodInfo method = GetMethod(name);
		ParameterInfo[] parameters2 = method.GetParameters();
		int num = parameters2.Length;
		LocalBuilder[] array = new LocalBuilder[num];
		parameter_list parameter_list = parameters;
		for (int i = 0; i < num; i++)
		{
			if (parameters2[i].ParameterType.Name == "Int32")
			{
				((expr_output)parameter_list.parameter).expr.emit_code(gil, 0);
				gil.Emit_Method("numbers_pkg", "integer_of");
			}
			else if (parameters2[i].ParameterType.Name == "Single")
			{
				((expr_output)parameter_list.parameter).expr.emit_code(gil, 0);
				gil.Emit_Method("numbers_pkg", "long_float_of");
				gen.Emit(OpCodes.Conv_R4);
			}
			else if (parameters2[i].ParameterType.Name == "Double")
			{
				((expr_output)parameter_list.parameter).expr.emit_code(gil, 0);
				gil.Emit_Method("numbers_pkg", "long_float_of");
			}
			else if (parameters2[i].ParameterType.Name == "String")
			{
				try
				{
					string @string = ((expr_output)parameter_list.parameter).get_string();
					FieldInfo field = type.GetField("s");
					gil.Emit_Load(@string);
					gen.Emit(OpCodes.Ldfld, field);
				}
				catch
				{
					((expr_output)parameter_list.parameter).expr.emit_code(gil, 0);
					gil.Emit_Method("numbers_pkg", "string_of");
				}
			}
			else
			{
				if (parameters2[i].ParameterType.Name == "Int32&")
				{
					throw new Exception("parameter type \"ref int\" of method " + method.Name + " not supported.");
				}
				if (parameters2[i].ParameterType.Name == "Single&")
				{
					throw new Exception("parameter type \"ref float\" of method " + method.Name + " not supported.");
				}
				if (parameters2[i].ParameterType.Name == "Double&")
				{
					string string2 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string2);
					FieldInfo field2 = type.GetField("v");
					gen.Emit(OpCodes.Ldflda, field2);
				}
				else if (parameters2[i].ParameterType.Name == "Int32[]" || parameters2[i].ParameterType.Name == "Int32[]&")
				{
					string string3 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string3);
					gil.Emit_Method_Virt("raptor.Value_Array", "get_Int32a");
					array[i] = gen.DeclareLocal(Type.GetType("System.Int32[]"));
					gen.Emit(OpCodes.Stloc, array[i]);
					if (parameters2[i].ParameterType.Name == "Int32[]&")
					{
						gen.Emit(OpCodes.Ldloca, array[i]);
					}
					else
					{
						gen.Emit(OpCodes.Ldloc, array[i]);
					}
				}
				else if (parameters2[i].ParameterType.Name == "Single[]" || parameters2[i].ParameterType.Name == "Single[]&")
				{
					string string4 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string4);
					gil.Emit_Method_Virt("raptor.Value_Array", "get_Singlea");
					array[i] = gen.DeclareLocal(Type.GetType("System.Single[]"));
					gen.Emit(OpCodes.Stloc, array[i]);
					if (parameters2[i].ParameterType.Name == "Single[]&")
					{
						gen.Emit(OpCodes.Ldloca, array[i]);
					}
					else
					{
						gen.Emit(OpCodes.Ldloc, array[i]);
					}
				}
				else if (parameters2[i].ParameterType.Name == "Double[]" || parameters2[i].ParameterType.Name == "Double[]&")
				{
					string string5 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string5);
					gil.Emit_Method_Virt("raptor.Value_Array", "get_Doublea");
					array[i] = gen.DeclareLocal(Type.GetType("System.Double[]"));
					gen.Emit(OpCodes.Stloc, array[i]);
					if (parameters2[i].ParameterType.Name == "Double[]&")
					{
						gen.Emit(OpCodes.Ldloca, array[i]);
					}
					else
					{
						gen.Emit(OpCodes.Ldloc, array[i]);
					}
				}
				else if (parameters2[i].ParameterType.Name == "Int32[][]" || parameters2[i].ParameterType.Name == "Int32[][]&")
				{
					string string6 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string6);
					gil.Emit_Method_Virt("raptor.Value_2D_Array", "get_Int32aa");
					array[i] = gen.DeclareLocal(Type.GetType("System.Int32[][]"));
					gen.Emit(OpCodes.Stloc, array[i]);
					if (parameters2[i].ParameterType.Name == "Int32[][]&")
					{
						gen.Emit(OpCodes.Ldloca, array[i]);
					}
					else
					{
						gen.Emit(OpCodes.Ldloc, array[i]);
					}
				}
				else if (parameters2[i].ParameterType.Name == "Single[][]" || parameters2[i].ParameterType.Name == "Single[][]&")
				{
					string string7 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string7);
					gil.Emit_Method_Virt("raptor.Value_2D_Array", "get_Singleaa");
					array[i] = gen.DeclareLocal(Type.GetType("System.Single[][]"));
					gen.Emit(OpCodes.Stloc, array[i]);
					if (parameters2[i].ParameterType.Name == "Single[][]&")
					{
						gen.Emit(OpCodes.Ldloca, array[i]);
					}
					else
					{
						gen.Emit(OpCodes.Ldloc, array[i]);
					}
				}
				else if (parameters2[i].ParameterType.Name == "Double[][]" || parameters2[i].ParameterType.Name == "Double[][]&")
				{
					string string8 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string8);
					gil.Emit_Method_Virt("raptor.Value_2D_Array", "get_Doubleaa");
					array[i] = gen.DeclareLocal(Type.GetType("System.Double[][]"));
					gen.Emit(OpCodes.Stloc, array[i]);
					if (parameters2[i].ParameterType.Name == "Double[][]&")
					{
						gen.Emit(OpCodes.Ldloca, array[i]);
					}
					else
					{
						gen.Emit(OpCodes.Ldloc, array[i]);
					}
				}
			}
			parameter_list = parameter_list.next;
		}
		Set_Is_Used(method.DeclaringType.Assembly.GetName().Name);
		gen.Emit(OpCodes.Call, method);
		parameter_list = parameters;
		for (int j = 0; j < num; j++)
		{
			if (!(parameters2[j].ParameterType.Name == "Int32&") && !(parameters2[j].ParameterType.Name == "Single&"))
			{
				if (parameters2[j].ParameterType.Name == "Int32[]" || parameters2[j].ParameterType.Name == "Int32[]&")
				{
					string string9 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string9);
					gen.Emit(OpCodes.Ldloc, array[j]);
					gil.Emit_Method_Virt("raptor.Value_Array", "set_Int32a");
				}
				else if (parameters2[j].ParameterType.Name == "Single[]" || parameters2[j].ParameterType.Name == "Single[]&")
				{
					string string10 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string10);
					gen.Emit(OpCodes.Ldloc, array[j]);
					gil.Emit_Method_Virt("raptor.Value_Array", "set_Singlea");
				}
				else if (parameters2[j].ParameterType.Name == "Double[]" || parameters2[j].ParameterType.Name == "Double[]&")
				{
					string string11 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string11);
					gen.Emit(OpCodes.Ldloc, array[j]);
					gil.Emit_Method_Virt("raptor.Value_Array", "set_Doublea");
				}
				else if (parameters2[j].ParameterType.Name == "Int32[][]" || parameters2[j].ParameterType.Name == "Int32[][]&")
				{
					string string12 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string12);
					gen.Emit(OpCodes.Ldloc, array[j]);
					gil.Emit_Method_Virt("raptor.Value_2D_Array", "set_Int32aa");
				}
				else if (parameters2[j].ParameterType.Name == "Single[][]" || parameters2[j].ParameterType.Name == "Single[][]&")
				{
					string string13 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string13);
					gen.Emit(OpCodes.Ldloc, array[j]);
					gil.Emit_Method_Virt("raptor.Value_2D_Array", "set_Singleaa");
				}
				else if (parameters2[j].ParameterType.Name == "Double[][]" || parameters2[j].ParameterType.Name == "Double[][]&")
				{
					string string14 = ((expr_output)parameter_list.parameter).get_string();
					gil.Emit_Load(string14);
					gen.Emit(OpCodes.Ldloc, array[j]);
					gil.Emit_Method_Virt("raptor.Value_2D_Array", "set_Doubleaa");
				}
			}
			parameter_list = parameter_list.next;
		}
		if (method.ReturnType.Name == "Single")
		{
			gen.Emit(OpCodes.Conv_R8);
			gil.Emit_Method("numbers_pkg", "make_value__2");
		}
		else if (method.ReturnType.Name == "Double")
		{
			gil.Emit_Method("numbers_pkg", "make_value__2");
		}
		else if (method.ReturnType.Name == "Int32")
		{
			gil.Emit_Method("numbers_pkg", "make_value__3");
		}
		else if (method.ReturnType.Name == "String")
		{
			gil.Emit_Method("numbers_pkg", "make_string_value");
		}
		else
		{
			_ = method.ReturnType.Name == "Boolean";
		}
	}

	public static void Invoke(string name, parameter_list parameters)
	{
		Invoke_Function(name, parameters);
	}

	public static int Parameter_Count(string name)
	{
		try
		{
			MethodInfo method = GetMethod(name);
			if (method == null)
			{
				return 0;
			}
			return method.GetParameters().Length;
		}
		catch
		{
			return 0;
		}
	}

	public static bool Is_Procedure(string name)
	{
		try
		{
			MethodInfo method = GetMethod(name);
			if (method == null)
			{
				return false;
			}
			return method.ReturnType.Name == "Void";
		}
		catch
		{
			return false;
		}
	}

	public static bool Is_Function(string name)
	{
		try
		{
			MethodInfo method = GetMethod(name);
			if (method == null)
			{
				return false;
			}
			return method.ReturnType.Name == "Single" || method.ReturnType.Name == "String" || method.ReturnType.Name == "Double" || method.ReturnType.Name == "Int32";
		}
		catch
		{
			return false;
		}
	}

	public static bool Is_Boolean_Function(string name)
	{
		try
		{
			MethodInfo method = GetMethod(name);
			if (method == null)
			{
				return false;
			}
			return method.ReturnType.Name == "Boolean";
		}
		catch
		{
			return false;
		}
	}

	public static bool Is_Pluginable_Method(MethodInfo m)
	{
		if (m.IsStatic)
		{
			if (!(m.ReturnType.Name == "Void") && !(m.ReturnType.Name == "Int32") && !(m.ReturnType.Name == "Single") && !(m.ReturnType.Name == "String") && !(m.ReturnType.Name == "Double"))
			{
				return m.ReturnType.Name == "Boolean";
			}
			return true;
		}
		return false;
	}

	public static void Process_Assembly(Assembly assembly, ArrayList method_list)
	{
		Type[] types = assembly.GetTypes();
		for (int i = 0; i < types.Length; i++)
		{
			if (!types[i].IsPublic)
			{
				continue;
			}
			MethodInfo[] array = types[i].GetMethods();
			for (int j = 0; j < array.Length; j++)
			{
				if (Is_Pluginable_Method(array[j]))
				{
					method_list.Add(array[j]);
				}
			}
		}
	}

	public static string[] Get_Plugin_List()
	{
		int num = 0;
		for (int i = 0; i < assemblies.Count; i++)
		{
			if (is_used[assemblies[i]])
			{
				num++;
			}
		}
		string[] array = new string[num];
		for (int j = 0; j < plugins.Count; j++)
		{
			if (is_used[assemblies[j]])
			{
				array[--num] = plugins[j];
			}
		}
		return array;
	}

	public static string[] Get_Assembly_Names()
	{
		int num = 0;
		for (int i = 0; i < assemblies.Count; i++)
		{
			if (is_used[assemblies[i]])
			{
				num++;
			}
		}
		string[] array = new string[num];
		for (int j = 0; j < assemblies.Count; j++)
		{
			if (is_used[assemblies[j]])
			{
				array[--num] = assemblies[j];
			}
		}
		return array;
	}

	public static void Set_Is_Used(string name)
	{
		is_used[name] = true;
	}

	public static void Load_Plugins(string filename)
	{
		ArrayList arrayList = new ArrayList();
		DirectoryInfo parent = Directory.GetParent(Application.ExecutablePath);
		plugins.Clear();
		assemblies.Clear();
		FileInfo[] files = parent.GetFiles("plugin*.dll");
		for (int i = 0; i < files.Length; i++)
		{
			try
			{
				Assembly assembly = Assembly.LoadFrom(files[i].FullName);
				Process_Assembly(assembly, arrayList);
				plugins.Add(files[i].FullName);
				assemblies.Add(assembly.GetName().Name);
				if (!is_used.ContainsKey(assembly.GetName().Name))
				{
					is_used.Add(assembly.GetName().Name, value: false);
				}
			}
			catch
			{
			}
		}
		if (filename != "")
		{
			try
			{
				files = Directory.GetParent(filename).GetFiles("plugin*.dll");
				for (int j = 0; j < files.Length; j++)
				{
					try
					{
						Assembly assembly = Assembly.LoadFrom(files[j].FullName);
						Process_Assembly(assembly, arrayList);
						plugins.Add(files[j].FullName);
						assemblies.Add(assembly.GetName().Name);
						is_used.Add(assembly.GetName().Name, value: false);
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}
		if (arrayList.Count <= 0)
		{
			return;
		}
		methods = new MethodInfo[arrayList.Count];
		Suggestions = new string[methods.Length];
		for (int k = 0; k < methods.Length; k++)
		{
			methods[k] = (MethodInfo)arrayList[k];
			Suggestions[k] = methods[k].Name;
			ParameterInfo[] parameters = methods[k].GetParameters();
			if (parameters.Length >= 1)
			{
				Suggestions[k] += "(";
			}
			for (int l = 0; l < parameters.Length - 1; l++)
			{
				Suggestions[k] = Suggestions[k] + parameters[l].Name + ",";
			}
			if (parameters.Length >= 1)
			{
				Suggestions[k] += parameters[parameters.Length - 1].Name;
				Suggestions[k] += ")";
			}
		}
	}

	public static Prefix_Results Prefix_Suggestions(string name, int kind, bool check_all)
	{
		Prefix_Results prefix_Results = new Prefix_Results();
		int num = 0;
		int num2 = methods.Length;
		prefix_Results.count = 0;
		if (name.Contains(".") && Runtime.isObjectOriented())
		{
			prefix_Results.count += Dialog_Helpers.Prefix_Suggestion_Count(name.ToLower());
		}
		for (int i = 0; i < num2; i++)
		{
			if (methods[i].IsStatic && methods[i].Name.ToLower().StartsWith(name.ToLower()) && (check_all || (kind == interpreter_pkg.expr_dialog && methods[i].ReturnType.Name != "Void") || (kind == interpreter_pkg.call_dialog && methods[i].ReturnType.Name == "Void")))
			{
				prefix_Results.count++;
			}
		}
		if (kind == interpreter_pkg.call_dialog && Component.Current_Mode != Mode.Expert)
		{
			for (int j = Runtime.parent.mainIndex; j < Runtime.parent.carlisle.TabCount; j++)
			{
				if (Runtime.parent.carlisle.TabPages[j].Text.ToLower().StartsWith(name.ToLower()))
				{
					prefix_Results.count++;
				}
			}
		}
		else
		{
			for (int k = 0; k < Dialog_Helpers.Get_List().Count; k++)
			{
				if (Dialog_Helpers.Get_List()[k].ToLower().StartsWith(name.ToLower()))
				{
					prefix_Results.count++;
				}
			}
		}
		prefix_Results.Suggestions = new string[prefix_Results.count];
		if (name.Contains(".") && Runtime.isObjectOriented())
		{
			foreach (string oO_ in Dialog_Helpers.Get_OO_List())
			{
				prefix_Results.Suggestions[num++] += oO_;
			}
		}
		if (kind == interpreter_pkg.call_dialog && Component.Current_Mode != Mode.Expert)
		{
			for (int l = Runtime.parent.mainIndex; l < Runtime.parent.carlisle.TabCount; l++)
			{
				if (Runtime.parent.carlisle.TabPages[l].Text.ToLower().StartsWith(name.ToLower()))
				{
					if (Runtime.parent.carlisle.TabPages[l] is Procedure_Chart)
					{
						prefix_Results.Suggestions[num++] = Runtime.parent.carlisle.TabPages[l].Text + ((Procedure_Chart)Runtime.parent.carlisle.TabPages[l]).param_string;
					}
					else
					{
						prefix_Results.Suggestions[num++] = Runtime.parent.carlisle.TabPages[l].Text;
					}
				}
			}
		}
		else
		{
			for (int m = 0; m < Dialog_Helpers.Get_List().Count; m++)
			{
				if (Dialog_Helpers.Get_List()[m].ToLower().StartsWith(name.ToLower()))
				{
					prefix_Results.Suggestions[num++] = Dialog_Helpers.Get_List()[m];
				}
			}
		}
		for (int n = 0; n < num2; n++)
		{
			if (methods[n].IsStatic && methods[n].Name.ToLower().StartsWith(name.ToLower()) && (check_all || (kind == interpreter_pkg.expr_dialog && methods[n].ReturnType.Name != "Void") || (kind == interpreter_pkg.call_dialog && methods[n].ReturnType.Name == "Void")))
			{
				prefix_Results.Suggestions[num++] = Suggestions[n];
			}
		}
		return prefix_Results;
	}

	public static Bold_Results Get_Bold_Locations(string name, int comma_count)
	{
		Bold_Results bold_Results = new Bold_Results();
		if (Runtime.isObjectOriented())
		{
			foreach (string oO_ in Dialog_Helpers.Get_OO_List())
			{
				int num = oO_.IndexOf('(');
				int i = 0;
				if (num <= 0 || !oO_.Substring(0, num).ToLower().Equals(name.ToLower()))
				{
					continue;
				}
				bold_Results.found = true;
				bold_Results.Suggestion = oO_;
				bold_Results.bold_start = num + 2;
				try
				{
					for (; i < comma_count; i++)
					{
						bold_Results.bold_start = oO_.IndexOfAny(separators, bold_Results.bold_start) + 1;
					}
					bold_Results.bold_finish = oO_.IndexOfAny(separators, bold_Results.bold_start);
				}
				catch
				{
					bold_Results.bold_start = 0;
					bold_Results.bold_finish = 0;
				}
				if (bold_Results.bold_start < 0 || bold_Results.bold_finish < 0)
				{
					bold_Results.bold_start = 0;
					bold_Results.bold_finish = 0;
				}
				return bold_Results;
			}
		}
		MethodInfo method = GetMethod(name);
		if (method == null)
		{
			bold_Results.found = false;
			for (int j = Runtime.parent.mainIndex; j < Runtime.parent.carlisle.TabCount; j++)
			{
				if (Runtime.parent.carlisle.TabPages[j].Text.ToLower() == name.ToLower() && Runtime.parent.carlisle.TabPages[j] is Procedure_Chart && ((Procedure_Chart)Runtime.parent.carlisle.TabPages[j]).num_params > 0)
				{
					Procedure_Chart procedure_Chart = Runtime.parent.carlisle.TabPages[j] as Procedure_Chart;
					int[] array = new int[procedure_Chart.num_params];
					for (int k = 0; k < procedure_Chart.num_params; k++)
					{
						array[k] = procedure_Chart.parameter_string(k).Length;
					}
					bold_Results.Suggestion = procedure_Chart.Text + procedure_Chart.param_string;
					bold_Results.found = true;
					bold_Results.bold_start = bold_Results.Suggestion.IndexOf("(") + 2;
					for (int l = 0; l < comma_count; l++)
					{
						bold_Results.bold_start += array[l] + 1;
					}
					bold_Results.bold_finish = bold_Results.bold_start + array[comma_count] - 1;
				}
			}
			return bold_Results;
		}
		try
		{
			bold_Results.found = true;
			ParameterInfo[] parameters = method.GetParameters();
			if (comma_count >= parameters.Length)
			{
				bold_Results.bold_start = 0;
				bold_Results.bold_finish = 0;
				bold_Results.Suggestion = Prefix_Suggestions(name, 1, check_all: true).Suggestions[0];
			}
			else
			{
				for (int m = 0; m < methods.Length; m++)
				{
					if (methods[m] == method)
					{
						bold_Results.Suggestion = Suggestions[m];
					}
				}
				bold_Results.bold_start = method.Name.Length + 2;
				for (int n = 0; n < comma_count; n++)
				{
					bold_Results.bold_start += parameters[n].Name.Length + 1;
				}
				bold_Results.bold_finish = bold_Results.bold_start + parameters[comma_count].Name.Length - 1;
			}
			return bold_Results;
		}
		catch
		{
			bold_Results.found = false;
			return bold_Results;
		}
	}
}
