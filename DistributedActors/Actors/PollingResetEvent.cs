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

		bool signalled;

		public bool Reset(){
			return signalled = false;
		}
        public bool Set()
        {
            return signalled = true;
        }

		public bool WaitOne(){
            while (!signalled)
                Thread.Yield();
			signalled = false;
			return true;
		}

        public bool WaitOne(TimeSpan s)
        {
            return WaitOne((int)s.TotalMilliseconds);
        }

		public bool WaitOne(int timeoutInMs){
            var start = DateTime.Now;
            while (!signalled && (DateTime.Now - start).TotalMilliseconds < timeoutInMs)
                Thread.Yield();			
			signalled = false;
			return true;
		}
	}
}

