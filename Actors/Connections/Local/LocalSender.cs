using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Messages;
using Actors.Connections.Bytes;

namespace Connections.Connections.Local
{
    public class LocalSender : ISender
    {
        public LocalSender(IEndPoint ep, LocalReceiver r)
        {
            Remote = ep;
            this.receiver = r;
        }
        LocalReceiver receiver;
        public Actors.Connections.Bytes.IEndPoint Remote { get; private set; }

        public void Send(object o)
        {
            receiver.Received.Post(o);
        }

        public event Action<Exception> Error;

        public void Dispose()
        {
            receiver.Dispose();
            
        }
    }
}
