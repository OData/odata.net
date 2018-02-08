//---------------------------------------------------------------------
// <copyright file="BaseAsyncResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData;

    /// <summary>
    /// Implementation of IAsyncResult
    /// </summary>
    internal abstract class BaseAsyncResult : IAsyncResult
    {
        /// <summary>Originating object, used to validate End*</summary>
        internal readonly object Source;

        /// <summary>Originating method on source, to differentiate between different methods from same source</summary>
        internal readonly string Method;

        /// <summary>wrapped request</summary>
        protected PerRequest perRequest;

        /// <summary>
        /// The int equivalent for true.
        /// </summary>
        private const int True = 1;

        /// <summary>
        /// The int equivalent for false.
        /// </summary>
        private const int False = 0;

        /// <summary>User callback passed to Begin*</summary>
        private readonly AsyncCallback userCallback;

        /// <summary>User state passed to Begin*</summary>
        private readonly object userState;

        /// <summary>wait handle for user to wait until done, we only use this within lock of asyncWaitDisposeLock.</summary>
        private System.Threading.ManualResetEvent asyncWait;

        /// <summary>Holding exception to throw as a nested exception during to End*</summary>
        private Exception failure;

        /// <summary>Abortable request</summary>
        private ODataRequestMessageWrapper abortable;

        /// <summary>true unless something completes asynchronously</summary>
        private int completedSynchronously = BaseAsyncResult.True;

        /// <summary>true when really completed for the user</summary>
        private bool userCompleted;

        /// <summary>true when no more changes are pending, 0 false, 1 completed, 2 aborted</summary>
        private int completed;

        /// <summary>verify we only invoke the user callback once, 0 false, 1 true</summary>
        private int userNotified;

        /// <summary>non-zero after End*, 0 false, 1, true</summary>
        private int done;

        /// <summary>true if the AsyncWaitHandle has already been disposed.</summary>
        private bool asyncWaitDisposed;

        /// <summary>delay created object to lock to prevent using disposed asyncWait handle.</summary>
        private object asyncWaitDisposeLock;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="source">source object of async request</param>
        /// <param name="method">async method name on source object</param>
        /// <param name="callback">user callback to invoke when complete</param>
        /// <param name="state">user state</param>
        internal BaseAsyncResult(object source, string method, AsyncCallback callback, object state)
        {
            Debug.Assert(null != source, "null source");
            this.Source = source;
            this.Method = method;
            this.userCallback = callback;
            this.userState = state;
        }

        /// <summary>
        /// This delegate exists to workaround limitations in the WP7 runtime.
        /// When limitations on the number of parameters to Func&lt;&gt; are resolved, this can be subsumed by the following:
        /// Func&lt;byte[], int, int, AsyncCallback, object, IAsyncResult&gt;
        /// </summary>
        /// <param name="buffer">buffer to transfer the data</param>
        /// <param name="offset">byte offset in buffer</param>
        /// <param name="length">max number of bytes in the buffer</param>
        /// <param name="asyncCallback">async callback to be called when the operation is complete</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous request from other requests.</param>
        /// <returns>An IAsyncResult that represents the asynchronous operation, which could still be pending</returns>
        internal delegate IAsyncResult AsyncAction(byte[] buffer, int offset, int length, AsyncCallback asyncCallback, object state);

        #region IAsyncResult implementation - AsyncState, AsyncWaitHandle, CompletedSynchronously, IsCompleted

        /// <summary>user state object parameter</summary>
        public object AsyncState
        {
            get { return this.userState; }
        }

        /// <summary>wait handle for when waiting is required</summary>
        /// <remarks>if displayed by debugger, it undesirable to create the WaitHandle</remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get
            {
                if (null == this.asyncWait)
                {   // delay create the wait handle since the user may never use it
                    // like asyncWait which will be GC'd, the losers in creating the asyncWait will also be GC'd
                    System.Threading.Interlocked.CompareExchange(ref this.asyncWait, new System.Threading.ManualResetEvent(this.IsCompleted), null);

                    // multi-thread condition
                    // 1) thread 1 returned IAsyncResult and !IsCompleted so AsyncWaitHandle.WaitOne()
                    // 2) thread 2 signals complete, however thread 1 has retrieved this.completed but not assigned asyncWait
                    if (this.IsCompleted)
                    {   // yes, Set may be called multiple times - but user would have to assume ManualResetEvent and call Reset
                        //
                        // There is a very small window for race condition between setting the wait handle here and disposing
                        // the wait handle inside EndExecute(). Say thread1 calls EndExecute() and IsCompleted is already true we won't
                        // create asyncWait from inside thread1, and thread2 wakes up right before thread1 tries to dispose the handle and
                        // thread2 calls AsyncWaitHandle which creates a new asyncWait handle, if thread1 wakes up right before the Set event
                        // here and disposes the asyncWait handle we just created here, Set() will throw ObjectDisposedException.
                        // SetAsyncWaitHandle() will protect this scenario with a critical section.
                        this.SetAsyncWaitHandle();
                    }
                }

                // Note that if the first time AsyncWaitHandle gets called is after EndExecute() is completed, we don't dispose the
                // newly created handle, it'll just get GC'd.
                return this.asyncWait;
            }
        }

        /// <summary>did the result complete synchronously?</summary>
        public bool CompletedSynchronously
        {
            get
            {
                return this.completedSynchronously == BaseAsyncResult.True;
            }
        }

        /// <summary>is the result complete?</summary>
        public bool IsCompleted
        {
            get { return this.userCompleted; }
        }

        /// <summary>is the result complete?</summary>
        internal bool IsCompletedInternally
        {
            get { return (0 != this.completed); }
        }

        /// <summary>abort the result</summary>
        internal bool IsAborted
        {
            get { return (2 == this.completed); }
        }

        #endregion

        /// <summary>
        /// WebRequest available for DataServiceContext.CancelRequest
        /// </summary>
        internal ODataRequestMessageWrapper Abortable
        {
            get
            {
                return this.abortable;
            }

            set
            {
                this.abortable = value;
                if ((null != value) && this.IsAborted)
                {   // if the value hadn't been set yet, but aborting then propagate the abort
                    value.Abort();
                }
            }
        }

        /// <summary>first exception that happened</summary>
        internal Exception Failure
        {
            get { return this.failure; }
        }

        /// <summary>
        /// common handler for EndExecuteBatch &amp; EndSaveChanges
        /// </summary>
        /// <typeparam name="T">derived type of the AsyncResult</typeparam>
        /// <param name="source">source object of async request</param>
        /// <param name="method">async method name on source object</param>
        /// <param name="asyncResult">the asyncResult being ended</param>
        /// <returns>data service response for batch</returns>
        internal static T EndExecute<T>(object source, string method, IAsyncResult asyncResult) where T : BaseAsyncResult
        {
            Util.CheckArgumentNull(asyncResult, "asyncResult");

            T result = (asyncResult as T);
            if ((null == result) || (source != result.Source) || (result.Method != method))
            {
                throw Error.Argument(Strings.Context_DidNotOriginateAsync, "asyncResult");
            }

            Debug.Assert((result.CompletedSynchronously && result.IsCompleted) || !result.CompletedSynchronously, "CompletedSynchronously && !IsCompleted");

            if (!result.IsCompleted)
            {   // if the user doesn't want to wait forever, they should explicitly wait on the handle with a timeout
                result.AsyncWaitHandle.WaitOne();

                Debug.Assert(result.IsCompleted, "not completed after waiting");
            }

            // Prevent EndExecute from being called more than once.
            if (System.Threading.Interlocked.Exchange(ref result.done, 1) != 0)
            {
                throw Error.Argument(Strings.Context_AsyncAlreadyDone, "asyncResult");
            }

            // Dispose the wait handle.
            if (null != result.asyncWait)
            {
                System.Threading.Interlocked.CompareExchange(ref result.asyncWaitDisposeLock, new object(), null);
                lock (result.asyncWaitDisposeLock)
                {
                    result.asyncWaitDisposed = true;
                    Util.Dispose(result.asyncWait);
                }
            }

            if (result.IsAborted)
            {
                throw Error.InvalidOperation(Strings.Context_OperationCanceled);
            }

            if (null != result.Failure)
            {
                if (Util.IsKnownClientExcption(result.Failure))
                {
                    throw result.Failure;
                }

                throw Error.InvalidOperation(Strings.DataServiceException_GeneralError, result.Failure);
            }

            return result;
        }

        /// <summary>
        /// Due to the unexpected behaviors of IAsyncResult.CompletedSynchronously in the System.Net networking stack, we have to make
        /// async calls to their APIs using the specific pattern they've prescribed. This method runs in the caller thread and invokes
        /// the BeginXXX methods.  It then checks IAsyncResult.CompletedSynchronously and if it is true, we invoke the callback in the
        /// caller thread.
        /// </summary>
        /// <param name="asyncAction">
        /// This is the action that invokes the BeginXXX method. Note we MUST use our special callback from GetDataServiceAsyncCallback()
        /// when invoking the async call.
        /// </param>
        /// <param name="callback">async callback to be called when the operation is complete</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous request from other requests.</param>
        /// <returns>Returns the async result from the BeginXXX method.</returns>
        /// <remarks>
        /// CompletedSynchronously (for System.Net networking stack) means "was the operation completed before the first time
        /// that somebody asked if it was completed synchronously"? They do this because some of their asynchronous operations
        /// (particularly those in the Socket class) will avoid the cost of capturing and transferring the ExecutionContext
        /// to the callback thread by checking CompletedSynchronously, and calling the callback from within BeginXxx instead of
        /// on the completion port thread if the native winsock call completes quickly.
        ///
        /// For other operations however (notably those in HttpWebRequest), they use the same underlying IAsyncResult implementation,
        /// but do NOT check CompletedSynchronously before returning from BeginXxx.  That means that CompletedSynchronously will
        /// be false if and only if you checked it from the thread which called BeginXxx BEFORE the operation completed.  It will
        /// then continue to be false even after IsCompleted becomes true.
        ///
        /// Note that CompletedSynchronously == true does not guarantee anything about how much of your callback has executed.
        ///
        /// The usual pattern for handling synchronous completion is that both the caller and callback should check CompletedSynchronously.
        /// If its true, the callback should do nothing and the caller should call EndRead and process the result.
        /// This guarantees that the caller and callback are not accessing the stream or buffer concurrently without the need
        /// for explicit synchronization between the two.
        /// </remarks>
        internal static IAsyncResult InvokeAsync(Func<AsyncCallback, object, IAsyncResult> asyncAction, AsyncCallback callback, object state)
        {
            IAsyncResult asyncResult = asyncAction(BaseAsyncResult.GetDataServiceAsyncCallback(callback), state);
            return PostInvokeAsync(asyncResult, callback);
        }

