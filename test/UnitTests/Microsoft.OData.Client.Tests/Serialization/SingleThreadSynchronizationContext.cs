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

        // Best-effort, non-blocking shutdown. Signals the worker to stop after its current callback;
        // we deliberately do NOT Join the worker or Dispose the queue. If a posted continuation is
        // deadlocked (the failure mode these tests guard against), Join would hang and Dispose would
        // race the still-parked consumer. The worker is IsBackground=true and exits with the process.
        public void Dispose()
        {
            if (!_queue.IsAddingCompleted)
            {
                _queue.CompleteAdding();
            }
        }

        // Runs asyncAction on a worker thread that has this context installed. Returns the task that
        // settles when asyncAction's task settles, plus a Shutdown handle the caller can dispose to
        // tear down the pump on timeout. On natural completion the context is disposed automatically;
        // Dispose is idempotent, so a redundant Shutdown.Dispose() from a `using` is harmless.
        public static (Task Task, IDisposable Shutdown) Run(Func<Task> asyncAction)
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

            _ = tcs.Task.ContinueWith(_ => ctx.Dispose(), TaskScheduler.Default);
            return (tcs.Task, ctx);
        }

        public static (Task<T> Task, IDisposable Shutdown) Run<T>(Func<Task<T>> asyncAction)
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

            _ = tcs.Task.ContinueWith(_ => ctx.Dispose(), TaskScheduler.Default);
            return (tcs.Task, ctx);
        }
    }
}
