using System;
using System.Linq;

namespace Actors
{
	public class ProcessActor : DistributedActor
	{
		public ProcessActor (string shortName = "System.Process")
			: base(shortName)
		{
			log = Log.Get(this);
		}
		Log log;

		void GetProcesses(IRpcMail mail){
			log.Info("GetProcesses " + mail.From);
			var actors = Node.World.Actors
					.OrderBy(n=>n.Id)
					.Select(n=> new ActorInfo{
							Id = n.Id,
							IsAlive = n.IsAlive,
						})
					.ToArray();
			Node.Reply(mail, actors);
		}
	}

	public class ActorInfo{
		public ActorId Id {get;set;}
		public bool IsAlive {get;set;}
	}

	public interface IProcess{
		ActorInfo[] GetProcesses();
	}
}

