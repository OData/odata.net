//---------------------------------------------------------------------
// <copyright file="WriterMetadataDocumentAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing ATOM metadata with the ODatamessage writer.
    /// </summary>
    [TestClass, TestCase]
    public class WriterMetadataDocumentAtomTests : ODataWriterTestCase
    {
        [InjectDependency]
        public MetadataWriterTestDescriptor.Settings Settings { get; set; }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test the writing of ATOM metadata document payloads.")]
        public void MetadataDocumentWriterAtomTest()
        {
            MetadataWriterTestDescriptor testDescriptor =
                new MetadataWriterTestDescriptor(this.Settings)
                {
                    Model = new EdmModel().Fixup(),
                    ContentType = "application/unsupported",
                    ExpectedODataExceptionMessage = "A default MIME type could not be found for the requested payload in format 'Atom'.",
                };

            this.CombinatorialEngineProvider.RunCombinations(
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent
                    .Where(tc => tc.Synchronous && !tc.IsRequest),
                (testConfiguration) => testDescriptor.RunTest(testConfiguration, this.Logger));
        }
    }
}
