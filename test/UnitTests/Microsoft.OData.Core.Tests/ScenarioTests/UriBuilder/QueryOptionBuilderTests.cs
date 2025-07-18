//---------------------------------------------------------------------
// <copyright file="QueryOptionBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriBuilder
{
    public class QueryOptionBuilderTests : UriBuilderTestBase
    {
        [Fact]
        public void CustomQueryOptionWorks()
        {
            Uri queryUri = new Uri("People?customQueryOption=customValue", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?customQueryOption=customValue"), actualUri);
        }

        [Fact]
        public void MultipleCustomQueryOptionsWorks()
        {
            Uri queryUri = new Uri("People?customQueryOption1=customValue1&customQueryOption2=customValue2", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?customQueryOption1=customValue1&customQueryOption2=customValue2"), actualUri);
        }

        [Fact]
        public void NonKeyValueCustomQueryOptionWorks()
        {
            Uri queryUri = new Uri("People?customQueryOption1", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?customQueryOption1"), actualUri);
        }

        [Fact]
        public void KeyNoValueCustomQueryOptionWorks()
        {
            Uri queryUri = new Uri("People?customQueryOption1=", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?customQueryOption1="), actualUri);
        }

        [Fact]
        public void DuplicatedCustomQueryOptionWorks()
        {
            Uri queryUri = new Uri("People?customQueryOption=value1;customQueryOption=value2", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlKeyDelimiter.Parentheses, settings);
            Assert.Equal(new Uri("http://gobbledygook/People?customQueryOption=value1;customQueryOption=value2"), actualUri);
        }
    }
}