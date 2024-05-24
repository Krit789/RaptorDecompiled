using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace raptor;

public class Subchart_name : Form
{
	private TextBox textBox1;

	private Button button1;

	private Label label1;

	private Label label2;

	private Label label3;

	private static string result;

	private string init_name;

	private Label label4;

	private Visual_Flow_Form form;

	private Button buttonCancel;

	private Container components;

	public Subchart_name()
	{
		InitializeComponent();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	public static string RunDialog(string s, Visual_Flow_Form the_form)
	{
		Subchart_name subchart_name = new Subchart_name();
		subchart_name.textBox1.Text = s;
		subchart_name.init_name = s.ToLower();
		subchart_name.form = the_form;
		subchart_name.ShowDialog();
		return result;
	}

	private void InitializeComponent()
	{
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.buttonCancel = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.textBox1.AcceptsReturn = true;
		this.textBox1.Location = new System.Drawing.Point(22, 176);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(248, 20);
		this.textBox1.TabIndex = 0;
		this.textBox1.Text = "textBox1";
		this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.button1.Location = new System.Drawing.Point(42, 211);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 1;
		this.button1.Text = "Ok";
		this.button1.Click += new System.EventHandler(button1_Click);
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(40, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(216, 24);
		this.label1.TabIndex = 2;
		this.label1.Text = "Please enter name of subchart";
		this.label2.Location = new System.Drawing.Point(36, 48);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(224, 40);
		this.label2.TabIndex = 3;
		this.label2.Text = "Name must begin with letter, and contain only letters, numbers and underscores.";
		this.label3.Location = new System.Drawing.Point(36, 87);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(224, 57);
		this.label3.TabIndex = 4;
		this.label3.Text = "Examples:";
		this.label4.Location = new System.Drawing.Point(32, 152);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(224, 16);
		this.label4.TabIndex = 5;
		this.buttonCancel.Location = new System.Drawing.Point(180, 211);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 6;
		this.buttonCancel.Text = "Cancel";
		this.buttonCancel.Click += new System.EventHandler(button2_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(296, 246);
		base.ControlBox = false;
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.textBox1);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Subchart_name";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Name Subchart";
		base.Load += new System.EventHandler(Subchart_name_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void Subchart_name_Load(object sender, EventArgs e)
	{
		label3.Text = "Examples:\n   Draw_Boxes\n   Find_Smallest";
	}

	private bool All_Legal(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if (!char.IsLetterOrDigit(s, i) && s[i] != '_')
			{
				return false;
			}
		}
		return true;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		string text = textBox1.Text.Trim();
		result = "";
		if (text.Length == 0)
		{
			label4.Text = "Can't have blank name";
			return;
		}
		if (!char.IsLetter(text, 0))
		{
			label4.Text = "Name must begin with letter";
			return;
		}
		if (!All_Legal(text))
		{
			label4.Text = "Use only letter, number, or underscore";
			return;
		}
		if (!token_helpers_pkg.verify_id(text))
		{
			label4.Text = text + " is a reserved word";
			return;
		}
		if (form.Is_Subchart_Name(text) && text.ToLower() != init_name)
		{
			label4.Text = text + " is already used";
			return;
		}
		result = text;
		Close();
	}

	private void Control_Text_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			e.Handled = true;
		}
	}

	private void Check_key(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			button1_Click(sender, e);
			e.Handled = true;
		}
	}

	private void button2_Click(object sender, EventArgs e)
	{
		result = "";
		Close();
	}
}
