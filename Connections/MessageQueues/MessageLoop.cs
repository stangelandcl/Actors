using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cls.Extensions;


namespace Cls.Connections
{
    public abstract class MessageLoop<T>
    {
        public MessageLoop(int maxMessages = 1000)
        {
            this.maxMessages = maxMessages;
        }

        protected int maxMessages;
        protected Queue<T> messages = new Queue<T>();        
        bool isRunning = false;      

        public void Post(T message)
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
		
        protected abstract void HandleMessage(T message);
        protected virtual void Next()
        {
            TaskEx.Run(Execute);
        }
        
        protected void Execute()
        {           
            T message;
            lock (messages)
                message = messages.Dequeue();
            try
            {
                HandleMessage(message);                       
            }
            catch(Exception ex) 
			{ 
				Console.WriteLine("Execute error " + ex);
			}
            lock (messages)
                if (messages.Any())
                    Next();
                else
                    isRunning = false;            
        }               
    }
}
