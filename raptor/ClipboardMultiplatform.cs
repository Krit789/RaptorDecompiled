using System;
using System.Windows.Forms;

namespace raptor;

internal class ClipboardMultiplatform
{
	private class mydata : IDataObject
	{
		private object data;

		public mydata(object data)
		{
			this.data = data;
		}

		public object GetData(string format)
		{
			if (data != null && data.GetType().ToString() == format)
			{
				return data;
			}
			return null;
		}

		public object GetData(string format, bool autoConvert)
		{
			return GetData(format);
		}

		public object GetData(Type type)
		{
			if (data != null && data.GetType() == type)
			{
				return data;
			}
			return null;
		}

		public void SetData(string format, bool autoConvert, object data)
		{
			this.data = data;
		}

		public void SetData(string format, object data)
		{
			this.data = data;
		}

		public void SetData(Type type, object data)
		{
			this.data = data;
		}

		public void SetData(object data)
		{
			this.data = data;
		}

		public bool GetDataPresent(string format, bool autoConvert)
		{
			if (data != null)
			{
				return data.GetType().ToString() == format;
			}
			return false;
		}

		public bool GetDataPresent(string format)
		{
			return GetDataPresent(format, autoConvert: false);
		}

		public bool GetDataPresent(Type type)
		{
			if (data != null)
			{
				return data.GetType() == type;
			}
			return false;
		}

		public string[] GetFormats(bool autoconvert)
		{
			throw new Exception("not supported");
		}

		public string[] GetFormats()
		{
			throw new Exception("not supported");
		}
	}

	private static mydata clipboard_data;

	public static void SetDataObject(object data, bool afterExit)
	{
		if (Component.MONO)
		{
			clipboard_data = new mydata(data);
		}
		else
		{
			Clipboard.SetDataObject(data, afterExit);
		}
	}

	public static IDataObject GetDataObject()
	{
		if (Component.MONO)
		{
			return clipboard_data;
		}
		return Clipboard.GetDataObject();
	}
}
