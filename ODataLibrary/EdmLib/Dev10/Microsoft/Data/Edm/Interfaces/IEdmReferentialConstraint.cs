//   Copyright 2011 Microsoft Corporation
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

using System.Collections.Generic;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM referential constraint in an association type.
    /// </summary>
    public interface IEdmReferentialConstraint : IEdmElement
    {
        /// <summary>
        /// Gets the principal end of this referential constraint.
        /// </summary>
        IEdmAssociationEnd PrincipalEnd { get; }

        /// <summary>
        /// Gets the dependent properties of this referential constraint. (The principal properties of the constraint are the key of the principal end)
        /// </summary>
        IEnumerable<IEdmStructuralProperty> DependentProperties { get; }
    }
}
