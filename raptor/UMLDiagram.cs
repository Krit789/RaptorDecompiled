using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NClass.Core;
using NClass.CSharp;
using NClass.GUI;
using NClass.GUI.Diagram;
using NClass.Java;
using NClass.Translations;
using raptor.Properties;

namespace raptor;

public class UMLDiagram : UserControl
{
	public Project project;

	public DiagramControl diagram;

	private OptionsDialog optionsDialog;

	private List<IProjectPlugin> projectPlugins = new List<IProjectPlugin>();

	private List<IDiagramPlugin> diagramPlugins = new List<IDiagramPlugin>();

	private static readonly Dictionary<Language, AccessModifier[]> accessOrders;

	private static readonly Dictionary<Language, ClassModifier[]> modifierOrders;

	private IContainer components;

	private ToolStripContainer toolStripContainer;

	private ToolStrip elementsToolStrip;

	private ToolStripButton toolNewClass;

	private ToolStripButton toolNewStruct;

	private ToolStripButton toolNewInterface;

	private ToolStripButton toolNewEnum;

	private ToolStripButton toolNewComment;

	private ToolStripSeparator toolSepEntities;

	private ToolStripButton toolNewAssociation;

	private ToolStripButton toolNewComposition;

	private ToolStripButton toolNewAggregation;

	private ToolStripButton toolNewGeneralization;

	private ToolStripButton toolNewRealization;

	private ToolStripButton toolNewDependency;

	private ToolStripButton toolNewNesting;

	private ToolStripSeparator toolSepRelations;

	private ToolStripButton toolDelete;

	private ToolStrip standardToolStrip;

	private ToolStripLabel lblName;

	private ToolStripTextBox txtName;

	private ToolStripLabel lblAccess;

	private ToolStripComboBox cboAccess;

	private ToolStrip zoomToolStrip;

	private ToolStripLabel toolZoomValue;

	private ToolStripButton toolZoomOut;

	private ToolStripButton toolZoomIn;

	private ToolStripButton toolAutoZoom;

	private ZoomingToolStrip toolZoom;

	private ToolStripComboBox cboModifier;

	private ToolStripLabel lblModifier;

	private ToolStripSeparator toolStripSeparator1;

	public UMLDiagram(UMLupdater umlupdater)
	{
		InitializeComponent();
		Dock = DockStyle.Fill;
		project = new Project(NClass.GUI.Settings.DefaultLanguage);
		ProjectCore.raptorUpdater = umlupdater;
		diagram = new DiagramControl(project);
		diagram.Dock = DockStyle.Fill;
		diagram.ZoomChanged += diagram_ZoomChanged;
		toolStripContainer.ContentPanel.Controls.Add(diagram);
		optionsDialog = new OptionsDialog();
		project.FileStateChanged += project_FileStateChanged;
		project.LanguageChanged += delegate
		{
			UpdateLanguageChanges();
		};
		project.RelationAdded += project_RelationAdded;
		diagram.SelectionChanged += diagram_SelectionChanged;
		optionsDialog.Applied += optionsDialog_Apply;
		optionsDialog.CurrentStyleChanged += optionsDialog_CurrentStyleChanged;
		Strings.LanguageChanged += Strings_LanguageChanged;
		UpdateTexts();
		UpdateLanguageChanges();
	}

	static UMLDiagram()
	{
		accessOrders = new Dictionary<Language, AccessModifier[]>(3);
		modifierOrders = new Dictionary<Language, ClassModifier[]>(3);
		accessOrders.Add(CSharpLanguage.Instance, new AccessModifier[6]
		{
			AccessModifier.Public,
			AccessModifier.ProtectedInternal,
			AccessModifier.Internal,
			AccessModifier.Protected,
			AccessModifier.Private,
			AccessModifier.Default
		});
		accessOrders.Add(JavaLanguage.Instance, new AccessModifier[4]
		{
			AccessModifier.Public,
			AccessModifier.Protected,
			AccessModifier.Private,
			AccessModifier.Default
		});
		modifierOrders.Add(CSharpLanguage.Instance, new ClassModifier[4]
		{
			ClassModifier.None,
			ClassModifier.Abstract,
			ClassModifier.Sealed,
			ClassModifier.Static
		});
		modifierOrders.Add(JavaLanguage.Instance, new ClassModifier[4]
		{
			ClassModifier.None,
			ClassModifier.Abstract,
			ClassModifier.Sealed,
			ClassModifier.Static
		});
	}

