//---------------------------------------------------------------------
// <copyright file="ResponseVerificationContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Default implementation of the response verification context contract
    /// </summary>
    [ImplementationName(typeof(IResponseVerificationContext), "Default")]
    public class ResponseVerificationContext : IResponseVerificationContext
    {
        private readonly HashSet<ODataRequest> requestsBegun = new HashSet<ODataRequest>(ReferenceEqualityComparer.Create<ODataRequest>());

        private readonly Dictionary<ODataRequest, QueryStructuralValue> insertedEntityCache 
            = new Dictionary<ODataRequest, QueryStructuralValue>(ReferenceEqualityComparer.Create<ODataRequest>());

        private readonly Dictionary<ODataRequest, KeyValuePair<QueryStructuralValue, QueryStructuralValue>> updatedEntityCache
            = new Dictionary<ODataRequest, KeyValuePair<QueryStructuralValue, QueryStructuralValue>>(ReferenceEqualityComparer.Create<ODataRequest>());

        /// <summary>
        /// Gets or sets the uri evaluator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets the data synchronizer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAsyncDataSynchronizer Synchronizer { get; set; }

        /// <summary>
        /// Gets or sets the visitor to use for copying query values
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryValueDeepCopyingVisitor QueryValueCopier { get; set; }

        /// <summary>
        /// Gets the hash-set tracking requests that have been passed to Begin. Unit test hook ONLY.
        /// </summary>
        internal HashSet<ODataRequest> RequestsBegun
        {
            get { return this.requestsBegun; }
        }

        /// <summary>
        /// Gets the cache for inserted entities. Unit test hook ONLY.
        /// </summary>
        internal IDictionary<ODataRequest, QueryStructuralValue> InsertedEntityCache
        {
            get { return this.insertedEntityCache; }
        }

        /// <summary>
        /// Gets cache for updated entities. Unit test hook ONLY.
        /// </summary>
        internal IDictionary<ODataRequest, KeyValuePair<QueryStructuralValue, QueryStructuralValue>> UpdatedEntityCache
        {
            get { return this.updatedEntityCache; }
        }

        /// <summary>
        /// Informs the context that verification for the given request has begun. Can be called multiple times.
        /// Disposing the returned value indicates that verification has finished.
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>A disposable token which, when disposed, indicates that verification is finished.</returns>
        public IDisposable Begin(ODataRequest request)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");

            if (!this.requestsBegun.Add(request))
            {
                return new DelegateBasedDisposable(() => { });
            }

            return new DelegateBasedDisposable(
                () =>
                {
                    // clear all caches
                    insertedEntityCache.Remove(request);
                    updatedEntityCache.Remove(request);
                    requestsBegun.Remove(request);
                });
        }

        /// <summary>
        /// Gets the entity inserted by the given request.
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="response">The response</param>
        /// <returns>The inserted entity</returns>
        public QueryStructuralValue GetInsertedEntity(ODataRequest request, ODataResponse response)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            ExceptionUtilities.Assert(this.requestsBegun.Contains(request), "Cannot use GetInsertedEntity before calling Begin");
            ExceptionUtilities.Assert(request.GetEffectiveVerb() == HttpVerb.Post, "Cannot use GetInsertedEntity on non POST requests");

            QueryStructuralValue insertedEntity;
            if (!this.insertedEntityCache.TryGetValue(request, out insertedEntity))
            {
                EntitySet entitySet;
                ExceptionUtilities.Assert(request.Uri.TryGetExpectedEntitySet(out entitySet), "Could not infer entity set from URI");

                var entitySetUri = new ODataUri(new EntitySetSegment(entitySet));

                var beforeSync = this.Evaluator.Evaluate(entitySetUri, false, false) as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(beforeSync, "Could not evaluate entity set '{0}' before syncing", entitySet.Name);
                
                // create a shallow copy
                beforeSync = beforeSync.Type.CreateCollectionWithValues(beforeSync.Elements);

                SyncHelpers.ExecuteActionAndWait(c => this.Synchronizer.SynchronizeEntireEntitySet(c, entitySet.Name));

                var afterSync = this.Evaluator.Evaluate(entitySetUri, false, false) as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(afterSync, "Could not evaluate entity set '{0}' after syncing", entitySet.Name);

                // TODO: handle deep insert (using location header or response payload)
                var newElements = afterSync.Elements.Except(beforeSync.Elements).OfType<QueryStructuralValue>().ToList();
                this.insertedEntityCache[request] = insertedEntity = newElements.Single();

                // update the parent entity, if there is one
                var segments = request.Uri.Segments;
                var lastNavigationSegment = segments.OfType<NavigationSegment>().LastOrDefault();
                if (lastNavigationSegment != null)
                {
                    var segmentsBeforeNavigation = segments.TakeWhile(t => t != lastNavigationSegment);
                    var parentEntity = (QueryStructuralValue)this.Evaluator.Evaluate(new ODataUri(segmentsBeforeNavigation), false, false);
                    SyncHelpers.ExecuteActionAndWait(c => this.Synchronizer.SynchronizeEntity(c, parentEntity));
                }
            }

            return insertedEntity;
        }

        /// <summary>
        /// Gets the pre and post update representations of the entity updated by the given request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="beforeUpdate">The entity before the update</param>
        /// <param name="afterUpdate">The entity after the update</param>
        public void GetUpdatedEntity(ODataRequest request, out QueryStructuralValue beforeUpdate, out QueryStructuralValue afterUpdate)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.Assert(this.requestsBegun.Contains(request), "Cannot use GetUpdatedEntity before calling Begin");
            ExceptionUtilities.Assert(request.GetEffectiveVerb().IsUpdateVerb(), "Cannot use GetUpdatedEntity on non update requests");

            KeyValuePair<QueryStructuralValue, QueryStructuralValue> beforeAndAfter;
            if (!this.updatedEntityCache.TryGetValue(request, out beforeAndAfter))
            {
                var entityUri = request.Uri.ScopeToEntity();
                var entity = (QueryStructuralValue)this.Evaluator.Evaluate(entityUri, false, false);

                beforeUpdate = this.QueryValueCopier.PerformDeepCopy(entity);

                SyncHelpers.ExecuteActionAndWait(c => this.Synchronizer.SynchronizeEntity(c, entity));

                afterUpdate = entity;

                beforeAndAfter = new KeyValuePair<QueryStructuralValue, QueryStructuralValue>(beforeUpdate, afterUpdate);
                this.updatedEntityCache[request] = beforeAndAfter;
            }
            else
            {
                beforeUpdate = beforeAndAfter.Key;
                afterUpdate = beforeAndAfter.Value;
            }
        }
    }
}
