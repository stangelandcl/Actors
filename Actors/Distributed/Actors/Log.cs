using System;
using System.Diagnostics;

namespace Actors
{
	public class Log
	{
		public static Log Get(DistributedActor actor){
			var name =  new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name;			
			return new Log(actor.Node, new ActorId("System.Log"), name);
		}

		public Log (Node node, IActorId logger, string name)
		{
			this.logger = logger;
			this.node = node;
			this.name = name;
		}
		string name;
		Node node;
		IActorId logger;

		public void Info(params object[] args){
			Write("INFO", args);
		}

		public void Error(params object[] args){
			Write("ERROR", args);
		}

		void Write(string type,  params object[] args){
			node.Default.SendTo(logger, "Write", name, type, args);
		}

	}
}

