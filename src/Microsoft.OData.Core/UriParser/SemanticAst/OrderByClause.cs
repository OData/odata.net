//---------------------------------------------------------------------
// <copyright file="OrderByClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Represents the result of parsing the $orderby query option.
    /// </summary>
    public sealed class OrderByClause
    {
        /// <summary>
        /// The order-by expression.
        /// </summary>
        private readonly SingleValueNode expression;

        /// <summary>
        /// The direction to order.
        /// </summary>
        private readonly OrderByDirection direction;

        /// <summary>
        /// The rangeVariable for the expression which represents a single value from the collection we iterate over.
        /// </summary>
        private readonly RangeVariable rangeVariable;

        /// <summary>
        /// The next orderby to perform after performing this orderby, can be null in the case of only a single orderby expression.
        /// </summary>
        private readonly OrderByClause thenBy;

        /// <summary>
        /// Creates an <see cref="OrderByClause"/>.
        /// </summary>
        /// <param name="thenBy">The next orderby to perform after performing this orderby, can be null in the case of only a single orderby expression.</param>
        /// <param name="expression">The order-by expression. Cannot be null.</param>
        /// <param name="direction">The direction to order.</param>
        /// <param name="rangeVariable">The rangeVariable for the expression which represents a single value from the collection we iterate over. </param>
        /// <exception cref="System.ArgumentNullException">Throws if the input expression or rangeVariable is null.</exception>
        public OrderByClause(OrderByClause thenBy, SingleValueNode expression, OrderByDirection direction, RangeVariable rangeVariable)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(rangeVariable, "parameter");

            this.thenBy = thenBy;
            this.expression = expression;
            this.direction = direction;
            this.rangeVariable = rangeVariable;
        }

        /// <summary>
        /// Gets the next orderby to perform after performing this orderby, can be null in the case of only a single orderby expression.
        /// </summary>
        public OrderByClause ThenBy
        {
            get
            {
                return this.thenBy;
            }
        }

        /// <summary>
        /// Gets the order-by expression.
        /// </summary>
        public SingleValueNode Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the direction to order.
        /// </summary>
        public OrderByDirection Direction
        {
            get { return this.direction; }
        }

        /// <summary>
        /// Gets the rangeVariable for the expression which represents a single value from the collection we iterate over.
        /// </summary>
        public RangeVariable RangeVariable
        {
            get { return this.rangeVariable; }
        }

        /// <summary>
        /// Gets the type of a single item from the collection returned after ordering.
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