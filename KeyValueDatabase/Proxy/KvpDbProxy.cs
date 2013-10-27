using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using Fasterflect;

namespace KeyValueDatabase.Proxy
{
    class KvpDbProxy : RealProxy
    {
        public KvpDbProxy(IKvpDb db, Type t)
            : base(t)
        {
            this.db = db;
        }
        IKvpDb db;
        Dictionary<string, object> dbsets = new Dictionary<string, object>();

        public override IMessage Invoke(IMessage message)
        {
            var methodCall = (IMethodCallMessage)message;
            var method = (MethodInfo)methodCall.MethodBase;
            object ret = null;
            var args = methodCall.Args;
            switch (method.Name)
            {
                case "Get": ret = db.Get(args[0]); break;
                case "Add": db.Add(args[0], args[1]); break;
                case "Remove": db.Remove(args[0]); break;
                case "AddRange": db.AddRange((IEnumerable<KeyValuePair<object,object>>)args[0]); break;                            
                default:
                    if (method.Name.StartsWith("get_"))
                    {
                        var name = method.Name.Substring(4);      
                        switch(name){
                            case "Keys": ret = db.Keys; break;
                            case "Values": ret = db.Values; break;
                            case "Items": ret = db.Items; break;  
                            case "Database": ret = db.Database; break;
                            case "Serializer": ret = db.Serializer; break;
                            default:         
                                lock(dbsets)
                                if (!dbsets.TryGetValue(name, out ret))
                                {
                                    var type = method.ReturnType.GetGenericArguments()[0];
                                    var proxyType = typeof(KvpDbSetProxy<>).MakeGenericType(type);
                                    dbsets[name] = ret = proxyType.CreateInstance(db, name);
                                }
                                break;
                        }                                              
                    }
                    break;
            }


           
            return new ReturnMessage(ret,
                   null, 0, methodCall.LogicalCallContext,
                   methodCall);           
        }
    }
}
