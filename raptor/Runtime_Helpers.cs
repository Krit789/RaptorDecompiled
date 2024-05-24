using System;
using numbers;

namespace raptor;

public class Runtime_Helpers
{
	public static value Get_Value_String(value s, value value_index)
	{
		if (!numbers_pkg.is_integer(value_index))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index) + " is not a valid string index.");
		}
		int num = numbers_pkg.integer_of(value_index);
		return numbers_pkg.make_value__4(s.s[num - 1]);
	}

	public static void Set_Value_String(value s, value value_index, value v)
	{
		if (!numbers_pkg.is_integer(value_index))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index) + " is not a valid string index.");
		}
		int num = numbers_pkg.integer_of(value_index);
		if (num > s.s.Length)
		{
			s.s = s.s + new string(' ', num - s.s.Length - 1) + (char)numbers_pkg.integer_of(v);
		}
		else
		{
			s.s = s.s.Remove(num - 1, 1).Insert(num - 1, ((char)numbers_pkg.integer_of(v)).ToString() ?? "");
		}
	}
}
