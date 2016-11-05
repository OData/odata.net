//---------------------------------------------------------------------
// <copyright file="NodeAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Core.Tests.UriParser
{
    /// <summary>
    /// Contains fluent assertion APIs for testing QueryNodes.
    /// TODO: Consider using T : QueryNode instead, and writing a test to assert that the QueryNodeKind matches the relevent class seperately.
    /// </summary>
    internal static class NodeAssertions
    {
        public static AndConstraint<SingleValueNode> ShouldBeSingleValueNode(this SingleValueNode actual, SingleValueNode expected)
        {
            actual.TypeReference.Should().BeSameAs(expected.TypeReference);
            return new AndConstraint<SingleValueNode>(actual);
        }

        public static AndConstraint<ConstantNode> ShouldBeConstantQueryNode<TValue>(this QueryNode token, TValue expectedValue)
        {
            token.Should().BeOfType<ConstantNode>();
            var constantNode = token.As<ConstantNode>();
            if (expectedValue == null)
            {
                constantNode.Value.Should().BeNull();
            }
            else
            {
                constantNode.Value.Should().BeAssignableTo<TValue>();
                constantNode.Value.As<TValue>().ShouldBeEquivalentTo(expectedValue);
            }

            return new AndConstraint<ConstantNode>(constantNode);
        }

        public static AndConstraint<ConstantNode> ShouldBeEnumNode<TValue>(this QueryNode token, TValue expectedValue)
        {
            token.Should().BeOfType<ConstantNode>();
            var node = token.As<ConstantNode>();
            if (expectedValue == null)
            {
                node.Value.Should().BeNull();
            }
            else
            {
                node.Value.Should().BeAssignableTo<TValue>();
                node.Value.As<TValue>().ShouldBeEquivalentTo(expectedValue);
            }

            return new AndConstraint<ConstantNode>(node);
        }

        public static AndConstraint<ConvertNode> ShouldBeConvertQueryNode(this QueryNode token, EdmPrimitiveTypeKind expectedTypeKind)
        {
            token.Should().BeOfType<ConvertNode>();
            var convertNode = token.As<ConvertNode>();
            convertNode.TypeReference.PrimitiveKind().Should().Be(expectedTypeKind);
            return new AndConstraint<ConvertNode>(convertNode);
        }

        public static AndConstraint<ConvertNode> ShouldBeConvertQueryNode(this QueryNode token, IEdmTypeReference expectedTypeReference)
        {
            token.Should().BeOfType<ConvertNode>();
            var convertNode = token.As<ConvertNode>();
            convertNode.TypeReference.IsEquivalentTo(expectedTypeReference);
            return new AndConstraint<ConvertNode>(convertNode);
        }

        public static AndConstraint<EntityRangeVariable> ShouldBeEntityRangeVariable(this RangeVariable variable, IEdmEntityTypeReference expectedTypeReference)
        {
            variable.Should().BeOfType<EntityRangeVariable>();
            var node = variable.As<EntityRangeVariable>();
            node.TypeReference.ShouldBeEquivalentTo(expectedTypeReference);
            return new AndConstraint<EntityRangeVariable>(node);
        }

        public static AndConstraint<EntityRangeVariable> ShouldBeEntityRangeVariable(this RangeVariable variable, string expectedName)
        {
            variable.Should().BeOfType<EntityRangeVariable>();
            var rangeVariable = variable.As<EntityRangeVariable>();
            rangeVariable.Name.Should().Be(expectedName);
            return new AndConstraint<EntityRangeVariable>(rangeVariable);
        }

        public static AndConstraint<NonentityRangeVariable> ShouldBeNonentityRangeVariable(this RangeVariable token, string expectedName)
        {
            token.Should().BeOfType<NonentityRangeVariable>();
            var rangeVariable = token.As<NonentityRangeVariable>();
            rangeVariable.Name.Should().Be(expectedName);
            return new AndConstraint<NonentityRangeVariable>(rangeVariable);
        }

        public static AndConstraint<NonentityRangeVariableReferenceNode> ShouldBeNonentityRangeVariableReferenceNode(this QueryNode token, string expectedName)
        {
            token.Should().BeOfType<NonentityRangeVariableReferenceNode>();
            var rangeVariableNode = token.As<NonentityRangeVariableReferenceNode>();
            rangeVariableNode.Name.Should().Be(expectedName);
            return new AndConstraint<NonentityRangeVariableReferenceNode>(rangeVariableNode);
        }

        public static AndConstraint<EntityRangeVariableReferenceNode> ShouldBeEntityRangeVariableReferenceNode(this QueryNode token, string expectedName)
        {
            token.Should().BeOfType<EntityRangeVariableReferenceNode>();
            var parameterNode = token.As<EntityRangeVariableReferenceNode>();
            parameterNode.Name.Should().Be(expectedName);
            return new AndConstraint<EntityRangeVariableReferenceNode>(parameterNode);
        }

        public static AndConstraint<SingleValueFunctionCallNode> ShouldBeSingleValueFunctionCallQueryNode(this QueryNode token, params IEdmFunction[] operationImports)
        {
            token.Should().BeOfType<SingleValueFunctionCallNode>();
            var functionCallNode = token.As<SingleValueFunctionCallNode>();
            functionCallNode.Functions.Should().ContainExactly(operationImports);
            return new AndConstraint<SingleValueFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<SingleValueFunctionCallNode> ShouldHaveConstantParameter<TValue>(this SingleValueFunctionCallNode functionCallNode, string name, TValue value)
        {
            functionCallNode.Should().NotBeNull();
            functionCallNode.Parameters.Should().ContainItemsAssignableTo<NamedFunctionParameterNode>();
            var argument = functionCallNode.Parameters.Cast<NamedFunctionParameterNode>().SingleOrDefault(p => p.Name == name);
            argument.Should().NotBeNull();
            argument.Value.ShouldBeConstantQueryNode(value);
            return new AndConstraint<SingleValueFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<CollectionFunctionCallNode> ShouldHaveConstantParameter<TValue>(this CollectionFunctionCallNode functionCallNode, string name, TValue value)
        {
            functionCallNode.Should().NotBeNull();
            functionCallNode.Parameters.Should().ContainItemsAssignableTo<NamedFunctionParameterNode>();
            var argument = functionCallNode.Parameters.Cast<NamedFunctionParameterNode>().SingleOrDefault(p => p.Name == name);
            argument.Should().NotBeNull();
            argument.Value.ShouldBeConstantQueryNode(value);
            return new AndConstraint<CollectionFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<SingleValueFunctionCallNode> ShouldBeSingleValueFunctionCallQueryNode(this QueryNode token, string name, IEdmTypeReference returnType = null)
        {
            token.Should().BeOfType<SingleValueFunctionCallNode>();
            SingleValueFunctionCallNode functionCallNode = token.As<SingleValueFunctionCallNode>();
            functionCallNode.Name.Should().Be(name);
            if (returnType != null)
            {
                functionCallNode.TypeReference.ShouldBeEquivalentTo(returnType);
            }

            return new AndConstraint<SingleValueFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<SingleEntityFunctionCallNode> ShouldBeSingleEntityFunctionCallNode(this QueryNode token, params IEdmFunction[] operationImports)
        {
            token.Should().BeOfType<SingleEntityFunctionCallNode>();
            var functionCallNode = token.As<SingleEntityFunctionCallNode>();
            functionCallNode.Functions.Should().ContainExactly(operationImports);
            return new AndConstraint<SingleEntityFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<SingleEntityFunctionCallNode> ShouldBeSingleEntityFunctionCallNode(this QueryNode token, string name)
        {
            token.Should().BeOfType<SingleEntityFunctionCallNode>();
            var functionCallNode = token.As<SingleEntityFunctionCallNode>();
            functionCallNode.Name.Should().Be(name);
            return new AndConstraint<SingleEntityFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<CollectionFunctionCallNode> ShouldBeCollectionFunctionCallNode(this QueryNode token, params IEdmFunction[] operationImports)
        {
            token.Should().BeOfType<CollectionFunctionCallNode>();
            var functionCallNode = token.As<CollectionFunctionCallNode>();
            functionCallNode.Functions.Should().ContainExactly(operationImports);
            return new AndConstraint<CollectionFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<EntityCollectionFunctionCallNode> ShouldBeEntityCollectionFunctionCallNode(this QueryNode token, params IEdmFunction[] operationImports)
        {
            token.Should().BeOfType<EntityCollectionFunctionCallNode>();
            var functionCallNode = token.As<EntityCollectionFunctionCallNode>();
            functionCallNode.Functions.Should().ContainExactly(operationImports);
            return new AndConstraint<EntityCollectionFunctionCallNode>(functionCallNode);
        }

        public static AndConstraint<SingleValuePropertyAccessNode> ShouldBeSingleValuePropertyAccessQueryNode(this QueryNode token, IEdmProperty expectedProperty)
        {
            token.Should().BeOfType<SingleValuePropertyAccessNode>();
            var propertyAccessNode = token.As<SingleValuePropertyAccessNode>();
            propertyAccessNode.Property.Should().BeSameAs(expectedProperty);
            propertyAccessNode.TypeReference.Should().BeSameAs(expectedProperty.Type);
            return new AndConstraint<SingleValuePropertyAccessNode>(propertyAccessNode);
        }

        public static AndConstraint<SingleValueOpenPropertyAccessNode> ShouldBeSingleValueOpenPropertyAccessQueryNode(this QueryNode token, string expectedPropertyName)
        {
            token.Should().BeOfType<SingleValueOpenPropertyAccessNode>();
            var propertyAccessNode = token.As<SingleValueOpenPropertyAccessNode>();
            propertyAccessNode.Name.Should().Be(expectedPropertyName);
            return new AndConstraint<SingleValueOpenPropertyAccessNode>(propertyAccessNode);
        }

        public static AndConstraint<CountNode> ShouldBeCountNode(this QueryNode token)
        {
            token.Should().BeOfType<CountNode>();
            var propertyAccessNode = token.As<CountNode>();
            return new AndConstraint<CountNode>(propertyAccessNode);
        }
        
        public static AndConstraint<CollectionOpenPropertyAccessNode> ShouldBeCollectionOpenPropertyAccessQueryNode(this QueryNode token, string expectedPropertyName)
        {
            token.Should().BeOfType<CollectionOpenPropertyAccessNode>();
            var propertyAccessNode = token.As<CollectionOpenPropertyAccessNode>();
            propertyAccessNode.Name.Should().Be(expectedPropertyName);
            return new AndConstraint<CollectionOpenPropertyAccessNode>(propertyAccessNode);
        }

        public static AndConstraint<CollectionPropertyAccessNode> ShouldBeCollectionPropertyAccessQueryNode(this QueryNode token, IEdmProperty expectedProperty)
        {
            token.Should().BeOfType<CollectionPropertyAccessNode>();
            var propertyAccessNode = token.As<CollectionPropertyAccessNode>();
            propertyAccessNode.Property.Should().Be(expectedProperty);
            propertyAccessNode.ItemType.Should().BeSameAs(((IEdmCollectionType)expectedProperty.Type.Definition).ElementType);
            return new AndConstraint<CollectionPropertyAccessNode>(propertyAccessNode);
        }

        public static AndConstraint<SingleNavigationNode> ShouldBeSingleNavigationNode(this QueryNode token, IEdmNavigationProperty expectedProperty)
        {
            token.Should().BeOfType<SingleNavigationNode>();
            var navigationPropertyNode = token.As<SingleNavigationNode>();
            navigationPropertyNode.NavigationProperty.Should().BeSameAs(expectedProperty);
            return new AndConstraint<SingleNavigationNode>(navigationPropertyNode);
        }

        public static AndConstraint<CollectionNavigationNode> ShouldBeCollectionNavigationNode(this QueryNode token, IEdmNavigationProperty expectedProperty)
        {
            token.Should().BeOfType<CollectionNavigationNode>();
            var navigationPropertyNode = token.As<CollectionNavigationNode>();
            navigationPropertyNode.NavigationProperty.Should().BeSameAs(expectedProperty);
            navigationPropertyNode.ItemType.ShouldBeEquivalentTo(new EdmEntityTypeReference(expectedProperty.ToEntityType(), expectedProperty.TargetMultiplicity() == EdmMultiplicity.ZeroOrOne));
            return new AndConstraint<CollectionNavigationNode>(navigationPropertyNode);
        }

        public static AndConstraint<KeyLookupNode> ShouldBeKeyLookupQueryNode(this QueryNode node)
        {
            node.Should().BeOfType<KeyLookupNode>();
            var keyLookupNode = node.As<KeyLookupNode>();
            return new AndConstraint<KeyLookupNode>(keyLookupNode);
        }

        public static AndConstraint<EntitySetNode> ShouldBeEntitySetQueryNode(this QueryNode node, IEdmEntitySet expectedSet)
        {
            node.Should().BeOfType<EntitySetNode>();
            var entitySetQueryNode = node.As<EntitySetNode>();
            entitySetQueryNode.NavigationSource.Should().BeSameAs(expectedSet);
            return new AndConstraint<EntitySetNode>(entitySetQueryNode);
        }

        public static AndConstraint<EntityCollectionCastNode> ShouldBeCollectionCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            node.Should().BeOfType<EntityCollectionCastNode>();
            var collectionCastNode = node.As<EntityCollectionCastNode>();
            collectionCastNode.ItemType.ShouldBeEquivalentTo(expectedTypeReference); // TODO
            return new AndConstraint<EntityCollectionCastNode>(collectionCastNode);
        }

        public static AndConstraint<SingleEntityCastNode> ShouldBeSingleCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            node.Should().BeOfType<SingleEntityCastNode>();
            var singleCastNode = node.As<SingleEntityCastNode>();
            singleCastNode.TypeReference.ShouldBeEquivalentTo(expectedTypeReference);
            return new AndConstraint<SingleEntityCastNode>(singleCastNode);
        }

        public static AndConstraint<SingleValueCastNode> ShouldBeSingleValueCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            node.Should().BeOfType<SingleValueCastNode>();
            var singlePropertyCastNode = node.As<SingleValueCastNode>();
            singlePropertyCastNode.TypeReference.ShouldBeEquivalentTo(expectedTypeReference);
            return new AndConstraint<SingleValueCastNode>(singlePropertyCastNode);
        }

        public static AndConstraint<CollectionPropertyCastNode> ShouldBeCollectionPropertyCastNode(this QueryNode node, IEdmTypeReference expectedTypeReference)
        {
            node.Should().BeOfType<CollectionPropertyCastNode>();
            var collectionPropertyCastNode = node.As<CollectionPropertyCastNode>();
            collectionPropertyCastNode.ItemType.ShouldBeEquivalentTo(expectedTypeReference);
            return new AndConstraint<CollectionPropertyCastNode>(collectionPropertyCastNode);
        }

        public static AndConstraint<AnyNode> ShouldBeAnyQueryNode(this QueryNode node)
        {
            node.Should().BeOfType<AnyNode>();
            var orderByQueryNode = node.As<AnyNode>();
            return new AndConstraint<AnyNode>(orderByQueryNode);
        }

        public static AndConstraint<AllNode> ShouldBeAllQueryNode(this QueryNode node)
        {
            node.Should().BeOfType<AllNode>();
            var orderByQueryNode = node.As<AllNode>();
            return new AndConstraint<AllNode>(orderByQueryNode);
        }

        public static AndConstraint<ConstantNode> ShouldBeEnumNode(this QueryNode node, IEdmEnumType enumType, Int64 value)
        {
            node.Should().BeOfType<ConstantNode>();
            var enumNode = node.As<ConstantNode>();

            enumNode.TypeReference.FullName().Should().Be(enumType.FullTypeName());
            ((ODataEnumValue)enumNode.Value).Value.Should().Be(value + "");
            ((ODataEnumValue)enumNode.Value).TypeName.Should().Be(enumType.FullTypeName());

            return new AndConstraint<ConstantNode>(enumNode);
        }

        public static AndConstraint<BinaryOperatorNode> ShouldBeBinaryOperatorNode(this QueryNode node, BinaryOperatorKind expectedOperatorKind)
        {
            node.Should().BeOfType<BinaryOperatorNode>();
            var orderByQueryNode = node.As<BinaryOperatorNode>();
            orderByQueryNode.OperatorKind.Should().Be(expectedOperatorKind);
            return new AndConstraint<BinaryOperatorNode>(orderByQueryNode);
        }

        public static AndConstraint<UnaryOperatorNode> ShouldBeUnaryOperatorNode(this QueryNode node, UnaryOperatorKind expectedOperatorKind)
        {
            node.Should().BeOfType<UnaryOperatorNode>();
            var unaryOpeartorNode = node.As<UnaryOperatorNode>();
            unaryOpeartorNode.OperatorKind.Should().Be(expectedOperatorKind);
            return new AndConstraint<UnaryOperatorNode>(unaryOpeartorNode);
        }

        public static AndConstraint<SearchTermNode> ShouldBeSearchTermNode(this QueryNode node, string text)
        {
            node.Should().BeOfType<SearchTermNode>();
            var searchTermNode = node.As<SearchTermNode>();
            searchTermNode.Text.Should().Be(text);
            return new AndConstraint<SearchTermNode>(searchTermNode);
        }

        public static AndConstraint<double> ShouldBe(this double value, Double expectedValue)
        {
            value.Should().BeInRange(expectedValue, expectedValue);
            return new AndConstraint<double>(value);
        }

        public static AndConstraint<double?> ShouldBe(this double? value, Double expectedValue)
        {
            value.Should().BeInRange(expectedValue, expectedValue);
            return new AndConstraint<double?>(value);
        }

        public static AndConstraint<ParameterAliasNode> ShouldBeParameterAliasNode(this QueryNode node, string alias, IEdmTypeReference typeReference)
        {
            var tmp = node.As<ParameterAliasNode>();
            tmp.Alias.Should().Be(alias);
            tmp.TypeReference.ShouldBeEquivalentTo(typeReference);
            return new AndConstraint<ParameterAliasNode>(tmp);
        }
    }
}
