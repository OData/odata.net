//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Remembers the result of evaluating an expensive function so that subsequent
    /// evaluations are faster. Thread-safe.
    /// </summary>
    /// <typeparam name="TArg">Type of the argument to the function.</typeparam>
    /// <typeparam name="TResult">Type of the function result.</typeparam>
    internal sealed class Memoizer<TArg, TResult>
    {
        private readonly Func<TArg, TResult> function;
        private readonly Dictionary<TArg, Result> resultCache;
        private readonly ReaderWriterLockSlim slimLock;

        /// <summary>
        /// Constructs the memoizer.
        /// </summary>
        /// <param name="function">Required. Function whose values are being cached.</param>
        /// <param name="argComparer">Optional. Comparer used to determine if two functions arguments are the same.</param>
        internal Memoizer(Func<TArg, TResult> function, IEqualityComparer<TArg> argComparer)
        {
            Debug.Assert(function != null, "function != null");

            this.function = function;
            this.resultCache = new Dictionary<TArg, Result>(argComparer);

            this.slimLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Evaluates the wrapped function for the given argument. If the function has already
        /// been evaluated for the given argument, returns cached value. Otherwise, the value
        /// is computed and returned.
        /// </summary>
        /// <param name="arg">Function argument.</param>
        /// <returns>Function result.</returns>
        internal TResult Evaluate(TArg arg)
        {
            Result result;
            bool hasResult;

            // check to see if a result has already been computed
            this.slimLock.EnterReadLock();

            try
            {
                hasResult = this.resultCache.TryGetValue(arg, out result);
            }
            finally
            {
                this.slimLock.ExitReadLock();
            }

            if (!hasResult)
            {
                // compute the new value
                this.slimLock.EnterWriteLock();

                try
                {
                    // see if the value has been computed in the interim
                    if (!this.resultCache.TryGetValue(arg, out result))
                    {
                        result = new Result(() => this.function(arg));
                        this.resultCache.Add(arg, result);
                    }
                }
                finally
                {
                    this.slimLock.ExitWriteLock();
                }
            }

            // note: you need to release the global cache lock before (potentially) acquiring
            // a result lock in result.GetValue()
            return result.GetValue();
        }

        /// <summary>
        /// Encapsulates a 'deferred' result. The result is constructed with a delegate (must not 
        /// be null) and when the user requests a value the delegate is invoked and stored.
        /// </summary>
        private class Result
        {
            private TResult value;
            private Func<TResult> createValueDelegate;

            internal Result(Func<TResult> createValueDelegate)
            {
                Debug.Assert(null != createValueDelegate, "delegate must be given");
                this.createValueDelegate = createValueDelegate;
            }

            internal TResult GetValue()
            {
                if (null == this.createValueDelegate)
                {
                    // if the delegate has been cleared, it means we have already computed the value
                    return this.value;
                }

                // lock the entry while computing the value so that two threads
                // don't simultaneously do the work
                lock (this)
                {
                    if (null == this.createValueDelegate)
                    {
                        // between our initial check and our acquisition of the lock, some other
                        // thread may have computed the value
                        return this.value;
                    }

                    this.value = this.createValueDelegate();

                    // ensure createValueDelegate (and its closure) is garbage collected, and set to null
                    // to indicate that the value has been computed
                    this.createValueDelegate = null;
                    return this.value;
                }
            }
        }

#if !ORCAS
        /// <summary>Read-writer lock, implemented over a Monitor.</summary>
        private sealed class ReaderWriterLockSlim
        {
            /// <summary>Single object on which to lock.</summary>
            private object readerWriterLock = new object();

            /// <summary>Enters a reader lock. Writers will also be blocked.</summary>
            internal void EnterReadLock()
            {
                Monitor.Enter(this.readerWriterLock);
            }

            /// <summary>Enters a writer lock. Readers will also be blocked.</summary>
            internal void EnterWriteLock()
            {
                Monitor.Enter(this.readerWriterLock);
            }

            /// <summary>Exits a reader lock.</summary>
            internal void ExitReadLock()
            {
                Monitor.Exit(this.readerWriterLock);
            }

            /// <summary>Exits a writer lock.</summary>
            internal void ExitWriteLock()
            {
                Monitor.Exit(this.readerWriterLock);
            }
        }
#endif
    }
}
