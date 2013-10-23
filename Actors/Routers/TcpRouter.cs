using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using Serialization;

namespace Actors
{
	public class TcpRouter 
	{	
		public TcpRouter(ISerializer serializer){
			this.serializer = serializer;
		}
		ISerializer serializer;

		Dictionary<string, List<MessageTConnection>> connections = new Dictionary<string, List<MessageTConnection>>();
		public event Action<Mail> Received;
	
		public void Connect(string host, int port){
			var connection = new MessageTConnection(new TcpClient(host, port), serializer);
			Add(connection);
		}

		public void Add(MessageTConnection c){
			Add(((IPEndPoint)c.Sender.Client.Client.Client.RemoteEndPoint).Address.ToString(), c);
		}

		public void Add(string computer, MessageTConnection c){
			lock(connections)
				connections.GetOrAdd(computer).Add(c);
            c.Disconnected += HandleDisconnected;
			c.Receiver.Received += HandleReceived;
		}

      
		public void Remove(MessageTConnection c){
			lock(connections){
				foreach(var conn in connections.ToArray())
				{
					var l = conn.Value;
					for(int i=l.Count-1;i>=0;i--)
						if(l[i] == c){
							l[i].Receiver.Received -= HandleReceived;
							l.RemoveAt(i);					
						}
					if(l.Count == 0)
						connections.Remove(conn.Key);
				}
			}
		}
		void HandleReceived (object obj){
			if(Received != null && obj is Mail)
				Received((Mail)obj);
		}
		public MessageTConnection Get (string computer){
			List<MessageTConnection> c;
			lock(connections)
				foreach(var name in DnsAlias.Get(computer))
					if(connections.TryGetValue(name, out c))
						return c[0];
			return null;
		}

		public MessageTConnection Get(ActorId id){
			var s = id.ToString().Split('/');
			return Get(s[0]);
		}
	}
}

