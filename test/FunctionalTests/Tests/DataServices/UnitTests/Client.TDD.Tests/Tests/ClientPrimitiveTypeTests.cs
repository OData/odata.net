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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Strings = Microsoft.OData.Client.Strings;

    /// <summary>
    /// Tests PrimitiveTypeConverter functionalities
    /// </summary>
    [TestClass]
    public class ClientPrimitiveTypeConverterTests
    {
        [TestMethod]
        public void TokenizeFromXmlTestHandlesReaderPositionAndReturnsCorrectTokens()
        {
            XElement xel = XElement.Parse("<element>true</element>");
            var r = xel.CreateReader();
            r.Read();

            PrimitiveTypeConverter converter = new BooleanTypeConverter();
            var token = converter.TokenizeFromXml(r);
            Assert.AreEqual(typeof(TextPrimitiveParserToken), token.GetType());
            Assert.AreEqual(XmlNodeType.EndElement, r.NodeType);
            Assert.AreEqual("element", r.LocalName);
            Assert.AreEqual(true, token.Materialize(typeof(Boolean)));
        }

        [TestMethod]
        public void TokenizeFromXmlTestWithNull()
        {
            XElement xel = XElement.Parse("<element xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" m:null=\"true\" />");
            var r = xel.CreateReader();
            r.Read();

            PrimitiveTypeConverter converter = new StringTypeConverter();
            var token = converter.TokenizeFromXml(r);
            Assert.IsNull(token, "Expected null token when m:null=true");
        }

        [TestMethod]
        public void TokenizeFromXmlHandlesEmptyStringAndIsNullShouldBeFalse()
        {
            XElement xel = XElement.Parse("<element xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" m:null=\"false\" />");
            var r = xel.CreateReader();
            r.Read();

            PrimitiveTypeConverter converter = new StringTypeConverter();
            var token = converter.TokenizeFromXml(r);
            Assert.AreEqual(String.Empty, token.Materialize(typeof(String)));
        }

        [TestMethod]
        public void BooleanTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new BooleanTypeConverter();
            Assert.AreEqual(true, converter.Parse("true"));
            Assert.AreEqual(false, converter.Parse("false"));
            Assert.AreEqual("true", converter.ToString(true));
            Assert.AreEqual("false", converter.ToString(false));
        }

        [TestMethod]
        public void ByteTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new ByteTypeConverter();
            Assert.AreEqual((byte)1, converter.Parse("1"));
            Assert.AreEqual("1", converter.ToString((Byte)1));
        }

        [TestMethod]
        public void ByteArrayTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new ByteArrayTypeConverter();
            Byte[] array = (Byte[])converter.Parse("AAECAw==");
            Assert.AreEqual(4, array.Length);
            for (int i = 0; i < array.Length; ++i)
            {
                Assert.AreEqual(i, array[i]);
            }
            Assert.AreEqual("AAECAw==", converter.ToString(new byte[] { 0, 1, 2, 3 }));
        }

#if !ASTORIA_LIGHT && !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void BinaryTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new BinaryTypeConverter();
            Type temp = BinaryTypeConverter.BinaryType;
            BinaryTypeConverter.BinaryType = typeof(System.Data.Linq.Binary);
            try
            {
                Assert.AreEqual(new System.Data.Linq.Binary(new byte[] { 0, 1, 2, 3 }), (System.Data.Linq.Binary)converter.Parse("AAECAw=="));
                Assert.AreEqual("\"AAECAw==\"", converter.ToString(new System.Data.Linq.Binary(new byte[] { 0, 1, 2, 3 })));
            }
            finally
            {
                BinaryTypeConverter.BinaryType = temp;
            }
        }
