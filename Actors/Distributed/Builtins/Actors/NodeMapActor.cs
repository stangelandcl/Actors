using System;
using System.Collections.Generic;

using System.Linq;
using Cls.Connections;
using Cls.Extensions;

namespace Cls.Actors
{
	public class NodeMapActor : DistributedActor
	{
		public NodeMapActor (string shortName = "System.Nodes")
			: base(shortName)
		{

		}

		public event Action<IEndPoint, NodeId> NodeFound;

		Dictionary<NodeId, List<IEndPoint>> nodeToMachine = 
			new Dictionary<NodeId, List<IEndPoint>>();

		Log log = Log.Get();

		public void Check(IConnection connection){
			log.Info("Check");
			connection.Send(new RpcMail{
				To = new ActorId("System.Nodes"),
				From = Id,
				Message = new FunctionCall("GetNodes", connection.Remote),
				MessageId = MessageId.New(),
			});
		}

		public IEndPoint GetMachine(NodeId node){
			lock(nodeToMachine)
			{
				if(!nodeToMachine.ContainsKey(node))
					return null;
				return nodeToMachine[node].SingleOrDefault();
			}
		}

		public void Add(IEndPoint machine, NodeId node){
			lock(nodeToMachine)
			{
				if(!nodeToMachine.ContainsKey(node))
					nodeToMachine[node] = new List<IEndPoint>();
				if(!nodeToMachine[node].Contains(machine))
					nodeToMachine[node].Add(machine);
			}
			NodeFound.FireEventAsync (machine, node);
		}


		void GetNodes(RpcMail mail, IEndPoint ep){
			log.Info("GetNodes " + ep);
			lock(nodeToMachine)
				Node.Reply(mail,ep, Node.Id, nodeToMachine.ToDictionary(n=>n.Key,n=>n.Value));
		}

		void GetNodesReply(RpcMail mail,IEndPoint ep, NodeId id, Dictionary<NodeId, List<IEndPoint>> nodes){
			log.Info ("GetNodesReply", ep, id, nodes.Count);
			lock (nodeToMachine) {
				Add (ep, id);
				foreach (var kvp in nodes)
					foreach (var v in kvp.Value)
						Add (v, kvp.Key);									
			}
		}
	}
}

