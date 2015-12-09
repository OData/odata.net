//---------------------------------------------------------------------
// <copyright file="ContextUrlPathStringTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class ContextUrlPathStringTests : UriBuilderTestBase
    {
        [Fact]
        public void ContextUrlPathWithSimpleEntitySet()
        {
            Uri queryUri = new Uri("Dogs", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.Equal("Dogs", res);
        }

        [Fact]
        public void ContextUrlPathWithNavigationPropertyLinks()
        {
            Uri queryUri = new Uri("People(1)/MyDog/$ref", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.Equal("People(1)", res);
        }

        [Fact]
        public void ContextUrlPathWithSimpleServiceOperation()
        {
            Uri queryUri = new Uri("GetCoolPeople", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.Equal("", res);
        }

        [Fact]
        public void ContextUrlPathWithComplexServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetSomeAddress/City", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.Equal("City", res);
        }

        [Fact]
        public void ContextUrlPathWithEntityServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetCoolestPerson/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            string res = this.GetContextUrlPathString(queryUri);
            Assert.Equal("Fully.Qualified.Namespace.Employee", res);
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
