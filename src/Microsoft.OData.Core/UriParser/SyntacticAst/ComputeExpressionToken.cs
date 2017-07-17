//---------------------------------------------------------------------
// <copyright file="ComputeExpressionToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Query token representing an Aggregate token.
    /// </summary>
    public sealed class ComputeExpressionToken : QueryToken
    {
        private QueryToken expression;

        private string alias;

        /// <summary>
        /// Create an ComputeExpressionToken.
        /// </summary>
        /// <param name="expression">The computation token.</param>
        /// <param name="alias">The alias for the computation.</param>
        public ComputeExpressionToken(QueryToken expression, string alias)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(alias, "alias");

            this.expression = expression;
            this.alias = alias;
        }

        /// <summary>
        /// Gets the kind of this token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get
            {
                return QueryTokenKind.ComputeExpression;
            }
        }

        /// <summary>
        /// Gets the QueryToken.
        /// </summary>
        public QueryToken Expression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>
        /// Gets the alias of the computation.
        /// </summary>
        public string Alias
        {
            get
            {
                return this.alias;
            }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            SyntacticTreeVisitor<T> implementation = visitor as SyntacticTreeVisitor<T>;
            if (implementation != null)
            {
                return implementation.Visit(this);
            }

            throw new NotImplementedException();
        }
    }
}
