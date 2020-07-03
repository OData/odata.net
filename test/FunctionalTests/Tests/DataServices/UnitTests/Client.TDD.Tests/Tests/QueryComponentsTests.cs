//---------------------------------------------------------------------
// <copyright file="QueryComponentsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Unit tests for the QueryComponents class.
    /// </summary>
    public class QueryComponentsTests
    {
        [Fact]
        public void UriWithSelectInMiddleOfQueryString()
        {
            CreateQueryOptions("?$filter=City eq 'Paris'&$select=Name,OrderDate").HasSelectQueryOption.Should().BeTrue();
        }

        [Fact]
        public void UriWithSelectAtBeginningOfQueryString()
        {
            CreateQueryOptions("?$select=Name,OrderDate").HasSelectQueryOption.Should().BeTrue();
        }

        [Fact]
        public void UriWithNoQueryString()
        {
            CreateQueryOptions("").HasSelectQueryOption.Should().BeFalse();
        }

        [Fact]
        public void UriWithEmptyQueryString()
        {
            CreateQueryOptions("?").HasSelectQueryOption.Should().BeFalse();
        }

        [Fact]
        public void UriWithQueryStringOtherThanSelect()
        {
            CreateQueryOptions("?$orderby=State").HasSelectQueryOption.Should().BeFalse();
        }

        [Fact]
        public void UriWithSelectAtBeginningAndOtherOptions()
        {
            CreateQueryOptions("?$select=Name,OrderDate&$&$filter=City eq 'Redmond'").HasSelectQueryOption.Should().BeTrue();
        }

        [Fact]
        public void UriWithSelectInMiddleAndOtherOptions()
        {
            CreateQueryOptions("?$filter=City eq 'Redmond'&$select=Name,OrderDate&$orderby=State").HasSelectQueryOption.Should().BeTrue();
        }

        [Fact]
        public void UriWithSelectNotFollowedByEquals()
        {
            CreateQueryOptions("?$selectExtra=SomeOtherKindOfValue").HasSelectQueryOption.Should().BeFalse();
        }

        [Fact]
        public void UriWithSelectInDataString()
        {
            // $select is part of data not a query option
            CreateQueryOptions("?$filter=Name eq " + Uri.EscapeDataString("'?$select='")).HasSelectQueryOption.Should().BeFalse();
        }

        private static QueryComponents CreateQueryOptions(string uriQueryString)
        {
            var queryUri = new Uri("/Orders(key1='abc',key2=44)" + uriQueryString, UriKind.Relative);
            return new QueryComponents(queryUri, Util.ODataVersion4, typeof(object), null, null);
        }
    }
}