using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Plugin.PortClient.Data;
using Plugin.PortClient.PortTest;

namespace Plugin.PortClient.UI
{
	internal class ServerListView : TreeListView
	{
		private readonly OLVColumn _colName;
		private readonly OLVColumn _colDescription;

		public TargetsBll Project { get; private set; }

		[Browsable(false)]
		public PluginWindows Plugin { get; set; }

		public String FilePath { get; private set; }

		public Boolean IsDirty { get; private set; }

		public event EventHandler<EventArgs> DirtyChanged;
		public event EventHandler<PortTestEventArgs> PortTestFinished;
		public event EventHandler<EventArgs> TestFinished;

		public ServerListView()
		{
			this._colName = new OLVColumn()
			{
				Text = "Name",
				AspectName = "Name",
				IsEditable = true,
				IsTileViewColumn = true,
				UseInitialLetterForGroup = false,
				AutoCompleteEditor = false,
				WordWrap = false,
				AspectGetter = (item) =>
				{
					if(item is ServerDto server)
						return server.HostAddress;
					else if(item is PortsDto port)
						return port.Ports;
					else if(item is GroupDto group)
						return group.Name;
					else
						return null;
				},
				ImageGetter = (item) =>
				{
					if(item is ServerDto)
						return (Int32)ItemDto.TreeImageList.Server;
					else if(item is PortsDto)
						return (Int32)ItemDto.TreeImageList.Port;
					else if(item is GroupDto)
						return (Int32)ItemDto.TreeImageList.Folder;
					else
						return null;
				},
			};

			this._colDescription = new OLVColumn()
			{
				Text = "Description",
				AspectName = "Description",
				IsEditable = false,
				UseInitialLetterForGroup = false,
				WordWrap = false,
				AspectGetter = (item) => {
					return item is ServerDto server
						? server.Description
						: item is PortsDto port
						? port.Description
						: null;
				},
			};
		}

		protected override void OnCreateControl()
		{
			if(this.IsDesignMode)
				return;

			base.CanExpandGetter = (item) =>
			{
				if(item is ServerDto server)
					return server.Ports?.Count > 0;
				else if(item is GroupDto group)
					return group.Servers?.Count > 0;
				return false;
			};

			base.ChildrenGetter = (item) =>
			{
				return item is ServerDto server
					? server.Ports
					: item is GroupDto group
					? group.Servers
					: (System.Collections.IEnumerable)null;
			};

			base.AllColumns.Add(this._colName);
			base.AllColumns.Add(this._colDescription);
			base.Columns.AddRange(new ColumnHeader[] { this._colName, this._colDescription, });

			this.TreeColumnRenderer.IsShowGlyphs = true;
			this.TreeColumnRenderer.UseTriangles = true;

			base.LastSortColumn = this._colName;
			base.IsSimpleDragSource = true;
			base.IsSimpleDropSink = true;
			SimpleDropSink dropSing = (SimpleDropSink)base.DropSink;
			dropSing.AcceptExternal = false;
			dropSing.CanDropBetween = false;
			dropSing.CanDropOnBackground = true;

			base.OnCreateControl();
		}

		protected override void OnModelDropped(ModelDropEventArgs args)
		{
			GroupDto group = args.TargetModel as GroupDto;
			if(group == null && args.TargetModel is ServerDto server)
				group = server.Group;

			IEnumerable<ItemDto> source = args.SourceModels.Cast<ItemDto>();

			foreach(ServerDto item in source)
			{
				GroupDto oldGroup = item.Group;
				Boolean isWasInRoot = item.Group == null;

				item.ChangeGroup(group);

				if(item.Group == null)//Moved to root
					base.AddObject(item);
				else if(isWasInRoot)//Moved from root
					base.RemoveObject(item);
				else//Moving between groups
				{
					if(oldGroup != null)
						base.RefreshObject(oldGroup);
					base.RefreshObject(group);
				}
			}

			if(group != null)
				base.Expand(group);

			base.OnModelDropped(args);
		}

