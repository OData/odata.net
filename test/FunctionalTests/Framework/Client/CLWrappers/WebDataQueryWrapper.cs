//---------------------------------------------------------------------
// <copyright file="WebDataQueryWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.OData.Client;
using System.Collections;

namespace System.Data.Test.Astoria
{
    /// <summary>
    /// Wrapper for DataServiceQuery
    /// </summary>
    public abstract class WebDataQueryWrapper
    {
        private Microsoft.OData.Client.DataServiceQuery _DataServiceQuery = null;
        public int InstanceID { get; set; }
        public WebDataQueryWrapper(DataServiceQuery query)
        {
            this._DataServiceQuery = query;
        }

        public System.Uri RequestUri
        {
            get
            {
                return this._DataServiceQuery.RequestUri;
            }
        }

        public System.Type ElementType
        {
            get
            {
                return this._DataServiceQuery.ElementType;
            }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this._DataServiceQuery.Expression;
            }
        }

        public System.Linq.IQueryProvider Provider
        {
            get
            {
                return this._DataServiceQuery.Provider;
            }
        }

        public virtual DataServiceQuery DataServiceQuery
        {
            get { return this._DataServiceQuery; }
        }
    }

    public class WebDataQueryWrapper<T> : WebDataQueryWrapper, IQueryable<T>
    {

        public Microsoft.OData.Client.DataServiceQuery<T> _WebDataQuery = null;

        public WebDataQueryWrapper(DataServiceQuery<T> query)
            : base(query)
        {
            this._WebDataQuery = query;
        }


        public Microsoft.OData.Client.DataServiceQuery<T> AddQueryOption(String name, Object value)
        {
            return this._WebDataQuery.AddQueryOption(name, value);
        }

        public Microsoft.OData.Client.DataServiceQuery<T> Expand(String path)
        {
            return this._WebDataQuery.Expand(path);
        }

        public System.IAsyncResult BeginExecute(AsyncCallback callback, Object state)
        {
            return this._WebDataQuery.BeginExecute(callback, state);
        }

        public IEnumerable<T> EndExecute(IAsyncResult asyncResult)
        {
            return SocketExceptionHandler.Execute(() => this._WebDataQuery.EndExecute(asyncResult));
        }

        public IEnumerable<T> Execute()
        {
            return SocketExceptionHandler.Execute(() => this._WebDataQuery.Execute());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return SocketExceptionHandler.Execute(() => this._WebDataQuery.Execute().GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return SocketExceptionHandler.Execute(() => this._WebDataQuery.GetEnumerator());
        }

        public Microsoft.OData.Client.DataServiceQuery<T> IncludeCount()
        {
            return this._WebDataQuery.IncludeCount();
        }

        public long LongCount()
        {
            return this._WebDataQuery.LongCount();
        }

        public int Count()
        {
            return this._WebDataQuery.Count();
        }
    }

    public class WebDataQueryGenericWrapper : IQueryable
    {
        public object _webDataQuery = null;

        public WebDataQueryGenericWrapper(object query)
        {
            this._webDataQuery = query;
        }

        public System.Type ElementType
        {
            get
            {
                PropertyInfo pi = _webDataQuery.GetType().GetProperty("ElementType");
                return (Type)pi.GetValue(_webDataQuery, new object[] { });
            }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                PropertyInfo pi = _webDataQuery.GetType().GetProperty("Expression");
                return (System.Linq.Expressions.Expression)pi.GetValue(_webDataQuery, new object[] { });
            }
        }

        public System.Linq.IQueryProvider Provider
        {
            get
            {
                PropertyInfo pi = _webDataQuery.GetType().GetProperty("Provider");
                return (System.Linq.IQueryProvider)pi.GetValue(_webDataQuery, new object[] { });
            }
        }

        public System.IAsyncResult BeginExecute(AsyncCallback callback, Object state)
        {
            MethodInfo mi = _webDataQuery.GetType().GetMethod("BeginExecute");
            object o = mi.Invoke(_webDataQuery, new object[] { callback, state });
            return (System.IAsyncResult)o;
        }

        public System.Collections.IEnumerable EndExecute(IAsyncResult asyncResult)
        {
            MethodInfo mi = _webDataQuery.GetType().GetMethod("EndExecute");
            object o = mi.Invoke(_webDataQuery, new object[] { asyncResult });
            return (System.Collections.IEnumerable)o;
        }

        public System.Collections.IEnumerable Execute()
        {
            MethodInfo mi = _webDataQuery.GetType().GetMethod("Execute");
            object o = mi.Invoke(_webDataQuery, new object[] { });
            return (System.Collections.IEnumerable)o;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            MethodInfo mi = _webDataQuery.GetType().GetMethod("GetEnumerator");
            object o = mi.Invoke(_webDataQuery, new object[] { });
            return (System.Collections.IEnumerator)o;
        }
    }

    /// <summary>
    /// Wrapper for DataServiceRequest
    /// </summary>
    public abstract class DataServiceRequestWrapper
    {
        public abstract long ActualCount { get; set; }
        public abstract System.Uri RequestUri
        {
            get;
        }

        public abstract System.Type ElementType
        {
            get;
        }

        public abstract DataServiceRequest DataServiceRequest
        {
            get;
        }
    }

    public class DataServiceRequestWrapper<T> : DataServiceRequestWrapper
    {
        private Microsoft.OData.Client.DataServiceRequest<T> _DataServiceRequest;

        public DataServiceRequestWrapper(System.Uri uri)
        {
            this._DataServiceRequest = new Microsoft.OData.Client.DataServiceRequest<T>(uri);
        }

        public override long ActualCount
        {
            get
            ;
            set;
        }
        public override System.Uri RequestUri
        {
            get
            {
                return this._DataServiceRequest.RequestUri;
            }
        }

        public override System.Type ElementType
        {
            get
            {
                return this._DataServiceRequest.ElementType;
            }
        }

        public override DataServiceRequest DataServiceRequest
        {
            get { return this._DataServiceRequest; }
        }
    }

    /// <summary>
    /// Wrapper for DataServiceResponse
    /// </summary>
    public class DataServiceResponseWrapper : IEnumerable<OperationResponseWrapper>
    {
        private Microsoft.OData.Client.DataServiceResponse _DataServiceResponse = null;
        public int InstanceID;
        public DataServiceResponseWrapper(DataServiceResponse response)
        {
            this._DataServiceResponse = response;
        }
        public DataServiceResponseWrapper(int InstanceID)
        {
            this.InstanceID = InstanceID;
        }
        public System.Collections.Generic.IDictionary<string, string> BatchHeaders
        {
            get
            {
                return this._DataServiceResponse.BatchHeaders;
            }
        }

        public int BatchStatusCode
        {
            get
            {
                return this._DataServiceResponse.BatchStatusCode;
            }
        }

        public bool IsBatchResponse
        {
            get
            {
                return this._DataServiceResponse.IsBatchResponse;
            }
        }

        private QueryOperationResponseWrapper ConvertToTyped(QueryOperationResponse qoResponse)
        {
            Type entityType = qoResponse.GetType().GetGenericArguments()[0];
            ConstructorInfo qorTypeConstructor = typeof(QueryOperationResponseWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(QueryOperationResponse<>).MakeGenericType(entityType) });
            return qorTypeConstructor.Invoke(new object[] { qoResponse }) as QueryOperationResponseWrapper;
        }

        public System.Collections.Generic.IEnumerator<OperationResponseWrapper> GetEnumerator()
        {
            foreach (OperationResponse op in this._DataServiceResponse)
            {
                if (op is QueryOperationResponse)
                {
                    yield return ConvertToTyped((QueryOperationResponse)op);
                }
                else
                {
                    yield return new ChangesetResponseWrapper((ChangeOperationResponse)op);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }


    /// <summary>
    /// Wrapper for OperationResponse
    /// </summary>
    public abstract class OperationResponseWrapper
    {
        private Microsoft.OData.Client.OperationResponse _OperationResponse = null;

        protected OperationResponseWrapper(Microsoft.OData.Client.OperationResponse or)
        {
            _OperationResponse = or;
        }

        public Exception Error
        {
            get
            {
                return this._OperationResponse.Error;
            }
        }

        public System.Collections.Generic.IDictionary<string, string> Headers
        {
            get
            {
                return this._OperationResponse.Headers;
            }
        }

        public int StatusCode
        {
            get
            {
                return this._OperationResponse.StatusCode;
            }
        }
    }

    /// <summary>
    /// Wrapper for ChangesetResponse
    /// </summary>
    public class ChangesetResponseWrapper : OperationResponseWrapper
    {
        private Microsoft.OData.Client.ChangeOperationResponse _ChangesetResponse = null;

        public ChangesetResponseWrapper(ChangeOperationResponse cr)
            : base(cr)
        {
            this._ChangesetResponse = cr;
        }
        public DescriptorWrapper Descriptor
        {
            get
            {
                if (this._ChangesetResponse.Descriptor is LinkDescriptor)
                {
                    return new LinkDescriptorWrapper((LinkDescriptor)this._ChangesetResponse.Descriptor);
                }
                else
                {
                    return new EntityDescriptorWrapper((EntityDescriptor)this._ChangesetResponse.Descriptor);
                }
            }
        }
    }

    /// <summary>
    /// Wrapper for QueryResponse
    /// </summary>
    public class QueryOperationResponseWrapper : OperationResponseWrapper, System.Collections.IEnumerable
    {
        public readonly Microsoft.OData.Client.QueryOperationResponse _QueryResponse = null;

        public QueryOperationResponseWrapper(QueryOperationResponse qr)
            : base(qr)
        {
            this._QueryResponse = qr;
        }

        public Microsoft.OData.Client.DataServiceRequest Query
        {
            get
            {
                return this._QueryResponse.Query;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this._QueryResponse).GetEnumerator();
        }

        public virtual long GetTotalCount
        {
            get
            {
                return this._QueryResponse.Count;
            }
        }
        //public Uri GetNextLinkUri(IEnumerable results)
        //{
        //    return GetNextLinkUri(results
        //}
        public DataServiceQueryContinuation GetNextLinkUri(ICollection collection)
        {
            return GetNextLinkUri(collection, true);
        }

        public DataServiceQueryContinuation GetContinuation()
        {
            return this._QueryResponse.GetContinuation();
        }


        public Uri GetNextLinkUri()
        {
            var continuationToken = this._QueryResponse.GetContinuation();
            return continuationToken != null ? continuationToken.NextLinkUri : null;
        }

        public DataServiceQueryContinuation GetNextLinkUri(ICollection collection, bool catchException)
        {
            DataServiceQueryContinuation nextLink = null;
            try
            {
                nextLink = this._QueryResponse.GetContinuation(collection);
            }
            catch (ArgumentNullException arge)
            {
                if (catchException)
                {
                    AstoriaTestLog.WriteLine(arge.Message);
                    nextLink = null;
                }
                else
                {
                    throw;
                }
            }
            return nextLink;
        }

    }

    /// <summary>
    /// Wrapper for QueryOperationResponse<T>
    /// </summary>
    public class QueryOperationResponseWrapper<T> : QueryOperationResponseWrapper, IEnumerable<T>
    {
        private Microsoft.OData.Client.QueryOperationResponse<T> _QueryOpResponse = null;

        public QueryOperationResponseWrapper(QueryOperationResponse<T> qr)
            : base(qr)
        {
            this._QueryOpResponse = qr;
        }

        public QueryOperationResponseWrapper(QueryOperationResponseWrapper qr)
            : base(qr._QueryResponse)
        {
            this._QueryOpResponse = (QueryOperationResponse<T>)qr._QueryResponse;
        }

        public override long GetTotalCount
        {
            get
            {
                return this._QueryOpResponse.Count;
            }
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return this._QueryOpResponse.GetEnumerator();
        }
    }
}
