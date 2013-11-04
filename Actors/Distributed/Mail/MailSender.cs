using System;
using System.Diagnostics;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;

namespace Cls.Actors
{
	public class MailSender : IMailSender
	{
		public MailSender (IConnection s)
		{
			this.sender = s;
		}
		IConnection sender;

		public void Send (IMail mail)
		{
            try
            {
                sender.Send(mail);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("send " + ex);
            }
		}

		public void Send (ActorId to, ActorId fromId, MessageId msg, string name, params object[] args)
		{
			Send(new RpcMail{
				From = fromId,
				To = to,
				MessageId = msg,
				Message = new FunctionCall(name, args),
			});
		}

		public void Send(ActorId to, string name, params object[] args){
			Send(new RpcMail{
				To = to,
				From = ActorId.Empty,
				MessageId = new MessageId(Guid.NewGuid()),
				Message = new FunctionCall(name, args),
			});
		}

		public void Reply (IMail m, string name, params object[] args)
		{
			var mail = m.As<RpcMail>();
			Send(new RpcMail{
				To = mail.From,
				From = mail.To,
				MessageId = mail.MessageId,
				Message = new FunctionCall(name, args),
			});
		}
	}
}

