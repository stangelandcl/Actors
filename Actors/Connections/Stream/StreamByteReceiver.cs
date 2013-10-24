using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Actors.Connections.Bytes;
using Actors.Tasks;
using System.Net;
using System.IO;

namespace Actors
{
    public class StreamByteReceiver : IByteReceiver
    {
        public StreamByteReceiver(IEndPoint remote, Stream client)
        {
            Remote = remote;
            this.client = client;
            Listen();
        }
        public event Action<byte[]> Received;
        public event Action<Exception> Error;
        public IEndPoint Remote { get; private set; }       
        Stream client;

        class Frame
        {
            public byte[] Buffer;
            public int Count;
        }

        void Listen()
        {
            //create new message. this is important.
            var message = new Frame { Buffer = new byte[4] };
            Listen(message, EndReadHeader);
        }

        void Listen(Frame message, AsyncCallback cb)
        {
            client.BeginRead(message.Buffer, message.Count, message.Buffer.Length - message.Count, cb, message);
        }

        void EndReadHeader(IAsyncResult ar)
        {
            try
            {
                var msg = (Frame)ar.AsyncState;
                int count = client.EndRead(ar);
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
            catch (Exception e)
            {
                Error.FireEventAsync(e);
            }
        }

        void EndReadMessage(IAsyncResult ar)
        {
            try
            {
                var msg = (Frame)ar.AsyncState;
                int count = client.EndRead(ar);
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
                Error.FireEventAsync(ex);
            }
        }       
    }
}

