//---------------------------------------------------------------------
// <copyright file="QueryOptionExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using Microsoft.OData.UriParser;

    public static class QueryOptionExtensions
    {
        /// <summary>
        /// Apply filter to the given resouce expression
        /// </summary>
        /// <param name="rootExpression"></param>
        /// <param name="entityInstanceType"></param>
        /// <param name="uriParser"></param>
        /// <param name="filterClause"></param>
        /// <returns></returns>
        public static Expression ApplyFilter(this Expression rootExpression, Type entityInstanceType, ODataUriParser uriParser, FilterClause filterClause)
        {
            ParameterExpression parameter = Expression.Parameter(entityInstanceType, "it");
            NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
            {
                ImplicitVariableParameterExpression = parameter,
                UriParser = uriParser,
            };
            Expression filterNodeExpression = nodeToExpressionTranslator.TranslateNode(filterClause.Expression);

            // IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
            // translate to rootExpression.Where(filterNodeExpression)
            return Expression.Call(
                typeof(Enumerable),
                "Where",
                new Type[] { entityInstanceType },
                rootExpression,
                Expression.Lambda(filterNodeExpression, parameter));
        }

        /// <summary>
        /// Apply orderby to the given resouce expression
        /// </summary>
        /// <param name="rootExpression"></param>
        /// <param name="entityInstanceType"></param>
        /// <param name="uriParser"></param>
        /// <param name="orderByClause"></param>
        /// <returns></returns>
        public static Expression ApplyOrderBy(this Expression rootExpression, Type entityInstanceType, ODataUriParser uriParser, OrderByClause orderByClause)
        {
            ParameterExpression parameter = Expression.Parameter(entityInstanceType, "it");
            NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
            {
                ImplicitVariableParameterExpression = parameter,
                UriParser = uriParser,
            };

            Expression orderByNodeExpression = nodeToExpressionTranslator.TranslateNode(orderByClause.Expression);

            var keyType = EdmClrTypeUtils.GetInstanceType(orderByClause.Expression.TypeReference);

            if (orderByClause.Expression.TypeReference.IsNullable && keyType.IsValueType)
            {
                keyType = typeof(Nullable<>).MakeGenericType(keyType);
            }

            var method = orderByClause.Direction == OrderByDirection.Ascending ? "OrderBy" : "OrderByDescending";
            //OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
            var expression = Expression.Call(
                typeof(Enumerable),
                method,
                new Type[] { entityInstanceType, keyType },
                rootExpression,
                Expression.Lambda(orderByNodeExpression, parameter)) as Expression;
            var thenBy = orderByClause.ThenBy;
            while (null != thenBy)
            {
                expression = expression.ApplyThenBy(entityInstanceType, uriParser, thenBy);
                thenBy = thenBy.ThenBy;
            }
            return expression;
        }

        /// <summary>
        /// Apply thenOrderBy to the given resouce expression
        /// </summary>
        /// <param name="rootExpression"></param>
        /// <param name="entityInstanceType"></param>
        /// <param name="uriParser"></param>
        /// <param name="thenBy"></param>
        /// <returns></returns>
        public static Expression ApplyThenBy(this Expression rootExpression, Type entityInstanceType, ODataUriParser uriParser, OrderByClause thenBy)
        {
            ParameterExpression parameter = Expression.Parameter(entityInstanceType, "it");
            NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
            {
                ImplicitVariableParameterExpression = parameter,
                UriParser = uriParser,
            };
            Expression orderByNodeExpression = nodeToExpressionTranslator.TranslateNode(thenBy.Expression);

            var keyType = EdmClrTypeUtils.GetInstanceType(thenBy.Expression.TypeReference);
            var method = thenBy.Direction == OrderByDirection.Ascending ? "ThenBy" : "ThenByDescending";
            //ThenBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
            return Expression.Call(
                typeof(Enumerable),
                method,
                new Type[] { entityInstanceType, keyType },
                rootExpression,
                Expression.Lambda(orderByNodeExpression, parameter));
        }


        /// <summary>
        /// Apply search to the given resouce expression
        /// </summary>
        /// <param name="rootExpression"></param>
        /// <param name="entityInstanceType"></param>
        /// <param name="uriParser"></param>
        /// <param name="searchClause"></param>
        /// <returns></returns>
        public static Expression ApplySearch(this Expression rootExpression, Type entityInstanceType, ODataUriParser uriParser, SearchClause searchClause)
        {
            ParameterExpression parameter = Expression.Parameter(entityInstanceType, "it");
            NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
            {
                ImplicitVariableParameterExpression = parameter,
                UriParser = uriParser,
            };

            Expression searchNodeExpression = nodeToExpressionTranslator.TranslateNode(searchClause.Expression);

            // IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
            // translate to rootExpression.Where(searchNodeExpression)
            return Expression.Call(
                typeof(Enumerable),
                "Where",
                new Type[] { entityInstanceType },
                rootExpression,
                Expression.Lambda(searchNodeExpression, parameter));
        }

        /// <summary>
        /// Apply top option to the given resouce expression
        /// </summary>
        /// <param name="rootExpression"></param>
        /// <param name="entityInstanceType"></param>
        /// <param name="uriParser"></param>
        /// <param name="topOption"></param>
        /// <returns></returns>
        public static Expression ApplyTop(this Expression rootExpression, Type entityInstanceType, long topOption)
        {
            //Translate topOption to "Take"
            //IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
            return Expression.Call(typeof(Enumerable), "Take", new Type[] { entityInstanceType }, rootExpression, Expression.Constant(topOption.ToInt32()));
        }

        /// <summary>
        /// Apply skip option to the given resouce expression
        /// </summary>
        /// <param name="rootExpression"></param>
        /// <param name="entityInstanceType"></param>
        /// <param name="uriParser"></param>
        /// <param name="skipOption"></param>
        /// <returns></returns>
        public static Expression ApplySkip(this Expression rootExpression, Type entityInstanceType, long skipOption)
        {
            //Translate skipOption to "Skip"
            //IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
            return Expression.Call(typeof(Enumerable), "Skip", new Type[] { entityInstanceType }, rootExpression, Expression.Constant(skipOption.ToInt32()));
        }

        private static int ToInt32(this long value)
        {
            if (value < 0)
            {
                throw Utility.BuildException(HttpStatusCode.BadRequest, "Query option was smaller than 0.", null);
            }

            try
            {
                return (int)value;
            }
            catch (OverflowException e)
            {
                throw Utility.BuildException(HttpStatusCode.NotImplemented, "Query option was too large for an Int32.", e);
            }
        }
    }
}