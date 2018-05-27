//---------------------------------------------------------------------
// <copyright file="ComputeToken.cs" company="Microsoft">
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
#if !ODATA_CLIENT
    using Aggregation;
#endif

    /// <summary>
    /// Query token representing an Compute token.
    /// </summary>
    public sealed class ComputeToken : ApplyTransformationToken
    {
        private readonly IEnumerable<ComputeExpressionToken> expressions;

        /// <summary>
        /// Create an ComputeToken.
        /// </summary>
        /// <param name="expressions">The list of ComputeExpressionToken.</param>
        public ComputeToken(IEnumerable<ComputeExpressionToken> expressions)
        {
            ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");
            this.expressions = expressions;
        }

        /// <summary>
        /// Gets the kind of this token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Compute; }
        }

        /// <summary>
        /// Gets the list of ComputeExpressionToken.
        /// </summary>
        public IEnumerable<ComputeExpressionToken> Expressions
        {
            get
            {
                return expressions;
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
