using System;
using ManyConsole;


namespace Actors
{
	public class DhtCommand : ConsoleCommand
	{
		public DhtCommand ()
		{
			IsCommand("dht", "use dht");
			AllowsAnyAdditionalArguments();
		}

		TcpNode node;
		IDht dht;

		#region implemented abstract members of ConsoleCommand

		public override int Run (string[] remainingArguments)
		{			
			CreateNode();
			switch(remainingArguments[0]){
			case "join":{
				var port = remainingArguments[1].Convert<int>();
				node.Connect(Environment.MachineName, port);
				dht = node.New<IDht>("System.Dht");
				dht.Join(new ActorId(remainingArguments[2] + ".localhost/System.Dht"));
			}break;

			case "put":{
				dht.Put(remainingArguments[1], remainingArguments[2]);
			}break;
			case "get":{
				Console.WriteLine(dht.Get(remainingArguments[1]));
			}break;
			}


//			var dht = node.Proxy.New<IDht>(); 
//			// join node
//			dht.Put(new Resource("abc"), "def");     
//			Console.WriteLine("found " + dht.Get(new Resource("abc")));
			return 0;
		}


		void CreateNode ()
		{
			if(node != null) return;
			node = new TcpNode(12453,"dht");		
		}
		#endregion
	}
}

