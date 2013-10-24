using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Connections.Bytes;
using Serialization;

namespace Actors.Connections.Messages
{
    class Sender : ISender
    {
        public Sender(IByteSender sender, ISerializer serializer)
        {
            this.sender = sender;
            this.serializer = serializer;
            this.sender.Disconnected += HandleDisconnected;
        }
        public IEndPoint Remote { get { return sender.Remote; } }

        IByteSender sender;
        ISerializer serializer;

        void HandleDisconnected(IByteSender s)
        {
            Disconnected.FireEventAsync(this);
        }

        public void Send(object o)
        {
            sender.Send(serializer.Serialize(o));
        }


        public event Action<ISender> Disconnected;

        public void Dispose()
        {
            sender.Disconnected -= HandleDisconnected;
        }
    }
}
