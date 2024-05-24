using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FilesBrowser;

namespace raptor;

public class BARTPEFileOpenList : Form
{
	public static string filename;

	private IContainer components;

	private Label label1;

	private FilesListBox filesListBox1;

	private CheckBox checkBox1;

	public BARTPEFileOpenList()
	{
		InitializeComponent();
		filename = null;
		if (!Component.BARTPE)
		{
			filesListBox1.SelectedPath = "x:\\";
		}
		else
		{
			filesListBox1.SelectedPath = Component.BARTPE_ramdrive_path;
		}
	}

	private void filesListBox1_FileSelected(object sender, FileSelectEventArgs fse)
	{
		filename = filesListBox1.SelectedFile;
		Close();
	}

	private void checkBox1_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBox1.Checked)
		{
			filesListBox1.Extension = "";
		}
		else
		{
			filesListBox1.Extension = ".rap";
		}
	}

	public void View_HD()
	{
		filesListBox1.Extension = ".aes";
		label1.Text = "";
		filesListBox1.SelectedPath = Component.BARTPE_partition_path;
		checkBox1.Visible = false;
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
		this.label1 = new System.Windows.Forms.Label();
		this.filesListBox1 = new FilesBrowser.FilesListBox();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.label1.Location = new System.Drawing.Point(0, 253);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(121, 13);
		this.label1.TabIndex = 1;
		this.label1.Text = "Double click file to open";
		this.filesListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.filesListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
		this.filesListBox1.Extension = ".rap";
		this.filesListBox1.FileIconSize = FilesBrowser.IconSize.Small;
		this.filesListBox1.FormattingEnabled = true;
		this.filesListBox1.ItemHeight = 16;
		this.filesListBox1.Location = new System.Drawing.Point(0, 0);
		this.filesListBox1.Name = "filesListBox1";
		this.filesListBox1.Size = new System.Drawing.Size(292, 244);
		this.filesListBox1.TabIndex = 2;
		this.filesListBox1.FileSelected += new FilesBrowser.FileSelectedEventHandler(filesListBox1_FileSelected);
		this.checkBox1.AutoSize = true;
		this.checkBox1.Location = new System.Drawing.Point(168, 251);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(113, 17);
		this.checkBox1.TabIndex = 3;
		this.checkBox1.Text = "Show backup files";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox1.CheckedChanged += new System.EventHandler(checkBox1_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(292, 266);
		base.Controls.Add(this.checkBox1);
		base.Controls.Add(this.filesListBox1);
		base.Controls.Add(this.label1);
		base.Name = "BARTPEFileOpenList";
		this.Text = "Open File";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
