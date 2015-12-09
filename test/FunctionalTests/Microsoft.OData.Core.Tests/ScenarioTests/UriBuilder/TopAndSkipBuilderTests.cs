//---------------------------------------------------------------------
// <copyright file="TopAndSkipBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriBuilder;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class TopAndSkipBuilderTests : UriBuilderTestBase
    {
        #region $top option
        [Fact]
        public void PositiveTopValueWorks()
        {
            Uri queryUri = new Uri("People?$top=5", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$top=5"), actualUri);
        }

        [Fact]
        public void ZeroTopValueWorks()
        {
            Uri queryUri = new Uri("People?$top= 0  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$top=0"), actualUri);
        }
        #endregion $top option

        #region $skip option
        [Fact]
        public void PositiveSkipValueWorks()
        {
            Uri queryUri = new Uri("People?$skip=5", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$skip=5"), actualUri);
        }

        [Fact]
        public void ZeroSkipValueWorks()
        {
            Uri queryUri = new Uri("People?$skip= 0  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$skip=0"), actualUri);
        }
        #endregion $skip option

        #region new ODataUri
        [Fact]
        public void BuildUrlWithNewODataUri()
        {
            ODataUri uri = new ODataUri();
            uri.ServiceRoot = new Uri("http://gobbledygook/");
            uri.Skip = 4;
            uri.Top = 5;
            uri.Path = new ODataPath(new EntitySetSegment(HardCodedTestModel.GetPeopleSet()));
            Assert.Equal(uri.ParameterAliasNodes.Count, 0);

            ODataUriBuilder builder = new ODataUriBuilder(ODataUrlConventions.Default, uri);
            Uri res = builder.BuildUri();
            Assert.Equal(new Uri("http://gobbledygook/People?$top=5&$skip=4"), res);
        }
        #endregion

    }
}
