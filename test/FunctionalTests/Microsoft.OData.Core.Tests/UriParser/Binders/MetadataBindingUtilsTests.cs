//---------------------------------------------------------------------
// <copyright file="MetadataBindingUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;
using System.Linq;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit and Short-Span Integration tests for MetadataBindingUtils methods.
    /// </summary>
    public class MetadataBindingUtilsTests
    {
        #region MetadataBindingUtils.ConvertToType Tests
        [Fact]
        public void IfTypePromotionNeededConvertNodeIsCreatedAndSourcePropertySet()
        {
            SingleValueNode node = new ConstantNode(7);
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(node, EdmCoreModel.Instance.GetDouble(false));
            result.ShouldBeConstantQueryNode(7d);
        }

        [Fact]
        public void IfTypeReferenceIsNullNoConvertIsInjected()
        {
            SingleValueNode node = new ConstantNode(7);
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(node, null);
            result.ShouldBeConstantQueryNode(7);
        }

        [Fact]
        public void IfTypesMatchIsNullNoConvertIsInjected()
        {
            SingleValueNode node = new ConstantNode(7);
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(node, EdmCoreModel.Instance.GetInt32(false));
            result.ShouldBeConstantQueryNode(7);
        }

        [Fact]
        public void IfTypesCannotPromoteErrorIsThrown()
        {
            SingleValueNode node = new ConstantNode(7);
            var targetType = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, false);
            Action convertMethod = () => MetadataBindingUtils.ConvertToTypeIfNeeded(node, targetType);
            convertMethod.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CannotConvertToType(node.TypeReference.FullName(), targetType.FullName()));
        }

        [Fact]
        public void IfTypePromotionNeeded_SourceIsIntegerMemberValueAndTargetIsEnum_ConstantNodeIsCreated()
        {
            // Arrange
            int enumValue = 3;
            bool success = WeekDayEmumType.TryParse(enumValue, out IEdmEnumMember expectedMember);

            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            Assert.True(success);
            result.ShouldBeEnumNode(WeekDayEmumType, expectedMember.Name);
            Assert.Equal(expectedMember.Name, result.Value.ToString());
        }

        [Fact]
        public void IfTypePromotionNeeded_SourceIsLongMemberValueAndTargetIsEnum_ConstantNodeIsCreated()
        {
            // Arrange
            long enumValue = 7L;
            bool success = WeekDayEmumType.TryParse(enumValue, out IEdmEnumMember expectedMember);

            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            Assert.True(success);
            result.ShouldBeEnumNode(WeekDayEmumType, expectedMember.Name);
            Assert.Equal(expectedMember.Name, result.Value.ToString()); // Compare the enum member name
        }

        [Fact]
        public void IfTypePromotionNeeded_SourceIsHugeLongMemberValueAndTargetIsEnum_ConstantNodeIsCreated()
        {
            // Arrange
            long enumValue = 2147483657; // ((long)int.MaxValue + 10L).ToString()
            bool success = EmployeeType.TryParse(enumValue, out IEdmEnumMember expectedMember);

            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(EmployeeType, false);

            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            Assert.True(success);
            result.ShouldBeEnumNode(EmployeeType, expectedMember.Name);
            Assert.Equal(expectedMember.Name, result.Value.ToString()); // Compare the enum member name
        }

        [Fact]
        public void IfTypePromotionNeeded_SourceIsLongAsStringMemberValueAndTargetIsEnum_ConstantNodeIsCreated()
        {
            // Arrange
            string enumValue = "4294967294"; // ((long)int.MaxValue + (long)int.MaxValue).ToString();
            bool success = EmployeeType.TryParse(long.Parse(enumValue), out IEdmEnumMember expectedMember);

            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(EmployeeType, false);

            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            Assert.True(success);
            result.ShouldBeEnumNode(EmployeeType, expectedMember.Name);
            Assert.Equal(expectedMember.Name, result.Value.ToString()); // Compare the enum member name
        }

        [Fact]
        public void IfTypePromotionNeededForEnum_SourceIsIntegralMemberValueInStringAndTargetIsEnumType_ConstantNodeIsCreated()
        {
            // Arrange
            string enumValue = "5";
            bool success = WeekDayEmumType.TryParse(long.Parse(enumValue), out IEdmEnumMember expectedMember);

            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            Assert.True(success);
            result.ShouldBeEnumNode(WeekDayEmumType, expectedMember.Name);
            Assert.Equal(expectedMember.Name, result.Value.ToString()); // compare the enum member name
        }

        [Fact]
        public void IfTypePromotionNeededForEnum_SourceIsMemberName_ConstantNodeIsCreated()
        {
            // Arrange
            string enumValue = "Monday";
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);
            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            result.ShouldBeEnumNode(WeekDayEmumType, enumValue);
            Assert.Equal(enumValue, result.Value.ToString());
        }

        [Fact]
        public void IfTypePromotionNeededForEnum_SourceIsIntegerExceedingDefinedIntegralLimits_ValueIsNotValidEnumConstantExceptionIsThrown()
        {
            // Arrange
            int enumValue = 10;
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            Action convertIfNeeded = () => MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference);

            // Assert
            convertIfNeeded.Throws<ODataException>(ODataErrorStrings.Binder_IsNotValidEnumConstant(enumValue.ToString()));
        }

        [Fact]
        public void IfTypePromotionNeeded_SourceIsFloatMemberValuesAndTargetIsEnum_CannotConvertToTypeExceptionIsThrown()
        {
            // Arrange
            float[] floatValues = new float[] { 1.0F, 3.3F, 5.0F, 6.0F };

            foreach (float enumValue in floatValues)
            {
                SingleValueNode source = new ConstantNode(enumValue);
                IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

                // Act
                Action convertIfNeeded = () => MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference);

                // Assert
                convertIfNeeded.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CannotConvertToType(source.TypeReference.FullName(), targetTypeReference.FullName()));
            }
        }

        [Fact]
        public void IfTypePromotionNeeded_SourceIsFloatMemberValuesInStringAndTargetIsEnum_ValueIsNotValidEnumConstantExceptionIsThrown()
        {
            // Arrange
            string[] floatValues = new string[] { "1.0", "3.1", "5.5", "7.0" };

            foreach (string enumValue in floatValues)
            {
                SingleValueNode source = new ConstantNode(enumValue);
                IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

                // Act
                Action convertIfNeeded = () => MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference);

                // Assert
                convertIfNeeded.Throws<ODataException>(ODataErrorStrings.Binder_IsNotValidEnumConstant(enumValue));
            }
        }

        private static EdmEnumType WeekDayEmumType
        {
            get
            {
                EdmEnumType weekDayType = new EdmEnumType("NS", "WeekDay");
                weekDayType.AddMember("Monday", new EdmEnumMemberValue(1L));
                weekDayType.AddMember("Tuesday", new EdmEnumMemberValue(2L));
                weekDayType.AddMember("Wednesday", new EdmEnumMemberValue(3L));
                weekDayType.AddMember("Thursday", new EdmEnumMemberValue(4L));
                weekDayType.AddMember("Friday", new EdmEnumMemberValue(5L));
                weekDayType.AddMember("Saturday", new EdmEnumMemberValue(6L));
                weekDayType.AddMember("Sunday", new EdmEnumMemberValue(7L));

                return weekDayType;
            }
        }

        private static EdmEnumType EmployeeType
        {
            get
            {
                EdmEnumType employeeType = new EdmEnumType("NS", "EmployeeType");
                employeeType.AddMember("FullTime", new EdmEnumMemberValue((long)int.MaxValue));
                employeeType.AddMember("PartTime", new EdmEnumMemberValue((long)int.MaxValue + 10L));
                employeeType.AddMember("Contractor", new EdmEnumMemberValue((long)int.MaxValue + (long)int.MaxValue));

                return employeeType;
            }
        }
        #endregion
    }
}
