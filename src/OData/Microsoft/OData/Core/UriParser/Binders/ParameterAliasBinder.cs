//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// This class binds parameter alias by :
    /// (1) parse and bind the alias value's expression into SingleValueNode, then get its type. 
    /// (2) asign SingleValueNode's type to alias' ParameterAliasNode.
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
                    aliasValueNode = this.ParseAndBindParameterAliasValueExpression(bindingState, aliasValueExpression);
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
        /// <returns>The semantcs node of the expression text.</returns>
        private SingleValueNode ParseAndBindParameterAliasValueExpression(BindingState bindingState, string aliasValueExpression)
        {
            // Get the syntactic representation of the filter expression
            // TODO: change Settings.FilterLimit to ParameterAliasValueLimit
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(bindingState.Configuration.Settings.FilterLimit);
            QueryToken aliasValueToken = expressionParser.ParseExpressionText(aliasValueExpression);

            // Get the semantic node, and check for SingleValueNode
            QueryNode aliasValueNode = this.bindMethod(aliasValueToken);
            SingleValueNode result = aliasValueNode as SingleValueNode;
            if (result == null)
            {
                // TODO: add string resource
                throw new ODataException("ODataErrorStrings.MetadataBinder_ParameterAliasValueExpressionNotSingleValue");
            }

            return result;
        }
    }
}
