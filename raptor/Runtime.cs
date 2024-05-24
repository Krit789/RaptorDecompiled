#define TRACE
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using dotnetgraphlibrary;
using NClass.Core;
using numbers;
using PlaySound;

namespace raptor;

public class Runtime
{
	public enum Parameter_Kind
	{
		Value,
		One_D_Array,
		Two_D_Array
	}

	public enum Variable_Kind
	{
		Value,
		Class_Value,
		One_D_Array,
		Two_D_Array,
		Heap_Object,
		Scope
	}

	private delegate void Clear_Updated_Delegate_Type();

	private delegate void Parent_Focus_Delegate_Type(Visual_Flow_Form form);

	private delegate void Add_To_Updated_Delegate_Type(Variable temp);

	public delegate void Add_Node_Delegate(TreeNode node);

	public delegate void Delete_Scope_Delegate();

	internal class Variable : TreeNode
	{
		public string Var_Name;

		public Variable_Kind Kind;

		public value Variable_Value;

		private bool isConstant;

		private ClassType theClass;

		internal ClassType getClass()
		{
			return theClass;
		}

		public Variable(string name)
		{
			Var_Name = name;
			Kind = Variable_Kind.Scope;
			base.Text = "--" + name + "--";
		}

		public Variable(string name, value Value)
		{
			Var_Name = name;
			Kind = Variable_Kind.Value;
			Variable_Value = Value;
			base.Text = name + ": " + numbers_pkg.msstring_view_image(Value);
			Add_To_Updated(this);
		}

		public Variable(ClassType ct)
		{
			Var_Name = ct.Name;
			Kind = Variable_Kind.Class_Value;
			Variable_Value = numbers_pkg.make_object_value(this);
			theClass = ct;
			base.Text = "<" + ct.Name + "> : static vars";
			isConstant = true;
			foreach (Field field in ct.Fields)
			{
				if (field.IsStatic)
				{
					addField(field);
				}
			}
		}

		private void addField(Field f)
		{
			if (f.Type.Contains("[][]"))
			{
				base.Nodes.Add(new Variable(f.Name, 1, 1, numbers_pkg.zero));
				return;
			}
			if (f.Type.Contains("[]"))
			{
				base.Nodes.Add(new Variable(f.Name, 1, numbers_pkg.zero));
				return;
			}
			Variable variable = new Variable(Value: (f.InitialValue == null || !(f.InitialValue != "")) ? numbers_pkg.zero : ((!f.InitialValue.Contains("\"")) ? numbers_pkg.make_number_value(f.InitialValue) : numbers_pkg.make_string_value(f.InitialValue)), name: f.Name);
			base.Nodes.Add(variable);
			if (f.IsConstant || f.IsReadonly)
			{
				variable.setConstant();
			}
		}

		public Variable(string name, string class_name)
		{
			Var_Name = name;
			Kind = Variable_Kind.Heap_Object;
			Variable_Value = numbers_pkg.make_object_value(this);
			base.Text = "[" + GetHashCode() + "] : <" + class_name + ">";
			if (Component.Current_Mode != Mode.Expert)
			{
				throw new Exception("can't create object unless in OO mode");
			}
			theClass = parent.projectCore.findClass(class_name);
			if (theClass == null)
			{
				throw new Exception("can't create object of class: " + class_name);
			}
			if (theClass.Modifier == ClassModifier.Abstract)
			{
				throw new Exception("can't create object of abstract class: " + class_name);
			}
			for (ClassType baseClass = theClass; baseClass != null; baseClass = baseClass.BaseClass)
			{
				foreach (Field field in baseClass.Fields)
				{
					if (!field.IsStatic)
					{
						addField(field);
					}
				}
			}
			Add_To_Updated(this);
		}

		public Variable(string name, int index, value Value)
		{
			if (index > 10000)
			{
				throw new Exception("array index " + index + " too large.");
			}
			Var_Name = name;
			Kind = Variable_Kind.One_D_Array;
			base.Nodes.Add(new Variable("Size", numbers_pkg.make_value__3(index)));
			if (numbers_pkg.is_string(Value))
			{
				for (int i = 1; i < index; i++)
				{
					base.Nodes.Add(new Variable("<" + i + ">", numbers_pkg.make_string_value("")));
				}
			}
			else
			{
				for (int j = 1; j < index; j++)
				{
					base.Nodes.Add(new Variable("<" + j + ">", numbers_pkg.make_value__2(0.0)));
				}
			}
			base.Nodes.Add(new Variable("<" + index + ">", Value));
			base.Text = name + "[]";
			Add_To_Updated(this);
		}

		public Variable(string name, int index1, int index2, value Value)
		{
			if (index1 * index2 > 10000)
			{
				throw new Exception("array indices " + index1 + "," + index2 + " too large.");
			}
			Var_Name = name;
			Kind = Variable_Kind.Two_D_Array;
			base.Nodes.Add(new Variable("Rows", numbers_pkg.make_value__3(index1)));
			if (numbers_pkg.is_string(Value))
			{
				for (int i = 1; i < index1; i++)
				{
					base.Nodes.Add(new Variable("<" + i + ">", index2, numbers_pkg.make_string_value("")));
				}
			}
			else
			{
				for (int j = 1; j < index1; j++)
				{
					base.Nodes.Add(new Variable("<" + j + ">", index2, numbers_pkg.make_value__2(0.0)));
				}
			}
			base.Nodes.Add(new Variable("<" + index1 + ">", index2, Value));
			base.Text = name + "[,]";
			Add_To_Updated(this);
		}

