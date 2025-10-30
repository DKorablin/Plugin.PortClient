namespace Plugin.PortClient
{
	partial class PanelPortTestClient
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStrip tsMain;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelPortTestClient));
			this.tsbnServerAdd = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiAddGroup = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiAddServer = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiAddPort = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbnProjectSave = new System.Windows.Forms.ToolStripButton();
			this.tsddlProject = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiProjectLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddlProjectSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiProjectExport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectImport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbnLaunch = new System.Windows.Forms.ToolStripButton();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.gridSearch = new AlphaOmega.Windows.Forms.SearchGrid();
			this.lvServers = new Plugin.PortClient.UI.ServerListView();
			this.cmsNodes = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiNodesLaunch = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiNodesAddGroup = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiNodesAddServer = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiNodesAddPort = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiNodesCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiNodesDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiNodesProperties = new System.Windows.Forms.ToolStripMenuItem();
			this.ilServers = new System.Windows.Forms.ImageList(this.components);
			this.pgProperties = new System.Windows.Forms.PropertyGrid();
			tsMain = new System.Windows.Forms.ToolStrip();
			tsMain.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lvServers)).BeginInit();
			this.cmsNodes.SuspendLayout();
			this.SuspendLayout();
			// 
			// tsMain
			// 
			tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnServerAdd,
            this.tsbnProjectSave,
            this.tsddlProject,
            this.tsbnLaunch});
			tsMain.Location = new System.Drawing.Point(0, 0);
			tsMain.Name = "tsMain";
			tsMain.Size = new System.Drawing.Size(200, 27);
			tsMain.TabIndex = 0;
			// 
			// tsbnServerAdd
			// 
			this.tsbnServerAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnServerAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddGroup,
            this.tsmiAddServer,
            this.tsmiAddPort});
			this.tsbnServerAdd.Image = global::Plugin.PortClient.Properties.Resources.FileNew;
			this.tsbnServerAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnServerAdd.Name = "tsbnServerAdd";
			this.tsbnServerAdd.Size = new System.Drawing.Size(34, 24);
			this.tsbnServerAdd.Text = "Add";
			this.tsbnServerAdd.DropDownOpening += new System.EventHandler(this.tsbnServerAdd_DropDownOpening);
			this.tsbnServerAdd.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsbnServerAdd_DropDownItemClicked);
			// 
			// tsmiAddGroup
			// 
			this.tsmiAddGroup.Name = "tsmiAddGroup";
			this.tsmiAddGroup.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.G)));
			this.tsmiAddGroup.Size = new System.Drawing.Size(181, 26);
			this.tsmiAddGroup.Text = "&Group";
			// 
			// tsmiAddServer
			// 
			this.tsmiAddServer.Name = "tsmiAddServer";
			this.tsmiAddServer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
			this.tsmiAddServer.Size = new System.Drawing.Size(181, 26);
			this.tsmiAddServer.Text = "&Server";
			// 
			// tsmiAddPort
			// 
			this.tsmiAddPort.Name = "tsmiAddPort";
			this.tsmiAddPort.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
			this.tsmiAddPort.Size = new System.Drawing.Size(181, 26);
			this.tsmiAddPort.Text = "&Port";
			// 
			// tsbnProjectSave
			// 
			this.tsbnProjectSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnProjectSave.Image = global::Plugin.PortClient.Properties.Resources.FileSave;
			this.tsbnProjectSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnProjectSave.Name = "tsbnProjectSave";
			this.tsbnProjectSave.Size = new System.Drawing.Size(29, 24);
			this.tsbnProjectSave.Text = "Save... (Ctrl+S)";
			this.tsbnProjectSave.Click += new System.EventHandler(this.tsbnProjectSave_Click);
			// 
			// tsddlProject
			// 
			this.tsddlProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsddlProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiProjectLoad,
            this.tsddlProjectSeparator,
            this.tsmiProjectExport,
            this.tsmiProjectImport});
			this.tsddlProject.Image = global::Plugin.PortClient.Properties.Resources.iconOpen;
			this.tsddlProject.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddlProject.Name = "tsddlProject";
			this.tsddlProject.Size = new System.Drawing.Size(34, 24);
			this.tsddlProject.ToolTipText = "Project";
			this.tsddlProject.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsddlProject_DropDownItemClicked);
			// 
			// tsmiProjectLoad
			// 
			this.tsmiProjectLoad.Image = global::Plugin.PortClient.Properties.Resources.iconOpen;
			this.tsmiProjectLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsmiProjectLoad.Name = "tsmiProjectLoad";
			this.tsmiProjectLoad.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.tsmiProjectLoad.Size = new System.Drawing.Size(190, 26);
			this.tsmiProjectLoad.Text = "&Open...";
			// 
			// tsddlProjectSeparator
			// 
			this.tsddlProjectSeparator.Name = "tsddlProjectSeparator";
			this.tsddlProjectSeparator.Size = new System.Drawing.Size(187, 6);
			// 
			// tsmiProjectExport
			// 
			this.tsmiProjectExport.Name = "tsmiProjectExport";
			this.tsmiProjectExport.Size = new System.Drawing.Size(190, 26);
			this.tsmiProjectExport.Text = "&Export...";
			// 
			// tsmiProjectImport
			// 
			this.tsmiProjectImport.Name = "tsmiProjectImport";
			this.tsmiProjectImport.Size = new System.Drawing.Size(190, 26);
			this.tsmiProjectImport.Text = "&Import";
			// 
			// tsbnLaunch
			// 
			this.tsbnLaunch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.tsbnLaunch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnLaunch.Image = global::Plugin.PortClient.Properties.Resources.iconDebug;
			this.tsbnLaunch.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnLaunch.Name = "tsbnLaunch";
			this.tsbnLaunch.Size = new System.Drawing.Size(29, 24);
			this.tsbnLaunch.Text = "Launch &test (Ctrl+T)";
			this.tsbnLaunch.Click += new System.EventHandler(this.tsbnLaunch_Click);
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 27);
			this.splitMain.Margin = new System.Windows.Forms.Padding(4);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.gridSearch);
			this.splitMain.Panel1.Controls.Add(this.lvServers);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.pgProperties);
			this.splitMain.Panel2Collapsed = true;
			this.splitMain.Size = new System.Drawing.Size(200, 158);
			this.splitMain.SplitterWidth = 5;
			this.splitMain.TabIndex = 1;
			this.splitMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitMain_MouseDoubleClick);
			// 
			// gridSearch
			// 
			this.gridSearch.DataGrid = null;
			this.gridSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gridSearch.EnableFindCase = true;
			this.gridSearch.EnableFindHighlight = true;
			this.gridSearch.EnableFindPrevNext = true;
			this.gridSearch.EnableSearchHighlight = false;
			this.gridSearch.ListView = null;
			this.gridSearch.Location = new System.Drawing.Point(3, 155);
			this.gridSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.gridSearch.Name = "gridSearch";
			this.gridSearch.Size = new System.Drawing.Size(440, 29);
			this.gridSearch.TabIndex = 1;
			this.gridSearch.TreeView = null;
			this.gridSearch.Visible = false;
			// 
			// lvServers
			// 
			this.lvServers.AllowDrop = true;
			this.lvServers.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.lvServers.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
			this.lvServers.ShowCommandMenuOnRightClick = true;
			this.lvServers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvServers.FullRowSelect = true;
			this.lvServers.HideSelection = false;
			this.lvServers.IsSimpleDragSource = true;
			this.lvServers.IsSimpleDropSink = true;
			this.lvServers.UseFilterIndicator = true;
			this.lvServers.UseFiltering = true;
			this.lvServers.Location = new System.Drawing.Point(0, 0);
			this.lvServers.Margin = new System.Windows.Forms.Padding(4);
			this.lvServers.Name = "lvServers";
			this.lvServers.ShowGroups = false;
			this.lvServers.Size = new System.Drawing.Size(200, 158);
			this.lvServers.SmallImageList = this.ilServers;
			this.lvServers.TabIndex = 0;
			this.lvServers.UseCompatibleStateImageBehavior = false;
			this.lvServers.View = System.Windows.Forms.View.Details;
			this.lvServers.VirtualMode = true;
			this.lvServers.DirtyChanged += new System.EventHandler<System.EventArgs>(this.lvServers_DirtyChanged);
			this.lvServers.PortTestFinished += new System.EventHandler<Plugin.PortClient.PortTest.PortTestEventArgs>(this.lvServers_PortTestFinished);
			this.lvServers.TestFinished += new System.EventHandler<System.EventArgs>(this.lvServers_TestFinished);
			this.lvServers.SelectedIndexChanged += new System.EventHandler(this.lvServers_SelectedIndexChanged);
			this.lvServers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvServers_KeyDown);
			this.lvServers.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvServers_MouseClick);
			this.lvServers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvServers_MouseDoubleClick);
			// 
			// cmsNodes
			// 
			this.cmsNodes.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsNodes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNodesLaunch,
            this.toolStripSeparator2,
            this.tsmiNodesAddGroup,
            this.tsmiNodesAddServer,
            this.tsmiNodesAddPort,
            this.toolStripSeparator1,
            this.tsmiNodesCopy,
            this.tsmiNodesDelete,
            this.tsmiNodesProperties});
			this.cmsNodes.Name = "cmsServer";
			this.cmsNodes.Size = new System.Drawing.Size(200, 184);
			this.cmsNodes.Opening += new System.ComponentModel.CancelEventHandler(this.cmsServer_Opening);
			this.cmsNodes.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsServer_ItemClicked);
			// 
			// tsmiNodesLaunch
			// 
			this.tsmiNodesLaunch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tsmiNodesLaunch.Name = "tsmiNodesLaunch";
			this.tsmiNodesLaunch.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.tsmiNodesLaunch.Size = new System.Drawing.Size(199, 24);
			this.tsmiNodesLaunch.Text = "&Launch";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(196, 6);
			// 
			// tsmiNodesAddGroup
			// 
			this.tsmiNodesAddGroup.Name = "tsmiNodesAddGroup";
			this.tsmiNodesAddGroup.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.G)));
			this.tsmiNodesAddGroup.Size = new System.Drawing.Size(199, 24);
			this.tsmiNodesAddGroup.Text = "Add &Group";
			// 
			// tsmiNodesAddServer
			// 
			this.tsmiNodesAddServer.Name = "tsmiNodesAddServer";
			this.tsmiNodesAddServer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
			this.tsmiNodesAddServer.Size = new System.Drawing.Size(199, 24);
			this.tsmiNodesAddServer.Text = "Add &Server";
			// 
			// tsmiNodesAddPort
			// 
			this.tsmiNodesAddPort.Name = "tsmiNodesAddPort";
			this.tsmiNodesAddPort.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
			this.tsmiNodesAddPort.Size = new System.Drawing.Size(199, 24);
			this.tsmiNodesAddPort.Text = "Add &Port";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
			// 
			// tsmiNodesCopy
			// 
			this.tsmiNodesCopy.Name = "tsmiNodesCopy";
			this.tsmiNodesCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.tsmiNodesCopy.Size = new System.Drawing.Size(199, 24);
			this.tsmiNodesCopy.Text = "&Copy";
			// 
			// tsmiNodesDelete
			// 
			this.tsmiNodesDelete.Name = "tsmiNodesDelete";
			this.tsmiNodesDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.tsmiNodesDelete.Size = new System.Drawing.Size(199, 24);
			this.tsmiNodesDelete.Text = "&Delete";
			// 
			// tsmiNodesProperties
			// 
			this.tsmiNodesProperties.Name = "tsmiNodesProperties";
			this.tsmiNodesProperties.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.tsmiNodesProperties.Size = new System.Drawing.Size(199, 24);
			this.tsmiNodesProperties.Text = "&Properties";
			// 
			// ilServers
			// 
			this.ilServers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilServers.ImageStream")));
			this.ilServers.TransparentColor = System.Drawing.Color.Magenta;
			this.ilServers.Images.SetKeyName(0, "i.Server.bmp");
			this.ilServers.Images.SetKeyName(1, "i.Port.bmp");
			this.ilServers.Images.SetKeyName(2, "i.Folder.bmp");
			// 
			// pgProperties
			// 
			this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgProperties.LineColor = System.Drawing.SystemColors.ControlDark;
			this.pgProperties.Location = new System.Drawing.Point(0, 0);
			this.pgProperties.Margin = new System.Windows.Forms.Padding(4);
			this.pgProperties.Name = "pgProperties";
			this.pgProperties.Size = new System.Drawing.Size(150, 46);
			this.pgProperties.TabIndex = 0;
			this.pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProperties_PropertyValueChanged);
			// 
			// PanelPortTestClient
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Controls.Add(tsMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PanelPortTestClient";
			this.Size = new System.Drawing.Size(200, 185);
			tsMain.ResumeLayout(false);
			tsMain.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lvServers)).EndInit();
			this.cmsNodes.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.PropertyGrid pgProperties;
		private Plugin.PortClient.UI.ServerListView lvServers;
		private System.Windows.Forms.ContextMenuStrip cmsNodes;
		private System.Windows.Forms.ToolStripMenuItem tsmiNodesCopy;
		private System.Windows.Forms.ToolStripDropDownButton tsbnServerAdd;
		private System.Windows.Forms.ToolStripMenuItem tsmiAddServer;
		private System.Windows.Forms.ToolStripMenuItem tsmiAddPort;
		private System.Windows.Forms.ToolStripMenuItem tsmiNodesDelete;
		private System.Windows.Forms.ToolStripMenuItem tsmiNodesAddServer;
		private System.Windows.Forms.ToolStripMenuItem tsmiNodesAddPort;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem tsmiNodesProperties;
		private System.Windows.Forms.ToolStripButton tsbnLaunch;
		private System.Windows.Forms.ImageList ilServers;
		private AlphaOmega.Windows.Forms.SearchGrid gridSearch;
		private System.Windows.Forms.ToolStripMenuItem tsmiNodesLaunch;
		private System.Windows.Forms.ToolStripDropDownButton tsddlProject;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectLoad;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectExport;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectImport;
		private System.Windows.Forms.ToolStripButton tsbnProjectSave;
		private System.Windows.Forms.ToolStripSeparator tsddlProjectSeparator;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem tsmiNodesAddGroup;
		private System.Windows.Forms.ToolStripMenuItem tsmiAddGroup;
	}
}
