using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using interpreter;

namespace raptor;

public class Call_Dialog : Form
{
	private suggestion_result suggestion_result;

	private syntax_result result;

	private Rectangle Rec;

	private bool error;

	public TextBox assignment_Text;

	private Button done_button;

	private Label label1;

	private Label label2;

	private Graphics labelGraphics;

	private Visual_Flow_Form the_form;

	private string error_msg;

	private RichTextBox textBox1;

	private string current_suggestion = "";

	private MainMenu mainMenu1;

	private MenuItem menuHelp;

	private MenuItem menuGeneralHelp;

	private IContainer components;

	public Call_Dialog(Rectangle parent_Rec, Visual_Flow_Form form)
	{
		Rec = parent_Rec;
		the_form = form;
		InitializeComponent();
		Dialog_Helpers.Init();
		label1.Text = "Enter a procedure call.\n\nExamples:\n   Wait_For_Mouse_Button(Left_Button)\n   Open_Graph_Window(300,300)";
		labelGraphics = label2.CreateGraphics();
		if (Rec.Text != null)
		{
			assignment_Text.Text = Rec.Text;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.Call_Dialog));
		this.assignment_Text = new System.Windows.Forms.TextBox();
		this.done_button = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.textBox1 = new System.Windows.Forms.RichTextBox();
		this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
		this.menuHelp = new System.Windows.Forms.MenuItem();
		this.menuGeneralHelp = new System.Windows.Forms.MenuItem();
		base.SuspendLayout();
		this.assignment_Text.AcceptsReturn = true;
		this.assignment_Text.AcceptsTab = true;
		this.assignment_Text.Location = new System.Drawing.Point(48, 96);
		this.assignment_Text.Multiline = true;
		this.assignment_Text.Name = "assignment_Text";
		this.assignment_Text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.assignment_Text.Size = new System.Drawing.Size(200, 72);
		this.assignment_Text.TabIndex = 1;
		this.assignment_Text.TextChanged += new System.EventHandler(Check_Hint);
		this.assignment_Text.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.assignment_Text.KeyPress += new System.Windows.Forms.KeyPressEventHandler(assignment_Text_KeyPress);
		this.done_button.Location = new System.Drawing.Point(104, 368);
		this.done_button.Name = "done_button";
		this.done_button.Size = new System.Drawing.Size(88, 24);
		this.done_button.TabIndex = 2;
		this.done_button.Text = "Done";
		this.done_button.Click += new System.EventHandler(done_button_Click);
		this.label1.Location = new System.Drawing.Point(51, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(221, 80);
		this.label1.TabIndex = 3;
		this.label2.Location = new System.Drawing.Point(16, 200);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(264, 48);
		this.label2.TabIndex = 4;
		this.textBox1.Location = new System.Drawing.Point(8, 264);
		this.textBox1.Name = "textBox1";
		this.textBox1.ReadOnly = true;
		this.textBox1.Size = new System.Drawing.Size(272, 96);
		this.textBox1.TabIndex = 5;
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
		base.ClientSize = new System.Drawing.Size(288, 401);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.done_button);
		base.Controls.Add(this.assignment_Text);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Menu = this.mainMenu1;
		base.Name = "Call_Dialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Enter Call";
		base.TopMost = true;
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		base.Resize += new System.EventHandler(Call_Dialog_Resize);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private bool CreateNewTab(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if (!char.IsLetterOrDigit(s[i]) && s[i] != '_')
			{
				return false;
			}
		}
		if (!char.IsLetter(s, 0) || !token_helpers_pkg.verify_id(s))
		{
			return false;
		}
		DialogResult dialogResult = ((Component.Current_Mode == Mode.Expert) ? DialogResult.No : MessageBox.Show("Do you wish to create a new tab named " + s, "Create new tab?", MessageBoxButtons.YesNo));
		if (dialogResult == DialogResult.Yes)
		{
			Subchart subchart = new Subchart(the_form, s);
			Undo_Stack.Make_Add_Tab_Undoable(the_form, subchart);
			the_form.Make_Undoable();
			the_form.carlisle.TabPages.Add(subchart);
			result = interpreter_pkg.call_syntax(assignment_Text.Text, Rec);
			the_form.carlisle.SelectedTab = subchart;
			Rec.Text = assignment_Text.Text;
			Rec.parse_tree = result.tree;
			error = false;
			Rec.changed();
			the_form.flow_panel.Invalidate();
			Close();
			return true;
		}
		return false;
	}

	private void done_button_Click(object sender, EventArgs e)
	{
		if (assignment_Text.Text == "")
		{
			return;
		}
		if (Rec.Text != null && Rec.Text != "" && assignment_Text.Text.CompareTo(Rec.Text) == 0)
		{
			Close();
			return;
		}
		result = interpreter_pkg.call_syntax(assignment_Text.Text, Rec);
		if (result.valid || assignment_Text.Text.Length < 2 || !CreateNewTab(assignment_Text.Text))
		{
			if (result.valid)
			{
				the_form.Make_Undoable();
				Rec.Text = assignment_Text.Text;
				Rec.parse_tree = result.tree;
				error = false;
				Rec.changed();
				the_form.flow_panel.Invalidate();
				Close();
			}
			else
			{
				error = true;
				error_msg = result.message;
				Invalidate();
			}
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		int location = ((result != null) ? result.location : 0);
		labelGraphics = label2.CreateGraphics();
		Dialog_Helpers.Paint_Helper(labelGraphics, assignment_Text.Text, label1, error_msg, location, error);
		assignment_Text.Focus();
	}

	private bool Complete_Suggestion()
	{
		return Dialog_Helpers.Complete_Suggestion(assignment_Text, interpreter_pkg.call_dialog, current_suggestion, ref suggestion_result);
	}

	private void Check_key(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = Complete_Suggestion();
			if (!e.Handled)
			{
				e.Handled = true;
				done_button_Click(sender, e);
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
		Dialog_Helpers.Check_Hint(assignment_Text, textBox1, interpreter_pkg.call_dialog, ref current_suggestion, ref suggestion_result, ref error, Font);
		Invalidate();
	}

	private void assignment_Text_KeyPress(object sender, KeyPressEventArgs e)
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
		assignment_Text.Focus();
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

	private void Call_Dialog_Resize(object sender, EventArgs e)
	{
		label2.Width = base.Width - 32;
		assignment_Text.Width = base.Width - 96;
		label2.Invalidate();
		assignment_Text.Invalidate();
	}
}
