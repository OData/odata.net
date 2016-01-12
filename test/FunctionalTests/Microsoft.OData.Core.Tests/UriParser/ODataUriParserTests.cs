//---------------------------------------------------------------------
// <copyright file="ODataUriParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser
{
    /// <summary>
    /// Unit tests for ODataUriParser.
    /// </summary>
    public class ODataUriParserTests
    {
        private readonly Uri ServiceRoot = new Uri("http://host");
        private readonly Uri FullUri = new Uri("http://host/People");

        [Fact]
        public void NoneQueryOptionShouldWork()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri);
            var path = uriParser.ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            uriParser.ParseFilter().Should().BeNull();
            uriParser.ParseSelectAndExpand().Should().BeNull();
            uriParser.ParseOrderBy().Should().BeNull();
            uriParser.ParseTop().Should().Be(null);
            uriParser.ParseSkip().Should().Be(null);
            uriParser.ParseCount().Should().Be(null);
            uriParser.ParseSearch().Should().BeNull();
            uriParser.ParseSkipToken().Should().BeNull();
            uriParser.ParseDeltaToken().Should().BeNull();
        }

        [Fact]
        public void EmptyValueQueryOptionShouldWork()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(FullUri, "?$filter=&$select=&$expand=&$orderby=&$top=&$skip=&$count=&$search=&$unknow=&$unknowvalue&$skiptoken=&$deltatoken="));
            var path = uriParser.ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            uriParser.ParseFilter().Should().BeNull();
            var results = uriParser.ParseSelectAndExpand();
            results.AllSelected.Should().BeTrue();
            results.SelectedItems.Should().HaveCount(0);
            uriParser.ParseOrderBy().Should().BeNull();
            Action action = () => uriParser.ParseTop();
            action.ShouldThrow<ODataException>().WithMessage(Strings.SyntacticTree_InvalidTopQueryOptionValue(""));
            action = () => uriParser.ParseSkip();
            action.ShouldThrow<ODataException>().WithMessage(Strings.SyntacticTree_InvalidSkipQueryOptionValue(""));
            action = () => uriParser.ParseCount();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataUriParser_InvalidCount(""));
            action = () => uriParser.ParseSearch();
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(0, ""));
            uriParser.ParseSkipToken().Should().BeEmpty();
            uriParser.ParseDeltaToken().Should().BeEmpty();
        }

        #region Setter/getter and validation tests
        [Fact]
        public void ModelCannotBeNull()
        {
            Action createWithNullModel = () => new ODataUriParser(null, ServiceRoot, FullUri);
            createWithNullModel.ShouldThrow<Exception>(Error.ArgumentNull("model").ToString());
        }

        [Fact]
        public void ModelIsSetCorrectly()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri);
            parser.Model.Should().Be(HardCodedTestModel.TestModel);
        }

        [Fact]
        public void ServiceRootUriIsSet()
        {
            var serviceRoot = new Uri("http://example.com/Foo/");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, FullUri);
            parser.ServiceRoot.Should().BeSameAs(serviceRoot);
        }

        [Fact]
        public void ServiceRootMustBeAbsoluteUri()
        {
            var serviceRoot = new Uri("one/two/three", UriKind.Relative);
            Action create = () => new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, new Uri("test", UriKind.Relative));
            create.ShouldThrow<ODataException>();

            serviceRoot = new Uri("one/two/three", UriKind.RelativeOrAbsolute);
            create = () => new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, new Uri("test", UriKind.Relative));
            create.ShouldThrow<ODataException>();
        }

        [Fact]
        public void MaxExpandDepthCannotBeNegative()
        {
            Action setNegative = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Settings.MaximumExpansionDepth = -1;
            setNegative.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void MaxExpandCountCannotBeNegative()
        {
            Action setNegative = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Settings.MaximumExpansionCount = -1;
            setNegative.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }
        #endregion

        #region Parser limit config tests
        [Fact]
        public void FilterLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { FilterLimit = 3 } };
            parser.Settings.FilterLimit.Should().Be(3);
        }

        [Fact]
        public void FilterLimitIsRespectedForFilter()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$filter=1 eq 1")) { Settings = { FilterLimit = 0 } };
            Action parseWithLimit = () => parser.ParseFilter();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void FilterLimitWithInterestingTreeStructures()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$filter=MyDog/Color eq 'Brown' or MyDog/Color eq 'White'")) { Settings = { FilterLimit = 5 } };
            Action parseWithLimit = () => parser.ParseFilter();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void NegativeFilterLimitThrows()
        {
            Action negativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { FilterLimit = -98798 } };
            negativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void OrderbyLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { OrderByLimit = 3 } };
            parser.Settings.OrderByLimit.Should().Be(3);
        }

        [Fact]
        public void OrderByLimitIsRespectedForOrderby()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$orderby= 1 eq 1")) { Settings = { OrderByLimit = 0 } };
            Action parseWithLimit = () => parser.ParseOrderBy();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void OrderByLimitWithInterestingTreeStructures()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$orderby=MyDog/MyPeople/MyDog/MyPeople/MyPaintings asc")) { Settings = { OrderByLimit = 5 } };
            Action parseWithLimit = () => parser.ParseOrderBy();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void OrderByLimitCannotBeNegative()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { OrderByLimit = -9879 } };
            parseWithNegativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void PathLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { PathLimit = 3 } };
            parser.Settings.PathLimit.Should().Be(3);
        }

        [Fact]
        public void PathLimitIsRespectedForPath()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/", UriKind.Absolute), new Uri("http://gobbldygook/path/to/something", UriKind.Absolute)) { Settings = { PathLimit = 0 } };
            Action parseWithLimit = () => parser.ParsePath();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryPathParser_TooManySegments);
        }

        [Fact]
        public void PathLimitCannotBeNegative()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { PathLimit = -8768 } };
            parseWithNegativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void SelectExpandLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { SelectExpandLimit = 3 } };
            parser.Settings.SelectExpandLimit.Should().Be(3);
        }

        [Fact]
        public void SelectExpandLimitIsRespectedForSelectExpand()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$select=MyDog&$expand=MyDog($select=color)")) { Settings = { SelectExpandLimit = 0 } };
            Action parseWithLimit = () => parser.ParseSelectAndExpand();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void NegativeSelectExpandLimitIsRespected()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { SelectExpandLimit = -87657 } };
            parseWithNegativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }
        #endregion

        #region Default value tests
        [Fact]
        public void DefaultUrlConventionsShouldBeDefault()
        {
            new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlConventions.Should().BeSameAs(ODataUrlConventions.Default);
        }

        [Fact]
        public void UrlConventionsCannotBeSetToNull()
        {
            Action setToNull = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlConventions = null;
            setToNull.ShouldThrow<ArgumentNullException>().WithMessage("UrlConventions", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void DefaultEnableTemplateParsingShouldBeFalse()
        {
            new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).EnableUriTemplateParsing.Should().BeFalse();
        }

        [Fact]
        public void DefaultEnableCaseInsensitiveBuiltinIdentifierShouldBeFalse()
        {
            new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Resolver.EnableCaseInsensitive.Should().BeFalse();
        }

        [Fact]
        public void DefaultParameterAliasNodesShouldBeEmtpy()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People"));
            uriParser.ParameterAliasNodes.Count.Should().Be(0);
        }
        #endregion

        [Fact]
        public void ParseSelectExpandForContainment()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$select=MyContainedDog&$expand=MyContainedDog")) { Settings = { SelectExpandLimit = 5 } };
            SelectExpandClause containedSelectExpandClause = parser.ParseSelectAndExpand();
            IEnumerator<SelectItem> enumerator = containedSelectExpandClause.SelectedItems.GetEnumerator();
            enumerator.MoveNext();
            ExpandedNavigationSelectItem expandedNavigationSelectItem = enumerator.Current as ExpandedNavigationSelectItem;
            expandedNavigationSelectItem.Should().NotBeNull();
            (expandedNavigationSelectItem.NavigationSource is IEdmContainedEntitySet).Should().BeTrue();
        }

        #region Relative full path smoke test
        [Fact]
        public void AbsoluteUriInConstructorShouldThrow()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host/People(1)"));
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriParser_FullUriMustBeRelative);
        }

        [Fact]
        public void AlternateKeyShouldWork()
        {
            ODataPath pathSegment = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(SocialSN = \'1\')"))
            {
                Resolver = new AlternateKeysODataUriResolver(HardCodedTestModel.TestModel)
            }.ParsePath();

            pathSegment.Should().HaveCount(2);
            pathSegment.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.TestModel.FindDeclaredEntitySet("People"));
            pathSegment.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("SocialSN", "1"));
        }

        [Fact]
        public void CompositeAlternateKeyShouldWork()
        {
            Uri fullUri = new Uri("http://host/People(NameAlias=\'anyName\',FirstNameAlias=\'anyFirst\')");
            ODataPath pathSegment = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), fullUri)
            {
                Resolver = new AlternateKeysODataUriResolver(HardCodedTestModel.TestModel)
            }.ParsePath();

            pathSegment.Should().HaveCount(2);
            pathSegment.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.TestModel.FindDeclaredEntitySet("People"));
            pathSegment.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("NameAlias", "anyName"), new KeyValuePair<string, object>("FirstNameAlias", "anyFirst"));
        }

        [Fact]
        public void CompositeAlternateKeyShouldFailOnlyWithPartialAlternateKey()
        {
            Uri fullUri = new Uri("http://host/People(NameAlias=\'anyName\')");
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), fullUri)
            {
                Resolver = new AlternateKeysODataUriResolver(HardCodedTestModel.TestModel)
            }.ParsePath();

            action.ShouldThrow<ODataException>().WithMessage("Bad Request - Error in query syntax.");
        }

        [Fact]
        public void AlternateKeyShouldFailWithDefaultUriResolver()
        {
            Uri fullUri = new Uri("http://host/People(SocialSN = \'1\')");
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), fullUri)
            {
                Resolver = new ODataUriResolver()
            }.ParsePath();

            action.ShouldThrow<ODataException>().WithMessage("Bad Request - Error in query syntax.");
        }

        [Fact]
        public void ParsePathShouldWork()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People", UriKind.Relative)).ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void ParseQueryOptionsShouldWork()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People?$filter=MyDog/Color eq 'Brown'&$select=ID&$expand=MyDog&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA&$unknow=&$unknowvalue&$skiptoken=abc&$deltatoken=def", UriKind.Relative));
            parser.ParseSelectAndExpand().Should().NotBeNull();
            parser.ParseFilter().Should().NotBeNull();
            parser.ParseOrderBy().Should().NotBeNull();
            parser.ParseTop().Should().Be(1);
            parser.ParseSkip().Should().Be(2);
            parser.ParseCount().Should().Be(true);
            parser.ParseSearch().Should().NotBeNull();
            parser.ParseSkipToken().Should().Be("abc");
            parser.ParseDeltaToken().Should().Be("def");
        }

        [Fact]
        public void ParseDeltaTokenWithKindsofCharactorsShouldWork()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People?$deltatoken=Start@Next_Chunk:From%26$To=Here!?()*+%2B,1-._~;", UriKind.Relative));
            parser.ParseDeltaToken().Should().Be("Start@Next_Chunk:From&$To=Here!?()* +,1-._~;");
        }

        #endregion

        #region ODataUnrecognizedPathException tests
        [Fact]
        public void ParsePathExceptionDataTestWithInvalidEntitySet()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People1(1)/MyDog/Color", UriKind.Relative)).ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(0);
            ex.CurrentSegment.Should().Be("People1(1)");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(2);
        }

        [Fact]
        public void ParsePathExceptionDataTestWithInvalidContainedNavigationProperty()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/MyDog1/Color", UriKind.Relative)).ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(2);
            ex.CurrentSegment.Should().Be("MyDog1");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(1);
        }

        [Fact]
        public void ParsePathExceptionDataTestInvalidNavigationProperty()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/MyDog/Color1", UriKind.Relative));
            Action action = () => parser.ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(3);
            ex.CurrentSegment.Should().Be("Color1");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(0);
            parser.ParameterAliasNodes.Count.Should().Be(0);
        }

        [Fact]
        public void ParsePathExceptionDataTestWithAlias()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("GetCoolestPersonWithStyle(styleID=@p1)/UndeclaredProperty?@p1=32", UriKind.Relative));
            Action action = () => parser.ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(1);
            ex.CurrentSegment.Should().Be("UndeclaredProperty");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(0);

            var aliasNode = parser.ParameterAliasNodes;
            aliasNode.Count.Should().Be(1);
            aliasNode["@p1"].ShouldBeConstantQueryNode(32);
        }
        #endregion

        #region Composite key parse test

        public enum TestUrlConvention
        {
            Default,
            KeyAsSegment,
            ODataSimplified
        }

        [Theory]
        [InlineData(TestUrlConvention.Default, "http://host/tableDefinitions('tableKey')/columnDefinitions(tableId='tableKey',propertyName='columnKey')/Test.ChoiceColumnDefinition/choices(tableId='tableKey',propertyName='columnKey',id=1)")]
        [InlineData(TestUrlConvention.Default, "http://host/tableDefinitions('tableKey')/columnDefinitions('columnKey')/Test.ChoiceColumnDefinition/choices(1)")]
        [InlineData(TestUrlConvention.ODataSimplified, "http://host/tableDefinitions/tableKey/columnDefinitions/columnKey/Test.ChoiceColumnDefinition/choices/1")]
        [InlineData(TestUrlConvention.KeyAsSegment, "http://host/tableDefinitions/tableKey/columnDefinitions/columnKey/Test.ChoiceColumnDefinition/choices/1")]
        [InlineData(TestUrlConvention.Default, "http://host/tableDefinitions('tableKey')/columnDefinitions(tableId='tableKey',propertyName='columnKey')")]
        [InlineData(TestUrlConvention.Default, "http://host/tableDefinitions('tableKey')/columnDefinitions('columnKey')")]
        [InlineData(TestUrlConvention.ODataSimplified, "http://host/tableDefinitions/tableKey/columnDefinitions/columnKey")]
        [InlineData(TestUrlConvention.KeyAsSegment, "http://host/tableDefinitions/tableKey/columnDefinitions/columnKey")]
        public void ParseCompositeKeyReference(TestUrlConvention testUrlConvention, string fullUrl)
        {
            var model = new EdmModel();

            var tableDefinition = new EdmEntityType("Test", "TableDefinition", null, false, true);
            var tableDefinitionId = tableDefinition.AddStructuralProperty("id", EdmPrimitiveTypeKind.String, false);
            tableDefinition.AddKeys(tableDefinitionId);
            model.AddElement(tableDefinition);

            var columnDefinition = new EdmEntityType("Test", "ColumnDefinition", null, false, true);
            var columnDefinitionTableId = columnDefinition.AddStructuralProperty("tableId", EdmPrimitiveTypeKind.String, true);
            var columnPropertyName = columnDefinition.AddStructuralProperty("propertyName", EdmPrimitiveTypeKind.String, true);
            columnDefinition.AddKeys(columnDefinitionTableId, columnPropertyName);
            model.AddElement(columnDefinition);

            var tableDefinitionColumnDefinitions = tableDefinition.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                ContainsTarget = true,
                Name = "columnDefinitions",
                Target = columnDefinition,
                TargetMultiplicity = EdmMultiplicity.Many,
                DependentProperties = new[] { tableDefinitionId },
                PrincipalProperties = new[] { columnDefinitionTableId }
            });

            var choice = new EdmEntityType("Test", "Choice");
            var choiceTableId = choice.AddStructuralProperty("tableId", EdmPrimitiveTypeKind.String);
            var choicePropertyName = choice.AddStructuralProperty("propertyName", EdmPrimitiveTypeKind.String);
            choice.AddKeys(choiceTableId, choicePropertyName,
                choice.AddStructuralProperty("id", EdmPrimitiveTypeKind.Int32, false));
            model.AddElement(choice);

            var choiceColumnDefinition = new EdmEntityType("Test", "ChoiceColumnDefinition", columnDefinition);
            var choiceColumnDefinitionChoices = choiceColumnDefinition.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    ContainsTarget = true,
                    Target = choice,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Name = "choices",
                    DependentProperties = new[] { columnPropertyName, columnDefinitionTableId },
                    PrincipalProperties = new[] { choicePropertyName, choiceTableId }
                });
            model.AddElement(choiceColumnDefinition);

            var container = new EdmEntityContainer("Test", "Container");
            var tableDefinitions = container.AddEntitySet("tableDefinitions", tableDefinition);
            model.AddElement(container);

            var parser = new ODataUriParser(model, new Uri("http://host"), new Uri(fullUrl));
            switch (testUrlConvention)
            {
                case TestUrlConvention.Default:
                    parser.UrlConventions = ODataUrlConventions.Default;
                    break;
                case TestUrlConvention.KeyAsSegment:
                    parser.UrlConventions = ODataUrlConventions.KeyAsSegment;
                    break;
                case TestUrlConvention.ODataSimplified:
                    parser.UrlConventions = ODataUrlConventions.ODataSimplified;
                    break;
                default:
                    Assert.True(false, "Unreachable code path");
                    break;
            }

            var path = parser.ParsePath().ToList();
            path[0].ShouldBeEntitySetSegment(tableDefinitions);
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("id", "tableKey"));
            path[2].ShouldBeNavigationPropertySegment(tableDefinitionColumnDefinitions);
            path[3].ShouldBeKeySegment(new KeyValuePair<string, object>("tableId", "tableKey"),
                new KeyValuePair<string, object>("propertyName", "columnKey"));
            if (path.Count > 4)
            {
                // For tests with a type cast and a second-level navigation property.
                path[4].ShouldBeTypeSegment(choiceColumnDefinition);
                path[5].ShouldBeNavigationPropertySegment(choiceColumnDefinitionChoices);
                path[6].ShouldBeKeySegment(new KeyValuePair<string, object>("tableId", "tableKey"),
                new KeyValuePair<string, object>("propertyName", "columnKey"),
                new KeyValuePair<string, object>("id", 1));
            }
        }

        #endregion
    }
}
