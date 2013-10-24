﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Connections.Bytes
{
    public interface IByteListener : IDisposable
    {
        event Action<IByteConnection> Connected;
    }
}
