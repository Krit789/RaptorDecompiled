using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using parse_tree.emit_code;

namespace raptor;

public class SubmitServerForm : Form
{
	private Label label1;

	private Label label2;

	private Label label3;

	private Button refreshButton;

	private TextBox textBoxServer;

	private TextBox textBoxPort;

	private ListBox assignmentsList;

	private Container components;

	private Button OKbutton;

	private Button cancelButton;

	public static Process process;

	public static string submit_filename;

	private static void Connect_To_Server(string server, int port)
	{
		IPAddress iPAddress;
		try
		{
			iPAddress = IPAddress.Parse(server);
		}
		catch
		{
			iPAddress = Dns.GetHostEntry(server).AddressList[0];
		}
		raptor_files_pkg.network_redirect(iPAddress.ToString(), port, use_tcp: true);
	}

	private static void StopProcessRedirect()
	{
		if (process != null && !process.HasExited)
		{
			process.Kill();
		}
		raptor_files_pkg.stop_process_redirect();
	}

	private static void Connect_To_Executable()
	{
		StopProcessRedirect();
		process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.CreateNoWindow = true;
		if (Directory.Exists("r:\\testserver"))
		{
			process.StartInfo.FileName = "r:\\testserver\\testserver.exe";
			process.StartInfo.WorkingDirectory = "r:\\testserver";
		}
		else if (Directory.Exists("x:\\programs\\testserver"))
		{
			process.StartInfo.FileName = "x:\\programs\\testserver\\testserver.exe";
			process.StartInfo.WorkingDirectory = "x:\\programs\\testserver";
		}
		else if (Directory.Exists("y:\\programs\\testserver"))
		{
			process.StartInfo.FileName = "y:\\programs\\testserver\\testserver.exe";
			process.StartInfo.WorkingDirectory = "y:\\programs\\testserver";
		}
		else if (Directory.Exists("b:\\programs\\testserver"))
		{
			process.StartInfo.FileName = "b:\\programs\\testserver\\testserver.exe";
			process.StartInfo.WorkingDirectory = "b:\\programs\\testserver";
		}
		else if (Directory.Exists("c:\\program files\\testserver"))
		{
			process.StartInfo.FileName = "c:\\program files\\testserver\\testserver.exe";
			process.StartInfo.WorkingDirectory = "c:\\program files\\testserver";
		}
		process.Start();
		raptor_files_pkg.process_redirect(process);
	}

	private void Refresh_List_Executable()
	{
		process.StandardInput.WriteLine("DIRECTORY");
		while (!process.StandardOutput.EndOfStream)
		{
			string text = process.StandardOutput.ReadLine();
			if (text != "" && text != "EOF")
			{
				assignmentsList.Items.Add(text);
			}
		}
		if (assignmentsList.Items.Count == 0)
		{
			assignmentsList.Items.Add("server unavailable");
		}
	}

	private void Refresh_List()
	{
		int num = 0;
		assignmentsList.Items.Clear();
		if (Component.BARTPE)
		{
			Connect_To_Executable();
			Refresh_List_Executable();
			return;
		}
		try
		{
			try
			{
				Connect_To_Server(textBoxServer.Text, int.Parse(textBoxPort.Text));
			}
			catch
			{
				assignmentsList.Items.Add("Unable to connect");
				throw new Exception();
			}
			try
			{
				string s = "DIRECTORY\r\n";
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				raptor_files_pkg.current_socket.Send(bytes);
			}
			catch
			{
				assignmentsList.Items.Add("Server terminated abnormally");
				throw new Exception();
			}
			try
			{
				while (true)
				{
					string line = raptor_files_pkg.get_line();
					assignmentsList.Items.Add(line);
					num++;
				}
			}
			catch (Exception)
			{
				if (assignmentsList.Items.Count == 0)
				{
					assignmentsList.Items.Add("No assignments returned");
					throw new Exception();
				}
			}
			try
			{
				StopRedirection();
			}
			catch
			{
			}
		}
		catch
		{
			try
			{
				StopRedirection();
			}
			catch
			{
			}
		}
		if (assignmentsList.Items.Count == 0)
		{
			assignmentsList.Items.Add("server unavailable");
		}
	}

