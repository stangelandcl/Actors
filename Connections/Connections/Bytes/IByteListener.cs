using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Connections
{
    public interface IByteListener : IDisposable
    {
        event Action<IByteConnection> Connected;
    }
}
