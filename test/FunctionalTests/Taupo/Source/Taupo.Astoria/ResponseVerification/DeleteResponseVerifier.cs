//---------------------------------------------------------------------
// <copyright file="DeleteResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Response verifier for DELETE requests
    /// </summary>
    public class DeleteResponseVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
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
        /// Gets or sets the request manager
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }
        
        /// <summary>
        /// Returns true if this is a DELETE request
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>Whether or not this verifier applies to the request</returns>
        public bool Applies(ODataRequest request)
        {
            return request.GetEffectiveVerb() == HttpVerb.Delete;
        }

        /// <summary>
        /// Returns true if the observed status code is 204 (No content)
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Verifies the delete succeeded
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            var originalUri = request.Uri;

            var entityUri = originalUri.ScopeToEntity();

            var beforeSync = this.Evaluator.Evaluate(entityUri, false, false) as QueryStructuralValue;
            ExceptionUtilities.CheckObjectNotNull(beforeSync, "Could not get entity before syncing");
            ExceptionUtilities.Assert(!beforeSync.IsNull, "Entity was null before syncing");

            var afterSync = this.SynchronizeAndEvaluate(entityUri, beforeSync, originalUri.IsEntity());

            QueryValue currentValue = afterSync;
            string message = null;

            if (originalUri.IsEntity())
            {
                // DELETE Customers(1)
                message = "Entity was not deleted";
            }
            else if (originalUri.IsPropertyValue())
            {
                // DELETE Customers(1)/Name/$value
                message = "Property value was not null";

                // drill down by each property in the uri after the entity portion
                foreach (var propertySegment in originalUri.Segments.Skip(entityUri.Segments.Count).OfType<PropertySegment>())
                {
                    currentValue = ((QueryStructuralValue)currentValue).GetValue(propertySegment.Property.Name);
                }
            }
            else if (originalUri.IsEntityReferenceLink())
            {
                // TODO: verify back-links?
                var navigation = originalUri.Segments.OfType<NavigationSegment>().Last();

                if (navigation.NavigationProperty.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
                {
                    // DELETE Customers(1)/Orders/$ref?$id=Orders(1)
                    message = "Collection link was not deleted";

                    var linkUri = new ODataUri(entityUri.Segments.Concat(navigation, originalUri.Segments.OfType<KeyExpressionSegment>().Last()));
                    currentValue = this.Evaluator.Evaluate(linkUri, false, false);
                }
                else
                {
                    // DELETE Customers(1)/BestFriend/$ref
                    message = "Reference link was not deleted";

                    currentValue = ((QueryStructuralValue)afterSync).GetValue(navigation.NavigationProperty.Name);
                }
            }

            // at this point, the current value should be null if the delete was successful
            ExceptionUtilities.CheckObjectNotNull(message, "Uri did not represent an entity, value, or link: '{0}'", request.GetRequestUri());
            this.Assert(currentValue.IsNull, message, request, response);

            if (originalUri.IsEntity())
            {
                this.RequeryEntityAndVerifyStatusCode(request, response, entityUri);

                if (entityUri.Segments.OfType<NavigationSegment>().Any())
                {
                    // convert the uri into a top-level access
                    entityUri = GetTopLevelUri(beforeSync);
                    entityUri.Segments.Insert(0, originalUri.Segments.OfType<ServiceRootSegment>().Single());

                    this.RequeryEntityAndVerifyStatusCode(request, response, entityUri);
                }
            }
        }

        private static ODataUri GetTopLevelUri(QueryStructuralValue entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            QueryEntityType entityType = entity.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Given structural value was not an entity type");

            var setSegment = ODataUriBuilder.EntitySet(entityType.EntitySet);
            var keyValues = entityType.EntityType.AllKeyProperties.Select(k => new NamedValue(k.Name, entity.GetScalarValue(k.Name).Value));
            var keySegment = ODataUriBuilder.Key(entityType.EntityType, keyValues);

            return new ODataUri(setSegment, keySegment);
        }

        private QueryValue SynchronizeAndEvaluate(ODataUri entityUri, QueryStructuralValue beforeSync, bool synchronizeEntireSet)
        {
            var entityType = beforeSync.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Structural value was not an entity type");

            // if an entity was deleted, synchronize the entire set. Otherwise just synchronize the entity
            if (synchronizeEntireSet)
            {
                SyncHelpers.ExecuteActionAndWait(c => this.Synchronizer.SynchronizeEntireEntitySet(c, entityType.EntitySet.Name));
            }
            else
            {
                SyncHelpers.ExecuteActionAndWait(c => this.Synchronizer.SynchronizeEntity(c, beforeSync));
            }

            return this.Evaluator.Evaluate(entityUri, false, false);
        }

        private void RequeryEntityAndVerifyStatusCode(ODataRequest request, ODataResponse response, ODataUri entityUri)
        {
            var getVerificationRequest = this.RequestManager.BuildRequest(entityUri, HttpVerb.Get, new HttpHeaderCollection());
            var getVerificationResponse = this.RequestManager.GetResponse(getVerificationRequest);

            var expectedStatusCode = HttpStatusCode.NotFound;
            if (entityUri.LastSegment.SegmentType == ODataUriSegmentType.NavigationProperty)
            {
                var navigation = (NavigationSegment)entityUri.LastSegment;
                if (navigation.NavigationProperty.ToAssociationEnd.Multiplicity != EndMultiplicity.Many)
                {
                    expectedStatusCode = HttpStatusCode.NoContent;
                }
            }

            string message = null;
            bool succeeded = getVerificationResponse.StatusCode == expectedStatusCode;
            if (!succeeded)
            {
                message = @"Re-querying the deleted entity did not result in correct status code
Expected: {0}
Actual:   {1}";
                message = string.Format(CultureInfo.InvariantCulture, message, expectedStatusCode, getVerificationResponse.StatusCode);
                this.ReportFailure(request, response);
            }

            this.Assert(succeeded, message, getVerificationRequest, getVerificationResponse);
        }
    }
}
