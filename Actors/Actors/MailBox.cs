using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Actors
{
	public class MailBox : IMailReceiver
	{
		public MailBox(ActorId id)
			: this(id, 256, TimeSpan.FromSeconds(10))
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
		AutoResetEvent hasMail = new AutoResetEvent(false);
#endif

		public ActorId Id {get; private set;}
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

		public void Receive(Mail m){
			lock(mail){
				mail.AddLast(m);
				if(mail.Count > limit)
					mail.RemoveFirst();
				hasMail.Reset();
				if(Received != null)
					ThreadPool.QueueUserWorkItem(q=>Received());
			}
		}

		public Mail WaitForAny(TimeSpan? timeout = null){
			timeout = timeout ?? defaultTimeout;
			do{
				lock(mail){
					if(mail.Any())
					{
						var m = mail.First.Value;
						mail.RemoveFirst();
						return m;
					}
				}

				if(WaitOne (ref timeout)) continue;
			}while(timeout.Value.TotalSeconds > 0);
			return null;
		}

		public Mail WaitFor(MessageId id, TimeSpan? timeout = null){
			timeout = timeout ?? defaultTimeout;
			do{
				lock(mail){
					for(var i=mail.First;i != null;i = i.Next){
						var m = i.Value;
						if(m.MessageId == id){					
							mail.Remove(i);
							return m;
						}
					}
				}
				if(WaitOne(ref timeout)) continue;
			}while(timeout.Value.TotalSeconds > 0);
			return null;
		}	

		static void Sleep (ref TimeSpan? timeout)
		{
			Thread.Sleep (checkIntervalMs);
			timeout -= TimeSpan.FromMilliseconds (checkIntervalMs);
		}

		bool WaitOne (ref TimeSpan? timeout)
		{
			bool hasOne = hasMail.WaitOne (checkIntervalMs);			
			if(hasOne)
				timeout -= TimeSpan.FromMilliseconds (checkIntervalMs);
			return hasOne;
		}
	}
}

