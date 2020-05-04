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
    using Xunit.Abstractions;
    using Xunit;

    public class DefaultUrlConventionsTests : EndToEndTestBase
    {
        public DefaultUrlConventionsTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.ODataWCFServiceDescriptor, helper)
        {
        }

        [Fact]
        public void ClientWithKeyAsSegmentSendsRequestsToServerWithoutKeyAsSegment()
        {
            try
            {
                var contextWrapper = this.CreateWrappedContext<InMemoryEntities>();

                contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

                contextWrapper.Context.Orders.Where(c => c.OrderID == 0).ToArray();
                Assert.True(false, "Expected DataServiceException was not thrown.");
            }
            catch (DataServiceQueryException ex)
            {
                Assert.NotNull(ex.InnerException);
                Assert.IsType<DataServiceClientException>(ex.InnerException);
                StringResourceUtil.VerifyDataServicesString(ClientExceptionUtil.ExtractServerErrorMessage(ex), "RequestUriProcessor_CannotQueryCollections", "Orders");
            }
        }
    }
}
