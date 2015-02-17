//---------------------------------------------------------------------
// <copyright file="AtomPayloadElementPropertyOrderNormalizer.cs" company="Microsoft">
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
    /// Normalizes property order in payload element tree for ATOM format.
    /// </summary>
    /// <remarks>
    /// When entry is written as ATOM the order of properties is partially lost on the wire due to limitations of the format.
    /// This normalizer will fix the order of the properties in the payload element tree to match the one used by the ATOM format.
    /// Note that the normalizer will modify the tree, so the caller may need to clone it before normalizing.
    /// </remarks>
    public class AtomPayloadElementPropertyOrderNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Normalizes property order in payload element tree for ATOM format.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        /// <returns>The normalized payload element.</returns>
        public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement)
        {
            new AtomPayloadElementPropertyOrderNormalizer().Recurse(payloadElement);
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

            // When we write properties into ATOM we write them in this order:
            // - Navigation properties
            // - Named stream properties
            // - All other properties (simple properties)
            // Inside each group the original order is maintained.
            // Also note that the association link order is lost due to the way we report it through the API, so put those after all the properties
            //   (only pure association links, otherwise we match them against the nav. links)
            payloadElement.Properties =
                payloadElement.Properties.Where(p => p.ElementType == ODataPayloadElementType.NavigationPropertyInstance && !p.IsAssociationLinkOnly())
                .Concat(payloadElement.Properties.Where(p => p.ElementType == ODataPayloadElementType.NamedStreamInstance))
                .Concat(payloadElement.Properties.Where(p =>
                    p.ElementType != ODataPayloadElementType.NavigationPropertyInstance &&
                    p.ElementType != ODataPayloadElementType.NamedStreamInstance))
                .Concat(payloadElement.Properties.Where(p => p.IsAssociationLinkOnly()))
                .ToList();
        }
    }
}