	private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
	{
	}

	private void UpdateTexts()
	{
		toolZoomIn.Text = Strings.GetString("zoom_in");
		toolZoomOut.Text = Strings.GetString("zoom_out");
		toolAutoZoom.Text = Strings.GetString("auto_zoom");
		toolNewClass.Text = Strings.GetString("add_new_class");
		toolNewStruct.Text = Strings.GetString("add_new_struct");
		toolNewInterface.Text = Strings.GetString("add_new_interface");
		toolNewEnum.Text = Strings.GetString("add_new_enum");
		toolNewComment.Text = Strings.GetString("add_new_comment");
		toolNewAssociation.Text = Strings.GetString("add_new_association");
		toolNewComposition.Text = Strings.GetString("add_new_composition");
		toolNewAggregation.Text = Strings.GetString("add_new_aggregation");
		toolNewGeneralization.Text = Strings.GetString("add_new_generalization");
		toolNewRealization.Text = Strings.GetString("add_new_realization");
		toolNewDependency.Text = Strings.GetString("add_new_dependency");
		toolNewNesting.Text = Strings.GetString("add_new_nesting");
		toolDelete.Text = Strings.GetString("delete_selected_items");
		lblName.Text = Strings.GetString("name:");
		lblAccess.Text = Strings.GetString("access:");
		lblModifier.Text = Strings.GetString("modifier:");
	}

