using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Threading.Tasks;
using Fasterflect;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;


namespace Cls.Actors
{
    class RemoteActorProxy : Proxy
    {
        public RemoteActorProxy(RemoteActor remote, Type t)
            : base(t)
        {
            this.Remote = remote;
        }
        public RemoteActor Remote { get; private set; }

		protected override object Invoke (MethodInfo method, IMethodCallMessage call, out object[] outArgs)
		{
			outArgs = null;
			var msg = Remote.Send(method.Name, call.Args);
			return GetReturnValue(method, msg);            			
		}
		      
        private object GetReturnValue(MethodInfo method, IMessageId msg)
        {
            if (method.ReturnType == typeof(void)) return null;
            if (method.IsGenericMethod && typeof(Task).IsAssignableFrom(method.ReturnType))
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
				var mail = Remote.Receive(msg).Result;
                if (!mail.HasValue || mail.Value.As<RpcMail>().Message.Args.Length == 0)
                {
                    if (method.ReturnType.IsClass) return null;
                    return method.ReturnType.CreateInstance();
                }
                return mail.Value.As<RpcMail>().Message.Args[0].Convert(method.ReturnType);
            }
        }
    }


}
