using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Actors.Connections;
using Actors.Connections.Messages;
using Serialization;

namespace Actors.Network.Tcp
{
    public class TcpNode : Node
    {
        public TcpNode(int defaultPort)
            : base()
        {
            this.defaultPort = defaultPort;
            router.ConnectionNotFound += HandleConnectionNotFound;
        }
        int defaultPort;

        void HandleConnectionNotFound(object sender, ConnectionRouter.MissingEventArgs args)
        {
			try{
				var tcp = new TcpClient();
				tcp.Connect(args.EndPoint.ToString(), defaultPort, TimeSpan.FromSeconds(3));

				var conn = new Connection(new TcpByteConnection(tcp), this.Serializer);
	            AddConnection(conn, isOutbound: true);
	            args.Added = true;
			}catch{}
        }

        public IDisposable AddConnection(string host, int port, ISerializer serializer)
        {
            return AddConnection(() =>
                new Connection(new TcpByteConnection(new TcpClient(host, port)), serializer));
        }

        public IDisposable AddConnection(TcpClient client, ISerializer serializer)
        {
            return AddConnection(new Connection(new TcpByteConnection(client), serializer));
        }

        public IDisposable AddListener(int port, ISerializer serializer, bool isLocalOnly = true)
        {
            var host = isLocalOnly ? "127.0.0.1" : "0.0.0.0";
            return server.Add(new Listener(new TcpByteListener(host, port), serializer));
        }

        public override void Dispose()
        {
            router.ConnectionNotFound -= HandleConnectionNotFound;
            base.Dispose();
        }
    }
}
