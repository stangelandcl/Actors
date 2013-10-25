using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Actors
{
	public class MailBox : IMailReceiver
	{
		public MailBox(ActorId id)
			: this(id, 256, TimeSpan.FromSeconds(5))
		{}
		public MailBox(ActorId id, int limit, TimeSpan defaultTimeout){
			this.Id = id;
			this.limit = limit;
			this.defaultTimeout = defaultTimeout;
		}

		LinkedList<Mail> mail = new LinkedList<Mail>();
		int limit;
		TimeSpan defaultTimeout;
		const int checkIntervalMs = 10;
#if MONO
		PollingResetEvent hasMail = new PollingResetEvent(false);
#else
		ManualResetEvent hasMail = new ManualResetEvent(false);
#endif

		public ActorId Id {get; internal set;}
		public event Action Received;

		public Mail CheckFor (Func<Mail, bool> filter, TimeSpan? timeout = null)
		{
			timeout = timeout ?? defaultTimeout;
			do{
				lock(mail){
					for(var i=mail.First;i != null;i = i.Next){
						var m = i.Value;
						if(filter(m))						
						{					
							mail.Remove(i);
							return m;
						}
					}
				}
				Sleep (ref timeout);
			}while(timeout.Value.TotalSeconds > 0);
			return null;
		}

        public void Execute(Func<Mail, bool> exec)
        {                    
            lock (mail)
            {
                for (var i = mail.First; i != null; i = i.Next)
                {
                    var m = i.Value;
                    if (exec(m))
                        mail.Remove(i);                  
                }
            }               
        }

		public void Receive(Mail m){
			lock(mail){
				mail.AddLast(m);
				if(mail.Count > limit)
					mail.RemoveFirst();
				hasMail.Set();
                if (Received != null)                    
                    Task.Factory.StartNew(Received);
			}
		}

		public Mail WaitForAny(TimeSpan? timeout = null){
			timeout = timeout ?? defaultTimeout;
			while(true){
				lock(mail){
					if(mail.Any())
					{
						var m = mail.First.Value;
						mail.RemoveFirst();
						return m;
					}
				}

                if (hasMail.WaitOne(timeout.Value))
                {
                    lock (hasMail) hasMail.Reset();
                    continue;
                }
                return null;
			}			
		}

		public Mail WaitFor(MessageId id, TimeSpan? timeout = null){
            var start = Stopwatch.GetTimestamp();
            timeout = timeout ?? defaultTimeout;
            var ticks = timeout.Value.TotalSeconds * Stopwatch.Frequency;
            while (true) 
            {
                lock (mail)
                {
                    for (var i = mail.First; i != null; i = i.Next)
                    {
                        var m = i.Value;
                        if (m.MessageId == id)
                        {
                            mail.Remove(i);
                            return m;
                        }
                    }
                }

                var now = Stopwatch.GetTimestamp();
                var wait = (ticks - (now - start)) / Stopwatch.Frequency * 1000;
                if (hasMail.WaitOne((int)wait))
                {
                    lock (hasMail) hasMail.Reset();
                    continue;
                }
                return null;
            }
            
		}	

		void Sleep (ref TimeSpan? timeout)
		{
            hasMail.WaitOne(checkIntervalMs); // wait but don't reset
			timeout -= TimeSpan.FromMilliseconds (checkIntervalMs);
		}		
	}
}

