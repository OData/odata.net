//---------------------------------------------------------------------
// <copyright file="TaskUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.OData.Edm.Helpers
{
    internal class TaskUtils
    {
        #region Completed task
        /// <summary>
        /// Already completed task.
        /// </summary>
        private static Task _completedTask;

        // <summary>
        /// Returns already completed task instance.
        /// </summary>
        public static Task CompletedTask
        {
            get
            {
#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
                _completedTask = Task.CompletedTask;
#else
                // Note that in case of two threads competing here we would create two completed tasks, but only one
                // will be stored in the static variable. In any case, they are identical for all other purposes,
                // so it doesn't matter which one wins
                if (_completedTask == null)
                {
                    // Create a TaskCompletionSource - since there's no non-generic version use a dummy one
                    // and then cast to the non-generic version.
                    TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetResult(null);
                    _completedTask = taskCompletionSource.Task;
                }
#endif
                return _completedTask;
            }
        }
        #endregion

        #region Faulted task
        /// <summary>
        /// Returns an already completed task instance with the specified error.
        /// </summary>
        /// <param name="exception">The exception of the faulted result.</param>
        /// <returns>An already completed task with the specified exception.</returns>
        internal static Task GetFaultedTask(Exception exception)
        {
            // Since there's no non-generic version use a dummy object return value and cast to non-generic version.
            return GetFaultedTask<object>(exception);
        }

        /// <summary>
        /// Returns an already completed task instance with the specified error.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="exception">The exception of the faulted result.</param>
        /// <returns>An already completed task with the specified exception.</returns>
        internal static Task<T> GetFaultedTask<T>(Exception exception)
        {
#if NETSTANDARD2_0 || NETCOREAPP3_1_OR_GREATER
            return Task.FromException<T>(exception);
#else
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetException(exception);
            return taskCompletionSource.Task;
#endif
        }
        #endregion
    }
}
