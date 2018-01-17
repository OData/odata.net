//---------------------------------------------------------------------
// <copyright file="ODataUriParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Unit tests for ODataUriParser.
    /// </summary>
    public class ODataUriParserTests
    {
        private readonly Uri ServiceRoot = new Uri("http://host");
        private readonly Uri FullUri = new Uri("http://host/People");

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void NonQueryOptionShouldWork(bool enableNoDollarQueryOptions)
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri);
            uriParser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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

        [Theory]
        [InlineData("?filter=&select=&expand=&orderby=&top=&skip=&count=&search=&unknown=&$unknownvalue&skiptoken=&deltatoken=&$compute=", true)]
        [InlineData("?$filter=&$select=&$expand=&$orderby=&$top=&$skip=&$count=&$search=&$unknown=&$unknownvalue&$skiptoken=&$deltatoken=&$compute=", true)]
        [InlineData("?$filter=&$select=&$expand=&$orderby=&$top=&$skip=&$count=&$search=&$unknown=&$unknownvalue&$skiptoken=&$deltatoken=&$compute=", false)]
        public void EmptyValueQueryOptionShouldWork(string relativeUriString, bool enableNoDollarQueryOptions)
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(FullUri, relativeUriString));
            uriParser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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

        [Fact]
        public void DupilicateNonODataQueryOptionShouldWork()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(FullUri, "?$filter=UserName eq 'foo'&$filter=UserName eq 'bar'")).ParsePath();
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(FullUri, "?$filter=UserName eq 'Tom'&nonODataQuery=foo&$select=Emails&nonODataQuery=bar"));
            var nonODataqueryOptions = uriParser.CustomQueryOptions;

            action.ShouldThrow<ODataException>().WithMessage(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce("$filter"));
            Assert.Equal(nonODataqueryOptions.Count, 2);
            Assert.True(nonODataqueryOptions[0].Key.Equals("nonODataQuery") && nonODataqueryOptions[1].Key.Equals("nonODataQuery"));
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

        [Theory]
        [InlineData("http://host/People?filter=1 eq 1", true)]
        [InlineData("http://host/People?$filter=1 eq 1", true)]
        [InlineData("http://host/People?$filter=1 eq 1", false)]
        public void FilterLimitIsRespectedForFilter(string fullUriString, bool enableNoDollarQueryOptions)
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(fullUriString)) { Settings = { FilterLimit = 0 } };
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
            Action parseWithLimit = () => parser.ParseFilter();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Theory]
        [InlineData("http://host/People?filter=MyDog/Color eq 'Brown' or MyDog/Color eq 'White'", true)]
        [InlineData("http://host/People?$filter=MyDog/Color eq 'Brown' or MyDog/Color eq 'White'", true)]
        [InlineData("http://host/People?$filter=MyDog/Color eq 'Brown' or MyDog/Color eq 'White'", false)]
        public void FilterLimitWithInterestingTreeStructures(string fullUriString, bool enableNoDollarQueryOptions)
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(fullUriString)) { Settings = { FilterLimit = 5 } };
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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

        [Theory]
        [InlineData("http://host/People?orderby= 1 eq 1", true)]
        [InlineData("http://host/People?$orderby= 1 eq 1", true)]
        [InlineData("http://host/People?$orderby= 1 eq 1", false)]
        public void OrderByLimitIsRespectedForOrderby(string fullUriString, bool enableNoDollarQueryOptions)
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(fullUriString)) { Settings = { OrderByLimit = 0 } };
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
            Action parseWithLimit = () => parser.ParseOrderBy();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Theory]
        [InlineData("http://host/People?orderby=MyDog/MyPeople/MyDog/MyPeople/MyPaintings asc", true)]
        [InlineData("http://host/People?$orderby=MyDog/MyPeople/MyDog/MyPeople/MyPaintings asc", true)]
        [InlineData("http://host/People?$orderby=MyDog/MyPeople/MyDog/MyPeople/MyPaintings asc", false)]
        public void OrderByLimitWithInterestingTreeStructures(string fullUriString, bool enableNoDollarQueryOptions)
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(fullUriString)) { Settings = { OrderByLimit = 5 } };
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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

        [Theory]
        [InlineData("http://host/People?select=MyDog&expand=MyDog(select=color)", true)]
        [InlineData("http://host/People?$select=MyDog&$expand=MyDog($select=color)", true)]
        [InlineData("http://host/People?$select=MyDog&$expand=MyDog($select=color)", false)]
        public void SelectExpandLimitIsRespectedForSelectExpand(string fullUriString, bool enableNoDollarQueryOptions)
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(fullUriString)) { Settings = { SelectExpandLimit = 0 } };
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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
        public void DefaultKeyDelimiterShouldBeSlash()
        {
            new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlKeyDelimiter.Should().BeSameAs(ODataUrlKeyDelimiter.Slash);
        }

        [Fact]
        public void ODataUrlKeyDelimiterCannotBeSetToNull()
        {
            Action setToNull = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlKeyDelimiter = null;
            setToNull.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("UrlKeyDelimiter"));
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

        [Theory]
        [InlineData("http://host/People?select=MyContainedDog&expand=MyContainedDog", true)]
        [InlineData("http://host/People?$select=MyContainedDog&$expand=MyContainedDog", true)]
        [InlineData("http://host/People?$select=MyContainedDog&$expand=MyContainedDog", false)]
        public void ParseSelectExpandForContainment(string fullUriString, bool enableNoDollarQueryOptions)
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(fullUriString)) { Settings = { SelectExpandLimit = 5 } };
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriParser_RelativeUriMustBeRelative);
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

        [Theory]
        [InlineData("People?filter=MyDog/Color eq 'Brown'&select=ID&expand=MyDog&orderby=ID&top=1&skip=2&count=true&search=FA&$unknown=&$unknownvalue&skiptoken=abc&deltatoken=def", true)]
        [InlineData("People?$filter=MyDog/Color eq 'Brown'&$select=ID&$expand=MyDog&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA&$unknown=&$unknownvalue&$skiptoken=abc&$deltatoken=def", true)]
        [InlineData("People?$filter=MyDog/Color eq 'Brown'&$select=ID&$expand=MyDog&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA&$unknown=&$unknownvalue&$skiptoken=abc&$deltatoken=def", false)]
        public void ParseQueryOptionsShouldWork(string relativeUriString, bool enableNoDollarQueryOptions)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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
        public void ParseNoDollarQueryOptionsShouldReturnNullIfNoDollarQueryOptionsIsNotEnabled()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People?filter=MyDog/Color eq 'Brown'&select=ID&expand=MyDog&orderby=ID&top=1&skip=2&count=true&search=FA&$unknown=&$unknownvalue&skiptoken=abc&deltatoken=def", UriKind.Relative));
            parser.ParseFilter().Should().BeNull();
            parser.ParseSelectAndExpand().Should().BeNull();
            parser.ParseOrderBy().Should().BeNull();
            parser.ParseTop().Should().Be(null);
            parser.ParseSkip().Should().Be(null);
            parser.ParseCount().Should().Be(null);
            parser.ParseSearch().Should().BeNull();
            parser.ParseSkipToken().Should().BeNull();
            parser.ParseDeltaToken().Should().BeNull();
        }

        [Theory]
        // Should not throw duplicate query options exception.
        // 1. Case sensitive, No dollar enabled.
        [InlineData("People?select=ID&$SELECT=Name", false, true, "select", false)]
        [InlineData("People?SELECT=ID&$select=Name", false, true, "select", false)]
        [InlineData("People?SELECT=ID&$SELECT=Name", false, true, "select", false)]
        // 2. Case insensitive, No dollar not enabled.
        [InlineData("People?$select=ID&select=Name", true, false, "$select", false)]
        [InlineData("People?$select=ID&SELECT=Name", true, false, "$select", false)]

        // 3. Case insensitive, No dollar not enabled, be treated as custom query options.
        // Duplication is allowed.
        [InlineData("People?select=ID&select=Name", false, false, "select", false)]

        // 4. Should throw duplicate query options exception.
        [InlineData("People?$select=ID&$select=Name", false, false, "$select", true)]
        [InlineData("People?select=ID&$select=Name", false, true, "select", true)]
        [InlineData("People?$select=ID&$SELECT=Name", true, false, "$select", true)]
        [InlineData("People?$select=ID&$SELECT=Name", true, true, "select", true)]
        [InlineData("People?select=ID&$SELECT=Name", true, true, "select", true)]
        public void ParseShouldFailWithDuplicateQueryOptions(string relativeUriString, bool enableCaseInsensitive, bool enableNoDollarQueryOptions, string queryOptionName, bool shouldThrow)
        {
            Uri relativeUri = new Uri(relativeUriString, UriKind.Relative);
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, relativeUri)
            {
                Resolver = new ODataUriResolver()
                {
                    EnableCaseInsensitive = enableCaseInsensitive
                },

                EnableNoDollarQueryOptions = enableNoDollarQueryOptions

            }.ParseSelectAndExpand();

            if (shouldThrow)
            {
                action.ShouldThrow<ODataException>().WithMessage(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(
                    enableNoDollarQueryOptions ? string.Format(CultureInfo.InvariantCulture, "${0}/{0}", queryOptionName) : queryOptionName));
            }
            else
            {
                action.ShouldNotThrow<ODataException>();
            }
        }

        [Theory]
        [InlineData("People?expand=MyDog(select=ID,Color)")]
        [InlineData("People?$expand=MyDog(select=ID,Color)")]
        [InlineData("People?expand=MyDog(expand=MyPeople(select=Name))")]
        [InlineData("People?expand=MyDog($expand=MyPeople($select=Name))")]
        [InlineData("People?expand=MyDog(select=Color;expand=MyPeople(select=Name;count=true))")]
        [InlineData("People?$expand=MyDog($select=Color;expand=MyPeople(select=Name;$count=true))")]
        public void ParseNestedNoDollarQueryOptionsShouldWorkWhenNoDollarQueryOptionsIsEnabled(string relativeUriString)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            parser.EnableNoDollarQueryOptions = true;
            parser.ParseSelectAndExpand().Should().NotBeNull();
        }

        [Theory]
        [InlineData("People?deltatoken=Start@Next_Chunk:From%26$To=Here!?()*+%2B,1-._~;", true)]
        [InlineData("People?$deltatoken=Start@Next_Chunk:From%26$To=Here!?()*+%2B,1-._~;", true)]
        [InlineData("People?$deltatoken=Start@Next_Chunk:From%26$To=Here!?()*+%2B,1-._~;", false)]
        public void ParseDeltaTokenWithKindsofCharactorsShouldWork(string relativeUriString, bool enableNoDollarQueryOptions)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
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

        public enum TestODataUrlKeyDelimiter
        {
            Parentheses,
            Slash,
        }

        [Theory]
        [InlineData(TestODataUrlKeyDelimiter.Parentheses, "http://host/customers('customerId')/orders(customerId='customerId',orderId='orderId')/Test.DetailedOrder/details(customerId='customerId',orderId='orderId',id=1)")]
        [InlineData(TestODataUrlKeyDelimiter.Parentheses, "http://host/customers('customerId')/orders('orderId')/Test.DetailedOrder/details(1)")]
        [InlineData(TestODataUrlKeyDelimiter.Slash, "http://host/customers/customerId/orders/orderId/Test.DetailedOrder/details/1")]
        [InlineData(TestODataUrlKeyDelimiter.Parentheses, "http://host/customers('customerId')/orders(customerId='customerId',orderId='orderId')")]
        [InlineData(TestODataUrlKeyDelimiter.Parentheses, "http://host/customers('customerId')/orders('orderId')")]
        [InlineData(TestODataUrlKeyDelimiter.Slash, "http://host/customers('customerId')/orders('orderId')")]
        [InlineData(TestODataUrlKeyDelimiter.Slash, "http://host/customers/customerId/orders/orderId")]
        public void ParseCompositeKeyReference(TestODataUrlKeyDelimiter testODataUrlKeyDelimiter, string fullUrl)
        {
            var model = new EdmModel();

            var customer = new EdmEntityType("Test", "Customer", null, false, true);
            var customerId = customer.AddStructuralProperty("id", EdmPrimitiveTypeKind.String, false);
            customer.AddKeys(customerId);
            model.AddElement(customer);

            var order = new EdmEntityType("Test", "Order", null, false, true);
            var orderCustomerId = order.AddStructuralProperty("customerId", EdmPrimitiveTypeKind.String, true);
            var orderOrderId = order.AddStructuralProperty("orderId", EdmPrimitiveTypeKind.String, true);
            order.AddKeys(orderCustomerId, orderOrderId);
            model.AddElement(order);

            var customerOrders = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                ContainsTarget = true,
                Name = "orders",
                Target = order,
                TargetMultiplicity = EdmMultiplicity.Many,
                DependentProperties = new[] { customerId },
                PrincipalProperties = new[] { orderCustomerId }
            });

            var detail = new EdmEntityType("Test", "Detail");
            var detailCustomerId = detail.AddStructuralProperty("customerId", EdmPrimitiveTypeKind.String);
            var detailOrderId = detail.AddStructuralProperty("orderId", EdmPrimitiveTypeKind.String);
            detail.AddKeys(detailCustomerId, detailOrderId,
                detail.AddStructuralProperty("id", EdmPrimitiveTypeKind.Int32, false));
            model.AddElement(detail);

            var detailedOrder = new EdmEntityType("Test", "DetailedOrder", order);
            var detailedOrderDetails = detailedOrder.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    ContainsTarget = true,
                    Target = detail,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Name = "details",
                    DependentProperties = new[] { orderOrderId, orderCustomerId },
                    PrincipalProperties = new[] { detailOrderId, detailCustomerId }
                });
            model.AddElement(detailedOrder);

            var container = new EdmEntityContainer("Test", "Container");
            var customers = container.AddEntitySet("customers", customer);
            model.AddElement(container);

            var parser = new ODataUriParser(model, new Uri("http://host"), new Uri(fullUrl));
            switch (testODataUrlKeyDelimiter)
            {
                case TestODataUrlKeyDelimiter.Parentheses:
                    parser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
                    break;
                case TestODataUrlKeyDelimiter.Slash:
                    parser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash;
                    break;
                default:
                    Assert.True(false, "Unreachable code path");
                    break;
            }

            var path = parser.ParsePath().ToList();
            path[0].ShouldBeEntitySetSegment(customers);
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("id", "customerId"));
            path[2].ShouldBeNavigationPropertySegment(customerOrders);
            path[3].ShouldBeKeySegment(new KeyValuePair<string, object>("customerId", "customerId"),
                new KeyValuePair<string, object>("orderId", "orderId"));
            if (path.Count > 4)
            {
                // For tests with a type cast and a second-level navigation property.
                path[4].ShouldBeTypeSegment(detailedOrder);
                path[5].ShouldBeNavigationPropertySegment(detailedOrderDetails);
                path[6].ShouldBeKeySegment(new KeyValuePair<string, object>("customerId", "customerId"),
                new KeyValuePair<string, object>("orderId", "orderId"),
                new KeyValuePair<string, object>("id", 1));
            }
        }
        #endregion

        #region Null EntitySetPath

        [Fact]
        public void ParsePathFunctionWithNullEntitySetPath()
        {
            var model = new EdmModel();

            var customer = new EdmEntityType("Test", "Customer", null, false, false);
            var customerId = customer.AddStructuralProperty("id", EdmPrimitiveTypeKind.String, false);
            customer.AddKeys(customerId);
            model.AddElement(customer);

            var detail = new EdmEntityType("Test", "Detail", null, false, true);
            detail.AddStructuralProperty("address", EdmPrimitiveTypeKind.String, true);
            model.AddElement(detail);

            var customerDetail = customer.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "detail",
                    Target = detail,
                    TargetMultiplicity = EdmMultiplicity.One,
                    ContainsTarget = true
                });

            // The test is to make sure the ODataUriParser works even though
            // the entitySetPathExpression is null.
            var getCurrentCustomer = new EdmFunction(
                "Test",
                "getCurrentCustomer",
                new EdmEntityTypeReference(customer, false),
                isBound: false,
                entitySetPathExpression: null,
                isComposable: true);
            model.AddElement(getCurrentCustomer);

            var container = new EdmEntityContainer("Test", "Container");
            var getCurrentCustomerImport = container.AddFunctionImport(getCurrentCustomer);
            model.AddElement(container);

            var parser = new ODataUriParser(model, new Uri("http://host"), new Uri("http://host/getCurrentCustomer()/detail"));
            var path = parser.ParsePath();
            var pathSegmentList = path.ToList();
            pathSegmentList.Count.Should().Be(2);
            pathSegmentList[0].ShouldBeOperationImportSegment(getCurrentCustomerImport);
            pathSegmentList[1].ShouldBeNavigationPropertySegment(customerDetail);
        }
        #endregion

        #region Parse Compute Tests

        [Fact]
        public void ParseBadComputeWithMissingAs()
        {
            Uri url = new Uri("http://host/Paintings?$compute=nonsense");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, url);
            Action action = () => parser.ParseCompute();
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_AsExpected(8, "nonsense"));
        }

        [Fact]
        public void ParseBadComputeWithMissingAlias()
        {
            Uri url = new Uri("http://host/Paintings?$compute=nonsense as");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, url);
            Action action = () => parser.ParseCompute();
            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null or empty.\r\nParameter name: alias");
        }

        [Fact]
        public void ParseComputeAsQueryOption()
        {
            // Create model
            EdmModel model = new EdmModel();
            EdmEntityType elementType = model.AddEntityType("DevHarness", "Entity");
            EdmTypeReference typeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            elementType.AddProperty(new EdmStructuralProperty(elementType, "Prop1", typeReference));

            EdmEntityContainer container = model.AddEntityContainer("Default", "Container");
            container.AddEntitySet("Entities", elementType);

            // Define queries and new up parser.
            Uri root = new Uri("http://host");
            Uri url = new Uri("http://host/Entities?$compute=cast(Prop1, 'Edm.String') as Property1AsString, tolower(Prop1) as Property1Lower");
            ODataUriParser parser = new ODataUriParser(model, root, url);

            // parse and validate
            ComputeClause clause = parser.ParseCompute();
            List<ComputeExpression> items = clause.ComputedItems.ToList();
            items.Count().Should().Be(2);
            items[0].Alias.ShouldBeEquivalentTo("Property1AsString");
            items[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            items[0].Expression.TypeReference.ShouldBeEquivalentTo(typeReference);
            items[1].Alias.ShouldBeEquivalentTo("Property1Lower");
            items[1].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            items[1].Expression.TypeReference.FullName().ShouldBeEquivalentTo("Edm.String");
            items[1].Expression.TypeReference.IsNullable.ShouldBeEquivalentTo(true); // tolower is built in function that allows nulls.

            ComputeExpression copy = new ComputeExpression(items[0].Expression, items[0].Alias, null);
            copy.Expression.Should().NotBeNull();
            copy.TypeReference.Should().BeNull();
            ComputeClause varied = new ComputeClause(null);
        }

        [Fact]
        public void ParseComputeAsExpandQueryOption()
        {
            // Create model
            EdmModel model = new EdmModel();
            EdmEntityType elementType = model.AddEntityType("DevHarness", "Entity");
            EdmEntityType targetType = model.AddEntityType("DevHarness", "Navigation");
            EdmTypeReference typeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            targetType.AddProperty(new EdmStructuralProperty(targetType, "Prop1", typeReference));
            EdmNavigationPropertyInfo propertyInfo = new EdmNavigationPropertyInfo();
            propertyInfo.Name = "Nav1";
            propertyInfo.Target = targetType;
            propertyInfo.TargetMultiplicity = EdmMultiplicity.One;
            EdmProperty navigation = EdmNavigationProperty.CreateNavigationProperty(elementType, propertyInfo);
            elementType.AddProperty(navigation);

            EdmEntityContainer container = model.AddEntityContainer("Default", "Container");
            container.AddEntitySet("Entities", elementType);

            // Define queries and new up parser.
            Uri root = new Uri("http://host");
            Uri url = new Uri("http://host/Entities?$expand=Nav1($compute=cast(Prop1, 'Edm.String') as NavProperty1AsString)");
            ODataUriParser parser = new ODataUriParser(model, root, url);

            // parse and validate
            SelectExpandClause clause = parser.ParseSelectAndExpand();
            List<SelectItem> items = clause.SelectedItems.ToList();
            items.Count.Should().Be(1);
            ExpandedNavigationSelectItem expanded = items[0] as ExpandedNavigationSelectItem;
            List<ComputeExpression> computes = expanded.ComputeOption.ComputedItems.ToList();
            computes.Count.Should().Be(1);
            computes[0].Alias.ShouldBeEquivalentTo("NavProperty1AsString");
            computes[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            computes[0].Expression.TypeReference.ShouldBeEquivalentTo(typeReference);
        }

        [Fact]
        public void ParseComputeAsLevel2ExpandQueryOption()
        {
            // Create model
            EdmModel model = new EdmModel();
            EdmEntityType elementType = model.AddEntityType("DevHarness", "Entity");
            EdmEntityType targetType = model.AddEntityType("DevHarness", "Navigation");
            EdmEntityType subTargetType = model.AddEntityType("DevHarness", "SubNavigation");

            EdmTypeReference typeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            elementType.AddProperty(new EdmStructuralProperty(elementType, "Prop1", typeReference));
            targetType.AddProperty(new EdmStructuralProperty(targetType, "Prop1", typeReference));
            subTargetType.AddProperty(new EdmStructuralProperty(subTargetType, "Prop1", typeReference));

            EdmNavigationPropertyInfo propertyInfo = new EdmNavigationPropertyInfo();
            propertyInfo.Name = "Nav1";
            propertyInfo.Target = targetType;
            propertyInfo.TargetMultiplicity = EdmMultiplicity.One;
            EdmProperty navigation = EdmNavigationProperty.CreateNavigationProperty(elementType, propertyInfo);
            elementType.AddProperty(navigation);

            EdmNavigationPropertyInfo subPropertyInfo = new EdmNavigationPropertyInfo();
            subPropertyInfo.Name = "SubNav1";
            subPropertyInfo.Target = subTargetType;
            subPropertyInfo.TargetMultiplicity = EdmMultiplicity.One;
            EdmProperty subnavigation = EdmNavigationProperty.CreateNavigationProperty(targetType, subPropertyInfo);
            targetType.AddProperty(subnavigation);

            EdmEntityContainer container = model.AddEntityContainer("Default", "Container");
            container.AddEntitySet("Entities", elementType);

            // Define queries and new up parser.
            string address = "http://host/Entities?$compute=cast(Prop1, 'Edm.String') as Property1AsString, tolower(Prop1) as Property1Lower&" +
                                                  "$expand=Nav1($compute=cast(Prop1, 'Edm.String') as NavProperty1AsString;" +
                                                               "$expand=SubNav1($compute=cast(Prop1, 'Edm.String') as SubNavProperty1AsString))";
            Uri root = new Uri("http://host");
            Uri url = new Uri(address);
            ODataUriParser parser = new ODataUriParser(model, root, url);

            // parse
            ComputeClause computeClause = parser.ParseCompute();
            SelectExpandClause selectClause = parser.ParseSelectAndExpand();

            // validate top compute
            List<ComputeExpression> items = computeClause.ComputedItems.ToList();
            items.Count().Should().Be(2);
            items[0].Alias.ShouldBeEquivalentTo("Property1AsString");
            items[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            items[0].Expression.TypeReference.ShouldBeEquivalentTo(typeReference);
            items[1].Alias.ShouldBeEquivalentTo("Property1Lower");
            items[1].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            items[1].Expression.TypeReference.FullName().ShouldBeEquivalentTo("Edm.String");
            items[1].Expression.TypeReference.IsNullable.ShouldBeEquivalentTo(true); // tolower is built in function that allows nulls.

            // validate level 1 expand compute
            List<SelectItem> selectItems = selectClause.SelectedItems.ToList();
            selectItems.Count.Should().Be(1);
            ExpandedNavigationSelectItem expanded = selectItems[0] as ExpandedNavigationSelectItem;
            List<ComputeExpression> computes = expanded.ComputeOption.ComputedItems.ToList();
            computes.Count.Should().Be(1);
            computes[0].Alias.ShouldBeEquivalentTo("NavProperty1AsString");
            computes[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            computes[0].Expression.TypeReference.ShouldBeEquivalentTo(typeReference);

            // validate level 2 expand compute
            List<SelectItem> subSelectItems = expanded.SelectAndExpand.SelectedItems.ToList();
            subSelectItems.Count.Should().Be(1);
            ExpandedNavigationSelectItem subExpanded = subSelectItems[0] as ExpandedNavigationSelectItem;
            List<ComputeExpression> subComputes = subExpanded.ComputeOption.ComputedItems.ToList();
            subComputes.Count.Should().Be(1);
            subComputes[0].Alias.ShouldBeEquivalentTo("SubNavProperty1AsString");
            subComputes[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            subComputes[0].Expression.TypeReference.ShouldBeEquivalentTo(typeReference);
        }
        #endregion
    }
}