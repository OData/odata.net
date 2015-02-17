//---------------------------------------------------------------------
// <copyright file="AsyncExecutionContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Asynchronous execution context - can be used to enqueue asynchronous 
    /// actions to be executed at the end of currently executed synchronous operation 
    /// (such as variation).
    /// </summary>
    public sealed class AsyncExecutionContext : IDisposable
    {
        private static object lockObject = new object();
        private static AsyncExecutionContext current;

        private List<Action<IAsyncContinuation>> queuedActions = new List<Action<IAsyncContinuation>>();
        private Stack<IDisposable> usingStack = new Stack<IDisposable>();

        /// <summary>
        /// Prevents a default instance of the AsyncExecutionContext class from being created.
        /// </summary>
        private AsyncExecutionContext()
        {
        }

        /// <summary>
        /// Begins new asynchronous execution context, returns it and assigns it to Current
        /// </summary>
        /// <returns>Instance of <see cref="AsyncExecutionContext"/> which can be disposed later when the execution ends.</returns>
        /// <remarks>This method is to be used internally by test runners to set up execution context for variations</remarks>
        public static AsyncExecutionContext Begin()
        {
            lock (lockObject)
            {
                if (current != null)
                {
                    throw new TaupoInvalidOperationException("Cannot nest asynchronous execution contexts.");
                }

                current = new AsyncExecutionContext();
                return current;
            }
        }

        /// <summary>
        /// Enqueues the specified asynchronous action to be executed before the end of current
        /// execution context (such as variation)
        /// </summary>
        /// <param name="action">The asynchronous action.</param>
        public static void EnqueueAsynchronousAction(Action<IAsyncContinuation> action)
        {
            lock (lockObject)
            {
                ExceptionUtilities.CheckObjectNotNull(current, "Cannot run asynchronous action outside of asynchronous execution context.");
                current.AddActionWithDispose(action);
            }
        }

        /// <summary>
        /// Enqueues the synchronous action to be executed at the end of current <see cref="AsyncExecutionContext"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <remarks>This method throws if there is no asynchronous execution context.</remarks>
        public static void EnqueueSynchronousAction(Action action)
        {
            EnqueueAsynchronousAction(c => AsyncHelpers.TryCatch(c, action));
        }

        /// <summary>
        /// Wraps a disposable object in order to delay enqueuing its disposal until the end of a using block
        /// </summary>
        /// <param name="toDispose">The disposable object</param>
        /// <returns>A wrapping object that enqueues disposal of the given object when it is disposed</returns>
        public static IDisposable Using(IDisposable toDispose)
        {
            StartUsingBlock(toDispose);
            return new DelegateBasedDisposable(() => EndUsingBlock());
        }
        
        /// <summary>
        /// Ends the execution context.
        /// </summary>
        public void Dispose()
        {
            this.End();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Ends this execution context.
        /// </summary>
        public void End()
        {
            lock (lockObject)
            {
                if (current != this)
                {
                    throw new TaupoInvalidOperationException("Cannot end context other than current.");
                }

                current = null;
            }
        }

        /// <summary>
        /// Gets the actions that have been queued during context lifetime.
        /// </summary>
        /// <returns>Collection of actions.</returns>
        public ReadOnlyCollection<Action<IAsyncContinuation>> GetQueuedActions()
        {
            ExceptionUtilities.Assert(this.usingStack.Count == 0, "Using stack should be empty when done executing queued actions");
            return this.queuedActions.AsReadOnly();
        }

        private static void StartUsingBlock(IDisposable disposable)
        {
            ExceptionUtilities.CheckArgumentNotNull(disposable, "disposable");

            lock (lockObject)
            {
                ExceptionUtilities.CheckObjectNotNull(current, "Cannot start using block outside of asynchronous execution context.");
                current.usingStack.Push(disposable);
            }
        }
        
        private static void EndUsingBlock()
        {
            lock (lockObject)
            {
                ExceptionUtilities.CheckObjectNotNull(current, "Cannot end using block outside of asynchronous execution context.");
                ExceptionUtilities.Assert(current.usingStack.Count > 0, "Cannot end using, nothing on stack");

                var toDispose = current.usingStack.Pop();
                current.AddActionWithDispose(c => AsyncHelpers.TryCatch(c, toDispose.Dispose));
            }
        }

        private void AddActionWithDispose(Action<IAsyncContinuation> action)
        {
            if (current.usingStack.Count > 0)
            {
                var toDispose = current.usingStack.ToArray();

                Action onException = () =>
                {
                    foreach (var d in toDispose)
                    {
                        d.Dispose();
                    }
                };

                var originalAction = action;
                action = c =>
                {
                    var wrappedContinuation = AsyncHelpers.CreateContinuation(
                        () => c.Continue(),
                        e =>
                        {
                            AsyncHelpers.CatchErrors(c, onException);
                            c.Fail(e);
                        });

                    originalAction(wrappedContinuation);
                };
            }

            current.queuedActions.Add(action);
        }
    }
}