using System;
using System.Net.Sockets;

namespace Actors
{
	public class MessageConnection : IDisposable
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

		public void Dispose(){	
			Sender.Client.Close();
		}
	}
}

