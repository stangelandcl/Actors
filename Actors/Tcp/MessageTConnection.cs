using System;
using System.Net.Sockets;
using Serialization;

namespace Actors
{
	public class MessageTConnection
	{
		public MessageTConnection(TcpClient c, ISerializer serializer)
			: this(new MessageTClient(new MessageClient(c), serializer), new MessageTReader(new MessageReader(c), serializer))
		{}		
		public MessageTConnection (MessageConnection c, ISerializer s){
			this.Sender = new MessageTClient(c.Sender, s);
            this.Receiver = new MessageTReader(c.Receiver, s);            
		}

		public MessageTConnection (MessageTClient sender, MessageTReader reader)
		{
			this.Sender = sender;
			this.Receiver = reader;
		}

        public event Action<MessageTConnection> Disconnected;
		public MessageTClient Sender {get;set;}
		public MessageTReader Receiver {get;set;}
	}
}

