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
