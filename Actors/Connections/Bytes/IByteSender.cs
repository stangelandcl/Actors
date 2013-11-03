using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public interface IByteSender
    {
        IEndPoint Remote { get; }
        event Action<Exception> Error;
        void Send(byte[] bytes);
    }
}
