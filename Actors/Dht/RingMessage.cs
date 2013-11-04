using System;


namespace Cls.Actors
{
	public class RingMessage<T>
	{
		public int TimeToLive {get;set;}
		public IActorId Originator {get;set;}
		public T Message {get;set;}
	}
}

