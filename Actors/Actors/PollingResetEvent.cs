using System;
using System.Threading;

namespace Actors
{
	public class PollingResetEvent
	{
		public PollingResetEvent (bool signalled)
		{
			this.signalled = signalled;
		}

#if DEBUG
		Guid instanceId = Guid.NewGuid();
#endif

		bool signalled;

		public bool Reset(){
			return signalled = true;
		}

		public bool WaitOne(){
			while(!signalled)
				Thread.Sleep(100);
			signalled = false;
			return true;
		}

		public bool WaitOne(int timeoutInMs){
			while(!signalled && timeoutInMs > 0){
				Thread.Sleep(10);
				timeoutInMs -= 10;
			}
			signalled = false;
			return true;
		}
	}
}

