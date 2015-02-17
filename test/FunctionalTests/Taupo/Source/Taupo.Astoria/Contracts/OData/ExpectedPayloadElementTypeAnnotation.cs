//---------------------------------------------------------------------
// <copyright file="ExpectedPayloadElementTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Annotation to represent the expected type of a payload based on some external context or knowledge. Used exclusively for ambiguous payload normalization.
    /// </summary>
    public class ExpectedPayloadElementTypeAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the expected type.
        /// </summary>
        public ODataPayloadElementType ExpectedType { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get { return "ExpectedType:" + this.ExpectedType; }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>
        /// a clone of the annotation
        /// </returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new ExpectedPayloadElementTypeAnnotation() { ExpectedType = this.ExpectedType };
        }
    }
}
