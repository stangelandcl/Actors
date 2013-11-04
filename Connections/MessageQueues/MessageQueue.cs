using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Connections
{
    public class MessageQueue<T> : MessageLoop<T>, IDisposable
    {
        public MessageQueue(int maxMessages = 1000)
            : base(maxMessages)
        { }
                           
        SynchronizedCollection<Action<T>> subscriptions = new SynchronizedCollection<Action<T>>();
       
        public void Subscribe(Action<T> subscription)
        {            
            subscriptions.Add(subscription);
        }

        public void Unsubscribe(Action<T> subscription)
        {            
            subscriptions.Remove(subscription);
        }

        public void Dispose()
        {
            subscriptions = null;                       
        }

        protected override void HandleMessage(T message)
        {            
            foreach (var subscription in subscriptions)
                subscription(message);   
        }

		public override string ToString ()
		{
			lock(messages)
				return string.Format ("[MessageQueue: Count={0}, Subscriptions={1}]", this.messages.Count, this.subscriptions.Count);
		}
    }
}
