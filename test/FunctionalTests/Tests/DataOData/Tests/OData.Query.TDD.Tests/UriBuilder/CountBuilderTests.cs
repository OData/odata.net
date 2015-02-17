//---------------------------------------------------------------------
// <copyright file="CountBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.UriBuilder
{
    using System;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CountBuilderTests : UriBuilderTestBase
    {
        [TestMethod]
        public void CountTrueWorks()
        {
            Uri queryUri = new Uri("People?$count=true", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$count=true"), actualUri);
        }

        [TestMethod]
        public void CountFalseWorks()
        {
            Uri queryUri = new Uri("People?$count=false", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$count=false"), actualUri);
        }

        [TestMethod]
        public void LeadingAndTrailingWhitespaceIsTrimmed()
        {
            Uri queryUri = new Uri("People?$count=   true  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$count=true"), actualUri);
        }
    }
}
