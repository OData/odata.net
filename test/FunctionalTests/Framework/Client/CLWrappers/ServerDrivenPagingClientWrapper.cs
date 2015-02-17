//---------------------------------------------------------------------
// <copyright file="ServerDrivenPagingClientWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;
    using System.Linq;
    using System.Reflection;

    #endregion Namespaces

    public abstract class ServerDrivenPagingClientWrapper : IEnumerable
    {
        public static IEnumerable CreatePagedEnumerator(Type enumerableType, IEnumerable results)
        {
            ConstructorInfo pagedEnumerableConstruct = typeof(PagedEnumerable<>).MakeGenericType(enumerableType).GetConstructor(new Type[] { typeof(IEnumerable) });
            return pagedEnumerableConstruct.Invoke(new object[] { results }) as IEnumerable;
        }
#if !EXCLUDEWINDOWSBASE
        public static object CreateDerivedDSC(Type enumerableType, WebDataCtxWrapper Context, string entitySetName, IEnumerable results,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
        {
            //DataServiceContext context,
            //string entitySetName,
            //IEnumerable<T> items,
            //Func<EntityChangedParams, bool> entityChangedCallback,
            //Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            ConstructorInfo pagedEnumerableConstruct = typeof(DerivedEnumerable<>).MakeGenericType(enumerableType)
                .GetConstructor(new Type[] { typeof(DataServiceContext), typeof(string), typeof(IEnumerable<>).MakeGenericType(enumerableType) 
                    , typeof(Func<EntityChangedParams, bool>) 
                    , typeof(Func<EntityCollectionChangedParams, bool>)
                });
            object instance = pagedEnumerableConstruct.Invoke(new object[] { Context.UnderlyingContext, entitySetName, results, entityChangedCallback, collectionChangedCallback });
            return instance;
        }

        public static ServerDrivenPagingClientWrapper<T> Create<T>(IEnumerable<T> queryResults, WebDataCtxWrapper context)
        {
            return new ServerDrivenPagingClientWrapper<T>(queryResults, context, false);
        }

        public static ServerDrivenPagingClientWrapper<T> CreateTracked<T>(IEnumerable<T> queryResults, WebDataCtxWrapper context)
        {
            return new ServerDrivenPagingClientWrapper<T>(queryResults, context, true);
        }

        public static ServerDrivenPagingClientWrapper Create(Type entityType, IEnumerable QueryResults, WebDataCtxWrapper context, bool trackChanges)
        {
            ConstructorInfo constructor = typeof(ServerDrivenPagingClientWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(IEnumerable), typeof(WebDataCtxWrapper), typeof(bool) });
            return constructor.Invoke(new object[] { QueryResults, context, trackChanges }) as ServerDrivenPagingClientWrapper;
        }

        public static ServerDrivenPagingClientWrapper Create(Type entityType, object dsCollection, WebDataCtxWrapper context)
        {
            Type dsCollectionType = typeof(DataServiceCollection<>).MakeGenericType(entityType);
            ConstructorInfo constructor = typeof(ServerDrivenPagingClientWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { dsCollectionType, typeof(WebDataCtxWrapper) });
            return constructor.Invoke(new object[] { dsCollection, context }) as ServerDrivenPagingClientWrapper;
        }

        public static ServerDrivenPagingClientWrapper Create(Type entityType, WebDataCtxWrapper context, bool trackChanges)
        {
            ConstructorInfo constructor = typeof(ServerDrivenPagingClientWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(WebDataCtxWrapper), typeof(bool) });
            return constructor.Invoke(new object[] { context, trackChanges }) as ServerDrivenPagingClientWrapper;
        }
#endif
        public abstract DataServiceQueryContinuation Continuation { get; set; }

        public abstract Uri NextLinkUri { get; }

        public virtual void Load(IEnumerable qoResponse)
        {
        }

        public virtual void Detach() { }

        public virtual void Clear(bool detach)
        {
        }

        public virtual void Clear()
        {
        }
        //public virtual void Load(object Item)
        //{
        //}

        public virtual void Add(object Item)
        {
        }

        #region IEnumerable Members

        public virtual IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
