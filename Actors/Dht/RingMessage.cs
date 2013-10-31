using System;
using Actors;

namespace Dht
{
	public class RingMessage<T>
	{
		public int TimeToLive {get;set;}
		public IActorId Originator {get;set;}
		public T Message {get;set;}
	}
}

