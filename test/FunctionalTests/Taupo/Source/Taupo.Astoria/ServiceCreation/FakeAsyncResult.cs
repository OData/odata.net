//---------------------------------------------------------------------
// <copyright file="FakeAsyncResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ServiceCreation
{
    using System;
    using System.Threading;

    /// <summary>
    /// Mock IAsync implementation so that DataServiceWebServerLookup can 
    /// be tested without using DataServices
    /// </summary>
    internal class FakeAsyncResult : IAsyncResult
    {
        private AsyncCallback asyncCallback;
        private bool isCompleted;
        private object state;

        /// <summary>
        /// Initializes a new instance of the FakeAsyncResult class
        /// </summary>
        /// <param name="asyncCallback">Callback to invoke when waiting has completed</param>
        /// <param name="state">state to save</param>
        internal FakeAsyncResult(AsyncCallback asyncCallback, object state)
        {
            this.isCompleted = false;
            this.asyncCallback = asyncCallback;
            this.state = state;

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.Invoke));
        }
        
        /// <summary>
        /// Gets the Async state
        /// </summary>
        public object AsyncState
        {
            get { return this.state; }
        }

        /// <summary>
        /// Gets a value indicating whether the async state, always returns null
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a value indicating whether CompletedSynchronously is true or false, its always true
        /// </summary>
        public bool CompletedSynchronously
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether IsCompleted is true or false, its always true
        /// </summary>
        public bool IsCompleted
        {
            get { return this.isCompleted; }
        }

        private void Invoke(object asyncState)
        {
            this.asyncCallback.Invoke(this);
            this.isCompleted = true;
        }
    }
}
