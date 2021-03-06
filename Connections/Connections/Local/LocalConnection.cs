﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cls.Connections
{
    public class LocalConnection : IConnection
    {
        public LocalConnection(LocalSender s, LocalReceiver r)
        {
            Sender = s;
            Receiver = r;
			Received = new MessageQueue<object>();
			Receiver.Received.Subscribe(n=> Received.Post(n));
			IsAlive = true;
        }
        public event Action<IConnection> Disconnected;
		public bool IsAlive {get; private set;}
		public IEndPoint Remote {get { return new EndPoint("local");}}

        public void Send(object o)
        {
            Sender.Send(o);
        }

        public MessageQueue<object> Received { get; private set; }
        public ISender Sender {get; private set;}
        public IReceiver Receiver {get; private set;}
     
        public void Dispose()
        {
			IsAlive = false;
            Received.Dispose();
            Receiver.Dispose();
            Sender.Dispose();
        }

		public override string ToString ()
		{
			return string.Format ("[LocalConnection: IsAlive={0}, Received={1}, Sender={2}, Receiver={3}]", IsAlive, Received, Sender, Receiver);
		}
    }
}