		public void Add_Rows(int current_count, int new_count, int col_count)
		{
			base.FirstNode.Text = "Rows: " + new_count;
			((Variable)base.FirstNode).Variable_Value = numbers_pkg.make_value__3(new_count);
			Add_To_Updated((Variable)base.FirstNode);
			for (int i = current_count + 1; i <= new_count; i++)
			{
				base.Nodes.Add(new Variable("<" + i + ">", col_count, numbers_pkg.make_value__2(0.0)));
			}
		}

		public void Add_Cols(int current_count, int new_count, int row_count)
		{
			for (int i = 1; i <= row_count; i++)
			{
				base.Nodes[i].FirstNode.Text = "Size: " + new_count;
				((Variable)base.Nodes[i].FirstNode).Variable_Value = numbers_pkg.make_value__3(new_count);
				Add_To_Updated((Variable)base.Nodes[i].FirstNode);
				for (int j = current_count + 1; j <= new_count; j++)
				{
					base.Nodes[i].Nodes.Add(new Variable("<" + j + ">", numbers_pkg.make_value__2(0.0)));
				}
			}
		}

		public void setConstant()
		{
			isConstant = true;
		}

		public void set_value(value v)
		{
			if (!isConstant)
			{
				Variable_Value = v;
				base.Text = Var_Name + ": " + numbers_pkg.msstring_view_image(v);
				Add_To_Updated(this);
				return;
			}
			throw new Exception("can't assign to constant variable: " + Var_Name);
		}

		public void Extend_1D(int index)
		{
			int num = numbers_pkg.integer_of(((Variable)base.FirstNode).Variable_Value);
			if (index > num)
			{
				((Variable)base.FirstNode).Variable_Value = numbers_pkg.make_value__3(index);
				base.FirstNode.Text = "Size: " + index;
				Add_To_Updated((Variable)base.FirstNode);
				for (int i = num + 1; i <= index; i++)
				{
					base.Nodes.Add(new Variable("<" + i + ">", numbers_pkg.make_value__2(0.0)));
				}
			}
		}

		public Variable get_field_variable(string name)
		{
			foreach (TreeNode node in base.Nodes)
			{
				if (node.Name == name)
				{
					return node as Variable;
				}
			}
			return null;
		}

		public value get_field_1d(string name, int index)
		{
			Variable variable = get_field_variable(name);
			if (variable.Kind != Variable_Kind.One_D_Array)
			{
				throw new Exception("field " + name + " is not a 1D array");
			}
			return variable.getArrayElement(index);
		}

		public value get_field_2d(string name, int index, int index2)
		{
			Variable variable = get_field_variable(name);
			if (variable.Kind != Variable_Kind.Two_D_Array)
			{
				throw new Exception("field " + name + " is not a 2D array");
			}
			return variable.get2DArrayElement(index, index2);
		}

		public value get_field(string name)
		{
			foreach (TreeNode node in base.Nodes)
			{
				if (node.Name == name)
				{
					if ((node as Variable).Kind == Variable_Kind.Value || (node as Variable).Kind == Variable_Kind.Heap_Object)
					{
						return (node as Variable).Variable_Value;
					}
					throw new Exception("field " + name + " is " + (node as Variable).Kind);
				}
			}
			throw new Exception("object doesn't have field: " + name);
		}

		public void set_field(string name, value f)
		{
			if (Kind != Variable_Kind.Heap_Object)
			{
				throw new Exception("can't set field: " + name + "-- not an object.");
			}
			Variable obj = get_field_variable(name) ?? throw new Exception("can't set field: " + name + "-- no such field.");
			obj.Text = name + ": " + numbers_pkg.msstring_view_image(f);
			obj.Variable_Value = f;
			Add_To_Updated(this);
			Add_To_Updated(obj);
		}

		public void set_1D_value(int index, value f)
		{
			if (index > 10000)
			{
				throw new Exception("array index " + index + " too large.");
			}
			if (Kind == Variable_Kind.Value && numbers_pkg.is_string(Variable_Value))
			{
				int num;
				if (numbers_pkg.is_number(f))
				{
					num = numbers_pkg.integer_of(f);
				}
				else if (numbers_pkg.is_character(f))
				{
					num = f.c;
				}
				else
				{
					if (!numbers_pkg.is_string(f) || numbers_pkg.length_of(f) != 1)
					{
						throw new Exception("Can't assign " + numbers_pkg.msstring_view_image(f) + " to position " + index);
					}
					num = f.s[0];
				}
				if (num < 0 || num > 65535)
				{
					throw new Exception("Character values only 0-65535, not " + num);
				}
				string text = ((index <= numbers_pkg.length_of(Variable_Value)) ? Variable_Value.s.Remove(index - 1, 1).Insert(index - 1, ((char)num).ToString() ?? "") : (Variable_Value.s + new string(' ', index - numbers_pkg.length_of(Variable_Value) - 1) + (char)num));
				Variable_Value = numbers_pkg.make_string_value(text);
				base.Text = Var_Name + ": \"" + text + "\"";
				Add_To_Updated(this);
			}
			else
			{
				if (Kind != Variable_Kind.One_D_Array)
				{
					throw new Exception(Var_Name + " is not a 1D array");
				}
				Extend_1D(index);
				base.Nodes[index].Text = "<" + index + ">: " + numbers_pkg.msstring_view_image(f);
				((Variable)base.Nodes[index]).Variable_Value = f;
				Add_To_Updated(this);
				Add_To_Updated((Variable)base.Nodes[index]);
			}
		}

