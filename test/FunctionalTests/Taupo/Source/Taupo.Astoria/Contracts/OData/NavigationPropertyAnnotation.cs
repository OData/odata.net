//---------------------------------------------------------------------
// <copyright file="NavigationPropertyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Payload annotation containing a navigation property
    /// </summary>
    public class NavigationPropertyAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the navigation property
        /// </summary>
        public NavigationProperty Property { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "NavigationProperty:{0}", this.Property.Name);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new NavigationPropertyAnnotation { Property = this.Property };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<NavigationPropertyAnnotation>(other, o => o.Property == this.Property);
        }
    }
}
