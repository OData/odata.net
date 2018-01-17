//---------------------------------------------------------------------
// <copyright file="BatchReaderHttpStreamSimulationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.PayloadTransformation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class tests the behaviour of the batch reader when the stream doesn't always return all the bytes requested.
    /// </summary>
    [TestClass]
    public class BatchReaderHttpStreamSimulationTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadReaderTestDescriptor.Settings PayloadReaderSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings PayloadReaderResultSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        [TestMethod, TestCategory("Reader.Batch")]
        public void ReadBatchWithHttpSimulatingStream()
        {
            EdmModel model = new EdmModel();
            IEnumerable<PayloadTestDescriptor> batchRequestDescriptors =
                TestBatches.CreateBatchRequestTestDescriptors(this.RequestManager, model, /*withTypeNames*/ true);
            IEnumerable<PayloadReaderTestDescriptor> requestTestDescriptors =
                batchRequestDescriptors.Select(bd => (PayloadReaderTestDescriptor) new BatchMessageHttpSimulationTestDescriptor(this.PayloadReaderSettings)
                {
                    PayloadDescriptor = bd,
                    SkipTestConfiguration = tc => !tc.IsRequest || (tc.Format != ODataFormat.Json)
                });

            IEnumerable<PayloadTestDescriptor> batchResponseDescriptors =
                TestBatches.CreateBatchResponseTestDescriptors(this.RequestManager, model, /*withTypeNames*/ true);
            IEnumerable<PayloadReaderTestDescriptor> responseTestDescriptors =
                batchResponseDescriptors.Select(bd => (PayloadReaderTestDescriptor) new BatchMessageHttpSimulationTestDescriptor(this.PayloadReaderSettings)
                {
                    PayloadDescriptor = bd,
                    SkipTestConfiguration = tc => tc.IsRequest || (tc.Format != ODataFormat.Json)
                });

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = requestTestDescriptors.Concat(responseTestDescriptors);

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
               (testDescriptor, testConfiguration) =>
               {
                   testDescriptor.RunTest(testConfiguration);
               });
        }

        /// <summary>
        /// Test descriptor for testing that we fail if we don't create a message for each operation
        /// in a batch or changeset.
        /// </summary>
        private sealed class BatchMessageHttpSimulationTestDescriptor : PayloadReaderTestDescriptor
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="settings">Settings class to use.</param>
            public BatchMessageHttpSimulationTestDescriptor(Settings settings)
                : base(settings)
            {
            }

            /// <summary>
            /// Called to create the input message for the reader test.
            /// </summary>
            /// <param name="testConfiguration">The test configuration.</param>
            /// <returns>The newly created test message to use.</returns>
            protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
            {
                bool originalApplyTransformValue = false;
                var odataTransformFactory = this.settings.PayloadTransformFactory as ODataLibPayloadTransformFactory;

                try
                {
                    if (this.ApplyPayloadTransformations.HasValue && odataTransformFactory != null)
                    {
                        originalApplyTransformValue = odataTransformFactory.ApplyTransform;
                        odataTransformFactory.ApplyTransform = this.ApplyPayloadTransformations.Value;
                    }

                    MemoryStream memoryStream = new MemoryStream(TestReaderUtils.GetPayload(testConfiguration,this.PayloadNormalizers, this.settings, this.PayloadElement));
                    TestStream messageStream = new BatchReaderTestStream(memoryStream);
                    if (testConfiguration.Synchronous)
                    {
                        messageStream.FailAsynchronousCalls = true;
                    }
                    else
                    {
                        messageStream.FailSynchronousCalls = true;
                    }

                    TestMessage testMessage = TestReaderUtils.CreateInputMessageFromStream(
                        messageStream,
                        testConfiguration,
                        this.PayloadElement.GetPayloadKindFromPayloadElement(),
                        this.PayloadElement.GetCustomContentTypeHeader(),
                        this.UrlResolver);

                    return testMessage;
                }
                finally
                {
                    if (this.ApplyPayloadTransformations.HasValue && odataTransformFactory != null)
                    {
                        odataTransformFactory.ApplyTransform = originalApplyTransformValue;
                    }
                }
            }
        }
    }
}
