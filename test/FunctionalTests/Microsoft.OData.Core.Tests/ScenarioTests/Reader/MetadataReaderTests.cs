//---------------------------------------------------------------------
// <copyright file="MetadataReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using Xunit;
using Microsoft.OData.Json;

namespace Microsoft.OData.Tests.ScenarioTests.Reader
{
    public class MetadataReaderTests
    {
        [Fact]
        public void ReadSingleMetadataDocument()
        {
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""demo"">    
        <ComplexType Name=""C1"" BaseType=""demo.B1"">
        </ComplexType>
        <ComplexType Name=""B1"">
        </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            string mainUrl = "main";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {mainUrl, payload}
            };

            IEdmModel model = this.ReadMetadataDocument(map, mainUrl);
            var c1 = model.FindDeclaredType("demo.C1") as IEdmComplexType;
            Assert.NotNull(c1);
            var b1 = model.FindDeclaredType("demo.B1") as IEdmComplexType;
            Assert.NotNull(b1);
            Assert.True(b1.IsAssignableFrom(c1));
        }

        [Fact]
        public void ReadMetadataDocumentWithReference()
        {
            string csdlMain =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:Reference Uri=""Ref1"">
    <edmx:Include Namespace=""demo"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""demo"">    
        <ComplexType Name=""C1"" BaseType=""demo.B1"">
        </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            string csdlR1 =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""demo"">    
        <ComplexType Name=""B1"">
        </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {"main", csdlMain},
                {"Ref1", csdlR1}
            };

            IEdmModel model = this.ReadMetadataDocument(map, "main");
            var c1 = model.FindDeclaredType("demo.C1") as IEdmComplexType;
            Assert.NotNull(c1);
            var b1 = model.FindType("demo.B1") as IEdmComplexType;
            Assert.NotNull(b1);
            Assert.True(b1.IsAssignableFrom(c1));
        }

        [Fact]
        public void ReadErrorMetadataDocumentThrows()
        {
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<m:error xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <m:code>code42</m:code>
  <m:message>message text</m:message>
</m:error>";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {"main", payload}
            };

            Action test = () => this.ReadMetadataDocument(map, "main");

            ODataErrorException exception = Assert.Throws<ODataErrorException>(test);
            Assert.Equal("An error was read from the payload. See the 'Error' property for more details.", exception.Message);
            Assert.Equal("code42", exception.Error.Code);
            Assert.Equal("message text", exception.Error.Message);
        }

        [Fact]
        public void VerifyErrorElementNotFoundForErrorMetadataDocumentThrows()
        {
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<m:error xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <m:code>code42</m:code>
  <m:code>code54</m:code>
  <m:message>message text</m:message>
</m:error>";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {"main", payload}
            };

            Action test = () => this.ReadMetadataDocument(map, "main");
            test.Throws<ODataException>(Strings.ODataXmlErrorDeserializer_MultipleErrorElementsWithSameName("code"));
        }

        [Theory]
        [InlineData(JsonConstants.ODataErrorInnerErrorInnerErrorName)]
        [InlineData(JsonConstants.ODataErrorInnerErrorName)]
        public void ReadInnerErrorMetadataDocumentThrows(string nestedInnerErrorName)
        {
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<m:error xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <m:code>code42</m:code>
  <m:message>message text</m:message>
  <m:innererror>
    <m:message>some inner error</m:message>
    <m:type></m:type>
    <m:stacktrace></m:stacktrace>" +
    $"    <m:{nestedInnerErrorName}></m:{nestedInnerErrorName}>" +
@"  </m:innererror>
</m:error>";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {"main", payload}
            };

            Action test = () => this.ReadMetadataDocument(map, "main");

            ODataErrorException exception = Assert.Throws<ODataErrorException>(test);
            Assert.Equal("An error was read from the payload. See the 'Error' property for more details.", exception.Message);
            Assert.Equal("code42", exception.Error.Code);
            Assert.Equal("message text", exception.Error.Message);
            Assert.True(exception.Error.InnerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorMessageName, out ODataValue innerErrorMessage));
            Assert.Equal("some inner error", Assert.IsType<ODataPrimitiveValue>(innerErrorMessage).Value);
            Assert.NotNull(exception.Error.InnerError.InnerError);
            Assert.Empty(exception.Error.InnerError.InnerError.Properties);
        }

        [Fact]
        public void VerifyInnerErrorElementNotFoundForErrorMetadataDocumentThrows()
        {
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<m:error xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <m:code>code42</m:code>
  <m:message>message text</m:message>
  <m:innererror>
    <m:message>some inner error</m:message>
    <m:type></m:type>
    <m:stacktrace></m:stacktrace>
    <m:stacktrace>my trackstrace</m:stacktrace>
  </m:innererror>
</m:error>";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {"main", payload}
            };

            Action test = () => this.ReadMetadataDocument(map, "main");
            test.Throws<ODataException>(Strings.ODataXmlErrorDeserializer_MultipleInnerErrorElementsWithSameName("stacktrace"));
        }

        private IEdmModel ReadMetadataDocument(Dictionary<string, string> map, string mainUrl)
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(map[mainUrl]));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/xml");
            ODataMessageReader reader = new ODataMessageReader(responseMessage);
            return reader.ReadMetadataDocument((uri) =>
            {
                string uriStr = uri.ToString();
                if (map.ContainsKey(uriStr))
                {
                    return new XmlTextReader(new StringReader(map[uriStr]));
                }

                return null;
            });
        }
    }
}
