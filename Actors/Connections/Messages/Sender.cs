using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;
using Serialization;
using Actors.Extensions;

namespace Actors.Connections.Messages
{
    class Sender : ISender
    {
        public Sender(IByteSender sender, ISerializer serializer)
        {
            this.sender = sender;
            this.serializer = serializer;
            this.sender.Error += HandleError;
        }
        public IEndPoint Remote { get { return sender.Remote; } }
        public event Action<Exception> Error;
        IByteSender sender;
        ISerializer serializer;

        void HandleError(Exception e)
        {
            Error.FireEventAsync(e);
        }

        public void Send(object o)
        {
            sender.Send(serializer.SerializeToBytes(o));
        }
        public void Dispose()
        {
            sender.Error -= HandleError;
        }
    }
}
