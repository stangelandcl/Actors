using System;


namespace Actors
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
			Dht = new DhtPeer(Id, new RpcSender(node, Id));
			functions.Add(Dht);
		}
	}
}

