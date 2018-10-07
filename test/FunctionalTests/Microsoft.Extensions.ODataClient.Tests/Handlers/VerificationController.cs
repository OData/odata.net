//---------------------------------------------------------------------
// <copyright file="VerificationController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Test.OData.Services.TestServices.ActionOverloadingServiceReference;
using System;
using System.Threading.Tasks;

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

        internal async Task TestHappyCase()
        {
            counter.ODataInvokeCount.Should().Be(0);
            var client = factory.CreateClient(new Uri("http://localhost"), "Verification");
            client.Configurations.Properties.Add("api-version", "1.0");

            counter.ODataInvokeCount.Should().Be(1);
            counter.HttpInvokeCount.Should().Be(0);

            Func<Task> task = async() => await client.Person.GetAllPagesAsync();
            task.ShouldThrow<InvalidOperationException>("No one is listen to that Url.");

            counter.ODataInvokeCount.Should().Be(1);
            counter.HttpInvokeCount.Should().Be(1);
        }
    }
}
