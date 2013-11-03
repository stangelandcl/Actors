using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Actors
{
    public class ByteConnection : IByteConnection
    {       
        public ByteConnection(IByteSender sender, IByteReceiver receiver)
        {               
			this.sender = sender;
            this.receiver = receiver;
    		receiver.Received.Subscribe(HandleReceived);
            Received = new MessageQueue<byte[]>();
            sender.Error += HandleError;
            receiver.Error += HandleError;

        }           
		protected IByteSender sender;
		protected IByteReceiver receiver ;

		public IEndPoint Remote {get {return receiver.Remote;}}

        protected virtual void HandleError(Exception e) { }
       
        void HandleReceived(byte[] bytes)
        {
            Received.Post(bytes);
        }

        public virtual void Dispose()
        {
            receiver.Received.Dispose();
            receiver.Error -= HandleError;
            sender.Error -= HandleError;       
        }

        public MessageQueue<byte[]> Received { get; private set; }
        public event Action<IByteConnection> Disconnected;
        protected void FireDisconnected()
        {
            Disconnected.FireEventAsync(this);
        }

        public void Send(byte[] d)
        {
            sender.Send(d);
        }
        
    }
}
