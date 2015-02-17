//---------------------------------------------------------------------
// <copyright file="JsonLightPayloadElementNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Contracts.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Normalizer for JSON Light payloads.
    /// </summary>
    public class JsonLightPayloadElementNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>The payload element this normalizer was created for; 
        /// used to detect whether we are visiting the top-level element or not.</summary>
        private readonly ODataPayloadElement rootPayloadElement;

        /// <summary>
        /// The test configuration to use.
        /// </summary>
        private readonly ReaderTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rootPayloadElement">The payload element this normalizer was created for; 
        /// used to detect whether we are visiting the top-level element or not.</param>
        /// <param name="testConfig"></param>
        private JsonLightPayloadElementNormalizer(ODataPayloadElement rootPayloadElement, ReaderTestConfiguration testConfiguration)
        {
            this.rootPayloadElement = rootPayloadElement;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// Normalizes JSON Light payload.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The normalized payload element.</returns>
        public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement, ReaderTestConfiguration testConfiguration)
        {
            new JsonLightPayloadElementNormalizer(payloadElement, testConfiguration).Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Normalizes an expanded link.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            base.Visit(payloadElement);

            // Expanded link Url is only writtern for responses.
            if (this.testConfiguration.IsRequest)
            {
                payloadElement.UriString = null;
            }
        }

        /// <summary>
        /// Normalizes navigation property.
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            base.Visit(payloadElement);

            // Inject an empty $select projection for top-level entries (if no projection exists)
            this.InjectEmptyContextUriProjection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            base.Visit(payloadElement);

            // Inject an empty $select projection for top-level feeds (if no projection exists)
            this.InjectEmptyContextUriProjection(payloadElement);
        }

        /// <summary>
        /// If not context URI projection is present on a root element, injects an empty
        /// projection to not have template expansion interfere with the reported payloads.
        /// </summary>
        /// <param name="payloadElement">The payload element to check.</param>
        private void InjectEmptyContextUriProjection(ODataPayloadElement payloadElement)
        {
            if (!this.testConfiguration.IsRequest && this.IsRootElement(payloadElement))
            {
                JsonLightContextUriProjectionAnnotation projectionAnnotation = 
                    payloadElement.Annotations.OfType<JsonLightContextUriProjectionAnnotation>().SingleOrDefault();
                if (projectionAnnotation == null)
                {
                    payloadElement.Annotations.Add(JsonLightContextUriProjectionAnnotation.EmptyProjectionAnnotation);
                }
            }
        }

        /// <summary>
        /// Checks whether the specified payload element is the root element the normalizer was created for.
        /// </summary>
        /// <param name="payloadElement">The payload element to check.</param>
        /// <returns>true if the <paramref name="payloadElement"/> is the root element; otherwise false.</returns>
        private bool IsRootElement(ODataPayloadElement payloadElement)
        {
            return object.ReferenceEquals(this.rootPayloadElement, payloadElement);
        }
    }
}
