using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;
using Serialization;

namespace Actors.Connections.Messages
{
    class Receiver : IReceiver
    {
        public Receiver(IByteReceiver receiver, ISerializer serializer)
        {
            this.receiver = receiver;
            this.serializer = serializer;
            this.receiver.Received += HandleReceived;
            this.receiver.Disconnected += HandleDisconnected;
        }

        IByteReceiver receiver;
        ISerializer serializer;
        public event Action<object> Received;
        public event Action<IReceiver> Disconnected;

        void HandleDisconnected(IByteReceiver r)
        {
            Disconnected.FireEventAsync(this);
        }

        void HandleReceived(byte[] b)
        {
            var obj = serializer.Deserialize<object>(b);
            Received.FireEventAsync(obj);
        }

      

        public void Dispose()
        {
            receiver.Received -= HandleReceived;
        }

        public IEndPoint Remote
        {
            get { return receiver.Remote; }
        }
    }
}
