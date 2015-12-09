//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataMessageReaderTests
    {
        [Fact]
        public void CreateMessageReaderShouldNotSetAnnotationFilterWhenODataAnnotationsIsNotSetOnPreferenceAppliedHeader()
        {
            ODataMessageReader reader = new ODataMessageReader((IODataResponseMessage)new InMemoryMessage(), new ODataMessageReaderSettings());
            reader.Settings.ShouldIncludeAnnotation.Should().BeNull();
        }

        [Fact]
        public void CreateMessageReaderShouldSetAnnotationFilterWhenODataAnnotationIsSetOnPreferenceAppliedHeader()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage();
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings());
            reader.Settings.ShouldIncludeAnnotation.Should().NotBeNull();
        }

        [Fact]
        public void CreateMessageReaderShouldNotSetAnnotationFilterWhenItIsAlreadySet()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage();
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            Func<string, bool> shouldWrite = name => false;
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings { ShouldIncludeAnnotation = shouldWrite });
            reader.Settings.ShouldIncludeAnnotation.Should().BeSameAs(shouldWrite);
        }

        [Fact]
        public void EncodingShouldRemainInvariantInReader()
        {
            Stream stream = new MemoryStream(Encoding.GetEncoding("iso-8859-1").GetBytes("{\"@odata.context\":\"http://stuff/#Edm.Int32\",\"value\":4}"));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/json;odata.metadata=minimal;");
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            reader.ReadProperty();
        }

        [Fact]
        public void ReadValueOfTypeDefinitionShouldWork()
        {
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("123"));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Length", EdmPrimitiveTypeKind.Int32), true)).Should().Be(123);
        }

        [Fact]
        public void ReadValueOfDateShouldWork()
        {
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("2014-01-03"));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "DateValue", EdmPrimitiveTypeKind.Date), true)).Should().Be(new Date(2014, 1, 3));
        }

        [Fact]
        public void ReadValueOfAbbreviativeDateShouldWork()
        {
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("2014-1-3"));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "DateValue", EdmPrimitiveTypeKind.Date), true)).Should().Be(new Date(2014, 1, 3));
        }

        [Fact]
        public void ReadValueOfTimeOfDayShouldWork()
        {
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("12:30:04.998"));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "TimeOfDayValue", EdmPrimitiveTypeKind.TimeOfDay), true)).Should().Be(new TimeOfDay(12, 30, 4, 998));
        }

        [Fact]
        public void ReadValueOfAbbreviativeTimeOfDayShouldWork()
        {
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("12:30:4.998"));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "TimeOfDayValue", EdmPrimitiveTypeKind.TimeOfDay), true)).Should().Be(new TimeOfDay(12, 30, 4, 998));
        }

        [Fact]
        public void ErrorLocationReportedByMessageReaderForBadEdmxShouldBeAbsolute()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Org.OData.Core.V1"" Alias=""Core"">
      <Annotation Term=""Core.Description"">
        <String>Core terms needed to write vocabularies</String>
      </Annotation>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>"));

            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/xml");
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());

            const string expectedErrorMessage =
                "The metadata document could not be read from the message content.\r\n" +
                "UnexpectedXmlElement : The schema element 'Annotation' was not expected in the given context. : (6, 8)\r\n";

            Action test = () => reader.ReadMetadataDocument();
            test.ShouldThrow<ODataException>().WithMessage(expectedErrorMessage, ComparisonMode.Exact);
        }

        [Fact]
        public void ErrorLocationReportedByMessageReaderForBadEdmxOfSingleLineShouldBeAbsolute()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<!--Remark-->" +
                "<edmx:Edmx xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\" Version=\"4.0\">" +
                "<edmx:DataServices>" +
                "<Schema xmlns=\"http://docs.oasis-open.org/odata/ns/edm\" Namespace=\"Org.OData.Core.V1\" Alias=\"Core\">" +
                "<Annotation Term=\"Core.Description\">" +
                "<String>Core terms needed to write vocabularies</String>" +
                "</Annotation>" +
                "</Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>"));

            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/xml");
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());

            const string expectedErrorMessage =
                "The metadata document could not be read from the message content.\r\n" +
                "UnexpectedXmlElement : The schema element 'Annotation' was not expected in the given context. : (1, 250)\r\n";

            Action test = () => reader.ReadMetadataDocument();
            test.ShouldThrow<ODataException>().WithMessage(expectedErrorMessage, ComparisonMode.Exact);
        }

        [Fact]
        public void ReadMetadataDocumentShouldIncludeConverterForDefaultUnsignedIntImplementation()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""MyNS"">
      <TypeDefinition Name=""UInt16"" UnderlyingType=""Edm.Int32"" />
      <TypeDefinition Name=""UInt32"" UnderlyingType=""Edm.Decimal"" />
      <TypeDefinition Name=""UInt64"" UnderlyingType=""Edm.Decimal"" />
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""UInt16"" Type=""MyNS.UInt16"" Nullable=""false"" />
        <Property Name=""UInt32"" Type=""MyNS.UInt32"" Nullable=""false"" />
        <Property Name=""UInt64"" Type=""MyNS.UInt64"" Nullable=""false"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>"));

            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/xml");
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            var model = reader.ReadMetadataDocument();

            var personType = model.FindDeclaredType("MyNS.Person") as IEdmEntityType;
            Assert.NotNull(personType);

            var uint16Type = personType.FindProperty("UInt16").Type;
            var uint32Type = personType.FindProperty("UInt32").Type;
            var uint64Type = personType.FindProperty("UInt64").Type;

            var uint16Converter = model.GetPrimitiveValueConverter(uint16Type.AsTypeDefinition());
            var uint32Converter = model.GetPrimitiveValueConverter(uint32Type.AsTypeDefinition());
            var uint64Converter = model.GetPrimitiveValueConverter(uint64Type.AsTypeDefinition());

            Assert.Equal(Type.GetTypeCode(uint16Converter.ConvertToUnderlyingType((UInt16)123).GetType()), TypeCode.Int32);
            Assert.Equal(Type.GetTypeCode(uint32Converter.ConvertToUnderlyingType((UInt32)123).GetType()), TypeCode.UInt32);
            Assert.Equal(Type.GetTypeCode(uint64Converter.ConvertToUnderlyingType((UInt64)123).GetType()), TypeCode.Decimal);

            Assert.Equal(Type.GetTypeCode(uint16Converter.ConvertFromUnderlyingType(123).GetType()), TypeCode.UInt16);
            Assert.Equal(Type.GetTypeCode(uint32Converter.ConvertFromUnderlyingType((Int64)123).GetType()), TypeCode.Int64);
            Assert.Equal(Type.GetTypeCode(uint64Converter.ConvertFromUnderlyingType((Decimal)123).GetType()), TypeCode.UInt64);
        }

        [Fact]
        public void ReadTopLevelPropertyWithTypeDefinitionShouldWork()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("{value:123}"));
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = stream };
            var model = new EdmModel();

            ODataMessageReader reader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), model);
            Action read = () => reader.ReadProperty(model.GetUInt32("MyNS", false));
            read.ShouldNotThrow();
        }
    }
}
