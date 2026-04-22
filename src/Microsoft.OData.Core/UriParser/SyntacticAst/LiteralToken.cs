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
    using System;

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
        private readonly ReadOnlyMemory<char> originalText;

        /// <summary>
        /// The value of the literal. This is a parsed primitive value.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// The expected EDM type of literal.
        /// </summary>
        private IEdmTypeReference expectedEdmTypeReference;

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
        /// <param name="value">The value of the literal. This is a pre-parsed primitive value.</param>
        /// <param name="originalText">The original text value of the literal.</param>
        /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.</remarks>
        internal LiteralToken(object value, ReadOnlyMemory<char> originalText)
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
            : this(value, originalText.AsMemory())
        {
            this.expectedEdmTypeReference = expectedEdmTypeReference;
        }

        //internal LiteralToken(ReadOnlyMemory<char> originalText, LiteralKind literalKind)
        //{
        //    this.originalText = originalText;
        //    this.LiteralKind = literalKind;
        //}

        //public LiteralToken(object value, ReadOnlyMemory<char> originalText)
        //    : this (value)
        //{
        //    this.originalText = originalText.ToString();
        //    this.LiteralKind = literalKind;
        //}

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
        internal ReadOnlyMemory<char> OriginalText
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
            set
            {
                this.expectedEdmTypeReference = value;
            }
        }

        internal LiteralKind LiteralKind { get; set; }

        internal IEdmTypeReference InferredEdmTypeReference { get; set; } // The EdmType reference of the literal token

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

    internal enum LiteralKind
    {
        None,
        Null,
        Boolean,
        DoubleQuotedString,
        SingleQuotedString,
        Date,
        TimeOfDay,
        Integer,
        Decimal,
        Int64,
        Double,
        Single,
        Binary,
        Geography,
        Geometry,
        DateTimeOffset,
        Guid,
        Duration,
        Quoted,
        CustomTypeLiteral
    }
}