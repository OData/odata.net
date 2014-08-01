//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Query
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;

    /// <summary>
    /// Build QueryNode to String Representation 
    /// </summary>
    internal sealed class NodeToStringBuilder : QueryNodeVisitor<String>
    {
        /// <summary>
        /// Translates a <see cref="AllNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(AllNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");

            String result = String.Concat(this.TranslateNode(node.Source), ExpressionConstants.SymbolForwardSlash, "all", ExpressionConstants.SymbolOpenParen, node.CurrentRangeVariable.Name, ": ", this.TranslateNode(node.Body), ExpressionConstants.SymbolClosedParen);
            return result;
        }

        /// <summary>
        /// Translates a <see cref="AnyNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(AnyNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");

            String result = String.Concat(this.TranslateNode(node.Source), ExpressionConstants.SymbolForwardSlash, "any", ExpressionConstants.SymbolOpenParen, node.CurrentRangeVariable.Name, ": ", this.TranslateNode(node.Body), ExpressionConstants.SymbolClosedParen);
            return result;
        }

        /// <summary>
        /// Translates a <see cref="BinaryOperatorNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(BinaryOperatorNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");

            var left = this.TranslateNode(node.Left);
            var right = this.TranslateNode(node.Right);
            return String.Concat(left, ' ', this.BinaryOperatorNodeToString(node.OperatorKind), ' ', right);
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(CollectionNavigationNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty.Name, node.NavigationSource);
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(CollectionPropertyAccessNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.Property.Name);
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(ConstantNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            if (node.Value == null)
            {
                return ExpressionConstants.KeywordNull;
            }

            return node.LiteralText;
        }

        /// <summary>
        /// Translates a <see cref="ConvertNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(ConvertNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslateNode(node.Source);
        }

        /// <summary>
        /// Translates a <see cref="EntityCollectionCastNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String of EntityCollectionCastNode.</returns>
        public override String Visit(EntityCollectionCastNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.EntityItemType.Definition.ToString());
        }

        /// <summary>
        /// Visit an CollectionPropertyCastNode
        /// </summary>
        /// <param name="node">the node to visit</param>
        /// <returns>The translated String of CollectionPropertyCastNode</returns>
        public override String Visit(CollectionPropertyCastNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.CollectionType.Definition.ToString());
        }

        /// <summary>
        /// Translates a <see cref="EntityRangeVariableReferenceNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(EntityRangeVariableReferenceNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            if (node.Name == "$it")
            {
                return String.Empty;
            }
            else
            {
                return node.Name;
            }
        }

        /// <summary>
        /// Translates a <see cref="NonentityRangeVariableReferenceNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(NonentityRangeVariableReferenceNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            if (node.Name == "$it")
            {
                return String.Empty;
            }
            else
            {
                return node.Name;
            }
        }

        /// <summary>
        /// Translates a <see cref="SingleEntityCastNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(SingleEntityCastNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.EntityTypeReference.Definition.ToString());
        }

        /// <summary>
        /// Translates a <see cref="SingleValueCastNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String of SingleValueCastNode.</returns>
        public override String Visit(SingleValueCastNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.TypeReference.Definition.ToString());
        }

        /// <summary>
        /// Translates a <see cref="SingleNavigationNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(SingleNavigationNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty.Name, node.NavigationSource);
        }

        /// <summary>
        /// Translates a <see cref="SingleEntityFunctionCallNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(SingleEntityFunctionCallNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            String result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="SingleValueFunctionCallNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(SingleValueFunctionCallNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            String result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="CollectionFunctionCallNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String of CollectionFunctionCallNode.</returns>
        public override String Visit(CollectionFunctionCallNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            String result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="EntityCollectionFunctionCallNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String of EntityCollectionFunctionCallNode.</returns>
        public override String Visit(EntityCollectionFunctionCallNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            String result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="SingleValueOpenPropertyAccessNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(SingleValueOpenPropertyAccessNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.Name);
        }

        /// <summary>
        /// Translates an <see cref="CollectionOpenPropertyAccessNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(CollectionOpenPropertyAccessNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.Name);
        }

        /// <summary>
        /// Translates a <see cref="SingleValuePropertyAccessNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(SingleValuePropertyAccessNode node)
        {   
           ExceptionUtils.CheckArgumentNotNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.Property.Name);
        }

        /// <summary>
        /// Translates a <see cref="ParameterAliasNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(ParameterAliasNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return node.Alias;
        }

        /// <summary>
        /// Translates a <see cref="NamedFunctionParameterNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String of NamedFunctionParameterNode.</returns>
        public override string Visit(NamedFunctionParameterNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return String.Concat(node.Name, ExpressionConstants.SymbolEqual, this.TranslateNode(node.Value));
        }

        /// <summary>
        /// Translates a <see cref="NamedFunctionParameterNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String of SearchTermNode.</returns>
        public override string Visit(SearchTermNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            return node.Text;
        }

        /// <summary>
        /// Translates a <see cref="UnaryOperatorNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(UnaryOperatorNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            String result = String.Empty;
            if (node.OperatorKind == UnaryOperatorKind.Negate)
            {
                result = ExpressionConstants.SymbolNegate;
            }

            if (node.OperatorKind == UnaryOperatorKind.Not)
            {
                result = ExpressionConstants.KeywordNot;
            }

            return result = String.Concat(result, ExpressionConstants.SymbolOpenParen, this.TranslateNode(node.Operand), ExpressionConstants.SymbolClosedParen);
        }

        /// <summary>
        /// Main dispatching visit method for translating query-nodes into expressions.
        /// </summary>
        /// <param name="node">The node to visit/translate.</param>
        /// <returns>The LINQ String resulting from visiting the node.</returns>
        internal String TranslateNode(QueryNode node)
        {
            Debug.Assert(node != null, "node != null");
            return node.Accept(this);
        }

        /// <summary>Translates a <see cref="FilterClause"/> into a <see cref="FilterClause"/>.</summary>
        /// <param name="filterClause">The filter clause to translate.</param>
        /// <returns>The translated String.</returns>
        internal String TranslateFilterClause(FilterClause filterClause)
        {
            Debug.Assert(filterClause != null, "filterClause != null");
            return this.TranslateNode(filterClause.Expression);
        }

        /// <summary>Translates a <see cref="OrderByClause"/> into a <see cref="OrderByClause"/>.</summary>
        /// <param name="orderByClause">The orderBy clause to translate.</param>
        /// <returns>The translated String.</returns>
        internal String TranslateOrderByClause(OrderByClause orderByClause)
        {
            Debug.Assert(orderByClause != null, "orderByClause != null");
           
            String expr = this.TranslateNode(orderByClause.Expression);
            if (orderByClause.Direction == OrderByDirection.Descending)
            {
                expr = String.Concat(expr, ' ', ExpressionConstants.KeywordDescending);
            }

            orderByClause = orderByClause.ThenBy;
            if (orderByClause == null)
            {
                return expr;
            }
            else
            {
                return String.Concat(expr, ExpressionConstants.SymbolComma, this.TranslateOrderByClause(orderByClause));
            }   
        }

        /// <summary>Translates a <see cref="SearchClause"/> into a <see cref="SearchClause"/>.</summary>
        /// <param name="searchClause">The search clause to translate.</param>
        /// <returns>The translated String.</returns>
        internal String TranslateSearchClause(SearchClause searchClause)
        {
            Debug.Assert(searchClause != null, "searchClause != null");

            return this.TranslateNode(searchClause.Expression);
        }

        /// <summary>
        /// Add dictinoary to url 
        /// </summary>
        /// <param name="dictionary">Dictionary</param>
        /// <returns>the string format of iDictionary</returns>
        internal String TranslateParameterAliasNodes(IDictionary<string, SingleValueNode> dictionary)
        {
            String result = null; 
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, SingleValueNode> keyValuePair in dictionary)
                {
                    if (keyValuePair.Value != null)
                    {
                        String tmp = this.TranslateNode(keyValuePair.Value);
                        result = string.IsNullOrEmpty(tmp) ? result : string.Concat(result, String.IsNullOrEmpty(result) ? null : ExpressionConstants.SymbolQueryConcatenate, keyValuePair.Key, ExpressionConstants.SymbolEqual, tmp);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Helper for translating an access to a metadata-defined property or navigation.
        /// </summary>
        /// <param name="sourceNode">The source of the property access.</param>
        /// <param name="edmPropertyName">The structural or navigation property being accessed.</param>
        /// <param name="navigationSource">The navigation source of the result, required for navigations.</param>
        /// <returns>The translated String.</returns>
        private String TranslatePropertyAccess(QueryNode sourceNode, String edmPropertyName, IEdmNavigationSource navigationSource = null)
        {
            ExceptionUtils.CheckArgumentNotNull(sourceNode, "sourceNode");
            ExceptionUtils.CheckArgumentNotNull(edmPropertyName, "edmPropertyName");

            String source = this.TranslateNode(sourceNode);
            return String.IsNullOrEmpty(source) ? edmPropertyName : string.Concat(source, ExpressionConstants.SymbolForwardSlash, edmPropertyName);
        }

        /// <summary>
        /// Translates a function call into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="argumentNodes">The argument nodes.</param>
        /// <returns>
        /// The translated String.
        /// </returns>
        private String TranslateFunctionCall(string functionName, IEnumerable<QueryNode> argumentNodes)
        {
            ExceptionUtils.CheckArgumentNotNull(functionName, "functionName");
            
            String result = String.Empty;
            foreach (QueryNode queryNode in argumentNodes)
            {
                result = String.Concat(result, String.IsNullOrEmpty(result) ? null : ExpressionConstants.SymbolComma, this.TranslateNode(queryNode));
            }

            return String.Concat(functionName, ExpressionConstants.SymbolOpenParen, result, ExpressionConstants.SymbolClosedParen);
        }

        /// <summary>
        /// Build BinaryOperatorNode to uri 
        /// </summary>
        /// <param name="operatorKind">the kind of the BinaryOperatorNode</param>
        /// <returns>String format of the operator</returns>
        private String BinaryOperatorNodeToString(BinaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case BinaryOperatorKind.Equal:
                    return ExpressionConstants.KeywordEqual;
                case BinaryOperatorKind.NotEqual:
                    return ExpressionConstants.KeywordNotEqual;
                case BinaryOperatorKind.GreaterThan:
                    return ExpressionConstants.KeywordGreaterThan;
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return ExpressionConstants.KeywordGreaterThanOrEqual;
                case BinaryOperatorKind.LessThan:
                    return ExpressionConstants.KeywordLessThan;
                case BinaryOperatorKind.LessThanOrEqual:
                    return ExpressionConstants.KeywordLessThanOrEqual;
                case BinaryOperatorKind.And:
                    return ExpressionConstants.KeywordAnd;
                case BinaryOperatorKind.Or:
                    return ExpressionConstants.KeywordOr;
                case BinaryOperatorKind.Add:
                    return ExpressionConstants.KeywordAdd;
                case BinaryOperatorKind.Subtract:
                    return ExpressionConstants.KeywordSub;
                case BinaryOperatorKind.Multiply:
                    return ExpressionConstants.KeywordMultiply;
                case BinaryOperatorKind.Divide:
                    return ExpressionConstants.KeywordDivide;
                case BinaryOperatorKind.Modulo:
                    return ExpressionConstants.KeywordModulo;
                default:
                    return null;
            }
        }
    }
}
