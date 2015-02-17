//---------------------------------------------------------------------
// <copyright file="CollectionItemElementNameAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// Represents the per-item element name to use when serializing collections
    /// </summary>
    public class CollectionItemElementNameAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the local name for the element
        /// </summary>
        public string LocalName { get; set; }

        /// <summary>
        /// Gets or sets the namespace name for the element
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "CollectionElementName:{0}.{1}", this.NamespaceName, this.LocalName);
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new CollectionItemElementNameAnnotation
            {
                LocalName = this.LocalName,
                NamespaceName = this.NamespaceName,
            };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<CollectionItemElementNameAnnotation>(other, o => o.LocalName == this.LocalName && o.NamespaceName == this.NamespaceName);
        }
    }
}
