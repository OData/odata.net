//---------------------------------------------------------------------
// <copyright file="TaskUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OData.Core;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class TaskUtilsTests
    {
        #region ThenOnSuccess (value)
        [Fact]
        public async Task ThenOnSuccess_Value_ThrowsOnNullAntecedent()
        {
            Task<int> antecedent = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenOnSuccess((int _) => { }));
        }

        [Fact]
        public async Task ThenOnSuccess_Value_ThrowsOnNullOperation()
        {
            var antecedent = Task.FromResult(5);
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenOnSuccess((Action<int>)null!));
        }

        [Fact]
        public async Task ThenOnSuccess_Value_Success_InvokesOperationAndPreservesResult()
        {
            int called = 0;
            var result = await Task.FromResult(7).ThenOnSuccess(r => called = r);
            Assert.Equal(7, result);
            Assert.Equal(7, called);
        }

        [Fact]
        public async Task ThenOnSuccess_Value_OperationThrows_PropagatesException()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Task.FromResult(3).ThenOnSuccess((int _) => throw new InvalidOperationException()));
        }

        [Fact]
        public async Task ThenOnSuccess_Value_Faulted_PropagatesOriginalException()
        {
            var faulted = Task.FromException<int>(new ArgumentException("boom"));
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => faulted.ThenOnSuccess((int _) => { }));
            Assert.Equal("boom", ex.Message);
        }

        [Fact]
        public async Task ThenOnSuccess_Value_Canceled_PropagatesCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled<int>(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() => canceled.ThenOnSuccess((int _) => { }));
        }

        [Fact]
        public async Task ThenOnSuccess_Value_FaultedSingleInnerAggregate_Unwraps()
        {
            var antecedent = Task.FromException<int>(new ArgumentException("wrapped"));

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                antecedent.ThenOnSuccess((int _) => { }));

            Assert.Equal("wrapped", ex.Message);
        }

        [Fact]
        public async Task ThenOnSuccess_Value_FaultedUserAggregate_PreservesAggregate()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetException(new AggregateException(new ArgumentException("inner")));
            var ex = await Assert.ThrowsAsync<AggregateException>(() =>
                tcs.Task.ThenOnSuccess((int _) => { }));
            Assert.Single(ex.InnerExceptions);
            Assert.IsType<ArgumentException>(ex.InnerExceptions[0]);
            Assert.Equal("inner", ex.InnerExceptions[0].Message);
        }

        [Fact]
        public async Task ThenOnSuccess_Value_FaultedMultiInnerAggregate_PreservesAggregate()
        {
            var agg = new AggregateException(new InvalidOperationException("a"), new FormatException("b"));
            var tcs = new TaskCompletionSource<int>();
            tcs.SetException(agg);
            var ex = await Assert.ThrowsAsync<AggregateException>(() =>
                tcs.Task.ThenOnSuccess((int _) => { }));
            Assert.Equal(2, ex.InnerExceptions.Count);
        }
        #endregion

        #region ThenOnSuccess (task parameter)
        [Fact]
        public async Task ThenOnSuccess_TaskParam_ThrowsOnNullAntecedent()
        {
            Task<int> antecedent = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenOnSuccess((int _) => { }));
        }

        [Fact]
        public async Task ThenOnSuccess_TaskParam_ThrowsOnNullOperation()
        {
            var antecedent = Task.FromResult(5);
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenOnSuccess((Action<Task<int>>)null!));
        }

        [Fact]
        public async Task ThenOnSuccess_TaskParam_Success_InvokesOperation()
        {
            int called = 0;
            await Task.FromResult(11).ThenOnSuccess(t => called = t.Result);
            Assert.Equal(11, called);
        }

        [Fact]
        public async Task ThenOnSuccess_TaskParam_OperationThrows_PropagatesException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Task.FromResult(1).ThenOnSuccess((int _) => throw new InvalidOperationException()));
        }

        [Fact]
        public async Task ThenOnSuccess_TaskParam_Faulted_PropagatesOriginalException()
        {
            var faulted = Task.FromException<int>(new ApplicationException("oops"));
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => faulted.ThenOnSuccess((Task<int> _) => { }));
            Assert.Equal("oops", ex.Message);
        }

        [Fact]
        public async Task ThenOnSuccess_TaskParam_Canceled_PropagatesCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled<int>(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() => canceled.ThenOnSuccess((Task<int> _) => { }));
        }

        [Fact]
        public async Task ThenOnSuccess_TaskParam_FaultedUserAggregate_PreservesAggregate()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetException(new AggregateException(new InvalidOperationException("inner")));
            bool invoked = false;
            var ex = await Assert.ThrowsAsync<AggregateException>(() =>
                tcs.Task.ThenOnSuccess((Task<int> t) => { invoked = true; }));
            Assert.False(invoked);
            Assert.Single(ex.InnerExceptions);
            Assert.IsType<InvalidOperationException>(ex.InnerExceptions[0]);
            Assert.Equal("inner", ex.InnerExceptions[0].Message);
        }

        [Fact]
        public async Task ThenOnSuccess_TaskParam_Canceled_DoesNotInvokeOperation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            bool invoked = false;
            var canceled = Task.FromCanceled<int>(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                canceled.ThenOnSuccess((Task<int> t) => { invoked = true; }));
            Assert.False(invoked);
        }
        #endregion

        #region ThenMapOnSuccess
        [Fact]
        public async Task ThenMapOnSuccess_ThrowsOnNullAntecedent()
        {
            Task antecedent = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenMapOnSuccess(_ => 0));
        }

        [Fact]
        public async Task ThenMapOnSuccess_ThrowsOnNullMap()
        {
            var antecedent = Task.CompletedTask;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenMapOnSuccess<int>(null!));
        }

        [Fact]
        public async Task ThenMapOnSuccess_Success_MapsResult()
        {
            var mapped = await Task.CompletedTask.ThenMapOnSuccess(_ => "mapped");
            Assert.Equal("mapped", mapped);
        }

        [Fact]
        public async Task ThenMapOnSuccess_MapThrows_PropagatesException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Task.CompletedTask.ThenMapOnSuccess<object>(_ => throw new InvalidOperationException()));
        }

        [Fact]
        public async Task ThenMapOnSuccess_Faulted_PropagatesOriginal()
        {
            var antecedent = Task.FromException(new FormatException("bad"));
            var ex = await Assert.ThrowsAsync<FormatException>(() => antecedent.ThenMapOnSuccess<object>(_ => new object()));
            Assert.Equal("bad", ex.Message);
        }

        [Fact]
        public async Task ThenMapOnSuccess_Canceled_PropagatesCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() => canceled.ThenMapOnSuccess<object>(_ => new object()));
        }

        [Fact]
        public async Task ThenMapOnSuccess_MapNotInvokedWhenFaulted()
        {
            bool invoked = false;
            var faulted = Task.FromException(new ApplicationException("x"));
            await Assert.ThrowsAsync<ApplicationException>(() =>
                faulted.ThenMapOnSuccess(_ => { invoked = true; return 1; }));
            Assert.False(invoked);
        }

        [Fact]
        public async Task ThenMapOnSuccess_MapReturnsNull_AllowsNull()
        {
            var result = await Task.CompletedTask.ThenMapOnSuccess(_ => (string)null!);
            Assert.Null(result);
        }
        #endregion

        #region ThenMapOnSuccessAsync
        [Fact]
        public async Task ThenMapOnSuccessAsync_ThrowsOnNullAntecedent()
        {
            Task<int> antecedent = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenMapOnSuccessAsync<int, int>(null!));
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_ThrowsOnNullMap()
        {
            var antecedent = Task.FromResult(1);
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenMapOnSuccessAsync<int, int>(null!));
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_Success_MapsResult()
        {
            var mapped = await Task.FromResult(5).ThenMapOnSuccessAsync<int, string>(t => Task.FromResult($"v{t.Result}"));
            Assert.Equal("v5", mapped);
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_MapReturnsNull_FaultsWithInvalidOperation()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Task.FromResult(2).ThenMapOnSuccessAsync<int, int>(_ => (Task<int>)null!));
            Assert.Equal(SRResources.TaskUtils_NullContinuationTask, ex.Message);
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_MapThrows_PropagatesException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Task.FromResult(2).ThenMapOnSuccessAsync<int, int>(_ => throw new InvalidOperationException()));
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_AntecedentFaulted_PropagatesOriginal()
        {
            var antecedent = Task.FromException<int>(new TimeoutException("late"));
            var ex = await Assert.ThrowsAsync<TimeoutException>(() =>
                antecedent.ThenMapOnSuccessAsync<int, int>(_ => Task.FromResult(0)));
            Assert.Equal("late", ex.Message);
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_AntecedentCanceled_PropagatesCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled<int>(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                canceled.ThenMapOnSuccessAsync<int, int>(_ => Task.FromResult(0)));
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_ContinuationTaskFaults_PropagatesContinuationFault()
        {
            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                Task.FromResult(9).ThenMapOnSuccessAsync<int, int>(_ => Task.FromException<int>(new ApplicationException("fail cont"))));
            Assert.Equal("fail cont", ex.Message);
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_ContinuationTaskCanceled_PropagatesCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var ex = await Assert.ThrowsAsync<TaskCanceledException>(() =>
                Task.FromResult(9).ThenMapOnSuccessAsync<int, int>(_ => Task.FromCanceled<int>(cts.Token)));
            Assert.IsType<TaskCanceledException>(ex);
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_MapNotInvokedWhenCanceled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            bool invoked = false;
            var canceled = Task.FromCanceled<int>(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                canceled.ThenMapOnSuccessAsync<int, string>(_ => { invoked = true; return Task.FromResult("x"); }));
            Assert.False(invoked);
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_ContinuationTaskResultNull_AllowsNull()
        {
            var result = await Task.FromResult(1)
                .ThenMapOnSuccessAsync<int, string>(_ => Task.FromResult<string>(null));
            Assert.Null(result);
        }

        [Fact]
        public async Task ThenMapOnSuccessAsync_ContinuationTaskFaultsWithAggregate_PreservesMultipleInners()
        {
            var agg = new AggregateException(new InvalidOperationException("a"), new FormatException("b"));
            var ex = await Assert.ThrowsAsync<AggregateException>(() =>
                Task.FromResult(5).ThenMapOnSuccessAsync<int, int>(_ => Task.FromException<int>(agg)));
            Assert.Equal(2, ex.InnerExceptions.Count);
            Assert.IsType<InvalidOperationException>(ex.InnerExceptions[0]);
            Assert.IsType<FormatException>(ex.InnerExceptions[1]);
        }
        #endregion

        #region ThenAlways (non-generic)
        [Fact]
        public async Task ThenAlways_ThrowsOnNullAntecedent()
        {
            Task antecedent = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenAlways(_ => { }));
        }

        [Fact]
        public async Task ThenAlways_ThrowsOnNullOperation()
        {
            var antecedent = Task.CompletedTask;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenAlways((Action<Task>)null!));
        }

        [Fact]
        public async Task ThenAlways_Success_NoOperationException_Completes()
        {
            await Task.CompletedTask.ThenAlways(_ => { });
        }

        [Fact]
        public async Task ThenAlways_Success_OperationThrows_FaultsWithOperationException()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Task.CompletedTask.ThenAlways(_ => throw new InvalidOperationException("op")));
            Assert.Equal("op", ex.Message);
        }

        [Fact]
        public async Task ThenAlways_Faulted_NoOperationException_PropagatesOriginal()
        {
            var antecedent = Task.FromException(new ArgumentNullException("p"));
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenAlways(_ => { }));
            Assert.Equal("p", ex.ParamName);
        }

        [Fact]
        public async Task ThenAlways_Faulted_OperationThrows_AggregatesExceptions()
        {
            var antecedent = Task.FromException(new ArgumentException("first"));
            var ex = await Assert.ThrowsAsync<AggregateException>(() =>
                antecedent.ThenAlways(_ => throw new InvalidOperationException("second")));
            Assert.Equal(2, ex.InnerExceptions.Count);
            Assert.IsType<ArgumentException>(ex.InnerExceptions[0]);
            Assert.IsType<InvalidOperationException>(ex.InnerExceptions[1]);
        }

        [Fact]
        public async Task ThenAlways_Canceled_NoOperationException_PreservesCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() => canceled.ThenAlways(_ => { }));
        }

        [Fact]
        public async Task ThenAlways_Canceled_OperationThrows_FaultsWithOperationException()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled(cts.Token);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                canceled.ThenAlways(_ => throw new InvalidOperationException("cleanup")));
            Assert.Equal("cleanup", ex.Message);
        }

        [Fact]
        public async Task ThenAlways_Canceled_ExecutesOperation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled(cts.Token);
            bool invoked = false;
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                canceled.ThenAlways(_ => invoked = true));
            Assert.True(invoked);
        }
        #endregion

        #region ThenAlways<TResult>
        [Fact]
        public async Task ThenAlways_Result_ThrowsOnNullAntecedent()
        {
            Task<int> antecedent = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenAlways(_ => { }));
        }

        [Fact]
        public async Task ThenAlways_Result_ThrowsOnNullOperation()
        {
            var antecedent = Task.FromResult(1);
            await Assert.ThrowsAsync<ArgumentNullException>(() => antecedent.ThenAlways((Action<Task<int>>)null!));
        }

        [Fact]
        public async Task ThenAlways_Result_Success_NoOpException_PreservesResult()
        {
            var value = await Task.FromResult(42).ThenAlways(_ => { });
            Assert.Equal(42, value);
        }

        [Fact]
        public async Task ThenAlways_Result_Success_OperationThrows_FaultsWithOperationException()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Task.FromResult(5).ThenAlways(_ => throw new InvalidOperationException("post")));
            Assert.Equal("post", ex.Message);
        }

        [Fact]
        public async Task ThenAlways_Result_Faulted_NoOpException_PropagatesOriginal()
        {
            var antecedent = Task.FromException<int>(new ApplicationException("boom"));
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => antecedent.ThenAlways(_ => { }));
            Assert.Equal("boom", ex.Message);
        }

        [Fact]
        public async Task ThenAlways_Result_Faulted_OperationThrows_AggregatesExceptions()
        {
            var antecedent = Task.FromException<int>(new ArgumentException("primary"));
            var ex = await Assert.ThrowsAsync<AggregateException>(() =>
                antecedent.ThenAlways(_ => throw new InvalidOperationException("secondary")));
            Assert.Equal(2, ex.InnerExceptions.Count);
            Assert.IsType<ArgumentException>(ex.InnerExceptions[0]);
            Assert.IsType<InvalidOperationException>(ex.InnerExceptions[1]);
        }

        [Fact]
        public async Task ThenAlways_Result_Canceled_NoOperationException_PreservesCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled<int>(cts.Token);
            await Assert.ThrowsAsync<TaskCanceledException>(() => canceled.ThenAlways(_ => { }));
        }

        [Fact]
        public async Task ThenAlways_Result_Canceled_OperationThrows_FaultsWithOperationException()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var canceled = Task.FromCanceled<int>(cts.Token);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                canceled.ThenAlways(_ => throw new InvalidOperationException("cleanup")));
            Assert.Equal("cleanup", ex.Message);
        }

        [Fact]
        public async Task ThenAlways_Result_FaultedNestedAggregate_OperationThrows_AppendsOperationException()
        {
            // User supplies an AggregateException with 2 leaf exceptions.
            var suppliedAggregate = new AggregateException(
                new InvalidOperationException("a"),
                new FormatException("b"));

            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(suppliedAggregate);

            // The Task infrastructure wraps any single exception (including AggregateException) in an outer AggregateException.
            // So task.Exception seen by the continuation has one supplied AggregateException (itself with two leaf inner exceptions).
            var ex = await Assert.ThrowsAsync<AggregateException>(() =>
                tcs.Task.ThenAlways(_ => throw new ApplicationException("c")));

            // The single inner exception (which is an AggregateException) is NOT unwrapped.
            // ThenAlways appends the operation exception, yielding two inner exceptions:
            //   [0] = supplied AggregateException (itself with two leaf inner exceptions)
            //   [1] = ApplicationException("c")
            Assert.Equal(2, ex.InnerExceptions.Count);
            Assert.IsType<AggregateException>(ex.InnerExceptions[0]);
            Assert.IsType<ApplicationException>(ex.InnerExceptions[1]);

            var nested = (AggregateException)ex.InnerExceptions[0];
            Assert.Equal(2, nested.InnerExceptions.Count);
            Assert.IsType<InvalidOperationException>(nested.InnerExceptions[0]);
            Assert.IsType<FormatException>(nested.InnerExceptions[1]);
            Assert.Equal("c", ex.InnerExceptions[1].Message);
        }
        #endregion
    }
}
