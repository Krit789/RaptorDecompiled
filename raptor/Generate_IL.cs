#define TRACE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using dotnetgraphlibrary;
using generate_interface;
using GeneratorAda;
using numbers;
using parse_tree;

namespace raptor;

public class Generate_IL : Imperative_Interface, typ
{
	private class procedure_class
	{
		public string name;

		public string[] args;

		public bool[] arg_is_input;

		public bool[] arg_is_output;

		public MethodBuilder subMethodBuilder;

		public procedure_class(string name_in, string[] args_in, bool[] arg_is_input_in, bool[] arg_is_output_in)
		{
			name = name_in;
			args = args_in;
			arg_is_input = arg_is_input_in;
			arg_is_output = arg_is_output_in;
		}
	}

	private class method_call
	{
		public int name;

		public int param_count;

		public method_call()
		{
		}

		public method_call(int in_name)
		{
			name = in_name;
			param_count = 0;
		}
	}

	private class subchart_call
	{
		public procedure_class proc;

		public subchart_call(procedure_class in_proc)
		{
			proc = in_proc;
		}
	}

	private class label_pair
	{
		public Label l2;

		public Label l3;

		public label_pair(ILGenerator gen)
		{
			l2 = gen.DefineLabel();
			l3 = gen.DefineLabel();
		}
	}

	private struct loop_vars
	{
		public label_pair lp;

		public bool is_negated;
	}

	private enum input_kind
	{
		variable,
		array,
		array2d
	}

	private FieldInfo random_generator;

	private Hashtable variables = new Hashtable();

	private Hashtable arrays = new Hashtable();

	private Hashtable arrays_2d = new Hashtable();

	private Dictionary<string, procedure_class> procedures = new Dictionary<string, procedure_class>();

	private TypeBuilder myTypeBuilder;

	private MethodBuilder myMethodBuilder;

	private ILGenerator myILGenerator;

	private AssemblyBuilder myAssemblyBuilder;

	public ILGenerator gen;

	private procedure_class pc;

	private string name_array_2d;

	private string name_array_1d;

	private string name_variable;

	private bool dest_is_array;

	private input_kind kind_of_input;

