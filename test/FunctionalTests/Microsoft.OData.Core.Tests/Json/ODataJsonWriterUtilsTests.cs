//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit and short-span integration tests for the ODataJsonWriterUtils.
    /// TODO: Create an interface and make a JsonWriter simulator so we don't have to double test its writing functionality.
    //  TODO: Write unit tests the remaining methods on ODataJsonWriterUtils.
    /// </summary>
    public class ODataJsonWriterUtilsTests
    {
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;
        private ODataMessageWriterSettings settings;

        public ODataJsonWriterUtilsTests()
        {
            this.stringWriter = new StringWriter();
            this.jsonWriter = new JsonWriter(this.stringWriter, isIeee754Compatible: true);
            this.settings = new ODataMessageWriterSettings();
        }

        [Fact]
        public void StartJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [Fact]
        public void StartJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [Fact]
        public void EndJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [Fact]
        public void EndJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [Fact]
        public void StartAndEndJsonPaddingSuccessTest()
        {
            settings.JsonPCallback = "functionName";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().Be("functionName()");
        }

        [Fact]
        public void WriteError_WritesTargetAndDetails()
        {
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } }
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: false,
                maxInnerErrorDepth: 0,
                writingJsonLight: false,
                skipNullProperties:false);
            var result = stringWriter.GetStringBuilder().ToString();
            result.Should().Be(@"{""error"":{""code"":"""",""message"":"""",""target"":""any target""," +
                @"""details"":[{""code"":""500"",""target"":""any target"",""message"":""any msg""}]}}");
        }


        [Fact]
        public void WriteError_InnerErrorWithDetails()
        {
            IDictionary<string, ODataValue> properties = new Dictionary<string, ODataValue>();
            properties.Add("stacktrace", "NormalString".ToODataValue());
            properties.Add("MyNewObject", new ODataResourceValue(){TypeName = "ComplexValue", Properties = new List<ODataProperty>(){ new ODataProperty(){ Name = "NestedResourcePropertyName", Value = "NestedPropertyValue"}}});
            var error = new ODataError
            {
                Target = "any target",
                Details =
                    new[] { new ODataErrorDetail { ErrorCode = "500", Target = "any target", Message = "any msg" } },
                InnerError = new ODataInnerError(properties, "innererror", new ODataInnerError(properties, "nested", null))
            };

            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                error,
                includeDebugInformation: true,
                maxInnerErrorDepth: 5,
                writingJsonLight: false,
                skipNullProperties: false);
            var result = stringWriter.GetStringBuilder().ToString();
            result.Should().Be("{\"error\":{\"code\":\"\",\"message\":\"\",\"target\":\"any target\",\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}],\"innererror\":{\"stacktrace\":\"NormalString\",\"MyNewObject\":{\"NestedResourcePropertyName\":\"NestedPropertyValue\"},\"message\":\"\",\"type\":\"\",\"innererror\":{\"stacktrace\":\"NormalString\",\"MyNewObject\":{\"NestedResourcePropertyName\":\"NestedPropertyValue\"},\"message\":\"\",\"type\":\"\"}}}}");
        }
    }

    public class Animal
    {
        public string Name { get; set; }
    }
}
