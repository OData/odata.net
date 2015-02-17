//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaKeyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Expression node representing the key expression in a query.
    /// </summary>
    public class LinqToAstoriaKeyExpression : LinqQueryMethodExpression
    {
        private const string ParameterName = "element";
        
        internal LinqToAstoriaKeyExpression(QueryExpression source, IEnumerable<KeyValuePair<QueryProperty, QueryConstantExpression>> keys, QueryType type)
            : base(source, type)
        {
            this.KeyProperties = new ReadOnlyCollection<KeyValuePair<QueryProperty, QueryConstantExpression>>(keys.ToList());
            this.Lambda = CreateLambda(this.Source, this.KeyProperties);
        }

        /// <summary>
        /// Gets the key properties
        /// </summary>
        public ReadOnlyCollection<KeyValuePair<QueryProperty, QueryConstantExpression>> KeyProperties { get; private set; }

        /// <summary>
        /// Gets the LinqLambda Expression
        /// </summary>
        public LinqLambdaExpression Lambda { get; private set; }
        
        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            string keyString = string.Join(", ", this.KeyProperties.Select(k => string.Format(CultureInfo.InvariantCulture, "{0}={1}", k.Key.Name, k.Value)).ToArray());
            return string.Format(CultureInfo.InvariantCulture, "{0}.Key({1})", this.Source, keyString);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return ((ILinqToAstoriaExpressionVisitor<TResult>)visitor).Visit(this);
        }

        internal static LinqLambdaExpression CreateLambda(QueryExpression source, IEnumerable<KeyValuePair<QueryProperty, QueryConstantExpression>> keys)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckCollectionNotEmpty(keys, "keys");

            QueryType parameterType;
            if (source.ExpressionType.IsUnresolved)
            {
                parameterType = QueryType.Unresolved;
            }
            else
            {
                var queryCollectionType = source.ExpressionType as QueryCollectionType;
                ExceptionUtilities.CheckObjectNotNull(queryCollectionType, "Source expression type was not a collection. Type was: {0}", source.ExpressionType);
                parameterType = queryCollectionType.ElementType;
            }
            
            var parameter = LinqBuilder.Parameter(ParameterName, parameterType);

            // specifically using the overload of .Property which takes a type because it may have already been resolved and we don't want to throw that away
            var predicate = keys.Select(k => parameter.Property(k.Key.Name, k.Key.PropertyType).EqualTo(k.Value)).ToList();

            QueryExpression body = predicate[0];
            if (predicate.Count > 1)
            {
                for (int i = 1; i < predicate.Count; i++)
                {
                    body = CommonQueryBuilder.And(body, predicate[i]);
                }
            }

            return LinqBuilder.Lambda(body, parameter);
        }
    }
}