	private void LoadProject(string fileName)
	{
		try
		{
			Cursor = Cursors.WaitCursor;
			SuspendLayout();
			project.Load(fileName);
		}
		catch (Exception ex)
		{
			MessageBox.Show(Strings.GetString("error") + ": " + ex.Message, Strings.GetString("load"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		finally
		{
			ResumeLayout();
			Cursor = Cursors.Default;
		}
	}

	private Control PanelFromPosition(DockStyle dockStyle)
	{
		return dockStyle switch
		{
			DockStyle.Left => toolStripContainer.LeftToolStripPanel, 
			DockStyle.Right => toolStripContainer.RightToolStripPanel, 
			DockStyle.Bottom => toolStripContainer.BottomToolStripPanel, 
			_ => toolStripContainer.TopToolStripPanel, 
		};
	}

	private DockStyle PositionFromPanel(Control control)
	{
		if (control == toolStripContainer.LeftToolStripPanel)
		{
			return DockStyle.Left;
		}
		if (control == toolStripContainer.RightToolStripPanel)
		{
			return DockStyle.Right;
		}
		if (control == toolStripContainer.BottomToolStripPanel)
		{
			return DockStyle.Bottom;
		}
		return DockStyle.Top;
	}

	protected override void OnDragEnter(DragEventArgs e)
	{
		base.OnDragEnter(e);
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.Copy;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	protected override void OnDragDrop(DragEventArgs e)
	{
		base.OnDragDrop(e);
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (array.Length != 0)
			{
				LoadProject(array[0]);
			}
		}
	}

	private void UpdateWindow()
	{
		diagram.RefreshDiagram();
	}

	private void UpdateModifiersToolBar()
	{
		int selectedShapeCount = diagram.SelectedShapeCount;
		DiagramElement firstSelectedElement = diagram.FirstSelectedElement;
		if (selectedShapeCount == 1 && firstSelectedElement is TypeShape)
		{
			TypeBase typeBase = ((TypeShape)firstSelectedElement).TypeBase;
			txtName.Text = typeBase.Name;
			txtName.Enabled = true;
			SetAccessLabel(typeBase.AccessModifier);
			cboAccess.Enabled = true;
			if (typeBase is ClassType)
			{
				lblModifier.Text = Strings.GetString("modifier:");
				SetModifierLabel(((ClassType)typeBase).Modifier);
				cboModifier.Enabled = true;
				cboModifier.Visible = true;
				lblModifier.Visible = true;
			}
			else if (typeBase is DelegateType)
			{
				cboModifier.Visible = false;
				lblModifier.Visible = false;
			}
			else
			{
				lblModifier.Text = Strings.GetString("modifier:");
				cboModifier.Text = null;
				cboModifier.Enabled = false;
				cboModifier.Visible = true;
				lblModifier.Visible = true;
			}
		}
		else
		{
			txtName.Text = null;
			txtName.Enabled = false;
			cboAccess.Text = null;
			cboAccess.Enabled = false;
			cboModifier.Text = null;
			cboModifier.Enabled = false;
			cboModifier.Visible = true;
			lblModifier.Visible = true;
		}
	}

	private void SetAccessLabel(AccessModifier access)
	{
		AccessModifier[] array = accessOrders[project.Language];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == access)
			{
				cboAccess.SelectedIndex = i;
				return;
			}
		}
		cboAccess.SelectedIndex = 0;
	}

	private void SetModifierLabel(ClassModifier modifier)
	{
		ClassModifier[] array = modifierOrders[project.Language];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == modifier)
			{
				cboModifier.SelectedIndex = i;
				return;
			}
		}
		cboModifier.SelectedIndex = 0;
	}

	private void UpdateLanguageChanges()
	{
		NClass.GUI.Settings.DefaultLanguage = project.Language;
		toolNewInterface.Visible = project.Language.SupportsInterfaces;
		toolNewStruct.Visible = project.Language.SupportsStructures;
		toolNewEnum.Visible = project.Language.SupportsEnums;
		UpdateComboBoxes(project.Language);
	}

	private void UpdateComboBoxes(Language language)
	{
		cboAccess.Items.Clear();
		cboModifier.Items.Clear();
		if (language == CSharpLanguage.Instance)
		{
			cboAccess.Items.Add("Public");
			cboAccess.Items.Add("Protected Internal");
			cboAccess.Items.Add("Internal");
			cboAccess.Items.Add("Protected");
			cboAccess.Items.Add("Private");
			cboAccess.Items.Add("Default");
			cboModifier.Items.Add("None");
			cboModifier.Items.Add("Abstract");
			cboModifier.Items.Add("Sealed");
			cboModifier.Items.Add("Static");
		}
		else
		{
			cboAccess.Items.Add("Public");
			cboAccess.Items.Add("Protected");
			cboAccess.Items.Add("Private");
			cboAccess.Items.Add("Default");
			cboModifier.Items.Add("None");
			cboModifier.Items.Add("Abstract");
			cboModifier.Items.Add("Sealed");
			cboModifier.Items.Add("Static");
		}
	}

	private void project_FileStateChanged(object sender, EventArgs e)
	{
		if (!project.IsUntitled)
		{
			NClass.GUI.Settings.AddRecentFile(project.ProjectFile);
		}
	}

	private void project_RelationAdded(object sender, RelationEventArgs e)
	{
		toolNewAssociation.Checked = false;
		toolNewComposition.Checked = false;
		toolNewAggregation.Checked = false;
		toolNewGeneralization.Checked = false;
		toolNewRealization.Checked = false;
		toolNewDependency.Checked = false;
		toolNewNesting.Checked = false;
	}

	private void diagram_SelectionChanged(object sender, EventArgs e)
	{
		UpdateModifiersToolBar();
		toolDelete.Enabled = diagram.HasSelectedElement;
	}

	private void Strings_LanguageChanged(object sender, EventArgs e)
	{
		UpdateTexts();
	}

	private void OpenRecentFile_Click(object sender, EventArgs e)
	{
		if (sender is ToolStripItem && ((ToolStripItem)sender).Tag is int)
		{
			int num = (int)((ToolStripItem)sender).Tag;
			if (num >= 0 && num < NClass.GUI.Settings.RecentFileCount)
			{
				LoadProject(NClass.GUI.Settings.GetRecentFile(num));
			}
		}
	}

	private void optionsDialog_Apply(object sender, EventArgs e)
	{
		UpdateWindow();
	}

	private void optionsDialog_CurrentStyleChanged(object sender, EventArgs e)
	{
		diagram.RefreshDiagram();
	}

	private void diagram_ZoomChanged(object sender, EventArgs e)
	{
		toolZoomValue.Text = diagram.ZoomPercentage + "%";
		toolZoom.ZoomValue = diagram.Zoom;
	}

	private void toolZoomIn_Click(object sender, EventArgs e)
	{
		diagram.ChangeZoom(enlarge: true);
	}

	private void toolZoomOut_Click(object sender, EventArgs e)
	{
		diagram.ChangeZoom(enlarge: false);
	}

	private void toolZoom_ZoomValueChanged(object sender, EventArgs e)
	{
		diagram.ChangeZoom(toolZoom.ZoomValue);
	}

	private void mnuNewClass_Click(object sender, EventArgs e)
	{
		project.AddClass();
	}

	private void mnuNewStructure_Click(object sender, EventArgs e)
	{
		if (project.Language.SupportsStructures)
		{
			project.AddStructure();
		}
	}

	private void mnuNewInterface_Click(object sender, EventArgs e)
	{
		if (project.Language.SupportsInterfaces)
		{
			project.AddInterface();
		}
	}

	private void mnuNewEnum_Click(object sender, EventArgs e)
	{
		if (project.Language.SupportsEnums)
		{
			project.AddEnum();
		}
	}

	private void mnuNewDelegate_Click(object sender, EventArgs e)
	{
		if (project.Language.SupportsDelegates)
		{
			project.AddDelegate();
		}
	}

	private void mnuNewComment_Click(object sender, EventArgs e)
	{
		project.AddComment();
	}

	private void mnuNewAssociation_Click(object sender, EventArgs e)
	{
		diagram.CreateNewAssociation();
		toolNewAssociation.Checked = true;
	}

	private void mnuNewComposition_Click(object sender, EventArgs e)
	{
		diagram.CreateNewComposition();
		toolNewComposition.Checked = true;
	}

	private void mnuNewAggregation_Click(object sender, EventArgs e)
	{
		diagram.CreateNewAggregation();
		toolNewAggregation.Checked = true;
	}

	private void mnuNewGeneralization_Click(object sender, EventArgs e)
	{
		diagram.CreateNewGeneralization();
		toolNewGeneralization.Checked = true;
	}

	private void mnuNewRealization_Click(object sender, EventArgs e)
	{
		diagram.CreateNewRealization();
		toolNewRealization.Checked = true;
	}

	private void mnuNewDependency_Click(object sender, EventArgs e)
	{
		diagram.CreateNewDependency();
		toolNewDependency.Checked = true;
	}

	private void mnuNewNesting_Click(object sender, EventArgs e)
	{
		diagram.CreateNewNesting();
		toolNewNesting.Checked = true;
	}

	private void mnuNewCommentRelation_Click(object sender, EventArgs e)
	{
		diagram.CreateNewCommentRelationsip();
	}

	private void mnuAutoZoom_Click(object sender, EventArgs e)
	{
		diagram.AutoZoom();
	}

	private void mnuDiagramSize_Click(object sender, EventArgs e)
	{
		using DiagramSizeDialog diagramSizeDialog = new DiagramSizeDialog(diagram.DiagramSize, diagram.GetMinimumDiagramSize());
		if (diagramSizeDialog.ShowDialog() == DialogResult.OK)
		{
			diagram.DiagramSize = diagramSizeDialog.DiagramSize;
		}
	}

	private void txtName_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '<')
		{
			int selectionStart = txtName.SelectionStart;
			if (selectionStart == txtName.Text.Length)
			{
				txtName.Text = txtName.Text.Insert(selectionStart, "<T>");
				txtName.SelectionStart = selectionStart + 1;
				txtName.SelectionLength = 1;
				e.Handled = true;
			}
		}
	}

	private void txtName_TextChanged(object sender, EventArgs e)
	{
		if (!diagram.SingleSelection || !(diagram.FirstSelectedElement is TypeShape))
		{
			return;
		}
		TypeBase typeBase = ((TypeShape)diagram.FirstSelectedElement).TypeBase;
		if (typeBase.Name != txtName.Text)
		{
			try
			{
				typeBase.Name = txtName.Text;
				txtName.ForeColor = SystemColors.WindowText;
				if (typeBase is ClassType)
				{
					ClassType classType = typeBase as ClassType;
					ProjectCore.raptorUpdater.renameClass(classType.raptorTab, typeBase.Name);
					{
						foreach (Operation operation in classType.Operations)
						{
							if (operation is Constructor)
							{
								ProjectCore.raptorUpdater.renameMethod(classType, (operation as Constructor).raptorTab, typeBase.Name);
							}
						}
						return;
					}
				}
				return;
			}
			catch (BadSyntaxException)
			{
				txtName.ForeColor = Color.Red;
				return;
			}
		}
		txtName.ForeColor = SystemColors.WindowText;
	}

	private void txtName_Validated(object sender, EventArgs e)
	{
		txtName.ForeColor = SystemColors.WindowText;
	}

	private void cboAccess_SelectedIndexChanged(object sender, EventArgs e)
	{
		DiagramElement firstSelectedElement = diagram.FirstSelectedElement;
		if (diagram.SingleSelection && firstSelectedElement is TypeShape)
		{
			TypeBase typeBase = ((TypeShape)firstSelectedElement).TypeBase;
			int selectedIndex = cboAccess.SelectedIndex;
			typeBase.AccessModifier = accessOrders[typeBase.Language][selectedIndex];
		}
	}

	private void cboModifier_SelectedIndexChanged(object sender, EventArgs e)
	{
		DiagramElement firstSelectedElement = diagram.FirstSelectedElement;
		if (diagram.SingleSelection && firstSelectedElement is ClassShape)
		{
			ClassType classType = ((ClassShape)firstSelectedElement).ClassType;
			int selectedIndex = cboModifier.SelectedIndex;
			classType.Modifier = modifierOrders[classType.Language][selectedIndex];
		}
	}

	private void txtReturnType_TextChanged(object sender, EventArgs e)
	{
		_ = diagram.FirstSelectedElement;
	}

	private void txtReturnType_Validated(object sender, EventArgs e)
	{
	}

	public void mnuDelete_Click(object sender, EventArgs e)
	{
		diagram.DeleteSelectedElements();
	}

	public void mnuSelectAll_Click(object sender, EventArgs e)
	{
		diagram.SelectAll();
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
		this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
		this.standardToolStrip = new System.Windows.Forms.ToolStrip();
		this.lblName = new System.Windows.Forms.ToolStripLabel();
		this.txtName = new System.Windows.Forms.ToolStripTextBox();
		this.lblAccess = new System.Windows.Forms.ToolStripLabel();
		this.cboAccess = new System.Windows.Forms.ToolStripComboBox();
		this.lblModifier = new System.Windows.Forms.ToolStripLabel();
		this.cboModifier = new System.Windows.Forms.ToolStripComboBox();
		this.elementsToolStrip = new System.Windows.Forms.ToolStrip();
		this.toolNewClass = new System.Windows.Forms.ToolStripButton();
		this.toolNewStruct = new System.Windows.Forms.ToolStripButton();
		this.toolNewInterface = new System.Windows.Forms.ToolStripButton();
		this.toolNewEnum = new System.Windows.Forms.ToolStripButton();
		this.toolNewComment = new System.Windows.Forms.ToolStripButton();
		this.toolSepEntities = new System.Windows.Forms.ToolStripSeparator();
		this.toolNewGeneralization = new System.Windows.Forms.ToolStripButton();
		this.toolNewRealization = new System.Windows.Forms.ToolStripButton();
		this.toolNewNesting = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.toolNewAssociation = new System.Windows.Forms.ToolStripButton();
		this.toolNewComposition = new System.Windows.Forms.ToolStripButton();
		this.toolNewAggregation = new System.Windows.Forms.ToolStripButton();
		this.toolNewDependency = new System.Windows.Forms.ToolStripButton();
		this.toolSepRelations = new System.Windows.Forms.ToolStripSeparator();
		this.toolDelete = new System.Windows.Forms.ToolStripButton();
		this.zoomToolStrip = new System.Windows.Forms.ToolStrip();
		this.toolZoomValue = new System.Windows.Forms.ToolStripLabel();
		this.toolZoomOut = new System.Windows.Forms.ToolStripButton();
		this.toolZoom = new NClass.GUI.ZoomingToolStrip();
		this.toolZoomIn = new System.Windows.Forms.ToolStripButton();
		this.toolAutoZoom = new System.Windows.Forms.ToolStripButton();
		this.toolStripContainer.TopToolStripPanel.SuspendLayout();
		this.toolStripContainer.SuspendLayout();
		this.standardToolStrip.SuspendLayout();
		this.elementsToolStrip.SuspendLayout();
		this.zoomToolStrip.SuspendLayout();
		base.SuspendLayout();
		this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(544, 241);
		this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
		this.toolStripContainer.Name = "toolStripContainer";
		this.toolStripContainer.Size = new System.Drawing.Size(544, 316);
		this.toolStripContainer.TabIndex = 0;
		this.toolStripContainer.Text = "toolStripContainer1";
		this.toolStripContainer.TopToolStripPanel.Controls.Add(this.standardToolStrip);
		this.toolStripContainer.TopToolStripPanel.Controls.Add(this.elementsToolStrip);
		this.toolStripContainer.TopToolStripPanel.Controls.Add(this.zoomToolStrip);
		this.toolStripContainer.TopToolStripPanel.Click += new System.EventHandler(toolStripContainer1_TopToolStripPanel_Click);
		this.standardToolStrip.Dock = System.Windows.Forms.DockStyle.None;
		this.standardToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.lblName, this.txtName, this.lblAccess, this.cboAccess, this.lblModifier, this.cboModifier });
		this.standardToolStrip.Location = new System.Drawing.Point(3, 0);
		this.standardToolStrip.Name = "standardToolStrip";
		this.standardToolStrip.Size = new System.Drawing.Size(460, 25);
		this.standardToolStrip.TabIndex = 7;
		this.lblName.Name = "lblName";
		this.lblName.Size = new System.Drawing.Size(38, 22);
		this.lblName.Text = "Name:";
		this.txtName.Enabled = false;
		this.txtName.Name = "txtName";
		this.txtName.Size = new System.Drawing.Size(120, 25);
		this.txtName.Validated += new System.EventHandler(txtName_Validated);
		this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txtName_KeyPress);
		this.txtName.TextChanged += new System.EventHandler(txtName_TextChanged);
		this.lblAccess.Name = "lblAccess";
		this.lblAccess.Size = new System.Drawing.Size(40, 22);
		this.lblAccess.Text = "Access";
		this.cboAccess.Enabled = false;
		this.cboAccess.Name = "cboAccess";
		this.cboAccess.Size = new System.Drawing.Size(81, 25);
		this.cboAccess.SelectedIndexChanged += new System.EventHandler(cboAccess_SelectedIndexChanged);
		this.lblModifier.Name = "lblModifier";
		this.lblModifier.Size = new System.Drawing.Size(49, 22);
		this.lblModifier.Text = "Modifier:";
		this.cboModifier.Enabled = false;
		this.cboModifier.Name = "cboModifier";
		this.cboModifier.Size = new System.Drawing.Size(81, 25);
		this.cboModifier.SelectedIndexChanged += new System.EventHandler(cboModifier_SelectedIndexChanged);
		this.elementsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
		this.elementsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[16]
		{
			this.toolNewClass, this.toolNewStruct, this.toolNewInterface, this.toolNewEnum, this.toolNewComment, this.toolSepEntities, this.toolNewGeneralization, this.toolNewRealization, this.toolNewNesting, this.toolStripSeparator1,
			this.toolNewAssociation, this.toolNewComposition, this.toolNewAggregation, this.toolNewDependency, this.toolSepRelations, this.toolDelete
		});
		this.elementsToolStrip.Location = new System.Drawing.Point(3, 25);
		this.elementsToolStrip.Name = "elementsToolStrip";
		this.elementsToolStrip.Size = new System.Drawing.Size(329, 25);
		this.elementsToolStrip.TabIndex = 6;
		this.toolNewClass.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewClass.Image = raptor.Properties.Resources.class1;
		this.toolNewClass.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewClass.Name = "toolNewClass";
		this.toolNewClass.Size = new System.Drawing.Size(23, 22);
		this.toolNewClass.Click += new System.EventHandler(mnuNewClass_Click);
		this.toolNewStruct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewStruct.Image = raptor.Properties.Resources.struct1;
		this.toolNewStruct.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewStruct.Name = "toolNewStruct";
		this.toolNewStruct.Size = new System.Drawing.Size(23, 22);
		this.toolNewStruct.Click += new System.EventHandler(mnuNewStructure_Click);
		this.toolNewInterface.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewInterface.Image = raptor.Properties.Resources.interface1;
		this.toolNewInterface.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewInterface.Name = "toolNewInterface";
		this.toolNewInterface.Size = new System.Drawing.Size(23, 22);
		this.toolNewInterface.Click += new System.EventHandler(mnuNewInterface_Click);
		this.toolNewEnum.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewEnum.Image = raptor.Properties.Resources.enum1;
		this.toolNewEnum.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewEnum.Name = "toolNewEnum";
		this.toolNewEnum.Size = new System.Drawing.Size(23, 22);
		this.toolNewEnum.Click += new System.EventHandler(mnuNewEnum_Click);
		this.toolNewComment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewComment.Image = raptor.Properties.Resources.comment1;
		this.toolNewComment.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewComment.Name = "toolNewComment";
		this.toolNewComment.Size = new System.Drawing.Size(23, 22);
		this.toolNewComment.Click += new System.EventHandler(mnuNewComment_Click);
		this.toolSepEntities.Name = "toolSepEntities";
		this.toolSepEntities.Size = new System.Drawing.Size(6, 25);
		this.toolNewGeneralization.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewGeneralization.Image = raptor.Properties.Resources.generalization1;
		this.toolNewGeneralization.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewGeneralization.Name = "toolNewGeneralization";
		this.toolNewGeneralization.Size = new System.Drawing.Size(23, 22);
		this.toolNewGeneralization.Click += new System.EventHandler(mnuNewGeneralization_Click);
		this.toolNewRealization.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewRealization.Image = raptor.Properties.Resources.realization1;
		this.toolNewRealization.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewRealization.Name = "toolNewRealization";
		this.toolNewRealization.Size = new System.Drawing.Size(23, 22);
		this.toolNewRealization.Click += new System.EventHandler(mnuNewRealization_Click);
		this.toolNewNesting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewNesting.Image = raptor.Properties.Resources.nesting1;
		this.toolNewNesting.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewNesting.Name = "toolNewNesting";
		this.toolNewNesting.Size = new System.Drawing.Size(23, 22);
		this.toolNewNesting.Click += new System.EventHandler(mnuNewNesting_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
		this.toolNewAssociation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewAssociation.Image = raptor.Properties.Resources.association1;
		this.toolNewAssociation.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewAssociation.Name = "toolNewAssociation";
		this.toolNewAssociation.Size = new System.Drawing.Size(23, 22);
		this.toolNewAssociation.Click += new System.EventHandler(mnuNewAssociation_Click);
		this.toolNewComposition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewComposition.Image = raptor.Properties.Resources.composition1;
		this.toolNewComposition.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewComposition.Name = "toolNewComposition";
		this.toolNewComposition.Size = new System.Drawing.Size(23, 22);
		this.toolNewAggregation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewAggregation.Image = raptor.Properties.Resources.aggregation1;
		this.toolNewAggregation.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewAggregation.Name = "toolNewAggregation";
		this.toolNewAggregation.Size = new System.Drawing.Size(23, 22);
		this.toolNewAggregation.Click += new System.EventHandler(mnuNewAggregation_Click);
		this.toolNewDependency.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolNewDependency.Image = raptor.Properties.Resources.dependency1;
		this.toolNewDependency.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolNewDependency.Name = "toolNewDependency";
		this.toolNewDependency.Size = new System.Drawing.Size(23, 22);
		this.toolNewDependency.Click += new System.EventHandler(mnuNewDependency_Click);
		this.toolSepRelations.Name = "toolSepRelations";
		this.toolSepRelations.Size = new System.Drawing.Size(6, 25);
		this.toolDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolDelete.Enabled = false;
		this.toolDelete.Image = raptor.Properties.Resources.delete1;
		this.toolDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolDelete.Name = "toolDelete";
		this.toolDelete.Size = new System.Drawing.Size(23, 22);
		this.toolDelete.Click += new System.EventHandler(mnuDelete_Click);
		this.zoomToolStrip.Dock = System.Windows.Forms.DockStyle.None;
		this.zoomToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.toolZoomValue, this.toolZoomOut, this.toolZoom, this.toolZoomIn, this.toolAutoZoom });
		this.zoomToolStrip.Location = new System.Drawing.Point(3, 50);
		this.zoomToolStrip.Name = "zoomToolStrip";
		this.zoomToolStrip.Size = new System.Drawing.Size(217, 25);
		this.zoomToolStrip.TabIndex = 8;
		this.toolZoomValue.AutoSize = false;
		this.toolZoomValue.Name = "toolZoomValue";
		this.toolZoomValue.Size = new System.Drawing.Size(36, 22);
		this.toolZoomValue.Text = "100%";
		this.toolZoomValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.toolZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolZoomOut.Image = raptor.Properties.Resources.ZoomOut;
		this.toolZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolZoomOut.Name = "toolZoomOut";
		this.toolZoomOut.Size = new System.Drawing.Size(23, 22);
		this.toolZoomOut.Click += new System.EventHandler(toolZoomOut_Click);
		this.toolZoom.Name = "toolZoom";
		this.toolZoom.Size = new System.Drawing.Size(100, 22);
		this.toolZoom.ZoomValue = 1f;
		this.toolZoom.ZoomValueChanged += new System.EventHandler(toolZoom_ZoomValueChanged);
		this.toolZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolZoomIn.Image = raptor.Properties.Resources.ZoomIn;
		this.toolZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolZoomIn.Name = "toolZoomIn";
		this.toolZoomIn.Size = new System.Drawing.Size(23, 22);
		this.toolZoomIn.Click += new System.EventHandler(toolZoomIn_Click);
		this.toolAutoZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolAutoZoom.Image = raptor.Properties.Resources.AutoZoom;
		this.toolAutoZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.toolAutoZoom.Name = "toolAutoZoom";
		this.toolAutoZoom.Size = new System.Drawing.Size(23, 22);
		this.toolAutoZoom.Click += new System.EventHandler(mnuAutoZoom_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.toolStripContainer);
		base.Name = "UMLDiagram";
		base.Size = new System.Drawing.Size(544, 316);
		this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
		this.toolStripContainer.TopToolStripPanel.PerformLayout();
		this.toolStripContainer.ResumeLayout(false);
		this.toolStripContainer.PerformLayout();
		this.standardToolStrip.ResumeLayout(false);
		this.standardToolStrip.PerformLayout();
		this.elementsToolStrip.ResumeLayout(false);
		this.elementsToolStrip.PerformLayout();
		this.zoomToolStrip.ResumeLayout(false);
		this.zoomToolStrip.PerformLayout();
		base.ResumeLayout(false);
	}
}
