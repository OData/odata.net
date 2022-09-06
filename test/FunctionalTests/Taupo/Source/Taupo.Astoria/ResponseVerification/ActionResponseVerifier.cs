//---------------------------------------------------------------------
// <copyright file="ActionResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Response verifier for all uris that contain an action segment
    /// </summary>
    public class ActionResponseVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        private DataServiceProtocolVersion maxProtocolVersion;
        private EntityModelSchema entityModelSchema;

        /// <summary>
        /// Initializes a new instance of the ActionResponseVerifier class
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <param name="maxProtocolVersion">Max Protocol Version</param>
        internal ActionResponseVerifier(EntityModelSchema entityModelSchema, DataServiceProtocolVersion maxProtocolVersion)
        {
            this.entityModelSchema = entityModelSchema;
            this.maxProtocolVersion = maxProtocolVersion;
        }

        /// <summary>
        /// Gets or sets the data synchronizer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAsyncDataSynchronizer Synchronizer { get; set; }

        /// <summary>
        /// Gets or sets the response verification services
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IResponseVerificationServices VerificationServices { get; set; }

        /// <summary>
        /// Gets or sets the ODataUriEvaluator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriEvaluator ODataUriEvaluator { get; set; }

        /// <summary>
        /// Gets or sets the Parameters PayloadElement to QueryValue converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IActionPayloadElementToQueryValuesConverter ParametersPayloadElementToQueryValuesConverter { get; set; }

        /// <summary>
        /// Gets or sets the literal converter to use when generating etags
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Returns true if this is an action request
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>Whether or not this verifier applies to the request</returns>
        public bool Applies(ODataRequest request)
        {
            return request.Uri.IsAction();
        }

        /// <summary>
        /// Returns true if for all responses
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return true;
        }

        /// <summary>
        /// Verifies the update succeeded
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            var functionSegment = request.Uri.LastSegment as FunctionSegment;
            ExceptionUtilities.CheckObjectNotNull(functionSegment, "Expected a Uri with a FunctionSegment at the end");
            ExceptionUtilities.CheckObjectNotNull(functionSegment.Function, "Expected a FunctionSegment to have a valid Function");
            ExceptionUtilities.Assert(functionSegment.Function.IsAction(), "Expected a FunctionSegment that is an action");

            var dataServiceMemberGeneratorAnnotation = functionSegment.Function.Annotations.OfType<DataServiceMemberGeneratorAnnotation>().SingleOrDefault();
            var verifyServiceQueryResult = dataServiceMemberGeneratorAnnotation as IVerifyServiceActionQueryResult;
            var throwErrorAnnotation = functionSegment.Function.Annotations.OfType<ThrowDataServiceExceptionAnnotation>().SingleOrDefault();

            // TODO: add verification of the thrown error case, will add later
            if (throwErrorAnnotation == null && verifyServiceQueryResult != null)
            {
                this.VerifyQueryResult(request, response, functionSegment, verifyServiceQueryResult);
            }
        }

        internal static ODataUri ConstructODataUriWithoutActionInformation(IEnumerable<EntitySet> entitySets, ODataUri uri)
        {
            var segmentsToInclude = new List<ODataUriSegment>();
            ServiceOperationAnnotation serviceOperationAnnotation = null;
            string setName = null;
            foreach (var segment in uri.Segments)
            {
                var functionSegment = segment as FunctionSegment;
                if (functionSegment != null && functionSegment.Function.IsAction())
                {
                    serviceOperationAnnotation = functionSegment.Function.Annotations.OfType<ServiceOperationAnnotation>().Single();
                    var toggleBooleanAnnotation = functionSegment.Function.Annotations.OfType<ToggleBoolPropertyValueActionAnnotation>().SingleOrDefault();

                    if (toggleBooleanAnnotation != null)
                    {
                        setName = toggleBooleanAnnotation.SourceEntitySet;
                    }

                    break;
                }

                segmentsToInclude.Add(segment);
            }

            if (!serviceOperationAnnotation.BindingKind.IsBound())
            {
                ExceptionUtilities.CheckObjectNotNull(setName, "Cannot find the set name that the action starts from");
                var sourceEntitySet = entitySets.Single(es => es.Name == setName);
                segmentsToInclude.Add(new EntitySetSegment(sourceEntitySet));
            }

            var actionlessODataUri = new ODataUri(segmentsToInclude);
            actionlessODataUri.CustomQueryOptions = uri.CustomQueryOptions;
            actionlessODataUri.Filter = uri.Filter;
            actionlessODataUri.InlineCount = uri.InlineCount;
            actionlessODataUri.OrderBy = uri.OrderBy;
            actionlessODataUri.Skip = uri.Skip;
            actionlessODataUri.SkipToken = uri.SkipToken;
            actionlessODataUri.Top = uri.Top;
            EntitySet expectedEntitySet = null;

            ExceptionUtilities.Assert(actionlessODataUri.TryGetExpectedEntitySet(out expectedEntitySet), "Expected entity set not found");

            // expand all navigations for actionlessODataUri so that we do not need to send additional request when calculating expected action result
            foreach (NavigationProperty navigation in expectedEntitySet.EntityType.NavigationProperties)
            {
                actionlessODataUri.ExpandSegments.Add(new List<ODataUriSegment>() { new NavigationSegment(navigation) });
            }

            return actionlessODataUri;
        }

        internal static HttpStatusCode CalculateExpectedStatusCode(FunctionSegment functionSegment)
        {
            // If the action returns information expect 200, otherwise 204
            if (functionSegment.Function.ReturnType == null)
            {
                return HttpStatusCode.NoContent;
            }
            else
            {
                return HttpStatusCode.OK;
            }
        }

        internal string CalculateExpectedETag(QueryValue expected)
        {
            if (!expected.IsNull)
            {
                QueryEntityType queryEntityType = expected.Type as QueryEntityType;
                if (queryEntityType != null)
                {
                    return this.LiteralConverter.ConstructWeakETag(expected as QueryStructuralValue);
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ETag", Justification = "Casing is correct")]
        private void VerifyQueryResult(ODataRequest request, ODataResponse response, FunctionSegment functionSegment, IVerifyServiceActionQueryResult verifyServiceQueryResult)
        {
            EntitySet expectedBindingEntitySet = null;
            var actionLessUri = ConstructODataUriWithoutActionInformation(this.entityModelSchema.GetDefaultEntityContainer().EntitySets, request.Uri);
            bool entitySetFound = actionLessUri.TryGetExpectedEntitySet(out expectedBindingEntitySet);
            ExceptionUtilities.Assert(entitySetFound, "EntitySet not found for Uri {0}", actionLessUri.ToString());

            // SDP never applies for actions
            var preActionQueryValueResult = this.ODataUriEvaluator.Evaluate(actionLessUri, true, false);

            IDictionary<string, QueryValue> queryValueParametersLookup = new Dictionary<string, QueryValue>();

            if (request.Body != null)
            {
                queryValueParametersLookup = this.CreateQueryValueParameters(request, functionSegment, queryValueParametersLookup);
            }

            var expected = verifyServiceQueryResult.GetExpectedQueryValue(preActionQueryValueResult, queryValueParametersLookup.Values.ToArray());
            HttpStatusCode expectedStatusCode = CalculateExpectedStatusCode(functionSegment);

            string expectedETag = null;
            SyncHelpers.ExecuteActionAndWait(c => this.Synchronizer.SynchronizeEntireEntitySet(c, expectedBindingEntitySet.Name));

            expectedETag = this.VerifyExpected(request, response, expected, expectedStatusCode, expectedETag);

            ETagHeaderVerifier etagHeaderVerifier = new ETagHeaderVerifier();
            etagHeaderVerifier.Verify(expectedETag, request, response);
        }

        private IDictionary<string, QueryValue> CreateQueryValueParameters(ODataRequest request, FunctionSegment functionSegment, IDictionary<string, QueryValue> queryValueParametersLookup)
        {
            var parameters = request.Body.RootElement as ComplexInstance;

            // Convert the parameters to query values
            ExceptionUtilities.CheckObjectNotNull(parameters, "Expected parameters to not be null for action");
            queryValueParametersLookup = this.ParametersPayloadElementToQueryValuesConverter.Convert(parameters, functionSegment.Function);

            return queryValueParametersLookup;
        }

        private string VerifyExpected(ODataRequest request, ODataResponse response, QueryValue expected, HttpStatusCode expectedStatusCode, string expectedETag)
        {
            if (expected.IsNull)
            {
                ExceptionUtilities.Assert(response.StatusCode == HttpStatusCode.NoContent, "Expected status code {0} actual {1}", HttpStatusCode.NoContent, response.StatusCode);
            }
            else
            {
                this.VerificationServices.ValidateResponsePayload(request.Uri, response, expected, this.maxProtocolVersion);
                ExceptionUtilities.Assert(response.StatusCode == expectedStatusCode, "Expected status code {0} actual {1}", expectedStatusCode, response.StatusCode);

                // Verify that there are no next links in a response payload to an action with SDP enabled
                var entitySetInstance = response.RootElement as EntitySetInstance;
                if (entitySetInstance != null)
                {
                    this.Assert(entitySetInstance.NextLink == null, "Should be no next links in response", request, response);
                }

                expectedETag = this.CalculateExpectedETag(expected);
            }

            return expectedETag;
        }
    }
}
