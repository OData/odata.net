//---------------------------------------------------------------------
// <copyright file="ODataUriUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Query
{
    public class ODataUriUtilsTests
    {
        [Theory]
        [InlineData(EdmPrimitiveTypeKind.SByte, (sbyte)2)]
        [InlineData(EdmPrimitiveTypeKind.Byte, (byte)2)]
        [InlineData(EdmPrimitiveTypeKind.Int16, (short)2)]
        [InlineData(EdmPrimitiveTypeKind.Int32, (int)2)]
        [InlineData(EdmPrimitiveTypeKind.Int64, (long)2)]
        public void TestIntegerConvertFromUriLiteral(EdmPrimitiveTypeKind kind, object expected)
        {
            Type expectedType = expected.GetType();
            var numberType = EdmCoreModel.Instance.GetPrimitive(kind, true);

            object primitiveValue = ODataUriUtils.ConvertFromUriLiteral("2", ODataVersion.V4, EdmCoreModel.Instance, numberType);

            Assert.IsType(expectedType, primitiveValue);
            Assert.Equal(expected, primitiveValue);
        }

        [Theory]
        [InlineData("42767")]
        [InlineData("-42767")]
        public void TestIntegerConvertFromUriLiteralThrowOverflow(string value)
        {
            var numberType = EdmCoreModel.Instance.GetInt16(true);

            Action test = () => ODataUriUtils.ConvertFromUriLiteral(value, ODataVersion.V4, EdmCoreModel.Instance, numberType);

            test.Throws<ODataException>(Strings.ODataUriUtils_ConvertFromUriLiteralOverflowNumber("Edm.Int16", "Value was either too large or too small for an Int16."));
        }

        [Fact]
        public void TestDecimalConvertToUriLiteral()
        {
            string decimalString = ODataUriUtils.ConvertToUriLiteral(decimal.MaxValue, ODataVersion.V4);
            Assert.Equal("79228162514264337593543950335", decimalString);

            decimalString = ODataUriUtils.ConvertToUriLiteral(decimal.MinValue, ODataVersion.V4);
            Assert.Equal("-79228162514264337593543950335", decimalString);

            decimalString = ODataUriUtils.ConvertToUriLiteral(112M, ODataVersion.V4);
            Assert.Equal("112", decimalString);
        }

        [Theory]
        [InlineData("79228162514264337593543950335")]
        [InlineData("-79228162514264337593543950335")]
        [InlineData("112M")]
        public void TestDecimalConvertFromUriLiteral(string value)
        {
            object dec = ODataUriUtils.ConvertFromUriLiteral(value, ODataVersion.V4);
#if NETCOREAPP3_1
            Assert.True(dec is double || dec is decimal);
#else
            Assert.True(dec is decimal);
#endif
        }

        [Fact]
        public void TestLongConvertToUriLiteral()
        {
            string longString = ODataUriUtils.ConvertToUriLiteral(long.MaxValue, ODataVersion.V4);
            Assert.Equal("9223372036854775807", longString);

            longString = ODataUriUtils.ConvertToUriLiteral(long.MinValue, ODataVersion.V4);
            Assert.Equal("-9223372036854775808", longString);

            longString = ODataUriUtils.ConvertToUriLiteral(123L, ODataVersion.V4);
            Assert.Equal("123", longString);
        }

        [Theory]
        [InlineData("9223372036854775807")]
        [InlineData("-9223372036854775808")]
        [InlineData("123L")]
        public void TestLongConvertFromUriLiteral(string value)
        {
            object longNumber = ODataUriUtils.ConvertFromUriLiteral(value, ODataVersion.V4);
            Assert.True(longNumber is long);
        }

        [Fact]
        public void TestSingleConvertToUriLiteral()
        {
            string singleString = ODataUriUtils.ConvertToUriLiteral(float.MaxValue, ODataVersion.V4);
#if NETCOREAPP3_1
            Assert.Equal("3.4028235E+38", singleString);
#else
            Assert.Equal("3.40282347E+38", singleString);
#endif 
            singleString = ODataUriUtils.ConvertToUriLiteral(float.MinValue, ODataVersion.V4);
#if NETCOREAPP3_1
            Assert.Equal("-3.4028235E+38", singleString);
#else
            Assert.Equal("-3.40282347E+38", singleString);
#endif
            singleString = ODataUriUtils.ConvertToUriLiteral(1000000000000f, ODataVersion.V4);
            Assert.Equal("1E+12", singleString);
        }

        [Theory]
        [InlineData("3.40282347E+38")]
        [InlineData("-3.40282347E+38")]
        [InlineData("1000000000000f")]
        public void TestSingleConvertFromUriLiteral(string value)
        {
            object singleNumber = ODataUriUtils.ConvertFromUriLiteral(value, ODataVersion.V4);
#if NETCOREAPP3_1
            Assert.True(singleNumber is float || singleNumber is double);
#else
            Assert.True(singleNumber is float);
#endif
        }

        [Fact]
        public void TestDoubleConvertToUriLiteral()
        {
            string doubleString = ODataUriUtils.ConvertToUriLiteral(double.MaxValue, ODataVersion.V4);
            Assert.Equal("1.7976931348623157E+308", doubleString);

            doubleString = ODataUriUtils.ConvertToUriLiteral(double.MinValue, ODataVersion.V4);
            Assert.Equal("-1.7976931348623157E+308", doubleString);

            doubleString = ODataUriUtils.ConvertToUriLiteral(1000000000000D, ODataVersion.V4);
            Assert.Equal("1000000000000.0", doubleString);
        }

        [Theory]
        [InlineData("1.7976931348623157E+308")]
        [InlineData("-1.7976931348623157E+308")]
        [InlineData("1000000000000D")]
        public void TestDoubleConvertFromUriLiteral(string value)
        {
            object doubleNumber = ODataUriUtils.ConvertFromUriLiteral(value, ODataVersion.V4);
            Assert.True(doubleNumber is double);
        }

        [Fact]
        public void TestBinaryConvertToUriLiteral()
        {
            byte[] value1 = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            string binaryString = ODataUriUtils.ConvertToUriLiteral(value1, ODataVersion.V4);
            Assert.Equal("binary'AAECAwQFBgc='", binaryString);

            byte[] value2 = new byte[] { 0x3, 0x1, 0x4, 0x1, 0x5, 0x9, 0x2, 0x6, 0x5, 0x3, 0x5, 0x9 };
            binaryString = ODataUriUtils.ConvertToUriLiteral(value2, ODataVersion.V4);
            Assert.Equal("binary'AwEEAQUJAgYFAwUJ'", binaryString);

            binaryString = ODataUriUtils.ConvertToUriLiteral(new byte[] { }, ODataVersion.V4);
            Assert.Equal("binary''", binaryString);
        }

        [Fact]
        public void TestBinaryConvertFromUriLiteral()
        {
            byte[] value1 = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            byte[] result = ODataUriUtils.ConvertFromUriLiteral("binary'AAECAwQFBgc='", ODataVersion.V4) as byte[];
            Assert.Equal(value1, result);

            byte[] value2 = new byte[] { 0x3, 0x1, 0x4, 0x1, 0x5, 0x9, 0x2, 0x6, 0x5, 0x3, 0x5, 0x9 };
            result = ODataUriUtils.ConvertFromUriLiteral("binary'AwEEAQUJAgYFAwUJ'", ODataVersion.V4) as byte[];
            Assert.Equal(value2, result);

            result = ODataUriUtils.ConvertFromUriLiteral("binary''", ODataVersion.V4) as byte[];
            Assert.Empty(result);

            // Invalid base64 value.
            Action action = () => { ODataUriUtils.ConvertFromUriLiteral("binary'AwEEAQUJAgYFAwUJ='", ODataVersion.V4); };
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_UnrecognizedLiteral("Edm.Binary", "binary'AwEEAQUJAgYFAwUJ='", "0", "binary'AwEEAQUJAgYFAwUJ='"));
        }

#region enum testings
        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumName()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'Red'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            Assert.Equal(1L + "", enumVal.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumVal.TypeName);
        }

        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumName_Combined()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'Red,Solid,BlueYellowStriped'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            Assert.Equal(31L + "", enumVal.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumVal.TypeName);
        }

        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumLong()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'11'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            Assert.Equal(11L + "", enumVal.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumVal.TypeName);
        }

        [Fact]
        public void TestEnumConvertToUriLiteral_EnumValue()
        {
            var val = new ODataEnumValue(11L + "", "Fully.Qualified.Namespace.ColorPattern");
            string enumValStr = ODataUriUtils.ConvertToUriLiteral(val, ODataVersion.V4);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern'11'", enumValStr);
        }
