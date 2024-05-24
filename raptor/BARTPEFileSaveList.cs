using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FilesBrowser;

namespace raptor;

public class BARTPEFileSaveList : Form
{
	public static string filename;

	private IContainer components;

	private Label label1;

	private Panel panel1;

	private TextBox textBox1;

	private Button buttonCancel;

	private Button buttonOK;

	private FilesListBox filesListBox1;

	public BARTPEFileSaveList()
	{
		InitializeComponent();
		textBox1.Focus();
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
		CheckOverwrite(filesListBox1.SelectedFile);
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		string text = textBox1.Text;
		if (text.StartsWith("x:\\", ignoreCase: true, null) || text.StartsWith("y:\\", ignoreCase: true, null) || text.StartsWith("b:\\", ignoreCase: true, null))
		{
			text = text.Substring(3);
		}
		if (text.EndsWith(".rap", ignoreCase: true, null))
		{
			text = text.Substring(0, text.Length - 4);
		}
		if (text.Length < 1)
		{
			MessageBox.Show("Filename must not be blank!", "Invalid filename", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		for (int i = 0; i < text.Length; i++)
		{
			if (!char.IsLetterOrDigit(text[i]) && text[i] != '_' && text[i] != '-')
			{
				MessageBox.Show("Filename can only contain letters, numbers, dashes and underscores.", "Invalid filename", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
		}
		text = filesListBox1.SelectedPath + text + ".rap";
		if (System.IO.File.Exists(text))
		{
			CheckOverwrite(text);
			return;
		}
		filename = text;
		Close();
	}

	private void CheckOverwrite(string test)
	{
		if (MessageBox.Show(test + " already exists.\nDo you want to overwrite it?", "Overwrite file?", MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			filename = test;
			Close();
		}
	}

	private void textBox1_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			buttonOK_Click(sender, e);
		}
	}

	private void filesListBox1_SelectedValueChanged(object sender, EventArgs e)
	{
		textBox1.Text = Path.GetFileName(filesListBox1.SelectedFile);
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
		this.panel1 = new System.Windows.Forms.Panel();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOK = new System.Windows.Forms.Button();
		this.filesListBox1 = new FilesBrowser.FilesListBox();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.label1.Location = new System.Drawing.Point(0, 253);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(192, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Type filename for saving, then click OK";
		this.panel1.Controls.Add(this.textBox1);
		this.panel1.Controls.Add(this.buttonCancel);
		this.panel1.Controls.Add(this.buttonOK);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 201);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(292, 52);
		this.panel1.TabIndex = 1;
		this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
		this.textBox1.Location = new System.Drawing.Point(0, 0);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(292, 20);
		this.textBox1.TabIndex = 0;
		this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox1_KeyDown);
		this.buttonCancel.Location = new System.Drawing.Point(166, 26);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 4;
		this.buttonCancel.Text = "Cancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		this.buttonOK.Location = new System.Drawing.Point(52, 26);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(75, 23);
		this.buttonOK.TabIndex = 3;
		this.buttonOK.Text = "OK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		this.filesListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.filesListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
		this.filesListBox1.Extension = ".rap";
		this.filesListBox1.FileIconSize = FilesBrowser.IconSize.Small;
		this.filesListBox1.FormattingEnabled = true;
		this.filesListBox1.ItemHeight = 16;
		this.filesListBox1.Location = new System.Drawing.Point(0, 0);
		this.filesListBox1.Name = "filesListBox1";
		this.filesListBox1.Size = new System.Drawing.Size(292, 196);
		this.filesListBox1.TabIndex = 5;
		this.filesListBox1.FileSelected += new FilesBrowser.FileSelectedEventHandler(filesListBox1_FileSelected);
		this.filesListBox1.SelectedValueChanged += new System.EventHandler(filesListBox1_SelectedValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(292, 266);
		base.Controls.Add(this.filesListBox1);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.label1);
		base.Name = "BARTPEFileSaveList";
		this.Text = "Save File As";
		base.TopMost = true;
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