		public void set_2D_value(int index1, int index2, value Value)
		{
			if (index1 * index2 > 10000)
			{
				throw new Exception("array indices " + index1 + "," + index2 + " too large.");
			}
			int num = numbers_pkg.integer_of(((Variable)base.FirstNode).Variable_Value);
			int num2 = numbers_pkg.integer_of(((Variable)base.Nodes[1].FirstNode).Variable_Value);
			if (index2 > num2)
			{
				Add_Cols(num2, index2, num);
				num2 = index2;
			}
			if (index1 > num)
			{
				Add_Rows(num, index1, num2);
			}
			base.Nodes[index1].Nodes[index2].Text = "<" + index2 + ">: " + numbers_pkg.msstring_view_image(Value);
			((Variable)base.Nodes[index1].Nodes[index2]).Variable_Value = Value;
			Add_To_Updated(this);
			Add_To_Updated((Variable)base.Nodes[index1]);
			Add_To_Updated((Variable)base.Nodes[index1].Nodes[index2]);
		}

		public value get2DArrayElement(int index1, int index2)
		{
			if (Kind == Variable_Kind.Two_D_Array)
			{
				int num = numbers_pkg.integer_of(((Variable)base.FirstNode).Variable_Value);
				int num2 = numbers_pkg.integer_of(((Variable)base.Nodes[1].FirstNode).Variable_Value);
				if (num >= index1)
				{
					if (num2 >= index2)
					{
						return ((Variable)base.Nodes[index1].Nodes[index2]).Variable_Value;
					}
					throw new Exception(Var_Name + " doesn't have " + index2 + " Columns.");
				}
				throw new Exception(Var_Name + " doesn't have " + index1 + " Rows.");
			}
			throw new Exception(Var_Name + " is not a two-dimensional array");
		}

		public value getArrayElement(int index)
		{
			if (Kind == Variable_Kind.One_D_Array)
			{
				if (numbers_pkg.integer_of(((Variable)base.FirstNode).Variable_Value) >= index)
				{
					return ((Variable)base.Nodes[index]).Variable_Value;
				}
				throw new Exception(Var_Name + " doesn't have " + index + " elements.");
			}
			if (Kind == Variable_Kind.Value && numbers_pkg.is_string(Variable_Value))
			{
				if (numbers_pkg.length_of(Variable_Value) >= index)
				{
					return numbers_pkg.make_value__4(Variable_Value.s[index - 1]);
				}
				throw new Exception(Var_Name + " doesn't have " + index + " elements.");
			}
			throw new Exception(Var_Name + " is not a one-dimensional array");
		}

		public int getArraySize()
		{
			if (Kind == Variable_Kind.One_D_Array)
			{
				return numbers_pkg.integer_of(((Variable)base.FirstNode).Variable_Value);
			}
			if (Kind == Variable_Kind.Value && numbers_pkg.is_string(Variable_Value))
			{
				return numbers_pkg.length_of(Variable_Value);
			}
			throw new Exception(Var_Name + " is not a 1D array.");
		}

		public int row_count()
		{
			return numbers_pkg.integer_of(((Variable)base.FirstNode).Variable_Value);
		}

		public int col_count()
		{
			return numbers_pkg.integer_of(((Variable)base.Nodes[1].FirstNode).Variable_Value);
		}

		public void Overwrite(Variable source)
		{
			if (Kind == source.Kind)
			{
				throw new Exception("can't overwrite " + base.Text + " with " + source.Text);
			}
			switch (Kind)
			{
			case Variable_Kind.One_D_Array:
			{
				Extend_1D(source.getArraySize());
				for (int k = 1; k < source.getArraySize(); k++)
				{
					if (getArrayElement(k) != source.getArrayElement(k))
					{
						set_1D_value(k, source.getArrayElement(k));
					}
				}
				break;
			}
			case Variable_Kind.Value:
				try
				{
					if (!numbers_pkg.Oeq(Variable_Value, source.Variable_Value))
					{
						set_value(source.Variable_Value);
					}
					break;
				}
				catch
				{
					set_value(source.Variable_Value);
					break;
				}
			case Variable_Kind.Two_D_Array:
			{
				if (row_count() != source.row_count())
				{
					Add_Rows(row_count(), source.row_count(), col_count());
				}
				if (col_count() != source.col_count())
				{
					Add_Cols(col_count(), source.col_count(), row_count());
				}
				for (int i = 1; i <= row_count(); i++)
				{
					for (int j = 1; j <= col_count(); j++)
					{
						if (get2DArrayElement(i, j) != source.get2DArrayElement(i, j))
						{
							set_2D_value(i, j, source.get2DArrayElement(i, j));
						}
					}
				}
				break;
			}
			case Variable_Kind.Class_Value:
				break;
			}
		}
	}

