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
        public T New<T>(string name)
        {
            return (T)New(name, typeof(T));
        }
       
        public T New<T>(string name, ActorId remote)
        {
            return (T)New(name, remote, typeof(T));
        }
        public T New<T>(RemoteActor a)
        {
            return (T)New(a, typeof(T));
        }
        public object New(string name, ActorId remote, Type t)
        {
            return New(new RemoteActor(name, remote), t);
        }

        public object New(string name, Type t)
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
        public dynamic New(string name)
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
        private string GenerateProxyName(string name)
        {
            return "proxy" + Interlocked.Increment(ref id) + "-" + name;
        }

        private void Attach(RemoteActor a)
        {
            node.Add(a);
        }
    }
}
