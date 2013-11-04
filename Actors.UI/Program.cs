using System;
using Xwt;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace Cls.Actors.UI
{
	public class Program
	{
		public static void Main (string[] args)
		{
			//LoadPlatformLibrary ();
			App.Run (ToolkitType.Gtk);
		}

		static void LoadPlatformLibrary ()
		{
			Assembly assembly;
			var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
			switch (Environment.OSVersion.Platform) {
			case PlatformID.MacOSX:
			case PlatformID.Unix:
				assembly = Assembly.LoadFrom (Path.Combine(path, "Xwt.Gtk.dll"));
				break;
			default:
				assembly = Assembly.LoadFrom (Path.Combine(path, "Xwt.WPF.dll"));
				break;
			}

			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => 
			{
				if(args.Name == assembly.FullName)
					return assembly;
				return null;
			};
		}
	}
}

