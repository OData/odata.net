//---------------------------------------------------------------------
// <copyright file="ReplacedElementAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// Represents the original value for a payload element that has been replaced
    /// </summary>
    public class ReplacedElementAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the original element that was replaced
        /// </summary>
        public ODataPayloadElement Original { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                if (this.Original == null)
                {
                    return "ReplacedElement:null";
                }

                return string.Format(CultureInfo.InvariantCulture, "ReplacedElement: {{ {0} }}", this.Original.StringRepresentation);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new ReplacedElementAnnotation { Original = this.Original };
        }
    }
}
