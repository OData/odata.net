//---------------------------------------------------------------------
// <copyright file="AsyncHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Helper functions for simplifying executing asynchronous patterns.
    /// </summary>
    public static class AsyncHelpers
    {
        /// <summary>
        /// Asynchronous executes the given callback method for all items in the collection.
        /// </summary>
        /// <typeparam name="T">Collection element type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="continuation">The continuation to be invoked at the end of the iteration of whenever
        /// the user callback returns an error.</param>
        /// <param name="userCallback">The user callback.</param>
        /// <remarks>The callback method should execute one of the methods on the passed continuation method to indicate
        /// that the execution should proceed to the next item.</remarks>
        public static void AsyncForEach<T>(this IEnumerable<T> collection, IAsyncContinuation continuation, Action<T, IAsyncContinuation> userCallback)
        {
            var continuator = new Continuator<T>(collection.GetEnumerator(), userCallback, continuation, false, Logger.Null);
            continuator.ExecuteNextItem();
        }

        /// <summary>
        /// Asynchronous executes the given callback method for all items in the collection and reports failures at the end.
        /// </summary>
        /// <typeparam name="T">Collection element type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="continuation">The continuation to be invoked at the end of the iteration.</param>
        /// <param name="userCallback">The user callback.</param>
        /// <param name="logger">The logger to use for reporting.</param>
        public static void AsyncForEachWithDelayedFailures<T>(this IEnumerable<T> collection, IAsyncContinuation continuation, Action<T, IAsyncContinuation> userCallback, Logger logger)
        {
            var continuator = new Continuator<T>(collection.GetEnumerator(), userCallback, continuation, true, logger);
            continuator.ExecuteNextItem();
        }

        /// <summary>
        /// Invokes the specified synchronous action and notifies <see cref="IAsyncContinuation"/>
        /// in the same way asynchronous action would, except it does not report success.
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to use.</param>
        /// <param name="action">The action to invoke.</param>
        public static void CatchErrors(this IAsyncContinuation continuation, Action action)
        {
            CatchErrors(continuation, action, null);
        }

        /// <summary>
        /// Invokes the specified synchronous action and notifies <see cref="IAsyncContinuation"/>
        /// in the same way asynchronous action would, except it does not report success.
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to use.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="final">The action to invoke on success and failure</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exceptions here.")]
        public static void CatchErrors(this IAsyncContinuation continuation, Action action, Action final)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ExceptionUtilities.IsCatchable(ex))
                {
                    continuation.Fail(ex);
                }
            }
            finally
            {
                if (final != null)
                {
                    CatchErrors(continuation, final);
                }
            }
        }

        /// <summary>
        /// Runs the action sequence asynchronously.
        /// </summary>
        /// <param name="continuation">The continuation to be executed at the end of the action sequence.</param>
        /// <param name="sequence">The sequence of asynchronous actions.</param>
        public static void RunActionSequence(this IAsyncContinuation continuation, params Action<IAsyncContinuation>[] sequence)
        {
            AsyncForEach(
                sequence, 
                continuation, 
                (action, c) =>
                {
                    action(c);
                });
        }

        /// <summary>
        /// Runs the action sequence asynchronously.
        /// </summary>
        /// <param name="continuation">The continuation to be executed at the end of the action sequence.</param>
        /// <param name="sequence">The sequence of asynchronous actions.</param>
        public static void RunActionSequence(this IAsyncContinuation continuation, IEnumerable<Action<IAsyncContinuation>> sequence)
        {
            RunActionSequence(continuation, sequence.ToArray());
        }

        /// <summary>
        /// Runs the in specified action in asynchronous context and waits for all asynchronous actions to complete.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void RunInAsyncContext(Action action)
        {
            ReadOnlyCollection<Action<IAsyncContinuation>> actions;

            using (var context = AsyncExecutionContext.Begin())
            {
                action();
                actions = context.GetQueuedActions();
            }

            using (ManualResetEvent finishedEvent = new ManualResetEvent(false))
            {
                Exception lastException = null;

                var continuation = CreateContinuation(
                    () => finishedEvent.Set(),
                    ex =>
                    {
                        lastException = ex;
                        finishedEvent.Set();
                    });

                AsyncHelpers.RunActionSequence(continuation, actions);
                finishedEvent.WaitOne();

                if (lastException != null)
                {
                    throw new TaupoInvalidOperationException("Error during asynchronous execution, see inner message for details.", lastException);
                }
            }
        }

        /// <summary>
        /// Creates a continuation object given two delegates which will be executed on either completion or failure.
        /// </summary>
        /// <param name="onContinue">The action to be executed on success.</param>
        /// <param name="onFail">The action to be executed on failure.</param>
        /// <returns>Instance of a callback object.</returns>
        public static IAsyncContinuation CreateContinuation(Action onContinue, Action<Exception> onFail)
        {
            ExceptionUtilities.CheckArgumentNotNull(onContinue, "onContinue");
            ExceptionUtilities.CheckArgumentNotNull(onFail, "onFail");

            return new DelegateBasedContinuation(onContinue, onFail);
        }

        /// <summary>
        /// Wraps the given continuation with one that performs the given cleanup logic on both success and failure
        /// </summary>
        /// <param name="continuation">The continuation to wrap</param>
        /// <param name="cleanup">The cleanup logic to invoke in both cases</param>
        /// <returns>A wrapping continuation with the cleanup logic</returns>
        public static IAsyncContinuation OnContinueOrFail(this IAsyncContinuation continuation, Action cleanup)
        {
            return continuation.OnContinueOrFail(b => cleanup());
        }

        /// <summary>
        /// Wraps the given continuation with one that performs the given cleanup logic on both success and failure
        /// </summary>
        /// <param name="continuation">The continuation to wrap</param>
        /// <param name="cleanup">The cleanup logic to invoke in both cases, which takes a value indicating whether an exception was caught</param>
        /// <returns>A wrapping continuation with the cleanup logic</returns>
        public static IAsyncContinuation OnContinueOrFail(this IAsyncContinuation continuation, Action<bool> cleanup)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(cleanup, "cleanup");
            return CreateContinuation(
               () =>
               {
                   AsyncHelpers.CatchErrors(continuation, () => cleanup(false));
                   continuation.Continue();
               },
               e =>
               {
                   AsyncHelpers.CatchErrors(continuation, () => cleanup(true));
                   continuation.Fail(e);
               });
        }

        /// <summary>
        /// Wraps the given continuation with one that performs the given action on Continue
        /// </summary>
        /// <param name="continuation">The continuation to wrap</param>
        /// <param name="action">The action to invoke</param>
        /// <returns>A wrapping continuation</returns>
        public static IAsyncContinuation OnContinue(this IAsyncContinuation continuation, Action action)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(action, "action");
            return CreateContinuation(
               () =>
               {
                   AsyncHelpers.CatchErrors(continuation, action);
                   continuation.Continue();
               },
               e => continuation.Fail(e));
        }

        /// <summary>
        /// Wraps the given continuation with one that performs the given action on failure
        /// </summary>
        /// <param name="continuation">The continuation to wrap</param>
        /// <param name="action">The action to invoke</param>
        /// <returns>A wrapping continuation</returns>
        public static IAsyncContinuation OnFail(this IAsyncContinuation continuation, Action action)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(action, "action");

            return CreateContinuation(
               () => continuation.Continue(),
               e =>
               {
                   AsyncHelpers.CatchErrors(continuation, action);
                   continuation.Fail(e);
               });
        }

        /// <summary>
        /// Invokes the given async action and handles exceptions of a particular type using a callback
        /// </summary>
        /// <typeparam name="TException">The type of exceptions to handle</typeparam>
        /// <param name="continuation">The continuation to use</param>
        /// <param name="action">The async action to invoke</param>
        /// <param name="handleExceptionCallback">The callback for handling exceptions</param>
        public static void HandleException<TException>(this IAsyncContinuation continuation, Action<IAsyncContinuation> action, Action<TException> handleExceptionCallback) where TException : Exception
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(action, "action");
            ExceptionUtilities.CheckArgumentNotNull(handleExceptionCallback, "handleExceptionCallback");

            var exceptionContinuation = CreateContinuation(
                () => continuation.Continue(),
                e =>
                {
                    var exception = e as TException;
                    if (exception != null)
                    {
                        CatchErrors(continuation, () => handleExceptionCallback(exception));
                    }
                    else
                    {
                        continuation.Fail(e);
                    }
                });

            CatchErrors(exceptionContinuation, () => action(exceptionContinuation));
        }

        /// <summary>
        /// Invokes either the given sync or async method call based on the given parameter
        /// Note: Will not call .Continue on the continuation, ensure that this is done in the completion delegate
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method</typeparam>
        /// <param name="continuation">The async continuation</param>
        /// <param name="async">A value indicating whether to use the sync or async API</param>
        /// <param name="syncCall">The sync API call</param>
        /// <param name="beginAsyncCall">The 'begin' part of the async API</param>
        /// <param name="endAsyncCall">The 'end' part of the async API</param>
        /// <param name="onCompletion">Delegate to call with the method's return value</param>
        public static void InvokeSyncOrAsyncMethodCall<TReturn>(this IAsyncContinuation continuation, bool async, Func<TReturn> syncCall, Action<AsyncCallback> beginAsyncCall, Func<IAsyncResult, TReturn> endAsyncCall, Action<TReturn> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(syncCall, "syncCall");
            ExceptionUtilities.CheckArgumentNotNull(beginAsyncCall, "beginAsyncCall");
            ExceptionUtilities.CheckArgumentNotNull(endAsyncCall, "endAsyncCall");
            ExceptionUtilities.CheckArgumentNotNull(onCompletion, "onCompletion");

            if (async)
            {
                beginAsyncCall(
                    result =>
                    {
                        AsyncHelpers.CatchErrors(
                            continuation,
                            () =>
                            {
                                var returnValue = endAsyncCall(result);
                                onCompletion(returnValue);
                            });
                    });
            }
            else
            {
                AsyncHelpers.CatchErrors(
                    continuation,
                    () =>
                    {
                        var returnValue = syncCall();
                        onCompletion(returnValue);
                    });
            }
        }

        /// <summary>
        /// Helper method for emulating while-loops asynchronously using recursion
        /// </summary>
        /// <param name="continuation">The continuation to call when the condition is false</param>
        /// <param name="condition">The condition to check before starting the next iteration</param>
        /// <param name="action">The action to call for each iteration.</param>
        public static void AsyncWhile(this IAsyncContinuation continuation, Func<bool> condition, Action<IAsyncContinuation> action)
        {
            var wrappedContinuation = CreateContinuation(
                () => AsyncWhile(continuation, condition, action),
                e => continuation.Fail(e));

            if (condition())
            {
                CatchErrors(wrappedContinuation, () => action(wrappedContinuation));
            }
            else
            {
                continuation.Continue();
            }
        }

        /// <summary>
        /// Invokes the specified synchronous action and notifies <see cref="IAsyncContinuation"/>
        /// in the same way asynchronous action would
        /// NOTE: Method ONLY FOR SYNC, making it internal as making it public causes incorrect usage of this instead of using CatchErrors
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to use.</param>
        /// <param name="action">The action to invoke.</param>
        internal static void TryCatch(IAsyncContinuation continuation, Action action)
        {
            CatchErrors(continuation, () => { action(); continuation.Continue(); });
        }

        /// <summary>
        /// Implements <see cref="IAsyncContinuation"/> which advances to the next element in the collection.
        /// </summary>
        /// <typeparam name="T">Collection element type.</typeparam>
        private class Continuator<T> : IAsyncContinuation
        {
            private IEnumerator<T> enumerator;
            private Action<T, IAsyncContinuation> userCallback;
            private IAsyncContinuation continuation;
            private bool delayedFailures;
            private int failures = 0;
            private int skipped = 0;
            private int passed = 0;
            private Logger logger;

            /// <summary>
            /// Initializes a new instance of the Continuator class.
            /// </summary>
            /// <param name="enumerator">The enumerator.</param>
            /// <param name="userCallback">The user callback.</param>
            /// <param name="continuation">The continuation.</param>
            /// <param name="delayedFailures">If set to <c>true</c> the failures during enumeration will be delayed (and logged to the provider logger).</param>
            /// <param name="logger">The logger to log failures to.</param>
            public Continuator(IEnumerator<T> enumerator, Action<T, IAsyncContinuation> userCallback, IAsyncContinuation continuation, bool delayedFailures, Logger logger)
            {
                this.enumerator = enumerator;
                this.userCallback = userCallback;
                this.continuation = continuation;
                this.delayedFailures = delayedFailures;
                this.logger = logger;
            }

            /// <summary>
            /// Reports success of asynchronous method invocation.
            /// </summary>
            public void Continue()
            {
                this.passed++;
                this.ExecuteNextItem();
            }

            /// <summary>
            /// Reports the failure of asynchronous method invocation.
            /// </summary>
            /// <param name="exception">The exception.</param>
            public void Fail(Exception exception)
            {
                if (this.delayedFailures)
                {
                    if (exception is TestSkippedException)
                    {
                        this.logger.WriteLine(LogLevel.Info, "Skipped: {0}", exception.Message);
                        this.skipped++;
                    }
                    else
                    {
                        this.logger.WriteLine(LogLevel.Error, exception.ToString());
                        this.failures++;
                    }

                    this.ExecuteNextItem();
                }
                else
                {
                    this.continuation.Fail(exception);
                }
            }

            /// <summary>
            /// Executes the next item (if available) or finishes enumeration.
            /// </summary>
            public void ExecuteNextItem()
            {
                if (this.enumerator.MoveNext())
                {
                    CatchErrors(this, () => InvokeAsynchronousAction(this, c => this.userCallback(this.enumerator.Current, c)));
                }
                else
                {
                    if (this.failures > 0)
                    {
                        this.continuation.Fail(new TaupoInvalidOperationException("Got " + this.failures + " failures."));
                    }
                    else if (this.skipped > 0 && this.passed == 0)
                    {
                        this.continuation.Skip("Skipped all " + this.skipped + " items.");
                    }
                    else
                    {
                        if (this.skipped > 0)
                        {
                            this.logger.WriteLine(LogLevel.Info, "Skipped {0} items.", this.skipped);
                        }

                        this.continuation.Continue();
                    }
                }
            }

            /// <summary>
            /// Helper method for invoking asynchronous actions provided by tests or other framework components.
            /// Wraps the continuation with one that won't allow Continue to be called twice.
            /// </summary>
            /// <param name="continuation">The asynchronous continuation</param>
            /// <param name="action">The action to invoke on the callback</param>
            private static void InvokeAsynchronousAction(IAsyncContinuation continuation, Action<IAsyncContinuation> action)
            {
                ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
                ExceptionUtilities.CheckArgumentNotNull(action, "action");
                action(new SingleCallContinuationProxy(continuation));
            }
        }

        /// <summary>
        /// Implements <see cref="IAsyncContinuation"/> which delegates continue and fail calls
        /// to provided delegates.
        /// </summary>
        private class DelegateBasedContinuation : IAsyncContinuation
        {
            private Action onContinue;
            private Action<Exception> onFail;

            internal DelegateBasedContinuation(Action onContinue, Action<Exception> onFail)
            {
                this.onContinue = onContinue;
                this.onFail = onFail;
            }

            /// <summary>
            /// Reports success of asynchronous method invocation.
            /// </summary>
            public void Continue()
            {
                this.onContinue();
            }

            /// <summary>
            /// Reports the failure of asynchronous method invocation.
            /// </summary>
            /// <param name="exception">The exception.</param>
            public void Fail(Exception exception)
            {
                this.onFail(exception);
            }
        }

        /// <summary>
        /// Implements <see cref="IAsyncContinuation"/> which delegates success and failure calls
        /// to provided implementation while only allows Continue to be called once.
        /// </summary>
        private class SingleCallContinuationProxy : IAsyncContinuation
        {
            private IAsyncContinuation underlying;
            private bool called = false;

            /// <summary>
            /// Initializes a new instance of the SingleCallContinuationProxy class
            /// </summary>
            /// <param name="underlying">The underlying continuation</param>
            internal SingleCallContinuationProxy(IAsyncContinuation underlying)
            {
                ExceptionUtilities.CheckArgumentNotNull(underlying, "underlying");
                this.underlying = underlying;
            }

            /// <summary>
            /// Reports success of asynchronous method invocation.
            /// </summary>
            public void Continue()
            {
                if (this.called)
                {
                    this.underlying.Fail(new TaupoInvalidOperationException("Continue called more than once."));
                }
                else
                {
                    this.called = true;
                    this.underlying.Continue();
                }
            }

            /// <summary>
            /// Reports the failure of asynchronous method invocation.
            /// </summary>
            /// <param name="exception">The exception.</param>
            public void Fail(Exception exception)
            {
                this.underlying.Fail(exception);
            }
        }
    }
}