//---------------------------------------------------------------------
// <copyright file="AtomTextConstructMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    /// Tests to verify writer correctly works with different kinds of ATOM text constructs.
    /// </summary>
    [TestClass, TestCase]
    public class AtomTextConstructMetadataTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Tests that text construct metadata on ODataItem derived classes is correctly written.")]
        public void AtomTextConstructMetadataOnODataItemWriterTest()
        {
            var testDescriptors = this.CreateTextConstructTestDescriptors();

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases<AtomTextConstruct, ODataItem>(WriterPayloads.AtomTextConstructPayloadsForItem),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Tests that text construct metadata on workspace and collection is correctly written.")]
        public void AtomTextConstructMetadataOnODataWorkspaceWriterTest()
        {
            var testDescriptors = this.CreateTextConstructTestDescriptors();

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases<AtomTextConstruct, ODataServiceDocument>(WriterPayloads.AtomTextConstructPayloadsForWorkspace),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations.Where(tc =>!tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteServiceDocument(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        private IEnumerable<PayloadWriterTestDescriptor<AtomTextConstruct>> CreateTextConstructTestDescriptors()
        {
            AtomTextConstruct testTitle = new AtomTextConstruct { Text = null };
            AtomTextConstruct testTitle2 = string.Empty;
            AtomTextConstruct testTitle3 = "Test title";
            AtomTextConstruct testTitle4 = new AtomTextConstruct { Kind = AtomTextConstructKind.Text, Text = "Test title" };
            AtomTextConstruct testTitle5 = new AtomTextConstruct { Kind = AtomTextConstructKind.Html, Text = "Test title" };
            // NOTE: according to the spec '<' and '>' in Html payloads have to be escaped so the below would not be valid;
            //       we are not as strict so this works (and the string gets escaped)
            AtomTextConstruct testTitle6 = new AtomTextConstruct { Kind = AtomTextConstructKind.Html, Text = "<h2>Some headline</h2>" };
            AtomTextConstruct testTitle7 = new AtomTextConstruct { Kind = AtomTextConstructKind.Html, Text = "&lt;h2>Some headline&lt;/h2>" };
            // NOTE: according to the spec the text has to start with an <xhtml:div> element and thus this is not a valid
            //       XHtml payload; we are not as strict so this works fine.
            AtomTextConstruct testTitle8 = new AtomTextConstruct { Kind = AtomTextConstructKind.Xhtml, Text = "Test title" };
            AtomTextConstruct testTitle9 = new AtomTextConstruct { Kind = AtomTextConstructKind.Xhtml, Text = @"<div xmlns=""http://www.w3.org/1999/xhtml""><h2>Some headline</h2></div>" };

            var testCases = new[] {
                new { // null text
                    TextConstruct = testTitle,
                    Xml = @"<TextConstructElement type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @"""></TextConstructElement>",
                },
                new { // empty text
                    TextConstruct = testTitle2,
                    Xml = @"<TextConstructElement type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @"""></TextConstructElement>",
                },
                new { // non-empty text (kind = text plain, implicit)
                    TextConstruct = testTitle3,
                    Xml = @"<TextConstructElement type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testTitle3.Text + @"</TextConstructElement>",
                },
                new { // non-empty text (kind = text plain, explicit)
                    TextConstruct = testTitle4,
                    Xml = @"<TextConstructElement type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testTitle4.Text + @"</TextConstructElement>",
                },
                new { // non-empty text (kind = Html)
                    TextConstruct = testTitle5,
                    Xml = @"<TextConstructElement type=""html"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testTitle5.Text + @"</TextConstructElement>",
                },
                new { // unescaped Html text (kind = Html)
                    TextConstruct = testTitle6,
                    Xml = @"<TextConstructElement type=""html"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">&lt;h2&gt;Some headline&lt;/h2&gt;</TextConstructElement>",
                },
                new { // escaped Html text (kind = Html)
                    TextConstruct = testTitle7,
                    Xml = @"<TextConstructElement type=""html"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">&amp;lt;h2&gt;Some headline&amp;lt;/h2&gt;</TextConstructElement>",
                },
                new { // plain text (kind = XHtml)
                    TextConstruct = testTitle8,
                    Xml = @"<TextConstructElement type=""xhtml"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testTitle8.Text + @"</TextConstructElement>",
                },
                new { // xhtml non-escaped text (kind = XHtml)
                    TextConstruct = testTitle9,
                    Xml = @"<TextConstructElement type=""xhtml"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testTitle9.Text + @"</TextConstructElement>",
                },
            };

            // Convert test cases to test descriptions
            return testCases.Select(testCase =>
            {
                return new PayloadWriterTestDescriptor<AtomTextConstruct>(this.Settings, testCase.TextConstruct, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = testCase.Xml, FragmentExtractor = (e) => e });
            });
        }
    }
}
