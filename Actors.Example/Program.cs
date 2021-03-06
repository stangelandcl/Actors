using System;
using System.Threading;
using Cls.Serialization;
using Cls.Actors.Examples.Clients;
using Cls.Extensions;

namespace Cls.Actors.Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			serializer = Defaults.Serializer;
            if (args[0] == "server")
            {
                new MainClass().RunAsServer();
            }
            else if(args[0] == "client")
            {
                new MainClass().RunAsClient();
            }
            else if (args[0] == "both")
            {
                new MainClass().RunAsBoth();
            }
            else
            {
				Test();
                //Console.WriteLine("choose server, client, or both");
            }
		}

        static ISerializer serializer;

			private static void Test(){			                               
				var node = Defaults.Node();                                                 
				var dht = node.New<IDht>("System.Dht") ;                                      
				dht.Put("abc", 123);
			}

        private void RunAsBoth()
        {
            // a node is like an erlang node. it is like a VM. like its own world
            using (var node = new TcpNode(18223))
            using (var server = node.Listen(18222, serializer))
            {
                // add actor, an actor is like an object. it can send & receive messages
                node.Add(new BandwidthActor());
                node.Add(new EchoActor());
                node.Add(new PingActor());
				var mainDht = new DhtActor();
                node.Add(mainDht);
				node.Add(new FileCopyActor());
                for (int i = 0; i < 1000; i++)
                {
                    var actor = new DhtActor("dht" + i);
                    node.Add(actor);
					actor.Dht.Join(mainDht.Id);
                }
                           
                var echo = node.New<IEcho>("System.Echo");
                Console.WriteLine(echo.Echo("hey dude"));

				var dht = node.New<IDht>("dht1");   
				dht.Put("abc", "def");     
				Console.WriteLine("found " + dht.Get("abc"));
				var dht2 = node.New<IDht>("dht85");
				int j=0;
                while (true)
                {			
					var result = dht2.Get<string>("abc");
					Console.WriteLine("result = " + result);
					if(result == "def") break;
                    Console.WriteLine((++j) + "  " + dht.Get("abc") );
                    Thread.Sleep(1000);
                }
                Console.WriteLine("def");

				

                Console.WriteLine("press a key to quit");
                Console.ReadKey();              
            }
        }        

        private void RunAsClient()
        {
            string machine = "localhost";
            // create a node
            using(var node = new TcpNode(18222, "client"))
            // connect to an endpoint which is in this case a separate node. 
            // so now we can route messages from actors in this node to the
            // other node
/* optional */ //           using (var conn = node.AddConnection("127.0.0.1", 18222, serializer))
            using (var conn = node.Connect(machine, 18222, serializer))
            {
                // send async but receive sync. send to localhost/echo which happens to
                // be in the other node
				var result = node.Default.SendReceive<string>("server.{0}/System.Echo".FormatWith(machine), "Echo", "hi");
                // print response
                Console.WriteLine(result);


                // DynamicProxy wraps a remote actor.
				var echo = node.New<IEcho>("server.{0}/System.Echo".FormatWith(machine));
                // this is the same as SendReceive<string>("localhost/System.Echo", "echo", "hey dude");
                var r2 = echo.Echo("hey dude");
                // print response
                Console.WriteLine(r2);
                r2 = echo.Echo("hey dude");
                // print response
                Console.WriteLine(r2);

				var ping = node.New<IPing>("server.{0}/System.Ping".FormatWith(machine));
                var pong = ping.Ping(new byte[1024]);
                Console.WriteLine("ping-pong " + pong.Length);

                var client = new PingClient(ping);
                Console.WriteLine("delay = " + client.Ping(10));

				var bandwidth = node.New<IBandwidth>("server.{0}/System.Bandwidth".FormatWith(machine));
                var bw = new BandwidthClient(bandwidth, ping);
                Console.WriteLine("bandwidth = " + bw.Test());

//                using (var dht = new DhtClient(node.Proxy.New<IByteDht>(new ActorId("localhost", "System.Dht")), new JsonSerializer()))
//                {
//                    dht.Add("abc", "123");
//                    dht.Subscribe(DhtOperation.All, ".*");
//                    dht.KeyMatch += (operation, key) =>
//                    {
//                        Console.WriteLine("DHT callback " + operation + " key=" + key);
//                    };
//                    dht.Add("def", "456");
//                    var x = dht.Get<string>("abc");
//					Console.WriteLine("DHT result " + x);
//                }
				//Thread.Sleep(100000);
                node.AddBuiltins();
				var dhtLocal = node.New<IDht>("System.Dht");
				dhtLocal.Join(ActorId.FromUrl("server.{0}/System.Dht".FormatWith(machine)) );
				var dht = node.New<IDht>("server.{0}/System.Dht".FormatWith(machine));

                dhtLocal.Put("abc", "def");

                Console.WriteLine("press a key to test dht get");
                Console.ReadKey();
                Console.WriteLine("found " + dht.Get("abc"));
                Console.WriteLine("found " + dhtLocal.Get("abc"));

                Console.ReadKey();
                using (var shell = new ConsoleClientActor("cmd.exe-client"))
                {
                    node.Add(shell);
					var proxy = node.New<IShell>("server.{0}/System.Shell".FormatWith(machine));
                    var remoteId = proxy.RunConsole("cmd.exe", new string[0], shell.Id);
                    node.Link(shell.Id, remoteId);
                    while (shell.IsAlive)
                        Thread.Sleep(10);
                }

                //var console = n2.GetProxy("localhost/cmd.exe");
                //using (var realConsole = new ConsoleClientActor("cmd2.exe"))
                //{
                //    n2.Add(realConsole);
                //    n2.Link(realConsole.Box.Id, ((IDynamicProxy)console).Proxy.Remote);
                //    console.SendAttach(realConsole.Box.Id);
                //    while (realConsole.IsAlive)
                //        Thread.Sleep(10);
                //}

            } // close the connection and remove from n2. Now there is no route to the first node and the echo actor
            // TODO: set default ports so given a computer name we can try to connect to a few ports if someone
            // tries to send a message to one of those actors then we could avoid this explicit connection and 
            // let the node/VM handle it.

            //TODO: change router to directly put messages in mailbox using function without going through tcp sockets
            // if the other actor is in the same node. This would make it feasible to use actors for some things inside a single 
            // process/node/program
        }

        void RunAsServer()
        {
            // a node is like an erlang node. it is like a VM. like its own world
            using (var node = new TcpNode(18223, "server"))
            using(var server = node.Listen(18222, serializer, isLocalOnly: false))
            {                              
                // add actor, an actor is like an object. it can send & receive messages
                node.AddBuiltins();
               // node.Add(new DhtActor(ProxyFactory.New<IDhtBackend>()));
                //using (var cmd = new ConsoleProcessActor("cmd.exe", "cmd.exe"))
                //{
                //    node.Add(cmd);
                //    while (cmd.IsAlive)
                //        Thread.Sleep(10);                  
                //}
                Thread.Sleep(Timeout.Infinite);
            }
        }
	}
}
