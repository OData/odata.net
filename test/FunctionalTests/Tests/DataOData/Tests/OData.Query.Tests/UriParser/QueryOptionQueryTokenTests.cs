//---------------------------------------------------------------------
// <copyright file="QueryOptionQueryTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the lexical parsing of query options.
    /// </summary>
    [TestClass, TestCase]
    public class QueryOptionQueryTokenTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Test query options.")]
        public void QueryOptionsTest()
        {
            RunTest(false);
        }

        [TestMethod, Variation(Description = "Test query options with spaces.")]
        public void QueryOptionsWithSpacesTests()
        {
            RunTest(true);
        }

        void RunTest(bool includeSpaceAroundSymbols)
        {
            var testCases = new[]
            {
                new
                {
                    QueryOptions = (List<KeyValuePair<string, string>>)null,
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("$filter", "3 eq 2") },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("CustomValue", "null") },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("null", "CustomValue") },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("$custom", "2") },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() 
                    { 
                        new KeyValuePair<string, string>("$a", "1"),
                        new KeyValuePair<string, string>("$b", "2"),
                    },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() 
                    { 
                        new KeyValuePair<string, string>("a", "1"),
                        new KeyValuePair<string, string>("$filter", "3 eq 2"),
                        new KeyValuePair<string, string>("b", "2"),
                    },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() 
                    { 
                        new KeyValuePair<string, string>("$a", "1"),
                        new KeyValuePair<string, string>("$filter", "3 eq 2"),
                        new KeyValuePair<string, string>("$b", "2"),
                    },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() 
                    { 
                        new KeyValuePair<string, string>("a", "1"),
                        new KeyValuePair<string, string>("b", "2"),
                        new KeyValuePair<string, string>("a", "2"),
                        new KeyValuePair<string, string>("b", "4"),
                    },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>() 
                    { 
                        new KeyValuePair<string, string>("$filter", "3 eq 2"),
                        new KeyValuePair<string, string>("$filter", "2 eq 3"),
                        new KeyValuePair<string, string>("$b", "2"),
                    },
                    ExpectedErrorMessage = "Query option '$filter' was specified more than once, but it must be specified at most once.",
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>()  {  new KeyValuePair<string, string>("$select", "Name") },
                    ExpectedErrorMessage = (string)null,
                },
                new
                {
                    QueryOptions = new List<KeyValuePair<string, string>>()  {  new KeyValuePair<string, string>("$expand", "Products") },
                    ExpectedErrorMessage = (string)null,
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Assert.ExpectedException<ODataException>(
                        () =>
                        {
                            SyntacticTree actual = QueryTokenUtils.ParseQuery("Root", testCase.QueryOptions, includeSpaceAroundSymbols);
                            List<CustomQueryOptionToken> options = QueryTokenUtils.NormalizeAndRemoveBuiltInQueryOptions(testCase.QueryOptions);
                            options = options != null && options.Count == 0 ? null : options;

                            SyntacticTree expected = new SyntacticTree(
                                null,
                                new[] { "Root" },
                                actual.Filter,
                                actual.OrderByTokens,
                                null,
                                null,
                                actual.Skip,
                                null,
                                null,
                                null,
                                options);

                            QueryTokenUtils.VerifySyntaxTreesAreEqual(expected, actual, this.Assert);
                        },
                        testCase.ExpectedErrorMessage,
                        null);
                });
        }
    }
}
