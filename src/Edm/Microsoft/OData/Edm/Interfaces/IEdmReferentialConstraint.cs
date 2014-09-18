//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Edm
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an EDM referential constraint on a navigation property.
    /// </summary>
    public interface IEdmReferentialConstraint
    {
        /// <summary>
        /// Gets the set of property pairs from the referential constraint. No particular ordering should be assumed.
        /// </summary>
        IEnumerable<EdmReferentialConstraintPropertyPair> PropertyPairs { get; }
    }

    /// <summary>
    /// Represents a pair of properties as part of a referential constraint.
    /// </summary>
    public class EdmReferentialConstraintPropertyPair
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EdmReferentialConstraintPropertyPair"/>.
        /// </summary>
        /// <param name="dependentProperty">The local or dependent property in the referential constraint property pair. A null value is not allowed.</param>
        /// <param name="principalProperty">The foreign or principal property in the referential constraint property pair. A null value is not allowed.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if either of the properties given are null.</exception>
        public EdmReferentialConstraintPropertyPair(IEdmStructuralProperty dependentProperty, IEdmStructuralProperty principalProperty)
        {
            EdmUtil.CheckArgumentNull(dependentProperty, "dependentProperty");
            EdmUtil.CheckArgumentNull(principalProperty, "principalProperty");

            this.DependentProperty = dependentProperty;
            this.PrincipalProperty = principalProperty;
        }

        /// <summary>
        /// The local or dependent property in the referential constraint property pair. Will never be null.
        /// </summary>
        public IEdmStructuralProperty DependentProperty { get; private set; }

        /// <summary>
        /// The foreign or principal property in the referential constraint property pair. Will never be null.
        /// </summary>
        public IEdmStructuralProperty PrincipalProperty { get; private set; }
    }
}
