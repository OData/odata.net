//---------------------------------------------------------------------
// <copyright file="PrimitiveValuesRoundtripVerboseJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Roundtripping.VerboseJson
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.VerboseJson;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrimitiveValuesRoundtripVerboseJsonTests
    {
        [TestMethod()]
        public void BinaryRoundtripVerboseJsonTest()
        {
            var values = new byte[][]
            {
                new byte[0],
                new byte[] { 0 },
                new byte[] { 42, Byte.MinValue, Byte.MaxValue },
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Binary");
        }

        [TestMethod()]
        public void BooleanRoundtripVerboseJsonTest()
        {
            var values = new bool[]
            {
                true, 
                false,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Boolean");
        }

        [TestMethod()]
        public void ByteRoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void DateTimeRoundtripVerboseJsonTest()
        {
            var values = new DateTime[]
            {
                new DateTime(2012, 4, 13, 2, 43, 10, 215),
                // This fails to roundtrip in V1 - because the date time is read as UTC
                // by design since the V1/V2 format stores everything as UTC and doesn't store kind information
                // new DateTime(2013, 5, 14, 3, 44, 11, DateTimeKind.Local),
                new DateTime(2014, 6, 15, 4, 45, 12, DateTimeKind.Unspecified),
                new DateTime(2015, 7, 16, 5, 46, 13, DateTimeKind.Utc),
                DateTime.MinValue,
                // This fails to roundtrip in V1 - the input has higher ticks precision than the output - by design.
                // DateTime.MaxValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.DateTime");
        }

        [TestMethod()]
        public void DateTimeOffsetRoundtripVerboseJsonTest()
        {
            var values = new DateTimeOffset[]
            {
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, TimeSpan.Zero),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(840)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-840)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(123)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-42)),
                DateTimeOffset.MinValue,
                // This fails to roundtrip in V1 - the input has higher ticks precision than the output - by design.
                // DateTimeOffset.MaxValue,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.DateTimeOffset");
        }

        [TestMethod()]
        public void DecimalRoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void DoubleRoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void GuidRoundtripVerboseJsonTest()
        {
            var values = new Guid[]
            {
                new Guid(0, 0, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }),
                new Guid("C8864E5E-BDB1-4FB2-A1C4-8F8E49C271EA"),
                Guid.Empty,
            };

            this.VerifyPrimitiveValuesRoundtrip(values, "Edm.Guid");
        }

        [TestMethod()]
        public void Int16RoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void Int32RoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void Int64RoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void SByteRoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void StringRoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void SingleRoundtripVerboseJsonTest()
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

        [TestMethod()]
        public void TimeRoundtripVerboseJsonTest()
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

        private void VerifyPrimitiveValuesRoundtrip(IEnumerable clrValues, string edmTypeName)
        {
            foreach (ODataVersion version in Enum.GetValues(typeof(ODataVersion)))
            {
                foreach (object clrValue in clrValues)
                {
                    this.VerifyPrimitiveValueRoundtrips(clrValue, edmTypeName, version, string.Format("Verbose JSON roundtrip value {0} of type {1} in version {2}.", clrValue, edmTypeName, version));
                }
            }
        }

        private void VerifyPrimitiveValueRoundtrips(object clrValue, string edmTypeName, ODataVersion version, string description)
        {
            IEdmModel model = new EdmModel();
            IEdmPrimitiveTypeReference typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)model.FindType(edmTypeName), true);

            MemoryStream stream = new MemoryStream();
            using (ODataVerboseJsonOutputContext outputContext = new ODataVerboseJsonOutputContext(
                ODataFormat.VerboseJson,
                new NonDisposingStream(stream),
                Encoding.UTF8,
                new ODataMessageWriterSettings() { Version = version },
                /*writingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                ODataVerboseJsonPropertyAndValueSerializer serializer = new ODataVerboseJsonPropertyAndValueSerializer(outputContext);
                serializer.WritePrimitiveValue(
                    clrValue,
                    /*collectionValidator*/ null,
                    typeReference);
            }

            stream.Position = 0;

            object actualValue;
            using (ODataVerboseJsonInputContext inputContext = new ODataVerboseJsonInputContext(
                ODataFormat.VerboseJson,
                stream,
                Encoding.UTF8,
                new ODataMessageReaderSettings(),
                version,
                /*readingResponse*/ true,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null))
            {
                ODataVerboseJsonPropertyAndValueDeserializer deserializer = new ODataVerboseJsonPropertyAndValueDeserializer(inputContext);
                deserializer.JsonReader.Read();
                actualValue = deserializer.ReadNonEntityValue(
                    typeReference,
                    /*duplicatePropertyNamesChecker*/ null,
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true,
                    /*propertyName*/ null);
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
