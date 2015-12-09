//---------------------------------------------------------------------
// <copyright file="CountBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class CountBuilderTests : UriBuilderTestBase
    {
        [Fact]
        public void CountTrueWorks()
        {
            Uri queryUri = new Uri("People?$count=true", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$count=true"), actualUri);
        }

        [Fact]
        public void CountFalseWorks()
        {
            Uri queryUri = new Uri("People?$count=false", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$count=false"), actualUri);
        }

        [Fact]
        public void LeadingAndTrailingWhitespaceIsTrimmed()
        {
            Uri queryUri = new Uri("People?$count=   true  ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?$count=true"), actualUri);
        }
    }
}
