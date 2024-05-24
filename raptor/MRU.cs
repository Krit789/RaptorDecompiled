using Microsoft.Win32;

namespace raptor;

public class MRU
{
	public static void Update_MRU_Menus(Visual_Flow_Form form)
	{
		try
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Raptor");
			string text = (string)registryKey.GetValue("MRU1");
			form.menuMRU1.Text = "&1 - " + text;
			string text2 = (string)registryKey.GetValue("MRU2");
			form.menuMRU2.Text = "&2 - " + text2;
			string text3 = (string)registryKey.GetValue("MRU3");
			form.menuMRU3.Text = "&3 - " + text3;
			string text4 = (string)registryKey.GetValue("MRU4");
			form.menuMRU4.Text = "&4 - " + text4;
			string text5 = (string)registryKey.GetValue("MRU5");
			form.menuMRU5.Text = "&5 - " + text5;
			string text6 = (string)registryKey.GetValue("MRU6");
			form.menuMRU6.Text = "&6 - " + text6;
			string text7 = (string)registryKey.GetValue("MRU7");
			form.menuMRU7.Text = "&7 - " + text7;
			string text8 = (string)registryKey.GetValue("MRU8");
			form.menuMRU8.Text = "&8 - " + text8;
			string text9 = (string)registryKey.GetValue("MRU9");
			form.menuMRU9.Text = "&9 - " + text9;
		}
		catch
		{
		}
	}

	public static string Get_MRU_Registry(int i)
	{
		try
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Raptor");
			return i switch
			{
				1 => (string)registryKey.GetValue("MRU1"), 
				2 => (string)registryKey.GetValue("MRU2"), 
				3 => (string)registryKey.GetValue("MRU3"), 
				4 => (string)registryKey.GetValue("MRU4"), 
				5 => (string)registryKey.GetValue("MRU5"), 
				6 => (string)registryKey.GetValue("MRU6"), 
				7 => (string)registryKey.GetValue("MRU7"), 
				8 => (string)registryKey.GetValue("MRU8"), 
				9 => (string)registryKey.GetValue("MRU9"), 
				_ => null, 
			};
		}
		catch
		{
			return null;
		}
	}

	public static void Add_To_MRU_Registry(string name)
	{
		try
		{
			string[] array = new string[10];
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", writable: true).CreateSubKey("Raptor");
			for (int i = 1; i < 10; i++)
			{
				try
				{
					array[i] = (string)registryKey.GetValue("MRU" + i);
				}
				catch
				{
					array[i] = "";
				}
			}
			for (int j = 1; j < 10; j++)
			{
				if (array[j] != null && array[j].ToLower().CompareTo(name.ToLower()) == 0)
				{
					string text = array[j];
					for (int num = j; num >= 2; num--)
					{
						array[num] = array[num - 1];
						registryKey.SetValue("MRU" + num, array[num]);
					}
					array[1] = text;
					registryKey.SetValue("MRU1", text);
					return;
				}
			}
			registryKey.SetValue("MRU1", name);
			for (int k = 2; k < 10; k++)
			{
				if (array[k - 1] != null)
				{
					registryKey.SetValue("MRU" + k, array[k - 1]);
				}
			}
		}
		catch
		{
		}
	}
}
