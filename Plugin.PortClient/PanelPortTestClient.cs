using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Plugin.PortClient.Data;
using Plugin.PortClient.PortTest;
using Plugin.PortClient.Properties;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.PortClient
{
	public partial class PanelPortTestClient : UserControl, IPluginSettings<PanelPortTestClientSettings>
	{
		private const String Caption = "Port Test Client";
		private PanelPortTestClientSettings _settings;

		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		private IWindow Window => (IWindow)base.Parent;

		Object IPluginSettings.Settings => this.Settings;

		public PanelPortTestClientSettings Settings
			=> this._settings ?? (this._settings = new PanelPortTestClientSettings());

		public PanelPortTestClient()
		{
			this.InitializeComponent();
			gridSearch.ListView = lvServers;
		}

		protected override void OnCreateControl()
		{
			this.SetWindowCaption();
			this.Window.SetTabPicture(Resources.WindowCaption);
			lvServers.Plugin = this.Plugin;
			this.LoadProject();

			base.OnCreateControl();
		}

		private void SetWindowCaption()
		{
			List<String> captions = new List<String>();
			if(this.Settings.ProjectFileName != null)
				captions.Add(Path.GetFileName(this.Settings.ProjectFileName));
			captions.Add(PanelPortTestClient.Caption);
			if(tsbnLaunch.Checked)
				captions.Add("Checking...");

			this.Window.Caption = String.Join(" - ", captions.ToArray());
		}

		/// <summary>Upload project to form</summary>
		/// <param name="filePath">Path to file or upload from local storage</param>
		private void LoadProject()
		{
			String filePath = this.Settings.ProjectFileName;
			if(filePath != null && !File.Exists(filePath))
			{
				this.Plugin.Trace.TraceEvent(TraceEventType.Warning, 7, "File {0} not found", filePath);
				filePath = null;
			}

			lvServers.LoadProject(filePath);

			this.SetWindowCaption();
			tsmiProjectExport.Visible = filePath == null;
			tsmiProjectImport.Visible = filePath != null;
		}

		private void lvServers_PortTestFinished(Object sender, PortTestEventArgs args)
		{
			if(args.Exception != null)
			{
				this.Plugin.Trace.TraceData(TraceEventType.Error, 6, args.Exception);
				return;
			}

			switch(this.Plugin.Settings.LogType)
			{
			case PluginSettings.LogPortsType.Opened:
				if(!args.IsConnected)
					return;
				break;
			case PluginSettings.LogPortsType.Closed:
				if(args.IsConnected)
					return;
				break;
			case PluginSettings.LogPortsType.All:
			default:
				break;
			}

			var props = new
			{
				args.IsConnected,
				Status = args.IsConnected ? "Opened" : "Closed",
				args.Elapsed,
				IpAddress = args.Dto.Address,
				args.Dto.Port,
				ServerName = args.Dto.ServerRow.HostAddress,
				ServerComments = args.Dto.ServerRow.CommentsI,
				PortComments = args.Dto.PortRow?.CommentsI,
			};
			String message = Constant.FormatMessage(this.Plugin.Settings.MessageFormat, props);
			this.Plugin.Trace.TraceEvent(TraceEventType.Information, args.IsConnected ? 1 : 0, message);
		}

		private void lvServers_TestFinished(Object sender, EventArgs args)
		{
			base.Cursor = Cursors.Default;
			tsbnLaunch.Checked = false;
			tsbnLaunch.Enabled = true;
			this.SetWindowCaption();
		}

		private void ShowProperties(ItemDto node)
		{
			if(node == null)
				pgProperties.SelectedObject = null;
			else
			{
				try
				{
					pgProperties.SelectedObject = node;
				} catch(SocketException exc)
				{
					this.Plugin.Trace.TraceData(TraceEventType.Warning, 10, exc);
				}
				splitMain.Panel2Collapsed = false;
			}
		}

		private void pgProperties_PropertyValueChanged(Object sender, PropertyValueChangedEventArgs e)
		{
			lvServers.ToggleDirty(true);
			lvServers.RefreshSelectedObjects();
		}

		private void tsbnServerAdd_DropDownOpening(Object sender, EventArgs e)
		{
			ItemDto[] nodes = lvServers.SelectedObjects.Cast<ItemDto>().ToArray();
			tsmiAddPort.Enabled = nodes.Length == 1 && (nodes[0] is ServerDto || nodes[0] is PortsDto);
		}

		private void tsbnServerAdd_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{//TODO: ObjectListView don't work with .NET Core
			if(lvServers.SelectedObject != null && lvServers.IsCellEditing)
				return;//If the node is already being edited

			if(e.ClickedItem == tsmiAddGroup)
				lvServers.AddNewNode(ItemDto.TreeImageList.Folder);
			else if(e.ClickedItem == tsmiAddServer)
				lvServers.AddNewNode(ItemDto.TreeImageList.Server);
			else if(e.ClickedItem == tsmiAddPort)
				lvServers.AddNewNode(ItemDto.TreeImageList.Port);
			else
				throw new NotImplementedException();
		}

		private void splitMain_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(splitMain.SplitterRectangle.Contains(e.Location))
				splitMain.Panel2Collapsed = true;
		}

		private void lvServers_MouseClick(Object sender, MouseEventArgs e)
		{
			switch(e.Button)
			{
			case MouseButtons.Right:
				ListViewHitTestInfo info = lvServers.HitTest(e.Location);
				if(info.Item != null)
					cmsNodes.Show(lvServers, e.Location);
				break;
			}
		}

		private void lvServers_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			ListViewHitTestInfo info = lvServers.HitTest(e.Location);
			ItemDto item = (ItemDto)(info.Item as OLVListItem)?.RowObject;
			if(item != null)
				this.LaunchTest(item);
		}

		private void lvServers_DirtyChanged(Object sender, EventArgs e)
		{
			tsbnProjectSave.Enabled = lvServers.IsDirty;

			if(!lvServers.IsDirty)
			{
				this.Settings.SetValues(lvServers.FilePath);
				this.SetWindowCaption();
				tsmiProjectExport.Visible = lvServers.FilePath == null;
				tsmiProjectImport.Visible = lvServers.FilePath != null;
			}
		}

		private void lvServers_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
			case Keys.Control | Keys.T:
				this.tsbnLaunch_Click(sender, e);
				e.Handled = true;
				break;
			case Keys.Control | Keys.O:
				this.tsbnServerAdd_DropDownItemClicked(sender, new ToolStripItemClickedEventArgs(tsmiProjectLoad));
				e.Handled = true;
				break;
			case Keys.Control | Keys.P:
				ItemDto item = (ItemDto)lvServers.SelectedObject;
				if(item != null)
				{
					this.ShowProperties(item);
					e.Handled = true;
				}
				break;
			}
		}

		private void lvServers_SelectedIndexChanged(Object sender, EventArgs e)
		{
			if(!splitMain.Panel2Collapsed)
			{
				ItemDto node = (ItemDto)lvServers.SelectedObject;
				this.ShowProperties(node);
			}
		}

		private void cmsServer_Opening(Object sender, CancelEventArgs e)
		{
			ItemDto[] nodes = lvServers.SelectedObjects.Cast<ItemDto>().ToArray();
			tsmiNodesCopy.Visible = nodes.Length == 1;
			tsmiNodesDelete.Visible = nodes.Length > 0;
			tsmiNodesAddPort.Enabled = nodes.Length == 1 && (nodes[0] is ServerDto || nodes[0] is PortsDto);
			tsmiNodesProperties.Visible = nodes.Length == 1;
			tsmiNodesLaunch.Visible = nodes.Length > 0;
		}

		private void cmsServer_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiNodesCopy)
			{
				ItemDto dto = (ItemDto)lvServers.SelectedObject;
				if(dto is ServerDto server)
					Clipboard.SetText(server.HostAddress);
				else if(dto is PortsDto ports)
					Clipboard.SetText(ports.Ports);
				else
					throw new NotSupportedException();
			} else if(e.ClickedItem == tsmiNodesDelete)
				lvServers.RemoveSelectedNodes();
			else if(e.ClickedItem == tsmiNodesAddServer)
				lvServers.AddNewNode(ItemDto.TreeImageList.Server);
			else if(e.ClickedItem == tsmiNodesAddPort)
				lvServers.AddNewNode(ItemDto.TreeImageList.Port);
			else if(e.ClickedItem == tsmiNodesAddGroup)
				lvServers.AddNewNode(ItemDto.TreeImageList.Folder);
			else if(e.ClickedItem == tsmiNodesProperties)
				this.ShowProperties((ItemDto)lvServers.SelectedObject);
			else if(e.ClickedItem == tsmiNodesLaunch)
			{
				ItemDto[] items = lvServers.SelectedObjects.Cast<ItemDto>().ToArray();
				this.LaunchTest(items);
			} else
				throw new NotSupportedException();
		}

		private void LaunchTest(params ItemDto[] nodes)
		{
			if(nodes == null || nodes.Length == 0)
				return;

			if(tsbnLaunch.Checked)
			{
				tsbnLaunch.Enabled = false;
				lvServers.CancelTest();
				return;
			}

			if(lvServers.LaunchTest(nodes))
			{
				base.Cursor = Cursors.WaitCursor;
				tsbnLaunch.Checked = true;
				this.SetWindowCaption();
			}
		}

		private void tsbnLaunch_Click(Object sender, EventArgs e)
		{
			ItemDto[] items = lvServers.Objects.Cast<ItemDto>().ToArray();
			this.LaunchTest(items);
		}

		private void tsbnProjectSave_Click(Object sender, EventArgs e)
			=> lvServers.SaveProject();

		private void tsddlProject_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			tsddlProject.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);

			if(e.ClickedItem == tsmiProjectLoad)
			{
				String fileName = lvServers.ChooseProjectFile();
				if(fileName == null)
					return;
				if(fileName == this.Settings.ProjectFileName)
					this.Settings.ProjectFileName = fileName;
				else if(this.Plugin.CreateWindow(typeof(PanelPortTestClient).ToString(), true, new PanelPortTestClientSettings() { ProjectFileName = fileName, }) == null)
					this.Plugin.Trace.TraceEvent(TraceEventType.Warning, 1, "Error opening window");
			} else if(e.ClickedItem == tsmiProjectExport)
				lvServers.SaveProjectToFile(true);
			else if(e.ClickedItem == tsmiProjectImport)
				lvServers.SaveProjectToStorage();
			else
				throw new NotSupportedException($"Unknown item: {e.ClickedItem}");
		}
	}
}