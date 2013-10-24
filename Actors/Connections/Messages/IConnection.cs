using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;
using Actors.Connections.Messages;

namespace Actors.Connections
{
    public interface IConnection : IDisposable
    {              
        event Action<IConnection> Disconnected;
        void Send(object o);
        event Action<object> Received;
        ISender Sender { get; }
        IReceiver Receiver { get; }
    }
}
