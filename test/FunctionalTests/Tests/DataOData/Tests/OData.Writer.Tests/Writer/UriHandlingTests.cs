//---------------------------------------------------------------------
// <copyright file="UriHandlingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Tests.WriterTests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests to verify correct handling of URIs in the writer
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    // [TestClass, TestCase]
    public class UriHandlingTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public BatchWriterTestDescriptor.Settings BatchSettings { get; set; }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that relative Uris are only allowed when a base Uri is specified.")]
        public void BaseUriErrorTest()
        {
            Uri baseUri = new Uri("http://odata.org");
            Uri testUri = new Uri("http://odata.org/relative");
            IEnumerable<Func<Uri, BaseUriErrorTestCase>> testCaseFuncs = new Func<Uri, BaseUriErrorTestCase>[]
            {
                relativeUri => new BaseUriErrorTestCase
                {   // next page link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                        feed.NextPageLink = relativeUri;
                        return new [] { feed };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // entry read link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        entry.ReadLink = relativeUri;
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // entry edit link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        entry.EditLink = relativeUri;
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // media resource (default stream) read link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();
                        mediaResource.ContentType = "image/jpg";
                        mediaResource.ReadLink = relativeUri;
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        entry.MediaResource = mediaResource;
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // media resource (default stream) edit link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();
                        mediaResource.ContentType = "image/jpg";    // required
                        mediaResource.ReadLink = testUri;           // required
                        mediaResource.EditLink = relativeUri;
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        entry.MediaResource = mediaResource;
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // link Url
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataNestedResourceInfo link = ObjectModelUtils.CreateDefaultCollectionLink();
                        link.Url = relativeUri;

                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        return new ODataItem[] { entry, link };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // association link Url
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataNestedResourceInfo link = ObjectModelUtils.CreateDefaultSingletonLink();
                        link.AssociationLinkUrl = relativeUri;

                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        return new ODataItem[] { entry, link };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // named stream read link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataStreamReferenceValue namedStream = new ODataStreamReferenceValue()
                        {
                            ContentType = "image/jpg",
                            ReadLink = relativeUri,
                        };
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        ODataProperty property = new ODataProperty()
                        {
                            Name = "NamedStream",
                            Value = namedStream
                        };

                        entry.Properties = new[] { property };
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // named stream edit link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataStreamReferenceValue namedStream = new ODataStreamReferenceValue()
                        {
                            ContentType = "image/jpg",
                            ReadLink = testUri,
                            EditLink = relativeUri
                        };
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        ODataProperty property = new ODataProperty()
                        {
                            Name = "NamedStream",
                            Value = namedStream
                        };

                        entry.Properties = new[] { property };
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: feed generator Uri
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                        return new [] { feed };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: feed logo
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                        return new [] { feed };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: feed icon
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                        return new [] { feed };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: feed author
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                        return new [] { feed };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: feed contributor
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                        return new [] { feed };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: feed link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                        return new [] { feed };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: entry author
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: entry contributor
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
                relativeUri => new BaseUriErrorTestCase
                {   // Atom metadata: entry link
                    ItemFunc = new Func<IEnumerable<ODataItem>>(() =>
                    {
                        ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                        return new [] { entry };
                    }),
                    Formats = new [] { ODataFormat.Json }
                },
            };

            // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            Uri testRelativeUri = baseUri.MakeRelativeUri(testUri);
            Uri invalidRelativeUri = new Uri("../invalid/relative/uri", UriKind.Relative);
            this.CombinatorialEngineProvider.RunCombinations(
                testCaseFuncs,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(c => false),
                new Uri[] { testRelativeUri, invalidRelativeUri },
                new bool[] { false, true },
                (testCaseFunc, testConfiguration, uri, implementUrlResolver) =>
                {
                    var testCase = testCaseFunc(uri);
                    var testDescriptor = new
                    {
                        Descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                            this.Settings,
                            testCase.ItemFunc(),
                            testConfig => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException2 = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", uri.OriginalString)
                            }),
                        Formats = testCase.Formats
                    };

                    if (testDescriptor.Formats.Contains(testConfiguration.Format))
                    {
                        PayloadWriterTestDescriptor<ODataItem> payloadTestDescriptor = testDescriptor.Descriptor;
                        TestUrlResolver urlResolver = null;
                        if (implementUrlResolver)
                        {
                            payloadTestDescriptor = new PayloadWriterTestDescriptor<ODataItem>(payloadTestDescriptor);
                            urlResolver = new TestUrlResolver();
                            payloadTestDescriptor.UrlResolver = urlResolver;
                        }

                        testConfiguration = testConfiguration.Clone();
                        testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                        TestWriterUtils.WriteAndVerifyODataPayload(payloadTestDescriptor, testConfiguration, this.Assert, this.Logger);

                        if (implementUrlResolver)
                        {
                            this.Assert.AreEqual(1, urlResolver.Calls.Where(call => call.Value.OriginalString == uri.OriginalString).Count(), "The resolver should be called exactly once for each URL.");
                        }
                    }
                });
        }

        IEnumerable<UriTestCase> uriTestCases = new UriTestCase[]
        {
            new UriTestCase
            {   // next page link
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    feed.NextPageLink = url;
                    return new [] { feed };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object().PropertyValue("__next"),
                ResponseOnly = true
            },
            new UriTestCase
            {   // entry read link
                ItemFunc = (url) =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.ReadLink = url;
                    return new [] { entry };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object().PropertyObject("__metadata").PropertyValue("uri"),
            },
            new UriTestCase
            {   // entry edit link
                ItemFunc = (url) =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.EditLink = url;
                    return new [] { entry };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object().PropertyObject("__metadata").PropertyValue("uri"),
            },
            new UriTestCase
            {   // media resource (default stream) read link
                ItemFunc = (url) =>
                {
                    ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();
                    mediaResource.ContentType = "image/jpg";
                    mediaResource.ReadLink = url;
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.MediaResource = mediaResource;
                    return new [] { entry };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object().PropertyObject("__metadata").PropertyValue("media_src"),
            },
            new UriTestCase
            {   // media resource (default stream) edit link
                ItemFunc = (url) =>
                {
                    ODataStreamReferenceValue mediaResource = new ODataStreamReferenceValue();
                    mediaResource.ContentType = "image/jpg";    // required
                    mediaResource.ReadLink = url;           // required
                    mediaResource.EditLink = url;
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.MediaResource = mediaResource;
                    return new [] { entry };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object().PropertyObject("__metadata").PropertyValue("edit_media"),
            },
            new UriTestCase
            {   // navigation link Url
                ItemFunc = (url) =>
                {
                    ODataNestedResourceInfo link = ObjectModelUtils.CreateDefaultSingletonLink();
                    link.Url = url;

                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    return new ODataItem[] { entry, link };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object().PropertyObject("SampleLinkName").PropertyObject(tc.IsRequest ? "__metadata" : "__deferred").PropertyValue("uri"),
            },
            new UriTestCase
            {   // association link Url
                ItemFunc = (url) =>
                {
                    ODataNestedResourceInfo link = ObjectModelUtils.CreateDefaultSingletonLink();
                    link.AssociationLinkUrl = url;
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    return new ODataItem[] { entry, link };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object().PropertyObject("__metadata").PropertyObject("properties").PropertyObject("SampleLinkName").PropertyValue("associationuri"),
                // Association links are not allowed in requests.
                ResponseOnly = true,
            },
            new UriTestCase
            {   // named stream read link
                ItemFunc = (url) =>
                {
                    ODataStreamReferenceValue namedStream = new ODataStreamReferenceValue()
                    {
                        ContentType = "image/jpg",
                        ReadLink = url,
                    };
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    ODataProperty property = new ODataProperty()
                    {
                        Name = "NamedStream",
                        Value = namedStream
                    };

                    entry.Properties = new[] { property };
                    return new [] { entry };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object()
                    .PropertyObject("NamedStream")
                    .PropertyObject("__mediaresource")
                    .PropertyValue("media_src"),
                // Association links are not allowed in requests.
                ResponseOnly = true,

            },
            new UriTestCase
            {   // named stream edit link
                ItemFunc = (url) =>
                {
                    ODataStreamReferenceValue namedStream = new ODataStreamReferenceValue()
                    {
                        ContentType = "image/jpg",
                        ReadLink = url,
                        EditLink = url
                    };
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    ODataProperty property = new ODataProperty()
                    {
                        Name = "NamedStream",
                        Value = namedStream
                    };

                    entry.Properties = new[] { property };
                    return new [] { entry };
                },
                JsonExtractor = (tc, json) => JsonUtils.UnwrapTopLevelValue(tc, json).Object()
                    .PropertyObject("NamedStream")
                    .PropertyObject("__mediaresource")
                    .PropertyValue("edit_media"),
                // Named streams fail in requests
                ResponseOnly = true,
            },
            new UriTestCase
            {   // Atom metadata: feed generator Uri
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    return new [] { feed };
                },
                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: feed logo
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    return new [] { feed };
                },
                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: feed icon
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    return new [] { feed };
                },
                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: feed author
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    return new [] { feed };
                },
                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: feed contributor
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();

                    return new [] { feed };
                },
                AtomExtractor = (tc, element) => new XElement("uri", element.Element(TestAtomConstants.AtomXNamespace + "contributor").Element(TestAtomConstants.AtomXNamespace + "uri").Value),
                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: feed self link
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    return new [] { feed };
                },
                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: feed next page link
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    feed.NextPageLink = url;
                    return new [] { feed };
                },
                JsonExtractor = null,
                ResponseOnly = true
            },
            new UriTestCase
            {   // Atom metadata: feed link
                ItemFunc = (url) =>
                {
                    ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                    return new [] { feed };
                },
                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: entry author
                ItemFunc = (url) =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    return new [] { entry };
                },

                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: entry contributor
                ItemFunc = (url) =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    return new [] { entry };
                },

                JsonExtractor = null,
            },
            new UriTestCase
            {   // Atom metadata: entry link
                ItemFunc = (url) =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    return new [] { entry };
                },
                JsonExtractor = null,
            },
        };

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that realtive Uris are allowed (if a base URI exists) and written in an escaped form.")]
        public void RelativeUriTest()
        {
            var testCases = new[]
            {
                new
                {
                    BaseUri = new Uri("http://odata.org/"),
                    RelativeUri = new Uri ("relative", UriKind.Relative),
                },
                new
                {
                    BaseUri = new Uri("http://odata.org/"),
                    RelativeUri = new Uri ("relative that needs escaping", UriKind.Relative),
                },
                new
                {
                    BaseUri = new Uri("http://odata.org/a/b/"),
                    RelativeUri = new Uri ("../../relative that needs escaping", UriKind.Relative),
                },
            };

            var testDescriptors = uriTestCases.SelectMany(uriTestCase => testCases.Select(testCase =>
            {
                return new
                {
                    TestCase = uriTestCase,
                    BaseUri = testCase.BaseUri,
                    Descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        uriTestCase.ItemFunc(testCase.RelativeUri),
                        CreateUriTestCaseExpectedResultCallback(testCase.BaseUri, testCase.RelativeUri, uriTestCase))
                };
            }));

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(c => false),
                new bool[] { false, true },
                (testDescriptor, testConfiguration, implementUrlResolver) =>
                {
                    if (testConfiguration.Format == ODataFormat.Json && testDescriptor.TestCase.JsonExtractor != null)
                    {
                        PayloadWriterTestDescriptor<ODataItem> payloadTestDescriptor = testDescriptor.Descriptor;
                        if (implementUrlResolver)
                        {
                            payloadTestDescriptor = new PayloadWriterTestDescriptor<ODataItem>(payloadTestDescriptor);
                            payloadTestDescriptor.UrlResolver = new TestUrlResolver();
                        }

                        testConfiguration = testConfiguration.Clone();
                        testConfiguration.MessageWriterSettings.BaseUri = testDescriptor.BaseUri;
                        testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                        TestWriterUtils.WriteAndVerifyODataPayload(payloadTestDescriptor, testConfiguration, this.Assert, this.Logger);
                    }
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that absolute Uris are allowed and written as-is (not made relative).")]
        public void AbsoluteUriTest()
        {
            Uri[] baseUris = new Uri[]
            {
                null,
                // Standard
                new Uri("http://odata.org/"),
                new Uri("http://second.odata.org/"),                                
                // Reserved Characters
                new Uri("http://odata.org/MyC%2B%2BService.svc/"), // +                
                new Uri("http://odata.org/My%24Service.svc/"), // $                
                new Uri("http://odata.org/My%26Service.svc/"), // &
                new Uri("http://odata.org/My%2FService.svc/"), // /
                new Uri("http://odata.org/My%3DService.svc/"), // =                                                                        
                new Uri("http://odata.org/My%3FService.svc/"), // ?                                       
                new Uri("http://odata.org/My%2CService.svc/"), // ,          
                new Uri("http://odata.org/My%3AService.svc/"), // :
                new Uri("http://odata.org/My%40Service.svc/"), // @       
                //Unsafe Characters
                new Uri("http://odata.org/My%20Service.svc/"), // space
                new Uri("http://odata.org/My%22Service.svc/"), // "
                new Uri("http://odata.org/My%23Service.svc/"), // #
                new Uri("http://odata.org/My%25Service.svc/"), // %
                new Uri("http://odata.org/My%5CService.svc/"), // \
                new Uri("http://odata.org/My%7EService.svc/"), // ~
                new Uri("http://odata.org/My%5EService.svc/"), // ^
                new Uri("http://odata.org/My%5BService.svc/"), // [
                new Uri("http://odata.org/My%5DService.svc/"), // ]
                new Uri("http://odata.org/My%60Service.svc/"), // `
                new Uri("http://odata.org/My%7CService.svc/"), // |
                new Uri("http://odata.org/My%7BService.svc/"), // {
                new Uri("http://odata.org/My%7DService.svc/"), // }
                // Others                         
                new Uri("http://odata.org:80/MyService.svc/"),
                new Uri("http://odata.org/My_Service.svc/"),
                new Uri("http://odata.org/My-Service.svc/"),
                new Uri("http://odata.org/My31572Service.svc/"),
            };

            Uri[] testUris = new Uri[]
            {
                new Uri("http://odata.org/testuri"),
                new Uri("http://odata.org/testuri?$filter=3.14E%2B%20ne%20null"),
                new Uri("http://odata.org/testuri?$filter='foo%20%26%20'%20ne%20null"),
                new Uri("http://odata.org/testuri?$filter=not%20endswith(Name,'%2B')"),
                new Uri("http://odata.org/testuri?$filter=geo.distance(Point,%20geometry'SRID=0;Point(6.28E%2B3%20-2.1e%2B4)')%20eq%20null"),
            };

            var testDescriptors = uriTestCases
                .SelectMany(testCase => testUris
                    .SelectMany(testUri => baseUris
                        .Select(baseUri =>
           {
               return new
               {
                   TestCase = testCase,
                   BaseUri = baseUri,
                   Descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                       this.Settings,
                       testCase.ItemFunc(testUri),
                       CreateUriTestCaseExpectedResultCallback(baseUri, testUri, testCase))
               };
           })));

            // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(c => false),
                new bool[] { false, true },
                (testDescriptor, testConfiguration, implementUrlResolver) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testConfiguration.Format == ODataFormat.Json && testDescriptor.TestCase.JsonExtractor != null)
                    {
                        PayloadWriterTestDescriptor<ODataItem> payloadTestDescriptor = testDescriptor.Descriptor;
                        if (implementUrlResolver)
                        {
                            payloadTestDescriptor = new PayloadWriterTestDescriptor<ODataItem>(payloadTestDescriptor);
                            payloadTestDescriptor.UrlResolver = new TestUrlResolver();
                        }

                        TestWriterUtils.WriteAndVerifyODataPayload(payloadTestDescriptor, testConfiguration, this.Assert, this.Logger);
                    }
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that custom URL resolver works as expected.")]
        public void ResolverUriTest()
        {
            Uri inputUri = new Uri("inputUri", UriKind.Relative);
            Uri resultRelativeUri = new Uri("resultRelativeUri", UriKind.Relative);
            Uri resultAbsoluteUri = new Uri("http://odata.org/absoluteresolve");

            var resolvers = new[]
                {
                    // Resolver which always returns relative URL
                    new
                    {
                        Resolver = new Func<Uri, Uri, Uri>((baseUri, payloadUri) => {
                            if (payloadUri.OriginalString == inputUri.OriginalString)
                            {
                                return resultRelativeUri;
                            }
                            else
                            {
                                return null;
                            }}),
                        ResultUri = resultRelativeUri
                    },
                    // Resolver which always returns absolute URL
                    new
                    {
                        Resolver = new Func<Uri, Uri, Uri>((baseUri, payloadUri) => {
                            if (payloadUri.OriginalString == inputUri.OriginalString)
                            {
                                return resultAbsoluteUri;
                            }
                            else
                            {
                                return null;
                            }}),
                        ResultUri = resultAbsoluteUri
                    }
                };

            var testDescriptors = uriTestCases.SelectMany(testCase => resolvers.Select(resolver =>
            {
                return new
                {
                    TestCase = testCase,
                    Descriptor = new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        testCase.ItemFunc(inputUri),
                        CreateUriTestCaseExpectedResultCallback(/*baseUri*/ null, resolver.ResultUri, testCase))
                    {
                        UrlResolver = new TestUrlResolver() { ResolutionCallback = resolver.Resolver }
                    }
                };
            }));

            // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                new bool[] { false, true },
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(c => false),
                (testDescriptor, runInBatch, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if ((!testConfiguration.IsRequest || !testDescriptor.TestCase.ResponseOnly) &&
                        (testConfiguration.Format == ODataFormat.Json && testDescriptor.TestCase.JsonExtractor != null))
                    {
                        var td = testDescriptor.Descriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration);
                        if (!runInBatch)
                        {
                            TestWriterUtils.WriteAndVerifyODataPayload(td, testConfiguration, this.Assert, this.Logger);
                        }
                        else
                        {
                            testConfiguration = testConfiguration.Clone();
                            testConfiguration.MessageWriterSettings.EnableMessageStreamDisposal = true;
                            var batchDescriptor = new List<BatchWriterTestDescriptor.InvocationAndOperationDescriptor>();
                            if (testConfiguration.IsRequest)
                            {
                                batchDescriptor.Add(BatchWriterUtils.StartBatch());
                                batchDescriptor.Add(BatchWriterUtils.StartChangeSet());
                                batchDescriptor.Add(BatchWriterUtils.ChangeSetRequest(
                                    "PUT",
                                    new Uri("http://odata.org"),
                                    null,
                                    null,
                                    new BatchWriterUtils.ODataPayload()
                                    {
                                        Items = td.PayloadItems.ToArray(),
                                        WriterTestExpectedResults = td.ExpectedResultCallback(testConfiguration),
                                        TestConfiguration = testConfiguration
                                    }));
                                batchDescriptor.Add(BatchWriterUtils.EndChangeSet());
                                batchDescriptor.Add(BatchWriterUtils.EndBatch());
                            }
                            else
                            {
                                batchDescriptor.Add(BatchWriterUtils.StartBatch());
                                batchDescriptor.Add(BatchWriterUtils.QueryOperationResponse(
                                    200,
                                    new BatchWriterUtils.ODataPayload()
                                    {
                                        Items = td.PayloadItems.ToArray(),
                                        WriterTestExpectedResults = td.ExpectedResultCallback(testConfiguration),
                                        TestConfiguration = testConfiguration
                                    }));
                                batchDescriptor.Add(BatchWriterUtils.EndBatch());
                            }

                            var batchTd = new BatchWriterTestDescriptor(
                                this.BatchSettings,
                                batchDescriptor.ToArray(),
                                (Dictionary<string, string>)null,
                                new Uri("http://odata.org/service"),
                                td.UrlResolver);

                            ODataMessageWriterSettings batchWriterSettings = testConfiguration.MessageWriterSettings.Clone();
                            batchWriterSettings.SetContentType(null);
                            WriterTestConfiguration batchTestConfiguration = new WriterTestConfiguration(
                                null,
                                batchWriterSettings,
                                testConfiguration.IsRequest,
                                testConfiguration.Synchronous);
                            BatchWriterUtils.WriteAndVerifyBatchPayload(batchTd, batchTestConfiguration, testConfiguration, this.Assert);
                        }
                    }
                });
        }

        /// <summary>
        ///  Helper method to create the expected result callback for a URI test.
        /// </summary>
        /// <param name="uri">The URI that is the expected result.</param>
        /// <param name="testCase">The <see cref="UriTestCase"/> to create the result callback for.</param>
        /// <returns>The expected result callback for a URI test.</returns>
        private PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateUriTestCaseExpectedResultCallback(
            Uri baseUri,
            Uri uri,
            UriTestCase testCase)
        {
            return (testConfiguration) =>
            {
                if (testConfiguration.Format == ODataFormat.Json)
                {
                    // In JSON we always expect an absolute URI
                    string absoluteEscapedUri = TestUriUtils.ToAbsoluteUriString(uri, baseUri);
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = "\"" + absoluteEscapedUri + "\"",
                        FragmentExtractor = json => testCase.JsonExtractor(testConfiguration, json)
                    };
                }
                else
                {
                    throw new NotSupportedException("Unsupported format " + testConfiguration.Format.ToString() + " found.");
                }
            };
        }

        /// <summary>
        /// Class representing a test case to verify that URIs are written correctly.
        /// </summary>
        private sealed class UriTestCase
        {
            public Func<Uri, IEnumerable<ODataItem>> ItemFunc { get; set; }
            public Func<WriterTestConfiguration, XElement, XElement> AtomExtractor { get; set; }
            public Func<WriterTestConfiguration, JsonValue, JsonValue> JsonExtractor { get; set; }
            public bool ResponseOnly { get; set; }
        }

        private sealed class BaseUriErrorTestCase
        {
            public Func<IEnumerable<ODataItem>> ItemFunc { get; set; }
            public ODataFormat[] Formats { get; set; }
        }
    }
}
