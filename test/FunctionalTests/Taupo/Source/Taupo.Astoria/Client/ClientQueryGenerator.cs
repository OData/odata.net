//---------------------------------------------------------------------
// <copyright file="ClientQueryGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Generaters the expression tree of Linq.Expression nodes that represents Linq to Astoria expression for the given <see cref="QueryExpression"/> tree.
    /// </summary>
    public class ClientQueryGenerator
    {
        private IAnonymousTypeBuilder anonymousTypeBuilder;

        /// <summary>
        /// Initializes a new instance of the ClientQueryGenerator class.
        /// </summary>
        /// <param name="anonymousTypeBuilder">The anonymous type builder.</param>
        public ClientQueryGenerator(IAnonymousTypeBuilder anonymousTypeBuilder)
        {
            this.anonymousTypeBuilder = anonymousTypeBuilder;
        }

        /// <summary>
        /// Generates the tree of Linq.Expression nodes that represents query.
        /// </summary>
        /// <param name="expression">The root node of the expression tree that the resulting tree will be built from.</param>
        /// <param name="context">Context for the expression.</param>
        /// <param name="closures">Closures that are in scope. Properties of the type should match free variable names.</param>
        /// <returns>The tree that represents a query.</returns>
        public Expression Generate(QueryExpression expression, DataServiceContext context, params object[] closures)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            ExceptionUtilities.CheckArgumentNotNull(closures, "closures");

            var visitor = new LinqToAstoriaQueryGenerationVisitor(context, closures, this.anonymousTypeBuilder);
            var result = visitor.GenerateLinqExpression(expression);

            return result;
        }

        /// <summary>
        /// The visitor that creates Linq.Expression tree that represents query.
        /// </summary>
        private class LinqToAstoriaQueryGenerationVisitor : LinqQueryGenerationVisitor, ILinqToAstoriaExpressionVisitor<Expression>
        {
            /// <summary>
            /// Initializes a new instance of the LinqToAstoriaQueryGenerationVisitor class.
            /// </summary>
            /// <param name="context">The object context.</param>
            /// <param name="closures">Lst of closures for the expression.</param>
            /// <param name="anonymousTypeBuilder">The anonymous type builder.</param>
            public LinqToAstoriaQueryGenerationVisitor(DataServiceContext context, IEnumerable<object> closures, IAnonymousTypeBuilder anonymousTypeBuilder)
                : base(context, closures, anonymousTypeBuilder)
            {
            }

            /// <summary>
            /// Visits a LinqAddQueryOptionExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public Expression Visit(LinqToAstoriaAddQueryOptionExpression expression)
            {
                Expression source = this.GenerateLinqExpression(expression.Source);
                var queryOptionParameters = new Expression[] { Expression.Constant(expression.QueryOption), Expression.Constant(expression.QueryValue) };

                var elementType = source.Type.GetGenericArguments()[0];
                ExceptionUtilities.Assert(elementType != null, "Element type should not be null");
                var typedDataServiceQuery = typeof(DataServiceQuery<>).MakeGenericType(elementType);

                var result = Expression.Call(Expression.Convert(source, typedDataServiceQuery), "AddQueryOption", new Type[] { }, queryOptionParameters);

                return result;
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqToAstoriaConditionalExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>The result of visiting this expression.</returns>
            public Expression Visit(LinqToAstoriaConditionalExpression expression)
            {
                var condition = this.GenerateLinqExpression(expression.Condition);
                var ifTrue = this.GenerateLinqExpression(expression.IfTrue);
                var ifFalse = this.GenerateLinqExpression(expression.IfFalse);
                
                return Expression.Condition(condition, ifTrue, ifFalse);
            }

            /// <summary>
            /// Visits a LinqExpandExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public Expression Visit(LinqToAstoriaExpandExpression expression)
            {
                Expression source = this.GenerateLinqExpression(expression.Source);
                if (!expression.IsImplicit)
                {
                    var queryOptionParameters = new Expression[] { Expression.Constant(expression.ExpandString) };

                    var elementType = source.Type.GetGenericArguments()[0];
                    ExceptionUtilities.Assert(elementType != null, "Element type should not be null");
                    var typedDataServiceQuery = typeof(DataServiceQuery<>).MakeGenericType(elementType);

                    var result = Expression.Call(Expression.Convert(source, typedDataServiceQuery), "Expand", new Type[] { }, queryOptionParameters);
                    return result;
                }

                return source;
            }

            /// <summary>
            /// Visits a LinqToAstoriaExpandLambdaExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public Expression Visit(LinqToAstoriaExpandLambdaExpression expression)
            {
                Expression source = this.GenerateLinqExpression(expression.Source);
                Expression predicate = this.GenerateLinqExpression(expression.Lambda);

                Type sourceType = predicate.Type.GetGenericArguments()[0];
                Type targetType = predicate.Type.GetGenericArguments()[1];

                Type closedDataServiceQueryType = typeof(DataServiceQuery<>).MakeGenericType(new Type[] { sourceType });
                Expression sourceAsDataServiceQuery = Expression.Convert(source, closedDataServiceQueryType);

                // public DataServiceQuery<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
                MethodInfo genericExpand = closedDataServiceQueryType.GetMethods().Single(m => m.IsPublic && m.Name == "Expand" && m.IsGenericMethod);
                MethodInfo closedGenericExpand = genericExpand.MakeGenericMethod(targetType);
                var result = Expression.Call(sourceAsDataServiceQuery, closedGenericExpand, predicate);
                return result;
            }

            /// <summary>
            /// Visits a LinqKeyExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public Expression Visit(LinqToAstoriaKeyExpression expression)
            {
                Expression source = this.GenerateLinqExpression(expression.Source);
                Expression predicate = this.GenerateLinqExpression(expression.Lambda);

                Type sourceTypeArgument = this.GetSourceCollectionTypeArgument(source);

                var result = Expression.Call(typeof(Queryable), "Where", new Type[] { sourceTypeArgument }, source, predicate);
                if (!(expression.ExpressionType is QueryCollectionType))
                {
                    result = Expression.Call(typeof(Queryable), "SingleOrDefault", new Type[] { sourceTypeArgument }, result);
                }

                return result;
            }

            /// <summary>
            /// Visits a LinqToAstoriaLinksExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public Expression Visit(LinqToAstoriaLinksExpression expression)
            {
                throw new TaupoNotSupportedException("Not supported");
            }

            /// <summary>
            /// Visits a LinqToAstoriaValueExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public Expression Visit(LinqToAstoriaValueExpression expression)
            {
                throw new TaupoNotSupportedException("Not supported");
            }

            /// <summary>
            /// Builds a method call expression.
            /// </summary>
            /// <param name="instance">The instance expression.</param>
            /// <param name="methodName">Name of the method.</param>
            /// <param name="arguments">The argument expressions.</param>
            /// <returns>
            /// The method call expression
            /// </returns>
            protected override Expression BuildMethodCallExpression(Expression instance, string methodName, params Expression[] arguments)
            {
                MethodInfo methodInfo = instance.Type.GetMethod(methodName);

                if (methodInfo == null)
                {
                    methodInfo = instance.Type.GetExtensionMethod(methodName, arguments.Select(a => a.Type).ToArray());
                    arguments = new[] { instance }.Concat(arguments).ToArray();
                }

                ExceptionUtilities.CheckObjectNotNull(methodInfo, "Could not find method '{0}' on type of expression '{1}'", methodName, instance);

                return LinqQueryGenerationVisitor.BuildMethodCallExpression(instance, methodInfo, arguments);
            }

            protected override Expression VisitBinaryExpression(QueryBinaryExpression expression, Func<Expression, Expression, BinaryExpression> builderMethodCall)
            {
                Expression left = this.GenerateLinqExpression(expression.Left);
                Expression right = this.GenerateLinqExpression(expression.Right);

                if (Nullable.GetUnderlyingType(left.Type) != null)
                {
                    left = Expression.Property(left, "Value");
                }

                if (Nullable.GetUnderlyingType(right.Type) != null)
                {
                    right = Expression.Property(right, "Value");
                }

                var leftTypeCode = Type.GetTypeCode(left.Type);
                var rightTypeCode = Type.GetTypeCode(right.Type);
                if (leftTypeCode > rightTypeCode)
                {
                    right = Expression.Convert(right, left.Type);
                }
                else if (rightTypeCode > leftTypeCode)
                {
                    left = Expression.Convert(left, right.Type);
                }

                return builderMethodCall(left, right);
            }

            private Type GetSourceCollectionTypeArgument(Expression source)
            {
                Type enumerableInterface = source.Type.GetInterfaces().Where(i => i.IsGenericType() && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Single();

                return enumerableInterface.GetGenericArguments()[0];
            }
        }
    }
}
