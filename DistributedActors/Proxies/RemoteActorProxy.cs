using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Threading.Tasks;
using Fasterflect;

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
            var returnValue = GetReturnValue(method, msg);            
            return new ReturnMessage(returnValue, null, 0, methodCall.LogicalCallContext, methodCall);            
        }

        private object GetReturnValue(MethodInfo method, IMessageId msg)
        {
            if (method.ReturnType == typeof(void)) return null;
            if (method.IsGenericMethod && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return TaskEx.New(() =>
                {
                    var mail = Remote.Receive(msg);
                    var returnType = method.ReturnType.GetGenericArguments()[0];
                    return mail.As<RpcMail>().Message.Args[0].Convert(returnType);
                }, method.ReturnType);
            }
            else
            {
                var mail = Remote.Receive(msg);
                if (mail == null || mail.As<RpcMail>().Message.Args.Length == 0)
                {
                    if (method.ReturnType.IsClass) return null;
                    return method.ReturnType.CreateInstance();
                }
                return mail.As<RpcMail>().Message.Args[0].Convert(method.ReturnType);
            }
        }
    }


}
