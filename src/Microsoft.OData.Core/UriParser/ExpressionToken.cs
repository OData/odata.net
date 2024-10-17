//---------------------------------------------------------------------
// <copyright file="ExpressionToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>Use this class to represent a lexical expression token.</summary>
    [DebuggerDisplay("{InternalKind} @ {Position}: [{Text}]")]
    internal struct ExpressionToken
    {
        /// <summary>Token representing gt keyword</summary>
        internal static readonly ExpressionToken GreaterThan = new ExpressionToken { Text = ExpressionConstants.KeywordGreaterThan.AsMemory(), Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing eq keyword</summary>
        internal static readonly ExpressionToken EqualsTo = new ExpressionToken { Text = ExpressionConstants.KeywordEqual.AsMemory(), Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing lt keyword</summary>
        internal static readonly ExpressionToken LessThan = new ExpressionToken { Text = ExpressionConstants.KeywordLessThan.AsMemory(), Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>InternalKind of token.</summary>
        internal ExpressionTokenKind Kind;

        /// <summary>Token text.</summary>
        internal ReadOnlyMemory<char> Text;

        /// <summary>Position of token.</summary>
        internal int Position;

        /// <summary>
        /// Gets the token text as <see cref="ReadOnlySpan{T}"/>
        /// Be noted, avoid calling 'ToString()' multiple times since it will create a new string everytime.
        /// </summary>
        internal ReadOnlySpan<char> Span => Text.Span;

        /// <summary>
        /// Gets the token length.
        /// </summary>
        internal int Length => Text.Length;

        /// <summary>The edm type of the expression token literal.</summary>
        private IEdmTypeReference LiteralEdmType;

        /// <summary>Checks whether this token is a valid token for a key value.</summary>
        internal bool IsKeyValueToken
        {
            get
            {
                return
                    this.Kind == ExpressionTokenKind.BinaryLiteral ||
                    this.Kind == ExpressionTokenKind.BooleanLiteral ||
                    this.Kind == ExpressionTokenKind.DateLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeOffsetLiteral ||
                    this.Kind == ExpressionTokenKind.DurationLiteral ||
                    this.Kind == ExpressionTokenKind.GuidLiteral ||
                    this.Kind == ExpressionTokenKind.StringLiteral ||
                    this.Kind == ExpressionTokenKind.GeographyLiteral ||
                    this.Kind == ExpressionTokenKind.GeometryLiteral ||
                    this.Kind == ExpressionTokenKind.QuotedLiteral ||
                    this.Kind == ExpressionTokenKind.TimeOfDayLiteral ||
                    ExpressionLexerUtils.IsNumeric(this.Kind);
            }
        }

        /// <summary>Checks whether this token is a valid token for a function parameter.</summary>
        internal bool IsFunctionParameterToken
        {
            get
            {
                return this.IsKeyValueToken
                    || this.Kind == ExpressionTokenKind.BracketedExpression
                    || this.Kind == ExpressionTokenKind.BracedExpression
                    || this.Kind == ExpressionTokenKind.NullLiteral;
            }
        }

        /// <summary>Provides a string representation of this token.</summary>
        /// <returns>String representation of this token.</returns>
        public override string ToString()
        {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, $"{this.Kind} @ {this.Position}: [{this.Text}]", this.Kind, this.Position);
        }

        /// <summary>Gets the current identifier text.</summary>
        /// <returns>The current identifier text.</returns>
        internal ReadOnlySpan<char> GetIdentifier()
        {
            if (this.Kind != ExpressionTokenKind.Identifier)
            {
                string message = Strings.ExpressionToken_IdentifierExpected(this.Position);
                throw new ODataException(message);
            }

            Debug.Assert(!this.Text.IsEmpty, "Text is null");
            return this.Span;
        }

        /// <summary>Checks that this token has the specified identifier.</summary>
        /// <param name="id">Identifier to check.</param>
        /// <param name="enableCaseInsensitive">whether to allow case insensitive.</param>
        /// <returns>true if this is an identifier with the specified text.</returns>
        internal bool IdentifierIs(string id, bool enableCaseInsensitive)
        {
            return this.Kind == ExpressionTokenKind.Identifier
                && this.Span.Equals(id, enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        internal void SetCustomEdmTypeLiteral(IEdmTypeReference edmType)
        {
            this.Kind = ExpressionTokenKind.CustomTypeLiteral;
            this.LiteralEdmType = edmType;
        }

        internal IEdmTypeReference GetLiteralEdmTypeReference()
        {
            Debug.Assert(this.Kind != ExpressionTokenKind.CustomTypeLiteral || this.LiteralEdmType != null, "ExpressionTokenKind is marked as CustomTypeLiteral but not EdmType was set");

            if (this.LiteralEdmType == null && this.Kind != ExpressionTokenKind.CustomTypeLiteral)
            {
                this.LiteralEdmType = UriParserHelper.GetLiteralEdmTypeReference(this.Kind);
            }

            return this.LiteralEdmType;
        }
    }
}