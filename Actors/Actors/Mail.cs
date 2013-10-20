using System;

namespace Actors
{
	public class Mail{
		public int MessageId {get;set;}
		public int SenderId {get;set;}
		public int ReceiverId {get;set;}
		public string Name {get;set;}
		public object[] Args {get;set;}
	}
}

