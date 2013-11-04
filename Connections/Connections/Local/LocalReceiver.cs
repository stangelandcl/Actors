using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cls.Connections
{
    public class LocalReceiver : IReceiver
    {
        public LocalReceiver(IEndPoint ep)
        {
            Remote = ep;
            Received = new MessageQueue<object>();
        }

        public IEndPoint Remote { get; private set; }
        public MessageQueue<object> Received { get; private set; }
        public event Action<Exception> Error;

        public void Dispose()
        {
            Received.Dispose();
        }
    }
}
