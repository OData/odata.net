//---------------------------------------------------------------------
// <copyright file="TaskExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for working with Tasks
    /// </summary>
    public static class TaskExtensionMethods
    {
        /// <summary>
        /// Adapts a Task to the APM pattern.
        /// </summary>
        /// <typeparam name="TResult">The result type of the task.</typeparam>
        /// <param name="task">The Task to adapt.</param>
        /// <param name="callback">The APM callback to inject.</param>
        /// <param name="state">The APM state to inject.</param>
        /// <returns>A Task that has been adapted to use the APM pattern.</returns>
        /// <remarks>Taken from http://blogs.msdn.com/b/pfxteam/archive/2011/06/27/10179452.aspx </remarks>
        public static Task<TResult> ToApm<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(state);

            task.ContinueWith(
                delegate
                {
                    if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception.InnerExceptions);
                    }
                    else if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(task.Result);
                    }

                    if (callback != null)
                    {
                        callback(tcs.Task);
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

            return tcs.Task;
        }
    }
}
