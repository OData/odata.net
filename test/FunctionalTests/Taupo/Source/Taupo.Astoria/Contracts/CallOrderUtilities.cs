//---------------------------------------------------------------------
// <copyright file="CallOrderUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <summary>
    /// Utility methods for dealing with call order
    /// </summary>
    public static class CallOrderUtilities
    {
        /// <summary>
        /// Gets or sets the current wrapper scope
        /// </summary>
        public static IWrapperScope CurrentScope { get; set; }
                
        /// <summary>
        /// Tries to wrap the given method call
        /// </summary>
        /// <param name="callExpression">An expression containing the method call to wrap</param>
        /// <param name="realWork">A delegate for doing the actual work of the method</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Need this type to be able to call with a lambda")]
        public static void TryWrapArbitraryMethodCall(Expression<Action> callExpression, Action realWork)
        {
            if (realWork == null)
            {
                TryWrapArbitraryMethodCall<object>((LambdaExpression)callExpression, () => null);
            }
            else
            {
                TryWrapArbitraryMethodCall<object>((LambdaExpression)callExpression, () => { realWork(); return null; });
            }
        }

        /// <summary>
        /// Tries to wrap the given method call
        /// </summary>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="callExpression">An expression containing the method call to wrap</param>
        /// <param name="realWork">A delegate for doing the actual work of the method</param>
        /// <returns>The return value of the given delegate</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Need this type to be able to call with a lambda")]
        public static TReturn TryWrapArbitraryMethodCall<TReturn>(Expression<Func<TReturn>> callExpression, Func<TReturn> realWork)
        {
            return TryWrapArbitraryMethodCall<TReturn>((LambdaExpression)callExpression, realWork);
        }

        private static TReturn TryWrapArbitraryMethodCall<TReturn>(LambdaExpression callExpression, Func<TReturn> realWork)
        {
            ExceptionUtilities.CheckArgumentNotNull(callExpression, "callExpression");
            ExceptionUtilities.CheckObjectNotNull(realWork, "realWork");

            var methodCall = callExpression.Body as MethodCallExpression;
            ExceptionUtilities.CheckObjectNotNull(methodCall, "Expression must be a method call");
            var methodInfo = methodCall.Method;

            object instance = null;
            if (methodCall.Object != null)
            {
                instance = ExtractConstantValue(methodCall.Object);
            }

            IWrapperScope scope = CurrentScope;
            var wrapped = instance as IWrappedObject;
            if (wrapped != null)
            {
                scope = wrapped.Scope;
            }

            if (scope == null)
            {
                return realWork();
            }

            var argumentList = new List<object>();
            foreach (var argumentExpression in methodCall.Arguments)
            {
                argumentList.Add(ExtractConstantValue(argumentExpression));
            }

            var arguments = argumentList.ToArray();

            int callId = scope.BeginTraceCall(methodInfo, instance, arguments);
            try
            {
                var result = realWork();

                if (callId != 0)
                {
                    object returnValue = result;
                    scope.TraceResult(callId, methodInfo, instance, arguments, ref returnValue);
                    result = (TReturn)returnValue;
                }

                return result;
            }
            catch (Exception exception)
            {
                if (callId != 0)
                {
                    scope.TraceException(callId, methodInfo, instance, exception.GetBaseException());
                }

                throw;
            }
        }

        private static object ExtractConstantValue(Expression argumentExpression)
        {
            var memberExpression = argumentExpression as MemberExpression;
            if (memberExpression != null)
            {
                var field = memberExpression.Member as FieldInfo;
                ExceptionUtilities.CheckObjectNotNull(field, "Member expression did not refer to a field");
                var instance = ExtractConstantValue(memberExpression.Expression);
                return field.GetValue(instance);
            }

            var constantExpression = argumentExpression as ConstantExpression;
            ExceptionUtilities.CheckObjectNotNull(constantExpression, "Expression was not a constant or field");
            return constantExpression.Value;
        }
    }
}
