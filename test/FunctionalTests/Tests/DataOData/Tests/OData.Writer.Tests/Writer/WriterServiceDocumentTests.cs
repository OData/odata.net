//---------------------------------------------------------------------
// <copyright file="WriterServiceDocumentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for writing service documents using the ODataMessageWriter.
    /// </summary>
    [TestClass, TestCase]
    public class WriterServiceDocumentTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        private const string baseUri = "http://odata.org/";

        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        private static readonly ODataMessageWriterSettings settingsWithBaseUri = new ODataMessageWriterSettings { BaseUri = new Uri(baseUri) };

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test different combinations of service document writing.")]
        public void WorkspaceNamesTests()
        {
            string[] workspaceNames = new string[] { null, string.Empty, "MyWorkspaceName" };

            var testCases = workspaceNames.Select(name => new PayloadWriterTestDescriptor<ODataServiceDocument>(this.Settings, CreateWorkspace(/*createMetadataFirst*/ true, name), this.CreateExpectedResultCallback(baseUri, name)));

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testCase, testConfig) =>
                {
                    WriterTestConfiguration newConfiguration = testConfig.Clone();
                    newConfiguration.MessageWriterSettings.BaseUri = new Uri(baseUri);
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testCase,
                        newConfiguration,
                        (messageWriter) => messageWriter.WriteServiceDocument(testCase.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Tests the behavior of the Name property on a resource collection.")]
        public void ResourceCollectionNamePropertyTests()
        {
            // The behavior of the Name property depends on the format.
            // JSON Light: The Name property is required and written to the "name" property in the format.

            IList<CollectionInfo> interestingCollections =
                new[]
                {
                    // Non null collection name
                    new CollectionInfo
                    {
                        Name = "CollectionName",
                        Url = "CollectionRelativeUrl",
                    },
                    // No name or title annotation
                    new CollectionInfo
                    {
                        Url = "CollectionRelativeUrl"
                    },
                    // Name and title annotation are both non null and are equal.
                    new CollectionInfo
                    {
                        Name = "CollectionName",
                        Url = "CollectionRelativeUrl",
                        TitleAnnotation = "CollectionName"
                    },
                    // Name and title annotation differ (expect failure in ATOM).
                    new CollectionInfo
                    {
                        Name = "CollectionName",
                        Url = "CollectionRelativeUrl",
                        TitleAnnotation = "CollectionTitle"
                    }
                };

            var collectionArrays = interestingCollections.Variations(0, 1, 3);

            EdmModel model = new EdmModel();
            EdmEntityContainer edmEntityContainer = new EdmEntityContainer("DefaultNamespace", "DefaultContainer");
            model.AddElement(edmEntityContainer);

            var testDescriptors = collectionArrays.Select(collectionArray => new PayloadWriterTestDescriptor<ODataServiceDocument>(
                this.Settings,
                CreateWorkspace( /*createMetadataFirst*/ false, /* workspaceName */ null, collectionArray),
                CreateExpectedResultCallback(baseUri, /* workspaceName */ null, collectionArray))
            {
                Model = model,
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfig) =>
                {
                    WriterTestConfiguration newConfiguration = testConfig.Clone();
                    newConfiguration.MessageWriterSettings.BaseUri = new Uri(baseUri);
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        newConfiguration,
                        (messageWriter) => messageWriter.WriteServiceDocument(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test different combinations of writing service documents with resource collections and serviceDocument names.")]
        public void WorkspaceNamesAndResourceCollectionTests()
        {
            const string baseUri = "http://odata.org/";

            int[] collectionSizes = new int[] { 0, 1, 3 };

            IList<CollectionInfo> coreCollections =
                new List<CollectionInfo>
                {
                    new CollectionInfo { Url = "EntitySet1" },
                    new CollectionInfo { Url = "EntitySet2" },
                    new CollectionInfo { Url = "EntitySet3" },
                };

            IList<CollectionInfo> additionalCollections =
                new List<CollectionInfo>
                {
                    new CollectionInfo() { Url = "Additional1" },
                    new CollectionInfo() { Url = "Additional2" },
                };

            bool[] includeAdditionalCollections = new bool[] { true, false };

            IEnumerable<CollectionInfo[]> collectionArrays = null;
            foreach (int length in collectionSizes)
            {
                if (collectionArrays == null)
                {
                    collectionArrays = coreCollections.Variations(length);
                }
                else
                {
                    collectionArrays = collectionArrays.Concat(coreCollections.Variations(length));
                }
            }

            bool[] useMetadata = new bool[] { true, false };
            bool[] setTitle = new bool[] { true, false };

            string[] workspaceNames = new string[] { null, string.Empty, "MyWorkspaceName" };
            IEnumerable<PayloadWriterTestDescriptor<ODataServiceDocument>> testDescriptors = null;

            foreach (var name in workspaceNames)
            {
                var vals = collectionArrays
                    .SelectMany(collectionArray =>
                        includeAdditionalCollections.Select(flag => flag ? collectionArray.Concat(additionalCollections) : collectionArray))
                    .SelectMany(collectionArray2 =>
                        setTitle.Select(setTitleFlag => setTitleFlag ? collectionArray2.Select(c => new CollectionInfo { Url = c.Url, TitleAnnotation = c.Url }) : collectionArray2))
                    .Select(collectionArray3 => new PayloadWriterTestDescriptor<ODataServiceDocument>(
                        this.Settings,
                        CreateWorkspace(/*createMetadataFirst*/ false, name, collectionArray3),
                        CreateExpectedResultCallback(baseUri, name, collectionArray3))
                    {
                        Model = CreateMetadata(collectionArray3)
                    });

                if (testDescriptors == null)
                {
                    testDescriptors = vals;
                }
                else
                {
                    testDescriptors.Concat(vals);
                }
            }

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfig) =>
                {
                    WriterTestConfiguration newConfiguration = testConfig.Clone();
                    newConfiguration.MessageWriterSettings.BaseUri = new Uri(baseUri);
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        newConfiguration,
                        (messageWriter) => messageWriter.WriteServiceDocument(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test different combinations of writing service documents with resource collections.")]
        public void ResourceCollectionTests()
        {
            const string baseUri = "http://odata.org/";

            int[] collectionSizes = new int[] { 0, 1, 3 };

            IList<CollectionInfo> coreCollections = new string[] { "EntitySet1", "EntitySet2", "EntitySet3" }.Select(
                    name => new CollectionInfo { Name = name, Url = name, TitleAnnotation = name }).ToList();
            IList<CollectionInfo> additionalCollections = new string[] { "Additional1", "Additional2" }.Select(
                    name => new CollectionInfo { Name = name, Url = name, TitleAnnotation = name }).ToList();

            bool[] includeAdditionalCollections = new bool[] { true, false };

            IEnumerable<CollectionInfo[]> collectionArrays = null;
            foreach (int length in collectionSizes)
            {
                if (collectionArrays == null)
                {
                    collectionArrays = coreCollections.Variations(length);
                }
                else
                {
                    collectionArrays = collectionArrays.Concat(coreCollections.Variations(length));
                }
            }

            bool[] useMetadata = new bool[] { true, false };
            bool[] setTitle = new bool[] { true, false };

            const string workspaceName = null;

            var testDescriptors = collectionArrays
                .SelectMany(collectionArray =>
                    includeAdditionalCollections.Select(flag => flag ? collectionArray.Concat(additionalCollections) : collectionArray))
                    .SelectMany(collectionArray2 => setTitle.Select(setTitleFlag => setTitleFlag ? collectionArray2.Select(c => new CollectionInfo { Name = c.Name, Url = c.Url, TitleAnnotation = c.Url }) : collectionArray2))
                    .Select(collectionArray3 =>
                        new PayloadWriterTestDescriptor<ODataServiceDocument>(
                        this.Settings,
                        CreateWorkspace(/*createMetadataFirst*/ true, workspaceName, collectionArray3),
                        CreateExpectedResultCallback(baseUri, workspaceName, collectionArray3))
                        {
                            Model = CreateMetadata(collectionArray3)
                        });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfig) =>
                {
                    WriterTestConfiguration newConfiguration = testConfig.Clone();
                    newConfiguration.MessageWriterSettings.BaseUri = new Uri(baseUri);
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        newConfiguration,
                        (messageWriter) => messageWriter.WriteServiceDocument(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test different combinations of writing service documents with singletons.")]
        public void SingletonTests()
        {
            const string baseUri = "http://odata.org/";

            int[] singletonSizes = new int[] { 0, 1, 3 };

            IList<SingletonInfo> coreSignletons = new string[] { "Singleton1", "Singleton2", "Singleton3" }.Select(
                    name => new SingletonInfo { Name = name, Url = name, TitleAnnotation = name }).ToList();
            IList<SingletonInfo> additionalSingletons = new string[] { "Additional1", "Additional2" }.Select(
                    name => new SingletonInfo { Name = name, Url = name, TitleAnnotation = name }).ToList();

            bool[] includeAdditionalSingletons = new bool[] { true, false };

            IEnumerable<SingletonInfo[]> singletonArrays = null;
            foreach (int length in singletonSizes)
            {
                if (singletonArrays == null)
                {
                    singletonArrays = coreSignletons.Variations(length);
                }
                else
                {
                    singletonArrays = singletonArrays.Concat(coreSignletons.Variations(length));
                }
            }

            bool[] useMetadata = new bool[] { true, false };
            bool[] setTitle = new bool[] { true, false };

            const string workspaceName = null;

            var testDescriptors = singletonArrays
                .SelectMany(collectionArray =>
                    includeAdditionalSingletons.Select(flag => flag ? collectionArray.Concat(additionalSingletons) : collectionArray))
                    .SelectMany(singletonArray2 => setTitle.Select(setTitleFlag => setTitleFlag ? singletonArray2.Select(c => new SingletonInfo { Name = c.Name, Url = c.Url, TitleAnnotation = c.Url }) : singletonArray2))
                    .Select(singletonArray3 =>
                        new PayloadWriterTestDescriptor<ODataServiceDocument>(
                        this.Settings,
                        CreateWorkspace(/*createMetadataFirst*/ true, workspaceName, null, singletonArray3),
                        CreateExpectedResultCallback(baseUri, workspaceName, null, singletonArray3))
                        {
                            Model = CreateMetadata(null, singletonArray3)
                        });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfig) =>
                {
                    WriterTestConfiguration newConfiguration = testConfig.Clone();
                    newConfiguration.MessageWriterSettings.BaseUri = new Uri(baseUri);
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        newConfiguration,
                        (messageWriter) => messageWriter.WriteServiceDocument(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test different combinations of writing service documents with resource collections.")]
        public void AdditionalResourceCollectionTests()
        {
            IEdmModel model = this.CreateMetadata(new[] { new CollectionInfo { Url = "EntitySet1" }, new CollectionInfo { Url = "EntitySet2" } });
            IEdmEntityType entityType = (IEdmEntityType)model.FindType("TestNS.EntityType");

            var testCases = new[]
                {
                    // duplicate resource collection name
                    new
                    {
                        Collections = new [] { new CollectionInfo { Url = "A" }, new CollectionInfo { Url = "A" } },
                        Model = (IEdmModel)null,
                    },

                    // resource collection name does not match metadata.
                    new
                    {
                        Collections = new [] { new CollectionInfo { Url = "SomeName" } },
                        Model = model,
                    },
                };

            var testDescriptors = testCases.Select(tc => new PayloadWriterTestDescriptor<WorkspaceWithSettings>(
                this.Settings,
                new WorkspaceWithSettings()
                {
                    ServiceDocument = CreateWorkspace(/*createMetadataFirst*/ false, null, tc.Collections),
                },
                CreateExpectedResultCallback(baseUri, null, tc.Collections))
            {
                Model = tc.Model,
            });

            this.CombinatorialEngineProvider.RunCombinations(
              testDescriptors,
              this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
              (testDescriptor, testConfig) =>
              {
                  WorkspaceWithSettings docWithSettings = testDescriptor.PayloadItems.Single();
                  if (docWithSettings.WriterSettings != null)
                  {
                      docWithSettings.WriterSettings.EnableMessageStreamDisposal = testConfig.MessageWriterSettings.EnableMessageStreamDisposal;
                  }

                  WriterTestConfiguration newConfiguration = testConfig.Clone();
                  newConfiguration.MessageWriterSettings.BaseUri = new Uri(baseUri);
                  newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                  TestWriterUtils.WriteAndVerifyTopLevelContent(
                      testDescriptor,
                      newConfiguration,
                      (messageWriter) => messageWriter.WriteServiceDocument(docWithSettings.ServiceDocument),
                      this.Assert,
                      docWithSettings.WriterSettings,
                      baselineLogger: this.Logger);
              });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test different error conditions writing service documents.")]
        public void ServiceDocumentErrorTests()
        {
            IEdmModel model = this.CreateMetadata(new[] { new CollectionInfo() { Url = "EntitySet1" }, new CollectionInfo() { Url = "EntitySet2" } });

            var testCases = new[]
                {
                    //new
                    //{   // relative uri without any base Uri
                    //     Workspace = new Func<ODataServiceDocument>(() => {
                    //        var defaultWorkspace = ObjectModelUtils.CreateDefaultWorkspace();
                    //        defaultWorkspace.EntitySets = new ODataEntitySetInfo[]
                    //        {
                    //            new ODataEntitySetInfo() { Url = new Uri("SomeUri", UriKind.Relative) }
                    //        };
                    //        return defaultWorkspace;
                    //    })(),
                    //    MessageWriterSettings = (ODataMessageWriterSettings)null,
                    //    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", "SomeUri"),
                    //    Model = (IEdmModel)null,
                    //    OnlyForFormat = ODataFormat.Atom
                    //},
                    //new
                    //{   // empty relative uri without any base Uri
                    //     Workspace = new Func<ODataServiceDocument>(() => {
                    //        var defaultWorkspace = ObjectModelUtils.CreateDefaultWorkspace();
                    //        defaultWorkspace.EntitySets = new ODataEntitySetInfo[]
                    //        {
                    //            new ODataEntitySetInfo() { Url = new Uri(string.Empty, UriKind.Relative) }
                    //        };
                    //        return defaultWorkspace;
                    //    })(),
                    //    MessageWriterSettings = (ODataMessageWriterSettings)null,
                    //    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", string.Empty),
                    //    Model = (IEdmModel)null,
                    //    OnlyForFormat = ODataFormat.Atom
                    //},
                    new
                    {   // empty collection name
                        Workspace = new Func<ODataServiceDocument>(() => {
                            var defaultWorkspace = ObjectModelUtils.CreateDefaultWorkspace();
                            defaultWorkspace.EntitySets = new ODataEntitySetInfo[]
                            {
                                new ODataEntitySetInfo() { Url = null }
                            };
                            return defaultWorkspace;
                        })(),
                        MessageWriterSettings = settingsWithBaseUri,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_ResourceMustSpecifyUrl"),
                        Model = (IEdmModel)null,
                        OnlyForFormat = (ODataFormat)null
                    },
                    new
                    {   // null collection
                        Workspace = new Func<ODataServiceDocument>(() => {
                            var defaultWorkspace = ObjectModelUtils.CreateDefaultWorkspace();
                            defaultWorkspace.EntitySets = new ODataEntitySetInfo[]
                            {
                                null
                            };
                            return defaultWorkspace;
                        })(),
                        MessageWriterSettings = settingsWithBaseUri,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_WorkspaceResourceMustNotContainNullItem"),
                        Model = (IEdmModel)null,
                        OnlyForFormat = (ODataFormat)null
                    },
                    new
                    {   // no serviceDocument
                        Workspace = (ODataServiceDocument)null,
                        MessageWriterSettings = settingsWithBaseUri,
                        ExpectedException = new ExpectedException(typeof(ArgumentNullException)),
                        Model = (IEdmModel)null,
                        OnlyForFormat = (ODataFormat)null
                    },
                };

            var testDescriptors = testCases.Select(tc => new PayloadWriterTestDescriptor<WorkspaceWithSettings>(
                this.Settings,
                new WorkspaceWithSettings()
                {
                    ServiceDocument = tc.Workspace,
                    WriterSettings = tc.MessageWriterSettings
                },
                CreateErrorResultCallback(tc.ExpectedException, tc.OnlyForFormat, this.Settings.ExpectedResultSettings))
            {
                Model = tc.Model,
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfig) =>
                {
                    testConfig = testConfig.Clone();
                    testConfig.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    WorkspaceWithSettings docWithSettings = testDescriptor.PayloadItems.Single();
                    if (docWithSettings.WriterSettings != null)
                    {
                        docWithSettings.WriterSettings.EnableMessageStreamDisposal = testConfig.MessageWriterSettings.EnableMessageStreamDisposal;
                        docWithSettings.WriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    }

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfig,
                        (messageWriter) => messageWriter.WriteServiceDocument(docWithSettings.ServiceDocument),
                        this.Assert,
                        docWithSettings.WriterSettings,
                        baselineLogger: this.Logger);
                });
        }

        private IEdmModel CreateMetadata(IEnumerable<CollectionInfo> collections = null, IEnumerable<SingletonInfo> singletons = null)
        {
            EdmModel edmModel = new EdmModel();

            EdmEntityType edmEntityType = new EdmEntityType("DefaultNamespace", "EntityType");
            EdmStructuralProperty edmStructuralProperty = new EdmStructuralProperty(edmEntityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            edmEntityType.AddKeys(edmStructuralProperty);
            edmEntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            edmModel.AddElement(edmEntityType);

            EdmEntityContainer edmEntityContainer = new EdmEntityContainer("DefaultNamespace", "TestContainer");
            edmModel.AddElement(edmEntityContainer);

            if (collections != null)
            {
                foreach (var collection in collections)
                {
                    edmEntityContainer.AddEntitySet(collection.Url, edmEntityType);
                }
            }

            if (singletons != null)
            {
                foreach (var singleton in singletons)
                {
                    edmEntityContainer.AddSingleton(singleton.Url, edmEntityType);
                }
            }

            return edmModel;
        }

        private static ODataServiceDocument CreateWorkspace(bool createMetadataFirst, string workspaceName = null, IEnumerable<CollectionInfo> incomingCollections = null, IEnumerable<SingletonInfo> incomingSingletons = null)
        {
            ODataServiceDocument serviceDocument = ObjectModelUtils.CreateDefaultWorkspace();


            if (incomingCollections != null)
            {
                var collections = new List<ODataEntitySetInfo>();
                foreach (var collectionInfo in incomingCollections)
                {
                    var collection = new ODataEntitySetInfo() { Url = new Uri(collectionInfo.Url, UriKind.RelativeOrAbsolute), Name = collectionInfo.Name, Title = collectionInfo.TitleAnnotation };
                    collections.Add(collection);
                }

                serviceDocument.EntitySets = collections;
            }

            if (incomingSingletons != null)
            {
                var singletons = new List<ODataSingletonInfo>();
                foreach (var singletonInfo in incomingSingletons)
                {
                    var singleton = new ODataSingletonInfo() { Url = new Uri(singletonInfo.Url, UriKind.RelativeOrAbsolute), Name = singletonInfo.Name, Title = singletonInfo.TitleAnnotation };
                    singletons.Add(singleton);
                }

                serviceDocument.Singletons = singletons;
            }

            return serviceDocument;
        }

        private class ResourceInfo
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string TitleAnnotation { get; set; }
        }

        private class CollectionInfo : ResourceInfo
        {
        }

        private class SingletonInfo : ResourceInfo
        {
        }

        private WriterTestDescriptor.WriterTestExpectedResultCallback CreateExpectedResultCallback(
            string baseUri,
            string workspaceName = null,
            IEnumerable<CollectionInfo> collections = null,
            IEnumerable<SingletonInfo> singletons = null)
        {
            string xmlResultTemplate;
            string[] jsonLightResultTemplate;
            CreateResultTemplates(baseUri, workspaceName, collections, singletons, out xmlResultTemplate, out jsonLightResultTemplate);

            return testConfiguration =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("ODataMessageWriter_ServiceDocumentInRequest"),
                        };
                    }

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        if (collections != null)
                        {
                            if (collections.Any(collection => collection.Name == null))
                            {
                                var urlString = collections.First().Url.ToString();
                                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException2 = ODataExpectedExceptions.ODataException("ValidationUtils_ResourceMustSpecifyName", urlString)
                                };
                            }
                        }

                        if (singletons != null)
                        {
                            if (singletons.Any(collection => collection.Name == null))
                            {
                                var urlString = collections.First().Url.ToString();
                                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException2 = ODataExpectedExceptions.ODataException("ValidationUtils_ResourceMustSpecifyName", urlString)
                                };
                            }
                        }

                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = string.Join("$(NL)", jsonLightResultTemplate),
                            FragmentExtractor = (result) => result
                        };
                    }
                    else
                    {
                        throw new NotSupportedException("Format " + testConfiguration.Format.ToString() + " is not supported for service document tests.");
                    }
                };
        }

        private static PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateErrorResultCallback(
            ExpectedException expectedException,
            ODataFormat onlyForFormat,
            WriterTestExpectedResults.Settings expectedResultSettings)
        {
            return testConfiguration =>
            {
                if (onlyForFormat != null && testConfiguration.Format != onlyForFormat)
                {
                    return null;
                }

                return new WriterTestExpectedResults(expectedResultSettings)
                {
                    // Replace the expected exception for requests where an ODataException is expected
                    ExpectedException2 =
                        testConfiguration.IsRequest && (expectedException != null && expectedException.ExpectedExceptionType.Equals(typeof(ODataException)))
                        ? ODataExpectedExceptions.ODataException("ODataMessageWriter_ServiceDocumentInRequest")
                        : expectedException,
                };
            };
        }

        private static void CreateResultTemplates(
            string baseUri,
            string workspaceName,
            IEnumerable<CollectionInfo> collections,
            IEnumerable<SingletonInfo> singletons,
            out string xmlResultTemplate,
            out string[] jsonLightResultTemplate)
        {
            Debug.Assert(baseUri != null, "baseUri != null");

            List<string> xmlLines = new List<string>();
            List<string> jsonLightLines = new List<string>();

            xmlLines.Add(@"<service xml:base=""" + baseUri + @""" xmlns=""http://www.w3.org/2007/app"" xmlns:atom=""http://www.w3.org/2005/Atom"">");
            xmlLines.Add(@"$(Indent)<serviceDocument>");
            jsonLightLines.Add("{");
            jsonLightLines.Add(@"$(Indent)""" + JsonLightConstants.ODataContextAnnotationName + @""":""" + TestUriUtils.ToEscapedUriString(JsonLightConstants.DefaultMetadataDocumentUri) + @""",""value"":[");

            if (workspaceName == null)
            {
                workspaceName = TestAtomConstants.AtomWorkspaceDefaultTitle;
            }
            xmlLines.Add(@"$(Indent)$(Indent)<atom:title type='text'>" + workspaceName + "</atom:title>");

            StringBuilder jsonLightBuilder = new StringBuilder();

            if (collections != null)
            {
                foreach (var collection in collections)
                {
                    xmlLines.Add(@"$(Indent)$(Indent)<collection href=""" + collection.Url + @""">");

                    if (collection.Name != null || collection.TitleAnnotation != null)
                    {
                        string titleValue = collection.TitleAnnotation ?? collection.Name;

                        xmlLines.Add(@"$(Indent)$(Indent)$(Indent)<atom:title type='text'>" + titleValue + @"</atom:title>");
                    }
                    else
                    {
                        xmlLines.Add(@"$(Indent)$(Indent)$(Indent)<atom:title />");
                    }

                    xmlLines.Add(@"$(Indent)$(Indent)</collection>");

                    if (jsonLightBuilder.Length > 0)
                    {
                        jsonLightBuilder.Append(",");
                    }

                    jsonLightBuilder.Append("{");
                    jsonLightBuilder.Append("$(NL)");
                    jsonLightBuilder.Append(@"$(Indent)$(Indent)$(Indent)""name"":""" + collection.Name + "\",");
                    jsonLightBuilder.Append(@"""url"":""" + collection.Url + "\"");
                    jsonLightBuilder.Append(@"$(NL)");
                    jsonLightBuilder.Append("$(Indent)$(Indent)}");
                }

                jsonLightLines.Add("$(Indent)$(Indent)" + jsonLightBuilder.ToString());
            }
            else
            {
                jsonLightLines.Add("$(Indent)$(Indent)");
            }

            if (singletons != null)
            {
                foreach (var singleton in singletons)
                {
                    xmlLines.Add(@"$(Indent)$(Indent)<singleton href=""" + singleton.Url + @""">");

                    if (singleton.Name != null || singleton.TitleAnnotation != null)
                    {
                        string titleValue = singleton.TitleAnnotation ?? singleton.Name;

                        xmlLines.Add(@"$(Indent)$(Indent)$(Indent)<atom:title type='text'>" + titleValue + @"</atom:title>");
                    }
                    else
                    {
                        xmlLines.Add(@"$(Indent)$(Indent)$(Indent)<atom:title />");
                    }

                    xmlLines.Add(@"$(Indent)$(Indent)</collection>");

                    if (jsonLightBuilder.Length > 0)
                    {
                        jsonLightBuilder.Append(",");
                    }

                    jsonLightBuilder.Append("{");
                    jsonLightBuilder.Append("$(NL)");
                    jsonLightBuilder.Append(@"$(Indent)$(Indent)$(Indent)""name"":""" + singleton.Name + "\",");
                    jsonLightBuilder.Append(@"""url"":""" + singleton.Url + "\"");
                    jsonLightBuilder.Append(@"$(NL)");
                    jsonLightBuilder.Append("$(Indent)$(Indent)}");
                }

                jsonLightLines.Add("$(Indent)$(Indent)" + jsonLightBuilder.ToString());
            }
            else
            {
                jsonLightLines.Add("$(Indent)$(Indent)");
            }

            xmlLines.Add(@"$(Indent)</serviceDocument>");
            xmlLines.Add(@"</service>");
            jsonLightLines.Add("$(Indent)]");
            jsonLightLines.Add("}");

            xmlResultTemplate = string.Join("$(NL)", xmlLines);
            jsonLightResultTemplate = jsonLightLines.ToArray();
        }

        private sealed class WorkspaceWithSettings
        {
            public ODataServiceDocument ServiceDocument
            {
                get;
                set;
            }

            public ODataMessageWriterSettings WriterSettings
            {
                get;
                set;
            }
        }

        private sealed class ServiceDocumentTestDefinition
        {
            public string WorkspaceName { get; set; }
            public string[] Collections { get; set; }
            public string[] AdditionalCollections { get; set; }
        }
    }
}
