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

#if ASTORIA_SERVER
namespace System.Data.Services
#else
namespace System.Data.Services.Client
#endif
{
    using System.Diagnostics;
    using System.Linq.Expressions;

    /// <summary>Provides an expression visitor that can replace a <see cref="ParameterExpression"/>.</summary>
    internal class ParameterReplacerVisitor : ALinqExpressionVisitor
    {
        /// <summary>Expression to replace with.</summary>
        private readonly Expression newExpression;

        /// <summary>Parameter to replace.</summary>
        private readonly ParameterExpression oldParameter;

        /// <summary>Initializes a new <see cref="ParameterReplacerVisitor"/> instance.</summary>
        /// <param name="oldParameter">Parameter to replace.</param>
        /// <param name="newExpression">Expression to replace with.</param>
        private ParameterReplacerVisitor(ParameterExpression oldParameter, Expression newExpression)
        {
            this.oldParameter = oldParameter;
            this.newExpression = newExpression;
        }

        /// <summary>
        /// Replaces the occurences of <paramref name="oldParameter"/> for <paramref name="newExpression"/> in
        /// <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">Expression to perform replacement on.</param>
        /// <param name="oldParameter">Parameter to replace.</param>
        /// <param name="newExpression">Expression to replace with.</param>
        /// <returns>A new expression with the replacement performed.</returns>
        internal static Expression Replace(Expression expression, ParameterExpression oldParameter, Expression newExpression)
        {
            Debug.Assert(expression != null, "expression != null");
            Debug.Assert(oldParameter != null, "oldParameter != null");
            Debug.Assert(newExpression != null, "newExpression != null");
            return new ParameterReplacerVisitor(oldParameter, newExpression).Visit(expression);
        }

        /// <summary>ParameterExpression visit method.</summary>
        /// <param name="p">The ParameterExpression expression to visit</param>
        /// <returns>The visited ParameterExpression expression </returns>
        internal override Expression VisitParameter(ParameterExpression p)
        {
            return p == this.oldParameter ? this.newExpression : p;
        }
    }
}
