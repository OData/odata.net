//---------------------------------------------------------------------
// <copyright file="JsonLightContextUriAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.JsonLight
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation which stores the context URI for a top-level payload element in JSON Light.
    /// </summary>
    public class JsonLightContextUriAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// The string representation of the context URI.
        /// </summary>
        public string ContextUri { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "JSON Light context URI: " + (this.ContextUri == null ? "<null>" : this.ContextUri);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonLightContextUriAnnotation { ContextUri = this.ContextUri };
        }
    }
}
