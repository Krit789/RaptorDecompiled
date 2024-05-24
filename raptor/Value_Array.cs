using System;
using System.Collections;
using numbers;

namespace raptor;

public class Value_Array
{
	private ArrayList values;

	public Value_Array()
	{
		values = new ArrayList();
	}

	public int Get_Length()
	{
		return values.Count;
	}

	public void Set_Value(value value_index, value v)
	{
		if (!numbers_pkg.is_integer(value_index))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index) + " is not a valid array index.");
		}
		int num = numbers_pkg.integer_of(value_index);
		if (num > values.Count)
		{
			for (int i = values.Count; i <= num - 1; i++)
			{
				values.Add(numbers_pkg.make_value__2(0.0));
			}
		}
		try
		{
			values[num - 1] = v._deep_clone();
		}
		catch
		{
			throw new Exception("can't do array assign to: " + (num - 1));
		}
	}

	public value Get_Value(value value_index)
	{
		if (!numbers_pkg.is_integer(value_index))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index) + " is not a valid array index.");
		}
		int num = numbers_pkg.integer_of(value_index);
		return (value)values[num - 1];
	}

	public double[] get_Doublea()
	{
		int count = values.Count;
		double[] array = new double[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = ((value)values[i]).v;
		}
		return array;
	}

	public float[] get_Singlea()
	{
		int count = values.Count;
		float[] array = new float[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = (float)((value)values[i]).v;
		}
		return array;
	}

	public int[] get_Int32a()
	{
		int count = values.Count;
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = numbers_pkg.integer_of((value)values[i]);
		}
		return array;
	}

	public void set_Int32a(int[] values)
	{
		for (int num = values.Length - 1; num >= 0; num--)
		{
			Set_Value(numbers_pkg.make_value__3(num + 1), numbers_pkg.make_value__3(values[num]));
		}
	}

	public void set_Singlea(float[] values)
	{
		for (int num = values.Length - 1; num >= 0; num--)
		{
			Set_Value(numbers_pkg.make_value__3(num + 1), numbers_pkg.make_value__2(values[num]));
		}
	}

	public void set_Doublea(double[] values)
	{
		for (int num = values.Length - 1; num >= 0; num--)
		{
			Set_Value(numbers_pkg.make_value__3(num + 1), numbers_pkg.make_value__2(values[num]));
		}
	}
}
