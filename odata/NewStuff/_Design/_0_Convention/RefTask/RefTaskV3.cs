using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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













    public interface IUriWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort, TUriDomain, TUriScheme>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
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

    public interface IUriSchemeWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort, TUriDomain>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriDomain : IUriDomainWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort>, allows ref struct
    {
        RefTaskV3<TUriDomain> Commit(UriScheme uriScheme); //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
    }

    public interface IUriDomainWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment, TUriPort>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
        where TUriPort : IUriPortWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption,  TPathSegment>, allows ref struct
    {
        RefTaskV3<TUriPort> Commit(UriDomain uriDomain);
    }

    public interface IUriPortWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
    {
        RefTaskV3<TPathSegment> Commit();

        RefTaskV3<TPathSegment> Commit(UriPort uriPort);
    }

    public interface IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TPathSegment : IUriPathSegmentWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption, TPathSegment>, allows ref struct
    {
        RefTaskV3<TQueryOption> Commit();
        RefTaskV3<TPathSegment> Commit(UriPathSegment uriPathSegment);
    }

    public interface IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
    {
        RefTaskV3<TNext> Commit();
        RefTaskV3<TFragment> CommitFragment();
        RefTaskV3<TQueryParameter> CommitParameter();
    }

    public interface IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
    {
        RefTaskV3<TQueryValue> Commit(QueryParameter queryParameter);
    }

    public interface IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>
        where TNext : allows ref struct
        where TFragment : IFragmentWriter<TNext>, allows ref struct
        where TQueryParameter : IQueryParameterWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryValue : IQueryValueWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
        where TQueryOption : IQueryOptionWriter<TNext, TFragment, TQueryParameter, TQueryValue, TQueryOption>, allows ref struct
    {
        RefTaskV3<TQueryOption> Commit();
        RefTaskV3<TQueryOption> Commit(QueryValue queryValue);
    }

    public interface IFragmentWriter<TNext>
        where TNext : allows ref struct
    {
        RefTaskV3<TNext> Commit(Fragment fragment);
    }
}
