//---------------------------------------------------------------------
// <copyright file="FunctionCallParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

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

            FunctionParameterToken[] arguments = this.ParseArgumentListOrEntityKeyList(() => lexer.RestorePosition(position), functionName);
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
        /// <param name="functionName">The name of the function being called. Default is an empty span.</param>
        /// <returns>The lexical tokens representing the arguments.</returns>
        public FunctionParameterToken[] ParseArgumentListOrEntityKeyList(Action restoreAction = null, ReadOnlySpan<char> functionName = default)
        {
            if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                if (restoreStateIfFail && restoreAction != null)
                {
                    restoreAction();
                    return null;
                }

                throw new ODataException(Error.Format(SRResources.UriQueryExpressionParser_OpenParenExpected, this.Lexer.CurrentToken.Position, this.Lexer.ExpressionText));
            }

            this.Lexer.NextToken();
            FunctionParameterToken[] arguments;
            if (this.Lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
            {
                arguments = FunctionParameterToken.EmptyParameterList;
            }
            else
            {
                arguments = this.ParseArguments(functionName);
            }

            if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                if (restoreStateIfFail && restoreAction != null)
                {
                    restoreAction();
                    return null;
                }

                throw new ODataException(Error.Format(SRResources.UriQueryExpressionParser_CloseParenOrCommaExpected, this.Lexer.CurrentToken.Position, this.Lexer.ExpressionText));
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
        /// <param name="functionName">The name of the function being called. Default is an empty span.</param>
        /// <returns>The lexical tokens representing the arguments.</returns>
        public FunctionParameterToken[] ParseArguments(ReadOnlySpan<char> functionName = default)
        {
            ICollection<FunctionParameterToken> argList;
            if (this.TryReadArgumentsAsNamedValues(out argList))
            {
                return argList.ToArray();
            }

            return this.ReadArgumentsAsPositionalValues(functionName).ToArray();
        }

        /// <summary>
        /// Read the list of arguments as a set of positional values
        /// </summary>
        /// <param name="functionName">The name of the function being called. Default is an empty span.</param>
        /// <returns>A list of FunctionParameterTokens representing each argument</returns>
        private List<FunctionParameterToken> ReadArgumentsAsPositionalValues(ReadOnlySpan<char> functionName = default)
        {
            // Store the parent expression of the current argument.
            Stack<QueryToken> expressionParents = new Stack<QueryToken>();
            bool isFunctionCallNameCastOrIsOf = functionName.Length > 0 &&
                (functionName.SequenceEqual(ExpressionConstants.UnboundFunctionCast.AsSpan()) || functionName.SequenceEqual(ExpressionConstants.UnboundFunctionIsOf.AsSpan()));
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
                    throw new ODataException(SRResources.FunctionCallParser_DuplicateParameterOrEntityKeyName);
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
            if (parameterToken is not DottedIdentifierToken dottedIdentifierToken || dottedIdentifierToken?.NextToken is not null)
            {
                return parameterToken;
            }

            // Check if the identifier is a primitive type
            IEdmSchemaType schemaType = NormalizedModelElementsCache.EdmCoreModelInstance.FindSchemaTypes(dottedIdentifierToken.Identifier)?.FirstOrDefault();

            // If the identifier is a primitive type
            if (schemaType is IEdmPrimitiveType)
            {
                return parameterToken;
            }

            // Set the parent of the next argument to the current argument
            dottedIdentifierToken.NextToken = parentExpression;
            return dottedIdentifierToken;
        }
    }
}
