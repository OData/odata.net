//---------------------------------------------------------------------
// <copyright file="TaskUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Microsoft.OData.Core;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for working with and implementing Task based APIs
    /// </summary>
    internal static class TaskUtils
    {
        /// <summary>
        /// Executes the specified operation if the antecedent task completes successfully.
        /// The operation runs synchronously after task completion and exceptions are propagated.
        /// </summary>
        /// <typeparam name="TResult">The result type of the antecedent task.</typeparam>
        /// <param name="antecedent">The source task to observe.</param>
        /// <param name="operation">An action to perform using the result of the task.</param>
        /// <returns>A task that completes with the same result as <paramref name="antecedent"/>.</returns>
        internal static Task<TResult> ThenOnSuccess<TResult>(this Task<TResult> antecedent, Action<TResult> operation)
        {
            if (antecedent == null)
            {
                throw new ArgumentNullException(nameof(antecedent));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            antecedent.ContinueWith(task =>
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        try
                        {
                            operation(task.Result);
                            // Preserve result
                            taskCompletionSource.TrySetResult(task.Result);
                        }
                        catch (Exception ex) when (ExceptionUtils.IsCatchableExceptionType(ex))
                        {
                            TrySetException(taskCompletionSource, ex);
                        }

                        break;

                    case TaskStatus.Faulted:
                        TrySetException(taskCompletionSource, task.Exception);

                        break;

                    case TaskStatus.Canceled:
                        taskCompletionSource.TrySetCanceled();

                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Executes the specified operation if the antecedent task completes successfully.
        /// The operation receives the completed task instance and may inspect its state or result.
        /// </summary>
        /// <typeparam name="TResult">The result type of the antecedent task.</typeparam>
        /// <param name="antecedent">The source task.</param>
        /// <param name="operation">An action to perform when the task completes successfully.</param>
        /// <returns>A task that completes when the operation has finished executing.</returns>
        internal static Task ThenOnSuccess<TResult>(this Task<TResult> antecedent, Action<Task<TResult>> operation)
        {
            if (antecedent == null)
            {
                throw new ArgumentNullException(nameof(antecedent));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            antecedent.ContinueWith(task =>
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        try
                        {
                            operation(task);
                            taskCompletionSource.TrySetResult(null);
                        }
                        catch (Exception ex) when (ExceptionUtils.IsCatchableExceptionType(ex))
                        {
                            TrySetException(taskCompletionSource, ex);
                        }

                        break;

                    case TaskStatus.Faulted:
                        TrySetException(taskCompletionSource, task.Exception);

                        break;

                    case TaskStatus.Canceled:
                        taskCompletionSource.TrySetCanceled();

                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Maps a successfully completed task to a new result using the specified mapping function.
        /// The mapping is not invoked if the antecedent faults or is canceled.
        /// </summary>
        /// <typeparam name="TContinuationResult">The result type of the continuation task.</typeparam>
        /// <param name="antecedent">The source task.</param>
        /// <param name="map">A function that produces a result from the completed task.</param>
        /// <returns>A task representing the mapped result or propagating failure from the antecedent.</returns>
        internal static Task<TContinuationResult> ThenMapOnSuccess<TContinuationResult>(this Task antecedent, Func<Task, TContinuationResult> map)
        {
            if (antecedent == null)
            {
                throw new ArgumentNullException(nameof(antecedent));
            }

            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            TaskCompletionSource<TContinuationResult> taskCompletionSource = new TaskCompletionSource<TContinuationResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            antecedent.ContinueWith(task =>
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        try
                        {
                            taskCompletionSource.TrySetResult(map(task));
                        }
                        catch (Exception ex) when (ExceptionUtils.IsCatchableExceptionType(ex))
                        {
                            TrySetException(taskCompletionSource, ex);
                        }

                        break;

                    case TaskStatus.Faulted:
                        TrySetException(taskCompletionSource, task.Exception);

                        break;

                    case TaskStatus.Canceled:
                        taskCompletionSource.TrySetCanceled();

                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Asynchronously maps a successfully completed task to a new result task.
        /// The mapping delegate is invoked only if the antecedent task completes successfully.
        /// </summary>
        /// <typeparam name="TResult">The result type of the antecedent task.</typeparam>
        /// <typeparam name="TContinuationResult">The result type of the continuation task.</typeparam>
        /// <param name="antecedent">The source task.</param>
        /// <param name="map">A function that produces a result from the completed task.</param>
        /// <returns>A task that represents the asynchronous mapped result or propagating failure from the antecedent.</returns>
        internal static Task<TContinuationResult> ThenMapOnSuccessAsync<TResult, TContinuationResult>(this Task<TResult> antecedent, Func<Task<TResult>, Task<TContinuationResult>> map)
        {
            if (antecedent == null)
            {
                throw new ArgumentNullException(nameof(antecedent));
            }

            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            // We first produce a Task<Task<TContinuationResult>> and then unwrap it to get Task<TContinuationResult>
            TaskCompletionSource<Task<TContinuationResult>> taskCompletionSource = new TaskCompletionSource<Task<TContinuationResult>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            antecedent.ContinueWith(task =>
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        try
                        {
                            // Invoke continuation only on success; capture returned Task<TContinuationResult>
                            Task<TContinuationResult> continuationTask = map(task);
                            
                            // .NET 8.0+ throws a TaskCanceledException if Result is set to null and Unwrap called later
                            // From a developer's standpoint, getting a TaskCanceledException here is semantically wrong
                            // where there's no explicit cancellation involved in the operation.
                            // To maintain consistent behavior, we explicitly check for null and throw an InvalidOperationException.
                            if (continuationTask == null)
                            {
                                taskCompletionSource.TrySetException(
                                    new InvalidOperationException(SRResources.TaskUtils_NullContinuationTask));
                            }
                            else
                            {
                                taskCompletionSource.TrySetResult(continuationTask);
                            }
                        }
                        catch (Exception ex) when (ExceptionUtils.IsCatchableExceptionType(ex))
                        {
                            TrySetException(taskCompletionSource, ex);
                        }

                        break;

                    case TaskStatus.Faulted:
                        TrySetException(taskCompletionSource, task.Exception);

                        break;

                    case TaskStatus.Canceled:
                        taskCompletionSource.TrySetCanceled();

                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task.Unwrap();
        }

        /// <summary>
        /// Executes the specified operation when the antecedent task completes, regardless of success,
        /// fault, or cancellation.
        /// Exceptions from both the antecedent and the operation are aggregated.
        /// </summary>
        /// <param name="antecedent">The source task to observe.</param>
        /// <param name="operation">An action to execute after task completion.</param>
        /// <returns>A task representing completion of both the antecedent and the operation.</returns>
        internal static Task ThenAlways(this Task antecedent, Action<Task> operation)
        {
            if (antecedent == null)
            {
                throw new ArgumentNullException(nameof(antecedent));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            antecedent.ContinueWith(task =>
            {
                Exception operationException = null;

                try
                {
                    operation(task);
                }
                catch (Exception ex) when (ExceptionUtils.IsCatchableExceptionType(ex))
                {
                    operationException = ex;
                }

                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        if (operationException != null)
                        {
                            TrySetException(taskCompletionSource, operationException);
                        }
                        else
                        {
                            taskCompletionSource.TrySetResult(null);
                        }

                        break;

                    case TaskStatus.Faulted:
                        if (operationException != null)
                        {
                            taskCompletionSource.TrySetException(MergeExceptions(task.Exception, operationException));
                        }
                        else
                        {
                            TrySetException(taskCompletionSource, task.Exception);
                        }

                        break;

                    case TaskStatus.Canceled:
                        // Cancellation behavior: if the antecedent was canceled and the (cleanup) operation throws,
                        // this continuation reports the exception (i.e., Task faults) rather than preserving cancellation.
                        // This matches legacy semantics. Changing it would be a breaking behavioral change.
                        // TODO (major release): Consider preserving cancellation even when operation throws; document rationale.
                        if (operationException != null)
                        {
                            TrySetException(taskCompletionSource, operationException);
                        }
                        else
                        {
                            taskCompletionSource.TrySetCanceled();
                        }

                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Executes the specified operation when the antecedent task completes, regardless of success, fault, or cancellation.
        /// Exceptions from both the antecedent and the operation are aggregated.
        /// </summary>
        /// <typeparam name="TResult">The result type of the antecedent task.</typeparam>
        /// <param name="antecedent">The source task to observe.</param>
        /// <param name="operation">An action to execute upon completion of the task.</param>
        /// <returns>A task representing completion of both operations, preserving the original result if successful.</returns>
        internal static Task<TResult> ThenAlways<TResult>(this Task<TResult> antecedent, Action<Task<TResult>> operation)
        {
            if (antecedent == null)
            {
                throw new ArgumentNullException(nameof(antecedent));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            antecedent.ContinueWith(task =>
            {
                Exception operationException = null;

                try
                {
                    operation(task);
                }
                catch (Exception ex) when (ExceptionUtils.IsCatchableExceptionType(ex))
                {
                    operationException = ex;
                }

                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        if (operationException != null)
                        {
                            TrySetException(taskCompletionSource, operationException);
                        }
                        else
                        {
                            taskCompletionSource.TrySetResult(task.Result);
                        }

                        break;

                    case TaskStatus.Faulted:
                        if (operationException != null)
                        {
                            taskCompletionSource.TrySetException(MergeExceptions(task.Exception, operationException));
                        }
                        else
                        {
                            TrySetException(taskCompletionSource, task.Exception);
                        }

                        break;

                    case TaskStatus.Canceled:
                        // Cancellation behavior: if the antecedent was canceled and the (cleanup) operation throws,
                        // this continuation reports the exception (i.e., Task faults) rather than preserving cancellation.
                        // This matches legacy semantics. Changing it would be a breaking behavioral change.
                        // TODO (major release): Consider preserving cancellation even when operation throws; document rationale.
                        if (operationException != null)
                        {
                            TrySetException(taskCompletionSource, operationException);
                        }
                        else
                        {
                            taskCompletionSource.TrySetCanceled();
                        }

                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Attempts to fault <paramref name="taskCompletionSource"/> with the supplied <paramref name="exception"/>,
        /// performing minimal normalization of <see cref="AggregateException"/> instances.
        /// </summary>
        /// <typeparam name="TResult">The result type of the associated task.</typeparam>
        /// <param name="taskCompletionSource">The task completion source to transition to the <see cref="TaskStatus.Faulted"/> state.</param>
        /// <param name="exception">The exception to propagate.</param>
        /// <remarks>
        /// Normalization rules:
        /// 1. If the exception is an <see cref="AggregateException"/> with exactly one inner exception, that inner exception is used directly.
        ///    This removes the synthetic wrapper created by the Task infrastructure around a single exception.
        /// 2. If the <see cref="AggregateException"/> has multiple inner exceptions, those inner exceptions are propagated as-is by
        ///    calling <c>TrySetException(IEnumerable&lt;Exception&gt;)</c>.
        /// 3. Non-aggregate exceptions are passed through unchanged.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TrySetException<TResult>(TaskCompletionSource<TResult> taskCompletionSource, Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
                if (innerExceptions.Count == 1)
                {
                    // Preserve single inner exception without wrapping
                    taskCompletionSource.TrySetException(innerExceptions[0]);
                }
                else
                {
                    // Propagate as one AggregateException of leaf exceptions
                    taskCompletionSource.TrySetException(innerExceptions);
                }
            }
            else
            {
                taskCompletionSource.TrySetException(exception);
            }
        }

        /// <summary>
        /// Attempts to fault <paramref name="taskCompletionSource"/> with <paramref name="exception"/>,
        /// applying the same <see cref="AggregateException"/> normalization rules as the generic overload.
        /// </summary>
        /// <param name="taskCompletionSource">The non-generic task completion source to fault.</param>
        /// <param name="exception">The exception to propagate.</param>
        /// <remarks>
        /// See the generic overload for detailed behavior.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TrySetException(TaskCompletionSource<object> taskCompletionSource, Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
                if (innerExceptions.Count == 1)
                {
                    taskCompletionSource.TrySetException(innerExceptions[0]);
                }
                else
                {
                    taskCompletionSource.TrySetException(innerExceptions);
                }
            }
            else
            {
                taskCompletionSource.TrySetException(exception);
            }
        }

        /// <summary>
        /// Creates a new <see cref="AggregateException"/> combining the antecedent task's faults with an operation exception.
        /// </summary>
        /// <param name="antecedentException">Fault from the antecedent task.</param>
        /// <param name="operationException">Exception thrown by the continuation.</param>
        /// <returns>
        /// An <see cref="AggregateException"/> containing the single unwrapped inner exception plus
        /// <paramref name="operationException"/> if the antecedent was a synthetic single wrapper;
        /// otherwise all antecedent inner exceptions followed by <paramref name="operationException"/>.
        /// </returns>
        private static Exception MergeExceptions(AggregateException antecedentException, Exception operationException)
        {
            // Antecedent exception could include user-thrown aggregate exceptions
            ReadOnlyCollection<Exception> antecedentInnerExceptions = antecedentException.InnerExceptions;

            // Task infrastructure always wraps thrown exceptions in an AggregateException; single non-aggregate
            // inner exception is synthetic and safe to unwrap
            if (antecedentInnerExceptions.Count == 1 && !(antecedentInnerExceptions[0] is AggregateException))
            {
                // Synthetic single wrapper aggregate exception + operation exception
                return new AggregateException(antecedentInnerExceptions[0], operationException);
            }

            // Combine antecedent inner exceptions plus operation exception
            Exception[] combinedExceptions = new Exception[antecedentInnerExceptions.Count + 1];
            antecedentInnerExceptions.CopyTo(combinedExceptions, 0);
            combinedExceptions[antecedentInnerExceptions.Count] = operationException;            

            return new AggregateException(combinedExceptions);
        }
    }
}
