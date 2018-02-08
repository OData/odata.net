//---------------------------------------------------------------------
// <copyright file="CollectionWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing collections with the ODataMessageWriter.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    // [TestClass, TestCase]
    public class CollectionWriterTests : ODataWriterTestCase
    {
        //// NOTE
        //// {0} ... collection name
        //// {1} ... data service namespace
        //// {2} ... default metadata namespace prefix
        //// {3} ... collection item element name 'element'
        //// {4} ... atom 'type' attribute name
        //// {5} ... data service metadata namespace

        private static readonly CollectionWriterTestDescriptor.ItemDescription nullItem =
            new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = (object)null,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = new string[] { "null" },
            };

        private static readonly Func<int, CollectionWriterTestDescriptor.ItemDescription> intItem =
            i => new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = (object)i,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = new string[] { i.ToString() },
            };

        private static readonly CollectionWriterTestDescriptor.ItemDescription intTwoItem =
            new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = (object)2,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = new string[] { "2" },
            };

        private static readonly Func<string, CollectionWriterTestDescriptor.ItemDescription> stringItem =
            s => new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = (object)s,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = new string[] { "\"" + s.ToString() + "\"" },
            };

        private static readonly Func<string, CollectionWriterTestDescriptor.ItemDescription> complexItem =
            (typeName) => new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = (object)new ODataComplexValue()
                {
                    TypeName = typeName,
                    Properties = new ODataProperty[]
                        {
                            new ODataProperty() { Name = "Street", Value = "One Microsoft Way" },
                            new ODataProperty() { Name = "City", Value = "Redmond " },
                            new ODataProperty() { Name = "Zip", Value = 98052 },
                        }
                },
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines =
                    new string[]
                    {
                        "{",
                        "$(Indent)\"Street\":\"One Microsoft Way\",\"City\":\"Redmond \",\"Zip\":98052",
                        "}"
                    }
            };

        [InjectDependency(IsRequired = true)]
        public CollectionWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test error conditions when writing collections.")]
        public void CollectionErrorTest()
        {
            var testCases = new[]
                {
                    new
                    {
                        CollectionName = (string)null,
                        PayloadItems = new object[]
                        {
                            new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("String"),
                                Items = new string[] { "One", "Two", "Three" }
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_NestedCollectionsAreNotSupported"),
                    },
                    new
                    {
                        CollectionName = (string)null,
                        PayloadItems = new object[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = "TestNS.UnknownType",
                                Properties = null
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "TestNS.UnknownType"),
                    },
                    new
                    {
                        CollectionName = (string)null,
                        PayloadItems = new object[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = "TestNS.AddressType",
                                Properties = new ODataProperty[]
                                {
                                    new ODataProperty() { Name = "Street", Value = "One Microsoft Way" },
                                    new ODataProperty() { Name = "City", Value = "Redmond" },
                                    new ODataProperty() { Name = "Zip", Value = "98052" },
                                }
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.String", "True", "Edm.Int32", "False"),
                    },
                    new
                    {
                        CollectionName = (string)null,
                        PayloadItems = new object[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = "TestNS.AddressType",
                                Properties = new ODataProperty[]
                                {
                                    new ODataProperty() { Name = "Street2", Value = "One Microsoft Way" },
                                    new ODataProperty() { Name = "City", Value = "Redmond" },
                                    new ODataProperty() { Name = "Zip", Value = 98052 },
                                }
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "Street2", "TestNS.AddressType"),
                    },
                    new
                    {
                        CollectionName = (string)null,
                        PayloadItems = new object[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = "TestNS.AddressType",
                                Properties = new ODataProperty[]
                                {
                                    new ODataProperty() { Name = "Street", Value = "One Microsoft Way" },
                                    new ODataProperty() { Name = "Street2", Value = "Two Microsoft Way" },
                                    new ODataProperty() { Name = "City", Value = "Redmond" },
                                    new ODataProperty() { Name = "Zip", Value = 98052 },
                                }
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "Street2", "TestNS.AddressType"),
                    },
                    new
                    {
                        CollectionName = (string)null,
                        PayloadItems = new object[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = "OtherTestNamespace.AddressType",
                                Properties = new ODataProperty[]
                                {
                                    new ODataProperty() { Name = "Street", Value = "One Microsoft Way" },
                                }
                            }
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "Street", "OtherTestNamespace.AddressType"),
                    },
                };

            IEdmStringTypeReference stringTypeRef = EdmCoreModel.Instance.GetString(isNullable: true);
            var model = new EdmModel();
            var addressType = new EdmComplexType("TestNS", "AddressType");
            addressType.AddStructuralProperty("Street", stringTypeRef);
            addressType.AddStructuralProperty("City", stringTypeRef);
            addressType.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(addressType);

            var container = new EdmEntityContainer("TestNS", "TestContainer");
            model.AddElement(container);

            addressType = new EdmComplexType("OtherTestNamespace", "AddressType");
            model.AddElement(addressType);

            var testDescriptors = testCases.Select(tc =>
                new CollectionWriterTestDescriptor(
                    this.Settings,
                    tc.CollectionName,
                    tc.PayloadItems,
                    tc.ExpectedException,
                    model));

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testDescriptor, testConfig) =>
                {
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });

        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test different combinations of collection writing.")]
        public void CollectionPayloadTest()
        {
            IEdmPrimitiveTypeReference int32NullableTypeRef = EdmCoreModel.Instance.GetInt32(isNullable: true);
            IEdmStringTypeReference stringTypeRef = EdmCoreModel.Instance.GetString(isNullable: true);
            var model = new EdmModel();
            var addressType = new EdmComplexType("TestNS", "AddressType");
            addressType.AddStructuralProperty("Street", stringTypeRef);
            addressType.AddStructuralProperty("City", stringTypeRef);
            addressType.AddStructuralProperty("Zip", EdmPrimitiveTypeKind.Int32);
            model.AddElement(addressType);

            var container = new EdmEntityContainer("TestNS", "DefaultContainer");

            var intServiceOp = new EdmFunction("TestNS", "GetIntCollection", EdmCoreModel.GetCollection(int32NullableTypeRef));
            intServiceOp.AddParameter("a", int32NullableTypeRef);
            intServiceOp.AddParameter("b", stringTypeRef);
            var intServiceOpFunctionImport = container.AddFunctionImport("GetIntCollection", intServiceOp);

            EdmFunction stringServiceOp = new EdmFunction("TestNS", "GetStringCollection", EdmCoreModel.GetCollection(stringTypeRef));
            stringServiceOp.AddParameter("a", int32NullableTypeRef);
            stringServiceOp.AddParameter("b", stringTypeRef);
            var stringServiceOpFunctionImport = container.AddFunctionImport("GetStringCollection", stringServiceOp);

            EdmFunction complexServiceOp = new EdmFunction("TestNS", "GetComplexCollection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressType, true)));
            complexServiceOp.AddParameter("a", int32NullableTypeRef);
            complexServiceOp.AddParameter("b", stringTypeRef);
            var complexServiceOpFunctionImport = container.AddFunctionImport("GetComplexCollection", complexServiceOp);

            EdmFunction geometryServiceOp = new EdmFunction("TestNS", "GetGeometryCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, true)));
            geometryServiceOp.AddParameter("a", int32NullableTypeRef);
            geometryServiceOp.AddParameter("b", stringTypeRef);
            var geometryServiceOpFunctionImport = container.AddFunctionImport("GetGeometryCollection", geometryServiceOp);

            EdmFunction geographyServiceOp = new EdmFunction("TestNS", "GetGeographyCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true)));
            geographyServiceOp.AddParameter("a", int32NullableTypeRef);
            geographyServiceOp.AddParameter("b", stringTypeRef);
            var geographyServiceOpFunctionImport = container.AddFunctionImport("GetGeographyCollection", geographyServiceOp);

            model.AddElement(container);
            model.AddElement(intServiceOp);
            model.AddElement(stringServiceOp);
            model.AddElement(complexServiceOp);
            model.AddElement(geometryServiceOp);
            model.AddElement(geographyServiceOp);

            IEnumerable<CollectionWriterTestCase> testCases = new[]
            {
                new CollectionWriterTestCase(new CollectionWriterTestDescriptor.ItemDescription[]
                         {
                             nullItem,
                             intItem(1),
                             intItem(2),
                         }, intServiceOpFunctionImport),

                new CollectionWriterTestCase(new CollectionWriterTestDescriptor.ItemDescription[]
                         {
                             nullItem,
                             stringItem("One"),
                             stringItem("Two"),
                         }, stringServiceOpFunctionImport),

                new CollectionWriterTestCase(new CollectionWriterTestDescriptor.ItemDescription[]
                         {
                             nullItem,
                             complexItem("TestNS.AddressType"),
                             complexItem("TestNS.AddressType"),
                         }, complexServiceOpFunctionImport),

                new CollectionWriterTestCase(new CollectionWriterTestDescriptor.ItemDescription[]
                         {
                             nullItem,
                             GetGeometryPointItem("GeometryPoint"),
                             GetGeometryPolygonItem("GeometryPolygon"),
                             GetGeometryCollectionItem("GeometryCollection"),
                             GetGeometryMultiLineStringItem("GeometryMultiLineString"),
                         }, geometryServiceOpFunctionImport),

                new CollectionWriterTestCase(new CollectionWriterTestDescriptor.ItemDescription[]
                         {
                             nullItem,
                             GetGeographyPointItem("GeographyPoint"),
                             GetGeographyPolygonItem("GeographyPolygon"),
                             GetGeographyCollectionItem("GeographyCollection"),
                             GetGeographyMultiLineStringItem("GeographyMultiLineString"),
                         }, geographyServiceOpFunctionImport),
            };

            IEnumerable<CollectionWriterTestCase> cases = testCases;
            int[] collectionSizes = new int[] { 0, 1, 3 };
            testCases = collectionSizes.SelectMany(length =>
                cases.SelectMany(tc =>
                    tc.Items.Combinations(true, length).Select(items =>
                       new CollectionWriterTestCase(items, tc.FunctionImport))));

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                new bool[] { false, true },
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => tc.IsRequest == false),
                (testCase, withModel, testConfig) =>
                {
                    if (testConfig.Format == ODataFormat.Json && !withModel)
                    {
                        return;
                    }

                    IEdmTypeReference type = null;

                    if (withModel)
                    {
                        type = testCase.FunctionImport.Operation.ReturnType.AsCollection().ElementType();
                    }

                    CollectionWriterTestDescriptor testDescriptor = new CollectionWriterTestDescriptor(this.Settings, testCase.FunctionImport.Name, testCase.Items, withModel ? model : null)
                    {
                        ItemTypeParameter = type
                    };

                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test different invalid combinations of collection writing.")]
        public void CollectionPayloadErrorTest()
        {
            EdmModel model = new EdmModel();
            var addressType = new EdmComplexType("TestNS", "AddressType");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String, isNullable: true);
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, isNullable: true);
            addressType.AddStructuralProperty("Zip", EdmPrimitiveTypeKind.Int32, isNullable: false);
            model.AddElement(addressType);
            var container = new EdmEntityContainer("TestNS", "TestContainer");
            model.AddElement(container);

            var testCases = new[]
            {
                new
                {
                    Items = new [] { nullItem, intItem(1), stringItem("One")},
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.String", "Edm.Int32"),
                },
                new
                {
                    Items = new [] { complexItem("TestNS.AddressType"), nullItem, intItem(1) },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                },
                new
                {
                    Items = new [] { stringItem("One"), nullItem, complexItem("TestNS.AddressType") },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },
                new
                {
                    Items = new [] { stringItem("One"), nullItem, GetGeometryPointItem() },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.GeometryPoint", "Edm.String"),
                }
            };

            const string collectionName = "TestCollection";

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testCase, testConfig) =>
                {
                    CollectionWriterTestDescriptor testDescriptor = new CollectionWriterTestDescriptor(this.Settings, collectionName, testCase.Items, testCase.ExpectedException, /*model*/ null);
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);

                    testDescriptor = new CollectionWriterTestDescriptor(this.Settings, collectionName, testCase.Items, testCase.ExpectedException, model);
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test different combinations of collection writing without metadata present.")]
        public void HomogeneousCollectionWriterWithoutMetadataTest()
        {
            var collections = new[]
            {
                // Primitive collection with only nulls
                new CollectionWriterTestDescriptor.ItemDescription[] { nullItem, nullItem, nullItem },

                // Primitive collection with type names on string values
                new CollectionWriterTestDescriptor.ItemDescription[] { stringItem("foo"), stringItem("bar") },

                // Primitive collection with type names on Int32 values
                new CollectionWriterTestDescriptor.ItemDescription[] { intItem(1), intItem(2) },

                // Primitive collection with type names on Int32 values and null values
                new CollectionWriterTestDescriptor.ItemDescription[] { intItem(1), nullItem, intItem(2), nullItem },

                // Complex collection with type names on complex values
                new CollectionWriterTestDescriptor.ItemDescription[] { complexItem("TestNS.AddressType"), complexItem("TestNS.AddressType") },

                // Complex collection with type names on complex values and null values
                new CollectionWriterTestDescriptor.ItemDescription[] { complexItem("TestNS.AddressType"), nullItem, complexItem("TestNS.AddressType") },

                // Complex collection without type names on complex values
                new CollectionWriterTestDescriptor.ItemDescription[] { complexItem(/*typeName*/null), complexItem(/*typeName*/null) },

                // Complex collection without type names on complex values and null values
                new CollectionWriterTestDescriptor.ItemDescription[] { complexItem(/*typeName*/null), nullItem, complexItem(/*typeName*/null) },

                // Primitive collection with different geographic and null values
                new CollectionWriterTestDescriptor.ItemDescription[] {
                    GetGeographyPointItem("GeographyPoint"),
                    nullItem,
                    GetGeographyPolygonItem("GeographyPolygon"),
                    GetGeographyMultiLineStringItem("GeographyMultiLineString"),
                    GetGeographyCollectionItem("GeographyCollection")
                },

                // Primitive collection with different geometric and null values
                new CollectionWriterTestDescriptor.ItemDescription[] {
                    GetGeometryPointItem("GeometryPoint"),
                    nullItem,
                    GetGeometryPolygonItem("GeometryPolygon"),
                    GetGeometryMultiLineStringItem("GeometryMultiLineString"),
                    GetGeometryCollectionItem("GeometryCollection") },
                };

            const string collectionName = "TestCollection";

            this.CombinatorialEngineProvider.RunCombinations(
                collections,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations.Where(tc => tc.IsRequest == false),
                (collection, testConfig) =>
                {
                    CollectionWriterTestDescriptor testDescriptor = new CollectionWriterTestDescriptor(this.Settings, collectionName, collection, null);
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test the the reading of heterogeneous ATOM collection payloads.")]
        public void HeterogeneousCollectionWriterWithoutMetadataTest()
        {
            var collections = new[]
            {
                // Collection with different item type kinds (complex instead of primitive)
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { intItem(0), complexItem(/*typeName*/null) },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },

                // Collection where item type kind does not match item type name (primitive and complex items)
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { stringItem(string.Empty), complexItem(/*typeName*/null) },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },

                // Collection where item type names don't match (Edm.String and Edm.Int32)
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { stringItem(string.Empty), intItem(2) },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                },

                // Collection where item type names don't match (Edm.String and Edm.Int32); including some null items
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { nullItem, stringItem(string.Empty), nullItem, intItem(2), nullItem },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                },

                // Collection where item type names don't match (TestModel.SomeComplexType and TestModel.OtherComplexType)
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { complexItem("TestModel.SomeComplexType"), complexItem("TestModel.OtherComplexType") },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "TestModel.OtherComplexType", "TestModel.SomeComplexType"),
                },

                // Collection where item type names don't match (TestModel.SomeComplexType and TestModel.OtherComplexType); including some null items
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { nullItem, complexItem("TestModel.SomeComplexType"), nullItem, complexItem("TestModel.OtherComplexType"), nullItem },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "TestModel.OtherComplexType", "TestModel.SomeComplexType"),
                },

                // Collection where different item type kinds (primitive instead of complex)
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { complexItem("TestModel.SomeComplexType"), intItem(0) },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                },

                // Collection with primitive and complex elements
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { stringItem("Foo"), complexItem("TestModel.SomeComplexType"), stringItem("Perth") },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },

                // Collection with primitive and geographic elements
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { stringItem("Foo"), GetGeographyMultiLineStringItem() },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.GeographyMultiLineString", "Edm.String"),
                },

                // Collection with primitive and geometric elements
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { stringItem("Foo"), GetGeometryPointItem() },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.GeometryPoint", "Edm.String"),
                },

                // Collection with geographic and geometric elements
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { GetGeographyPointItem(), GetGeometryPointItem() },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.GeometryPoint", "Edm.GeographyPoint"),
                },

                // Collection with complex and geometric elements
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { complexItem(/*typename*/ null), GetGeometryPointItem() },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                },

                // Collection with complex and geometric elements
                new
                {
                    Items = new CollectionWriterTestDescriptor.ItemDescription[] { complexItem(/*typename*/ null), GetGeometryPointItem() },
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                },
            };

            const string collectionName = "TestCollection";

            this.CombinatorialEngineProvider.RunCombinations(
                collections,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations.Where(tc => tc.IsRequest == false),
                (collection, testConfig) =>
                {
                    CollectionWriterTestDescriptor testDescriptor = new CollectionWriterTestDescriptor(this.Settings, collectionName, collection.Items, collection.ExpectedException, null);
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test different combinations of collection writing.")]
        public void CollectionWriterTest()
        {
            int[] collectionSizes = new int[] { 0, 1, 3 };

            // NOTE
            // {0} ... collection name
            // {1} ... data service namespace
            // {2} ... default metadata namespace prefix
            // {3} ... collection item element name 'element'
            // {4} ... atom 'type' attribute name
            // {5} ... data service metadata namespace

            var collectionItem = new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = (object)1,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = new string[] { "1" },
            };

            // TODO: add inner error handling
            var topLevelErrorItem = new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = new ODataAnnotatedError
                {
                    Error = new ODataError()
                    {
                        ErrorCode = "5555",
                        Message = "Dummy error message",
                    }
                },
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = new string[]
                {
                    "{",
                    "$(Indent)\"" + JsonLightConstants.ODataErrorPropertyName + "\":{",
                    "$(Indent)$(Indent)\"code\":\"5555\",\"message\":\"Dummy error message\"",
                    "$(Indent)}",
                    "}"
                }
            };

            var itemsAndOutput = new InvocationsAndExpectedException[]
            {
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[0],
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.EndCollection
                    },
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.Item,
                        CollectionWriterTestDescriptor.WriterInvocations.EndCollection
                    },
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.Error,
                    },
                    ExpectedExceptionFunc = tc => tc.IsRequest
                        ? ODataExpectedExceptions.ODataException("ODataMessageWriter_ErrorPayloadInRequest")
                        : null
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.EndCollection
                    },
                    ExpectedExceptionFunc = tc => ODataExpectedExceptions.ODataException("ODataCollectionWriterCore_InvalidTransitionFromCollection", "Collection", "Collection")
                },

                #region User code throws an exception
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.UserException
                    },
                    ExpectedExceptionFunc = tc => new ExpectedException(typeof(Exception)),
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.UserException
                    },
                    ExpectedExceptionFunc = tc => new ExpectedException(typeof(Exception)),
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.Item,
                        CollectionWriterTestDescriptor.WriterInvocations.UserException
                    },
                    ExpectedExceptionFunc = tc => new ExpectedException(typeof(Exception)),
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.UserException,
                    },
                    ExpectedExceptionFunc = tc => new ExpectedException(typeof(Exception)),
                },
                new InvocationsAndExpectedException
                {
                    Invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
                    {
                        CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.EndCollection,
                        CollectionWriterTestDescriptor.WriterInvocations.UserException,
                    },
                    ExpectedExceptionFunc = tc => new ExpectedException(typeof(Exception)),
                },
                #endregion
            };

            // TODO: add in-stream error tests
            // TODO: Enable JSON lite test configuration

            const string collectionName = "TestCollection";

            var testDescriptors = itemsAndOutput.Select(iao => new CollectionWriterTestDescriptor(this.Settings, collectionName, iao.Invocations, iao.ExpectedExceptionFunc, collectionItem, topLevelErrorItem, null));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfig) =>
                {
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });
        }

        // These tests and helpers are disabled on Silverlight and Phone because they
        // use private reflection not available on Silverlight and Phone
