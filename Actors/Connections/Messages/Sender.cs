using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Actors
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

		public override string ToString ()
		{
			return string.Format ("[Sender: Remote={0}]", Remote);
		}
    }
}
