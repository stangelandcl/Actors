using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Actors
{
	public class MailBox
	{
		public MailBox(int id){
			this.Id = id;
		}

		LinkedList<Mail> mail = new LinkedList<Mail>();
#if MONO
		PollingResetEvent hasMail = new PollingResetEvent(false);
#else
		AutoResetEvent hasMail = new AutoResetEvent(false);
#endif

		public int Id {get; private set;}
		public event Action Received;

		public Mail CheckFor (Func<Mail, bool> filter, int timeout)
		{
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
				Thread.Sleep(10);
				timeout -= 10;
			}while(timeout > 0);
			return null;
		}

		public void Receive(Mail m){
			lock(mail){
				mail.AddLast(m);
				hasMail.Reset();
				if(Received != null)
					ThreadPool.QueueUserWorkItem(q=>Received());
			}
		}

		public Mail WaitForAny(){
			while(true){
				lock(mail){
					if(mail.Any())
					{
						var m = mail.First.Value;
						mail.RemoveFirst();
						return m;
					}
				}

				hasMail.WaitOne();
			}
		}

		public Mail WaitFor(int id){
			while(true){
				lock(mail){
					for(var i=mail.First;i != null;i = i.Next){
						var m = i.Value;
						if(m.MessageId == id){					
							mail.Remove(i);
							return m;
						}
					}
				}
				hasMail.WaitOne();
			}
		}	
	}
}