		protected override void OnModelCanDrop(ModelDropEventArgs args)
		{
			args.Handled = true;
			args.Effect = DragDropEffects.None;
			if(args.SourceModels.Contains(args.TargetModel))
				args.InfoMessage = "Can't drop to self";
			else
			{
				ItemDto[] source = args.SourceModels.Cast<ItemDto>().ToArray();
				ItemDto target = (ItemDto)args.TargetModel;

				switch(args.DropTargetLocation)
				{
				case DropTargetLocation.None:
					if(target == null)
					{
						if(source.All(s => { return s is ServerDto; }))
							args.Effect = DragDropEffects.Move;
					}
					break;
				case DropTargetLocation.Item:
					if(target is GroupDto group)
					{
						if(source.All(s => { return s is ServerDto item && (item.Group == null || item.Group != group); }))
							args.Effect = DragDropEffects.Move;
					} else if(target is ServerDto server)
					{
						if(source.All(s => { return s is ServerDto item && item.Group != server.Group; }))
							args.Effect = DragDropEffects.Move;
					}
					break;
				}
			}

			base.OnModelCanDrop(args);
		}

		private static readonly Dictionary<ItemDto.StateType, Color> StateTypeMapping = new Dictionary<ItemDto.StateType, Color>()
		{
			{ ItemDto.StateType.Default, Color.Empty },
			{ ItemDto.StateType.Error, Color.Red },
			{ ItemDto.StateType.Pending, Color.Orange },
			{ ItemDto.StateType.Success, Color.Green },
		};

		protected override void HandleApplicationIdleResizeColumns(Object sender, EventArgs e)
		{
			/*if(base.IsCellEditing)//This logic is not working. Because when we started to edit cell this event cancels it
				base.CancelCellEdit();*/

			base.HandleApplicationIdleResizeColumns(sender, e);
		}

		protected override void OnFormatRow(FormatRowEventArgs args)
		{
			if(args.Model is ServerDto server)
				args.Item.ForeColor = StateTypeMapping[server.State];
			else if(args.Model is PortsDto ports)
				args.Item.ForeColor = StateTypeMapping[ports.State];

			base.OnFormatRow(args);
		}

		protected override void OnCellEditorValidating(CellEditEventArgs e)
		{
			String newValue = (String)e.NewValue;
			e.Cancel = String.IsNullOrEmpty(newValue);

			base.OnCellEditorValidating(e);
		}

		protected override void OnCellEditFinishing(CellEditEventArgs e)
		{
			ItemDto rowObject = (ItemDto)e.RowObject;
			String newValue = (String)e.NewValue;
			if(e.Cancel == true)
			{
				if(rowObject is PortsDto port && port.Row == null)
					port.Server.Ports.Remove(port);
				else if(rowObject is ServerDto server && server.Row == null && server.Group != null)
					server.Group.Servers.Remove(server);
				else if(rowObject is GroupDto group && group.Row == null)
					base.RemoveObject(rowObject);
				return;
			}

			try
			{
				if(rowObject is GroupDto group)
				{
					group.Row = this.Project.ModifyGroupRow(group.Row, newValue);
					this.UpdateObject(group);
					this.Expand(group);
				} else if(rowObject is ServerDto server)
				{
					server.Row = this.Project.ModifyServerRow(server.Group?.Row, server.Row, newValue);
					this.Expand(server);
				} else if(rowObject is PortsDto ports)
					ports.Row = this.Project.ModifyPortRow(ports.Server.Row, ports.Row, newValue);
				else
					throw new NotImplementedException();

				this.ToggleDirty(true);
			} catch(System.Data.ConstraintException)
			{//Maybe the node with the same name already exists
				ItemDto oldNode = null;
				if(rowObject is GroupDto group && group.Row == null)
					oldNode = this.FindNode<GroupDto>((node) => { return node.Row != null && String.Equals(node.Name, newValue, StringComparison.OrdinalIgnoreCase); });
				else if(rowObject is ServerDto server && server.Row == null)
					oldNode = this.FindNode<ServerDto>((node) => { return node.Row != null && String.Equals(node.HostAddress, newValue, StringComparison.OrdinalIgnoreCase); });
				else if(rowObject is PortsDto port && port.Row == null)
					oldNode = this.FindNode<PortsDto>((node) => { return node.Row != null && node.Ports == newValue; });

				if(oldNode != null)
				{
					if(rowObject is PortsDto port && port.Row == null)
						port.Server.Ports.Remove(port);
					else if(rowObject is ServerDto server && server.Row == null && server.Group != null)
						server.Group.Servers.Remove(server);
					base.RemoveObject(rowObject);
					base.SelectedObject = oldNode;
					return;
				}

				e.Cancel = true;
				e.Control.ForeColor = Color.Red;
				throw;
			} catch(Exception)
			{//Не удалось добавить ноду. Пытаемся оставить старую ноду с текстом
				e.Cancel = true;
				e.Control.ForeColor = Color.Red;
				throw;
			}

			base.OnCellEditFinishing(e);
		}

