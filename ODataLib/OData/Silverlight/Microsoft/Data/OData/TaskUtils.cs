//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#if ODATALIB_ASYNC || ASTORIA_CLIENT
#if ODATALIB
namespace Microsoft.Data.OData
#else
namespace System.Data.Services.Client
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
#if ASTORIA_CLIENT
    using ExceptionUtils = CommonUtil;
#endif
    #endregion Namespaces
    
    /// <summary>
    /// Class with utility methods for working with and implementing Task based APIs
    /// </summary>
    internal static class TaskUtils
    {
        #region Completed task
        /// <summary>
        /// Already completed task.
        /// </summary>
        private static Task completedTask;

        /// <summary>
        /// Returns already completed task instance.
        /// </summary>
        internal static Task CompletedTask
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

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
        }

        /// <summary>
        /// Returns an already completed task instance with the specified result.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="value">The value of the result.</param>
        /// <returns>An already completed task with the specified result.</returns>
        internal static Task<T> GetCompletedTask<T>(T value)
        {
            DebugUtils.CheckNoExternalCallers();

            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetResult(value);
            return taskCompletionSource.Task;
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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();

            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetException(exception);
            return taskCompletionSource.Task;
        }
        #endregion

        #region GetTaskForSynchronousOperation
        /// <summary>
        /// Returns an already completed task for the specified synchronous operation.
        /// </summary>
        /// <param name="synchronousOperation">The synchronous operation to perform.</param>
        /// <returns>An already completed task. If the <paramref name="synchronousOperation"/> succeeded this will be a successfully completed task,
        /// otherwise it will be a faulted task holding the exception thrown.</returns>
        /// <remarks>The advantage of this method over CompletedTask property is that if the <paramref name="synchronousOperation"/> fails
        /// this method returns a faulted task, instead of throwing exception.</remarks>
        internal static Task GetTaskForSynchronousOperation(Action synchronousOperation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(synchronousOperation != null, "synchronousOperation != null");

            try
            {
                synchronousOperation();
                return TaskUtils.CompletedTask;
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                return TaskUtils.GetFaultedTask(e);
            }
        }

        /// <summary>
        /// Returns an already completed task for the specified synchronous operation.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the operation. This MUST NOT be a Task type.</typeparam>
        /// <param name="synchronousOperation">The synchronous operation to perform.</param>
        /// <returns>An already completed task. If the <paramref name="synchronousOperation"/> succeeded this will be a successfully completed task,
        /// otherwise it will be a faulted task holding the exception thrown.</returns>
        /// <remarks>The advantage of this method over GetCompletedTask property is that if the <paramref name="synchronousOperation"/> fails
        /// this method returns a faulted task, instead of throwing exception.</remarks>
        internal static Task<T> GetTaskForSynchronousOperation<T>(Func<T> synchronousOperation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(synchronousOperation != null, "synchronousOperation != null");
            Debug.Assert(!(typeof(Task).IsAssignableFrom(typeof(T))), "This method doesn't support operations returning Task instances.");

            try
            {
                T result = synchronousOperation();
                return TaskUtils.GetCompletedTask<T>(result);
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                return TaskUtils.GetFaultedTask<T>(e);
            }
        }

        /// <summary>
        /// Returns an already completed task for the specified synchronous operation which returns a task.
        /// </summary>
        /// <param name="synchronousOperation">The synchronous operation to perform.</param>
        /// <returns>The task returned by the <paramref name="synchronousOperation"/> or a faulted task if the operation failed.</returns>
        /// <remarks>The advantage of this method over direct call is that if the <paramref name="synchronousOperation"/> fails
        /// this method returns a faulted task, instead of throwing exception.</remarks>
        internal static Task GetTaskForSynchronousOperationReturningTask(Func<Task> synchronousOperation)
        {
            DebugUtils.CheckNoExternalCallers();

            try
            {
                return synchronousOperation();
            }
            catch (Exception exception)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(exception))
                {
                    throw;
                }

                return TaskUtils.GetFaultedTask(exception);
            }
        }
        #endregion

        #region FollowOnSuccessWith
        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task succeeded.
        /// </summary>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> succeeded.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.</remarks>
        internal static Task FollowOnSuccessWith(this Task antecedentTask, Action<Task> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            return FollowOnSuccessWithImplementation<object>(antecedentTask, t => { operation(t); return null; });
        }

        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task succeeded.
        /// </summary>
        /// <typeparam name="TFollowupTaskResult">The result type of the operation. This MUST NOT be a Task or a type derived from Task.</typeparam>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> succeeded.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>
        /// This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.
        /// This method doesn't support operations which return another Task instance, to use that call FollowOnSuccessWithTask instead.
        /// </remarks>
        internal static Task<TFollowupTaskResult> FollowOnSuccessWith<TFollowupTaskResult>(this Task antecedentTask, Func<Task, TFollowupTaskResult> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            return FollowOnSuccessWithImplementation(antecedentTask, operation);
        }

        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task succeeded.
        /// </summary>
        /// <typeparam name="TAntecedentTaskResult">The result type of the antecedent task.</typeparam>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> succeeded.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.</remarks>
        internal static Task FollowOnSuccessWith<TAntecedentTaskResult>(this Task<TAntecedentTaskResult> antecedentTask, Action<Task<TAntecedentTaskResult>> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            return FollowOnSuccessWithImplementation<object>(antecedentTask, t => { operation((Task<TAntecedentTaskResult>)t); return null; });
        }

        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task succeeded.
        /// </summary>
        /// <typeparam name="TAntecedentTaskResult">The result type of the antecedent task.</typeparam>
        /// <typeparam name="TFollowupTaskResult">The result type of the operation. This MUST NOT be a Task or a type derived from Task.</typeparam>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> succeeded.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>
        /// This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.
        /// This method doesn't support operations which return another Task instance, to use that call FollowOnSuccessWithTask instead.
        /// </remarks>
        internal static Task<TFollowupTaskResult> FollowOnSuccessWith<TAntecedentTaskResult, TFollowupTaskResult>(this Task<TAntecedentTaskResult> antecedentTask, Func<Task<TAntecedentTaskResult>, TFollowupTaskResult> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            return FollowOnSuccessWithImplementation(antecedentTask, t => operation((Task<TAntecedentTaskResult>)t));
        }
        #endregion

        #region FollowOnSuccessWithTask
        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task succeeded.
        /// </summary>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> succeeded.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>
        /// This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.
        /// This method handles operation which returns another task. The method will unwrap and return a task which finishes when both
        /// the antecedent task, the operation as well as the task returned by that operation finished.
        /// </remarks>
        internal static Task FollowOnSuccessWithTask(this Task antecedentTask, Func<Task, Task> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(antecedentTask != null, "antecedentTask != null");
            Debug.Assert(operation != null, "operation != null");

            TaskCompletionSource<Task> taskCompletionSource = new TaskCompletionSource<Task>();
            antecedentTask.ContinueWith(
                (taskToContinueOn) => FollowOnSuccessWithContinuation(taskToContinueOn, taskCompletionSource, operation),
                TaskContinuationOptions.ExecuteSynchronously);
            return taskCompletionSource.Task.Unwrap();
        }

        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task succeeded.
        /// </summary>
        /// <typeparam name="TAntecedentTaskResult">The result type of the antecedent task.</typeparam>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> succeeded.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>
        /// This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.
        /// This method handles operation which returns another task. The method will unwrap and return a task which finishes when both
        /// the antecedent task, the operation as well as the task returned by that operation finished.
        /// </remarks>
        internal static Task FollowOnSuccessWithTask<TAntecedentTaskResult>(this Task<TAntecedentTaskResult> antecedentTask, Func<Task<TAntecedentTaskResult>, Task> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(antecedentTask != null, "antecedentTask != null");
            Debug.Assert(operation != null, "operation != null");

            TaskCompletionSource<Task> taskCompletionSource = new TaskCompletionSource<Task>();
            antecedentTask.ContinueWith(
                (taskToContinueOn) => FollowOnSuccessWithContinuation(taskToContinueOn, taskCompletionSource, (taskForOperation) => operation((Task<TAntecedentTaskResult>)taskForOperation)),
                TaskContinuationOptions.ExecuteSynchronously);
            return taskCompletionSource.Task.Unwrap();
        }

        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task succeeded.
        /// </summary>
        /// <typeparam name="TAntecedentTaskResult">The result type of the antecedent task.</typeparam>
        /// <typeparam name="TFollowupTaskResult">The result type of the operation. This MUST NOT be a Task or a type derived from Task.</typeparam>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> succeeded.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>
        /// This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.
        /// This method handles operation which returns another task. The method will unwrap and return a task which finishes when both
        /// the antecedent task, the operation as well as the task returned by that operation finished.
        /// </remarks>
        internal static Task<TFollowupTaskResult> FollowOnSuccessWithTask<TAntecedentTaskResult, TFollowupTaskResult>(this Task<TAntecedentTaskResult> antecedentTask, Func<Task<TAntecedentTaskResult>, Task<TFollowupTaskResult>> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(antecedentTask != null, "antecedentTask != null");
            Debug.Assert(operation != null, "operation != null");

            TaskCompletionSource<Task<TFollowupTaskResult>> taskCompletionSource = new TaskCompletionSource<Task<TFollowupTaskResult>>();
            antecedentTask.ContinueWith(
                (taskToContinueOn) => FollowOnSuccessWithContinuation(taskToContinueOn, taskCompletionSource, (taskForOperation) => operation((Task<TAntecedentTaskResult>)taskForOperation)),
                TaskContinuationOptions.ExecuteSynchronously);
            return taskCompletionSource.Task.Unwrap();
        }
        #endregion

        #region FollowOnFaultWith
        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task faulted.
        /// </summary>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> faulted.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.</remarks>
        internal static Task FollowOnFaultWith(this Task antecedentTask, Action<Task> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            return FollowOnFaultWithImplementation<object>(antecedentTask, t => null, operation);
        }

        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will only be invoked if the antecendent task faulted.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the task.</typeparam>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute if the <paramref name="antecedentTask"/> faulted.</param>
        /// <returns>A new task which represents the antecedent task followed by a conditional invoke to the operation.</returns>
        /// <remarks>This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.</remarks>
        internal static Task<TResult> FollowOnFaultWith<TResult>(this Task<TResult> antecedentTask, Action<Task<TResult>> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            return FollowOnFaultWithImplementation(antecedentTask, t => ((Task<TResult>)t).Result, t => operation((Task<TResult>)t));
        }
        #endregion

        #region FollowAlwaysWith
        /// <summary>
        /// Returns a new task which will consist of the <paramref name="antecedentTask"/> followed by a call to the <paramref name="operation"/>
        /// which will get called no matter what the result of the antecedent task was.
        /// </summary>
        /// <param name="antecedentTask">The task to "append" the operation to.</param>
        /// <param name="operation">The operation to execute after the <paramref name="antecedentTask"/> finished.</param>
        /// <returns>A new task which represents the antecedent task followed by an invoke to the operation.</returns>
        /// <remarks>
        /// This method unlike ContinueWith will return a task which will fail if the antecedent task fails, thus it propagates failures.
        /// Note that the operation may not return any value, since the original result of the antecedent task will be used always.
        /// Also if the operation fails, the resulting task fails. If both tasks fail, the antecedent task failure is reported only.</remarks>
        internal static Task FollowAlwaysWith(this Task antecedentTask, Action<Task> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(antecedentTask != null, "antecedentTask != null");
            Debug.Assert(operation != null, "operation != null");

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            antecedentTask.ContinueWith(
                t =>
                {
                    Exception operationException = null;
                    try
                    {
                        operation(t);
                    }
                    catch (Exception exception)
                    {
                        if (!ExceptionUtils.IsCatchableExceptionType(exception))
                        {
                            throw;
                        }

                        operationException = exception;
                    }

                    switch (t.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            if (operationException != null)
                            {
                                taskCompletionSource.TrySetException(operationException);
                            }
                            else
                            {
                                taskCompletionSource.TrySetResult(null);
                            }

                            break;
                        case TaskStatus.Faulted:
                            // If the parent task failed, use that exception instead of the follow up operation's exception (if there was one).
                            taskCompletionSource.TrySetException(t.Exception);
                            break;
                        case TaskStatus.Canceled:
                            if (operationException != null)
                            {
                                taskCompletionSource.TrySetException(operationException);
                            }
                            else
                            {
                                taskCompletionSource.TrySetCanceled();
                            }

                            break;
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously)

                // This is only for the body of delegate in the .ContinueWith, the antecedent task failures are handles by the body itself.
                .IgnoreExceptions();
            return taskCompletionSource.Task;
        }
        #endregion

        #region Task.IgnoreExceptions - copied from Samples for Parallel Programming
        /// <summary>Suppresses default exception handling of a Task that would otherwise reraise the exception on the finalizer thread.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Need to access t.Exception to invoke the getter which will mark the Task to not throw the exception.")]
        internal static Task IgnoreExceptions(this Task task)
        {
            DebugUtils.CheckNoExternalCallers();

            task.ContinueWith(
                t => { var ignored = t.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }
        #endregion Task.IgnoreExceptions - copied from Samples for Parallel Programming

        #region TaskFactory.GetTargetScheduler - copied from Samples for Parallel Programming
        /// <summary>Gets the TaskScheduler instance that should be used to schedule tasks.</summary>
        /// <param name="factory">Factory to get the scheduler for.</param>
        /// <returns>The scheduler for the specified factory.</returns>
        internal static TaskScheduler GetTargetScheduler(this TaskFactory factory)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(factory != null, "factory != null");
            return factory.Scheduler ?? TaskScheduler.Current;
        }
        #endregion TaskFactory.GetTargetScheduler - copied from Samples for Parallel Programming

        #region TaskFactory.Iterate - copied from Samples for Parallel Programming
        //// Note that if we would migrate to .NET 4.5 and could get dependency on the "await" keyword, all of this is not needed
        //// and we could use the await functionality instead.

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Stores the exception so that it doesn't bring down the process but isntead rethrows on the task calling thread.")]
        internal static Task Iterate(
            this TaskFactory factory,
            IEnumerable<Task> source)
        {
            DebugUtils.CheckNoExternalCallers();

            // Validate/update parameters
            Debug.Assert(factory != null, "factory != null");
            Debug.Assert(source != null, "source != null");

            // Get an enumerator from the enumerable
            var enumerator = source.GetEnumerator();
            Debug.Assert(enumerator != null, "enumerator != null");

            // Create the task to be returned to the caller.  And ensure
            // that when everything is done, the enumerator is cleaned up.
            var trc = new TaskCompletionSource<object>(null, factory.CreationOptions);
            trc.Task.ContinueWith(_ => enumerator.Dispose(), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            // This will be called every time more work can be done.
            Action<Task> recursiveBody = null;
            recursiveBody = antecedent =>
            {
                try
                {
                    // If the previous task completed with any exceptions, bail 
                    if (antecedent != null && antecedent.IsFaulted)
                    {
                        trc.TrySetException(antecedent.Exception);
                        return;
                    }

                    // If we should continue iterating and there's more to iterate
                    // over, create a continuation to continue processing.  We only
                    // want to continue processing once the current Task (as yielded
                    // from the enumerator) is complete.
                    if (enumerator.MoveNext())
                    {
                        var nextTask = enumerator.Current;

                        // The IgnoreException here is effective only for the recursiveBody code
                        // (not the nextTask, which is being checked by the recursiveBody above).
                        // And since the recursiveBody already catches all exceptions except for the uncatchable
                        // ones, we think it's OK to ignore all those exception in the finalizer thread.
                        nextTask.ContinueWith(recursiveBody).IgnoreExceptions();
                    }
                    else
                    {
                        // Otherwise, we're done
                        trc.TrySetResult(null);
                    }
                }
                catch (Exception exc)
                {
                    if (!ExceptionUtils.IsCatchableExceptionType(exc))
                    {
                        throw;
                    }

                    // If MoveNext throws an exception, propagate that to the user,
                    // either as cancellation or as a fault
                    var oce = exc as OperationCanceledException;
                    if (oce != null && oce.CancellationToken == factory.CancellationToken)
                    {
                        trc.TrySetCanceled();
                    }
                    else
                    {
                        trc.TrySetException(exc);
                    }
                }
            };

            // Get things started by launching the first task
            // The IgnoreException here is effective only for the recursiveBody code
            // (not the nextTask, which is being checked by the recursiveBody above).
            // And since the recursiveBody already catches all exceptions except for the uncatchable
            // ones, we think it's OK to ignore all those exception in the finalizer thread.
            factory.StartNew(() => recursiveBody(null), CancellationToken.None, TaskCreationOptions.None, factory.GetTargetScheduler()).IgnoreExceptions();

            // Return the representative task to the user
            return trc.Task;
        }
        #endregion TaskFactory.Iterate - copied from Samples for Parallel Programming

        #region FollowOnSuccess helpers
        /// <summary>
        /// The func used as the continuation (the func in the ContinueWith) for FollowOnSuccess implementations.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the operation to follow up with.</typeparam>
        /// <param name="antecedentTask">The task which just finished.</param>
        /// <param name="taskCompletionSource">The task completion source to apply the result to.</param>
        /// <param name="operation">The func to execute as the follow up action in case of success of the <paramref name="antecedentTask"/>.</param>
        private static void FollowOnSuccessWithContinuation<TResult>(Task antecedentTask, TaskCompletionSource<TResult> taskCompletionSource, Func<Task, TResult> operation)
        {
            switch (antecedentTask.Status)
            {
                case TaskStatus.RanToCompletion:
                    try
                    {
                        taskCompletionSource.TrySetResult(operation(antecedentTask));
                    }
                    catch (Exception exception)
                    {
                        if (!ExceptionUtils.IsCatchableExceptionType(exception))
                        {
                            throw;
                        }

                        taskCompletionSource.TrySetException(exception);
                    }

                    break;
                case TaskStatus.Faulted:
                    taskCompletionSource.TrySetException(antecedentTask.Exception);
                    break;
                case TaskStatus.Canceled:
                    taskCompletionSource.TrySetCanceled();
                    break;
            }
        }

        /// <summary>
        /// The implementation helper for FollowOnSuccess methods which don't allow result type of Task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the followup operation, this MUST NOT be a Task type.</typeparam>
        /// <param name="antecedentTask">The task to follow with operation.</param>
        /// <param name="operation">The operation to follow up with.</param>
        /// <returns>A new Task which wraps both the <paramref name="antecedentTask"/> and the conditional execution of <paramref name="operation"/>.</returns>
        private static Task<TResult> FollowOnSuccessWithImplementation<TResult>(Task antecedentTask, Func<Task, TResult> operation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(antecedentTask != null, "antecedentTask != null");
            Debug.Assert(operation != null, "operation != null");
            Debug.Assert(
                !(typeof(Task).IsAssignableFrom(typeof(TResult))),
                "It's not valid to call FollowOnSucessWith on an operation which returns a Task, instead use FollowOnSuccessWithTask.");

            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
            antecedentTask.ContinueWith(
                (taskToContinueOn) => FollowOnSuccessWithContinuation(taskToContinueOn, taskCompletionSource, operation),
                TaskContinuationOptions.ExecuteSynchronously)

                // This is only for the body of delegate in the .ContinueWith, the antecedent task failures are handles by the body itself.
                .IgnoreExceptions();
            return taskCompletionSource.Task;
        }
        #endregion

        #region FollowOnFault helpers
        /// <summary>
        /// The implementation helper for FollowOnFault methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the task.</typeparam>
        /// <param name="antecedentTask">The task to follow with operation in case of fault.</param>
        /// <param name="getTaskResult">Func which gets a task result value.</param>
        /// <param name="operation">The operation to follow up with.</param>
        /// <returns>A new Task which wraps both the <paramref name="antecedentTask"/> and the conditional execution of <paramref name="operation"/>.</returns>
        private static Task<TResult> FollowOnFaultWithImplementation<TResult>(Task antecedentTask, Func<Task, TResult> getTaskResult, Action<Task> operation)
        {
            Debug.Assert(antecedentTask != null, "antecedentTask != null");
            Debug.Assert(operation != null, "operation != null");

            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
            antecedentTask.ContinueWith(
                t =>
                {
                    switch (t.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            taskCompletionSource.TrySetResult(getTaskResult(t));
                            break;
                        case TaskStatus.Faulted:
                            try
                            {
                                operation(t);
                                taskCompletionSource.TrySetException(t.Exception);
                            }
                            catch (Exception exception)
                            {
                                if (!ExceptionUtils.IsCatchableExceptionType(exception))
                                {
                                    throw;
                                }

                                AggregateException aggregateException = new AggregateException(t.Exception, exception);
                                taskCompletionSource.TrySetException(aggregateException);
                            }

                            break;
                        case TaskStatus.Canceled:
                            taskCompletionSource.TrySetCanceled();
                            break;
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously)

                // This is only for the body of delegate in the .ContinueWith, the antecedent task failures are handles by the body itself.
                .IgnoreExceptions();
            return taskCompletionSource.Task;
        }
        #endregion
    }
}
#endif
