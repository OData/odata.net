//---------------------------------------------------------------------
// <copyright file="MetadataDocumentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using test = System.Data.Test.Astoria;

    #endregion Namespaces

    [TestModule]
    public partial class UnitTestModule : test.AstoriaTestModule
    {
        private const OperationParameterBindingKind Default_OperationParameterBinding = OperationParameterBindingKind.Never;
        private const bool Default_IsComposable = false;
        private const bool Default_IsSideEffecting = true;

        [TestClass, TestCase]
        public class MetadataDocumentTests
        {
            private class DSPMetadata_CustomTypeEnumeration : DSPMetadata
            {
                public Func<IEnumerable<ResourceType>, IEnumerable<ResourceType>> TypesOverride { get; set; }

                public DSPMetadata_CustomTypeEnumeration(string containerName, string namespaceName)
                    : base(containerName, namespaceName)
                {
                }

                public override IEnumerable<ResourceType> Types
                {
                    get
                    {
                        return this.TypesOverride != null ? this.TypesOverride(base.Types) : base.Types;
                    }
                }
            }

            [TestMethod, Variation]
            public void ResourceTypeEnumeration()
            {
                DSPMetadata_CustomTypeEnumeration metadata = new DSPMetadata_CustomTypeEnumeration("TestContext", "TestNamespace");
                var complexType = metadata.AddComplexType("ComplexType", null, null, false);
                metadata.AddPrimitiveProperty(complexType, "Name", typeof(string));

                var typesToIgnore = UnitTestsUtil.GetPrimitiveResourceTypes().Concat(
                    UnitTestsUtil.GetPrimitiveResourceTypes().Except(new[] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)) }).Select(prt => ResourceType.GetCollectionResourceType(prt))).Concat(
                    new ResourceType[] { ResourceType.GetCollectionResourceType(complexType) });
                DSPServiceDefinition service = new DSPServiceDefinition()
                {
                    Metadata = metadata,
                    DataSource = new DSPContext(),
                    EnableAccess = new List<string>() { "*" }
                };
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    test.TestUtil.RunCombinations(typesToIgnore, (typeToIgnore) =>
                    {
                        metadata.TypesOverride = (baseTypes) => { return baseTypes.Concat(new ResourceType[] { typeToIgnore }); };
                        request.RequestUriString = "/$metadata";
                        request.SendRequest();
                        XDocument metadataDocument = request.GetResponseStreamAsXDocument();
                        XElement schema = metadataDocument.Root.Element(UnitTestsUtil.EdmxNamespace + "DataServices").Elements().First(e => e.Name.LocalName == "Schema");
                        Assert.AreEqual(2, schema.Elements().Count(), "Only two elements should be in the schema, the complex type and the entity container.");
                        Assert.IsNotNull(schema.Elements(schema.Name.Namespace + "ComplexType").Where(e => (string)e.Attribute("Name") == "ComplexType").FirstOrDefault(),
                            "The complex type must be in the metadata.");
                        Assert.IsNotNull(schema.Element(schema.Name.Namespace + "EntityContainer"), "The entity container must be in the metadata.");
                        // The typeToIgnore should not appear in the metadata
                    });
                }
            }

            [TestMethod, Variation("Verify that complex properties generate correct CSDL for $metadata.")]
            public void ComplexPropertyProviderMetadataTests()
            {
                // MPV should not be used to compute the nullability of complex properties. Rather, the DSV and the edm version 
                // computed by walking through the resource model should be used to get the nullability facet of complex properties.
                System.Data.Test.Astoria.TestUtil.RunCombinations(
                    new[] { "EF", "Custom", "Reflection" },
                    new[] { ODataProtocolVersion.V4 },
                    new[] { ODataProtocolVersion.V4 },
                    (provider, serviceVersion, maxProtocolVersion) =>
                    {
                        if (serviceVersion > maxProtocolVersion) return;

                        XNamespace edmNamespace = UnitTestsUtil.EdmOasisNamespace;
                        string propertyName = "ComplexProperty";
                        TestWebRequest request = null;

                        try
                        {
                            DSPServiceDefinition service = null;
                            if (provider == "Custom" || provider == "EF")
                            {
                                DSPMetadata metadata = new DSPMetadata("TestService" + serviceVersion + maxProtocolVersion, "TestNamespace");
                                ResourceType complexType = metadata.AddComplexType("TestComplexType", null, null, false);
                                metadata.AddPrimitiveProperty(complexType, "Name", typeof(string));
                                ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                                metadata.AddComplexProperty(entityType, "ComplexProperty", complexType);

                                if (provider == "EF")
                                {
                                    if (serviceVersion == ODataProtocolVersion.V4)
                                    {
                                        // TODO: Ability to create a v3 model for EF.
                                        // Once this bug is resolved, add code here to create a V3 model.
                                        return;
                                    }
                                    metadata.AddResourceSet("TestEntities", entityType);
                                    service = new DSPUnitTestServiceDefinition(metadata, DSPDataProviderKind.EF, new DSPContext());
                                }
                                else
                                {
                                    // Add a collection property to get DSV as V3 when walking the model
                                    if (serviceVersion == ODataProtocolVersion.V4)
                                    {
                                        metadata.AddCollectionProperty(entityType, "CollectionOfIntOnEntity", typeof(int));
                                    }
                                    metadata.AddResourceSet("TestEntities", entityType);
                                    service = new DSPServiceDefinition { Metadata = metadata };
                                }
                            }
                            else if (provider == "Reflection")
                            {
                                // Add a collection property to get DSV as V3 when walking the model
                                service = new DSPServiceDefinition() {DataServiceType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<int>, ComplexSimple>>)};
                            }
                            else
                            {
                                throw new Exception(provider + " is an invalid provider name. Valid values are 'EF', 'Custom' and 'Reflection'.");
                            }

                            // What we set here for MPV should not influence $metadata serialization.
                            service.DataServiceBehavior.MaxProtocolVersion = maxProtocolVersion;
                            request = service.CreateForInProcess();
                            request.RequestUriString = "/$metadata";
                            request.SendRequest();
                            var response = request.GetResponseStreamAsXDocument();
                            var csdlProperty = response.Descendants(edmNamespace + "Property")
                                .Single(e => (string)e.Attribute("Name") == propertyName);

                            string actualNullableFacet = (string)csdlProperty.Attribute("Nullable");

                            // The nullability facet value is dependent on DSV and not MPV.
                            // TODO: Once we can create a v3 model for EF, update the condition below so that for EF, we always get the 
                            // nullable facet as false.
                            Assert.IsTrue(actualNullableFacet == null,
                                    "Complex property has wrong nullable facet. Expected = null, Actual = " + actualNullableFacet);
                        }
                        finally
                        {
                            if (request != null) request.Dispose();
                        }
                    }
                );
            }


            [TestMethod, Variation("Verify that complex type without child properties is allowed.")]
            public void MetadataShouldNotThrowWhenModelContainsComplexTypesWithoutProperties()
            {
                DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                ResourceType complexType = metadata.AddComplexType("TestComplexType", null, null, false);
                ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                metadata.AddComplexProperty(entityType, "Complex", complexType);
                metadata.AddResourceSet("TestEntities", entityType);

                using (TestWebRequest request = new DSPServiceDefinition() { Metadata = metadata }.CreateForInProcess())
                {
                    request.RequestUriString = "/$metadata";
                    Exception e = test.TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNull(e);
                }
            }

            [TestMethod, Variation("Verify that collection properties generate correct CSDL for $metadata.")]
            public void CollectionPropertyCSDL()
            {
                DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                ResourceType complexType = metadata.AddComplexType("TestComplexType", null, null, false);
                metadata.AddPrimitiveProperty(complexType, "Name", typeof(string));
                metadata.AddCollectionProperty(complexType, "CollectionOfIntOnComplex", typeof(int));
                metadata.AddCollectionProperty(complexType, "CollectionOfStringOnComplex", typeof(string));
                metadata.AddCollectionProperty(complexType, "CollectionOfComplexOnComplex", complexType);
                ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                metadata.AddComplexProperty(entityType, "Complex", complexType);
                metadata.AddCollectionProperty(entityType, "CollectionOfIntOnEntity", typeof(int));
                metadata.AddCollectionProperty(entityType, "CollectionOfStringOnEntity", typeof(string));
                metadata.AddCollectionProperty(entityType, "CollectionOfComplexOnEntity", complexType);
                metadata.AddCollectionProperty(entityType, "CollectionOfNullableIntOnEntity", typeof(int?));
                metadata.AddResourceSet("TestEntities", entityType);

                var collectionProperties = new[]{
                    new { PropertyName = "CollectionOfIntOnComplex", ExpectedItemType = "Edm.Int32" },
                    new { PropertyName = "CollectionOfStringOnComplex", ExpectedItemType = "Edm.String" },
                    new { PropertyName = "CollectionOfComplexOnComplex", ExpectedItemType = "TestNamespace.TestComplexType" },
                    new { PropertyName = "CollectionOfIntOnEntity", ExpectedItemType = "Edm.Int32" },
                    new { PropertyName = "CollectionOfStringOnEntity", ExpectedItemType = "Edm.String" },
                    new { PropertyName = "CollectionOfComplexOnEntity", ExpectedItemType = "TestNamespace.TestComplexType" },
                    new { PropertyName = "CollectionOfNullableIntOnEntity", ExpectedItemType = "Edm.Int32" },
                };

                using (TestWebRequest request = new DSPServiceDefinition() { Metadata = metadata }.CreateForInProcess())
                {
                    request.RequestUriString = "/$metadata";
                    request.SendRequest();
                    var response = request.GetResponseStreamAsXDocument();
                    foreach (var collectionProperty in collectionProperties)
                    {
                        var csdlProperty = response.Descendants(UnitTestsUtil.EdmOasisNamespace + "Property")
                            .Single(e => (string)e.Attribute("Name") == collectionProperty.PropertyName);
                        string itemType = collectionProperty.ExpectedItemType;
                        string collectionType = "Collection(" + itemType + ")";
                        Assert.AreEqual(collectionType, (string)csdlProperty.Attribute("Type"), "Collection property has wrong type. Should always be 'Collection(itemType)'.");
                    }
                }
            }

            private sealed class PropertiesWithReservedNameTestCase
            {
                public PropertiesWithReservedNameTestCase(string typeName, string propertyName, char invalidChar)
                {
                    this.TypeName = typeName;
                    this.PropertyName = propertyName;
                    this.InvalidChar = InvalidChar;
                }

                public string TypeName { get; set; }
                public string PropertyName { get; set; }
                public char InvalidChar { get; set; }
            }
            [TestMethod, Variation("Verify that property names that contain reserved characters generate an error.")]
            public void PropertiesWithReservedNames()
            {
                var testCases = new PropertiesWithReservedNameTestCase[]
                    {
                        // Complex type properties
                        new PropertiesWithReservedNameTestCase("TestComplexType", "Invalid.Complex", '.'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", "Invalid:Complex", ':'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", "Invalid@Complex", '@'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", ".InvalidComplex", '.'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", ":InvalidComplex", ':'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", "@InvalidComplex", '@'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", "InvalidComplex.", '.'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", "InvalidComplex:", ':'),
                        new PropertiesWithReservedNameTestCase("TestComplexType", "InvalidComplex@", '@'),

                        // Entity type properties
                        new PropertiesWithReservedNameTestCase("TestEntityType", "Invalid.Entity", '.'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", "Invalid:Entity", ':'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", "Invalid@Entity", '@'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", ".InvalidEntity", '.'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", ":InvalidEntity", ':'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", "@InvalidEntity", '@'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", "InvalidEntity.", '.'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", "InvalidEntity:", ':'),
                        new PropertiesWithReservedNameTestCase("TestEntityType", "InvalidEntity@", '@'),

                    };

                test.TestUtil.RunCombinations(
                    testCases,
                    testCase =>
                    {
                        DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                        ResourceType complexType = metadata.AddComplexType("TestComplexType", null, null, false);
                        ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                        metadata.AddKeyProperty(entityType, "ID", typeof(int));
                        metadata.AddComplexProperty(entityType, "Complex", complexType);
                        metadata.AddResourceSet("TestEntities", entityType);

                        ResourceType type = metadata.GetResourceType(testCase.TypeName);

                        // Add a property to the complex type to avoid an error if it doesn't have one.
                        metadata.AddPrimitiveProperty(complexType, "Primitive", typeof(string));
                        metadata.AddPrimitiveProperty(type, testCase.PropertyName, typeof(string));

                        DSPServiceDefinition service = new DSPServiceDefinition()
                        {
                            Metadata = metadata,
                            DataSource = new DSPContext(),
                            EnableAccess = new List<string>() { "*" }
                        };

                        using (TestWebRequest request = service.CreateForInProcess())
                        {
                            request.RequestUriString = "/$metadata";
                            Exception e = test.TestUtil.RunCatching(request.SendRequest);
                            Assert.IsNotNull(e, "The request should have failed.");
                            Assert.IsNotNull(e.InnerException, "The exception should have an inner exception.");
                            Assert.AreEqual(
                                string.Format("An IEdmModel instance was found that failed validation. The following errors were reported:\r\nInvalidName : The name '{0}' of a property is invalid; property names must not contain any of the reserved characters ':', '.', '@'.\r\n", testCase.PropertyName),
                                e.InnerException.Message);
                        }
                    });
            }

            public class EntityWithCollectionProperty<TCollectionPropertyType, TComplexPropertyType>
            {
                public int ID { get; set; }
                public TCollectionPropertyType CollectionProperty { get; set; }
                public TComplexPropertyType ComplexProperty { get; set; }
            }

            public class EntityDerivedFromEnumerable : IEnumerable<int>
            {
                public int ID { get; set; }
                public IEnumerator<int> GetEnumerator() { throw new NotImplementedException(); }
                IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
            }

            public class ComplexWithCollectionProperty<TCollectionPropertyType>
            {
                public TCollectionPropertyType ComplexCollectionProperty { get; set; }
            }

            public class ComplexSimple
            {
                public string Name { get; set; }
            }

            public class ComplexBase
            {
                public string Name { get; set; }
            }

            public class ComplexDerived : ComplexBase
            {
                public string Surname { get; set; }
            }

            public class ComplexWithRecursiveCollection
            {
                public IEnumerable<ComplexWithRecursiveCollection> ComplexRecursiveCollectionProperty { get; set; }
            }

            public class ComplexDerivedFromEnumerable : IEnumerable<int>
            {
                public IEnumerator<int> GetEnumerator() { throw new NotImplementedException(); }
                IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
            }

            public class CollectionContext<TEntityType>
            {
                public IQueryable<TEntityType> Items { get { return new TEntityType[] { }.AsQueryable(); } }
            }

            public class CollectionContext<TEntityType1, TEntityType2>
            {
                public IQueryable<TEntityType1> Items1 { get { return new TEntityType1[] { }.AsQueryable(); } }
                public IQueryable<TEntityType2> Items2 { get { return new TEntityType2[] { }.AsQueryable(); } }
            }

            [TestMethod, Variation("Verify that collection properties declared through reflection provider generate correct CSDL for $metadata.")]
            public void ReflectionProviderCollectionPropertyCSDL()
            {
                // CollectionContext<EntityWithCollectionProperty<IEnumerable<primitiveType>, int>>
                var primitiveTypeCollectionOnEntity = UnitTestsUtil.GetPrimitiveResourceTypes().Except(new[] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)) }).Select(rt => new Tuple<Type, Tuple<string, string>[]>(
                    typeof(CollectionContext<>).MakeGenericType(
                        typeof(EntityWithCollectionProperty<,>).MakeGenericType(
                            typeof(IEnumerable<>).MakeGenericType(rt.InstanceType), typeof(int))),
                    new Tuple<string, string>[] { new Tuple<string, string>("CollectionProperty", rt.FullName) }));

                // CollectionContext<EntityWithCollectionProperty<int, ComplexWithCollectionProperty<IEnumerable<primitiveType>>>>
                var primitiveTypeCollectionOnComplex = UnitTestsUtil.GetPrimitiveResourceTypes().Except(new[] { ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)) }).Select(rt => new Tuple<Type, Tuple<string, string>[]>(
                    typeof(CollectionContext<>).MakeGenericType(
                        typeof(EntityWithCollectionProperty<,>).MakeGenericType(
                            typeof(int),
                            typeof(ComplexWithCollectionProperty<>).MakeGenericType(
                                typeof(IEnumerable<>).MakeGenericType(rt.InstanceType)))),
                    new Tuple<string, string>[] { new Tuple<string, string>("ComplexCollectionProperty", rt.FullName) }));

                var manualTestCases = new Tuple<Type, Tuple<string, string>[]>[] {
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<List<int>, int>>), 
                        new [] { new Tuple<string, string>("CollectionProperty", "Edm.Int32") }),
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<System.Collections.ObjectModel.Collection<int>, int>>), 
                        new [] { new Tuple<string, string>("CollectionProperty", "Edm.Int32") }),
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<ComplexSimple>, int>>), 
                        new [] { new Tuple<string, string>("CollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexSimple") }),
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<ComplexDerived>, int>>), 
                        new [] { new Tuple<string, string>("CollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexDerived") }),
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<ComplexWithRecursiveCollection>, int>>), 
                        new [] { new Tuple<string, string>("CollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexWithRecursiveCollection"),
                            new Tuple<string, string>("ComplexRecursiveCollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexWithRecursiveCollection") }),
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<int, ComplexWithCollectionProperty<IEnumerable<ComplexSimple>>>>), 
                        new [] { new Tuple<string, string>("ComplexCollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexSimple") }),
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<int, ComplexWithRecursiveCollection>>), 
                        new [] { new Tuple<string, string>("ComplexRecursiveCollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexWithRecursiveCollection") }),
                    new Tuple<Type, Tuple<string, string>[]> (
                        typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<ComplexWithRecursiveCollection>, ComplexWithRecursiveCollection>>), 
                        new [] { new Tuple<string, string>("CollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexWithRecursiveCollection"),
                            new Tuple<string, string>("ComplexRecursiveCollectionProperty", "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexWithRecursiveCollection") }),
                };

                test.TestUtil.RunCombinations(primitiveTypeCollectionOnEntity.Concat(primitiveTypeCollectionOnComplex).Concat(manualTestCases),
                    (testCase) =>
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = testCase.Item1;
                            request.RequestUriString = "/$metadata";
                            request.SendRequest();
                            var response = request.GetResponseStreamAsXDocument();
                            foreach (var expectedCollectionProperty in testCase.Item2)
                            {
                                var csdlProperty = response.Descendants(UnitTestsUtil.EdmOasisNamespace + "Property")
                                    .Single(e => (string)e.Attribute("Name") == expectedCollectionProperty.Item1);
                                string itemType = expectedCollectionProperty.Item2;
                                string collectionType = "Collection(" + itemType + ")";
                                Assert.AreEqual(collectionType, (string)csdlProperty.Attribute("Type"), "Collection property has wrong type. Should always be 'Collection(itemType)'.");
                            }
                        }
                    });
            }

            [TestMethod, Variation("Verify that reflection provider correctly fails on certain context classes.")]
            public void CollectionProperty_InvalidReflectionContext()
            {
                var testCases = new[] {
                    new { 
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<IEnumerable<int>>, int>>), 
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.IEnumerable`1[System.Collections.Generic.IEnumerable`1[System.Int32]] System.Int32]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new {
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<List<int>>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.IEnumerable`1[System.Collections.Generic.List`1[System.Int32]] System.Int32]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new {
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<List<IEnumerable<int>>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.List`1[System.Collections.Generic.IEnumerable`1[System.Int32]] System.Int32]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new {
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<List<List<int>>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.List`1[System.Collections.Generic.List`1[System.Int32]] System.Int32]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new {
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<List<ComplexSimple>>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.IEnumerable`1[System.Collections.Generic.List`1[AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexSimple]] System.Int32]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new { 
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<int, ComplexWithCollectionProperty<IEnumerable<IEnumerable<int>>>>>), 
                        ExpectedExceptionMessage = "The property 'ComplexCollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexWithCollectionProperty`1[System.Collections.Generic.IEnumerable`1[System.Collections.Generic.IEnumerable`1[System.Int32]]]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new { 
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<int, ComplexWithCollectionProperty<List<List<ComplexSimple>>>>>), 
                        ExpectedExceptionMessage = "The property 'ComplexCollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexWithCollectionProperty`1[System.Collections.Generic.List`1[System.Collections.Generic.List`1[AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexSimple]]]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new {  // Collection of type which looks like an entity type (but does not have a IQueryable property) should fail as well (since we recognize it as a collection of complex types which implement IEnumerable<T> and thus it's a collection of collection)
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<EntityDerivedFromEnumerable>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.IEnumerable`1[AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityDerivedFromEnumerable] System.Int32]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new {  // Complex type deriving from IEnumerable<T> looks exactly like a collection property and should be treated as such. This is not a breaking change as such types were not allowed in V2.
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<ComplexDerivedFromEnumerable>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on a type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.IEnumerable`1[AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexDerivedFromEnumerable] System.Int32]' is not a valid property. A collection property that contains collection types is not supported."
                    },
                    new {  // Collection of entity type which derives from IEnumerable<T> is still a resource set reference and should not fail.
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<EntityDerivedFromEnumerable>, int>, EntityDerivedFromEnumerable>),
                        ExpectedExceptionMessage = (string)null
                    },
                    new {  // Reference to an entity type which derives from IEnumerable<T> is still a resource reference and should not fail.
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<EntityDerivedFromEnumerable, int>, EntityDerivedFromEnumerable>),
                        ExpectedExceptionMessage = (string)null
                    },
                    new {  // Collection of unsupported type (any value type which is not a primitive type is unsupported)
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<char>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.IEnumerable`1[System.Char] System.Int32]' is a collection property with unsupported item type 'System.Char'. Only primitive types and complex types are valid item types for a collection property."
                    },
                    new {  // Collection of unsupported type (some types are explicitely unsupported right now)
                        ContextType = typeof(CollectionContext<EntityWithCollectionProperty<IEnumerable<Tuple<int>>, int>>),
                        ExpectedExceptionMessage = "The property 'CollectionProperty' on type 'AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_EntityWithCollectionProperty`2[System.Collections.Generic.IEnumerable`1[System.Tuple`1[System.Int32]] System.Int32]' is a collection property with unsupported item type 'System.Tuple`1[System.Int32]'. Only primitive types and complex types are valid item types for a collection property."
                    },
                };

                test.TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = testCase.ContextType;
                        request.RequestUriString = "/$metadata";
                        Exception e = test.TestUtil.RunCatching(request.SendRequest);
                        if (testCase.ExpectedExceptionMessage != null)
                        {
                            Assert.IsNotNull(e, "The request should have failed.");
                            Assert.AreEqual(testCase.ExpectedExceptionMessage, e.Message, "Unexpected error message.");
                        }
                        else
                        {
                            Assert.IsNull(e, "The request should have succeeded.");
                        }
                    }
                });
            }

            [TestMethod, Variation("Verify that invalid collection properties fail to generate CSDL.")]
            public void CollectionProperty_InvalidCSDL()
            {
                // Reflection provider doesn't support inheritance of complex types, so no need to test invalid collection there yet (as the only invalid case is
                //   collection of complex types which can be derived).

                var testCases = new[] {
                    new {
                        MetadataAction = new Action<DSPMetadata>((metadata) => { 
                            metadata.AddCollectionProperty(metadata.GetResourceType("TestEntityType"), "CollectionProperty",
                                metadata.GetResourceType("ComplexBase")); }),
                        ExpectedExceptionMessage = "Complex type 'TestNamespace.ComplexBase' has derived types and is used as the item type in a collection property. Only collection properties containing complex types without derived types are supported."
                    },
                    new {
                        MetadataAction = new Action<DSPMetadata>((metadata) => { 
                            metadata.AddCollectionProperty(metadata.GetResourceType("ComplexSimple"), "CollectionProperty",
                                metadata.GetResourceType("ComplexBase")); }),
                        ExpectedExceptionMessage = "Complex type 'TestNamespace.ComplexBase' has derived types and is used as the item type in a collection property. Only collection properties containing complex types without derived types are supported."
                    },
                    new {
                        MetadataAction = new Action<DSPMetadata>((metadata) => { 
                            metadata.AddCollectionProperty(metadata.GetResourceType("TestEntityType"), "CollectionProperty",
                                metadata.GetResourceType("ComplexSimple"));
                            metadata.AddCollectionProperty(metadata.GetResourceType("ComplexSimple"), "InnerCollectionProperty",
                                metadata.GetResourceType("ComplexBase")); }),
                        ExpectedExceptionMessage = "Complex type 'TestNamespace.ComplexBase' has derived types and is used as the item type in a collection property. Only collection properties containing complex types without derived types are supported."
                    },
                };

                System.Data.Test.Astoria.TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                    ResourceType complexBaseType = metadata.AddComplexType("ComplexBase", null, null, false);
                    metadata.AddPrimitiveProperty(complexBaseType, "Name", typeof(string));
                    ResourceType complexDerivedType = metadata.AddComplexType("ComplexDerived", null, complexBaseType, false);
                    metadata.AddPrimitiveProperty(complexDerivedType, "Surname", typeof(string));
                    ResourceType complexSimple = metadata.AddComplexType("ComplexSimple", null, null, false);
                    ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    metadata.AddComplexProperty(entityType, "SimpleComplexProperty", complexSimple);
                    metadata.AddResourceSet("TestEntities", entityType);

                    testCase.MetadataAction(metadata);

                    using (TestWebRequest request = new DSPServiceDefinition() { Metadata = metadata }.CreateForInProcess())
                    {
                        request.RequestUriString = "/$metadata";
                        Exception exception = System.Data.Test.Astoria.TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(exception, "Exception expected. The request should have failed.");
                        Assert.AreEqual(testCase.ExpectedExceptionMessage, exception.InnerException.Message, "Unexpected exception message.");
                    }
                });
            }

            public class EntityWithComplexTypeWithReferenceProperty
            {
                public int ID { get; set; }
                public ComplexTypeWithReferenceProperty ComplexProperty { get; set; }
            }

            public class ComplexTypeWithReferenceProperty
            {
                public EntityWithComplexTypeWithReferenceProperty ReferenceProperty { get; set; }
            }

            public class EntityWithComplexTypeWithReferenceSetProperty
            {
                public int ID { get; set; }
                public ComplexTypeWithReferenceSetProperty ComplexProperty { get; set; }
            }

            public class ComplexTypeWithReferenceSetProperty
            {
                public IEnumerable<EntityWithComplexTypeWithReferenceSetProperty> ReferenceSetProperty { get; set; }
            }

            [TestMethod, Variation("Verify that we don't allow nav properties on complex types.")]
            public void ReflectionProviderNavigationPropertyOnComplexType()
            {
                var testCases = new[] {
                    new { ContextType = typeof(CollectionContext<EntityWithComplexTypeWithReferenceProperty>), PropertyName = "ReferenceProperty", TypeName = "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexTypeWithReferenceProperty" },
                    new { ContextType = typeof(CollectionContext<EntityWithComplexTypeWithReferenceSetProperty>), PropertyName = "ReferenceSetProperty", TypeName = "AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_ComplexTypeWithReferenceSetProperty" }
                };

                System.Data.Test.Astoria.TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = testCase.ContextType;
                        request.RequestUriString = "/$metadata";
                        Exception e = System.Data.Test.Astoria.TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e, "The request should have failed.");
                        Assert.IsInstanceOfType(e, typeof(InvalidOperationException));
                        Assert.AreEqual(
                            ResourceUtil.GetStringResource(ResourceUtil.SystemDataServicesResourceManager, "ReflectionProvider_ComplexTypeWithNavigationProperty", testCase.PropertyName, testCase.TypeName),
                            e.Message,
                            "Wrong exception message");
                    }
                });
            }

            private void AddXPath(List<string> xPaths, string name, ResourceType returnType, ResourceSetPathExpression pathExpression, OperationParameterBindingKind operationParameterBindingKind, bool isFunction, bool isComposable)
            {
                string xpath = string.Format("//csdl:Action[@Name='{0}' and not(@adsm:HttpMethod)", name);
               
                switch (operationParameterBindingKind)
                {
                    case OperationParameterBindingKind.Never:
                        xpath += string.Format(" and not(@IsBound) and not(@adsm:IsAlwaysBindable)");
                        break;

                    case OperationParameterBindingKind.Sometimes:
                        xpath += string.Format(" and @IsBound='true' and not(@adsm:IsAlwaysBindable)");
                        break;

                    case OperationParameterBindingKind.Always:
                        xpath += string.Format(" and @IsBound='true'");
                        break;

                    default:
                        break;
                }

                if (isComposable != Default_IsComposable)
                {
                    xpath += string.Format(" and @IsComposable='{0}'", isComposable ? "true" : "false");
                }

                if (returnType != null)
                {
                    CollectionResourceType collectionType = returnType as CollectionResourceType;
                    if (collectionType == null)
                    {
                        EntityCollectionResourceType entityCollectionType = returnType as EntityCollectionResourceType;
                        if (entityCollectionType == null)
                        {
                            xpath += string.Format(" and csdl:ReturnType[@Type='{0}']", returnType.FullName);
                        }
                        else
                        {
                            xpath += string.Format(" and csdl:ReturnType[@Type='Collection({0})']", entityCollectionType.ItemType.FullName);
                        }
                    }
                    else
                    {
                        xpath += string.Format(" and csdl:ReturnType[@Type='Collection({0})']", collectionType.ItemType.FullName);
                    }
                }

                if (pathExpression != null)
                {
                    xpath += string.Format(" and @EntitySetPath='{0}'", pathExpression.PathExpression);
                }

                xpath += "]";

                xPaths.Add(xpath);
            }

            [TestMethod, Variation("Verify CSDL for actions.")]
            public void ServiceAction_CSDL()
            {
                DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                ResourceType customerType = metadata.AddEntityType("Customer", null, null, false);
                metadata.AddKeyProperty(customerType, "ID", typeof(int));
                ResourceSet customerSet = metadata.AddResourceSet("Customers", customerType);
                ResourceType complexType = metadata.AddComplexType("Address", null, null, false);
                metadata.AddPrimitiveProperty(complexType, "Data", typeof(string));
                DSPActionProvider actionProvider = new DSPActionProvider();

                var pathExpression = new ResourceSetPathExpression("p1");
                var types = new ResourceType[]
                {
                    null,
                    customerType,
                    complexType,
                    ResourceType.GetEntityCollectionResourceType(customerType),
                    ResourceType.GetCollectionResourceType(complexType),
                    ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(int))),
                    ResourceType.GetPrimitiveResourceType(typeof(string)),
                };

                List<string> xPaths = new List<string>();

                int idx = 0;
                foreach (ResourceType returnType in types)
                {
                    ResourceSet resultSet = null;
                    ResourceSetPathExpression resultPath = null;
                    if (returnType != null && (returnType.ResourceTypeKind == ResourceTypeKind.EntityType || returnType.ResourceTypeKind == ResourceTypeKind.EntityCollection))
                    {
                        resultSet = customerSet;
                        resultPath = pathExpression;
                    }

                    foreach (ResourceType parameterType in types)
                    {
                        var parameters = parameterType == null ? null : new ServiceActionParameter[] { new ServiceActionParameter("p1", parameterType) };
                        string operationName;

                        if (parameters == null || (parameters.First().ParameterType.ResourceTypeKind != ResourceTypeKind.EntityType && parameters.First().ParameterType.ResourceTypeKind != ResourceTypeKind.EntityCollection))
                        {
                            // OperationParameterBindingKind.Never
                            operationName = "foo" + ++idx;
                            actionProvider.AddAction(operationName, returnType, resultSet, parameters, OperationParameterBindingKind.Never);
                            AddXPath(xPaths, operationName, returnType, null, OperationParameterBindingKind.Never, isFunction: false, isComposable: false);
                        }

                        if (parameters != null && (parameters.First().ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType || parameters.First().ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection))
                        {
                            // OperationParameterBindingKind.Sometimes
                            operationName = "foo" + ++idx;
                            actionProvider.AddAction(operationName, returnType, resultPath, parameters, OperationParameterBindingKind.Sometimes);
                            AddXPath(xPaths, operationName, returnType, null, OperationParameterBindingKind.Sometimes, isFunction: false, isComposable: false);

                            // OperationParameterBindingKind.Sometimes
                            operationName = "foo" + ++idx;
                            actionProvider.AddAction(operationName, returnType, resultPath, parameters);
                            AddXPath(xPaths, operationName, returnType, resultPath, OperationParameterBindingKind.Sometimes, isFunction: false, isComposable: false);

                            // OperationParameterBindingKind.Always
                            operationName = "foo" + ++idx;
                            actionProvider.AddAction(operationName, returnType, resultPath, parameters, OperationParameterBindingKind.Always);
                            AddXPath(xPaths, operationName, returnType, null, OperationParameterBindingKind.Always, isFunction: false, isComposable: false);
                        }
                    }
                }

                using (TestWebRequest request = new DSPServiceDefinition() { Metadata = metadata, ActionProvider = actionProvider }.CreateForInProcess())
                {
                    request.RequestUriString = "/$metadata";
                    request.SendRequest();
                    var metadataResponse = request.GetResponseStreamAsText();
                    var responseBytes = Encoding.UTF8.GetBytes(metadataResponse);
                    UnitTestsUtil.VerifyXPaths(new MemoryStream(responseBytes), UnitTestsUtil.MimeApplicationXml, xPaths.ToArray());
                }
            }

            [TestMethod, Variation("Verify validation of metadata on write.")]
            public void MetadataValidationOnWrite()
            {
                var testCases = new MetadataValidationTestCase[]
                {
                    // Create an entity type with a whitespace-only name; this is invalid and should fail with metadata validation enabled.
                    new MetadataValidationTestCase
                    {
                        Metadata = InvalidModels.CreateEntityTypeWithWhitespaceNameMetadata,
                        ValidationExpectedExceptionMessage = "An IEdmModel instance was found that failed validation. The following errors were reported:\r\nInvalidName : The name is missing or not valid.\r\n",
                        NoValidationExpectedExceptionMessage = "The metadata document could not be written as specified.\r\nReferencedTypeMustHaveValidName : A referenced type can not be serialized with an invalid name. The name 'TestNamespace.    ' is invalid.",
                    },
                    // Invalid names should not be checked by metadata validation in Astoria.
                    new MetadataValidationTestCase
                    {
                        Metadata = InvalidModels.CreateEntityTypeWithInvalidEdmNameMetadata,
                        ValidationExpectedExceptionMessage = (string)null,
                        NoValidationExpectedExceptionMessage = (string)null,
                    },
                    new MetadataValidationTestCase
                    {
                        Metadata = InvalidModels.CreateEntityTypeWithoutKeyMetadata,
                        ValidationExpectedExceptionMessage = "The entity type 'TestNamespace.EntityTypeWithoutKey' does not have any key properties. Please make sure that one or more key properties are defined for this entity type.",
                        NoValidationExpectedExceptionMessage =  "The entity type 'TestNamespace.EntityTypeWithoutKey' does not have any key properties. Please make sure that one or more key properties are defined for this entity type.",
                    },
                    // TODO: Test infrastructure does not support entity types with duplicate names
                    //new MetadataValidationTestCase
                    //{
                    //    Metadata = InvalidModels.CreateDuplicateEntityTypeMetadata,
                    //    ExpectedExceptionMessage = "TBD",
                    //    FailsAlways = true,
                    //},
                    // TODO: Test infrastructure does not support complex types with duplicate names
                    //new MetadataValidationTestCase
                    //{
                    //    Metadata = InvalidModels.CreateDuplicateComplexTypeMetadata,
                    //    ExpectedExceptionMessage = "TBD",
                    //    FailsAlways = true,
                    //},
                    new MetadataValidationTestCase
                    {
                        Metadata = InvalidModels.CreateComplexTypeWithDuplicatePropertiesMetadata,
                        ValidationExpectedExceptionMessage = "A property with same name 'DuplicateName' already exists in type 'TestNamespace.ComplexTypeWithDuplicatePropertyNames'. Please make sure that there is no property with the same name defined in one of the base types.",
                        NoValidationExpectedExceptionMessage =  "A property with same name 'DuplicateName' already exists in type 'TestNamespace.ComplexTypeWithDuplicatePropertyNames'. Please make sure that there is no property with the same name defined in one of the base types.",
                    },
                    new MetadataValidationTestCase
                    {
                        Metadata = InvalidModels.CreateEntityTypeWithDuplicatePropertiesMetadata,
                        ValidationExpectedExceptionMessage = "A property with same name 'DuplicateName' already exists in type 'TestNamespace.TestEntityType'. Please make sure that there is no property with the same name defined in one of the base types.",
                        NoValidationExpectedExceptionMessage = "A property with same name 'DuplicateName' already exists in type 'TestNamespace.TestEntityType'. Please make sure that there is no property with the same name defined in one of the base types.",
                    },
                };

                test.TestUtil.RunCombinations(
                    testCases,
                    new bool[] { true, false },
                    (testCase, disableMetadataValidation) =>
                    {
                        XDocument response;
                        Action testAction = () =>
                        {
                            TestWebRequest request = new DSPServiceDefinition()
                            {
                                Metadata = testCase.Metadata(),
                                DisableValidationOnMetadataWrite = disableMetadataValidation,
                            }.CreateForInProcess();

                            using (request)
                            {
                                request.RequestUriString = "/$metadata";
                                request.SendRequest();
                                response = request.GetResponseStreamAsXDocument();
                            }
                        };
                        Exception e = test.TestUtil.RunCatching(testAction);

                        if (testCase.NoValidationExpectedExceptionMessage != null ||
                            (!disableMetadataValidation && testCase.ValidationExpectedExceptionMessage != null))
                        {
                            Assert.IsNotNull(e, "The request should have failed.");

                            TargetInvocationException tie = e as TargetInvocationException;
                            Exception exceptionToCheck = tie == null ? e : tie.InnerException;

                            WebException we = e as WebException;
                            exceptionToCheck = we == null ? exceptionToCheck : we.InnerException;

                            Assert.IsNotNull(exceptionToCheck, "The exception to check should not be null.");
                            Assert.IsTrue(exceptionToCheck.Message.Contains(disableMetadataValidation ? testCase.NoValidationExpectedExceptionMessage : testCase.ValidationExpectedExceptionMessage), "Unexpected error message.");

                            DataServiceException dataServiceException = exceptionToCheck as DataServiceException;
                            if (dataServiceException != null)
                            {
                                Assert.AreEqual(500, dataServiceException.StatusCode, "Status code should be 500.");
                            }
                        }
                        else
                        {
                            Assert.IsNull(e, "The request should have succeeded.");
                        }
                    });
            }

            public class GenericPerson<T>
            {
                public int ID { get; set; }
                public T Data { get; set; }
            }

            public class GenericAddress<T, U>
            {
                public int ID { get; set; }
                public T Data { get; set; }
                public U OtherData { get; set; }
            }

            public class DataType
            {
                public string Description { get; set; }
                public byte[] Bytes { get; set; }
            }

            public class GenericEntityTypeContext<T>
            {
                public IQueryable<T> GenericEntities { get { return new T[0].AsQueryable(); } }
            }

            [TestMethod, Variation("Verify that we produce consistent type names on entity sets and entity types for generic CLR entity types.")]
            public void GenericEntityTypeNamesConsistency()
            {
                var testCases = new[]
                    {
                        new
                        {
                            DataServiceType = typeof(GenericEntityTypeContext<GenericPerson<string>>),
                            ExpectedSchemaName = "AstoriaUnitTests.Tests",
                            ExpectedEntityTypeName = "UnitTestModule_MetadataDocumentTests_GenericPerson`1[System.String]",
                            ExpectedEntityContainerName = "GenericEntityTypeContext`1"
                        },
                        new
                        {
                            DataServiceType = typeof(GenericEntityTypeContext<GenericPerson<DataType>>),
                            ExpectedSchemaName = "AstoriaUnitTests.Tests",
                            ExpectedEntityTypeName = "UnitTestModule_MetadataDocumentTests_GenericPerson`1[AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_DataType]",
                            ExpectedEntityContainerName = "GenericEntityTypeContext`1"
                        },
                        new
                        {
                            DataServiceType = typeof(GenericEntityTypeContext<GenericAddress<string, int>>),
                            ExpectedSchemaName = "AstoriaUnitTests.Tests",
                            ExpectedEntityTypeName = "UnitTestModule_MetadataDocumentTests_GenericAddress`2[System.String System.Int32]",
                            ExpectedEntityContainerName = "GenericEntityTypeContext`1"
                        },
                        new
                        {
                            DataServiceType = typeof(GenericEntityTypeContext<GenericAddress<DataType, DataType>>),
                            ExpectedSchemaName = "AstoriaUnitTests.Tests",
                            ExpectedEntityTypeName = "UnitTestModule_MetadataDocumentTests_GenericAddress`2[AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_DataType AstoriaUnitTests.Tests.UnitTestModule_MetadataDocumentTests_DataType]",
                            ExpectedEntityContainerName = "GenericEntityTypeContext`1"
                        },
                    };

                test.TestUtil.RunCombinations(
                    testCases,
                    (testCase) =>
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = testCase.DataServiceType;
                            request.RequestUriString = "/$metadata";
                            request.SendRequest();
                            var response = request.GetResponseStreamAsXDocument();
                            var entityTypeCount = response
                                .Descendants(UnitTestsUtil.EdmOasisNamespace + "Schema")
                                .Where(d => d.Attribute("Namespace").Value == testCase.ExpectedSchemaName)
                                .Elements(UnitTestsUtil.EdmOasisNamespace + "EntityType")
                                .Where(e => e.Attribute("Name").Value == testCase.ExpectedEntityTypeName)
                                .Count();
                            Assert.AreEqual(1, entityTypeCount, "Expected to find exactly 1 entity type with name '" + testCase.ExpectedEntityTypeName + "'.");

                            var entitySetCount = response
                                .Descendants(UnitTestsUtil.EdmOasisNamespace + "Schema")
                                .Where(d => d.Attribute("Namespace").Value == testCase.ExpectedSchemaName)
                                .Elements(UnitTestsUtil.EdmOasisNamespace + "EntityContainer")
                                .Where(e => e.Attribute("Name").Value == testCase.ExpectedEntityContainerName)
                                .Elements(UnitTestsUtil.EdmOasisNamespace + "EntitySet")
                                .Where(es => es.Attribute("EntityType").Value == testCase.ExpectedSchemaName + "." + testCase.ExpectedEntityTypeName)
                                .Count();
                            Assert.AreEqual(1, entitySetCount, "Expected to find exactly 1 entity set with entity type name '" + testCase.ExpectedEntityTypeName + "'.");
                        }
                    });
            }

            /// <summary>
            /// Test case class for metadata validation on write.
            /// </summary>
            private sealed class MetadataValidationTestCase
            {
                public Func<DSPMetadata> Metadata { get; set; }
                public string ValidationExpectedExceptionMessage { get; set; }
                public string NoValidationExpectedExceptionMessage { get; set; }
            }

            /// <summary>
            /// A set of invalid models used for error tests.
            /// </summary>
            private static class InvalidModels
            {
                /// <summary>
                /// Create invalid metadata (entity type with a whitespace-only name)
                /// </summary>
                /// <returns>The created (invalid) metadata instance.</returns>
                public static DSPMetadata CreateEntityTypeWithWhitespaceNameMetadata()
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                    ResourceType entityType = metadata.AddEntityType("    ", null, null, false);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    ResourceSet entitySet = metadata.AddResourceSet("Customers", entityType);
                    metadata.SetReadOnly();
                    return metadata;
                }

                /// <summary>
                /// Create metadata with an invalid EDM name (this is valid in Astoria)
                /// </summary>
                /// <returns>The created metadata instance.</returns>
                public static DSPMetadata CreateEntityTypeWithInvalidEdmNameMetadata()
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                    ResourceType entityType = metadata.AddEntityType("Customer`1", null, null, false);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    ResourceSet entitySet = metadata.AddResourceSet("Customers", entityType);
                    metadata.SetReadOnly();
                    return metadata;
                }

                /// <summary>
                /// Model with a single entity type that is lacking a key.
                /// </summary>
                public static DSPMetadata CreateEntityTypeWithoutKeyMetadata()
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                    ResourceType entityType = metadata.AddEntityType("EntityTypeWithoutKey", null, null, false);
                    ResourceSet entitySet = metadata.AddResourceSet("EntitiesWithoutKey", entityType);
                    metadata.SetReadOnly();
                    return metadata;
                }

                /// <summary>
                /// Model with two entity types that have same name.
                /// </summary>
                public static DSPMetadata CreateDuplicateEntityTypeMetadata()
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                    ResourceType entityType = metadata.AddEntityType("EntityTypeWithDuplicateName", null, null, false);
                    ResourceType entityType2 = metadata.AddEntityType("EntityTypeWithDuplicateName", null, null, false);
                    ResourceSet entitySet = metadata.AddResourceSet("EntitiesWithDuplicateName1", entityType);
                    ResourceSet entitySet2 = metadata.AddResourceSet("EntitiesWithDuplicateName2", entityType2);
                    metadata.SetReadOnly();
                    return metadata;
                }

                /// <summary>
                /// Model with two complex types that have same name.
                /// </summary>
                public static DSPMetadata CreateDuplicateComplexTypeMetadata()
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                    ResourceType complexType = metadata.AddComplexType("ComplexTypeWithDuplicateName", null, null, false);
                    ResourceType complexType2 = metadata.AddComplexType("ComplexTypeWithDuplicateName", null, null, false);
                    ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    metadata.AddComplexProperty(entityType, "First", complexType);
                    metadata.AddComplexProperty(entityType, "Second", complexType2);
                    ResourceSet entitySet = metadata.AddResourceSet("TestEntities", entityType);
                    metadata.SetReadOnly();
                    return metadata;
                }

                /// <summary>
                /// Model with a complex type that has two properties with the same name.
                /// </summary>
                public static DSPMetadata CreateComplexTypeWithDuplicatePropertiesMetadata()
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");
                    ResourceType complexType = metadata.AddComplexType("ComplexTypeWithDuplicatePropertyNames", null, null, false);
                    metadata.AddPrimitiveProperty(complexType, "DuplicateName", typeof(int));
                    metadata.AddPrimitiveProperty(complexType, "DuplicateName", typeof(string));

                    ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    metadata.AddComplexProperty(entityType, "ComplexProp", complexType);

                    ResourceSet entitySet = metadata.AddResourceSet("TestEntities", entityType);
                    metadata.SetReadOnly();
                    return metadata;
                }

                /// <summary>
                /// Model with an entity type that has two properties with the same name.
                /// </summary>
                public static DSPMetadata CreateEntityTypeWithDuplicatePropertiesMetadata()
                {
                    DSPMetadata metadata = new DSPMetadata("TestService", "TestNamespace");

                    ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    metadata.AddPrimitiveProperty(entityType, "DuplicateName", typeof(int));
                    metadata.AddPrimitiveProperty(entityType, "DuplicateName", typeof(string));

                    ResourceSet entitySet = metadata.AddResourceSet("TestEntities", entityType);
                    metadata.SetReadOnly();
                    return metadata;
                }
            }

            [TestMethod, Variation("MetadataProviderEdmFunctionImport should not blindly make all function parameters nullable but do so only in versions < V3")]
            public void VerifyNullabilityOfServiceOperationParameters()
            {
                foreach (var version in new[] { ODataProtocolVersion.V4 })
                {
                    var metadata = CreateServiceMetadata(version);

                    using (TestWebRequest request = new DSPServiceDefinition() { Metadata = metadata }.CreateForInProcess())
                    {
                        request.RequestUriString = "/$metadata";
                        request.SendRequest();
                        var metadataXml = request.GetResponseStreamAsXDocument();

                        var x = metadataXml.Descendants().Where(e => e.Name.LocalName == "Parameter").ToList();

                        Assert.IsNull(metadataXml.Descendants().Single(e => e.Name.LocalName == "Parameter" && (string)e.Attribute("Name") == "nullableParam").Attribute("Nullable"));
                        var nullableAttr = metadataXml.Descendants().Single(e => e.Name.LocalName == "Parameter" && (string)e.Attribute("Name") == "nonNullableParam").Attribute("Nullable");
                        Assert.IsNotNull(nullableAttr);
                        Assert.IsFalse((bool)nullableAttr);
                    }
                }
            }

            private static DSPMetadata CreateServiceMetadata(ODataProtocolVersion serviceVersion)
            {
                // Changed the result type to string since void Functions are no longer allowed.
                var metadata = new DSPMetadata("TestService", "TestNamespace");
                metadata.AddServiceOperation(
                    "ServiceOp",
                    ServiceOperationResultKind.DirectValue,
                    ResourceType.GetPrimitiveResourceType(typeof(string)),
                    null,
                    "GET",
                    new ServiceOperationParameter[]
                    {
                        new ServiceOperationParameter("nullableParam", metadata.GetResourceTypeFromTestSpecification(typeof(int ?))),
                        new ServiceOperationParameter("nonNullableParam", metadata.GetResourceTypeFromTestSpecification(typeof(int))),
                    });

                ResourceType entityType = metadata.AddEntityType("TestEntityType", null, null, false);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));

                // Add a collection property makes the service to be V3 and bumps the Edm version to 4.0
                metadata.AddCollectionProperty(entityType, "CollectionOfIntOnEntity", typeof(int));

                metadata.AddResourceSet("TestEntities", entityType);

                return metadata;
            }
        }
    }
}
