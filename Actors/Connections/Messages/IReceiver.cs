using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;

namespace Actors.Connections.Messages
{
    public interface IReceiver : IDisposable
    {
        IEndPoint Remote { get; }
        event Action<object> Received;
        event Action<IReceiver> Disconnected;
    }
}
