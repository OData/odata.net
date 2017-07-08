//---------------------------------------------------------------------
// <copyright file="WriterMetadataInputValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests to verify writer correctly validates input
    /// </summary>
    // [TestClass, TestCase]
    public class WriterMetadataInputValidationTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that entry type is correctly validated if metadata is specified.")]
        public void InvalidEntryTypeTest()
        {
            Action<EdmModel> entityType = (model) =>
            {
                EdmEntityType newEntityType = new EdmEntityType("TestNS", "EntityType");
                model.AddElement(newEntityType);
            };

            Action<EdmModel> complexType = (model) =>
            {
                EdmComplexType newComplexType = new EdmComplexType("TestNS", "ComplexType");
                model.AddElement(newComplexType);
            };

            var testCases = new[] {
                new {
                    TypeName = (string)null,
                    TypeCreate = entityType,
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata"),
                },
                new {
                    TypeName = "",
                    TypeCreate = entityType,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_TypeNameMustNotBeEmpty"),
                },
                new {
                    TypeName = "EntityType",
                    TypeCreate = entityType,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "EntityType"),
                },
                new {
                    TypeName = "TestNS.ComplexType",
                    TypeCreate = complexType,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestNS.ComplexType", "Entity", "Complex"),
                },
            };

            var testDescriptors = testCases.Select(tc =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = tc.TypeName;
                var descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    entry,
                    (testConfiguration) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = tc.ExpectedException });

                EdmModel model = new EdmModel();
                tc.TypeCreate(model);
                var container = new EdmEntityContainer("TestNS", "TestContainer");
                model.AddElement(container);

                descriptor.Model = model;
                return descriptor;
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that property value types are correctly validated if metadata is specified.")]
        public void InvalidPropertyValueTypeTest()
        {
            Func<EdmModel, EdmEntityType> entityTypeWithComplexProperty =
                (model) =>
                {
                    EdmComplexType addressComplexType = new EdmComplexType("TestNS", "AddressComplexType");
                    addressComplexType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
                    model.AddElement(addressComplexType);

                    EdmEntityType entityType = new EdmEntityType("TestNS", "EntityType");
                    entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                    entityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressComplexType, false));

                    model.AddElement(entityType);

                    return entityType;
                };

            Func<EdmModel, EdmEntityType> entityTypeWithCollectionProperty =
                (model) =>
                {
                    EdmEntityType entityType = new EdmEntityType("TestNS", "EntityType");
                    entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                    entityType.AddStructuralProperty("Address", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));

                    model.AddElement(entityType);

                    return entityType;
                };

            Func<EdmModel, EdmEntityType> openEntityType =
                (model) =>
                {
                    EdmEntityType entityType = new EdmEntityType("TestNS", "EntityType", null, false, true);
                    entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));

                    model.AddElement(entityType);

                    return entityType;
                };

            var testCases = new[] {
                new { // empty type name
                    PropertyCreate = (Func<ODataProperty>)(() => new ODataProperty() { Name = "Address", Value = new ODataComplexValue() { TypeName = string.Empty } }),
                    MetadataCreate = entityTypeWithComplexProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_TypeNameMustNotBeEmpty"),
                    ResponseOnly = false,
                },
                new { // Invalid collection type name
                    PropertyCreate = (Func<ODataProperty>)(() => new ODataProperty() { Name = "Address", Value = new ODataCollectionValue() { TypeName = "TestNS.AddressComplexType" } }),
                    MetadataCreate = entityTypeWithComplexProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestNS.AddressComplexType", "Collection", "Complex"),
                    ResponseOnly = false,
                },
                new { // Invalid complex type name
                    PropertyCreate = (Func<ODataProperty>)(() => new ODataProperty() { Name = "Address", Value = new ODataComplexValue() { TypeName = EntityModelUtils.GetCollectionTypeName("String") } }),
                    MetadataCreate = entityTypeWithCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", EntityModelUtils.GetCollectionTypeName("Edm.String"), "Complex", "Collection"),
                    ResponseOnly = false,
                },
                new { // open property with complex value but without type; error.
                    PropertyCreate = (Func<ODataProperty>)(() => new ODataProperty() { Name = "Address", Value = new ODataComplexValue() { Properties = null } }),
                    MetadataCreate = openEntityType,
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata"),
                    ResponseOnly = false,
                },
                new { // open property with collection but without a type; error.
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Address",
                            Value = new ODataCollectionValue()
                            {
                                Items = null
                            }
                        }),
                    MetadataCreate = openEntityType,
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata"),
                    ResponseOnly = false,
                },
                new { // open property with stream value
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Address",
                            Value = new ODataStreamReferenceValue()
                            {
                                ReadLink = new Uri("http://odata.org/readlink"),
                                EditLink = new Uri("http://odata.org/editlink"),
                                ContentType = "plain/text"
                            }
                        }),
                    MetadataCreate = openEntityType,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_OpenStreamProperty", "Address"),
                    ResponseOnly = true,
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testConfiguration.IsRequest && testCase.ResponseOnly)
                    {
                        return;
                    }

                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.TypeName = "TestNS.EntityType";
                    ODataProperty idProperty = new ODataProperty() { Name = "Id", Value = "1" };
                    entry.Properties = new ODataProperty[] { idProperty, testCase.PropertyCreate() };
                    var descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        entry,
                        (tc) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = testCase.ExpectedException
                        });
                    var model = new EdmModel();
                    testCase.MetadataCreate(model);
                    var container = new EdmEntityContainer("TestNS", "TestContainer");
                    model.AddElement(container);

                    descriptor.Model = model;
                    TestWriterUtils.WriteAndVerifyODataPayload(descriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that item types in collections match the type specified for the collection.")]
        public void InvalidCollectionItemTypeTest()
        {
            Func<EdmModel, EdmEntityType> entityTypeWithComplexCollectionProperty =
               (model) =>
               {
                   EdmComplexType addressType = new EdmComplexType("OtherTestNamespace", "AddressComplexType");
                   addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
                   model.AddElement(addressType);

                   EdmComplexType otherComplexType = new EdmComplexType("OtherTestNamespace", "OtherComplexType");
                   model.AddElement(otherComplexType);

                   EdmEntityType type = new EdmEntityType("TestNS", "EntityType");
                   type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                   type.AddStructuralProperty("Addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressType, false)));
                   model.AddElement(type);

                   return type;
               };

            Func<EdmModel, EdmEntityType> entityTypeWithPrimitiveCollectionProperty =
                (model) =>
                {
                    EdmComplexType addressType = new EdmComplexType("OtherTestNamespace", "AddressComplexType");
                    model.AddElement(addressType);

                    EdmComplexType otherComplexType = new EdmComplexType("OtherTestNamespace", "OtherComplexType");
                    model.AddElement(otherComplexType);

                    EdmEntityType type = new EdmEntityType("TestNS", "EntityType");
                    type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                    type.AddStructuralProperty("Addresses", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
                    model.AddElement(type);

                    return type;
                };

            var testCases = new[] {
                // Primitive item in a complex collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.AddressComplexType"),
                                Items = new string[] { "One Redmond Way" }
                            }
                        }),
                    MetadataCreate = entityTypeWithComplexCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                },
                // Complex item in a primitive collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                                Items = new ODataComplexValue[]
                                {
                                    new ODataComplexValue() { TypeName = "OtherTestNamespace.AddressComplexType" }
                                }
                            }
                        }),
                    MetadataCreate = entityTypeWithPrimitiveCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },
                // Type name of the collection doesn't match the metadata - complex collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.OtherComplexType"),
                                Items = new ODataComplexValue[0]
                            }
                        }),
                    MetadataCreate = entityTypeWithComplexCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.OtherComplexType"), EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.AddressComplexType")),
                },
                // Type name of the collection doesn't match the metadata - primitive collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("Int32"),
                                Items = new object[0]
                            }
                        }),
                    MetadataCreate = entityTypeWithPrimitiveCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", EntityModelUtils.GetCollectionTypeName("Edm.Int32"), EntityModelUtils.GetCollectionTypeName("Edm.String")),
                },
                // Item of a different primitive type in a complex collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                                Items = new object[] { 1 }
                            }
                        }),
                    MetadataCreate = entityTypeWithPrimitiveCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                },
                // Item of a different complex type in a complex collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.AddressComplexType"),
                                Items = new ODataComplexValue[]
                                {
                                    new ODataComplexValue() { TypeName = "OtherTestNamespace.OtherComplexType" }
                                }
                            }
                        }),
                    MetadataCreate = entityTypeWithComplexCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "OtherTestNamespace.OtherComplexType", "OtherTestNamespace.AddressComplexType"),
                },
                // Bogus item for a primitive collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                                Items = new object[] { new ODataMessageWriterSettings() }
                            }
                        }),
                    MetadataCreate = entityTypeWithPrimitiveCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnsupportedPrimitiveType", "Microsoft.OData.ODataMessageWriterSettings"),
                },
                // Bogus item for a complex collection
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.AddressComplexType"),
                                Items = new object[] { new ODataMessageWriterSettings() }
                            }
                        }),
                    MetadataCreate = entityTypeWithComplexCollectionProperty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnsupportedPrimitiveType", "Microsoft.OData.ODataMessageWriterSettings"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.TypeName = "TestNS.EntityType";
                    ODataProperty idProperty = new ODataProperty() { Name = "Id", Value = "1" };
                    entry.Properties = new ODataProperty[] { idProperty, testCase.PropertyCreate() };
                    var testDescriptor = new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        entry,
                        (tc) =>
                            new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException2 = testCase.ExpectedException
                            });
                    var model = new EdmModel();
                    testCase.MetadataCreate(model);
                    var container = new EdmEntityContainer("TestNS", "TestContainer");
                    model.AddElement(container);
                    testDescriptor.Model = model;

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that collection properties must not have null values.")]
        public void NullCollectionTest()
        {
            Func<EdmModel, EdmEntityType> entityTypeWithComplexCollectionProperty =
                (model) =>
                {
                    EdmComplexType addressType = new EdmComplexType("TestNS", "AddressComplexType");
                    addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
                    model.AddElement(addressType);

                    EdmComplexType otherComplexType = new EdmComplexType("TestNS", "OtherComplexType");
                    model.AddElement(otherComplexType);

                    EdmEntityType type = new EdmEntityType("TestNS", "EntityType");
                    type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                    type.AddStructuralProperty("Addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressType, false)));
                    model.AddElement(type);

                    return type;
                };

            Func<EdmModel, EdmEntityType> entityTypeWithPrimitiveCollectionProperty =
                (model) =>
                {
                    EdmComplexType addressType = new EdmComplexType("TestNS", "AddressComplexType");
                    EdmComplexType otherComplexType = new EdmComplexType("TestNS", "OtherComplexType");
                    EdmEntityType type = new EdmEntityType("TestNS", "EntityType");
                    type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                    type.AddStructuralProperty("Addresses", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
                    model.AddElement(type);

                    return type;
                };

            ExpectedException expectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue", "Addresses");

            var testCases = new[] {
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = null
                        }),
                    MetadataCreate = entityTypeWithComplexCollectionProperty,
                    ExpectedException = expectedException
                },
                new {
                    PropertyCreate = (Func<ODataProperty>)(
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = null
                        }),
                    MetadataCreate = entityTypeWithPrimitiveCollectionProperty,
                    ExpectedException = expectedException
                },
            };

            var testDescriptors = testCases.Select(tc =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = "TestNS.EntityType";
                ODataProperty idProperty = new ODataProperty() { Name = "Id", Value = "1" };
                entry.Properties = new ODataProperty[] { idProperty, tc.PropertyCreate() };
                var descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    entry,
                    (testConfiguration) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = tc.ExpectedException });
                var model = new EdmModel();
                tc.MetadataCreate(model);
                var container = new EdmEntityContainer("TestNS", "TestContainer");
                model.AddElement(container);
                descriptor.Model = model;
                return descriptor;
            });

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that all properties on (closed) types have to be declared.")]
        public void MissingPropertyTest()
        {
            Func<EdmModel, EdmEntityType> entityType =
                (model) =>
                {
                    EdmComplexType addressType = new EdmComplexType("TestNS", "AddressComplexType");
                    model.AddElement(addressType);

                    EdmEntityType type = new EdmEntityType("TestNS", "EntityType");
                    type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                    type.AddStructuralProperty("Addresses", new EdmComplexTypeReference(addressType, false));
                    model.AddElement(type);

                    return type;
                };

            Func<EdmModel, EdmEntityType> openEntityType =
                (model) =>
                {
                    EdmEntityType type = new EdmEntityType("TestNS", "EntityType", null, false, true);
                    type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                    model.AddElement(type);

                    return type;
                };

            var testCases = new[]
            {
                new
                {
                    PropertyCreate = (Func<ODataProperty>) (
                        () => new ODataProperty()
                        {
                            Name = "NonExistentProperty",
                            Value = null
                        }),
                    MetadataCreate = entityType,
                    ExpectedResults = (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback) (
                        (testConfiguration) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 =
                                ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType",
                                    "NonExistentProperty", "TestNS.EntityType"),
                        })
                },
                new
                {
                    PropertyCreate = (Func<ODataProperty>) (
                        () => new ODataProperty()
                        {
                            Name = "Addresses",
                            Value = new ODataComplexValue()
                            {
                                TypeName = "TestNS.AddressComplexType",
                                Properties = new ODataProperty[]
                                {
                                    new ODataProperty() {Name = "Street", Value = "One Microsoft Way"}
                                }

                            }
                        }),
                    MetadataCreate = entityType,
                    ExpectedResults = (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback) (
                        (testConfiguration) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 =
                                ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType",
                                    "Street", "TestNS.AddressComplexType"),
                        })
                },
                new
                {
                    PropertyCreate = (Func<ODataProperty>) (
                        () => new ODataProperty()
                        {
                            Name = "FirstName",
                            Value = "Bill"
                        }),
                    MetadataCreate = openEntityType,
                    ExpectedResults =
                        (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback)
                            ((testConfiguration) =>
                            {

                                string jsonResult = "\"FirstName\":\"Bill\"";

                                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    Json = jsonResult,
                                    FragmentExtractor =
                                        (element) =>
                                            JsonUtils.UnwrapTopLevelValue(testConfiguration, element)
                                                .Object()
                                                .Properties.Where(p => p.Name == "FirstName")
                                                .Single()
                                };

                            })
                },
            };

            var testDescriptors = testCases.Select(tc =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = "TestNS.EntityType";

                ODataProperty idProperty = new ODataProperty() { Name = "Id", Value = "1" };
                entry.Properties = new ODataProperty[] { idProperty, tc.PropertyCreate() };

                var descriptor = new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, tc.ExpectedResults);
                var model = new EdmModel();
                tc.MetadataCreate(model);
                var container = new EdmEntityContainer("TestNS", "TestContainer");
                model.AddElement(container);
                descriptor.Model = model;
                return descriptor;
            });

            //TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private static IEnumerable<ODataStreamReferenceValue> NonMLEDefaultStreamValues = new[]
        {
            // Just null is a valid non-MLE default stream value
            (ODataStreamReferenceValue)null,
        };

        private static IEnumerable<ODataStreamReferenceValue> MLEDefaultStreamValues = new[]
        {
            // No properties set, but the default MR is present
            new ODataStreamReferenceValue(),
            // Read link and content type
            new ODataStreamReferenceValue() { ReadLink = new Uri("http://odata.org/ReadLink"), ContentType = "media/type" },
            // Edit link and ETag
            new ODataStreamReferenceValue() { EditLink = new Uri("http://odata.org/EditLink"), ETag = "W/\"myetag\"" },
            // Read link, content type, Edit link and ETag
            new ODataStreamReferenceValue() { ReadLink = new Uri("http://odata.org/ReadLink"), ContentType = "media/type", EditLink = new Uri("http://odata.org/EditLink"), ETag = "W/\"myetag\"" },
        };

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that default stream is validated against metadata.")]
        public void DefaultStreamMetadataTest()
        {
            EdmModel model = new EdmModel();
            var cityType = new EdmEntityType("TestModel", "CityType");
            model.AddElement(cityType);
            var cityWithMapType = new EdmEntityType("TestModel", "CityWithMapType", null, false, false, true);
            model.AddElement(cityWithMapType);

            var container = new EdmEntityContainer("TestModel", "TestContainer");
            container.AddEntitySet("CityType", cityType);
            container.AddEntitySet("CityWithMapType", cityWithMapType);
            model.AddElement(container);

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors =
                // non-MLE payload and non-MLE type
                NonMLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityType.FullName(), mr))
                // non-MLE payload and MLE type
                .Concat(NonMLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityWithMapType.FullName(), mr,
                    ODataExpectedExceptions.ODataException("ValidationUtils_ResourceWithoutMediaResourceAndMLEType", "TestModel.CityWithMapType"))))
                // MLE payload and non-MLE type
                .Concat(MLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityType.FullName(), mr,
                    ODataExpectedExceptions.ODataException("ValidationUtils_ResourceWithMediaResourceAndNonMLEType", "TestModel.CityType"))))
                // MLE payload and MLE type
                .Concat(MLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityWithMapType.FullName(), mr)));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that default stream is validated against metadata in WCF DS Server mode.")]
        public void DefaultStreamMetadataWcfDSServerTest()
        {

            EdmModel model = new EdmModel();
            var cityType = new EdmEntityType("TestModel", "CityType");
            model.AddElement(cityType);
            var cityWithMapType = new EdmEntityType("TestModel", "CityWithMapType", null, false, false, true);
            model.AddElement(cityWithMapType);

            var container = new EdmEntityContainer("TestModel", "TestContainer");
            container.AddEntitySet("CityType", cityType);
            container.AddEntitySet("CityWithMapType", cityWithMapType);
            model.AddElement(container);

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors =
                // non-MLE payload and non-MLE type
                NonMLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityType.FullName(), mr))
                // non-MLE payload and MLE type
                .Concat(NonMLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityWithMapType.FullName(), mr,
                    ODataExpectedExceptions.ODataException("ValidationUtils_ResourceWithoutMediaResourceAndMLEType", "TestModel.CityWithMapType"))))
                // MLE payload and non-MLE type
                .Concat(MLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityType.FullName(), mr,
                    ODataExpectedExceptions.ODataException("ValidationUtils_ResourceWithMediaResourceAndNonMLEType", "TestModel.CityType"))))
                // MLE payload and MLE type
                .Concat(MLEDefaultStreamValues.Select(mr => this.CreateDefaultStreamMetadataTestDescriptor(model, cityWithMapType.FullName(), mr)));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.Validations &= ~ValidationKinds.ThrowOnDuplicatePropertyNames;
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private PayloadWriterTestDescriptor<ODataItem> CreateDefaultStreamMetadataTestDescriptor(
            IEdmModel model,
            string typeName,
            ODataStreamReferenceValue mediaResourceValue,
            ExpectedException expectedException = null)
        {
            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                new ODataResource()
                {
                    TypeName = typeName,
                    MediaResource = mediaResourceValue,
                    SerializationInfo = new ODataResourceSerializationInfo()
                    {
                        NavigationSourceEntityTypeName = typeName,
                        NavigationSourceName = "MySet",
                        ExpectedTypeName = typeName
                    }
                },
                (tc) =>
                {
                    if (expectedException != null)
                    {
                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = expectedException };
                    }
                    else
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { FragmentExtractor = (result) => new JsonObject(), Json = "{}" };
                    }
                })
            {
                Model = model
            };
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that we do not allow inconsistent type information in the metadata and on the values.")]
        public void InconsistentTypeNamesTest()
        {
            Func<EdmModel, EdmEntityType> entityType =
               (model) =>
               {
                   EdmComplexType addressComplexType = new EdmComplexType("TestNS", "AddressComplexType");
                   addressComplexType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
                   model.AddElement(addressComplexType);

                   EdmComplexType orderComplexType = new EdmComplexType("OtherTestNamespace", "OrderComplexType");
                   model.AddElement(orderComplexType);

                   EdmEntityType type = new EdmEntityType("TestNS", "EntityType");
                   type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(true)));
                   type.AddStructuralProperty("Address", new EdmComplexTypeReference(addressComplexType, false));
                   type.AddStructuralProperty("Scores", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
                   type.AddStructuralProperty("Addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressComplexType, false)));
                   model.AddElement(type);

                   return type;
               };

            ODataProperty[] defaultProperties = new ODataProperty[]
            {
                new ODataProperty() { Name = "Id", Value = "1" },
                new ODataProperty()
                {
                    Name = "Address",
                    Value = new ODataComplexValue()
                    {
                        TypeName = "TestNS.AddressComplexType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "Street", Value = "First" }
                        }
                    }
                },
                new ODataProperty()
                {
                    Name = "Scores",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                        Items = null
                    }
                },
                new ODataProperty()
                {
                    Name = "Addresses",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("TestNS.AddressComplexType"),
                        Items = new object[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = "TestNS.AddressComplexType",
                                Properties = null
                            }
                        }
                    }
                },
                new ODataProperty()
                {
                    Name = "Addresses",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("TestNS.AddressComplexType"),
                        Items = new object[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = null,
                                Properties = null
                            }
                        }
                    }
                }
            };

            var testCases = new[] {
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            // inconsistent primitive type
                            new ODataProperty() { Name = "Id", Value = 1 },
                            defaultProperties[1],
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Int32", "False" /* nullable */, "Edm.String", "True" /* nullable */),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            // inconsistent primitive type
                            new ODataProperty() { Name = "Id", Value = new ODataComplexValue { } },
                            defaultProperties[1],
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKindNoTypeName", "Complex", "Primitive"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            // inconsistent primitive type
                            new ODataProperty() { Name = "Id", Value = new ODataCollectionValue { } },
                            defaultProperties[1],
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKindNoTypeName", "Collection", "Primitive"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            // inconsistent primitive type
                            new ODataProperty() { Name = "Id", Value = new ODataStreamReferenceValue { } },
                            defaultProperties[1],
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_MismatchPropertyKindForStreamProperty", "Id"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            // inconsistent complex type (same kind)
                            new ODataProperty()
                            {
                                Name = "Address",
                                Value = new ODataComplexValue()
                                {
                                    TypeName = "OtherTestNamespace.OrderComplexType",
                                    Properties = null
                                }
                            },
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", "OtherTestNamespace.OrderComplexType", "TestNS.AddressComplexType"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            // inconsistent complex type (different kind)
                            new ODataProperty()
                            {
                                Name = "Address",
                                Value = "some"
                            },
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_NonPrimitiveTypeForPrimitiveValue", "TestNS.AddressComplexType"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            // inconsistent complex type (different kind)
                            new ODataProperty()
                            {
                                Name = "Address",
                                Value = new ODataCollectionValue { }
                            },
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKindNoTypeName", "Collection", "Complex"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            // inconsistent complex type (different kind)
                            new ODataProperty()
                            {
                                Name = "Address",
                                Value = new ODataStreamReferenceValue { }
                            },
                            defaultProperties[2],
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_MismatchPropertyKindForStreamProperty", "Address"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            defaultProperties[1],
                            // inconsistent collection type
                            new ODataProperty()
                            {
                                Name = "Scores",
                                Value = new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("Int32"),
                                    Items = null
                                }
                            },
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", EntityModelUtils.GetCollectionTypeName("Edm.Int32"), EntityModelUtils.GetCollectionTypeName("Edm.String")),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            defaultProperties[1],
                            new ODataProperty()
                            {
                                Name = "Scores",
                                Value = new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                                    // inconsistent collection item type (same kind)
                                    Items = new object[] { 1, 2, 3 }
                                }
                            },
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            defaultProperties[1],
                            new ODataProperty()
                            {
                                Name = "Scores",
                                Value = new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                                    // inconsistent collection item type (different kind)
                                    Items = new object[]
                                    {
                                        new ODataComplexValue()
                                        {
                                            TypeName = "OtherTestNamespace.OrderComplexType",
                                            Properties = new ODataProperty[]
                                            {
                                                new ODataProperty { Name = "Street", Value = "First" }
                                            }
                                        }
                                    }
                                }
                            },
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            defaultProperties[1],
                            new ODataProperty()
                            {
                                Name = "Scores",
                                Value = new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                                    // inconsistent collection item type (different kind)
                                    Items = new object[] { new ODataStreamReferenceValue {} }
                                }
                            },
                            defaultProperties[3]
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_StreamReferenceValuesNotSupportedInCollections", EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.OrderComplexType"), EntityModelUtils.GetCollectionTypeName("TestNS.AddressComplexType")),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            defaultProperties[1],
                            defaultProperties[2],
                            // inconsistent collection type
                            new ODataProperty()
                            {
                                Name = "Addresses",
                                Value = new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.OrderComplexType"),
                                    Items = null
                                }
                            },
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", EntityModelUtils.GetCollectionTypeName("OtherTestNamespace.OrderComplexType"), EntityModelUtils.GetCollectionTypeName("TestNS.AddressComplexType")),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            defaultProperties[1],
                            defaultProperties[2],
                            new ODataProperty()
                            {
                                Name = "Addresses",
                                Value = new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("TestNS.AddressComplexType"),
                                    Items = new object[]
                                    {
                                        new ODataComplexValue()
                                        {
                                            // inconsistent item type (same kind)
                                            TypeName = "OtherTestNamespace.OrderComplexType",
                                            Properties = new ODataProperty[]
                                            {
                                                new ODataProperty { Name = "Street", Value = "First" }
                                            }
                                        }
                                    }
                                }
                            },
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "OtherTestNamespace.OrderComplexType", "TestNS.AddressComplexType"),
                },
                new InconsistentTypeNamesTestCase {
                    CreateProperties = () => new ODataProperty[]
                        {
                            defaultProperties[0],
                            defaultProperties[1],
                            defaultProperties[2],
                            new ODataProperty()
                            {
                                Name = "Addresses",
                                Value = new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("TestNS.AddressComplexType"),
                                    // inconsistent item type (different kind)
                                    Items = new object[] { "foo" }
                                }
                            },
                        },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                },
            };

            var testDescriptors = testCases.Select(tc =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = "TestNS.EntityType";

                ODataProperty idProperty = new ODataProperty() { Name = "Id", Value = "1" };
                entry.Properties = tc.CreateProperties();

                var descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    entry,
                    (testConfiguration) =>
                    {
                        ExpectedException expectedException = tc.ExpectedException;

                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = expectedException
                        };
                    });
                EdmModel model = new EdmModel();
                entityType(model);
                var container = new EdmEntityContainer("TestNS", "TestContainer");
                model.AddElement(container);

                descriptor.Model = model;
                return descriptor;
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private sealed class InconsistentTypeNamesTestCase
        {
            public Func<ODataProperty[]> CreateProperties { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public ExpectedException EpmExpectedException { get; set; }
            public bool SkipEpm { get; set; }
        }
    }
}
