using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Cls.Extensions;


namespace Cls.Connections
{
    public class Listeners
    {
        List<IListener> listeners = new List<IListener>();
        public IDisposable Add(IListener t)
        {
            t.Connected += HandleConnected;
            lock(listeners)
                listeners.Add(t);
            return Disposable.New(o => Remove(o), t);
        }
        public void Remove(IListener t)
        {
            lock (listeners)
                listeners.Remove(t);
            t.Connected -= HandleConnected;
        }

        public event Action<IConnection> Connected;
        void HandleConnected(IConnection t)
        {
            Connected.FireEventAsync(t);
        }
    }
}