		private PortTestInstance _instance;

		public Boolean LaunchTest(params ItemDto[] nodes)
		{
			if(nodes == null || nodes.Length == 0)
				throw new ArgumentNullException(nameof(nodes));

			//We need to remove all groups before testing and add child nodes
			List<ItemDto> groupCheck = new List<ItemDto>(nodes);
			for(Int32 loop = groupCheck.Count - 1; loop >= 0; loop--)
				if(groupCheck[loop] is GroupDto group)
				{
					groupCheck.RemoveAt(loop);
					groupCheck.AddRange(group.Servers.ToArray());
				}
			nodes = groupCheck.ToArray();
			if(nodes.Length == 0)
				return false;

			//We need to clear state of sibling ports to empty or server node will stay in pending state
			foreach(ItemDto node in nodes)
				if(node is PortsDto port)
					foreach(PortsDto item in port.Server.Ports)
						item.State = ItemDto.StateType.Default;
				else if(node is ServerDto server)
					foreach(PortsDto item in server.Ports)
						item.State = ItemDto.StateType.Default;

			foreach(ItemDto node in nodes)
			{
				node.State = ItemDto.StateType.Pending;
				if(node is PortsDto port)
					port.Server.State = ItemDto.StateType.Pending;
				else if(node is ServerDto server)
					foreach(PortsDto item in server.Ports)
						item.State = ItemDto.StateType.Pending;
			}

			this.RefreshObjects(nodes);

			base.Cursor = Cursors.WaitCursor;
			this.SaveProject();

			this._instance = new PortTestInstance();
			this._instance.OnPortTestFinished += this.Instance_PortTestFinished;
			this._instance.OnTestFinished += this.Instance_TestFinished;
			foreach(ItemDto item in nodes)
			{
				try
				{
					if(item is ServerDto server)
						this._instance.CheckPorts(server.Row);
					else if(item is PortsDto ports)
						this._instance.CheckPorts(ports.Server.Row, ports.Row);
				}catch(SocketException exc)
				{
					this.Plugin.Trace.TraceData(TraceEventType.Error, 6, exc);
					item.State = ItemDto.StateType.Error;
				}
			}
			this._instance.IsSended = true;
			return true;
		}

		public void CancelTest()
			=> this._instance?.Cancel();

		private void Instance_PortTestFinished(Object sender, PortTestEventArgs args)
		{
			ItemDto.StateType state = args.IsConnected ? ItemDto.StateType.Success : ItemDto.StateType.Error;
			if(args.Dto.PortRow != null)
			{
				PortsDto port = this.FindNode(args.Dto.PortRow);
				port.SetNodeStateRecursive(state);
				this.RefreshObjects(new ItemDto[] { port, port.Server });
			} else if(args.Dto.ServerRow != null)
			{
				ServerDto server = this.FindNode(args.Dto.ServerRow);
				server.State = state;
				this.RefreshObject(state);
			} else
				throw new NotSupportedException();

			this.PortTestFinished?.Invoke(sender, args);
		}

		private void Instance_TestFinished(Object sender, EventArgs args)
		{
			if(this.InvokeRequired)
				this.BeginInvoke((MethodInvoker)delegate { this.Instance_TestFinished(sender, args); });
			else
			{
				base.Cursor = Cursors.Default;
				this.TestFinished?.Invoke(sender, args);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.Delete:
			case Keys.Back:
				this.RemoveSelectedNodes();
				e.Handled = true;
				break;
			case Keys.Alt | Keys.P:
				if(this.SelectedObject != null)
				{
					this.AddNewNode(ItemDto.TreeImageList.Port);
					e.Handled = true;
				}
				break;
			case Keys.Alt | Keys.S:
				this.AddNewNode(ItemDto.TreeImageList.Server);
				e.Handled = true;
				break;
			case Keys.Alt | Keys.G:
				this.AddNewNode(ItemDto.TreeImageList.Folder);
				e.Handled = true;
				break;
			case Keys.Control | Keys.S:
				this.SaveProject();
				e.Handled = true;
				break;
			}
			base.OnKeyDown(e);
		}

