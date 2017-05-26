//---------------------------------------------------------------------
// <copyright file="PropertyReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various top-level property payloads.
    /// </summary>
    [TestClass, TestCase]
    public class PropertyReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Properties"), Variation(Description = "Test reading of top-level property payloads with metadata.")]
        public void TopLevelPropertiesWithMetadataTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreatePrimitiveValueTestDescriptors(this.Settings);
            testDescriptors = testDescriptors.Concat(PayloadReaderTestDescriptorGenerator.CreateComplexValueTestDescriptors(this.Settings, true));
            testDescriptors = testDescriptors.Concat(PayloadReaderTestDescriptorGenerator.CreateCollectionTestDescriptors(this.Settings, true));

            testDescriptors = testDescriptors.Select(collectionTestDescriptor => collectionTestDescriptor.InProperty("propertyName"));
            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            // Limit to only top-level property payloads
            testDescriptors = testDescriptors.Where(td => td.PayloadElement is PropertyInstance);

            // Add a couple of invalid cases which use a standard model
            EdmModel model = new EdmModel();

            model.ComplexType("UnusedComplexType");

            EdmEntityType unusedEntityType = model.EntityType("UnusedEntityType");
            unusedEntityType.AddKeys(unusedEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            unusedEntityType.Property("Name", EdmPrimitiveTypeKind.String, isNullable: false);

            EdmEntityType streamPropertyEntityType = model.EntityType("EntityTypeWithStreamProperty");
            streamPropertyEntityType.AddKeys(streamPropertyEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            streamPropertyEntityType.AddStructuralProperty("Video", EdmPrimitiveTypeKind.Stream, isNullable: false);
            streamPropertyEntityType.Property("NonStreamProperty", EdmPrimitiveTypeKind.Boolean, isNullable: false);

            EdmEntityType navigationPropertyEntityType = model.EntityType("EntityTypeWithNavigationProperty");
            navigationPropertyEntityType.AddKeys(navigationPropertyEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            navigationPropertyEntityType.NavigationProperty("Navigation", streamPropertyEntityType);

            model.Fixup();

            EdmEntityContainer container = model.EntityContainer as EdmEntityContainer;

            EdmFunction nameFunction = new EdmFunction(container.Namespace, "NameFunctionImport", EdmCoreModel.Instance.GetInt32(false), false /*isBound*/, null, false /*isComposable*/);
            model.AddElement(nameFunction);
            container.AddFunctionImport("NameFunctionImport", nameFunction);
            model.Fixup();

            var videoPropertyType = model.GetEntityType("TestModel.EntityTypeWithStreamProperty").Properties().Single(p => p.Name == "Video").Type;

            var explicitTestDescriptors = new[]
                {
                    // Non existant type name
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Property("propertyName", PayloadBuilder.ComplexValue("TestModel.NonExistantType")),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "TestModel.NonExistantType"),
                        // This test has different meaning in JSON-L (no expected type + non-existent typename)
                        SkipTestConfiguration = (tc) => tc.Format == ODataFormat.Json,
                    },
                    // Existing type name without namespace
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Property("propertyName", PayloadBuilder.ComplexValue("UnusedComplexType")),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "UnusedComplexType"),
                        // This test has different meaning in JSON-L (no expected type + non-existent typename)
                        SkipTestConfiguration = (tc) => tc.Format == ODataFormat.Json,
                    },
                    // Existing type of wrong kind
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Property("propertyName", PayloadBuilder.ComplexValue("TestModel.UnusedEntityType")),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectValueTypeKind", "TestModel.UnusedEntityType", "Entity"),
                        // This test has different meaning in JSON-L
                        SkipTestConfiguration = (tc) => tc.Format == ODataFormat.Json,
                    },
                    // A stream is not allowed in a property with a non-stream property kind.
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.EntityTypeWithStreamProperty").StreamProperty("NonStreamProperty", "http://readlink", "http://editlink"),
                        PayloadEdmModel = model,
                        ExpectedResultCallback = tc =>
                            new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException = tc.Format == ODataFormat.Json
                                        ? ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType", "NonStreamProperty", "Edm.Boolean")
                                        : ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject")
                            },
                    },
                    // Top-level property of stream type is not allowed
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveProperty("Video", 42).ExpectedPropertyType(videoPropertyType),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeStream"),
                    },
                    // Top-level deferred navigation property is not allowed
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveProperty("Video", 42).ExpectedPropertyType(streamPropertyEntityType),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityKind"),
                    },
                    // Top-level expanded navigation property is not allowed
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveProperty("Video", 42).ExpectedPropertyType(streamPropertyEntityType),
                        PayloadEdmModel = model,
                        ExpectedException = ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityKind"),
                    },
                };

            testDescriptors = testDescriptors.Concat(explicitTestDescriptors);

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    var property = testDescriptor.PayloadElement as PropertyInstance;
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class NonNullablePropertyTest
        {
            public object Value { get; set; }
            public IEdmTypeReference DataType { get; set; }
            public string TypeName { get; set; }
        }

        [TestMethod, TestCategory("Reader.Properties"), Variation(Description = "Test reading of non-nullable property payloads with 'null' value and metadata.")]
        // TODO: Change the payload of null top-level properties #645
        public void NonNullablePropertiesWithNullValuesTest()
        {
            IEnumerable<NonNullablePropertyTest> testCases = new NonNullablePropertyTest[]
            {
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetString(false), TypeName = "Edm.String" },
                new NonNullablePropertyTest { Value = "foo", DataType = EdmCoreModel.Instance.GetString(false), TypeName = "Edm.String" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetBinary(false), TypeName = "Edm.Binary" },
                new NonNullablePropertyTest { Value = new byte[] { 1, 2, 3 }, DataType = EdmCoreModel.Instance.GetBinary(false), TypeName = "Edm.Binary" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetBoolean(false), TypeName = "Edm.Boolean" },
                new NonNullablePropertyTest { Value = true, DataType = EdmCoreModel.Instance.GetBoolean(false), TypeName = "Edm.Boolean" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetByte(false), TypeName = "Edm.Byte" },
                new NonNullablePropertyTest { Value = (byte)1, DataType = EdmCoreModel.Instance.GetByte(false), TypeName = "Edm.Byte" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetDateTimeOffset(false), TypeName = "Edm.DateTimeOffset" },
                new NonNullablePropertyTest { Value = new DateTimeOffset(new DateTime(2011, 08, 31), TimeSpan.Zero), DataType = EdmCoreModel.Instance.GetDateTimeOffset(false), TypeName = "Edm.DateTimeOffset" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetDecimal(false), TypeName = "Edm.Decimal" },
                new NonNullablePropertyTest { Value = (decimal)1.0, DataType = EdmCoreModel.Instance.GetDecimal(false), TypeName = "Edm.Decimal" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetDouble(false), TypeName = "Edm.Double" },
                new NonNullablePropertyTest { Value = (double)1.0, DataType = EdmCoreModel.Instance.GetDouble(false), TypeName = "Edm.Double" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetSingle(false), TypeName = "Edm.Single" },
                new NonNullablePropertyTest { Value = (float)1.0, DataType = EdmCoreModel.Instance.GetSingle(false), TypeName = "Edm.Single" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetSByte(false), TypeName = "Edm.SByte" },
                new NonNullablePropertyTest { Value = (sbyte)1, DataType = EdmCoreModel.Instance.GetSByte(false), TypeName = "Edm.SByte" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetInt16(false), TypeName = "Edm.Int16" },
                new NonNullablePropertyTest { Value = (Int16)1, DataType = EdmCoreModel.Instance.GetInt16(false), TypeName = "Edm.Int16" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetInt32(false), TypeName = "Edm.Int32" },
                new NonNullablePropertyTest { Value = (Int32)1, DataType = EdmCoreModel.Instance.GetInt32(false), TypeName = "Edm.Int32" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetInt64(false), TypeName = "Edm.Int64" },
                new NonNullablePropertyTest { Value = (Int64)1, DataType = EdmCoreModel.Instance.GetInt64(false), TypeName = "Edm.Int64" },
                new NonNullablePropertyTest { Value = null, DataType = EdmCoreModel.Instance.GetGuid(false), TypeName = "Edm.Guid" },
                new NonNullablePropertyTest { Value = Guid.NewGuid(), DataType = EdmCoreModel.Instance.GetGuid(false), TypeName = "Edm.Guid" },
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                testCases.Select(testCase => new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveValue(testCase.Value).WithTypeAnnotation(testCase.DataType),
                    PayloadEdmModel = new EdmModel().Fixup(),
                    ExpectedException = testCase.Value == null
                        ? ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullNamedValueForNonNullableType", "value", testCase.TypeName)
                        : null,
                });

            testDescriptors = testDescriptors.Select(td => td.InProperty("propertyName"));
            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (!(testDescriptor.PayloadElement is PrimitiveProperty)
                        && testDescriptor.ExpectedException != null)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);
                        testDescriptor.ExpectedException = ODataExpectedExceptions.ODataException(
                            "ReaderValidationUtils_NullNamedValueForNonNullableType",
                            "propertyName",
                            testDescriptor.ExpectedException.ExpectedMessage.Arguments.ElementAt(1));
                    }

                    var property = testDescriptor.PayloadElement as PropertyInstance;

                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
