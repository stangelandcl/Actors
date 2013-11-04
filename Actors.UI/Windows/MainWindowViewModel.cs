using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using System.Web;

using System.Collections.ObjectModel;
using Cls.Actors;
using Cls.Extensions;

namespace Cls.Actors.UI
{
	public class MainWindowViewModel 
	{	
		TcpNode node = new TcpNode();
		public string NewConnection {get; set;}
		public List<string> Nodes = new List<string>();
		public event Action ConnectionAdded;

		public void Connect(){
			var conn = node.Connect(NewConnection);
			Nodes.Add(NewConnection);
			ConnectionAdded.FireEvent();
		}
	}
}

