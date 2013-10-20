using System;

namespace Actors.Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var node = new Node();
			var server = node.Listen("127.0.0.1", 18222);
			node.Add<EchoActor>("echo");

			var n2 = new Node();
			var conn = n2.Connect("127.0.0.1", 18222);

			var result = n2.SendReceive<string>("localhost/echo", "echo", "hi"); 
			Console.WriteLine(result);
		}
	}
}
