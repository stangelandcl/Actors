using System;
using Serialization;

namespace Actors
{
	public class NodeEnvironment
	{
		public string Name {get;set;}
		public TcpServers Server {get; set;}
		public TcpRouter Router {get;set;}
		public TcpWorld World {get;set;}
		public TcpConnector Connector {get;set;}
		public ISerializer Serializer {get;set;}
		public Actor DefaultActor {get; set;}

		public NodeEnvironment(){
			Name = Guid.NewGuid().ToString();
			Serializer = new JsonSerializer();
			Server = new TcpServers();
			Router = new TcpRouter(Serializer);
			World = new TcpWorld(Serializer);
			Connector = new TcpConnector();

			Server.Connected += HandleConnected;
			Connector.Connected += HandleConnected;
			Router.Received += HandleReceived;
		}

		void HandleReceived (Mail obj)
		{
			World.Dispatch(obj);
		}

		void HandleConnected (MessageConnection obj)
		{
			Router.Add(new MessageTConnection(obj, Serializer));
		}
	}
}

