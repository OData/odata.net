//---------------------------------------------------------------------
// <copyright file="ExpressionLexerLiteralExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Parsers.Common;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
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
        private static object ParseTypedLiteral(this ExpressionLexer expressionLexer, IEdmTypeReference targetTypeReference)
        {
            UriLiteralParsingException typeParsingException;
            object targetValue = DefaultUriLiteralParser.Instance.ParseUriStringToType(expressionLexer.CurrentToken.Text, targetTypeReference, out typeParsingException);
            if (targetValue == null)
            {
                string message;

                if (typeParsingException == null)
                {
                    message = ODataErrorStrings.UriQueryExpressionParser_UnrecognizedLiteral(
                        targetTypeReference.FullName(),
                        expressionLexer.CurrentToken.Text,
                        expressionLexer.CurrentToken.Position,
                        expressionLexer.ExpressionText);

                    throw new ODataException(message);
                }
                else
                {
                    message = ODataErrorStrings.UriQueryExpressionParser_UnrecognizedLiteralWithReason(
                        targetTypeReference.FullName(),
                        expressionLexer.CurrentToken.Text,
                        expressionLexer.CurrentToken.Position,
                        expressionLexer.ExpressionText,
                        typeParsingException.Message);

                    throw new ODataException(message, typeParsingException);
                }
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
                case ExpressionTokenKind.NullLiteral:
                    return ParseNullLiteral(expressionLexer);
                case ExpressionTokenKind.BooleanLiteral:
                case ExpressionTokenKind.DecimalLiteral:
                case ExpressionTokenKind.StringLiteral:
                case ExpressionTokenKind.Int64Literal:
                case ExpressionTokenKind.IntegerLiteral:
                case ExpressionTokenKind.DoubleLiteral:
                case ExpressionTokenKind.SingleLiteral:
                case ExpressionTokenKind.GuidLiteral:
                case ExpressionTokenKind.BinaryLiteral:
                case ExpressionTokenKind.DateLiteral:
                case ExpressionTokenKind.DateTimeOffsetLiteral:
                case ExpressionTokenKind.DurationLiteral:
                case ExpressionTokenKind.GeographyLiteral:
                case ExpressionTokenKind.GeometryLiteral:
                case ExpressionTokenKind.QuotedLiteral:
                case ExpressionTokenKind.TimeOfDayLiteral:
                case ExpressionTokenKind.CustomTypeLiteral:
                    return ParseTypedLiteral(expressionLexer, expressionLexer.CurrentToken.GetLiteralEdmTypeReference());
                default:
                    return null;
            }
        }
    }
}