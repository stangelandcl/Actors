﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    class DhtMetadata
    {
        public int TimeToLive { get; set; }
        public ActorId Originator { get; set; }
    }
}