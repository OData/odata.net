//---------------------------------------------------------------------
// <copyright file="JsonLightExpectedPayloadElementNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces

    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// Normalizer for JSON Light expected result payloads.
    /// </summary>
    /// <remarks>
    /// Fixes certains differences in reported JSON Light payload.
    /// - When navigation link is present, it always has an association link as well (this is because the reader fills in association link to enable templating).
    /// </remarks>
    public class JsonLightExpectedPayloadElementNormalizer : ODataPayloadElementVisitorBase
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
        private JsonLightExpectedPayloadElementNormalizer(ODataPayloadElement rootPayloadElement, ReaderTestConfiguration testConfiguration)
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
            new JsonLightExpectedPayloadElementNormalizer(payloadElement, testConfiguration).Recurse(payloadElement);
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
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            base.Visit(payloadElement);

            // Each navigation property has navigation link - in JSON Light we never report just association link.
            if (payloadElement.Value == null)
            {
                payloadElement.Value = new DeferredLink();
            }

            // Each navigation property always has association link in response due to templating.
            // If it wasn't present, fill an empty one in.
            if (payloadElement.AssociationLink == null && !this.testConfiguration.IsRequest)
            {
                payloadElement.AssociationLink = new DeferredLink();
            }
        }

        /// <summary>
        /// Normalizes a complex collection property.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            base.Visit(payloadElement);
            this.NormalizePropertyName(payloadElement);
        }

        /// <summary>
        /// Normalizes a complex property.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            base.Visit(payloadElement);
            this.NormalizePropertyName(payloadElement);
        }

        /// <summary>
        /// Normalizes an empty collection property property.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(EmptyCollectionProperty payloadElement)
        {
            base.Visit(payloadElement);
            this.NormalizePropertyName(payloadElement);
        }

        /// <summary>
        /// Normalizes a null property.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(NullPropertyInstance payloadElement)
        {
            base.Visit(payloadElement);
            this.NormalizePropertyName(payloadElement);
        }

        /// <summary>
        /// Normalizes primitive collection property.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            base.Visit(payloadElement);
            this.NormalizePropertyName(payloadElement);
        }

        /// <summary>
        /// Normalizes primitive property.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(PrimitiveProperty payloadElement)
        {
            base.Visit(payloadElement);
            this.NormalizePropertyName(payloadElement);
        }

        /// <summary>
        /// Normalizes the name of a property instance (JSON Light reports an empty name for top-level properties in requests).
        /// </summary>
        /// <param name="payloadElement">The property instance to normalize the name for.</param>
        private void NormalizePropertyName(PropertyInstance payloadElement)
        {
            if (this.IsRootElement(payloadElement))
            { 
                // NOTE: in JSON Light we report null property names.
                payloadElement.Name = null;
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
