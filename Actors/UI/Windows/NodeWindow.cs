using System;
using Xwt;

namespace Cls.Actors.UI
{
	public class NodeWindow : Window
	{
		public NodeWindow (string name)
		{
			Title = "Node " + name;
			Width = 800;
			Height = 600;
			Content = new HPaned ();
			//this.InitialLocation = WindowLocation.CenterScreen;
		}
	}
}

