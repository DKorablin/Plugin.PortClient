using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlphaOmega.Bll;

namespace Plugin.PortClient.Data
{
	internal class TargetsBll : BllBase<TargetsDataSet, TargetsDataSet.ServerRow>
	{
		public event EventHandler<EventArgs> OnFileChanged;

		/// <summary>Создать экземпляр настроек и загрузить настройки из провайдера плагинов</summary>
		/// <param name="plugin">Плагин</param>
		public TargetsBll()
			: base(0)
		{ }

		public TargetsBll(Stream stream)
			: base(0)
			=> base.DataSet.ReadXml(stream);

		public TargetsBll(String filePath)
			: base(filePath, 0)
			=> this.Load(false);

		public override void Load(bool isReload)
		{
			base.Load(isReload);
			if(isReload)
				this.OnFileChanged?.Invoke(this, EventArgs.Empty);
		}

		public void SaveAsXml(Stream stream)
			=> base.DataSet.WriteXml(stream);

		public void SaveAsXml(String filePath)
		{
			using(FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				this.SaveAsXml(stream);
		}

		public void SaveAsPowerShell(String filePath)
		{
			using(FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			using(StreamWriter writer = new StreamWriter(stream))
			{
				foreach(TargetsDataSet.ServerRow server in base.DataSet.Server)
					foreach(TargetsDataSet.PortsRow ports in server.GetPortsRows())
						foreach(UInt16 port in ports.GetPorts())
							writer.WriteLine(String.Format("tnc {0} {1}", server.HostAddress, port));
			}
		}

		/// <summary>Получить список всех хостов в настройках</summary>
		/// <returns>Список хостов в настройках</returns>
		public IEnumerable<String> GetHostAddress()
			=> base.DataSet.Server.Select(p => p.HostAddress);

		/// <summary>Получить ряд информации о сервере по идентификатору сервера</summary>
		/// <param name="serverId">Идентификатор сервреа</param>
		/// <returns>Ряд сервера</returns>
		public TargetsDataSet.ServerRow GetServerRow(Int32 serverId)
			=> base.DataSet.Server.FirstOrDefault(p => p.ServerId == serverId);

		/// <summary>Получить ряд информации о серверу по адресу сервера</summary>
		/// <param name="hostAddress">Адрес сервера</param>
		/// <returns>Ряд сервера</returns>
		public TargetsDataSet.ServerRow GetServerRow(String hostAddress)
		{
			if(String.IsNullOrEmpty(hostAddress))
				throw new ArgumentNullException(nameof(hostAddress));

			return base.DataSet.Server.FirstOrDefault(p => p.HostAddress == hostAddress);
		}

		public ItemDto[] GetTree()
			=> base.DataSet.Group.Select(group => new GroupDto(group)).Cast<ItemDto>()
				.Union(base.DataSet.Server.Where(s => s.GroupIdI == null).Select(server => new ServerDto(server)).Cast<ItemDto>())
				.ToArray();

		public void MoveNode(GroupDto group, ServerDto server)
			=> server.ChangeGroup(group);

		public TargetsDataSet.GroupRow ModifyGroupRow(TargetsDataSet.GroupRow groupRow,String name)
		{
			TargetsDataSet.GroupRow result = base.DataSet.Group.ModifyGroup(groupRow, name);
			return result;
		}

		/// <summary>Поменять данные по серверу</summary>
		/// <param name="serverId">Идентификатор сервера или null, если новая</param>
		/// <param name="hostAddress">Адрес сервера</param>
		/// <returns>Данные по ряду добавленного в датасет</returns>
		public TargetsDataSet.ServerRow ModifyServerRow(TargetsDataSet.GroupRow groupRow, TargetsDataSet.ServerRow serverRow, String hostAddress)
		{
			String[] hostAddressParts = hostAddress.Split(':');
			UInt16 port = 0;
			if(hostAddressParts.Length > 1 && UInt16.TryParse(hostAddressParts[1], out port))
				hostAddress = hostAddressParts[0];
			/*IPAddress ip = null;
			if(hostAddressParts.Length > 1 && UInt16.TryParse(hostAddressParts[1], out port)
				&& IPAddress.TryParse(hostAddressParts[0], out ip)
				&& ip.AddressFamily == AddressFamily.InterNetwork)
				hostAddress = hostAddressParts[0];//Пытаюсь распарсить IP адрес с портом. Пример - 127.0.0.1:80
			else
				ip = null;*/

			TargetsDataSet.ServerRow result = base.DataSet.Server.ModifyServerRow(groupRow, serverRow, hostAddress);
			if(/*ip != null && */port > 0)
				base.DataSet.Ports.ModifyPort(result, null, port.ToString());
			return result;
		}

		public TargetsDataSet.PortsRow ModifyPortRow(TargetsDataSet.ServerRow serverRow, TargetsDataSet.PortsRow portRow, String portTitle)
		{
			TargetsDataSet.PortsRow result = base.DataSet.Ports.ModifyPort(serverRow, portRow, portTitle);
			return result;
		}

		public void RemovePort(TargetsDataSet.PortsRow row)
		{
			_ = row ?? throw new ArgumentNullException(nameof(row));

			base.DataSet.Ports.RemovePortsRow(row);
		}

		public void RemoveServer(TargetsDataSet.ServerRow row)
		{
			_ = row ?? throw new ArgumentNullException(nameof(row));

			TargetsDataSet.PortsRow[] ports = base.DataSet.Ports.Where(p => p.ServerId == row.ServerId).ToArray();
			for(Int32 loop = 0; loop < ports.Length; loop++)
				this.RemovePort(ports[loop]);

			base.DataSet.Server.RemoveServerRow(row);
		}

		public void RemoveGroup(TargetsDataSet.GroupRow row)
		{
			_ = row ?? throw new ArgumentNullException(nameof(row));

			TargetsDataSet.ServerRow[] servers = base.DataSet.Server.Where(p => p.GroupIdI == row.GroupId).ToArray();
			for(Int32 loop = 0; loop < servers.Length; loop++)
				this.RemoveServer(servers[loop]);

			base.DataSet.Group.RemoveGroupRow(row);
		}
	}
}