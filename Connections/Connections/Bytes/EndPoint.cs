using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Cls.Connections
{
    public class EndPoint : IEndPoint
    {       
        public EndPoint(string name)
        {
            this.name = name;
        }

        string name;
        public override string ToString()
        {
            return name;
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var ep = obj as EndPoint;
            if (ep != null)
                return string.Equals(name, ep.name, StringComparison.InvariantCultureIgnoreCase);
            return false;
        }

        public static IEndPoint GetRemote(TcpClient client)
        {
            return new EndPoint(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
        }
    }
}
