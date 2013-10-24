using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Connections.Bytes
{
    public interface IByteSender
    {
        IEndPoint Remote { get; }
        event Action<IByteSender> Disconnected;
        void Send(byte[] bytes);
    }
}
