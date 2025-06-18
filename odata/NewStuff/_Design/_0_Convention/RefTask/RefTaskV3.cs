using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using NewStuff._Design._1_Protocol.Sample;

namespace NewStuff._Design._0_Convention.RefTask
{
    public struct RefTaskV3<T> where T : allows ref struct
    {
        private readonly ValueTask doWork;
        private readonly Func<T> generateResult;

        public RefTaskV3(ValueTask doWork, Func<T> generateResult)
        {
            this.doWork = doWork;
            this.generateResult = generateResult;
        }

        public RefTaskV3Awaiter<T> GetAwaiter()
        {
            return new RefTaskV3Awaiter<T>(this.doWork.GetAwaiter(), this.generateResult);
        }

        public RefTaskV3<T> ConfigureAwait(bool continueOnCapturedContext)
        {
            //// TODO actually implement this
            return this;
        }
    }

    public struct RefTaskV3Awaiter<T> : ICriticalNotifyCompletion where T : allows ref struct
    {
        private readonly ValueTaskAwaiter doWork;
        private readonly Func<T> generateResult;

        public RefTaskV3Awaiter(ValueTaskAwaiter doWork, Func<T> generateResult)
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

        public T GetResult()
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




    public static class PlaygroundRefV3
    {
        public static async Task<T> WriteUri<TUriWriter, T>(System.Uri uri, TUriWriter uriWriter)
            where TUriWriter : IUriWriter2<T>, allows ref struct
        {
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
