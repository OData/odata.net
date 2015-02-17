//---------------------------------------------------------------------
// <copyright file="MessageSizeLimitsReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PayloadTestDescriptor = Microsoft.Test.Taupo.OData.Common.PayloadTestDescriptor;
    #endregion Namespaces

    /// <summary>
    /// Tests the message size limits for various values and payloads.
    /// </summary>
    [TestCase, TestClass]
    public class MessageSizeLimitsReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public MetadataReaderTestDescriptor.Settings MetadataSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        [Ignore]
        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading property payloads.")]
        public void PropertyMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            ODataPayloadElement propertyPayload = PayloadBuilder.PrimitiveProperty("LongName", "This is a name with a lot of characters so that we can test some security limits.");

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = 328, ResponseSize = 328 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 162, ResponseSize = 162 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 40,
                    AtomSizes = new RequestResponseSizes { RequestSize = 328, ResponseSize = 328 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 162, ResponseSize = 162 },
                },
                // ATOM & JSON-L fails, Verbose JSON works
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 120,
                    AtomSizes = new RequestResponseSizes { RequestSize = 328, ResponseSize = 328 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 162, ResponseSize = 162 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, propertyPayload, testCases);

            // Another tests with a payload that is too large for the default max message size
            ODataPayloadElement propertyPayload2 = PayloadBuilder.PrimitiveProperty("LongName2", new string('a', 1024 * 1024 + 1));
            testCases = new MessageSizeLimitTestCase[]
            {
                // Default should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                    AtomSizes = new RequestResponseSizes { RequestSize = 1048839, ResponseSize = 1048839 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 1048658, ResponseSize = 1048658 },
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, propertyPayload2, testCases);
        }

        [Ignore]
        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading feed payloads.")]
        public void FeedMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var entityType = model.FindDeclaredType("TestModel.Person") as EdmEntityType;

            ODataPayloadElement payload = PayloadBuilder.EntitySet(new EntityInstance[]
                {
                    PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 1),
                    PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 2),
                }).WithTypeAnnotation(entityType);

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = 735, ResponseSize = 735 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 200, ResponseSize = 202 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 40,
                    AtomSizes = new RequestResponseSizes { RequestSize = 735, ResponseSize = 735 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 200, ResponseSize = 202 },
                },
                // ATOM & JSON-L fails, Verbose JSON works
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 150,
                    AtomSizes = new RequestResponseSizes { RequestSize = 735, ResponseSize = 735 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 200, ResponseSize = 202 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases);
        }

        [Ignore]
        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading entry payloads.")]
        public void EntryMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            ODataPayloadElement payload = PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 1);

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = 443, ResponseSize = 443 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 142, ResponseSize = 144 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 40,
                    AtomSizes = new RequestResponseSizes { RequestSize = 443, ResponseSize = 443 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 142, ResponseSize = 144 },
                },
                // Only JSON request works
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 65,
                    AtomSizes = new RequestResponseSizes { RequestSize = 443, ResponseSize = 443 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 142, ResponseSize = 144 },
                },
                // Only ATOM & JSON-L response should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 142,
                    AtomSizes = new RequestResponseSizes { RequestSize = 443, ResponseSize = 443 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 144 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases);
        }

        [Ignore]
        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading entity reference link payloads.")]
        public void EntityReferenceLinkMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            EdmEntityType cityType = model.FindDeclaredType("TestModel.CityType") as EdmEntityType;
            EdmEntitySet citySet = model.EntityContainersAcrossModels().Single().FindEntitySet("Cities") as EdmEntitySet;

            ODataPayloadElement payload = PayloadBuilder.DeferredLink("http://odata.org/erl").ExpectedNavigationProperty(citySet, cityType, "PoliceStation");

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = 237, ResponseSize = 237 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 99, ResponseSize = 99 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    AtomSizes = new RequestResponseSizes { RequestSize = 237, ResponseSize = 237 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 99, ResponseSize = 99 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases);
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading entity reference links payloads.")]
        public void EntityReferenceLinksMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            EdmEntityType cityType = model.FindDeclaredType("TestModel.CityType") as EdmEntityType;
            EdmEntitySet citySet = model.EntityContainersAcrossModels().Single().FindEntitySet("Cities") as EdmEntitySet;

            ODataPayloadElement payload = PayloadBuilder.LinkCollection()
                .Item(PayloadBuilder.DeferredLink("http://odata.org/erl1"))
                .Item(PayloadBuilder.DeferredLink("http://odata.org/erl2"))
                .Item(PayloadBuilder.DeferredLink("http://odata.org/erl3"))
                .ExpectedNavigationProperty(citySet, cityType, "CityHall");

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 337 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 216 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    AtomSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 337 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 216 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases, tc => tc.IsRequest);
        }

        [Ignore]
        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading collection payloads.")]
        public void CollectionMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var itemTypeAnnotationType = MetadataUtils.GetPrimitiveTypeReference(typeof(string)).Definition;
            var collectionTypeAnnotationType = MetadataUtils.GetPrimitiveTypeReference(typeof(string)).ToCollectionTypeReference().Definition;

            ODataPayloadElement payload = new PrimitiveCollection(
                PayloadBuilder.PrimitiveValue("Vienna").WithTypeAnnotation(itemTypeAnnotationType),
                PayloadBuilder.PrimitiveValue("Prague").WithTypeAnnotation(itemTypeAnnotationType),
                PayloadBuilder.PrimitiveValue("Redmond").WithTypeAnnotation(itemTypeAnnotationType))
                .WithTypeAnnotation(collectionTypeAnnotationType)
                .ExpectedCollectionItemType(itemTypeAnnotationType).CollectionName("PrimitiveCollection");

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = 411, ResponseSize = 411 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 123, ResponseSize = 123 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    AtomSizes = new RequestResponseSizes { RequestSize = 411, ResponseSize = 411 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 123, ResponseSize = 123 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases);
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading service document payloads.")]
        public void ServiceDocumentMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            ODataPayloadElement payload = PayloadBuilder.ServiceDocument().Workspace(
                PayloadBuilder.Workspace()
                    .ResourceCollection(null, "http://odata.org/FirstCollection"));

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 248 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 185 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    AtomSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 248 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 185 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases, tc => tc.IsRequest);
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading metadata document payloads.")]
        public void MetadataDocumentMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;
            model.Fixup();

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    RawSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 4096 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    RawSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 4096 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunMetadataMessageSizeLimitTests(model, testCases);
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading error payloads.")]
        public void ErrorMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            ODataPayloadElement payload = PayloadBuilder.Error("Some error code");

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    AtomSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 259 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 47 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    AtomSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 259 },
                    JsonLightSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 47 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases, tc => tc.IsRequest);
        }

        // These tests use different encodings on SL/phone and thus will behave differently on those platforms.
