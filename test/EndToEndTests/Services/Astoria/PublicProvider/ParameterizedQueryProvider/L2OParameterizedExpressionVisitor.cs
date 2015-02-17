//---------------------------------------------------------------------
// <copyright file="L2OParameterizedExpressionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Linq to object parameterized expression visitor
    /// </summary>
    class L2OParameterizedExpressionVisitor : ExpressionVisitor
    {
        public readonly static Dictionary<Type, bool> ParameterizableTypes = new Dictionary<Type, bool> { 
                {typeof(bool), true},
                {typeof(byte),  true},
                {typeof(DateTimeOffset),  true},
                {typeof(decimal),  true},
                {typeof(double),  true},
                {typeof(float),  true},
                {typeof(Guid),  true},
                {typeof(short),  true},
                {typeof(int),  true},
                {typeof(long),  true},
                {typeof(sbyte),  true},
                {typeof(bool?),  true},
                {typeof(byte?),  true},
                {typeof(DateTimeOffset?),  true},
                {typeof(decimal?),  true},
                {typeof(double?),  true},
                {typeof(float?),  true},
                {typeof(Guid?),  true},
                {typeof(short?),  true},
                {typeof(int?),  true},
                {typeof(long?),  true},
                {typeof(sbyte?),  true},
                {typeof(string),  true}
            };

        /// <summary>
        /// The parameter expression list
        /// </summary>
        readonly List<ParameterExpression> parameters = new List<ParameterExpression>();

        /// <summary>
        /// The parameter values
        /// </summary>
        readonly List<object> values = new List<object>();

        /// <summary>
        /// Parameterize the expression
        /// </summary>
        /// <param name="expression">The original expression</param>
        /// <param name="parameterExpressions">The parameterExpressions which are converted from ConstantExpression</param>
        /// <param name="paraValues">The parameter values</param>
        /// <returns>The parameterized expression</returns>
        public Expression Parameterize(Expression expression, out ParameterExpression[] parameterExpressions, out object[] paraValues)
        {
            expression = Visit(expression);
            paraValues = values.ToArray();
            parameterExpressions = parameters.ToArray();
            return expression;
        }

        /// <summary>
        /// Visit the constantExpression
        /// </summary>
        /// <param name="node">The original expression</param>
        /// <returns>The ParameterExpression</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (ParameterizableTypes.ContainsKey(node.Type))
            {
                // Replace the constant expression to parameter expression and save the parameter value.
                var para = Expression.Parameter(node.Type, "para" + parameters.Count);
                parameters.Add(para);
                values.Add(node.Value);
                return para;
            }
            return base.VisitConstant(node);
        }
    }
}
