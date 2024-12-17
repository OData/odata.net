﻿//---------------------------------------------------------------------
// <copyright file="DateReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Json;
using Microsoft.OData.Tests.Json;
using Microsoft.OData.Edm;
using Xunit;
using Microsoft.OData.Core;

namespace Microsoft.OData.Tests.ScenarioTests.Reader.Json
{
    public class DateReaderJsonTests
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
                action.Throws<ODataException>(Error.Format(SRResources.ReaderValidationUtils_CannotConvertPrimitiveValue, testCase.Show, "Edm.Date"));
            }
        }

        private void VerifyDateValueReader(string payload, string edmTypeName, object expectedResult)
        {
            IEdmModel model = new EdmModel();
            IEdmPrimitiveTypeReference typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)model.FindType(edmTypeName), true);

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = JsonUtils.JsonStreamingMediaType,
                IsAsync = false,
                Model = new EdmModel(),
            };

            object actualValue;
            using (var inputContext = new ODataJsonInputContext(
                new StringReader(payload), messageInfo, new ODataMessageReaderSettings()))
            {
                var deserializer = new ODataJsonPropertyAndValueDeserializer(inputContext);
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