//---------------------------------------------------------------------
// <copyright file="ProductExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extension methods for instances of product classes
    /// </summary>
    public static class ProductExtensionMethods
    {
        /// <summary>
        /// Extension method to perform sync/async version of DataServiceQuery.Execute dynamically
        /// </summary>
        /// <typeparam name="TElement">The element type of the results</typeparam>
        /// <param name="query">The query to execute</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void Execute<TElement>(this DataServiceQuery<TElement> query, IAsyncContinuation continuation, bool async, Action<IEnumerable<TElement>> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<IEnumerable<TElement>>(
                continuation, 
                async, 
                () => query.Execute(), 
                c => query.BeginExecute(c, null), 
                r => query.EndExecute(r), 
                onCompletion);
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
        public static void LoadProperty(this DataServiceContext context, IAsyncContinuation continuation, bool async, object entity, string propertyName, Action<QueryOperationResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<QueryOperationResponse>(
                continuation, 
                async, 
                () => context.LoadProperty(entity, propertyName), 
                c => context.BeginLoadProperty(entity, propertyName, c, null), 
                r => context.EndLoadProperty(r), 
                onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.Execute dynamically
        /// </summary>
        /// <param name="context">The context to call execute on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="requestUri">The uri to make a request to</param>
        /// <param name="verb">verb to execute in</param>
        /// <param name="inputParameters">Input parameters for execute</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "requestUri", Justification = "When product api comes in this will be hooked up")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "async", Justification = "When product api comes in this will be hooked up")]
        public static void ExecuteUri(this DataServiceContext context, IAsyncContinuation continuation, bool async, Uri requestUri, HttpVerb verb, OperationParameter[] inputParameters, Action<OperationResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(onCompletion, "onCompletion");

            string method = Enum.GetName(typeof(HttpVerb), verb).ToUpperInvariant();
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<OperationResponse>(
               continuation,
               async,
               () => context.Execute(requestUri, method, inputParameters),
               c => context.BeginExecute(requestUri, c, null, method, inputParameters),
               r => context.EndExecute(r),
           onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.Execute dynamically
        /// </summary>
        /// <param name="context">The context to call execute on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="requestUri">The uri to make a request to</param>
        /// <param name="verb">verb to execute in</param>
        /// <param name="inputParameters">Input parameters for execute</param>
        /// <param name="singleResult">Determines if its querying a multivalue or a single value</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        /// <typeparam name="TResult">Result Type of the query</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "async", Justification = "When product api comes in this will be hooked up")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "requestUri", Justification = "When product api comes in this will be hooked up")]
        public static void ExecuteUri<TResult>(this DataServiceContext context, IAsyncContinuation continuation, bool async, Uri requestUri, HttpVerb verb, OperationParameter[] inputParameters, bool singleResult, Action<QueryOperationResponse<TResult>> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(onCompletion, "onCompletion");

            string method = Enum.GetName(typeof(HttpVerb), verb).ToUpperInvariant();

            AsyncHelpers.InvokeSyncOrAsyncMethodCall<QueryOperationResponse<TResult>>(
               continuation,
               async,
               () => (QueryOperationResponse<TResult>)context.Execute<TResult>(requestUri, method, singleResult, inputParameters),
               c => context.BeginExecute<TResult>(requestUri, c, null, method, singleResult, inputParameters),
               r => (QueryOperationResponse<TResult>)context.EndExecute<TResult>(r),
               onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.Execute dynamically
        /// </summary>
        /// <typeparam name="TElement">The element type of the expression results</typeparam>
        /// <param name="context">The context to call execute on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="requestUri">The uri to make a request to</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void Execute<TElement>(this DataServiceContext context, IAsyncContinuation continuation, bool async, Uri requestUri, Action<IEnumerable<TElement>> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<IEnumerable<TElement>>(continuation, async, () => context.Execute<TElement>(requestUri), c => context.BeginExecute<TElement>(requestUri, c, null), r => context.EndExecute<TElement>(r), onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.SaveChanges dynamically
        /// </summary>
        /// <param name="context">The context to save changes on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="options">The save changes options to use or null to use the overload without options</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void SaveChanges(this DataServiceContext context, IAsyncContinuation continuation, bool async, SaveChangesOptions? options, Action<DataServiceResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            if (options == null)
            {
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<DataServiceResponse>(continuation, async, () => context.SaveChanges(), c => context.BeginSaveChanges(c, null), r => context.EndSaveChanges(r), onCompletion);
            }
            else
            {
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<DataServiceResponse>(continuation, async, () => context.SaveChanges(options.Value), c => context.BeginSaveChanges(options.Value, c, null), r => context.EndSaveChanges(r), onCompletion);
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
        public static void ExecuteBatch(this DataServiceContext context, IAsyncContinuation continuation, bool async, IEnumerable<DataServiceRequest> queries, Action<DataServiceResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            AsyncHelpers.InvokeSyncOrAsyncMethodCall<DataServiceResponse>(continuation, async, () => context.ExecuteBatch(queries.ToArray()), c => context.BeginExecuteBatch(c, null, queries.ToArray()), r => context.EndExecuteBatch(r), onCompletion);
        }

        /// <summary>
        /// Extension method to perform sync/async version of DataServiceContext.GetReadStream dynamically
        /// </summary>
        /// <param name="context">The context to call get read stream on</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="async">A value indicating whether or not to use async API</param>
        /// <param name="entity">The entity to get the read stream for</param>
        /// <param name="streamName">The name of the stream or null to indicate the default stream</param>
        /// <param name="args">The args to the request</param>
        /// <param name="onCompletion">A callback for when the call completes</param>
        public static void GetReadStream(this DataServiceContext context, IAsyncContinuation continuation, bool async, object entity, string streamName, DataServiceRequestArgs args, Action<DataServiceStreamResponse> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            if (streamName == null)
            {
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<DataServiceStreamResponse>(continuation, async, () => context.GetReadStream(entity, args), c => context.BeginGetReadStream(entity, args, c, null), r => context.EndGetReadStream(r), onCompletion);
            }
            else
            {
                AsyncHelpers.InvokeSyncOrAsyncMethodCall<DataServiceStreamResponse>(continuation, async, () => context.GetReadStream(entity, streamName, args), c => context.BeginGetReadStream(entity, streamName, args, c, null), r => context.EndGetReadStream(r), onCompletion);
            }
        }
    }
}