#if PORTABLELIB
        /// <summary>
        /// This is the Win8 version of the InvokeAsync overload below. See comments on that method for more details.
        /// </summary>
        /// <remarks>
        /// Beta bits of the Win8 profile always return false for IAsyncResult.CompletedSynchronously, but that
        /// is not guaranteed, so keeping the existing pattern here that we use on other platforms.
        /// </remarks>
        /// <param name="task">
        /// Func that invokes the async operation. We must use our special callback from GetDataServiceAsyncCallback(), see InvokeAsync comments below for details.
        /// </param>
        /// <param name="buffer">buffer to transfer the data</param>
        /// <param name="offset">byte offset in buffer</param>
        /// <param name="length">max number of bytes in the buffer</param>
        /// <param name="callback">async callback to be called when the operation is complete</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous request from other requests.</param>
        /// <returns>An Task that represents the asynchronous operation, which could still be pending.</returns>
        internal static Task InvokeTask(Func<byte[], int, int, Task> task, byte[] buffer, int offset, int length, Action<Task, object> callback, object state)
        {
            Action<Task, object> taskCallback = BaseAsyncResult.GetDataServiceTaskCallback(callback);
            Task returnTask = task(buffer, offset, length).FollowOnSuccessWith(t => taskCallback(t, state));
            return PostInvokeTask(returnTask, callback, state);
        }
