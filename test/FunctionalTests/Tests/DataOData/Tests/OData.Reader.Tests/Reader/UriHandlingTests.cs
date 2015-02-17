//---------------------------------------------------------------------
// <copyright file="UriHandlingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;
    #endregion Namespaces

    /// <summary>
    /// Tests handling of URIs in payloads.
    /// </summary>
    [TestClass, TestCase]
    public class UriHandlingTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToXmlConverter PayloadElementToXmlConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadElementToJsonConverter PayloadElementToJsonConverter { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        /// <summary>
        /// Gets or sets the dependency injector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public Microsoft.Test.Taupo.Contracts.IDependencyInjector Injector { get; set; }

        private PayloadReaderTestDescriptor.Settings noValidatorSettings;

        public PayloadReaderTestDescriptor.Settings NoValidatorSettings 
        {
            get
            {
                if (this.noValidatorSettings == null)
                {
                    this.noValidatorSettings = new PayloadReaderTestDescriptor.Settings();
                    this.Injector.InjectDependenciesInto(this.noValidatorSettings);
                    this.noValidatorSettings.ExpectedResultSettings.DefaultODataObjectModelValidator = new Microsoft.Test.Taupo.OData.Reader.Tests.Common.AggregateODataObjectModelValidator();
                }

                return this.noValidatorSettings;
            }
        }

        protected override void ConfigureDependencies(Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);

            // We have to register JSON converter for this test case explicitely since we're using the batch infrastructure
            // which doesn't inherit the JSON converter from the PayloadReaderTestDescriptor.Settings.
            container.Register<IPayloadElementToJsonConverter, AnnotatedPayloadElementToJsonConverter>();
            container.Register<IPayloadElementToJsonLightConverter, AnnotatedPayloadElementToJsonLightConverter>();
        }

        /// <summary>
        /// Set of absolute and relative URIs used in the tests as payload URIs.
        /// </summary>
        private static readonly Uri[] payloadUris = new Uri[]
        {
            new Uri("http://odata.org/absolute"),
            new Uri(string.Empty, UriKind.Relative),
            new Uri("relative", UriKind.Relative),
        };

        /// <summary>Default base URI from the xml:base attribute</summary>
        private static readonly Uri xmlBaseUri = new Uri("http://odata.org/xmlbase");

        /// <summary>Default base URI from the reader settings.</summary>
        private static readonly Uri readerSettingBaseUri = new Uri("http://odata.org/readersetting/");

        /// <summary>All interesting combinations of xml:base and reader settings-specified base URIs.</summary>
        private static readonly BaseUriValue[] baseUriValues = new BaseUriValue[]
        {
            // No base URIs
            new BaseUriValue { XmlBaseUri = null, ReaderSettingBaseUri = null, },
            // Only Xml base URI
            new BaseUriValue { XmlBaseUri = xmlBaseUri, ReaderSettingBaseUri = null, },
            // Only reader setting base URI
            new BaseUriValue { XmlBaseUri = null, ReaderSettingBaseUri = readerSettingBaseUri, },
            //Both base URIs
            new BaseUriValue { XmlBaseUri = xmlBaseUri, ReaderSettingBaseUri = readerSettingBaseUri, },
        };

        private static readonly Uri resolverResultRelative = new Uri("resolverelative", UriKind.Relative);
        private static readonly Uri resolverResultAbsolute = new Uri("http://odata.org/resolverabsolute");

        /// <summary>Resolvers to use.</summary>
        private static readonly KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>[] resolvers = new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>[]
        {
            // No resolver
            new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>(null, null),
            // Resolver which always returns relative URL
            new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>((inputUri, baseUri, payloadUri) =>
                {
                    return (inputUri.OriginalString == payloadUri.OriginalString) ? resolverResultRelative : null;
                },
                resolverResultRelative),
            // Resolver which always returns absolute URL
            new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>((inputUri, baseUri, payloadUri) => 
                {
                    return (inputUri.OriginalString == payloadUri.OriginalString) ? resolverResultAbsolute : null;
                },
                resolverResultAbsolute),
        };

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading URLs in feed payloads (with and without base URI).")]
        public void FeedReadingBaseUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            EntitySetInstance feed = PayloadBuilder.EntitySet();

            // Run only in version >= 2 since next links are only supported since v2 and not allowed in requests
            this.CombinatorialEngineProvider.RunCombinations(
                payloadUris,
                baseUriValues,
                resolvers,
                new bool[] { false, true },
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (payloadUri, baseUriValue, resolver, runInBatch, testConfiguration) =>
                {
                    Action<EntitySetInstance, Uri, ReaderTestConfiguration> setNextLinkAction =
                        (instance, uri, testConfig) => instance.NextLink = UriToString(uri);
                    this.RunBaseUriReadingTest(feed, setNextLinkAction, model, payloadUri, baseUriValue, resolver, testConfiguration, runInBatch);
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading URLs in entry payloads (with and without base URI).")]
        public void EntryReadingBaseUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            Action<EntityInstance, Uri, ReaderTestConfiguration>[] setUriActions = new Action<EntityInstance, Uri, ReaderTestConfiguration>[]
            {
                // Setting the read link of an entry
                (instance, uri, testConfig) => 
                    {
                        instance.SetAnnotation(new SelfLinkAnnotation(UriToString(uri)));
                    },
                // Setting the edit link of an entry
                (instance, uri, testConfig) => 
                    {
                        instance.WithEditLink(UriToString(uri));
                    },

                // Setting the navigation link of an entry
                (instance, uri, testConfig) => 
                    {
                        NavigationPropertyInstance navProperty = instance.GetProperty("CityHall") as NavigationPropertyInstance;
                        this.Assert.IsNotNull(navProperty, "Did not find expected navigation property 'CityHall'.");
                        DeferredLink deferredLink = navProperty.Value as DeferredLink;
                        this.Assert.IsNotNull(deferredLink, "Did not find expected deferred link.");
                        deferredLink.UriString = UriToString(uri);
                    },

                // Setting the association link of an entry
                (instance, uri, testConfig) => 
                    {
                        NavigationPropertyInstance navProperty = instance.GetProperty("CityHall") as NavigationPropertyInstance;
                        this.Assert.IsNotNull(navProperty, "Did not find expected navigation property 'CityHall'.");
                        DeferredLink deferredLink = navProperty.AssociationLink as DeferredLink;
                        this.Assert.IsNotNull(deferredLink, "Did not find expected assocation link.");
                        deferredLink.UriString = UriToString(uri);
                    },

                // Setting the read link of a stream property
                (instance, uri, testConfig) => 
                    {
                        NamedStreamInstance namedStream = instance.GetProperty("Skyline") as NamedStreamInstance;
                        this.Assert.IsNotNull(namedStream, "Did not find expected stream property 'Skyline'.");
                        namedStream.SourceLink = UriToString(uri);
                    },

                // Setting the edit link of a stream property
                (instance, uri, testConfig) => 
                    {
                        NamedStreamInstance namedStream = instance.GetProperty("Skyline") as NamedStreamInstance;
                        this.Assert.IsNotNull(namedStream, "Did not find expected stream property 'Skyline'.");
                        namedStream.EditLink = UriToString(uri);
                    },

                // Setting the read link of the default stream
                (instance, uri, testConfig) => 
                    {
                        instance.StreamSourceLink = UriToString(uri);
                    },

                // Setting the edit link of the default stream
                (instance, uri, testConfig) => 
                    {
                        instance.StreamEditLink = UriToString(uri);
                    },

                // TODO: add tests for operation links (not yet supported in the test infrastructure).
            };

            EntityInstance entry = PayloadBuilder.Entity("TestModel.CityWithMapType")
                .PrimitiveProperty("Id", 1)
                .Property(PayloadBuilder.NavigationProperty("CityHall", /*url*/ "http://odata.org/dummy", /*associationUrl*/ "http://odata.org/dummy").IsCollection(true))
                .StreamProperty("Skyline", "http://odata.org./dummy")
                .AsMediaLinkEntry()
                .StreamSourceLink("http://odata.org/dummy");

            this.CombinatorialEngineProvider.RunCombinations(
                payloadUris,
                baseUriValues,
                resolvers,
                setUriActions,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (payloadUri, baseUriValue, resolver, setUriAction, testConfiguration) =>
                {
                    this.CombinatorialEngineProvider.RunCombinations(
                        new bool[] { false, true },
                        runInBatch =>
                        {
                            this.RunBaseUriReadingTest(entry, setUriAction, model, payloadUri, baseUriValue, resolver, testConfiguration, runInBatch);
                        });
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading URLs in entity reference link payloads (with and without base URI).")]
        public void EntityReferenceLinkReadingBaseUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            DeferredLink link = PayloadBuilder.DeferredLink("http://odata.org/dummy");

            this.CombinatorialEngineProvider.RunCombinations(
                payloadUris,
                baseUriValues,
                resolvers,
                new bool[] { false, true },
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (payloadUri, baseUriValue, resolver, runInBatch, testConfiguration) =>
                {
                    Action<DeferredLink, Uri, ReaderTestConfiguration> setLinkAction =
                        (instance, uri, testConfig) => instance.UriString = UriToString(uri);
                    this.RunBaseUriReadingTest(link, setLinkAction, model, payloadUri, baseUriValue, resolver, testConfiguration, runInBatch);
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading URLs in entity reference links payloads (with and without base URI).")]
        public void EntityReferenceLinksReadingBaseUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            LinkCollection linkCollection = PayloadBuilder.LinkCollection();
            linkCollection.Add(PayloadBuilder.DeferredLink("http://odata.org/dummy"));
            linkCollection.Add(PayloadBuilder.DeferredLink("http://odata.org/dummy"));
            linkCollection.Add(PayloadBuilder.DeferredLink("http://odata.org/dummy"));

            this.CombinatorialEngineProvider.RunCombinations(
                payloadUris,
                baseUriValues,
                resolvers,
                new bool[] { false, true },
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (payloadUri, baseUriValue, resolver, runInBatch, testConfiguration) =>
                {
                    Action<LinkCollection, Uri, ReaderTestConfiguration> setLinkAction =
                        (links, uri, testConfig) => links[1].UriString = UriToString(uri);
                    this.RunBaseUriReadingTest(linkCollection, setLinkAction, model, payloadUri, baseUriValue, resolver, testConfiguration, runInBatch);
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading URLs in service document payloads (with and without base URI).")]
        public void ServiceDocumentReadingBaseUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            ServiceDocumentInstance serviceDocument = PayloadBuilder.ServiceDocument()
                .Workspace(PayloadBuilder.Workspace()
                    .ResourceCollection(/*title*/ null, /*href*/ null));

            // Run only on responses
            this.CombinatorialEngineProvider.RunCombinations(
                payloadUris.Where(uri => uri.OriginalString.Length != 0),
                baseUriValues,
                resolvers,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (payloadUri, baseUriValue, resolver, testConfiguration) =>
                {
                    Action<ServiceDocumentInstance, Uri, ReaderTestConfiguration> setCollectionHrefAction =
                        (instance, uri, testConfig) => instance.Workspaces[0].ResourceCollections[0].Href = UriToString(uri);
                    this.RunBaseUriReadingTest(serviceDocument, setCollectionHrefAction, model, payloadUri, baseUriValue, resolver, testConfiguration);
                });
        }

        // TODO: add base URI reading tests for ATOM metadata (contributor URI, author URI, atom:link)

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading null URLs in feed payloads (with and without base URI).")]
        public void FeedReadingNullUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            EntitySetInstance feed = PayloadBuilder.EntitySet();
            feed.NextLink = null;

            // Run only in version >= 2 since next links are only supported since v2 and not allowed in requests
            this.CombinatorialEngineProvider.RunCombinations(
                baseUriValues,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (baseUriValue, testConfiguration) =>
                {
                    // The next link should always be null independently of whether a base URI is specified or not
                    NullUriValueTestCase<EntitySetInstance> testCase = new NullUriValueTestCase<EntitySetInstance>
                    {
                        SetNullUriAction = (instance, uri, testConfig) => instance.NextLink = null,
                        SetExpectedUriAction = null     // will use SetNullUriAction instead
                    };
                        
                    this.RunNullUriReadingTest(feed, testCase, model, baseUriValue, testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading null URLs in entry payloads (with and without base URI).")]
        public void EntryReadingNullUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            NullUriValueTestCase<EntityInstance>[] testCases = new NullUriValueTestCase<EntityInstance>[]
            {
                // Setting the read link of an entry
                // NOTE: in JSON, the 'uri' property will be always omitted from __metadata when the read link is null (and no edit link exists);
                //       in ATOM, the self link will be omitted from the payload
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction = (instance, uri, testConfig) => instance.RemoveAnnotations(typeof(SelfLinkAnnotation)),
                },

                // Setting the edit link of an entry
                // NOTE: in JSON, the 'uri' property will be always omitted from __metadata when the edit link is null (and no read link exists);
                //       in ATOM, the link with the 'edit' rel will be omitted from the payload
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction = (instance, uri, testConfig) => instance.WithEditLink(null),
                },

                // Setting the navigation link of an entry
                // NOTE: in JSON, the 'uri' property in the __deferred object will have a 'null' value (and we expect an error)
                //       in ATOM, the 'related' link for the navigation property will not have the 'href' property.
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction =
                        (instance, uri, testConfig) => ((NavigationPropertyInstance)instance.GetProperty("CityHall")).Value = new DeferredLink(),
                },

                // Setting the association link of an entry
                // NOTE: in JSON, the associationuri property will have a 'null' value (and we expect an error)
                //       in ATOM, the 'relatedlinks' link will not have the 'href' property
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction =
                        (instance, uri, testConfig) => ((NavigationPropertyInstance)instance.GetProperty("CityHall")).AssociationLink = null,
                },
                
                // Setting the read link of a stream property
                // NOTE: in JSON, the 'media_src' property will be omitted from the __mediaresource object.
                //       in ATOM, the 'mediaresource' link of the stream property will be omitted from the payload.
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction =
                        (instance, uri, testConfig) =>  ((NamedStreamInstance)instance.GetProperty("Skyline")).SourceLink = null,
                },

                // Setting the edit link of a stream property
                // NOTE: in JSON, the 'edit_media' property will be omitted from the __mediaresource object.
                //       in ATOM, the 'edit-media' link of the stream property will be omitted from the payload.
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction =
                        (instance, uri, testConfig) =>  ((NamedStreamInstance)instance.GetProperty("Skyline")).EditLink = null,
                },

                // Setting the read link of the default stream
                // NOTE: in JSON, the 'media_src' property will be omitted from the __metadata object.
                //       in ATOM, the <content> element will be omitted from the payload.
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction = (instance, uri, testConfig) => instance.StreamSourceLink = null,
                },

                // Setting the edit link of the default stream
                // NOTE: in JSON, the 'edit_media' property will be omitted from the __metadata object.
                //       in ATOM, the edit-media link will be omitted from the payload.
                new NullUriValueTestCase<EntityInstance>
                {
                    SetNullUriAction = (instance, uri, testConfig) => instance.StreamEditLink = null,
                },
            };

            EntityInstance entry = PayloadBuilder.Entity("TestModel.CityWithMapType")
                .PrimitiveProperty("Id", 1)
                .Property(PayloadBuilder.NavigationProperty("CityHall", /*link*/ "http://odata.org/nav-prop").IsCollection(true))
                .StreamProperty("Skyline", /*readLink*/ "http://odata.org/stream-read", /*editLink*/ "http://odata.org/stream-edit")
                .AsMediaLinkEntry()
                .StreamSourceLink(/*link*/"http://odata.org/media-read");

            this.CombinatorialEngineProvider.RunCombinations(
                baseUriValues,
                testCases,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (baseUriValue, testCase, testConfiguration) =>
                {
                    this.RunNullUriReadingTest(entry, testCase, model, baseUriValue, testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading null URLs in entity reference link payloads (with and without base URI).")]
        public void EntityReferenceLinkReadingNullUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            DeferredLink link = PayloadBuilder.DeferredLink(/*uri*/ null);

            this.CombinatorialEngineProvider.RunCombinations(
                baseUriValues,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (baseUriValue, testConfiguration) =>
                {
                    // NOTE: In JSON, the value of the 'uri' property will be 'null' which is not allowed (and we expect an error)
                    //       In ATOM, the HREF of an entity reference link is stored as element content and will thus be read as 
                    //       string.Empty even if the source link was null
                    NullUriValueTestCase<DeferredLink> testCase = new NullUriValueTestCase<DeferredLink>
                    {
                        SetNullUriAction = (instance, uri, testConfig) => instance.UriString = null,
                        SetExpectedUriAction = (instance, uri, testConfig) => instance.UriString = UriToString(uri),
                    };

                    this.RunNullUriReadingTest(link, testCase, model, baseUriValue, testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading null URLs in entity reference links payloads (with and without base URI).")]
        public void EntityReferenceLinksReadingNullUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            LinkCollection linkCollection = PayloadBuilder.LinkCollection();
            linkCollection.Add(PayloadBuilder.DeferredLink(/*uri*/ null));
            linkCollection.Add(PayloadBuilder.DeferredLink(/*uri*/ null));
            linkCollection.Add(PayloadBuilder.DeferredLink(/*uri*/ null));

            this.CombinatorialEngineProvider.RunCombinations(
                baseUriValues,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (baseUriValue, testConfiguration) =>
                {
                    // NOTE: In JSON, the value of the 'uri' property will be 'null' which is not allowed (and we expect an error)
                    //       In ATOM, the HREF of an entity reference link is stored as element content and will thus be read as 
                    //       string.Empty even if the source link was null
                    NullUriValueTestCase<LinkCollection> testCase = new NullUriValueTestCase<LinkCollection>
                    {
                        SetNullUriAction = 
                            (links, uri, testConfig) => 
                            {
                                links[0].UriString = null;
                                links[1].UriString = null;
                                links[2].UriString = null;
                            },
                        SetExpectedUriAction = 
                            (links, uri, testConfig) =>
                            {
                                links[0].UriString = UriToString(uri);
                                links[1].UriString = UriToString(uri);
                                links[2].UriString = UriToString(uri);
                            }
                    };

                    this.RunNullUriReadingTest(linkCollection, testCase, model, baseUriValue, testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.UriHandling"), Variation(Description = "Verifies that readers behave properly when reading null URLs in service document payloads (with and without base URI).")]
        public void ServiceDocumentReadingNullUriTest()
        {
            EdmModel model = (EdmModel)Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            model.Fixup();

            ServiceDocumentInstance serviceDocument = PayloadBuilder.ServiceDocument()
                .Workspace(PayloadBuilder.Workspace()
                    .ResourceCollection(/*title*/ null, /*href*/ null));

            // Run only on responses
            this.CombinatorialEngineProvider.RunCombinations(
                baseUriValues,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (baseUriValue, testConfiguration) =>
                {
                    // NOTE: In JSON, collection names must not be null (and we expect an error)
                    //       In ATOM, the HREF of a resource collection is an attribute and will be missing if null; 
                    //       this is not allowed (and we expect an error)
                    NullUriValueTestCase<ServiceDocumentInstance> testCase = new NullUriValueTestCase<ServiceDocumentInstance>
                    {
                        SetNullUriAction = (instance, uri, testConfig) => instance.Workspaces[0].ResourceCollections[0].Href = null,
                        SetExpectedUriAction = (instance, uri, testConfig) => instance.Workspaces[0].ResourceCollections[0].Href = UriToString(uri),
                    };
                        
                    this.RunNullUriReadingTest(serviceDocument, testCase, model, baseUriValue, testConfiguration);
                });
        }

        // TODO: add null URI reading tests for ATOM metadata (contributor URI, author URI, atom:link)

        /// <summary>
        /// Converts the <paramref name="uri"/> to its string representation.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to convert.</param>
        /// <returns>The string representation of the <paramref name="uri"/>.</returns>
        private static string UriToString(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
        }

        /// <summary>
        /// Helper method to run a single iteration of the URI reading tests in a specified configuration.
        /// </summary>
        /// <typeparam name="T">The type of the payload to read.</typeparam>
        /// <param name="payloadElement">The payload to read.</param>
        /// <param name="setExpectedUriAction">An action to set the URI in question on the payload.</param>
        /// <param name="model">The metadata model.</param>
        /// <param name="payloadUri">The payload URI for the current iteration.</param>
        /// <param name="baseUriValue">The base URI value for the current iteration.</param>
        /// <param name="resolver">The resolver to use.</param>
        /// <param name="testConfiguration">The reader test configuration.</param>
        private void RunBaseUriReadingTest<T>(
            T payloadElement,
            Action<T, Uri, ReaderTestConfiguration> setExpectedUriAction,
            IEdmModel model,
            Uri payloadUri, 
            BaseUriValue baseUriValue, 
            KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri> resolver,
            ReaderTestConfiguration testConfiguration,
            bool runInBatch = false) where T : ODataPayloadElement
        {
            this.Assert.IsNull(testConfiguration.MessageReaderSettings.BaseUri, "No base URI expected on reader settings.");
            ExpectedException expectedException = null;

            Uri settingsBaseUri = baseUriValue.ReaderSettingBaseUri;

            // Set the base URI on the message reader settings if specified
            if (settingsBaseUri != null)
            {
                testConfiguration = new ReaderTestConfiguration(testConfiguration);
                testConfiguration.MessageReaderSettings.BaseUri = settingsBaseUri;
            }

            // Create the payload element
            T clonedPayloadElement = payloadElement.DeepCopy();
            setExpectedUriAction(clonedPayloadElement, payloadUri, testConfiguration);

            if (testConfiguration.Format == ODataFormat.Atom)
            {
                XElement xmlRepresentation = this.PayloadElementToXmlConverter.ConvertToXml(clonedPayloadElement);

                // add an xml:base attribute if specified
                Uri xmlBaseUri = baseUriValue.XmlBaseUri;
                if (xmlBaseUri != null)
                {
                    xmlRepresentation.Add(new XAttribute(XNamespace.Xml.GetName("base"), xmlBaseUri.OriginalString));
                }

                clonedPayloadElement.XmlRepresentation(xmlRepresentation);

                if (resolver.Value != null)
                {
                    setExpectedUriAction(clonedPayloadElement, resolver.Value, testConfiguration);
                }
                else
                {
                    // compute the expected URI value for ATOM
                    if (!payloadUri.IsAbsoluteUri)
                    {
                        if (xmlBaseUri != null)
                        {
                            setExpectedUriAction(clonedPayloadElement, new Uri(xmlBaseUri, payloadUri), testConfiguration);
                        }
                        else if (settingsBaseUri != null)
                        {
                            setExpectedUriAction(clonedPayloadElement, new Uri(settingsBaseUri, payloadUri), testConfiguration);
                        }
                        else
                        {
                            // fail for relative URIs without base URI
                            expectedException = ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", payloadUri.OriginalString);
                        }
                    }
                }
            }
            else
            {
                throw new NotSupportedException("Unsupported configuration format: " + testConfiguration.Format.ToString());
            }

            PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(resolver.Key == null ? this.Settings : this.NoValidatorSettings)
            {
                PayloadElement = clonedPayloadElement,
                PayloadEdmModel = model,
                ExpectedException = expectedException,
                UrlResolver = resolver.Key == null ? null : new TestUrlResolver { ResolutionCallback = (baseUri, realPayloadUri) => resolver.Key(payloadUri, baseUri, realPayloadUri) },
                SkipTestConfiguration = tc => ODataPayloadElementConfigurationValidator.GetSkipTestConfiguration(payloadElement, ODataPayloadElementConfigurationValidator.AllValidators)(tc),
            };

            if (runInBatch)
            {
                // TODO: Batch reader does not enter Exception state upon cross reference error in payload.
                // Once fixed allow the batch tests to run even for error cases.
                if (expectedException != null)
                {
                    return;
                }

                if (testConfiguration.IsRequest)
                {
                    testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                    {
                        PayloadElement =
                            PayloadBuilder.BatchRequestPayload(
                                BatchUtils.GetRequestChangeset(
                                    new IMimePart[] { testDescriptor.PayloadDescriptor.InRequestOperation(
                                        HttpVerb.Put, 
                                        new ODataUri(ODataUriBuilder.Root(new Uri("http://odata.org/service"))),
                                        this.RequestManager,
                                        TestMediaTypeUtils.GetDefaultContentType(testDescriptor.PayloadDescriptor.PayloadKind, testConfiguration.Format)) },
                                    this.RequestManager))
                            .AddAnnotation(new BatchBoundaryAnnotation("bb_request"))
                    };
                }
                else
                {
                    testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                    {
                        PayloadElement =
                            PayloadBuilder.BatchResponsePayload(
                                testDescriptor.PayloadDescriptor.InResponseOperation(
                                    200,
                                    this.RequestManager,
                                    TestMediaTypeUtils.GetDefaultContentType(testDescriptor.PayloadDescriptor.PayloadKind, testConfiguration.Format)))
                            .AddAnnotation(new BatchBoundaryAnnotation("bb_response"))
                    };
                }

                testConfiguration = new ReaderTestConfiguration(null, testConfiguration.MessageReaderSettings, testConfiguration.IsRequest, testConfiguration.Synchronous, testConfiguration.Version);
            }

            testDescriptor.RunTest(testConfiguration);
        }

        /// <summary>
        /// Helper method to run a single iteration of the URI reading tests in a specified configuration.
        /// </summary>
        /// <typeparam name="T">The type of the payload to read.</typeparam>
        /// <param name="payloadElement">The payload to read.</param>
        /// <param name="setExpectedUriAction">An action to set the URI in question on the payload.</param>
        /// <param name="model">The metadata model.</param>
        /// <param name="baseUriValue">The base URI value for the current iteration.</param>
        /// <param name="testConfiguration">The reader test configuration.</param>
        private void RunNullUriReadingTest<T>(
            T payloadElement,
            NullUriValueTestCase<T> testCase,
            IEdmModel model,
            BaseUriValue baseUriValue,
            ReaderTestConfiguration testConfiguration) where T : ODataPayloadElement
        {
            this.Assert.IsNotNull(testCase.SetNullUriAction, "SetNullUriAction must not be null.");
            this.Assert.IsNull(testConfiguration.MessageReaderSettings.BaseUri, "No base URI expected on reader settings.");

            ExpectedException expectedException = null;
            Uri settingsBaseUri = baseUriValue.ReaderSettingBaseUri;

            // Set the base URI on the message reader settings if specified
            if (settingsBaseUri != null)
            {
                testConfiguration = new ReaderTestConfiguration(testConfiguration);
                testConfiguration.MessageReaderSettings.BaseUri = settingsBaseUri;
            }

            // Create the payload element
            T clonedPayloadElement = payloadElement.DeepCopy();

            // modify the payload to represent the 'null' URI
            testCase.SetNullUriAction(clonedPayloadElement, /*uri*/ null, testConfiguration);

            var setExpectedUriAction = testCase.SetExpectedUriAction ?? testCase.SetNullUriAction;
            if (testConfiguration.Format == ODataFormat.Atom)
            {
                XElement xmlRepresentation = this.PayloadElementToXmlConverter.ConvertToXml(clonedPayloadElement);

                // add an xml:base attribute if specified
                Uri xmlBaseUri = baseUriValue.XmlBaseUri;
                if (xmlBaseUri != null)
                {
                    xmlRepresentation.Add(new XAttribute(XNamespace.Xml.GetName("base"), xmlBaseUri.OriginalString));
                }

                clonedPayloadElement.XmlRepresentation(xmlRepresentation);

                // First check all error conditions that are independent of a potential base URI
                if (payloadElement.ElementType == ODataPayloadElementType.ServiceDocumentInstance)
                {
                    // Resource collections must have a URL
                    expectedException = ODataExpectedExceptions.ODataException("ValidationUtils_ServiceDocumentElementUrlMustNotBeNull");
                }
                else
                {
                    if (xmlBaseUri != null)
                    {
                        setExpectedUriAction(clonedPayloadElement, xmlBaseUri, testConfiguration);

                    }
                    else if (settingsBaseUri != null)
                    {
                        setExpectedUriAction(clonedPayloadElement, settingsBaseUri, testConfiguration);
                    }
                    else
                    {
                        // fail for relative URIs without base URI
                        if (payloadElement.ElementType == ODataPayloadElementType.DeferredLink ||
                            payloadElement.ElementType == ODataPayloadElementType.LinkCollection)
                        {
                            expectedException = ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", string.Empty);
                        }
                    }

                    // NOTE: ATOM properly detects MLEs even without read or edit link
                }
            }
            else
            {
                throw new NotSupportedException("Unsupported configuration format: " + testConfiguration.Format.ToString());
            }

            PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
            {
                PayloadElement = clonedPayloadElement,
                PayloadEdmModel = model,
                ExpectedException = expectedException,
                SkipTestConfiguration = tc => ODataPayloadElementConfigurationValidator.GetSkipTestConfiguration(payloadElement, ODataPayloadElementConfigurationValidator.AllValidators)(tc),
            };

            testDescriptor.RunTest(testConfiguration);
        }

        /// <summary>
        /// A base URI configuration specifying a base URI to use on the Xml reader 
        /// and a base URI to use on the reader settings.
        /// </summary>
        private sealed class BaseUriValue
        {
            public Uri XmlBaseUri { get; set; }
            public Uri ReaderSettingBaseUri { get; set; }

            public override string ToString()
            {
                string xmlBase = this.XmlBaseUri == null ? "<null>" : this.XmlBaseUri.OriginalString;
                string settingsBase = this.ReaderSettingBaseUri == null ? "<null>" : this.ReaderSettingBaseUri.OriginalString;
                return "XmlBaseUri: " + xmlBase + "; SettingsBaseUri: " + settingsBase;
            }
        }

        /// <summary>
        /// Class representing a test case.
        /// </summary>
        private sealed class BaseUriValueTestCase
        {
            public ODataPayloadElement PayloadElement { get; set; }
            public Uri ReaderSettingBaseUri { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        private sealed class NullUriValueTestCase<T>
        {
            public Action<T, Uri, ReaderTestConfiguration> SetNullUriAction { get; set; }
            public Action<T, Uri, ReaderTestConfiguration> SetExpectedUriAction { get; set; }
        }
    }
}