using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;

namespace Actors.Connections.Messages
{
    public interface ISender : IDisposable
    {
        IEndPoint Remote { get; }
        void Send(object o);
        event Action<Exception> Error;
    }
}
