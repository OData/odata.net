//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaLinqBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Factory methods used to construct QueryExpression tree nodes.
    /// </summary>
    public static class LinqToAstoriaLinqBuilder
    {
        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaAddQueryOptionExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="queryOption">The query option.</param>
        /// <param name="queryValue">The query value.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression AddQueryOption(this QueryExpression source, string queryOption, object queryValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            // TODO: fix the type for queryoption="$select"
            return new LinqToAstoriaAddQueryOptionExpression(source, queryOption, queryValue, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to converts the result type of a <see cref="LinqToAstoriaKeyExpression"/> to a collection type.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="QueryExpression"/> that is of a collection type.</returns>
        /// <remarks>This method is no longer needed, and should not be used when creating Linq to Astoria queries.</remarks>
        public static QueryExpression AsSingleton(this LinqToAstoriaKeyExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            // because query type resolution is now delayed, we don't actually need to do anything here
            return source;
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaExpandExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="expandValue">The expand string.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression Expand(this QueryExpression source, string expandValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqToAstoriaExpandExpression(source, expandValue, QueryType.Unresolved, false);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaExpandLambdaExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="expandValues">The expand lambda expression.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression Expand(this QueryExpression source, LinqLambdaExpression expandValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqToAstoriaExpandLambdaExpression(source, expandValues, QueryType.Unresolved);
        }

        /// <summary>
        /// Method to indicate that the Expand is implicit. When this is used $expand will not be added
        /// to any builder that builds code for the Client as the client automatically puts expands
        /// in for selects
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="expandValue">The expand string.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression ImplicitExpand(this QueryExpression source, string expandValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqToAstoriaExpandExpression(source, expandValue, QueryType.Unresolved, true);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaKeyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keys">The list of key properties.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static LinqToAstoriaKeyExpression Key(this QueryExpression source, IEnumerable<KeyValuePair<QueryProperty, QueryConstantExpression>> keys)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckCollectionNotEmpty(keys, "keys");

            return new LinqToAstoriaKeyExpression(source, keys, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaKeyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="keys">The set of key property values.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static LinqToAstoriaKeyExpression Key(this QueryExpression source, IEnumerable<NamedValue> keys)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckCollectionNotEmpty(keys, "keys");

            QueryType elementType = QueryType.Unresolved;
            var queryCollectionType = source.ExpressionType as QueryCollectionType;
            if (queryCollectionType != null)
            {
                elementType = queryCollectionType.ElementType;
            }

            var structuralType = elementType as QueryStructuralType;

            var key = new List<KeyValuePair<QueryProperty, QueryConstantExpression>>();
            foreach (var keyValue in keys)
            {
                QueryScalarType primitiveType = QueryType.UnresolvedPrimitive;
                QueryProperty queryProperty = QueryProperty.Create(keyValue.Name, primitiveType);

                if (structuralType != null)
                {
                    queryProperty = structuralType.Properties.SingleOrDefault(p => p.Name == keyValue.Name);
                    ExceptionUtilities.CheckObjectNotNull(queryProperty, "Could not find property with name '{0}' on type '{1}'", keyValue.Name, structuralType);

                    primitiveType = queryProperty.PropertyType as QueryScalarType;
                    ExceptionUtilities.CheckObjectNotNull(primitiveType, "Property '{0}' on type '{1}' was not a primitive type", keyValue.Name, structuralType);
                }

                var value = CommonQueryBuilder.Constant(keyValue.Value, primitiveType);
                key.Add(new KeyValuePair<QueryProperty, QueryConstantExpression>(queryProperty, value));
            }

            return new LinqToAstoriaKeyExpression(source, key, source.ExpressionType);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaKeyExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="instance">The instance to get key values from.</param>
        /// <returns>The <see cref="QueryExpression"/> with a key matching the values of the given instance.</returns>
        public static LinqToAstoriaKeyExpression Key(this QueryExpression source, QueryStructuralValue instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            var entityType = instance.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Given structural instance was not an entity. Type was: {0}", instance.Type);

            var key = new List<KeyValuePair<QueryProperty, QueryConstantExpression>>();
            foreach (var keyProperty in entityType.EntityType.AllKeyProperties)
            {
                var queryProperty = entityType.Properties.SingleOrDefault(p => p.Name == keyProperty.Name);
                ExceptionUtilities.CheckObjectNotNull(queryProperty, "Could not find property with name '{0}' on type '{1}'", keyProperty.Name, entityType);

                var value = instance.GetScalarValue(keyProperty.Name);

                key.Add(new KeyValuePair<QueryProperty, QueryConstantExpression>(queryProperty, CommonQueryBuilder.Constant(value)));
            }

            return new LinqToAstoriaKeyExpression(source, key, entityType.CreateCollectionType());
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaLinksExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression Links(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqToAstoriaLinksExpression(source, source.ExpressionType);
        }

        /// <summary>
        /// Returns a LinqToAstoriaExpandExpression
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>LinqToAstoriaExpandExpression representation for the expression.</returns>
        /// <remarks>This expression is only used on the client, so it is safe to use LinqToAstoriaExpandExpression equivalent for protocol.</remarks>
        public static LinqToAstoriaExpandExpression ToLinqToAstoriaExpandExpression(this LinqToAstoriaExpandLambdaExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            string expandString = source.Lambda.Body.ToExpandString();
            return new LinqToAstoriaExpandExpression(source.Source, expandString, source.ExpressionType, false);
        }

        /// <summary>
        /// Factory method to create the <see cref="LinqToAstoriaValueExpression"/>.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <returns>The <see cref="QueryExpression"/> with the provided arguments.</returns>
        public static QueryExpression Value(this QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            return new LinqToAstoriaValueExpression(source, source.ExpressionType);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryConstantExpression"/>.
        /// </summary>
        /// <param name="testCondition">The condition to evaluate for the expression.</param>
        /// <param name="ifTrue">Value of the expression if the condition evaluates to true</param>
        /// <param name="ifFalse">Value of the expression if the condition evaluates to false</param>
        /// <param name="type">The type of the expression's result</param>
        /// <returns>The <see cref="LinqToAstoriaConditionalExpression"/> with the provided arguments.</returns>
        public static QueryExpression Condition(QueryExpression testCondition, QueryExpression ifTrue, QueryExpression ifFalse, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(testCondition, "testCondition");
            ExceptionUtilities.CheckArgumentNotNull(ifTrue, "ifTrue");
            ExceptionUtilities.CheckArgumentNotNull(ifFalse, "ifFalse");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new LinqToAstoriaConditionalExpression(testCondition, ifTrue, ifFalse, type);
        }

        /// <summary>
        /// Returns the expand string that represents the expression.
        /// </summary>
        /// <param name="lambdaExpression">The lambda expression to convert to an expand string.</param>
        /// <returns>String representation of the lambda expression.</returns>
        private static string ToExpandString(this QueryExpression lambdaExpression)
        {
            ExceptionUtilities.CheckArgumentNotNull(lambdaExpression, "lambdaExpression");

            QueryPropertyExpression queryPropertyExpression = lambdaExpression as QueryPropertyExpression;
            ExceptionUtilities.CheckArgumentNotNull(queryPropertyExpression, "queryExpression as QueryPropertyExpression");

            QueryAsExpression queryAsExpression = queryPropertyExpression.Instance as QueryAsExpression;
            if (queryAsExpression != null)
            {
                QueryEntityType queryEntityType = queryAsExpression.TypeToOperateAgainst as QueryEntityType;
                ExceptionUtilities.CheckObjectNotNull(queryEntityType, "The type to operate against for the query As expression is not a QueryEntityType");

                return queryEntityType.EntityType.FullName + "/" + queryPropertyExpression.Name;
            }
            else
            {
                return queryPropertyExpression.Name;
            }
        }
    }
}
