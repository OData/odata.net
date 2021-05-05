//---------------------------------------------------------------------
// <copyright file="CsdlJsonParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1
using System;
using System.Text;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Parsing
{
    public class CsdlJsonParserTests
    {
        [Fact]
        public void InvalidJsonReportingJsonInvalidError()
        {
            string json = @" { ""$Version"": ""4.01"" "; // missing a }

            Utf8JsonReader jsonReader = GetJsonReader(json);
            JsonParserContext context = new JsonParserContext();
            CsdlJsonParser.ParseCsdlDocument(ref jsonReader, context);
            EdmError error = Assert.Single(context.Errors);
            Assert.Equal(EdmErrorCode.InvalidJson, error.ErrorCode);
            Assert.Equal("$ LineNumber:0 BytePositionInLine:22 Path:N/A ActualMessage:Expected depth to be zero at the end of the JSON payload." +
                " There is an open JSON object or array that should be closed. LineNumber: 0 | BytePositionInLine: 22.", error.ErrorMessage);
        }

        [Fact]
        public void InvalidCsdlDocumentJsonValueKindReportingJsonInvalidValueKindError()
        {
            string json = @" [ {""$Version"": ""4.01""} ]";

            Utf8JsonReader jsonReader = GetJsonReader(json);
            JsonParserContext context = new JsonParserContext();
            CsdlJsonParser.ParseCsdlDocument(ref jsonReader, context);
            EdmError error = Assert.Single(context.Errors);
            Assert.Equal(EdmErrorCode.UnexpectedValueKind, error.ErrorCode);
            Assert.Equal("An unexpected 'Array' value kind was found when parsing the JSON path '$'. A 'Object' value kind was expected.", error.ErrorMessage);
        }

        [Fact]
        public void UnexpectedJsonMemberReportingJsonUnexpectedJsonMemberError()
        {
            string json = @" {""$Version"": ""4.01"", ""Anything"": 1.0 }";

            Utf8JsonReader jsonReader = GetJsonReader(json);
            JsonParserContext context = new JsonParserContext();
            CsdlJsonParser.ParseCsdlDocument(ref jsonReader, context);
            EdmError error = Assert.Single(context.Errors);
            Assert.Equal(EdmErrorCode.UnexpectedElement, error.ErrorCode);
            Assert.Equal("A member '$.Anything' with value type 'Number' is unexpected.", error.ErrorMessage);
        }

        #region Version
        [Theory]
        [InlineData("4.0")]
        [InlineData("4.01")]
        public void ParseValidVersionStringWorksAsExpected(string version)
        {
            string json = @"{""$Version"":""" + version + @"""}";

            Utf8JsonReader jsonReader = GetJsonReader(json);
            JsonParserContext context = new JsonParserContext();
            CsdlModel csdlModel = CsdlJsonParser.ParseCsdlDocument(ref jsonReader, context);
            Assert.Equal(version == "4.0" ? EdmConstants.EdmVersion4 : EdmConstants.EdmVersion401, csdlModel.CsdlVersion);
        }

        [Fact]
        public void MissingVersionReportingMissingRequiredPropertyError()
        {
            string json = @" { }";

            Utf8JsonReader jsonReader = GetJsonReader(json);
            JsonParserContext context = new JsonParserContext();
            CsdlJsonParser.ParseCsdlDocument(ref jsonReader, context);
            EdmError error = Assert.Single(context.Errors);
            Assert.Equal(EdmErrorCode.MissingRequiredProperty, error.ErrorCode);
            Assert.Equal("A property '$Version' is missing when parsing the JSON path '$'.", error.ErrorMessage);
        }

        [Fact]
        public void InvalidVersionValueKindReportingUnexpectedValueKind()
        {
            string json = @" { ""$Version"": 4.01 }";

            Utf8JsonReader jsonReader = GetJsonReader(json);
            JsonParserContext context = new JsonParserContext();
            CsdlJsonParser.ParseCsdlDocument(ref jsonReader, context);
            EdmError error = Assert.Single(context.Errors);
            Assert.Equal(EdmErrorCode.UnexpectedValueKind, error.ErrorCode);
            Assert.Equal("An unexpected 'Number' value kind was found when parsing the JSON path '$.$Version'. A 'String' value kind was expected.", error.ErrorMessage);
        }

        [Fact]
        public void InvalidVersionValueReportingUnexpectedValueKind()
        {
            string json = @" { ""$Version"": ""5.0"" }";

            Utf8JsonReader jsonReader = GetJsonReader(json);
            JsonParserContext context = new JsonParserContext();
            CsdlJsonParser.ParseCsdlDocument(ref jsonReader, context);
            EdmError error = Assert.Single(context.Errors);
            Assert.Equal(EdmErrorCode.InvalidVersionNumber, error.ErrorCode);
            Assert.Equal("The version specified at '$.$Version' is not valid. It should be a string containing either '4.0' or '4.01'.", error.ErrorMessage);
        }
        #endregion

        [Fact]
        public void ParseIncludeWorksAsExpected()
        {
            string json = @" {
""$Namespace"": ""org.example.display"",
 ""$Alias"": ""UI""  }";

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement rootElement = document.RootElement;
                JsonParserContext context = new JsonParserContext();

                CsdlInclude include = CsdlJsonParser.ParseInclude(rootElement, context);

                Assert.NotNull(include);
                Assert.Equal("UI", include.Alias);
                Assert.Equal("org.example.display", include.Namespace);
            }
        }

        [Fact]
        public void ParseIncludeReportUnExpectedMember()
        {
            string json = @" { 
""$Namespace"": ""org.example.display"",
""$Unknown"": ""UI""  }";

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement rootElement = document.RootElement;
                JsonParserContext context = new JsonParserContext();

                CsdlInclude include = CsdlJsonParser.ParseInclude(rootElement, context);

                Assert.NotNull(include);
                Assert.Equal("org.example.display", include.Namespace);

                var error = Assert.Single(context.Errors);
                Assert.Equal(EdmErrorCode.UnexpectedElement, error.ErrorCode);
                Assert.Equal("$.$Unknown", error.ErrorLocation.ToString());
                Assert.Equal("A member '$.$Unknown' with value type 'String' is unexpected.", error.ErrorMessage);
            }
        }

        [Fact]
        public void ParseIncludeAnnotationsWorksAsExpected()
        {
            string json = @" {
  ""$TermNamespace"": ""org.example.hcm"",
  ""$Qualifier"": ""Tablet"",
  ""$TargetNamespace"":   ""com.example.Person""  }";

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement rootElement = document.RootElement;

                JsonParserContext context = new JsonParserContext();
                CsdlIncludeAnnotations includeAnnotations = CsdlJsonParser.ParseIncludeAnnotations(rootElement, context);

                Assert.NotNull(includeAnnotations);
                Assert.Equal("org.example.hcm", includeAnnotations.TermNamespace);
                Assert.Equal("Tablet", includeAnnotations.Qualifier);
                Assert.Equal("com.example.Person", includeAnnotations.TargetNamespace);
            }
        }

        private Utf8JsonReader GetJsonReader(string json)
        {
            ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(json);
            return new Utf8JsonReader(jsonReadOnlySpan);
        }
    }
}
#endif