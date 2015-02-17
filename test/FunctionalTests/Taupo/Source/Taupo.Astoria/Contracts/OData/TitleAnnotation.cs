//---------------------------------------------------------------------
// <copyright file="TitleAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An annotation to capture the title attribute in the links or title element in set instance when serializing an entity as Atom
    /// </summary>
    public class TitleAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the TitleAnnotation class.
        /// </summary>
        /// <param name="value">The value of the annotation</param>
        internal TitleAnnotation(string value)
        {
            ExceptionUtilities.Assert(value != null, "Value for title annotation must not be null; use string.Empty if no value is desired.");
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
                return string.Format(CultureInfo.InvariantCulture, "Title:{0}", this.Value);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new TitleAnnotation(this.Value); 
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<TitleAnnotation>(other, o => o.Value == this.Value);
        }
    }
}