#else
        /// <summary>
        /// Due to the unexpected behaviors of IAsyncResult.CompletedSynchronously in the System.Net networking stack, we have to make
        /// async calls to their APIs using the specific pattern they've prescribed. This method runs in the caller thread and invokes
        /// the BeginXXX methods.  It then checks IAsyncResult.CompletedSynchronously and if it is true, we invoke the callback in the
        /// caller thread.
        /// </summary>
        /// <param name="asyncAction">
        /// This is the action that invokes the BeginXXX method. Note we MUST use our special callback from GetDataServiceAsyncCallback()
        /// when invoking the async call.
        /// </param>
        /// <param name="buffer">buffer to transfer the data</param>
        /// <param name="offset">byte offset in buffer</param>
        /// <param name="length">max number of bytes in the buffer</param>
        /// <param name="callback">async callback to be called when the operation is complete</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous request from other requests.</param>
        /// <returns>An IAsyncResult that represents the asynchronous operation, which could still be pending</returns>
        /// <remarks>
        /// Please see remarks on the other InvokeAsync() overload.
        /// Also note that the InvokeTask method above is a Win8 version of this method, so it should be kept in sync with any changes that occur here.
        /// </remarks>
        internal static IAsyncResult InvokeAsync(AsyncAction asyncAction, byte[] buffer, int offset, int length, AsyncCallback callback, object state)
        {
            IAsyncResult asyncResult = asyncAction(buffer, offset, length, BaseAsyncResult.GetDataServiceAsyncCallback(callback), state);
            return PostInvokeAsync(asyncResult, callback);
        }
