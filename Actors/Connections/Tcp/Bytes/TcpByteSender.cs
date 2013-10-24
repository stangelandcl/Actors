using System;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;
using Actors.Connections.Bytes;
using Actors.Tasks;
using System.Net;

namespace Actors
{
    public class TcpByteSender : IByteSender
    {
        public TcpByteSender(TcpClient client)
        {
            //client.NoDelay = true;
            this.Client = client;
        }

        public TcpClient Client { get; private set; }
        public IEndPoint Remote { get { return new Actors.Connections.Bytes.EndPoint(((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString()); } }
        public event Action<IByteSender> Disconnected;
        public void Send(byte[] bytes)
        {
            SendRaw(BitConverter.GetBytes(bytes.Length), bytes);
        }

        void SendRaw(params byte[][] buffers)
        {
            try
            {
                var b = buffers.Select(n => new ArraySegment<byte>(n)).ToArray();
                Client.Client.BeginSend(b, SocketFlags.None, EndSend, b);
            }
            catch (SocketException)
            {
                if (!Client.Connected)
                    Disconnected.FireEventAsync(this); 
            }
        }

        void EndSend(IAsyncResult ar)
        {
            try
            {
                var s = (ArraySegment<byte>[])ar.AsyncState;
                int count = Client.Client.EndSend(ar);
                int length = s.Sum(n => n.Count);
                if (count == length)
                    return;
                var buffer = s.SelectMany(n => n.Array.Skip(n.Offset).Take(n.Count)).Skip(count).ToArray();
                SendRaw(buffer);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error " + ex);
            }
        }

    }
}

