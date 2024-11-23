using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Plugin.PortClient.Data
{
	internal class ItemDto
	{
		public enum TreeImageList
		{
			Server = 0,
			Port = 1,
			Folder = 2,
		}

		public enum StateType
		{
			Default = 0,
			Pending = 1,
			Error = 2,
			Success = 3,
		}

		[Browsable(false)]
		public StateType State { get; set; }

		internal static String FixPorts(String ports)
		{
			if(String.IsNullOrEmpty(ports))
				throw new ArgumentNullException(nameof(ports), "Empty ports string");

			String[] parts = ports.Split(',');
			List<String> result = new List<String>(parts.Length);

			foreach(String part in parts)
			{
				String[] portsArr = part.Split('-');
				if(portsArr.Length == 1)
				{
					UInt16 check = UInt16.Parse(portsArr[0]);
					result.Add(check.ToString());
				} else if(portsArr.Length == 2)
				{
					UInt16 checkMin = portsArr[0] == String.Empty ? UInt16.MinValue : UInt16.Parse(portsArr[0]);
					UInt16 checkMax = portsArr[1] == String.Empty ? UInt16.MaxValue : UInt16.Parse(portsArr[1]);
					if(checkMin >= checkMax)
						throw new ArgumentException("Минимальное значение должно быть больше максимального. Порты: " + part);
					result.Add(String.Format("{0}-{1}",
						checkMin == UInt16.MinValue ? String.Empty : checkMin.ToString(),
						checkMax == UInt16.MaxValue ? String.Empty : checkMax.ToString()));
				} else
					throw new ArgumentException("Невозможно разобрать значение. Порты: " + part);
			}

			return String.Join(",", result.ToArray());
		}

		internal static IEnumerable<UInt16> GetPorts(String portsFixed)
		{
			String[] parts = portsFixed.Split(',');
			foreach(String part in parts)
			{
				String[] portsArr = part.Split('-');
				if(portsArr.Length == 1)
					yield return UInt16.Parse(portsArr[0]);
				else if(portsArr.Length == 2)
				{
					UInt16 min = portsArr[0] == String.Empty ? UInt16.MinValue : UInt16.Parse(portsArr[0]);
					UInt16 max = portsArr[1] == String.Empty ? UInt16.MaxValue : UInt16.Parse(portsArr[1]);
					while(min <= max)
						yield return min++;
				}
			}
		}
	}
}
