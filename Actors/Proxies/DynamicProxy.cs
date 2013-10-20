using System;
using System.Dynamic;

namespace Actors
{
//	public class DynamicProxy : DynamicObject
//	{
//		public DynamicProxy (RemoteActor actor)
//		{
//			this.actor = actor;
//		}
//
//		RemoteActor actor;
//
//		public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result)
//		{
//			var msg = actor.Send(binder.Name, args);
//			if(binder.ReturnType != typeof(void))
//				result = actor.Receive(msg).Args[0];
//			else result = null;
//			return true;
//		}
//
//		public override bool TryGetMember (GetMemberBinder binder, out object result)
//		{
//			var msg = actor.Send(binder.Name);
//			result = actor.Receive(msg);
//			return true;
//		}
//
//		public override bool TrySetMember (SetMemberBinder binder, object value)
//		{
//			actor.Send(binder.Name, value);
//			return true;
//		}
//	}
}

