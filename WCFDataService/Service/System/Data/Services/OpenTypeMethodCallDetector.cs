//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
