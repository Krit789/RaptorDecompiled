using System.Collections.Generic;
using System.Windows.Forms;
using NClass.Core;

namespace raptor;

public class UMLupdater : RAPTORUpdater
{
	private Visual_Flow_Form form;

	public UMLupdater(Visual_Flow_Form form)
	{
		this.form = form;
	}

	public void resetAttributes(object theClass, IEnumerable<Field> fields)
	{
		ClassTabPage classTabPage = theClass as ClassTabPage;
		classTabPage.listBox1.Nodes.Clear();
		foreach (Field field in fields)
		{
			string text = "";
			if (field.IsReadonly || field.IsConstant)
			{
				text += "(constant)";
			}
			if (field.IsStatic)
			{
				text += "(static)";
			}
			classTabPage.listBox1.Nodes.Add(field.GetCaption() + text);
		}
		form.modified = true;
	}

	public object createClass(string name, ClassType ct)
	{
		form.Clear_Undo();
		ClassTabPage classTabPage = new ClassTabPage(form, name);
		classTabPage.ct = ct;
		form.carlisle.TabPages.Add(classTabPage);
		form.modified = true;
		return classTabPage;
	}

	public void deleteClass(object theClass)
	{
		form.Clear_Undo();
		form.carlisle.TabPages.Remove(theClass as ClassTabPage);
		form.modified = true;
	}

	public void renameClass(object theClass, string name)
	{
		(theClass as ClassTabPage).Text = name;
		form.modified = true;
	}

	public object createMethod(object theClass, string name, Method method)
	{
		form.Clear_Undo();
		Procedure_Chart procedure_Chart = new Procedure_Chart(form, name, 0);
		procedure_Chart.method = method;
		(theClass as ClassTabPage).tabControl1.TabPages.Add(procedure_Chart);
		form.modified = true;
		return procedure_Chart;
	}

	public bool makeAbstract(object theClass, object subchart)
	{
		if ((subchart as Procedure_Chart).Start.Count_Symbols() > 2 && MessageBox.Show("This will delete the code in " + (subchart as Procedure_Chart).Text + ".\nDo you want to continue?", "Delete method?", MessageBoxButtons.YesNo) == DialogResult.No)
		{
			return false;
		}
		deleteMethod(theClass, subchart);
		form.modified = true;
		return true;
	}

	public void changeParameters(object theClass, object subchart, int num_params, string[] param_names, bool[] param_is_input, bool[] param_is_output)
	{
		((subchart as Procedure_Chart).Start as Oval_Procedure).changeParameters(num_params, param_names, param_is_input, param_is_output);
		form.modified = true;
	}

	public void deleteMethod(object theClass, object subchart)
	{
		form.Clear_Undo();
		form.modified = true;
		(theClass as ClassTabPage).tabControl1.TabPages.Remove(subchart as Procedure_Chart);
	}

	public void renameMethod(object theClass, object subchart, string name)
	{
		(subchart as Procedure_Chart).Text = name;
		form.modified = true;
	}

	public void reorderMethods(object theClass, IEnumerable<Operation> operations)
	{
		(theClass as ClassTabPage).tabControl1.TabPages.Clear();
		foreach (Operation operation in operations)
		{
			if (operation is Method)
			{
				(theClass as ClassTabPage).tabControl1.TabPages.Add((operation as Method).raptorTab as Procedure_Chart);
			}
		}
		form.modified = true;
	}
}
