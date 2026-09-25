//---------------------------------------------------------------------
// <copyright file="RangeVariable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A RangeVariable, which represents an iterator variable either over a collection, either of entities or not.
    /// Exists outside of the main SemanticAST, but hooked in via a RangeVariableReferenceNode (either Non-Entity or Entity).
    /// </summary>
    public abstract class RangeVariable : IEquatable<RangeVariable>
    {
        /// <summary>
        /// Gets the name of the associated rangeVariable.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the type of entity referenced by this rangeVariable
        /// </summary>
        public abstract IEdmTypeReference TypeReference { get; }

        /// <summary>
        /// Gets the kind of this rangeVariable.
        /// </summary>
        public abstract int Kind { get; }

        /// <summary>
        /// Determines whether the specified <see cref="RangeVariable"/> is equal to the current <see cref="RangeVariable"/>.
        /// </summary>
        /// <param name="other">The <see cref="RangeVariable"/> to compare with the current <see cref="RangeVariable"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="RangeVariable"/> is equal to the current <see cref="RangeVariable"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(RangeVariable other)
        {
            return SemanticAstStructuralEqualityComparer.AreEqual(this, other);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="RangeVariable"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="RangeVariable"/>.</param>
        /// <returns><c>true</c> if the specified object is equal to the current <see cref="RangeVariable"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as RangeVariable);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="RangeVariable"/>.</returns>
        public override int GetHashCode()
        {
            return SemanticAstStructuralEqualityComparer.GetHashCode(this);
        }
    }
}