using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace raptor;

public class PromptForm : Form
{
	public delegate void Kill_Delegate_Type(PromptForm f);

	private Label promptLabel;

	private TextBox inputBox;

	private Button OKbutton;

	private Container components;

	private string result;

	public static PromptForm current;

	public static Kill_Delegate_Type Kill_delegate = Kill_Delegate;

	public PromptForm(string str, Form p)
	{
		InitializeComponent();
		if (str.Length > 0)
		{
			promptLabel.Text = str;
		}
		else
		{
			promptLabel.Text = "Please enter a number.";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(raptor.PromptForm));
		this.promptLabel = new System.Windows.Forms.Label();
		this.inputBox = new System.Windows.Forms.TextBox();
		this.OKbutton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.promptLabel.Location = new System.Drawing.Point(20, 16);
		this.promptLabel.Name = "promptLabel";
		this.promptLabel.Size = new System.Drawing.Size(236, 40);
		this.promptLabel.TabIndex = 0;
		this.promptLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.promptLabel.Paint += new System.Windows.Forms.PaintEventHandler(promptLabel_Paint);
		this.inputBox.Location = new System.Drawing.Point(16, 64);
		this.inputBox.Name = "inputBox";
		this.inputBox.Size = new System.Drawing.Size(240, 20);
		this.inputBox.TabIndex = 1;
		this.inputBox.WordWrap = false;
		this.inputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(inputBox_KeyDown);
		this.OKbutton.Location = new System.Drawing.Point(112, 96);
		this.OKbutton.Name = "OKbutton";
		this.OKbutton.Size = new System.Drawing.Size(56, 24);
		this.OKbutton.TabIndex = 2;
		this.OKbutton.Text = "OK";
		this.OKbutton.Click += new System.EventHandler(OKbutton_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(292, 134);
		base.ControlBox = false;
		base.Controls.Add(this.OKbutton);
		base.Controls.Add(this.inputBox);
		base.Controls.Add(this.promptLabel);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PromptForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Input";
		base.TopMost = true;
		base.Load += new System.EventHandler(PromptForm_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void OKbutton_Click(object sender, EventArgs e)
	{
		result = inputBox.Text;
		current = null;
		Close();
	}

	public string Go()
	{
		current = this;
		ShowDialog();
		if (result == null)
		{
			ShowDialog();
		}
		return result;
	}

	private void inputBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			OKbutton_Click(sender, e);
		}
	}

	private void PromptForm_Load(object sender, EventArgs e)
	{
		inputBox.Focus();
	}

	public static bool Close_All()
	{
		return current != null;
	}

	public static void Kill_Delegate(PromptForm f)
	{
		f.result = "0";
		f.Close();
	}

	public static void Kill()
	{
		if (current != null)
		{
			object[] args = new object[1] { current };
			current.Invoke(Kill_delegate, args);
			current = null;
		}
	}

	private void promptLabel_Paint(object sender, PaintEventArgs e)
	{
		inputBox.Focus();
	}
}
