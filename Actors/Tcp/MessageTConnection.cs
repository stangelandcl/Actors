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

		public MessageTConnection (MessageTClient sender, MessageTReader reader)
		{
			this.Sender = sender;
			this.Receiver = reader;
		}

		public MessageTClient Sender {get;set;}
		public MessageTReader Receiver {get;set;}
	}
}

