//---------------------------------------------------------------------
// <copyright file="RemoveFeedIDFixup.cs" company="Microsoft">
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
    /// Removes the ID from all feeds in the payload
    /// </summary>
    public class RemoveFeedIDFixup : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var idAnnotation = payloadElement.Annotations
                .Where(a => a is XmlTreeAnnotation && ((XmlTreeAnnotation)a).LocalName == TestAtomConstants.AtomIdElementName).SingleOrDefault();
            if (idAnnotation != null)
            {
                payloadElement.Annotations.Remove(idAnnotation);
            }
            
            base.Visit(payloadElement);
        }
    }
}
