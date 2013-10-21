using System;
using System.ComponentModel;
using System.Linq;

namespace Actors
{
	public class Node
	{
		public NodeEnvironment Environment {get;set;}

		public Node()
		: this(new NodeEnvironment()){}

		public Node(NodeEnvironment env){
			Environment = env;
			Environment.DefaultActor = new DefaultActor(new MailBox(System.Environment.MachineName + "/default"), this);
			Environment.World.Add(Environment.DefaultActor);
		}

		public dynamic GetProxy(string name){
			var r = new RemoteActor("proxy-" + name, name);
			return new DynamicProxy(Add(r));
		}

		public T Add<T>(T a) where T : Actor{
			a.Node = this;
			a.Box.Id = new ActorId(System.Environment.MachineName + "/" + a.Box.Id);
			Environment.World.Add(a);
			return a;
		}

		public IDisposable Listen(string host, int port){
			return Environment.Server.Listen(host, port);
		}

		public IDisposable Connect(string host, int port){
			return Environment.Connector.Connect(host, port);
		}

		public void Remove(Actor a){
			Environment.World.Remove(a.Box.Id);
		}

		#region IMailSender implementation
		public MessageId Send (Mail mail)
		{
			var sender = new MailSender( Environment.Router.Get(mail.To).Sender);
			sender.Send(mail);
			return mail.MessageId;
		}
		public void Send (ActorId to, ActorId fromId, MessageId msg, FunctionId name, params object[] args)
		{
			Send(new Mail{To = to, From = fromId, MessageId = msg, Name = name, Args = args});
		}
		public MessageId Send (ActorId to, FunctionId name, params object[] args)
		{
			MessageId msg;
			Send(new Mail{To = to, From = Environment.DefaultActor.Box.Id, MessageId = msg = MessageId.New(), Name = name, Args = args});
			return msg;
		}
		public void Reply (Mail mail, FunctionId name, params object[] args)
		{
			Send(new Mail{To = mail.From, From = mail.To, MessageId = mail.MessageId, Name = name, Args = args});
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
			return (T)TypeDescriptor.GetConverter (typeof(T)).ConvertTo (mail.Args [0], typeof(T));
		}
	}
}

