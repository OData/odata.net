//-----------------------------------------------------------------------------
// <copyright file="TestStartupBase.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.OData.E2E.TestCommon
{
    /// <summary>
    /// A base class for setting up and configuring an ASP.NET Core application's services and request pipeline.
    /// </summary>
    public class TestStartupBase
    {
        /// <summary>
        /// Configures the services for the application. 
        /// This method can be overridden in a derived class to register custom services.
        /// </summary>
        /// <param name="services">The service collection to which services are added.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// Configures the application's request pipeline.
        /// This method sets up middleware components that handle HTTP requests.
        /// It can be overridden in a derived class to customize the request pipeline.
        /// </summary>
        /// <param name="app">An application builder used to configure the request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureBeforeRouting(app, env);

            app.UseRouting();

            ConfigureInRouting(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // <summary>
        /// A method intended to be overridden to configure middleware before the routing middleware is added to the request pipeline.
        /// </summary>
        /// <param name="app">An application builder used to configure the request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment.</param>
        protected virtual void ConfigureBeforeRouting(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        /// <summary>
        /// A method intended to be overridden to configure middleware during the routing setup in the request pipeline.
        /// </summary>
        /// <param name="app">An application builder used to configure the request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment.</param>
        protected virtual void ConfigureInRouting(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