#if !SILVERLIGHT && !WINDOWS_PHONE
        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing collection payloads with complex items that have duplicate property names.")]
        public void DuplicatePropertyNamesTest()
        {
            ODataProperty primitiveProperty = new ODataProperty { Name = "Foo", Value = 1 };
            ODataProperty complexProperty = new ODataProperty { Name = "Foo", Value = new ODataComplexValue { Properties = new[] { new ODataProperty() { Name = "StringProperty", Value = "xyz" } } } };
            ODataProperty collectionProperty = new ODataProperty { Name = "Foo", Value = new ODataCollectionValue { Items = new object[] { 1, 2 } } };

            ExpectedException expectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "Foo");
            Func<WriterTestConfiguration, ExpectedException> expectedExceptionFunc = (tc) => tc.MessageWriterSettings.GetAllowDuplicatePropertyNames() ? null : expectedException;

            DuplicatePropertyNamesTestCase[] testCases = new DuplicatePropertyNamesTestCase[]
            {
                new DuplicatePropertyNamesTestCase
                {
                    CollectionItem = new ODataComplexValue() { Properties = new ODataProperty[] { primitiveProperty, primitiveProperty } },
                    ExpectedODataException = expectedExceptionFunc,
                },
                new DuplicatePropertyNamesTestCase
                {
                    CollectionItem = new ODataComplexValue() { Properties = new ODataProperty[] { primitiveProperty, complexProperty } },
                    ExpectedODataException = expectedExceptionFunc,
                },
                new DuplicatePropertyNamesTestCase
                {
                    CollectionItem = new ODataComplexValue() { Properties = new ODataProperty[] { complexProperty, complexProperty } },
                    ExpectedODataException = expectedExceptionFunc,
                },
                new DuplicatePropertyNamesTestCase
                {
                    CollectionItem = new ODataComplexValue() { Properties = new ODataProperty[] { primitiveProperty, collectionProperty } },
                    ExpectedODataException = (tc) => expectedException,
                },
                new DuplicatePropertyNamesTestCase
                {
                    CollectionItem = new ODataComplexValue() { Properties = new ODataProperty[] { complexProperty, collectionProperty } },
                    ExpectedODataException = (tc) => expectedException,
                },
                new DuplicatePropertyNamesTestCase
                {
                    CollectionItem = new ODataComplexValue() { Properties = new ODataProperty[] { collectionProperty, collectionProperty } },
                    ExpectedODataException = (tc) => expectedException,
                },
            };

            var invocations = new CollectionWriterTestDescriptor.WriterInvocations[]
            {
                CollectionWriterTestDescriptor.WriterInvocations.StartCollection,
                CollectionWriterTestDescriptor.WriterInvocations.Item,
                CollectionWriterTestDescriptor.WriterInvocations.EndCollection
            };

            const string collectionName = "TestCollection";

            IEnumerable<CollectionWriterTestDescriptor> testDescriptors = testCases.Select(testCase =>
                new CollectionWriterTestDescriptor(
                    this.Settings,
                    collectionName,
                    invocations,
                    testCase.ExpectedODataException,
                    new CollectionWriterTestDescriptor.ItemDescription { Item = testCase.CollectionItem }
                ));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfig) =>
                {
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });
        }
