//---------------------------------------------------------------------
// <copyright file="XmlBaseAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// An annotation containing the xml:base value seen when deserializing a payload
    /// </summary>
    public class XmlBaseAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the XmlBaseAnnotation class
        /// </summary>
        /// <param name="value">The value of the annotation</param>
        public XmlBaseAnnotation(string value)
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
                return string.Format(CultureInfo.InvariantCulture, "XmlBase:{0}", this.Value);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new XmlBaseAnnotation(this.Value);
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<XmlBaseAnnotation>(other, o => o.Value == this.Value);
        }
    }
}
