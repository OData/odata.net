//   OData .NET Libraries ver. 5.6.3
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

namespace System.Data.Services.Client
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

#if WINDOWS_PHONE_MANGO
        /// <summary>
        /// Creates a SkipQueryOption expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="skipAmount">the query option value</param>
        internal SkipQueryOptionExpression(Type type, ConstantExpression skipAmount)
            : base((ExpressionType)ResourceExpressionType.SkipQueryOption, type)
        {
            this.skipAmount = skipAmount;
        }
#else
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

#endif
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
