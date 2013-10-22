using System;
using Actors.Examples;
using Actors.Examples.Clients;
using Actors.Examples.Actors;
using RemoteConsole;
using System.Threading;

namespace Actors.Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            if (args[0] == "server")
            {
                // a node is like an erlang node. it is like a VM. like its own world
                var node = new Node();
                // listen on port
                var server = node.Listen("127.0.0.1", 18222);
                // add actor, an actor is like an object. it can send & receive messages
                node.Add(new BandwidthActor());
                node.Add(new EchoActor());
                node.Add(new PingActor());
                node.Add(new ConsoleProcessActor("cmd.exe", "cmd.exe"));
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {

                // create a node
                var n2 = new Node();
                // connect to an endpoint which is in this case a separate node. 
                // so now we can route messages from actors in this node to the
                // other node
                using (var conn = n2.Connect("127.0.0.1", 18222))
                {
                    // send async but receive sync. send to localhost/echo which happens to
                    // be in the other node
                    var result = n2.SendReceive<string>("localhost/System.Echo", "Echo", "hi");
                    // print response
                    Console.WriteLine(result);


                    // DynamicProxy wraps a remote actor.
                    var echo = n2.GetProxy("localhost/System.Echo");
                    // this is the same as SendReceive<string>("localhost/echo", "echo", "hey dude");
                    var r2 = echo.Echo("hey dude");
                    // print response
                    Console.WriteLine(r2);

                    var ping = n2.GetProxy("localhost/System.Ping");
                    var pong = ping.Ping(new byte[1024]);
                    Console.WriteLine("ping-pong " + pong.Length);

                    var client = new PingClient(ping);
                    Console.WriteLine("delay = " + client.Ping(10));

                    var bw = new BandwidthClient(n2.GetProxy("localhost/System.Bandwidth"), ping);
                    Console.WriteLine("bandwidth = " + bw.Test());

                    var console = n2.GetProxy("localhost/cmd.exe");
                    var realConsole = new ConsoleClientActor("cmd.exe");
                    n2.Add(realConsole);
                    console.Attach(realConsole.Box.Id);

                } // close the connection and remove from n2. Now there is no route to the first node and the echo actor
                // TODO: set default ports so given a computer name we can try to connect to a few ports if someone
                // tries to send a message to one of those actors then we could avoid this explicit connection and 
                // let the node/VM handle it.

                //TODO: change router to directly put messages in mailbox using function without going through tcp sockets
                // if the other actor is in the same node. This would make it feasible to use actors for some things inside a single 
                // process/node/program
            }
		}
	}
}
