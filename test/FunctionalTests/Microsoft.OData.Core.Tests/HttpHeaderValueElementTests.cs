//---------------------------------------------------------------------
// <copyright file="HttpHeaderValueElementTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class HttpHeaderValueElementTests
    {
        [Fact]
        public void NullNameShouldThrow()
        {
            Action test = () => new HttpHeaderValueElement(null, null, new KeyValuePair<string, string>[0]);
            test.ShouldThrow<ArgumentNullException>().WithMessage("name", ComparisonMode.Substring);
        }

        [Fact]
        public void EmptyNameShouldThrow()
        {
            Action test = () => new HttpHeaderValueElement("", "", new KeyValuePair<string, string>[0]);
            test.ShouldThrow<ArgumentNullException>().WithMessage("name", ComparisonMode.Substring);
        }

        [Fact]
        public void NullParametersShouldThrow()
        {
            Action test = () => new HttpHeaderValueElement("name", null, null);
            test.ShouldThrow<ArgumentNullException>().WithMessage("parameters", ComparisonMode.Substring);
        }

        [Fact]
        public void ToStringShouldReturnExpectedString()
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

            var testCases = new[]
            {
                new KeyValuePair<HttpHeaderValueElement, string>(e1, eStr1),
                new KeyValuePair<HttpHeaderValueElement, string>(e2, eStr2), 
                new KeyValuePair<HttpHeaderValueElement, string>(e3, eStr3), 
                new KeyValuePair<HttpHeaderValueElement, string>(e4, eStr4), 
                new KeyValuePair<HttpHeaderValueElement, string>(e5, eStr5), 
                new KeyValuePair<HttpHeaderValueElement, string>(e6, eStr6), 
            };

            foreach(KeyValuePair<HttpHeaderValueElement, string> test in testCases)
            {
                test.Key.ToString().Should().Be(test.Value);
            }
        }
    }
}