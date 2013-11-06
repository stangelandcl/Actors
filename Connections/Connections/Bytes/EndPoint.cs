using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using Cls.Extensions;

namespace Cls.Connections
{
	//[TypeConverter(typeof(StringConverter<EndPoint>))]
    public class EndPoint : IEndPoint
    {       
        public EndPoint(string name)
        {
            this.Name = name;
        }

		public string Name { get; set; }
        public override string ToString()
        {
			return Name ?? "";
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var ep = obj as EndPoint;
            if (ep != null)
                return string.Equals(Name, ep.Name, StringComparison.InvariantCultureIgnoreCase);
            return false;
        }

        public static IEndPoint GetRemote(TcpClient client)
        {
            return new EndPoint(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
        }
    }
}
