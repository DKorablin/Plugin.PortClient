using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Plugin.PortClient.Data;

namespace Plugin.PortClient.PortTest
{
	internal class PortTestInstance
	{
		private abstract class CallbackBase
		{
			public PortTestDto Dto { get; }

			public abstract TimeSpan Elapsed { get; }

			protected CallbackBase(PortTestDto dto)
				=> this.Dto = dto;

			public abstract Boolean FinishTest();

		}

		private sealed class PingCallback : CallbackBase
		{
			public class FakeState : IAsyncResult
			{
				public Boolean IsCompleted => true;

				public WaitHandle AsyncWaitHandle => null;

				public Boolean CompletedSynchronously => true;

				public Object AsyncState { get; set; }
			}

			private PingReply _reply;
			private readonly Stopwatch _elapsed = new Stopwatch();
			private AsyncCallback _callback;

			private Ping Pong { get; }

			public override TimeSpan Elapsed => this._elapsed.Elapsed;

			public PingCallback(PortTestDto dto)
				: base(dto)
				=> this.Pong = new Ping();

			public void BeginConnect(AsyncCallback callback)
			{
				this._callback = callback;
				this.Pong.PingCompleted += this.Pong_PingCompleted;

				this._elapsed.Start();
				this.Pong.SendAsync(base.Dto.Address,
					base.Dto.ConnectTimeout.GetValueOrDefault(5000),//5 seconds same as ping.exe
					null);
			}

			private void Pong_PingCompleted(Object sender, PingCompletedEventArgs e)
			{
				this._elapsed.Stop();
				this._reply = e.Reply;// e.Reply.Status == IPStatus.Success;
				FakeState ar = new FakeState() { AsyncState = this, };
				this._callback.Invoke(ar);
			}

			public override Boolean FinishTest()
			{
				this.Pong.Dispose();
				return this._reply != null && this._reply.Status == IPStatus.Success;
			}
		}

		private class SocketCallback : CallbackBase
		{
			private Boolean _inProgress = true;
			private readonly Stopwatch _elapsed = new Stopwatch();
			private Socket Socket { get; }
			public override TimeSpan Elapsed => this._elapsed.Elapsed;

			public Boolean IsConnected => this.Socket.Connected;

			public SocketCallback(PortTestDto dto)
				: base(dto)
			{
				this.Socket = new Socket(this.Dto.Address.AddressFamily, this.Dto.Socket, this.Dto.Protocol);
			}

			public IAsyncResult BeginConnect(AsyncCallback callback)
			{
				this._elapsed.Start();
				IAsyncResult result = this.Socket.BeginConnect(this.Dto.Address, this.Dto.Port, callback, this);
				if(this.Dto.ConnectTimeout != null)
					result.AsyncWaitHandle.WaitOne(this.Dto.ConnectTimeout.Value, true);
				return result;
			}

			public override Boolean FinishTest()
			{
				this._elapsed.Stop();
				Boolean result = false;
				if(this._inProgress)
				{
					this._inProgress = false;
					result = this.Socket.Connected;
					this.Socket.Close();
				}
				return result;
			}
		}

		private volatile Boolean _cancellationPending = false;

		public Boolean IsSent { get; set; } = false;

		public ManualResetEvent FinishedEvent { get; private set; }

		public Int32 TestsCount => this._instances.Count;

		private readonly Object _instanceLock = new Object();

		private Dictionary<PortTestDto, SocketCallback> _instances;

		public event EventHandler<PortTestEventArgs> OnPortTestFinished;
		public event EventHandler<EventArgs> OnTestFinished;

		public PortTestInstance()
		{
			this.FinishedEvent = new ManualResetEvent(false);
			this.Reset();
		}

		public void Reset()
		{
			this.IsSent = false;
			this._instances = new Dictionary<PortTestDto, SocketCallback>();
			this.FinishedEvent.Reset();
		}

		public void Cancel()
		{
			this._cancellationPending = true;
			SocketCallback[] sockets = this._instances.Values.ToArray();
			this._instances = null;

			for(Int32 loop = 0; loop < sockets.Length; loop++)
				sockets[loop].FinishTest();
			this.TestFinished();
		}

		public void CheckPorts(params TargetsDataSet.ServerRow[] serverRows)
		{
			foreach(TargetsDataSet.ServerRow row in serverRows)
				this.CheckPorts(row, (TargetsDataSet.PortsRow)null);
		}

		public void CheckPorts(TargetsDataSet.ServerRow serverRow, TargetsDataSet.PortsRow portRow)
		{
			_ = serverRow ?? throw new ArgumentNullException(nameof(serverRow));

			foreach(PortTestDto dto in serverRow.GetServerTests(portRow))
				if(!this.CheckPortAsync(dto))
				{
					this.FinishedEvent.Set();
					break;
				}
		}

		private Boolean CheckPortAsync(PortTestDto dto)
		{
			if(this._cancellationPending)//Cancelled
				return false;

			Boolean isSuccess = false;
			try
			{
				SocketCallback payload = new SocketCallback(dto);
				this._instances.Add(dto, payload);
				payload.BeginConnect(this.CheckPortAsyncCallback);
				isSuccess = true;
			} catch(SocketException exc)
			{
				this.PortTestFinished(new PortTestEventArgs(dto, exc));
				isSuccess = false;
			}
			return isSuccess;
		}

		private void CheckPortAsyncCallback(IAsyncResult result)
		{
			if(this._cancellationPending)
				return;

			CallbackBase payload = (CallbackBase)result.AsyncState;
			Boolean isConnected = payload.FinishTest();

			this.PortTestFinished(new PortTestEventArgs(payload.Dto, isConnected, payload.Elapsed));
		}

		private void PortTestFinished(PortTestEventArgs args)
		{
			lock(this._instanceLock)
				if(!this._instances.Remove(args.Dto))
					throw new InvalidOperationException();

			if(this._cancellationPending)
				return;

			this.OnPortTestFinished?.Invoke(this, args);
			if(!args.IsContinue)
				this.Cancel();
			else if(this._instances.Count == 0)
				this.TestFinished();
		}

		private void TestFinished()
		{
			this.FinishedEvent.Set();
			this.OnTestFinished?.Invoke(this, EventArgs.Empty);
		}
	}
}