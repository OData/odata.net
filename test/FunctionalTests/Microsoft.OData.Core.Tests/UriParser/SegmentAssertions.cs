//---------------------------------------------------------------------
// <copyright file="SegmentAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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

        public static TypeSegment ShouldBeTypeSegment(this ODataPathSegment segment, IEdmType type)
        {
            Assert.NotNull(segment);
            TypeSegment typeSegment = Assert.IsType<TypeSegment>(segment);
            Assert.True(typeSegment.EdmType.IsEquivalentTo(type));
            return typeSegment;
        }

        public static TypeSegment ShouldBeTypeSegment(this ODataPathSegment segment, IEdmType actualType, IEdmType expectType)
        {
            Assert.NotNull(segment);
            TypeSegment typeSegment = Assert.IsType<TypeSegment>(segment);
            Assert.Equal(typeSegment.EdmType.FullTypeName(), actualType.FullTypeName());
            Assert.Equal(typeSegment.ExpectedType.FullTypeName(), expectType.FullTypeName());
            return typeSegment;
        }

        public static PropertySegment ShouldBePropertySegment(this ODataPathSegment segment, IEdmProperty expectedProperty)
        {
            Assert.NotNull(segment);
            PropertySegment propertySegment = Assert.IsType<PropertySegment>(segment);
            Assert.Same(expectedProperty, propertySegment.Property);
            return propertySegment;
        }

        public static AnnotationSegment ShouldBeAnnotationSegment(this ODataPathSegment segment, IEdmTerm expectedTerm)
        {
            Assert.NotNull(segment);
            AnnotationSegment annotationSegment = Assert.IsType<AnnotationSegment>(segment);
            Assert.Equal(expectedTerm, annotationSegment.Term);
            return annotationSegment;
        }

        public static NavigationPropertySegment ShouldBeNavigationPropertySegment(this ODataPathSegment segment, IEdmNavigationProperty navigationProperty)
        {
            Assert.NotNull(segment);
            NavigationPropertySegment navPropSegment = Assert.IsType<NavigationPropertySegment>(segment);
            Assert.Same(navPropSegment.NavigationProperty, navigationProperty);
            return navPropSegment;
        }

        public static ReferenceSegment ShouldBeReferenceSegment(this ODataPathSegment segment, IEdmNavigationSource navigationSource)
        {
            Assert.NotNull(segment);
            ReferenceSegment referenceSegment = Assert.IsType<ReferenceSegment>(segment);
            Assert.Same(referenceSegment.TargetEdmNavigationSource, navigationSource);
            return referenceSegment;
        }

        public static DynamicPathSegment ShouldBeDynamicPathSegment(this ODataPathSegment segment, string identifier)
        {
            Assert.NotNull(segment);
            DynamicPathSegment openPropertySegment = Assert.IsType<DynamicPathSegment>(segment);
            Assert.Equal(identifier, openPropertySegment.Identifier);
            return openPropertySegment;
        }

        public static OperationImportSegment ShouldBeOperationImportSegment(this ODataPathSegment segment, params IEdmOperationImport[] operationImports)
        {
            Assert.NotNull(segment);
            OperationImportSegment operationImportSegment = Assert.IsType<OperationImportSegment>(segment);
            operationImportSegment.OperationImports.ContainExactly(operationImports);
            return operationImportSegment;
        }

        public static OperationSegment ShouldBeOperationSegment(this ODataPathSegment segment, params IEdmOperation[] operations)
        {
            Assert.NotNull(segment);
            OperationSegment operationSegment = Assert.IsType<OperationSegment>(segment);
            operationSegment.Operations.ContainExactly(operations);
            return operationSegment;
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

        public static OperationSegment ShouldHaveParameterCount(this OperationSegment segment, int count)
        {
            Assert.NotNull(segment);
            Assert.Equal(count, segment.Parameters.Count());
            return segment;
        }

        public static OperationImportSegment ShouldHaveParameterCount(this OperationImportSegment segment, int count)
        {
            Assert.NotNull(segment);
            Assert.NotNull(segment.Parameters);
            Assert.Equal(count, segment.Parameters.Count());
            return segment;
        }

        public static OperationSegment ShouldHaveConstantParameter<TValue>(this OperationSegment segment, string name, TValue value)
        {
            Assert.NotNull(segment);
            OperationSegmentParameter parameter = segment.Parameters.SingleOrDefault(p => p.Name == name);
            Assert.NotNull(parameter);
            parameter.ShouldBeConstantParameterWithValueType(name, value);
            return segment;
        }

        public static OperationImportSegment ShouldHaveConstantParameter<TValue>(this OperationImportSegment segment, string name, TValue value)
        {
            Assert.NotNull(segment);
            OperationSegmentParameter parameter = segment.Parameters.SingleOrDefault(p => p.Name == name);
            Assert.NotNull(parameter);
            parameter.ShouldBeConstantParameterWithValueType(name, value);
            return segment;
        }

        public static object ShouldBeConstantParameterWithValueType<TValue>(this OperationSegmentParameter parameter, string name, TValue value)
        {
            Assert.Equal(name, parameter.Name);
            ConstantNode constantNode = Assert.IsType<ConstantNode>(parameter.Value);
            if (value == null)
            {
                Assert.Null(constantNode.Value);
            }
            else
            {
                if (typeof(TValue).IsPrimitive() || typeof(TValue) == typeof(decimal))
                {
                    // for int value --> long TValue
                    TValue tmp = (TValue)Convert.ChangeType(constantNode.Value, typeof(TValue));
                    Assert.NotNull(tmp);
                    Assert.Equal(value, tmp);
                }
                else if (typeof(TValue) == typeof(UriTemplateExpression))
                {
                    UriTemplateExpression actual = Assert.IsType<UriTemplateExpression>(constantNode.Value);
                    UriTemplateExpression expect = Assert.IsType<UriTemplateExpression>(value);
                    Assert.Equal(expect.LiteralText, actual.LiteralText);
                    Assert.Equal(expect.ExpectedType.FullName(), expect.ExpectedType.FullName());
                }
                else
                {
                    constantNode.Value.GetType().IsAssignableFrom(typeof(TValue));
                    Assert.Equal(value, constantNode.Value);
                }
            }

            return constantNode.Value;
        }

        public static T ShouldHaveValueType<T>(this OperationSegmentParameter parameter, string name)
        {
            Assert.Equal(name, parameter.Name);
            Assert.IsType<T>(parameter.Value);
            return (T)parameter.Value;
        }

        public static T ShouldBeConstantParameterWithValueType<T>(this OperationSegmentParameter parameter, string name)
        {
            Assert.Equal(name, parameter.Name);
            object val = Assert.IsType<ConstantNode>(parameter.Value).Value;
            Assert.IsType<T>(val);
            return (T)val;
        }

        public static OperationSegment ShouldHaveSegmentOfParameterAliasNode(this OperationSegment segment, string name, string alias, IEdmTypeReference typeReference = null)
        {
            Assert.NotNull(segment);
            OperationSegmentParameter parameter = segment.Parameters.SingleOrDefault(p => p.Name == name);
            Assert.NotNull(parameter);
            parameter.ShouldHaveParameterAliasNode(name, alias, typeReference);
            return segment;
        }

        public static ParameterAliasNode ShouldHaveParameterAliasNode(this OperationSegmentParameter parameter, string name, string alias, IEdmTypeReference typeReference = null)
        {
            Assert.Equal(name, parameter.Name);
            var node = Assert.IsType<ParameterAliasNode>(parameter.Value);
            Assert.Equal(alias, node.Alias);
            if (typeReference == null)
            {
                Assert.Null(node.TypeReference);
            }
            else
            {
                Assert.Equal(node.TypeReference.FullName(), typeReference.FullName());
            }

            return node;
        }

        public static ParameterAliasNode ShouldHaveParameterAliasNode(this NamedFunctionParameterNode parameter, string name, string alias)
        {
            Assert.Equal(name, parameter.Name);
            var token = Assert.IsType<ParameterAliasNode>(parameter.Value);
            Assert.Equal(alias, token.Alias);
            return token;
        }

        public static ConvertNode ShouldHaveConvertNode(this OperationSegmentParameter parameter,
            string name, IEdmTypeReference typeReference = null)
        {
            Assert.Equal(name, parameter.Name);
            var node = Assert.IsType<ConvertNode>(parameter.Value);
            if (typeReference == null)
            {
                Assert.Null(node.TypeReference);
            }
            else
            {
                Assert.Equal(node.TypeReference.FullName(), typeReference.FullName());
            }

            return node;
        }
    }
}