#if !EXCLUDEWINDOWSBASE
    public class DerivedEnumerable
    {
        public static DerivedEnumerable<T> Create<T>()
        {
            return new DataServiceCollection<T>(null, TrackingMode.None) as DerivedEnumerable<T>;
        }
    }

    public class DerivedEnumerable<T> : DataServiceCollection<T>
    {
        public DerivedEnumerable(
            DataServiceContext context,
            string entitySetName,
            IEnumerable<T> items,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : base(context, items, TrackingMode.AutoChangeTracking, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
        }
    }
#endif

    public abstract class PagedEnumerable
    {
        protected DataServiceQueryContinuation continuation;

        //public DataServiceQueryContinuation Continuation { get; set; }
        public virtual Uri NextLinkUri { get; set; }
        public static PagedEnumerable<T> Create<T>()
        {
            return new PagedEnumerable<T>();
        }
    }

    public class PagedEnumerable<T> : PagedEnumerable, IList<T>, ICollection, IList
    {
        public DataServiceQueryContinuation<T> Continuation
        {
            get { return (DataServiceQueryContinuation<T>)base.continuation; }
            set { base.continuation = value; }
        }

        public override Uri NextLinkUri { get; set; }
        private IEnumerable innerEnumerable;
        private Collection<T> innerCollection;

        public PagedEnumerable(IEnumerable results)
        {
            innerEnumerable = results;
            innerCollection = new Collection<T>();
            InnerAddCollection(results);
        }

        public PagedEnumerable()
        {
            innerCollection = new Collection<T>();
        }

        public void Load(IEnumerable results)
        {
            InnerAddCollection(results);
            ExtractNextLinkUri(results);
        }

        public void Load(IEnumerable<T> results)
        {
            InnerAddCollection(results);
            ExtractNextLinkUri(results);
        }

        private void ExtractNextLinkUri(IEnumerable results)
        {
            this.NextLinkUri = null;
            if (results is QueryOperationResponseWrapper)
            {
                this.NextLinkUri = ((QueryOperationResponseWrapper)results)._QueryResponse.GetContinuation().NextLinkUri;
            }
            else if (results is QueryOperationResponse)
            {
                DataServiceQueryContinuation continuation = ((QueryOperationResponse)results).GetContinuation(null);
                if (continuation != null)
                    this.NextLinkUri = ((QueryOperationResponse)results).GetContinuation(null).NextLinkUri;
                else
                    this.NextLinkUri = null;
            }
        }

        private void InnerAddCollection(IEnumerable enumerable)
        {
            if (innerCollection == null)
            {
                innerCollection = new Collection<T>();
            }
            foreach (object o in enumerable)
            {
                innerCollection.Add((T)o);
            }
        }
        public PagedEnumerable(IEnumerable<T> results)
        {
            innerEnumerable = results;
            innerCollection = new Collection<T>();
            InnerAddCollection(results);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerCollection.GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)innerCollection.GetEnumerator();
        }

        #endregion


        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            innerCollection.CopyTo(array.Cast<T>().ToArray(), index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return innerCollection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            innerCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerCollection.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return innerCollection[index];
            }
            set
            {
                innerCollection[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            innerCollection.Add(item);
        }

        public void Clear()
        {
            innerCollection.Clear();
        }

        public bool Contains(T item)
        {
            return innerCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            innerCollection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return innerCollection.Remove(item);
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            innerCollection.Add((T)value);
            return innerCollection.IndexOf((T)value);
        }

        public bool Contains(object value)
        {
            return innerCollection.Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return innerCollection.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            innerCollection.Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            innerCollection.Remove((T)value);
        }

        object IList.this[int index]
        {
            get
            {
                return innerCollection[index];
            }
            set
            {
                innerCollection[index] = (T)value;
            }
        }

        #endregion
    }

    public partial class WebDataCtxWrapper
    {
        public IEnumerable LoadPropertyPage(object entity, string propertyName, Uri nextUri)
        {
            QueryOperationResponse qoLoadPropResponse = null;
            SocketExceptionHandler.Execute(() => qoLoadPropResponse = this._DataServiceContext.LoadProperty(entity, propertyName, nextUri));
            return new QueryOperationResponseWrapper(qoLoadPropResponse);
        }
        public IEnumerable LoadPropertyPage(object entity, string propertyName, DataServiceQueryContinuation continuation)
        {
            QueryOperationResponse qoLoadPropResponse = null;
            SocketExceptionHandler.Execute(() => qoLoadPropResponse = this._DataServiceContext.LoadProperty(entity, propertyName, continuation));
            return new QueryOperationResponseWrapper(qoLoadPropResponse);
        }
        public IEnumerable LoadPropertyPage<TEntity>(object entity, string propertyName, DataServiceQueryContinuation<TEntity> continuation)
        {
            QueryOperationResponse qoLoadPropResponse = null;
            SocketExceptionHandler.Execute(() => qoLoadPropResponse = this._DataServiceContext.LoadProperty(entity, propertyName, continuation));
            return new QueryOperationResponseWrapper(qoLoadPropResponse);
        }
        public IAsyncResult BeginLoadPropertyPage(object entity, string propertyName, Uri nextLinkUri, AsyncCallback callback, object state)
        {
            return this.UnderlyingContext.BeginLoadProperty(entity, propertyName, nextLinkUri, callback, state);
        }
        public IAsyncResult BeginLoadPropertyPage(object entity, string propertyName, DataServiceQueryContinuation continuationToken, AsyncCallback callback, object state)
        {
            return this.UnderlyingContext.BeginLoadProperty(entity, propertyName, continuationToken.NextLinkUri, callback, state);
        }
        public IEnumerable ExecuteOfT(Type entityType, DataServiceQueryContinuation continuationToken)
        {
            var executeMethod = typeof(DataServiceContext).GetMethods().Where(meth => meth.Name.StartsWith("Execute") && meth.GetParameters().Any(param => param.Name == "continuation")).Single();
            MethodInfo method = executeMethod.MakeGenericMethod(entityType);
            ConstructorInfo qorTypeConstructor = typeof(QueryOperationResponseWrapper<>).MakeGenericType(entityType).GetConstructor(new Type[] { typeof(QueryOperationResponse<>).MakeGenericType(entityType) });
            object qoResponse = method.Invoke(this._DataServiceContext, new object[] { continuationToken });
            return qorTypeConstructor.Invoke(new object[] { qoResponse }) as QueryOperationResponseWrapper;
        }
    }
#if !EXCLUDEWINDOWSBASE
    public class ServerDrivenPagingClientWrapper<T> : ServerDrivenPagingClientWrapper, IEnumerable<T>
    {
        public bool BindingEnabled { get; set; }
        internal DataServiceCollection<T> InnerCollection { get; set; }
        internal int ContextInstanceId { get; set; }
        private int InstanceId { get; set; }

        public ServerDrivenPagingClientWrapper(WebDataCtxWrapper context, string entitySetName, IEnumerable<T> items,
            Func<EntityChangedParams, bool> entityChangedCallback, Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
        {
            InnerCollection = new DataServiceCollection<T>(context.UnderlyingContext, items, TrackingMode.AutoChangeTracking, entitySetName, entityChangedCallback, collectionChangedCallback);
        }

        public ServerDrivenPagingClientWrapper(WebDataCtxWrapper context, string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback, Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
        {
            InnerCollection = new DataServiceCollection<T>(context.UnderlyingContext, entitySetName, entityChangedCallback, collectionChangedCallback);
        }

        public ServerDrivenPagingClientWrapper(WebDataCtxWrapper context)
        {
            ContextInstanceId = context.instance;
            InnerCollection = new DataServiceCollection<T>(context.UnderlyingContext);
            //create remote DataServiceCollection corresponding to this Context Instance
        }

        public ServerDrivenPagingClientWrapper(DataServiceCollection<T> innerCollection, WebDataCtxWrapper context)
        {
            ContextInstanceId = context.instance;
            //create remote DataServiceCollection corresponding to this Context Instance
            InnerCollection = innerCollection;
        }

        public override void Detach()
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
            }
            else
            {
                InnerCollection.Detach();
            }
        }
        public override void Clear(bool detach)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
            }
            else
            {
                this.InnerCollection.Clear(detach);
            }
        }
        public override void Clear()
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
            }
            else
            {
                this.InnerCollection.Clear();
            }
        }
        public ServerDrivenPagingClientWrapper(IEnumerable QueryResults, WebDataCtxWrapper context, bool trackChanges)
        {
            ContextInstanceId = context.instance;
            if (trackChanges)
            {
                if (AstoriaTestProperties.IsRemoteClient)
                {
                    InnerCollection = new DataServiceCollection<T>(context.UnderlyingContext, (EnumerableWrapper<T>)QueryResults, TrackingMode.AutoChangeTracking, null, null, null);
                }
                else
                {
                    InnerCollection = new DataServiceCollection<T>(context.UnderlyingContext, (IEnumerable<T>)QueryResults, TrackingMode.AutoChangeTracking, null, null, null);
                }
            }
            else
            {
                if (AstoriaTestProperties.IsRemoteClient)
                {
                    InnerCollection = new DataServiceCollection<T>((EnumerableWrapper<T>)QueryResults, TrackingMode.None);
                }
                else
                {
                    if (QueryResults is QueryOperationResponseWrapper<T>)
                    {
                        InnerCollection = new DataServiceCollection<T>(((QueryOperationResponseWrapper<T>)QueryResults)._QueryResponse as IEnumerable<T>, TrackingMode.None);
                    }
                    else
                    {
                        InnerCollection = new DataServiceCollection<T>((IEnumerable<T>)QueryResults, TrackingMode.None);
                    }
                }
            }
        }

        public override void Add(object Item)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
            }
            else
            {
                this.InnerCollection.Add((T)Item);
            }
        }

        public ServerDrivenPagingClientWrapper(WebDataCtxWrapper context, bool trackChanges)
        {
            ContextInstanceId = context.instance;
            if (trackChanges)
            {
                if (AstoriaTestProperties.IsRemoteClient)
                {
                    string[] paramS = new string[] { typeof(T).FullName };
                    SilverlightRemote.Remoter.InvokeSend(ContextInstanceId, "DataServiceCollection.CreateTracked(Ctx)", paramS);
                }
                else
                {
                    InnerCollection = new DataServiceCollection<T>(context.UnderlyingContext);
                }
            }
            else
            {
                if (AstoriaTestProperties.IsRemoteClient)
                {

                    string[] paramS = new string[] { typeof(T).FullName.Replace("NorthwindV2", "northwindClient") };
                    InstanceId = Convert.ToInt32(
                            SilverlightRemote.Remoter.InvokeSend(ContextInstanceId, "DataServiceCollection.Create()", paramS)
                            );
                }
                else
                {
                    InnerCollection = new DataServiceCollection<T>(null, TrackingMode.None);
                }
            }
        }
        public override DataServiceQueryContinuation Continuation
        {
            get { return this.InnerCollection.Continuation; }
            set { this.InnerCollection.Continuation = (DataServiceQueryContinuation<T>)value; }
        }

        public DataServiceQueryContinuation<T> ContinuationOfT
        {
            get { return this.InnerCollection.Continuation; }
            set { this.InnerCollection.Continuation = value; }
        }

        public override Uri NextLinkUri
        {
            get
            {
                var continuation = InnerCollection.Continuation;
                if (continuation == null)
                {
                    return null;
                }
                else
                {
                    return continuation.NextLinkUri;
                }
            }
        }

        public override void Load(IEnumerable qoResponse)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                string[] paramS = new string[] { 
                        typeof(T).FullName,
                        SilverlightRemote.WriteObject(qoResponse)   
                };
                SilverlightRemote.Remoter.InvokeSend(ContextInstanceId, "DataServiceCollection.Load(IEnumerable)", paramS);
            }
            else
            {
                if (qoResponse is QueryOperationResponseWrapper<T> || qoResponse is QueryOperationResponseWrapper)
                {
                    InnerCollection.Load((IEnumerable<T>)((QueryOperationResponseWrapper)qoResponse)._QueryResponse);
                }
                else if (qoResponse is WebDataQueryWrapper<T>)
                {
                    InnerCollection.Load(((WebDataQueryWrapper<T>)qoResponse)._WebDataQuery);
                }
                else
                {
                    InnerCollection.Load((IEnumerable<T>)qoResponse);
                }
                //Intermediate test hook
            }
        }

        //public override void Load(object Item)
        //{
        //    this.Load((T)Item);
        //}
        public void Load(T item)
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                string[] paramS = new string[] { 
                        typeof(T).FullName,
                        SilverlightRemote.WriteObject(item)   
                };
                SilverlightRemote.Remoter.InvokeSend(InstanceId, "DataServiceCollection.Load(T)", paramS);
            }
            else
            {
                InnerCollection.Load(item);
            }
        }


        #region IEnumerable Members

        public override IEnumerator GetEnumerator()
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                throw (new NotSupportedException("Not Suppored for Remote client"));
            }
            else
            {
                return InnerCollection.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (AstoriaTestProperties.IsRemoteClient)
            {
                throw (new NotSupportedException("Not Suppored for Remote client"));
            }
            else
            {
                return InnerCollection.GetEnumerator() as IEnumerator<T>;
            }
        }

        #endregion
    }
#endif
}
