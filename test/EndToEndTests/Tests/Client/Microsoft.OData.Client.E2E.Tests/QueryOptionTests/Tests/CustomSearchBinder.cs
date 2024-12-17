//---------------------------------------------------------------------
// <copyright file="CustomSearchBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Tests
{
    public class CustomSearchBinder : QueryBinder, ISearchBinder
    {
        internal readonly MethodInfo StringEqualsMethodInfo =
            typeof(string).GetMethod("Equals", new[]
            {
                typeof(string),
                typeof(string),
                typeof(StringComparison)
            })!;

        internal readonly MethodInfo StringContainsMethodInfo =
            typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) })!;


        public Expression BindSearch(SearchClause searchClause, QueryBinderContext context)
        {
            Expression exp = BindSingleValueNode(searchClause.Expression, context);

            LambdaExpression lambdaExp = Expression.Lambda(exp, context.CurrentParameter);

            return lambdaExp;
        }

        public override Expression BindSingleValueNode(SingleValueNode node, QueryBinderContext context)
        {
            switch (node.Kind)
            {
                case QueryNodeKind.BinaryOperator:
                    return BindBinaryOperatorNode(node as BinaryOperatorNode, context);

                case QueryNodeKind.SearchTerm:
                    return BindSearchTerm((SearchTermNode)node, context);

                case QueryNodeKind.UnaryOperator:
                    return BindUnaryOperatorNode(node as UnaryOperatorNode, context);
            }

            return null;
        }

        public Expression BindSearchTerm(SearchTermNode node, QueryBinderContext context)
        {
            // Source is from context;
            Expression source = context.CurrentParameter;
            string text = node.Text.ToLowerInvariant();

            if (context.ElementClrType == typeof(ProductReview))
            {
                // $it.Comment
                Expression commentProperty = Expression.Property(source, "Comment");

                // $it.Comment.ToString()
                Expression commentPropertyString = Expression.Call(commentProperty, "ToString", typeArguments: null, arguments: null);

                // string.Contains($it.Comment.ToString(), text, StringComparison.OrdinalIgnoreCase)
                return Expression.Call(commentPropertyString, StringContainsMethodInfo,
                    Expression.Constant(text, typeof(string)), Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison)));
            }

            if (context.ElementClrType == typeof(ProductDetail))
            {
                // $it.Description
                Expression descriptionProperty = Expression.Property(source, "Description");

                // $it.Description.ToString()
                Expression descriptionPropertyString = Expression.Call(descriptionProperty, "ToString", typeArguments: null, arguments: null);

                // string.Contains($it.Description.ToString(), text, StringComparison.OrdinalIgnoreCase)
                return Expression.Call(descriptionPropertyString, StringContainsMethodInfo,
                    Expression.Constant(text, typeof(string)), Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison)));
            }

            return Expression.Constant(false, typeof(bool));
        }

        private Expression CombineContainsAndEquals(Expression propertyString, string text)
        {
            // string.Contains(propertyString, text, StringComparison.OrdinalIgnoreCase)
            Expression containsExpression = Expression.Call(propertyString, StringContainsMethodInfo,
                Expression.Constant(text, typeof(string)), Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison)));

            // string.Equals(propertyString, text, StringComparison.OrdinalIgnoreCase)
            Expression equalsExpression = Expression.Call(propertyString, StringEqualsMethodInfo,
                Expression.Constant(text, typeof(string)), Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison)));

            // Combine Contains and Equals using OR
            return Expression.OrElse(containsExpression, equalsExpression);
        }
    }
}
