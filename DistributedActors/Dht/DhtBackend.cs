using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dht;
using KeyValueDatabase;
using KeyValueDatabase.Proxy;

namespace Actors.Dht
{
    public interface IDhtBackend : IKvpDb
    {       
        IKvpDbSet<ActorId> Peers { get; }
    }
}
