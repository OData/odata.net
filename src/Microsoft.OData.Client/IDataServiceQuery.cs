//---------------------------------------------------------------------
// <copyright file="IDataServiceQuery.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IDataServiceQuery
    {
        Expression Expression { get; }
        IQueryProvider Provider { get; }

        IAsyncResult BeginExecute(AsyncCallback callback, object state);
        IEnumerable EndExecute(IAsyncResult asyncResult);
#if !PORTABLELIB
        IEnumerable Execute();
#endif
        Task<IEnumerable> ExecuteAsync();
    }
}