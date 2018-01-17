//---------------------------------------------------------------------
// <copyright file="OperationReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of actions and functions in JSON Light.
    /// </summary>
    [TestClass, TestCase]
    public class OperationReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Error testing the reading of odata actions functions.")]
        public void ReadODataOperationErrorTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet cities = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType");

            var testCases = new[]
            {
                new
                {
                    DebugDescription = "Metadata reference property not supported in request.",
                    Json = "\"#operationMetadata\":{}",
                    SkipForRequest = false,
                    SkipForResponse = true,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_MetadataReferencePropertyInRequest"),
                },
                new
                {
                    DebugDescription = "Duplicated metadata reference property names.",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{}, \"#TestModel.PrimitiveResultOperation\":{}",
                    SkipForRequest = true,
                    SkipForResponse = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "#TestModel.PrimitiveResultOperation"),
                },
                new
                {
                    DebugDescription = "Operation value must start with start array or start object.",
                    Json = "\"#TestModel.PrimitiveResultOperation\":null",
                    SkipForRequest = true,
                    SkipForResponse = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue", "#TestModel.PrimitiveResultOperation", "PrimitiveValue"),
                },
                new
                {
                    DebugDescription = "Operation value can only have at most 1 title.",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{\"title\":\"PrimitiveResultOperation\",\"title\":\"PrimitiveResultOperation\"}",
                    SkipForRequest = true,
                    SkipForResponse = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation", "title", "#TestModel.PrimitiveResultOperation"),
                },
                new
                {
                    DebugDescription = "Operation value can only have at most 1 target.",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{\"target\":\"http://www.example.com/foo\",\"target\":\"http://www.example.com/foo\"}",
                    SkipForRequest = true,
                    SkipForResponse = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation", "target", "#TestModel.PrimitiveResultOperation"),
                },
                new
                {
                    DebugDescription = "Operation values inside an array must have target.",
                    Json = "\"#TestModel.PrimitiveResultOperation\":[{},{}]",
                    SkipForRequest = true,
                    SkipForResponse = false,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_OperationMissingTargetProperty", "#TestModel.PrimitiveResultOperation"),
                },
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Select(
                t => new PayloadReaderTestDescriptor(this.Settings)
                {
                    IgnorePropertyOrder = true,
                    DebugDescription = t.DebugDescription,
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation(
                            "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#DefaultContainer.Cities/$entity\"," +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                                "\"Id\":1," +
                                t.Json +
                            "}")
                        .ExpectedEntityType(cityType, cities),
                    ExpectedException = t.ExpectedException,
                    SkipTestConfiguration = tc => tc.IsRequest ? t.SkipForRequest : t.SkipForResponse
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveOperationsNormalizer.Normalize);
                    }

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of odata actions functions.")]
        public void ReadODataOperationTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet cities = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType");

            var testCases = new[]
            {
                new
                {
                    DebugDescription = "Single operation without title and target",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{}",
                    ExpectedServiceOperationDescriptor = new[] {new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"}}
                },
                new
                {
                    DebugDescription = "Single operation with title",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{\"title\":\"PrimitiveResultOperation\"}",
                    ExpectedServiceOperationDescriptor = new[] {new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation" }}
                },
                new
                {
                    DebugDescription = "Single operation with target",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{\"target\":\"http://odata.org/test/PrimitiveResultOperation\"}",
                    ExpectedServiceOperationDescriptor = new[] {new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation" }}
                },
                new
                {
                    DebugDescription = "Single operation with title and target",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{\"title\":\"PrimitiveResultOperation\", \"target\":\"http://odata.org/test/PrimitiveResultOperation\"}",
                    ExpectedServiceOperationDescriptor = new[] {new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation" }}
                },
                new
                {
                    DebugDescription = "Single operation with 2 targets",
                    Json = "\"#TestModel.PrimitiveResultOperation\":[{\"target\":\"http://odata.org/test/PrimitiveResultOperation1\"},{\"target\":\"http://odata.org/test/PrimitiveResultOperation2\"}]",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation1" },
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation2" }
                    }
                },
                new
                {
                    DebugDescription = "Single operation with 2 targets and 2 titles",
                    Json = "\"#TestModel.PrimitiveResultOperation\":[{\"title\":\"title1\",\"target\":\"http://odata.org/test/PrimitiveResultOperation1\"},{\"title\":\"title2\",\"target\":\"http://odata.org/test/PrimitiveResultOperation2\"}]",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "title1", Target = "http://odata.org/test/PrimitiveResultOperation1" },
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "title2", Target = "http://odata.org/test/PrimitiveResultOperation2" }
                    }
                },
                new
                {
                    DebugDescription = "Two operations, one without target or title, the other with with 2 targets",
                    Json = "\"#TestModel.PrimitiveResultOperation\":[{\"target\":\"http://odata.org/test/PrimitiveResultOperation1\"},{\"target\":\"http://odata.org/test/PrimitiveResultOperation2\"}]," +
                           "\"#TestModel.ComplexResultOperation\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation1" },
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation2" },
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.ComplexResultOperation", Title = "TestModel.ComplexResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.ComplexResultOperation" },
                    }
                },
                new
                {
                    DebugDescription = "Two operations, one with target and title, the other with 2 targets and 1 title",
                    Json = "\"#TestModel.PrimitiveResultOperation\":[{\"title\":\"PrimitiveResultOperation\",\"target\":\"http://odata.org/test/PrimitiveResultOperation1\"},{\"target\":\"http://odata.org/test/PrimitiveResultOperation2\"}]," +
                           "\"#TestModel.ComplexResultOperation\":{\"title\":\"ComplexResultOperation\",\"target\":\"http://odata.org/test/ComplexResultOperation\"}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation1" },
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/PrimitiveResultOperation2" },
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.ComplexResultOperation", Title = "ComplexResultOperation", Target = "http://odata.org/test/ComplexResultOperation" }
                    }
                },
                new
                {
                    DebugDescription = "function group with overloads",
                    Json = "\"#TestModel.FunctionImportWithOverload\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload", Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload" },
                    }
                },
                new
                {
                    DebugDescription = "function group with overloads and the overload with 0 parameter",
                    Json = "\"#TestModel.FunctionImportWithOverload\":{}," +
                           "\"#TestModel.FunctionImportWithOverload()\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload", Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload" },
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload" },
                    }
                },
                new
                {
                    DebugDescription = "function group with overloads and the overload with 1 parameter",
                    Json = "\"#TestModel.FunctionImportWithOverload\":{}," +
                           "\"#TestModel.FunctionImportWithOverload(p1)\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload", Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload" },
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("(p1)"), Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload(p1=@p1)" },
                    }
                },
                new
                {
                    DebugDescription = "three overloads of an operation, with 0, 1 and 2 parameters",
                    Json = "\"#TestModel.FunctionImportWithOverload()\":{}," +
                           "\"#TestModel.FunctionImportWithOverload(p1)\":{}," +
                           "\"#TestModel.FunctionImportWithOverload(p1,p2)\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload" },
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("(p1)"), Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload(p1=@p1)" },
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("(p1,p2)"), Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.FunctionImportWithOverload(p1=@p1,p2=@p2)" },
                    }
                },
                new
                {
                    DebugDescription = "Two overloads of an operation, one with target and title, the other with 2 targets and 1 title",
                    Json = "\"#TestModel.FunctionImportWithOverload()\":[{\"title\":\"FunctionImportWithOverload\",\"target\":\"http://odata.org/test/FunctionImportWithOverload/Target1\"},{\"target\":\"http://odata.org/test/FunctionImportWithOverload/Target2\"}]," +
                           "\"#TestModel.FunctionImportWithOverload(p1)\":{\"title\":\"FunctionImportWithOverload(p1)\",\"target\":\"http://odata.org/test/FunctionImportWithOverload(p1=@p1)\"}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("()"), Title = "FunctionImportWithOverload", Target = "http://odata.org/test/FunctionImportWithOverload/Target1" },
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.FunctionImportWithOverload", Target = "http://odata.org/test/FunctionImportWithOverload/Target2" },
                        new ServiceOperationDescriptor { IsAction = false, Metadata = "http://odata.org/test/$metadata#TestModel.FunctionImportWithOverload" + Uri.EscapeDataString("(p1)"), Title = "FunctionImportWithOverload(p1)", Target = "http://odata.org/test/FunctionImportWithOverload(p1=@p1)" }
                    }
                },
                new
                {
                    DebugDescription = "The operation that is not in model",
                    Json = "\"#TestModel.UnknownOperation()\":[{\"title\":\"FunctionImportWithOverload\",\"target\":\"http://odata.org/test/FunctionImportWithOverload/Target1\"}]",
                    ExpectedServiceOperationDescriptor = new ServiceOperationDescriptor[]{},
                },
                new
                {
                    DebugDescription = "Single operation with title and a operation that is not in model",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{\"title\":\"PrimitiveResultOperation\"}," + 
                           "\"#TestModel.UnknownOperation\":{\"title\":\"UnknownOperation\"}",
                    ExpectedServiceOperationDescriptor = new[] {new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation" }}
                },
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Select(
                t => new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = t.DebugDescription,
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation(
                            "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#DefaultContainer.Cities/$entity\"," +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                                "\"Id\":1," +
                                "\"Name\":\"cityTypeName\"," +
                                t.Json +
                            "}")
                        .ExpectedEntityType(cityType, cities),
                    ExpectedResultPayloadElement = (tc) =>
                    {
                        var entityInstance = PayloadBuilder.Entity("TestModel.CityType").
                            PrimitiveProperty("Id", 1).
                            PrimitiveProperty("Name", "cityTypeName").
                            StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/test/Cities(1)/Skyline").
                            NavigationProperty("CityHall", "http://odata.org/test/Cities(1)/CityHall").
                            NavigationProperty("DOL", "http://odata.org/test/Cities(1)/DOL").
                            NavigationProperty("PoliceStation", "http://odata.org/test/Cities(1)/PoliceStation");
                        foreach (var op in t.ExpectedServiceOperationDescriptor)
                        {
                            entityInstance.OperationDescriptor(op);
                        }
                        return entityInstance;
                    },
                    SkipTestConfiguration = tc => tc.IsRequest
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveOperationsNormalizer.Normalize);
                    }

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of odata actions functions.")]
        public void ReadBindableODataOperationTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;
            EdmEntityContainer defaultContainer = model.FindEntityContainer("DefaultContainer") as EdmEntityContainer;
            IEdmEntitySet cities = defaultContainer.FindEntitySet("Cities");
            IEdmEntityType cityType = (IEdmEntityType)model.FindDeclaredType("TestModel.CityType");

            var addressInCity1BindableFunctionWithOverload = defaultContainer.AddActionAndActionImport(model, "AddressInCity", EdmCoreModel.Instance.GetBoolean(isNullable: true), null, true);
            var addressInCity1Action = addressInCity1BindableFunctionWithOverload.Action.AsEdmAction();
            addressInCity1Action.AddParameter("city", new EdmEntityTypeReference(cityType, isNullable: true));
            addressInCity1Action.AddParameter("address", EdmCoreModel.Instance.GetString(isNullable: true));

            var funcWithOneParamImport = defaultContainer.AddFunctionAndFunctionImport(model, "BindableFunctionWithOverload", EdmCoreModel.Instance.GetInt32(true), null, false, true);
            var funcWithOneParam = funcWithOneParamImport.Function.AsEdmFunction();
            funcWithOneParam.AddParameter("p1", new EdmEntityTypeReference(cityType, isNullable: true));

            var funcWithTwoParamImport = defaultContainer.AddFunctionAndFunctionImport(model, "BindableFunctionWithOverload", EdmCoreModel.Instance.GetInt32(true), null, false, true);
            var funcWithTwoParam = funcWithTwoParamImport.Function.AsEdmFunction();
            funcWithTwoParam.AddParameter("p1", new EdmEntityTypeReference(cityType, isNullable: true));
            funcWithTwoParam.AddParameter("p2", EdmCoreModel.Instance.GetString(isNullable: true));

            var testCases = new[]
            {
                new
                {
                    DebugDescription = "No always bindable operations in payload",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.AddressInCity", Title = "TestModel.AddressInCity", Target = "http://odata.org/test/Cities(1)/TestModel.AddressInCity"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("(p2)"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload(p2=@p2)"},
                    }
                },
                new
                {
                    DebugDescription = "Always bindable operations from default container and always bindable function group in payload",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{},\"#TestModel.AddressInCity\":{},\"#TestModel.BindableFunctionWithOverload\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.AddressInCity", Title = "TestModel.AddressInCity", Target = "http://odata.org/test/Cities(1)/TestModel.AddressInCity"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload", Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload"},
                    }
                },
                new
                {
                    DebugDescription = "Always bindable operations and function group with namespace from default container in payload",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{},\"#TestModel.AddressInCity\":{},\"#TestModel.BindableFunctionWithOverload\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.AddressInCity", Title = "TestModel.AddressInCity", Target = "http://odata.org/test/Cities(1)/TestModel.AddressInCity"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload", Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload"},
                    }
                },
                new
                {
                    DebugDescription = "Always bindable operations from non-default container and function overload with 1 param in payload",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{},\"#TestModel.AddressInCity\":{},\"#TestModel.BindableFunctionWithOverload(p1)\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.AddressInCity", Title = "TestModel.AddressInCity", Target = "http://odata.org/test/Cities(1)/TestModel.AddressInCity"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("(p2)"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload(p2=@p2)"},
                    }
                },
                new
                {
                    DebugDescription = "Always bindable operations and function overload with 2 params, with namespace from non-default container in payload",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{},\"#TestModel.AddressInCity\":{},\"#TestModel.BindableFunctionWithOverload(p1)\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.AddressInCity", Title = "TestModel.AddressInCity", Target = "http://odata.org/test/Cities(1)/TestModel.AddressInCity"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("(p2)"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload(p2=@p2)"},
                    }
                },
                new
                {
                    DebugDescription = "All of the always bindable operations in payload",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{},\"#TestModel.AddressInCity\":{}," +
                           "\"#TestModel.BindableFunctionWithOverload(p1)\":{},\"#TestModel.BindableFunctionWithOverload(p1,p2)\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.AddressInCity", Title = "TestModel.AddressInCity", Target = "http://odata.org/test/Cities(1)/TestModel.AddressInCity"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("(p2)"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload(p2=@p2)"},
                    }
                },
                new
                {
                    DebugDescription = "All of the always bindable operations with namespace in payload",
                    Json = "\"#TestModel.PrimitiveResultOperation\":{},\"#TestModel.AddressInCity\":{}," +
                           "\"#TestModel.BindableFunctionWithOverload(p1)\":{},\"#TestModel.BindableFunctionWithOverload(p1,p2)\":{}",
                    ExpectedServiceOperationDescriptor = new[]
                    {
                        new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/test/$metadata#TestModel.AddressInCity", Title = "TestModel.AddressInCity", Target = "http://odata.org/test/Cities(1)/TestModel.AddressInCity"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.PrimitiveResultOperation", Title = "TestModel.PrimitiveResultOperation", Target = "http://odata.org/test/Cities(1)/TestModel.PrimitiveResultOperation"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("()"), Title = "TestModel.BindableFunctionWithOverload", Target = "http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload"},
                        new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/test/$metadata#TestModel.BindableFunctionWithOverload" + Uri.EscapeDataString("(p2)"), Title = "TestModel.BindableFunctionWithOverload", Target="http://odata.org/test/Cities(1)/TestModel.BindableFunctionWithOverload(p2=@p2)"},
                    }
                },
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Select(
                t => new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = t.DebugDescription,
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation(
                            "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#DefaultContainer.Cities/$entity\"," +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                                "\"Id\":1," +
                                "\"Name\":\"New York\"," +
                                t.Json +
                            "}")
                        .ExpectedEntityType(cityType, cities),
                    ExpectedResultPayloadElement = (tc) =>
                    {
                        var entityInstance = PayloadBuilder.Entity("TestModel.CityType").
                            PrimitiveProperty("Id", 1).
                            PrimitiveProperty("Name", "New York").
                            StreamProperty("Skyline", "http://odata.org/test/Cities(1)/Skyline", "http://odata.org/test/Cities(1)/Skyline").
                            NavigationProperty("CityHall", "http://odata.org/test/Cities(1)/CityHall").
                            NavigationProperty("DOL", "http://odata.org/test/Cities(1)/DOL").
                            NavigationProperty("PoliceStation", "http://odata.org/test/Cities(1)/PoliceStation");
                        foreach (var op in t.ExpectedServiceOperationDescriptor)
                        {
                            entityInstance.OperationDescriptor(op);
                        }
                        return entityInstance;
                    },
                    SkipTestConfiguration = tc => tc.IsRequest
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveOperationsNormalizer.Normalize);
                    }

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();

                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}