using System;
using System.ComponentModel;
using System.Linq;

namespace Actors
{
	public class Node
	{
		public NodeEnvironment Environment {get;set;}
        public static Node Default = new Node();

		public Node()
		    : this(new NodeEnvironment()){}

		public Node(NodeEnvironment env){
			Environment = env;
			Environment.DefaultActor = new DefaultActor(new MailBox(System.Environment.MachineName + "/default"), this);
			Environment.World.Add(Environment.DefaultActor);           
		}

      

		public dynamic GetProxy(string name){
			var r = new RemoteActor("proxy-" + name, name);
            Add(r);
			return new DynamicProxy(r);
		}

		public void Add<T>(T a) where T : Actor{
			a.Node = this;
			a.Box.Id = new ActorId(System.Environment.MachineName + "/" + a.Box.Id);
			Environment.World.Add(a);
		}

		public IDisposable Listen(string host, int port){
			return Environment.Server.Listen(host, port);
		}

		public IDisposable Connect(string host, int port){
			return Environment.Connector.Connect(host, port);
		}

        public void Link(ActorId creator, ActorId other){
            Environment.Links.Add(creator, other);
            Send(other, creator, "Link", LinkStatus.Create, "Create");
        }

		public void Remove(Actor a, string msg = ""){
            var links = Environment.Links.Get(a);         
			Environment.World.Remove(a.Box.Id);
            foreach (var link in links)
                Send(link, a.Box.Id, "Link", LinkStatus.Died, msg); 
		}

		#region IMailSender implementation
		public MessageId Send (Mail mail)
		{
            if (mail.To.IsEmpty) throw new Exception("To address is null");
			var sender = new MailSender( Environment.Router.Get(mail.To).Sender);
			sender.Send(mail);
			return mail.MessageId;
		}
		public void Send (ActorId to, ActorId fromId, FunctionId name, params object[] args)
		{           
			Send(new Mail{To = to, From = fromId, MessageId = MessageId.New(), Name = name, Args = args});
		}
		public MessageId Send (ActorId to, FunctionId name, params object[] args)
		{            
			MessageId msg;
			Send(new Mail{To = to, From = Environment.DefaultActor.Box.Id, MessageId = msg = MessageId.New(), Name = name, Args = args});
			return msg;
		}
		public void Reply (Mail mail, params object[] args)
		{
			Send(new Mail{To = mail.From, From = mail.To, MessageId = mail.MessageId, Name = "On" + mail.Name + "Reply", Args = args});
		}
		#endregion

		public T SendReceive<T>(ActorId to, FunctionId name, params object[] args){
			var msg = Send(to, name, args);		
			return Receive <T>(msg);
		}

		public T Receive<T>(){
			var mail = Environment.DefaultActor.Box.WaitForAny();
			if(mail == null) return default(T);
			return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertTo(mail.Args[0], typeof(T));
		}

		T Receive<T> (MessageId msg)
		{
			var mail = Environment.DefaultActor.Box.WaitFor (msg);
			if(mail == null) return default(T);
            return (T)ConvertEx.ChangeType(mail.Args[0], typeof(T));
		}
	}
}

