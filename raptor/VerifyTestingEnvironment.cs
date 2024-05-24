namespace raptor;

internal class VerifyTestingEnvironment
{
	private static string drive = Component.BARTPE_partition_path;

	private static string[] hashes = new string[11]
	{
		"007771AFE87B37CACD105C0FBDFB9135", "0DE0DAFA887669ECDC6AC13F98599818", "B31D299FF98B4C7D20BE6AC0ADFD2344", "22C24408D784EC590CC07AA33BBCDC5D", "B93A94DFA32197F21D95C6A3FAD9DC17", "171F0F8D05C090600B3633B9DCDC4732", "74F6B503A0F0DB3C25D0F27595C645AF", "2AC26248E0AEB120AF4BF12B1E17F329", "0D23D78B35BA6D1FAD60AF9FB7FB39B0", "FD432628CE8B41ADAFE52E81C4B43A44",
		"D45AC76AFF1438925578BBAEFF0A07A9"
	};

	private static string[] files_to_verify = new string[11]
	{
		"autorun.inf", "minint\\system32\\autorun.cmd", "minint\\system32\\autorun0raptor_regadd.cmd", "programs\\raptor\\gpg.exe", "Programs\\Nu2Menu\\setres.exe", "Programs\\Nu2Menu\\nu2menu.exe", "Programs\\Nu2Menu\\nu2menu.xml", "Programs\\RAPTOR\\Perl\\bin\\perl.exe", "minint\\system32\\disablecd.reg", "minint\\system32\\vc2005rt.reg",
		"programs\\raptor\\unzip.exe"
	};

	public static bool VerifyEnvironment()
	{
		return true;
	}
}
