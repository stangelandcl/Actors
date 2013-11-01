using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;


namespace Actors
{
    public class SslByteConnection : TcpStreamConnection
    {
        public SslByteConnection(TcpClient client, SslStream stream)
            : base(client, stream)
        { }      
    }
}
