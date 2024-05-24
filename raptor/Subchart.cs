using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Ink;

namespace raptor;

public class Subchart : TabPage
{
	public Panel panel1;

	public Buffered flow_panel;

	protected static Visual_Flow_Form form;

	public Oval Start;

	public Oval End;

	private int selected_comment_x;

	private int selected_comment_y;

	public bool am_compiling;

	public System.Drawing.Rectangle selection_rectangle;

	public Point scroll_location = new Point(0, 0);

	public bool region_selected;

	protected Subchart_Kinds kind;

	public InkOverlay tab_overlay;

	private bool have_cut_drag;

	private bool have_drag;

	private Component current_selection;

	public Component Breakpoint_Selection;

	public CommentBox selectedComment;

	private int drag_x;

	private int drag_y;

	private bool possible_drag_drop;

	public double ink_resolution;

	private bool droppable_icon;

	private bool droppable_file;

	public virtual int num_params => 0;

	public Subchart_Kinds Subchart_Kind => kind;

	public Component Current_Selection
	{
		get
		{
			return current_selection;
		}
		set
		{
			current_selection = value;
			region_selected = false;
		}
	}

	public bool Am_Dragging => have_drag;

	public virtual string getFullName()
	{
		return Text;
	}

	protected void initialize(Visual_Flow_Form the_form, string s)
	{
		form = the_form;
		Text = s;
		panel1 = new Panel();
		flow_panel = new Buffered();
		panel1.AutoScroll = true;
		panel1.BackColor = SystemColors.Window;
		panel1.Controls.Add(flow_panel);
		panel1.Dock = DockStyle.Fill;
		panel1.Location = new Point(136, 32);
		panel1.Name = "panel1";
		panel1.Size = new Size(578, 480);
		panel1.TabIndex = 3;
		flow_panel.AllowDrop = true;
		flow_panel.AutoScrollMargin = new Size(5, 5);
		flow_panel.BackColor = SystemColors.Window;
		flow_panel.Location = new Point(0, 0);
		flow_panel.Name = "flow_panel";
		flow_panel.Size = new Size(512, 296);
		flow_panel.TabIndex = 2;
		flow_panel.MouseUp += flow_panel_MouseUp;
		flow_panel.Paint += flow_panel_Paint;
		flow_panel.MouseEnter += flow_panel_MouseEnter;
		flow_panel.Leave += flow_panel_Leave;
		flow_panel.DoubleClick += flow_dbl_Click;
		flow_panel.MouseMove += Set_Hover;
		flow_panel.MouseLeave += flow_panel_MouseLeave;
		flow_panel.MouseDown += select_flow_shape;
		flow_panel.DragDrop += carlisle_DragDrop;
		flow_panel.DragEnter += carlisle_DragEnter;
		flow_panel.DragOver += carlisle_DragOver;
		form.GiveFeedback += form_GiveFeedback;
		base.GiveFeedback += carlisle_GiveFeedback;
		((Control)flow_panel).KeyDown += form.Visual_Flow_Form_KeyDown;
		base.Controls.Add(panel1);
		End = new Oval(60, 90, "Oval");
		if (!Component.USMA_mode)
		{
			End.Text = "End";
		}
		else
		{
			End.Text = "Stop";
		}
	}

	protected Subchart()
	{
	}

	public void tab_disposed(object o, EventArgs e)
	{
		if (!Component.BARTPE && !Component.VM && !Component.MONO)
		{
			tab_overlay.Dispose();
		}
	}

	public Subchart(Visual_Flow_Form the_form, string s)
	{
		initialize(the_form, s);
		Start = new Oval(End, 60, 90, "Oval");
		Start.Text = "Start";
		Start.scale = form.scale;
		Start.Scale(form.scale);
		if (!Component.MONO)
		{
			Initialize_Ink();
		}
		flow_panel.Invalidate();
		kind = Subchart_Kinds.Subchart;
	}

