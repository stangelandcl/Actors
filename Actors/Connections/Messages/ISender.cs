using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Actors
{
    public interface ISender : IDisposable
    {
        IEndPoint Remote { get; }
        void Send(object o);
        event Action<Exception> Error;
    }
}
