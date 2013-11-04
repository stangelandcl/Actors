using System;
using System.Linq;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;

namespace Cls.Actors
{
	public class ProcessActor : DistributedActor
	{
		public ProcessActor (string shortName = "System.Process")
			: base(shortName)
		{}			
		Log log;

		public override void AttachNode (Node node)
		{
			base.AttachNode (node);
			log = Log.Get(this);
		}

		void GetConnections(IRpcMail mail){
			log.Info("GetConnections " + mail.From);
			var conn = Node.Router.Connections.ToArray(n=>n.ToString());
			Node.Reply(mail, new[]{conn});
		}

		void GetProcesses(IRpcMail mail){
			log.Info("GetProcesses " + mail.From);
			var actors = Node.World.Actors					
					.Select(n=> new ActorInfo{
							Id = n.Id,
							IsAlive = n.IsAlive,
						})
					.OrderBy(n=>n.IsAlive)
					.ThenBy(n=>n.Id)
					.ToArray();		
			Node.Reply(mail, new[]{actors});	
		}
	}
	
	public class ActorInfo{
		public ActorId Id {get;set;}
		public bool IsAlive {get;set;}
		public override string ToString ()
		{
			return string.Format ("[ActorInfo: Id={0}, IsAlive={1}]", Id, IsAlive);		
		}
	
	}

	public interface IProcess{
		ActorInfo[] GetProcesses();
		string[] GetConnections();
	}
}

