//-----------------------------------------------------------------------------
// <copyright file="AsyncRequestTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Server;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Tests
{
    public class AsyncRequestTests : EndToEndTestBase<AsyncRequestTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private IEdmModel _model = null;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(PeopleController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", AsyncRequestTestsEdmModel.GetEdmModel()));
            }
        }

        public AsyncRequestTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _model = AsyncRequestTestsEdmModel.GetEdmModel();
        }

        protected readonly string[] mimeTypes =
            [
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            ];
    }
}
