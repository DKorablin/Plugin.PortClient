using System;
using System.Collections.Generic;
using System.Threading;

namespace Plugin.PortClient.PortTest
{
	internal class PortTestQueue : IDisposable
	{
		private readonly EventWaitHandle wh = new AutoResetEvent(false);
		private readonly Thread _worker;
		private readonly Object _locker = new Object();
		private readonly Queue<PortTestDto> _tasks = new Queue<PortTestDto>();

		public PortTestQueue()
		{
			_worker = new Thread(Work);
			_worker.Start();
		}

		public void EnqueueTask(PortTestDto portTest)
		{
			lock(_locker)
				_tasks.Enqueue(portTest);

			wh.Set();
		}

		void Work()
		{
			while(true)
			{
				PortTestDto task = null;
				lock(_locker)
					if(_tasks.Count > 0)
					{
						task = _tasks.Dequeue();
						if(task == null)
							return;
					}

				if(task != null)
				{
					//task.
					Console.WriteLine("The task is being performed:" + task);
					Thread.Sleep(1000); // simulation of work...
				} else
					wh.WaitOne();       // There are no more tasks, we are waiting for a signal...
			}
		}

		public void Dispose()
		{
			this.EnqueueTask(null); // Signal the Consumer to complete
			_worker.Join(); // Wait for the Consumer to complete
			wh.Close(); // Release resources
		}
	}
}