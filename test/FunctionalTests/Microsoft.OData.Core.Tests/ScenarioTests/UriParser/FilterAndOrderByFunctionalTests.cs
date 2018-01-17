//---------------------------------------------------------------------
// <copyright file="FilterAndOrderByFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
        [Fact]
        public void ParseFilterLongValuesWithOptionalSuffix()
        {
            var filterQueryNode = ParseFilter("ID eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID eq 123L", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID eq " + ((long)int.MaxValue + 10), HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode((long)int.MaxValue + 10);
            filterQueryNode = ParseFilter("ID eq NaN", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID add 123 eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID add 123L eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterLongValuesWithOptionalSuffixUsingContainedEntitySet()
        {
            var filterQueryNode = ParseFilter("ID eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID eq 123L", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID eq " + ((long)int.MaxValue + 10), HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            ((ConstantNode)((BinaryOperatorNode)filterQueryNode.Expression).Right).ShouldBeConstantQueryNode((long)int.MaxValue + 10);
            filterQueryNode = ParseFilter("ID eq NaN", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID add 123 eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID add 123L eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetContainedDogEntitySet());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterLongValuesNeedPromotion()
        {
            var filterQueryNode = ParseFilter("ID eq 1F", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID eq 1D", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID eq 1M", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterFloatValuesWithOptionalSuffix()
        {
            var filterQueryNode = ParseFilter("ID eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID eq 123F", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID add 123 eq 123F", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID add 123F eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterFloatValuesNeedPromotion()
        {
            var filterQueryNode = ParseFilter("ID eq 123L", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID eq 123D", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            //Non-ConstantNode "ID" whose type is float cannot be promoted to Decimal
            Action parse = () => ParseFilter("ID eq 123M", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Single", "Edm.Decimal", "Equal"));

            string decimalPrecisionStr = "3258.678765765489753678965390";
            parse = () => ParseFilter("ID eq " + decimalPrecisionStr, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Single", "Edm.Decimal", "Equal"));
        }

        [Fact]
        public void ParseFilterDoubleValuesWithOptionalSuffix()
        {
            string decimalPrecisionStr = "3258.678765765489753678965390";
            var filterQueryNode = ParseFilter("ID eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID eq 123D", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID add 123 eq 123D", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filterQueryNode = ParseFilter("ID add 123D eq 123", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            // double and (implicit) decimal are incompatible
            Action parse = () => ParseFilter("ID eq " + decimalPrecisionStr, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Equal"));
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
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Equal"));
        }

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
            parse.ShouldThrow<OverflowException>();

            // numeric string overflowing all types:
            parse = () => ParseFilter("1.79769313486232E+30700 eq " + decimalPrecisionStr, HardCodedTestModel.TestModel, HardCodedTestModel.GetPet3Type(), HardCodedTestModel.GetPet3Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidNumericString("1.79769313486232E+30700"));
        }

        [Fact]
        public void ParseFilterDecimalValuesNeedPromotion()
        {
            var filterQueryNode = ParseFilter("ID eq 123L", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID eq 123F", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            filterQueryNode = ParseFilter("ID eq 123D", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet4Type(), HardCodedTestModel.GetPet4Set());
            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void ParseFilterPropertyAndConstantsWithOptionalSuffix()
        {
            var filterQueryNode = ParseFilter("2 gt 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2);

            filterQueryNode = ParseFilter("2 gt 2l", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2L);

            filterQueryNode = ParseFilter("2 gt 2.2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2F);

            filterQueryNode = ParseFilter("2 gt 2.2d", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2D);

            filterQueryNode = ParseFilter("2 gt 2.34243223423235234423400003", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2m);
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2.34243223423235234423400003m);

            filterQueryNode = ParseFilter("2.2 gt 2.2D", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2.2D);

            filterQueryNode = ParseFilter("2D gt 2.34243223423235234423400003", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2m);
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2.34243223423235234423400003m);

            filterQueryNode = ParseFilter("2F gt 2.34243223423235234423400003M", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(2m);
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2.34243223423235234423400003m);

            filterQueryNode = ParseFilter("SingleID eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2f);

            filterQueryNode = ParseFilter("DoubleID eq 2.01", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(2.01d);

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
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Single", "Edm.Decimal", "Equal"));

            parse = () => ParseFilter("DoubleID eq DecimalID", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Equal"));

            parse = () => ParseFilter("SingleID eq DecimalID", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Single", "Edm.Decimal", "Equal"));
        }

        [Fact]
        public void ParseFilterINFNaNWithoutSuffix()
        {
            var filterQueryNode = ParseFilter("ID gt INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            filterQueryNode = ParseFilter("ID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            filterQueryNode = ParseFilter("ID gt -INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.NegativeInfinity);

            filterQueryNode = ParseFilter("ID eq NaN", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.NaN);

            filterQueryNode = ParseFilter("SingleID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            filterQueryNode = ParseFilter("DoubleID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            Action parse = () => ParseFilter("DecimalID add INF eq INF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.ShouldThrow<OverflowException>();
        }

        [Fact]
        public void ParseFilterINFNaNWithSuffix()
        {
            var filterQueryNode = ParseFilter("ID gt INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);

            filterQueryNode = ParseFilter("ID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);

            filterQueryNode = ParseFilter("ID gt -INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Single.NegativeInfinity);

            filterQueryNode = ParseFilter("ID eq NaNF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Single.NaN);

            filterQueryNode = ParseFilter("SingleID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Single.PositiveInfinity);

            filterQueryNode = ParseFilter("DoubleID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Double.PositiveInfinity);

            Action parse = () => ParseFilter("DecimalID add INFF eq INFF", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.ShouldThrow<OverflowException>();
        }

        [Fact]
        public void ParseFilterWithBoolean()
        {
            var filterQueryNode = ParseFilter("ID eq true", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(true);

            filterQueryNode = ParseFilter("ID eq false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(false);

            filterQueryNode = ParseFilter("ID eq not true", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.As<UnaryOperatorNode>().Operand.ShouldBeConstantQueryNode(true);

            filterQueryNode = ParseFilter("ID eq not false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.As<UnaryOperatorNode>().Operand.ShouldBeConstantQueryNode(false);

            filterQueryNode = ParseFilter("ID and true eq false", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.As<BinaryOperatorNode>().Left.ShouldBeConstantQueryNode(true);
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(false);

            Action parse = () => ParseFilter("ID eq 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet5Type(), HardCodedTestModel.GetPet5Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Boolean", "Edm.Int32", "Equal"));

        }

        [Fact]
        public void ParseFilterNodeInComplexExpression()
        {
            var filterQueryNode = ParseFilter("(ID mul 1 add 1.01 sub 1.000000001) mul 2 ge 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(1D);

            Action parse = () => ParseFilter("(DoubleID mul 1 add 1.000000000000001 sub 1.000000001) mul 2 ge 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandsError("Edm.Double", "Edm.Decimal", "Add"));
        }

        [Fact]
        public void ParseFilterDoublePrecision()
        {
            var filterQueryNode = ParseFilter("DoubleID eq 1.0099999904632568", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(1.0099999904632568D);
        }

        [Fact]
        public void ParseFilterSinglePrecision()
        {
            var filterQueryNode = ParseFilter("SingleID eq " + Single.MinValue.ToString("R"), HardCodedTestModel.TestModel, HardCodedTestModel.GetPet1Type(), HardCodedTestModel.GetPet1Set());
            filterQueryNode.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode(Single.MinValue);
        }

        [Fact]
        public void ParseFilterWithEntitySetShouldBeAbleToDetermineSets()
        {
            var filterQueryNode = ParseFilter("MyDog/Color eq 'brown'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                    And.Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).
                    And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void ParseFilterWithPrimitiveCollectionCount()
        {
            var filterQueryNode = ParseFilter("MyDates/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeCountNode().
                    And.Source.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDatesProp());
        }

        [Fact]
        public void ParseFilterWithComplexCollectionCount()
        {
            var filterQueryNode = ParseFilter("PreviousAddresses/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeCountNode().
                    And.Source.ShouldBeCollectionComplexNode(HardCodedTestModel.GetPersonPreviousAddressesProp());
        }

        [Fact]
        public void ParseFilterWithEnumCollectionCount()
        {
            var filterQueryNode = ParseFilter("FavoriteColors/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeCountNode().
                    And.Source.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonFavoriteColorsProp());
        }

        [Fact]
        public void ParseFilterWithEntityCollectionCount()
        {
            var filterQueryNode = ParseFilter("MyFriendsDogs/$count eq 2", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeCountNode().
                    And.Source.ShouldBeCollectionNavigationNode(HardCodedTestModel.GetPersonMyFriendsDogsProp());
        }

        [Fact]
        public void CompareComplexWithNull()
        {
            var filter = ParseFilter("MyAddress eq null", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            var binary = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binary.Left.ShouldBeSingleComplexNode(HardCodedTestModel.GetPersonAddressProp());
            binary.Right.ShouldBeConvertQueryNode(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void ReplaceShouldWork()
        {
            var filterQueryNode = ParseFilter("replace(Name, 'a', 'e') eq 'endrew'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeSingleValueFunctionCallQueryNode("replace");
        }

        [Fact]
        public void FilterWithKeyLookupOnNavPropIsNotAllowed()
        {
            Action parse = () => ParseFilter("MyPeople(987)", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_UnknownFunction("MyPeople"));
        }

        [Fact]
        public void ParseFilterWithNoEntitySetUsesNull()
        {
            var filterQueryNode = ParseFilter("MyDog/Color eq 'brown'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterQueryNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                    And.Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).
                    And.NavigationSource.Should().BeNull();
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
                And.Source.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetAddressMyNeighborsProperty());
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

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("Any"));
        }

        [Fact]
        public void ParseOrderByWithEntitySetShouldBeAbleToDetermineSets()
        {
            var orderByQueryNode = ParseOrderBy("MyDog/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                And.Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).
                    And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void ParseOrderByWithContainedEntitySetShouldBeAbleToDetermineSets()
        {
            var orderByQueryNode = ParseOrderBy("MyContainedDog/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                And.Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyContainedDogNavProp()).
                    And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetContainedDogEntitySet());
        }

        [Fact]
        public void ParseOrderByWithNoEntitySetUsesNull()
        {
            var orderByQueryNode = ParseOrderBy("MyDog/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                And.Source.ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp()).
                    And.NavigationSource.Should().BeNull();
        }

        [Fact]
        public void ParseMultipleOrderBys()
        {
            var orderByQueryNode = ParseOrderBy("Name asc, Shoe desc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            orderByQueryNode.Direction.Should().Be(OrderByDirection.Ascending);

            orderByQueryNode.ThenBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            orderByQueryNode.ThenBy.Direction.Should().Be(OrderByDirection.Descending);
        }

        [Fact]
        public void ParseEnumMultipleOrderBys()
        {
            var orderByQueryNode = ParseOrderBy("PetColorPattern asc, cast(PetColorPattern,'Edm.String') desc, PetColorPattern has Fully.Qualified.Namespace.ColorPattern'SolidYellow' asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type());
            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            orderByQueryNode.Direction.Should().Be(OrderByDirection.Ascending);
            orderByQueryNode.ThenBy.Expression.ShouldBeSingleValueFunctionCallQueryNode().And.Parameters.Count().Should().Be(2);
            orderByQueryNode.ThenBy.Direction.Should().Be(OrderByDirection.Descending);
            orderByQueryNode.ThenBy.ThenBy.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Has)
                .And.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            orderByQueryNode.ThenBy.ThenBy.Direction.Should().Be(OrderByDirection.Ascending);
        }

        [Fact]
        public void ParseEnumPropertyOrderBy()
        {
            var orderByQueryNode = ParseOrderBy("PetColorPattern asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type());
            orderByQueryNode.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            orderByQueryNode.Direction.Should().Be(OrderByDirection.Ascending);
        }

        [Fact]
        public void ParseEnumConstantOrderBy()
        {
            var orderByQueryNode = ParseOrderBy("Fully.Qualified.Namespace.ColorPattern'SolidYellow' asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPet2Type());
            var enumtypeRef = new EdmEnumTypeReference(UriEdmHelpers.FindEnumTypeFromModel(HardCodedTestModel.TestModel, "Fully.Qualified.Namespace.ColorPattern"), true);
            orderByQueryNode.Expression.ShouldBeEnumNode(new ODataEnumValue(12L + "", enumtypeRef.FullName()));
            orderByQueryNode.Direction.Should().Be(OrderByDirection.Ascending);
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

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                And.Right.ShouldBeConstantQueryNode(new DateTimeOffset(1997, 2, 4, 0, 0, 0, 0, new TimeSpan(11, 0, 0)));
        }

        [Fact]
        public void FilterWithShortDatetimeOffset()
        {
            var filterNode = ParseFilter("Birthdate gt 1997-02-04", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                And.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                .And.Source.ShouldBeConstantQueryNode(new Date(1997, 2, 4));
        }

        [Fact]
        public void FilterWithDate()
        {
            var filterNode = ParseFilter("MyDate gt 1997-02-04", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                 And.Right.ShouldBeConstantQueryNode(new Date(1997, 2, 4));
        }

        [Fact]
        public void FilterWithTimeOfDay()
        {
            var filterNode = ParseFilter("MyTimeOfDay gt 23:40:40.900", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).
                And.Right.ShouldBeConstantQueryNode(new TimeOfDay(23, 40, 40, 900));
        }

        [Fact]
        public void FilterWithDuration()
        {
            var filterNode = ParseFilter("duration'PT0H0M15S' eq duration'P1DT0H0M30S'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            var binaryOp = filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binaryOp.Left.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 15));
            binaryOp.Right.ShouldBeConstantQueryNode(new TimeSpan(1, 0, 0, 30));
        }

        [Fact]
        public void FilterByWithNonEntityType()
        {
            FilterClause filter = ParseFilter("$it gt 6", HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            var binaryOp = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan).And;
            binaryOp.Left.ShouldBeNonResourceRangeVariableReferenceNode("$it");
            binaryOp.Right.ShouldBeConstantQueryNode(6);
        }

        [Fact]
        public void FilterWithIncompatibleTypeShouldThrow()
        {
            Action action = () => ParseFilter("contains($it,'6')", HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound("contains", "contains(Edm.String Nullable=true, Edm.String Nullable=true)"));
        }

        [Fact]
        public void FilterWithInvalidParameterShouldThrow()
        {
            Action action = () => ParseFilter("$It gt 6", HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Edm.Int32", "$It"));
        }

        [Fact]
        public void BadCastShouldResultInNiceException()
        {
            Action parse = () => ParseOrderBy("MyDog/Missing.Type/Color", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.CastBinder_ChildTypeIsNotEntity("Missing.Type"));
        }

        [Fact]
        public void NullValueInCanonicalFunction()
        {
            var result = ParseFilter("day(null)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());

            result.Expression.ShouldBeSingleValueFunctionCallQueryNode("day").
                And.Parameters.Single().ShouldBeConstantQueryNode<object>(null).And.TypeReference.Should().BeNull();
        }

        [Fact]
        public void OrderByWithEntityExpressionShouldThrow()
        {
            Action parse = () => ParseOrderBy("MyDog", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
        }

        [Fact]
        public void OrderByWithEntityCollectionExpressionShouldThrow()
        {
            Action parse = () => ParseOrderBy("MyPeople", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
        }

        [Fact]
        public void NegateAnEntityShouldThrow()
        {
            Action parse = () => ParseOrderBy("-MyDog", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_IncompatibleOperandError(HardCodedTestModel.GetPersonMyDogNavProp().Type.FullName(), UnaryOperatorKind.Negate));
        }

        [Fact]
        public void IsOfFunctionWorksWithSingleQuotesOnType()
        {
            FilterClause filter = ParseFilter("isof(Shoe, 'Edm.String')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeSingleValueFunctionCallQueryNode("isof");
            filter.Expression.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            filter.Expression.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
        }

        [Fact]
        public void CastFunctionWorksWithNoSingleQuotesOnType()
        {
            FilterClause filter = ParseFilter("cast(Shoe, Edm.String) eq 'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String);
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.ShouldBeSingleValueFunctionCallQueryNode("cast");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Edm.String");
            filter.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode("blue");
        }

        [Fact]
        public void CastFunctionWorksForEnum()
        {
            FilterClause filter = ParseFilter("cast(Shoe, Fully.Qualified.Namespace.ColorPattern) eq Fully.Qualified.Namespace.ColorPattern'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeSingleValueFunctionCallQueryNode("cast");
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Fully.Qualified.Namespace.ColorPattern");
            filter.Expression.As<BinaryOperatorNode>().Right.ShouldBeEnumNode(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern") as IEdmEnumType, 2L);
        }


        [Fact]
        public void CastFunctionWorksForCastFromNullToEnum()
        {
            FilterClause filter = ParseFilter("cast(null, Fully.Qualified.Namespace.ColorPattern) eq Fully.Qualified.Namespace.ColorPattern'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeSingleValueFunctionCallQueryNode("cast");
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).As<ConstantNode>().Value.Should().BeNull();
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).ShouldBeConstantQueryNode("Fully.Qualified.Namespace.ColorPattern");
            filter.Expression.As<BinaryOperatorNode>().Right.ShouldBeEnumNode(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern") as IEdmEnumType, 2L);
        }

        [Fact]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfDottedIdentifier()
        {
            FilterClause filter = ParseFilter("cast(null, Fully.Qualified.Namespace.ColorPattern) eq Fully.Qualified.Namespace.ColorPattern'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeSingleValueFunctionCallQueryNode("cast");
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.ElementAt(0).As<ConstantNode>().LiteralText.Should().Be("null");
            filter.Expression.As<BinaryOperatorNode>().Left.As<SingleValueFunctionCallNode>().Parameters.ElementAt(1).As<ConstantNode>().LiteralText.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConstantNode>().LiteralText.Should().Be("Fully.Qualified.Namespace.ColorPattern'blue'");
        }

        [Fact]
        public void CastFunctionProducesAnEntityType()
        {
            FilterClause filter = ParseFilter("cast(MyDog, 'Fully.Qualified.Namespace.Dog')/Color eq 'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            SingleResourceFunctionCallNode function = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                .And.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp())
                .And.Source.ShouldBeSingleResourceFunctionCallNode("cast").And;
            function.Parameters.Should().HaveCount(2);
            function.Parameters.ElementAt(0).ShouldBeSingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp());
            function.Parameters.ElementAt(1).ShouldBeConstantQueryNode("Fully.Qualified.Namespace.Dog");
            filter.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode("blue");
        }

        [Fact]
        public void OrderByWithExpression()
        {
            OrderByClause orderBy = ParseOrderBy("Shoe eq 'blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            orderBy.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            orderBy.Expression.As<BinaryOperatorNode>()
                   .Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp());
            orderBy.Expression.As<BinaryOperatorNode>().Right.ShouldBeConstantQueryNode("blue");
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
            filterClause.Should().NotBeNull();
            filterClause.Expression.ShouldBeAnyQueryNode()
                        .And.Body.ShouldBeSingleValueOpenPropertyAccessQueryNode("OpenProperty");
        }

        [Fact]
        public void InvalidPropertyNameThrowsUsefulErrorMessage()
        {
            // regression test for: [Fuzz] InvalidCastException being thrown for filter/orderby with query "PersonMetadataId /c"
            Action parseInvalidPropertyName = () => ParseFilter("PersonMetadataId /c", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseInvalidPropertyName.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "PersonMetadataId"));
        }

        [Fact]
        public void ControlCharactersShouldBeIgnored()
        {
            // regression test for: [UriParser] \n in string leads to ambiguity
            var filterClause = ParseFilter("length(Name) \neq 30", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binaryOperatorNode.Left.ShouldBeSingleValueFunctionCallQueryNode("length").And.Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            binaryOperatorNode.Right.ShouldBeConstantQueryNode(30);

            // regression test for: [UriParser] \r in the path string gets disappeared
            var filterClause1 = ParseFilter("length(Name) \req 30", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode1 = filterClause1.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binaryOperatorNode1.Left.ShouldBeSingleValueFunctionCallQueryNode("length").And.Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp());
            binaryOperatorNode1.Right.ShouldBeConstantQueryNode(30);
        }

        [Fact]
        public void TrailingDollarSegmentThows()
        {
            // regression test for: [UriParser] Trailing $ lost
            Action parseWithTrailingDollarSign = () => ParseOrderBy("Name/$", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithTrailingDollarSign.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Edm.String", "$"));
        }

        [Fact]
        public void ArbitraryFunctionCallsNotAllowed()
        {
            // regression test for: [UriParser] day() allowed. What does that mean?
            // make sure arbitrary funcitons aren't allowed...
            Action parseArbitraryFunctionCall = () => ParseFilter("gobbldygook() eq 20", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseArbitraryFunctionCall.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_UnknownFunction("gobbldygook"));
        }

        [Fact]
        public void EmptyFunctionCallParametersAreProperlyValidated()
        {
            // regression test for: [UriParser] day() allowed. What does that mean?
            // make sure that, if we do find a cannonical function, we match its parameters. 
            Action parseWithInvalidParameters = () => ParseFilter("day() eq 20", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetUriFunctionSignatures("day"); // to match the error message... blah
            parseWithInvalidParameters.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [Fact]
        public void FunctionCallParametersAreValidated()
        {
            // regression test for: [UriParser] day() allowed. What does that mean?
            // make sure that, if we do find a cannonical function, we match its parameters. 
            Action parseWithInvalidParameters = () => ParseFilter("day(1) eq 20", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            FunctionSignatureWithReturnType[] signatures = FunctionCallBinder.GetUriFunctionSignatures("day"); // to match the error message... blah
            parseWithInvalidParameters.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                    "day",
                    UriFunctionsHelper.BuildFunctionSignatureListDescription("day", signatures)));
        }

        [Fact]
        public void AnyAllOnAPrimitiveShouldThrowODataException()
        {
            // regression test for: [URIParser] $filter with Any/All throws invalid cast instead of Odata Exception
            Action anyOnPrimitiveType = () => ParseFilter("Name/any(a: a eq 'Bob')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            anyOnPrimitiveType.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_LambdaParentMustBeCollection);
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
            orderbyClause.Direction.Should().Be(OrderByDirection.Ascending);
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
            orderByClause.Direction.Should().Be(OrderByDirection.Ascending);
        }

        [Fact]
        public void FunctionCallWithParametersWorksInFilter()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.HasDog(inOffice=true)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters())
                .And.Source.ShouldBeResourceRangeVariableReferenceNode("$it");
        }

        [Fact]
        public void DeepFunctionCallWithParametersWorksInOrderby()
        {
            var filterClause = ParseOrderBy("MyPeople/any(a: a/Fully.Qualified.Namespace.HasDog(inOffice=true))", HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            filterClause.Expression.ShouldBeAnyQueryNode()
                .And.Body.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters())
                .And.Source.ShouldBeResourceRangeVariableReferenceNode("a");
        }

        [Fact]
        public void FunctionWithComplexParameterInJsonWithSingleQuotesInsteadOfDoubleQuotesWorks()
        {
            const string text = "Fully.Qualified.Namespace.CanMoveToAddress(address={'Street' : 'stuff', 'City' : 'stuff'})";
            var filterClause = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForCanMoveToAddress())
                .And.Parameters.Single().As<NamedFunctionParameterNode>().Value.As<ConstantNode>().Value.Should().Be("{'Street' : 'stuff', 'City' : 'stuff'}");
        }

        [Fact]
        public void FunctionWithInvalidComplexParameterThrows()
        {
            Action parseInvalidComplex = () => ParseFilter("Fully.Qualified.Namespace.CanMoveToAddress(address={}})", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseInvalidComplex.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidCharacter("}", "53", "Fully.Qualified.Namespace.CanMoveToAddress(address={}})"));
        }

        [Fact]
        public void FunctionWithComplexParameterInJsonWorks()
        {
            var filterCaluse = ParseFilter("Fully.Qualified.Namespace.CanMoveToAddress(address={\"@odata.type\":\"#Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"})", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterCaluse.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForCanMoveToAddress())
                .And.Parameters.Single().As<NamedFunctionParameterNode>()
                .Value.As<ConstantNode>().Value.Should().Be("{\"@odata.type\":\"#Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"}");
        }

        [Fact]
        public void FunctionWithCollectionParameterInJsonWorks()
        {
            const string text = "Fully.Qualified.Namespace.OwnsTheseDogs(dogNames=[\"Barky\",\"Junior\"])";
            var filterCaluse = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterCaluse.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForOwnsTheseDogs()).And.Parameters.Single().As<NamedFunctionParameterNode>().Value.As<ConstantNode>().Value.ShouldBeODataCollectionValue()
            .And.ItemsShouldBeAssignableTo<string>().And.Count().Should().Be(2);
        }

        [Fact]
        public void FunctionWithCollectionOfComplexParameterInJsonWorks()
        {
            const string text = "Fully.Qualified.Namespace.CanMoveToAddresses(addresses=[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}])";
            var filterCaluse = ParseFilter(text, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            // TODO: parameter value is ConstantNode, whose .TypeReference should NOT be null though .Value is ok to be ODataCollectionValue.
            filterCaluse.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForCanMoveToAddresses())
                .And.Parameters.Single().As<NamedFunctionParameterNode>().Value.As<ConstantNode>().Value.Should().Be("[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}]");
        }

        [Fact]
        public void FunctionParameterAliasWorksInFilter()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host/"), new Uri("http://host/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice=@a)&@a=null"));
            var filterClause = uriParser.ParseFilter();
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters())
                .And.Parameters.Single().As<NamedFunctionParameterNode>()
                .Value.As<ParameterAliasNode>().Alias.Should().Be("@a");

            // verify alias value node:
            uriParser.ParameterAliasNodes["@a"].ShouldBeConstantQueryNode((object)null);
        }

        [Fact]
        public void UnresolvedFunctionParameterAliasWorksInFilter()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.HasDog(inOffice=@a)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters())
                .And.Parameters.Single().As<NamedFunctionParameterNode>()
                .Value.As<ParameterAliasNode>()
                .Alias.Should().Be("@a");
        }

        [Fact]
        public void FunctionsAreComposable()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.GetMyDog/Color eq 'Blue'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp())
                              .And.Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog());
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
            parseWithExplicitBindingParam.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_UnknownFunction("Fully.Qualified.Namespace.HasDog")); // no '...HasDog' method has parameter type of '$it' RangeVariableToken.
        }

        [Fact]
        public void CannotAddEntityAsBindingParameterToFunction()
        {
            Action parseWithExplicitBindingParam = () => ParseFilter("HasDog(person=People(1))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithExplicitBindingParam.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_UnknownFunction("People"));
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
            filterNode.Expression.ShouldBeSingleValueFunctionCallQueryNode(model.FindDeclaredOperations("Test.IsDark").Single().As<IEdmFunction>());
            filterNode.Expression.As<SingleValueFunctionCallNode>().Source.ShouldBeSingleComplexNode(model.EntityTypes().Single().Properties().Single(p => p.Name == "Color"));
        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToComplexWithParameterShouldWork()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var filterNode = ParseFilter("Color/Test.IsDarkerThan(other={\"Red\":64}) eq true", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            var left = filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Left;
            left.ShouldBeSingleValueFunctionCallQueryNode(model.FindDeclaredOperations("Test.IsDarkerThan").Single().As<IEdmFunction>());
            left.As<SingleValueFunctionCallNode>().Source.ShouldBeSingleComplexNode(model.EntityTypes().Single().Properties().Single(p => p.Name == "Color"));

        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToComplexAllowsComposition()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var filterNode = ParseFilter("Color/Test.GetMostPopularVegetableWithThisColor()/ID eq ID", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            filterNode.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).
                And.Left.ShouldBeSingleValuePropertyAccessQueryNode(model.EntityTypes().Single().Properties().Single(p => p.Name == "ID"));
        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToPrimitiveShouldNotLookForFunctions()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            Action parse = () => ParseFilter("ID/IsPrime()", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("IsPrime"));
        }

        [Fact]
        public void ParseFilterWithFunctionCallBoundToPrimitiveWithoutParensShouldNotLookForFunctions()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            Action parse = () => ParseFilter("ID/Test.IsPrime", model, model.EntityTypes().Single(e => e.Name == "Vegetable"), null);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.CastBinder_ChildTypeIsNotEntity("Test.IsPrime"));
        }

        [Fact]
        public void FunctionWithExpressionParameterThrows()
        {
            Action parseWithExpressionParameter = () => ParseFilter("OwnsTheseDogs(dogNames=Dogs(0))", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithExpressionParameter.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_UnknownFunction("Dogs"));
        }

        [Fact]
        public void FunctionWithMultipleParametersWithTheSameNameThrows()
        {
            var model = ModelBuildingHelpers.GetModelWithFunctionWithDuplicateParameterNames();
            Action parseWithMultipleParameters = () => ParseFilter("Test.Foo(p2='stuff', p2=1)", model, model.EntityTypes().Single(e => e.Name == "Vegetable"));
            parseWithMultipleParameters.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
        }

        [Fact]
        public void LongFunctionChainWorksInFilter()
        {
            var filter = ParseFilter("Fully.Qualified.Namespace.GetMyDog/Fully.Qualified.Namespace.GetMyPerson/Fully.Qualified.Namespace.GetMyDog/Fully.Qualified.Namespace.GetMyPerson/Name eq 'Bob'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperatorNode = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp())
                .And.Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyPerson())
                .And.Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog())
                .And.Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyPerson())
                .And.Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForGetMyDog())
                .And.Source.ShouldBeResourceRangeVariableReferenceNode(ExpressionConstants.It);
            binaryOperatorNode.Right.ShouldBeConstantQueryNode("Bob");
        }

        #endregion

        [Fact]
        public void ActionsThrowOnClosedTypeInFilter()
        {
            Action parseWithAction = () => ParseFilter("Move", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithAction.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Move"));
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
            var binaryOperatorNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And).And;
            var leftBinaryOperatorNode =
                binaryOperatorNode.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual).And;
            var rightBinaryOperatorNode =
                binaryOperatorNode.Right.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual).And;
            leftBinaryOperatorNode.Left.As<ConvertNode>().Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
            rightBinaryOperatorNode.Left.As<ConvertNode>().Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Max");
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
            orderByClause.Direction.Should().Be(OrderByDirection.Ascending);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
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
            orderByClause.Direction.Should().Be(OrderByDirection.Ascending);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Total");
            orderByClause = orderByClause.ThenBy;
            orderByClause.Direction.Should().Be(OrderByDirection.Descending);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Max");
        }

        [Fact]
        public void ActionsThrowOnClosedInOrderby()
        {
            Action parseWithAction = () => ParseOrderBy("Move asc", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithAction.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Move"));
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
            orderByClause.Direction.Should().Be(OrderByDirection.Ascending);
            orderByClause.Expression.ShouldBeSingleValueOpenPropertyAccessQueryNode("Restore");
        }

        [Fact]
        public void ActionBoundToComplexThrows()
        {
            Action parse = () => ParseFilter("MyAddress/ChangeState", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.ShouldThrow<ODataException>().WithMessage("Could not find a property named 'ChangeState' on type 'Fully.Qualified.Namespace.Address'.");
        }

        [Fact]
        public void FunctionCallWithGeometryAndNullParameterValuesShouldWorkInOrderBy()
        {
            var point = GeometryPoint.Create(1, 2);
            var orderByClause = ParseOrderBy("Fully.Qualified.Namespace.GetColorAtPosition(position=geometry'" + SpatialHelpers.WriteSpatial(point) + "',includeAlpha=null)", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet());
            orderByClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetColorAtPositionFunction())
                .And.ShouldHaveConstantParameter("position", point)
                .And.ShouldHaveConstantParameter("includeAlpha", (object)null);
        }

        [Fact]
        public void FunctionCallWithGeographyAndNullParameterValuesShouldWorkInFilter()
        {
            var point = GeographyPoint.Create(1, 2);
            var filterClause = ParseFilter("Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)/any()", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filterClause.Expression.ShouldBeAnyQueryNode()
                .And.Source.ShouldBeCollectionResourceFunctionCallNode(HardCodedTestModel.GetNearbyPriorAddressesFunction())
                    .And.ShouldHaveConstantParameter("currentLocation", point)
                    .And.ShouldHaveConstantParameter("limit", (object)null);
        }

        [Fact]
        public void LongFunctionChain()
        {
            // in this case all I really care about is that it doesn't throw... baselining this is overkill.
            Action parseLongFunctionChain = () => ParseFilter("Fully.Qualified.Namespace.AllMyFriendsDogs()/Fully.Qualified.Namespace.OwnerOfFastestDog()/MyDog/MyPeople/Fully.Qualified.Namespace.AllHaveDog(inOffice=true) eq true", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseLongFunctionChain.ShouldNotThrow();
        }

        [Fact]
        public void FunctionBindingFailsIfParameterNameIsIncorrect()
        {
            Action parseWithIncorrectName = () => ParseFilter("Fully.Qualified.Namespace.HasDog(inOfFiCe=true)", HardCodedTestModel.TestModel, HardCodedTestModel.GetEmployeeType());
            parseWithIncorrectName.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_UnknownFunction("Fully.Qualified.Namespace.HasDog")); // no '...HasDog' method has parameter 'inOfFiCe'.
        }

        [Fact]
        public void FunctionWithoutBindingParameterShouldWorkInFilterOrderby()
        {
            var filterClause = ParseFilter("Fully.Qualified.Namespace.FindMyOwner(dogsName='fido')/Name eq 'Bob'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var binaryOperator = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binaryOperator.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonNameProp())
                .And.Source.ShouldBeSingleResourceFunctionCallNode(HardCodedTestModel.GetFunctionForFindMyOwner());
            binaryOperator.Right.ShouldBeConstantQueryNode("Bob");
        }

        [Fact]
        public void FunctionCallWithKeyExpressionShouldFail()
        {
            Action parse = () => ParseFilter("AllMyFriendsDogs(inOffice=true)(1) ne null", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_SyntaxError(32, "AllMyFriendsDogs(inOffice=true)(1) ne null"));
        }

        [Fact]
        public void FunctionCallWithKeyExpressionShouldFailEvenIfFunctionHasNoParameters()
        {
            Action parse = () => ParseFilter("AllMyFriendsDogs(1) ne null", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.ShouldThrow<InvalidOperationException>().WithMessage(ODataErrorStrings.MetadataBinder_UnknownFunction("AllMyFriendsDogs"));
        }

        [Fact]
        public void AmbiguousFunctionCallThrows()
        {
            var model = ModelBuildingHelpers.GetModelWithFunctionOverloadsWithSameParameterNames();
            Action parse = () => ParseFilter("Test.Foo(p2='1')", model, model.EntityTypes().Single(x => x.Name == "Vegetable"));
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound("Test.Foo", "p2"));
        }

        [Fact]
        public void FilterOnTypeDefinitionProperty()
        {
            var filter = ParseFilter("FirstName ne 'Bob'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.As<BinaryOperatorNode>().Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.String);
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
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().TypeReference.FullName().Should().Be("Edm.Boolean");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int32");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConstantNode>().TypeReference.FullName().Should().Be("Edm.Int32");
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenUInt32AndPrimitive()
        {
            FilterClause filter = ParseFilter("StockQuantity ne " + UInt32.MaxValue, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            filter.Expression.As<BinaryOperatorNode>().TypeReference.FullName().Should().Be("Edm.Boolean");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int64");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConstantNode>().TypeReference.FullName().Should().Be("Edm.Int64");
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenUInt64AndPrimitive()
        {
            FilterClause filter = ParseFilter("LifeTime ne " + UInt64.MaxValue, HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            filter.Expression.As<BinaryOperatorNode>().TypeReference.FullName().Should().Be("Edm.Boolean");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Decimal");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConstantNode>().TypeReference.FullName().Should().Be("Edm.Decimal");
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenPrimitiveAndUInt()
        {
            FilterClause filter = ParseFilter("123 ne StockQuantity", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            filter.Expression.As<BinaryOperatorNode>().TypeReference.FullName().Should().Be("Edm.Boolean");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConstantNode>().TypeReference.FullName().Should().Be("Edm.Int64");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int64");
        }

        [Fact]
        public void FilterWithBinaryOperatorBetweenUInts()
        {
            FilterClause filter = ParseFilter("FavoriteNumber eq StockQuantity", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().TypeReference.FullName().Should().Be("Edm.Boolean");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int64");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int64");
        }

        [Fact]
        public void FilterWithBinaryOperatorAddBetweenPrimitiveAndUInt()
        {
            FilterClause filter = ParseFilter("123 add StockQuantity eq 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().TypeReference.FullName().Should().Be("Edm.Boolean");
            var addExpr = filter.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>();
            addExpr.Left.As<ConstantNode>().TypeReference.FullName().Should().Be("Edm.Int64");
            addExpr.Right.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int64");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConstantNode>().TypeReference.FullName().Should().Be("Edm.Int64");
        }

        [Fact]
        public void FilterWithBinaryOperatorAddBetweenUInts()
        {
            FilterClause filter = ParseFilter("FavoriteNumber add StockQuantity eq 1", HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().TypeReference.FullName().Should().Be("Edm.Boolean");
            var addExpr = filter.Expression.As<BinaryOperatorNode>().Left.As<BinaryOperatorNode>();
            addExpr.Left.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int64");
            addExpr.Right.As<ConvertNode>().TypeReference.FullName().Should().Be("Edm.Int64");
            filter.Expression.As<BinaryOperatorNode>().Right.As<ConstantNode>().TypeReference.FullName().Should().Be("Edm.Int64");
        }

        #region operators on temporal type
        [Fact]
        public void NegateOnDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("-TimeEmployed", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var node = orderby.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate).And;
            node.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDuration(true));
            node.Operand.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [Fact]
        public void NegateOnDurationLiteralShouldWork()
        {
            FilterClause filter = ParseFilter("-duration'PT130S' eq  TimeEmployed", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            var left = expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true)).And;
            left.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDuration(true));
            left.Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                .And.Operand.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
            expression.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [Fact]
        public void DateConstantAddDurationShouldWork()
        {
            FilterClause filter = ParseFilter("TimeEmployed add 2010-06-10 le 2011-06-18+00:00", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual).And;
            var left =
                expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                    .And.Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add)
                    .And;
            left.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDate(true));
            left.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .And.Source.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                .And.Source.ShouldBeConstantQueryNode(new DateTimeOffset(new DateTime(2011, 06, 18), new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void DatePropertyAddDurationShouldWork()
        {
            FilterClause filter = ParseFilter("TimeEmployed add MyDate le 2011-06-18+00:00", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual).And;
            var left = expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                    .And.Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add).And;
            left.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDate(true));
            left.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .And.Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                .And.Source.ShouldBeConstantQueryNode(new DateTimeOffset(new DateTime(2011, 06, 18), new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void DurationAddDateShouldWork()
        {
            FilterClause filter = ParseFilter("2010-06-10 add TimeEmployed le 2011-06-18+00:00", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual).And;
            var left =
                expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                    .And.Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add)
                    .And;
            left.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDate(true));
            left.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .And.Source.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(true))
                .And.Source.ShouldBeConstantQueryNode(new DateTimeOffset(new DateTime(2011, 06, 18), new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void DateTimeOffsetAddDurationShouldWork()
        {
            FilterClause filter = ParseFilter("TimeEmployed add 2010-06-10 le 2011-06-18", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual).And;
            var left = expression.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add).And;
            left.TypeReference.AsPrimitive().AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDate(true));
            left.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            left.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .And.Source.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDate(true))
                .And.Source.ShouldBeConstantQueryNode(new Date(2011, 06, 18));
        }

        [Fact]
        public void DurationAddDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("TimeEmployed add -duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add).And;
            expression.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDuration(true));
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true))
                .And.Source.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate)
                .And.Operand.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateTimeOffsetSubDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("2011-06-18 sub duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract).And;
            expression.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDate(false));
            expression.Left.ShouldBeConstantQueryNode(new Date(2011, 06, 18));
            expression.Right.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateSubDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("MyDate sub duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract).And;
            expression.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDate(false));//PrimitiveKind().Should().Be(EdmPrimitiveTypeKind.Date);
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            expression.Right.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateSubDateShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("MyDate sub 2010-06-18", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract).And;
            expression.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDuration(false));
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            expression.Right.ShouldBeConstantQueryNode(new Date(2010, 06, 18));
        }

        [Fact]
        public void DurationSubDurationShouldWork()
        {
            OrderByClause orderby = ParseOrderBy("TimeEmployed sub duration'PT130S'", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = orderby.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract).And;
            expression.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDuration(true));
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
            expression.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true))
                .And.Source.ShouldBeConstantQueryNode(new TimeSpan(0, 0, 2, 10));
        }

        [Fact]
        public void DateTimeOffsetSubDateTimeOffsetShouldWork()
        {
            FilterClause filterClause = ParseFilter("2011-06-18 sub 2010-06-10 ge TimeEmployed", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual).And;
            var left = expression.Left.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDuration(true))
                .And.Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract).And;
            left.TypeReference.AsPrimitive().ShouldBeEquivalentTo(EdmCoreModel.Instance.GetDuration(false));
            left.Left.ShouldBeConstantQueryNode(new Date(2011, 06, 18));
            left.Right.ShouldBeConstantQueryNode(new Date(2010, 06, 10));
            expression.Right.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [Fact]
        public void FunctionDateWithDateTimeOffsetShouldWorkInFilter()
        {
            FilterClause filterClause = ParseFilter("MyDate ge date(2011-06-18)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual).And;
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("date", EdmCoreModel.Instance.GetDate(false)).And;
            right.Parameters.Single()
                .ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetDateTimeOffset(false))
                .And.Source.ShouldBeConstantQueryNode(new Date(2011, 6, 18));

        }

        [Fact]
        public void OrderByFunctionDateWithDateTimeOffsetShouldWork()
        {
            OrderByClause clause = ParseOrderBy("date(Birthdate)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("date", EdmCoreModel.Instance.GetDate(false));
            clause.Direction.Should().Be(OrderByDirection.Ascending);
        }

        [Fact]
        public void FunctionTimeWithDateTimeOffsetShouldWorkInFilter()
        {
            FilterClause filterClause = ParseFilter("MyTimeOfDay ge time(2014-09-19T12:13:14+00:00)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual).And;
            expression.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDayProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("time", EdmCoreModel.Instance.GetTimeOfDay(false)).And;
            right.Parameters.Single().ShouldBeConstantQueryNode(new DateTimeOffset(2014, 9, 19, 12, 13, 14, new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void OrderByFunctionTimeWithDateTimeOffsetShouldWork()
        {
            OrderByClause clause = ParseOrderBy("time(Birthdate)",
                HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("time", EdmCoreModel.Instance.GetTimeOfDay(false));
            clause.Direction.Should().Be(OrderByDirection.Ascending);
        }

        [Fact]
        public void FunctionYearWithDateShouldWork()
        {
            FilterClause filterClause = ParseFilter("year(MyDate) ge year(2010-12-13)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual).And;
            var left = expression.Left.ShouldBeSingleValueFunctionCallQueryNode("year", EdmCoreModel.Instance.GetInt32(false)).And;
            left.Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("year", EdmCoreModel.Instance.GetInt32(false)).And;
            right.Parameters.Single().ShouldBeConstantQueryNode(new Date(2010, 12, 13));
        }

        [Fact]
        public void FunctionHourWithTimeOfDayShouldWork()
        {
            FilterClause filterClause = ParseFilter("hour(MyTimeOfDay) ge hour(19:20:5)", HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var expression = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual).And;
            var left = expression.Left.ShouldBeSingleValueFunctionCallQueryNode("hour", EdmCoreModel.Instance.GetInt32(false)).And;
            left.Parameters.Single().ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDayProp());
            var right = expression.Right.ShouldBeSingleValueFunctionCallQueryNode("hour", EdmCoreModel.Instance.GetInt32(false)).And;
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
            clause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Left.ShouldBeSingleValuePropertyAccessQueryNode(homeNo);
        }
        #endregion

        #region Primitive type cast
        [Fact]
        public void FilterWithCastStringProperty()
        {
            FilterClause filter = ParseFilter("Artist/Edm.String eq 'sdb'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().TypeReference.FullName().Should().Be("Edm.String");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().InternalKind.ShouldBeEquivalentTo(InternalQueryNodeKind.SingleValueCast);
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Kind.ShouldBeEquivalentTo(QueryNodeKind.SingleValueCast);
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
        }

        [Fact]
        public void FilterWithCastStringOpenProperty()
        {
            FilterClause filter = ParseFilter("Assistant/Edm.String eq 'sdb'", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().TypeReference.FullName().Should().Be("Edm.String");
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().InternalKind.ShouldBeEquivalentTo(InternalQueryNodeKind.SingleValueCast);
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Kind.ShouldBeEquivalentTo(QueryNodeKind.SingleValueCast);
            filter.Expression.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Assistant");
        }

        [Fact]
        public void FilterWithCastAnyProperty()
        {
            FilterClause filter = ParseFilter("Colors/any(x:x/Edm.String eq 'blue')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            filter.Expression.ShouldBeAnyQueryNode();
            filter.Expression.As<AnyNode>().Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().TypeReference.FullName().Should().Be("Edm.String");
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().InternalKind.ShouldBeEquivalentTo(InternalQueryNodeKind.SingleValueCast);
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Kind.ShouldBeEquivalentTo(QueryNodeKind.SingleValueCast);
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Source.ShouldBeNonResourceRangeVariableReferenceNode("x");
        }

        [Fact]
        public void FilterWithCastAnyOpenProperty()
        {
            FilterClause filter = ParseFilter("Exhibits/any(x:x/Edm.String eq 'Louvre')", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            filter.Expression.ShouldBeAnyQueryNode();
            filter.Expression.As<AnyNode>().Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().TypeReference.FullName().Should().Be("Edm.String");
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().InternalKind.ShouldBeEquivalentTo(InternalQueryNodeKind.SingleValueCast);
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Kind.ShouldBeEquivalentTo(QueryNodeKind.SingleValueCast);
            filter.Expression.As<AnyNode>().Body.As<BinaryOperatorNode>().Left.As<ConvertNode>().Source.As<SingleValueCastNode>().Source.ShouldBeNonResourceRangeVariableReferenceNode("x");
        }

        [Fact]
        public void OrderByWithCastStringProperty()
        {
            OrderByClause orderby = ParseOrderBy("Artist/Edm.String", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            orderby.Expression.As<SingleValueCastNode>().TypeReference.FullName().Should().Be("Edm.String");
            orderby.Expression.As<SingleValueCastNode>().InternalKind.ShouldBeEquivalentTo(InternalQueryNodeKind.SingleValueCast);
            orderby.Expression.As<SingleValueCastNode>().Kind.ShouldBeEquivalentTo(QueryNodeKind.SingleValueCast);
            orderby.Expression.As<SingleValueCastNode>().Source.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingArtistProp());
        }

        [Fact]
        public void OrderByWithCastStringOpenProperty()
        {
            OrderByClause orderby = ParseOrderBy("Assistant/Edm.String", HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            orderby.Expression.As<SingleValueCastNode>().TypeReference.FullName().Should().Be("Edm.String");
            orderby.Expression.As<SingleValueCastNode>().InternalKind.ShouldBeEquivalentTo(InternalQueryNodeKind.SingleValueCast);
            orderby.Expression.As<SingleValueCastNode>().Kind.ShouldBeEquivalentTo(QueryNodeKind.SingleValueCast);
            orderby.Expression.As<SingleValueCastNode>().Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("Assistant");
        }
        #endregion

        private static FilterClause ParseFilter(string text, IEdmModel edmModel, IEdmType edmType, IEdmNavigationSource edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmType, edmEntitySet, new Dictionary<string, string>() { { "$filter", text } }).ParseFilter();
        }

        private static OrderByClause ParseOrderBy(string text, IEdmModel edmModel, IEdmType edmType, IEdmNavigationSource edmEntitySet = null)
        {
            return new ODataQueryOptionParser(edmModel, edmType, edmEntitySet, new Dictionary<string, string>() { { "$orderby", text } }).ParseOrderBy();
        }
    }
}