	protected void Initialize_Ink()
	{
		if (!Component.BARTPE && !Component.VM && !Component.compiled_flowchart && !Component.MONO)
		{
			tab_overlay = new InkOverlay(flow_panel);
			tab_overlay.Enabled = false;
			tab_overlay.EditingMode = InkOverlayEditingMode.Ink;
			tab_overlay.CursorInRange += tab_overlay_CursorInRange;
			tab_overlay.Stroke += tab_overlay_Stroke;
			tab_overlay.StrokesDeleted += tab_overlay_StrokesDeleted;
			base.Disposed += tab_disposed;
			Matrix viewTransform = new Matrix();
			Matrix viewTransform2 = new Matrix();
			tab_overlay.Renderer.GetViewTransform(ref viewTransform2);
			tab_overlay.Renderer.SetViewTransform(viewTransform);
			Graphics graphics = flow_panel.CreateGraphics();
			Point pt = new Point(50, 50);
			Point pt2 = new Point(100, 100);
			tab_overlay.Renderer.InkSpaceToPixel(graphics, ref pt);
			tab_overlay.Renderer.InkSpaceToPixel(graphics, ref pt2);
			ink_resolution = Math.Abs(50.0 / (double)(pt2.X - pt.X));
			tab_overlay.Renderer.SetViewTransform(viewTransform2);
			graphics.Dispose();
		}
	}

	private void tab_overlay_StrokesDeleted(object sender, EventArgs e)
	{
		form.modified = true;
	}

	private void tab_overlay_Stroke(object sender, InkCollectorStrokeEventArgs e)
	{
		form.modified = true;
	}

	private void tab_overlay_CursorInRange(object sender, InkCollectorCursorInRangeEventArgs e)
	{
		if (e.Cursor.Inverted && tab_overlay.EditingMode != InkOverlayEditingMode.Delete)
		{
			tab_overlay.Enabled = false;
			tab_overlay.EditingMode = InkOverlayEditingMode.Delete;
			tab_overlay.EraserMode = InkOverlayEraserMode.StrokeErase;
			tab_overlay.Enabled = true;
		}
		else if (!form.menuItemInkErase.Checked && !form.menuItemInkSelect.Checked)
		{
			tab_overlay.Enabled = false;
			tab_overlay.EditingMode = InkOverlayEditingMode.Ink;
			tab_overlay.Enabled = true;
		}
	}

	public void scale_ink(float scale)
	{
		if (!Component.BARTPE && !Component.VM && !Component.MONO)
		{
			Matrix matrix = new Matrix();
			tab_overlay.Renderer.SetViewTransform(matrix);
			float offsetX = (float)(95.0 * ink_resolution);
			matrix.Translate(offsetX, 0f);
			matrix.Scale(scale, scale);
			tab_overlay.Renderer.SetViewTransform(matrix);
		}
	}

	private void flow_panel_Paint(object sender, PaintEventArgs e)
	{
		if (Start != null)
		{
			Start.footprint(e.Graphics);
			new Point(50, 50);
			new Point(100, 100);
			int num = Start.FP.left + 90;
			int num2 = (int)Math.Round(form.scale * 30f);
			form.my_layout();
			if (Component.compiled_flowchart)
			{
				num = Start.FP.left + 90;
				num2 = (int)Math.Round(form.scale * 30f);
			}
			Start.draw(e.Graphics, num, num2);
			if (selection_rectangle.Width > 0)
			{
				e.Graphics.DrawRectangle(PensBrushes.black_dash_pen, selection_rectangle);
			}
		}
	}

