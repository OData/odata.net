//---------------------------------------------------------------------
// <copyright file="EntryMaterializationPolicyForComplexResourceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;
    using Xunit;
    using DSClient = Microsoft.OData.Client;


    public class EntryMaterializationPolicyForComplexResourceTests
    {
        [Fact]
        public void ComplexWithPrimitiveValueShouldMaterialize()
        {
            ODataResource pointResource = new ODataResource()
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() { Name = "X", Value = 15 },
                    new ODataProperty() { Name = "Y", Value = 18 }
                }
            };

            var entriesMaterializer = CreateODataEntriesEntityMaterializer(new List<ODataResource> { pointResource }, typeof(CollectionValueMaterializationPolicyTests.Point));

            var points = new List<CollectionValueMaterializationPolicyTests.Point>();

            while (entriesMaterializer.Read())
            {
                points.Add(entriesMaterializer.CurrentValue as CollectionValueMaterializationPolicyTests.Point);
            }

            Assert.Single(points);
            Assert.Equal(15, points.First().X);
        }

        [Fact]
        public void ApplyNonExistantPropertyWithIgnoreMissingPropertiesShouldNotError()
        {
            TestMaterializerContext context = new TestMaterializerContext() { UndeclaredPropertyBehavior = DSClient.UndeclaredPropertyBehavior.Support };
            CollectionValueMaterializationPolicyTests.Point point = new CollectionValueMaterializationPolicyTests.Point();
            ODataProperty property = new ODataProperty() { Name = "Z", Value = 10 };
            this.CreateEntryMaterializationPolicy(context)
                .ApplyDataValue(context.ResolveTypeForMaterialization(typeof(CollectionValueMaterializationPolicyTests.Point), null), property, point);
        }

        [Fact]
        public void ApplyNullOnCollectionPropertyShouldError()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = null };

            Action test = () => this.CreateEntryMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.Collection_NullCollectionNotSupported(property.Name));
        }

        [Fact]
        public void ApplyStringValueForCollectionPropertyShouldError()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = "foo" };

            Action test = () => this.CreateEntryMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.Deserialize_MixedTextWithComment);
        }

        [Fact]
        public void MaterializeDerivedComplexForBaseComplexTypeProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();

            //In a true client, a TypeResolver will be used to resolve derived property type.
            context.ResolveTypeForMaterializationOverrideFunc = (Type type, string name) =>
            {
                var edmType = context.Model.GetOrCreateEdmType(typeof(DerivedComplexType));
                return new ClientTypeAnnotation(edmType, typeof(DerivedComplexType), "DerivedComplexType", context.Model);
            };

            var derivedResource = new ODataResource()
            {
                TypeName = "DerivedComplexType",
                Properties = new ODataProperty[] { new ODataProperty { Name = "DerivedProp", Value = 1 } }
            };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var materializerEntry = MaterializerEntry.CreateEntry(derivedResource, ODataFormat.Json, true, clientEdmModel);
            this.CreateEntryMaterializationPolicy(context).Materialize(materializerEntry, typeof(ChildComplexType), false);

            var derived = materializerEntry.ResolvedObject as DerivedComplexType;
            Assert.NotNull(derived);
            Assert.Equal(1, derived.DerivedProp);
        }

        [Fact]
        public void ApplyDerivedComplexForBaseComplexTypeProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();

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
            ComplexTypeWithChildComplexType complexInstance = new ComplexTypeWithChildComplexType();
            complexInstance.InnerComplexProperty = new DerivedComplexType { DerivedProp = 1 };

            var innerResource = new ODataResource()
            {
                TypeName = "DerivedComplexType",
                Properties = new ODataProperty[] { new ODataProperty { Name = "DerivedProp", Value = 2 } }
            };
            this.ApplyInnerProperty(innerResource, complexInstance, context);

            Assert.Equal(2, (complexInstance.InnerComplexProperty as DerivedComplexType).DerivedProp);
        }

        [Fact]
        public void ApplyODataCollectionValueToNonNullExistingCollectionProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();

            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            complexInstance.Strings.Add("ShouldBeCleared");
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = new ODataCollectionValue() };

            this.CreateEntryMaterializationPolicy(context)
                .ApplyDataValue(
                context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null),
                property, complexInstance);
            complexInstance.Strings.Should().HaveCount(0);
        }

        [Fact]
        public void ApplyODataCollectionValueToNullCollectionProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithPrimitiveCollection complexInstance = new ComplexTypeWithPrimitiveCollection();
            complexInstance.Strings = null;
            ODataProperty property = new ODataProperty() { Name = "Strings", Value = new ODataCollectionValue() { Items = new string[] { "foo" }, TypeName = typeof(ComplexTypeWithPrimitiveCollection).FullName } };

            this.CreateEntryMaterializationPolicy(context).ApplyDataValue(context.ResolveTypeForMaterialization(typeof(ComplexTypeWithPrimitiveCollection), null), property, complexInstance);
            complexInstance.Strings.Should().HaveCount(1);
            complexInstance.Strings[0].Should().Be("foo");
        }

        [Fact]
        public void ValueShouldBeAppliedRegardlessIfPropertyStartsNullOrNot()
        {
            foreach (var startingPropertyState in new ChildComplexType[] { null, new ChildComplexType() })
            {
                TestMaterializerContext context = new TestMaterializerContext();
                ComplexTypeWithChildComplexType complexInstance = new ComplexTypeWithChildComplexType();
                complexInstance.InnerComplexProperty = startingPropertyState;
                var innerEntry = new ODataResource() { Properties = new ODataProperty[] { new ODataProperty() { Name = "Prop", Value = 1 } } };
                this.ApplyInnerProperty(innerEntry, complexInstance);
                complexInstance.InnerComplexProperty.Prop.Should().Be(1);
            }
        }

        [Fact]
        public void NullValueShouldBeAppliedToSubComplexValueProperty()
        {
            TestMaterializerContext context = new TestMaterializerContext();
            ComplexTypeWithChildComplexType complexInstance = new ComplexTypeWithChildComplexType();
            complexInstance.InnerComplexProperty = new ChildComplexType();

            this.ApplyInnerProperty(null, complexInstance);
            Assert.Null(complexInstance.InnerComplexProperty);
        }

        private void ApplyInnerProperty(ODataResource innerResource, ComplexTypeWithChildComplexType parentInstance, TestMaterializerContext context = null)
        {
            context = context ?? new TestMaterializerContext();
            var resource = new ODataResource() { TypeName = "ComplexTypeWithChildComplexType", Properties = new ODataProperty[0] };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var materializerEntry = MaterializerEntry.CreateEntry(resource, ODataFormat.Json, false, clientEdmModel);
            materializerEntry.ResolvedObject = parentInstance;
            ODataNestedResourceInfo innerComplexP = new ODataNestedResourceInfo() { Name = "InnerComplexProperty", IsCollection = false };

            MaterializerEntry innerMaterializerEntry;
            if (innerResource != null)
            {
                innerMaterializerEntry = MaterializerEntry.CreateEntry(innerResource, ODataFormat.Json, true, clientEdmModel);
            }
            else
            {
                innerMaterializerEntry = MaterializerEntry.CreateEmpty();
            }

            MaterializerNavigationLink.CreateLink(innerComplexP, innerMaterializerEntry);
            materializerEntry.AddNestedResourceInfo(innerComplexP);

            var policy = this.CreateEntryMaterializationPolicy(context);
            policy.EntityTrackingAdapter.TargetInstance = parentInstance;
            policy.Materialize(materializerEntry, typeof(ComplexTypeWithChildComplexType), true);
        }

        internal EntryValueMaterializationPolicy CreateEntryMaterializationPolicy(TestMaterializerContext materializerContext)
        {
            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var context = new DataServiceContext();
            materializerContext = materializerContext ?? new TestMaterializerContext() { Model = clientEdmModel, Context = context };
            var adapter = new EntityTrackingAdapter(new TestEntityTracker(), MergeOption.OverwriteChanges, clientEdmModel, context);
            var lazyPrimitivePropertyConverter = new DSClient.SimpleLazy<PrimitivePropertyConverter>(() => new PrimitivePropertyConverter());
            var primitiveValueMaterializerPolicy = new PrimitiveValueMaterializationPolicy(materializerContext, lazyPrimitivePropertyConverter);
            var entryPolicy = new EntryValueMaterializationPolicy(materializerContext, adapter, lazyPrimitivePropertyConverter, null);
            var collectionPolicy = new CollectionValueMaterializationPolicy(materializerContext, primitiveValueMaterializerPolicy);
            var intanceAnnotationPolicy = new InstanceAnnotationMaterializationPolicy(materializerContext);

            entryPolicy.CollectionValueMaterializationPolicy = collectionPolicy;
            entryPolicy.InstanceAnnotationMaterializationPolicy = intanceAnnotationPolicy;

            return entryPolicy;
        }

        [Fact]
        public void ShouldMaterializeComplexCollection()
        {
            var pointResources = new List<ODataResource>(new ODataResource[]
            {
                new ODataResource(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = 15}, new ODataProperty(){Name="Y", Value = 18} }}, 
                new ODataResource(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = 22}, new ODataProperty(){Name="Y", Value = 25} }}, 
                new ODataResource(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = -100}, new ODataProperty(){Name="Y", Value = -201} }},
            });

            var points = new List<CollectionValueMaterializationPolicyTests.Point>();
            var entriesMaterializer = CreateODataEntriesEntityMaterializer(pointResources, typeof(CollectionValueMaterializationPolicyTests.Point));

            while (entriesMaterializer.Read())
            {
                points.Add(entriesMaterializer.CurrentValue as CollectionValueMaterializationPolicyTests.Point);
            }

            points.Should().HaveCount(3);
            points[0].X.Should().Be(15);
            points[0].Y.Should().Be(18);
            points[1].X.Should().Be(22);
            points[1].Y.Should().Be(25);
            points[2].X.Should().Be(-100);
            points[2].Y.Should().Be(-201);
        }

        [Fact]
        public void ShouldMaterializeConcreteComplexCollectionDeclaredAsAbstract()
        {
            var pointResources = new List<ODataResource>(new[]
            {
                new ODataResource(){Properties = new ODataProperty[]{ new ODataProperty(){Name="Points", Value = 0}, new ODataProperty(){Name="Diameter", Value = 15} }}, 
                new ODataResource(){Properties = new ODataProperty[]{ new ODataProperty(){Name="Points", Value = 0}, new ODataProperty(){Name="Diameter", Value = 18} }},
            });

            var testContext = new TestMaterializerContext();
            testContext.ResolveTypeForMaterializationOverrideFunc = (Type type, string name) =>
            {
                var edmType = testContext.Model.GetOrCreateEdmType(typeof(CollectionValueMaterializationPolicyTests.Circle));
                return new ClientTypeAnnotation(edmType, typeof(CollectionValueMaterializationPolicyTests.Circle), "Circle", testContext.Model);
            };

            var outputCollection = new List<CollectionValueMaterializationPolicyTests.Shape>();
            var entriesMaterializer = CreateODataEntriesEntityMaterializer(pointResources, typeof(CollectionValueMaterializationPolicyTests.Shape), testContext);

            while (entriesMaterializer.Read())
            {
                outputCollection.Add(entriesMaterializer.CurrentValue as CollectionValueMaterializationPolicyTests.Shape);
            }
            outputCollection.Should().HaveCount(2);
            outputCollection[0].Points.Should().Be(0);
            ((CollectionValueMaterializationPolicyTests.Circle)outputCollection[0]).Diameter.Should().Be(15);
            outputCollection[1].Points.Should().Be(0);
            ((CollectionValueMaterializationPolicyTests.Circle)outputCollection[1]).Diameter.Should().Be(18);
        }

        [Fact]
        public void ShouldMaterializeNullableComplexCollection()
        {
            var pointResources = new List<ODataResource>(new[]
            {
                new ODataResource(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = 15}, new ODataProperty(){Name="Y", Value = 18} }}, 
                null,
                new ODataResource(){Properties = new ODataProperty[]{ new ODataProperty(){Name="X", Value = -100}, new ODataProperty(){Name="Y", Value = -201} }},
            });

            var points = new List<CollectionValueMaterializationPolicyTests.Point>();
            var entriesMaterializer = CreateODataEntriesEntityMaterializer(pointResources, typeof(CollectionValueMaterializationPolicyTests.Point));

            while (entriesMaterializer.Read())
            {
                points.Add(entriesMaterializer.CurrentValue as CollectionValueMaterializationPolicyTests.Point);
            }

            points.Should().HaveCount(3);
            points[0].X.Should().Be(15);
            points[0].Y.Should().Be(18);
            points[1].Should().BeNull();
            points[2].X.Should().Be(-100);
            points[2].Y.Should().Be(-201);
        }

        internal ODataEntriesEntityMaterializer CreateODataEntriesEntityMaterializer(
            List<ODataResource> resources,
            Type resourceType,
            TestMaterializerContext materializerContext = null)
        {
            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var context = new DataServiceContext();

            var resourceSet = new ODataResourceSet();
            MaterializerFeed.CreateFeed(resourceSet, resources);
            resources.ForEach(r =>
            {
                if (r == null)
                {
                    MaterializerEntry.CreateEmpty();
                }
                else
                {
                    MaterializerEntry.CreateEntry(r, ODataFormat.Json, true, clientEdmModel);
                }
            });
            materializerContext = materializerContext ?? new TestMaterializerContext() { Model = clientEdmModel, Context = context };
            var adapter = new EntityTrackingAdapter(new TestEntityTracker(), MergeOption.OverwriteChanges, clientEdmModel, context);
            QueryComponents components = new QueryComponents(new Uri("http://foo.com/Service"), new Version(4, 0), resourceType, null, new Dictionary<Expression, Expression>());

            return new ODataEntriesEntityMaterializer(resources, materializerContext, adapter, components, resourceType, null, ODataFormat.Json);
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