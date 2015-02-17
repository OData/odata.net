//---------------------------------------------------------------------
// <copyright file="CollectionNameAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// Represents the name of a top-level collection in xml
    /// </summary>
    public class CollectionNameAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the name of the collection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "CollectionName:{0}", this.Name);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new CollectionNameAnnotation { Name = this.Name };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<CollectionNameAnnotation>(other, o => o.Name == this.Name);
        }
    }
}
