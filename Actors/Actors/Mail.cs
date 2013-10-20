using System;

namespace Actors
{
	public class Mail{
		public MessageId MessageId {get;set;}
		public ActorId From {get;set;}
		public ActorId To {get;set;}
		public FunctionId Name {get;set;}
		public object[] Args {get;set;}
	}
}

