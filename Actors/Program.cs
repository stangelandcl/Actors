using System;
using Serialization;

namespace Actors
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using(var server = new ActorServer("127.0.0.1", 13000)){

				using(var channel = new ActorChannel("localhost", 13000)){
					var newId = channel.GetRemoteActorId("NewInstance");
					var echoId = channel.SendReceive<int>(newId, "new", "Actors.EchoActor", Guid.NewGuid().ToString());					
					var reply = channel.SendReceive<string>(echoId, "echo", "hi");
					Console.WriteLine(reply);
					reply = channel.SendReceive<string>(echoId, "echo", "hi");
					Console.WriteLine(reply);				

					var echo = channel.GetRemoteProxy(echoId);
					Console.WriteLine(echo.echo("hi"));
				}
			}
					
		}
	}
}
