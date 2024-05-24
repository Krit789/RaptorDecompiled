using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace raptor;

public class Autoupdate
{
	private static bool result = false;

	private static string path;

	private static string setup_path;

	public static ThreadStart question_delegate = Ask_The_Question;

	public static void Ask_The_Question()
	{
		try
		{
			if (System.IO.File.GetLastWriteTime(path) > System.IO.File.GetLastWriteTime(Application.ExecutablePath).AddMinutes(30.0))
			{
				result = true;
			}
		}
		catch
		{
		}
	}

	public static bool Autoupdate_Requested()
	{
		return false;
	}
}
