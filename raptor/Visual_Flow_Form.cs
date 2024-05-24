using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using dotnetgraphlibrary;
using generate_interface;
using Microsoft.Ink;
using Microsoft.Win32;
using NClass.Core;
using NClass.GUI.Diagram;
using parse_tree;
using static System.Net.WebRequestMethods;

namespace raptor;

public class Visual_Flow_Form : Form
{
	public delegate Oval Start_Delegate_Type(Visual_Flow_Form f);

	public delegate void Set_Current_Tab_Delegate_Type(Visual_Flow_Form f, TabPage tb);

	public delegate void Set_Running_Delegate_Type(Visual_Flow_Form f, bool v);

	public delegate void Set_TopMost_Delegate_Type(Visual_Flow_Form f, bool v);

	public delegate void MessageBox_Delegate_Type(Visual_Flow_Form f, string text, string caption, MessageBoxIcon icon);

	public delegate void updateScreen_Delegate_Type(Visual_Flow_Form f);

	public delegate void invalidateScreen_Delegate_Type(Visual_Flow_Form f);

	public delegate void Load_Delegate_Type(Visual_Flow_Form f);

	public class Win32
	{
		[DllImport("user32.dll")]
		public static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("user32.dll")]
		public static extern bool EmptyClipboard();

		[DllImport("user32.dll")]
		public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

		[DllImport("user32.dll")]
		public static extern bool CloseClipboard();

