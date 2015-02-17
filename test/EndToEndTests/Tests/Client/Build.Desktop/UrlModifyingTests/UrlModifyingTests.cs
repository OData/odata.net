//---------------------------------------------------------------------
// <copyright file="UrlModifyingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.UrlModifyingTests
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UrlModifyingTests : EndToEndTestBase
    {
        public UrlModifyingTests()
            : base(ServiceDescriptors.UrlModifyingService)
        {
        }

        [TestMethod]
        public void ModifyQueryOptions()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            var personQuery = context.CreateQuery<Person>("Person");

            try
            {
                personQuery.Execute();
                Assert.Fail("ModifyingQueryOptionsShouldFail");
            }
            catch (DataServiceQueryException ex)
            {
                Assert.IsNotNull(ex.InnerException, "No inner exception found");
                Assert.IsInstanceOfType(ex.InnerException, typeof(DataServiceClientException), "Unexpected inner exception type");

                StringResourceUtil.VerifyDataServicesString(ClientExceptionUtil.ExtractServerErrorMessage(ex),
                    "AstoriaRequestMessage_CannotChangeQueryString");
            }
        }

        [TestMethod]
        public void RemapRequestUri()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            DataServiceQuery customQuery = context.CreateQuery<Customer>("RemapPath");
            //Path should remap to the customers set

            var retrievedCustomers = (IEnumerable<Customer>)customQuery.Execute();
            Assert.IsNotNull(retrievedCustomers);
        }

        [TestMethod]
        public void RemapBase()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            DataServiceQuery customQuery = context.CreateQuery<Customer>("RemapBase");
            //Path should remap to the customers set
            var retrievedPersons = (IEnumerable<Customer>)customQuery.Execute();

            Assert.IsNotNull(retrievedPersons);
            EntityDescriptor descriptor = null;
            foreach (var element in retrievedPersons)
            {
                descriptor = context.GetEntityDescriptor(element);
                Assert.IsTrue(descriptor.EditLink.AbsoluteUri.StartsWith("http://potato"));
            }
        }

        [TestMethod]
        public void RemapBaseAndPathSeparately()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            DataServiceQuery customQuery = context.CreateQuery<Customer>("RemapBaseAndPathSeparately");
            //Path should remap to the customers set
            var retrievedPersons = (IEnumerable<Customer>)customQuery.Execute();

            Assert.IsNotNull(retrievedPersons);
            EntityDescriptor descriptor = null;
            foreach (var element in retrievedPersons)
            {
                descriptor = context.GetEntityDescriptor(element);
                Assert.IsTrue(descriptor.EditLink.AbsoluteUri.StartsWith("http://potato"));
            }
        }

        [TestMethod]
        public void BasesDontMatchFail()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();

            DataServiceQuery customQuery = context.CreateQuery<Customer>("BasesDontMatchFail");
            //Path should remap to the customers set
            var expectedServiceUrl = "http://potato:9090/FailMeService/";
            var expectedRequestUrl = "http://potato:9090/DontFailMeService/Customer";
            try
            {
                customQuery.Execute();
                Assert.Fail("Different service bases between service uri and request uri should fail");
            }
            catch (DataServiceQueryException ex)
            {
                Assert.IsNotNull(ex.InnerException, "No inner exception found");
                Assert.IsInstanceOfType(ex.InnerException, typeof(DataServiceClientException), "Unexpected inner exception type");

                StringResourceUtil.VerifyODataLibString(ClientExceptionUtil.ExtractServerErrorMessage(ex),
                    "UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri", true, expectedRequestUrl, expectedServiceUrl);
            }
        }

        [TestMethod]
        public void BatchRequest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            //Setup queries
            DataServiceRequest[] reqs = new DataServiceRequest[] {
                context.CreateQuery<Customer>("BatchRequest1"),
                context.CreateQuery<Person>("BatchRequest2"),
            };

            var response = context.ExecuteBatch(reqs);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsBatchResponse);
            foreach (var item in response)
            {
                Assert.IsTrue(item.StatusCode == 200);
            }
        }


        [TestMethod]
        public void BatchRequestBaseUriDifferentBetweenBatchAndRequest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();

            //Setup queries
            DataServiceRequest[] reqs = new DataServiceRequest[] {
                context.CreateQuery<Customer>("BatchRequest3"),
            };

            var response = context.ExecuteBatch(reqs);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsBatchResponse);
            foreach (QueryOperationResponse item in response)
            {
                Assert.IsNotNull(item.Error);

                Assert.IsInstanceOfType(item.Error, typeof(DataServiceClientException), "Unexpected inner exception type");
                var ex = item.Error as DataServiceClientException;
                StringResourceUtil.VerifyDataServicesString(ClientExceptionUtil.ExtractServerErrorMessage(item), "DataServiceOperationContext_CannotModifyServiceUriInsideBatch");
            }
        }
    }
}
