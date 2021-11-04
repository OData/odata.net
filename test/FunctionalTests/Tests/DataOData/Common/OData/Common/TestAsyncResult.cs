//---------------------------------------------------------------------
// <copyright file="TestAsyncResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// Enumeration of all the available async behaviors
    /// </summary>
    public enum AsyncMethodBehavior
    {
        Synchronous,
        AsynchronousImmediateSameThread,
        AsynchronousImmediateDifferentThread,
        AsynchronousDelayed
    }

    /// <summary>
    /// Implementation of IAsyncResult which defines some interesting behaviors
    /// </summary>
    /// <remarks>The assumption is that the test can complete the operation synchronously always, but it needs to simulate async behavior.
    /// This class provides the implementation of the IAsyncResult to return which will simulate interesting async behaviors.
    /// After creation the test should call the Start method once everything is setup and then return the value from its Begin... method.</remarks>
    public abstract class TestAsyncResult : IAsyncResult
    {
        /// <summary>
        /// The async callback to call when the operation has finished
        /// </summary>
        private AsyncCallback callback;

        /// <summary>
        /// The state object to pass to the callback
        /// </summary>
        private object asyncState;

        /// <summary>
        /// The value of the CompletedSynchronously property
        /// </summary>
        protected bool CompletedSynchronouslyValue { get; set; }

        /// <summary>
        /// The value of the IsCompleted property
        /// </summary>
        protected bool IsCompletedValue { get; set; }

        /// <summary>
        /// The Event which backs the WaitHandle
        /// </summary>
        protected ManualResetEvent AsyncWaitEvent { get; set; }

        /// <summary>
        /// Creates an implementation of the IAsyncResult with the specified behavior
        /// </summary>
        /// <param name="behavior">The behavior to use for the IAsyncResult.</param>
        /// <param name="callback">The callback for the async operation</param>
        /// <param name="state">The state of the callback.</param>
        /// <returns>The newly create IAsyncResult implementation.</returns>
        public static TestAsyncResult Create(AsyncMethodBehavior behavior, AsyncCallback callback, object state)
        {
            switch (behavior)
            {
                case AsyncMethodBehavior.Synchronous:
                    return new SynchronousTestAsyncResult(callback, state);
                case AsyncMethodBehavior.AsynchronousImmediateSameThread:
                    return new AsynchronousImmediateSameThreadTestAsyncResult(callback, state);
                case AsyncMethodBehavior.AsynchronousImmediateDifferentThread:
                    return new AsynchronousImmediateDifferentThreadTestAsyncResult(callback, state);
                case AsyncMethodBehavior.AsynchronousDelayed:
                    return new AsynchronousDelayedTestAsyncResult(callback, state);
                default:
                    throw new ArgumentException("Unrecognized behavior.", "behavior");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback">The callback for the async operation.</param>
        /// <param name="state">The state for the callback.</param>
        protected TestAsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.asyncState = state;
            this.AsyncWaitEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Starts the async operation
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Invokes the callback for the async operation.
        /// </summary>
        protected void InvokeCallback()
        {
            this.callback(this);
        }

        /// <summary>
        /// Custom state for the test to store data in.
        /// </summary>
        public object CustomState { get; set; }

        /// <summary>
        /// The custom specified state
        /// </summary>
        public object AsyncState { get { return this.asyncState; } }

        /// <summary>
        /// The wait handle
        /// </summary>
        public System.Threading.WaitHandle AsyncWaitHandle { get { return this.AsyncWaitEvent; } }

        /// <summary>
        /// True if the operation completed synchronously
        /// </summary>
        public bool CompletedSynchronously { get { return this.CompletedSynchronouslyValue; } }

        /// <summary>
        /// True if the operation completed
        /// </summary>
        public bool IsCompleted { get { return this.IsCompletedValue; } }

        /// <summary>
        /// Implementation which completes the operation synchronously
        /// </summary>
        private class SynchronousTestAsyncResult : TestAsyncResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="callback">The callback for the async operation.</param>
            /// <param name="state">The state for the callback.</param>
            public SynchronousTestAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            /// <summary>
            /// Starts the async operation
            /// </summary>
            public override void Start()
            {
                this.AsyncWaitEvent.Set();
                this.CompletedSynchronouslyValue = true;
                this.IsCompletedValue = true;
                this.InvokeCallback();
            }
        }

        /// <summary>
        /// Implementation which completes the operation asynchronously but completes it before returning from Start on the same thread
        /// </summary>
        private class AsynchronousImmediateSameThreadTestAsyncResult : TestAsyncResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="callback">The callback for the async operation.</param>
            /// <param name="state">The state for the callback.</param>
            public AsynchronousImmediateSameThreadTestAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            /// <summary>
            /// Starts the async operation
            /// </summary>
            public override void Start()
            {
                this.AsyncWaitEvent.Set();
                this.CompletedSynchronouslyValue = false;
                this.IsCompletedValue = true;
                this.InvokeCallback();
            }
        }

        /// <summary>
        /// Implementation which completes the operation asynchronously but completes it before returning from the Start on a different thread
        /// </summary>
        private class AsynchronousImmediateDifferentThreadTestAsyncResult : TestAsyncResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="callback">The callback for the async operation.</param>
            /// <param name="state">The state for the callback.</param>
            public AsynchronousImmediateDifferentThreadTestAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            /// <summary>
            /// Starts the async operation
            /// </summary>
            public override void Start()
            {
                this.AsyncWaitEvent.Set();
                this.CompletedSynchronouslyValue = false;
                this.IsCompletedValue = true;
                Task.Factory.StartNew(() =>
                {
                    this.InvokeCallback();
                }).Wait();
            }
        }

        /// <summary>
        /// Implementation which completes the operation asynchronously dealyed by 10 milliseconds on a different thread
        /// </summary>
        private class AsynchronousDelayedTestAsyncResult : TestAsyncResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="callback">The callback for the async operation.</param>
            /// <param name="state">The state for the callback.</param>
            public AsynchronousDelayedTestAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            /// <summary>
            /// Starts the async operation
            /// </summary>
            public override void Start()
            {
                this.AsyncWaitEvent.Set();
                this.CompletedSynchronouslyValue = false;

                Timer timer = null;
                timer = new Timer((o) =>
                {
                    timer.Dispose();
                    this.IsCompletedValue = true;
                    this.InvokeCallback();
                },
                    null,
                    10,
                    Timeout.Infinite);
            }
        }
    }
}
