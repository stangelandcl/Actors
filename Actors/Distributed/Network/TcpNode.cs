using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Actors
{
    public class TcpNode : Node
    {
		public static TcpNode Open(int defaultPort = 0, string name = null){
			var n = new TcpNode(defaultPort, name);
			n.Listen(defaultPort);
			return n;
		}

        public TcpNode(int defaultPort = 0, string name = null)
            : base(name ?? Defaults.GetPort(defaultPort).ToString())
        {
			this.defaultPort = Defaults.GetPort(defaultPort);
            Router.ConnectionNotFound += HandleConnectionNotFound;
        }
        int defaultPort;

        void HandleConnectionNotFound(object sender, ConnectionRouter.MissingEventArgs args)
        {
			try{
				var tcp = new TcpClient();
				tcp.Connect(args.EndPoint.ToString(), defaultPort, TimeSpan.FromSeconds(3));

				var conn = new Connection(new TcpByteConnection(tcp), this.Serializer);
	            Connect(conn, isOutbound: true);
	            args.Added = true;
			}catch{}
        }

        public IDisposable Connect(string host, int port, ISerializer serializer= null)
        {
            return Connect(() =>
                new Connection(new TcpByteConnection(new TcpClient(host, port)), Defaults.Get(serializer)));
        }

		public IDisposable Connect(int port, ISerializer serializer= null)
		{
			return Connect("localhost", port, serializer);					
		}

        public IDisposable Connect(TcpClient client, ISerializer serializer)
        {
            return Connect(new Connection(new TcpByteConnection(client), serializer));
        }

        public IDisposable Listen(int port, ISerializer serializer = null, bool isLocalOnly = false)
        {		
            var host = isLocalOnly ? "127.0.0.1" : "0.0.0.0";
            return server.Add(new Listener(new TcpByteListener(host, port), serializer));
        }

        public override void Dispose()
        {
            Router.ConnectionNotFound -= HandleConnectionNotFound;
            base.Dispose();
        }
    }
}
