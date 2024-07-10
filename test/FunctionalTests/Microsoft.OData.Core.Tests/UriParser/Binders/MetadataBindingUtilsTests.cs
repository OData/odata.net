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
        public void IfTypePromotionNeeded_SourceIsIntegralMemberValueAndTargetIsEnum_ConstantNodeIsCreated()
        {
            // Arrange
            var enumValue = 3;

            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference);

            // Assert
            var member = WeekDayEmumType.Members.First(m => m.Value.Value == enumValue);
            result.ShouldBeEnumNode(WeekDayEmumType, member.Name);
        }

        [Fact]
        public void IfTypePromotionNeededForEnum_SourceIsIntegralMemberValueInStringAndTargetIsEnumType_ConstantNodeIsCreated()
        {
            // Arrange
            var enumValue = "5";
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference);

            // Assert
            var member = WeekDayEmumType.Members.First(m => m.Value.Value == int.Parse(enumValue));
            result.ShouldBeEnumNode(WeekDayEmumType, member.Name);
        }

        [Fact]
        public void IfTypePromotionNeededForEnum_SourceIsMemberName_ConstantNodeIsCreated()
        {
            // Arrange
            var enumValue = "Monday";
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference);

            // Assert
            result.ShouldBeEnumNode(WeekDayEmumType, enumValue);
        }

        [Fact]
        public void IfTypePromotionNeededForEnum_SourceIsIntegerExceedingDefinedIntegralLimits_ErrorIsThrown()
        {
            // Arrange
            var enumValue = 10;
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(WeekDayEmumType, false);

            // Act
            Action convertIfNeeded = () => MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference);

            // Assert
            convertIfNeeded.Throws<ODataException>(ODataErrorStrings.Binder_IsNotValidEnumConstant(enumValue.ToString()));
        }

        private static EdmEnumType WeekDayEmumType
        {
            get
            {
                var weekDayType = new EdmEnumType("NS", "WeekDay");
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
        #endregion
    }
}
