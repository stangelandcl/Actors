﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cls.Serialization;
using Cls.Extensions;


namespace Cls.Connections
{
    class TcpConnector
    {
        public TcpConnector(ISerializer s)
        {
            this.serializer = s;
        }
        public event Action<IConnection> Connected;
        ISerializer serializer;
        public void Connect(string host, int port)
        {
            var c = new Connection(new TcpByteConnection(
                new System.Net.Sockets.TcpClient(host, port)), serializer);
            Connected.FireEventAsync(c);
        }
    }
}
