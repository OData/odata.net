//---------------------------------------------------------------------
// <copyright file="EdmNavigationPropertyInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM navigation property info used during construction of navigation properties.
    /// </summary>
    public sealed class EdmNavigationPropertyInfo
    {
        /// <summary>
        /// Gets or sets the name of this navigation property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the entity type that this navigation property leads to.
        /// </summary>
        public IEdmEntityType Target { get; set; }

        /// <summary>
        /// Gets or sets multiplicity of the navigation target.
        /// </summary>
        public EdmMultiplicity TargetMultiplicity { get; set; }

        /// <summary>
        /// Gets or sets the dependent properties of the association this navigation property expresses.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DependentProperties { get; set; }

        /// <summary>
        /// Gets or sets the principal properties of the association this navigation property expresses.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> PrincipalProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        public bool ContainsTarget { get; set; }

        /// <summary>
        /// Gets or sets the action to take when an instance of the declaring type is deleted.
        /// </summary>
        public EdmOnDeleteAction OnDelete { get; set; }

        /// <summary>
        /// Clones this object.
        /// </summary>
        /// <returns>A copy of this object.</returns>
        public EdmNavigationPropertyInfo Clone()
        {
            return new EdmNavigationPropertyInfo()
            {
                Name = this.Name,
                Target = this.Target,
                TargetMultiplicity = this.TargetMultiplicity,
                DependentProperties = this.DependentProperties,
                PrincipalProperties = this.PrincipalProperties,
                ContainsTarget = this.ContainsTarget,
                OnDelete = this.OnDelete
            };
        }
    }
}
