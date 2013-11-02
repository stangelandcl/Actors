using System;
using System.Linq;
using ManyConsole;
using NDesk.Options;
using System.Threading;


namespace Actors
{
	public class Program
	{

		private static void Test(){			                               
			var node = Defaults.Node();                                                 
			var dht = node.New<IDht>("System.Dht") ;                                      
			dht.Put("abc", 123);
			//Thread.Sleep(100000);
		}
		public static void Main (string[] args)
		{
			Test();

            string[] bootstrap = null;
            var p = new OptionSet()
                .Add("dht", n => bootstrap = n.Split(',').ToArray());
            p.Parse(args);
          
            const int DefaultPort = 12848;
            using (var node = new TcpNode(DefaultPort))
            using(var server = node.Listen(DefaultPort, new JsonSerializer()))
            {
                node.AddBuiltins();
                if (bootstrap != null)
                {
                    var dht = node.New<IDht>("System.Dht");
                    var peers = bootstrap.Select(n => (IActorId)ActorId.FromUrl(n)).ToArray();
                    dht.Join(peers);
                    Console.WriteLine("Joined dht with bootstrap peers " + string.Join<IActorId>(", ", peers));
                }
               
                Console.WriteLine("Listening on " + DefaultPort + " " + node.Id);
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
            }
		}
	}
}

