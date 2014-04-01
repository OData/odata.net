//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Enumerates the multiplicities of EDM navigation properties.
    /// </summary>
    public enum EdmMultiplicity
    {
        /// <summary>
        /// The Multiplicity of the association end is unknown.
        /// </summary>
        Unknown = 0,

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
        None = 0,

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
        /// Gets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        bool ContainsTarget { get; }

        /// <summary>
        /// Gets the referential constraint for this navigation, returning null if this is the principal end or if there is no referential constraint.
        /// </summary>
        IEdmReferentialConstraint ReferentialConstraint { get; }
    }
}
