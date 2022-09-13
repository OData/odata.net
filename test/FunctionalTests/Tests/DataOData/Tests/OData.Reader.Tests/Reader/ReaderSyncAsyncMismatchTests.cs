//---------------------------------------------------------------------
// <copyright file="ReaderSyncAsyncMismatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCase]
    public class ReaderSyncAsyncMismatchTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Errors"), Variation(Description = "Verifies that sync and async calls cannot be mixed on a single reader.")]
        public void SyncAsyncMismatchTest()
        {
            var model = new EdmModel();
            var payloadDescriptors = Test.OData.Utils.ODataLibTest.TestFeeds.GetFeeds(model, true /*withTypeNames*/);
            var testDescriptors = this.PayloadDescriptorsToReaderDescriptors(payloadDescriptors);
            
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors, 
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations, 
                (testDescriptor, testConfiguration) =>
                {
                    var message = TestReaderUtils.CreateInputMessage(testConfiguration, testDescriptor, this.Settings, false);
                    message.IgnoreSynchronousError = true;
                    message.TestStream.IgnoreSynchronousError = true;

                    Exception exception = TestExceptionUtils.RunCatching(() =>
                    {
                        using (ODataMessageReaderTestWrapper messageReaderWrapper = TestReaderUtils.CreateMessageReader(message, model, testConfiguration))
                        {
                            var feedReader = messageReaderWrapper.MessageReader.CreateODataResourceSetReader(model.EntityContainer.FindEntitySet("MyBaseType"), model.EntityTypes().FirstOrDefault());
                            if (testConfiguration.Synchronous)
                            {
                                feedReader.Read();
                                feedReader.ReadAsync();
                            }
                            else
                            {
                                feedReader.ReadAsync();
                                feedReader.Read();
                            }
                        }
                    });
                    
                    var expected = ODataExpectedExceptions.ODataException("ODataReaderCore_AsyncCallOnSyncReader");
                    ExceptionVerifier.VerifyExceptionResult(expected, exception);
                });
        }

        private List<PayloadReaderTestDescriptor> PayloadDescriptorsToReaderDescriptors(IEnumerable<PayloadTestDescriptor> payloadDescriptors)
        {
            var testDescriptors = new List<PayloadReaderTestDescriptor>();

            foreach (var payloadDescriptor in payloadDescriptors)
            {
                var payload = payloadDescriptor.PayloadElement.DeepCopy();
                var newDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                                    {
                                        PayloadDescriptor = payloadDescriptor,
                                        PayloadElement = payload,
                                        PayloadEdmModel = payloadDescriptor.PayloadEdmModel,
                                        SkipTestConfiguration = payloadDescriptor.SkipTestConfiguration,
                                    };
                testDescriptors.Add(newDescriptor);
            }

            return testDescriptors;
        }
    }
}
