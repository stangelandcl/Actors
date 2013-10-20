using System;

namespace Actors
{
	public class DisposableAction : IDisposable
	{
		public DisposableAction (Action<object> callback, object o = null)
		{
			this.callback = callback;
			this.Tag = o;
		}
		public object Tag {get;set;}
		Action<object> callback;
		public void Dispose(){
			callback(Tag);
		}
	}
}

