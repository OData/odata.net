//---------------------------------------------------------------------
// <copyright file="ContextUrlPathStringTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Query.TDD.Tests.UriBuilder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContextUrlPathStringTests : UriBuilderTestBase
    {
        [TestMethod]
        public void ContextUrlPathWithSimpleEntitySet()
        {
            Uri queryUri = new Uri("Dogs", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.AreEqual("Dogs", res);
        }

        [TestMethod]
        public void ContextUrlPathWithNavigationPropertyLinks()
        {
            Uri queryUri = new Uri("People(1)/MyDog/$ref", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.AreEqual("People(1)", res);
        }

        [TestMethod]
        public void ContextUrlPathWithSimpleServiceOperation()
        {
            Uri queryUri = new Uri("GetCoolPeople", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.AreEqual("", res);
        }

        [TestMethod]
        public void ContextUrlPathWithComplexServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetSomeAddress/City", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.AreEqual("City", res);
        }

        [TestMethod]
        public void ContextUrlPathWithEntityServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetCoolestPerson/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.AreEqual("Fully.Qualified.Namespace.Employee", res);
        }

        #region private methods
        private string GetContextUrlPathString(Uri queryUri)
        {
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, queryUri);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();
            ODataPath odataPath = odataUri.Path;
           return odataPath.ToContextUrlPathString();
        }
        #endregion  
    }
}
