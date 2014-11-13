//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
