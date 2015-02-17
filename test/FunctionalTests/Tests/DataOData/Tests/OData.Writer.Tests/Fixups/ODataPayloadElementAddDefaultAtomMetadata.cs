//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementAddDefaultAtomMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Atom;

    /// <summary>
    /// Converts any entry with a null ID to string.empty 
    /// </summary>
    public class ODataPayloadElementAddDefaultAtomMetadata : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            payloadElement.WithContentType("application/xml");
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            XmlTreeAnnotation updated = XmlTreeAnnotation.Atom(TestAtomConstants.AtomUpdatedElementName, "2013-08-13T01:03:16.7800000");
            updated.SetValueEqualityFunc((obj1, obj2) =>
            {
                return true;
            });
            payloadElement.Annotations.Add(updated);
            payloadElement.Add(XmlTreeAnnotation.Atom("title", null));
            
            //If a feed is empty it must have an author
            bool feedEmpty = payloadElement.Count == 0;
            if (feedEmpty)
            {
                XmlTreeAnnotation author = XmlTreeAnnotation.Atom(TestAtomConstants.AtomAuthorElementName,
                  null, XmlTreeAnnotation.Atom(TestAtomConstants.AtomAuthorNameElementName, null));
                payloadElement.Annotations.Add(author);
            }

            base.Visit(payloadElement);
        }
    }
}
