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

    internal readonly ref struct UriWriter2<T> : IUriWriter2<T>, IBoxable<Func<System.Uri, T>, UriWriter2<T>>
        where T : allows ref struct
    {
        private readonly Func<System.Uri, T> nextFactory;

        public UriWriter2(Func<System.Uri, T> nextFactory)
        {
            this.nextFactory = nextFactory;
        }

        public Box<Func<System.Uri, T>, UriWriter2<T>> Box()
        {
            return new Box<Func<System.Uri, T>, UriWriter2<T>>(this.nextFactory, _ => new UriWriter2<T>(_));
        }

        public RefTaskV4<IUriSchemeWriter2<T>, Func<System.Uri, IUriSchemeWriter2<T>>> Commit2()
        {
            return new RefTaskV4<IUriSchemeWriter2<T>, Func<System.Uri, IUriSchemeWriter2<T>>>(ValueTask.FromResult(this.nextFactory), (_) => new UriSchemeWriter(new StringBuilder(), _));
        }

        RefTaskV3<IUriSchemeWriter2<T>> IUriWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>, IUriPathSegmentWriter2<T>, IUriPortWriter2<T>, IUriDomainWriter2<T>, IUriSchemeWriter2<T>>.Commit()
        {
            return new RefTaskV3<IUriSchemeWriter2<T>>(ValueTask.CompletedTask, () => new UriSchemeWriter(new StringBuilder(), this.nextFactory));
        }

        private sealed class UriSchemeWriter : IUriSchemeWriter2<T>
        {
            private readonly StringBuilder builder;
            private readonly Func<System.Uri, T> nextFactory;

            public UriSchemeWriter(StringBuilder builder, Func<System.Uri, T> nextFactory)
            {
                this.builder = builder; //// TODO this means that writer instances won't be re-usable, are you ok with that?
                this.nextFactory = nextFactory;
            }

            public RefTask<Nothing2, Nothing2, IUriDomainWriter<T>> Commit(UriScheme uriScheme)
            {
                this.builder.Append($"{uriScheme.Scheme}://");
                return RefTask.FromFunc<IUriDomainWriter<T>>(() => new UriDomainWriter(this.builder, this.nextFactory));
            }

            private sealed class UriDomainWriter : IUriDomainWriter<T>
            {
                private readonly StringBuilder builder;
                private readonly Func<System.Uri, T> nextFactory;

                public UriDomainWriter(StringBuilder builder, Func<System.Uri, T> nextFactory)
                {
                    this.builder = builder;
                    this.nextFactory = nextFactory;
                }

                public RefTask<Nothing2, Nothing2, IUriPortWriter<T>> Commit(UriDomain uriDomain)
                {
                    this.builder.Append(uriDomain.Domain);

                    return RefTask.FromFunc<IUriPortWriter<T>>(() => new UriPortWriter(this.builder, this.nextFactory));
                }

                private sealed class UriPortWriter : IUriPortWriter<T>
                {
                    private readonly StringBuilder builder;
                    private readonly Func<System.Uri, T> nextFactory;

                    public UriPortWriter(StringBuilder builder, Func<System.Uri, T> nextFactory)
                    {
                        this.builder = builder;
                        this.nextFactory = nextFactory;
                    }

                    public RefTask<Nothing2, Nothing2, IUriPathSegmentWriter<T>> Commit()
                    {
                        return RefTask.FromFunc<IUriPathSegmentWriter<T>>(() => new UriPathSegmentWriter(this.builder, this.nextFactory));
                    }

                    public RefTask<Nothing2, Nothing2, IUriPathSegmentWriter<T>> Commit(UriPort uriPort)
                    {
                        this.builder.Append($":{uriPort.Port}");

                        return RefTask.FromFunc<IUriPathSegmentWriter<T>>(() => new UriPathSegmentWriter(this.builder, this.nextFactory));
                    }

                    private sealed class UriPathSegmentWriter : IUriPathSegmentWriter<T>
                    {
                        private readonly StringBuilder builder;
                        private readonly Func<System.Uri, T> nextFactory;

                        public UriPathSegmentWriter(StringBuilder builder, Func<System.Uri, T> nextFactory)
                        {
                            this.builder = builder;
                            this.nextFactory = nextFactory;
                        }

                        public RefTask<Nothing2, Nothing2, IQueryOptionWriter<T>> Commit()
                        {
                            return RefTask.FromFunc<IQueryOptionWriter<T>>(() => new QueryOptionWriter(this.builder, this.nextFactory, false));
                        }

                        private sealed class QueryOptionWriter : IQueryOptionWriter<T>
                        {
                            private readonly StringBuilder builder;
                            private readonly Func<System.Uri, T> nextFactory;
                            private readonly bool queryParametersWritten;

                            public QueryOptionWriter(StringBuilder builder, Func<System.Uri, T> nextFactory, bool queryParametersWritten)
                            {
                                this.builder = builder;
                                this.nextFactory = nextFactory;
                                this.queryParametersWritten = queryParametersWritten;
                            }

                            public RefTask<Nothing2, Nothing2, T> Commit()
                            {
                                return RefTask.FromFunc(() => this.nextFactory(new System.Uri(this.builder.ToString())));
                            }

                            public RefTask<Nothing2, Nothing2, IFragmentWriter<T>> CommitFragment()
                            {
                                return RefTask.FromFunc<IFragmentWriter<T>>(() => new FragmentWriter(this.builder, this.nextFactory));
                            }

                            private sealed class FragmentWriter : IFragmentWriter<T>
                            {
                                private readonly StringBuilder builder;
                                private readonly Func<System.Uri, T> nextFactory;

                                public FragmentWriter(StringBuilder builder, Func<System.Uri, T> nextFactory)
                                {
                                    this.builder = builder;
                                    this.nextFactory = nextFactory;
                                }

                                public RefTask<Nothing2, Nothing2, T> Commit(Fragment fragment)
                                {
                                    this.builder.Append($"#{fragment.Value}");

                                    return RefTask.FromFunc(() => this.nextFactory(new System.Uri(this.builder.ToString())));
                                }
                            }

                            public RefTask<Nothing2, Nothing2, IQueryParameterWriter<T>> CommitParameter()
                            {
                                return RefTask.FromFunc<IQueryParameterWriter<T>>(() => new QueryParameterWriter(this.builder, this.nextFactory, this.queryParametersWritten));
                            }

                            private sealed class QueryParameterWriter : IQueryParameterWriter<T>
                            {
                                private readonly StringBuilder builder;
                                private readonly Func<System.Uri, T> nextFactory;
                                private readonly bool queryParametersWritten;

                                public QueryParameterWriter(StringBuilder builder, Func<System.Uri, T> nextFactory, bool queryParametersWritten)
                                {
                                    this.builder = builder;
                                    this.nextFactory = nextFactory;
                                    this.queryParametersWritten = queryParametersWritten;
                                }

                                public RefTask<Nothing2, Nothing2, IQueryValueWriter<T>> Commit(QueryParameter queryParameter)
                                {
                                    if (this.queryParametersWritten)
                                    {
                                        this.builder.Append("&");
                                    }
                                    else
                                    {
                                        this.builder.Append("?");
                                    }

                                    this.builder.Append(queryParameter.Name);

                                    return RefTask.FromFunc<IQueryValueWriter<T>>(() => new QueryValueWriter(this.builder, this.nextFactory));
                                }

                                private sealed class QueryValueWriter : IQueryValueWriter<T>
                                {
                                    private readonly StringBuilder builder;
                                    private readonly Func<System.Uri, T> nextFactory;

                                    public QueryValueWriter(StringBuilder builder, Func<System.Uri, T> nextFactory)
                                    {
                                        this.builder = builder;
                                        this.nextFactory = nextFactory;
                                    }

                                    public RefTask<Nothing2, Nothing2, IQueryOptionWriter<T>> Commit()
                                    {
                                        //// TODO `fromfunc` calls should almost certainly be `fromresult` calls
                                        //// TODO ref tasks would also let you avoid this closure
                                        return RefTask.FromFunc<IQueryOptionWriter<T>>(() => new QueryOptionWriter(this.builder, this.nextFactory, true));
                                    }

                                    public RefTask<Nothing2, Nothing2, IQueryOptionWriter<T>> Commit(QueryValue queryValue)
                                    {
                                        return RefTask.FromFunc<IQueryOptionWriter<T>>(() => new QueryOptionWriter(this.builder, this.nextFactory, true));
                                    }
                                }
                            }
                        }


                        public RefTask<Nothing2, Nothing2, IUriPathSegmentWriter<T>> Commit(UriPathSegment uriPathSegment)
                        {
                            this.builder.Append($"/{uriPathSegment.Segment}");

                            return RefTask.FromFunc<IUriPathSegmentWriter<T>>(() => new UriPathSegmentWriter(this.builder, this.nextFactory));
                        }
                    }
                }
            }
        }
    }












    public static class PlaygroundRefV3
    {
        public static async Task<T> WriteUri<TContext, TUriWriter, T>(System.Uri uri, TContext context, Box<TContext, TUriWriter> uriWriterBox)
            where TUriWriter : IUriWriter2<T>, IBoxable<TContext, TUriWriter>, allows ref struct
        {
            var uriWriter = uriWriterBox.GetValue();

            var schemeWriter = await uriWriter.Commit2().ConfigureAwait(false);

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
    }



    public interface IUriWriter2<T> : IUriWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>, IUriPathSegmentWriter2<T>, IUriPortWriter2<T>, IUriDomainWriter2<T>, IUriSchemeWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IUriWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort, TUriDomain, TUriScheme>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriDomain : IUriDomainWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort>, allows ref struct
        where TUriScheme : IUriSchemeWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort, TUriDomain>, allows ref struct
    {
        RefTaskV3<TUriScheme> Commit();

        RefTaskV4<TUriScheme, Func<System.Uri, IUriSchemeWriter2<TNext>>> Commit2();
    }

    public interface IUriSchemeWriter2<T> : IUriSchemeWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>, IUriPathSegmentWriter2<T>, IUriPortWriter2<T>, IUriDomainWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IUriSchemeWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort, TUriDomain>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriDomain : IUriDomainWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort>, allows ref struct
    {
        RefTaskV3<TUriDomain> Commit(UriScheme uriScheme); //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
    }

    public interface IUriDomainWriter2<T> : IUriDomainWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>, IUriPathSegmentWriter2<T>, IUriPortWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IUriDomainWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
    {
        RefTaskV3<TUriPort> Commit(UriDomain uriDomain);
    }

    public interface IUriPortWriter2<T> : IUriPortWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>, IUriPathSegmentWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IUriPortWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
    {
        RefTaskV3<TPathSegment> Commit();

        RefTaskV3<TPathSegment> Commit(UriPort uriPort);
    }

    public interface IUriPathSegmentWriter2<T> : IUriPathSegmentWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>, IUriPathSegmentWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
    {
        RefTaskV3<TQueryOption> Commit();
        RefTaskV3<TPathSegment> Commit(UriPathSegment uriPathSegment);
    }

    public interface IQueryOptionWriter2<T> : IQueryOptionWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
    {
        RefTaskV3<TNext> Commit();
        RefTaskV3<TFragment> CommitFragment();
        RefTaskV3<TQueryParameter> CommitParameter();
    }

    public interface IQueryParameterWriter2<T> : IQueryParameterWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
    {
        RefTaskV3<TQueryValue> Commit(QueryParameter queryParameter);
    }

    public interface IQueryValueWriter2<T> : IQueryValueWriter<T, IFragmentWriter2<T>, IQueryParameterWriter2<T>, IQueryValueWriter2<T>, IQueryOptionWriter2<T>>
        where T : allows ref struct
    {
    }

    public interface IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter2<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
    {
        RefTaskV3<TQueryOption> Commit();
        RefTaskV3<TQueryOption> Commit(QueryValue queryValue);
    }

    public interface IFragmentWriter2<TNext>
        where TNext : allows ref struct
    {
        RefTaskV3<TNext> Commit(Fragment fragment);
    }
}
