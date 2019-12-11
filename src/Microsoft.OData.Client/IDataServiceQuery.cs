using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.OData.Client
{
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