#endif

        /// <summary>
        /// Sets the CompletedSynchronously property.
        /// </summary>
        /// <param name="isCompletedSynchronously">true if the async operation was completed synchronously, false otherwise.</param>
        internal void SetCompletedSynchronously(bool isCompletedSynchronously)
        {
            // This is equivalent to "CompletedSynchronously &= completedSynchronously". The &= operator is a nonatomic operation involving a read and a write operations.
            Interlocked.CompareExchange(ref this.completedSynchronously, isCompletedSynchronously ? BaseAsyncResult.True : BaseAsyncResult.False, BaseAsyncResult.True);
            Debug.Assert(isCompletedSynchronously || this.completedSynchronously == BaseAsyncResult.False, "this.completedSynchronously must be false if the isCompletedSynchronously argument is false.");
        }

        /// <summary>Set the AsyncWait and invoke the user callback.</summary>
        /// <remarks>
        /// If the background thread gets a ThreadAbort, the userCallback will never be invoked.
        /// This is why it's generally important to never wait forever, but to have more specific
        /// time limit.  Also then cancel the operation, to make sure its stopped, to avoid
        /// multi-threading if your wait time limit was just too short.
        /// </remarks>
        internal void HandleCompleted()
        {
            // TODO: even if background thread of async operation encounters
            // an "uncatchable" exception, do the minimum to unblock the async result.
            if (this.IsCompletedInternally && (System.Threading.Interlocked.Exchange(ref this.userNotified, 1) == 0))
            {
                this.abortable = null; // reset abort via CancelRequest
                try
                {
                    // avoid additional work when aborting for exceptional reasons
                    if (CommonUtil.IsCatchableExceptionType(this.Failure))
                    {
                        // the CompleteRequest may do additional work which is why
                        // it is important not to signal the user via either the
                        // IAsyncResult.IsCompleted, IAsyncResult.WaitHandle or the callback
                        this.CompletedRequest();
                    }
                }
                catch (Exception ex)
                {
                    if (this.HandleFailure(ex))
                    {
                        throw;
                    }
                }
                finally
                {
                    // 1. set IAsyncResult.IsCompleted, otherwise user was
                    // signalled on another thread, but the property may not be true.
                    this.userCompleted = true;

                    // 2. signal the wait handle because it can't be first nor can it be last.
                    //
                    // There is a very small window for race condition between setting the wait handle here and disposing
                    // the wait handle inside EndExecute(). Say thread1 is the async thread that executes up till this point, i.e. right
                    // after userCompleted is set to true and before the asyncWait is signaled; thread2 wakes up and calls EndExecute() till
                    // right before we try to dispose the wait handle; thread3 wakes up and calls AsyncWaitHandle which creates a new instance
                    // for this.asyncWait; thread2 then resumes to dispose this.asyncWait and if at this point thread1 sets this.asyncWait,
                    // we'll get an ObjectDisposedException on thread1.  SetAsyncWaitHandle() will protect this scenario with a critical section.
                    this.SetAsyncWaitHandle();

                    // 3. invoke the callback because user may throw an exception and stop any further processing
#if PORTABLELIB
                    if ((null != this.userCallback))
#else
                    if ((null != this.userCallback) && !(this.Failure is System.Threading.ThreadAbortException) && !(this.Failure is System.StackOverflowException))
#endif
                    {   // any exception thrown by user should be "unhandled"
                        // it's possible callback will be invoked while another creates and sets the asyncWait
                        this.userCallback(this);
                    }
                }
            }
        }

        /// <summary>Cache the exception that happened on the background thread for the caller of EndSaveChanges.</summary>
        /// <param name="e">exception object from background thread</param>
        /// <returns>true if the exception (like StackOverflow or ThreadAbort) should be rethrown</returns>
        internal bool HandleFailure(Exception e)
        {
            System.Threading.Interlocked.CompareExchange(ref this.failure, e, null);
            this.SetCompleted();
            return !CommonUtil.IsCatchableExceptionType(e);
        }

        /// <summary>Set the async result as completed and aborted.</summary>
        internal void SetAborted()
        {
            System.Threading.Interlocked.Exchange(ref this.completed, 2);
        }

        /// <summary>Set the async result as completed.</summary>
        internal void SetCompleted()
        {
            System.Threading.Interlocked.CompareExchange(ref this.completed, 1, 0);
        }

        /// <summary>verify they have the same reference</summary>
        /// <param name="actual">the actual thing</param>
        /// <param name="expected">the expected thing</param>
        /// <param name="errorcode">error code if they are not</param>
        protected static void EqualRefCheck(PerRequest actual, PerRequest expected, InternalError errorcode)
        {
            if (!Object.ReferenceEquals(actual, expected))
            {
                Error.ThrowInternalError(errorcode);
            }
        }

        /// <summary>invoked for derived classes to cleanup before callback is invoked</summary>
        protected abstract void CompletedRequest();

        /// <summary>Disposes the request object if it is not null. Invokes the user callback</summary>
        /// <param name="pereq">the request object</param>
        protected abstract void HandleCompleted(PerRequest pereq);

        /// <summary>handle request.BeginGetResponse with request.EndGetResponse and then copy response stream</summary>
        /// <param name="asyncResult">async result</param>
        protected abstract void AsyncEndGetResponse(IAsyncResult asyncResult);

        /// <summary>verify non-null and not completed</summary>
        /// <param name="value">the request in progress</param>
        /// <param name="errorcode">error code if null or completed</param>
        protected virtual void CompleteCheck(PerRequest value, InternalError errorcode)
        {
            if ((null == value) || value.RequestCompleted)
            {
                // since PerRequest is nested, it won't get set true during Abort unlike BaseAsyncResult
                // but like QueryAsyncResult, when the request is aborted it it lets the request throw on next operation
                Error.ThrowInternalError(errorcode);
            }
        }

        /// <summary>Read and store response data for the current change, and try to start the next one</summary>
        /// <param name="pereq">the completed per request object</param>
        protected virtual void FinishCurrentChange(PerRequest pereq)
        {
            if (!pereq.RequestCompleted)
            {
                Error.ThrowInternalError(InternalError.SaveNextChangeIncomplete);
            }

            // Note that this.perRequest can be set to null by another thread executing concurrently in this.HandleCompleted(pereq).
            // We need to cache the value for this.perRequest and only call EqualRefCheck() if it is not null.
            PerRequest request = this.perRequest;
            if (null != request)
            {
                EqualRefCheck(request, pereq, InternalError.InvalidSaveNextChange);
            }
        }

        /// <summary>Cache the exception that happened on the background thread for the caller of EndSaveChanges.</summary>
        /// <param name="pereq">the request object</param>
        /// <param name="e">exception object from background thread</param>
        /// <returns>true if the exception should be rethrown</returns>
        protected bool HandleFailure(PerRequest pereq, Exception e)
        {
            if (null != pereq)
            {
                if (this.IsAborted)
                {
                    pereq.SetAborted();
                }
                else
                {
                    pereq.SetComplete();
                }
            }

            return this.HandleFailure(e);
        }

        /// <summary>handle request.BeginGetRequestStream with request.EndGetRequestStream and then write out request stream</summary>
        /// <param name="asyncResult">async result</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        protected void AsyncEndGetRequestStream(IAsyncResult asyncResult)
        {
            Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult.IsCompleted");
            AsyncStateBag asyncState = asyncResult.AsyncState as AsyncStateBag;
            PerRequest pereq = asyncState == null ? null : asyncState.PerRequest;

            try
            {
                this.CompleteCheck(pereq, InternalError.InvalidEndGetRequestCompleted);
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginGetRequestStream

                EqualRefCheck(this.perRequest, pereq, InternalError.InvalidEndGetRequestStream);

                ODataRequestMessageWrapper requestMessage = Util.NullCheck(pereq.Request, InternalError.InvalidEndGetRequestStreamRequest);

                Stream httpRequestStream = Util.NullCheck(requestMessage.EndGetRequestStream(asyncResult), InternalError.InvalidEndGetRequestStreamStream);
                pereq.RequestStream = httpRequestStream;

                ContentStream contentStream = pereq.RequestContentStream;
                Util.NullCheck(contentStream, InternalError.InvalidEndGetRequestStreamContent);
                Util.NullCheck(contentStream.Stream, InternalError.InvalidEndGetRequestStreamContent);

                if (contentStream.IsKnownMemoryStream)
                {
                    MemoryStream memoryStream = contentStream.Stream as MemoryStream;
#if PORTABLELIB
                    byte[] buffer = memoryStream.ToArray();
#else
                    byte[] buffer = memoryStream.GetBuffer();
#endif
                    int bufferOffset = checked((int)memoryStream.Position);
                    int bufferLength = checked((int)memoryStream.Length) - bufferOffset;
                    if ((null == buffer) || (0 == bufferLength))
                    {
                        Error.ThrowInternalError(InternalError.InvalidEndGetRequestStreamContentLength);
                    }
                }

                // Start the Read on the request content stream.
                // Note that we don't deal with synchronous results here.
                // If the read finishes synchronously the AsyncRequestContentEndRead will be called from inside the BeginRead
                //   call below. In there we will call BeginWrite. If that completes synchronously we will loop
                //   and call BeginRead again. If that completes synchronously as well we will call BeginWrite and so on.
                //   AsyncEndWrite will return immediately if it finished synchronously (otherwise it calls BeginRead).
                // So in the worst case we will have a stack like this:
                //   AsyncEndGetRequestStream
                //     AsyncRequestContentEndRead
                //       AsyncRequestContentEndRead or AsyncEndWrite

                // We just need to differentiate between the first AsyncRequestContentEndRead and the others (the first one
                //   must not return even if it completed synchronously, otherwise we would have to do the loop here as well).
                //   We'll use the RequestContentBufferValidLength as the notification. It will start with -1 which means
                //   we didn't read anything at all and thus it's the first read ending.
                pereq.RequestContentBufferValidLength = -1;

                Util.DebugInjectFault("SaveAsyncResult::AsyncEndGetRequestStream_BeforeBeginRead");
#if PORTABLELIB
                asyncResult = BaseAsyncResult.InvokeTask(contentStream.Stream.ReadAsync, pereq.RequestContentBuffer, 0, pereq.RequestContentBuffer.Length, this.AsyncRequestContentEndRead, asyncState);
#else
                asyncResult = BaseAsyncResult.InvokeAsync(contentStream.Stream.BeginRead, pereq.RequestContentBuffer, 0, pereq.RequestContentBuffer.Length, this.AsyncRequestContentEndRead, asyncState);
#endif
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
            }
            catch (Exception e)
            {
                if (this.HandleFailure(pereq, e))
                {
                    throw;
                }
            }
            finally
            {
                this.HandleCompleted(pereq);
            }
        }

        /// <summary>
        /// Due to the unexpected behaviors of IAsyncResult.CompletedSynchronously in the System.Net networking stack, we have to make
        /// async calls to their APIs using the specific pattern they've prescribed. This method runs in the caller thread after the
        /// BeginXXX method returns.  It checks IAsyncResult.CompletedSynchronously and if it is true, we invoke the callback in the
        /// caller thread.
        /// </summary>
        /// <param name="asyncResult">The IAsyncResult that represents the asynchronous operation we just called, which could still be pending</param>
        /// <param name="callback">Callback to be invoked when IAsyncResult.CompletedSynchronously is true.</param>
        /// <returns>Returns an IAsyncResult that represents the asynchronous operation we just called, which could still be pending</returns>
        /// <remarks>
        /// Please see remarks on BaseAsyncResult.InvokeAsync().
        /// Also note that PostInvokeTask below is a Win8 version of this method, so it should be kept in sync with any changes that occur here.
        /// </remarks>
        private static IAsyncResult PostInvokeAsync(IAsyncResult asyncResult, AsyncCallback callback)
        {
            Debug.Assert(asyncResult != null, "asyncResult != null");
            if (asyncResult.CompletedSynchronously)
            {
                Debug.Assert(asyncResult.IsCompleted, "asyncResult.IsCompleted");
                callback(asyncResult);
            }

            return asyncResult;
        }

