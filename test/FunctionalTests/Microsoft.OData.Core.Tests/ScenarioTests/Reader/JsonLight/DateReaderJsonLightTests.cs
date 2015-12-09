//---------------------------------------------------------------------
// <copyright file="DateReaderJsonLightTests.cs" company="Microsoft">
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
    public class DateReaderJsonLightTests
    {
        [Fact]
        public void ValidDateReaderTest()
        {
            var testCases = new[]
            {
                new { Payload = "\"2012-04-13\"", Value = new Date(2012, 4, 13)},
                new { Payload = "\"0001-01-01\"", Value = new Date(1, 1, 1)},
                new { Payload = "\"9999-12-31\"", Value = new Date(9999, 12, 31)},
            };

            foreach (var testCase in testCases)
            {
                this.VerifyDateValueReader(testCase.Payload, "Edm.Date", testCase.Value);
            }
        }

        [Fact]
        public void InvalidDateReaderTest()
        {
            var testCases = new[]
            {
                new { Payload = "\"\"", Show = "" },
                new { Payload = "\"value\"", Show = "value" },
                new { Payload = "42", Show = "42" },
                new { Payload = "true", Show = "True" },
                new { Payload = "\"\\/Date(-0001-01-01)\\/\"", Show = "/Date(-0001-01-01)/" },
                new { Payload = "\"\\/Date(-9999-12-31)\\/\"", Show = "/Date(-9999-12-31)/" },
                new { Payload = "\"\\/Date(2012-04-13T02:43:10.215Z)\\/\"",  Show = "/Date(2012-04-13T02:43:10.215Z)/" },
                new { Payload = "\"2/26/2011\"", Show = "2/26/2011" },
                new { Payload = "\"\\/Date(1298678400000)\\/\"", Show = "/Date(1298678400000)/" },
                new { Payload = "\"\\/Date(1286705410000+0060)\\/\"", Show = "/Date(1286705410000+0060)/" },
                new { Payload = "\"7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf\"", Show = "7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf" },
            };

            foreach (var testCase in testCases)
            {
                Action action = () => this.VerifyDateValueReader(testCase.Payload, "Edm.Date", null);
                action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderValidationUtils_CannotConvertPrimitiveValue(testCase.Show, "Edm.Date"));
            }
        }

        private void VerifyDateValueReader(string payload, string edmTypeName, object expectedResult)
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