#endregion

#region Date/DateTimeOffset
        [Fact]
        public void TestDateConvertFromUriLiteral()
        {
            Date dateValue = (Date)ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetDate(false));
            Assert.Equal(new Date(1997, 7, 1), dateValue);

            DateTimeOffset dtoValue1 = (DateTimeOffset)ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetDateTimeOffset(false));
            Assert.Equal(new DateTimeOffset(1997, 7, 1, 0, 0, 0, new TimeSpan(0)), dtoValue1);

            var dtoValue2 = ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4);
            Assert.Equal(new Date(1997, 7, 1), dtoValue2);
        }

        [Fact]
        public void TestDateTimeOffsetConvertFromUriLiteral()
        {
            // Date is not in right format
            Action action = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-1T12:12:12-11:00", ODataVersion.V4);
            action.Throws<ODataException>(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-1T12:12:12-11:00"));

            // Time is not in right format
            Action action2 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:2-11:00", ODataVersion.V4);
            action2.Throws<ODataException>(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:2-11:00"));

            // Date and Time separator is incorrect
            // Call from DataUriUtils, it will parse till blank space which is a correct Date
            var dtoValue1 = ODataUriUtils.ConvertFromUriLiteral("1997-07-01 12:12:02-11:00", ODataVersion.V4);
            Assert.Equal(new Date(1997, 7, 1), dtoValue1);

            // Date is not with limit
            Action action4 = () => ODataUriUtils.ConvertFromUriLiteral("1997-13-01T12:12:12-11:00", ODataVersion.V4);
            action4.Throws<ODataException>(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-13-01T12:12:12-11:00"));

            // Time is not within limit
            Action action5 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:62-11:00", ODataVersion.V4);
            action5.Throws<ODataException>(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:62-11:00"));

            // Timezone separator is incorrect
            // Call from DataUriUtils, it will parse till blank space, so error message string is without timezone information.
            Action action6 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:02 11:00", ODataVersion.V4);
            action6.Throws<ODataException>(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:02"));

            // Timezone is not within limit
            Action action7 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:02-15:00", ODataVersion.V4);
            action7.Throws<ODataException>(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:02-15:00"));

            // Timezone is not specified
            Action action8 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:02", ODataVersion.V4);
            action8.Throws<ODataException>(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:02"));
        }

        [Fact]
        public void TestTimeOfDayConvertFromUriLiteral()
        {
            TimeOfDay timeValue1 = (TimeOfDay)ODataUriUtils.ConvertFromUriLiteral("12:13:14.015", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetTimeOfDay(false));
            Assert.Equal(new TimeOfDay(12, 13, 14, 15), timeValue1);

            TimeOfDay timeValue2 = (TimeOfDay)ODataUriUtils.ConvertFromUriLiteral("12:13:14.015", ODataVersion.V4);
            Assert.Equal(new TimeOfDay(12, 13, 14, 15), timeValue2);
        }

        [Fact]
        public void TestCollectionConvertFromBracketCollection()
        {
            object collection = ODataUriUtils.ConvertFromUriLiteral("[1,2,3]", ODataVersion.V4, HardCodedTestModel.TestModel, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            var collectionValue = Assert.IsType<ODataCollectionValue>(collection);
            Assert.Equal(3, collectionValue.Items.Count());
            Assert.Equal(new int[] { 1, 2, 3 }, collectionValue.Items.Cast<int>());
        }

        [Fact]
        public void TestCollectionConvertWithMismatchedBracket()
        {
            Action parse = () => ODataUriUtils.ConvertFromUriLiteral("[1,2,3)", ODataVersion.V4, HardCodedTestModel.TestModel, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            parse.Throws<ODataException>(Strings.ExpressionLexer_UnbalancedBracketExpression);
        }
#endregion

#region resource testings

        [Fact]
        public void TestResourceValueConvertToUriLiteral()
        {
            ODataResourceValue value = new ODataResourceValue
            {
                TypeName = "Fully.Qualified.Namespace.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 42 },
                    new ODataProperty { Name = "SSN", Value = "777-42-9001" }
                }
            };

            string actual = ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.V4, HardCodedTestModel.TestModel);
            Assert.Equal(@"{""@odata.type"":""#Fully.Qualified.Namespace.Person"",""ID"":42,""SSN"":""777-42-9001""}", actual);
        }

        [Fact]
        public void TestResourceValueWithInstanceAnnotationConvertToUriLiteral()
        {
            ODataResourceValue value = new ODataResourceValue
            {
                TypeName = "Fully.Qualified.Namespace.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 42 }
                },
                InstanceAnnotations = new []
                {
                    new ODataInstanceAnnotation("Custom.Annotation", new ODataResourceValue
                    {
                        TypeName = "Fully.Qualified.Namespace.Dog",
                        Properties = new []
                        {
                            new ODataProperty { Name = "Color", Value = "Red" }
                        }
                    })
                }
            };

            string actual = ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.V4, HardCodedTestModel.TestModel);
            Assert.Equal(
                "{" +
                  "\"@odata.type\":\"#Fully.Qualified.Namespace.Person\"," +
                  "\"@Custom.Annotation\":{\"@odata.type\":\"#Fully.Qualified.Namespace.Dog\",\"Color\":\"Red\"}," +
                  "\"ID\":42" +
                "}", actual);
        }

        [Fact]
        public void TestResourceValueWithNestedResourceValueConvertToUriLiteral()
        {
            ODataResourceValue value = new ODataResourceValue
            {
                TypeName = "Fully.Qualified.Namespace.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 42 },
                    new ODataProperty { Name = "SSN", Value = "777-42-9001" },
                    new ODataProperty
                    {
                        Name = "MyDog",
                        Value = new ODataResourceValue
                        {
                            TypeName = "Fully.Qualified.Namespace.Dog",
                            Properties = new []
                            {
                                new ODataProperty { Name = "Color", Value = "Red" }
                            }
                        }
                    }
                }
            };

            string actual = ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.V4, HardCodedTestModel.TestModel);
            Assert.Equal(@"{""@odata.type"":""#Fully.Qualified.Namespace.Person"",""ID"":42,""SSN"":""777-42-9001"",""MyDog"":{""Color"":""Red""}}", actual);
        }
#endregion resource testings

#region Collection of Resource Value

        [Fact]
        public void TestCollectionResourceValueWithInstanceAnnotationConvertToUriLiteral()
        {
            ODataResourceValue person = new ODataResourceValue
            {
                TypeName = "Fully.Qualified.Namespace.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 42 }
                },
                InstanceAnnotations = new[]
                {
                    new ODataInstanceAnnotation("Custom.Annotation", new ODataResourceValue
                    {
                        TypeName = "Fully.Qualified.Namespace.Dog",
                        Properties = new []
                        {
                            new ODataProperty { Name = "Color", Value = "Red" }
                        }
                    })
                }
            };
            ODataResourceValue employee = new ODataResourceValue
            {
                TypeName = "Fully.Qualified.Namespace.Employee",
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 42 },
                    new ODataProperty { Name = "WorkEmail", Value = "WorkEmail@work.com" }
                }
            };
            ODataCollectionValue collection = new ODataCollectionValue
            {
                TypeName = "Collection(Fully.Qualified.Namespace.Person)",
                Items = new[]
                {
                    person,
                    employee
                }
            };

            string actual = ODataUriUtils.ConvertToUriLiteral(collection, ODataVersion.V4, HardCodedTestModel.TestModel);
            Assert.Equal(
                "[" +
                  "{\"@Custom.Annotation\":{\"@odata.type\":\"#Fully.Qualified.Namespace.Dog\",\"Color\":\"Red\"},\"ID\":42}," +
                  "{\"@odata.type\":\"#Fully.Qualified.Namespace.Employee\",\"ID\":42,\"WorkEmail\":\"WorkEmail@work.com\"}" +
                "]", actual);
        }
#endregion Collection of Resource Value
    }
}
