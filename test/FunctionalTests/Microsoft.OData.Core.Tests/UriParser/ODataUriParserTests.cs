//---------------------------------------------------------------------
// <copyright file="ODataUriParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            Assert.Single(path);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.Null(uriParser.ParseFilter());
            Assert.Null(uriParser.ParseSelectAndExpand());
            Assert.Null(uriParser.ParseOrderBy());
            Assert.Null(uriParser.ParseTop());
            Assert.Null(uriParser.ParseSkip());
            Assert.Null(uriParser.ParseCount());
            Assert.Null(uriParser.ParseSearch());
            Assert.Null(uriParser.ParseSkipToken());
            Assert.Null(uriParser.ParseDeltaToken());
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
            Assert.Single(path);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.Null(uriParser.ParseFilter());
            var results = uriParser.ParseSelectAndExpand();
            Assert.True(results.AllSelected);
            Assert.Empty(results.SelectedItems);
            Assert.Null(uriParser.ParseOrderBy());
            Action action = () => uriParser.ParseTop();
            action.Throws<ODataException>(Strings.SyntacticTree_InvalidTopQueryOptionValue(""));
            action = () => uriParser.ParseSkip();
            action.Throws<ODataException>(Strings.SyntacticTree_InvalidSkipQueryOptionValue(""));
            action = () => uriParser.ParseIndex();
            action.Throws<ODataException>(Strings.SyntacticTree_InvalidIndexQueryOptionValue(""));
            action = () => uriParser.ParseCount();
            action.Throws<ODataException>(Strings.ODataUriParser_InvalidCount(""));
            action = () => uriParser.ParseSearch();
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(0, ""));
            Assert.Empty(uriParser.ParseSkipToken());
            Assert.Empty(uriParser.ParseDeltaToken());
        }

        [Fact]
        public void ParseAnnotationInFilterForOpenTypeShouldWork()
        {
            Uri entitySetUri = new Uri("http://host/Paintings");
            var filterClauseString = "?filter=@my.annotation eq 5";

            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(entitySetUri, filterClauseString));
            uriParser.EnableNoDollarQueryOptions = true;
            var path = uriParser.ParsePath();
            Assert.Single(path);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPaintingsSet());
            var filterResult = uriParser.ParseFilter();
            Assert.NotNull(filterResult);
            Assert.Equal(QueryNodeKind.BinaryOperator, filterResult.Expression.Kind);
            var bon = Assert.IsType<BinaryOperatorNode>(filterResult.Expression);
            Assert.NotNull(bon.Left);
            Assert.NotNull(bon.Right);

            var selectExpandResult = uriParser.ParseSelectAndExpand();
            Assert.Null(selectExpandResult);
        }

        [Fact]
        public void ParseAnnotationInFilterForEntityTypeShouldThrow()
        {
            var filterClauseString = "?filter=@my.annotation eq 5";

            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(FullUri, filterClauseString));
            uriParser.EnableNoDollarQueryOptions = true;
            var path = uriParser.ParsePath();
            Assert.Single(path);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Action action  = () => uriParser.ParseFilter();
            action.Throws<ODataException>(
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

                action.Throws<ODataException>(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce("$filter"));
                Assert.Equal(2, nonODataqueryOptions.Count);
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
            Assert.Throws<ArgumentNullException>("model", createWithNullModel);
        }

        [Fact]
        public void ModelIsSetCorrectly()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri);
            Assert.Same(HardCodedTestModel.TestModel, parser.Model);
        }

        [Fact]
        public void ServiceRootUriIsSet()
        {
            var serviceRoot = new Uri("http://example.com/Foo/");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, FullUri);
            Assert.Equal(serviceRoot, parser.ServiceRoot);
        }

        [Fact]
        public void ServiceRootMustBeAbsoluteUri()
        {
            var serviceRoot = new Uri("one/two/three", UriKind.Relative);
            Action create = () => new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, new Uri("test", UriKind.Relative));
            Assert.Throws<ODataException>(create);

            serviceRoot = new Uri("one/two/three", UriKind.RelativeOrAbsolute);
            create = () => new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, new Uri("test", UriKind.Relative));
            Assert.Throws<ODataException>(create);
        }

        [Fact]
        public void MaxExpandDepthCannotBeNegative()
        {
            Action setNegative = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Settings.MaximumExpansionDepth = -1;
            setNegative.Throws<ODataException>(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void MaxExpandCountCannotBeNegative()
        {
            Action setNegative = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Settings.MaximumExpansionCount = -1;
            setNegative.Throws<ODataException>(ODataErrorStrings.UriParser_NegativeLimit);
        }
        #endregion

        #region Parser limit config tests
        [Fact]
        public void FilterLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { FilterLimit = 3 } };
            Assert.Equal(3, parser.Settings.FilterLimit);
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
            parseWithLimit.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
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
            parseWithLimit.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void NegativeFilterLimitThrows()
        {
            Action negativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { FilterLimit = -98798 } };
            negativeLimit.Throws<ODataException>(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void OrderbyLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { OrderByLimit = 3 } };
            Assert.Equal(3, parser.Settings.OrderByLimit);
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
            parseWithLimit.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
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
            parseWithLimit.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void OrderByLimitCannotBeNegative()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { OrderByLimit = -9879 } };
            parseWithNegativeLimit.Throws<ODataException>(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void PathLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { PathLimit = 3 } };
            Assert.Equal(3, parser.Settings.PathLimit);
        }

        [Fact]
        public void PathLimitIsRespectedForPath()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/", UriKind.Absolute), new Uri("http://gobbldygook/path/to/something", UriKind.Absolute)) { Settings = { PathLimit = 0 } };
            Action parseWithLimit = () => parser.ParsePath();
            parseWithLimit.Throws<ODataException>(ODataErrorStrings.UriQueryPathParser_TooManySegments);
        }

        [Fact]
        public void PathLimitCannotBeNegative()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { PathLimit = -8768 } };
            parseWithNegativeLimit.Throws<ODataException>(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [Fact]
        public void SelectExpandLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { SelectExpandLimit = 3 } };
            Assert.Equal(3, parser.Settings.SelectExpandLimit);
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
            parseWithLimit.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void NegativeSelectExpandLimitIsRespected()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { SelectExpandLimit = -87657 } };
            parseWithNegativeLimit.Throws<ODataException>(ODataErrorStrings.UriParser_NegativeLimit);
        }
        #endregion

        #region Default value tests
        [Fact]
        public void DefaultKeyDelimiterShouldBeSlash()
        {
            Assert.Same(ODataUrlKeyDelimiter.Slash, new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlKeyDelimiter);
        }

        [Fact]
        public void ODataUrlKeyDelimiterCannotBeSetToNull()
        {
            Action setToNull = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlKeyDelimiter = null;
            Assert.Throws<ArgumentNullException>("UrlKeyDelimiter", setToNull);
        }

        [Fact]
        public void DefaultEnableTemplateParsingShouldBeFalse()
        {
            Assert.False(new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).EnableUriTemplateParsing);
        }

        [Fact]
        public void DefaultEnableCaseInsensitiveBuiltinIdentifierShouldBeFalse()
        {
            Assert.False(new ODataUriResolver().EnableCaseInsensitive);
        }

        [Fact]
        public void DefaultParameterAliasNodesShouldBeEmtpy()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People"));
            Assert.Empty(uriParser.ParameterAliasNodes);
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
            Assert.NotNull(expandedNavigationSelectItem);
            Assert.True((expandedNavigationSelectItem.NavigationSource is IEdmContainedEntitySet));
        }

        #region Relative full path smoke test
        [Fact]
        public void AbsoluteUriInConstructorShouldThrow()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host/People(1)"));
            action.Throws<ODataException>(Strings.UriParser_RelativeUriMustBeRelative);
        }

        [Fact]
        public void AlternateKeyShouldWork()
        {
            ODataPath pathSegment = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(SocialSN = \'1\')"))
            {
                Resolver = new AlternateKeysODataUriResolver(HardCodedTestModel.TestModel)
            }.ParsePath();

            Assert.Equal(2, pathSegment.Count);
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

            Assert.Equal(2, pathSegment.Count);
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

            action.Throws<ODataException>("Bad Request - Error in query syntax.");
        }

        [Fact]
        public void CompositeAlternateKeyShouldFailOnlyWithInvalidAlternateKey()
        {
            Uri fullUri = new Uri("http://host/People(NameAlias='anyName', FirstNameAlias='anyFirst', extraAltKey='any')");
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), fullUri)
            {
                Resolver = new AlternateKeysODataUriResolver(HardCodedTestModel.TestModel)
            }.ParsePath();

            action.Throws<ODataException>(ODataErrorStrings.BadRequest_KeyCountMismatch(HardCodedTestModel.GetPersonType().FullTypeName()));
        }

        [Fact]
        public void AlternateKeyShouldFailWithDefaultUriResolver()
        {
            Uri fullUri = new Uri("http://host/People(SocialSN = \'1\')");
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), fullUri)
            {
                Resolver = new ODataUriResolver()
            }.ParsePath();

            action.Throws<ODataException>("Bad Request - Error in query syntax.");
        }

        [Fact]
        public void ParsePathShouldWork()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People", UriKind.Relative)).ParsePath();
            Assert.Single(path);
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
            Assert.NotNull(parser.ParseSelectAndExpand());
            Assert.NotNull(parser.ParseFilter());
            Assert.NotNull(parser.ParseOrderBy());
            Assert.Equal(1, parser.ParseTop());
            Assert.Equal(2, parser.ParseSkip());
            Assert.True(parser.ParseCount());
            Assert.NotNull(parser.ParseSearch());
            Assert.Equal("abc", parser.ParseSkipToken());
            Assert.Equal("def", parser.ParseDeltaToken());
        }

        [Theory]
        [InlineData("People(1)/RelatedIDs?index=42", true)]
        [InlineData("People(1)/RelatedIDs?$index=42", true)]
        [InlineData("People(1)/RelatedIDs?$index=42", false)]
        public void ParseIndexQueryOptionShouldWork(string relativeUriString, bool enableNoDollarQueryOptions)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
            Assert.Equal(42, parser.ParseIndex());
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
            Assert.NotNull(selectExpand);
            PathSelectItem selectItem = selectExpand.SelectedItems.First() as PathSelectItem;
            Assert.NotNull(selectItem);
            AnnotationSegment annotationSegment = selectItem.SelectedPath.FirstSegment as AnnotationSegment;
            Assert.NotNull(annotationSegment);
            Assert.Equal(termName, annotationSegment.Term.FullName());
            Assert.Equal(typeName, annotationSegment.Term.Type.FullName());
        }

        [Theory]
        [InlineData("People?$select=@odata.type", "@odata.type")]
        [InlineData("People?$select=@odata.unknown", "@odata.unknown")]
        public void ParseSelectODataControlInformationShouldFail(string relativeUriString, string term)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            Action action = () => parser.ParseSelectAndExpand();
            action.Throws<ODataException>(ODataErrorStrings.UriSelectParser_TermIsNotValid(term));
        }

        [Fact]
        public void ParseSelectPropertyAnnotationShouldWork()
        {
            string relativeUriString = "People?$select=Name/@Fully.Qualified.Namespace.PrimitiveTerm";
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            var selectExpand = parser.ParseSelectAndExpand();
            Assert.NotNull(selectExpand);
            PathSelectItem selectItem = selectExpand.SelectedItems.First() as PathSelectItem;
            Assert.NotNull(selectItem);
            List<ODataPathSegment> segments = selectItem.SelectedPath.ToList();
            Assert.Equal(2, segments.Count);
            PropertySegment propertySegment = segments[0] as PropertySegment;
            Assert.Equal("Name", propertySegment.Property.Name);
            AnnotationSegment annotationSegment = segments[1] as AnnotationSegment;
            Assert.Equal("Fully.Qualified.Namespace.PrimitiveTerm", annotationSegment.Term.FullName());
            Assert.Equal(EdmTypeKind.Primitive, annotationSegment.Term.Type.TypeKind());
        }

        [Fact]
        public void ParseSelectComplexAnnotationWithPathShouldWork()
        {
            string relativeUriString = "People?$select=@Fully.Qualified.Namespace.ComplexTerm/Street";
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            var selectExpand = parser.ParseSelectAndExpand();
            Assert.NotNull(selectExpand);
            PathSelectItem selectItem = selectExpand.SelectedItems.First() as PathSelectItem;
            List<ODataPathSegment> segments = selectItem.SelectedPath.ToList();
            Assert.Equal(2, segments.Count);
            AnnotationSegment annotationSegment = segments[0] as AnnotationSegment;
            Assert.Equal("Fully.Qualified.Namespace.ComplexTerm", annotationSegment.Term.FullName());
            Assert.Equal("Fully.Qualified.Namespace.Address", annotationSegment.Term.Type.FullName());
            PropertySegment propertySegment = segments[1] as PropertySegment;
            Assert.Equal("Street", propertySegment.Property.Name);
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

                Assert.Null(parser.ParseFilter());
                Assert.Null(parser.ParseSelectAndExpand());
                Assert.Null(parser.ParseOrderBy());
                Assert.Null(parser.ParseTop());
                Assert.Null(parser.ParseSkip());
                Assert.Null(parser.ParseCount());
                Assert.Null(parser.ParseSearch());
                Assert.Null(parser.ParseSkipToken());
                Assert.Null(parser.ParseDeltaToken());
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
                action.Throws<ODataException>(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(
                    enableNoDollarQueryOptions ? string.Format(CultureInfo.InvariantCulture, "${0}/{0}", queryOptionName) : queryOptionName));
            }
            else
            {
                action.DoesNotThrow();
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
            Assert.NotNull(parser.ParseSelectAndExpand());
        }

        [Theory]
        [InlineData("People?deltatoken=Start@Next_Chunk:From%26$To=Here!?()*+%2B,1-._~;", true)]
        [InlineData("People?$deltatoken=Start@Next_Chunk:From%26$To=Here!?()*+%2B,1-._~;", true)]
        [InlineData("People?$deltatoken=Start@Next_Chunk:From%26$To=Here!?()*+%2B,1-._~;", false)]
        public void ParseDeltaTokenWithKindsofCharactorsShouldWork(string relativeUriString, bool enableNoDollarQueryOptions)
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(relativeUriString, UriKind.Relative));
            parser.EnableNoDollarQueryOptions = enableNoDollarQueryOptions;
            Assert.Equal("Start@Next_Chunk:From&$To=Here!?()* +,1-._~;", parser.ParseDeltaToken());
        }

        #endregion

        #region ODataUnrecognizedPathException tests
        [Fact]
        public void ParsePathExceptionDataTestWithInvalidEntitySet()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People1(1)/MyDog/Color", UriKind.Relative)).ParsePath();
            ODataUnrecognizedPathException ex = Assert.Throws<ODataUnrecognizedPathException>(action);
            Assert.Empty(ex.ParsedSegments);
            Assert.Equal("People1(1)", ex.CurrentSegment);
            Assert.Equal(2, ex.UnparsedSegments.Count());
        }

        [Fact]
        public void ParsePathExceptionDataTestWithInvalidContainedNavigationProperty()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/MyDog1/Color", UriKind.Relative)).ParsePath();
            ODataUnrecognizedPathException ex = Assert.Throws<ODataUnrecognizedPathException>(action);
            Assert.Equal(2, ex.ParsedSegments.Count());
            Assert.Equal("MyDog1", ex.CurrentSegment);
            Assert.Single(ex.UnparsedSegments);
        }

        [Fact]
        public void ParsePathExceptionDataTestInvalidNavigationProperty()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/MyDog/Color1", UriKind.Relative));
            Action action = () => parser.ParsePath();
            ODataUnrecognizedPathException ex = Assert.Throws<ODataUnrecognizedPathException>(action);
            Assert.Equal(3, ex.ParsedSegments.Count());
            Assert.Equal("Color1", ex.CurrentSegment);
            Assert.Empty(ex.UnparsedSegments);
            Assert.Empty(parser.ParameterAliasNodes);
        }

        [Fact]
        public void ParsePathExceptionDataTestWithAlias()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("GetCoolestPersonWithStyle(styleID=@p1)/UndeclaredProperty?@p1=32", UriKind.Relative));
            Action action = () => parser.ParsePath();
            ODataUnrecognizedPathException ex = Assert.Throws<ODataUnrecognizedPathException>(action);
            Assert.Single(ex.ParsedSegments);
            Assert.Equal("UndeclaredProperty", ex.CurrentSegment);
            Assert.Empty(ex.UnparsedSegments);

            var aliasNode = parser.ParameterAliasNodes;
            Assert.Single(aliasNode);
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
            Assert.Equal(2, pathSegmentList.Count);
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
            action.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_AsExpected(8, "nonsense"));
        }

        [Fact]
        public void ParseBadComputeWithMissingAlias()
        {
            Uri url = new Uri("http://host/Paintings?$compute=nonsense as");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, url);
            Action action = () => parser.ParseCompute();
