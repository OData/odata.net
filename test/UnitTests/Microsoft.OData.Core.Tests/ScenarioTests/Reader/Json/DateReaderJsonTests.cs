//---------------------------------------------------------------------
// <copyright file="DateReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Tests.Json;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Reader.Json
{
    public class DateReaderJsonTests
    {
        [Theory]
        [InlineData("\"2012-04-13\"", 2012, 4, 13)]
        [InlineData("\"0001-01-01\"", 1, 1, 1)]
        [InlineData("\"9999-12-31\"", 9999, 12, 31)]
        public void ValidDateReaderTest(string payload, int year, int month, int day)
        {
            this.VerifyDateValueReader(payload, "Edm.Date", new DateOnly(year, month, day));
        }

        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\"value\"", "value")]
        [InlineData("42", "42")]
        [InlineData("true", "True")]
        [InlineData("\"\\/Date(-0001-01-01)\\/\"", "/Date(-0001-01-01)/")]
        [InlineData("\"\\/Date(-9999-12-31)\\/\"", "/Date(-9999-12-31)/")]
        [InlineData("\"\\/Date(2012-04-13T02:43:10.215Z)\\/\"", "/Date(2012-04-13T02:43:10.215Z)/")]
        [InlineData("\"2/26/2011\"", "2/26/2011")]
        [InlineData("\"\\/Date(1298678400000)\\/\"", "/Date(1298678400000)/")]
        [InlineData("\"\\/Date(1286705410000+0060)\\/\"", "/Date(1286705410000+0060)/")]
        [InlineData("\"7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf\"", "7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf")]
        public void InvalidDateReaderTest(string payload, string show)
        {
            Action action = () => this.VerifyDateValueReader(payload, "Edm.Date", null);
            action.Throws<ODataException>(Error.Format(SRResources.ReaderValidationUtils_CannotConvertPrimitiveValue, show, "Edm.Date"));
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