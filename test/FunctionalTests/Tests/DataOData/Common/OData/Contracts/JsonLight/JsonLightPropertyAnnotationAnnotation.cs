//---------------------------------------------------------------------
// <copyright file="JsonLightPropertyAnnotationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.JsonLight
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation which represents a Json Lite Property Annotation.
    /// </summary>
    public class JsonLightPropertyAnnotationAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the annotation name.
        /// </summary>
        public string AnnotationName { get; set; }

        /// <summary>
        /// Gets or sets the annotation value.
        /// </summary>
        public string AnnotationValue { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "JSON Lite property annotation: " + (this.AnnotationName ?? "<null>") + ":" + (this.AnnotationValue ?? "<null>");
            }
        }

        /// <summary>
        /// Creates a clone of the annotation.
        /// </summary>
        /// <returns>A clone of the annotation.</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonLightPropertyAnnotationAnnotation { AnnotationName = this.AnnotationName, AnnotationValue = this.AnnotationValue };
        }
    }
}