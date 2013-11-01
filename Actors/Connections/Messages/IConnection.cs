using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Actors
{
    public interface IConnection : IDisposable
    {              
        event Action<IConnection> Disconnected;
        void Send(object o);
        MessageQueue<object> Received { get; }
        ISender Sender { get; }
        IReceiver Receiver { get; }
		bool IsAlive {get;}
    }
}
