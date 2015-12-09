//---------------------------------------------------------------------
// <copyright file="UriQueryExpressionParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
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
    }
}