	private void select_flow_shape(object sender, MouseEventArgs e)
	{
		if ((!Component.BARTPE && !Component.VM && !Component.MONO && tab_overlay.Enabled) || Component.compiled_flowchart)
		{
			form.mouse_x = e.X;
			form.mouse_y = e.Y;
			return;
		}
		possible_drag_drop = false;
		if (e.Button == MouseButtons.Left && Start.check_expansion_click(e.X, e.Y))
		{
			form.mouse_x = e.X;
			form.mouse_y = e.Y;
			flow_panel.Invalidate();
		}
		else if (!form.runningState)
		{
			form.mouse_x = e.X;
			form.mouse_y = e.Y;
			if (e.Button == MouseButtons.Left)
			{
				Component component = Start.Find_Component(form.mouse_x, form.mouse_y);
				if (component != null && component.selected)
				{
					possible_drag_drop = true;
					return;
				}
				if (form.control_figure_selected < 0)
				{
					Current_Selection = Start.select(form.mouse_x, form.mouse_y);
				}
				if (form.control_figure_selected == 0)
				{
					Insert_Figure(new Rectangle(60, 90, "Rectangle", Rectangle.Kind_Of.Assignment));
				}
				else if (form.control_figure_selected == 1)
				{
					Insert_Figure(new Rectangle(60, 90, "Rectangle", Rectangle.Kind_Of.Call));
				}
				else if (form.control_figure_selected == 2)
				{
					Insert_Figure(new Parallelogram(60, 90, "Parallelogram", input: true));
				}
				else if (form.control_figure_selected == 3)
				{
					Insert_Figure(new Parallelogram(60, 90, "Parallelogram", input: false));
				}
				else if (form.control_figure_selected == 4)
				{
					Insert_Figure(new IF_Control(60, 90, "IF_Control"));
				}
				else if (form.control_figure_selected == 5)
				{
					Insert_Figure(new Loop(60, 90, "Loop"));
				}
				else if (form.control_figure_selected == 6)
				{
					Insert_Figure(new Oval_Return(60, 90, "Return"));
				}
				if (component != null && component.selected)
				{
					Start.Scale(form.scale);
					form.my_layout();
					flow_panel.Invalidate();
					possible_drag_drop = true;
					return;
				}
				selectedComment = Start.selectComment(form.mouse_x, form.mouse_y);
				if (selectedComment != null)
				{
					selected_comment_x = form.mouse_x - selectedComment.parent.x_location - (int)((float)selectedComment.X * form.scale);
					selected_comment_y = form.mouse_y - selectedComment.parent.y_location - (int)((float)selectedComment.Y * form.scale);
				}
				Start.Scale(form.scale);
				form.my_layout();
				flow_panel.Invalidate();
			}
			else if (e.Button == MouseButtons.Right)
			{
				if (!region_selected)
				{
					Current_Selection = Start.select(form.mouse_x, form.mouse_y);
					selectedComment = Start.selectComment(form.mouse_x, form.mouse_y);
					flow_panel.Invalidate();
				}
				if (flow_panel.Cursor == System.Windows.Forms.Cursors.Hand)
				{
					IDataObject dataObject = ClipboardMultiplatform.GetDataObject();
					Component.warned_about_error = true;
					Clipboard_Data clipboard_Data = (Clipboard_Data)dataObject.GetData("raptor.Clipboard_Data");
					form.contextMenu2Paste.Enabled = (clipboard_Data != null && clipboard_Data.kind == Clipboard_Data.kinds.symbols && Current_Selection == null) || (clipboard_Data != null && clipboard_Data.kind == Clipboard_Data.kinds.comment && Current_Selection != null);
					form.contextMenuInsert.Show(flow_panel, new Point(form.mouse_x, form.mouse_y));
				}
				else
				{
					Breakpoint_Selection = Current_Selection;
					form.contextMenu1.Show(flow_panel, new Point(form.mouse_x, form.mouse_y));
				}
			}
		}
		else if (e.Button == MouseButtons.Right)
		{
			form.mouse_x = e.X;
			form.mouse_y = e.Y;
			Breakpoint_Selection = Start.select(e.X, e.Y);
			Current_Selection = Start.select(-1000, -1000);
			form.contextMenu2.Show(flow_panel, new Point(e.X, e.Y));
		}
	}

	private void Start_DragDrop(MouseEventArgs e)
	{
		Point point = panel1.PointToClient(new Point(e.X, e.Y));
		_ = point.X;
		Math.Abs(panel1.AutoScrollPosition.X);
		_ = point.Y;
		Math.Abs(panel1.AutoScrollPosition.Y);
		have_cut_drag = false;
		have_drag = true;
		drag_x = e.X;
		drag_y = e.Y;
		DoDragDrop("raptor_DRAG", DragDropEffects.Move | DragDropEffects.Link);
	}

	private void Insert_Figure(Component c)
	{
		form.Make_Undoable();
		if (Start.insert(c, form.mouse_x, form.mouse_y, 0))
		{
			Current_Selection = Start.select(-1000, -1000);
			return;
		}
		Undo_Stack.Decrement_Undoable(form);
		Current_Selection = Start.select(form.mouse_x, form.mouse_y);
	}

	private void flow_panel_MouseLeave(object sender, EventArgs e)
	{
		scroll_location = panel1.AutoScrollPosition;
	}

	private void flow_panel_Leave(object sender, EventArgs e)
	{
		scroll_location = panel1.AutoScrollPosition;
	}

