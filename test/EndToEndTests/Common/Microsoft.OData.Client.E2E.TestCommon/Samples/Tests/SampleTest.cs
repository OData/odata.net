//-----------------------------------------------------------------------------
// <copyright file="SampleTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon.Samples.Server;
using Xunit;

namespace Microsoft.OData.Client.E2E.TestCommon.Samples.Tests
{
    public class SampleTest : EndToEndTestBase<SampleTest.TestsStartup>
    {
        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(SampleController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("actionresult", SampleEdmModel.GetEdmModel()));
            }
        }
        public SampleTest(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
        }

        [Fact]
        public void ExampleTest()
        {
            Uri uri = new Uri(Client.BaseAddress, "actionresult");
            var context = new Default.Container(uri);
            context.HttpClientFactory = HttpClientFactory;
            var product = context?.Products.Execute().FirstOrDefault(a=>a.ProductId == -10);

            Assert.NotNull(product);
        }
    }
}
