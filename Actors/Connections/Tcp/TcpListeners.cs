using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Messages;
using Serialization;

namespace Actors.Connections.Tcp
{
    class TcpListeners : Listeners
    {   
        ISerializer serializer;
        public TcpListeners(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public IDisposable Listen(string host, int port)
        {
            return Add(new Listener(new TcpByteListener(host, port), serializer));
        }
    }
}
