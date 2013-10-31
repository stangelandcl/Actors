using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;


namespace Actors.Connections.Local
{
    class LocalByteReceiver : IByteReceiver
    {
        public LocalByteReceiver(IEndPoint remote)
        {
            Remote = remote;
            Received = new MessageQueue<byte[]>();
        }
       
        public IEndPoint Remote { get; private set; }
        public MessageQueue<byte[]> Received { get; private set; }
        public event Action<Exception> Error;        
    }
}
