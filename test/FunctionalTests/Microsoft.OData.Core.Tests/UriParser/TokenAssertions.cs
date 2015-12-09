//---------------------------------------------------------------------
// <copyright file="TokenAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;

namespace Microsoft.OData.Core.Tests.UriParser
{
    /// <summary>
    /// Contains fluent assertion APIs for testing QueryTokens.
    /// TODO: Consider using T : QueryToken instead, and writing a test to assert that the QueryTokenKind matches the relevent class seperately.
    /// </summary>
    internal static class TokenAssertions
    {
        public static AndConstraint<AnyToken> ShouldBeAnyToken(this QueryToken token, string expectedParameterName)
        {
            token.Should().BeOfType<AnyToken>();
            var anyToken = token.As<AnyToken>();
            anyToken.Kind.Should().Be(QueryTokenKind.Any);
            anyToken.Parameter.Should().Be(expectedParameterName);
            return new AndConstraint<AnyToken>(anyToken);
        }

        public static AndConstraint<AllToken> ShouldBeAllToken(this QueryToken token, string expectedParameterName)
        {
            token.Should().BeOfType<AllToken>();
            var allToken = token.As<AllToken>();
            allToken.Kind.Should().Be(QueryTokenKind.Any);
            allToken.Parameter.Should().Be(expectedParameterName);
            return new AndConstraint<AllToken>(allToken);
        }

        public static AndConstraint<FunctionCallToken> ShouldBeFunctionCallToken(this QueryToken token, string name)
        {
            token.Should().BeOfType<FunctionCallToken>();
            var functionCallQueryToken = token.As<FunctionCallToken>();
            functionCallQueryToken.Kind.Should().Be(QueryTokenKind.FunctionCall);
            functionCallQueryToken.Name.Should().Be(name);
            return new AndConstraint<FunctionCallToken>(functionCallQueryToken);
        }

        public static AndConstraint<FunctionParameterToken> ShouldHaveParameter(this FunctionCallToken token, string name)
        {
            var argument = token.Arguments.SingleOrDefault(arg => arg.ParameterName == name);
            argument.Should().NotBeNull();
            return new AndConstraint<FunctionParameterToken>(argument);
        }

        public static AndConstraint<EndPathToken> ShouldBeEndPathToken(this QueryToken token, string expectedName)
        {
            token.Should().BeOfType<EndPathToken>();
            var propertyAccessQueryToken = token.As<EndPathToken>();
            propertyAccessQueryToken.Kind.Should().Be(QueryTokenKind.EndPath);
            propertyAccessQueryToken.Identifier.Should().Be(expectedName);
            return new AndConstraint<EndPathToken>(propertyAccessQueryToken);
        }

        public static AndConstraint<RangeVariableToken> ShouldBeRangeVariableToken(this QueryToken token, string expectedName)
        {
            token.Should().BeOfType<RangeVariableToken>();
            var parameterQueryToken = token.As<RangeVariableToken>();
            parameterQueryToken.Kind.Should().Be(QueryTokenKind.RangeVariable);
            parameterQueryToken.Name.Should().Be(expectedName);
            return new AndConstraint<RangeVariableToken>(parameterQueryToken);
        }

        public static AndConstraint<BinaryOperatorToken> ShouldBeBinaryOperatorQueryToken(this QueryToken token, BinaryOperatorKind expectedOperatorKind)
        {
            token.Should().BeOfType<BinaryOperatorToken>();
            var propertyAccessQueryToken = token.As<BinaryOperatorToken>();
            propertyAccessQueryToken.Kind.Should().Be(QueryTokenKind.BinaryOperator);
            propertyAccessQueryToken.OperatorKind.Should().Be(expectedOperatorKind);
            return new AndConstraint<BinaryOperatorToken>(propertyAccessQueryToken);
        }

        public static AndConstraint<UnaryOperatorToken> ShouldBeUnaryOperatorQueryToken(this QueryToken token, UnaryOperatorKind expectedOperatorKind)
        {
            token.Should().BeOfType<UnaryOperatorToken>();
            var propertyAccessQueryToken = token.As<UnaryOperatorToken>();
            propertyAccessQueryToken.Kind.Should().Be(QueryTokenKind.UnaryOperator);
            propertyAccessQueryToken.OperatorKind.Should().Be(expectedOperatorKind);
            return new AndConstraint<UnaryOperatorToken>(propertyAccessQueryToken);
        }

