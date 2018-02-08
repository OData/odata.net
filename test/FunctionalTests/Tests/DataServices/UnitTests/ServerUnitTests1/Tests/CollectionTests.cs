//---------------------------------------------------------------------
// <copyright file="CollectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using providers = Microsoft.OData.Service.Providers;
    using p=Microsoft.OData.Service.Providers;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    [Ignore] // Remove Atom
    [TestModule]
    public partial class UnitTestModule1
    {
        [TestClass, TestCase]
        public partial class CollectionTest
        {
            #region CustomEnumerable
            public abstract class CustomEnumerable
            {
                protected List<object> values;
                public abstract void CheckEnumeratorsAndReset();
                public abstract void CheckUsedEnumeratorsAndReset();

                internal List<object> Values
                {
                    get { return this.values; }
                }
            }

            public class CustomEnumerable<T> : CustomEnumerable, IEnumerable<T>
            {
                private CustomEnumerator enumerator;

                public CustomEnumerable(IEnumerable underlyingEnumerable)
                {
                    if (underlyingEnumerable != null)
                    {
                        this.values = new List<object>(underlyingEnumerable.Cast<object>());
                    }
                    else
                    {
                        this.values = new List<object>();
                    }
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    Assert.IsTrue(this.enumerator == null, "Enumerator has already been created. It should be created only once.");
                    this.enumerator = new CustomEnumerator(this.values.GetEnumerator());
                    return this.enumerator;
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    Assert.Fail("The generic IEnumerable<T>.GetEnumerator was called.");
                    return null;
                }

                private class CustomEnumerator : IEnumerator, IDisposable
                {
                    private IEnumerator underlyingEnumerator;
                    internal bool disposed;

                    public CustomEnumerator(IEnumerator underlyingEnumerator)
                    {
                        this.underlyingEnumerator = underlyingEnumerator;
                        this.disposed = false;
                    }

                    public object Current
                    {
                        get { return this.underlyingEnumerator.Current; }
                    }

                    public bool MoveNext()
                    {
                        return this.underlyingEnumerator.MoveNext();
                    }

                    public void Reset()
                    {
                        Assert.Fail("IEnumerator.Reset must not be called.");
                    }

                    public void Dispose()
                    {
                        Assert.IsFalse(this.disposed, "Dispose has already been called.");
                        this.disposed = true;
                    }
                }

                public override void CheckEnumeratorsAndReset()
                {
                    Assert.IsTrue(this.enumerator != null, "No enumerator has been created yet.");
                    Assert.IsTrue(this.enumerator.disposed, "Enumerator has not been disposed yet.");
                    this.enumerator = null;
                }

                public override void CheckUsedEnumeratorsAndReset()
                {
                    if (this.enumerator != null)
                    {
                        Assert.IsTrue(this.enumerator.disposed, "Enumerator has not been disposed yet.");
                    }

                    this.enumerator = null;
                }
            }
            #endregion CustomEnumerable

            #region Serialization
            // [TestCategory("Partition1"), TestMethod(), Variation("Verifies serialization of collection properties in entity and complex types.")]
            public void Collection_CollectionPropertySerialization()
            {
                // ItemType - type of a single item in the collection, either Type for primitive types, or String - name of the complex type
                // Values - tuples, Item1 - the actual value of the item, Item2 - the expected value in the payload, Item3 - the expected type attribute in the payload
                var testCases = new[]{
                    new {
                        ItemType = (object)typeof(int),
                        Values = new Tuple<object, string>[] {
                            new Tuple<object, string>((int)0, null),
                            new Tuple<object, string>((int)1, null),
                            new Tuple<object, string>((int)-42, null),
                        }
                    },
                    new {
                        ItemType = (object)typeof(string),
                        Values = new Tuple<object, string>[] {
                            new Tuple<object, string>("", null),
                            new Tuple<object, string>("value", null),
                            new Tuple<object, string>("value", null),
                            new Tuple<object, string>(" value\t", null),
                        }
                    },
                    new {
                        ItemType = (object)typeof(string),
                        Values = new Tuple<object, string>[] {
                            new Tuple<object, string>(" ", null),
                            new Tuple<object, string>("\rvalue", null),
                            new Tuple<object, string>("value\n", null),
                            new Tuple<object, string>(" value\t", null),
                        }
                    },
                    new {
                        ItemType = (object)typeof(double?),
                        Values = new Tuple<object, string>[] {
                            new Tuple<object, string>((double?)1.52, null),
                            new Tuple<object, string>((double?)-0.54, null)
                        }
                    },
                    new {
                        ItemType = (object)"SimpleComplexType",
                        Values = new Tuple<object, string>[] {
                            new Tuple<object, string>(
                                new Func<DSPMetadata, object>((metadata) => new DSPResource(
                                    metadata.GetResourceType("SimpleComplexType"), 
                                    new KeyValuePair<string, object>[] { 
                                        new KeyValuePair<string, object>("Name", "Bart") })),
                                null),
                            new Tuple<object, string>(
                                new Func<DSPMetadata, object>((metadata) => new DSPResource(
                                    metadata.GetResourceType("SimpleComplexType"),
                                    new KeyValuePair<string, object>[] { 
                                        new KeyValuePair<string, object>("Name", "") })),
                                null),
                            new Tuple<object, string>(
                                new Func<DSPMetadata, object>((metadata) => new DSPResource(
                                    metadata.GetResourceType("SimpleComplexType"),
                                    new KeyValuePair<string, object>[] { 
                                        new KeyValuePair<string, object>("Name", null) })),
                                null),
                        }
                    }
                };

                var collectionCreateMethods = new string[] {
                    "CreateList",
                    "CreateArray",
                    "CreateCustomEnumerable"
                };

                var metadataShapes = new string[] {
                    "BaseEntity",
                    "OnComplexType",
                    "OnOpenComplexType",
                    "OnDerivedEntity"
                };

                var formats = new[] {
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false },
                    new { Format = UnitTestsUtil.MimeApplicationXml, DirectPropertyRequest = true },
                };

                TestUtil.RunCombinations(testCases, collectionCreateMethods, metadataShapes, formats, (testCase, collectionCreateMethod, metadataShape, format) =>
                {
                    // Precreate some standard metadata
                    DSPMetadata metadata = new DSPMetadata("TestCollection", "TestNamespace");
                    var entityType = metadata.AddEntityType("EntityType", null, null, false);
                    metadata.AddResourceSet("Entities", entityType);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    var derivedEntityType = metadata.AddEntityType("DerivedEntityType", null, entityType, false);
                    var complexType = metadata.AddComplexType("ComplexType", null, null, false);
                    var simpleComplexType = metadata.AddComplexType("SimpleComplexType", null, null, false);
                    metadata.AddPrimitiveProperty(simpleComplexType, "Name", typeof(string));

                    // Determine the resource type of a single item in the collection
                    var itemResourceType = metadata.GetResourceTypeFromTestSpecification(testCase.ItemType);

                    // Add the collection property
                    switch (metadataShape)
                    {
                        case "BaseEntity":
                            metadata.AddCollectionProperty(entityType, "CollectionProperty", itemResourceType);
                            break;
                        case "OnComplexType":
                            metadata.AddCollectionProperty(complexType, "CollectionProperty", itemResourceType);
                            metadata.AddComplexProperty(entityType, "ComplexProperty", complexType);
                            break;
                        case "OnOpenComplexType":
                            if (format.DirectPropertyRequest)
                            {
                                // Can't address collection property on an open property directly (we don't have a way to determine the resource type)
                                return;
                            }

                            entityType.IsOpenType = true;
                            metadata.AddCollectionProperty(complexType, "CollectionProperty", itemResourceType);
                            break;
                        case "OnDerivedEntity":
                            if (format.DirectPropertyRequest)
                            {
                                // We can't address a single property declared on derived type only.
                                return;
                            }

                            metadata.AddCollectionProperty(derivedEntityType, "CollectionProperty", itemResourceType);
                            break;
                    }

                    // Create the values (for complex types they are funcs which create the resources based on the metadata)
                    var collectionPropertyValues = testCase.Values.Select(v => new Tuple<object, string>(
                            v.Item1 is Func<DSPMetadata, object> ? ((Func<DSPMetadata, object>)v.Item1)(metadata) : v.Item1,
                            v.Item2)).ToList().Combinations().ToList();
                    List<object> collectionCreated = new List<object>();

                    // Create the entity resources (for each combination of values)
                    DSPContext data = new DSPContext();
                    var entities = data.GetResourceSetEntities("Entities");
                    int id = 0;
                    foreach (var value in collectionPropertyValues)
                    {
                        var resource = new DSPResource(entityType);
                        entities.Add(resource);
                        resource.SetValue("ID", id);
                        var resValue = typeof(CollectionTest).GetMethod(collectionCreateMethod, BindingFlags.NonPublic | BindingFlags.Static)
                            .MakeGenericMethod(itemResourceType.InstanceType)
                            .Invoke(null, new object[] { value.Select(t => t.Item1) });
                        collectionCreated.Add(resValue);
                        switch (metadataShape)
                        {
                            case "BaseEntity":
                                resource.SetRawValue("CollectionProperty", resValue);
                                break;
                            case "OnComplexType":
                            case "OnOpenComplexType":
                                {
                                    var complexResource = new DSPResource(complexType);
                                    complexResource.SetRawValue("CollectionProperty", resValue);
                                    resource.SetRawValue("ComplexProperty", complexResource);
                                }
                                break;
                            case "OnDerivedEntity":
                                {
                                    var newresource = new DSPResource(derivedEntityType);
                                    newresource.SetValue("ID", id);
                                    resource.SetValue("ID", id + 1000); // Leave the underived entity there but change its ID - verifies that "mixed" entities work
                                    newresource.SetRawValue("CollectionProperty", resValue);
                                    entities.Add(newresource);
                                }
                                break;
                        }

                        id++;
                    }

                    // Start the service
                    DSPServiceDefinition service = new DSPServiceDefinition { Metadata = metadata, DataSource = data };
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        // Walk all the entities (value combinations) and verify
                        for (int entryIndex = 0; entryIndex < collectionPropertyValues.Count; entryIndex++)
                        {
                            // If the collection property is declared on a complex type and we are going to ask for the property itself
                            //   there are two ways to do this, one is to ask for /Entities(0)/ComplexProperty/CollectionProperty
                            //   and the other is to ask for /Entities(0)/ComplexProperty and then extract the collection property from the payload.
                            // This boolean is used to run the loop twice in such case and differentiate between the two cases.
                            bool plainXmlAccessComplexPropertyInsteadOfCollection = false;
                            do
                            {
                                string requestUri = "/Entities(" + entryIndex.ToString() + ")";
                                if (format.DirectPropertyRequest)
                                {
                                    if (metadataShape == "OnComplexType")
                                    {
                                        requestUri += "/ComplexProperty";

                                        if (!plainXmlAccessComplexPropertyInsteadOfCollection)
                                        {
                                            requestUri += "/CollectionProperty";
                                        }
                                    }
                                    else
                                    {
                                        requestUri += "/CollectionProperty";
                                    }
                                }

                                var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri, format.Format);

                                XElement collectionProperty = null;
                                if (!format.DirectPropertyRequest)
                                {
                                    var properties = response.Root.Element(UnitTestsUtil.AtomNamespace + "content").Element(UnitTestsUtil.MetadataNamespace + "properties");
                                    switch (metadataShape)
                                    {
                                        case "BaseEntity":
                                            collectionProperty = properties.Element(UnitTestsUtil.DataNamespace + "CollectionProperty");
                                            break;
                                        case "OnComplexType":
                                        case "OnOpenComplexType":
                                            collectionProperty = properties.Element(UnitTestsUtil.DataNamespace + "ComplexProperty").Element(UnitTestsUtil.DataNamespace + "CollectionProperty");
                                            break;
                                        case "OnDerivedEntity":
                                            var category = response.Root.Elements(UnitTestsUtil.AtomNamespace + "category").Single(e => (string)e.Attribute("scheme") == "http://docs.oasis-open.org/odata/ns/scheme");
                                            // Skip the non-derived entities
                                            if ((string)category.Attribute("term") != derivedEntityType.FullName) continue;
                                            collectionProperty = properties.Element(UnitTestsUtil.DataNamespace + "CollectionProperty");
                                            break;
                                    }
                                }
                                else
                                {
                                    if (metadataShape == "OnComplexType" && plainXmlAccessComplexPropertyInsteadOfCollection)
                                    {
                                        collectionProperty = response.Root.Element(UnitTestsUtil.DataNamespace + "CollectionProperty");
                                    }
                                    else
                                    {
                                        collectionProperty = response.Root;
                                    }
                                }

                                VerifyCollectionPropertyValue(
                                    collectionProperty,
                                    "CollectionProperty",
                                    itemResourceType.ShortQualifiedName,
                                    collectionPropertyValues[entryIndex]);

                                if (format.DirectPropertyRequest && metadataShape == "OnComplexType" && !plainXmlAccessComplexPropertyInsteadOfCollection)
                                {
                                    plainXmlAccessComplexPropertyInsteadOfCollection = true;

                                    // We are using the values twice here, so we need to the collection to verify the enumerators and reset.
                                    CustomEnumerable customEnumerable = collectionCreated[entryIndex] as CustomEnumerable;
                                    if (customEnumerable != null) customEnumerable.CheckEnumeratorsAndReset();
                                }
                                else
                                {
                                    plainXmlAccessComplexPropertyInsteadOfCollection = false;
                                }
                            } while (plainXmlAccessComplexPropertyInsteadOfCollection);
                        }
                    }

                    // If it's a custom enumerable, verify, that enumerators were enumerated and disposed of correctly
                    foreach (var _collectionCreated in collectionCreated)
                    {
                        CustomEnumerable customEnumerable = _collectionCreated as CustomEnumerable;
                        if (customEnumerable != null) customEnumerable.CheckEnumeratorsAndReset();
                    }
                });
            }

            // [TestCategory("Partition1"), TestMethod(), Variation("Verifies that serialization fails with in-stream error when the value of the collection property is wrong.")]
            public void Collection_InvalidValues()
            {
                var testCases = new[]{
                    new {
                        CollectionPropertyValue = (object)null,
                        ExpectedExceptionMessage = AstoriaUnitTests.DataServicesResourceUtil.GetString("Serializer_CollectionCanNotBeNull", "CollectionProperty")
                    },
                    new {
                        CollectionPropertyValue = (object)new List<string>() { null },
                        ExpectedExceptionMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString("ValidationUtils_NonNullableCollectionElementsMustNotBeNull")
                    },
                    new {
                        // Something which definitely doesn't implement IEnumerable
                        CollectionPropertyValue = (object)new object(),
                        ExpectedExceptionMessage = AstoriaUnitTests.DataServicesResourceUtil.GetString("Serializer_CollectionPropertyValueMustImplementIEnumerable", "CollectionProperty")
                    },
                    new {
                        // Even though string does implement IEnumerable, it should still fail since we don't want to treat string as a collection
                        CollectionPropertyValue = (object)"some string", 
                        ExpectedExceptionMessage = AstoriaUnitTests.DataServicesResourceUtil.GetString("Serializer_CollectionPropertyValueMustImplementIEnumerable", "CollectionProperty")
                    },
                    new {
                        // Even though byte array does implement IEnumerable, it should still fail since we don't want to treat it as a collection
                        CollectionPropertyValue = (object)new byte[] { 1, 2, 3 },
                        ExpectedExceptionMessage = AstoriaUnitTests.DataServicesResourceUtil.GetString("Serializer_CollectionPropertyValueMustImplementIEnumerable", "CollectionProperty")
                    }
                };

                var formats = new[] {
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false },
                    new { Format = UnitTestsUtil.MimeApplicationXml, DirectPropertyRequest = true },
                };

                TestUtil.RunCombinations(testCases, formats, (testCase, format) =>
                {
                    DSPMetadata metadata = new DSPMetadata("TestCollection", "TestNamespace");
                    var entityType = metadata.AddEntityType("EntityType", null, null, false);
                    metadata.AddResourceSet("Entities", entityType);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    metadata.AddCollectionProperty(entityType, "CollectionProperty", typeof(string));

                    DSPContext data = new DSPContext();
                    IList<object> entities = data.GetResourceSetEntities("Entities");
                    DSPResource entity = new DSPResource(entityType);
                    entity.SetValue("ID", 0);
                    entity.SetRawValue("CollectionProperty", testCase.CollectionPropertyValue);
                    entities.Add(entity);

                    DSPServiceDefinition service = new DSPServiceDefinition { Metadata = metadata, DataSource = data };
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.Accept = format.Format;
                        request.RequestUriString = "/Entities";
                        if (format.DirectPropertyRequest)
                        {
                            request.RequestUriString += "(0)/CollectionProperty";
                        }

                        Exception exception = TestUtil.RunCatching(request.SendRequest);

                        Assert.IsNotNull(exception, "Request should have failed.");
                        Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException), "Unexpected exception type.");
                        Assert.AreEqual(testCase.ExpectedExceptionMessage, exception.InnerException.Message, "Unexpected exception message.");
                    }
                });
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verify that if the actual values of items in a collection are a mix of mismatched types, the serializer correctly fails.")]
            public void Collection_MismatchOfItemTypes()
            {
                var testCases = new[] {
                    new {
                        ItemType = (object)typeof(string),
                        Values = new object[] { "some", 42, 1.25, DateTime.Now },
                        ExpectedExceptionMessageId = "CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName",
                        JsonLightExpectedExceptionMessageId = (string)null,
                    },
                    new {
                        ItemType = (object)typeof(string),
                        Values = new object[] { "some", "ComplexTypeA" },
                        ExpectedExceptionMessageId = "ValidationUtils_UnsupportedPrimitiveType",
                        JsonLightExpectedExceptionMessageId = "ODataJsonWriter_UnsupportedValueType",
                    },
                    new {
                        ItemType = (object)typeof(int),
                        Values = new object[] { "some", 42, 1.25, DateTime.Now },
                        ExpectedExceptionMessageId = "CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName",
                        JsonLightExpectedExceptionMessageId = (string)null,
                    },
                    new {
                        ItemType = (object)typeof(int),
                        Values = new object[] { 42, "ComplexTypeA" },
                        ExpectedExceptionMessageId = "ValidationUtils_UnsupportedPrimitiveType",
                        JsonLightExpectedExceptionMessageId = "ODataJsonWriter_UnsupportedValueType",
                    },
                    new {
                        ItemType = (object)typeof(int?),
                        Values = new object[] { 42, (int?)42 },
                        ExpectedExceptionMessageId = (string)null,
                        JsonLightExpectedExceptionMessageId = (string)null,
                    },
                    new {
                        ItemType = (object)"ComplexTypeA",
                        Values = new object[] { "ComplexTypeA", "ComplexTypeB" },
                        ExpectedExceptionMessageId = "CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName",
                        JsonLightExpectedExceptionMessageId = (string)null,
                    },
                    // Inheritance of complex types is not supported in a collection
                    new {
                        ItemType = (object)"ComplexTypeA",
                        Values = new object[] { "ComplexTypeA", "ComplexTypeADerived" },
                        ExpectedExceptionMessageId = "CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName",
                        JsonLightExpectedExceptionMessageId = (string)null,
                    },
                    new {
                        ItemType = (object)"ComplexTypeA",
                        Values = new object[] { "ComplexTypeA", "some" },
                        ExpectedExceptionMessageId = "BadProvider_InvalidTypeSpecified",
                        JsonLightExpectedExceptionMessageId = "BadProvider_InvalidTypeSpecified",
                    },
                    // Wrong errors and asserts when complex property value is in fact an entity value
                    // With ODataLib the errors are now consistent and reasonable (we still fail).
                    new {
                        ItemType = (object)"ComplexTypeA",
                        Values = new object[] { "ComplexTypeA", "EntityType" },
                        ExpectedExceptionMessageId = "CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName",
                        JsonLightExpectedExceptionMessageId = (string)null,
                    }
                };

                var formats = new[] {
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false },
                    new { Format = UnitTestsUtil.MimeApplicationXml, DirectPropertyRequest = true },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = true },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = false }
                };

                TestUtil.RunCombinations(testCases, formats, (testCase, format) =>
                {
                    DSPMetadata metadata = new DSPMetadata("TestCollection", "TestNamespace");
                    var entityType = metadata.AddEntityType("EntityType", null, null, false);
                    metadata.AddResourceSet("Entities", entityType);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    var complexTypeA = metadata.AddComplexType("ComplexTypeA", null, null, false);
                    metadata.AddPrimitiveProperty(complexTypeA, "Name", typeof(string));
                    var complexTypeADerived = metadata.AddComplexType("ComplexTypeADerived", null, complexTypeA, false);
                    metadata.AddPrimitiveProperty(complexTypeADerived, "Surname", typeof(string));
                    var complexTypeB = metadata.AddComplexType("ComplexTypeB", null, null, false);
                    metadata.AddPrimitiveProperty(complexTypeB, "Name", typeof(string));
                    var collectionItemType = metadata.GetResourceTypeFromTestSpecification(testCase.ItemType);
                    metadata.AddCollectionProperty(entityType, "CollectionProperty", collectionItemType);

                    bool collectionItemTypeIsNullable = true;
                    Type collectionItemClrType = testCase.ItemType as Type;
                    if (collectionItemClrType != null)
                    {
                        collectionItemTypeIsNullable = !collectionItemClrType.IsValueType || Nullable.GetUnderlyingType(collectionItemClrType) != null;
                    }

                    DSPContext data = new DSPContext();
                    IList<object> entities = data.GetResourceSetEntities("Entities");

                    var valueCombinations = testCase.Values.Combinations(2).ToList();
                    string[] invalidTypeNames = new string[valueCombinations.Count];
                    for (int index = 0; index < valueCombinations.Count; index++)
                    {
                        DSPResource entity = new DSPResource(entityType);
                        entity.SetValue("ID", index);
                        List<object> values = new List<object>();

                        foreach (object value in valueCombinations[index])
                        {
                            if (value is string)
                            {
                                if ((string)value == "ComplexTypeA") { values.Add(new DSPResource(complexTypeA)); }
                                else if ((string)value == "ComplexTypeADerived") { values.Add(new DSPResource(complexTypeADerived)); }
                                else if ((string)value == "ComplexTypeB") { values.Add(new DSPResource(complexTypeB)); }
                                else if ((string)value == "EntityType") { values.Add(entity); }
                                else values.Add(value);
                            }
                            else
                            {
                                values.Add(value);
                            }

                            // if the invalid type name is not already calculated, 
                            // then check if value is the invalid type
                            if (invalidTypeNames[index] == null)
                            {
                                object lastAddedValue = values[values.Count - 1];
                                if (collectionItemType.ResourceTypeKind != providers.ResourceTypeKind.Primitive)
                                {
                                    if (lastAddedValue is DSPResource)
                                    {
                                        if (((DSPResource)lastAddedValue).ResourceType != collectionItemType)
                                        {
                                            invalidTypeNames[index] = ((DSPResource)lastAddedValue).ResourceType.FullName;
                                        }
                                    }
                                    else
                                    {
                                        invalidTypeNames[index] = lastAddedValue.GetType().FullName;
                                    }
                                }
                                else if (lastAddedValue.GetType() != collectionItemType.InstanceType)
                                {
                                    if (lastAddedValue is DSPResource)
                                    {
                                        invalidTypeNames[index] = typeof(DSPResource).FullName;
                                    }
                                    else
                                    {
                                        invalidTypeNames[index] = lastAddedValue.GetType().FullName.Replace("System.", "Edm.").Replace("DateTime", "DateTimeOffset");
                                    }
                                }
                            }
                        }

                        entity.SetRawValue("CollectionProperty", values);
                        entities.Add(entity);
                    }

                    DSPServiceDefinition service = new DSPServiceDefinition { Metadata = metadata, DataSource = data };
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        for (int index = 0; index < valueCombinations.Count; index++)
                        {
                            request.Accept = format.Format;
                            request.RequestUriString = "/Entities(" + index.ToString() + ")";
                            if (format.DirectPropertyRequest)
                            {
                                request.RequestUriString += "/CollectionProperty";
                            }

                            Exception exception = TestUtil.RunCatching(request.SendRequest);

                            if (testCase.ExpectedExceptionMessageId == null)
                            {
                                Assert.IsNull(exception, "Unexpected exception.");
                            }
                            else if (IsJsonLight(format.Format))
                            {
                                string expectedMessage = null;
                                string actualMessage = exception == null ? null : exception.InnerException.Message;

                                if (testCase.JsonLightExpectedExceptionMessageId == "ValidationUtils_IncompatiblePrimitiveItemType")
                                {
                                    // Note: this isn't testing the nullability of the expected vs actual (we use collectionItemTypeIsNullable for both).
                                    actualMessage = actualMessage.Replace("[Nullable=True]", "[Nullable=False]");
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.JsonLightExpectedExceptionMessageId, invalidTypeNames[index], false, collectionItemType.FullName, false);
                                }
                                else if (testCase.JsonLightExpectedExceptionMessageId == "ValidationUtils_IncompatibleType")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.JsonLightExpectedExceptionMessageId, invalidTypeNames[index], collectionItemType.FullName);
                                }
                                else if (testCase.JsonLightExpectedExceptionMessageId == "ValidationUtils_UnsupportedPrimitiveType")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.JsonLightExpectedExceptionMessageId, invalidTypeNames[index]);
                                }
                                else if (testCase.JsonLightExpectedExceptionMessageId == "CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.JsonLightExpectedExceptionMessageId, invalidTypeNames[index], collectionItemType.FullName);
                                }
                                else if (testCase.JsonLightExpectedExceptionMessageId == "ValidationUtils_IncorrectTypeKind")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", invalidTypeNames[index], "Complex", "Entity");
                                }
                                else if (testCase.JsonLightExpectedExceptionMessageId == "ODataJsonWriter_UnsupportedValueType")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString("ODataJsonWriter_UnsupportedValueType", invalidTypeNames[index]);
                                }
                                else if (testCase.JsonLightExpectedExceptionMessageId != null)
                                {
                                    expectedMessage = AstoriaUnitTests.DataServicesResourceUtil.GetString(testCase.JsonLightExpectedExceptionMessageId, invalidTypeNames[index]);
                                }

                                Assert.AreEqual(expectedMessage, actualMessage, "Unexpected exception message -" + index);
                            }
                            else
                            {
                                Assert.IsNotNull(exception, "Exception was expected, the request should have failed.");
                                Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException), "Unexpected exception type.");

                                string expectedMessage = null;
                                if (testCase.ExpectedExceptionMessageId == "ValidationUtils_IncompatiblePrimitiveItemType")
                                {
                                    // Note: this isn't testing the nullability of the expected vs actual (we use collectionItemTypeIsNullable for both).
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.ExpectedExceptionMessageId, invalidTypeNames[index], collectionItemTypeIsNullable, collectionItemType.FullName, collectionItemTypeIsNullable);
                                }
                                else if (testCase.ExpectedExceptionMessageId == "ValidationUtils_IncompatibleType")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.ExpectedExceptionMessageId, invalidTypeNames[index], collectionItemType.FullName);
                                }
                                else if (testCase.ExpectedExceptionMessageId == "ValidationUtils_UnsupportedPrimitiveType")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.ExpectedExceptionMessageId, invalidTypeNames[index]);
                                }
                                else if (testCase.ExpectedExceptionMessageId == "CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName")
                                {
                                    expectedMessage = AstoriaUnitTests.ODataLibResourceUtil.GetString(testCase.ExpectedExceptionMessageId, invalidTypeNames[index], collectionItemType.FullName);
                                }
                                else
                                {
                                    expectedMessage = AstoriaUnitTests.DataServicesResourceUtil.GetString(testCase.ExpectedExceptionMessageId, invalidTypeNames[index]);
                                }

                                Assert.AreEqual(expectedMessage, exception.InnerException.Message, "Unexpected exception message -" + index);
                            }
                        }
                    }
                });
            }

            private static bool IsJsonLight(string format)
            {
                var lowercaseFormat = format.ToLowerInvariant();
                if (!(lowercaseFormat.Contains("atom") || lowercaseFormat.Contains("application/xml")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // [TestCategory("Partition1"), TestMethod(), Variation("Verifies that serializer correctly detects loops and fails.")]
            public void Collection_EndlessLoop()
            {
                var testCases = new[] {
                    new {
                        CreateComplexResource = new Func<DSPMetadata, object>((metadata) => {
                            DSPResource complex = new DSPResource(metadata.GetResourceType("ComplexType"));
                            complex.SetValue("CollectionProperty", new List<DSPResource> { complex });
                            return complex;
                        }),
                        ExpectedExceptionMessageId = "Serializer_LoopsNotAllowedInComplexTypes"
                    },
                    new {
                        CreateComplexResource = new Func<DSPMetadata, object>((metadata) => {
                            DSPResource complex = new DSPResource(metadata.GetResourceType("ComplexType"));
                            DSPResource complex2 = new DSPResource(metadata.GetResourceType("ComplexType"));
                            complex.SetValue("CollectionProperty", new List<DSPResource> { complex2 });
                            complex2.SetValue("CollectionProperty", new List<DSPResource>());
                            return complex;
                        }),
                        ExpectedExceptionMessageId = (string)null
                    },
                    new {
                        CreateComplexResource = new Func<DSPMetadata, object>((metadata) => {
                            DSPResource complex = new DSPResource(metadata.GetResourceType("ComplexType"));
                            DSPResource complex2 = new DSPResource(metadata.GetResourceType("ComplexType"));
                            DSPResource complex3 = new DSPResource(metadata.GetResourceType("ComplexType"));
                            complex.SetValue("CollectionProperty", new List<DSPResource> { complex2, complex3 });
                            complex2.SetValue("CollectionProperty", new List<DSPResource>() { complex3, complex });
                            complex3.SetValue("CollectionProperty", new List<DSPResource>());
                            return complex;
                        }),
                        ExpectedExceptionMessageId = "Serializer_LoopsNotAllowedInComplexTypes"
                    }
                };

                var formats = new[] {
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false },
                    new { Format = UnitTestsUtil.MimeApplicationXml, DirectPropertyRequest = true },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = false },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = true }
                };

                TestUtil.RunCombinations(testCases, formats, (testCase, format) =>
                {
                    DSPMetadata metadata = new DSPMetadata("TestCollection", "TestNamespace");
                    var entityType = metadata.AddEntityType("EntityType", null, null, false);
                    metadata.AddResourceSet("Entities", entityType);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    var complexType = metadata.AddComplexType("ComplexType", null, null, false);
                    metadata.AddCollectionProperty(complexType, "CollectionProperty", complexType);
                    metadata.AddComplexProperty(entityType, "ComplexProperty", complexType);

                    DSPContext data = new DSPContext();
                    IList<object> entities = data.GetResourceSetEntities("Entities");
                    DSPResource entity = new DSPResource(entityType);
                    entity.SetValue("ID", 0);
                    entity.SetValue("ComplexProperty", testCase.CreateComplexResource(metadata));
                    entities.Add(entity);

                    DSPServiceDefinition service = new DSPServiceDefinition { Metadata = metadata, DataSource = data };
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.Accept = format.Format;
                        request.RequestUriString = "/Entities";
                        if (format.DirectPropertyRequest)
                        {
                            request.RequestUriString += "(0)/ComplexProperty";
                        }

                        Exception exception = TestUtil.RunCatching(request.SendRequest);

                        if (testCase.ExpectedExceptionMessageId == null)
                        {
                            Assert.IsNull(exception, "No exception was expected.");
                        }
                        else
                        {
                            Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException), "Unexpected exception type.");
                            string expectedExceptionMessage =
                                AstoriaUnitTests.DataServicesResourceUtil.GetString("Serializer_LoopsNotAllowedInComplexTypes", "CollectionProperty");
                            Assert.AreEqual(expectedExceptionMessage, exception.InnerException.Message, "Unexpected exception message.");
                        }
                    }
                });
            }

            public class ReflectionContext
            {
                // Note - reflection provider doesn't support inheritance on complex types, so just a simple case here
                public class ReflectionComplexType
                {
                    public string Name { get; set; }
                }

                public class ReflectionEntityType
                {
                    public int ID { get; set; }
                    public List<string> Names { get; set; }
                    public List<ReflectionComplexType> ComplexCollection { get; set; }
                }

                private static List<ReflectionEntityType> entities;

                static ReflectionContext()
                {
                    ReflectionEntityType entity = new ReflectionEntityType() { ID = 0, Names = new List<string>(), ComplexCollection = new List<ReflectionComplexType>() };
                    entity.Names.Add("");
                    entity.Names.Add("value");
                    entity.ComplexCollection.Add(new ReflectionComplexType() { Name = "Bart" });
                    entity.ComplexCollection.Add(new ReflectionComplexType() { Name = "Homer" });
                    entities = new List<ReflectionEntityType>();
                    entities.Add(entity);
                }

                public IQueryable<ReflectionEntityType> Entities { get { return entities.AsQueryable(); } }
            }

            // [TestCategory("Partition1"), TestMethod(), Variation("Verify that serialization works on reflection based collection.")]
            public void Collection_SerializationOfReflectionBasedCollection()
            {
                var formats = new[] {
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false },
                    new { Format = UnitTestsUtil.MimeApplicationXml, DirectPropertyRequest = true },
                };

                TestUtil.RunCombinations(formats, (format) =>
                {
                    using (TestWebRequest request = (new OpenWebDataServiceDefinition { DataServiceType = typeof(OpenWebDataService<ReflectionContext>) }).CreateForInProcess())
                    {
                        string requestUri = "/Entities(0)";
                        XElement namesPropertyElement = null, complexCollectionPropertyElement = null;

                        if (!format.DirectPropertyRequest)
                        {
                            XDocument response = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri, format.Format);
                            var properties = response.Root.Element(UnitTestsUtil.AtomNamespace + "content").Element(UnitTestsUtil.MetadataNamespace + "properties");
                            namesPropertyElement = properties.Element(UnitTestsUtil.DataNamespace + "Names");
                            complexCollectionPropertyElement = properties.Element(UnitTestsUtil.DataNamespace + "ComplexCollection");
                        }
                        else
                        {
                            namesPropertyElement = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri + "/Names", format.Format).Root;
                            complexCollectionPropertyElement = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri + "/ComplexCollection", format.Format).Root;
                        }

                        VerifyCollectionPropertyValue(namesPropertyElement, "Names", "String", new Tuple<object, string>[] {
                                new Tuple<object, string>("", null),
                                new Tuple<object, string>("value", null)
                            });

                        VerifyCollectionPropertyValue(complexCollectionPropertyElement, "ComplexCollection",
                            "AstoriaUnitTests.Tests.UnitTestModule1_CollectionTest_ReflectionContext_ReflectionComplexType", new Tuple<object, string>[] {
                                new Tuple<object, string>(new DSPResource(null, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Name", "Bart") }), null),
                                new Tuple<object, string>(new DSPResource(null, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Name", "Homer") }), null),
                            });
                    }
                });
            }

            // [TestCategory("Partition1"), TestMethod(), Variation("Verify that in json light, we write the collection type name in full metadata")]
            public void Collection_VerificationOfCollectionTypeNamesInJsonLightFullMetadata()
            {
                var testCases = new[] {
                    new { Uri = "/Entities(0)" , XPaths = new string [] {
                        String.Format("/Object[{0}='#Collection(String)']", XmlConvert.EncodeName("Names@odata.type")),
                        String.Format("/Object[{0}='#Collection({1})']", XmlConvert.EncodeName("ComplexCollection@odata.type"), typeof(ReflectionContext.ReflectionComplexType).FullName.Replace('+', '_')) } },
                    new { Uri = "/Entities(0)/Names", XPaths = new string[] {
                        "/Object[odata.type='#Collection(String)']" } }
                };

                TestUtil.RunCombinations(testCases , (testCase) =>
                {
                    using (TestWebRequest request = (new OpenWebDataServiceDefinition { DataServiceType = typeof(OpenWebDataService<ReflectionContext>) }).CreateForInProcess())
                    {
                        request.RequestUriString = testCase.Uri;
                        request.Accept = UnitTestsUtil.JsonLightMimeTypeFullMetadata;
                        request.SendRequest();

                        XmlDocument response = request.GetResponseStreamAsXmlDocument(request.ResponseContentType);
                        UnitTestsUtil.VerifyXPaths(response, testCase.XPaths);
                    }
                });
            }
            #endregion Serialization

            #region Deserialization

            // Use derived type for the actual collection value since the inner storage of the product is List<object> and we want to make sure that
            //   the product doesn't confuse its own storage values with our "user" values. Note that this is only for Asserts to work correctly
            //   the product should not make such mistakes and we have specific tests for that here as well.
            public class InternalCollectionPropertyValueStore<T> : List<T>
            {
            }

            /// <summary>Helper class to represent a resource with a collection property and preform some check on it.</summary>
            public class DSPResourceWithCollectionProperty : DSPResource
            {
                private List<string> collectionPropertiesSet;

                // whether to allow setting collection properties more than once
                private static Restorable<bool> collectionPropertiesResettable = new Restorable<bool>();
                public static Restorable<bool> CollectionPropertiesResettable { get { return collectionPropertiesResettable; } }

                public DSPResourceWithCollectionProperty(providers.ResourceType resourceType)
                    : base(resourceType)
                {
                    this.ResetCollectionPropertiesSet();
                    foreach (providers.ResourceProperty collectionProperty in resourceType.Properties.Where(p => p.ResourceType.ResourceTypeKind == providers.ResourceTypeKind.Collection))
                    {
                        this.SetRawValue(collectionProperty.Name, CreateInternalCollectionPropertyValueStoreInstance(collectionProperty));
                    }
                }

                private static IList CreateInternalCollectionPropertyValueStoreInstance(providers.ResourceProperty collectionProperty)
                {
                    Debug.Assert(collectionProperty.ResourceType is providers.CollectionResourceType, "Only collection properties are supported by this method.");
                    Type t = typeof(InternalCollectionPropertyValueStore<>).MakeGenericType((collectionProperty.ResourceType as providers.CollectionResourceType).ItemType.InstanceType);
                    return (IList)Activator.CreateInstance(t);
                }

                public List<string> CollectionPropertiesSet
                {
                    get { return this.collectionPropertiesSet; }
                }

                public void ResetCollectionPropertiesSet()
                {
                    this.collectionPropertiesSet = new List<string>();
                }

                public override void SetValue(string propertyName, object value)
                {
                    providers.ResourceProperty property = this.ResourceType.Properties.FirstOrDefault(p => p.Name == propertyName);

                    if (property != null && property.ResourceType.ResourceTypeKind == providers.ResourceTypeKind.Collection)
                    {
                        providers.CollectionResourceType CollectionResourceType = (providers.CollectionResourceType)property.ResourceType;

                        // ResetResource will call SetValue with null, so deal with it here.
                        //   if it gets called by the serializer (would be a bug) we would catch in the tests when we verify the content of the collection.
                        if (value == null)
                        {
                            this.SetRawValue(propertyName, CreateInternalCollectionPropertyValueStoreInstance(property));
                            return;
                        }
                        else
                        {
                            // Otherwise the value must not be any known type other than IEnumerable
                            Assert.IsNotInstanceOfType(value, typeof(List<object>), "Collection property value must be set with a value which implements only IEnumerable.");
                            Assert.IsNotInstanceOfType(value, typeof(IList), "Collection property value must be set with a value which implements only IEnumerable.");
                            Assert.IsTrue(
                                !value.GetType().IsGenericType || value.GetType().GetGenericTypeDefinition() != typeof(InternalCollectionPropertyValueStore<>),
                                "Collection property value must be set with a value which implements only IEnumerable.");
                        }

                        if (!CollectionPropertiesResettable.Value)
                        {
                            Assert.AreEqual(false, this.CollectionPropertiesSet.Contains(propertyName), "The collection property was already set once, this should not happen more than once.");
                            this.CollectionPropertiesSet.Add(propertyName);
                        }
                        else
                        {
                            if (!this.CollectionPropertiesSet.Contains(propertyName))
                            {
                                this.CollectionPropertiesSet.Add(propertyName);
                            }
                        }

                        IEnumerable enumerable = value as IEnumerable;
                        Assert.IsNotNull(enumerable, "Collection property values must be set as IEnumerable.");

                        List<object> items = new List<object>(enumerable.Cast<object>());
                        this.SetRawValue("Original" + propertyName, new List<object>(items));

                        IList result = CreateInternalCollectionPropertyValueStoreInstance(property);

                        // Now walk the items and convert the values if the TItemType is not object
                        for (int i = 0; i < items.Count; i++)
                        {
                            Assert.IsNotNull(items[i], "Collection item is null, this is not allowed.");

                            if (CollectionResourceType.ItemType.InstanceType == typeof(int))
                            {
                                if (items[i] is string)
                                {
                                    items[i] = Int32.Parse(items[i] as string);
                                }
                            }
                            else if (CollectionResourceType.ItemType.InstanceType == typeof(string))
                            {
                                if (items[i] is int)
                                {
                                    items[i] = ((int)items[i]).ToString();
                                }
                            }
                            else if (CollectionResourceType.ItemType.InstanceType == typeof(double?))
                            {
                                if (items[i] is string)
                                {
                                    items[i] = Double.Parse(items[i] as string);
                                }
                            }

                            result.Add(items[i]);
                        }

                        this.SetRawValue(propertyName, result);
                    }
                    else if (property != null && property.ResourceType.InstanceType == typeof(int))
                    {
                        if (value is string)
                        {
                            value = Int32.Parse((string)value);
                        }

                        base.SetRawValue(propertyName, value);
                    }
                    else
                    {
                        base.SetValue(propertyName, value);
                    }
                }
            }

            private class DeserializationTestCase
            {
                public object ItemType { get; set; }
                public string[] Values { get; set; }
                public Func<string, DSPServiceDefinition, bool> IgnoreIf { get; set; }
            }

            private class DeserializationInvalidTestCase
            {
                public object ItemType { get; set; }
                public string CollectionPropertyPayload { get; set; }
                public string XmlCollectionPropertyPayload { get; set; }
                public string JsonCollectionPropertyPayload { get; set; }
                public string ExpectedExceptionMessage { get; set; }
                public string XmlExpectedExceptionMessage { get; set; }
                public string JsonExpectedExceptionMessage { get; set; }
                public string ExpectedTopLevelExceptionMessage { get; set; }
                public Func<string, DSPServiceDefinition, bool> IgnoreIf { get; set; }
                public Func<string, bool, bool, string, string> ExpectedExceptionMessageCallback { get; set; }
                public Type DSPResourceType { get; set; }

                public override string ToString()
                {
                    string payload = this.CollectionPropertyPayload ?? this.XmlCollectionPropertyPayload ?? this.JsonCollectionPropertyPayload;
                    return "Payload: " + payload;
                }
            }

            internal class DSPResourceWithCollectionProperty_VerifyEnumeratesJustOnce : DSPResourceWithCollectionProperty
            {
                public DSPResourceWithCollectionProperty_VerifyEnumeratesJustOnce(providers.ResourceType resourceType) : base(resourceType) { }

                public override void SetValue(string propertyName, object value)
                {
                    if (propertyName == "CollectionProperty" && value != null)
                    {
                        IEnumerable enumerable = value as IEnumerable;
                        enumerable.GetEnumerator();
                        enumerable.GetEnumerator();
                    }

                    base.SetValue(propertyName, value);
                }
            }

            internal class DSPResourceWithCollectionProperty_VerifyNoReset : DSPResourceWithCollectionProperty
            {
                public DSPResourceWithCollectionProperty_VerifyNoReset(providers.ResourceType resourceType) : base(resourceType) { }

                public override void SetValue(string propertyName, object value)
                {
                    if (propertyName == "CollectionProperty" && value != null)
                    {
                        Type valueType = value.GetType();
                        var interfaces = valueType.GetInterfaces();
                        Assert.AreEqual(1, interfaces.Count(), "Only one interface should be implemented by the enumerable.");
                        Assert.AreEqual(typeof(IEnumerable), interfaces[0], "The IEnumerable needs to be implemented by the value type.");

                        IEnumerable enumerable = (IEnumerable)value;
                        IEnumerator enumerator = enumerable.GetEnumerator();
                        Type enumeratorType = enumerator.GetType();
                        interfaces = enumeratorType.GetInterfaces();
                        Assert.AreEqual(2, interfaces.Count(), "Only two interfaces should be implemented by the enumerator.");
                        Assert.IsTrue(interfaces.Contains(typeof(IEnumerator)), "The IEnumerator must be implemented.");
                        Assert.IsTrue(interfaces.Contains(typeof(IDisposable)), "The IDisposable must be implemented.");

                        enumerator.MoveNext();
                        enumerator.MoveNext();
                        enumerator.Reset();
                    }

                    base.SetValue(propertyName, value);
                }
            }

            internal class DSPResourceWithCollectionProperty_VerifyDisposeAndMove : DSPResourceWithCollectionProperty
            {
                public DSPResourceWithCollectionProperty_VerifyDisposeAndMove(providers.ResourceType resourceType) : base(resourceType) { }

                public override void SetValue(string propertyName, object value)
                {
                    if (propertyName == "CollectionProperty" && value != null)
                    {
                        IEnumerable enumerable = (IEnumerable)value;
                        IEnumerator enumerator = enumerable.GetEnumerator();
                        enumerator.MoveNext();
                        IDisposable disposable = (IDisposable)enumerator;
                        disposable.Dispose();
                        disposable.Dispose();
                        enumerator.MoveNext();
                    }

                    base.SetValue(propertyName, value);
                }
            }

            internal class DSPResourceWithCollectionProperty_VerifyDisposeAndCurrent : DSPResourceWithCollectionProperty
            {
                public DSPResourceWithCollectionProperty_VerifyDisposeAndCurrent(providers.ResourceType resourceType) : base(resourceType) { }

                public override void SetValue(string propertyName, object value)
                {
                    if (propertyName == "CollectionProperty" && value != null)
                    {
                        IEnumerable enumerable = (IEnumerable)value;
                        IEnumerator enumerator = enumerable.GetEnumerator();
                        enumerator.MoveNext();
                        IDisposable disposable = (IDisposable)enumerator;
                        disposable.Dispose();
                        disposable.Dispose();
                        object item = enumerator.Current;
                    }

                    base.SetValue(propertyName, value);
                }
            }

            // [TestCategory("Partition1"), TestMethod(), Variation("Verifies failure to deserialize of invalid collection properties payloads.")]
            public void Collection_DeserializationOfInvalidPayloads()
            {
                var testCases = new DeserializationInvalidTestCase[] {
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><ads:foo/></ads:CollectionProperty>",
                        ExpectedExceptionMessage = ODataLibResourceUtil.GetString("ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement", "foo", "http://docs.oasis-open.org/odata/ns/metadata"),
                    },
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' adsm:null='true' " + TestUtil.CommonPayloadNamespaces + "/>",
                        ExpectedTopLevelExceptionMessage = ODataLibResourceUtil.GetString("ReaderValidationUtils_NullNamedValueForNonNullableType", "CollectionProperty", "Collection(Edm.Int32)"),
                        XmlExpectedExceptionMessage = ODataLibResourceUtil.GetString("ReaderValidationUtils_NullValueForNonNullableType", "Collection(Edm.Int32)"),
                        JsonExpectedExceptionMessage = ODataLibResourceUtil.GetString("ReaderValidationUtils_NullValueForNonNullableType", "Collection(Edm.Int32)")
                    },
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element adsm:null='true'/></ads:CollectionProperty>",
                        ExpectedExceptionMessageCallback = (format, enableTypeConversion, directPropertyRequest, metadataShape) => enableTypeConversion ?
                            ODataLibResourceUtil.GetString("ReaderValidationUtils_NullValueForNonNullableType", "Edm.Int32") :
                            ODataLibResourceUtil.GetString("ValidationUtils_NonNullableCollectionElementsMustNotBeNull")
                    },
                    // In ODataLib text nodes are ignored in values, so this is a valid empty collection
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + ">42</ads:CollectionProperty>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection(Edm.Int32)\", \"value\": 42 }",
                        XmlExpectedExceptionMessage = null,
                        JsonExpectedExceptionMessage = ODataLibResourceUtil.GetString("JsonReaderExtensions_UnexpectedNodeDetected", "StartArray", "PrimitiveValue")
                    },
                    // In ODataLib text nodes are ignored in values, so this is a valid empty collection
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><![CDATA[<adsm:element>42</adsm:element>]]></ads:CollectionProperty>",
                        XmlExpectedExceptionMessage = null,
                    },
                    // In ODataLib text nodes are ignored in values, so this is a valid empty collection
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element>42</adsm:element>mixed content<adsm:element>43</adsm:element></ads:CollectionProperty>",
                        XmlExpectedExceptionMessage = null,
                    },
                    // ODataLib Lax validation allows unknown types, it ignores them
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Foo' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Foo\", \"value\": [] }",
                        ExpectedExceptionMessage = null
                    },
                    // ODataLib Lax validation uses the type from metadata if the type in the payload is of a different kind
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Edm.String' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Edm.String\", \"value\": [] }",
                        ExpectedExceptionMessage = null
                    },
                    // ODataLib Lax validation allows unknown types, it ignores them
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.String' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection(Edm.String\", \"value\": [] }",
                        ExpectedExceptionMessage = null
                    },
                    // ODataLib Lax validation allows unknown types, it ignores them
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection (Edm.String)' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection (Edm.String)\", \"value\": [] }",
                        ExpectedExceptionMessage = null
                    },
                    // ODataLib Lax validation allows unknown types, it ignores them
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection( Edm.String)' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection( Edm.String)\", \"value\": [] }",
                        ExpectedExceptionMessage = null
                    },
                    // ODataLib Lax validation allows unknown types, it ignores them
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection()' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection()\" }, \"value\": [] }",
                        ExpectedExceptionMessage = null
                    },
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"\", \"value\": [] }",
                        // ODataLib Lax validation allows empty types, it ignores them
                        ExpectedExceptionMessage = null
                    },
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Collection(Edm.String))' " + TestUtil.CommonPayloadNamespaces + "/>",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection(Collection(Edm.String))\", \"value\": [] }",
                        ExpectedExceptionMessage = ODataLibResourceUtil.GetString("ValidationUtils_NestedCollectionsAreNotSupported")
                    },
                    // ODataLib Lax validation allows unknown types, it ignores them
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.String)' " + TestUtil.CommonPayloadNamespaces + "/>",
                        ExpectedExceptionMessage = null
                    },
                    // ODataLib Lax validation allows unknown types, it ignores them
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(TestNamespace.SimpleComplexType)' " + TestUtil.CommonPayloadNamespaces + "/>",
                        ExpectedExceptionMessage = null
                    },
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(TestNamespace.EntityType)' " + TestUtil.CommonPayloadNamespaces + "/>",
                        ExpectedExceptionMessage = ODataLibResourceUtil.GetString("EdmLibraryExtensions_CollectionItemCanBeOnlyPrimitiveEnumComplex"),
                    },
                    // Wrong name of the XML property in the payload
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        XmlCollectionPropertyPayload = "<ads:Foo adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "/>",
                        ExpectedExceptionMessage = ODataLibResourceUtil.GetString("ValidationUtils_PropertyDoesNotExistOnType", "Foo", "$(TypeWithCollectionProperty)"),
                        IgnoreIf = (format, service) => format != UnitTestsUtil.AtomFormat
                    },
                    // Complex value in a collection of primitive types
                    new DeserializationInvalidTestCase {
                        ItemType = (object)typeof(int),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element adsm:type='TestNamespace.SimpleComplexType'><ads:Name>Bart</ads:Name></adsm:element></ads:CollectionProperty>",
                        ExpectedExceptionMessageCallback = (format, enableTypeConversion, directPropertyRequest, metadataShape) => {
                            if (format == UnitTestsUtil.JsonLightMimeType)
                            {
                                if (enableTypeConversion) return ODataLibResourceUtil.GetString("ODataJsonPropertyAndValueDeserializer_InvalidPrimitiveTypeName", "TestNamespace.SimpleComplexType");
                                return ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "TestNamespace.SimpleComplexType", "Primitive", "Complex");
                            }
                            if (enableTypeConversion) return ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element");
                            return ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "TestNamespace.SimpleComplexType", "Primitive", "Complex");
                        }
                    },
                    // Unfortunately the XML deserializer ignores mixed content/text value when deserializing complex types, so even <m:element>42</m:element>
                    //   as an item in collection of complex types will pass, will materialize as empty element (no properties set).
                    // Similar JSON payload correctly fails
                    new DeserializationInvalidTestCase {
                        ItemType = "SimpleComplexType",
                        JsonCollectionPropertyPayload = "[ 42 ]",
                        ExpectedExceptionMessageCallback = (format, enableTypeConversion, directPropertyRequest, metadataShape) => directPropertyRequest || metadataShape != "BaseEntity" ?
                            ODataLibResourceUtil.GetString("ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue", "StartArray") :
                            ODataLibResourceUtil.GetString("ODataJsonPropertyAndValueDeserializer_CannotReadPropertyValue", "StartArray")
                    },
                    // Since the only way to specify a self/edit link for XML payload is using ATOM, there's no way to express a self/edit link for a complex type
                    // In JSON it is possible, and it should fail if such thing is used inside a collection (it won't fail outside of collection because we will think it's an entity)
                    //    Note that outside of collection (normal complex property), this case is broken as it asserts and fails with 500.
                    // With ODataLib this doesn't fail anymore since ODL ignores additional properties in __metadata which it doesn't consume.
                    new DeserializationInvalidTestCase {
                        ItemType = "SimpleComplexType",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection(TestNamespace.SimpleComplexType)\", \"value\": [ {\"@odata.type\": \"TestNamespace.SimpleComplexType\", \"Name\": \"Bart\" } ] }",
                    },
                    // Use of derived complex type should pass
                    new DeserializationInvalidTestCase {
                        ItemType = "SimpleComplexType",
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(TestNamespace.SimpleComplexType)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element adsm:type='TestNamespace.DerivedComplexType'><ads:Name>Bart</ads:Name><ads:Surname>Simpson</ads:Surname></adsm:element></ads:CollectionProperty>",
                        ExpectedExceptionMessage = null
                    },
                    // JSON collection value non-object
                    new DeserializationInvalidTestCase {
                        ItemType = typeof(int),
                        JsonCollectionPropertyPayload = "42",
                        JsonExpectedExceptionMessage = ODataLibResourceUtil.GetString("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                    },
                    // JSON collection __metadata type non-string
                    new DeserializationInvalidTestCase {
                        ItemType = typeof(int),
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": 42, \"value\": [] }",
                        JsonExpectedExceptionMessage = ODataLibResourceUtil.GetString("ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName", "42")
                    },
                    // JSON collection missing results element
                    new DeserializationInvalidTestCase {
                        ItemType = typeof(int),
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection(Edm.Int32)\" }",
                        JsonExpectedExceptionMessage = ODataLibResourceUtil.GetString("ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyWithPrimitiveNullValue")
                    },
                    // JSON collection with complex item which is not an object record
                    new DeserializationInvalidTestCase {
                        ItemType = "SimpleComplexType",
                        JsonCollectionPropertyPayload = "{ \"@odata.type\": \"Collection(TestNamespace.SimpleComplexType)\", \"value\": [ \"some\" ] }",
                        JsonExpectedExceptionMessage = ODataLibResourceUtil.GetString("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                    },
                    // Verify that the enumerable specified as a value for the collection property can be enumerated just once
                    new DeserializationInvalidTestCase {
                        ItemType = typeof(int),
                        DSPResourceType = typeof(DSPResourceWithCollectionProperty_VerifyEnumeratesJustOnce),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element>1</adsm:element></ads:CollectionProperty>",
                        ExpectedExceptionMessage = DataServicesResourceUtil.GetString("CollectionCanOnlyBeEnumeratedOnce")
                    },
                    // Verify that the enumerable specified as a value for the collection property does not support IEnumerator.Reset
                    new DeserializationInvalidTestCase {
                        ItemType = typeof(int),
                        DSPResourceType = typeof(DSPResourceWithCollectionProperty_VerifyNoReset),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element>1</adsm:element></ads:CollectionProperty>",
                        ExpectedExceptionMessage = "Specified method is not supported."
                    },
                    // Verify that the enumerable specified as a value for the collection property has enumerator which can be disposed, but then no MoveNext is supported anymore
                    new DeserializationInvalidTestCase {
                        ItemType = typeof(int),
                        DSPResourceType = typeof(DSPResourceWithCollectionProperty_VerifyDisposeAndMove),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element>1</adsm:element></ads:CollectionProperty>",
                        ExpectedExceptionMessage = "Cannot access a disposed object.\r\nObject name: 'CollectionPropertyValueEnumerator'."
                    },
                    // Verify that the enumerable specified as a value for the collection property has enumerator which can be disposed, but then no Current is supported anymore
                    new DeserializationInvalidTestCase {
                        ItemType = typeof(int),
                        DSPResourceType = typeof(DSPResourceWithCollectionProperty_VerifyDisposeAndCurrent),
                        CollectionPropertyPayload = "<ads:CollectionProperty adsm:type='Collection(Edm.Int32)' " + TestUtil.CommonPayloadNamespaces + "><adsm:element>1</adsm:element></ads:CollectionProperty>",
                        ExpectedExceptionMessage = "Cannot access a disposed object.\r\nObject name: 'CollectionPropertyValueEnumerator'."
                    },
                };

                var metadataShapes = new string[] {
                    "BaseEntity",
                    "OnComplexType",
                };

                // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                // To Add json verbose, make sure that the json light test cases are correct and then uncomment it below.
                var requestTypes = new[] {
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false, Method = "PUT" },
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false, Method = "PATCH" },
                    new { Format = UnitTestsUtil.AtomFormat, DirectPropertyRequest = false, Method = "POST" },
                    new { Format = UnitTestsUtil.MimeApplicationXml, DirectPropertyRequest = true, Method = "PUT" },
                    new { Format = UnitTestsUtil.MimeApplicationXml, DirectPropertyRequest = true, Method = "PATCH" },
                   /* new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = false, Method = "PUT" },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = false, Method = "MERGE" },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = false, Method = "POST" },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = true, Method = "PUT" },
                    new { Format = UnitTestsUtil.JsonLightMimeType, DirectPropertyRequest = true, Method = "MERGE" } */
                };

                TestUtil.RunCombinations(testCases, new bool[] { true, false }, requestTypes, metadataShapes, (testCase, enableTypeConversion, requestType, metadataShape) =>
                {
                    DSPMetadata metadata = new DSPMetadata("TestCollection", "TestNamespace");
                    var simpleComplexType = metadata.AddComplexType("SimpleComplexType", typeof(DSPResourceWithCollectionProperty), null, false);
                    metadata.AddPrimitiveProperty(simpleComplexType, "Name", typeof(string));
                    metadata.AddCollectionProperty(simpleComplexType, "InnerCollection", typeof(int));
                    var derivedComplexType = metadata.AddComplexType("DerivedComplexType", typeof(DSPResourceWithCollectionProperty), simpleComplexType, false);
                    metadata.AddPrimitiveProperty(derivedComplexType, "Surname", typeof(string));

                    // Determine the resource type of a single item in the collection
                    var itemResourceType = metadata.GetResourceTypeFromTestSpecification(testCase.ItemType);
                    Type dspResourceWithCollectionPropertyType = testCase.DSPResourceType ?? typeof(DSPResourceWithCollectionProperty);
                    Func<providers.ResourceType, DSPResource> createResourceWithCollectionProperty =
                        (resourceType) => { return (DSPResource)Activator.CreateInstance(dspResourceWithCollectionPropertyType, resourceType); };

                    var entityType = metadata.AddEntityType("EntityType", dspResourceWithCollectionPropertyType, null, false);
                    metadata.AddResourceSet("Entities", entityType);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    var derivedEntityType = metadata.AddEntityType("DerivedEntityType", dspResourceWithCollectionPropertyType, entityType, false);
                    var complexTypeWithCollection = metadata.AddComplexType("ComplexTypeWithCollection", dspResourceWithCollectionPropertyType, null, false);
                    metadata.AddComplexProperty(entityType, "ComplexProperty", complexTypeWithCollection);

                    providers.ResourceType typeWithCollectionProperty = null;
                    switch (metadataShape)
                    {
                        case "BaseEntity": metadata.AddCollectionProperty(entityType, "CollectionProperty", itemResourceType); typeWithCollectionProperty = entityType; break;
                        case "OnComplexType": metadata.AddCollectionProperty(complexTypeWithCollection, "CollectionProperty", itemResourceType); typeWithCollectionProperty = complexTypeWithCollection; break;
                        case "OnDerivedEntity":
                            if (requestType.DirectPropertyRequest) return;
                            metadata.AddCollectionProperty(derivedEntityType, "CollectionProperty", itemResourceType); typeWithCollectionProperty = derivedEntityType; break;
                    }

                    var usedEntityType = metadataShape == "OnDerivedEntity" ? derivedEntityType : entityType;

                    DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, Writable = true };
                    service.CreateDataSource = (m) =>
                    {
                        DSPContext data = new DSPContext();
                        var entities = data.GetResourceSetEntities("Entities");

                        if (requestType.Method != "POST")
                        {
                            var resource = createResourceWithCollectionProperty(usedEntityType);
                            entities.Add(resource);
                            resource.SetValue("ID", 0);
                            var complex = createResourceWithCollectionProperty(complexTypeWithCollection);
                            complex.SetValue("Name", "0");
                            complex.SetValue("InnerCollection", new List<object>());
                            resource.SetValue("ComplexProperty", complex);
                        }

                        return data;
                    };
                    service.EnableTypeConversion = enableTypeConversion;

                    if (testCase.IgnoreIf != null && testCase.IgnoreIf(requestType.Format, service)) return;

                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        XDocument xmlCollectionPropertyPayload = null;
                        if (testCase.CollectionPropertyPayload != null) xmlCollectionPropertyPayload = XDocument.Parse(testCase.CollectionPropertyPayload);
                        if (testCase.XmlCollectionPropertyPayload != null && requestType.Format != UnitTestsUtil.JsonLightMimeType) xmlCollectionPropertyPayload = XDocument.Parse(testCase.XmlCollectionPropertyPayload);
                        if (testCase.JsonCollectionPropertyPayload != null && requestType.Format == UnitTestsUtil.JsonLightMimeType)
                        {
                            xmlCollectionPropertyPayload = new XDocument(
                                new XElement(UnitTestsUtil.DataNamespace + "CollectionProperty",
                                    new XElement(UnitTestsUtil.JsonInXmlPayloadNamespace + "JsonRepresentation", testCase.JsonCollectionPropertyPayload)));
                        }

                        if (xmlCollectionPropertyPayload == null)
                        {
                            return;
                        }

                        XDocument payload;
                        if (requestType.DirectPropertyRequest)
                        {
                            payload = xmlCollectionPropertyPayload;
                        }
                        else
                        {
                            XElement dataProperty = null;
                            switch (metadataShape)
                            {
                                case "BaseEntity":
                                case "OnDerivedEntity":
                                    dataProperty = xmlCollectionPropertyPayload.Root;
                                    break;
                                case "OnComplexType":
                                    dataProperty = new XElement(UnitTestsUtil.DataNamespace + "ComplexProperty",
                                        new XAttribute(UnitTestsUtil.MetadataNamespace + "type", complexTypeWithCollection.FullName),
                                        xmlCollectionPropertyPayload.Root);
                                    break;
                            }

                            payload = new XDocument(
                                new XElement(UnitTestsUtil.AtomNamespace + "entry",
                                    new XElement(UnitTestsUtil.AtomNamespace + "category",
                                        new XAttribute("term", usedEntityType.FullName),
                                        new XAttribute("scheme", "http://docs.oasis-open.org/odata/ns/scheme")),
                                    new XElement(UnitTestsUtil.AtomNamespace + "content",
                                        new XAttribute("type", "application/xml"),
                                        new XElement(UnitTestsUtil.MetadataNamespace + "properties",
                                            new XElement(UnitTestsUtil.DataNamespace + "ID", "0"),
                                            dataProperty))));
                        }

                        request.SetRequestStreamAsText(requestType.Format == UnitTestsUtil.JsonLightMimeType ? UnitTestsUtil.Atom2JsonXLinq(payload) : payload.ToString());
                        string requestUri = "/Entities";
                        if (requestType.Method != "POST")
                        {
                            requestUri += "(0)";
                            if (requestType.DirectPropertyRequest)
                            {
                                if (metadataShape == "OnComplexType") requestUri += "/ComplexProperty";
                                requestUri += "/CollectionProperty";
                            }
                        }

                        request.RequestUriString = requestUri;
                        request.RequestContentType = requestType.Format;
                        request.HttpMethod = requestType.Method;

                        Exception exception = TestUtil.RunCatching(request.SendRequest);

                        string expectedExceptionMessage = testCase.ExpectedExceptionMessage;
                        if (testCase.XmlExpectedExceptionMessage != null && requestType.Format != UnitTestsUtil.JsonLightMimeType)
                        {
                            if (!requestType.DirectPropertyRequest && testCase.ExpectedTopLevelExceptionMessage != null)
                            {
                                expectedExceptionMessage = testCase.ExpectedTopLevelExceptionMessage;
                            }
                            else
                            {
                                expectedExceptionMessage = testCase.XmlExpectedExceptionMessage;
                            }
                        }
                        if (testCase.JsonExpectedExceptionMessage != null && requestType.Format == UnitTestsUtil.JsonLightMimeType)
                        {
                            if (!requestType.DirectPropertyRequest && testCase.ExpectedTopLevelExceptionMessage != null)
                            {
                                expectedExceptionMessage = testCase.ExpectedTopLevelExceptionMessage;
                            }
                            else
                            {
                                expectedExceptionMessage = testCase.JsonExpectedExceptionMessage;
                            } 
                        }
                        if (testCase.ExpectedExceptionMessageCallback != null)
                        {
                            expectedExceptionMessage = testCase.ExpectedExceptionMessageCallback(requestType.Format, enableTypeConversion, requestType.DirectPropertyRequest, metadataShape);
                        }

                        if (expectedExceptionMessage != null)
                        {
                            expectedExceptionMessage = UnitTestsUtil.ProcessStringVariables(expectedExceptionMessage, (variableName) =>
                            {
                                if (variableName == "TypeWithCollectionProperty") return typeWithCollectionProperty.FullName;
                                return null;
                            });

                            Assert.IsNotNull(exception, "The request should have failed with exception message " + expectedExceptionMessage);
                            while (true)
                            {
                                if (exception.InnerException != null)
                                {
                                    exception = exception.InnerException;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            Assert.AreEqual(expectedExceptionMessage, exception.Message, "Wrong exception message.");
                        }
                        else
                        {
                            Assert.IsNull(exception, "The request should have succeeded, but it failed with " + (exception == null ? null : exception.ToString()));
                        }
                    }
                });
            }

            private void VerifyDeserializedCollectionPropertyValue(XElement collectionXml, List<object> collectionPropertyValue, providers.CollectionResourceType expectedResourceType, DSPServiceDefinition service, string format)
            {
                string collectionTypeName = (string)collectionXml.Attribute(UnitTestsUtil.MetadataNamespace + "type");
                var collectionType = collectionTypeName == null ? null : GetCollectionResourceTypeFromName(service.Metadata, collectionTypeName);
                var actualCollectionItemType = collectionType == null ? expectedResourceType.ItemType : collectionType.ItemType;

                int itemIndex = 0;
                foreach (var itemXml in collectionXml.Elements(UnitTestsUtil.MetadataNamespace + "element"))
                {
                    Assert.IsTrue(itemIndex < collectionPropertyValue.Count, "The collection doesn't have enough items.");
                    object itemValue = collectionPropertyValue[itemIndex++];
                    VerifyDeserializedPropertyValue(itemXml, itemValue, actualCollectionItemType, service, format);
                }

                Assert.AreEqual(itemIndex, collectionPropertyValue.Count, "The collection has too many items.");
            }

            private void VerifyDeserializedPropertyValue(XElement propertyXml, object value, providers.ResourceType expectedType, DSPServiceDefinition service, string format)
            {
                string typeName = (string)propertyXml.Attribute(UnitTestsUtil.MetadataNamespace + "type");
                providers.ResourceType resourceType = null;
                if (typeName == null)
                {
                    resourceType = expectedType;
                }
                else
                {
                    resourceType = GetCollectionResourceTypeFromName(service.Metadata, typeName) ?? service.Metadata.GetResourceTypeFromTestSpecification(typeName);
                }

                switch (resourceType.ResourceTypeKind)
                {
                    case providers.ResourceTypeKind.Collection:
                        VerifyDeserializedCollectionPropertyValue(propertyXml, (List<object>)value, (providers.CollectionResourceType)expectedType, service, format);
                        break;
                    case providers.ResourceTypeKind.ComplexType:
                        VerifyDeserializedComplexValue(propertyXml, (DSPResource)value, expectedType, service, format);
                        break;
                    case providers.ResourceTypeKind.Primitive:
                        VerifyDeserializedPrimitiveValue(propertyXml, value, expectedType, service, format);
                        break;
                }
            }

            private void VerifyDeserializedComplexValue(XElement complexXml, DSPResource resource, providers.ResourceType expectedResourceType, DSPServiceDefinition service, string format)
            {
                string typeName = (string)complexXml.Attribute(UnitTestsUtil.MetadataNamespace + "type");
                var payloadResourceType = typeName == null ? expectedResourceType : service.Metadata.GetResourceTypeFromTestSpecification(typeName);
                Assert.AreEqual(payloadResourceType, resource.ResourceType, "The resource type of a complex object doesn't match.");

                foreach (var propertyXml in complexXml.Elements().Where(e => e.Name.Namespace == UnitTestsUtil.DataNamespace))
                {
                    string propertyName = propertyXml.Name.LocalName;
                    object propertyValue = resource.GetValue(propertyName);
                    if (propertyName.Contains("Collection"))
                    {
                        propertyValue = resource.GetValue("Original" + propertyName);
                    }

                    VerifyDeserializedPropertyValue(propertyXml, propertyValue,
                        payloadResourceType.Properties.Single(rp => rp.Name == propertyName).ResourceType, service, format);
                }
            }

            private void VerifyDeserializedPrimitiveValue(XElement primitiveXml, object value, providers.ResourceType expectedResourceType, DSPServiceDefinition service, string format)
            {
                string typeName = (string)primitiveXml.Attribute(UnitTestsUtil.MetadataNamespace + "type");
                var payloadResourceType = service.Metadata.GetResourceTypeFromTestSpecification(typeName ?? "Edm.String"); // The default type is string
                if (service.EnableTypeConversion.Value)
                {
                    // If the type conversion was enabled the actual type of the value should exactly match the excepted type (as declared in the metadata)
                    CompareCollectionItemTypes(expectedResourceType.InstanceType, value.GetType());
                }
                else
                {
                    // If the type conversion was disable, the actual type of the value should exactly match the payload type
                    // Unless this was JSON payload in which case our test framework (conversion from ATOM to JSON) can't tell
                    //   if the test wanted the item type be a string (as in XML) or for example int (as in JSON) in the case where no type was specified.
                    if (format == UnitTestsUtil.JsonLightMimeType && typeName == null)
                    {
                        CompareCollectionItemTypes(expectedResourceType.InstanceType, value.GetType());
                    }
                    else
                    {
                        CompareCollectionItemTypes(payloadResourceType.InstanceType, value.GetType());
                    }
                }
                Assert.AreEqual(primitiveXml.Value, value.ToString(), "Item value doesn't match.");
            }

            private static void CompareCollectionItemTypes(Type expectedType, Type actualType)
            {
                expectedType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
                Assert.AreEqual(expectedType, actualType, "The item has wrong type.");
            }

            #endregion Deserialization

            #region Uri parsing and query options
            // [TestCategory("Partition1"), TestMethod, Variation("Verify that uri parsing correctly fails in case collection property is not addressed directly without any query options.")]
            public void Collection_UriParsing()
            {
                var testCases = new[] {
                    new { RequestUri = "/Entities?$select=CollectionProperty/Item", ExpectedExceptionMessage = "Found a path with multiple navigation properties or a bad complex property path in a select clause. Please reword your query such that each level of select or expand only contains either TypeSegments or Properties." },
                    new { RequestUri = "/Entities?$filter=CollectionProperty", ExpectedExceptionMessage = "The $filter expression must evaluate to a single boolean value." },
                    new { RequestUri = "/Entities?$filter=CollectionProperty/Item", ExpectedExceptionMessage = "The parent value for a property access of a property 'Item' is not a single value. Property access can only be applied to a single value." },
                    new { RequestUri = "/Entities?$filter=length(CollectionProperty)", ExpectedExceptionMessage = "The argument for an invocation of a function with name 'length' is not a single value. All arguments for this function must be single values." },
                    new { RequestUri = "/Entities?$orderby=CollectionProperty", ExpectedExceptionMessage = "The $orderby expression must evaluate to a single value of primitive type." },
                    new { RequestUri = "/Entities?$orderby=CollectionProperty/Item", ExpectedExceptionMessage = "The parent value for a property access of a property 'Item' is not a single value. Property access can only be applied to a single value." },
                    new { RequestUri = "/Entities?$orderby=length(CollectionProperty)", ExpectedExceptionMessage = "The argument for an invocation of a function with name 'length' is not a single value. All arguments for this function must be single values." },
                    new { RequestUri = "/Entities(0)/CollectionProperty/$value", ExpectedExceptionMessage = "$value cannot be applied to a collection." },
                    new { RequestUri = "/Entities(0)/CollectionProperty/Name", ExpectedExceptionMessage = "The request URI is not valid. Since the segment 'CollectionProperty' refers to a collection, this must be the last segment in the request URI or it must be followed by an function or action that can be bound to it otherwise all intermediate segments must refer to a single resource." },
                    new { RequestUri = "/Entities(0)/CollectionProperty()", ExpectedExceptionMessage = "Bad Request - Error in query syntax." },
                    new { RequestUri = "/Entities(0)/CollectionProperty?$top=1", ExpectedExceptionMessage = "Query options $orderby, $count, $skip and $top cannot be applied to the requested resource." },
                    new { RequestUri = "/Entities(0)/CollectionProperty?$skip=1", ExpectedExceptionMessage = "Query options $orderby, $count, $skip and $top cannot be applied to the requested resource." },
                    new { RequestUri = "/Entities(0)/CollectionProperty?$filter=1 eq 1", ExpectedExceptionMessage = "Query option $filter cannot be applied to the requested resource." },
                    new { RequestUri = "/Entities(0)/CollectionProperty?$orderby=1", ExpectedExceptionMessage = "Query options $orderby, $count, $skip and $top cannot be applied to the requested resource." },
                    new { RequestUri = "/Entities(0)/CollectionProperty?$select=*", ExpectedExceptionMessage = "Query option $select cannot be applied to the requested resource." },
                    new { RequestUri = "/Entities(0)/CollectionProperty?$expand=foo", ExpectedExceptionMessage = "Query option $expand cannot be applied to the requested resource." },
                    new { RequestUri = "/Entities(0)/CollectionProperty?$skiptoken=1", ExpectedExceptionMessage = "Skip tokens can only be provided for requests that return collections of entities." },
                    new { RequestUri = "/Entities(0)/CollectionProperty/$ref", ExpectedExceptionMessage = "The request URI is not valid. $ref cannot be applied to the segment 'CollectionProperty' since $ref can only follow an entity segment or entity collection segment." },

                    // trailing slash in the URI is ignored
                    new { RequestUri = "/Entities(0)/CollectionProperty/", ExpectedExceptionMessage = (string)null },
                };

                var collectionProperties = new[] {
                    new { ItemType = (object)typeof(int), Value = (object)new List<int> { 1, 2, 3 } },
                    new { ItemType = (object)"ComplexType", Value = (object)new List<DSPResource>(){} },
                };

                TestUtil.RunCombinations(testCases, collectionProperties, (testCase, collectionProperty) =>
                {
                    using (TestWebRequest request = CreateServiceWithCollectionProperty(false, collectionProperty.ItemType, collectionProperty.Value).CreateForInProcess())
                    {
                        request.RequestUriString = testCase.RequestUri;
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        if (testCase.ExpectedExceptionMessage == null)
                        {
                            Assert.IsNull(exception, "Unexpected exception.");
                        }
                        else
                        {
                            Assert.IsNotNull(exception, "Request should fail, expected exception.");
                            Assert.IsInstanceOfType(exception.InnerException, typeof(DataServiceException), "Unexpected exception type.");
                            Assert.AreEqual(testCase.ExpectedExceptionMessage, exception.InnerException.Message, "Wrong exception message.");
                        }
                    }
                });
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verify that projections work correctly over collection properties.")]
            public void Collection_Projections()
            {
                var testCases = new[] {
                    new { ItemType = (object)typeof(int), Values = (object)new List<int>() { 1, 2, 3 } },
                    new { ItemType = (object)typeof(int), Values = (object)new List<int>() { } },
                    new { ItemType = (object)typeof(string), Values = (object)new List<string>() { "some", "" } },
                    new { ItemType = (object)typeof(DateTime), Values = (object)new List<DateTime>() { DateTime.Now } },
                    new { ItemType = (object)"SimpleComplexType", Values = (object)new List<DSPResource>() { } },
                };

                string[] responseFormats = new string[] { UnitTestsUtil.AtomFormat };

                TestUtil.RunCombinations(testCases, new bool[] { false, true }, responseFormats, (testCase, useInheritance, format) =>
                {
                    using (TestWebRequest request = CreateServiceWithCollectionProperty(useInheritance, testCase.ItemType, testCase.Values).CreateForInProcess())
                    {
                        var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities?$select=CollectionProperty", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//atom:entry/atom:content/adsm:properties/ads:CollectionProperty",
                            "atom:feed[not(//atom:entry/atom:content/adsm:properties/ads:ID)]");
                    }
                });
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verify expression tree shape used for projecting collection properties.")]
            public void Collection_ProjectionExpressionTree()
            {
                OpenWebDataServiceDefinition service = CreateServiceWithCollectionProperty(false, typeof(int), new List<int> { 1, 2, 3 });
                service.ServiceConstructionCallback = ExpressionTreeTestUtils.RegisterOnService;
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.RequestUriString = "/Entities?$select=CollectionProperty";
                    request.SendRequest();
                }
                var expressionTree = ExpressionTreeTestUtils.GetLastExpressionTreeXml();
                UnitTestsUtil.VerifyXPathExists(
                    expressionTree,
                    "//MemberAssignment[@member='ProjectedProperty0']//Call[Method='GetSequenceValue' and Arguments/Constant='CollectionProperty']");


                service = new OpenWebDataServiceDefinition { DataServiceType = typeof(OpenWebDataService<ReflectionContext>) };
                service.ServiceConstructionCallback = ExpressionTreeTestUtils.RegisterOnService;
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.RequestUriString = "/Entities?$select=Names,ComplexCollection";
                    request.SendRequest();
                }
                expressionTree = ExpressionTreeTestUtils.GetLastExpressionTreeXml();
                UnitTestsUtil.VerifyXPathExists(
                    expressionTree,
                    "//MemberAssignment[@member='ProjectedProperty0']//MemberAccess[Member='Names']",
                    "//MemberAssignment[@member='ProjectedProperty1']//MemberAccess[Member='ComplexCollection']");
            }

            private DSPServiceDefinition CreateServiceWithCollectionProperty(bool useInheritance, object itemType, object values)
            {
                DSPMetadata metadata = new DSPMetadata("TestCollection", "TestNamespace");
                var entityType = metadata.AddEntityType("EntityType", null, null, false);
                metadata.AddResourceSet("Entities", entityType);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                providers.ResourceType derivedEntityType = null;
                if (useInheritance)
                {
                    derivedEntityType = metadata.AddEntityType("DerivedEntityType", null, entityType, false);
                }
                var simpleComplexType = metadata.AddComplexType("SimpleComplexType", null, null, false);
                metadata.AddPrimitiveProperty(simpleComplexType, "Name", typeof(string));
                var complexType = metadata.AddComplexType("ComplexType", null, null, false);
                metadata.AddPrimitiveProperty(complexType, "Name", typeof(string));
                metadata.AddComplexProperty(entityType, "ComplexProperty", complexType);
                metadata.AddCollectionProperty(entityType, "CollectionProperty", metadata.GetResourceTypeFromTestSpecification(itemType));

                DSPContext dataSource = new DSPContext();
                var entities = dataSource.GetResourceSetEntities("Entities");
                var resource = new DSPResource(useInheritance ? derivedEntityType : entityType);
                resource.SetValue("ID", 0);
                resource.SetValue("CollectionProperty", values);
                entities.Add(resource);

                return new DSPServiceDefinition() { Metadata = metadata, DataSource = dataSource };
            }

            #endregion Uri parsing and query options

            #region Open properties
            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that open collection properties are supported.")]
            public void Collection_NoOpenCollectionProperties()
            {
                // Note that we expect these errors because the provider is reporting these types are Collections, not because of their actual CLR types
                var testCases = new[] {
                    new {
                        MetadataShape = "EntityWithOpenCollection",  // Open collection property on an entity, entire entity is requested
                    },
                    new {
                        MetadataShape = "OpenCollectionProperty", // Open collection property on an entity, the collection property itself is requested
                    },
                    new {
                        MetadataShape = "CollectionOnOpenProperty", // Declared collection property on an open complex property, the collection property itself is requested
                    }
                };

                TestUtil.RunCombinations(testCases, UnitTestsUtil.ResponseFormats, (testCase, format) =>
                {
                    DSPMetadata metadata = new DSPMetadata("Test", "TestNamespace");
                    var itemType = metadata.AddEntityType("Item", null, null, false);
                    metadata.AddKeyProperty(itemType, "ID", typeof(int));
                    itemType.IsOpenType = true;
                    var addressType = metadata.AddComplexType("Address", null, null, false);
                    if (testCase.MetadataShape == "CollectionOnOpenProperty" || testCase.MetadataShape == "EntityWithCollectionOnOpenPropertyEpm")
                    {
                        metadata.AddCollectionProperty(addressType, "Collection", typeof(int));
                    }

                    metadata.AddResourceSet("Items", itemType);
                    SetReadOnlyAndCheckCollectionPropagation(metadata);

                    DSPContext context = new DSPContext();
                    var items = context.GetResourceSetEntities("Items");
                    var item = new DSPResource(itemType);
                    item.SetRawValue("ID", 0);
                    if (testCase.MetadataShape == "CollectionOnOpenProperty" || testCase.MetadataShape == "EntityWithCollectionOnOpenPropertyEpm")
                    {
                        var address = new DSPResource(addressType);
                        address.SetRawValue("Collection", (object)new List<int>() {1, 2, 3});
                        item.SetRawValue("Address", address);
                    }
                    else
                    {
                        item.SetRawValue("Collection", new List<int>() {1, 2, 3});
                    }
                    items.Add(item);

                    DSPServiceDefinition service = new DSPServiceDefinition() {Metadata = metadata, DataSource = context,};
                    service.DataServiceType = typeof(OpenCollectionTestService);
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        if ((testCase.MetadataShape == "CollectionOnOpenProperty" || testCase.MetadataShape == "OpenCollectionProperty") && format == UnitTestsUtil.AtomFormat)
                        {
                            request.Accept = UnitTestsUtil.MimeApplicationXml;
                        }
                        else
                        {
                            request.Accept = format;
                        }

                        switch (testCase.MetadataShape)
                        {
                            case "EntityWithOpenCollection":
                                request.RequestUriString = "/Items";
                                break;
                            case "OpenCollectionProperty":
                                request.RequestUriString = "/Items(0)/Collection";
                                break;
                            case "CollectionOnOpenProperty":
                                request.RequestUriString = "/Items(0)/Address/Collection";
                                break;
                            case "EntityWithCollectionOnOpenPropertyEpm":
                                request.RequestUriString = "/Items";
                                break;
                        }
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNull(exception, "Request should have passed.");
                    }
                });
            }

            public class OpenCollectionQueryProvider : DSPResourceQueryProvider
            {
                /// <summary>Constructor.</summary>
                /// <param name="metadata">The metadata provider.</param>
                public OpenCollectionQueryProvider(DSPMetadata metadata, bool enableCustomPaging) 
                    : base(metadata, enableCustomPaging)
                {
                }

                /// <summary>
                /// If we don't find the ResourceType in the normal way, we always return a new ResourceType that has ResourceTypeKind.Collection to help
                /// test Open Collection properties.
                /// </summary>
                public override p.ResourceType GetResourceType(object target)
                {
                    p.ResourceType type = base.GetResourceType(target);
                    if (type == null)
                    {
                        type = p.ResourceType.GetCollectionResourceType(p.ResourceType.GetPrimitiveResourceType(typeof(int)));
                    }

                    return type;
                }
            }

            public class OpenCollectionTestService : DSPDataService
            {
                protected override DSPResourceQueryProvider CreateDSPResourceQueryProvider()
                {
                    return DSPServiceDefinition.Current.ResourceQueryProvider ?? new OpenCollectionQueryProvider(this.Metadata, DSPServiceDefinition.Current.EnableCustomPaging);
                }
            }
            #endregion

            #region DBNull handling
            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that collection properties can correctly handle DBNull values from the provider.")]
            public void Collection_DBNull()
            {
                object[] itemTypes = new object[] { typeof(int), typeof(string), "Address" };

                TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, itemTypes, UnitTestsUtil.BooleanValues,
                    (collectionOnComplexType, collectionItemType, dbnullCollectionPropertyValue) =>
                    {
                        DSPMetadata metadata = new DSPMetadata("Test", "TestNamespace");
                        var entityType = metadata.AddEntityType("Entity", null, null, false);
                        metadata.AddKeyProperty(entityType, "ID", typeof(int));
                        // We will use the DSPResource as the actual storage type for the address, but in order to be able to store DBNull
                        //   we need to declare it as using typeof(object). Note that the DSP providers work based on the actual instance, not on the declared storage type
                        //   so all the infrastructure will continue to work correctly.
                        var addressType = metadata.AddComplexType("Address", typeof(object), null, false);
                        metadata.AddPrimitiveProperty(addressType, "Name", typeof(string));
                        // If the storage type is not DSPResource (or null which means DSPResource), the metadata marks the type as CanReflect=true
                        //   assuming the type is a true CLR storage type (it can't assume DSPResource then). We need to fool it here, so mark it back as
                        //   CanReflect=false since we will always use DSPResource in fact.
                        addressType.CanReflectOnInstanceType = false;
                        providers.ResourceType collectionItemResourceType = metadata.GetResourceTypeFromTestSpecification(collectionItemType);
                        if (collectionOnComplexType)
                        {
                            metadata.AddCollectionProperty(addressType, "CollectionProperty", collectionItemResourceType);
                            metadata.AddComplexProperty(entityType, "Address", addressType);
                        }
                        else
                        {
                            metadata.AddCollectionProperty(entityType, "CollectionProperty", collectionItemResourceType);
                        }
                        metadata.AddResourceSet("Entities", entityType);
                        SetReadOnlyAndCheckCollectionPropagation(metadata);

                        DSPServiceDefinition service = new DSPServiceDefinition()
                        {
                            Metadata = metadata,
                            Writable = true,
                            CreateDataSource = (m) =>
                            {
                                DSPContext context = new DSPContext();
                                DSPResource item = new DSPResource(entityType);
                                item.SetRawValue("ID", 1);
                                DSPResource resourceWithCollection = item;
                                if (collectionOnComplexType)
                                {
                                    resourceWithCollection = new DSPResource(addressType);
                                    item.SetRawValue("Address", resourceWithCollection);
                                }
                                if (dbnullCollectionPropertyValue)
                                {
                                    resourceWithCollection.SetRawValue("CollectionProperty", DBNull.Value);
                                }
                                else
                                {
                                    Type enumerableType = typeof(CustomEnumerable<>).MakeGenericType(collectionItemResourceType.InstanceType);
                                    object value = Activator.CreateInstance(enumerableType, new object[] { new List<object>() { DBNull.Value } });
                                    resourceWithCollection.SetRawValue("CollectionProperty", value);
                                }
                                context.GetResourceSetEntities("Entities").Add(item);
                                return context;
                            },
                            ExpressionTreeInterceptor = (expr) => { return new RemoveConvertForCollectionPropertyExpressionVisitor().Visit(expr); }
                        };
                        using (TestWebRequest request = service.CreateForInProcess())
                        {
                            TestUtil.RunCombinations(new string[] { "entity", "projection" , "complex", "collection" }, UnitTestsUtil.ResponseFormats, (accessType, format) =>
                            {
                                // Reset the instances cause the CustomEnumerable only allows single enumeration ever.
                                // This will recreated everything by calling CreateDataSource.
                                service.ClearChanges();
                                switch (accessType)
                                {
                                    case "entity":
                                        request.RequestUriString = "/Entities";
                                        break;
                                    case "projection":
                                        request.RequestUriString = collectionOnComplexType ? "/Entities?$select=Address" : "/Entities?$select=CollectionProperty";
                                        break;
                                    case "complex":
                                        if (!collectionOnComplexType) return;
                                        request.RequestUriString = "/Entities(1)/Address";
                                        if (format == UnitTestsUtil.AtomFormat) format = UnitTestsUtil.MimeApplicationXml;
                                        break;
                                    case "collection":
                                        request.RequestUriString = collectionOnComplexType ? "/Entities(1)/Address/CollectionProperty" : "/Entities(1)/CollectionProperty";
                                        if (format == UnitTestsUtil.AtomFormat) format = UnitTestsUtil.MimeApplicationXml;
                                        break;
                                }
                                request.Accept = format;
                                Exception e = TestUtil.RunCatching(request.SendRequest);
                                Assert.IsNotNull(e, "The request should have failed.");
                                e = e.InnerException;
                                Assert.AreEqual(500, request.ResponseStatusCode, "Expected InternalServerError for null items in collectionProperty");
                                if (dbnullCollectionPropertyValue)
                                {
                                    Assert.AreEqual(DataServicesResourceUtil.GetString("Serializer_CollectionCanNotBeNull", "CollectionProperty"), e.Message);
                                }
                                else
                                {
                                    Assert.AreEqual(ODataLibResourceUtil.GetString("ValidationUtils_NonNullableCollectionElementsMustNotBeNull"), e.Message, "The error string was not as expected");
                                }
                            });
                        }
                    });
            }

            internal class RemoveConvertForCollectionPropertyExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
            {
                protected override Expression VisitMethodCall(MethodCallExpression node)
                {
                    // In the queries we use the only place where a property access to a collection property is performed will be in projection expression
                    //   in the assignment of the projected property. That's the place where we want to remove the Convert around the property access
                    //   so that we can return DBNull even though it is not the instance type of the property.
                    //   Some providers (like EF, which doesn't support collection yet) do this and ignore the real convert in the expression and return DBNull
                    //   as value for a property if it is null in the DB. This is to simulate that behavior. (our LINQ to Objects provider used underneath
                    //   would fail if we would not fixup the expression tree).
                    // Property access is a call to the GetSequenceValue method (Collection properties are accessed through GetSequenceValue)
                    if (node.Method.Name == "GetSequenceValue" && node.Arguments.Count == 2 && node.Arguments[1] is System.Linq.Expressions.ConstantExpression)
                    {
                        // The second argument is the ResourceProperty representing the collection property
                        var resourcePropertyConstant = (System.Linq.Expressions.ConstantExpression)node.Arguments[1];
                        var property = (providers.ResourceProperty)resourcePropertyConstant.Value;
                        if (property.Kind == providers.ResourcePropertyKind.Collection)
                        {
                            // We have to translate the GetSequence because the default translation would try to cast it to the IEnumerable<T>
                            // Just call the targetResource.GetValue(resourceProperty.Name)
                            return Expression.Call(
                                    this.Visit(node.Arguments[0]),
                                    typeof(DSPResource).GetMethod("GetValue"),
                                    Expression.Property(node.Arguments[1], "Name"));
                        }
                    }

                    return base.VisitMethodCall(node);
                }
            }
            #endregion

            #region ETags
            public class DSPResourceWithVersionProperty : DSPResourceWithCollectionProperty
            {
                private DSPResourceWithVersionProperty parent;

                public DSPResourceWithVersionProperty(providers.ResourceType resourceType)
                    : this(resourceType, null)
                {
                }

                public DSPResourceWithVersionProperty(providers.ResourceType resourceType, DSPResourceWithVersionProperty parent)
                    : base(resourceType)
                {
                    this.parent = parent;
                    this.SetRawValue("Version", 0);
                }

                public override void SetValue(string propertyName, object value)
                {
                    // Version value is non-modifyable by the client
                    if (propertyName != "Version")
                    {
                        base.SetValue(propertyName, value);
                    }

                    this.ResourceModified();
                }

                private void ResourceModified()
                {
                    if (this.parent != null)
                    {
                        this.parent.ResourceModified();
                    }
                    else
                    {
                        int version = (int)this.GetValue("Version");
                        this.SetRawValue("Version", version + 1);
                    }
                }
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verifies server generates the right ETag values and uses them correctly even when collections are involved.")]
            public void Collection_ETags()
            {
                string[] requestTypes = new string[] {
                    "EntitySet",
                    "EntityInstance",
                    "CollectionProperty"
                };

                string[] metadataShapes = new string[] {
                    "CollectionOnEntity",
                    "CollectionOnComplexType"
                };

                string[] httpMethods = new string[] {
                    "GET",
                    "POST",
                    "PUT",
                    "PATCH",
                    "DELETE"
                };

                string[] responseFormats = new string[] {UnitTestsUtil.AtomFormat};

                TestUtil.RunCombinations(metadataShapes, requestTypes, httpMethods, responseFormats, (metadataShape, requestType, httpMethod, format) =>
                {
                    // POST, PATCH and DELETE can only be done on the entity instance itself
                    if ((httpMethod == "POST" || httpMethod == "DELETE" || httpMethod == "PATCH") && (requestType != "EntityInstance")) return;
                    // PUT can be done on entity instance or the collection property, but not on the entity set
                    if ((httpMethod == "PUT") && (requestType != "EntityInstance") && (requestType != "CollectionProperty")) return;

                    DSPMetadata metadata = new DSPMetadata("Test", "TestNamespace");
                    var entityType = metadata.AddEntityType("EntityType", typeof(DSPResourceWithVersionProperty), null, false);
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    metadata.AddPrimitiveProperty(entityType, "Version", typeof(int), etag: true);
                    var addressType = metadata.AddComplexType("Address", typeof(DSPResourceWithVersionProperty), null, false);
                    if (metadataShape == "CollectionOnComplexType")
                    {
                        metadata.AddCollectionProperty(addressType, "Collection", typeof(int));
                        metadata.AddComplexProperty(entityType, "Address", addressType);
                    }
                    else
                    {
                        metadata.AddCollectionProperty(entityType, "Collection", typeof(int));
                    }
                    metadata.AddResourceSet("EntityTypeSet", entityType);
                    SetReadOnlyAndCheckCollectionPropagation(metadata);

                    DSPServiceDefinition service = new DSPServiceDefinition()
                    {
                        Metadata = metadata,
                        CreateDataSource = (m) =>
                        {
                            DSPContext context = new DSPContext();
                            var entities = context.GetResourceSetEntities("EntityTypeSet");
                            var entity = new DSPResourceWithVersionProperty(entityType);
                            entity.SetRawValue("ID", 0);
                            if (metadataShape == "CollectionOnComplexType")
                            {
                                var address = new DSPResourceWithVersionProperty(addressType, entity);
                                address.SetRawValue("Collection", new List<int>() { 1, 2, 3 });
                                entity.SetRawValue("Address", address);
                            }
                            else
                            {
                                entity.SetRawValue("Collection", new List<int>() { 1, 2, 3 });
                            }
                            entities.Add(entity);
                            return context;
                        },
                        Writable = true
                    };
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        string etag = null;
                        XDocument payload = null;
                        string requestUri = null;
                        switch (requestType)
                        {
                            case "EntitySet":
                                {
                                    requestUri = "/EntityTypeSet";
                                    payload = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri, format);
                                    etag = (string)payload.Root.Element(UnitTestsUtil.AtomNamespace + "entry").Attribute(UnitTestsUtil.MetadataNamespace + "etag");
                                    break;
                                }
                            case "EntityInstance":
                                {
                                    requestUri = "/EntityTypeSet(0)";
                                    payload = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri, format);
                                    etag = request.ResponseETag;
                                    break;
                                }
                            case "CollectionProperty":
                                {
                                    if (format == UnitTestsUtil.AtomFormat) format = UnitTestsUtil.MimeApplicationXml;
                                    requestUri = "/EntityTypeSet(0)";
                                    if (metadataShape == "CollectionOnComplexType") requestUri += "/Address";
                                    requestUri += "/Collection";
                                    payload = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri, format);
                                    etag = request.ResponseETag;
                                    break;
                                }
                        }
                        Assert.IsNotNull(etag, "ETag must be sent in the response.");
                        Assert.AreEqual(
                            service.CurrentDataSource.GetResourceSetEntities("EntityTypeSet").OfType<DSPResource>().First(dspResource => (int)dspResource.GetValue("ID") == 0).GetETag(),
                            etag,
                            "The ETag computed from the entity and the ETag value from the response do not match.");

                        if (httpMethod == "GET")
                        {
                            // Get has already been done above
                            return;
                        }
                        else if (httpMethod == "POST")
                        {
                            // Update the ID in the payload
                            payload.Root.Element(UnitTestsUtil.AtomNamespace + "content").Element(UnitTestsUtil.MetadataNamespace + "properties")
                                .Element(UnitTestsUtil.DataNamespace + "ID").Value = "1";

                            request.HttpMethod = "POST";
                            request.RequestContentType = format;
                            request.SetRequestStreamAsText(format == UnitTestsUtil.JsonLightMimeType ? UnitTestsUtil.Atom2JsonXLinq(payload) : payload.ToString());
                            request.RequestUriString = "/EntityTypeSet";
                            request.SendRequest();
                            Assert.IsNotNull(request.ResponseETag, "ETag of the POST response must not be empty.");
                            Assert.AreEqual(
                                service.CurrentDataSource.GetResourceSetEntities("EntityTypeSet").OfType<DSPResource>().First(dspResource => (int)dspResource.GetValue("ID") == 1).GetETag(),
                                request.ResponseETag,
                                "The ETag computed from the entity and the ETag value from the response do not match.");
                        }
                        else
                        {
                            // First try a wrong ETag (missing is too easy) and then try the right one
                            request.IfMatch = "W/\"-1\"";
                            request.RequestUriString = requestUri;
                            request.HttpMethod = httpMethod;
                            if (httpMethod != "DELETE")
                            {
                                request.RequestContentType = format;
                                request.SetRequestStreamAsText(format == UnitTestsUtil.JsonLightMimeType ? UnitTestsUtil.Atom2JsonXLinq(payload) : payload.ToString());
                            }
                            Exception e = TestUtil.RunCatching(request.SendRequest);
                            Assert.IsNotNull(e, "Request should have failed.");
                            Assert.AreEqual(412, ((DataServiceException)e.InnerException).StatusCode, "The request should have failed with 412.");

                            // And now try the right ETag - should succeeed
                            request.IfMatch = etag;
                            if (httpMethod != "DELETE")
                            {
                                request.RequestContentType = format;
                                request.SetRequestStreamAsText(format == UnitTestsUtil.JsonLightMimeType ? UnitTestsUtil.Atom2JsonXLinq(payload) : payload.ToString());
                            }
                            request.SendRequest();
                            if (httpMethod != "DELETE")
                            {
                                // And the response should have a new different etag
                                Assert.IsNotNull(request.ResponseETag, "Response must have ETag.");
                                Assert.AreNotEqual(etag, request.ResponseETag, "New ETag must differ from the old one.");
                                Assert.AreEqual(
                                    service.CurrentDataSource.GetResourceSetEntities("EntityTypeSet").OfType<DSPResource>().First(dspResource => (int)dspResource.GetValue("ID") == 0).GetETag(),
                                    request.ResponseETag,
                                    "The ETag computed from the entity and the ETag value from the response do not match.");
                            }
                        }
                    }
                });
            }
            #endregion

            #region Helpers
            /// <summary>Verifies the payload for a collection property.</summary>
            /// <param name="collectionPropertyElement">The element with the payload for the collection property.</param>
            /// <param name="collectionPropertyName">The name of the collection property.</param>
            /// <param name="itemTypeName">The full name of the type of a single item in the collection.</param>
            /// <param name="expectedValues">List of tuples, where Item1 is the expected value of the item and Item2 is the expected type of the item.</param>
            /// <param name="jsonSource">true if the XML came from JSON in which case not all of the type information is available in the payload.</param>
            private static void VerifyCollectionPropertyValue(
                XElement collectionPropertyElement,
                string collectionPropertyName,
                string itemTypeName,
                IEnumerable<Tuple<object, string>> expectedValues)
            {
                Assert.IsNotNull(collectionPropertyElement, "The collection property was not found in the payload.");
                Assert.IsTrue((UnitTestsUtil.MetadataNamespace + "value").Equals(collectionPropertyElement.Name) ||
                              (UnitTestsUtil.DataNamespace + collectionPropertyName).Equals(collectionPropertyElement.Name), "The name of the collection property element is wrong.");
                if (itemTypeName != null)
                {
                    Assert.AreEqual("#Collection(" + itemTypeName + ")", (string)collectionPropertyElement.Attribute(UnitTestsUtil.MetadataNamespace + "type"), "The type of the collection property is wrong.");
                }

                var expectedValuesList = expectedValues.ToList();
                int i = 0;
                foreach (var item in collectionPropertyElement.Elements(UnitTestsUtil.MetadataNamespace + "element"))
                {
                    Assert.IsTrue(expectedValuesList.Count > i, "Payload contains more items than expected.");
                    Tuple<object, string> expectedValue = expectedValuesList[i++];
                    VerifyPropertyValue(item, expectedValue.Item1);
                    Assert.AreEqual(expectedValue.Item2, (string)item.Attribute(UnitTestsUtil.MetadataNamespace + "type"), "The type of the item is wrong.");
                }

                Assert.AreEqual(i, expectedValuesList.Count, "Payload doesn't contain expected number of items.");
            }

            /// <summary>Verifies payload for a complex property (simple verification).</summary>
            /// <param name="complexPropertyElement">The payload of the complex property.</param>
            /// <param name="resource">The expected value of the resource.</param>
            private static void VerifyComplexPropertyValue(XElement complexPropertyElement, DSPResource resource)
            {
                foreach (var property in complexPropertyElement.Elements().Where(e => e.Name.Namespace == UnitTestsUtil.DataNamespace))
                {
                    VerifyPropertyValue(property, resource.GetValue(property.Name.LocalName));
                }
            }

            /// <summary>Verifies a value of a property (primitive or complex)</summary>
            /// <param name="propertyElement">The payload of the property.</param>
            /// <param name="expectedValue">The expected value of the property.</param>
            private static void VerifyPropertyValue(XElement propertyElement, object expectedValue)
            {
                if (expectedValue == null)
                {
                    Assert.AreEqual("true", (string)propertyElement.Attribute(UnitTestsUtil.MetadataNamespace + "null"), "Value of the property is null, but it was not serialized with m:null='true'.");
                    Assert.AreEqual("", propertyElement.Value, "The content of a null property element should be empty.");
                }
                else if (expectedValue is DSPResource)
                {
                    VerifyComplexPropertyValue(propertyElement, expectedValue as DSPResource);
                }
                else
                {
                    string expectedStringValue = expectedValue.ToString();
                    Assert.AreEqual(expectedStringValue, (string)propertyElement, "The value of the item is wrong.");
                    if (expectedStringValue.Length > 0 &&
                        (XmlConvert.IsWhitespaceChar(expectedStringValue[0]) ||
                        XmlConvert.IsWhitespaceChar(expectedStringValue[expectedStringValue.Length - 1])))
                    {
                        Assert.AreEqual("preserve", (string)propertyElement.Attribute(XNamespace.Xml + "space"), "Property with whitespaces should have xml:space='preserve'.");
                    }
                }
            }

            /// <summary>Returns the collection resource type specified by its name.</summary>
            /// <param name="metadata">The metadata provider to use to identify the type.</param>
            /// <param name="typeName">The name of the collection type to parse.</param>
            /// <returns>The collection resource type, or null if it can't be recognized.</returns>
            private static providers.CollectionResourceType GetCollectionResourceTypeFromName(DSPMetadata metadata, string typeName)
            {
                if (!typeName.StartsWith("Collection(") || !typeName.EndsWith(")")) return null;
                var itemTypeName = typeName.Substring("Collection(".Length, typeName.Length - 1 - "Collection(".Length);
                var itemType = metadata.GetResourceTypeFromTestSpecification(itemTypeName);
                if (itemType == null) return null;
                return providers.ResourceType.GetCollectionResourceType(itemType);
            }

            /// <summary>
            /// Sets the metadata provider to read-only, and checks that all collection properties are sealed as a result.
            /// </summary>
            /// <param name="metadata">The DSPMetadata provider to set to read-only.</param>
            private static void SetReadOnlyAndCheckCollectionPropagation(DSPMetadata metadata)
            {
                metadata.SetReadOnly();

                foreach (providers.ResourceProperty property in metadata.Types.SelectMany(type => type.Properties)
                    .Where(property => property.ResourceType.ResourceTypeKind == providers.ResourceTypeKind.Collection))
                {
                    providers.CollectionResourceType collectionType = property.ResourceType as providers.CollectionResourceType;
                    Debug.Assert(collectionType != null, "ResourceTypeKind is Collection but the instance is not of CollectionResourceType");

                    Assert.IsTrue(
                        collectionType.ItemType.IsReadOnly,
                        string.Format(
                            "The item resource type {0} belonging to the collection property {1} was not set to read-only after the property resource type was set to read-only.",
                            collectionType.ItemType.Name, collectionType.Name));
                }
            }

            /// <summary>Helper method to create a List.</summary>
            /// <typeparam name="T">The type of an item.</typeparam>
            /// <param name="items">The items to preload the list with.</param>
            /// <returns>The new list object.</returns>
            private static object CreateList<T>(IEnumerable<object> items)
            {
                return items.Cast<T>().ToList();
            }

            /// <summary>Helper method to create an array.</summary>
            /// <typeparam name="T">The type of an item.</typeparam>
            /// <param name="items">The items to preload the array with.</param>
            /// <returns>The new array object.</returns>
            private static object CreateArray<T>(IEnumerable<object> items)
            {
                return items.Cast<T>().ToArray();
            }

            /// <summary>Helper method to create a custom enumerable collection.</summary>
            /// <typeparam name="T">The type of an item.</typeparam>
            /// <param name="items">The items to preload the collection with.</param>
            /// <returns>The new collection object.</returns>
            private static object CreateCustomEnumerable<T>(IEnumerable<object> items)
            {
                return new CustomEnumerable<T>(items);
            }
            #endregion
        }
    }
}
