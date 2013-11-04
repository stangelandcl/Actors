using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cls.Connections
{
    public interface IReceiver : IDisposable
    {
        IEndPoint Remote { get; }
        MessageQueue<object> Received { get; }
        event Action<Exception> Error;
    }
}
