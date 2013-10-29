using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Messages;
using Actors.Connections.Bytes;

namespace Connections.Connections.Local
{
    public class LocalReceiver : IReceiver
    {
        public LocalReceiver(IEndPoint ep)
        {
            Remote = ep;
            Received = new Actors.MessageQueue<object>();
        }

        public Actors.Connections.Bytes.IEndPoint Remote { get; private set; }


        public Actors.MessageQueue<object> Received { get; private set; }

        public event Action<Exception> Error;

        public void Dispose()
        {
            Received.Dispose();
        }
    }
}
