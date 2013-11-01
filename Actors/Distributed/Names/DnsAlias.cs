using System;
using System.Net;
using System.Linq;


namespace Actors
{
	public static class DnsAlias
	{
        public static IEndPoint[] Get(IEndPoint ep)
        {
            var name = ep.ToString();
            if (name == Environment.MachineName || name == "localhost" || name == "127.0.0.1")
                return new[] { Environment.MachineName, "localhost", "127.0.0.1" }
                    .Select(n => new Actors.EndPoint(n)).ToArray();
            var addresses = Dns.GetHostAddresses(name);
            return Enumerable.Repeat(ep, 1).Concat(
                addresses.Where(n => n.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(n => new Actors.EndPoint(n.ToString())))
                    .ToArray();
        }

		public static ActorId[] Get(ActorId id){
			var s = id.ToString().Split('/');
            var names = Get(new Actors.EndPoint(s[0]));
			return names.Select(n=> new ActorId(n + "/" + string.Join("/", s.Skip(1)))).ToArray();
		}
	}
}

