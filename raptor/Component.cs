using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using generate_interface;
using interpreter;
using numbers;
using parse_tree;

namespace raptor;

public abstract class Component : ISerializable
{
	[Serializable]
	public class FootPrint
	{
		public int left;

		public int right;

		public int height;

		public FootPrint Clone()
		{
			return (FootPrint)MemberwiseClone();
		}
	}

	private static bool MONO_INITIALIZED = false;

	private static bool MONO_VALUE = false;

	internal CommentBox My_Comment;

	public static Mode Current_Mode = Mode.Novice;

	public static bool USMA_mode = false;

	public static bool _reverse_loop_logic = false;

	public static bool BARTPE = false;

	public static string BARTPE_ramdrive_path = "x:\\";

	public static string BARTPE_partition_path = "y:\\";

	public static bool VM = true;

	public static bool negate_loops = false;

	public static bool compiled_flowchart = false;

	public static bool run_compiled_flowchart = false;

	public static bool warned_about_newer_version = false;

	public static bool warned_about_error = false;

	internal int incoming_serialization_version;

	internal static int last_incoming_serialization_version;

	protected bool has_breakpoint;

	public bool is_child;

	public bool is_beforeChild;

	public bool is_afterChild;

	public bool my_selected;

	public static bool text_visible = true;

	public static bool full_text = true;

	public static bool view_comments = true;

	public static bool Inside_Print = false;

	public static bool Just_After_Print = false;

	public int height_of_text;

	public int width_of_text;

	public int char_length;

	internal System.Drawing.Rectangle rect;

	public ArrayList method_expressions = new ArrayList();

	public ArrayList values = new ArrayList();

	public int number_method_expressions_run;

	internal bool need_to_decrease_scope;

	public bool running;

	public float scale = 1f;

	public int head_height;

	public int head_width;

	public int head_heightOrig;

	public int head_widthOrig;

	public int connector_length;

	public int x_location;

	public int y_location;

	public Component Successor;

	public Component parent;

	public FootPrint FP;

	public string text_str = "";

	public string name = "";

	public int proximity = 10;

	public parseable parse_tree;

	public syntax_result result;

	protected int drawing_text_width;

	public Guid created_guid;

	public Guid changed_guid;

	public static bool MONO
	{
		get
		{
			if (!MONO_INITIALIZED)
			{
				MONO_INITIALIZED = true;
				if (Type.GetType("Mono.Runtime") != null)
				{
					MONO_VALUE = true;
				}
				else
				{
					MONO_VALUE = false;
				}
			}
			return MONO_VALUE;
		}
	}

	public static char assignmentSymbol
	{
		get
		{
			if (MONO)
			{
				return '=';
			}
			if (BARTPE)
			{
				return '«';
			}
			return '←';
		}
	}

	public static bool reverse_loop_logic
	{
		get
		{
			if (!_reverse_loop_logic)
			{
				return Current_Mode == Mode.Expert;
			}
			return true;
		}
		set
		{
			if (Current_Mode != Mode.Expert)
			{
				_reverse_loop_logic = value;
			}
		}
	}

	public static int current_serialization_version
	{
		get
		{
			if (Current_Mode == Mode.Expert)
			{
				return 18;
			}
			return 17;
		}
	}

	public bool selected
	{
		get
		{
			return my_selected;
		}
		set
		{
			my_selected = value;
		}
	}

	public int X
	{
		get
		{
			return x_location;
		}
		set
		{
			x_location = value;
		}
	}

	public int Y
	{
		get
		{
			return y_location;
		}
		set
		{
			y_location = value;
		}
	}

	public int H
	{
		get
		{
			return head_height;
		}
		set
		{
			head_height = value;
		}
	}

	public int W
	{
		get
		{
			return head_width;
		}
		set
		{
			head_width = value;
		}
	}

	public string Text
	{
		get
		{
			return text_str;
		}
		set
		{
			text_str = value;
		}
	}

