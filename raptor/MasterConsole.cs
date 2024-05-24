using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using interpreter;
using NClass.Core;

namespace raptor;

public class MasterConsole : Form
{
	public delegate void clear_text_delegate_type(MasterConsole mc);

	public delegate void set_text_delegate_type(MasterConsole mc, string text);

	private const int min_width = 100;

	private const int min_height = 50;

	private PensBrushes.family currentFamily;

	private int currentFontSize = 8;

	private ArrayList commands = new ArrayList();

	private int current_command;

	private bool last_had_new_line = true;

	private Panel panel1;

	private Button clear_button;

	private TextBox textBox2;

	private TextBox textBox1;

	private MainMenu mainMenu1;

	private MenuItem menuFile;

	private MenuItem menuFont;

	private MenuItem menuHelp;

	private MenuItem menuGeneralHelp;

	private MenuItem menuFont6;

	private MenuItem menuFont8;

	private MenuItem menuFont10;

	private MenuItem menuFont12;

	private MenuItem menuFont14;

	private MenuItem menuFont16;

	private MenuItem menuFont20;

	private MenuItem menuFont24;

	private MenuItem menuFont28;

	private MenuItem menuFont36;

	private MenuItem menuPrintConsole;

	private MenuItem current_font;

	private MenuItem menuItem1;

	private MenuItem menuItemCopy;

	private MenuItem menuItem2;

	private MenuItem menuCourier;

	private MenuItem menuTimes;

	private MenuItem menuArial;

	private IContainer components;

	private MenuItem menuItemSelectAll;

	private MenuItem menuItemShowLog;

	private bool am_standalone;

	public clear_text_delegate_type clear_text_Delegate = clear_text_delegate;

	public set_text_delegate_type set_text_Delegate = set_text_delegate;

	public MasterConsole()
	{
		InitializeComponent();
		current_font = menuFont8;
		string text = Registry_Settings.Read("ConsoleWidth");
		System.Drawing.Rectangle desktopBounds = base.DesktopBounds;
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		if (text != null)
		{
			num = int.Parse(text);
		}
		string text2 = Registry_Settings.Read("ConsoleHeight");
		num2 = ((text2 == null) ? base.DesktopBounds.Height : int.Parse(text2));
		string text3 = Registry_Settings.Read("ConsoleX");
		if (text3 != null)
		{
			num3 = int.Parse(text3);
		}
		string text4 = Registry_Settings.Read("ConsoleY");
		if (text4 != null)
		{
			num4 = int.Parse(text4);
		}
		if (Visual_Flow_Form.IsVisibleOnAnyScreen(new System.Drawing.Rectangle(num3, num4, 20, 20)))
		{
			desktopBounds.X = num3;
			desktopBounds.Y = num4;
		}
		if (num > 100)
		{
			desktopBounds.Width = num;
		}
		if (num2 > 50)
		{
			desktopBounds.Height = num2;
		}
		if (num3 >= 0 && num4 >= 0 && num2 > 0 && num > 0)
		{
			SetDesktopBounds(desktopBounds.X, desktopBounds.Y, desktopBounds.Width, desktopBounds.Height);
			return;
		}
		Size primaryMonitorMaximizedWindowSize = SystemInformation.PrimaryMonitorMaximizedWindowSize;
		base.Left = primaryMonitorMaximizedWindowSize.Width - base.Width;
		base.Top = primaryMonitorMaximizedWindowSize.Height - base.Height - 20;
	}

	public void Set_Font_Size()
	{
		switch (Registry_Settings.Read("ConsoleFontSize"))
		{
		case "6":
			menuFont6_Click(null, null);
			break;
		case "8":
			menuFont8_Click(null, null);
			break;
		case "10":
			menuFont10_Click(null, null);
			break;
		case "12":
			menuFont12_Click(null, null);
			break;
		case "14":
			menuFont14_Click(null, null);
			break;
		case "16":
			menuFont16_Click(null, null);
			break;
		case "20":
			menuFont20_Click(null, null);
			break;
		case "24":
			menuFont24_Click(null, null);
			break;
		case "28":
			menuFont28_Click(null, null);
			break;
		case "36":
			menuFont36_Click(null, null);
			break;
		}
	}

