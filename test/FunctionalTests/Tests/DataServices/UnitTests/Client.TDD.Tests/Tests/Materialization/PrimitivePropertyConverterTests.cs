//---------------------------------------------------------------------
// <copyright file="PrimitivePropertyConverterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using System.Xml;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.OData;
    using Xunit;

    public class PrimitivePropertyConverterTests
    {
        private readonly PrimitivePropertyConverter jsonConverter = new PrimitivePropertyConverter();

        private readonly Dictionary<object, object> basicConversionsList = new Dictionary<object, object>
        {
            // from string to all types
            {"true", true},
            {"1", 1},
            {"2", (byte)2},
            {"-1", (sbyte)-1},
            {"3", (short)3},
            {XmlConvert.ToString(long.MaxValue), long.MaxValue},
            {XmlConvert.ToString(float.MaxValue), float.MaxValue},
            {XmlConvert.ToString(double.MinValue), double.MinValue},
            {XmlConvert.ToString(decimal.MaxValue), decimal.MaxValue},
            {XmlConvert.ToString(DateTimeOffset.MinValue), DateTimeOffset.MinValue},
            {XmlConvert.ToString(TimeSpan.MaxValue), TimeSpan.MaxValue},
            {XmlConvert.ToString(Guid.Empty), Guid.Empty},
            {Convert.ToBase64String(new byte[] {0, 1, 2}), new byte[] {0, 1, 2}},
            {Date.MinValue.ToString(), Date.MinValue},
            {TimeOfDay.MinValue.ToString(), TimeOfDay.MinValue},

            // from all types to string
            {true, "true"},
            {1, "1"},
            {(byte)2, "2"},
            {(sbyte)-1, "-1"},
            {(short)3, "3"},
            {long.MaxValue, XmlConvert.ToString(long.MaxValue)},
            {float.MaxValue, XmlConvert.ToString(float.MaxValue)},
            {double.MinValue, XmlConvert.ToString(double.MinValue)},
            {decimal.MaxValue, XmlConvert.ToString(decimal.MaxValue)},
            {DateTimeOffset.MinValue, XmlConvert.ToString(DateTimeOffset.MinValue)},
            {TimeSpan.MaxValue, XmlConvert.ToString(TimeSpan.MaxValue)},
            {Guid.Empty, XmlConvert.ToString(Guid.Empty)},
            {new byte[] {0, 1, 2, }, Convert.ToBase64String(new byte[] {0, 1, 2})},
            {Date.MaxValue, Date.MaxValue.ToString()},
            {TimeOfDay.MaxValue, TimeOfDay.MaxValue.ToString()},

            // from byte to other numeric types
            {(byte)5, (sbyte)5},
            {(byte)6, (short)6},
            {(byte)7, 7},
            {(byte)8, 8L},
            {(byte)9, 9.0f},
            {(byte)10, 10.0},
            {(byte)11, 11m},

            // from sbyte to other numeric types
            {(sbyte)12, (byte)12},
            {(sbyte)-13, (short)-13},
            {(sbyte)-14, -14},
            {(sbyte)-15, -15L},
            {(sbyte)-16, -16.0f},
            {(sbyte)-17, -17.0},
            {(sbyte)-18, -18m},

            // from short to other numeric types
            {(short)19, (byte)19},
            {(short)-20, (sbyte)-20},
            {(short)21, 21},
            {(short)22, 22L},
            {(short)23, 23.0f},
            {(short)24, 24.0},
            {(short)25, 25m},

            // from int to other numeric types
            {26, (byte)26},
            {-27, (sbyte)-27},
            {28, (short)28},
            {29, 29L},
            {30, 30.0f},
            {31, 31.0},
            {32, 32m},

            // from long to other numeric types
            {33L, (byte)33},
            {-34L, (sbyte)-34},
            {35L, (short)35},
            {36L, 36},
            {37L, 37.0f},
            {38L, 38.0},
            {39L, 39m},

            // from float to other numeric types
            {40.0f, (byte)40},
            {-41.0f, (sbyte)-41},
            {42.0f, (short)42},
            {43.0f, 43},
            {44.0f, 44L}, // Converting to long does not work in JSON because the value should have been quoted
            {45.1f, 45.1},
            {46.0f, 46m},

            // from double to other numeric types
            {47.0d, (byte)47},
            {-48.0d, (sbyte)-48},
            {49.0d, (short)49},
            {50.0d, 50},
            {51.0d, 51L}, // Converting to long does not work in JSON because the value should have been quoted
            {52.1d, 52.1f},
            {53.0d, 53m},

            // from decimal to other numeric types
            {54.0m, (byte)54},
            {-55.0m, (sbyte)-55},
            {56.0m, (short)56},
            {57.0m, 57},
            {58.0m, 58L}, // Converting to long does not work in JSON because the value should have been quoted
            {59.1m, 59.1f},
            {60.1m, 60.1d},
        };

        [Fact]
        public void GeometryToGeographyInJson()
        {
            var point = GeometryPoint.Create(1, 2);
            var result = this.jsonConverter.ConvertPrimitiveValue(point, typeof(GeographyPoint));
            result.Should().BeAssignableTo<GeographyPoint>();
            result.As<GeographyPoint>().Latitude.Should().BeInRange(2, 2);
            result.As<GeographyPoint>().Longitude.Should().BeInRange(1, 1);
        }

        [Fact]
        public void GeographyToGeometryInJson()
        {
            var point = GeographyPoint.Create(1, 2);
            var result = this.jsonConverter.ConvertPrimitiveValue(point, typeof(Geometry));
            result.Should().BeAssignableTo<GeometryPoint>();
            result.As<GeometryPoint>().X.Should().BeInRange(2, 2);
            result.As<GeometryPoint>().Y.Should().BeInRange(1, 1);
        }

        [Fact]
        public void GeographyPointToGeographyShouldNotDoAnything()
        {
            var point = GeographyPoint.Create(1, 2);
            var result = this.jsonConverter.ConvertPrimitiveValue(point, typeof(Geography));
            result.Should().BeSameAs(point);
        }

        [Fact]
        public void GeometryPointToGeometryShouldNotDoAnything()
        {
            var point = GeometryPoint.Create(1, 2);
            var result = this.jsonConverter.ConvertPrimitiveValue(point, typeof(Geometry));
            result.Should().BeSameAs(point);
        }

        [Fact]
        public void FloatShouldConvertToDoubleWithoutLossOfPrecision()
        {
            this.jsonConverter.ConvertPrimitiveValue(45.1f, typeof(double)).Should().Be(45.1d);
        }

        [Fact]
        public void StringShouldBeSameReferenceAfterConversion()
        {
            const string test = "temp";
            this.jsonConverter.ConvertPrimitiveValue(test, typeof(string)).Should().BeSameAs(test);
        }

        [Fact]
        public void StringShouldConvertToUri()
        {
            this.jsonConverter.ConvertPrimitiveValue("http://temp.org", typeof(Uri)).Should().Be(new Uri("http://temp.org"));
        }

        [Fact]
        public void StringShouldConvertToXElement()
        {
            this.jsonConverter.ConvertPrimitiveValue("<Fake/>", typeof(XElement)).As<XElement>().Should().Be(XElement.Parse("<Fake/>"));
        }

        [Fact]
        public void InfinityShouldConvertFromFloatToDouble()
        {
            this.jsonConverter.ConvertPrimitiveValue(float.PositiveInfinity, typeof(double)).Should().Be(double.PositiveInfinity);
        }

        [Fact]
        public void NegativeInfinityShouldConvertFromDoubleToFloat()
        {
            this.jsonConverter.ConvertPrimitiveValue(double.NegativeInfinity, typeof(float)).Should().Be(float.NegativeInfinity);
        }

        [Fact]
        public void NaNShouldConvertFromStringToFloat()
        {
            float.IsNaN(this.jsonConverter.ConvertPrimitiveValue("NaN", typeof(float)).As<float>()).Should().BeTrue();
        }

        [Fact]
        public void ConversionsForAllBasicPrimitiveTypesShouldWork()
        {
            foreach (var conversion in this.basicConversionsList)
            {
                Type targetType = conversion.Value.GetType();
                object result = this.jsonConverter.ConvertPrimitiveValue(conversion.Key, targetType);

                if (targetType != typeof(byte[]))
                {
                    result.Should().Be(conversion.Value);
                }
                else
                {
                    result.As<byte[]>().Should().ContainInOrder((byte[])conversion.Value);
                }
            }
        }

        [Fact]
        public void EnumCouldBeConvertedToString()
        {
            this.jsonConverter.ConvertPrimitiveValue(new ODataEnumValue("Yellow"), typeof(string)).Should().Be("Yellow");
        }

        [Fact]
        public void JsonConversionForDateTimeShouldThrow()
        {
            Type dateTimeType = typeof(DateTime);
            var dateTime = DateTime.Now;
            var dateTimeOffset = (DateTimeOffset)dateTime;
            var result = this.jsonConverter.ConvertPrimitiveValue(dateTimeOffset, dateTimeType);

            // returned DateTime should be the DateTime UTC
            Assert.True(result is DateTime);
            Assert.Equal(result, dateTime.ToUniversalTime());
        }
    }
}