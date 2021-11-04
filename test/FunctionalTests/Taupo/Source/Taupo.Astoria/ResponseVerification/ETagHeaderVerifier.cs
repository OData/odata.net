//---------------------------------------------------------------------
// <copyright file="ETagHeaderVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Response verifier for the etag header
    /// </summary>
    public class ETagHeaderVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        /// <summary>
        /// Gets or sets the literal converter to use when generating etags
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Gets or sets the response verification context
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IResponseVerificationContext Context { get; set; }

        /// <summary>
        /// Gets or sets the uri evaluator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriEvaluator Evaluator { get; set; }

        /// <summary>
        /// Returns true if this is not action
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>Whether or not this verifier applies to the request</returns>
        public bool Applies(ODataRequest request)
        {
            // ActionResponseVerifier verifies action ETag
            return !request.Uri.IsAction();
        }

        /// <summary>
        /// Returns true for all responses
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return true;
        }

        /// <summary>
        /// Verifies the etag header on the response is correct or not present
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ETag", Justification = "Casing is correct")]
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            string expectedETag = this.CalculateExpectedETag(request, response);
            this.Verify(expectedETag, request, response);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ETag", Justification = "Casing is correct")]
        internal void Verify(string expectedETag, ODataRequest request, ODataResponse response)
        {
            string etagValue;
            bool hadETag = response.Headers.TryGetValue(HttpHeaders.ETag, out etagValue);
            if (expectedETag == null)
            {
                this.Assert(!hadETag, string.Format(CultureInfo.InvariantCulture, "Response contained unexpected ETag header with value '{0}'.", etagValue), request, response);
            }
            else
            {
                this.Assert(hadETag, string.Format(CultureInfo.InvariantCulture, "ETag header unexpectedly missing from response. Expected '{0}'.", expectedETag), request, response);
                this.Assert(expectedETag == etagValue, string.Format(CultureInfo.InvariantCulture, "ETag header value incorrect.\r\nExpected: '{0}'\r\nActual:   '{1}'", expectedETag, etagValue), request, response);
            }
        }

        internal string CalculateExpectedETag(ODataRequest request, ODataResponse response)
        {
            // error responses should never have an ETag
            if (response.StatusCode.IsError())
            {
                return null;
            }

            // $ref never involves ETags
            if (request.Uri.IsEntityReferenceLink())
            {
                return null;
            }

            var verb = request.GetEffectiveVerb();

            if (verb == HttpVerb.Get)
            {
                return this.CalculateExpectedETagForRetrieve(request);
            }

            if (verb == HttpVerb.Delete)
            {
                // TODO: Server should write ETag header in response to property-value deletes
                return null;
            }
            
            if (verb == HttpVerb.Post)
            {
                if (request.Uri.IsServiceOperation())
                {
                    return this.CalculateExpectedETagForRetrieve(request);
                }

                if (!request.Uri.IsEntitySet())
                {
                    return null;
                }

                using (this.Context.Begin(request))
                {
                    var insertedEntity = this.Context.GetInsertedEntity(request, response);
                    return this.LiteralConverter.ConstructWeakETag(insertedEntity);
                }
            }

            return this.CalculateExpectedETagForUpdate(request);
        }

        private static bool HasETagOnRetrieve(ODataUri uri)
        {
            if (uri.ExpandSegments.Any())
            {
                return false;
            }

            if (uri.IsEntity())
            {
                return true;
            }

            if (uri.IsProperty())
            {
                return true;
            }

            if (uri.IsPropertyValue() || uri.IsMediaResource())
            {
                // this covers media-resources as well
                return true;
            }

            if (uri.IsNamedStream())
            {
                return true;
            }

            return false;
        }

        private string CalculateExpectedETagForRetrieve(ODataRequest request)
        {
            if (!HasETagOnRetrieve(request.Uri))
            {
                return null;
            }

            var scopedUri = request.Uri.ScopeToEntity();
            var entity = this.Evaluator.Evaluate(scopedUri, false, false) as QueryStructuralValue;
            ExceptionUtilities.CheckArgumentNotNull(entity, "Evaluated uri did not result in an entity");

            return this.CalculateExpectedETagForEntityOrStream(request.Uri, entity);
        }

        private string CalculateExpectedETagForUpdate(ODataRequest request)
        {
            using (this.Context.Begin(request))
            {
                QueryStructuralValue beforeUpdate;
                QueryStructuralValue afterUpdate;
                this.Context.GetUpdatedEntity(request, out beforeUpdate, out afterUpdate);

                return this.CalculateExpectedETagForEntityOrStream(request.Uri, afterUpdate);
            }
        }

        private string CalculateExpectedETagForEntityOrStream(ODataUri uri, QueryStructuralValue entity)
        {
            if (uri.IsMediaResource())
            {
                return entity.GetDefaultStreamValue().GetExpectedETag();
            }

            if (uri.IsNamedStream())
            {
                var streamSegment = uri.Segments.OfType<NamedStreamSegment>().Last();
                return entity.GetStreamValue(streamSegment.Name).GetExpectedETag();
            }

            return this.LiteralConverter.ConstructWeakETag(entity);
        }
    }
}
