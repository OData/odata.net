//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Parsing
{
    #region Namespaces
    using System;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Spatial;
    using Microsoft.Data.OData.Query.SemanticAst;
    using DataServiceProviderMethods = System.Data.Services.Providers.DataServiceProviderMethods;
    using OpenTypeMethods = System.Data.Services.Providers.OpenTypeMethods;
    using Strings = System.Data.Services.Strings;
    #endregion Namespaces

    /// <summary>
    /// This class provides static methods to parse query options and compose 
    /// them on an existing query.
    /// </summary>
    internal static class RequestQueryParser
    {
        #region Internal methods.

        /// <summary>Sorts a query like a SQL ORDER BY clause does.</summary>
        /// <param name="source">Original source for query.</param>
        /// <param name="orderingInfo">Ordering definition to compose.</param>
        /// <returns>The composed query.</returns>
        internal static Expression OrderBy(Expression source, OrderingInfo orderingInfo)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(orderingInfo != null, "orderingInfo != null");

            Expression queryExpr = source;
            bool useOrderBy = true;
            foreach (OrderingExpression o in orderingInfo.OrderingExpressions)
            {
                LambdaExpression selectorLambda = (LambdaExpression)o.Expression;

                Type selectorType = selectorLambda.Body.Type;
                Debug.Assert(selectorType != null, "type != null");
                
                // ensure either the expression type is orderable (ie, primitive) or its an open expression.
                if (!WebUtil.IsPrimitiveType(selectorType) && !OpenTypeMethods.IsOpenExpression(selectorLambda.Body))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryParser_OrderByDoesNotSupportType(WebUtil.GetTypeName(selectorType)));
                }

                if (useOrderBy)
                {
                    queryExpr = o.IsAscending ? queryExpr.QueryableOrderBy(selectorLambda) : queryExpr.QueryableOrderByDescending(selectorLambda);
                }
                else
                {
                    queryExpr = o.IsAscending ? queryExpr.QueryableThenBy(selectorLambda) : queryExpr.QueryableThenByDescending(selectorLambda);
                }

                useOrderBy = false;
            }

            return queryExpr;
        }

        /// <summary>Filters a query like a SQL WHERE clause does.</summary>
        /// <param name="service">Service with data and configuration.</param>
        /// <param name="requestDescription">RequestDescription instance containing information about the current request being parsed.</param>
        /// <param name="source">Original source for query expression.</param>
        /// <param name="predicate">Predicate to compose.</param>
        /// <returns>The composed query expression.</returns>
        internal static Expression Where(IDataService service, RequestDescription requestDescription, Expression source, string predicate)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(predicate != null, "predicate != null");
            Debug.Assert(requestDescription != null, "requestDescription != null");

            FilterClause filterClause = new RequestExpressionParser(service, requestDescription, predicate).ParseFilter();

            Type queryElementType = source.ElementType();
            Debug.Assert(queryElementType != null, "queryElementType != null");

            ParameterExpression parameterForIt = Expression.Parameter(queryElementType, "it");
            Debug.Assert(
                (requestDescription.TargetResourceSet == null && (requestDescription.TargetResourceType == null || requestDescription.TargetResourceType.ResourceTypeKind != ResourceTypeKind.EntityType)) ||
                (requestDescription.TargetResourceType != null && requestDescription.TargetResourceType.ResourceTypeKind == ResourceTypeKind.EntityType),
                "setForIt cannot be null if typeForIt is an entity type.");
            Debug.Assert(
                requestDescription.TargetResourceType == null && parameterForIt.Type == typeof(object) ||
                requestDescription.TargetResourceType != null && requestDescription.TargetResourceType.InstanceType == parameterForIt.Type,
                "non-open type expressions should have a typeForIt");

            var translator = NodeToExpressionTranslator.Create(service, requestDescription, parameterForIt);
            LambdaExpression lambda = translator.TranslateFilterClause(filterClause);
            return source.QueryableWhere(lambda);
        }

        #endregion Internal methods.
    }
}
