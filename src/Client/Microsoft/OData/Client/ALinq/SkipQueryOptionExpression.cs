//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    /// <summary>
    /// An resource specific expression representing a skip query option.
    /// </summary>
    [DebuggerDisplay("SkipQueryOptionExpression {SkipAmount}")]
    internal class SkipQueryOptionExpression : QueryOptionExpression
    {
        /// <summary>amount to skip</summary>
        private ConstantExpression skipAmount;

        /// <summary>
        /// Creates a SkipQueryOption expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="skipAmount">the query option value</param>
        internal SkipQueryOptionExpression(Type type, ConstantExpression skipAmount)
            : base(type)
        {
            this.skipAmount = skipAmount;
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.SkipQueryOption; }
        }

        /// <summary>
        /// query option value
        /// </summary>
        internal ConstantExpression SkipAmount
        {
            get
            {
                return this.skipAmount;
            }
        }

        /// <summary>
        /// Composes the <paramref name="previous"/> expression with this one when it's specified multiple times.
        /// </summary>
        /// <param name="previous"><see cref="QueryOptionExpression"/> to compose.</param>
        /// <returns>
        /// The expression that results from composing the <paramref name="previous"/> expression with this one.
        /// </returns>
        internal override QueryOptionExpression ComposeMultipleSpecification(QueryOptionExpression previous)
        {
            Debug.Assert(previous != null, "other != null");
            Debug.Assert(previous.GetType() == this.GetType(), "other.GetType == this.GetType() -- otherwise it's not the same specification");
            Debug.Assert(this.skipAmount != null, "this.skipAmount != null");
            Debug.Assert(
                this.skipAmount.Type == typeof(int),
                "this.skipAmount.Type == typeof(int) -- otherwise it wouldn't have matched the Enumerable.Skip(source, int count) signature");
            int thisValue = (int)this.skipAmount.Value;
            int previousValue = (int)((SkipQueryOptionExpression)previous).skipAmount.Value;
            return new SkipQueryOptionExpression(this.Type, Expression.Constant(thisValue + previousValue, typeof(int)));
        }
    }
}
