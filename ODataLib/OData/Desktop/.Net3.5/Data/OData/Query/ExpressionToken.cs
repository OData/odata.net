//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>Use this class to represent a lexical expression token.</summary>
    [DebuggerDisplay("{InternalKind} @ {Position}: [{Text}]")]
    internal struct ExpressionToken
    {
        /// <summary>Token representing gt keyword</summary>
        internal static readonly ExpressionToken GreaterThan = new ExpressionToken { Text = ExpressionConstants.KeywordGreaterThan, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing eq keyword</summary>
        internal static readonly ExpressionToken EqualsTo = new ExpressionToken { Text = ExpressionConstants.KeywordEqual, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing lt keyword</summary>
        internal static readonly ExpressionToken LessThan = new ExpressionToken { Text = ExpressionConstants.KeywordLessThan, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>InternalKind of token.</summary>
        internal ExpressionTokenKind Kind;

        /// <summary>Token text.</summary>
        internal string Text;

        /// <summary>Position of token.</summary>
        internal int Position;

        /// <summary>Checks whether this token is a comparison operator.</summary>
        internal bool IsComparisonOperator
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                if (this.Kind != ExpressionTokenKind.Identifier)
                {
                    return false;
                }

                return
                    this.Text == ExpressionConstants.KeywordEqual ||
                    this.Text == ExpressionConstants.KeywordNotEqual ||
                    this.Text == ExpressionConstants.KeywordLessThan ||
                    this.Text == ExpressionConstants.KeywordGreaterThan ||
                    this.Text == ExpressionConstants.KeywordLessThanOrEqual ||
                    this.Text == ExpressionConstants.KeywordGreaterThanOrEqual;
            }
        }

        /// <summary>Checks whether this token is an equality operator.</summary>
        internal bool IsEqualityOperator
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                return 
                    this.Kind == ExpressionTokenKind.Identifier &&
                    (this.Text == ExpressionConstants.KeywordEqual || 
                     this.Text == ExpressionConstants.KeywordNotEqual);
            }
        }

        /// <summary>Checks whether this token is a valid token for a key value.</summary>
        internal bool IsKeyValueToken
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                return
                    this.Kind == ExpressionTokenKind.BinaryLiteral ||
                    this.Kind == ExpressionTokenKind.BooleanLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeOffsetLiteral ||
                    this.Kind == ExpressionTokenKind.TimeLiteral ||
                    this.Kind == ExpressionTokenKind.GuidLiteral ||
                    this.Kind == ExpressionTokenKind.StringLiteral ||
                    this.Kind == ExpressionTokenKind.GeographyLiteral ||
                    this.Kind == ExpressionTokenKind.GeometryLiteral ||
                    ExpressionLexerUtils.IsNumeric(this.Kind);
            }
        }

        /// <summary>Checks whether this token is a valid token for a function parameter.</summary>
        internal bool IsFunctionParameterToken
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.IsKeyValueToken || this.Kind == ExpressionTokenKind.BracketedExpression || this.Kind == ExpressionTokenKind.NullLiteral;
            }
        }

        /// <summary>Provides a string representation of this token.</summary>
        /// <returns>String representation of this token.</returns>
        public override string ToString()
        {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} @ {1}: [{2}]", this.Kind, this.Position, this.Text);
        }

        /// <summary>Gets the current identifier text.</summary>
        /// <returns>The current identifier text.</returns>
        internal string GetIdentifier()
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.Kind != ExpressionTokenKind.Identifier)
            {
                string message = Strings.ExpressionToken_IdentifierExpected(this.Position);
                throw new ODataException(message);
            }

            Debug.Assert(this.Text != null, "Text is null");
            return this.Text;
        }

        /// <summary>Checks that this token has the specified identifier.</summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>true if this is an identifier with the specified text.</returns>
        internal bool IdentifierIs(string id)
        {
            DebugUtils.CheckNoExternalCallers();

            return this.Kind == ExpressionTokenKind.Identifier && this.Text == id;
        }
    }
}
