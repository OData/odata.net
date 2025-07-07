namespace NewStuff._Design._0_Convention.RefTask
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public struct RefTask<TResult, TIntermediate> where TResult : allows ref struct
    {
        private readonly ValueTask<TIntermediate> doWork;
        private readonly Func<TIntermediate, TResult> generateResult;

        public RefTask(ValueTask<TIntermediate> doWork, Func<TIntermediate, TResult> generateResult)
        {
            this.doWork = doWork;
            this.generateResult = generateResult;
        }

        public RefTaskAwaiter<TResult, TIntermediate> GetAwaiter()
        {
            return new RefTaskAwaiter<TResult, TIntermediate>(this.doWork.GetAwaiter(), this.generateResult);
        }

        public RefTask<TResult, TIntermediate> ConfigureAwait(bool continueOnCapturedContext)
        {
            //// TODO actually implement this
            return this;
        }
    }

    public struct RefTaskAwaiter<TResult, TIntermediate> : ICriticalNotifyCompletion where TResult : allows ref struct
    {
        private readonly ValueTaskAwaiter<TIntermediate> doWork;
        private readonly Func<TIntermediate, TResult> generateResult;

        public RefTaskAwaiter(ValueTaskAwaiter<TIntermediate> doWork, Func<TIntermediate, TResult> generateResult)
        {
            this.doWork = doWork;
            this.generateResult = generateResult;
        }

        public bool IsCompleted
        {
            get
            {
                return this.doWork.IsCompleted;
            }
        }

        public TResult GetResult()
        {
            return this.generateResult(this.doWork.GetResult());
        }

        public void OnCompleted(Action continuation)
        {
            this.doWork.OnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            this.doWork.UnsafeOnCompleted(continuation);
        }
    }
}
