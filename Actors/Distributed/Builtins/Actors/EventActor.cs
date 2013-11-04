using System;
using System.Linq.Expressions;
using Serialize.Linq;
using Serialize.Linq.Serializers;

namespace Cls.Actors
{
	public class EventActor : DistributedActor
	{
		public EventActor (string shortName = "System.Events")
			: base(shortName)
		{
		}

		ExpressionSerializer serializer;

		void Subscribe(IRpcMail mail, Expression<Func<IRpcMail, bool>> subscribe){

		}

		protected override void HandleMessage (IRpcMail mail)
		{
			base.HandleMessage (mail);
		} 
	}
}

