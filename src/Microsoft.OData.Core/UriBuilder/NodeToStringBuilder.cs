//---------------------------------------------------------------------
// <copyright file="NodeToStringBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;
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
        /// whether translating search options or others
        /// </summary>
        private bool searchFlag;

        /// <summary>
        /// Translates a <see cref="AllNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(AllNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");

            String result = String.Concat(this.TranslateNode(node.Source), ExpressionConstants.SymbolForwardSlash, ExpressionConstants.KeywordAll, ExpressionConstants.SymbolOpenParen, node.CurrentRangeVariable.Name, ":", this.TranslateNode(node.Body), ExpressionConstants.SymbolClosedParen);
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

            if (node.CurrentRangeVariable == null && node.Body.Kind == QueryNodeKind.Constant)
            {
                return String.Concat(this.TranslateNode(node.Source), ExpressionConstants.SymbolForwardSlash, ExpressionConstants.KeywordAny, ExpressionConstants.SymbolOpenParen, ExpressionConstants.SymbolClosedParen);
            }
            else
            {
                return String.Concat(this.TranslateNode(node.Source), ExpressionConstants.SymbolForwardSlash, ExpressionConstants.KeywordAny, ExpressionConstants.SymbolOpenParen, node.CurrentRangeVariable.Name, ":", this.TranslateNode(node.Body), ExpressionConstants.SymbolClosedParen);
            }
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
            if (node.Left.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)node.Left).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                left = String.Concat(ExpressionConstants.SymbolOpenParen, left, ExpressionConstants.SymbolClosedParen);
            }

            var right = this.TranslateNode(node.Right);
            if (node.Right.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)node.Right).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                right = String.Concat(ExpressionConstants.SymbolOpenParen, right, ExpressionConstants.SymbolClosedParen);
            }

            return String.Concat(left, ' ', this.BinaryOperatorNodeToString(node.OperatorKind), ' ', right);
        }

        /// <summary>
        /// Translates a <see cref="CollectionCountNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(CollectionCountNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");

            String source = this.TranslateNode(node.Source);
            return string.Concat(source, ExpressionConstants.SymbolForwardSlash, UriQueryConstants.CountSegment);
        }

        /// <summary>
        /// Translates a <see cref="CollectionNavigationNode"/> into a corresponding <see cref="String"/>.
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
            return node.Name;
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

            if (false == IsValidSearchWord(node.Text))
            {
                return String.Concat("\"", node.Text, "\"");
            }
            else
            {
                return node.Text;
            }
        }

        /// <summary>
        /// Translates a <see cref="UnaryOperatorNode"/> into a corresponding <see cref="String"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated String.</returns>
        public override String Visit(UnaryOperatorNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");
            String result = null;
            if (node.OperatorKind == UnaryOperatorKind.Negate)
            {
                result = ExpressionConstants.SymbolNegate;
            }

            // if current translated node is SearchNode, the UnaryOperator should return NOT, or return not
            if (node.OperatorKind == UnaryOperatorKind.Not)
            {
                if (searchFlag)
                {
                    result = ExpressionConstants.SearchKeywordNot;
                }
                else
                {
                    result = ExpressionConstants.KeywordNot;
                }
            }

            if (node.Operand.Kind == QueryNodeKind.Constant || node.Operand.Kind == QueryNodeKind.SearchTerm)
            {
                return String.Concat(result, ' ', this.TranslateNode(node.Operand));
            }
            else
            {
                return String.Concat(result, ExpressionConstants.SymbolOpenParen, this.TranslateNode(node.Operand), ExpressionConstants.SymbolClosedParen);
            }
        }

        /// <summary>Translates a <see cref="LevelsClause"/> into a string.</summary>
        /// <param name="levelsClause">The levels clause to translate.</param>
        /// <returns>The translated String.</returns>
        internal static string TranslateLevelsClause(LevelsClause levelsClause)
        {
            Debug.Assert(levelsClause != null, "levelsClause != null");
            string levelsStr = levelsClause.IsMaxLevel
                ? ExpressionConstants.KeywordMax
                : levelsClause.Level.ToString(CultureInfo.InvariantCulture);
            return levelsStr;
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
            searchFlag = true;
            string searchStr = this.TranslateNode(searchClause.Expression);
            searchFlag = false;
            return searchStr;
        }

        /// <summary>
        /// Add dictinoary to url and each alias value will be URL encoded.
        /// </summary>
        /// <param name="dictionary">Dictionary</param>
        /// <returns>The url query string of dictionary's key value pairs (URL encoded)</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Values passed to this method are property names and not literals.")]
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
                        result = string.IsNullOrEmpty(tmp) ? result : string.Concat(result, String.IsNullOrEmpty(result) ? null : ExpressionConstants.SymbolQueryConcatenate, keyValuePair.Key, ExpressionConstants.SymbolEqual, Uri.EscapeDataString(tmp));
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
                case BinaryOperatorKind.Has:
                    return ExpressionConstants.KeywordHas;
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

                // if current translated node is SearchNode, the BinaryOperator should return AND, OR; or return and, or.
                case BinaryOperatorKind.And:
                    if (searchFlag)
                    {
                        return ExpressionConstants.SearchKeywordAnd;
                    }

                    return ExpressionConstants.KeywordAnd;
                case BinaryOperatorKind.Or:
                    if (searchFlag)
                    {
                        return ExpressionConstants.SearchKeywordOr;
                    }

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

        /// <summary>
        /// Get the priority of BinaryOperatorNode
        /// This priority table is from http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part2-url-conventions.html (5.1.1.9 Operator Precedence )
        /// </summary>
        /// <param name="operatorKind">binary operator </param>
        /// <returns>the priority value of the binary operator</returns>
        private static int TranslateBinaryOperatorPriority(BinaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case BinaryOperatorKind.Or:
                    return 1;
                case BinaryOperatorKind.And:
                    return 2;
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.LessThanOrEqual:
                    return 3;
                case BinaryOperatorKind.Add:
                case BinaryOperatorKind.Subtract:
                    return 4;
                case BinaryOperatorKind.Divide:
                case BinaryOperatorKind.Multiply:
                case BinaryOperatorKind.Modulo:
                    return 5;
                case BinaryOperatorKind.Has:
                    return 6;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Judge a string text is a valid SearchWord or not ?
        /// </summary>
        /// <param name="text">string text to be judged</param>
        /// <returns>if the string is a valid SearchWord, return true, or return false.</returns>
        private static bool IsValidSearchWord(string text)
        {
            Match match = SearchLexer.InvalidWordPattern.Match(text);
            if (match.Success || String.Equals(text, "AND") || String.Equals(text, "OR") || String.Equals(text, "NOT"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
