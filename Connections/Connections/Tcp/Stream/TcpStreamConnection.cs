using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Cls.Connections
{
    public class TcpStreamConnection : ByteConnection
    {
        public TcpStreamConnection(TcpClient c, System.IO.Stream s)
            : base(
            new StreamByteSender(Cls.Connections.EndPoint.GetRemote(c), s),
            new StreamByteReceiver(Cls.Connections.EndPoint.GetRemote(c), s))
        {
            client = c;
            sender.Error += HandleError;
            receiver.Error += HandleError;
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
            sender.Error -= HandleError;
            receiver.Error -= HandleError;
            base.Dispose();
            client.Close();
        }
    }
}
