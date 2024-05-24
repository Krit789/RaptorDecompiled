using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using interpreter;

namespace raptor;

public class Input_Dlg : Form
{
	private suggestion_result suggestion_result_expr;

	private string current_suggestion_expr = "";

	private suggestion_result suggestion_result_var;

	private string current_suggestion_var = "";

	private syntax_result result;

	private syntax_result prompt_result;

	private bool error;

	private Parallelogram PAR;

	private Label examplesLabel;

	private TextBox variableTextBox;

	private Label label2;

	private TextBox exprTextBox;

	private Label label3;

	private Button done_button;

	private Graphics labelGraphics;

	private StringFormat stringFormat;

	private string error_msg;

	private Label errorLabel;

	private Container components;

	private RichTextBox suggestionTextBox;

	private Visual_Flow_Form the_form;

	public Input_Dlg(Parallelogram Parent_Parallelogram, Visual_Flow_Form form)
	{
		PAR = Parent_Parallelogram;
		the_form = form;
		InitializeComponent();
		Dialog_Helpers.Init();
		if (PAR.Text != null && PAR.Text.CompareTo("") != 0)
		{
			exprTextBox.Text = PAR.prompt;
			variableTextBox.Text = PAR.Text;
		}
		examplesLabel.Text = "Examples:\n   Coins\n   Board[3,3]";
		labelGraphics = errorLabel.CreateGraphics();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.Input_Dlg));
		this.examplesLabel = new System.Windows.Forms.Label();
		this.variableTextBox = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.exprTextBox = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.done_button = new System.Windows.Forms.Button();
		this.errorLabel = new System.Windows.Forms.Label();
		this.suggestionTextBox = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.examplesLabel.Location = new System.Drawing.Point(40, 145);
		this.examplesLabel.Name = "examplesLabel";
		this.examplesLabel.Size = new System.Drawing.Size(216, 40);
		this.examplesLabel.TabIndex = 0;
		this.variableTextBox.Location = new System.Drawing.Point(48, 193);
		this.variableTextBox.Name = "variableTextBox";
		this.variableTextBox.Size = new System.Drawing.Size(208, 20);
		this.variableTextBox.TabIndex = 5;
		this.variableTextBox.WordWrap = false;
		this.variableTextBox.TextChanged += new System.EventHandler(variableTextBox_TextChanged);
		this.variableTextBox.Enter += new System.EventHandler(variableTextBox_TextChanged);
		this.variableTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.label2.Location = new System.Drawing.Point(48, 8);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(208, 16);
		this.label2.TabIndex = 4;
		this.label2.Text = "Enter Prompt Here";
		this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.exprTextBox.Location = new System.Drawing.Point(48, 33);
		this.exprTextBox.Multiline = true;
		this.exprTextBox.Name = "exprTextBox";
		this.exprTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.exprTextBox.Size = new System.Drawing.Size(208, 72);
		this.exprTextBox.TabIndex = 3;
		this.exprTextBox.TextChanged += new System.EventHandler(textBox2_TextChanged);
		this.exprTextBox.Enter += new System.EventHandler(textBox2_TextChanged);
		this.exprTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key_expr);
		this.label3.Location = new System.Drawing.Point(48, 121);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(208, 16);
		this.label3.TabIndex = 6;
		this.label3.Text = "Enter Variable Here";
		this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.done_button.Location = new System.Drawing.Point(120, 415);
		this.done_button.Name = "done_button";
		this.done_button.Size = new System.Drawing.Size(64, 24);
		this.done_button.TabIndex = 7;
		this.done_button.Text = "Done";
		this.done_button.Click += new System.EventHandler(done_button_Click);
		this.errorLabel.BackColor = System.Drawing.SystemColors.Control;
		this.errorLabel.Location = new System.Drawing.Point(16, 233);
		this.errorLabel.Name = "errorLabel";
		this.errorLabel.Size = new System.Drawing.Size(264, 48);
		this.errorLabel.TabIndex = 8;
		this.suggestionTextBox.BackColor = System.Drawing.SystemColors.Control;
		this.suggestionTextBox.Location = new System.Drawing.Point(8, 297);
		this.suggestionTextBox.Name = "suggestionTextBox";
		this.suggestionTextBox.Size = new System.Drawing.Size(272, 96);
		this.suggestionTextBox.TabIndex = 11;
		this.suggestionTextBox.Text = "";
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(299, 448);
		base.Controls.Add(this.suggestionTextBox);
		base.Controls.Add(this.errorLabel);
		base.Controls.Add(this.done_button);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.exprTextBox);
		base.Controls.Add(this.variableTextBox);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.examplesLabel);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "Input_Dlg";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Enter Input";
		base.TopMost = true;
		base.Resize += new System.EventHandler(Input_Dlg_Resize);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void done_button_Click(object sender, EventArgs e)
	{
		result = interpreter_pkg.input_syntax(variableTextBox.Text, PAR);
		prompt_result = interpreter_pkg.output_syntax(exprTextBox.Text, new_line: false, PAR);
		PAR.is_input = true;
		if (result.valid && prompt_result.valid)
		{
			the_form.Make_Undoable();
			PAR.prompt = exprTextBox.Text;
			PAR.Text = variableTextBox.Text;
			PAR.parse_tree = result.tree;
			PAR.prompt_tree = prompt_result.tree;
			PAR.input_is_expression = true;
			PAR.changed();
			error = false;
			Close();
		}
		else
		{
			error = true;
			if (!result.valid)
			{
				error_msg = result.message;
			}
			else
			{
				error_msg = prompt_result.message;
			}
			Invalidate();
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Console.WriteLine(error);
		int location;
		string text;
		if (result != null && !result.valid)
		{
			location = result.location;
			text = variableTextBox.Text;
		}
		else if (prompt_result != null && !prompt_result.valid)
		{
			location = prompt_result.location;
			text = exprTextBox.Text;
		}
		else
		{
			location = 0;
			text = "";
		}
		labelGraphics = errorLabel.CreateGraphics();
		Dialog_Helpers.Paint_Helper(labelGraphics, text, examplesLabel, error_msg, location, error);
	}

	private bool Complete_Suggestion_Expr()
	{
		return Dialog_Helpers.Complete_Suggestion(exprTextBox, interpreter_pkg.expr_dialog, current_suggestion_expr, ref suggestion_result_expr);
	}

	private bool Complete_Suggestion_Var()
	{
		return Dialog_Helpers.Complete_Suggestion(variableTextBox, interpreter_pkg.lhs_dialog, current_suggestion_var, ref suggestion_result_var);
	}

	private void Check_key(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = Complete_Suggestion_Var();
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
			Dialog_Helpers.suggestions_downarrow(suggestionTextBox, ref current_suggestion_var);
		}
		else if (e.KeyCode.ToString() == "Up")
		{
			e.Handled = true;
			e.SuppressKeyPress = e.Handled;
			Dialog_Helpers.suggestions_uparrow(suggestionTextBox, ref current_suggestion_var);
		}
	}

	private void Check_key_expr(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = Complete_Suggestion_Expr();
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
			Dialog_Helpers.suggestions_downarrow(suggestionTextBox, ref current_suggestion_expr);
		}
		else if (e.KeyCode.ToString() == "Up")
		{
			e.Handled = true;
			e.SuppressKeyPress = e.Handled;
			Dialog_Helpers.suggestions_uparrow(suggestionTextBox, ref current_suggestion_expr);
		}
	}

	private void Input_Dlg_Resize(object sender, EventArgs e)
	{
		errorLabel.Width = base.Width - 40;
		exprTextBox.Width = base.Width - 96;
		variableTextBox.Width = base.Width - 96;
		exprTextBox.Invalidate();
		variableTextBox.Invalidate();
		errorLabel.Invalidate();
	}

	private void textBox2_TextChanged(object sender, EventArgs e)
	{
		if (exprTextBox.Lines.Length > 1)
		{
			exprTextBox.Text = exprTextBox.Lines[0] + exprTextBox.Lines[1];
			exprTextBox.Select(exprTextBox.Text.Length, 0);
		}
		Dialog_Helpers.Check_Hint(exprTextBox, suggestionTextBox, interpreter_pkg.expr_dialog, ref current_suggestion_expr, ref suggestion_result_expr, ref error, Font);
		Invalidate();
	}

	private void variableTextBox_TextChanged(object sender, EventArgs e)
	{
		Dialog_Helpers.Check_Hint(variableTextBox, suggestionTextBox, interpreter_pkg.lhs_dialog, ref current_suggestion_var, ref suggestion_result_var, ref error, Font);
		Invalidate();
	}
}
