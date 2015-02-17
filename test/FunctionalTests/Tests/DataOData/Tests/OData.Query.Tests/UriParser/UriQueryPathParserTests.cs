//---------------------------------------------------------------------
// <copyright file="UriQueryPathParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Query;
    using Microsoft.OData.Core.Query.SyntacticAst;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the path parsing of the URI query.
    /// </summary>
    [TestClass, TestCase]
    public class UriQueryPathParserTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Test acceptance of various wrong URIs.")]
        public void UriCheckTests()
        {
            this.Assert.ExpectedException<ODataException>(
                () => SyntacticTree.ParseUri(new Uri("http://www.odata.org/base1/foo"), new Uri("http://www.odata.org/base2/")),
                "The URI 'http://www.odata.org/base1/foo' is not valid because it is not based on 'http://www.odata.org/base2/'.",
                "The operation should fail if the request URI is not relative to the specified base.");

            // Starting with .Net 4.5, Uri.UnescapeDataString does not throw an exception on invalid sequence. It just does not unescape something
            // that it does not understand. Hence disabling these tests.
            //this.Assert.ExpectedException<ODataException>(
            //    () => SyntacticTree.ParseUri(new Uri("http://www.odata.org/base1/foo%ZZ%%/", true), new Uri("http://www.odata.org/base1/")),
            //    "Bad Request: there was an error in the query syntax.",
            //    "Wrong escape sequence should cause ODataException.");

            // Test the what will be a default max depth which should be 800
            string longUri = "http://www.odata.org/";
            for (int i = 0; i < 801; i++)
            {
                longUri += "seg/";
            }

            this.Assert.ExpectedException<ODataException>(
                () => SyntacticTree.ParseUri(new Uri(longUri), new Uri("http://www.odata.org/")),
                "Too many segments in URI.",
                "Recursion limit should reject long URI.");

            // Test the explicit max depth
            // Test short max depth
            longUri = "http://www.odata.org/";
            for (int i = 0; i < 11; i++)
            {
                longUri += "seg/";
            }

            this.Assert.ExpectedException<ODataException>(
                () => SyntacticTree.ParseUri(new Uri(longUri), new Uri("http://www.odata.org/"), 10),
                "Too many segments in URI.",
                "Recursion limit should reject long URI.");
        }

        [TestMethod, Variation(Description = "Verifies the parser correcly handles segments.")]
        public void SegmentTests()
        {
            var testCases = new []
            {
                new{
                    Uri = "/Customers",
                    ExpectedNodes = new StartPathToken("Customers", null, null)
                },
                new{
                    Uri = "/Customers()",
                    ExpectedNodes = new StartPathToken("Customers", null, new NamedValue[0])
                },
                new{
                    Uri = "/Customers/$count",
                    ExpectedNodes = (StartPathToken)new KeywordSegmentToken(KeywordKind.Count, new StartPathToken("Customers", null, null))
                },
                new{
                    Uri = "/$metadata/Foo",
                    ExpectedNodes = new StartPathToken("Foo", new KeywordSegmentToken(KeywordKind.Metadata, null), null)
                },
                new{
                    Uri = "/Customers()/$count",
                    ExpectedNodes = (StartPathToken)new KeywordSegmentToken(KeywordKind.Count, new StartPathToken("Customers", null, new NamedValue[0]))
                },
                new{
                    Uri = "/Customers()/$links/Orders()",
                    ExpectedNodes = new StartPathToken(
                        "Orders", 
                        new KeywordSegmentToken(KeywordKind.Links, new StartPathToken("Customers", null, new NamedValue[0])), new NamedValue[0])
                },
                new{
                    Uri = "/Customers/$batch",
                    ExpectedNodes = (StartPathToken)new KeywordSegmentToken(KeywordKind.Batch, new StartPathToken("Customers", null, null))
                },
                new{
                    Uri = "/Orders/$value",
                    ExpectedNodes = (StartPathToken)new KeywordSegmentToken(KeywordKind.Value, new StartPathToken("Orders", null, null))
                },
                new{
                    Uri = "/",
                    ExpectedNodes = new StartPathToken("", null, null)
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                testCase => QueryTokenUtils.VerifyQueryTokensAreEqual(
                    testCase.ExpectedNodes, 
                    QueryTokenUtils.ParseQuery(testCase.Uri).Path, 
                    this.Assert));
        }

        class KeyValuesTestCase
        {
            public string KeyValues { get; set; }
            public NamedValue[] ExpectedKeyValues { get; set; }
            public override string ToString() { return this.KeyValues; }
        }

        [TestMethod, Variation(Description = "Verifies correct behavior of the key values parser.")]
        public void KeyValuesTest()
        {
            IEnumerable<KeyValuesTestCase> testCases = new[]
            {
                new KeyValuesTestCase {
                    KeyValues = "()",
                    ExpectedKeyValues = new NamedValue[0]
                },
                new KeyValuesTestCase {
                    KeyValues = "(0)",
                    ExpectedKeyValues = new [] { new NamedValue(null, new LiteralToken(0)) }
                },
                new KeyValuesTestCase {
                    KeyValues = "(0,1)",
                    ExpectedKeyValues = new [] { 
                        new NamedValue(null, new LiteralToken(0)),
                        new NamedValue(null, new LiteralToken(1))
                    }
                },
                new KeyValuesTestCase {
                    KeyValues = "(Key='some')",
                    ExpectedKeyValues = new [] { 
                        new NamedValue("Key", new LiteralToken("some")),
                    }
                },
                new KeyValuesTestCase {
                    KeyValues = "(Key=datetime'2010-12-09T14:10:20')",
                    ExpectedKeyValues = new [] { 
                        new NamedValue("Key", new LiteralToken(new DateTime(2010, 12, 9, 14, 10, 20)))
                    }
                },
                new KeyValuesTestCase {
                    KeyValues = "(Key1=42.42,  Key2=42f)",
                    ExpectedKeyValues = new [] { 
                        new NamedValue("Key1", new LiteralToken((double)42.42)),
                        new NamedValue("Key2", new LiteralToken((Single)42))
                    }
                },
                new KeyValuesTestCase {
                    KeyValues = "(Key=1,Key=2)",
                    ExpectedKeyValues = new [] { 
                        new NamedValue("Key", new LiteralToken(1)),
                        new NamedValue("Key", new LiteralToken(2))
                    }
                },
            };

            testCases.Concat(ExpressionTestCase.PrimitiveLiteralTestCases().Select(tc =>
                new KeyValuesTestCase()
                {
                    KeyValues = "(" + tc.Expression + ")",
                    ExpectedKeyValues = new[] { new NamedValue(null, (LiteralToken)tc.ExpectedToken) }
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                testCase => QueryTokenUtils.VerifyQueryTokensAreEqual(
                    new StartPathToken("Customers", null, testCase.ExpectedKeyValues),
                    QueryTokenUtils.ParseQuery("/Customers" + testCase.KeyValues).Path,
                    this.Assert));
        }

        [TestMethod, Variation(Description = "Verifies correct behavior for invalid key values.")]
        public void InvalidKeyValuesTest()
        {
            var testCases = new[] 
            {
                new {
                    KeyValues = "(",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(42",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(42,",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(Key)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(Key,0)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(Key=Key)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "\t(  Key = eq)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(eq)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "\t(  gt )",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "\t(()",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(-INFI)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(INFI)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(-NaNn)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
                new {
                    KeyValues = "(NaNn)",
                    ExpectedErrorMessage = "Bad Request: there was an error in the query syntax."
                },
            };

            var keyValuesContainers = new string[]
            {
                "/Customers{0}",
                "/Customers{0}/Orders",
                "/Customers(0)/Orders{0}",
                "/Customers(0)/$links/Orders{0}"
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                keyValuesContainers,
                (testCase, keyValuesContainer) =>
                {
                    this.Assert.ExpectedException<ODataException>(
                        () => QueryTokenUtils.ParseQuery(string.Format(keyValuesContainer, testCase.KeyValues)),
                        testCase.ExpectedErrorMessage,
                        "Parser should fail to parse a key value.");
                });
        }

        [TestMethod, Variation(Description = "Verifies correct behavior with invalid key values parsing.")]
        public void InvalidSingleKeyValueTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                InvalidExpressionTestCase.InvalidPrimitiveLiteralTestCases,
                (testCase) =>
                {
                    this.Assert.ExpectedException<ODataException>(
                        () => QueryTokenUtils.ParseQuery("/Customers(" + testCase.Expression + ")"),
                        StringUtils.ResolveVariables(testCase.ExpectedErrorMessage, new Dictionary<string,string>() { { "Expression", testCase.Expression } }),
                        "Parser should fail to parse a key value.");
                });
        }
    }
}
