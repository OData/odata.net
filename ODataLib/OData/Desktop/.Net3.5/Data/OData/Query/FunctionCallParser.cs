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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
        private readonly UriQueryExpressionParser.Parser parseMethod;

        /// <summary>
        /// Create a new FunctionCallParser.
        /// </summary>
        /// <param name="lexer">Lexer positioned at a function identifier.</param>
        /// <param name="parseMethod">Method to use for parsing individual arguments in the function.</param>
        public FunctionCallParser(ExpressionLexer lexer, UriQueryExpressionParser.Parser parseMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(lexer, "lexer");
            ExceptionUtils.CheckArgumentNotNull(parseMethod, "parseMethod");
            this.lexer = lexer;
            this.parseMethod = parseMethod;
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

            FunctionParameterToken[] arguments = this.ParseArgumentList();

            return new FunctionCallToken(functionName, arguments, parent);
        }

        /// <summary>
        /// Parses argument lists.
        /// </summary>
        /// <returns>The lexical tokens representing the arguments.</returns>
        public FunctionParameterToken[] ParseArgumentList()
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
                argList.Add(new FunctionParameterToken(null, this.parseMethod()));
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
            if (this.lexer.TrySplitFunctionParameters(out argList))
            {
                if (argList.Select(t => t.ParameterName).Distinct().Count() != argList.Count)
                {
                    throw new ODataException(ODataErrorStrings.FunctionCallParser_DuplicateParameterName);
                }

                return true;
            }

            return false;
        }
    }
}
