using System;
using Actors;

namespace DistributedActors
{
	public class SystemInfoActor : DistributedActor
	{
		public SystemInfoActor (string shortName)
			: base(shortName)
		{
		}
	}
}

