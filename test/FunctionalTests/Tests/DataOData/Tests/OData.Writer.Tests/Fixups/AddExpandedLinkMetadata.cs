//---------------------------------------------------------------------
// <copyright file="AddExpandedLinkMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    
    /// <summary>
    /// Adds metadata to navigation properties and expanded links
    /// </summary>
    public class AddExpandedLinkMetadata : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            string contenttype = null;
            if (payloadElement.IsSingleEntity)
            {
                contenttype = "application/atom+xml;type=entry";
            }
            else
            {
                contenttype = "application/atom+xml;type=feed";
            }
            payloadElement.Add(new ContentTypeAnnotation(contenttype));
            if (payloadElement.ExpandedElement != null)
            {
                this.Recurse(payloadElement.ExpandedElement);
            }
        }
        
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            
            if (payloadElement.Value != null)
            {
                payloadElement.Value = (payloadElement.Value.WithTitleAttribute(payloadElement.Name));
                this.Recurse(payloadElement.Value);
            }
        }
    }
}
