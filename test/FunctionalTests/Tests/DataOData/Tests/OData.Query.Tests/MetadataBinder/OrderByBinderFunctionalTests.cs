//---------------------------------------------------------------------
// <copyright file="OrderByBinderFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Query.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// Tests for the MetadataBinder on a order-by expression.
    /// </summary>
    [TestClass, TestCase]
    public class OrderByBinderFunctionalTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public UntypedDataServiceProviderFactory UntypedDataServiceProviderFactory { get; set; }

        public class OrderByTestCase
        {
            public string[] OrderBy { get; set; }
            public SingleValueNode[] ExpectedOrderByExpressions { get; set; }
            public string ExpectedExceptionMessage { get; set; }
            public override string ToString() { return string.Join(", ", this.OrderBy); }
        }

        [TestMethod, Variation(Description = "Verifies correct binding of order-by.")]
        public void OrderByBinderTest()
        {
            IEdmModel model = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);

            // TODO: add an error test where the input collection to the order-by is a singleton

            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dummy", model.ResolveTypeReference("TestNS.Customer", false).AsEntity(), model.FindEntityContainer("BinderTestMetadata").FindEntitySet("Customers"));

            OrderByTestCase[] testCases = new OrderByTestCase[]
            {
                new OrderByTestCase()
                {
                    OrderBy = new string[] { "true" },
                    ExpectedOrderByExpressions = new SingleValueNode[]
                    {
                        new ConstantNode(true)
                    }
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "Name" },
                    ExpectedOrderByExpressions = new SingleValueNode[]
                    {
                        new SingleValuePropertyAccessNode(
                            new ResourceRangeVariableReferenceNode(entityRangeVariable.Name, entityRangeVariable),
                            model.ResolveProperty("TestNS.Customer.Name")
                        )
                    }
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "3" },
                    ExpectedOrderByExpressions = new SingleValueNode[]
                    {
                        new ConstantNode(3)
                    }
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "null" },
                    ExpectedOrderByExpressions = new SingleValueNode[]
                    {
                        new ConstantNode(null)
                    }
                },


                new OrderByTestCase()
                {
                    OrderBy = new string[] { "Address" },
                    ExpectedExceptionMessage = "The $orderby expression must evaluate to a single value of primitive type."
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "Emails" },
                    ExpectedExceptionMessage = "The $orderby expression must evaluate to a single value of primitive type."
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "NonExistant" },
                    ExpectedExceptionMessage = "Could not find a property named 'NonExistant' on type 'TestNS.Customer'."
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "Name", "ID" },
                    ExpectedOrderByExpressions = new SingleValueNode[]
                    {
                        new SingleValuePropertyAccessNode(
                            new ResourceRangeVariableReferenceNode(entityRangeVariable.Name, entityRangeVariable),
                            model.ResolveProperty("TestNS.Customer.Name")
                        ),
                        new SingleValuePropertyAccessNode(
                            new ResourceRangeVariableReferenceNode(entityRangeVariable.Name, entityRangeVariable), 
                            model.ResolveProperty("TestNS.Customer.ID")
                        ),
                    }
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "Name", "Address" },
                    ExpectedExceptionMessage = "The $orderby expression must evaluate to a single value of primitive type."
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "Name", "Emails" },
                    ExpectedExceptionMessage = "The $orderby expression must evaluate to a single value of primitive type."
                },

                new OrderByTestCase()
                {
                    OrderBy = new string[] { "Name", "NonExistant" },
                    ExpectedExceptionMessage = "Could not find a property named 'NonExistant' on type 'TestNS.Customer'."
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                new OrderByDirection?[] { null, OrderByDirection.Ascending, OrderByDirection.Descending },
                (testCase, direction) =>
                {
                    string orderByDirection = direction == null
                        ? string.Empty
                        : direction == OrderByDirection.Ascending
                            ? " asc"
                            : " desc";
                    
                    this.Assert.IsTrue(testCase.OrderBy.Length > 0, "Need at least one order-by expression");

                    StringBuilder queryBuilder = new StringBuilder("/Customers?$orderby=");
                    for (int i = 0; i < testCase.OrderBy.Length; ++i)
                    {
                        if (i > 0)
                        {
                            queryBuilder.Append(", ");
                        }
                        queryBuilder.Append(testCase.OrderBy[i]);
                        queryBuilder.Append(orderByDirection);
                    }

                    TestExceptionUtils.ExpectedException<ODataException>(
                        this.Assert,
                        () =>
                        {
                            ODataUri actual = QueryNodeUtils.BindQuery(queryBuilder.ToString(), model);

                            // construct the expected order-by node
                            OrderByClause orderByNode = null;
                            for (int i = testCase.ExpectedOrderByExpressions.Length - 1; i >= 0; --i)
                            {
                                orderByNode = new OrderByClause(
                                    orderByNode,
                                    testCase.ExpectedOrderByExpressions[i],
                                    direction ?? OrderByDirection.Ascending,
                                    new ResourceRangeVariable(ExpressionConstants.It, model.ResolveTypeReference("TestNS.Customer", false).AsEntity(), model.FindEntityContainer("BinderTestMetadata").FindEntitySet("Customers"))
                                );
                            }

                            QueryNodeUtils.VerifyOrderByClauseAreEqual(
                                orderByNode,
                                actual.OrderBy,
                                this.Assert);
                        },
                        testCase.ExpectedExceptionMessage,
                        null);
                });
        }
    }
}
