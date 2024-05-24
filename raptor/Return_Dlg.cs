using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using interpreter;

namespace raptor;

public class Return_Dlg : Form
{
	private suggestion_result suggestion_result;

	private string current_suggestion = "";

	private syntax_result result;

	private bool error;

	private Oval_Return RETURN;

	private TextBox textBox1;

	private Label label3;

	private Button done_button;

	private Graphics labelGraphics;

	private StringFormat stringFormat;

	private string error_msg;

	private Label label4;

	private Container components;

	private Label label2;

	private RichTextBox suggestionTextBox;

	private Visual_Flow_Form the_form;

	public Return_Dlg(Oval_Return Parent_Oval, Visual_Flow_Form form)
	{
		RETURN = Parent_Oval;
		the_form = form;
		InitializeComponent();
		Dialog_Helpers.Init();
		if (RETURN.Text != null && RETURN.Text.CompareTo("") != 0)
		{
			textBox1.Text = RETURN.Text;
		}
		labelGraphics = label4.CreateGraphics();
		stringFormat = new StringFormat();
		stringFormat.LineAlignment = StringAlignment.Center;
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
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.done_button = new System.Windows.Forms.Button();
		this.label4 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.suggestionTextBox = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.textBox1.Location = new System.Drawing.Point(48, 102);
		this.textBox1.Multiline = true;
		this.textBox1.Name = "textBox1";
		this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.textBox1.Size = new System.Drawing.Size(208, 72);
		this.textBox1.TabIndex = 4;
		this.textBox1.TextChanged += new System.EventHandler(textBox1_TextChanged);
		this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.label3.Location = new System.Drawing.Point(48, 6);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(208, 16);
		this.label3.TabIndex = 12;
		this.label3.Text = "Enter variable to return here";
		this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.done_button.Location = new System.Drawing.Point(120, 381);
		this.done_button.Name = "done_button";
		this.done_button.Size = new System.Drawing.Size(64, 24);
		this.done_button.TabIndex = 6;
		this.done_button.Text = "Done";
		this.done_button.Click += new System.EventHandler(done_button_Click);
		this.label4.Location = new System.Drawing.Point(8, 190);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(264, 48);
		this.label4.TabIndex = 8;
		this.label2.Location = new System.Drawing.Point(40, 30);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(216, 64);
		this.label2.TabIndex = 11;
		this.suggestionTextBox.BackColor = System.Drawing.SystemColors.Control;
		this.suggestionTextBox.Location = new System.Drawing.Point(8, 243);
		this.suggestionTextBox.Name = "suggestionTextBox";
		this.suggestionTextBox.Size = new System.Drawing.Size(272, 96);
		this.suggestionTextBox.TabIndex = 13;
		this.suggestionTextBox.Text = "";
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(288, 422);
		base.Controls.Add(this.suggestionTextBox);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.done_button);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.textBox1);
		base.Name = "Return_Dlg";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Enter Return";
		base.Resize += new System.EventHandler(Output_Dlg_Resize);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void radioButton2_CheckedChanged(object sender, EventArgs e)
	{
		label2.Text = "Examples:\n   Coins\n   \"Number of Coins: \"+Coins\n   Board[3,3]";
		textBox1_TextChanged(sender, e);
	}

	private void radioButton3_CheckedChanged(object sender, EventArgs e)
	{
		label2.Text = "Examples:\n   Welcome to tic-tac-toe\n   The total is\n   The word is \"blue\"";
		textBox1_TextChanged(sender, e);
	}

	private void done_button_Click(object sender, EventArgs e)
	{
		if (textBox1.Text.Contains("("))
		{
			result = new syntax_result();
			result.valid = false;
			result.location = textBox1.Text.IndexOf("(") + 1;
			result.message = "can not call function in RETURN";
		}
		else
		{
			result = interpreter_pkg.output_syntax(textBox1.Text, new_line: false, RETURN);
		}
		if (result.valid)
		{
			the_form.Make_Undoable();
			RETURN.Text = textBox1.Text;
			RETURN.parse_tree = result.tree;
			RETURN.changed();
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
		labelGraphics = label4.CreateGraphics();
		Dialog_Helpers.Paint_Helper(labelGraphics, textBox1.Text, label2, error_msg, location, error);
		textBox1.Focus();
	}

	private bool Complete_Suggestion()
	{
		return Dialog_Helpers.Complete_Suggestion(textBox1, interpreter_pkg.expr_dialog, current_suggestion, ref suggestion_result);
	}

	private void Check_key(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = Complete_Suggestion();
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

	private void Output_Dlg_Resize(object sender, EventArgs e)
	{
		label4.Width = base.Width - 32;
		textBox1.Width = base.Width - 88;
		label4.Invalidate();
		textBox1.Invalidate();
	}

	private void textBox1_TextChanged(object sender, EventArgs e)
	{
		if (textBox1.Lines.Length > 1)
		{
			textBox1.Text = textBox1.Lines[0] + textBox1.Lines[1];
			textBox1.Select(textBox1.Text.Length, 0);
		}
		Dialog_Helpers.Check_Hint(textBox1, suggestionTextBox, interpreter_pkg.expr_dialog, ref current_suggestion, ref suggestion_result, ref error, Font);
		Invalidate();
	}
}
