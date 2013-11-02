using System;
using System.Linq;

namespace Actors
{
	public class ProcessActor : LoggingActor
	{
		public ProcessActor (string shortName = "System.Process")
			: base(shortName)
		{}			


		void GetConnections(IRpcMail mail){
			log.Info("GetConnections " + mail.From);

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
