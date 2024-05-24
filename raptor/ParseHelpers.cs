using numbers;

namespace raptor;

public class ParseHelpers
{
	public static void clearExpressions(object o)
	{
		(o as Component).method_expressions.Clear();
	}

	public static int addExpression(object o, object e)
	{
		return (o as Component).addExpression(e);
	}

	public static value getValue(object o, int i)
	{
		return (o as Component).getValue(i);
	}

	public static void addValue(object o, value v)
	{
		(o as Component).addValue(v);
	}
}
