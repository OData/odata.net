//---------------------------------------------------------------------
// <copyright file="ODataUriUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.Query
{
    public class ODataUriUtilsTests
    {
        [Fact]
        public void TestDecimalConvertToUriLiteral()
        {
            string decimalString = ODataUriUtils.ConvertToUriLiteral(decimal.MaxValue, ODataVersion.V4);
            decimalString.Should().Be("79228162514264337593543950335");

            decimalString = ODataUriUtils.ConvertToUriLiteral(decimal.MinValue, ODataVersion.V4);
            decimalString.Should().Be("-79228162514264337593543950335");

            decimalString = ODataUriUtils.ConvertToUriLiteral(112M, ODataVersion.V4);
            decimalString.Should().Be("112");
        }

        [Fact]
        public void TestDecimalConvertFromUriLiteral()
        {
            object dec = ODataUriUtils.ConvertFromUriLiteral("79228162514264337593543950335", ODataVersion.V4);
            (dec is decimal).Should().Be(true);

            dec = ODataUriUtils.ConvertFromUriLiteral("-79228162514264337593543950335", ODataVersion.V4);
            (dec is decimal).Should().Be(true);

            dec = ODataUriUtils.ConvertFromUriLiteral("112M", ODataVersion.V4);
            (dec is decimal).Should().Be(true);
        }

        [Fact]
        public void TestLongConvertToUriLiteral()
        {
            string longString = ODataUriUtils.ConvertToUriLiteral(long.MaxValue, ODataVersion.V4);
            longString.Should().Be("9223372036854775807");

            longString = ODataUriUtils.ConvertToUriLiteral(long.MinValue, ODataVersion.V4);
            longString.Should().Be("-9223372036854775808");

            longString = ODataUriUtils.ConvertToUriLiteral(123L, ODataVersion.V4);
            longString.Should().Be("123");
        }

        [Fact]
        public void TestLongConvertFromUriLiteral()
        {
            object longNumber = ODataUriUtils.ConvertFromUriLiteral("9223372036854775807", ODataVersion.V4);
            (longNumber is long).Should().Be(true);

            longNumber = ODataUriUtils.ConvertFromUriLiteral("-9223372036854775808", ODataVersion.V4);
            (longNumber is long).Should().Be(true);

            longNumber = ODataUriUtils.ConvertFromUriLiteral("123L", ODataVersion.V4);
            (longNumber is long).Should().Be(true);
        }

        [Fact]
        public void TestSingleConvertToUriLiteral()
        {
            string singleString = ODataUriUtils.ConvertToUriLiteral(float.MaxValue, ODataVersion.V4);
            singleString.Should().Be("3.40282347E+38");

            singleString = ODataUriUtils.ConvertToUriLiteral(float.MinValue, ODataVersion.V4);
            singleString.Should().Be("-3.40282347E+38");

            singleString = ODataUriUtils.ConvertToUriLiteral(1000000000000f, ODataVersion.V4);
            singleString.Should().Be("1E+12");
        }

        [Fact]
        public void TestSingleConvertFromUriLiteral()
        {
            object singleNumber = ODataUriUtils.ConvertFromUriLiteral("3.40282347E+38", ODataVersion.V4);
            (singleNumber is float).Should().Be(true);

            singleNumber = ODataUriUtils.ConvertFromUriLiteral("-3.40282347E+38", ODataVersion.V4);
            (singleNumber is float).Should().Be(true);

            singleNumber = ODataUriUtils.ConvertFromUriLiteral("1000000000000f", ODataVersion.V4);
            (singleNumber is float).Should().Be(true);
        }

        [Fact]
        public void TestDoubleConvertToUriLiteral()
        {
            string doubleString = ODataUriUtils.ConvertToUriLiteral(double.MaxValue, ODataVersion.V4);
            doubleString.Should().Be("1.7976931348623157E+308");

            doubleString = ODataUriUtils.ConvertToUriLiteral(double.MinValue, ODataVersion.V4);
            doubleString.Should().Be("-1.7976931348623157E+308");

            doubleString = ODataUriUtils.ConvertToUriLiteral(1000000000000D, ODataVersion.V4);
            doubleString.Should().Be("1000000000000.0");
        }

        [Fact]
        public void TestDoubleConvertFromUriLiteral()
        {
            object doubleNumber = ODataUriUtils.ConvertFromUriLiteral("1.7976931348623157E+308", ODataVersion.V4);
            (doubleNumber is double).Should().Be(true);

            doubleNumber = ODataUriUtils.ConvertFromUriLiteral("-1.7976931348623157E+308", ODataVersion.V4);
            (doubleNumber is double).Should().Be(true);

            doubleNumber = ODataUriUtils.ConvertFromUriLiteral("1000000000000D", ODataVersion.V4);
            (doubleNumber is double).Should().Be(true);
        }

        [Fact]
        public void TestBinaryConvertToUriLiteral()
        {
            byte[] value1 = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            string binaryString = ODataUriUtils.ConvertToUriLiteral(value1, ODataVersion.V4);
            binaryString.Should().Be("binary'AAECAwQFBgc='");

            byte[] value2 = new byte[] { 0x3, 0x1, 0x4, 0x1, 0x5, 0x9, 0x2, 0x6, 0x5, 0x3, 0x5, 0x9 };
            binaryString = ODataUriUtils.ConvertToUriLiteral(value2, ODataVersion.V4);
            binaryString.Should().Be("binary'AwEEAQUJAgYFAwUJ'");

            binaryString = ODataUriUtils.ConvertToUriLiteral(new byte[] { }, ODataVersion.V4);
            binaryString.Should().Be("binary''");
        }

        [Fact]
        public void TestBinaryConvertFromUriLiteral()
        {
            byte[] value1 = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            byte[] result = ODataUriUtils.ConvertFromUriLiteral("binary'AAECAwQFBgc='", ODataVersion.V4) as byte[];
            result.Should().BeEquivalentTo(value1);

            byte[] value2 = new byte[] { 0x3, 0x1, 0x4, 0x1, 0x5, 0x9, 0x2, 0x6, 0x5, 0x3, 0x5, 0x9 };
            result = ODataUriUtils.ConvertFromUriLiteral("binary'AwEEAQUJAgYFAwUJ'", ODataVersion.V4) as byte[];
            result.Should().BeEquivalentTo(value2);

            result = ODataUriUtils.ConvertFromUriLiteral("binary''", ODataVersion.V4) as byte[];
            result.Should().BeEmpty();

            // Invalid base64 value.
            Action action = () => { ODataUriUtils.ConvertFromUriLiteral("binary'AwEEAQUJAgYFAwUJ='", ODataVersion.V4); };
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_UnrecognizedLiteral("Edm.Binary", "binary'AwEEAQUJAgYFAwUJ='", "0", "binary'AwEEAQUJAgYFAwUJ='"));
        }

        #region enum testings
        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumName()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'Red'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            enumVal.Value.Should().Be(1L + "");
            enumVal.TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
        }

        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumName_Combined()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'Red,Solid,BlueYellowStriped'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            enumVal.Value.Should().Be(31L + "");
            enumVal.TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
        }

        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumLong()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'11'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            enumVal.Value.Should().Be(11L + "");
            enumVal.TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
        }

        [Fact]
        public void TestEnumConvertToUriLiteral_EnumValue()
        {
            var val = new ODataEnumValue(11L + "", "Fully.Qualified.Namespace.ColorPattern");
            string enumValStr = ODataUriUtils.ConvertToUriLiteral(val, ODataVersion.V4);
            enumValStr.Should().Be("Fully.Qualified.Namespace.ColorPattern'11'");
        }
        #endregion

        #region Date/DateTimeOffset
        [Fact]
        public void TestDateConvertFromUriLiteral()
        {
            Date dateValue = (Date)ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetDate(false));
            dateValue.Should().Be(new Date(1997, 7, 1));
        }

        [Fact]
        public void TestDateTimeOffsetConvertFromUriLiteral()
        {
            DateTimeOffset dtoValue1 = (DateTimeOffset)ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetDateTimeOffset(false));
            dtoValue1.Should().Be(new DateTimeOffset(new DateTime(1997, 7, 1)));

            DateTimeOffset dtoValue2 = (DateTimeOffset)ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4);
            dtoValue2.Should().Be(new DateTimeOffset(new DateTime(1997, 7, 1)));
        }

        [Fact]
        public void TesTimeOfDayConvertFromUriLiteral()
        {
            TimeOfDay timeValue1 = (TimeOfDay)ODataUriUtils.ConvertFromUriLiteral("12:13:14.015", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetTimeOfDay(false));
            timeValue1.Should().Be(new TimeOfDay(12, 13, 14, 15));

            TimeOfDay timeValue2 = (TimeOfDay)ODataUriUtils.ConvertFromUriLiteral("12:13:14.015", ODataVersion.V4);
            timeValue2.Should().Be(new TimeOfDay(12, 13, 14, 15));
        }
        #endregion
    }
}
