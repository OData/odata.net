//---------------------------------------------------------------------
// <copyright file="ClientPrimitiveTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Spatial;
    using System.Xml;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Strings = Microsoft.OData.Client.Strings;
    using Xunit;

    /// <summary>
    /// Tests PrimitiveTypeConverter functionalities
    /// </summary>
    public class ClientPrimitiveTypeConverterTests
    {
        [Fact]
        public void TokenizeFromXmlTestHandlesReaderPositionAndReturnsCorrectTokens()
        {
            XElement xel = XElement.Parse("<element>true</element>");
            var r = xel.CreateReader();
            r.Read();

            PrimitiveTypeConverter converter = new BooleanTypeConverter();
            var token = converter.TokenizeFromXml(r);
            Assert.Equal(typeof(TextPrimitiveParserToken), token.GetType());
            Assert.Equal(XmlNodeType.EndElement, r.NodeType);
            Assert.Equal("element", r.LocalName);
            Assert.Equal(true, token.Materialize(typeof(Boolean)));
        }

        [Fact]
        public void TokenizeFromXmlTestWithNull()
        {
            XElement xel = XElement.Parse("<element xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" m:null=\"true\" />");
            var r = xel.CreateReader();
            r.Read();

            PrimitiveTypeConverter converter = new StringTypeConverter();
            var token = converter.TokenizeFromXml(r);
            Assert.Null(token);
        }

        [Fact]
        public void TokenizeFromXmlHandlesEmptyStringAndIsNullShouldBeFalse()
        {
            XElement xel = XElement.Parse("<element xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" m:null=\"false\" />");
            var r = xel.CreateReader();
            r.Read();

            PrimitiveTypeConverter converter = new StringTypeConverter();
            var token = converter.TokenizeFromXml(r);
            Assert.Equal(String.Empty, token.Materialize(typeof(String)));
        }

        [Fact]
        public void BooleanTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new BooleanTypeConverter();
            Assert.Equal(true, converter.Parse("true"));
            Assert.Equal(false, converter.Parse("false"));
            Assert.Equal("true", converter.ToString(true));
            Assert.Equal("false", converter.ToString(false));
        }

        [Fact]
        public void ByteTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new ByteTypeConverter();
            Assert.Equal((byte)1, converter.Parse("1"));
            Assert.Equal("1", converter.ToString((Byte)1));
        }

        [Fact]
        public void ByteArrayTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new ByteArrayTypeConverter();
            Byte[] array = (Byte[])converter.Parse("AAECAw==");
            Assert.Equal(4, array.Length);
            for (int i = 0; i < array.Length; ++i)
            {
                Assert.Equal(i, array[i]);
            }
            Assert.Equal("AAECAw==", converter.ToString(new byte[] { 0, 1, 2, 3 }));
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [Fact]
        public void BinaryTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new BinaryTypeConverter();
            Type temp = BinaryTypeConverter.BinaryType;
            BinaryTypeConverter.BinaryType = typeof(System.Data.Linq.Binary);
            try
            {
                Assert.Equal(new System.Data.Linq.Binary(new byte[] { 0, 1, 2, 3 }), (System.Data.Linq.Binary)converter.Parse("AAECAw=="));
                Assert.Equal("\"AAECAw==\"", converter.ToString(new System.Data.Linq.Binary(new byte[] { 0, 1, 2, 3 })));
            }
            finally
            {
                BinaryTypeConverter.BinaryType = temp;
            }
        }
