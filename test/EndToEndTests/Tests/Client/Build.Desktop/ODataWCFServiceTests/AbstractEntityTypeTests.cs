//---------------------------------------------------------------------
// <copyright file="ODataWCFServiceQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AbstractEntityTypeTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public AbstractEntityTypeTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [TestMethod]
        public void FunctionReturnDifferentTypes()
        {
            var customer = this.TestClientContext.Customers.First();
            var results = customer.getOrderAndOrderDetails().Execute().ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results[0] is Order);
            Assert.IsTrue(results[1] is OrderDetail);

            //var details = customer.getOrderAndOrderDetails().OfType<OrderDetail>().ToList();
            //Assert.AreEqual(1, details.Count);
        }
    }
}
