//---------------------------------------------------------------------
// <copyright file="WrapperExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Query.Contracts;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Extension methods for Astoria-specific wrappers.
    /// </summary>
    public static class WrapperExtensionMethods
    {
        /// <summary>
        /// Creates a query using the clr type and entity set from the given query entity type
        /// </summary>
        /// <typeparam name="TElement">The wrapped element type of the query</typeparam>
        /// <param name="context">The wrapped context</param>
        /// <param name="queryEntityType">The query entity type</param>
        /// <returns>The query created using the clr type and entity set name from the query type</returns>
        public static WrappedDataServiceQuery<TElement> CreateQuery<TElement>(this WrappedDataServiceContext context, QueryEntityType queryEntityType) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(queryEntityType, "queryEntityType");
            return context.CreateQuery<TElement>(queryEntityType.ClrType, queryEntityType.EntitySet.Name);
        }

        /// <summary>
        /// Creates the data service context instance for a given type and URI and wraps it.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="serviceUri">The service URI.</param>
        /// <returns>Wrapped object context object.</returns>
        public static WrappedDataServiceContext CreateDataServiceContext(this IWrapperScope wrapperScope, Type contextType, Uri serviceUri)
        {
            return CreateDataServiceContext(wrapperScope, contextType, serviceUri, DataServiceProtocolVersion.Unspecified);
        }

        /// <summary>
        /// Creates the data service context instance for a given type and URI and wraps it.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="maxProtocolVersion">max protocol version that the client understands.</param>
        /// <returns>Wrapped object context object.</returns>
        public static WrappedDataServiceContext CreateDataServiceContext(this IWrapperScope wrapperScope, Type contextType, Uri serviceUri, DataServiceProtocolVersion maxProtocolVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(wrapperScope, "wrapperScope");
            ExceptionUtilities.CheckArgumentNotNull(contextType, "contextType");

            object dataServiceContext = null;
            if (maxProtocolVersion != DataServiceProtocolVersion.Unspecified)
            {
                dataServiceContext = Activator.CreateInstance(contextType, serviceUri, maxProtocolVersion.ToProductEnum());
            }
            else
            {
                dataServiceContext = Activator.CreateInstance(contextType, serviceUri);
            }

            return wrapperScope.Wrap<WrappedDataServiceContext>(dataServiceContext);
        }

        /// <summary>
        /// Creates a wrapped data service request
        /// </summary>
        /// <typeparam name="TElement">The wrapped element type</typeparam>
        /// <param name="wrapperScope">The wrapper scope</param>
        /// <param name="elementType">The actual element type</param>
        /// <param name="requestUri">The request uri</param>
        /// <returns>The wrapped data service request</returns>
        public static WrappedDataServiceRequest<TElement> CreateDataServiceRequest<TElement>(this IWrapperScope wrapperScope, Type elementType, Uri requestUri) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(wrapperScope, "wrapperScope");
            ExceptionUtilities.CheckArgumentNotNull(elementType, "elementType");

            var request = Activator.CreateInstance(typeof(DSClient.DataServiceRequest<>).MakeGenericType(elementType), requestUri);
            return wrapperScope.Wrap<WrappedDataServiceRequest<TElement>>(request);
        }

        /// <summary>
        /// Creates the data service collection.
        /// </summary>
        /// <typeparam name="TElement">The wrapped element type of the collection</typeparam>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="clrType">The clr type.</param>
        /// <param name="context">The data service context.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <param name="entityChangedCallback">The entity changed callback.</param>
        /// <param name="collectionChangedCallback">The collection changed callback.</param>
        /// <returns>Wrapped DataServiceCollection.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The type should be the inherited one.")]
        public static WrappedDataServiceCollection<TElement> CreateDataServiceCollection<TElement>(this IWrapperScope wrapperScope, Type clrType, WrappedDataServiceContext context, string entitySetName, Func<DSClient.EntityChangedParams, bool> entityChangedCallback, Func<DSClient.EntityCollectionChangedParams, bool> collectionChangedCallback) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(wrapperScope, "wrapperScope");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(entitySetName, "entitySetName");

            Type dataServiceCollectionType = typeof(DSClient.DataServiceCollection<>).MakeGenericType(clrType);
            ConstructorInfo constructorWithContext = dataServiceCollectionType.GetInstanceConstructor(true, new Type[] { typeof(DSClient.DataServiceContext), typeof(string), typeof(Func<DSClient.EntityChangedParams, bool>), typeof(Func<DSClient.EntityCollectionChangedParams, bool>) });
            ExceptionUtilities.CheckObjectNotNull(constructorWithContext, "Could not find constructor");

            var collection = constructorWithContext.Invoke(new object[] { context.Product, entitySetName, entityChangedCallback, collectionChangedCallback });
            return wrapperScope.Wrap<WrappedDataServiceCollection<TElement>>(collection);
        }

        /// <summary>
        /// Invokes the Count extension method on the wrapped query via reflection
        /// </summary>
        /// <typeparam name="TElement">The element type of the wrapped query</typeparam>
        /// <param name="query">The wrapped query</param>
        /// <returns>The results of the count extension method</returns>
        public static int Count<TElement>(this WrappedDataServiceQuery<TElement> query) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");

            var countMethod = typeof(Queryable).GetMethods(true, true).SingleOrDefault(m => m.Name == "Count" && m.GetParameters().Length == 1);
            ExceptionUtilities.CheckObjectNotNull(countMethod, "Could not find method 'Count' on type 'Queryable'");
            countMethod = countMethod.MakeGenericMethod(query.ElementType);

            var product = query.Product;
            return (int)countMethod.Invoke(null, new object[] { product });
        }

        /// <summary>
        /// Invokes the Take extension method on the wrapped query via reflection
        /// </summary>
        /// <typeparam name="TElement">The element type of the wrapped query</typeparam>
        /// <param name="query">The wrapped query</param>
        /// <param name="count">The number of elements to take</param>
        /// <returns>The wrapped query after calling Take</returns>
        public static WrappedDataServiceQuery<TElement> Take<TElement>(this WrappedDataServiceQuery<TElement> query, int count) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");

            var takeMethod = typeof(Queryable).GetMethod("Take", true, true);
            ExceptionUtilities.CheckObjectNotNull(takeMethod, "Could not find method 'Take' on type 'Queryable'");
            takeMethod = takeMethod.MakeGenericMethod(query.ElementType);

            var product = query.Product;
            product = takeMethod.Invoke(null, new object[] { product, count });

            return query.Scope.Wrap<WrappedDataServiceQuery<TElement>>(product);
        }

        /// <summary>
        /// Invokes the Skip extension method on the wrapped query via reflection
        /// </summary>
        /// <typeparam name="TElement">The element type of the wrapped query</typeparam>
        /// <param name="query">The wrapped query</param>
        /// <param name="count">The number of elements to skip</param>
        /// <returns>The wrapped query after calling Skip</returns>
        public static WrappedDataServiceQuery<TElement> Skip<TElement>(this WrappedDataServiceQuery<TElement> query, int count) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");

            var skipMethod = typeof(Queryable).GetMethod("Skip", true, true);
            ExceptionUtilities.CheckObjectNotNull(skipMethod, "Could not find method 'Skip' on type 'Queryable'");
            skipMethod = skipMethod.MakeGenericMethod(query.ElementType);

            var product = query.Product;
            product = skipMethod.Invoke(null, new object[] { product, count });

            return query.Scope.Wrap<WrappedDataServiceQuery<TElement>>(product);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceQuery.Execute dynamically
        /// </summary>
        /// <typeparam name="TElement">The element type of the wrapped results</typeparam>
        /// <param name="query">The query to execute</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void Execute<TElement>(this WrappedDataServiceQuery<TElement> query, IAsyncContinuation continuation, bool async, Action<WrappedIEnumerable<TElement>> onCompletion) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedIEnumerable<TElement>>(continuation, async, () => query.Execute(), c => query.BeginExecute(c, null), r => query.EndExecute(r), onCompletion);
        }
        
        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.LoadProperty dynamically
        /// </summary>
        /// <param name="context">The context to call call load property on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="entity">The entity to load a property on</param>
        /// <param name="propertyName">The name of the property to load</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void LoadProperty(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, WrappedEntityInstance entity, string propertyName, Action<WrappedQueryOperationResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedQueryOperationResponse>(continuation, async, () => context.LoadProperty(entity, propertyName), c => context.BeginLoadProperty(entity, propertyName, c, null), r => context.EndLoadProperty(r), onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.Execute dynamically
        /// </summary>
        /// <typeparam name="TElement">The element type of the wrapped query results</typeparam>
        /// <param name="context">The context to call execute on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="elementType">The element type of the query results</param>
        /// <param name="requestUri">The uri to make a request to</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void Execute<TElement>(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, Type elementType, Uri requestUri, Action<WrappedIEnumerable<TElement>> onCompletion) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedIEnumerable<TElement>>(continuation, async, () => context.Execute<TElement>(elementType, requestUri), c => context.BeginExecute<TElement>(elementType, requestUri, c, null), r => context.EndExecute<TElement>(elementType, r), onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.Execute dynamically
        /// </summary>
        /// <typeparam name="TElement">The element type of the wrapped query results</typeparam>
        /// <param name="context">The context to call execute on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="elementType">The element type of the query results</param>
        /// <param name="requestUri">The uri to make a request to</param>
        /// <param name="httpMethod">The HttpMethod to make a request</param>
        /// <param name="singleResult">Whether expect single item in result</param>
        /// <param name="operationParameters">Parameter for the request</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void Execute<TElement>(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, Type elementType, System.Uri requestUri, string httpMethod, bool singleResult, WrappedArray<WrappedOperationParameter> operationParameters, Action<WrappedIEnumerable<TElement>> onCompletion) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedIEnumerable<TElement>>(
                continuation, 
                async, 
                () => context.Execute<TElement>(elementType, requestUri, httpMethod, singleResult, operationParameters), 
                c => context.BeginExecute<TElement>(elementType, requestUri, c, null, httpMethod, singleResult, operationParameters), 
                r => context.EndExecute<TElement>(elementType, r), 
                onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.Execute dynamically
        /// </summary>
        /// <param name="context">The context to call execute on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="requestUri">The uri to make a request to</param>
        /// <param name="httpMethod">The HttpMethod to make a request</param>
        /// <param name="operationParameters">Parameter for the request</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void Execute(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, System.Uri requestUri, string httpMethod, WrappedArray<WrappedOperationParameter> operationParameters, Action<WrappedObject> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            // because EndExecute is reused for the async version, we need to do some work to get the right return type
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedObject>(
                continuation,
                async,
                () => context.Execute(requestUri, httpMethod, operationParameters),
                c => context.BeginExecute(requestUri, c, null, httpMethod, operationParameters),
                r => new WrappedObject(context.Scope, context.EndExecute(r).Product),
                onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.Execute dynamically
        /// </summary>
        /// <typeparam name="TElement">The element type of the wrapped query results</typeparam>
        /// <param name="context">The context to call execute on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="elementType">The element type of the query results</param>
        /// <param name="productContinuation">The wrapped continuation token</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void Execute<TElement>(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, Type elementType, WrappedDataServiceQueryContinuation<TElement> productContinuation, Action<WrappedQueryOperationResponse<TElement>> onCompletion) where TElement : WrappedObject
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            // because EndExecute is reused for the async version, we need to do some work to get the right return type
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedQueryOperationResponse<TElement>>(
                continuation, 
                async,
                () => context.Execute(elementType, productContinuation), 
                c => context.BeginExecute(elementType, productContinuation, c, null), 
                r => new WrappedQueryOperationResponse<TElement>(context.Scope, context.EndExecute<TElement>(elementType, r).Product), 
                onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.SaveChanges dynamically
        /// </summary>
        /// <param name="context">The context to save changes on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="options">The save changes options to use or null to use the overload without options</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void SaveChanges(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, SaveChangesOptions? options, Action<WrappedDataServiceResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            if (options == null)
            {
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedDataServiceResponse>(continuation, async, () => context.SaveChanges(), c => context.BeginSaveChanges(c, null), r => context.EndSaveChanges(r), onCompletion);
            }
            else
            {
                var wrappedEnum = context.Scope.Wrap<WrappedEnum>(options.Value.ToProductEnum());
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedDataServiceResponse>(continuation, async, () => context.SaveChanges(wrappedEnum), c => context.BeginSaveChanges(wrappedEnum, c, null), r => context.EndSaveChanges(r), onCompletion);
            }
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.ExecuteBatch dynamically
        /// </summary>
        /// <param name="context">The context to call execute batch on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="queries">The queries to execute</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void ExecuteBatch(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, WrappedArray<WrappedDataServiceRequest> queries, Action<WrappedDataServiceResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedDataServiceResponse>(continuation, async, () => context.ExecuteBatch(queries), c => context.BeginExecuteBatch(c, null, queries), r => context.EndExecuteBatch(r), onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.GetReadStream dynamically
        /// </summary>
        /// <param name="context">The context to call get read stream on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="entity">The entity to get the read stream for</param>
        /// <param name="streamName">The name of the stream or null to indicate the default stream</param>
        /// <param name="args">The wrapped args to the request</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void GetReadStream(this WrappedDataServiceContext context, IAsyncContinuation continuation, bool async, WrappedEntityInstance entity, string streamName, WrappedObject args, Action<WrappedDataServiceStreamResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            if (streamName == null)
            {
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedDataServiceStreamResponse>(continuation, async, () => context.GetReadStream(entity, args), c => context.BeginGetReadStream(entity, args, c, null), r => context.EndGetReadStream(r), onCompletion);
            }
            else
            {
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<WrappedDataServiceStreamResponse>(continuation, async, () => context.GetReadStream(entity, streamName, args), c => context.BeginGetReadStream(entity, streamName, args, c, null), r => context.EndGetReadStream(r), onCompletion);
            }
        }
    }
}
