using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Actors
{
    public interface IReceiver : IDisposable
    {
        IEndPoint Remote { get; }
        MessageQueue<object> Received { get; }
        event Action<Exception> Error;
    }
}
