//---------------------------------------------------------------------
// <copyright file="ExpandQueryTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
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

    /// <summary>
    /// Tests for the expand expression parsing.
    /// </summary>
    [TestClass, TestCase]
    public class ExpandQueryTokenTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Invalid expand expressions.")]
        public void InvalidExpandExpressionsTest()
        {
            IEnumerable<InvalidExpressionTestCase> testCases = new InvalidExpressionTestCase[]
            {
                new InvalidExpressionTestCase { Expression = "", ExpectedErrorMessage = "Expression expected at position 0 in ''." },
                new InvalidExpressionTestCase { Expression = "foo,", ExpectedErrorMessage = "Expression expected at position 4 in 'foo,'." },
                new InvalidExpressionTestCase { Expression = ",foo", ExpectedErrorMessage = "Expression expected at position 0 in ',foo'." },
                new InvalidExpressionTestCase { Expression = "/foo", ExpectedErrorMessage = "Expression expected at position 0 in '/foo'." },
                new InvalidExpressionTestCase { Expression = "foo/", ExpectedErrorMessage = "An identifier was expected at position 4." },
            };

            testCases = testCases.Concat(InvalidExpressionTestCase.InvalidPrimitiveLiteralTestCases);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Assert.ExpectedException<ODataException>(
                        () => QueryTokenUtils.ParseQuery("Root", expand: testCase.Expression),
                        StringUtils.ResolveVariables(testCase.ExpectedErrorMessage, new Dictionary<string, string>() { { "Expression", testCase.Expression } }),
                        "The expand parsing should have failed.");
                });
        }

        [TestMethod, Variation(Description = "Expand expressions.")]
        public void ExpandExpressionTest()
        {
            IEnumerable<ExpressionTestCase> expressionTestCases = ExpressionTestCase.PrimitiveLiteralTestCases()
                .Concat(ExpressionTestCase.BinaryOperatorTestCases())
                .Concat(ExpressionTestCase.UnaryOperatorTestCases())
                .Concat(ExpressionTestCase.PropertyAccessTestCases(ExpressionTestCase.PropertyAccessNames))
                .Concat(ExpressionTestCase.ParenthesisTestCases())
                .Concat(ExpressionTestCase.FunctionCallTestCases());

            var testCases = expressionTestCases.Select(tc => new ExpandTestCase()
            {
                Expand = tc.Expression,
                ExpectedExpandToken = new ExpandToken(new QueryToken[] { tc.ExpectedToken })
            });

            // All expressions
            testCases.Concat(new ExpandTestCase[] { new ExpandTestCase()
            {
                Expand = string.Join(", ", expressionTestCases.Select(tc => tc.Expression).ToArray()),
                ExpectedExpandToken = new ExpandToken(expressionTestCases.Select(tc => tc.ExpectedToken))
            }});

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    SyntacticTree actual = QueryTokenUtils.ParseQuery("Root", expand: testCase.Expand);
                    SyntacticTree expected = new SyntacticTree(
                        new SegmentToken("Root", null, null),
                        null,
                        null,
                        null,
                        testCase.ExpectedExpandToken,
                        null,
                        null,
                        null,
                        null,
                        null);

                    QueryTokenUtils.VerifySyntaxTreesAreEqual(
                        expected,
                        actual,
                        this.Assert);
                });
        }

        internal class ExpandTestCase
        {
            public string Expand { get; set; }
            public ExpandToken ExpectedExpandToken { get; set; }
        }
    }
}
