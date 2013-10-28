using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
namespace System
{   
    static partial class TaskEx
    {
        static readonly Task _sPreCompletedTask = GetCompletedTask();
        static readonly Task _sPreCanceledTask = GetPreCanceledTask();          

        /// <summary>
        /// Run task with try-catch
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Task Run(Action a)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    a();
                }
                catch { }
            });
        }

        public static Task Delay(int dueTimeMs, Action<Task> action)
        {
            return Delay(dueTimeMs).ContinueWith(action);
        }

        public static Task Delay(int dueTimeMs, CancellationToken? cancel = null)
        {
            var cancellationToken = cancel ?? CancellationToken.None;
            if (dueTimeMs < -1)
                throw new ArgumentOutOfRangeException("dueTimeMs", "Invalid due time");
            if (cancellationToken.IsCancellationRequested)
                return _sPreCanceledTask;
            if (dueTimeMs == 0)
                return _sPreCompletedTask;

            var tcs = new TaskCompletionSource<object>();
            var ctr = new CancellationTokenRegistration();
            var timer = new Timer(delegate(object self)
            {
                ctr.Dispose();
                ((Timer)self).Dispose();
                tcs.TrySetResult(null);
            });
            if (cancellationToken.CanBeCanceled)
                ctr = cancellationToken.Register(delegate
                {
                    timer.Dispose();
                    tcs.TrySetCanceled();
                });

            timer.Change(dueTimeMs, -1);
            return tcs.Task;
        }

        private static Task GetPreCanceledTask()
        {
            var source = new TaskCompletionSource<object>();
            source.TrySetCanceled();
            return source.Task;
        }

        private static Task GetCompletedTask()
        {
            var source = new TaskCompletionSource<object>();
            source.TrySetResult(null);
            return source.Task;
        }

        public static void FireEvent(this Action e)
        {
            if (e != null) e();
        }
        public static void FireEventAsync(this Action e)
        {
            if (e != null) TaskEx.Run(e);
        }
        public static void FireEvent<T>(this Action<T> e, T args)
        {
            if (e != null) e(args);
        }
        public static void FireEventAsync<T>(this Action<T> e, T args)
        {
            if (e != null) TaskEx.Run(() => e(args));
        }
        public static void FireEvent<T, T2>(this Action<T, T2> e, T args, T2 args2)
        {
            if (e != null) e(args, args2);
        }
        public static void FireEventAsync<T, T2>(this Action<T, T2> e, T args, T2 args2)
        {
            if (e != null) TaskEx.Run(() => e(args, args2));
        }
        public static void FireEvent<T>(this EventHandler<T> e, object sender, T args) where T : EventArgs
        {
            if (e != null) e(sender, args);
        }
        public static void FireEventAsync<T>(this EventHandler<T> e, object sender, T args) where T : EventArgs
        {
            if (e != null) TaskEx.Run(() => e(sender, args));
        }
       
    }
}
