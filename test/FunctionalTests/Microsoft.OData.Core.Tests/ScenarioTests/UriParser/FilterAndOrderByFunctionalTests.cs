//---------------------------------------------------------------------
// <copyright file="FilterAndOrderByFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.Tests.ScenarioTests.UriBuilder;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.Spatial;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// This file contains functional tests for the ODataUriParser.ParseFilter and ParseOrderBy.
    /// </summary>
    public class FilterAndOrderByFunctionalTests
    {
        [Theory]
        [InlineData("ID eq 123")]
        [InlineData("ID eq 123L")]
        [InlineData("ID eq NaN")]
        [InlineData("ID add 123 eq 123")]
        [InlineData("ID add 123L eq 123")]
        public void ParseFilterLongValuesWithOptionalSuffix(string text)
        {
            var filterQueryNode = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            var binaryNode = filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterLongValuesWithOptionalSuffixForSepcial()
        {
            var filterQueryNode = ParseFilter("ID eq " + ((long)int.MaxValue + 10), HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode((long)int.MaxValue + 10);
        }

        [Theory]
        [InlineData("ID eq 123")]
        [InlineData("ID eq 123L")]
        [InlineData("ID eq NaN")]
        [InlineData("ID add 123 eq 123")]
        [InlineData("ID add 123L eq 123")]
        public void ParseFilterLongValuesWithOptionalSuffixUsingContainedEntitySet(string text)
        {
            var filterQueryNode = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterLongValuesWithOptionalSuffixUsingContainedEntitySetForSpecial()
        {
            var filterQueryNode = ParseFilter("ID eq " + ((long)int.MaxValue + 10), HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode((long)int.MaxValue + 10);
        }

        [Theory]
        [InlineData("ID eq 1F")]
        [InlineData("ID eq 1D")]
        [InlineData("ID eq 1M")]
        public void ParseFilterLongValuesNeedPromotion(string text)
        {
            var filterQueryNode = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Theory]
        [InlineData("ID eq 123")]
        [InlineData("ID eq 123F")]
        [InlineData("ID add 123 eq 123F")]
        public void ParseFilterFloatValuesWithOptionalSuffix(string text)
        {
            var filterQueryNode = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Theory]
        [InlineData("ID eq 123L")]
        public void ParseFilterFloatValuesNeedPromotion(string text)
        {
            var filterQueryNode = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Theory]
        [InlineData("ID eq 123M")]
        [InlineData("ID eq 3258.678765765489753678965390")]
        public void ParseFilterFloatValuesNeedPromotionThrows(string text)
        {
            //Non-ConstantNode "ID" whose type is float cannot be promoted to Decimal
            Action parse = () => ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Single", "Edm.Decimal", "Equal"));
        }

        [Theory]
        [InlineData("ID eq 123")]
        [InlineData("ID eq 123D")]
        [InlineData("ID add 123 eq 123D")]
        [InlineData("ID add 123D eq 123")]
        public void ParseFilterDoubleValuesWithOptionalSuffix(string text)
        {
            var filterQueryNode = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterDoubleValuesWithOptionalSuffixThrows()
        {
            // double and (implicit) decimal are incompatible
            string decimalPrecisionStr = "3258.678765765489753678965390";
            Action parse = () => ParseFilter("ID eq " + decimalPrecisionStr, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Equal"));
        }

        [Fact]
        public void ParseFilterDoubleValuesNeedPromotion()
        {
            var filterQueryNode = ParseFilter("ID eq 123L", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID eq 123F", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            //Non-ConstantNode whose type is Single cannot be promoted to Decimal
            Action parse = () => ParseFilter("ID eq 123M", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Equal"));
        }

#if !NETCOREAPP3_1
        [Fact]
        public void ParseFilterDecimalValuesWithOptionalSuffix()
        {
            var filterQueryNode = ParseFilter("ID eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID eq 123m", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID add 123 eq 123m", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID add 123m eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            // promotion: double constant -> decimal
            filterQueryNode = ParseFilter("ID eq 123.01", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            string decimalPrecisionStr = "3258.678765765489753678965390";

            // high precision decimal
            filterQueryNode = ParseFilter("ID eq " + decimalPrecisionStr, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode(3258.678765765489753678965390m);

            // double already overflows decimal
            Action parse = () => ParseFilter("1.79769313486232E+307 eq " + decimalPrecisionStr, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            Assert.Throws<OverflowException>(parse);

            // numeric string overflowing all types:
            parse = () => ParseFilter("1.79769313486232E+30700 eq " + decimalPrecisionStr, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            parse.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_InvalidNumericString("1.79769313486232E+30700"));
        }
#endif

        [Theory]
        [InlineData("ID eq 123L")]
        [InlineData("ID eq 123F")]
        [InlineData("ID eq 123D")]
        public void ParseFilterDecimalValuesNeedPromotion(string text)
        {
            var filterQueryNode = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterPropertyAndConstantsWithOptionalSuffix()
        {
            var filterQueryNode = ParseFilter("2 gt 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2);

            filterQueryNode = ParseFilter("2 gt 2l", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2L);

            filterQueryNode = ParseFilter("2 gt 2.2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2F);

            filterQueryNode = ParseFilter("2 gt 2.2d", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2D);

            filterQueryNode = ParseFilter("2 gt 2.34243223423235234423400003", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2m);
            (filterQueryNode.Expression as BinaryOperatorNode).Right.ShouldBeConstantQueryNode(2.34243223423235234423400003m);

            filterQueryNode = ParseFilter("2.2 gt 2.2D", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2.2D);

            filterQueryNode = ParseFilter("2D gt 2.34243223423235234423400003", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2m);
            (filterQueryNode.Expression as BinaryOperatorNode).Right.ShouldBeConstantQueryNode(2.34243223423235234423400003m);

            filterQueryNode = ParseFilter("2F gt 2.34243223423235234423400003M", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Left.ShouldBeConstantQueryNode(2m);
            (filterQueryNode.Expression as BinaryOperatorNode).Right.ShouldBeConstantQueryNode(2.34243223423235234423400003m);

            filterQueryNode = ParseFilter("SingleID eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Right.ShouldBeConstantQueryNode(2f);

            filterQueryNode = ParseFilter("DoubleID eq 2.01", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            (filterQueryNode.Expression as BinaryOperatorNode).Right.ShouldBeConstantQueryNode(2.01d);

            filterQueryNode = ParseFilter("SingleID eq 10000000000000000000"/*Single Precision*/, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode(10000000000000000000F);

            filterQueryNode = ParseFilter("SingleID eq 1000000000000f"/*Single Precision*/, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode(1000000000000f);

            filterQueryNode = ParseFilter("SingleID eq 11111111111100000000"/*Double Precision*/, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode(11111111111100000000D);

            Action parse = () => filterQueryNode = ParseFilter("SingleID eq 2.34243223423235234423400003m", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Single", "Edm.Decimal", "Equal"));

            parse = () => ParseFilter("DoubleID eq DecimalID", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Equal"));

            parse = () => ParseFilter("SingleID eq DecimalID", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Single", "Edm.Decimal", "Equal"));
        }

        [Fact]
        public void ParseFilterINFNaNWithoutSuffix()
        {
            var filterQueryNode = ParseFilter("ID gt INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            filterQueryNode = ParseFilter("ID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            var left = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Left;
            Assert.IsType<BinaryOperatorNode>(left).Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            filterQueryNode = ParseFilter("ID gt -INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Double.NegativeInfinity);

            filterQueryNode = ParseFilter("ID eq NaN", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Double.NaN);

            filterQueryNode = ParseFilter("SingleID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            left = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Left;
            Assert.IsType<BinaryOperatorNode>(left).Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            filterQueryNode = ParseFilter("DoubleID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            left = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Left;
            Assert.IsType<BinaryOperatorNode>(left).Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            Action parse = () => ParseFilter("DecimalID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.Throws<OverflowException>(parse);
        }

        [Fact]
        public void ParseFilterINFNaNWithSuffix()
        {
            var filterQueryNode = ParseFilter("ID gt INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);

            filterQueryNode = ParseFilter("ID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            var left = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Left;
            Assert.IsType<BinaryOperatorNode>(left).Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);

            filterQueryNode = ParseFilter("ID gt -INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Single.NegativeInfinity);

            filterQueryNode = ParseFilter("ID eq NaNF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Single.NaN);

            filterQueryNode = ParseFilter("SingleID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            left = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Left;
            Assert.IsType<BinaryOperatorNode>(left).Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);

            filterQueryNode = ParseFilter("DoubleID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            left = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Left;
            Assert.IsType<BinaryOperatorNode>(left).Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            Action parse = () => ParseFilter("DecimalID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.Throws<OverflowException>(parse);
        }

        [Fact]
        public void ParseFilterWithBoolean()
        {
            var filterQueryNode = ParseFilter("ID eq true", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(true);

            filterQueryNode = ParseFilter("ID eq false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(false);

            filterQueryNode = ParseFilter("ID eq not true", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            var right = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right;
            Assert.IsType<UnaryOperatorNode>(right).Operand.ShouldBeConstantQueryNode(true);

            filterQueryNode = ParseFilter("ID eq not false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            right = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right;
            Assert.IsType<UnaryOperatorNode>(right).Operand.ShouldBeConstantQueryNode(false);

            filterQueryNode = ParseFilter("ID and true eq false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            right = Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right;
            var bon = Assert.IsType<BinaryOperatorNode>(right);
            bon.Left.ShouldBeConstantQueryNode(true);
            bon.Right.ShouldBeConstantQueryNode(false);

            Action parse = () => ParseFilter("ID eq 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Boolean", "Edm.Int32", "Equal"));

        }

#if !NETCOREAPP3_1
        [Fact]
        public void ParseFilterNodeInComplexExpression()
        {
            var filterQueryNode = ParseFilter("(ID mul 1 add 1.01 sub 1.000000001) mul 2 ge 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(1D);

            Action parse = () => ParseFilter("(DoubleID mul 1 add 1.000000000000001 sub 1.000000001) mul 2 ge 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Add"));
        }
#endif
        [Fact]
        public void ParseFilterDoublePrecision()
        {
            var filterQueryNode = ParseFilter("DoubleID eq 1.0099999904632568", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(1.0099999904632568D);
        }

        [Fact]
        public void ParseFilterSinglePrecision()
        {
            var filterQueryNode = ParseFilter("SingleID eq " + Single.MinValue.ToString("R"), HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            Assert.IsType<BinaryOperatorNode>(filterQueryNode.Expression).Right.ShouldBeConstantQueryNode(Single.MinValue);
        }

        [Fact]
        public void ParseFilterWithEntitySetShouldBeAbleToDetermineSets()
        {
            var filterQueryNode = ParseFilter("MyDog/Color eq 'brown'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.Same(HardCodedTestModel.GetDogsSet(), filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                    Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).
                    NavigationSource);
        }

        [Fact]
        public void ParseFilterWithPrimitiveCollectionCount()
        {
            var filterQueryNode = ParseFilter("MyDates/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeCountNode().
                    Source.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDatesProp());
        }

        [Fact]
        public void ParseFilterWithComplexCollectionCount()
        {
            var filterQueryNode = ParseFilter("PreviousAddresses/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeCountNode().
                    Source.ShouldBeCollectionComplexNode(HardCodedTestModel.GetPersonPreviousAddressesProp());
        }

        [Fact]
        public void ParseFilterWithEnumCollectionCount()
        {
            var filterQueryNode = ParseFilter("FavoriteColors/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeCountNode().
                    Source.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonFavoriteColorsProp());
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCount()
        {
            var filterQueryNode = ParseFilter("MyFriendsDogs/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeCountNode().
                    Source.ShouldBeCollectionNavigationNode(HardCodedTestModel.GetPersonMyFriendsDogsProp());
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCountWithFilterOption()
        {
            var filterQueryNode = ParseFilter("MyFriendsDogs/$count($filter=Color eq 'Brown') gt 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                Left.ShouldBeCountNode().
                    Source.ShouldBeCollectionNavigationNode(HardCodedTestModel.GetPersonMyFriendsDogsProp());

            BinaryOperatorNode binaryNode = filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
            CountNode countNode = binaryNode.Left.ShouldBeCountNode();
            countNode.Source.ShouldBeCollectionNavigationNode(HardCodedTestModel.GetPersonMyFriendsDogsProp());

            Assert.Null(countNode.SearchClause);
            BinaryOperatorNode innerBinaryNode = countNode.FilterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            Assert.Equal("Color", Assert.IsType<SingleValuePropertyAccessNode>(innerBinaryNode.Left).Property.Name);
            Assert.Equal("Brown", Assert.IsType<ConstantNode>(innerBinaryNode.Right).Value);
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCountWithUnbalanceParenthesisThrows()
        {
            Action parse = () => ParseFilter("MyFriendsDogs/$count($filter=Color eq 'Brown' gt 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError(50, "MyFriendsDogs/$count($filter=Color eq 'Brown' gt 1"));
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCountWithEmptyParenthesisThrows()
        {
            Action parse = () => ParseFilter("MyFriendsDogs/$count()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.UriParser_EmptyParenthesis());
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCountWithIllegalQueryOptionThrows()
        {
            Action parse = () => ParseFilter("MyFriendsDogs/$count($orderby=Color) gt 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_IllegalQueryOptioninDollarCount());
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCountWithSearchOption()
        {
            var filterQueryNode = ParseFilter("MyFriendsDogs/$count($search=brown) gt 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            BinaryOperatorNode binaryNode = filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
            CountNode countNode = binaryNode.Left.ShouldBeCountNode();
            countNode.Source.ShouldBeCollectionNavigationNode(HardCodedTestModel.GetPersonMyFriendsDogsProp());

            Assert.Null(countNode.FilterClause);
            countNode.SearchClause.Expression.ShouldBeSearchTermNode("brown");
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCountWithFilterAndSearchOptions()
        {
            var filterQueryNode = ParseFilter("MyFriendsDogs/$count($filter=Color eq 'Brown';$search=brown) gt 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            BinaryOperatorNode binaryNode = filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
            CountNode countNode = binaryNode.Left.ShouldBeCountNode();
            countNode.SearchClause.Expression.ShouldBeSearchTermNode("brown");

            BinaryOperatorNode innerBinaryNode = countNode.FilterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            Assert.Equal("Color", Assert.IsType<SingleValuePropertyAccessNode>(innerBinaryNode.Left).Property.Name);
            Assert.Equal("Brown", Assert.IsType<ConstantNode>(innerBinaryNode.Right).Value);
        }

        [Fact]
        public void ParseFilterWithSingleValueCountWithFilterAndSearchOptionsThrows()
        {
            Action parse = () => ParseFilter("ID/$count($filter=Color eq 'Brown';$search=brown) gt 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CountSegmentNextTokenNotCollectionValue());
        }

        [Fact]
        public void CompareComplexWithNull()
        {
            var filter = ParseFilter("MyAddress eq null", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            var binary = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binary.Left.ShouldBeSingleComplexNode(HardCodedTestModel.GetPersonAddressProp());
            binary.Right.ShouldBeConvertQueryNode(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void ReplaceShouldWork()
        {
            var filterQueryNode = ParseFilter("replace(Name, 'a', 'e') eq 'endrew'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeSingleValueFunctionCallQueryNode("replace");
        }

        [Fact]
        public void FilterWithKeyLookupOnNavPropIsNotAllowed()
        {
            Action parse = () => ParseFilter("MyPeople(987)", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnknownFunction("MyPeople"));
        }

        [Fact]
        public void ParseFilterWithNoEntitySetUsesNull()
        {
            var filterQueryNode = ParseFilter("MyDog/Color eq 'brown'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            Assert.Null(filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                    Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).
                    NavigationSource);
        }

        [Fact]
        public void FilterWithAnyOnStringCollectionProperty()
        {
            var filterQueryNode = ParseFilter("Nicknames/any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());

            filterQueryNode.Expression.ShouldBeAnyQueryNode();
        }

        [Fact]
        public void FilterWithAnyOnCollectionOfComplex()
        {
            var filterQueryNode = ParseFilter("MyAddress/MyNeighbors/any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterQueryNode.Expression.ShouldBeAnyQueryNode().
                Source.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetAddressMyNeighborsProperty());
        }

        [Fact]
        public void FilterWithAnyOnCollectionNavigationProperty()
        {
            var filterQueryNode = ParseFilter("MyPeople/any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());

            filterQueryNode.Expression.ShouldBeAnyQueryNode();
        }

        [Fact]
        public void AnyIsCaseSensitive()
        {
            Action parse = () => ParseFilter("MyPeople/Any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());

            parse.Throws<ODataException>(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("Any"));
        }

        [Fact]
        public void ParseOrderByWithEntitySetShouldBeAbleToDetermineSets()
        {
            var orderByQueryNode = ParseOrderBy("MyDog/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.Same(HardCodedTestModel.GetDogsSet(),
                orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).NavigationSource);
        }

        [Fact]
        public void ParseOrderByWithContainedEntitySetShouldBeAbleToDetermineSets()
        {
            var orderByQueryNode = ParseOrderBy("MyContainedDog/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.Same(HardCodedTestModel.GetContainedDogEntitySet(),
                orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyContainedDogNavProp()).NavigationSource);
        }

        [Fact]
        public void ParseOrderByWithNoEntitySetUsesNull()
        {
            var orderByQueryNode = ParseOrderBy("MyDog/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            Assert.Null(orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).NavigationSource);
        }

        [Fact]
        public void ParseMultipleOrderBys()
        {
            var orderByQueryNode = ParseOrderBy("Name asc, Shoe desc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            Assert.Equal(OrderByDirection.Ascending, orderByQueryNode.Direction);

            orderByQueryNode.ThenBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            Assert.Equal(OrderByDirection.Descending, orderByQueryNode.ThenBy.Direction);
        }

        [Fact]
        public void ParseEnumMultipleOrderBys()
        {
            var orderByQueryNode = ParseOrderBy("PetColorPattern asc, cast(PetColorPattern,'Edm.String') desc, PetColorPattern has Fully.Qualified.Namespace.ColorPattern'SolidYellow' asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type());
            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            Assert.Equal(OrderByDirection.Ascending, orderByQueryNode.Direction);
            Assert.Equal(2, orderByQueryNode.ThenBy.Expression.ShouldBeSingleValueFunctionCallQueryNode().Parameters.Count());
            Assert.Equal(OrderByDirection.Descending, orderByQueryNode.ThenBy.Direction);
            orderByQueryNode.ThenBy.ThenBy.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            Assert.Equal(OrderByDirection.Ascending, orderByQueryNode.ThenBy.ThenBy.Direction);
        }

        [Fact]
        public void ParseEnumPropertyOrderBy()
        {
            var orderByQueryNode = ParseOrderBy("PetColorPattern asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type());
            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            Assert.Equal(OrderByDirection.Ascending, orderByQueryNode.Direction);
        }

        [Fact]
        public void ParseEnumConstantOrderBy()
        {
            var orderByQueryNode = ParseOrderBy("Fully.Qualified.Namespace.ColorPattern'SolidYellow' asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type());
            var enumtypeRef = new EdmEnumTypeReference(UriEdmHelpers.FindEnumTypeFromModel(HardCodedTestModel.TestModel, "Fully.Qualified.Namespace.ColorPattern"), true);
            orderByQueryNode.Expression.ShouldBeEnumNode(enumtypeRef.EnumDefinition(), "12");
            Assert.Equal(OrderByDirection.Ascending, orderByQueryNode.Direction);
        }

        [Fact]
        public void OrderbyDatetimeOffset()
        {
            var orderByQueryNode = ParseOrderBy("Birthdate", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonBirthdateProp());
        }

        [Fact]
        public void OrderbyDate()
        {
            var orderByQueryNode = ParseOrderBy("MyDate", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
        }

        [Fact]
        public void OrderbyTimeOfDay()
        {
            var orderByQueryNode = ParseOrderBy("MyTimeOfDay", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDayProp());
        }

        [Fact]
        public void FilterWithDatetimeOffset()
        {
            var filterNode = ParseFilter("Birthdate gt 1997-02-04+11:00", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan)
                .Right.ShouldBeConstantQueryNode(new DateTimeOffset(1997, 2, 4, 0, 0, 0, 0, new TimeSpan(11, 0, 0)));
        }

        [Fact]
        public void FilterWithShortDatetimeOffset()
        {
            var filterNode = ParseFilter("Birthdate gt 1997-02-04", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(false))
                .Source.ShouldBeConstantQueryNode(new Date(1997, 2, 4));
        }

        [Fact]
        public void FilterWithDate()
        {
            var filterNode = ParseFilter("MyDate gt 1997-02-04", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                 Right.ShouldBeConstantQueryNode(new Date(1997, 2, 4));
        }

        [Fact]
        public void FilterWithTimeOfDay()
        {
            var filterNode = ParseFilter("MyTimeOfDay gt 23:40:40.900", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                Right.ShouldBeConstantQueryNode(new TimeOfDay(23, 40, 40, 900));
        }

        [Fact]
        public void FilterWithDuration()
        {
            var filterNode = ParseFilter("duration'PT0H0M15S' eq duration'P1DT0H0M30S'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var binaryOp = filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryOp.Left.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 15));
            binaryOp.Right.ShouldBeConstantQueryNode(new TimeSpan(1, 0, 0, 30));
        }

        [Fact]
        public void FilterByWithNonEntityType()
        {
            FilterClause filter = ParseFilter("$it gt 6", HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            var binaryOp = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
            binaryOp.Left.ShouldBeNonResourceRangeVariableReferenceNode("$it");
            binaryOp.Right.ShouldBeConstantQueryNode(6);
        }

        [Fact]
        public void FilterWithIncompatibleTypeShouldThrow()
        {
            Action action = () => ParseFilter("contains($it,'6')", HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            action.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound("contains", "contains(Edm.String Nullable=true, Edm.String Nullable=true)"));
        }

        [Fact]
        public void FilterWithInvalidParameterShouldThrow()
        {
            Action action = () => ParseFilter("$It gt 6", HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            action.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Edm.Int32", "$It"));
        }

        [Fact]
        public void BadCastShouldResultInNiceException()
        {
            Action parse = () => ParseOrderBy("MyDog/Missing.Type/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            parse.Throws<ODataException>(ODataErrorStrings.CastBinder_ChildTypeIsNotEntity("Missing.Type"));
        }

        [Fact]
        public void NullValueInCanonicalFunction()
        {
            var result = ParseFilter("day(null)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            var typeReference = result.Expression.ShouldBeSingleValueFunctionCallQueryNode("day")
                .Parameters.Single().ShouldBeConstantQueryNode<object>(null).TypeReference;
            Assert.Null(typeReference);
        }

        [Fact]
        public void OrderByWithEntityExpressionShouldThrow()
        {
            Action parse = () => ParseOrderBy("MyDog", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
        }

        [Fact]
        public void OrderByWithEntityCollectionExpressionShouldThrow()
        {
            Action parse = () => ParseOrderBy("MyPeople", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());

            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
        }

        [Fact]
        public void NegateAnEntityShouldThrow()
        {
            Action parse = () => ParseOrderBy("-MyDog", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_IncompatibleOperandError(HardCodedTestModel.GetPersonMyDogNavProp().Type.FullName(), UnaryOperatorKind.Negate));
        }

        [Fact]
        public void IsOfFunctionWorksWithSingleQuotesOnType()
        {
            FilterClause filter = ParseFilter("isof(Shoe, 'Edm.String')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var singleValueFunctionCallNode = filter.Expression.ShouldBeSingleValueFunctionCallQueryNode("isof");
            singleValueFunctionCallNode.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            singleValueFunctionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
        }

        [Fact]
        public void CastFunctionWorksWithNoSingleQuotesOnType()
        {
            FilterClause filter = ParseFilter("cast(Shoe, Edm.String) eq 'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var convertQueryNode = bon.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String);
            var singleFunctionCallNode = convertQueryNode.Source.ShouldBeSingleValueFunctionCallQueryNode("cast");
            singleFunctionCallNode.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            singleFunctionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
            bon.Right.ShouldBeConstantQueryNode("blue");
        }

        [Fact]
        public void CastFunctionWorksForEnum()
        {
            FilterClause filter = ParseFilter("cast(Shoe, Fully.Qualified.Namespace.ColorPattern) eq Fully.Qualified.Namespace.ColorPattern'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var singleFunctionCallNode = bon.Left.ShouldBeSingleValueFunctionCallQueryNode("cast");
            singleFunctionCallNode.Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            singleFunctionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Fully.Qualified.Namespace.ColorPattern");
            bon.Right.ShouldBeEnumNode(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern") as IEdmEnumType, 2L);
        }

        [Fact]
        public void CastFunctionWorksForCastFromNullToEnum()
        {
            FilterClause filter = ParseFilter("cast(null, Fully.Qualified.Namespace.ColorPattern) eq Fully.Qualified.Namespace.ColorPattern'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var singleFunctionCallNode = bon.Left.ShouldBeSingleValueFunctionCallQueryNode("cast");
            Assert.Null(Assert.IsType<ConstantNode>(singleFunctionCallNode.Parameters.ElementAt(0)).Value);
            singleFunctionCallNode.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Fully.Qualified.Namespace.ColorPattern");
            bon.Right.ShouldBeEnumNode(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern") as IEdmEnumType, 2L);
        }

        [Fact]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfDottedIdentifier()
        {
            FilterClause filter = ParseFilter("cast(null, Fully.Qualified.Namespace.ColorPattern) eq Fully.Qualified.Namespace.ColorPattern'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var singleFunctionCallNode = bon.Left.ShouldBeSingleValueFunctionCallQueryNode("cast");
            Assert.Equal("null", Assert.IsType<ConstantNode>(singleFunctionCallNode.Parameters.ElementAt(0)).LiteralText);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", Assert.IsType<ConstantNode>(singleFunctionCallNode.Parameters.ElementAt(1)).LiteralText);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern'blue'", Assert.IsType<ConstantNode>(bon.Right).LiteralText);
        }

        [Fact]
        public void CastFunctionProducesAnEntityType()
        {
            FilterClause filter = ParseFilter("cast(MyDog, 'Fully.Qualified.Namespace.Dog')/Color eq 'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            SingleResourceFunctionCallNode function = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                .Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp())
                .Source.ShouldBeSingleResourceFunctionCallNode("cast");
            Assert.Equal(2, function.Parameters.Count());
            function.Parameters.ElementAt(0).ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp());
            function.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Fully.Qualified.Namespace.Dog");
            Assert.IsType<BinaryOperatorNode>(filter.Expression).Right.ShouldBeConstantQueryNode("blue");
        }

        [Fact]
        public void OrderByWithExpression()
        {
            OrderByClause orderBy = ParseOrderBy("Shoe eq 'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var bon = orderBy.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            bon.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            bon.Right.ShouldBeConstantQueryNode("blue");
        }

        [Fact]
        public void OrderByWithNonEntityType()
        {
            OrderByClause orderBy = ParseOrderBy("$it", HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            orderBy.Expression.ShouldBeNonResourceRangeVariableReferenceNode("$it");
        }

        [Fact]
        public void FilterWithOpenPropertyInsideAny()
        {
            // regression coverage for: URI Parser fails on open property expression inside any/all
            var filterClause = ParseFilter("MyPaintings/any(p:p/OpenProperty)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            Assert.NotNull(filterClause);
            filterClause.Expression.ShouldBeAnyQueryNode()
                        .Body.ShouldBeSingleValueOpenPropertyAccessQueryNode("OpenProperty");
        }

        [Fact]
        public void InvalidPropertyNameThrowsUsefulErrorMessage()
        {
            // regression test for: [Fuzz] InvalidCastException being thrown for filter/orderby with query "PersonMetadataId /c"
            Action parseInvalidPropertyName = () => ParseFilter("PersonMetadataId /c", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseInvalidPropertyName.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "PersonMetadataId"));
        }

        [Fact]
        public void ControlCharactersShouldBeIgnored()
        {
            // regression test for: [UriParser] \n in string leads to ambiguity
            var filterClause = ParseFilter("length(Name) \neq 30", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryOperatorNode.Left.ShouldBeSingleValueFunctionCallQueryNode("length").Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            binaryOperatorNode.Right.ShouldBeConstantQueryNode(30);

            // regression test for: [UriParser] \r in the path string gets disappeared
            var filterClause1 = ParseFilter("length(Name) \req 30", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode1 = filterClause1.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryOperatorNode1.Left.ShouldBeSingleValueFunctionCallQueryNode("length").Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            binaryOperatorNode1.Right.ShouldBeConstantQueryNode(30);
        }

        [Fact]
        public void TrailingDollarSegmentThows()
        {
            // regression test for: [UriParser] Trailing $ lost
            Action parseWithTrailingDollarSign = () => ParseOrderBy("Name/$", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithTrailingDollarSign.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Edm.String", "$"));
        }

        [Fact]
        public void ArbitraryFunctionCallsNotAllowed()
        {
            // regression test for: [UriParser] day() allowed. What does that mean?
            // make sure arbitrary funcitons aren't allowed...
            Action parseArbitraryFunctionCall = () => ParseFilter("gobbldygook() eq 20", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseArbitraryFunctionCall.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnknownFunction("gobbldygook"));
        }

        [Fact]
        public void EmptyFunctionCallParametersAreProperlyValidated()
        {
            // regression test for: [UriParser] day() allowed. What does that mean?
            // make sure that, if we do find a cannonical function, we match its parameters.
            Action parseWithInvalidParameters = () => ParseFilter("day() eq 20", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.ExtractSignatures(
                FunctionCallBinder.GetUriFunctionSignatures("day")); // to match the error message... blah
            parseWithInvalidParameters.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [Fact]
        public void FunctionCallParametersAreValidated()
        {
            // regression test for: [UriParser] day() allowed. What does that mean?
            // make sure that, if we do find a cannonical function, we match its parameters.
            Action parseWithInvalidParameters = () => ParseFilter("day(1) eq 20", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.ExtractSignatures(
                FunctionCallBinder.GetUriFunctionSignatures("day")); // to match the error message... blah
            parseWithInvalidParameters.Throws<ODataException>(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [Fact]
        public void AnyAllOnAPrimitiveShouldThrowODataException()
        {
            // regression test for: [URIParser] $filter with Any/All throws invalid cast instead of Odata Exception
            Action anyOnPrimitiveType = () => ParseFilter("Name/any(a: a eq 'Bob')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            anyOnPrimitiveType.Throws<ODataException>(ODataErrorStrings.MetadataBinder_LambdaParentMustBeCollection);
        }

#region Custom Functions

        [Fact]
        public void FunctionWithASingleOverloadWorksInFilter()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.HasJob", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("Fully.Qualified.Namespace.HasJob");
        }

        [Fact]
        public void FunctionWithASingleOverloadWorksInOrderby()
        {
            var orderbyClause = ParseOrderBy("Fully.Qualified.Namespace.HasJob asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            orderbyClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("Fully.Qualified.Namespace.HasJob");
            Assert.Equal(OrderByDirection.Ascending, orderbyClause.Direction);
        }

        [Fact]
        public void NamespaceQualifiedFunctionWorksInFilter()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.HasJob", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("Fully.Qualified.Namespace.HasJob");
        }

        [Fact]
        public void NamespaceQualifiedFunctionWorksInOrderby()
        {
            var orderByClause = ParseOrderBy("Fully.Qualified.Namespace.HasJob asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            orderByClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("Fully.Qualified.Namespace.HasJob");
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
        }

        [Fact]
        public void FunctionCallWithParametersWorksInFilter()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.HasDog(inOffice=true)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters())
                .Source.ShouldBeResourceRangeVariableReferenceNode("$it");
        }

        [Fact]
        public void DeepFunctionCallWithParametersWorksInOrderby()
        {
            var filterClause = ParseOrderBy("MyPeople/any(a: a/Fully.Qualified.Namespace.HasDog(inOffice=true))", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            filterClause.Expression.ShouldBeAnyQueryNode()
                .Body.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters())
                .Source.ShouldBeResourceRangeVariableReferenceNode("a");
        }

        [Fact]
        public void FunctionWithComplexParameterInJsonWithSingleQuotesInsteadOfDoubleQuotesWorks()
        {
            const string text = "Fully.Qualified.Namespace.CanMoveToAddress(address={'Street' : 'stuff', 'City' : 'stuff'})";
            var filterClause = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForCanMoveToAddress()).Parameters;
            var paramNode = Assert.IsType<NamedFunctionParameterNode>(Assert.Single(parameters));
            var constantNode = Assert.IsType<ConstantNode>(paramNode.Value);
            Assert.Equal("{'Street' : 'stuff', 'City' : 'stuff'}", constantNode.Value);
        }

        [Fact]
        public void FunctionWithInvalidComplexParameterThrows()
        {
            Action parseInvalidComplex = () => ParseFilter("Fully.Qualified.Namespace.CanMoveToAddress(address={}})", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseInvalidComplex.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_InvalidCharacter("}", "53", "Fully.Qualified.Namespace.CanMoveToAddress(address={}})"));
        }

        [Fact]
        public void FunctionWithComplexParameterInJsonWorks()
        {
            var filterCaluse = ParseFilter("Fully.Qualified.Namespace.CanMoveToAddress(address={\"@odata.type\":\"#Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"})", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var parameters = filterCaluse.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForCanMoveToAddress()).Parameters;
            var paramNode = Assert.IsType<NamedFunctionParameterNode>(Assert.Single(parameters));
            var constantNode = Assert.IsType<ConstantNode>(paramNode.Value);
            Assert.Equal("{\"@odata.type\":\"#Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"}", constantNode.Value);
        }

        [Fact]
        public void FunctionWithCollectionParameterInJsonWorks()
        {
            const string text = "Fully.Qualified.Namespace.OwnsTheseDogs(dogNames=[\"Barky\",\"Junior\"])";
            var filterCaluse = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var parameters = filterCaluse.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForOwnsTheseDogs()).Parameters;
            var paramNode = Assert.IsType<NamedFunctionParameterNode>(Assert.Single(parameters));
            var constantNode = Assert.IsType<ConstantNode>(paramNode.Value);
            var collectionValue = constantNode.Value.ShouldBeODataCollectionValue();
            collectionValue.ItemsShouldBeAssignableTo<string>();
            Assert.Equal(2, collectionValue.Items.Count());
        }

        [Fact]
        public void FunctionWithCollectionOfComplexParameterInJsonWorks()
        {
            const string text = "Fully.Qualified.Namespace.CanMoveToAddresses(addresses=[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}])";
            var filterCaluse = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            // TODO: parameter value is ConstantNode, whose .TypeReference should NOT be null though .Value is ok to be ODataCollectionValue.
            var parameters = filterCaluse.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForCanMoveToAddresses()).Parameters;
            var paramNode = Assert.IsType<NamedFunctionParameterNode>(Assert.Single(parameters));
            var constantNode = Assert.IsType<ConstantNode>(paramNode.Value);
            Assert.Equal("[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}]", constantNode.Value);
        }

        [Fact]
        public void FunctionParameterAliasWorksInFilter()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host/"), new Uri("http://host/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice=@a)&@a=null"));
            var filterClause = uriParser.ParseFilter();
            var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters()).Parameters;
            var paramNode = Assert.IsType<NamedFunctionParameterNode>(Assert.Single(parameters));
            var aliasNode = Assert.IsType<ParameterAliasNode>(paramNode.Value);
            Assert.Equal("@a", aliasNode.Alias);

            // verify alias value node:
            uriParser.ParameterAliasNodes["@a"].ShouldBeConstantQueryNode((object)null);
        }

        [Fact]
        public void UnresolvedFunctionParameterAliasWorksInFilter()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.HasDog(inOffice=@a)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters()).Parameters;
            var paramNode = Assert.IsType<NamedFunctionParameterNode>(Assert.Single(parameters));
            var aliasNode = Assert.IsType<ParameterAliasNode>(paramNode.Value);
            Assert.Equal("@a", aliasNode.Alias);
        }

        [Fact]
        public void FunctionsAreComposable()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.GetMyDog/Color eq 'Blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp())
                              .Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog());
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Blue");
        }

        [Fact]
        public void FunctionsBoundToCollectionTypesWork()
        {
            var filterClause = ParseFilter("MyPeople/Fully.Qualified.Namespace.AllHaveDog()", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForAllHaveDogWithOneParameter());
        }

        [Fact]
        public void CannotFindFunctionWithMatchedParameters()
        {
            Action parseWithExplicitBindingParam = () => ParseFilter("Fully.Qualified.Namespace.HasDog(person=$it)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithExplicitBindingParam.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnknownFunction("Fully.Qualified.Namespace.HasDog")); // no '...HasDog' method has parameter type of '$it' RangeVariableToken.
        }

        [Fact]
        public void CannotAddEntityAsBindingParameterToFunction()
        {
            Action parseWithExplicitBindingParam = () => ParseFilter("HasDog(person=People(1))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithExplicitBindingParam.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnknownFunction("People"));
        }

        [Fact]
        public void FunctionsBoundToCollectionTypesWithParametersWork()
        {
            var filterClause = ParseFilter("MyPeople/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForAllHaveDogWithTwoParameters());
        }

        [Fact]
        public void FunctionBoundToCollectionInMiddleOfPathShouldWork()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.AllMyFriendsDogs()/Fully.Qualified.Namespace.OwnerOfFastestDog()/Fully.Qualified.Namespace.HasDog()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("Fully.Qualified.Namespace.HasDog"); // TODO: should only actually match the one with no params...
        }

        [Fact]
        public void FunctionCallChainShouldWorkEvenIfEntitySetIsNotKnownForAllFunctions()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet()/Fully.Qualified.Namespace.OwnerOfFastestDog()/Fully.Qualified.Namespace.HasDog()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("Fully.Qualified.Namespace.HasDog"); // TODO: should only actually match the one with no params...
        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToComplexShouldWork()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var filterNode = ParseFilter("Color/Test.IsDark()", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            filterNode.Expression.ShouldBeSingleValueFunctionCallQueryNode(model.FindDeclaredOperations("Test.IsDark").Single() as IEdmFunction);
            Assert.IsType<SingleValueFunctionCallNode>(filterNode.Expression).Source.ShouldBeSingleComplexNode(model.EntityTypes().Single().Properties().Single(p => p.Name == "Color"));
        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToComplexWithParameterShouldWork()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var filterNode = ParseFilter("Color/Test.IsDarkerThan(other={\"Red\":64}) eq true", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            var left = filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Left;
            left.ShouldBeSingleValueFunctionCallQueryNode(model.FindDeclaredOperations("Test.IsDarkerThan").Single() as IEdmFunction);
            Assert.IsType<SingleValueFunctionCallNode>(left).Source.ShouldBeSingleComplexNode(model.EntityTypes().Single().Properties().Single(p => p.Name == "Color"));

        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToComplexAllowsComposition()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var filterNode = ParseFilter("Color/Test.GetMostPopularVegetableWithThisColor()/ID eq ID", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                Left.ShouldBeSingleValuePropertyAccessQueryNode(model.EntityTypes().Single().Properties().Single(p => p.Name == "ID"));
        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToPrimitiveShouldNotLookForFunctions()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            Action parse = () => ParseFilter("ID/IsPrime()", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            parse.Throws<ODataException>(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("IsPrime"));
        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToPrimitiveWithoutParensShouldNotLookForFunctions()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            Action parse = () => ParseFilter("ID/Test.IsPrime", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            parse.Throws<ODataException>(ODataErrorStrings.CastBinder_ChildTypeIsNotEntity("Test.IsPrime"));
        }

        [Fact]
        public void FunctionWithExpressionParameterThrows()
        {
            Action parseWithExpressionParameter = () => ParseFilter("OwnsTheseDogs(dogNames=Dogs(0))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithExpressionParameter.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnknownFunction("Dogs"));
        }

        [Fact]
        public void FunctionWithMultipleParametersWithTheSameNameThrows()
        {
            var model = ModelBuildingHelpers.GetModelWithFunctionWithDuplicateParameterNames();
            Action parseWithMultipleParameters = () => ParseFilter("Test.Foo(p2='stuff', p2=1)", model, model.EntityTypes().Single(e => e.Name == "Vegetable"));
            parseWithMultipleParameters.Throws<ODataException>(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
        }

        [Fact]
        public void LongFunctionChainWorksInFilter()
        {
            var filter = ParseFilter("Fully.Qualified.Namespace.GetMyDog/Fully.Qualified.Namespace.GetMyPerson/Fully.Qualified.Namespace.GetMyDog/Fully.Qualified.Namespace.GetMyPerson/Name eq 'Bob'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp())
                .Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyPerson())
                .Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog())
                .Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyPerson())
                .Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog())
                .Source.ShouldBeResourceRangeVariableReferenceNode(ExpressionConstants.It);
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Bob");
        }

#endregion

        [Fact]
        public void ActionsThrowOnClosedTypeInFilter()
        {
            Action parseWithAction = () => ParseFilter("Move", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithAction.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Move"));
        }

        [Fact]
        public void AggregatedPropertyTreatedAsOpenProperty()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$filter", "Total"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total)"}
                });
            odataQueryOptionParser.ParseApply();
            var filterClause = odataQueryOptionParser.ParseFilter();
            filterClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
        }

        [Fact]
        public void AggregatedPropertiesTreatedAsOpenProperty()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$filter", "Total ge 10 and Max le 2"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total, StockQuantity with max as Max)"}
                });
            odataQueryOptionParser.ParseApply();
            var filterClause = odataQueryOptionParser.ParseFilter();
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            var leftBinaryOperatorNode =
                binaryOperatorNode.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual);
            var rightBinaryOperatorNode =
                binaryOperatorNode.Right.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual);
            Assert.IsType<ConvertNode>(leftBinaryOperatorNode.Left).Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
            Assert.IsType<ConvertNode>(rightBinaryOperatorNode.Left).Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Max");
        }

        [Fact]
        public void AggregatedPropertyTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "Total asc"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total)"}
                });
            odataQueryOptionParser.ParseApply();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
        }

        [Fact]
        public void ComputedPropertyTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "DoubleTotal asc"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total)/compute(Total mul 2 as DoubleTotal)"}
                });
            odataQueryOptionParser.ParseApply();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("DoubleTotal");
        }

        [Fact]
        public void DollarComputedPropertyTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "DoubleTotal asc"},
                    {"$compute", "FavoriteNumber mul 2 as DoubleTotal"}
                });
            odataQueryOptionParser.ParseCompute();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("DoubleTotal");
        }

        [Fact]
        public void DollarComputedPropertyTreatedAsOpenPropertyInFilter()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$filter", "DoubleTotal gt 10"},
                    {"$compute", "FavoriteNumber mul 2 as DoubleTotal"}
                });
            odataQueryOptionParser.ParseCompute();
            var filterClause = odataQueryOptionParser.ParseFilter();
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
            var cvNode = Assert.IsType<ConvertNode>(binaryOperatorNode.Left);
            cvNode.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("DoubleTotal");
        }

        [Fact]
        public void DollarComputedAndApplyPropertiesTreatedAsOpenPropertyInFilter()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$filter", "DoubleTotal gt Total"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total)"},
                    {"$compute", "Total mul 2 as DoubleTotal"}
                });
            odataQueryOptionParser.ParseApply();
            odataQueryOptionParser.ParseCompute();
            var filterClause = odataQueryOptionParser.ParseFilter();
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
            binaryOperatorNode.Left.ShouldBeSingleValueOpenPropertyAccessQueryNode("DoubleTotal");
            binaryOperatorNode.Right.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
        }

        [Fact]
        public void AggregatedPropertiesTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "Total asc, Max desc"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total, StockQuantity with max as Max)"}
                });
            odataQueryOptionParser.ParseApply();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
            orderByClause = orderByClause.ThenBy;
            Assert.Equal(OrderByDirection.Descending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Max");
        }

        [Fact]
        public void AggregatedAndComputePropertiesTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "DoubleTotal asc, Total desc"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total)/compute(Total mul 2 as DoubleTotal)"}
                });
            odataQueryOptionParser.ParseApply();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("DoubleTotal");
            orderByClause = orderByClause.ThenBy;
            Assert.Equal(OrderByDirection.Descending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
        }

        [Fact]
        public void NavigationPropertiesTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "MyDog/Color"},
                    {"$apply", "groupby((MyDog/Color))"}
                });
            odataQueryOptionParser.ParseApply();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp());
        }

        [Fact]
        public void MultipleComputePropertiesTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "DoubleTotal1 asc, DoubleTotal2 desc"},
                    {"$apply", "aggregate(FavoriteNumber with sum as Total)/compute(Total mul 2 as DoubleTotal1)/compute(DoubleTotal1 mul 2 as DoubleTotal2)"}
                });
            odataQueryOptionParser.ParseApply();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("DoubleTotal1");
            orderByClause = orderByClause.ThenBy;
            Assert.Equal(OrderByDirection.Descending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("DoubleTotal2");
        }


        [Fact]
        public void GroupedByComputePropertiesTreatedAsOpenPropertyInOrderBy()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "Median asc"},
                    {"$apply", "compute(1 as Median)/groupby((Median))"}
                });
            odataQueryOptionParser.ParseApply();
            var orderByClause = odataQueryOptionParser.ParseOrderBy();
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Median");
        }

        [Fact]
        public void ReferenceComputeAliasCreatedBeforeAggrageteThrows()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$orderby", "DoubleFavorite asc"},
                    {"$apply", "compute(FavoriteNumber mul 2 as DoubleFavorite)/aggregate(DoubleFavorite with sum as Total)"}
                });
            Action parseAction = () => { odataQueryOptionParser.ParseApply(); odataQueryOptionParser.ParseOrderBy(); };
            parseAction.Throws<ODataException>(ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue("DoubleFavorite"));
        }

        [Fact]
        public void ActionsThrowOnClosedInOrderby()
        {
            Action parseWithAction = () => ParseOrderBy("Move asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithAction.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Move"));
        }

        [Fact]
        public void ActionIsTreatedAsOpenProperty()
        {
            var filterClause = ParseFilter("Restore", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet());
            filterClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Restore");
        }

        [Fact]
        public void ActionsSucceedOnOpenTypeInOrderby()
        {
            var orderByClause = ParseOrderBy("Restore asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet());
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Restore");
        }

        [Fact]
        public void ActionBoundToComplexThrows()
        {
            Action parse = () => ParseFilter("MyAddress/ChangeState", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>("Could not find a property named 'ChangeState' on type 'Fully.Qualified.Namespace.Address'.");
        }

        [Fact]
        public void FunctionCallWithGeometryAndNullParameterValuesShouldWorkInOrderBy()
        {
            var point = GeometryPoint.Create(1, 2);
            var orderByClause = ParseOrderBy("Fully.Qualified.Namespace.GetColorAtPosition(position=geometry'" + SpatialHelpers.WriteSpatial(point) + "',includeAlpha=null)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet());
            orderByClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetColorAtPositionFunction())
                .ShouldHaveConstantParameter("position", point)
                .ShouldHaveConstantParameter("includeAlpha", (object)null);
        }

        [Fact]
        public void FunctionCallWithGeographyAndNullParameterValuesShouldWorkInFilter()
        {
            var point = GeographyPoint.Create(1, 2);
            var filterClause = ParseFilter("Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)/any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeAnyQueryNode()
                .Source.ShouldBeCollectionResourceFunctionCallNode(HardCodedTestModel.GetNearbyPriorAddressesFunction())
                    .ShouldHaveConstantParameter("currentLocation", point)
                    .ShouldHaveConstantParameter("limit", (object)null);
        }

        [Fact]
        public void LongFunctionChain()
        {
            // in this case all I really care about is that it doesn't throw... baselining this is overkill.
            Action parseLongFunctionChain = () => ParseFilter("Fully.Qualified.Namespace.AllMyFriendsDogs()/Fully.Qualified.Namespace.OwnerOfFastestDog()/MyDog/MyPeople/Fully.Qualified.Namespace.AllHaveDog(inOffice=true) eq true", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseLongFunctionChain.DoesNotThrow();
        }

        [Fact]
        public void FunctionBindingFailsIfParameterNameIsIncorrect()
        {
            Action parseWithIncorrectName = () => ParseFilter("Fully.Qualified.Namespace.HasDog(inOfFiCe=true)", HardCodedTestModel.TestModel, HardCodedTestModel.GetEmployeeType());
            parseWithIncorrectName.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnknownFunction("Fully.Qualified.Namespace.HasDog")); // no '...HasDog' method has parameter 'inOfFiCe'.
        }

        [Fact]
        public void FunctionWithoutBindingParameterShouldWorkInFilterOrderby()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.FindMyOwner(dogsName='fido')/Name eq 'Bob'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperator = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            binaryOperator.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp())
                .Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForFindMyOwner());
            binaryOperator.Right.ShouldBeConstantQueryNode("Bob");
        }

        [Fact]
        public void FunctionCallWithKeyExpressionShouldFail()
        {
            Action parse = () => ParseFilter("AllMyFriendsDogs(inOffice=true)(1) ne null", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError(32, "AllMyFriendsDogs(inOffice=true)(1) ne null"));
        }

        [Fact]
        public void FunctionCallWithKeyExpressionShouldFailEvenIfFunctionHasNoParameters()
        {
            Action parse = () => ParseFilter("AllMyFriendsDogs(1) ne null", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_UnknownFunction("AllMyFriendsDogs"));
        }

        [Fact]
        public void AmbiguousFunctionCallThrows()
        {
            var model = ModelBuildingHelpers.GetModelWithFunctionOverloadsWithSameParameterNames();
            Action parse = () => ParseFilter("Test.Foo(p2='1')", model, model.EntityTypes().Single(x => x.Name == "Vegetable"));
            parse.Throws<ODataException>(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound("Test.Foo", "p2"));
        }

        [Fact]
        public void FilterOnTypeDefinitionProperty()
        {
            var filter = ParseFilter("FirstName ne 'Bob'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = Assert.IsType<BinaryOperatorNode>(filter.Expression);
            bon.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String);
        }

        [Fact]
        public void OrderByTypeDefinitionProperty()
        {
            var orderBy = ParseOrderBy("FirstName", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            orderBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonFirstNameProp());
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenUInt16AndPrimitive()
        {
            FilterClause filter = ParseFilter("FavoriteNumber eq " + UInt16.MaxValue, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            Assert.Equal("Edm.Boolean", bon.TypeReference.FullName());
            Assert.Equal("Edm.Int32", Assert.IsType<ConvertNode>(bon.Left).TypeReference.FullName());
            Assert.Equal("Edm.Int32", Assert.IsType<ConstantNode>(bon.Right).TypeReference.FullName());
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenUInt32AndPrimitive()
        {
            FilterClause filter = ParseFilter("StockQuantity ne " + UInt32.MaxValue, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            Assert.Equal("Edm.Boolean", bon.TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConvertNode>(bon.Left).TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConstantNode>(bon.Right).TypeReference.FullName());
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenUInt64AndPrimitive()
        {
            FilterClause filter = ParseFilter("LifeTime ne " + UInt64.MaxValue, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            Assert.Equal("Edm.Boolean", bon.TypeReference.FullName());
            Assert.Equal("Edm.Decimal", Assert.IsType<ConvertNode>(bon.Left).TypeReference.FullName());
            Assert.Equal("Edm.Decimal", Assert.IsType<ConstantNode>(bon.Right).TypeReference.FullName());
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenPrimitiveAndUInt()
        {
            FilterClause filter = ParseFilter("123 ne StockQuantity", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            Assert.Equal("Edm.Boolean", bon.TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConstantNode>(bon.Left).TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConvertNode>(bon.Right).TypeReference.FullName());
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenUInts()
        {
            FilterClause filter = ParseFilter("FavoriteNumber eq StockQuantity", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            Assert.Equal("Edm.Boolean", bon.TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConvertNode>(bon.Left).TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConvertNode>(bon.Right).TypeReference.FullName());
        }

        [Fact]
        public void FilterWithBinaryOperatorAddBetweenPrimitiveAndUInt()
        {
            FilterClause filter = ParseFilter("123 add StockQuantity eq 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            Assert.Equal("Edm.Boolean", bon.TypeReference.FullName());
            var addExpr = Assert.IsType<BinaryOperatorNode>(bon.Left);
            Assert.Equal("Edm.Int64", Assert.IsType<ConstantNode>(addExpr.Left).TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConvertNode>(addExpr.Right).TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConstantNode>(bon.Right).TypeReference.FullName());
        }

        [Fact]
        public void FilterWithBinaryOperatorAddBetweenUInts()
        {
            FilterClause filter = ParseFilter("FavoriteNumber add StockQuantity eq 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            Assert.Equal("Edm.Boolean", bon.TypeReference.FullName());
            var addExpr = Assert.IsType<BinaryOperatorNode>(bon.Left);
            var convertNode = Assert.IsType<ConvertNode>(addExpr.Left);
            Assert.Equal("Edm.Int64", Assert.IsType<ConvertNode>(addExpr.Left).TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConvertNode>(addExpr.Right).TypeReference.FullName());
            Assert.Equal("Edm.Int64", Assert.IsType<ConstantNode>(bon.Right).TypeReference.FullName());
        }

#region operators on temporal type
        [Fact]
        public void NegateOnDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("-TimeEmployed", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var node = orderby.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate);
            Assert.True(node.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDuration(true)));
            node.Operand.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [Fact]
        public void NegateOnDurationLiteralShouldWork()
        {
            FilterClause filter = ParseFilter("-duration'PT130S' eq  TimeEmployed", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var left = expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true));
            Assert.True(left.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDuration(true)));
            left.Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                .Operand.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
            expression.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [Fact]
        public void DateConstantAddDurationShouldWork()
        {
            FilterClause filter = ParseFilter("TimeEmployed add 2010-06-10 le 2011-06-18+00:00", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual);
            var left =
                expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                    .Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add);

            Assert.True(left.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDate(true)));
            left.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .Source.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                .Source.ShouldBeConstantQueryNode(new DateTimeOffset(new DateTime(2011, 06, 18), new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void DatePropertyAddDurationShouldWork()
        {
            FilterClause filter = ParseFilter("TimeEmployed add MyDate le 2011-06-18+00:00", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual);
            var left = expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                    .Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add);
            Assert.True(left.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDate(true)));
            left.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                .Source.ShouldBeConstantQueryNode(new DateTimeOffset(new DateTime(2011, 06, 18), new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void DurationAddDateShouldWork()
        {
            FilterClause filter = ParseFilter("2010-06-10 add TimeEmployed le 2011-06-18+00:00", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual);
            var left =
                expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                    .Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add);

            Assert.True(left.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDate(true)));
            left.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .Source.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                .Source.ShouldBeConstantQueryNode(new DateTimeOffset(new DateTime(2011, 06, 18), new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void DateTimeOffsetAddDurationShouldWork()
        {
            FilterClause filter = ParseFilter("TimeEmployed add 2010-06-10 le 2011-06-18", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual);
            var left = expression.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add);
            Assert.True(left.TypeReference.AsPrimitive().AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDate(true)));
            left.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .Source.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .Source.ShouldBeConstantQueryNode(new Date(2011, 06, 18));
        }

        [Fact]
        public void DurationAddDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("TimeEmployed add -duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add);
            Assert.True(expression.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDuration(true)));
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true))
                .Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                .Operand.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateTimeOffsetSubDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("2011-06-18 sub duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract);
            Assert.True(expression.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDate(false)));
            expression.Left.ShouldBeConstantQueryNode(new Date(2011, 06, 18));
            expression.Right.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateSubDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("MyDate sub duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract);
            Assert.True(expression.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDate(false)));//PrimitiveKind().Should().Be(EdmPrimitiveTypeKind.Date);
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            expression.Right.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateSubDateShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("MyDate sub 2010-06-18", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract);
            Assert.True(expression.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDuration(false)));
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            expression.Right.ShouldBeConstantQueryNode(new Date(2010, 06, 18));
        }

        [Fact]
        public void DurationSubDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("TimeEmployed sub duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract);
            Assert.True(expression.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDuration(true)));
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true))
                .Source.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateTimeOffsetSubDateTimeOffsetShouldWork()
        {
            FilterClause filterClause = ParseFilter("2011-06-18 sub 2010-06-10 ge TimeEmployed", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual);
            var left = expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true))
                .Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract);
            Assert.True(left.TypeReference.AsPrimitive().IsEquivalentTo(EdmCoreModel.Instance.GetDuration(false)));
            left.Left.ShouldBeConstantQueryNode(new Date(2011, 06, 18));
            left.Right.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [Fact]
        public void FunctionDateWithDateTimeOffsetShouldWorkInFilter()
        {
            FilterClause filterClause = ParseFilter("MyDate ge date(2011-06-18)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual);
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("date", EdmCoreModel.Instance.GetDate(false));
            right.Parameters.Single()
                .ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(false))
                .Source.ShouldBeConstantQueryNode(new Date(2011, 6, 18));

        }

        [Fact]
        public void OrderByFunctionDateWithDateTimeOffsetShouldWork()
        {
            OrderByClause clause = ParseOrderBy("date(Birthdate)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("date", EdmCoreModel.Instance.GetDate(false));
            Assert.Equal(OrderByDirection.Ascending, clause.Direction);
        }

        [Fact]
        public void FunctionTimeWithDateTimeOffsetShouldWorkInFilter()
        {
            FilterClause filterClause = ParseFilter("MyTimeOfDay ge time(2014-09-19T12:13:14+00:00)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual);
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDayProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("time", EdmCoreModel.Instance.GetTimeOfDay(false));
            right.Parameters.Single().ShouldBeConstantQueryNode(new DateTimeOffset(2014, 9, 19, 12, 13, 14, new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void OrderByFunctionTimeWithDateTimeOffsetShouldWork()
        {
            OrderByClause clause = ParseOrderBy("time(Birthdate)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("time", EdmCoreModel.Instance.GetTimeOfDay(false));
            Assert.Equal(OrderByDirection.Ascending, clause.Direction);
        }

        [Fact]
        public void FunctionYearWithDateShouldWork()
        {
            FilterClause filterClause = ParseFilter("year(MyDate) ge year(2010-12-13)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual);
            var left = expression.Left.ShouldBeSingleValueFunctionCallQueryNode("year", EdmCoreModel.Instance.GetInt32(false));
            left.Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("year", EdmCoreModel.Instance.GetInt32(false));
            right.Parameters.Single().ShouldBeConstantQueryNode(new Date(2010, 12, 13));
        }

        [Fact]
        public void FunctionHourWithTimeOfDayShouldWork()
        {
            FilterClause filterClause = ParseFilter("hour(MyTimeOfDay) ge hour(19:20:5)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual);
            var left = expression.Left.ShouldBeSingleValueFunctionCallQueryNode("hour", EdmCoreModel.Instance.GetInt32(false));
            left.Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDayProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("hour", EdmCoreModel.Instance.GetInt32(false));
            right.Parameters.Single().ShouldBeConstantQueryNode(new TimeOfDay(19, 20, 5, 0));
        }
#endregion

#region Complex type test cases
        private static IEdmProperty homeNo = ((IEdmComplexType)HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.HomeAddress")).FindProperty("HomeNO");

        [Fact]
        public void OrderByOnComplexTypeProperty()
        {
            OrderByClause clause = ParseOrderBy("MyAddress/Fully.Qualified.Namespace.HomeAddress/NextHome/Fully.Qualified.Namespace.HomeAddress/HomeNO", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(homeNo);
        }

        [Fact]
        public void FilterOnComplexTypeProperty()
        {
            FilterClause clause = ParseFilter("MyAddress/Fully.Qualified.Namespace.HomeAddress/HomeNO eq 'dva'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            clause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Left.ShouldBeSingleValuePropertyAccessQueryNode(homeNo);
        }
#endregion

#region Primitive type cast
        [Fact]
        public void FilterWithCastStringProperty()
        {
            FilterClause filter = ParseFilter("Artist/Edm.String eq 'sdb'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            var binaryOperatorNode = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var convertNode = Assert.IsType<ConvertNode>(binaryOperatorNode.Left);
            var castNode = Assert.IsType<SingleValueCastNode>(convertNode.Source);
            Assert.Equal("Edm.String", castNode.TypeReference.FullName());
            Assert.Equal(InternalQueryNodeKind.SingleValueCast, castNode.InternalKind);
            Assert.Equal(QueryNodeKind.SingleValueCast, castNode.Kind);
            castNode.Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
        }

        [Fact]
        public void FilterWithCastStringOpenProperty()
        {
            FilterClause filter = ParseFilter("Assistant/Edm.String eq 'sdb'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            var binaryOperatorNode = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var convertNode = Assert.IsType<ConvertNode>(binaryOperatorNode.Left);
            var castNode = Assert.IsType<SingleValueCastNode>(convertNode.Source);
            Assert.Equal("Edm.String", castNode.TypeReference.FullName());
            Assert.Equal(InternalQueryNodeKind.SingleValueCast, castNode.InternalKind);
            Assert.Equal(QueryNodeKind.SingleValueCast, castNode.Kind);
            castNode.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Assistant");
        }

        [Fact]
        public void FilterWithCastAnyProperty()
        {
            FilterClause filter = ParseFilter("Colors/any(x:x/Edm.String eq 'blue')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            var anyNode = filter.Expression.ShouldBeAnyQueryNode();
            var bon = anyNode.Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var cn = Assert.IsType<ConvertNode>(bon.Left);
            var svcn = Assert.IsType<SingleValueCastNode>(cn.Source);
            Assert.Equal("Edm.String", svcn.TypeReference.FullName());
            Assert.Equal(InternalQueryNodeKind.SingleValueCast, svcn.InternalKind);
            Assert.Equal(QueryNodeKind.SingleValueCast, svcn.Kind);
            svcn.Source.ShouldBeNonResourceRangeVariableReferenceNode("x");
        }

        [Fact]
        public void FilterWithCastAnyOpenProperty()
        {
            FilterClause filter = ParseFilter("Exhibits/any(x:x/Edm.String eq 'Louvre')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            var anyNode = filter.Expression.ShouldBeAnyQueryNode();
            var bon = anyNode.Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var cn = Assert.IsType<ConvertNode>(bon.Left);
            var svcn = Assert.IsType<SingleValueCastNode>(cn.Source);

            Assert.Equal("Edm.String", svcn.TypeReference.FullName());
            Assert.Equal(InternalQueryNodeKind.SingleValueCast, svcn.InternalKind);
            Assert.Equal(QueryNodeKind.SingleValueCast, svcn.Kind);
            svcn.Source.ShouldBeNonResourceRangeVariableReferenceNode("x");
        }

        [Fact]
        public void OrderByWithCastStringProperty()
        {
            OrderByClause orderby = ParseOrderBy("Artist/Edm.String", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            SingleValueCastNode scn = Assert.IsType<SingleValueCastNode>(orderby.Expression);
            Assert.Equal("Edm.String", scn.TypeReference.FullName());
            Assert.Equal(InternalQueryNodeKind.SingleValueCast, scn.InternalKind);
            Assert.Equal(QueryNodeKind.SingleValueCast, scn.Kind);
            scn.Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
        }

        [Fact]
        public void OrderByWithCastStringOpenProperty()
        {
            OrderByClause orderby = ParseOrderBy("Assistant/Edm.String", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            SingleValueCastNode scn = Assert.IsType<SingleValueCastNode>(orderby.Expression);
            Assert.Equal("Edm.String", scn.TypeReference.FullName());
            Assert.Equal(InternalQueryNodeKind.SingleValueCast, scn.InternalKind);
            Assert.Equal(QueryNodeKind.SingleValueCast, scn.Kind);
            scn.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Assistant");
        }
#endregion

#region In Operator Tests
        [Fact]
        public void FilterWithInOperationWithPrimitiveTypeProperties()
        {
            FilterClause filter = ParseFilter("ID in RelatedIDs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            InNode inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithIntConstant()
        {
            FilterClause filter = ParseFilter("9001 in RelatedIDs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            InNode inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal(9001, Assert.IsType<ConstantNode>(inNode.Left).Value);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithStringConstant()
        {
            FilterClause filter = ParseFilter("'777-42-9001' in RelatedSSNs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            InNode inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("777-42-9001", Assert.IsType<ConstantNode>(inNode.Left).Value);
            Assert.Equal("RelatedSSNs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithMismatchedOperandTypes()
        {
            Action parse = () => ParseFilter("ID in RelatedSSNs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ArgumentException>(ODataErrorStrings.Nodes_InNode_CollectionItemTypeMustBeSameAsSingleItemType("Edm.String", "Edm.Int32"));
        }

        [Fact]
        public void FilterWithInOperationWithLogicalNotOperation()
        {
            FilterClause filter = ParseFilter("not ID in RelatedIDs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var uon = filter.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not);
            var inNode = Assert.IsType<InNode>(uon.Operand);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithNestedOperation()
        {
            FilterClause filter = ParseFilter("(ID in RelatedIDs) eq false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var inNode = Assert.IsType<InNode>(bon.Left);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithMultipleNestedOperations()
        {
            FilterClause filter = ParseFilter("((ID in RelatedIDs) eq ('777-42-9001' in RelatedSSNs))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var bon = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var inNode = Assert.IsType<InNode>(bon.Left);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithNavigationProperties()
        {
            FilterClause filter = ParseFilter("MyDog/LionWhoAteMe in MyDog/LionsISaw", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("LionWhoAteMe", Assert.IsType<SingleNavigationNode>(inNode.Left).NavigationProperty.Name);
            Assert.Equal("LionsISaw", Assert.IsType<CollectionNavigationNode>(inNode.Right).NavigationProperty.Name);
        }

        [Fact]
        public void FilterWithInOperationWithBoundFunctions()
        {
            FilterClause filter = ParseFilter("Fully.Qualified.Namespace.GetPriorAddress in Fully.Qualified.Namespace.GetPriorAddresses", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("Fully.Qualified.Namespace.GetPriorAddress", Assert.IsType<SingleValueFunctionCallNode>(inNode.Left).Name);
            Assert.Equal("Fully.Qualified.Namespace.GetPriorAddresses", Assert.IsType<CollectionResourceFunctionCallNode>(inNode.Right).Name);
        }

        [Fact]
        public void FilterWithInOperationWithComplexTypeProperties()
        {
            FilterClause filter = ParseFilter("GeographyPoint in GeographyCollection", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("GeographyPoint", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("GeographyCollection", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithDerivedTypeCollection()
        {
            FilterClause filter = ParseFilter("Geography in GeographyCollection", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("Geography", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("GeographyCollection", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithDerivedTypeSingleValue()
        {
            FilterClause filter = ParseFilter("GeographyPoint in GeographyParentCollection", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("GeographyPoint", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("GeographyParentCollection", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithEnums()
        {
            FilterClause filter = ParseFilter("Fully.Qualified.Namespace.ColorPattern'SolidYellow' in FavoriteColors", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern'SolidYellow'", Assert.IsType<ConstantNode>(inNode.Left).LiteralText);
            Assert.Equal("FavoriteColors", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithAny()
        {
            // https://github.com/OData/odata.net/issues/1447
            FilterClause filter = ParseFilter("Colors/any(x:x/Edm.String in ('Blue','Red'))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            var anyNode = filter.Expression.ShouldBeAnyQueryNode();
            var inNode = anyNode.Body.ShouldBeInNode();
            var svcn = Assert.IsType<SingleValueCastNode>(inNode.Left);

            Assert.Equal("Edm.String", svcn.TypeReference.FullName());
            Assert.Equal(InternalQueryNodeKind.SingleValueCast, svcn.InternalKind);
            Assert.Equal(QueryNodeKind.SingleValueCast, svcn.Kind);
            svcn.Source.ShouldBeNonResourceRangeVariableReferenceNode("x");

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal("('Blue','Red')", collectionNode.LiteralText);
            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("Blue");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("Red");
        }

        [Fact]
        public void FilterWithInOperationWithParensCollection()
        {
            FilterClause filter = ParseFilter("ID in (1,2,3)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(1,2,3)", Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
        }

        [Fact]
        public void FilterWithInOperationWithParensCollectionAndLogicalNotOperator()
        {
            FilterClause filter = ParseFilter("not ID in (1,2,3)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var uon = filter.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not);
            var inNode = Assert.IsType<InNode>(uon.Operand);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(1,2,3)", Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
        }

        [Theory]
        [InlineData("('abc','xyz')")]
        [InlineData("(  'abc',      'xyz'  )")]
        [InlineData("(\"abc\",\"xyz\")")]  // for backward compatibility
        [InlineData("(  \"abc\",     \"xyz\"   )")]
        public void FilterWithInOperationWithParensStringCollection(string collection)
        {
            string filterClause = $"SSN in {collection}";
            FilterClause filter = ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(collection, collectionNode.LiteralText);
            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("abc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("xyz");
        }

        [Theory]
        [InlineData("('abc','xyz')")]
        [InlineData("(  'abc',      'xyz'  )")]
        [InlineData("(\"abc\",\"xyz\")")]  // for backward compatibility
        [InlineData("(  \"abc\",     \"xyz\"   )")]
        public void FilterWithInOperationWithParensStringCollectionAndLogicalNotOperator(string collection)
        {
            string filterClause = $"not SSN in {collection}";
            FilterClause filter = ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var uon = filter.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not);
            var inNode = Assert.IsType<InNode>(uon.Operand);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(collection, collectionNode.LiteralText);
            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("abc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("xyz");
        }

        [Theory]
        [InlineData("('abc'def, 'xy,z')")]
        [InlineData("('ab c',    def, 'xy,z')")]
        [InlineData("('xy,z', 'abc'd)")]
        [InlineData("('xy,z', 'abc'  def)")]
        public void FilterWithInOperationWithMalformCollection(string collection)
        {
            string filterClause = $"SSN in {collection}";
            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(ODataErrorStrings.StringItemShouldBeQuoted("d"));
        }

        [Fact]
        public void FilterWithInOperationWithParensStringCollection_EscapedSingleQuote()
        {
            FilterClause filter = ParseFilter("SSN in ('a''bc','''def','xyz''')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal("('a''bc','''def','xyz''')", collectionNode.LiteralText);
            Assert.Equal(3, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("a'bc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("'def");
            collectionNode.Collection.ElementAt(2).ShouldBeConstantQueryNode("xyz'");
        }

        [Fact]
        public void FilterWithInOperationWithParensStringCollection_DoubleQuoteInSingleQuoteString()
        {
            FilterClause filter = ParseFilter("SSN in ('a\"\t\u00A9bc')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal("('a\"\t\u00A9bc')", collectionNode.LiteralText);
            ConstantNode constantNode = Assert.Single(collectionNode.Collection);
            constantNode.ShouldBeConstantQueryNode("a\"\t©bc");
        }

        [Fact]
        public void FilterWithInOperationWithParensStringCollection_EscapedDoubleQuoteInDoubleQuoteString()
        {
            FilterClause filter = ParseFilter("SSN in (\"a\\\"\\tbc\", \"x\\u00A9\")", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal("(\"a\\\"\\tbc\", \"x\\u00A9\")", collectionNode.LiteralText);

            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("a\"\tbc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("x©");
        }

        [Fact]
        public void FilterWithInOperationWithParensStringCollection_WithCommaInValue()
        {
            FilterClause filter = ParseFilter("SSN in (  'a' , 'x,  y,z ')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal("(  'a' , 'x,  y,z ')", collectionNode.LiteralText);
            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("a");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("x,  y,z ");
        }

        [Fact]
        public void FilterWithInOperationWithParensStringCollection_SlashesInDoubleQuotedStringLiterals()
        {
            FilterClause filter = ParseFilter(@"SSN in (""a\b\\kc""   , ""x \t\\t'' /y "")", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal("(\"a\\b\\\\kc\"   , \"x \\t\\\\t'' /y \")", collectionNode.LiteralText);
            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("a\b\\kc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("x \t\\t'' /y ");
        }

        [Fact]
        public void FilterWithInOperationWithParensStringCollection_SlashesInSingleQuotedStringLiterals()
        {
            FilterClause filter = ParseFilter(@"SSN in ('a\b\\bc', 'd\ff''\t','xy/z''')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal("('a\\b\\\\bc', 'd\\ff''\\t','xy/z''')", collectionNode.LiteralText);
            Assert.Equal(3, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("a\\b\\\\bc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("d\\ff'\\t");
            collectionNode.Collection.ElementAt(2).ShouldBeConstantQueryNode("xy/z'");
        }


        [Theory]
        [InlineData(@"'a\b\\bc'")]
        [InlineData(@"'a''b''''bc'")]
        [InlineData(@"'a,b,bc'")]
        public void FilterWithInOperationShouldMatchEquals_SlashesInSingleQuotedStringLiterals(string constantString)
        {
            FilterClause inFilter = ParseFilter(String.Format(@"SSN in ({0})", constantString), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            FilterClause eqFilter = ParseFilter(String.Format(@"SSN eq {0}", constantString), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(inFilter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(1, collectionNode.Collection.Count);

            var eqNode = Assert.IsType<BinaryOperatorNode>(eqFilter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(eqNode.Left).Property.Name);
            ConstantNode constantNode = Assert.IsType<ConstantNode>(eqNode.Right);

            Assert.Equal(collectionNode.Collection.ElementAt(0).Value, constantNode.Value);
        }

        [Theory]
        [InlineData(@"\abc")]
        [InlineData(@"\\abc")]
        [InlineData(@"\\\abc")]
        [InlineData(@"\\\\abc")]
        public void BackslashTests(string constantString)
        {
            string query = String.Format(@"SSN in ('{0}')", constantString);

            FilterClause inFilter = ParseFilter(query, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(inFilter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(1, collectionNode.Collection.Count);

            Assert.Equal(constantString, collectionNode.Collection.ElementAt(0).Value);

            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(String.Format(@"People?$filter={0}",query), UriKind.Relative));
            var uri = parser.ParseUri().BuildUri(ODataUrlKeyDelimiter.Slash);
            var uriString = Uri.UnescapeDataString(uri.OriginalString);
        }

        [Theory]
        [InlineData("SSN in (null, 'abc')")]
        [InlineData("SSN in (    null  , 'abc')")]
        [InlineData("SSN in ( null , \"abc\")")]
        public void FilterWithInOperationWithParensStringCollection_NullAtStartOfStringLiterals(string inLiteral)
        {
            FilterClause filter = ParseFilter(inLiteral, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode<string>(null);
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode("abc");
        }

        [Theory]
        [InlineData("SSN in ('abc', null)")]
        [InlineData("SSN in ('abc', null    )")]
        [InlineData("SSN in (\"abc\", null    )")]
        public void FilterWithInOperationWithParensStringCollection_NullAtEndOfStringLiterals(string inLiteral)
        {
            FilterClause filter = ParseFilter(inLiteral, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(2, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("abc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode<string>(null);
        }

        [Theory]
        [InlineData("SSN in ('abc', null, 'null')")]
        [InlineData("SSN in (  'abc',   null,   'null'    )")]
        [InlineData("SSN in (\"abc\", null , \"null\"   )")]
        public void FilterWithInOperationWithParensStringCollection_NullAtMiddleOfStringLiterals(string inLiteral)
        {
            FilterClause filter = ParseFilter(inLiteral, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(3, collectionNode.Collection.Count);
            collectionNode.Collection.ElementAt(0).ShouldBeConstantQueryNode("abc");
            collectionNode.Collection.ElementAt(1).ShouldBeConstantQueryNode<string>(null);
            collectionNode.Collection.ElementAt(2).ShouldBeConstantQueryNode("null");
        }

        [Theory]
        [InlineData("(nu ll)", "nu ll")]
        [InlineData("(Null)", "N")]
        [InlineData("('abc', Null)", "N")]
        [InlineData("('abc', n  ull)", "n  ull")]
        [InlineData("('abc', Null , 'xyz')", "N")]
        [InlineData("('abc', n  ull, 'xyz')", "n  ull")]
        [InlineData("('abc', n)", "n")]
        public void FilterWithInOperationWithInvalidNullLiteral(string collection, string error)
        {
            string filterClause = $"SSN in {collection}";
            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(ODataErrorStrings.StringItemShouldBeQuoted(error));
        }

        [Fact]
        public void FilterWithEqOperation_EscapedSingleQuote()
        {
            FilterClause filter = ParseFilter("SSN eq 'a''bc'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var bon = Assert.IsType<BinaryOperatorNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(bon.Left).Property.Name);
            bon.Right.ShouldBeConstantQueryNode("a'bc");
        }

        [Fact]
        public void FilterWithEqOperation_EmptyString()
        {
            FilterClause filter = ParseFilter("SSN eq ''", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var bon = Assert.IsType<BinaryOperatorNode>(filter.Expression);
            Assert.Equal("SSN", Assert.IsType<SingleValuePropertyAccessNode>(bon.Left).Property.Name);
            bon.Right.ShouldBeConstantQueryNode("");
        }

        [Fact]
        public void FilterWithInOperationWithBracketedCollection()
        {
            FilterClause filter = ParseFilter("ID in [1,2,3]", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("[1,2,3]", Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
        }

        [Theory]
        [InlineData("ID in ()")]
        [InlineData("ID in (  )")]
        [InlineData("SSN in (  )")]     // Edm.String
        [InlineData("MyGuid in (  )")]  // Edm.Guid
        [InlineData("Birthdate in (  )")]  // Edm.DateTimeOffset
        [InlineData("MyDate in (  )")]  // Edm.Date
        public void FilterWithInOperationWithEmptyCollection(string filterClause)
        {
            FilterClause filter = ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(0, collectionNode.Collection.Count);
        }

        [Theory]
        [InlineData("SSN in ('')")]     // Edm.String
        [InlineData("SSN in ( '' )")]     // Edm.String
        [InlineData("SSN in (\"\")")]     // Edm.String
        [InlineData("SSN in ( \"\" )")]     // Edm.String
        public void FilterWithInOperationWithEmptyString(string filterClause)
        {
            FilterClause filter = ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);
            Assert.Equal(1, collectionNode.Collection.Count);

            ConstantNode constantNode = collectionNode.Collection.First();
            Assert.Equal("\"\"", constantNode.LiteralText);
        }

        [Theory]
        [InlineData("SSN in ( ' ' )", " ")]     // 1 space
        [InlineData("SSN in ( '   ' )", "   ")]     // 3 spaces
        [InlineData("SSN in ( \"  \" )", "  ")]     // 2 spaces
        [InlineData("SSN in ( \"    \" )", "    ")]     // 4 spaces
        public void FilterWithInOperationWithWhitespace(string filterClause, string expectedLiteralText)
        {
            FilterClause filter = ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);

            // A single whitespace or multiple whitespaces are valid literals
            Assert.Equal(1, collectionNode.Collection.Count);

            ConstantNode constantNode = collectionNode.Collection.First();
            Assert.Equal(expectedLiteralText, constantNode.LiteralText);
        }

        [Theory]
        [InlineData("SSN in ( '', ' ' )")]     // Edm.String
        [InlineData("SSN in ( \"\", \" \" )")]     // Edm.String
        [InlineData("SSN in ( '', \" \" )")]     // Edm.String
        [InlineData("SSN in ( \"\", ' ' )")]     // Edm.String
        public void FilterWithInOperationWithEmptyStringAndWhitespace(string filterClause)
        {
            FilterClause filter = ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);

            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(inNode.Right);

            // A single whitespace or multiple whitespaces are valid literals
            Assert.Equal(2, collectionNode.Collection.Count);
        }

        [Theory]
        [InlineData("MyGuid in ( '' )", "")]  // Edm.Guid
        [InlineData("MyGuid in ( '  ' )", "  ")]  // Edm.Guid
        [InlineData("MyGuid in ( \" \" )", " ")]  // Edm.Guid
        public void FilterWithInOperationGuidWithEmptyQuotesThrows(string filterClause, string quotedString)
        {
            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue(quotedString, "Edm.Guid"));
        }

        [Theory]
        [InlineData("Birthdate in ( '' )", "")]  // Edm.DateTimeOffset
        [InlineData("Birthdate in ( \" \" )", " ")]  // Edm.DateTimeOffset
        [InlineData("Birthdate in ('   ')", "   ")]  // Edm.DateTimeOffset
        public void FilterWithInOperationDateTimeOffsetWithEmptyQuotesThrows(string filterClause, string quotedString)
        {
            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue(quotedString, "Edm.DateTimeOffset"));
        }

        [Theory]
        [InlineData("MyDate in ( '' )", "")]  // Edm.Date
        [InlineData("MyDate in ( \" \" )", " ")]  // Edm.Date
        [InlineData("MyDate in ('   ')", "   ")]  // Edm.Date
        public void FilterWithInOperationDateWithEmptyQuotesThrows(string filterClause, string quotedString)
        {
            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue(quotedString, "Edm.Date"));
        }

        [Theory]
        [InlineData("('D01663CF-EB21-4A0E-88E0-361C10ACE7FD', '','492CF54A-84C9-490C-A7A4-B5010FAD8104')")]
        [InlineData("('D01663CF-EB21-4A0E-88E0-361C10ACE7FD', \"\",'492CF54A-84C9-490C-A7A4-B5010FAD8104')")]
        public void FilterWithInOperationWithQuotedGuidCollectionWithInvalidValuesThrows(string guidsCollection)
        {
            string filterClause = $"MyGuid in {guidsCollection}";

            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue("", "Edm.Guid"));
        }

        [Theory]
        [InlineData("(1950-01-02T06:15:00Z, '',1977-09-16T15:00:00+05:00)")]
        [InlineData("(1950-01-02T06:15:00Z, \"\",1977-09-16T15:00:00+05:00)")]
        public void FilterWithInOperationWithQuotedDateTimeOffsetCollectionWithInvalidValuesThrows(string dateTimeOffsetCollection)
        {
            string filterClause = $"Birthdate in {dateTimeOffsetCollection}";

            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue("", "Edm.DateTimeOffset"));
        }

        [Theory]
        [InlineData("(1950-01-02, '',1977-09-16)")]
        [InlineData("(1950-01-02, \"\",1977-09-16)")]
        public void FilterWithInOperationWithQuotedDateCollectionWithInvalidValuesThrows(string dateCollection)
        {
            string filterClause = $"MyDate in {dateCollection}";

            Action parse = () => ParseFilter(filterClause, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue("", "Edm.Date"));
        }

        [Fact]
        public void FilterWithInOperationWithMismatchedClosureCollection()
        {
            Action parse = () => ParseFilter("ID in (1,2,3]", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
        }

        [Fact]
        public void OrderByWithInOperationWithPrimitiveTypeProperties()
        {
            OrderByClause orderby = ParseOrderBy("ID in RelatedIDs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithPrimitiveTypePropertiesWithLogicalNotOperator()
        {
            OrderByClause orderby = ParseOrderBy("not ID in RelatedIDs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var uon = orderby.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not);
            var inNode = Assert.IsType<InNode>(uon.Operand);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithIntConstant()
        {
            OrderByClause orderby = ParseOrderBy("9001 in RelatedIDs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal(9001, Assert.IsType<ConstantNode>(inNode.Left).Value);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithStringConstant()
        {
            OrderByClause orderby = ParseOrderBy("'777-42-9001' in RelatedSSNs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("777-42-9001", Assert.IsType<ConstantNode>(inNode.Left).Value);
            Assert.Equal("RelatedSSNs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void FilterWithInOperationWithGuidCollection()
        {
            FilterClause filter = ParseFilter("MyGuid in (D01663CF-EB21-4A0E-88E0-361C10ACE7FD, 492CF54A-84C9-490C-A7A4-B5010FAD8104, null)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("MyGuid", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(D01663CF-EB21-4A0E-88E0-361C10ACE7FD, 492CF54A-84C9-490C-A7A4-B5010FAD8104, null)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
        }

        [Theory]
        [InlineData("('D01663CF-EB21-4A0E-88E0-361C10ACE7FD','492CF54A-84C9-490C-A7A4-B5010FAD8104')")]
        [InlineData("(\"D01663CF-EB21-4A0E-88E0-361C10ACE7FD\", \"492CF54A-84C9-490C-A7A4-B5010FAD8104\")")]
        [InlineData("(\"D01663CF-EB21-4A0E-88E0-361C10ACE7FD\",\"492CF54A-84C9-490C-A7A4-B5010FAD8104\")")]
        public void FilterWithInOperationWithQuotedGuidCollection(string guidsCollection)
        {
            FilterClause filter = ParseFilter($"MyGuid in {guidsCollection}",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("MyGuid", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal(guidsCollection, Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
        }

        [Fact]
        public void FilterWithBinaryOperationWithGuid()
        {
            FilterClause filter = ParseFilter("MyGuid eq D01663CF-EB21-4A0E-88E0-361C10ACE7FD",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var bon = Assert.IsType<BinaryOperatorNode>(filter.Expression);
            Assert.Equal("MyGuid", Assert.IsType<SingleValuePropertyAccessNode>(bon.Left).Property.Name);

            ConstantNode cNode = Assert.IsType<ConstantNode>(Assert.IsType<ConvertNode>(bon.Right).Source);
            Assert.Equal(Guid.Parse("D01663CF-EB21-4A0E-88E0-361C10ACE7FD"), cNode.Value);
        }

        [Fact]
        public void OrderByWithInOperationWithMismatchedOperandTypes()
        {
            Action parse = () => ParseOrderBy("ID in RelatedSSNs", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ArgumentException>(
                ODataErrorStrings.Nodes_InNode_CollectionItemTypeMustBeSameAsSingleItemType("Edm.String", "Edm.Int32"));
        }

        [Fact]
        public void OrderByWithInOperationWithNestedOperation()
        {
            OrderByClause orderby = ParseOrderBy("(ID in RelatedIDs) eq false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var bon = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            var inNode = Assert.IsType<InNode>(bon.Left);

            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithMultipleNestedOperations()
        {
            OrderByClause orderby = ParseOrderBy("((ID in RelatedIDs) eq ('777-42-9001' in RelatedSSNs))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var bon = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            var inNode = Assert.IsType<InNode>(bon.Left);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("RelatedIDs", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithNavigationProperties()
        {
            OrderByClause orderby = ParseOrderBy("MyDog/LionWhoAteMe in MyDog/LionsISaw", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("LionWhoAteMe", Assert.IsType<SingleNavigationNode>(inNode.Left).NavigationProperty.Name);
            Assert.Equal("LionsISaw", Assert.IsType<CollectionNavigationNode>(inNode.Right).NavigationProperty.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithBoundFunctions()
        {
            OrderByClause orderby = ParseOrderBy("Fully.Qualified.Namespace.GetPriorAddress in Fully.Qualified.Namespace.GetPriorAddresses", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("Fully.Qualified.Namespace.GetPriorAddress", Assert.IsType<SingleValueFunctionCallNode>(inNode.Left).Name);
            Assert.Equal("Fully.Qualified.Namespace.GetPriorAddresses", Assert.IsType<CollectionResourceFunctionCallNode>(inNode.Right).Name);
        }

        [Fact]
        public void OrderByWithInOperationWithComplexTypeProperties()
        {
            OrderByClause orderby = ParseOrderBy("GeographyPoint in GeographyCollection", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("GeographyPoint", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("GeographyCollection", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithEnums()
        {
            OrderByClause orderby = ParseOrderBy("Fully.Qualified.Namespace.ColorPattern'SolidYellow' in FavoriteColors", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern'SolidYellow'", Assert.IsType<ConstantNode>(inNode.Left).LiteralText);
            Assert.Equal("FavoriteColors", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithDerivedTypeCollection()
        {
            OrderByClause orderby = ParseOrderBy("Geography in GeographyCollection", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("Geography", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("GeographyCollection", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithDerivedTypeSingleValue()
        {
            OrderByClause orderby = ParseOrderBy("GeographyPoint in GeographyParentCollection", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("GeographyPoint", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("GeographyParentCollection", Assert.IsType<CollectionPropertyAccessNode>(inNode.Right).Property.Name);
        }

        [Fact]
        public void OrderByWithInOperationWithParensCollection()
        {
            OrderByClause orderby = ParseOrderBy("ID in (1,2,3)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(1,2,3)", Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
        }

        [Fact]
        public void OrderByWithInOperationWithBracketedCollection()
        {
            OrderByClause orderby = ParseOrderBy("ID in [1,2,3]", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            var inNode = Assert.IsType<InNode>(orderby.Expression);
            Assert.Equal("ID", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("[1,2,3]", Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
        }

        [Fact]
        public void OrderByWithInOperationWithMismatchedClosureCollection()
        {
            Action parse = () => ParseOrderBy("ID in (1,2,3]", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            parse.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
        }
        
        [Fact]
        public void FilterWithInOperationWithDateTimeOffsetCollection()
        {
            FilterClause filter = ParseFilter("Birthdate in (1950-01-02T06:15:00Z, 1977-09-16T15:00:00+05:00)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("Birthdate", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(1950-01-02T06:15:00Z, 1977-09-16T15:00:00+05:00)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[]{new DateTimeOffset(1950, 1, 2, 6, 15, 0, TimeSpan.Zero), 
                    new DateTimeOffset(1977, 9, 16, 15, 0, 0, TimeSpan.FromHours(5))},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
        
        [Fact]
        public void FilterWithInOperationWithDateCollection()
        {
            FilterClause filter = ParseFilter("MyDate in (1950-01-02, 1977-09-16)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("MyDate", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(1950-01-02, 1977-09-16)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[]{new Date(1950, 1, 2), new Date(1977, 9, 16)},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
        
        [Fact]
        public void FilterWithInOperationWithTimeOfDayCollection()
        {
            FilterClause filter = ParseFilter("MyTimeOfDay in (12:00:00, 08:00:01)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("MyTimeOfDay", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(12:00:00, 08:00:01)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[]{new TimeOfDay(12, 0, 0, 0), new TimeOfDay(8, 0, 1, 0)},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
        
        [Fact]
        public void FilterWithInOperationWithDurationCollection()
        {
            FilterClause filter = ParseFilter("TimeEmployed in (duration'PT2H47M30S', duration'PT2H46M40S')",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("TimeEmployed", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(duration'PT2H47M30S', duration'PT2H46M40S')",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[]{new TimeSpan(2, 47, 30), new TimeSpan(2, 46, 40)},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
        
        [Fact]
        public void FilterWithInOperationWithDateTimeOffsetCollectionContainingNulls()
        {
            FilterClause filter = ParseFilter("FavoriteDate in (1950-01-02T06:15:00Z, null)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("FavoriteDate", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(1950-01-02T06:15:00Z, null)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[]{new DateTimeOffset(1950, 1, 2, 6, 15, 0, TimeSpan.Zero), 
                    null},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
        
        [Fact]
        public void FilterWithInOperationWithDurationCollectionContainingNulls()
        {
            FilterClause filter = ParseFilter("TimeEmployed in (duration'PT2H47M30S', null)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("TimeEmployed", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(duration'PT2H47M30S', null)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[]{new TimeSpan(2, 47, 30), null},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
        
        [Fact]
        public void FilterWithInOperationWithDateTimeOffsetCollectionContainingSingleItem()
        {
            FilterClause filter = ParseFilter("FavoriteDate in (1950-01-02T06:15:00Z)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("FavoriteDate", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(1950-01-02T06:15:00Z)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[]{new DateTimeOffset(1950, 1, 2, 6, 15, 0, TimeSpan.Zero)},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
        
        [Fact]
        public void FilterWithInOperationWithDateTimeOffsetCollectionContainingSingleNull()
        {
            FilterClause filter = ParseFilter("FavoriteDate in (null)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var inNode = Assert.IsType<InNode>(filter.Expression);
            Assert.Equal("FavoriteDate", Assert.IsType<SingleValuePropertyAccessNode>(inNode.Left).Property.Name);
            Assert.Equal("(null)",
                Assert.IsType<CollectionConstantNode>(inNode.Right).LiteralText);
            Assert.Equal(new object[] {null},
                Assert.IsType<CollectionConstantNode>(inNode.Right).Collection.Select(x => x.Value));
        }
#endregion

        private static FilterClause ParseFilter(string text, IEdmModel edmModel, IEdmType edmType, IEdmNavigationSource edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmType, edmEntitySet, new Dictionary<string, string>() { { "$filter", text } }) { Resolver = new ODataUriResolver() { EnableCaseInsensitive = false } }.ParseFilter();
        }

        private static OrderByClause ParseOrderBy(string text, IEdmModel edmModel, IEdmType edmType, IEdmNavigationSource edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmType, edmEntitySet, new Dictionary<string, string>() { { "$orderby", text } }).ParseOrderBy();
        }
    }
}
