//---------------------------------------------------------------------
// <copyright file="ODataJsonLightSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    /// <summary>
    /// Unit tests and short-span integration tests for ODataJsonLightSerializer.
    /// </summary>
    public class ODataJsonLightSerializerTests
    {
        [Fact]
        public void WritePayloadStartWritesJsonPaddingStuffIfSpecified()
        {
            var result = SetupSerializerAndRunTest("functionName", serializer => serializer.WritePayloadStart());
            result.Should().StartWith("functionName(");
        }

        [Fact]
        public void WritePayloadEndWritesClosingParenthesisIfJsonPaddingSpecified()
        {
            var result = SetupSerializerAndRunTest("functionName", serializer =>
            {
                serializer.WritePayloadStart();
                serializer.WritePayloadEnd();
            });

            result.Should().EndWith(")");
        }

        [Fact]
        public void WillNotWriteJsonPaddingStuffIfUnspecified()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                serializer.WritePayloadStart();
                serializer.WritePayloadEnd();
            });

            result.Should().NotContain("(").And.NotContain(")");
        }

        [Fact]
        public void WriteTopLevelErrorUsesProvidedErrorCode()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError { ErrorCode = "Error Code" };
                serializer.WriteTopLevelError(error, false);
            });

            result.Should().Contain("\"code\":\"Error Code\"");
        }

        [Fact]
        public void WriteTopLevelErrorUsesProvidedMessage()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError { Message = "error message text" };
                serializer.WriteTopLevelError(error, false);
            });

            result.Should().Contain("\"message\":\"error message text\"");
        }

        [Fact]
        public void WriteTopLevelErrorUsesProvidedTarget()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError { Target = "error target text" };
                serializer.WriteTopLevelError(error, includeDebugInformation: false);
            });

            result.Should().Contain("\"target\":\"error target text\"");
        }
        
        [Fact]
        public void WriteTopLevelErrorHasCorrectDefaults()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                serializer.WriteTopLevelError(error, false);
            });

            result.Should().Contain("\"code\":\"\"");
            result.Should().Contain("\"message\":\"\"");
            result.Should().NotContain("\"target\"");
            result.Should().NotContain("\"details\"");
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

            result.Should().Contain("\"@sample.primitive\":\"stringValue\"");
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

            result.Should().Contain("\"sample.primitive@odata.type\":\"#DateTimeOffset\",\"@sample.primitive\":\"2000-01-01T12:30:00Z\"");
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

            result.Should().Contain("\"sample.primitive@odata.type\":\"#Date\",\"@sample.primitive\":\"2014-08-08\"");
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

            result.Should().Contain("\"sample.primitive@odata.type\":\"#TimeOfDay\",\"@sample.primitive\":\"12:30:05.0900000\"");
        }

        [Fact]
        public void WriteTopLevelErrorWithStringInstanceAnnotationWithTypeNameAttribute()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var primitiveValue = new ODataPrimitiveValue("stringValue");
                primitiveValue.SetAnnotation(new SerializationTypeNameAnnotation() { TypeName = "Custom.Type" });
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.primitive", primitiveValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            result.Should().Contain("\"sample.primitive@odata.type\":\"#Custom.Type\",\"@sample.primitive\":\"stringValue\"");
        }

        [Fact]
        public void WriteTopLevelErrorWithComplexInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var complexValue = new ODataComplexValue();
                complexValue.TypeName = "ns.ErrorDetails";
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.complex", complexValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            result.Should().Contain("\"@sample.complex\":{\"@odata.type\":\"#ns.ErrorDetails\"}");
        }

        [Fact]
        public void WriteTopLevelErrorWithComplexInstanceAnnotationNoTypeNameShouldThrow()
        {
            SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var complexValue = new ODataComplexValue();
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.complex", complexValue);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                Action writeError = () => serializer.WriteTopLevelError(error, false);
                writeError.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
            });
        }

        [Fact]
        public void WriteTopLevelErrorWithCollectionOfComplexInstanceAnnotation()
        {
            var result = SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var collection = new ODataCollectionValue
                {
                    TypeName = "Collection(ns.ErrorDetails)",
                    Items = new[] { new ODataComplexValue(), new ODataComplexValue { TypeName = "ns.ErrorDetails" } }
                };
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.collection", collection);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                serializer.WriteTopLevelError(error, false);
            });

            result.Should().Contain("\"sample.collection@odata.type\":\"#Collection(ns.ErrorDetails)\",\"@sample.collection\":[{},{}]");
        }

        [Fact]
        public void WriteTopLevelErrorWithCollectionOfComplexInstanceAnnotationWithNoTypeNameShouldThrow()
        {
            SetupSerializerAndRunTest(null, serializer =>
            {
                ODataError error = new ODataError();
                var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
                var collection = new ODataCollectionValue
                {
                    Items = new[] { new ODataComplexValue(), new ODataComplexValue { TypeName = "ns.ErrorDetails" } }
                };
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation("sample.collection", collection);
                instanceAnnotations.Add(annotation);
                error.InstanceAnnotations = instanceAnnotations;

                Action writeError = () => serializer.WriteTopLevelError(error, false);
                writeError.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
            });
        }

        [Fact]
        public void UrlToStringShouldThrowWithNoMetadataAndMetadataDocumentUriIsNotProvided()
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream, null, true, false);
            var uri = new Uri("TestUri", UriKind.Relative);
            Action uriToStrongError = () => serializer.UriToString(uri);
            uriToStrongError.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata(UriUtils.UriToString(uri)));
        }

        [Fact]
        public void UrlToStringShouldReturnAbsoluteUriWithNoMetadata()
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream, null, true);
            string uri = serializer.UriToString(new Uri("TestUri", UriKind.Relative));
            uri.Should().Equals("http://example.com/TestUri");
        }

        [Fact]
        public void UrlToStringShouldReturnRelativeUriWithMinimalMetadata()
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream);
            serializer.JsonWriter.StartObjectScope();
            serializer.WriteContextUriProperty(ODataPayloadKind.ServiceDocument);
            string uri = serializer.UriToString(new Uri("TestUri", UriKind.Relative));
            uri.Should().Equals("TestUri");
        }

        /// <summary>
        /// Sets up a ODataJsonLightSerializer, runs the given test code, and then flushes and reads the stream back as a string for
        /// customized verification.
        /// </summary>
        private static string SetupSerializerAndRunTest(string jsonpFunctionName, Action<ODataJsonLightSerializer> action)
        {
            var stream = new MemoryStream();
            var serializer = GetSerializer(stream, jsonpFunctionName);
            action(serializer);
            serializer.JsonWriter.Flush();
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        private static ODataJsonLightSerializer GetSerializer(Stream stream, string jsonpFunctionName = null, bool nometadata = false, bool setMetadataDocumentUri = true)
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("ns", "ErrorDetails");
            //var collectionType = new EdmCollectionType(new EdmComplexTypeReference(complexType, false));
            model.AddElement(complexType);

            var settings = new ODataMessageWriterSettings { JsonPCallback = jsonpFunctionName, DisableMessageStreamDisposal = true, Version = ODataVersion.V4 };
            if (setMetadataDocumentUri)
            {
                settings.SetServiceDocumentUri(new Uri("http://example.com"));
            }
            ODataMediaType mediaType = nometadata ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata", "none")) : new ODataMediaType("application", "json");
            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel(model);
            var context = new ODataJsonLightOutputContext(ODataFormat.Json, stream, mediaType, Encoding.Default, settings, true, true, mainModel, null);
            return new ODataJsonLightSerializer(context, setMetadataDocumentUri);
        }
    }
}
