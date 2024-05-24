using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace raptor;

public class EmergencyDialog : Form
{
	private static int challenge;

	private BigInteger bi_m;

	private BigInteger bi_n;

	private BigInteger bi_e;

	private BigInteger bi_r;

	private BigInteger bi_check;

	private Random random = new Random();

	private IContainer components;

	private Button button1;

	private TextBox textBox1;

	private Label label1;

	private Label label2;

	private Label label3;

	public EmergencyDialog()
	{
		InitializeComponent();
		challenge = random.Next(99999);
		label3.Text = challenge.ToString() ?? "";
		textBox1.Focus();
	}

	private void button1_Click(object sender, EventArgs e)
	{
		try
		{
			bi_e = new BigInteger(65537L);
			bi_m = new BigInteger(challenge);
			bi_n = new BigInteger("5239739256519985939", 10);
			bi_r = new BigInteger(textBox1.Text, 10);
			bi_check = bi_r.modPow(bi_e, bi_n);
			if (bi_check.Equals(bi_m))
			{
				Process process = new Process();
				try
				{
					Registry.LocalMachine.OpenSubKey("System").OpenSubKey("CurrentControlSet").OpenSubKey("Control")
						.OpenSubKey("StorageDevicePolicies", writable: true)
						.SetValue("WriteProtect", 0, RegistryValueKind.DWord);
				}
				catch (Exception)
				{
				}
				if (System.IO.File.Exists("c:\\windows\\system32\\cmd.exe"))
				{
					process.StartInfo.FileName = "c:\\windows\\system32\\cmd.exe";
				}
				else if (System.IO.File.Exists("x:\\minint\\system32\\cmd.exe"))
				{
					process.StartInfo.FileName = "x:\\minint\\system32\\cmd.exe";
				}
				else if (System.IO.File.Exists("x:\\windows\\system32\\cmd.exe"))
				{
					process.StartInfo.FileName = "x:\\windows\\system32\\cmd.exe";
				}
				else
				{
					process.StartInfo.FileName = "x:\\i386\\system32\\cmd.exe";
				}
				process.StartInfo.ErrorDialog = false;
				process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
				process.Start();
				Close();
			}
			else
			{
				challenge = random.Next(9999999);
				label3.Text = challenge.ToString() ?? "";
			}
		}
		catch
		{
			MessageBox.Show("exception");
			Close();
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
		this.button1 = new System.Windows.Forms.Button();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.button1.Location = new System.Drawing.Point(162, 83);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 1;
		this.button1.Text = "Respond";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.textBox1.Location = new System.Drawing.Point(30, 56);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(339, 20);
		this.textBox1.TabIndex = 0;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(32, 12);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(57, 13);
		this.label1.TabIndex = 2;
		this.label1.Text = "Challenge:";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(32, 33);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(58, 13);
		this.label2.TabIndex = 3;
		this.label2.Text = "Response:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(95, 12);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(37, 13);
		this.label3.TabIndex = 4;
		this.label3.Text = "45677";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(395, 125);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.button1);
		base.Name = "EmergencyDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "EmergencyDialog";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
