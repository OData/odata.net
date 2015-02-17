//---------------------------------------------------------------------
// <copyright file="CustomQueryOptionBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    #region Namespaces
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Query;
    using Microsoft.OData.Core.Query.SemanticAst;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Query.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the MetadataBinder on a query with custom query options.
    /// </summary>
    [TestClass, TestCase]
    public class CustomQueryOptionBinderTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public UntypedDataServiceProviderFactory UntypedDataServiceProviderFactory { get; set; }

        internal class QueryOptionTestCase
        {
            public string Query { get; set; }
            public CustomQueryOptionNode[] ExpectedQueryOptions { get; set; }
            public string ExpectedExceptionMessage { get; set; }
        }

        [TestMethod, Variation(Description = "Verifies correct binding of queries with custom query options.")]
        public void CustomQueryOptionBinderTest()
        {
            var metadata = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);

            var testCases = new[]
            {
                new QueryOptionTestCase
                {
                    Query = "/Customers?$foo=12",
                    ExpectedExceptionMessage="The system query option '$foo' is not supported."
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?a=b",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode("a","b")
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?a=b&c=d",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode("a", "b"),
                        new CustomQueryOptionNode("c", "d" )
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?a=b&a=c",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode("a","b"),
                        new CustomQueryOptionNode("a","c")
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?foo",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode(null,"foo")
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?foo=",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode("foo",string.Empty)
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?&",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode(null,string.Empty),
                        new CustomQueryOptionNode(null,string.Empty)
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?a=b&",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode("a","b" ),
                        new CustomQueryOptionNode(null,string.Empty)
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?&&",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode(null,string.Empty),
                        new CustomQueryOptionNode(null,string.Empty),
                        new CustomQueryOptionNode(null,string.Empty)
                    }
                },
                new QueryOptionTestCase
                {
                    Query = "/Customers?a=b&&",
                    ExpectedQueryOptions = new CustomQueryOptionNode[]
                    {
                        new CustomQueryOptionNode("a","b"),
                        new CustomQueryOptionNode(null, string.Empty),
                        new CustomQueryOptionNode(null, string.Empty)
                    }
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    TestExceptionUtils.ExpectedException<ODataException>(
                        this.Assert,
                        () =>
                        {
                            SemanticTree actual = QueryNodeUtils.BindQuery(testCase.Query, metadata);
                            QueryNodeUtils.VerifyQueryNodesAreEqual(
                                testCase.ExpectedQueryOptions,
                                actual.CustomQueryOptions,
                                this.Assert);
                        },
                        testCase.ExpectedExceptionMessage,
                        null);
                });
        }
    }
}
