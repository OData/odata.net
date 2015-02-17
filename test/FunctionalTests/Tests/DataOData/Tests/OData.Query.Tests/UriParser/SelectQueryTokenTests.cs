//---------------------------------------------------------------------
// <copyright file="SelectQueryTokenTests.cs" company="Microsoft">
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
    /// Tests for the select expression parsing.
    /// </summary>
    [TestClass, TestCase]
    public class SelectQueryTokenTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Valid select expressions.")]
        public void SelectExpressionTest()
        {
            IEnumerable<SelectExpressionTestCase> expressionTestCases = SelectExpressionTestCase.SelectPropertyAccessTestCases(ExpressionTestCase.PropertyAccessNames);

            var testCases = expressionTestCases.Select(tc => new SelectTestCase()
            {
                Select = tc.Expression,
                ExpectedSelectToken = new SelectToken(new PathSegmentToken[] { tc.ExpectedToken })
            });

            // All expressions
            testCases.Concat(new SelectTestCase[] { new SelectTestCase()
            {
                Select = string.Join(", ", expressionTestCases.Select(tc => tc.Expression).ToArray()),
                ExpectedSelectToken = new SelectToken(expressionTestCases.Select(tc => tc.ExpectedToken))
            }});

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    SyntacticTree actual = QueryTokenUtils.ParseQuery("Root", select: testCase.Select);
                    SyntacticTree expected = new SyntacticTree(
                        new StartPathToken("Root", null, null),
                        null,
                        null,
                        testCase.ExpectedSelectToken,
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

        private sealed class SelectTestCase
        {
            public string Select { get; set; }
            public SelectToken ExpectedSelectToken { get; set; }
        }
    }
}
