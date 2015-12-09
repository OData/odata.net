//---------------------------------------------------------------------
// <copyright file="PrimitiveValuesRoundtripAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.Atom
{
    public class PrimitiveValuesRoundtripAtomTests
    {
        [Fact]
        public void BinaryRoundtripAtomTest()
        {
            var values = new byte[][]
            {
                new byte[0],
                new byte[] { 0 },
                new byte[] { 42, Byte.MinValue, Byte.MaxValue },
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Binary");
        }

        [Fact]
        public void BooleanRoundtripAtomTest()
        {
            var values = new bool[]
            {
                true, 
                false,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Boolean");
        }

        [Fact]
        public void ByteRoundtripAtomTest()
        {
            var values = new byte[]
            {
                0,
                42,
                Byte.MaxValue,
                Byte.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Byte");
        }

        [Fact]
        public void DateTimeOffsetRoundtripAtomTest()
        {
            var values = new DateTimeOffset[]
            {
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, TimeSpan.Zero),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(840)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-840)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(123)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-42)),
                DateTimeOffset.MinValue,
                DateTimeOffset.MaxValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.DateTimeOffset");
        }

        [Fact]
        public void DecimalRoundtripAtomTest()
        {
            var values = new decimal[]
            {
                0,
                1,
                -1,
                Decimal.MinValue,
                Decimal.MaxValue,
                10^-28,
                10^28,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Decimal");
        }

        [Fact]
        public void DoubleRoundtripAtomTest()
        {
            var values = new double[]
            {
                0,
                42,
                42.42,
                Double.MaxValue,
                Double.MinValue,
                Double.PositiveInfinity,
                Double.NegativeInfinity,
                Double.NaN,
                -4.42330604244772E-305,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Double");
        }

        [Fact]
        public void GuidRoundtripAtomTest()
        {
            var values = new Guid[]
            {
                new Guid(0, 0, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }),
                new Guid("C8864E5E-BDB1-4FB2-A1C4-8F8E49C271EA"),
                Guid.Empty,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Guid");
        }

        [Fact]
        public void Int16RoundtripAtomTest()
        {
            var values = new Int16[]
            {
                0,
                42,
                -43,
                Int16.MaxValue,
                Int16.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Int16");
        }

        [Fact]
        public void Int32RoundtripAtomTest()
        {
            var values = new Int32[]
            {
                0,
                42,
                -43,
                Int32.MaxValue,
                Int32.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Int32");
        }

        [Fact]
        public void Int64RoundtripAtomTest()
        {
            var values = new Int64[]
            {
                0,
                42,
                -43,
                Int64.MaxValue,
                Int64.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Int64");
        }

        [Fact]
        public void SByteRoundtripAtomTest()
        {
            var values = new SByte[]
            {
                0,
                42,
                -43,
                SByte.MaxValue,
                SByte.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.SByte");
        }

        [Fact]
        public void StringRoundtripAtomTest()
        {
            var values = new string[]
            {
                string.Empty,
                " ",
                "testvalue",
                "TestValue",
                "\r\n\t",
                "\"",
                "\'",
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.String");
        }

        [Fact]
        public void SingleRoundtripAtomTest()
        {
            var values = new Single[]
            {
                0,
                42,
                (float)-43.43,
                Single.MaxValue,
                Single.MinValue,
                Single.PositiveInfinity,
                Single.NegativeInfinity,
                Single.NaN,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Single");
        }

        [Fact]
        public void DurationRoundtripAtomTest()
        {
            var values = new TimeSpan[]
            {
                new TimeSpan(1, 2, 3, 4, 5),
                TimeSpan.Zero,
                TimeSpan.MinValue,
                TimeSpan.MaxValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Duration");
        }

        [Fact]
        public void GeographyMultiPointRoundtripAtomTest()
        {
            var values = new GeographyMultiPoint[]
            {
                GeographyFactory.MultiPoint().Point(0, 0).Build(),
                GeographyFactory.MultiPoint().Point(1.5, 1.0).Point(2.5, 2.0).Build(), 
                GeographyFactory.MultiPoint().Point(-90.0, 0).Point(0, 90.0).Point(-90.0, 90.0).Build()
            };
            this.VerifyPrimitiveValuesRoundtrip(values, "GeographyMultiPoint");
        }

        [Fact]
        public void GeometryMultiPolygonRoundtripAtomTest()
        {
            var values = new GeometryMultiPolygon[]
            {
                GeometryFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build()
            };
            this.VerifyPrimitiveValuesRoundtrip(values, "GeometryMultiPolygon");
        }


        private void VerifyPrimitiveValuesRoundtrip(IEnumerable clrValues, string edmTypeName)
        {
            foreach (ODataVersion version in Enum.GetValues(typeof(ODataVersion)))
            {
                foreach (object clrValue in clrValues)
                {
                    this.VerifyPrimitiveValueRoundtrips(clrValue, edmTypeName, version, string.Format("ATOM roundtrip value {0} of type {1} in version {2}.", clrValue, edmTypeName, version));
                }
            }
        }

        private void VerifyPrimitiveValueRoundtrips(object clrValue, string edmTypeName, ODataVersion version, string description)
        {
            IEdmModel model = new EdmModel();
            IEdmPrimitiveTypeReference typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)model.FindType(edmTypeName), true);

            MemoryStream stream = new MemoryStream();
            using (ODataAtomOutputContext outputContext = new ODataAtomOutputContext(
                ODataFormat.Atom,
                new NonDisposingStream(stream),
                Encoding.UTF8,
                new ODataMessageWriterSettings() { Version = version },
                /*writingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                ODataAtomPropertyAndValueSerializer serializer = new ODataAtomPropertyAndValueSerializer(outputContext);
                serializer.XmlWriter.WriteStartElement("ValueElement");
                serializer.WritePrimitiveValue(
                    clrValue,
                    /*collectionValidator*/ null,
                    typeReference,
                    /*serializationTypeNameAnnotation*/ null);
                serializer.XmlWriter.WriteEndElement();
            }

            stream.Position = 0;

            object actualValue;
            using (ODataAtomInputContext inputContext = new ODataAtomInputContext(
                ODataFormat.Atom,
                stream,
                Encoding.UTF8,
                new ODataMessageReaderSettings(),
                /*readingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                ODataAtomPropertyAndValueDeserializer deserializer = new ODataAtomPropertyAndValueDeserializer(inputContext);
                deserializer.XmlReader.MoveToContent();
                actualValue = deserializer.ReadNonEntityValue(
                    typeReference,
                    /*duplicatePropertyNamesChecker*/ null,
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true);
            }

            if (clrValue is byte[])
            {
                ((byte[])actualValue).Should().Equal((byte[])clrValue, description);
            }
            else
            {
                actualValue.Should().Be(clrValue, description);
            }
        }
    }
}