#endif

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests behavior of collection writers with collection type specified.")]
        public void CollectionWriterWithTypeProvidedTest()
        {
            EdmModel model = new EdmModel();

            var container = new EdmEntityContainer("DefaultNamespace", "TestContainer");
            model.AddElement(container);

            var testDescriptors = new[]
            {
                // Case with entity type is now covered in TDD tests
                // Incompatible item type
                new CollectionWriterTestDescriptor(this.Settings, "CollectionFunction",
                    new [] { new CollectionWriterTestDescriptor.ItemDescription() { Item = 42 } },
                    ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Int32", "False", "Edm.String", "True"), null)
                {
                    Model = model,
                    ItemTypeParameter = EdmCoreModel.Instance.GetString(isNullable: true)
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeometryPointItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeometryPointValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryPointValue)),
            };
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeometryPolygonItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeometryPolygonValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryPolygonValue)),
            };
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeometryCollectionItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeometryCollectionValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryCollectionValue)),
            };
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeometryMultiLineStringItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeometryMultiLineStringValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryMultiLineStringValue)),
            };
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeographyPointItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeographyPointValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPointValue)),
            };
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeographyPolygonItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeographyPolygonValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPolygonValue)),
            };
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeographyCollectionItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeographyCollectionValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyCollectionValue)),
            };
        }

        private static CollectionWriterTestDescriptor.ItemDescription GetGeographyMultiLineStringItem(string typeName = null)
        {
            return new CollectionWriterTestDescriptor.ItemDescription
            {
                Item = ObjectModelUtils.GeographyMultiLineStringValue,
                ExpectedXml = string.Empty,
                ExpectedJsonLightLines = JsonUtils.GetJsonLines(SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiLineStringValue)),
            };
        }

        private sealed class DuplicatePropertyNamesTestCase
        {
            public ODataComplexValue CollectionItem { get; set; }
            public Func<WriterTestConfiguration, ExpectedException> ExpectedODataException { get; set; }
        }

        private sealed class InvocationsAndExpectedException
        {
            public CollectionWriterTestDescriptor.WriterInvocations[] Invocations { get; set; }
            public Func<WriterTestConfiguration, ExpectedException> ExpectedExceptionFunc { get; set; }
        }

        private sealed class CollectionWriterTestCase
        {
            private readonly CollectionWriterTestDescriptor.ItemDescription[] items;
            private readonly IEdmOperationImport functionImport;

            public CollectionWriterTestCase(CollectionWriterTestDescriptor.ItemDescription[] items, IEdmOperationImport functionImport)
            {
                this.items = items;
                this.functionImport = functionImport;
            }

            public CollectionWriterTestDescriptor.ItemDescription[] Items
            {
                get { return this.items; }
            }

            public IEdmOperationImport FunctionImport
            {
                get { return this.functionImport; }
            }
        }
    }
}
