using System;

namespace Actors
{
    public class Disposable : IDisposable
    {
        public Disposable(Action callback)
        {
            this.callback = callback;           
        }     
        Action callback;
        public void Dispose()
        {
            callback();
        }
        public static Disposable New(Action callback)
        {
            return new Disposable(callback);
        }
        public static Disposable<T> New<T>(Action<T> callback, T o)
        {
            return new Disposable<T>(callback, o);
        }
    }
	public class Disposable<T> : IDisposable
	{
		public Disposable(Action<T> callback, T o)
		{
			this.callback = callback;
			this.Tag = o;
		}
		public T Tag {get;set;}
		Action<T> callback;
		public void Dispose(){
			callback(Tag);
		}
	}
}

