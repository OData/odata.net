//---------------------------------------------------------------------
// <copyright file="TestDataServiceClientRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.ClientExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Threading;
    using Microsoft.OData;

    internal class TestDataServiceClientRequestMessage : DataServiceClientRequestMessage
    {
        private readonly IODataRequestMessage requestMessage;
        private readonly Func<IODataResponseMessage> getResponseMessage;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        private bool sendChunked;
#endif
        private bool headersAlreadySent;

        internal TestDataServiceClientRequestMessage(IODataRequestMessage requestMessage, Func<IODataResponseMessage> getResponseMessage): base(requestMessage.Method)
        {
            this.requestMessage = requestMessage;
            this.getResponseMessage = getResponseMessage;
        }

        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.requestMessage.Headers; }
        }

        public override Uri Url
        {
            get { return this.requestMessage.Url; }
            set { this.requestMessage.Url = value; }
        }

        public override string Method
        {
            get { return this.requestMessage.Method; }
            set { this.requestMessage.Method = value; }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        public override bool SendChunked
        {
            get { return this.sendChunked; }
            set
            {
                if (this.headersAlreadySent)
                {
                    throw new InvalidOperationException("Headers cannot be modified since GetStream or GetResponse method has been called");
                }

                this.sendChunked = value;
            }
        }
#endif

        public override System.Net.ICredentials Credentials
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        public override int Timeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
#endif

        public override string GetHeader(string headerName)
        {
            return this.requestMessage.GetHeader(headerName);
        }

        public override void SetHeader(string headerName, string headerValue)
        {
            if (this.headersAlreadySent)
            {
                throw new InvalidOperationException("Headers cannot be modified since GetStream or GetResponse method has been called");
            }

            this.requestMessage.SetHeader(headerName, headerValue);
        }

        public override Stream GetStream()
        {
            this.LockHeaders();
            return this.requestMessage.GetStream();
        }

        public override void Abort()
        {
            // TODO: Need to implement this
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            this.LockHeaders();
            var asyncResult = new AsyncResultNoResult(callback, state);
            asyncResult.SetAsCompleted(null, true);
            return asyncResult;
        }

        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            var asyncResultNoResult = (AsyncResultNoResult)asyncResult;
            asyncResultNoResult.EndInvoke();
            return this.requestMessage.GetStream();
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            this.LockHeaders();
            var asyncResult = new AsyncResultNoResult(callback, state);
            asyncResult.SetAsCompleted(null, true);
            return asyncResult;
        }

        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            var asyncResultNoResult = (AsyncResultNoResult)asyncResult;
            asyncResultNoResult.EndInvoke();
            return this.getResponseMessage();
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        public override IODataResponseMessage GetResponse()
        {
            this.LockHeaders();
            return this.getResponseMessage();
        }
#endif

        private void LockHeaders()
        {
            if (this.SendChunked && !this.headersAlreadySent)
            {
                var headerValue = this.GetHeader("Transfer-Encoding");
                if (headerValue != "chunked")
                {
                    this.SetHeader("Transfer-Encoding", "chunked");
                }
            }

            this.headersAlreadySent = true;
        }

        private class AsyncResultNoResult : IAsyncResult
        {
            // Fields set at construction which never change while
            // operation is pending
            private readonly AsyncCallback asyncCallback;
            private readonly Object asyncState;

            // Fields set at construction which do change after
            // operation completes
            private const Int32 StatePending = 0;
            private const Int32 StateCompletedSynchronously = 1;
            private const Int32 StateCompletedAsynchronously = 2;
            private Int32 completedState = StatePending;

            // Field that may or may not get set depending on usage
            private ManualResetEvent asyncWaitHandle;

            // Fields set when operation completes
            private Exception exception;

            public AsyncResultNoResult(AsyncCallback asyncCallback, Object state)
            {
                this.asyncCallback = asyncCallback;
                this.asyncState = state;
            }

            public void SetAsCompleted(Exception ex, Boolean completedSynchronously)
            {
                // Passing null for exception means no error occurred.
                // This is the common case
                this.exception = ex;

                // The completedState field MUST be set prior calling the callback
                Int32 prevState = Interlocked.Exchange(ref this.completedState, completedSynchronously ? StateCompletedSynchronously : StateCompletedAsynchronously);
                if (prevState != StatePending)
                    throw new InvalidOperationException("You can set a result only once");

                // If the event exists, set it
                if (this.asyncWaitHandle != null)
                    this.asyncWaitHandle.Set();

                // If a callback method was set, call it
                if (this.asyncCallback != null)
                    this.asyncCallback(this);
            }

            public void EndInvoke()
            {
                // This method assumes that only 1 thread calls EndInvoke
                // for this object
                if (!IsCompleted)
                {
                    // If the operation isn't done, wait for it
                    AsyncWaitHandle.WaitOne();
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
                    AsyncWaitHandle.Dispose();
#else
                    AsyncWaitHandle.Close();
#endif
                    this.asyncWaitHandle = null;
                    // Allow early GC
                }

                // Operation is done: if an exception occured, throw it
                if (this.exception != null)
                    throw this.exception;
            }

#region Implementation of IAsyncResult
            public Object AsyncState
            {
                get { return this.asyncState; }
            }

            public Boolean CompletedSynchronously
            {
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
                // NET Core 1.0 does not have full support for threads. Replace Volate.Read with Thread.VolatileRead when on NET Core 2.0.
                get { return Volatile.Read(ref this.completedState) == StateCompletedSynchronously; }
#else
                get { return Thread.VolatileRead(ref this.completedState) == StateCompletedSynchronously; }
#endif
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (this.asyncWaitHandle == null)
                    {
                        Boolean done = IsCompleted;
                        ManualResetEvent mre = new ManualResetEvent(done);
                        if (Interlocked.CompareExchange(ref this.asyncWaitHandle, mre, null) != null)
                        {
                            // Another thread created this object's event; dispose
                            // the event we just created
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
                            mre.Dispose();
#else
                            mre.Close();
#endif
                        }
                        else
                        {
                            if (!done && IsCompleted)
                            {
                                // If the operation wasn't done when we created
                                // the event but now it is done, set the event
                                this.asyncWaitHandle.Set();
                            }
                        }
                    }

                    return this.asyncWaitHandle;
                }
            }

            public Boolean IsCompleted
            {
                get
                {
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
                    // NET Core 1.0 does not have full support for threads. Replace Volate.Read with Thread.VolatileRead when on NET Core 2.0.
                    return Volatile.Read(ref this.completedState) != StatePending;
#else
                    return Thread.VolatileRead(ref this.completedState) != StatePending;
#endif
                }
            }

#endregion
        }
    }
}