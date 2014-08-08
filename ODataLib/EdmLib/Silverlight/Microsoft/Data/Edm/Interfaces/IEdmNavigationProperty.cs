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

using System.Collections.Generic;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Enumerates the multiplicities of EDM navigation properties.
    /// </summary>
    public enum EdmMultiplicity
    {
        /// <summary>
        /// The Multiplicity of the association end is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The Multiplicity of the association end is zero or one.
        /// </summary>
        ZeroOrOne,

        /// <summary>
        /// The Multiplicity of the association end is one.
        /// </summary>
        One,

        /// <summary>
        /// The Multiplicity of the association end is many.
        /// </summary>
        Many
    }

    /// <summary>
    /// Enumerates the actions EDM can apply on deletes.
    /// </summary>
    public enum EdmOnDeleteAction
    {
        /// <summary>
        /// Take no action on delete.
        /// </summary>
        None,

        /// <summary>
        /// On delete also delete items on the other end of the association.
        /// </summary>
        Cascade
    }

    /// <summary>
    /// Represents an EDM navigation property.
    /// </summary>
    public interface IEdmNavigationProperty : IEdmProperty
    {
        /// <summary>
        /// Gets the partner of this navigation property.
        /// </summary>
        IEdmNavigationProperty Partner { get; }

        /// <summary>
        /// Gets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        EdmOnDeleteAction OnDelete { get; }

        /// <summary>
        /// Gets whether this navigation property originates at the principal end of an association.
        /// </summary>
        bool IsPrincipal { get; }

        /// <summary>
        /// Gets the dependent properties of this navigation property, returning null if this is the principal end or if there is no referential constraint.
        /// </summary>
        IEnumerable<IEdmStructuralProperty> DependentProperties { get; }

        /// <summary>
        /// Gets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        bool ContainsTarget { get; }
    }
}
