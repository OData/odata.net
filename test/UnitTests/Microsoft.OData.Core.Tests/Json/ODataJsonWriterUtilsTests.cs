//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit and short-span integration tests for the ODataJsonWriterUtils.
    /// TODO: Create an interface and make a JsonWriter simulator so we don't have to double test its writing functionality.
    //  TODO: Write unit tests the remaining methods on ODataJsonWriterUtils.
    /// </summary>
    public class ODataJsonWriterCoreUtilsTests
    {
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;
        private ODataMessageWriterSettings messageWriterSettings;

        public ODataJsonWriterCoreUtilsTests()
        {
            this.stringWriter = new StringWriter();
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.messageWriterSettings = new ODataMessageWriterSettings();
        }

        [Fact]
        public void StartJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            messageWriterSettings.JsonPCallback = null;
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, messageWriterSettings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void StartJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            messageWriterSettings.JsonPCallback = "";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, messageWriterSettings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void EndJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            messageWriterSettings.JsonPCallback = null;
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, messageWriterSettings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void EndJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            messageWriterSettings.JsonPCallback = "";
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, messageWriterSettings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void StartAndEndJsonPaddingSuccessTest()
        {
            messageWriterSettings.JsonPCallback = "functionName";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, messageWriterSettings);
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, messageWriterSettings);
            Assert.Equal("functionName()", stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void WriteError_WritesTargetAndDetails()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } }
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: false,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal(@"{""error"":{""code"":"""",""message"":"""",""target"":""any target""," +
                @"""details"":[{""code"":""500"",""message"":""any msg"",""target"":""any target""}]}}", result);
        }


        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization, JsonConstants.ODataErrorInnerErrorInnerErrorName)]
        [InlineData(ODataLibraryCompatibility.None, JsonConstants.ODataErrorInnerErrorName)]
        public void WriteError_InnerErrorWithNestedNullValue(ODataLibraryCompatibility libraryCompatibility, string nestedInnerErrorPropertyName)
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>
            {
                { "stacktrace", "NormalString".ToODataValue() },
                {
                    "MyNewObject",
                    new ODataResourceValue()
                    {
                        TypeName = "ComplexValue",
                        Properties = new List<ODataProperty>()
                {
                    new ODataProperty()
                    {
                        Name = "NestedResourcePropertyName",
                        Value = new ODataResourceValue()
                        {
                            Properties = new List<ODataProperty>()
                            {
                                new ODataProperty()
                                {
                                    Name = "InnerMostPropertyName",
                                    Value = "InnerMostPropertyValue"
                                }
                            }
                        }
                    }
                }
                    }
                }
            };
            IDictionary<string, ODataValue> nestedDict = new Dictionary<string, ODataValue>
            {
                { "nested", null }
            };
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(nestedDict)}
            };

            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 5;
            this.messageWriterSettings.LibraryCompatibility |= libraryCompatibility;
            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
             var result = stringWriter.GetStringBuilder().ToString();
             Assert.Equal("{\"error\":" +
                                "{\"code\":\"\"," +
                                "\"message\":\"\"," +
                                "\"target\":\"any target\"," +
                                "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
                                "\"innererror\":{" +
                                    "\"stacktrace\":\"NormalString\"," +
                                    "\"MyNewObject\":{" +
                                        "\"NestedResourcePropertyName\":{" +
                                            "\"InnerMostPropertyName\":\"InnerMostPropertyValue\"" +
                                          "}" +
                                     "}," +
                                     $"\"{nestedInnerErrorPropertyName}\":{{" +
                                         "\"nested\":null" +
                                     "}" +
                                  "}" +
                                "}" +
                                "}", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization, JsonConstants.ODataErrorInnerErrorInnerErrorName)]
        [InlineData(ODataLibraryCompatibility.None, JsonConstants.ODataErrorInnerErrorName)]
        public void WriteError_InnerErrorWithNestedProperties(ODataLibraryCompatibility libraryCompatibility, string nestedInnerErrorPropertyName)
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>
            {
                { "stacktrace", "NormalString".ToODataValue() },
                { "MyNewObject", new ODataResourceValue() { TypeName = "ComplexValue", Properties = new List<ODataProperty>() { new ODataProperty() { Name = "NestedResourcePropertyName", Value = "NestedPropertyValue" } } } }
            };

            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(properties) }
            };

            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 5;
            this.messageWriterSettings.LibraryCompatibility |= libraryCompatibility;
            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":" +
                               "{\"code\":\"\"," +
                               "\"message\":\"\"," +
                               "\"target\":\"any target\"," +
                               "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
                               "\"innererror\":{" +
                                    "\"stacktrace\":\"NormalString\"," +
                                    "\"MyNewObject\":{" +
                                        "\"NestedResourcePropertyName\":\"NestedPropertyValue\"" +
                                    "}," +
                                    $"\"{nestedInnerErrorPropertyName}\":{{" +
                                        "\"stacktrace\":\"NormalString\"," +
                                        "\"MyNewObject\":{" +
                                            "\"NestedResourcePropertyName\":\"NestedPropertyValue\"" +
                                        "}" +
                                    "}" +
                               "}" +
                               "}" +
                               "}", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization)]
        [InlineData(ODataLibraryCompatibility.None)]
        public void WriteError_InnerErrorWithEmptyStringProperties(ODataLibraryCompatibility libraryCompatibility)
        {
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(
                    new Dictionary<string, ODataValue>
                    {
                        {
                            JsonConstants.ODataErrorInnerErrorMessageName,
                            new ODataPrimitiveValue("The other properties on the inner error object should serialize as empty strings because of using this constructor.")
                        },
                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("") },
                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("") }
                    })
            };

            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 5;
            this.messageWriterSettings.LibraryCompatibility |= libraryCompatibility;
            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":" +
                               "{\"code\":\"\"," +
                               "\"message\":\"\"," +
                               "\"target\":\"any target\"," +
                               "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
                               "\"innererror\":{" +
                                    "\"message\":\"The other properties on the inner error object should serialize as empty strings because of using this constructor.\"," +
                                    "\"type\":\"\"," +
                                    "\"stacktrace\":\"\"" +
                                "}" +
                               "}" +
                               "}", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization)]
        [InlineData(ODataLibraryCompatibility.None)]
        public void WriteError_InnerErrorWithCollectionAndNulls(ODataLibraryCompatibility libraryCompatibility)
        {
            ODataInnerError innerError = new ODataInnerError(
                new Dictionary<string, ODataValue>
                {
                    { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("") },
                    { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("") },
                    { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("") }
                });
            innerError.Properties.Add("ResourceValue", new ODataResourceValue() { Properties = new [] { new ODataProperty() { Name = "PropertyName", Value = "PropertyValue" }, new ODataProperty() { Name = "NullProperty", Value = new ODataNullValue() } } });
            innerError.Properties.Add("NullProperty", new ODataNullValue());
            innerError.Properties.Add("CollectionValue", new ODataCollectionValue() { Items = new List<object>() { new ODataNullValue(), new ODataPrimitiveValue("CollectionValue"), new ODataPrimitiveValue(1) } });

            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
                InnerError = innerError
            };

            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 5;
            this.messageWriterSettings.LibraryCompatibility |= libraryCompatibility;
            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":" +
                               "{\"code\":\"\"," +
                               "\"message\":\"\"," +
                               "\"target\":\"any target\"," +
                               "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
                               "\"innererror\":{" +
                                    "\"message\":\"\"," +
                                    "\"type\":\"\"," +
                                    "\"stacktrace\":\"\"," +
                                    "\"ResourceValue\":{" +
                                        "\"PropertyName\":\"PropertyValue\"," +
                                        "\"NullProperty\":null" +
                                     "}," +
                                    "\"NullProperty\":null," +
                                    "\"CollectionValue\":[null,\"CollectionValue\",1]" +
                                "}" +
                               "}" +
                               "}", result);
        }

        [Fact]
        public void ODataInnerErrorToStringTest()
        {
            ODataInnerError innerError = new ODataInnerError();
            innerError.Properties.Add("ResourceValue", new ODataResourceValue() { Properties = new[] { new ODataProperty { Name = "PropertyName", Value = "PropertyValue" }, new ODataProperty { Name = "NullProperty", Value = new ODataNullValue() } } });
            innerError.Properties.Add("NullProperty", new ODataNullValue());
            innerError.Properties.Add("CollectionValue", new ODataCollectionValue() { Items = new List<object>() { new ODataNullValue(), new ODataPrimitiveValue("CollectionValue"), new ODataPrimitiveValue(1) } });
            innerError.Properties.Add(JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue(""));
            innerError.Properties.Add(JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue(""));
            innerError.Properties.Add(JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue(""));
            string result = innerError.ToJsonString();

            Assert.Equal("{\"ResourceValue\":{" +
                "\"PropertyName\":\"PropertyValue\"," +
                "\"NullProperty\":null}," +
                "\"NullProperty\":null," +
                "\"CollectionValue\":[null,\"CollectionValue\",1]," +
                "\"message\":\"\"," +
                "\"type\":\"\"," +
                "\"stacktrace\":\"\"}", result);
        }
    }
}
