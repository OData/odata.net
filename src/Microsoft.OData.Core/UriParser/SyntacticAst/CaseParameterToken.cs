//---------------------------------------------------------------------
// <copyright file="LambdaToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Lexical token representing the a parameter in case function
    /// </summary>
    public class CaseParameterToken : QueryToken
    {
        /// <summary>
        /// Create a CaseParameterToken given the condition, value
        /// </summary>
        /// <param name="condition">The associated expression.</param>
        /// <param name="value">The parameter denoting source type.</param>
        public CaseParameterToken(QueryToken condition, QueryToken value)
        {
            ExceptionUtils.CheckArgumentNotNull(condition, "operand");
            ExceptionUtils.CheckArgumentNotNull(value, "operand");

            Condition = condition;
            Value = value;
        }

        /// <summary>
        /// The condition token.
        /// </summary>
        public QueryToken Condition { get; }

        /// <summary>
        /// The value token.
        /// </summary>
        public QueryToken Value { get; }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind => QueryTokenKind.CaseParameter;

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}