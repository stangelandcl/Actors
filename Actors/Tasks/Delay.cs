﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Actors.Tasks
{   
    public static class TaskEx
    {
        static readonly Task _sPreCompletedTask = GetCompletedTask();
        static readonly Task _sPreCanceledTask = GetPreCanceledTask();

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
    }
}
