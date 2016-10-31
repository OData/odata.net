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

namespace System.Data.Services
{
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq.Expressions;

#if DEBUG
    /// <summary>Detects calls to OpenTypeMethods members and asserts if it finds any.</summary>
    internal class OpenTypeMethodCallDetector : ALinqExpressionVisitor
    {
        /// <summary> summary</summary>
        private const string UnexpectedCallErrorMessage = "Unexpected call to OpenTypeMethods found in query when the provider did not have any OpenTypes.";

        /// <summary>Public interface for using this class.</summary>
        /// <param name="input">Input expression.</param>
        public static void CheckForOpenTypeMethodCalls(Expression input)
        {
            new OpenTypeMethodCallDetector().Visit(input);
        }

        /// <summary>Forgiving Visit method which allows unknown expression types to pass through.</summary>
        /// <param name="exp">Input expression.</param>
        /// <returns>Visit expression.</returns>
        internal override Expression Visit(Expression exp)
        {
            try
            {
                return base.Visit(exp);
            }
            catch (NotSupportedException)
            {
                return exp;
            }
        }

        /// <summary>Method call visitor.</summary>
        /// <param name="m">Method call being visited.</param>
        /// <returns>Whatever is provided on input.</returns>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(OpenTypeMethods))
            {
                Debug.Fail(UnexpectedCallErrorMessage);
            }

            return base.VisitMethodCall(m);
        }

        /// <summary>
        /// BinaryExpression visit method
        /// </summary>
        /// <param name="b">The BinaryExpression expression to visit</param>
        /// <returns>The visited BinaryExpression expression </returns>
        internal override Expression VisitBinary(BinaryExpression b)
        {
            if (b.Method != null && b.Method.DeclaringType == typeof(OpenTypeMethods))
            {
                Debug.Fail(UnexpectedCallErrorMessage);
            }

            return base.VisitBinary(b);
        }

        /// <summary>
        /// UnaryExpression visit method
        /// </summary>
        /// <param name="u">The UnaryExpression expression to visit</param>
        /// <returns>The visited UnaryExpression expression </returns>
        internal override Expression VisitUnary(UnaryExpression u)
        {
            if (u.Method != null && u.Method.DeclaringType == typeof(OpenTypeMethods))
            {
                Debug.Fail(UnexpectedCallErrorMessage);
            }

            return base.VisitUnary(u);
        }
    }
#endif
}
