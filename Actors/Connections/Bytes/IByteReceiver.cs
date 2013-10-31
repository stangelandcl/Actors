using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Connections.Bytes
{
    public interface IByteReceiver
    {
        IEndPoint Remote { get; }
        MessageQueue<byte[]> Received { get; }
        event Action<Exception> Error;
    }
}
