using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Plugin.PortClient.Data;

namespace Plugin.PortClient
{
	public class PluginSettings : INotifyPropertyChanged
	{
		private static class Constants
		{
			public const String MessageFormat = "{IpAddress}:{Port} {ServerComments}{PortComments} {Status}";
			/// <summary>The name of the file in which all project settings are saved</summary>
			public const String ProjectFileName = "ServersSettings.xml";
			internal const String Project = "Project";
		}

		private readonly PluginWindows _plugin;
		private Boolean _logSocketExceptions = false;
		private String _messageFormat = null;
		private LogPortsType _logType = LogPortsType.All;

		public enum LogPortsType
		{
			All,
			Opened,
			Closed,
		}

		[Category("Diagnostics")]
		[DisplayName("Log errors")]
		[Description("Log connect errors while testing ports")]
		[DefaultValue(false)]
		public Boolean LogSocketExceptions
		{
			get => this._logSocketExceptions;
			set => this.SetField(ref this._logSocketExceptions, value, nameof(this.LogSocketExceptions));
		}

		[Category("Diagnostics")]
		[DisplayName("Log port state")]
		[Description("Log checked port state")]
		[DefaultValue(LogPortsType.All)]
		public LogPortsType LogType
		{
			get => this._logType;
			set => this.SetField(ref this._logType, value, nameof(this.LogType));
		}

		[Category("Diagnostics")]
		[DisplayName("Log message format")]
		[Description(@"Message that will be shown in the output window on test finished. Values:
{IpAddress} - Server IP address
{Port} - Server port
{ServerName} - Server name
{ServerComments} - Comments on the server node
{PortComments} - Comments on the port node
{IsConnected} - Is connection succeeded
{Status} - Check port status
{Elapsed} - Time spent testing")]
		[DefaultValue(Constants.MessageFormat)]
		public String MessageFormat
		{
			get => this._messageFormat ?? Constants.MessageFormat;
			set
			{
				String v = String.IsNullOrEmpty(value?.Trim()) ? null : value.Trim();
				this.SetField(ref this._messageFormat, v, nameof(this.MessageFormat));
			}
		}

		/// <summary>Displayed columns in the list</summary>
		[Browsable(false)]
		public String ColumnVisible { get; set; }

		internal PluginSettings(PluginWindows plugin)
			=> this._plugin = plugin;

		/// <summary>Internal project</summary>
		internal TargetsBll LoadProject()
		{
			TargetsBll result = null;
			using(Stream stream = this._plugin.HostWindows.Plugins.Settings(this._plugin).LoadAssemblyBlob(Constants.ProjectFileName))
				if(stream != null)
					result = new TargetsBll(stream);
			if(result == null)
				result = new TargetsBll();
			return result;
		}

		internal void SaveProject(TargetsBll project)
		{
			if(!project.IsRowExists)
				this._plugin.HostWindows.Plugins.Settings(this._plugin).RemoveAssemblyBlob(Constants.ProjectFileName);
			else
				using(MemoryStream stream = new MemoryStream())
				{
					project.SaveAsXml(stream);
					this._plugin.HostWindows.Plugins.Settings(this._plugin).SaveAssemblyBlob(Constants.ProjectFileName, stream);
				}
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Constants.Project));
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean SetField<T>(ref T field, T value, String propertyName)
		{
			if(EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
		#endregion INotifyPropertyChanged
	}
}