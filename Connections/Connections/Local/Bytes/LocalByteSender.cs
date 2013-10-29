using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;

namespace Actors.Connections.Local
{
    class LocalByteSender : IByteSender
    {
        public LocalByteSender(IEndPoint remote, LocalByteReceiver receiver)
        {
            this.receiver = receiver;
            Remote = remote;
        }

        LocalByteReceiver receiver;
        public IEndPoint Remote { get; private set; }        
        public event Action<Exception> Error;

        public void Send(byte[] bytes)
        {            
            receiver.Received.Send(bytes);           
        }
    }
}
