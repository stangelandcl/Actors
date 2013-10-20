using System;

namespace Actors.Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var router = new TcpRouter();

			var peer = new TcpWorld();
			peer.Listen("127.0.0.1", 13000);
			peer.Add(new Actor());

			var client = new TcpWorld();
			client.Connect("127.0.0.1", 13000);
			var echoActor = client.Resolve("echo");
		

			var router = new MailRouter(new TcpRouter());
			var servers = new TcpServers();		
			servers.Listen("127.0.0.1", 13000);
			router.AddGlobal(new Actor());
			router.AddPerServer(()=>new Actor());
			router.AddPerServer(server, new Actor());
			router.AddPerSession(()=> new Actor());
			router.AddPerSession(session, new Actor());
			using(var server = servers.Listen("127.0.0.1", 13000)){
				var dns = new NameResolver();
				var addr = dns.NameToAddress("localhost/13000/NewInstance");
				var sender = router.GetSender(addr);
				sender.Send(addr, "abc", "123");
			}

		


			using(var server = new ActorServer("127.0.0.1", 13000)){

				using(var channel = new ActorChannel("localhost", 13000)){
					var newId = channel.GetRemoteActorId("NewInstance");
					var echoId = channel.SendReceive<ActorId>(newId, "new", "Actors.EchoActor", Guid.NewGuid().ToString());					
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