	private void flow_dbl_Click(object sender, EventArgs e)
	{
		if ((Component.BARTPE || Component.VM || Component.MONO || !tab_overlay.Enabled) && !Component.compiled_flowchart && !form.runningState)
		{
			if (Start.setText(form.mouse_x, form.mouse_y, form))
			{
				form.my_layout();
				flow_panel.Invalidate();
				form.my_layout();
			}
			else if (selectedComment != null)
			{
				selectedComment.setText(form);
			}
			flow_panel.Invalidate();
		}
	}

	private void Set_Hover(object sender, MouseEventArgs e)
	{
		if (!Component.BARTPE && !Component.VM && !Component.MONO && tab_overlay.Enabled && !Component.compiled_flowchart)
		{
			return;
		}
		if ((!form.full_speed || !form.continuous_Run) && !Component.compiled_flowchart)
		{
			form.tooltip_text = Start.getText(e.X, e.Y);
			form.toolTip1.SetToolTip(flow_panel, form.tooltip_text);
		}
		if (form.runningState)
		{
			return;
		}
		if (Start.insert(null, e.X, e.Y, 0))
		{
			Cursor = System.Windows.Forms.Cursors.Hand;
		}
		else
		{
			Cursor = System.Windows.Forms.Cursors.Default;
		}
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		Component component = Start.Find_Component(e.X, e.Y);
		if (component != null && component.selected && possible_drag_drop)
		{
			possible_drag_drop = false;
			Start_DragDrop(e);
			return;
		}
		if (selectedComment != null)
		{
			if (e.X < 0)
			{
				selectedComment.X = (int)((float)(-selectedComment.parent.x_location) / form.scale);
			}
			else
			{
				selectedComment.X = (int)((float)(e.X - selected_comment_x - selectedComment.parent.x_location) / form.scale);
			}
			if (e.Y < 0)
			{
				selectedComment.Y = (int)((float)(-selectedComment.parent.y_location) / form.scale);
			}
			else
			{
				selectedComment.Y = (int)((float)(e.Y - selected_comment_y - selectedComment.parent.y_location) / form.scale);
			}
			flow_panel.Invalidate();
			return;
		}
		int num = ((form.mouse_x < e.X) ? form.mouse_x : e.X);
		int num2 = ((form.mouse_y < e.Y) ? form.mouse_y : e.Y);
		int num3 = Math.Abs(form.mouse_x - e.X);
		int num4 = Math.Abs(form.mouse_y - e.Y);
		if (num3 > 0 && num4 > 0)
		{
			selection_rectangle = new System.Drawing.Rectangle(num, num2, num3, num4);
			region_selected = Start.SelectRegion(selection_rectangle);
		}
		else
		{
			selection_rectangle.Width = 0;
		}
		flow_panel.Invalidate();
	}

	private void flow_panel_MouseUp(object sender, MouseEventArgs e)
	{
		if (Component.BARTPE || Component.VM || Component.MONO || !tab_overlay.Enabled || Component.compiled_flowchart)
		{
			have_drag = false;
			if (selection_rectangle.Width > 0)
			{
				selection_rectangle.Width = 0;
				flow_panel.Invalidate();
			}
		}
	}

	public void flow_panel_MouseEnter(object sender, EventArgs e)
	{
		if (!form.ContainsFocus || (form.runningState && PromptForm.current != null))
		{
			return;
		}
		if (!flow_panel.Focused && !panel1.Focused)
		{
			flow_panel.Focus();
			if (scroll_location.Y < 0)
			{
				scroll_location.Y = -scroll_location.Y;
			}
			if (scroll_location.X < 0)
			{
				scroll_location.X = -scroll_location.X;
			}
			panel1.AutoScrollPosition = scroll_location;
		}
		else
		{
			flow_panel.Focus();
		}
	}

	public void Activated(object sender, EventArgs e)
	{
		if (scroll_location.Y < 0)
		{
			scroll_location.Y = -scroll_location.Y;
		}
		if (scroll_location.X < 0)
		{
			scroll_location.X = -scroll_location.X;
		}
		panel1.AutoScrollPosition = scroll_location;
	}

	public static bool Is_Subchart_Name(string s)
	{
		return form.Is_Subchart_Name(s);
	}

