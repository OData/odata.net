//---------------------------------------------------------------------
// <copyright file="StarToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing an all-properties access.
    /// </summary>
    public sealed class StarToken : PathToken
    {
        /// <summary>
        /// The NextToken token to access the property on.
        /// If this is null, then the property access has no NextToken. That usually means to access the property
        /// on the implicit parameter for the expression, the result on which the expression is being applied.
        /// </summary>
        private QueryToken nextToken;

        /// <summary>
        /// Create a new StarToken given the NextToken (if any).
        /// </summary>
        /// <param name="nextToken">The NextToken token to access the property on. Pass no if this property has no NextToken.</param>
        public StarToken(QueryToken nextToken)
        {
            this.nextToken = nextToken;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Star; }
        }

        /// <summary>
        /// The NextToken token to access the property on.
        /// If this is null, then the property access has no NextToken. That usually means to access the property
        /// on the implicit parameter for the expression, the result on which the expression is being applied.
        /// </summary>
        public override QueryToken NextToken
        {
            get { return this.nextToken; }
            set { this.nextToken = value; }
        }

        /// <summary>
        /// the name of this token(inherited from PathToken), which in this case is always "*"
        /// </summary>
        public override string Identifier
        {
            get { return "*"; }
        }

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