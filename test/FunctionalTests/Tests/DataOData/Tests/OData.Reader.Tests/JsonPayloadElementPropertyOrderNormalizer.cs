//---------------------------------------------------------------------
// <copyright file="JsonPayloadElementPropertyOrderNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    #endregion Namespaces

    /// <summary>
    /// Normalizes property order in payload element tree for JSON format.
    /// </summary>
    /// <remarks>
    /// When entry is written as JSON the order of properties is partially lost on the wire due to limitations of the format.
    /// This normalizer will fix the order of the properties in the payload element tree to match the one used by the JSON format.
    /// Note that the normalizer will modify the tree, so the caller may need to clone it before normalizing.
    /// </remarks>
    public class JsonPayloadElementPropertyOrderNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Normalizes property order in payload element tree for JSON format.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        /// <returns>The normalized payload element.</returns>
        public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement)
        {
            new JsonPayloadElementPropertyOrderNormalizer().Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Normalizes property order on an entry.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            // First recurse to children so that we get those normalized and then normalize the parent entity.
            base.Visit(payloadElement);

            // When we write properties into JSON we write them all in the original order except for association links.
            // Since association links are matched against nav. properties, most of the time that is not visible
            // but if there's an association link without its nav. link portion that one will be reported last.
            payloadElement.Properties =
                payloadElement.Properties.Where(p => !p.IsAssociationLinkOnly())
                .Concat(payloadElement.Properties.Where(p => p.IsAssociationLinkOnly()))
                .ToList();
        }
    }
}