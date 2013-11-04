using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Cls.Connections
{
    public class HttpListener
    {
        public HttpListener(int port)
        {
            this.tcp = new TcpClientListener("0.0.0.0", port);
            tcp.Connected += tcp_Connected;
        }

        void tcp_Connected(TcpClient obj)
        {
            var stream = obj.GetStream();           
        }

        TcpClientListener tcp;
    }
}
