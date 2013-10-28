using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Actors.Proxies
{
    public class ProxyFactory
    {
        public ProxyFactory(Node node)
        {
            this.node = node;
        }
        Node node;

        static long id;
        public T New<T>(ActorId name)
        {
            return (T)New(name, typeof(T));
        }
           
        public T New<T>(RemoteActor a)
        {
            return (T)New(a, typeof(T));
        }
       
        public object New(ActorId name, Type t)
        {
            return New(new RemoteActor(GenerateProxyName(name), name), t);
        }

        public object New(RemoteActor a, Type t)
        {
            Attach(a);
            var o = new RemoteActorProxy(a, t).GetTransparentProxy();
            if (o == null) throw new Exception("Could not generate proxy for " + t.Name);
            return o;
        }
        public dynamic New(ActorId name)
        {
            return New(GenerateProxyName(name), name);
        }
        public dynamic New(string name, ActorId remote)
        {
            return New(new RemoteActor(name, remote));
        }
        public dynamic New(RemoteActor a)
        {
            Attach(a);
            return new DynamicProxy(a);
        }
        private string GenerateProxyName(ActorId name)
        {
            return "proxy-" + Guid.NewGuid().ToString().Substring(0, 8) + name.Name;                
        }

        private void Attach(RemoteActor a)
        {
            node.Add(a);
        }
    }
}
