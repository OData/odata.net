//---------------------------------------------------------------------
// <copyright file="LiteralToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// JSON value token, Lexical token representing a literal value.
    /// </summary>
    public sealed class LiteralToken : QueryToken
    {
        /// <summary>
        /// Create a new LiteralToken with the given value.
        /// </summary>
        /// <param name="value">The value of the literal. This is a pre-parsed primitive value.</param>
        public LiteralToken(object value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Create a new LiteralToken with given value and originalText, without the inferred type.
        /// </summary>
        /// <param name="value">The value of the literal. This is a pre-parsed primitive value.</param>
        /// <param name="originalText">The original text value of the literal.</param>
        internal LiteralToken(object value, string originalText)
            : this(value)
        {
            this.OriginalText = originalText ?? string.Empty;
        }

        /// <summary>
        /// Create a new LiteralToken with given value and originalText, with the inferred type.
        /// </summary>
        /// <param name="value">The value of the literal. This is a pre-parsed primitive value.</param>
        /// <param name="originalText">The original text value of the literal.</param>
        /// <param name="typeReference">The inferred EDM type of literal.</param>
        internal LiteralToken(object value, string originalText, IEdmTypeReference typeReference)
            : this(value, originalText)
        {
            this.InferredType = typeReference;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind => QueryTokenKind.Literal;

        /// <summary>
        /// Gets the value of the literal. This is a pre-parsed primitive value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the original text value of the literal.
        /// </summary>
        internal string OriginalText { get; } = string.Empty;

        /// <summary>
        /// Gets or sets the expected Edm type of literal. This type is from the metadata, and is used for type promotion.
        /// </summary>
        internal IEdmTypeReference ExpectedType { get; set; }

        /// <summary>
        /// Gets the Edm type inferred from the literal itself. The nullability seems un-used but let's keep it.(One exception is when it's from the Customized Prefix type.)
        /// This could be null if no inferred type provided or for a 'null' literal since there's no type information from the literal itself.
        /// </summary>
        internal IEdmTypeReference InferredType { get; set; }

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