#endif

        [Fact]
        public void DecimalTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DecimalTypeConverter();
            Assert.Equal(12.01m, converter.Parse("12.01"));
            Assert.Equal("12.01", converter.ToString(12.01m));
        }

        [Fact]
        public void DoubleTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DoubleTypeConverter();
            Assert.Equal(12.01, converter.Parse("12.01"));
            Assert.Equal("12.01", converter.ToString(12.01));
        }

        [Fact]
        public void GuidTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new GuidTypeConverter();
            Assert.Equal(new Guid("00000000-1000-0000-1000-100000000000"), converter.Parse("00000000-1000-0000-1000-100000000000"));
            Assert.Equal("00000000-1000-0000-1000-100000000000", converter.ToString(new Guid("00000000-1000-0000-1000-100000000000")));
        }

        [Fact]
        public void Int16TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Int16TypeConverter();
            Assert.Equal((Int16)12, converter.Parse("12"));
            Assert.Equal("12", converter.ToString((Int16)12));
        }

        [Fact]
        public void Int32TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Int32TypeConverter();
            Assert.Equal(12, converter.Parse("12"));
            Assert.Equal("12", converter.ToString(12));
        }

        [Fact]
        public void Int64TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Int64TypeConverter();
            Assert.Equal(12L, converter.Parse("12"));
            Assert.Equal("12", converter.ToString(12L));
        }

        [Fact]
        public void SingleTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new SingleTypeConverter();
            Assert.Equal(12.5f, converter.Parse("12.5"));
            Assert.Equal("12.5", converter.ToString(12.5f));
        }

        [Fact]
        public void StringTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new StringTypeConverter();
            Assert.Equal("ABC", converter.Parse("ABC"));
            Assert.Equal("ABC", converter.ToString("ABC"));
        }

        [Fact]
        public void SByteTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new SByteTypeConverter();
            Assert.Equal((SByte)5, converter.Parse("5"));
            Assert.Equal("5", converter.ToString((SByte)5));
        }

        [Fact]
        public void CharTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new CharTypeConverter();
            Assert.Equal('A', converter.Parse("A"));
            Assert.Equal("A", converter.ToString('A'));
        }

        [Fact]
        public void CharArrayTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new CharArrayTypeConverter();
            char[] array = (char[])converter.Parse("ABCD");
            Assert.Equal("ABCD", new String(array));
            Assert.Equal("ABCD", converter.ToString("ABCD".ToCharArray()));
        }

        [Fact]
        public void ClrTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new ClrTypeConverter();
            String typeString = typeof(Double).AssemblyQualifiedName;
            Assert.Equal(typeof(System.Double), converter.Parse(typeString));
            Assert.Equal(typeString, converter.ToString(typeof(System.Double)));
        }

        [Fact]
        public void UriTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Microsoft.OData.Client.UriTypeConverter();
            Assert.Equal(new Uri("http://localhost/"), converter.Parse("http://localhost/"));
            Assert.Equal("http://localhost/", converter.ToString(new Uri("http://localhost/")));
        }

        [Fact]
        public void XDocumentTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new XDocumentTypeConverter();
            System.Xml.Linq.XDocument xdoc = (System.Xml.Linq.XDocument)converter.Parse("<?xml version=\"1.0\" encoding=\"iso-8859-1\" standalone=\"yes\"?><feed></feed>");
            Assert.Equal("<feed></feed>", xdoc.ToString());
            Assert.Equal("<feed></feed>", converter.ToString(xdoc));
        }

        [Fact]
        public void XElementTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new XElementTypeConverter();
            System.Xml.Linq.XElement xel = (System.Xml.Linq.XElement)converter.Parse("<feed></feed>");
            Assert.Equal("<feed></feed>", xel.ToString());
            Assert.Equal("<feed></feed>", converter.ToString(xel));
        }

        [Fact]
        public void DateTimeOffsetTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DateTimeOffsetTypeConverter();
            var offset = new DateTimeOffset(2010, 01, 01, 12, 30, 45, TimeSpan.FromMinutes(15));
            Assert.Equal(offset, converter.Parse("2010-01-01T12:30:45+00:15"));
            Assert.Equal("2010-01-01T12:30:45+00:15", converter.ToString(offset));
        }

        [Fact]
        public void TimeSpanTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new TimeSpanTypeConverter();
            var span = TimeSpan.FromMinutes(15);
            Assert.Equal(span, converter.Parse("PT15M"));
            Assert.Equal("PT15M", converter.ToString(span));
        }

        [Fact]
        public void DateTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DateTypeConverter();
            var date = new Date(2014, 9, 17);
            Assert.Equal(date, converter.Parse("2014-09-17"));
            Assert.Equal("2014-09-17", converter.ToString(date));
        }

        [Fact]
        public void TimeOfDayTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new TimeOfDayConvert();
            var time = new TimeOfDay(12, 5, 10, 9);
            Assert.Equal(time, converter.Parse("12:05:10.0090000"));
            Assert.Equal("12:05:10.0090000", converter.ToString(time));
        }

        [Fact]
        public void UInt16TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new UInt16TypeConverter();
            Assert.Equal((UInt16)12, converter.Parse("12"));
            Assert.Equal("12", converter.ToString((UInt16)12));
        }

        [Fact]
        public void UInt32TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new UInt32TypeConverter();
            Assert.Equal(12U, converter.Parse("12"));
            Assert.Equal("12", converter.ToString(12U));
        }

        [Fact]
        public void UInt64TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new UInt64TypeConverter();
            Assert.Equal(12UL, converter.Parse("12"));
            Assert.Equal("12", converter.ToString(12UL));
        }

        [Fact]
        public void GeographyTypeConverter_TokenizeFromXml_GeoRssParserShouldParse()
        {
            XElement xel = XElement.Parse("<?xml version=\"1.0\" encoding=\"utf-16\"?><Element><gml:Point xmlns:gml=\"http://www.opengis.net/gml\" srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\"><gml:pos>45.256 -71.92</gml:pos></gml:Point></Element>");
            var r = xel.CreateReader();
            r.Read();

            GeographyTypeConverter converter = new GeographyTypeConverter();
            var token = converter.TokenizeFromXml(r) as InstancePrimitiveParserToken<Geography>;
            Assert.NotNull(token);
            Assert.True(token.Instance is GeographyPoint);

            Assert.Equal(XmlNodeType.EndElement, r.NodeType);
            Assert.Equal("Element", r.LocalName);
        }
    }

    /// <summary>
    /// Test behavior of PrimitiveType class
    /// </summary>
    public class ClientPrimitiveTypeTests
    {
        // Clr Type, Edm Type Name, EdmPrimitiveType Converter Type, Has Reverse Mapping
        private Tuple<Type, String, IEdmPrimitiveType, Type, Boolean>[] primitiveTypes = new Tuple<Type, string, IEdmPrimitiveType, Type, bool>[]
            {
                Tuple.Create(typeof(Boolean), "Edm.Boolean", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), typeof(BooleanTypeConverter), true),
                Tuple.Create(typeof(Byte), "Edm.Byte", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), typeof(ByteTypeConverter), true),
                Tuple.Create(typeof(Byte[]), "Edm.Binary", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), typeof(ByteArrayTypeConverter), true),
                Tuple.Create(typeof(Decimal), "Edm.Decimal", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), typeof(DecimalTypeConverter), true),
                Tuple.Create(typeof(Double), "Edm.Double", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), typeof(DoubleTypeConverter), true),
                Tuple.Create(typeof(Guid), "Edm.Guid", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), typeof(GuidTypeConverter), true),
                Tuple.Create(typeof(Byte[]), "Edm.Binary", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), typeof(ByteArrayTypeConverter), true),
                Tuple.Create(typeof(Int16), "Edm.Int16", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), typeof(Int16TypeConverter), true),
                Tuple.Create(typeof(Int32), "Edm.Int32", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), typeof(Int32TypeConverter), true),
                Tuple.Create(typeof(Int64), "Edm.Int64", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), typeof(Int64TypeConverter), true),
                Tuple.Create(typeof(Single), "Edm.Single", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), typeof(SingleTypeConverter), true),
                Tuple.Create(typeof(String), "Edm.String", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(StringTypeConverter), true),
                Tuple.Create(typeof(SByte), "Edm.SByte", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), typeof(SByteTypeConverter), true),
                Tuple.Create(typeof(Geography), "Edm.Geography", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(GeographyPoint), "Edm.GeographyPoint", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(GeographyLineString), "Edm.GeographyLineString", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(GeographyPolygon), "Edm.GeographyPolygon", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(GeographyCollection), "Edm.GeographyCollection", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(GeographyMultiPoint), "Edm.GeographyMultiPoint", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(GeographyMultiLineString), "Edm.GeographyMultiLineString", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(GeographyMultiPolygon), "Edm.GeographyMultiPolygon", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon), typeof(GeographyTypeConverter), true),
                Tuple.Create(typeof(Geometry), "Edm.Geometry", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(GeometryPoint), "Edm.GeometryPoint", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(GeometryLineString), "Edm.GeometryLineString", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(GeometryPolygon), "Edm.GeometryPolygon", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(GeometryCollection), "Edm.GeometryCollection", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(GeometryMultiPoint), "Edm.GeometryMultiPoint", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(GeometryMultiLineString), "Edm.GeometryMultiLineString", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(GeometryMultiPolygon), "Edm.GeometryMultiPolygon", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon), typeof(GeometryTypeConverter), true),
                Tuple.Create(typeof(DateTimeOffset), "Edm.DateTimeOffset", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), typeof(DateTimeOffsetTypeConverter), true),
                Tuple.Create(typeof(TimeSpan), "Edm.Duration", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), typeof(TimeSpanTypeConverter), true),

                Tuple.Create(typeof(Char), "Edm.String", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(CharTypeConverter), false),
                Tuple.Create(typeof(Char[]), "Edm.String", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(CharArrayTypeConverter), false),
                Tuple.Create(typeof(Type), "Edm.String", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(ClrTypeConverter), false),
                Tuple.Create(typeof(Uri), "Edm.String", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(Microsoft.OData.Client.UriTypeConverter), false),
                Tuple.Create(typeof(XDocument), "Edm.String", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(XDocumentTypeConverter), false),
                Tuple.Create(typeof(XElement), "Edm.String", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(XElementTypeConverter), false),
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
                Tuple.Create(typeof(System.Data.Linq.Binary), "Edm.Binary", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), typeof(BinaryTypeConverter), false),