#if PORTABLELIB
        /// <summary>
        /// This is the Win8 version of the PostInvokeAsync method above. Note that method is still used where possible on Win8, but there are some
        /// case where the Win8 API has been completely migrated to use Task, so this method supports that usage.
        /// </summary>
        /// <remarks>
        /// See PostInvokeAsync for more details.
        /// </remarks>
        /// <param name="task">The Task that represents the asynchronous operation we just called, which could still be pending</param>
        /// <param name="callback">Callback to be invoked when IAsyncResult.CompletedSynchronously is true.</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous request from other requests.</param>
        /// <returns>Returns a Task that represents the asynchronous operation we just called, which could still be pending.</returns>
        private static Task PostInvokeTask(Task task, Action<Task, object> callback, object state)
        {
            Debug.Assert(task != null, "task != null");
            if (((IAsyncResult)task).CompletedSynchronously)
            {
                Debug.Assert(task.IsCompleted, "asyncResult.IsCompleted");
                callback(task, state);
            }

            return task;
        }
#endif

        /// <summary>
        /// Due to the unexpected behaviors of IAsyncResult.CompletedSynchronously in the System.Net networking stack, we have to make
        /// async calls to their APIs using the specific pattern they've prescribed. This method returns an AsyncCallback which we can pass
        /// to the BeginXXX methods in the caller thread.  The returned callback will only run the wrapped callback if
        /// IAsyncResult.CompletedSynchronously is false, otherwise it returns immediately.
        /// </summary>
        /// <param name="callback">callback to be wrapped</param>
        /// <returns>Returns a callback which will only run the wrapped callback if IAsyncResult.CompletedSynchronously is false, otherwise it returns immediately.</returns>
        /// <remarks>
        /// Please see remarks on BaseAsyncResult.InvokeAsync().
        /// Also note that the GetDataServiceTaskCallback method below is a Win8 version of this method, so it should be kept in sync with any changes that occur here.
        /// </remarks>
        private static AsyncCallback GetDataServiceAsyncCallback(AsyncCallback callback)
        {
            return (asyncResult) =>
            {
                Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult != null && asyncResult.IsCompleted");
                if (asyncResult.CompletedSynchronously)
                {
                    return;
                }

                callback(asyncResult);
            };
        }

