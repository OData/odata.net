//---------------------------------------------------------------------
// <copyright file="DefaultUrlConventionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultUrlConventionsTests : EndToEndTestBase
    {
        public DefaultUrlConventionsTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void ClientWithKeyAsSegmentSendsRequestsToServerWithoutKeyAsSegment()
        {
            try
            {
                var contextWrapper = this.CreateWrappedContext<InMemoryEntities>();

                contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

                contextWrapper.Context.Orders.Where(c => c.OrderID == 0).ToArray();
                Assert.Fail("Expected DataServiceException was not thrown.");
            }
            catch (DataServiceQueryException ex)
            {
                Assert.IsNotNull(ex.InnerException, "No inner exception found");
                Assert.IsInstanceOfType(ex.InnerException, typeof(DataServiceClientException), "Unexpected inner exception type");
                StringResourceUtil.VerifyDataServicesString(ClientExceptionUtil.ExtractServerErrorMessage(ex), "RequestUriProcessor_CannotQueryCollections", "Orders");
            }
        }
    }
}
