using System;
using ManyConsole;


namespace Actors
{
	public class ServeCommand : ConsoleCommand
	{
		public ServeCommand(){
			IsCommand("serve", "serve a node");
			this.AllowsAnyAdditionalArguments(" node_name port|stop");
		}

		TcpNode node;

		void Cleanup ()
		{
			if (node != null) {
				node.Dispose ();
				node = null;
			}
		}

		public override int Run (string[] remainingArguments)
		{
			if(remainingArguments[0] == "stop"){
				if(node != null){
					Console.WriteLine("stopping " + node.Id);
					Cleanup();
				}
				return 0;
			}
			Cleanup ();

			var name = remainingArguments[0];
			var port = remainingArguments[1].Convert<int>();
			node = new TcpNode(port, name);			
			node.Listen(port, new JsonSerializer(), isLocalOnly: false);
			node.AddBuiltins();

			Console.WriteLine("node " + node.Id + " listening on " + port);
			return 0;
		}


	}
}

