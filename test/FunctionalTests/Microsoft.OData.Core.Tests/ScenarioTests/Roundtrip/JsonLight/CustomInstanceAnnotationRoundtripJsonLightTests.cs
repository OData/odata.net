//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationRoundtripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class CustomInstanceAnnotationRoundtripTests
    {
        private EdmModel model;
        private MemoryStream stream;
        private EdmComplexType complexType;

        public CustomInstanceAnnotationRoundtripTests()
        {
            this.model = new EdmModel();
            this.complexType = new EdmComplexType("ns", "ErrorDetails");
            var property = new EdmStructuralProperty(complexType, "ErrorDetailName", EdmCoreModel.Instance.GetString(false));
            this.complexType.AddProperty(property);
            this.model.AddElement(complexType);

            this.stream = new MemoryStream();
        }

        [Fact]
        public void ZeroCustomInstanceAnnotationsOnErrorShouldRoundtrip()
        {
            ODataError error = this.WriteThenReadErrorWithInstanceAnnotation(new KeyValuePair<string, ODataValue>[] { });
            error.Should().NotBeNull();
            ((ODataAnnotatable)error).GetAnnotation<object>().Should().BeNull();
        }

        [Fact]
        public void MultipleTypeCustomInstanceAnnotationsOnErrorShouldRoundtrip()
        {
            var originalInt = new KeyValuePair<string, ODataValue>("int.error", new ODataPrimitiveValue(1));
            var originalDouble = new KeyValuePair<string, ODataValue>("double.error", new ODataPrimitiveValue(double.NaN));
            DateTimeOffset dateTimeOffset = new DateTimeOffset(2012, 10, 10, 12, 12, 59, new TimeSpan());
            var originalDateTimeOffset = new KeyValuePair<string, ODataValue>("DateTimeOffset.error", new ODataPrimitiveValue(dateTimeOffset));
            Date date = new Date(2014, 12, 12);
            var originalDate = new KeyValuePair<string, ODataValue>("Date.error", new ODataPrimitiveValue(date));
            TimeOfDay time = new TimeOfDay(10, 12, 3, 9);
            var originaltime = new KeyValuePair<string, ODataValue>("TimeOfDay.error", new ODataPrimitiveValue(time));
            TimeSpan timeSpan = new TimeSpan(12345);
            var originalTimeSpan = new KeyValuePair<string, ODataValue>("TimeSpan.error", new ODataPrimitiveValue(timeSpan));
            GeographyPoint geographyPoint = GeographyPoint.Create(32.0, -100.0);
            var originalGeography = new KeyValuePair<string, ODataValue>("Geography.error", new ODataPrimitiveValue(geographyPoint));
            var originalNull = new KeyValuePair<string, ODataValue>("null.error", new ODataNullValue());

            var complexValue = new ODataComplexValue
            {
                TypeName = "ns.ErrorDetails",
                Properties = new[] { new ODataProperty { Name = "ErrorDetailName", Value = "inner property value" } }
            };
            var originalComplex = new KeyValuePair<string, ODataValue>("sample.error", complexValue);

            var error = this.WriteThenReadErrorWithInstanceAnnotation(originalInt, originalDouble, originalDate, originalDateTimeOffset, originaltime, originalTimeSpan, originalGeography, originalNull, originalComplex);

            var annotation = RunBasicVerificationAndGetAnnotationValue("int.error", error);
            annotation.Should().BeOfType<ODataPrimitiveValue>();
            annotation.As<ODataPrimitiveValue>().Value.Should().Be(1);

            annotation = RunBasicVerificationAndGetAnnotationValue("double.error", error);
            annotation.Should().BeOfType<ODataPrimitiveValue>();
            annotation.As<ODataPrimitiveValue>().Value.Should().Be(double.NaN);

            annotation = RunBasicVerificationAndGetAnnotationValue("Date.error", error);
            annotation.Should().BeOfType<ODataPrimitiveValue>();
            annotation.As<ODataPrimitiveValue>().Value.Should().Be(date);

            annotation = RunBasicVerificationAndGetAnnotationValue("DateTimeOffset.error", error);
            annotation.Should().BeOfType<ODataPrimitiveValue>();
            annotation.As<ODataPrimitiveValue>().Value.Should().Be(dateTimeOffset);

            annotation = RunBasicVerificationAndGetAnnotationValue("TimeOfDay.error", error);
            annotation.Should().BeOfType<ODataPrimitiveValue>();
            annotation.As<ODataPrimitiveValue>().Value.Should().Be(time);

            annotation = RunBasicVerificationAndGetAnnotationValue("TimeSpan.error", error);
            annotation.Should().BeOfType<ODataPrimitiveValue>();
            annotation.As<ODataPrimitiveValue>().Value.Should().Be(timeSpan);

            annotation = RunBasicVerificationAndGetAnnotationValue("Geography.error", error);
            annotation.Should().BeOfType<ODataPrimitiveValue>();
            annotation.As<ODataPrimitiveValue>().Value.Should().Be(geographyPoint);

            annotation = RunBasicVerificationAndGetAnnotationValue("null.error", error);
            annotation.Should().BeOfType<ODataNullValue>();

            annotation = RunBasicVerificationAndGetAnnotationValue("sample.error", error);
            annotation.Should().BeOfType<ODataComplexValue>();
            annotation.As<ODataComplexValue>().Properties.First().Value.Should().Be("inner property value");
        }

        [Fact]
        public void ComplexCustomInstanceAnnotationOnErrorShouldRoundtrip()
        {
            var originalComplexValue = new ODataComplexValue
            {
                TypeName = "ns.ErrorDetails",
                Properties = new[] { new ODataProperty { Name = "ErrorDetailName", Value = "inner property value" } }
            };
            var original = new KeyValuePair<string, ODataValue>("sample.error", originalComplexValue);
            var error = this.WriteThenReadErrorWithInstanceAnnotation(original);
            var annotation = RunBasicVerificationAndGetAnnotationValue("sample.error", error);
            annotation.Should().BeOfType<ODataComplexValue>();
            annotation.As<ODataComplexValue>().Properties.First().Value.Should().Be("inner property value");
        }

        [Fact]
        public void CollectionOfPrimitiveCustomInstanceAnnotationOnErrorShouldRoundtrip()
        {
            var originalCollectionValue = new ODataCollectionValue
            {
                TypeName = "Collection(Edm.String)",
                Items = new[] { "value1", "value2" }
            };
            var original = new KeyValuePair<string, ODataValue>("sample.error", originalCollectionValue);
            var error = this.WriteThenReadErrorWithInstanceAnnotation(original);
            var annotation = RunBasicVerificationAndGetAnnotationValue("sample.error", error);
            annotation.Should().BeOfType<ODataCollectionValue>();
            annotation.As<ODataCollectionValue>().Items.Cast<string>().Count().Should().Be(2);
        }

        #region Test Helpers

        private static ODataValue RunBasicVerificationAndGetAnnotationValue(string name, ODataError error)
        {
            error.Should().NotBeNull();
            var instanceAnnotations = error.InstanceAnnotations;
            instanceAnnotations.Should().NotBeNull("there was an instance annotation in the payload.");
            instanceAnnotations.Should().NotBeEmpty("there was an instance annotation in the payload.");
            var annotation = instanceAnnotations.Where(instanceAnnotation => instanceAnnotation.Name.Equals(name)).FirstOrDefault();
            annotation.Should().NotBeNull("an instance annotation with the requested name was in the payload.");
            return annotation.Value;
        }

        /// <summary>
        /// Writes an error payload with the given value as an instance annotation on the error, and then
        /// reads it back. Handles all the necessary plumbing.
        /// </summary>
        private ODataError WriteThenReadErrorWithInstanceAnnotation(params KeyValuePair<string, ODataValue>[] annotations)
        {
            this.WriteError(annotations);
            this.stream.Position = 0;
            this.stream.Flush();
            return this.ReadError();
        }

        /// <summary>
        /// Writes an ODataError with the given custom instance annotation to the test stream.
        /// </summary>
        private void WriteError(params KeyValuePair<string, ODataValue>[] annotations)
        {
            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true };
            writerSettings.SetContentType(ODataFormat.Json);
            writerSettings.SetServiceDocumentUri(new Uri("http://example.com/"));

            IODataResponseMessage messageToWrite = new InMemoryMessage { StatusCode = 400, Stream = this.stream };

            var error = new ODataError();
            var instanceAnnotations = new Collection<ODataInstanceAnnotation>();
            error.SetInstanceAnnotations(instanceAnnotations);

            foreach (var pair in annotations)
            {
                ODataInstanceAnnotation annotation = new ODataInstanceAnnotation(pair.Key, pair.Value);
                instanceAnnotations.Add(annotation);
            }

            using (var writer = new ODataMessageWriter(messageToWrite, writerSettings, this.model))
            {
                writer.WriteError(error, false);
            }
        }

        /// <summary>
        /// Reads an ODataError from the stream and returns it for verification.
        /// </summary>
        private ODataError ReadError()
        {
            ODataError readError;
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };

            IODataResponseMessage messageToRead = new InMemoryMessage { StatusCode = 400, Stream = this.stream };
            messageToRead.SetHeader("Content-Type", "application/json;odata.metadata=minimal");

            using (var reader = new ODataMessageReader(messageToRead, readerSettings, this.model))
            {
                readError = reader.ReadError();
            }

            return readError;
        }

        #endregion
    }
}