	public MasterConsole(bool standalone)
	{
		am_standalone = standalone;
		InitializeComponent();
		current_font = menuFont8;
		base.Width = 400;
		base.Height = 200;
		Set_Font_Size();
		if (standalone)
		{
			textBox2.Enabled = false;
			menuHelp.Enabled = false;
		}
	}

	private void Extract_Process_Directory(DirectoryInfo di, StreamWriter output_stream, bool do_all, bool recursive)
	{
		Visual_Flow_Form visual_Flow_Form = new Visual_Flow_Form(null);
		ProjectCore.raptorUpdater = new UMLupdater(visual_Flow_Form);
		if (recursive)
		{
			DirectoryInfo[] directories = di.GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				output_stream.WriteLine("Subfolder: " + directoryInfo.FullName);
				Extract_Process_Directory(directoryInfo, output_stream, do_all, recursive);
			}
		}
		Component.warned_about_error = true;
		Component.warned_about_newer_version = true;
		FileInfo[] files = di.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			try
			{
				Mode mode = Mode.Intermediate;
				TabControl owner = (visual_Flow_Form.carlisle = new TabControl());
				if (!(fileInfo.Extension.ToLower() == ".rap"))
				{
					continue;
				}
				Stream stream = System.IO.File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read);
				TabControl.TabPageCollection tabPageCollection = new TabControl.TabPageCollection(owner);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				int num;
				try
				{
					num = (int)binaryFormatter.Deserialize(stream);
					if (num >= 13)
					{
						_ = (bool)binaryFormatter.Deserialize(stream);
					}
					int num2 = (int)binaryFormatter.Deserialize(stream);
					for (int j = 0; j < num2; j++)
					{
						string text = (string)binaryFormatter.Deserialize(stream);
						Subchart_Kinds subchart_Kinds = ((num >= 14) ? ((Subchart_Kinds)binaryFormatter.Deserialize(stream)) : Subchart_Kinds.Subchart);
						if (j == 0 && subchart_Kinds != Subchart_Kinds.UML)
						{
							mode = Mode.Intermediate;
							tabPageCollection.Add(new Subchart(visual_Flow_Form, "main"));
						}
						else if (j == 0)
						{
							mode = Mode.Expert;
							TabPage tabPage = new TabPage("UML");
							tabPage.Controls.Add(new UMLDiagram((UMLupdater)ProjectCore.raptorUpdater));
							tabPageCollection.Add(tabPage);
						}
						if (j <= 0)
						{
							continue;
						}
						int param_count = 0;
						switch (subchart_Kinds)
						{
						case Subchart_Kinds.Function:
							param_count = (int)binaryFormatter.Deserialize(stream);
							tabPageCollection.Add(new Procedure_Chart(visual_Flow_Form, text, param_count));
							break;
						case Subchart_Kinds.Procedure:
							if (num >= 15)
							{
								param_count = (int)binaryFormatter.Deserialize(stream);
							}
							tabPageCollection.Add(new Procedure_Chart(visual_Flow_Form, text, param_count));
							break;
						case Subchart_Kinds.Subchart:
							tabPageCollection.Add(new Subchart(visual_Flow_Form, text));
							break;
						}
					}
					if (mode == Mode.Expert)
					{
						UMLDiagram obj = tabPageCollection[0].Controls[0] as UMLDiagram;
						BinarySerializationHelper.diagram = obj.diagram;
						obj.project.LoadBinary(binaryFormatter, stream);
					}
					for (int k = ((mode == Mode.Expert) ? 1 : 0); k < num2; k++)
					{
						((Subchart)tabPageCollection[k]).Start = (Oval)binaryFormatter.Deserialize(stream);
						if (num >= 17)
						{
							_ = (byte[])binaryFormatter.Deserialize(stream);
						}
					}
				}
				catch
				{
					stream.Seek(0L, SeekOrigin.Begin);
					((Subchart)tabPageCollection[0]).Start = (Oval)binaryFormatter.Deserialize(stream);
					num = ((Subchart)tabPageCollection[0]).Start.incoming_serialization_version;
				}
				if (mode == Mode.Expert)
				{
					for (int l = 2; l < visual_Flow_Form.carlisle.TabPages.Count; l++)
					{
						ClassTabPage classTabPage = visual_Flow_Form.carlisle.TabPages[l] as ClassTabPage;
						for (int m = 0; m < classTabPage.tabControl1.TabPages.Count; m++)
						{
							(classTabPage.tabControl1.TabPages[m] as Subchart).Start = (Oval)binaryFormatter.Deserialize(stream);
							_ = (byte[])binaryFormatter.Deserialize(stream);
						}
					}
				}
				if (num >= 4)
				{
					logging_info logging_info2 = (logging_info)binaryFormatter.Deserialize(stream);
					logging_info2.Record_Open(logging_info2.Last_Username());
					output_stream.Write(fileInfo.Name + "," + logging_info2.Total_Minutes() + "," + logging_info2.Count_Saves() + "," + logging_info2.Last_Username());
					if (do_all)
					{
						output_stream.Write("," + logging_info2.Other_Authors());
					}
				}
				if (num >= 8)
				{
					_ = (bool)binaryFormatter.Deserialize(stream);
					Guid guid = (Guid)binaryFormatter.Deserialize(stream);
					Guid guid2 = guid;
					output_stream.Write("," + guid2.ToString());
					if (do_all)
					{
						Generate_Hash generate_Hash = new Generate_Hash(null);
						Compile_Helpers.Do_Compilation(((Subchart)tabPageCollection[0]).Start, generate_Hash, tabPageCollection);
						output_stream.Write("," + generate_Hash.toString());
					}
				}
				output_stream.WriteLine("");
				stream.Close();
			}
			catch (Exception ex)
			{
				output_stream.WriteLine(fileInfo.Name + ",failed," + ex.Message);
			}
		}
		visual_Flow_Form.Close();
		ProjectCore.raptorUpdater = new UMLupdater(Runtime.parent);
		Component.warned_about_newer_version = false;
		Component.warned_about_error = false;
	}

	private void compute_md5()
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.CheckFileExists = true;
		openFileDialog.Title = "Select file to compute hash";
		openFileDialog.Filter = "All files (*.*)|*.*";
		if (openFileDialog.ShowDialog() != DialogResult.Cancel)
		{
			string fileName = openFileDialog.FileName;
			set_text("The hash is: " + MD5Helper.ComputeHash(fileName) + "\n");
		}
	}

	private void extract_times(bool do_all, bool recursive)
	{
		Runtime.parent.new_clicked(null, null);
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
		{
			return;
		}
		string selectedPath = folderBrowserDialog.SelectedPath;
		if (selectedPath == "" || selectedPath == null)
		{
			MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.CheckFileExists = false;
		saveFileDialog.Title = "Select result file";
		saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
		if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
		{
			string fileName = saveFileDialog.FileName;
			if (fileName == "" || fileName == null)
			{
				MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			StreamWriter streamWriter = System.IO.File.CreateText(fileName);
			streamWriter.WriteLine("Filename,Minutes,#Saves,Last author,Previous author,GUID,SHA-512 hash");
			DirectoryInfo di = new DirectoryInfo(selectedPath);
			Extract_Process_Directory(di, streamWriter, do_all, recursive);
			Runtime.parent.carlisle.TabPages.Clear();
			Runtime.parent.carlisle.TabPages.Add(new Subchart(Runtime.parent, "main"));
			Runtime.parent.Create_Control_graphx();
			streamWriter.Close();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	public void clear_txt()
	{
		object[] args = new object[1] { this };
		if (base.IsHandleCreated)
		{
			Invoke(clear_text_Delegate, args);
		}
	}

	public static void clear_text_delegate(MasterConsole mc)
	{
		mc.textBox1.Lines = new string[0];
		mc.last_had_new_line = true;
	}

	public void program_stopped(string message)
	{
		last_had_new_line = true;
		raptor_files_pkg.close_files();
		set_text("----" + message + "----\n");
	}

	public static void set_text_delegate(MasterConsole mc, string text)
	{
		bool flag;
		string text2;
		if (text.Length >= 1 && text[text.Length - 1] != '\n')
		{
			flag = false;
			text2 = text;
		}
		else
		{
			flag = true;
			text2 = text.Substring(0, text.Length - 1);
		}
		string[] array = text2.Split('\n');
		int num = array.Length - 1;
		int num2 = num;
		if (mc.last_had_new_line || mc.textBox1.Lines.Length == 0)
		{
			num2++;
		}
		int num3 = mc.textBox1.Lines.Length;
		string[] array2 = new string[num3 + num2];
		for (int i = 0; i < num3; i++)
		{
			if (Component.MONO)
			{
				array2[i] = mc.textBox1.Lines[i] + "\n";
			}
			else
			{
				array2[i] = mc.textBox1.Lines[i];
			}
		}
		int j = 0;
		if (!mc.last_had_new_line && mc.textBox1.Lines.Length != 0)
		{
			if (Component.MONO)
			{
				array2[num3 - 1] = mc.textBox1.Lines[mc.textBox1.Lines.Length - 1] + array[0] + "\n";
			}
			else
			{
				array2[num3 - 1] = mc.textBox1.Lines[mc.textBox1.Lines.Length - 1] + array[0];
			}
			j++;
		}
		for (; j <= num; j++)
		{
			if (Component.MONO)
			{
				array2[num3++] = array[j] + "\n";
			}
			else
			{
				array2[num3++] = array[j];
			}
		}
		mc.textBox1.Lines = array2;
		mc.last_had_new_line = flag;
		mc.Focus();
		mc.textBox1.Focus();
		mc.textBox1.Select(mc.textBox1.Text.Length, 0);
		mc.textBox1.ScrollToCaret();
		if (mc.WindowState == FormWindowState.Minimized)
		{
			mc.WindowState = FormWindowState.Normal;
		}
		mc.BringToFront();
	}

	public void set_text(string text)
	{
		object[] args = new object[2] { this, text };
		if (base.IsHandleCreated)
		{
			Invoke(set_text_Delegate, args);
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.MasterConsole));
		this.panel1 = new System.Windows.Forms.Panel();
		this.clear_button = new System.Windows.Forms.Button();
		this.textBox2 = new System.Windows.Forms.TextBox();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
		this.menuFile = new System.Windows.Forms.MenuItem();
		this.menuPrintConsole = new System.Windows.Forms.MenuItem();
		this.menuItem2 = new System.Windows.Forms.MenuItem();
		this.menuArial = new System.Windows.Forms.MenuItem();
		this.menuCourier = new System.Windows.Forms.MenuItem();
		this.menuTimes = new System.Windows.Forms.MenuItem();
		this.menuFont = new System.Windows.Forms.MenuItem();
		this.menuFont6 = new System.Windows.Forms.MenuItem();
		this.menuFont8 = new System.Windows.Forms.MenuItem();
		this.menuFont10 = new System.Windows.Forms.MenuItem();
		this.menuFont12 = new System.Windows.Forms.MenuItem();
		this.menuFont14 = new System.Windows.Forms.MenuItem();
		this.menuFont16 = new System.Windows.Forms.MenuItem();
		this.menuFont20 = new System.Windows.Forms.MenuItem();
		this.menuFont24 = new System.Windows.Forms.MenuItem();
		this.menuFont28 = new System.Windows.Forms.MenuItem();
		this.menuFont36 = new System.Windows.Forms.MenuItem();
		this.menuItem1 = new System.Windows.Forms.MenuItem();
		this.menuItemCopy = new System.Windows.Forms.MenuItem();
		this.menuHelp = new System.Windows.Forms.MenuItem();
		this.menuGeneralHelp = new System.Windows.Forms.MenuItem();
		this.menuItemSelectAll = new System.Windows.Forms.MenuItem();
		this.menuItemShowLog = new System.Windows.Forms.MenuItem();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.clear_button);
		this.panel1.Controls.Add(this.textBox2);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 214);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(360, 56);
		this.panel1.TabIndex = 1;
		this.clear_button.Location = new System.Drawing.Point(272, 16);
		this.clear_button.Name = "clear_button";
		this.clear_button.Size = new System.Drawing.Size(72, 24);
		this.clear_button.TabIndex = 4;
		this.clear_button.Text = "Clear";
		this.clear_button.Click += new System.EventHandler(clear_button_Click);
		this.textBox2.Location = new System.Drawing.Point(16, 18);
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size(248, 20);
		this.textBox2.TabIndex = 3;
		this.textBox2.TextChanged += new System.EventHandler(textBox2_TextChanged);
		this.textBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox2_KeyDown);
		this.textBox1.AcceptsReturn = true;
		this.textBox1.BackColor = System.Drawing.SystemColors.Window;
		this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.textBox1.Font = new System.Drawing.Font("Times New Roman", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.textBox1.Location = new System.Drawing.Point(0, 0);
		this.textBox1.Multiline = true;
		this.textBox1.Name = "textBox1";
		this.textBox1.ReadOnly = true;
		this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.textBox1.Size = new System.Drawing.Size(360, 214);
		this.textBox1.TabIndex = 2;
		this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[5] { this.menuFile, this.menuItem2, this.menuFont, this.menuItem1, this.menuHelp });
		this.menuFile.Index = 0;
		this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[1] { this.menuPrintConsole });
		this.menuFile.Text = "&File";
		this.menuFile.Visible = false;
		this.menuPrintConsole.Index = 0;
		this.menuPrintConsole.Text = "&Print Console";
		this.menuPrintConsole.Click += new System.EventHandler(menuPrintConsole_Click);
		this.menuItem2.Index = 1;
		this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[3] { this.menuArial, this.menuCourier, this.menuTimes });
		this.menuItem2.Text = "Fo&nt";
		this.menuArial.Index = 0;
		this.menuArial.Text = "&Arial";
		this.menuArial.Click += new System.EventHandler(menuArial_Click);
		this.menuCourier.Index = 1;
		this.menuCourier.Text = "&Courier";
		this.menuCourier.Click += new System.EventHandler(menuCourier_Click);
		this.menuTimes.Checked = true;
		this.menuTimes.Index = 2;
		this.menuTimes.Text = "&Times New Roman";
		this.menuTimes.Click += new System.EventHandler(menuTimes_Click);
		this.menuFont.Index = 2;
		this.menuFont.MenuItems.AddRange(new System.Windows.Forms.MenuItem[10] { this.menuFont6, this.menuFont8, this.menuFont10, this.menuFont12, this.menuFont14, this.menuFont16, this.menuFont20, this.menuFont24, this.menuFont28, this.menuFont36 });
		this.menuFont.Text = "Font &Size";
		this.menuFont6.Index = 0;
		this.menuFont6.Text = "6";
		this.menuFont6.Click += new System.EventHandler(menuFont6_Click);
		this.menuFont8.Checked = true;
		this.menuFont8.Index = 1;
		this.menuFont8.Text = "8";
		this.menuFont8.Click += new System.EventHandler(menuFont8_Click);
		this.menuFont10.Index = 2;
		this.menuFont10.Text = "10";
		this.menuFont10.Click += new System.EventHandler(menuFont10_Click);
		this.menuFont12.Index = 3;
		this.menuFont12.Text = "12";
		this.menuFont12.Click += new System.EventHandler(menuFont12_Click);
		this.menuFont14.Index = 4;
		this.menuFont14.Text = "14";
		this.menuFont14.Click += new System.EventHandler(menuFont14_Click);
		this.menuFont16.Index = 5;
		this.menuFont16.Text = "16";
		this.menuFont16.Click += new System.EventHandler(menuFont16_Click);
		this.menuFont20.Index = 6;
		this.menuFont20.Text = "20";
		this.menuFont20.Click += new System.EventHandler(menuFont20_Click);
		this.menuFont24.Index = 7;
		this.menuFont24.Text = "24";
		this.menuFont24.Click += new System.EventHandler(menuFont24_Click);
		this.menuFont28.Index = 8;
		this.menuFont28.Text = "28";
		this.menuFont28.Click += new System.EventHandler(menuFont28_Click);
		this.menuFont36.Index = 9;
		this.menuFont36.Text = "36";
		this.menuFont36.Click += new System.EventHandler(menuFont36_Click);
		this.menuItem1.Index = 3;
		this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[2] { this.menuItemCopy, this.menuItemSelectAll });
		this.menuItem1.Text = "&Edit";
		this.menuItem1.Popup += new System.EventHandler(menuItem1_Popup);
		this.menuItemCopy.Index = 0;
		this.menuItemCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
		this.menuItemCopy.Text = "&Copy";
		this.menuItemCopy.Click += new System.EventHandler(menuItem2_Click);
		this.menuHelp.Index = 4;
		this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[2] { this.menuGeneralHelp, this.menuItemShowLog });
		this.menuHelp.Text = "&Help";
		this.menuGeneralHelp.Index = 0;
		this.menuGeneralHelp.Text = "&General Help";
		this.menuGeneralHelp.Click += new System.EventHandler(menuGeneralHelp_Click);
		this.menuItemSelectAll.Index = 1;
		this.menuItemSelectAll.Text = "Select &all";
		this.menuItemSelectAll.Click += new System.EventHandler(menuItemSelectAll_Click);
		this.menuItemShowLog.Index = 1;
		this.menuItemShowLog.Text = "&Show log";
		this.menuItemShowLog.Click += new System.EventHandler(menuItemShowLog_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		this.AutoScroll = true;
		base.ClientSize = new System.Drawing.Size(360, 270);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.panel1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Location = new System.Drawing.Point(500, 100);
		base.Menu = this.mainMenu1;
		this.MinimumSize = new System.Drawing.Size(350, 140);
		base.Name = "MasterConsole";
		base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
		this.Text = "MasterConsole";
		base.Resize += new System.EventHandler(MasterConsole_Resize);
		base.Move += new System.EventHandler(MasterConsole_Resize);
		base.Closing += new System.ComponentModel.CancelEventHandler(MasterConsole_Closing);
		base.Load += new System.EventHandler(MasterConsole_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void textBox2_TextChanged(object sender, EventArgs e)
	{
	}

	private void clear_button_Click(object sender, EventArgs e)
	{
		textBox1.Clear();
	}

	private void textBox2_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode.ToString() == "Up")
			{
				if (current_command > 0)
				{
					textBox2.Text = (string)commands[--current_command];
				}
			}
			else if (e.KeyCode.ToString() == "Down")
			{
				if (current_command < commands.Count - 1)
				{
					textBox2.Text = (string)commands[++current_command];
				}
				else if (current_command == commands.Count - 1)
				{
					textBox2.Text = "";
					current_command++;
				}
			}
			else
			{
				if (e.KeyCode != Keys.Return && e.KeyCode != Keys.Return)
				{
					return;
				}
				if (textBox2.Text == "compute_md5")
				{
					compute_md5();
				}
				else
				{
					if (textBox2.Text == "bartpe" && Environment.UserName.ToLower() == "martin.carlisle")
					{
						Component.BARTPE = true;
						command_success();
						return;
					}
					if (textBox2.Text == "extract_times")
					{
						extract_times(do_all: false, recursive: false);
						command_success();
						return;
					}
					if (textBox2.Text == "extract_times_all")
					{
						extract_times(do_all: true, recursive: false);
						command_success();
						return;
					}
					if (textBox2.Text == "extract_times_recursive")
					{
						extract_times(do_all: true, recursive: true);
						command_success();
						return;
					}
					if (textBox2.Text == "extract_times_recursive_all")
					{
						extract_times(do_all: true, recursive: true);
						command_success();
						return;
					}
					if (textBox2.Text == "show_guids")
					{
						int tabCount = Runtime.parent.carlisle.TabCount;
						for (int i = 0; i < tabCount; i++)
						{
							Runtime.consoleWriteln("GUIDs in " + Runtime.parent.carlisle.TabPages[i].Text + ": ");
							((Subchart)Runtime.parent.carlisle.TabPages[i]).Start.Show_Guids();
						}
						command_success();
						return;
					}
					if (textBox2.Text == "count_symbols")
					{
						Runtime.parent.menuCountSymbols_Click(null, null);
						command_success();
						return;
					}
					if (textBox2.Text == "increase_scope")
					{
						Runtime.Increase_Scope("test");
						command_success();
						return;
					}
					if (textBox2.Text == "show_full_log")
					{
						Runtime.parent.log.Display(Runtime.parent, show_full_log: true);
						command_success();
						return;
					}
					if (textBox2.Text == "decrease_scope")
					{
						Runtime.Decrease_Scope();
						command_success();
						return;
					}
					if (textBox2.Text.Contains("emergency_key"))
					{
						char[] separator = new char[1] { ',' };
						string[] array = textBox2.Text.Split(separator, 2);
						Runtime.parent.AES_KeyHint(array[1]);
						command_success();
						return;
					}
					if (textBox2.Text.Contains("emergency_decrypt"))
					{
						char[] separator2 = new char[1] { ',' };
						string[] array2 = textBox2.Text.Split(separator2, 4);
						Runtime.parent.AES_Decrypt(array2[1], array2[2], array2[3]);
						command_success();
						return;
					}
					if (textBox2.Text == "emergency")
					{
						new EmergencyDialog().ShowDialog(this);
						command_success();
						return;
					}
					if (textBox2.Text == "show_disassembly" && Environment.UserName.ToLower() == "martin.carlisle")
					{
						Component.compiled_flowchart = false;
						textBox2.Text = "";
						((Subchart)Runtime.parent.carlisle.SelectedTab).flow_panel.Invalidate();
						command_success();
						return;
					}
				}
				Runtime.Clear_Updated();
				syntax_result syntax_result = interpreter_pkg.statement_syntax(textBox2.Text, is_call_box: false, null);
				if (syntax_result.valid)
				{
					commands.Add(textBox2.Text);
					current_command = commands.Count;
					interpreter_pkg.run_assignment(syntax_result.tree, textBox2.Text);
					textBox2.Text = "";
					Activate();
					return;
				}
				syntax_result = interpreter_pkg.conditional_syntax(textBox2.Text, null);
				if (syntax_result.valid)
				{
					commands.Add(textBox2.Text);
					current_command = commands.Count;
					bool num = interpreter_pkg.run_boolean(syntax_result.tree, textBox2.Text);
					textBox2.Text = "";
					Activate();
					if (num)
					{
						Runtime.consoleWriteln("TRUE");
					}
					else
					{
						Runtime.consoleWriteln("FALSE");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Runtime.consoleWriteln(ex.Message);
		}
	}

	private void command_success()
	{
		commands.Add(textBox2.Text);
		current_command = commands.Count;
		textBox2.Text = "";
		Activate();
	}

	private void MasterConsole_Load(object sender, EventArgs e)
	{
	}

	private void MasterConsole_Closing(object sender, CancelEventArgs e)
	{
		if (!am_standalone)
		{
			e.Cancel = true;
			base.WindowState = FormWindowState.Minimized;
		}
		else
		{
			Application.Exit();
		}
	}

	private void menuGeneralHelp_Click(object sender, EventArgs e)
	{
		if (!Component.BARTPE && !Component.VM)
		{
			Help.ShowHelp(this, Directory.GetParent(Application.ExecutablePath)?.ToString() + "\\raptor.chm");
		}
		else
		{
			MessageBox.Show("Help not installed properly");
		}
	}

	public void menuFont6_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont6.Checked = true;
		currentFontSize = 6;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont6;
		Registry_Settings.Write("ConsoleFontSize", "6");
	}

	public void menuFont8_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont8.Checked = true;
		currentFontSize = 8;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont8;
		Registry_Settings.Write("ConsoleFontSize", "8");
	}

	public void menuFont10_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont10.Checked = true;
		currentFontSize = 10;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont10;
		Registry_Settings.Write("ConsoleFontSize", "10");
	}

	public void menuFont12_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont12.Checked = true;
		currentFontSize = 12;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont12;
		Registry_Settings.Write("ConsoleFontSize", "12");
	}

	public void menuFont14_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont14.Checked = true;
		currentFontSize = 14;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont14;
		Registry_Settings.Write("ConsoleFontSize", "14");
	}

	public void menuFont16_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont16.Checked = true;
		currentFontSize = 16;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont16;
		Registry_Settings.Write("ConsoleFontSize", "16");
	}

	public void menuFont20_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont20.Checked = true;
		currentFontSize = 20;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont20;
		Registry_Settings.Write("ConsoleFontSize", "20");
	}

	public void menuFont24_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont24.Checked = true;
		currentFontSize = 24;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont24;
		Registry_Settings.Write("ConsoleFontSize", "24");
	}

	public void menuFont28_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont28.Checked = true;
		currentFontSize = 28;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont28;
		Registry_Settings.Write("ConsoleFontSize", "28");
	}

	public void menuFont36_Click(object sender, EventArgs e)
	{
		current_font.Checked = false;
		menuFont36.Checked = true;
		currentFontSize = 36;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		current_font = menuFont36;
		Registry_Settings.Write("ConsoleFontSize", "36");
	}

	private void menuPrintConsole_Click(object sender, EventArgs e)
	{
	}

	private void menuItem2_Click(object sender, EventArgs e)
	{
		Clipboard.SetDataObject(textBox1.SelectedText);
	}

	private void menuItem1_Popup(object sender, EventArgs e)
	{
		if (textBox1.SelectedText != null && textBox1.SelectedText != "")
		{
			menuItemCopy.Enabled = true;
		}
		else
		{
			menuItemCopy.Enabled = false;
		}
	}

	public void menuCourier_Click(object sender, EventArgs e)
	{
		menuArial.Checked = false;
		menuTimes.Checked = false;
		menuCourier.Checked = true;
		currentFamily = PensBrushes.family.courier;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		Registry_Settings.Write("ConsoleFontFamily", "courier");
	}

	public void menuTimes_Click(object sender, EventArgs e)
	{
		menuArial.Checked = false;
		menuTimes.Checked = true;
		menuCourier.Checked = false;
		currentFamily = PensBrushes.family.times;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		Registry_Settings.Write("ConsoleFontFamily", "times");
	}

	public void menuArial_Click(object sender, EventArgs e)
	{
		menuArial.Checked = true;
		menuTimes.Checked = false;
		menuCourier.Checked = false;
		currentFamily = PensBrushes.family.arial;
		textBox1.Font = PensBrushes.Get_Font(currentFamily, currentFontSize);
		Registry_Settings.Write("ConsoleFontFamily", "arial");
	}

	public void MasterConsole_Resize(object sender, EventArgs e)
	{
		if (!am_standalone && base.WindowState != FormWindowState.Maximized)
		{
			if (base.DesktopLocation.X >= 0)
			{
				Registry_Settings.Write("ConsoleX", base.DesktopLocation.X.ToString());
			}
			if (base.DesktopLocation.Y >= 0)
			{
				Registry_Settings.Write("ConsoleY", base.DesktopLocation.Y.ToString());
			}
			int num;
			try
			{
				num = int.Parse(Registry_Settings.Read("ConsoleHeight"));
			}
			catch
			{
				num = 0;
			}
			if (base.DesktopBounds.Width > 100 && Math.Abs(base.Width - num) > 20)
			{
				Registry_Settings.Write("ConsoleWidth", base.DesktopBounds.Width.ToString());
			}
			try
			{
				num = int.Parse(Registry_Settings.Read("ConsoleHeight"));
			}
			catch
			{
				num = 0;
			}
			if (base.DesktopBounds.Height > 50 && Math.Abs(base.Height - num) > 20)
			{
				Registry_Settings.Write("ConsoleHeight", base.DesktopBounds.Height.ToString());
			}
		}
	}

	private void menuItemSelectAll_Click(object sender, EventArgs e)
	{
		textBox1.SelectAll();
	}

	private void menuItemShowLog_Click(object sender, EventArgs e)
	{
		Runtime.parent.log.Display(Runtime.parent, show_full_log: false);
		BringToFront();
	}
}
