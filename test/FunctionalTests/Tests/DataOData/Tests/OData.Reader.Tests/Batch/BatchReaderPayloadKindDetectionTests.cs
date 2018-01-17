//---------------------------------------------------------------------
// <copyright file="BatchReaderPayloadKindDetectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various batch payloads in both success and error scenarios.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderPayloadKindDetectionTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadKindDetectionTestDescriptor.Settings PayloadKindDetectionSettings { get; set; }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Test the payload kind detection of batch payloads.")]
        public void BatchReaderPayloadKindDetectionAtomTest()
        {
            // TODO: once we can register custom formats for content type multipart/mixed add more tests here
            //       since currently we will never hit the actual payload kind detection code.

            // Test cases
            PayloadKindDetectionResult batchResult = new PayloadKindDetectionResult(ODataPayloadKind.Batch, ODataFormat.Batch);
            Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> batchDetectionResult = 
                testConfig => new PayloadKindDetectionResult[] { batchResult };

            // NOTE: we currently only use the content type header to determine whether something is a batch payload or not.
            //       We require a 'multipart/mixed' content type and a 'boundary' parameter
            var testDescriptors = new PayloadKindDetectionTestDescriptor[]
            {
                // Correct content type header
                new PayloadKindDetectionTestDescriptor(this.PayloadKindDetectionSettings)
                {
                    ContentType = "multipart/mixed;boundary=b",
                    PayloadString = "Does not matter",
                    ExpectedDetectionResults = batchDetectionResult,
                },
                // Correct content type header with additional parameters after boundary
                new PayloadKindDetectionTestDescriptor(this.PayloadKindDetectionSettings)
                {
                    ContentType = "multipart/mixed;boundary=b;a=c;d=e",
                    PayloadString = "Does not matter",
                    ExpectedDetectionResults = batchDetectionResult,
                },
                // Correct content type header with additional parameters before boundary
                new PayloadKindDetectionTestDescriptor(this.PayloadKindDetectionSettings)
                {
                    ContentType = "multipart/mixed;a=c;d=e;boundary=b",
                    PayloadString = "Does not matter",
                    ExpectedDetectionResults = batchDetectionResult,
                },

                // Unsupported content type
                new PayloadKindDetectionTestDescriptor(this.PayloadKindDetectionSettings)
                {
                    ContentType = "multipart/invalid",
                    PayloadString = "Does not matter",
                    ExpectedDetectionResults = testConfig => Enumerable.Empty<PayloadKindDetectionResult>(),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration));
        }
    }
}
