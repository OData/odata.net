//---------------------------------------------------------------------
// <copyright file="EnumFilterFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
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
            var red = new EdmEnumMember(enumType, "Red", new EdmIntegerConstant(1));
            enumType.AddMember(red);
            enumType.AddMember("Green", new EdmIntegerConstant(2));
            enumType.AddMember("Blue", new EdmIntegerConstant(3));
            enumType.AddMember("White", new EdmIntegerConstant(-10));

            // add to model
            this.userModel.AddElement(enumType);

            // add enum property
            this.entityType = new EdmEntityType("NS", "MyEntityType", isAbstract: false, isOpen: true, baseType: null);
            var enumTypeReference = new EdmEnumTypeReference(enumType, true);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "Color", enumTypeReference));

            // enum with flags
            var enumFlagsType = new EdmEnumType("NS", "ColorFlags", EdmPrimitiveTypeKind.Int64, true);
            enumFlagsType.AddMember("Red", new EdmIntegerConstant(1L));
            enumFlagsType.AddMember("Green", new EdmIntegerConstant(2L));
            enumFlagsType.AddMember("Blue", new EdmIntegerConstant(4L));
            enumFlagsType.AddMember("GreenRed", new EdmIntegerConstant(3L));

            // add to model
            this.userModel.AddElement(enumFlagsType);

            // add enum with flags
            var enumFlagsTypeReference = new EdmEnumTypeReference(enumFlagsType, true);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "ColorFlags", enumFlagsTypeReference));

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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(GetColorProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetColorType(this.userModel), (int)Color.Green);
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(GetColorProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetColorType(this.userModel), (int)Color.Green);
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorFlagsProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetColorFlagsType(this.userModel), (int)ColorFlags.Green);
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorFlagsProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetColorFlagsType(this.userModel), (int)ColorFlags.Green);
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
            .Left
            .ShouldBeEnumNode(this.GetColorFlagsType(this.userModel), (int)(ColorFlags.Green | ColorFlags.Red));

            binaryNode
            .Right
            .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorFlagsProp(this.userModel));
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(this.GetColorType(this.userModel), (int)Color.Green);
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorFlagsProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetColorFlagsType(this.userModel),
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorFlagsProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetColorFlagsType(this.userModel),
                (int)(ColorFlags.Green | ColorFlags.Red));
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorFlagsProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetColorFlagsType(this.userModel),
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorFlagsProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetColorFlagsType(this.userModel),
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
                .ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                .And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetColorType(this.userModel),
                (int)(Color.White));

            binaryNode.Right.As<ConstantNode>().TypeReference.IsEnum();
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
                .And.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;

            binaryNode
                .Left
                .ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorProp(this.userModel));

            binaryNode
                .Right
                .ShouldBeEnumNode(
                this.GetColorType(this.userModel),
                -132534290);

            binaryNode.Right.As<ConstantNode>().TypeReference.IsEnum();
        }

        [Fact]
        public void ParseFilterWithEmptyEnumValue()
        {
            Action parse = () => ParseFilter("Color has NS.Color''", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.Binder_IsNotValidEnumConstant("NS.Color''"));
        }

        [Fact]
        public void ParseFilterWithNullEnumValue()
        {
            var filterQueryNode = ParseFilter("Color eq null", this.userModel, this.entityType, this.entitySet);
            var binaryNode = filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryNode.And.Left.ShouldBeSingleValuePropertyAccessQueryNode(this.GetColorProp(this.userModel));
            binaryNode.And.Right.As<ConvertNode>().Source.ShouldBeConstantQueryNode((object)null);
        }

        [Fact]
        public void ParseFilterCastMethod1()
        {
            var filter = ParseFilter("cast(NS.Color'Green', 'Edm.String') eq 'blue'", this.userModel, this.entityType, this.entitySet);
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.As<ConvertNode>().Source.As<SingleValueFunctionCallNode>().Name.Should().Be("cast"); // ConvertNode is because cast() result's nullable=false.
        }

        [Fact]
        public void ParseFilterCastMethod2()
        {
            var filter = ParseFilter("cast('Green', 'NS.Color') eq NS.Color'Green'", this.userModel, this.entityType, this.entitySet);
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.As<SingleValueFunctionCallNode>().Name.Should().Be("cast");
        }

        //to do: verify the exceptions for the Mismatch cases.
        [Fact]
        public void ParseFilterEnumTypesMismatch1()
        {
            Action parse = () => ParseFilter("Color'Green' eq NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>();
        }

        [Fact]
        public void ParseFilterEnumTypesMismatch2()
        {
            Action parse = () => ParseFilter("NS.Color'Green' eq NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>();
        }

        [Fact]
        public void ParseFilterEnumTypesMismatch3()
        {
            Action parse = () => ParseFilter("NS.Color'Green' has ColorFlags", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>();
        }

        [Fact]
        public void ParseFilterEnumTypesMismatch4()
        {
            Action parse = () => ParseFilter("NS.Color'Green' has NS.ColorFlags'2'", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>();
        }

        [Fact]
        public void ParseFilterEnumTypesUndefined1()
        {
            Action parse = () => ParseFilter("NS1234.Color'Green' eq Color", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.Binder_IsNotValidEnumConstant("NS1234.Color'Green'"));
        }

        [Fact]
        public void ParseFilterEnumTypesUndefined2()
        {
            Action parse = () => ParseFilter("NS.BadColor'Green' eq Color", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.Binder_IsNotValidEnumConstant("NS.BadColor'Green'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined1()
        {
            Action parse = () => ParseFilter("NS.Color'_54' has NS.Color'Green'", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.Binder_IsNotValidEnumConstant("NS.Color'_54'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined2()
        {
            Action parse = () => ParseFilter("NS.ColorFlags'GreenYellow' has NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.Binder_IsNotValidEnumConstant("NS.ColorFlags'GreenYellow'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined3()
        {
            Action parse = () => ParseFilter("NS.ColorFlags'Green,Yellow' has NS.ColorFlags'Green'", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.Binder_IsNotValidEnumConstant("NS.ColorFlags'Green,Yellow'"));
        }

        [Fact]
        public void ParseFilterEnumMemberUndefined4()
        {
            Action parse = () => ParseFilter("ColorFlags has NS.ColorFlags'Red,2'", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.Binder_IsNotValidEnumConstant("NS.ColorFlags'Red,2'"));
        }

        [Fact]
        public void ParseFilterEnumTypesWrongCast1()
        {
            Action parse = () => ParseFilter("cast(NS.ColorFlags'Green', 'Edm.Int64') eq 2", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.CastBinder_EnumOnlyCastToOrFromString);
        }

        [Fact]
        public void ParseFilterEnumTypesWrongCast2()
        {
            Action parse = () => ParseFilter("cast(321, 'NS.ColorFlags') eq 2", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.CastBinder_EnumOnlyCastToOrFromString);
        }

        [Fact]
        public void ParseFilterEnumTypesWrongCast3()
        {
            Action parse = () => ParseFilter("cast(321, 'NS.NotExistingColorFlags') eq 2", this.userModel, this.entityType, this.entitySet);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
        }

        private IEdmStructuralProperty GetColorProp(IEdmModel model)
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)model
                .FindType("NS.MyEntityType"))
                .FindProperty("Color");
        }

        private IEdmEnumType GetColorType(IEdmModel model)
        {
            return (IEdmEnumType)model.FindType("NS.Color");
        }

        private IEdmStructuralProperty GetColorFlagsProp(IEdmModel model)
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)model
                .FindType("NS.MyEntityType"))
                .FindProperty("ColorFlags");
        }

        private IEdmEnumType GetColorFlagsType(IEdmModel model)
        {
            return (IEdmEnumType)model.FindType("NS.ColorFlags");
        }

        private FilterClause ParseFilter(string text, IEdmModel edmModel, IEdmEntityType edmEntityType, IEdmEntitySet edmEntitySet)
        {
            return new ODataQueryOptionParser(edmModel, entityType, edmEntitySet, new Dictionary<string, string>() { { "$filter", text } }).ParseFilter();
        }
    }
}
