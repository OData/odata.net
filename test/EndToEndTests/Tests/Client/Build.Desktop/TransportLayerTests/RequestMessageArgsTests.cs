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
#if WIN8 || WINDOWSPHONE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class RequestMessageArgsTests : EndToEndTestBase
    {
        private IODataRequestMessage lastRequestMessage = null;

        public RequestMessageArgsTests() : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        public override void CustomTestInitialize()
        {
            this.lastRequestMessage = null;
            base.CustomTestInitialize();
        }

        [TestMethod]
        public void AddCustomHeader()
        {
            string headerName = "MyNewHeader";
            string headerValue = "MyNewHeaderValue";

            Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> addHeader = 
                (args) =>
                {
                    args.Headers.Add(headerName, headerValue);
                    return new HttpWebRequestMessage(args);
                };

            var ctx = this.CreateContext(addHeader);
            var query = ctx.Context.Product as DataServiceQuery<Product>;
            var ar = query.BeginExecute(null, null).EnqueueWait(this);
            query.EndExecute(ar);

            Assert.IsNotNull(this.lastRequestMessage, "No request sent");
            var header = this.lastRequestMessage.Headers.SingleOrDefault(h => h.Key == headerName);
            Assert.IsNotNull(header, "Custom header not sent");
            Assert.AreEqual(headerValue, header.Value, "Header value incorrect");
        }

        [TestMethod]
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

                        return new HttpWebRequestMessage(newArgs);
                    }

                    return new HttpWebRequestMessage(args);
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

            Assert.IsNotNull(this.lastRequestMessage, "No request sent");
            Assert.AreEqual("PATCH", this.lastRequestMessage.Method);
        }

        [TestMethod]
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

                        return new HttpWebRequestMessage(newArgs);
                    }
                    return new HttpWebRequestMessage(args);
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
                Assert.Fail("Expected error not thrown");
            }
            catch (DataServiceRequestException e)
            {
#if !PORTABLELIB && !SILVERLIGHT
                StringResourceUtil.VerifyDataServicesClientString(e.Message, "DataServiceException_GeneralError");
#else
                Assert.IsNotNull(e);
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
