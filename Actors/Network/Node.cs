using System;
using System.ComponentModel;
using System.Linq;
using Actors.Connections;
using Actors.Connections.Messages;
using Actors.Proxies;
using Serialization;
using Actors.Connections.Tcp;
using Actors.Connections.Bytes;
using Actors.Builtin.Clients;
using Actors.Dht;
using Actors.Builtin.Actors;

namespace Actors
{
	public class Node : IDisposable
	{
        public Node()
        {
            Name = Guid.NewGuid().ToString();
            Serializer = new JsonSerializer();
            server = new TcpListeners(Serializer);
            world = new TcpWorld(Serializer);
            Links = new LinkMap();	
            Default = new DefaultActor(new MailBox(System.Environment.MachineName + "/default"), this);
            world.Add(Default);
            Proxy = new ProxyFactory(this);
            router = new ConnectionRouter(this);
            var dhtActor = new DhtActor(new DhtMemoryBackend());
            Add(dhtActor);
            Dht = dhtActor;
            server.Connected += HandleConnected;         
        }

        public ProxyFactory Proxy { get; private set; }
        public DefaultActor Default { get; private set; }
        public string Name { get; set; }
        public ISerializer Serializer { get; set; }
        public LinkMap Links { get; set; }
        public IDht Dht { get; private set; }
        protected Listeners server;
        protected TcpWorld world;
        protected ConnectionRouter router;

        public virtual void Dispose()
        {            
            server.Connected -= HandleConnected;
            //TODO: World.Dispose(); // dispose actors
        }

        void HandleConnected(IConnection obj)
        {
            AddConnection(obj, isOutbound: false);
        }

        public IDisposable AddConnection(IConnection connection, bool isOutbound = true)
        {            
            var disposable = router.Add(connection, isOutbound: isOutbound);
            connection.Received += HandleReceived;
            return Disposable.New(() =>
            {
                connection.Received -= HandleReceived;
                disposable.Dispose();
                connection.Dispose();
            });
        }

        void HandleReceived(object obj)
        {
            var mail = obj as Mail;
            if (mail == null) return;
            world.Dispatch(mail);
        }

        public IDisposable AddListener(IListener listener)
        {
            return server.Add(listener);
        }

        public void Add<T>(T a) where T : Actor
        {
            a.Node = this;
            a.Box.Id = new ActorId(System.Environment.MachineName + "/" + a.Box.Id);
            world.Add(a);
        }

        public void Link(ActorId creator, ActorId other)
        {
            Links.Add(creator, other);
            Send(other, creator, "Link", LinkStatus.Create, "Create");
        }

		public void Remove(Actor a, string msg = ""){
            var links = Links.Get(a);         
			world.Remove(a.Box.Id);
            foreach (var link in links)
                Send(link, a.Box.Id, "Link", LinkStatus.Died, msg); 
		}

		#region IMailSender implementation
		public MessageId Send (Mail mail)
		{
            if (mail.To.IsEmpty) return MessageId.Empty;
            var sender = router.Get(mail.To);
            if (sender == null) return MessageId.Empty;
			var mailSender = new MailSender(sender.Sender);
			mailSender.Send(mail);
			return mail.MessageId;
		}
		public MessageId Send (ActorId to, ActorId fromId, FunctionId name, params object[] args)
		{           
            var id = MessageId.New();
			Send(new Mail{To = to, From = fromId, MessageId = id, Name = name, Args = args});
            return id;
		}
		
		public MessageId Reply (Mail mail, params object[] args)
		{
			return Send(new Mail{To = mail.From, From = mail.To, MessageId = mail.MessageId, Name = "On" + mail.Name + "Reply", Args = args});
		}
		#endregion		
	}
}

