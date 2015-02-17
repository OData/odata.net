//---------------------------------------------------------------------
// <copyright file="AtomGeneratorMetadataTests.cs" company="Microsoft">
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
    /// Tests to verify writer correctly works with ATOM generator metadata
    /// </summary>
    [TestClass, TestCase]
    public class AtomGeneratorMetadataTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Tests that generator metadata is correctly written.")]
        public void GeneratorMetadataWriterTest()
        {
            string testName = "Test generator";
            string testUri = "http://odata.org/generator";
            string testVersion = "3.0";

            var testCases = new[]
                {
                    new 
                    {
                        Generator = new AtomGeneratorMetadata()
                        {
                            Name = testName,
                            Uri = new Uri(testUri),
                            Version = testVersion
                        },
                        Xml = @"<generator uri=""" + testUri + @""" version=""" + testVersion + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testName + @"</generator>",
                        ExpectedException = (string)null
                    },
                    new 
                    {
                        Generator = new AtomGeneratorMetadata()
                        {
                            Name = testName,
                            Uri = null,
                            Version = null
                        },
                        Xml = @"<generator xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testName + @"</generator>",
                        ExpectedException = (string)null
                    },
                    new 
                    {
                        Generator = new AtomGeneratorMetadata()
                        {
                            Name = string.Empty,
                            Uri = null,
                            Version = null
                        },
                        Xml = @"<generator xmlns=""" + TestAtomConstants.AtomNamespace + @"""></generator>",
                        ExpectedException = (string)null
                    },
                    new 
                    {
                        Generator = new AtomGeneratorMetadata()
                        {
                            Name = null,
                            Uri = null,
                            Version = null
                        },
                        Xml = @"<generator xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                        ExpectedException = (string)null
                    },
                    new 
                    {
                        Generator = new AtomGeneratorMetadata()
                        {
                            Name = "\n\t ",
                            Uri = null,
                            Version = null
                        },
                        Xml = @"<generator xml:space=""preserve"" xmlns=""" + TestAtomConstants.AtomNamespace + "\">\n\t </generator>",
                        ExpectedException = (string)null
                    },
                };

            Func<XElement, XElement> fragmentExtractor = (e) => e.Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomGeneratorElementName);

            // Convert test cases to test descriptions
            var testDescriptors = testCases.Select(testCase =>
            {
                ODataFeed feed = ObjectModelUtils.CreateDefaultFeedWithAtomMetadata();
                AtomFeedMetadata metadata = feed.Atom();
                this.Assert.IsNotNull(metadata, "Expected default feed metadata on a default feed.");
                metadata.Generator = testCase.Generator;
                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, feed, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = testCase.Xml, ExpectedODataExceptionMessage = testCase.ExpectedException, FragmentExtractor = fragmentExtractor, PreserveWhitespace = true });
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.FeedPayloads),
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
