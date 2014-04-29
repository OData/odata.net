//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Library
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
        /// Gets or sets the entity type that this navigation property belongs to.
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
