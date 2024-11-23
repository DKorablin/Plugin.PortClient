using System;
using System.Collections.Generic;
using System.Threading;

namespace Plugin.PortClient.PortTest
{
	internal class PortTestQueue : IDisposable
	{
		private readonly EventWaitHandle wh = new AutoResetEvent(false);
		private Thread worker;
		private readonly Object locker = new Object();
		Queue<PortTestDto> tasks = new Queue<PortTestDto>();

		public PortTestQueue()
		{
			worker = new Thread(Work);
			worker.Start();
		}

		public void EnqueueTask(PortTestDto portTest)
		{
			lock(locker)
				tasks.Enqueue(portTest);

			wh.Set();
		}

		void Work()
		{
			while(true)
			{
				PortTestDto task = null;
				lock(locker)
					if(tasks.Count > 0)
					{
						task = tasks.Dequeue();
						if(task == null)
							return;
					}

				if(task != null)
				{
					//task.
					Console.WriteLine("Выполняется задача: " + task);
					Thread.Sleep(1000); // симуляция работы...
				}
				else
					wh.WaitOne();       // Больше задач нет, ждем сигнала...
			}
		}

		public void Dispose()
		{
			EnqueueTask(null);      // Сигнал Потребителю на завершение
			worker.Join();          // Ожидание завершения Потребителя
			wh.Close();             // Освобождение ресурсов
		}
	}
}