	private delegate value getVariable_Delegate_Type(string s);

	private delegate void setVariable_Delegate_Type(string s, value f);

	private delegate void setArrayElement_Delegate_Type(string s, int index, value f);

	private delegate void set2DArrayElement_Delegate_Type(string s, int index1, int index2, value f);

	private delegate void Clear_Delegate_Type();

	private delegate string Redirect_Input_Delegate_Type();

	private delegate string Redirect_Output_Delegate_Type();

	private const int max_array_size = 10000;

	public static bool processing_parameter_list = false;

	private static bool superContext = false;

	private static ArrayList updated_list = new ArrayList();

	public static value method_return_value = null;

	private static Clear_Updated_Delegate_Type clear_updated_delegate = Clear_Updated_Delegate;

	private static Parent_Focus_Delegate_Type parent_focus_delegate = Parent_Focus_Delegate;

	private static Add_To_Updated_Delegate_Type add_to_updated_delegate = Add_To_Updated_Delegate;

	public static Add_Node_Delegate add_delegate = Add_Node_To_WatchBox;

	public static Add_Node_Delegate add_at_front_delegate = Add_Node_To_Front_Of_WatchBox;

	public static Add_Node_Delegate add_to_heap_delegate = Add_Node_To_Heap;

	public static Add_Node_Delegate add_to_classes_delegate = Add_Node_To_Classes;

	public static Delete_Scope_Delegate delete_scope = Delete_Scope_From_WatchBox;

	public static Visual_Flow_Form parent;

	private static MasterConsole console;

	private static TreeView watchBox;

	private static getVariable_Delegate_Type getVariable_delegate = getVariable_Delegate;

	private static Variable context;

	private static setVariable_Delegate_Type setVariable_delegate = setVariable_Delegate;

	private static setArrayElement_Delegate_Type setArrayElement_delegate = setArrayElement_Delegate;

	private static set2DArrayElement_Delegate_Type set2DArrayElement_delegate = set2DArrayElement_Delegate;

	private static Clear_Delegate_Type clear_delegate = Clear_Delegate;

	private static Redirect_Input_Delegate_Type redirect_input_delegate = Redirect_Input_Static;

	private static Redirect_Output_Delegate_Type redirect_output_delegate = Redirect_Output_Static;

	private static bool am_appending = false;

	public static bool isObjectOriented()
	{
		return Component.Current_Mode == Mode.Expert;
	}

