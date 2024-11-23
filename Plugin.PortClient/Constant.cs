using System;
using System.Reflection;

namespace Plugin.PortClient
{
	internal static class Constant
	{
		public const String PSScriptArgs2 = "tnc {0} {1}";

		public static String FormatMessage(String template, Object payload)
		{
			PropertyInfo[] properties = payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach(PropertyInfo property in properties)
				if(property.CanRead)
				{
					Object value = property.GetValue(payload, null);
					String strValue = value == null ? String.Empty : value.ToString();
					template = template.Replace('{' + property.Name + '}', strValue);
				}
			return template;
		}
	}
}