//---------------------------------------------------------------------
// <copyright file="DynamicProxyMethodGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Security;

    /// <summary>
    /// Generates proxy methods for external callers to call internal methods
    /// All lambda_methods are considered external. When these methods need
    /// to access internal resources, a proxy must be used. Otherwise the call
    /// will fail for partial trust scenario.
    /// </summary>
    internal class DynamicProxyMethodGenerator
    {
        /// <summary>
        /// Builds an expression to best call the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The original method or constructor</param>
        /// <param name="arguments">The arguments with which to call the method.</param>
        /// <returns>An expression to call the argument method or constructor</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "'this' parameter is required when compiling for the desktop.")]
        internal Expression GetCallWrapper(MethodBase method, params Expression[] arguments)
        {
            return WrapOriginalMethodWithExpression(method, arguments);
        }

        /// <summary>
        /// Wraps the specified <see cref="MethodBase"/> in an expression that invokes it.
        /// </summary>
        /// <param name="method">The method to wrap in an expression.</param>
        /// <param name="arguments">The arguments with which to invoke the <paramref name="method"/>.</param>
        /// <returns>An expression which invokes the <paramref name="method"/> with the specified <paramref name="arguments"/>.</returns>
        private static Expression WrapOriginalMethodWithExpression(MethodBase method, Expression[] arguments)
        {
            var methodInfo = method as MethodInfo;
            if (methodInfo != null)
            {
                return Expression.Call(methodInfo, arguments);
            }

            return Expression.New((ConstructorInfo)method, arguments);
        }
    }
}