//-----------------------------------------------------------------------------
// <copyright file="TestStartupBase.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.OData.Client.E2E.TestCommon
{
    /// <summary>
    /// The startup base class
    /// </summary>
    public class TestStartupBase
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureBeforeRouting(app, env);

            app.UseODataBatching();

            app.UseRouting();

            ConfigureInRouting(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual void ConfigureBeforeRouting(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        protected virtual void ConfigureInRouting(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
