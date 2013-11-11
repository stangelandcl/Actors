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
		public MainWindowViewModel(){
			Node.Connected += (arg1, arg2) => ConnectionAdded.FireEventAsync(arg1.ToString(), arg2);
		}

		public TcpNode Node = new TcpNode();
		//public List<string> Nodes = new List<string>();
		public event Action<string, NodeId> ConnectionAdded;
	//	List<TcpNode> nodes = new List<TcpNode>();

		public void Connect(string conn){
			var c = Node.Connect(conn);
		//	Nodes.Add(NewConnection);
		}

		public void Listen(int port){
			var node = new TcpNode (port);
			node.Listen (port);
			Node.Connect ("localhost:" + port);
		//	nodes.Add (node);
		}
	}
}