		[DllImport("gdi32.dll")]
		public static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteEnhMetaFile(IntPtr hemf);
	}

	private const int min_width = 570;

	private const int min_height = 370;

	private static bool starting_up = true;

	public static bool command_line_run = false;

	public static bool command_line_input_redirect = false;

	public static bool command_line_output_redirect = false;

	private string My_Title = "Raptor";

	public string tooltip_text = "";

	public float scale = 0.75f;

	public float print_scale = 0.75f;

	private bool Save_Error;

	private DateTime last_autosave = DateTime.Now;

	private DateTime last_draw = DateTime.MinValue;

	public logging_info log = new logging_info();

	private int symbol_count;

	public const int assignment_fig = 0;

	public const int call_fig = 1;

	public const int input_fig = 2;

	public const int output_fig = 3;

	public const int if_control_fig = 4;

	public const int loop_fig = 5;

	public const int return_fig = 6;

	public int control_figure_selected = -1;

	public const int flow_height = 60;

	public const int flow_width = 90;

	public const int control_height = 24;

	public const int control_width = 36;

	public const int control_X = 65;

	private PageSettings pgSettings = new PageSettings();

	private PrinterSettings prtSettings = new PrinterSettings();

	private int num_vert_pages;

	private int num_hor_pages;

	private int vert_counter;

	private int hor_counter;

	private int current_page;

	private bool first_time = true;

	private IEnumerator<Subchart> current_tab_enumerator;

	private int[] vert_page_breaks = new int[250];

	private int[] hor_page_breaks = new int[250];

	public int mouse_x;

	public int mouse_y;

	private TabPage tab_moving;

	private int tab_moving_index;

	private Rectangle ASGN;

	private Rectangle CALL;

	private Parallelogram INPUT;

	private Parallelogram OUTPUT;

	private IF_Control IFC;

	private Loop LP;

	private Oval_Return RETURN;

	public bool full_speed;

	private int x1;

	private int y1;

	private UMLupdater _umlupdater;

	public Component currentObj;

	public bool modified;

	private Guid file_guid_back = Guid.NewGuid();

	public Component clipboard;

	private string fileName;

	private int Timer_Frequency = 405;

	private Thread InstanceCaller;

	private bool Resetting;

	private ToolBar toolBar1;

	private Panel control_panel;

	private Splitter form_splitter;

	private Label label2;

	private MainMenu mainMenu1;

	private MenuItem menuItem8;

	private MenuItem menuItemPageSetup;

	private MenuItem menuItemPrintPreview;

	private MenuItem menuItemPrint;

	private MenuItem menuItem13;

	private MenuItem FileOpen;

	private MenuItem menuHelp;

	private MenuItem menuAbout;

	public MasterConsole MC;

	private bool the_runningState;

	public bool continuous_Run;

	internal TabPage running_tab;

	private MenuItem step;

	public ToolTip toolTip1;

	private IContainer components;

	private MenuItem menuSaveAs;

	private MenuItem menuReset;

	private MenuItem menuResetExecute;

	private ImageList imageList1;

	private ToolBarButton newButton;

	private ToolBarButton openButton;

	private ToolBarButton saveButton;

	private ToolBarButton cutButton;

	private ToolBarButton copyButton;

	private ToolBarButton pasteButton;

	private ToolBarButton printButton;

	public ToolBarButton undoButton;

	public ToolBarButton redoButton;

	private ToolBarButton playButton;

	private ToolBarButton pauseButton;

	private ToolBarButton stopButton;

	private ToolBarButton stepButton;

	private MenuItem menuExecute;

	private MenuItem menuPause;

	private TrackBar trackBar1;

	private TreeView watchBox;

	private ComboBox comboBox1;

	private MenuItem menuItem14;

	private MenuItem menuFile;

	public MenuItem menuMRU1;

	public MenuItem menuMRU2;

	public MenuItem menuMRU3;

	public MenuItem menuMRU4;

	private MenuItem menuItem1;

	private MenuItem menuItem25;

	private MenuItem menuScale;

	private MenuItem menuRun;

	private MenuItem menuAllText;

	private MenuItem menuTruncated;

	private MenuItem menuNoText;

	private PrintDocument printDoc;

	private MenuItem generalHelpMenu;

	private MenuItem menuScale125;

	private MenuItem menuScale100;

	private MenuItem menuScale80;

	private MenuItem menuScale60;

	private MenuItem menuScale40;

	public System.Windows.Forms.ContextMenu contextMenu1;

	private MenuItem menuItem21;

	private MenuItem menuItem22;

	private MenuItem menuItem23;

	public MenuItem menuItemUndo;

	public MenuItem menuItemRedo;

	private MenuItem menuItemComment;

	private MenuItem menuItemCut;

	private MenuItem menuItemCopy;

	private MenuItem menuItemPaste;

	private MenuItem menuItemDelete;

	private MenuItem menuEdit;

	private MenuItem contextMenuEdit;

	private MenuItem contextMenuComment;

	private MenuItem contextMenuCut;

	private MenuItem contextMenuCopy;

	private MenuItem contextMenuDelete;

	private MenuItem menuItem2;

	private MenuItem menuViewComments;

	private MenuItem menuView;

	private MenuItem menuItem3;

	private MenuItem menuViewVariables;

	private MenuItem menuItem4;

	private MenuItem menuShowLog;

	private MenuItem menuWindow;

	private MenuItem menuTileVertical;

	private MenuItem menuTileHorizontal;

	private MenuItem menuItemEditSelection;

	public System.Windows.Forms.ContextMenu contextMenuInsert;

	private MenuItem menuItemLoop;

	private MenuItem menuItemIf;

	public MenuItem contextMenu2Paste;

	private MenuItem menuItem5;

	private MenuItem menuItemCompile;

	private MenuItem menuItemAssignment;

	private MenuItem menuItemInput;

	private MenuItem menuItemCall;

	private MenuItem menuItemOutput;

	private MenuItem menuBreakpoint;

	public System.Windows.Forms.ContextMenu contextMenu2;

	private MenuItem menuItem7;

	private MenuItem menuClearBreakpoints;

	private MenuItem menuBreakpoint2;

	private MenuItem menuPrintClipboard;

	private MenuItem menuScale150;

	private MenuItem menuScale175;

	private MenuItem menuScale200;

	private MenuItem menuScale300;

	private System.Timers.Timer myTimer;

	private System.Timers.Timer loadTimer;

	private string load_filename;

	private float leftMargin;

	private float rightMargin;

	private float topMargin;

	private float bottomMargin;

	private float pageheight;

	private float pagewidth;

	private int drawing_height;

	private int drawing_width;

	private MenuItem menuItem6;

	private MenuItem menuSelectAll;

	private MenuItem menuExpandAll;

	private MenuItem menuCollapseAll;

	private MenuItem menuItem17;

	public TabControl carlisle;

	private TabPage tabPage1;

	private System.Windows.Forms.ContextMenu tabContextMenu;

	private MenuItem menuAddSubchart;

	private MenuItem menuDeleteSubchart;

	private MenuItem menuRenameSubchart;

	private MenuItem menuItemPrintScale;

	private MenuItem printScale60;

	private MenuItem printScale40;

	private MenuItem printScale80;

	private MenuItem printScale100;

	private MenuItem printScale125;

	private MenuItem printScale150;

	private MenuItem printScale200;

	private MenuItem printScale300;

	private MenuItem printScale175;

	private MenuItem menuItemRunCompiled;

	private MenuItem menuItem15;

	private MenuItem menuRunServer;

	private MenuItem menuItemSelectServer;

	private MenuItem menuGraphOnTop;

	private MenuItem menuProgramCompleteDialog;

	private MenuItem DefaultWindowSize;

	private MenuItem menuMode;

	private MenuItem menuNovice;

	private MenuItem menuIntermediate;

	private MenuItem menuAddFunction;

	private MenuItem menuAddProcedure;

	public MenuItem menuItemGenerate;

	private MenuItem menuGenerateStandalone;

	private MenuItem menuItemInk;

	public MenuItem menuItemInkOff;

	private MenuItem menuItem20;

	public MenuItem menuItemInkBlack;

	public MenuItem menuItemInkBlue;

	public MenuItem menuItemInkRed;

	private MenuItem menuItem19;

	public MenuItem menuItemInkErase;

	public MenuItem menuItemInkGreen;

	private MenuItem menuItem10;

	public MenuItem menuItemInkSelect;

	public MenuItem menuMRU5;

	public MenuItem menuMRU6;

	public MenuItem menuMRU7;

	public MenuItem menuMRU8;

	public MenuItem menuMRU9;

	private MenuItem menuViewHardDrive;

	private ToolBarButton testServerButton;

	private ToolBarButton InkButton1;

	private MenuItem menuObjectiveMode;

	private MenuItem menuItemReturn;

	private MenuItem menuCountSymbols;

	public static Start_Delegate_Type Start_delegate = Start_Delegate;

	public static Set_Current_Tab_Delegate_Type Set_Current_Tab_delegate = Set_Current_Tab_Delegate;

	public static Set_Running_Delegate_Type Set_Running_delegate = Set_Running_Delegate;

	public static Set_TopMost_Delegate_Type Set_TopMost_delegate = Set_TopMost_Delegate;

	public static MessageBox_Delegate_Type MessageBox_delegate = MessageBox_Delegate;

	public static updateScreen_Delegate_Type updateScreen_delegate = updateScreen;

	public static invalidateScreen_Delegate_Type invalidateScreen_delegate = invalidateScreen;

	public static Load_Delegate_Type Load_delegate = Load_Delegate;

	private Color Last_Ink_Color = Color.Green;

	internal Project projectCore => (carlisle.TabPages[0].Controls[0] as UMLDiagram).project;

	private UMLupdater umlupdater => _umlupdater;

	public int mainIndex
	{
		get
		{
			if (Component.Current_Mode == Mode.Expert)
			{
				return 1;
			}
			return 0;
		}
	}

	public Guid file_guid
	{
		get
		{
			return file_guid_back;
		}
		set
		{
			file_guid_back = value;
			if (Component.BARTPE)
			{
				MC.Text = file_guid_back.ToString().Substring(0, 8) + ": Console";
			}
		}
	}

	public Buffered flow_panel
	{
		get
		{
			if (carlisle.SelectedTab is Subchart)
			{
				return ((Subchart)carlisle.SelectedTab).flow_panel;
			}
			if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				return ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Subchart).flow_panel;
			}
			if (carlisle.TabPages.Count > 1 && carlisle.TabPages[1] is Subchart)
			{
				return ((Subchart)carlisle.TabPages[1]).flow_panel;
			}
			return null;
		}
	}

	public Panel panel1
	{
		get
		{
			if (carlisle.SelectedTab is Subchart)
			{
				return ((Subchart)carlisle.SelectedTab).panel1;
			}
			if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				return ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Subchart).panel1;
			}
			if (carlisle.TabPages.Count > 1 && carlisle.TabPages[1] is Subchart)
			{
				return ((Subchart)carlisle.TabPages[1]).panel1;
			}
			return null;
		}
	}

	public Point scroll_location
	{
		get
		{
			if (carlisle.SelectedTab is Subchart)
			{
				return ((Subchart)carlisle.SelectedTab).scroll_location;
			}
			if (carlisle.SelectedTab is ClassTabPage)
			{
				if ((carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
				{
					return ((Procedure_Chart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).scroll_location;
				}
				return ((Subchart)carlisle.TabPages[1]).scroll_location;
			}
			return ((Subchart)carlisle.TabPages[1]).scroll_location;
		}
		set
		{
			if (carlisle.SelectedTab is Subchart)
			{
				((Subchart)carlisle.SelectedTab).scroll_location = value;
			}
			else if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				((Subchart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).scroll_location = value;
			}
		}
	}

	public bool region_selected
	{
		get
		{
			if (carlisle.SelectedTab is Subchart)
			{
				return ((Subchart)carlisle.SelectedTab).region_selected;
			}
			if (carlisle.SelectedTab is ClassTabPage)
			{
				if ((carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
				{
					return ((Procedure_Chart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).region_selected;
				}
				return false;
			}
			return false;
		}
		set
		{
			if (carlisle.SelectedTab is Subchart)
			{
				((Subchart)carlisle.SelectedTab).region_selected = value;
			}
			else if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				((Subchart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).region_selected = value;
			}
		}
	}

	public Component Breakpoint_Selection
	{
		get
		{
			if (carlisle.SelectedTab is Subchart)
			{
				return ((Subchart)carlisle.SelectedTab).Breakpoint_Selection;
			}
			if (carlisle.SelectedTab is ClassTabPage)
			{
				if ((carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
				{
					return ((Procedure_Chart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).Breakpoint_Selection;
				}
				return null;
			}
			return null;
		}
		set
		{
			if (carlisle.SelectedTab is Subchart)
			{
				((Subchart)carlisle.SelectedTab).Breakpoint_Selection = value;
			}
			else if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				((Subchart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).Breakpoint_Selection = value;
			}
		}
	}

	public CommentBox selectedComment
	{
		get
		{
			if (carlisle.SelectedTab is Subchart)
			{
				return ((Subchart)carlisle.SelectedTab).selectedComment;
			}
			if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				return ((Subchart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).selectedComment;
			}
			return null;
		}
		set
		{
			if (carlisle.SelectedTab is Subchart)
			{
				((Subchart)carlisle.SelectedTab).selectedComment = value;
			}
			else if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				((Subchart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).selectedComment = value;
			}
		}
	}

	public Component Current_Selection
	{
		get
		{
			if (carlisle.SelectedTab is Subchart)
			{
				return ((Subchart)carlisle.SelectedTab).Current_Selection;
			}
			if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				return ((Subchart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).Current_Selection;
			}
			return null;
		}
		set
		{
			if (carlisle.SelectedTab is Subchart)
			{
				((Subchart)carlisle.SelectedTab).Current_Selection = value;
			}
			else if (carlisle.SelectedTab is ClassTabPage && (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				((Subchart)(carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab).Current_Selection = value;
			}
		}
	}

	public bool runningState
	{
		get
		{
			return the_runningState;
		}
		set
		{
			if (value)
			{
				menuEdit.Enabled = false;
				menuItem13.Enabled = false;
				FileOpen.Enabled = false;
				menuItem8.Enabled = false;
				menuSaveAs.Enabled = false;
				menuItemPageSetup.Enabled = false;
				menuItemPrintPreview.Enabled = false;
				menuItemPrint.Enabled = false;
			}
			else
			{
				menuEdit.Enabled = true;
				menuItem13.Enabled = true;
				FileOpen.Enabled = true;
				menuItem8.Enabled = true;
				menuSaveAs.Enabled = true;
				if (!Component.BARTPE)
				{
					menuItemPageSetup.Enabled = true;
					menuItemPrintPreview.Enabled = true;
					menuItemPrint.Enabled = true;
				}
			}
			the_runningState = value;
		}
	}

	public IEnumerable<ClassTabPage> allClasses
	{
		get
		{
			if (Component.Current_Mode == Mode.Expert)
			{
				for (int i = mainIndex + 1; i < carlisle.TabPages.Count; i++)
				{
					yield return carlisle.TabPages[i] as ClassTabPage;
				}
			}
		}
	}

	public IEnumerable<Subchart> allSubcharts => Compile_Helpers.allSubcharts(carlisle.TabPages);

	public Subchart mainSubchart()
	{
		if (Component.Current_Mode == Mode.Expert)
		{
			if (carlisle.TabPages.Count > 1)
			{
				return (Subchart)carlisle.TabPages[1];
			}
			return (Subchart)carlisle.TabPages[0];
		}
		return (Subchart)carlisle.TabPages[0];
	}

	public Visual_Flow_Form(object dummy)
	{
		InitializeComponent();
	}

	public static bool IsVisibleOnAnyScreen(System.Drawing.Rectangle rect)
	{
		Screen[] allScreens = Screen.AllScreens;
		for (int i = 0; i < allScreens.Length; i++)
		{
			if (allScreens[i].WorkingArea.IntersectsWith(rect))
			{
				return true;
			}
		}
		return false;
	}

	public Visual_Flow_Form(bool silent)
	{
		try
		{
			Component.BARTPE = false;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System").OpenSubKey("CurrentControlSet").OpenSubKey("Control");
			RegistryKey registryKey2 = registryKey.OpenSubKey("PE Builder");
			string text = (string)registryKey.GetValue("SystemStartOptions");
			if (registryKey2 != null || text.ToLower().Contains("minint"))
			{
				VerifyTestingEnvironment.VerifyEnvironment();
				Component.BARTPE = true;
			}
		}
		catch
		{
		}
		try
		{
			Component.VM = false;
		}
		catch
		{
		}
		try
		{
			PensBrushes.initialize();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
		InitializeComponent();
		_umlupdater = new UMLupdater(this);
		if (Registry_Settings.Global_Read("USMA_mode") != null)
		{
			Component.USMA_mode = true;
			Component.reverse_loop_logic = true;
		}
		else
		{
			try
			{
				if (Environment.UserDomainName.ToLower() == "usmaedu")
				{
					Component.USMA_mode = true;
					Component.reverse_loop_logic = true;
				}
				else
				{
					Component.USMA_mode = false;
					Component.reverse_loop_logic = false;
				}
			}
			catch
			{
				Component.USMA_mode = false;
				Component.reverse_loop_logic = false;
			}
		}
		if (Registry_Settings.Global_Read("reverse_loop_logic") != null)
		{
			Component.reverse_loop_logic = true;
		}
		switch (Registry_Settings.Read("UserMode"))
		{
		case "Novice":
			menuNovice_Click(null, null);
			break;
		case "Intermediate":
			menuIntermediate_Click(null, null);
			break;
		case "Expert":
			menuObjectiveMode_Click(null, null);
			break;
		}
		_ = Component.Current_Mode;
		_ = 2;
		tabPage1 = new Subchart(this, "main");
		carlisle.TabPages.Add(tabPage1);
		carlisle.SelectedTab = tabPage1;
		Create_Control_graphx();
		Create_Flow_graphx();
		ada_interpreter_pkg.adainit();
		interpreter_pkg.statement_syntax("x:=5", is_call_box: false, null);
		MC = new MasterConsole();
		file_guid = Guid.NewGuid();
		if (!silent)
		{
			MC.Show();
		}
		Runtime.Init(this, MC, watchBox);
		Runtime.Clear_Variables();
		log.Record_Open();
		switch (Registry_Settings.Read("TextView"))
		{
		case "NoText":
			menuNoText_Click(null, null);
			break;
		case "AllText":
			menuAllText_Click(null, null);
			break;
		}
		mainSubchart().Start.scale = scale;
		mainSubchart().Start.Scale(scale);
		flow_panel.Invalidate();
		switch (Registry_Settings.Read("ConsoleFontFamily"))
		{
		case "times":
			MC.menuTimes_Click(null, null);
			break;
		case "arial":
			MC.menuArial_Click(null, null);
			break;
		case "courier":
			MC.menuCourier_Click(null, null);
			break;
		}
		MC.Set_Font_Size();
		string text2 = Registry_Settings.Read("Scale");
		if (text2 != null)
		{
			try
			{
				comboBox1.SelectedIndex = int.Parse(text2) + 4;
			}
			catch
			{
				comboBox1.SelectedIndex = 6;
			}
		}
		else
		{
			comboBox1.SelectedIndex = 6;
		}
		string text3 = Registry_Settings.Read("PrintScale");
		if (text3 != null)
		{
			try
			{
				switch (int.Parse(text3))
				{
				case -4:
					PrintScale_300(null, null);
					break;
				case -3:
					PrintScale_200(null, null);
					break;
				case -2:
					PrintScale_175(null, null);
					break;
				case -1:
					PrintScale_150(null, null);
					break;
				case 0:
					PrintScale_100(null, null);
					break;
				case 1:
					PrintScale_80(null, null);
					break;
				case 2:
					PrintScale_60(null, null);
					break;
				case 3:
					PrintScale_40(null, null);
					break;
				case 4:
					PrintScale_20(null, null);
					break;
				case 5:
					break;
				}
			}
			catch
			{
				PrintScale_40(null, null);
			}
		}
		else
		{
			PrintScale_40(null, null);
		}
		string text4 = Registry_Settings.Read("Speed");
		if (text4 != null)
		{
			try
			{
				trackBar1.Value = int.Parse(text4);
			}
			catch
			{
			}
			trackBar1_Scroll(null, null);
		}
		string text5 = Registry_Settings.Read("ViewComments");
		if (text5 != null)
		{
			try
			{
				if (menuViewComments.Checked != bool.Parse(text5))
				{
					menuViewComments_Click(null, null);
				}
			}
			catch
			{
			}
		}
		string text6 = Registry_Settings.Read("ViewVariables");
		if (text6 != null)
		{
			try
			{
				if (menuViewVariables.Checked != bool.Parse(text6))
				{
					menuViewVariables_Click(null, null);
				}
			}
			catch
			{
			}
		}
		string text7 = Registry_Settings.Read("RAPTORGraphOnTop");
		if (text7 != null)
		{
			try
			{
				menuGraphOnTop.Checked = bool.Parse(text7);
				dotnetgraph.start_topmost = bool.Parse(text7);
			}
			catch
			{
			}
		}
		string text8 = Registry_Settings.Read("ProgramCompleteDialog");
		if (text8 != null)
		{
			try
			{
				menuProgramCompleteDialog.Checked = bool.Parse(text8);
			}
			catch
			{
			}
		}
		string text9 = Registry_Settings.Read("FormWidth");
		System.Drawing.Rectangle desktopBounds = base.DesktopBounds;
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		if (text9 != null)
		{
			try
			{
				num = int.Parse(text9);
			}
			catch
			{
			}
		}
		string text10 = Registry_Settings.Read("FormHeight");
		if (text10 != null)
		{
			try
			{
				num2 = int.Parse(text10);
			}
			catch
			{
				num2 = base.DesktopBounds.Height;
			}
		}
		else
		{
			num2 = base.DesktopBounds.Height;
		}
		string text11 = Registry_Settings.Read("FormX");
		if (text11 != null)
		{
			try
			{
				num3 = int.Parse(text11);
			}
			catch
			{
			}
		}
		string text12 = Registry_Settings.Read("FormY");
		if (text12 != null)
		{
			try
			{
				num4 = int.Parse(text12);
			}
			catch
			{
			}
		}
		if (IsVisibleOnAnyScreen(new System.Drawing.Rectangle(num3, num4, 20, 20)))
		{
			desktopBounds.X = num3;
			desktopBounds.Y = num4;
		}
		if (num > 570)
		{
			desktopBounds.Width = num;
		}
		if (num2 > 370)
		{
			desktopBounds.Height = num2;
		}
		SetDesktopBounds(desktopBounds.X, desktopBounds.Y, desktopBounds.Width, desktopBounds.Height);
		Plugins.Load_Plugins("");
		Generators.Load_Generators(this);
		if (Component.USMA_mode)
		{
			menuIntermediate_Click(null, null);
			menuItemAssignment.Text = "Insert &Process";
			menuItemCall.Text = "Insert &Flow transfer";
			menuItemLoop.Text = "Insert I&teration";
		}
		Invalidate();
		if (Component.MONO)
		{
			menuItemRunCompiled.Enabled = false;
			testServerButton.Enabled = false;
			menuItemSelectServer.Enabled = false;
			menuRunServer.Enabled = false;
			InkButton1.Enabled = false;
			InkButton1.Visible = false;
			menuPrintClipboard.Enabled = false;
			menuItemInk.Enabled = false;
		}
		if (Component.BARTPE && !Component.VM)
		{
			menuItemGenerate.Enabled = false;
			menuItemInk.Enabled = false;
			InkButton1.Enabled = false;
			InkButton1.Visible = false;
			menuItemCompile.Enabled = false;
			menuItemPageSetup.Enabled = false;
			menuItemPrint.Enabled = false;
			menuItemPrintPreview.Enabled = false;
			menuItemPrintScale.Enabled = false;
			menuPrintClipboard.Enabled = false;
			menuViewHardDrive.Visible = true;
		}
		starting_up = false;
		carlisle.SelectedTab = carlisle.TabPages[0];
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.Visual_Flow_Form));
		this.toolBar1 = new System.Windows.Forms.ToolBar();
		this.newButton = new System.Windows.Forms.ToolBarButton();
		this.openButton = new System.Windows.Forms.ToolBarButton();
		this.saveButton = new System.Windows.Forms.ToolBarButton();
		this.cutButton = new System.Windows.Forms.ToolBarButton();
		this.copyButton = new System.Windows.Forms.ToolBarButton();
		this.pasteButton = new System.Windows.Forms.ToolBarButton();
		this.printButton = new System.Windows.Forms.ToolBarButton();
		this.undoButton = new System.Windows.Forms.ToolBarButton();
		this.redoButton = new System.Windows.Forms.ToolBarButton();
		this.playButton = new System.Windows.Forms.ToolBarButton();
		this.pauseButton = new System.Windows.Forms.ToolBarButton();
		this.stopButton = new System.Windows.Forms.ToolBarButton();
		this.stepButton = new System.Windows.Forms.ToolBarButton();
		this.testServerButton = new System.Windows.Forms.ToolBarButton();
		this.InkButton1 = new System.Windows.Forms.ToolBarButton();
		this.contextMenu1 = new System.Windows.Forms.ContextMenu();
		this.contextMenuEdit = new System.Windows.Forms.MenuItem();
		this.contextMenuComment = new System.Windows.Forms.MenuItem();
		this.menuBreakpoint = new System.Windows.Forms.MenuItem();
		this.contextMenuCut = new System.Windows.Forms.MenuItem();
		this.contextMenuCopy = new System.Windows.Forms.MenuItem();
		this.contextMenuDelete = new System.Windows.Forms.MenuItem();
		this.imageList1 = new System.Windows.Forms.ImageList(this.components);
		this.form_splitter = new System.Windows.Forms.Splitter();
		this.control_panel = new System.Windows.Forms.Panel();
		this.watchBox = new System.Windows.Forms.TreeView();
		this.label2 = new System.Windows.Forms.Label();
		this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
		this.menuFile = new System.Windows.Forms.MenuItem();
		this.menuItem13 = new System.Windows.Forms.MenuItem();
		this.FileOpen = new System.Windows.Forms.MenuItem();
		this.menuItem8 = new System.Windows.Forms.MenuItem();
		this.menuSaveAs = new System.Windows.Forms.MenuItem();
		this.menuViewHardDrive = new System.Windows.Forms.MenuItem();
		this.menuItem5 = new System.Windows.Forms.MenuItem();
		this.menuItemCompile = new System.Windows.Forms.MenuItem();
		this.menuItem23 = new System.Windows.Forms.MenuItem();
		this.menuItemPageSetup = new System.Windows.Forms.MenuItem();
		this.menuItemPrintScale = new System.Windows.Forms.MenuItem();
		this.printScale40 = new System.Windows.Forms.MenuItem();
		this.printScale60 = new System.Windows.Forms.MenuItem();
		this.printScale80 = new System.Windows.Forms.MenuItem();
		this.printScale100 = new System.Windows.Forms.MenuItem();
		this.printScale125 = new System.Windows.Forms.MenuItem();
		this.printScale150 = new System.Windows.Forms.MenuItem();
		this.printScale175 = new System.Windows.Forms.MenuItem();
		this.printScale200 = new System.Windows.Forms.MenuItem();
		this.printScale300 = new System.Windows.Forms.MenuItem();
		this.menuItemPrintPreview = new System.Windows.Forms.MenuItem();
		this.menuItemPrint = new System.Windows.Forms.MenuItem();
		this.menuPrintClipboard = new System.Windows.Forms.MenuItem();
		this.menuItem1 = new System.Windows.Forms.MenuItem();
		this.menuMRU1 = new System.Windows.Forms.MenuItem();
		this.menuMRU2 = new System.Windows.Forms.MenuItem();
		this.menuMRU3 = new System.Windows.Forms.MenuItem();
		this.menuMRU4 = new System.Windows.Forms.MenuItem();
		this.menuMRU5 = new System.Windows.Forms.MenuItem();
		this.menuMRU6 = new System.Windows.Forms.MenuItem();
		this.menuMRU7 = new System.Windows.Forms.MenuItem();
		this.menuMRU8 = new System.Windows.Forms.MenuItem();
		this.menuMRU9 = new System.Windows.Forms.MenuItem();
		this.menuItem25 = new System.Windows.Forms.MenuItem();
		this.menuItem14 = new System.Windows.Forms.MenuItem();
		this.menuEdit = new System.Windows.Forms.MenuItem();
		this.menuItemUndo = new System.Windows.Forms.MenuItem();
		this.menuItemRedo = new System.Windows.Forms.MenuItem();
		this.menuItem21 = new System.Windows.Forms.MenuItem();
		this.menuItemComment = new System.Windows.Forms.MenuItem();
		this.menuItemEditSelection = new System.Windows.Forms.MenuItem();
		this.menuItem22 = new System.Windows.Forms.MenuItem();
		this.menuItemCut = new System.Windows.Forms.MenuItem();
		this.menuItemCopy = new System.Windows.Forms.MenuItem();
		this.menuItemPaste = new System.Windows.Forms.MenuItem();
		this.menuItemDelete = new System.Windows.Forms.MenuItem();
		this.menuItem6 = new System.Windows.Forms.MenuItem();
		this.menuSelectAll = new System.Windows.Forms.MenuItem();
		this.menuScale = new System.Windows.Forms.MenuItem();
		this.menuScale300 = new System.Windows.Forms.MenuItem();
		this.menuScale200 = new System.Windows.Forms.MenuItem();
		this.menuScale175 = new System.Windows.Forms.MenuItem();
		this.menuScale150 = new System.Windows.Forms.MenuItem();
		this.menuScale125 = new System.Windows.Forms.MenuItem();
		this.menuScale100 = new System.Windows.Forms.MenuItem();
		this.menuScale80 = new System.Windows.Forms.MenuItem();
		this.menuScale60 = new System.Windows.Forms.MenuItem();
		this.menuScale40 = new System.Windows.Forms.MenuItem();
		this.menuView = new System.Windows.Forms.MenuItem();
		this.menuAllText = new System.Windows.Forms.MenuItem();
		this.menuTruncated = new System.Windows.Forms.MenuItem();
		this.menuNoText = new System.Windows.Forms.MenuItem();
		this.menuItem2 = new System.Windows.Forms.MenuItem();
		this.menuViewComments = new System.Windows.Forms.MenuItem();
		this.menuItem3 = new System.Windows.Forms.MenuItem();
		this.menuViewVariables = new System.Windows.Forms.MenuItem();
		this.menuItem17 = new System.Windows.Forms.MenuItem();
		this.menuExpandAll = new System.Windows.Forms.MenuItem();
		this.menuCollapseAll = new System.Windows.Forms.MenuItem();
		this.menuRun = new System.Windows.Forms.MenuItem();
		this.step = new System.Windows.Forms.MenuItem();
		this.menuExecute = new System.Windows.Forms.MenuItem();
		this.menuReset = new System.Windows.Forms.MenuItem();
		this.menuResetExecute = new System.Windows.Forms.MenuItem();
		this.menuItemRunCompiled = new System.Windows.Forms.MenuItem();
		this.menuPause = new System.Windows.Forms.MenuItem();
		this.menuItem7 = new System.Windows.Forms.MenuItem();
		this.menuItemSelectServer = new System.Windows.Forms.MenuItem();
		this.menuRunServer = new System.Windows.Forms.MenuItem();
		this.menuItem15 = new System.Windows.Forms.MenuItem();
		this.menuClearBreakpoints = new System.Windows.Forms.MenuItem();
		this.menuMode = new System.Windows.Forms.MenuItem();
		this.menuNovice = new System.Windows.Forms.MenuItem();
		this.menuIntermediate = new System.Windows.Forms.MenuItem();
		this.menuObjectiveMode = new System.Windows.Forms.MenuItem();
		this.menuItemInk = new System.Windows.Forms.MenuItem();
		this.menuItemInkOff = new System.Windows.Forms.MenuItem();
		this.menuItem20 = new System.Windows.Forms.MenuItem();
		this.menuItemInkBlack = new System.Windows.Forms.MenuItem();
		this.menuItemInkBlue = new System.Windows.Forms.MenuItem();
		this.menuItemInkGreen = new System.Windows.Forms.MenuItem();
		this.menuItemInkRed = new System.Windows.Forms.MenuItem();
		this.menuItem19 = new System.Windows.Forms.MenuItem();
		this.menuItemInkErase = new System.Windows.Forms.MenuItem();
		this.menuItem10 = new System.Windows.Forms.MenuItem();
		this.menuItemInkSelect = new System.Windows.Forms.MenuItem();
		this.menuWindow = new System.Windows.Forms.MenuItem();
		this.menuProgramCompleteDialog = new System.Windows.Forms.MenuItem();
		this.menuGraphOnTop = new System.Windows.Forms.MenuItem();
		this.menuTileVertical = new System.Windows.Forms.MenuItem();
		this.menuTileHorizontal = new System.Windows.Forms.MenuItem();
		this.DefaultWindowSize = new System.Windows.Forms.MenuItem();
		this.menuItemGenerate = new System.Windows.Forms.MenuItem();
		this.menuGenerateStandalone = new System.Windows.Forms.MenuItem();
		this.menuHelp = new System.Windows.Forms.MenuItem();
		this.menuAbout = new System.Windows.Forms.MenuItem();
		this.generalHelpMenu = new System.Windows.Forms.MenuItem();
		this.menuItem4 = new System.Windows.Forms.MenuItem();
		this.menuShowLog = new System.Windows.Forms.MenuItem();
		this.menuCountSymbols = new System.Windows.Forms.MenuItem();
		this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
		this.trackBar1 = new System.Windows.Forms.TrackBar();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.printDoc = new System.Drawing.Printing.PrintDocument();
		this.contextMenuInsert = new System.Windows.Forms.ContextMenu();
		this.contextMenu2Paste = new System.Windows.Forms.MenuItem();
		this.menuItemAssignment = new System.Windows.Forms.MenuItem();
		this.menuItemCall = new System.Windows.Forms.MenuItem();
		this.menuItemInput = new System.Windows.Forms.MenuItem();
		this.menuItemOutput = new System.Windows.Forms.MenuItem();
		this.menuItemIf = new System.Windows.Forms.MenuItem();
		this.menuItemLoop = new System.Windows.Forms.MenuItem();
		this.menuItemReturn = new System.Windows.Forms.MenuItem();
		this.contextMenu2 = new System.Windows.Forms.ContextMenu();
		this.menuBreakpoint2 = new System.Windows.Forms.MenuItem();
		this.carlisle = new System.Windows.Forms.TabControl();
		this.tabContextMenu = new System.Windows.Forms.ContextMenu();
		this.menuAddSubchart = new System.Windows.Forms.MenuItem();
		this.menuAddFunction = new System.Windows.Forms.MenuItem();
		this.menuAddProcedure = new System.Windows.Forms.MenuItem();
		this.menuDeleteSubchart = new System.Windows.Forms.MenuItem();
		this.menuRenameSubchart = new System.Windows.Forms.MenuItem();
		this.control_panel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.trackBar1).BeginInit();
		base.SuspendLayout();
		this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[15]
		{
			this.newButton, this.openButton, this.saveButton, this.cutButton, this.copyButton, this.pasteButton, this.printButton, this.undoButton, this.redoButton, this.playButton,
			this.pauseButton, this.stopButton, this.stepButton, this.testServerButton, this.InkButton1
		});
		this.toolBar1.ButtonSize = new System.Drawing.Size(25, 25);
		this.toolBar1.DropDownArrows = true;
		this.toolBar1.ImageList = this.imageList1;
		this.toolBar1.Location = new System.Drawing.Point(0, 0);
		this.toolBar1.Name = "toolBar1";
		this.toolBar1.ShowToolTips = true;
		this.toolBar1.Size = new System.Drawing.Size(714, 32);
		this.toolBar1.TabIndex = 0;
		this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(toolBar1_ButtonClick);
		this.toolBar1.KeyDown += new System.Windows.Forms.KeyEventHandler(Visual_Flow_Form_KeyDown);
		this.newButton.ImageIndex = 2;
		this.newButton.Name = "newButton";
		this.newButton.ToolTipText = "New";
		this.openButton.ImageIndex = 3;
		this.openButton.Name = "openButton";
		this.openButton.ToolTipText = "Open";
		this.saveButton.ImageIndex = 6;
		this.saveButton.Name = "saveButton";
		this.saveButton.ToolTipText = "Save";
		this.cutButton.ImageIndex = 1;
		this.cutButton.Name = "cutButton";
		this.cutButton.ToolTipText = "Cut";
		this.copyButton.ImageIndex = 0;
		this.copyButton.Name = "copyButton";
		this.copyButton.ToolTipText = "Copy";
		this.pasteButton.ImageIndex = 4;
		this.pasteButton.Name = "pasteButton";
		this.pasteButton.ToolTipText = "Paste";
		this.printButton.ImageIndex = 16;
		this.printButton.Name = "printButton";
		this.printButton.ToolTipText = "Print";
		this.undoButton.ImageIndex = 14;
		this.undoButton.Name = "undoButton";
		this.undoButton.ToolTipText = "Undo";
		this.redoButton.ImageIndex = 15;
		this.redoButton.Name = "redoButton";
		this.redoButton.ToolTipText = "Redo";
		this.playButton.ImageIndex = 11;
		this.playButton.Name = "playButton";
		this.playButton.ToolTipText = "Execute to Completion";
		this.pauseButton.ImageIndex = 12;
		this.pauseButton.Name = "pauseButton";
		this.pauseButton.ToolTipText = "Pause";
		this.stopButton.ImageIndex = 13;
		this.stopButton.Name = "stopButton";
		this.stopButton.ToolTipText = "Stop/Reset";
		this.stepButton.ImageIndex = 10;
		this.stepButton.Name = "stepButton";
		this.stepButton.ToolTipText = "Step to Next Shape";
		this.testServerButton.ImageIndex = 17;
		this.testServerButton.Name = "testServerButton";
		this.testServerButton.ToolTipText = "Test against server";
		this.InkButton1.DropDownMenu = this.contextMenu1;
		this.InkButton1.ImageIndex = 18;
		this.InkButton1.Name = "InkButton1";
		this.InkButton1.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
		this.InkButton1.ToolTipText = "Toggle ink";
		this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[6] { this.contextMenuEdit, this.contextMenuComment, this.menuBreakpoint, this.contextMenuCut, this.contextMenuCopy, this.contextMenuDelete });
		this.contextMenu1.Popup += new System.EventHandler(contextMenu1_Popup);
		this.contextMenuEdit.Index = 0;
		this.contextMenuEdit.Text = "&Edit";
		this.contextMenuEdit.Click += new System.EventHandler(contextMenuEdit_Click);
		this.contextMenuComment.Index = 1;
		this.contextMenuComment.Text = "Co&mment";
		this.contextMenuComment.Click += new System.EventHandler(contextMenuComment_Click);
		this.menuBreakpoint.Index = 2;
		this.menuBreakpoint.Text = "Toggle &Breakpoint";
		this.menuBreakpoint.Click += new System.EventHandler(menuBreakpoint_Click);
		this.contextMenuCut.Index = 3;
		this.contextMenuCut.Text = "Cu&t";
		this.contextMenuCut.Click += new System.EventHandler(Cut_Click);
		this.contextMenuCopy.Index = 4;
		this.contextMenuCopy.Text = "&Copy";
		this.contextMenuCopy.Click += new System.EventHandler(Copy_Click);
		this.contextMenuDelete.Index = 5;
		this.contextMenuDelete.Text = "&Delete";
		this.contextMenuDelete.Click += new System.EventHandler(delete_Click);
		this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
		this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
		this.imageList1.Images.SetKeyName(0, "");
		this.imageList1.Images.SetKeyName(1, "");
		this.imageList1.Images.SetKeyName(2, "");
		this.imageList1.Images.SetKeyName(3, "");
		this.imageList1.Images.SetKeyName(4, "");
		this.imageList1.Images.SetKeyName(5, "");
		this.imageList1.Images.SetKeyName(6, "");
		this.imageList1.Images.SetKeyName(7, "");
		this.imageList1.Images.SetKeyName(8, "");
		this.imageList1.Images.SetKeyName(9, "");
		this.imageList1.Images.SetKeyName(10, "");
		this.imageList1.Images.SetKeyName(11, "");
		this.imageList1.Images.SetKeyName(12, "");
		this.imageList1.Images.SetKeyName(13, "");
		this.imageList1.Images.SetKeyName(14, "");
		this.imageList1.Images.SetKeyName(15, "");
		this.imageList1.Images.SetKeyName(16, "");
		this.imageList1.Images.SetKeyName(17, "test_server.bmp");
		this.imageList1.Images.SetKeyName(18, "pen.bmp");
		this.form_splitter.BackColor = System.Drawing.SystemColors.ScrollBar;
		this.form_splitter.Location = new System.Drawing.Point(200, 32);
		this.form_splitter.MinSize = 100;
		this.form_splitter.Name = "form_splitter";
		this.form_splitter.Size = new System.Drawing.Size(4, 450);
		this.form_splitter.TabIndex = 2;
		this.form_splitter.TabStop = false;
		this.control_panel.BackColor = System.Drawing.SystemColors.Control;
		this.control_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.control_panel.Controls.Add(this.watchBox);
		this.control_panel.Controls.Add(this.label2);
		this.control_panel.Dock = System.Windows.Forms.DockStyle.Left;
		this.control_panel.Location = new System.Drawing.Point(0, 32);
		this.control_panel.Name = "control_panel";
		this.control_panel.Size = new System.Drawing.Size(200, 450);
		this.control_panel.TabIndex = 0;
		this.control_panel.Paint += new System.Windows.Forms.PaintEventHandler(control_panel_Paint);
		this.control_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(select_control_Shape);
		this.watchBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.watchBox.Indent = 15;
		this.watchBox.Location = new System.Drawing.Point(0, 359);
		this.watchBox.Name = "watchBox";
		this.watchBox.Size = new System.Drawing.Size(196, 87);
		this.watchBox.TabIndex = 3;
		this.toolTip1.SetToolTip(this.watchBox, "Watch Window");
		this.watchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(Visual_Flow_Form_KeyDown);
		this.label2.AllowDrop = true;
		this.label2.BackColor = System.Drawing.Color.Transparent;
		this.label2.Dock = System.Windows.Forms.DockStyle.Top;
		this.label2.Font = new System.Drawing.Font("Times New Roman", 9.75f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Location = new System.Drawing.Point(0, 0);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(196, 359);
		this.label2.TabIndex = 1;
		this.label2.Text = "           Symbols";
		this.label2.DragOver += new System.Windows.Forms.DragEventHandler(label2_DragOver);
		this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(select_control_Shape);
		this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[10] { this.menuFile, this.menuEdit, this.menuScale, this.menuView, this.menuRun, this.menuMode, this.menuItemInk, this.menuWindow, this.menuItemGenerate, this.menuHelp });
		this.menuFile.Index = 0;
		this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[25]
		{
			this.menuItem13, this.FileOpen, this.menuItem8, this.menuSaveAs, this.menuViewHardDrive, this.menuItem5, this.menuItemCompile, this.menuItem23, this.menuItemPageSetup, this.menuItemPrintScale,
			this.menuItemPrintPreview, this.menuItemPrint, this.menuPrintClipboard, this.menuItem1, this.menuMRU1, this.menuMRU2, this.menuMRU3, this.menuMRU4, this.menuMRU5, this.menuMRU6,
			this.menuMRU7, this.menuMRU8, this.menuMRU9, this.menuItem25, this.menuItem14
		});
		this.menuFile.Text = "&File";
		this.menuFile.Popup += new System.EventHandler(menuFile_Click);
		this.menuItem13.Index = 0;
		this.menuItem13.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
		this.menuItem13.Text = "&New";
		this.menuItem13.Click += new System.EventHandler(new_clicked);
		this.FileOpen.Index = 1;
		this.FileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
		this.FileOpen.Text = "&Open";
		this.FileOpen.Click += new System.EventHandler(FileOpen_Click);
		this.menuItem8.Index = 2;
		this.menuItem8.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
		this.menuItem8.Text = "&Save";
		this.menuItem8.Click += new System.EventHandler(FileSave_Click);
		this.menuSaveAs.Index = 3;
		this.menuSaveAs.Text = "Save &As";
		this.menuSaveAs.Click += new System.EventHandler(SaveAs_Click);
		this.menuViewHardDrive.Index = 4;
		this.menuViewHardDrive.Text = "&View Hard Drive";
		this.menuViewHardDrive.Visible = false;
		this.menuViewHardDrive.Click += new System.EventHandler(menuViewHardDrive_Click);
		this.menuItem5.Index = 5;
		this.menuItem5.Text = "-";
		this.menuItemCompile.Index = 6;
		this.menuItemCompile.Text = "&Compile";
		this.menuItemCompile.Click += new System.EventHandler(menuItemCompile_Click);
		this.menuItem23.Index = 7;
		this.menuItem23.Text = "-";
		this.menuItemPageSetup.Index = 8;
		this.menuItemPageSetup.Text = "Page Setup";
		this.menuItemPageSetup.Click += new System.EventHandler(filePageSetupMenuItem_Click);
		this.menuItemPrintScale.Index = 9;
		this.menuItemPrintScale.MenuItems.AddRange(new System.Windows.Forms.MenuItem[9] { this.printScale40, this.printScale60, this.printScale80, this.printScale100, this.printScale125, this.printScale150, this.printScale175, this.printScale200, this.printScale300 });
		this.menuItemPrintScale.Text = "Print Scale";
		this.printScale40.Index = 0;
		this.printScale40.Text = "40%";
		this.printScale40.Click += new System.EventHandler(PrintScale_20);
		this.printScale60.Checked = true;
		this.printScale60.Index = 1;
		this.printScale60.Text = "60%";
		this.printScale60.Click += new System.EventHandler(PrintScale_40);
		this.printScale80.Index = 2;
		this.printScale80.Text = "80%";
		this.printScale80.Click += new System.EventHandler(PrintScale_60);
		this.printScale100.Index = 3;
		this.printScale100.Text = "100%";
		this.printScale100.Click += new System.EventHandler(PrintScale_80);
		this.printScale125.Index = 4;
		this.printScale125.Text = "125%";
		this.printScale125.Click += new System.EventHandler(PrintScale_100);
		this.printScale150.Index = 5;
		this.printScale150.Text = "150%";
		this.printScale150.Click += new System.EventHandler(PrintScale_150);
		this.printScale175.Index = 6;
		this.printScale175.Text = "175%";
		this.printScale175.Click += new System.EventHandler(PrintScale_175);
		this.printScale200.Index = 7;
		this.printScale200.Text = "200%";
		this.printScale200.Click += new System.EventHandler(PrintScale_200);
		this.printScale300.Index = 8;
		this.printScale300.Text = "300%";
		this.printScale300.Click += new System.EventHandler(PrintScale_300);
		this.menuItemPrintPreview.Index = 10;
		this.menuItemPrintPreview.Text = "Print Preview";
		this.menuItemPrintPreview.Click += new System.EventHandler(filePrintPreviewMenuItem_Click);
		this.menuItemPrint.Index = 11;
		this.menuItemPrint.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
		this.menuItemPrint.Text = "Print";
		this.menuItemPrint.Click += new System.EventHandler(filePrintMenuItem_Click);
		this.menuPrintClipboard.Index = 12;
		this.menuPrintClipboard.Text = "Print to Clipboard";
		this.menuPrintClipboard.Click += new System.EventHandler(menuPrintClipboard_Click);
		this.menuItem1.Index = 13;
		this.menuItem1.Text = "-";
		this.menuMRU1.Index = 14;
		this.menuMRU1.Text = "&1";
		this.menuMRU1.Click += new System.EventHandler(menuMRU1_Click);
		this.menuMRU2.Index = 15;
		this.menuMRU2.Text = "&2";
		this.menuMRU2.Click += new System.EventHandler(menuMRU2_Click);
		this.menuMRU3.Index = 16;
		this.menuMRU3.Text = "&3";
		this.menuMRU3.Click += new System.EventHandler(menuMRU3_Click);
		this.menuMRU4.Index = 17;
		this.menuMRU4.Text = "&4";
		this.menuMRU4.Click += new System.EventHandler(menuMRU4_Click);
		this.menuMRU5.Index = 18;
		this.menuMRU5.Text = "&5";
		this.menuMRU5.Click += new System.EventHandler(menuMRU5_Click);
		this.menuMRU6.Index = 19;
		this.menuMRU6.Text = "&6";
		this.menuMRU6.Click += new System.EventHandler(menuMRU6_Click);
		this.menuMRU7.Index = 20;
		this.menuMRU7.Text = "&7";
		this.menuMRU7.Click += new System.EventHandler(menuMRU7_Click);
		this.menuMRU8.Index = 21;
		this.menuMRU8.Text = "&8";
		this.menuMRU8.Click += new System.EventHandler(menuMRU8_Click);
		this.menuMRU9.Index = 22;
		this.menuMRU9.Text = "&9";
		this.menuMRU9.Click += new System.EventHandler(menuMRU9_Click);
		this.menuItem25.Index = 23;
		this.menuItem25.Text = "-";
		this.menuItem14.Index = 24;
		this.menuItem14.Text = "E&xit";
		this.menuItem14.Click += new System.EventHandler(exit_Click);
		this.menuEdit.Index = 1;
		this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[12]
		{
			this.menuItemUndo, this.menuItemRedo, this.menuItem21, this.menuItemComment, this.menuItemEditSelection, this.menuItem22, this.menuItemCut, this.menuItemCopy, this.menuItemPaste, this.menuItemDelete,
			this.menuItem6, this.menuSelectAll
		});
		this.menuEdit.Text = "&Edit";
		this.menuEdit.Popup += new System.EventHandler(menuEdit_Click);
		this.menuItemUndo.Index = 0;
		this.menuItemUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
		this.menuItemUndo.Text = "&Undo";
		this.menuItemUndo.Click += new System.EventHandler(Undo_Click);
		this.menuItemRedo.Enabled = false;
		this.menuItemRedo.Index = 1;
		this.menuItemRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
		this.menuItemRedo.Text = "&Redo";
		this.menuItemRedo.Click += new System.EventHandler(Redo_Click);
		this.menuItem21.Index = 2;
		this.menuItem21.Text = "-";
		this.menuItemComment.Index = 3;
		this.menuItemComment.Text = "Co&mment";
		this.menuItemComment.Click += new System.EventHandler(contextMenuComment_Click);
		this.menuItemEditSelection.Index = 4;
		this.menuItemEditSelection.Shortcut = System.Windows.Forms.Shortcut.F2;
		this.menuItemEditSelection.Text = "&Edit selection";
		this.menuItemEditSelection.Click += new System.EventHandler(menuItemEditSelection_Click);
		this.menuItem22.Index = 5;
		this.menuItem22.Text = "-";
		this.menuItemCut.Index = 6;
		this.menuItemCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
		this.menuItemCut.Text = "Cu&t";
		this.menuItemCut.Click += new System.EventHandler(Cut_Click);
		this.menuItemCopy.Index = 7;
		this.menuItemCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
		this.menuItemCopy.Text = "&Copy";
		this.menuItemCopy.Click += new System.EventHandler(Copy_Click);
		this.menuItemPaste.Index = 8;
		this.menuItemPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
		this.menuItemPaste.Text = "&Paste";
		this.menuItemPaste.Click += new System.EventHandler(paste_Click);
		this.menuItemDelete.Index = 9;
		this.menuItemDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
		this.menuItemDelete.Text = "&Delete";
		this.menuItemDelete.Click += new System.EventHandler(delete_Click);
		this.menuItem6.Index = 10;
		this.menuItem6.Text = "-";
		this.menuSelectAll.Index = 11;
		this.menuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
		this.menuSelectAll.Text = "Select &All";
		this.menuSelectAll.Click += new System.EventHandler(menuSelectAll_Click);
		this.menuScale.Index = 2;
		this.menuScale.MenuItems.AddRange(new System.Windows.Forms.MenuItem[9] { this.menuScale300, this.menuScale200, this.menuScale175, this.menuScale150, this.menuScale125, this.menuScale100, this.menuScale80, this.menuScale60, this.menuScale40 });
		this.menuScale.Text = "&Scale";
		this.menuScale300.Index = 0;
		this.menuScale300.Text = "300%";
		this.menuScale300.Click += new System.EventHandler(menuScale300_Click);
		this.menuScale200.Index = 1;
		this.menuScale200.Text = "200%";
		this.menuScale200.Click += new System.EventHandler(menuScale200_Click);
		this.menuScale175.Index = 2;
		this.menuScale175.Text = "175%";
		this.menuScale175.Click += new System.EventHandler(menuScale175_Click);
		this.menuScale150.Index = 3;
		this.menuScale150.Text = "150%";
		this.menuScale150.Click += new System.EventHandler(menuScale150_Click);
		this.menuScale125.Index = 4;
		this.menuScale125.Text = "125%";
		this.menuScale125.Click += new System.EventHandler(Scale_100);
		this.menuScale100.Checked = true;
		this.menuScale100.Index = 5;
		this.menuScale100.Text = "100%";
		this.menuScale100.Click += new System.EventHandler(Scale_80);
		this.menuScale80.Index = 6;
		this.menuScale80.Text = "80%";
		this.menuScale80.Click += new System.EventHandler(Scale_60);
		this.menuScale60.Index = 7;
		this.menuScale60.Text = "60%";
		this.menuScale60.Click += new System.EventHandler(Scale_40);
		this.menuScale40.Index = 8;
		this.menuScale40.Text = "40%";
		this.menuScale40.Click += new System.EventHandler(Scale_20);
		this.menuView.Index = 3;
		this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[10] { this.menuAllText, this.menuTruncated, this.menuNoText, this.menuItem2, this.menuViewComments, this.menuItem3, this.menuViewVariables, this.menuItem17, this.menuExpandAll, this.menuCollapseAll });
		this.menuView.Text = "&View";
		this.menuAllText.Checked = true;
		this.menuAllText.Index = 0;
		this.menuAllText.Text = "&All text";
		this.menuAllText.Click += new System.EventHandler(menuAllText_Click);
		this.menuTruncated.Index = 1;
		this.menuTruncated.Text = "&Truncated";
		this.menuTruncated.Click += new System.EventHandler(menuTruncated_Click);
		this.menuNoText.Index = 2;
		this.menuNoText.Text = "&No text";
		this.menuNoText.Click += new System.EventHandler(menuNoText_Click);
		this.menuItem2.Index = 3;
		this.menuItem2.Text = "-";
		this.menuViewComments.Checked = true;
		this.menuViewComments.Index = 4;
		this.menuViewComments.Text = "&Comments";
		this.menuViewComments.Click += new System.EventHandler(menuViewComments_Click);
		this.menuItem3.Index = 5;
		this.menuItem3.Text = "-";
		this.menuViewVariables.Checked = true;
		this.menuViewVariables.Index = 6;
		this.menuViewVariables.Text = "&Variables";
		this.menuViewVariables.Click += new System.EventHandler(menuViewVariables_Click);
		this.menuItem17.Index = 7;
		this.menuItem17.Text = "-";
		this.menuExpandAll.Index = 8;
		this.menuExpandAll.Text = "E&xpand all";
		this.menuExpandAll.Click += new System.EventHandler(menuExpandAll_Click);
		this.menuCollapseAll.Index = 9;
		this.menuCollapseAll.Text = "C&ollapse all";
		this.menuCollapseAll.Click += new System.EventHandler(menuCollapseAll_Click);
		this.menuRun.Index = 4;
		this.menuRun.MenuItems.AddRange(new System.Windows.Forms.MenuItem[11]
		{
			this.step, this.menuExecute, this.menuReset, this.menuResetExecute, this.menuItemRunCompiled, this.menuPause, this.menuItem7, this.menuItemSelectServer, this.menuRunServer, this.menuItem15,
			this.menuClearBreakpoints
		});
		this.menuRun.Text = "&Run";
		this.menuRun.Popup += new System.EventHandler(menuRun_Popup);
		this.step.Index = 0;
		this.step.Shortcut = System.Windows.Forms.Shortcut.F10;
		this.step.Text = "&Step";
		this.step.Click += new System.EventHandler(menuStep_Click);
		this.menuExecute.Index = 1;
		this.menuExecute.Text = "E&xecute to Completion";
		this.menuExecute.Click += new System.EventHandler(menuExecute_Click);
		this.menuReset.Index = 2;
		this.menuReset.Text = "&Reset";
		this.menuReset.Click += new System.EventHandler(menuReset_Click);
		this.menuResetExecute.Index = 3;
		this.menuResetExecute.Shortcut = System.Windows.Forms.Shortcut.F5;
		this.menuResetExecute.Text = "Reset/Execute";
		this.menuResetExecute.Click += new System.EventHandler(menuResetExecute_Click);
		this.menuItemRunCompiled.Index = 4;
		this.menuItemRunCompiled.Shortcut = System.Windows.Forms.Shortcut.CtrlF5;
		this.menuItemRunCompiled.Text = "R&un Compiled";
		this.menuItemRunCompiled.Click += new System.EventHandler(menuItemRunCompiled_Click);
		this.menuPause.Index = 5;
		this.menuPause.Text = "&Pause";
		this.menuPause.Click += new System.EventHandler(menuPause_Click);
		this.menuItem7.Index = 6;
		this.menuItem7.Text = "-";
		this.menuItemSelectServer.Index = 7;
		this.menuItemSelectServer.Text = "Se&lect server";
		this.menuItemSelectServer.Click += new System.EventHandler(ConfigServer_Click);
		this.menuRunServer.Index = 8;
		this.menuRunServer.Shortcut = System.Windows.Forms.Shortcut.F2;
		this.menuRunServer.Text = "Test against ser&ver";
		this.menuRunServer.Click += new System.EventHandler(menuRunServer_Click);
		this.menuItem15.Index = 9;
		this.menuItem15.Text = "-";
		this.menuClearBreakpoints.Index = 10;
		this.menuClearBreakpoints.Text = "&Clear all Breakpoints";
		this.menuClearBreakpoints.Click += new System.EventHandler(menuClearBreakpoints_Click);
		this.menuMode.Index = 5;
		this.menuMode.MenuItems.AddRange(new System.Windows.Forms.MenuItem[3] { this.menuNovice, this.menuIntermediate, this.menuObjectiveMode });
		this.menuMode.Text = "&Mode";
		this.menuNovice.Checked = true;
		this.menuNovice.Index = 0;
		this.menuNovice.Text = "&Novice";
		this.menuNovice.Click += new System.EventHandler(menuNovice_Click);
		this.menuIntermediate.Index = 1;
		this.menuIntermediate.Text = "&Intermediate";
		this.menuIntermediate.Click += new System.EventHandler(menuIntermediate_Click);
		this.menuObjectiveMode.Index = 2;
		this.menuObjectiveMode.Text = "&Object-oriented";
		this.menuObjectiveMode.Click += new System.EventHandler(menuObjectiveMode_Click);
		this.menuItemInk.Index = 6;
		this.menuItemInk.MenuItems.AddRange(new System.Windows.Forms.MenuItem[10] { this.menuItemInkOff, this.menuItem20, this.menuItemInkBlack, this.menuItemInkBlue, this.menuItemInkGreen, this.menuItemInkRed, this.menuItem19, this.menuItemInkErase, this.menuItem10, this.menuItemInkSelect });
		this.menuItemInk.Text = "&Ink";
		this.menuItemInkOff.Checked = true;
		this.menuItemInkOff.Index = 0;
		this.menuItemInkOff.Text = "&Off";
		this.menuItemInkOff.Click += new System.EventHandler(menuItemInkOff_Click);
		this.menuItem20.Index = 1;
		this.menuItem20.Text = "-";
		this.menuItemInkBlack.Index = 2;
		this.menuItemInkBlack.Text = "&Black";
		this.menuItemInkBlack.Click += new System.EventHandler(menuItemInkBlack_Click);
		this.menuItemInkBlue.Index = 3;
		this.menuItemInkBlue.Text = "Bl&ue";
		this.menuItemInkBlue.Click += new System.EventHandler(menuItemInkBlue_Click);
		this.menuItemInkGreen.Index = 4;
		this.menuItemInkGreen.Text = "&Green";
		this.menuItemInkGreen.Click += new System.EventHandler(menuItemInkGreen_Click);
		this.menuItemInkRed.Index = 5;
		this.menuItemInkRed.Text = "&Red";
		this.menuItemInkRed.Click += new System.EventHandler(menuItemInkRed_Click);
		this.menuItem19.Index = 6;
		this.menuItem19.Text = "-";
		this.menuItemInkErase.Index = 7;
		this.menuItemInkErase.Text = "&Eraser";
		this.menuItemInkErase.Click += new System.EventHandler(menuItemInkErase_Click);
		this.menuItem10.Index = 8;
		this.menuItem10.Text = "-";
		this.menuItemInkSelect.Index = 9;
		this.menuItemInkSelect.Text = "&Select";
		this.menuItemInkSelect.Click += new System.EventHandler(menuItemInkSelect_Click);
		this.menuWindow.Index = 7;
		this.menuWindow.MenuItems.AddRange(new System.Windows.Forms.MenuItem[5] { this.menuProgramCompleteDialog, this.menuGraphOnTop, this.menuTileVertical, this.menuTileHorizontal, this.DefaultWindowSize });
		this.menuWindow.Text = "&Window";
		this.menuProgramCompleteDialog.Index = 0;
		this.menuProgramCompleteDialog.Text = "Display \"Flowchart &Complete\" dialog";
		this.menuProgramCompleteDialog.Click += new System.EventHandler(menuProgramCompleteDialog_Click);
		this.menuGraphOnTop.Checked = true;
		this.menuGraphOnTop.Index = 1;
		this.menuGraphOnTop.Text = "Keep RAPTORGraph on &Top";
		this.menuGraphOnTop.Click += new System.EventHandler(menuGraphOnTop_Click);
		this.menuTileVertical.Index = 2;
		this.menuTileVertical.Text = "Tile &Vertical";
		this.menuTileVertical.Click += new System.EventHandler(menuTileVertical_Click);
		this.menuTileHorizontal.Index = 3;
		this.menuTileHorizontal.Text = "Tile &Horizontal";
		this.menuTileHorizontal.Click += new System.EventHandler(menuTileHorizontal_Click);
		this.DefaultWindowSize.Index = 4;
		this.DefaultWindowSize.Text = "&Default sizes";
		this.DefaultWindowSize.Click += new System.EventHandler(DefaultWindowSize_Click);
		this.menuItemGenerate.Index = 8;
		this.menuItemGenerate.MenuItems.AddRange(new System.Windows.Forms.MenuItem[1] { this.menuGenerateStandalone });
		this.menuItemGenerate.Text = "&Generate";
		this.menuItemGenerate.Popup += new System.EventHandler(menuItemGenerate_Popup);
		this.menuGenerateStandalone.Index = 0;
		this.menuGenerateStandalone.Text = "&Standalone";
		this.menuGenerateStandalone.Click += new System.EventHandler(menuGenerateStandalone_Click);
		this.menuHelp.Index = 9;
		this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[5] { this.menuAbout, this.generalHelpMenu, this.menuItem4, this.menuShowLog, this.menuCountSymbols });
		this.menuHelp.Text = "&Help";
		this.menuAbout.Index = 0;
		this.menuAbout.Text = "&About";
		this.menuAbout.Click += new System.EventHandler(menuAbout_Click);
		this.generalHelpMenu.Index = 1;
		this.generalHelpMenu.Shortcut = System.Windows.Forms.Shortcut.F1;
		this.generalHelpMenu.Text = "&General Help";
		this.generalHelpMenu.Click += new System.EventHandler(listOfFunctionsMenu_Click);
		this.menuItem4.Index = 2;
		this.menuItem4.Text = "-";
		this.menuShowLog.Index = 3;
		this.menuShowLog.Text = "&Show log";
		this.menuShowLog.Click += new System.EventHandler(menuShowLog_Click);
		this.menuCountSymbols.Index = 4;
		this.menuCountSymbols.Text = "&Count symbols";
		this.menuCountSymbols.Click += new System.EventHandler(menuCountSymbols_Click);
		this.trackBar1.AutoSize = false;
		this.trackBar1.BackColor = System.Drawing.Color.FromArgb(237, 232, 217);
		this.trackBar1.Location = new System.Drawing.Point(639, 3);
		this.trackBar1.Name = "trackBar1";
		this.trackBar1.Size = new System.Drawing.Size(168, 37);
		this.trackBar1.TabIndex = 4;
		this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
		this.toolTip1.SetToolTip(this.trackBar1, "Play Speed");
		this.trackBar1.Value = 6;
		this.trackBar1.Scroll += new System.EventHandler(trackBar1_Scroll);
		this.trackBar1.KeyDown += new System.Windows.Forms.KeyEventHandler(Visual_Flow_Form_KeyDown);
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.Items.AddRange(new object[9] { "300%", "200%", "175%", "150%", "125%", "100%", "80%", "60%", "40%" });
		this.comboBox1.Location = new System.Drawing.Point(819, 3);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(96, 33);
		this.comboBox1.TabIndex = 5;
		this.toolTip1.SetToolTip(this.comboBox1, "Scale");
		this.comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
		this.comboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(Visual_Flow_Form_KeyDown);
		this.printDoc.BeginPrint += new System.Drawing.Printing.PrintEventHandler(printDoc_BeginPrint);
		this.printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDoc_PrintPage);
		this.contextMenuInsert.MenuItems.AddRange(new System.Windows.Forms.MenuItem[8] { this.contextMenu2Paste, this.menuItemAssignment, this.menuItemCall, this.menuItemInput, this.menuItemOutput, this.menuItemIf, this.menuItemLoop, this.menuItemReturn });
		this.contextMenuInsert.Popup += new System.EventHandler(contextMenuInsert_Popup);
		this.contextMenu2Paste.Index = 0;
		this.contextMenu2Paste.Text = "Paste";
		this.contextMenu2Paste.Click += new System.EventHandler(paste_Click);
		this.menuItemAssignment.Index = 1;
		this.menuItemAssignment.Text = "Insert &Assignment";
		this.menuItemAssignment.Click += new System.EventHandler(menuItemAssignment_Click);
		this.menuItemCall.Index = 2;
		this.menuItemCall.Text = "Insert &Call";
		this.menuItemCall.Click += new System.EventHandler(menuItemCall_Click);
		this.menuItemInput.Index = 3;
		this.menuItemInput.Text = "Insert &Input";
		this.menuItemInput.Click += new System.EventHandler(menuItemParallelogram_Click);
		this.menuItemOutput.Index = 4;
		this.menuItemOutput.Text = "Insert &Output";
		this.menuItemOutput.Click += new System.EventHandler(menuItemOutput_Click);
		this.menuItemIf.Index = 5;
		this.menuItemIf.Text = "Insert &Selection";
		this.menuItemIf.Click += new System.EventHandler(menuItemIf_Click);
		this.menuItemLoop.Index = 6;
		this.menuItemLoop.Text = "Insert &Loop";
		this.menuItemLoop.Click += new System.EventHandler(menuItemLoop_Click);
		this.menuItemReturn.Index = 7;
		this.menuItemReturn.Text = "Insert &Return";
		this.menuItemReturn.Click += new System.EventHandler(menuItemReturn_Click);
		this.contextMenu2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[1] { this.menuBreakpoint2 });
		this.menuBreakpoint2.Index = 0;
		this.menuBreakpoint2.Text = "Toggle &Breakpoint";
		this.menuBreakpoint2.Click += new System.EventHandler(menuBreakpoint_Click);
		this.carlisle.Dock = System.Windows.Forms.DockStyle.Fill;
		this.carlisle.Location = new System.Drawing.Point(204, 32);
		this.carlisle.Name = "carlisle";
		this.carlisle.SelectedIndex = 0;
		this.carlisle.Size = new System.Drawing.Size(510, 450);
		this.carlisle.TabIndex = 6;
		this.carlisle.TabIndexChanged += new System.EventHandler(tabControl1_TabIndexChanged);
		this.carlisle.MouseDown += new System.Windows.Forms.MouseEventHandler(tabControl1_MouseDown);
		this.carlisle.MouseMove += new System.Windows.Forms.MouseEventHandler(tabControl1_MouseMove);
		this.carlisle.MouseUp += new System.Windows.Forms.MouseEventHandler(tabControl1_MouseUp);
		this.tabContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[5] { this.menuAddSubchart, this.menuAddFunction, this.menuAddProcedure, this.menuDeleteSubchart, this.menuRenameSubchart });
		this.tabContextMenu.Popup += new System.EventHandler(tabContextMenu_Popup);
		this.menuAddSubchart.Index = 0;
		this.menuAddSubchart.Text = "&Add subchart";
		this.menuAddSubchart.Click += new System.EventHandler(menuAddSubchart_Click);
		this.menuAddFunction.Index = 1;
		this.menuAddFunction.Text = "Add &function";
		this.menuAddFunction.Visible = false;
		this.menuAddProcedure.Index = 2;
		this.menuAddProcedure.Text = "Add &procedure";
		this.menuAddProcedure.Visible = false;
		this.menuAddProcedure.Click += new System.EventHandler(menuAddProcedure_Click);
		this.menuDeleteSubchart.Index = 3;
		this.menuDeleteSubchart.Text = "&Delete";
		this.menuDeleteSubchart.Click += new System.EventHandler(menuDeleteSubchart_Click);
		this.menuRenameSubchart.Index = 4;
		this.menuRenameSubchart.Text = "&Rename";
		this.menuRenameSubchart.Click += new System.EventHandler(menuRenameSubchart_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(9, 23);
		this.BackColor = System.Drawing.SystemColors.Window;
		base.ClientSize = new System.Drawing.Size(714, 482);
		base.Controls.Add(this.carlisle);
		base.Controls.Add(this.comboBox1);
		base.Controls.Add(this.trackBar1);
		base.Controls.Add(this.form_splitter);
		base.Controls.Add(this.control_panel);
		base.Controls.Add(this.toolBar1);
		this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Menu = this.mainMenu1;
		base.Name = "Visual_Flow_Form";
		base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
		this.Text = "Raptor";
		base.Activated += new System.EventHandler(Visual_Flow_Form_Activated);
		base.Closing += new System.ComponentModel.CancelEventHandler(Visual_Flow_Form_Closing);
		base.Deactivate += new System.EventHandler(Visual_Flow_Form_Deactivate);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(Visual_Flow_Form_KeyDown);
		base.Move += new System.EventHandler(Visual_Flow_Form_Move);
		base.Resize += new System.EventHandler(Visual_Flow_Form_Resize);
		this.control_panel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.trackBar1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	[STAThread]
	private static void Main(string[] args)
	{
		if (Autoupdate.Autoupdate_Requested())
		{
			return;
		}
		Visual_Flow_Form form;
		if (args != null && args.Length >= 1)
		{
			if (args.Length >= 2 && args[1].ToLower() == "/runsilent")
			{
				form = new Visual_Flow_Form(silent: false);
				commandLineRun(args, silent: true, form);
				return;
			}
			if (args.Length >= 2 && args[1] == "/run")
			{
				form = new Visual_Flow_Form(args.Length >= 4);
				commandLineRun(args, silent: false, form);
				return;
			}
			form = new Visual_Flow_Form(silent: false);
			form.loadTimer = new System.Timers.Timer(500.0);
			form.loadTimer.Elapsed += form.loader;
			form.load_filename = args[0];
			form.loadTimer.Start();
		}
		else
		{
			form = new Visual_Flow_Form(silent: false);
		}
		Application.Run(form);
	}

	private static void commandLineRun(string[] args, bool silent, Visual_Flow_Form form)
	{
		bool flag = false;
		bool compiled_flowchart = Component.compiled_flowchart;
		command_line_run = true;
		try
		{
			form.Load_File(args[0]);
			if (Component.compiled_flowchart)
			{
				throw new Exception("can't run compiled file from commandline");
			}
			Compile_Helpers.Compile_Flowchart(form.carlisle.TabPages);
		}
		catch (Exception)
		{
			flag = true;
		}
		form.full_speed = true;
		form.trackBar1.Value = form.trackBar1.Maximum;
		form.trackBar1_Scroll(null, null);
		form.menuReset_Click(null, null);
		Component.compiled_flowchart = true;
		try
		{
			if (silent)
			{
				ada_runtime_pkg.redirect_standard_input();
				command_line_input_redirect = true;
			}
			else if (args.Length >= 3)
			{
				ada_runtime_pkg.redirect_input(numbers_pkg.make_string_value(args[2]));
				command_line_input_redirect = true;
			}
		}
		catch
		{
            // MessageBox.Show("Failed reading: " + args[2]);
            Runtime.consoleWriteln("Failed reading: " + args[2]);
            Application.Exit();
		}
		try
		{
			if (silent)
			{
				ada_runtime_pkg.redirect_standard_output();
				command_line_output_redirect = true;
			}
			else if (args.Length >= 4)
			{
				ada_runtime_pkg.redirect_output(numbers_pkg.make_string_value(args[3]));
				command_line_output_redirect = true;
			}
		}
		catch
		{
            Runtime.consoleWriteln("Failed creating: " + args[2]);
            // MessageBox.Show("Failed creating: " + args[2]);
			Application.Exit();
		}
		try
		{
			if (!flag)
			{
				Compile_Helpers.Run_Compiled_NoThread(was_from_commandline: true);
				Component.compiled_flowchart = compiled_flowchart;
			}
			else
			{
				Runtime.consoleWriteln("Was unable to either load or compile file");
			}
		}
		catch (Exception ex2)
		{
			Runtime.consoleWriteln("Exception!: " + ex2.Message);
		}
		form.Close();
		if (command_line_output_redirect)
		{
			raptor_files_pkg.stop_redirect_output();
		}
		Application.Exit();
	}

	private void control_panel_Paint(object sender, PaintEventArgs e)
	{
		ASGN.selected = false;
		CALL.selected = false;
		INPUT.selected = false;
		OUTPUT.selected = false;
		IFC.selected = false;
		LP.selected = false;
		RETURN.selected = false;
		if (control_figure_selected == 0)
		{
			ASGN.selected = true;
		}
		else if (control_figure_selected == 1)
		{
			CALL.selected = true;
		}
		else if (control_figure_selected == 2)
		{
			INPUT.selected = true;
		}
		else if (control_figure_selected == 3)
		{
			OUTPUT.selected = true;
		}
		else if (control_figure_selected == 4)
		{
			IFC.selected = true;
		}
		else if (control_figure_selected == 5)
		{
			LP.selected = true;
		}
		else if (control_figure_selected == 6)
		{
			RETURN.selected = true;
		}
		ASGN.draw(e.Graphics, 65, 25);
		ASGN.X = 65;
		ASGN.Y = 25;
		if (!Component.USMA_mode)
		{
			e.Graphics.DrawString("Assignment", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 - ASGN.W / 2 - 15, 50));
		}
		else
		{
			e.Graphics.DrawString("Process", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 - ASGN.W / 2 - 5, 50));
		}
		CALL.Y = 68;
		if (Component.Current_Mode == Mode.Expert)
		{
			CALL.X = 65 - 3 * ASGN.W / 4;
			RETURN.X = 65 + 3 * ASGN.W / 4 + 3;
			RETURN.Y = CALL.Y;
			RETURN.draw(e.Graphics, RETURN.X, RETURN.Y);
			e.Graphics.DrawString("Return", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 + ASGN.W / 2 - 8, 93));
		}
		else
		{
			CALL.X = 65;
		}
		CALL.draw(e.Graphics, CALL.X, 68);
		if (!Component.USMA_mode)
		{
			e.Graphics.DrawString("Call", PensBrushes.times10, PensBrushes.blackbrush, new Point(CALL.X - 10, 93));
		}
		else
		{
			e.Graphics.DrawString("Flow transfer", PensBrushes.times10, PensBrushes.blackbrush, new Point(CALL.X - ASGN.W / 2 - 20, 93));
		}
		INPUT.draw(e.Graphics, 65 - 3 * ASGN.W / 4, 111);
		INPUT.X = 65 - 3 * ASGN.W / 4;
		INPUT.Y = 111;
		e.Graphics.DrawString("Input", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 - INPUT.W - 8, 136));
		OUTPUT.draw(e.Graphics, 65 + 3 * ASGN.W / 4 + 3, 111);
		OUTPUT.X = 65 + 3 * ASGN.W / 4 + 3;
		OUTPUT.Y = 111;
		e.Graphics.DrawString("Output", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 + ASGN.W / 2 - 8, 136));
		IFC.draw(e.Graphics, 65, 153);
		IFC.X = 65;
		IFC.Y = 153;
		e.Graphics.DrawString("Selection", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 - ASGN.W / 2 - 8, 168));
		LP.draw(e.Graphics, 65, 183);
		LP.X = 65;
		LP.Y = 183;
		if (!Component.USMA_mode)
		{
			e.Graphics.DrawString("Loop", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 - ASGN.W / 2 + 3, 218));
		}
		else
		{
			e.Graphics.DrawString("Iteration", PensBrushes.times10, PensBrushes.blackbrush, new Point(65 - ASGN.W / 2 - 8, 218));
		}
	}

	public void Create_Control_graphx()
	{
		ASGN = new Rectangle(24, 36, "Rectangle", Rectangle.Kind_Of.Assignment);
		CALL = new Rectangle(24, 36, "Rectangle", Rectangle.Kind_Of.Call);
		INPUT = new Parallelogram(24, 36, "Parallelogram", input: true);
		OUTPUT = new Parallelogram(24, 36, "Parallelogram", input: false);
		RETURN = new Oval_Return(24, 36, "Return");
		IFC = new IF_Control(9, 21, "IF_Control");
		LP = new Loop(9, 21, "Loop");
	}

	private void select_control_Shape(object sender, MouseEventArgs e)
	{
		mouse_x = e.X;
		mouse_y = e.Y;
		if (e.Button == MouseButtons.Left)
		{
			ASGN.selected = false;
			CALL.selected = false;
			INPUT.selected = false;
			OUTPUT.selected = false;
			IFC.selected = false;
			LP.selected = false;
			if (ASGN.In_Footprint(mouse_x, mouse_y))
			{
				control_figure_selected = 0;
				ASGN.selected = true;
				control_panel.Invalidate();
				DoDragDrop("raptor_ASGN", DragDropEffects.Copy | DragDropEffects.Link);
			}
			else if (CALL.In_Footprint(mouse_x, mouse_y))
			{
				control_figure_selected = 1;
				CALL.selected = true;
				control_panel.Invalidate();
				DoDragDrop("raptor_CALL", DragDropEffects.Copy | DragDropEffects.Link);
			}
			else if (INPUT.In_Footprint(mouse_x, mouse_y))
			{
				control_figure_selected = 2;
				INPUT.selected = true;
				control_panel.Invalidate();
				DoDragDrop("raptor_INPUT", DragDropEffects.Copy | DragDropEffects.Link);
			}
			else if (OUTPUT.In_Footprint(mouse_x, mouse_y))
			{
				control_figure_selected = 3;
				OUTPUT.selected = true;
				control_panel.Invalidate();
				DoDragDrop("raptor_OUTPUT", DragDropEffects.Copy | DragDropEffects.Link);
			}
			else if (IFC.In_Footprint(mouse_x, mouse_y))
			{
				control_figure_selected = 4;
				IFC.selected = true;
				control_panel.Invalidate();
				DoDragDrop("raptor_SELECTION", DragDropEffects.Copy | DragDropEffects.Link);
			}
			else if (LP.In_Footprint(mouse_x, mouse_y))
			{
				control_figure_selected = 5;
				LP.selected = true;
				control_panel.Invalidate();
				DoDragDrop("raptor_LOOP", DragDropEffects.Copy | DragDropEffects.Link);
			}
			else if (RETURN.In_Footprint(mouse_x, mouse_y))
			{
				control_figure_selected = 6;
				RETURN.selected = true;
				control_panel.Invalidate();
				DoDragDrop("raptor_RETURN", DragDropEffects.Copy | DragDropEffects.Link);
			}
			else
			{
				control_figure_selected = -1;
				control_panel.Invalidate();
			}
		}
	}

	public void Create_Flow_graphx()
	{
		Oval oval = new Oval(60, 90, "Oval");
		if (!Component.USMA_mode)
		{
			oval.Text = "End";
		}
		else
		{
			oval.Text = "Stop";
		}
		mainSubchart().Start = new Oval(oval, 60, 90, "Oval");
		mainSubchart().Start.Text = "Start";
		mainSubchart().Start.scale = scale;
		mainSubchart().Start.Scale(scale);
		Clear_Undo();
		modified = false;
	}

	public void my_layout()
	{
		int num = 0;
		int num2 = 0;
		Component successor = null;
		Subchart subchart;
		if (carlisle.SelectedTab is Subchart)
		{
			subchart = carlisle.SelectedTab as Subchart;
		}
		else
		{
			if (!(carlisle.SelectedTab is ClassTabPage) || (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count <= 0)
			{
				return;
			}
			subchart = (carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Subchart;
		}
		if (Component.compiled_flowchart)
		{
			successor = subchart.Start.Successor;
			subchart.Start.Successor = null;
		}
		if (subchart.Start != null)
		{
			System.Drawing.Rectangle rectangle = subchart.Start.comment_footprint();
			subchart.Start.footprint(CreateGraphics());
			num = subchart.Start.FP.height + 85;
			if (rectangle.Height > num)
			{
				num = rectangle.Height + 20;
			}
			num2 = subchart.Start.FP.left + subchart.Start.FP.right + 360;
			if (rectangle.Width > num2)
			{
				num2 = rectangle.Width + 20;
			}
		}
		if (Component.compiled_flowchart)
		{
			subchart.Start.Successor = successor;
		}
		if (num < panel1.Height)
		{
			num = panel1.Height;
		}
		if (num2 < panel1.Width)
		{
			num2 = panel1.Width;
		}
		if (num < panel1.Height)
		{
			num = panel1.Height;
		}
		if (subchart.tab_overlay != null && subchart.tab_overlay.Ink != null)
		{
			try
			{
				System.Drawing.Rectangle boundingBox = subchart.tab_overlay.Ink.GetBoundingBox();
				Graphics graphics = flow_panel.CreateGraphics();
				Point pt = new Point(boundingBox.Width, boundingBox.Height);
				subchart.tab_overlay.Renderer.InkSpaceToPixel(graphics, ref pt);
				graphics.Dispose();
				if (num2 < pt.X + 30)
				{
					num2 = pt.X + 30;
				}
				if (num < pt.Y + 30)
				{
					num = pt.Y + 30;
				}
			}
			catch
			{
			}
		}
		if (subchart.Start != null)
		{
			flow_panel.Height = num;
			flow_panel.Width = num2;
		}
	}

	private void filePrintMenuItem_Click(object sender, EventArgs e)
	{
		if (!Component.BARTPE)
		{
			printDoc.DefaultPageSettings = pgSettings;
			if (new PrintDialog
			{
				Document = printDoc,
				UseEXDialog = true
			}.ShowDialog() == DialogResult.OK)
			{
				menuExpandAll_Click(sender, e);
				printDoc.Print();
			}
		}
	}

	private void printDoc_BeginPrint(object sender, PrintEventArgs e)
	{
		current_page = 1;
		first_time = true;
		current_tab_enumerator = allSubcharts.GetEnumerator();
		current_tab_enumerator.MoveNext();
		rescale_all(print_scale);
	}

	private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
	{
		leftMargin = e.MarginBounds.Left;
		rightMargin = e.MarginBounds.Right;
		topMargin = e.MarginBounds.Top;
		bottomMargin = e.MarginBounds.Bottom;
		pageheight = e.MarginBounds.Height;
		pagewidth = e.MarginBounds.Width;
		Oval start = current_tab_enumerator.Current.Start;
		if (first_time)
		{
			start.select(-100, -100);
			start.footprint(CreateGraphics());
			start.scale = print_scale;
			current_page = 1;
			drawing_width = start.FP.left + start.FP.right + (int)Math.Round(leftMargin);
			drawing_height = start.FP.height;
			System.Drawing.Rectangle rectangle = start.comment_footprint();
			if (rectangle.Height > drawing_height)
			{
				drawing_height = rectangle.Height + 20;
			}
			if (rectangle.Width > drawing_width)
			{
				drawing_width = rectangle.Width + 20;
			}
		}
		x1 = (int)Math.Round((float)start.FP.left + leftMargin + 90f);
		int num = (int)Math.Round(leftMargin);
		y1 = (int)Math.Round(topMargin);
		int num2 = (int)Math.Round(bottomMargin - pageheight);
		int num3 = (int)Math.Round(rightMargin - pagewidth);
		int num4 = vert_page_breaks[current_page] - y1;
		int num5 = hor_page_breaks[current_page] - x1;
		int num6 = x1;
		int num7 = y1;
		if (first_time)
		{
			num_vertical_print_pages(e, start);
			num_horizontal_print_pages(e, start);
			first_time = false;
			hor_counter = 1;
			vert_counter = 1;
		}
		int num8 = hor_counter;
		int num9 = vert_counter;
		if (vert_counter == 1 && hor_counter == 1)
		{
			num6 = x1;
			num7 = y1;
			num4 = vert_page_breaks[1] - y1;
			num5 = hor_page_breaks[1] - num;
			if (num_hor_pages == 1 && num_vert_pages > 1)
			{
				vert_counter++;
			}
			else
			{
				hor_counter++;
			}
		}
		else
		{
			num7 = y1 - vert_page_breaks[vert_counter - 1];
			if (vert_counter > 1)
			{
				num7 += num2;
				num4 = vert_page_breaks[vert_counter] - vert_page_breaks[vert_counter - 1];
			}
			else
			{
				num4 = vert_page_breaks[1] - y1;
			}
			if (hor_counter == 1)
			{
				num6 = x1;
				num5 = hor_page_breaks[1] - num;
			}
			else
			{
				num6 = x1 + num3 - hor_page_breaks[hor_counter - 1];
				num5 = hor_page_breaks[hor_counter] - hor_page_breaks[hor_counter - 1];
			}
			if (hor_counter < num_hor_pages)
			{
				hor_counter++;
			}
			else
			{
				hor_counter = 1;
				vert_counter++;
			}
		}
		e.Graphics.SetClip(new RectangleF(leftMargin, topMargin, num5, num4));
		Subchart current = current_tab_enumerator.Current;
		if (current.tab_overlay != null && current.tab_overlay.Ink != null)
		{
			Matrix viewTransform = new Matrix();
			current.tab_overlay.Renderer.GetViewTransform(ref viewTransform);
			Matrix matrix = new Matrix();
			current.tab_overlay.Renderer.SetViewTransform(matrix);
			Point pt = new Point(50, 50);
			Point pt2 = new Point(100, 100);
			current.tab_overlay.Renderer.InkSpaceToPixel(e.Graphics, ref pt);
			current.tab_overlay.Renderer.InkSpaceToPixel(e.Graphics, ref pt2);
			Math.Abs(50.0 / (double)(pt2.X - pt.X));
			float num10 = (float)((double)(float)(num6 - start.FP.left) * current.ink_resolution);
			float num11 = (float)((double)(float)num7 * current.ink_resolution);
			matrix.Translate((float)((double)num10 - 0.0 * (double)print_scale), num11 - 790f * print_scale);
			matrix.Scale(print_scale, print_scale);
			current.tab_overlay.Renderer.SetViewTransform(matrix);
			current.tab_overlay.Renderer.Draw(e.Graphics, current.tab_overlay.Ink.Strokes);
			current.tab_overlay.Renderer.SetViewTransform(viewTransform);
		}
		Component.Inside_Print = true;
		start.draw(e.Graphics, num6, num7);
		Component.Inside_Print = false;
		Component.Just_After_Print = true;
		e.Graphics.DrawRectangle(PensBrushes.black_dash_pen, leftMargin, topMargin, num5, num4);
		RectangleF rectangleF = new RectangleF(leftMargin, bottomMargin, num5, 20f);
		e.Graphics.SetClip(rectangleF);
		if (num_hor_pages > 1)
		{
			e.Graphics.DrawString(Path.GetFileName(fileName) + "-" + current_tab_enumerator.Current.getFullName() + " : page " + num8 + "," + num9 + " : " + log.Last_Username() + " : " + log.Total_Minutes() + " mins, " + log.Count_Saves() + " saves", PensBrushes.default_times, PensBrushes.blackbrush, rectangleF, PensBrushes.centered_stringFormat);
		}
		else
		{
			e.Graphics.DrawString(Path.GetFileName(fileName) + "-" + current_tab_enumerator.Current.getFullName() + " : page " + num9 + " : " + log.Last_Username() + " : " + log.Total_Minutes() + " mins, " + log.Count_Saves() + " saves", PensBrushes.default_times, PensBrushes.blackbrush, rectangleF, PensBrushes.centered_stringFormat);
		}
		e.HasMorePages = hor_counter <= num_hor_pages && vert_counter <= num_vert_pages;
		if (!e.HasMorePages && current_tab_enumerator.MoveNext())
		{
			first_time = true;
			e.HasMorePages = true;
		}
		if (!e.HasMorePages)
		{
			rescale_all(scale);
		}
	}

	private void filePageSetupMenuItem_Click(object sender, EventArgs e)
	{
		PageSetupDialog pageSetupDialog = new PageSetupDialog();
		pageSetupDialog.PageSettings = pgSettings;
		pageSetupDialog.PrinterSettings = prtSettings;
		pageSetupDialog.AllowOrientation = true;
		pageSetupDialog.AllowMargins = true;
		pageSetupDialog.ShowDialog();
	}

	private void filePrintPreviewMenuItem_Click(object sender, EventArgs e)
	{
		printDoc.DefaultPageSettings = pgSettings;
		PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
		printPreviewDialog.Document = printDoc;
		printPreviewDialog.ShowDialog();
	}

	private void Copy_Click(object sender, EventArgs e)
	{
		if (!UML_Displayed() && (!(carlisle.SelectedTab is ClassTabPage) || (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count != 0))
		{
			if (selectedComment != null)
			{
				ClipboardMultiplatform.SetDataObject(new Clipboard_Data(selectedComment.Clone(), file_guid), afterExit: true);
			}
			else if ((carlisle.SelectedTab is Subchart && (carlisle.SelectedTab as Subchart).Start.copy(this)) || (carlisle.SelectedTab is ClassTabPage && ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Subchart).Start.copy(this)))
			{
				ClipboardMultiplatform.SetDataObject(new Clipboard_Data(clipboard, file_guid, log.Clone()), afterExit: true);
				control_figure_selected = -1;
				control_panel.Invalidate();
				flow_panel.Invalidate();
			}
		}
	}

	public void Undo_Click(object sender, EventArgs e)
	{
		Undo_Stack.Undo_Action(this);
	}

	public Subchart selectedTabMaybeNull()
	{
		if (carlisle.SelectedTab is Subchart)
		{
			return carlisle.SelectedTab as Subchart;
		}
		if (carlisle.SelectedTab is ClassTabPage)
		{
			if ((carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count > 0)
			{
				return (carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Subchart;
			}
			return null;
		}
		return null;
	}

	public void Make_Undoable()
	{
		Undo_Stack.Make_Undoable(this);
		modified = true;
		if (!((double)DateTime.Now.Subtract(last_autosave).Minutes >= 3.0))
		{
			return;
		}
		if (fileName != null && fileName != "")
		{
			Perform_Autosave();
			return;
		}
		Subchart subchart = selectedTabMaybeNull();
		if (subchart == null || !subchart.Am_Dragging)
		{
			MessageBox.Show("Please save now.", "Save your work");
			SaveAs_Click(null, null);
		}
	}

	private void Redo_Click(object sender, EventArgs e)
	{
		Undo_Stack.Redo_Action(this);
	}

	public void Clear_Undo()
	{
		Undo_Stack.Clear_Undo(this);
	}

	private void exit_Click(object sender, EventArgs e)
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		Visual_Flow_Form_Closing(sender, cancelEventArgs);
		if (!cancelEventArgs.Cancel)
		{
			Application.Exit();
		}
	}

	public string Get_Autosave_Name()
	{
		DateTime dateTime = DateTime.MaxValue;
		string result = fileName + ".backup0";
		try
		{
			char c = '0';
			while (c <= '3')
			{
				string text = fileName + ".backup" + c;
				if (System.IO.File.Exists(text))
				{
					DateTime lastWriteTime = System.IO.File.GetLastWriteTime(text);
					if (lastWriteTime < dateTime)
					{
						dateTime = lastWriteTime;
						result = text;
					}
					c = (char)(c + 1);
					continue;
				}
				return text;
			}
		}
		catch
		{
		}
		return result;
	}

	public void Perform_Autosave()
	{
		string autosave_Name = Get_Autosave_Name();
		Perform_Save(autosave_Name, is_autosave: true);
	}

	public void paste_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart == null)
		{
			return;
		}
		try
		{
			if (!Component.BARTPE && !Component.VM && !Component.MONO && subchart.tab_overlay.Ink.CanPaste())
			{
				Point pt = new Point(mouse_x, mouse_y);
				using (Graphics g = CreateGraphics())
				{
					subchart.tab_overlay.Renderer.PixelToInkSpace(g, ref pt);
				}
				subchart.tab_overlay.Ink.ClipboardPaste(pt);
				subchart.Refresh();
				return;
			}
		}
		catch (Exception)
		{
			MessageBox.Show("Please install the Microsoft.Ink.dll CLR 2.0 Update (KB900722)");
		}
		if (Current_Selection != null)
		{
			Clipboard_Data clipboard_Data = (Clipboard_Data)ClipboardMultiplatform.GetDataObject().GetData("raptor.Clipboard_Data");
			if (clipboard_Data != null && clipboard_Data.cb != null)
			{
				Make_Undoable();
				Current_Selection.My_Comment = clipboard_Data.cb.Clone();
				Current_Selection.My_Comment.parent = Current_Selection;
				Current_Selection.My_Comment.selected = false;
				flow_panel.Invalidate();
			}
			return;
		}
		Clipboard_Data clipboard_Data2 = (Clipboard_Data)ClipboardMultiplatform.GetDataObject().GetData("raptor.Clipboard_Data");
		if (clipboard_Data2 == null || clipboard_Data2.symbols == null)
		{
			return;
		}
		Make_Undoable();
		Component symbols = clipboard_Data2.symbols;
		int count_symbols = symbols.Count_Symbols();
		if (subchart.Start.insert(symbols, mouse_x, mouse_y, 0))
		{
			if (clipboard_Data2.guid != file_guid)
			{
				log.Record_Paste(clipboard_Data2.log, count_symbols, clipboard_Data2.guid);
			}
			mouse_x = 0;
			mouse_y = 0;
			my_layout();
			flow_panel.Invalidate();
		}
		else
		{
			Undo_Stack.Decrement_Undoable(this);
		}
	}

	private void delete_Click(object sender, EventArgs e)
	{
		if (UML_Displayed())
		{
			(carlisle.SelectedTab.Controls[0] as UMLDiagram).mnuDelete_Click(sender, e);
			return;
		}
		bool flag = false;
		Make_Undoable();
		Subchart subchart = selectedTabMaybeNull();
		if (subchart == null)
		{
			return;
		}
		if (!Component.BARTPE && !Component.VM && !Component.MONO && subchart.tab_overlay.Selection != null && subchart.tab_overlay.Selection.Count > 0)
		{
			subchart.tab_overlay.Ink.DeleteStrokes(subchart.tab_overlay.Selection);
			subchart.Refresh();
			return;
		}
		if (selectedComment != null)
		{
			flag = true;
			selectedComment.parent.My_Comment = null;
		}
		else if (subchart.Start.delete())
		{
			flag = true;
		}
		if (flag)
		{
			my_layout();
			flow_panel.Invalidate();
			mouse_x = 0;
			mouse_y = 0;
			Current_Selection = null;
			selectedComment = null;
		}
		else
		{
			Undo_Stack.Decrement_Undoable(this);
		}
	}

	public void Cut_Click(object sender, EventArgs e)
	{
		if (UML_Displayed())
		{
			return;
		}
		Make_Undoable();
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			if (!Component.BARTPE && !Component.VM && !Component.MONO && subchart.tab_overlay.Selection != null && subchart.tab_overlay.Selection.Count > 0)
			{
				subchart.tab_overlay.Ink.ClipboardCopy(subchart.tab_overlay.Selection, InkClipboardFormats.InkSerializedFormat, InkClipboardModes.Cut);
				subchart.Refresh();
			}
			else if (selectedComment != null)
			{
				ClipboardMultiplatform.SetDataObject(new Clipboard_Data(selectedComment, file_guid), afterExit: true);
				selectedComment.parent.My_Comment = null;
				flow_panel.Invalidate();
				selectedComment = null;
			}
			else if (subchart.Start.cut(this))
			{
				mouse_x = 0;
				mouse_y = 0;
				Current_Selection = null;
				ClipboardMultiplatform.SetDataObject(new Clipboard_Data(clipboard, file_guid, log.Clone()), afterExit: true);
				control_figure_selected = -1;
				my_layout();
				control_panel.Invalidate();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	private void Perform_Save(string name, bool is_autosave)
	{
		last_autosave = DateTime.Now;
		string text = ((!is_autosave) ? "Error during save:" : "Error during autosave:");
		Stream stream;
		try
		{
			stream = System.IO.File.Open(name, FileMode.Create);
		}
		catch
		{
			if (System.IO.File.Exists(name) && (System.IO.File.GetAttributes(name) & FileAttributes.ReadOnly) > (FileAttributes)0)
			{
				MessageBox.Show(text + "\n" + name + " is a read-only file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else
			{
				MessageBox.Show(text + "\nUnable to create file: " + name, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			Save_Error = true;
			return;
		}
		try
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(stream, Component.current_serialization_version);
			binaryFormatter.Serialize(stream, Component.reverse_loop_logic);
			if (Component.Current_Mode == Mode.Expert)
			{
				binaryFormatter.Serialize(stream, 2);
			}
			else
			{
				binaryFormatter.Serialize(stream, carlisle.TabCount);
			}
			if (Component.Current_Mode == Mode.Expert)
			{
				binaryFormatter.Serialize(stream, "UML");
				binaryFormatter.Serialize(stream, Subchart_Kinds.UML);
				binaryFormatter.Serialize(stream, carlisle.TabPages[mainIndex].Text);
				binaryFormatter.Serialize(stream, ((Subchart)carlisle.TabPages[mainIndex]).Subchart_Kind);
			}
			else
			{
				for (int i = mainIndex; i < carlisle.TabCount; i++)
				{
					binaryFormatter.Serialize(stream, carlisle.TabPages[i].Text);
					binaryFormatter.Serialize(stream, ((Subchart)carlisle.TabPages[i]).Subchart_Kind);
					if (((Subchart)carlisle.TabPages[i]) is Procedure_Chart)
					{
						binaryFormatter.Serialize(stream, ((Procedure_Chart)carlisle.TabPages[i]).num_params);
					}
				}
			}
			if (Component.Current_Mode == Mode.Expert)
			{
				BinarySerializationHelper.diagram = (carlisle.TabPages[0].Controls[0] as UMLDiagram).diagram;
				(carlisle.TabPages[0].Controls[0] as UMLDiagram).project.SaveBinary(binaryFormatter, stream);
				binaryFormatter.Serialize(stream, ((Subchart)carlisle.TabPages[mainIndex]).Start);
				byte[] graph = ((Component.BARTPE || Component.VM || Component.MONO) ? new byte[1] : ((Subchart)carlisle.TabPages[mainIndex]).tab_overlay.Ink.Save());
				binaryFormatter.Serialize(stream, graph);
				for (int j = mainIndex + 1; j < carlisle.TabCount; j++)
				{
					for (int k = 0; k < (carlisle.TabPages[j] as ClassTabPage).tabControl1.TabPages.Count; k++)
					{
						Subchart subchart = (carlisle.TabPages[j] as ClassTabPage).tabControl1.TabPages[k] as Subchart;
						binaryFormatter.Serialize(stream, subchart.Start);
						graph = ((Component.BARTPE || Component.VM || Component.MONO) ? new byte[1] : subchart.tab_overlay.Ink.Save());
						binaryFormatter.Serialize(stream, graph);
					}
				}
			}
			else
			{
				for (int l = mainIndex; l < carlisle.TabCount; l++)
				{
					binaryFormatter.Serialize(stream, ((Subchart)carlisle.TabPages[l]).Start);
					byte[] graph2 = ((Component.BARTPE || Component.VM || Component.MONO) ? new byte[1] : ((Subchart)carlisle.TabPages[l]).tab_overlay.Ink.Save());
					binaryFormatter.Serialize(stream, graph2);
				}
			}
			if (!is_autosave)
			{
				log.Record_Save();
			}
			else
			{
				log.Record_Autosave();
			}
			binaryFormatter.Serialize(stream, log);
			binaryFormatter.Serialize(stream, Component.compiled_flowchart);
			binaryFormatter.Serialize(stream, file_guid);
			stream.Close();
			Save_Error = false;
			if (!is_autosave)
			{
				modified = false;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(text + "\nPlease report to Martin.Carlisle@usafa.edu\nMeantime, try undo then save (keep doing undo until success)\nOr open an autosave file: " + fileName + ".[0-9]\nUse Alt-PrtSc and paste into email\n" + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Save_Error = true;
		}
		if (!Component.BARTPE)
		{
			return;
		}
		try
		{
			try
			{
				if (System.IO.File.Exists("c:\\hibernate.sys"))
				{
					MC.set_text("found hibernate.sys\n");
					System.IO.File.Delete("c:\\hibernate.sys");
				}
				if (System.IO.File.Exists("c:\\hiberfil.sys"))
				{
					MC.set_text("found hiberfil.sys\n");
					System.IO.File.Delete("c:\\hiberfil.sys");
				}
			}
			catch
			{
				MC.set_text("unable to delete hibernation file!\n");
			}
			string text2 = (Directory.Exists("d:\\program files (x86)\\raptor") ? "d:\\" : ((!Directory.Exists("c:\\")) ? "e:\\" : "c:\\"));
			try
			{
				if (!Directory.Exists(text2 + "program files (x86)"))
				{
					Directory.CreateDirectory(text2 + "\\program files (x86)");
				}
				if (!Directory.Exists(text2 + "program files (x86)\\raptor"))
				{
					Directory.CreateDirectory(text2 + "program files (x86)\\raptor");
				}
				if (!Directory.Exists(text2 + "program files (x86)\\raptor\\backup"))
				{
					Directory.CreateDirectory(text2 + "program files (x86)\\raptor\\backup");
				}
			}
			catch
			{
			}
			string text3 = fileName.ToLower().Replace(Component.BARTPE_ramdrive_path, text2 + "program files\\raptor\\backup\\") + ".enc";
			try
			{
				AES_Encrypt(text3.Replace(".enc", ".aes"));
			}
			catch
			{
				MessageBox.Show("failed to save to HD.");
			}
			text3 = fileName.ToLower().Replace(Component.BARTPE_ramdrive_path, Component.BARTPE_partition_path) + ".enc";
			try
			{
				AES_Encrypt(text3.Replace(".enc", ".aes"));
			}
			catch
			{
				MessageBox.Show("Failed to save to USB.  Contact your instructor!");
			}
		}
		catch
		{
			MessageBox.Show("Failed to save.  Contact your instructor!");
		}
	}

	public void AES_KeyHint(string input_file)
	{
		_ = new byte[256];
		FileStream fileStream = System.IO.File.Open(input_file, FileMode.Open, FileAccess.Read);
		_ = new byte[16];
		int num = fileStream.ReadByte();
		byte[] array = new byte[num];
		fileStream.Read(array, 0, num);
		BigInteger bigInteger = new BigInteger(array);
		fileStream.Close();
		MC.set_text("key hint is: " + bigInteger.ToHexString() + "\n");
	}

	public void AES_Decrypt(string input_file, string output_file, string hex_aes_key)
	{
		byte[] array = new byte[256];
		FileStream fileStream = System.IO.File.Open(output_file, FileMode.OpenOrCreate, FileAccess.Write);
		FileStream fileStream2 = System.IO.File.Open(input_file, FileMode.Open, FileAccess.Read);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.GenerateIV();
		rijndaelManaged.GenerateKey();
		byte[] array2 = new byte[16];
		int num = fileStream2.ReadByte();
		byte[] array3 = new byte[num];
		fileStream2.Read(array3, 0, num);
		new BigInteger(array3);
		BigInteger bigInteger = new BigInteger(hex_aes_key, 16);
		fileStream2.Read(array2, 0, rijndaelManaged.IV.Length);
		array3 = bigInteger.getBytes();
		byte[] array4 = new byte[16];
		for (int i = 0; i < 16; i++)
		{
			array4[i] = 0;
		}
		int num2 = 0;
		for (int i = 16 - array3.Length; i < 16; i++)
		{
			array4[i] = array3[num2++];
		}
		CryptoStream cryptoStream = null;
		try
		{
			ICryptoTransform transform = rijndaelManaged.CreateDecryptor(array4, array2);
			cryptoStream = new CryptoStream(fileStream2, transform, CryptoStreamMode.Read);
			int i;
			while ((i = cryptoStream.Read(array, 0, array.Length)) > 0)
			{
				fileStream.Write(array, 0, i);
			}
		}
		finally
		{
			fileStream.Close();
			fileStream2.Close();
			if (cryptoStream != null)
			{
				try
				{
					cryptoStream.Close();
				}
				catch
				{
				}
			}
		}
	}

	private void AES_Encrypt(string output_file)
	{
		byte[] array = new byte[256];
		MC.set_text("Saving AES encrypted file to: " + output_file + "\n");
		FileStream fileStream = System.IO.File.Open(output_file, FileMode.OpenOrCreate, FileAccess.Write);
		FileStream fileStream2 = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.GenerateIV();
		rijndaelManaged.GenerateKey();
		ICryptoTransform transform = rijndaelManaged.CreateEncryptor();
		byte[] bytes = new BigInteger(rijndaelManaged.Key).modPow(new BigInteger(65537L), new BigInteger("2729864799507477297863452061164390880807", 10)).getBytes();
		fileStream.WriteByte((byte)bytes.Length);
		fileStream.Write(bytes, 0, bytes.Length);
		fileStream.Write(rijndaelManaged.IV, 0, rijndaelManaged.IV.Length);
		fileStream.Flush();
		CryptoStream cryptoStream = new CryptoStream(fileStream, transform, CryptoStreamMode.Write);
		int count;
		while ((count = fileStream2.Read(array, 0, array.Length)) > 0)
		{
			cryptoStream.Write(array, 0, count);
		}
		fileStream2.Close();
		cryptoStream.FlushFinalBlock();
		cryptoStream.Close();
		fileStream.Close();
	}

	private void GPG_Encrypt(string output_file)
	{
		Process process = new Process();
		MC.set_text("Saving encrypted file to: " + output_file + "\n");
		process.StartInfo.FileName = Directory.GetParent(Application.ExecutablePath)?.ToString() + "\\gpg.exe";
		process.StartInfo.Arguments = "-r RAPTOR --trust-model always --output " + output_file + " --yes -e " + fileName;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.ErrorDialog = false;
		process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
		process.Start();
		process.WaitForExit();
		if (process.ExitCode != 0)
		{
			MessageBox.Show("failed to save to HD");
		}
	}

	private void FileSave_Click(object sender, EventArgs e)
	{
		if (fileName == "" || fileName == null)
		{
			SaveAs_Click(sender, e);
		}
		else
		{
			Perform_Save(fileName, is_autosave: false);
		}
	}

	private void Update_View_Variables()
	{
		string text = Registry_Settings.Read("ViewVariables");
		if (text != null && menuViewVariables.Checked != bool.Parse(text))
		{
			menuViewVariables_Click(null, null);
		}
	}

	private void Load_File(string dialog_fileName)
	{
		Stream stream;
		FileAttributes attributes;
		try
		{
			stream = System.IO.File.Open(dialog_fileName, FileMode.Open, FileAccess.Read);
			attributes = System.IO.File.GetAttributes(dialog_fileName);
		}
		catch
		{
			MessageBox.Show("Unable to open file: " + dialog_fileName);
			return;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		try
		{
			try
			{
				Component.warned_about_newer_version = false;
				Component.warned_about_error = false;
				Clear_Subcharts();
				try
				{
					Component.last_incoming_serialization_version = (int)binaryFormatter.Deserialize(stream);
					bool flag = Component.last_incoming_serialization_version >= 13 && (bool)binaryFormatter.Deserialize(stream);
					int num = (int)binaryFormatter.Deserialize(stream);
					for (int i = 0; i < num; i++)
					{
						string text = (string)binaryFormatter.Deserialize(stream);
						Subchart_Kinds subchart_Kinds = ((Component.last_incoming_serialization_version >= 14) ? ((Subchart_Kinds)binaryFormatter.Deserialize(stream)) : Subchart_Kinds.Subchart);
						if (i == 0 && subchart_Kinds != Subchart_Kinds.UML && Component.Current_Mode == Mode.Expert)
						{
							MessageBox.Show("Changing to Intermediate Mode");
							menuIntermediate_Click(null, null);
						}
						if (subchart_Kinds != 0)
						{
							if (Component.Current_Mode != Mode.Expert && subchart_Kinds == Subchart_Kinds.UML)
							{
								MessageBox.Show("Changing to Object-Oriented Mode");
								menuObjectiveMode_Click(null, null);
							}
							if (Component.Current_Mode == Mode.Novice)
							{
								MessageBox.Show("Changing to Intermediate Mode");
								menuIntermediate_Click(null, null);
							}
						}
						fileName = dialog_fileName;
						Plugins.Load_Plugins(fileName);
						if (i <= mainIndex)
						{
							continue;
						}
						int param_count = 0;
						switch (subchart_Kinds)
						{
						case Subchart_Kinds.Function:
							param_count = (int)binaryFormatter.Deserialize(stream);
							carlisle.TabPages.Add(new Procedure_Chart(this, text, param_count));
							break;
						case Subchart_Kinds.Procedure:
							if (Component.last_incoming_serialization_version >= 15)
							{
								param_count = (int)binaryFormatter.Deserialize(stream);
							}
							carlisle.TabPages.Add(new Procedure_Chart(this, text, param_count));
							break;
						case Subchart_Kinds.Subchart:
							carlisle.TabPages.Add(new Subchart(this, text));
							break;
						}
					}
					Component.negate_loops = false;
					if (Component.Current_Mode == Mode.Expert)
					{
						BinarySerializationHelper.diagram = (carlisle.TabPages[0].Controls[0] as UMLDiagram).diagram;
						(carlisle.TabPages[0].Controls[0] as UMLDiagram).project.LoadBinary(binaryFormatter, stream);
					}
					else if (flag != Component.reverse_loop_logic)
					{
						Component.negate_loops = true;
					}
					for (int j = mainIndex; j < num; j++)
					{
						((Subchart)carlisle.TabPages[j]).Start = (Oval)binaryFormatter.Deserialize(stream);
						((Subchart)carlisle.TabPages[j]).Start.scale = scale;
						((Subchart)carlisle.TabPages[j]).Start.Scale(scale);
						if (Component.last_incoming_serialization_version >= 17)
						{
							byte[] array = (byte[])binaryFormatter.Deserialize(stream);
							if (!Component.BARTPE && !Component.MONO && array.Length > 1)
							{
								bool enabled = ((Subchart)carlisle.TabPages[j]).tab_overlay.Enabled;
								((Subchart)carlisle.TabPages[j]).tab_overlay.Enabled = false;
								((Subchart)carlisle.TabPages[j]).tab_overlay.Ink = new Ink();
								((Subchart)carlisle.TabPages[j]).tab_overlay.Ink.Load(array);
								((Subchart)carlisle.TabPages[j]).tab_overlay.Enabled = enabled;
								((Subchart)carlisle.TabPages[j]).scale_ink(scale);
							}
							else if (((Subchart)carlisle.TabPages[j]).tab_overlay != null)
							{
								bool enabled2 = ((Subchart)carlisle.TabPages[j]).tab_overlay.Enabled;
								((Subchart)carlisle.TabPages[j]).tab_overlay.Enabled = false;
								((Subchart)carlisle.TabPages[j]).tab_overlay.Ink = new Ink();
								((Subchart)carlisle.TabPages[j]).tab_overlay.Enabled = enabled2;
								((Subchart)carlisle.TabPages[j]).scale_ink(scale);
							}
						}
						Current_Selection = ((Subchart)carlisle.TabPages[j]).Start.select(-1000, -1000);
					}
					carlisle.SelectedTab = mainSubchart();
				}
				catch (Exception)
				{
					fileName = dialog_fileName;
					Plugins.Load_Plugins(fileName);
					stream.Seek(0L, SeekOrigin.Begin);
					mainSubchart().Start = (Oval)binaryFormatter.Deserialize(stream);
					Component.last_incoming_serialization_version = mainSubchart().Start.incoming_serialization_version;
				}
				if (Component.Current_Mode == Mode.Expert)
				{
					for (int k = mainIndex + 1; k < carlisle.TabPages.Count; k++)
					{
						ClassTabPage classTabPage = carlisle.TabPages[k] as ClassTabPage;
						for (int l = 0; l < classTabPage.tabControl1.TabPages.Count; l++)
						{
							Subchart subchart = classTabPage.tabControl1.TabPages[l] as Subchart;
							subchart.Start = (Oval)binaryFormatter.Deserialize(stream);
							subchart.Start.scale = scale;
							subchart.Start.Scale(scale);
							byte[] array2 = (byte[])binaryFormatter.Deserialize(stream);
							if (!Component.BARTPE && array2.Length > 1)
							{
								bool enabled3 = subchart.tab_overlay.Enabled;
								subchart.tab_overlay.Enabled = false;
								subchart.tab_overlay.Ink = new Ink();
								subchart.tab_overlay.Ink.Load(array2);
								subchart.tab_overlay.Enabled = enabled3;
								subchart.scale_ink(scale);
							}
							else if (subchart.tab_overlay != null)
							{
								bool enabled4 = subchart.tab_overlay.Enabled;
								subchart.tab_overlay.Enabled = false;
								subchart.tab_overlay.Ink = new Ink();
								subchart.tab_overlay.Enabled = enabled4;
								subchart.scale_ink(scale);
							}
							Current_Selection = subchart.Start.select(-1000, -1000);
						}
					}
				}
				if (Component.last_incoming_serialization_version >= 4)
				{
					log = (logging_info)binaryFormatter.Deserialize(stream);
				}
				else
				{
					log.Clear();
				}
				if (Component.last_incoming_serialization_version >= 6)
				{
					Component.compiled_flowchart = (bool)binaryFormatter.Deserialize(stream);
				}
				else
				{
					Component.compiled_flowchart = false;
				}
				if (Component.last_incoming_serialization_version >= 8)
				{
					file_guid = (Guid)binaryFormatter.Deserialize(stream);
				}
				else
				{
					file_guid = Guid.NewGuid();
				}
				if (Component.compiled_flowchart)
				{
					Registry_Settings.Ignore_Updates = true;
					trackBar1.Value = trackBar1.Maximum;
					trackBar1_Scroll(null, null);
					if (menuViewVariables.Checked)
					{
						menuViewVariables_Click(null, null);
					}
					Registry_Settings.Ignore_Updates = false;
					Compile_Helpers.Compile_Flowchart(carlisle.TabPages);
				}
				if (Component.Current_Mode != Mode.Expert)
				{
					for (int m = mainIndex; m < carlisle.TabCount; m++)
					{
						((Subchart)carlisle.TabPages[m]).flow_panel.Invalidate();
					}
				}
				else
				{
					((Subchart)carlisle.TabPages[mainIndex]).flow_panel.Invalidate();
					for (int n = mainIndex + 1; n < carlisle.TabCount; n++)
					{
						for (int num2 = 0; num2 < (carlisle.TabPages[n] as ClassTabPage).tabControl1.TabCount; num2++)
						{
							((carlisle.TabPages[n] as ClassTabPage).tabControl1.TabPages[num2] as Subchart).flow_panel.Invalidate();
						}
					}
				}
				log.Record_Open();
				stream.Close();
			}
			catch
			{
				if (command_line_run)
				{
					stream.Close();
					return;
				}
				if (MessageBox.Show("Invalid File-not a flowchart, abort?\nRecommend selecting \"Yes\"\nIf you choose \"No\", RAPTOR may not function properly", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
				{
					new_clicked(null, null);
				}
				try
				{
					stream.Close();
					return;
				}
				catch
				{
					return;
				}
			}
			Update_View_Variables();
			Environment.CurrentDirectory = Path.GetDirectoryName(dialog_fileName);
			Runtime.Clear_Variables();
			runningState = false;
			MRU.Add_To_MRU_Registry(fileName);
			Text = My_Title + " - " + Path.GetFileName(fileName);
			if ((attributes & FileAttributes.ReadOnly) > (FileAttributes)0)
			{
				Text += " [Read-Only]";
			}
			modified = false;
			mainSubchart().Start.scale = scale;
			mainSubchart().Start.Scale(scale);
			Current_Selection = mainSubchart().Start.select(-1000, -1000);
			Clear_Undo();
			if (menuAllText.Checked)
			{
				menuAllText_Click(null, null);
			}
			else if (menuTruncated.Checked)
			{
				menuTruncated_Click(null, null);
			}
			else
			{
				menuNoText_Click(null, null);
			}
			Component.view_comments = menuViewComments.Checked;
			if (flow_panel.IsHandleCreated)
			{
				flow_panel.Invalidate();
			}
			MC.clear_txt();
		}
		catch (Exception ex2)
		{
			MessageBox.Show(ex2.Message + "\n" + ex2.StackTrace + "\nInvalid Filename:" + dialog_fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void FileOpen_BARTPE()
	{
		new BARTPEFileOpenList().ShowDialog();
		string filename = BARTPEFileOpenList.filename;
		if (filename != null)
		{
			Load_File(filename);
		}
	}

	private void FileOpen_Click(object sender, EventArgs e)
	{
		if (!Save_Before_Losing(sender, e))
		{
			return;
		}
		if (Component.BARTPE)
		{
			FileOpen_BARTPE();
			return;
		}
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Raptor files (*.rap)|*.rap|All files (*.*)|*.*";
		openFileDialog.CheckFileExists = true;
		openFileDialog.RestoreDirectory = false;
		if (openFileDialog.ShowDialog() != DialogResult.Cancel)
		{
			string text = openFileDialog.FileName;
			if (text == "" || text == null)
			{
				MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else
			{
				Load_File(text);
			}
		}
	}

	private void rescale_all(float scale_value)
	{
		foreach (Subchart allSubchart in allSubcharts)
		{
			allSubchart.scale_ink(scale_value);
			allSubchart.Start.scale = scale_value;
			allSubchart.Start.Scale(scale_value);
			allSubchart.flow_panel.Invalidate();
		}
	}

	private void menuScale300_Click(object sender, EventArgs e)
	{
		menuScale300.Checked = true;
		menuScale200.Checked = false;
		menuScale175.Checked = false;
		menuScale150.Checked = false;
		menuScale125.Checked = false;
		menuScale100.Checked = false;
		menuScale80.Checked = false;
		menuScale60.Checked = false;
		menuScale40.Checked = false;
		scale = 2.75f;
		comboBox1.SelectedIndex = 0;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "-4");
	}

	private void menuScale200_Click(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = true;
		menuScale175.Checked = false;
		menuScale150.Checked = false;
		menuScale125.Checked = false;
		menuScale100.Checked = false;
		menuScale80.Checked = false;
		menuScale60.Checked = false;
		menuScale40.Checked = false;
		scale = 1.75f;
		comboBox1.SelectedIndex = 1;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "-3");
	}

	private void menuScale175_Click(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = false;
		menuScale175.Checked = true;
		menuScale150.Checked = false;
		menuScale125.Checked = false;
		menuScale100.Checked = false;
		menuScale80.Checked = false;
		menuScale60.Checked = false;
		menuScale40.Checked = false;
		scale = 1.5f;
		comboBox1.SelectedIndex = 2;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "-2");
	}

	private void menuScale150_Click(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = false;
		menuScale175.Checked = false;
		menuScale150.Checked = true;
		menuScale125.Checked = false;
		menuScale100.Checked = false;
		menuScale80.Checked = false;
		menuScale60.Checked = false;
		menuScale40.Checked = false;
		scale = 1.25f;
		comboBox1.SelectedIndex = 3;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "-1");
	}

	private void Scale_100(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = false;
		menuScale175.Checked = false;
		menuScale150.Checked = false;
		menuScale125.Checked = true;
		menuScale100.Checked = false;
		menuScale80.Checked = false;
		menuScale60.Checked = false;
		menuScale40.Checked = false;
		scale = 1f;
		comboBox1.SelectedIndex = 4;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "0");
	}

	private void Scale_80(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = false;
		menuScale175.Checked = false;
		menuScale150.Checked = false;
		menuScale125.Checked = false;
		menuScale100.Checked = true;
		menuScale80.Checked = false;
		menuScale60.Checked = false;
		menuScale40.Checked = false;
		scale = 0.75f;
		comboBox1.SelectedIndex = 5;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "1");
	}

	private void Scale_60(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = false;
		menuScale175.Checked = false;
		menuScale150.Checked = false;
		menuScale125.Checked = false;
		menuScale100.Checked = false;
		menuScale80.Checked = true;
		menuScale60.Checked = false;
		menuScale40.Checked = false;
		scale = 0.6f;
		comboBox1.SelectedIndex = 6;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "2");
	}

	private void Scale_40(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = false;
		menuScale175.Checked = false;
		menuScale150.Checked = false;
		menuScale125.Checked = false;
		menuScale100.Checked = false;
		menuScale80.Checked = false;
		menuScale60.Checked = true;
		menuScale40.Checked = false;
		scale = 0.4f;
		comboBox1.SelectedIndex = 7;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "3");
	}

	private void Scale_20(object sender, EventArgs e)
	{
		menuScale300.Checked = false;
		menuScale200.Checked = false;
		menuScale175.Checked = false;
		menuScale150.Checked = false;
		menuScale125.Checked = false;
		menuScale100.Checked = false;
		menuScale80.Checked = false;
		menuScale60.Checked = false;
		menuScale40.Checked = true;
		scale = 0.2f;
		comboBox1.SelectedIndex = 8;
		rescale_all(scale);
		Registry_Settings.Write("Scale", "4");
	}

	private void PrintScale_300(object sender, EventArgs e)
	{
		printScale300.Checked = true;
		printScale200.Checked = false;
		printScale175.Checked = false;
		printScale150.Checked = false;
		printScale125.Checked = false;
		printScale100.Checked = false;
		printScale80.Checked = false;
		printScale60.Checked = false;
		printScale40.Checked = false;
		print_scale = 2.75f;
		Registry_Settings.Write("PrintScale", "-4");
	}

	private void PrintScale_200(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = true;
		printScale175.Checked = false;
		printScale150.Checked = false;
		printScale125.Checked = false;
		printScale100.Checked = false;
		printScale80.Checked = false;
		printScale60.Checked = false;
		printScale40.Checked = false;
		print_scale = 1.75f;
		Registry_Settings.Write("PrintScale", "-3");
	}

	private void PrintScale_175(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = false;
		printScale175.Checked = true;
		printScale150.Checked = false;
		printScale125.Checked = false;
		printScale100.Checked = false;
		printScale80.Checked = false;
		printScale60.Checked = false;
		printScale40.Checked = false;
		print_scale = 1.5f;
		Registry_Settings.Write("PrintScale", "-2");
	}

	private void PrintScale_150(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = false;
		printScale175.Checked = false;
		printScale150.Checked = true;
		printScale125.Checked = false;
		printScale100.Checked = false;
		printScale80.Checked = false;
		printScale60.Checked = false;
		printScale40.Checked = false;
		print_scale = 1.25f;
		Registry_Settings.Write("PrintScale", "-1");
	}

	private void PrintScale_100(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = false;
		printScale175.Checked = false;
		printScale150.Checked = false;
		printScale125.Checked = true;
		printScale100.Checked = false;
		printScale80.Checked = false;
		printScale60.Checked = false;
		printScale40.Checked = false;
		print_scale = 1f;
		Registry_Settings.Write("PrintScale", "0");
	}

	private void PrintScale_80(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = false;
		printScale175.Checked = false;
		printScale150.Checked = false;
		printScale125.Checked = false;
		printScale100.Checked = true;
		printScale80.Checked = false;
		printScale60.Checked = false;
		printScale40.Checked = false;
		print_scale = 0.75f;
		Registry_Settings.Write("PrintScale", "1");
	}

	private void PrintScale_60(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = false;
		printScale175.Checked = false;
		printScale150.Checked = false;
		printScale125.Checked = false;
		printScale100.Checked = false;
		printScale80.Checked = true;
		printScale60.Checked = false;
		printScale40.Checked = false;
		print_scale = 0.6f;
		Registry_Settings.Write("PrintScale", "2");
	}

	private void PrintScale_40(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = false;
		printScale175.Checked = false;
		printScale150.Checked = false;
		printScale125.Checked = false;
		printScale100.Checked = false;
		printScale80.Checked = false;
		printScale60.Checked = true;
		printScale40.Checked = false;
		print_scale = 0.4f;
		Registry_Settings.Write("PrintScale", "3");
	}

	private void PrintScale_20(object sender, EventArgs e)
	{
		printScale300.Checked = false;
		printScale200.Checked = false;
		printScale175.Checked = false;
		printScale150.Checked = false;
		printScale125.Checked = false;
		printScale100.Checked = false;
		printScale80.Checked = false;
		printScale60.Checked = false;
		printScale40.Checked = true;
		print_scale = 0.2f;
		Registry_Settings.Write("PrintScale", "4");
	}

	private bool Save_Before_Losing(object sender, EventArgs e)
	{
		DialogResult dialogResult = DialogResult.No;
		if (modified)
		{
			dialogResult = MessageBox.Show("Choosing this option will delete the current flow chart!\nDo you want to save first?", "Open New Chart", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
		}
		switch (dialogResult)
		{
		case DialogResult.Cancel:
			return false;
		case DialogResult.Yes:
			FileSave_Click(sender, e);
			while (Save_Error)
			{
				switch (MessageBox.Show("Save failed-- try again?", "Open New Chart", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
				{
				case DialogResult.Yes:
					SaveAs_Click(sender, e);
					break;
				case DialogResult.Cancel:
					return false;
				default:
					Save_Error = false;
					break;
				}
			}
			break;
		}
		return true;
	}

	private void Clear_Subcharts()
	{
		int num = ((Component.Current_Mode == Mode.Expert) ? 1 : 0);
		for (int num2 = carlisle.TabCount - 1; num2 > num; num2--)
		{
			carlisle.TabPages.RemoveAt(num2);
		}
		if (carlisle.TabPages.Count > 0)
		{
			carlisle.SelectedTab = mainSubchart();
		}
	}

	public void new_clicked(object sender, EventArgs e)
	{
		if (Save_Before_Losing(sender, e))
		{
			if (Component.Current_Mode == Mode.Expert)
			{
				(carlisle.TabPages[0].Controls[0] as UMLDiagram).project.ClearProject();
			}
			Clear_Subcharts();
			Create_Flow_graphx();
			Runtime.Clear_Variables();
			Component.compiled_flowchart = false;
			runningState = false;
			mainSubchart().Start.scale = scale;
			mainSubchart().Start.Scale(scale);
			flow_panel.Invalidate();
			Undo_Stack.Clear_Undo(this);
			Text = My_Title + "- Untitled";
			if (!Component.BARTPE && !Component.VM && !Component.MONO)
			{
				mainSubchart().tab_overlay.Ink.DeleteStrokes();
				mainSubchart().scale_ink(scale);
			}
			modified = false;
			fileName = null;
			Plugins.Load_Plugins("");
			log.Clear();
			log.Record_Open();
			file_guid = Guid.NewGuid();
			MC.clear_txt();
			Update_View_Variables();
		}
	}

	private void menuAbout_Click(object sender, EventArgs e)
	{
		new HelpForm().ShowDialog();
	}

	public static Oval Start_Delegate(Visual_Flow_Form f)
	{
		return f.mainSubchart().Start;
	}

	public static void Set_Current_Tab_Delegate(Visual_Flow_Form f, TabPage tb)
	{
		if (Component.Current_Mode != Mode.Expert)
		{
			f.carlisle.SelectedTab = tb;
			return;
		}
		if (f.carlisle.TabPages.Contains(tb))
		{
			f.carlisle.SelectedTab = tb;
			return;
		}
		for (int i = f.mainIndex + 1; i < f.carlisle.TabCount; i++)
		{
			if ((f.carlisle.TabPages[i] as ClassTabPage).tabControl1.TabPages.Contains(tb))
			{
				(f.carlisle.TabPages[i] as ClassTabPage).tabControl1.SelectedTab = tb;
				f.carlisle.SelectedTab = f.carlisle.TabPages[i];
				break;
			}
		}
	}

	public static void Set_Running_Delegate(Visual_Flow_Form f, bool v)
	{
		f.runningState = v;
	}

	public static void Set_TopMost_Delegate(Visual_Flow_Form f, bool v)
	{
		f.TopMost = v;
	}

	public static void MessageBox_Delegate(Visual_Flow_Form f, string text, string caption, MessageBoxIcon icon)
	{
		f.TopMost = true;
		Thread.Sleep(50);
		MessageBox.Show(f, text, caption, MessageBoxButtons.OK, icon);
		f.TopMost = false;
		f.MC.BringToFront();
	}

	public static void updateScreen(Visual_Flow_Form f)
	{
		if (f.currentObj != null)
		{
			Point autoScrollPosition = f.panel1.AutoScrollPosition;
			if (autoScrollPosition.Y < 0)
			{
				autoScrollPosition.Y = -autoScrollPosition.Y;
			}
			int num = f.currentObj.X;
			int diamond_top = f.currentObj.Y;
			if (f.currentObj.Name == "Loop" && !((Loop)f.currentObj).light_head)
			{
				diamond_top = ((Loop)f.currentObj).diamond_top;
			}
			diamond_top = ((autoScrollPosition.Y >= diamond_top || diamond_top >= autoScrollPosition.Y + f.panel1.Height - 20) ? (diamond_top - f.panel1.Height / 2) : autoScrollPosition.Y);
			num = ((autoScrollPosition.X >= num || num >= autoScrollPosition.X + f.panel1.Width - 20) ? (num - f.panel1.Width / 2) : autoScrollPosition.X);
			f.panel1.AutoScrollPosition = new Point(num, diamond_top);
			f.scroll_location = f.panel1.AutoScrollPosition;
		}
		f.flow_panel.Invalidate();
	}

	public static void invalidateScreen(Visual_Flow_Form f)
	{
		f.flow_panel.Invalidate();
	}

	private int firstSubchart()
	{
		if (Component.Current_Mode != Mode.Expert)
		{
			return 0;
		}
		return 1;
	}

	private void Clear_Selections()
	{
		foreach (Subchart allSubchart in allSubcharts)
		{
			allSubchart.Start.select(-1000, -1000);
		}
	}

	private void Clear_Expression_Counts()
	{
		foreach (Subchart allSubchart in allSubcharts)
		{
			allSubchart.Start.reset_number_method_expressions_run();
		}
	}

	private bool Has_Code()
	{
		bool result = true;
		foreach (Subchart allSubchart in allSubcharts)
		{
			if (!allSubchart.Start.has_code())
			{
				allSubchart.Start.mark_error();
				allSubchart.flow_panel.Invalidate();
				result = false;
			}
		}
		return result;
	}

	public Subchart Find_Tab(string s)
	{
		int num = s.IndexOf('(');
		string text = ((num <= 0) ? s.Trim() : s.Substring(0, num).Trim());
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			if (carlisle.TabPages[i].Text.ToLower() == text.ToLower() && carlisle.TabPages[i] is Subchart)
			{
				return (Subchart)carlisle.TabPages[i];
			}
		}
		throw new Exception("can't find procedure or subchart: " + s);
	}

	public ClassTabPage currentClass()
	{
		return findClass(running_tab);
	}

	public void Possible_Tab_Update(TabPage tb)
	{
		if (!full_speed || !continuous_Run)
		{
			Set_Current_Tab_Using_Delegate(tb);
		}
	}

	private void menuStep()
	{
		try
		{
			if (Component.compiled_flowchart)
			{
				if (!runningState)
				{
					try
					{
						Compile_Helpers.Run_Compiled(was_from_commandline: false);
						return;
					}
					catch (Exception ex)
					{
						MessageBox.Show("Flowchart terminated abnormally\n" + ex.ToString());
						return;
					}
				}
				return;
			}
			if (!runningState)
			{
				object[] array = new object[2];
				currentObj = null;
				symbol_count = 0;
				Runtime.Clear_Variables();
				numbers_pkg.set_precision(-1);
				array[0] = this;
				array[1] = true;
				Invoke(Set_Running_delegate, array);
				Clear_Selections();
				Clear_Expression_Counts();
				if (!Has_Code())
				{
					string text = "Before you can execute this flow chart, you need to add code to the the symbols with the red error message";
					Invoke(args: new object[4]
					{
						this,
						text,
						"Error",
						MessageBoxIcon.Exclamation
					}, method: MessageBox_delegate);
					array[0] = this;
					array[1] = false;
					Invoke(Set_Running_delegate, array);
					continuous_Run = false;
					if (myTimer != null)
					{
						myTimer.Stop();
					}
					return;
				}
				Runtime.createStaticVariables();
				running_tab = mainSubchart();
				Set_Current_Tab_Using_Delegate(mainSubchart());
			}
			Runtime.Clear_Updated();
			control_figure_selected = -1;
			control_panel.Invalidate();
			_ = currentObj;
			do
			{
				symbol_count++;
				if (currentObj == null)
				{
					currentObj = (Oval)Invoke(args: new object[1] { this }, method: Start_delegate);
					currentObj.running = true;
					continue;
				}
				Component component = currentObj;
				if (currentObj is Rectangle && currentObj.parse_tree is method_proc_call)
				{
					CallStack.Push(currentObj, (Subchart)running_tab);
					Step_Helpers.Set_State_Entering();
					interpreter_pkg.run_assignment(currentObj.parse_tree, currentObj.Text);
					Possible_Tab_Update(running_tab);
					currentObj = ((Subchart)running_tab).Start;
					currentObj.running = true;
				}
				else if (currentObj.Name == "Rectangle" && Is_Subchart_Call(currentObj.Text))
				{
					CallStack.Push(currentObj, (Subchart)running_tab);
					TabPage tabPage = (running_tab = Find_Tab(currentObj.Text));
					if (running_tab is Procedure_Chart)
					{
						Step_Helpers.Set_State_Entering();
						interpreter_pkg.run_assignment(currentObj.parse_tree, currentObj.Text);
					}
					Possible_Tab_Update(tabPage);
					currentObj = ((Subchart)tabPage).Start;
					currentObj.running = true;
				}
				else
				{
					currentObj = Step_Helpers.Step_Once(currentObj, this);
					component.running = false;
					if (currentObj == null && CallStack.Count() == 0)
					{
						object[] array2 = new object[2] { this, false };
						Invoke(Set_Running_delegate, array2);
						if (menuProgramCompleteDialog.Checked)
						{
							array2[0] = this;
							array2[1] = true;
							Invoke(Set_TopMost_delegate, array2);
							Thread.Sleep(10);
							Invoke(args: new object[4]
							{
								this,
								"Flowchart complete.",
								"End of algorithm",
								MessageBoxIcon.None
							}, method: MessageBox_delegate);
							array2[0] = this;
							array2[1] = false;
							Invoke(Set_TopMost_delegate, array2);
						}
						continuous_Run = false;
						MC.program_stopped("Run complete.  " + (symbol_count - 1) + " symbols evaluated.");
						if (command_line_run)
						{
							exit_Click(null, null);
						}
					}
					else if (currentObj == null)
					{
						CallStack.StackFrame stackFrame = CallStack.Top();
						CallStack.Pop();
						Step_Helpers.call_rect = stackFrame.obj;
						Step_Helpers.end_oval = (running_tab as Subchart).Start.find_end();
						Step_Helpers.next_chart = stackFrame.code;
						Runtime.setContext(stackFrame.context);
						Step_Helpers.Set_State_Leaving();
						if (!(stackFrame.obj is Rectangle) || (stackFrame.obj as Rectangle).kind != Rectangle.Kind_Of.Call)
						{
							stackFrame.obj.need_to_decrease_scope = true;
						}
						currentObj = Step_Helpers.Step_Once(stackFrame.obj, this);
						currentObj.running = true;
						running_tab = stackFrame.code;
						Possible_Tab_Update(running_tab);
					}
					else
					{
						currentObj.running = true;
					}
				}
				if (continuous_Run && currentObj != null && currentObj.break_now())
				{
					Set_Current_Tab_Using_Delegate(running_tab);
					continuous_Run = false;
				}
			}
			while (continuous_Run && full_speed);
			if (!full_speed || !continuous_Run)
			{
				Invoke(args: new object[1] { this }, method: updateScreen_delegate);
			}
			if (continuous_Run)
			{
				myTimer.Start();
			}
		}
		catch (Exception ex2)
		{
			if (!Resetting)
			{
				Invoke(args: new object[1] { this }, method: updateScreen_delegate);
				if (running_tab != null)
				{
					Set_Current_Tab_Using_Delegate(running_tab);
				}
				Invoke(Set_TopMost_delegate, this, true);
				if (ex2.InnerException != null)
				{
					Runtime.consoleWriteln(ex2.InnerException.Message);
					Invoke(MessageBox_delegate, this, ex2.InnerException.Message, "Error during run", MessageBoxIcon.Hand);
				}
				else
				{
					Runtime.consoleWriteln(ex2.Message);
					Invoke(MessageBox_delegate, this, ex2.Message, "Error during run", MessageBoxIcon.Hand);
				}
				Invoke(Set_TopMost_delegate, this, false);
				if (command_line_run)
				{
					exit_Click(null, null);
				}
				MC.program_stopped("Error, run halted");
			}
			if (continuous_Run)
			{
				myTimer.Stop();
			}
			continuous_Run = false;
			Invoke(Set_Running_delegate, this, false);
			if (currentObj != null)
			{
				currentObj.running = false;
				currentObj.selected = true;
			}
			currentObj = null;
			CallStack.Clear_Call_Stack();
			Invoke(invalidateScreen_delegate, this);
		}
	}

	private void Set_Current_Tab_Using_Delegate(TabPage tb)
	{
		Invoke(args: new object[2] { this, tb }, method: Set_Current_Tab_delegate);
	}

	private void menuStep_Click(object sender, EventArgs e)
	{
		if ((InstanceCaller == null || !InstanceCaller.IsAlive) && !continuous_Run)
		{
			InstanceCaller = new Thread(menuStep);
			InstanceCaller.SetApartmentState(ApartmentState.MTA);
			InstanceCaller.Priority = ThreadPriority.BelowNormal;
			InstanceCaller.Start();
		}
	}

	private void menuExecute_Click(object sender, EventArgs e)
	{
		if (fileName != null)
		{
			try
			{
				Environment.CurrentDirectory = Directory.GetParent(fileName).FullName;
			}
			catch
			{
			}
		}
		if (Component.compiled_flowchart)
		{
			if (!runningState)
			{
				try
				{
					Compile_Helpers.Run_Compiled(was_from_commandline: false);
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Flowchart terminated abnormally\n" + ex.ToString());
					return;
				}
			}
			return;
		}
		continuous_Run = true;
		if (currentObj != null && full_speed)
		{
			currentObj.running = false;
			flow_panel.Invalidate();
		}
		if (myTimer == null)
		{
			myTimer = new System.Timers.Timer(Timer_Frequency);
			myTimer.Elapsed += stepper;
		}
		myTimer.Start();
	}

	public static void Load_Delegate(Visual_Flow_Form f)
	{
		f.Load_File(f.load_filename);
	}

	private void loader(object sender, ElapsedEventArgs e)
	{
		loadTimer.Stop();
		Invoke(args: new object[1] { this }, method: Load_delegate);
	}

	private void stepper(object sender, ElapsedEventArgs e)
	{
		if (InstanceCaller != null && InstanceCaller.IsAlive)
		{
			return;
		}
		myTimer.Stop();
		try
		{
			InstanceCaller = new Thread(menuStep);
			InstanceCaller.SetApartmentState(ApartmentState.MTA);
			InstanceCaller.Priority = ThreadPriority.BelowNormal;
			InstanceCaller.Start();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private ClassTabPage findClass(TabPage tp)
	{
		Control control = tp.Parent;
		while (control != null && !(control is TabPage))
		{
			control = control.Parent;
		}
		return control as ClassTabPage;
	}

	private void menuPause_Click(object sender, EventArgs e)
	{
		if (Component.compiled_flowchart)
		{
			return;
		}
		if (continuous_Run && full_speed)
		{
			if (currentObj != null)
			{
				currentObj.running = true;
			}
			if (running_tab != null)
			{
				if (running_tab.Parent == carlisle)
				{
					carlisle.SelectedTab = running_tab;
				}
				else
				{
					(running_tab.Parent as TabControl).SelectedTab = running_tab;
					carlisle.SelectedTab = findClass(running_tab);
				}
			}
			flow_panel.Invalidate();
		}
		continuous_Run = false;
		if (myTimer != null)
		{
			myTimer.Stop();
		}
	}

	private void SaveAs_Click(object sender, EventArgs e)
	{
		string text = fileName;
		string filename;
		if (!Component.BARTPE)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.CheckFileExists = false;
			saveFileDialog.Filter = "Raptor files (*.rap)|*.rap|All files (*.*)|*.*";
			saveFileDialog.DefaultExt = ".rap";
			saveFileDialog.RestoreDirectory = false;
			if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
			{
				return;
			}
			filename = saveFileDialog.FileName;
		}
		else
		{
			new BARTPEFileSaveList().ShowDialog();
			filename = BARTPEFileSaveList.filename;
			if (filename == null)
			{
				return;
			}
		}
		if (filename == "" || filename == null)
		{
			MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		fileName = filename;
		FileSave_Click(sender, e);
		if (Save_Error)
		{
			fileName = text;
			return;
		}
		Text = My_Title + " - " + Path.GetFileName(fileName);
		modified = false;
		Plugins.Load_Plugins(fileName);
		MRU.Add_To_MRU_Registry(fileName);
	}

	private void menuStop_Click(object sender, EventArgs e)
	{
		if (runningState || (Compile_Helpers.run_compiled_thread != null && Compile_Helpers.run_compiled_thread.ThreadState == System.Threading.ThreadState.Running))
		{
			MC.program_stopped("Run halted");
		}
		else
		{
			MC.program_stopped("Reset");
		}
		if (Compile_Helpers.run_compiled_thread != null && Compile_Helpers.run_compiled_thread.ThreadState == System.Threading.ThreadState.Running)
		{
			try
			{
				Compile_Helpers.run_compiled_thread.Abort();
			}
			catch
			{
			}
		}
		Resetting = true;
		runningState = false;
		if (currentObj != null)
		{
			currentObj.running = false;
		}
		currentObj = null;
		continuous_Run = false;
		if (myTimer != null)
		{
			myTimer.Stop();
		}
		PromptForm.Kill();
		if (InstanceCaller != null && InstanceCaller.IsAlive)
		{
			try
			{
				InstanceCaller.Abort();
				InstanceCaller.Join(new TimeSpan(0, 0, 0, 1, 0));
			}
			catch
			{
			}
		}
		Resetting = false;
	}

	private void menuReset_Click(object sender, EventArgs e)
	{
		Resetting = true;
		Component.run_compiled_flowchart = false;
		menuStop_Click(sender, e);
		Runtime.Clear_Variables();
		CallStack.Clear_Call_Stack();
		flow_panel.Invalidate();
		numbers_pkg.set_precision(-1);
		Resetting = false;
	}

	private void menuResetExecute_Click(object sender, EventArgs e)
	{
		menuReset_Click(sender, e);
		menuExecute_Click(sender, e);
	}

	private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
	{
		if (e.Button == newButton)
		{
			new_clicked(sender, e);
		}
		else if (e.Button == openButton)
		{
			FileOpen_Click(sender, e);
		}
		else if (e.Button == saveButton)
		{
			FileSave_Click(sender, e);
		}
		else if (e.Button == cutButton)
		{
			Cut_Click(sender, e);
		}
		else if (e.Button == copyButton)
		{
			Copy_Click(sender, e);
		}
		else if (e.Button == pasteButton)
		{
			paste_Click(sender, e);
		}
		else if (e.Button == printButton)
		{
			filePrintMenuItem_Click(sender, e);
		}
		else if (e.Button == undoButton)
		{
			Undo_Click(sender, e);
		}
		else if (e.Button == redoButton)
		{
			Redo_Click(sender, e);
		}
		else if (e.Button == stepButton)
		{
			menuStep_Click(sender, e);
		}
		else if (e.Button == stopButton)
		{
			if (runningState)
			{
				menuPause_Click(sender, e);
				menuStop_Click(sender, e);
			}
			else
			{
				menuReset_Click(sender, e);
			}
		}
		else if (e.Button == pauseButton)
		{
			menuPause_Click(sender, e);
		}
		else if (e.Button == playButton)
		{
			menuExecute_Click(sender, e);
		}
		else if (e.Button == testServerButton)
		{
			menuRunServer_Click(sender, e);
		}
		else
		{
			if (e.Button != InkButton1)
			{
				return;
			}
			if (InkButton1.Pushed)
			{
				if (Last_Ink_Color == Color.Green)
				{
					menuItemInkGreen_Click(sender, e);
				}
				else if (Last_Ink_Color == Color.Black)
				{
					menuItemInkBlack_Click(sender, e);
				}
				else if (Last_Ink_Color == Color.Red)
				{
					menuItemInkRed_Click(sender, e);
				}
				else
				{
					menuItemInkBlue_Click(sender, e);
				}
			}
			else
			{
				menuItemInkOff_Click(sender, e);
			}
		}
	}

	private void trackBar1_Scroll(object sender, EventArgs e)
	{
		Timer_Frequency = 1005 - trackBar1.Value * 100;
		if (myTimer != null)
		{
			myTimer.Interval = Timer_Frequency;
		}
		if (trackBar1.Value == trackBar1.Maximum && !Component.BARTPE)
		{
			full_speed = true;
			if (continuous_Run)
			{
				currentObj.running = false;
				flow_panel.Invalidate();
			}
		}
		else
		{
			full_speed = false;
			if (continuous_Run)
			{
				currentObj.running = true;
				flow_panel.Invalidate();
			}
		}
		Registry_Settings.Write("Speed", trackBar1.Value.ToString());
	}

	private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (comboBox1.SelectedIndex)
		{
		case 0:
			menuScale300_Click(sender, e);
			break;
		case 1:
			menuScale200_Click(sender, e);
			break;
		case 2:
			menuScale175_Click(sender, e);
			break;
		case 3:
			menuScale150_Click(sender, e);
			break;
		case 4:
			Scale_100(sender, e);
			break;
		case 5:
			Scale_80(sender, e);
			break;
		case 6:
			Scale_60(sender, e);
			break;
		case 7:
			Scale_40(sender, e);
			break;
		case 8:
			Scale_20(sender, e);
			break;
		}
	}

	private void menuFile_Click(object sender, EventArgs e)
	{
		menuItemCompile.Enabled = Component.Current_Mode != Mode.Expert;
		MRU.Update_MRU_Menus(this);
	}

	public void Load_MRU(string name)
	{
		if (Save_Before_Losing(null, null))
		{
			if (name != null && name.CompareTo("") != 0)
			{
				Load_File(name);
			}
			MRU.Add_To_MRU_Registry(name);
		}
	}

	private void menuMRU1_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(1);
		Load_MRU(name);
	}

	private void menuMRU2_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(2);
		Load_MRU(name);
	}

	private void menuMRU3_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(3);
		Load_MRU(name);
	}

	private void menuMRU4_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(4);
		Load_MRU(name);
	}

	private void menuMRU5_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(5);
		Load_MRU(name);
	}

	private void menuMRU6_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(6);
		Load_MRU(name);
	}

	private void menuMRU7_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(7);
		Load_MRU(name);
	}

	private void menuMRU8_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(8);
		Load_MRU(name);
	}

	private void menuMRU9_Click(object sender, EventArgs e)
	{
		string name = MRU.Get_MRU_Registry(9);
		Load_MRU(name);
	}

	private void Visual_Flow_Form_Closing(object sender, CancelEventArgs e)
	{
		if (modified)
		{
			switch (MessageBox.Show("Flowchart was modified-- save these changes?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
			{
			case DialogResult.Yes:
				FileSave_Click(sender, null);
				e.Cancel = Save_Error;
				break;
			case DialogResult.Cancel:
				e.Cancel = true;
				break;
			default:
				e.Cancel = PromptForm.Close_All();
				break;
			}
		}
		else
		{
			e.Cancel = PromptForm.Close_All();
		}
		if (!e.Cancel)
		{
			if (InstanceCaller != null && InstanceCaller.IsAlive)
			{
				InstanceCaller.Abort();
			}
			Runtime.Clear_Variables();
			dotnetgraph.Shutdown_Dotnetgraph();
		}
	}

	public void Show_Text_On_Error()
	{
		if (!Component.text_visible)
		{
			Component.text_visible = true;
			if (Component.full_text)
			{
				menuAllText.Checked = true;
				menuTruncated.Checked = false;
				menuNoText.Checked = false;
			}
			else
			{
				menuAllText.Checked = false;
				menuTruncated.Checked = true;
				menuNoText.Checked = false;
			}
			flow_panel.Invalidate();
		}
	}

	private void menuAllText_Click(object sender, EventArgs e)
	{
		Component.full_text = true;
		Component.text_visible = true;
		menuAllText.Checked = true;
		menuTruncated.Checked = false;
		menuNoText.Checked = false;
		flow_panel.Invalidate();
		Registry_Settings.Write("TextView", "AllText");
	}

	private void menuTruncated_Click(object sender, EventArgs e)
	{
		Component.full_text = false;
		Component.text_visible = true;
		menuAllText.Checked = false;
		menuTruncated.Checked = true;
		menuNoText.Checked = false;
		flow_panel.Invalidate();
		Registry_Settings.Write("TextView", "Truncated");
	}

	private void menuNoText_Click(object sender, EventArgs e)
	{
		Component.full_text = false;
		Component.text_visible = false;
		menuAllText.Checked = false;
		menuTruncated.Checked = false;
		menuNoText.Checked = true;
		flow_panel.Invalidate();
		Registry_Settings.Write("TextView", "NoText");
	}

	private void listOfFunctionsMenu_Click(object sender, EventArgs e)
	{
		Help.ShowHelp(this, Directory.GetParent(Application.ExecutablePath)?.ToString() + "\\raptor.chm");
	}

	private void contextMenu1_Popup(object sender, EventArgs e)
	{
		bool num = Current_Selection != null && Current_Selection.Name != "Oval";
		contextMenuComment.Enabled = Current_Selection != null;
		if (num || selectedComment != null || region_selected)
		{
			contextMenuCopy.Enabled = true;
			contextMenuCut.Enabled = true;
			contextMenuDelete.Enabled = true;
			contextMenuEdit.Enabled = true;
		}
		else
		{
			contextMenuCopy.Enabled = false;
			contextMenuCut.Enabled = false;
			contextMenuDelete.Enabled = false;
			contextMenuEdit.Enabled = false;
		}
	}

	private bool UML_Displayed()
	{
		if (Component.Current_Mode == Mode.Expert)
		{
			return carlisle.SelectedIndex == 0;
		}
		return false;
	}

	private void menuEdit_Click(object sender, EventArgs e)
	{
		if (UML_Displayed())
		{
			menuItemCopy.Enabled = false;
			menuItemDelete.Enabled = true;
			menuItemCut.Enabled = false;
			menuItemPaste.Enabled = false;
			menuItemComment.Enabled = false;
			menuItemEditSelection.Enabled = false;
			return;
		}
		bool flag = Current_Selection != null && Current_Selection.Name != "Oval";
		IDataObject dataObject = ClipboardMultiplatform.GetDataObject();
		Component.warned_about_error = true;
		Clipboard_Data clipboard_Data = null;
		if (dataObject != null)
		{
			clipboard_Data = (Clipboard_Data)dataObject.GetData("raptor.Clipboard_Data");
		}
		menuItemPaste.Enabled = (clipboard_Data != null && clipboard_Data.kind == Clipboard_Data.kinds.symbols && Current_Selection == null) || (clipboard_Data != null && clipboard_Data.kind == Clipboard_Data.kinds.comment && Current_Selection != null);
		menuItemComment.Enabled = Current_Selection != null;
		Subchart subchart = selectedTabMaybeNull();
		try
		{
			if (!Component.BARTPE && !Component.VM && !Component.MONO && subchart != null && subchart.tab_overlay.Ink.CanPaste())
			{
				menuItemPaste.Enabled = true;
			}
		}
		catch
		{
			MessageBox.Show("Please install the Microsoft.Ink.dll CLR 2.0 Update (KB900722)");
		}
		if (flag || selectedComment != null || region_selected)
		{
			menuItemCopy.Enabled = true;
			menuItemDelete.Enabled = true;
			menuItemCut.Enabled = true;
		}
		else
		{
			menuItemCopy.Enabled = false;
			menuItemDelete.Enabled = false;
			menuItemCut.Enabled = false;
		}
		if (!Component.BARTPE && !Component.VM && !Component.MONO && subchart != null && subchart.tab_overlay.Selection != null && subchart.tab_overlay.Selection.Count > 0)
		{
			menuItemCopy.Enabled = true;
			menuItemDelete.Enabled = true;
			menuItemCut.Enabled = true;
		}
		menuItemEditSelection.Enabled = Current_Selection != null;
	}

	private void contextMenuComment_Click(object sender, EventArgs e)
	{
		Current_Selection.addComment(this);
		my_layout();
		flow_panel.Invalidate();
	}

	private void menuViewComments_Click(object sender, EventArgs e)
	{
		menuViewComments.Checked = !menuViewComments.Checked;
		Component.view_comments = menuViewComments.Checked;
		flow_panel.Invalidate();
		Registry_Settings.Write("ViewComments", menuViewComments.Checked.ToString());
	}

	private void menuViewVariables_Click(object sender, EventArgs e)
	{
		menuViewVariables.Checked = !menuViewVariables.Checked && !Component.compiled_flowchart;
		if (watchBox.IsHandleCreated)
		{
			if (!menuViewVariables.Checked || Component.compiled_flowchart)
			{
				watchBox.Hide();
			}
			else
			{
				watchBox.Show();
			}
		}
		if (!Component.compiled_flowchart)
		{
			Registry_Settings.Write("ViewVariables", menuViewVariables.Checked.ToString());
		}
	}

	private void menuShowLog_Click(object sender, EventArgs e)
	{
		log.Display(this, show_full_log: false);
		MC.BringToFront();
	}

	private void menuTileVertical_Click(object sender, EventArgs e)
	{
		base.WindowState = FormWindowState.Normal;
		MC.WindowState = FormWindowState.Normal;
		Size primaryMonitorMaximizedWindowSize = SystemInformation.PrimaryMonitorMaximizedWindowSize;
		base.Left = 0;
		base.Top = 0;
		base.Height = primaryMonitorMaximizedWindowSize.Height - 20;
		base.Width = primaryMonitorMaximizedWindowSize.Width - MC.MinimumSize.Width;
		MC.Left = base.Width;
		MC.Top = 0;
		MC.Width = MC.MinimumSize.Width;
		MC.Height = primaryMonitorMaximizedWindowSize.Height - 20;
		MC.MasterConsole_Resize(sender, e);
		Visual_Flow_Form_Resize(sender, e);
	}

	private void menuTileHorizontal_Click(object sender, EventArgs e)
	{
		base.WindowState = FormWindowState.Normal;
		MC.WindowState = FormWindowState.Normal;
		Size primaryMonitorMaximizedWindowSize = SystemInformation.PrimaryMonitorMaximizedWindowSize;
		base.Left = 0;
		base.Top = 0;
		base.Width = primaryMonitorMaximizedWindowSize.Width;
		base.Height = primaryMonitorMaximizedWindowSize.Height - 20 - 150;
		MC.Left = 0;
		MC.Top = base.Height;
		MC.Height = 150;
		MC.Width = primaryMonitorMaximizedWindowSize.Width;
		MC.MasterConsole_Resize(sender, e);
		Visual_Flow_Form_Resize(sender, e);
	}

	private void menuItemEditSelection_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null && subchart.Current_Selection != null)
		{
			subchart.Start.setText(subchart.Current_Selection.X + 1, subchart.Current_Selection.Y + 1, this);
		}
	}

	private void Change_Selection_To(Component c)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			subchart.Current_Selection.selected = false;
			subchart.Current_Selection = c;
			subchart.Start.select(subchart.Current_Selection.X + 1, subchart.Current_Selection.Y + subchart.Current_Selection.H / 2);
			flow_panel.Invalidate();
		}
	}

	public void Visual_Flow_Form_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Prior)
		{
			Point autoScrollPosition = panel1.AutoScrollPosition;
			if (autoScrollPosition.Y < 0)
			{
				autoScrollPosition.Y = -autoScrollPosition.Y;
			}
			autoScrollPosition.Y -= 450;
			if (autoScrollPosition.Y < 0)
			{
				autoScrollPosition.Y = 0;
			}
			panel1.AutoScrollPosition = autoScrollPosition;
		}
		else if (e.KeyCode == Keys.Next)
		{
			Point autoScrollPosition2 = panel1.AutoScrollPosition;
			if (autoScrollPosition2.Y < 0)
			{
				autoScrollPosition2.Y = -autoScrollPosition2.Y;
			}
			autoScrollPosition2.Y += 450;
			panel1.AutoScrollPosition = autoScrollPosition2;
		}
		if (Current_Selection != null)
		{
			if (e.KeyCode == Keys.Down)
			{
				if (Current_Selection.Name != "Loop")
				{
					if (Current_Selection.Successor != null)
					{
						Change_Selection_To(Current_Selection.Successor);
					}
					else if (Current_Selection.is_afterChild)
					{
						Change_Selection_To(Current_Selection.parent.Successor);
					}
					else if (Current_Selection.is_beforeChild)
					{
						if (((Loop)Current_Selection.parent).after_Child != null)
						{
							Change_Selection_To(((Loop)Current_Selection.parent).after_Child);
						}
						else
						{
							Change_Selection_To(Current_Selection.parent.Successor);
						}
					}
				}
				else if (((Loop)Current_Selection).before_Child != null)
				{
					Change_Selection_To(((Loop)Current_Selection).before_Child);
				}
				else if (((Loop)Current_Selection).after_Child != null)
				{
					Change_Selection_To(((Loop)Current_Selection).after_Child);
				}
				else
				{
					Change_Selection_To(Current_Selection.Successor);
				}
			}
			else if (e.KeyCode == Keys.Up && selectedTabMaybeNull() != null)
			{
				Component component = selectedTabMaybeNull().Start.Find_Predecessor(Current_Selection);
				if (component != null)
				{
					Change_Selection_To(component);
				}
			}
			else if (e.KeyCode == Keys.Left && selectedTabMaybeNull() != null)
			{
				if (Current_Selection.Name == "IF_Control" && ((IF_Control)Current_Selection).left_Child != null)
				{
					Change_Selection_To(((IF_Control)Current_Selection).left_Child);
				}
				else if (Current_Selection.Name == "Loop")
				{
					Change_Selection_To(Current_Selection.Successor);
				}
			}
			else if (e.KeyCode == Keys.Right && selectedTabMaybeNull() != null)
			{
				if (Current_Selection.Name == "IF_Control" && ((IF_Control)Current_Selection).right_Child != null)
				{
					Change_Selection_To(((IF_Control)Current_Selection).right_Child);
				}
			}
			else if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Return) && selectedTabMaybeNull() != null)
			{
				selectedTabMaybeNull().Start.setText(Current_Selection.X + 5, Current_Selection.Y + 5, this);
			}
		}
		else if (e.KeyCode == Keys.Down && selectedTabMaybeNull() != null)
		{
			Subchart subchart = selectedTabMaybeNull();
			Current_Selection = subchart.Start;
			Change_Selection_To(subchart.Start);
		}
	}

	public void num_vertical_print_pages(PrintPageEventArgs e, Oval current_tab_start)
	{
		int num = 1;
		int num2 = drawing_height;
		vert_page_breaks[0] = 0;
		e.Graphics.SetClip(new RectangleF(0f, 0f, 0f, 0f));
		current_tab_start.draw(e.Graphics, x1, y1);
		while (num2 > 0)
		{
			int num3;
			int num4;
			if (num == 1)
			{
				num3 = y1 + (int)pageheight;
				num4 = y1 + (int)pageheight * 2 / 3;
			}
			else
			{
				num3 = vert_page_breaks[num - 1] + (int)pageheight;
				num4 = vert_page_breaks[num - 1] + (int)pageheight * 2 / 3;
			}
			int num5 = num3;
			System.Drawing.Rectangle rec = new System.Drawing.Rectangle(1, num3, drawing_width, 1);
			bool flag = false;
			while (current_tab_start.SelectRegion(rec) && num3 > num4)
			{
				num3 = (rec.Y = num3 - 1);
				flag = true;
			}
			if (num3 == num4)
			{
				num3 = num5;
			}
			if (flag)
			{
				vert_page_breaks[num] = num3 - current_tab_start.CL / 2;
			}
			else
			{
				vert_page_breaks[num] = num3;
			}
			num2 = y1 + drawing_height - num3;
			if (num2 > 0)
			{
				num++;
			}
		}
		num_vert_pages = num;
	}

	public void num_horizontal_print_pages(PrintPageEventArgs e, Oval current_tab_start)
	{
		int num = 1;
		int num2 = (int)Math.Round(leftMargin);
		int num3 = drawing_width;
		hor_page_breaks[0] = 0;
		e.Graphics.SetClip(new RectangleF(0f, 0f, 0f, 0f));
		current_tab_start.draw(e.Graphics, x1, y1);
		while (num3 > 0)
		{
			int num4;
			int num5;
			if (num == 1)
			{
				num4 = num2 + (int)pagewidth;
				num5 = num2 + (int)pagewidth * 2 / 3;
			}
			else
			{
				num4 = hor_page_breaks[num - 1] + (int)pagewidth;
				num5 = hor_page_breaks[num - 1] + (int)pagewidth * 2 / 3;
			}
			int num6 = num4;
			System.Drawing.Rectangle rec = new System.Drawing.Rectangle(num4, 1, 1, drawing_height);
			bool flag = false;
			while (current_tab_start.SelectRegion(rec) && num4 > num5)
			{
				num4 = (rec.X = num4 - 1);
				flag = true;
			}
			if (num4 == num5)
			{
				num4 = num6;
			}
			if (flag)
			{
				hor_page_breaks[num] = num4 - current_tab_start.CL / 2;
			}
			else
			{
				hor_page_breaks[num] = num4;
			}
			num3 = drawing_width - num4;
			if (num3 > 0)
			{
				num++;
			}
		}
		num_hor_pages = num;
	}

	public void menuItemAssignment_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			Oval start = subchart.Start;
			Make_Undoable();
			if (start.insert(new Rectangle(60, 90, "Rectangle", Rectangle.Kind_Of.Assignment), mouse_x, mouse_y, 0))
			{
				Current_Selection = start.select(-1000, -1000);
				start.Scale(scale);
				my_layout();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	public void menuItemCall_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			Oval start = subchart.Start;
			Make_Undoable();
			if (start.insert(new Rectangle(60, 90, "Rectangle", Rectangle.Kind_Of.Call), mouse_x, mouse_y, 0))
			{
				Current_Selection = start.select(-1000, -1000);
				start.Scale(scale);
				my_layout();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	public void menuItemParallelogram_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			Oval start = subchart.Start;
			Make_Undoable();
			if (start.insert(new Parallelogram(60, 90, "Parallelogram", input: true), mouse_x, mouse_y, 0))
			{
				Current_Selection = start.select(-1000, -1000);
				start.Scale(scale);
				my_layout();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	public void menuItemOutput_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			Oval start = subchart.Start;
			Make_Undoable();
			if (start.insert(new Parallelogram(60, 90, "Parallelogram", input: false), mouse_x, mouse_y, 0))
			{
				Current_Selection = start.select(-1000, -1000);
				start.Scale(scale);
				my_layout();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	public void menuItemIf_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			Oval start = subchart.Start;
			Make_Undoable();
			if (start.insert(new IF_Control(60, 90, "IF_Control"), mouse_x, mouse_y, 0))
			{
				Current_Selection = start.select(-1000, -1000);
				start.Scale(scale);
				my_layout();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	public void menuItemLoop_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			Oval start = subchart.Start;
			Make_Undoable();
			if (start.insert(new Loop(60, 90, "Loop"), mouse_x, mouse_y, 0))
			{
				Current_Selection = start.select(-1000, -1000);
				start.Scale(scale);
				my_layout();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	public void menuItemReturn_Click(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			Oval start = subchart.Start;
			Make_Undoable();
			if (start.insert(new Oval_Return(60, 90, "Return"), mouse_x, mouse_y, 0))
			{
				Current_Selection = start.select(-1000, -1000);
				start.Scale(scale);
				my_layout();
				flow_panel.Invalidate();
			}
			else
			{
				Undo_Stack.Decrement_Undoable(this);
			}
		}
	}

	private void menuItemCompile_Click(object sender, EventArgs e)
	{
		mainSubchart().Start.selected = false;
		if (fileName == null || fileName == "")
		{
			MessageBox.Show("Must save before compiling", "Can't compile");
			return;
		}
		if (Component.compiled_flowchart)
		{
			MessageBox.Show("Already compiled", "Can't compile");
			return;
		}
		string text = ((fileName.Length <= 4) ? (fileName + " (compiled).rap") : (fileName.Substring(0, fileName.Length - 4) + " (compiled).rap"));
		if (MessageBox.Show("Are you sure you want to compile?\nYou can not undo this operation.\n\nThis will save your current flowchart as:\n   " + fileName + "\nand compile to:\n   " + text, "Compile?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
		{
			return;
		}
		try
		{
			FileSave_Click(sender, e);
			fileName = text;
			Compile_Helpers.Compile_Flowchart(carlisle.TabPages);
			Component.compiled_flowchart = true;
			FileSave_Click(sender, e);
			Text = My_Title + " - " + Path.GetFileName(fileName);
			modified = false;
			Registry_Settings.Ignore_Updates = true;
			trackBar1.Value = trackBar1.Maximum;
			trackBar1_Scroll(sender, e);
			menuViewVariables_Click(sender, e);
			Registry_Settings.Ignore_Updates = false;
			Undo_Stack.Clear_Undo(this);
			my_layout();
			flow_panel.Invalidate();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Can't compile");
		}
	}

	private void Visual_Flow_Form_Deactivate(object sender, EventArgs e)
	{
		Subchart subchart = selectedTabMaybeNull();
		if (panel1 != null && subchart != null)
		{
			scroll_location = subchart.AutoScrollPosition;
		}
	}

	private void Visual_Flow_Form_Activated(object sender, EventArgs e)
	{
		selectedTabMaybeNull()?.Activated(sender, e);
	}

	private void menuBreakpoint_Click(object sender, EventArgs e)
	{
		if (Breakpoint_Selection != null && Breakpoint_Selection.Name != "Oval")
		{
			Breakpoint_Selection.Toggle_Breakpoint(mouse_x, mouse_y);
			flow_panel.Invalidate();
		}
	}

	private void menuClearBreakpoints_Click(object sender, EventArgs e)
	{
		Oval start;
		if (carlisle.SelectedTab is Subchart)
		{
			start = (carlisle.SelectedTab as Subchart).Start;
		}
		else
		{
			if (!(carlisle.SelectedTab is ClassTabPage) || (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count <= 0)
			{
				return;
			}
			start = ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).Start;
		}
		start.Clear_Breakpoints();
		flow_panel.Invalidate();
	}

	private void menuPrintClipboard_Click(object sender, EventArgs e)
	{
		Oval start;
		if (carlisle.SelectedTab is Subchart)
		{
			start = (carlisle.SelectedTab as Subchart).Start;
		}
		else
		{
			if (!(carlisle.SelectedTab is ClassTabPage) || (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count <= 0)
			{
				return;
			}
			start = ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).Start;
		}
		if (start == null)
		{
			return;
		}
		Graphics graphics = CreateGraphics();
		start.footprint(graphics);
		x1 = start.FP.left + 90;
		y1 = (int)Math.Round(scale * 30f);
		my_layout();
		Metafile metafile = new Metafile(graphics.GetHdc(), EmfType.EmfPlusDual);
		Graphics graphics2 = Graphics.FromImage(metafile);
		graphics2.Clear(Color.White);
		start.draw(graphics2, x1, y1);
		Subchart subchart = selectedTabMaybeNull();
		if (subchart != null)
		{
			if (subchart.tab_overlay != null && subchart.tab_overlay.Ink != null)
			{
				subchart.tab_overlay.Renderer.Draw(graphics2, subchart.tab_overlay.Ink.Strokes);
			}
			graphics2.Dispose();
			MessageBox.Show("Please open MS Word, or other application you intend to paste into.\nOtherwise it may not paste correctly.", "Paste to clipboard");
			IntPtr handle = base.Handle;
			IntPtr intPtr = Win32.CopyEnhMetaFile(metafile.GetHenhmetafile(), new IntPtr(0));
			if (!intPtr.Equals(new IntPtr(0)) && Win32.OpenClipboard(handle) && Win32.EmptyClipboard())
			{
				Win32.SetClipboardData(14u, intPtr).Equals(intPtr);
				Win32.CloseClipboard();
			}
		}
	}

	public void menuCountSymbols_Click(object sender, EventArgs e)
	{
		int num = 0;
		int tabCount = carlisle.TabCount;
		for (int i = mainIndex; i < tabCount; i++)
		{
			int num2 = Runtime.Count_Symbols(((Subchart)carlisle.TabPages[i]).Start);
			Runtime.consoleWriteln("The number of symbols in " + carlisle.TabPages[i].Text + " is: " + num2);
			num += num2;
		}
		if (tabCount > 1)
		{
			Runtime.consoleWriteln("The total number of symbols is: " + num);
		}
		MC.BringToFront();
	}

	private void menuSelectAll_Click(object sender, EventArgs e)
	{
		if (UML_Displayed())
		{
			(carlisle.SelectedTab.Controls[0] as UMLDiagram).mnuSelectAll_Click(sender, e);
			return;
		}
		Oval start;
		if (carlisle.SelectedTab is Subchart)
		{
			start = (carlisle.SelectedTab as Subchart).Start;
		}
		else
		{
			if (!(carlisle.SelectedTab is ClassTabPage) || (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count <= 0)
			{
				return;
			}
			start = ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).Start;
		}
		start.selectAll();
		flow_panel.Invalidate();
	}

	private void menuExpandAll_Click(object sender, EventArgs e)
	{
		Oval start;
		if (carlisle.SelectedTab is Subchart)
		{
			start = (carlisle.SelectedTab as Subchart).Start;
		}
		else
		{
			if (!(carlisle.SelectedTab is ClassTabPage) || (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count <= 0)
			{
				return;
			}
			start = ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).Start;
		}
		Make_Undoable();
		start.change_compressed(compressed: false);
		flow_panel.Invalidate();
	}

	private void menuCollapseAll_Click(object sender, EventArgs e)
	{
		Oval start;
		if (carlisle.SelectedTab is Subchart)
		{
			start = (carlisle.SelectedTab as Subchart).Start;
		}
		else
		{
			if (!(carlisle.SelectedTab is ClassTabPage) || (carlisle.SelectedTab as ClassTabPage).tabControl1.TabPages.Count <= 0)
			{
				return;
			}
			start = ((carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).Start;
		}
		Make_Undoable();
		start.change_compressed(compressed: true);
		flow_panel.Invalidate();
	}

	private void tabControl1_TabIndexChanged(object sender, EventArgs e)
	{
		selectedTabMaybeNull()?.Activated(sender, e);
	}

	private void tabControl1_MouseUp(object sender, MouseEventArgs e)
	{
		tab_moving = null;
		tab_moving_index = 0;
	}

	private void tabControl1_MouseMove(object sender, MouseEventArgs e)
	{
		if (tab_moving == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			if (carlisle.GetTabRect(i).IntersectsWith(new System.Drawing.Rectangle(e.X, e.Y, 1, 1)))
			{
				num = i;
			}
		}
		if (num > 0 && num != tab_moving_index)
		{
			carlisle.TabPages.Remove(tab_moving);
			carlisle.TabPages.Insert(num, tab_moving);
			tab_moving_index = num;
			carlisle.SelectedIndex = num;
			carlisle.Refresh();
		}
	}

	private void tabControl1_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button.ToString() == "Right")
		{
			if (Component.Current_Mode == Mode.Expert)
			{
				return;
			}
			for (int i = 0; i < carlisle.TabCount; i++)
			{
				if (carlisle.GetTabRect(i).IntersectsWith(new System.Drawing.Rectangle(e.X, e.Y, 1, 1)))
				{
					carlisle.SelectedIndex = i;
				}
			}
			if (carlisle.SelectedTab.Text == "main")
			{
				menuDeleteSubchart.Enabled = false;
				menuRenameSubchart.Enabled = false;
			}
			else
			{
				menuDeleteSubchart.Enabled = true;
				menuRenameSubchart.Enabled = true;
				if (carlisle.SelectedTab is Procedure_Chart)
				{
					menuRenameSubchart.Text = "&Modify procedure";
					menuDeleteSubchart.Text = "&Delete procedure";
				}
				else
				{
					menuRenameSubchart.Text = "&Rename subchart";
					menuDeleteSubchart.Text = "&Delete subchart";
				}
				for (int j = 0; j < carlisle.TabCount; j++)
				{
					if (((Subchart)carlisle.TabPages[j]).Start.Called_Tab(carlisle.SelectedTab.Text))
					{
						menuDeleteSubchart.Enabled = false;
						break;
					}
				}
			}
			tabContextMenu.Show(carlisle, new Point(e.X, e.Y));
		}
		else
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			for (int k = ((Component.Current_Mode != Mode.Expert) ? 1 : 2); k < carlisle.TabCount; k++)
			{
				if (carlisle.GetTabRect(k).IntersectsWith(new System.Drawing.Rectangle(e.X, e.Y, 1, 1)))
				{
					tab_moving = carlisle.TabPages[k];
					tab_moving_index = k;
				}
			}
		}
	}

	private void menuAddSubchart_Click(object sender, EventArgs e)
	{
		string text = Subchart_name.RunDialog("", this);
		if (text != null && text != "")
		{
			Subchart subchart = new Subchart(this, text);
			carlisle.TabPages.Add(subchart);
			carlisle.SelectedTab = subchart;
			Undo_Stack.Make_Add_Tab_Undoable(this, subchart);
		}
	}

	private void menuDeleteSubchart_Click(object sender, EventArgs e)
	{
		if (carlisle.SelectedTab != mainSubchart())
		{
			Undo_Stack.Make_Delete_Tab_Undoable(this, (Subchart)carlisle.SelectedTab);
			carlisle.TabPages.Remove(carlisle.SelectedTab);
		}
	}

	public void Rename_Tab(string old_name, string name)
	{
		if (name.ToLower() != old_name.ToLower())
		{
			for (int i = 0; i < carlisle.TabCount; i++)
			{
				((Subchart)carlisle.TabPages[i]).Start.Rename_Tab(old_name, name);
			}
		}
	}

	public void menuRenameSubchart_Click(object sender, EventArgs e)
	{
		string text = carlisle.SelectedTab.Text;
		if (!(carlisle.SelectedTab is Procedure_Chart))
		{
			string text2 = Subchart_name.RunDialog(carlisle.SelectedTab.Text, this);
			if (text2 != null && text2 != "" && text != text2)
			{
				carlisle.SelectedTab.Text = text2;
				Rename_Tab(text, text2);
				Undo_Stack.Make_Rename_Tab_Undoable(this, (Subchart)carlisle.SelectedTab, text, text2);
			}
		}
		else
		{
			string text3 = ((Procedure_Chart)carlisle.SelectedTab).RunDialog(text, this);
			if (text3 != null && text3 != "" && text != text3)
			{
				carlisle.SelectedTab.Text = text3;
				Rename_Tab(text, text3);
				Undo_Stack.Make_Rename_Tab_Undoable(this, (Subchart)carlisle.SelectedTab, text, text3);
			}
		}
	}

	public bool Is_Subchart_Name(string s)
	{
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			if (!(carlisle.TabPages[i] is ClassTabPage) && carlisle.TabPages[i].Text.ToLower() == s.Trim().ToLower())
			{
				return true;
			}
		}
		return false;
	}

	public bool Is_Subchart_Call(string s)
	{
		int num = s.IndexOf("(");
		if (num > 0)
		{
			return Is_Subchart_Name(s.Substring(0, num));
		}
		return Is_Subchart_Name(s);
	}

	private void menuItemRunCompiled_Click(object sender, EventArgs e)
	{
		if (Component.compiled_flowchart)
		{
			menuResetExecute_Click(sender, e);
			return;
		}
		FileSave_Click(sender, e);
		menuReset_Click(sender, e);
		Component.run_compiled_flowchart = true;
		try
		{
			Compile_Helpers.Compile_Flowchart(carlisle.TabPages);
			try
			{
				Compile_Helpers.Run_Compiled(was_from_commandline: false);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Flowchart terminated abnormally\n" + ex.ToString());
			}
		}
		catch (Exception ex2)
		{
			MessageBox.Show(ex2.Message + "\n", "Compilation error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		Component.run_compiled_flowchart = false;
		rescale_all(scale);
	}

	private void ConfigServer_Click(object sender, EventArgs e)
	{
		new SubmitServerForm().ShowDialog();
	}

	private void menuRunServer_Click(object sender, EventArgs e)
	{
		if (fileName == "" || fileName == null)
		{
			SaveAs_Click(sender, e);
		}
		if (!(fileName == "") && fileName != null)
		{
			FileSave_Click(sender, e);
			SubmitServerForm.submit(Path.GetFileNameWithoutExtension(fileName));
		}
	}

	private void contextMenuEdit_Click(object sender, EventArgs e)
	{
		menuItemEditSelection_Click(sender, e);
	}

	private void label2_DragOver(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.Text))
		{
			switch ((string)e.Data.GetData(DataFormats.Text))
			{
			case "raptor_ASGN":
			case "raptor_CALL":
			case "raptor_INPUT":
			case "raptor_OUTPUT":
			case "raptor_SELECTION":
			case "raptor_LOOP":
			case "raptor_RETURN":
				e.Effect = DragDropEffects.Copy;
				break;
			default:
				e.Effect = DragDropEffects.None;
				break;
			}
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void menuGraphOnTop_Click(object sender, EventArgs e)
	{
		menuGraphOnTop.Checked = !menuGraphOnTop.Checked;
		dotnetgraph.start_topmost = menuGraphOnTop.Checked;
		if (menuGraphOnTop.Checked)
		{
			dotnetgraph.MakeTopMost();
		}
		else
		{
			dotnetgraph.MakeNonTopMost();
		}
		Registry_Settings.Write("RAPTORGraphOnTop", menuGraphOnTop.Checked.ToString());
	}

	private void menuProgramCompleteDialog_Click(object sender, EventArgs e)
	{
		menuProgramCompleteDialog.Checked = !menuProgramCompleteDialog.Checked;
		Registry_Settings.Write("ProgramCompleteDialog", menuProgramCompleteDialog.Checked.ToString());
	}

	private void Visual_Flow_Form_Move(object sender, EventArgs e)
	{
		Visual_Flow_Form_Resize(sender, e);
	}

	private void Visual_Flow_Form_Resize(object sender, EventArgs e)
	{
		if (!starting_up && base.WindowState != FormWindowState.Maximized)
		{
			if (base.DesktopLocation.X >= 0)
			{
				Registry_Settings.Write("FormX", base.DesktopLocation.X.ToString());
			}
			if (base.DesktopLocation.Y >= 0)
			{
				Registry_Settings.Write("FormY", base.DesktopLocation.Y.ToString());
			}
			int num;
			try
			{
				num = int.Parse(Registry_Settings.Read("FormWidth"));
			}
			catch
			{
				num = 0;
			}
			if (base.DesktopBounds.Width > 570 && Math.Abs(base.Width - num) > 20)
			{
				Registry_Settings.Write("FormWidth", base.Width.ToString());
			}
			try
			{
				num = int.Parse(Registry_Settings.Read("FormHeight"));
			}
			catch
			{
				num = 0;
			}
			if (base.DesktopBounds.Height > 370 && Math.Abs(base.Height - num) > 20)
			{
				Registry_Settings.Write("FormHeight", base.Height.ToString());
			}
		}
	}

	private void DefaultWindowSize_Click(object sender, EventArgs e)
	{
		Size primaryMonitorMaximizedWindowSize = SystemInformation.PrimaryMonitorMaximizedWindowSize;
		MC.Width = 368;
		MC.Height = 344;
		MC.Left = primaryMonitorMaximizedWindowSize.Width - MC.Width;
		MC.Top = primaryMonitorMaximizedWindowSize.Height - MC.Height - 20;
		SetDesktopBounds(100, 100, 722, 566);
	}

	private void menuObjectiveMode_Click(object sender, EventArgs e)
	{
		if (Component.Current_Mode != Mode.Expert)
		{
			if (!Save_Before_Losing(sender, e))
			{
				return;
			}
			modified = false;
			if (carlisle.TabPages.Count > 0)
			{
				new_clicked(sender, e);
			}
			TabPage tabPage = new TabPage("UML");
			if (carlisle.TabPages.Count == 0)
			{
				carlisle.TabPages.Add(tabPage);
			}
			else
			{
				carlisle.TabPages.Insert(0, tabPage);
			}
			tabPage.Controls.Add(new UMLDiagram(umlupdater));
			control_panel.Invalidate();
		}
		Registry_Settings.Write("UserMode", "Expert");
		Component.Current_Mode = Mode.Expert;
		menuIntermediate.Checked = false;
		menuNovice.Checked = false;
		menuObjectiveMode.Checked = true;
	}

	private void menuNovice_Click(object sender, EventArgs e)
	{
		if (Component.Current_Mode == Mode.Expert)
		{
			if (!Save_Before_Losing(sender, e))
			{
				return;
			}
			modified = false;
			new_clicked(sender, e);
			carlisle.TabPages.RemoveAt(0);
			control_panel.Invalidate();
		}
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			if (carlisle.TabPages[i] is Procedure_Chart)
			{
				MessageBox.Show("Can't switch to novice mode, " + carlisle.TabPages[i].Text + " is a procedure");
				return;
			}
		}
		Registry_Settings.Write("UserMode", "Novice");
		Component.Current_Mode = Mode.Novice;
		menuIntermediate.Checked = false;
		menuNovice.Checked = true;
		menuObjectiveMode.Checked = false;
	}

	private void menuIntermediate_Click(object sender, EventArgs e)
	{
		if (Component.Current_Mode == Mode.Expert)
		{
			if (!Save_Before_Losing(sender, e))
			{
				return;
			}
			modified = false;
			new_clicked(sender, e);
			carlisle.TabPages.RemoveAt(0);
			control_panel.Invalidate();
		}
		Registry_Settings.Write("UserMode", "Intermediate");
		Component.Current_Mode = Mode.Intermediate;
		menuIntermediate.Checked = true;
		menuNovice.Checked = false;
		menuObjectiveMode.Checked = false;
	}

	private void tabContextMenu_Popup(object sender, EventArgs e)
	{
		if (Component.Current_Mode == Mode.Intermediate)
		{
			menuAddProcedure.Visible = true;
		}
		else if (Component.Current_Mode == Mode.Novice)
		{
			menuAddProcedure.Visible = false;
			menuAddFunction.Visible = false;
		}
	}

	private void menuAddProcedure_Click(object sender, EventArgs e)
	{
		string[] incoming_param_names = new string[0];
		bool[] is_input = null;
		bool[] is_output = null;
		string text = Procedure_name.RunDialog("", ref incoming_param_names, ref is_input, ref is_output, this);
		if (text != null && text != "")
		{
			Procedure_Chart procedure_Chart = new Procedure_Chart(this, text, incoming_param_names, is_input, is_output);
			carlisle.TabPages.Add(procedure_Chart);
			carlisle.SelectedTab = procedure_Chart;
			Undo_Stack.Make_Add_Tab_Undoable(this, procedure_Chart);
		}
	}

	private void menuGenerateStandalone_Click(object sender, EventArgs e)
	{
		mainSubchart().Start.selected = false;
		if (fileName == null || fileName == "")
		{
			MessageBox.Show("Must save before generating standalone", "Can't generate EXE");
			return;
		}
		try
		{
			string text = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileName) + "_exe";
			Compile_Helpers.Compile_Flowchart_To(mainSubchart().Start, text, Path.GetFileNameWithoutExtension(fileName) + "_rap.exe");
			MessageBox.Show("Result in folder: " + text, "Generation complete");
		}
		catch (Exception ex)
		{
			MessageBox.Show("Generation failed: " + ex.Message);
		}
	}

	public void handle_click(object sender, EventArgs e)
	{
		MenuItem menuItem = sender as MenuItem;
		if (fileName == null || fileName == "")
		{
			MessageBox.Show("Must save before generating code", "Can't generate code");
			return;
		}
		if (Component.compiled_flowchart)
		{
			MessageBox.Show("Cannot generate code from compiled flowchart", "Unable to generate");
			return;
		}
		try
		{
			typ gil = Generators.Create_From_Menu(menuItem.Text, fileName);
			Compile_Helpers.Do_Compilation(mainSubchart().Start, gil, Runtime.parent.carlisle.TabPages);
		}
		catch (Exception ex)
		{
			string text = ((ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
			MessageBox.Show(text + "\n", "Compilation error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void Ink_Enable(Color c)
	{
		Last_Ink_Color = c;
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			((Subchart)carlisle.TabPages[i]).tab_overlay.Enabled = true;
			((Subchart)carlisle.TabPages[i]).tab_overlay.EditingMode = InkOverlayEditingMode.Ink;
			((Subchart)carlisle.TabPages[i]).tab_overlay.DefaultDrawingAttributes.Color = c;
		}
	}

	private void Ink_Erase()
	{
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			((Subchart)carlisle.TabPages[i]).tab_overlay.Enabled = true;
			((Subchart)carlisle.TabPages[i]).tab_overlay.EditingMode = InkOverlayEditingMode.Delete;
		}
	}

	private void Ink_Disable()
	{
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			((Subchart)carlisle.TabPages[i]).tab_overlay.Enabled = false;
			((Subchart)carlisle.TabPages[i]).tab_overlay.EditingMode = InkOverlayEditingMode.Select;
		}
	}

	private void Ink_Select()
	{
		for (int i = 0; i < carlisle.TabCount; i++)
		{
			((Subchart)carlisle.TabPages[i]).tab_overlay.Enabled = true;
			((Subchart)carlisle.TabPages[i]).tab_overlay.EditingMode = InkOverlayEditingMode.Select;
		}
	}

	private void menuItemInkBlack_Click(object sender, EventArgs e)
	{
		Make_Undoable();
		InkButton1.Pushed = true;
		menuItemInkBlue.Checked = false;
		menuItemInkErase.Checked = false;
		menuItemInkRed.Checked = false;
		menuItemInkBlack.Checked = true;
		menuItemInkOff.Checked = false;
		menuItemInkGreen.Checked = false;
		menuItemInkSelect.Checked = false;
		Ink_Enable(Color.Black);
	}

	private void menuItemInkOff_Click(object sender, EventArgs e)
	{
		InkButton1.Pushed = false;
		menuItemInkBlue.Checked = false;
		menuItemInkErase.Checked = false;
		menuItemInkRed.Checked = false;
		menuItemInkBlack.Checked = false;
		menuItemInkOff.Checked = true;
		menuItemInkGreen.Checked = false;
		menuItemInkSelect.Checked = false;
		Ink_Disable();
	}

	private void menuItemInkBlue_Click(object sender, EventArgs e)
	{
		Make_Undoable();
		InkButton1.Pushed = true;
		menuItemInkBlue.Checked = true;
		menuItemInkErase.Checked = false;
		menuItemInkRed.Checked = false;
		menuItemInkBlack.Checked = false;
		menuItemInkOff.Checked = false;
		menuItemInkGreen.Checked = false;
		menuItemInkSelect.Checked = false;
		Ink_Enable(Color.Blue);
	}

	private void menuItemInkRed_Click(object sender, EventArgs e)
	{
		Make_Undoable();
		InkButton1.Pushed = true;
		menuItemInkBlue.Checked = false;
		menuItemInkErase.Checked = false;
		menuItemInkRed.Checked = true;
		menuItemInkBlack.Checked = false;
		menuItemInkOff.Checked = false;
		menuItemInkGreen.Checked = false;
		menuItemInkSelect.Checked = false;
		Ink_Enable(Color.Red);
	}

	private void menuItemInkErase_Click(object sender, EventArgs e)
	{
		Make_Undoable();
		InkButton1.Pushed = true;
		menuItemInkBlue.Checked = false;
		menuItemInkErase.Checked = true;
		menuItemInkRed.Checked = false;
		menuItemInkBlack.Checked = false;
		menuItemInkGreen.Checked = false;
		menuItemInkOff.Checked = false;
		menuItemInkSelect.Checked = false;
		Ink_Erase();
	}

	private void menuItemInkGreen_Click(object sender, EventArgs e)
	{
		Make_Undoable();
		InkButton1.Pushed = true;
		menuItemInkBlue.Checked = false;
		menuItemInkErase.Checked = false;
		menuItemInkRed.Checked = false;
		menuItemInkGreen.Checked = true;
		menuItemInkBlack.Checked = false;
		menuItemInkOff.Checked = false;
		menuItemInkSelect.Checked = false;
		Ink_Enable(Color.Green);
	}

	private void menuItemInkSelect_Click(object sender, EventArgs e)
	{
		Make_Undoable();
		InkButton1.Pushed = true;
		menuItemInkBlue.Checked = false;
		menuItemInkErase.Checked = false;
		menuItemInkRed.Checked = false;
		menuItemInkGreen.Checked = false;
		menuItemInkBlack.Checked = false;
		menuItemInkOff.Checked = false;
		menuItemInkSelect.Checked = true;
		Ink_Select();
	}

	private void menuViewHardDrive_Click(object sender, EventArgs e)
	{
		BARTPEFileOpenList bARTPEFileOpenList = new BARTPEFileOpenList();
		bARTPEFileOpenList.Text = "Saved on Hard Drive";
		bARTPEFileOpenList.View_HD();
		bARTPEFileOpenList.ShowDialog();
	}

	private void contextMenuInsert_Popup(object sender, EventArgs e)
	{
		menuItemReturn.Visible = Component.Current_Mode == Mode.Expert;
	}

	private void menuItemGenerate_Popup(object sender, EventArgs e)
	{
		foreach (MenuItem menuItem in menuItemGenerate.MenuItems)
		{
			if (menuItem == menuGenerateStandalone)
			{
				menuItem.Enabled = Component.Current_Mode != Mode.Expert && !Component.MONO;
			}
			else if (Component.Current_Mode == Mode.Expert)
			{
				menuItem.Visible = Generators.Handles_OO(menuItem.Text);
			}
			else
			{
				menuItem.Visible = Generators.Handles_Imperative(menuItem.Text);
			}
		}
	}

	private void menuRun_Popup(object sender, EventArgs e)
	{
		menuItemRunCompiled.Enabled = Component.Current_Mode != Mode.Expert;
	}
}
