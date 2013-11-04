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

		protected Log log;
		public override void AttachNode (Node node)
		{
			base.AttachNode (node);
			log = Log.Get(this);
		}
	}
}

