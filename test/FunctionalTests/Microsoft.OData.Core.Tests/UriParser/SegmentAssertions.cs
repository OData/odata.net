//---------------------------------------------------------------------
// <copyright file="SegmentAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.Tests.UriParser
{
    /// <summary>
    /// Contains fluent assertion APIs for testing Segments
    /// </summary>
    internal static class SegmentAssertions
    {
        public static AndConstraint<CountSegment> ShouldBeCountSegment(this ODataPathSegment segment)
        {
            segment.Should().BeOfType<CountSegment>();
            return new AndConstraint<CountSegment>(segment.As<CountSegment>());
        }

        public static AndConstraint<BatchSegment> ShouldBeBatchSegment(this ODataPathSegment segment)
        {
            segment.Should().BeOfType<BatchSegment>();
            return new AndConstraint<BatchSegment>(segment.As<BatchSegment>());
        }

        public static AndConstraint<NavigationPropertyLinkSegment> ShouldBeNavigationPropertyLinkSegment(this ODataPathSegment segment, IEdmNavigationProperty navigationProperty)
        {
            segment.Should().BeOfType<NavigationPropertyLinkSegment>();
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = segment.As<NavigationPropertyLinkSegment>();
            navigationPropertyLinkSegment.NavigationProperty.Should().Be(navigationProperty);
            return new AndConstraint<NavigationPropertyLinkSegment>(navigationPropertyLinkSegment);
        }

        public static AndConstraint<MetadataSegment> ShouldBeMetadataSegment(this ODataPathSegment segment)
        {
            segment.Should().BeOfType<MetadataSegment>();
            return new AndConstraint<MetadataSegment>(segment.As<MetadataSegment>());
        }

        public static AndConstraint<ValueSegment> ShouldBeValueSegment(this ODataPathSegment segment)
        {
            segment.Should().BeOfType<ValueSegment>();
            return new AndConstraint<ValueSegment>(segment.As<ValueSegment>());
        }

        public static AndConstraint<TypeSegment> ShouldBeTypeSegment(this ODataPathSegment segment, IEdmType type)
        {
            segment.Should().BeOfType<TypeSegment>();
            TypeSegment typeSegment = segment.As<TypeSegment>();
            typeSegment.EdmType.ShouldBeEquivalentTo(type);
            return new AndConstraint<TypeSegment>(typeSegment);
        }

        public static AndConstraint<PropertySegment> ShouldBePropertySegment(this ODataPathSegment segment, IEdmProperty expectedProperty)
        {
            segment.Should().BeOfType<PropertySegment>();
            PropertySegment propertySegment = segment.As<PropertySegment>();
            propertySegment.Property.Should().Be(expectedProperty);
            return new AndConstraint<PropertySegment>(propertySegment);
        }

        public static AndConstraint<NavigationPropertySegment> ShouldBeNavigationPropertySegment(this ODataPathSegment segment, IEdmNavigationProperty navigationProperty)
        {
            segment.Should().BeOfType<NavigationPropertySegment>();
            NavigationPropertySegment navPropSegment = segment.As<NavigationPropertySegment>();
            navPropSegment.NavigationProperty.Should().Be(navigationProperty);
            return new AndConstraint<NavigationPropertySegment>(navPropSegment);
        }

        public static AndConstraint<OpenPropertySegment> ShouldBeOpenPropertySegment(this ODataPathSegment segment, string openPropertyName)
        {
            segment.Should().BeOfType<OpenPropertySegment>();
            OpenPropertySegment openPropertySegment = segment.As<OpenPropertySegment>();
            openPropertySegment.PropertyName.Should().Be(openPropertyName);
            return new AndConstraint<OpenPropertySegment>(openPropertySegment);
        }

        public static AndConstraint<OperationImportSegment> ShouldBeOperationImportSegment(this ODataPathSegment segment, params IEdmOperationImport[] operationImports)
        {
            segment.Should().BeOfType<OperationImportSegment>();
            OperationImportSegment operationImportSegment = segment.As<OperationImportSegment>();
            operationImportSegment.OperationImports.Should().ContainExactly(operationImports);
            return new AndConstraint<OperationImportSegment>(operationImportSegment);
        }

        public static AndConstraint<OperationSegment> ShouldBeOperationSegment(this ODataPathSegment segment, params IEdmOperation[] operations)
        {
            segment.Should().BeOfType<OperationSegment>();
            OperationSegment operationSegment = segment.As<OperationSegment>();
            operationSegment.Operations.Should().ContainExactly(operations);
            return new AndConstraint<OperationSegment>(operationSegment);
        }

        public static AndConstraint<BatchReferenceSegment> ShouldBeBatchReferenceSegment(this ODataPathSegment segment, IEdmType resultingType)
        {
            segment.Should().BeOfType<BatchReferenceSegment>();
            BatchReferenceSegment batchReferenceSegment = segment.As<BatchReferenceSegment>();
            batchReferenceSegment.EdmType.Should().Be(resultingType);
            return new AndConstraint<BatchReferenceSegment>(batchReferenceSegment);
        }

        public static AndConstraint<EntitySetSegment> ShouldBeEntitySetSegment(this ODataPathSegment segment, IEdmEntitySet entitySet)
        {
            segment.Should().BeOfType<EntitySetSegment>();
            EntitySetSegment entitySetSegment = segment.As<EntitySetSegment>();
            entitySetSegment.EntitySet.Should().BeSameAs(entitySet);
            return new AndConstraint<EntitySetSegment>(entitySetSegment);
        }

        public static AndConstraint<SingletonSegment> ShouldBeSingletonSegment(this ODataPathSegment segment, IEdmSingleton singleton)
        {
            segment.Should().BeOfType<SingletonSegment>();
            SingletonSegment singletonSegment = segment.As<SingletonSegment>();
            singletonSegment.Singleton.Should().BeSameAs(singleton);
            return new AndConstraint<SingletonSegment>(singletonSegment);
        }

        public static AndConstraint<KeySegment> ShouldBeKeySegment(this ODataPathSegment segment, params KeyValuePair<string, object>[] keys)
        {
            segment.Should().BeOfType<KeySegment>();
            KeySegment entitySetSegment = segment.As<KeySegment>();
            entitySetSegment.Keys.Should().ContainExactly(keys);
            return new AndConstraint<KeySegment>(entitySetSegment);
        }

        public static AndConstraint<KeySegment> ShouldBeSimpleKeySegment(this ODataPathSegment segment, object value)
        {
            segment.Should().BeOfType<KeySegment>();
            KeySegment entitySetSegment = segment.As<KeySegment>();
            entitySetSegment.Keys.Count().Should().Be(1);
            entitySetSegment.Keys.Single().Value.Should().Be(value);
            return new AndConstraint<KeySegment>(entitySetSegment);
        }

        public static AndConstraint<OperationSegment> ShouldHaveParameterCount(this OperationSegment segment, int count)
        {
            segment.Parameters.Count().Should().Be(count);
            return new AndConstraint<OperationSegment>(segment);
        }

        public static AndConstraint<OperationImportSegment> ShouldHaveParameterCount(this OperationImportSegment segment, int count)
        {
            segment.Parameters.Count().Should().Be(count);
            return new AndConstraint<OperationImportSegment>(segment);
        }

        public static AndConstraint<OperationSegment> ShouldHaveConstantParameter<TValue>(this OperationSegment segment, string name, TValue value)
        {
            OperationSegmentParameter parameter = segment.Parameters.SingleOrDefault(p => p.Name == name);
            parameter.Should().NotBeNull();
            parameter.ShouldBeConstantParameterWithValueType(name, value);
            return new AndConstraint<OperationSegment>(segment);
        }

        public static AndConstraint<OperationImportSegment> ShouldHaveConstantParameter<TValue>(this OperationImportSegment segment, string name, TValue value)
        {
            OperationSegmentParameter parameter = segment.Parameters.SingleOrDefault(p => p.Name == name);
            parameter.Should().NotBeNull();
            parameter.ShouldBeConstantParameterWithValueType(name, value);
            return new AndConstraint<OperationImportSegment>(segment);
        }

        public static AndConstraint<object> ShouldBeConstantParameterWithValueType<TValue>(this OperationSegmentParameter parameter, string name, TValue value)
        {
            parameter.Name.Should().Be(name);
            ConstantNode constantNode = parameter.Value.As<ConstantNode>();
            if (value == null)
            {
                constantNode.Value.Should().BeNull();
            }
            else
            {
                if (typeof(TValue).IsPrimitive || typeof(TValue) == typeof(decimal))
                {
                    // for int value --> long TValue
                    TValue tmp = (TValue)Convert.ChangeType(constantNode.Value, typeof(TValue));
                    tmp.Should().NotBeNull();
                    tmp.ShouldBeEquivalentTo(value);
                }
                else
                {
                    constantNode.Value.Should().BeAssignableTo<TValue>();
                    constantNode.Value.As<TValue>().ShouldBeEquivalentTo(value);
                }
            }

            return new AndConstraint<object>(constantNode.Value);
        }

        public static AndConstraint<T> ShouldHaveValueType<T>(this OperationSegmentParameter parameter, string name)
        {
            parameter.Name.Should().Be(name);
            parameter.Value.Should().BeAssignableTo<T>();
            return new AndConstraint<T>((T)parameter.Value);
        }

        public static AndConstraint<T> ShouldBeConstantParameterWithValueType<T>(this OperationSegmentParameter parameter, string name)
        {
            parameter.Name.Should().Be(name);
            object val = parameter.Value.As<ConstantNode>().Value;
            val.Should().BeAssignableTo<T>();
            return new AndConstraint<T>((T)val);
        }

        public static AndConstraint<OperationSegment> ShouldHaveSegmentOfParameterAliasNode(this OperationSegment segment, string name, string alias, IEdmTypeReference typeReference = null)
        {
            OperationSegmentParameter parameter = segment.Parameters.SingleOrDefault(p => p.Name == name);
            parameter.Should().NotBeNull();
            parameter.ShouldHaveParameterAliasNode(name, alias, typeReference);
            return new AndConstraint<OperationSegment>(segment);
        }

        public static AndConstraint<ParameterAliasNode> ShouldHaveParameterAliasNode(this OperationSegmentParameter parameter, string name, string alias, IEdmTypeReference typeReference = null)
        {
            parameter.Name.Should().Be(name);
            var node = parameter.Value.As<ParameterAliasNode>();
            node.Alias.Should().Be(alias);
            if (typeReference == null)
            {
                node.TypeReference.Should().BeNull();
            }
            else
            {
                node.TypeReference.FullName().Should().Be(typeReference.FullName());
            }

            return new AndConstraint<ParameterAliasNode>(node);
        }

        public static AndConstraint<ParameterAliasNode> ShouldHaveParameterAliasNode(this NamedFunctionParameterNode parameter, string name, string alias)
        {
            parameter.Name.Should().Be(name);
            var token = parameter.Value.As<ParameterAliasNode>();
            token.Alias.Should().Be(alias);
            return new AndConstraint<ParameterAliasNode>(token);
        }
    }
}
