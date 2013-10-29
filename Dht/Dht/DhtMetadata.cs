using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dht.Ring;

namespace Actors.Dht
{
    class DhtMetadata
    {
        public int TimeToLive { get; set; }
        public IActorId Originator { get; set; }
    }
}
