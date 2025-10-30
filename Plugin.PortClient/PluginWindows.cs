using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Plugin.PortClient.Data;
using Plugin.PortClient.PortTest;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.PortClient
{
	public class PluginWindows : IPlugin, IPluginSettings<PluginSettings>
	{
		private PluginSettings _settings;

		private TraceSource _trace;

		private Dictionary<String, DockState> _documentTypes;

		internal TraceSource Trace => this._trace ?? (this._trace = PluginWindows.CreateTraceSource<PluginWindows>());

		internal IHostWindows HostWindows { get; }

		/// <summary>Settings for interaction from the host</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Settings for interaction from the plugin</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings(this);
					this.HostWindows.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		private IMenuItem MenuTest { get; set; }

		private IMenuItem MenuNetworkTest { get; set; }

		private IMenuItem MenuPortTest { get; set; }

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(PanelPortTestClient).ToString(), DockState.DockRightAutoHide },
						//{ typeof(DocumentSvcTestMethod).ToString(), DockState.Document },
					};
				return this._documentTypes;
			}
		}

		public PluginWindows(IHostWindows hostWindows)
			=> this.HostWindows = hostWindows ?? throw new ArgumentNullException(nameof(hostWindows));

		/// <summary>Get a list of all hosts in the settings</summary>
		/// <returns>List of hosts in the settings</returns>
		public IEnumerable<String> GetHostAddress()
		=> this.Settings.LoadProject().GetHostAddress();

		/// <summary>Check all ports from the settings for a specific host</summary>
		/// <param name="hostAddress">Host to check</param>
		/// <returns>One of the ports on the server from the server settings is open</returns>
		public Boolean CheckPorts(String hostAddress)
		{
			if(String.IsNullOrEmpty(hostAddress))
				throw new ArgumentNullException(nameof(hostAddress));

			TargetsBll project = this.Settings.LoadProject();
			TargetsDataSet.ServerRow serverRow = project.GetServerRow(hostAddress);
			if(serverRow == null)
				throw new InvalidOperationException($"The {nameof(serverRow)} is not found by specified {nameof(hostAddress)}");

			Boolean result = true;
			PortTestInstance instance = new PortTestInstance();
			instance.CheckPorts(serverRow, (TargetsDataSet.PortsRow)null);
			instance.FinishedEvent.WaitOne();

			return result;
		}

		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IMenuItem menuTools = this.HostWindows.MainMenu.FindMenuItem("Tools");
			if(menuTools == null)
			{
				this.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'Tools' not found");
				return false;
			}

			this.MenuTest = menuTools.FindMenuItem("Test");
			if(this.MenuTest == null)
			{
				this.MenuTest = menuTools.Create("Test");
				this.MenuTest.Name = "Tools.Test";
				menuTools.Items.Add(this.MenuTest);
			}

			this.MenuNetworkTest = this.MenuTest.FindMenuItem("Network");
			if(this.MenuNetworkTest == null)
			{
				this.MenuNetworkTest = this.MenuTest.Create("Network");
				this.MenuNetworkTest.Name = "Tools.Test.Network";
				this.MenuTest.Items.Add(this.MenuNetworkTest);
			}

			this.MenuPortTest = this.MenuNetworkTest.Create("Port Test Client");
			this.MenuPortTest.Name = "Tools.Test.Network.PortTestClient";
			this.MenuPortTest.Click += (sender, e) => this.CreateWindow(typeof(PanelPortTestClient).ToString(), true);
			this.MenuNetworkTest.Items.AddRange(new IMenuItem[] { this.MenuPortTest, });
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(this.MenuPortTest != null)
				this.HostWindows.MainMenu.Items.Remove(this.MenuPortTest);
			if(this.MenuNetworkTest != null && this.MenuNetworkTest.Items.Count == 0)
				this.HostWindows.MainMenu.Items.Remove(this.MenuNetworkTest);
			if(this.MenuTest != null && this.MenuTest.Items.Count == 0)
				this.HostWindows.MainMenu.Items.Remove(this.MenuTest);
			return true;
		}

		internal IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
			=> this.DocumentTypes.TryGetValue(typeName, out DockState state)
				? this.HostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
				: null;

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}
	}
}