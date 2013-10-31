using System;
using System.Net.Sockets;
using Actors.Connections.Bytes;
using System.Diagnostics;
using System.Net;
using Actors.Extensions;

namespace Actors
{
	public class TcpByteListener : IByteListener	
    {
        public TcpByteListener(string host, int port)
            : this(new TcpListener(IPAddress.Parse(host.Replace("localhost", "127.0.0.1")), port))
        {}        

        public TcpByteListener(TcpListener l)
		{
            listener = l;
            listener.Start();
            listener.BeginAcceptSocket(EndAccept, null);
		}
        TcpListener listener;

		public event Action<IByteConnection> Connected;

		void HandleAccepted (System.Net.Sockets.TcpClient obj)
		{
			if(Connected != null)
				Connected(new TcpByteConnection(obj));
		}	

        void EndAccept(IAsyncResult ar)
        {
            try
            {
                var socket = listener.EndAcceptTcpClient(ar);
                Connected.FireEventAsync(new TcpByteConnection(socket));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Accept failed " + ex);
            }
            finally
            {
                listener.BeginAcceptSocket(EndAccept, null);
            }

        }

        public void Dispose()
        {
            listener.Stop();
        }
	}
}

