//---------------------------------------------------------------------
// <copyright file="FilterClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// The result of parsing a $filter query option.
    /// </summary>
    public sealed class FilterClause
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
    }
}