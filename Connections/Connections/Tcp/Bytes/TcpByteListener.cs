using System;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using Cls.Extensions;


namespace Cls.Connections
{
	public class TcpByteListener : TcpListener<IByteConnection>, IByteListener	
    {
        public TcpByteListener(string host, int port)
            : base(host, port)
        {}        

        public TcpByteListener(TcpListener l)
            : base(l)
		{            
		}
        protected override IByteConnection OnConnect(TcpClient c)
        {
            return new TcpByteConnection(c);
        }
	}
}

