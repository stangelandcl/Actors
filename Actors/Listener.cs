using System;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;

namespace NetworkTransport
{
	public class Listener : IDisposable
	{
		public Listener (string host, int port)
		{
			listener = new TcpListener(IPAddress.Parse(host), port);
			listener.BeginAcceptSocket(EndAccept, null);
		}

		TcpListener listener;
		public event Action<TcpClient> Accepted;
	
		void EndAccept (IAsyncResult ar)
		{
			var socket = listener.EndAcceptTcpClient(ar);
			if(Accepted != null)
				Accepted(socket);
		}

		public void Dispose(){
			listener.Stop();
		}
	}
}