        public static AndConstraint<InnerPathToken> ShouldBeInnerPathToken(this QueryToken token, string expectedName)
        {
            token.Should().BeOfType<InnerPathToken>();
            var nonRootToken = token.As<InnerPathToken>();
            nonRootToken.Kind.Should().Be(QueryTokenKind.InnerPath);
            nonRootToken.Identifier.Should().Be(expectedName);
            return new AndConstraint<InnerPathToken>(nonRootToken);
        }

        public static AndConstraint<LiteralToken> ShouldBeLiteralQueryToken(this QueryToken token, object expectedValue)
        {
            token.Should().BeOfType<LiteralToken>();
            var literalToken = token.As<LiteralToken>();
            literalToken.Kind.Should().Be(QueryTokenKind.Literal);
            literalToken.Value.Should().Be(expectedValue);
            return new AndConstraint<LiteralToken>(literalToken);
        }

        public static AndConstraint<StarToken> ShouldBeStarToken(this QueryToken token)
        {
            token.Should().BeOfType<StarToken>();
            var starToken = token.As<StarToken>();
            starToken.Kind.Should().Be(QueryTokenKind.Star);
            return new AndConstraint<StarToken>(starToken);
        }

        public static AndConstraint<DottedIdentifierToken> ShouldBeDottedIdentifierToken(this QueryToken token, string typeName)
        {
            token.Should().BeOfType<DottedIdentifierToken>();
            var dottedIdentifierToken = token.As<DottedIdentifierToken>();
            dottedIdentifierToken.Kind.Should().Be(QueryTokenKind.DottedIdentifier);
            dottedIdentifierToken.Identifier.Should().Be(typeName);
            return new AndConstraint<DottedIdentifierToken>(dottedIdentifierToken);
        }

        public static AndConstraint<ExpandTermToken> ShouldBeExpandTermToken(this QueryToken token, string propertyName, bool checkNullParent)
        {
            token.Should().BeOfType<ExpandTermToken>();
            ExpandTermToken expandTermToken = token.As<ExpandTermToken>();
            expandTermToken.Kind.Should().Be(QueryTokenKind.ExpandTerm);
            expandTermToken.PathToNavProp.Identifier.Should().Be(propertyName);
            if (checkNullParent)
            {
                expandTermToken.PathToNavProp.NextToken.Should().BeNull();
            }
            return new AndConstraint<ExpandTermToken>(expandTermToken);
        }

        public static AndConstraint<SelectToken> ShouldBeSelectToken(this QueryToken token, string[] propertyNames)
        {
            token.Should().BeOfType<SelectToken>();
            SelectToken selectToken = token as SelectToken;
            if (propertyNames.Any())
            {
                selectToken.Properties.Should().HaveSameCount(propertyNames);
                selectToken.Properties.Should().OnlyContain(x => propertyNames.Contains(x.Identifier));

            }
            return new AndConstraint<SelectToken>(selectToken);
        }

        public static AndConstraint<NonSystemToken> ShouldBeNonSystemToken(this PathSegmentToken token, string tokenIdentifier)
        {
            token.Should().BeOfType<NonSystemToken>();
            NonSystemToken nonSystemToken = token.As<NonSystemToken>();
            nonSystemToken.Identifier.Should().Be(tokenIdentifier);
            return new AndConstraint<NonSystemToken>(nonSystemToken);
        }

        public static AndConstraint<SystemToken> ShouldBeSystemToken(this PathSegmentToken token, string tokenIdentifier)
        {
            token.Should().BeOfType<SystemToken>();
            SystemToken systemToken = token.As<SystemToken>();
            systemToken.Identifier.Should().Be(tokenIdentifier);
            return new AndConstraint<SystemToken>(systemToken);
        }

        public static AndConstraint<NamedValue> ShouldBeNamedValue(this NamedValue namedValue, string name, object value)
        {
            namedValue.Name.Should().Be(name);
            namedValue.Value.ShouldBeLiteralQueryToken(value);
            return new AndConstraint<NamedValue>(namedValue);
        }

        public static AndConstraint<StringLiteralToken> ShouldBeStringLiteralToken(this QueryToken token, string text)
        {
            token.Should().BeOfType<StringLiteralToken>();
            StringLiteralToken stringLiteralToken = token.As<StringLiteralToken>();
            stringLiteralToken.Text.Should().Be(text);
            return new AndConstraint<StringLiteralToken>(stringLiteralToken);
        }
    }
}