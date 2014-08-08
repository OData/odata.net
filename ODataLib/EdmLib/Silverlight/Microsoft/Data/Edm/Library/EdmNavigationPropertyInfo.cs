//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
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
                ContainsTarget = this.ContainsTarget,
                OnDelete = this.OnDelete
            };
        }
    }
}
