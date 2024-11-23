using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("0481c2a9-7564-4bae-a326-f5436d086259")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=126")]
#else

[assembly: AssemblyTitle("Plugin.PortClient")]
[assembly: AssemblyDescription("Ports/Sockets test client")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.PortClient")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2017-2024")]
#endif