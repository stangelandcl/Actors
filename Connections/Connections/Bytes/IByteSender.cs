using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Connections
{
    public interface IByteSender
    {
        IEndPoint Remote { get; }
        event Action<Exception> Error;
        void Send(byte[] bytes);
    }
}
