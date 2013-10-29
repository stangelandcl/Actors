using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;

namespace Dht.Ring
{
    public interface ISender
    {
        void Send(IActorId id, string message, params object[] args);
    }
}
