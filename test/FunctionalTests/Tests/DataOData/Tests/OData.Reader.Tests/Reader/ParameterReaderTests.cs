//---------------------------------------------------------------------
// <copyright file="ParameterReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various entry payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ParameterReaderTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        private enum CreateReaderMethods
        {
            CreateResourceReader,
            CreateResourceSetReader,
            CreateCollectionReader,
        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies error cases for the ODataParameterReader.")]
        public void ParameterReaderErrorTests()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Primitive").First();
            IEdmOperationImport functionImport_Stream = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Stream").First();
            IEdmOperationImport functionImport_Entry = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Entry").First();
            IEdmOperationImport functionImport_Feed = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Feed").First();

            var testConfigurations = this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(
                testConfigurations,
                (testConfiguration) =>
                {
                    // Reading an empty payload should fail.
                    ODataParameterReaderTestWrapper reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration);
                    this.RunParameterReaderErrorTest(reader,
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ParametersMissingInPayload", "FunctionImport_Primitive", "primitive"));

                    // Calling Read/ReadAsync in 'Exception' state should fail.
                    this.RunParameterReaderErrorTest(reader,
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState", "Exception"));

                    // Reading a parameter payload with no parameter should fail because of the missing parameter
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, "{}");
                    this.RunParameterReaderErrorTest(reader,
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ParametersMissingInPayload", "FunctionImport_Primitive", "primitive"));

                    // Duplicated parameter name in the payload should fail.
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, "{ primitive: \"foo\", primitive: \"bar\" }");
                    if (reader != null)
                    {
                        this.RunParameterReaderErrorTest(reader, null, ODataParameterReaderState.Value);
                        this.Assert.AreEqual(ODataParameterReaderState.Value, reader.State, "Reader should be in Value state.");
                        this.Assert.AreEqual("primitive", reader.Name, "Parameter name should be 'primitive'.");
                        this.Assert.AreEqual("foo", reader.Value, "Parameter value should be 'foo'.");
                        this.RunParameterReaderErrorTest(reader,
                            ODataExpectedExceptions.ODataException("ODataParameterReaderCore_DuplicateParametersInPayload", "primitive"));
                    }

                    // Read until we are in Completed state.
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, "{ primitive: \"foo\" }");
                    if (reader != null)
                    {
                        this.RunParameterReaderErrorTest(reader, null, ODataParameterReaderState.Value);
                        this.Assert.AreEqual("primitive", reader.Name, "Parameter name should be 'primitive'.");
                        this.Assert.AreEqual("foo", reader.Value, "Parameter value should be 'foo'.");
                        this.RunParameterReaderErrorTest(reader, null, ODataParameterReaderState.Completed);
                    }

                    // Calling Read/ReadAsync in 'Completed' state should fail.
                    this.RunParameterReaderErrorTest(reader,
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState", "Completed"),
                        ODataParameterReaderState.Completed);

                    // Calling Read in async reader or ReadAsync in sync reader should fail.
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration);
                    if (reader != null)
                    {
                        if (testConfiguration.Synchronous)
                        {
                            this.Assert.ExpectedException(
                                () => ((ODataParameterReaderTestWrapper)reader).ParameterReader.ReadAsync(),
                                ODataExpectedExceptions.ODataException("ODataParameterReaderCore_AsyncCallOnSyncReader"), this.ExceptionVerifier);
                            this.Assert.AreEqual(ODataParameterReaderState.Start, reader.State, "Reader should be in 'Start' state.");
                        }
                        else
                        {
                            this.Assert.ExpectedException(
                                () => ((ODataParameterReaderTestWrapper)reader).ParameterReader.Read(),
                                ODataExpectedExceptions.ODataException("ODataParameterReaderCore_SyncCallOnAsyncReader"), this.ExceptionVerifier);
                            this.Assert.AreEqual(ODataParameterReaderState.Start, reader.State, "Reader should be in 'Start' state.");
                        }
                    }

                    // Reading payload with parameter not in metadata should fail.
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, "{ p1 : \"foo\" }");
                    this.RunParameterReaderErrorTest(reader,
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ParameterNameNotInMetadata", "p1", "FunctionImport_Primitive"));

                    // Reading payload with first parameter not in metadata should fail.
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, "{ p2 : \"foo\", primitive : \"bar\"  }");
                    this.RunParameterReaderErrorTest(reader,
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ParameterNameNotInMetadata", "p2", "FunctionImport_Primitive"));

                    // Reading payload with additional parameter not in metadata should fail.
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, "{ primitive : \"foo\", p2 : \"bar\"  }");
                    if (reader != null)
                    {
                        reader.Read();
                        this.RunParameterReaderErrorTest(reader,
                            ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ParameterNameNotInMetadata", "p2", "FunctionImport_Primitive"));
                    }

                    // Reading a parameter of stream primitive type should fail.
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_Stream, testConfiguration, "{ stream : \"foo\" }");
                    this.RunParameterReaderErrorTest(reader,
                        ODataExpectedExceptions.ODataException("ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType", "stream", "Stream"));
                });
        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies the state machine for create reader methods.")]
        public void ParameterReaderCreateReaderStateMachineTests()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport_Primitive = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Primitive").First();
            IEdmOperationImport functionImport_PrimitiveCollection = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_PrimitiveCollection").First();
            IEdmOperationImport functionImport_Complex = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Complex").First();
            IEdmOperationImport functionImport_ComplexCollection = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_ComplexCollection").First();
            IEdmOperationImport functionImport_Entry = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Entry").First();
            IEdmOperationImport functionImport_Feed = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Feed").First();

            CreateReaderMethods[] createReaderMethods = new CreateReaderMethods[]
            {
                CreateReaderMethods.CreateCollectionReader,
            };

            var testConfigurations = this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(
                testConfigurations,
                createReaderMethods,
                (testConfiguration, createReaderMethod) =>
                {
                    // Calling Create*Reader in Start state should fail.
                    ODataParameterReader reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_Complex, testConfiguration, "{ complex : { PrimitiveProperty : \"456\" } }");
                    this.Assert.ExpectedException(
                        () => CreateSubReader(reader, createReaderMethod),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState", createReaderMethod.ToString(), ODataParameterReaderState.Start.ToString()), this.ExceptionVerifier);
                    this.Assert.AreEqual(ODataParameterReaderState.Start, reader.State, "Unexpected parameter reader state.");

                    // Calling Create*Reader in Value state should fail.
                    reader.Read();
                    this.Assert.AreEqual(ODataParameterReaderState.Resource, reader.State, "Unexpected parameter reader state.");
                    this.Assert.ExpectedException(
                        () => CreateSubReader(reader, createReaderMethod),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState", createReaderMethod.ToString(), ODataParameterReaderState.Resource.ToString()), this.ExceptionVerifier);
                    this.Assert.AreEqual(ODataParameterReaderState.Resource, reader.State, "Unexpected parameter reader state.");

                    if (createReaderMethod != CreateReaderMethods.CreateResourceReader)
                    {
                        // Calling Create*Reader in Entry state should fail.
                        reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_Entry, testConfiguration, "{ entry : {} }");
                        reader.Read();
                        this.Assert.AreEqual(ODataParameterReaderState.Resource, reader.State, "Unexpected parameter reader state.");
                        this.Assert.ExpectedException(
                            () => CreateSubReader(reader, createReaderMethod),
                            ODataExpectedExceptions.ODataException("ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState", createReaderMethod.ToString(), ODataParameterReaderState.Resource.ToString()), this.ExceptionVerifier);
                        this.Assert.AreEqual(ODataParameterReaderState.Resource, reader.State, "Unexpected parameter reader state.");
                    }

                    if (createReaderMethod != CreateReaderMethods.CreateResourceSetReader)
                    {
                        // Calling Create*Reader in Feed state should fail.
                        reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_Feed, testConfiguration, "{ feed : [] }");
                        reader.Read();
                        this.Assert.AreEqual(ODataParameterReaderState.ResourceSet, reader.State, "Unexpected parameter reader state.");
                        this.Assert.ExpectedException(
                            () => CreateSubReader(reader, createReaderMethod),
                            ODataExpectedExceptions.ODataException("ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState", createReaderMethod.ToString(), ODataParameterReaderState.ResourceSet.ToString()), this.ExceptionVerifier);
                        this.Assert.AreEqual(ODataParameterReaderState.ResourceSet, reader.State, "Unexpected parameter reader state.");
                    }

                    if (createReaderMethod != CreateReaderMethods.CreateCollectionReader)
                    {
                        // Calling Create*Reader in Collection state should fail.
                        reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_ComplexCollection, testConfiguration, "{ complexCollection : [] }");
                        reader.Read();
                        this.Assert.AreEqual(ODataParameterReaderState.Collection, reader.State, "Unexpected parameter reader state.");
                        this.Assert.ExpectedException(
                            () => CreateSubReader(reader, createReaderMethod),
                            ODataExpectedExceptions.ODataException("ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState", createReaderMethod.ToString(), ODataParameterReaderState.Collection.ToString()), this.ExceptionVerifier);
                        this.Assert.AreEqual(ODataParameterReaderState.Collection, reader.State, "Unexpected parameter reader state.");
                    }

                    // Calling Read() in Entry/Feed/Collection state without calling Create***Reader should fail.
                    IEdmOperationImport functionImport = createReaderMethod == CreateReaderMethods.CreateResourceReader ? functionImport_Entry : (createReaderMethod == CreateReaderMethods.CreateResourceSetReader ? functionImport_Feed : functionImport_ComplexCollection);
                    string payload = createReaderMethod == CreateReaderMethods.CreateResourceReader ? "{ entry : {} }" : (createReaderMethod == CreateReaderMethods.CreateResourceSetReader ? "{ feed : [] }" : "{ complexCollection : [] }");
                    ODataParameterReaderState expectedParameterState = createReaderMethod == CreateReaderMethods.CreateResourceReader ? ODataParameterReaderState.Resource : ODataParameterReaderState.ResourceSet;
                    var expectedReaderMethod = createReaderMethod == CreateReaderMethods.CreateCollectionReader ? CreateReaderMethods.CreateResourceSetReader.ToString() : createReaderMethod.ToString();
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, payload);
                    reader.Read();
                    this.Assert.AreEqual(expectedParameterState, reader.State, "Unexpected parameter reader state.");
                    this.Assert.ExpectedException(
                        () => reader.Read(),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall", expectedParameterState.ToString(), expectedReaderMethod), this.ExceptionVerifier);
                    this.Assert.AreEqual(expectedParameterState, reader.State, "Unexpected parameter reader state.");

                    // Calling Read() in Entry/Feed/Collection state after Create***Reader() is called but before the created reader finishes should fail.
                    var subReaderMethod = createReaderMethod == CreateReaderMethods.CreateCollectionReader
                        ? CreateReaderMethods.CreateResourceSetReader : createReaderMethod;
                    object subReader = CreateSubReader(reader,
                        subReaderMethod);
                    this.Assert.ExpectedException(
                        () => reader.Read(),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall", expectedParameterState.ToString(), expectedReaderMethod), this.ExceptionVerifier);
                    this.Assert.AreEqual(expectedParameterState, reader.State, "Unexpected parameter reader state.");

                    // Calling Create*Reader() before the sub-reader is completed should fail.
                    string parameterName = createReaderMethod == CreateReaderMethods.CreateResourceReader ? "entry" : (createReaderMethod == CreateReaderMethods.CreateResourceSetReader ? "feed" : "complexCollection");
                    subReader.GetType().GetMethod("Read").Invoke(subReader, null);
                    this.Assert.ExpectedException(
                        () => CreateSubReader(reader, subReaderMethod),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_CreateReaderAlreadyCalled", expectedReaderMethod, parameterName), this.ExceptionVerifier);
                    this.Assert.AreEqual(expectedParameterState, reader.State, "Unexpected parameter reader state.");

                    // Calling Create*Reader() after sub-reader is completed should fail.
                    while ((bool)subReader.GetType().GetMethod("Read").Invoke(subReader, null)) ;
                    this.Assert.AreEqual("Completed", subReader.GetType().GetProperty("State").GetValue(subReader, null).ToString(), "Unexpected sub-reader state.");
                    this.Assert.ExpectedException(
                        () => CreateSubReader(reader, subReaderMethod),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_CreateReaderAlreadyCalled", expectedReaderMethod, parameterName), this.ExceptionVerifier);
                    this.Assert.AreEqual(expectedParameterState, reader.State, "Unexpected parameter reader state.");

                    // Finish reading...
                    reader.Read();
                    this.Assert.AreEqual(ODataParameterReaderState.Completed, reader.State, "Unexpected parameter reader state.");

                    // Calling Create*Reader in Completed state should fail.
                    this.Assert.ExpectedException(
                        () => CreateSubReader(reader, subReaderMethod),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState", expectedReaderMethod, ODataParameterReaderState.Completed.ToString()), this.ExceptionVerifier);
                    this.Assert.AreEqual(ODataParameterReaderState.Completed, reader.State, "Unexpected parameter reader state.");

                    // Exception in subReader should put parent reader in Exception state.
                    payload = createReaderMethod == CreateReaderMethods.CreateResourceReader ? "{ entry : \"foo\" }" : (createReaderMethod == CreateReaderMethods.CreateResourceSetReader ? "{ feed : { \"foo\" : \"bar\" } }" : "{ complexCollection : { \"foo\" : \"bar\" } }");
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, payload);
                    reader.Read();
                    this.Assert.AreEqual(expectedParameterState, reader.State, "Unexpected parameter reader state.");
                    subReader = CreateSubReader(reader, subReaderMethod);
                    this.Assert.IsNotNull(TestExceptionUtils.RunCatching(() => { while ((bool)subReader.GetType().GetMethod("Read").Invoke(subReader, null)) { } }), "Expecting sub-reader.Read() to fail.");
                    this.Assert.AreEqual("Exception", subReader.GetType().GetProperty("State").GetValue(subReader, null).ToString(), "Unexpected sub-reader state.");
                    this.Assert.AreEqual(ODataParameterReaderState.Exception, reader.State, "Unexpected parameter reader state.");

                    // Calling Create*Reader in Exception state should fail.
                    this.Assert.ExpectedException(
                        () => CreateSubReader(reader, subReaderMethod),
                        ODataExpectedExceptions.ODataException("ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState", expectedReaderMethod, ODataParameterReaderState.Exception.ToString()), this.ExceptionVerifier);
                    this.Assert.AreEqual(ODataParameterReaderState.Exception, reader.State, "Unexpected parameter reader state.");
                });
        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies parameter reader will ignore nullable parameters that are not on the payload.")]
        public void ParameterReaderIgnoreMissingNullableParameterTest()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_NullablePrimitive").First();

            this.CombinatorialEngineProvider.RunCombinations(
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest == true),
                (testConfiguration) =>
                {
                    // Reading an empty payload should not fail
                    ODataParameterReaderTestWrapper reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration);
                    this.Assert.IsFalse(reader.Read(), "Read() should return false on an empty payload.");
                    this.Assert.AreEqual(ODataParameterReaderState.Completed, reader.State, "State should be completed.");

                    // Reading a parameter payload with no parameter should not fail
                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, "{}");
                    this.Assert.IsFalse(reader.Read(), "Read() should return false on a parameter payload with no parameter.");
                    this.Assert.AreEqual(ODataParameterReaderState.Completed, reader.State, "State should be completed.");
                });
        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies parameter reader will ignore multiple nullable parameters that are not on the payload.")]
        public void ParameterReaderIgnoreMissingMultipleNullableParameterTest()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_MultipleNullableParameters").Single();

            var parameterValues = new[] 
            {
              "\"p1\" : \"AAEC\"",
              "\"p2\" : true",
              "\"p3\" : 1",
              "\"p5\" : \"2012-05-25T09:00:42.5018138-07:00\"",
              "\"p6\" : 79228162514264337593543950335",
              "\"p7\" : \"INF\"",
              "\"p8\" : \"446806b7-7d7f-4e60-9e2b-bc28d9671a4f\"",
              "\"p9\" : \"32767\"",
              "\"p10\" : 2147483647",
              "\"p11\" : 0",
              "\"p12\" : \"1\"",
              "\"p13\" : \"0.01\"",
              "\"p14\" : { type: \"Point\", coordinates: [ -122.107711791992, 47.6472206115723 ], crs: { type: \"name\", properties: { name: \"EPSG:4326\" } } }",
              "\"p15\" : \"foo\"",
              "\"p16\" : { @odata.type: \"#TestModel.ComplexTypeWithNullableProperties\", IntegerProperty: 1 }",
              "\"p17\" : \"enumType1_value2\"", // supports reading enum values without validation
            };

            var multipleParameterTestCases = parameterValues.Combinations(false, new[] { 1, 2, 16 }).ToArray();

            this.CombinatorialEngineProvider.RunCombinations(
                multipleParameterTestCases,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest == true),
                (testCase, testConfiguration) =>
                {
                    string payload = "{" + string.Join(",", testCase) + "}";

                    var reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport, testConfiguration, payload);
                    foreach (var parameter in testCase)
                    {
                        this.Assert.IsTrue(reader.Read(), "Read() should not fail for payload " + payload);
                        if (parameter.Contains("p16"))
                        {
                            var complexReader = reader.CreateResourceReader();
                            while (complexReader.Read()) ;
                            this.Assert.AreEqual(ODataParameterReaderState.Resource, reader.State, "State should be Value.");
                        }
                        else
                        {
                            this.Assert.AreEqual(ODataParameterReaderState.Value, reader.State, "State should be Value.");
                        }
                    }

                    this.Assert.IsFalse(reader.Read(), "Read() should fail after reading all parameters");
                    this.Assert.AreEqual(ODataParameterReaderState.Completed, reader.State, "State should be Complete.");
                });

        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies null parameter defined as various types in FunctionImport can be read.")]
        public void ParameterReaderAdditionalNullValueTest()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport_Complex = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Complex").First();
            IEdmOperationImport functionImport_PrimitiveCollection = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_PrimitiveCollection").First();
            IEdmOperationImport functionImport_ComplexCollection = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_ComplexCollection").First();

            var testConfigurations = this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(
                testConfigurations,
                (testConfiguration) =>
                {
                    ODataParameterReader reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_PrimitiveCollection, testConfiguration, "{\"primitiveCollection\":null}");
                    reader.Read();
                    this.Assert.AreEqual("primitiveCollection", reader.Name, "Unexpected first parameter name.");
                    this.Assert.IsNull(reader.Value, "Unexpected first parameter value.");
                    this.Assert.AreEqual(ODataParameterReaderState.Value, reader.State, "Unexpected first parameter state.");

                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_PrimitiveCollection, testConfiguration, "{\"primitiveCollection\":123}");
                    this.Assert.ExpectedException(
                        () => reader.Read(),
                        ODataExpectedExceptions.ODataException("ODataJsonLightParameterDeserializer_NullCollectionExpected", "PrimitiveValue", "123"),
                        this.ExceptionVerifier);
                    this.Assert.AreEqual(ODataParameterReaderState.Exception, reader.State, "Unexpected first parameter state.");

                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_ComplexCollection, testConfiguration, "{\"complexCollection\":null}");
                    reader.Read();
                    this.Assert.AreEqual("complexCollection", reader.Name, "Unexpected first parameter name.");
                    this.Assert.IsNull(reader.Value, "Unexpected first parameter value.");
                    this.Assert.AreEqual(ODataParameterReaderState.Value, reader.State, "Unexpected first parameter state.");

                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_ComplexCollection, testConfiguration, "{\"complexCollection\":123}");
                    this.Assert.ExpectedException(
                        () => reader.Read(),
                        ODataExpectedExceptions.ODataException("ODataJsonLightParameterDeserializer_NullCollectionExpected", "PrimitiveValue", "123"),
                        this.ExceptionVerifier);
                    this.Assert.AreEqual(ODataParameterReaderState.Exception, reader.State, "Unexpected first parameter state.");

                    reader = this.CreateParameterReaderForRequestOrResponse(model, functionImport_Complex, testConfiguration, "{\"complex\":null}");
                    reader.Read();
                    this.Assert.AreEqual("complex", reader.Name, "Unexpected first parameter name.");
                    this.Assert.IsNull(reader.Value, "Unexpected first parameter value.");
                    this.Assert.AreEqual(ODataParameterReaderState.Resource, reader.State, "Unexpected first parameter state.");
                });
        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies correct versioning behavior.")]
        public void ParameterReaderVersioningTest()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport_Complex = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Complex").First();

            var testConfigurations = this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(
                testConfigurations,
                (testConfiguration) =>
                {
                    this.Assert.ExpectedException(
                        () =>
                        {
                            ODataParameterReaderTestWrapper reader = CreateODataParameterReader(model, functionImport_Complex, testConfiguration, "{\"complex\":null}");
                            while (reader.Read())
                            {
                                if (reader.State == ODataParameterReaderState.Resource)
                                {
                                    var complexReader = reader.CreateResourceReader();
                                    while (complexReader.Read()) ;
                                }
                            }
                        },
                        null,
                        this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies reading a non-null primitive value for complex parameter will fail.")]
        public void ParameterReaderReadComplexParameterWithPrimitiveValueShouldThrow()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport_Complex = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Complex").First();

            var testConfigurations = this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(
                testConfigurations,
                (testConfiguration) =>
                {
                    this.Assert.ExpectedException(
                        () =>
                        {
                            ODataParameterReaderTestWrapper reader = CreateODataParameterReader(model, functionImport_Complex, testConfiguration, "{\"complex\":1}");
                            while (reader.Read())
                            {
                                if (reader.State == ODataParameterReaderState.Resource)
                                {
                                    var complexReader = reader.CreateResourceReader();
                                    while (complexReader.Read()) ;
                                }
                            }
                        },
                        ODataExpectedExceptions.ODataException("ODataJsonLightReader_UnexpectedPrimitiveValueForODataResource"),
                        this.ExceptionVerifier);
                });
        }

        /// <summary>
        /// Creates an ODataParameterReader with the given input.
        /// </summary>
        /// <param name="model">Model containing the function import.</param>
        /// <param name="functionImport">function import whose parameters are being read.</param>
        /// <param name="testConfiguration">test configuration.</param>
        /// <param name="payload">optional parameter payload.</param>
        /// <returns>Returns the created ODataParameterReader</returns>
        internal static ODataParameterReaderTestWrapper CreateODataParameterReader(IEdmModel model, IEdmOperationImport functionImport, ReaderTestConfiguration testConfiguration, string payload = null)
        {
            // TODO: ODataLib test item: Add new ODataPayloadElement for parameters payload
            // Once the bug is fixed, we should generate the parameters payload from the new ODataPayloadElement to make
            // tests in this file format agnostic.
            TestStream messageStream;
            if (payload != null)
            {
                messageStream = new TestStream(new MemoryStream(Encoding.UTF8.GetBytes(payload)));
            }
            else
            {
                messageStream = new TestStream();
            }

            TestMessage message = TestReaderUtils.CreateInputMessageFromStream(messageStream, testConfiguration, ODataPayloadKind.Parameter, /*customContentTypeHeader*/null, /*urlResolver*/null);
            ODataMessageReaderTestWrapper messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
            return messageReader.CreateODataParameterReader(functionImport);
        }

        /// <summary>
        /// Fix up expected collection parameter payload element for comparison.
        /// </summary>
        /// <param name="expectedResultPayloadElement"></param>
        /// <returns>Modified expected collection payload element</returns>
        internal static ODataPayloadElement FixupExpectedCollectionParameterPayloadElement(ODataPayloadElement expectedResultPayloadElement)
        {
            ComplexInstance expected = expectedResultPayloadElement as ComplexInstance;
            if (expected != null)
            {
                List<ComplexMultiValueProperty> propertiesToReplace = new List<ComplexMultiValueProperty>();
                List<PrimitiveMultiValueProperty> replacementProperties = new List<PrimitiveMultiValueProperty>();
                foreach (PropertyInstance pi in expected.Properties)
                {
                    PrimitiveMultiValueProperty primitiveCollectionProperty = pi as PrimitiveMultiValueProperty;
                    ComplexMultiValueProperty complexCollectionProperty = pi as ComplexMultiValueProperty;
                    if (primitiveCollectionProperty != null)
                    {
                        // collection parameter is an array of element without type name or result wrapper
                        if (primitiveCollectionProperty.Value.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().Any(a => !a.Value))
                        {
                            primitiveCollectionProperty.Value.FullTypeName = null;
                        }
                    }
                    else if (complexCollectionProperty != null)
                    {
                        if (complexCollectionProperty.Value.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().Any(a => !a.Value))
                        {
                            // collection parameter is an array of element without type name or result wrapper
                            complexCollectionProperty.Value.FullTypeName = null;

                            // replace empty ComplexMultiValueProperty with empty PrimitiveMultiValueProperty for comparison since they have the same payload
                            if (complexCollectionProperty.Value.Count == 0)
                            {
                                PrimitiveMultiValueProperty replacementProperty = new PrimitiveMultiValueProperty(complexCollectionProperty.Name, null);
                                replacementProperty.Value.IsNull = complexCollectionProperty.Value.IsNull;
                                foreach (var annotation in complexCollectionProperty.Annotations)
                                {
                                    replacementProperty.Annotations.Add(annotation);
                                }

                                propertiesToReplace.Add(complexCollectionProperty);
                                replacementProperties.Add(replacementProperty);
                            }
                        }
                    }
                }

                for (int i = 0; i < propertiesToReplace.Count; i++)
                {
                    expected.Replace(propertiesToReplace[i], replacementProperties[i]);
                }

                return expected;
            }

            return expectedResultPayloadElement;
        }

        /// <summary>
        /// Creates an ODataParameterReader with the given input.
        /// </summary>
        /// <param name="model">Model containing the function import.</param>
        /// <param name="functionImport">function import whose parameters are being read.</param>
        /// <param name="testConfiguration">test configuration.</param>
        /// <param name="payload">optional parameter payload.</param>
        /// <returns>Returns the created ODataParameterReader</returns>
        private ODataParameterReaderTestWrapper CreateParameterReaderForRequestOrResponse(IEdmModel model, IEdmOperationImport functionImport, ReaderTestConfiguration testConfiguration, string payload = null)
        {
            if (testConfiguration.IsRequest)
            {
                return CreateODataParameterReader(model, functionImport, testConfiguration, payload);
            }
            else
            {
                this.Assert.ExpectedException(
                    () => CreateODataParameterReader(model, functionImport, testConfiguration, payload),
                    ODataExpectedExceptions.ODataException("ODataMessageReader_ParameterPayloadInResponse"),
                    this.ExceptionVerifier);
                return null;
            }
        }

        private object CreateSubReader(ODataParameterReader parameterReader, CreateReaderMethods createMethod)
        {
            switch (createMethod)
            {
                case CreateReaderMethods.CreateResourceReader:
                    return parameterReader.CreateResourceReader();

                case CreateReaderMethods.CreateResourceSetReader:
                    return parameterReader.CreateResourceSetReader();

                case CreateReaderMethods.CreateCollectionReader:
                    return parameterReader.CreateCollectionReader();

                default:
                    throw new NotSupportedException("Unsupported create method: " + createMethod);
            }
        }

        private void RunParameterReaderErrorTest(
            ODataParameterReaderTestWrapper reader,
            ExpectedException expectedException,
            ODataParameterReaderState expectedState = ODataParameterReaderState.Exception)
        {
            if (reader == null)
            {
                // Reader creation failed and we verified the exception already
                return;
            }

            this.Assert.ExpectedException(() => reader.Read(), expectedException, this.ExceptionVerifier);
            this.Assert.AreEqual(expectedState, reader.State, "Reader should be in '" + expectedState.ToString() + "' state.");
        }
    }
}
