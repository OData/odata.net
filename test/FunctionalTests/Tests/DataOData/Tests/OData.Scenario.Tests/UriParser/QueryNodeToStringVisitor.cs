//---------------------------------------------------------------------
// <copyright file="QueryNodeToStringVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    /// <summary>
    /// Serializes the QueryNode to string.
    /// </summary>
    public static class QueryNodeToStringVisitor
    {
        /// <summary>
        /// TabHelper used to control tabs
        /// </summary>
        private static TabHelper tabHelper = new TabHelper();

        #region Filter and OrderBy Object Model
        public static string ToString(QueryNode node)
        {
            if (node == null) return String.Empty;

            switch (node.Kind)
            {
                case QueryNodeKind.Any:
                    return ToString((AnyNode)node);
                case QueryNodeKind.All:
                    return ToString((AllNode)node);
                case QueryNodeKind.NonResourceRangeVariableReference:
                    return ToString((NonResourceRangeVariableReferenceNode)node);
                case QueryNodeKind.Convert:
                    return ToString((ConvertNode)node);
                case QueryNodeKind.BinaryOperator:
                    return ToString((BinaryOperatorNode)node);
                case QueryNodeKind.UnaryOperator:
                    return ToString((UnaryOperatorNode)node);
                case QueryNodeKind.SingleValueFunctionCall:
                    return ToString((SingleValueFunctionCallNode)node);
                case QueryNodeKind.SingleValuePropertyAccess:
                    return ToString((SingleValuePropertyAccessNode)node);
                case QueryNodeKind.CollectionPropertyAccess:
                    return ToString((CollectionPropertyAccessNode)node);
                case QueryNodeKind.CollectionOpenPropertyAccess:
                    return ToString((CollectionOpenPropertyAccessNode)node);
                case QueryNodeKind.SingleResourceCast:
                    return ToString((SingleResourceCastNode)node);
                case QueryNodeKind.CollectionResourceCast:
                    return ToString((CollectionResourceCastNode)node);
                case QueryNodeKind.ResourceRangeVariableReference:
                    return ToString((ResourceRangeVariableReferenceNode)node);
                case QueryNodeKind.Constant:
                    return ToString((ConstantNode)node);
                case QueryNodeKind.CollectionNavigationNode:
                    return ToString((CollectionNavigationNode)node);
                case QueryNodeKind.SingleNavigationNode:
                    return ToString((SingleNavigationNode)node);
                case QueryNodeKind.SingleResourceFunctionCall:
                    return ToString((SingleResourceFunctionCallNode)node);
                case QueryNodeKind.NamedFunctionParameter:
                    return ToString((NamedFunctionParameterNode)node);
                case QueryNodeKind.ParameterAlias:
                    return ToString((ParameterAliasNode)node);
                case QueryNodeKind.SearchTerm:
                    return ToString((SearchTermNode)node);
                case QueryNodeKind.SingleComplexNode:
                    return ToString((SingleComplexNode)node);
                case QueryNodeKind.CollectionComplexNode:
                    return ToString((CollectionComplexNode)node);
                case QueryNodeKind.Count:
                    return ToString((CountNode)node);
                default:
                    throw new NotSupportedException(String.Format("Node kind not yet supported: {0}", node.Kind.ToString()));
            }
        }

        /// <summary>
        /// Writes convert query node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(ConvertNode node)
        {
            return tabHelper.Prefix + "ConvertNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "TypeReference = " + node.TypeReference +
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
        }

        /// <summary>
        /// Writes ResourceRangeVariableReferenceNode to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(ResourceRangeVariableReferenceNode node)
        {
            return tabHelper.Prefix + "ResourceRangeVariableReferenceNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Name = " + node.Name +
                    tabHelper.Prefix + "NavigationSource = " + node.NavigationSource.Name +
                    tabHelper.Prefix + "TypeReference = " + node.TypeReference +
                    tabHelper.Prefix + "Range Variable = " + node.RangeVariable
            );
        }

        /// <summary>
        /// Writes entity range variable to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(ResourceRangeVariable node)
        {
            return tabHelper.Prefix + "ResourceRangeVariable" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Name = " + node.Name +
                    tabHelper.Prefix + "NavigationSource = " + node.NavigationSource.Name +
                    tabHelper.Prefix + "TypeReference = " + node.TypeReference
            );
        }

        /// <summary>
        /// Writes non entity range variable reference node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(NonResourceRangeVariableReferenceNode node)
        {
            return tabHelper.Prefix + "NonentityRangeVariableReferenceNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Name = " + node.Name +
                    tabHelper.Prefix + "Range Variable = " + ToString(node.RangeVariable) +
                    tabHelper.Prefix + "TypeReference = " + node.TypeReference
                );
        }

        /// <summary>
        /// Compares non entity parameter query nodes query nodes.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(NonResourceRangeVariable node)
        {
            return tabHelper.Prefix + "NonResourceRangeVariable" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Name = " + node.Name +
                    tabHelper.Prefix + "TypeReference = " + node.TypeReference
                );
        }

        /// <summary>
        /// Writes single value property access node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SingleValuePropertyAccessNode node)
        {
            return tabHelper.Prefix + "SingleValuePropertyAccessNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Property = " + node.Property.Name +
                    tabHelper.Prefix + "TypeReference = " + node.TypeReference +
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
        }

        /// <summary>
        /// Writes collection property access node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(CollectionPropertyAccessNode node)
        {
            return tabHelper.Prefix + "CollectionPropertyAccessNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Property = " + node.Property.Name +
                    tabHelper.Prefix + "ItemType = " + node.ItemType +
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
        }

        /// <summary>
        /// Writes collection property access node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(CollectionOpenPropertyAccessNode node)
        {
            return tabHelper.Prefix + "CollectionOpenPropertyAccessNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Name = " + node.Name +
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
        }

        /// <summary>
        /// Writes filter clause to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        public static string ToString(FilterClause node)
        {
            if (node != null)
            {
                string text = tabHelper.Prefix + "FilterQueryOption" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "ItemType = " + node.ItemType +
                        tabHelper.Prefix + "Parameter = " + ToStringParameter(node.RangeVariable) +
                        tabHelper.Prefix + "Expression = " + ToString(node.Expression)
                    );

                return text;
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes unary operator node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(UnaryOperatorNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + node.OperatorKind + "(" +
                        tabHelper.Indent(() => ToString(node.Operand)) +
                        tabHelper.Prefix + ")";
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes binary operator node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(BinaryOperatorNode node)
        {
            if (node != null)
            {
                return tabHelper.Indent(() => ToString(node.Left)) +
                    tabHelper.Prefix + node.OperatorKind +
                    tabHelper.Indent(() => (ToString(node.Right)));
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes constant node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(ConstantNode node)
        {
            return tabHelper.Prefix + String.Format("{0}(Type: {1})", node.Value, node.TypeReference);
        }

        /// <summary>
        /// Writes any node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(AnyNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "AnyNode" +
                    tabHelper.Indent(() =>
                    {
                        string text = tabHelper.Prefix + "TypeReference = " + node.TypeReference +
                                tabHelper.Prefix + "Body = " + ToString(node.Body) +
                                tabHelper.Prefix + "Source = " + ToString(node.Source) +
                                tabHelper.Prefix + "Parameters = ";

                        for (int i = 0; i < node.RangeVariables.Count(); ++i)
                        {
                            text += ToStringParameter(node.RangeVariables.ElementAt(i));
                        }

                        return text;
                    });
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes single entity cast node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SingleResourceCastNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "SingleResourceCastNode" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "Type Reference = " + node.TypeReference +
                        tabHelper.Prefix + "NavigationSource = " + node.NavigationSource.Name +
                        tabHelper.Prefix + "Entity Type Reference = " + node.StructuredTypeReference +
                        tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes all node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(AllNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "AllNode" +
                    tabHelper.Indent(() =>
                    {
                        string text = tabHelper.Prefix + "Type Reference = " + node.TypeReference +
                                tabHelper.Prefix + "Body = " + ToString(node.Body) +
                                tabHelper.Prefix + "Source = " + ToString(node.Source) +
                                tabHelper.Prefix + "Parameters = ";

                        for (int i = 0; i < node.RangeVariables.Count(); ++i)
                        {
                            text += ToStringParameter(node.RangeVariables.ElementAt(i));
                        }

                        return text;
                    });
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes entity collection cast node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(CollectionResourceCastNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "CollectionResourceCastNode" +
                 tabHelper.Indent(() =>
                    tabHelper.Prefix + "Entity Item Type = " + node.ItemStructuredType +
                    tabHelper.Prefix + "NavigationSource = " + node.NavigationSource.Name +
                    tabHelper.Prefix + "Item Type = " + node.ItemType +
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes CountNode.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(CountNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "CountNode" +
                 tabHelper.Indent(() =>
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes single value function call node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SingleValueFunctionCallNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "SingleValueFunctionCallNode" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "Name = " + node.Name +
                        tabHelper.Prefix + "Return Type = " + node.TypeReference +
                        tabHelper.Prefix + "Function = " + ToString(node.Functions) +
                        ArgumentsToString(node.Parameters)
                    );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes single value function call node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SearchTermNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "SearchTermNode" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "Text = " + node.Text
                    );
            }

            return String.Empty;
        }

        /// <summary>
        /// Write order by clause to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        public static string ToString(OrderByClause node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "OrderByClause" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "Item Type = " + node.ItemType +
                        tabHelper.Prefix + "Direction = " + node.Direction +
                        tabHelper.Prefix + "Range Variable = " + ToStringParameter(node.RangeVariable) +
                        tabHelper.Prefix + "Expression = " + ToString(node.Expression) +
                        tabHelper.Prefix + "Then By = " + ToString(node.ThenBy)
                    );
            }

            return String.Empty;
        }

        /// <summary>
        /// Write single navigation node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SingleNavigationNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "SingleNavigationNode" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "NavigationSource = " + node.NavigationSource.Name +
                        tabHelper.Prefix + "Type Reference = " + node.EntityTypeReference +
                        tabHelper.Prefix + "Property = " + node.NavigationProperty.Name +
                        tabHelper.Prefix + "Multiplicity = " + node.TargetMultiplicity +
                        tabHelper.Prefix + "Source = " + ToString(node.Source)
                    );
            }

            return String.Empty;
        }

        /// <summary>
        /// Write collection navigation node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(CollectionNavigationNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "CollectionNavigationNode" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "ItemType = " + node.ItemType +
                        tabHelper.Prefix + "Entity Item Type = " + node.EntityItemType +
                        tabHelper.Prefix + "NavigationSource = " + node.NavigationSource.Name +
                        tabHelper.Prefix + "Multiplicity = " + node.TargetMultiplicity +
                        tabHelper.Prefix + "Navigation Property = " + node.NavigationProperty.Name +
                        tabHelper.Prefix + "Source = " + ToString(node.Source)
                    );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes single entity function call node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SingleResourceFunctionCallNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "SingleResourceFunctionCallNode" +
                            tabHelper.Indent(() =>
                                tabHelper.Prefix + "NavigationSource = " + node.NavigationSource.Name +
                                tabHelper.Prefix + "Type Reference = " + node.TypeReference +
                                tabHelper.Prefix + "Name = " + node.Name +
                                tabHelper.Prefix + "Function = " + ToString(node.Functions) +
                                ArgumentsToString(node.Parameters)
                        );
            }

            return String.Empty;

        }

        /// <summary>
        /// Writes single entity function call node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(NamedFunctionParameterNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "NamedFunctionParameterNode" +
                        tabHelper.Indent(() =>
                            tabHelper.Prefix + "Parameter Name = " + node.Name +
                            tabHelper.Prefix + "Value = " + ToString(node.Value)
                        );
            }

            return String.Empty;

        }

        /// <summary>
        /// Writes single entity function call node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(ParameterAliasNode node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "ParameterAliasNode" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "Parameter Name = " + node.Alias +
                        tabHelper.Prefix + "Type = " + node.TypeReference
                    );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes Range Variable node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToStringParameter(RangeVariable node)
        {
            if (node is ResourceRangeVariable)
                return ToString((ResourceRangeVariable)node);
            if (node is NonResourceRangeVariable)
                return ToString((NonResourceRangeVariable)node);
            return String.Empty;
        }
        #endregion

        #region Select, Expand, and Path Object Model

        /// <summary>
        /// Writes a test case for baselining.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <param name="originalSelect">Original $select string.</param>
        /// <param name="originalExpand">Original $expand string.</param>
        /// <returns>String representation of the output from the URI Parser.</returns>
        internal static string GetTestCaseAndResultString(SelectExpandClause node, string originalSelect, string originalExpand)
        {
            return "$select = " + originalSelect + "\n$expand = " + originalExpand + "\n\n" + ToString(node);
        }

        /// <summary>
        /// Writes a test case for baselining.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <param name="originalOrderBy">Original $filter string.</param>
        /// <returns>String representation of the output from the URI Parser.</returns>
        internal static string GetTestCaseAndResultString(FilterClause node, string originalOrderBy)
        {
            return "$filter = " + originalOrderBy + "\n\n" + ToString(node);
        }

        /// <summary>
        /// Writes a test case for baselining.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <param name="originalOrderBy">Original $orderby string.</param>
        /// <returns>String representation of the output from the URI Parser.</returns>
        internal static string GetTestCaseAndResultString(OrderByClause node, string originalOrderBy)
        {
            return "$orderby = " + originalOrderBy + "\n\n" + ToString(node);
        }

        /// <summary>
        /// Writes a test case for baselining.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <param name="originalSearch">Original $search string.</param>
        /// <returns>String representation of the output from the URI Parser.</returns>
        internal static string GetTestCaseAndResultString(SearchClause node, string originalSearch)
        {
            return "$search = " + originalSearch + "\n\n" + ToString(node);
        }

        /// <summary>
        /// Writes Select/Expand Clause node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <param name="originalSelect"></param>
        /// <param name="originalExpand"></param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SelectExpandClause node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "SelectExpandQueryOption" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "AllSelected = " + node.AllSelected + tabHelper.Prefix + "SelectedItems" + ToString(node.SelectedItems)
                     );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes out the list of selected items. Increase tab scope.
        /// </summary>
        /// <param name="items">Items to turn into a string.</param>
        /// <returns>String representation of the items.</returns>
        private static string ToString(IEnumerable<SelectItem> items)
        {
            return tabHelper.Indent(() =>
            {
                string selectedItemStr = "";
                foreach (SelectItem item in items)
                {
                    selectedItemStr += tabHelper.Prefix + ToString(item);
                }

                if (selectedItemStr == "")
                {
                    selectedItemStr += "(Empty List)";
                }
                return selectedItemStr;
            });
        }

        /// <summary>
        /// Writes Selection Item node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SelectItem node)
        {
            if (node != null)
            {
                if (node is NamespaceQualifiedWildcardSelectItem)
                {
                    return ToString(node as NamespaceQualifiedWildcardSelectItem);
                }
                else if (node is PathSelectItem)
                {
                    return ToString(node as PathSelectItem);
                }
                else if (node is ExpandedNavigationSelectItem)
                {
                    return ToString(node as ExpandedNavigationSelectItem);
                }
                else if (node is WildcardSelectItem)
                {
                    return ToString(node as WildcardSelectItem);
                }
            }

            throw new NotSupportedException(String.Format("Node kind not yet supported: {0}", node.ToString()));
        }

        /// <summary>
        /// Writes Selection Item of type WildcardSelectItem node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(WildcardSelectItem node)
        {
            return String.Format("(Wildcard)");
        }

        /// <summary>
        /// Writes Selection Item of type PathSelectItem node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(PathSelectItem node)
        {
            return ToSingleLineString(node.SelectedPath);
        }

        /// <summary>
        /// Writes Selection Item of type ContainerQualifiedWildcardSelectItem node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(NamespaceQualifiedWildcardSelectItem node)
        {
            return String.Format("(ContainerQualifiedWildcard)");
        }

        /// <summary>
        /// Writes an ODataPath to string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToString(ODataPath path)
        {
            // TODO: Consider making this print 1 line per segment
            string pathname = path.Aggregate("Path[", (current, segment) => current + (ToString(segment) + "/"));
            return pathname.Substring(0, pathname.Length - 1) + "]";
        }

        /// <summary>
        /// Writes an ODataPath to string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToString(SearchClause node)
        {
            if (node != null)
            {
                return tabHelper.Prefix + "SearchQueryOption" +
                    tabHelper.Indent(() =>
                        tabHelper.Prefix + "Expression = " + ToString(node.Expression)
                     );
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes an ODataPath to string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToSingleLineString(ODataPath path)
        {
            string pathname = path.Aggregate("Path[", (current, segment) => current + (ToString(segment) + "/"));
            return pathname.Substring(0, pathname.Length - 1) + "]";
        }

        /// <summary>
        /// Writes a Segment to string
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        private static string ToString(ODataPathSegment segment)
        {
            SegmentToStringVisitor vistor = new SegmentToStringVisitor();
            return segment.TranslateWith(vistor);
        }

        /// <summary>
        /// Writes ExpandItem node to string
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(ExpandedNavigationSelectItem node)
        {
            if (node != null)
            {
                var expandItemString = "Expanded Navigation Property" +
                    tabHelper.Indent(() =>
                    {
                        string text = tabHelper.Prefix + ToString(node.PathToNavigationProperty);
                        // Include when we add V4 support
                        ////if (node.TopOption != null) expandItemString += tabHelper.Prefix + string.Format("TopOption({0})", node.TopOption);
                        ////if (node.SkipOption != null) expandItemString += tabHelper.Prefix + string.Format("SkipOption({0})", node.SkipOption);
                        ////if (node.InlineCountOption != null) expandItemString += tabHelper.Prefix + string.Format("InlineCountOption({0})", node.InlineCountOption);
                        if (node.FilterOption != null) text += ToString(node.FilterOption);
                        if (node.OrderByOption != null) text += ToString(node.OrderByOption);
                        if (node.SearchOption != null) text += ToString(node.SearchOption);
                        if (node.SelectAndExpand != null) text += ToString(node.SelectAndExpand);

                        return text;
                    });

                return expandItemString;
            }

            return String.Empty;
        }

        /// <summary>
        /// Writes single complex property node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(SingleComplexNode node)
        {
            return tabHelper.Prefix + "SingleComplexNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Property = " + node.Property.Name +
                    tabHelper.Prefix + "TypeReference = " + node.TypeReference +
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
        }

        /// <summary>
        /// Writes collection complex property node to string.
        /// </summary>
        /// <param name="node">Node to write to string</param>
        /// <returns>String representation of node.</returns>
        private static string ToString(CollectionComplexNode node)
        {
            return tabHelper.Prefix + "CollectionComplexNode" +
                tabHelper.Indent(() =>
                    tabHelper.Prefix + "Property = " + node.Property.Name +
                    tabHelper.Prefix + "ItemType = " + node.ItemType +
                    tabHelper.Prefix + "Source = " + ToString(node.Source)
                );
        }

        #endregion

        /// <summary>
        /// Returns a string representing arguments at current tab level.
        /// </summary>
        /// <param name="args">Arguments to write.</param>
        /// <returns>Returns string representing arguments.</returns>
        private static string ArgumentsToString(IEnumerable<QueryNode> args)
        {
            if (args != null)
            {
                string text = tabHelper.Prefix + "Arguments = ";
                return tabHelper.Indent(() => args.Aggregate(text, (current, arg) => current + ToString(arg)));
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns a string representing the list of function imports
        /// </summary>
        /// <param name="functions">Function imports to write to string</param>
        /// <returns>Returns a string representing the list of function imports.</returns>
        private static string ToString(IEnumerable<IEdmOperationImport> functions)
        {
            return tabHelper.Indent(() =>
            {
                string text = string.Empty;
                foreach (var func in functions)
                {
                    text += tabHelper.Prefix + func.Operation.ReturnType.Definition.TypeKind + " " + func.Name + "(";
                    text = func.Operation.Parameters.Aggregate(text, (current, parameter) => current + (parameter.Type + ",")) + ")";
                }

                return text;
            });
        }

        /// <summary>
        /// Returns a string representing the list of function imports
        /// </summary>
        /// <param name="functions">Function imports to write to string</param>
        /// <returns>Returns a string representing the list of function imports.</returns>
        private static string ToString(IEnumerable<IEdmFunction> functions)
        {
            return tabHelper.Indent(() =>
            {
                string text = string.Empty;
                foreach (var func in functions)
                {
                    text += tabHelper.Prefix + func.ReturnType.Definition.TypeKind + " " + func.Name + "(";
                    text = func.Parameters.Aggregate(text, (current, parameter) => current + (parameter.Type + ",")) + ")";
                }
                return text;
            });
        }

        private class TabHelper
        {
            private const string NewLineStr = "\n";
            private const string TabStr = "\t";

            private string tabs;

            public string Prefix
            {
                get { return NewLineStr + this.tabs; }
            }

            public string Indent(Func<string> act)
            {
                string save = this.tabs;
                this.tabs += TabStr;
                string result = act();
                this.tabs = save;

                return result;
            }
        }
    }
}
