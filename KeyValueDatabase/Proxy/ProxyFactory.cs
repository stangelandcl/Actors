using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueDatabase.Proxy
{
    public static class ProxyFactory
    {
        public static T New<T>()
        {
            return New<T>(new MemoryKvpDb());
        }

        public static T New<T>(IKvpDb backend)
        {
            return (T)new KvpDbProxy(backend, typeof(T)).GetTransparentProxy();
        }
    }
}
