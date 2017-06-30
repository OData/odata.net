//---------------------------------------------------------------------
// <copyright file="BatchReaderCrossReferenceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of batch payloads containing references to requests in the same batch.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderCrossReferenceTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadReaderTestDescriptor.Settings PayloadReaderSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        // NOTE: Testing of URL cross reference resolver in combination with custom URL resolver from the batch message is not implemented here
        //   but existing WCF DS tests will cover that since WCF DS uses custom URL resolver always.

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Verify reading batch requests with references in the changeset operation headers.")]
        public void BatchReaderCrossReferenceLinksInHeaderTest()
        {
            var testCases = new[]
            {
                new CrossReferenceTestCase
                {
                    DebugDescription = "Valid cross-reference link in request header (reference only).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "1", },
                                new CrossReferenceOperation { Uri = new Uri("$1", UriKind.Relative), ContentId = "2", },
                            }
                        },
                    },
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Valid cross-reference link in request header (segment after reference).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "1", },
                                new CrossReferenceOperation { Uri = new Uri("$1/foo", UriKind.Relative), ContentId = "2", },
                            }
                        },
                    },
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Valid cross-reference link in request header (special name '').",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "", },
                                new CrossReferenceOperation { Uri = new Uri("$/foo", UriKind.Relative), ContentId = "2", },
                            }
                        },
                    },
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Valid cross-reference link in request header (special name '$$$').",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "$$$", },
                                new CrossReferenceOperation { Uri = new Uri("$$$$/foo", UriKind.Relative), ContentId = "2", },
                            }
                        },
                    },
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Invalid cross-reference link in request header (content ID does not exist).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "1", },
                                new CrossReferenceOperation { Uri = new Uri("$DoesNotExist/foo", UriKind.Relative), ContentId = "2", },
                            }
                        },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$DoesNotExist/foo"),
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Invalid cross-reference link in request header (content ID will only appear later in the batch).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("$2/foo", UriKind.Relative), ContentId = "1", },
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "2", },
                            }
                        },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$2/foo"),
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Invalid payload (duplicate content ID, not in the last operation of the changeset).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "Duplicate", },
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/2"), ContentId = "Duplicate", },
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/3"), ContentId = "3", },
                            }
                        },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_DuplicateContentIDsNotAllowed", "Duplicate"),
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Invalid payload (duplicate content ID in last operation of the changeset).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "Duplicate", },
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/2"), ContentId = "Duplicate", },
                            }
                        },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_DuplicateContentIDsNotAllowed", "Duplicate"),
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Invalid cross-reference link in request header (reference content ID in different changeset).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "1", },
                            }
                        },
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("$1", UriKind.Relative), ContentId = "2", },
                            }
                        },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$1"),
                },
                new CrossReferenceTestCase
                {
                    DebugDescription = "Invalid cross-reference link in request header (reference to current content ID).",
                    ChangeSets = new[]
                    {
                        new CrossReferenceChangeSet 
                        { 
                            Operations = new [] 
                            {
                                new CrossReferenceOperation { Uri = new Uri("http://foo.com/1"), ContentId = "1", },
                                new CrossReferenceOperation { Uri = new Uri("$2", UriKind.Relative), ContentId = "2", },
                            }
                        },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$2"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    this.CombinatorialEngineProvider.RunCombinations(
                        CreateCrossReferenceTestDescriptors(testCase, testConfiguration),
                        testDescriptor => testDescriptor.RunTest(testConfiguration));
                });
        }

        private IEnumerable<PayloadReaderTestDescriptor> CreateCrossReferenceTestDescriptors(CrossReferenceTestCase testCase, ReaderTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(testCase, "testCase");

            var emptyPayload = new OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = new EdmModel().Fixup()
            };

            IEnumerable<OData.Common.PayloadTestDescriptor> operationPayloads = new[] { emptyPayload };

            // One of the operations in the test case may specify a reference link value to use to generate payloads
            string payloadReferenceLink = testCase.ChangeSets.SelectMany(cset => cset.Operations).Select(o => o.PayloadCrossReferenceLink).SingleOrDefault(s => !string.IsNullOrEmpty(s));
            if (payloadReferenceLink != null)
            {
                EdmModel testModel = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
                operationPayloads =
                    GeneratePayloadElementsWithCrossReferenceLinks(payloadReferenceLink, testConfiguration).Select(
                        p => new OData.Common.PayloadTestDescriptor
                        {
                            PayloadElement = p,
                            PayloadEdmModel = testModel,
                        });
            }

            var testDescriptors = new List<PayloadReaderTestDescriptor>();
            foreach (var payload in operationPayloads)
            {
                IEnumerable<IMimePart> requestChangesets = testCase.ChangeSets.Select(
                    c => (IMimePart)BatchUtils.GetRequestChangeset(
                        c.Operations.Select(o =>
                            {
                                // check whether we need to inject a payload into this operation
                                var operationPayload = string.IsNullOrEmpty(o.PayloadCrossReferenceLink) ? emptyPayload : payload;

                                ODataUri operationUri = new ODataUri(new[] { ODataUriBuilder.Unrecognized(o.Uri.OriginalString) });
                                var requestOperation = operationPayload.InRequestOperation(HttpVerb.Post, operationUri, this.RequestManager);
                                requestOperation.Headers.Add(HttpHeaders.ContentId, o.ContentId);

                                return (IMimePart)requestOperation;
                            }).ToArray(),
                        this.RequestManager));

                var testDescriptor = new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = testCase.DebugDescription,
                    PayloadElement = PayloadBuilder.BatchRequestPayload(requestChangesets.ToArray()).AddAnnotation(new BatchBoundaryAnnotation("batch_foo")),
                    ExpectedException = testCase.ExpectedException,
                    SkipTestConfiguration = (testConfig) => !testConfig.IsRequest,
                };

                testDescriptors.Add(testDescriptor);
            }

            return testDescriptors;
        }

        /// <summary>
        /// Generates various payloads with different link properties set to the specified cross reference value.
        /// </summary>
        /// <param name="crossReferenceLink">The cross reference link values to set on each payload.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The payloads.</returns>
        private static IEnumerable<ODataPayloadElement> GeneratePayloadElementsWithCrossReferenceLinks(string crossReferenceLink, ReaderTestConfiguration testConfiguration)
        {
            List<ODataPayloadElement> payloadElements = new List<ODataPayloadElement>();

            // Entry with edit link
            payloadElements.Add(PayloadBuilder.Entity("TestModel.Address").PrimitiveProperty("Zip", 12345).WithEditLink(crossReferenceLink));

            // Entry with self link
            payloadElements.Add(PayloadBuilder.Entity("TestModel.Address").PrimitiveProperty("Zip", 98765).WithSelfLink(crossReferenceLink));

            // Entry with deferred navprop
            payloadElements.Add(PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("CityHall", PayloadBuilder.DeferredLink(crossReferenceLink)));

            // Entry with expanded navprop
            payloadElements.Add(PayloadBuilder.Entity("TestModel.CityType").ExpandedNavigationProperty("PoliceStation", PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 0), crossReferenceLink));

            if (!testConfiguration.IsRequest)
            {
                // Entry with deferred navprop with association link
                payloadElements.Add(PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("DOL", PayloadBuilder.DeferredLink("http://odata.org"), PayloadBuilder.DeferredLink(crossReferenceLink)));

                // Entry with expanded navprop with association link
                payloadElements.Add(PayloadBuilder.Entity("TestModel.CityType").NavigationProperty(
                    PayloadBuilder.ExpandedNavigationProperty("PoliceStation", PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 0), "http://odata.org", crossReferenceLink)));

                // Entry with stream property with read link
                payloadElements.Add(PayloadBuilder.Entity("TestModel.CityType").StreamProperty("Skyline", crossReferenceLink));

                // Entry with stream property with edit link
                payloadElements.Add(PayloadBuilder.Entity("TestModel.CityType").StreamProperty("Skyline", null, crossReferenceLink));

                // Feed with multiple entries referencing same link
                payloadElements.Add(PayloadBuilder.EntitySet(
                    new[]
                {
                    PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("CityHall", PayloadBuilder.DeferredLink(crossReferenceLink)),
                    PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("CityHall", PayloadBuilder.DeferredLink(crossReferenceLink)),
                }));
            }

            return payloadElements;
        }

        private class CrossReferenceChangeSet
        {
            internal IEnumerable<CrossReferenceOperation> Operations { get; set; }
        }

        private class CrossReferenceOperation
        {
            internal Uri Uri { get; set; }
            internal string ContentId { get; set; }
            internal string PayloadCrossReferenceLink { get; set; }
        }

        private class CrossReferenceTestCase
        {
            internal string DebugDescription { get; set; }
            internal IEnumerable<CrossReferenceChangeSet> ChangeSets { get; set; }
            internal ExpectedException ExpectedException { get; set; }
        }
    }
}
