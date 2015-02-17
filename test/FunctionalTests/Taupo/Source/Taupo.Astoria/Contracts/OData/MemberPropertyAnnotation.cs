//---------------------------------------------------------------------
// <copyright file="MemberPropertyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Payload annotation containing a member property 
    /// </summary>
    public class MemberPropertyAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the member property
        /// </summary>
        public MemberProperty Property { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "MemberProperty:{0}", this.Property.Name);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new MemberPropertyAnnotation { Property = this.Property };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<MemberPropertyAnnotation>(other, o => o.Property == this.Property);
        }
    }
}
