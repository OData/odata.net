//---------------------------------------------------------------------
// <copyright file="ODataPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
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
            Assert.IsType<ODataNullValue>(this.property.ODataValue);
        }

        [Fact]
        public void IfValueIsStringThenODataValueShouldBePrimitiveValue()
        {
            this.property.Value = "myString";
            var value = Assert.IsType<ODataPrimitiveValue>(this.property.ODataValue);
            Assert.Equal("myString", value.Value);
        }

        [Fact]
        public void IfValueIsEnumThenODataValueShouldBeReferenceEqual()
        {
            var enumValue = new ODataEnumValue(Color.Green.ToString());
            this.property.Value = enumValue;
            Assert.Same(enumValue, this.property.ODataValue);
        }

        [Fact]
        public void IfValueIsResourceThenODataValueShouldBeReferenceEqual()
        {
            ODataResourceValue resourceValue = new ODataResourceValue();
            this.property.Value = resourceValue;
            Assert.Same(resourceValue, this.property.ODataValue);
        }

        [Fact]
        public void IfValueIsODataPrimitiveValueThenODataValueShouldBeReferenceEqual()
        {
            ODataPrimitiveValue primitiveValue = new ODataPrimitiveValue(42);
            this.property.Value = primitiveValue;
            Assert.Same(primitiveValue, this.property.ODataValue);
        }

        [Fact]
        public void SettingValueToNonPrimitiveTypeShouldThrow()
        {
            Action testSubject = () => this.property.Value = new ODataMessageWriterSettings();
            testSubject.Throws<ODataException>(Strings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType("Microsoft.OData.ODataMessageWriterSettings"));
        }

        [Fact]
        public void NewPropertyShouldNotContainSerializationInfo()
        {
            Assert.Null(this.property.SerializationInfo);
        }

        [Fact]
        public void PrimitiveValueShouldSupportUInt16()
        {
            ODataPrimitiveValue valueUInt16 = new ODataPrimitiveValue((UInt16)123);
            this.property.Value = valueUInt16;
            Assert.Same(valueUInt16, this.property.ODataValue);
        }

        [Fact]
        public void PrimitiveValueShouldSupportUInt32()
        {
            ODataPrimitiveValue valueUInt32 = new ODataPrimitiveValue((UInt32)123);
            this.property.Value = valueUInt32;
            Assert.Same(valueUInt32, this.property.ODataValue);
        }

        [Fact]
        public void PrimitiveValueShouldSupportUInt64()
        {
            ODataPrimitiveValue valueUInt64 = new ODataPrimitiveValue((UInt64)123);
            this.property.Value = valueUInt64;
            Assert.Same(valueUInt64, this.property.ODataValue);
        }
    }
}
