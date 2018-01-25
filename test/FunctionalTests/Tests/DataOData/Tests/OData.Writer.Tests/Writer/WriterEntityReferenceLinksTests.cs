//---------------------------------------------------------------------
// <copyright file="WriterEntityReferenceLinksTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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
    /// Tests for writing results of $ref queries with the OData writer.
    /// </summary>
    // [TestClass, TestCase]
    public class WriterEntityReferenceLinksTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test $ref payloads that return a single link.")]
        public void EntityReferenceLinkTest()
        {
            ODataEntityReferenceLink entityReferenceLink1 = new ODataEntityReferenceLink { Url = new Uri("http://odata.org/linkresult") };
            ODataEntityReferenceLink entityReferenceLink2 = new ODataEntityReferenceLink { Url = new Uri("relative", UriKind.Relative) };
            ODataEntityReferenceLink entityReferenceLink3 = new ODataEntityReferenceLink { Url = new Uri("http://odata.org/linkresult") };
            entityReferenceLink3.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestModel.unknown", new ODataPrimitiveValue(123)));
            entityReferenceLink3.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.name", new ODataPrimitiveValue(456)));
            ODataEntityReferenceLink entityReferenceLink4 = new ODataEntityReferenceLink { Url = new Uri("relative", UriKind.Relative) };
            entityReferenceLink4.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestModel.unknown", new ODataPrimitiveValue(123)));
            entityReferenceLink4.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.name", new ODataPrimitiveValue(456)));

            ODataEntityReferenceLink[] resultLinks = new ODataEntityReferenceLink[]
            {
                entityReferenceLink1, entityReferenceLink2, entityReferenceLink3, entityReferenceLink4
            };

            this.CombinatorialEngineProvider.RunCombinations(
                resultLinks,
                TestWriterUtils.ODataBehaviorKinds,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (resultLink, behaviorKind, testConfiguration) =>
                {
                    bool expectMetadataNamespace = false;
                    PayloadWriterTestDescriptor<ODataEntityReferenceLink> testDescriptor =
                        new PayloadWriterTestDescriptor<ODataEntityReferenceLink>(
                            this.Settings,
                            resultLink,
                            CreateExpectedCallback(resultLink, expectMetadataNamespace, "http://odata.org/test/$metadata#$ref"));

                    // When writing JSON lite, always provide a model and a non-null nav prop.
                    // The error cases when a model isn't provided or the nav prop is null are tested in JsonLightEntityReferenceLinkWriterTests
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testDescriptor.Model = CreateModelWithNavProps();
                        var model = testDescriptor.GetMetadataProvider();
                    }

                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (!testConfiguration.IsRequest)
                    {
                        testConfiguration.MessageWriterSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
                    }

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteEntityReferenceLink(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test error cases when writing a single link.")]
        public void EntityReferenceLinkErrorTest()
        {
            string resultUriString = "http://odata.org/linkresult";
            ODataEntityReferenceLink resultLink = new ODataEntityReferenceLink { Url = new Uri(resultUriString) };

            PayloadWriterTestDescriptor<ODataEntityReferenceLink>[] testCases = new PayloadWriterTestDescriptor<ODataEntityReferenceLink>[]
                {
                    new PayloadWriterTestDescriptor<ODataEntityReferenceLink>(this.Settings, resultLink, (string)null, (string)null),
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                TestWriterUtils.InvalidSettingSelectors,
                (testCase, testConfiguration, selector) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testCase.Model = CreateModelWithNavProps();
                        var edmModel = testCase.GetMetadataProvider();
                    }

                    TestWriterUtils.WriteWithStreamErrors(
                        testCase,
                        selector,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteEntityReferenceLink(testCase.PayloadItems.Single()),
                        this.Assert);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test $ref payloads that return multiple links.")]
        public void EntityReferenceLinksTest()
        {
            string resultUri1String = "http://odata.org/linkresult1";
            string resultUri2String = "linkresult2";
            string resultUri3String = "http://odata.org/linkresult3";
            ODataEntityReferenceLink entityReferenceLink1 = new ODataEntityReferenceLink { Url = new Uri(resultUri1String) };
            ODataEntityReferenceLink entityReferenceLink2 = new ODataEntityReferenceLink { Url = new Uri(resultUri2String, UriKind.Relative) };
            ODataEntityReferenceLink entityReferenceLink3 = new ODataEntityReferenceLink { Url = new Uri(resultUri3String) };
            entityReferenceLink3.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestModel.unknown", new ODataPrimitiveValue(123)));
            entityReferenceLink3.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.name", new ODataPrimitiveValue(456)));

            Uri nextPageLink = new Uri("http://odata.org/nextpage");
            Uri relativeNextPageLink = new Uri("relative-nextpage", UriKind.Relative);

            long?[] inputCounts = new long?[] { null, 1, 3, -1, -3, 0, long.MaxValue, long.MinValue };
            Uri[] inputNextLinks = new Uri[] { nextPageLink, relativeNextPageLink, null };
            ODataInstanceAnnotation[][] inputAnnotations = new ODataInstanceAnnotation[][]
            {
                new ODataInstanceAnnotation[0],
                new ODataInstanceAnnotation[]
                {
                    new ODataInstanceAnnotation("TestModel.annotation", new ODataPrimitiveValue(321)),
                    new ODataInstanceAnnotation("custom.annotation", new ODataPrimitiveValue(654))
                }
            };
            ODataEntityReferenceLink[][] inputReferenceLinks = new ODataEntityReferenceLink[][]
            {
                new ODataEntityReferenceLink[] { entityReferenceLink1, entityReferenceLink2, entityReferenceLink3 },
                new ODataEntityReferenceLink[] { entityReferenceLink1, entityReferenceLink3},
                new ODataEntityReferenceLink[] { entityReferenceLink1 },
                new ODataEntityReferenceLink[0],
                null
            };
            var testCases = inputCounts.SelectMany(
                inputCount => inputNextLinks.SelectMany(
                    inputNextLink => inputReferenceLinks.Select(
                        (inputReferenceLink, index) => new ODataEntityReferenceLinks { Count = inputCount, Links = inputReferenceLink, NextPageLink = inputNextLink, InstanceAnnotations = inputAnnotations[index == 1 ? 1 : 0] })));

            var testDescriptors = testCases.Select(
                testCase =>
                    new PayloadWriterTestDescriptor<ODataEntityReferenceLinks>(this.Settings, testCase, this.CreateExpectedCallback(testCase, /*forceNextLinkAndCountAtEnd*/ false)));

            // TODO: also cover the cases in protocol v1 (errors for inline count and next page link; different format for JSON)
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    ODataEntityReferenceLinks entityReferenceLinks = testDescriptor.PayloadItems.Single();

                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    if (!testConfiguration.IsRequest)
                    {
                        testConfiguration.MessageWriterSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
                    }

                    // When writing JSON lite, always provide a model and a non-null nav prop.
                    // The error cases when a model isn't provided or the nav prop is null are tested in JsonLightEntityReferenceLinkWriterTests
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testDescriptor.Model = CreateModelWithNavProps();
                        var edmModel = testDescriptor.GetMetadataProvider();
                    }

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteEntityReferenceLinks(entityReferenceLinks),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test error cases when writing multiple links.")]
        public void EntityReferenceLinksErrorTest()
        {
            string resultUri1String = "http://odata.org/linkresult1";
            string resultUri2String = "http://odata.org/linkresult2";
            string resultUri3String = "http://odata.org/linkresult3";
            ODataEntityReferenceLink resultUri1 = new ODataEntityReferenceLink { Url = new Uri(resultUri1String) };
            ODataEntityReferenceLink resultUri2 = new ODataEntityReferenceLink { Url = new Uri(resultUri2String) };
            ODataEntityReferenceLink resultUri3 = new ODataEntityReferenceLink { Url = new Uri(resultUri3String) };

            var testCase = new ODataEntityReferenceLinks
            {
                Links = new ODataEntityReferenceLink[] { resultUri1, resultUri2, resultUri3 },
            };

            PayloadWriterTestDescriptor<ODataEntityReferenceLinks>[] testCases = new PayloadWriterTestDescriptor<ODataEntityReferenceLinks>[]
                {
                    new PayloadWriterTestDescriptor<ODataEntityReferenceLinks>(
                        this.Settings,
                        testCase,
                        (testConfiguration) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) {
                            // Top-level EntityReferenceLinks payload write requests are not allowed.
                            ExpectedException2 = testConfiguration.IsRequest ? ODataExpectedExceptions.ODataException("ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed") : null
                        })
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                TestWriterUtils.InvalidSettingSelectors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (descriptor, selector, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        descriptor.Model = CreateModelWithNavProps();
                        var edmModel = descriptor.GetMetadataProvider();
                    }

                    TestWriterUtils.WriteWithStreamErrors(
                        descriptor,
                        selector,
                        testConfiguration,
                        (messageWriter) =>
                        {
                            messageWriter.WriteEntityReferenceLinks(testCase);
                        },
                        this.Assert);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test that we write the inline count first, then all the links and only then the next link.")]
        public void EntityReferenceLinksPropertyAccessOrderTest()
        {
            //// NOTE: this tests is important as Astoria relies on this behavior. Astoria only provides a next link after all the entity reference
            ////       links have been written so we must not access the next link before that point.

            ODataEntityReferenceLink entityReferenceLink1 = new ODataEntityReferenceLink { Url = new Uri("http://odata.org/linkresult1") };
            ODataEntityReferenceLink entityReferenceLink2 = new ODataEntityReferenceLink { Url = new Uri("http://odata.org/linkresult2") };
            ODataEntityReferenceLink entityReferenceLink3 = new ODataEntityReferenceLink { Url = new Uri("http://odata.org/linkresult3") };

            Uri nextPageLink = new Uri("http://odata.org/nextpage");
            Uri incorrectNextPageLink = new Uri("http://odata.org/incorrectnextpage");
            long correctCountValue = 3;

            // the expected result instance
            ODataEntityReferenceLinks expectedResult = new ODataEntityReferenceLinks
            {
                Count = 3,
                Links = new ODataEntityReferenceLink[] { entityReferenceLink1, entityReferenceLink2, entityReferenceLink3 },
                NextPageLink = nextPageLink
            };

            PayloadWriterTestDescriptor.WriterTestExpectedResultCallback expectedResultCallback = this.CreateExpectedCallback(expectedResult, /*forceNextLinkAndCountAtEnd*/ true);

            this.CombinatorialEngineProvider.RunCombinations(
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                (testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    // The instance with the proper inline count set but an incorrect next link; as we enumerate the links themselves
                    // we will invalidate the inline count and set the correct next link to guarantee the correct order of property accesses
                    // NOTE: we need to create this new for each iteration since the checking enumerable can (intentionally) only be enumerated once.
                    ODataEntityReferenceLinks testReferenceLink = new ODataEntityReferenceLinks
                    {
                        Count = correctCountValue,
                        // In JSON lite, we will write the next link first if one is available.  Otherwise, we'll write it at the end.
                        NextPageLink = testConfiguration.Format == ODataFormat.Json ? null : incorrectNextPageLink
                    };
                    testReferenceLink.Links = new CheckingEntityReferenceLinkEnumerable(
                        testReferenceLink,
                        correctCountValue,
                        nextPageLink /* correct next link */,
                        entityReferenceLink1,
                        entityReferenceLink2,
                        entityReferenceLink3);

                    PayloadWriterTestDescriptor<ODataEntityReferenceLinks> testDescriptor =
                        new PayloadWriterTestDescriptor<ODataEntityReferenceLinks>(this.Settings, testReferenceLink, expectedResultCallback);

                    IEdmNavigationProperty navProp = null;
                    IEdmEntitySet entitySet = null;
                    // When writing JSON lite, always provide a model and a non-null nav prop.
                    // The error cases when a model isn't provided or the nav prop is null are tested in JsonLightEntityReferenceLinkWriterTests
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testDescriptor.Model = CreateModelWithNavProps();
                        var edmModel = testDescriptor.GetMetadataProvider();
                        navProp = GetCollectionNavProp(edmModel);
                        entitySet = GetCollectionEntitySet(edmModel);
                    }

                    ODataEntityReferenceLinks entityReferenceLinks = testDescriptor.PayloadItems.Single();

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteEntityReferenceLinks(entityReferenceLinks),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        /// <summary>
        /// Creates a test model with one singleton and one collection navigation property.
        /// </summary>
        /// <returns>The new model.</returns>
        private static EdmModel CreateModelWithNavProps()
        {
            var model = new EdmModel();

            var Order = new EdmEntityType("TestModel", "Order");
            Order.AddKeys(Order.AddStructuralProperty("OrderId", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(Order);

            var CustomerInfo = new EdmEntityType("TestModel", "CustomerInfo");
            CustomerInfo.AddKeys(CustomerInfo.AddStructuralProperty("CustomerInfoId", EdmCoreModel.Instance.GetInt32(false)));
            CustomerInfo.AddStructuralProperty("Information", EdmCoreModel.Instance.GetString(true));
            model.AddElement(CustomerInfo);

            var Customer = new EdmEntityType("TestModel", "Customer");
            Customer.AddKeys(Customer.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(false)));
            Customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Orders",
                Target = Order,
                TargetMultiplicity = EdmMultiplicity.Many,
            });
            Customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Info",
                Target = CustomerInfo,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
            });
            model.AddElement(Customer);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("Customer", Customer);
            container.AddEntitySet("CustomerInfo", CustomerInfo);
            container.AddEntitySet("Order", Order);
            model.AddElement(container);
            return model;
        }

        /// <summary>
        /// Gets the collection navigation property of a model which was created via CreateModelWithNavProps(). 
        /// </summary>
        /// <param name="edmModel">The model to search.</param>
        /// <returns>The collection navigation property in the model.</returns>
        private static IEdmNavigationProperty GetCollectionNavProp(IEdmModel edmModel)
        {
            return edmModel
                .EntityTypes()
                .Single(et => et.Name == "Customer")
                .DeclaredProperties
                .OfType<IEdmNavigationProperty>()
                .Single(p => p.Name == "Orders");
        }

        /// <summary>
        /// Gets the entitySet cooresponding to the Collection navigation property.
        /// </summary>
        /// <param name="edmModel">The model to search.</param>
        /// <returns>The collection navigation property in the model.</returns>
        private static IEdmEntitySet GetCollectionEntitySet(IEdmModel edmModel)
        {
            return edmModel.EntityContainer.FindEntitySet("Customer");
        }

        /// <summary>
        /// Creates the expected result callback for the provided <paramref name="entityReferenceLinks"/>.
        /// </summary>
        /// <param name="entityReferenceLinks">The entity reference links to create the expected result for.</param>
        /// <param name="forceNextLinkAndCountAtEnd">Whether the next link and count should be expected at the end of the payload for JSON lite.</param>
        /// <returns>The expected result callback for the <paramref name="entityReferenceLinks"/>.</returns>
        private WriterTestDescriptor.WriterTestExpectedResultCallback CreateExpectedCallback(ODataEntityReferenceLinks entityReferenceLinks, bool forceNextLinkAndCountAtEnd = false)
        {
            long? count = entityReferenceLinks.Count;
            string nextPageLinkString = entityReferenceLinks.NextPageLink == null ? null : entityReferenceLinks.NextPageLink.OriginalString;

            return (testConfiguration) =>
            {
                ExpectedException expectedException = null;
                if (testConfiguration.IsRequest)
                {
                    // Top-level EntityReferenceLinks payload write requests are not allowed.
                    expectedException = ODataExpectedExceptions.ODataException("ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed");
                }

                if (expectedException == null)
                {
                    ODataEntityReferenceLink relativeLink = entityReferenceLinks.Links == null ? null : entityReferenceLinks.Links.FirstOrDefault(l => !l.Url.IsAbsoluteUri);
                    bool nextPageLinkRelative = entityReferenceLinks.NextPageLink == null ? false : !entityReferenceLinks.NextPageLink.IsAbsoluteUri;
                    if ((relativeLink != null || nextPageLinkRelative) && testConfiguration.MessageWriterSettings.BaseUri == null)
                    {
                        // We allow relative Uri strings in JSON Light.
                        if (testConfiguration.Format != ODataFormat.Json)
                        {
                            string relativeUriString = relativeLink == null ? entityReferenceLinks.NextPageLink.OriginalString : relativeLink.Url.OriginalString;
                            expectedException = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", relativeUriString);
                        }
                    }
                }

                if (expectedException != null)
                {
                    return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException2 = expectedException
                    };
                }

                string[] resultUriStrings = entityReferenceLinks.Links == null ? null : entityReferenceLinks.Links.Select(l => GetResultUri(l.Url, testConfiguration)).ToArray();

                List<string> atomStrings = new List<string>();
                List<string> jsonLightStrings = new List<string>();
                string nextLinkString = null;

                jsonLightStrings.Add("{");
                var jsonLightFirstLine = "$(Indent)\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection($ref)\",";

                if ((resultUriStrings == null || resultUriStrings.Length == 0) && !count.HasValue && nextPageLinkString == null && testConfiguration.Format != ODataFormat.Json)
                {
                    atomStrings.Add(@"<links xmlns=""http://docs.oasis-open.org/odata/ns/data"" />");
                }
                else
                {
                    atomStrings.Add(@"<links xmlns=""http://docs.oasis-open.org/odata/ns/data"">");
                    if (count.HasValue)
                    {
                        atomStrings.Add(@"<m:count xmlns:m=""" + TestAtomConstants.ODataMetadataNamespace + @""">" + count.Value + @"</m:count>");

                        if (!forceNextLinkAndCountAtEnd)
                        {
                            jsonLightFirstLine += "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName + "\":\"" + count.Value + "\",";
                        }
                    }

                    if (nextPageLinkString != null && !forceNextLinkAndCountAtEnd)
                    {
                        // In JSON lite, the next link comes at the beginning of the payload if available, but in JSON verbose and ATOM it comes at the end.
                        jsonLightFirstLine += "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName + "\":\"" + nextPageLinkString + "\",";
                    }

                    jsonLightFirstLine += "\"value\":[";

                    jsonLightStrings.Add(jsonLightFirstLine);

                    int length = resultUriStrings == null ? 0 : resultUriStrings.Length;
                    if (length == 0)
                    {
                        jsonLightStrings.Add("$(Indent)$(Indent)");
                    }
                    else
                    {
                        for (int i = 0; i < length; ++i)
                        {
                            atomStrings.Add(@"<uri>" + resultUriStrings[i] + @"</uri>");
                            if (i == 0)
                            {
                                jsonLightStrings.Add("$(Indent)$(Indent){");
                            }
                            else
                            {
                                jsonLightStrings.Add("$(Indent)$(Indent)},{");
                            }

                            jsonLightStrings.Add("$(Indent)$(Indent)$(Indent)" + "\"url\":\"" + resultUriStrings[i] + "\"");

                            if (i == resultUriStrings.Length - 1)
                            {
                                jsonLightStrings.Add("$(Indent)$(Indent)}");
                            }
                        }
                    }

                    var jsonLightLastLine = "$(Indent)]";

                    if (count.HasValue && forceNextLinkAndCountAtEnd)
                    {
                        jsonLightLastLine += ",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName + "\":" + count.Value;
                    }

                    if (nextPageLinkString != null)
                    {
                        atomStrings.Add(@"<next>" + nextPageLinkString + @"</next>");
                        nextLinkString = "\"__next\":\"" + nextPageLinkString + "\"";

                        if (forceNextLinkAndCountAtEnd)
                        {
                            jsonLightLastLine += ",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName + "\":\"" + nextPageLinkString + "\"";
                        }
                    }


                    jsonLightStrings.Add(jsonLightLastLine);
                    jsonLightStrings.Add("}");

                    atomStrings.Add(@"</links>");
                }

                if (testConfiguration.Format == ODataFormat.Json)
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Join("$(NL)", jsonLightStrings),
                        FragmentExtractor = (result) => result
                    };
                }
                else
                {
                    throw new TestInfrastructureException("Expected ATOM or JSON Lite.");
                }
            };
        }

        /// <summary>
        /// Compute the resulting string representation of a URI in a given test configuration.
        /// </summary>
        /// <param name="uri">The URI to convert to string.</param>
        /// <param name="testConfig">The test configuration to use.</param>
        /// <returns>The string representation of the <paramref name="uri"/> for the given test configuration.</returns>
        private string GetResultUri(Uri uri, WriterTestConfiguration testConfig)
        {
            Uri baseUri = testConfig.MessageWriterSettings.BaseUri;

            Debug.Assert(testConfig.Format == ODataFormat.Json, "Only ATOM and JSON lite are supported.");
            if (uri.IsAbsoluteUri)
            {
                return uri.AbsoluteUri;
            }
            else if (baseUri != null)
            {
                // In JSON we expect the absolute URI if a base URI is present
                return new Uri(baseUri, uri).AbsoluteUri;
            }
            else
            {
                // This will fail; return the original relative URI.
                return uri.OriginalString;
            }
        }

        /// <summary>
        /// Creates the expected result callback for the provided <paramref name="entityReferenceLink"/>.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to create the expected result for.</param>
        /// <param name="expectMetadataNamespace">true if the XML representation is expected to use the OData metadata namespace,
        /// false if it's expected to use the default OData namespace.</param>
        /// <param name="expectedContextUrl">The expected value of the odata.context url when writing JSON lite responses.</param>
        /// <returns>The expected result callback for the <paramref name="entityReferenceLink"/>.</returns>
        private PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateExpectedCallback(ODataEntityReferenceLink entityReferenceLink, bool expectMetadataNamespace, string expectedContextUrl)
        {
            Uri uri = entityReferenceLink.Url;
            return (testConfiguration) =>
            {
                string uriResultString = GetResultUri(uri, testConfiguration);

                if (!uri.IsAbsoluteUri && testConfiguration.MessageWriterSettings.BaseUri == null && testConfiguration.Format != ODataFormat.Json)
                {
                    // for relative URIs without base URI we expect to fail
                    return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", uri.OriginalString)
                    };
                }

                if (testConfiguration.Format == ODataFormat.Json)
                {
                    string jsonLightResult =
                        "{" +
                        "$(NL)" +
                        "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + expectedContextUrl + "\"," +
                        "\"url\":\"" + uriResultString + "\"" +
                        "$(NL)" +
                        "}";

                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Join("$(NL)", jsonLightResult),
                        FragmentExtractor = (result) => result
                    };
                }
                else
                {
                    throw new TestInfrastructureException("Expected ATOM or JSON Lite.");
                }
            };
        }

        /// <summary>
        /// Class representing a checking enumerable for the entity reference links in an <see cref="ODataEntityReferenceLinks"/> instance.
        /// The class will invalidate the inline count of the instance when the enumerator for the links is retrieved and only set the correct
        /// next link after the last link has been enumerated. The enumerable can only be enumerated once to ensure we only go over the links once during writing.
        /// </summary>
        private sealed class CheckingEntityReferenceLinkEnumerable : IEnumerable<ODataEntityReferenceLink>
        {
            /// <summary>The owning <see cref="ODataEntityReferenceLinks"/> instance.</summary>
            private readonly ODataEntityReferenceLinks owner;

            /// <summary>The value for the incorrect inline count; this is set when the enumerator for the links is retrieved.</summary>
            private readonly long? incorrectInlineCount;

            /// <summary>The correct next link; this is set after all the links have been enumerated.</summary>
            private readonly Uri correctNextPageLink;

            /// <summary>The array of links to enumerate.</summary>
            private readonly ODataEntityReferenceLink[] entityReferenceLinks;

            /// <summary>true if an enumerator has been retrieved; otherwise false.</summary>
            private bool enumerationStarted;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="owner">The owning <see cref="ODataEntityReferenceLinks"/> instance.</param>
            /// <param name="incorrectInlineCount">The value for the incorrect count; this is set when the enumerator for the links is retrieved.</param>
            /// <param name="correctNextPageLink">The correct next link; this is set after all the links have been enumerated.</param>
            /// <param name="links">The array of links to enumerate.</param>
            public CheckingEntityReferenceLinkEnumerable(
                ODataEntityReferenceLinks owner,
                long? incorrectInlineCount,
                Uri correctNextPageLink,
                params ODataEntityReferenceLink[] links)
            {
                this.owner = owner;
                this.incorrectInlineCount = incorrectInlineCount;
                this.entityReferenceLinks = links;
                this.correctNextPageLink = correctNextPageLink;
            }

            /// <summary>
            /// Gets an enumerator for the entity reference links.
            /// </summary>
            /// <returns>An enumerator for the entity reference links.</returns>
            /// <remarks>This method can only be called once.</remarks>
            public IEnumerator<ODataEntityReferenceLink> GetEnumerator()
            {
                return this.GetEnumeratorImplementation();
            }

            /// <summary>
            /// Gets an enumerator for the entity reference links.
            /// </summary>
            /// <returns>An enumerator for the entity reference links.</returns>
            /// <remarks>This method can only be called once.</remarks>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumeratorImplementation();
            }

            /// <summary>
            /// Gets an enumerator for the entity reference links.
            /// </summary>
            /// <returns>An enumerator for the entity reference links.</returns>
            /// <remarks>This method can only be called once.</remarks>
            private IEnumerator<ODataEntityReferenceLink> GetEnumeratorImplementation()
            {
                if (this.enumerationStarted)
                {
                    throw new InvalidOperationException("Entity reference links have already been enumerated.");
                }

                this.enumerationStarted = true;

                // set the inline count to the invalid value; it should have been read at this point.
                owner.Count = this.incorrectInlineCount;

                return new CheckingEntityReferenceLinkEnumerator(this);
            }

            /// <summary>
            /// Implementation of the checkin enumerator that will set the correct next link on the owning <see cref="ODataEntityRefereceLinks"/> instance
            /// only after the last item has been enumerated.
            /// </summary>
            private sealed class CheckingEntityReferenceLinkEnumerator : IEnumerator<ODataEntityReferenceLink>
            {
                /// <summary>The enumerable the enumerator was retrieved from; used to access the owning <see cref="ODataEntityRefereceLinks"/> instance.</summary>
                private readonly CheckingEntityReferenceLinkEnumerable enumerable;

                /// <summary>The current index into the array of links.</summary>
                /// <remarks>
                /// This has the value -2 if the object has been disposed, -1 if the enumeration has not been started, 'length-of-array' if the enumeration completed
                /// and otherwise the index into the array.
                /// </remarks>
                private int currentIx = -1;

                /// <summary>
                /// Constructor.
                /// </summary>
                /// <param name="enumerable">The enumerable the enumerator was retrieved from; used to access the owning <see cref="ODataEntityRefereceLinks"/> instance.</param>
                public CheckingEntityReferenceLinkEnumerator(CheckingEntityReferenceLinkEnumerable enumerable)
                {
                    this.enumerable = enumerable;
                }

                /// <summary>
                /// The current item.
                /// </summary>
                public ODataEntityReferenceLink Current
                {
                    get
                    {
                        Debug.Assert(this.currentIx >= -1, "Object disposed.");
                        return this.GetCurrent();
                    }
                }

                /// <summary>
                /// Disposes the enumerator.
                /// </summary>
                public void Dispose()
                {
                    Debug.Assert(this.currentIx >= -1, "Object disposed.");
                    this.currentIx = -2;
                }

                /// <summary>
                /// The current item.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        Debug.Assert(this.currentIx >= -1, "Object disposed.");
                        return this.GetCurrent();
                    }
                }

                /// <summary>
                /// Moves the enumerator to the next item in the array.
                /// </summary>
                /// <returns>true if more items are available; otherwise false.</returns>
                public bool MoveNext()
                {
                    Debug.Assert(this.currentIx >= -1, "Object disposed.");

                    if (this.currentIx == this.enumerable.entityReferenceLinks.Length)
                    {
                        // already beyond the end
                        return false;
                    }

                    this.currentIx++;

                    if (this.currentIx == this.enumerable.entityReferenceLinks.Length)
                    {
                        // we just moved beyond the end of the enumerable; set the next link to the valid value now
                        this.enumerable.owner.NextPageLink = this.enumerable.correctNextPageLink;
                        return false;
                    }

                    return true;
                }

                /// <summary>
                /// Resets the enumerator; not supported.
                /// </summary>
                public void Reset()
                {
                    Debug.Assert(this.currentIx >= -1, "Object disposed.");
                    throw new NotSupportedException("Resetting the checking enumerator is not supported.");
                }

                /// <summary>
                /// Gets the current item.
                /// </summary>
                /// <returns>The current item of the enumerator.</returns>
                private ODataEntityReferenceLink GetCurrent()
                {
                    if (this.currentIx < 0)
                    {
                        throw new InvalidOperationException("MoveNext has not been called on the enumerator.");
                    }
                    else if (this.currentIx == this.enumerable.entityReferenceLinks.Length)
                    {
                        return this.enumerable.entityReferenceLinks[this.enumerable.entityReferenceLinks.Length - 1];
                    }
                    else
                    {
                        return this.enumerable.entityReferenceLinks[this.currentIx];
                    }
                }
            }
        }
    }
}
