using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using interpreter;

namespace raptor;

public class Control_Dlg : Form
{
	private suggestion_result suggestion_result;

	private syntax_result result;

	private Component Cmp;

	private bool error;

	public TextBox Control_Text;

	private Button done_button;

	private Label label1;

	private Label label2;

	private Graphics labelGraphics;

	private string error_msg;

	private Splitter splitter1;

	private RichTextBox textBox1;

	private string current_suggestion;

	private IContainer components;

	private MainMenu mainMenu1;

	private MenuItem menuHelp;

	private MenuItem menuGeneralHelp;

	private Visual_Flow_Form the_form;

	private string examples = "Examples:\n   Count = X+2\n   Count != 5\n   Score_Array[4] < 10\n   Middle <= Y and Y <= Top";

	public Control_Dlg(Component parent_Cmp, Visual_Flow_Form form, bool is_loop)
	{
		Cmp = parent_Cmp;
		the_form = form;
		InitializeComponent();
		Dialog_Helpers.Init();
		if (is_loop)
		{
			if (!Component.reverse_loop_logic)
			{
				label1.Text = "Enter loop exit condition.\n\n" + examples;
			}
			else
			{
				label1.Text = "Enter loop condition.\n\n" + examples;
			}
			Text = "Enter Loop Condition";
		}
		else
		{
			label1.Text = "Enter selection condition.\n\n" + examples;
			Text = "Enter Selection Condition";
		}
		labelGraphics = label2.CreateGraphics();
		if (Cmp.Text != null)
		{
			Control_Text.Text = Cmp.Text;
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

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.Control_Dlg));
		this.Control_Text = new System.Windows.Forms.TextBox();
		this.done_button = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.splitter1 = new System.Windows.Forms.Splitter();
		this.textBox1 = new System.Windows.Forms.RichTextBox();
		this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
		this.menuHelp = new System.Windows.Forms.MenuItem();
		this.menuGeneralHelp = new System.Windows.Forms.MenuItem();
		base.SuspendLayout();
		this.Control_Text.AcceptsReturn = true;
		this.Control_Text.AcceptsTab = true;
		this.Control_Text.Location = new System.Drawing.Point(48, 96);
		this.Control_Text.Multiline = true;
		this.Control_Text.Name = "Control_Text";
		this.Control_Text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.Control_Text.Size = new System.Drawing.Size(200, 72);
		this.Control_Text.TabIndex = 1;
		this.Control_Text.TextChanged += new System.EventHandler(Check_Hint);
		this.Control_Text.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.Control_Text.KeyPress += new System.Windows.Forms.KeyPressEventHandler(Control_Text_KeyPress);
		this.Control_Text.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.done_button.Location = new System.Drawing.Point(104, 368);
		this.done_button.Name = "done_button";
		this.done_button.Size = new System.Drawing.Size(88, 24);
		this.done_button.TabIndex = 2;
		this.done_button.Text = "Done";
		this.done_button.Click += new System.EventHandler(done_button_Click);
		this.label1.Location = new System.Drawing.Point(51, 8);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(221, 88);
		this.label1.TabIndex = 3;
		this.label2.Location = new System.Drawing.Point(16, 184);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(264, 48);
		this.label2.TabIndex = 4;
		this.splitter1.Location = new System.Drawing.Point(0, 0);
		this.splitter1.Name = "splitter1";
		this.splitter1.Size = new System.Drawing.Size(3, 401);
		this.splitter1.TabIndex = 5;
		this.splitter1.TabStop = false;
		this.textBox1.Location = new System.Drawing.Point(8, 248);
		this.textBox1.Name = "textBox1";
		this.textBox1.ReadOnly = true;
		this.textBox1.Size = new System.Drawing.Size(272, 112);
		this.textBox1.TabIndex = 6;
		this.textBox1.TabStop = false;
		this.textBox1.Text = "";
		this.textBox1.Visible = false;
		this.textBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(textBox1_MouseDown);
		this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[1] { this.menuHelp });
		this.menuHelp.Index = 0;
		this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[1] { this.menuGeneralHelp });
		this.menuHelp.Text = "&Help";
		this.menuGeneralHelp.Index = 0;
		this.menuGeneralHelp.Shortcut = System.Windows.Forms.Shortcut.F1;
		this.menuGeneralHelp.Text = "&General Help";
		this.menuGeneralHelp.Click += new System.EventHandler(menuGeneralHelp_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(287, 401);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.splitter1);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.done_button);
		base.Controls.Add(this.Control_Text);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Menu = this.mainMenu1;
		base.Name = "Control_Dlg";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Control_Dlg";
		base.TopMost = true;
		base.Resize += new System.EventHandler(Control_Dlg_Resize);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void done_button_Click(object sender, EventArgs e)
	{
		if (Control_Text.Text.CompareTo(Cmp.Text) == 0)
		{
			Close();
			return;
		}
		result = interpreter_pkg.conditional_syntax(Control_Text.Text, Cmp);
		if (result.valid)
		{
			the_form.Make_Undoable();
			Cmp.Text = Control_Text.Text;
			Cmp.parse_tree = result.tree;
			Cmp.changed();
			error = false;
			Close();
		}
		else
		{
			error = true;
			error_msg = result.message;
			Invalidate();
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		int location = ((result != null) ? result.location : 0);
		labelGraphics = label2.CreateGraphics();
		Dialog_Helpers.Paint_Helper(labelGraphics, Control_Text.Text, label1, error_msg, location, error);
		Control_Text.Focus();
	}

	private bool Complete_Suggestion()
	{
		return Dialog_Helpers.Complete_Suggestion(Control_Text, interpreter_pkg.expr_dialog, current_suggestion, ref suggestion_result);
	}

	private void Check_key(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = Complete_Suggestion();
			if (!e.Handled)
			{
				Control_Text.Select(Control_Text.TextLength, 0);
				done_button_Click(sender, e);
				e.Handled = true;
			}
		}
		else if (e.KeyCode.ToString() == "Down")
		{
			e.Handled = true;
			Dialog_Helpers.suggestions_downarrow(textBox1, ref current_suggestion);
		}
		else if (e.KeyCode.ToString() == "Up")
		{
			e.Handled = true;
			Dialog_Helpers.suggestions_uparrow(textBox1, ref current_suggestion);
		}
	}

	private void Check_Hint(object sender, EventArgs e)
	{
		Dialog_Helpers.Check_Hint(Control_Text, textBox1, interpreter_pkg.expr_dialog, ref current_suggestion, ref suggestion_result, ref error, Font);
		Invalidate();
	}

	private void Control_Text_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\t')
		{
			e.Handled = Complete_Suggestion();
			if (!e.Handled)
			{
				done_button.Focus();
			}
			e.Handled = true;
		}
		else if (e.KeyChar == '\u001b')
		{
			Close();
		}
	}

	private void textBox1_MouseDown(object sender, MouseEventArgs e)
	{
		Dialog_Helpers.suggestions_mousedown(textBox1, ref current_suggestion, e);
		if (e.Clicks == 2)
		{
			Complete_Suggestion();
		}
		Control_Text.Focus();
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

	private void Control_Text_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = true;
		}
	}

	private void Control_Dlg_Resize(object sender, EventArgs e)
	{
		label2.Width = base.Width - 31;
		Control_Text.Width = base.Width - 95;
		Control_Text.Invalidate();
		label2.Invalidate();
	}
}
