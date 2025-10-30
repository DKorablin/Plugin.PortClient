using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Plugin.PortClient.Data
{
	internal class GroupDto : ItemDto
	{
		private List<ServerDto> _servers;

		public String Name
		{
			get => this.Row?.Name;
			set
			{
				if(this.Row != null)
					this.Row.Name = value;
			}
		}

		[Browsable(false)]
		internal TargetsDataSet.GroupRow Row { get; set; }

		[Browsable(false)]
		public List<ServerDto> Servers
		{
			get
			{
				if(this._servers == null && this.Row != null)
					this._servers = new List<ServerDto>(Array.ConvertAll(this.Row.GetServerRows(), (server) => new ServerDto(this, server)));
				return this._servers;
			}
		}

		public GroupDto()
		{ }

		public GroupDto(TargetsDataSet.GroupRow row)
			=> this.Row = row;

		public ServerDto AddNew()
		{
			ServerDto result = new ServerDto(this);
			this.Servers.Add(result);
			return result;
		}
	}
}
