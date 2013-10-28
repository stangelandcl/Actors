using System;
using System.ComponentModel;

namespace Actors
{
	public class DefaultActor : DistributedActor
	{
        public DefaultActor(MailBox box, Node env)
            : base(box, env)
        { }
        public T SendReceive<T>(ActorId to, FunctionId name, params object[] args)
        {
            var msg = Send(to, name, args);
            return Receive<T>(msg);
        }

        public T Receive<T>()
        {
            var mail = Box.WaitForAny();
            if (mail == null) return default(T);
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertTo(mail.Args[0], typeof(T));
        }

        T Receive<T>(MessageId msg)
        {
            var mail = Box.WaitFor(msg);
            if (mail == null) return default(T);
            return (T)ConvertEx.Convert(mail.Args[0], typeof(T));
        }

        public MessageId Send(ActorId to, FunctionId name, params object[] args)
        {
            MessageId msg;
            Node.Send(new Mail { To = to, From = Box.Id, MessageId = msg = MessageId.New(), Name = name, Args = args });
            return msg;
        }
	}
}

