//---------------------------------------------------------------------
// <copyright file="UriQueryExpressionParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;
using Microsoft.OData.Core;

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
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_SyntaxError, "13", "Things/any(a,true)"));
        }

        [Fact]
        public void AnyAllSyntacticParsingShouldNotAllowImplicitRangeVariableToBeRedefined()
        {
            // Repro for: Syntactic parser fails to block $it as a range variable name
            Action parse = () => this.testSubject.ParseFilter("Things/any($it:true)");
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_RangeVariableAlreadyDeclared, "$it"));
        }

        [Fact]
        public void AnyAllSyntacticParsingShouldNotAllowAnyRangeVariableToBeRedefined()
        {
            // Repro for: Semantic binding fails with useless error message when a range variable is redefined within a nested any/all
            Action parse = () => this.testSubject.ParseFilter("Things/any(o:o/Things/any(o:true))");
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_RangeVariableAlreadyDeclared, "o"));
        }

        [Fact]
        public void AnyAllSyntacticParsingShouldNotRecognizeRangeVariablesOutsideOfScope()
        {
            // Repro for: Syntactic parser assumes any token which matches the name of a previously used range variable is also a range variable, even after the scope has been exited
            QueryToken tree = this.testSubject.ParseFilter("Things/any(o:true) and o");
            tree.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).Right.ShouldBeEndPathToken("o");
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
        public void QualifiedFunctionNamedAfterOperatorShouldParseSuccessfully()
        {
            this.testSubject.ParseFilter("fq.ns.in()").ShouldBeFunctionCallToken("fq.ns.in");
            this.testSubject.ParseFilter("fq.ns.eq()").ShouldBeFunctionCallToken("fq.ns.eq");
            this.testSubject.ParseFilter("fq.ns.ne()").ShouldBeFunctionCallToken("fq.ns.ne");
            this.testSubject.ParseFilter("fq.ns.any()").ShouldBeFunctionCallToken("fq.ns.any");
            this.testSubject.ParseFilter("fq.ns.all()").ShouldBeFunctionCallToken("fq.ns.all");
        }

        [Fact]
        public void QualifiedFunctionNameWithParameterShouldParseSuccessfully()
        {
            this.testSubject.ParseFilter("fq.ns.container.function(arg=1)")
                .ShouldBeFunctionCallToken("fq.ns.container.function")
                .ShouldHaveParameter("arg")
                .ValueToken.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void ParseUnclosedGeographyPolygonThrowsWithReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geography'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            var exception = Assert.Throws<ODataException>(parse);
            Assert.Contains("points must form a closed linestring", exception.Message);
        }

        [Fact]
        public void ParseUnclosedGeometryPolygonThrowsWithReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geometry'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            var exception = Assert.Throws<ODataException>(parse);
            Assert.Contains("points must form a closed linestring", exception.Message);
        }

        [Fact]
        public void ParseGeometryPolygonWithBadPrefixThrowsWithoutReason()
        {
            Action parse = () => this.testSubject.ParseFilter("geo.intersects(GeoTag, geometr'POLYGON((-148.734375 71.459124,-43.265625 71.459124,-43.265625 -8.109371,-148.734375 -8.109371))')");
            var exception = Assert.Throws<ODataException>(parse);
            Assert.Contains("Unrecognized 'Edm.String' literal 'geometr'POLYGON((-148.734375 71.4591", exception.Message);
        }

        private static void VerifyAggregateExpressionToken(string expectedEndPathIdentifier, AggregationMethodDefinition expectedVerb, string expectedAlias, AggregateExpressionToken actual)
        {
            Assert.NotNull(actual.Expression);

            EndPathToken expression = actual.Expression as EndPathToken;
            Assert.NotNull(expression);
            Assert.Equal(expectedEndPathIdentifier, expression.Identifier);

            Assert.Equal(expectedVerb.MethodLabel, actual.MethodDefinition.MethodLabel);
            Assert.Equal(expectedVerb.MethodKind, actual.MethodDefinition.MethodKind);

            Assert.Equal(expectedAlias, actual.Alias);
        }

        [Fact]
        public void ParseApplyWithEmptyStringShouldReturnEmptyCollection()
        {
            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(string.Empty);
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void ParseApplyWithInvalidTransformationIdentifierThrows()
        {
            string apply = "invalid(UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_KeywordOrIdentifierExpected, "aggregate|filter|groupby|compute|expand", 0,apply));
        }

        [Fact]
        public void ParseApplyWithTrailingNotSlashThrows()
        {
            string apply = "aggregate(UnitPrice with sum as TotalPrice),";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_SyntaxError, apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithSingleAggregateExpressionShouldReturnAggregateToken()
        {
            string apply = "aggregate(UnitPrice with sum as TotalPrice)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            var token = Assert.Single(actual);

            AggregateToken aggregate = Assert.IsType<AggregateToken>(token);
            var aggregateExpr = Assert.Single(aggregate.AggregateExpressions);

            VerifyAggregateExpressionToken("UnitPrice", AggregationMethodDefinition.Sum, "TotalPrice", aggregateExpr as AggregateExpressionToken);
            
        }

        [Fact]
        public void ParseApplyWithDottedUnkownExpressionShouldReturnCustomAggregateToken()
        {
            string apply = "aggregate(UnitPrice with Custom.Aggregate as CustomAggregate)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            AggregateToken aggregate = actual.First() as AggregateToken;
            Assert.NotNull(aggregate);
            Assert.Single(aggregate.AggregateExpressions);

            VerifyAggregateExpressionToken("UnitPrice", AggregationMethodDefinition.Custom("Custom.Aggregate"), "CustomAggregate", aggregate.AggregateExpressions.First() as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithMultipleAggregateExpressionsShouldReturnAggregateTokens()
        {
            string apply = "aggregate(CustomerId with sum as Total, SharePrice with countdistinct as SharePriceDistinctCount)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            AggregateToken aggregate = actual.First() as AggregateToken;
            Assert.NotNull(aggregate);
            Assert.Equal(2, aggregate.AggregateExpressions.Count());

            List<AggregateTokenBase> statements = aggregate.AggregateExpressions.ToList();
            
            VerifyAggregateExpressionToken("CustomerId", AggregationMethodDefinition.Sum, "Total", statements[0] as AggregateExpressionToken);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethodDefinition.CountDistinct, "SharePriceDistinctCount", statements[1] as AggregateExpressionToken);        
        }

        [Fact]
        public void ParseApplyWithSingleCountExpressionShouldReturnAggregateToken()
        {
            string apply = "aggregate($count as Count)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            AggregateToken aggregate = actual.First() as AggregateToken;
            Assert.NotNull(aggregate);
            Assert.Single(aggregate.AggregateExpressions);

            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", aggregate.AggregateExpressions.First() as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithSingleCountExpressionCannotHaveWithKeyWord()
        {
            string apply = "aggregate($count with sum as Count)";

            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_AsExpected, 17, apply));
        }

        [Fact]
        public void ParseApplyWithCountAndOtherAggregationExpressionShouldReturnAggregateToken()
        {
            string apply = "aggregate($count as Count, SharePrice with countdistinct as SharePriceDistinctCount)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            AggregateToken aggregate = actual.First() as AggregateToken;
            Assert.NotNull(aggregate);
            Assert.Equal(2, aggregate.AggregateExpressions.Count());

            List<AggregateTokenBase> statements = aggregate.AggregateExpressions.ToList();

            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", aggregate.AggregateExpressions.First() as AggregateExpressionToken);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethodDefinition.CountDistinct, "SharePriceDistinctCount", statements[1] as AggregateExpressionToken);        
        }

        [Fact]
        public void ParseApplyWithAggregateMissingOpenParenThrows()
        {
            string apply = "aggregate UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_OpenParenExpected, 10, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateMissingCloseParenThrows()
        {
            string apply = "aggregate(UnitPrice with sum as TotalPrice";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_CloseParenOrCommaExpected, apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateMissingStatementThrows()
        {
            string apply = "aggregate()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionExpected, apply.Length - 1, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateAfterGroupByMissingStatementThrows()
        {
            string apply = "groupby((UnitPrice))/aggregate()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionExpected, apply.Length - 1, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingWithThrows()
        {
            string apply = "aggregate(UnitPrice sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_WithExpected, 20, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionWithInvalidAggregateExpressionWithThrows()
        {
            string apply = "aggregate(UnitPrice mul with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_WithExpected, 29, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionWithInvalidVerbThrows()
        {
            string apply = "aggregate(UnitPrice with invalid as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_UnrecognizedWithMethod, "invalid", 25, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingAsThrows()
        {
            string apply = "aggregate(UnitPrice with sum TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_AsExpected, 29, apply));
        }

        [Fact]
        public void ParseApplyWithAggregateExpressionMissingAliasThrows()
        {
            string apply = "aggregate(UnitPrice with sum as)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_CloseParenOrCommaExpected, apply.Length, apply));
        }

        private static void VerifyGroupByTokenProperties(IEnumerable<string> expectedEndPathIdentifiers, GroupByToken actual)
        {
            Assert.NotNull(actual);

            if (expectedEndPathIdentifiers == null || !expectedEndPathIdentifiers.Any() )
            {
                Assert.Empty(actual.Properties);
            }
            else
            {
                Assert.Equal(actual.Properties.Count(), expectedEndPathIdentifiers.Count());

                List<string> expectedIdentifierList = expectedEndPathIdentifiers.ToList();
                int i = 0;
                foreach (EndPathToken actualProperty in actual.Properties)
                {
                    Assert.NotNull(actualProperty);

                    EndPathToken endPathToken = actualProperty as EndPathToken;
                    Assert.NotNull(endPathToken);
                    Assert.Equal(endPathToken.Identifier, expectedIdentifierList[i]);
                    i++;
                }
            }
        }


        [Fact]
        public void ParseApplyWithSingleGroupByPropertyShouldReturnGroupByToken()
        {
            string apply = "groupby((UnitPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            GroupByToken groupBy = actual.First() as GroupByToken;
            Assert.NotNull(groupBy);

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);
        }

        [Fact]
        public void ParseApplyWithMultipleGroupByPropertiesShouldReturnGroupByToken()
        {
            string apply = "groupby((UnitPrice, SharePrice, ReservedPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            GroupByToken groupBy = actual.First() as GroupByToken;

            VerifyGroupByTokenProperties(new string[] { "UnitPrice", "SharePrice", "ReservedPrice" }, groupBy);       
        }

        [Fact]
        public void ParseApplyWithGroupByAndAggregateShouldReturnGroupByToken()
        {
            string apply = "groupby((UnitPrice), aggregate(SalesPrice with average as RetailPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            GroupByToken groupBy = actual.First() as GroupByToken;

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            Assert.NotNull(groupBy.Child);

            AggregateToken aggregate = groupBy.Child as AggregateToken;
            Assert.Single(aggregate.AggregateExpressions);

            VerifyAggregateExpressionToken("SalesPrice", AggregationMethodDefinition.Average, "RetailPrice", aggregate.AggregateExpressions.First() as AggregateExpressionToken);      
        }

        [Fact]
        public void ParseApplyWithGroupByMissingOpenParenThrows()
        {
            string apply = "groupby (UnitPrice))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_OpenParenExpected, 9, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingCloseParenThrows()
        {
            string apply = "groupby((UnitPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_CloseParenOrCommaExpected, apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByAndAggregateMissingCloseParenThrows()
        {
            string apply = "groupBy((UnitPrice), aggregate(UnitPrice with sum as TotalPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_KeywordOrIdentifierExpected, "aggregate|filter|groupby|compute|expand", 0, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingGroupingThrows()
        {
            string apply = "groupby()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_OpenParenExpected, 8, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByMissingGroupingParensThrows()
        {
            string apply = "groupby(UnitPrice)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_OpenParenExpected, 8, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByWithEmptyGroupingThrows()
        {
            string apply = "groupby(())";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionExpected, 9, apply));
        }

        [Fact]
        public void ParseApplyWithGroupByWithChildGroupThrows()
        {
            string apply = "groupby((UnitPrice), groupby((UnitPrice)))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_KeywordOrIdentifierExpected, "aggregate", 21, apply));
        }


        private static void VerifyBinaryOperatorToken<T>(string expectedEndPathIdentifier, BinaryOperatorKind expectedOperator, T expectedLiteralValue, BinaryOperatorToken actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(actual.OperatorKind, expectedOperator);

            EndPathToken left = actual.Left as EndPathToken;
            Assert.NotNull(left);
            Assert.Equal(left.Identifier, expectedEndPathIdentifier);

            LiteralToken right = actual.Right as LiteralToken;
            Assert.NotNull(right);
            Assert.Equal(right.Value, expectedLiteralValue);
        }

        [Fact]
        public void ParseApplyWithSingleFilterByShouldReturnFilterExpression()
        {
            string apply = "filter(UnitPrice eq 5)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            BinaryOperatorToken filter = actual.First() as BinaryOperatorToken;
            VerifyBinaryOperatorToken<int>("UnitPrice", BinaryOperatorKind.Equal, 5, filter);
        }

        [Fact]
        public void ParseApplyWithFilterMissingOpenParenThrows()
        {
            string apply = "filter UnitPrice eq 5)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_OpenParenExpected, 7, apply));
        }

        [Fact]
        public void ParseApplyWithFilterMissingCloseParenThrows()
        {
            string apply = "filter(UnitPrice eq 5";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_CloseParenOrOperatorExpected, apply.Length, apply));
        }

        [Fact]
        public void ParseApplyWithFilterMissingExpressionThrows()
        {
            string apply = "filter()";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionExpected, 7, apply));
        }

        [Fact]
        public void ParseApplyWithFilterInvalidExpressionThrows()
        {
            string apply = "filter(UnitPrice eq)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionExpected, apply.Length - 1, apply));
        }


        [Fact]
        public void ParseApplyWithMultipleTransformationShouldReturnTransformations()
        {
            string apply = "groupby((UnitPrice), aggregate(SalesPrice with average as RetailPrice))/filter(UnitPrice eq 5)/aggregate(CustomerId with sum as Total, SharePrice with countdistinct as SharePriceDistinctCount)";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Equal(3, actual.Count());

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            Assert.NotNull(groupBy);

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            Assert.Single(groupBy.Properties);
            Assert.NotNull(groupBy.Child);

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            Assert.Single(groupByAggregate.AggregateExpressions);
            VerifyAggregateExpressionToken("SalesPrice", AggregationMethodDefinition.Average, "RetailPrice", groupByAggregate.AggregateExpressions.First() as AggregateExpressionToken);

            // verify filter
            BinaryOperatorToken filter = transformations[1] as BinaryOperatorToken;

            VerifyBinaryOperatorToken<int>("UnitPrice", BinaryOperatorKind.Equal, 5, filter);

            // verify aggregate
            AggregateToken aggregate = transformations[2] as AggregateToken;
            Assert.NotNull(aggregate);
            Assert.Equal(2, aggregate.AggregateExpressions.Count());

            List<AggregateTokenBase> aggregateExpressions = aggregate.AggregateExpressions.ToList();

            VerifyAggregateExpressionToken("CustomerId", AggregationMethodDefinition.Sum, "Total", aggregateExpressions[0] as AggregateExpressionToken);
            VerifyAggregateExpressionToken("SharePrice", AggregationMethodDefinition.CountDistinct, "SharePriceDistinctCount", aggregateExpressions[1] as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithNestedAggregation()
        {
            string apply = "groupby((UnitPrice), aggregate(Sales($count as Count)))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            Assert.NotNull(groupBy);

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            Assert.Single(groupBy.Properties);
            Assert.NotNull(groupBy.Child);

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            Assert.Single(groupByAggregate.AggregateExpressions);

            EntitySetAggregateToken entitySetAggregate = groupByAggregate.AggregateExpressions.First() as EntitySetAggregateToken;
            Assert.NotNull(entitySetAggregate);

            entitySetAggregate.EntitySet.ShouldBeEndPathToken("Sales");
            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", entitySetAggregate.Expressions.First() as AggregateExpressionToken);
        }

        [Fact]
        public void ParseApplyWithNestedAggregationAndFunction()
        {
            string apply = "groupby((UnitPrice), aggregate(Sales($count as Count),  cast(SalesPrice, Edm.Decimal)  with average as RetailPrice))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            Assert.NotNull(groupBy);

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            Assert.Single(groupBy.Properties);
            Assert.NotNull(groupBy.Child);

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            Assert.Equal(2, groupByAggregate.AggregateExpressions.Count());

            EntitySetAggregateToken entitySetAggregate = groupByAggregate.AggregateExpressions.First() as EntitySetAggregateToken;
            Assert.NotNull(entitySetAggregate);

            entitySetAggregate.EntitySet.ShouldBeEndPathToken("Sales");
            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", entitySetAggregate.Expressions.First() as AggregateExpressionToken);

            AggregateExpressionToken funcAggregate = groupByAggregate.AggregateExpressions.Last() as AggregateExpressionToken;
            Assert.NotNull(funcAggregate);
            Assert.Equal("RetailPrice", funcAggregate.Alias);
            Assert.Equal(AggregationMethod.Average, funcAggregate.Method);

            FunctionCallToken funcToken = funcAggregate.Expression as FunctionCallToken;
            Assert.NotNull(funcToken);
            Assert.Equal("cast", funcToken.Name);
        }

        [Fact]
        public void ParseApplyWithNestedFunctionAggregation()
        {
            string apply = "groupby((UnitPrice), aggregate(Sales($count as Count,  cast(SalesPrice, Edm.Decimal)  with average as RetailPrice)))";

            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            List<QueryToken> transformations = actual.ToList();

            // verify groupby
            GroupByToken groupBy = transformations[0] as GroupByToken;
            Assert.NotNull(groupBy);

            VerifyGroupByTokenProperties(new string[] { "UnitPrice" }, groupBy);

            Assert.Single(groupBy.Properties);
            Assert.NotNull(groupBy.Child);

            AggregateToken groupByAggregate = groupBy.Child as AggregateToken;
            Assert.Single(groupByAggregate.AggregateExpressions);

            EntitySetAggregateToken entitySetAggregate = groupByAggregate.AggregateExpressions.First() as EntitySetAggregateToken;
            Assert.NotNull(entitySetAggregate);

            entitySetAggregate.EntitySet.ShouldBeEndPathToken("Sales");
            Assert.Equal(2, entitySetAggregate.Expressions.Count());
            VerifyAggregateExpressionToken("$count", AggregationMethodDefinition.VirtualPropertyCount, "Count", entitySetAggregate.Expressions.First() as AggregateExpressionToken);

            AggregateExpressionToken funcAggregate = entitySetAggregate.Expressions.Last() as AggregateExpressionToken;
            Assert.NotNull(funcAggregate);
            Assert.Equal("RetailPrice", funcAggregate.Alias);
            Assert.Equal(AggregationMethod.Average, funcAggregate.Method);

            FunctionCallToken funcToken = funcAggregate.Expression as FunctionCallToken;
            Assert.NotNull(funcToken);
            Assert.Equal("cast", funcToken.Name);
        }

        [Fact]
        public void ParseApplyWithComputeWithMathematicalOperations()
        {
            string compute = "compute(Prop1 mul Prop2 as Product,Prop1 div Prop2 as Ratio,Prop2 mod Prop2 as Remainder)";
            ComputeToken token = this.testSubject.ParseApply(compute).First() as ComputeToken;
            Assert.Equal(QueryTokenKind.Compute, token.Kind);
            List<ComputeExpressionToken> tokens = token.Expressions.ToList();
            Assert.Equal(3, tokens.Count);
            Assert.Equal(QueryTokenKind.ComputeExpression, tokens[0].Kind);
            Assert.Equal(QueryTokenKind.ComputeExpression, tokens[1].Kind);
            Assert.Equal(QueryTokenKind.ComputeExpression, tokens[2].Kind);
            Assert.Equal("Product", tokens[0].Alias);
            Assert.Equal("Ratio", tokens[1].Alias);
            Assert.Equal("Remainder", tokens[2].Alias);
            Assert.Equal(BinaryOperatorKind.Multiply, (tokens[0].Expression as BinaryOperatorToken).OperatorKind);
            Assert.Equal(BinaryOperatorKind.Divide, (tokens[1].Expression as BinaryOperatorToken).OperatorKind);
            Assert.Equal(BinaryOperatorKind.Modulo, (tokens[2].Expression as BinaryOperatorToken).OperatorKind);

            Action accept1 = () => tokens[0].Accept<ComputeExpression>(null);
            Assert.Throws<NotImplementedException>(accept1);
            Action accept2 = () => token.Accept<ComputeExpression>(null);
            Assert.Throws<NotImplementedException>(accept2);
        }

        [Fact]
        public void ParseComputeWithMathematicalOperations()
        {
            string compute = "Prop1 mul Prop2 as Product,Prop1 div Prop2 as Ratio,Prop2 mod Prop2 as Remainder";
            ComputeToken token = this.testSubject.ParseCompute(compute);
            Assert.Equal(QueryTokenKind.Compute, token.Kind);
            List<ComputeExpressionToken> tokens = token.Expressions.ToList();
            Assert.Equal(3, tokens.Count);
            Assert.Equal(QueryTokenKind.ComputeExpression, tokens[0].Kind);
            Assert.Equal(QueryTokenKind.ComputeExpression, tokens[1].Kind);
            Assert.Equal(QueryTokenKind.ComputeExpression, tokens[2].Kind);
            Assert.Equal("Product", tokens[0].Alias);
            Assert.Equal("Ratio", tokens[1].Alias);
            Assert.Equal("Remainder", tokens[2].Alias);
            Assert.Equal(BinaryOperatorKind.Multiply, (tokens[0].Expression as BinaryOperatorToken).OperatorKind);
            Assert.Equal(BinaryOperatorKind.Divide, (tokens[1].Expression as BinaryOperatorToken).OperatorKind);
            Assert.Equal(BinaryOperatorKind.Modulo, (tokens[2].Expression as BinaryOperatorToken).OperatorKind);

            Action accept1 = () => tokens[0].Accept<ComputeExpression>(null);
            Assert.Throws<NotImplementedException>(accept1);
            Action accept2 = () => token.Accept<ComputeExpression>(null);
            Assert.Throws<NotImplementedException>(accept2);
        }

        [Fact]
        public void ParseApplyWithOnlyExpandThrows()
        {
            string apply = "expand(Sales)";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_InnerMostExpandRequireFilter, apply.Length - 1, apply));
        }

        [Fact]
        public void ParseApplyWithExpandFollowedByAggregateShouldParseSuccessfully()
        {
            string apply = "expand(Sales, filter(Amount gt 3))/aggregate($count as Count)";
            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());

            ExpandToken expand = (ExpandToken)actual.First();
            Assert.Single(expand.ExpandTerms);

            ExpandTermToken expandTerm = expand.ExpandTerms.First();
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTerm.Kind);
            Assert.Equal("Sales", expandTerm.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm.FilterOption);

            AggregateToken aggregate = (AggregateToken)actual.Last();
            Assert.Single(aggregate.AggregateExpressions);
        }

        [Fact]
        public void ParseApplyWithFilteredExpandShouldParseSuccessfully()
        {
            string apply = "expand(Sales, filter(Amount gt 3))";
            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            ExpandToken expand = (ExpandToken)actual.First();
            Assert.Single(expand.ExpandTerms);

            ExpandTermToken expandTerm = expand.ExpandTerms.First();
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTerm.Kind);
            Assert.Equal("Sales", expandTerm.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm.FilterOption);
        }

        [Fact]
        public void ParseApplyWithNestedExpandShouldParseSuccessfully()
        {
            string apply = "expand(Sales, expand(Customers, filter(City eq 'Seattle')))";
            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            ExpandToken expand = (ExpandToken)actual.First();
            Assert.Single(expand.ExpandTerms);

            ExpandTermToken expandTerm = expand.ExpandTerms.First();
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTerm.Kind);
            Assert.Equal("Sales", expandTerm.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm.ExpandOption);
            Assert.Single(expandTerm.ExpandOption.ExpandTerms);
            expandTerm = expandTerm.ExpandOption.ExpandTerms.First();
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTerm.Kind);
            Assert.Equal("Customers", expandTerm.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm.FilterOption);
        }

        [Fact]
        public void ParseApplyWithNestedExpandOnlyShowThrowOnInnerMost()
        {
            string apply = "expand(Sales, expand(Customers))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_InnerMostExpandRequireFilter, apply.Length - 2, apply));
        }


        [Fact]
        public void ParseApplyWithMultipleNestedExpandsOnlyThrowsOnFirstLeaf()
        {
            string apply = "expand(Sales, expand(Customers), expand(Cashiers))";
            Action parse = () => this.testSubject.ParseApply(apply);
            parse.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_InnerMostExpandRequireFilter, apply.IndexOf(")"), apply));

        }

        [Fact]
        public void ParseApplyWithMultipleNestedExpandFiltersAndLevelsShouldParseSuccessfully()
        {
            string apply = "expand(Sales, expand(Customers, filter(City eq 'Redmond')), expand(Cashiers, expand(Stores, filter(City eq 'Seattle'))))";
            IEnumerable<QueryToken> actual = this.testSubject.ParseApply(apply);
            Assert.NotNull(actual);
            Assert.Single(actual);

            ExpandToken expand = (ExpandToken)actual.First();
            Assert.Single(expand.ExpandTerms);

            ExpandTermToken expandTerm = expand.ExpandTerms.First();
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTerm.Kind);
            Assert.Equal("Sales", expandTerm.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm.ExpandOption);
            Assert.Equal(2, expandTerm.ExpandOption.ExpandTerms.Count());
            ExpandTermToken expandTerm1 = expandTerm.ExpandOption.ExpandTerms.First();
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTerm1.Kind);
            Assert.Equal("Customers", expandTerm1.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm1.FilterOption);

            ExpandTermToken expandTerm2 = expandTerm.ExpandOption.ExpandTerms.Last();
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTerm2.Kind);
            Assert.Equal("Cashiers", expandTerm2.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm2.ExpandOption);

            Assert.Single(expandTerm2.ExpandOption.ExpandTerms);
            ExpandTermToken expandTerm3 = expandTerm2.ExpandOption.ExpandTerms.First();
            Assert.Equal("Stores", expandTerm3.PathToNavigationProp.Identifier);
            Assert.NotNull(expandTerm3.FilterOption);
        }
    }
}