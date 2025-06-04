namespace NewStuff._Design._0_Convention.RefTask
{
    using NewStuff._Design._0_Convention.Sample;
    using System;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public ref struct RefTask<TIntermediate, TContext, TResult>
        where TContext : allows ref struct
        where TResult : allows ref struct
    {
        private readonly Task<TIntermediate> doWork;
        private readonly Func<TIntermediate, TContext, TResult> generateResult;
        private readonly TContext context;

        public RefTask(Func<TIntermediate> doWork, Func<TIntermediate, TContext, TResult> generateResult, TContext context)
        {
            this.doWork = Task.Run(doWork); //// TODO is this the right way to create the task?
            this.generateResult = generateResult;
            this.context = context;
        }

        public RefTask(Task<TIntermediate> doWork, Func<TIntermediate, TContext, TResult> generateResult, TContext context)
        {
            this.doWork = doWork;
            this.generateResult = generateResult;
            this.context = context;
        }

        public RefTaskAwaiter<TIntermediate, TContext, TResult> GetAwaiter()
        {
            return new RefTaskAwaiter<TIntermediate, TContext, TResult>(this.doWork.GetAwaiter(), this.generateResult, this.context);
        }

        public RefTask<TIntermediate, TContext, TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            //// TODO actually implement this
            return this;
        }
    }

    public ref struct RefTaskAwaiter<TIntermediate, TContext, TResult> : ICriticalNotifyCompletion 
        where TContext : allows ref struct
        where TResult : allows ref struct
    {
        private readonly TaskAwaiter<TIntermediate> doWork;
        private readonly Func<TIntermediate, TContext, TResult> generateResult;
        private readonly TContext context;

        public RefTaskAwaiter(TaskAwaiter<TIntermediate> doWork, Func<TIntermediate, TContext, TResult> generateResult, TContext context)
        {
            this.doWork = doWork;
            this.generateResult = generateResult;
            this.context = context;
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
            return this.generateResult(this.doWork.GetResult(), this.context);
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

    public static class Playground
    {
        public interface IGetBodyWriter : IGetBodyWriter<Task<IGetResponseReader>, IGetResponseReader>
        {
        }

        private sealed class GetBodyWriter : IGetBodyWriter
        {
            private readonly IHttpClient httpClient;
            private readonly Uri requestUri;

            public GetBodyWriter(IHttpClient httpClient, Uri requestUri)
            {
                this.httpClient = httpClient;
                this.requestUri = requestUri;
            }

            public async Task<IGetResponseReader> Commit()
            {
                //// TODO malformed headers will throw here
                var httpResponseMessage = await this.httpClient.GetAsync(this.requestUri).ConfigureAwait(false);

                throw new Exception("TODO");
            }
        }

        public interface IGetBodyWriter<TTask, TGetResponseReader>
            where TTask : /*TODO Task<TGetResponseReader>, */allows ref struct
            where TGetResponseReader : IGetResponseReader, allows ref struct
        {
            TTask Commit();
        }

        public ref struct RefGetBodyWriter : IGetBodyWriter<RefTask<GetResponseReader>, GetResponseReader>
        {
            private readonly IHttpClient httpClient;
            private readonly Uri requestUri;

            public RefGetBodyWriter(IHttpClient httpClient, Uri requestUri)
            {
                this.httpClient = httpClient;
                this.requestUri = requestUri;
            }

            public RefTask<GetResponseReader> Commit()
            {

            }
        }

        public ref struct GetResponseReader : IGetResponseReader
        {
            private readonly HttpResponseMessage httpResponseMessage;

            public GetResponseReader(HttpResponseMessage httpResponseMessage)
            {
                this.httpResponseMessage = httpResponseMessage;
            }

            public ValueTask DisposeAsync()
            {
                throw new NotImplementedException();
            }

            public Task<IGetResponseHeaderReader> Next()
            {
                throw new NotImplementedException();
            }
        }

        public ref struct Foo
        {
            public Foo(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        public static void DoWork(Func<Foo> func)
        {
            var foo = func();
        }

        public static void DoWork2()
        {
            var value = 42;
            DoWork(() =>
            {
                ++value;
                return new Foo(value);
            });
        }
    }
}
