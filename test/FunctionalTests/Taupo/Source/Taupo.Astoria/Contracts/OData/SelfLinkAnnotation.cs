//---------------------------------------------------------------------
// <copyright file="SelfLinkAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// An annotation containing the self link of an entity seen during deserialization from xml
    /// </summary>
    public class SelfLinkAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the SelfLinkAnnotation class
        /// </summary>
        /// <param name="value">The value of the annotation</param>
        public SelfLinkAnnotation(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value of the annotation
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "SelfLink:{0}", this.Value);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new SelfLinkAnnotation(this.Value);
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<SelfLinkAnnotation>(other, o => o.Value == this.Value);
        }
    }
}
