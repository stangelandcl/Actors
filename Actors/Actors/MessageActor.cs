using System;

namespace Actors
{
//	public class MessageActor : Actor, IMailSender, IMailReceiver
//	{
//		public MessageActor (MailBox box, IWorld world)
//			: this(box, world)
//		{ }
//
//		#region IMailReceiver implementation
//		public Mail CheckFor (Func<Mail, bool> filter, TimeSpan? timeout = default(TimeSpan?))
//		{
//			return MailBox.CheckFor(filter, timeout);
//		}
//		public Mail WaitForAny (TimeSpan? timeout = default(TimeSpan?))
//		{
//			return MailBox.WaitForAny(timeout);
//		}
//		public Mail WaitFor (MessageId id, TimeSpan? timeout = default(TimeSpan?))
//		{
//			return MailBox.WaitFor(id, timeout);
//		}
//		#endregion
//
//		#region IMailSender implementation
//		public void Send (Mail mail)
//		{
//			Node.GetSender(mail.To).Send(mail);
//		}
//		public void Send (ActorId to, ActorId fromId, MessageId msg, FunctionId name, params object[] args)
//		{
//			Node.GetSender(to).Send(to, fromId, msg, name, args);
//		}
//		public void Send (ActorId to, FunctionId name, params object[] args)
//		{
//			Node.GetSender(to, name, args);
//		}
//		public void Reply (Mail mail, FunctionId name, params object[] args)
//		{
//			Node.GetSender(mail, name, args);
//		}
//		#endregion
//	}
}

