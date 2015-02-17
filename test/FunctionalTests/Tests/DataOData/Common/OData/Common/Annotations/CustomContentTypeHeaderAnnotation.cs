//---------------------------------------------------------------------
// <copyright file="CustomContentTypeHeaderAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Annotations
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotation on (top-level) payload elements to specify a custom content type annotation.
    /// </summary>
    public class CustomContentTypeHeaderAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Constructor that takes a content type.
        /// </summary>
        /// <param name="contentType">The content type to use.</param>
        public CustomContentTypeHeaderAnnotation(string contentType)
        {
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");

            this.ContentType = contentType;
        }

        /// <summary>
        /// The content type to be used.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// String representation of annotation.
        /// </summary>
        public override string StringRepresentation
        {
            get { return this.ContentType; }
        }

        /// <summary>
        /// Clone to create a copy.
        /// </summary>
        /// <returns></returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new CustomContentTypeHeaderAnnotation(this.ContentType);
        }
    }
}
