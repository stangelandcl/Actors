using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Actors
{
    public class LocalReceiver : IReceiver
    {
        public LocalReceiver(IEndPoint ep)
        {
            Remote = ep;
            Received = new Actors.MessageQueue<object>();
        }

        public IEndPoint Remote { get; private set; }
        public Actors.MessageQueue<object> Received { get; private set; }
        public event Action<Exception> Error;

        public void Dispose()
        {
            Received.Dispose();
        }
    }
}
