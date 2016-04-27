//---------------------------------------------------------------------
// <copyright file="WcfDsServerPayloadElementNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Normalizes payload for reader tests running in WCF DS Server mode.
    /// </summary>
    /// <remarks>
    /// The WCF DS Server mode ignores certain properties in the payload, this normalizer removes them from the expected result tree.
    /// </remarks>
    public class WcfDsServerPayloadElementNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// The format being used by the test.
        /// </summary>
        ODataFormat format;

        /// <summary>
        /// Model for the payload.
        /// </summary>
        EdmModel payloadEdmModel;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="payloadEdmModel">Model for the payload.</param>
        private WcfDsServerPayloadElementNormalizer(ODataFormat format, EdmModel payloadEdmModel)
        {
            this.format = format;
            this.payloadEdmModel = payloadEdmModel;
        }

        /// <summary>
        /// Normalizes the payload for WCF DS Server mode.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        /// <param name="format">The format ot use.</param>
        /// <returns>The normalized payload element.</returns>
        public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement, ODataFormat format, EdmModel payloadEdmModel)
        {
            new WcfDsServerPayloadElementNormalizer(format, payloadEdmModel).Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Normalizes the payload for WCF DS Server mode.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        /// <param name="format">The format ot use.</param>
        /// <returns>The normalized payload element.</returns>
        public static ODataPayloadElement Normalize(ODataFormat format, EdmModel payloadEdmModel, ODataPayloadElement payloadElement)
        {
            new WcfDsServerPayloadElementNormalizer(format, payloadEdmModel).Recurse(payloadElement);
            return payloadElement;
        }

        private bool IsMLE(EntityInstance entity)
        {
            return entity.StreamSourceLink != null ||
                HasDefaultStream(entity) ||
                HasPropertiesOnEntryChildLevel(entity);
        }

        private bool HasPropertiesOnEntryChildLevel(EntityInstance entity)
        {
            var xmlPayloadAnnotation = entity.Annotations.OfType<XmlPayloadElementRepresentationAnnotation>().SingleOrDefault();

            return xmlPayloadAnnotation != null &&
                xmlPayloadAnnotation
                .XmlNodes
                .OfType<XElement>()
                .Single(e => e.Name == TestAtomConstants.AtomXNamespace + "entry")
                .Elements(TestAtomConstants.ODataMetadataXNamespace + "properties")
                .Any();
        }

        private bool HasDefaultStream(EntityInstance entity)
        {
            if (entity.FullTypeName == null)
            {
                return false;
            }

            if (payloadEdmModel != null)
            {
                var edmEntityType = payloadEdmModel.GetEntityType(entity.FullTypeName);
                return edmEntityType != null && edmEntityType.HasStream;
            }

            return false;
        }
    }
}