	public static int Parameter_Count(string s)
	{
		return form.Find_Tab(s).num_params;
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		base.ResumeLayout(false);
	}

	private void carlisle_DragEnter(object sender, DragEventArgs e)
	{
		droppable_icon = false;
		droppable_file = false;
		if (e.Data.GetDataPresent(DataFormats.Text))
		{
			string text = (string)e.Data.GetData(DataFormats.Text);
			switch (text)
			{
			case "raptor_ASGN":
			case "raptor_CALL":
			case "raptor_INPUT":
			case "raptor_OUTPUT":
			case "raptor_SELECTION":
			case "raptor_LOOP":
			case "raptor_RETURN":
				droppable_icon = true;
				have_cut_drag = true;
				break;
			}
			if (text == "raptor_DRAG" && !have_drag)
			{
				have_drag = true;
			}
		}
		else if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			droppable_file = true;
		}
	}

	private void form_GiveFeedback(object sender, GiveFeedbackEventArgs e)
	{
		carlisle_GiveFeedback(sender, e);
	}

	private void carlisle_GiveFeedback(object sender, GiveFeedbackEventArgs e)
	{
		if (e.Effect == DragDropEffects.Link)
		{
			e.UseDefaultCursors = false;
			System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Help;
		}
		else
		{
			e.UseDefaultCursors = true;
		}
	}

	private void carlisle_DragOver(object sender, DragEventArgs e)
	{
		if (droppable_file)
		{
			e.Effect = DragDropEffects.Copy;
			return;
		}
		Point point = panel1.PointToClient(new Point(e.X, e.Y));
		int num = point.X + Math.Abs(panel1.AutoScrollPosition.X);
		int num2 = point.Y + Math.Abs(panel1.AutoScrollPosition.Y);
		if (droppable_icon && Start.insert(null, num, num2, 0))
		{
			e.Effect = DragDropEffects.Scroll | DragDropEffects.Copy;
		}
		else if (have_drag)
		{
			if (!have_cut_drag && (Math.Abs(num - drag_x) > 5 || Math.Abs(num2 - drag_y) > 5))
			{
				form.Cut_Click(sender, e);
				have_cut_drag = true;
			}
			if (Start.insert(null, point.X + Math.Abs(panel1.AutoScrollPosition.X), point.Y + Math.Abs(panel1.AutoScrollPosition.Y), 0))
			{
				e.Effect = DragDropEffects.Move;
			}
			else
			{
				e.Effect = DragDropEffects.Link;
			}
		}
		else
		{
			e.Effect = DragDropEffects.Link;
		}
	}

	private void carlisle_DragDrop(object sender, DragEventArgs e)
	{
		if (e.Effect == DragDropEffects.Link && have_drag && have_cut_drag)
		{
			have_drag = false;
			have_cut_drag = false;
			form.Undo_Click(sender, e);
			return;
		}
		if (droppable_file)
		{
			string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
			form.Load_MRU(array[0]);
		}
		e.Effect = DragDropEffects.Copy;
		Point point = panel1.PointToClient(new Point(e.X, e.Y));
		if ((droppable_icon || have_drag) && Start.insert(null, point.X + Math.Abs(panel1.AutoScrollPosition.X), point.Y + Math.Abs(panel1.AutoScrollPosition.Y), 0))
		{
			form.mouse_x = point.X + Math.Abs(panel1.AutoScrollPosition.X);
			form.mouse_y = point.Y + Math.Abs(panel1.AutoScrollPosition.Y);
			switch ((string)e.Data.GetData(DataFormats.Text))
			{
			case "raptor_ASGN":
				form.menuItemAssignment_Click(sender, null);
				break;
			case "raptor_CALL":
				form.menuItemCall_Click(sender, null);
				break;
			case "raptor_INPUT":
				form.menuItemParallelogram_Click(sender, null);
				break;
			case "raptor_OUTPUT":
				form.menuItemOutput_Click(sender, null);
				break;
			case "raptor_SELECTION":
				form.menuItemIf_Click(sender, null);
				break;
			case "raptor_LOOP":
				form.menuItemLoop_Click(sender, null);
				break;
			case "raptor_RETURN":
				form.menuItemReturn_Click(sender, null);
				break;
			case "raptor_DRAG":
				form.paste_Click(null, null);
				break;
			}
		}
		have_drag = false;
	}
}
