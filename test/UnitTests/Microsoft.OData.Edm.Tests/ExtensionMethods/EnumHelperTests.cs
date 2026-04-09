//---------------------------------------------------------------------
// <copyright file="EnumHelperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Edm.Tests.ExtensionMethods
{
    public class EnumHelperTests
    {
        private readonly IEdmEnumType colorEnumType;
        private readonly IEdmEnumType flagsEnumType;

        public EnumHelperTests()
        {
            // Create a simple non-flags enum: Color { Red = 1, Green = 2, Blue = 3 }
            var colorEnum = new EdmEnumType("TestNamespace", "Color", false);
            colorEnum.AddMember(new EdmEnumMember(colorEnum, "Red", new EdmEnumMemberValue(1)));
            colorEnum.AddMember(new EdmEnumMember(colorEnum, "Green", new EdmEnumMemberValue(2)));
            colorEnum.AddMember(new EdmEnumMember(colorEnum, "Blue", new EdmEnumMemberValue(3)));
            colorEnumType = colorEnum;

            // Create a flags enum: Permissions { Read = 1, Write = 2, Execute = 4 }
            var flagsEnum = new EdmEnumType("TestNamespace", "Permissions", true);
            flagsEnum.AddMember(new EdmEnumMember(flagsEnum, "Read", new EdmEnumMemberValue(1)));
            flagsEnum.AddMember(new EdmEnumMember(flagsEnum, "Write", new EdmEnumMemberValue(2)));
            flagsEnum.AddMember(new EdmEnumMember(flagsEnum, "Execute", new EdmEnumMemberValue(4)));
            flagsEnumType = flagsEnum;
        }

        #region Basic Parsing Tests

        [Fact]
        public void TryParseEnum_WithNullEnumType_ThrowsArgumentNullException()
        {
            // Arrange
            IEdmEnumType enumType = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => enumType.TryParseEnum("Red", false, out _));
        }

        [Theory]
        [InlineData("Red", false, true, 1L)]          // Valid member name, case sensitive
        [InlineData("Green", false, true, 2L)]        // Valid member name, case sensitive
        [InlineData("Blue", false, true, 3L)]         // Valid member name, case sensitive
        [InlineData("red", true, true, 1L)]           // Valid member name, case insensitive
        [InlineData("GREEN", true, true, 2L)]         // Valid member name, case insensitive
        [InlineData("red", false, false, 0L)]         // Invalid: case mismatch
        [InlineData("Yellow", false, false, 0L)]      // Invalid: unknown member
        [InlineData("", false, false, 0L)]            // Invalid: empty string
        public void TryParseEnum_BasicParsing_ReturnsExpectedResult(string input, bool ignoreCase, bool expectedSuccess, long expectedValue)
        {
            // Act
            bool result = colorEnumType.TryParseEnum(input, ignoreCase, out long value);

            // Assert
            Assert.Equal(expectedSuccess, result);
            Assert.Equal(expectedValue, value);
        }

        #endregion

        #region Numeric Value Parsing Tests

        [Theory]
        [InlineData("2", 2L)]                           // Valid defined numeric value
        [InlineData("99", 99L)]                         // Undefined numeric value (allowed)
        [InlineData("-1", -1L)]                         // Negative numeric value
        [InlineData("0", 0L)]                           // Zero value
        [InlineData("9223372036854775806", 9223372036854775806L)] // Large value
        public void TryParseEnum_WithNumericValue_ReturnsTrue(string input, long expectedValue)
        {
            // Act
            bool result = colorEnumType.TryParseEnum(input, false, out long value);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, value);
        }

        #endregion

        #region Flags Enum Tests

        [Theory]
        [InlineData("Read", 1L)]                                    // Single value
        [InlineData("Read,Write", 3L)]                              // Multiple values (1 | 2 = 3)
        [InlineData("Read, Write, Execute", 7L)]                    // Multiple values with spaces (1 | 2 | 4 = 7)
        [InlineData("Read,2", 3L)]                                  // Mixed names and numbers (1 | 2 = 3)
        [InlineData("1,2,4", 7L)]                                   // Multiple numeric values (1 | 2 | 4 = 7)
        public void TryParseEnum_FlagsEnumWithValidValues_ReturnsTrue(string input, long expectedValue)
        {
            // Act
            bool result = flagsEnumType.TryParseEnum(input, false, out long value);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, value);
        }

        [Theory]
        [InlineData("Read,Invalid")]                                // Invalid member name in combination
        public void TryParseEnum_FlagsEnumWithInvalidValue_ReturnsFalse(string input)
        {
            // Act
            bool result = flagsEnumType.TryParseEnum(input, false, out long value);

            // Assert
            Assert.False(result);
            Assert.Equal(0L, value);
        }

        #endregion

        #region Non-Flags Enum with Multiple Values Tests

        [Theory]
        [InlineData("Red,Green")]                                   // Multiple values not allowed
        [InlineData("Red, Green")]                                  // Multiple values with spaces not allowed
        public void TryParseEnum_NonFlagsEnumWithMultipleValues_ReturnsFalse(string input)
        {
            // Act
            bool result = colorEnumType.TryParseEnum(input, false, out long value);

            // Assert
            Assert.False(result);
            Assert.Equal(0L, value);
        }

        #endregion

        #region Edge Cases and Whitespace Tests

        [Theory]
        [InlineData("  Red", 1L)]                                   // Leading whitespace
        [InlineData("Red  ", 1L)]                                   // Trailing whitespace
        [InlineData("  Red  ", 1L)]                                 // Surrounding whitespace
        public void TryParseEnum_WithWhitespace_ReturnsTrue(string input, long expectedValue)
        {
            // Act
            bool result = colorEnumType.TryParseEnum(input, false, out long value);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void TryParseEnum_FlagsEnumWithWhitespaceAroundCommas_ReturnsTrue()
        {
            // Act
            bool result = flagsEnumType.TryParseEnum("  Read  ,  Write  ", false, out long value);

            // Assert
            Assert.True(result);
            Assert.Equal(3L, value);
        }

        [Fact]
        public void TryParseEnum_WithOnlyWhitespace_ReturnsFalse()
        {
            // Act
            bool result = colorEnumType.TryParseEnum("   ", false, out long value);

            // Assert
            Assert.False(result);
            Assert.Equal(0L, value);
        }

        #endregion

        #region Invalid Input Tests

        [Theory]
        [InlineData("abc123")]                                      // Invalid number format
        [InlineData("Red@#$")]                                      // Special characters
        public void TryParseEnum_WithInvalidInput_ReturnsFalse(string input)
        {
            // Act
            bool result = colorEnumType.TryParseEnum(input, false, out long value);

            // Assert
            Assert.False(result);
            Assert.Equal(0L, value);
        }

        [Theory]
        [InlineData("Read,,Write")]                                 // Empty value between commas
        [InlineData("Read,")]                                       // Trailing comma
        public void TryParseEnum_FlagsEnumWithInvalidFormat_ReturnsFalse(string input)
        {
            // Act
            bool result = flagsEnumType.TryParseEnum(input, false, out long value);

            // Assert
            Assert.False(result);
            Assert.Equal(0L, value);
        }

        #endregion
    }
}
