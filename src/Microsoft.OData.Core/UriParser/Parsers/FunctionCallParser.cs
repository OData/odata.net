//---------------------------------------------------------------------
// <copyright file="FunctionCallParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Implementation of IFunctionCallParser that allows functions calls and parses arguments with a provided method.
    /// TODO: This inmplementaiton is incomplete.
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
        /// Create a new FunctionCallParser.
        /// </summary>
        /// <param name="lexer">Lexer positioned at a function identifier.</param>
        /// <param name="parser">The UriQueryExpressionParser.</param>
        public FunctionCallParser(ExpressionLexer lexer, UriQueryExpressionParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(lexer, "lexer");
            ExceptionUtils.CheckArgumentNotNull(parser, "parser");
            this.lexer = lexer;
            this.parser = parser;
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
        /// Parses an identifier that represents a function.
        /// </summary>
        /// <param name="parent">Token for the parent of the function being parsed.</param>
        /// <returns>QueryToken representing this function.</returns>
        public QueryToken ParseIdentifierAsFunction(QueryToken parent)
        {
            string functionName;

            if (this.Lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot)
            {
                // handle the case where we prefix a function with its namespace.
                functionName = this.Lexer.ReadDottedIdentifier(false);
            }
            else
            {
                Debug.Assert(this.Lexer.CurrentToken.Kind == ExpressionTokenKind.Identifier, "Only identifier tokens can be treated as function calls.");
                functionName = this.Lexer.CurrentToken.Text;
                this.Lexer.NextToken();
            }

            FunctionParameterToken[] arguments = this.ParseArgumentListOrEntityKeyList();

            return new FunctionCallToken(functionName, arguments, parent);
        }

        /// <summary>
        /// Parses argument lists or entity key value list.
        /// </summary>
        /// <returns>The lexical tokens representing the arguments.</returns>
        public FunctionParameterToken[] ParseArgumentListOrEntityKeyList()
        {
            if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
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
            List<FunctionParameterToken> argList = new List<FunctionParameterToken>();
            while (true)
            {
                argList.Add(new FunctionParameterToken(null, this.parser.ParseExpression()));
                if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

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
    }
}
