//---------------------------------------------------------------------
// <copyright file="RawTextPayloadElementRepresentationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// An annotation which stores the exact text representation of an ODataPayloadElement.
    /// </summary>
    public class RawTextPayloadElementRepresentationAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the text representing the payload element the annotation is on.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "Text representation: " + this.Text;
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new RawTextPayloadElementRepresentationAnnotation { Text = this.Text };
        }
    }
}
