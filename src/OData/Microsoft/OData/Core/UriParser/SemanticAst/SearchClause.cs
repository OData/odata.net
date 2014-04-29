//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    /// <summary>
    /// The result of parsing a $search query option.
    /// </summary>
    public sealed class SearchClause
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
    }
}
