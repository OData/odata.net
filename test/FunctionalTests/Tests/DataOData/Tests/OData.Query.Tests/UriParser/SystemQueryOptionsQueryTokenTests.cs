//---------------------------------------------------------------------
// <copyright file="SystemQueryOptionsQueryTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the lexical parsing of other system query options (like $format and $inlinecount).
    /// </summary>
    [TestClass, TestCase]
    public class SystemQueryOptionsQueryTokenTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description="Format expressions tests")]
        public void FormatExpressionTest()
        {
            string[] values = new string[] { "json", "xml", "foo", "bar" };

            var testCases = values.Select(value =>
                new
                {
                    Value = value,
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    SyntacticTree actual =
                        QueryTokenUtils.ParseQuery("Root", format: testCase.Value);

                    SyntacticTree expected = new SyntacticTree(
                        null,
                        new[] { "Root" },
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        testCase.Value,
                        null);

                    QueryTokenUtils.VerifySyntaxTreesAreEqual(expected, actual, this.Assert);
                });
        }

        [TestMethod, Variation(Description = "Count expressions tests")]
        public void CountExpressionTest()
        {
            object[] values = new object[] { null, 0, 1, 2, 5, 11111, -2, 12.3, "Foo", -12.4m, 'c', (long)int.MaxValue + 1, (long)int.MinValue - 1, 2.3f, "(1)", "2 + 3", "int.MaxValue", "false", "true", "False", "True", "FaLSE", "trUE" };

            string errorMessageTemplate = "Invalid value '{0}' for $count query option found. Valid values are '{1}'.";
            HashSet<string> correctValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true", "false" };
            string correctValuesStr = string.Join(", ", correctValues.ToArray());
            var testCases = values.Select(value =>
                new
                {
                    Value = value == null ? null : value.ToString(),
                    ExpectedErrorMessage = value == null || correctValues.Contains(value.ToString())
                        ? null
                        : string.Format(CultureInfo.InvariantCulture, errorMessageTemplate, value, correctValuesStr).Replace('+', ' ')    // URI all + will be replace by space
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Assert.ExpectedException<ODataException>(
                        () =>
                        {
                            SyntacticTree actual =
                                QueryTokenUtils.ParseQuery("Root", count: testCase.Value);

                            bool? countQuery = null;

                            if (testCase.Value != null)
                            {
                                bool countValue;
                                bool.TryParse(testCase.Value, out countValue);
                                
                                countQuery = countValue;
                            }

                            SyntacticTree expected = new SyntacticTree(
                                null,
                                new[] { "Root" },
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                countQuery,
                                null,
                                null);

                            QueryTokenUtils.VerifySyntaxTreesAreEqual(expected, actual, this.Assert);
                        },
                        testCase.ExpectedErrorMessage,
                        null);
                });
        }
    }
}
