using System;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;
using System.Net;

namespace Cls.Connections
{
	public class TcpByteConnection : TcpStreamConnection
	{
        public TcpByteConnection(TcpClient c)
            : base(c, c.GetStream())
        { }        
    }
}

