using System.Collections;

namespace raptor;

public class CallStack
{
	public class StackFrame
	{
		public Component obj;

		public Subchart code;

		public object context;

		public StackFrame(Component the_obj, Subchart the_code)
		{
			obj = the_obj;
			code = the_code;
		}
	}

	private static ArrayList stack = new ArrayList();

	public static void Push(Component obj, Subchart code)
	{
		StackFrame value = new StackFrame(obj, code);
		stack.Add(value);
	}

	public static void Pop()
	{
		stack.RemoveAt(stack.Count - 1);
	}

	public static void setContext(object context)
	{
		((StackFrame)stack[stack.Count - 1]).context = context;
	}

	public static StackFrame Top()
	{
		return (StackFrame)stack[stack.Count - 1];
	}

	public static int Count()
	{
		return stack.Count;
	}

	public static void Clear_Call_Stack()
	{
		for (int i = 0; i < stack.Count; i++)
		{
			((StackFrame)stack[i]).obj.running = false;
			((StackFrame)stack[i]).code.flow_panel.Invalidate();
		}
		stack.Clear();
	}
}
