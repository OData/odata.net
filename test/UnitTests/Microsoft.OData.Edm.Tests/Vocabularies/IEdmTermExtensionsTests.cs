//---------------------------------------------------------------------
// <copyright file="IEdmTermExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    /// <summary>
    /// Test IEdmTermExtensions
    /// </summary>
    public class IEdmTermExtensionsTests
    {
        [Fact]
        public void GetDefaultValueExpression_Works_ForTermWithOrWithoutDefaultValue()
        {
            // Arrange
            IEdmTypeReference typeRef = EdmCoreModel.Instance.GetBoolean(true);

            EdmTerm term1 = new EdmTerm("NS", "MyTerm", typeRef);
            EdmTerm term2 = new EdmTerm("NS", "MyTerm", typeRef, null, "true");

            // Act & Assert
            IEdmExpression exp1 = term1.GetDefaultValueExpression();
            Assert.Null(exp1);

            IEdmExpression exp2 = term2.GetDefaultValueExpression();
            Assert.NotNull(exp2);
            EdmBooleanConstant constant = Assert.IsType<EdmBooleanConstant>(exp2);
            Assert.True(constant.Value);
        }

        [Fact]
        public void GetDefaultValueExpression_Works_ForTermFromCoreVocabularyModel()
        {
            // Arrange
            IEdmTerm term = CoreVocabularyModel.Instance.FindTerm("Org.OData.Core.V1.PositionalInsert");
            Assert.NotNull(term);
            Assert.NotNull(term.DefaultValue);
            Assert.Equal("true", term.DefaultValue);

            // Act
            IEdmExpression exp = term.GetDefaultValueExpression();
            EdmBooleanConstant constant = Assert.IsType<EdmBooleanConstant>(exp);
            Assert.True(constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForBinaryValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "01");

            // Assert
            Assert.NotNull(exp);
            EdmBinaryConstant constant = Assert.IsType<EdmBinaryConstant>(exp);
            byte[] result = constant.Value;
            Assert.Single(result);
            Assert.Equal(1, result[0]);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void BuildEdmExpression_Works_ForBooleanValue(string value, bool expected)
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, value);

            // Assert
            Assert.NotNull(exp);
            EdmBooleanConstant constant = Assert.IsType<EdmBooleanConstant>(exp);
            Assert.Equal(expected, constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForDateValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Date);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "2000-12-10");

            // Assert
            Assert.NotNull(exp);
            EdmDateConstant constant = Assert.IsType<EdmDateConstant>(exp);
            Assert.Equal(new Date(2000, 12, 10), constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForDateTimeOffsetValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "2012-07-28T13:22:16.123-07:15");

            // Assert
            Assert.NotNull(exp);
            EdmDateTimeOffsetConstant constant = Assert.IsType<EdmDateTimeOffsetConstant>(exp);
            Assert.Equal(new DateTimeOffset(2012, 7, 28, 13, 22, 16, 123, new TimeSpan(-7, -15, 0)), constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForDecimalValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "3.14");

            // Assert
            Assert.NotNull(exp);
            EdmDecimalConstant constant = Assert.IsType<EdmDecimalConstant>(exp);
            Assert.Equal((decimal)3.14, constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForDurationValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "P11DT23H59M59.999999999999S");

            // Assert
            Assert.NotNull(exp);
            EdmDurationConstant constant = Assert.IsType<EdmDurationConstant>(exp);
            Assert.Equal("11.23:59:59.9999999", constant.Value.ToString());
        }

        [Fact]
        public void BuildEdmExpression_Works_ForSingleValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "0.2");

            // Assert
            Assert.NotNull(exp);
            EdmFloatingConstant constant = Assert.IsType<EdmFloatingConstant>(exp);
            Assert.Equal(0.2, constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForDoubleValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "3.94");

            // Assert
            Assert.NotNull(exp);
            EdmFloatingConstant constant = Assert.IsType<EdmFloatingConstant>(exp);
            Assert.Equal(3.94, constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForGuidValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "21EC2020-3AEA-1069-A2DD-08002B30309D");

            // Assert
            Assert.NotNull(exp);
            EdmGuidConstant constant = Assert.IsType<EdmGuidConstant>(exp);
            Assert.Equal("21EC2020-3AEA-1069-A2DD-08002B30309D".ToLowerInvariant(), constant.Value.ToString());
        }

        [Fact]
        public void BuildEdmExpression_Works_ForIntValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "3");

            // Assert
            Assert.NotNull(exp);
            EdmIntegerConstant constant = Assert.IsType<EdmIntegerConstant>(exp);
            Assert.Equal(3, constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForLongValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, long.MaxValue.ToString());

            // Assert
            Assert.NotNull(exp);
            EdmIntegerConstant constant = Assert.IsType<EdmIntegerConstant>(exp);
            Assert.Equal(long.MaxValue, constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForTimeOfDayValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "21:45:00");

            // Assert
            Assert.NotNull(exp);
            EdmTimeOfDayConstant constant = Assert.IsType<EdmTimeOfDayConstant>(exp);
            Assert.Equal(new TimeOfDay(21, 45, 0, 0), constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForStringValue()
        {
            // Arrange
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(edmType, "This is a test");

            // Assert
            Assert.NotNull(exp);
            EdmStringConstant constant = Assert.IsType<EdmStringConstant>(exp);
            Assert.Equal("This is a test", constant.Value);
        }

        [Theory]
        [InlineData(EdmPrimitiveTypeKind.Binary, "The value 'abc' is not a valid binary value. The value must be a hexadecimal string and must not be prefixed by '0x'.")]
        [InlineData(EdmPrimitiveTypeKind.Boolean, "The value 'abc' is not a valid boolean. The value must be 'true' or 'false'.")]
        [InlineData(EdmPrimitiveTypeKind.Date, "The value 'abc' is not a valid date value.")]
        [InlineData(EdmPrimitiveTypeKind.DateTimeOffset, "The value 'abc' is not a valid date time offset value.")]
        [InlineData(EdmPrimitiveTypeKind.Decimal, "The value 'abc' is not a valid decimal.")]
        [InlineData(EdmPrimitiveTypeKind.Duration, "The value 'abc' is not a valid duration value.")]
        [InlineData(EdmPrimitiveTypeKind.Single, "The value 'abc' is not a valid floating point value.")]
        [InlineData(EdmPrimitiveTypeKind.Double, "The value 'abc' is not a valid floating point value.")]
        [InlineData(EdmPrimitiveTypeKind.Guid, "The value 'abc' is not a valid Guid.")]
        [InlineData(EdmPrimitiveTypeKind.Int16, "The value 'abc' is not a valid integer. The value must be a valid 32 bit integer.")]
        [InlineData(EdmPrimitiveTypeKind.TimeOfDay, "The value 'abc' is not a valid TimeOfDay value.")]
        public void BuildEdmExpression_Throws_ForInvalidDefaultValue(EdmPrimitiveTypeKind kind, string expected)
        {
            // Arrange
            IEdmType primitiveType = EdmCoreModel.Instance.GetPrimitiveType(kind);

            // Act
            Action test = () => IEdmTermExtensions.BuildEdmExpression(primitiveType, "abc");

            // Assert
            FormatException exception = Assert.Throws<FormatException>(test);
            Assert.Equal(expected, exception.Message);
        }

        [Fact]
        public void BuildEdmExpression_ThrowsNotSupported_ForNotSupportedValueType()
        {
            // Arrange
            IEdmType primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream);

            // Act
            Action test = () => IEdmTermExtensions.BuildEdmExpression(primitiveType, "abc");

            // Assert
            NotSupportedException exception = Assert.Throws<NotSupportedException>(test);
            Assert.Equal("Term type 'Edm.Stream' is not supported for value retrieval.", exception.Message);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForTypeDefinitionValue()
        {
            // Arrange
            EdmTypeDefinition definition = new EdmTypeDefinition("NS", "MyTypeDefinition", EdmPrimitiveTypeKind.Int32);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(definition, "42");

            // Assert
            Assert.NotNull(exp);
            EdmIntegerConstant constant = Assert.IsType<EdmIntegerConstant>(exp);
            Assert.Equal(42, constant.Value);
        }

        [Fact]
        public void BuildEdmExpression_Works_ForPropertyPathValue()
        {
            // Arrange
            IEdmPathType pathType = EdmCoreModel.Instance.GetPathType(EdmPathTypeKind.PropertyPath);

            // Act
            IEdmExpression exp = IEdmTermExtensions.BuildEdmExpression(pathType, "HomeAddress/City");

            // Assert
            Assert.NotNull(exp);
            EdmPropertyPathExpression constant = Assert.IsType<EdmPropertyPathExpression>(exp);
            Assert.Equal("HomeAddress/City", constant.Path);
        }
    }
}
