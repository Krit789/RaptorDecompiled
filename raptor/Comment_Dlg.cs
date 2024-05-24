using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace raptor;

public class Comment_Dlg : Form
{
	private Label label1;

	private RichTextBox textBox1;

	private Button Done_button;

	private CommentBox CB;

	private Visual_Flow_Form the_form;

	private Container components;

	public Comment_Dlg(CommentBox CBpointer, Visual_Flow_Form form)
	{
		CB = CBpointer;
		the_form = form;
		InitializeComponent();
		label1.Text = "Enter the desired line(s) of comments.";
		if (CB.Text_Array != null)
		{
			textBox1.Lines = CB.Text_Array;
			textBox1.Select(textBox1.Text.Length, 0);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.Comment_Dlg));
		this.label1 = new System.Windows.Forms.Label();
		this.textBox1 = new System.Windows.Forms.RichTextBox();
		this.Done_button = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.Location = new System.Drawing.Point(39, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(280, 40);
		this.label1.TabIndex = 3;
		this.textBox1.Location = new System.Drawing.Point(15, 72);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(328, 96);
		this.textBox1.TabIndex = 1;
		this.textBox1.Text = "";
		this.Done_button.Location = new System.Drawing.Point(135, 184);
		this.Done_button.Name = "Done_button";
		this.Done_button.Size = new System.Drawing.Size(88, 24);
		this.Done_button.TabIndex = 6;
		this.Done_button.Text = "Done";
		this.Done_button.Click += new System.EventHandler(done_button_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(360, 230);
		base.Controls.Add(this.Done_button);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.label1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "Comment_Dlg";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Enter Comment";
		base.TopMost = true;
		base.Closed += new System.EventHandler(Comment_Dlg_Closed);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		base.ResumeLayout(false);
	}

	private void done_button_Click(object sender, EventArgs e)
	{
		the_form.Make_Undoable();
		CB.Text_Array = textBox1.Lines;
		if (CB.Text_Array == null || CB.Text_Array.Length == 0)
		{
			CB.parent.My_Comment = null;
			CB.text_change = false;
		}
		else
		{
			CB.text_change = true;
		}
		Close();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		textBox1.Focus();
	}

	private void Check_key(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			done_button_Click(sender, e);
		}
	}

	private void Comment_Dlg_Closed(object sender, EventArgs e)
	{
		if (CB.Text_Array == null)
		{
			CB.parent.My_Comment = null;
			CB.text_change = false;
		}
	}
}
