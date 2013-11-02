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
			using(var node = Defaults.Node()){
				var dht = node.New<IDht>("System.Dht");
				dht.Put("abc", "def");
				dht.Get("abc");

				var proc = node.New<IProcess>("System.Process");
				var p = proc.GetProcesses();
				Console.WriteLine(p.ToDelimitedString(Environment.NewLine));


				using(node.Connect("localhost", 12848)){
					var p2 = proc.GetConnections();
					Console.WriteLine(p2.ToDelimitedString(Environment.NewLine));


					var dht2 = node.New<IDht>("12848.localhost/System.Dht");
					Console.WriteLine(dht2.Get("abc"));
					proc = node.New<IProcess>("12848.localhost/System.Process");
					Console.WriteLine(proc.GetProcesses().ToDelimitedString(Environment.NewLine));


				}}
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
            using(var server = node.Listen(DefaultPort))
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

