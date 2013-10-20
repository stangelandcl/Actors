using System;
using Serialization;

namespace Actors
{
	public interface ISender
	{
		void Send(object msg);
	}

	public class TcpSender
	{
		public TcpSender(MessageTClient client){
			this.client = client;
		}
		MessageTClient client;

		public void Send(object o){
			client.Send(o);
		}
	}
}

