//---------------------------------------------------------------------
// <copyright file="FilterClause.cs" company="Microsoft">
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
    /// The result of parsing a $filter query option.
    /// </summary>
    public sealed class FilterClause : IEquatable<FilterClause>
    {
        /// <summary>
        /// The filter expression - this should evaluate to a single boolean value.
        /// </summary>
        private readonly SingleValueNode expression;

        /// <summary>
        /// The parameter for the expression which represents a single value from the collection.
        /// </summary>
        private readonly RangeVariable rangeVariable;

        /// <summary>
        /// Creates a <see cref="FilterClause"/>.
        /// </summary>
        /// <param name="expression">The filter expression - this should evaluate to a single boolean value. Cannot be null.</param>
        /// <param name="rangeVariable">The parameter for the expression which represents a single value from the collection. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input expression or rangeVariable is null.</exception>
        public FilterClause(SingleValueNode expression, RangeVariable rangeVariable)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(rangeVariable, "parameter");

            this.expression = expression;
            this.rangeVariable = rangeVariable;
        }

        /// <summary>
        /// Gets the filter expression - this should evaluate to a single boolean value.
        /// </summary>
        public SingleValueNode Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the parameter for the expression which represents a single value from the collection.
        /// </summary>
        public RangeVariable RangeVariable
        {
            get { return this.rangeVariable; }
        }

        /// <summary>
        /// Gets the type of item returned by this clause.
        /// </summary>
        public IEdmTypeReference ItemType
        {
            get
            {
                return this.RangeVariable.TypeReference;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="FilterClause"/> is equal to the current <see cref="FilterClause"/>.
        /// </summary>
        /// <param name="other">The <see cref="FilterClause"/> to compare with the current <see cref="FilterClause"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="FilterClause"/> is equal to the current <see cref="FilterClause"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(FilterClause other)
        {
            return SemanticAstStructuralEqualityComparer.AreEqual(this, other);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="FilterClause"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="FilterClause"/>.</param>
        /// <returns><c>true</c> if the specified object is equal to the current <see cref="FilterClause"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as FilterClause);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="FilterClause"/>.</returns>
        public override int GetHashCode()
        {
            return SemanticAstStructuralEqualityComparer.GetHashCode(this);
        }
    }
}