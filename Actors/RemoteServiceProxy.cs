using System;
using System.Dynamic;

namespace NetworkTransport
{
	public class RemoteServiceProxy : DynamicObject
	{
		public RemoteServiceProxy (RpcDuplexClient rpc, string type)
		{
			this.rpc = rpc;
			this.type = type;
		}
		RpcDuplexClient rpc;
		string type;

		public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result)
		{
			var function = binder.Name;
			var msg = new RpcMessage{
				Type = type, Function = function, Args = args
			};
			rpc.Reader.Sender.Send(msg);
			if(binder.ReturnType != typeof(void)){
				result = rpc.Reader.Receive();
			}
			return true;
		}

	}
}

