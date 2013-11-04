using System;
using System.Collections.Generic;

using System.Linq;
using Cls.Connections;

namespace Cls.Actors
{
	public class NodeMapActor : DistributedActor
	{
		public NodeMapActor (string shortName = "System.Nodes")
			: base(shortName)
		{
		}

		Dictionary<NodeId, List<string>> nodeToMachine = 
			new Dictionary<NodeId, List<string>>();

		public void Check(IConnection connection){
			connection.Send(new RpcMail{
				To = new ActorId("System.Nodes"),
				From = Id,
				Message = new FunctionCall("GetNodes"),
				MessageId = MessageId.New(),
			});
			lock(nodeToMachine)
			connection.Send(new RpcMail{
				To = new ActorId("System.Nodes"),
				From = Id,
				Message = new FunctionCall("GetNodesReply", nodeToMachine.ToDictionary(n=>n.Key, n=>n.Value)),
				MessageId = MessageId.New(),
			});
		}

		public string GetMachine(NodeId node){
			lock(nodeToMachine)
			{
				if(!nodeToMachine.ContainsKey(node))
					return null;
				return nodeToMachine[node].SingleOrDefault();
			}
		}

		public void Add(string machine, NodeId node){
			lock(nodeToMachine)
			{
				if(!nodeToMachine.ContainsKey(node))
					nodeToMachine[node] = new List<string>();
				if(!nodeToMachine[node].Contains(machine, StringComparer.OrdinalIgnoreCase))
					nodeToMachine[node].Add(machine);
			}
		}


		void GetNodes(RpcMail mail){
			lock(nodeToMachine)
				Node.Reply(mail, nodeToMachine.ToDictionary(n=>n.Key,n=>n.Value));
		}

		void GetNodesReply(RpcMail mail, Dictionary<NodeId, List<string>> nodes){
			lock(nodeToMachine)
			foreach(var kvp in nodes)
				foreach(var v in kvp.Value)
					Add(v, kvp.Key);									
		}
	}
}

