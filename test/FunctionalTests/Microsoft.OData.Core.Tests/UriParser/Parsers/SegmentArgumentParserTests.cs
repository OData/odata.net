//---------------------------------------------------------------------
// <copyright file="SegmentArgumentParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the SegmentArgumentParser class.
    /// </summary>
    public class SegmentArgumentParserTests
    {
        [Fact]
        public void AddNamedValueDoesNotOverrideCurrentValueIfPresent()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID=0", out key, false);
            key.AddNamedValue("ID", "10");
            Assert.Contains("ID", key.NamedValues.Keys);
            Assert.Contains("0", key.NamedValues.Values);
        }

        [Fact]
        public void AddNamedValueAddsNewValueIfNotPresent()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID=0", out key, false);
            key.AddNamedValue("ID1", "10");

            Assert.Contains("ID", key.NamedValues.Keys);
            Assert.Contains("0", key.NamedValues.Values);

            Assert.Contains("ID1", key.NamedValues.Keys);
            Assert.Contains("10", key.NamedValues.Values);
        }

        [Fact]
        public void AddNamedValueCreatesNewDictionaryIfNull()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("", out key, false);
            key.AddNamedValue("ID", "10");
            Assert.Contains("ID", key.NamedValues.Keys);
            Assert.Contains("10", key.NamedValues.Values);
        }

        [Fact]
        public void TestTemplateKeyParsing()
        {
            SegmentArgumentParser key;
            var result = SegmentArgumentParser.TryParseKeysFromUri("ID={K0}", out key, true);
            Assert.True(result);
            Assert.Contains("ID", key.NamedValues.Keys);
            Assert.Contains("{K0}", key.NamedValues.Values);
        }

        [Fact]
        public void TestTemplateKeyParsingWithTemplateParsingDisabled()
        {
            SegmentArgumentParser key;
            var result = SegmentArgumentParser.TryParseKeysFromUri("ID={K0}", out key, false);
            Assert.False(result);
        }
    }
}
