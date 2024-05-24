using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace raptor;

[Serializable]
public class logging_info : ISerializable
{
	public enum event_kind
	{
		Opened,
		Saved,
		Autosaved,
		Pasted_From,
		Paste_Opened,
		Paste_Saved,
		Paste_Autosaved
	}

	[Serializable]
	public class event_record
	{
		public string Username;

		public string Machine_Name;

		public event_kind Kind;

		public DateTime Time;

		public event_record(string user, string machine, event_kind k, DateTime t)
		{
			Username = user;
			Machine_Name = machine;
			Kind = k;
			Time = t;
		}
	}

	private ArrayList events = new ArrayList();

	public logging_info()
	{
	}

	[DllImport("kernel32.dll")]
	private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, uint VolumeNameSize, ref uint VolumeSerialNumber, ref uint MaximumComponentLength, ref uint FileSystemFlags, StringBuilder FileSystemNameBuffer, uint FileSystemNameSize);

	public string GetVolumeSerial(string strDriveLetter)
	{
		uint VolumeSerialNumber = 0u;
		uint MaximumComponentLength = 0u;
		StringBuilder stringBuilder = new StringBuilder(256);
		uint FileSystemFlags = 0u;
		StringBuilder stringBuilder2 = new StringBuilder(256);
		strDriveLetter += ":\\";
		GetVolumeInformation(strDriveLetter, stringBuilder, (uint)stringBuilder.Capacity, ref VolumeSerialNumber, ref MaximumComponentLength, ref FileSystemFlags, stringBuilder2, (uint)stringBuilder2.Capacity);
		return Convert.ToString(VolumeSerialNumber);
	}

	public string GetMachineName()
	{
		string machineName = Environment.MachineName;
		try
		{
			if (machineName.CompareTo("MININT-JVC") == 0)
			{
				return GetVolumeSerial("c");
			}
		}
		catch
		{
		}
		return Environment.MachineName;
	}

	public logging_info Clone()
	{
		return new logging_info
		{
			events = (ArrayList)events.Clone()
		};
	}

	public logging_info(SerializationInfo info, StreamingContext ctxt)
	{
		events.Clear();
		_ = (int)info.GetValue("_serialization_version", typeof(int));
		int num = (int)info.GetValue("_count", typeof(int));
		for (int i = 0; i < num; i++)
		{
			string @string = info.GetString("_user" + i);
			string string2 = info.GetString("_machine" + i);
			DateTime dateTime = info.GetDateTime("_date" + i);
			event_kind k = (event_kind)info.GetValue("_kind" + i, typeof(event_kind));
			events.Add(new event_record(@string, string2, k, dateTime));
		}
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("_serialization_version", Component.current_serialization_version);
		info.AddValue("_count", events.Count);
		for (int i = 0; i < events.Count; i++)
		{
			info.AddValue("_user" + i, ((event_record)events[i]).Username);
			info.AddValue("_machine" + i, ((event_record)events[i]).Machine_Name);
			info.AddValue("_date" + i, ((event_record)events[i]).Time);
			info.AddValue("_kind" + i, ((event_record)events[i]).Kind);
		}
	}

	public void Record_Save()
	{
		events.Add(new event_record(Environment.UserName, Environment.MachineName, event_kind.Saved, DateTime.Now));
	}

	public void Record_Autosave()
	{
		events.Add(new event_record(Environment.UserName, Environment.MachineName, event_kind.Autosaved, DateTime.Now));
	}

	public void Record_Open(string name)
	{
		events.Add(new event_record(name, Environment.MachineName, event_kind.Opened, DateTime.Now));
	}

	public void Record_Open()
	{
		events.Add(new event_record(Environment.UserName, Environment.MachineName, event_kind.Opened, DateTime.Now));
	}

	public static bool New_Pair(string username, string machinename, logging_info log, int i)
	{
		for (int j = 0; j < i; j++)
		{
			if (((event_record)log.events[j]).Kind == event_kind.Paste_Saved && ((event_record)log.events[j]).Username == username && ((event_record)log.events[j]).Machine_Name == machinename)
			{
				return false;
			}
		}
		return true;
	}

	public static bool Last_Pair(string username, string machinename, logging_info log, int i)
	{
		for (int j = i + 1; j < log.events.Count; j++)
		{
			if (((event_record)log.events[j]).Kind == event_kind.Paste_Saved && ((event_record)log.events[j]).Username == username && ((event_record)log.events[j]).Machine_Name == machinename)
			{
				return false;
			}
		}
		return true;
	}

	public void Record_Paste(logging_info log, int count_symbols, Guid guid)
	{
		events.Add(new event_record(count_symbols.ToString(), guid.ToString(), event_kind.Pasted_From, DateTime.Now));
		for (int i = 0; i < log.events.Count; i++)
		{
			event_record event_record = (event_record)log.events[i];
			switch (event_record.Kind)
			{
			case event_kind.Opened:
				event_record.Kind = event_kind.Paste_Opened;
				break;
			case event_kind.Saved:
				event_record.Kind = event_kind.Paste_Saved;
				break;
			case event_kind.Autosaved:
				event_record.Kind = event_kind.Paste_Autosaved;
				break;
			}
		}
		for (int j = 0; j < log.events.Count; j++)
		{
			event_record event_record2 = (event_record)log.events[j];
			if ((event_record2.Username != Environment.UserName || event_record2.Machine_Name != Environment.MachineName) && event_record2.Kind == event_kind.Paste_Saved && (New_Pair(event_record2.Username, event_record2.Machine_Name, log, j) || Last_Pair(event_record2.Username, event_record2.Machine_Name, log, j)))
			{
				events.Add(event_record2);
			}
		}
	}

	public void Clear()
	{
		events.Clear();
	}

	public TimeSpan Compute_Total_Time()
	{
		TimeSpan timeSpan = new TimeSpan(0L);
		DateTime time = ((event_record)events[0]).Time;
		for (int i = 0; i < events.Count; i++)
		{
			event_record event_record = (event_record)events[i];
			if (event_record.Kind == event_kind.Opened)
			{
				time = event_record.Time;
			}
			else if (i < events.Count - 1 && ((event_record)events[i + 1]).Kind == event_kind.Opened)
			{
				timeSpan += event_record.Time.Subtract(time);
			}
		}
		return timeSpan + DateTime.Now.Subtract(time);
	}

	public int Count_Saves()
	{
		int num = 0;
		for (int i = 0; i < events.Count; i++)
		{
			event_record event_record = (event_record)events[i];
			if (event_record.Kind == event_kind.Saved || event_record.Kind == event_kind.Autosaved)
			{
				num++;
			}
		}
		return num;
	}

	public void Display(Visual_Flow_Form form, bool show_full_log)
	{
		int num = 0;
		int num2 = 0;
		string text = "LOG for: " + form.Text + "(" + form.file_guid.ToString() + ")\n";
		for (int i = 0; i < events.Count; i++)
		{
			event_record event_record = (event_record)events[i];
			if (event_record.Kind == event_kind.Autosaved)
			{
				num++;
				num2++;
				if (show_full_log)
				{
					text = text + event_record.Kind.ToString() + " by: " + event_record.Username + " on: " + event_record.Machine_Name + " at: " + event_record.Time.ToString() + "\n";
				}
			}
			else if (event_record.Kind != event_kind.Paste_Autosaved)
			{
				if (num != 0)
				{
					text = text + num + " autosaves.\n";
					num = 0;
				}
				text = ((event_record.Kind == event_kind.Pasted_From) ? (text + event_record.Kind.ToString() + " " + event_record.Machine_Name + " " + event_record.Username + " symbols at: " + event_record.Time.ToString() + "\n") : (text + event_record.Kind.ToString() + " by: " + event_record.Username + " on: " + event_record.Machine_Name + " at: " + event_record.Time.ToString() + "\n"));
			}
		}
		if (num != 0)
		{
			text = text + num + " autosaves.\n";
		}
		text = text + "Total time (minutes): " + $"{Compute_Total_Time().TotalMinutes:F2}" + "\n";
		text = text + "Total autosaves: " + num2 + "\n";
		Runtime.consoleWriteln(text);
	}

	public string Last_Username()
	{
		return ((event_record)events[events.Count - 1]).Username;
	}

	public string Total_Minutes()
	{
		return $"{Compute_Total_Time().TotalMinutes:F2}";
	}

	public string Second_Author()
	{
		string text = Last_Username();
		if (events.Count > 1)
		{
			for (int num = events.Count - 1; num >= 0; num--)
			{
				if (((event_record)events[num]).Username.ToLower() != text.ToLower() && ((event_record)events[num]).Kind != event_kind.Pasted_From)
				{
					return ((event_record)events[num]).Username;
				}
			}
		}
		return "";
	}

	public string Other_Authors()
	{
		string text = Last_Username();
		string text2 = "";
		if (events.Count > 1)
		{
			for (int num = events.Count - 1; num >= 0; num--)
			{
				if (((event_record)events[num]).Username.ToLower() != text.ToLower() && ((event_record)events[num]).Kind != event_kind.Pasted_From)
				{
					text = ((event_record)events[num]).Username;
					text2 = ((!(text2 != "")) ? ((event_record)events[num]).Username : (text2 + "&" + ((event_record)events[num]).Username));
				}
			}
		}
		return text2;
	}

	public string All_Authors()
	{
		string text = Last_Username();
		string text2 = text;
		if (events.Count > 1)
		{
			for (int num = events.Count - 1; num >= 0; num--)
			{
				if (((event_record)events[num]).Username.ToLower() != text.ToLower() && ((event_record)events[num]).Kind != event_kind.Pasted_From)
				{
					text = ((event_record)events[num]).Username;
					text2 = text2 + "&" + ((event_record)events[num]).Username;
				}
			}
		}
		return text2;
	}
}