		public void SaveProject()
		{
			if(this.IsDirty)
			{
				if(this.FilePath == null)
					this.SaveProjectToStorage();
				else
					this.SaveProjectToFile(false);
			}
		}

		public void SaveProjectToStorage()
			=> this.SaveProject(null);

		public String ChooseProjectFile()
		{
			using(OpenFileDialog dlg = new OpenFileDialog()
			{
				Filter = "Servers test list (xml) (*.xptst)|*.xptst|All files (*.*)|*.*",
				DefaultExt = "xptst"
			})
				return dlg.ShowDialog() == DialogResult.OK
					? dlg.FileName
					: null;
		}

		/// <summary>Save all project data to file on file system</summary>
		/// <param name="showSaveDialog">Show save dialog or silently save project to previosly choosed path</param>
		public void SaveProjectToFile(Boolean showSaveDialog)
		{
			String filePath = this.FilePath;
			if(showSaveDialog)
			{
				using(SaveFileDialog dlg = new SaveFileDialog()
				{
					Filter = "Servers test list (*.xptst)|*.xptst|PowerShell (export) (*.ps1)|*.ps1",
					DefaultExt = "xptst",
					AddExtension = true,
					FileName = this.FilePath,
				})
					if(dlg.ShowDialog() == DialogResult.OK)
						filePath = dlg.FileName;
					else
						return;
			}

			this.SaveProject(filePath);
		}

		private void SaveProject(String filePath)
		{
			if(filePath == null)
				this.Plugin.Settings.SaveProject(this.Project);
			else if(Path.GetExtension(filePath).ToLowerInvariant() == ".ps1")
			{//It's export not the storage (Don't forget to save after export)
				this.Project.SaveAsPowerShell(filePath);
				return;
			} else
				this.Project.SaveAsXml(filePath);

			this.FilePath = filePath;
			this.SaveColumnState();
			this.ToggleDirty(false);
		}

		public void LoadProject(String filePath = null)
		{
			this.Project = filePath == null
				? this.Plugin.Settings.LoadProject()
				: new TargetsBll(filePath);
			this.Project.OnFileChanged += this.Project_OnFileChanged;

			this.RestoreColumnState();

			base.ClearObjects();
			base.AddObjects(this.Project.GetTree());
			foreach(ItemDto item in base.Objects)
				if(item is GroupDto)
					base.Expand(item);
			this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

			this.FilePath = filePath;
			this.ToggleDirty(false);
		}

		private void Project_OnFileChanged(Object sender, EventArgs e)
		{
			base.ClearObjects();
			base.AddObjects(this.Project.GetTree());
			foreach(ItemDto item in base.Objects)
				if(item is GroupDto)
					base.Expand(item);
			this.ToggleDirty(false);
		}

		public void AddNewNode(ItemDto.TreeImageList nodeToAdd)
		{
			if(base.IsCellEditing)
				return;//Editing available only once at a time

			ItemDto newNode;
			switch(nodeToAdd)
			{
			case ItemDto.TreeImageList.Folder:
				newNode = new GroupDto();
				base.AddObject(newNode);
				break;
			case ItemDto.TreeImageList.Server:
				if(!(this.SelectedObject is GroupDto group))
				{
					ServerDto sibling = this.SelectedObject as ServerDto;
					group = sibling?.Group;
				}

				if(group == null)
				{
					newNode = new ServerDto(group);
					base.AddObject(newNode);
				} else
				{
					newNode = group.AddNew();
					base.RefreshObject(group);
					base.Expand(group);
				}
				break;
			case ItemDto.TreeImageList.Port:
				if(this.SelectedObject == null)
					return;

				ServerDto server;
				if(this.SelectedObject is ServerDto check3)
					server = check3;
				else if(this.SelectedObject is PortsDto check4)
					server = check4.Server;
				else
					throw new NotSupportedException();

				newNode = server.AddNew();
				base.RefreshObject(server);
				base.Expand(server);
				break;
			default:
				throw new NotSupportedException();
			}
			base.SelectedObject = newNode;
			base.EditModel(newNode);
		}

