//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Component for parsing function parameters in both $filter/$orderby expressions and in paths.
    /// </summary>
    internal static class FunctionParameterParser
    {
        /// <summary>
        /// Tries to parse a collection of function parameters. Allows path and filter to share the core algorithm while representing parameters differently.
        /// </summary>
        /// <param name="lexer">The lexer to read from.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        internal static bool TrySplitFunctionParameters(this ExpressionLexer lexer, out ICollection<FunctionParameterToken> splitParameters)
        {
            DebugUtils.CheckNoExternalCallers();
            return lexer.TrySplitFunctionParameters(ExpressionTokenKind.CloseParen, out splitParameters);
        }

        /// <summary>
        /// Tries to parse a collection of function parameters for filter/orderby.
        /// </summary>
        /// <param name="splitParameters">The syntactically split parameters to parse.</param>
        /// <param name="configuration">The configuration for the URI Parser.</param>
        /// <param name="functionImport">The function import for the function whose parameters are being parsed.</param>
        /// <param name="parsedParameters">The parameters if they were successfully parsed.</param>
        /// <returns>Whether the parameters could be parsed.</returns>
        internal static bool TryParseFunctionParameters(ICollection<FunctionParameterToken> splitParameters, ODataUriParserConfiguration configuration, IEdmFunctionImport functionImport, out ICollection<FunctionParameterToken> parsedParameters)
        {
            DebugUtils.CheckNoExternalCallers();
            return TryParseFunctionParameters(splitParameters, configuration, functionImport, (paramName, convertedValue) => new FunctionParameterToken(paramName, new LiteralToken(convertedValue)), out parsedParameters);
        }

        /// <summary>
        /// Tries to parse a collection of function parameters for path.
        /// </summary>     
        /// <param name="functionName">The function name to use in error messages.</param>
        /// <param name="parenthesisExpression">The contents of the parentheses portion of the current path segment.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        internal static bool TrySplitFunctionParameters(string functionName, string parenthesisExpression, out ICollection<FunctionParameterToken> splitParameters)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(parenthesisExpression), "!string.IsNullOrEmpty(parenthesisExpression)");
            var lexer = new ExpressionLexer(parenthesisExpression, true /*moveToFirstToken*/, false /*useSemicolonDelimiter*/, true /*parsingFunctionParameters*/);

            if (lexer.CurrentToken.IsFunctionParameterToken)
            {
                splitParameters = null;
                return false;
            }

            return TrySplitFunctionParameters(lexer, ExpressionTokenKind.End, out splitParameters);
        }

        /// <summary>
        /// Tries to parse a collection of function parameters for path.
        /// </summary>     
        /// <param name="splitParameters">The split parameters from the syntactic parsing step.</param>
        /// <param name="configuration">The configuration for the URI Parser.</param>
        /// <param name="functionImport">The function import for the function whose parameters are being parsed.</param>
        /// <param name="parsedParameters">The parameters if they were successfully parsed.</param>
        /// <returns>Whether the parameters could be parsed.</returns>
        internal static bool TryParseFunctionParameters(ICollection<FunctionParameterToken> splitParameters, ODataUriParserConfiguration configuration, IEdmFunctionImport functionImport, out ICollection<OperationSegmentParameter> parsedParameters)
        {
            DebugUtils.CheckNoExternalCallers();
            return TryParseFunctionParameters(splitParameters, configuration, functionImport, (paramName, convertedValue) => new OperationSegmentParameter(paramName, convertedValue), out parsedParameters);
        }

        /// <summary>
        /// Tries to parse a collection of function parameters. Allows path and filter to share the core algorithm while representing parameters differently.
        /// </summary>
        /// <param name="lexer">The lexer to read from.</param>
        /// <param name="endTokenKind">The token kind that marks the end of the parameters.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        private static bool TrySplitFunctionParameters(this ExpressionLexer lexer, ExpressionTokenKind endTokenKind, out ICollection<FunctionParameterToken> splitParameters)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(lexer != null, "lexer != null");

            var parameters = new List<FunctionParameterToken>();
            splitParameters = parameters;

            ExpressionToken currentToken = lexer.CurrentToken;
            if (currentToken.Kind == endTokenKind)
            {
                return true;
            }

            if (currentToken.Kind != ExpressionTokenKind.Identifier || lexer.PeekNextToken().Kind != ExpressionTokenKind.Equal)
            {
                return false;
            }

            while (currentToken.Kind != endTokenKind)
            {
                lexer.ValidateToken(ExpressionTokenKind.Identifier);
                string identifier = lexer.CurrentToken.GetIdentifier();
                lexer.NextToken();

                lexer.ValidateToken(ExpressionTokenKind.Equal);
                lexer.NextToken();

                QueryToken parameterValue;
                if (!TryCreateParameterValueToken(lexer.CurrentToken, out parameterValue))
                {
                    throw new ODataException(ODataErrorStrings.ExpressionLexer_SyntaxError(lexer.Position, lexer.ExpressionText));
                }

                parameters.Add(new FunctionParameterToken(identifier, parameterValue));

                // Read the next parameterToken. We should be at the end, or find
                // we have a comma followed by something.
                lexer.NextToken();
                currentToken = lexer.CurrentToken;
                if (currentToken.Kind == ExpressionTokenKind.Comma)
                {
                    lexer.NextToken();
                    currentToken = lexer.CurrentToken;
                    if (currentToken.Kind == endTokenKind)
                    {
                        // Trailing comma.
                        throw new ODataException(ODataErrorStrings.ExpressionLexer_SyntaxError(lexer.Position, lexer.ExpressionText));
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to parse a collection of function parameters. Allows path and filter to share the core algorithm while representing parameters differently.
        /// </summary>
        /// <typeparam name="TParam">The type representing a parameter.</typeparam>
        /// <param name="splitParameters">The syntactically split parameters to parse.</param>
        /// <param name="configuration">The configuration for the URI Parser.</param>
        /// <param name="functionImport">The function import for the function whose parameters are being parsed.</param>
        /// <param name="createParameter">The callback to use for individual parameter parsing.</param>
        /// <param name="parsedParameters">The parameters if they were successfully parsed.</param>
        /// <returns>Whether the parameters could be parsed.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        private static bool TryParseFunctionParameters<TParam>(ICollection<FunctionParameterToken> splitParameters, ODataUriParserConfiguration configuration, IEdmFunctionImport functionImport, Func<string, object, TParam> createParameter, out ICollection<TParam> parsedParameters)
        {
            Debug.Assert(splitParameters != null, "splitParameters != null");
            Debug.Assert(createParameter != null, "createParameter != null");
            Debug.Assert(configuration != null, "configuration != null");
            Debug.Assert(functionImport != null, "functionImport != null");

            parsedParameters = new List<TParam>(splitParameters.Count);
            foreach (var splitParameter in splitParameters)
            {
                TParam parameter;

                IEdmTypeReference expectedType = null;
                IEdmFunctionParameter edmFunctionParameter = null;

                try
                {
                    edmFunctionParameter = functionImport.FindParameter(splitParameter.ParameterName);
                }
                catch (InvalidOperationException ex)
                {
                    // this can throw an exception if there are multiple parameters with the same name..
                    // catch that exception and throw something more sane.
                    throw new ODataException(ODataErrorStrings.FunctionCallParser_DuplicateParameterName, ex);
                }

                Debug.Assert(edmFunctionParameter != null, "At this point we should know that the parameter names match the given function import.");
                expectedType = edmFunctionParameter.Type;

                if (!TryCreateParameter(splitParameter, configuration, expectedType, o => createParameter(splitParameter.ParameterName, o), out parameter))
                {
                    return false;
                }

                parsedParameters.Add(parameter);
            }

            return true;
        }

        /// <summary>
        /// Tries to create a parameter using any representation based on the provided delegate for creating it from a converted value.
        /// </summary>
        /// <param name="expressionToken">The current expression parameterToken from the lexer.</param>
        /// <param name="parameterValue">The parameter value if one was successfully created.</param>
        /// <returns>Whether the parameter could be created from the parameterToken.</returns>
        private static bool TryCreateParameterValueToken(ExpressionToken expressionToken, out QueryToken parameterValue)
        {
            if (expressionToken.Kind == ExpressionTokenKind.ParameterAlias)
            {
                parameterValue = new FunctionParameterAliasToken(expressionToken.Text);
                return true;
            }

            if (expressionToken.IsFunctionParameterToken)
            {
                parameterValue = new RawFunctionParameterValueToken(expressionToken.Text);
                return true;
            }

            parameterValue = null;
            return false;
        }

        /// <summary>
        /// Tries to create a parameter using any representation based on the provided delegate for creating it from a converted value.
        /// </summary>
        /// <typeparam name="TParam">The type used to represent a parameter.</typeparam>
        /// <param name="parameterToken">The token from the syntactic parsing step.</param>
        /// <param name="configuration">The configuration for the URI Parser.</param>
        /// <param name="expectedType">The type that the parameter is expected to resolve to.</param>
        /// <param name="createParameter">Callback to create the final parameter from the parsed value.</param>
        /// <param name="parameter">The parameter if one was successfully created.</param>
        /// <returns>Whether the parameter could be created from the parameterToken.</returns>
        private static bool TryCreateParameter<TParam>(FunctionParameterToken parameterToken, ODataUriParserConfiguration configuration, IEdmTypeReference expectedType, Func<object, TParam> createParameter, out TParam parameter)
        {
            Debug.Assert(parameterToken != null, "parameterToken != null");
            QueryToken valueToken = parameterToken.ValueToken;
            object convertedValue;
            if (valueToken.Kind == QueryTokenKind.FunctionParameterAlias && configuration.FunctionParameterAliasCallback == null)
            {
                convertedValue = new ODataUnresolvedFunctionParameterAlias(((FunctionParameterAliasToken)valueToken).Alias, expectedType);
            }
            else
            {
                string textToParse;
                if (valueToken.Kind == QueryTokenKind.FunctionParameterAlias)
                {
                    textToParse = configuration.FunctionParameterAliasCallback(((FunctionParameterAliasToken)valueToken).Alias);
                }
                else if (valueToken.Kind == QueryTokenKind.RawFunctionParameterValue)
                {
                    textToParse = ((RawFunctionParameterValueToken)valueToken).RawText;
                }
                else
                {
                    parameter = default(TParam);
                    return false;
                }

                if (textToParse == null)
                {
                    convertedValue = null;
                }
                else
                {
                    convertedValue = ODataUriUtils.ConvertFromUriLiteral(textToParse, ODataVersion.V3, configuration.Model, expectedType);
                }
            }

            parameter = createParameter(convertedValue);
            return true;
        }
    }
}
