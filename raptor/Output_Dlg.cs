using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using interpreter;

namespace raptor;

public class Output_Dlg : Form
{
	private suggestion_result suggestion_result;

	private string current_suggestion = "";

	private syntax_result result;

	private bool error;

	private Parallelogram PAR;

	private TextBox textBox1;

	private Label label3;

	private Button done_button;

	private Graphics labelGraphics;

	private StringFormat stringFormat;

	private string error_msg;

	private Label label4;

	private Container components;

	private CheckBox new_line;

	private Label label2;

	private RichTextBox suggestionTextBox;

	private Visual_Flow_Form the_form;

	public Output_Dlg(Parallelogram Parent_Parallelogram, Visual_Flow_Form form)
	{
		PAR = Parent_Parallelogram;
		the_form = form;
		InitializeComponent();
		Dialog_Helpers.Init();
		label2.Text = "Examples:\n   \"exact text\"\n   Coins\n   \"Number of Coins: \"+Coins\n   Board[3,3]";
		textBox1.Text = PAR.Text;
		new_line.Checked = PAR.new_line;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.Output_Dlg));
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.done_button = new System.Windows.Forms.Button();
		this.label4 = new System.Windows.Forms.Label();
		this.new_line = new System.Windows.Forms.CheckBox();
		this.label2 = new System.Windows.Forms.Label();
		this.suggestionTextBox = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.textBox1.Location = new System.Drawing.Point(48, 106);
		this.textBox1.Multiline = true;
		this.textBox1.Name = "textBox1";
		this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.textBox1.Size = new System.Drawing.Size(208, 72);
		this.textBox1.TabIndex = 4;
		this.textBox1.TextChanged += new System.EventHandler(textBox1_TextChanged);
		this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.label3.Location = new System.Drawing.Point(48, 10);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(208, 16);
		this.label3.TabIndex = 12;
		this.label3.Text = "Enter Output Here";
		this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.done_button.Location = new System.Drawing.Point(120, 385);
		this.done_button.Name = "done_button";
		this.done_button.Size = new System.Drawing.Size(64, 24);
		this.done_button.TabIndex = 6;
		this.done_button.Text = "Done";
		this.done_button.Click += new System.EventHandler(done_button_Click);
		this.label4.Location = new System.Drawing.Point(8, 194);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(264, 48);
		this.label4.TabIndex = 8;
		this.new_line.Location = new System.Drawing.Point(100, 353);
		this.new_line.Name = "new_line";
		this.new_line.Size = new System.Drawing.Size(116, 24);
		this.new_line.TabIndex = 5;
		this.new_line.Text = "&End current line";
		this.label2.Location = new System.Drawing.Point(40, 34);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(216, 64);
		this.label2.TabIndex = 11;
		this.suggestionTextBox.BackColor = System.Drawing.SystemColors.Control;
		this.suggestionTextBox.Location = new System.Drawing.Point(8, 247);
		this.suggestionTextBox.Name = "suggestionTextBox";
		this.suggestionTextBox.Size = new System.Drawing.Size(272, 96);
		this.suggestionTextBox.TabIndex = 13;
		this.suggestionTextBox.Text = "";
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(288, 435);
		base.Controls.Add(this.suggestionTextBox);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.new_line);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.done_button);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.textBox1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "Output_Dlg";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Enter Output";
		base.Resize += new System.EventHandler(Output_Dlg_Resize);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void done_button_Click(object sender, EventArgs e)
	{
		result = interpreter_pkg.output_syntax(textBox1.Text, new_line.Checked, PAR);
		PAR.is_input = false;
		if (result.valid)
		{
			the_form.Make_Undoable();
			PAR.Text = textBox1.Text;
			PAR.parse_tree = result.tree;
			PAR.new_line = new_line.Checked;
			PAR.changed();
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
