//---------------------------------------------------------------------
// <copyright file="AtomResourceCollectionMetadataTests.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests to verify writer correctly writes ATOM PUB's resource collection metadata.
    /// </summary>
    [TestClass, TestCase]
    public class AtomResourceCollectionMetadataTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Tests that ATOMPUB resource collection metadata gets written correctly.")]
        public void AtomResourceCollectionMetadataTest()
        {
            var testCases = new[]
            {
                // No title specified - an empty one is used as it's required by the spec.
                new
                {
                    CustomizeMetadata = (Action<ODataEntitySetInfo>)(collection => {
                        AtomResourceCollectionMetadata metadata = collection.Atom();
                    }),
                    Xml = "<Collection><title xmlns='http://www.w3.org/2005/Atom'/></Collection>"
                },
                // Simple title (other titles are tested in AtomTextConstructMetadataTests)
                new
                {
                    CustomizeMetadata = (Action<ODataEntitySetInfo>)(collection => {
                        AtomResourceCollectionMetadata metadata = collection.Atom();
                        metadata.Title = new AtomTextConstruct { Text = "collection title" };
                    }),
                    Xml = "<Collection><title type='text' xmlns='http://www.w3.org/2005/Atom'>collection title</title></Collection>"
                },
                // With accept
                new
                {
                    CustomizeMetadata = (Action<ODataEntitySetInfo>)(collection => {
                        AtomResourceCollectionMetadata metadata = collection.Atom();
                        metadata.Title = "collection title";
                        metadata.Accept = "mime/type";
                    }),
                    Xml = "<Collection><title type='text' xmlns='http://www.w3.org/2005/Atom'>collection title</title><accept xmlns='http://www.w3.org/2007/app'>mime/type</accept></Collection>"
                },
                // With empty accept
                new
                {
                    CustomizeMetadata = (Action<ODataEntitySetInfo>)(collection => {
                        AtomResourceCollectionMetadata metadata = collection.Atom();
                        metadata.Title = "collection title";
                        metadata.Accept = string.Empty;
                    }),
                    Xml = "<Collection><title type='text' xmlns='http://www.w3.org/2005/Atom'>collection title</title><accept xmlns='http://www.w3.org/2007/app'></accept></Collection>"
                },
                // With categories
                new
                {
                    CustomizeMetadata = (Action<ODataEntitySetInfo>)(collection => {
                        AtomResourceCollectionMetadata metadata = collection.Atom();
                        metadata.Title = "collection title";
                        metadata.Categories = new AtomCategoriesMetadata();
                    }),
                    Xml = "<Collection><title type='text' xmlns='http://www.w3.org/2005/Atom'>collection title</title><categories xmlns='http://www.w3.org/2007/app' /></Collection>"
                },
                // With accept and categories
                new
                {
                    CustomizeMetadata = (Action<ODataEntitySetInfo>)(collection => {
                        AtomResourceCollectionMetadata metadata = collection.Atom();
                        metadata.Title = "collection title";
                        metadata.Accept = "mime/type";
                        metadata.Categories = new AtomCategoriesMetadata();
                    }),
                    Xml = "<Collection><title type='text' xmlns='http://www.w3.org/2005/Atom'>collection title</title><accept xmlns='http://www.w3.org/2007/app'>mime/type</accept><categories xmlns='http://www.w3.org/2007/app' /></Collection>"
                },
            };

            Func<ODataEntitySetInfo>[] collectionCreators = new Func<ODataEntitySetInfo>[]
            {
                () => new ODataEntitySetInfo { Url = new Uri("http://odata.org/url") },
                () => { var collection = new ODataEntitySetInfo { Url = new Uri("http://odata.org/url") }; collection.SetAnnotation(new AtomResourceCollectionMetadata()); return collection; }
            };
            var testDescriptors = testCases.SelectMany(testCase =>
                collectionCreators.Select(collectionCreator =>
                {
                    ODataEntitySetInfo collection = collectionCreator();
                    testCase.CustomizeMetadata(collection);
                    ODataServiceDocument serviceDocument = new ODataServiceDocument { EntitySets = new[] { collection } };
                    return new PayloadWriterTestDescriptor<ODataServiceDocument>(
                        this.Settings,
                        new[] { serviceDocument },
                        tc => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Xml = testCase.Xml,
                            FragmentExtractor = result =>
                                new XElement("Collection",
                                    result
                                        .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingWorkspaceElementName)
                                        .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingCollectionElementName)
                                        .Elements())
                        });
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
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

        [TestMethod, Variation(Description = "Tests that ATOMPUB categories metadata gets written correctly.")]
        public void AtomCategoriesMetadataTest()
        {
            var testCases = new []
            {
                // Empty categories
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata(),
                    Xml = "<categories xmlns='http://www.w3.org/2007/app'/>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with href
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Href = new Uri("http://odata.org/href") },
                    Xml = "<categories href='http://odata.org/href' xmlns='http://www.w3.org/2007/app'/>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with href and non-null empty categories (null and empty collection should be treated the same)
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Href = new Uri("http://odata.org/href"), Categories = new AtomCategoryMetadata[0] },
                    Xml = "<categories href='http://odata.org/href' xmlns='http://www.w3.org/2007/app'/>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with fixed yes
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Fixed = true },
                    Xml = "<categories fixed='yes' xmlns='http://www.w3.org/2007/app'/>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with fixed no
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Fixed = false },
                    Xml = "<categories fixed='no' xmlns='http://www.w3.org/2007/app'/>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with scheme
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Scheme = "http://odata.org/scheme" },
                    Xml = "<categories scheme='http://odata.org/scheme' xmlns='http://www.w3.org/2007/app'/>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with scheme and fixed
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Fixed = true, Scheme = "http://odata.org/scheme" },
                    Xml = "<categories fixed='yes' scheme='http://odata.org/scheme' xmlns='http://www.w3.org/2007/app'/>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with single category
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Categories = new [] { new AtomCategoryMetadata { Term = "myterm", Scheme = "http://odata.org/scheme" } } },
                    Xml = "<categories xmlns='http://www.w3.org/2007/app'><atom:category term='myterm' scheme='http://odata.org/scheme' xmlns:atom='http://www.w3.org/2005/Atom'/></categories>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with two categories
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Categories = new [] { 
                        new AtomCategoryMetadata { Term = "myterm", Scheme = "http://odata.org/scheme" },
                        new AtomCategoryMetadata { Term = "second", Scheme = "http://odata.org/scheme2" } } },
                    Xml = "<categories xmlns='http://www.w3.org/2007/app'>" +
                            "<atom:category term='myterm' scheme='http://odata.org/scheme' xmlns:atom='http://www.w3.org/2005/Atom'/>" +
                            "<atom:category term='second' scheme='http://odata.org/scheme2' xmlns:atom='http://www.w3.org/2005/Atom'/>" +
                        "</categories>",
                    ExpectedException = (ExpectedException)null
                },
                // Categories with href and fixed (error)
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Href = new Uri("http://odata.org/href"), Fixed = true },
                    Xml = string.Empty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues")
                },
                // Categories with href and scheme (error)
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Href = new Uri("http://odata.org/href"), Scheme = "http://odata.org/scheme" },
                    Xml = string.Empty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues")
                },
                // Categories with href and non-empty categories (error)
                new
                {
                    CategoriesMetadata = new AtomCategoriesMetadata() { Href = new Uri("http://odata.org/href"), Categories = new [] { new AtomCategoryMetadata() } },
                    Xml = string.Empty,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues")
                },
            };

            var testDescriptors = testCases.Select(testCase =>
            {
                ODataEntitySetInfo collection = new ODataEntitySetInfo { Url = new Uri("http://odata.org/url") };
                AtomResourceCollectionMetadata metadata = new AtomResourceCollectionMetadata() { Categories = testCase.CategoriesMetadata };
                collection.SetAnnotation(metadata);
                ODataServiceDocument serviceDocument = new ODataServiceDocument { EntitySets = new [] { collection } };
                return new PayloadWriterTestDescriptor<ODataServiceDocument>(
                    this.Settings,
                    new [] { serviceDocument },
                    tc =>
                    {
                        if (testCase.ExpectedException != null)
                        {
                            return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException };
                        }
                        else
                        {
                            return new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Xml = testCase.Xml,
                                FragmentExtractor = result =>
                                    result
                                        .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingWorkspaceElementName)
                                        .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingCollectionElementName)
                                        .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingCategoriesElementName)
                            };
                        }
                    });
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
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
    }
}
