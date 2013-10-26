using System;
using Actors.Examples;
using Actors.Examples.Clients;
using Actors.Examples.Actors;
using RemoteConsole;
using System.Threading;
using Actors.Network.Tcp;
using Serialization;
using Actors.Dht;
using Actors.Builtins.Actors.Dht;

namespace Actors.Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            serializer = new JsonSerializer();
            if (args[0] == "server")
            {
                new MainClass().RunAsServer();
            }
            else
            {
                new MainClass().RunAsClient();
            }
		}

        static ISerializer serializer;

        private void RunAsClient()
        {
            // create a node
            using(var node = new TcpNode(18222))
            // connect to an endpoint which is in this case a separate node. 
            // so now we can route messages from actors in this node to the
            // other node
/* optional */ //           using (var conn = node.AddConnection("127.0.0.1", 18222, serializer))
            {
                // send async but receive sync. send to localhost/echo which happens to
                // be in the other node
                var result = node.Default.SendReceive<string>("localhost/System.Echo", "Echo", "hi");
                // print response
                Console.WriteLine(result);


                // DynamicProxy wraps a remote actor.
                var echo = node.Proxy.New<IEcho>("localhost/System.Echo");
                // this is the same as SendReceive<string>("localhost/System.Echo", "echo", "hey dude");
                var r2 = echo.Echo("hey dude");
                // print response
                Console.WriteLine(r2);

                var ping = node.Proxy.New<IPing>("localhost/System.Ping");
                var pong = ping.Ping(new byte[1024]);
                Console.WriteLine("ping-pong " + pong.Length);

                var client = new PingClient(ping);
                Console.WriteLine("delay = " + client.Ping(10));

                var bandwidth = node.Proxy.New<IBandwidth>("localhost/System.Bandwidth");
                var bw = new BandwidthClient(bandwidth, ping);
                Console.WriteLine("bandwidth = " + bw.Test());

                using (var dht = new DhtClient(node.Proxy.New<IByteDht>("localhost/System.Dht"), new JsonSerializer()))
                {
                    dht.Add("abc", "123");
                    dht.Subscribe(DhtOperation.All, ".*");
                    dht.KeyMatch += (operation, key) =>
                    {
                        Console.WriteLine("DHT callback " + operation + " key=" + key);
                    };
                    dht.Add("def", "456");
                    var x = dht.Get<string>("abc");
					Console.WriteLine("DHT result " + x);
                }
				Thread.Sleep(100000);


                using (var shell = new ConsoleClientActor("cmd.exe-client"))
                {
                    node.Add(shell);
                    var proxy = node.Proxy.New<IShell>("localhost/System.Shell");
                    var remoteId = proxy.RunConsole("cmd.exe", new string[0], shell.Box.Id);
                    node.Link(shell.Box.Id, remoteId);
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
            using (var node = new TcpNode(18223))
            using(var server = node.AddListener(18222, serializer))
            {                              
                // add actor, an actor is like an object. it can send & receive messages
                node.Add(new BandwidthActor());
                node.Add(new EchoActor());
                node.Add(new PingActor());
                node.Add(new Shell());
                node.Add(new DhtActor(new DhtMemoryBackend()));
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
