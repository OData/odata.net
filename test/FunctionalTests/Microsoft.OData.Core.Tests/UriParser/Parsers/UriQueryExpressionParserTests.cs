//---------------------------------------------------------------------
// <copyright file="UriQueryExpressionParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Parsers
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
            QueryToken tree = this.testSubject.ParseFilter("Things/any(o:true) and o");
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
            parse.ShouldThrow<ODataException>().Where(e => e.Message.Contains("Invalid spatial data"));
        }

        [Fact]
        public void ParseUnclosedGeometryPolygonShouldThrowWithReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geometry'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            parse.ShouldThrow<ODataException>().Where(e => e.Message.Contains("Invalid spatial data"));
        }

        [Fact]
        public void ParseGeometryPolygonWithBadPrefixShouldThrowWithoutReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geometr'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            parse.ShouldThrow<ODataException>().Where(e => !e.Message.Contains("with reason"));
        }   
        
        private static void VerifyAggregateExpressionToken(string expectedEndPathIdentifier, AggregationMethodDefinition expectedVerb, string expectedAlias, AggregateExpressionToken actual)
        {
            actual.Expression.Should().NotBeNull();

            EndPathToken expression = actual.Expression as EndPathToken;
            expression.Should().NotBeNull();
            expression.Identifier.Should().Be(expectedEndPathIdentifier);

            actual.MethodDefinition.MethodLabel.Should().Be(expectedVerb.MethodLabel);
            actual.MethodDefinition.MethodKind.Should().Be(expectedVerb.MethodKind);

            actual.Alias.Should().Be(expectedAlias);
        }

        [Fact]
        public void ParseApplyWithEmptyStringShouldReturnEmptyCollection()
        {
            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(string.Empty);
            actual.Should().NotBeNull();
            actual.Should().BeEmpty();
        }

        [Fact]
        public void ParseApplyWithInvalidTransformationIdentifierShouldThrow()
        {
            string apply = "invalid(UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected("aggregate|filter|groupby",0,apply));
        }

        [Fact]
        public void ParseApplyWithTrailingNotSlashShouldThrow()
        {
            string apply = "aggregate(UnitPrice with sum as TotalPrice),";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.ExpressionLexer_SyntaxError(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithSingleAggregateExpressionShouldReturnAggregateToken()
        {
            string apply = "aggregate(UnitPrice with sum as TotalPrice)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            AggregateToken aggregate = actual.First() as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);

            VerifyAggregateExpressionToken("UnitPrice", AggregationMethodDefinition.Sum, "TotalPrice", aggregate.AggregateExpressions.First() as AggregateExpressionToken);
            
        }

        [Fact]
        public void ParseApplyWithDottedUnkownExpressionShouldReturnCustomAggregateToken()
        {
            string apply = "aggregate(UnitPrice with Custom.Aggregate as CustomAggregate)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            AggregateToken aggregate = actual.First() as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);

            VerifyAggregateExpressionToken("UnitPrice", AggregationMethodDefinition.Custom("Custom.Aggregate"), "CustomAggregate", aggregate.AggregateExpressions.First() as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithMultipleAggregateExpressionsShouldReturnAggregateTokens()
        {
            string apply = "aggregate(CustomerId with sum as Total, SharePrice with countdistinct as SharePriceDistinctCount)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            AggregateToken aggregate = actual.First() as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(2);

            List<AggregateTokenBase> statements = aggregate.AggregateExpressions.ToList();
            
            VerifyAggregateExpressionToken("CustomerId", AggregationMethodDefinition.Sum, "Total", statements[0] as AggregateExpressionToken);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethodDefinition.CountDistinct, "SharePriceDistinctCount", statements[1] as AggregateExpressionToken);        
        }

        [Fact]
        public void ParseApplyWithSingleCountExpressionShouldReturnAggregateToken()
        {
            string apply = "aggregate($count as Count)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            AggregateToken aggregate = actual.First() as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(1);

            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", aggregate.AggregateExpressions.First() as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithSingleCountExpressionCannotHaveWithKeyWord()
        {
            string apply = "aggregate($count with sum as Count)";

            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_AsExpected(17, apply));
        }

        [Fact]
        public void ParseApplyWithCountAndOtherAggregationExpressionShouldReturnAggregateToken()
        {
            string apply = "aggregate($count as Count, SharePrice with countdistinct as SharePriceDistinctCount)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            AggregateToken aggregate = actual.First() as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(2);

            List<AggregateTokenBase> statements = aggregate.AggregateExpressions.ToList();

            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", aggregate.AggregateExpressions.First() as AggregateExpressionToken);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethodDefinition.CountDistinct, "SharePriceDistinctCount", statements[1] as AggregateExpressionToken);        
        }

        [Fact]
        public void ParseApplyWithAggregateMissingOpenParenShouldThrow()
        {
            string apply = "aggregate UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(10, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateMissingCloseParenShouldThrow()
        {
            string apply = "aggregate(UnitPrice with sum as TotalPrice";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateMissingStatementShouldThrow()
        {
            string apply = "aggregate()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(apply.Length - 1, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateAfterGroupByMissingStatementShouldThrow()
        {
            string apply = "groupby((UnitPrice))/aggregate()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(apply.Length - 1, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingWithShouldThrow()
        {
            string apply = "aggregate(UnitPrice sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_WithExpected(20, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionWithInvalidAggregateExpressionWithShouldThrow()
        {
            string apply = "aggregate(UnitPrice mul with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_WithExpected(29, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionWithInvalidVerbShouldThrow()
        {
            string apply = "aggregate(UnitPrice with invalid as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_UnrecognizedWithMethod("invalid", 25, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingAsShouldThrow()
        {
            string apply = "aggregate(UnitPrice with sum TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_AsExpected(29, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingAliasShouldThrow()
        {
            string apply = "aggregate(UnitPrice with sum as)";
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

                List<string> expectedIdentifierList = expectedEndPathIdentifiers.ToList();
                int i = 0;
                foreach (EndPathToken actualProperty in actual.Properties)
                {
                    actualProperty.Should().NotBeNull();

                    EndPathToken endPathToken = actualProperty as EndPathToken;
                    endPathToken.Should().NotBeNull();
                    endPathToken.Identifier.Should().Be(expectedIdentifierList[i]);
                    i++;
                }
            }
        }


        [Fact]
        public void ParseApplyWithSingleGroupByPropertyShouldReturnGroupByToken()
        {
            string apply = "groupby((UnitPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            GroupByToken groupBy = actual.First() as GroupByToken;
            groupBy.Should().NotBeNull();

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);
        }

        [Fact]
        public void ParseApplyWithMultipleGroupByPropertiesShouldReturnGroupByToken()
        {
            string apply = "groupby((UnitPrice, SharePrice, ReservedPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            GroupByToken groupBy = actual.First() as GroupByToken;

            VerifyGroupByTokenProperties(new string[] { "UnitPrice", "SharePrice", "ReservedPrice" }, groupBy);       
        }

        [Fact]
        public void ParseApplyWithGroupByAndAggregateShouldReturnGroupByToken()
        {
            string apply = "groupby((UnitPrice), aggregate(SalesPrice with average as RetailPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            GroupByToken groupBy = actual.First() as GroupByToken;

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            groupBy.Child.Should().NotBeNull();

            AggregateToken aggregate = groupBy.Child as AggregateToken;
            aggregate.AggregateExpressions.Should().HaveCount(1);

            VerifyAggregateExpressionToken("SalesPrice", AggregationMethodDefinition.Average, "RetailPrice", aggregate.AggregateExpressions.First() as AggregateExpressionToken);      
        }

        [Fact]
        public void ParseApplyWithGroupByMissingOpenParenShouldThrow()
        {
            string apply = "groupby (UnitPrice))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(9, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingCloseParenShouldThrow()
        {
            string apply = "groupby((UnitPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByAndAggregateMissingCloseParenShouldThrow()
        {
            string apply = "groupBy((UnitPrice), aggregate(UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected("aggregate|filter|groupby", 0, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingGroupingShouldThrow()
        {
            string apply = "groupby()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(8, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingGroupingParensShouldThrow()
        {
            string apply = "groupby(UnitPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(8, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByWithEmptyGroupingShouldThrow()
        {
            string apply = "groupby(())";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(9, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByWithChildGroupShouldThrow()
        {
            string apply = "groupby((UnitPrice), groupby((UnitPrice)))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected("aggregate", 21, apply));
        }


        private static void VerifyBinaryOperatorToken<T>(string expectedEndPathIdentifier, BinaryOperatorKind expectedOperator, T expectedLiteralValue, BinaryOperatorToken actual)
        {
            actual.Should().NotBeNull();
            actual.OperatorKind.Should().Be(expectedOperator);

            EndPathToken left = actual.Left as EndPathToken;
            left.Should().NotBeNull();
            left.Identifier.Should().Be(expectedEndPathIdentifier);

            LiteralToken right = actual.Right as LiteralToken;
            right.Should().NotBeNull();
            right.Value.Should().Be(expectedLiteralValue);
        }

        [Fact]
        public void ParseApplyWithSingleFilterByShouldReturnFilterExpression()
        {
            string apply = "filter(UnitPrice eq 5)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            BinaryOperatorToken filter = actual.First() as BinaryOperatorToken;
            VerifyBinaryOperatorToken<int>("UnitPrice", BinaryOperatorKind.Equal, 5, filter);
        }

        [Fact]
        public void ParseApplyWithFilterMissingOpenParenShouldThrow()
        {
            string apply = "filter UnitPrice eq 5)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_OpenParenExpected(7, apply));
        }

        [Fact]
        public void ParseApplyWithFilterMissingCloseParenShouldThrow()
        {
            string apply = "filter(UnitPrice eq 5";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithFilterMissingExpressionShouldThrow()
        {
            string apply = "filter()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(7, apply));
        }

        [Fact]
        public void ParseApplyWithFilterInvalidExpressionShouldThrow()
        {
            string apply = "filter(UnitPrice eq)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.ShouldThrow<ODataException>().Where(e => e.Message == ErrorStrings.UriQueryExpressionParser_ExpressionExpected(apply.Length - 1, apply));
        }


        [Fact]
        public void ParseApplyWithMultipleTransformationShouldReturnTransformations()
        {
            string apply = "groupby((UnitPrice), aggregate(SalesPrice with average as RetailPrice))/filter(UnitPrice eq 5)/aggregate(CustomerId with sum as Total, SharePrice with countdistinct as SharePriceDistinctCount)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(3);

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            groupBy.Should().NotBeNull();

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            
            groupBy.Properties.Should().HaveCount(1);
            groupBy.Child.Should().NotBeNull();

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            groupByAggregate.AggregateExpressions.Should().HaveCount(1);
            VerifyAggregateExpressionToken("SalesPrice", AggregationMethodDefinition.Average, "RetailPrice", groupByAggregate.AggregateExpressions.First() as AggregateExpressionToken);

            // verify filter
            BinaryOperatorToken filter = transformations[1] as BinaryOperatorToken;

            VerifyBinaryOperatorToken<int>("UnitPrice", BinaryOperatorKind.Equal, 5, filter);

            // verify aggregate
            AggregateToken aggregate = transformations[2] as AggregateToken;
            aggregate.Should().NotBeNull();
            aggregate.AggregateExpressions.Should().HaveCount(2);

            List<AggregateTokenBase> aggregateExpressions = aggregate.AggregateExpressions.ToList();

            VerifyAggregateExpressionToken("CustomerId", AggregationMethodDefinition.Sum, "Total", aggregateExpressions[0] as AggregateExpressionToken);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethodDefinition.CountDistinct, "SharePriceDistinctCount", aggregateExpressions[1] as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithNestedAggregation()
        {
            string apply = "groupby((UnitPrice), aggregate(Sales($count as Count)))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            groupBy.Should().NotBeNull();

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            groupBy.Properties.Should().HaveCount(1);
            groupBy.Child.Should().NotBeNull();

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            groupByAggregate.AggregateExpressions.Should().HaveCount(1);

            EntitySetAggregateToken entitySetAggregate = groupByAggregate.AggregateExpressions.First() as EntitySetAggregateToken;
            entitySetAggregate.Should().NotBeNull();

            entitySetAggregate.EntitySet.ShouldBeEndPathToken("Sales");
            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", entitySetAggregate.Expressions.First() as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithNestedAggregationAndFunction()
        {
            string apply = "groupby((UnitPrice), aggregate(Sales($count as Count),  cast(SalesPrice, Edm.Decimal)  with average as RetailPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            groupBy.Should().NotBeNull();

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            groupBy.Properties.Should().HaveCount(1);
            groupBy.Child.Should().NotBeNull();

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            groupByAggregate.AggregateExpressions.Should().HaveCount(2);

            EntitySetAggregateToken entitySetAggregate = groupByAggregate.AggregateExpressions.First() as EntitySetAggregateToken;
            entitySetAggregate.Should().NotBeNull();

            entitySetAggregate.EntitySet.ShouldBeEndPathToken("Sales");
            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", entitySetAggregate.Expressions.First() as AggregateExpressionToken);

            AggregateExpressionToken funcAggregate = groupByAggregate.AggregateExpressions.Last() as AggregateExpressionToken;
            funcAggregate.Should().NotBeNull();
            funcAggregate.Alias.ShouldBeEquivalentTo("RetailPrice");
            funcAggregate.Method.Should().Equals(AggregationMethodDefinition.Average);

            FunctionCallToken funcToken = funcAggregate.Expression as FunctionCallToken;
            funcToken.Should().NotBeNull();
            funcToken.Name.ShouldBeEquivalentTo("cast");
        }

        [Fact]
        public void ParseApplyWithNestedFunctionAggregation()
        {
            string apply = "groupby((UnitPrice), aggregate(Sales($count as Count,  cast(SalesPrice, Edm.Decimal)  with average as RetailPrice)))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            actual.Should().NotBeNull();
            actual.Should().HaveCount(1);

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            groupBy.Should().NotBeNull();

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            groupBy.Properties.Should().HaveCount(1);
            groupBy.Child.Should().NotBeNull();

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            groupByAggregate.AggregateExpressions.Should().HaveCount(1);

            EntitySetAggregateToken entitySetAggregate = groupByAggregate.AggregateExpressions.First() as EntitySetAggregateToken;
            entitySetAggregate.Should().NotBeNull();

            entitySetAggregate.EntitySet.ShouldBeEndPathToken("Sales");
            entitySetAggregate.Expressions.Should().HaveCount(2);
            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", entitySetAggregate.Expressions.First() as AggregateExpressionToken);

            AggregateExpressionToken funcAggregate = entitySetAggregate.Expressions.Last() as AggregateExpressionToken;
            funcAggregate.Should().NotBeNull();
            funcAggregate.Alias.ShouldBeEquivalentTo("RetailPrice");
            funcAggregate.Method.Should().Equals(AggregationMethodDefinition.Average);

            FunctionCallToken funcToken = funcAggregate.Expression as FunctionCallToken;
            funcToken.Should().NotBeNull();
            funcToken.Name.ShouldBeEquivalentTo("cast");
        }

        [Fact]
        public void ParseComputeWithMathematicalOperations()
        {
            string compute = "Prop1 mul Prop2 as Product,Prop1 div Prop2 as Ratio,Prop2 mod Prop2 as Remainder";
            ComputeToken token = this.testSubject.ParseCompute(compute);
            token.Kind.ShouldBeEquivalentTo(QueryTokenKind.Compute);
            List<ComputeExpressionToken> tokens = token.Expressions.ToList();
            tokens.Count.Should().Be(3);
            tokens[0].Kind.ShouldBeEquivalentTo(QueryTokenKind.ComputeExpression);
            tokens[1].Kind.ShouldBeEquivalentTo(QueryTokenKind.ComputeExpression);
            tokens[2].Kind.ShouldBeEquivalentTo(QueryTokenKind.ComputeExpression);
            tokens[0].Alias.ShouldBeEquivalentTo("Product");
            tokens[1].Alias.ShouldBeEquivalentTo("Ratio");
            tokens[2].Alias.ShouldBeEquivalentTo("Remainder");
            (tokens[0].Expression as BinaryOperatorToken).OperatorKind.ShouldBeEquivalentTo(BinaryOperatorKind.Multiply);
            (tokens[1].Expression as BinaryOperatorToken).OperatorKind.ShouldBeEquivalentTo(BinaryOperatorKind.Divide);
            (tokens[2].Expression as BinaryOperatorToken).OperatorKind.ShouldBeEquivalentTo(BinaryOperatorKind.Modulo);

            Action accept1 = () => tokens[0].Accept<ComputeExpression>(null);
            accept1.ShouldThrow<NotImplementedException>();
            Action accept2 = () => token.Accept<ComputeExpression>(null);
            accept2.ShouldThrow<NotImplementedException>();
        }
    }
}