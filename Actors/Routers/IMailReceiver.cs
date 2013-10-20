using System;

namespace Actors
{
	public interface IMailReceiver
	{
		Mail CheckFor (Func<Mail, bool> filter, TimeSpan? timeout = null);
		Mail WaitForAny(TimeSpan? timeout = null);			
		Mail WaitFor(MessageId id, TimeSpan? timeout = null);
	}
}

