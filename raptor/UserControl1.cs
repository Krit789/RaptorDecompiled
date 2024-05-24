using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace raptor;

public class UserControl1 : UserControl
{
	private IContainer components;

	private SplitContainer splitContainer1;

	private ListBox listBox1;

	private TabControl tabControl1;

	public UserControl1()
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

	private void InitializeComponent()
	{
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.listBox1 = new System.Windows.Forms.ListBox();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		base.SuspendLayout();
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Panel1.Controls.Add(this.listBox1);
		this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
		this.splitContainer1.Size = new System.Drawing.Size(150, 150);
		this.splitContainer1.TabIndex = 0;
		this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl1.Location = new System.Drawing.Point(0, 0);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(96, 150);
		this.tabControl1.TabIndex = 0;
		this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.listBox1.FormattingEnabled = true;
		this.listBox1.Location = new System.Drawing.Point(0, 0);
		this.listBox1.Name = "listBox1";
		this.listBox1.Size = new System.Drawing.Size(50, 147);
		this.listBox1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.splitContainer1);
		base.Name = "UserControl1";
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		this.splitContainer1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
