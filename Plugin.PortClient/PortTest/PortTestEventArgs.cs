using System;

namespace Plugin.PortClient.PortTest
{
	internal class PortTestEventArgs : EventArgs
	{
		public PortTestDto Dto { get; private set; }

		public Boolean IsConnected { get; private set; }

		public Exception Exception { get; private set; }

		public Boolean IsContinue { get; set; } = true;
		public TimeSpan Elapsed { get; }

		private PortTestEventArgs(PortTestDto dto)
			=> this.Dto = dto;

		public PortTestEventArgs(PortTestDto dto, Boolean isConnected, TimeSpan elapsed)
			: this(dto)
		{
			this.IsConnected = isConnected;
			this.Elapsed = elapsed;
		}

		public PortTestEventArgs(PortTestDto dto, Exception exc)
			: this(dto)
		{
			this.IsConnected = false;
			this.Exception = exc;
		}
	}
}
