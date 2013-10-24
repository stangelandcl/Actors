using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

namespace Actors.Proxies
{
    class RemoteActorProxy : RealProxy
    {
        public RemoteActorProxy(RemoteActor remote, Type t)
            : base(t)
        {
            this.Remote = remote;
        }
        public RemoteActor Remote { get; private set; }

        public override IMessage Invoke(IMessage message)
        {
            var methodCall = (IMethodCallMessage)message;
            var method = (MethodInfo)methodCall.MethodBase;
            var msg = Remote.Send(method.Name, methodCall.Args);
            object returnValue = null;
            if (method.ReturnType != typeof(void))
            {
                var mail = Remote.Receive(msg);
                returnValue = mail.Args[0].Convert(method.ReturnType);                
            }
            return new ReturnMessage(returnValue, null, 0, methodCall.LogicalCallContext, methodCall);            
        }
    }
}