#endif

        [TestMethod]
        public void DecimalTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DecimalTypeConverter();
            Assert.AreEqual(12.01m, converter.Parse("12.01"));
            Assert.AreEqual("12.01", converter.ToString(12.01m));
        }

        [TestMethod]
        public void DoubleTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DoubleTypeConverter();
            Assert.AreEqual(12.01, converter.Parse("12.01"));
            Assert.AreEqual("12.01", converter.ToString(12.01));
        }

        [TestMethod]
        public void GuidTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new GuidTypeConverter();
            Assert.AreEqual(new Guid("00000000-1000-0000-1000-100000000000"), converter.Parse("00000000-1000-0000-1000-100000000000"));
            Assert.AreEqual("00000000-1000-0000-1000-100000000000", converter.ToString(new Guid("00000000-1000-0000-1000-100000000000")));
        }

        [TestMethod]
        public void Int16TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Int16TypeConverter();
            Assert.AreEqual((Int16)12, converter.Parse("12"));
            Assert.AreEqual("12", converter.ToString((Int16)12));
        }

        [TestMethod]
        public void Int32TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Int32TypeConverter();
            Assert.AreEqual(12, converter.Parse("12"));
            Assert.AreEqual("12", converter.ToString(12));
        }

        [TestMethod]
        public void Int64TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Int64TypeConverter();
            Assert.AreEqual(12L, converter.Parse("12"));
            Assert.AreEqual("12", converter.ToString(12L));
        }

        [TestMethod]
        public void SingleTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new SingleTypeConverter();
            Assert.AreEqual(12.5f, converter.Parse("12.5"));
            Assert.AreEqual("12.5", converter.ToString(12.5f));
        }

        [TestMethod]
        public void StringTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new StringTypeConverter();
            Assert.AreEqual("ABC", converter.Parse("ABC"));
            Assert.AreEqual("ABC", converter.ToString("ABC"));
        }

        [TestMethod]
        public void SByteTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new SByteTypeConverter();
            Assert.AreEqual((SByte)5, converter.Parse("5"));
            Assert.AreEqual("5", converter.ToString((SByte)5));
        }

        [TestMethod]
        public void CharTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new CharTypeConverter();
            Assert.AreEqual('A', converter.Parse("A"));
            Assert.AreEqual("A", converter.ToString('A'));
        }

        [TestMethod]
        public void CharArrayTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new CharArrayTypeConverter();
            char[] array = (char[])converter.Parse("ABCD");
            Assert.AreEqual("ABCD", new String(array));
            Assert.AreEqual("ABCD", converter.ToString("ABCD".ToCharArray()));
        }

        [TestMethod]
        public void ClrTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new ClrTypeConverter();
            String typeString = typeof(Double).AssemblyQualifiedName;
            Assert.AreEqual(typeof(System.Double), converter.Parse(typeString));
            Assert.AreEqual(typeString, converter.ToString(typeof(System.Double)));
        }

        [TestMethod]
        public void UriTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new Microsoft.OData.Client.UriTypeConverter();
            Assert.AreEqual(new Uri("http://localhost/"), converter.Parse("http://localhost/"));
            Assert.AreEqual("http://localhost/", converter.ToString(new Uri("http://localhost/")));
        }

        [TestMethod]
        public void XDocumentTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new XDocumentTypeConverter();
            System.Xml.Linq.XDocument xdoc = (System.Xml.Linq.XDocument)converter.Parse("<?xml version=\"1.0\" encoding=\"iso-8859-1\" standalone=\"yes\"?><feed></feed>");
            Assert.AreEqual("<feed></feed>", xdoc.ToString());
            Assert.AreEqual("<feed></feed>", converter.ToString(xdoc));
        }

        [TestMethod]
        public void XElementTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new XElementTypeConverter();
            System.Xml.Linq.XElement xel = (System.Xml.Linq.XElement)converter.Parse("<feed></feed>");
            Assert.AreEqual("<feed></feed>", xel.ToString());
            Assert.AreEqual("<feed></feed>", converter.ToString(xel));
        }

        [TestMethod]
        public void DateTimeOffsetTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DateTimeOffsetTypeConverter();
            var offset = new DateTimeOffset(2010, 01, 01, 12, 30, 45, TimeSpan.FromMinutes(15));
            Assert.AreEqual(offset, converter.Parse("2010-01-01T12:30:45+00:15"));
            Assert.AreEqual("2010-01-01T12:30:45+00:15", converter.ToString(offset));
        }

        [TestMethod]
        public void TimeSpanTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new TimeSpanTypeConverter();
            var span = TimeSpan.FromMinutes(15);
            Assert.AreEqual(span, converter.Parse("PT15M"));
            Assert.AreEqual("PT15M", converter.ToString(span));
        }

        [TestMethod]
        public void DateTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new DateTypeConverter();
            var date = new Date(2014, 9, 17);
            Assert.AreEqual(date, converter.Parse("2014-09-17"));
            Assert.AreEqual("2014-09-17", converter.ToString(date));
        }

        [TestMethod]
        public void TimeOfDayTypeConverterTests()
        {
            PrimitiveTypeConverter converter = new TimeOfDayConvert();
            var time = new TimeOfDay(12, 5, 10, 9);
            Assert.AreEqual(time, converter.Parse("12:05:10.0090000"));
            Assert.AreEqual("12:05:10.0090000", converter.ToString(time));
        }

        [TestMethod]
        public void UInt16TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new UInt16TypeConverter();
            Assert.AreEqual((UInt16)12, converter.Parse("12"));
            Assert.AreEqual("12", converter.ToString((UInt16)12));
        }

        [TestMethod]
        public void UInt32TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new UInt32TypeConverter();
            Assert.AreEqual(12U, converter.Parse("12"));
            Assert.AreEqual("12", converter.ToString(12U));
        }

        [TestMethod]
        public void UInt64TypeConverterTests()
        {
            PrimitiveTypeConverter converter = new UInt64TypeConverter();
            Assert.AreEqual(12UL, converter.Parse("12"));
            Assert.AreEqual("12", converter.ToString(12UL));
        }

        [TestMethod]
        public void GeographyTypeConverter_TokenizeFromXml_GeoRssParserShouldParse()
        {
            XElement xel = XElement.Parse("<?xml version=\"1.0\" encoding=\"utf-16\"?><Element><gml:Point xmlns:gml=\"http://www.opengis.net/gml\" srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\"><gml:pos>45.256 -71.92</gml:pos></gml:Point></Element>");
            var r = xel.CreateReader();
            r.Read();

            GeographyTypeConverter converter = new GeographyTypeConverter();
            var token = converter.TokenizeFromXml(r) as InstancePrimitiveParserToken<Geography>;
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Instance is GeographyPoint);

            Assert.AreEqual(XmlNodeType.EndElement, r.NodeType);
            Assert.AreEqual("Element", r.LocalName);
        }
    }

    /// <summary>
    /// Test behavior of PrimitiveType class
    /// </summary>
    [TestClass]
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

        [TestMethod]
        public void TypeRegistrationTest()
        {
            PrimitiveType.RegisterKnownType(typeof(ClientPrimitiveTypeTests), "MyEdmKey", EdmPrimitiveTypeKind.Int32, new Int32TypeConverter(), true);

            PrimitiveType ptype;
            try
            {
                Assert.IsTrue(PrimitiveType.TryGetPrimitiveType(typeof(ClientPrimitiveTypeTests), out ptype));
                Assert.IsTrue(PrimitiveType.TryGetPrimitiveType("MyEdmKey", out ptype));

                Assert.AreEqual(typeof(ClientPrimitiveTypeTests), ptype.ClrType);
                Assert.AreEqual("MyEdmKey", ptype.EdmTypeName);
                Assert.AreEqual(true, ptype.HasReverseMapping);
                Assert.IsTrue(ptype.TypeConverter != null && ptype.TypeConverter is Int32TypeConverter);
            }
            finally
            {
                PrimitiveType.DeleteKnownType(typeof(ClientPrimitiveTypeTests), "MyEdmKey");
                Assert.IsFalse(PrimitiveType.TryGetPrimitiveType(typeof(ClientPrimitiveTypeTests), out ptype));
                Assert.IsFalse(PrimitiveType.TryGetPrimitiveType("MyEdmKey", out ptype));
            }
        }

        [TestMethod]
        public void ClrMappingTableTests()
        {
            foreach (var t in primitiveTypes)
            {
                Assert.IsTrue(PrimitiveType.IsKnownType(t.Item1));
                Assert.IsTrue(PrimitiveType.IsKnownNullableType(t.Item1));
                PrimitiveType ptype;
                Assert.IsTrue(PrimitiveType.TryGetPrimitiveType(t.Item1, out ptype));
                Assert.AreEqual(t.Item2, ptype.EdmTypeName);
                Assert.AreEqual(t.Item3.PrimitiveKind, ptype.CreateEdmPrimitiveType().PrimitiveKind);
                Assert.AreEqual(t.Item4, ptype.TypeConverter.GetType());
                Assert.AreEqual(t.Item5, ptype.HasReverseMapping);
            }
        }

        [TestMethod]
        public void EdmMappingTableTests()
        {
            foreach (var t in primitiveTypes)
            {
                PrimitiveType ptype;
                if (!String.IsNullOrEmpty(t.Item2))
                {
                    // type always mapped to some Edm type
                    Assert.IsTrue(PrimitiveType.TryGetPrimitiveType(t.Item2, out ptype));

                    if (t.Item5)
                    {
                        // has reverse mapping
                        Assert.AreEqual(t.Item1, ptype.ClrType);
                    }
                    else
                    {
                        Assert.AreNotEqual(t.Item4, ptype.ClrType);
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
    [TestClass]
    public class ClientConvertTests
    {
        TestTypeConverter converter;

        [TestInitialize]
        public void TestInit()
        {
            converter = new TestTypeConverter();
            PrimitiveType.RegisterKnownType(typeof(TestPrimitiveType), "Edm.TestPrimitive", EdmPrimitiveTypeKind.Int32, converter, true);
        }

        [TestCleanup]
        public void TestClean()
        {
            PrimitiveType.DeleteKnownType(typeof(TestPrimitiveType), "Edm.TestPrimitive");
        }

        [TestMethod]
        public void ChangeTypeTest()
        {
            TestPrimitiveType value = (TestPrimitiveType)ClientConvert.ChangeType("Property_Value", typeof(TestPrimitiveType));
            Assert.AreEqual("Property_Value", value.Data);
            Assert.AreEqual(1, converter.ParseCall);
            Assert.AreEqual(0, converter.ToStringCall);
        }

        [TestMethod]
        public void ToNamedTypeTests()
        {
            Type t;
            Assert.IsTrue(ClientConvert.ToNamedType(String.Empty, out t));
            Assert.AreEqual(typeof(String), t);

            Assert.IsTrue(ClientConvert.ToNamedType(null, out t));
            Assert.AreEqual(typeof(String), t);

            Assert.IsTrue(ClientConvert.ToNamedType("Edm.TestPrimitive", out t));
            Assert.AreEqual(typeof(TestPrimitiveType), t);
        }

        [TestMethod]
        public void ToStringTests()
        {
            var str = ClientConvert.ToString(new TestPrimitiveType() { Data = "Property_Value" });
            Assert.AreEqual("Property_Value", str);
            Assert.AreEqual(0, converter.ParseCall);
            Assert.AreEqual(1, converter.ToStringCall);
        }

        [TestMethod]
        public void ConvertSpatialPointToLiteralString()
        {
            var value = GeographyFactory.Point(CoordinateSystem.DefaultGeography, 47.5, -127.234).Build();
            TestSpatialLiteral(value, Microsoft.OData.Client.XmlConstants.LiteralPrefixGeography);
        }

        [TestMethod]
        public void ConvertGeometryPointToString()
        {
            var value = GeometryFactory.Point(CoordinateSystem.DefaultGeometry, 47.5, -127.234).Build();
            TestSpatialLiteral(value, Microsoft.OData.Client.XmlConstants.LiteralPrefixGeometry);
        }

        [TestMethod]
        public void GetEdmTypeTests()
        {
            Assert.AreEqual("Edm.TestPrimitive", ClientConvert.GetEdmType(typeof(TestPrimitiveType)));
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
            Assert.AreEqual(prefix + "'" + Uri.EscapeDataString(wkt) + "'", result);
        }
    }
#endif
}
