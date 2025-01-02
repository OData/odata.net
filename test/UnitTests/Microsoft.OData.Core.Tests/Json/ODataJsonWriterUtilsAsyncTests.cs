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
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for ODataJsonWriterUtils asynchronous API.
    /// </summary>
    public class ODataJsonWriterUtilsAsyncTests
    {
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;
        private Func<ICollection<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate;
        private ODataMessageWriterSettings messageWriterSettings;

        public ODataJsonWriterUtilsAsyncTests()
        {
            this.stringWriter = new StringWriter();
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.writeInstanceAnnotationsDelegate = async (ICollection<ODataInstanceAnnotation> instanceAnnotations) => await TaskUtils.CompletedTask;
            this.messageWriterSettings = new ODataMessageWriterSettings();
        }

        [Fact]
        public async Task WriteErrorAsync_WritesTargetAndDetails()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } }
            };

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: false,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]" +
            "}}", result);
        }

        [Fact]
        public async Task WriteErrorAsync_WritesTargetAndDetailsWithEscapedCharacters()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg with \"escaped characters\"" } }
            };

            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: false,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"message\":\"any msg with \\\"escaped characters\\\"\",\"target\":\"any target\"}]" +
            "}}", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization, JsonConstants.ODataErrorInnerErrorInnerErrorName)]
        [InlineData(ODataLibraryCompatibility.None, JsonConstants.ODataErrorInnerErrorName)]
        public async Task WriteErrorAsync_InnerErrorWithNestedNullValue(ODataLibraryCompatibility libraryCompatibility, string nestedInnerErrorPropertyName)
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
                Details = new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(nestedDict)}
            };

            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 5;
            this.messageWriterSettings.LibraryCompatibility |= libraryCompatibility;
            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
             var result = stringWriter.GetStringBuilder().ToString();
             Assert.Equal("{\"error\":{" +
                 "\"code\":\"\"," +
                 "\"message\":\"\"," +
                 "\"target\":\"any target\"," +
                 "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
                 "\"innererror\":{" +
                    "\"stacktrace\":\"NormalString\"," +
                    "\"MyNewObject\":{" +
                        "\"NestedResourcePropertyName\":{\"InnerMostPropertyName\":\"InnerMostPropertyValue\"}" +
                    "}," +
                    $"\"{nestedInnerErrorPropertyName}\":{{\"nested\":null}}" +
                "}" +
            "}}", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization, JsonConstants.ODataErrorInnerErrorInnerErrorName)]
        [InlineData(ODataLibraryCompatibility.None, JsonConstants.ODataErrorInnerErrorName)]
        public async Task WriteErrorAsync_InnerErrorWithNestedProperties(ODataLibraryCompatibility libraryCompatibility, string nestedInnerErrorPropertyName)
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>
            {
                { "stacktrace", "NormalString".ToODataValue() },
                {
                    "MyNewObject",
                    new ODataResourceValue
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
                    }
                }
            };

            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties) { InnerError = new ODataInnerError(properties) }
            };

            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 5;
            this.messageWriterSettings.LibraryCompatibility |= libraryCompatibility;
            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
                "\"innererror\":{" +
                    "\"stacktrace\":\"NormalString\"," +
                    "\"MyNewObject\":{\"NestedResourcePropertyName\":\"NestedPropertyValue\"}," +
                    $"\"{nestedInnerErrorPropertyName}\":{{" +
                        "\"stacktrace\":\"NormalString\"," +
                        "\"MyNewObject\":{\"NestedResourcePropertyName\":\"NestedPropertyValue\"}" +
                    "}" +
                "}" +
            "}}", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization)]
        [InlineData(ODataLibraryCompatibility.None)]
        public async Task WriteErrorAsync_InnerErrorWithEmptyStringProperties(ODataLibraryCompatibility libraryCompatibility)
        {
            var error = new ODataError
            {
                Target = "any target",
                Details = new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
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
            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
                "\"innererror\":{" +
                    "\"message\":\"The other properties on the inner error object should serialize as empty strings because of using this constructor.\"," +
                    "\"type\":\"\"," +
                    "\"stacktrace\":\"\"" +
                "}" +
            "}}", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization)]
        [InlineData(ODataLibraryCompatibility.None)]
        public async Task WriteErrorAsync_InnerErrorWithCollectionAndNulls(ODataLibraryCompatibility libraryCompatibility)
        {
            ODataInnerError innerError = new ODataInnerError(
                new Dictionary<string, ODataValue>
                {
                    { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("") },
                    { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("") },
                    { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("") }
                });
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
                Details = new[] { new ODataErrorDetail { Code = "500", Target = "any target", Message = "any msg" } },
                InnerError = innerError
            };

            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 5;
            this.messageWriterSettings.LibraryCompatibility |= libraryCompatibility;
            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                this.writeInstanceAnnotationsDelegate,
                error,
                includeDebugInformation: true,
                this.messageWriterSettings);
            var result = stringWriter.GetStringBuilder().ToString();
            Assert.Equal("{\"error\":{" +
                "\"code\":\"\"," +
                "\"message\":\"\"," +
                "\"target\":\"any target\"," +
                "\"details\":[{\"code\":\"500\",\"message\":\"any msg\",\"target\":\"any target\"}]," +
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
                async (ICollection<ODataInstanceAnnotation> instanceAnnotations) =>
                {
                    foreach (var annotation in instanceAnnotations)
                    {
                        await this.jsonWriter.WriteNameAsync(ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + annotation.Name);
                        await this.jsonWriter.WritePrimitiveValueAsync(((ODataPrimitiveValue)annotation.Value).Value);
                    }
                },
                error,
                includeDebugInformation: false,
                this.messageWriterSettings);
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
                    this.messageWriterSettings);
                });
        }
    }
}
