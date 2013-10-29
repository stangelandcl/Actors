using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Connections.Bytes
{
    public interface IByteConnection : IDisposable
    {
        void Send(byte[] d);
        MessageQueue<byte[]> Received { get; }
        event Action<IByteConnection> Disconnected;        
        IByteSender Sender { get; }
        IByteReceiver Receiver { get; }
    }
}
