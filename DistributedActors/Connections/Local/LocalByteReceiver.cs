using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;
using Actors.Network;

namespace Actors.Connections.Local
{
    class LocalByteReceiver : IByteReceiver
    {
        public LocalByteReceiver(NodeId remote)
        {           
            Remote = new EndPoint(remote.ToString());           
        }
       
        public IEndPoint Remote { get; private set; }
        public event Action<byte[]> Received;
        public event Action<Exception> Error;
        public void OnReceived(byte[] bytes)
        {
            Received.FireEventAsync(bytes);
        }
    }
}
