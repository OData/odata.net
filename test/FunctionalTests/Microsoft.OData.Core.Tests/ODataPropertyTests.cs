//---------------------------------------------------------------------
// <copyright file="ODataPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataPropertyTests
    {
        private ODataProperty property;

        public enum Color
        {
            Red,
            Green,
            Blue
        }

        public ODataPropertyTests()
        {
            this.property = new ODataProperty();
        }

        [Fact]
        public void IfValueIsNullThenODataValueShouldBeODataNullValue()
        {
            this.property.Value = null;
            this.property.ODataValue.Should().BeOfType<ODataNullValue>();
        }

        [Fact]
        public void IfValueIsStringThenODataValueShouldBePrimitiveValue()
        {
            this.property.Value = "myString";
            this.property.ODataValue.As<ODataPrimitiveValue>().Value.Should().Be("myString");
        }

        [Fact]
        public void IfValueIsEnumThenODataValueShouldBeReferenceEqual()
        {
            var enumValue = new ODataEnumValue(Color.Green.ToString());
            this.property.Value = enumValue;
            this.property.ODataValue.Should().BeSameAs(enumValue);
        }

        [Fact]
        public void IfValueIsComplexThenODataValueShouldBeReferenceEqual()
        {
            ODataComplexValue complexValue = new ODataComplexValue();
            this.property.Value = complexValue;
            this.property.ODataValue.Should().BeSameAs(complexValue);
        }

        [Fact]
        public void IfValueIsODataPrimitiveValueThenODataValueShouldBeReferenceEqual()
        {
            ODataPrimitiveValue primitiveValue = new ODataPrimitiveValue(42);
            this.property.Value = primitiveValue;
            this.property.ODataValue.Should().BeSameAs(primitiveValue);
        }

        [Fact]
        public void SettingValueToNonPrimitiveTypeShouldThrow()
        {
            Action testSubject = () => this.property.Value = new ODataMessageWriterSettings();
            testSubject.ShouldThrow<ODataException>().WithMessage(Strings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType("Microsoft.OData.Core.ODataMessageWriterSettings"));
        }

        [Fact]
        public void NewPropertyShouldNotContainSerializationInfo()
        {
            this.property.SerializationInfo.Should().BeNull();
        }

        [Fact]
        public void PrimitiveValueShouldSupportUInt16()
        {
            ODataPrimitiveValue valueUInt16 = new ODataPrimitiveValue((UInt16)123);
            this.property.Value = valueUInt16;
            this.property.ODataValue.Should().BeSameAs(valueUInt16);
        }

        [Fact]
        public void PrimitiveValueShouldSupportUInt32()
        {
            ODataPrimitiveValue valueUInt32 = new ODataPrimitiveValue((UInt32)123);
            this.property.Value = valueUInt32;
            this.property.ODataValue.Should().BeSameAs(valueUInt32);
        }

        [Fact]
        public void PrimitiveValueShouldSupportUInt64()
        {
            ODataPrimitiveValue valueUInt64 = new ODataPrimitiveValue((UInt64)123);
            this.property.Value = valueUInt64;
            this.property.ODataValue.Should().BeSameAs(valueUInt64);
        }
    }
}
