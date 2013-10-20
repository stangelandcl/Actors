using System;
using System.Text;

namespace NetworkTransport
{
	public class RpcMessage
	{
		public string Type {get;set;}
		public string Function {get;set;}
		public object[] Args {get;set;}
	}

	public class RpcResponse{
		public RpcResponse(){}
		public RpcResponse(RpcMessage m, object returnValue, string error = null){
			Type = m.Type;
			Function = m.Function;
			ReturnValue = returnValue;
			Error = error;
		}
		public string Type {get;set;}
		public string Function {get;set;}
		public string Error {get;set;}
		public object ReturnValue {get;set;}
	}
}