		public void RemoveSelectedNodes()
		{
			ItemDto[] nodes = this.SelectedObjects.Cast<ItemDto>().ToArray();
			Boolean isMultiDelete = false;
			if(nodes.Length == 0)
				return;
			else if(nodes.Length > 1)
			{
				isMultiDelete = true;
				if(MessageBox.Show("Are you shure you want to remove selected nodes?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					return;
			}

			foreach(ItemDto node in nodes)
			{
				if(node == null)
					return;
				else if(node is ServerDto server)
				{
					if(isMultiDelete || MessageBox.Show("Are you shure you want to remove selected server node and all ports?", "Server: " + server.HostAddress, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						server.Group?.Servers.Remove(server);
						this.Project.RemoveServer(server.Row);
					}
				} else if(node is PortsDto ports)
				{
					if(isMultiDelete || MessageBox.Show("Are you shure you want to remove selected port range?", "Ports: " + ports.Ports, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						ports.Server.Ports.Remove(ports);
						this.Project.RemovePort(ports.Row);
					}
				}else if(node is GroupDto group)
				{
					if(isMultiDelete || MessageBox.Show("Are you shure you want to remove selected group?", "Group: " + group.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						this.Project.RemoveGroup(group.Row);
				}
				else
					throw new NotSupportedException();

				this.RemoveObject(node);
			}
			this.ToggleDirty(true);
		}

		public ServerDto FindNode(TargetsDataSet.ServerRow row)
			=> this.FindNode<ServerDto>((node) => node.Row == row);

		public T FindNode<T>(Func<T, Boolean> callback) where T : ItemDto
			=> FindNode(this.Objects.Cast<ItemDto>(), callback);

		private static T FindNode<T>(IEnumerable<ItemDto> items, Func<T, Boolean> callback) where T : ItemDto
		{
			foreach(Object obj in items)
			{
				if(obj is T item && callback(item))
					return item;
				else if(obj is GroupDto group && group.Servers != null)
				{
					T result = FindNode<T>(group.Servers.Cast<ItemDto>(), callback);
					if(result != null)
						return result;
				} else if(obj is ServerDto server && server.Ports != null)
				{
					T result = FindNode<T>(server.Ports.Cast<ItemDto>(), callback);
					if(result != null)
						return result;
				}
			}

			return null;
		}

		public PortsDto FindNode(TargetsDataSet.PortsRow row)
		{//We need to search for the server node because port node can be in detached state
			ServerDto dto = this.FindNode(row.ServersRow);
			foreach(PortsDto ports in dto.Ports)
				if(ports.Row == row)
					return ports;

			return null;
		}

		public void ToggleDirty(Boolean isDirty)
		{
			this.IsDirty = isDirty;
			this.DirtyChanged?.Invoke(this, EventArgs.Empty);
		}

		private void SaveColumnState()
		{
			List<String> result = new List<String>(base.AllColumns.Count);

			foreach(OLVColumn column in base.AllColumns)
				if(column.IsVisible)
					result.Add(column.Text);
			if(result.Count == base.AllColumns.Count)
				result.Clear();

			this.Plugin.Settings.ColumnVisible = ObjectPropertyParser.SetPropertiesToString(typeof(TargetsBll), result.ToArray(), this.Plugin.Settings.ColumnVisible);
		}

		private void RestoreColumnState()
		{
			String[] columns = ObjectPropertyParser.GetPropertiesFromString(typeof(TargetsBll), this.Plugin.Settings.ColumnVisible);
			if(columns.Length > 0)
			{
				foreach(OLVColumn column in base.AllColumns)
				{
					Boolean isVisible = false;
					foreach(String columnName in columns)
						if(column.Text == columnName)
						{
							isVisible = true;
							break;
						}
					if(!isVisible)
						column.IsVisible = false;
				}
				base.RebuildColumns();
			}
		}
	}
}
