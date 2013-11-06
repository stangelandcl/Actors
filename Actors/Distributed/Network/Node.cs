using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;

namespace Cls.Actors
{
	public class Node : IDisposable
	{
        public Node(string name = null)
        {
            Id = new NodeId(name ?? Guid.NewGuid().ToString());           
			Serializer = Defaults.Serializer;
            server = new TcpListeners(Serializer);
            World = new TcpWorld();
            Links = new LinkMap();	
			Default = new DefaultActor();
            Add(Default);
			nodeMap = new NodeMapActor();
			Add(nodeMap);
			nodeMap.NodeFound += (arg1, arg2) => Connected.FireEventAsync (arg1, arg2);
            proxy = new ProxyFactory(this);
            var local = CreateLocalConnection();
            Router = new ConnectionRouter(local);         
            server.Connected += HandleConnected;
            Connect(local);
			AddBuiltins();
        }

        private IConnection CreateLocalConnection()
        {
            var receiver = new LocalReceiver(new EndPoint(Id.ToString()));
            var sender = new LocalSender(new EndPoint(Id.ToString()), receiver);
            var local = new LocalConnection(sender, receiver);         
            return local;
        }

        public NodeId Id { get; private set; }
		ProxyFactory proxy;
        public DefaultActor Default { get; private set; }    
        public ISerializer Serializer { get; set; }
        public LinkMap Links { get; set; }       
        protected Listeners server;
		public TcpWorld World {get; private set;}
		public ConnectionRouter Router {get; private set;}
		protected NodeMapActor nodeMap;

        public void AddBuiltins()
        {
            Add(new BandwidthActor());
            Add(new EchoActor());
            Add(new PingActor());
            Add(new Shell());
			Add(new DhtActor());
			Add(new FileCopyActor());
			Add(new LogActor());
			Add(new ProcessActor());
        }

		public event Action<IEndPoint, NodeId> Connected;

		/// <summary>
		/// New proxy
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T New<T>(ActorId id){
			return proxy.New<T>(id);
		}

        public virtual void Dispose()
        {            
            server.Connected -= HandleConnected;
            //TODO: World.Dispose(); // dispose actors
        }

        void HandleConnected(IConnection obj)
        {
            Connect(obj, isOutbound: false);
        }

		public IDisposable Connect(Func<IConnection> connection, bool isOutBound  = true)
        {
            return ConnectionFactory.Connect(this, connection, isOutBound);
        }

        public IDisposable Connect(IConnection connection, bool isOutbound = true)
        {            		
            var disposable = Router.Add(connection, isOutbound: isOutbound);
            connection.Received.Subscribe(HandleReceived);
			nodeMap.Check(connection);

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
            World.Dispatch(mail);
        }

        public IDisposable Listen(IListener listener)
        {
            return server.Add(listener);
        }

        public void Add<T>(T a) where T : DistributedActor
        {
            a.AttachNode(this);          
            World.Add(a);
        }

		public DistributedActor Get(ActorId id){
			return World.Get(id);
		}

        public void Link(ActorId creator, ActorId other)
        {
            Links.Add(creator, other);
            Send(other, creator, "Link", LinkStatus.Create, "Create");
        }

		public void Remove(DistributedActor a, string msg = ""){
            var links = Links.Get(a);         
			World.Remove(a.Id);
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
            var sender = Router.Get(mail.As<RpcMail>().To.As<ActorId>());
            if (sender == null) return MessageId.Empty;
			var mailSender = new MailSender(sender);
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

