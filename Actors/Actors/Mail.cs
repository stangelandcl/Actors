using System;
using System.Diagnostics;

namespace Actors
{

    [DebuggerDisplay("Name={Name} From={From} To={To} Args={Args.Length}")]
	public class Mail{
		public MessageId MessageId {get;set;}
		public ActorId From {get;set;}
		public ActorId To {get;set;}
		public FunctionId Name {get;set;}
		public object[] Args {get;set;}
	}
}