	public SubmitServerForm()
	{
		InitializeComponent();
		if (Component.BARTPE)
		{
			textBoxServer.Text = "localhost";
			textBoxPort.Enabled = false;
		}
		string text = Registry_Settings.Read("test_server");
		string text2 = Registry_Settings.Read("test_port");
		if (text != null)
		{
			textBoxServer.Text = text;
		}
		if (text2 != null)
		{
			textBoxPort.Text = text2;
		}
		Refresh_List();
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
		this.textBoxServer = new System.Windows.Forms.TextBox();
		this.textBoxPort = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.cancelButton = new System.Windows.Forms.Button();
		this.refreshButton = new System.Windows.Forms.Button();
		this.assignmentsList = new System.Windows.Forms.ListBox();
		this.OKbutton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.textBoxServer.Location = new System.Drawing.Point(64, 16);
		this.textBoxServer.Name = "textBoxServer";
		this.textBoxServer.Size = new System.Drawing.Size(144, 20);
		this.textBoxServer.TabIndex = 2;
		this.textBoxServer.Text = "dfcs.usafa.edu";
		this.textBoxPort.Location = new System.Drawing.Point(64, 40);
		this.textBoxPort.MaxLength = 6;
		this.textBoxPort.Name = "textBoxPort";
		this.textBoxPort.Size = new System.Drawing.Size(144, 20);
		this.textBoxPort.TabIndex = 3;
		this.textBoxPort.Text = "4242";
		this.label1.Location = new System.Drawing.Point(16, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(40, 16);
		this.label1.TabIndex = 2;
		this.label1.Text = "Server";
		this.label2.Location = new System.Drawing.Point(16, 40);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(32, 16);
		this.label2.TabIndex = 3;
		this.label2.Text = "Port";
		this.label3.Location = new System.Drawing.Point(24, 80);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(136, 16);
		this.label3.TabIndex = 5;
		this.label3.Text = "Assignments Available";
		this.cancelButton.Location = new System.Drawing.Point(152, 264);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(88, 32);
		this.cancelButton.TabIndex = 4;
		this.cancelButton.Text = "Cancel";
		this.cancelButton.Click += new System.EventHandler(cancel_Click);
		this.refreshButton.Location = new System.Drawing.Point(216, 24);
		this.refreshButton.Name = "refreshButton";
		this.refreshButton.Size = new System.Drawing.Size(64, 32);
		this.refreshButton.TabIndex = 1;
		this.refreshButton.Text = "Check";
		this.refreshButton.Click += new System.EventHandler(refreshButton_Click);
		this.assignmentsList.HorizontalScrollbar = true;
		this.assignmentsList.Location = new System.Drawing.Point(32, 104);
		this.assignmentsList.Name = "assignmentsList";
		this.assignmentsList.Size = new System.Drawing.Size(208, 134);
		this.assignmentsList.Sorted = true;
		this.assignmentsList.TabIndex = 6;
		this.OKbutton.Location = new System.Drawing.Point(56, 264);
		this.OKbutton.Name = "OKbutton";
		this.OKbutton.Size = new System.Drawing.Size(64, 32);
		this.OKbutton.TabIndex = 7;
		this.OKbutton.Text = "Ok";
		this.OKbutton.Click += new System.EventHandler(OKbutton_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(292, 318);
		base.Controls.Add(this.OKbutton);
		base.Controls.Add(this.assignmentsList);
		base.Controls.Add(this.refreshButton);
		base.Controls.Add(this.cancelButton);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.textBoxPort);
		base.Controls.Add(this.textBoxServer);
		base.Name = "SubmitServerForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Server Status";
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private static void submitNoThread(string filename)
	{
		submit_filename = filename;
		submitHelper();
	}

	public static void submit(string filename)
	{
		submit_filename = filename;
		try
		{
			if (Compile_Helpers.run_compiled_thread != null && Compile_Helpers.run_compiled_thread.ThreadState == System.Threading.ThreadState.Running)
			{
				Compile_Helpers.run_compiled_thread.Abort();
			}
		}
		catch
		{
		}
		Runtime.consoleWriteln("Testing file: " + filename + ": please wait!");
		Compile_Helpers.run_compiled_thread = new Thread(submitHelper);
		Compile_Helpers.run_compiled_thread.Start();
	}

	public static void submitHelper()
	{
		string text = submit_filename;
		_ = Component.compiled_flowchart;
		int num = 0;
		bool flag = true;
		if (text.ToLower() == "directory")
		{
			MessageBox.Show("No tests found for: " + text + "\n\nUse Save As to change filename.\nView available tests using Select Server (Run menu).", "Check filename", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		try
		{
			try
			{
				string server = Registry_Settings.Read("test_server");
				string s = Registry_Settings.Read("test_port");
				if (!Component.BARTPE)
				{
					Connect_To_Server(server, int.Parse(s));
				}
				else
				{
					Connect_To_Executable();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Flowchart terminated abnormally\n" + ex.ToString());
			}
			try
			{
				string s2 = text + "\r\n" + Runtime.parent.log.GetMachineName() + "\r\n" + Runtime.parent.log.Last_Username() + "\r\n" + Runtime.parent.log.All_Authors() + "\r\n" + Runtime.parent.log.Total_Minutes() + "\r\n" + Runtime.parent.file_guid.ToString() + "\r\n";
				byte[] bytes = Encoding.ASCII.GetBytes(s2);
				if (!Component.BARTPE)
				{
					raptor_files_pkg.current_socket.Send(bytes);
				}
				else
				{
					process.StandardInput.WriteLine(text);
					process.StandardInput.Flush();
				}
			}
			catch
			{
				MessageBox.Show("Server terminated abnormally.", "Server failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				throw new Exception();
			}
			try
			{
				if (Component.BARTPE && !process.StandardOutput.EndOfStream)
				{
					string s3 = process.StandardOutput.ReadLine();
					num = int.Parse(s3);
				}
				else
				{
					string s3 = raptor_files_pkg.get_line();
					num = int.Parse(s3);
				}
				if (num == 0)
				{
					throw new Exception();
				}
			}
			catch
			{
				MessageBox.Show("No tests found for: " + text + "\n\nUse Save As to change filename.\nView available tests using Select Server (Run menu).", "Check filename", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				throw new Exception();
			}
			Runtime.consoleClear();
			Component.run_compiled_flowchart = true;
			try
			{
				Compile_Helpers.Compile_Flowchart(Runtime.parent.carlisle.TabPages);
			}
			catch (illegal_code)
			{
				MessageBox.Show("Illegal flowchart: no graphics allowed when testing against server.");
				throw;
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Check for blank symbols!\n" + ex2.Message + "\n", "Compilation Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				throw;
			}
			for (int i = 1; i <= num; i++)
			{
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				try
				{
					Compile_Helpers.Run_Compiled_NoThread(was_from_commandline: false);
				}
				catch
				{
					MessageBox.Show("Program terminated abnormally", "Program error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					throw new Exception();
				}
				Thread.CurrentThread.Priority = priority;
				try
				{
					string s4 = "\r\nEOF\r\n";
					byte[] bytes2 = Encoding.ASCII.GetBytes(s4);
					if (!Component.BARTPE)
					{
						raptor_files_pkg.current_socket.Send(bytes2);
					}
					else
					{
						process.StandardInput.WriteLine();
						process.StandardInput.WriteLine("EOF");
						process.StandardInput.Flush();
					}
				}
				catch
				{
					MessageBox.Show("Server terminated abnormally", "Network error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					throw new Exception();
				}
				try
				{
					while (true)
					{
						string s3 = raptor_files_pkg.get_line();
					}
				}
				catch
				{
				}
				try
				{
					string s3 = raptor_files_pkg.get_line();
					if (s3.StartsWith("INCORRECT"))
					{
						flag = false;
					}
					Runtime.consoleMessage(text + ": test case #" + i + ": " + s3);
				}
				catch
				{
					Runtime.consoleMessage("no response");
					flag = false;
				}
			}
			StopRedirection();
			if (flag)
			{
				Runtime.consoleMessage(text + ": PASSED");
			}
			else
			{
				Runtime.consoleMessage(text + ": FAILED");
			}
			Component.run_compiled_flowchart = false;
		}
		catch (Exception)
		{
			try
			{
				StopRedirection();
			}
			catch
			{
			}
			Component.run_compiled_flowchart = false;
		}
	}

	private static void StopRedirection()
	{
		if (!Component.BARTPE)
		{
			raptor_files_pkg.stop_network_redirect();
		}
		else
		{
			StopProcessRedirect();
		}
	}

	private void refreshButton_Click(object sender, EventArgs e)
	{
		Refresh_List();
	}

	private void OKbutton_Click(object sender, EventArgs e)
	{
		Registry_Settings.Write("test_server", textBoxServer.Text);
		try
		{
			int.Parse(textBoxPort.Text);
			Registry_Settings.Write("test_port", textBoxPort.Text);
		}
		catch
		{
		}
		Close();
	}

	private void cancel_Click(object sender, EventArgs e)
	{
		Close();
	}
}
