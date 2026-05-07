//---------------------------------------------------------------------
// <copyright file="SingleThreadSynchronizationContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.Tests.Serialization
{
    // Pumps Post-ed callbacks on a single dedicated worker thread. Mirrors the load-bearing invariant of
    // Blazor Server's renderer scheduler (and WPF/WinForms Dispatcher): one thread serially runs queued
    // continuations, so any sync-over-async wait on that thread blocks the very thread the continuation
    // needs to resume on. Used to reproduce issue #3521.
    internal sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
    {
        private readonly BlockingCollection<(SendOrPostCallback Callback, object State)> _queue =
            new BlockingCollection<(SendOrPostCallback, object)>();

        private readonly Thread _worker;

        public SingleThreadSynchronizationContext()
        {
            _worker = new Thread(Pump) { IsBackground = true, Name = "SingleThreadSyncCtx" };
            _worker.Start();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            _queue.Add((d, state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            // Blazor's renderer scheduler likewise refuses synchronous Send from foreign threads;
            // the deadlock under test comes from Post-ed continuations, not Send.
            throw new NotSupportedException("Send is not supported on " + nameof(SingleThreadSynchronizationContext));
        }

        private void Pump()
        {
            SetSynchronizationContext(this);
            foreach (var item in _queue.GetConsumingEnumerable())
            {
                item.Callback(item.State);
            }
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
            if (_worker.IsAlive)
            {
                _worker.Join();
            }
            _queue.Dispose();
        }

        // Runs asyncAction on a worker thread that has this context installed. The returned task completes
        // when asyncAction's task completes; the context is disposed afterwards. Callers enforce timeouts
        // via Task.WhenAny so a deadlocked test releases the runner instead of hanging it.
        public static Task Run(Func<Task> asyncAction)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));

            var ctx = new SingleThreadSynchronizationContext();
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            ctx.Post(async _ =>
            {
                try
                {
                    await asyncAction().ConfigureAwait(true);
                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null);

            // Dispose the context once the action's task settles, regardless of outcome.
            return tcs.Task.ContinueWith(t =>
            {
                ctx.Dispose();
                return t;
            }, TaskScheduler.Default).Unwrap();
        }

        public static Task<T> Run<T>(Func<Task<T>> asyncAction)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));

            var ctx = new SingleThreadSynchronizationContext();
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            ctx.Post(async _ =>
            {
                try
                {
                    T result = await asyncAction().ConfigureAwait(true);
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null);

            return tcs.Task.ContinueWith(t =>
            {
                ctx.Dispose();
                return t;
            }, TaskScheduler.Default).Unwrap();
        }
    }
}
