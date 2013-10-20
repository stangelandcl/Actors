using System;

namespace Actors
{
	public class MailRouter : IMailRouter
	{
		public MailRouter(IRouter router){
			this.router = router;
		}

		IRouter router;
		public event Action<Mail> Received;

		public IMailSender GetSender (ActorId id)
		{
			return new MailSender(router.GetSender(id));
		}

		public void Add (MessageTConnection c)
		{
			router.Add(c);
		}
	}
}

