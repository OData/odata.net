//---------------------------------------------------------------------
// <copyright file="EntitySetAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Payload annotation containing an entity set
    /// </summary>
    public class EntitySetAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the entity set
        /// </summary>
        public EdmEntitySet EdmEntitySet { get; set; }
        
        /// <summary>
        /// Gets or sets the entity set
        /// </summary>
        public EntitySet EntitySet { get; set; }
        
        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                if (this.EdmEntitySet != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "EdmEntitySet:{0}", this.EdmEntitySet.Name);
                }
                else
                {
                    return string.Format(CultureInfo.InvariantCulture, "EntitySet:{0}", this.EntitySet.Name);
                }
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new EntitySetAnnotation
            {
                EntitySet = this.EntitySet,
                EdmEntitySet = this.EdmEntitySet
            };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<EntitySetAnnotation>(
                other, o => o.EntitySet == this.EntitySet && o.EdmEntitySet == this.EdmEntitySet);
        }
    }
}
