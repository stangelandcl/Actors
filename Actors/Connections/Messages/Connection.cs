using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;



namespace Actors
{
    public class Connection : IConnection
    {        
        public Connection(IByteConnection c, ISerializer serializer)
        {
			this.serializer = serializer;
            this.ByteConnection = c;           
            this.Received = new MessageQueue<object>();
            this.ByteConnection.Received.Subscribe(HandleReceived);
            ByteConnection.Disconnected += HandleDisconnected;            
			IsAlive = true;			
        }
		ISerializer serializer;
		public IEndPoint Remote {get { return ByteConnection.Remote;}}
		public bool IsAlive {get; private set;}      
        public event Action<IConnection> Disconnected;
        public MessageQueue<object> Received { get; private set; }
		public IByteConnection ByteConnection {get; private set;}

		public override string ToString ()
		{
			return string.Format ("[Connection: IsAlive={0}, Received={1}]", IsAlive, Received);
		}

        void HandleReceived(byte[] bytes)
        {           
            Received.Post(serializer.Deserialize<object>(bytes));
        }

        void HandleDisconnected(IByteConnection c)
        {
            FireDisconnected();
        }

        void FireDisconnected()
        {
            Disconnected.FireEventAsync(this);
        }
        public void Send(object o)
        {
			ByteConnection.Send(serializer.SerializeToBytes(o));            
        }       

        public void Dispose()
        {
			IsAlive = false;
            ByteConnection.Disconnected -= HandleDisconnected;
			ByteConnection.Dispose();
            Received.Dispose();
        }
    }
}
