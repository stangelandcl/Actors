using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;



namespace Actors
{
    public class Connection : IConnection
    {
        public Connection(IByteConnection b, ISerializer serializer)
            : this(b, new Sender(b.Sender, serializer),
                   new Receiver(b.Receiver, serializer))
        { }

        /// <summary>
        /// Don't make this public. higher level class must be
        /// used to determine if sender.error/receiver.error = disconnect
        /// so we use byteconnection to do that
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        Connection(IByteConnection c, ISender sender, IReceiver receiver)
        {
            this.connection = c;
            this.Sender = sender;
            this.Receiver = receiver;
            this.Received = new MessageQueue<object>();
            this.Receiver.Received.Subscribe(HandleReceived);
            connection.Disconnected += HandleDisconnected;            
			IsAlive = true;
        }
		public bool IsAlive {get; private set;}
        public ISender Sender { get; private set; }
        public IReceiver Receiver { get; private set; }
        public event Action<IConnection> Disconnected;
        public MessageQueue<object> Received { get; private set; }
        IByteConnection connection;

        void HandleReceived(object ob)
        {           
            Received.Post(ob);
        }

        void HandleDisconnected(IByteConnection c)
        {
            FireDisconnected();
        }

        void FireDisconnected()
        {
            Disconnected.FireEventAsync(this);
        }
        public void Send(object o)
        {
            Sender.Send(o);
        }       

        public void Dispose()
        {
			IsAlive = false;
            connection.Disconnected -= HandleDisconnected;
            Sender.Dispose();
            Receiver.Dispose();
            Received.Dispose();
        }
    }
}
