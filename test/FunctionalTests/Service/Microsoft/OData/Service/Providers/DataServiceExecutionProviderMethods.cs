//---------------------------------------------------------------------
// <copyright file="DataServiceExecutionProviderMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    #endregion

    /// <summary>Data service methods that can be inside the expression that is passed to the IDataServiceExecutionProvider interface.</summary>
    internal static class DataServiceExecutionProviderMethods
    {
        #region Internal MethodInfos

        /// <summary>MethodInfo for TypeIs.</summary>
        internal static readonly MethodInfo OfTypeMethodInfo = typeof(DataServiceExecutionProviderMethods).GetMethod(
            "OfType",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for SetContinuationToken.</summary>
        internal static readonly MethodInfo SetContinuationTokenMethodInfo = typeof(DataServiceExecutionProviderMethods).GetMethod(
            "SetContinuationToken",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for ApplyProjections.</summary>
        internal static readonly MethodInfo ApplyProjectionsMethodInfo = typeof(DataServiceExecutionProviderMethods).GetMethod(
            "ApplyProjections",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for IDataServiceActionProvider.CreateInvokable().</summary>
        internal static readonly MethodInfo CreateServiceActionInvokableMethodInfo = typeof(DataServiceExecutionProviderMethods).GetMethod(
            "CreateServiceActionInvokable",
            BindingFlags.Static | BindingFlags.Public);

        #endregion

        /// <summary>
        /// Filters the given <paramref name="query"/> based on the <paramref name="resourceType"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of the IQueryable instance passed in <paramref name="query"/>.</typeparam>
        /// <typeparam name="TResult">Type representing the resource type passed in <paramref name="resourceType"/>.</typeparam>
        /// <param name="query">IQueryable instance.</param>
        /// <param name="resourceType">ResourceType based on which IQueryable needs to be filtered.</param>
        /// <returns>an IQueryable instance filtered by ResourceType.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "need TSource and TResult for proper binding type binding")]
        public static IQueryable<TResult> OfType<TSource, TResult>(IQueryable<TSource> query, ResourceType resourceType)
        {
            if (query == null)
            {
                throw Error.ArgumentNull("query");
            }

            if (resourceType == null)
            {
                throw Error.ArgumentNull("resourceType");
            }

            return query.Provider.CreateQuery<TResult>(Expression.Call(null, DataServiceProviderMethods.OfTypeIQueryableMethodInfo.MakeGenericMethod(new Type[] { typeof(TSource), typeof(TResult) }), new Expression[] { query.Expression, Expression.Constant(resourceType) }));
        }

        /// <summary>
        /// Forwards the call to the SetContinuationToken method on <paramref name="pagingProvider"/> to give the
        /// continuation token ($skiptoken) from the request URI, parsed into primitive values, to the provider.
        /// </summary>
        /// <typeparam name="TElement">Element type of the query.</typeparam>
        /// <param name="pagingProvider">An instance of <see cref="IDataServicePagingProvider"/>.</param>
        /// <param name="query">Query for which continuation token is being provided.</param>
        /// <param name="resourceType">Resource type of the result on which skip token is to be applied.</param>
        /// <param name="continuationToken">Continuation token parsed into primitive typed values.</param>
        /// <returns>Returns <paramref name="query"/> after calling SetContinuationToken.</returns>
        /// <remarks>This method simply forwards the call to IDataServicePagingProvider.SetContinuationToken(). This
        /// method is added because IDataServicePagingProvider.SetContinuationToken() returns void and this method
        /// returns <paramref name="query"/> so that we can call SetContinuationToken() in the expression tree
        /// without using the Block expression, to make the expression tree simpler to visit.</remarks>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public static IQueryable<TElement> SetContinuationToken<TElement>(IDataServicePagingProvider pagingProvider, IQueryable<TElement> query, ResourceType resourceType, object[] continuationToken)
        {
            WebUtil.CheckArgumentNull(pagingProvider, "pagingProvider");
            pagingProvider.SetContinuationToken(query, resourceType, continuationToken);
            return query;
        }

        /// <summary>Applies expansions and projections to the specified <paramref name="source"/>.</summary>
        /// <param name="projectionProvider">IProjectionProvider instance.</param>
        /// <param name="source"><see cref="IQueryable"/> object to expand and apply projections to.</param>
        /// <param name="rootProjectionNode">The root node of the tree which describes
        /// the projections and expansions to be applied to the <paramref name="source"/>.</param>
        /// <returns>
        /// An <see cref="IQueryable"/> object, with the results including 
        /// the expansions and projections specified in <paramref name="rootProjectionNode"/>. 
        /// </returns>
        /// <remarks>
        /// The returned <see cref="IQueryable"/> may implement the <see cref="IExpandedResult"/> interface 
        /// to provide enumerable objects for the expansions; otherwise, the expanded
        /// information is expected to be found directly in the enumerated objects. If paging is 
        /// requested by providing a non-empty list in <paramref name="rootProjectionNode"/>.OrderingInfo then
        /// it is expected that the topmost <see cref="IExpandedResult"/> would have a $skiptoken property 
        /// which will be an <see cref="IExpandedResult"/> in itself and each of it's sub-properties will
        /// be named SkipTokenPropertyXX where XX represents numbers in increasing order starting from 0. Each of 
        /// SkipTokenPropertyXX properties will be used to generated the $skiptoken to support paging.
        /// If projections are required, the provider may choose to return <see cref="IQueryable"/>
        /// which returns instances of <see cref="IProjectedResult"/>. In that case property values are determined
        /// by calling the <see cref="IProjectedResult.GetProjectedPropertyValue"/> method instead of
        /// accessing properties of the returned object directly.
        /// If both expansion and projections are required, the provider may choose to return <see cref="IQueryable"/>
        /// of <see cref="IExpandedResult"/> which in turn returns <see cref="IProjectedResult"/> from its
        /// <see cref="IExpandedResult.ExpandedElement"/> property.
        /// </remarks>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public static IQueryable ApplyProjections(object projectionProvider, IQueryable source, object rootProjectionNode)
        {
            return (IQueryable)((IProjectionProvider)projectionProvider).ApplyProjections(source, (RootProjectionNode)rootProjectionNode);
        }

        /// <summary>
        /// Builds up an instance of <see cref="IDataServiceInvokable"/> for the given <paramref name="serviceAction"/> with the provided <paramref name="parameterTokens"/>.
        /// </summary>
        /// <param name="operationContext">The data service operation context instance.</param>
        /// <param name="actionProvider">The IDataServiceActionProvider instance.</param>
        /// <param name="serviceAction">The service action to invoke.</param>
        /// <param name="parameterTokens">The parameter tokens required to invoke the service action.</param>
        /// <returns>An instance of <see cref="IDataServiceInvokable"/> to invoke the action with.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public static IDataServiceInvokable CreateServiceActionInvokable(DataServiceOperationContext operationContext, IDataServiceActionProvider actionProvider, ServiceAction serviceAction, object[] parameterTokens)
        {
            WebUtil.CheckArgumentNull(operationContext, "operationContext");
            WebUtil.CheckArgumentNull(actionProvider, "actionProvider");
            WebUtil.CheckArgumentNull(serviceAction, "serviceAction");

            operationContext.CurrentDataService.ProcessingPipeline.AssertAndUpdateDebugStateAtInvokeServiceAction(operationContext.CurrentDataService);

            IDataServiceInvokable result = actionProvider.CreateInvokable(operationContext, serviceAction, parameterTokens);
            WebUtil.CheckResourceExists(result != null, serviceAction.Name);
            return result;
        }
    }
}
