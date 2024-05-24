using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace raptor;

public class Procedure_name : Form
{
	private TextBox textBoxProcedureName;

	private Button button1;

	private Label labelInstructions;

	private Label labelExamples;

	private static string result;

	private static string[] param_names;

	private static bool[] param_is_input;

	private static bool[] param_is_output;

	private string init_name;

	private Label labelError;

	private Visual_Flow_Form form;

	private Label labelProcName;

	private Label label1;

	private TextBox textBoxParam1;

	private Label label2;

	private TextBox textBoxParam2;

	private Label label3;

	private TextBox textBoxParam5;

	private Label label4;

	private TextBox textBoxParam4;

	private Label label5;

	private TextBox textBoxParam3;

	private Label label8;

	private TextBox textBoxParam6;

	private CheckBox checkBoxInput1;

	private CheckBox checkBoxOutput1;

	private CheckBox checkBoxOutput2;

	private CheckBox checkBoxInput2;

	private CheckBox checkBoxOutput3;

	private CheckBox checkBoxInput3;

	private CheckBox checkBoxOutput4;

	private CheckBox checkBoxInput4;

	private CheckBox checkBoxOutput5;

	private CheckBox checkBoxInput5;

	private CheckBox checkBoxOutput6;

	private CheckBox checkBoxInput6;

	private Button buttonCancel;

	private Container components;

	public Procedure_name()
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

	public static string RunDialog(string s, ref string[] incoming_param_names, ref bool[] is_input, ref bool[] is_output, Visual_Flow_Form the_form)
	{
		Procedure_name procedure_name = new Procedure_name();
		result = "";
		if (Component.USMA_mode)
		{
			procedure_name.checkBoxInput2.Visible = false;
			procedure_name.checkBoxInput3.Visible = false;
			procedure_name.checkBoxInput4.Visible = false;
			procedure_name.checkBoxInput5.Visible = false;
			procedure_name.checkBoxInput6.Visible = false;
			procedure_name.checkBoxOutput1.Text = "Return";
			procedure_name.checkBoxOutput2.Visible = false;
			procedure_name.checkBoxOutput3.Visible = false;
			procedure_name.checkBoxOutput4.Visible = false;
			procedure_name.checkBoxOutput5.Visible = false;
			procedure_name.checkBoxOutput6.Visible = false;
		}
		procedure_name.textBoxProcedureName.Text = s;
		if (incoming_param_names.Length >= 1)
		{
			procedure_name.textBoxParam1.Text = incoming_param_names[0];
			procedure_name.checkBoxInput1.Checked = is_input[0];
			procedure_name.checkBoxOutput1.Checked = is_output[0];
		}
		if (incoming_param_names.Length >= 2)
		{
			procedure_name.textBoxParam2.Text = incoming_param_names[1];
			procedure_name.checkBoxInput2.Checked = is_input[1];
			procedure_name.checkBoxOutput2.Checked = is_output[1];
		}
		if (incoming_param_names.Length >= 3)
		{
			procedure_name.textBoxParam3.Text = incoming_param_names[2];
			procedure_name.checkBoxInput3.Checked = is_input[2];
			procedure_name.checkBoxOutput3.Checked = is_output[2];
		}
		if (incoming_param_names.Length >= 4)
		{
			procedure_name.textBoxParam4.Text = incoming_param_names[3];
			procedure_name.checkBoxInput4.Checked = is_input[3];
			procedure_name.checkBoxOutput4.Checked = is_output[3];
		}
		if (incoming_param_names.Length >= 5)
		{
			procedure_name.textBoxParam5.Text = incoming_param_names[4];
			procedure_name.checkBoxInput5.Checked = is_input[4];
			procedure_name.checkBoxOutput5.Checked = is_output[4];
		}
		if (incoming_param_names.Length >= 6)
		{
			procedure_name.textBoxParam6.Text = incoming_param_names[5];
			procedure_name.checkBoxInput6.Checked = is_input[5];
			procedure_name.checkBoxOutput6.Checked = is_output[5];
		}
		procedure_name.init_name = s.ToLower();
		procedure_name.form = the_form;
		procedure_name.ShowDialog();
		if (result != null && result != "")
		{
			incoming_param_names = param_names;
			is_input = param_is_input;
			is_output = param_is_output;
			return result;
		}
		return null;
	}

