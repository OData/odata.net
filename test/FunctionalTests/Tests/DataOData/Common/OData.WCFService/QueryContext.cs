//---------------------------------------------------------------------
// <copyright file="QueryContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Wrapper class for incoming client request URI/queries.
    /// </summary>
    public class QueryContext
    {
        /// <summary>
        /// Gets or sets the ODL QueryPath for the parsed URI
        /// </summary>
        public ODataPath QueryPath { get; private set; }

        /// <summary>
        /// Parses the specified URI and wraps the results in a QueryContext.
        /// </summary>
        /// <param name="requestUri">The URI to parse.</param>
        /// <param name="model">The data store model.</param>
        /// <returns>The parsed URI wrapped in a QueryContext.</returns>
        public static QueryContext ParseUri(Uri requestUri, IEdmModel model)
        {
            var requestUriParts = requestUri.OriginalString.Split('?');
            var queryPath = requestUriParts.First();
            if (requestUriParts.Count() > 1)
            {
                throw new NotSupportedException("Query option is not supported by the service yet.");
            }

            ODataUriParser uriParser = new ODataUriParser(model, ServiceConstants.ServiceBaseUri, new Uri(queryPath, UriKind.Absolute));
            return new QueryContext { QueryPath = uriParser.ParsePath() };
        }

        /// <summary>
        /// Resolves the parsed URI against the data store.
        /// </summary>
        /// <param name="model">The data store model.</param>
        /// <param name="dataContext">The data access context.</param>
        /// <returns>The results of querying the data store.</returns>
        public object ResolveQuery(IEdmModel model, DataContext dataContext)
        {
            var queryProvider = new EntityFrameworkQueryProvider(dataContext);
            var testExpressionVisitor = new ODataUriToExpressionTranslator(queryProvider, model);

            // build linq expression from ODataPath and execute the expression 
            Expression boundExpression = Expression.Constant(null);
            foreach (var segment in this.QueryPath)
            {
                boundExpression = segment.TranslateWith(testExpressionVisitor);
            }

            boundExpression = Expression.Convert(boundExpression, typeof(object));
            Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(boundExpression);

            Func<object> compiled = lambda.Compile();
            return compiled();
        }

        /// <summary>
        /// Attempts to resolve the entity set central to the parsed URI.
        /// </summary>
        /// <returns>The entity set central to the parsed URI.</returns>
        public IEdmEntitySet ResolveEntitySet()
        {
            // TODO: update this to handle various segments in ODataPath
            NavigationPropertySegment navigationPropertySegment = this.QueryPath.LastSegment as NavigationPropertySegment;
            if (navigationPropertySegment != null)
            {
                return (IEdmEntitySet)navigationPropertySegment.NavigationSource;
            }

            EntitySetSegment entitySetSegment = this.QueryPath.FirstSegment as EntitySetSegment;
            if (entitySetSegment != null)
            {
                return entitySetSegment.EntitySet;
            }

            throw new NotSupportedException("Unsupported ODataPathSegment EntitySet");
        }

        /// <summary>
        /// Attempts to resolve the key values for a entity based query.
        /// </summary>
        /// <returns>The key values for the query.</returns>
        public IDictionary<string, object> ResolveKeyValues()
        {
            throw new NotImplementedException();
        }
    }
}
