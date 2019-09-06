//---------------------------------------------------------------------
// <copyright file="SegmentAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Contains fluent assertion APIs for testing Segments
    /// </summary>
    internal static class SegmentAssertions
    {
        public static CountSegment ShouldBeCountSegment(this ODataPathSegment segment)
        {
            Assert.NotNull(segment);
            return Assert.IsType<CountSegment>(segment);
        }

        public static FilterSegment ShouldBeFilterSegment(this ODataPathSegment segment)
        {
            Assert.NotNull(segment);
            return Assert.IsType<FilterSegment>(segment);
        }

        public static EachSegment ShouldBeEachSegment(this ODataPathSegment segment)
        {
            Assert.NotNull(segment);
            return Assert.IsType<EachSegment>(segment);
        }

        public static BatchSegment ShouldBeBatchSegment(this ODataPathSegment segment)
        {
            Assert.NotNull(segment);
            return Assert.IsType<BatchSegment>(segment);
        }

        public static NavigationPropertyLinkSegment ShouldBeNavigationPropertyLinkSegment(this ODataPathSegment segment, IEdmNavigationProperty navigationProperty)
        {
            Assert.NotNull(segment);
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = Assert.IsType<NavigationPropertyLinkSegment>(segment);
            Assert.Same(navigationProperty, navigationPropertyLinkSegment.NavigationProperty);
            return navigationPropertyLinkSegment;
        }

        public static MetadataSegment ShouldBeMetadataSegment(this ODataPathSegment segment)
        {
            Assert.NotNull(segment);
            return Assert.IsType<MetadataSegment>(segment);
        }

        public static ValueSegment ShouldBeValueSegment(this ODataPathSegment segment)
        {
            Assert.NotNull(segment);
            return Assert.IsType<ValueSegment>(segment);
        }

        public static AndConstraint<TypeSegment> ShouldBeTypeSegment(this ODataPathSegment segment, IEdmType type)
        {
            segment.Should().BeOfType<TypeSegment>();
            TypeSegment typeSegment = segment.As<TypeSegment>();
            typeSegment.EdmType.ShouldBeEquivalentTo(type);
            return new AndConstraint<TypeSegment>(typeSegment);
        }

        public static AndConstraint<TypeSegment> ShouldBeTypeSegment(this ODataPathSegment segment, IEdmType actualType, IEdmType expectType)
        {
            segment.Should().BeOfType<TypeSegment>();
            TypeSegment typeSegment = segment.As<TypeSegment>();
            typeSegment.EdmType.ShouldBeEquivalentTo(actualType);
            typeSegment.TargetEdmType.ShouldBeEquivalentTo(expectType);
            return new AndConstraint<TypeSegment>(typeSegment);
        }

        public static AndConstraint<PropertySegment> ShouldBePropertySegment(this ODataPathSegment segment, IEdmProperty expectedProperty)
        {
            segment.Should().BeOfType<PropertySegment>();
            PropertySegment propertySegment = segment.As<PropertySegment>();
            propertySegment.Property.Should().Be(expectedProperty);
            return new AndConstraint<PropertySegment>(propertySegment);
        }

        public static AndConstraint<AnnotationSegment> ShouldBeAnnotationSegment(this ODataPathSegment segment, IEdmTerm expectedTerm)
        {
            segment.Should().BeOfType<AnnotationSegment>();
            AnnotationSegment annotationSegment = segment.As<AnnotationSegment>();
            annotationSegment.Term.Should().Be(expectedTerm);
            return new AndConstraint<AnnotationSegment>(annotationSegment);
        }

        public static AndConstraint<NavigationPropertySegment> ShouldBeNavigationPropertySegment(this ODataPathSegment segment, IEdmNavigationProperty navigationProperty)
        {
            segment.Should().BeOfType<NavigationPropertySegment>();
            NavigationPropertySegment navPropSegment = segment.As<NavigationPropertySegment>();
            navPropSegment.NavigationProperty.Should().Be(navigationProperty);
            return new AndConstraint<NavigationPropertySegment>(navPropSegment);
        }

        public static AndConstraint<ReferenceSegment> ShouldBeReferenceSegment(this ODataPathSegment segment, IEdmNavigationSource navigationSource)
        {
            segment.Should().BeOfType<ReferenceSegment>();
            ReferenceSegment referenceSegment = segment.As<ReferenceSegment>();
            referenceSegment.TargetEdmNavigationSource.Should().Be(navigationSource);
            return new AndConstraint<ReferenceSegment>(referenceSegment);
        }

        public static AndConstraint<DynamicPathSegment> ShouldBeDynamicPathSegment(this ODataPathSegment segment, string identifier)
        {
            segment.Should().BeOfType<DynamicPathSegment>();
            DynamicPathSegment openPropertySegment = segment.As<DynamicPathSegment>();
            openPropertySegment.Identifier.Should().Be(identifier);
            return new AndConstraint<DynamicPathSegment>(openPropertySegment);
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

        public static BatchReferenceSegment ShouldBeBatchReferenceSegment(this ODataPathSegment segment, IEdmType resultingType)
        {
            Assert.NotNull(segment);
            BatchReferenceSegment batchReferenceSegment = Assert.IsType<BatchReferenceSegment>(segment);
            Assert.Same(resultingType, batchReferenceSegment.EdmType);
            return batchReferenceSegment;
        }

        public static EntitySetSegment ShouldBeEntitySetSegment(this ODataPathSegment segment, IEdmEntitySet entitySet)
        {
            Assert.NotNull(segment);
            EntitySetSegment entitySetSegment = Assert.IsType<EntitySetSegment>(segment);
            Assert.Same(entitySet, entitySetSegment.EntitySet);
            return entitySetSegment;
        }

        public static SingletonSegment ShouldBeSingletonSegment(this ODataPathSegment segment, IEdmSingleton singleton)
        {
            Assert.NotNull(segment);
            SingletonSegment singletonSegment = Assert.IsType<SingletonSegment>(segment);
            Assert.Same(singleton, singletonSegment.Singleton);
            return singletonSegment;
        }

        public static KeySegment ShouldBeKeySegment(this ODataPathSegment segment, params KeyValuePair<string, object>[] keys)
        {
            Assert.NotNull(segment);
            KeySegment keySegment = Assert.IsType<KeySegment>(segment);
            keySegment.Keys.ContainExactly(keys);
            return keySegment;
        }

        public static KeySegment ShouldBeSimpleKeySegment(this ODataPathSegment segment, object value)
        {
            Assert.NotNull(segment);
            KeySegment keySegment = Assert.IsType<KeySegment>(segment);
            var key = Assert.Single(keySegment.Keys);
            Assert.Equal(value, key.Value);
            return keySegment;
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
                if (typeof(TValue).IsPrimitive() || typeof(TValue) == typeof(decimal))
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

        public static AndConstraint<ConvertNode> ShouldHaveConvertNode(this OperationSegmentParameter parameter,
            string name, IEdmTypeReference typeReference = null)
        {
            parameter.Name.Should().Be(name);
            var node = parameter.Value.As<ConvertNode>();
            if (typeReference == null)
            {
                node.TypeReference.Should().BeNull();
            }
            else
            {
                node.TypeReference.FullName().Should().Be(typeReference.FullName());
            }

            return new AndConstraint<ConvertNode>(node);
        }
    }
}
