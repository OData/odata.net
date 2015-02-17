//---------------------------------------------------------------------
// <copyright file="TopAndSkipBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.UriBuilder
{
    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriBuilder;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TopAndSkipBuilderTests : UriBuilderTestBase
    {
        #region $top option
        [TestMethod]
        public void PositiveTopValueWorks()
        {
            Uri queryUri = new Uri("People?$top=5", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$top=5"), actualUri);
        }

        [TestMethod]
        public void ZeroTopValueWorks()
        {
            Uri queryUri = new Uri("People?$top= 0  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$top=0"), actualUri);
        }
        #endregion $top option

        #region $skip option
        [TestMethod]
        public void PositiveSkipValueWorks()
        {
            Uri queryUri = new Uri("People?$skip=5", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$skip=5"), actualUri);
        }

        [TestMethod]
        public void ZeroSkipValueWorks()
        {
            Uri queryUri = new Uri("People?$skip= 0  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$skip=0"), actualUri);
        }
        #endregion $skip option

        #region new ODataUri
        [TestMethod]
        public void BuildUrlWithNewODataUri()
        {
            ODataUri uri = new ODataUri();
            uri.ServiceRoot = new Uri("http://gobbledygook/");
            uri.Skip = 4;
            uri.Top = 5;
            uri.Path = new ODataPath(new EntitySetSegment(HardCodedTestModel.GetPeopleSet()));
            Assert.AreEqual(uri.ParameterAliasNodes.Count, 0);

            ODataUriBuilder builder = new ODataUriBuilder(ODataUrlConventions.Default, uri);
            Uri res = builder.BuildUri();
            Assert.AreEqual(new Uri("http://gobbledygook/People?$top=5&$skip=4"), res);
        }
        #endregion

    }
}
