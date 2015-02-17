//---------------------------------------------------------------------
// <copyright file="AtomCategoryMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;

    /// <summary>
    /// Tests to verify writer correctly works with ATOM category metadata
    /// </summary>
    [TestClass, TestCase]
    public class AtomCategoryMetadataTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Tests that category metadata on entry is correctly written.")]
        public void CategoryMetadataOnEntryWriterTest()
        {
            var testCases = this.CreateAtomCategoryTestCases();

            Func<XElement, XElement> fragmentExtractor = (e) =>
                {
                    var categoryElement = e.Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomCategoryElementName);
                    return categoryElement ?? new XElement("missingCategory");
                };

            // Convert test cases to test descriptions
            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                {
                    ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                    AtomEntryMetadata metadata = entry.Atom();
                    this.Assert.IsNotNull(metadata, "Expected default entry metadata on a default entry.");
                    metadata.Categories = new AtomCategoryMetadata[] { testCase.Category };
                    return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration => 
                        new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = testCase.Xml, ExpectedException2 = testCase.ExpectedException, FragmentExtractor = fragmentExtractor });
                });

            // Add tests for category with type name
            string testTypeName = "TestModel.TestTypeName";
            string testLabel = "Test category 1 label";
            var categoryWithTypeNameTestCases = new[]
            {
                // Clean merge, no conflicts
                new AtomCategoryTestCase
                {
                    Category = new AtomCategoryMetadata()
                    {
                        Term = null,
                        Scheme = null,
                        Label = testLabel,
                    },
                    Xml = @"<category term=""" + testTypeName + @""" scheme=""" + TestAtomConstants.ODataSchemeNamespace + @""" label=""" + testLabel + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                    ExpectedException = null
                },
                // Patch, conflicting values match
                new AtomCategoryTestCase
                {
                    Category = new AtomCategoryMetadata()
                    {
                        Term = testTypeName,
                        Scheme = TestAtomConstants.ODataSchemeNamespace,
                        Label = testLabel,
                    },
                    Xml = @"<category term=""" + testTypeName + @""" scheme=""" + TestAtomConstants.ODataSchemeNamespace + @""" label=""" + testLabel + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                    ExpectedException = null
                },
                // Patch conflict on term
                new AtomCategoryTestCase
                {
                    Category = new AtomCategoryMetadata()
                    {
                        Term = testTypeName.ToUpper(),
                        Scheme = TestAtomConstants.ODataSchemeNamespace,
                        Label = testLabel,
                    },
                    Xml = @"<category term=""" + testTypeName + @""" scheme=""" + TestAtomConstants.ODataSchemeNamespace + @""" label=""" + testLabel + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_CategoryTermsMustMatch", testTypeName, testTypeName.ToUpper())
                },
                // Patch conflict on scheme
                new AtomCategoryTestCase
                {
                    Category = new AtomCategoryMetadata()
                    {
                        Term = testTypeName,
                        Scheme = TestAtomConstants.ODataSchemeNamespace.ToUpper(),
                        Label = testLabel,
                    },
                    Xml = @"<category term=""" + testTypeName + @""" scheme=""" + TestAtomConstants.ODataSchemeNamespace + @""" label=""" + testLabel + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_CategorySchemesMustMatch", TestAtomConstants.ODataSchemeNamespace, TestAtomConstants.ODataSchemeNamespace.ToUpper())
                },
            };

            testDescriptors = testDescriptors.Concat(categoryWithTypeNameTestCases.Select(testCase =>
                {
                    ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                    entry.TypeName = testTypeName;
                    AtomEntryMetadata metadata = entry.Atom();
                    this.Assert.IsNotNull(metadata, "Expected default entry metadata on a default entry.");
                    metadata.CategoryWithTypeName = testCase.Category;
                    return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                        new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = testCase.Xml, ExpectedException2 = testCase.ExpectedException, FragmentExtractor = fragmentExtractor });
                }));

            // Add all the categories as the category with type name on entry which has no type name.
            // This verifies that the writer won't write the category with type name if there's no type name.
            testDescriptors = testDescriptors.Concat(testCases.Select(testCase =>
                {
                    ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                    AtomEntryMetadata metadata = entry.Atom();
                    this.Assert.IsNotNull(metadata, "Expected default entry metadata on a default entry.");
                    metadata.CategoryWithTypeName = testCase.Category;
                    return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                        new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = "<missingCategory />", ExpectedException2 = null, FragmentExtractor = fragmentExtractor });
                }));

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

        [TestMethod, Variation(Description = "Tests that category metadata on app:categories is correctly written.")]
        public void CategoryMetadataOnWorkspaceCollectionCategoriesWriterTest()
        {
            var testCases = this.CreateAtomCategoryTestCases();

            // Convert test cases to test descriptions
            var testDescriptors = testCases.Select(testCase =>
            {
                AtomResourceCollectionMetadata metadata = new AtomResourceCollectionMetadata
                {
                    Categories = new AtomCategoriesMetadata
                    {
                        Categories = new[] { testCase.Category }
                    }
                };

                ODataEntitySetInfo collection = new ODataEntitySetInfo { Url = new Uri("http://odata.org/collection") };
                collection.SetAnnotation(metadata);
                ODataServiceDocument serviceDocument = new ODataServiceDocument { EntitySets = new[] { collection } };

                return new PayloadWriterTestDescriptor<ODataServiceDocument>(
                    this.Settings, 
                    serviceDocument,
                    testConfiguration =>
                        new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) 
                        { 
                            Xml = testCase.Xml, 
                            ExpectedException2 = testCase.ExpectedException, 
                            FragmentExtractor = result => result
                                .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingWorkspaceElementName)
                                .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingCollectionElementName)
                                .Element(TestAtomConstants.AtomPublishingXNamespace + TestAtomConstants.AtomPublishingCategoriesElementName)
                                .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomCategoryElementName)
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
                        baselineLogger:this.Logger);
                });
        }

        private sealed class AtomCategoryTestCase
        {
            public AtomCategoryMetadata Category { get; set; }
            public string Xml { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        private IEnumerable<AtomCategoryTestCase> CreateAtomCategoryTestCases()
        {
            string testTerm = "Test category 1 term";
            string testLabel = "Test category 1 label";
            string testScheme = "http://odata.org/categories/1";

            return new[]
                {
                    new AtomCategoryTestCase
                    {
                        Category = new AtomCategoryMetadata()
                        {
                            Term = testTerm,
                            Label = testLabel,
                            Scheme = testScheme
                        },
                        Xml = @"<category term=""" + testTerm + @""" scheme=""" + testScheme + @""" label=""" + testLabel + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                        ExpectedException = null
                    },
                    new AtomCategoryTestCase
                    {
                        Category = new AtomCategoryMetadata()
                        {
                            Term = null,
                            Label = testLabel,
                            Scheme = testScheme
                        },
                        Xml = (string)null,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm")
                    },
                    new AtomCategoryTestCase
                    {
                        Category = new AtomCategoryMetadata()
                        {
                            Term = testTerm,
                            Label = null,
                            Scheme = null
                        },
                        Xml = @"<category term=""" + testTerm + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                        ExpectedException = null
                    }
                };
        }
    }
}
