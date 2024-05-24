using System;
using System.IO;
using System.Security.Cryptography;

namespace raptor;

public class MD5Helper
{
	public static string ComputeHash(string filename)
	{
		MD5 mD = new MD5CryptoServiceProvider();
		StreamReader streamReader;
		try
		{
			streamReader = new StreamReader(filename);
		}
		catch
		{
			return "";
		}
		streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
		byte[] array = mD.ComputeHash(streamReader.BaseStream);
		string text = "";
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			byte b = array[i];
			text += Convert(b, 16);
		}
		streamReader.Close();
		return text;
	}

	public static bool CheckValueAgainstHash(string md5Hash, string inputValue)
	{
		return ComputeHash(inputValue).Equals(md5Hash);
	}

	private static string Convert(long dblCount, int intBaseformat)
	{
		string text = "";
		int i;
		for (i = 1; dblCount / (long)Math.Pow(intBaseformat, i) >= intBaseformat; i++)
		{
		}
		while (i >= 0)
		{
			long num;
			if (i == 0)
			{
				num = dblCount;
				dblCount = 0L;
			}
			else
			{
				long num2 = (long)Math.Pow(intBaseformat, i);
				num = dblCount / num2;
				dblCount -= num * num2;
			}
			text = ((num >= 10) ? (text + (char)(65 + (num - 10))) : (text + num));
			i--;
		}
		if (text == "")
		{
			text = "0";
		}
		return text;
	}
}
