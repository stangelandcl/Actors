using System;


namespace Cls.Actors
{
	public class DhtActor : DistributedActor
	{
		public DhtActor(string shortName = "System.Dht")
			: base(shortName)
		{ 

		}		
		public DhtPeer Dht {get; private set;}

		public override void AttachNode (Node node)
		{
			base.AttachNode(node);
			var log = Log.Get();
			Dht = new DhtPeer(log, Id, new RpcSender(node, Id));
			functions.Add(Dht);
		}
	}
}

