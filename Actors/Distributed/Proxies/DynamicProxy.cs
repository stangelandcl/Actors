using System;
using System.Dynamic;


namespace Actors
{
    public interface IDynamicProxy : IDisposable
    {
        RemoteActor Proxy { get; }
    }
	public class DynamicProxy : DynamicObject, IDynamicProxy
	{
		public DynamicProxy (RemoteActor actor)
		{
			this.Proxy = actor;
		}

        public RemoteActor Proxy { get; private set; }

		public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result)
		{
            var isSend = binder.Name.StartsWith("Send");
            var name = binder.Name;
            if (isSend)
                name = binder.Name.Substring(4);
			var msg = Proxy.Send(name, args);
            if (!binder.Name.StartsWith("Send"))
                result = Proxy.Receive(msg).As<RpcMail>().Message.Args[0];
            else result = null;
			return true;
		}

		public override bool TryGetMember (GetMemberBinder binder, out object result)
		{
			var msg = Proxy.Send(binder.Name);
			result = Proxy.Receive(msg);
			return true;
		}

		public override bool TrySetMember (SetMemberBinder binder, object value)
		{
			Proxy.Send(binder.Name, value);
			return true;
		}

		public void Dispose(){
			Proxy.Dispose();
		}
	}
}

