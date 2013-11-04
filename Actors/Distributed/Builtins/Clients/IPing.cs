using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Actors.Examples.Clients
{
    public interface IPing
    {
        byte[] Ping(byte[] data);
    }
}
