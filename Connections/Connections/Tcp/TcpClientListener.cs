using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Cls.Connections
{
    public class TcpClientListener : TcpListener<TcpClient>
    {
        public TcpClientListener(string host, int port)
            : base(host, port) { }

        protected override TcpClient OnConnect(TcpClient c)
        {
            return c;
        }
    }
}
