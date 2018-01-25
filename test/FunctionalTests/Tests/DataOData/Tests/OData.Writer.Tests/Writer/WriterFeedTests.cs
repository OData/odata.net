//---------------------------------------------------------------------
// <copyright file="WriterFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Fixups;
    using Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing feeds with the OData writer.
    /// </summary>
    [TestClass, TestCase]
    public class WriterFeedTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://www.odata.org/");

        [InjectDependency(IsRequired = true)]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test feed payloads.")]
        public void TaupoTopLevelFeedTest()
        {
            EntitySetInstance entitySet = new EntitySetInstance(PayloadGenerator.GenerateAtomPayloads().First())
                .WithDefaultAtomFeedAnnotations();

            this.CombinatorialEngineProvider.RunCombinations(
                new[] { entitySet },
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testCase, testConfiguration) =>
                {
                    WriterTestConfiguration newConfiguration = testConfiguration.Clone();
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    this.WriteAndVerifyODataPayloadElement(testCase, newConfiguration);
                });

            // The ID annotation is added for JSON as even though JSON has no way to represent the ID ODataLib requires it.
            entitySet = new EntitySetInstance(PayloadGenerator.GenerateJsonPayloads().ToArray()).WithDefaultAtomIDAnnotation();

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            //this.CombinatorialEngineProvider.RunCombinations(
            //    new[] { entitySet },
            //    this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
            //    (testCase, testConfiguration) =>
            //    {
            //        this.WriteAndVerifyODataPayloadElement(testCase, testConfiguration);
            //    });
        }

        [TestMethod, Variation(Description = "Test feed payloads.")]
        public void FeedTests()
        {
            PayloadWriterTestDescriptor<ODataItem>[] testPayloads = new[]
                {
                    this.CreateDefaultFeedMetadataDescriptor(/*withModel*/false),
                    this.CreateDefaultFeedMetadataDescriptor(/*withModel*/true),
                    this.CreateDefaultFeedWithAtomMetadataDescriptor(/*withModel*/false),
                    this.CreateDefaultFeedWithAtomMetadataDescriptor(/*withModel*/true),
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testPayloads.PayloadCases(WriterPayloads.FeedPayloads),
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    if (testDescriptor.IsGeneratedPayload && (testConfiguration.Format == ODataFormat.Json || testDescriptor.Model != null))
                    {
                        return;
                    }

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test writing feed payloads with user exceptions being thrown at various points.")]
        public void FeedUserExceptionTests()
        {
            IEdmModel edmModel = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            ODataResourceSet cityFeed = ObjectModelUtils.CreateDefaultFeed();
            ODataResource cityEntry = ObjectModelUtils.CreateDefaultEntry("TestModel.CityType");
            ODataNestedResourceInfo cityHallCollectionLink = ObjectModelUtils.CreateDefaultCollectionLink("CityHall");
            ODataNestedResourceInfo policeStationSingletonLink = ObjectModelUtils.CreateDefaultSingletonLink("PoliceStation");
            ODataResourceSet officeFeed = ObjectModelUtils.CreateDefaultFeed();
            ODataResource officeEntry = ObjectModelUtils.CreateDefaultEntry("TestModel.OfficeType");

            var container = edmModel.FindEntityContainer("DefaultContainer");
            var citySet = container.FindEntitySet("Cities") as EdmEntitySet;
            var cityType = edmModel.FindType("TestModel.CityType") as EdmEntityType;

            ODataItem[] writerPayload = new ODataItem[]
            {
                cityFeed,
                    cityEntry,
                    null,
                    cityEntry,
                        cityHallCollectionLink,
                            officeFeed,
                                officeEntry,
                                null,
                            null,
                        null,
                    null,
                    cityEntry,
                    null,
                    cityEntry,
                        policeStationSingletonLink,
                            officeEntry,
                            null,
                        null,
                    null,
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = new PayloadWriterTestDescriptor<ODataItem>[]
            {
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    writerPayload,
                    tc => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException = new Exception("User code triggered an exception."),
                    }
                )
                {
                    Model = edmModel,
                    PayloadEdmElementContainer = citySet,
                    PayloadEdmElementType = cityType,
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.FeedPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    if (testDescriptor.IsGeneratedPayload && (testConfiguration.Format == ODataFormat.Json || testDescriptor.Model != null))
                    {
                        return;
                    }

                    foreach (int throwUserExceptionAt in Enumerable.Range(0, testDescriptor.PayloadItems.Count + 1))
                    {
                        var configuredTestDescriptor = new PayloadWriterTestDescriptor<ODataItem>(this.Settings, testDescriptor.PayloadItems, testDescriptor.ExpectedResultCallback)
                        {
                            Model = testDescriptor.Model,
                            PayloadEdmElementContainer = testDescriptor.PayloadEdmElementContainer,
                            PayloadEdmElementType = testDescriptor.PayloadEdmElementType,
                            ThrowUserExceptionAt = throwUserExceptionAt,
                        };

                        TestWriterUtils.WriteAndVerifyODataEdmPayload(configuredTestDescriptor, testConfiguration, this.Assert, this.Logger);
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test feed payloads with next links.")]
        public void FeedNextLinkTests()
        {
            PayloadWriterTestDescriptor<ODataItem>[] testPayloads = this.CreateFeedNextLinkDescriptors();

            this.CombinatorialEngineProvider.RunCombinations(
                testPayloads.PayloadCases(WriterPayloads.FeedPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    if (testDescriptor.IsGeneratedPayload && (testConfiguration.Format == ODataFormat.Json || testDescriptor.Model != null))
                    {
                        return;
                    }

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test feed payloads with next link value set after writing Feed Start.")]
        public void SetNextLinkAfterResourceSetStartTests()
        {
            var testPayloads = this.CreateFeedNextLinkDescriptors().ToArray();
            foreach (var descriptor in testPayloads)
            {
                // Replace each feed with one without the next link value, and an action to write it back
                // after writing Feed Start.
                var nextLink = new Uri(descriptor.PayloadItems.OfType<ODataResourceSet>().Single().NextPageLink.OriginalString);
                var newFeed = ObjectModelUtils.CreateDefaultFeed().WithAnnotation(new WriteFeedCallbacksAnnotation { AfterWriteStartCallback = (f) => f.NextPageLink = nextLink });
                descriptor.PayloadItems = new ReadOnlyCollection<ODataItem>(new List<ODataItem> { newFeed });
            }

            this.CombinatorialEngineProvider.RunCombinations(
                testPayloads.PayloadCases(WriterPayloads.FeedPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    if (testDescriptor.IsGeneratedPayload && (testConfiguration.Format == ODataFormat.Json || testDescriptor.Model != null))
                    {
                        return;
                    }

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test feed payloads with inline count.")]
        public void FeedInlineCountTests()
        {
            PayloadWriterTestDescriptor<ODataItem>[] testDescriptors = this.CreateFeedQueryCountDescriptors();

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test feeds with invalid content.")]
        public void FeedInvalidContentTests()
        {
            ODataResourceSet defaultFeed = ObjectModelUtils.CreateDefaultFeed();

            ODataItem[] invalidPayload1 = new ODataItem[] { defaultFeed, defaultFeed };

            var testCases = new[]
                {
                    new
                    {
                        Items = invalidPayload1,
                        ExpectedError = "Cannot transition from state 'ResourceSet' to state 'ResourceSet'. The only valid action in state 'ResourceSet' is to write a resource."
                    }
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                ODataFormatUtils.ODataFormats.Where(f => f != null),

                // Async  test configuration is not supported for Phone and Silverlight
#if !SILVERLIGHT && !WINDOWS_PHONE
                new bool[] { false, true },
#else
                new bool[] {true},
#endif
                (testCase, format, synchronous) =>
                {
                    using (var memoryStream = new TestStream())
                    {
                        ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
                        settings.Version = ODataVersion.V4;
                        settings.SetServiceDocumentUri(ServiceDocumentUri);

                        using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, new WriterTestConfiguration(format, settings, false, synchronous), this.Assert))
                        {
                            ODataWriter writer = messageWriter.CreateODataWriter(isFeed: true);
                            TestExceptionUtils.ExpectedException<ODataException>(
                                this.Assert,
                                () => TestWriterUtils.WritePayload(messageWriter, writer, true, testCase.Items),
                                testCase.ExpectedError);
                        }
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test feeds with payloads combining inline count, next link and element type.")]
        public void WriteFeedTests()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            var payloadDescriptors = Test.OData.Utils.ODataLibTest.TestFeeds.GetFeeds(model, true);
            var testDescriptors = new List<PayloadWriterTestDescriptor>();

            foreach (var payloadDescriptor in payloadDescriptors)
            {
                var payload = payloadDescriptor.PayloadElement;
                testDescriptors.Add(new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, payload)
                {
                    PayloadDescriptor = payloadDescriptor,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = payload,
                        };
                    },
                });
            }

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            ////this.CombinatorialEngineProvider.RunCombinations(
            ////   testDescriptors,
            ////   this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
            ////   (testDescriptor, testConfiguration) =>
            ////   {
            ////       testDescriptor.RunTest(testConfiguration, this.Logger);
            ////   });

            testDescriptors.Clear();
            foreach (var payloadDescriptor in payloadDescriptors)
            {
                var payload = payloadDescriptor.PayloadElement;
                payload.Accept(new AddFeedIDFixup());
                testDescriptors.Add(new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, payload)
                {
                    PayloadDescriptor = payloadDescriptor,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = payload,
                        };
                    },
                });
            }

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
               (testDescriptor, testConfiguration) =>
               {
                   testConfiguration = testConfiguration.Clone();
                   testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                   testDescriptor.RunTest(testConfiguration, this.Logger);
               });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests various error cases for feeds.")]
        public void ODataFeedWriterErrorTests()
        {
            var feed = new EntitySetInstance(
                new EntityInstance("TestModel.Type1", false)
                {
                    Properties = new List<PropertyInstance>()
                    {
                        new PrimitiveProperty("StringProp", "Edm.String", "hello"),
                        new PrimitiveProperty("IntProp", "Edm.Int32", 5)
                    }
                },
                new EntityInstance("TestModel.Type2", false)
                {
                    Properties = new List<PropertyInstance>()
                    {
                        new PrimitiveProperty("GuidProp", "Edm.Guid", Guid.NewGuid()),
                        new PrimitiveProperty("BooleanProp", "Edm.Boolean", true)
                    }
                }
            ).WithDefaultAtomIDAnnotation();

            List<PayloadWriterTestDescriptor<ODataPayloadElement>> testDescriptors = new List<PayloadWriterTestDescriptor<ODataPayloadElement>>()
            {
                // Write feed with inline count on a request.
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    PayloadElement = new EntitySetInstance(){ InlineCount = 0 }.WithDefaultAtomIDAnnotation(),
                    SkipTestConfiguration = (tc) => !tc.IsRequest,
                    ExpectedResultCallback = (tc) => new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ODataWriterCore_QueryCountInRequest")
                    }
                },
                // Write feed with next link on a request.
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    PayloadElement = new EntitySetInstance(){ NextLink = "http://www.odata.org"}.WithDefaultAtomIDAnnotation(),
                    SkipTestConfiguration = (tc) => !tc.IsRequest,
                    ExpectedResultCallback = (tc) => new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("WriterValidationUtils_NextPageLinkInRequest")
                    }
                },
                // TODO: Add appropriate exception and enable the test when this is bug is fixed.
                // new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, feed)
                // {
                //    ExpectedResultCallback = (tc) => new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                //    {
                //        ExpectedException2 = 
                //    }
                // },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfig) =>
                {
                    testConfig = testConfig.Clone();
                    testConfig.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    testDescriptor.RunTest(testConfig, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test homogeneity of feed payloads.")]
        public void FeedValidatorTests()
        {
            var model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            var testDescriptors = this.CreateFeedValidatorDescriptors(model);

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
               (testDescriptor, testConfiguration) =>
               {
                   testConfiguration = testConfiguration.Clone();
                   testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                   testDescriptor.RunTest(testConfiguration, this.Logger);
               });
        }

        private PayloadWriterTestDescriptor<ODataItem> CreateDefaultFeedWithAtomMetadataDescriptor(bool withModel)
        {
            EdmModel model = null;
            if (withModel)
            {
                model = new EdmModel();
            }

            ODataResourceSet resourceCollection = ObjectModelUtils.CreateDefaultFeed("CustomersSet", "CustomerType", model);

            EdmEntitySet customerSet = null;
            EdmEntityType customerType = null;
            if (model != null)
            {
                var container = model.FindEntityContainer("DefaultContainer");
                customerSet = container.FindEntitySet("CustomersSet") as EdmEntitySet;
                customerType = model.FindType("TestModel.CustomerType") as EdmEntityType;
            }

            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                (testConfiguration) =>
                {
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = string.Join("$(NL)",
                                    "{",
                                    "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#CustomersSet\",\"value\":[",
                                    string.Empty,
                                    "]",
                                    "}"),
                            FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                        };
                    }
                    else
                    {
                        string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                        throw new NotSupportedException("Invalid format detected: " + formatName);
                    }
                })
            {
                // JSON Light does not support writing without model
                SkipTestConfiguration = tc => model == null && tc.Format == ODataFormat.Json,
                Model = model,
                PayloadEdmElementContainer = customerSet,
                PayloadEdmElementType = customerType,
            };
        }

        private PayloadWriterTestDescriptor<ODataItem>[] CreateFeedQueryCountDescriptors()
        {
            Func<long?, ODataResourceSet> feedCreator = (c) =>
            {
                ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed();
                feed.Count = c;
                return feed;
            };

            long?[] counts = new long?[] { 0, 1, 2, 1000, -1 - 10, long.MaxValue, long.MinValue, null };

            EdmModel model = new EdmModel();
            ODataResource entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata("DefaultEntitySet", "DefaultEntityType", model);

            var container = model.FindEntityContainer("DefaultContainer");
            var entitySet = container.FindEntitySet("DefaultEntitySet") as EdmEntitySet;
            var entityType = model.FindType("TestModel.DefaultEntityType") as EdmEntityType;

            var descriptors = counts.Select(count => new PayloadWriterTestDescriptor<ODataItem>(this.Settings, feedCreator(count),
                (testConfiguration) =>
                {
                    if (testConfiguration.IsRequest && count.HasValue)
                    {
                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("ODataWriterCore_QueryCountInRequest")
                        };
                    }

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        if (count.HasValue)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = "$(Indent)$(Indent)\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName + "\":\"" + count + "\"",
                                FragmentExtractor = (result) => result.Object().Property(JsonLightConstants.ODataCountAnnotationName)
                            };
                        }
                        else
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = string.Join("$(NL)",
                                    "[",
                                    string.Empty,
                                    "]"),
                                FragmentExtractor = (result) =>
                                {
                                    return JsonLightWriterUtils.GetTopLevelFeedItemsArray(testConfiguration, result).RemoveAllAnnotations(true);
                                }
                            };
                        }
                    }
                    else
                    {
                        string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                        throw new NotSupportedException("Invalid format detected: " + formatName);
                    }
                })
            {
                Model = model,
                PayloadEdmElementContainer = entitySet,
                PayloadEdmElementType = entityType,
            });

            return descriptors.ToArray();
        }

        private PayloadWriterTestDescriptor<ODataItem>[] CreateFeedNextLinkDescriptors()
        {
            string[] nextLinkUris = new string[]
            {
                "http://my.customers.com/?skiptoken=1234",
                "http://my.customers.com/?",
                "http://my.customers.com/",
                "http://my.customers.com/?$filter=3.14E%2B%20ne%20null",
                "http://my.customers.com/?$filter='foo%20%26%20'%20ne%20null",
                "http://my.customers.com/?$filter=not%20endswith(Name,'%2B')",
                "http://my.customers.com/?$filter=geo.distance(Point,%20geometry'SRID=0;Point(6.28E%2B3%20-2.1e%2B4)')%20eq%20null",
            };

            Func<string, ODataResourceSet> feedCreator = (nextLink) =>
                {
                    ODataResourceSet resourceCollection = ObjectModelUtils.CreateDefaultFeed();
                    resourceCollection.NextPageLink = new Uri(nextLink);
                    return resourceCollection;
                };

            EdmModel model = new EdmModel();

            ODataResourceSet dummyFeed = ObjectModelUtils.CreateDefaultFeed("CustomersSet", "CustomerType", model);

            var container = model.FindEntityContainer("DefaultContainer");
            var customerSet = container.FindEntitySet("CustomersSet") as EdmEntitySet;
            var customerType = model.FindType("TestModel.CustomerType") as EdmEntityType;

            return nextLinkUris.Select(nextLink => new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                feedCreator(nextLink),
                (testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("WriterValidationUtils_NextPageLinkInRequest")
                        };
                    }

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = "\"" + JsonLightConstants.ODataNextLinkAnnotationName + "\":\"" + nextLink + "\"",
                            FragmentExtractor = (result) => result.Object().Property(JsonLightConstants.ODataNextLinkAnnotationName)
                        };
                    }
                    else
                    {
                        string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                        throw new NotSupportedException("Invalid format detected: " + formatName);
                    }
                })
            {
                Model = model,
                PayloadEdmElementContainer = customerSet,
                PayloadEdmElementType = customerType,
            }).ToArray();
        }

        private PayloadWriterTestDescriptor<ODataItem> CreateDefaultFeedMetadataDescriptor(bool withModel)
        {
            EdmModel model = null;
            if (withModel)
            {
                model = new EdmModel();
            }

            ODataResourceSet feed = ObjectModelUtils.CreateDefaultFeed("CustomersSet", "CustomerType", model);

            EdmEntitySet customerSet = null;
            EdmEntityType customerType = null;
            if (model != null)
            {
                var container = model.FindEntityContainer("DefaultContainer");
                customerSet = container.FindEntitySet("CustomersSet") as EdmEntitySet;
                customerType = model.FindType("TestModel.CustomerType") as EdmEntityType;
            }

            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                feed,
                (testConfiguration) =>
                {
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = string.Join("$(NL)",
                                    "{",
                                    "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#CustomersSet\",\"value\":[",
                                    string.Empty,
                                    "]",
                                    "}"),
                            FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                        };
                    }
                    else
                    {
                        string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                        throw new NotSupportedException("Invalid format detected: " + formatName);
                    }
                })
            {
                // JSON Light does not support writing without model
                SkipTestConfiguration = tc => model == null && tc.Format == ODataFormat.Json,
                Model = model,
                PayloadEdmElementContainer = customerSet,
                PayloadEdmElementType = customerType,
            };
        }

        private IEnumerable<PayloadWriterTestDescriptor<ODataPayloadElement>> CreateFeedValidatorDescriptors(IEdmModel model)
        {
            var cityType = model.EntityTypes().Single(type => type.FullName() == "TestModel.CityType");
            var personType = model.EntityTypes().Single(type => type.FullName() == "TestModel.Person");
            var employeeType = model.EntityTypes().Single(type => type.FullName() == "TestModel.Employee");

            var testCases = new[]
                {
                    new
                    {
                        Description = "Homogenous collection (no inheritance)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithDefaultAtomIDAnnotation().WithTypeAnnotation(personType)
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(personType))
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(personType)),
                        ExpectedException = (ExpectedException)null,
                        Model = model
                    },
                    new
                    {
                        Description = "Homogenous collection (inheritance, base type first)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithDefaultAtomIDAnnotation().WithTypeAnnotation(personType)
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(personType))
                            .Append(PayloadBuilder.Entity("TestModel.Employee")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(employeeType)),
                        ExpectedException = (ExpectedException)null,
                        Model = model
                    },
                    new
                    {
                        Description = "Homogenous collection (inheritance, derived type first)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithDefaultAtomIDAnnotation().WithTypeAnnotation(personType)
                            .Append(PayloadBuilder.Entity("TestModel.Employee")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(employeeType))
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(personType)),
                        ExpectedException = (ExpectedException)null,
                        Model = model
                    },
                    new
                    {
                        Description = "Heterogeneous collection",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithDefaultAtomIDAnnotation().WithTypeAnnotation(personType)
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(personType))
                            .Append(PayloadBuilder.Entity("TestModel.CityType")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(cityType)),
                        ExpectedException = ODataExpectedExceptions.ODataException("ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes", "TestModel.CityType", "TestModel.Person"),
                        Model = model
                    },
                    new
                    {
                        Description = "Heterogeneous collection (no model)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithDefaultAtomIDAnnotation()
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1))
                            .Append(PayloadBuilder.Entity("TestModel.CityType")
                                .PrimitiveProperty("Id", 2)),
                        ExpectedException = (ExpectedException)null,
                        Model = (IEdmModel)null
                    },
                };

            // Create the tests for a top-level feed
            return testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, testCase.Feed)
                {
                    DebugDescription = testCase.Description,
                    Model = testCase.Model,
                    PayloadDescriptor = new PayloadTestDescriptor()
                    {
                        DebugDescription = testCase.Description,
                        PayloadElement = testCase.Feed,
                        PayloadEdmModel = testCase.Model,
                        SkipTestConfiguration = tc => testCase.Model == null && tc.Format == ODataFormat.Json,
                    },
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = testCase.Feed,
                            ExpectedException2 = testCase.ExpectedException,
                        };
                    }
                });
        }
    }
}
