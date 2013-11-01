using System;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;
using System.Net;

namespace Actors
{
	public class TcpByteConnection : TcpStreamConnection
	{
        public TcpByteConnection(TcpClient c)
            : base(c, c.GetStream())
        { }        
    }
}

