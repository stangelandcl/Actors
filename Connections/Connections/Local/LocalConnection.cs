using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections;
using Actors;

namespace Connections.Connections.Local
{
    public class LocalConnection : IConnection
    {
        public LocalConnection(LocalSender s, LocalReceiver r)
        {
            Sender = s;
            Receiver = r;
        }
        public event Action<IConnection> Disconnected;

        public void Send(object o)
        {
            Sender.Send(o);
        }

        public MessageQueue<object> Received { get; private set; }

        public Actors.Connections.Messages.ISender Sender {get; private set;}

        public Actors.Connections.Messages.IReceiver Receiver {get; private set;}
     
        public void Dispose()
        {
            Received.Dispose();
            Receiver.Dispose();
            Sender.Dispose();
        }
    }
}
