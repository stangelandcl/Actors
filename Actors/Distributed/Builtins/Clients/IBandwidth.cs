using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Actors.Examples.Clients
{
    public interface IBandwidth
    {
        int Upload(byte[] data);
        byte[] Download(int count);
    }
}
