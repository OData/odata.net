//---------------------------------------------------------------------
// <copyright file="SkipTopBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Query.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// Tests for the MetadataBinder on a query with skip/top.
    /// </summary>
    [TestClass, TestCase]
    public class SkipTopBinderTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public UntypedDataServiceProviderFactory UntypedDataServiceProviderFactory { get; set; }

        public class SkipTopTestCase
        {
            public string Query { get; set; }
            internal long? ExpectedSkipClause { get; set; }
            internal long? ExpectedTopClause { get; set; }
            public string ExpectedExceptionMessage { get; set; }
        }

        [TestMethod, Variation(Description = "Verifies correct binding of queries with a skip operator.")]
        public void SkipAndTopBinderTest()
        {
            IEdmModel model = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);

            var testCases = new[]
            {
                new SkipTopTestCase
                {
                    Query = "/Customers?$skip=2",
                    ExpectedSkipClause = 2
                },
                new SkipTopTestCase
                {
                    Query = "/Customers?$top=2",
                    ExpectedTopClause = 2
                },
                new SkipTopTestCase
                {
                    Query = "/Customers?$skip=3&$top=2",
                    ExpectedTopClause = 2,
                    ExpectedSkipClause = 3
                },
                new SkipTopTestCase
                {
                    Query = "/Customers?$top=2&$skip=3",
                    ExpectedTopClause = 2,
                    ExpectedSkipClause = 3
                },
                // TODO: enable those?
                /* 
                new SkipTopTestCase
                {
                    Query = "/Customers(0)?$top=100",
                    ExpectedExceptionMessage = "The $top query option cannot be applied to the query path. Top can only be applied to a collection of entities."
                },
                new SkipTopTestCase
                {
                    Query = "/Customers(0)?$skip=100",
                    ExpectedExceptionMessage = "The $skip query option cannot be applied to the query path. Skip can only be applied to a collection of entities."
                },
                new SkipTopTestCase
                {
                    Query = "/Customers(0)?$skip=100&top=100",
                    ExpectedExceptionMessage = "The $skip query option cannot be applied to the query path. Skip can only be applied to a collection of entities."
                },
                new SkipTopTestCase
                {
                    Query = "/Customers(0)?top=100&$skip=100",
                    ExpectedExceptionMessage = "The $skip query option cannot be applied to the query path. Skip can only be applied to a collection of entities."
                },
                 */
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    TestExceptionUtils.ExpectedException<ODataException>(
                        this.Assert,
                        () =>
                        {
                            ODataUri actual = QueryNodeUtils.BindQuery(testCase.Query, model);
                            if (testCase.ExpectedSkipClause != null)
                            {
                                Assert.AreEqual(testCase.ExpectedSkipClause, actual.Skip, "Skip amounts are not equal!");
                            }
                            
                            if (testCase.ExpectedTopClause != null)
                            {
                                Assert.AreEqual(testCase.ExpectedTopClause, actual.Top, "Top amounts are not equal!");
                            }
                        },
                        testCase.ExpectedExceptionMessage,
                        null);
                });
        }
    }
}
