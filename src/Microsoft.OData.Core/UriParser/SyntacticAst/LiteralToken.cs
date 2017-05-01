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
    /// Lexical token representing a literal value.
    /// </summary>
    public sealed class LiteralToken : QueryToken
    {
        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.
        /// We should use this during type promotion when applying metadata.</remarks>
        private readonly string originalText;

        /// <summary>
        /// The value of the literal. This is a parsed primitive value.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// The expected EDM type of literal.
        /// </summary>
        private readonly IEdmTypeReference expectedEdmTypeReference;

        /// <summary>
        /// Create a new LiteralToken given value and originalText
        /// </summary>
        /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
        public LiteralToken(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// Create a new LiteralToken given value and originalText
        /// </summary>
        /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
        /// <param name="originalText">The original text value of the literal.</param>
        /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.</remarks>
        internal LiteralToken(object value, string originalText)
            : this(value)
        {
            this.originalText = originalText;
        }

        /// <summary>
        /// Create a new LiteralToken given value and originalText
        /// </summary>
        /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
        /// <param name="originalText">The original text value of the literal.</param>
        /// <param name="expectedEdmTypeReference">The expected EDM type of literal..</param>
        /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.</remarks>
        internal LiteralToken(object value, string originalText, IEdmTypeReference expectedEdmTypeReference)
            : this(value, originalText)
        {
            this.expectedEdmTypeReference = expectedEdmTypeReference;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Literal; }
        }

        /// <summary>
        /// The value of the literal. This is a parsed primitive value.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.
        /// We should use this during type promotion when applying metadata.</remarks>
        internal string OriginalText
        {
            get
            {
                return this.originalText;
            }
        }

        /// <summary>
        /// The expected EDM type of literal.
        /// </summary>
        internal IEdmTypeReference ExpectedEdmTypeReference
        {
            get
            {
                return this.expectedEdmTypeReference;
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
            return visitor.Visit(this);
        }
    }
}