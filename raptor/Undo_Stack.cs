using System.Collections;
using System.Windows.Forms;
using Microsoft.Ink;

namespace raptor;

public class Undo_Stack
{
	public enum Action_Kind
	{
		Rename_Tab,
		Add_Tab,
		Delete_Tab,
		Change_Tab
	}

	public class Action
	{
		public Action_Kind kind;

		public Subchart chart;

		public Ink ink;

		public string old_name;

		public string new_name;

		public Component clone;

		public Action Clone()
		{
			Action obj = (Action)MemberwiseClone();
			obj.clone = clone.Clone();
			return obj;
		}
	}

	public static int num_undo = 0;

	public static int num_redo = 0;

	public static ArrayList Undo_array = new ArrayList();

	public static ArrayList Redo_array = new ArrayList();

	private const int Can_Undo = 8;

	private const int Can_Redo = 9;

	private const int No_Undo = 14;

	private const int No_Redo = 15;

	private static void Add_Undo_Action(Action new_action, Visual_Flow_Form form)
	{
		num_undo++;
		form.undoButton.ImageIndex = 8;
		form.menuItemUndo.Enabled = true;
		if (num_undo - 1 >= Undo_array.Count)
		{
			Undo_array.Add(new_action);
		}
		else
		{
			Undo_array[num_undo - 1] = new_action;
		}
	}

	private static void Clear_Redo(Visual_Flow_Form form)
	{
		num_redo = 0;
		form.redoButton.ImageIndex = 15;
		form.menuItemRedo.Enabled = false;
	}

	private static void Add_Redo_Action(Action new_action, Visual_Flow_Form form)
	{
		num_redo++;
		form.redoButton.ImageIndex = 9;
		form.menuItemRedo.Enabled = true;
		if (num_redo - 1 >= Redo_array.Count)
		{
			Redo_array.Add(new_action);
		}
		else
		{
			Redo_array[num_redo - 1] = new_action;
		}
	}

	public static void Decrement_Undoable(Visual_Flow_Form form)
	{
		num_undo--;
		if (num_undo < 1)
		{
			form.undoButton.ImageIndex = 14;
			form.menuItemUndo.Enabled = false;
			form.modified = false;
		}
	}

	private static void Decrement_Redoable(Visual_Flow_Form form)
	{
		num_redo--;
		if (num_redo < 1)
		{
			form.redoButton.ImageIndex = 15;
			form.menuItemRedo.Enabled = false;
			form.modified = false;
			num_redo = 0;
		}
	}

	public static void Make_Add_Tab_Undoable(Visual_Flow_Form form, Subchart chart)
	{
		form.modified = true;
		Add_Undo_Action(new Action
		{
			kind = Action_Kind.Add_Tab,
			chart = chart
		}, form);
		Clear_Redo(form);
	}

	public static void Make_Delete_Tab_Undoable(Visual_Flow_Form form, Subchart chart)
	{
		form.modified = true;
		Add_Undo_Action(new Action
		{
			kind = Action_Kind.Delete_Tab,
			chart = chart
		}, form);
		Clear_Redo(form);
	}

	public static void Make_Rename_Tab_Undoable(Visual_Flow_Form form, Subchart chart, string old_name, string new_name)
	{
		form.modified = true;
		Add_Undo_Action(new Action
		{
			kind = Action_Kind.Rename_Tab,
			new_name = new_name,
			old_name = old_name,
			chart = chart
		}, form);
		Clear_Redo(form);
	}

	public static void Make_Undoable(Visual_Flow_Form form)
	{
		Subchart subchart = form.selectedTabMaybeNull();
		if (subchart != null)
		{
			Action action = new Action();
			action.clone = subchart.Start.Clone();
			action.kind = Action_Kind.Change_Tab;
			action.chart = subchart;
			if (!Component.BARTPE && !Component.VM && !Component.MONO)
			{
				bool enabled = action.chart.tab_overlay.Enabled;
				action.chart.tab_overlay.Enabled = false;
				action.ink = action.chart.tab_overlay.Ink.Clone();
				action.chart.tab_overlay.Enabled = enabled;
			}
			Add_Undo_Action(action, form);
			Clear_Redo(form);
		}
	}

