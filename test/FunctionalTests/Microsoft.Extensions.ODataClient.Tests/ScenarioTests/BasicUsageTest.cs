using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers;
using Microsoft.Test.OData.Services.TestServices.ActionOverloadingServiceReference;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore.ScenarioTests
{
    public class BasicUsageTest
    {
        [Fact]
        public void TestHappyCase()
        {
            ServiceCollection sc = new ServiceCollection();
            var startup = new Startup();
            var sp = startup.ConfigureServices(sc);
            var controller = sp.GetRequiredService<VerificationController>();
            controller.TestHappyCase();
        }
    }
}
