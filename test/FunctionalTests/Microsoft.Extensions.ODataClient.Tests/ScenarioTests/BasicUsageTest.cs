//---------------------------------------------------------------------
// <copyright file="BasicUsageTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers;
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
