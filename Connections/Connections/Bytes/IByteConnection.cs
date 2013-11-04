using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Connections
{
    public interface IByteConnection : IDisposable
    {
		IEndPoint Remote {get;}
        void Send(byte[] d);
        MessageQueue<byte[]> Received { get; }
        event Action<IByteConnection> Disconnected;                
    }
}
