using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Actors.Connections.Bytes;
using System.Net.Security;
using Actors.Connections.Tcp.Stream;

namespace Actors.Connections.Ssl.Stream
{
    public class SslByteConnection : TcpStreamConnection
    {
        public SslByteConnection(TcpClient client, SslStream stream)
            : base(client, stream)
        { }

        public static SslByteConnection New(TcpClient client)
        {
            throw new NotImplementedException();
        }
    }
}
