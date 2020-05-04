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
    using Xunit;
    using Xunit.Abstractions;

    public class UrlModifyingTests : EndToEndTestBase
    {
        public UrlModifyingTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.UrlModifyingService, helper)
        {
        }

        [Fact]
        public void ModifyQueryOptions()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            var personQuery = context.CreateQuery<Person>("Person");

            try
            {
                personQuery.Execute();
                Assert.True(false, "ModifyingQueryOptionsShouldFail");
            }
            catch (DataServiceQueryException ex)
            {
                //No inner exception found
                Assert.NotNull(ex.InnerException);
                //Unexpected inner exception type
                Assert.IsType<DataServiceClientException>(ex.InnerException);
                StringResourceUtil.VerifyDataServicesString(ClientExceptionUtil.ExtractServerErrorMessage(ex),
                    "AstoriaRequestMessage_CannotChangeQueryString");
            }
        }

        [Fact]
        public void RemapRequestUri()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            DataServiceQuery customQuery = context.CreateQuery<Customer>("RemapPath");
            //Path should remap to the customers set

            var retrievedCustomers = (IEnumerable<Customer>)customQuery.Execute();
            Assert.NotNull(retrievedCustomers);
        }

        [Fact]
        public void RemapBase()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            DataServiceQuery customQuery = context.CreateQuery<Customer>("RemapBase");
            //Path should remap to the customers set
            var retrievedPersons = (IEnumerable<Customer>)customQuery.Execute();

            Assert.NotNull(retrievedPersons);
            EntityDescriptor descriptor = null;
            foreach (var element in retrievedPersons)
            {
                descriptor = context.GetEntityDescriptor(element);
                Assert.True(descriptor.EditLink.AbsoluteUri.StartsWith("http://potato"));
            }
        }

        [Fact]
        public void RemapBaseAndPathSeparately()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            DataServiceQuery customQuery = context.CreateQuery<Customer>("RemapBaseAndPathSeparately");
            //Path should remap to the customers set
            var retrievedPersons = (IEnumerable<Customer>)customQuery.Execute();

            Assert.NotNull(retrievedPersons);
            EntityDescriptor descriptor = null;
            foreach (var element in retrievedPersons)
            {
                descriptor = context.GetEntityDescriptor(element);
                Assert.True(descriptor.EditLink.AbsoluteUri.StartsWith("http://potato"));
            }
        }

        [Fact]
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
                Assert.True(false, "Different service bases between service uri and request uri should fail");
            }
            catch (DataServiceQueryException ex)
            {
                //No inner exception found
                Assert.NotNull(ex.InnerException);
                //Unexpected inner exception type
                Assert.IsType<DataServiceClientException>(ex.InnerException);
                StringResourceUtil.VerifyODataLibString(ClientExceptionUtil.ExtractServerErrorMessage(ex),
                    "UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri", true, expectedRequestUrl, expectedServiceUrl);
            }
        }

        [Fact]
        public void BatchRequest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            //Setup queries
            DataServiceRequest[] reqs = new DataServiceRequest[] {
                context.CreateQuery<Customer>("BatchRequest1"),
                context.CreateQuery<Person>("BatchRequest2"),
            };

            var response = context.ExecuteBatch(reqs);
            Assert.NotNull(response);
            Assert.True(response.IsBatchResponse);
            foreach (var item in response)
            {
                Assert.True(item.StatusCode == 200);
            }
        }


        [Fact]
        public void BatchRequestBaseUriDifferentBetweenBatchAndRequest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();

            //Setup queries
            DataServiceRequest[] reqs = new DataServiceRequest[] {
                context.CreateQuery<Customer>("BatchRequest3"),
            };

            var response = context.ExecuteBatch(reqs);
            Assert.NotNull(response);
            Assert.True(response.IsBatchResponse);
            foreach (QueryOperationResponse item in response)
            {
                Assert.NotNull(item.Error);

                //Unexpected inner exception type
                Assert.IsType<DataServiceClientException>(item.Error);
                var ex = item.Error as DataServiceClientException;
                StringResourceUtil.VerifyDataServicesString(ClientExceptionUtil.ExtractServerErrorMessage(item), "DataServiceOperationContext_CannotModifyServiceUriInsideBatch");
            }
        }
    }
}
