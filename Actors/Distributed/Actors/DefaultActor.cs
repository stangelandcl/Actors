using System;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Cls.Extensions;

namespace Cls.Actors
{
	public class DefaultActor : DistributedActor
	{
        public DefaultActor(string shortname = "System.Default")
            : base(shortname)
        { }
		public Task<Option<T>> SendReceive<T>(ActorId to, string name, params object[] args)
        {
            var msg = SendTo(to, name, args);
            return Receive<T>(msg);
        }
	}
}

