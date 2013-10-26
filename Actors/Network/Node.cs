using System;
using System.ComponentModel;
using System.Linq;
using Actors.Connections;
using Actors.Connections.Messages;
using Actors.Proxies;
using Serialization;
using Actors.Connections.Tcp;
using Actors.Connections.Bytes;
using Actors.Dht;
using Actors.Builtins.Actors;
using Actors.Examples;
using Actors.Examples.Actors;
using System.Collections.Generic;
using Actors.Builtins.Actors.Dht;
using Actors.Network;
using Actors.Connections.Local;

namespace Actors
{
	public class Node : IDisposable
	{
        public Node()
        {
            Id = NodeId.New();
            Serializer = new JsonSerializer();
            server = new TcpListeners(Serializer);
            world = new TcpWorld();
            Links = new LinkMap();	
            Default = new DefaultActor(new MailBox(new ActorId(Environment.MachineName, Id, "default")), this);
            world.Add(Default);
            Proxy = new ProxyFactory(this);
            var local = CreateLocalConnection();
            router = new ConnectionRouter(local);         
            server.Connected += HandleConnected;
            AddConnection(local);
        }

        private Connection CreateLocalConnection()
        {
            var receiver = new LocalByteReceiver(Id);
            var sender = new LocalByteSender(Id, receiver);
            var local = new Connection(new ByteConnection(sender, receiver), new JsonSerializer());            
            return local;
        }

        public NodeId Id { get; private set; }
        public ProxyFactory Proxy { get; private set; }
        public DefaultActor Default { get; private set; }      
        public ISerializer Serializer { get; set; }
        public LinkMap Links { get; set; }       
        protected Listeners server;
        protected TcpWorld world;
        protected ConnectionRouter router;

        public void AddBuiltins()
        {
            Add(new BandwidthActor());
            Add(new EchoActor());
            Add(new PingActor());
            Add(new Shell());
            Add(new DhtActor(new DhtMemoryBackend()));
        }

        public virtual void Dispose()
        {            
            server.Connected -= HandleConnected;
            //TODO: World.Dispose(); // dispose actors
        }

        void HandleConnected(IConnection obj)
        {
            AddConnection(obj, isOutbound: false);
        }

        public IDisposable AddConnection(Func<IConnection> connection, bool isOutBound  = true)
        {
            return ConnectionFactory.Connect(this, connection, isOutBound);
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
            a.AttachNode(this);          
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
        //public void SendStream(ActorId to, ActorId from, string name, IEnumerable<object[]> mails)
        //{
        //    if (to.IsEmpty) return;
        //    var sender = router.Get(to);
        //    if (sender == null) return;
        //    var mailSender = new MailSender(sender.Sender);
        //    mailSender.SendStream(
        //}

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

