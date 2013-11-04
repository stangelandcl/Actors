using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using Cls.Extensions;
using System.Net;

namespace Cls.Connections
{
    public abstract class TcpListener<T> 
    {
        public TcpListener(string host, int port)
            : this(new TcpListener(IPAddress.Parse(host.Replace("localhost", "127.0.0.1")), port))
        { }

        public TcpListener(TcpListener l)
        {
            listener = l;
            listener.Start();
            listener.BeginAcceptSocket(EndAccept, null);
        }
        TcpListener listener;

        public event Action<T> Connected;

        void HandleAccepted(System.Net.Sockets.TcpClient obj)
        {
            if (Connected != null)
                Connected(OnConnect(obj));
        }

        void EndAccept(IAsyncResult ar)
        {
            try
            {
                var socket = listener.EndAcceptTcpClient(ar);
                Connected.FireEventAsync(OnConnect(socket));
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

        protected abstract T OnConnect(TcpClient c);

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
