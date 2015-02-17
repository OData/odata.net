//---------------------------------------------------------------------
// <copyright file="LateBoundExpressionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.LateBound
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Client;
    using System.Linq;

    /// <summary>
    /// Late Bound Expression Visitor
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Instantiated by reflection")]
    internal class LateBoundExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Creates a new instance of LateBoundExpressionVisitor
        /// </summary>
        /// <param name="evaluator">An evaluator class in which expressions will be bound to.</param>
        /// <param name="provider">The provider</param>
        internal LateBoundExpressionVisitor(OpenTypeMethodsImplementation openTypeImplementation, DSPMethodsImplementation dspImplementation)
        {
            this.OpenTypeMethodsImplementation = openTypeImplementation;
            this.DSPMethodsImplementation = dspImplementation;
        }

        public OpenTypeMethodsImplementation OpenTypeMethodsImplementation
        {
            get;
            set;
        }

        public DSPMethodsImplementation DSPMethodsImplementation
        {
            get;
            set;
        }

        #region Internal Methods

        /// <summary>
        /// Visits a method call expression
        /// </summary>
        /// <param name="m">The method call expression</param>
        /// <returns>A new expression after the parameter is visited</returns>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            MethodInfo method = m.Method;
            if (method.ReflectedType == typeof(DataServiceProviderMethods))
            {
                if (DSPMethodsImplementation.ShouldReplaceMethod(method))
                {
                    return DSPMethodsImplementation.ConvertMethodCall(method, this.VisitExpressionList(m.Arguments));
                }
                else
                {
                    return base.VisitMethodCall(m);
                }
            }
            else if (method.ReflectedType == typeof(OpenTypeMethods))
            {
                if (OpenTypeMethodsImplementation.UseLazyBooleanEvaluation)
                {
                    Expression left = this.Visit(m.Arguments[0]);
                    Expression right = this.Visit(m.Arguments[1]);
                    ParameterExpression param = Expression.Parameter(typeof(object), "leftSideLazyEvaluator");
                    if (method.Name.StartsWith("And") || method.Name.StartsWith("Or"))
                    {
                        bool lazyValue = method.Name.StartsWith("And");

                        // if the left-hand side evaluates to a boolean constant that allows lazy evaluation, 
                        // then do not evaluate the right hand side
                        right = Expression.Condition(
                            Expression.And(
                                Expression.TypeIs(param, typeof(bool)),
                                Expression.Equal(Expression.Convert(param, typeof(bool)), Expression.Constant(!lazyValue))),
                            Expression.Constant(false, typeof(object)),
                            right);
                    }
                    Expression call = OpenTypeMethodsImplementation.ConvertMethodCall(method, param, right);
                    return Expression.Invoke(Expression.Lambda(call, param), left);
                }
                else
                {
                    return OpenTypeMethodsImplementation.ConvertMethodCall(method, this.VisitExpressionList(m.Arguments));
                }
            }
            else if (method.ReflectedType == typeof(Queryable))
            {
                if (method.Name.StartsWith("OrderBy") || method.Name.StartsWith("ThenBy"))
                {
                    // untyped orderby, must be late bound, use our comparison instead of the default
                    if (method.GetGenericArguments()[1] == typeof(object))
                    {
                        return Expression.Call(typeof(Queryable), method.Name, method.GetGenericArguments(),
                            this.Visit(m.Arguments[0]), this.Visit(m.Arguments[1]), Expression.Constant(this.OpenTypeMethodsImplementation));
                    }
                }
            }
                 
            return base.VisitMethodCall(m);
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            MemberInfo method = m.Member;
            if (method.ReflectedType == typeof(OpenTypeMethods) ||
                method.ReflectedType == typeof(DataServiceProviderMethods))
            {
                throw new NotImplementedException("How did we get here?");
            }

            return base.VisitMemberAccess(m);
        }

        /// <summary>
        /// BinaryExpression visit method
        /// </summary>
        /// <param name="b">The BinaryExpression expression to visit</param>
        /// <returns>The visited BinaryExpression expression </returns>
        internal override Expression VisitUnary(UnaryExpression u)
        {
            MethodInfo method = u.Method;
            if (method != null && method.ReflectedType == typeof(OpenTypeMethods))
            {
                return OpenTypeMethodsImplementation.ConvertMethodCall(method, this.Visit(u.Operand));
            }

            return base.VisitUnary(u);
        }

        /// <summary>
        /// BinaryExpression visit method
        /// </summary>
        /// <param name="b">The BinaryExpression expression to visit</param>
        /// <returns>The visited BinaryExpression expression </returns>
        internal override Expression VisitBinary(BinaryExpression b)
        {
            MethodInfo method = b.Method;
            if (method != null && method.ReflectedType == typeof(OpenTypeMethods))
            {
                return OpenTypeMethodsImplementation.ConvertMethodCall(method, this.Visit(b.Left), this.Visit(b.Right));
            }

            return base.VisitBinary(b);
        }
        #endregion
    }
}