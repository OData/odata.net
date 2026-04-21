//---------------------------------------------------------------------
// <copyright file="SearchTermToken.cs" company="Microsoft">
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
    /// Lexical token representing a search term value.
    /// </summary>
    [DebuggerDisplay("SearchTermToken ({text})")]
    internal sealed class SearchTermToken : QueryToken
    {
        /// <summary>
        /// Raw text value for this token.
        /// </summary>
        private readonly ReadOnlyMemory<char> originalText;
        private string text;

        /// <summary>
        /// Constructor for the SearchTermToken
        /// </summary>
        /// <param name="text">The text value for this token</param>
        internal SearchTermToken(string text)
            : this(text.AsMemory())
        {
            this.text = text;
        }

        /// <summary>
        /// Constructor for the SearchTermToken
        /// </summary>
        /// <param name="text">The text value for this token</param>
        internal SearchTermToken(ReadOnlyMemory<char> text)
        {
            this.originalText = text;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind => QueryTokenKind.SearchTerm;

        /// <summary>
        /// Gets the original text for this search term.
        /// </summary>
        public ReadOnlyMemory<char> OriginalText => this.originalText;

        /// <summary>
        /// Gets an original string text value of the literal.
        /// Avoid creating the string multiple times.
        /// </summary>
        internal string Text
        {
            get
            {
                if (this.text == null)
                {
                    this.text = this.originalText.ToString();
                }

                return this.text;
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
            throw new NotImplementedException();
        }
    }
}