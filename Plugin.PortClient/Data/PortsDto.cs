using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Net.Sockets;

namespace Plugin.PortClient.Data
{
	internal class PortsDto : ItemDto
	{
		[Browsable(false)]
		internal TargetsDataSet.PortsRow Row { get; set; }

		[Browsable(false)]
		public ServerDto Server { get; }

		[Category("Connection")]
		[Description("Test port(s). Format: {0},{1},{2}-{4}...")]
		public String Ports
		{
			get => this.Row?.PortsI;
			set => this.Row.PortsI = value;
		}

		[Category("Connection")]
		[Description("Socket type")]
		[DefaultValue(SocketType.Stream)]
		public SocketType SocketType
		{
			get => (this.Row?.SocketTypeI).GetValueOrDefault(SocketType.Stream);
			set => this.Row.SocketTypeI = value;
		}

		[Category("Connection")]
		[Description("Protocol type")]
		[DefaultValue(ProtocolType.Tcp)]
		public ProtocolType ProtocolType
		{
			get => (this.Row?.ProtocolTypeI).GetValueOrDefault(ProtocolType.Tcp);
			set => this.Row.ProtocolTypeI = value;
		}

		[Category("Connection")]
		[Description("Connection timeout in milliseconds to wait for connection to established")]
		public Int32? ConnectTimeout
		{
			get => this.Row?.ConnectTimeoutI;
			set => this.Row.ConnectTimeoutI = value;
		}

		[Category("Data")]
		[Description("Description of the port(s)")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String Description
		{
			get => this.Row?.CommentsI;
			set => this.Row.CommentsI = value;
		}


		public PortsDto(ServerDto server)
			=> this.Server = server;

		public PortsDto(ServerDto server, TargetsDataSet.PortsRow row)
			: this(server)
			=> this.Row = row;

		/// <summary>Sets the state of the node after test is finished</summary>
		/// <param name="newState">Result of the operation</param>
		public void SetNodeStateRecursive(ItemDto.StateType newState)
		{
			ItemDto.StateType state = this.State;
			if(state == ItemDto.StateType.Error)
				return;//Marking server node only if current node state not in the error state (Ex.: MultiPort node)

			state = this.State = newState;
			foreach(PortsDto sibling in this.Server.Ports)
				if(sibling.State != state)
				{
					if(sibling.State == ItemDto.StateType.Pending)
					{
						state = ItemDto.StateType.Pending;
						break;//Mark server as pending. Because something is in process
					} else if(sibling.State == ItemDto.StateType.Error)
						return;//Some of the nodes already failed to the error state.
				}
			this.Server.State = state;
		}
	}
}