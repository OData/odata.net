//---------------------------------------------------------------------
// <copyright file="MetadataBindingUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit and Short-Span Integration tests for MetadataBindingUtils methods.
    /// </summary>
    public class MetadataBindingUtilsTests
    {
        #region MetadataBindingUtils.ConvertToType Tests

        private static MethodInfo PrivateBuildCollectionLiteral =>
            typeof(MetadataBindingUtils).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(m => m.Name == "BuildCollectionLiteral");

        private static MethodInfo PrivateConvertNodes =>
            typeof(MetadataBindingUtils).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(
                m => m.Name == "ConvertNodes"
                && m.GetParameters().Length == 2
                && m.GetParameters()[0].ParameterType == typeof(IList<ConstantNode>));

        private static IEdmCollectionTypeReference CollectionOf(IEdmTypeReference element) =>
            new EdmCollectionTypeReference(new EdmCollectionType(element));

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
            convertMethod.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_CannotConvertToType, node.TypeReference.FullName(), targetType.FullName()));
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
            convertIfNeeded.Throws<ODataException>(Error.Format(SRResources.Binder_IsNotValidEnumConstant, enumValue.ToString()));
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
                convertIfNeeded.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.TypeReference.FullName(), targetTypeReference.FullName()));
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
                convertIfNeeded.Throws<ODataException>(Error.Format(SRResources.Binder_IsNotValidEnumConstant, enumValue));
            }
        }

        [Fact]
        public void ConvertCollection_TargetNull_ReturnsSource()
        {
            // Arrange
            var elem = EdmCoreModel.Instance.GetInt32(false);
            var sourceType = CollectionOf(elem);
            var source = CreateCollection(new[] { new ConstantNode(1, "1", elem) }, sourceType);
            
            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, null);

            // Assert
            Assert.Same(source, result);
        }

        [Fact]
        public void ConvertCollection_TargetNotCollection_Throws()
        {
            // Arrange
            var elem = EdmCoreModel.Instance.GetInt32(false);
            var sourceType = CollectionOf(elem);
            var source = CreateCollection(new[] { new ConstantNode(1, "1", elem) }, sourceType);

            // Non-collection primitive target
            var nonCollectionTarget = EdmCoreModel.Instance.GetInt64(false);

            // Act
            var ex = Assert.Throws<ODataException>(() => MetadataBindingUtils.ConvertToTypeIfNeeded(source, nonCollectionTarget));

            // Assert
            Assert.Contains(source.CollectionType.FullName(), ex.Message, StringComparison.Ordinal);
            Assert.Contains(nonCollectionTarget.FullName(), ex.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void ConvertCollection_EquivalentTypes_PrimitiveElements_ReturnsSourceUnchanged()
        {
            // Arrange
            var elem = EdmCoreModel.Instance.GetString(true);
            var type = CollectionOf(elem);
            var source = CreateCollection(new[] { new ConstantNode("a", "'a'", elem) }, type);
            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, type);

            // Assert
            Assert.Same(source, result);
        }

        [Fact]
        public void ConvertCollection_EquivalentTypes_TypeDefinitionElements_ConvertsUnderlying()
        {
            // Arrange
            // Define a type definition over Int32
            var intDef = new EdmTypeDefinition("NS", "MyIntDef", EdmPrimitiveTypeKind.Int32);
            var sourceElemDef = new EdmTypeDefinitionReference(intDef, false);
            var targetElem = EdmCoreModel.Instance.GetInt32(false);

            var sourceType = CollectionOf(sourceElemDef);
            var targetType = CollectionOf(targetElem);

            Assert.True(sourceType.IsEquivalentTo(targetType)); // Equivalent collection types

            var source = CreateCollection(new[]
            {
                new ConstantNode(10, "10", sourceElemDef),
                new ConstantNode(20, "20", sourceElemDef)
            }, sourceType);

            // Act
            var resultNode = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetType);

            // Assert
            Assert.NotSame(source, resultNode); // New collection produced due to definition conversion

            var collectionConstant = Assert.IsType<CollectionConstantNode>(resultNode);
            Assert.Equal(2, collectionConstant.Collection.Count);
            Assert.All(collectionConstant.Collection, c =>
            {
                Assert.Equal(EdmPrimitiveTypeKind.Int32, c.TypeReference.AsPrimitive().PrimitiveKind());
            });

            // Literal rebuilt via BuildCollectionLiteral
            Assert.Equal("(10,20)", collectionConstant.LiteralText);
        }

        [Fact]
        public void ConvertCollection_NonEquivalentConvertible_PrimitiveElements_ProducesNewCollection()
        {
            // Arrange
            var sourceElem = EdmCoreModel.Instance.GetInt32(true);
            var targetElem = EdmCoreModel.Instance.GetDecimal(false);

            var sourceType = CollectionOf(sourceElem);
            var targetType = CollectionOf(targetElem);

            var source = CreateCollection(new[]
            {
                new ConstantNode(1, "1", sourceElem),
                new ConstantNode(2, "2", sourceElem)
            }, sourceType);

            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetType);

            // Assert
            Assert.NotSame(source, result);

            var converted = Assert.IsType<CollectionConstantNode>(result);
            Assert.Equal(2, converted.Collection.Count);
            Assert.All(converted.Collection, c =>
            {
                Assert.Equal(EdmPrimitiveTypeKind.Decimal, c.TypeReference.AsPrimitive().PrimitiveKind());
            });
            Assert.Equal("(1,2)", converted.LiteralText);
        }

        [Fact]
        public void ConvertCollection_ItemProducesConvertNode_MaterializedAsConstant()
        {
            // Arrange
            var sourceElem = EdmCoreModel.Instance.GetInt32(false);
            var targetElem = EdmCoreModel.Instance.GetDecimal(false);
            var sourceType = CollectionOf(sourceElem);
            var targetType = CollectionOf(targetElem);

            var item = new ConstantNode(123, "123", sourceElem);
            var source = CreateCollection(new[] { item }, sourceType);

            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetType);

            // Assert
            var converted = Assert.IsType<CollectionConstantNode>(result);
            var convertedItem = converted.Collection.Single();

            Assert.Equal(123m, convertedItem.Value);
            Assert.Equal(EdmPrimitiveTypeKind.Decimal, convertedItem.TypeReference.AsPrimitive().PrimitiveKind());
            Assert.Equal("(123)", converted.LiteralText);
        }

        [Fact]
        public void ConvertCollection_NonConvertibleElementTypes_Throws()
        {
            // Arrange
            var sourceElem = EdmCoreModel.Instance.GetString(false);
            var targetElem = EdmCoreModel.Instance.GetDate(false); // string -> date not directly convertible in promotion rules
            var sourceType = CollectionOf(sourceElem);
            var targetType = CollectionOf(targetElem);

            var source = CreateCollection(new[] { new ConstantNode("2020-01-01", "'2020-01-01'", sourceElem) }, sourceType);

            // Act
            var ex = Assert.Throws<ODataException>(() => MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetType));

            // Assert
            Assert.Contains(sourceElem.FullName(), ex.Message, StringComparison.Ordinal);
            Assert.Contains(targetElem.FullName(), ex.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void ConvertNodes_PreservesNullAndCoercesTypes()
        {
            // Arrange
            var sourceElem = EdmCoreModel.Instance.GetInt32(true);
            var targetElem = EdmCoreModel.Instance.GetInt64(false);

            var list = new List<ConstantNode>
            {
                null, // should become ConstantNode(null,"null",targetElem)
                new ConstantNode(9, "9", sourceElem)
            };

            // Act
            var converted = (List<ConstantNode>)PrivateConvertNodes.Invoke(
                null,
                new object[] { list, targetElem });


            // Assert
            Assert.Equal(2, converted.Count);
            Assert.Null(converted[0].Value);
            Assert.Equal("null", converted[0].LiteralText);
            Assert.Equal(EdmPrimitiveTypeKind.Int64, converted[1].TypeReference.AsPrimitive().PrimitiveKind());
        }

        [Fact]
        public void BuildCollectionLiteral_QuotesStringsAndEscapes()
        {
            // Arrange
            var stringType = EdmCoreModel.Instance.GetString(false);
            var nodes = new List<ConstantNode>
            {
                new ConstantNode("abc", "abc", stringType),                // needs quoting
                new ConstantNode("O'Malley", "O'Malley", stringType),      // needs quote + escape
                new ConstantNode("'alreadyQuoted'", "'alreadyQuoted'", stringType), // already quoted
                new ConstantNode(null, "null", stringType),                  // null value -> null literal
            };

            // Act
            string literal = (string)PrivateBuildCollectionLiteral.Invoke(null, new object[] { nodes, stringType });

            // Assert
            Assert.Equal("('abc','O''Malley','alreadyQuoted',null)", literal);
        }

        [Fact]
        public void BuildCollectionLiteral_NonString_NoQuoting()
        {
            // Arrange
            var intType = EdmCoreModel.Instance.GetInt32(true);
            var nodes = new List<ConstantNode>
            {
                new ConstantNode(1, "1", intType),
                new ConstantNode(2, "2", intType),
                new ConstantNode(null, "null", intType),
            };

            // Act
            string literal = (string)PrivateBuildCollectionLiteral.Invoke(null, new object[] { nodes, intType });

            // Assert
            Assert.Equal("(1,2,null)", literal);
        }

        [Fact]
        public void ConvertCollection_TypeDefinitionWithMixedItems_ProducesEscapedLiteral()
        {
            // Arrange
            var def = new EdmTypeDefinition("NS", "MyStringDef", EdmPrimitiveTypeKind.String);
            var sourceElemDef = new EdmTypeDefinitionReference(def, true);
            var targetElem = EdmCoreModel.Instance.GetString(true);

            var sourceType = CollectionOf(sourceElemDef);
            var targetType = CollectionOf(targetElem);

            var source = CreateCollection(new[]
            {
                new ConstantNode("plain", "plain", sourceElemDef),
                new ConstantNode("O'Hara", "O'Hara", sourceElemDef),
                new ConstantNode("'quotedAlready'", "'quotedAlready'", sourceElemDef),
                new ConstantNode(null, "null", sourceElemDef)
            }, sourceType);

            // Act
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetType);

            // Assert
            var converted = Assert.IsType<CollectionConstantNode>(result);
            Assert.Equal("('plain','O''Hara','quotedAlready',null)", converted.LiteralText);
            Assert.All(converted.Collection, n =>
            {
                if (n.Value != null)
                {
                    Assert.Equal(EdmPrimitiveTypeKind.String, n.TypeReference.AsPrimitive().PrimitiveKind());
                }
            });
        }

        [Theory]
        [InlineData("FullTime")]
        [InlineData("PartTime")]
        [InlineData("Contractor")]
        [InlineData("Temporary")]
        [InlineData("FullTime,PartTime")]
        [InlineData("FullTime,Temporary")]
        [InlineData("Permanent,Temporary")]
        [InlineData("Intern,Temporary")]
        [InlineData("Permanent")]
        [InlineData("PartTime,Contractor,Temporary")]
        public void IfTypePromotionNeeded_SourceIsFlagsCompositeMemberNameOrComposite_ConstantNodeIsCreated(string enumValue)
        {
            // Arrange
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(EmployeeTypeWithFlags, false);
            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            result.ShouldBeEnumNode(EmployeeTypeWithFlags, enumValue);
            Assert.Equal(enumValue, result.Value.ToString());
        }

        [Theory]
        [InlineData("fulltime", "FullTime")]
        [InlineData("FULLTIME", "FullTime")]
        [InlineData("parttime", "PartTime")]
        [InlineData("PARTTIME", "PartTime")]
        [InlineData("contractor", "Contractor")]
        [InlineData("temporary", "Temporary")]
        [InlineData("fulltime, parttime", "FullTime,PartTime")]
        [InlineData("fulltime, temporary", "FullTime,Temporary")]
        [InlineData("permanent, temporary", "Permanent,Temporary")]
        [InlineData("intern, temporary", "Intern,Temporary")]
        [InlineData("permanent", "Permanent")]
        [InlineData("parttime, contractor, temporary", "PartTime,Contractor,Temporary")]
        [InlineData("PARTTIME, contractor, Temporary", "PartTime,Contractor,Temporary")]
        public void IfTypePromotionNeeded_SourceIsFlagsCompositeMemberNameOrComposite_enableCaseInsensitive_ConstantNodeIsCreated(string enumValue, string expected)
        {
            // Arrange
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(EmployeeTypeWithFlags, false);

            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference, true) as ConstantNode;

            // Assert
            result.ShouldBeEnumNode(EmployeeTypeWithFlags, expected);
            Assert.Equal(expected, result.Value.ToString());
        }

        [Theory]
        [InlineData(2, "FullTime")]
        [InlineData("2", "FullTime")]
        [InlineData(6, "Permanent")]
        [InlineData("6", "Permanent")]
        [InlineData(8, "Contractor")]
        [InlineData("8", "Contractor")]
        [InlineData(28, "Temporary")]
        [InlineData("28", "Temporary")]
        [InlineData(26, "FullTime,Contractor,Intern")]
        [InlineData("26", "FullTime,Contractor,Intern")]
        [InlineData(30, "FullTime,Temporary")]
        [InlineData("22", "Permanent,Intern")]
        [InlineData(22, "Permanent,Intern")]
        public void IfTypePromotionNeeded_SourceIsFlagsIntegralValues_ConstantNodeIsCreated(object enumValue, string expectedLiteralValue)
        {
            // Arrange
            SingleValueNode source = new ConstantNode(enumValue);
            IEdmTypeReference targetTypeReference = new EdmEnumTypeReference(EmployeeTypeWithFlags, false);
            // Act
            ConstantNode result = MetadataBindingUtils.ConvertToTypeIfNeeded(source, targetTypeReference) as ConstantNode;

            // Assert
            result.ShouldBeEnumNode(EmployeeTypeWithFlags, expectedLiteralValue);
            Assert.Equal(expectedLiteralValue, result.Value.ToString());
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

        private static CollectionConstantNode CreateCollection(IEnumerable<ConstantNode> items, IEdmCollectionTypeReference typeRef, string literal = null)
        {
            var list = items?.ToList() ?? new List<ConstantNode>();
            literal ??= "[" + string.Join(",", list.Select(n => n?.LiteralText ?? (n?.Value == null ? "null" : ODataUriUtils.ConvertToUriLiteral(n.Value, ODataVersion.V4)))) + "]";
            return new CollectionConstantNode(list, literal, typeRef);
        }

        private static EdmEnumType EmployeeTypeWithFlags
        {
            get
            {
                EdmEnumType employeeType = new EdmEnumType("NS", "EmployeeTypeWithFlags", isFlags: true);
                employeeType.AddMember("FullTime", new EdmEnumMemberValue((long)2));
                employeeType.AddMember("PartTime", new EdmEnumMemberValue((long)4));
                employeeType.AddMember("Contractor", new EdmEnumMemberValue((long)8));
                employeeType.AddMember("Intern", new EdmEnumMemberValue((long)16));
                employeeType.AddMember("Permanent", new EdmEnumMemberValue((long)(2 | 4))); // FullTime | PartTime = 6
                employeeType.AddMember("Temporary", new EdmEnumMemberValue((long) (4 | 8 | 16))); // PartTime | Contractor | Intern = 28

                return employeeType;
            }
        }

        #endregion
    }
}
