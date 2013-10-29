using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Connections.Messages
{
    public interface IListener  : IDisposable
    {
        event Action<IConnection> Connected;
    }
}
