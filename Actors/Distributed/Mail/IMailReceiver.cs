using System;

namespace Cls.Actors
{
	public interface IMailReceiver
	{
		IMail CheckFor (Func<IMail, bool> filter, TimeSpan? timeout = null);
		IMail WaitForAny(TimeSpan? timeout = null);			
		IMail WaitFor(MessageId id, TimeSpan? timeout = null);
	}
}

