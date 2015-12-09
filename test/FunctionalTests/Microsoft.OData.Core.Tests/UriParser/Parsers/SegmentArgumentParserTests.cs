//---------------------------------------------------------------------
// <copyright file="SegmentArgumentParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
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
            key.NamedValues.Should().Contain("ID", "0");
        }

        [Fact]
        public void AddNamedValueAddsNewValueIfNotPresent()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID=0", out key, false);
            key.AddNamedValue("ID1", "10");
            key.NamedValues.Should().Contain("ID", "0")
                .And.Contain("ID1", "10");
        }

        [Fact]
        public void AddNamedValueCreatesNewDictionaryIfNull()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("", out key, false);
            key.AddNamedValue("ID", "10");
            key.NamedValues.Should().Contain("ID", "10");
        }

        [Fact]
        public void TestTemplateKeyParsing()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID={K0}", out key, true).Should().BeTrue();
            key.NamedValues.Should().Contain("ID", "{K0}");
        }

        [Fact]
        public void TestTemplateKeyParsingWithTemplateParsingDisabled()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID={K0}", out key, false).Should().BeFalse();
        }
    }
}
