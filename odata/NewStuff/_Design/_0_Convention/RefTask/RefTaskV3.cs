using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using NewStuff._Design._1_Protocol.Sample;

namespace NewStuff._Design._0_Convention.RefTask
{
    public struct RefTaskV3<TResult> where TResult : allows ref struct
    {
        private readonly ValueTask doWork;
        private readonly Func<TResult> generateResult;

        public RefTaskV3(ValueTask doWork, Func<TResult> generateResult)
        {
            this.doWork = doWork;
            this.generateResult = generateResult;
        }

        public RefTaskV3Awaiter<TResult> GetAwaiter()
        {
            return new RefTaskV3Awaiter<TResult>(this.doWork.GetAwaiter(), this.generateResult);
        }

        public RefTaskV3<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            //// TODO actually implement this
            return this;
        }
    }

    public struct RefTaskV3Awaiter<TResult> : ICriticalNotifyCompletion where TResult : allows ref struct
    {
        private readonly ValueTaskAwaiter doWork;
        private readonly Func<TResult> generateResult;

        public RefTaskV3Awaiter(ValueTaskAwaiter doWork, Func<TResult> generateResult)
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






    public struct RefTaskV4<TResult, TIntermediate> where TResult : allows ref struct
    {
        private readonly ValueTask<TIntermediate> doWork;
        private readonly Func<TIntermediate, TResult> generateResult;

        public RefTaskV4(ValueTask<TIntermediate> doWork, Func<TIntermediate, TResult> generateResult)
        {
            this.doWork = doWork;
            this.generateResult = generateResult;
        }

        public RefTaskV4Awaiter<TResult, TIntermediate> GetAwaiter()
        {
            return new RefTaskV4Awaiter<TResult, TIntermediate>(this.doWork.GetAwaiter(), this.generateResult);
        }

        public RefTaskV4<TResult, TIntermediate> ConfigureAwait(bool continueOnCapturedContext)
        {
            //// TODO actually implement this
            return this;
        }
    }

    public struct RefTaskV4Awaiter<TResult, TIntermediate> : ICriticalNotifyCompletion where TResult : allows ref struct
    {
        private readonly ValueTaskAwaiter<TIntermediate> doWork;
        private readonly Func<TIntermediate, TResult> generateResult;

        public RefTaskV4Awaiter(ValueTaskAwaiter<TIntermediate> doWork, Func<TIntermediate, TResult> generateResult)
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









    public interface IBoxable<TContext, TValue> where TValue : IBoxable<TContext, TValue>, allows ref struct
    {
        Box<TContext, TValue> Box();
    }

    public readonly struct Box<TContext, TValue>
        where TValue : IBoxable<TContext, TValue>, allows ref struct
    {
        private readonly TContext context;
        private readonly Func<TContext, TValue> func;

        public Box(TContext context, Func<TContext, TValue> func)
        {
            this.context = context;
            this.func = func;
        }

        public TValue GetValue()
        {
            return this.func(this.context);
        }
    }














    public static class PlaygroundRefV3
    {
        public static async Task<T> WriteUri<TContext, TUriWriter, T>(System.Uri uri, TContext context, Box<TContext, TUriWriter> uriWriterBox)
            where TUriWriter : IUriWriter2<T>, IBoxable<TContext, TUriWriter>, allows ref struct
        {
            var uriWriter = uriWriterBox.GetValue();

            var schemeWriter = await uriWriter.Commit().ConfigureAwait(false);

            var domainWriter = await schemeWriter.Commit(new UriScheme(uri.Scheme)).ConfigureAwait(false);
            var portWriter = await domainWriter.Commit(new UriDomain(uri.DnsSafeHost)).ConfigureAwait(false);

            IUriPathSegmentWriter2<T> uriPathSegmentWriter;
            if (uri.IsDefaultPort)
            {
                uriPathSegmentWriter = await portWriter.Commit().ConfigureAwait(false);
            }
            else
            {
                uriPathSegmentWriter = await portWriter.Commit(new UriPort(uri.Port)).ConfigureAwait(false);
            }

            foreach (var pathSegment in uri.Segments)
            {
                uriPathSegmentWriter = await uriPathSegmentWriter.Commit(new UriPathSegment(pathSegment)).ConfigureAwait(false);
            }

            var queryOptionWriter = await uriPathSegmentWriter.Commit().ConfigureAwait(false);
            foreach (var queryOption in SplitQueryQueryString(uri.Query))
            {
                var parameterWriter = await queryOptionWriter.CommitParameter().ConfigureAwait(false);
                var valueWriter = await parameterWriter.Commit(new QueryParameter(queryOption.Item1)).ConfigureAwait(false);
                if (queryOption.Item2 == null)
                {
                    queryOptionWriter = await valueWriter.Commit().ConfigureAwait(false);
                }
                else
                {
                    queryOptionWriter = await valueWriter.Commit(new QueryValue(queryOption.Item2)).ConfigureAwait(false);
                }
            }

            T getHeaderWriter;
            if (string.IsNullOrEmpty(uri.Fragment))
            {
                getHeaderWriter = await queryOptionWriter.Commit().ConfigureAwait(false);
            }
            else
            {
                var fragmentWriter = await queryOptionWriter.CommitFragment().ConfigureAwait(false);
                getHeaderWriter = await fragmentWriter.Commit(new Fragment(uri.Fragment)).ConfigureAwait(false);
            }

            return getHeaderWriter;
        }

        private static IEnumerable<Tuple<string, string?>> SplitQueryQueryString(string queryString)
        {
            return SplitQueryQueryString(queryString, 1);
        }

        private static IEnumerable<Tuple<string, string?>> SplitQueryQueryString(string queryString, int currentIndex)
        {
            while (true)
            {
                if (currentIndex >= queryString.Length)
                {
                    yield break;
                }

                var queryOptionDelimiterIndex = queryString.IndexOf('&', currentIndex);
                if (queryOptionDelimiterIndex == -1)
                {
                    queryOptionDelimiterIndex = queryString.Length;
                }

                var parameterNameDelimiterIndex = queryString.IndexOf('=', currentIndex, queryOptionDelimiterIndex - currentIndex);
                if (parameterNameDelimiterIndex == -1)
                {
                    yield return Tuple.Create(
                        queryString.Substring(currentIndex, queryOptionDelimiterIndex - currentIndex),
                        (string?)null);
                }
                else
                {
                    var name = queryString.Substring(currentIndex, parameterNameDelimiterIndex - currentIndex);
                    string? value;
                    if (parameterNameDelimiterIndex + 1 == queryOptionDelimiterIndex)
                    {
                        value = null;
                    }
                    else
                    {
                        value = queryString.Substring(parameterNameDelimiterIndex + 1, queryOptionDelimiterIndex - parameterNameDelimiterIndex);
                    }

                    yield return Tuple.Create(
                        name,
                        value);
                }

                currentIndex = queryOptionDelimiterIndex + 1;
            }
        }

        public readonly struct UriWriterContext<T> where T : allows ref struct
        {
            public UriWriterContext(Func<System.Uri, T> nextFactory)
            {
                NextFactory = nextFactory;
            }

            public Func<System.Uri, T> NextFactory { get; }
        }

        public readonly ref struct UriWriter<T> :
            IUriWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>, UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>, UriPortWriter<T>, UriPortWriterContext<T>, UriDomainWriter<T>, UriDomainWriterContext<T>, UriSchemeWriter<T>, UriSchemeWriterContext<T>>,
            IBoxable<UriWriterContext<T>, UriWriter<T>>
            where T : allows ref struct
        {
            private readonly UriWriterContext<T> uriWriterContext;

            public UriWriter(UriWriterContext<T> uriWriterContext)
            {
                this.uriWriterContext = uriWriterContext;
            }

            public Box<UriWriterContext<T>, UriWriter<T>> Box()
            {
                return new Box<UriWriterContext<T>, UriWriter<T>>(this.uriWriterContext, _ => new UriWriter<T>(_));
            }

            public RefTaskV4<UriSchemeWriter<T>, UriSchemeWriterContext<T>> Commit()
            {
                var uriSchemeWriterContext = new UriSchemeWriterContext<T>(
                    new StringBuilder(),
                    this.uriWriterContext.NextFactory);
                return new RefTaskV4<UriSchemeWriter<T>, UriSchemeWriterContext<T>>(
                    ValueTask.FromResult(uriSchemeWriterContext),
                    context => new UriSchemeWriter<T>(context));
            }
        }

        public readonly struct UriSchemeWriterContext<T> where T : allows ref struct
        {
            public UriSchemeWriterContext(StringBuilder builder, Func<System.Uri, T> nextFactory)
            {
                Builder = builder; //// TODO this means that writer instances won't be re-usable, are you ok with that?
                NextFactory = nextFactory;
            }

            public StringBuilder Builder { get; } //// TODO can this be read-only?
            public Func<System.Uri, T> NextFactory { get; }
        }

        public readonly ref struct UriSchemeWriter<T> :
            IUriSchemeWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>, UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>, UriPortWriter<T>, UriPortWriterContext<T>, UriDomainWriter<T>, UriDomainWriterContext<T>>
            where T : allows ref struct
        {
            private readonly UriSchemeWriterContext<T> uriSchemeWriterContext;

            public UriSchemeWriter(UriSchemeWriterContext<T> uriSchemeWriterContext)
            {
                this.uriSchemeWriterContext = uriSchemeWriterContext;
            }

            public RefTaskV4<UriDomainWriter<T>, UriDomainWriterContext<T>> Commit(UriScheme uriScheme)
            {
                return new RefTaskV4<UriDomainWriter<T>, UriDomainWriterContext<T>>(
                    CommitImpl(uriScheme),
                    context => new UriDomainWriter<T>(context));
            }

            private ValueTask<UriDomainWriterContext<T>> CommitImpl(UriScheme uriScheme)
            {
                this.uriSchemeWriterContext.Builder.Append($"{uriScheme.Scheme}://");
                return ValueTask.FromResult(new UriDomainWriterContext<T>(
                    this.uriSchemeWriterContext.Builder,
                    this.uriSchemeWriterContext.NextFactory));
            }
        }

        public readonly struct UriDomainWriterContext<T> where T : allows ref struct
        {
            public UriDomainWriterContext(StringBuilder builder, Func<System.Uri, T> nextFactory)
            {
                Builder = builder;
                NextFactory = nextFactory;
            }

            public StringBuilder Builder { get; }
            public Func<System.Uri, T> NextFactory { get; }
        }

        public readonly ref struct UriDomainWriter<T> : 
            IUriDomainWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>, UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>, UriPortWriter<T>, UriPortWriterContext<T>>
            where T : allows ref struct
        {
            private readonly UriDomainWriterContext<T> uriDomainWriterContext;

            public UriDomainWriter(UriDomainWriterContext<T> uriDomainWriterContext)
            {
                this.uriDomainWriterContext = uriDomainWriterContext;
            }

            public RefTaskV3<UriPortWriter<T>> Commit(UriDomain uriDomain)
            {
                throw new NotImplementedException();
            }
        }

        public readonly struct UriPortWriterContext<T> where T : allows ref struct
        {
        }

        public readonly ref struct UriPortWriter<T> :
            IUriPortWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>, UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>>
            where T : allows ref struct
        {
            private readonly UriPortWriterContext<T> uriPortWriterContext;

            public UriPortWriter(UriPortWriterContext<T> uriPortWriterContext)
            {
                this.uriPortWriterContext = uriPortWriterContext;
            }

            public RefTaskV4<UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>> Commit()
            {
                throw new NotImplementedException();
            }

            public RefTaskV4<UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>> Commit(UriPort uriPort)
            {
                throw new NotImplementedException();
            }
        }

        public readonly struct UriPathSegmentWriterContext<T> where T : allows ref struct
        {
        }

        public readonly ref struct UriPathSegmentWriter<T> :
            IUriPathSegmentWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>, UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>>
            where T : allows ref struct
        {
            private readonly UriPathSegmentWriterContext<T> uriPathSegmentWriterContext;

            public UriPathSegmentWriter(UriPathSegmentWriterContext<T> uriPathSegmentWriterContext)
            {
                this.uriPathSegmentWriterContext = uriPathSegmentWriterContext;
            }

            public RefTaskV4<QueryOptionWriter<T>, QueryOptionWriterContext<T>> Commit()
            {
                throw new NotImplementedException();
            }

            public RefTaskV4<UriPathSegmentWriter<T>, UriPathSegmentWriterContext<T>> Commit(UriPathSegment uriPathSegment)
            {
                throw new NotImplementedException();
            }
        }

        public readonly struct QueryOptionWriterContext<T> where T : allows ref struct
        {
        }

        public readonly ref struct QueryOptionWriter<T> :
            IQueryOptionWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>>
            where T : allows ref struct
        {
            private readonly QueryOptionWriterContext<T> queryOptionWriterContext;

            public QueryOptionWriter(QueryOptionWriterContext<T> queryOptionWriterContext)
            {
                this.queryOptionWriterContext = queryOptionWriterContext;
            }

            public RefTaskV4<T, Nothing2> Commit()
            {
                throw new NotImplementedException();
            }

            public RefTaskV4<FragmentWriter<T>, FragmentWriterContext<T>> CommitFragment()
            {
                throw new NotImplementedException();
            }

            public RefTaskV4<QueryParameterWriter<T>, QueryParameterWriterContext<T>> CommitParameter()
            {
                throw new NotImplementedException();
            }
        }

        public readonly struct QueryParameterWriterContext<T> where T : allows ref struct
        {
        }

        public readonly ref struct QueryParameterWriter<T> :
            IQueryParameterWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>>
            where T : allows ref struct
        {
            private readonly QueryParameterWriterContext<T> queryParameterWriterContext;

            public QueryParameterWriter(QueryParameterWriterContext<T> queryParameterWriterContext)
            {
                this.queryParameterWriterContext = queryParameterWriterContext;
            }

            public RefTaskV4<QueryValueWriter<T>, QueryValueWriterContext<T>> Commit(QueryParameter queryParameter)
            {
                throw new NotImplementedException();
            }
        }

        public readonly struct QueryValueWriterContext<T> where T : allows ref struct
        {
        }

        public readonly ref struct QueryValueWriter<T> :
            IQueryValueWriter<T, FragmentWriter<T>, FragmentWriterContext<T>, QueryParameterWriter<T>, QueryParameterWriterContext<T>, QueryValueWriter<T>, QueryValueWriterContext<T>, QueryOptionWriter<T>, QueryOptionWriterContext<T>>
            where T : allows ref struct
        {
            private readonly QueryValueWriterContext<T> queryValueWriterContext;

            public QueryValueWriter(QueryValueWriterContext<T> queryValueWriterContext)
            {
                this.queryValueWriterContext = queryValueWriterContext;
            }

            public RefTaskV4<QueryOptionWriter<T>, QueryOptionWriterContext<T>> Commit()
            {
                throw new NotImplementedException();
            }

            public RefTaskV4<QueryOptionWriter<T>, QueryOptionWriterContext<T>> Commit(QueryValue queryValue)
            {
                throw new NotImplementedException();
            }
        }

        public readonly struct FragmentWriterContext<T> where T : allows ref struct
        {
        }

        public readonly ref struct FragmentWriter<T> :
            IFragmentWriter2<T>
            where T : allows ref struct
        {
            private readonly FragmentWriterContext<T> fragmentWriterContext;

            public FragmentWriter(FragmentWriterContext<T> fragmentWriterContext)
            {
                this.fragmentWriterContext = fragmentWriterContext;
            }

            public RefTaskV4<T, Nothing2> Commit(Fragment fragment)
            {
                throw new NotImplementedException();
            }
        }
    }



    public interface IUriWriter2<T> : IUriWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2, IUriPathSegmentWriter2<T>, Nothing2, IUriPortWriter2<T>, Nothing2, IUriDomainWriter2<T>, Nothing2, IUriSchemeWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IUriWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext, TUriPort, TUriPortContext, TUriDomain, TUriDomainContext, TUriScheme, TUriSchemeContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
        where TUriDomain : IUriDomainWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext, TUriPort, TUriPortContext>, allows ref struct
        where TUriScheme : IUriSchemeWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext, TUriPort, TUriPortContext, TUriDomain, TUriDomainContext>, allows ref struct
    {
        RefTaskV4<TUriScheme, TUriSchemeContext> Commit();
    }

    public interface IUriSchemeWriter2<T> : IUriSchemeWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2, IUriPathSegmentWriter2<T>, Nothing2, IUriPortWriter2<T>, Nothing2, IUriDomainWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IUriSchemeWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext, TUriPort, TUriPortContext, TUriDomain, TUriDomainContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
        where TUriDomain : IUriDomainWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext, TUriPort, TUriPortContext>, allows ref struct
    {
        RefTaskV4<TUriDomain, TUriDomainContext> Commit(UriScheme uriScheme); //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
    }

    public interface IUriDomainWriter2<T> : IUriDomainWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2, IUriPathSegmentWriter2<T>, Nothing2, IUriPortWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IUriDomainWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext, TUriPort, TUriPortContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
    {
        RefTaskV3<TUriPort> Commit(UriDomain uriDomain);
    }

    public interface IUriPortWriter2<T> : IUriPortWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2, IUriPathSegmentWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IUriPortWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
    {
        RefTaskV4<TPathSegment, TPathSegmentContext> Commit();

        RefTaskV4<TPathSegment, TPathSegmentContext> Commit(UriPort uriPort);
    }

    public interface IUriPathSegmentWriter2<T> : IUriPathSegmentWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2, IUriPathSegmentWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IUriPathSegmentWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext, TPathSegment, TPathSegmentContext>, allows ref struct
    {
        RefTaskV4<TQueryOption, TQueryOptionContext> Commit();
        RefTaskV4<TPathSegment, TPathSegmentContext> Commit(UriPathSegment uriPathSegment);
    }

    public interface IQueryOptionWriter2<T> : IQueryOptionWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
    {
        RefTaskV4<TNext, Nothing2> Commit();
        RefTaskV4<TFragment, TFragmentContext> CommitFragment();
        RefTaskV4<TQueryParameter, TQueryParameterContext> CommitParameter();
    }

    public interface IQueryParameterWriter2<T> : IQueryParameterWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
    {
        RefTaskV4<TQueryValue, TQueryValueContext> Commit(QueryParameter queryParameter);
    }

    public interface IQueryValueWriter2<T> : IQueryValueWriter<T, IFragmentWriter2<T>, Nothing2, IQueryParameterWriter2<T>, Nothing2, IQueryValueWriter2<T>, Nothing2, IQueryOptionWriter2<T>, Nothing2>
        where T : allows ref struct
    {
    }

    public interface IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TFragmentContext, TQueryParameter, TQueryParameterContext, TQueryValue, TQueryValueContext, TQueryOption, TQueryOptionContext>, allows ref struct
    {
        RefTaskV4<TQueryOption, TQueryOptionContext> Commit();
        RefTaskV4<TQueryOption, TQueryOptionContext> Commit(QueryValue queryValue);
    }

    public interface IFragmentWriter2<TNext>
        where TNext : allows ref struct
    {
        RefTaskV4<TNext, Nothing2> Commit(Fragment fragment);
    }
}
