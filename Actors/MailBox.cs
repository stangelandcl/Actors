using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace NetworkTransport
{
	public class MailBox
	{
		LinkedList<Mail> mail = new LinkedList<Mail>();
		AutoResetEvent hasMail = new AutoResetEvent();

		public void Received(Message m){
			lock(mail){
				mail.AddLast(m);
				hasMail.Reset();
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

