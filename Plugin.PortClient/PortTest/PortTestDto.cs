using System;
using System.Net;
using System.Net.Sockets;
using Plugin.PortClient.Data;

namespace Plugin.PortClient.PortTest
{
	internal class PortTestDto
	{
		public TargetsDataSet.ServerRow ServerRow { get; private set; }
		public TargetsDataSet.PortsRow PortRow { get; private set; }
		public IPAddress Address { get; private set; }
		public UInt16 Port { get; private set; }

		public SocketType Socket
			=> this.PortRow == null
				? SocketType.Stream
				: this.PortRow.SocketTypeI;

		public ProtocolType Protocol
			=> this.PortRow == null
				? ProtocolType.Tcp
				: this.PortRow.ProtocolTypeI;

		public Int32? ConnectTimeout
			=> this.PortRow?.ConnectTimeoutI;

		public PortTestDto(TargetsDataSet.ServerRow serverRow, TargetsDataSet.PortsRow portRow, IPAddress address,UInt16 port)
		{
			this.ServerRow = serverRow;
			this.PortRow = portRow;
			this.Address = address;
			this.Port = port;
		}
	}
}
