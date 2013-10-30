using System;
using System.ComponentModel;
using System.Linq;
using Actors.Connections;
using Actors.Connections.Messages;
using Actors.Proxies;
using Serialization;
using Actors.Connections.Tcp;
using Actors.Connections.Bytes;
using Actors.Builtins.Actors;
using Actors.Examples;
using Actors.Examples.Actors;
using System.Collections.Generic;
using Actors.Network;
using Actors.Connections.Local;
using KeyValueDatabase;
using Connections.Connections.Local;

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
            Default = new DefaultActor(new ActorId(Environment.MachineName, Id, "default"), this);
            world.Add(Default);
            Proxy = new ProxyFactory(this);
            var local = CreateLocalConnection();
            router = new ConnectionRouter(local);         
            server.Connected += HandleConnected;
            AddConnection(local);
        }

        private IConnection CreateLocalConnection()
        {
            var receiver = new LocalReceiver(new EndPoint(Id.ToString()));
            var sender = new LocalSender(new EndPoint(Id.ToString()), receiver);
            var local = new LocalConnection(sender, receiver);         
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
            //Add(new DhtActor(KeyValueDatabase.Proxy.ProxyFactory.New<IDhtBackend>()));
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
            connection.Received.Subscribe(HandleReceived);
            return Disposable.New(() =>
            {                
                disposable.Dispose();
                connection.Dispose();
            });
        }

        void HandleReceived(object obj)
        {
            var mail = obj as RpcMail;
            if (mail == null) return;
            world.Dispatch(mail);
        }

        public IDisposable AddListener(IListener listener)
        {
            return server.Add(listener);
        }

        public void Add<T>(T a) where T : DistributedActor
        {
            a.AttachNode(this);          
            world.Add(a);
        }

        public void Link(ActorId creator, ActorId other)
        {
            Links.Add(creator, other);
            Send(other, creator, "Link", LinkStatus.Create, "Create");
        }

		public void Remove(DistributedActor a, string msg = ""){
            var links = Links.Get(a);         
			world.Remove(a.Id);
            foreach (var link in links)
                Send(link, a.Id, "Link", LinkStatus.Died, msg); 
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

		public IMessageId Send (IMail mail)
		{
            if (mail.As<RpcMail>().To.As<ActorId>().IsEmpty) return MessageId.Empty;
            var sender = router.Get(mail.As<RpcMail>().To.As<ActorId>());
            if (sender == null) return MessageId.Empty;
			var mailSender = new MailSender(sender.Sender);
			mailSender.Send(mail);
			return mail.As<RpcMail>().MessageId;
		}

		public IMessageId Send (IActorId to, IActorId fromId, string name, params object[] args)
		{           
            var id = MessageId.New();
			Send(new RpcMail{To = to, From = fromId, MessageId = id, Message = new FunctionCall(name, args)});
            return id;
		}
		
		public IMessageId Reply (IMail m, params object[] args)
		{
			var mail = m.As<RpcMail>();
			return Send(new RpcMail{To = mail.From, From = mail.To, MessageId = mail.MessageId,Message = new FunctionCall(mail.Message.Name + "Reply",args)});
		}

		public IMessageId Reply (IActorId to, IActorId fromId, IMessageId msg, string name, params object[] args)
		{
			return Send(new RpcMail{To = to, From = fromId, 
				MessageId = msg,Message = new FunctionCall(name,args)});
		}
		#endregion		
	}
}

