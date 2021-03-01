//---------------------------------------------------------------------
// <copyright file="ParameterAliasBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// This class binds parameter alias by :
    /// (1) parse and bind the alias value's expression into SingleValueNode, then get its type.
    /// (2) assign SingleValueNode's type to alias' ParameterAliasNode.
    /// </summary>
    internal sealed class ParameterAliasBinder
    {
        /// <summary>
        /// Method to use to visit the token tree and bind the tokens recursively.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Creates an OrderByBinder
        /// </summary>
        /// <param name="bindMethod">Method to use to visit the token tree and bind the tokens recursively.</param>
        internal ParameterAliasBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(bindMethod, "bindMethod");
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Bind a parameter alias which is inside another alias value.
        /// </summary>
        /// <param name="bindingState">The alias name which is inside another alias value.</param>
        /// <param name="aliasToken">The cache of alias value nodes</param>
        /// <returns>The semantics node tree for alias (the @p1 in "@p1=...", not alias value expression)</returns>
        internal ParameterAliasNode BindParameterAlias(BindingState bindingState, FunctionParameterAliasToken aliasToken)
        {
            ExceptionUtils.CheckArgumentNotNull(bindingState, "bindingState");
            ExceptionUtils.CheckArgumentNotNull(aliasToken, "aliasToken");

            string alias = aliasToken.Alias;
            ParameterAliasValueAccessor aliasValueAccessor = bindingState.Configuration.ParameterAliasValueAccessor;
            if (aliasValueAccessor == null)
            {
                return new ParameterAliasNode(alias, null);
            }

            // in cache?
            SingleValueNode aliasValueNode = null;
            if (!aliasValueAccessor.ParameterAliasValueNodesCached.TryGetValue(alias, out aliasValueNode))
            {
                // has value expression?
                string aliasValueExpression = aliasValueAccessor.GetAliasValueExpression(alias);
                if (aliasValueExpression == null)
                {
                    aliasValueAccessor.ParameterAliasValueNodesCached[alias] = null;
                }
                else
                {
                    aliasValueNode = this.ParseAndBindParameterAliasValueExpression(bindingState, aliasValueExpression, aliasToken.ExpectedParameterType);
                    aliasValueAccessor.ParameterAliasValueNodesCached[alias] = aliasValueNode;
                }
            }

            return new ParameterAliasNode(alias, aliasValueNode.GetEdmTypeReference());
        }

        /// <summary>
        /// Parse expression into syntaxs token tree, and bind it into semantics node tree.
        /// </summary>
        /// <param name="bindingState">The BindingState.</param>
        /// <param name="aliasValueExpression">The alias value's expression text.</param>
        /// <param name="parameterType">The edm type of the parameter.</param>
        /// <returns>The semantics node of the expression text.</returns>
        private SingleValueNode ParseAndBindParameterAliasValueExpression(BindingState bindingState, string aliasValueExpression, IEdmTypeReference parameterType)
        {
            // Get the syntactic representation of the filter expression
            // TODO: change Settings.FilterLimit to ParameterAliasValueLimit
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(bindingState.Configuration.Settings.FilterLimit);
            QueryToken aliasValueToken = expressionParser.ParseExpressionText(aliasValueExpression);

            // Special logic to handle parameter alias token.
            aliasValueToken = ParseComplexOrCollectionAlias(aliasValueToken, parameterType, bindingState.Model);

            // Get the semantic node, and check for SingleValueNode
            QueryNode aliasValueNode = this.bindMethod(aliasValueToken);
            SingleValueNode result = aliasValueNode as SingleValueNode;
            if (result == null)
            {
                // TODO: add string resource
                throw new ODataException(Strings.MetadataBinder_ParameterAliasValueExpressionNotSingleValue);
            }

            return result;
        }

        /// <summary>
        /// Parse the complex/collection value in parameter alias.
        /// </summary>
        /// <param name="queryToken">The parsed token.</param>
        /// <param name="parameterType">The expected parameter type.</param>
        /// <param name="model">The model</param>
        /// <returns>Token with complex/collection value passed.</returns>
        private static QueryToken ParseComplexOrCollectionAlias(QueryToken queryToken, IEdmTypeReference parameterType, IEdmModel model)
        {
            LiteralToken valueToken = queryToken as LiteralToken;
            string valueStr;
            if (valueToken != null && (valueStr = valueToken.Value as string) != null && !string.IsNullOrEmpty(valueToken.OriginalText))
            {
                var lexer = new ExpressionLexer(valueToken.OriginalText, true /*moveToFirstToken*/, false /*useSemicolonDelimiter*/, true /*parsingFunctionParameters*/);
                if (lexer.CurrentToken.Kind == ExpressionTokenKind.BracketedExpression || lexer.CurrentToken.Kind == ExpressionTokenKind.BracedExpression)
                {
                    object result = valueStr;
                    if (!parameterType.IsStructured() && !parameterType.IsStructuredCollectionType())
                    {
                        result = ODataUriUtils.ConvertFromUriLiteral(valueStr, ODataVersion.V4, model, parameterType);
                    }

                    // For non-primitive type, we have to pass parameterType to LiteralToken, then to ConstantNode so the service can know what the type it is.
                    return new LiteralToken(result, valueToken.OriginalText, parameterType);
                }
            }

            return queryToken;
        }
    }
}
