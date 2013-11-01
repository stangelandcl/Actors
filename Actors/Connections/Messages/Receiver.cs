using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Actors
{
    class Receiver : IReceiver
    {
        public Receiver(IByteReceiver receiver, ISerializer serializer)
        {
            this.receiver = receiver;
            this.serializer = serializer;
            Received = new MessageQueue<object>();
            this.receiver.Received.Subscribe(HandleReceived);
            this.receiver.Error += HandleError;
        }

        IByteReceiver receiver;
        ISerializer serializer;
        public MessageQueue<object> Received { get; private set; }
        public event Action<Exception> Error;

        void HandleError(Exception e)
        {
            Error.FireEventAsync(e);
        }

        void HandleReceived(byte[] b)
        {
            var obj = serializer.Deserialize<object>(b);
            Received.Post(obj);
        }      

        public void Dispose()
        {
            Received.Dispose();
            receiver.Received.Dispose();
            receiver.Error -= Error;
        }

        public IEndPoint Remote
        {
            get { return receiver.Remote; }
        }
    }
}
