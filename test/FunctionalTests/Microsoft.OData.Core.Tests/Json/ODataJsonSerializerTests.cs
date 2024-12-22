﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests and short-span integration tests for ODataJsonSerializer.
    /// </summary>
    public class ODataJsonSerializerTests
    {
        [Fact]
        public void WritePayloadStartWritesJsonPaddingStuffIfSpecified()
        {
            var result = SetupSerializerAndRunTest("functionName", serializer => serializer.WritePayloadStart());
            Assert.StartsWith("functionName(", result);
        }

        [Fact]
        public void WritePayloadEndWritesClosingParenthesisIfJsonPaddingSpecified()
        {
            var result = SetupSerializerAndRunTest("functionName", serializer =>
            {
                serializer.WritePayloadStart();
                serializer.WritePayloadEnd();
            });

            Assert.EndsWith(")", result);
        }

        [Fact]
        public void WillNotWriteJsonPaddingStuffIfUnspecified()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                serializer.WritePayloadStart();
                serializer.WritePayloadEnd();
            });

            Assert.DoesNotContain("(", result);
            Assert.DoesNotContain(")", result);
        }

        [Fact]
        public void WriteTopLevelErrorUsesProvidedErrorCode()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError { Code = "Error Code" };
                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"code\":\"Error Code\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorUsesProvidedMessage()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError { Message = "error message text" };
                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"message\":\"error message text\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorUsesProvidedTarget()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError { Target = "error target text" };
                serializer.WriteTopLevelError(error, includeDebugInformation: false);
            });

            Assert.Contains("\"target\":\"error target text\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorHasCorrectDefaults()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"code\":\"\"", result);
            Assert.Contains("\"message\":\"\"", result);
            Assert.DoesNotContain("\"target\"", result);
            Assert.DoesNotContain("\"details\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithStringInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.primitive", new ODataPrimitiveValue("stringValue"));
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"@sample.primitive\":\"stringValue\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithDateTimeInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var primitiveValue = new ODataPrimitiveValue(new DateTimeOffset(2000, 1, 1, 12, 30, 0, new TimeSpan()));
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.primitive", primitiveValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"sample.primitive@odata.type\":\"#DateTimeOffset\",\"@sample.primitive\":\"2000-01-01T12:30:00Z\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithDateInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var primitiveValue = new ODataPrimitiveValue(new Date(2014, 8, 8));
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.primitive", primitiveValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"sample.primitive@odata.type\":\"#Date\",\"@sample.primitive\":\"2014-08-08\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithTimeOfDayInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var primitiveValue = new ODataPrimitiveValue(new TimeOfDay(12, 30, 5, 90));
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.primitive", primitiveValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"sample.primitive@odata.type\":\"#TimeOfDay\",\"@sample.primitive\":\"12:30:05.0900000\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithStringInstanceAnnotationWithTypeNameAttribute()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var primitiveValue = new ODataPrimitiveValue("stringValue");
                primitiveValue.TypeAnnotation = new ODataTypeAnnotation("Custom.Type");
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.primitive", primitiveValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"sample.primitive@odata.type\":\"#Custom.Type\",\"@sample.primitive\":\"stringValue\"", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithResourceInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var resourceValue = new ODataResourceValue();
                resourceValue.TypeName = "ns.ErrorDetails";
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.complex", resourceValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"@sample.complex\":{\"@odata.type\":\"#ns.ErrorDetails\"}", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithResourceInstanceAnnotationNoTypeNameShouldThrow()
        {
            SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var resourceValue = new ODataResourceValue();
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.complex", resourceValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                Action writeError = () => serializer.WriteTopLevelError(error, false);
                writeError.Throws<ODataException>(SRResources.WriterValidationUtils_MissingTypeNameWithMetadata);
            });
        }

        [Fact]
        public void WriteTopLevelErrorWithCollectionOfResourceInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var collection = new ODataCollectionValue
                {
                    TypeName = "Collection(ns.ErrorDetails)",
                    Items = new[] { new ODataResourceValue(), new ODataResourceValue { TypeName = "ns.ErrorDetails" } }
                };
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.collection", collection);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            Assert.Contains("\"sample.collection@odata.type\":\"#Collection(ns.ErrorDetails)\",\"@sample.collection\":[{},{}]", result);
        }

        [Fact]
        public void WriteTopLevelErrorWithCollectionOfResourceInstanceAnnotationWithNoTypeNameShouldThrow()
        {
            SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var collection = new ODataCollectionValue
                {
                    Items = new[] { new ODataResourceValue(), new ODataResourceValue { TypeName = "ns.ErrorDetails" } }
                };
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.collection", collection);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                Action writeError = () => serializer.WriteTopLevelError(error, false);
                writeError.Throws<ODataException>(SRResources.WriterValidationUtils_MissingTypeNameWithMetadata);
            });
        }

        [Fact]
        public void UrlToStringShouldThrowWithNoMetadataAndMetadataDocumentUriIsNotProvided()
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream, null, true, false);
            var uri = new Uri("TestUri", UriKind.Relative);
            Action uriToStrongError = () => serializer.UriToString(uri);
            uriToStrongError.Throws<ODataException>(Error.Format(SRResources.ODataJsonSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata, UriUtils.UriToString(uri)));
        }

        [Fact]
        public void UrlToStringShouldReturnAbsoluteUriWithNoMetadata()
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream, null, true);
            string uri = serializer.UriToString(new Uri("TestUri", UriKind.Relative));
            Assert.Equal("http://example.com/TestUri", uri);
        }

        [Fact]
        public void UrlToStringShouldReturnRelativeUriWithMinimalMetadata()
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream);
            serializer.JsonWriter.StartObjectScope();
            serializer.WriteContextUriProperty(ODataPayloadKind.ServiceDocument);
            string uri = serializer.UriToString(new Uri("TestUri", UriKind.Relative));
            Assert.Equal("TestUri", uri);
        }

        [Fact]
        public async Task WritePayloadStartAsync_WritesLeftParenIfJsonPaddingSpecified()
        {
            var result = await SetupSerializerAndRunTestAsync(
                "functionName",
                lightJsonSerializer => lightJsonSerializer.WritePayloadStartAsync());
            Assert.Equal("functionName(", result);
        }

        [Fact]
        public async Task WritePayloadEndAsync_WritesRightParenIfJsonPaddingSpecified()
        {
            var result = await SetupSerializerAndRunTestAsync(
                "functionName",
                async (jsonSerializer) =>
                {
                    await jsonSerializer.WritePayloadStartAsync();
                    await jsonSerializer.WritePayloadEndAsync();
                });

            Assert.Equal("functionName()", result);
        }

        [Fact]
        public async Task WriteContextUriPropertyAsync_WritesInstanceAnnotationContextUri()
        {
            var result = await SetupSerializerAndRunTestAsync(
                null,
                async (jsonSerializer) =>
                {
                    await jsonSerializer.JsonWriter.StartObjectScopeAsync();
                    await jsonSerializer.WriteContextUriPropertyAsync(ODataPayloadKind.ServiceDocument);
                });

            Assert.Equal("{\"@odata.context\":\"http://example.com/$metadata\"", result);
        }

        [Fact]
        public async Task WriteContextUriPropertyAsync_WritesPropertyAnnotationContextUri()
        {
            var result = await SetupSerializerAndRunTestAsync(
                null,
                async (jsonSerializer) =>
                {
                    var property = new ODataProperty { Name = "Prop", Value = 13 };
                    var contextUrlInfo = ODataContextUrlInfo.Create(
                        property.ODataValue, ODataVersion.V4,
                        jsonSerializer.JsonOutputContext.MessageWriterSettings.ODataUri,
                        jsonSerializer.Model);

                    await jsonSerializer.JsonWriter.StartObjectScopeAsync();
                    await jsonSerializer.WriteContextUriPropertyAsync(ODataPayloadKind.Resource,
                        () => contextUrlInfo, /* parentContextUrlInfo */ null, propertyName: "Prop");
                });

            Assert.Equal("{\"Prop@odata.context\":\"http://example.com/$metadata#Edm.Int32\"", result);
        }

        [Fact]
        public async Task WriteTopLevelPayloadAsync_WritesTopLevelPayload()
        {
            var result = await SetupSerializerAndRunTestAsync(
                null,
                (jsonSerializer) =>
                {
                    return jsonSerializer.WriteTopLevelPayloadAsync(
                        () =>
                        {
                            return jsonSerializer.JsonWriter.WriteValueAsync(13);
                        });
                });

            Assert.Equal("13", result);
        }

        [Fact]
        public async Task WriteTopLevelErrorAsync_WritesErrorPayload()
        {
            var result = await SetupSerializerAndRunTestAsync(
                null,
                (jsonSerializer) =>
                {
                    ODataError error = new ODataError
                    {
                        Code = "forbidden",
                        Message = "Access to the resource is forbidden",
                        Target = "Resource",
                        Details = new Collection<ODataErrorDetail>
                        {
                            new ODataErrorDetail { Code = "insufficientPrivileges", Message = "You don't have the required privileges"}
                        },
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("Contact administrator") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("") }
                            }),
                        InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                        {
                            new ODataInstanceAnnotation("ns.workloadId", new ODataPrimitiveValue(new Guid("5a3c4b92-f401-416f-bf88-106cb03efaf4")))
                        }
                    };
                    error.InnerError.Properties.Add("correlationId", new ODataPrimitiveValue(new Guid("4784efae-d1c4-4f1f-baba-e811b3b0826c")));

                    return jsonSerializer.WriteTopLevelErrorAsync(error, true);
                });

            Assert.Equal("{\"error\":{" +
                "\"code\":\"forbidden\"," +
                "\"message\":\"Access to the resource is forbidden\"," +
                "\"target\":\"Resource\"," +
                "\"details\":[{\"code\":\"insufficientPrivileges\",\"message\":\"You don't have the required privileges\"}]," +
                "\"innererror\":{" +
                    "\"message\":\"Contact administrator\"," +
                    "\"type\":\"\"," +
                    "\"stacktrace\":\"\"," +
                    "\"correlationId\":\"4784efae-d1c4-4f1f-baba-e811b3b0826c\"}," +
                "\"ns.workloadId@odata.type\":\"#Guid\"," +
                "\"@ns.workloadId\":\"5a3c4b92-f401-416f-bf88-106cb03efaf4\"" +
            "}}", result);
        }

        /// <summary>
        /// Sets up an ODataJsonSerializer,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private static async Task<string> SetupSerializerAndRunTestAsync(string jsonpFunctionName, Func<ODataJsonSerializer, Task> func)
        {
            var stream = new AsyncStream(new MemoryStream());
            var jsonSerializer = GetSerializer(stream, jsonpFunctionName, isAsync: true);
            await func(jsonSerializer);
            await jsonSerializer.JsonOutputContext.FlushAsync();
            await jsonSerializer.JsonWriter.FlushAsync();
            stream.Position = 0;
            var streamReader = new StreamReader(stream);

            return await streamReader.ReadToEndAsync();
        }

        /// <summary>
        /// Sets up a ODataJsonSerializer, runs the given test code, and then flushes and reads the stream back as a string for
        /// customized verification.
        /// </summary>
        private static string SetupSerializerAndRunTest(string jsonpFunctionName, Action<ODataJsonSerializer> action)
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream, jsonpFunctionName);
            action(serializer);
            serializer.JsonWriter.Flush();
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        private static ODataJsonSerializer GetSerializer(Stream stream, string jsonpFunctionName = null, bool nometadata = false, bool setMetadataDocumentUri = true, bool isAsync = false)
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("ns", "ErrorDetails");
            //var collectionType = new EdmCollectionType(new EdmComplexTypeReference(complexType, false));
            model.AddElement(complexType);

            var settings = new ODataMessageWriterSettings { JsonPCallback = jsonpFunctionName, EnableMessageStreamDisposal = false, Version = ODataVersion.V4 };
            if (setMetadataDocumentUri)
            {
                settings.SetServiceDocumentUri(new Uri("http://example.com"));
            }
            var mediaType = nometadata ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata", "none")) : new ODataMediaType("application", "json");
            var mainModel = TestUtils.WrapReferencedModelsToMainModel(model);
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = stream,
                MediaType = mediaType,
                Encoding = Encoding.Default,
                IsResponse = true,
                IsAsync = isAsync,
                Model = mainModel,
            };
            var context = new ODataJsonOutputContext(messageInfo, settings);
            return new ODataJsonSerializer(context, setMetadataDocumentUri);
        }
    }
}
