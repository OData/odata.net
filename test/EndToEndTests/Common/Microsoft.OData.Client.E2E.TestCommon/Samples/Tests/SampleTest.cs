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
                    .AddRouteComponents("sample", SampleEdmModel.GetEdmModel()));
            }
        }
        public SampleTest(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
        }

        [Fact(Skip = "E2E test depend on Microsoft.AspNetCore.OData library. Test currently fails because #2916 changes ODataResource.Properties property type to IEnumerable<ODataPropertyInfo> and the dependent library is not updated.")]
        public void ExampleTest()
        {
            Uri uri = new Uri(Client.BaseAddress, "sample");
            var context = new Default.Container(uri);
            context.HttpClientFactory = HttpClientFactory;
            var product = context.Products.Execute().FirstOrDefault(a=>a.ProductId == -10);

            Assert.NotNull(product);
        }
    }
}
