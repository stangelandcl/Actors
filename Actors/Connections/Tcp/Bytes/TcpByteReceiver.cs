using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Actors.Connections.Bytes;
using Actors.Tasks;
using System.Net;

namespace Actors
{
    public class TcpByteReceiver : IByteReceiver
    {
        public TcpByteReceiver(TcpClient client)
        {
            this.client = client;
            Listen();
        }
        public event Action<byte[]> Received;
        public event Action<IByteReceiver> Disconnected;
        public IEndPoint Remote
        {
            get { return new Actors.Connections.Bytes.EndPoint(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()); }
        }

        TcpClient client;

        public class TcpFrame
        {
            public byte[] Buffer;
            public int Count;
        }

        void Listen()
        {
            //create new message. this is important.
            var message = new TcpFrame { Buffer = new byte[4] };
            Listen(message, EndReadHeader);
        }

        void Listen(TcpFrame message, AsyncCallback cb)
        {
            client.Client.BeginReceive(message.Buffer, message.Count, message.Buffer.Length - message.Count, SocketFlags.None, cb, message);
        }

        void EndReadHeader(IAsyncResult ar)
        {
            try
            {
                var msg = (TcpFrame)ar.AsyncState;
                int count = client.Client.EndReceive(ar);
                if (count <= 0) return;
                msg.Count += count;
                if (msg.Count == msg.Buffer.Length)
                {
                    msg.Count = 0;
                    msg.Buffer = new byte[BitConverter.ToInt32(msg.Buffer, 0)];
                    Listen(msg, EndReadMessage);
                }
                else
                {
                    Listen(msg, EndReadHeader);
                }
            }
            catch (SocketException)
            {
                if (!client.Connected)
                    Disconnected.FireEventAsync(this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in read header" + ex);
            }
        }

        void EndReadMessage(IAsyncResult ar)
        {
            try
            {
                var msg = (TcpFrame)ar.AsyncState;
                int count = client.Client.EndReceive(ar);
                if (count <= 0) return;
                msg.Count += count;
                if (msg.Count > msg.Buffer.Length)
                    return;
                if (msg.Count == msg.Buffer.Length)
                {
                    Received.FireEventAsync(msg.Buffer);
                    Listen();
                }
                else
                {
                    Listen(msg, EndReadMessage);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in read message " + ex);
            }
        }       
    }
}

