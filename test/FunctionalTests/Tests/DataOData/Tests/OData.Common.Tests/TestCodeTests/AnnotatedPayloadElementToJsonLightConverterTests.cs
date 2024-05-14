//---------------------------------------------------------------------
// <copyright file="AnnotatedPayloadElementToJsonConverterTests.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for using the AnnotatedPayloadElementToJsonConverter to serialize JSON Light payloads.
    /// </summary>
    [TestClass, TestCase]
    public class AnnotatedPayloadElementToJsonConverterTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency]
        public AnnotatedPayloadElementToJsonConverter JsonSerializer { get; set; }

        [InjectDependency]
        public JsonValueComparer JsonValueComparer { get; set; }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize feeds.")]
        public void JsonTaupoSerializerFeedTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Empty feed
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.EntitySet().WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[]
                        }"
                },
                // Empty feed with count and next link
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .InlineCount(42)
                        .NextLink("http://odata.org/next")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataCountAnnotationName + @""":42,
                          """ + JsonConstants.ODataValuePropertyName + @""":[],
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataNextLinkAnnotationName + @""":""http://odata.org/next""
                        }"
                },
                // Feed with single entry
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .Append(PayloadBuilder.Entity().PrimitiveProperty("ID", (long)42))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[
                            { ""ID"":42 }
                          ]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize entries.")]
        public void JsonTaupoSerializerEntryTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Entry with only ID
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .PrimitiveProperty("ID", (long)42)
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          ""ID"":42
                        }"
                },
                // Entry with ID and typename
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42
                        }"
                },
                // Entry with all metadata expanded
                new JsonSerializerTestCase
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
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.CustomerWithImage"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataIdAnnotationName + @""":""CustomerId"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataETagAnnotationName + @""":""etag"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataEditLinkAnnotationName + @""":""http://odata.org/editlink"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataReadLinkAnnotationName + @""":""http://odata.org/readlink"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataMediaEditLinkAnnotationName + @""":""http://odata.org/streameditlink"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataMediaReadLinkAnnotationName + @""":""http://odata.org/streamreadlink"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataMediaContentTypeAnnotationName + @""":""image/jpg"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataMediaETagAnnotationName + @""":""stream-etag"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataActionsAnnotationName + @""":{ ""./metadata"":[{ ""title"":""ActionTitle"", ""target"":""http://odata.org/target""}], ""./metadata2"":[{ ""title"":""ActionTitle2"", ""target"":""http://odata.org/target2""}]},
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataFunctionsAnnotationName + @""":{ ""./metadata"":[{ ""title"":""ActionTitle"", ""target"":""http://odata.org/target""}, { ""title"":""ActionTitle2"", ""target"":""http://odata.org/target2""}]},
                          ""ID"":42
                        }"
                },
                // Entry with deferred navigation and association properties
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .NavigationProperty("NavProp1", "http://odata.org/NavProp1", "http://odata.org/AssocProp1")
                        .NavigationProperty("NavProp2", "http://odata.org/NavProp2")
                        .NavigationProperty("NavProp3", null, "http://odata.org/AssocProp3")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42,
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp1", JsonConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp1"",
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp1", JsonConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp1"",
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp2", JsonConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp2"",
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp3", JsonConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp3""
                        }"
                },
                // Entry with navigation link with expanded entry
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.Order").PrimitiveProperty("ID", ((long)43)))
                        .ExpandedNavigationProperty("NavProp2", PayloadBuilder.Entity("TestModel.Order").PrimitiveProperty("ID", ((long)43)), "http://odata.org/NavProp2")
                        .ExpandedNavigationProperty("NavProp3", PayloadBuilder.Entity("TestModel.Order").PrimitiveProperty("ID", ((long)43)), new DeferredLink { UriString = "http://odata.org/AssocProp3" })
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42,
                          ""NavProp1"":{ """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.Order"",""ID"":43 },
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp2", JsonConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp2"",
                          ""NavProp2"":{ """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.Order"",""ID"":43 },
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp3", JsonConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp3"",
                          ""NavProp3"":{ """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.Order"",""ID"":43 }
                        }"
                },
                // Entry with navigation link with expanded feed
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", (long)42)
                        .ExpandedNavigationProperty("NavProp1", PayloadBuilder.EntitySet())
                        .ExpandedNavigationProperty("NavProp2", PayloadBuilder.EntitySet(), "http://odata.org/NavProp2")
                        .ExpandedNavigationProperty("NavProp3", PayloadBuilder.EntitySet(), new DeferredLink { UriString = "http://odata.org/AssocProp3" })
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.Customer"",
                          ""ID"":42,
                          ""NavProp1"":[],
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp2", JsonConstants.ODataNavigationLinkUrlAnnotationName) + @""":""http://odata.org/NavProp2"",
                          ""NavProp2"":[],
                          """ + JsonUtils.GetPropertyAnnotationName("NavProp3", JsonConstants.ODataAssociationLinkUrlAnnotationName) + @""":""http://odata.org/AssocProp3"",
                          ""NavProp3"":[]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    PayloadFormatVersionAnnotatingVisitor.AnnotateJson(testCase.PayloadElement, DataServiceProtocolVersion.Unspecified, false);
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level properties.")]
        public void JsonTaupoSerializerPropertyTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Null property
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty("Prop", null).WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          ""value"":null
                        }"
                },
                // Primitive property
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty("Prop", "" + JsonConstants.ODataValuePropertyName + @"").WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":""" + JsonConstants.ODataValuePropertyName + @"""
                        }"
                },
                // Complex property
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Property("Prop", 
                        PayloadBuilder.ComplexValue("TestModel.City")
                            .PrimitiveProperty("City", "Vienna"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.City"", ""City"":""Vienna""
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level collection properties.")]
        public void JsonTaupoSerializerCollectionPropertyTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Primitive collection property
                new JsonSerializerTestCase
                {
                    PayloadElement = new PrimitiveMultiValueProperty("Prop", PayloadBuilder.PrimitiveMultiValue().Item((long)1).Item((long)2)).WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[1,2]
                        }"
                },
                // Complex collection property
                new JsonSerializerTestCase
                {
                    PayloadElement = new ComplexMultiValueProperty("Prop", 
                        PayloadBuilder.ComplexMultiValue()
                            .Item(PayloadBuilder.ComplexValue("TestModel.City")
                                .PrimitiveProperty("City", "Vienna")))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[{ """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.City"", ""City"":""Vienna"" }]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level (streamable) collections.")]
        public void JsonTaupoSerializerCollectionTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Primitive collection
                new JsonSerializerTestCase
                {
                    PayloadElement = new PrimitiveCollection(PayloadBuilder.PrimitiveValue((long)1), PayloadBuilder.PrimitiveValue((long)2)).WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[1,2]
                        }"
                },
                // Complex collection
                new JsonSerializerTestCase
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.City")
                                .PrimitiveProperty("City", "Vienna"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[{ """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.City"", ""City"":""Vienna"" }]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize top-level errors.")]
        public void JsonTaupoSerializerErrorTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Top-level error
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.Error("error-code").Message("error-message"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataErrorPropertyName + @""":{""code"":""error-code"",""message"": ""error-message""}}
                        }"
                },
                // Top-level error with inner error
                new JsonSerializerTestCase
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
                          """ + JsonConstants.ODataErrorPropertyName + @""":{
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
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize service documents.")]
        public void JsonTaupoSerializerServiceDocumentTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Service document
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection("Coll1Title", "Coll1Href")
                            .ResourceCollection("Coll2Title", "Coll2Href")
                            .WithTitle("WorkspaceTitle"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[
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
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize entity reference links.")]
        public void JsonTaupoSerializerEntityReferenceLinkTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Single entity reference link
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/erl")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataIdAnnotationName + @""":""http://odata.org/erl""
                        }"
                },
                // Collection of entity reference links 
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.LinkCollection()
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl1"))
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl2"))
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataValuePropertyName + @""":[
                            { """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataIdAnnotationName + @""":""http://odata.org/erl1"" },
                            { """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataIdAnnotationName + @""":""http://odata.org/erl2"" }
                          ]
                        }"
                },
                // Collection of entity reference links with inline count and next link
                new JsonSerializerTestCase
                {
                    PayloadElement = PayloadBuilder.LinkCollection()
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl1"))
                        .Item(PayloadBuilder.DeferredLink("http://odata.org/erl2"))
                        .InlineCount(42)
                        .NextLink("http://odata.org/nextlink")
                        .WithContextUri("http://odata.org/metadatauri"),
                    ExpectedJson = @"
                        {
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + @""":""http://odata.org/metadatauri"",
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataCountAnnotationName + @""": 42,
                          """ + JsonConstants.ODataValuePropertyName + @""":[
                            { """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataIdAnnotationName + @""":""http://odata.org/erl1"" },
                            { """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataIdAnnotationName + @""":""http://odata.org/erl2"" }
                          ],
                          """ + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataNextLinkAnnotationName + @""":""http://odata.org/nextlink""
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    PayloadFormatVersionAnnotatingVisitor.AnnotateJson(testCase.PayloadElement, DataServiceProtocolVersion.Unspecified, false);
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly serialize parameters.")]
        public void JsonTaupoSerializerParameterTest()
        {
            var testCases = new JsonSerializerTestCase[]
            {
                // Parameter payload with null, primitive, complex and collection parameters
                new JsonSerializerTestCase
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
                          ""ComplexProp"":{""" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + @""":""TestModel.CityType"", ""Name"":""Vienna""},
                          ""PrimitiveColl"":[1,2],
                          ""ComplexColl"":[{""Name"":""Vienna""}, {""Name"":""Prague""}]
                        }"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    JsonValue actualValue = this.JsonSerializer.ConvertToJsonValue(testCase.PayloadElement);
                    JsonValue expectedValue = JsonTextPreservingParser.ParseValue(new StringReader(testCase.ExpectedJson));

                    this.JsonValueComparer.Compare(expectedValue, actualValue);
                });
        }

        private sealed class JsonSerializerTestCase
        {
            public ODataPayloadElement PayloadElement { get; set; }
            public string ExpectedJson { get; set; }
        }
    }
}