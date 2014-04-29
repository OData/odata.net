//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;

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
                    this.Text == ExpressionConstants.KeywordGreaterThanOrEqual ||
                    this.Text == ExpressionConstants.KeywordHas;
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

        /// <summary>Checks whether this token is a valid token for a key value.</summary>
        internal bool IsKeyValueToken
        {
            get
            {
                return
                    this.Kind == ExpressionTokenKind.BinaryLiteral ||
                    this.Kind == ExpressionTokenKind.BooleanLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeOffsetLiteral ||
                    this.Kind == ExpressionTokenKind.DurationLiteral ||
                    this.Kind == ExpressionTokenKind.GuidLiteral ||
                    this.Kind == ExpressionTokenKind.StringLiteral ||
                    this.Kind == ExpressionTokenKind.GeographyLiteral ||
                    this.Kind == ExpressionTokenKind.GeometryLiteral ||
                    this.Kind == ExpressionTokenKind.QuotedLiteral ||
                    ExpressionLexerUtils.IsNumeric(this.Kind);
            }
        }

        /// <summary>Checks whether this token is a valid token for a function parameter.</summary>
        internal bool IsFunctionParameterToken
        {
            get
            {
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
            return this.Kind == ExpressionTokenKind.Identifier && this.Text == id;
        }
    }
}
