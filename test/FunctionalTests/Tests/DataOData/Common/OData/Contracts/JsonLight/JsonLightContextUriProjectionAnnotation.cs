//---------------------------------------------------------------------
// <copyright file="JsonLightContextUriProjectionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.JsonLight
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation which stores the context URI projection for a top-level payload element in JSON Light.
    /// </summary>
    public class JsonLightContextUriProjectionAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// A projection annotation for the case where no projection is desired ($select=)
        /// </summary>
        public static readonly JsonLightContextUriProjectionAnnotation EmptyProjectionAnnotation =
            new JsonLightContextUriProjectionAnnotation { ContextUriProjection = string.Empty };

        /// <summary>
        /// The string representation of the context URI projection.
        /// </summary>
        public string ContextUriProjection { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "JSON Light context URI projection: " + (this.ContextUriProjection == null ? "<null>" : this.ContextUriProjection);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonLightContextUriProjectionAnnotation { ContextUriProjection = this.ContextUriProjection };
        }
    }
}
