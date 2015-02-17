//---------------------------------------------------------------------
// <copyright file="OpenTypeMethodCallDetector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Microsoft.OData.Service.Providers;

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