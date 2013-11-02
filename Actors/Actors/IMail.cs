using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Actors
{
    public interface IMail<T> : IMail
    {
		IActorId From { get; set; }
		IActorId To { get;set ; }
		IMessageId MessageId {get; set;}
        T Message { get; set; }
    }
    public interface IMail 
    {
    }
    public struct FunctionCall{
		public FunctionCall(string name, params object[] args){
			Name = name; Args = args;
		}
        public string Name;
        public object[] Args;
    }
    public interface IRpcMail : IMail<FunctionCall> { }

	[DebuggerDisplay("Name={Name} From={From} To={To} Args={Args.Length}")]
	public class RpcMail : IRpcMail
	{
		/// <summary>
		/// Used so a receiver can wait for a reply to this specific request.        
		/// </summary>
		public IActorId From { get; set; }
		public IActorId To { get; set; }
		public IMessageId MessageId { get; set; }
		public FunctionCall Message { get; set; }
	}
}
