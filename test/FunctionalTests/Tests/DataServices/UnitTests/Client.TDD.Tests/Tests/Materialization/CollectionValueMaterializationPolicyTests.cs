//---------------------------------------------------------------------
// <copyright file="CollectionValueMaterializationPolicyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DSClient = Microsoft.OData.Client;

    [TestClass]
    public class CollectionValueMaterializationPolicyTests
    {
        [TestMethod]
        public void ShouldMaterializeIntCollection()
        {
            var primitiveValues = new List<int>(new int[] {1, 5, 10});
            var outputCollection = new List<int>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                primitiveValues, "Edm.Int32", outputCollection, typeof(int), addToDelegate, false);
            outputCollection.Should().HaveCount(3);
            outputCollection[0].Should().Be(1);
            outputCollection[1].Should().Be(5);
            outputCollection[2].Should().Be(10);
        }

        [TestMethod]
        public void ShouldMaterializeNullableIntCollection()
        {
            var primitiveValues = new List<int?>(new int?[] { 1, null, 10 });
            var outputCollection = new List<int?>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                primitiveValues, "Edm.Int32", outputCollection, typeof(int?), addToDelegate, true);
            outputCollection.Should().HaveCount(3);
            outputCollection[0].Should().Be(1);
            outputCollection[1].Should().Be(null);
            outputCollection[2].Should().Be(10);
        }

        [TestMethod]
        public void ShouldMaterializeDateCollection()
        {
            var primitiveValues = new List<Date>(new Date[] {Date.MinValue, new Date(2014, 9, 28), Date.MaxValue});
            var outputCollection = new List<Date>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                primitiveValues, "Edm.Date", outputCollection, typeof(Date), addToDelegate, false);
            outputCollection.Should().HaveCount(3);
            outputCollection[0].Should().Be(Date.MinValue);
            outputCollection[1].Should().Be(new Date(2014, 9, 28));
            outputCollection[2].Should().Be(Date.MaxValue);
        }

        [TestMethod]
        public void ShouldMaterializeTimeOfDayCollection()
        {
            var primitiveValues = new List<TimeOfDay>(new TimeOfDay[] { TimeOfDay.MinValue, new TimeOfDay(19, 9, 28, 123), TimeOfDay.MaxValue });
            var outputCollection = new List<TimeOfDay>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                primitiveValues, "Edm.TimeOfDay", outputCollection, typeof(TimeOfDay), addToDelegate, false);
            outputCollection.Should().HaveCount(3);
            outputCollection[0].Should().Be(TimeOfDay.MinValue);
            outputCollection[1].Should().Be(new TimeOfDay(19, 9, 28, 123));
            outputCollection[2].Should().Be(TimeOfDay.MaxValue);
        }

        [TestMethod]
        public void ShouldMaterializeComplexCollection()
        {
            var primitiveValues = new List<ODataComplexValue>(new ODataComplexValue[]
            {
                new ODataComplexValue(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = 15}, new ODataProperty(){Name="Y", Value = 18} }}, 
                new ODataComplexValue(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = 22}, new ODataProperty(){Name="Y", Value = 25} }}, 
                new ODataComplexValue(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = -100}, new ODataProperty(){Name="Y", Value = -201} }},
            });

            var outputCollection = new List<Point>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                primitiveValues, "Point", outputCollection, typeof(Point), addToDelegate, false);
            outputCollection.Should().HaveCount(3);
            outputCollection[0].X.Should().Be(15);
            outputCollection[0].Y.Should().Be(18);
            outputCollection[1].X.Should().Be(22);
            outputCollection[1].Y.Should().Be(25);
            outputCollection[2].X.Should().Be(-100);
            outputCollection[2].Y.Should().Be(-201);
        }

        [TestMethod]
        public void ShouldMaterializeNullableComplexCollection()
        {
            var primitiveValues = new List<ODataComplexValue>(new[]
            {
                new ODataComplexValue(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = 15}, new ODataProperty(){Name="Y", Value = 18} }}, 
                null, 
                new ODataComplexValue(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = -100}, new ODataProperty(){Name="Y", Value = -201} }},
            });

            var outputCollection = new List<Point>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                primitiveValues, "Point", outputCollection, typeof(Point), addToDelegate, true);
            outputCollection.Should().HaveCount(3);
            outputCollection[0].X.Should().Be(15);
            outputCollection[0].Y.Should().Be(18);
            outputCollection[1].Should().BeNull();
            outputCollection[2].X.Should().Be(-100);
            outputCollection[2].Y.Should().Be(-201);
        }

        [TestMethod]
        public void ShouldMaterializeConcreteComplexCollectionDeclaredAsAbstract()
        {
            var complexValues = new List<ODataComplexValue>(new[]
            {
                new ODataComplexValue(){Properties = new ODataProperty[]{ new ODataProperty(){Name="Points", Value = 0}, new ODataProperty(){Name="Diameter", Value = 15} }}, 
                new ODataComplexValue(){Properties = new ODataProperty[]{ new ODataProperty(){Name="Points", Value = 0}, new ODataProperty(){Name="Diameter", Value = 18} }},
            });

            var testContext = new TestMaterializerContext();
            testContext.ResolveTypeForMaterializationOverrideFunc = (Type type, string name) =>
            {
                var edmType = testContext.Model.GetOrCreateEdmType(typeof(Circle));
                return new ClientTypeAnnotation(edmType, typeof(Circle), "Circle", testContext.Model);
            };

            var outputCollection = new List<Shape>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            this.CreateCollectionValueMaterializationPolicy(testContext).ApplyCollectionDataValues(
                complexValues, "Shape", outputCollection, typeof(Shape), addToDelegate, true);
            outputCollection.Should().HaveCount(2);
            outputCollection[0].Points.Should().Be(0);
            ((Circle)outputCollection[0]).Diameter.Should().Be(15);
            outputCollection[1].Points.Should().Be(0);
            ((Circle)outputCollection[1]).Diameter.Should().Be(18);
        }

        [TestMethod] public void NullComplexValueShouldFail()
        {
            var primitiveValues = new List<ODataComplexValue>(new ODataComplexValue[] { null });

            var outputCollection = new List<Point>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            Action test =
                () =>
                this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                    primitiveValues, "Point", outputCollection, typeof(Point), addToDelegate, false);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.Collection_NullCollectionItemsNotSupported);
        }

        [TestMethod]
        public void AddingPrimitiveValueToComplexCollectionShouldFail()
        {
            var primitiveValues = new List<object>(new object[]{ 1 });

            var outputCollection = new List<Point>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            Action test =
                () =>
                this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                    primitiveValues, "Point", outputCollection, typeof(Point), addToDelegate, false);
            test.ShouldThrow<InvalidOperationException>(DSClient.Strings.Collection_PrimitiveTypesInCollectionOfComplexTypesNotAllowed);
        }

        [TestMethod]
        public void AddingComplexToPrimitiveCollectionShouldFail()
        {
            var complexValues = new List<object>(new object[] { new ODataComplexValue() { Properties = new ODataProperty[] { new ODataProperty() { Name = "X", Value = 15 }, new ODataProperty() { Name = "Y", Value = 18 } } } });

            var outputCollection = new List<int>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            Action test =
                () =>
                this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                    complexValues, "Edm.Int32", outputCollection, typeof(int), addToDelegate, false);
            test.ShouldThrow<InvalidOperationException>(DSClient.Strings.Collection_ComplexTypesInCollectionOfPrimitiveTypesNotAllowed);
        }

        [TestMethod]
        public void AddingCollectionToPrimitiveCollectionShouldFail()
        {
            var collectionValues = new List<object>(new object[] { new ODataCollectionValue() });

            var outputCollection = new List<int>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            Action test =
                () =>
                this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                    collectionValues, "Edm.Int32", outputCollection, typeof(int), addToDelegate, false);
            test.ShouldThrow<InvalidOperationException>(DSClient.Strings.Collection_ComplexTypesInCollectionOfPrimitiveTypesNotAllowed);
        }

        [TestMethod]
        public void AddingCollectionToComplexCollectionShouldFail()
        {
            var collectionValues = new List<object>(new object[] { new ODataCollectionValue() });

            var outputCollection = new List<Point>();
            var addToDelegate = ClientTypeUtil.GetAddToCollectionDelegate(outputCollection.GetType());

            Action test =
                () =>
                this.CreateCollectionValueMaterializationPolicy().ApplyCollectionDataValues(
                    collectionValues, "Point", outputCollection, typeof(Point), addToDelegate, false);
            
            // Error message is a little off we could improve this
            test.ShouldThrow<InvalidOperationException>(DSClient.Strings.Collection_ComplexTypesInCollectionOfPrimitiveTypesNotAllowed);
        }

        [TestMethod]
        public void DataServicCollectionOfTAsCollectionTypeShouldFailForPrimitiveOrComplexCollections()
        {
            var testContext = new TestMaterializerContext();
            var edmType = testContext.Model.GetOrCreateEdmType(typeof(MyInfo));
            var clientTypeAnnotation = new ClientTypeAnnotation(edmType, typeof(MyInfo), "MyInfo", testContext.Model);
            
            Action test = () => this.CreateCollectionValueMaterializationPolicy(testContext).CreateCollectionInstance((IEdmCollectionTypeReference)clientTypeAnnotation.EdmTypeReference, clientTypeAnnotation.ElementType);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.AtomMaterializer_DataServiceCollectionNotSupportedForNonEntities);
        }

        [TestMethod]
        public void CreateCollectionInstanceShouldFailOnTypeWithNoParametersLessConstructors()
        {
            var testContext = new TestMaterializerContext();
            var edmType = testContext.Model.GetOrCreateEdmType(typeof(ListWithNoEmptyConstructors));
            var clientTypeAnnotation = new ClientTypeAnnotation(edmType, typeof(ListWithNoEmptyConstructors), "Points", testContext.Model);

            Action test = () => this.CreateCollectionValueMaterializationPolicy(testContext).CreateCollectionInstance((IEdmCollectionTypeReference)clientTypeAnnotation.EdmTypeReference, clientTypeAnnotation.ElementType);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.AtomMaterializer_MaterializationTypeError(clientTypeAnnotation.ElementType.FullName));
        }

        [TestMethod]
        public void CreateCollectionPropertyInstanceShouldFailOnTypeWithNoParametersLessConstructors()
        {      
            var odataProperty = new ODataProperty() {Name = "foo", Value = new ODataCollectionValue(){TypeName = "Points"}};
            var testContext = new TestMaterializerContext();
            testContext.ResolveTypeForMaterializationOverrideFunc = (Type type, string name) =>
            {
                var edmType = testContext.Model.GetOrCreateEdmType(typeof(ListWithNoEmptyConstructors));
                return new ClientTypeAnnotation(edmType, typeof(ListWithNoEmptyConstructors), "ListWithNoEmptyConstructors", testContext.Model);
            };

            Action test = () => this.CreateCollectionValueMaterializationPolicy(testContext).CreateCollectionPropertyInstance(odataProperty, typeof(ListWithNoEmptyConstructors));
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.AtomMaterializer_NoParameterlessCtorForCollectionProperty("foo", typeof(ListWithNoEmptyConstructors).Name));
        }

        [TestMethod]
        public void NonMissingMethodExceptionOnCreateInstanceShouldNotBeCaught()
        {
            var testContext = new TestMaterializerContext();
            var edmType = testContext.Model.GetOrCreateEdmType(typeof(ListWithNoEmptyConstructors));
            var clientTypeAnnotation = new ClientTypeAnnotation(edmType, typeof(ErrorThrowingList), "Points", testContext.Model);

            Action test = () => this.CreateCollectionValueMaterializationPolicy(testContext).CreateCollectionInstance((IEdmCollectionTypeReference)clientTypeAnnotation.EdmTypeReference, clientTypeAnnotation.ElementType);
            test.ShouldThrow<TargetInvocationException>().WithInnerException<ApplicationException>().WithInnerMessage("foo");
        }

        internal CollectionValueMaterializationPolicy CreateCollectionValueMaterializationPolicy()
        {
            return CreateCollectionValueMaterializationPolicy(new TestMaterializerContext());
        }

        internal CollectionValueMaterializationPolicy CreateCollectionValueMaterializationPolicy(IODataMaterializerContext materializerContext)
        {
            var lazyPrimitivePropertyConverter = new DSClient.SimpleLazy<PrimitivePropertyConverter>(() => new PrimitivePropertyConverter());
            var primitiveValueMaterializerPolicy = new PrimitiveValueMaterializationPolicy(materializerContext, lazyPrimitivePropertyConverter);
            var complexPolicy = new ComplexValueMaterializationPolicy(materializerContext, lazyPrimitivePropertyConverter);
            var collectionPolicy = new CollectionValueMaterializationPolicy(materializerContext, primitiveValueMaterializerPolicy);
            var instanceAnnotationPolicy = new InstanceAnnotationMaterializationPolicy(materializerContext);

            collectionPolicy.ComplexValueMaterializationPolicy = complexPolicy;
            collectionPolicy.InstanceAnnotationMaterializationPolicy = instanceAnnotationPolicy;
            complexPolicy.CollectionValueMaterializationPolicy = collectionPolicy;
            complexPolicy.InstanceAnnotationMaterializationPolicy = instanceAnnotationPolicy;

            return collectionPolicy;
        }

        public class ErrorThrowingList : List<Point>
        {
            public ErrorThrowingList()
            {
                throw new ApplicationException("foo");
            }
        }

        public class ListWithNoEmptyConstructors : List<Point>
        {
            public ListWithNoEmptyConstructors(Point firstPoint)
            {
            }
        }

        public class MyInfo: DSClient.DataServiceCollection<Point> 
        {
            public int Description { get; set; }
        }

        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public abstract class Shape
        {
            public int Points { get; set; }
        }

        public class Circle : Shape
        {
            public int Diameter { get; set; }
        }
    }
}