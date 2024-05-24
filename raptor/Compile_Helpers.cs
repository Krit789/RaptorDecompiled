using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using generate_interface;
using GeneratorAda;
using NClass.Core;

namespace raptor;

public class Compile_Helpers
{
	private static Hashtable declarations = new Hashtable();

	private static TabControl.TabPageCollection _tpc;

	private static bool from_commandline;

	public static Thread run_compiled_thread;

	public static bool Start_New_Declaration(string name)
	{
		if (name.ToLower().Equals("this") || name.ToLower().Equals("super"))
		{
			return true;
		}
		bool num = declarations.ContainsKey(name.ToLower());
		if (!num)
		{
			declarations.Add(name.ToLower(), null);
		}
		return num;
	}

	public static void Do_Compilation(Oval start, typ gil, TabControl.TabPageCollection tpc)
	{
		if (Component.Current_Mode != Mode.Expert)
		{
			Do_Compilation_Imperative(start, gil as Imperative_Interface, tpc);
			return;
		}
		try
		{
			Do_Compilation_OO(start, gil as OO_Interface, tpc);
		}
		catch
		{
			(gil as OO_Interface).abort();
			throw;
		}
	}

	private static void Do_Compilation_OO(Oval start, OO_Interface gil, TabControl.TabPageCollection tpc)
	{
		_tpc = tpc;
		foreach (IEntity entity in Runtime.parent.projectCore.Entities)
		{
			if (!(entity is InterfaceType))
			{
				continue;
			}
			gil.start_interface(entity as InterfaceType);
			foreach (Operation operation in (entity as InterfaceType).Operations)
			{
				if (operation is Method)
				{
					gil.declare_interface_method(operation as Method);
				}
			}
			gil.done_interface(entity as InterfaceType);
		}
		foreach (ClassTabPage item in allClasses(tpc))
		{
			gil.declare_class(item.ct);
			foreach (Procedure_Chart item2 in allMethods(item))
			{
				Method method = item2.method;
				gil.declare_method(method);
			}
		}
		foreach (ClassTabPage item3 in allClasses(tpc))
		{
			ClassType ct = item3.ct;
			gil.start_class(ct);
			foreach (Field field in ct.Fields)
			{
				gil.declare_field(field);
			}
			foreach (Operation operation2 in ct.Operations)
			{
				if (operation2 is Method && operation2.IsAbstract)
				{
					gil.declare_abstract_method(operation2 as Method);
				}
			}
			foreach (Procedure_Chart item4 in allMethods(item3))
			{
				Method method2 = item4.method;
				gil.start_method(method2);
				declarations.Clear();
				item4.Start.compile_pass1(gil);
				gil.Done_Variable_Declarations();
				item4.Start.Emit_Code(gil);
				gil.Done_Method();
			}
			gil.done_class(item3.ct);
		}
		gil.Start_Method("Main");
		declarations.Clear();
		start.compile_pass1(gil);
		gil.Done_Variable_Declarations();
		start.Emit_Code(gil);
		gil.Done_Method();
		gil.Finish();
	}

	private static void Do_Compilation_Imperative(Oval start, Imperative_Interface gil, TabControl.TabPageCollection tpc)
	{
		_tpc = tpc;
		for (int i = 1; i < tpc.Count; i++)
		{
			if (tpc[i] is Procedure_Chart)
			{
				Procedure_Chart procedure_Chart = tpc[i] as Procedure_Chart;
				gil.Declare_Procedure(procedure_Chart.Text.Trim(), procedure_Chart.getArgs(), procedure_Chart.getArgIsInput(), procedure_Chart.getArgIsOutput());
			}
		}
		for (int j = 1; j < tpc.Count; j++)
		{
			if (tpc[j] is Procedure_Chart)
			{
				Procedure_Chart procedure_Chart2 = tpc[j] as Procedure_Chart;
				gil.Start_Method(procedure_Chart2.Text);
				declarations.Clear();
				procedure_Chart2.Start.compile_pass1(gil);
				gil.Done_Variable_Declarations();
				procedure_Chart2.Start.Emit_Code(gil);
				gil.Done_Method();
			}
		}
		gil.Start_Method("Main");
		declarations.Clear();
		start.compile_pass1(gil);
		gil.Done_Variable_Declarations();
		start.Emit_Code(gil);
		gil.Done_Method();
		gil.Finish();
	}

	public static TabControl.TabPageCollection get_tpc()
	{
		return _tpc;
	}

	public static IEnumerable<Subchart> allSubcharts(TabControl.TabPageCollection tabpages)
	{
		foreach (TabPage tabpage in tabpages)
		{
			if (tabpage is Subchart)
			{
				yield return tabpage as Subchart;
			}
			else if (tabpage is ClassTabPage)
			{
				ClassTabPage ctp = tabpage as ClassTabPage;
				for (int j = 0; j < ctp.tabControl1.TabPages.Count; j++)
				{
					yield return ctp.tabControl1.TabPages[j] as Subchart;
				}
			}
		}
	}

