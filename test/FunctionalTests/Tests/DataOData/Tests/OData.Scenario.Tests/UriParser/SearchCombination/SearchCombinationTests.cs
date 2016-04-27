//---------------------------------------------------------------------
// <copyright file="SearchCombinationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.SearchCombination
{
    #region namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion namespaces

    [UseReporter(typeof(LoggingReporter))]
    [DeploymentItem("UriParser\\SearchCombination")]
    [TestClass]
    public class SearchCombinationTests : ODataTestCaseBase
    {
        /// <summary>
        /// The combinatorial engine to use for running matrix based tests.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        private readonly static Uri ResourceRoot = new Uri("http://host/");

        /// <summary>
        /// Run tests with one word, and use a lexicon with a max length of 2 to test match information.
        /// Total case number:
        /// t(n) = Σ{i=1 to n}(t(i) * t(n-i)) * 2 * 4
        ///         SubcombinationsSum * OperationCombinations * NotCombinations
        /// Example:
        /// 
        /// Lexicon: ,A,B,AB
        /// $search=NOT A
        /// 
        /// Result is:
        /// Match:, B;
        /// Unmatch:A, AB;
        /// 
        /// Total combinations: t(1) = 2
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void OneWordTest()
        {
            RunTestCases(BuildUpCases(SearchTestCase.Words.Take(1)), SearchTestCase.BuildLexicon(2));
        }

        /// <summary>
        /// Run tests with one word, and use a lexicon with a max length of 2 to test match information.
        /// Total combinations: t(2) = 1 * 1 * 2 * 4 = 8
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void TwoWordsTest()
        {
            RunTestCases(BuildUpCases(SearchTestCase.Words.Take(2)), SearchTestCase.BuildLexicon(3));
        }

        /// <summary>
        /// Run tests with one word, and use a lexicon with a max length of 2 to test match information.
        /// Total combinations: t(3) = t(1) * t(2) * 2 * 2 * 4 = 256
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void ThreeWordsTest()
        {
            RunTestCases(BuildUpCases(SearchTestCase.Words.Take(3)), SearchTestCase.BuildLexicon(4));
        }

        /// <summary>
        /// Run tests with one word, and use a lexicon with a max length of 2 to test match information.
        /// Total combinations: t(4) = (t(1) * t(3) + t(2) * t(2) + t(3) * t(1))* 2 * 4 = 8704
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void FourWordsTest()
        {
            RunTestCases(BuildUpCases(SearchTestCase.Words.Take(4)), SearchTestCase.BuildLexicon(5));
        }

        private void RunTestCases(IEnumerable<SearchTestCase> testCases, IEnumerable<string> lexicon)
        {
            int count = 0;
            StringBuilder builder = new StringBuilder();

            foreach (var testCase in testCases)
            {
                builder.AppendFormat("Combination #{0}: $search={1}\n", ++count, testCase.RawQuery);
                builder.AppendLine(this.GetResult(testCase, lexicon));
            }

            this.ApprovalVerify(builder.ToString());
        }

        /// <summary>
        /// Build up cases using given <paramref name="words"/>
        /// </summary>
        /// <param name="words">The list of string to be used as basic characters.</param>
        /// <param name="includeExceptionCase">Wheter include invalid cases.</param>
        /// <returns></returns>
        private IEnumerable<SearchTestCase> BuildUpCases(IEnumerable<string> words, bool includeExceptionCase = true)
        {
            List<SearchTestCase> list = new List<SearchTestCase>();
            int l = words.Count();
            if (l == 1)
            {
                var word = words.First();
                list.Add(new SearchTestCase() { RawQuery = word, TopOperator = SearchOperator.None });
                list.Add(new SearchTestCase() { RawQuery = "NOT " + word, TopOperator = SearchOperator.Not });
            }
            else if (l == 2)
            {
                var ca1 = new[] { new SearchTestCase() { RawQuery = words.First(), TopOperator = SearchOperator.None } };
                var ca2 = new[] { new SearchTestCase() { RawQuery = words.Last(), TopOperator = SearchOperator.None } };
                list.AddRange(this.CombineCases(ca1, ca2, true, includeExceptionCase));
            }
            else if (l > 2)
            {
                for (int i = 1; i < l; i++)
                {
                    //[0,i) Combine with [i,l)
                    var ca1 = this.BuildUpCases(words.Take(i).ToList(), includeExceptionCase);
                    var ca2 = this.BuildUpCases(words.Skip(i).Take(l - i).ToList(), includeExceptionCase);
                    list.AddRange(this.CombineCases(ca1, ca2, true, includeExceptionCase));
                }
            }

            return list;
        }

        private IEnumerable<SearchTestCase> CombineCases(IEnumerable<SearchTestCase> leftCases, IEnumerable<SearchTestCase> rightCases, bool fullNotTest, bool includeExceptionCase)
        {
            List<SearchTestCase> cases = new List<SearchTestCase>();

            int combinationCount = 0;

            bool[] leftVariations = fullNotTest ? new bool[] { false, true } : new bool[] { false };
            bool[] rightVariations = new bool[] { false, true };

            this.CombinatorialEngineProvider.RunCombinations(
                leftCases, rightCases, new[] { SearchOperator.And, SearchOperator.Or }, leftVariations, rightVariations,
                (left, right, searchOperator, leftNot, rightNot) =>
                {
                    ++combinationCount;
                    string leftQuery = leftNot ? "NOT " + left.ParenedQuery(SearchOperator.Not) : left.ParenedQuery(searchOperator);
                    string rightQuery = rightNot ? "NOT " + right.ParenedQuery(SearchOperator.Not) : right.ParenedQuery(searchOperator);

                    if (includeExceptionCase)
                    {
                        switch (combinationCount % 7)
                        {
                            case 5:
                                leftQuery = leftQuery + "(";
                                break;
                            case 6:
                                leftQuery = leftQuery + " AND";
                                break;
                        }

                    }

                    switch (searchOperator)
                    {
                        case SearchOperator.Or:
                            cases.Add(new SearchTestCase { RawQuery = leftQuery + " OR " + rightQuery, TopOperator = SearchOperator.Or });
                            break;
                        case SearchOperator.And:
                            cases.Add(new SearchTestCase { RawQuery = leftQuery + (combinationCount % 2 == 0 ? " " : " AND ") + rightQuery, TopOperator = SearchOperator.And });
                            break;
                    }
                }
               );

            return cases;
        }

        private string GetResult(SearchTestCase testCase, IEnumerable<string> dic)
        {
            var builder = new StringBuilder();

            try
            {
                var clause = ParseSearch(testCase.RawQuery);
                var eval = clause.GetMatchFunc();
                List<string> pass = new List<string>();
                List<string> nass = new List<string>();
                foreach (var word in dic)
                {
                    if (eval(word))
                    {
                        pass.Add(word);
                    }
                    else
                    {
                        nass.Add(word);
                    }
                }

                builder.AppendLine("Match:" + string.Join(", ", pass) + ";");
                builder.AppendLine("Unmatch:" + string.Join(", ", nass) + ";");
            }
            catch (ODataException e)
            {
                builder.AppendLine("Exception:" + e.Message);
            }

            return builder.ToString();
        }

        private SearchClause ParseSearch(string queryOption)
        {
            ODataUriParser parser = new ODataUriParser(EdmCoreModel.Instance, ResourceRoot, new Uri(ResourceRoot, "?$search=" + queryOption));
            return parser.ParseSearch();
        }

        private void ApprovalVerify(string result)
        {
            Approvals.Verify(new ApprovalTextWriter(result), new CustomSourcePathNamer(this.TestContext.DeploymentDirectory), Approvals.GetReporter());
        }
    }
}
