//---------------------------------------------------------------------
// <copyright file="SegmentArgumentParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the SegmentArgumentParser class.
    /// </summary>
    [TestClass]
    public class SegmentArgumentParserUnitTests
    {
        [TestMethod]
        public void AddNamedValueDoesNotOverrideCurrentValueIfPresent()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID=0", out key, false);
            key.AddNamedValue("ID", "10");
            key.NamedValues.Should().Contain("ID", "0");
        }

        [TestMethod]
        public void AddNamedValueAddsNewValueIfNotPresent()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID=0", out key, false);
            key.AddNamedValue("ID1", "10");
            key.NamedValues.Should().Contain("ID", "0")
                .And.Contain("ID1", "10");
        }

        [TestMethod]
        public void AddNamedValueCreatesNewDictionaryIfNull()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("", out key, false);
            key.AddNamedValue("ID", "10");
            key.NamedValues.Should().Contain("ID", "10");
        }

        [TestMethod]
        public void TestTemplateKeyParsing()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID={K0}", out key, true).Should().BeTrue();
            key.NamedValues.Should().Contain("ID", "{K0}");
        }

        [TestMethod]
        public void TestTemplateKeyParsingWithTemplateParsingDisabled()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID={K0}", out key, false).Should().BeFalse();
        }
    }
}
