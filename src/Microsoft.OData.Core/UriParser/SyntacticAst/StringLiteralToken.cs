//---------------------------------------------------------------------
// <copyright file="StringLiteralToken.cs" company="Microsoft">
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
    using System.Diagnostics;

    /// <summary>
    /// Lexical token representing a string literal value.
    /// </summary>
    [DebuggerDisplay("StringLiteralToken ({text})")]
    internal sealed class StringLiteralToken : QueryToken
    {
        /// <summary>
        /// Raw text value for this token.
        /// </summary>
        private readonly ReadOnlyMemory<char> text;

        /// <summary>
        /// Constructor for the StringLiteralToken
        /// </summary>
        /// <param name="text">The text value for this token</param>
        /// <param name="isSearch">Indicates whether this token is used in a search context</param>
        internal StringLiteralToken(string text, bool isSearch = true)
        {
            this.text = text.AsMemory();
            this.IsSearchLiteralToken = isSearch;
        }

        internal StringLiteralToken(ReadOnlyMemory<char> text)
        {
            this.text = text;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.StringLiteral; }
        }

        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        internal ReadOnlyMemory<char> Text
        {
            get
            {
                return this.text;
            }
        }

        public bool IsSearchLiteralToken { get; }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}