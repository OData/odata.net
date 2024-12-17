//---------------------------------------------------------------------
// <copyright file="ExpandQueryOptionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Server;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Tests
{
    public class QueryOptionOnCollectionTypePropertyTests : EndToEndTestBase<QueryOptionOnCollectionTypePropertyTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(QueryOptionTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt =>
                    opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        protected QueryOptionOnCollectionTypePropertyTests(TestWebApplicationFactory<QueryOptionOnCollectionTypePropertyTests.TestsStartup> factory) 
            : base(factory)
        {
            if (Client.BaseAddress == null)
            {
                throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
            }

            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        #region $skip



        #endregion

        #region Private methods

        private QueryOptionTestsHelper TestsHelper
        {
            get
            {
                return new QueryOptionTestsHelper(_baseUri, _model, Client);
            }
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "queryoption/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }

        #endregion
    }
}
