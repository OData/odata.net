using FluentAssertions;
using Microsoft.Test.OData.Services.TestServices.ActionOverloadingServiceReference;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers
{
    public class VerificationController
    {
        private readonly IODataClientFactory<DefaultContainer> factory;
        private readonly VerificationCounter counter;

        public VerificationController(VerificationCounter counter, IODataClientFactory<DefaultContainer> odataClientFactory)
        {
            this.factory = odataClientFactory;
            this.counter = counter;
        }

        internal void TestHappyCase()
        {
            counter.ODataInvokeCount.Should().Be(0);
            var client = factory.CreateClient("Verification");

            counter.ODataInvokeCount.Should().Be(1);
            counter.HttpInvokeCount.Should().Be(0);

            client.SaveChangesAsync();

            counter.ODataInvokeCount.Should().Be(1);
            counter.HttpInvokeCount.Should().Be(1);
        }
    }
}
