//---------------------------------------------------------------------
// <copyright file="OrderByQueryTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.Test.Taupo.Common;
using Microsoft.Test.Taupo.Execution;
using Microsoft.Test.Taupo.OData.Common;
using Microsoft.Test.Taupo.OData.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
    /// <summary>
    /// Tests for the filter and orderby expression parsing.
    /// </summary>
    [TestClass, TestCase]
    public class OrderByQueryTokenTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Test failures on invalid orderby expressions.")]
        public void InvalidOrderByExpressionsTest()
        {
            IEnumerable<InvalidExpressionTestCase> testCases = new InvalidExpressionTestCase[]
            {
                new InvalidExpressionTestCase { Expression = "", ExpectedErrorMessage = "Expression expected at position 0 in '$(Expression)'." },

                new InvalidExpressionTestCase { Expression = "foo,", ExpectedErrorMessage = "Expression expected at position 4 in '$(Expression)'." },
                new InvalidExpressionTestCase { Expression = ",foo", ExpectedErrorMessage = "Expression expected at position 0 in '$(Expression)'." },
                new InvalidExpressionTestCase { Expression = "foo asc,", ExpectedErrorMessage = "Expression expected at position 8 in '$(Expression)'." },
                new InvalidExpressionTestCase { Expression = "foo desc,", ExpectedErrorMessage = "Expression expected at position 9 in '$(Expression)'." },
                new InvalidExpressionTestCase { Expression = "foo asc desc", ExpectedErrorMessage = "Syntax error at position 12 in '$(Expression)'." },
                new InvalidExpressionTestCase { Expression = "foo ASC", ExpectedErrorMessage = "Syntax error at position 7 in '$(Expression)'." },
            };

            testCases = testCases.Concat(InvalidExpressionTestCase.InvalidPrimitiveLiteralTestCases);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Assert.ExpectedException<ODataException>(
                        () => QueryTokenUtils.ParseQuery("Root", orderby: testCase.Expression),
                        StringUtils.ResolveVariables(testCase.ExpectedErrorMessage, new Dictionary<string, string>() { { "Expression", testCase.Expression } }),
                        "The orderby parsing should have failed.");
                });
        }

        [TestMethod, Variation(Description = "Test orderby expressions.")]
        public void OrderByExpressionTest()
        {
            IEnumerable<ExpressionTestCase> expressionTestCases = ExpressionTestCase.PrimitiveLiteralTestCases()
                .Concat(ExpressionTestCase.BinaryOperatorTestCases())
                .Concat(ExpressionTestCase.UnaryOperatorTestCases())
                .Concat(ExpressionTestCase.PropertyAccessTestCases(ExpressionTestCase.PropertyAccessNames))
                .Concat(ExpressionTestCase.ParenthesisTestCases())
                .Concat(ExpressionTestCase.FunctionCallTestCases());

            // Use filter expressions first without anything
            var filterWithNoDirection = expressionTestCases.Select(tc => new OrderByTestCase()
            {
                OrderBy = tc.Expression,
                ExpectedOrderByTokens = new OrderByToken[] { new OrderByToken(tc.ExpectedToken, OrderByDirection.Ascending) }
            });

            // Use filter expressions with asc
            var filterWithAscending = expressionTestCases.Select(tc => new OrderByTestCase()
            {
                OrderBy = tc.Expression + " asc",
                ExpectedOrderByTokens = new OrderByToken[] { new OrderByToken(tc.ExpectedToken, OrderByDirection.Ascending) }
            });

            // Use filter expressions with desc
            var filterWithDescending = expressionTestCases.Select(tc => new OrderByTestCase()
            {
                OrderBy = tc.Expression + " desc",
                ExpectedOrderByTokens = new OrderByToken[] { new OrderByToken(tc.ExpectedToken, OrderByDirection.Descending) }
            });

            // And now some order by specific cases with multiple orderbys
            var orderByTestCases = ExpressionTestCase.VariousExpressions().ToList().Variations(3).Select(tc =>
                new OrderByTestCase
                {
                    OrderBy = string.Join(",", tc.Select((t, index) => t.Expression + ((index % 3 == 0) ? "" : ((index % 3 == 1) ? " asc" : " desc"))).ToArray()),
                    ExpectedOrderByTokens = tc.Select((t, index) => new OrderByToken(
                        t.ExpectedToken,
                        (index % 3 == 2) ? OrderByDirection.Descending : OrderByDirection.Ascending)).ToArray()
                });

            CombinatorialEngineProvider.RunCombinations(
                filterWithNoDirection
                .Concat(filterWithAscending)
                .Concat(filterWithDescending)
                .Concat(orderByTestCases),
                (testCase) =>
                {
                    SyntacticTree actual = QueryTokenUtils.ParseQuery("Root", orderby: testCase.OrderBy);
                    SyntacticTree expected = new SyntacticTree(
                        null,
                        new[] { "Root" }, 
                        null,
                        testCase.ExpectedOrderByTokens,
                        null,
                        null,
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

        internal class OrderByTestCase
        {
            public string OrderBy { get; set; }
            public OrderByToken[] ExpectedOrderByTokens { get; set; }
            public override string ToString() { return this.OrderBy; }
        }
    }
}
