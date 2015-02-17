//---------------------------------------------------------------------
// <copyright file="ContentTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// An annotation to capture the mime-type used on the content section when serializing an entity as Atom
    /// </summary>
    public class ContentTypeAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the ContentTypeAnnotation class.
        /// </summary>
        /// <param name="value">The value of the annotation</param>
        public ContentTypeAnnotation(string value)
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
                return string.Format(CultureInfo.InvariantCulture, "ContentType:{0}", this.Value);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new ContentTypeAnnotation(this.Value);
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<ContentTypeAnnotation>(other, o => o.Value == this.Value);
        }
    }
}
