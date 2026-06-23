//---------------------------------------------------------------------
// <copyright file="SemanticAstStructuralEqualityComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser.Aggregation;

    /// <summary>
    /// Structural equality helpers for SemanticAst objects.
    /// </summary>
    internal static class SemanticAstStructuralEqualityComparer
    {
        public static bool AreEqual(FilterClause left, FilterClause right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                AreEqual(left.Expression, right.Expression) &&
                AreEqual(left.RangeVariable, right.RangeVariable));
        }

        public static int GetHashCode(FilterClause clause)
        {
            if (clause == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(GetHashCode(clause.Expression));
            hashCode.Add(GetHashCode(clause.RangeVariable));
            return hashCode.ToHashCode();
        }

        public static bool AreEqual(OrderByClause left, OrderByClause right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                left.Direction == right.Direction &&
                AreEqual(left.Expression, right.Expression) &&
                AreEqual(left.RangeVariable, right.RangeVariable) &&
                AreEqual(left.ThenBy, right.ThenBy));
        }

        public static int GetHashCode(OrderByClause clause)
        {
            if (clause == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(clause.Direction);
            hashCode.Add(GetHashCode(clause.Expression));
            hashCode.Add(GetHashCode(clause.RangeVariable));
            hashCode.Add(GetHashCode(clause.ThenBy));
            return hashCode.ToHashCode();
        }

        public static bool AreEqual(SearchClause left, SearchClause right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                AreEqual(left.Expression, right.Expression));
        }

        public static int GetHashCode(SearchClause clause)
        {
            return clause == null ? 0 : GetHashCode(clause.Expression);
        }

        public static bool AreEqual(SelectExpandClause left, SelectExpandClause right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                left.AllSelected == right.AllSelected &&
                SequenceEqual(left.SelectedItems, right.SelectedItems, AreEqual));
        }

        public static int GetHashCode(SelectExpandClause clause)
        {
            if (clause == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(clause.AllSelected);
            AddSequenceHashCode(ref hashCode, clause.SelectedItems, GetHashCode);
            return hashCode.ToHashCode();
        }

        public static bool AreEqual(RangeVariable left, RangeVariable right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null || left.Kind != right.Kind || left.Name != right.Name || !AreEqual(left.TypeReference, right.TypeReference))
            {
                return false;
            }

            ResourceRangeVariable leftResource = left as ResourceRangeVariable;
            ResourceRangeVariable rightResource = right as ResourceRangeVariable;
            if (leftResource != null && rightResource != null)
            {
                return Equals(leftResource.NavigationSource, rightResource.NavigationSource) &&
                    AreEqual(leftResource.StructuredTypeReference, rightResource.StructuredTypeReference) &&
                    AreEqual(leftResource.CollectionResourceNode, rightResource.CollectionResourceNode);
            }

            NonResourceRangeVariable leftNonResource = left as NonResourceRangeVariable;
            NonResourceRangeVariable rightNonResource = right as NonResourceRangeVariable;
            if (leftNonResource != null && rightNonResource != null)
            {
                return AreEqual(leftNonResource.CollectionNode, rightNonResource.CollectionNode);
            }

            return false;
        }

        public static int GetHashCode(RangeVariable rangeVariable)
        {
            if (rangeVariable == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(rangeVariable.Kind);
            hashCode.Add(rangeVariable.Name, StringComparer.Ordinal);
            hashCode.Add(GetHashCode(rangeVariable.TypeReference));

            ResourceRangeVariable resourceRangeVariable = rangeVariable as ResourceRangeVariable;
            if (resourceRangeVariable != null)
            {
                hashCode.Add(resourceRangeVariable.NavigationSource);
                hashCode.Add(GetHashCode(resourceRangeVariable.StructuredTypeReference));
                hashCode.Add(GetHashCode(resourceRangeVariable.CollectionResourceNode));
            }

            NonResourceRangeVariable nonResourceRangeVariable = rangeVariable as NonResourceRangeVariable;
            if (nonResourceRangeVariable != null)
            {
                hashCode.Add(GetHashCode(nonResourceRangeVariable.CollectionNode));
            }

            return hashCode.ToHashCode();
        }

        public static bool AreEqual(QueryNode left, QueryNode right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null || left.InternalKind != right.InternalKind)
            {
                return false;
            }

            return string.Equals(GetNodeText(left), GetNodeText(right), StringComparison.Ordinal) &&
                HaveEquivalentMetadata(left, right);
        }

        public static int GetHashCode(QueryNode node)
        {
            if (node == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(node.InternalKind);
            hashCode.Add(GetNodeText(node), StringComparer.Ordinal);
            AddNodeMetadataHashCode(ref hashCode, node);
            return hashCode.ToHashCode();
        }

        private static bool AreEqual(ComputeClause left, ComputeClause right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                SequenceEqual(left.ComputedItems, right.ComputedItems, AreEqual));
        }

        private static int GetHashCode(ComputeClause clause)
        {
            if (clause == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            AddSequenceHashCode(ref hashCode, clause.ComputedItems, GetHashCode);
            return hashCode.ToHashCode();
        }

        private static bool AreEqual(ComputeExpression left, ComputeExpression right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                left.Alias == right.Alias &&
                AreEqual(left.TypeReference, right.TypeReference) &&
                AreEqual(left.Expression, right.Expression));
        }

        private static int GetHashCode(ComputeExpression expression)
        {
            if (expression == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(expression.Alias, StringComparer.Ordinal);
            hashCode.Add(GetHashCode(expression.TypeReference));
            hashCode.Add(GetHashCode(expression.Expression));
            return hashCode.ToHashCode();
        }

        private static bool AreEqual(LevelsClause left, LevelsClause right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                left.IsMaxLevel == right.IsMaxLevel &&
                left.Level == right.Level);
        }

        private static int GetHashCode(LevelsClause clause)
        {
            if (clause == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(clause.IsMaxLevel);
            hashCode.Add(clause.Level);
            return hashCode.ToHashCode();
        }

        private static bool AreEqual(ApplyClause left, ApplyClause right)
        {
            return ReferenceEquals(left, right) ||
                (left != null &&
                right != null &&
                string.Equals(GetApplyClauseText(left), GetApplyClauseText(right), StringComparison.Ordinal));
        }

        private static int GetHashCode(ApplyClause clause)
        {
            return clause == null ? 0 : StringComparer.Ordinal.GetHashCode(GetApplyClauseText(clause));
        }

        private static bool AreEqual(SelectItem left, SelectItem right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null || left.GetType() != right.GetType())
            {
                return false;
            }

            PathSelectItem leftPathSelectItem = left as PathSelectItem;
            if (leftPathSelectItem != null)
            {
                PathSelectItem rightPathSelectItem = (PathSelectItem)right;
                return AreEqual(leftPathSelectItem.SelectedPath, rightPathSelectItem.SelectedPath) &&
                    Equals(leftPathSelectItem.NavigationSource, rightPathSelectItem.NavigationSource) &&
                    AreEqual(leftPathSelectItem.SelectAndExpand, rightPathSelectItem.SelectAndExpand) &&
                    AreEqual(leftPathSelectItem.FilterOption, rightPathSelectItem.FilterOption) &&
                    AreEqual(leftPathSelectItem.OrderByOption, rightPathSelectItem.OrderByOption) &&
                    leftPathSelectItem.TopOption == rightPathSelectItem.TopOption &&
                    leftPathSelectItem.SkipOption == rightPathSelectItem.SkipOption &&
                    leftPathSelectItem.CountOption == rightPathSelectItem.CountOption &&
                    AreEqual(leftPathSelectItem.SearchOption, rightPathSelectItem.SearchOption) &&
                    AreEqual(leftPathSelectItem.ComputeOption, rightPathSelectItem.ComputeOption);
            }

            ExpandedNavigationSelectItem leftExpandedNavigation = left as ExpandedNavigationSelectItem;
            if (leftExpandedNavigation != null)
            {
                ExpandedNavigationSelectItem rightExpandedNavigation = (ExpandedNavigationSelectItem)right;
                return AreEqualExpandedReference(leftExpandedNavigation, rightExpandedNavigation) &&
                    AreEqual(leftExpandedNavigation.SelectAndExpand, rightExpandedNavigation.SelectAndExpand) &&
                    AreEqual(leftExpandedNavigation.LevelsOption, rightExpandedNavigation.LevelsOption);
            }

            ExpandedCountSelectItem leftExpandedCount = left as ExpandedCountSelectItem;
            if (leftExpandedCount != null)
            {
                return AreEqualExpandedReference(leftExpandedCount, (ExpandedCountSelectItem)right);
            }

            ExpandedReferenceSelectItem leftExpandedReference = left as ExpandedReferenceSelectItem;
            if (leftExpandedReference != null)
            {
                return AreEqualExpandedReference(leftExpandedReference, (ExpandedReferenceSelectItem)right);
            }

            PathCountSelectItem leftPathCount = left as PathCountSelectItem;
            if (leftPathCount != null)
            {
                PathCountSelectItem rightPathCount = (PathCountSelectItem)right;
                return AreEqual(leftPathCount.SelectedPath, rightPathCount.SelectedPath) &&
                    Equals(leftPathCount.NavigationSource, rightPathCount.NavigationSource) &&
                    AreEqual(leftPathCount.Filter, rightPathCount.Filter) &&
                    AreEqual(leftPathCount.Search, rightPathCount.Search);
            }

            WildcardSelectItem leftWildcard = left as WildcardSelectItem;
            if (leftWildcard != null)
            {
                return SequenceEqual(leftWildcard.SubsumedSelectItems, ((WildcardSelectItem)right).SubsumedSelectItems, AreEqual);
            }

            NamespaceQualifiedWildcardSelectItem leftNamespaceWildcard = left as NamespaceQualifiedWildcardSelectItem;
            if (leftNamespaceWildcard != null)
            {
                return string.Equals(leftNamespaceWildcard.Namespace, ((NamespaceQualifiedWildcardSelectItem)right).Namespace, StringComparison.Ordinal);
            }

            return false;
        }

        private static int GetHashCode(SelectItem item)
        {
            if (item == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(item.GetType());

            PathSelectItem pathSelectItem = item as PathSelectItem;
            if (pathSelectItem != null)
            {
                hashCode.Add(GetHashCode(pathSelectItem.SelectedPath));
                hashCode.Add(pathSelectItem.NavigationSource);
                hashCode.Add(GetHashCode(pathSelectItem.SelectAndExpand));
                hashCode.Add(GetHashCode(pathSelectItem.FilterOption));
                hashCode.Add(GetHashCode(pathSelectItem.OrderByOption));
                hashCode.Add(pathSelectItem.TopOption);
                hashCode.Add(pathSelectItem.SkipOption);
                hashCode.Add(pathSelectItem.CountOption);
                hashCode.Add(GetHashCode(pathSelectItem.SearchOption));
                hashCode.Add(GetHashCode(pathSelectItem.ComputeOption));
                return hashCode.ToHashCode();
            }

            ExpandedNavigationSelectItem expandedNavigationSelectItem = item as ExpandedNavigationSelectItem;
            if (expandedNavigationSelectItem != null)
            {
                AddExpandedReferenceHashCode(ref hashCode, expandedNavigationSelectItem);
                hashCode.Add(GetHashCode(expandedNavigationSelectItem.SelectAndExpand));
                hashCode.Add(GetHashCode(expandedNavigationSelectItem.LevelsOption));
                return hashCode.ToHashCode();
            }

            ExpandedReferenceSelectItem expandedReferenceSelectItem = item as ExpandedReferenceSelectItem;
            if (expandedReferenceSelectItem != null)
            {
                AddExpandedReferenceHashCode(ref hashCode, expandedReferenceSelectItem);
                return hashCode.ToHashCode();
            }

            PathCountSelectItem pathCountSelectItem = item as PathCountSelectItem;
            if (pathCountSelectItem != null)
            {
                hashCode.Add(GetHashCode(pathCountSelectItem.SelectedPath));
                hashCode.Add(pathCountSelectItem.NavigationSource);
                hashCode.Add(GetHashCode(pathCountSelectItem.Filter));
                hashCode.Add(GetHashCode(pathCountSelectItem.Search));
                return hashCode.ToHashCode();
            }

            WildcardSelectItem wildcardSelectItem = item as WildcardSelectItem;
            if (wildcardSelectItem != null)
            {
                AddSequenceHashCode(ref hashCode, wildcardSelectItem.SubsumedSelectItems, GetHashCode);
                return hashCode.ToHashCode();
            }

            NamespaceQualifiedWildcardSelectItem namespaceQualifiedWildcardSelectItem = item as NamespaceQualifiedWildcardSelectItem;
            if (namespaceQualifiedWildcardSelectItem != null)
            {
                hashCode.Add(namespaceQualifiedWildcardSelectItem.Namespace, StringComparer.Ordinal);
                return hashCode.ToHashCode();
            }

            return hashCode.ToHashCode();
        }

        private static bool AreEqual(ODataPath left, ODataPath right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null || left.GetType() != right.GetType() || left.Count != right.Count)
            {
                return false;
            }

            for (int i = 0; i < left.Count; i++)
            {
                if (!left[i].Equals(right[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static int GetHashCode(ODataPath path)
        {
            if (path == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(path.GetType());

            foreach (ODataPathSegment segment in path)
            {
                hashCode.Add(segment?.TranslateWith(PathSegmentToStringTranslator.Instance), StringComparer.Ordinal);
                hashCode.Add(segment?.GetType());
            }

            return hashCode.ToHashCode();
        }

        private static bool HaveEquivalentMetadata(QueryNode left, QueryNode right)
        {
            SingleValueNode leftSingleValue = left as SingleValueNode;
            SingleValueNode rightSingleValue = right as SingleValueNode;
            if (leftSingleValue != null || rightSingleValue != null)
            {
                if (leftSingleValue == null || rightSingleValue == null || !AreEqual(leftSingleValue.TypeReference, rightSingleValue.TypeReference))
                {
                    return false;
                }
            }

            CollectionNode leftCollection = left as CollectionNode;
            CollectionNode rightCollection = right as CollectionNode;
            if (leftCollection != null || rightCollection != null)
            {
                if (leftCollection == null || rightCollection == null || !AreEqual(leftCollection.ItemType, rightCollection.ItemType) || !AreEqual(leftCollection.CollectionType, rightCollection.CollectionType))
                {
                    return false;
                }
            }

            SingleResourceNode leftSingleResource = left as SingleResourceNode;
            SingleResourceNode rightSingleResource = right as SingleResourceNode;
            if (leftSingleResource != null || rightSingleResource != null)
            {
                if (leftSingleResource == null || rightSingleResource == null || !Equals(leftSingleResource.NavigationSource, rightSingleResource.NavigationSource) || !AreEqual(leftSingleResource.StructuredTypeReference, rightSingleResource.StructuredTypeReference))
                {
                    return false;
                }
            }

            CollectionResourceNode leftCollectionResource = left as CollectionResourceNode;
            CollectionResourceNode rightCollectionResource = right as CollectionResourceNode;
            if (leftCollectionResource != null || rightCollectionResource != null)
            {
                if (leftCollectionResource == null || rightCollectionResource == null || !Equals(leftCollectionResource.NavigationSource, rightCollectionResource.NavigationSource) || !AreEqual(leftCollectionResource.ItemStructuredType, rightCollectionResource.ItemStructuredType))
                {
                    return false;
                }
            }

            LambdaNode leftLambda = left as LambdaNode;
            LambdaNode rightLambda = right as LambdaNode;
            if (leftLambda != null || rightLambda != null)
            {
                if (leftLambda == null || rightLambda == null || !SequenceEqual(leftLambda.RangeVariables, rightLambda.RangeVariables, AreEqual))
                {
                    return false;
                }
            }

            ConstantNode leftConstant = left as ConstantNode;
            ConstantNode rightConstant = right as ConstantNode;
            if (leftConstant != null || rightConstant != null)
            {
                if (leftConstant == null || rightConstant == null || !Equals(leftConstant.Value, rightConstant.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddNodeMetadataHashCode(ref HashCode hashCode, QueryNode node)
        {
            SingleValueNode singleValueNode = node as SingleValueNode;
            if (singleValueNode != null)
            {
                hashCode.Add(GetHashCode(singleValueNode.TypeReference));
            }

            CollectionNode collectionNode = node as CollectionNode;
            if (collectionNode != null)
            {
                hashCode.Add(GetHashCode(collectionNode.ItemType));
                hashCode.Add(GetHashCode(collectionNode.CollectionType));
            }

            SingleResourceNode singleResourceNode = node as SingleResourceNode;
            if (singleResourceNode != null)
            {
                hashCode.Add(singleResourceNode.NavigationSource);
                hashCode.Add(GetHashCode(singleResourceNode.StructuredTypeReference));
            }

            CollectionResourceNode collectionResourceNode = node as CollectionResourceNode;
            if (collectionResourceNode != null)
            {
                hashCode.Add(collectionResourceNode.NavigationSource);
                hashCode.Add(GetHashCode(collectionResourceNode.ItemStructuredType));
            }

            LambdaNode lambdaNode = node as LambdaNode;
            if (lambdaNode != null)
            {
                AddSequenceHashCode(ref hashCode, lambdaNode.RangeVariables, GetHashCode);
            }

            ConstantNode constantNode = node as ConstantNode;
            if (constantNode != null)
            {
                hashCode.Add(constantNode.Value);
            }
        }

        private static string GetNodeText(QueryNode node)
        {
            return new NodeToStringBuilder().TranslateNode(node);
        }

        private static string GetApplyClauseText(ApplyClause clause)
        {
            return new ApplyClauseToStringBuilder().TranslateApplyClause(clause);
        }

        private static bool AreEqual(Microsoft.OData.Edm.IEdmTypeReference left, Microsoft.OData.Edm.IEdmTypeReference right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            return left.IsNullable == right.IsNullable &&
                string.Equals(left.FullName(), right.FullName(), StringComparison.Ordinal);
        }

        private static int GetHashCode(Microsoft.OData.Edm.IEdmTypeReference typeReference)
        {
            if (typeReference == null)
            {
                return 0;
            }

            HashCode hashCode = new HashCode();
            hashCode.Add(typeReference.FullName(), StringComparer.Ordinal);
            hashCode.Add(typeReference.IsNullable);
            return hashCode.ToHashCode();
        }

        private static bool AreEqualExpandedReference(ExpandedReferenceSelectItem left, ExpandedReferenceSelectItem right)
        {
            return AreEqual(left.PathToNavigationProperty, right.PathToNavigationProperty) &&
                Equals(left.NavigationSource, right.NavigationSource) &&
                AreEqual(left.FilterOption, right.FilterOption) &&
                AreEqual(left.OrderByOption, right.OrderByOption) &&
                left.TopOption == right.TopOption &&
                left.SkipOption == right.SkipOption &&
                left.CountOption == right.CountOption &&
                AreEqual(left.SearchOption, right.SearchOption) &&
                AreEqual(left.ComputeOption, right.ComputeOption) &&
                AreEqual(left.ApplyOption, right.ApplyOption);
        }

        private static void AddExpandedReferenceHashCode(ref HashCode hashCode, ExpandedReferenceSelectItem item)
        {
            hashCode.Add(GetHashCode(item.PathToNavigationProperty));
            hashCode.Add(item.NavigationSource);
            hashCode.Add(GetHashCode(item.FilterOption));
            hashCode.Add(GetHashCode(item.OrderByOption));
            hashCode.Add(item.TopOption);
            hashCode.Add(item.SkipOption);
            hashCode.Add(item.CountOption);
            hashCode.Add(GetHashCode(item.SearchOption));
            hashCode.Add(GetHashCode(item.ComputeOption));
            hashCode.Add(GetHashCode(item.ApplyOption));
        }

        private static bool SequenceEqual<T>(IEnumerable<T> left, IEnumerable<T> right, Func<T, T, bool> areEqual)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            using (IEnumerator<T> leftEnumerator = left.GetEnumerator())
            using (IEnumerator<T> rightEnumerator = right.GetEnumerator())
            {
                while (true)
                {
                    bool leftMoved = leftEnumerator.MoveNext();
                    bool rightMoved = rightEnumerator.MoveNext();

                    if (leftMoved != rightMoved)
                    {
                        return false;
                    }

                    if (!leftMoved)
                    {
                        return true;
                    }

                    if (!areEqual(leftEnumerator.Current, rightEnumerator.Current))
                    {
                        return false;
                    }
                }
            }
        }

        private static void AddSequenceHashCode<T>(ref HashCode hashCode, IEnumerable<T> sequence, Func<T, int> getHashCode)
        {
            if (sequence == null)
            {
                hashCode.Add(0);
                return;
            }

            foreach (T item in sequence)
            {
                hashCode.Add(getHashCode(item));
            }
        }
    }
}