	public int CL
	{
		get
		{
			connector_length = H / 4;
			return connector_length;
		}
	}

	public string Name => name;

	public int addExpression(object e)
	{
		return method_expressions.Add(e);
	}

	internal void addValue(value v)
	{
		values.Add(v);
	}

	internal value getValue(int i)
	{
		if (values[i] == null)
		{
			throw new Exception("method failed to return a value");
		}
		return values[i] as value;
	}

	public virtual void reset_this_method_expressions_run()
	{
		Runtime.setContext(null);
		number_method_expressions_run = 0;
		need_to_decrease_scope = false;
		values.Clear();
	}

	public virtual void reset_number_method_expressions_run()
	{
		reset_this_method_expressions_run();
		if (Successor != null)
		{
			Successor.reset_number_method_expressions_run();
		}
	}

	public Component(int h, int w, string str_name)
	{
		head_height = h;
		head_width = w;
		head_heightOrig = h;
		head_widthOrig = w;
		name = str_name;
		FP = new FootPrint();
		created_guid = Guid.NewGuid();
		changed_guid = created_guid;
	}

	public Component(Component S, int h, int w, string str_name)
	{
		Successor = S;
		head_height = h;
		head_width = w;
		head_heightOrig = h;
		head_widthOrig = w;
		name = str_name;
		FP = new FootPrint();
		created_guid = Guid.NewGuid();
		changed_guid = created_guid;
	}

