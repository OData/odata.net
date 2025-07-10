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
    public class ODataJsonWriterUtilsTests
    {
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;
        private ODataMessageWriterSettings settings;

        public ODataJsonWriterUtilsTests()
        {
            this.stringWriter = new StringWriter();
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.settings = new ODataMessageWriterSettings();
        }

        [Fact]
        public void StartJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void StartJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void EndJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void EndJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void StartAndEndJsonPaddingSuccessTest()
        {
            settings.JsonPCallback = "functionName";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            Assert.Equal("functionName()", stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public void WriteError_WritesTargetAndDetails()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } }
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: false,
                maxInnerErrorDepth: 0,
                writingJsonLight: false);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal(@"{""error"":{""code"":"""",""message"":"""",""target"":""any target""," +
                @"""details"":[{""code"":""500"",""target"":""any target"",""message"":""any msg""}]}}", result);
        }


        [Fact]
        public void WriteError_InnerErrorWithNestedNullValue()
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>();
            properties.Add("stacktrace", "NormalString".ToODataValue());
            properties.Add("MyNewObject", new ODataResourceValue()
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
            });
            IDictionary<string, ODataValue> nestedDict = new Dictionary<string, ODataValue>();
            nestedDict.Add("nested", null);
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(nestedDict)}
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5,
                writingJsonLight: false);
             var result = stringWriter.GetStringBuilder().ToString();
             Assert.Equal("{\"error\":" +
                                "{\"code\":\"\"," +
                                "\"message\":\"\"," +
                                "\"target\":\"any target\"," +
                                "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                                "\"innererror\":{" +
                                    "\"stacktrace\":\"NormalString\"," +
                                    "\"MyNewObject\":{" +
                                        "\"NestedResourcePropertyName\":{" +
                                            "\"InnerMostPropertyName\":\"InnerMostPropertyValue\"" +
                                          "}" +
                                     "}," +
                                     "\"internalexception\":{" +
                                            "\"nested\":null" +
                                      "}" +
                                  "}" +
                                "}" +
                                "}", result);
        }

        [Fact]
        public void WriteError_InnerErrorWithNestedProperties()
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>();
            properties.Add("stacktrace", "NormalString".ToODataValue());
            properties.Add("MyNewObject", new ODataResourceValue() { TypeName = "ComplexValue", Properties = new List<ODataProperty>() { new ODataProperty() { Name = "NestedResourcePropertyName", Value = "NestedPropertyValue" } } });

            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(properties) }
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5,
                writingJsonLight: false);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":" +
                               "{\"code\":\"\"," +
                               "\"message\":\"\"," +
                               "\"target\":\"any target\"," +
                               "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                               "\"innererror\":{" +
                                    "\"stacktrace\":\"NormalString\"," +
                                    "\"MyNewObject\":{" +
                                        "\"NestedResourcePropertyName\":\"NestedPropertyValue\"" +
                                    "}," +
                                    "\"internalexception\":{" +
                                        "\"stacktrace\":\"NormalString\"," +
                                        "\"MyNewObject\":{" +
                                            "\"NestedResourcePropertyName\":\"NestedPropertyValue\"" +
                                        "}" +
                                    "}" +
                               "}" +
                               "}" +
                               "}", result);
        }

        [Fact]
        public void WriteError_InnerErrorWithEmptyStringProperties()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError() { Message = "The other properties on the inner error object should serialize as empty strings because of using this constructor."}
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5,
                writingJsonLight: false);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":" +
                               "{\"code\":\"\"," +
                               "\"message\":\"\"," +
                               "\"target\":\"any target\"," +
                               "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                               "\"innererror\":{" +
                                    "\"message\":\"The other properties on the inner error object should serialize as empty strings because of using this constructor.\"," +
                                    "\"type\":\"\"," +
                                    "\"stacktrace\":\"\"" +
                                "}" +
                               "}" +
                               "}", result);
        }

        [Fact]
        public void WriteError_InnerErrorWithCollectionAndNulls()
        {
            ODataInnerError innerError = new ODataInnerError();
            innerError.Properties.Add("ResourceValue", new ODataResourceValue() { Properties = new ODataProperty[] { new ODataProperty() { Name = "PropertyName", Value = "PropertyValue" }, new ODataProperty() { Name = "NullProperty", Value = ODataNullValue.Instance } } });
            innerError.Properties.Add("NullProperty", ODataNullValue.Instance);
            innerError.Properties.Add("CollectionValue", new ODataCollectionValue() { Items = new List<object>() { ODataNullValue.Instance, new ODataPrimitiveValue("CollectionValue"), new ODataPrimitiveValue(1) } });

            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = innerError
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5,
                writingJsonLight: false);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":" +
                               "{\"code\":\"\"," +
                               "\"message\":\"\"," +
                               "\"target\":\"any target\"," +
                               "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
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
            innerError.Properties.Add("ResourceValue", new ODataResourceValue(){Properties = new ODataProperty[] { new ODataProperty() { Name = "PropertyName", Value = "PropertyValue"}, new ODataProperty(){Name = "NullProperty", Value = ODataNullValue.Instance}}});
            innerError.Properties.Add("NullProperty", ODataNullValue.Instance);
            innerError.Properties.Add("CollectionValue", new ODataCollectionValue(){Items = new List<object>() {ODataNullValue.Instance, new ODataPrimitiveValue("CollectionValue"), new ODataPrimitiveValue(1)}});

            string result = innerError.ToJson();

            Assert.Equal("{\"message\":\"\"," +
                               "\"type\":\"\"," +
                               "\"stacktrace\":\"\"," +
                               "\"innererror\":{}," +
                               "\"ResourceValue\":{" +
                                    "\"PropertyName\":\"PropertyValue\"," +
                                    "\"NullProperty\":null" +
                                    "}," +
                               "\"NullProperty\":null," +
                               "\"CollectionValue\":[null,\"CollectionValue\",1]" +
                               "}", result);
        }
    }
}
