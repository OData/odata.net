using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.OData.Client
{
    public interface IDataServiceQuerySingle<TElement>
    {
        DataServiceContext Context { get; }
        bool IsComposable { get; }
        Uri RequestUri { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        string AppendRequestUri(string nextSegment);
        IAsyncResult BeginGetValue(AsyncCallback callback, object state);
        DataServiceQuerySingle<TResult> CastTo<TResult>();
        DataServiceQuery<T> CreateFunctionQuery<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters);
        DataServiceQuerySingle<T> CreateFunctionQuerySingle<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters);
        TElement EndGetValue(IAsyncResult asyncResult);
        DataServiceQuerySingle<TElement> Expand(string path);
        DataServiceQuerySingle<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor);
        string GetPath(string nextSegment);
#if !PORTABLELIB
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        TElement GetValue();
#endif
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Task<TElement> GetValueAsync();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Select")]
        DataServiceQuerySingle<TResult> Select<TResult>(Expression<Func<TElement, TResult>> selector);
    }
}