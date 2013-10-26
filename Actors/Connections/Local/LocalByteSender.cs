using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;
using Actors.Network;

namespace Actors.Connections.Local
{
    class LocalByteSender : IByteSender
    {
        public LocalByteSender(NodeId remote, LocalByteReceiver receiver)
        {
            this.receiver = receiver;
            Remote = new EndPoint(remote.ToString());
        }

        LocalByteReceiver receiver;
        public IEndPoint Remote { get; private set; }        
        public event Action<Exception> Error;

        public void Send(byte[] bytes)
        {
            try
            {
                receiver.OnReceived(bytes);
            }
            catch (Exception ex)
            {
                Error.FireEventAsync(ex);
            }
        }
    }
}
