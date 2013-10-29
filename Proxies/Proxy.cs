using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Threading.Tasks;


namespace Proxies
{
    public abstract class Proxy : RealProxy
    {
        public Proxy(Type t)
            : base(t)
        { }        

        public override IMessage Invoke(IMessage message)
        {
            var methodCall = (IMethodCallMessage)message;
            var method = (MethodInfo)methodCall.MethodBase;
            object[] outArgs;
            var returnValue = Invoke(method, methodCall, out outArgs);                        
            return new ReturnMessage(returnValue, outArgs, outArgs == null ? 0 : outArgs.Length,
                methodCall.LogicalCallContext, methodCall);
        }

        protected abstract object Invoke(MethodInfo method, IMethodCallMessage call, out object[] outArgs);
               
    }


}

