//---------------------------------------------------------------------
// <copyright file="HttpClientRequestMessageTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Microsoft.Extensions.ODataClient.Tests.UnitTests
{
    public class HttpClientRequestMessageTest
    {
        [Theory]
        [InlineData("Get", "Http://Localhost/")]
        [InlineData("Put", "Http://Localhost2/Test")]
        [InlineData("Post", "Http://Localhost/Test?api-version=1.0")]
        public void TestBasic(string method, string requestUri)
        {
            var client = new HttpClient();
            var arg = new DataServiceClientRequestMessageArgs(method, new Uri(requestUri), true, true, new Dictionary<string, string>());
            var message = new HttpClientRequestMessage(client, arg, new DataServiceClientConfigurations(this));
            message.Method.Should().Be(method);
            message.Url.Should().Be(requestUri);
        }

        [Theory, MemberData(nameof(HeadersData))]
        public void TestHeaders(Dictionary<string, string> headers)
        {
            var client = new HttpClient();
            var arg = new DataServiceClientRequestMessageArgs("Get", new Uri("Http://localhost"), true, true, headers);
            var message = new HttpClientRequestMessage(client, arg, new DataServiceClientConfigurations(this));
            message.Headers.Should().BeEquivalentTo(headers);
        }

        public static IEnumerable<object[]> HeadersData
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new Dictionary<string, string>
                        {
                            {"Header1", "Value1" },
                            {"Header2", "Value2" },
                            {"Header3", "Value3" },
                            { "Content-Length", "1"},
                            { "Content-Type","txt" }
                        },
                    },

                    new object[]
                    {
                        new Dictionary<string, string>
                        {
                        },
                    },

                    new object[]
                    {
                        new Dictionary<string, string>
                        {
                            {"Header1", "Value1" },
                            {"Header2", "Value2" },
                            {"Header3", "Value3" },
                        },
                    },

                    new object[]
                    {
                        new Dictionary<string, string>
                        {
                            { "Content-Length", "1"},
                            { "Content-Type","txt" }
                        },
                    }
                };
            }
        }
    }
}
