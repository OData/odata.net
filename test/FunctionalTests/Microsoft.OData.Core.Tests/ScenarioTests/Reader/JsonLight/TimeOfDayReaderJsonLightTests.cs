//---------------------------------------------------------------------
// <copyright file="TimeOfDayReaderJsonLightTests.cs" company="Microsoft">
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
    public class TimeOfDayReaderJsonLightTests
    {
        [Fact]
        public void ValidTimeOfDayReaderTest()
        {
            var testCases = new[]
            {
                new { Payload = "\"01:20:03.900\"", Value = new TimeOfDay(1, 20, 3, 900)},
                new { Payload = "\"00:00:00.000\"", Value = new TimeOfDay(0, 0, 0, 0)},
                new { Payload = "\"23:59:59.999\"", Value = new TimeOfDay(23, 59, 59, 999)},
                new { Payload = "\"1:20:3.900\"", Value = new TimeOfDay(1, 20, 3, 900)},
                new { Payload = "\"01:20:03.009\"", Value = new TimeOfDay(1, 20, 3, 9)},
                new { Payload = "\"01:20:03.09\"", Value = new TimeOfDay(1, 20, 3, 90)},
                new { Payload = "\"23:59:59.9999999\"", Value = new TimeOfDay(TimeOfDay.MaxTickValue)},
                new { Payload = "\"23:59:59\"", Value = new TimeOfDay(23, 59, 59, 0)},
                new { Payload = "\"23:59\"", Value = new TimeOfDay(23, 59, 0, 0)},
            };

            foreach (var testCase in testCases)
            {
                this.VerifyTimeOfDayValueReader(testCase.Payload, "Edm.TimeOfDay", testCase.Value);
            }
        }

        [Fact]
        public void InvalidTimeOfDayReaderTest()
        {
            var testCases = new[]
            {
                new { Payload = "\"\"", Show = "" },
                new { Payload = "\"value\"", Show = "value" },
                new { Payload = "12", Show = "12" },
                new { Payload = "true", Show = "True" },
                new { Payload = "\"\\/TimeOfDay(12:30:03.000)\\/\"", Show = "/TimeOfDay(12:30:03.000)/" },
                new { Payload = "\".920\"", Show = ".920" },
                new { Payload = "\"-12:30:0.920\"", Show = "-12:30:0.920" },
                new { Payload = "\"1.12:30:0.920\"", Show = "1.12:30:0.920" },
            };

            foreach (var testCase in testCases)
            {
                Action action = () => this.VerifyTimeOfDayValueReader(testCase.Payload, "Edm.TimeOfDay", null);
                action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderValidationUtils_CannotConvertPrimitiveValue(testCase.Show, "Edm.TimeOfDay"));
            }
        }

        private void VerifyTimeOfDayValueReader(string payload, string edmTypeName, object expectedResult)
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
        }
    }
}