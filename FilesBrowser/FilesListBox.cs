using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FilesBrowser;

[Description("Display a list of files that the user can select from")]
[DefaultProperty("DirectoryName")]
[DefaultEvent("FileSelected")]
public class FilesListBox : ListBox
{
	private string _selectedPath = "C:\\";

	private string _Extension = ".rap";

	private bool _showDirectories;

	private bool _showBackIcon;

	private IconSize _fileIconSize;

	[Description("Gets or sets the directory name that the files should relate to")]
	[DefaultValue("C:\\")]
	[Category("Data")]
	public string SelectedPath
	{
		get
		{
			return _selectedPath;
		}
		set
		{
			if (_selectedPath != string.Empty)
			{
				_selectedPath = value;
				PopulatingItems();
			}
		}
	}

	public string Extension
	{
		get
		{
			return _Extension;
		}
		set
		{
			_Extension = value.ToLower();
			PopulatingItems();
		}
	}

	[Description("Gets or sets a value indicating wheater to show directories on the control")]
	[DefaultValue(false)]
	[Category("Behavior")]
	public bool ShowDirectories
	{
		get
		{
			return _showDirectories;
		}
		set
		{
			_showDirectories = value;
			PopulatingItems();
		}
	}

	[Description("Gets or sets a value indicating whether to show the back directory icon on the control (when the IsToShowDirectories property is true)")]
	[DefaultValue(false)]
	[Category("Behavior")]
	public bool ShowBackIcon
	{
		get
		{
			return _showBackIcon;
		}
		set
		{
			_showBackIcon = value;
			PopulatingItems();
		}
	}

	[Browsable(false)]
	public string[] SelectedFiles
	{
		get
		{
			ArrayList arrayList = new ArrayList();
			foreach (object selectedItem in base.SelectedItems)
			{
				string fullName = GetFullName(selectedItem.ToString());
				if (!fullName.EndsWith("..") && System.IO.File.Exists(fullName))
				{
					arrayList.Add(fullName);
				}
			}
			return arrayList.ToArray(typeof(string)) as string[];
		}
	}

	[Browsable(false)]
	public string SelectedFile
	{
		get
		{
			string[] selectedFiles = SelectedFiles;
			if (selectedFiles.Length != 0)
			{
				return selectedFiles[0];
			}
			return null;
		}
	}

	[Description("Specifies the size of the icon of each file - small or large")]
	[DefaultValue(typeof(IconSize), "IconSize.Small")]
	[Category("Appearance")]
	public IconSize FileIconSize
	{
		get
		{
			return _fileIconSize;
		}
		set
		{
			_fileIconSize = value;
			switch (_fileIconSize)
			{
			case IconSize.Small:
				base.ItemHeight = 16;
				break;
			case IconSize.Large:
				base.ItemHeight = 32;
				break;
			}
			Invalidate();
		}
	}

	[Browsable(false)]
	public override int ItemHeight
	{
		get
		{
			return base.ItemHeight;
		}
		set
		{
			if (value == 32)
			{
				FileIconSize = IconSize.Large;
				base.ItemHeight = 32;
			}
			else
			{
				FileIconSize = IconSize.Small;
				base.ItemHeight = 16;
			}
		}
	}

	[Description("Occures whenever a file is selected by a double click on the control")]
	[Category("Action")]
	public event FileSelectedEventHandler FileSelected;

	public FilesListBox()
	{
		SelectionMode = SelectionMode.One;
		DrawMode = DrawMode.OwnerDrawFixed;
	}

	public FilesListBox(string directoryName)
		: this()
	{
		_selectedPath = SelectedPath;
		PopulatingItems();
	}

	private void AddDirectory(string directoryName)
	{
		base.Items.Add(directoryName);
	}

	private void AddFile(string fileName)
	{
		base.Items.Add(fileName);
	}

	private string GetFullName(string fileNameOnly)
	{
		return Path.Combine(_selectedPath, fileNameOnly);
	}

	private void PopulatingItems()
	{
		if (base.DesignMode)
		{
			return;
		}
		base.Items.Clear();
		if (_showBackIcon && _selectedPath.Length > 3)
		{
			base.Items.Add("..");
		}
		try
		{
			string[] directories;
			if (_showDirectories)
			{
				directories = Directory.GetDirectories(_selectedPath);
				for (int i = 0; i < directories.Length; i++)
				{
					string fileName = Path.GetFileName(directories[i]);
					base.Items.Add(fileName);
				}
			}
			directories = Directory.GetFiles(_selectedPath);
			for (int i = 0; i < directories.Length; i++)
			{
				string fileName2 = Path.GetFileName(directories[i]);
				if (Extension != null && Extension.CompareTo("") != 0)
				{
					if (Path.GetExtension(fileName2).ToLower() == Extension)
					{
						base.Items.Add(fileName2);
					}
				}
				else
				{
					base.Items.Add(fileName2);
				}
			}
		}
		catch
		{
		}
		Invalidate();
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		if (base.SelectedItem == null)
		{
			return;
		}
		string fullName = GetFullName(base.SelectedItem.ToString());
		if (fullName.EndsWith(".."))
		{
			if (_selectedPath.EndsWith("\\"))
			{
				_selectedPath = _selectedPath.Remove(_selectedPath.Length - 1, 1);
			}
			_selectedPath = Directory.GetParent(_selectedPath).FullName;
			PopulatingItems();
		}
		else if (Directory.Exists(fullName))
		{
			_selectedPath = fullName + "\\";
			PopulatingItems();
		}
		else
		{
			OnFileSelected(new FileSelectEventArgs(fullName));
		}
		base.OnMouseDoubleClick(e);
	}

	protected void OnFileSelected(FileSelectEventArgs fse)
	{
		if (this.FileSelected != null)
		{
			this.FileSelected(this, fse);
		}
	}

	protected override void OnDrawItem(DrawItemEventArgs e)
	{
		e.DrawBackground();
		e.DrawFocusRectangle();
		Rectangle bounds = e.Bounds;
		if (e.Index > -1 && e.Index < base.Items.Count)
		{
			string text = base.Items[e.Index].ToString();
			string fullName = GetFullName(text);
			Icon icon = null;
			Rectangle targetRect = new Rectangle(bounds.Left + 1, bounds.Top + 1, ItemHeight - 2, ItemHeight - 2);
			if (text.Equals(".."))
			{
				icon = IconExtractor.GetFileIcon(Application.StartupPath, _fileIconSize);
				e.Graphics.DrawIcon(icon, targetRect);
			}
			else
			{
				icon = IconExtractor.GetFileIcon(fullName, _fileIconSize);
				e.Graphics.DrawIcon(icon, targetRect);
			}
			Size size = targetRect.Size;
			icon.Dispose();
			Rectangle rectangle = new Rectangle(bounds.Left + size.Width + 3, bounds.Top, bounds.Width - size.Width - 3, bounds.Height);
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			e.Graphics.DrawString(text, e.Font, new SolidBrush(e.ForeColor), rectangle, stringFormat);
		}
		base.OnDrawItem(e);
	}
}