	private void InitializeComponent()
	{
		this.textBoxProcedureName = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.labelInstructions = new System.Windows.Forms.Label();
		this.labelExamples = new System.Windows.Forms.Label();
		this.labelError = new System.Windows.Forms.Label();
		this.labelProcName = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.textBoxParam1 = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.textBoxParam2 = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.textBoxParam5 = new System.Windows.Forms.TextBox();
		this.label4 = new System.Windows.Forms.Label();
		this.textBoxParam4 = new System.Windows.Forms.TextBox();
		this.label5 = new System.Windows.Forms.Label();
		this.textBoxParam3 = new System.Windows.Forms.TextBox();
		this.label8 = new System.Windows.Forms.Label();
		this.textBoxParam6 = new System.Windows.Forms.TextBox();
		this.checkBoxInput1 = new System.Windows.Forms.CheckBox();
		this.checkBoxOutput1 = new System.Windows.Forms.CheckBox();
		this.checkBoxOutput2 = new System.Windows.Forms.CheckBox();
		this.checkBoxInput2 = new System.Windows.Forms.CheckBox();
		this.checkBoxOutput3 = new System.Windows.Forms.CheckBox();
		this.checkBoxInput3 = new System.Windows.Forms.CheckBox();
		this.checkBoxOutput4 = new System.Windows.Forms.CheckBox();
		this.checkBoxInput4 = new System.Windows.Forms.CheckBox();
		this.checkBoxOutput5 = new System.Windows.Forms.CheckBox();
		this.checkBoxInput5 = new System.Windows.Forms.CheckBox();
		this.checkBoxOutput6 = new System.Windows.Forms.CheckBox();
		this.checkBoxInput6 = new System.Windows.Forms.CheckBox();
		this.buttonCancel = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.textBoxProcedureName.AcceptsReturn = true;
		this.textBoxProcedureName.Location = new System.Drawing.Point(22, 172);
		this.textBoxProcedureName.Name = "textBoxProcedureName";
		this.textBoxProcedureName.Size = new System.Drawing.Size(248, 20);
		this.textBoxProcedureName.TabIndex = 0;
		this.textBoxProcedureName.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBoxProcedureName.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.button1.Location = new System.Drawing.Point(43, 458);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 30;
		this.button1.Text = "Ok";
		this.button1.Click += new System.EventHandler(button1_Click);
		this.labelInstructions.Location = new System.Drawing.Point(36, 15);
		this.labelInstructions.Name = "labelInstructions";
		this.labelInstructions.Size = new System.Drawing.Size(224, 40);
		this.labelInstructions.TabIndex = 3;
		this.labelInstructions.Text = "Names must begin with letter, and contain only letters, numbers and underscores.";
		this.labelExamples.Location = new System.Drawing.Point(36, 54);
		this.labelExamples.Name = "labelExamples";
		this.labelExamples.Size = new System.Drawing.Size(224, 57);
		this.labelExamples.TabIndex = 4;
		this.labelExamples.Text = "Examples:";
		this.labelError.Location = new System.Drawing.Point(22, 121);
		this.labelError.Name = "labelError";
		this.labelError.Size = new System.Drawing.Size(248, 32);
		this.labelError.TabIndex = 5;
		this.labelProcName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.labelProcName.Location = new System.Drawing.Point(19, 153);
		this.labelProcName.Name = "labelProcName";
		this.labelProcName.Size = new System.Drawing.Size(224, 16);
		this.labelProcName.TabIndex = 6;
		this.labelProcName.Text = "Procedure Name";
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(19, 194);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(224, 16);
		this.label1.TabIndex = 8;
		this.label1.Text = "Parameter 1 (or blank)";
		this.textBoxParam1.AcceptsReturn = true;
		this.textBoxParam1.Location = new System.Drawing.Point(22, 213);
		this.textBoxParam1.Name = "textBoxParam1";
		this.textBoxParam1.Size = new System.Drawing.Size(248, 20);
		this.textBoxParam1.TabIndex = 3;
		this.textBoxParam1.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBoxParam1.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Location = new System.Drawing.Point(19, 235);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(224, 16);
		this.label2.TabIndex = 10;
		this.label2.Text = "Parameter 2 (or blank)";
		this.textBoxParam2.AcceptsReturn = true;
		this.textBoxParam2.Location = new System.Drawing.Point(22, 254);
		this.textBoxParam2.Name = "textBoxParam2";
		this.textBoxParam2.Size = new System.Drawing.Size(248, 20);
		this.textBoxParam2.TabIndex = 6;
		this.textBoxParam2.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBoxParam2.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.Location = new System.Drawing.Point(19, 358);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(224, 16);
		this.label3.TabIndex = 16;
		this.label3.Text = "Parameter 5 (or blank)";
		this.textBoxParam5.AcceptsReturn = true;
		this.textBoxParam5.Location = new System.Drawing.Point(22, 377);
		this.textBoxParam5.Name = "textBoxParam5";
		this.textBoxParam5.Size = new System.Drawing.Size(248, 20);
		this.textBoxParam5.TabIndex = 23;
		this.textBoxParam5.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBoxParam5.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.Location = new System.Drawing.Point(19, 317);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(224, 16);
		this.label4.TabIndex = 14;
		this.label4.Text = "Parameter 4 (or blank)";
		this.textBoxParam4.AcceptsReturn = true;
		this.textBoxParam4.Location = new System.Drawing.Point(22, 336);
		this.textBoxParam4.Name = "textBoxParam4";
		this.textBoxParam4.Size = new System.Drawing.Size(248, 20);
		this.textBoxParam4.TabIndex = 17;
		this.textBoxParam4.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBoxParam4.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.Location = new System.Drawing.Point(19, 276);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(224, 16);
		this.label5.TabIndex = 12;
		this.label5.Text = "Parameter 3 (or blank)";
		this.textBoxParam3.AcceptsReturn = true;
		this.textBoxParam3.Location = new System.Drawing.Point(22, 295);
		this.textBoxParam3.Name = "textBoxParam3";
		this.textBoxParam3.Size = new System.Drawing.Size(248, 20);
		this.textBoxParam3.TabIndex = 9;
		this.textBoxParam3.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBoxParam3.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label8.Location = new System.Drawing.Point(19, 399);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(224, 16);
		this.label8.TabIndex = 18;
		this.label8.Text = "Parameter 6 (or blank)";
		this.textBoxParam6.AcceptsReturn = true;
		this.textBoxParam6.Location = new System.Drawing.Point(22, 418);
		this.textBoxParam6.Name = "textBoxParam6";
		this.textBoxParam6.Size = new System.Drawing.Size(248, 20);
		this.textBoxParam6.TabIndex = 28;
		this.textBoxParam6.KeyDown += new System.Windows.Forms.KeyEventHandler(Check_key);
		this.textBoxParam6.KeyUp += new System.Windows.Forms.KeyEventHandler(Control_Text_KeyUp);
		this.checkBoxInput1.AutoSize = true;
		this.checkBoxInput1.Checked = true;
		this.checkBoxInput1.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxInput1.Location = new System.Drawing.Point(139, 194);
		this.checkBoxInput1.Name = "checkBoxInput1";
		this.checkBoxInput1.Size = new System.Drawing.Size(50, 17);
		this.checkBoxInput1.TabIndex = 1;
		this.checkBoxInput1.Text = "Input";
		this.checkBoxInput1.UseVisualStyleBackColor = true;
		this.checkBoxOutput1.AutoSize = true;
		this.checkBoxOutput1.Location = new System.Drawing.Point(196, 194);
		this.checkBoxOutput1.Name = "checkBoxOutput1";
		this.checkBoxOutput1.Size = new System.Drawing.Size(58, 17);
		this.checkBoxOutput1.TabIndex = 2;
		this.checkBoxOutput1.Text = "Output";
		this.checkBoxOutput1.UseVisualStyleBackColor = true;
		this.checkBoxOutput2.AutoSize = true;
		this.checkBoxOutput2.Location = new System.Drawing.Point(195, 235);
		this.checkBoxOutput2.Name = "checkBoxOutput2";
		this.checkBoxOutput2.Size = new System.Drawing.Size(58, 17);
		this.checkBoxOutput2.TabIndex = 5;
		this.checkBoxOutput2.Text = "Output";
		this.checkBoxOutput2.UseVisualStyleBackColor = true;
		this.checkBoxInput2.AutoSize = true;
		this.checkBoxInput2.Checked = true;
		this.checkBoxInput2.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxInput2.Location = new System.Drawing.Point(138, 235);
		this.checkBoxInput2.Name = "checkBoxInput2";
		this.checkBoxInput2.Size = new System.Drawing.Size(50, 17);
		this.checkBoxInput2.TabIndex = 4;
		this.checkBoxInput2.Text = "Input";
		this.checkBoxInput2.UseVisualStyleBackColor = true;
		this.checkBoxOutput3.AutoSize = true;
		this.checkBoxOutput3.Location = new System.Drawing.Point(195, 276);
		this.checkBoxOutput3.Name = "checkBoxOutput3";
		this.checkBoxOutput3.Size = new System.Drawing.Size(58, 17);
		this.checkBoxOutput3.TabIndex = 8;
		this.checkBoxOutput3.Text = "Output";
		this.checkBoxOutput3.UseVisualStyleBackColor = true;
		this.checkBoxInput3.AutoSize = true;
		this.checkBoxInput3.Checked = true;
		this.checkBoxInput3.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxInput3.Location = new System.Drawing.Point(138, 276);
		this.checkBoxInput3.Name = "checkBoxInput3";
		this.checkBoxInput3.Size = new System.Drawing.Size(50, 17);
		this.checkBoxInput3.TabIndex = 7;
		this.checkBoxInput3.Text = "Input";
		this.checkBoxInput3.UseVisualStyleBackColor = true;
		this.checkBoxOutput4.AutoSize = true;
		this.checkBoxOutput4.Location = new System.Drawing.Point(195, 317);
		this.checkBoxOutput4.Name = "checkBoxOutput4";
		this.checkBoxOutput4.Size = new System.Drawing.Size(58, 17);
		this.checkBoxOutput4.TabIndex = 15;
		this.checkBoxOutput4.Text = "Output";
		this.checkBoxOutput4.UseVisualStyleBackColor = true;
		this.checkBoxInput4.AutoSize = true;
		this.checkBoxInput4.Checked = true;
		this.checkBoxInput4.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxInput4.Location = new System.Drawing.Point(138, 317);
		this.checkBoxInput4.Name = "checkBoxInput4";
		this.checkBoxInput4.Size = new System.Drawing.Size(50, 17);
		this.checkBoxInput4.TabIndex = 10;
		this.checkBoxInput4.Text = "Input";
		this.checkBoxInput4.UseVisualStyleBackColor = true;
		this.checkBoxOutput5.AutoSize = true;
		this.checkBoxOutput5.Location = new System.Drawing.Point(195, 358);
		this.checkBoxOutput5.Name = "checkBoxOutput5";
		this.checkBoxOutput5.Size = new System.Drawing.Size(58, 17);
		this.checkBoxOutput5.TabIndex = 21;
		this.checkBoxOutput5.Text = "Output";
		this.checkBoxOutput5.UseVisualStyleBackColor = true;
		this.checkBoxInput5.AutoSize = true;
		this.checkBoxInput5.Checked = true;
		this.checkBoxInput5.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxInput5.Location = new System.Drawing.Point(138, 358);
		this.checkBoxInput5.Name = "checkBoxInput5";
		this.checkBoxInput5.Size = new System.Drawing.Size(50, 17);
		this.checkBoxInput5.TabIndex = 19;
		this.checkBoxInput5.Text = "Input";
		this.checkBoxInput5.UseVisualStyleBackColor = true;
		this.checkBoxOutput6.AutoSize = true;
		this.checkBoxOutput6.Location = new System.Drawing.Point(195, 399);
		this.checkBoxOutput6.Name = "checkBoxOutput6";
		this.checkBoxOutput6.Size = new System.Drawing.Size(58, 17);
		this.checkBoxOutput6.TabIndex = 26;
		this.checkBoxOutput6.Text = "Output";
		this.checkBoxOutput6.UseVisualStyleBackColor = true;
		this.checkBoxInput6.AutoSize = true;
		this.checkBoxInput6.Checked = true;
		this.checkBoxInput6.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxInput6.Location = new System.Drawing.Point(138, 399);
		this.checkBoxInput6.Name = "checkBoxInput6";
		this.checkBoxInput6.Size = new System.Drawing.Size(50, 17);
		this.checkBoxInput6.TabIndex = 24;
		this.checkBoxInput6.Text = "Input";
		this.checkBoxInput6.UseVisualStyleBackColor = true;
		this.buttonCancel.Location = new System.Drawing.Point(172, 458);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 31;
		this.buttonCancel.Text = "Cancel";
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(291, 493);
		base.ControlBox = false;
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.checkBoxOutput6);
		base.Controls.Add(this.checkBoxInput6);
		base.Controls.Add(this.checkBoxOutput5);
		base.Controls.Add(this.checkBoxInput5);
		base.Controls.Add(this.checkBoxOutput4);
		base.Controls.Add(this.checkBoxInput4);
		base.Controls.Add(this.checkBoxOutput3);
		base.Controls.Add(this.checkBoxInput3);
		base.Controls.Add(this.checkBoxOutput2);
		base.Controls.Add(this.checkBoxInput2);
		base.Controls.Add(this.checkBoxOutput1);
		base.Controls.Add(this.checkBoxInput1);
		base.Controls.Add(this.label8);
		base.Controls.Add(this.textBoxParam6);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.textBoxParam5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.textBoxParam4);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.textBoxParam3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.textBoxParam2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.textBoxParam1);
		base.Controls.Add(this.labelProcName);
		base.Controls.Add(this.labelError);
		base.Controls.Add(this.labelExamples);
		base.Controls.Add(this.labelInstructions);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.textBoxProcedureName);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Procedure_name";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Create Procedure";
		base.Load += new System.EventHandler(Subchart_name_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void Subchart_name_Load(object sender, EventArgs e)
	{
		labelExamples.Text = "Examples:\n   Draw_Boxes\n   Find_Smallest";
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

	private bool Check_Name(string result, string error_prefix)
	{
		if (!char.IsLetter(result, 0))
		{
			labelError.Text = error_prefix + "must begin with letter";
			return true;
		}
		if (!All_Legal(result))
		{
			labelError.Text = error_prefix + "must have only letters, numbers, or underscores";
			return true;
		}
		if (!token_helpers_pkg.verify_id(result))
		{
			labelError.Text = result + " is a reserved word";
			return true;
		}
		return false;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		int num = 0;
		string text = textBoxProcedureName.Text.Trim();
		result = "";
		bool flag;
		if (text.Length == 0)
		{
			labelError.Text = "Procedure name must not be blank";
			flag = true;
			return;
		}
		if (form.Is_Subchart_Name(text) && text.ToLower() != init_name)
		{
			labelError.Text = text + " is already used";
			flag = true;
			return;
		}
		string[] array = new string[6]
		{
			textBoxParam1.Text.Trim(),
			textBoxParam2.Text.Trim(),
			textBoxParam3.Text.Trim(),
			textBoxParam4.Text.Trim(),
			textBoxParam5.Text.Trim(),
			textBoxParam6.Text.Trim()
		};
		bool[] array2 = new bool[6] { checkBoxInput1.Checked, checkBoxInput2.Checked, checkBoxInput3.Checked, checkBoxInput4.Checked, checkBoxInput5.Checked, checkBoxInput6.Checked };
		bool[] array3 = new bool[6] { checkBoxOutput1.Checked, checkBoxOutput2.Checked, checkBoxOutput3.Checked, checkBoxOutput4.Checked, checkBoxOutput5.Checked, checkBoxOutput6.Checked };
		flag = Check_Name(text, "Procedure name ");
		if (flag)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "")
			{
				num++;
				flag = Check_Name(array[i], "Parameter " + (i + 1) + " ");
				for (int j = i + 1; j < array.Length; j++)
				{
					if (array[j].ToLower() == array[i].ToLower())
					{
						labelError.Text = "Can not have two parameters (" + (i + 1) + ") and (" + (j + 1) + ") named the same.";
						flag = true;
					}
				}
			}
			else
			{
				for (int k = i + 1; k < array.Length; k++)
				{
					if (array[k] != "")
					{
						labelError.Text = "Can not have blank parameter (" + (i + 1) + ") before non-blank (" + (k + 1) + ")";
						flag = true;
					}
				}
			}
			if (flag)
			{
				return;
			}
		}
		if (!flag)
		{
			param_names = new string[num];
			param_is_input = new bool[num];
			param_is_output = new bool[num];
			for (int l = 0; l < num; l++)
			{
				param_names[l] = array[l];
				param_is_input[l] = array2[l];
				param_is_output[l] = array3[l];
			}
			result = text;
			Close();
		}
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

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		result = "";
		Close();
	}
}
