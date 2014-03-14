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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces
    using Microsoft.Data.Edm;
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
