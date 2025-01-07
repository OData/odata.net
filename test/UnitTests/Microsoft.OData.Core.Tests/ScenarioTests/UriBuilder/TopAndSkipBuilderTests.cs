//---------------------------------------------------------------------
// <copyright file="TopAndSkipBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriBuilder
{
    public class TopAndSkipBuilderTests : UriBuilderTestBase
    {
        #region $top option
        [Fact]
        public void PositiveTopValueWorks()
        {
            Uri queryUri = new Uri("People?$top=5", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$top=5"), actualUri);
        }

        [Fact]
        public void ZeroTopValueWorks()
        {
            Uri queryUri = new Uri("People?$top= 0  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$top=0"), actualUri);
        }
        #endregion $top option

        #region $skip option
        [Fact]
        public void PositiveSkipValueWorks()
        {
            Uri queryUri = new Uri("People?$skip=5", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$skip=5"), actualUri);
        }

        [Fact]
        public void ZeroSkipValueWorks()
        {
            Uri queryUri = new Uri("People?$skip= 0  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$skip=0"), actualUri);
        }
        #endregion $skip option

        [Theory]
        [InlineData(42)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-998)]
        public void BuildUrlWithIndexValueWorks(int value)
        {
            string expect = "http://gobbledygook/People(1)/RelatedIDs?$index=" + value;
            string uriString = "People(1)/RelatedIDs?$index=" + value;
            Uri queryUri = new Uri(uriString, UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri(expect), actualUri);
        }

        #region new ODataUri
        [Fact]
        public void BuildUrlWithNewODataUri()
        {
            ODataUri uri = new ODataUri();
            uri.ServiceRoot = new Uri("http://gobbledygook/");
            uri.Skip = 4;
            uri.Top = 5;
            uri.Path = new ODataPath(new EntitySetSegment(HardCodedTestModel.GetPeopleSet()));
            Assert.Empty(uri.ParameterAliasNodes);

            Uri res = uri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(new Uri("http://gobbledygook/People?$top=5&$skip=4"), res);
        }

        [Fact]
        public void BuildUrlWithSkipTokenODataUri()
        {
            ODataUri uri = new ODataUri();
            uri.ServiceRoot = new Uri("http://gobbledygook/");
            uri.SkipToken = "MyToken";
            uri.Top = 5;
            uri.Path = new ODataPath(new EntitySetSegment(HardCodedTestModel.GetPeopleSet()));
            Assert.Empty(uri.ParameterAliasNodes);

            Uri res = uri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(new Uri("http://gobbledygook/People?$top=5&$skiptoken=MyToken"), res);
        }

        [Fact]
        public void BuildUrlWithDeltaTokenODataUri()
        {
            ODataUri uri = new ODataUri();
            uri.ServiceRoot = new Uri("http://gobbledygook/");
            uri.DeltaToken = "MyToken";
            uri.Path = new ODataPath(new EntitySetSegment(HardCodedTestModel.GetPeopleSet()));
            Assert.Empty(uri.ParameterAliasNodes);

            Uri res = uri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(new Uri("http://gobbledygook/People?$deltatoken=MyToken"), res);
        }
        #endregion
    }
}