	public Component(SerializationInfo info, StreamingContext ctxt)
	{
		incoming_serialization_version = (int)info.GetValue("_serialization_version", typeof(int));
		FP = (FootPrint)info.GetValue("_FP", typeof(FootPrint));
		text_str = (string)info.GetValue("_text_str", typeof(string));
		name = (string)info.GetValue("_name", typeof(string));
		proximity = (int)info.GetValue("_proximity", typeof(int));
		running = false;
		head_height = (int)info.GetValue("_head_height", typeof(int));
		head_width = (int)info.GetValue("_head_width", typeof(int));
		head_heightOrig = (int)info.GetValue("_head_heightOrig", typeof(int));
		head_widthOrig = (int)info.GetValue("_head_widthOrig", typeof(int));
		connector_length = (int)info.GetValue("_connector_length", typeof(int));
		x_location = (int)info.GetValue("_x_location", typeof(int));
		y_location = (int)info.GetValue("_y_location", typeof(int));
		height_of_text = (int)info.GetValue("_height_of_text", typeof(int));
		char_length = (int)info.GetValue("_char_length", typeof(int));
		Successor = (Component)info.GetValue("_Successor", typeof(Component));
		parent = (Component)info.GetValue("_parent", typeof(Component));
		is_child = (bool)info.GetValue("_is_child", typeof(bool));
		is_beforeChild = (bool)info.GetValue("_is_beforeChild", typeof(bool));
		is_afterChild = (bool)info.GetValue("_is_afterChild", typeof(bool));
		my_selected = false;
		full_text = (bool)info.GetValue("_full_text", typeof(bool));
		try
		{
			rect = (System.Drawing.Rectangle)info.GetValue("_rect", typeof(System.Drawing.Rectangle));
		}
		catch
		{
			
		}
		if (incoming_serialization_version >= 3)
		{
			My_Comment = (CommentBox)info.GetValue("_comment", typeof(CommentBox));
			if (My_Comment != null)
			{
				My_Comment.parent = this;
			}
		}
		if (incoming_serialization_version >= 12)
		{
			created_guid = (Guid)info.GetValue("_created_guid", typeof(Guid));
			changed_guid = (Guid)info.GetValue("_changed_guid", typeof(Guid));
		}
		else
		{
			created_guid = Guid.NewGuid();
			changed_guid = created_guid;
		}
		if (incoming_serialization_version > current_serialization_version && !warned_about_newer_version)
		{
			warned_about_newer_version = true;
			MessageBox.Show("File created with newer version.\nNot all features of this flowchart may work.\nSuggest downloading latest version.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	public void changed()
	{
		changed_guid = Guid.NewGuid();
	}

	public virtual bool break_now()
	{
		return has_breakpoint;
	}

	public virtual void Toggle_Breakpoint(int x, int y)
	{
		has_breakpoint = !has_breakpoint;
	}

	public virtual void Clear_Breakpoints()
	{
		has_breakpoint = false;
		if (Successor != null)
		{
			Successor.Clear_Breakpoints();
		}
	}

	public virtual int Count_Symbols()
	{
		if (Successor != null)
		{
			return 1 + Successor.Count_Symbols();
		}
		return 1;
	}

	public virtual Component Find_Predecessor(Component c)
	{
		if (Successor == null)
		{
			return null;
		}
		if (Successor == c)
		{
			return this;
		}
		return Successor.Find_Predecessor(c);
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("_serialization_version", current_serialization_version);
		info.AddValue("_FP", FP);
		info.AddValue("_text_str", text_str);
		info.AddValue("_name", name);
		info.AddValue("_proximity", proximity);
		info.AddValue("_head_height", head_height);
		info.AddValue("_head_width", head_width);
		info.AddValue("_head_heightOrig", head_heightOrig);
		info.AddValue("_head_widthOrig", head_widthOrig);
		info.AddValue("_connector_length", connector_length);
		info.AddValue("_x_location", x_location);
		info.AddValue("_y_location", y_location);
		info.AddValue("_Successor", Successor);
		info.AddValue("_parent", parent);
		info.AddValue("_is_child", is_child);
		info.AddValue("_is_beforeChild", is_beforeChild);
		info.AddValue("_is_afterChild", is_afterChild);
		info.AddValue("_full_text", full_text);
		info.AddValue("_height_of_text", height_of_text);
		info.AddValue("_char_length", char_length);
		info.AddValue("_rect", rect);
		info.AddValue("_comment", My_Comment);
		info.AddValue("_created_guid", created_guid);
		info.AddValue("_changed_guid", changed_guid);
	}

	public void reset()
	{
		is_child = false;
		is_beforeChild = false;
		is_afterChild = false;
		selected = false;
		running = false;
		parent = null;
		if (Successor != null)
		{
			Successor.reset();
		}
	}

	public virtual Component First_Of()
	{
		return this;
	}

	public virtual void addComment(Visual_Flow_Form form)
	{
		if (My_Comment == null)
		{
			form.Make_Undoable();
			My_Comment = new CommentBox(this);
			if (drawing_text_width > W)
			{
				My_Comment.X = (int)((float)(drawing_text_width / 2 + 20) / form.scale);
			}
			else
			{
				My_Comment.X = (int)((float)(W / 2 + 20) / form.scale);
			}
			My_Comment.Y = 0;
			My_Comment.Scale(form.scale);
		}
		My_Comment.setText(form);
	}

	public virtual Component Clone()
	{
		Component component = (Component)MemberwiseClone();
		component.FP = FP.Clone();
		component.selected = false;
		if (My_Comment != null)
		{
			component.My_Comment = My_Comment.Clone();
			component.My_Comment.parent = component;
		}
		if (Successor != null)
		{
			component.Successor = Successor.Clone();
		}
		return component;
	}

	public virtual bool contains(int x, int y)
	{
		int num = ((drawing_text_width <= W) ? W : drawing_text_width);
		bool num2 = Math.Abs(x - X) <= num / 2;
		bool flag = Math.Abs(y - (Y + H / 2)) <= H / 2;
		return num2 && flag;
	}

	public virtual bool contains(System.Drawing.Rectangle rec)
	{
		int num = ((drawing_text_width <= W) ? W : drawing_text_width);
		System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(X - num / 2 + 2, Y, num - 4, H);
		return rec.IntersectsWith(rectangle);
	}

	public virtual bool editable_selected()
	{
		return selected;
	}

	public virtual bool SelectRegion(System.Drawing.Rectangle rec)
	{
		if (compiled_flowchart)
		{
			return false;
		}
		bool flag = false;
		if (Successor != null)
		{
			flag = Successor.SelectRegion(rec);
		}
		if (contains(rec))
		{
			selected = true;
			return true;
		}
		selected = false;
		return flag;
	}

	public Component find_selection_end()
	{
		if (Successor != null && Successor.selected)
		{
			return Successor.find_selection_end();
		}
		return this;
	}

	public virtual bool cut(Visual_Flow_Form VF)
	{
		if (compiled_flowchart)
		{
			return false;
		}
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
			return Successor.cut(VF);
		}
		return false;
	}

	public virtual bool copy(Visual_Flow_Form VF)
	{
		if (compiled_flowchart)
		{
			return false;
		}
		if (selected)
		{
			Component component = find_selection_end();
			if (component.Successor != null)
			{
				Component successor = component.Successor;
				component.Successor = null;
				Component component2 = Clone();
				component2.reset();
				VF.clipboard = component2;
				component.Successor = successor;
				return true;
			}
			Component component3 = Clone();
			component3.reset();
			VF.clipboard = component3;
			return true;
		}
		if (Successor != null)
		{
			return Successor.copy(VF);
		}
		return false;
	}

	public virtual bool delete()
	{
		if (compiled_flowchart)
		{
			return false;
		}
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
			return Successor.delete();
		}
		return false;
	}

	public virtual void draw(Graphics gr, int x, int y)
	{
		if (My_Comment != null && view_comments)
		{
			My_Comment.draw(gr, x, y);
		}
	}

	public virtual System.Drawing.Rectangle comment_footprint()
	{
		System.Drawing.Rectangle r = ((Successor == null) ? new System.Drawing.Rectangle(0, 0, 0, 0) : Successor.comment_footprint());
		if (My_Comment != null && view_comments)
		{
			return CommentBox.Union(My_Comment.Get_Bounds(), r);
		}
		return r;
	}

	public virtual void init()
	{
		FP.left = W / 2;
		FP.right = W / 2;
		FP.height = H;
	}

	public virtual void wide_footprint(Graphics gr)
	{
		drawing_text_width = 0;
	}

	public virtual void footprint(Graphics gr)
	{
		init();
		if (full_text && (double)scale > 0.4)
		{
			wide_footprint(gr);
		}
		else
		{
			drawing_text_width = 0;
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
			FP.height = H + CL + Successor.FP.height;
		}
	}

	public abstract bool has_code();

	public Component find_end()
	{
		if (Successor == null)
		{
			return this;
		}
		return Successor.find_end();
	}

	public void set_parent_info(bool is_child, bool is_before_child, bool is_after_child, Component parent)
	{
		this.is_child = is_child;
		is_beforeChild = is_before_child;
		is_afterChild = is_after_child;
		this.parent = parent;
		if (Successor != null)
		{
			Successor.set_parent_info(is_child, is_before_child, is_after_child, parent);
		}
	}

	public virtual bool insert(Component newObj, int x, int y, int connector_y)
	{
		if (compiled_flowchart)
		{
			return false;
		}
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
		if (Successor != null)
		{
			return Successor.insert(newObj, x, y, connector_y);
		}
		return false;
	}

	public abstract void mark_error();

	public void Show_Guid()
	{
		string text = GetType().ToString();
		Guid guid = created_guid;
		Runtime.consoleWriteln(text + " created: " + guid.ToString());
		string text2 = GetType().ToString();
		guid = changed_guid;
		Runtime.consoleWriteln(text2 + " changed: " + guid.ToString());
	}

	public virtual void Show_Guids()
	{
		Show_Guid();
		if (Successor != null)
		{
			Successor.Show_Guids();
		}
	}

	public virtual bool overline(int x, int y, int connector_y)
	{
		if (Successor != null)
		{
			bool num = Math.Abs(x - X) < proximity;
			bool flag = y < Y + H + CL && y > Y + H;
			return num && flag;
		}
		bool num2 = Math.Abs(x - X) < proximity;
		bool flag2 = y < connector_y && y > Y + H;
		return num2 && flag2;
	}

	public virtual void Scale(float new_scale)
	{
		if (My_Comment != null)
		{
			My_Comment.Scale(new_scale);
		}
	}

	public virtual bool In_Footprint(int x, int y)
	{
		return new System.Drawing.Rectangle(X - FP.left, Y, FP.left + FP.right, FP.height).Contains(x, y);
	}

	public virtual Component select(int x, int y)
	{
		if (compiled_flowchart)
		{
			return null;
		}
		selected = false;
		Component component = null;
		if (Successor != null)
		{
			component = Successor.select(x, y);
		}
		if (contains(x, y))
		{
			selected = true;
			return this;
		}
		return component;
	}

	public virtual Component Find_Component(int x, int y)
	{
		if (compiled_flowchart)
		{
			return null;
		}
		Component component = null;
		if (Successor != null)
		{
			component = Successor.Find_Component(x, y);
		}
		if (contains(x, y))
		{
			return this;
		}
		return component;
	}

	public virtual void selectAll()
	{
		selected = true;
		if (Successor != null)
		{
			Successor.selectAll();
		}
	}

	public virtual CommentBox selectComment(int x, int y)
	{
		CommentBox commentBox = null;
		if (Successor != null)
		{
			commentBox = Successor.selectComment(x, y);
		}
		if (My_Comment != null)
		{
			My_Comment.selected = false;
			if (My_Comment.contains(x, y))
			{
				My_Comment.selected = true;
				return My_Comment;
			}
		}
		if (commentBox != null)
		{
			return commentBox;
		}
		return null;
	}

	public abstract bool setText(int x, int y, Visual_Flow_Form form);

	public abstract string getText(int x, int y);

	public static string unbreakString(string s)
	{
		return s.Replace("+", " + ").Replace("-", " - ").Replace("*", " * ")
			.Replace("^", " ^ ")
			.Replace("/", " / ")
			.Replace(",", ", ")
			.Replace("*  *", "**");
	}

	public virtual string getDrawText()
	{
		return unbreakString(Text);
	}

	public virtual void change_compressed(bool compressed)
	{
		if (Successor != null)
		{
			Successor.change_compressed(compressed);
		}
	}

	public virtual bool child_running()
	{
		if (running)
		{
			return true;
		}
		if (Successor != null)
		{
			return Successor.child_running();
		}
		return false;
	}

	public virtual bool check_expansion_click(int x, int y)
	{
		if (Successor != null)
		{
			return Successor.check_expansion_click(x, y);
		}
		return false;
	}

	public virtual bool Called_Tab(string s)
	{
		if (Successor != null)
		{
			return Successor.Called_Tab(s);
		}
		return false;
	}

	public virtual void Rename_Tab(string from, string to)
	{
		if (Successor != null)
		{
			Successor.Rename_Tab(from, to);
		}
	}

	public virtual void Emit_Code(typ gen)
	{
		if (parse_tree != null)
		{
			interpreter_pkg.emit_code(parse_tree, Text, gen);
		}
		if (Successor != null)
		{
			Successor.Emit_Code(gen);
		}
	}

	public virtual void compile_pass1(typ gen)
	{
		if (parse_tree != null)
		{
			interpreter_pkg.compile_pass1(parse_tree, Text, gen);
		}
		if (Successor != null)
		{
			Successor.compile_pass1(gen);
		}
	}

	public virtual void collect_variable_names(IList<string> l, IDictionary<string, string> types)
	{
		if (Successor != null)
		{
			Successor.collect_variable_names(l, types);
		}
	}
}
