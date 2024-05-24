using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using interpreter;
using NClass.Core;

namespace raptor;

public class Dialog_Helpers
{
	private static int current_suggestion_line = -1;

	private static bool have_bold;

	private static List<string> list;

	private static List<string> oo_suggestion_list;

	private static Dictionary<string, string> types;

	public static List<string> Get_List()
	{
		return list;
	}

	public static List<string> Get_OO_List()
	{
		return oo_suggestion_list;
	}

	internal static int Suggestions(CompositeType startingClass, string name, bool isStatic)
	{
		CompositeType compositeType = startingClass;
		while (compositeType != null && !compositeType.Name.Equals("Object"))
		{
			foreach (Field field in compositeType.Fields)
			{
				if (field.IsStatic == isStatic && field.Name.ToLower().StartsWith(name))
				{
					oo_suggestion_list.Add(field.Name);
				}
			}
			foreach (Operation operation in compositeType.Operations)
			{
				if (!(operation is Method) || operation.IsStatic != isStatic || operation is Constructor || !operation.Name.ToLower().StartsWith(name))
				{
					continue;
				}
				Method method = operation as Method;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(method.Name + "(");
				method.getParameters(out var num_params, out var param_names, out var _, out var _);
				for (int i = 0; i < num_params; i++)
				{
					stringBuilder.Append(param_names[i]);
					if (i < num_params - 1)
					{
						stringBuilder.Append(",");
					}
				}
				stringBuilder.Append(")");
				oo_suggestion_list.Add(stringBuilder.ToString());
			}
			compositeType = ((!(compositeType is ClassType)) ? null : (compositeType as ClassType).BaseClass);
		}
		return oo_suggestion_list.Count;
	}

