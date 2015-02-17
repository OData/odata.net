//---------------------------------------------------------------------
// <copyright file="JsonSelfLinkToEditLinkFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// As json can only represent 1 uri and the edit link takes priority 
    /// if there is a self link and no edit link the edit link is made the self link
    /// the self link should always be removed
    /// </summary>
    class JsonSelfLinkToEditLinkFixup : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var selflink = payloadElement.Annotations.OfType<SelfLinkAnnotation>().SingleOrDefault();
            if (selflink != null)
            {
                if (payloadElement.EditLink == null)
                {
                    payloadElement.EditLink = selflink.Value;
                }
                payloadElement.Annotations.Remove(selflink);
            }

            base.Visit(payloadElement);
        }
    }
}
