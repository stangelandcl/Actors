using System;
using System.Net;
using System.Linq;

namespace Actors
{
	public class DnsAlias
	{
		public static string[] Get(string name){
			if(name == Environment.MachineName || name == "localhost" || name == "127.0.0.1")			
				return new[]{Environment.MachineName, "localhost", "127.0.0.1"};
			var addresses = Dns.GetHostAddresses(name);
			return Enumerable.Repeat(name, 1).Concat(
				addresses.Where(n=>n.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					.Select(n=>n.ToString()))
					.ToArray();
		}

		public static ActorId[] Get(ActorId id){
			var s = id.ToString().Split('/');
			var names = Get(s[0]);
			return names.Select(n=> new ActorId(n + "/" + string.Join("/", s.Skip(1)))).ToArray();
		}
	}
}

