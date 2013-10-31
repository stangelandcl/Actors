using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using Serialization;
using Actors.Connections;
using Actors.Connections.Bytes;
using Actors.Connections.Messages;
using Actors.Connections.Local;
using Actors.Extensions;

namespace Actors
{
    public class ConnectionRouter
    {
        public ConnectionRouter(IConnection local)
        {
            localConnection = local;
        }
        
        IConnection localConnection;
        public event EventHandler<MissingEventArgs> ConnectionNotFound;
        public class MissingEventArgs : EventArgs{
            public IEndPoint EndPoint;
            public bool Added;
        }

        Dictionary<IEndPoint, List<IConnection>> connections = new Dictionary<IEndPoint, List<IConnection>>();        
 
        public IDisposable Add(IConnection connection, bool isOutbound)
        {
            lock (connections)
            {
                var endPoint = isOutbound ? connection.Sender.Remote : connection.Receiver.Remote;
                connections.GetOrAdd(endPoint).Add(connection);
            }
            connection.Disconnected += HandleDisconnected;
            return Disposable.New(obj => Remove(obj), connection);
        }

        void HandleDisconnected(IConnection connection)
        {
            Remove(connection);                
        }

        public void Remove(IConnection connection)
        {
            connection.Disconnected -= HandleDisconnected;
            
            lock (connections)
                foreach (var key in connections.Keys.ToArray())
                {
                    var list = connections[key];
                    var c = list.FindOrDefault(connection);
                    if (c != null)
                    {                        
                        if (list.Remove(c) && !list.Any())
                            connections.Remove(key);
                    }
                }
            connection.Dispose();
        }       
        public IConnection Get(IEndPoint computer)
        {
            var conn = GetInternal(computer);
            if (conn != null) return conn;           
            var added = new MissingEventArgs{EndPoint = computer};
            ConnectionNotFound.FireEvent(this, added);
            if (added.Added)
                return GetInternal(computer);
            return conn;                       
        }

        IConnection GetInternal(IEndPoint computer)
        {
            List<IConnection> c;
            lock (connections)
                foreach (var name in DnsAlias.Get(computer))
                    if (connections.TryGetValue(name, out c))
                        return c[0];
            return null;
        }

        public IConnection Get(ActorId id)
        {
            if (IsLocal(ref id))
                return localConnection;
            
            var s = id.ToString().Split('/');
            var computer = NormalizedComputerName(s[0]);
            return Get(new Actors.Connections.Bytes.EndPoint(computer));
        }

        private bool IsLocal(ref ActorId id)
        {
            return (id.Machine == null || id.Machine == Environment.MachineName) &&
                   (id.Node.IsEmpty || id.Node.ToString() == localConnection.Sender.Remote.ToString());
        }

        private string NormalizedComputerName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                return "localhost";
            return name;
        }
    }
}

