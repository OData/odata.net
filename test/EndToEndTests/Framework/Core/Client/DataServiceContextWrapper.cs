//---------------------------------------------------------------------
// <copyright file="DataServiceContextWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData.Client;

    /// <summary>
    /// Test wrapper for the product DataServiceContext class.
    /// </summary>
    /// <typeparam name="TContext">The context type being wrapped.</typeparam>
    public class DataServiceContextWrapper<TContext> where TContext : DataServiceContext
    {
        private readonly TContext wrappedInstance;

        /// <summary>
        /// Initializes a new instance of the DataServiceContextWrapper class.
        /// </summary>
        /// <param name="instance">The underlying instance being wrapped.</param>
        public DataServiceContextWrapper(TContext instance)
        {
            this.wrappedInstance = instance;
        }

        #region DataServiceContext EventHandlers
        /// <summary>
        /// Wrapper entry for DataServiceContext.SendingRequest2 event handler.
        /// </summary>
        public event EventHandler<SendingRequest2EventArgs> SendingRequest2
        {
            add { this.wrappedInstance.SendingRequest2 += value; }
            remove { this.wrappedInstance.SendingRequest2 -= value; }
        }

        /// <summary>
        /// Wrapper entry for DataServiceContext.BuildingRequest event handler.
        /// </summary>
        public event EventHandler<BuildingRequestEventArgs> BuildingRequest
        {
            add { this.wrappedInstance.BuildingRequest += value; }
            remove { this.wrappedInstance.BuildingRequest -= value; }
        }

        /// <summary>
        /// Wrapper entry for DataServiceContext.ReceivingResponse event handler.
        /// </summary>
        public event EventHandler<ReceivingResponseEventArgs> ReceivingResponse
        {
            add { this.wrappedInstance.ReceivingResponse += value; }
            remove { this.wrappedInstance.ReceivingResponse -= value; }
        }

        #endregion

        /// <summary>
        /// Gets the underlying instance being wrapped.
        /// </summary>
        public TContext Context
        {
            get { return this.wrappedInstance; }
        }

        /// <summary>
        /// Gets or sets the label that identifies this context for reporting purposes.
        /// </summary>
        public string ContextLabel { get; set; }

        #region DataServiceContext Properties
        /// <summary>
        /// Gets the DataServiceContext.Format property.
        /// </summary>
        public DataServiceClientFormat Format
        {
            get { return this.wrappedInstance.Format; }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether the DataServiceContext.IgnoreMissingProperties property is set.
        ///// </summary>
        //public UndeclaredPropertyBehavior UndeclaredPropertyBehavior
        //{
        //    get { return this.wrappedInstance.UndeclaredPropertyBehavior; }
        //    set { this.wrappedInstance.UndeclaredPropertyBehavior = value; }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether the DataServiceContext.IgnoreResourceNotFoundException property is set.
        /// </summary>
        public bool IgnoreResourceNotFoundException
        {
            get { return this.wrappedInstance.IgnoreResourceNotFoundException; }
            set { this.wrappedInstance.IgnoreResourceNotFoundException = value; }
        }

        /// <summary>
        /// Gets or sets the DataServiceContext.MergeOption value
        /// </summary>
        public MergeOption MergeOption
        {
            get { return this.wrappedInstance.MergeOption; }
            set { this.wrappedInstance.MergeOption = value; }
        }
       
        /// <summary>
        /// Gets or sets the DataServiceContext.ResolveName property.
        /// </summary>
        public Func<Type, string> ResolveName
        {
            get { return this.wrappedInstance.ResolveName; }
            set { this.wrappedInstance.ResolveName = value; }
        }

        /// <summary>
        /// Gets or sets the DataServiceContext.ResolveType property.
        /// </summary>
        public Func<string, Type> ResolveType
        {
            get { return this.wrappedInstance.ResolveType; }
            set { this.wrappedInstance.ResolveType = value; }
        }

        /// <summary>
        /// Gets or sets the DataServiceContext.UrlKeyDelimiter property.
        /// </summary>
        public DataServiceUrlKeyDelimiter UrlKeyDelimiter
        {
            get { return this.wrappedInstance.UrlKeyDelimiter; }
            set { this.wrappedInstance.UrlKeyDelimiter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DataServiceContext.UsePostTunneling property is set.
        /// </summary>
        public bool UsePostTunneling
        {
            get { return this.wrappedInstance.UsePostTunneling; }
            set { this.wrappedInstance.UsePostTunneling = value; }
        }

        /// <summary>
        /// Gets the DataServiceContext.Configurations
        /// </summary>
        public DataServiceClientConfigurations Configurations
        {
            get { return this.wrappedInstance.Configurations; }
        }

        #endregion

        /// <summary>
        /// Override the ToString method to return the ContextLabel if it has been set.
        /// </summary>
        /// <returns>The context label if set, otherwise default ToString value.</returns>
        public override string ToString()
        {
            return this.ContextLabel ?? base.ToString();
        }

        #region DataServiceContext Methods
        /// <summary>
        /// Wrapper entry for the DataServiceContext.AddLink method.
        /// </summary>
        /// <param name="source">Source object participating in the link.</param>
        /// <param name="sourceProperty">The name of the property on the source object which represents a link from the source to the target object.</param>
        /// <param name="target">The target object involved in the link which is bound to the source object also specified in this call.</param>
        public void AddLink(object source, string sourceProperty, object target)
        {
            this.wrappedInstance.AddLink(source, sourceProperty, target);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.AddObject method.
        /// </summary>
        /// <param name="entitySetName">EntitySet for the object to be added.</param>
        /// <param name="entity">entity graph to add</param>
        public void AddObject(string entitySetName, object entity)
        {
            this.wrappedInstance.AddObject(entitySetName, entity);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.AddRelatedObject method.
        /// </summary>
        /// <param name="source">the parent object which is already tracked by the context.</param>
        /// <param name="sourceProperty">The name of the navigation property which forms the association between the source and target.</param>
        /// <param name="target">the target object which needs to be added.</param>
        public void AddRelatedObject(object source, string sourceProperty, object target)
        {
            this.wrappedInstance.AddRelatedObject(source, sourceProperty, target);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.AttachTo method.
        /// </summary>
        /// <param name="entitySetName">EntitySet for the object to be attached.</param>        
        /// <param name="entity">entity graph to attach</param>
        public void AttachTo(string entitySetName, object entity)
        {
            this.wrappedInstance.AttachTo(entitySetName, entity);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.AttachTo method.
        /// </summary>
        /// <param name="entitySetName">EntitySet for the object to be attached.</param>        
        /// <param name="entity">entity graph to attach</param>
        /// <param name="etag">The entity's etag.</param>
        public void AttachTo(string entitySetName, object entity, string etag)
        {
            this.wrappedInstance.AttachTo(entitySetName, entity, etag);
        }

        /// <summary>Wrapper entry for the DataServiceContext.BeginExecute method.</summary>
        /// <typeparam name="T">element type of the result</typeparam>
        /// <param name="requestUri">request to execute</param>
        /// <param name="callback">User callback when results from execution are available.</param>
        /// <param name="state">user state in IAsyncResult</param>
        /// <returns>async result object</returns>
        public IAsyncResult BeginExecute<T>(Uri requestUri, AsyncCallback callback, object state)
        {
            return this.wrappedInstance.BeginExecute<T>(requestUri, callback, state);
        }

        /// <summary>Wrapper entry for the DataServiceContext.BeginExecute method.</summary>
        /// <param name="requestUri">request to execute</param>
        /// <param name="callback">User callback when results from execution are available.</param>
        /// <param name="state">user state in IAsyncResult</param>
        /// <param name="httpMethod">HttpMethod to use. Only GET or POST are supported.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>async result object</returns>
        public IAsyncResult BeginExecute(Uri requestUri, AsyncCallback callback, object state, string httpMethod, params OperationParameter[] operationParameters)
        {
            return this.wrappedInstance.BeginExecute(requestUri, callback, state, httpMethod, operationParameters);
        }

        /// <summary>Wrapper entry for the DataServiceContext.BeginExecute method.</summary>
        /// <typeparam name="T">element type of the result</typeparam>
        /// <param name="requestUri">request to execute</param>
        /// <param name="callback">User callback when results from execution are available.</param>
        /// <param name="state">user state in IAsyncResult</param>
        /// <param name="httpMethod">HttpMethod to use.</param>
        /// <param name="singleResult">If set to true, indicates that a single result is expected as a response.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>async result object</returns>
        public IAsyncResult BeginExecute<T>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            return this.wrappedInstance.BeginExecute<T>(requestUri, callback, state, httpMethod, singleResult, operationParameters);
        }

        /// <summary>Wrapper entry for the DataServiceContext.BeginExecute method.</summary>
        /// <typeparam name="T">element type of the result</typeparam>
        /// <param name="requestUri">request to execute</param>
        /// <param name="callback">User callback when results from execution are available.</param>
        /// <param name="state">user state in IAsyncResult</param>
        /// <param name="httpMethod">HttpMethod to use.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>async result object</returns>
        public IAsyncResult BeginExecute<T>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, params OperationParameter[] operationParameters)
        {
            return this.wrappedInstance.BeginExecute<T>(requestUri, callback, state, httpMethod, operationParameters);
        }

        /// <summary>Wrapper entry for the DataServiceContext.BeginExecute method.</summary>
        /// <typeparam name="T">Element type of the result</typeparam>
        /// <param name="continuation">Continutation to execute.</param>
        /// <param name="callback">User callback when results from execution are available.</param>
        /// <param name="state">user state in IAsyncResult</param>
        /// <returns>async result object</returns>
        public IAsyncResult BeginExecute<T>(DataServiceQueryContinuation<T> continuation, AsyncCallback callback, object state)
        {
            return this.wrappedInstance.BeginExecute<T>(continuation, callback, state);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.BeginExecuteBatch method.
        /// </summary>
        /// <param name="callback">User callback when results from batch are available.</param>
        /// <param name="state">user state in IAsyncResult</param>
        /// <param name="queries">queries to batch</param>
        /// <returns>async result object</returns>
        public IAsyncResult BeginExecuteBatch(AsyncCallback callback, object state, DataServiceRequest[] queries)
        {
            return this.wrappedInstance.BeginExecuteBatch(callback, state, queries);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.BeginLoadProperty method.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="propertyName">name of collection or reference property to load</param>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        public IAsyncResult BeginLoadProperty(object entity, string propertyName, AsyncCallback callback, object state)
        {
            return this.wrappedInstance.BeginLoadProperty(entity, propertyName, callback, state);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.BeginLoadProperty method.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="propertyName">name of collection or reference property to load</param>
        /// <param name="nextLinkUri">load the page from this URI</param>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        public IAsyncResult BeginLoadProperty(object entity, string propertyName, Uri nextLinkUri, AsyncCallback callback, object state)
        {
            return this.wrappedInstance.BeginLoadProperty(entity, propertyName, nextLinkUri, callback, state);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.BeginLoadProperty method.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="propertyName">name of collection or reference property to load</param>
        /// <param name="continuation">Continuation from which the property should be loaded.</param>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        public IAsyncResult BeginLoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation, AsyncCallback callback, object state)
        {
            return this.wrappedInstance.BeginLoadProperty(entity, propertyName, continuation, callback, state);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.BeginSaveChanges method.
        /// </summary>
        /// <param name="callback">The callback</param>
        /// <param name="state">The state</param>
        /// <returns>async result</returns>
        public IAsyncResult BeginSaveChanges(AsyncCallback callback, object state)
        {
            return this.wrappedInstance.BeginSaveChanges(callback, state);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.BeginSaveChanges method.
        /// </summary>
        /// <param name="options">options on how to save changes</param>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        public IAsyncResult BeginSaveChanges(SaveChangesOptions options, AsyncCallback callback, object state)
        {
            return this.wrappedInstance.BeginSaveChanges(options, callback, state);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.CancelRequest method.
        /// </summary>
        /// <param name="asyncResult">the current async request to cancel</param>
        public void CancelRequest(IAsyncResult asyncResult)
        {
            this.wrappedInstance.CancelRequest(asyncResult);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.ChangeState method.
        /// </summary>        
        /// <param name="entity">entity whose state is to be changed</param>
        /// <param name="state">State to be applied</param>
        public void ChangeState(object entity, EntityStates state)
        {
            this.wrappedInstance.ChangeState(entity, state);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.CreateQuery method.
        /// </summary>
        /// <typeparam name="T">type of object to materialize</typeparam>
        /// <param name="entitySetName">The entitySet's name</param>
        /// <returns>composible, enumerable query object</returns>
        public DataServiceQuery<T> CreateQuery<T>(string entitySetName)
        {
            return this.wrappedInstance.CreateQuery<T>(entitySetName);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.DeleteLink method.
        /// </summary>
        /// <param name="source">Source object participating in the link.</param>
        /// <param name="sourceProperty">The name of the property on the source object which represents a link from the source to the target object.</param>
        /// <param name="target">The target object involved in the link which is bound to the source object also specified in this call.</param>
        public void DeleteLink(object source, string sourceProperty, object target)
        {
            this.wrappedInstance.DeleteLink(source, sourceProperty, target);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.DeleteObject method.
        /// </summary>
        /// <param name="entity">entity to be mark deleted</param>
        public void DeleteObject(object entity)
        {
            this.wrappedInstance.DeleteObject(entity);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.DetachLink method.
        /// </summary>
        /// <param name="source">The source object participating in the link to be marked for deletion.</param>
        /// <param name="sourceProperty">The name of the property on the source object that represents the source in the link between the source and the target.</param>
        /// <param name="target">The target object participating in the link to be marked for deletion.</param>
        /// <returns>Returns true if the specified entity was detached; otherwise false.</returns>
        public bool DetachLink(object source, string sourceProperty, object target)
        {
            return this.wrappedInstance.DetachLink(source, sourceProperty, target);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.Detach method.
        /// </summary>
        /// <param name="entity">entity to detach.</param>
        /// <returns>true if object was detached</returns>
        public bool Detach(object entity)
        {
            return this.wrappedInstance.Detach(entity);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.ExecuteBatch method.
        /// </summary>
        /// <param name="param">requests to execute</param>
        /// <returns>batch response from which query results can be enumerated.</returns>
        public DataServiceResponse ExecuteBatch(DataServiceRequest[] param)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.ExecuteBatch(param);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.CreateQuery method.
        /// </summary>
        /// <typeparam name="T">element type of the result</typeparam>
        /// <param name="asyncResult">async result object returned from BeginExecute</param>
        /// <returns>response from the request which can be enumerated.</returns>
        public IEnumerable<T> EndExecute<T>(IAsyncResult asyncResult)
        {
            return this.wrappedInstance.EndExecute<T>(asyncResult);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.CreateQuery method.
        /// </summary>
        /// <param name="asyncResult">async result object returned from BeginExecute</param>
        /// <returns>response from the request which can be enumerated.</returns>
        public OperationResponse EndExecute(IAsyncResult asyncResult)
        {
            return this.wrappedInstance.EndExecute(asyncResult);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.EndExecuteBatch method.
        /// </summary>
        /// <param name="asyncResult">async result object returned from BeginExecuteBatch</param>
        /// <returns>batch response from which query results can be enumerated.</returns>
        public DataServiceResponse EndExecuteBatch(IAsyncResult asyncResult)
        {
            return this.wrappedInstance.EndExecuteBatch(asyncResult);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.EndLoadProperty method.
        /// </summary>
        /// <param name="asyncResult">async result generated by BeginLoadProperty</param>
        /// <returns>QueryOperationResponse instance containing information about the response.</returns>
        public QueryOperationResponse EndLoadProperty(IAsyncResult asyncResult)
        {
            return this.wrappedInstance.EndLoadProperty(asyncResult);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.EndSaveChanges method.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response. </param>
        /// <returns>changeset response</returns>
        public DataServiceResponse EndSaveChanges(IAsyncResult asyncResult)
        {
            return this.wrappedInstance.EndSaveChanges(asyncResult);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.GetEntityDescriptor method.
        /// </summary>
        /// <param name="entity">Entity for which to find the entity descriptor</param>
        /// <returns>EntityDescriptor for the <paramref name="entity"/> or null if not found</returns>
        public EntityDescriptor GetEntityDescriptor(object entity)
        {
            return this.wrappedInstance.GetEntityDescriptor(entity);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.GetReadStream method.
        /// </summary>
        /// <param name="entity">The entity which is the Media Link Entry for the requested Media Resource. Thist must specify
        /// a tracked entity in a non-added state.</param>
        /// <returns>An instance of <see cref="DataServiceStreamResponse"/> which represents the response.</returns>
        public DataServiceStreamResponse GetReadStream(object entity)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.GetReadStream(entity);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.GetReadStream method.
        /// </summary>
        /// <param name="entity">The entity which contains the stream with the given name. This must specify
        /// a tracked entity in a non-added state.</param>
        /// <param name="name">name of the stream.</param>
        /// <param name="args">Instance of <see cref="DataServiceRequestArgs"/> class with additional metadata for the request.
        /// Must not be null.</param>
        /// <returns>An instance of <see cref="DataServiceStreamResponse"/> which represents the response.</returns>
        public DataServiceStreamResponse GetReadStream(object entity, string name, DataServiceRequestArgs args)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.GetReadStream(entity, name, args);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.GetReadStreamUri method.
        /// </summary>
        /// <param name="entity">The entity to lookup the MR URI for.</param>
        /// <returns>The read URI of the MR.</returns>
        public Uri GetReadStreamUri(object entity)
        {
            return this.wrappedInstance.GetReadStreamUri(entity);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.Execute method.
        /// </summary>
        /// <typeparam name="TElement">element type of the result</typeparam>
        /// <param name="requestUri">request uri to execute</param>
        /// <returns>response from the request which can be enumerated or downcast to OperationResponse."/></returns>
        public IEnumerable<TElement> Execute<TElement>(Uri requestUri)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.Execute<TElement>(requestUri);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.Execute method.
        /// </summary>
        /// <typeparam name="TElement">Element type for response.</typeparam>
        /// <param name="continuation">Continuation for query to execute.</param>
        /// <returns>The response for the specified <paramref name="continuation"/>.</returns>
        public QueryOperationResponse<TElement> Execute<TElement>(DataServiceQueryContinuation<TElement> continuation) 
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.Execute(continuation);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.Execute method.
        /// </summary>
        /// <param name="requestUri">request uri to execute</param>
        /// <param name="httpMethod">HttpMethod to use. Must be GET or POST only.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>A QueryOperationResponse with nothing to enumerate but holds other response information.</returns>
        public OperationResponse Execute(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.Execute(requestUri, httpMethod, operationParameters);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.Execute method.
        /// </summary>
        /// <typeparam name="TElement">element type of the result</typeparam>
        /// <param name="requestUri">request uri to execute</param>
        /// <param name="httpMethod">HttpMethod to use. Must be GET or POST only.</param>
        /// <param name="singleResult">Whether to expect single result.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>A IEnumerable holiding response information.</returns>
        public IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.Execute<TElement>(requestUri, httpMethod, singleResult, operationParameters);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.Execute method.
        /// </summary>
        /// <typeparam name="TElement">element type of the result</typeparam>
        /// <param name="requestUri">request uri to execute</param>
        /// <param name="httpMethod">HttpMethod to use. Must be GET or POST only.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>A IEnumerable holiding response information.</returns>
        public IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.Execute<TElement>(requestUri, httpMethod, operationParameters);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.LoadProperty method.
        /// </summary>
        /// <param name="entity">The entity to load property for.</param>
        /// <param name="propertyName">name of collection or reference property to load</param>
        /// <returns>QueryOperationResponse instance containing information about the response.</returns>
        public QueryOperationResponse LoadProperty(object entity, string propertyName)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.LoadProperty(entity, propertyName);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.LoadProperty method.
        /// </summary>
        /// <param name="entity">The entity to load property for.</param>
        /// <param name="propertyName">Name of collection or reference property to load.</param>
        /// <param name="continuation">The continuation object from a previous response.</param>
        /// <returns>QueryOperationResponse instance containing information about the response.</returns>
        public QueryOperationResponse LoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.LoadProperty(entity, propertyName, continuation);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.LoadProperty method.
        /// </summary>
        /// <typeparam name='T'>Element type of collection to load.</typeparam>
        /// <param name="entity">The entity to load property for.</param>
        /// <param name="propertyName">name of collection or reference property to load</param>
        /// <param name="continuation">The continuation object from a previous response.</param>
        /// <returns>QueryOperationResponse instance containing information about the response.</returns>
        public QueryOperationResponse<T> LoadProperty<T>(object entity, string propertyName, DataServiceQueryContinuation<T> continuation)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.LoadProperty(entity, propertyName, continuation);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.SaveChanges method.
        /// </summary>
        /// <returns>changeset response</returns>
        public DataServiceResponse SaveChanges()
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.SaveChanges();
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.SaveChanges method.
        /// </summary>
        /// <param name="options">options on how to save changes</param>
        /// <returns>changeset response</returns>
        public DataServiceResponse SaveChanges(SaveChangesOptions options)
        {
#if SILVERLIGHT || PORTABLELIB || (NETCOREAPP1_0 || NETCOREAPP2_0)
            throw new NotImplementedException();
#else
            return this.wrappedInstance.SaveChanges(options);
#endif
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.SaveChanges method.
        /// </summary>
        /// <param name="source">Source object participating in the link.</param>
        /// <param name="sourceProperty">The name of the property on the source object which represents a link from the source to the target object.</param>
        /// <param name="target">The target object involved in the link which is bound to the source object also specified in this call.</param>
        public void SetLink(object source, string sourceProperty, object target)
        {
            this.wrappedInstance.SetLink(source, sourceProperty, target);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.SetSaveStream method.
        /// </summary>
        /// <param name="entity">The entity (MLE) for which to set the MR content.</param>
        /// <param name="stream">The stream from which to read the content. The stream will be read to its end.
        /// No Seek operation will be tried on the stream.</param>
        /// <param name="closeStream">If set to true SaveChanges will close the stream before it returns. It will close the stream
        /// even if it failed (throws) and even if it didn't get to use the stream.</param>
        /// <param name="contentType">The Content-Type header value to set for the MR request. The value is not validated
        /// in any way and it's the responsibility of the user to make sure it's a valid value for Content-Type header.</param>
        /// <param name="slug">The Slug header value to set for the MR request. The value is not validated in any way 
        /// and it's the responsibility of the user to make usre it's a valid value for Slug header.</param>
        public void SetSaveStream(object entity, Stream stream, bool closeStream, string contentType, string slug)
        {
            this.wrappedInstance.SetSaveStream(entity, stream, closeStream, contentType, slug);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.SetSaveStream method.
        /// </summary>
        /// <param name="entity">The entity for which to set the stream content.</param>
        /// <param name="name">name of the stream.</param>
        /// <param name="stream">The stream from which to read the content. The stream will be read to its end.
        /// No Seek operation will be tried on the stream.</param>
        /// <param name="closeStream">If set to true SaveChanges will close the stream before it returns. It will close the stream
        /// even if it failed (throws) and even if it didn't get to use the stream.</param>
        /// <param name="contentType">
        /// The Content-Type header value to set for the stream request. The value is not validated in any way other than that 
        /// it is not null or empty and it's the responsibility of the user to make sure it's a valid value for Content-Type header.
        /// </param>
        public void SetSaveStream(object entity, string name, Stream stream, bool closeStream, string contentType)
        {
            this.wrappedInstance.SetSaveStream(entity, name, stream, closeStream, contentType);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.SetSaveStream method.
        /// </summary>
        /// <param name="entity">The entity (MLE) for which to set the MR content.</param>
        /// <param name="name">name of the stream.</param>
        /// <param name="stream">The stream from which to read the content. The stream will be read to its end.
        /// No Seek operation will be tried on the stream.</param>
        /// <param name="closeStream">If set to true SaveChanges will close the stream before it returns. It will close the stream
        /// even if it failed (throws) and even if it didn't get to use the stream.</param>
        /// <param name="args">Instance of <see cref="DataServiceRequestArgs"/> class with additional metadata for the MR request.
        /// Must not be null.</param>
        public void SetSaveStream(object entity, string name, Stream stream, bool closeStream, DataServiceRequestArgs args)
        {
            this.wrappedInstance.SetSaveStream(entity, name, stream, closeStream, args);
        }

        /// <summary>
        /// Wrapper entry for the DataServiceContext.UpdateObject method.
        /// </summary>
        /// <param name="entity">entity to be mark for update</param>
        public void UpdateObject(object entity)
        {
            this.wrappedInstance.UpdateObject(entity);
        }
        #endregion
    }
}
