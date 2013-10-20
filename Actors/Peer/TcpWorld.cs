using System;
using System.Collections.Generic;
using Serialization;
using System.Collections.Concurrent;

namespace Actors
{
	public class TcpWorld : IWorld
	{
		public TcpWorld()
			: this(36802, new JsonSerializer())
		{}

		public TcpWorld(int defaultPort, ISerializer defaultSerializer)
			: this(new TcpRouter(), new TcpServers(), defaultPort, defaultSerializer)
		{}
		public TcpWorld(IMailRouter router, TcpServers servers, int defaultPort, ISerializer defaultSerializer){
			this.router = router;
			this.server = servers;
			this.defaultPort = defaultPort;
			this.defaultSerializer = defaultSerializer;
			postOffice = new PostOfficeActor(new MailBox(new ActorId(id.ToString(), Guid.NewGuid())), this);
			this.router.Received += HandleReceived;		
			servers.Connected += HandleConnected;
		}
		Guid id = Guid.NewGuid();
		int defaultPort;
		ISerializer defaultSerializer;
		IMailRouter router;
		TcpServers server; 
		INameResolver names;
		ConcurrentBag<Actor> actors = new ConcurrentBag<Actor>();
		PostOfficeActor postOffice;

		void HandleReceived (Mail obj)
		{
			postOffice.MailBox.Receive(obj);
		}

		public void Listen(string host ,int port){
			server.Listen(host, port);
			server.Connected += HandleConnected;
		}

		void HandleConnected(MessageConnection connection){
			router.Add(new MessageTConnection(connection, serializer));
		}

		public ActorId Resolve(string name){
			return names.NameToAddress(name);
		}

		public void Connect(string host, int port){
		}

		public void Add(Actor actor){
			lock(actors)
				actors.Add(actor);
			names.Alias(actor.MailBox.Id, actor.MailBox.Id);
		}

		public void Remove(ActorId id){
			lock(actors)
				foreach(var actor in actors){
					if(actor.MailBox.Id == id)
						actors.Remove(actor);
				}
			names.Remove(id);
		}

		public ISender GetSender(ActorId id){
			return router.GetSender(id);
		}

	}
}

