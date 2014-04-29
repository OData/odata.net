//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
                    throw new ODataException(ODataErrorStrings.FunctionCallParser_DuplicateParameterName);
                }

                return true;
            }

            return false;
        }
    }
}
