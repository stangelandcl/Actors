using System;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.IO;


namespace Actors
{
    public class StreamByteSender : IByteSender
    {
        public StreamByteSender(IEndPoint remote, Stream client)
        {            
            this.Remote = remote;
            this.client = client;
        }

        Stream client;
        public event Action<Exception> Error;
        public IEndPoint Remote { get; private set; }

        class Message
        {
            public byte[] Bytes;
            public int Offset;
        }

        public void Send(byte[] bytes)
        {
            var b = new byte[bytes.Length + 4];
            var buffer = new Message { Bytes = b.Write(bytes.Length).Write(bytes, 4) };                       
            SendRaw(buffer);          
        }

        void SendRaw(Message buffer)
        {
            try
            {               
                client.BeginWrite(buffer.Bytes, buffer.Offset, buffer.Bytes.Length, EndSend, buffer);
            }
            catch (Exception e)
            {
                Error.FireEventAsync(e);
            }
        }

        void EndSend(IAsyncResult ar)
        {
            try
            {
                var buffer = (Message)ar.AsyncState;
                int count = client.EndRead(ar);
                if (count <= 0) return;
                buffer.Offset += count;
                if (buffer.Offset != buffer.Bytes.Length)
                    SendRaw(buffer);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error " + ex);
            }
        }

    }
}

