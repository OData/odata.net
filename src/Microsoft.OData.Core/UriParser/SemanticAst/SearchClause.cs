//---------------------------------------------------------------------
// <copyright file="SearchClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// The result of parsing a $search query option.
    /// </summary>
    public sealed class SearchClause : IEquatable<SearchClause>
    {
        /// <summary>
        /// The filter expression  this should evaluate to a single boolean value.
        /// </summary>
        private readonly SingleValueNode expression;

        /// <summary>
        /// Creates a <see cref="SearchClause"/>.
        /// </summary>
        /// <param name="expression">The filter expression - this should evaluate to a single boolean value. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input expression or rangeVariable is null.</exception>
        public SearchClause(SingleValueNode expression)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");

            this.expression = expression;
        }

        /// <summary>
        /// Gets the filter expression - this should evaluate to a single boolean value.
        /// </summary>
        public SingleValueNode Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SearchClause"/> is equal to the current <see cref="SearchClause"/>.
        /// </summary>
        /// <param name="other">The <see cref="SearchClause"/> to compare with the current <see cref="SearchClause"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="SearchClause"/> is equal to the current <see cref="SearchClause"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(SearchClause other)
        {
            return SemanticAstStructuralEqualityComparer.AreEqual(this, other);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="SearchClause"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="SearchClause"/>.</param>
        /// <returns><c>true</c> if the specified object is equal to the current <see cref="SearchClause"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as SearchClause);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="SearchClause"/>.</returns>
        public override int GetHashCode()
        {
            return SemanticAstStructuralEqualityComparer.GetHashCode(this);
        }
    }
}