#if PORTABLELIB
        /// <summary>
        /// This is the Win8 version of the GetDataServiceAsyncCallback overload above. See comments on that method for more details.
        /// </summary>
        /// <remarks>
        /// Beta bits of the Win8 .NETCore profile always return false for IAsyncResult.CompletedSynchronously, but that
        /// is not guaranteed, so keeping the existing pattern here that we use on other platforms.
        /// </remarks>
        /// <param name="callback">callback to be wrapped</param>
        /// <returns>Returns a callback which will only run the wrapped callback if IAsyncResult.CompletedSynchronously is false, otherwise it returns immediately.</returns>
        private static Action<Task, object> GetDataServiceTaskCallback(Action<Task, object> callback)
        {
            return (task, state) =>
            {
                Debug.Assert(task != null && task.IsCompleted, "task != null && task.IsCompleted");
                if (((IAsyncResult)task).CompletedSynchronously)
                {
                    return;
                }

                callback(task, state);
            };
        }
#endif

        /// <summary>
        /// Sets the async wait handle
        /// </summary>
        private void SetAsyncWaitHandle()
        {
            if (null != this.asyncWait)
            {
                System.Threading.Interlocked.CompareExchange(ref this.asyncWaitDisposeLock, new object(), null);
                lock (this.asyncWaitDisposeLock)
                {
                    if (!this.asyncWaitDisposed)
                    {
                        this.asyncWait.Set();
                    }
                }
            }
        }

#if PORTABLELIB
        /// <summary>
        /// Callback for Stream.ReadAsync on the request content input stream. Calls request content output stream WriteAsync
        /// and in case of synchronous also the next ReadAsync.
        /// </summary>
        /// <param name="task">The task associated with the completed operation.</param>
        /// <param name="asyncState">State associated with the task.</param>
        private void AsyncRequestContentEndRead(Task task, object asyncState)
#else
        /// <summary>
        /// Callback for Stream.BeginRead on the request content input stream. Calls request content output stream BeginWrite
        /// and in case of synchronous also the next BeginRead.
        /// </summary>
        /// <param name="asyncResult">The asynchronous result associated with the completed operation.</param>
        private void AsyncRequestContentEndRead(IAsyncResult asyncResult)
#endif
        {
#if PORTABLELIB
            IAsyncResult asyncResult = (IAsyncResult)task;
#endif
            Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult.IsCompleted");
#if PORTABLELIB
            AsyncStateBag asyncStateBag = asyncState as AsyncStateBag;
#else
            AsyncStateBag asyncStateBag = asyncResult.AsyncState as AsyncStateBag;
#endif
            PerRequest pereq = asyncStateBag == null ? null : asyncStateBag.PerRequest;

            try
            {
                this.CompleteCheck(pereq, InternalError.InvalidEndReadCompleted);
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginRead

                EqualRefCheck(this.perRequest, pereq, InternalError.InvalidEndRead);

                ContentStream contentStream = pereq.RequestContentStream;
                Util.NullCheck(contentStream, InternalError.InvalidEndReadStream);
                Util.NullCheck(contentStream.Stream, InternalError.InvalidEndReadStream);

                Stream httpRequestStream = Util.NullCheck(pereq.RequestStream, InternalError.InvalidEndReadStream);

                Util.DebugInjectFault("SaveAsyncResult::AsyncRequestContentEndRead_BeforeEndRead");
#if PORTABLELIB
                int count = ((Task<int>)task).Result;
#else
                int count = contentStream.Stream.EndRead(asyncResult);
#endif
                if (0 < count)
                {
                    bool firstEndRead = (pereq.RequestContentBufferValidLength == -1);
                    pereq.RequestContentBufferValidLength = count;

                    // If we completed synchronously then just return. Our caller will take care of processing the results.
                    // First EndRead must not return even if completed synchronously.
                    if (!asyncResult.CompletedSynchronously || firstEndRead)
                    {
                        do
                        {
                            // Write the data we've read to the request stream
                            Util.DebugInjectFault("SaveAsyncResult::AsyncRequestContentEndRead_BeforeBeginWrite");
#if PORTABLELIB
                            asyncResult = BaseAsyncResult.InvokeTask(httpRequestStream.WriteAsync, pereq.RequestContentBuffer, 0, pereq.RequestContentBufferValidLength, this.AsyncEndWrite, asyncStateBag);
#else
                            asyncResult = BaseAsyncResult.InvokeAsync(httpRequestStream.BeginWrite, pereq.RequestContentBuffer, 0, pereq.RequestContentBufferValidLength, this.AsyncEndWrite, asyncStateBag);
#endif
                            pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);

                            // If the write above completed synchronously
                            //   immediately start the next read so that we loop instead of recursion.
                            // If it completed asynchronously we just return as we will deal with the results in the EndWrite
                            if (asyncResult.CompletedSynchronously && !pereq.RequestCompleted && !this.IsCompletedInternally)
                            {
                                Util.DebugInjectFault("SaveAsyncResult::AsyncRequestContentEndRead_BeforeBeginRead");
#if PORTABLELIB
                                asyncResult = BaseAsyncResult.InvokeTask(contentStream.Stream.ReadAsync, pereq.RequestContentBuffer, 0, pereq.RequestContentBuffer.Length, this.AsyncRequestContentEndRead, asyncStateBag);
#else
                                asyncResult = BaseAsyncResult.InvokeAsync(contentStream.Stream.BeginRead, pereq.RequestContentBuffer, 0, pereq.RequestContentBuffer.Length, this.AsyncRequestContentEndRead, asyncStateBag);
#endif
                                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
                            }

                            // If the read completed synchronously as well we loop to write the data to the request stream without recursion.
                            // Only loop if there's actually some data to be processed. If there's no more data then return.
                            // The request will continue inside the inner call to AsyncRequestContentEndRead (which will get 0 data
                            //   and will end up in the else branch of the big if).
                        }
                        while (asyncResult.CompletedSynchronously && !pereq.RequestCompleted && !this.IsCompletedInternally &&
                            pereq.RequestContentBufferValidLength > 0);
                    }
                }
                else
                {
                    // Done reading data (and writing them)
                    pereq.RequestContentBufferValidLength = 0;
                    pereq.RequestStream = null;
#if PORTABLELIB
                    httpRequestStream.Dispose();
#else
                    httpRequestStream.Close();
#endif
                    ODataRequestMessageWrapper requestMessage = Util.NullCheck(pereq.Request, InternalError.InvalidEndWriteRequest);
                    asyncResult = BaseAsyncResult.InvokeAsync(requestMessage.BeginGetResponse, this.AsyncEndGetResponse, asyncStateBag);
                    pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginGetResponse
                }
            }
            catch (Exception e)
            {
                if (this.HandleFailure(pereq, e))
                {
                    throw;
                }
            }
            finally
            {
                this.HandleCompleted(pereq);
            }
        }

