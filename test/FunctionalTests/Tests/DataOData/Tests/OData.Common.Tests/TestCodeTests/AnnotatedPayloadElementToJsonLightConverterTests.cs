//---------------------------------------------------------------------
// <copyright file="AnnotatedPayloadElementToJsonLightConverterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.TestCodeTests
{
    #region Namespaces
    using System.IO;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.JsonLight;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for using the AnnotatedPayloadElementToJsonLightConverter to serialize JSON Light payloads.
    /// </summary>
    [TestClass, TestCase]
    public class AnnotatedPayloadElementToJsonLightConverterTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency]
        public AnnotatedPayloadElementToJsonLightConverter JsonLightSerializer { get; set; }

        [InjectDependency]
        public JsonValueComparer JsonValueComparer { get; set; }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize feeds.")]
        public void JsonLightTaupoSerializerFeedTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Empty feed
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.EntitySet().WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[]
                        }"
                },
                // Empty feed with count and next link
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .InlineCount(42)
                        .NextLink("http://odata.org/next")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName + @""":42,
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[],
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName + @""":""http://odata.org/next""
                        }"
                },
                // Feed with single entry
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .Append(PayloadBuilder.Entity().PrimitiveProperty("ID", (long)42))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[
                            { ""ID"":42 }
                          ]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize entries.")]
        public void JsonLightTaupoSerializerEntryTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Entry with only ID
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .PrimitiveProperty("ID", (long)42)
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          ""ID"":42
                        }"
                },
                // Entry with ID and typename
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42
                        }"
                },
                // Entry with all metadata expanded
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CustomerWithImage")
                        .Id("CustomerId")
                        .ETag("etag")
                        .WithEditLink("http://odata.org/editlink")
                        .WithSelfLink("http://odata.org/readlink")
                        .WithStreamContentType("image/jpg")
                        .WithStreamEditLink("http://odata.org/streameditlink")
                        .WithStreamSourceLink("http://odata.org/streamreadlink")
                        .WithStreamETag("stream-etag")
                        .PrimitiveProperty("ID", (long)42)
                        .Operation(new ServiceOperationDescriptor { IsAction = true, Metadata = "./metadata", Target = "http://odata.org/target", Title = "ActionTitle" })
                        .Operation(new ServiceOperationDescriptor { IsAction = true, Metadata = "./metadata2", Target = "http://odata.org/target2", Title = "ActionTitle2" })
                        .Operation(new ServiceOperationDescriptor { IsAction = false, Metadata = "./metadata", Target = "http://odata.org/target", Title = "ActionTitle" })
                        .Operation(new ServiceOperationDescriptor { IsAction = false, Metadata = "./metadata", Target = "http://odata.org/target2", Title = "ActionTitle2" })
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.CustomerWithImage"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + @""":""CustomerId"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName + @""":""etag"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataEditLinkAnnotationName + @""":""http://odata.org/editlink"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + @""":""http://odata.org/readlink"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaEditLinkAnnotationName + @""":""http://odata.org/streameditlink"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaReadLinkAnnotationName + @""":""http://odata.org/streamreadlink"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaContentTypeAnnotationName + @""":""image/jpg"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaETagAnnotationName + @""":""stream-etag"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataActionsAnnotationName + @""":{ ""./metadata"":[{ ""title"":""ActionTitle"", ""target"":""http://odata.org/target""}], ""./metadata2"":[{ ""title"":""ActionTitle2"", ""target"":""http://odata.org/target2""}]},
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataFunctionsAnnotationName + @""":{ ""./metadata"":[{ ""title"":""ActionTitle"", ""target"":""http://odata.org/target""}, { ""title"":""ActionTitle2"", ""target"":""http://odata.org/target2""}]},
                          ""ID"":42
                        }"
                },
                // Entry with deferred navigation and association properties
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .NavigationProperty("NavProp1", "http://odata.org/NavProp1", "http://odata.org/AssocProp1")
                        .NavigationProperty("NavProp2", "http://odata.org/NavProp2")
                        .NavigationProperty("NavProp3", null, "http://odata.org/AssocProp3")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42,
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp1", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp1"",
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp1", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp1"",
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp2", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp2"",
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp3", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp3""
                        }"
                },
                // Entry with navigation link with expanded entry
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.Order").PrimitiveProperty("ID", ((long)43)))
                        .ExpandedNavigationProperty("NavProp2", PayloadBuilder.Entity("TestModel.Order").PrimitiveProperty("ID", ((long)43)), "http://odata.org/NavProp2")
                        .ExpandedNavigationProperty("NavProp3", PayloadBuilder.Entity("TestModel.Order").PrimitiveProperty("ID", ((long)43)), new DeferredLink { UriString = "http://odata.org/AssocProp3" })
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42,
                          ""NavProp1"":{ """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.Order"",""ID"":43 },
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp2", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp2"",
                          ""NavProp2"":{ """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.Order"",""ID"":43 },
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp3", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp3"",
                          ""NavProp3"":{ """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.Order"",""ID"":43 }
                        }"
                },
                // Entry with navigation link with expanded feed
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .ExpandedNavigationProperty("NavProp1", PayloadBuilder.EntitySet())
                        .ExpandedNavigationProperty("NavProp2", PayloadBuilder.EntitySet(), "http://odata.org/NavProp2")
                        .ExpandedNavigationProperty("NavProp3", PayloadBuilder.EntitySet(), new DeferredLink { UriString = "http://odata.org/AssocProp3" })
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42,
                          ""NavProp1"":[],
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp2", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp2"",
                          ""NavProp2"":[],
                          """ + JsonLightUtils.GetPropertyAnnotationName("NavProp3", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp3"",
                          ""NavProp3"":[]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    PayloadFormatVersionAnnotatingVisitor.AnnotateJsonLight(testCase.PayloadElement, DataServiceProtocolVersion.Unspecified, false);
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level properties.")]
        public void JsonLightTaupoSerializerPropertyTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Null property
                // TODO: Change the payload of null top-level properties #645
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty("Prop", null).WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          ""value"":null
                        }"
                },
                // Primitive property
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty("Prop", "" + JsonLightConstants.ODataValuePropertyName + @"").WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":""" + JsonLightConstants.ODataValuePropertyName + @"""
                        }"
                },
                // Complex property
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Property("Prop", 
                        PayloadBuilder.ComplexValue("TestModel.City")
                            .PrimitiveProperty("City", "Vienna"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.City"", ""City"":""Vienna""
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level collection properties.")]
        public void JsonLightTaupoSerializerCollectionPropertyTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Primitive collection property
                new JsonLightSerializerTestCase
                {
                    PayloadElement = new PrimitiveMultiValueProperty("Prop", PayloadBuilder.PrimitiveMultiValue().Item((long)1).Item((long)2)).WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[1,2]
                        }"
                },
                // Complex collection property
                new JsonLightSerializerTestCase
                {
                    PayloadElement = new ComplexMultiValueProperty("Prop", 
                        PayloadBuilder.ComplexMultiValue()
                            .Item(PayloadBuilder.ComplexValue("TestModel.City")
                                .PrimitiveProperty("City", "Vienna")))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[{ """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.City"", ""City"":""Vienna"" }]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level (streamable) collections.")]
        public void JsonLightTaupoSerializerCollectionTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Primitive collection
                new JsonLightSerializerTestCase
                {
                    PayloadElement = new PrimitiveCollection(PayloadBuilder.PrimitiveValue((long)1), PayloadBuilder.PrimitiveValue((long)2)).WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[1,2]
                        }"
                },
                // Complex collection
                new JsonLightSerializerTestCase
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.City")
                                .PrimitiveProperty("City", "Vienna"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[{ """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.City"", ""City"":""Vienna"" }]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level errors.")]
        public void JsonLightTaupoSerializerErrorTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Top-level error
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Error("error-code").Message("error-message"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataErrorPropertyName + @""":{""code"":""error-code"",""message"": ""error-message""}}
                        }"
                },
                // Top-level error with inner error
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Error("error-code").Message("error-message").InnerError(
                        new ODataInternalExceptionPayload { 
                            Message = "inner-message", 
                            TypeName = "inner-typename", 
                            StackTrace = "inner-stack",
                            InternalException = new ODataInternalExceptionPayload {
                                Message = "inner-inner-message", 
                                TypeName = "inner-inner-typename", 
                                StackTrace = "inner-inner-stack",
                            }}),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataErrorPropertyName + @""":{
                            ""code"":""error-code"",
                            ""message"":""error-message"", 
                            ""innererror"":{
                              ""message"":""inner-message"",
                              ""type"":""inner-typename"",
                              ""stacktrace"":""inner-stack"",
                              ""internalexception"":{
                                ""message"":""inner-inner-message"",
                                ""type"":""inner-inner-typename"",
                                ""stacktrace"":""inner-inner-stack""
                              }
                            }
                          }
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize service documents.")]
        public void JsonLightTaupoSerializerServiceDocumentTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Service document
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection("Coll1Title", "Coll1Href")
                            .ResourceCollection("Coll2Title", "Coll2Href")
                            .WithTitle("WorkspaceTitle"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[
                            { ""name"":""Coll1Title"", ""url"":""Coll1Href"" },
                            { ""name"":""Coll2Title"", ""url"":""Coll2Href"" }
                          ]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize entity reference links.")]
        public void JsonLightTaupoSerializerEntityReferenceLinkTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Single entity reference link
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/erl")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + @""":""http://odata.org/erl""
                        }"
                },
                // Collection of entity reference links 
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.LinkCollection()
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl1"))
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl2"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[
                            { """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + @""":""http://odata.org/erl1"" },
                            { """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + @""":""http://odata.org/erl2"" }
                          ]
                        }"
                },
                // Collection of entity reference links with inline count and next link
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.LinkCollection()
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl1"))
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl2"))
                        .InlineCount(42)
                        .NextLink("http://odata.org/nextlink")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName + @""": 42,
                          """ + JsonLightConstants.ODataValuePropertyName + @""":[
                            { """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + @""":""http://odata.org/erl1"" },
                            { """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + @""":""http://odata.org/erl2"" }
                          ],
                          """ + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName + @""":""http://odata.org/nextlink""
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    PayloadFormatVersionAnnotatingVisitor.AnnotateJsonLight(testCase.PayloadElement, DataServiceProtocolVersion.Unspecified, false);
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize parameters.")]
        public void JsonLightTaupoSerializerParameterTest()
        {
            var testCases = new JsonLightSerializerTestCase[]
            {
                // Parameter payload with null, primitive, complex and collection paramters
                new JsonLightSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.ComplexValue()
                        .PrimitiveProperty("NullProp", null)
                        .PrimitiveProperty("LongProp", (long)42)
                        .Property("ComplexProp", PayloadBuilder.ComplexValue("TestModel.CityType").PrimitiveProperty("Name", "Vienna"))
                        .Property("PrimitiveColl", PayloadBuilder.PrimitiveMultiValue().Item((long)1).Item((long)2))
                        .Property("ComplexColl", PayloadBuilder.ComplexMultiValue()
                            .Item(PayloadBuilder.ComplexValue().PrimitiveProperty("Name", "Vienna"))
                            .Item(PayloadBuilder.ComplexValue().PrimitiveProperty("Name", "Prague"))),
                    ExpectedJson = @"
                        {
                          ""NullProp"":null,
                          ""LongProp"":42,
                          ""ComplexProp"":{""" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + @""":""TestModel.CityType"", ""Name"":""Vienna""},
                          ""PrimitiveColl"":[1,2],
                          ""ComplexColl"":[{""Name"":""Vienna""}, {""Name"":""Prague""}]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonLightSerializer.ConvertToJsonLightValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        private sealed class JsonLightSerializerTestCase
        {
            public ODataPayloadElement PayloadElement { get; set; }
            public string ExpectedJson { get; set; }
        }
    }
}