using System;
using System.Collections.Generic;
using Serialization;
using System.Collections.Concurrent;

namespace NetworkTransport
{
	public class RpcMessageReader
	{
		public RpcMessageReader (MessageReader reader, ISerializer serializer,  Dictionary<string, Type> handlers = null)
		{
			this.Reader = reader;
			this.handlers = handlers ?? new Dictionary<string, Type>();
			this.serializer = serializer;
			this.Sender = new RpcSender(reader.Client, serializer);
			reader.MessageReceived += HandleMessageReceived;
		}

		public MessageReader Reader {get; private set;}
		Dictionary<string, Type> handlers;
		Dictionary<string, object> instances;
		ISerializer serializer;
		public RpcSender Sender {get; private set;}
		ConcurrentQueue<object> received  = new ConcurrentQueue<object>();

		public void Add<T>(){
			handlers[typeof(T).Name] = typeof(T);
		}

		public object Receive(){

		}

		void HandleMessageReceived (Message obj)
		{
			var o = serializer.Deserialize(obj.Buffer);
			var rpc = o as RpcMessage;
			if(rpc == null){
				var response = o as RpcSender
			}

			var instance = GetInstance(rpc.Type);
			if(instance == null)
				return;

			var method = instance.GetType().GetMethod(rpc.Function);
			var result = method.Invoke(instance, rpc.Args);
			if(method.ReturnType != typeof(void))
			{
				var response = new RpcResponse(rpc, result);
				Sender.Send(response);
			}
		}

		object GetInstance (string type)
		{
			object o = null;
			if(!instances.TryGetValue(type, out o)){
				Type t;
				if(handlers.TryGetValue(type, out t))
					instances[type] = o = Activator.CreateInstance(t, new object[]{Sender});				
			}
			return o;
		}
	}
}

