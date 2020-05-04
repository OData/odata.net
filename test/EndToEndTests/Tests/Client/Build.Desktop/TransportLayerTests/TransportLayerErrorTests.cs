//---------------------------------------------------------------------
// <copyright file="TransportLayerErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.TransportLayerTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultWithAccessRestrictionsServiceReference;
    using Xunit.Abstractions;
    using Xunit;

    public class TransportLayerErrorTests : EndToEndTestBase
    {
        private int lastResponseStatusCode;

        public TransportLayerErrorTests(ITestOutputHelper helper) : base(ServiceDescriptors.AstoriaDefaultWithAccessRestrictions, helper)
        {
        }

        [Fact]
        public void QueryWithInvalidUri()
        {
            this.CompareErrors((ctx) => ctx.Execute<Product>(new Uri("http://var1.svc/Products")));
        }

        [Fact]
        public void QueryForEntryWithInvalidKey()
        {
            this.CompareErrors((ctx) => ctx.Context.MessageAttachment.Where(ma => ma.AttachmentId == Guid.NewGuid()).ToList());
        }

        [Fact]
        public void JsonQueryWithInvalidDataServiceVersion()
        {
            var defaultContext = this.CreateDefaultContext();
            var httpClientContext = this.CreateHttpClientBasedContext();
            defaultContext.SendingRequest2 += SetInvalidDataServiceVersion;
            httpClientContext.SendingRequest2 += SetInvalidDataServiceVersion;

            this.CompareErrors(
                new[] { defaultContext, httpClientContext }, 
                (ctx) =>
                {
                    ctx.Format.UseJson();
                    ctx.Context.Customer.ToList();
                });
        }

        [Fact]
        public void QueryEntitySetWithNoAccess()
        {
            this.CompareErrors((ctx) => ctx.Context.MappedEntityType.ToList());
        }

        [Fact]
        public void WriteToEntitySetWithNoRights()
        {
            this.CompareErrors(
                (ctx) =>
                {
                    ctx.AddObject("Message", new Message {MessageId = 4638, Body = "Var1"});
                    ctx.SaveChanges();
                });
        }

        private static void  SetInvalidDataServiceVersion(object obj, SendingRequest2EventArgs args)
        {
            args.RequestMessage.SetHeader("DataServiceVersion", "99.99;NetFx");
        }

        private static void AssertExceptionsEqual(Exception expected, Exception actual)
        {
            Assert.Equal(expected.GetType(), actual.GetType());

            if (expected.InnerException == null)
            {
                Assert.Null(actual.InnerException); 
            }
            else
            {
                Assert.NotNull(actual.InnerException); 
                AssertExceptionsEqual(expected.InnerException, actual.InnerException); 
            }
        }

        private void CompareErrors(Action<DataServiceContextWrapper<DefaultContainer>> test)
        {
            CompareErrors(new[] {this.CreateDefaultContext(), this.CreateHttpClientBasedContext()}, test);
        }

        private void CompareErrors(DataServiceContextWrapper<DefaultContainer>[] contexts, Action<DataServiceContextWrapper<DefaultContainer>> test)
        {
            Debug.Assert(contexts.Length == 2);

            var results = new ResponseDetails[2];
            int i = 0;

            foreach (var context in contexts)
            {
                var responseDetails = new ResponseDetails();

                try
                {
                    test(context);
                }
                catch (Exception ex)
                {
                    responseDetails.Exception = ex;
                }

                Assert.NotNull(responseDetails.Exception);
                responseDetails.StatusCode = this.lastResponseStatusCode;
                results[i++] = responseDetails;
            }

            Assert.Equal(results[0].StatusCode, results[1].StatusCode);
            AssertExceptionsEqual(results[0].Exception, results[1].Exception);
        }

        private DataServiceContextWrapper<DefaultContainer> CreateHttpClientBasedContext()
        {
            var wrappedContext = this.CreateDefaultContext();
            wrappedContext.Configurations.RequestPipeline.OnMessageCreating = 
                (args) =>
                {
                    var message = new HttpClientRequestMessage(args.ActualMethod) {Url = args.RequestUri, Method = args.Method,};
                    foreach (var header in args.Headers)
                    {
                        message.SetHeader(header.Key, header.Value); 
                    }

                    return message;
                };

            return wrappedContext;
        }
        
        private DataServiceContextWrapper<DefaultContainer> CreateDefaultContext()
        {
            var wrappedContext = this.CreateWrappedContext<DefaultContainer>();
            wrappedContext.ReceivingResponse += (sender, args) => { this.lastResponseStatusCode = args.ResponseMessage.StatusCode; };
            return wrappedContext;
        }

        private class ResponseDetails
        {
            public int StatusCode { get; set; }
            public Exception Exception { get; set; } 
        }
    }
}