#if !SILVERLIGHT && !WINDOWS_PHONE
        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading primitive value payloads.")]
        public void PrimitiveValueMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            ODataPayloadElement payload = PayloadBuilder.PrimitiveValue("This is a long string representing a value that will hit the message size limit.");

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    RawSizes = new RequestResponseSizes { RequestSize = 80, ResponseSize = 80 }
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    RawSizes = new RequestResponseSizes { RequestSize = 80, ResponseSize = 80 }
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunRawMessageSizeLimitTests(model, payload, testCases);
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading binary value payloads.")]
        public void BinaryValueMessageSizeLimitReadTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            ODataPayloadElement payload = PayloadBuilder.PrimitiveValue(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 })
                .WithTypeAnnotation(EdmCoreModel.Instance.GetBinary(false))
                .ExpectedPrimitiveValueType(EdmCoreModel.Instance.GetBinary(false));

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    RawSizes = new RequestResponseSizes { RequestSize = 21, ResponseSize = 21 }
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    RawSizes = new RequestResponseSizes { RequestSize = 21, ResponseSize = 21 }
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunRawMessageSizeLimitTests(model, payload, testCases);
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading batch request payloads.")]
        public void BatchRequestMessageSizeLimitReadTest()
        {
            var model = new EdmModel();
            PayloadTestDescriptor batchRequestDescriptor;

            var transformScope = this.Settings.PayloadTransformFactory.EmptyScope();
            using (transformScope.Apply())
            {
                batchRequestDescriptor = TestBatches.CreateBatchRequestTestDescriptors(this.RequestManager, model, /*withTypeNames*/ true).Last();
            }

            ODataPayloadElement payload = batchRequestDescriptor.PayloadElement;
            model = (EdmModel)batchRequestDescriptor.PayloadEdmModel;

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    RawSizes = new RequestResponseSizes { RequestSize = 2942, ResponseSize = -1 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    RawSizes = new RequestResponseSizes { RequestSize = 2942, ResponseSize = -1 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunRawMessageSizeLimitTests(model, payload, testCases, tc => !tc.IsRequest || (tc.Format != ODataFormat.Atom && tc.Format != ODataFormat.Json));
        }

        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading batch response payloads.")]
        public void BatchResponseMessageSizeLimitReadTest()
        {
            var model = new EdmModel();
            PayloadTestDescriptor batchResponseDescriptor;

            var transformScope = this.Settings.PayloadTransformFactory.EmptyScope();
            using (transformScope.Apply())
            {
                batchResponseDescriptor = TestBatches.CreateBatchResponseTestDescriptors(this.RequestManager, model, /*withTypeNames*/ true).Last();
            }

            ODataPayloadElement payload = batchResponseDescriptor.PayloadElement;
            model = (EdmModel)batchResponseDescriptor.PayloadEdmModel;

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    RawSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 4515 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 20,
                    RawSizes = new RequestResponseSizes { RequestSize = -1, ResponseSize = 4515 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunRawMessageSizeLimitTests(model, payload, testCases, tc => tc.IsRequest || (tc.Format != ODataFormat.Json || tc.Format != ODataFormat.Atom));
        }
#endif

        [Ignore]
        [TestMethod, TestCategory("Reader.MessageReader"), Variation(Description = "Verify correct behavior of the max message size setting when reading parameter payloads.")]
        public void ParameterMessageSizeLimitReadTest()
        {
            EdmModel model = new EdmModel();
            ODataPayloadElement payload = TestParameters.CreateParameterValues(model, false /*fullSet*/).First();

            var testCases = new MessageSizeLimitTestCase[]
            {
                // Single byte size should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 1,
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 18, ResponseSize = -1 },
                },
                // Small number should fail
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10,
                    JsonLightSizes = new RequestResponseSizes { RequestSize = 18, ResponseSize = -1 },
                },
                // Large number should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = 10000,
                },
                // Default should work
                new MessageSizeLimitTestCase
                {
                    MaxMessageSize = -1,
                },
            };

            this.RunAtomJsonMessageSizeLimitTests(model, payload, testCases, tc => !tc.IsRequest || tc.Format == ODataFormat.Atom);
        }

        private void RunAtomJsonMessageSizeLimitTests(
            EdmModel model,
            ODataPayloadElement payload,
            MessageSizeLimitTestCase[] testCases,
            Func<ReaderTestConfiguration, bool> skipTestConfigurationFunc = null)
        {
            var testConfigurations = this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations;
            this.RunMessageSizeLimitTests(testConfigurations, model, payload, testCases, skipTestConfigurationFunc);
        }

        private void RunRawMessageSizeLimitTests(
            EdmModel model,
            ODataPayloadElement payload,
            MessageSizeLimitTestCase[] testCases,
            Func<ReaderTestConfiguration, bool> skipTestConfigurationFunc = null)
        {
            var testConfigurations = this.ReaderTestConfigurationProvider.DefaultFormatConfigurations;
            this.RunMessageSizeLimitTests(testConfigurations, model, payload, testCases, skipTestConfigurationFunc);
        }

        private void RunMessageSizeLimitTests(
            IEnumerable<ReaderTestConfiguration> testConfigurations,
            EdmModel model,
            ODataPayloadElement payload,
            MessageSizeLimitTestCase[] testCases,
            Func<ReaderTestConfiguration, bool> skipTestConfigurationFunc = null)
        {
            var transformScope = this.Settings.PayloadTransformFactory.EmptyScope();
            using (transformScope.Apply())
            {
                this.CombinatorialEngineProvider.RunCombinations(
                    testCases,
                    testConfigurations,
                    (testCase, testConfiguration) =>
                    {
                        int size = -1;
                        if (testConfiguration.Format == ODataFormat.Atom && testCase.AtomSizes != null)
                        {
                            size = testConfiguration.IsRequest ? testCase.AtomSizes.RequestSize : testCase.AtomSizes.ResponseSize;
                        }
                        else if (testConfiguration.Format == ODataFormat.Json && testCase.JsonLightSizes != null)
                        {
                            size = testConfiguration.IsRequest ? testCase.JsonLightSizes.RequestSize : testCase.JsonLightSizes.ResponseSize;
                        }
                        else if (testCase.RawSizes != null)
                        {
                            size = testConfiguration.IsRequest ? testCase.RawSizes.RequestSize : testCase.RawSizes.ResponseSize;
                        }

                        int maxSize = testCase.MaxMessageSize >= 0 ? testCase.MaxMessageSize : 1024 * 1024;
                        ExpectedException expectedException = size < 0
                            ? null
                            : ODataExpectedExceptions.ODataException("MessageStreamWrappingStream_ByteLimitExceeded", size.ToString(), maxSize.ToString());

                        var testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadEdmModel = model,
                            PayloadElement = payload.DeepCopy(),
                            ExpectedException = expectedException,
                            SkipTestConfiguration = skipTestConfigurationFunc,
                            ApplyPayloadTransformations = false,
                        };

                        testDescriptor.ExpectedResultNormalizers.Add(
                            tc => (Func<ODataPayloadElement, ODataPayloadElement>)null);

                        if (testCase.MaxMessageSize > 0)
                        {
                            testConfiguration = new ReaderTestConfiguration(testConfiguration);
                            testConfiguration.MessageReaderSettings.MessageQuotas.MaxReceivedMessageSize = testCase.MaxMessageSize;
                        }

                        testDescriptor.RunTest(testConfiguration);
                    });
            }
        }

        private void RunMetadataMessageSizeLimitTests(IEdmModel model, MessageSizeLimitTestCase[] testCases)
        {
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations.Where(tc => !tc.IsRequest && tc.Synchronous),
                (testCase, testConfiguration) =>
                {
                    int size = -1;
                    if (testConfiguration.Format == ODataFormat.Atom && testCase.AtomSizes != null)
                    {
                        size = testConfiguration.IsRequest ? testCase.AtomSizes.RequestSize : testCase.AtomSizes.ResponseSize;
                    }
                    else if (testConfiguration.Format == ODataFormat.Json && testCase.JsonLightSizes != null)
                    {
                        size = testConfiguration.IsRequest ? testCase.JsonLightSizes.RequestSize : testCase.JsonLightSizes.ResponseSize;
                    }
                    else if (testCase.RawSizes != null)
                    {
                        size = testConfiguration.IsRequest ? testCase.RawSizes.RequestSize : testCase.RawSizes.ResponseSize;
                    }

                    ExpectedException expectedException = size < 0
                        ? null
                        : ODataExpectedExceptions.ODataException("MessageStreamWrappingStream_ByteLimitExceeded", size.ToString(), testCase.MaxMessageSize.ToString());

                    var testDescriptor = new MetadataReaderTestDescriptor(this.MetadataSettings)
                    {
                        PayloadEdmModel = model,
                        ExpectedException = expectedException,
                    };

                    if (testCase.MaxMessageSize > 0)
                    {
                        testConfiguration = new ReaderTestConfiguration(testConfiguration);
                        testConfiguration.MessageReaderSettings.MessageQuotas.MaxReceivedMessageSize = testCase.MaxMessageSize;
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class MessageSizeLimitTestCase
        {
            public int MaxMessageSize { get; set; }
            public RequestResponseSizes AtomSizes { get; set; }
            public RequestResponseSizes JsonLightSizes { get; set; }
            public RequestResponseSizes RawSizes { get; set; }
        }

        private sealed class RequestResponseSizes
        {
            public int RequestSize { get; set; }
            public int ResponseSize { get; set; }
        }
    }
}
