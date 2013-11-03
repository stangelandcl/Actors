using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Actors
{
    public class TcpStreamConnection : ByteConnection
    {
        public TcpStreamConnection(TcpClient c, System.IO.Stream s)
            : base(
            new StreamByteSender(Actors.EndPoint.GetRemote(c), s),
            new StreamByteReceiver(Actors.EndPoint.GetRemote(c), s))
        {
            client = c;
            Sender.Error += HandleError;
            Receiver.Error += HandleError;
        }
        TcpClient client;
        protected override void HandleError(Exception e)
        {
            if (!client.Connected)
            {
                FireDisconnected();
                Dispose();
            }
        }

        public override void Dispose()
        {
            Sender.Error -= HandleError;
            Receiver.Error -= HandleError;
            base.Dispose();
            client.Close();
        }
    }
}
