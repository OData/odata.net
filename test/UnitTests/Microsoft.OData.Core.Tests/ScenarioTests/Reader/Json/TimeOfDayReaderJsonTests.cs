//---------------------------------------------------------------------
// <copyright file="TimeOfDayReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Tests.Json;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Reader.Json
{
    public class TimeOfDayReaderJsonTests
    {
        [Theory]
        [InlineData("\"01:20:03.900\"", 1, 20, 3, 900)]
        [InlineData("\"00:00:00.000\"", 0, 0, 0, 0)]
        [InlineData("\"23:59:59.999\"", 23, 59, 59, 999)]
        [InlineData("\"1:20:3.900\"", 1, 20, 3, 900)]
        [InlineData("\"01:20:03.009\"", 1, 20, 3, 9)]
        [InlineData("\"01:20:03.09\"", 1, 20, 3, 90)]
        [InlineData("\"23:59:59.9999999\"", 23, 59, 59, 9999999)] // TimeOnly.MaxValue.Ticks maps to 23:59:59.9999999
        [InlineData("\"23:59:59\"", 23, 59, 59, 0)]
        [InlineData("\"23:59\"", 23, 59, 0, 0)]
        public void ValidTimeOfDayReaderTest(string payload, int hour, int minute, int second, int millisecond)
        {
            var timeOnly = new TimeOnly(hour, minute, second, millisecond);
            this.VerifyTimeOfDayValueReader(payload, "Edm.TimeOfDay", timeOnly);
        }

        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\"value\"", "value")]
        [InlineData("12", "12")]
        [InlineData("true", "True")]
        [InlineData("\"\\/TimeOfDay(12:30:03.000)\\/\"", "/TimeOfDay(12:30:03.000)/")]
        [InlineData("\".920\"", ".920")]
        [InlineData("\"-12:30:0.920\"", "-12:30:0.920")]
        [InlineData("\"1.12:30:0.920\"", "1.12:30:0.920")]
        public void InvalidTimeOfDayReaderTest(string payload, string show)
        {
            Action action = () => this.VerifyTimeOfDayValueReader(payload, "Edm.TimeOfDay", null);
            action.Throws<ODataException>(Error.Format(SRResources.ReaderValidationUtils_CannotConvertPrimitiveValue, show, "Edm.TimeOfDay"));
        }

        private void VerifyTimeOfDayValueReader(string payload, string edmTypeName, object expectedResult)
        {
            IEdmModel model = new EdmModel();
            IEdmPrimitiveTypeReference typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)model.FindType(edmTypeName), true);

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = JsonUtils.JsonStreamingMediaType,
                IsAsync = false,
                Model = model,
            };

            object actualValue;
            using (var inputContext = new ODataJsonInputContext(
                new StringReader(payload), messageInfo, new ODataMessageReaderSettings()))
            {
                ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(inputContext);
                deserializer.JsonReader.Read();
                actualValue = deserializer.ReadNonEntityValue(
                    /*payloadTypeName*/ null,
                    typeReference,
                    /*propertyAndAnnotationCollector*/ null,
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true,
                    /*isTopLevelPropertyValue*/ true,
                    /*insideResourceValue*/ false,
                    /*propertyName*/ null);
            }

            Assert.Equal(expectedResult, actualValue);
        }
    }
}