	private static void Clear_Updated_Delegate()
	{
		IEnumerator enumerator = updated_list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			((Variable)enumerator.Current).ForeColor = Color.Black;
		}
		updated_list.Clear();
	}

	private static void Parent_Focus_Delegate(Visual_Flow_Form form)
	{
		form.Focus();
	}

	public static void Clear_Updated()
	{
		watchBox.Invoke(clear_updated_delegate, null);
	}

	private static void Add_To_Updated_Delegate(Variable temp)
	{
		temp.ForeColor = Color.Red;
		updated_list.Add(temp);
	}

	private static void Add_To_Updated(Variable temp)
	{
		object[] args = new object[1] { temp };
		watchBox.Invoke(add_to_updated_delegate, args);
	}

	public static void Add_Node_To_WatchBox(TreeNode node)
	{
		for (int i = 0; i < watchBox.Nodes.Count; i++)
		{
			if ((string.Compare(watchBox.Nodes[i].Text, node.Text) > 0 && ((Variable)watchBox.Nodes[i]).Kind != Variable_Kind.Scope) || (((Variable)watchBox.Nodes[i]).Kind == Variable_Kind.Scope && i > 0) || (((Variable)watchBox.Nodes[i]).Kind == Variable_Kind.Scope && (watchBox.Nodes[i] as Variable).Var_Name.CompareTo("Classes") == 0) || (((Variable)watchBox.Nodes[i]).Kind == Variable_Kind.Scope && (watchBox.Nodes[i] as Variable).Var_Name.CompareTo("Heap") == 0))
			{
				watchBox.Nodes.Insert(i, node);
				return;
			}
		}
		watchBox.Nodes.Add(node);
	}

	public static void Add_Node_To_Front_Of_WatchBox(TreeNode node)
	{
		watchBox.Nodes.Insert(0, node);
	}

	public static int findClassesScope()
	{
		for (int i = 0; i < watchBox.Nodes.Count; i++)
		{
			if (((Variable)watchBox.Nodes[i]).Kind == Variable_Kind.Scope && (watchBox.Nodes[i] as Variable).Var_Name.CompareTo("Classes") == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static int findHeapScope()
	{
		for (int i = 0; i < watchBox.Nodes.Count; i++)
		{
			if (((Variable)watchBox.Nodes[i]).Kind == Variable_Kind.Scope && (watchBox.Nodes[i] as Variable).Var_Name.CompareTo("Heap") == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static void Add_Node_To_Heap(TreeNode node)
	{
		int num = findHeapScope();
		if (num == -1)
		{
			num = watchBox.Nodes.Add(new Variable("Heap"));
		}
		for (int i = num + 1; i < watchBox.Nodes.Count; i++)
		{
			if (node.GetHashCode() < numbers_pkg.object_of((watchBox.Nodes[i] as Variable).Variable_Value).GetHashCode())
			{
				watchBox.Nodes.Insert(i, node);
				return;
			}
		}
		watchBox.Nodes.Add(node);
	}

	public static void Add_Node_To_Classes(TreeNode node)
	{
		int num = findClassesScope();
		int num2 = findHeapScope();
		if (num2 == -1)
		{
			num2 = watchBox.Nodes.Count;
		}
		if (num == -1)
		{
			num = watchBox.Nodes.Add(new Variable("Classes"));
		}
		for (int i = num + 1; i < num2; i++)
		{
			if ((node as Variable).Var_Name.CompareTo((watchBox.Nodes[i] as Variable).Var_Name) < 0)
			{
				watchBox.Nodes.Insert(i, node);
				return;
			}
		}
		watchBox.Nodes.Add(node);
	}

	public static void Delete_Scope_From_WatchBox()
	{
		watchBox.Nodes.RemoveAt(0);
		while (((Variable)watchBox.Nodes[0]).Kind != Variable_Kind.Scope)
		{
			watchBox.Nodes.RemoveAt(0);
		}
		Clear_Updated_Delegate();
		Trace.WriteLine(watchBox.Nodes.Count);
		watchBox.Invalidate();
	}

	public static void Init(Visual_Flow_Form p, MasterConsole MC, TreeView watch)
	{
		parent = p;
		console = MC;
		watchBox = watch;
		Clear_Variables();
		dotnetgraph.Init(p);
	}

	public static int Count_Symbols(Oval start)
	{
		return start.Count_Symbols();
	}

	public static void updateWatchBox()
	{
	}

	private static value getVariable_Delegate(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Value || variable.Kind == Variable_Kind.Class_Value)
			{
				return variable.Variable_Value;
			}
			if (variable.Kind == Variable_Kind.One_D_Array && !processing_parameter_list)
			{
				throw new Exception("1D Array " + s + " must be indexed here (e.g. " + s + "[3]).");
			}
			if (variable.Kind == Variable_Kind.Two_D_Array && !processing_parameter_list)
			{
				throw new Exception("2D Array " + s + " must be indexed here (e.g. " + s + "[3,3]).");
			}
			if (variable.Kind == Variable_Kind.One_D_Array && processing_parameter_list)
			{
				return numbers_pkg.make_1d_ref(variable);
			}
			if (variable.Kind == Variable_Kind.Two_D_Array && processing_parameter_list)
			{
				return numbers_pkg.make_2d_ref(variable);
			}
		}
		if (variable != null && variable.Kind == Variable_Kind.Scope && Component.Current_Mode == Mode.Expert)
		{
			return findStaticClass(s).Variable_Value;
		}
		throw new Exception("Variable " + s + " not found!");
	}

	public static value getVariable(string s)
	{
		object[] args = new object[1] { s };
		return (value)watchBox.Invoke(getVariable_delegate, args);
	}

	public static object getVariableContext(string s)
	{
		object[] args = new object[1] { s };
		return numbers_pkg.object_of((value)watchBox.Invoke(getVariable_delegate, args));
	}

	public static void setContext(object c)
	{
		context = c as Variable;
	}

	public static object getContextObject()
	{
		return context;
	}

	internal static Variable getContext()
	{
		return context;
	}

	private static Variable findStaticClass(string s)
	{
		int num = findClassesScope();
		if (num < 0)
		{
			return null;
		}
		for (int i = num + 1; i < watchBox.Nodes.Count; i++)
		{
			Variable variable = (Variable)watchBox.Nodes[i];
			if (variable.Var_Name.ToLower() == s.ToLower())
			{
				return variable;
			}
			if (variable.Kind == Variable_Kind.Scope)
			{
				return null;
			}
		}
		return null;
	}

	private static Variable Lookup_Variable(string s)
	{
		if (s.ToLower().Equals("super"))
		{
			Variable result = Lookup_Variable("this");
			superContext = true;
			return result;
		}
		superContext = false;
		if (context == null)
		{
			for (int i = 0; i < watchBox.Nodes.Count; i++)
			{
				Variable result = (Variable)watchBox.Nodes[i];
				if (result.Var_Name.ToLower() == s.ToLower())
				{
					return result;
				}
				if (result.Kind == Variable_Kind.Scope && (i > 0 || result.Var_Name == "Classes"))
				{
					if (Component.Current_Mode == Mode.Expert)
					{
						return findStaticClass(s);
					}
					return null;
				}
			}
			return null;
		}
		foreach (TreeNode node in context.Nodes)
		{
			if ((node as Variable).Var_Name.ToLower() == s.ToLower())
			{
				return node as Variable;
			}
		}
		throw new Exception(s + ": not found!");
	}

	private static void setVariable_Delegate(string s, value f)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Value || variable.Kind == Variable_Kind.Heap_Object)
			{
				variable.set_value(f);
				return;
			}
			if (variable.Kind == Variable_Kind.Class_Value)
			{
				throw new Exception("Invalid assignment to class " + s);
			}
			if (variable.Kind == Variable_Kind.One_D_Array)
			{
				throw new Exception("Invalid assignment to array " + s + " -- missing index (e.g. " + s + "[3]).");
			}
			if (variable.Kind == Variable_Kind.Two_D_Array)
			{
				throw new Exception("Invalid assignment to 2D array " + s + " -- missing indices (e.g. " + s + "[3,3]).");
			}
		}
		else
		{
			variable = new Variable(s.ToLower(), f);
			object[] args = new object[1] { variable };
			watchBox.Invoke(add_delegate, args);
		}
	}

	public static void setVariable(string s, value f)
	{
		object[] args = new object[2] { s, f };
		if (parent.runningState || Component.compiled_flowchart || Component.run_compiled_flowchart)
		{
			watchBox.Invoke(setVariable_delegate, args);
		}
	}

	internal static value[] getValueArray(Variable temp)
	{
		int num = numbers_pkg.integer_of(((Variable)temp.FirstNode).Variable_Value);
		value[] array = new value[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = temp.getArrayElement(i + 1);
		}
		return array;
	}

	public static value[] getValueArray(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.One_D_Array)
			{
				return getValueArray(variable);
			}
			throw new Exception(s + " is not a one-dimensional array");
		}
		throw new Exception(s + " not found.");
	}

	public static double[] getArray(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.One_D_Array)
			{
				int num = numbers_pkg.integer_of(((Variable)variable.FirstNode).Variable_Value);
				double[] array = new double[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = numbers_pkg.long_float_of(variable.getArrayElement(i + 1));
				}
				return array;
			}
			throw new Exception(s + " is not a one-dimensional array");
		}
		throw new Exception(s + " not found.");
	}

	public static int[] getIntArray(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.One_D_Array)
			{
				int num = numbers_pkg.integer_of(((Variable)variable.FirstNode).Variable_Value);
				int[] array = new int[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = (int)numbers_pkg.long_float_of(variable.getArrayElement(i + 1));
				}
				return array;
			}
			throw new Exception(s + " is not a one-dimensional array");
		}
		throw new Exception(s + " not found.");
	}

	public static object getVariableContext1D(string s, int index)
	{
		if (index <= 0)
		{
			throw new Exception("can't use " + index + " as an array index");
		}
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			return numbers_pkg.object_of(variable.getArrayElement(index));
		}
		throw new Exception(s + " not found.");
	}

	public static object getVariableContext2D(string s, int index1, int index2)
	{
		if (index1 <= 0)
		{
			throw new Exception("can't use " + index1 + " as an array index");
		}
		if (index2 <= 0)
		{
			throw new Exception("can't use " + index2 + " as an array index");
		}
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			return numbers_pkg.object_of(variable.get2DArrayElement(index1, index2));
		}
		throw new Exception(s + " not found.");
	}

	public static value getArrayElement(string s, int index)
	{
		if (index <= 0)
		{
			throw new Exception("can't use " + index + " as an array index");
		}
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			return variable.getArrayElement(index);
		}
		throw new Exception(s + " not found.");
	}

	public static double[][] get2DArray(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Two_D_Array)
			{
				int num = variable.row_count();
				int num2 = variable.col_count();
				double[][] array = new double[num][];
				for (int i = 0; i < num; i++)
				{
					array[i] = new double[num2];
					for (int j = 0; j < num2; j++)
					{
						array[i][j] = numbers_pkg.long_float_of(variable.get2DArrayElement(i + 1, j + 1));
					}
				}
				return array;
			}
			throw new Exception(s + " is not a two-dimensional array");
		}
		throw new Exception(s + " not found.");
	}

	public static value[][] get2DValueArray(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Two_D_Array)
			{
				return get2DValueArray(variable);
			}
			throw new Exception(s + " is not a two-dimensional array");
		}
		throw new Exception(s + " not found.");
	}

	internal static value[][] get2DValueArray(Variable temp)
	{
		int num = temp.row_count();
		int num2 = temp.col_count();
		value[][] array = new value[num][];
		for (int i = 0; i < num; i++)
		{
			array[i] = new value[num2];
			for (int j = 0; j < num2; j++)
			{
				array[i][j] = temp.get2DArrayElement(i + 1, j + 1);
			}
		}
		return array;
	}

	public static int[][] get2DIntArray(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Two_D_Array)
			{
				int num = variable.row_count();
				int num2 = variable.col_count();
				int[][] array = new int[num][];
				for (int i = 0; i < num; i++)
				{
					array[i] = new int[num2];
					for (int j = 0; j < num2; j++)
					{
						array[i][j] = numbers_pkg.integer_of(variable.get2DArrayElement(i + 1, j + 1));
					}
				}
				return array;
			}
			throw new Exception(s + " is not a two-dimensional array");
		}
		throw new Exception(s + " not found.");
	}

	public static value get2DArrayElement(string s, int index1, int index2)
	{
		if (index1 <= 0)
		{
			throw new Exception("can't use " + index1 + " as an array index");
		}
		if (index2 <= 0)
		{
			throw new Exception("can't use " + index2 + " as an array index");
		}
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			return variable.get2DArrayElement(index1, index2);
		}
		throw new Exception(s + " not found.");
	}

	public static int getArraySize(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			return variable.getArraySize();
		}
		throw new Exception(s + " not found.");
	}

	private static void setArrayElement_Delegate(string s, int index, value f)
	{
		if (index <= 0)
		{
			throw new Exception("can't use " + index + " as an array index");
		}
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			variable.set_1D_value(index, f);
			return;
		}
		variable = new Variable(s, index, f);
		try
		{
			object[] args = new object[1] { variable };
			watchBox.Invoke(add_delegate, args);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void setArrayElement(string s, int index, value f)
	{
		object[] args = new object[3] { s, index, f };
		watchBox.Invoke(setArrayElement_delegate, args);
	}

	private static void set2DArrayElement_Delegate(string s, int index1, int index2, value f)
	{
		if (index1 <= 0)
		{
			throw new Exception("can't use " + index1 + " as an array index");
		}
		if (index2 <= 0)
		{
			throw new Exception("can't use " + index2 + " as an array index");
		}
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Two_D_Array)
			{
				variable.set_2D_value(index1, index2, f);
				return;
			}
			throw new Exception(s + " is not a 2D array");
		}
		variable = new Variable(s, index1, index2, f);
		try
		{
			object[] args = new object[1] { variable };
			watchBox.Invoke(add_delegate, args);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void set2DArrayElement(string s, int index1, int index2, value f)
	{
		object[] args = new object[4] { s, index1, index2, f };
		watchBox.Invoke(set2DArrayElement_delegate, args);
	}

	public static void Increase_Scope(string s)
	{
		object[] array = new object[1];
		if (watchBox.Nodes.Count == 0 || ((Variable)watchBox.Nodes[0]).Kind != Variable_Kind.Scope || ((Variable)watchBox.Nodes[0]).Var_Name.CompareTo("Heap") == 0)
		{
			Variable variable = new Variable("main");
			array[0] = variable;
			watchBox.Invoke(add_at_front_delegate, array);
		}
		Variable variable2 = new Variable(s);
		array[0] = variable2;
		watchBox.Invoke(add_at_front_delegate, array);
	}

	public static void Decrease_Scope()
	{
		object[] args = new object[0];
		watchBox.Invoke(delete_scope, args);
	}

	private static Variable_Kind Kind_Of(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			return variable.Kind;
		}
		throw new Exception("not found");
	}

	public static bool isArray(string s)
	{
		try
		{
			return Kind_Of(s) == Variable_Kind.One_D_Array;
		}
		catch
		{
			return false;
		}
	}

	public static bool is_2D_Array(string s)
	{
		try
		{
			return Kind_Of(s) == Variable_Kind.Two_D_Array;
		}
		catch
		{
			return false;
		}
	}

	public static bool is_Value(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			return variable.Kind == Variable_Kind.Value;
		}
		return false;
	}

	public static bool is_Character(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Value)
			{
				return numbers_pkg.is_character(variable.Variable_Value);
			}
			return false;
		}
		return false;
	}

	public static bool is_String(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Value)
			{
				return numbers_pkg.is_string(variable.Variable_Value);
			}
			return false;
		}
		return false;
	}

	public static bool is_Scalar(string s)
	{
		Variable variable = Lookup_Variable(s);
		if (variable != null)
		{
			if (variable.Kind == Variable_Kind.Value)
			{
				return numbers_pkg.is_number(variable.Variable_Value);
			}
			return false;
		}
		return false;
	}

	public static bool is_Variable(string s)
	{
		try
		{
			Kind_Of(s);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static string promptDialog(string s)
	{
		if (parent != null && !parent.runningState && !Component.compiled_flowchart && !Component.run_compiled_flowchart && (Compile_Helpers.run_compiled_thread == null || Compile_Helpers.run_compiled_thread.ThreadState != 0))
		{
			return "0.0";
		}
		if (!raptor_files_pkg.input_redirected())
		{
			return new PromptForm(s, parent).Go();
		}
		return raptor_files_pkg.get_line();
	}

	public static void consoleMessage(string s)
	{
		console.set_text(s + "\n");
	}

	public static void ShowConsole()
	{
		Application.Run(console);
	}

	public static void consoleWrite(string s)
	{
		if (!raptor_files_pkg.output_redirected())
		{
			Create_Standalone_Console_if_needed();
			if (console != null)
			{
				console.set_text(s);
			}
			if (console == null || Visual_Flow_Form.command_line_run)
			{
				Console.Write(s);
			}
			if (parent != null)
			{
				Thread.Sleep(1);
				parent.Invoke(parent_focus_delegate, parent);
			}
		}
		else
		{
			raptor_files_pkg.write(s);
		}
	}

	public static void consoleWriteln(string s)
	{
		if (!raptor_files_pkg.output_redirected())
		{
			consoleWrite(s + "\n");
		}
		else
		{
			raptor_files_pkg.writeln(s);
		}
	}

	public static void consoleClear()
	{
		Create_Standalone_Console_if_needed();
		console.clear_txt();
	}

	private static void Create_Standalone_Console_if_needed()
	{
		if (console == null && !Visual_Flow_Form.command_line_run)
		{
			console = new MasterConsole(standalone: true);
			new Thread(ShowConsole).Start();
		}
		while (!console.Created)
		{
			Thread.Sleep(100);
		}
	}

	private static void Clear_Delegate()
	{
		watchBox.Nodes.Clear();
		watchBox.Update();
		if (dotnetgraph.Is_Open())
		{
			dotnetgraph.Close_Graph_Window_Force();
		}
	}

	public static void Clear_Variables()
	{
		setContext(null);
		if (Compile_Helpers.run_compiled_thread != null && Compile_Helpers.run_compiled_thread.IsAlive)
		{
			Compile_Helpers.run_compiled_thread.Abort();
		}
		if (watchBox != null && watchBox.Created)
		{
			watchBox.Invoke(clear_delegate, null);
		}
		else if (dotnetgraph.Is_Open())
		{
			dotnetgraph.Close_Graph_Window_Force();
		}
		Sound.Play_Sound(null);
		raptor_files_pkg.close_files();
	}

	public static bool End_Of_Input()
	{
		return raptor_files_pkg.end_of_input();
	}

	private static string Redirect_Input_Static()
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
		openFileDialog.CheckFileExists = true;
		if (openFileDialog.ShowDialog() == DialogResult.Cancel)
		{
			return "";
		}
		return openFileDialog.FileName;
	}

	public static void Redirect_Input(int yes_or_no)
	{
		if (raptor_files_pkg.network_is_redirected || Visual_Flow_Form.command_line_input_redirect)
		{
			return;
		}
		if (yes_or_no == 0)
		{
			raptor_files_pkg.stop_redirect_input();
			return;
		}
		string text = (string)watchBox.Invoke(redirect_input_delegate, null);
		if (text == "" || text == null)
		{
			MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			Redirect_Input(text);
		}
	}

	public static void Redirect_Input(string filename)
	{
		if (!raptor_files_pkg.network_is_redirected && !Visual_Flow_Form.command_line_input_redirect)
		{
			raptor_files_pkg.redirect_input(filename);
		}
	}

	public static void createStaticVariables()
	{
		object[] array = new object[1];
		foreach (ClassTabPage allClass in parent.allClasses)
		{
			Variable variable = new Variable(parent.projectCore.findClass(allClass.Text));
			array[0] = variable;
			watchBox.Invoke(add_to_classes_delegate, array);
		}
	}

	public static value createObject(string class_name)
	{
		object[] array = new object[1];
		Variable variable = (Variable)(array[0] = new Variable("object_ref", class_name));
		watchBox.Invoke(add_to_heap_delegate, array);
		return variable.Variable_Value;
	}

	private static string Redirect_Output_Static()
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Reset();
		saveFileDialog.OverwritePrompt = false;
		saveFileDialog.CheckFileExists = false;
		saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
		saveFileDialog.DefaultExt = ".txt";
		if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
		{
			return "";
		}
		return saveFileDialog.FileName;
	}

	public static void Redirect_Output_Append(int yes_or_no)
	{
		am_appending = true;
		Redirect_Output(yes_or_no);
	}

	public static void Redirect_Output(int yes_or_no)
	{
		if (raptor_files_pkg.network_is_redirected || Visual_Flow_Form.command_line_output_redirect)
		{
			return;
		}
		if (yes_or_no == 0)
		{
			raptor_files_pkg.stop_redirect_output();
			return;
		}
		string text = (string)watchBox.Invoke(redirect_input_delegate, null);
		if (text == "" || text == null)
		{
			MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			Redirect_Output(text);
		}
	}

	public static void Redirect_Output_Append(string filename)
	{
		am_appending = true;
		Redirect_Output(filename);
	}

	public static void Redirect_Output(string filename)
	{
		bool append = am_appending;
		am_appending = false;
		if (!raptor_files_pkg.network_is_redirected && !Visual_Flow_Form.command_line_output_redirect)
		{
			raptor_files_pkg.redirect_output(filename, append);
		}
	}

	public static void Set_Running(Procedure_Chart sc)
	{
		parent.running_tab = sc;
	}

	public static Procedure_Chart Find_Method_Set_Running(string methodname, int num_params)
	{
		ClassType classType = context.getClass();
		if (superContext)
		{
			classType = classType.BaseClass;
		}
		ClassType classType2 = classType;
		while (classType2.BaseClass != null)
		{
			foreach (TabPage tabPage in (classType2.raptorTab as ClassTabPage).tabControl1.TabPages)
			{
				if (tabPage.Text.ToLower() == methodname.ToLower() && (tabPage as Procedure_Chart).num_params == num_params)
				{
					Set_Running(tabPage as Procedure_Chart);
					return tabPage as Procedure_Chart;
				}
			}
			classType2 = classType2.BaseClass;
		}
		throw new Exception("can't find method " + methodname + " with " + num_params + " parameters in class " + classType.Name + ".");
	}
}
