//---------------------------------------------------------------------
// <copyright file="HttpHeaderValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class HttpHeaderValueTests
    {
        [Fact]
        public void EmptyHttpHeaderValueToStringShouldReturnNull()
        {
            HttpHeaderValue elements = new HttpHeaderValue();
            elements.ToString().Should().BeNull();
        }
        
        [Fact]
        public void HttpHeaderValueToStringShouldPutAllHttpHeaderValueElementsInString()
        {
            HttpHeaderValueElement e1 = new HttpHeaderValueElement("e1", null, new KeyValuePair<string, string>[0]);
            HttpHeaderValueElement e2 = new HttpHeaderValueElement("e2", "token", new KeyValuePair<string, string>[0]);
            HttpHeaderValueElement e3 = new HttpHeaderValueElement("e3", "\"quoted-string\"", new KeyValuePair<string, string>[0]);
            HttpHeaderValueElement e4 = new HttpHeaderValueElement("e4", null, new[] { new KeyValuePair<string, string>("p1", "v1"), new KeyValuePair<string, string>("p2", null) });
            HttpHeaderValueElement e5 = new HttpHeaderValueElement("e5", "token", new[] { new KeyValuePair<string, string>("p1", null), new KeyValuePair<string, string>("p2", "\"v2\"") });
            HttpHeaderValueElement e6 = new HttpHeaderValueElement("e6", "\"quoted-string\"", new[] { new KeyValuePair<string, string>("p1", null), new KeyValuePair<string, string>("p2", null), new KeyValuePair<string, string>("p3", "v3"), new KeyValuePair<string, string>("p4", "\"v4\"") });
            const string eStr1 = "e1";
            const string eStr2 = "e2=token";
            const string eStr3 = "e3=\"quoted-string\"";
            const string eStr4 = "e4;p1=v1;p2";
            const string eStr5 = "e5=token;p1;p2=\"v2\"";
            const string eStr6 = "e6=\"quoted-string\";p1;p2;p3=v3;p4=\"v4\"";

            HttpHeaderValue elements = new HttpHeaderValue {{"e1", e1}, {"e2", e2}, {"e3", e3}, {"e4", e4}, {"e5", e5}, {"e6", e6},};
            elements.ToString().Split(new[] {','}).Should().Contain(eStr1).And.Contain(eStr2).And.Contain(eStr3).And.Contain(eStr4).And.Contain(eStr5).And.Contain(eStr6);
        }
    }
}