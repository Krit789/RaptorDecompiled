using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NClass.Core;

namespace raptor;

public class ClassTabPage : TabPage
{
	internal ClassType ct;

	private IContainer components;

	private SplitContainer splitContainer1;

	internal TreeView listBox1;

	internal TabControl tabControl1;

	public ClassTabPage(Visual_Flow_Form form, string name)
	{
		InitializeComponent();
		Text = name;
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
		this.listBox1 = new System.Windows.Forms.TreeView();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		base.SuspendLayout();
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
		this.splitContainer1.Panel1MinSize = 75;
		this.splitContainer1.Panel1.Controls.Add(this.listBox1);
		this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
		this.splitContainer1.Size = new System.Drawing.Size(250, 150);
		this.splitContainer1.TabIndex = 0;
		this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl1.Location = new System.Drawing.Point(0, 0);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(146, 150);
		this.tabControl1.TabIndex = 0;
		this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.listBox1.Location = new System.Drawing.Point(0, 0);
		this.listBox1.Name = "listBox1";
		this.listBox1.Size = new System.Drawing.Size(100, 147);
		this.listBox1.TabIndex = 0;
		base.Controls.Add(this.splitContainer1);
		base.Name = "ClassTabPage";
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		this.splitContainer1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
