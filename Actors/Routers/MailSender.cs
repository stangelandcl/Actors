using System;

namespace Actors
{
	public class MailSender : IMailSender
	{
		public MailSender (ISender s)
		{
			this.sender = s;
		}
		ISender sender;

		public void Send (Mail mail)
		{
			sender.Send(mail);
		}

		public void Send (ActorId to, ActorId fromId, MessageId msg, FunctionId name, params object[] args)
		{
			Send(new Mail{
				From = fromId,
				To = to,
				MessageId = msg,
				Name = name,
				Args = args,
			});
		}

		public void Send(ActorId to, FunctionId name, params object[] args){
			Send(new Mail{
				To = to,
				From = ActorId.Empty,
				MessageId = Guid.NewGuid(),
				Name = name,
				Args = args,
			});
		}

		public void Reply (Mail mail, FunctionId name, params object[] args)
		{
			Send(new Mail{
				To = mail.From,
				From = mail.To,
				MessageId = mail.MessageId,
				Name = name,
				Args = args,
			});
		}
	}
}

