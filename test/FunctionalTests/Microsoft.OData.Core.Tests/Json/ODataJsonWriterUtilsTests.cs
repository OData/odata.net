//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using FluentAssertions;
using Microsoft.OData.Core.Json;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
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
            this.jsonWriter = new JsonWriter(this.stringWriter, /*indent*/ false, ODataFormat.Json, isIeee754Compatible: true);
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
                writingJsonLight: false);
            var result = stringWriter.GetStringBuilder().ToString();
            result.Should().Be(@"{""error"":{""code"":"""",""message"":"""",""target"":""any target"","+
                @"""details"":[{""code"":""500"",""target"":""any target"",""message"":""any msg""}]}}");
        }
    }
}
