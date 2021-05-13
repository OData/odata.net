//---------------------------------------------------------------------
// <copyright file="SelectExpandParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Short-span integration tests for the SelectExpandParser, which knows how to use a lexer to 
    /// syntactically parse a V4 $expand or $select query option with nested expand options. 
    /// Unit testing the underlying pieces of parsing here that is be better, but tests at this level are
    /// still quite nice.
    /// TODO: Need more tests for $levels, $ref, and mixing all of the stuff together.
    /// </summary>
    public class SelectExpandParserTests
    {
        #region ParseSelect
        [Fact]
        public void SelectParsesEachTermOnce()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("foo,bar,thing,prop,yoda", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            SelectToken selectToken = parser.ParseSelect();

            // Assert
            Assert.Equal(5, selectToken.Properties.Count());
            Assert.Equal(5, selectToken.SelectTerms.Count());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        public void NullOrEmptyOrWhiteSpaceSelectBecomesEmptyList(string clauseToParse)
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser(clauseToParse, ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            SelectToken selectToken = parser.ParseSelect();

            // Assert
            Assert.NotNull(selectToken.Properties);
            Assert.Empty(selectToken.Properties);
            Assert.NotNull(selectToken.SelectTerms);
            Assert.Empty(selectToken.SelectTerms);
        }

        [Fact]
        public void EmptySelectTermShouldThrow()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("one,,two", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            Action test = () => parser.ParseSelect();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.ExpressionToken_IdentifierExpected("4"));
        }

        [Fact]
        public void MaxRecursionDepthIsEnforcedInSelect()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("stuff/stuff/stuff/stuff", 3);

            // Act
            Action test = () => parser.ParseSelect();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void SemicolonOnlyAllowedInParensInSelect()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("one;two", 3);

            // Act
            Action test = () => parser.ParseSelect();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_InvalidCharacter(";", 3, "one;two"));
        }

        [Fact]
        public void WhitespaceInMiddleOfSegmentShouldThrowInSelect()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("what happens here/foo", 3);

            // Act
            Action test = () => parser.ParseSelect();

            // Assert
            test.Throws<ODataException>(Strings.UriSelectParser_TermIsNotValid("what happens here/foo"));
        }

        #endregion

        #region ParseExpand
        [Fact]
        public void ExpandParsesEachTermOnce()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("foo,bar,thing,prop,yoda", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            ExpandToken expandToken = parser.ParseExpand();

            // Assert
            Assert.Equal(5, expandToken.ExpandTerms.Count());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("      ")]
        public void NullOrEmptyOrWhitespaceExpandBecomesEmptyList(string clauseToParse)
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser(clauseToParse, ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            ExpandToken expandToken = parser.ParseExpand();

            // Assert
            Assert.NotNull(expandToken.ExpandTerms);
            Assert.Empty(expandToken.ExpandTerms);
        }

        [Fact]
        public void EmptyExpandTermShouldThrow()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("one,,two", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            Action test = () => parser.ParseExpand();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.ExpressionToken_IdentifierExpected("4"));
        }

        [Fact]
        public void NestedOptionsOnMultipleExpansionsIsOk()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("one($filter=true), two($filter=false)", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            ExpandToken expandToken = parser.ParseExpand();

            // Assert
            Assert.NotNull(expandToken);
            Assert.NotNull(expandToken.ExpandTerms);
            Assert.Equal(2, expandToken.ExpandTerms.Count());
            Assert.NotNull(expandToken.ExpandTerms.First().FilterOption);
            Assert.NotNull(expandToken.ExpandTerms.Last().FilterOption);
        }

        [Fact]
        public void NestedOptionsWithoutClosingParenthesisShouldThrow()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("one($filter=true", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            Action test = () => parser.ParseExpand();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
        }

        [Fact]
        public void NestedOptionsWithExtraCloseParenShouldThrow()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("one($filter=true)), two", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            Action test = () => parser.ParseExpand();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.UriSelectParser_TermIsNotValid("one($filter=true)), two"));
        }

        [Fact]
        public void OpenCloseParensAfterNavPropShouldThrow()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("NavProp()", ODataUriParserSettings.DefaultSelectExpandLimit);

            // Act
            Action test = () => parser.ParseExpand();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.UriParser_MissingExpandOption("NavProp"));
        }

        [Fact]
        public void MaxRecursionDepthIsEnforcedInExpand()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("stuff/stuff/stuff/stuff", 3);

            // Act
            Action test = () => parser.ParseExpand();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void SemicolonOnlyAllowedInParensInExpand()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("one;two", 3);

            // Act
            Action test = () => parser.ParseExpand();

            // Assert
            test.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_InvalidCharacter(";", 3, "one;two"));
        }

        [Fact]
        public void WhitespaceInMiddleOfSegmentShouldThrowInExpand()
        {
            // Arrange
            SelectExpandParser parser = new SelectExpandParser("what happens here/foo", 3);

            // Act
            Action test = () => parser.ParseExpand();

            // Assert
            test.Throws<ODataException>(Strings.UriSelectParser_TermIsNotValid("what happens here/foo"));
        }

        [Fact]
        public void ParseNestedFilterInSelectWorks()
        {
            // Arrange & Act
            SelectToken selectToken = ParseSelectClause("Address($filter=true)");

            // Assert
            Assert.NotNull(selectToken);
            SelectTermToken selectTermToken = Assert.Single(selectToken.SelectTerms);
            selectTermToken.PathToProperty.ShouldBeNonSystemToken("Address");
            Assert.NotNull(selectTermToken.FilterOption);
            selectTermToken.FilterOption.ShouldBeLiteralQueryToken(true);
        }

        [Fact]
        public void ParseNestedTopAndSkipInSelectWorks()
        {
            // Arrange & Act
            SelectToken selectToken = ParseSelectClause("Address($top=2;$skip=4)");

            // Assert
            Assert.NotNull(selectToken);
            SelectTermToken selectTermToken = Assert.Single(selectToken.SelectTerms);
            selectTermToken.PathToProperty.ShouldBeNonSystemToken("Address");
            Assert.NotNull(selectTermToken.TopOption);
            Assert.Equal(2, selectTermToken.TopOption);

            Assert.NotNull(selectTermToken.SkipOption);
            Assert.Equal(4, selectTermToken.SkipOption);
        }

        [Fact]
        public void ParseNestedOrderByAndSearchInSelectWorks()
        {
            // Arrange & Act
            SelectToken selectToken = ParseSelectClause("Address($orderby=abc;$search=xyz)");

            // Assert
            Assert.NotNull(selectToken);
            SelectTermToken selectTermToken = Assert.Single(selectToken.SelectTerms);
            selectTermToken.PathToProperty.ShouldBeNonSystemToken("Address");
            Assert.NotNull(selectTermToken.OrderByOptions);
            OrderByToken orderBy = Assert.Single(selectTermToken.OrderByOptions);
            orderBy.Expression.ShouldBeEndPathToken("abc");

            Assert.NotNull(selectTermToken.SearchOption);
            selectTermToken.SearchOption.ShouldBeStringLiteralToken("xyz");
        }

        [Theory]
        [InlineData("Emails($orderby=$this)", OrderByDirection.Ascending)]
        [InlineData("Emails($orderby=$this asc)", OrderByDirection.Ascending)]
        [InlineData("Emails($orderby=$this desc)", OrderByDirection.Descending)]
        public void ParseOrderByThisInSelectWorks(string queryString, OrderByDirection orderByDirection)
        {
            // Arrange & Act
            SelectToken selectToken = ParseSelectClause(queryString);

            // Assert
            Assert.NotNull(selectToken);
            SelectTermToken selectTermToken = Assert.Single(selectToken.SelectTerms);
            selectTermToken.PathToProperty.ShouldBeNonSystemToken("Emails");
            Assert.NotNull(selectTermToken.OrderByOptions);
            OrderByToken orderBy = Assert.Single(selectTermToken.OrderByOptions);
            orderBy.Expression.ShouldBeRangeVariableToken("$this");
            Assert.Equal(orderByDirection, orderBy.Direction);
        }

        [Fact]
        public void ParseFilterByThisInSelectWorks()
        {
            // Arrange & Act
            SelectToken selectToken = ParseSelectClause("RelatedSSNs($filter=endswith($this,'xyz'))");

            // Assert
            Assert.NotNull(selectToken);
            SelectTermToken selectTermToken = Assert.Single(selectToken.SelectTerms);
            selectTermToken.PathToProperty.ShouldBeNonSystemToken("RelatedSSNs");
            Assert.NotNull(selectTermToken.FilterOption);

            FunctionCallToken functionCallToken = (FunctionCallToken) selectTermToken.FilterOption;
            functionCallToken.ShouldBeFunctionCallToken("endswith");
            Assert.Equal(2, functionCallToken.Arguments.Count());

            FunctionParameterToken parameterToken = functionCallToken.Arguments.First();
            parameterToken.ValueToken.ShouldBeRangeVariableToken(ExpressionConstants.This);
        }

        [Fact]
        public void ParseNestedSelectInSelectWorks()
        {
            // Arrange & Act
            SelectToken selectToken = ParseSelectClause("Address($select=abc,xyz)");

            // Assert
            Assert.NotNull(selectToken);
            SelectTermToken selectTermToken = Assert.Single(selectToken.SelectTerms);
            selectTermToken.PathToProperty.ShouldBeNonSystemToken("Address");

            Assert.NotNull(selectTermToken.SelectOption);
            Assert.Equal(2, selectTermToken.SelectOption.SelectTerms.Count());
            SelectTermToken nestedSelectTermToken = selectTermToken.SelectOption.SelectTerms.First();
            nestedSelectTermToken.PathToProperty.ShouldBeNonSystemToken("abc");

            nestedSelectTermToken = selectTermToken.SelectOption.SelectTerms.Last();
            nestedSelectTermToken.PathToProperty.ShouldBeNonSystemToken("xyz");
        }

        [Fact]
        public void ParseDeepNestedSelectInSelectWorks()
        {
            // Arrange & Act
            SelectToken selectToken = ParseSelectClause("Address($select=one($select=two)),City($select=three($select=four))");

            // Assert
            Assert.NotNull(selectToken);
            SelectTermToken[] selectTermTokens = selectToken.SelectTerms.ToArray();
            Assert.Equal(2, selectTermTokens.Length);

            // #1 Depth 0
            selectTermTokens[0].PathToProperty.ShouldBeNonSystemToken("Address");

            // #1 Depth 1
            Assert.NotNull(selectTermTokens[0].SelectOption);
            SelectTermToken nestedSelectToken = Assert.Single(selectTermTokens[0].SelectOption.SelectTerms);
            nestedSelectToken.PathToProperty.ShouldBeNonSystemToken("one");

            // #1 Depth 2
            Assert.NotNull(nestedSelectToken.SelectOption);
            nestedSelectToken = Assert.Single(nestedSelectToken.SelectOption.SelectTerms);
            nestedSelectToken.PathToProperty.ShouldBeNonSystemToken("two");

            // #2 Depth 0
            selectTermTokens[1].PathToProperty.ShouldBeNonSystemToken("City");

            // #2 Depth 1
            Assert.NotNull(selectTermTokens[1].SelectOption);
            nestedSelectToken = Assert.Single(selectTermTokens[1].SelectOption.SelectTerms);
            nestedSelectToken.PathToProperty.ShouldBeNonSystemToken("three");

            // #2 Depth 2
            Assert.NotNull(nestedSelectToken.SelectOption);
            nestedSelectToken = Assert.Single(nestedSelectToken.SelectOption.SelectTerms);
            nestedSelectToken.PathToProperty.ShouldBeNonSystemToken("four");
        }

        private SelectToken ParseSelectClause(string select)
        {
            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(EdmCoreModel.Instance)
            {
                Settings = { PathLimit = 10, FilterLimit = 20, OrderByLimit = 10, SearchLimit = 10, SelectExpandLimit = 10 }
            };

            SelectExpandParser expandParser = new SelectExpandParser(select, configuration.Settings.SelectExpandLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier)
            {
                MaxPathDepth = configuration.Settings.PathLimit,
                MaxFilterDepth = configuration.Settings.FilterLimit,
                MaxOrderByDepth = configuration.Settings.OrderByLimit,
                MaxSearchDepth = configuration.Settings.SearchLimit
            };

            return expandParser.ParseSelect();
        }
        #endregion

        #region ParseSelectExpand
        [Fact]
        public void NoDollarNestedOptions()
        {
            // Act
            SelectExpandParserTests.ParseSelectExpand(
                select: "prop1,prop2(select=prop3,prop4)",
                expand: "nav1,nav2(select=prop5,prop6;expand=nav3,nav4)",
                out SelectToken selectToken,
                out ExpandToken expandToken,
                enableNoDollarQueryOptions: true);

            // Assert Select
            Assert.NotNull(selectToken.Properties);
            PathSegmentToken[] properties = selectToken.Properties.ToArray();
            Assert.Equal(2, properties.Length);
            properties[0].ShouldBeNonSystemToken("prop1");
            properties[1].ShouldBeNonSystemToken("prop2");

            Assert.NotNull(selectToken.SelectTerms);
            SelectTermToken[] selectTerms = selectToken.SelectTerms.ToArray();
            Assert.Equal(2, selectTerms.Length);
            selectTerms[0].PathToProperty.ShouldBeNonSystemToken("prop1");
            Assert.Null(selectTerms[0].SelectOption);
            selectTerms[1].PathToProperty.ShouldBeNonSystemToken("prop2");
            Assert.NotNull(selectTerms[1].SelectOption);
            Assert.NotNull(selectTerms[1].SelectOption.SelectTerms);
            SelectTermToken[] subSelectTerms = selectTerms[1].SelectOption.SelectTerms.ToArray();
            Assert.Equal(2, subSelectTerms.Length);
            subSelectTerms[0].PathToProperty.ShouldBeNonSystemToken("prop3");
            subSelectTerms[1].PathToProperty.ShouldBeNonSystemToken("prop4");

            // Assert Expand
            Assert.NotNull(expandToken.ExpandTerms);
            ExpandTermToken[] expandTerms = expandToken.ExpandTerms.ToArray();
            Assert.Equal(2, expandTerms.Length);
            expandTerms[0].PathToProperty.ShouldBeNonSystemToken("nav1");
            Assert.Null(expandTerms[0].ExpandOption);
            Assert.Null(expandTerms[0].SelectOption);
            expandTerms[1].PathToProperty.ShouldBeNonSystemToken("nav2");
            Assert.NotNull(expandTerms[1].ExpandOption);
            ExpandTermToken[] subExpandTerms = expandTerms[1].ExpandOption.ExpandTerms.ToArray();
            Assert.Equal(2, subExpandTerms.Length);
            subExpandTerms[0].PathToProperty.ShouldBeNonSystemToken("nav3");
            Assert.Null(subExpandTerms[0].ExpandOption);
            Assert.Null(subExpandTerms[0].SelectOption);
            subExpandTerms[1].PathToProperty.ShouldBeNonSystemToken("nav4");
            Assert.Null(subExpandTerms[1].ExpandOption);
            Assert.Null(subExpandTerms[1].SelectOption);
            Assert.NotNull(expandTerms[1].SelectOption);
            subSelectTerms = expandTerms[1].SelectOption.SelectTerms.ToArray();
            Assert.Equal(2, subSelectTerms.Length);
            subSelectTerms[0].PathToProperty.ShouldBeNonSystemToken("prop5");
            Assert.Null(subSelectTerms[0].SelectOption);
            subSelectTerms[1].PathToProperty.ShouldBeNonSystemToken("prop6");
            Assert.Null(subSelectTerms[1].SelectOption);
        }

        private static void ParseSelectExpand(
            string select,
            string expand,
            out SelectToken selectToken,
            out ExpandToken expandToken,
            int selectExpandLimit = ODataUriParserSettings.DefaultSelectExpandLimit,
            bool enableCaseInsensitiveUriFunctionIdentifier = false,
            bool enableNoDollarQueryOptions = false)
        {
            SelectExpandSyntacticParser.Parse(
                select,
                expand,
                parentStructuredType: null,
                new ODataUriParserConfiguration(EdmCoreModel.Instance)
                {
                    EnableCaseInsensitiveUriFunctionIdentifier = enableCaseInsensitiveUriFunctionIdentifier,
                    EnableNoDollarQueryOptions = enableNoDollarQueryOptions,
                    Settings = { SelectExpandLimit = selectExpandLimit },
                },
                out expandToken,
                out selectToken);
        }
        #endregion

        #region Star Expand Testing
        [Fact]
        public void ParseStarInExpand()
        {
            // Arrange & Act
            ExpandToken expandToken = this.StarExpandTesting("*", "Person");

            // Assert
            ExpandTermToken innerExpandTerm = Assert.Single(expandToken.ExpandTerms);
            innerExpandTerm.PathToNavigationProp.ShouldBeNonSystemToken("Friend");
            Assert.Null(innerExpandTerm.PathToNavigationProp.NextToken);
        }

        [Fact]
        public void ParseStarInExpandWithComplexTypeAsParent()
        {
            // Arrange
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            EdmEntityType cityType = model.SchemaElements.OfType<EdmEntityType>().First(c => c.Name == "CityType");
            EdmComplexType addressType = model.SchemaElements.OfType<EdmComplexType>().First(c => c.Name == "Address");

            // Add two navigation properties to the "Address" complex type.
            addressType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "CityNav1",
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                Target = cityType
            });

            addressType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "CityNav2",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = cityType
            });

            // Act
            ExpandToken expandToken = this.StarExpandTesting("*", "Address", model);

            // Assert
            Assert.Equal(2, expandToken.ExpandTerms.Count());

            ExpandTermToken innerExpandTerm1 = expandToken.ExpandTerms.First();
            innerExpandTerm1.PathToNavigationProp.ShouldBeNonSystemToken("CityNav1");

            ExpandTermToken innerExpandTerm2 = expandToken.ExpandTerms.Last();
            innerExpandTerm2.PathToNavigationProp.ShouldBeNonSystemToken("CityNav2");
        }

        [Fact]
        public void ParseStarWithRefInExpand()
        {
            // Arrange & Act
            ExpandToken expandToken = this.StarExpandTesting("*/$ref", "Person");

            // Assert
            ExpandTermToken innerExpandTerm = Assert.Single(expandToken.ExpandTerms);
            innerExpandTerm.PathToNavigationProp.ShouldBeNonSystemToken("$ref");
            Assert.NotNull(innerExpandTerm.PathToNavigationProp.NextToken);
            innerExpandTerm.PathToNavigationProp.NextToken.ShouldBeNonSystemToken("Friend");
        }

        [Fact]
        public void ParseStarWithEntitySetInExpand()
        {
            // Arrange & Act
            ExpandToken expandToken = this.StarExpandTesting("*/$ref,CityHall", "CityType");

            // Assert
            Assert.NotNull(expandToken);
            Assert.NotNull(expandToken.ExpandTerms);

            ExpandTermToken[] expandTermTokens = expandToken.ExpandTerms.ToArray();
            Assert.Equal(3, expandTermTokens.Length);

            // #1
            expandTermTokens[0].PathToNavigationProp.ShouldBeNonSystemToken("CityHall");
            Assert.Null(expandTermTokens[0].PathToNavigationProp.NextToken);

            // #2,  DOL/$ref
            expandTermTokens[1].PathToNavigationProp.ShouldBeNonSystemToken("$ref");
            Assert.NotNull(expandTermTokens[1].PathToNavigationProp.NextToken);
            expandTermTokens[1].PathToNavigationProp.NextToken.ShouldBeNonSystemToken("DOL");
            Assert.Null(expandTermTokens[1].PathToNavigationProp.NextToken.NextToken);

            // #3  PoliceStation/$ref
            expandTermTokens[2].PathToNavigationProp.ShouldBeNonSystemToken("$ref");
            Assert.NotNull(expandTermTokens[2].PathToNavigationProp.NextToken);
            expandTermTokens[2].PathToNavigationProp.NextToken.ShouldBeNonSystemToken("PoliceStation");
            Assert.Null(expandTermTokens[2].PathToNavigationProp.NextToken.NextToken);
        }

        [Fact]
        public void ParseStarWithMultipleEntitySetInExpand()
        {
            // Arrange & Act
            ExpandToken expandToken = this.StarExpandTesting("CityHall($levels=2),*/$ref,PoliceStation($select=Id, Address)", "CityType");

            // Assert
            Assert.NotNull(expandToken);
            Assert.NotNull(expandToken.ExpandTerms);

            ExpandTermToken[] expandTermTokens = expandToken.ExpandTerms.ToArray();
            Assert.Equal(3, expandTermTokens.Length);

            // #1
            expandTermTokens[0].PathToNavigationProp.ShouldBeNonSystemToken("CityHall");
            Assert.Null(expandTermTokens[0].PathToNavigationProp.NextToken);
            Assert.NotNull(expandTermTokens[0].LevelsOption);
            Assert.Equal(2, expandTermTokens[0].LevelsOption);

            // #2
            expandTermTokens[1].PathToNavigationProp.ShouldBeNonSystemToken("PoliceStation");
            Assert.Null(expandTermTokens[1].PathToNavigationProp.NextToken);
            Assert.NotNull(expandTermTokens[1].SelectOption);
            Assert.Equal(2, expandTermTokens[1].SelectOption.SelectTerms.Count());
            expandTermTokens[1].SelectOption.SelectTerms.First().PathToProperty.ShouldBeNonSystemToken("Id");
            expandTermTokens[1].SelectOption.SelectTerms.Last().PathToProperty.ShouldBeNonSystemToken("Address");

            // #3
            expandTermTokens[2].PathToNavigationProp.ShouldBeNonSystemToken("$ref");
            Assert.NotNull(expandTermTokens[2].PathToNavigationProp.NextToken);
            expandTermTokens[2].PathToNavigationProp.NextToken.ShouldBeNonSystemToken("DOL");
            Assert.Null(expandTermTokens[2].PathToNavigationProp.NextToken.NextToken);
        }

        [Fact]
        public void NestedStarExpand()
        {
            // Arrange & Act
            ExpandToken expandToken = this.StarExpandTesting("Friend($expand=*)", "Person");

            // Assert
            Assert.NotNull(expandToken);
            ExpandTermToken expandTermToken = Assert.Single(expandToken.ExpandTerms);

            // Depth 0
            expandTermToken.PathToNavigationProp.ShouldBeNonSystemToken("Friend");
            Assert.Null(expandTermToken.PathToNavigationProp.NextToken);

            // Depth 1
            Assert.NotNull(expandTermToken.ExpandOption);
            ExpandTermToken nestedExpandTermToken = Assert.Single(expandTermToken.ExpandOption.ExpandTerms);
            nestedExpandTermToken.PathToNavigationProp.ShouldBeNonSystemToken("Friend");
            Assert.Null(nestedExpandTermToken.PathToNavigationProp.NextToken);
        }

        [Fact]
        public void ParseStarWithOptions()
        {
            // Arrange & Act
            ExpandToken expandToken = this.StarExpandTesting("*($levels=2)", "Person");

            // Assert
            Assert.NotNull(expandToken);
            ExpandTermToken expandTermToken = Assert.Single(expandToken.ExpandTerms);

            expandTermToken.PathToNavigationProp.ShouldBeNonSystemToken("Friend");
            Assert.Null(expandTermToken.PathToNavigationProp.NextToken);
            Assert.NotNull(expandTermToken.LevelsOption);
            Assert.Equal(2, expandTermToken.LevelsOption);
        }

        [Fact]
        public void ParseStarInvalidWithOptions()
        {
            // Arrange & Act
            Action test = () => this.StarExpandTesting("*($select=*)", "Person");

            // Assert
            test.Throws<ODataException>(Strings.UriExpandParser_TermIsNotValidForStar("($select=*)"));
        }

        [Fact]
        public void ParseStarRefWithInvalidOptions()
        {
            // Arrange & Act
            Action test = () => this.StarExpandTesting("*/$ref($levels=2)", "Person");

            // Assert
            test.Throws<ODataException>(Strings.UriExpandParser_TermIsNotValidForStarRef("($levels=2)"));
        }

        [Fact]
        public void ParseMultipleStarInExpand()
        {
            // Arrange & Act
            Action test = () => this.StarExpandTesting("*, Friend, */$ref", "Person");

            // Assert
            test.Throws<ODataException>(Strings.UriExpandParser_TermWithMultipleStarNotAllowed("*, Friend, */$ref"));
        }

        private ExpandToken StarExpandTesting(string expand, string typeName, IEdmModel model = null)
        {
            if (model == null)
            {
                model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            }

            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(EdmCoreModel.Instance)
            {
                Settings = { PathLimit = 10, FilterLimit = 10, OrderByLimit = 10, SearchLimit = 10, SelectExpandLimit = 10 }
            };

            var parentStructuredType = configuration.Resolver.ResolveType(model, "TestModel." + typeName) as IEdmStructuredType;
            SelectExpandParser expandParser = new SelectExpandParser(configuration.Resolver, expand, parentStructuredType, configuration.Settings.SelectExpandLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier)
            {
                MaxPathDepth = configuration.Settings.PathLimit,
                MaxFilterDepth = configuration.Settings.FilterLimit,
                MaxOrderByDepth = configuration.Settings.OrderByLimit,
                MaxSearchDepth = configuration.Settings.SearchLimit
            };

            return expandParser.ParseExpand();
        }

        [Fact]
        public void ParentEntityTypeIsNullForExpandStar()
        {
            // Arrange
            string expand = "*";

            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(EdmCoreModel.Instance)
            {
                Settings = { PathLimit = 10, FilterLimit = 10, OrderByLimit = 10, SearchLimit = 10, SelectExpandLimit = 10 }
            };

            SelectExpandParser expandParser = new SelectExpandParser(configuration.Resolver, expand, null, configuration.Settings.SelectExpandLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier)
            {
                MaxPathDepth = configuration.Settings.PathLimit,
                MaxFilterDepth = configuration.Settings.FilterLimit,
                MaxOrderByDepth = configuration.Settings.OrderByLimit,
                MaxSearchDepth = configuration.Settings.SearchLimit
            };

            Action test = () => expandParser.ParseExpand();

            // Assert
            test.Throws<ODataException>(Strings.UriExpandParser_ParentStructuredTypeIsNull(""));
        }
        #endregion

        #region Limit Test
        [Fact]
        public void PathLimitTest()
        {
            // Arrange & Act & Assert
            this.LimitationTest("d1/d2", null);
            Action test = () => this.LimitationTest("d1/d2/d3", null);
            test.Throws<ODataException>(Strings.UriQueryExpressionParser_TooDeep);

            this.LimitationTest(null, "d1/d2");
            test = () => this.LimitationTest(null,"d1/d2/d3");
            test.Throws<ODataException>(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void FilterLimitTest()
        {
            // Arrange & Act & Assert
            this.LimitationTest(null, "d1($filter=a or b and c)");
            Action test = () => this.LimitationTest(null, "d1($filter=(a or b) and c)");
            test.Throws<ODataException>(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void OrderByLimitTest()
        {
            // Arrange & Act & Assert
            this.LimitationTest(null, "d1($orderby=a or b and c)");
            Action test = () => this.LimitationTest(null, "d1($orderby=(a or b) and c)");
            test.Throws<ODataException>(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void SearchLimitTest()
        {
            // Arrange & Act & Assert
            this.LimitationTest(null, "d1($search=a OR b AND c)");
            Action test = () => this.LimitationTest(null, "d1($search=(a OR b) AND c)");
            test.Throws<ODataException>(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void ExpandLimitTest()
        {
            // Arrange & Act & Assert
            this.LimitationTest(null, "d1($expand=d2($expand=d3($expand=d4($expand=d5))))");
            Action test = () => this.LimitationTest(null, "d1($expand=d2($expand=d3($expand=d4($expand=d5($expand=d6)))))");
            test.Throws<ODataException>(Strings.UriQueryExpressionParser_TooDeep);
        }

        private void LimitationTest(string select, string expand)
        {
            SelectToken selectTree;
            ExpandToken expandTree;
            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(EdmCoreModel.Instance)
            {
                Settings = { PathLimit = 2, FilterLimit = 8, OrderByLimit = 8, SearchLimit = 7, SelectExpandLimit = 5 }
            };

            SelectExpandSyntacticParser.Parse(select, expand, null , configuration, out expandTree, out selectTree);
        }
        #endregion
    }
}
