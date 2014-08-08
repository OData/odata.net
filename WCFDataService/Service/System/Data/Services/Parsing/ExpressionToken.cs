//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Parsing
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>Use this class to represent a lexical token.</summary>
    [DebuggerDisplay("{Id} @ {Position}: [{Text}]")]
    internal struct ExpressionToken
    {
        /// <summary>Token representing gt keyword</summary>
        internal static readonly ExpressionToken GreaterThan = new ExpressionToken { Text = ExpressionConstants.KeywordGreaterThan, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing eq keyword</summary>
        internal static readonly ExpressionToken EqualsTo = new ExpressionToken { Text = ExpressionConstants.KeywordEqual, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing lt keyword</summary>
        internal static readonly ExpressionToken LessThan = new ExpressionToken { Text = ExpressionConstants.KeywordLessThan, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Kind of token.</summary>
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
                return 
                    this.Kind == ExpressionTokenKind.Identifier &&
                    (this.Text == ExpressionConstants.KeywordEqual || 
                     this.Text == ExpressionConstants.KeywordNotEqual);
            }
        }

        /// <summary>Checks whether this token is a valid token for a literal.</summary>
        internal bool IsLiteral
        {
            get
            {
                return
                    this.Kind == ExpressionTokenKind.NullLiteral ||
                    this.Kind == ExpressionTokenKind.BinaryLiteral ||
                    this.Kind == ExpressionTokenKind.BooleanLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeLiteral ||
                    this.Kind == ExpressionTokenKind.GuidLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeOffsetLiteral ||
                    this.Kind == ExpressionTokenKind.TimeLiteral ||
                    this.Kind == ExpressionTokenKind.StringLiteral ||
                    ExpressionLexerUtils.IsNumeric(this.Kind);
            }
        }

        /// <summary>Provides a string representation of this token.</summary>
        /// <returns>String representation of this token.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} @ {1}: [{2}]", this.Kind, this.Position, this.Text);
        }

        /// <summary>Gets the current identifier text.</summary>
        /// <returns>The current identifier text.</returns>
        internal string GetIdentifier()
        {
            if (this.Kind != ExpressionTokenKind.Identifier)
            {
                throw DataServiceException.CreateSyntaxError(Strings.RequestQueryParser_IdentifierExpected);
            }

            return this.Text;
        }

        /// <summary>Checks that this token has the specified identifier.</summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>true if this is an identifier with the specified text.</returns>
        internal bool IdentifierIs(string id)
        {
            return this.Kind == ExpressionTokenKind.Identifier && this.Text == id;
        }

        /// <summary>Validates the current token is of the specified kind.</summary>
        /// <param name="t">Expected token kind.</param>
        internal void ValidateId(ExpressionTokenKind t)
        {
            if (this.Kind != t)
            {
                throw DataServiceException.CreateSyntaxError(Strings.RequestQueryParser_SyntaxError);
            }
        }
    }
}
