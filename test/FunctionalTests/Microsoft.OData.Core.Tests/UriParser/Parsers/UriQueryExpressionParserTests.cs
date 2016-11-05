//---------------------------------------------------------------------
// <copyright file="UriQueryExpressionParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Aggregation;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class UriQueryExpressionParserTests
    {
        private readonly UriQueryExpressionParser testSubject = new UriQueryExpressionParser(50);

        [Fact]
        public void AnyAllSyntacticParsingShouldCheckSeperatorTokenIsColon()
        {
            // Repro for: Syntactic parsing for Any/All allows an arbitrary token between range variable and expression
            Action parse = () => this.testSubject.ParseFilter("Things/any(a,true)");
            parse.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ExpressionLexer_SyntaxError("13", "Things/any(a,true)"));
        }

        [Fact]
        public void AnyAllSyntacticParsingShouldNotAllowImplicitRangeVariableToBeRedefined()
        {
            // Repro for: Syntactic parser fails to block $it as a range variable name
            Action parse = () => this.testSubject.ParseFilter("Things/any($it:true)");
            parse.ShouldThrow<ODataException>().WithMessage(ErrorStrings.UriQueryExpressionParser_RangeVariableAlreadyDeclared("$it"));
        }

        [Fact]
        public void AnyAllSyntacticParsingShouldNotAllowAnyRangeVariableToBeRedefined()
        {
            // Repro for: Semantic binding fails with useless error message when a range variable is redefined within a nested any/all
            Action parse = () => this.testSubject.ParseFilter("Things/any(o:o/Things/any(o:true))");
            parse.ShouldThrow<ODataException>().WithMessage(ErrorStrings.UriQueryExpressionParser_RangeVariableAlreadyDeclared("o"));
        }

        [Fact]
        public void AnyAllSyntacticParsingShouldNotRecognizeRangeVariablesOutsideOfScope()
        {
            // Repro for: Syntactic parser assumes any token which matches the name of a previously used range variable is also a range variable, even after the scope has been exited
            var tree = this.testSubject.ParseFilter("Things/any(o:true) and o");
            tree.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And.Right.ShouldBeEndPathToken("o");
        }

        [Fact]
        public void TypeNameShouldParseSuccessfully()
        {
            this.testSubject.ParseFilter("fq.ns.typename").ShouldBeDottedIdentifierToken("fq.ns.typename");
        }

        [Fact]
        public void QualifiedFunctionNameShouldParseSuccessfully()
        {
            this.testSubject.ParseFilter("fq.ns.container.function()").ShouldBeFunctionCallToken("fq.ns.container.function");
        }

        [Fact]
        public void QualifiedFunctionNameWithParameterShouldParseSuccessfully()
        {
            this.testSubject.ParseFilter("fq.ns.container.function(arg=1)")
                .ShouldBeFunctionCallToken("fq.ns.container.function")
                .And.ShouldHaveParameter("arg")
                .And.ValueToken.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void ParseUnclosedGeographyPolygonShouldThrowWithReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geography'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            parse.ShouldThrow<ODataException>().WithMessage("Invalid spatial data", ComparisonMode.Substring);
        }

        [Fact]
        public void ParseUnclosedGeometryPolygonShouldThrowWithReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geometry'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            parse.ShouldThrow<ODataException>().WithMessage("Invalid spatial data", ComparisonMode.Substring);
        }

        [Fact]
        public void ParseGeometryPolygonWithBadPrefixShouldThrowWithoutReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geometr'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            parse.ShouldThrow<ODataException>().Where(e => !e.Message.Contains("with reason"));
        }   
        
        private static void VerifyAggregateExpressionToken(string expectedEndPathIdentifier, AggregationMethod expectedVerb, string expectedAlias, AggregateExpressionToken actual)
        {
            actual.Expression.Should().NotBeNull();

            var expression = actual.Expression as EndPathToken;
            expression.Should().NotBeNull();
            expression.Identifier.Should().Be(expectedEndPathIdentifier);

            actual.Method.Should().Be(expectedVerb);
            actual.Alias.Should().Be(expectedAlias);
        }

        [Fact]
        public void ParseApplyWithEmptyStringShouldReturnEmptyCollection()
        {
            var actual = this.testSubject.ParseApply(string.Empty);
            actual.Should().NotBeNull();
            actual.Should().BeEmpty();
        }

        [Fact]
        public void ParseApplyWithInvalidTransformationIdentifierShouldThrow()
        {
            var apply = "invalid(UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected("aggregate|filter|groupby",0,apply));
        }

        [Fact]
        public void ParseApplyWithTrailingNotSlashShouldThrow()
        {
            var apply = "aggregate(UnitPrice with sum as TotalPrice),";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.ExpressionLexer_SyntaxError(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithSingleAggregateExpressionShouldReturnAggregateToken()
        {
            var apply = "aggregate(UnitPrice with sum as TotalPrice)";

            var actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            var aggregate = actual.First() as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.Expressions.Should().HaveCount(1);

            VerifyAggregateExpressionToken("UnitPrice", AggregationMethod.Sum, "TotalPrice", aggregate.Expressions.First());
        }

        [Fact]
        public void ParseApplyWithMultipleAggregateExpressionsShouldReturnAggregateTokens()
        {
            var apply = "aggregate(CustomerId with sum as Total, SharePrice with countdistinct as SharePriceDistinctCount)";

            var actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            var aggregate = actual.First() as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.Expressions.Should().HaveCount(2);

            var statements = aggregate.Expressions.ToList();
            
            VerifyAggregateExpressionToken("CustomerId", AggregationMethod.Sum, "Total", statements[0]);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethod.CountDistinct, "SharePriceDistinctCount", statements[1]);        
        }

        [Fact]
        public void ParseApplyWithAggregateMissingOpenParenShouldThrow()
        {
            var apply = "aggregate UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(10, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateMissingCloseParenShouldThrow()
        {
            var apply = "aggregate(UnitPrice with sum as TotalPrice";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateMissingStatementShouldThrow()
        {
            var apply = "aggregate()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(apply.Length - 1, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateAfterGroupByMissingStatementShouldThrow()
        {
            var apply = "groupby((UnitPrice))/aggregate()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(apply.Length - 1, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingWithShouldThrow()
        {
            var apply = "aggregate(UnitPrice sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_WithExpected(20, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionWithInvalidAggregateExpressionWithShouldThrow()
        {
            var apply = "aggregate(UnitPrice mul with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_WithExpected(29, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionWithInvalidVerbShouldThrow()
        {
            var apply = "aggregate(UnitPrice with invalid as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_UnrecognizedWithVerb("invalid",25, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingAsShouldThrow()
        {
            var apply = "aggregate(UnitPrice with sum TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_AsExpected(29, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingAliasShouldThrow()
        {
            var apply = "aggregate(UnitPrice with sum as)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(apply.Length, apply));
        }

        private static void VerifyGroupByTokenProperties(IEnumerable<string> expectedEndPathIdentifiers, GroupByToken actual)
        {
            actual.Should().NotBeNull();

            if (expectedEndPathIdentifiers == null || !expectedEndPathIdentifiers.Any() )
            {
                actual.Properties.Should().HaveCount(0);
            }
            else
            {                
                actual.Properties.Should().HaveCount(expectedEndPathIdentifiers.Count());

                var expectedIdentifierList = expectedEndPathIdentifiers.ToList();
                var i = 0;
                foreach (var actualProperty in actual.Properties)
                {
                    actualProperty.Should().NotBeNull();

                    var endPathToken = actualProperty as EndPathToken;
                    endPathToken.Should().NotBeNull();
                    endPathToken.Identifier.Should().Be(expectedIdentifierList[i]);
                    i++;
                }
            }
        }


        [Fact]
        public void ParseApplyWithSingleGroupByPropertyShouldReturnGroupByToken()
        {
            var apply = "groupby((UnitPrice))";

            var actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            var groupBy = actual.First() as GroupByToken;
            groupBy.Should().NotBeNull();

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);
        }

        [Fact]
        public void ParseApplyWithMultipleGroupByPropertiesShouldReturnGroupByToken()
        {
            var apply = "groupby((UnitPrice, SharePrice, ReservedPrice))";

            var actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            var groupBy = actual.First() as GroupByToken;

            VerifyGroupByTokenProperties(new string[] { "UnitPrice", "SharePrice", "ReservedPrice" }, groupBy);       
        }

        [Fact]
        public void ParseApplyWithGroupByAndAggregateShouldReturnGroupByToken()
        {
            var apply = "groupby((UnitPrice), aggregate(SalesPrice with average as RetailPrice))";

            var actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            var groupBy = actual.First() as GroupByToken;

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            groupBy.Child.Should().NotBeNull();

            var aggregate = groupBy.Child as AggregateToken;                        
            aggregate.Expressions.Should().HaveCount(1);

            VerifyAggregateExpressionToken("SalesPrice", AggregationMethod.Average, "RetailPrice", aggregate.Expressions.First());      
        }

        [Fact]
        public void ParseApplyWithGroupByMissingOpenParenShouldThrow()
        {
            var apply = "groupby (UnitPrice))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(9, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingCloseParenShouldThrow()
        {
            var apply = "groupby((UnitPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByAndAggregateMissingCloseParenShouldThrow()
        {
            var apply = "groupBy((UnitPrice), aggregate(UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected("aggregate|filter|groupby", 0, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingGroupingShouldThrow()
        {
            var apply = "groupby()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(8, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingGroupingParensShouldThrow()
        {
            var apply = "groupby(UnitPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(8, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByWithEmptyGroupingShouldThrow()
        {
            var apply = "groupby(())";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(9, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByWithChildGroupShouldThrow()
        {
            var apply = "groupby((UnitPrice), groupby((UnitPrice)))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected("aggregate", 21, apply));
        }


        private static void VerifyBinaryOperatorToken<T>(string expectedEndPathIdentifier, BinaryOperatorKind expectedOperator, T expectedLiteralValue, BinaryOperatorToken actual)
        {
            actual.Should().NotBeNull();
            actual.OperatorKind.Should().Be(expectedOperator);

            var left = actual.Left as EndPathToken;
            left.Should().NotBeNull();
            left.Identifier.Should().Be(expectedEndPathIdentifier);

            var right = actual.Right as LiteralToken;
            right.Should().NotBeNull();
            right.Value.Should().Be(expectedLiteralValue);
        }

        [Fact]
        public void ParseApplyWithSingleFilterByShouldReturnFilterExpression()
        {
            var apply = "filter(UnitPrice eq 5)";

            var actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            var filter = actual.First() as BinaryOperatorToken;
            VerifyBinaryOperatorToken<int>("UnitPrice", BinaryOperatorKind.Equal, 5, filter);
        }

        [Fact]
        public void ParseApplyWithFilterMissingOpenParenShouldThrow()
        {
            var apply = "filter UnitPrice eq 5)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(7, apply));
        }

        [Fact]
        public void ParseApplyWithFilterMissingCloseParenShouldThrow()
        {
            var apply = "filter(UnitPrice eq 5";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithFilterMissingExpressionShouldThrow()
        {
            var apply = "filter()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(7, apply));
        }

        [Fact]
        public void ParseApplyWithFilterInvalidExpressionShouldThrow()
        {
            var apply = "filter(UnitPrice eq)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(apply.Length - 1, apply));
        }


        [Fact]
        public void ParseApplyWithMultipleTransformationShouldReturnTransformations()
        {
            var apply = "groupby((UnitPrice), aggregate(SalesPrice with average as RetailPrice))/filter(UnitPrice eq 5)/aggregate(CustomerId with sum as Total, SharePrice with countdistinct as SharePriceDistinctCount)";

            var actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(3);

            var transformations = actual.ToList();

            // verify groupby
            var groupBy = transformations[0] as GroupByToken;
            groupBy.Should().NotBeNull();

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            
            groupBy.Properties.Should().HaveCount(1);
            groupBy.Child.Should().NotBeNull();

            var groupByAggregate = groupBy.Child as AggregateToken;
            groupByAggregate.Expressions.Should().HaveCount(1);
            VerifyAggregateExpressionToken("SalesPrice", AggregationMethod.Average, "RetailPrice", groupByAggregate.Expressions.First());      

            // verify filter
            var filter = transformations[1] as BinaryOperatorToken;

            VerifyBinaryOperatorToken<int>("UnitPrice", BinaryOperatorKind.Equal, 5, filter);            

            // verify aggregate         
            var aggregate = transformations[2] as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.Expressions.Should().HaveCount(2);

            var aggregateExpressions = aggregate.Expressions.ToList();            

            VerifyAggregateExpressionToken("CustomerId", AggregationMethod.Sum, "Total", aggregateExpressions[0]);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethod.CountDistinct, "SharePriceDistinctCount", aggregateExpressions[1]);
        }
    }
}