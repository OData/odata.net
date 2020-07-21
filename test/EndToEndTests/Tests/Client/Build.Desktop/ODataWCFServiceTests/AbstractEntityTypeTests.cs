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
    using Xunit;

    public class AbstractEntityTypeTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public AbstractEntityTypeTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [Fact]
        public void FunctionReturnDifferentTypes()
        {
            var customer = this.TestClientContext.Customers.First();
            var results = customer.getOrderAndOrderDetails().Execute().ToList();

            Assert.Equal(2, results.Count);
            Assert.True(results[0] is Order);
            Assert.True(results[1] is OrderDetail);

            //var details = customer.getOrderAndOrderDetails().OfType<OrderDetail>().ToList();
            //Assert.Equal(1, details.Count);
        }
    }
}
