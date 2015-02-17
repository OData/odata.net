//---------------------------------------------------------------------
// <copyright file="QueryNodeComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;

    /// <summary>
    /// Compares 2 query nodes
    /// </summary>
    public class QueryNodeComparer
    {
        public bool Compare(QueryNode left, QueryNode right)
        {
            if (left.Kind != right.Kind)
                return false;

            switch (left.Kind)
            {
                case QueryNodeKind.Any:
                    return this.Compare((AnyNode)left, (AnyNode)right);
                case QueryNodeKind.All:
                    return this.Compare((AllNode)left, (AllNode)right);
                case QueryNodeKind.NonentityRangeVariableReference:
                    return this.Compare((NonentityRangeVariableReferenceNode)left, (NonentityRangeVariableReferenceNode)right);
                case QueryNodeKind.Convert:
                    return this.Compare((ConvertNode)left, (ConvertNode)right);
                case QueryNodeKind.BinaryOperator:
                    return this.Compare((BinaryOperatorNode)left, (BinaryOperatorNode)right);
                case QueryNodeKind.UnaryOperator:
                    return this.Compare((UnaryOperatorNode)left, (UnaryOperatorNode)right);
                case QueryNodeKind.SingleValueFunctionCall:
                    return this.Compare((SingleValueFunctionCallNode)left, (SingleValueFunctionCallNode)right);
                case QueryNodeKind.SingleValuePropertyAccess:
                    return this.Compare((SingleValuePropertyAccessNode)left, (SingleValuePropertyAccessNode)right);
                case QueryNodeKind.CollectionPropertyAccess:
                    return this.Compare((CollectionPropertyAccessNode)left, (CollectionPropertyAccessNode)right);
                case QueryNodeKind.SingleEntityCast:
                    return this.Compare((SingleEntityCastNode)left, (SingleEntityCastNode)right);
                case QueryNodeKind.EntityCollectionCast:
                    return this.Compare((EntityCollectionCastNode)left, (EntityCollectionCastNode)right);
                case QueryNodeKind.EntityRangeVariableReference:
                    return this.Compare((EntityRangeVariableReferenceNode)left, (EntityRangeVariableReferenceNode)right);
                case QueryNodeKind.Constant:
                    return this.Compare((ConstantNode)left, (ConstantNode)right);
                case QueryNodeKind.CollectionNavigationNode:
                    return this.Compare((CollectionNavigationNode)left, (CollectionNavigationNode)right);
                case QueryNodeKind.SingleNavigationNode:
                    return this.Compare((SingleNavigationNode)left, (SingleNavigationNode)right);
                default:
                    throw new NotSupportedException(String.Format("Node kind not yet supported: {0}", left.Kind.ToString()));
            }
        }

        /// <summary>
        /// Compares convert query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(ConvertNode left, ConvertNode right)
        {
            if (left.TypeReference != right.TypeReference) return false;
            if (!this.Compare(left.Source, right.Source)) return false;

            return true;
        }
        
        /// <summary>
        /// Compares entity parameter query nodes query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(EntityRangeVariableReferenceNode left, EntityRangeVariableReferenceNode right)
        {
            if (left.Name != right.Name) return false;
            if (left.TypeReference != right.TypeReference) return false;
            if (left.NavigationSource != right.NavigationSource) return false;
            if (left.EntityTypeReference != right.EntityTypeReference) return false;
            return true;
        }

        /// <summary>
        /// Compares entityrangevariables.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(EntityRangeVariable left, EntityRangeVariable right)
        {
            if (left.Name != right.Name) return false;
            if (left.TypeReference != right.TypeReference) return false;
            if (left.NavigationSource != right.NavigationSource) return false;
            if (left.EntityTypeReference != right.EntityTypeReference) return false;
            return true;
        }

        /// <summary>
        /// Compares non entity parameter query nodes query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(NonentityRangeVariableReferenceNode left, NonentityRangeVariableReferenceNode right)
        {
            if (left.Name != right.Name) return false;
            if (left.TypeReference != right.TypeReference) return false;
            return true;
        }

        /// <summary>
        /// Compares non entity parameter query nodes query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(NonentityRangeVariable left, NonentityRangeVariable right)
        {
            if (left.Name != right.Name) return false;
            if (left.TypeReference != right.TypeReference) return false;
            return true;
        }

        /// <summary>
        /// Compares Single Value Property Access query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(SingleValuePropertyAccessNode left, SingleValuePropertyAccessNode right)
        {
            if (left.Property != right.Property) return false;
            if (left.TypeReference != right.TypeReference) return false;
            return this.Compare(left.Source, right.Source);
        }

        /// <summary>
        /// Compares Collection Property Access query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(CollectionPropertyAccessNode left, CollectionPropertyAccessNode right)
        {
            if (left.Property != right.Property) return false;
            if (left.ItemType != right.ItemType) return false;
            return this.Compare(left.Source, right.Source);
        }

        /// <summary>
        /// Compares Filter query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        public bool Compare(FilterClause left, FilterClause right)
        {
            if (left.ItemType != right.ItemType) return false;
            if (!this.Compare(left.Expression, right.Expression)) return false;
            if (!this.CompareParameters(left.RangeVariable, right.RangeVariable)) return false;
            return true;
        }

        /// <summary>
        /// Compares skip query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        //private bool Compare(SkipNode left, SkipNode right)
        //{
        //    if (left.ItemType != right.ItemType) return false;
        //    if (!this.Compare(left.Collection, right.Collection)) return false;
        //    return this.Compare(left.Amount, right.Amount);
        //}

        /// <summary>
        /// Compares Unary Operator query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(UnaryOperatorNode left, UnaryOperatorNode right)
        {
            if (left.OperatorKind != right.OperatorKind) return false;
            if (left.TypeReference != right.TypeReference) return false;

            return this.Compare(left.Operand, right.Operand);
        }

        /// <summary>
        /// Compares binary operator query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(BinaryOperatorNode left, BinaryOperatorNode right)
        {
            if (left.OperatorKind != right.OperatorKind) return false;
            if (left.TypeReference != right.TypeReference) return false;
            if (!this.Compare(left.Left, right.Left)) return false;
            return this.Compare(left.Right, right.Right);
        }

        /// <summary>
        /// Compares top query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        //private bool Compare(TopNode left, TopNode right)
        //{
        //    if (left.ItemType != right.ItemType) return false;
        //    if (!this.Compare(left.Amount, right.Amount)) return false;
        //    return this.Compare(left.Collection, right.Collection);
        //}

        /// <summary>
        /// Compares key lookup query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        //private bool Compare(KeyLookupNode left, KeyLookupNode right)
        //{
        //    if (left.EntityTypeReference != right.EntityTypeReference) return false;
        //    if (left.EntitySet != right.EntitySet) return false;
        //    if (left.TypeReference != right.TypeReference) return false;
        //    if (left.KeyPropertyValues.Count() != right.KeyPropertyValues.Count()) return false;

        //    for (int i = 0; i < left.KeyPropertyValues.Count(); ++i)
        //    {
        //        KeyPropertyValue leftKpv= left.KeyPropertyValues.ElementAt(i);
        //        KeyPropertyValue rightKpv = right.KeyPropertyValues.ElementAt(i);

        //        if (leftKpv.KeyProperty != rightKpv.KeyProperty) return false;
        //        if (!this.Compare(leftKpv.KeyValue, rightKpv.KeyValue)) return false;
        //    }

        //    return this.Compare(left.Source, right.Source);
        //}

        /// <summary>
        /// Compares entity set query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        //private bool Compare(EntitySetNode left, EntitySetNode right)
        //{
        //    if (left.EntityItemType != right.EntityItemType) return false;
        //    if (left.EntitySet != right.EntitySet) return false;
        //    if (left.ItemType != right.ItemType) return false;
        //    if (left.OverrideType != right.OverrideType) return false;
        //    return true;
        //}

        /// <summary>
        /// Compares constant query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>        
        private bool Compare(ConstantNode left, ConstantNode right)
        {
            if (left.TypeReference != right.TypeReference) return false;
            if (left.Value != right.Value) return false;
            return true;
        }

        /// <summary>
        /// Compares any query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(AnyNode left, AnyNode right)
        {
            if (left.TypeReference != right.TypeReference) return false;
            if (!this.Compare(left.Body, right.Body)) return false;
            if (left.RangeVariables.Count() != right.RangeVariables.Count()) return false;
            for (int i = 0; i < left.RangeVariables.Count(); ++i)
            {
                if (!this.CompareParameters(left.RangeVariables.ElementAt(i), right.RangeVariables.ElementAt(i))) return false;
            }

            return this.Compare(left.Source, right.Source);
        }

        /// <summary>
        /// Compares single cast query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(SingleEntityCastNode left, SingleEntityCastNode right)
        {
            if (left.NavigationSource != right.NavigationSource) return false;
            if (left.EntityTypeReference != right.EntityTypeReference) return false;
            if (left.TypeReference != right.TypeReference) return false;
            return this.Compare(left.Source, right.Source);
        }

        /// <summary>
        /// Compares all query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(AllNode left, AllNode right)
        {
            
            if (left.TypeReference != right.TypeReference) return false;
            if (!this.Compare(left.Body, right.Body)) return false;
            if (left.RangeVariables.Count() != right.RangeVariables.Count()) return false;
            for (int i = 0; i < left.RangeVariables.Count(); ++ i )
            {
                if (!this.CompareParameters(left.RangeVariables.ElementAt(i), right.RangeVariables.ElementAt(i))) return false;
            }
            
            return this.Compare(left.Source, right.Source);
        }

        /// <summary>
        /// Compares collection cast nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(EntityCollectionCastNode left, EntityCollectionCastNode right)
        {
            if (left.EntityItemType != right.EntityItemType) return false;
            if (left.NavigationSource != right.NavigationSource) return false;
            if (left.ItemType != right.ItemType) return false;
            return this.Compare(left.Source, right.Source);
        }

        /// <summary>
        /// Compares custom query option query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        //private bool Compare(CustomQueryOptionNode left, CustomQueryOptionNode right)
        //{
        //    if (left.Name != right.Name) return false;
        //    if (left.Value != right.Value) return false;
        //    return true;
        //}

        /// <summary>
        /// Compares single value function call query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(SingleValueFunctionCallNode left, SingleValueFunctionCallNode right)
        {
            if (left.Name != right.Name) return false;
            if (left.TypeReference != right.TypeReference) return false;
            if (left.TypeReference != right.TypeReference) return false;

            for (int i = 0; i < left.Parameters.Count(); ++i)
            {
                if (!this.Compare(left.Parameters.ElementAt(i), right.Parameters.ElementAt(i))) return false;
            }

            return true;
        }

        /// <summary>
        /// Compares order by query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(OrderByClause left, OrderByClause right)
        {
            if (left.ItemType != right.ItemType) return false;
            if (left.Direction != right.Direction) return false;
            if (!this.Compare(left.ThenBy, right.ThenBy)) return false;
            if (!this.Compare(left.Expression, right.Expression)) return false;
            return this.CompareParameters(left.RangeVariable, right.RangeVariable);
            
        }

        /// <summary>
        /// Compares single navigation nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(SingleNavigationNode left, SingleNavigationNode right)
        {
            if (left.NavigationSource != right.NavigationSource) return false;
            if (left.EntityTypeReference != right.EntityTypeReference) return false;
            if (left.NavigationProperty != right.NavigationProperty) return false;
            if (left.TargetMultiplicity != right.TargetMultiplicity) return false;
            if (left.TypeReference != right.TypeReference) return false;
            return this.Compare(left.Source, right.Source);

        }

        /// <summary>
        /// Compares collection navigation nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(CollectionNavigationNode left, CollectionNavigationNode right)
        {
            if (left.ItemType != right.ItemType) return false;
            if (left.EntityItemType != right.EntityItemType) return false;
            if (left.NavigationSource != right.NavigationSource) return false;
            if (left.TargetMultiplicity != right.TargetMultiplicity) return false;
            if (left.NavigationProperty != right.NavigationProperty) return false;
            return this.Compare(left.Source, right.Source);
        }

        /// <summary>
        /// Compares SingleEntityFunctionCall nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool Compare(SingleEntityFunctionCallNode left, SingleEntityFunctionCallNode right)
        {
            if (left.Parameters.Count() != right.Parameters.Count()) return false;
            if (left.NavigationSource != right.NavigationSource) return false;
            if (left.EntityTypeReference != right.EntityTypeReference) return false;
            if (left.Name != right.Name) return false;
            if (left.TypeReference != right.TypeReference) return false;

            for (int i = 0; i < left.Parameters.Count(); ++i)
            {
                if (!this.Compare(left.Parameters.ElementAt(i), right.Parameters.ElementAt(i))) return false;
            }

            return true;            
        }
        
        /// <summary>
        /// Redirects the comparison of parameter query nodes.
        /// </summary>
        /// <param name="left">Left side of comparison</param>
        /// <param name="right">Right side of comparison</param>
        /// <returns>True if equal, otherwise false</returns>
        private bool CompareParameters(RangeVariable left, RangeVariable right)
        {
            if (left is EntityRangeVariable && right is EntityRangeVariable)
                return this.Compare((EntityRangeVariable)left, (EntityRangeVariable)right);
            if (left is NonentityRangeVariable && right is NonentityRangeVariable)
                return this.Compare((NonentityRangeVariable)left, (NonentityRangeVariable)right);
            return false;
        }
    }
}
