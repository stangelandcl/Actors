using System;
using System.Net;
using System.Linq;

using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;

namespace Cls.Actors
{
	public static class DnsAlias
	{
        public static IEndPoint[] Get(IEndPoint ep)
        {
            var name = ep.ToString();
            if (name == Environment.MachineName || name == "localhost" || name == "127.0.0.1")
                return new[] { Environment.MachineName, "localhost", "127.0.0.1" }
                    .Select(n => new Cls.Connections.EndPoint(n)).ToArray();
            var addresses = Dns.GetHostAddresses(name);
            return Enumerable.Repeat(ep, 1).Concat(
                addresses.Where(n => n.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(n => new Cls.Connections.EndPoint(n.ToString())))
                    .ToArray();
        }

		public static ActorId[] Get(ActorId id){
			var s = id.ToString().Split('/');
            var names = Get(new Cls.Connections.EndPoint(s[0]));
			return names.Select(n=> new ActorId(n + "/" + string.Join("/", s.Skip(1)))).ToArray();
		}
	}
}

