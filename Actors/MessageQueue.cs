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
                           
        List<Action<T>> subscriptions = new List<Action<T>>();
       
        public void Subscribe(Action<T> subscription)
        {
            lock(subscriptions)
            subscriptions.Add(subscription);
        }

        public void Unsubscribe(Action<T> subscription)
        {
            lock(subscriptions)
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
