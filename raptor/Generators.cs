using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using generate_interface;
using GeneratorAda;

namespace raptor;

internal class Generators
{
	private static Dictionary<string, Type> Generator_List = new Dictionary<string, Type>();

	public static bool Handles_OO(string name)
	{
		return Generator_List[name].GetInterface(typeof(OO_Interface).FullName) != null;
	}

	public static bool Handles_Imperative(string name)
	{
		return Generator_List[name].GetInterface(typeof(Imperative_Interface).FullName) != null;
	}

	public static typ Create_From_Menu(string name, string filename)
	{
		Type type = Generator_List[name];
		Type[] array = new Type[1];
		object[] array2 = new object[1];
		array[0] = typeof(string);
		ConstructorInfo constructor = type.GetConstructor(array);
		array2[0] = filename;
		return constructor.Invoke(array2) as typ;
	}

	public static void Process_Assembly(Visual_Flow_Form form, Assembly assembly)
	{
		Type[] types = assembly.GetTypes();
		int i = 0;
		for (int j = 0; j < types.Length; j++)
		{
			if (!(types[j].GetInterface(typeof(typ).FullName) != null))
			{
				continue;
			}
			try
			{
				MethodInfo method = types[j].GetMethod("Get_Menu_Name");
				object obj = types[j].GetConstructor(Type.EmptyTypes).Invoke(null);
				string text = method.Invoke(obj, null) as string;
				MenuItem item = new MenuItem(text, form.handle_click);
				if (form.menuItemGenerate.MenuItems.Count > 1)
				{
					for (; i < form.menuItemGenerate.MenuItems.Count && text.Replace("&", "").CompareTo(form.menuItemGenerate.MenuItems[i].Text.Replace("&", "")) > 0; i++)
					{
					}
				}
				form.menuItemGenerate.MenuItems.Add(i, item);
				Generator_List.Add(text, types[j]);
			}
			catch
			{
			}
		}
	}

	public static void Load_Generators(Visual_Flow_Form form)
	{
		FileInfo[] files = Directory.GetParent(Application.ExecutablePath).GetFiles("generator*.dll");
		for (int i = 0; i < files.Length; i++)
		{
			try
			{
				Assembly assembly = Assembly.LoadFrom(files[i].FullName);
				Process_Assembly(form, assembly);
			}
			catch
			{
			}
		}
	}
}
