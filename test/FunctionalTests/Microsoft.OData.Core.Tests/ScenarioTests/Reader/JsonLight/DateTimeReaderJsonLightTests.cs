//---------------------------------------------------------------------
// <copyright file="DateTimeReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Core.Tests.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Reader.JsonLight
{
    public class DateTimeReaderJsonLightTests
    {
        [Fact]
        public void ValidDateTimeOffsetReaderTest()
        {
            var testCases = new[]
            {
                new { Payload = "\"2012-04-13T02:43:10.215Z\"", Value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.Zero) },
                new { Payload = "\"2012-04-13T02:43:10.215+14:00\"", Value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(840)) },
                new { Payload = "\"2012-04-13T02:43:10.215-14:00\"", Value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-840)) },
                new { Payload = "\"2012-04-13T02:43:10.215+02:03\"", Value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(123)) },
                new { Payload = "\"2012-04-13T02:43:10.215-00:42\"", Value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-42)) },
                new { Payload = "\"0001-01-01T00:00:00Z\"", Value = DateTimeOffset.MinValue },
                new { Payload = "\"9999-12-31T23:59:59.9999999Z\"", Value = DateTimeOffset.MaxValue },
            };

            foreach (var testCase in testCases)
            {
                this.VerifyDateTimeValueReader(testCase.Payload, "Edm.DateTimeOffset", testCase.Value);
            }
        }

        [Fact]
        public void InvalidDateTimeOffsetReaderTest()
        {
            var testCases = new[]
            {
                new { Payload = "\"\"", Show = "" },
                new { Payload = "\"value\"", Show = "value" },
                new { Payload = "42", Show = "42" },
                new { Payload = "true", Show = "True" },
                new { Payload = "\"2/26/2011\"", Show = "2/26/2011" },
                new { Payload = "\"\\/Date(1298678400000)\\/\"", Show = "/Date(1298678400000)/" },
                new { Payload = "\"\\/Date(1286705410000+0060)\\/\"", Show = "/Date(1286705410000+0060)/" },
                new { Payload = "\"7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf\"", Show = "7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf" },
            };

            foreach (var testCase in testCases)
            {
                Action action = () => this.VerifyDateTimeValueReader(testCase.Payload, "Edm.DateTimeOffset", null);
                action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderValidationUtils_CannotConvertPrimitiveValue(testCase.Show, "Edm.DateTimeOffset"));
            }
        }

        private void VerifyDateTimeValueReader(string payload, string edmTypeName, object expectedResult)
        {
            IEdmModel model = new EdmModel();
            IEdmPrimitiveTypeReference typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)model.FindType(edmTypeName), true);

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            object actualValue;
            using (ODataJsonLightInputContext inputContext = new ODataJsonLightInputContext(
                ODataFormat.Json,
                stream,
                JsonLightUtils.JsonLightStreamingMediaType,
                Encoding.UTF8,
                new ODataMessageReaderSettings(),
                /*readingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(inputContext);
                deserializer.JsonReader.Read();
                actualValue = deserializer.ReadNonEntityValue(
                    /*payloadTypeName*/ null,
                    typeReference,
                    /*duplicatePropertyNamesChecker*/ null,
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true,
                    /*isTopLevelPropertyValue*/ true,
                    /*insideComplexValue*/ false,
                    /*propertyName*/ null);
            }

            actualValue.Should().Be(expectedResult, "payload ->{0}<- for type '{1}'", payload, edmTypeName);
            if (actualValue is DateTime)
            {
                ((DateTime)actualValue).Kind.Should().Be(((DateTime)expectedResult).Kind, "payload ->{0}<- for type '{1}'", payload, edmTypeName);
            }
        }
    }
}