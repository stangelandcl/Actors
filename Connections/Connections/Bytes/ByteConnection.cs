using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Connections.Bytes
{
    public class ByteConnection : IByteConnection
    {       
        public ByteConnection(IByteSender sender, IByteReceiver receiver)
        {               
            Sender = sender;
            Receiver = receiver;
            Receiver.Received.Subscribe(HandleReceived);
            Sender.Error += HandleError;
            Receiver.Error += HandleError;

        }           
        public IByteSender Sender { get; private set; }
        public IByteReceiver Receiver { get; private set; }

        protected virtual void HandleError(Exception e) { }
       
        void HandleReceived(byte[] bytes)
        {
            Received.Send(bytes);
        }

        public virtual void Dispose()
        {
            Receiver.Received.Dispose();
            Receiver.Error -= HandleError;
            Sender.Error -= HandleError;       
        }

        public MessageQueue<byte[]> Received { get; private set; }
        public event Action<IByteConnection> Disconnected;
        protected void FireDisconnected()
        {
            Disconnected.FireEventAsync(this);
        }

        public void Send(byte[] d)
        {
            Sender.Send(d);
        }
        
    }
}
