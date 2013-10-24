using System;
using System.Net.Sockets;
using Actors.Connections;
using Serialization;
using Actors.Connections.Bytes;
using System.Linq;
using Actors.Tasks;
using System.Diagnostics;

namespace Actors
{
	public class TcpByteConnection : IByteConnection
	{
        public TcpByteConnection(TcpClient c)
        {
            client = c;
            Sender = new TcpByteSender(c);
            Sender.Disconnected += HandleDisconnected;
            Receiver = new TcpByteReceiver(c);
            Receiver.Disconnected += HandleDisconnected;
            Receiver.Received += HandleReceived;
            
        }
        TcpClient client;
        public IByteSender Sender { get; private set; }
        public IByteReceiver Receiver { get; private set; }

        void HandleDisconnected(IByteSender s)
        {
            Disconnected.FireEventAsync(this);
        }
        void HandleDisconnected(IByteReceiver r) { Disconnected.FireEventAsync(this); }
        void HandleReceived(byte[] bytes)
        {
            Received.FireEventAsync(bytes);
        }        

		public void Dispose()
        {	
			client.Close();
            Sender.Disconnected -= HandleDisconnected;
            Receiver.Received -= HandleReceived;            
		}

      
        public event Action<byte[]> Received;        
        public event Action<IByteConnection> Disconnected;

        public void Send(byte[] d)
        {
            Sender.Send(d);
        }    
    }
}

