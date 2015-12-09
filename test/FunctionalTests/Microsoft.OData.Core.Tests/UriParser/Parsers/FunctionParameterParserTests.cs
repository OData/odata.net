//---------------------------------------------------------------------
// <copyright file="FunctionParameterParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// TODO: cover everything in FunctionParameterParser once it replaces the code being added for filter/orderby.
    /// </summary>
    public class FunctionParameterParserTests
    {
        [Fact]
        public void FunctionParameterParserShouldSupportUnresolvedAliasesInPath()
        {
            ICollection<OperationSegmentParameter> parsedParameters;
            TryParseFunctionParameters("CanMoveToAddress", "address=@a", null /*resolveAlias*/, out parsedParameters).Should().BeTrue();
            parsedParameters.Should().HaveCount(1);
            parsedParameters.Single().Name.Should().Be("address");
            parsedParameters.Single().Value.As<ParameterAliasNode>().Alias.Should().Be("@a");
        }

        [Fact]
        public void FunctionParameterParserShouldResolveAliasesInPathIfParameterAliasAccessorProvided()
        {
            Dictionary<string, string> aliasValues = new Dictionary<string, string>()
            {
                { "@a", "true" }
            };
            ParameterAliasValueAccessor paramAliasAccessor = new ParameterAliasValueAccessor(aliasValues);
            ICollection<OperationSegmentParameter> parsedParameters;
            TryParseOperationParameters("HasDog", "inOffice=@a", paramAliasAccessor, HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters(), out parsedParameters).Should().BeTrue();
            parsedParameters.Should().HaveCount(1);
            parsedParameters.Single().Name.Should().Be("inOffice");
            parsedParameters.Single().Value.As<ConvertNode>().Source.As<ParameterAliasNode>().Alias.Should().Be("@a");

            // verify alias value node:
            paramAliasAccessor.ParameterAliasValueNodesCached["@a"].ShouldBeConstantQueryNode(true);
        }

        [Fact]
        public void FunctionParameterParserShouldHandleTheResolvedAliasBeingNull()
        {
            Dictionary<string, string> aliasValues = new Dictionary<string, string>()
            {
                { "@a", "null" }
            };
            ParameterAliasValueAccessor paramAliasAccessor = new ParameterAliasValueAccessor(aliasValues);
            ICollection<OperationSegmentParameter> parsedParameters;
            TryParseFunctionParameters("CanMoveToAddress", "address=@a", paramAliasAccessor, out parsedParameters).Should().BeTrue();
            parsedParameters.Should().HaveCount(1);
            parsedParameters.Single().ShouldHaveValueType<ParameterAliasNode>("address").And.Alias.Should().Be("@a");

            // verify alias value:
            paramAliasAccessor.ParameterAliasValueNodesCached["@a"].As<ConstantNode>().Value.Should().Be(null);
        }

        [Fact]
        public void FunctionParameterParserShouldSupportBracketedExpressionsInPath()
        {
            ICollection<OperationSegmentParameter> parsedParameters;
            TryParseFunctionParameters("CanMoveToAddress", "address={\'City\' : \'Seattle\'}", null, out parsedParameters).Should().BeTrue();
            parsedParameters.Should().HaveCount(1);
            parsedParameters.Single().ShouldBeConstantParameterWithValueType<ODataComplexValue>("address");
        }

        [Fact]
        public void FunctionParameterParserShouldSupportBracketedExpressionsInFilterOrderby()
        {
            ExpressionLexer lexer = new ExpressionLexer("address={\'City\' : \'Seattle\'})", true, false, false);
            ICollection<NamedFunctionParameterNode> parameterNodes;
            TryParseFunctionParameters(lexer, null, out parameterNodes).Should().BeTrue();
            parameterNodes.Should().HaveCount(1);
            var parameter = parameterNodes.Single();
            parameter.Name.Should().Be("address");
            parameter.Value.As<ConstantNode>().Value.Should().BeOfType<ODataComplexValue>();
        }

        [Fact]
        public void FunctionParameterParserShouldResolveAliasesInFilterOrderby()
        {
            Dictionary<string, string> aliasValues = new Dictionary<string, string>()
            {
                { "@a", "null" }
            };
            ParameterAliasValueAccessor paramAliasAccessor = new ParameterAliasValueAccessor(aliasValues);
            ExpressionLexer lexer = new ExpressionLexer("address=@a)", true, false, true);
            ICollection<NamedFunctionParameterNode> parameterTokens;
            TryParseFunctionParameters(lexer, paramAliasAccessor, out parameterTokens).Should().BeTrue();
            parameterTokens.Should().HaveCount(1);
            parameterTokens.Single().ShouldHaveParameterAliasNode("address", "@a");

            // verify alias value node:
            paramAliasAccessor.ParameterAliasValueNodesCached["@a"].ShouldBeConstantQueryNode((object)null);
        }

        [Fact]
        public void FunctionParameterParserShoulHandleUnResolvedAliasesInFilterOrderby()
        {
            ExpressionLexer lexer = new ExpressionLexer("address=@a)", true, false, true);
            ICollection<NamedFunctionParameterNode> parameterTokens;
            TryParseFunctionParameters(lexer, null, out parameterTokens).Should().BeTrue();
            parameterTokens.Should().HaveCount(1);
            parameterTokens.Single().ShouldHaveParameterAliasNode("address", "@a");
        }

        [Fact]
        public void NullFunctionParameterShouldParseCorrectly()
        {
            ICollection<OperationSegmentParameter> parsedParameters;
            TryParsOperationParameters("CanMoveToAddress", "address=null", null, out parsedParameters).Should().BeTrue();
            parsedParameters.Should().HaveCount(1);
            parsedParameters.Single().ShouldBeConstantParameterWithValueType("address", (object)null);
        }

        [Fact]
        public void GeometryParameterShouldParseCorrectly()
        {
            var point = GeometryPoint.Create(1.0, 2.0);
            ICollection<OperationSegmentParameter> parsedParameters;
            TryParseOperationParameters("GetColorAtPosition", "position=geometry'" + SpatialHelpers.WriteSpatial(point) + "',includeAlpha=true", null, HardCodedTestModel.GetColorAtPositionFunction(), out parsedParameters).Should().BeTrue();
            parsedParameters.Should().HaveCount(2);
            parsedParameters.First().ShouldBeConstantParameterWithValueType("position", point);
        }

        [Fact]
        public void GeographyParameterShouldParseCorrectly()
        {
            var point = GeographyPoint.Create(1.0, 2.0);
            ICollection<OperationSegmentParameter> parsedParameters;
            TryParseOperationParameters("GetNearbyPriorAddresses", "currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=50", null, HardCodedTestModel.GetNearbyPriorAddressesFunction(), out parsedParameters).Should().BeTrue();
            parsedParameters.Should().HaveCount(2);
            parsedParameters.First().ShouldBeConstantParameterWithValueType("currentLocation", point);
            parsedParameters.Last().ShouldBeConstantParameterWithValueType("limit", 50);
        }

        [Fact]
        public void FunctionParameterParserShouldNotAdvanceLexerIfNotANamedValue()
        {
            ExpressionLexer lexer = new ExpressionLexer("a?foo,bar", true, false, true);
            ICollection<NamedFunctionParameterNode> parameterTokens;
            TryParseFunctionParameters(lexer, null, out parameterTokens).Should().BeFalse();
            lexer.Position.Should().Be(0);
        }

        [Fact]
        public void TypedNullFunctionParameterParsingShouldThrow()
        {
            ICollection<OperationSegmentParameter> parsedParameters;
            Action parse = () => TryParsOperationParameters("CanMoveToAddress", "address=null'Fully.Qualified.Namespace.Address'", null, out parsedParameters).Should().BeTrue();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_SyntaxError(12, "address=null'Fully.Qualified.Namespace.Address'"));
        }

        [Fact]
        public void FunctionParameterParserShouldThrowIfParameterValueCannotBeParsed()
        {
            ICollection<OperationSegmentParameter> parsedParameters;
            Action parse = () => TryParseFunctionParameters("fakeFunc", "a='foo'", null, out parsedParameters);
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation("a", "IsAddressGood"));
        }

        [Fact]
        public void FunctionParameterParserShouldThrowIfSecondParameterIsPositional()
        {
            ICollection<OperationSegmentParameter> parsedParameters;
            Action parse = () => TryParseFunctionParameters("fakeFunc", "a=1,2", null, out parsedParameters);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_SyntaxError(5, "a=1,2"));
        }

        [Fact]
        public void FunctionParameterParserShouldSupportTemplateParser()
        {
            ICollection<OperationSegmentParameter> parsedParameters;
            IEdmFunction operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters();
            TryParseOperationParameters("HasDog", "inOffice={Da}", operation, out parsedParameters, true).Should().BeTrue();
            
            var parameter = parsedParameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression() { LiteralText = "{Da}", ExpectedType = operation.Parameters.Last().Type });
        }

        private static bool TryParseFunctionParameters(string functionName, string parenthesisExpression, ParameterAliasValueAccessor paramAliasAccessor, out ICollection<OperationSegmentParameter> parsedParameters)
        {
            return TryParseFunctionParameters(functionName, parenthesisExpression, paramAliasAccessor, HardCodedTestModel.GetFunctionImportIsAddressGood(), out parsedParameters);
        }

        private static bool TryParsOperationParameters(string functionName, string parenthesisExpression, ParameterAliasValueAccessor paramAliasAccessor, out ICollection<OperationSegmentParameter> parsedParameters)
        {
            return TryParseOperationParameters(functionName, parenthesisExpression, paramAliasAccessor, HardCodedTestModel.GetFunctionForCanMoveToAddress(), out parsedParameters);
        }

        private static bool TryParseOperationParameters(string functionName, string parenthesisExpression, ParameterAliasValueAccessor paramAliasAccessor, IEdmOperation operation, out ICollection<OperationSegmentParameter> parsedSegementParameters)
        {
            ICollection<FunctionParameterToken> splitParameters;
            parsedSegementParameters = null;
            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { ParameterAliasValueAccessor = paramAliasAccessor };
            if (FunctionParameterParser.TrySplitOperationParameters(parenthesisExpression, configuration, out splitParameters))
            {
                parsedSegementParameters = FunctionCallBinder.BindSegmentParameters(configuration, operation, splitParameters);
                return true;
            }

            return false;
        }

        private static bool TryParseOperationParameters(string functionName, string parenthesisExpression, IEdmOperation operation, out ICollection<OperationSegmentParameter> parsedSegementParameters, bool enableUriTemplateParsing = false)
        {
            ICollection<FunctionParameterToken> splitParameters;
            parsedSegementParameters = null;
            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { EnableUriTemplateParsing = enableUriTemplateParsing };
            if (FunctionParameterParser.TrySplitOperationParameters(parenthesisExpression, configuration, out splitParameters))
            {
                parsedSegementParameters = FunctionCallBinder.BindSegmentParameters(configuration, operation, splitParameters);
                return true;
            }

            return false;
        }

        private static bool TryParseFunctionParameters(string functionName, string parenthesisExpression, ParameterAliasValueAccessor paramAliasAccessor, IEdmOperationImport operationImport, out ICollection<OperationSegmentParameter> parsedSegementParameters)
        {
            return TryParseOperationParameters(functionName, parenthesisExpression, paramAliasAccessor, operationImport.Operation, out parsedSegementParameters);
        }

        private static bool TryParseFunctionParameters(ExpressionLexer lexer, ParameterAliasValueAccessor paramAliasAccessor, out ICollection<NamedFunctionParameterNode> parsedParameterNodes)
        {
            UriQueryExpressionParser parser = new UriQueryExpressionParser(345, lexer);
            ICollection<FunctionParameterToken> splitParameters;
            parsedParameterNodes = null;
            if (parser.TrySplitFunctionParameters(out splitParameters))
            {
                var parsedParameters = FunctionCallBinder.BindSegmentParameters(new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { ParameterAliasValueAccessor = paramAliasAccessor }, HardCodedTestModel.GetFunctionImportIsAddressGood().Function, splitParameters);
                parsedParameterNodes = parsedParameters.Select(s => new NamedFunctionParameterNode(s.Name, s.Value as QueryNode)).ToList();
                return true;
            }

            return false;
        }

        [Fact]
        public void FunctionParameterParserShouldFailIfAnExtraClosingParenthesisIsFoundInPath()
        {
            ICollection<FunctionParameterToken> splitParameters;
            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { ParameterAliasValueAccessor = null };
            Action parse = () => FunctionParameterParser.TrySplitOperationParameters(/*"fakeFunc", */ "a=1)", configuration, out splitParameters);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_SyntaxError(4, "a=1)"));
        }
    }
}