	public static void Undo_Action(Visual_Flow_Form form)
	{
		Subchart subchart = form.selectedTabMaybeNull();
		if (num_undo <= 0 || subchart == null)
		{
			return;
		}
		Action action = (Action)Undo_array[num_undo - 1];
		Action action2 = new Action();
		action2.kind = action.kind;
		action2.chart = action.chart;
		switch (action.kind)
		{
		case Action_Kind.Rename_Tab:
			action2.old_name = action.old_name;
			action2.new_name = action.new_name;
			Add_Redo_Action(action2, form);
			action.chart.Text = action.old_name;
			form.Rename_Tab(action.new_name, action.old_name);
			form.carlisle.SelectedTab = action.chart;
			break;
		case Action_Kind.Add_Tab:
			Add_Redo_Action(action2, form);
			form.carlisle.TabPages.Remove(action.chart);
			break;
		case Action_Kind.Delete_Tab:
			Add_Redo_Action(action2, form);
			form.carlisle.TabPages.Add(action.chart);
			form.carlisle.SelectedTab = action.chart;
			break;
		case Action_Kind.Change_Tab:
		{
			action2.clone = action.chart.Start.Clone();
			bool enabled = true;
			if (!Component.BARTPE && !Component.VM && !Component.MONO)
			{
				enabled = action.chart.tab_overlay.Enabled;
				action.chart.tab_overlay.Enabled = false;
				action2.ink = action.chart.tab_overlay.Ink.Clone();
			}
			Add_Redo_Action(action2, form);
			action.chart.Start = (Oval)action.clone.Clone();
			if (!Component.BARTPE && !Component.VM && !Component.MONO)
			{
				action.chart.tab_overlay.Ink = action.ink.Clone();
				action.chart.tab_overlay.Enabled = enabled;
			}
			action.chart.Start.scale = form.scale;
			action.chart.Start.Scale(form.scale);
			form.my_layout();
			form.Current_Selection = subchart.Start.select(-1000, -1000);
			action.chart.flow_panel.Invalidate();
			(action.chart.Parent as TabControl).SelectedTab = action.chart;
			if (action.chart.Parent != form.carlisle)
			{
				form.carlisle.SelectedTab = action.chart.Parent.Parent as TabPage;
			}
			break;
		}
		}
		Decrement_Undoable(form);
	}

	public static void Redo_Action(Visual_Flow_Form form)
	{
		Subchart subchart = form.selectedTabMaybeNull();
		if (num_redo <= 0 || subchart == null)
		{
			return;
		}
		Action action = (Action)Redo_array[num_redo - 1];
		Action action2 = new Action();
		action2.kind = action.kind;
		action2.chart = action.chart;
		switch (action.kind)
		{
		case Action_Kind.Rename_Tab:
			action2.old_name = action.old_name;
			action2.new_name = action.new_name;
			Add_Undo_Action(action2, form);
			action.chart.Text = action.new_name;
			form.Rename_Tab(action.old_name, action.new_name);
			form.carlisle.SelectedTab = action.chart;
			break;
		case Action_Kind.Add_Tab:
			Add_Undo_Action(action2, form);
			form.carlisle.TabPages.Add(action.chart);
			form.carlisle.SelectedTab = action.chart;
			break;
		case Action_Kind.Delete_Tab:
			Add_Undo_Action(action2, form);
			form.carlisle.TabPages.Remove(action.chart);
			break;
		case Action_Kind.Change_Tab:
		{
			action2.clone = action.chart.Start.Clone();
			bool enabled = true;
			if (!Component.BARTPE && !Component.VM && !Component.MONO)
			{
				enabled = action.chart.tab_overlay.Enabled;
				action.chart.tab_overlay.Enabled = false;
				action2.ink = action.chart.tab_overlay.Ink.Clone();
			}
			Add_Undo_Action(action2, form);
			action.chart.Start = (Oval)action.clone.Clone();
			if (!Component.BARTPE && !Component.VM && !Component.MONO)
			{
				action.chart.tab_overlay.Ink = action.ink.Clone();
				action.chart.tab_overlay.Enabled = enabled;
			}
			action.chart.Start.scale = form.scale;
			action.chart.Start.Scale(form.scale);
			form.my_layout();
			form.Current_Selection = subchart.Start.select(-1000, -1000);
			action.chart.flow_panel.Invalidate();
			(action.chart.Parent as TabControl).SelectedTab = action.chart;
			if (action.chart.Parent != form.carlisle)
			{
				form.carlisle.SelectedTab = action.chart.Parent.Parent as TabPage;
			}
			break;
		}
		}
		Decrement_Redoable(form);
	}

	public static void Clear_Undo(Visual_Flow_Form form)
	{
		num_undo = 0;
		num_redo = 0;
		form.redoButton.ImageIndex = 15;
		form.menuItemRedo.Enabled = false;
		form.undoButton.ImageIndex = 14;
		form.menuItemUndo.Enabled = false;
		form.modified = false;
	}
}
