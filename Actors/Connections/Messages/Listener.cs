using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Actors
{
    public class Listener : IListener
    {
        public Listener(IByteListener listener, ISerializer serializer)
        {
            this.listener = listener;
            this.serializer = serializer;
            this.listener.Connected += HandleConnected;
        }
        IByteListener listener;
        ISerializer serializer;
        public event Action<IConnection> Connected;
        void HandleConnected(IByteConnection conn)
        {
            Connected.FireEventAsync(new Connection(conn, serializer));
        }
        public void Dispose()
        {
            listener.Connected -= HandleConnected;
            listener.Dispose();
        }

    }
}
