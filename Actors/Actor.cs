using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public abstract class Actor<T>
    {
        public Actor(int maxMessages = 100)
        {
            this.maxMessages = maxMessages;
        }
        int maxMessages;
        Queue<T> messages = new Queue<T>();
        bool isRunning = false;
        public void Receive(T message)
        {
            lock (messages)
            {
                if (messages.Count >= maxMessages)
                    return;
                messages.Enqueue(message);
                if (!isRunning)
                {
                    isRunning = true;
                    TaskEx.Run(Execute);
                }
            }
        }

        protected abstract void HandleReceived(T message);
        void Execute()
        {
            T message;
            lock (messages)
                message = messages.Dequeue();
            try
            {
                HandleReceived(message);
            }
            catch { }
            lock (messages)
            if (messages.Any())
                TaskEx.Run(Execute);
            else
                isRunning = false;           
        }
        
    }
}
