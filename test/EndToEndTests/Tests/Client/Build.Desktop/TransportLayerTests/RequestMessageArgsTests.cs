//---------------------------------------------------------------------
// <copyright file="RequestMessageArgsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.TransportLayerTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Xunit.Abstractions;
    using Xunit;

    public class RequestMessageArgsTests : EndToEndTestBase
    {
        private IODataRequestMessage lastRequestMessage = null;

        public RequestMessageArgsTests(ITestOutputHelper helper) : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

        public override void CustomTestInitialize()
        {
            this.lastRequestMessage = null;
            base.CustomTestInitialize();
        }

        [Fact]
        public void AddCustomHeader()
        {
            string headerName = "MyNewHeader";
            string headerValue = "MyNewHeaderValue";

            Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> addHeader = 
                (args) =>
                {
                    args.Headers.Add(headerName, headerValue);
                    return new Microsoft.OData.Client.HttpClientRequestMessage(args);
                };

            var ctx = this.CreateContext(addHeader);
            var query = ctx.Context.Product as DataServiceQuery<Product>;
            var ar = query.BeginExecute(null, null).EnqueueWait(this);
            query.EndExecute(ar);

            Assert.NotNull(this.lastRequestMessage);
            var header = this.lastRequestMessage.Headers.SingleOrDefault(h => h.Key == headerName);
            Assert.NotNull(header);
            Assert.Equal(headerValue, header.Value);
        }

        [Fact]
        public void ChangeMergeToPatch()
        {
            Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> addHeader = 
                (args) =>
                {
                    if (args.Method == "MERGE")
                    {
                        var newArgs = new DataServiceClientRequestMessageArgs("PATCH", args.RequestUri, true, args.UsePostTunneling, args.Headers);

                        // PATCH verb not supported in V1 or V2
                        newArgs.Headers.Remove("DataServiceVersion");
                        newArgs.Headers.Add("DataServiceVersion", "3.0");

                        return new Microsoft.OData.Client.HttpClientRequestMessage(newArgs);
                    }

                    return new Microsoft.OData.Client.HttpClientRequestMessage(args);
                };

            var ctx = this.CreateContext(addHeader);

            Product product = null;
            var query = ctx.Context.Product.Take(1) as DataServiceQuery<Product>;
            var ar = query.BeginExecute(null, null).EnqueueWait(this);
            product = query.EndExecute(ar).Single();

            this.lastRequestMessage = null;

            product.Description = "New Description " + Guid.NewGuid().ToString();
            ctx.UpdateObject(product);

            var ar2 = ctx.BeginSaveChanges(null, null).EnqueueWait(this);
            ctx.EndSaveChanges(ar2);

            Assert.NotNull(this.lastRequestMessage);
            Assert.Equal("PATCH", this.lastRequestMessage.Method);
        }

        [Fact]
        public void HttpMergeIsNoLongerSupported()
        {
            Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> addHeader =
                (args) =>
                {
                    if (args.Method == "PATCH")
                    {
                        // use Merge
                        var newArgs = new DataServiceClientRequestMessageArgs("MERGE", args.RequestUri, true, args.UsePostTunneling, args.Headers);
                        // use V4 since Merge is removed only in V4
                        newArgs.Headers["DataServiceVersion"] = "4.0";

                        return new Microsoft.OData.Client.HttpClientRequestMessage(newArgs);
                    }
                    return new Microsoft.OData.Client.HttpClientRequestMessage(args);
                };

            var ctx = this.CreateContext(addHeader);

            Product product = null;
            var query = ctx.Context.Product.Take(1) as DataServiceQuery<Product>;
            var ar = query.BeginExecute(null, null).EnqueueWait(this);
            product = query.EndExecute(ar).Single();

            product.Description = "New Description " + Guid.NewGuid().ToString();
            ctx.UpdateObject(product);

            try
            {
                var ar2 = ctx.BeginSaveChanges(null, null).EnqueueWait(this);
                ctx.EndSaveChanges(ar2);
                Assert.True(false, "Expected error not thrown");
            }
            catch (DataServiceRequestException e)
            {
#if !PORTABLELIB && !SILVERLIGHT
                StringResourceUtil.VerifyDataServicesClientString(e.Message, "DataServiceException_GeneralError");
#else
                Assert.NotNull(e);
#endif
            }
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext(Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> onMessageCreate)
        {
            var context = base.CreateWrappedContext<DefaultContainer>();
            context.SendingRequest2 += (obj, args) => this.lastRequestMessage = args.RequestMessage;
            context.Configurations.RequestPipeline.OnMessageCreating = onMessageCreate;
            return context;
        }
    }
}
