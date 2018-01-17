//---------------------------------------------------------------------
// <copyright file="MessageReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestMessage = Microsoft.Test.Taupo.OData.Common.TestMessage;
    using TestStream = Microsoft.Test.Taupo.OData.Common.TestStream;
    #endregion Namespaces

    /// <summary>
    /// Tests for the <see cref="ODataMessageReader" /> class to read different payloads.
    /// </summary>
    [TestClass, TestCase]
    public class MessageReaderTests : ODataReaderTestCase
    {
        private SettingsActionTestCase[] settingsActionTestCases = new[]
            {
                new SettingsActionTestCase {
                    Action = new Action<TestMessage, ODataMessageReaderSettings>((message, s) => new ODataMessageReader((IODataRequestMessage)message, s)),
                    Response = false
                },
                new SettingsActionTestCase {
                    Action = new Action<TestMessage, ODataMessageReaderSettings>((message, s) => new ODataMessageReader((IODataRequestMessage)message, s, null)),
                    Response = false
                },
                new SettingsActionTestCase {
                    Action = new Action<TestMessage, ODataMessageReaderSettings>((message, s) => new ODataMessageReader((IODataResponseMessage)message, s)),
                    Response = true
                },
                new SettingsActionTestCase {
                    Action = new Action<TestMessage, ODataMessageReaderSettings>((message, s) => new ODataMessageReader((IODataResponseMessage)message, s, null)),
                    Response = true
                },
            };

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verifies correct behavior of constructor of ODataMessageReader in regard to argument validation of message parameter.")]
        public void MessageReaderConstructorNullMessageTest()
        {
            var nullMessageActions = new Action[]
            {
                new Action(() => new ODataMessageReader((IODataRequestMessage)null)),
                new Action(() => new ODataMessageReader((IODataRequestMessage)null, new ODataMessageReaderSettings())),
                new Action(() => new ODataMessageReader((IODataRequestMessage)null, new ODataMessageReaderSettings(), null)),
                new Action(() => new ODataMessageReader((IODataResponseMessage)null)),
                new Action(() => new ODataMessageReader((IODataResponseMessage)null, new ODataMessageReaderSettings())),
                new Action(() => new ODataMessageReader((IODataResponseMessage)null, new ODataMessageReaderSettings(), null)),
            };

            this.CombinatorialEngineProvider.RunCombinations(
                nullMessageActions,
                (nullMessageAction) =>
                {
                    this.Assert.ThrowsException<ArgumentException>(nullMessageAction, "Should have failed.");
                });
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verifies correct behavior of constructor of ODataMessageReader in regard to version validation.")]
        public void MessageReaderConstructorVersionsTest()
        {
            var versions = new[]
                {
                    new
                    {
                        DataServiceVersion = "4.0",
                        ODataVersion = ODataVersion.V4,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        DataServiceVersion = "4.01",
                        ODataVersion = ODataVersion.V401,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        DataServiceVersion = "5.0",
                        ODataVersion = ODataVersion.V4,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataUtils_UnsupportedVersionHeader", "5.0")
                    },
                    new
                    {
                        DataServiceVersion = "0.5",
                        ODataVersion = ODataVersion.V4,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataUtils_UnsupportedVersionHeader", "0.5")
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                settingsActionTestCases,
                versions,
                ODataVersionUtils.AllSupportedVersions,
                (settingsAction, version, maxProtocolVersion) =>
                {
                    TestMessage message = settingsAction.Response
                        ? (TestMessage)new TestResponseMessage(new MemoryStream())
                        : (TestMessage)new TestRequestMessage(new MemoryStream());
                    message.SetHeader(ODataConstants.ODataVersionHeader, version.DataServiceVersion);

                    ODataMessageReaderSettings settings = new ODataMessageReaderSettings() { MaxProtocolVersion = maxProtocolVersion };

                    ExpectedException expectedException = version.ExpectedException ??
                        (maxProtocolVersion < version.ODataVersion
                            ? ODataExpectedExceptions.ODataException("ODataUtils_MaxProtocolVersionExceeded", version.ODataVersion.ToText(), maxProtocolVersion.ToText())
                            : (ExpectedException)null);

                    this.Assert.ExpectedException(
                        () => settingsAction.Action(message, settings),
                        expectedException,
                        this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verifies correct behavior of CreateResourceSetReader method in regard to argument validation.")]
        public void CreateResourceSetReaderArgumentTest()
        {
            IEdmEntityType entityType = null;
            IEdmComplexType complexType = null;
            IEdmModel model = this.CreateTestMetadata(out entityType, out complexType);

            this.CombinatorialEngineProvider.RunCombinations(
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testConfiguration) =>
                {
                    TestMessage message = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration);
                    using (ODataMessageReaderTestWrapper messageReader = TestReaderUtils.CreateMessageReader(message, null, testConfiguration))
                    {
                        this.Assert.ExpectedException(
                            () => messageReader.CreateODataResourceSetReader(entityType),
                            ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata", "expectedBaseEntityType"),
                            this.ExceptionVerifier);
                    }
                });
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verifies correct behavior of CreateResourceReader method in regard to argument validation.")]
        public void CreateResourceReaderArgumentTest()
        {
            IEdmEntityType entityType = null;
            IEdmComplexType complexType = null;
            IEdmModel model = this.CreateTestMetadata(out entityType, out complexType);

            this.CombinatorialEngineProvider.RunCombinations(
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testConfiguration) =>
                {
                    TestMessage message = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration);
                    ODataMessageReaderTestWrapper messageReader = TestReaderUtils.CreateMessageReader(message, null, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.CreateODataResourceReader(entityType),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata", "resourceType"),
                        this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verifies correct behavior of ReadProperty method in regard to argument validation.")]
        public void ReadPropertyArgumentTest()
        {
            IEdmEntityType entityType = null;
            IEdmComplexType complexType = null;
            IEdmModel model = this.CreateTestMetadata(out entityType, out complexType);
            IEdmEntityContainer container = model.FindEntityContainer("TestContainer");
            IEdmOperationImport entityValueFunctionImport = container.FindOperationImports("EntityValueFunctionImport").Single();
            IEdmOperationImport entityCollectionValueFunctionImport = container.FindOperationImports("CollectionOfEntitiesFunctionImport").Single();

            IEdmStructuralProperty entityValueStructuralProperty = (IEdmStructuralProperty)complexType.FindProperty("EntityProp");
            IEdmStructuralProperty entityCollectionValueStructuralProperty = (IEdmStructuralProperty)complexType.FindProperty("EntityCollectionProp");

            this.CombinatorialEngineProvider.RunCombinations(
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testConfiguration) =>
                {
                    TestMessage message = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration);

                    ODataMessageReaderTestWrapper messageReader = TestReaderUtils.CreateMessageReader(message, null, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.ReadProperty(new EdmComplexTypeReference(complexType, false)),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata", "expectedPropertyTypeReference"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.ReadProperty(new EdmEntityTypeReference(entityType, false)),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityKind"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.ReadProperty(new EdmCollectionType(new EdmEntityTypeReference(entityType, false)).ToTypeReference()),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.ReadProperty(entityValueFunctionImport),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityKind"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.ReadProperty(entityCollectionValueFunctionImport),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.ReadProperty(entityValueStructuralProperty),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityKind"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.ReadProperty(entityCollectionValueStructuralProperty),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind"),
                        this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verifies correct behavior of CreateCollectionReader method in regard to argument validation.")]
        public void CreateCollectionReaderArgumentTest()
        {
            IEdmEntityType entityType = null;
            IEdmComplexType complexType = null;
            IEdmModel model = this.CreateTestMetadata(out entityType, out complexType);
            IEdmEntityContainer defaultContainer = model.FindEntityContainer("TestNS.TestContainer");
            IEdmOperationImport primitiveValueFunctionImport = defaultContainer.FindOperationImports("PrimitiveValueFunctionImport").Single();
            IEdmOperationImport collectionOfEntitiesFunctionImport = defaultContainer.FindOperationImports("CollectionOfEntitiesFunctionImport").Single();

            this.CombinatorialEngineProvider.RunCombinations(
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testConfiguration) =>
                {
                    TestMessage message = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration);
                    ODataMessageReaderTestWrapper messageReader = TestReaderUtils.CreateMessageReader(message, null, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.CreateODataCollectionReader(new EdmComplexTypeReference(complexType, false)),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata", "expectedItemTypeReference"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.CreateODataCollectionReader(new EdmEntityTypeReference(entityType, false)),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedCollectionTypeWrongKind", "Entity"),
                        this.ExceptionVerifier);

                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.CreateODataCollectionReader(collectionOfEntitiesFunctionImport),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedCollectionTypeWrongKind", "Entity"),
                        this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verifies CreateParameterReader.")]
        public void CreateParameterReaderTest()
        {
            IEdmOperationImport functionImport;
            IEdmModel model = this.CreateTestMetadata(out functionImport);

            var testConfigurations = this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(
                testConfigurations,
                (testConfiguration) =>
                {
                    TestMessage message;
                    ODataMessageReaderTestWrapper messageReader;

                    // Specifying a non-null functionImport without an EdmModel should fail.
                    message = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration, ODataPayloadKind.Parameter, /*customContentTypeHeader*/null, /*urlResolver*/null);
                    messageReader = TestReaderUtils.CreateMessageReader(message, null, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.CreateODataParameterReader(functionImport),
                        ODataExpectedExceptions.ArgumentException("ODataMessageReader_OperationSpecifiedWithoutMetadata", "operation"),
                        this.ExceptionVerifier);

                    // Specifying a null functionImport should fail.
                    message = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration, ODataPayloadKind.Parameter, /*customContentTypeHeader*/null, /*urlResolver*/null);
                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    this.Assert.ExpectedException(
                        () => messageReader.CreateODataParameterReader(null),
                            ODataExpectedExceptions.ArgumentNullException("ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader", "operation"),
                        this.ExceptionVerifier);

                    message = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration, ODataPayloadKind.Parameter, /*customContentTypeHeader*/null, /*urlResolver*/null);
                    messageReader = TestReaderUtils.CreateMessageReader(message, model, testConfiguration);
                    if (testConfiguration.IsRequest)
                    {
                        // CreateParameterReader should succeed.
                        messageReader.CreateODataParameterReader(functionImport);
                    }
                    else
                    {
                        // CreateODataParameterReader on a response message should fail.
                        this.Assert.ExpectedException(
                            () => messageReader.CreateODataParameterReader(functionImport),
                            ODataExpectedExceptions.ODataException("ODataMessageReader_ParameterPayloadInResponse"),
                            this.ExceptionVerifier);
                    }
                });
        }

        private IEdmModel CreateTestMetadata(out IEdmOperationImport functionImport)
        {
            IEdmEntityType entityType;
            IEdmComplexType complexType;
            var model = this.CreateTestMetadata(out entityType, out complexType);
            var container = model.FindEntityContainer("TestContainer");
            functionImport = container.FindOperationImports("FunctionImport1").Single();
            return model;
        }

        private IEdmModel CreateTestMetadata(out IEdmEntityType entityType, out IEdmComplexType complexType)
        {
            EdmModel model = new EdmModel();

            EdmEntityType modelEntityType = model.EntityType("EntityType", "TestNS")
                .KeyProperty("Id", EdmCoreModel.Instance.GetInt32(false) as EdmTypeReference);

            EdmComplexType modelComplexType = model.ComplexType("ComplexType", "TestNS")
                .Property("EntityProp", modelEntityType.ToTypeReference() as EdmTypeReference)
                .Property("EntityCollectionProp", EdmCoreModel.GetCollection(modelEntityType.ToTypeReference()) as EdmTypeReference);

            EdmEntityContainer container = new EdmEntityContainer("TestNS", "TestContainer");
            container.AddFunctionAndFunctionImport(model, "FunctionImport1", EdmCoreModel.Instance.GetInt32(false));
            container.AddFunctionAndFunctionImport(model, "PrimitiveValueFunctionImport", EdmCoreModel.Instance.GetInt32(false));
            container.AddFunctionAndFunctionImport(model, "EntityValueFunctionImport", modelEntityType.ToTypeReference());
            container.AddFunctionAndFunctionImport(model, "CollectionOfEntitiesFunctionImport", EdmCoreModel.GetCollection(modelEntityType.ToTypeReference()));
            model.AddElement(container);

            model.Fixup();

            entityType = (IEdmEntityType)model.FindType("TestNS.EntityType");
            ExceptionUtilities.Assert(entityType != null, "entityType != null");
            
            complexType = (IEdmComplexType)model.FindType("TestNS.ComplexType");
            ExceptionUtilities.Assert(complexType != null, "complexType != null");

            return model;
        }

        // TODO: Move these CLR classes (with the DataClasses in the OData.Query suite) into a shared location
        private class TestEntityType
        {
            public int Id { get; set; }
        }

        private class TestComplexType
        {

        }

        private class SettingsActionTestCase
        {
            public Action<TestMessage, ODataMessageReaderSettings> Action;
            public bool Response;
        }
    }
}
