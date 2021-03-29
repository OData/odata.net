//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtilsAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for ODataJsonWriterUtils asynchronous API.
    /// </summary>
    public class ODataJsonWriterUtilsAsyncTests
    {
        private StringWriter stringWriter;
        private IJsonWriterAsync jsonWriter;
        private ODataMessageWriterSettings settings;
        private Func<IEnumerable<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate;

        public ODataJsonWriterUtilsAsyncTests()
        {
            this.stringWriter = new StringWriter();
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.settings = new ODataMessageWriterSettings();
            this.writeInstanceAnnotationsDelegate = async (IEnumerable<ODataInstanceAnnotation> instanceAnnotations) => await TaskUtils.CompletedTask;
        }

        [Fact]
        public async Task StartJsonPaddingIfRequiredAsync_DoesNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            await ODataJsonWriterUtils.StartJsonPaddingIfRequiredAsync(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public async Task StartJsonPaddingIfRequiredAsync_DoesNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            await ODataJsonWriterUtils.StartJsonPaddingIfRequiredAsync(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public async Task EndJsonPaddingIfRequiredAsync_DoesNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            await ODataJsonWriterUtils.EndJsonPaddingIfRequiredAsync(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public async Task EndJsonPaddingIfRequiredAsync_DoesNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            await ODataJsonWriterUtils.EndJsonPaddingIfRequiredAsync(this.jsonWriter, settings);
            Assert.Empty(stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public async Task StartAndEndJsonPaddingAsync_SuccessTest()
        {
            settings.JsonPCallback = "functionName";
            await ODataJsonWriterUtils.StartJsonPaddingIfRequiredAsync(this.jsonWriter, settings);
            await ODataJsonWriterUtils.EndJsonPaddingIfRequiredAsync(this.jsonWriter, settings);
            Assert.Equal("functionName()", stringWriter.GetStringBuilder().ToString());
        }

        [Fact]
        public async Task WriteErrorAsync_WritesTargetAndDetails()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } }
            };

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: false,
                maxInnerErrorDepth: 0);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]" +
            "}}", result);
        }


        [Fact]
        public async Task WriteErrorAsync_InnerErrorWithNestedNullValue()
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>();
            properties.Add("stacktrace", "NormalString".ToODataValue());
            properties.Add("MyNewObject", new ODataResourceValue()
            {
                TypeName = "ComplexValue",
                Properties = new List<ODataProperty>()
                {
                    new ODataProperty
                    {
                        Name = "NestedResourcePropertyName",
                        Value = new ODataResourceValue()
                        {
                            Properties = new List<ODataProperty>()
                            {
                                new ODataProperty
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
                Details = new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(nestedDict)}
            };

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5);
             var result = stringWriter.GetStringBuilder().ToString();
             Assert.Equal("{\"error\":{" +
                 "\"code\":\"\"," +
                 "\"message\":\"\"," +
                 "\"target\":\"any target\"," +
                 "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                 "\"innererror\":{" +
                    "\"stacktrace\":\"NormalString\"," +
                    "\"MyNewObject\":{" +
                        "\"NestedResourcePropertyName\":{\"InnerMostPropertyName\":\"InnerMostPropertyValue\"}" +
                    "}," +
                    "\"internalexception\":{\"nested\":null}" +
                "}" +
            "}}", result);
        }

        [Fact]
        public async Task WriteErrorAsync_InnerErrorWithNestedProperties()
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>();
            properties.Add("stacktrace", "NormalString".ToODataValue());
            properties.Add("MyNewObject", new ODataResourceValue
            { 
                TypeName = "ComplexValue",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "NestedResourcePropertyName",
                        Value = "NestedPropertyValue"
                    }
                }
            });

            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(properties) }
            };

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                "\"innererror\":{" +
                    "\"stacktrace\":\"NormalString\"," +
                    "\"MyNewObject\":{\"NestedResourcePropertyName\":\"NestedPropertyValue\"}," +
                    "\"internalexception\":{" +
                        "\"stacktrace\":\"NormalString\"," +
                        "\"MyNewObject\":{\"NestedResourcePropertyName\":\"NestedPropertyValue\"}" +
                    "}" +
                "}" +
            "}}", result);
        }

        [Fact]
        public async Task WriteErrorAsync_InnerErrorWithEmptyStringProperties()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError
                {
                    Message = "The other properties on the inner error object should serialize as empty strings because of using this constructor."
                }
            };

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                "\"innererror\":{" +
                    "\"message\":\"The other properties on the inner error object should serialize as empty strings because of using this constructor.\"," +
                    "\"type\":\"\"," +
                    "\"stacktrace\":\"\"" +
                "}" +
            "}}", result);
        }

        [Fact]
        public async Task WriteErrorAsync_InnerErrorWithCollectionAndNulls()
        {
            ODataInnerError innerError = new ODataInnerError();
            innerError.Properties.Add("ResourceValue", new ODataResourceValue
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "PropertyName", Value = "PropertyValue" },
                    new ODataProperty { Name = "NullProperty", Value = new ODataNullValue() }
                }
            });
            innerError.Properties.Add("NullProperty", new ODataNullValue());
            innerError.Properties.Add("CollectionValue", new ODataCollectionValue
            { 
                Items = new List<object>
                {
                    new ODataNullValue(),
                    new ODataPrimitiveValue("CollectionValue"), new ODataPrimitiveValue(1)
                }
            });

            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = innerError
            };

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                "\"innererror\":{" +
                    "\"message\":\"\"," +
                    "\"type\":\"\"," +
                    "\"stacktrace\":\"\"," +
                    "\"ResourceValue\":{\"PropertyName\":\"PropertyValue\",\"NullProperty\":null}," +
                    "\"NullProperty\":null," +
                    "\"CollectionValue\":[null,\"CollectionValue\",1]" +
                "}" +
            "}}", result);
        }

        [Fact]
        public async Task WriteErrorAsync_WritesInstanceAnnotations()
        {
            var error = new ODataError { };
            error.InstanceAnnotations.Add(new ODataInstanceAnnotation("NS.AnnotationName", new ODataPrimitiveValue("AnnotationValue")));

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                async (IEnumerable<ODataInstanceAnnotation> instanceAnnotations) =>
                {
                    foreach (var annotation in instanceAnnotations)
                    {
                        await this.jsonWriter.WriteNameAsync(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + annotation.Name);
                        await this.jsonWriter.WritePrimitiveValueAsync(((ODataPrimitiveValue)annotation.Value).Value);
                    }
                },
                error,
                includeDebugInformation: false,
                maxInnerErrorDepth: 0);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"@NS.AnnotationName\":\"AnnotationValue\"" +
            "}}", result);
        }


        [Fact]
        public async Task WriteErrorAsync_ShouldThrowArgumentNullException_ForWriteInstanceAnnotationsDelegateIsNull()
        {
            var error = new ODataError { };

            await Assert.ThrowsAsync<ArgumentNullException>(
                "writeInstanceAnnotationsDelegate",
                async () =>
                {
                    await ODataJsonWriterUtils.WriteErrorAsync(
                    this.jsonWriter,
                    null,
                    error,
                    includeDebugInformation: false,
                    maxInnerErrorDepth: 0);
                });
        }
    }
}
