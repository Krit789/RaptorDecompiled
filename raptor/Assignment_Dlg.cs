using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using interpreter;

namespace raptor;

public class Assignment_Dlg : Form
{
	private suggestion_result suggestion_result;

	private syntax_result result;

	private Rectangle Rec;

	private bool error;

	private bool is_shifted;

	private Label label1;

	private Graphics labelGraphics;

	private Visual_Flow_Form the_form;

	private string error_msg;

	private string current_suggestion = "";

	private MainMenu mainMenu1;

	private MenuItem menuHelp;

	private MenuItem menuGeneralHelp;

	private Label label4;

	private Label label3;

	private TextBox lhsTextBox;

	private RichTextBox suggestionTextBox;

	private Label label2;

	private Button done_button;

	public TextBox assignment_Text;

	private IContainer components;

	public Assignment_Dlg(Rectangle parent_Rec, Visual_Flow_Form form)
	{
		Rec = parent_Rec;
		the_form = form;
		InitializeComponent();
		Dialog_Helpers.Init();
		label1.Text = "Enter an assignment.\n\nExamples:\n   Set Coins to 5\n   Set Count to Count + 1\n   Set Board[3,3] to 0";
		labelGraphics = label2.CreateGraphics();
		if (Rec.Text != null)
		{
			int num = Rec.Text.IndexOf(":=");
			if (num > 0)
			{
				lhsTextBox.Text = Rec.Text.Substring(0, num);
				assignment_Text.Text = Rec.Text.Substring(num + 2, Rec.Text.Length - (num + 2));
			}
			else
			{
				lhsTextBox.Text = Rec.Text;
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.Assignment_Dlg));
		this.label1 = new System.Windows.Forms.Label();
		this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
		this.menuHelp = new System.Windows.Forms.MenuItem();
		this.menuGeneralHelp = new System.Windows.Forms.MenuItem();
		this.label4 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.lhsTextBox = new System.Windows.Forms.TextBox();
		this.suggestionTextBox = new System.Windows.Forms.RichTextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.done_button = new System.Windows.Forms.Button();
		this.assignment_Text = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		this.label1.Location = new System.Drawing.Point(51, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(221, 80);
		this.label1.TabIndex = 3;
		this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[1] { this.menuHelp });
		this.menuHelp.Index = 0;
		this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[1] { this.menuGeneralHelp });
		this.menuHelp.Text = "&Help";
		this.menuGeneralHelp.Index = 0;
		this.menuGeneralHelp.Shortcut = System.Windows.Forms.Shortcut.F1;
		this.menuGeneralHelp.Text = "&General Help";
		this.menuGeneralHelp.Click += new System.EventHandler(menuGeneralHelp_Click);
		this.label4.Location = new System.Drawing.Point(16, 152);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(16, 16);
		this.label4.TabIndex = 15;
		this.label4.Text = "to";
		this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.label3.Location = new System.Drawing.Point(16, 112);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(32, 24);
		this.label3.TabIndex = 14;
		this.label3.Text = "Set";
		this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.lhsTextBox.Location = new System.Drawing.Point(48, 112);
		this.lhsTextBox.Name = "lhsTextBox";
		this.lhsTextBox.Size = new System.Drawing.Size(200, 20);
		this.lhsTextBox.TabIndex = 9;
		this.lhsTextBox.TextChanged += new System.EventHandler(Check_Hint_Lhs);
		this.lhsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox2_KeyDown);
		this.lhsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox2_KeyPress);
		this.suggestionTextBox.Location = new System.Drawing.Point(8, 304);
		this.suggestionTextBox.Name = "suggestionTextBox";
		this.suggestionTextBox.ReadOnly = true;
		this.suggestionTextBox.Size = new System.Drawing.Size(272, 96);
		this.suggestionTextBox.TabIndex = 12;
		this.suggestionTextBox.TabStop = false;
		this.suggestionTextBox.Text = "";
		this.suggestionTextBox.Visible = false;
		this.suggestionTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(textBox1_MouseDown);
		this.label2.Location = new System.Drawing.Point(16, 248);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(264, 48);
		this.label2.TabIndex = 11;
		this.done_button.Location = new System.Drawing.Point(104, 408);
		this.done_button.Name = "done_button";
		this.done_button.Size = new System.Drawing.Size(88, 24);
		this.done_button.TabIndex = 16;
		this.done_button.Text = "Done";
		this.done_button.Click += new System.EventHandler(done_button_Click);
		this.assignment_Text.AcceptsReturn = true;
		this.assignment_Text.AcceptsTab = true;
		this.assignment_Text.Location = new System.Drawing.Point(48, 152);
		this.assignment_Text.Multiline = true;
		this.assignment_Text.Name = "assignment_Text";
		this.assignment_Text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.assignment_Text.Size = new System.Drawing.Size(200, 72);
		this.assignment_Text.TabIndex = 13;
		this.assignment_Text.TextChanged += new System.EventHandler(Check_Hint);
		this.assignment_Text.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.assignment_Text.KeyPress += new System.Windows.Forms.KeyPressEventHandler(assignment_Text_KeyPress);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(296, 449);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.lhsTextBox);
		base.Controls.Add(this.assignment_Text);
		base.Controls.Add(this.suggestionTextBox);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.done_button);
		base.Controls.Add(this.label1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Menu = this.mainMenu1;
		base.Name = "Assignment_Dlg";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Enter Statement";
		base.TopMost = true;
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		base.Resize += new System.EventHandler(Assignment_Dlg_Resize);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void done_button_Click(object sender, EventArgs e)
	{
		if (Rec.Text != null && Rec.Text != "" && Rec.Text.CompareTo(lhsTextBox.Text + ":=" + assignment_Text.Text) == 0)
		{
			Close();
			return;
		}
		result = interpreter_pkg.assignment_syntax(lhsTextBox.Text, assignment_Text.Text, Rec);
		if (result.valid)
		{
			the_form.Make_Undoable();
			Rec.Text = lhsTextBox.Text + ":=" + assignment_Text.Text;
			Rec.parse_tree = result.tree;
			Rec.changed();
			error = false;
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

	protected override void OnPaint(PaintEventArgs e)
	{
		int location = ((result != null) ? result.location : 0);
		labelGraphics = label2.CreateGraphics();
		Dialog_Helpers.Paint_Helper(labelGraphics, lhsTextBox.Text + " " + Component.assignmentSymbol + assignment_Text.Text, label1, error_msg, location, error);
	}

	private bool Complete_Suggestion()
	{
		return Dialog_Helpers.Complete_Suggestion(assignment_Text, interpreter_pkg.expr_dialog, current_suggestion, ref suggestion_result);
	}

	private bool Complete_Suggestion_Lhs()
	{
		return Dialog_Helpers.Complete_Suggestion(lhsTextBox, interpreter_pkg.lhs_dialog, current_suggestion, ref suggestion_result);
	}

	private void Check_key(object sender, KeyEventArgs e)
	{
		if (e.Shift)
		{
			is_shifted = true;
		}
		else
		{
			is_shifted = false;
		}
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = Complete_Suggestion();
			if (!e.Handled)
			{
				e.Handled = true;
				assignment_Text.Select(assignment_Text.TextLength, 0);
				done_button_Click(sender, e);
			}
		}
		else if (e.KeyCode.ToString() == "Down")
		{
			e.Handled = true;
			Dialog_Helpers.suggestions_downarrow(suggestionTextBox, ref current_suggestion);
		}
		else if (e.KeyCode.ToString() == "Up")
		{
			e.Handled = true;
			Dialog_Helpers.suggestions_uparrow(suggestionTextBox, ref current_suggestion);
		}
	}

	private void Check_Hint(object sender, EventArgs e)
	{
		Dialog_Helpers.Check_Hint(assignment_Text, suggestionTextBox, interpreter_pkg.expr_dialog, ref current_suggestion, ref suggestion_result, ref error, Font);
		Invalidate();
	}

	private void Check_Hint_Lhs(object sender, EventArgs e)
	{
		Dialog_Helpers.Check_Hint(lhsTextBox, suggestionTextBox, interpreter_pkg.lhs_dialog, ref current_suggestion, ref suggestion_result, ref error, Font);
		Invalidate();
	}

	private void assignment_Text_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\t')
		{
			e.Handled = Complete_Suggestion();
			if (!e.Handled)
			{
				if (is_shifted)
				{
					lhsTextBox.Focus();
				}
				else
				{
					done_button.Focus();
				}
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
		Dialog_Helpers.suggestions_mousedown(suggestionTextBox, ref current_suggestion, e);
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

	private void textBox2_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = Complete_Suggestion_Lhs();
			e.SuppressKeyPress = e.Handled;
			if (!e.Handled)
			{
				done_button_Click(sender, e);
			}
		}
		else if (e.KeyCode.ToString() == "Down")
		{
			e.Handled = true;
			e.SuppressKeyPress = e.Handled;
			Dialog_Helpers.suggestions_downarrow(suggestionTextBox, ref current_suggestion);
		}
		else if (e.KeyCode.ToString() == "Up")
		{
			e.Handled = true;
			e.SuppressKeyPress = e.Handled;
			Dialog_Helpers.suggestions_uparrow(suggestionTextBox, ref current_suggestion);
		}
	}

	private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\t')
		{
			e.Handled = Complete_Suggestion();
			if (!e.Handled)
			{
				if (is_shifted)
				{
					done_button.Focus();
				}
				else
				{
					assignment_Text.Focus();
				}
			}
			e.Handled = true;
		}
		else if (e.KeyChar == '\u001b')
		{
			Close();
		}
	}

	private void Assignment_Dlg_Resize(object sender, EventArgs e)
	{
		label2.Width = base.Width - 40;
		lhsTextBox.Width = base.Width - 104;
		assignment_Text.Width = base.Width - 104;
		label2.Invalidate();
		lhsTextBox.Invalidate();
		assignment_Text.Invalidate();
	}
}