	internal static int Prefix_Suggestion_Count(string name)
	{
		oo_suggestion_list.Clear();
		int num = name.IndexOf('.');
		if (name.LastIndexOf('.') != num)
		{
			return 0;
		}
		if (name.StartsWith("this."))
		{
			if (Runtime.parent.carlisle.SelectedTab is ClassTabPage && ((Runtime.parent.carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).method != null)
			{
				return Suggestions(((Runtime.parent.carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).method.Parent as ClassType, name.Substring(num + 1), isStatic: false);
			}
		}
		else if (name.StartsWith("super."))
		{
			if (Runtime.parent.carlisle.SelectedTab is ClassTabPage && ((Runtime.parent.carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).method != null)
			{
				return Suggestions((((Runtime.parent.carlisle.SelectedTab as ClassTabPage).tabControl1.SelectedTab as Procedure_Chart).method.Parent as ClassType).BaseClass, name.Substring(num + 1), isStatic: false);
			}
		}
		else
		{
			foreach (IEntity entity in Runtime.parent.projectCore.Entities)
			{
				if (name.StartsWith(entity.Name.ToLower() + "."))
				{
					if (entity is CompositeType)
					{
						return Suggestions(entity as CompositeType, name.Substring(num + 1), isStatic: true);
					}
					_ = entity is EnumType;
				}
			}
			if (types.ContainsKey(name.Substring(0, num).ToLower()))
			{
				string text = types[name.Substring(0, num).ToLower()];
				foreach (IEntity entity2 in Runtime.parent.projectCore.Entities)
				{
					if (entity2.Name.ToLower().Equals(text.ToLower()))
					{
						if (entity2 is CompositeType)
						{
							return Suggestions(entity2 as CompositeType, name.Substring(num + 1), isStatic: false);
						}
						_ = entity2 is EnumType;
					}
				}
			}
		}
		return 0;
	}

	public static void Init()
	{
		have_bold = false;
		current_suggestion_line = -1;
		list = new List<string>();
		types = new Dictionary<string, string>();
		oo_suggestion_list = new List<string>();
		if (Runtime.parent.carlisle.SelectedTab is ClassTabPage)
		{
			((Procedure_Chart)((ClassTabPage)Runtime.parent.carlisle.SelectedTab).tabControl1.SelectedTab).Start.collect_variable_names(list, types);
			Method method = ((Procedure_Chart)((ClassTabPage)Runtime.parent.carlisle.SelectedTab).tabControl1.SelectedTab).method;
			if (method != null)
			{
				for (int i = 0; i < method.numberArguments; i++)
				{
					Parameter parameter = method.getParameter(i);
					if (!types.ContainsKey(parameter.Name.ToLower()))
					{
						types.Add(parameter.Name.ToLower(), parameter.Type);
					}
				}
			}
		}
		else if (Runtime.parent.carlisle.SelectedTab is Procedure_Chart)
		{
			((Procedure_Chart)Runtime.parent.carlisle.SelectedTab).Start.collect_variable_names(list, types);
		}
		else
		{
			for (int j = Runtime.parent.mainIndex; j < Runtime.parent.carlisle.TabCount; j++)
			{
				if (Runtime.parent.carlisle.TabPages[j] is Subchart && !(Runtime.parent.carlisle.TabPages[j] is Procedure_Chart))
				{
					((Subchart)Runtime.parent.carlisle.TabPages[j]).Start.collect_variable_names(list, types);
				}
			}
		}
		if (Component.Current_Mode == Mode.Expert)
		{
			if (Runtime.parent.carlisle.SelectedIndex != 1)
			{
				list.Add("this");
				list.Add("super");
			}
			foreach (IEntity entity in Runtime.parent.projectCore.Entities)
			{
				if (entity is ClassType)
				{
					list.Add(entity.Name);
				}
			}
		}
		list.Sort();
		int num = 0;
		while (num < list.Count - 1)
		{
			if (list[num].ToLower() == list[num + 1].ToLower())
			{
				list.Remove(list[num + 1]);
			}
			else
			{
				num++;
			}
		}
	}

	public static bool Is_Simple_String(string s)
	{
		if (s[0] != '"' || s[s.Length - 1] != '"')
		{
			return false;
		}
		for (int i = 1; i < s.Length - 1; i++)
		{
			if (s[i] == '\\')
			{
				if (i == s.Length - 2)
				{
					return false;
				}
				i++;
			}
			else if (s[i] == '"')
			{
				if (i == s.Length - 2 || s[i + 1] != '"')
				{
					return false;
				}
				i++;
			}
		}
		return true;
	}

	public static void Check_Hint(TextBox textbox, RichTextBox textBox1, int kind, ref string current_suggestion, ref suggestion_result suggestion_result, ref bool error, Font this_Font)
	{
		if (textbox.Lines.Length > 1)
		{
			textbox.Text = textbox.Lines[0] + textbox.Lines[1];
			textbox.Select(textbox.Text.Length, 0);
		}
		suggestion_result = interpreter_pkg.suggestion(textbox.Text, kind);
		have_bold = suggestion_result.bold_start > 0 && suggestion_result.bold_finish >= suggestion_result.bold_start;
		if (have_bold)
		{
			string[] array = new string[suggestion_result.suggestions.Length - 1];
			for (int i = 1; i < suggestion_result.suggestions.Length; i++)
			{
				array[i - 1] = suggestion_result.suggestions[i];
			}
			Array.Sort(array);
			for (int j = 1; j < suggestion_result.suggestions.Length; j++)
			{
				suggestion_result.suggestions[j] = array[j - 1];
			}
		}
		else
		{
			Array.Sort(suggestion_result.suggestions);
		}
		if (!Component.MONO)
		{
			textBox1.Lines = suggestion_result.suggestions;
		}
		else
		{
			textBox1.Lines = addNewlines(suggestion_result.suggestions);
		}
		textBox1.SelectAll();
		textBox1.SelectionColor = Color.Black;
		if (suggestion_result.suggestions.Length == 0)
		{
			current_suggestion = "";
			current_suggestion_line = -1;
			textBox1.Hide();
			return;
		}
		if (suggestion_result.suggestions.Length > 1)
		{
			int num;
			if (current_suggestion != null)
			{
				try
				{
					num = textBox1.Find(current_suggestion);
				}
				catch
				{
					num = -1;
				}
			}
			else
			{
				num = -1;
			}
			if (num < 0 && !have_bold)
			{
				current_suggestion = suggestion_result.suggestions[0];
				current_suggestion_line = 0;
			}
			else if (num <= 0 && have_bold)
			{
				current_suggestion = suggestion_result.suggestions[1];
				current_suggestion_line = 1;
			}
			try
			{
				int num2 = textBox1.Find(current_suggestion);
				current_suggestion_line = textBox1.GetLineFromCharIndex(num2);
				textBox1.Select(num2, current_suggestion.Length);
			}
			catch
			{
			}
			textBox1.SelectionColor = Color.Red;
			error = false;
			textBox1.Show();
		}
		else if (suggestion_result.suggestions.Length == 1)
		{
			current_suggestion = suggestion_result.suggestions[0];
			current_suggestion_line = 0;
			try
			{
				textBox1.Select(textBox1.Find(current_suggestion), current_suggestion.Length);
				if (have_bold)
				{
					textBox1.SelectionColor = Color.Black;
				}
				else
				{
					textBox1.SelectionColor = Color.Red;
				}
			}
			catch
			{
			}
		}
		if (have_bold)
		{
			textBox1.Select(suggestion_result.bold_start - 1, suggestion_result.bold_finish - suggestion_result.bold_start + 1);
			textBox1.SelectionFont = new Font(this_Font, FontStyle.Bold);
			textBox1.Select(0, textBox1.Lines[0].Length);
			textBox1.SelectionColor = Color.Black;
		}
		error = false;
		textBox1.Show();
	}

	private static string[] addNewlines(string[] p)
	{
		string[] array = new string[p.Length];
		for (int i = 0; i < p.Length; i++)
		{
			array[i] = p[i] + "\n";
		}
		return array;
	}

	public static bool Complete_Suggestion(TextBox textbox, int kind, string current_suggestion, ref suggestion_result suggestion_result)
	{
		bool result = false;
		suggestion_result = interpreter_pkg.suggestion(textbox.Text, kind);
		if (current_suggestion != null && current_suggestion != "" && textbox.Text.Length > 1 && textbox.Text[textbox.Text.Length - 1] != ')' && (suggestion_result.bold_start < 0 || suggestion_result.bold_finish < suggestion_result.bold_start || current_suggestion_line != 0))
		{
			result = true;
			int num = textbox.Text.Length - 1;
			while (num > 0 && (char.IsLetterOrDigit(textbox.Text, num) || textbox.Text[num] == '_'))
			{
				num--;
			}
			int num2 = current_suggestion.IndexOf("(");
			if (num2 < 0)
			{
				num2 = current_suggestion.Length;
			}
			if (textbox.Text.Length - num >= current_suggestion.Length)
			{
				return false;
			}
			if (num == 0)
			{
				textbox.Text = current_suggestion.Substring(0, num2);
			}
			else
			{
				textbox.Text = textbox.Text.Substring(0, num + 1) + current_suggestion.Substring(0, num2);
			}
			textbox.SelectionStart = textbox.Text.Length;
			textbox.SelectionLength = 0;
		}
		return result;
	}

	public static void Paint_Helper(Graphics labelGraphics, string text, Label label, string error_msg, int location, bool error)
	{
		if (error)
		{
			labelGraphics.Clear(Color.White);
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			labelGraphics.DrawString(error_msg, PensBrushes.arial8, PensBrushes.blackbrush, 0f, 12f, stringFormat);
			int length = text.Length;
			if (length < location)
			{
				string text2 = text.Substring(0, length);
				labelGraphics.DrawString(text2, PensBrushes.arial8, PensBrushes.blackbrush, 0f, 25f, stringFormat);
				int num = Convert.ToInt32(labelGraphics.MeasureString(text2, PensBrushes.arial8).Width);
				string s = "_";
				labelGraphics.DrawString(s, PensBrushes.arial8, PensBrushes.redbrush, num - 3, 25f, stringFormat);
			}
			if (length == location)
			{
				string text2 = text.Substring(0, length - 1);
				labelGraphics.DrawString(text2, PensBrushes.arial8, PensBrushes.blackbrush, 0f, 25f, stringFormat);
				int num2 = Convert.ToInt32(labelGraphics.MeasureString(text2, PensBrushes.arial8).Width);
				string s = text.Substring(location - 1, 1);
				labelGraphics.DrawString(s, PensBrushes.arial8, PensBrushes.redbrush, num2 - 3, 25f, stringFormat);
			}
			if (length > location)
			{
				string text2 = text.Substring(0, location - 1);
				labelGraphics.DrawString(text2, PensBrushes.arial8, PensBrushes.blackbrush, 0f, 25f, stringFormat);
				int num3 = Convert.ToInt32(labelGraphics.MeasureString(text2, PensBrushes.arial8).Width);
				string s = text.Substring(location, length - location);
				string text3 = text.Substring(location - 1, 1);
				int num4 = Convert.ToInt32(labelGraphics.MeasureString(text3, PensBrushes.arial8).Width);
				labelGraphics.DrawString(text2, PensBrushes.arial8, PensBrushes.blackbrush, 0f, 25f, stringFormat);
				labelGraphics.DrawString(text3, PensBrushes.arial8, PensBrushes.redbrush, num3 - 3, 25f, stringFormat);
				labelGraphics.DrawString(s, PensBrushes.arial8, PensBrushes.blackbrush, num3 + num4 - 6, 25f, stringFormat);
			}
		}
		else
		{
			labelGraphics.Clear(label.BackColor);
		}
	}

	public static void suggestions_downarrow(RichTextBox textBox1, ref string current_suggestion)
	{
		int num = -1;
		if (!textBox1.Visible)
		{
			return;
		}
		for (int i = 0; i < textBox1.Lines.Length - 1; i++)
		{
			if (textBox1.Lines[i].Equals(current_suggestion))
			{
				num = i;
			}
		}
		if (num < 0)
		{
			return;
		}
		try
		{
			textBox1.Select(textBox1.Find(current_suggestion), textBox1.Lines[num].Length);
			textBox1.SelectionColor = Color.Black;
			textBox1.Select(textBox1.Find(textBox1.Lines[num + 1]), textBox1.Lines[num + 1].Length);
			textBox1.SelectionColor = Color.Red;
			current_suggestion_line = num + 1;
			current_suggestion = textBox1.Lines[current_suggestion_line];
			int charIndexFromPosition = textBox1.GetCharIndexFromPosition(new Point(3, 3));
			int lineFromCharIndex = textBox1.GetLineFromCharIndex(charIndexFromPosition);
			if (num - lineFromCharIndex > 5)
			{
				textBox1.Select(textBox1.Find(textBox1.Lines[lineFromCharIndex + 1]), textBox1.Lines[lineFromCharIndex + 1].Length);
				textBox1.ScrollToCaret();
			}
		}
		catch
		{
		}
	}

	public static void suggestions_uparrow(RichTextBox textBox1, ref string current_suggestion)
	{
		int num = -1;
		if (!textBox1.Visible)
		{
			return;
		}
		for (int i = 1; i < textBox1.Lines.Length; i++)
		{
			if (textBox1.Lines[i].Equals(current_suggestion))
			{
				num = i;
			}
		}
		if (num <= (have_bold ? 1 : 0))
		{
			return;
		}
		try
		{
			textBox1.Select(textBox1.Find(current_suggestion), textBox1.Lines[num].Length);
			textBox1.SelectionColor = Color.Black;
			textBox1.Select(textBox1.Find(textBox1.Lines[num - 1]), textBox1.Lines[num - 1].Length);
			textBox1.SelectionColor = Color.Red;
			current_suggestion = textBox1.Lines[num - 1];
			current_suggestion_line = num - 1;
			int charIndexFromPosition = textBox1.GetCharIndexFromPosition(new Point(3, 3));
			int lineFromCharIndex = textBox1.GetLineFromCharIndex(charIndexFromPosition);
			if (num <= lineFromCharIndex)
			{
				textBox1.Select(textBox1.Find(textBox1.Lines[lineFromCharIndex - 1]), textBox1.Lines[lineFromCharIndex - 1].Length);
				textBox1.ScrollToCaret();
			}
		}
		catch
		{
		}
	}

	public static void suggestions_mousedown(RichTextBox textBox1, ref string current_suggestion, MouseEventArgs e)
	{
		int charIndexFromPosition = textBox1.GetCharIndexFromPosition(new Point(e.X, e.Y));
		int lineFromCharIndex = textBox1.GetLineFromCharIndex(charIndexFromPosition);
		if (lineFromCharIndex < (have_bold ? 1 : 0) || lineFromCharIndex >= textBox1.Lines.Length || textBox1.Lines.Length <= 1)
		{
			return;
		}
		try
		{
			if (current_suggestion_line >= 0)
			{
				textBox1.Select(textBox1.Find(current_suggestion), textBox1.Lines[current_suggestion_line].Length);
			}
			textBox1.SelectionColor = Color.Black;
			current_suggestion = textBox1.Lines[lineFromCharIndex];
			current_suggestion_line = lineFromCharIndex;
			textBox1.Select(textBox1.Find(current_suggestion), textBox1.Lines[lineFromCharIndex].Length);
			textBox1.SelectionColor = Color.Red;
		}
		catch
		{
		}
	}
}
