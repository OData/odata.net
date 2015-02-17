//---------------------------------------------------------------------
// <copyright file="JsonLightMetadataDocumentUriAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.JsonLight
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation which stores the metadata document URI for a top-level payload element in JSON Light.
    /// </summary>
    public class JsonLightMetadataDocumentUriAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// The string representation of the metadata document URI.
        /// </summary>
        public string MetadataDocumentUri { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "JSON Light metadata document URI: " + (this.MetadataDocumentUri == null ? "<null>" : this.MetadataDocumentUri);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation.
        /// </summary>
        /// <returns>A clone of the annotation.</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonLightMetadataDocumentUriAnnotation { MetadataDocumentUri = this.MetadataDocumentUri };
        }
    }
}
