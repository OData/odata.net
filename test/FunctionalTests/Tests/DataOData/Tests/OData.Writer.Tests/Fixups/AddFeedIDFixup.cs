//---------------------------------------------------------------------
// <copyright file="AddFeedIDFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Atom;

    /// <summary>
    /// Adds an ID to all feeds in the payload
    /// </summary>
    public class AddFeedIDFixup : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var hasID = payloadElement.Annotations.Where(a => a is XmlTreeAnnotation && (a as XmlTreeAnnotation).LocalName == TestAtomConstants.AtomIdElementName).SingleOrDefault();
            if (hasID == null)
            {
                payloadElement.AtomId("urn:FeedID");
            }
            
            base.Visit(payloadElement);
        }
    }
}
