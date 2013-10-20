using System;
using System.Net.Sockets;

namespace Actors
{
	public class MessageConnection
	{
		public MessageConnection(TcpClient c)
			: this(new MessageClient(c), new MessageReader(c))
		{}		

		public MessageConnection (MessageClient sender, MessageReader reader)
		{
			this.Sender = sender;
			this.Receiver = reader;
		}

		public MessageClient Sender {get;set;}
		public MessageReader Receiver {get;set;}
	}
}

