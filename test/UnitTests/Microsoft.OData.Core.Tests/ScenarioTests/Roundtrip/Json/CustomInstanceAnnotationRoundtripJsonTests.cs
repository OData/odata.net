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
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.Json
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
            Assert.NotNull(error);
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

            var resourceValue = new ODataResourceValue
            {
                TypeName = "ns.ErrorDetails",
                Properties = new[] { new ODataProperty { Name = "ErrorDetailName", Value = "inner property value" } }
            };
            var originalResource = new KeyValuePair<string, ODataValue>("sample.error", resourceValue);

            var error = this.WriteThenReadErrorWithInstanceAnnotation(originalInt, originalDouble, originalDate, originalDateTimeOffset, originaltime, originalTimeSpan, originalGeography, originalNull, originalResource);

            var annotation = RunBasicVerificationAndGetAnnotationValue("int.error", error);
            var primitiveValue = Assert.IsType<ODataPrimitiveValue>(annotation);
            Assert.Equal(1, primitiveValue.Value);

            annotation = RunBasicVerificationAndGetAnnotationValue("double.error", error);
            primitiveValue = Assert.IsType<ODataPrimitiveValue>(annotation);
            Assert.Equal(double.NaN, primitiveValue.Value);

            annotation = RunBasicVerificationAndGetAnnotationValue("Date.error", error);
            primitiveValue = Assert.IsType<ODataPrimitiveValue>(annotation);
            Assert.Equal(date, primitiveValue.Value);

            annotation = RunBasicVerificationAndGetAnnotationValue("DateTimeOffset.error", error);
            primitiveValue = Assert.IsType<ODataPrimitiveValue>(annotation);
            Assert.Equal(dateTimeOffset, primitiveValue.Value);

            annotation = RunBasicVerificationAndGetAnnotationValue("TimeOfDay.error", error);
            primitiveValue = Assert.IsType<ODataPrimitiveValue>(annotation);
            Assert.Equal(time, primitiveValue.Value);

            annotation = RunBasicVerificationAndGetAnnotationValue("TimeSpan.error", error);
            primitiveValue = Assert.IsType<ODataPrimitiveValue>(annotation);
            Assert.Equal(timeSpan, primitiveValue.Value);

            annotation = RunBasicVerificationAndGetAnnotationValue("Geography.error", error);
            primitiveValue = Assert.IsType<ODataPrimitiveValue>(annotation);
            Assert.Equal(geographyPoint, primitiveValue.Value);

            annotation = RunBasicVerificationAndGetAnnotationValue("null.error", error);
            Assert.IsType<ODataNullValue>(annotation);

            annotation = RunBasicVerificationAndGetAnnotationValue("sample.error", error);
            var value = Assert.IsType<ODataResourceValue>(annotation);
            Assert.Equal("inner property value", value.Properties.First().Value);
        }

        [Fact]
        public void ResourceCustomInstanceAnnotationOnErrorShouldRoundtrip()
        {
            var originalResourceValue = new ODataResourceValue
            {
                TypeName = "ns.ErrorDetails",
                Properties = new[] { new ODataProperty { Name = "ErrorDetailName", Value = "inner property value" } }
            };
            var original = new KeyValuePair<string, ODataValue>("sample.error", originalResourceValue);
            var error = this.WriteThenReadErrorWithInstanceAnnotation(original);
            var annotation = RunBasicVerificationAndGetAnnotationValue("sample.error", error);
            var value = Assert.IsType<ODataResourceValue>(annotation);
            Assert.Equal("inner property value", value.Properties.First().Value);
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
            var collectionValue = Assert.IsType<ODataCollectionValue>(annotation);
            Assert.Equal(2, collectionValue.Items.Count());
        }

        #region Test Helpers

        private static ODataValue RunBasicVerificationAndGetAnnotationValue(string name, ODataError error)
        {
            Assert.NotNull(error);
            var instanceAnnotations = error.InstanceAnnotations;
            Assert.NotNull(instanceAnnotations);
            Assert.NotEmpty(instanceAnnotations);
            var annotation = instanceAnnotations.Where(instanceAnnotation => instanceAnnotation.Name.Equals(name)).FirstOrDefault();
            Assert.NotNull(annotation);
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
            var writerSettings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
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
            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };

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
