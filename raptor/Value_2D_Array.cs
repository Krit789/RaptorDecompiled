using System;
using System.Collections;
using numbers;

namespace raptor;

public class Value_2D_Array
{
	private ArrayList values;

	public Value_2D_Array()
	{
		values = new ArrayList();
	}

	public void Set_Value(value value_index1, value value_index2, value v)
	{
		if (!numbers_pkg.is_integer(value_index1))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index1) + " is not a valid array index.");
		}
		if (!numbers_pkg.is_integer(value_index2))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index2) + " is not a valid array index.");
		}
		int num = numbers_pkg.integer_of(value_index1);
		int num2 = numbers_pkg.integer_of(value_index2);
		int num3 = values.Count;
		int num4 = 0;
		if (num3 > 0)
		{
			num4 = ((ArrayList)values[0]).Count;
		}
		if (num > num3)
		{
			for (int i = num3; i <= num - 1; i++)
			{
				values.Add(new ArrayList());
				for (int j = 0; j < num4; j++)
				{
					((ArrayList)values[i]).Add(numbers_pkg.make_value__2(0.0));
				}
			}
			num3 = num;
		}
		if (num2 > num4)
		{
			for (int k = 0; k < num3; k++)
			{
				for (int l = num4; l <= num2 - 1; l++)
				{
					((ArrayList)values[k]).Add(numbers_pkg.make_value__2(0.0));
				}
			}
		}
		try
		{
			((ArrayList)values[num - 1])[num2 - 1] = v._deep_clone();
		}
		catch
		{
			throw new Exception("can't do 2D assign to: " + (num - 1) + "," + (num2 - 1));
		}
	}

	public value Get_Value(value value_index1, value value_index2)
	{
		if (!numbers_pkg.is_integer(value_index1))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index1) + " is not a valid array index.");
		}
		if (!numbers_pkg.is_integer(value_index2))
		{
			throw new Exception(numbers_pkg.msstring_image(value_index2) + " is not a valid array index.");
		}
		int num = numbers_pkg.integer_of(value_index1);
		int num2 = numbers_pkg.integer_of(value_index2);
		return (value)((ArrayList)values[num - 1])[num2 - 1];
	}

	public double[][] get_Doubleaa()
	{
		int count = values.Count;
		int count2 = ((ArrayList)values[0]).Count;
		double[][] array = new double[count][];
		for (int i = 0; i < count; i++)
		{
			array[i] = new double[count2];
			for (int j = 0; j < count2; j++)
			{
				array[i][j] = ((value)((ArrayList)values[i])[j]).v;
			}
		}
		return array;
	}

	public float[][] get_Singleaa()
	{
		int count = values.Count;
		int count2 = ((ArrayList)values[0]).Count;
		float[][] array = new float[count][];
		for (int i = 0; i < count; i++)
		{
			array[i] = new float[count2];
			for (int j = 0; j < count2; j++)
			{
				array[i][j] = (float)((value)((ArrayList)values[i])[j]).v;
			}
		}
		return array;
	}

	public int[][] get_Int32aa()
	{
		int count = values.Count;
		int count2 = ((ArrayList)values[0]).Count;
		int[][] array = new int[count][];
		for (int i = 0; i < count; i++)
		{
			array[i] = new int[count2];
			for (int j = 0; j < count2; j++)
			{
				array[i][j] = numbers_pkg.integer_of((value)((ArrayList)values[i])[j]);
			}
		}
		return array;
	}

	public void set_Int32aa(int[][] values)
	{
		for (int num = values.Length - 1; num >= 0; num--)
		{
			for (int num2 = values[0].Length - 1; num2 >= 0; num2--)
			{
				Set_Value(numbers_pkg.make_value__3(num + 1), numbers_pkg.make_value__3(num2 + 1), numbers_pkg.make_value__3(values[num][num2]));
			}
		}
	}

	public void set_Singleaa(float[][] values)
	{
		for (int num = values.Length - 1; num >= 0; num--)
		{
			for (int num2 = values[0].Length - 1; num2 >= 0; num2--)
			{
				Set_Value(numbers_pkg.make_value__3(num + 1), numbers_pkg.make_value__3(num2 + 1), numbers_pkg.make_value__2(values[num][num2]));
			}
		}
	}

	public void set_Doubleaa(double[][] values)
	{
		for (int num = values.Length - 1; num >= 0; num--)
		{
			for (int num2 = values[0].Length - 1; num2 >= 0; num2--)
			{
				Set_Value(numbers_pkg.make_value__3(num + 1), numbers_pkg.make_value__3(num2 + 1), numbers_pkg.make_value__2(values[num][num2]));
			}
		}
	}
}
