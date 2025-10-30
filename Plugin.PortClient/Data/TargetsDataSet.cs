using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Plugin.PortClient.PortTest;

namespace Plugin.PortClient.Data
{
	/// <summary>Data about target servers</summary>
	internal partial class TargetsDataSet
	{
		partial class GroupDataTable
		{
			public TargetsDataSet.GroupRow ModifyGroup(TargetsDataSet.GroupRow groupRow, String name)
			{
				if(String.IsNullOrEmpty(name))
					throw new ArgumentNullException(nameof(name));

				name = name.Trim();
				if(groupRow?.Name == name)
					return groupRow;

				if(groupRow == null)
					groupRow = this.NewGroupRow();

				groupRow.BeginEdit();
				groupRow.Name = name;

				if(groupRow.RowState == System.Data.DataRowState.Detached)
					this.AddGroupRow(groupRow);
				else
					groupRow.AcceptChanges();

				return groupRow;
			}
		}

		partial class PortsDataTable
		{
			public TargetsDataSet.PortsRow ModifyPort(TargetsDataSet.ServerRow serverRow, TargetsDataSet.PortsRow portRow, String ports)
			{
				_ = serverRow ?? throw new ArgumentNullException(nameof(serverRow));

				String portsFixed = ItemDto.FixPorts(ports);

				if(portRow?.Ports == portsFixed)
					return portRow;

				foreach(PortsRow row in serverRow.GetPortsRows())
					if(row.IsPortExists(portsFixed))
						throw new System.Data.ConstraintException("This port already specified in current server");

				if(portRow == null)
					portRow = this.NewPortsRow();

				portRow.BeginEdit();
				portRow.ServerId = serverRow.ServerId;
				portRow.Ports = portsFixed;

				if(portRow.RowState == System.Data.DataRowState.Detached)
					this.AddPortsRow(portRow);
				else
					portRow.AcceptChanges();

				return portRow;
			}
		}

		partial class ServerDataTable
		{
			/// <summary>Change server data</summary>
			/// <param name="groupRow">Parent group where server belongs</param>
			/// <param name="serverRow">Server ID or null if new</param>
			/// <param name="serverAddress">Server address</param>
			/// <returns>Data for the row added to the dataset</returns>
			public TargetsDataSet.ServerRow ModifyServerRow(TargetsDataSet.GroupRow groupRow, TargetsDataSet.ServerRow serverRow, String serverAddress)
			{
				if(String.IsNullOrEmpty(serverAddress))
					throw new ArgumentNullException(nameof(serverAddress));

				TargetsDataSet.ServerRow row = serverRow ?? this.NewServerRow();

				row.BeginEdit();
				row.GroupIdI = groupRow?.GroupId;
				row.HostAddress = serverAddress;

				if(row.RowState == System.Data.DataRowState.Detached)
					this.AddServerRow(row);
				else
					row.AcceptChanges();

				return row;
			}
		}

		/// <summary>Data</summary>
		partial class ServerRow
		{
			public Int32? GroupIdI
			{
				get => this.IsGroupIdNull() ? (Int32?)null : this.GroupId;
				set
				{
					if(value == null)
						this.SetGroupIdNull();
					else
						this.GroupId = value.Value;
				}
			}

			public IPAddress[] IpAddressArr
			{
				get
				{//TODO: SocketException: No such host is known
					IPAddress[] addr = Dns.GetHostAddresses(this.HostAddress);
					return this.AddressFamilyI == null
						? addr
						: Array.FindAll(addr, item => item.AddressFamily == this.AddressFamilyI);
				}
			}

			public AddressFamily? AddressFamilyI
			{
				get => this.IsAddressFamilyNull() ? (AddressFamily?)null : (AddressFamily)this.AddressFamily;
				set
				{
					if(value == null || value == System.Net.Sockets.AddressFamily.InterNetwork)
						this.SetAddressFamilyNull();
					else
						this.AddressFamily = (UInt16)value;
				}
			}

			public String CommentsI
			{
				get => this.IsCommentsNull() ? null : this.Comments;
				set
				{
					if(value == null || value.Trim().Length == 0)
						this.SetCommentsNull();
					else
						this.Comments = value;
				}
			}

			public IEnumerable<PortTestDto> GetServerTests(TargetsDataSet.PortsRow portRow = null)
			{
				TargetsDataSet.PortsRow[] rows = portRow == null
					? this.GetPortsRows()
					: new TargetsDataSet.PortsRow[] { portRow };

				if(rows.Length == 0)//Server does not have specified ports
					foreach(IPAddress item in this.IpAddressArr)
						foreach(UInt16 port in Enumerable.Range(0, UInt16.MaxValue))/*TODO: Think about how to beautifully rewrite it to extension*/
							yield return new PortTestDto(this, null, item, port);
				else//No specific row is specified, we take all rows for the server
					foreach(TargetsDataSet.PortsRow row in rows)
						foreach(IPAddress item in this.IpAddressArr)
							foreach(UInt16 port in row.GetPorts())
								yield return new PortTestDto(this, row, item, port);
			}
		}

		/// <summary>Data on server monitoring ports</summary>
		partial class PortsRow
		{
			/// <summary>banana banana banana</summary>
			public SocketType SocketTypeI
			{
				get => this.IsSocketTypeNull() ? System.Net.Sockets.SocketType.Stream : (SocketType)this.SocketType;
				set
				{
					if(value == System.Net.Sockets.SocketType.Stream)
						this.SetSocketTypeNull();
					else
						this.SocketType = (UInt16)value;
				}
			}

			/// <summary>The type of protocol we will use to connect</summary>
			public ProtocolType ProtocolTypeI
			{
				get => this.IsProtocolTypeNull() ? System.Net.Sockets.ProtocolType.Tcp : (ProtocolType)this.ProtocolType;
				set
				{
					if(value == System.Net.Sockets.ProtocolType.Tcp)
						this.SetProtocolTypeNull();
					else
						this.ProtocolType = (UInt16)value;
				}
			}

			public String PortsI
			{
				get => this.Ports;
				set => this.Ports = ItemDto.FixPorts(value);
			}

			public String CommentsI
			{
				get => this.IsCommentsNull() ? null : this.Comments;
				set
				{
					if(value == null || value.Trim().Length == 0)
						this.SetCommentsNull();
					else
						this.Comments = value;
				}
			}

			public Int32? ConnectTimeoutI
			{
				get => this.IsConnectTimeoutNull() ? (Int32?)null : this.ConnectTimeout;
				set
				{
					if(value == null || value <= 0)
						this.SetConnectTimeoutNull();
					else
						this.ConnectTimeout = value.Value;
				}
			}

			internal IEnumerable<UInt16> GetPorts()
				=> ItemDto.GetPorts(this.PortsI);

			public Boolean IsPortExists(String portsFixed)
			{
				UInt16[] portsRange = ItemDto.GetPorts(portsFixed).ToArray();
				foreach(UInt16 port in this.GetPorts())
					if(portsRange.Any(r => r == port))
						return true;
				return false;
			}
		}
	}
}