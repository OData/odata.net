//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    using TypeCode = System.TypeCode;

    public class ODataMessageReaderTests
    {
        [Fact]
        public void CreateMessageReaderShouldNotSetAnnotationFilterWhenODataAnnotationsIsNotSetOnPreferenceAppliedHeader()
        {
            ODataMessageReader reader = new ODataMessageReader((IODataResponseMessage)new InMemoryMessage(), new ODataMessageReaderSettings());
            Assert.Null(reader.Settings.ShouldIncludeAnnotation);
        }

        [Fact]
        public void CreateMessageReaderShouldSetAnnotationFilterWhenODataAnnotationIsSetOnPreferenceAppliedHeader()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage();
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings());
            Assert.NotNull(reader.Settings.ShouldIncludeAnnotation);
        }

        [Fact]
        public void CreateMessageReaderShouldNotSetAnnotationFilterWhenItIsAlreadySet()
        {
            IODataResponseMessage responseMessage = new InMemoryMessage();
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            Func<string, bool> shouldWrite = name => false;
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings { ShouldIncludeAnnotation = shouldWrite });
            Assert.Same(reader.Settings.ShouldIncludeAnnotation, shouldWrite);
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
#if NETCOREAPP1_1
            Stream stream = new MemoryStream(Encoding.GetEncoding(0).GetBytes("123"));
#else
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("123"));
#endif
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            var result = reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Length", EdmPrimitiveTypeKind.Int32), true));
            Assert.Equal(123, result);
        }

        [Fact]
        public void ReadValueOfDateShouldWork()
        {
#if NETCOREAPP1_1
            Stream stream = new MemoryStream(Encoding.GetEncoding(0).GetBytes("2014-01-03"));
#else
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("2014-01-03"));
#endif
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            var result = reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "DateValue", EdmPrimitiveTypeKind.Date), true));
            Assert.Equal(new Date(2014, 1, 3), result);
        }

        [Fact]
        public void ReadValueOfAbbreviativeDateShouldWork()
        {
#if NETCOREAPP1_1
            Stream stream = new MemoryStream(Encoding.GetEncoding(0).GetBytes("2014-1-3"));
#else
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("2014-1-3"));
#endif
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            var result = reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "DateValue", EdmPrimitiveTypeKind.Date), true));
            Assert.Equal(new Date(2014, 1, 3), result);
        }

        [Fact]
        public void ReadValueOfTimeOfDayShouldWork()
        {
#if NETCOREAPP1_1
            Stream stream = new MemoryStream(Encoding.GetEncoding(0).GetBytes("12:30:04.998"));
#else
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("12:30:04.998"));
#endif
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            var result = reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "TimeOfDayValue", EdmPrimitiveTypeKind.TimeOfDay), true));
            Assert.Equal(new TimeOfDay(12, 30, 4, 998), result);
        }

        [Fact]
        public void ReadValueOfAbbreviativeTimeOfDayShouldWork()
        {
#if NETCOREAPP1_1
            Stream stream = new MemoryStream(Encoding.GetEncoding(0).GetBytes("12:30:4.998"));
#else
            Stream stream = new MemoryStream(Encoding.Default.GetBytes("12:30:4.998"));
#endif
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());
            var result = reader.ReadValue(new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "TimeOfDayValue", EdmPrimitiveTypeKind.TimeOfDay), true));
            Assert.Equal(new TimeOfDay(12, 30, 4, 998), result);
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
      <Invalid Term=""Core.Description"">
        <String>Core terms needed to write vocabularies</String>
      </Invalid>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>"));

            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/xml");
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());

            const string expectedErrorMessage =
                "The metadata document could not be read from the message content.\r\n" +
                "UnexpectedXmlElement : The schema element 'Invalid' was not expected in the given context. : (6, 8)\r\n";

            Action test = () => reader.ReadMetadataDocument();
            test.Throws<ODataException>(expectedErrorMessage);
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
                "<Invalid Term=\"Core.Description\">" +
                "<String>Core terms needed to write vocabularies</String>" +
                "</Invalid>" +
                "</Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>"));

            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/xml");
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());

            const string expectedErrorMessage =
                "The metadata document could not be read from the message content.\r\n" +
                "UnexpectedXmlElement : The schema element 'Invalid' was not expected in the given context. : (1, 250)\r\n";

            Action test = () => reader.ReadMetadataDocument();
            test.Throws<ODataException>(expectedErrorMessage);
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

            Assert.Equal(TypeCode.Int32, Type.GetTypeCode(uint16Converter.ConvertToUnderlyingType((UInt16)123).GetType()));
            Assert.Equal(TypeCode.UInt32, Type.GetTypeCode(uint32Converter.ConvertToUnderlyingType((UInt32)123).GetType()));
            Assert.Equal(TypeCode.Decimal, Type.GetTypeCode(uint64Converter.ConvertToUnderlyingType((UInt64)123).GetType()));

            Assert.Equal(TypeCode.UInt16, Type.GetTypeCode(uint16Converter.ConvertFromUnderlyingType(123).GetType()));
            Assert.Equal(TypeCode.Int64, Type.GetTypeCode(uint32Converter.ConvertFromUnderlyingType((Int64)123).GetType()));
            Assert.Equal(TypeCode.UInt64, Type.GetTypeCode(uint64Converter.ConvertFromUnderlyingType((Decimal)123).GetType()));
        }

        [Fact]
        public void ReadMetadataDocument_WorksForJsonCSDL()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Container"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {}
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }
    }
  }
}"));

            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/json");
            ODataMessageReader reader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), new EdmModel());

#if NETCOREAPP3_1 || NETCOREAPP2_1
            IEdmModel model = reader.ReadMetadataDocument();

            IEdmEntityType customerType = model.FindDeclaredType("NS.Customer") as IEdmEntityType;
            Assert.NotNull(customerType);

            IEdmProperty idProperty = customerType.FindProperty("Id");
            Assert.NotNull(idProperty);
            Assert.Equal("Edm.Int32", idProperty.Type.FullName());

            IEdmProperty nameProperty = customerType.FindProperty("Name");
            Assert.NotNull(nameProperty);
            Assert.Equal("Edm.String", nameProperty.Type.FullName());

            IEdmEntitySet customers = Assert.Single(model.EntityContainer.EntitySets());
            Assert.Equal("Customers", customers.Name);
            Assert.Same(customerType, customers.EntityType());
#else
            Action test = () => reader.ReadMetadataDocument();

            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal("The JSON metadata is not supported at this platform. It's only supported at platform implementing .NETStardard 2.0.", exception.Message);
#endif
        }

        [Fact]
        public void ReadTopLevelPropertyWithTypeDefinitionShouldWork()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("{value:123}"));
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = stream };
            var model = new EdmModel();

            ODataMessageReader reader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), model);
            Action read = () => reader.ReadProperty(model.GetUInt32("MyNS", false));
            read.DoesNotThrow();
        }
    }
}
