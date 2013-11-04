using System;
using System.Diagnostics;

namespace Cls.Extensions
{
	/// <summary>
	/// A timespan and a stopwatch that tracks whether or not a timeout has happened using real time.
	/// </summary>
	public struct TimeoutTimer
	{
		public TimeoutTimer (TimeSpan timeout)
		{
			this.timeout = timeout;
			this.sw = null;
		}

		Stopwatch sw;
		TimeSpan timeout;

		/// <summary>
		/// Timer will start on first check of IsTimedOut by default. Otherwise this can be called
		/// if you don't want to require at least one check of the loop to happen
		/// </summary>
		public void Start(){
			sw = Stopwatch.StartNew();
		}

		/// <summary>
		/// Start timer on first check. This makes for a nice pattern of checking the timeout at the top of a loop but
		/// still be sure to execute the loop at least once
		/// </summary>
		/// <value><c>true</c> if this instance is timed out; otherwise, <c>false</c>.</value>
		public bool IsTimedOut {get { 
				if(sw == null){
					sw = Stopwatch.StartNew();
					return false;
				}
				return sw.Elapsed >= timeout;
			}
		}
	}
}

