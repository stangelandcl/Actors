using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public class MessageQueue<T> : MessageLoop<T>, IDisposable
    {
        public MessageQueue(int maxMessages = 100)
            : base(100)
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
        
    }
}
