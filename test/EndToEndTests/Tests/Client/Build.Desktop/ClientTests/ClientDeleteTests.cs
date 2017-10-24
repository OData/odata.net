//---------------------------------------------------------------------
// <copyright file="ClientDeleteTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System.Linq;
    using System.Net.Http;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Generic client delete test cases.
    /// </summary>
    [TestClass]
    public class ClientDeleteTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        public ClientDeleteTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void ExecuteDeleteMethod()
        {
            DataServiceQuery query =  this.TestClientContext.People.Where(p => p.PersonID == 1).Select(p => p.Parent) as DataServiceQuery;
            var response = this.TestClientContext.Execute(query.RequestUri, HttpMethod.Delete.Method);
            Assert.AreEqual(204, response.StatusCode);
        }
#endif
    }
}
