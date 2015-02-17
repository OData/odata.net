//---------------------------------------------------------------------
// <copyright file="SearchBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.UriBuilder
{
    using System;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SearchBuilderTests : UriBuilderTestBase
    {
         [TestMethod]
        public void BuildSearchWithWordTest()
        {
            Uri queryUri = new Uri("People?$search=bike單車", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("bike單車"), actualUri.OriginalString);
        }

         [TestMethod]
         public void BuildSearchWithMultipleWordTest()
         {
             Uri queryUri = new Uri("People?$search=NOT A OR NOT (B AND C AND D)", UriKind.Relative);
             Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
             Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("NOT A OR NOT(B AND C AND D)"), actualUri.OriginalString);
         }

        [TestMethod]
         public void BuildSearchWithPhraseTest()
        {
            Uri queryUri = new Uri("People?$search=\"mountain bike\"", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("\"mountain bike\""), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithAndTest()
        {
            Uri queryUri = new Uri("People?$search=mountain bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("mountain AND bike"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithImplicitAndTest()
        {
            Uri queryUri = new Uri("People?$search=mountain NOT bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("mountain AND NOT bike"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithErrorProneImplicitAndTest()
        {
            Uri queryUri = new Uri("People?$search=mountain or bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("mountain AND or AND bike"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithOrTest()
        {
            Uri queryUri = new Uri("People?$search=mountain OR bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("mountain OR bike"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithORStringTest()
        {
            Uri queryUri = new Uri("People?$search=mountain \"OR\" bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("mountain AND \"OR\" AND bike"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithANDStringTest()
        {
            Uri queryUri = new Uri("People?$search=mountain \"AND\" bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("mountain AND \"AND\" AND bike"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithNOTStringTest()
        {
            Uri queryUri = new Uri("People?$search=mountain \"NOT\" bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("mountain AND \"NOT\" AND bike"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BuildSearchWithNotTest()
        {
            Uri queryUri = new Uri("People?$search=NOT bike", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("NOT bike"), actualUri.OriginalString);        
        }

        [TestMethod]
        public void BuildSearchWithCombinationTest()
        {
            Uri queryUri = new Uri("People?$search=NOT Tis AND (in OR \"my memory\") \"lock'd\"", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$search=" + Uri.EscapeDataString("NOT Tis AND (in OR \"my memory\") AND \"lock'd\""), actualUri.OriginalString);        
        }
    }
}
