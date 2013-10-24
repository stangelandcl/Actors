using System;
using System.Net.Sockets;
using Actors.Connections;
using Serialization;
using Actors.Connections.Bytes;
using System.Linq;
using Actors.Tasks;
using System.Diagnostics;
using System.Net;
using Actors.Connections.Tcp.Stream;

namespace Actors
{
	public class TcpByteConnection : TcpStreamConnection
	{
        public TcpByteConnection(TcpClient c)
            : base(c, c.GetStream())
        { }        
    }
}

