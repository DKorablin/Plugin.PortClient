using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Net;

namespace Plugin.PortClient.Data
{
	internal class ServerDto : ItemDto
	{
		private List<PortsDto> _ports;

		[Browsable(false)]
		internal GroupDto Group { get; private set; }

		[Browsable(false)]
		internal TargetsDataSet.ServerRow Row { get; set; }

		[Browsable(false)]
		public List<PortsDto> Ports
		{
			get
			{
				if(this._ports == null && this.Row != null)
					this._ports = new List<PortsDto>(Array.ConvertAll(this.Row.GetPortsRows(), (port) => new PortsDto(this, port)));
				return this._ports;
			}
		}

		[Category("Connection")]
		public String HostAddress
		{
			get => this.Row?.HostAddress;
			set => this.Row.HostAddress = value;
		}

		[Category("Connection")]
		public IPAddress[] IpAddressArr
			=> this.Row?.IpAddressArr;

		[Category("Connection")]
		[DefaultValue(System.Net.Sockets.AddressFamily.InterNetwork)]
		public System.Net.Sockets.AddressFamily? AddressFamily
		{
			get => this.Row?.AddressFamilyI;
			set => this.Row.AddressFamilyI = value;
		}

		[Category("UI")]
		[Description("Server description")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String Description
		{
			get => this.Row?.CommentsI;
			set => this.Row.CommentsI = value;
		}

		public ServerDto()
		{ }

		public ServerDto(GroupDto group)
			: this(group, null)
		{ }

		public ServerDto(TargetsDataSet.ServerRow row)
			: this(null, row)
		{ }

		public ServerDto(GroupDto group, TargetsDataSet.ServerRow row)
		{
			this.Group = group;
			this.Row = row;
		}

		public PortsDto AddNew()
		{
			PortsDto result = new PortsDto(this);
			this.Ports.Add(result);
			return result;
		}

		public void ChangeGroup(GroupDto group)
		{
			this.Group?.Servers.Remove(this);
			group?.Servers.Add(this);

			this.Group = group;
			this.Row.GroupIdI = group?.Row.GroupId;
		}
	}
}