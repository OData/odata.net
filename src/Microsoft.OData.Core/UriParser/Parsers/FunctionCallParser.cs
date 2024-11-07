﻿//---------------------------------------------------------------------
// <copyright file="FunctionCallParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Implementation of IFunctionCallParser that allows functions calls and parses arguments with a provided method.
    /// TODO: This implementation is incomplete.
    /// </summary>
    internal sealed class FunctionCallParser : IFunctionCallParser
    {
        /// <summary>
        /// Reference to the lexer.
        /// </summary>
        private readonly ExpressionLexer lexer;

        /// <summary>
        /// Method used to parse arguments.
        /// </summary>
        private readonly UriQueryExpressionParser parser;

        /// <summary>
        /// If set to true, catches any ODataException thrown while trying to parse function arguments.
        /// </summary>
        private readonly bool restoreStateIfFail;

        /// <summary>
        /// If the function call is cast or isof, set to true.
        /// </summary>
        private bool isFunctionCallNameCastOrIsOf = false;

        /// <summary>
        /// Create a new FunctionCallParser.
        /// </summary>
        /// <param name="lexer">Lexer positioned at a function identifier.</param>
        /// <param name="parser">The UriQueryExpressionParser.</param>
        public FunctionCallParser(ExpressionLexer lexer, UriQueryExpressionParser parser)
            : this(lexer, parser, false /* restoreStateIfFail */)
        {
        }

        /// <summary>
        /// Create a new FunctionCallParser.
        /// </summary>
        /// <param name="lexer">Lexer positioned at a function identifier.</param>
        /// <param name="parser">The UriQueryExpressionParser.</param>
        /// <param name="restoreStateIfFail">If set to true, catches any ODataException thrown while trying to parse function arguments.</param>
        public FunctionCallParser(ExpressionLexer lexer, UriQueryExpressionParser parser, bool restoreStateIfFail)
        {
            ExceptionUtils.CheckArgumentNotNull(lexer, "lexer");
            ExceptionUtils.CheckArgumentNotNull(parser, "parser");
            this.lexer = lexer;
            this.parser = parser;
            this.restoreStateIfFail = restoreStateIfFail;
        }

        /// <summary>
        /// Reference to the underlying UriQueryExpressionParser.
        /// </summary>
        public UriQueryExpressionParser UriQueryExpressionParser
        {
            get { return this.parser; }
        }

        /// <summary>
        /// Reference to the lexer.
        /// </summary>
        public ExpressionLexer Lexer
        {
            get { return this.lexer; }
        }

        /// <summary>
        /// Try to parse an identifier that represents a function. If the parser instance has
        /// <see cref="restoreStateIfFail"/> set as false, then an <see cref="ODataException"/>
        /// is thrown if the parser finds an error.
        /// </summary>
        /// <param name="parent">Token for the parent of the function being parsed.</param>
        /// <param name="result">QueryToken representing this function.</param>
        /// <returns>True if the parsing was successful.</returns>
        public bool TryParseIdentifierAsFunction(QueryToken parent, out QueryToken result)
        {
            result = null;
            ReadOnlySpan<char> functionName;

            ExpressionLexer.ExpressionLexerPosition position = lexer.SnapshotPosition();

            if (this.Lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot)
            {
                // handle the case where we prefix a function with its namespace.
                functionName = this.Lexer.ReadDottedIdentifier(false);
            }
            else
            {
                Debug.Assert(this.Lexer.CurrentToken.Kind == ExpressionTokenKind.Identifier, "Only identifier tokens can be treated as function calls.");
                functionName = this.Lexer.CurrentToken.Span;
                this.Lexer.NextToken();
            }

            // This is used to set the parent of the next argument to the current argument for cast and isof functions.
            isFunctionCallNameCastOrIsOf = functionName.SequenceEqual(ExpressionConstants.UnboundFunctionCast.AsSpan()) || functionName.SequenceEqual(ExpressionConstants.UnboundFunctionIsOf.AsSpan());

            FunctionParameterToken[] arguments = this.ParseArgumentListOrEntityKeyList(() => lexer.RestorePosition(position));
            if (arguments != null)
            {
                result = new FunctionCallToken(functionName.ToString(), arguments, parent);
            }

            return result != null;
        }

        /// <summary>
        /// Parses argument lists or entity key value list.
        /// </summary>
        /// <param name="restoreAction">Action invoked for restoring a state during failure.</param>
        /// <returns>The lexical tokens representing the arguments.</returns>
        public FunctionParameterToken[] ParseArgumentListOrEntityKeyList(Action restoreAction = null)
        {
            if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                if (restoreStateIfFail && restoreAction != null)
                {
                    restoreAction();
                    return null;
                }

                throw new ODataException(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.Lexer.CurrentToken.Position, this.Lexer.ExpressionText));
            }

            this.Lexer.NextToken();
            FunctionParameterToken[] arguments;
            if (this.Lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
            {
                arguments = FunctionParameterToken.EmptyParameterList;
            }
            else
            {
                arguments = this.ParseArguments();
            }

            if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                if (restoreStateIfFail && restoreAction != null)
                {
                    restoreAction();
                    return null;
                }

                throw new ODataException(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.Lexer.CurrentToken.Position, this.Lexer.ExpressionText));
            }

            this.Lexer.NextToken();
            return arguments;
        }

        /// <summary>
        /// Parses comma-separated arguments.
        /// </summary>
        /// <remarks>
        /// Arguments can either be of the form a=1,b=2,c=3 or 1,2,3.
        /// They cannot be mixed between those two styles.
        /// </remarks>
        /// <returns>The lexical tokens representing the arguments.</returns>
        public FunctionParameterToken[] ParseArguments()
        {
            ICollection<FunctionParameterToken> argList;
            if (this.TryReadArgumentsAsNamedValues(out argList))
            {
                return argList.ToArray();
            }

            return this.ReadArgumentsAsPositionalValues().ToArray();
        }

        /// <summary>
        /// Read the list of arguments as a set of positional values
        /// </summary>
        /// <returns>A list of FunctionParameterTokens representing each argument</returns>
        private List<FunctionParameterToken> ReadArgumentsAsPositionalValues()
        {
            // Store the parent expression of the current argument.
            Stack<QueryToken> expressionParents = new Stack<QueryToken>();

            List<FunctionParameterToken> argList = new List<FunctionParameterToken>();
            while (true)
            {
                // If we have a parent expression, we need to set the parent of the next argument to the current argument.
                QueryToken parentExpression = expressionParents.Count > 0 ? expressionParents.Pop() : null;
                QueryToken parameterToken = this.parser.ParseExpression();

                // If the function call is cast or isof, set the parent of the next argument to the current argument.
                if (parentExpression != null && isFunctionCallNameCastOrIsOf)
                {
                    parameterToken = SetParentForCurrentParameterToken(parentExpression, parameterToken);
                }

                argList.Add(new FunctionParameterToken(null, parameterToken));
                if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

                // In case of comma, we need to parse the next argument
                // but first we need to set the parent of the next argument to the current argument.
                expressionParents.Push(parameterToken);

                this.Lexer.NextToken();
            }

            return argList;
        }

        /// <summary>
        /// Try to read the list of arguments as a set of named values
        /// </summary>
        /// <param name="argList">the parsed list of arguments</param>
        /// <returns>true if the arguments were successfully read.</returns>
        private bool TryReadArgumentsAsNamedValues(out ICollection<FunctionParameterToken> argList)
        {
            if (this.parser.TrySplitFunctionParameters(out argList))
            {
                if (argList.Select(t => t.ParameterName).Distinct().Count() != argList.Count)
                {
                    throw new ODataException(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the parent of the next argument to the current argument.
        /// For example, in the following query:
        ///     cast(Location, NS.HomeAddress) where Location is the parentExpression and NS.HomeAddress is the parameterToken.
        ///     isof(Location, NS.HomeAddress) where Location is the parentExpression and NS.HomeAddress is the parameterToken.
        /// </summary>
        /// <param name="parentExpression">The previous parameter token.</param>
        /// <param name="parameterToken">The current parameter token.</param>
        /// <returns>The updated parameter token.</returns>
        private static QueryToken SetParentForCurrentParameterToken(QueryToken parentExpression, QueryToken parameterToken)
        {
            // If the parameter is a dotted identifier, we need to set the parent of the next argument to the current argument.
            // But if it is primitive literal, no need to set the parent.
            if (parameterToken is DottedIdentifierToken dottedIdentifierToken && dottedIdentifierToken.NextToken == null)
            {
                // Check if the dottedIdentifier is a primitive type
                EdmPrimitiveTypeKind primitiveTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(dottedIdentifierToken.Identifier);

                // If the dottedIdentifier is not a primitive type, set the parent of the next argument to the current argument.
                //  cast(1, Edm.Int32) -> Edm.Int32 is a dottedIdentifierToken but it is a primitive type
                if (primitiveTypeKind == EdmPrimitiveTypeKind.None)
                {
                    dottedIdentifierToken.NextToken = parentExpression;
                    parameterToken = dottedIdentifierToken;
                }
            }

            return parameterToken;
        }
    }
}
