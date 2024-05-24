using Microsoft.Win32;

namespace raptor;

public class Registry_Settings
{
	public static bool Ignore_Updates;

	public static void Write(string key, string val)
	{
		if (Ignore_Updates)
		{
			return;
		}
		try
		{
			Registry.CurrentUser.OpenSubKey("Software", writable: true).CreateSubKey("Raptor").SetValue(key, val);
		}
		catch
		{
		}
	}

	public static string Read(string key)
	{
		try
		{
			return (string)Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Raptor").GetValue(key);
		}
		catch
		{
			return null;
		}
	}

	public static string Global_Read(string key)
	{
		return null;
	}
}
