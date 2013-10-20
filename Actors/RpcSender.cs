using System;
using Serialization;

namespace NetworkTransport
{
	public class RpcSender
	{
		public RpcSender(MessageClient client, ISerializer serializer){
			this.client = client;
			this.serializer = serializer;
		}

		MessageClient client;
		ISerializer serializer;

		public void Send(RpcMessage m){
			client.Send(serializer.Serialize(m));
		}
		public void Send(RpcResponse m){
			client.Send(serializer.Serialize(m));
		}

		public void Send(string type, string function, params object[] args){
			Send(new RpcMessage{
				Type = type, Function =function, Args = args
			});
		}

		public void Send<T>(string function, params object[] args){
			Send(typeof(T).Name, function, args);
		}
	}
}

