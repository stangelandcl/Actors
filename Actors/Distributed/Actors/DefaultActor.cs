using System;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace Actors
{
	public class DefaultActor : DistributedActor
	{
        public DefaultActor(string shortname = "System.Default")
            : base(shortname)
        { }
		public T SendReceive<T>(ActorId to, string name, params object[] args)
        {
            var msg = SendTo(to, name, args);
            return Receive<T>(msg);
        }
	}
}