#endif
                Tuple.Create(typeof(UInt16), (String)null, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(UInt16TypeConverter), false),
                Tuple.Create(typeof(UInt32), (String)null, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(UInt32TypeConverter), false),
                Tuple.Create(typeof(UInt64), (String)null, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), typeof(UInt64TypeConverter), false),
            };

        [Fact]
        public void TypeRegistrationTest()
        {
            PrimitiveType.RegisterKnownType(typeof(ClientPrimitiveTypeTests), "MyEdmKey", EdmPrimitiveTypeKind.Int32, new Int32TypeConverter(), true);

            PrimitiveType ptype;
            try
            {
                Assert.True(PrimitiveType.TryGetPrimitiveType(typeof(ClientPrimitiveTypeTests), out ptype));
                Assert.True(PrimitiveType.TryGetPrimitiveType("MyEdmKey", out ptype));

                Assert.Equal(typeof(ClientPrimitiveTypeTests), ptype.ClrType);
                Assert.Equal("MyEdmKey", ptype.EdmTypeName);
                Assert.True(ptype.HasReverseMapping);
                Assert.True(ptype.TypeConverter != null && ptype.TypeConverter is Int32TypeConverter);
            }
            finally
            {
                PrimitiveType.DeleteKnownType(typeof(ClientPrimitiveTypeTests), "MyEdmKey");
                Assert.False(PrimitiveType.TryGetPrimitiveType(typeof(ClientPrimitiveTypeTests), out ptype));
                Assert.False(PrimitiveType.TryGetPrimitiveType("MyEdmKey", out ptype));
            }
        }

        [Fact]
        public void ClrMappingTableTests()
        {
            foreach (var t in primitiveTypes)
            {
                Assert.True(PrimitiveType.IsKnownType(t.Item1));
                Assert.True(PrimitiveType.IsKnownNullableType(t.Item1));
                PrimitiveType ptype;
                Assert.True(PrimitiveType.TryGetPrimitiveType(t.Item1, out ptype));
                Assert.Equal(t.Item2, ptype.EdmTypeName);
                Assert.Equal(t.Item3.PrimitiveKind, ptype.CreateEdmPrimitiveType().PrimitiveKind);
                Assert.Equal(t.Item4, ptype.TypeConverter.GetType());
                Assert.Equal(t.Item5, ptype.HasReverseMapping);
            }
        }

        [Fact]
        public void EdmMappingTableTests()
        {
            foreach (var t in primitiveTypes)
            {
                PrimitiveType ptype;
                if (!String.IsNullOrEmpty(t.Item2))
                {
                    // type always mapped to some Edm type
                    Assert.True(PrimitiveType.TryGetPrimitiveType(t.Item2, out ptype));

                    if (t.Item5)
                    {
                        // has reverse mapping
                        Assert.Equal(t.Item1, ptype.ClrType);
                    }
                    else
                    {
                        Assert.NotEqual(t.Item4, ptype.ClrType);
                    }
                }
            }
        }
    }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
    /// <summary>
    /// Test behavior of ClientConvert class
    /// Several APIs within the test framework are not supported in Net Core 1.0 so test only on other platforms.
    /// </summary>
    public class ClientConvertTests : IDisposable
    {
        TestTypeConverter converter;

        public ClientConvertTests()
        {
            converter = new TestTypeConverter();
            PrimitiveType.RegisterKnownType(typeof(TestPrimitiveType), "Edm.TestPrimitive", EdmPrimitiveTypeKind.Int32, converter, true);
        }

        public void Dispose()
        {
            PrimitiveType.DeleteKnownType(typeof(TestPrimitiveType), "Edm.TestPrimitive");
        }

        [Fact]
        public void ChangeTypeTest()
        {
            TestPrimitiveType value = (TestPrimitiveType)ClientConvert.ChangeType("Property_Value", typeof(TestPrimitiveType));
            Assert.Equal("Property_Value", value.Data);
            Assert.Equal(1, converter.ParseCall);
            Assert.Equal(0, converter.ToStringCall);
        }

        [Fact]
        public void ToNamedTypeTests()
        {
            Type t;
            Assert.True(ClientConvert.ToNamedType(String.Empty, out t));
            Assert.Equal(typeof(String), t);

            Assert.True(ClientConvert.ToNamedType(null, out t));
            Assert.Equal(typeof(String), t);

            Assert.True(ClientConvert.ToNamedType("Edm.TestPrimitive", out t));
            Assert.Equal(typeof(TestPrimitiveType), t);
        }

        [Fact]
        public void ToStringTests()
        {
            var str = ClientConvert.ToString(new TestPrimitiveType() { Data = "Property_Value" });
            Assert.Equal("Property_Value", str);
            Assert.Equal(0, converter.ParseCall);
            Assert.Equal(1, converter.ToStringCall);
        }

        [Fact]
        public void ConvertSpatialPointToLiteralString()
        {
            var value = GeographyFactory.Point(CoordinateSystem.DefaultGeography, 47.5, -127.234).Build();
            TestSpatialLiteral(value, Microsoft.OData.Client.XmlConstants.LiteralPrefixGeography);
        }

        [Fact]
        public void ConvertGeometryPointToString()
        {
            var value = GeometryFactory.Point(CoordinateSystem.DefaultGeometry, 47.5, -127.234).Build();
            TestSpatialLiteral(value, Microsoft.OData.Client.XmlConstants.LiteralPrefixGeometry);
        }

        [Fact]
        public void GetEdmTypeTests()
        {
            Assert.Equal("Edm.TestPrimitive", ClientConvert.GetEdmType(typeof(TestPrimitiveType)));
            foreach (Type t in new Type[] { typeof(UInt16), typeof(UInt32), typeof(UInt64) })
            {
                Action test = () => ClientConvert.GetEdmType(t);
                test.ShouldThrow<NotSupportedException>().WithMessage(Strings.ALinq_CantCastToUnsupportedPrimitive(t.Name));
            }
        }

        private static void TestSpatialLiteral<T>(T value, string prefix) where T : class, ISpatial
        {
            var result = LiteralFormatter.ForConstants.Format(value);
            var wkt = WellKnownTextSqlFormatter.Create().Write(value);
            Assert.Equal(prefix + "'" + Uri.EscapeDataString(wkt) + "'", result);
        }
    }
#endif
}
