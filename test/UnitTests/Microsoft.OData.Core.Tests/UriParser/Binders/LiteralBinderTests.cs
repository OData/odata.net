//---------------------------------------------------------------------
// <copyright file="LiteralBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class LiteralBinderTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        #region Basic Literal Binding Tests

        [Fact]
        public void BindLiteralShouldReturnIntValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(5)) as ConstantNode;
            Assert.Equal(5, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnDateTimeOffsetValue()
        {
            var value = new DateTimeOffset(2012, 12, 2, 3, 34, 20, 0, new TimeSpan(2, 0, 0));
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnDateValue()
        {
            var value = new DateOnly(2012, 12, 2);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnTimeOfDayValue()
        {
            var value = new TimeOnly(10, 15, 5, 20);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnQueryNode()
        {
            var value = GeometryPoint.Create(5, 2);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldSetLiteralTextFromToken()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(1, "originalText")) as ConstantNode;
            Assert.Equal("originalText", result.LiteralText);
        }

        #endregion

        #region Null Literal Tests

        [Fact]
        public void BindLiteralShouldReturnNullConstantNode()
        {
            var token = new LiteralToken(null, "null");
            ConstantNode result = LiteralBinder.BindLiteral(token) as ConstantNode;
            Assert.Null(result.Value);
        }

        [Fact]
        public void BindLiteralShouldBindNullToNullableType()
        {
            var token = new LiteralToken(null, "null")
            {
                ExpectedType = EdmCoreModel.Instance.GetString(true)
            };
            ConstantNode result = LiteralBinder.BindLiteral(token) as ConstantNode;
            Assert.Null(result.Value);
            Assert.True(result.TypeReference.IsNullable);
        }

        [Fact]
        public void BindLiteralShouldThrowWhenBindingNullToNonNullableType()
        {
            var token = new LiteralToken(null, "null")
            {
                ExpectedType = EdmCoreModel.Instance.GetString(false)
            };
            Assert.Throws<ODataException>(() => LiteralBinder.BindLiteral(token));
        }

        #endregion

        #region String Literal Tests

        [Fact]
        public void BindLiteralShouldReturnStringValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken("test string")) as ConstantNode;
            Assert.Equal("test string", result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnEmptyStringValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(string.Empty)) as ConstantNode;
            Assert.Equal(string.Empty, result.Value);
        }

        #endregion

        #region Numeric Literal Tests

        [Fact]
        public void BindLiteralShouldReturnLongValue()
        {
            long value = 9223372036854775807L;
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnDoubleValue()
        {
            double value = 3.14159;
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnDecimalValue()
        {
            decimal value = 123.456m;
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnFloatValue()
        {
            float value = 2.5f;
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnByteValue()
        {
            byte value = 255;
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnSByteValue()
        {
            sbyte value = -128;
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnInt16Value()
        {
            short value = 32767;
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        #endregion

        #region Boolean Literal Tests

        [Fact]
        public void BindLiteralShouldReturnTrueValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(true)) as ConstantNode;
            Assert.True((bool)result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnFalseValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(false)) as ConstantNode;
            Assert.False((bool)result.Value);
        }

        #endregion

        #region Guid Literal Tests

        [Fact]
        public void BindLiteralShouldReturnGuidValue()
        {
            Guid value = Guid.NewGuid();
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        #endregion

        #region TimeSpan/Duration Literal Tests

        [Fact]
        public void BindLiteralShouldReturnTimeSpanValue()
        {
            TimeSpan value = new TimeSpan(1, 2, 3, 4);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        #endregion

        #region Binary Literal Tests

        [Fact]
        public void BindLiteralShouldReturnBinaryValue()
        {
            byte[] value = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        #endregion

        #region Enum Literal Tests

        [Fact]
        public void BindLiteralShouldBindStringToEnumWithExpectedType()
        {
            var bindingState = CreateBindingState();
            var enumType = new EdmEnumType("TestNamespace", "TestColor", EdmPrimitiveTypeKind.Int32, false);
            enumType.AddMember("Red", new EdmEnumMemberValue(1));
            enumType.AddMember("Blue", new EdmEnumMemberValue(2));
            var colorType = new EdmEnumTypeReference(enumType, false);

            var token = new LiteralToken("Red", "'Red'", EdmCoreModel.Instance.GetString(false))
            {
                ExpectedType = colorType
            };

            ConstantNode result = LiteralBinder.BindLiteral(token, bindingState) as ConstantNode;
            Assert.NotNull(result);
            Assert.IsType<ODataEnumValue>(result.Value);
        }

        [Fact]
        public void BindLiteralShouldBindIntToEnumWithExpectedType()
        {
            var bindingState = CreateBindingState();
            var enumType = new EdmEnumType("TestNamespace", "TestColor", EdmPrimitiveTypeKind.Int32, false);
            enumType.AddMember("Red", new EdmEnumMemberValue(1));
            enumType.AddMember("Blue", new EdmEnumMemberValue(2));
            var colorType = new EdmEnumTypeReference(enumType, false);

            var token = new LiteralToken(1, "1", EdmCoreModel.Instance.GetInt32(false))
            {
                ExpectedType = colorType
            };

            ConstantNode result = LiteralBinder.BindLiteral(token, bindingState) as ConstantNode;
            Assert.NotNull(result);
            Assert.IsType<ODataEnumValue>(result.Value);
        }

        [Fact]
        public void BindLiteralShouldThrowForInvalidEnumValue()
        {
            var bindingState = CreateBindingState();
            var enumType = new EdmEnumType("TestNamespace", "TestColor", EdmPrimitiveTypeKind.Int32, false);
            enumType.AddMember("Red", new EdmEnumMemberValue(1));
            enumType.AddMember("Blue", new EdmEnumMemberValue(2));
            var colorType = new EdmEnumTypeReference(enumType, false);

            var token = new LiteralToken("InvalidColor", "'InvalidColor'", EdmCoreModel.Instance.GetString(false))
            {
                ExpectedType = colorType
            };

            Assert.Throws<ODataException>(() => LiteralBinder.BindLiteral(token, bindingState));
        }

        #endregion

        #region Type Conversion Tests

        [Fact]
        public void BindLiteralShouldConvertInt32ToByteWhenInRange()
        {
            var token = new LiteralToken(100, "100")
            {
                ExpectedType = EdmCoreModel.Instance.GetByte(false)
            };

            ConstantNode result = LiteralBinder.BindLiteral(token) as ConstantNode;
            Assert.Equal((byte)100, result.Value);
        }

        [Fact]
        public void BindLiteralShouldConvertInt32ToSByteWhenInRange()
        {
            var token = new LiteralToken(-50, "-50")
            {
                ExpectedType = EdmCoreModel.Instance.GetSByte(false)
            };

            ConstantNode result = LiteralBinder.BindLiteral(token) as ConstantNode;
            Assert.Equal((sbyte)-50, result.Value);
        }

        [Fact]
        public void BindLiteralShouldConvertInt32ToInt16WhenInRange()
        {
            var token = new LiteralToken(1000, "1000")
            {
                ExpectedType = EdmCoreModel.Instance.GetInt16(false)
            };

            ConstantNode result = LiteralBinder.BindLiteral(token) as ConstantNode;
            Assert.Equal((short)1000, result.Value);
        }

        [Fact]
        public void BindLiteralShouldConvertInt32ToInt64WhenInRange()
        {
            var token = new LiteralToken(5, "5")
            {
                ExpectedType = EdmCoreModel.Instance.GetInt64(false)
            };

            var result = LiteralBinder.BindLiteral(token);
            ConstantNode node = Assert.IsType<ConstantNode>(result);
            Assert.Equal((long)5, node.Value);
        }

        [Fact]
        public void BindLiteralShouldThrowWhenConversionIsNotPossible()
        {
            var token = new LiteralToken("not a number", "'not a number'")
            {
                ExpectedType = EdmCoreModel.Instance.GetInt32(false)
            };

            Assert.Throws<ODataException>(() => LiteralBinder.BindLiteral(token));
        }

        #endregion

        #region Spatial Literal Tests

        [Fact]
        public void BindLiteralShouldReturnGeographyPointValue()
        {
            var value = GeographyPoint.Create(47.6, -122.3);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnAnotherGeometryPointValue()
        {
            var value = GeometryPoint.Create(10.5, 20.3);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        #endregion

        #region Edge Cases and Special Scenarios

        [Fact]
        public void BindLiteralWithInferredTypeAndNoExpectedType()
        {
            var token = new LiteralToken(42, "42");
            ConstantNode result = LiteralBinder.BindLiteral(token) as ConstantNode;
            Assert.Equal(42, result.Value);
            Assert.NotNull(result.TypeReference);
        }

        [Fact]
        public void BindLiteralWithOriginalTextOnly()
        {
            var token = new LiteralToken(99, "99");
            ConstantNode result = LiteralBinder.BindLiteral(token) as ConstantNode;
            Assert.Equal("99", result.LiteralText);
        }

        [Fact]
        public void BindLiteralShouldThrowWhenTokenIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => LiteralBinder.BindLiteral(null));
        }

        [Fact]
        public void BindLiteralWithZeroValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(0)) as ConstantNode;
            Assert.Equal(0, result.Value);
        }

        [Fact]
        public void BindLiteralWithNegativeValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(-42)) as ConstantNode;
            Assert.Equal(-42, result.Value);
        }

        [Fact]
        public void BindLiteralWithMaxInt32Value()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(int.MaxValue)) as ConstantNode;
            Assert.Equal(int.MaxValue, result.Value);
        }

        [Fact]
        public void BindLiteralWithMinInt32Value()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(int.MinValue)) as ConstantNode;
            Assert.Equal(int.MinValue, result.Value);
        }

        #endregion

        #region Helper Methods

        private static BindingState CreateBindingState()
        {
            ResourceRangeVariable implicitRangeVariable = new ResourceRangeVariable(
                ExpressionConstants.It,
                HardCodedTestModel.GetPersonTypeReference(),
                HardCodedTestModel.GetPeopleSet());

            var bindingState = new BindingState(configuration)
            {
                ImplicitRangeVariable = implicitRangeVariable
            };

            bindingState.RangeVariables.Push(bindingState.ImplicitRangeVariable);
            return bindingState;
        }

        #endregion
    }
}
