//---------------------------------------------------------------------
// <copyright file="EnumFilterFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    public class EnumFilterFunctionalTests
    {
        private readonly EdmModel userModel;
        private readonly EdmEntitySet entitySet;
        private readonly EdmEntityType entityType;

        public enum Color
        {
            Red = 1,
            Green = 2,
            Blue = 3,
            White = -10
        }

        [Flags]
        public enum ColorFlags
        {
            Red = 1,
            Green = 2,
            Blue = 4,
            GreenRed = Red | Green
        }

        public EnumFilterFunctionalTests()
        {
            // set up the edm model etc
            this.userModel = new EdmModel();

            var enumType = new EdmEnumType("NS", "Color", EdmPrimitiveTypeKind.Int32, false);
            var red = new EdmEnumMember(enumType, "Red", new EdmEnumMemberValue(1));
            enumType.AddMember(red);
            enumType.AddMember("Green", new EdmEnumMemberValue(2));
            enumType.AddMember("Blue", new EdmEnumMemberValue(3));
            enumType.AddMember("White", new EdmEnumMemberValue(-10));

            // add to model
            this.userModel.AddElement(enumType);

            // add enum property
            this.entityType = new EdmEntityType("NS", "MyEntityType", isAbstract: false, isOpen: false, baseType: null);
            var enumTypeReference = new EdmEnumTypeReference(enumType, true);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "Color", enumTypeReference));

            // enum with flags
            var enumFlagsType = new EdmEnumType("NS", "ColorFlags", EdmPrimitiveTypeKind.Int64, true);
            enumFlagsType.AddMember("Red", new EdmEnumMemberValue(1L));
            enumFlagsType.AddMember("Green", new EdmEnumMemberValue(2L));
            enumFlagsType.AddMember("Blue", new EdmEnumMemberValue(4L));
            enumFlagsType.AddMember("GreenRed", new EdmEnumMemberValue(3L));

            // add to model
            this.userModel.AddElement(enumFlagsType);

            // add enum with flags
            var enumFlagsTypeReference = new EdmEnumTypeReference(enumFlagsType, true);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "ColorFlags", enumFlagsTypeReference));

            // add colors collection
            var colorTypeReference = new EdmEnumTypeReference(enumType, false);
            this.entityType.AddStructuralProperty("Colors", new EdmCollectionTypeReference(new EdmCollectionType(colorTypeReference)));

            this.userModel.AddElement(this.entityType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.entitySet = new EdmEntitySet(defaultContainer, "MySet", this.entityType);
            defaultContainer.AddElement(this.entitySet);
        }

        [Fact]
        public void ParseFilterWithEnum()
        {
            var filterQueryNode = ParseFilter(
                "Color eq NS.Color'Green'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("Color"));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetIEdmType<IEdmEnumType>("NS.Color"), (int)Color.Green);
        }

        [Fact]
        public void ParseFilterWithEnumInt()
        {
            var filterQueryNode = ParseFilter(
                "Color eq NS.Color'2'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("Color"));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetIEdmType<IEdmEnumType>("NS.Color"), (int)Color.Green);
        }

        [Fact]
        public void ParseFilterWithHasOperatorEnumMemberName()
        {
            var filterQueryNode = ParseFilter(
                "ColorFlags has NS.ColorFlags'Green'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"), (int)ColorFlags.Green);
        }

        [Fact]
        public void ParseFilterWithHasOperatorEnumUnderlyingValue()
        {
            var filterQueryNode = ParseFilter(
                "ColorFlags has NS.ColorFlags'2'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"), (int)ColorFlags.Green);
        }

        [Fact]
        public void ParseFilterWithHasOperatorEnumLiteralValueAsLeftOperand()
        {
            var filterQueryNode = ParseFilter(
                "NS.ColorFlags'GreenRed' has ColorFlags",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
            .Left
            .ShouldBeEnumNode(this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"), (int)(ColorFlags.Green | ColorFlags.Red));

            binaryNode
            .Right
            .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));
        }

        [Fact]
        public void ParseFilterWithHasOperatorNonFlagsEnum()
        {
            var filterQueryNode = ParseFilter(
                "Color has NS.Color'Green'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("Color"));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetIEdmType<IEdmEnumType>("NS.Color"), (int)Color.Green);
        }

        [Fact]
        public void ParseFilterWithEnumNormalConbinedValues()
        {
            var filterQueryNode = ParseFilter(
                "ColorFlags has NS.ColorFlags'Green, Red'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"),
                (int)(ColorFlags.Green | ColorFlags.Red));
        }

        [Fact]
        public void ParseFilterWithEnumCombinedValuesOrderReversed()
        {
            var filterQueryNode = ParseFilter(
                "ColorFlags has NS.ColorFlags'Red,Green'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"),
                (int)(ColorFlags.Green | ColorFlags.Red));
        }

        [Fact]
        public void ParseFilterWithEnumValuesCompatibleWithString()
        {
            var filterQueryNode = ParseFilter(
                "ColorFlags has 'Red'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));

            binaryNode
                 .Right
                 .ShouldBeEnumNode(
                 this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"),
                 "Red");
        }

        [Fact]
        public void ParseFilterWithEnumDefinedConbinedValues()
        {
            var filterQueryNode = ParseFilter(
                "ColorFlags has NS.ColorFlags'GreenRed'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"),
                (int)(ColorFlags.Green | ColorFlags.Red));
        }

        [Fact]
        public void ParseFilterWithEnumCombinedUnderlyingValues()
        {
            var filterQueryNode = ParseFilter(
                "ColorFlags has NS.ColorFlags'1,2'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("ColorFlags"));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetIEdmType<IEdmEnumType>("NS.ColorFlags"),
                (int)(ColorFlags.Green | ColorFlags.Red));
        }

        [Fact]
        public void ParseFilterWithEnumNegativeMember()
        {
            var filterQueryNode = ParseFilter(
                "Color eq NS.Color'-10'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("Color"));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetIEdmType<IEdmEnumType>("NS.Color"),
                (int)(Color.White));

            var constantNode = Assert.IsType<ConstantNode>(binaryNode.Right);
            Assert.True(constantNode.TypeReference.IsEnum());
        }

        [Fact]
        public void ParseFilterWithEnumUndefinedMember()
        {
            var filterQueryNode = ParseFilter(
                "Color eq NS.Color'-132534290' or Color eq NS.Color'6536231'",
                this.userModel,
                this.entityType,
                this.entitySet);

            var binaryNode = filterQueryNode
                .Expression
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Or)
                .Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("Color"));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetIEdmType<IEdmEnumType>("NS.Color"),
                -132534290);

            var constantNode = Assert.IsType<ConstantNode>(binaryNode.Right);
            Assert.True(constantNode.TypeReference.IsEnum());
        }

        [Fact]
        public void ParseFilterWithEmptyEnumValue()
        {
            Action parse = () => ParseFilter("Color has NS.Color''", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant("NS.Color''"));
        }

        [Fact]
        public void ParseFilterWithNullEnumValue()
        {
            var filterQueryNode = ParseFilter("Color eq null", this.userModel, this.entityType, this.entitySet);
            var binaryNode = filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(this.GetIEdmProperty("Color"));

            var convertNode = Assert.IsType<ConvertNode>(binaryNode.Right);
            convertNode.Source.ShouldBeConstantQueryNode((object)null);
        }

        [Fact]
        public void ParseFilterCastMethod1()
        {
            var filter = ParseFilter("cast(NS.Color'Green', 'Edm.String') eq 'blue'", this.userModel, this.entityType, this.entitySet);
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var convertNode = Assert.IsType<ConvertNode>(bon.Left);
            var functionCallNode = Assert.IsType<SingleValueFunctionCallNode>(convertNode.Source);
            Assert.Equal("cast", functionCallNode.Name); // ConvertNode is because cast() result's nullable=false.
        }

        [Fact]
        public void ParseFilterCastMethod2()
        {
            var filter = ParseFilter("cast('Green', 'NS.Color') eq NS.Color'Green'", this.userModel, this.entityType, this.entitySet);
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var functionCallNode = Assert.IsType<SingleValueFunctionCallNode>(bon.Left);
            Assert.Equal("cast", functionCallNode.Name);
        }

        //to do: verify the exceptions for the Mismatch cases.
        [Fact]
        public void ParseFilterEnumTypesMismatch1()
        {
            Action parse = () => ParseFilter("Color'Green' eq NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            Assert.Throws<ODataException>(parse);
        }

        [Fact]
        public void ParseFilterEnumTypesMismatch2()
        {
            Action parse = () => ParseFilter("NS.Color'Green' eq NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            Assert.Throws<ODataException>(parse);
        }

        [Fact]
        public void ParseFilterEnumTypesMismatch3()
        {
            Action parse = () => ParseFilter("NS.Color'Green' has ColorFlags", this.userModel, this.entityType, this.entitySet);
            Assert.Throws<ODataException>(parse);
        }

        [Fact]
        public void ParseFilterEnumTypesMismatch4()
        {
            Action parse = () => ParseFilter("NS.Color'Green' has NS.ColorFlags'2'", this.userModel, this.entityType, this.entitySet);
            Assert.Throws<ODataException>(parse);
        }

        [Fact]
        public void ParseFilterEnumTypesUndefined1()
        {
            Action parse = () => ParseFilter("NS1234.Color'Green' eq Color", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant("NS1234.Color'Green'"));
        }

        [Fact]
        public void ParseFilterEnumTypesUndefined2()
        {
            Action parse = () => ParseFilter("NS.BadColor'Green' eq Color", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant("NS.BadColor'Green'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined1()
        {
            Action parse = () => ParseFilter("NS.Color'_54' has NS.Color'Green'", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant("NS.Color'_54'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined2()
        {
            Action parse = () => ParseFilter("NS.ColorFlags'GreenYellow' has NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant("NS.ColorFlags'GreenYellow'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined3()
        {
            Action parse = () => ParseFilter("NS.ColorFlags'Green,Yellow' has NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant("NS.ColorFlags'Green,Yellow'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined4()
        {
            Action parse = () => ParseFilter("ColorFlags has NS.ColorFlags'Red,2'", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant("NS.ColorFlags'Red,2'"));
        }

        [Fact]
        public void ParseFilterEnumTypesWrongCast1()
        {
            Action parse = () => ParseFilter("cast(NS.ColorFlags'Green', 'Edm.Int64') eq 2", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.CastBinder_EnumOnlyCastToOrFromString);
        }

        [Fact]
        public void ParseFilterEnumTypesWrongCast2()
        {
            Action parse = () => ParseFilter("cast(321, 'NS.ColorFlags') eq 2", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.CastBinder_EnumOnlyCastToOrFromString);
        }

        [Fact]
        public void ParseFilterEnumTypesWrongCast3()
        {
            Action parse = () => ParseFilter("cast(321, 'NS.NotExistingColorFlags') eq 2", this.userModel, this.entityType, this.entitySet);
            parse.Throws<ODataException>(Strings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        [Theory]
        [InlineData("NS.Color'Green'")]
        [InlineData("'Green'")]
        [InlineData("'2'")]
        [InlineData("2")]
        public void ParseFilterWithInOperatorWithEnums(string filterOptionValue)
        {
            // Arrange
            string filterQuery = $"{filterOptionValue} in Colors";
            string enumValue = "Green";
            string colorTypeName = "NS.Color";

            IEdmEnumType colorType = this.GetIEdmType<IEdmEnumType>(colorTypeName);
            IEdmEnumMember enumMember = colorType.Members.First(m => m.Name == enumValue);

            string expectedLiteral = "'Green'"; 
            if (filterOptionValue.StartsWith(colorTypeName)) // if the filterOptionValue is already fully qualified, then the expectedLiteral should be the same as filterOptionValue
            {
                expectedLiteral = "NS.Color'Green'";
                enumValue = enumMember.Value.Value.ToString(); // "2" <cref="ODataEnumValue.Value"/> The backing type, can be "3" or "White" or "Black,Yellow,Cyan"
            }

            // Act
            FilterClause filterQueryNode = ParseFilter(filterQuery, this.userModel, this.entityType, this.entitySet);
            SingleValueNode expression = filterQueryNode.Expression;

            // Assert
            Assert.Equal(2, enumMember.Value.Value);
            Assert.Equal("Green", enumMember.Name);
            InNode inNode = expression.ShouldBeInNode();
            ConstantNode leftNode = Assert.IsType<ConstantNode>(inNode.Left);
            leftNode.ShouldBeEnumNode(colorType, enumValue);

            Assert.True(leftNode.TypeReference.IsEnum());
            Assert.Equal(enumValue, leftNode.Value.ToString());
            Assert.Equal(expectedLiteral, leftNode.LiteralText);

            CollectionPropertyAccessNode rightNode = Assert.IsType<CollectionPropertyAccessNode>(inNode.Right);
            rightNode.ShouldBeCollectionPropertyAccessQueryNode(this.GetIEdmProperty("Colors"));
            Assert.Equal(colorType, rightNode.ItemType.Definition);

        }

        [Fact]
        public void ParseFilterWithInOperatorWithEnumsMemberFloatIntegralValue_ThrowCollectionItemTypeMustBeSameAsSingleItemTypeException()
        {
            // Arrange
            string filterQuery = $"3.0 in Colors";

            // Act
            Action test = () => ParseFilter(filterQuery, this.userModel, this.entityType, this.entitySet);

            // Assert
            test.Throws<ArgumentException>(Strings.Nodes_InNode_CollectionItemTypeMustBeSameAsSingleItemType("NS.Color", "Edm.Single")); // Float are of Type Edm.Single
        }

        [Theory]
        [InlineData("-20", "-20")]
        [InlineData("'-20'", "-20")]
        [InlineData("'3.0'", "3.0")]
        public void ParseFilterWithInOperatorWithEnumsInvalidIntegralValues_ThrowsIsNotValidEnumConstantException(string integralValue, string errorMessageParam)
        {
            // Arrange
            string filterQuery = $"{integralValue} in Colors";

            // Act
            Action action = () => ParseFilter(filterQuery, this.userModel, this.entityType, this.entitySet);

            // Assert
            action.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant(errorMessageParam));
        }

        [Fact]
        public void ParseFilterWithInOperatorWithEnumsMemberNameWithoutSingleQuotes_ThrowsPropertyNotDeclaredException()
        {
            // Arrange
            string filterQuery = "Red in Colors";

            // Act
            Action action = () => ParseFilter(filterQuery, this.userModel, this.entityType, this.entitySet);

            // Assert
            action.Throws<ODataException>(Strings.MetadataBinder_PropertyNotDeclared(this.entityType.FullName, "Red"));
        }

        [Theory]
        [InlineData("'Yellow'")]
        [InlineData("'Teal'")]
        [InlineData("NS.Color'Yellow'")]
        [InlineData("NS.Color'Teal'")]
        public void ParseFilterWithInOperatorWithEnumsInvalidMemberNames_ThrowsIsNotValidEnumConstantException(string memberName)
        {
            // Arrange
            string filterQuery = $"{memberName} in Colors";

            string expectedExceptionParameter = memberName.StartsWith("'") ? memberName.Trim('\'') : memberName; // Trim('\'') method removes any single quotes from the start and end of the string

            // Act
            Action action = () => ParseFilter(filterQuery, this.userModel, this.entityType, this.entitySet);

            // Assert
            action.Throws<ODataException>(Strings.Binder_IsNotValidEnumConstant(expectedExceptionParameter));
        }

        private T GetIEdmType<T>(string typeName) where T : IEdmType
        {
            return (T)this.userModel.FindType(typeName);
        }

        private T GetIEdmType<T>(IEdmModel model, string typeName) where T : IEdmType
        {
            return (T)model.FindType(typeName);
        }

        private IEdmStructuralProperty GetIEdmProperty(string propertyName)
        {
            return (IEdmStructuralProperty)this.GetIEdmType<IEdmStructuredType>("NS.MyEntityType")
                .FindProperty(propertyName);
        }

        private IEdmStructuralProperty GetIEdmProperty(string entityName, string propertyName)
        {
            return (IEdmStructuralProperty)this.GetIEdmType<IEdmStructuredType>(entityName)
                .FindProperty(propertyName);
        }

        private FilterClause ParseFilter(string text, IEdmModel edmModel, IEdmEntityType edmEntityType, IEdmEntitySet edmEntitySet)
        {
            return new ODataQueryOptionParser(edmModel, entityType, edmEntitySet, new Dictionary<string, string>() { { "$filter", text } }).ParseFilter();
        }
    }
}
