//---------------------------------------------------------------------
// <copyright file="IDataServiceQueryOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IDataServiceQuery<TElement>
    {
        DataServiceContext Context { get; }
        Type ElementType { get; }
        Expression Expression { get; }
        bool IsComposable { get; }
        IQueryProvider Provider { get; }
        Uri RequestUri { get; }

        DataServiceQuery<TElement> AddQueryOption(string name, object value);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        string AppendRequestUri(string nextSegment);
        IAsyncResult BeginExecute(AsyncCallback callback, object state);
        DataServiceQuery<T> CreateFunctionQuery<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters);
        DataServiceQuerySingle<T> CreateFunctionQuerySingle<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters);
        IEnumerable<TElement> EndExecute(IAsyncResult asyncResult);
#if !PORTABLELIB
        IEnumerable<TElement> Execute();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<TElement> GetAllPages();
#endif
        Task<IEnumerable<TElement>> ExecuteAsync();
        DataServiceQuery<TElement> Expand(string path);
        DataServiceQuery<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Task<IEnumerable<TElement>> GetAllPagesAsync();
        IEnumerator<TElement> GetEnumerator();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
        string GetKeyPath(string keyString);
        string GetPath(string nextSegment);
        DataServiceQuery<TElement> IncludeTotalCount();
        string ToString();
    }
}