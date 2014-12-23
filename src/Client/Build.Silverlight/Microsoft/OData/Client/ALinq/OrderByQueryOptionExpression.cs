//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// An resource specific expression representing an OrderBy query option.
    /// </summary>
    internal class OrderByQueryOptionExpression : QueryOptionExpression
    {
        /// <summary> selectors for OrderBy query option</summary>
        private List<Selector> selectors;

        /// <summary>
        /// Creates an OrderByQueryOptionExpression expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="selectors">selectors for orderby expression</param>
        internal OrderByQueryOptionExpression(Type type, List<Selector> selectors)
            : base(type)
        {
            this.selectors = selectors; 
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.OrderByQueryOption; }
        }

        /// <summary>
        /// Selectors for OrderBy expression
        /// </summary>
        internal List<Selector> Selectors
        {
            get
            {
                return this.selectors;
            }
        }

        /// <summary>
        /// Structure for selectors.  Holds lambda expression + flag indicating desc.
        /// </summary>
        internal struct Selector
        {
            /// <summary>
            /// lambda expression for selector
            /// </summary>
            internal readonly Expression Expression;

            /// <summary>
            /// flag indicating if descending
            /// </summary>
            internal readonly bool Descending;

            /// <summary>
            /// Creates a Selector
            /// </summary>
            /// <param name="e">lambda expression for selector</param>
            /// <param name="descending">flag indicating if descending</param>
            internal Selector(Expression e, bool descending)
            {
                this.Expression = e;
                this.Descending = descending;
            }
        }
    }
}
