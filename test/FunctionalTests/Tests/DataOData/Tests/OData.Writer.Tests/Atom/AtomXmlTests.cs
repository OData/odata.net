//---------------------------------------------------------------------
// <copyright file="AtomXmlTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests to verify writer correctly writes out all the XML necessities for ATOM
    /// </summary>
    [TestClass, TestCase]
    public class AtomXmlTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public CollectionWriterTestDescriptor.Settings CollectionSettings { get; set; }

        [TestMethod, Variation(Description = "Tests that root elements have the right namespaces. Uses custom data namespace.")]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RootElementNamespaces_CustomDataNamespace()
        {
            // Support custom data namespace for compatibility with WCF DS client.
            RootElementNamespacesTest(TestAtomConstants.ODataNamespace + "/custom", true);
        }

        [TestMethod, Variation(Description = "Tests that root elements have the right namespaces.")]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RootElementNamespaces()
        {
            RootElementNamespacesTest(TestAtomConstants.ODataNamespace, false);
        }

        public void RootElementNamespacesTest(string odataNamespace, bool clientOnly)
        {
            var testCases = new Func<string, Action<WriterTestConfiguration>>[] {
                odataNs => this.WriteFeed(
                    ExtractRootElementAndNamespaceDeclarations,
                    version => "<feed xmlns='" + TestAtomConstants.AtomNamespace + 
                        "' xmlns:" + TestAtomConstants.ODataNamespacePrefix + "='" + odataNs + 
                        "' xmlns:" + TestAtomConstants.ODataMetadataNamespacePrefix + "='" + TestAtomConstants.ODataMetadataNamespace + 
                        "' xmlns:" + TestAtomConstants.GeoRssPrefix + "='" + TestAtomConstants.GeoRssNamespace +
                              "' xmlns:" + TestAtomConstants.GmlPrefix + "='" + TestAtomConstants.GmlNamespace + 
                        "' />"),
                odataNs => this.WriteEntry(
                    ExtractRootElementAndNamespaceDeclarations,
                    version => "<entry xmlns='" + TestAtomConstants.AtomNamespace + 
                        "' xmlns:" + TestAtomConstants.ODataNamespacePrefix + "='" + odataNs + 
                        "' xmlns:" + TestAtomConstants.ODataMetadataNamespacePrefix + "='" + TestAtomConstants.ODataMetadataNamespace +
                        "' xmlns:" + TestAtomConstants.GeoRssPrefix + "='" + TestAtomConstants.GeoRssNamespace +
                        "' xmlns:" + TestAtomConstants.GmlPrefix + "='" + TestAtomConstants.GmlNamespace + 
                        "' />"),
                odataNs => this.WriteCollection(
                    ExtractRootElementAndNamespaceDeclarations,
                    version => "<" + TestAtomConstants.ODataMetadataNamespacePrefix + ":" + TestAtomConstants.ODataValueElementName +  
                        " xmlns:" + TestAtomConstants.ODataNamespacePrefix + "='"+ odataNs +
                        "' xmlns:" + TestAtomConstants.GeoRssPrefix + "='" + TestAtomConstants.GeoRssNamespace +
                        "' xmlns:" + TestAtomConstants.GmlPrefix + "='" + TestAtomConstants.GmlNamespace + 
                        "' xmlns:" + TestAtomConstants.ODataMetadataNamespacePrefix + "='" + TestAtomConstants.ODataMetadataNamespace +
                        "' />"),
                odataNs => this.WriteServiceDocument(
                    ExtractRootElementAndNamespaceDeclarations,
                    "<service xmlns='" + TestAtomConstants.AtomPublishingNamespace + "' xmlns:atom='" + TestAtomConstants.AtomNamespace + "' />"),
                odataNs => this.WriteProperty(
                    ExtractRootElementAndNamespaceDeclarations,
                    version => "<" + TestAtomConstants.ODataMetadataNamespacePrefix + ":" + TestAtomConstants.ODataValueElementName +  
                        "  xmlns:" + TestAtomConstants.ODataNamespacePrefix + "='" + odataNs + 
                        "' xmlns:" + TestAtomConstants.GeoRssPrefix + "='" + TestAtomConstants.GeoRssNamespace +
                        "' xmlns:" + TestAtomConstants.GmlPrefix + "='" + TestAtomConstants.GmlNamespace + 
                        "' xmlns:" + TestAtomConstants.ODataMetadataNamespacePrefix + "='" + TestAtomConstants.ODataMetadataNamespace +
                        "' />"),
                odataNs => this.WriteError(
                    ExtractRootElementAndNamespaceDeclarations,
                    "<m:error xmlns:" + TestAtomConstants.ODataMetadataNamespacePrefix + "='" + TestAtomConstants.ODataMetadataNamespace + "' />"),
                odataNs => this.WriteEntityReferenceLinks(
                    ExtractRootElementAndNamespaceDeclarations,
                    "<ref xmlns='" + odataNs + "'/>"),
                odataNs => this.WriteEntityReferenceLink(
                    ExtractRootElementAndNamespaceDeclarations,
                    "<uri xmlns='" + odataNs + "'/>",
                    clientOnly,
                    odataNs),
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    string expectedODataNamespace;
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    if (clientOnly)
                    {
                        testConfiguration.MessageWriterSettings.EnableWcfDataServicesClientBehavior();
                        expectedODataNamespace = TestAtomConstants.ODataNamespace;
                    }
                    else
                    {
                        expectedODataNamespace = TestAtomConstants.ODataNamespace;
                    }

                    testCase(expectedODataNamespace)(testConfiguration);
                });
        }

        [TestMethod, Variation(Description = "Tests that root elements have the right namespaces.")]
        public void XmlBaseOnRootElement()
        {
            Uri baseUri = new Uri("http://odata.org/base/");
            string expectedXmlBase = "<root xml:base='http://odata.org/base/'/>";
            string noXmlBase = "<root/>";

            var testCases = new Action<WriterTestConfiguration>[] {
                this.WriteFeed(
                    ExtractXmlBase,
                    version => expectedXmlBase),
                this.WriteEntry(
                    ExtractXmlBase,
                    version => expectedXmlBase),
                this.WriteCollection(
                    ExtractXmlBase,
                    version => noXmlBase),
                this.WriteServiceDocument(
                    ExtractXmlBase,
                    expectedXmlBase),
                this.WriteProperty(
                    ExtractXmlBase,
                    version => noXmlBase),
                this.WriteError(
                    ExtractXmlBase,
                    noXmlBase),
                this.WriteEntityReferenceLinks(
                    ExtractXmlBase,
                    noXmlBase),
                this.WriteEntityReferenceLink(
                    ExtractXmlBase,
                    noXmlBase,
                    false,
                    null),
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    var tc = testConfiguration.Clone();
                    tc.MessageWriterSettings.PayloadBaseUri = baseUri;
                    tc.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testCase(tc);
                });
        }

        private Action<WriterTestConfiguration> WriteFeed(Func<XElement, XElement> fragmentExtractor, Func<ODataVersion, string> expectedXml)
        {
            return (testConfiguration) => TestWriterUtils.WriteAndVerifyODataPayload(
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    ObjectModelUtils.CreateDefaultFeed(),
                    expectedXml(testConfiguration.Version), 
                    null,
                    fragmentExtractor,
                    null,
                    /*disableXmlNamespaceNormalization*/ true),
                testConfiguration,
                this.Assert,
                this.Logger);
        }

        private Action<WriterTestConfiguration> WriteEntry(Func<XElement, XElement> fragmentExtractor, Func<ODataVersion, string> expectedXml)
        {
            return (testConfiguration) => TestWriterUtils.WriteAndVerifyODataPayload(
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    ObjectModelUtils.CreateDefaultEntry(),
                    expectedXml(testConfiguration.Version), 
                    null,
                    fragmentExtractor, 
                    null,
                    /*disableXmlNamespaceNormalization*/ true),
                testConfiguration,
                this.Assert,
                this.Logger);
        }

        private Action<WriterTestConfiguration> WriteCollection(Func<XElement, XElement> fragmentExtractor, Func<ODataVersion, string> expectedXml)
        {
            return (testConfiguration) =>
            {
                if (testConfiguration.IsRequest) return;
                CollectionWriterUtils.WriteAndVerifyCollectionPayload(new CollectionWriterTestDescriptor(
                    this.CollectionSettings,
                    "Collection", new CollectionWriterTestDescriptor.ItemDescription[0],
                    (tc) =>
                    {
                        return new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            FragmentExtractor = fragmentExtractor,
                            Xml = expectedXml(tc.Version),
                            DisableNamespaceNormalization = true
                        };
                    },
                    null), testConfiguration, this.Assert, this.Logger);
            };
        }

        private Action<WriterTestConfiguration> WriteServiceDocument(Func<XElement, XElement> fragmentExtractor, string expectedXml)
        {
            var sampleWorkspace = ObjectModelUtils.CreateDefaultWorkspace();

            return (testConfiguration) =>
            {
                if (testConfiguration.IsRequest)
                {
                    return;
                }

                ODataMessageWriterSettings actualSettings = testConfiguration.MessageWriterSettings;
                if (actualSettings.PayloadBaseUri == null)
                {
                    actualSettings = actualSettings.Clone();
                    actualSettings.PayloadBaseUri = new Uri("http://odata.org");
                }

                TestWriterUtils.WriteAndVerifyTopLevelContent(
                    new PayloadWriterTestDescriptor<ODataServiceDocument>(this.Settings, sampleWorkspace, expectedXml, null, fragmentExtractor, null, /*disableXmlNamespaceNormalization*/ true),
                    testConfiguration,
                    (messageWriter) => messageWriter.WriteServiceDocument(sampleWorkspace),
                    this.Assert,
                    actualSettings,
                    baselineLogger: this.Logger);
            };
        }

        private Action<WriterTestConfiguration> WriteProperty(Func<XElement, XElement> fragmentExtractor, Func<ODataVersion, string> expectedXml)
        {
            var sampleProperty = new ODataProperty() { Name = "Property", Value = 42 };

            return (testConfiguration) =>
            {
                TestWriterUtils.WriteAndVerifyTopLevelContent(new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    sampleProperty,
                    expectedXml(testConfiguration.Version), 
                    null,
                    fragmentExtractor,
                    null,
                    /*disableXmlNamespaceNormalization*/ true),
                testConfiguration,
                (messageWriter) => messageWriter.WriteProperty(sampleProperty),
                this.Assert,
                baselineLogger: this.Logger);
            };
        }

        private Action<WriterTestConfiguration> WriteError(Func<XElement, XElement> fragmentExtractor, string expectedXml)
        {
            var sampleError = new ODataError();

            return (testConfiguration) =>
            {
                if (testConfiguration.IsRequest) return;
                TestWriterUtils.WriteAndVerifyTopLevelContent(new PayloadWriterTestDescriptor<ODataError>(
                    this.Settings,
                    sampleError,
                    expectedXml, 
                    null,
                    fragmentExtractor,
                    null,
                    /*disableXmlNamespaceNormalization*/ true),
                testConfiguration,
                (messageWriter) => messageWriter.WriteError(sampleError, false),
                this.Assert,
                baselineLogger: this.Logger);
            };
        }

        private Action<WriterTestConfiguration> WriteEntityReferenceLinks(Func<XElement, XElement> fragmentExtractor, string expectedXml)
        {
            var sampleLinks = new ODataEntityReferenceLinks();

            return (testConfiguration) =>
            {
                PayloadWriterTestDescriptor<ODataEntityReferenceLinks> descriptor = new PayloadWriterTestDescriptor<ODataEntityReferenceLinks>(
                    this.Settings,
                    sampleLinks,
                    expectedXml,
                    null,
                    fragmentExtractor,
                    null,
                    /*disableXmlNamespaceNormalization*/ true);
                // Top-level EntityReferenceLinks payload write requests are not allowed.
                if (testConfiguration.IsRequest)
                {
                    descriptor.ExpectedResultCallback = (testConfig) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed")
                    };
                }
                TestWriterUtils.WriteAndVerifyTopLevelContent(
                    descriptor,
                    testConfiguration,
                    (messageWriter) => messageWriter.WriteEntityReferenceLinks(sampleLinks),
                    this.Assert,
                    baselineLogger: this.Logger);
            };
        }

        private Action<WriterTestConfiguration> WriteEntityReferenceLink(Func<XElement, XElement> fragmentExtractor, string expectedXml, bool isClient, string odataNamespace)
        {
            var sampleLink = new ODataEntityReferenceLink { Url = new Uri("http://odata.org/link") };

            return (testConfiguration) =>
            {
                PayloadWriterTestDescriptor<ODataEntityReferenceLink> descriptor = null;
                    descriptor = new PayloadWriterTestDescriptor<ODataEntityReferenceLink>(
                        this.Settings,
                        sampleLink,
                        expectedXml,
                        null,
                        fragmentExtractor,
                        null);

                TestWriterUtils.WriteAndVerifyTopLevelContent(
                    descriptor,
                    testConfiguration,
                    (messageWriter) => messageWriter.WriteEntityReferenceLink(sampleLink),
                    this.Assert,
                    baselineLogger: this.Logger);
            };
        }

        private static XElement ExtractRootElementAndNamespaceDeclarations(XElement root)
        {
            var e = new XElement(root);
            e.Attributes().Where(a => !a.IsNamespaceDeclaration).Remove();
            e.Nodes().Remove();
            return e;
        }

        private static XElement ExtractXmlBase(XElement root)
        {
            var e = new XElement("root");
            e.Add(root.Attribute(XNamespace.Xml + TestAtomConstants.XmlBaseAttributeName));
            return e;
        }
    }
}
