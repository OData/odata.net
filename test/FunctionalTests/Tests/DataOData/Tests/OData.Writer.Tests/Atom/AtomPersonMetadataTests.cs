//---------------------------------------------------------------------
// <copyright file="AtomPersonMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests to verify writer correctly works with ATOM person metadata
    /// </summary>
    [TestClass, TestCase]
    public class AtomPersonMetadataTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Tests that person metadata is correctly written.")]
        public void PersonMetadataWriterTest()
        {
            string testEmail = "author1@odata.org";
            string testName = "Test Author 1";
            string testUri = "http://odata.org/authors/1";

            var testCases = new[]
                {
                    new 
                    {
                        Person = new AtomPersonMetadata()
                        {
                            Name = testName,
                            Email = testEmail,
                            Uri = new Uri(testUri)
                        },
                        Xml = string.Join(
                            "$(NL)",
                            @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                            @"  <name>" + testName + @"</name>",
                            @"  <uri>" + testUri + @"</uri>",
                            @"  <email>" + testEmail + @"</email>",
                            @"</author>"),
                        ExpectedException = (string)null
                    },
                    new 
                    {
                        Person = new AtomPersonMetadata()
                        {
                            Name = null,
                            Email = testEmail,
                            Uri = new Uri(testUri)
                        },
                        Xml = string.Join(
                            "$(NL)",
                            @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                            @"  <name />",
                            @"  <uri>" + testUri + @"</uri>",
                            @"  <email>" + testEmail + @"</email>",
                            @"</author>"),
                        ExpectedException = (string)null
                    },
                    new 
                    {
                        Person = new AtomPersonMetadata()
                        {
                            Name = testName,
                            Email = null,
                            Uri = null
                        },
                        Xml = string.Join(
                            "$(NL)",
                            @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                            @"  <name>" + testName + @"</name>",
                            @"</author>"),
                        ExpectedException = (string)null
                    },
                    new 
                    {
                        Person = (AtomPersonMetadata)testName,
                        Xml = string.Join(
                            "$(NL)",
                            @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                            @"  <name>" + testName + @"</name>",
                            @"</author>"),
                        ExpectedException = (string)null
                    },
                };

            Func<XElement, XElement> fragmentExtractor = (e) => e.Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomAuthorElementName);

            // Convert test cases to test descriptions
            var testDescriptors = testCases.Select(testCase =>
            {
                ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                AtomEntryMetadata metadata = entry.Atom();
                this.Assert.IsNotNull(metadata, "Expected default entry metadata on a default entry.");
                metadata.Authors = new AtomPersonMetadata[] { testCase.Person };
                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = testCase.Xml, ExpectedODataExceptionMessage = testCase.ExpectedException, FragmentExtractor = fragmentExtractor });
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }
    }
}
