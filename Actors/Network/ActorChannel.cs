using System;
using Serialization;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Actors
{
	public class ActorChannel : PostOfficeActor
	{
		public ActorChannel(string host, int port, ISerializer serializer = null)
			: this(new TcpClient(host, port), serializer ?? Defaults.Serializer)
		{}			

		public ActorChannel (TcpClient client, ISerializer serializer)
			: base(new MailBox(PostOfficeId), new MessageClient(client), serializer)
		{		
			var reader = new MessageReader(client);
			reader.MessageReceived += HandleMessageReceived;
			var ni = new NewInstance(NextActorId(), this, this.remote, this.serializer);
			Add("NewInstance", ni.MailBox);
			Add("PostOffice", this.MailBox);
			this.MailBox.Received += HandleReceived;
			reader.Listen();
		}

		const int PostOfficeId = 1;

		void HandleReceived ()
		{
			var msg = CheckFor(n=>n.Name == "GetName");
			if(msg != null){
				var id = GetIdFromName((string)msg.Args[0]);
				Reply(msg, "", id);					
			}
		}

		void HandleMessageReceived (Message obj)
		{
			var mail = serializer.Deserialize<Mail>(obj.Buffer);
			MailBox mailBox;
			lock(mailboxes){	
				if(!mailboxes.TryGetValue(mail.ReceiverId, out mailBox))
					return;
			}
			mailBox.Receive(mail);
		}
			
		public int GetRemoteActorId(string name){
			return SendReceive<int>(PostOfficeId, "GetName", name);
		}
		public RemoteActor GetRemoteActor(string name){
			return GetRemoteActor(GetRemoteActorId(name));
		}
		public RemoteActor GetRemoteActor(int id){
			var box = new MailBox(NextActorId());
			Add(Guid.NewGuid().ToString(), box);
			return new RemoteActor(id, box,remote, serializer);
		}
		public dynamic GetRemoteProxy(int id){
			return new DynamicProxy(GetRemoteActor(id));
		}
		public dynamic GetRemoteProxy(string name){
			return new DynamicProxy(GetRemoteActor(name));
		}
	}
}