	public Generate_IL(string name)
	{
		AssemblyName name2 = new AssemblyName
		{
			Name = name,
			Version = new Version("1.0.0.2001")
		};
		myAssemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(name2, AssemblyBuilderAccess.RunAndSave);
		ModuleBuilder moduleBuilder = myAssemblyBuilder.DefineDynamicModule("MyModule", name + ".exe");
		myTypeBuilder = moduleBuilder.DefineType("MyType");
		myMethodBuilder = myTypeBuilder.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, typeof(void), null);
		myILGenerator = myMethodBuilder.GetILGenerator();
		Type type = null;
		Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType("System.Random");
			if (type != null)
			{
				break;
			}
		}
		variables.Clear();
		arrays.Clear();
		arrays_2d.Clear();
		random_generator = myTypeBuilder.DefineField("random_generator", type, FieldAttributes.Static);
		ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
		myILGenerator.Emit(OpCodes.Newobj, constructor);
		myILGenerator.Emit(OpCodes.Stsfld, random_generator);
		gen = myILGenerator;
		if (name != "MyAssembly")
		{
			Emit_Method("ada_interpreter_pkg", "adainit");
		}
	}

	public string Get_Menu_Name()
	{
		return "&Standalone";
	}

	public void Finish()
	{
		gen = myILGenerator;
		Emit_Method("raptor_files_pkg", "close_files");
		myILGenerator.Emit(OpCodes.Ret);
		myTypeBuilder.CreateType();
		myAssemblyBuilder.SetEntryPoint(myMethodBuilder);
	}

	public void Save_Result(string filename)
	{
		myAssemblyBuilder.Save(filename);
	}

	public bool Is_Postfix()
	{
		return true;
	}

	public void Declare_Procedure(string name, string[] args, bool[] arg_is_input, bool[] arg_is_output)
	{
		procedure_class procedure_class = new procedure_class(name, args, arg_is_input, arg_is_output);
		Type[] array = new Type[args.Length];
		for (int i = 0; i < args.Length; i++)
		{
			array[i] = typeof(object);
		}
		procedure_class.subMethodBuilder = myTypeBuilder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, typeof(void), array);
		procedures.Add(name.ToLower().Trim(), procedure_class);
	}

	private MethodInfo Get_Method(string package, string name)
	{
		Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
		Type type = null;
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType(package);
			if (type != null)
			{
				break;
			}
		}
		return type.GetMethod(name);
	}

	public void Emit_Get_Mouse_Button()
	{
		Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
		Type type = null;
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType("dotnetgraphlibrary.dotnetgraph");
			if (type != null)
			{
				break;
			}
		}
		Type[] array = new Type[1];
		Mouse_Button mouse_Button = Mouse_Button.Left_Button;
		array[0] = mouse_Button.GetType();
		MethodInfo method = type.GetMethod("Get_Mouse_Button", array);
		gen.EmitCall(OpCodes.Call, method, null);
	}

	public void Emit_Get_Click(int x_or_y)
	{
		if (x_or_y == 0)
		{
			Emit_Method("dotnetgraphlibrary.dotnetgraph", "Get_Click_X");
		}
		else
		{
			Emit_Method("dotnetgraphlibrary.dotnetgraph", "Get_Click_Y");
		}
		Emit_Method("numbers_pkg", "make_value__3");
	}

	public void Emit_Array_Size(string name)
	{
		LocalBuilder localBuilder = null;
		if (arrays.ContainsKey(name.ToLower()))
		{
			localBuilder = (LocalBuilder)arrays[name.ToLower()];
			gen.Emit(OpCodes.Ldloc, localBuilder);
			Emit_Method_Virt("raptor.Value_Array", "Get_Length");
		}
		else
		{
			if (!variables.ContainsKey(name.ToLower()))
			{
				throw new Exception("can only take length_of 1D array or string");
			}
			localBuilder = (LocalBuilder)variables[name.ToLower()];
			gen.Emit(OpCodes.Ldloc, localBuilder);
			Emit_Method("numbers_pkg", "string_of");
			Emit_Method_Virt("System.String", "get_Length");
		}
		Emit_Method("numbers_pkg", "make_value__3");
	}

	public void Emit_String_Length()
	{
		Emit_Method_Virt("System.String", "get_Length");
		Emit_Method("numbers_pkg", "make_value__3");
	}

	public void Emit_Assign_To(string name)
	{
		LocalBuilder localBuilder;
		if (variables.ContainsKey(name.ToLower()))
		{
			localBuilder = (LocalBuilder)variables[name.ToLower()];
		}
		else
		{
			localBuilder = gen.DeclareLocal(numbers_pkg.pi.GetType());
			variables.Add(name.ToLower(), localBuilder);
		}
		Emit_Method_Virt("numbers.value", "_deep_clone");
		gen.Emit(OpCodes.Stloc, localBuilder);
	}

	public void Indent()
	{
	}

	public void Emit_Assign_To_Array(string name)
	{
		if (arrays.ContainsKey(name.ToLower()))
		{
			_ = (LocalBuilder)arrays[name.ToLower()];
			Emit_Method_Virt("raptor.Value_Array", "Set_Value");
		}
		else
		{
			Emit_Method("raptor.Runtime_Helpers", "Set_Value_String");
		}
	}

	public void Emit_Assign_To_Array_2D(string name)
	{
		if (arrays_2d.ContainsKey(name.ToLower()))
		{
			_ = (LocalBuilder)arrays_2d[name.ToLower()];
		}
		Emit_Method_Virt("raptor.Value_2D_Array", "Set_Value");
	}

	public void Emit_Load(string name)
	{
		LocalBuilder local;
		if (variables.ContainsKey(name.ToLower()))
		{
			local = (LocalBuilder)variables[name.ToLower()];
		}
		else if (arrays.ContainsKey(name.ToLower()))
		{
			local = (LocalBuilder)arrays[name.ToLower()];
		}
		else
		{
			if (!arrays_2d.ContainsKey(name.ToLower()))
			{
				throw new Exception(name + ": variable used before assigned");
			}
			local = (LocalBuilder)arrays_2d[name.ToLower()];
		}
		gen.Emit(OpCodes.Ldloc, local);
	}

	public void Emit_Load_Array_Start(string name)
	{
		Emit_Load(name);
	}

	public void Emit_Load_Array_After_Index(string name)
	{
		if (arrays.ContainsKey(name.ToLower()))
		{
			_ = (LocalBuilder)arrays[name.ToLower()];
			Emit_Method_Virt("raptor.Value_Array", "Get_Value");
		}
		else
		{
			Emit_Method("raptor.Runtime_Helpers", "Get_Value_String");
		}
	}

	public void Emit_Load_Array_2D_Start(string name)
	{
		Emit_Load(name);
	}

	public void Emit_Load_Array_2D_Between_Indices()
	{
	}

	public void Emit_Load_Array_2D_After_Indices(string name)
	{
		if (arrays_2d.ContainsKey(name.ToLower()))
		{
			_ = (LocalBuilder)arrays_2d[name.ToLower()];
		}
		Emit_Method_Virt("raptor.Value_2D_Array", "Get_Value");
	}

	public bool Previously_Declared(string name)
	{
		if (!variables.ContainsKey(name.ToLower()) && !arrays.ContainsKey(name.ToLower()))
		{
			return arrays_2d.ContainsKey(name.ToLower());
		}
		return true;
	}

	public void Declare_String_Variable(string name)
	{
		if (!Previously_Declared(name))
		{
			LocalBuilder value = gen.DeclareLocal(name.GetType());
			variables.Add(name.ToLower(), value);
		}
	}

	public void Declare_As_Variable(string name)
	{
		if (!Previously_Declared(name))
		{
			Type type = numbers_pkg.pi.GetType();
			LocalBuilder localBuilder = gen.DeclareLocal(type);
			variables.Add(name.ToLower(), localBuilder);
			gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
			gen.Emit(OpCodes.Stloc, localBuilder);
		}
	}

	public void Declare_As_1D_Array(string name)
	{
		Trace.WriteLine("new array " + name);
		if (!Previously_Declared(name))
		{
			Type type = Type.GetType("raptor.Value_Array");
			LocalBuilder localBuilder = gen.DeclareLocal(type);
			arrays.Add(name.ToLower(), localBuilder);
			gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
			gen.Emit(OpCodes.Stloc, localBuilder);
		}
	}

	public void Declare_As_2D_Array(string name)
	{
		if (!Previously_Declared(name))
		{
			Type type = Type.GetType("raptor.Value_2D_Array");
			LocalBuilder localBuilder = gen.DeclareLocal(type);
			arrays_2d.Add(name.ToLower(), localBuilder);
			gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
			gen.Emit(OpCodes.Stloc, localBuilder);
		}
	}

	public void Emit_Relation(int relation)
	{
		switch (relation)
		{
		case 1:
			Emit_Method("numbers_pkg", "Ogt");
			break;
		case 2:
			Emit_Method("numbers_pkg", "Oge");
			break;
		case 3:
			Emit_Method("numbers_pkg", "Olt");
			break;
		case 4:
			Emit_Method("numbers_pkg", "Ole");
			break;
		case 5:
			Emit_Method("numbers_pkg", "Oeq");
			break;
		case 6:
			Emit_Method("numbers_pkg", "Oeq");
			gen.Emit(OpCodes.Ldc_I4_1);
			gen.Emit(OpCodes.Xor);
			break;
		}
	}

	public void Emit_Method(string package, string name)
	{
		gen.EmitCall(OpCodes.Call, Get_Method(package, name), null);
	}

	public void Emit_Method_Virt(string package, string name)
	{
		gen.EmitCall(OpCodes.Callvirt, Get_Method(package, name), null);
	}

	public void Emit_Sleep()
	{
	}

	public void Emit_Past_Sleep()
	{
		Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
		Type type = null;
		Emit_Method("numbers_pkg", "long_float_of");
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType("System.Threading.Thread");
			if (type != null)
			{
				break;
			}
		}
		Type[] types = new Type[1] { Type.GetType("System.Int32") };
		gen.Emit(OpCodes.Ldc_R8, 1000.0);
		gen.Emit(OpCodes.Mul);
		gen.Emit(OpCodes.Conv_I4);
		MethodInfo method = type.GetMethod("Sleep", types);
		gen.EmitCall(OpCodes.Call, method, null);
	}

	public void Emit_And()
	{
		gen.Emit(OpCodes.And);
	}

	public void Emit_Or()
	{
		gen.Emit(OpCodes.Or);
	}

	public void Emit_Not()
	{
		gen.Emit(OpCodes.Ldc_I4_1);
		gen.Emit(OpCodes.Xor);
	}

	public void Emit_Xor()
	{
		gen.Emit(OpCodes.Xor);
	}

	public void Emit_To_Integer()
	{
		gen.Emit(OpCodes.Conv_I4);
	}

	public void Emit_Load_Boolean(bool val)
	{
		gen.Emit(OpCodes.Ldc_I4, val ? 1 : 0);
	}

	public void Emit_Load_Number(double val)
	{
		gen.Emit(OpCodes.Ldc_R8, val);
		Emit_Method("numbers_pkg", "make_value__2");
	}

	public void Emit_Load_Character(char val)
	{
		gen.Emit(OpCodes.Ldc_I4, val);
		Emit_Method("numbers_pkg", "make_value__4");
	}

	public void Emit_Load_String(string val)
	{
		gen.Emit(OpCodes.Ldstr, val);
		Emit_Method("numbers_pkg", "make_string_value");
	}

	public void Emit_Load_String_Const(string val)
	{
		gen.Emit(OpCodes.Ldstr, val);
	}

	public void Emit_Load_Static(string package, string field)
	{
		Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
		Type type = null;
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType(package);
			if (type != null)
			{
				break;
			}
		}
		FieldInfo field2 = type.GetField(field);
		gen.Emit(OpCodes.Ldsfld, field2);
	}

	public void Emit_Conversion(int o)
	{
	}

	public void Emit_End_Conversion(int o)
	{
		parse_tree_pkg.emit_conversion(o, this);
	}

	public void Emit_Value_To_Color_Type()
	{
		Emit_Method("numbers_pkg", "integer_of");
	}

	public void Emit_Value_To_Bool()
	{
		Emit_Method("numbers_pkg", "integer_of");
	}

	public void Emit_Random()
	{
		gen.Emit(OpCodes.Ldsfld, random_generator);
		gen.EmitCall(OpCodes.Callvirt, Get_Method("System.Random", "NextDouble"), null);
		Emit_Method("numbers_pkg", "make_value__2");
	}

	public void Emit_Random_2(double first, double last)
	{
		gen.Emit(OpCodes.Ldsfld, random_generator);
		gen.EmitCall(OpCodes.Callvirt, Get_Method("System.Random", "NextDouble"), null);
		gen.Emit(OpCodes.Ldc_R8, last - first);
		gen.Emit(OpCodes.Mul);
		gen.Emit(OpCodes.Ldc_R8, first);
		gen.Emit(OpCodes.Add);
		Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
		Type type = null;
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType("System.Math");
			if (type != null)
			{
				break;
			}
		}
		MethodInfo method = type.GetMethod("Floor", new Type[1] { Type.GetType("System.Double") });
		gen.EmitCall(OpCodes.Call, method, null);
		Emit_Method("numbers_pkg", "make_value__2");
	}

	public void Emit_And_Shortcut(boolean_parseable left, boolean2 right, bool left_negated)
	{
		Label label = gen.DefineLabel();
		left.emit_code(this, 0);
		if (left_negated)
		{
			Emit_Not();
		}
		gen.Emit(OpCodes.Dup);
		gen.Emit(OpCodes.Brfalse, label);
		right.emit_code(this, 0);
		Emit_And();
		gen.MarkLabel(label);
	}

	public void Emit_Or_Shortcut(boolean2 left, boolean_expression right)
	{
		Label label = gen.DefineLabel();
		left.emit_code(this, 0);
		gen.Emit(OpCodes.Dup);
		gen.Emit(OpCodes.Brtrue, label);
		right.emit_code(this, 0);
		Emit_Or();
		gen.MarkLabel(label);
	}

	public void Emit_Is_Number(string name)
	{
		if (variables.ContainsKey(name.ToLower()))
		{
			LocalBuilder local = (LocalBuilder)variables[name.ToLower()];
			gen.Emit(OpCodes.Ldloc, local);
			Emit_Method("numbers_pkg", "is_number");
			return;
		}
		if (arrays.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		if (arrays_2d.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		throw new Exception(name + ": variable used before assigned");
	}

	public void Emit_Is_Character(string name)
	{
		if (variables.ContainsKey(name.ToLower()))
		{
			LocalBuilder local = (LocalBuilder)variables[name.ToLower()];
			gen.Emit(OpCodes.Ldloc, local);
			Emit_Method("numbers_pkg", "is_character");
			return;
		}
		if (arrays.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		if (arrays_2d.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		throw new Exception(name + ": variable used before assigned");
	}

	public void Emit_Is_String(string name)
	{
		if (variables.ContainsKey(name.ToLower()))
		{
			LocalBuilder local = (LocalBuilder)variables[name.ToLower()];
			gen.Emit(OpCodes.Ldloc, local);
			Emit_Method("numbers_pkg", "is_string");
			return;
		}
		if (arrays.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		if (arrays_2d.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		throw new Exception(name + ": variable used before assigned");
	}

	public void Emit_Is_Array(string name)
	{
		if (variables.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		if (arrays.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_1);
			return;
		}
		if (arrays_2d.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		throw new Exception(name + ": variable used before assigned");
	}

	public void Emit_Is_Array2D(string name)
	{
		if (variables.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		if (arrays.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_0);
			return;
		}
		if (arrays_2d.ContainsKey(name.ToLower()))
		{
			gen.Emit(OpCodes.Ldc_I4_1);
			return;
		}
		throw new Exception(name + ": variable used before assigned");
	}

	public void Emit_Plugin_Call(string name, parameter_list parameters)
	{
		Plugins.Emit_Invoke_Function(name, parameters, this);
	}

	public void Emit_Times()
	{
		Emit_Method("numbers_pkg", "Omultiply");
	}

	public void Emit_Divide()
	{
		Emit_Method("numbers_pkg", "Odivide");
	}

	public void Emit_Plus()
	{
		Emit_Method("numbers_pkg", "Oadd");
	}

	public void Emit_Unary_Minus()
	{
		Emit_Method("numbers_pkg", "Osubtract");
	}

	public void Emit_Minus()
	{
		Emit_Method("numbers_pkg", "Osubtract__2");
	}

	public void Emit_Mod()
	{
		Emit_Method("numbers_pkg", "Omod");
	}

	public void Emit_Rem()
	{
		Emit_Method("numbers_pkg", "Orem");
	}

	public void Emit_Exponentiation()
	{
		Emit_Method("numbers_pkg", "Oexpon");
	}

	public void Start_Method(string name)
	{
		variables.Clear();
		arrays.Clear();
		arrays_2d.Clear();
		if (name != "Main")
		{
			pc = procedures[name.ToLower().Trim()];
			gen = pc.subMethodBuilder.GetILGenerator();
		}
		else
		{
			gen = myILGenerator;
			pc = new procedure_class("Main", new string[0], new bool[0], new bool[0]);
		}
	}

	public void Done_Variable_Declarations()
	{
		ILGenerator iLGenerator = gen;
		for (int i = 0; i < pc.args.Length; i++)
		{
			string text = pc.args[i].ToLower();
			if (arrays.ContainsKey(text))
			{
				LocalBuilder local = (LocalBuilder)arrays[text];
				iLGenerator.Emit(OpCodes.Ldarg, i);
				iLGenerator.Emit(OpCodes.Castclass, typeof(Value_Array));
				iLGenerator.Emit(OpCodes.Stloc, local);
			}
			else if (arrays_2d.ContainsKey(text))
			{
				LocalBuilder local = (LocalBuilder)arrays_2d[text];
				iLGenerator.Emit(OpCodes.Ldarg, i);
				iLGenerator.Emit(OpCodes.Castclass, typeof(Value_2D_Array));
				iLGenerator.Emit(OpCodes.Stloc, local);
			}
			else if (variables.ContainsKey(text))
			{
				if (pc.arg_is_input[i])
				{
					LocalBuilder local = (LocalBuilder)variables[text];
					iLGenerator.Emit(OpCodes.Ldarg_S, i);
					iLGenerator.Emit(OpCodes.Castclass, typeof(value));
					iLGenerator.Emit(OpCodes.Ldloc, local);
					iLGenerator.Emit(OpCodes.Castclass, typeof(value));
					Emit_Method("numbers_pkg", "copy");
				}
			}
			else
			{
				LocalBuilder local = iLGenerator.DeclareLocal(typeof(object));
				variables.Add(text.ToLower(), local);
				iLGenerator.Emit(OpCodes.Ldarg_S, i);
				iLGenerator.Emit(OpCodes.Stloc, local);
			}
		}
	}

	public void Done_Method()
	{
		ILGenerator iLGenerator = gen;
		for (int i = 0; i < pc.args.Length; i++)
		{
			string key = pc.args[i].ToLower();
			if (pc.arg_is_output[i] && variables.ContainsKey(key))
			{
				LocalBuilder localBuilder = (LocalBuilder)variables[key];
				if (localBuilder.LocalType == typeof(value))
				{
					iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
					iLGenerator.Emit(OpCodes.Ldarg, i);
					iLGenerator.Emit(OpCodes.Castclass, typeof(value));
					Emit_Method("numbers_pkg", "copy");
				}
			}
		}
		iLGenerator.Emit(OpCodes.Ret);
	}

	public object Emit_Call_Method(int name)
	{
		return new method_call(name);
	}

	public void Emit_Next_Parameter(object o)
	{
		if (o is method_call)
		{
			((method_call)o).param_count++;
		}
	}

	public void Emit_Last_Parameter(object o)
	{
		if (o is method_call)
		{
			method_call method_call = o as method_call;
			method_call.param_count++;
			parse_tree_pkg.emit_method_call_il(method_call.name, method_call.param_count, this);
		}
		else if (o is subchart_call)
		{
			subchart_call subchart_call = o as subchart_call;
			gen.EmitCall(OpCodes.Call, subchart_call.proc.subMethodBuilder, null);
		}
	}

	public void Emit_No_Parameters(object o)
	{
		method_call method_call = o as method_call;
		parse_tree_pkg.emit_method_call_il(method_call.name, method_call.param_count, this);
	}

	public object Emit_Call_Subchart(string name)
	{
		return new subchart_call(procedures[name.Trim().ToLower()]);
	}

	public object If_Start()
	{
		return new label_pair(gen);
	}

	public void If_Then_Part(object o)
	{
		gen.Emit(OpCodes.Brfalse, ((label_pair)o).l2);
	}

	public void If_Else_Part(object o)
	{
		gen.Emit(OpCodes.Br, ((label_pair)o).l3);
		gen.MarkLabel(((label_pair)o).l2);
	}

	public void If_Done(object o)
	{
		gen.MarkLabel(((label_pair)o).l3);
	}

	public object Loop_Start(bool is_while, bool is_negated)
	{
		label_pair label_pair = new label_pair(gen);
		gen.MarkLabel(label_pair.l2);
		loop_vars loop_vars = default(loop_vars);
		loop_vars.lp = label_pair;
		loop_vars.is_negated = is_negated;
		return loop_vars;
	}

	public void Loop_Start_Condition(object o)
	{
	}

	public void Loop_End_Condition(object o)
	{
		if (((loop_vars)o).is_negated)
		{
			gen.Emit(OpCodes.Brfalse, ((loop_vars)o).lp.l3);
		}
		else
		{
			gen.Emit(OpCodes.Brtrue, ((loop_vars)o).lp.l3);
		}
	}

	public void Loop_End(object o)
	{
		gen.Emit(OpCodes.Br, ((loop_vars)o).lp.l2);
		gen.MarkLabel(((loop_vars)o).lp.l3);
	}

	public void Emit_Left_Paren()
	{
	}

	public void Emit_Right_Paren()
	{
	}

	public void Array_2D_Assignment_Start(string name)
	{
		name_array_2d = name;
		Emit_Load(name);
	}

	public void Array_2D_Assignment_Between_Indices()
	{
	}

	public void Array_2D_Assignment_After_Indices()
	{
	}

	public void Array_2D_Assignment_PastRHS()
	{
		Emit_Assign_To_Array_2D(name_array_2d);
	}

	public void Array_1D_Assignment_Start(string name)
	{
		name_array_1d = name;
		Emit_Load(name);
	}

	public void Array_1D_Assignment_After_Index()
	{
	}

	public void Array_1D_Assignment_PastRHS()
	{
		Emit_Assign_To_Array(name_array_1d);
	}

	public void Variable_Assignment_Start(string name)
	{
		name_variable = name;
	}

	public void Variable_Assignment_PastRHS()
	{
		if (name_variable == "raptor_prompt_variable_zzyz")
		{
			Emit_Method("numbers_pkg", "string_of");
			LocalBuilder local = (LocalBuilder)variables[name_variable.ToLower()];
			gen.Emit(OpCodes.Stloc, local);
		}
		else
		{
			Emit_Assign_To(name_variable);
		}
	}

	public void Input_Start_Variable(string name)
	{
		dest_is_array = false;
		Variable_Assignment_Start(name);
		kind_of_input = input_kind.variable;
	}

	public void Input_Start_Array_1D(string name, expression reference)
	{
		dest_is_array = true;
		Array_1D_Assignment_Start(name);
		reference.emit_code(this, 0);
		Array_1D_Assignment_After_Index();
		kind_of_input = input_kind.array;
	}

	public void Input_Start_Array_2D(string name, expression reference, expression reference2)
	{
		dest_is_array = true;
		Array_2D_Assignment_Start(name);
		reference.emit_code(this, 0);
		Array_2D_Assignment_Between_Indices();
		reference2.emit_code(this, 0);
		Array_2D_Assignment_After_Indices();
		kind_of_input = input_kind.array2d;
	}

	public void Input_Past_Prompt()
	{
		Emit_Load_Boolean(dest_is_array);
		Emit_Method("ada_runtime_pkg", "prompt_dialog");
		switch (kind_of_input)
		{
		case input_kind.variable:
			Variable_Assignment_PastRHS();
			break;
		case input_kind.array2d:
			Array_2D_Assignment_PastRHS();
			break;
		case input_kind.array:
			Array_1D_Assignment_PastRHS();
			break;
		}
	}

	public void Output_Start(bool has_newline, bool is_string)
	{
	}

	public void Output_Past_Expr(bool has_newline, bool is_string)
	{
		if (!is_string)
		{
			Emit_Method("numbers_pkg", "msstring_image");
		}
		if (has_newline)
		{
			Emit_Method("raptor.Runtime", "consoleWriteln");
		}
		else
		{
			Emit_Method("raptor.Runtime", "consoleWrite");
		}
	}
}
