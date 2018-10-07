//---------------------------------------------------------------------
// <copyright file="HttpClientResponseMessageTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Client;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Microsoft.Extensions.ODataClient.Tests.UnitTests
{
    public class HttpClientResponseMessageTest
    {
        [Theory]
        [InlineData(200)]
        [InlineData(500)]
        [InlineData(404)]
        public void TestBasic(int statusCode)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = (HttpStatusCode)statusCode;
            var message = new HttpClientResponseMessage(response, new DataServiceClientConfigurations(this));
            message.StatusCode.Should().Be(statusCode);
        }

        [Theory, MemberData(nameof(HeadersData))]
        public void TestHeaders(Dictionary<string, string> headers)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("TestContent");
            foreach (var item in headers)
            {
                response.Headers.Add(item.Key, item.Value);
            }

            var message = new HttpClientResponseMessage(response, new DataServiceClientConfigurations(this));
            message.Headers.Where(h => !h.Key.StartsWith("Content")).Should().BeEquivalentTo(headers);
        }

        [Theory]
        [InlineData("text/json", "text/json; charset=utf-8")]
        [InlineData("application/json", "application/json; charset=utf-8")]
        [InlineData("application/avro", "application/avro; charset=utf-8")]
        public void TestContentTypeHeaders(string contentType, string result)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("TestContent");
            response.Content.Headers.ContentType.MediaType = contentType;

            var message = new HttpClientResponseMessage(response, new DataServiceClientConfigurations(this));
            message.GetHeader("Content-Type").Should().Be(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(-1)]
        public void TestContentLenthHeaders(int length)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("TestContent");
            response.Content.Headers.ContentLength = length;

            var message = new HttpClientResponseMessage(response, new DataServiceClientConfigurations(this));
            message.GetHeader("Content-Length").Should().Be(length.ToString());
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
                        },
                    },

                    new object[]
                    {
                        new Dictionary<string, string>
                        {
                        },
                    },
                };
            }
        }
    }
}
