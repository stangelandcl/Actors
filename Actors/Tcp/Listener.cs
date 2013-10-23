using System;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using Actors.Tasks;

namespace Actors
{
	public class Listener : IDisposable
	{
		public Listener(TcpListener listener){
			TcpListener = listener;
			TcpListener.Start();
			TcpListener.BeginAcceptSocket(EndAccept, null);
		}
		public Listener (string host, int port)
			: this(new TcpListener(IPAddress.Parse(host), port))
		{}

		public static implicit operator Listener(TcpListener l){
			return new Listener(l);
		}
		public static implicit operator TcpListener(Listener l){
			return l.TcpListener;
		}

		public TcpListener TcpListener {get; private set;}
		public event Action<TcpClient> Accepted;
	
		void EndAccept (IAsyncResult ar)
		{
            try
            {
                var socket = TcpListener.EndAcceptTcpClient(ar);
                if (Accepted != null)
                    TaskEx.Run(() => Accepted(socket));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Accept failed "+ ex);
            }
            finally
            {
                TcpListener.BeginAcceptSocket(EndAccept, null);
            }

		}

		public void Dispose(){
			TcpListener.Stop();
		}
	}
}

