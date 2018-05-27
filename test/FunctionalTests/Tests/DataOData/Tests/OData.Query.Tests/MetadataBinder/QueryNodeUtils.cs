//---------------------------------------------------------------------
// <copyright file="QueryNodeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for testing QueryNode and related classes
    /// </summary>
    internal static class QueryNodeUtils
    {
        public static ODataUri BindQuery(string query, IEdmModel model)
        {
            if (query.StartsWith("/")) query = query.Substring(1);

            return new ODataUriParser(model, new Uri(query, UriKind.Relative)).ParseUri();
        }
        
        public static void VerifyFilterClausesAreEqual(FilterClause expected, FilterClause actual, AssertionHandler assert)
        {
            VeryfyRangeVariablesAreEqual(expected.RangeVariable, actual.RangeVariable, assert);
            VerifyQueryNodesAreEqual(expected.Expression, actual.Expression, assert);
        }

        public static void VerifyOrderByClauseAreEqual(OrderByClause expected, OrderByClause actual, AssertionHandler assert)
        {
            if (expected == null)
            {
                if (actual != null)
                {
                    throw new Exception("Expected no ThenBy, but found one.");
                }
                return;
            }
            
            if (actual == null)
            {
                throw new Exception("Expected a ThenBy, but found none.");
            }

            VeryfyRangeVariablesAreEqual(expected.RangeVariable, actual.RangeVariable, assert);
            VerifyQueryNodesAreEqual(expected.Expression, actual.Expression, assert);
            VerifyOrderByClauseAreEqual(expected.ThenBy, actual.ThenBy, assert);
        }

        public static void VeryfyRangeVariablesAreEqual(RangeVariable expected, RangeVariable actual,
                                                AssertionHandler assert)
        {
            assert.AreEqual(expected.Name, actual.Name, "Name is different");
            assert.AreEqual(expected.Kind, actual.Kind, "InternalKind is different");
        }

        public static void VerifyQueryNodesAreEqual(QueryNode expected, QueryNode actual, AssertionHandler assert)
        {
            try
            {
                if (expected == null)
                {
                    assert.IsNull(actual, "The node should be null.");
                    return;
                }
                else
                {
                    assert.IsNotNull(actual, "The node should not be null.");
                }

                assert.AreEqual(expected.InternalKind, actual.InternalKind, "The node kind differs from expected one.");
                switch (expected.InternalKind)
                {
                    case InternalQueryNodeKind.Constant:
                        VerifyConstantQueryNodesAreEqual((ConstantNode)expected, (ConstantNode)actual, assert);
                        break;
                    case InternalQueryNodeKind.Convert:
                        VerifyConvertQueryNodesAreEqual((ConvertNode)expected, (ConvertNode)actual, assert);
                        break;
                    case InternalQueryNodeKind.NonResourceRangeVariableReference:
                        VerifyNonResourceRangeVariableReferenceNodesAreEqual((NonResourceRangeVariableReferenceNode) expected, (NonResourceRangeVariableReferenceNode) actual,assert);
                        break;
                    case InternalQueryNodeKind.ResourceRangeVariableReference:
                        VerifyResourceRangeVariableReferenceNodesAreEqual((ResourceRangeVariableReferenceNode)expected, (ResourceRangeVariableReferenceNode)actual, assert);
                        break;
                    case InternalQueryNodeKind.BinaryOperator:
                        VerifyBinaryOperatorQueryNodesAreEqual((BinaryOperatorNode)expected, (BinaryOperatorNode)actual, assert);
                        break;
                    case InternalQueryNodeKind.UnaryOperator:
                        VerifyUnaryOperatorQueryNodesAreEqual((UnaryOperatorNode)expected, (UnaryOperatorNode)actual, assert);
                        break;
                    case InternalQueryNodeKind.SingleValuePropertyAccess:
                        VerifyPropertyAccessQueryNodesAreEqual((SingleValuePropertyAccessNode)expected, (SingleValuePropertyAccessNode)actual, assert);
                        break;
                    case InternalQueryNodeKind.SingleComplexNode:
                        VerifySingleComplexNodeAreEqual((SingleComplexNode)expected, (SingleComplexNode)actual, assert);
                        break;
                    case InternalQueryNodeKind.SingleValueFunctionCall:
                        VerifySingleValueFunctionCallQueryNodesAreEqual((SingleValueFunctionCallNode)expected, (SingleValueFunctionCallNode)actual, assert);
                        break;
                    default:
                        throw new Exception("Query node of kind '" + expected.InternalKind.ToString() + "' not yet supported by VerifyQueryNodesAreEqual.");
                }
            }
            catch (Exception)
            {
                assert.Warn("Expected query node: " + expected.ToDebugString());
                assert.Warn("Actual query node: " + actual.ToDebugString());
                throw;
            }
        }

        public static void VerifyQueryNodesAreEqual(IEnumerable<QueryNode> expected, IEnumerable<QueryNode> actual, AssertionHandler assert)
        {
            VerificationUtils.VerifyEnumerationsAreEqual(
                expected,
                actual,
                VerifyQueryNodesAreEqual,
                ToDebugString,
                assert);
        }

        private static void VerifyConstantQueryNodesAreEqual(ConstantNode expected, ConstantNode actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Value, actual.Value, "The Value is different.");
            QueryTestUtils.VerifyTypesAreEqual(expected.TypeReference, actual.TypeReference, assert);
        }

        private static void VerifyConvertQueryNodesAreEqual(ConvertNode expected, ConvertNode actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.TypeReference.TestFullName(), actual.TypeReference.TestFullName(), "The target type names differ.");
            VerifyQueryNodesAreEqual(expected.Source, actual.Source, assert);
        }

        private static void VerifyNonResourceRangeVariableReferenceNodesAreEqual(NonResourceRangeVariableReferenceNode expected, NonResourceRangeVariableReferenceNode actual, AssertionHandler assert)
        {
            QueryTestUtils.VerifyTypesAreEqual(expected.TypeReference, actual.TypeReference, assert);
        }

        private static void VerifyResourceRangeVariableReferenceNodesAreEqual(ResourceRangeVariableReferenceNode expected, ResourceRangeVariableReferenceNode actual, AssertionHandler assert)
        {
            QueryTestUtils.VerifyTypesAreEqual(expected.TypeReference, actual.TypeReference, assert);
        }

        private static void VerifyBinaryOperatorQueryNodesAreEqual(BinaryOperatorNode expected, BinaryOperatorNode actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.OperatorKind, actual.OperatorKind, "Operator kinds differ.");
            VerifyQueryNodesAreEqual(expected.Left, actual.Left, assert);
            VerifyQueryNodesAreEqual(expected.Right, actual.Right, assert);
        }

        private static void VerifyUnaryOperatorQueryNodesAreEqual(UnaryOperatorNode expected, UnaryOperatorNode actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.OperatorKind, actual.OperatorKind, "Operator kinds differ.");
            VerifyQueryNodesAreEqual(expected.Operand, actual.Operand, assert);
        }

        private static void VerifyPropertyAccessQueryNodesAreEqual(SingleValuePropertyAccessNode expected, SingleValuePropertyAccessNode actual, AssertionHandler assert)
        {
            VerifyQueryNodesAreEqual(expected.Source, actual.Source, assert);
            QueryTestUtils.VerifyPropertiesAreEqual(expected.Property, actual.Property, assert);
        }

        private static void VerifySingleComplexNodeAreEqual(SingleComplexNode expected, SingleComplexNode actual, AssertionHandler assert)
        {
            VerifyQueryNodesAreEqual(expected.Source, actual.Source, assert);
            QueryTestUtils.VerifyPropertiesAreEqual(expected.Property, actual.Property, assert);
        }

        private static void VerifySingleValueFunctionCallQueryNodesAreEqual(SingleValueFunctionCallNode expected, SingleValueFunctionCallNode actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Name, actual.Name, "The names of the functions are different.");
            VerifyQueryNodesAreEqual(expected.Parameters, actual.Parameters, assert);
            QueryTestUtils.VerifyTypesAreEqual(expected.TypeReference, actual.TypeReference, assert);
        }

        public static string ToDebugString(this RangeVariable node)
        {
            return "Parameter(" + node.TypeReference.TestFullName() + ")";
        }

        public static string ToDebugString(this QueryNode node)
        {
            string result = "";

            switch (node.InternalKind)
            {
                case InternalQueryNodeKind.Constant:
                    var constantNode = (ConstantNode)node;
                    if (constantNode.Value == null)
                    {
                        result = "Constant(null)";
                    }
                    else
                    {
                        result = "Constant(" + constantNode.Value.ToString() + ")";
                    }
                    break;
                case InternalQueryNodeKind.Convert:
                    var convert = (ConvertNode)node;
                    result = convert.Source.ToDebugString() + ".Convert(" + convert.TypeReference.TestFullName() + ")";
                    break;
                case InternalQueryNodeKind.NonResourceRangeVariableReference:
                    var rangeVariable = (NonResourceRangeVariableReferenceNode)node;
                    result = "Parameter(" + rangeVariable.TypeReference.TestFullName() + ")";
                    break;
                case InternalQueryNodeKind.BinaryOperator:
                    var binaryOperator = (BinaryOperatorNode)node;
                    result = "BinaryOperator(" + binaryOperator.Left.ToDebugString() + " '" + binaryOperator.OperatorKind.ToOperatorName() + "' " + binaryOperator.Right.ToDebugString() + ")";
                    break;
                case InternalQueryNodeKind.UnaryOperator:
                    var unaryOperator = (UnaryOperatorNode)node;
                    result = "UnaryOperator(" + unaryOperator.OperatorKind.ToOperatorName() + " " + unaryOperator.Operand.ToDebugString() + ")";
                    break;
                case InternalQueryNodeKind.SingleValuePropertyAccess:
                    var singlePropertyAccess = (SingleValuePropertyAccessNode)node;
                    result = singlePropertyAccess.Source + "/" + singlePropertyAccess.Property.Name;
                    break;
                case InternalQueryNodeKind.SingleComplexNode:
                    var singleComplexProperty = (SingleComplexNode)node;
                    result = singleComplexProperty.Source + "/" + singleComplexProperty.Property.Name;
                    break;
                case InternalQueryNodeKind.CollectionPropertyAccess:
                    var collectionComplexProperty = (CollectionComplexNode)node;
                    result = collectionComplexProperty.Source + "/" + collectionComplexProperty.Property.Name;
                    break;
                case InternalQueryNodeKind.CollectionComplexNode:
                    var collectionPropertyAccess = (CollectionPropertyAccessNode)node;
                    result = collectionPropertyAccess.Source + "/" + collectionPropertyAccess.Property.Name;
                    break;
                case InternalQueryNodeKind.SingleValueFunctionCall:
                    var singleValueFunctionCall = (SingleValueFunctionCallNode)node;
                    result = singleValueFunctionCall.Name + "(" + string.Join(", ", singleValueFunctionCall.Parameters.Select(a => a.ToDebugString())) + ")";
                    result += "<" + singleValueFunctionCall.TypeReference.TestFullName() + ">";
                    break;
                default:
                    throw new Exception("Node kind '" + node.InternalKind.ToString() + "' not supported by ToDebugString.");
            }

            return result;
        }

        public static string ToDebugString(this KeyPropertyValue keyPropertyValue)
        {
            return keyPropertyValue.KeyProperty.Name + "=" + keyPropertyValue.KeyValue.ToDebugString();
        }
    }
}
