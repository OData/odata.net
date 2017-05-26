//---------------------------------------------------------------------
// <copyright file="DottedIdentifierToken.cs" company="Microsoft">
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
    /// Lexical token representing a type segment.
    /// </summary>
    public sealed class DottedIdentifierToken : PathToken
    {
        /// <summary>
        /// The Identifier of the type segment.
        /// </summary>
        private readonly string identifier;

        /// <summary>
        /// The parent segment.
        /// </summary>
        private QueryToken nextToken;

        /// <summary>
        /// Create a TypeSegmentQueryToken given the Identifier and the parent (if any)
        /// </summary>
        /// <param name="identifier">The Identifier of the type segment, including the namespace.</param>
        /// <param name="nextToken">The parent segment.</param>
        public DottedIdentifierToken(string identifier, QueryToken nextToken)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(identifier, "Identifier");

            this.identifier = identifier;
            this.nextToken = nextToken;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.DottedIdentifier; }
        }

        /// <summary>
        /// The full name of the type.
        /// </summary>
        public override string Identifier
        {
            get { return this.identifier; }
        }

        /// <summary>
        /// The parent.
        /// </summary>
        public override QueryToken NextToken
        {
            get { return this.nextToken; }
            set { this.nextToken = value; }
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