//---------------------------------------------------------------------
// <copyright file="BatchReaderHeadersTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests the BatchReaderHeaders implementation.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderHeadersTests : ODataReaderTestCase
    {
        public sealed class BatchReaderHeaderTestSettings
        {
            [InjectDependency(IsRequired = true)]
            public AssertionHandler Assert { get; set; }

            [InjectDependency(IsRequired = true)]
            public IExceptionVerifier ExceptionVerifier { get; set; }
        }

        [InjectDependency(IsRequired = true)]
        public BatchReaderHeaderTestSettings Settings { get; set; }

#if !SILVERLIGHT && !WINDOWS_PHONE
        // BatchReaderHeaders tests use private reflection and thus cannot run on Silverlight or the phone.
        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the BatchReaderHeaders implementation.")]
        public void BatchReaderHeadersTest()
        {
            IEnumerable<KeyValuePair<string, string>> invalidInitialHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ABC", "ABC"),
                new KeyValuePair<string, string>("ABC", "ABC"),
            };

            Dictionary<string, string> validInitialHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ABC", "ABC"},
                { "DEF", "DEF"},
                { "GHI", "GHI"},
            };

            IEnumerable<BatchHeadersTestCase> testCases = new BatchHeadersTestCase[]
            {
                // Duplicate case-sensitive keys will fail
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = invalidInitialHeaders,
                    ExpectedException = new ExpectedException(typeof(ArgumentException))
                },
                // Valid items should work
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    ExpectedHeaders = validInitialHeaders
                },
                // Replacing an item and adding a new one via the indexer should work
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        headers["ABC"] = "NewAbc";
                        headers["XYZ"] = "XYZ";
                    },
                    ExpectedHeaders = new Dictionary<string, string>(validInitialHeaders).Set("ABC", "NewAbc").With("XYZ", "XYZ")
                },
                // Adding new items via Add should work
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        headers.Add("ABc", "ABc");
                    },
                    ExpectedException = new ExpectedException(typeof(ArgumentException))
                },
                // Adding the same item should fail
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        headers.Add("ABC", "ABC");
                    },
                    ExpectedException = new ExpectedException(typeof(ArgumentException))
                },
                // Removing an item (case-sensitive match) should work
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        headers.Remove("ABC");
                    },
                    ExpectedHeaders = new Dictionary<string, string>(validInitialHeaders,StringComparer.OrdinalIgnoreCase).Without("ABC")
                },
                // Removing an item (case-insensitive match) should work
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        headers.Remove("ghi");
                    },
                    ExpectedHeaders = new Dictionary<string, string>(validInitialHeaders,StringComparer.OrdinalIgnoreCase).Without("GHI")
                },
                // Removing a non-existing item should work
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        headers.Remove("AAA");
                    },
                    ExpectedHeaders = validInitialHeaders
                },
                // ContainsKeyOrdinal should work for existing key
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        this.Settings.Assert.IsTrue(headers.ContainsKeyOrdinal("abc"), "Expected to find 'abc' in the headers.");
                    },
                    ExpectedHeaders = validInitialHeaders
                },
                // TryGetValue should work for existing key (case-sensitive match)
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        string v;
                        this.Assert.IsTrue(headers.TryGetValue("abc", out v), "TryGetValue should work for 'abc'.");
                        this.Assert.AreEqual("ABC", v, "Header values don't match.");
                    },
                    ExpectedHeaders = validInitialHeaders
                },
                // TryGetValue should work for existing key (single case-insensitive match)
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        string v;
                        this.Assert.IsTrue(headers.TryGetValue("ghi", out v), "TryGetValue should work for 'ghi'.");
                        this.Assert.AreEqual("GHI", v, "Header values don't match.");
                    },
                    ExpectedHeaders = validInitialHeaders
                },
                // TryGetValue should work for non-existing key
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        string v;
                        this.Assert.IsFalse(headers.TryGetValue("AAA", out v), "TryGetValue should work for 'AAA'.");
                    },
                    ExpectedHeaders = validInitialHeaders
                },
                // Indexer should work for existing key (case-sensitive match)
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        string v = headers["abc"];
                        this.Assert.AreEqual("ABC", v, "Header values don't match.");
                    },
                    ExpectedHeaders = validInitialHeaders
                },
                // Indexer should work for existing key (single case-insensitive match)
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        string v = headers["ghi"];
                        this.Assert.AreEqual("GHI", v, "Header values don't match.");
                    },
                    ExpectedHeaders = validInitialHeaders
                },
                // Indexer should fail for non-existing key
                new BatchHeadersTestCase(this.Settings)
                {
                    InitialHeaders = validInitialHeaders,
                    CustomizationFunc = headers => 
                    {
                        string v = headers["AAA"];
                    },
                    ExpectedException = new ExpectedException(typeof(KeyNotFoundException))
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, testCase => testCase.Run());
        }

        /// <summary>
        /// Test case class to test the ODataBatchOperationHeaders class.
        /// </summary>
        private sealed class BatchHeadersTestCase
        {
            /// <summary>The settings for the test case.</summary>
            private readonly BatchReaderHeaderTestSettings settings;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="settings">The settings for the test case.</param>
            public BatchHeadersTestCase(BatchReaderHeaderTestSettings settings)
            {
                this.settings = settings;
            }

            /// <summary>The set of initial header to initialize the batch headers with.</summary>
            public IEnumerable<KeyValuePair<string, string>> InitialHeaders { get; set; }

            /// <summary>Func to modify the headers after initialization.</summary>
            public Action<BatchOperationHeadersWrapper> CustomizationFunc { get; set; }

            /// <summary>The set of expected part headers.</summary>
            public IDictionary<string, string> ExpectedHeaders { get; set; }

            /// <summary>The expected exception.</summary>
            public ExpectedException ExpectedException { get; set; }

            public void Run()
            {
                BatchOperationHeadersWrapper headers = new BatchOperationHeadersWrapper();

                this.settings.Assert.ExpectedException(
                    () =>
                        {
                            if (this.InitialHeaders != null)
                            {
                                foreach (var kvp in this.InitialHeaders)
                                {
                                    headers.Add(kvp.Key, kvp.Value);
                                }
                            }

                            if (this.CustomizationFunc != null)
                            {
                                this.CustomizationFunc(headers);
                            }

                            this.VerifyResult(headers);
                        },
                    this.ExpectedException,
                    this.settings.ExceptionVerifier);
            }

            /// <summary>
            /// Verifies the result.
            /// </summary>
            /// <param name="headers">The headers read from the part.</param>
            private void VerifyResult(BatchOperationHeadersWrapper headers)
            {
                if (this.ExpectedHeaders != null)
                {
                    this.settings.Assert.IsNotNull(headers, string.Format("Expected {0} headers but none were found.", this.ExpectedHeaders.Count()));

                    foreach (KeyValuePair<string, string> expectedHeader in this.ExpectedHeaders)
                    {
                        // check that the expected header is present
                        if (!headers.ContainsKeyOrdinal(expectedHeader.Key))
                        {
                            this.settings.Assert.Fail(string.Format("Did not find expected header '{0}'.", expectedHeader.Key));
                        }

                        string actualValue;
                        headers.TryGetValue(expectedHeader.Key, out actualValue);
                        this.settings.Assert.AreEqual(expectedHeader.Value, actualValue, "Expected value '{0}' but found value '{1}'.", expectedHeader.Value, actualValue);
                    }

                    int headerCount = headers.Count();
                    this.settings.Assert.AreEqual(this.ExpectedHeaders.Count, headerCount, "Expected {0} headers but found {1}.", this.ExpectedHeaders.Count, headerCount);

                    // Make sure the enumerable works the same way in expected and actual enumerables
                    IEnumerator<KeyValuePair<string, string>> expectedEnumerator = this.ExpectedHeaders.GetEnumerator();
                    IEnumerator<KeyValuePair<string, string>> actualEnumerator = headers.GetEnumerator();
                    using (expectedEnumerator)
                    using (actualEnumerator)
                    {
                        while (expectedEnumerator.MoveNext())
                        {
                            this.settings.Assert.IsTrue(actualEnumerator.MoveNext(), "Differing numbers of items should have been checked before.");

                            this.settings.Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current, "Items don't match.");
                        }
                    }
                }
            }
        }
#endif
    }
}