	public static IEnumerable<ClassTabPage> allClasses(TabControl.TabPageCollection tabpages)
	{
		foreach (TabPage tabpage in tabpages)
		{
			if (tabpage is ClassTabPage)
			{
				yield return tabpage as ClassTabPage;
			}
		}
	}

	public static IEnumerable<Procedure_Chart> allMethods(ClassTabPage ctp)
	{
		foreach (TabPage tabPage in ctp.tabControl1.TabPages)
		{
			if (tabPage is Procedure_Chart)
			{
				yield return tabPage as Procedure_Chart;
			}
		}
	}

	public static Subchart mainSubchart(TabControl.TabPageCollection tabpages)
	{
		if (Component.Current_Mode == Mode.Expert)
		{
			return (Subchart)tabpages[1];
		}
		return (Subchart)tabpages[0];
	}

	public static void Compile_Flowchart(TabControl.TabPageCollection tabpages)
	{
		_tpc = tabpages;
		Oval start = mainSubchart(tabpages).Start;
		foreach (Subchart item in allSubcharts(tabpages))
		{
			item.am_compiling = false;
		}
		mainSubchart(tabpages).am_compiling = true;
		Generate_IL gil = new Generate_IL("MyAssembly");
		try
		{
			Do_Compilation(start, gil, tabpages);
		}
		catch
		{
			foreach (Subchart item2 in allSubcharts(tabpages))
			{
				item2.am_compiling = false;
			}
			throw;
		}
		mainSubchart(tabpages).am_compiling = false;
	}

	public static void Compile_Flowchart_To(Oval start, string directory, string filename)
	{
		Generate_IL generate_IL = new Generate_IL(Path.GetFileNameWithoutExtension(filename));
		Do_Compilation(start, generate_IL, Runtime.parent.carlisle.TabPages);
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
		string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
		System.IO.File.Copy(Application.ExecutablePath, Path.Combine(directory, "raptor.dll"), overwrite: true);
		System.IO.File.Copy(Path.Combine(directoryName, "interpreter.dll"), Path.Combine(directory, "interpreter.dll"), overwrite: true);
		System.IO.File.Copy(Path.Combine(directoryName, "dotnetgraph.dll"), Path.Combine(directory, "dotnetgraph.dll"), overwrite: true);
		System.IO.File.Copy(Path.Combine(directoryName, "mgnat.dll"), Path.Combine(directory, "mgnat.dll"), overwrite: true);
		System.IO.File.Copy(Path.Combine(directoryName, "mgnatcs.dll"), Path.Combine(directory, "mgnatcs.dll"), overwrite: true);
		string[] plugin_List = Plugins.Get_Plugin_List();
		string[] assembly_Names = Plugins.Get_Assembly_Names();
		Runtime.consoleWriteln("DLLs copied");
		for (int i = 0; i < plugin_List.Length; i++)
		{
			System.IO.File.Copy(plugin_List[i], Path.Combine(directory, assembly_Names[i]) + ".dll", overwrite: true);
		}
		Runtime.consoleWriteln("Plugins copied");
		if (System.IO.File.Exists(Path.Combine(directory, filename)))
		{
			System.IO.File.Delete(Path.Combine(directory, filename));
		}
		Runtime.consoleWriteln("Old file deleted");
		generate_IL.Save_Result(filename);
		Runtime.consoleWriteln("New file saved");
		System.IO.File.Move(filename, Path.Combine(directory, filename));
		Runtime.consoleWriteln("Process complete");
	}

	public static void runCompiledHelper()
	{
		Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
		MethodInfo methodInfo = null;
		for (int i = 0; i < assemblies.Length; i++)
		{
			if (!(assemblies[i].GetName().Name == "MyAssembly"))
			{
				continue;
			}
			try
			{
				methodInfo = assemblies[i].GetType("MyType").GetMethod("Main");
			}
			catch
			{
				if (i == assemblies.Length - 1)
				{
					throw;
				}
			}
		}
		if (!(methodInfo != null))
		{
			return;
		}
		try
		{
			methodInfo.Invoke(null, null);
		}
		catch (ThreadAbortException)
		{
			Runtime.consoleWriteln("----compiled run aborted----");
		}
		catch (Exception ex2)
		{
			if (raptor_files_pkg.output_redirected() && from_commandline)
			{
				raptor_files_pkg.writeln("exception occurred! flowchart terminated abnormally\n" + ex2.Message);
				raptor_files_pkg.stop_redirect_output();
			}
			else if (ex2.InnerException != null)
			{
				Runtime.consoleWrite("Exception! " + ex2.InnerException.Message + ex2.InnerException.StackTrace);
			}
			else
			{
				Runtime.consoleWrite("Exception! " + ex2.Message + ex2.StackTrace);
			}
		}
	}

	public static void Run_Compiled_NoThread(bool was_from_commandline)
	{
		from_commandline = was_from_commandline;
		runCompiledHelper();
	}

	public static void Run_Compiled(bool was_from_commandline)
	{
		from_commandline = was_from_commandline;
		if (run_compiled_thread != null && run_compiled_thread.ThreadState == ThreadState.Running)
		{
			run_compiled_thread.Abort();
		}
		run_compiled_thread = new Thread(runCompiledHelper);
		run_compiled_thread.Start();
	}
}
