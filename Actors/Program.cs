using System;
using System.Linq;
using ManyConsole;
using Actors.Network.Tcp;
using Serialization;
using NDesk.Options;
using Dht;

namespace Actors
{
	public class Program
	{
		public static void Main (string[] args)
		{
            string[] bootstrap = null;
            var p = new OptionSet()
                .Add("dht", n => bootstrap = n.Split(',').ToArray());
            p.Parse(args);
          
            const int DefaultPort = 12848;
            using (var node = new TcpNode(DefaultPort))
            using(var server = node.AddListener(DefaultPort, new JsonSerializer()))
            {
                node.AddBuiltins();
                if (bootstrap != null)
                {
                    var dht = node.New<IDht>("System.Dht");
                    var peers = bootstrap.Select(n => (IActorId)ActorId.FromUrl(n)).ToArray();
                    dht.Join(peers);
                    Console.WriteLine("Joined dht with bootstrap peers " + string.Join<IActorId>(", ", peers));
                }
               
                Console.WriteLine("Listening on " + DefaultPort);
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
            }
		}
	}
}