#if NETCOREAPP3_1
            action.Throws<ArgumentNullException>("Value cannot be null or empty. (Parameter 'alias')");
#else
             action.Throws<ArgumentNullException>("Value cannot be null or empty.\r\nParameter name: alias");
#endif
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
            Assert.Equal(2, items.Count());
            Assert.Equal("Property1AsString", items[0].Alias);
            items[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            Assert.True(items[0].Expression.TypeReference.IsEquivalentTo(typeReference));
            Assert.Equal("Property1Lower", items[1].Alias);
            items[1].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            Assert.Equal("Edm.String", items[1].Expression.TypeReference.FullName());
            Assert.True(items[1].Expression.TypeReference.IsNullable); // tolower is built in function that allows nulls.

            ComputeExpression copy = new ComputeExpression(items[0].Expression, items[0].Alias, null);
            Assert.NotNull(copy.Expression);
            Assert.Null(copy.TypeReference);
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
            Assert.Single(items);
            ExpandedNavigationSelectItem expanded = items[0] as ExpandedNavigationSelectItem;
            List<ComputeExpression> computes = expanded.ComputeOption.ComputedItems.ToList();
            Assert.Single(computes);
            Assert.Equal("NavProperty1AsString", computes[0].Alias);
            computes[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            Assert.True(computes[0].Expression.TypeReference.IsEquivalentTo(typeReference));
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
            Assert.Equal(2, items.Count());
            Assert.Equal("Property1AsString", items[0].Alias);
            items[0].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            Assert.True(items[0].Expression.TypeReference.IsEquivalentTo(typeReference));
            Assert.Equal("Property1Lower", items[1].Alias);
            items[1].Expression.ShouldBeSingleValueFunctionCallQueryNode();
            Assert.Equal("Edm.String", items[1].Expression.TypeReference.FullName());
            Assert.True(items[1].Expression.TypeReference.IsNullable); // tolower is built in function that allows nulls.

            // validate level 1 expand compute
            List<SelectItem> selectItems = selectClause.SelectedItems.ToList();
            Assert.Single(selectItems);
            ExpandedNavigationSelectItem expanded = selectItems[0] as ExpandedNavigationSelectItem;
            var compute = Assert.Single(expanded.ComputeOption.ComputedItems);
            Assert.Equal("NavProperty1AsString", compute.Alias);
            compute.Expression.ShouldBeSingleValueFunctionCallQueryNode();
            Assert.True(compute.Expression.TypeReference.IsEquivalentTo(typeReference));

            // validate level 2 expand compute
            var subSelectItem = Assert.Single(expanded.SelectAndExpand.SelectedItems);
            ExpandedNavigationSelectItem subExpanded = Assert.IsType<ExpandedNavigationSelectItem>(subSelectItem);
            var subCompute = Assert.Single(subExpanded.ComputeOption.ComputedItems);
            Assert.Equal("SubNavProperty1AsString", subCompute.Alias);
            subCompute.Expression.ShouldBeSingleValueFunctionCallQueryNode();
            Assert.True(subCompute.Expression.TypeReference.IsEquivalentTo(typeReference));
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
        [InlineData("/root/items/32:/", "/root/items/32/NS.NormalFunction(path='')")]
        [InlineData("/root/items/32:/abc", "/root/items/32/NS.NormalFunction(path='abc')")]
        [InlineData("/root/items/32:/photos:February", "/root/items/32/NS.NormalFunction(path='photos:February')")]
        [InlineData("/root/items/32:/photos/2018/February", "/root/items/32/NS.NormalFunction(path='photos%2f2018%2fFebruary')")]
        [InlineData("/root/items/32:/photos/2018////February", "/root/items/32/NS.NormalFunction(path='photos%2f2018%2f%2f%2f%2fFebruary')")]
        [InlineData("/root/items/32:/photos%2F2018%2F/:February", "/root/items/32/NS.NormalFunction(path='photos%2F2018%2f%2f:February')")]
        public void ParseNonComposableEscapeFunctionUrlReturnsTheSameODataPathForContainedNavigation(string escapePathString, string normalPathString)
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

            Assert.Equal(4, escapePath.Count());
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

        [Theory]
        [InlineData("/root/items/32:/:", "/root/items/32/NS.ComposableFunction(arg='')")]
        [InlineData("/root/items/32:/abc:", "/root/items/32/NS.ComposableFunction(arg='abc')")]
        [InlineData("/root/items/32:/photos:February:", "/root/items/32/NS.ComposableFunction(arg='photos:February')")]
        [InlineData("/root/items/32:/photos/2018/February:", "/root/items/32/NS.ComposableFunction(arg='photos%2f2018%2fFebruary')")]
        [InlineData("/root/items/32:/photos/2018//////February:", "/root/items/32/NS.ComposableFunction(arg='photos%2f2018%2f%2f%2f%2f%2f%2fFebruary')")]
        [InlineData("/root/items/32:/photos%2F2018%2F/:February:", "/root/items/32/NS.ComposableFunction(arg='photos%2F2018%2F%2f:February')")]
        public void ParseComposableEscapeFunctionUrlReturnsTheSameODataPathForContainedNavigation(string escapePathString, string normalPathString)
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

            Assert.Equal(4, escapePath.Count());
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

        [Theory]
        [InlineData("/entitySetEscaped/32:/photos%2F2018%2F%2fFebruary:/Name", "/entitySetEscaped/32/NS.ComposableFunction(arg='photos%2F2018%2F%2fFebruary')/Name", true)]
        [InlineData("/entitySetEscaped/32:/:/Name", "/entitySetEscaped/32/NS.ComposableFunction(arg='')/Name", true)]
        [InlineData("/entitySetEscaped/32/:/:/Name", "/entitySetEscaped/32/NS.ComposableFunction(arg='')/Name", true)]
        [InlineData("/entitySetEscaped/32:/", "/entitySetEscaped/32/NS.NormalFunction(path='')", false)]
        [InlineData("/entitySetEscaped/32/:/", "/entitySetEscaped/32/NS.NormalFunction(path='')", false)]
        [InlineData("/entitySetEscaped/32/:", "/entitySetEscaped/32/NS.NormalFunction(path='')", false)]
        [InlineData("/entitySetEscaped/32:/photos%2F2018%2F%2fFebruary", "/entitySetEscaped/32/NS.NormalFunction(path='photos%2F2018%2F%2fFebruary')", false)]
        public void ParseEscapeFunctionWithColonInKeyValue(string escapeFunctionUri, string functionUri, bool isComposable)
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: true);

            // Act
            string fullUriString = ServiceRoot + functionUri;
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var functionPath = parser.ParsePath();

            fullUriString = ServiceRoot + escapeFunctionUri;
            parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.True(functionPath.Equals(escapePath));
            Assert.Equal(functionPath.FirstOrDefault(s => s is KeySegment).Identifier, escapePath.FirstOrDefault(s => s is KeySegment).Identifier);
            OperationSegment segment = escapePath.First(c => c is OperationSegment) as OperationSegment;
            Assert.Equal(isComposable ? "NS.ComposableFunction": "NS.NormalFunction", segment.Operations.First().FullName());

            if (isComposable)
            {
                PropertySegment proSegment = Assert.IsType<PropertySegment>(escapePath.Last());
                Assert.Equal("Name", proSegment.Property.Name);
            }
        }

        [Theory]
        [InlineData("/entitySetEscaped/32:/Name", "/entitySetEscaped/32:/Name")]
        [InlineData("/entitySetEscaped/:32:/Name", "/entitySetEscaped/:32:/Name")]
        public void ParseKeyValueWithColonWithoutEscapeFunction(string escapeFunctionUri, string functionUri)
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: false);

            // Act
            string fullUriString = ServiceRoot + functionUri;
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var functionPath = parser.ParsePath();

            fullUriString = ServiceRoot + escapeFunctionUri;
            parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.True(functionPath.Equals(escapePath));
            Assert.IsType<KeySegment>(escapePath.ElementAt(1));
            PropertySegment proSegment = Assert.IsType<PropertySegment>(escapePath.Last());       
            Assert.Equal("Name", proSegment.Property.Name);
        }

        [Theory]
        [InlineData("/entitySetEscaped/32/NS.SpecialDrive:/ComposableParameter:", "/entitySetEscaped/32/NS.SpecialDrive/NS.SpecialOrders(path ='ComposableParameter')")]
        [InlineData("/entitySetEscaped/32/:/ComposableParameter:", "/entitySetEscaped/32/NS.ComposableFunction(arg='ComposableParameter')")]
        [InlineData("/entitySetEscaped/32:/ComposableParameter:", "/entitySetEscaped/32/NS.ComposableFunction(arg='ComposableParameter')")]
        [InlineData("/entitySetEscaped/32:/NonComposableParameter", "/entitySetEscaped/32/NS.NormalFunction(path='NonComposableParameter')")]
        [InlineData("/entitySetEscaped/32/NS.SpecialDrive:/ComposableParameter::/nestedComposableParameter:", "/entitySetEscaped/32/NS.SpecialDrive/NS.SpecialOrders(path ='ComposableParameter')/NS.SpecialOrders(path ='nestedComposableParameter')")]
        [InlineData("/vsDrive/32:/ComposableParameter::/nestedComposableParameter:", "/vsDrive/32/NS.SpecialOrders(path ='ComposableParameter')/NS.SpecialOrders(path ='nestedComposableParameter')")]
        [InlineData("/vsDrive/32/NS.OneDrive:/ComposableParameter:", "/vsDrive/32/NS.OneDrive/NS.ComposableFunction(arg='ComposableParameter')")]
        [InlineData("/vsDrive/32:/NonComposableParameter", "/vsDrive/32/NS.NormalFunction(path='NonComposableParameter')")]
        public void ParseEscapeFunctionWithInheritance(string escapeFunctionUri, string functionUri)
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: true);

            // Act
            string fullUriString = ServiceRoot + functionUri;
            ODataUriParser parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var functionPath = parser.ParsePath();

            fullUriString = ServiceRoot + escapeFunctionUri;
            parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));
            var escapePath = parser.ParsePath();

            // Assert
            Assert.True(functionPath.Equals(escapePath));
            Assert.True(functionPath.LastSegment.Equals(escapePath.LastSegment));
            if (functionPath.LastSegment is OperationSegment)
            {
                OperationSegment functionSegment = functionPath.LastSegment as OperationSegment;
                OperationSegment escapeUriSegment = escapePath.LastSegment as OperationSegment;

                Assert.True(functionSegment.Parameters.First().Name == escapeUriSegment.Parameters.First().Name);
                Assert.True(((ConstantNode)functionSegment.Parameters.First().Value).LiteralText == ((ConstantNode)escapeUriSegment.Parameters.First().Value).LiteralText);
            }
        }

        [Theory]
        [InlineData("/entitySetEscaped::/32:/NonComposableParameter", typeof(ODataUnrecognizedPathException))]
        [InlineData("/entitySetEscaped('32')/NS.SpecialDrive::/ComposableParameter::/nestedNonComposableParameter:", typeof(ODataUnrecognizedPathException))]
        [InlineData("/entitySetEscaped(32)/:NS.SpecialDrive:/ComposableParameter::/nestedNonComposableParameter:", typeof(ODataException))]
        [InlineData("/entitySetEscaped('32')::/NS.SpecialDrive:/ComposableParameter::/nestedNonComposableParameter:", typeof(ODataException))]
        public void ParseInvalidEscapeURIShouldThrow(string escapeFunctionUri, Type exceptionType)
        {
            // Arrange
            IEdmModel model = GetEdmModelWithEscapeFunction(escape: true);

            // Act
            string fullUriString = ServiceRoot + escapeFunctionUri;
            var parser = new ODataUriParser(model, ServiceRoot, new Uri(fullUriString));

            // Assert
            Action test = () => parser.ParsePath();

            // Assert
            Assert.Throws(exceptionType, test);
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
            var odataException = Assert.Throws<ODataUnrecognizedPathException>(test);
            Assert.Equal(ODataErrorStrings.RequestUriProcessor_ResourceNotFound("root:"), odataException.Message);
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
            Assert.Equal(ODataErrorStrings.RequestUriProcessor_NoBoundEscapeFunctionSupported("NS.OneDrive"), odataException.Message);
        }

        internal static IEdmModel GetEdmModelWithEscapeFunction(bool escape, bool multipleParameter = false)
        {
            EdmModel model = new EdmModel();

            EdmEntityType itemEntityType = new EdmEntityType("NS", "Item");
            itemEntityType.AddKeys(itemEntityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(false)));
            itemEntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));

            EdmEntityType driveEntityType = new EdmEntityType("NS", "OneDrive");
            driveEntityType.AddKeys(driveEntityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(false)));
            driveEntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            driveEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    ContainsTarget = true,
                    Name = "items",
                    Target = itemEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            EdmEntityType derivedType = new EdmEntityType("NS", "SpecialDrive", driveEntityType);
            derivedType.AddStructuralProperty("SpecialName", EdmCoreModel.Instance.GetString(false));

            EdmEntityType verySpecialDrive = new EdmEntityType("NS", "VerySpecialDrive", derivedType);
            derivedType.AddStructuralProperty("VerySpecialName", EdmCoreModel.Instance.GetString(false));

            var derivedTypeRef = new EdmEntityTypeReference(derivedType, true);
            var itemEntityTypeRef = new EdmEntityTypeReference(itemEntityType, true);
            var driveEntityTypeRef = new EdmEntityTypeReference(driveEntityType, true);

            EdmFunction driveNonComFunction = new EdmFunction("NS", "NormalFunction", EdmCoreModel.Instance.GetInt32(true), true, null, false);
            driveNonComFunction.AddParameter("entity", driveEntityTypeRef);
            driveNonComFunction.AddParameter("path", EdmCoreModel.Instance.GetString(true));

            if (multipleParameter)
            {
                driveNonComFunction.AddParameter("path2", EdmCoreModel.Instance.GetString(true));
            }

            model.AddElement(itemEntityType);
            model.AddElement(driveEntityType);
            model.AddElement(driveNonComFunction);
            model.AddElement(derivedType);
            model.AddElement(verySpecialDrive);

            EdmFunction itemCompFunction = new EdmFunction("NS", "ComposableFunction", itemEntityTypeRef, true, null, true);
            itemCompFunction.AddParameter("entity", itemEntityTypeRef);
            itemCompFunction.AddParameter("arg", EdmCoreModel.Instance.GetString(true));
            model.AddElement(itemCompFunction);

            // Non-composable COULD return item metadata, but does not have to. For example, in the case where "item" represents a file
            // it could return the binary content of the file. We therefore choose a different return type to ensure we validate this.
            EdmFunction itemNonComFunction = new EdmFunction("NS", "NormalFunction", EdmCoreModel.Instance.GetInt32(true), true, null, false);
            itemNonComFunction.AddParameter("entity", itemEntityTypeRef);
            itemNonComFunction.AddParameter("path", EdmCoreModel.Instance.GetString(true));
            model.AddElement(itemNonComFunction);

            EdmFunction driveCompFunction = new EdmFunction("NS", "ComposableFunction", driveEntityTypeRef, true, null, true);
            driveCompFunction.AddParameter("entity", driveEntityTypeRef);
            driveCompFunction.AddParameter("arg", EdmCoreModel.Instance.GetString(true));
            model.AddElement(driveCompFunction);

            EdmFunction driveSpecialCompFunction = new EdmFunction("NS", "SpecialOrders", derivedTypeRef, true, null, true);
            driveSpecialCompFunction.AddParameter("dervidEntity", derivedTypeRef);
            driveSpecialCompFunction.AddParameter("path", EdmCoreModel.Instance.GetString(true));
            model.AddElement(driveSpecialCompFunction);

            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            container.AddSingleton("root", driveEntityType);
            container.AddEntitySet("entitySetEscaped", driveEntityType);
            container.AddEntitySet("specialDrive", derivedType);
            container.AddEntitySet("vsDrive", verySpecialDrive);

            model.AddElement(container);

            if (escape)
            {
                IEdmBooleanConstantExpression booleanConstant = new EdmBooleanConstant(true);
                IEdmTerm term = CommunityVocabularyModel.UrlEscapeFunctionTerm;
                foreach (var function in new[] { itemNonComFunction, itemCompFunction, driveNonComFunction, driveCompFunction, driveSpecialCompFunction })
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

            var parameter = Assert.Single(segment.Parameters);
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

            var parameter = Assert.Single(segment.Parameters);
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