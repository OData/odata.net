namespace NewStuff._Design._0_Convention.RefTask2
{
    using NewStuff._Design._0_Convention.Sample;
    using NewStuff._Design._1_Protocol.Sample;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;








    public struct RefTask<TIntermediate, TContext, TResult>
        where TResult : allows ref struct
    {
        private readonly ValueTask<TIntermediate> doWork;
        private readonly Func<TIntermediate, TContext, TResult> generateResult;
        private readonly TContext context;

        public RefTask(Func<TIntermediate> doWork, Func<TIntermediate, TContext, TResult> generateResult, TContext context)
        {
            this.doWork = new ValueTask<TIntermediate>(Task.Run(doWork)); //// TODO is this the right way to create the task? //// TODO you need to avoid the allocation from `task.run`
            this.generateResult = generateResult;
            this.context = context;
        }

        public RefTask(ValueTask<TIntermediate> doWork, Func<TIntermediate, TContext, TResult> generateResult, TContext context)
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

    public static class RefTask2
    {
        public static RefTask<Nothing2, Nothing2, T> FromFunc<T>(Func<T> func)
        {
            return new RefTask<Nothing2, Nothing2, T>(() => new Nothing2(), (_, _) => func(), new Nothing2());
        }
    }

    public static class RefTask
    {
        public static RefTask<Nothing2, Nothing2, T> FromFunc<T>(Func<T> func)
        {
            return new RefTask<Nothing2, Nothing2, T>(() => new Nothing2(), (_, _) => func(), new Nothing2());
        }
    }

    public struct RefTaskAwaiter<TIntermediate, TContext, TResult> : ICriticalNotifyCompletion
        where TResult : allows ref struct
    {
        private readonly ValueTaskAwaiter<TIntermediate> doWork;
        private readonly Func<TIntermediate, TContext, TResult> generateResult;
        private readonly TContext context;

        public RefTaskAwaiter(ValueTaskAwaiter<TIntermediate> doWork, Func<TIntermediate, TContext, TResult> generateResult, TContext context)
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
    



    public interface IUriWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment, TUriPortIntermediate, TUriPortContext, TUriPort, TUriDomainIntermediate, TUriDomainContext, TUriDomain, TUriSchemeIntermediate, TUriSchemeContext, TUriScheme>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
        where TUriDomain : IUriDomainWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment, TUriPortIntermediate, TUriPortContext, TUriPort>, allows ref struct
        where TUriScheme : IUriSchemeWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment, TUriPortIntermediate, TUriPortContext, TUriPort, TUriDomainIntermediate, TUriDomainContext, TUriDomain>, allows ref struct
    {
        RefTask<TUriSchemeIntermediate, TUriSchemeContext, TUriScheme> Commit();
    }

    public interface IUriSchemeWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment, TUriPortIntermediate, TUriPortContext, TUriPort, TUriDomainIntermediate, TUriDomainContext, TUriDomain>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
        where TUriDomain : IUriDomainWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment, TUriPortIntermediate, TUriPortContext, TUriPort>, allows ref struct
    {
        RefTask<TUriDomainIntermediate, TUriDomainContext, TUriDomain> Commit(UriScheme uriScheme); //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
    }

    public interface IUriDomainWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment, TUriPortIntermediate, TUriPortContext, TUriPort>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
    {
        RefTask<TUriPortIntermediate, TUriPortContext, TUriPort> Commit(UriDomain uriDomain);
    }

    public interface IUriPortWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
    {
        RefTask<TPathSegmentIntermediate, TPathSegmentContext, TPathSegment> Commit();

        RefTask<TPathSegmentIntermediate, TPathSegmentContext, TPathSegment> Commit(UriPort uriPort);
    }

    public interface IUriPathSegmentWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption, TPathSegmentIntermediate, TPathSegmentContext, TPathSegment>, allows ref struct
    {
        RefTask<TQueryOptionIntermediate, TQueryOptionContext, TQueryOption> Commit();
        RefTask<TPathSegmentIntermediate, TPathSegmentContext, TPathSegment> Commit(UriPathSegment uriPathSegment);
    }

    public interface IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
    {
        RefTask<TNextIntermediate, TNextContext, TNext> Commit();
        RefTask<TFragmentIntermediate, TFragmentContext, TFragment> CommitFragment();
        RefTask<TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter> CommitParameter();
    }

    public interface IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
    {
        RefTask<TQueryValueIntermediate, TQueryValueContext, TQueryValue> Commit(QueryParameter queryParameter);
    }

    public interface IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TFragmentIntermediate, TFragmentContext, TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNextIntermediate, TNextContext, TNext, TFragmentIntermediate, TFragmentContext, TFragment, TQueryParameterIntermediate, TQueryParameterContext, TQueryParameter, TQueryValueIntermediate, TQueryValueContext, TQueryValue, TQueryOptionIntermediate, TQueryOptionContext, TQueryOption>, allows ref struct
    {
        RefTask<TQueryOptionIntermediate, TQueryOptionContext, TQueryOption> Commit();
        RefTask<TQueryOptionIntermediate, TQueryOptionContext, TQueryOption> Commit(QueryValue queryValue);
    }

    public interface IFragmentWriter<TNextIntermediate, TNextContext, TNext>
        where TNext : allows ref struct
    {
        RefTask<TNextIntermediate, TNextContext, TNext> Commit(Fragment fragment);
    }


























    public static class Playground4
    {
        public interface IGetBodyWriter<TTask, TGetResponseReader>
            where TTask : /*TODO Task<TGetResponseReader>, */allows ref struct
            where TGetResponseReader : IGetResponseReader, allows ref struct
        {
            TTask Commit();
        }

        public ref struct RefGetBodyWriter : IGetBodyWriter<RefTask<HttpResponseMessage, Nothing2, GetResponseReader>, GetResponseReader>
        {
            private readonly IHttpClient httpClient;
            private readonly Uri requestUri;

            public RefGetBodyWriter(IHttpClient httpClient, Uri requestUri)
            {
                this.httpClient = httpClient;
                this.requestUri = requestUri;
            }

            public RefTask<HttpResponseMessage, Nothing2, GetResponseReader> Commit()
            {
                return new RefTask<HttpResponseMessage, Nothing2, GetResponseReader>(
                    new ValueTask<HttpResponseMessage>(this.httpClient.GetAsync(this.requestUri)),
                    (message, nothing) => new GetResponseReader(message),
                    new Nothing2());
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

        public static async Task ReaderTest()
        {
            var writer = new RefGetBodyWriter();
            await writer.Commit().ConfigureAwait(false);
        }
    }




    /*public static class Playground3
    {
        public interface IGetBodyWriter<TGetResponseReader>
            where TGetResponseReader : IGetResponseReader, allows ref struct
        {
            ValueTask Commit();

            TGetResponseReader Next();
        }

        public interface IGetBodyWriter : IGetBodyWriter<IGetResponseReader>
        {
        }

        private sealed class GetBodyWriter : IGetBodyWriter
        {
            private readonly IHttpClient httpClient;
            private readonly Uri requestUri;

            private HttpResponseMessage? httpResponseMessage;

            public GetBodyWriter(IHttpClient httpClient, Uri requestUri)
            {
                this.httpClient = httpClient;
                this.requestUri = requestUri;
            }

            public IGetResponseReader Next()
            {
                throw new Exception("tODO");
            }

            public async ValueTask Commit()
            {
                //// TODO malformed headers will throw here
                this.httpResponseMessage = await this.httpClient.GetAsync(this.requestUri).ConfigureAwait(false);
            }
        }

        public ref struct RefGetBodyWriter : IGetBodyWriter<GetResponseReader>
        {
            private readonly IHttpClient httpClient;
            private readonly Uri requestUri;

            private HttpResponseMessage? httpResponseMessage;

            public RefGetBodyWriter(IHttpClient httpClient, Uri requestUri)
            {
                this.httpClient = httpClient;
                this.requestUri = requestUri;
            }

            public GetResponseReader Next()
            {
                if (this.httpResponseMessage == null)
                {
                    throw new Exception("TODO");
                }

                return new GetResponseReader(this.httpResponseMessage);
            }

            public async ValueTask Commit()
            {
                this.httpResponseMessage = await this.httpClient.GetAsync(this.requestUri).ConfigureAwait(false);
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

        public struct CurrentReader
        {
            private readonly IHttpClient httpClient;
            private readonly Uri requestUri;

            public CurrentReader(IHttpClient httpClient, Uri requestUri)
            {
                this.httpClient = httpClient;
                this.requestUri = requestUri;
            }

            public async ValueTask<NextReader> Next()
            {
                return new NextReader(await this.httpClient.GetAsync(this.requestUri).ConfigureAwait(false));
            }
        }

        public struct NextReader
        {
            public NextReader(HttpResponseMessage message)
            {
            }

            public async ValueTask<NextReader> Next()
            {
                throw new NotImplementedException("TODO");
            }
        }

        public static async Task ReaderTest()
        {
            RefGetBodyWriter writer = new RefGetBodyWriter();
            await writer.Commit().ConfigureAwait(false);
            var nextWriter = writer.Next();
            var nextNextWriter = await nextWriter.Next().ConfigureAwait(false);
        }

        public static async Task ReaderTest2(RefGetBodyWriter writer)
        {
            await writer.Commit().ConfigureAwait(false);
            var nextWriter = writer.Next();
            var nextNextWriter = await nextWriter.Next().ConfigureAwait(false);
        }

        public static Task ReaderTest(RefGetBodyWriter writer)
        {
            return writer.Commit().AsTask().ContinueWith(
                (task, context) =>
                {
                    /*var nextWriter = ((RefGetBodyWriter)context).Next();
                    var nextNextWriter = nextWriter.Next();*/
    /*context.ToString();
},
writer);
}

public ref struct Wrapper<T> where T : allows ref struct
{
public async Task Wait(out T value)
{
}
}
}

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

public ref struct RefTaskAwaiter<TIntermediate, TContext, TResult> : INotifyCompletion
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

public static class Playground2
{
public ref struct RefTask
{
public RefTaskAwaiter GetAwaiter()
{
return new RefTaskAwaiter();
}

public RefTask ConfigureAwait(bool continueOnCapturedContext)
{
//// TODO actually implement this
return this;
}
}

public ref struct RefTaskAwaiter : ICriticalNotifyCompletion
{
public bool IsCompleted
{
get
{
    throw new NotImplementedException("TODO");
}
}

public void GetResult()
{
throw new NotImplementedException("TODO");
}

public void OnCompleted(Action continuation)
{
throw new NotImplementedException("TODO");
}

public void UnsafeOnCompleted(Action continuation)
{
throw new NotImplementedException("TODO");
}
}

public ref struct RefTask<TResult>
where TResult : allows ref struct
{
private readonly Task doWork;
private readonly Func<TResult> generateResult;

public RefTask(Action doWork, Func<TResult> generateResult)
{
this.doWork = Task.Run(doWork); //// TODO is this the right way to create the task?
this.generateResult = generateResult;
}

public RefTask(Task doWork, Func<TResult> generateResult)
{
this.doWork = doWork;
this.generateResult = generateResult;
}

public RefTaskAwaiter<TResult> GetAwaiter()
{
return new RefTaskAwaiter<TResult>(this.doWork.GetAwaiter(), this.generateResult);
}

public RefTask<TResult> ConfigureAwait(bool continueOnCapturedContext)
{
//// TODO actually implement this
return this;
}
}

public ref struct RefTaskAwaiter<TResult> : INotifyCompletion //// TODO , ICriticalNotifyCompletion
where TResult : allows ref struct
{
private readonly TaskAwaiter doWork;
private readonly Func<TResult> generateResult;

public RefTaskAwaiter(TaskAwaiter doWork, Func<TResult> generateResult)
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
return this.generateResult();
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

public interface IGetBodyWriter<TTask, TGetResponseReader>
where TTask : /*TODO Task<TGetResponseReader>, *//*allows ref struct
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
    var message = this.httpClient.GetAsync(this.requestUri);
    return new RefTask<GetResponseReader>(
        message,
        () => new GetResponseReader(message.Result));
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

public static async RefTask ReaderTest()
{
var writer = new RefGetBodyWriter();
await writer.Commit();
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
where TTask : /*TODO Task<TGetResponseReader>, *//*allows ref struct
where TGetResponseReader : IGetResponseReader, allows ref struct
{
TTask Commit();
}

public ref struct RefGetBodyWriter : IGetBodyWriter<RefTask<HttpResponseMessage, Nothing, GetResponseReader>, GetResponseReader>
{
private readonly IHttpClient httpClient;
private readonly Uri requestUri;

public RefGetBodyWriter(IHttpClient httpClient, Uri requestUri)
{
    this.httpClient = httpClient;
    this.requestUri = requestUri;
}

public RefTask<HttpResponseMessage, Nothing, GetResponseReader> Commit()
{
    return new RefTask<HttpResponseMessage, Nothing, GetResponseReader>(
        this.httpClient.GetAsync(this.requestUri),
        (message, nothing) => new GetResponseReader(message),
        new Nothing());
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

public static async Task ReaderTest()
{
var writer = new RefGetBodyWriter();
await writer.Commit().ConfigureAwait(false);
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
}*/
}
