using System;
using Xwt;

namespace Actors.UI
{
	public class App
	{
		public static void Run (ToolkitType type)
		{
			Application.Initialize (type);
			using(var w = new MainWindow ()){
				w.Show ();
				Application.Run ();
			}
			Application.Dispose ();
		}
	}
}

