//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// A set of extensions to <see cref="ExpressionLexer"/> for parsing literals.
    /// </summary>
    internal static class ExpressionLexerLiteralExtensions
    {
        /// <summary>
        /// Returns whether the <paramref name="tokenKind"/> is a primitive literal type: 
        /// Binary, Boolean, DateTime, Decimal, Double, Guid, In64, Integer, Null, Single, or String.
        /// Internal for test use only
        /// </summary>
        /// <param name="tokenKind">InternalKind of token.</param>
        /// <returns>Whether the <paramref name="tokenKind"/> is a literal type.</returns>
        internal static Boolean IsLiteralType(this ExpressionTokenKind tokenKind)
        {
            switch (tokenKind)
            {
                case ExpressionTokenKind.BinaryLiteral:
                case ExpressionTokenKind.BooleanLiteral:
                case ExpressionTokenKind.DateTimeLiteral:
                case ExpressionTokenKind.DecimalLiteral:
                case ExpressionTokenKind.DoubleLiteral:
                case ExpressionTokenKind.GuidLiteral:
                case ExpressionTokenKind.Int64Literal:
                case ExpressionTokenKind.IntegerLiteral:
                case ExpressionTokenKind.NullLiteral:
                case ExpressionTokenKind.SingleLiteral:
                case ExpressionTokenKind.StringLiteral:
                case ExpressionTokenKind.TimeOfDayLiteral:
                case ExpressionTokenKind.DateLiteral:
                case ExpressionTokenKind.DateTimeOffsetLiteral:
                case ExpressionTokenKind.DurationLiteral:
                case ExpressionTokenKind.GeographyLiteral:
                case ExpressionTokenKind.GeometryLiteral:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>Reads the next token, checks that it is a literal token type, converts to to a Common Language Runtime value as appropriate, and returns the value.</summary>
        /// <param name="expressionLexer">The expression lexer.</param>
        /// <returns>The value represented by the next token.</returns>
        internal static object ReadLiteralToken(this ExpressionLexer expressionLexer)
        {
            expressionLexer.NextToken();

            if (expressionLexer.CurrentToken.Kind.IsLiteralType())
            {
                return TryParseLiteral(expressionLexer);
            }

            throw new ODataException(ODataErrorStrings.ExpressionLexer_ExpectedLiteralToken(expressionLexer.CurrentToken.Text));
        }

        /// <summary>
        /// Parses null literals.
        /// </summary>
        /// <param name="expressionLexer">The expression lexer.</param>
        /// <returns>The literal token produced by building the given literal.</returns>
        private static object ParseNullLiteral(this ExpressionLexer expressionLexer)
        {
            Debug.Assert(expressionLexer.CurrentToken.Kind == ExpressionTokenKind.NullLiteral, "this.lexer.CurrentToken.InternalKind == ExpressionTokenKind.NullLiteral");

            expressionLexer.NextToken();
            ODataNullValue nullValue = new ODataNullValue();
            return nullValue;
        }

        /// <summary>
        /// Parses typed literals.
        /// </summary>
        /// <param name="expressionLexer">The expression lexer.</param>
        /// <param name="targetTypeReference">Expected type to be parsed.</param>
        /// <returns>The literal token produced by building the given literal.</returns>
        private static object ParseTypedLiteral(this ExpressionLexer expressionLexer, IEdmPrimitiveTypeReference targetTypeReference)
        {
            object targetValue;
            string reason;
            if (!UriPrimitiveTypeParser.TryUriStringToPrimitive(expressionLexer.CurrentToken.Text, targetTypeReference, out targetValue, out reason))
            {
                string message;

                if (reason == null)
                {
                    message = ODataErrorStrings.UriQueryExpressionParser_UnrecognizedLiteral(
                        targetTypeReference.FullName(),
                        expressionLexer.CurrentToken.Text,
                        expressionLexer.CurrentToken.Position,
                        expressionLexer.ExpressionText);
                }
                else
                {
                    message = ODataErrorStrings.UriQueryExpressionParser_UnrecognizedLiteralWithReason(
                        targetTypeReference.FullName(),
                        expressionLexer.CurrentToken.Text,
                        expressionLexer.CurrentToken.Position,
                        expressionLexer.ExpressionText,
                        reason);
                }

                throw new ODataException(message);
            }

            expressionLexer.NextToken();
            return targetValue;
        }

        /// <summary>
        /// Parses a literal. 
        /// Precondition: lexer is at a literal token type: Boolean, DateTime, Decimal, Null, String, Int64, Integer, Double, Single, Guid, Binary.
        /// </summary>
        /// <param name="expressionLexer">The expression lexer.</param>
        /// <returns>The literal query token or null if something else was found.</returns>
        private static object TryParseLiteral(this ExpressionLexer expressionLexer)
        {
            Debug.Assert(expressionLexer.CurrentToken.Kind.IsLiteralType(), "TryParseLiteral called when not at a literal type token");

            switch (expressionLexer.CurrentToken.Kind)
            {
                case ExpressionTokenKind.BooleanLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetBoolean(false));
                case ExpressionTokenKind.DecimalLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetDecimal(false));
                case ExpressionTokenKind.NullLiteral:
                    return ParseNullLiteral(expressionLexer);
                case ExpressionTokenKind.StringLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetString(true));
                case ExpressionTokenKind.Int64Literal:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetInt64(false));
                case ExpressionTokenKind.IntegerLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetInt32(false));
                case ExpressionTokenKind.DoubleLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetDouble(false));
                case ExpressionTokenKind.SingleLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetSingle(false));
                case ExpressionTokenKind.GuidLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetGuid(false));
                case ExpressionTokenKind.BinaryLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetBinary(true));
                case ExpressionTokenKind.DateLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetDate(false));
                case ExpressionTokenKind.DateTimeOffsetLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetDateTimeOffset(false));
                case ExpressionTokenKind.DurationLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false));
                case ExpressionTokenKind.GeographyLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false));
                case ExpressionTokenKind.GeometryLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, false));
                case ExpressionTokenKind.QuotedLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetString(true));
                case ExpressionTokenKind.TimeOfDayLiteral:
                    return ParseTypedLiteral(expressionLexer, EdmCoreModel.Instance.GetTimeOfDay(false));
            }

            return null;
        }
    }
}
