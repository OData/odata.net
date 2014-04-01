//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
