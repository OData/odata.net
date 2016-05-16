//---------------------------------------------------------------------
// <copyright file="ComplexValueMaterializationPolicyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client.Materialization;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DSClient = Microsoft.OData.Client;

    [TestClass]
    public class ComplexValueMaterializationPolicyTests
    {
        [TestMethod]
        public void ComplexWithPrimitiveValueShouldMaterialize()
        {
            ODataComplexValue pointComplexValue = new ODataComplexValue() { Properties = new ODataProperty[] { new ODataProperty() { Name = "X", Value = 15 }, new ODataProperty() { Name = "Y", Value = 18 } } };
            this.CreatePrimitiveValueMaterializationPolicy().MaterializeComplexTypeProperty(typeof(CollectionValueMaterializationPolicyTests.Point), pointComplexValue);
            pointComplexValue.HasMaterializedValue().Should().BeTrue();
            var point = pointComplexValue.GetMaterializedValue().As<CollectionValueMaterializationPolicyTests.Point>();
            point.X.Should().Be(15);
            point.Y.Should().Be(18);
        }

        [TestMethod]
        public void ApplyNonExistantPropertyWithIgnoreMissingPropertiesShouldNotError()
        {
            TestMaterializerContext context = new TestMaterializerContext() { UndeclaredPropertyBehavior = DSClient.UndeclaredPropertyBehavior.Support };
            CollectionValueMaterializationPolicyTests.Point point = new CollectionValueMaterializationPolicyTests.Point();
            ODataProperty property = new ODataProperty() { Name = "Z", Value = 10 };

            this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(CollectionValueMaterializationPolicyTests.Point), null), property, point);
        }

        [TestMethod]
        public void ApplyNullOnCollectionPropertyShouldError()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = null };

            Action test = () => this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.Collection_NullCollectionNotSupported(property.Name));
        }

        [TestMethod]
        public void ApplyStringValueForCollectionPropertyShouldError()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = "foo" };

            Action test = () => this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.Deserialize_MixedTextWithComment);
        }

        [TestMethod]
        public void ApplyODataComplexValueForCollectionPropertyShouldError()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = new ODataComplexValue() };

            Action test = () => this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.AtomMaterializer_InvalidCollectionItem(property.Name));
        }

        [TestMethod]
        public void ApplyODataDerivedComplexValueForBaseComplexTypeProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithChildComplexType complexInstance = new ComplexTypeWithChildComplexType();
            complexInstance.InnerComplexProperty = new DerivedComplexType { DerivedProp = 1 };

            //In a true client, a TypeResolver will be used to resolve derived property type.
            context.ResolveTypeForMaterializationOverrideFunc = (Type type, string name) =>
            {
                if (name == "DerivedComplexType")
                {
                    var edmType = context.Model.GetOrCreateEdmType(typeof(DerivedComplexType));
                    return new ClientTypeAnnotation(edmType, typeof(DerivedComplexType), "DerivedComplexType", context.Model);
                }
                else
                {
                    var edmType = context.Model.GetOrCreateEdmType(typeof(ComplexTypeWithChildComplexType));
                    return new ClientTypeAnnotation(edmType, typeof(ComplexTypeWithChildComplexType), "ComplexTypeWithChildComplexType", context.Model);
                }
            };

            IEdmType t = context.Model.GetOrCreateEdmType(typeof(DerivedComplexType));
            ODataProperty property = new ODataProperty() { Name = "InnerComplexProperty", Value = new ODataComplexValue { TypeName = "DerivedComplexType", Properties = new ODataProperty[] { new ODataProperty { Name = "DerivedProp", Value = 1 } } } };

            Action test = () => this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithChildComplexType), null), property, complexInstance);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ApplyODataCollectionValueToNonNullExistingCollectionProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            complexInstance.Strings.Add("ShouldBeCleared");
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = new ODataCollectionValue() };

            this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            complexInstance.Strings.Should().HaveCount(0);
        }

        [TestMethod]
        public void ApplyODataCollectionValueToNullCollectionProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            complexInstance.Strings = null;
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = new ODataCollectionValue() { Items = new string[] { "foo" }, TypeName = typeof(ComplexTypeWithPrimitiveCollection).FullName } };

            this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            complexInstance.Strings.Should().HaveCount(1);
            complexInstance.Strings[0].Should().Be("foo");
        }

        [TestMethod]
        public void ValueShouldBeAppliedRegardlessIfPropertyStartsNullOrNot()
        {
            foreach (var startingPropertyState in new ChildComplexType[] { null, new ChildComplexType() })
            {
                TestMaterializerContext context = new TestMaterializerContext();
                ComplexTypeWithChildComplexType complexInstance = new ComplexTypeWithChildComplexType();
                complexInstance.InnerComplexProperty = startingPropertyState;
                ODataProperty property = new ODataProperty() { Name = "InnerComplexProperty", Value = new ODataComplexValue() { Properties = new ODataProperty[] { new ODataProperty() { Name = "Prop", Value = 1 } } } };

                this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithChildComplexType), null), property, complexInstance);
                complexInstance.InnerComplexProperty.Prop.Should().Be(1);
            }
        }

        [TestMethod]
        public void NullValueShouldBeAppliedToSubComplexValueProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithChildComplexType complexInstance = new ComplexTypeWithChildComplexType();
            complexInstance.InnerComplexProperty = new ChildComplexType();
            ODataProperty property = new ODataProperty() { Name = "InnerComplexProperty", Value = null };

            this.CreatePrimitiveValueMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithChildComplexType), null), property, complexInstance);
            Assert.IsNull(complexInstance.InnerComplexProperty);
        }

        internal ComplexValueMaterializationPolicy CreatePrimitiveValueMaterializationPolicy()
        {
            TestMaterializerContext context = new TestMaterializerContext() { Context = new DSClient.DataServiceContext() };
            return CreatePrimitiveValueMaterializationPolicy(context);
        }

        internal ComplexValueMaterializationPolicy CreatePrimitiveValueMaterializationPolicy(TestMaterializerContext context)
        {
            var lazyPrimitivePropertyConverter = new DSClient.SimpleLazy<PrimitivePropertyConverter>(() => new PrimitivePropertyConverter());
            var primitiveValueMaterializerPolicy = new PrimitiveValueMaterializationPolicy(context, lazyPrimitivePropertyConverter);
            var complexPolicy = new ComplexValueMaterializationPolicy(context, lazyPrimitivePropertyConverter);
            var collectionPolicy = new CollectionValueMaterializationPolicy(context, primitiveValueMaterializerPolicy);
            var intanceAnnotationPolicy = new InstanceAnnotationMaterializationPolicy(context);

            collectionPolicy.ComplexValueMaterializationPolicy = complexPolicy;
            complexPolicy.CollectionValueMaterializationPolicy = collectionPolicy;
            complexPolicy.InstanceAnnotationMaterializationPolicy = intanceAnnotationPolicy;

            return complexPolicy;
        }

        public class ComplexTypeWithPrimitiveCollection
        {
            public ComplexTypeWithPrimitiveCollection()
            {
                this.Strings = new List<string>();
            }

            public List<string> Strings { get; set; }
        }

        public class ComplexTypeWithChildComplexType
        {
            public ChildComplexType InnerComplexProperty { get; set; }
        }

        public class ChildComplexType
        {
            public int Prop { get; set; }
        }

        public class DerivedComplexType : ChildComplexType
        {
            public int DerivedProp { get; set; }
        }
    }
}