//---------------------------------------------------------------------
// <copyright file="ServiceDocumentReaderTests.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various service document payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ServiceDocumentReaderTests : ODataReaderTestCase
    {
        private const string baseUri = "http://odata.org/";

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings PayloadExpectedResultSettings { get; set; }

        [TestMethod, TestCategory("Reader.ServiceDocument"), Variation(Description = "Test the the reading of service document payloads.")]
        public void ServiceDocumentReaderTest()
        {
            // NOTE: not using the payload generator here since the service documents can only appear at the top level
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                PayloadReaderTestDescriptorGenerator.CreateServiceDocumentDescriptors(this.Settings, baseUri, withTitles: false);

            // Add some hand-crafted payloads
            IEnumerable<PayloadReaderTestDescriptor> manualDescriptors = new PayloadReaderTestDescriptor[]
            {
                // service doc in request should fail
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "collection")),
                    SkipTestConfiguration = tc => !tc.IsRequest,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataMessageReader_ServiceDocumentInRequest"),
                },

                // null title should work
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "collection")),
                    SkipTestConfiguration = tc => tc.IsRequest,
                },

                // null Href should fail
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, null)),
                    SkipTestConfiguration = tc => tc.IsRequest,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_ServiceDocumentElementUrlMustNotBeNull"),
                },

                // multiple collections with the same name/href should work
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection(null, baseUri + "collection")
                            .ResourceCollection(null, baseUri + "collection")),
                    SkipTestConfiguration = tc => tc.IsRequest,
                },

                // relative URI without a base URI should fail in Atom and JSON Light, but is allowed in Verbose JSON.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, "collection")),
                    SkipTestConfiguration = tc => tc.IsRequest,
                    ExpectedResultCallback = tc =>
                        {
                            if (tc.Format == ODataFormat.Json)
                            {
                                return new PayloadReaderTestExpectedResult(this.PayloadExpectedResultSettings)
                                {
                                    ExpectedException = null
                                };
                            }
                            throw new NotImplementedException();
                        }
                },
            };

            testDescriptors = testDescriptors.Concat(manualDescriptors);

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);

                        // Json light requires a model
                        testDescriptor.PayloadEdmModel = new EdmModel();

                        // Add an empty expected type annotation to cause metadata link to be generated in test serializer.
                        testDescriptor.PayloadElement.AddExpectedTypeAnnotation();

                        // Json light resource collections require the "Name" property, but it won't round-trip for verbose json, so add Name here.
                        foreach (var workspace in ((ServiceDocumentInstance)testDescriptor.PayloadElement).Workspaces)
                        {
                            int count = 0;
                            foreach (var resourceCollection in workspace.ResourceCollections)
                            {
                                resourceCollection.Name = "Entity Set Name " + count;
                                count++;
                            }
                        }
                    }

                    ReaderTestConfiguration testConfigClone = new ReaderTestConfiguration(testConfiguration);
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testConfigClone.MessageReaderSettings.BaseUri = null;
                    }

                    testDescriptor.RunTest(testConfigClone);
                });
        }
    }
}
