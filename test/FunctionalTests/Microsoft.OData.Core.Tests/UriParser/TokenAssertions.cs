//---------------------------------------------------------------------
// <copyright file="TokenAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Contains fluent assertion APIs for testing QueryTokens.
    /// TODO: Consider using T : QueryToken instead, and writing a test to assert that the QueryTokenKind matches the relevent class seperately.
    /// </summary>
    internal static class TokenAssertions
    {
        public static AnyToken ShouldBeAnyToken(this QueryToken token, string expectedParameterName)
        {
            Assert.NotNull(token);
            AnyToken anyToken = Assert.IsType<AnyToken>(token);
            Assert.Equal(QueryTokenKind.Any, anyToken.Kind);
            Assert.Equal(expectedParameterName, anyToken.Parameter);
            return anyToken;
        }

        public static AllToken ShouldBeAllToken(this QueryToken token, string expectedParameterName)
        {
            Assert.NotNull(token);
            AllToken allToken = Assert.IsType<AllToken>(token);
            Assert.Equal(QueryTokenKind.Any, allToken.Kind);
            Assert.Equal(expectedParameterName, allToken.Parameter);
            return allToken;
        }

        public static FunctionCallToken ShouldBeFunctionCallToken(this QueryToken token, string name)
        {
            Assert.NotNull(token);
            FunctionCallToken functionCallQueryToken = Assert.IsType<FunctionCallToken>(token);
            Assert.Equal(QueryTokenKind.FunctionCall, functionCallQueryToken.Kind);
            Assert.Equal(name, functionCallQueryToken.Name);
            return functionCallQueryToken;
        }

        public static FunctionParameterToken ShouldHaveParameter(this FunctionCallToken token, string name)
        {
            Assert.NotNull(token);
            FunctionParameterToken argument = token.Arguments.SingleOrDefault(arg => arg.ParameterName == name);
            Assert.NotNull(argument);
            return argument;
        }

        public static EndPathToken ShouldBeEndPathToken(this QueryToken token, string expectedName)
        {
            Assert.NotNull(token);
            EndPathToken propertyAccessQueryToken = Assert.IsType<EndPathToken>(token);
            Assert.Equal(QueryTokenKind.EndPath, propertyAccessQueryToken.Kind);
            Assert.Equal(expectedName, propertyAccessQueryToken.Identifier);
            return propertyAccessQueryToken;
        }

        public static RangeVariableToken ShouldBeRangeVariableToken(this QueryToken token, string expectedName)
        {
            Assert.NotNull(token);
            RangeVariableToken parameterQueryToken = Assert.IsType<RangeVariableToken>(token);
            Assert.Equal(QueryTokenKind.RangeVariable, parameterQueryToken.Kind);
            Assert.Equal(expectedName, parameterQueryToken.Name);
            return parameterQueryToken;
        }

        public static BinaryOperatorToken ShouldBeBinaryOperatorQueryToken(this QueryToken token, BinaryOperatorKind expectedOperatorKind)
        {
            Assert.NotNull(token);
            BinaryOperatorToken propertyAccessQueryToken = Assert.IsType<BinaryOperatorToken>(token);
            Assert.Equal(QueryTokenKind.BinaryOperator, propertyAccessQueryToken.Kind);
            Assert.Equal(expectedOperatorKind, propertyAccessQueryToken.OperatorKind);
            return propertyAccessQueryToken;
        }

        public static UnaryOperatorToken ShouldBeUnaryOperatorQueryToken(this QueryToken token, UnaryOperatorKind expectedOperatorKind)
        {
            Assert.NotNull(token);
            UnaryOperatorToken propertyAccessQueryToken = Assert.IsType<UnaryOperatorToken>(token);
            Assert.Equal(QueryTokenKind.UnaryOperator, propertyAccessQueryToken.Kind);
            Assert.Equal(expectedOperatorKind, propertyAccessQueryToken.OperatorKind);
            return propertyAccessQueryToken;
        }

        public static InnerPathToken ShouldBeInnerPathToken(this QueryToken token, string expectedName)
        {
            Assert.NotNull(token);
            InnerPathToken nonRootToken = Assert.IsType<InnerPathToken>(token);
            Assert.Equal(QueryTokenKind.InnerPath, nonRootToken.Kind);
            Assert.Equal(expectedName, nonRootToken.Identifier);
            return nonRootToken;
        }

        public static LiteralToken ShouldBeLiteralQueryToken(this QueryToken token, object expectedValue)
        {
            Assert.NotNull(token);
            LiteralToken literalToken = Assert.IsType<LiteralToken>(token);
            Assert.Equal(QueryTokenKind.Literal, literalToken.Kind);
            Assert.Equal(expectedValue, literalToken.Value);
            return literalToken;
        }

        public static StarToken ShouldBeStarToken(this QueryToken token)
        {
            Assert.NotNull(token);
            StarToken starToken = Assert.IsType<StarToken>(token);
            Assert.Equal(QueryTokenKind.Star, starToken.Kind);
            return starToken;
        }

        public static DottedIdentifierToken ShouldBeDottedIdentifierToken(this QueryToken token, string typeName)
        {
            Assert.NotNull(token);
            DottedIdentifierToken dottedIdentifierToken = Assert.IsType<DottedIdentifierToken>(token);
            Assert.Equal(QueryTokenKind.DottedIdentifier, dottedIdentifierToken.Kind);
            Assert.Equal(typeName, dottedIdentifierToken.Identifier);
            return dottedIdentifierToken;
        }

        public static ExpandTermToken ShouldBeExpandTermToken(this QueryToken token, string propertyName, bool checkNullParent)
        {
            Assert.NotNull(token);
            ExpandTermToken expandTermToken = Assert.IsType<ExpandTermToken>(token);
            Assert.Equal(QueryTokenKind.ExpandTerm, expandTermToken.Kind);
            Assert.Equal(propertyName, expandTermToken.PathToNavigationProp.Identifier);

            if (checkNullParent)
            {
                Assert.Null(expandTermToken.PathToNavigationProp.NextToken);
            }

            return expandTermToken;
        }

        public static SelectToken ShouldBeSelectToken(this QueryToken token, string[] propertyNames)
        {
            Assert.NotNull(token);
            SelectToken selectToken = Assert.IsType<SelectToken>(token);
            if (propertyNames.Any())
            {
                Assert.Equal(propertyNames.Length, selectToken.Properties.Count());
                Assert.Equal(propertyNames, selectToken.Properties.Select(p => p.Identifier));
            }

            return selectToken;
        }

        public static NonSystemToken ShouldBeNonSystemToken(this PathSegmentToken token, string tokenIdentifier)
        {
            Assert.NotNull(token);
            NonSystemToken nonSystemToken = Assert.IsType<NonSystemToken>(token);
            Assert.Equal(tokenIdentifier, nonSystemToken.Identifier);
            return nonSystemToken;
        }

        public static SystemToken ShouldBeSystemToken(this PathSegmentToken token, string tokenIdentifier)
        {
            Assert.NotNull(token);
            SystemToken systemToken = Assert.IsType<SystemToken>(token);
            Assert.Equal(tokenIdentifier, systemToken.Identifier);
            return systemToken;
        }

        public static NamedValue ShouldBeNamedValue(this NamedValue namedValue, string name, object value)
        {
            Assert.NotNull(namedValue);
            Assert.Equal(name, namedValue.Name);
            namedValue.Value.ShouldBeLiteralQueryToken(value);
            return namedValue;
        }

        public static StringLiteralToken ShouldBeStringLiteralToken(this QueryToken token, string text)
        {
            Assert.NotNull(token);
            StringLiteralToken stringLiteralToken = Assert.IsType<StringLiteralToken>(token);
            Assert.Equal(text, stringLiteralToken.Text);
            return stringLiteralToken;
        }
    }
}