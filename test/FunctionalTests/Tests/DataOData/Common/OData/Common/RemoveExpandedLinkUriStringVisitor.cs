//---------------------------------------------------------------------
// <copyright file="RemoveExpandedLinkUriStringVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// Visitor which removes the UriString from ExpandedLinks in a payload.
    /// </summary>
    public class RemoveExpandedLinkUriStringVisitor : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visit the payload element and removes UriString value from ExpandedLinks.
        /// </summary>
        /// <param name="payloadElement">The root of the payload element tree to visit.</param>
        /// <remarks>This method will modify the input payload element, so if there is a need to 
        /// preserve the actual input payload element it should be cloned.
        /// </remarks>
        public static ODataPayloadElement Visit(ODataPayloadElement payloadElement)
        {
            if (payloadElement != null)
            {
                new RemoveExpandedLinkUriStringVisitor().Recurse(payloadElement);
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the payload element. 
        /// </summary>
        /// <param name="payloadElement">The payload element to visit.</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            payloadElement.UriString = null;
            base.Visit(payloadElement);
        }
    }
}
