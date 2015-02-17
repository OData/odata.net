//---------------------------------------------------------------------
// <copyright file="CombinatorialLinqExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.TestCodeTests
{
    #region Namespaces
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for using the combinatorial Linq extension methods.
    /// </summary>
    [TestClass, TestCase]
    public class CombinatorialLinqExtensionsTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestMethod, Variation(Description = "Verifies that we can properly create combinations without duplicates.")]
        public void CombinationsWithoutDuplicatesTest()
        {
            int[] items = new int[] { 1, 2, 3, 4 };

            var testCases = new []
                {
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 1 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 3 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 4 },
                            new int[] { 2, 3, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0, 1, 3 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 4 },
                            new int[] { 2, 3, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 4 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1, 2, 3, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0, 4 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1, 2, 3, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[0],
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                            new int[] { 1, 2 },
                            new int[] { 1, 3 },
                            new int[] { 1, 4 },
                            new int[] { 2, 3 },
                            new int[] { 2, 4 },
                            new int[] { 3, 4 },
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 4 },
                            new int[] { 2, 3, 4 },
                            new int[] { 1, 2, 3, 4 },
                        }
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    int[][] results = testCase.Items.Combinations(false, testCase.Lengths).ToArray();

                    this.Assert.IsNotNull(results, "results != null");
                    this.Assert.AreEqual(testCase.ExpectedCombinations.Length, results.Length, "Did not find expected number of combinations.");

                    for (int i = 0; i < results.Length; ++i)
                    {
                        VerificationUtils.VerifyEnumerationsAreEqual(testCase.ExpectedCombinations[i], results[i], this.Assert);
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly create combinations with duplicates.")]
        public void CombinationsWithDuplicatesTest()
        {
            int[] items = new int[] { 1, 2, 3, 4 };

            var testCases = new[]
                {
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 1 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 3 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1, 1, 1 },
                            new int[] { 1, 1, 2 },
                            new int[] { 1, 1, 3 },
                            new int[] { 1, 1, 4 },
                            new int[] { 1, 2, 2 },
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 3 },
                            new int[] { 1, 3, 4 },
                            new int[] { 1, 4, 4 },
                            new int[] { 2, 2, 2 },
                            new int[] { 2, 2, 3 },
                            new int[] { 2, 2, 4 },
                            new int[] { 2, 3, 3 },
                            new int[] { 2, 3, 4 },
                            new int[] { 2, 4, 4 },
                            new int[] { 3, 3, 3 },
                            new int[] { 3, 3, 4 },
                            new int[] { 3, 4, 4 },
                            new int[] { 4, 4, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0, 1, 3 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                            new int[] { 1, 1, 1 },
                            new int[] { 1, 1, 2 },
                            new int[] { 1, 1, 3 },
                            new int[] { 1, 1, 4 },
                            new int[] { 1, 2, 2 },
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 3 },
                            new int[] { 1, 3, 4 },
                            new int[] { 1, 4, 4 },
                            new int[] { 2, 2, 2 },
                            new int[] { 2, 2, 3 },
                            new int[] { 2, 2, 4 },
                            new int[] { 2, 3, 3 },
                            new int[] { 2, 3, 4 },
                            new int[] { 2, 4, 4 },
                            new int[] { 3, 3, 3 },
                            new int[] { 3, 3, 4 },
                            new int[] { 3, 4, 4 },
                            new int[] { 4, 4, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 4 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1, 1, 1, 1 },
                            new int[] { 1, 1, 1, 2 },
                            new int[] { 1, 1, 1, 3 },
                            new int[] { 1, 1, 1, 4 },
                            new int[] { 1, 1, 2, 2 },
                            new int[] { 1, 1, 2, 3 },
                            new int[] { 1, 1, 2, 4 },
                            new int[] { 1, 1, 3, 3 },
                            new int[] { 1, 1, 3, 4 },
                            new int[] { 1, 1, 4, 4 },
                            new int[] { 1, 2, 2, 2 },
                            new int[] { 1, 2, 2, 3 },
                            new int[] { 1, 2, 2, 4 },
                            new int[] { 1, 2, 3, 3 },
                            new int[] { 1, 2, 3, 4 },
                            new int[] { 1, 2, 4, 4 },
                            new int[] { 1, 3, 3, 3 },
                            new int[] { 1, 3, 3, 4 },
                            new int[] { 1, 3, 4, 4 },
                            new int[] { 1, 4, 4, 4 },
                            new int[] { 2, 2, 2, 2 },
                            new int[] { 2, 2, 2, 3 },
                            new int[] { 2, 2, 2, 4 },
                            new int[] { 2, 2, 3, 3 },
                            new int[] { 2, 2, 3, 4 },
                            new int[] { 2, 2, 4, 4 },
                            new int[] { 2, 3, 3, 3 },
                            new int[] { 2, 3, 3, 4 },
                            new int[] { 2, 3, 4, 4 },
                            new int[] { 2, 4, 4, 4 },
                            new int[] { 3, 3, 3, 3 },
                            new int[] { 3, 3, 3, 4 },
                            new int[] { 3, 3, 4, 4 },
                            new int[] { 3, 4, 4, 4 },
                            new int[] { 4, 4, 4, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0, 4 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1, 1, 1, 1 },
                            new int[] { 1, 1, 1, 2 },
                            new int[] { 1, 1, 1, 3 },
                            new int[] { 1, 1, 1, 4 },
                            new int[] { 1, 1, 2, 2 },
                            new int[] { 1, 1, 2, 3 },
                            new int[] { 1, 1, 2, 4 },
                            new int[] { 1, 1, 3, 3 },
                            new int[] { 1, 1, 3, 4 },
                            new int[] { 1, 1, 4, 4 },
                            new int[] { 1, 2, 2, 2 },
                            new int[] { 1, 2, 2, 3 },
                            new int[] { 1, 2, 2, 4 },
                            new int[] { 1, 2, 3, 3 },
                            new int[] { 1, 2, 3, 4 },
                            new int[] { 1, 2, 4, 4 },
                            new int[] { 1, 3, 3, 3 },
                            new int[] { 1, 3, 3, 4 },
                            new int[] { 1, 3, 4, 4 },
                            new int[] { 1, 4, 4, 4 },
                            new int[] { 2, 2, 2, 2 },
                            new int[] { 2, 2, 2, 3 },
                            new int[] { 2, 2, 2, 4 },
                            new int[] { 2, 2, 3, 3 },
                            new int[] { 2, 2, 3, 4 },
                            new int[] { 2, 2, 4, 4 },
                            new int[] { 2, 3, 3, 3 },
                            new int[] { 2, 3, 3, 4 },
                            new int[] { 2, 3, 4, 4 },
                            new int[] { 2, 4, 4, 4 },
                            new int[] { 3, 3, 3, 3 },
                            new int[] { 3, 3, 3, 4 },
                            new int[] { 3, 3, 4, 4 },
                            new int[] { 3, 4, 4, 4 },
                            new int[] { 4, 4, 4, 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[0],
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                            new int[] { 1, 1 },
                            new int[] { 1, 2 },
                            new int[] { 1, 3 },
                            new int[] { 1, 4 },
                            new int[] { 2, 2 },
                            new int[] { 2, 3 },
                            new int[] { 2, 4 },
                            new int[] { 3, 3 },
                            new int[] { 3, 4 },
                            new int[] { 4, 4 },
                            new int[] { 1, 1, 1 },
                            new int[] { 1, 1, 2 },
                            new int[] { 1, 1, 3 },
                            new int[] { 1, 1, 4 },
                            new int[] { 1, 2, 2 },
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 3 },
                            new int[] { 1, 3, 4 },
                            new int[] { 1, 4, 4 },
                            new int[] { 2, 2, 2 },
                            new int[] { 2, 2, 3 },
                            new int[] { 2, 2, 4 },
                            new int[] { 2, 3, 3 },
                            new int[] { 2, 3, 4 },
                            new int[] { 2, 4, 4 },
                            new int[] { 3, 3, 3 },
                            new int[] { 3, 3, 4 },
                            new int[] { 3, 4, 4 },
                            new int[] { 4, 4, 4 },
                            new int[] { 1, 1, 1, 1 },
                            new int[] { 1, 1, 1, 2 },
                            new int[] { 1, 1, 1, 3 },
                            new int[] { 1, 1, 1, 4 },
                            new int[] { 1, 1, 2, 2 },
                            new int[] { 1, 1, 2, 3 },
                            new int[] { 1, 1, 2, 4 },
                            new int[] { 1, 1, 3, 3 },
                            new int[] { 1, 1, 3, 4 },
                            new int[] { 1, 1, 4, 4 },
                            new int[] { 1, 2, 2, 2 },
                            new int[] { 1, 2, 2, 3 },
                            new int[] { 1, 2, 2, 4 },
                            new int[] { 1, 2, 3, 3 },
                            new int[] { 1, 2, 3, 4 },
                            new int[] { 1, 2, 4, 4 },
                            new int[] { 1, 3, 3, 3 },
                            new int[] { 1, 3, 3, 4 },
                            new int[] { 1, 3, 4, 4 },
                            new int[] { 1, 4, 4, 4 },
                            new int[] { 2, 2, 2, 2 },
                            new int[] { 2, 2, 2, 3 },
                            new int[] { 2, 2, 2, 4 },
                            new int[] { 2, 2, 3, 3 },
                            new int[] { 2, 2, 3, 4 },
                            new int[] { 2, 2, 4, 4 },
                            new int[] { 2, 3, 3, 3 },
                            new int[] { 2, 3, 3, 4 },
                            new int[] { 2, 3, 4, 4 },
                            new int[] { 2, 4, 4, 4 },
                            new int[] { 3, 3, 3, 3 },
                            new int[] { 3, 3, 3, 4 },
                            new int[] { 3, 3, 4, 4 },
                            new int[] { 3, 4, 4, 4 },
                            new int[] { 4, 4, 4, 4 },
                        }
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    int[][] results = testCase.Items.Combinations(true, testCase.Lengths).ToArray();

                    this.Assert.IsNotNull(results, "results != null");
                    this.Assert.AreEqual(testCase.ExpectedCombinations.Length, results.Length, "Did not find expected number of variations.");

                    for (int i = 0; i < results.Length; ++i)
                    {
                        VerificationUtils.VerifyEnumerationsAreEqual(testCase.ExpectedCombinations[i], results[i], this.Assert);
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly create variations.")]
        public void VariationsTest()
        {
            int[] items = new int[] { 1, 2, 3, 4 };

            var testCases = new[]
                {
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 1 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 3 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 2 },
                            new int[] { 1, 3, 4 },
                            new int[] { 1, 4, 2 },
                            new int[] { 1, 4, 3 },
                            new int[] { 2, 1, 3 },
                            new int[] { 2, 1, 4 },
                            new int[] { 2, 3, 1 },
                            new int[] { 2, 3, 4 },
                            new int[] { 2, 4, 1 },
                            new int[] { 2, 4, 3 },
                            new int[] { 3, 1, 2 },
                            new int[] { 3, 1, 4 },
                            new int[] { 3, 2, 1 },
                            new int[] { 3, 2, 4 },
                            new int[] { 3, 4, 1 },
                            new int[] { 3, 4, 2 },
                            new int[] { 4, 1, 2 },
                            new int[] { 4, 1, 3 },
                            new int[] { 4, 2, 1 },
                            new int[] { 4, 2, 3 },
                            new int[] { 4, 3, 1 },
                            new int[] { 4, 3, 2 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0, 1, 3 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 2 },
                            new int[] { 1, 3, 4 },
                            new int[] { 1, 4, 2 },
                            new int[] { 1, 4, 3 },
                            new int[] { 2, 1, 3 },
                            new int[] { 2, 1, 4 },
                            new int[] { 2, 3, 1 },
                            new int[] { 2, 3, 4 },
                            new int[] { 2, 4, 1 },
                            new int[] { 2, 4, 3 },
                            new int[] { 3, 1, 2 },
                            new int[] { 3, 1, 4 },
                            new int[] { 3, 2, 1 },
                            new int[] { 3, 2, 4 },
                            new int[] { 3, 4, 1 },
                            new int[] { 3, 4, 2 },
                            new int[] { 4, 1, 2 },
                            new int[] { 4, 1, 3 },
                            new int[] { 4, 2, 1 },
                            new int[] { 4, 2, 3 },
                            new int[] { 4, 3, 1 },
                            new int[] { 4, 3, 2 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 4 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1, 2, 3, 4 },
                            new int[] { 1, 2, 4, 3 },
                            new int[] { 1, 3, 2, 4 },
                            new int[] { 1, 3, 4, 2 },
                            new int[] { 1, 4, 2, 3 },
                            new int[] { 1, 4, 3, 2 },
                            new int[] { 2, 1, 3, 4 },
                            new int[] { 2, 1, 4, 3 },
                            new int[] { 2, 3, 1, 4 },
                            new int[] { 2, 3, 4, 1 },
                            new int[] { 2, 4, 1, 3 },
                            new int[] { 2, 4, 3, 1 },
                            new int[] { 3, 1, 2, 4 },
                            new int[] { 3, 1, 4, 2 },
                            new int[] { 3, 2, 1, 4 },
                            new int[] { 3, 2, 4, 1 },
                            new int[] { 3, 4, 1, 2 },
                            new int[] { 3, 4, 2, 1 },
                            new int[] { 4, 1, 2, 3 },
                            new int[] { 4, 1, 3, 2 },
                            new int[] { 4, 2, 1, 3 },
                            new int[] { 4, 2, 3, 1 },
                            new int[] { 4, 3, 1, 2 },
                            new int[] { 4, 3, 2, 1 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[] { 0, 4 },
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1, 2, 3, 4 },
                            new int[] { 1, 2, 4, 3 },
                            new int[] { 1, 3, 2, 4 },
                            new int[] { 1, 3, 4, 2 },
                            new int[] { 1, 4, 2, 3 },
                            new int[] { 1, 4, 3, 2 },
                            new int[] { 2, 1, 3, 4 },
                            new int[] { 2, 1, 4, 3 },
                            new int[] { 2, 3, 1, 4 },
                            new int[] { 2, 3, 4, 1 },
                            new int[] { 2, 4, 1, 3 },
                            new int[] { 2, 4, 3, 1 },
                            new int[] { 3, 1, 2, 4 },
                            new int[] { 3, 1, 4, 2 },
                            new int[] { 3, 2, 1, 4 },
                            new int[] { 3, 2, 4, 1 },
                            new int[] { 3, 4, 1, 2 },
                            new int[] { 3, 4, 2, 1 },
                            new int[] { 4, 1, 2, 3 },
                            new int[] { 4, 1, 3, 2 },
                            new int[] { 4, 2, 1, 3 },
                            new int[] { 4, 2, 3, 1 },
                            new int[] { 4, 3, 1, 2 },
                            new int[] { 4, 3, 2, 1 },
                        }
                    },
                    new
                    {
                        Items = items,
                        Lengths = new int[0],
                        ExpectedCombinations = new int[][]
                        {
                            new int[0],
                            new int[] { 1 },
                            new int[] { 2 },
                            new int[] { 3 },
                            new int[] { 4 },
                            new int[] { 1, 2 },
                            new int[] { 1, 3 },
                            new int[] { 1, 4 },
                            new int[] { 2, 1 },
                            new int[] { 2, 3 },
                            new int[] { 2, 4 },
                            new int[] { 3, 1 },
                            new int[] { 3, 2 },
                            new int[] { 3, 4 },
                            new int[] { 4, 1 },
                            new int[] { 4, 2 },
                            new int[] { 4, 3 },
                            new int[] { 1, 2, 3 },
                            new int[] { 1, 2, 4 },
                            new int[] { 1, 3, 2 },
                            new int[] { 1, 3, 4 },
                            new int[] { 1, 4, 2 },
                            new int[] { 1, 4, 3 },
                            new int[] { 2, 1, 3 },
                            new int[] { 2, 1, 4 },
                            new int[] { 2, 3, 1 },
                            new int[] { 2, 3, 4 },
                            new int[] { 2, 4, 1 },
                            new int[] { 2, 4, 3 },
                            new int[] { 3, 1, 2 },
                            new int[] { 3, 1, 4 },
                            new int[] { 3, 2, 1 },
                            new int[] { 3, 2, 4 },
                            new int[] { 3, 4, 1 },
                            new int[] { 3, 4, 2 },
                            new int[] { 4, 1, 2 },
                            new int[] { 4, 1, 3 },
                            new int[] { 4, 2, 1 },
                            new int[] { 4, 2, 3 },
                            new int[] { 4, 3, 1 },
                            new int[] { 4, 3, 2 },
                            new int[] { 1, 2, 3, 4 },
                            new int[] { 1, 2, 4, 3 },
                            new int[] { 1, 3, 2, 4 },
                            new int[] { 1, 3, 4, 2 },
                            new int[] { 1, 4, 2, 3 },
                            new int[] { 1, 4, 3, 2 },
                            new int[] { 2, 1, 3, 4 },
                            new int[] { 2, 1, 4, 3 },
                            new int[] { 2, 3, 1, 4 },
                            new int[] { 2, 3, 4, 1 },
                            new int[] { 2, 4, 1, 3 },
                            new int[] { 2, 4, 3, 1 },
                            new int[] { 3, 1, 2, 4 },
                            new int[] { 3, 1, 4, 2 },
                            new int[] { 3, 2, 1, 4 },
                            new int[] { 3, 2, 4, 1 },
                            new int[] { 3, 4, 1, 2 },
                            new int[] { 3, 4, 2, 1 },
                            new int[] { 4, 1, 2, 3 },
                            new int[] { 4, 1, 3, 2 },
                            new int[] { 4, 2, 1, 3 },
                            new int[] { 4, 2, 3, 1 },
                            new int[] { 4, 3, 1, 2 },
                            new int[] { 4, 3, 2, 1 },
                        }
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    int[][] results = testCase.Items.Variations(testCase.Lengths).ToArray();

                    this.Assert.IsNotNull(results, "results != null");
                    this.Assert.AreEqual(testCase.ExpectedCombinations.Length, results.Length, "Did not find expected number of combinations.");

                    for (int i = 0; i < results.Length; ++i)
                    {
                        VerificationUtils.VerifyEnumerationsAreEqual(testCase.ExpectedCombinations[i], results[i], this.Assert);
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly create permutations.")]
        public void PermutationsTest()
        {
            int[] items = new int[] { 1, 2, 3, 4 };

            var testCases = new[]
                {
                    new
                    {
                        Items = items,
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { 1, 2, 3, 4 },
                            new int[] { 1, 2, 4, 3 },
                            new int[] { 1, 3, 2, 4 },
                            new int[] { 1, 3, 4, 2 },
                            new int[] { 1, 4, 2, 3 },
                            new int[] { 1, 4, 3, 2 },
                            new int[] { 2, 1, 3, 4 },
                            new int[] { 2, 1, 4, 3 },
                            new int[] { 2, 3, 1, 4 },
                            new int[] { 2, 3, 4, 1 },
                            new int[] { 2, 4, 1, 3 },
                            new int[] { 2, 4, 3, 1 },
                            new int[] { 3, 1, 2, 4 },
                            new int[] { 3, 1, 4, 2 },
                            new int[] { 3, 2, 1, 4 },
                            new int[] { 3, 2, 4, 1 },
                            new int[] { 3, 4, 1, 2 },
                            new int[] { 3, 4, 2, 1 },
                            new int[] { 4, 1, 2, 3 },
                            new int[] { 4, 1, 3, 2 },
                            new int[] { 4, 2, 1, 3 },
                            new int[] { 4, 2, 3, 1 },
                            new int[] { 4, 3, 1, 2 },
                            new int[] { 4, 3, 2, 1 },
                        }
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    int[][] results = testCase.Items.Permutations().ToArray();

                    this.Assert.IsNotNull(results, "results != null");
                    this.Assert.AreEqual(testCase.ExpectedCombinations.Length, results.Length, "Did not find expected number of permutations.");

                    for (int i = 0; i < results.Length; ++i)
                    {
                        VerificationUtils.VerifyEnumerationsAreEqual(testCase.ExpectedCombinations[i], results[i], this.Assert);
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies that we can properly create column combinations.")]
        public void ColumnCombinationsTest()
        {
            int[] rowOneToFour = new int[] { 1, 2, 3, 4 };
            int[] rowFiveToSeven = new int[] { 5, 6, 7 };
            int[] rowEightToNine = new int[] { 8, 9 };
            int[] rowTenToFourteen = new int[] { 10, 11, 12, 13, 14 };

            var testCases = new[]
                {
                    new
                    {
                        Items = new int[0][],
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { },
                        }
                    },
                    new
                    {
                        Items = new int[][]
                        {
                            rowOneToFour,
                            rowFiveToSeven
                        },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { },
                            new int[] { 1 },
                            new int[] { 5 },
                            new int[] { 1, 5 },
                            new int[] { 2 },
                            new int[] { 6 },
                            new int[] { 2, 6 },
                            new int[] { 3 },
                            new int[] { 7 },
                            new int[] { 3, 7 },
                            new int[] { 4 },
                        }
                    },
                    new
                    {
                        Items = new int[][]
                        {
                            rowOneToFour,
                            rowFiveToSeven,
                            rowEightToNine,
                            rowTenToFourteen
                        },
                        ExpectedCombinations = new int[][]
                        {
                            new int[] { },
                            new int[] { 1 },
                            new int[] { 5 },
                            new int[] { 8 },
                            new int[] { 10 },
                            new int[] { 1, 5 },
                            new int[] { 1, 8 },
                            new int[] { 1, 10 },
                            new int[] { 5, 8 },
                            new int[] { 5, 10 },
                            new int[] { 8, 10 },
                            new int[] { 1, 5, 8 },
                            new int[] { 1, 5, 10 },
                            new int[] { 1, 8, 10 },
                            new int[] { 5, 8, 10 },
                            new int[] { 1, 5, 8, 10 },
                            new int[] { 2 },
                            new int[] { 6 },
                            new int[] { 9 },
                            new int[] { 11 },
                            new int[] { 2, 6 },
                            new int[] { 2, 9 },
                            new int[] { 2, 11 },
                            new int[] { 6, 9 },
                            new int[] { 6, 11 },
                            new int[] { 9, 11 },
                            new int[] { 2, 6, 9 },
                            new int[] { 2, 6, 11 },
                            new int[] { 2, 9, 11 },
                            new int[] { 6, 9, 11 },
                            new int[] { 2, 6, 9, 11 },
                            new int[] { 3 },
                            new int[] { 7 },
                            new int[] { 12 },
                            new int[] { 3, 7 },
                            new int[] { 3, 12 },
                            new int[] { 7, 12 },
                            new int[] { 3, 7, 12 },
                            new int[] { 4 },
                            new int[] { 13 },
                            new int[] { 4, 13 },
                            new int[] { 14 },
                        }
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    int[][] results = testCase.Items.ColumnCombinations().ToArray();

                    this.Assert.IsNotNull(results, "results != null");
                    this.Assert.AreEqual(testCase.ExpectedCombinations.Length, results.Length, "Did not find expected number of column combinations.");

                    for (int i = 0; i < results.Length; ++i)
                    {
                        VerificationUtils.VerifyEnumerationsAreEqual(testCase.ExpectedCombinations[i], results[i], this.Assert);
                    }
                });
        }
    }
}