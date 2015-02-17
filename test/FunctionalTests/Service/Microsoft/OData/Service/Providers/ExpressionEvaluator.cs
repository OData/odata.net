//---------------------------------------------------------------------
// <copyright file="ExpressionEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Class to evaluate the expression we would pass to the execution provider.
    /// </summary>
    internal class ExpressionEvaluator : ALinqExpressionVisitor
    {
        /// <summary>MethodInfo for CreateNewArray.</summary>
        private static readonly MethodInfo CreateNewArrayMethodInfo = typeof(ExpressionEvaluator).GetMethod(
            "CreateNewArray",
            BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// Evaluates the given expression.
        /// </summary>
        /// <param name="exp">Expression to evaluate.</param>
        /// <returns>Final result of the evaluation.</returns>
        internal static object Evaluate(Expression exp)
        {
            Debug.Assert(exp != null, "exp != null");

            ExpressionEvaluator evaluator = new ExpressionEvaluator();
            ConstantExpression result = (ConstantExpression)evaluator.Visit(exp);
            return result.Value;
        }

        /// <summary>
        /// Main visit method for ALinqExpressionVisitor
        /// </summary>
        /// <param name="exp">The expression to visit</param>
        /// <returns>The visited expression </returns>
        internal override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return exp;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.NewArrayInit:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Quote:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                default:
                    throw new NotSupportedException(Strings.ALinq_UnsupportedExpression(exp.NodeType.ToString()));
            }
        }

        /// <summary>
        /// UnaryExpression visit method
        /// </summary>
        /// <param name="u">The UnaryExpression expression to visit</param>
        /// <returns>The visited UnaryExpression expression </returns>
        internal override Expression VisitUnary(UnaryExpression u)
        {
            return this.Visit(u.Operand);
        }

        /// <summary>
        /// MethodCallExpression visit method
        /// </summary>
        /// <param name="m">The MethodCallExpression expression to visit</param>
        /// <returns>The visited MethodCallExpression expression </returns>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
#pragma warning disable 618 // Disable "obsolete" warning for the IExpandProvider interface. Used for backwards compatibilty.
            Debug.Assert(
                m.Method.DeclaringType == typeof(Queryable) ||
                m.Method.DeclaringType == typeof(Enumerable) ||
                m.Method.DeclaringType == typeof(DataServiceExecutionProviderMethods) ||
                m.Method.DeclaringType == typeof(IExpandProvider),
                "Expecting method defined in Queryable, Enumerable, DataServiceExecutionProviderMethods or IExpandProvider. Current method: " + m.Method.DeclaringType.FullName + "." + m.Method.Name);
#pragma warning restore 618

            object instance = m.Object == null ? null : ((ConstantExpression)this.Visit(m.Object)).Value;
            try
            {
                return Expression.Constant(m.Method.Invoke(instance, this.VisitExpressionList(m.Arguments).Select(arg => ((ConstantExpression)arg).Value).ToArray()));
            }
            catch (TargetInvocationException tie)
            {
                ErrorHandler.HandleTargetInvocationException(tie);
                throw;
            }
        }

        /// <summary>
        /// LambdaExpression visit method
        /// </summary>
        /// <param name="lambda">The LambdaExpression to visit</param>
        /// <returns>The visited LambdaExpression</returns>
        internal override Expression VisitLambda(LambdaExpression lambda)
        {
            return Expression.Constant(lambda);
        }

        /// <summary>
        /// NewArrayExpression visit method
        /// </summary>
        /// <param name="na">The NewArrayExpression to visit</param>
        /// <returns>The visited NewArrayExpression</returns>
        internal override Expression VisitNewArray(NewArrayExpression na)
        {
            Debug.Assert(na.NodeType == ExpressionType.NewArrayInit, "na.NodeType == ExpressionType.NewArrayInit");
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            MethodInfo method = ExpressionEvaluator.CreateNewArrayMethodInfo.MakeGenericMethod(na.Type.GetElementType());
            return (Expression)method.Invoke(null, new object[] { exprs.Select(e => ((ConstantExpression)e).Value) });
        }

        /// <summary>
        /// Creates a new array wrapped in a ConstantExpression.
        /// </summary>
        /// <typeparam name="TElement">Element type of the array.</typeparam>
        /// <param name="elements">Elements to initialize the array with.</param>
        /// <returns>ConstantExpression containing the newly created array.</returns>
        private static Expression CreateNewArray<TElement>(IEnumerable<TElement> elements)
        {
            return Expression.Constant((new List<TElement>(elements)).ToArray(), typeof(TElement[]));
        }
    }
}
