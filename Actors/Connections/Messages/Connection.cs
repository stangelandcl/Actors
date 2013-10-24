using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Serialization;
using Actors.Connections.Bytes;

namespace Actors.Connections.Messages
{
    class Connection : IConnection
    {
        public Connection(IByteConnection b, ISerializer serializer)
            : this(new Sender(b.Sender, serializer),
                   new Receiver(b.Receiver, serializer))
        { }

        public Connection(ISender sender, IReceiver receiver)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Receiver.Received += HandleReceived;
            this.Sender.Disconnected += HandleDisconnected;
            this.Receiver.Disconnected += HandleDisconnected;
        }

        public ISender Sender { get; private set; }
        public IReceiver Receiver { get; private set; }

        public event Action<IConnection> Disconnected;
        public event Action<object> Received;

        void HandleReceived(object ob)
        {          
            Received.FireEventAsync(ob);
        }

        void HandleDisconnected(ISender c)
        {
            Disconnected.FireEventAsync(this);
        }

        void HandleDisconnected(IReceiver r)
        {
            Disconnected.FireEventAsync(this);
        }
     
        public void Send(object o)
        {
            Sender.Send(o);
        }       

        public void Dispose()
        {
            Receiver.Received -= HandleReceived;
            Sender.Disconnected -= HandleDisconnected;
            Receiver.Disconnected -= HandleDisconnected;
            Sender.Dispose();
            Receiver.Dispose();
        }
    }
}
