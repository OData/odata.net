//---------------------------------------------------------------------
// <copyright file="TestTaskUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using System.Threading.Tasks;
    using Microsoft.Test.OData.Utils.Common;

    /// <summary>
    /// Class with utility methods for working with and implementing Task based APIs
    /// </summary>
    public static class TestTaskUtils
    {
        /// <summary>
        /// Already completed task.
        /// </summary>
        private static Task completedTask;

        /// <summary>
        /// Returns already completed task instance.
        /// </summary>
        public static Task GetCompletedTask()
        {
            // Note that in case of two threads competing here we would create two completed tasks, but only one 
            // will be stored in the static variable. In any case, they are identical for all other purposes, 
            // so it doesn't matter which one wins
            if (completedTask == null)
            {
                // Create a TaskCompletionSource - since there's no non-generic version use a dummy one
                // and then cast to the non-generic version.
                TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetResult(null);
                completedTask = taskCompletionSource.Task;
            }

            return completedTask;
        }

        /// <summary>
        /// Returns already completed task instance.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="value">The value of the result.</param>
        /// <returns>Already completed task.</returns>
        public static Task<T> GetCompletedTask<T>(T value)
        {
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetResult(value);
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Waits for a task to finish and returns its result.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task">The task to wait for.</param>
        /// <returns>The result of the task.</returns>
        public static T WaitForResult<T>(this Task<T> task)
        {
            ExceptionUtilities.CheckArgumentNotNull(task, "task");
            task.Wait();
            return task.Result;
        }
    }
}
