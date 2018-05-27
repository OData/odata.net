//---------------------------------------------------------------------
// <copyright file="MetadataDocumentReaderErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various metadata document payloads that should result in an error.
    /// </summary>
    [TestClass, TestCase]
    public class MetadataDocumentReaderErrorTests : ODataReaderTestCase
    {
        [InjectDependency]
        public MetadataReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.MetadataDocument"), Variation(Description = "Test the reading of metadata document payloads with errors.")]
        public void MetadataDocumentReaderErrorTest()
        {
            IEnumerable<MetadataReaderErrorTestCase> testCases = new MetadataReaderErrorTestCase[]
            {
                new MetadataReaderErrorTestCase
                {
                    TestDescriptor = new MetadataReaderTestDescriptor(this.Settings)
                    {
                        PayloadEdmModel = new EdmModel().Fixup(),
                        ExpectedException = ODataExpectedExceptions.TestException(),
                    },
                    RunInAsync = true,
                },
                new MetadataReaderErrorTestCase
                {
                    TestDescriptor = new MetadataReaderTestDescriptor(this.Settings)
                    {
                        PayloadEdmModel = new EdmModel().Fixup(),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataMessageReader_MetadataDocumentInRequest"),
                    },
                    RunInRequest = true,
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    if (testCase.RunInAsync == testConfiguration.Synchronous)
                    {
                        return;
                    }

                    if (testCase.RunInRequest != testConfiguration.IsRequest)
                    {
                        return;
                    }

                    testCase.TestDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.MetadataDocument"), Variation(Description = "Test the reading of metadata document payloads with incorrect content type.")]
        public void MetadataDocumentReaderContentTypeErrorTest()
        {
            MetadataReaderTestDescriptor[] errorDescriptors = new MetadataReaderTestDescriptor[]
            {
                new MetadataReaderTestDescriptor(this.Settings)
                {
                    PayloadEdmModel = new EdmModel().Fixup(),
                    ContentType = "application/unsupported",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("MediaTypeUtils_CannotDetermineFormatFromContentType", "application/xml", "application/unsupported"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                errorDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.Synchronous && !tc.IsRequest),
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration));
        }

        // Metadata serialization: add tests for reading/writing in-stream errors
        private sealed class MetadataReaderErrorTestCase
        {
            public MetadataReaderTestDescriptor TestDescriptor { get; set; }
            public bool RunInAsync { get; set; }
            public bool RunInRequest { get; set; }
        }

    }
}