#if PORTABLELIB
        /// <summary>Handle requestStream.WriteAsync and complete the write operation, then call BeginGetResponse.</summary>
        /// <param name="task">The task associated with the completed operation.</param>
        /// <param name="asyncState">State associated with the task.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        private void AsyncEndWrite(Task task, object asyncState)
#else
        /// <summary>handle requestStream.BeginWrite with requestStream.EndWrite then BeginGetResponse.</summary>
        /// <param name="asyncResult">async result</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        private void AsyncEndWrite(IAsyncResult asyncResult)
#endif
        {
#if PORTABLELIB
            IAsyncResult asyncResult = (IAsyncResult)task;
#endif
            Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult.IsCompleted");
#if PORTABLELIB
            AsyncStateBag asyncStateBag = asyncState as AsyncStateBag;
#else
            AsyncStateBag asyncStateBag = asyncResult.AsyncState as AsyncStateBag;
#endif
            PerRequest pereq = asyncStateBag == null ? null : asyncStateBag.PerRequest;

            try
            {
                this.CompleteCheck(pereq, InternalError.InvalidEndWriteCompleted);
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginWrite

                EqualRefCheck(this.perRequest, pereq, InternalError.InvalidEndWrite);

                ContentStream contentStream = pereq.RequestContentStream;
                Util.NullCheck(contentStream, InternalError.InvalidEndWriteStream);
                Util.NullCheck(contentStream.Stream, InternalError.InvalidEndWriteStream);
                Stream httpRequestStream = Util.NullCheck(pereq.RequestStream, InternalError.InvalidEndWriteStream);
                Util.DebugInjectFault("SaveAsyncResult::AsyncEndWrite_BeforeEndWrite");
#if PORTABLELIB
                // Ensure we surface any errors that may have occurred during the write operation
                task.Wait();
#else
                httpRequestStream.EndWrite(asyncResult);
#endif
                // If the write completed synchronously just return. The caller (AsyncRequestContentEndRead)
                //   will loop and initiate the next read.
                // If the write completed asynchronously we need to start the next read here. Note that we start the read
                //   regardless if the stream has other data to offer or not. This is to avoid dealing with the end
                //   of the read/write loop in several places. We simply issue a read which (if the stream is at the end)
                //   will return 0 bytes and we will deal with that in the AsyncRequestContentEndRead method.
                if (!asyncResult.CompletedSynchronously)
                {
                    Util.DebugInjectFault("SaveAsyncResult::AsyncEndWrite_BeforeBeginRead");
#if PORTABLELIB
                    asyncResult = BaseAsyncResult.InvokeTask(contentStream.Stream.ReadAsync, pereq.RequestContentBuffer, 0, pereq.RequestContentBuffer.Length, this.AsyncRequestContentEndRead, asyncStateBag);
#else
                    asyncResult = BaseAsyncResult.InvokeAsync(contentStream.Stream.BeginRead, pereq.RequestContentBuffer, 0, pereq.RequestContentBuffer.Length, this.AsyncRequestContentEndRead, asyncStateBag);
#endif
                    pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
                }
            }
            catch (Exception e)
            {
                if (this.HandleFailure(pereq, e))
                {
                    throw;
                }
            }
            finally
            {
                this.HandleCompleted(pereq);
            }
        }

        /// <summary>
        /// Wraps PerRequest and context reference together to save state information.
        /// Used instead of KeyValuePair in order to avoid FxCop CA908.
        /// </summary>
        protected sealed class AsyncStateBag
        {
            /// <summary>the request wrapper.</summary>
            internal readonly PerRequest PerRequest;

            /// <summary>
            /// Constructor for the state object
            /// </summary>
            /// <param name="pereq">See PerRequest field.</param>
            internal AsyncStateBag(PerRequest pereq)
            {
                Debug.Assert(pereq != null, "pereq cannot be null");
                this.PerRequest = pereq;
            }
        }

        /// <summary>wrap the full request</summary>
        protected sealed class PerRequest
        {
            /// <summary>
            /// The int equivalent for true.
            /// </summary>
            private const int True = 1;

            /// <summary>
            /// The int equivalent for false.
            /// </summary>
            private const int False = 0;

            /// <summary>
            /// did the sequence (BeginGetRequest, EndGetRequest, ... complete. 0 = In Progress, 1 = Completed, 2 = Aborted
            /// </summary>
            private int requestStatus;

            /// <summary>
            /// Buffer used when pumping data from the write stream to the request content stream
            /// </summary>
            private byte[] requestContentBuffer;

            /// <summary>True if Dispose is called.</summary>
            private bool isDisposed;

            /// <summary>Synchronize the Dispose method calls.</summary>
            private object disposeLock = new object();

            /// <summary>Did the request complete all of its steps synchronously? 1 represents true and 0 represents false.</summary>
            /// <remarks>Note that there is no overload for Interlocked.CompareExchange that takes booleans, we workaround using the overload for int.</remarks>
            private int requestCompletedSynchronously;

            /// <summary>ctor</summary>
            internal PerRequest()
            {
                this.requestCompletedSynchronously = PerRequest.True;
            }

            /// <summary>active web request</summary>
            internal ODataRequestMessageWrapper Request
            {
                get;
                set;
            }

            /// <summary>active web request stream</summary>
            internal Stream RequestStream
            {
                get;
                set;
            }

            /// <summary>content to write to request stream</summary>
            internal ContentStream RequestContentStream
            {
                get;
                set;
            }

            /// <summary>web response</summary>
            internal IODataResponseMessage ResponseMessage
            {
                get;
                set;
            }

            /// <summary>async web response stream</summary>
            internal Stream ResponseStream
            {
                get;
                set;
            }

            /// <summary>did the request complete all of its steps synchronously?</summary>
            internal bool RequestCompletedSynchronously
            {
                get
                {
                    return this.requestCompletedSynchronously == PerRequest.True;
                }
            }

            /// <summary>
            /// Short cut for testing if request has finished (either completed or aborted)
            /// </summary>
            internal bool RequestCompleted
            {
                get { return this.requestStatus != 0; }
            }

            /// <summary>
            /// Short cut for testing request status is 2 (Aborted)
            /// </summary>
            internal bool RequestAborted
            {
                get { return this.requestStatus == 2; }
            }

            /// <summary>
            /// Buffer used when pumping data from the write stream to the request content stream
            /// </summary>
            internal byte[] RequestContentBuffer
            {
                get
                {
                    if (this.requestContentBuffer == null)
                    {
                        this.requestContentBuffer = new byte[WebUtil.DefaultBufferSizeForStreamCopy];
                    }

                    return this.requestContentBuffer;
                }
            }

            /// <summary>
            /// The length of the valid content in the RequestContentBuffer
            /// Once the data is read from the request content stream into the RequestContent buffer
            /// this length is set to the amount of data read.
            /// When the data is written into the request stream it is set back to 0.
            /// </summary>
            internal int RequestContentBufferValidLength
            {
                get;
                set;
            }

            /// <summary>
            /// Sets the RequestCompletedSynchronously property.
            /// </summary>
            /// <param name="completedSynchronously">true if the async operation was completed synchronously, false otherwise.</param>
            internal void SetRequestCompletedSynchronously(bool completedSynchronously)
            {
                // This is equivalent to "RequestCompletedSynchronously &= completedSynchronously". The &= operator is a nonatomic operation involving a read and a write operations.
                Interlocked.CompareExchange(ref this.requestCompletedSynchronously, completedSynchronously ? PerRequest.True : PerRequest.False, PerRequest.True);
                Debug.Assert(completedSynchronously || this.requestCompletedSynchronously == PerRequest.False, "this.requestCompletedSynchronously must be false if the completedSynchronously argument is false.");
            }

            /// <summary>
            /// Change the request status to completed
            /// </summary>
            internal void SetComplete()
            {
                System.Threading.Interlocked.CompareExchange(ref this.requestStatus, 1, 0);
            }

            /// <summary>
            /// Change the request status to aborted
            /// </summary>
            internal void SetAborted()
            {
                System.Threading.Interlocked.Exchange(ref this.requestStatus, 2);
            }

            /// <summary>
            /// dispose of the request object
            /// </summary>
            internal void Dispose()
            {
                if (this.isDisposed)
                {
                    return;
                }

                // The dispose method is called by BaseSaveResult.HandleCompleted() and SaveResult.FinishCurrentChange().
                // These methods can be run by multiple threads.  We need to protect the Dispose method with a critical section
                // or else we will get sporadic null ref exceptions.
                lock (this.disposeLock)
                {
                    if (!this.isDisposed)
                    {
                        this.isDisposed = true;

                        if (null != this.ResponseStream)
                        {
                            this.ResponseStream.Dispose();
                            this.ResponseStream = null;
                        }

                        if (null != this.RequestContentStream)
                        {
                            if (this.RequestContentStream.Stream != null && this.RequestContentStream.IsKnownMemoryStream)
                            {
                                this.RequestContentStream.Stream.Dispose();
                            }

                            // We must not dispose the stream which came from outside
                            //   the disposing/closing of that stream depends on parameter passed to us and is dealt with
                            //   at the end of SaveChanges process.
                            this.RequestContentStream = null;
                        }

                        if (null != this.RequestStream)
                        {
                            try
                            {
                                Util.DebugInjectFault("PerRequest::Dispose_BeforeRequestStreamDisposed");
                                this.RequestStream.Dispose();
                                this.RequestStream = null;
                            }
                            catch (Exception e)
                            {
                                // if the request is aborted, then the connect stream
                                // cannot be disposed - since not all bytes are written to it yet
                                // In this case, we eat the exception
                                // Otherwise, keep throwing, always throw if the exception is not catchable.
                                if (!this.RequestAborted || !CommonUtil.IsCatchableExceptionType(e))
                                {
                                    throw;
                                }

                                // Call Injector to report the exception so test code can verify it is thrown
                                Util.DebugInjectFault("PerRequest::Dispose_WebExceptionThrown");
                            }
                        }

                        WebUtil.DisposeMessage(this.ResponseMessage);
                        this.Request = null;
                        this.SetComplete();
                    }
                }
            }
        }
    }
}
