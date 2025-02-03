//---------------------------------------------------------------------
// <copyright file="NodeAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using System.Collections.Generic;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Contains fluent assertion APIs for testing QueryNodes.
    /// TODO: Consider using T : QueryNode instead, and writing a test to assert that the QueryNodeKind matches the relevent class seperately.
    /// </summary>
    internal static class NodeAssertions
    {
        public static ConstantNode ShouldBeConstantQueryNode<TValue>(this QueryNode node, TValue expectedValue)
        {
            Assert.NotNull(node);
            var constantNode = Assert.IsType<ConstantNode>(node);
            if (expectedValue == null)
            {
                Assert.Null(constantNode.Value);
            }
            else
            {
                Type nodeValueType = constantNode.Value.GetType();
                Assert.True(typeof(TValue).IsAssignableFrom(nodeValueType));
                Assert.Equal(expectedValue, constantNode.Value);
            }

            return constantNode;
        }

        public static ConvertNode ShouldBeConvertQueryNode(this QueryNode node, EdmPrimitiveTypeKind expectedTypeKind)
        {
            Assert.NotNull(node);
            var convertNode = Assert.IsType<ConvertNode>(node);
            Assert.Equal(expectedTypeKind, convertNode.TypeReference.PrimitiveKind());
            return convertNode;
        }

        public static ConvertNode ShouldBeConvertQueryNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            Assert.NotNull(node);
            var convertNode = Assert.IsType<ConvertNode>(node);
            Assert.True(convertNode.TypeReference.IsEquivalentTo(expectedTypeReference));
            return convertNode;
        }

        public static ResourceRangeVariable ShouldBeResourceRangeVariable(this RangeVariable variable, IEdmEntityTypeReference expectedTypeReference)
        {
            Assert.NotNull(variable);
            var node = Assert.IsType<ResourceRangeVariable>(variable);
            Assert.True(node.TypeReference.IsEquivalentTo(expectedTypeReference));
            return node;
        }

        public static ResourceRangeVariable ShouldBeResourceRangeVariable(this RangeVariable variable, string expectedName)
        {
            Assert.NotNull(variable);
            var rangeVariable = Assert.IsType<ResourceRangeVariable>(variable);
            Assert.Equal(expectedName, rangeVariable.Name);
            return rangeVariable;
        }

        public static NonResourceRangeVariable ShouldBeNonentityRangeVariable(this RangeVariable variable, string expectedName)
        {
            Assert.NotNull(variable);
            var rangeVariable = Assert.IsType<NonResourceRangeVariable>(variable);
            Assert.Equal(expectedName, rangeVariable.Name);
            return rangeVariable;
        }

        public static NonResourceRangeVariableReferenceNode ShouldBeNonResourceRangeVariableReferenceNode(this QueryNode node, string expectedName)
        {
            Assert.NotNull(node);
            var rangeVariableNode = Assert.IsType<NonResourceRangeVariableReferenceNode>(node);
            Assert.Equal(expectedName, rangeVariableNode.Name);
            return rangeVariableNode;
        }

        public static ResourceRangeVariableReferenceNode ShouldBeResourceRangeVariableReferenceNode(this QueryNode node, string expectedName)
        {
            Assert.NotNull(node);
            var referenceNode = Assert.IsType<ResourceRangeVariableReferenceNode>(node);
            Assert.Equal(expectedName, referenceNode.Name);
            return referenceNode;
        }

        public static SingleValueFunctionCallNode ShouldBeSingleValueFunctionCallQueryNode(this QueryNode node, params IEdmFunction[] operationImports)
        {
            Assert.NotNull(node);
            var functionCallNode = Assert.IsType<SingleValueFunctionCallNode>(node);
            functionCallNode.Functions.ContainExactly(operationImports);
            return functionCallNode;
        }

        public static SingleResourceFunctionCallNode ShouldBeSingleResourceFunctionCallQueryNode(this QueryNode node, params IEdmFunction[] operationImports)
        {
            Assert.NotNull(node);
            var functionCallNode = Assert.IsType<SingleResourceFunctionCallNode>(node);
            functionCallNode.Functions.ContainExactly(operationImports);
            return functionCallNode;
        }

        /// <summary>
        /// Asserts that the current collection only contains items that are assignable to the type T.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="enumerable">The test enumerable.</param>
        private static void ContainItemsAssignableTo<TValue>(this IEnumerable<QueryNode> enumerable)
        {
            Type type = typeof(TValue);
            foreach (var item in enumerable)
            {
                Assert.True(type.IsAssignableFrom(item.GetType()));
            }
        }

        public static SingleValueFunctionCallNode ShouldHaveConstantParameter<TValue>(this SingleValueFunctionCallNode functionCallNode, string name, TValue value)
        {
            Assert.NotNull(functionCallNode);
            functionCallNode.Parameters.ContainItemsAssignableTo<NamedFunctionParameterNode>();
            var argument = functionCallNode.Parameters.OfType<NamedFunctionParameterNode>().SingleOrDefault(p => p.Name == name);
            Assert.NotNull(argument);
            argument.Value.ShouldBeConstantQueryNode(value);
            return functionCallNode;
        }

        public static CollectionFunctionCallNode ShouldHaveConstantParameter<TValue>(this CollectionFunctionCallNode functionCallNode, string name, TValue value)
        {
            Assert.NotNull(functionCallNode);
            functionCallNode.Parameters.ContainItemsAssignableTo<NamedFunctionParameterNode>();
            var argument = functionCallNode.Parameters.OfType<NamedFunctionParameterNode>().SingleOrDefault(p => p.Name == name);
            Assert.NotNull(argument);
            argument.Value.ShouldBeConstantQueryNode(value);
            return functionCallNode;
        }

        public static CollectionResourceFunctionCallNode ShouldHaveConstantParameter<TValue>(this CollectionResourceFunctionCallNode functionCallNode, string name, TValue value)
        {
            Assert.NotNull(functionCallNode);
            functionCallNode.Parameters.ContainItemsAssignableTo<NamedFunctionParameterNode>();
            var argument = functionCallNode.Parameters.OfType<NamedFunctionParameterNode>().SingleOrDefault(p => p.Name == name);
            Assert.NotNull(argument);
            argument.Value.ShouldBeConstantQueryNode(value);
            return functionCallNode;
        }

        public static SingleValueFunctionCallNode ShouldBeSingleValueFunctionCallQueryNode(this QueryNode node, string name, IEdmTypeReference returnType = null)
        {
            Assert.NotNull(node);
            SingleValueFunctionCallNode functionCallNode = Assert.IsType<SingleValueFunctionCallNode>(node);
            Assert.Equal(name, functionCallNode.Name);
            if (returnType != null)
            {
                Assert.True(functionCallNode.TypeReference.IsEquivalentTo(returnType));
            }

            return functionCallNode;
        }

        public static SingleResourceFunctionCallNode ShouldBeSingleResourceFunctionCallNode(this QueryNode node, params IEdmFunction[] operationImports)
        {
            Assert.NotNull(node);
            var functionCallNode = Assert.IsType<SingleResourceFunctionCallNode>(node);
            functionCallNode.Functions.ContainExactly(operationImports);
            return functionCallNode;
        }

        public static SingleResourceFunctionCallNode ShouldBeSingleResourceFunctionCallNode(this QueryNode node, string name)
        {
            Assert.NotNull(node);
            var functionCallNode = Assert.IsType<SingleResourceFunctionCallNode>(node);
            Assert.Equal(name, functionCallNode.Name);
            return functionCallNode;
        }

        public static CollectionFunctionCallNode ShouldBeCollectionFunctionCallNode(this QueryNode node, params IEdmFunction[] operationImports)
        {
            Assert.NotNull(node);
            var functionCallNode = Assert.IsType<CollectionFunctionCallNode>(node);
            functionCallNode.Functions.ContainExactly(operationImports);
            return functionCallNode;
        }

        public static CollectionResourceFunctionCallNode ShouldBeCollectionResourceFunctionCallNode(this QueryNode node, params IEdmFunction[] operationImports)
        {
            Assert.NotNull(node);
            var functionCallNode = Assert.IsType<CollectionResourceFunctionCallNode>(node);
            functionCallNode.Functions.ContainExactly(operationImports);
            return  functionCallNode;
        }

        public static SingleValuePropertyAccessNode ShouldBeSingleValuePropertyAccessQueryNode(this QueryNode node, IEdmProperty expectedProperty)
        {
            Assert.NotNull(node);
            var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(node);
            Assert.Same(expectedProperty, propertyAccessNode.Property);
            Assert.True(propertyAccessNode.TypeReference.IsEquivalentTo(expectedProperty.Type));
            return propertyAccessNode;
        }

        public static SingleComplexNode ShouldBeSingleComplexNode(this QueryNode node, IEdmProperty expectedProperty)
        {
            Assert.NotNull(node);
            var propertyAccessNode = Assert.IsType<SingleComplexNode>(node);
            Assert.Same(expectedProperty, propertyAccessNode.Property);
            Assert.True(propertyAccessNode.TypeReference.IsEquivalentTo(expectedProperty.Type.AsComplex()));
            return propertyAccessNode;
        }

        public static SingleValueOpenPropertyAccessNode ShouldBeSingleValueOpenPropertyAccessQueryNode(this QueryNode node, string expectedPropertyName)
        {
            Assert.NotNull(node);
            var propertyAccessNode = Assert.IsType<SingleValueOpenPropertyAccessNode>(node);
            Assert.Equal(expectedPropertyName, propertyAccessNode.Name);
            return propertyAccessNode;
        }

        public static CountNode ShouldBeCountNode(this QueryNode node)
        {
            Assert.NotNull(node);
            var propertyAccessNode = Assert.IsType<CountNode>(node);
            return propertyAccessNode;
        }
        
        public static CollectionOpenPropertyAccessNode ShouldBeCollectionOpenPropertyAccessQueryNode(this QueryNode node, string expectedPropertyName)
        {
            Assert.NotNull(node);
            var propertyAccessNode = Assert.IsType<CollectionOpenPropertyAccessNode>(node);
            Assert.Equal(expectedPropertyName, propertyAccessNode.Name);
            return propertyAccessNode;
        }

        public static CollectionPropertyAccessNode ShouldBeCollectionPropertyAccessQueryNode(this QueryNode node, IEdmProperty expectedProperty)
        {
            Assert.NotNull(node);
            var propertyAccessNode = Assert.IsType<CollectionPropertyAccessNode>(node);
            Assert.Same(expectedProperty, propertyAccessNode.Property);
            Assert.True(propertyAccessNode.ItemType.IsEquivalentTo(((IEdmCollectionType)expectedProperty.Type.Definition).ElementType));
            return propertyAccessNode;
        }

        public static CollectionComplexNode ShouldBeCollectionComplexNode(this QueryNode node, IEdmProperty expectedProperty)
        {
            Assert.NotNull(node);
            var propertyAccessNode = Assert.IsType<CollectionComplexNode>(node);
            Assert.Same(expectedProperty, propertyAccessNode.Property);
            Assert.True(propertyAccessNode.ItemType.IsEquivalentTo(((IEdmCollectionType)expectedProperty.Type.Definition).ElementType.AsComplex()));
            return propertyAccessNode;
        }

        public static SingleNavigationNode ShouldBeSingleNavigationNode(this QueryNode node, IEdmNavigationProperty expectedProperty)
        {
            Assert.NotNull(node);
            var navigationPropertyNode = Assert.IsType<SingleNavigationNode>(node);
            Assert.Same(expectedProperty, navigationPropertyNode.NavigationProperty);
            return navigationPropertyNode;
        }

        public static CollectionNavigationNode ShouldBeCollectionNavigationNode(this QueryNode node, IEdmNavigationProperty expectedProperty)
        {
            Assert.NotNull(node);
            var navigationPropertyNode = Assert.IsType<CollectionNavigationNode>(node);
            Assert.Same(expectedProperty, navigationPropertyNode.NavigationProperty);
            Assert.True(navigationPropertyNode.ItemType.IsEquivalentTo(new EdmEntityTypeReference(expectedProperty.ToEntityType(), expectedProperty.TargetMultiplicity() == EdmMultiplicity.ZeroOrOne)));
            return navigationPropertyNode;
        }

        public static KeyLookupNode ShouldBeKeyLookupQueryNode(this QueryNode node)
        {
            Assert.NotNull(node);
            var keyLookupNode = Assert.IsType<KeyLookupNode>(node);
            return keyLookupNode;
        }

        public static EntitySetNode ShouldBeEntitySetQueryNode(this QueryNode node, IEdmEntitySet expectedSet)
        {
            Assert.NotNull(node);
            var entitySetQueryNode = Assert.IsType<EntitySetNode>(node);
            Assert.Same(expectedSet, entitySetQueryNode.NavigationSource);
            return entitySetQueryNode;
        }

        public static CollectionResourceCastNode ShouldBeCollectionCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            Assert.NotNull(node);
            var collectionCastNode = Assert.IsType<CollectionResourceCastNode>(node);
            Assert.True(collectionCastNode.ItemType.IsEquivalentTo(expectedTypeReference));
            return collectionCastNode;
        }

        public static SingleResourceCastNode ShouldBeSingleCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            Assert.NotNull(node);
            var singleCastNode = Assert.IsType<SingleResourceCastNode>(node);
            Assert.True(singleCastNode.TypeReference.IsEquivalentTo(expectedTypeReference));
            return singleCastNode;
        }

        public static SingleResourceCastNode ShouldBeSingleResourceCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            Assert.NotNull(node);
            var singleValueCastNode = Assert.IsType<SingleResourceCastNode>(node);
            Assert.True(singleValueCastNode.TypeReference.IsEquivalentTo(expectedTypeReference));
            return singleValueCastNode;
        }

        public static CollectionResourceCastNode ShouldBeCollectionResourceCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            Assert.NotNull(node);
            var collectionResourceCastNode = Assert.IsType<CollectionResourceCastNode>(node);
            Assert.True(collectionResourceCastNode.ItemType.IsEquivalentTo(expectedTypeReference));
            return collectionResourceCastNode;
        }

        public static AnyNode ShouldBeAnyQueryNode(this QueryNode node)
        {
            Assert.NotNull(node);
            var anyNode = Assert.IsType<AnyNode>(node);
            return anyNode;
        }

        public static AllNode ShouldBeAllQueryNode(this QueryNode node)
        {
            Assert.NotNull(node);
            var allNode = Assert.IsType<AllNode>(node);
            return allNode;
        }

        public static ConstantNode ShouldBeEnumNode(this QueryNode node, IEdmEnumType enumType, string value)
        {
            Assert.NotNull(node);
            var enumNode = Assert.IsType<ConstantNode>(node);

            Assert.Equal(enumType.FullTypeName(), enumNode.TypeReference.FullName());
            Assert.Equal(value, ((ODataEnumValue)enumNode.Value).Value);
            Assert.Equal(enumType.FullTypeName(), ((ODataEnumValue)enumNode.Value).TypeName);

            return enumNode;
        }

        public static ConstantNode ShouldBeEnumNode(this QueryNode node, IEdmEnumType enumType, Int64 value)
        {
            Assert.NotNull(node);
            var enumNode = Assert.IsType<ConstantNode>(node);

            Assert.Equal(enumType.FullTypeName(), enumNode.TypeReference.FullName());
            Assert.Equal(value + "", ((ODataEnumValue)enumNode.Value).Value);
            Assert.Equal(enumType.FullTypeName(), ((ODataEnumValue)enumNode.Value).TypeName);

            return enumNode;
        }

        public static ConstantNode ShouldBeStringCompatibleEnumNode(this QueryNode node, IEdmEnumType enumType, string value)
        {
            Assert.NotNull(node);
            var enumNode = Assert.IsType<ConstantNode>(node);

            Assert.Equal(enumType.FullTypeName(), enumNode.TypeReference.FullName());
            Assert.Equal(value, ((ODataEnumValue)enumNode.Value).Value);
      
            return enumNode;
        }

        public static BinaryOperatorNode ShouldBeBinaryOperatorNode(this QueryNode node, BinaryOperatorKind expectedOperatorKind)
        {
            Assert.NotNull(node);
            var orderByQueryNode = Assert.IsType<BinaryOperatorNode>(node);
            Assert.Equal(expectedOperatorKind, orderByQueryNode.OperatorKind);
            return orderByQueryNode;
        }

        public static InNode ShouldBeInNode(this QueryNode node)
        {
            Assert.NotNull(node);
            var inNode = Assert.IsType<InNode>(node);
            return inNode;
        }

        public static UnaryOperatorNode ShouldBeUnaryOperatorNode(this QueryNode node, UnaryOperatorKind expectedOperatorKind)
        {
            Assert.NotNull(node);
            var unaryOpeartorNode = Assert.IsType<UnaryOperatorNode>(node);
            Assert.Equal(expectedOperatorKind, unaryOpeartorNode.OperatorKind);
            return unaryOpeartorNode;
        }

        public static SearchTermNode ShouldBeSearchTermNode(this QueryNode node, string text)
        {
            Assert.NotNull(node);
            var searchTermNode = Assert.IsType<SearchTermNode>(node);
            Assert.Equal(text, searchTermNode.Text);
            return searchTermNode;
        }

        public static ParameterAliasNode ShouldBeParameterAliasNode(this QueryNode node, string alias, IEdmTypeReference typeReference)
        {
            Assert.NotNull(node);
            var paramAliasNode = Assert.IsType<ParameterAliasNode>(node);
            Assert.Equal(alias, paramAliasNode.Alias);
            Assert.True(paramAliasNode.TypeReference.IsEquivalentTo(typeReference));
            return paramAliasNode;
        }

        public static UriTemplateExpression ShouldBeUriTemplateExpression(this object node, string literalText, IEdmTypeReference expectedType)
        {
            Assert.NotNull(node);
            UriTemplateExpression uriTemplate = Assert.IsType<UriTemplateExpression>(node);
            Assert.Equal(literalText, uriTemplate.LiteralText);
            Assert.True(uriTemplate.ExpectedType.IsEquivalentTo(expectedType));
            return uriTemplate;
        }
    }
}
