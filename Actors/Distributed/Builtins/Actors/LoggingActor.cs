using System;

namespace Cls.Actors
{
	public abstract class LoggingActor : DistributedActor
	{
		public LoggingActor(string shortName)
			: base(shortName)
		{}
		public LoggingActor(ActorId id, Node node)
			: base(id, node)
		{}

		protected Log log = Log.Get();
	}
}

