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
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
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
        [InlineData("?filter=&select=&expand=&orderby=&top=&skip=&index=&count=&search=&unknown=&$unknownvalue&skiptoken=&deltatoken=&$compute=", true)]
        [InlineData("?$filter=&$select=&$expand=&$orderby=&$top=&$skip=&$index=&$count=&$search=&$unknown=&$unknownvalue&$skiptoken=&$deltatoken=&$compute=", true)]
        [InlineData("?$filter=&$select=&$expand=&$orderby=&$top=&$skip=&$index=&$count=&$search=&$unknown=&$unknownvalue&$skiptoken=&$deltatoken=&$compute=", false)]
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
            action = () => uriParser.ParseIndex();
            action.ShouldThrow<ODataException>().WithMessage(Strings.SyntacticTree_InvalidIndexQueryOptionValue(""));
            action = () => uriParser.ParseCount();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataUriParser_InvalidCount(""));
            action = () => uriParser.ParseSearch();
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(0, ""));
            uriParser.ParseSkipToken().Should().BeEmpty();
            uriParser.ParseDeltaToken().Should().BeEmpty();
        }

        [Fact]
        public void ParseAnnotationInFilterForOpenTypeShouldWork()
        {
            Uri entitySetUri = new Uri("http://host/Paintings");
            var filterClauseString = "?filter=@my.annotation eq 5";

            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(entitySetUri, filterClauseString));
            uriParser.EnableNoDollarQueryOptions = true;
            var path = uriParser.ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPaintingsSet());
            var filterResult = uriParser.ParseFilter();
            filterResult.Should().NotBeNull();
            filterResult.Expression.Kind.Should().Be(QueryNodeKind.BinaryOperator);
            (filterResult.Expression as BinaryOperatorNode).Should().NotBeNull();
            (filterResult.Expression as BinaryOperatorNode).Left.Should().NotBeNull();
            (filterResult.Expression as BinaryOperatorNode).Right.Should().NotBeNull();

            var selectExpandResult = uriParser.ParseSelectAndExpand();
            selectExpandResult.Should().BeNull();
        }

        [Fact]
        public void ParseAnnotationInFilterForEntityTypeShouldThrow()
        {
            var filterClauseString = "?filter=@my.annotation eq 5";

            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(FullUri, filterClauseString));
            uriParser.EnableNoDollarQueryOptions = true;
            var path = uriParser.ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Action action  = () => uriParser.ParseFilter();
            action.ShouldThrow<ODataException>().WithMessage(
                ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "@my.annotation"));
        }

        [Fact]
        public void DupilicateNonODataQueryOptionShouldWork()
        {
            ODataUriParser uriParserProcessingDupODataSystemQuery = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot,
                new Uri(FullUri, "?$filter=UserName eq 'foo'&$filter=UserName eq 'bar'"));

            bool originalValue = uriParserProcessingDupODataSystemQuery.EnableNoDollarQueryOptions;
            try
            {
                // Set the parser option in the static singleton to be $-sign required.
                uriParserProcessingDupODataSystemQuery.EnableNoDollarQueryOptions = false;

                Action action = () => uriParserProcessingDupODataSystemQuery.ParsePath();

                var uriParserProcessingDupCustomQuery =
                    new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot,
                        new Uri(FullUri, "?$filter=UserName eq 'Tom'&nonODataQuery=foo&$select=Emails&nonODataQuery=bar"));
                var nonODataqueryOptions = uriParserProcessingDupCustomQuery.CustomQueryOptions;

                action.ShouldThrow<ODataException>()
                    .WithMessage(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce("$filter"));
                Assert.Equal(nonODataqueryOptions.Count, 2);
                Assert.True(nonODataqueryOptions[0].Key.Equals("nonODataQuery") &&
                            nonODataqueryOptions[1].Key.Equals("nonODataQuery"));
            }
            finally
            {
                // Restore original value
                uriParserProcessingDupODataSystemQuery.EnableNoDollarQueryOptions = originalValue;
            }
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
            new ODataUriResolver().EnableCaseInsensitive.Should().BeFalse();
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
        public void CompositeAlternateKeyShouldFailOnlyWithInvalidAlternateKey()
        {
            Uri fullUri = new Uri("http://host/People(NameAlias='anyName', FirstNameAlias='anyFirst', extraAltKey='any')");
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), fullUri)
            {
                Resolver = new AlternateKeysODataUriResolver(HardCodedTestModel.TestModel)
            }.ParsePath();

            action.ShouldThrow<ODataException>()
                .WithMessage(ODataErrorStrings.BadRequest_KeyCountMismatch(HardCodedTestModel.GetPersonType().FullTypeName()));
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

        [Theory]
        [InlineData("People(1)/RelatedIDs?index=42", true)]
        [InlineData("People(1)/RelatedIDs?$index=42", true)]
        [InlineData("People(1)/RelatedIDs?$index=42", false)]
        public void ParseIndexQueryOptionShouldWork(string relativeUriString, bool enableNoDollarQueryOptions)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
            parser.ParseIndex().Should().Be(42);
        }

        [Theory]
        [InlineData("People?$select=@Fully.Qualified.Namespace.PrimitiveTerm", "Fully.Qualified.Namespace.PrimitiveTerm", "Edm.String", false)]
        [InlineData("People?$select=@FuLly.quAlIfIeD.naMesPaCE.PRImiTIveTErm", "Fully.Qualified.Namespace.PrimitiveTerm", "Edm.String", true)]
        [InlineData("People?$select=@Fully.Qualified.Namespace.ComplexTerm", "Fully.Qualified.Namespace.ComplexTerm", "Fully.Qualified.Namespace.Address", false)]
        [InlineData("People?$select=@fUllY.QUalIFieD.NAMespaCe.compLExTerm", "Fully.Qualified.Namespace.ComplexTerm", "Fully.Qualified.Namespace.Address", true)]
        [InlineData("People?$select=@Org.OData.Core.V1.Description", "Org.OData.Core.V1.Description", "Edm.String", false)]
        [InlineData("People?$select=@ORg.oDAta.cOrE.V1.dEscrIPtioN", "Org.OData.Core.V1.Description", "Edm.String", true)]
        [InlineData("People?$select=@Fully.Qualified.Namespace.UnknownTerm", "Fully.Qualified.Namespace.UnknownTerm", "Edm.Untyped", false)]
        [InlineData("People?$select=@fuLLy.quaLIfied.NAMespACe.uNKNownTerm", "fuLLy.quaLIfied.NAMespACe.uNKNownTerm", "Edm.Untyped", true)]
        public void ParseSelectAnnotationShouldWork(string relativeUriString, string termName, string typeName, bool caseInsensitive)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            parser.Resolver.EnableCaseInsensitive = caseInsensitive;
            var selectExpand = parser.ParseSelectAndExpand();
            selectExpand.Should().NotBeNull();
            PathSelectItem selectItem = selectExpand.SelectedItems.First() as PathSelectItem;
            selectItem.Should().NotBeNull();
            AnnotationSegment annotationSegment = selectItem.SelectedPath.FirstSegment as AnnotationSegment;
            annotationSegment.Should().NotBeNull();
            annotationSegment.Term.FullName().Should().Be(termName);
            annotationSegment.Term.Type.FullName().Should().Be(typeName);
        }

        [Theory]
        [InlineData("People?$select=@odata.type", "@odata.type")]
        [InlineData("People?$select=@odata.unknown", "@odata.unknown")]
        public void ParseSelectODataControlInformationShouldFail(string relativeUriString, string term)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            Action action = () => parser.ParseSelectAndExpand();
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriSelectParser_TermIsNotValid(term));
        }

        [Fact]
        public void ParseSelectPropertyAnnotationShouldWork()
        {
            string relativeUriString = "People?$select=Name/@Fully.Qualified.Namespace.PrimitiveTerm";
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            var selectExpand = parser.ParseSelectAndExpand();
            selectExpand.Should().NotBeNull();
            PathSelectItem selectItem = selectExpand.SelectedItems.First() as PathSelectItem;
            selectItem.Should().NotBeNull();
            List<ODataPathSegment> segments = selectItem.SelectedPath.ToList();
            segments.Count.Should().Be(2);
            PropertySegment propertySegment = segments[0] as PropertySegment;
            propertySegment.Property.Name.Should().Be("Name");
            AnnotationSegment annotationSegment = segments[1] as AnnotationSegment;
            annotationSegment.Term.FullName().Should().Be("Fully.Qualified.Namespace.PrimitiveTerm");
            annotationSegment.Term.Type.TypeKind().Should().Be(EdmTypeKind.Primitive);
        }

        [Fact]
        public void ParseSelectComplexAnnotationWithPathShouldWork()
        {
            string relativeUriString = "People?$select=@Fully.Qualified.Namespace.ComplexTerm/Street";
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            var selectExpand = parser.ParseSelectAndExpand();
            selectExpand.Should().NotBeNull();
            PathSelectItem selectItem = selectExpand.SelectedItems.First() as PathSelectItem;
            List<ODataPathSegment> segments = selectItem.SelectedPath.ToList();
            segments.Count.Should().Be(2);
            AnnotationSegment annotationSegment = segments[0] as AnnotationSegment;
            annotationSegment.Term.FullName().Should().Be("Fully.Qualified.Namespace.ComplexTerm");
            annotationSegment.Term.Type.FullName().Should().Be("Fully.Qualified.Namespace.Address");
            PropertySegment propertySegment = segments[1] as PropertySegment;
            propertySegment.Property.Name.Should().Be("Street");
        }

        [Fact]
        public void ParseNoDollarQueryOptionsShouldReturnNullIfNoDollarQueryOptionsIsNotEnabled()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People?filter=MyDog/Color eq 'Brown'&select=ID&expand=MyDog&orderby=ID&top=1&skip=2&count=true&search=FA&$unknown=&$unknownvalue&skiptoken=abc&deltatoken=def", UriKind.Relative));

            bool originalValue = parser.EnableNoDollarQueryOptions;
            try
            {
                // Ensure $-sign is required.
                parser.EnableNoDollarQueryOptions = false;

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
            finally
            {
                // Restore original value
                parser.EnableNoDollarQueryOptions = originalValue;
            }
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
        // 3. Case sensitive, No dollar not enabled, be treated as custom query options.
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

        [Theory]
        [InlineData("NS.GetMinSalary(minSalary=1,maxSalary=2)", 2)]
        [InlineData("NS.GetMinSalary(minSalary=1)", 1)]
        [InlineData("NS.GetMinSalary()", 0)]
        public void ParsePathBoundFunctionWithOptionalParametersWorks(string functionPath, int expectedParameterCount)
        {
            // Arrange
            var model = new EdmModel();

            var customer = new EdmEntityType("NS", "Customer", null, false, false);
            var customerId = customer.AddStructuralProperty("id", EdmPrimitiveTypeKind.String, false);
            customer.AddKeys(customerId);
            model.AddElement(customer);

            var function = new EdmFunction("NS", "GetMinSalary", EdmCoreModel.Instance.GetString(isNullable: true), true /*isBound*/, null /*entitySetPath*/, false /*iscomposable*/);
            IEdmTypeReference int32Type = EdmCoreModel.Instance.GetInt32(isNullable: true);
            function.AddParameter("p", new EdmEntityTypeReference(customer, isNullable: true));
            function.AddOptionalParameter("minSalary", int32Type);
            function.AddOptionalParameter("maxSalary", int32Type);
            model.AddElement(function);

            var container = new EdmEntityContainer("NS", "Container");
            var me = container.AddSingleton("me", customer);
            model.AddElement(container);

            // Act
            var parser = new ODataUriParser(model, new Uri("http://host"), new Uri("http://host/me/" + functionPath));
            var pathSegments = parser.ParsePath().ToList();

            // Assert
            Assert.Equal(2, pathSegments.Count);
            var operationSegment = Assert.IsType<OperationSegment>(pathSegments[1]);
            Assert.Equal("NS.GetMinSalary", operationSegment.Operations.First().FullName());
            Assert.Equal(expectedParameterCount, operationSegment.Parameters.Count());
        }

        [Theory]
        [InlineData("UnboundGetMinSalary(minSalary=1,maxSalary=2)", 2)]
        [InlineData("UnboundGetMinSalary(minSalary=1)", 1)]
        [InlineData("UnboundGetMinSalary()", 0)]
        public void ParsePathUnboundFunctionWithOptionalParametersWorks(string functionPath, int expectedParameterCount)
        {
            // Arrange
            var model = new EdmModel();

            var function = new EdmFunction("NS", "UnboundGetMinSalary", EdmCoreModel.Instance.GetString(isNullable: true), false /*isBound*/, null /*entitySetPath*/, false /*iscomposable*/);
            IEdmTypeReference int32Type = EdmCoreModel.Instance.GetInt32(isNullable: true);
            function.AddOptionalParameter("minSalary", int32Type);
            function.AddOptionalParameter("maxSalary", int32Type);
            model.AddElement(function);

            var container = new EdmEntityContainer("NS", "Container");
            var me = container.AddFunctionImport(function);
            model.AddElement(container);

            // Act
            var parser = new ODataUriParser(model, new Uri("http://host"), new Uri("http://host/" + functionPath));
            var pathSegments = parser.ParsePath().ToList();

            // Assert
            Assert.Single(pathSegments);
            var operationImportSegment = Assert.IsType<OperationImportSegment>(pathSegments[0]);
            Assert.Equal("UnboundGetMinSalary", operationImportSegment.OperationImports.First().Name);
            Assert.Equal(expectedParameterCount, operationImportSegment.Parameters.Count());
        }

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

        private sealed class RefModelUriResolver : ODataUriResolver
        {
            public override IEdmNavigationSource ResolveNavigationSource(IEdmModel model, string identifier)
            {
                return model.ReferencedModels.Single(m => m.EntityContainer != null).FindDeclaredEntitySet(identifier);
            }
            public override IEdmSchemaType ResolveType(IEdmModel model, string typeName)
            {
                return model.ReferencedModels.Single(m => m.EntityContainer != null).FindType(typeName);
            }
        }

        [Fact]
        public void ParseFullyQualifiedEnumWithResolver()
        {
            var rootModel = new EdmModel();
            rootModel.AddReferencedModel(HardCodedTestModel.TestModel);

            string fullUriString = "http://host/Pet2Set?$filter=PetColorPattern eq Fully.Qualified.Namespace.ColorPattern'Red'";
            ODataUriParser parser = new ODataUriParser(rootModel, ServiceRoot, new Uri(fullUriString));
            parser.Resolver = new RefModelUriResolver();
            var uri = parser.ParseUri();

            Uri resultUri = uri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUriString, Uri.UnescapeDataString(resultUri.OriginalString));
        }

        #region Escape Function Parse
        [Theory]
        [InlineData("/root:/", "/root/NS.NormalFunction(path='')")]
        [InlineData("/root:/abc", "/root/NS.NormalFunction(path='abc')")]
        [InlineData("/root:/photos:February", "/root/NS.NormalFunction(path='photos:February')")]
        [InlineData("/root:/photos/2018/February", "/root/NS.NormalFunction(path='photos%2f2018%2fFebruary')")]
        [InlineData("/root:/photos/2018////February", "/root/NS.NormalFunction(path='photos%2f2018%2f%2f%2f%2fFebruary')")]
        [InlineData("/root:/photos%2F2018%2F/:February", "/root/NS.NormalFunction(path='photos%2F2018%2f%2f:February')")]
        public void ParseNonComposableEscapeFunctionUrlReturnsTheSameODataPath(string escapePathString, string normalPathString)
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: true);

            // Act
            string fullUriString = ServiceRoot + normalPathString;
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var functionPath = parser.ParsePath();

            fullUriString = ServiceRoot + escapePathString;
            parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.True(functionPath.Equals(escapePath));

            Assert.Equal(2, escapePath.Count());
            OperationSegment segment = Assert.IsType<OperationSegment>(escapePath.Last());
            Assert.Equal("NS.NormalFunction", segment.Operations.First().FullName());
        }

        [Theory]
        [InlineData("/root:/:", "/root/NS.ComposableFunction(arg='')")]
        [InlineData("/root:/abc:", "/root/NS.ComposableFunction(arg='abc')")]
        [InlineData("/root:/photos:February:", "/root/NS.ComposableFunction(arg='photos:February')")]
        [InlineData("/root:/photos/2018/February:", "/root/NS.ComposableFunction(arg='photos%2f2018%2fFebruary')")]
        [InlineData("/root:/photos/2018//////February:", "/root/NS.ComposableFunction(arg='photos%2f2018%2f%2f%2f%2f%2f%2fFebruary')")]
        [InlineData("/root:/photos%2F2018%2F/:February:", "/root/NS.ComposableFunction(arg='photos%2F2018%2F%2f:February')")]
        public void ParseComposableEscapeFunctionUrlReturnsTheSameODataPath(string escapePathString, string normalPathString)
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: true);

            // Act
            string fullUriString = ServiceRoot + normalPathString;
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var functionPath = parser.ParsePath();

            fullUriString = ServiceRoot + escapePathString;
            parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.True(functionPath.Equals(escapePath));

            Assert.Equal(2, escapePath.Count());
            OperationSegment segment = Assert.IsType<OperationSegment>(escapePath.Last());
            Assert.Equal("NS.ComposableFunction", segment.Operations.First().FullName());
        }

        [Fact]
        public void ParseComposableEscapeFunctionUrlAndPropertyReturnsTheSameODataPath()
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: true);

            // Act
            string fullUriString = ServiceRoot + "/root/NS.ComposableFunction(arg='photos%2F2018%2F%2f:February')/Name";
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var functionPath = parser.ParsePath();

            fullUriString = ServiceRoot + "/root:/photos%2F2018%2F/:February:/Name";
            parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.True(functionPath.Equals(escapePath));

            Assert.Equal(3, escapePath.Count());
            OperationSegment segment = escapePath.First(c => c is OperationSegment) as OperationSegment;
            Assert.Equal("NS.ComposableFunction", segment.Operations.First().FullName());

            PropertySegment proSegment = Assert.IsType<PropertySegment>(escapePath.Last());
            Assert.Equal("Name", proSegment.Property.Name);
        }

        [Fact]
        public void ParseEscapeFunctionUrlThrowsWithoutEscapeFunction()
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: false);

            // Act
            var fullUriString = ServiceRoot + "/root:/photos/2018/February";
            var parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            Action test = () => parser.ParsePath();

            // Assert
            var odataException = Assert.Throws<ODataException>(test);
            Assert.Equal(ODataErrorStrings.RequestUriProcessor_NoBoundEscapeFunctionSupported("NS.OneDrive"), odataException.Message);
        }

        [Fact]
        public void ParseEscapeFunctionUrlThrowsInvalidEscapeFunction()
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: true, multipleParameter: true);

            // Act
            var fullUriString = ServiceRoot + "/root:/photos/2018/February";
            var parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            Action test = () => parser.ParsePath();

            // Assert
            var odataException = Assert.Throws<ODataException>(test);
            Assert.Equal(ODataErrorStrings.RequestUriProcessor_EscapeFunctionMustHaveOneStringParameter("NS.NormalFunction"), odataException.Message);
        }

        internal static IEdmModel GetEdmModelWithEscapeFunction(bool escape, bool multipleParameter = false)
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "OneDrive");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            entityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));

            var entityTypeRef = new EdmEntityTypeReference(entityType, true);
            EdmFunction nonComFunction = new EdmFunction("NS", "NormalFunction", EdmCoreModel.Instance.GetInt32(true), true, null, false);
            nonComFunction.AddParameter("entity", entityTypeRef);
            nonComFunction.AddParameter("path", EdmCoreModel.Instance.GetString(true));

            if (multipleParameter)
            {
                nonComFunction.AddParameter("path2", EdmCoreModel.Instance.GetString(true));
            }

            model.AddElement(entityType);
            model.AddElement(nonComFunction);

            EdmFunction compFunction = new EdmFunction("NS", "ComposableFunction", entityTypeRef, true, null, true);
            compFunction.AddParameter("entity", entityTypeRef);
            compFunction.AddParameter("arg", EdmCoreModel.Instance.GetString(true));
            model.AddElement(compFunction);

            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            container.AddSingleton("root", entityType);
            model.AddElement(container);

            if (escape)
            {
                IEdmBooleanConstantExpression booleanConstant = new EdmBooleanConstant(true);
                IEdmTerm term = CommunityVocabularyModel.UrlEscapeFunctionTerm;
                foreach (var function in new[] { nonComFunction, compFunction })
                {
                    EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(function, term, booleanConstant);
                    annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
                    model.SetVocabularyAnnotation(annotation);
                }
            }

            return model;
        }

        [Theory]
        [InlineData("/Customers(2):/abc:xyz", "NS.FindOrder")]
        [InlineData("/Customers(2):/abc:xyz:", "NS.FindOrderComposable")]
        public void ParseEscapeFunctionUrlWithAndWithoutColonDelimiterReturnsODataPath(string escapePathString, string function)
        {
            // Arrange
            IEdmModel model = GetCustomerOrderEdmModelWithEscapeFunction();

            // Act
            string fullUriString = ServiceRoot + escapePathString;
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.Equal(3, escapePath.Count());
            OperationSegment segment = escapePath.Last() as OperationSegment;
            Assert.Equal(function, segment.Operations.First().FullName());

            Assert.Equal(1, segment.Parameters.Count());
            var parameter = segment.Parameters.First();
            Assert.Equal("orderName", parameter.Name);
            Assert.Equal("abc:xyz", ((ConstantNode)parameter.Value).Value);
        }

        [Fact]
        public void ParseEscapeFunctionUrlWithPropertyAndEscapeFunctionReturnsCorrectODataPath()
        {
            // Arrange
            IEdmModel model = GetCustomerOrderEdmModelWithEscapeFunction();

            // Act
            string fullUriString = ServiceRoot + "/Customers(2)/MyOrders:/xyz/abc:";
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.Equal(4, escapePath.Count());
            OperationSegment segment = Assert.IsType<OperationSegment>(escapePath.Last());
            Assert.Equal("NS.FindAllOrders", segment.Operations.First().FullName());

            Assert.Equal(1, segment.Parameters.Count());
            var parameter = segment.Parameters.First();
            Assert.Equal("name", parameter.Name);
            Assert.Equal("xyz/abc", ((ConstantNode)parameter.Value).Value);
        }

        [Fact]
        public void ParseEscapeFunctionUrlWithoutEndingDelimiterThrowsWithoutNonComposableFunction()
        {
            // Arrange
            IEdmModel model = GetCustomerOrderEdmModelWithEscapeFunction();

            // Act
            string fullUriString = ServiceRoot + "/Customers(2)/MyOrders:/xyz/abc";
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            Action test = () => parser.ParsePath();

            // Assert
            var odataException = Assert.Throws<ODataException>(test);
            Assert.Equal(ODataErrorStrings.RequestUriProcessor_NoBoundEscapeFunctionSupported("Collection(NS.Order)"), odataException.Message);
        }

        [Theory]
        [InlineData("Customers(2):/abc:/:/xyz")]
        [InlineData("Customers(2):/abc::/xyz")]
        public void ParseEscapeFunctionUrlWithAnotherEscapeFunctionReturnsCorrectODataPath(string escapePathString)
        {
            // Arrange
            IEdmModel model = GetCustomerOrderEdmModelWithEscapeFunction();

            // Act
            string fullUriString = ServiceRoot + "/" + escapePathString;
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.Equal(4, escapePath.Count());
            OperationSegment segment = escapePath.First(p => p is OperationSegment) as OperationSegment;
            Assert.Equal("NS.FindOrderComposable", segment.Operations.First().FullName());

            segment = escapePath.Last(p => p is OperationSegment) as OperationSegment;
            Assert.Equal("NS.CalcCustomers", segment.Operations.First().FullName());
        }

        internal static IEdmModel GetCustomerOrderEdmModelWithEscapeFunction()
        {
            EdmModel model = new EdmModel();

            EdmEntityType orderType = new EdmEntityType("NS", "Order");
            orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            orderType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            model.AddElement(orderType);

            EdmEntityType customerType = new EdmEntityType("NS", "Customer");
            customerType.AddKeys(customerType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(customerType);

            EdmNavigationProperty navOrder = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = orderType,
                Name = "MyOrder",
                TargetMultiplicity = EdmMultiplicity.One
            });

            EdmNavigationProperty navOrders = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = orderType,
                Name = "MyOrders",
                TargetMultiplicity = EdmMultiplicity.Many
            });

            EdmNavigationProperty navCustomer = orderType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = customerType,
                Name = "MyCustomer",
                TargetMultiplicity = EdmMultiplicity.One
            });

            EdmEntityTypeReference orderTypeRef = new EdmEntityTypeReference(orderType, false);
            EdmEntityTypeReference customerTypeRef = new EdmEntityTypeReference(customerType, false);

            EdmFunction orderFunction = new EdmFunction("NS", "CalcCustomers", customerTypeRef, true, null, false);
            orderFunction.AddParameter("bindingParameter", orderTypeRef);
            orderFunction.AddParameter("path", EdmCoreModel.Instance.GetString(true));
            model.AddElement(orderFunction);

            EdmFunction singleNonComposableFunction = new EdmFunction("NS", "FindOrder", orderTypeRef, true, null, false);
            singleNonComposableFunction.AddParameter("bindingParameter", customerTypeRef);
            singleNonComposableFunction.AddParameter("orderName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(singleNonComposableFunction);

            EdmFunction singleComposableFunction = new EdmFunction("NS", "FindOrderComposable", orderTypeRef, true, null, true);
            singleComposableFunction.AddParameter("bindingParameter", customerTypeRef);
            singleComposableFunction.AddParameter("orderName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(singleComposableFunction);

            var multipleFunction = new EdmFunction("NS", "FindAllOrders", new EdmCollectionTypeReference(new EdmCollectionType(orderTypeRef)), true, null, true);
            multipleFunction.AddParameter("bindingParameter", new EdmCollectionTypeReference(new EdmCollectionType(orderTypeRef)));
            multipleFunction.AddParameter("name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(multipleFunction);

            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmEntitySet customers = container.AddEntitySet("Customers", customerType);
            EdmEntitySet orders = container.AddEntitySet("Orders", orderType);
            customers.AddNavigationTarget(navOrder, orders);
            customers.AddNavigationTarget(navOrders, orders);
            model.AddElement(container);

            IEdmBooleanConstantExpression booleanConstant = new EdmBooleanConstant(true);
            IEdmTerm term = CommunityVocabularyModel.UrlEscapeFunctionTerm;
            foreach(var function in new[] { orderFunction, singleNonComposableFunction, singleComposableFunction, multipleFunction })
            {
                EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(function, term, booleanConstant);
                annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
                model.SetVocabularyAnnotation(annotation);
            }

            return model;
        }
        #endregion
    }
}