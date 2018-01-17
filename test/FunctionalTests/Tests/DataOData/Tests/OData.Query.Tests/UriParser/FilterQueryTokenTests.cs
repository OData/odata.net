//---------------------------------------------------------------------
// <copyright file="FilterQueryTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the filter and orderby expression parsing.
    /// </summary>
    [TestClass, TestCase]
    public class FilterQueryTokenTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Test failures on invalid filter expressions.")]
        public void InvalidFilterExpressionsTest()
        {
            IEnumerable<InvalidExpressionTestCase> testCases = new InvalidExpressionTestCase[]
            {
                new InvalidExpressionTestCase { Expression = "", ExpectedErrorMessage = "Expression expected at position 0 in '$(Expression)'." },
            };

            testCases = testCases.Concat(InvalidExpressionTestCase.InvalidPrimitiveLiteralTestCases);
            testCases = testCases.Concat(InvalidExpressionTestCase.InvalidExpressionTestCases);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    this.Assert.ExpectedException<ODataException>(
                        () => QueryTokenUtils.ParseQuery("Root", filter: testCase.Expression),
                        StringUtils.ResolveVariables(testCase.ExpectedErrorMessage, new Dictionary<string, string>() { { "Expression", testCase.Expression } }),
                        "The filter parsing should have failed.");
                });
        }

        [TestMethod, Variation(Description = "Test binary filter expressions.")]
        public void UriVariationExpressionsTest()
        {
            IEnumerable<ExpressionTestCase> testCases = new ExpressionTestCase[]
            {
                // Uri query where + instead of space is in Uri text
                new ExpressionTestCase { Expression = "1+lt+2+and+true+ne+false", ExpectedToken = new BinaryOperatorToken(
                    BinaryOperatorKind.And, 
                    new BinaryOperatorToken(BinaryOperatorKind.LessThan, new LiteralToken(1), new LiteralToken(2)),
                    new BinaryOperatorToken(BinaryOperatorKind.NotEqual, new LiteralToken(true), new LiteralToken(false))) }
            };

            CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    QueryTokenUtils.VerifyQueryTokensAreEqual(
                        testCase.ExpectedToken,
                        QueryTokenUtils.ParseQuery("Root", filter: testCase.Expression).Filter,
                        this.Assert);
                });
        }

        [TestMethod, Variation(Description = "Test filter expressions.")]
        public void FilterExpressionTest()
        {
            CombinatorialEngineProvider.RunCombinations(
                ExpressionTestCase.PrimitiveLiteralTestCases()
                .Concat(ExpressionTestCase.BinaryOperatorTestCases())
                .Concat(ExpressionTestCase.UnaryOperatorTestCases())
                .Concat(ExpressionTestCase.PropertyAccessTestCases(ExpressionTestCase.PropertyAccessNames))
                .Concat(ExpressionTestCase.ParenthesisTestCases())
                .Concat(ExpressionTestCase.FunctionCallTestCases()),
                testCase => QueryTokenUtils.VerifyQueryTokensAreEqual(
                    testCase.ExpectedToken,
                    QueryTokenUtils.ParseQuery("Root", filter: testCase.Expression).Filter,
                    this.Assert));
        }
    }
}
